using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Reporting;
using Ektron.Cms.Commerce.KPI;
using Ektron.Cms.Commerce.KPI.Provider;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;

public partial class Workarea_Widgets_ContentInWorkFlow : WorkareaWidgetBaseControl, IWidget
{
    #region properties
    Collection _data;
    private int _DaysLimit;

    [WidgetDataMember(7)]
    public int DaysLimit { get { return _DaysLimit; } set { _DaysLimit = value; } }
    #endregion

    #region Page Events
    /// <summary>
    /// Init Event
    /// </summary>
    /// <param name="e">Event Args</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        base.Host.Edit += new EditDelegate(EditEvent);
        base.Host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        base.Host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        base.Host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });

        ViewSet.SetActiveView(View);
    }
    #endregion

    #region Set Output
    /// <summary>
    /// Set Output
    /// </summary>
    protected void SetOutput()
    {

        Collection criteria = new Collection();
        criteria.Add("submitted", "StateWanted", null, null);
        criteria.Add("", "FilterType", null, null);
        criteria.Add(0, "FilterID", null, null);
        criteria.Add("", "OrderBy", null, null);

        _data = EkContentRef.GetContentReportv2_0(criteria);

        if (_data.Count > 0)
        {
            LoadData();
            grdData.Style.Add("display", "table");
            lblNoRecords.Visible = false;
            pnlData.Visible = true;
        }
        else
        {
            lblNoRecords.Text = GetMessage("lbl no records");
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }


        //Set Title
        string title = GetMessage("lbl Content Workflow") +" (" + GetCountText(grdData.Items.Count) + ")";
        SetTitle(title);
        ltrlViewAll.Text = GetMessage("lbl view all");

        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=viewsubmitted") + "', 'Reports')";

    }
    #endregion

    #region Edit
    /// <summary>
    /// Edit Event
    /// </summary>
    /// <param name="settings">Settings Data</param>
    protected void EditEvent(string settings)
    {
        uxDaysLimit.Text = DaysLimit.ToString();
        ViewSet.SetActiveView(uxEdit);
    }
    #endregion

    #region Save Data
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">Event Args</param>
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        int dayLimit;
        int.TryParse(uxDaysLimit.Text, out dayLimit);
        DaysLimit = dayLimit;

        Host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }
    #endregion

    #region Cancel Event
    /// <summary>
    /// Cancel Event
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">Event Args</param>
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
    #endregion

    #region Load Data
    private void LoadData()
    {
        BoundColumn colBound = new BoundColumn();
        colBound.DataField = "Title";
        colBound.HeaderText = GetMessage("generic Title");
        colBound.Initialize();
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "RequestType";
        colBound.HeaderText = GetMessage("generic ID");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "GoLive";
        colBound.HeaderText = GetMessage("generic Last Editor");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "Submitted";
        colBound.HeaderText = GetMessage("generic Date Modified");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "Path";
        colBound.HeaderText = GetMessage("generic Path");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "Language";
        colBound.HeaderText = GetMessage("generic Language");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);

        DataView dvData = GetDataSource();

        if (dvData.Count > 0)
        {
            grdData.DataSource = GetDataSource();
            grdData.DataBind();
            pnlData.Visible = true;
        }
        else
        {
            ltrlNoRecords.Text = GetMessage("lbl no records");
            //ltrlNoRecords.Visible = true;
            //pnlData.Visible = false;
        }
    }
    #endregion

    #region Get Data Source
    private DataView GetDataSource()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Title", typeof(string)));
        dt.Columns.Add(new DataColumn("RequestType", typeof(string)));
        dt.Columns.Add(new DataColumn("GoLive", typeof(string)));
        dt.Columns.Add(new DataColumn("Submitted", typeof(string)));
        dt.Columns.Add(new DataColumn("Path", typeof(string)));
        dt.Columns.Add(new DataColumn("Language", typeof(string)));



        EkDTSelector dtS = CommonAPIRef.EkDTSelectorRef;
        int counter = 0;
        DataRow dr;
        int contentType;
        string icon;
        DateTime currentTime = DateTime.Now;
        TimeSpan ts = new TimeSpan();
        foreach (Collection item in _data)
        {
            if (counter == PageSize)
            {
                break;
            }
            ts = currentTime - Convert.ToDateTime(item["LastDateEdited"]);

            contentType = Convert.ToInt32(item["ContentType"]);
            //icon = item["Icon"] == null ? "" : item["Icon"].ToString();
            icon = "";
            try
            {
                icon = item["ImageUrl"].ToString();
            }
            catch (Exception) { }

            long folderid = Convert.ToInt64(item["FolderID"]);
            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(folderid);

            if (DaysLimit == 0 || DaysLimit >= ts.Days)
            {
                dr = dt.NewRow();
                dr[0] = GetContentImage(contentType, icon);
                dr[0] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?action=ViewStaged&id=" + item["ContentID"] + "&callerpage=" + EkFunctions.UrlEncode("dashboard.aspx") + "&LangType=" + item["ContentLanguage"]) + "', 'Content', '" + foldercsvpath + "')\"" + " title=\'" + GetMessage("generic View") + " \"" + item["ContentTitle"].ToString().Replace("\'", "`") + "\"" + "\'>" + item["ContentTitle"] + "</a>";
                dr[1] = item["ContentID"];
                dr[2] = (item["EditorLname"] + (", " + item["EditorFname"]));
                dtS.targetDate = Convert.ToDateTime(item["LastDateEdited"]);
                dtS.spanId = "span_DispLastDateEdited";
                dr[3] = dtS.displayCultureDateTime(false, "", "");
                dr[4] = item["Path"];
                dr[5] = item["ContentLanguage"];
                dt.Rows.Add(dr);
                counter++;
            }
        }

        return new DataView(dt);
    }

    #endregion
}
