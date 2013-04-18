using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;
using System.Text;
using Ektron.DbSync.Core;

public partial class SyncHistory : System.Web.UI.Page, ISyncHistoryView
{
    private const string DataFieldLogId = "LogId";
    private const string DataFieldProfileName = "ProfileName";
    private const string DataFieldSyncType = "SyncType";
    private const string DataFieldSyncDirection = "SyncDirection";
    private const string DataFieldSyncEndTime = "SyncEndTime";
    private const string DataFieldSyncStartTime = "SyncStartTime";
    private const string DataFieldUserId = "UserId";
    private const string DataFieldAppliedChanges = "AppliedChanges";
    private const string DataFieldSkippedChanges = "SkippedChanges";
    private const string DataFieldTotalChanges = "TotalChanges";

    private const int ColumnLogId = 0;
    private const int ColumnProfileName = 1;
    private const int ColumnSyncType = 2;
    private const int ColumnSyncDirection = 3;
    private const int ColumnSyncEndTime = 4;
    private const int ColumnSyncStartTime = 5;
    private const int ColumnUserId = 6;
    private const int ColumnAppliedChanges = 7;
    private const int ColumnSkippedChanges = 8;
    private const int ColumnTotalChanges = 9;
    private const int ColumnStatus = 10;
    private const int ColumnRemote = 3;
    private const int ColumnLocal = 4;

    private const string LogLinkFormat = "SyncHistory.aspx?id={0}&referrer=SyncHistory.aspx";
    private const string SuccessImagePath = "images/ui/icons/check.png";
    private const string FailedImagePath = "images/ui/icons/error.png";
    private const string StatisticsFormat = "{0}: {1},  {2}: {3}, {4}: {5}";
    private const string ConflictDetailsFormat = "<a href=\"#\" onclick=\"showException();\">{0}</a>";


    private readonly SyncHistoryPresenter _presenter;
    private readonly CommonApi _commonApi;
    private readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;

    public SyncHistory()
    {
        _presenter = new SyncHistoryPresenter(this);
        _commonApi = new CommonApi();
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
    }

    public void Page_Init(object sender, EventArgs e)
    {
        Utilities.ValidateUserLogin();
        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        RegisterResources();

        gvLogData.RowDataBound += gvLogData_RowDataBound;
        grdSync.RowDataBound += grdSync_RowDataBound;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _presenter.Initialize();

        HistoryParameters parameters = new HistoryParameters(Request);
        RenderHeader(parameters);

        if (parameters.Id > 0 && parameters.Action == HistoryPresentationMode.Detail)
        {
            _presenter.ShowLog(parameters.Id);
        }
        else if (parameters.Action == HistoryPresentationMode.Resolve)
        {
            _presenter.ShowLog(parameters.Id);
            _presenter.ResolveConflictData(parameters.ProfileId);
        }


    }

    #region ISyncHistoryView Members

    public void DisplayList(List<SyncHistoryData> logs)
    {
        if (!IsPostBack)
        {
            pnlLogList.Visible = true;
            pnlLogDetails.Visible = false;

            BoundField columnLogID = CreateColumn(_siteApi.EkMsgRef.GetMessage("lbl Log ID"), DataFieldLogId);
            BoundField columnProfile = CreateColumn(_siteApi.EkMsgRef.GetMessage("lbl profile"), DataFieldProfileName);
            BoundField columnType = CreateColumn(_siteApi.EkMsgRef.GetMessage("lbl sync type"), DataFieldSyncType);
            BoundField columnDirection = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic Direction"), DataFieldSyncDirection);
            BoundField columnStartTime = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic start time"), DataFieldSyncStartTime);
            BoundField columnEndTime = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic end time"), DataFieldSyncEndTime);
            BoundField columnUserId = CreateColumn(_siteApi.EkMsgRef.GetMessage("lbl gateway userid"), DataFieldUserId);
            BoundField columnApplied = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic Applied"), DataFieldAppliedChanges);
            BoundField columnSkipped = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic Skipped"), DataFieldSkippedChanges);
            BoundField columnTotal = CreateColumn(_siteApi.EkMsgRef.GetMessage("lbl total"), DataFieldTotalChanges);
            BoundField columnStatus = CreateColumn(_siteApi.EkMsgRef.GetMessage("generic status"), DataFieldSkippedChanges);

            gvLogData.Columns.Add(columnLogID);
            gvLogData.Columns.Add(columnProfile);
            gvLogData.Columns.Add(columnType);
            gvLogData.Columns.Add(columnDirection);
            gvLogData.Columns.Add(columnStartTime);
            gvLogData.Columns.Add(columnEndTime);
            gvLogData.Columns.Add(columnUserId);
            gvLogData.Columns.Add(columnApplied);
            gvLogData.Columns.Add(columnSkipped);
            gvLogData.Columns.Add(columnTotal);
            gvLogData.Columns.Add(columnStatus);
        }

        gvLogData.DataSource = logs;
        gvLogData.DataBind();
    }

