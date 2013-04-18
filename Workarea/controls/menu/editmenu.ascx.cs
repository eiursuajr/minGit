using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;

public partial class Workarea_controls_menu_editmenu : System.Web.UI.UserControl
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper MsgHelper;
    protected Collection gtLinks = new Collection();
    protected string AssociatedFolderIdListString = "";
    protected string AssociatedFolderTitleListString = "";
    protected string AssociatedTemplatesString = "";
    protected long FolderId = 0;
    protected long MenuId = 0;
    protected string sitePath = "";
    protected string AppPath = "";
    protected string LanguageName = "";
    protected Int32 ContentLanguage = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        CommonApi m_refApi = new CommonApi();
        ApplicationAPI AppUI = new ApplicationAPI();
        sitePath = AppUI.SitePath;
        AppPath = AppUI.AppPath;
	    AxMenuData menuData = new AxMenuData();
        string ErrorString ="";
        MenuId = Convert.ToInt64(Request.QueryString["nid"]);
        FolderId = Convert.ToInt64(Request.QueryString["folderid"]);
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
        Hashtable cPerms = AppUI.EkSiteRef.GetPermissions(FolderId, 0, "folder");
        if (!(IsCollectionMenuRoleMember() || (cPerms.Contains("Collections") && Convert.ToBoolean(cPerms["Collections"]))))
            ErrorString = MsgHelper.GetMessage("com: user does not have permission");
        
        if (ErrorString == "")
        {
            if (m_refApi.TreeModel == 1)
			    gtLinks = AppUI.EkContentRef.GetMenuByID(MenuId, 0,false);
            else
                gtLinks = AppUI.EkContentRef.GetMenuByID(MenuId, 0, true);
            menuData = AppUI.EkContentRef.GetMenuDataByID(MenuId);
            if (!string.IsNullOrEmpty(menuData.AssociatedFolderIdList))
            {
			    AssociatedFolderIdListString = menuData.AssociatedFolderIdList;
			    AssociatedFolderTitleListString = GetTitlesFromFolderIds(menuData.AssociatedFolderIdList);
            }
            if (!string.IsNullOrEmpty(menuData.AssociatedTemplates))
			    AssociatedTemplatesString = menuData.AssociatedTemplates;
        }
        litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("edit menu title"));
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("editmenu", "") + "</td>";

        if (Request.QueryString["back"] != "")
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", Request.QueryString["back"], MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "", StyleHelper.CancelButtonCssClass, true);
        else if (Request.QueryString["iframe"] == "true")
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true);
        else
        {
            if (m_refApi.TreeModel == 1)
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "menutree.aspx?nid=" + MenuId + "&folderid=" + FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            else
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" + MenuId + "&folderid=" + FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
		}

		litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", "Save Menu", MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'menu\', \'VerifyMenuForm()\');\"", StyleHelper.SaveButtonCssClass, true);

        LanguageName = gtLinks["ContentLanguage"].ToString();
    }
    protected bool IsCollectionMenuRoleMember()
    {
        CommonApi m_refApi = new CommonApi();
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu) || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu);
    }
    private String GetTitlesFromFolderIds(string associatedFolderIdList)
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        string result = String.Empty;
        string[] listArray = associatedFolderIdList.Split(';');
        foreach (string lsArray in listArray)
        {
            if (lsArray.Length > 0)
            {
                if (Char.IsNumber(lsArray, 0))
                {
                    if (result.Length > 0)
                        result += ";";
                    result += AppUI.EkContentRef.GetFolderPath(Convert.ToInt64(lsArray));
                }
            }
        }
        return result;
    }
}
