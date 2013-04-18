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
using Ektron.Cms.Workarea;


public partial class addeditcontentreview : workareabase
{
    protected long content_id = 0;
    protected PermissionData security_data;
    protected ContentReviewData crReview;
    protected ContentReviewData[] aReviews = (Ektron.Cms.ContentReviewData[])Array.CreateInstance(typeof(Ektron.Cms.ContentReviewData), 0);
    protected string redirectUrl = "";

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
            {
                m_iID = Convert.ToInt64(Request.QueryString["rid"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["cid"]))
            {
                content_id = Convert.ToInt64(Request.QueryString["cid"]);
            }

            // the redirectUrl is provided for use by ContentStatistics.aspx specifically
            // so we can redirect back to that page after editting or deleting reviews or comments.
            if (!string.IsNullOrEmpty(Request.QueryString["redirectUrl"]))
            {
                redirectUrl = Request.QueryString["redirectUrl"];
            }

            CheckPermissions();

            if (Page.IsPostBack)
            {
                switch (base.m_sPageAction)
                {
                    default: // "edit"
                        Process_Edit();
                        break;
                }
            }
            else
            {
                aReviews = this.m_refContentApi.EkContentRef.GetContentRating(m_iID, 0, 0, -1, "", "");
                if (aReviews.Length > 0)
                {
                    crReview = aReviews[0];
                }
                RenderJS();
                switch (base.m_sPageAction)
                {
                    case "delete":
                        Process_Delete();
                        break;
                    case "edit":
                        Display_Edit();
                        break;
                    default: // "view"
                        Display_View();
                        break;
                }
                SetLabels();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    protected void Process_Edit()
    {
        aReviews = this.m_refContentApi.EkContentRef.GetContentRating(this.m_iID, 0, 0, -1, "", "");
        if (aReviews.Length > 0)
        {
            crReview = aReviews[0];
            crReview.UserComments = (string)txt_review.Text;
            crReview.State = (EkEnumeration.ContentReviewState)Enum.Parse(typeof(EkEnumeration.ContentReviewState), drp_status_data.SelectedValue);
            crReview.Rating = Convert.ToInt32(Request.Form["irating"]);
            crReview = this.m_refContentApi.EkContentRef.UpdateContentReview(crReview);
        }
        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        try
        {
            if (redirectUrl.Length > 0)
            {
                Response.Redirect(redirectUrl, false);
            }
            else
            {
                Response.Redirect((string)("addeditcontentreview.aspx?action=view&id=" + this.m_iID + "&cid=" + this.content_id + pagemode), false);
            }
        }
        catch (Exception)
        {
            // do nothing
        }
    }
    protected void Process_Delete()
    {
        aReviews = this.m_refContentApi.EkContentRef.GetContentRating(this.m_iID, 0, 0, -1, "", "");
        if (aReviews.Length > 0)
        {
            crReview = aReviews[0];
            this.m_refContentApi.EkContentRef.DeleteContentReview(crReview);
        }
        if (Request.QueryString["page"] == "workarea")
        {
            try
            {
                if (redirectUrl.Length > 0)
                {
                    Response.Redirect(redirectUrl, false);
                }
                else
                {
                    // redirect to workarea
                    Response.Write("<script language=\"Javascript\">" + "top.switchDesktopTab();" + "</script>");
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }
        else
        {
            Response.Redirect((string)("ContentStatistics.aspx?page=ContentStatistics.aspx&id=" + this.content_id + "&LangType=" + this.ContentLanguage), false);
        }
    }
    protected void Display_Edit()
    {
        if (crReview.UserID == 0)
        {
            ltr_uname_data.Text = base.GetMessage("lbl anon");
        }
        else
        {
            ltr_uname_data.Text = crReview.Username;
        }
        ltr_date_data.Text = crReview.RatingDate.ToLongDateString() + " " + crReview.RatingDate.ToShortTimeString();
        txt_review.Text = Server.HtmlDecode(crReview.UserComments);
        ltr_rating_val.Text = ShowRating(crReview.Rating, true);
        switch (crReview.State)
        {
            case EkEnumeration.ContentReviewState.Pending:
                drp_status_data.SelectedIndex = 0;
                break;
            case EkEnumeration.ContentReviewState.Approved:
                drp_status_data.SelectedIndex = 1;
                break;
            case EkEnumeration.ContentReviewState.Rejected:
                drp_status_data.SelectedIndex = 2;
                break;
        }
    }
    protected void Display_View()
    {
        if (crReview.UserID == 0)
        {
            ltr_uname_data.Text = base.GetMessage("lbl anon");
        }
        else
        {
            ltr_uname_data.Text = crReview.Username;
        }
        ltr_date_data.Text = crReview.RatingDate.ToLongDateString() + " " + crReview.RatingDate.ToShortTimeString();
        txt_review.Text = Server.HtmlDecode(crReview.UserComments);
        ltr_rating_val.Text = ShowRating(crReview.Rating, false);
        switch (crReview.State)
        {
            case EkEnumeration.ContentReviewState.Pending:
                drp_status_data.SelectedIndex = 0;
                break;
            case EkEnumeration.ContentReviewState.Approved:
                drp_status_data.SelectedIndex = 1;
                break;
            case EkEnumeration.ContentReviewState.Rejected:
                drp_status_data.SelectedIndex = 2;
                break;
        }
    }
    protected void CheckPermissions()
    {
        security_data = this.m_refContentApi.LoadPermissions(this.content_id, "content", 0);
        switch (base.m_sPageAction)
        {
            case "edit":
                if (security_data.CanEdit == true)
                {
                    // we are good
                }
                else
                {
                    throw (new Exception("You do not have permissions to edit."));
                }
                break;
            default: // "view"
                if (security_data.IsReadOnly == true)
                {
                    // we are good
                }
                else
                {
                    throw (new Exception("You do not have permissions to view."));
                }
                break;
        }
    }
    protected void SetLabels()
    {
        this.ltr_date.Text = "Date:";
        this.ltr_uname.Text = "Username:";
        this.ltr_status.Text = "Status:";
        this.ltr_rating.Text = "Rating:";
        this.ltr_review.Text = "Review:";
        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        switch (base.m_sPageAction)
        {
            case "edit":
                base.SetTitleBarToString("Edit");

				if (redirectUrl.Length > 0)
				{
					base.AddBackButton(redirectUrl);
				}
				else
				{
					base.AddBackButton((string)("?action=view&id=" + this.m_iID + "&cid=" + this.content_id + pagemode));
				}
				
				base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"javascript:SubmitForm();return true;\"", StyleHelper.SaveButtonCssClass, true);
                
                break;

            default: // "view"
                this.drp_status_data.Enabled = false;
                this.txt_review.Enabled = false;
                base.SetTitleBarToString("View Content Review " + "\"" + crReview.ContentTitle + "\"");

				if (Request.QueryString["page"] == "workarea")
				{
					// redirect to workarea when user clicks back button if we're in workarea
					base.AddButtonwithMessages(AppImgPath + "../UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"javascript:top.switchDesktopTab()\" ", StyleHelper.BackButtonCssClass, true);
				}
				else
				{
					base.AddBackButton((string)("ContentStatistics.aspx?page=ContentStatistics.aspx&id=" + this.content_id + "&LangType=" + this.ContentLanguage));
				}

                if (security_data.CanEdit == true)
                {
                    base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("?action=edit&id=" + this.m_iID + "&cid=" + this.content_id + pagemode), "alt edit button text", "btn edit", "", StyleHelper.EditButtonCssClass, true);
                    base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string)("?action=delete&id=" + this.m_iID + "&cid=" + this.content_id + pagemode), "btn alt del review", (string)("btn delete"), " onclick=\"javascript:return confirm(\'" + base.GetMessage("js conf del review") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                
                break;
        }
        base.AddHelpButton("AddEditReviews");
    }

    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        if (this.m_sPageAction == "edit")
        {
            sbJS.Append("function SubmitForm()" + Environment.NewLine);
            sbJS.Append("{" + Environment.NewLine);
            sbJS.Append("    document.forms[0].submit();" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);

            sbJS.Append("  function rhdl(item, act) ").Append(Environment.NewLine);
            sbJS.Append("  { ").Append(Environment.NewLine);
            sbJS.Append("  	var defrating = document.getElementById(\'irating\').value; ").Append(Environment.NewLine);
            sbJS.Append("  	switch(act) ").Append(Environment.NewLine);
            sbJS.Append("  	{ ").Append(Environment.NewLine);
            sbJS.Append("  	case \"over\" : ").Append(Environment.NewLine);
            sbJS.Append("  	case \"out\" : ").Append(Environment.NewLine);
            sbJS.Append("  		for (var w = 0; w <= 10; w++) ").Append(Environment.NewLine);
            sbJS.Append("  		{ ").Append(Environment.NewLine);
            sbJS.Append("  			if (w == 0) { (document.getElementById(\'img_\' + w)).src = \'" + this.m_refContentApi.AppPath + "images/UI/Icons/stop.png\'; } ").Append(Environment.NewLine);
            sbJS.Append("  			else if (((w % 2) > 0) && w > item) { (document.getElementById(\'img_\' + w)).src = \'" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyLeft.png\'; } ").Append(Environment.NewLine);
            sbJS.Append("  			else if (((w % 2) > 0) && w <= item) { (document.getElementById(\'img_\' + w)).src = \'" + this.m_refContentApi.AppPath + "images/UI/Icons/starLeft.png\'; } ").Append(Environment.NewLine);
            sbJS.Append("  			else if (((w % 2) == 0) && w > item) { (document.getElementById(\'img_\' + w)).src = \'" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyRight.png\'; } ").Append(Environment.NewLine);
            sbJS.Append("  			else if (((w % 2) == 0) && w <= item) { (document.getElementById(\'img_\' + w)).src = \'" + this.m_refContentApi.AppPath + "images/UI/Icons/starRight.png\'; } ").Append(Environment.NewLine);
            sbJS.Append("  		} ").Append(Environment.NewLine);
            sbJS.Append("  		break; ").Append(Environment.NewLine);
            //sbJS.Append("  	case ""out"" : ").Append(Environment.NewLine)
            //sbJS.Append("  		for (var w = 0; w <= 10; w++) ").Append(Environment.NewLine)
            //sbJS.Append("  		{ ").Append(Environment.NewLine)
            //sbJS.Append("  			if (w == 0) { (document.getElementById(""img_"" + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/stop.png'; } ").Append(Environment.NewLine)
            //sbJS.Append("  			else if (((w % 2) > 0) && item > defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyLeft.png'; } ").Append(Environment.NewLine)
            //sbJS.Append("  			else if (((w % 2) > 0) && item <= defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starLeft.png'; } ").Append(Environment.NewLine)
            //sbJS.Append("  			else if (((w % 2) == 0) && item > defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyRight.png'; } ").Append(Environment.NewLine)
            //sbJS.Append("  			else if (((w % 2) == 0) && item <= defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starRight.png'; } ").Append(Environment.NewLine)
            //sbJS.Append("  		} ").Append(Environment.NewLine)
            //sbJS.Append("  		break; ").Append(Environment.NewLine)
            sbJS.Append("  	case \"click\" : ").Append(Environment.NewLine);
            sbJS.Append("  		document.getElementById(\'irating\').value = item; ").Append(Environment.NewLine);
            sbJS.Append("  	    if (item == 0) { document.getElementById(\'d_rating\').innerHTML = \'No Rating\'; } ").Append(Environment.NewLine);
            sbJS.Append("  	    else if ((item % 2) > 0) {  ").Append(Environment.NewLine);
            sbJS.Append("  	        if ((Math.floor(item / 2)) == 0) { document.getElementById(\'d_rating\').innerHTML = \'1/2 star\'; } ").Append(Environment.NewLine);
            sbJS.Append("  	        else { document.getElementById(\'d_rating\').innerHTML = (Math.floor(item / 2)) + \' 1/2 stars\'; } ").Append(Environment.NewLine);
            sbJS.Append("  	    } else if ((item % 2) == 0) { document.getElementById(\'d_rating\').innerHTML = Math.floor(item / 2) + \' stars\'; } ").Append(Environment.NewLine);

            sbJS.Append("  		break; ").Append(Environment.NewLine);
            sbJS.Append("  	} ").Append(Environment.NewLine);
            sbJS.Append("  } ").Append(Environment.NewLine);

        }
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }
    private string ShowRating(int iRating, bool IsEdit)
    {
        StringBuilder sbRating = new StringBuilder();

        if (IsEdit == false) //view
        {
            for (int i = 0; i <= 10; i++)
            {
                if (i % 2 == 0 && i > 0 && i > iRating)
                {
                    sbRating.Append("<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyRight.png\" />");
                }
                else if (i % 2 == 0 && i > 0 && i <= iRating)
                {
                    sbRating.Append("<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starRight.png\" />");
                }
                else if (i > 0 && i > iRating)
                {
                    sbRating.Append("<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyLeft.png\" />");
                }
                else if (i > 0 && i <= iRating)
                {
                    sbRating.Append("<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starLeft.png\" />");
                }
                else
                {
                    sbRating.Append("<img src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/stop.png\" />");
                }
            }
        }
        else // edit
        {
            sbRating.Append("<input type=\"hidden\" id=\"irating\" name=\"irating\" value=\"" + iRating + "\"/>");
            for (int i = 0; i <= 10; i++)
            {
                sbRating.Append("<img id=\"img_" + i + "\" name=\"img_" + i + "\" onmouseover=\"rhdl(" + i + ",\'over\');\" onmouseoout=\"rhdl(" + i + ",\'out\');\" onclick=\"rhdl(" + i + ",\'click\');\" ");
                if (i % 2 == 0 && i > 0 && i > iRating)
                {
                    sbRating.Append("src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyRight.png\" ");
                }
                else if (i % 2 == 0 && i > 0 && i <= iRating)
                {
                    sbRating.Append("src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starRight.png\" ");
                }
                else if (i > 0 && i > iRating)
                {
                    sbRating.Append("src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starEmptyLeft.png\" ");
                }
                else if (i > 0 && i <= iRating)
                {
                    sbRating.Append("src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/starLeft.png\" ");
                }
                else
                {
                    sbRating.Append("src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/stop.png\" ");
                }
                sbRating.Append("/>");
            }
        }
        if (iRating == 0)
        {
            sbRating.Append("&nbsp;&nbsp;<span id=\"d_rating\" name=\"d_rating\">No rating</span>");
        }
        else if (iRating == 1)
        {
            sbRating.Append("&nbsp;&nbsp;<span id=\"d_rating\" name=\"d_rating\">1/2 star</span>");
        }
        else if (iRating % 2 > 0)
        {
            sbRating.Append("&nbsp;&nbsp;<span id=\"d_rating\" name=\"d_rating\">" + (iRating / 2) + (iRating > 2 ? " 1/2 stars" : " star") + "</span>");
        }
        else if (iRating % 2 == 0)
        {
            sbRating.Append("&nbsp;&nbsp;<span id=\"d_rating\" name=\"d_rating\">" + (iRating / 2) + (iRating > 2 ? " stars" : " star") + "</span>");
        }

        return sbRating.ToString();
    }
}
