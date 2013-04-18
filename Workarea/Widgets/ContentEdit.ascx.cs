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

public partial class Workarea_Widgets_ContentEdit : WorkareaWidgetBaseControl, IWidget
{
    #region properties
    List<ContentData> contentList;
    private int _DaysLimit;
    private int _ItemLimit;
    [WidgetDataMember(7)]
    public int DaysLimit { get { return _DaysLimit; } set { _DaysLimit = value; } }
    [WidgetDataMember(50)]
    public int ItemLimit { get { return _ItemLimit; } set { _ItemLimit = value; } }
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
        SetTitle("Content Edited");
        ViewSet.SetActiveView(View);
    }
    #endregion

    #region Set Output
    /// <summary>
    /// Set Output
    /// </summary>
    protected void SetOutput()
    {
        ContentAPI contentApi = new ContentAPI();
        Criteria<ContentProperty> criteria = new Criteria<ContentProperty>();
       

        if (DaysLimit != 0)
            criteria.AddFilter(ContentProperty.DateModified, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.AddDays(-DaysLimit));
        criteria.AddFilter(ContentProperty.Type, CriteriaFilterOperator.NotEqualTo, "1111");
        criteria.AddFilter(ContentProperty.FolderName, CriteriaFilterOperator.NotEqualTo, "Workspace");
		criteria.AddFilter(ContentProperty.FolderName, CriteriaFilterOperator.NotEqualTo, "_meta_");
        criteria.AddFilter(ContentProperty.Status, CriteriaFilterOperator.EqualTo, "A");
        if (ItemLimit > 0)
        {
            PagingInfo _pageInfo = new PagingInfo(ItemLimit);
            criteria.PagingInfo = _pageInfo;
        }
        contentList = contentApi.GetList(criteria);

        if (contentList.Count > 0)
        {
            LoadData();
            grdData.Style.Add("display", "table");

        }
        else
        {
            ltrlNoRecords.Text = GetMessage("lbl no records");

        }

        //Set Title
        string title = "";
        if (ItemLimit > PageSize)
            title = "Content Edited (" + grdData.Items.Count.ToString() + ")";
        else
             title = "Content Edited (" + GetCountText(grdData.Items.Count) + ")";
        SetTitle(title);
        //ltrlViewAll.Text = GetMessage("lbl view all") + " (" + GetCountText(grdData.Items.Count) + ")";

        //lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=viewsubmitted") + "', 'Reports')";

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
        uxItemLimit.Text = ItemLimit.ToString();
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
        int itemsLimit;
        int.TryParse(uxDaysLimit.Text, out dayLimit);
        int.TryParse(uxItemLimit.Text, out itemsLimit);
        DaysLimit = dayLimit;
        ItemLimit = itemsLimit;
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
        int counterCount = (ItemLimit > PageSize) ? ItemLimit : PageSize;
        string icon;
        DateTime currentTime = DateTime.Now;
        TimeSpan ts = new TimeSpan();
        foreach (ContentData item in contentList)
        {
            if (counter == counterCount)
            {
                break;
            }
            ts = currentTime - Convert.ToDateTime(item.LastEditDate);

            contentType = Convert.ToInt32(item.ContType);
            //icon = item["Icon"] == null ? "" : item["Icon"].ToString();
            icon = "";
            try
            {
                icon = item.AssetData.ImageUrl;//item.Image;
            }
            catch (Exception) { }

            long folderid = Convert.ToInt64(item.FolderId);
            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(folderid);

            //if (DaysLimit >= ts.Days)
            //{
            dr = dt.NewRow();
            dr[0] = GetContentImage(contentType, icon);
            dr[0] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?action=ViewStaged&id=" + item.Id + "&callerpage=" + Server.UrlEncode("dashboard.aspx") + "&LangType=" + item.LanguageId) + "', 'Content', '" + foldercsvpath + "')\"" + " title=\'" + GetMessage("generic View") + " \"" + item.Title.Replace("\'", "`") + "\"" + "\'>" + item.Title + "</a>";
            dr[1] = item.Id;
            dr[2] = (item.EditorLastName + (", " + item.EditorFirstName));
            dtS.targetDate = Convert.ToDateTime(item.LastEditDate);
            dtS.spanId = "span_DispLastDateEdited";
            dr[3] = dtS.displayCultureDateTime(false, "", "");
            dr[4] = item.Path;
            dr[5] = item.LanguageId;
            dt.Rows.Add(dr);
            counter++;
            //}
        }

        return new DataView(dt);
    }

    #endregion
}
