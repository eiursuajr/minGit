using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;

public partial class Workarea_Widgets_ContentReviews : WorkareaWidgetBaseControl, IWidget
{ 
    ContentReviewData[] _data;

    protected void Page_Load(object sender, EventArgs e)
    {
        int totalRecords = 0;
        PagingInfo pageInfo = new PagingInfo();
        _data = EkContentRef.GetUserReviewTotals(5, ref totalRecords, pageInfo);
        if (totalRecords > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

        string title = GetMessage("lbl review smrtdesk") + " (" + totalRecords.ToString() + ")";
        SetTitle(title);

        ltrlViewAll.Text = GetMessage("lbl view all");
        ltrlNoRecords.Text = GetMessage("lbl no records");
        lnkViewAll.OnClientClick = "top.showContentInWorkarea('" + EkFunctions.UrlEncode("reports.aspx?action=ContentReviews") + "', 'Reports')";
    }

    private void LoadData()
    {
        BoundColumn colBound = new BoundColumn();
        colBound = new BoundColumn();
        colBound.DataField = "APPROVE";
        colBound.HeaderText = GetMessage("generic approve title");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "DECLINE";
        colBound.HeaderText = GetMessage("btn decline");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = GetMessage("generic Title");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = GetMessage("display name label");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "DATETIME";
        colBound.HeaderText = GetMessage("generic date no colon");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "RATING";
        colBound.HeaderText = GetMessage("rating label");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        dataGrid.Columns.Add(colBound);
        colBound = new BoundColumn();
        colBound.DataField = "USER_COMMENTS";
        colBound.HeaderText = GetMessage("comment text");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = true;
        dataGrid.Columns.Add(colBound);

        dataGrid.DataSource = GetDataSource();
        dataGrid.DataBind();         
    }

    private DataView GetDataSource()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("APPROVE", typeof(string)));
        dt.Columns.Add(new DataColumn("DECLINE", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DATETIME", typeof(string)));
        dt.Columns.Add(new DataColumn("RATING", typeof(string)));
        dt.Columns.Add(new DataColumn("USER_COMMENTS", typeof(string)));
        
        bool bShowApprove = false;
        EkDTSelector dtS = CommonAPIRef.EkDTSelectorRef;

        DataRow dr;
        int contentType;
        string icon;
        foreach (ContentReviewData item in _data)
        {
            contentType = item.ContentType;
            icon = item.AssetData.Icon;

            dr = dt.NewRow();

            bShowApprove = true;
            string id = item.ID.ToString();

            long folderid = EkContentRef.GetFolderIdForContentId(item.ID);
            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(folderid);

            dr[0] = "<input type=\"radio\" id=\"cr_a_" + id + "\" name=\"cr_a_" + id + "\" value=\"cr_app_" + id + "\" />";
            dr[1] = "<input type=\"radio\" id=\"cr_a_" + id + "\" name=\"cr_a_" + id + "\" value=\"cr_dec_" + id + "\" />";
            dr[2] = GetContentImage(contentType, icon);
            //dr[2] += " <a href=\"addeditcontentreview.aspx?action=view&id=" + id + "&cid=" + item.ContentID.ToString() + "\">" + item.ContentTitle + "</a>";
            dr[2] += " <a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("addeditcontentreview.aspx?action=view&id=" + id + "&cid=" + item.ContentID.ToString()) + "&page=workarea', 'Content', '" + foldercsvpath + "')\">" + item.ContentTitle + "</a>";
            if (item.UserID == 0)
            {
                dr[3] = "<font color=\"gray\">" + GetMessage("lbl anon") + "</font>";
            }
            else
            {
                dr[3] = item.UserDisplayName;
            }
            dr[4] = item.RatingDate.ToShortDateString();
            dr[5] = GenerateStars(item.Rating);
            dr[6] = EkFunctions.HtmlEncode(System.Web.HttpUtility.UrlDecode(item.UserComments));
            dt.Rows.Add(dr);
        }
        if (bShowApprove)
        {
            btnApprove.Visible = true;
            btnApprove.Attributes.Add("onclick", "javascript: return CheckApproveSelect();");
        }

        return new DataView(dt);
    }

    public string GenerateStars(int rating)
    {
        StringBuilder sbRating = new StringBuilder();
        for (int i = 1; i <= 10; i++)
        {
            sbRating.Append("<img alt=\"\" style=\"vertical-align:middle\" src=\"");
            sbRating.Append(ImagePath);
            sbRating.Append("../UI/Icons/");
            if (i % 2 > 0)
            {
                if (rating < i)
                {
                    sbRating.Append("starEmptyLeft.png");
                }
                else
                {
                    sbRating.Append("starLeft.png");
                }
            }
            else if (rating < i)
            {
                sbRating.Append("starEmptyRight.png");
            }
            else
            {
                sbRating.Append("starRight.png");
            }

            sbRating.Append("\" />");
        }

        return sbRating.ToString();
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        ContentReviewData reviewData;

        for (int i = 0; (i <= (Request.Form.Count - 1)); i++) 
        {
            if (Request.Form.Keys[i].ToString().IndexOf("cr_a_") == 0)
            {
                string[] pieces = Request.Form[i].Split('_');
                string action = pieces[1].ToString();
                long reviewID = Convert.ToInt64(pieces[2]);

                if (reviewID > 0)
                {
                    reviewData = EkContentRef.GetContentRating(reviewID);
                    if (action.ToLower() == "app")
                    {
                        reviewData.State = EkEnumeration.ContentReviewState.Approved;
                    }
                    else if (action.ToLower() == "dec") 
                    {
                        reviewData.State = EkEnumeration.ContentReviewState.Rejected;
                    }

                    reviewData = EkContentRef.UpdateContentReview(reviewData);
                }
            }
        }

        Response.Redirect("dashboard.aspx", false);
    }
}
