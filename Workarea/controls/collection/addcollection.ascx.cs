using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using System.Collections;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;

public partial class Workarea_controls_collection_AddCollection : System.Web.UI.UserControl
{
    protected CommonApi m_refApi = new CommonApi();
    protected EkMessageHelper MsgHelper;
    protected string ErrorString = "";
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected void Page_Load(object sender, EventArgs e)
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        MsgHelper = new EkMessageHelper(AppUI.RequestInformationRef);
        if (!string.IsNullOrEmpty(Request.QueryString["folderid"]))
        {
            long folderId = Convert.ToInt64(Request.QueryString["folderid"]);
            Hashtable cPerms = AppUI.EkSiteRef.GetPermissions(folderId, 0, "folder");
            trPer.Visible = (m_refApi.IsAdmin()
                || IsCollectionMenuAdmin()
                || IsCollectionApprover()
                || (IsFolderAdmin(folderId)));
            if ((!IsCollectionMenuRoleMember()) && (!((cPerms.Contains("Collections") && Convert.ToBoolean(cPerms["Collections"])))))
            {
                ErrorString = MsgHelper.GetMessage("com: user does not have permission");
                divErrors.Visible = true;
                divMain.Visible = false;
            }
            cPerms = null;
            jsFolderId.Text = folderId.ToString();
            divErrors.InnerHtml = ErrorString;
            if (!string.IsNullOrEmpty(Request.QueryString["back"]))
            {
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            }
            else
            {
                litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            }
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: save collection text"), MsgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'nav\', \'VerifyCollectionForm()\');\"", StyleHelper.SaveButtonCssClass, true);
        }
        
        litAddCollectionTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("add collection title"));
        litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("Add", "").ToString() + "</td>";
        litGeneric.Text = MsgHelper.GetMessage("generic title label");
        litTemplate.Text = MsgHelper.GetMessage("generic template label");
        litLeave.Text = MsgHelper.GetMessage("collections: leave template empty");
        litDesc.Text = MsgHelper.GetMessage("description label");
        litInclude.Text = MsgHelper.GetMessage("generic include subfolders msg");
        litApprove.Text = MsgHelper.GetMessage("lbl approval required");
        AddCollection.Text = MsgHelper.GetMessage("add collection title");
        litSitePath.Text = AppUI.SitePath; 
    }
    private bool IsFolderAdmin(long folderId)
    {
        return m_refApi.IsARoleMemberForFolder_FolderUserAdmin(folderId, 1, false);
    }
    private bool IsCollectionMenuRoleMember()
    {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection);
    }
    private bool IsCollectionMenuAdmin() {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection);
    }
    private bool IsCollectionApprover() {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers);
    }
}