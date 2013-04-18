using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_ContentCheckedOut : WorkareaWidgetBaseControl, IWidget
{  
    Collection _data;

    protected void Page_Load(object sender, EventArgs e)
    {
        Collection criteria = new Collection();
        criteria.Add("checkedout", "StateWanted", null, null);
        criteria.Add("", "FilterType", null, null);
        criteria.Add(0, "FilterID", null, null);
        criteria.Add("", "OrderBy", null, null);

        _data = EkContentRef.GetContentReportv2_0(criteria);
        if (_data.Count > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl chckd out smrtdesk") + " (" + GetCountText(_data.Count) + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=ViewCheckedOut") + "', 'Reports')";
    }

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

        grdData.DataSource = GetDataSource();
        grdData.DataBind();
    }

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
        foreach (Collection item in _data) 
        {
            if (counter == PageSize)
            {
                break;
            }

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

            dr = dt.NewRow();
            dr[0] = GetContentImage(contentType, icon);
            dr[0] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?action=ViewStaged&id=" + item["ContentID"] + "&callerpage=" + Server.UrlEncode("dashboard.aspx") + "&LangType=" + item["ContentLanguage"]) + "', 'Content', '" + foldercsvpath + "')\"" + " title=\'" + GetMessage("generic View") + " \"" + item["ContentTitle"].ToString().Replace("\'", "`") + "\"" + "\'>" + item["ContentTitle"] + "</a>";
            dr[1] = item["ContentID"];
            dr[2] = (item["EditorLname"] + (", " + item["EditorFname"]));
            dtS.targetDate = Convert.ToDateTime(item["LastDateEdited"]);
            dtS.spanId = "span_DispLastDateEdited";
            dr[3] = dtS.displayCultureDateTime(false, "", "");
            dr[4] = item["Path"];
            dr[5] = item["ContentLanguage"];
            dt.Rows.Add(dr);
            counter ++;
        }

        return new DataView(dt);
    }
}
