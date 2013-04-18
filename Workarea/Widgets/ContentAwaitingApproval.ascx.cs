using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_ContentAwaitingApproval : WorkareaWidgetBaseControl, IWidget
{
    Collection _data;

    protected void Page_Load(object sender, EventArgs e)
    {
        Collection criteria = new Collection();
        criteria.Add(CommonAPIRef.UserId, "UserID", null, null);
        criteria.Add("", "FolderIDs", null, null);
        criteria.Add("", "OrderBy", null, null);

        _data = EkContentRef.GetApprovalListForUserIDv1_1(criteria);
        if (_data.Count > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl approv smrtdesk") + " (" + GetCountText(_data.Count) + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("approval.aspx?action=viewApprovalList&ContType=0&page=workarea") + "', 'Reports')";
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
        colBound.HeaderText = GetMessage("generic Request Type");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.Wrap = false;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = "GoLive";
        colBound.HeaderText = GetMessage("generic Go Live");        
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        grdData.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = "Submitted";
        colBound.HeaderText = GetMessage("generic Submitted by");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
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
            //dr[0] += " <a href=\"approval.aspx?action=viewContent&page=workarea&id=" + item["ContentID"] + "&LangType=" + item["ContentLanguage"] + "\" title=\'" + GetMessage("generic View") + " \"" + item["ContentTitle"].ToString().Replace("\'", "`") + "\"" + "\'>" + item["ContentTitle"] + "</a>";
            //dr[0] += " <a target=\"_top\" href=\"workarea.aspx?page=approval.aspx&action=viewContent&ContentNav=%5c%5c&id=" + item["ContentID"] + "&LangType=" + item["ContentLanguage"] + "\">" + item["ContentTitle"] + "</a>";
            dr[0] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("approval.aspx?action=viewContent&id=" + item["ContentID"] + "&LangType=" + item["ContentLanguage"]) + "&page=workarea', 'Content', '" + foldercsvpath + "')\"" + 
                " title=\"" + GetMessage("generic View") + " \'" + item["ContentTitle"].ToString().Replace("\'", "`") + "\'\"" + ">" + item["ContentTitle"] + "</a>";
            if (item["Status"].ToString() == "S") 
            {
                dr[1] = "\t<font color=\"#C0C000\">" + GetMessage("generic Publish") + "</font>";
            }
            else 
            {
                dr[1] = "<font color=\"red\">" + GetMessage("generic Delete title") + "</font>";
            }
            if (item["DisplayGoLive"].ToString() == "")
            {
                // TODO: Ross - Can't use null because targetData is not nullable
                //dtS.targetDate = null;                
                dtS.targetDate = DateTime.MinValue;
            }
            else
            {
                dtS.targetDate = Ektron.Cms.Common.EkFunctions.ReadDbDate(item["GoLive"]);
            }
            dtS.spanId = "span_DisplayGoLive";
            dr[2] = dtS.displayCultureDateTime(false, "", "");
            dr[3] = item["SirName"];
            dr[4] = item["Path"];
            dr[5] = item["ContentLanguage"];
            dt.Rows.Add(dr);
            counter ++;
        }

        return new DataView(dt);
    }
}
