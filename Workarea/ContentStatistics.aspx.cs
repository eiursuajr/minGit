using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Community;

public partial class ContentStatistics : System.Web.UI.Page
{
    public ContentStatistics()
    {
        aFlags = (Ektron.Cms.ContentFlagData[])Array.CreateInstance(typeof(Ektron.Cms.ContentFlagData), 0);

    }
    protected ContentAPI _ContentAPI = new Ektron.Cms.ContentAPI();
    protected FlaggingAPI apiFlagging = new Ektron.Cms.Community.FlaggingAPI();
    protected string start_date = "";
    protected string end_date = "";
    protected string start_date2 = "";
    protected string end_date2 = "";
    protected CommonApi common;
    protected long contentid;
    protected string action;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper _MessageHelper;
    protected string ContentLanguage;
    protected ContentData content_data;
    protected string refUrl;
    protected System.Data.DataView ratingDataSource = new System.Data.DataView();
    protected Collection results;
    protected ContentFlagData[] aFlags;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        MessageBoard1.MaxResults = _ContentAPI.RequestInformationRef.PagingSize;
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        InitPage();
        RegisterResources();
        checkAccess();

        Literal1.Text = BuildDateSelectors(true);
        Literal2.Text = BuildDateSelectors(false);

        BuildToolBar();
        getResultBtn.Text = _MessageHelper.GetMessage("lbl get reviews");
        Button2.Text = _MessageHelper.GetMessage("lbl purge reviews");
        getFlagBtn.Text = _MessageHelper.GetMessage("lbl get flags");
        cmdFlags.Text = _MessageHelper.GetMessage("lbl purge flags");
        dialogOkButtonText.Text = _MessageHelper.GetMessage("lbl ok");
        dialogCancelButtonText.Text = _MessageHelper.GetMessage("generic cancel");
        closeDialogLink.Text = "<span class=\"ui-icon ui-icon-closethick\">" + _MessageHelper.GetMessage("close this dialog") + "</span>";
        confirmDialogHeader.Text = _MessageHelper.GetMessage("delete comment");
        confirmDeleteCommentMessage.Text = Ektron.Cms.API.JS.EscapeAndEncode(_MessageHelper.GetMessage("delete comment message"));
        jsAppPath.Text = Ektron.Cms.API.JS.Escape(_ContentAPI.AppPath);