    public void DisplayLog(SyncHistoryData log)
    {
        pnlLogList.Visible = false;
        pnlLogDetails.Visible = true;
        ltrDCCMessage.Visible = false;

        cellTypeLabel.InnerText = _siteApi.EkMsgRef.GetMessage("type label");
        cellPositionLabel.InnerText = _siteApi.EkMsgRef.GetMessage("generic Position");
        cellStartTimeLabel.InnerText = _siteApi.EkMsgRef.GetMessage("generic start time")+":";
        cellEndTimeLabel.InnerText = _siteApi.EkMsgRef.GetMessage("generic end time")+":";
        cellStatisticsLabel.InnerText = _siteApi.EkMsgRef.GetMessage("lbl statistics")+":";
        cellReasonsLabel.InnerText = _siteApi.EkMsgRef.GetMessage("lbl Skipped Reasons");

        cellType.InnerText = GetFormattedSyncTypeString(log.SyncType);
        cellPosition.InnerText = GetFormattedSyncPositionString(log.SyncScopePosition);
        cellStartTime.InnerText = log.SyncStartTime.ToString();
        cellEndTime.InnerText = log.SyncEndTime.ToString();
        cellStatistics.InnerText = string.Format(
            StatisticsFormat,
            "Applied",
            log.AppliedChanges,
            "Skipped",
            log.SkippedChanges,
            "Total",
            log.TotalChanges);

        if (log.SkippedChanges > 0)
        {
            string conflictMessage = _presenter.GetConflictMessage(log);
            if (!string.IsNullOrEmpty(conflictMessage))
            {
                cellReasons.InnerHtml = string.Format(ConflictDetailsFormat, conflictMessage);
            }
            else
            {
                cellReasons.InnerHtml = string.Format(ConflictDetailsFormat, _siteApi.EkMsgRef.GetMessage("alert sync error"));
            }
        }
        else
        {
            cellReasons.InnerText = _siteApi.EkMsgRef.GetMessage("lbl No skipped changes");
        }

        if (log.ConflictData != null && log.ConflictData.Count > 0)
        {
            grdSync.DataSource = log.ConflictData;
            grdSync.DataBind();
        }

        if (log.ProfileId > 0 && log.ConflictData.Count > 0)
        {
            HtmlTableCell cellResolveButton = new HtmlTableCell();
            cellResolveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "cog.png",
                String.Format("synchistory.aspx?action=resolve&pid={0}&id={1}",log.ProfileId.ToString(), log.LogId.ToString()),
                _siteApi.EkMsgRef.GetMessage("resolve resolve button"),
                _siteApi.EkMsgRef.GetMessage("resolve resolve button"),
                string.Empty);
            rowToolbarButtons.Cells.Add(cellResolveButton);
        }
    }

    #endregion

    private void grdSync_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row != null &&
            e.Row.Cells != null &&
            e.Row.RowType == DataControlRowType.DataRow)
        {
            ConflictRow rowData = (ConflictRow)e.Row.DataItem;
            if (rowData.LocalChange.Count > 0)
            {
                e.Row.Cells[ColumnLocal].Text = GetFormattedChange(rowData.LocalChange);
            }
            if (rowData.RemoteChange.Count > 0)
            {
                e.Row.Cells[ColumnRemote].Text = GetFormattedChange(rowData.RemoteChange);
            }
        }
    }

    private void gvLogData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row != null &&
            e.Row.Cells != null &&
            e.Row.RowType == DataControlRowType.DataRow)
        {
            FormatSyncType(e.Row.Cells[ColumnSyncType]);
            FormatSyncDirection(e.Row.Cells[ColumnSyncDirection]);
            FormatSyncStatus(e.Row.Cells[ColumnStatus], e.Row.Cells[ColumnLogId].Text);
        }
    }

    protected void gvLogData_PagingIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvLogData.PageIndex = e.NewPageIndex;
        gvLogData.DataBind();
    }

    protected void grdSync_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdSync.PageIndex = e.NewPageIndex;
        grdSync.DataBind();

        Ektron.Cms.API.JS.RegisterJSBlock(this, "showException();", "SyncHistoryExpandReasons");
    }

    private void RegisterResources()
    {
        ektronClientScript.Text = _styleHelper.GetClientScript();

        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronToolBarRollJS);

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.Framework.UI.Css.Register(this, "css/ektron.workarea.sync.dialogs.css");

    }

    private void RenderHeader(HistoryParameters parameters)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("sync log history"));

        HtmlTableCell cellBackButton = new HtmlTableCell();

        string referrer = "Sync.aspx";
        if (!string.IsNullOrEmpty(parameters.Referrer))
        {
            referrer = parameters.Referrer;
        }

        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "back.png",
            referrer,
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            String.Empty,
            StyleHelper.BackButtonCssClass,
            true);

        rowToolbarButtons.Cells.Add(cellBackButton);


    }

    private BoundField CreateColumn(string header, string fieldName)
    {
        BoundField column = new BoundField();
        column.HeaderText = header;
        column.DataField = fieldName;

        return column;
    }

    private void FormatSyncType(TableCell cell)
    {
        cell.Text = GetFormattedSyncTypeString(cell.Text);
    }

    private void FormatSyncDirection(TableCell cell)
    {
        switch (cell.Text)
        {
            case "2":
                cell.Text = _siteApi.EkMsgRef.GetMessage("upload txt");
                break;
            case "3":
                cell.Text = _siteApi.EkMsgRef.GetMessage("btn download");
                break;
            default:
                cell.Text = _siteApi.EkMsgRef.GetMessage("lbl sync bidirectional");
                break;
        }
    }

    private void FormatSyncStatus(TableCell cell, string id)
    {
        LinkButton logLink = new LinkButton();
        logLink.PostBackUrl = string.Format(LogLinkFormat, id);

        Image logLinkImage = new Image();

        if (cell.Text == "0")
        {
            logLinkImage.ImageUrl = _commonApi.AppPath + SuccessImagePath;
        }
        else
        {
            logLinkImage.ImageUrl = _commonApi.AppPath + FailedImagePath;
        }

        logLink.Controls.Add(logLinkImage);
        cell.Controls.Add(logLink);
    }

    private string GetFormattedSyncTypeString(string value)
    {
        string formattedValue;

        switch (value)
        {
            case "FullSync":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Full Sync");
                break;
            case "FullStage":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Initial Sync");
                break;
            case "FolderSync":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Folder Sync");
                break;
            case "ContentSync":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Content Sync");
                break;
            case "DomainStage":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Domain Intial Sync");
                break;
            case "DomainSync":
                formattedValue = _siteApi.EkMsgRef.GetMessage("generic Domain Sync");
                break;
            default:
                formattedValue = value;
                break;
        }

        return formattedValue;
    }

    private string GetFormattedSyncPositionString(int value)
    {
        string formattedValue = string.Empty;

        if (value == 0)
        {
            formattedValue = _siteApi.EkMsgRef.GetMessage("generic lbl Sent");
        }
        else
        {
            formattedValue = _siteApi.EkMsgRef.GetMessage("generic Received");
        }

        return formattedValue;
    }

    private string GetFormattedChange(List<ConflictKeyValue> conflicts)
    {
        StringBuilder sb = new StringBuilder();

        if (conflicts != null && conflicts.Count > 0)
        {
            foreach (ConflictKeyValue conflict in conflicts)
            {
                sb.Append(conflict.ToString());
            }
        }

        return sb.ToString();
    }

    public void DisplayResolved(long resultCount)
    {
        ltrDCCMessage.Text = String.Format("<div class=\"success dcc\" >{0} " + _siteApi.EkMsgRef.GetMessage("lbl Data conflicts") + "</div>", resultCount);
        ltrDCCMessage.Visible = true;
    }
}
