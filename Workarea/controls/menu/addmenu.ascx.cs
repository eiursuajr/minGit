using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;
using Ektron.Cms.UI.CommonUI;
using System.Collections;

public partial class Workarea_controls_menu_addmenu : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper MsgHelper;
    protected Collection gtLinks;
    protected long MenuId;
    protected string sitePath;
    protected string AppPath;
    protected Int32 ContentLanguage;
    protected string LanguageName = "";
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string action = "";
    protected long folderId = 0;
    protected long nId = 0;
    CommonApi _commonApi;
    protected string noWorkAreaString = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        ApplicationAPI AppUI = new ApplicationAPI();
        sitePath = AppUI.SitePath;
        if (Request.QueryString["LangType"] == "-1")
        {
            ContentLanguage = AppUI.DefaultContentLanguage;
            AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            AppUI.ContentLanguage = ContentLanguage;
        }
        else
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == 0)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
                AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                AppUI.ContentLanguage = ContentLanguage;
            }
        }

        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]) && Request.QueryString["noworkarea"] == "1")
        {
            noWorkAreaString = "&noworkarea=1";
        }

        LanguageData language_data = (new SiteAPI()).GetLanguageById(ContentLanguage);
        if (language_data != null)
            LanguageName = language_data.Name;

        folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        nId = Convert.ToInt64(Request.QueryString["nId"]);

        if (!CanManageMenus)
        {
            Response.Redirect((string)("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login menu administrator")), false);
            return;
	    }
        action = Request.QueryString["action"];
        
		switch (action)
        {
            case "AddMenu":
                if (Request.QueryString["back"] != "" && Request.QueryString["back"] != null)
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
		        else if (Request.QueryString["bPage"] == "ViewMenuReport")
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		        else
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewAllMenus&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
                break;
            case "AddSubMenu":
	            if (Request.QueryString["back"] != "")
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
		        else if (Request.QueryString["iframe"] == "true")
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
		        else
                {
		            if (Request.QueryString["noworkarea"] == "1")
						litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "close.aspx", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
				    else
                    {
		                if (CommonAPI.TreeModel == 1)
							litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menutree.aspx?nid=" + nId + "&folderid=" + folderId + "&noworkarea=" + Request.QueryString["noworkarea"], MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		                else
							litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" + nId + "&folderid=" + folderId + "&noworkarea=" + Request.QueryString["noworkarea"], MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
	                }
		        }
                break;
            case "AddTransMenu":
                if (Request.QueryString["back"] != "")
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
		        else
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?langtype=" + Request.QueryString["backlang"] + "+action=ViewAllMenus+folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
                break;
        }

		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt save menu"), MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'menu\', \'VerifyMenuForm()\');\"", StyleHelper.SaveButtonCssClass, true);    
    }

    protected CommonApi CommonAPI {
        get { return (_commonApi ?? (_commonApi = new CommonApi())); }
    }

    protected bool CanManageMenus {
        get {
            return CommonAPI.IsAdmin()
                || IsFolderAdmin
                || CommonAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu) 
                || CommonAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu);
        }
    }

    private bool? _isFolderAdmin;
    protected bool IsFolderAdmin
    {
        get { return (_isFolderAdmin ?? (bool)(_isFolderAdmin = CommonAPI.IsARoleMemberForFolder_FolderUserAdmin(folderId, 0, false))); }
    }
}