        // If the page is a redirect from addeditcontentreview.aspx, it is because we displayed
        // the reviews and the user editted or deleted one of the reviews.
        // Display them again so the user cn see their changes.
        if (Request.QueryString["showReviews"] == "true")
        {
            try
            {
                object clickSender = DBNull.Value;
                System.EventArgs clickEvent = new System.EventArgs();
                getResultBtn_Click(clickSender, clickEvent);
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
    private void checkAccess()
    {
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (Convert.ToBoolean(_ContentAPI.RequestInformationRef.IsMembershipUser))
        {
            Response.Redirect((string)("reterror.aspx?info=" + _ContentAPI.EkMsgRef.GetMessage("msg login cms user")), false);
            return;
        }
    }

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, common.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronPlatformInfoJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDesignFormEntryJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, common.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncts");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);

    }

    private string SourcePage
    {
        get
        {
            return "content.aspx?action=View&folder_id=" + content_data.FolderId + "&id=" + content_data.Id + "&LangType=" + common.RequestInformationRef.ContentLanguage;
        }
    }

    protected void InitPage()
    {
        SetUpRating();

        if (refUrl == null)
        {
            refUrl = SourcePage;
        }

        if (!(Request.QueryString["redirect"] == null))
        {
            Response.Redirect(SourcePage);
        }

        if (GridView1.Rows.Count > 0)
        {
            Button1.Enabled = true;
            Button1.Visible = true;
        }
        else
        {
            Button1.Enabled = false;
            Button1.Visible = false;
        }

        _MessageHelper = common.EkMsgRef;

        DateTime today = DateTime.Today;
        try
        {
            ContentLanguage = System.Convert.ToString(Convert.ToInt32(Request.QueryString["LangType"]));
        }
        catch (Exception)
        {
            ContentLanguage = "-1";
        }

        if (!IsPostBack)
        {
            try
            {
                SetContentID();
                Collection res = _ContentAPI.GetContentRatingStatistics(contentid, 0, string.Empty);
                int total = Convert.ToInt32(res["total"]);
                if (total == 0)
                {
                    resultGraph.Text = "<b>" + _MessageHelper.GetMessage("content not rated") + "</b>";
                }
                else
                {
                    int sum = Convert.ToInt32(res["sum"]);
                    int[] r = (int[])Array.CreateInstance(typeof(int), 11);
                    int i;
                    for (i = 0; i <= 10; i++)
                    {
                        r[i] = Convert.ToInt32(res["r" + i]);
                    }
                    totalResults.Text = "<b>" + _MessageHelper.GetMessage("rating label") + ":</b> " + (total > 0 ? (Math.Round(Convert.ToDecimal(sum / total)) / 2) : 0) + " out of 5 Stars - " + total + " " + _MessageHelper.GetMessage("total ratings level");
                    DrawGraph(r);
                }
            }
            catch (Exception)
            {

            }

            Button2.Attributes.Add("onClick", "var arr=confirm(\'" + _MessageHelper.GetMessage("alert msg purge data") + "\'); if( arr ) { return true; } else { return false; }");
            this.cmdFlags.Attributes.Add("onClick", "var arr=confirm(\'" + _MessageHelper.GetMessage("alert msg purge data") + "\'); if( arr ) { return true; } else { return false; }");
        }
    }

    #region Content Flagging

    protected void getFlagBtn_Click(object sender, System.EventArgs e)
    {
        refUrl = Request.Url.OriginalString;
        BuildToolBar();

        SetContentID();
        DefineFlagData();

        if (aFlags.Length == 0)
        {
            no_results_lbl2.Text = _MessageHelper.GetMessage("lbl no flags") + "<br />";
        }
        else
        {
            LoadContentFlags(aFlags);
            no_results_lbl2.Text = string.Empty;
        }

        try
        {
            int pageSize = this._ContentAPI.RequestInformationRef.PagingSize;
            dg_flag.PageSize = pageSize;
        }
        catch (Exception)
        {
            dg_flag.PageSize = 50;
        }

        SelectedTab.Text = "dvFlagging";
    }

    private void DefineFlagData()
    {
        GridView1.Visible = false;
        Button1.Visible = false;
        dg_flag.Visible = true;
        int iTotalCF = 0;

        DateTime pStartDate;
        DateTime pEndDate;

        if (start_date2 == "")
        {
            pStartDate = new DateTime(1753, 1, 1);
        }
        else
        {
            pStartDate = DateTime.Parse(start_date2);
        }

        if (end_date2 == "")
        {
            pEndDate = DateTime.MaxValue;
        }
        else
        {
            pEndDate = DateTime.Parse(end_date2);
        }

        aFlags = apiFlagging.GetAllFlagEntries(contentid, 0, pStartDate, pEndDate, ref iTotalCF);
    }

    private void LoadContentFlags(ContentFlagData[] FlagList)
    {
        dg_flag.DataSource = this.CreateContentFlagSource(FlagList);
        dg_flag.CellPadding = 3;

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = _MessageHelper.GetMessage("display name label");
        //colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dg_flag.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATETIME";
        colBound.HeaderText = _MessageHelper.GetMessage("generic date no colon");
        //colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dg_flag.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FLAG";
        colBound.HeaderText = _MessageHelper.GetMessage("flag label");
        //colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dg_flag.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COMMENTS";
        colBound.HeaderText = _MessageHelper.GetMessage("comment text");
        //colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = true;
        dg_flag.Columns.Add(colBound);

        dg_flag.DataBind();
    }

    private ICollection CreateContentFlagSource(ContentFlagData[] FlagList)
    {
        DataTable dt = new DataTable();
        DataRow dr;
        EkDTSelector dtS;
        dtS = common.EkDTSelectorRef;
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DATETIME", typeof(string)));
        dt.Columns.Add(new DataColumn("FLAG", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMENTS", typeof(string)));

        for (int i = 0; i <= (FlagList.Length - 1); i++)
        {
            dr = dt.NewRow();
            if (FlagList[i].FlaggedUser.Id == 0)
            {
                dr[0] = "<font color=\"gray\">" + this._MessageHelper.GetMessage("lbl anon") + "</font>";
            }
            else
            {
                dr[0] = FlagList[i].FlaggedUser.DisplayName;
            }
            dr[1] = GetFlagEditURL(FlagList[i].FlagId, Convert.ToString(FlagList[i].FlagDate));
            dr[2] = FlagList[i].FlagName;
            dr[3] = FlagList[i].FlagComment;
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        return dv;
    }

    protected string GetFlagEditURL(long contentflagID, string flag_date)
    {
        return "<a href=\"ContentFlagging/addeditcontentflag.aspx?action=view&id=" + contentflagID.ToString() + "&cid=" + contentid.ToString() + "\">" + Convert.ToDateTime(flag_date).ToShortDateString() + "</a>";
    }

    protected void cmdFlags_Click(object sender, System.EventArgs e)
    {
        refUrl = Request.Url.OriginalString;
        BuildToolBar();
        DateTime pStartDate;
        DateTime pEndDate;

        if (start_date2 == "")
        {
            pStartDate = new DateTime(1753, 1, 1);
        }
        else
        {
            pStartDate = DateTime.Parse(start_date2);
        }

        if (end_date2 == "")
        {
            pEndDate = DateTime.MaxValue;
        }
        else
        {
            pEndDate = DateTime.Parse(end_date2);
        }

        apiFlagging.PurgeFlagEntries(this.contentid, pStartDate, pEndDate);

        SelectedTab.Text = "dvFlagging";
    }

    #endregion

    private string GetPurgeValidationMessage()
    {
        StringBuilder str = new StringBuilder();
        str.Append("Are you sure you want to purge all content");
        if (this.start_date == "[None]")
        {
            str.Append("up until " + this.end_date);
        }
        else
        {
            str.Append("from " + this.start_date + " to " + this.end_date + "?");
        }
        return str.ToString();
    }

    private void SetUpRating()
    {
        SetContentID();
        common = new Ektron.Cms.CommonApi();
    }

    private void SetContentID()
    {
        if (Request.QueryString["id"] != "")
        {
            try
            {
                contentid = Convert.ToInt64(Request.QueryString["id"]);
            }
            catch (Exception)
            {
                contentid = 0;
            }
        }
        else
        {
            contentid = 0;
        }
        try
        {
            content_data = _ContentAPI.GetContentById(contentid, 0);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + ex.Message.ToString()), true);
        }
    }

    private string BuildDateSelectors(bool IsDisplayRating)
    {
        StringBuilder sbHtml = new StringBuilder();
        Ektron.Cms.EkDTSelector dateSchedule = new Ektron.Cms.EkDTSelector(common.RequestInformationRef);
        string StartElementID = "start_date";
        string EndElementID = "end_date";
        if (!IsDisplayRating)
        {
            StartElementID = "start_date2";
            EndElementID = "end_date2";
        }
        try
        {
            if (Request.Form[StartElementID + "_iso"] != "")
            {
                if (!IsDisplayRating)
                {
                    start_date2 = (string)(Request.Form[StartElementID + "_iso"] + " " + Request.Form[StartElementID + "_hr"] + ":" + Request.Form[StartElementID + "_mi"]);
                }
                else
                {
                    start_date = (string)(Request.Form[StartElementID + "_iso"] + " " + Request.Form[StartElementID + "_hr"] + ":" + Request.Form[StartElementID + "_mi"]);
                }
            }
        }
        catch (Exception)
        {
            start_date = "";
        }


        try
        {
            if (Request.Form[EndElementID + "_iso"] != "")
            {
                if (!IsDisplayRating)
                {
                    end_date2 = (string)(Request.Form[EndElementID + "_iso"] + " " + Request.Form[EndElementID + "_hr"] + ":" + Request.Form[EndElementID + "_mi"]);
                }
                else
                {
                    end_date = (string)(Request.Form[EndElementID + "_iso"] + " " + Request.Form[EndElementID + "_hr"] + ":" + Request.Form[EndElementID + "_mi"]);
                }
            }
        }
        catch (Exception)
        {
            end_date = "";
        }

        sbHtml.Append("<table class=\"ektronGrid\">");
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">");
        sbHtml.Append(_MessageHelper.GetMessage("generic start date label"));
        sbHtml.Append("</td>");
        sbHtml.Append("<td class=\"value\">");
        dateSchedule.formName = "form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = StartElementID;
        dateSchedule.spanId = StartElementID + "_span";
        if (start_date != "")
        {
            try
            {
                dateSchedule.targetDate = DateTime.Parse(start_date);
            }
            catch (Exception)
            {
                start_date = "";
            }
        }
        if (start_date2 != "")
        {
            try
            {
                dateSchedule.targetDate = DateTime.Parse(start_date2);
            }
            catch (Exception)
            {
                start_date2 = "";
            }
        }

        sbHtml.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">");
        sbHtml.Append(_MessageHelper.GetMessage("generic end date label"));
        sbHtml.Append("</td>");
        sbHtml.Append("<td class=\"value\">");
        dateSchedule = new Ektron.Cms.EkDTSelector(common.RequestInformationRef);
        dateSchedule.formName = "form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = EndElementID;
        dateSchedule.spanId = EndElementID + "_span";
        if (end_date != "")
        {
            try
            {
                dateSchedule.targetDate = DateTime.Parse(end_date);
            }
            catch (Exception)
            {
                end_date = "";
            }
        }
        if (end_date2 != "")
        {
            try
            {
                dateSchedule.targetDate = DateTime.Parse(end_date2);
            }
            catch (Exception)
            {
                end_date2 = "";
            }

        }

        sbHtml.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("</table>");
        return sbHtml.ToString();

    }

