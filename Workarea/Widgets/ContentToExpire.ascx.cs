using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_ContentToExpire : WorkareaWidgetBaseControl, IWidget
{
    Collection _data;
    EmailHelper m_refEmail = new EmailHelper();

    protected void Page_Load(object sender, EventArgs e)
    {
        Collection criteria = new Collection();
        criteria.Add("", "OrderBy", null, null);
        criteria.Add("10", "Interval", null, null);

        _data = EkContentRef.GetContentToExpire(criteria);
        if (_data.Count > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl expire all smrtdesk") + " (" + GetCountText(_data.Count) + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=ViewToExpire&interval=10") + "', 'Reports')";
    }

    private void LoadData()
    {
        BoundColumn colBound = new BoundColumn();
        colBound.DataField = "Title";
        colBound.HeaderText = GetMessage("generic Title");
        colBound.Initialize();
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "ContentId";
        colBound.HeaderText = GetMessage("generic ID");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "EmailLink";
        colBound.HeaderText = GetMessage("generic Last Editor");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "DisplayEndDate";
        colBound.HeaderText = GetMessage("generic end date");
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
        dt.Columns.Add(new DataColumn("ContentId", typeof(string)));
        dt.Columns.Add(new DataColumn("EmailLink", typeof(string)));
        dt.Columns.Add(new DataColumn("DisplayEndDate", typeof(string)));
        dt.Columns.Add(new DataColumn("Path", typeof(string)));
        dt.Columns.Add(new DataColumn("Language", typeof(string)));

        int counter = 0;        
        EkDTSelector dtS = CommonAPIRef.EkDTSelectorRef;

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
            //dr[0] += " <a href=\"content.aspx?action=View&id=" + item["ContentID"] + "&LangType=" + item["ContentLanguage"] + "&callerpage=" + EkFunctions.UrlEncode("dashboard.aspx") + "\"" + " title=\'" + GetMessage("generic View") + " \"" + item["ContentTitle"].ToString().Replace("\'", "`") + "\"" + "\'>" + item["ContentTitle"] + "</a>";
            dr[0] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?action=View&id=" + item["ContentID"] + "&LangType=" + item["ContentLanguage"] + "&callerpage=" + EkFunctions.UrlEncode("dashboard.aspx")) + "', 'Content', '" + foldercsvpath + "')\"" + " title=\'" + GetMessage("generic View") + " \"" + item["ContentTitle"].ToString().Replace("\'", "`") + "\"" + "\'>" + item["ContentTitle"] + "</a>";
            dr[1] = item["ContentID"];
            Collection refItem = item;
            dr[2] = m_refEmail.MakeUserEditorContentEmailLink(ref refItem, true, false);
            dtS.targetDate = Convert.ToDateTime(item["DisplayEndDate"]);
            dtS.spanId = "span_DisplayEndDate";
            dr[3] = dtS.displayCultureDateTime(false, "", "");
            dr[4] = item["Path"];
            dr[5] = item["ContentLanguage"];
            dt.Rows.Add(dr);
            counter = (counter + 1);
        }

        return new DataView(dt);
    }
}
