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
using Ektron.Cms.Content;
using System.IO;

public partial class Workarea_DeclineContent : System.Web.UI.Page
{
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected ContentAPI content_api = new ContentAPI();
    protected int ContentLanguage = 0;
    protected string AppImgPath = "";

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            long contentid = 0;
            long folderid = 0;
            CommonApi AppUI = new CommonApi();
            m_refMsg = content_api.EkMsgRef;
			Utilities.ValidateUserLogin();
            if (content_api.RequestInformationRef.IsMembershipUser == 1 || content_api.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(content_api.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            RegisterResources();
            if ((!(Request.QueryString["contentId"] == null)) && (long.TryParse(Request.QueryString["contentId"], out contentid)) && (contentid > 0))
            {
                hdnContentId.Value = contentid.ToString();

                if ((!(Request.QueryString["LangType"] == null)) && (int.TryParse(Request.QueryString["LangType"], out ContentLanguage)) && (ContentLanguage > 0))
                {
                    if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                    {
                        ContentLanguage = AppUI.DefaultContentLanguage;
                    }
                    AppUI.ContentLanguage = ContentLanguage;
                    content_api.ContentLanguage = ContentLanguage;
                }
                else
                {
                    ContentLanguage = AppUI.DefaultContentLanguage;
                }
                hdnLangType.Value = ContentLanguage.ToString();

                if ((!(Request.QueryString["folderId"] == null)) && (long.TryParse(Request.QueryString["folderId"], out folderid)) && (folderid > 0))
                {
                    hdnFolderId.Value = folderid.ToString();
                }
                ViewToolBar();
            }
        }
    }

    protected void btnDecline_Click(object sender, System.EventArgs e)
    {
        string comment = "";
        Ektron.Cms.UI.CommonUI.ApplicationAPI appUI = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
        m_refMsg = appUI.EkMsgRef;

        RegExpValidator.ErrorMessage = m_refMsg.GetMessage("content size exceeded");
        //RegExpValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(65000)
        RegExpValidator.Validate();
        if (RegExpValidator.IsValid)
        {
            if (DeclineText.Content.Trim().Length > 0)
            {
                comment = (string)("&comment=" + EkFunctions.UrlEncode((string)(DeclineText.Content.Trim().Replace("<p>", "").Replace("</p>", ""))));
            }
	        if(comment.Length>255)
	        {
	    	    RegExpValidator.IsValid=false;
 	            ViewToolBar();
	        } 
	        else
	        {
            	Response.Redirect(content_api.ApplicationPath + "content.aspx?id=" + hdnContentId.Value + "&fldid=" + hdnFolderId.Value + "&action=declinecontent&LangType=" + hdnLangType.Value + comment);
            }
        }
        else
        {
            ViewToolBar();
        }
    }

    private void ViewToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        AppImgPath = content_api.AppImgPath;
        StyleSheetJS.Text = m_refStyle.GetClientScript();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl decline content"));
        result.Append("<table><tr>");
        result.Append("<td>" + m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", content_api.AppPath + "content.aspx?LangType=" + ContentLanguage + "&action=viewcontentbycategory&id=" + Request.QueryString["folderId"], m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true) + "</td>");
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>" + m_refStyle.GetHelpButton("Viewcontent", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}