    private void DrawGraph(int[] r)
    {
        resultGraph.Text = "<img src=\"" + common.ApplicationPath + "ContentRatingGraph.aspx?stars=true&";
        int i;
        for (i = 0; i <= 10; i++)
        {
            resultGraph.Text += "r" + (i) + "=" + r[i] + "&";
        }
        resultGraph.Text += "\" />";
    }

    protected void getResultBtn_Click(object sender, System.EventArgs e)
    {
        DefineData();
        refUrl = Request.Url.OriginalString;
        BuildToolBar();
        totalResults.Text = ""; // clear as we're getting some new results now...

        int index;
        for (index = 0; index <= GridView1.Columns.Count - 1; index++)
        {
            GridView1.Columns[index].HeaderStyle.CssClass = "title-header";
        }

        try
        {
            int pageSize = this._ContentAPI.RequestInformationRef.PagingSize;
            GridView1.PageSize = pageSize;
        }
        catch (Exception)
        {
            GridView1.PageSize = 50;
        }

        SetContentID();
        // ratings
        int i;
        int[] r = (int[])Array.CreateInstance(typeof(int), 11);

        if (GridView1.Rows.Count == 0)
        {
            no_results_lbl.Text = _MessageHelper.GetMessage("lbl no comments");
        }
        else
        {
            no_results_lbl.Text = string.Empty;
        }

        if (GridView1.Rows.Count > 0)
        {
            Button1.Enabled = true;
            Button1.Visible = true;
        }
        else
        {
            Button1.Enabled = false;
            Button1.Visible = false;
        }

        int rating;
        DefineResultsData();
        int total = results.Count;
        int sum = 0;
        for (i = 0; i <= 10; i++)
        {
            r[i] = 0;
        }
        for (i = 1; i <= results.Count; i++)
        {
            Collection row = (Collection)results[i];
            rating = Convert.ToInt32(row["user_rating"]);
            r[rating] = System.Convert.ToInt32(r[rating] + 1);
            sum = sum + rating;
        }

        if (total == 0)
        {
            resultGraph.Text = "<b>" + _MessageHelper.GetMessage("no ratings timeline") + "</b>";
            totalResults.Text = "";
        }
        else
        {
            totalResults.Text = "<b>" + _MessageHelper.GetMessage("rating label") + ":</b> " + (total > 0 ? (Math.Round(Convert.ToDecimal(sum / total)) / 2) : 0) + " out of 5 Stars - " + total + " " + _MessageHelper.GetMessage("total ratings level");
            DrawGraph(r);
        }
    }

