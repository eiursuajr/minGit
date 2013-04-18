using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using System.Collections;
using Ektron.Cms.Content;
using Microsoft.VisualBasic;
using Ektron.Cms.Common;
using Ektron.Cms;

public partial class Workarea_controls_collection_editcollection : System.Web.UI.UserControl
{
    protected CommonApi m_refApi = new CommonApi();
    protected string checkout = "";
    protected long folderId = -1;
    protected long nId = 0;
    protected Collection gtNavs = new Collection();
    protected void Page_Load(object sender, EventArgs e)
    {
        ContentAPI m_refContentApi = new ContentAPI();
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        ApplicationAPI AppUI = new ApplicationAPI();
        string action = "";
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            action = Request.QueryString["action"].ToString();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["folderid"]))
        {
            folderId = Convert.ToInt64(Request.QueryString["folderid"]);
            Hashtable cPerms = AppUI.EkSiteRef.GetPermissions(folderId, 0, "folder");
            nId = Convert.ToInt64(Request.QueryString["nid"]);
            EkContent gtObj = AppUI.EkContentRef; 
            
            if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
                checkout = "&checkout=" + Request.QueryString["checkout"].ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["status"]))
            {
                if (Request.QueryString["status"].ToString().ToLower() == "o")
                    checkout = checkout + "&status=o";
            }

            
            string ErrorString = "";
            
            if (ErrorString == "")
            {
                if (gtNavs.Count > 0)
                {
                     string CollectionTitle = gtNavs["CollectionTitle"].ToString();
                }
            }
            if (checkout != "")
                gtNavs = gtObj.GetEcmStageCollectionByID(nId, false, false, ref ErrorString, true, false, true);
            else
                gtNavs = gtObj.GetEcmCollectionByID(nId, false, false, ref ErrorString, true, false, true);
            divErrorString.InnerHtml = ErrorString;
            litEditColl.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("edit collection title"));
            litEditCollectionTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("edit collection title"));
			litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update collection text"), MsgHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'nav\', \'VerifyCollectionForm()\');\"", StyleHelper.SaveButtonCssClass, true);
            litSearch.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton(action, "") + "</td>";
            litTitle.Text = MsgHelper.GetMessage("generic title label");
            litIDLabel.Text = MsgHelper.GetMessage("id label");
            litPath.Text = MsgHelper.GetMessage("lbl path") + ":";
            litTemplate.Text = MsgHelper.GetMessage("generic template label");
            litDesc.Text = MsgHelper.GetMessage("description label");
            litSitePath.Text = AppUI.SitePath;
            litLeaveTemplate.Text = MsgHelper.GetMessage("collections: leave template empty");
            litInclueSub.Text = MsgHelper.GetMessage("generic include subfolders msg");
            litApproval.Text = MsgHelper.GetMessage("lbl approval required");
            //// NEED SOME LOGIC HERE
            frm_nav_template.Value = gtNavs["CollectionTemplate"].ToString();
            trID.InnerHtml = gtNavs["CollectionID"].ToString();
            if (gtNavs.Contains("FolderPath") && null != gtNavs["FolderPath"] && !string.IsNullOrEmpty(gtNavs["FolderPath"].ToString())) {
                lblFolderPath.Text = gtNavs["FolderPath"].ToString();
                trFolderPath.Visible = true;
                litFolderSelect.Text = (m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/folderopen.png", "#", MsgHelper.GetMessage("alt select folder"), MsgHelper.GetMessage("btn select folder"), "onclick=\"LoadFolderChildPage(\'\',\'" + AppUI.ContentLanguage.ToString() + "\');\"")).Replace("<td ", "<span ").Replace("</td>", "</span>");
            }
            trApproval.Visible = (m_refApi.IsAdmin()
                || IsCollectionMenuAdmin()
                || IsCollectionApprover()
                || (IsFolderAdmin(Convert.ToInt64(gtNavs["FolderID"]))));
        }
    }
    private bool IsFolderAdmin(long folderId) {
        return m_refApi.IsARoleMemberForFolder_FolderUserAdmin(folderId, 1, false);
    }
    private bool IsCollectionMenuAdmin() {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu) 
            || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection);
    }
    private bool IsCollectionApprover() {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers);
    }
}