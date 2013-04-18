using System;
using System.Data;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_ContentFlags : WorkareaWidgetBaseControl, IWidget
{
    ContentFlagData[] _data;

    protected void Page_Load(object sender, EventArgs e)
    {
        FlagRequest criteria = new FlagRequest();
        criteria.Top = 5;
        criteria.StartDate = DateTime.Now.Subtract(new TimeSpan(10, 0, 0, 0));
        criteria.EndDate = DateTime.Now;

        _data = EkContentRef.GetAllFlagEntries(ref criteria);
        int totalRecords = criteria.RecordsAffected;
        if (totalRecords > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl flag smrtdesk") + " (" + totalRecords + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=ContentFlags") + "', 'Reports')";
    }

    private void LoadData()
    {
        BoundColumn colBound = new BoundColumn();
        colBound = new BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = GetMessage("generic Title");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = GetMessage("display name label");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "DATETIME";
        colBound.HeaderText = GetMessage("generic date no colon");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "FLAG";
        colBound.HeaderText = GetMessage("flag label");        
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        grdData.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "COMMENTS";
        colBound.HeaderText = GetMessage("comment text");        
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = true;
        grdData.Columns.Add(colBound);

        grdData.DataSource = GetDataSource();
        grdData.DataBind();
    }

    private DataView GetDataSource()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DATETIME", typeof(string)));
        dt.Columns.Add(new DataColumn("FLAG", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMENTS", typeof(string)));

        EkDTSelector dtS = CommonAPIRef.EkDTSelectorRef;

        DataRow dr;
        int contentType;
        string icon;
        foreach (ContentFlagData item in _data)
        {
            contentType = item.ContentType;
            //icon = item["Icon"] == null ? "" : item["Icon"].ToString();
            icon = "";
            icon = (item.ImageThumbnail == "" ? item.AssetData.ImageUrl : item.ImageThumbnail);

            long folderid = EkContentRef.GetFolderIDForContent(item.Id);
            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(folderid);

            dr = dt.NewRow();
            dr[0] = GetContentImage(contentType, icon);
            //dr[0] += "<a href=\"contentflagging/addeditcontentflag.aspx?action=view&id=" + item.FlagId.ToString() + "&cid=" + item.Id.ToString() + "\">" + item.Title + "</a>";
            dr[0] += "<a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("contentflagging/addeditcontentflag.aspx?action=view&id=" + item.FlagId.ToString() + "&cid=" + item.Id.ToString()) + "&page=workarea" + "', 'Content', '" + foldercsvpath + "')\">" + item.Title + "</a>";
            if (item.FlaggedUser.Id == 0)
            {
                dr[1] = "<font color=\"gray\">" + GetMessage("lbl anon") + "</font>";
            }
            else
            {
                dr[1] = item.FlaggedUser.DisplayName;
            }
            dr[2] = item.FlagDate.ToShortDateString();
            dr[3] = item.FlagName;
            dr[4] = item.FlagComment;
            dt.Rows.Add(dr);
        }

        return new DataView(dt);
    }
}