    public void GridView1_RowDataBound(System.Object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        string sortBy = _MessageHelper.GetMessage("sort by x");
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                // cell 0 - command links
                // cell 1 - username
                // cell 2 - date
                // cell 3 - individual rating
                // cell 4 - comment
                // cell 5 - comment status

                // provide translated text for Header Components
                ((LinkButton)(e.Row.Cells[1].FindControl("userNameHeader"))).Text = _MessageHelper.GetMessage("generic username");
                ((LinkButton)(e.Row.Cells[1].FindControl("userNameHeader"))).ToolTip = string.Format(sortBy, _MessageHelper.GetMessage("generic username"));
                ((LinkButton)(e.Row.Cells[1].FindControl("dateHeader"))).Text = _MessageHelper.GetMessage("lbl generic date");
                ((LinkButton)(e.Row.Cells[1].FindControl("dateHeader"))).ToolTip = string.Format(sortBy, _MessageHelper.GetMessage("lbl generic date"));
                break;

            case DataControlRowType.DataRow:
                //((System.Data.DataRowView)(contentRating)).Row.ItemArray[0]
                object contentRating = e.Row.DataItem;
                string contentRatingUser = EkFunctions.ReadDbString(((System.Data.DataRowView)(contentRating)).Row.ItemArray[0]);
                string contentRatingId = (string)(((System.Data.DataRowView)(contentRating)).Row.ItemArray[1].ToString());
                string contentRatingDate = (string)(Convert.ToDateTime(((System.Data.DataRowView)(contentRating)).Row.ItemArray[2]).ToShortDateString());
                string contentRatingComment = (string)(((System.Data.DataRowView)(contentRating)).Row.ItemArray[5].ToString());
                string redirectUrl = Request.Url.ToString();
                if (!redirectUrl.Contains("showReviews"))
                {
                    redirectUrl = redirectUrl + "&showReviews=true";
                }
                // provide translated text for individual items
                ((LinkButton)(e.Row.Cells[0].FindControl("editCommentLink"))).Text = _MessageHelper.GetMessage("lbl edit comment");
                ((LinkButton)(e.Row.Cells[0].FindControl("editCommentLink"))).ToolTip = _MessageHelper.GetMessage("lbl edit comment");
                ((LinkButton)(e.Row.Cells[0].FindControl("editCommentLink"))).OnClientClick = "window.location = \'" + Ektron.Cms.API.JS.Escape(_ContentAPI.AppPath + "addeditcontentreview.aspx?action=edit&id=" + contentRatingId + "&cid=" + contentid + "&page=workarea&redirectUrl=" + EkFunctions.UrlEncode(redirectUrl)) + "\'; return false;";
                ((LinkButton)(e.Row.Cells[0].FindControl("deleteCommentLink"))).Text = _MessageHelper.GetMessage("delete comment");
                ((LinkButton)(e.Row.Cells[0].FindControl("deleteCommentLink"))).ToolTip = _MessageHelper.GetMessage("delete comment");

                ((LinkButton)(e.Row.Cells[0].FindControl("deleteCommentLink"))).OnClientClick = "deleteContentRatingPrompt({ratingId: " + contentRatingId + ", contentId: " + contentid + ", user: \'" + contentRatingUser + "\', date: \'" + contentRatingDate + "\', comment: \'" + Ektron.Cms.API.JS.EscapeAndEncode(Ektron.Cms.Common.EkConstants.UrlDecode(contentRatingComment)) + "\'}); return false;";
                break;
        }
    }

    private void DefineData()
    {
        DateTime pStartDate;
        DateTime pEndDate;

        if (start_date == "")
        {
            pStartDate = new DateTime(1753, 1, 1);
            start_date2 = "";
        }
        else
        {
            pStartDate = DateTime.Parse(start_date);
            start_date2 = "";
        }

        if (end_date == "")
        {
            pEndDate = DateTime.MaxValue;
            end_date2 = "";
        }
        else
        {
            pEndDate = DateTime.Parse(end_date);
            end_date2 = "";
        }
        ratingDataSource.Table = _ContentAPI.GetContentRatingResults(contentid, pStartDate, pEndDate).Tables[0];
        GridView1.DataSource = ratingDataSource;

        GridView1.DataBind();
    }

    private void DefineResultsData()
    {
        GridView1.Visible = true;
        Button1.Visible = true;
        dg_flag.Visible = false;

        DateTime pStartDate;
        DateTime pEndDate;

        if (start_date == "")
        {
            pStartDate = new DateTime(1753, 1, 1);
            start_date2 = "";
        }
        else
        {
            pStartDate = DateTime.Parse(start_date);
            end_date2 = "";
        }

        if (end_date == "")
        {
            pEndDate = DateTime.MaxValue;
            end_date2 = "";
        }
        else
        {
            pEndDate = DateTime.Parse(end_date);
            end_date2 = "";
        }

        results = _ContentAPI.GetContentRatingStatistics(contentid, pStartDate, pEndDate);
    }

    private void BuildToolBar()
    {
        System.Text.StringBuilder result;
        result = new System.Text.StringBuilder();
        string AppImgPath = _ContentAPI.AppImgPath;
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(_MessageHelper.GetMessage("content report for") + " \"" + content_data.Title + "\""));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath + "images/UI/Icons/back.png", refUrl, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>" + m_refStyle.GetHelpButton("ContentStatistics", "") + "</td>");
        result.Append("<td>");
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        Page.Header.Controls.Add(new LiteralControl(m_refStyle.GetClientScript()));

    }

    protected void Button1_Click(object sender, System.EventArgs e)
    {
        //disable sorting and paging so it won't have the javascript link once it is in the spreadsheet.
        GridView1.AllowSorting = false;
        GridView1.AllowPaging = false;
        //after changing the properties of the GridView, it needs to be re-DataBind to take effect.
        GridView1.DataBind();
        Response.Clear();
        Response.AddHeader("content-disposition", "attachment;filename=" + content_data.Title + "_Ratings.xls");
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = "application/vnd.xls";
        System.IO.StringWriter stringWrite = new System.IO.StringWriter();
        HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
        DefineData();
        GridView1.DataSource = ratingDataSource;
        GridView1.DataBind();
        GridView1.RenderControl(htmlWrite);
        Response.Write(stringWrite.ToString());
        Response.End();

        GridView1.AllowSorting = true;
        GridView1.AllowPaging = true;
    }

    public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    {
       
    }

    protected void Button2_Click(object sender, System.EventArgs e)
    {
        refUrl = SourcePage;
        BuildToolBar();
        DateTime pStartDate;
        DateTime pEndDate;

        if (start_date == "")
        {
            pStartDate = new DateTime(1753, 1, 1);
        }
        else
        {
            pStartDate = DateTime.Parse(start_date);
        }

        if (end_date == "")
        {
            pEndDate = DateTime.MaxValue;
        }
        else
        {
            pEndDate = DateTime.Parse(end_date);
        }

        _ContentAPI.PurgeContentRatings(this.contentid, pStartDate, pEndDate);
        GridView1.Visible = false;
        Button1.Visible = false;
        dg_flag.Visible = false;
        resultGraph.Text = "<b>" + _MessageHelper.GetMessage("content not rated") + "</b>"; //resultGraph.Text = String.Empty
        totalResults.Text = string.Empty;
        no_results_lbl.Text = _MessageHelper.GetMessage("lbl no comments");
    }

    protected void GridView1_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
    {
        DefineData();
        if (SortOrder == SortDirection.Descending)
        {
            ratingDataSource.Sort = e.SortExpression + " DESC";
        }
        else
        {
            ratingDataSource.Sort = e.SortExpression + " ASC";
        }
        GridView1.DataSource = ratingDataSource;
        GridView1.DataBind();
    }

    protected void GridView1_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        DefineData();
        GridView1.PageIndex = e.NewPageIndex;
        GridView1.DataBind();
    }

    protected System.Web.UI.WebControls.SortDirection SortOrder
    {
        get
        {
            System.Web.UI.WebControls.SortDirection order;
            try
            {
                order = (System.Web.UI.WebControls.SortDirection)(ViewState["ek_SortOrder"]);
            }
            catch (Exception)
            {
                order = System.Web.UI.WebControls.SortDirection.Ascending;
                ViewState.Add("ek_SortOrder", order);
            }
            if (order == System.Web.UI.WebControls.SortDirection.Ascending)
            {
                order = System.Web.UI.WebControls.SortDirection.Descending;

            }
            else
            {
                order = System.Web.UI.WebControls.SortDirection.Ascending;
            }
            ViewState["ek_SortOrder"] = order;
            return order;
        }
    }

    protected string DisplayRatingStatus(int Status)
    {
        if (Status == 0)
        {
            return "<font color=\"orange\">Pending</font>";
        }
        else if (Status == 1)
        {
            return "<font color=\"green\">Approved</font>";
        }
        else if (Status == 2)
        {
            return "<font color=\"red\"><strong>Rejected</strong></font>";
        }
        else
        {
            return "<font color=\"yellow\">Pending</font>";
        }
    }
    protected string GetEditURL(long contentratingID, string rating_date)
    {
        return "<a href=\"addeditcontentreview.aspx?action=view&id=" + contentratingID.ToString() + "&cid=" + contentid.ToString() + "\">" + Convert.ToDateTime(rating_date).ToShortDateString() + "</a>";
    }
    protected string DisplayComments(string Comments)
    {
        return Server.HtmlDecode(Comments);
    }
    protected string GetUserName(object username)
    {
        if (Information.IsDBNull(username) || Strings.Trim(username.ToString()) == "")
        {
            return "<span color=\"gray\">" + _MessageHelper.GetMessage("lbl anon") + "</span>";
        }
        else
        {
            return Convert.ToString(username);
        }
    }
    public string GenerateStars(int irating)
    {
        StringBuilder sbRating = new StringBuilder();
        string strHttpType = string.Empty;
        strHttpType = System.Web.HttpContext.Current.Request.Url.Scheme + "://";
        for (int i = 1; i <= 10; i++)
        {
            sbRating.Append("<img border=\"0\" src=\"" + strHttpType + _ContentAPI.RequestInformationRef.HostUrl + _ContentAPI.AppPath + "images/ui/icons/");
            if (i % 2 > 0)
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyLeft.png");
                }
                else
                {
                    sbRating.Append("starLeft.png");
                }
            }
            else
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyRight.png");
                }
                else
                {
                    sbRating.Append("starRight.png");
                }
            }
            sbRating.Append("\" />");
        }
        return sbRating.ToString();
    }
}

