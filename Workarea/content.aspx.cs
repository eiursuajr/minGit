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
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Microsoft.Security.Application;

public partial class content : System.Web.UI.Page
{
    #region Member Variables

    protected enum Flag
    {
        Disable,
        Enable
    }
    string MasterLayouts = string.Empty;
    protected bool m_bAjaxTree = false;
    protected viewfolder m_viewfolder;
    protected string PageAction = "";
    protected EkMessageHelper m_refMsg;
    protected int ContentLanguage = -1;
    protected long m_intFolderId = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected bool m_bLangChange = false;
    protected string m_strReloadJS = "";
    protected string m_strEmailArea = "";
    protected string m_strMembership = "";
    protected SyncResources m_jsResources;
    protected Ektron.Cms.Commerce.CatalogEntry m_refCatalog = null;

    private string _ApplicationPath;
    private string _SitePath;

    #endregion

    #region Properties

    public string SitePath
    {
        get
        {
            return _SitePath;
        }
        set
        {
            _SitePath = value;
        }
    }

    public string ApplicationPath
    {
        get
        {
            return _ApplicationPath;
        }
        set
        {
            _ApplicationPath = value;
        }
    }

    #endregion

    #region Constructor

    public content()
    {
        char[] endSlash = new char[] { '/' };
        this.ApplicationPath = m_refContentApi.ApplicationPath.TrimEnd(endSlash.ToString().ToCharArray());
        this.SitePath = m_refContentApi.SitePath.TrimEnd(endSlash.ToString().ToCharArray());
        m_refMsg = m_refContentApi.EkMsgRef;
    }

    #endregion

    #region PreInit, Load, PreRender

    private void Page_PreInit(System.Object sender, System.EventArgs e)
    {

        if (Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser))
        {
            Response.Redirect((string)("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user")), true);
            return;
        }

        if (!(Request.QueryString["action"] == null))
        {
            PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }

        bool bCompleted;
        switch (PageAction)
        {
            case "view":
            case "viewstaged":
                UniqueLiteral.Text = "content";
                viewcontent m_viewcontent;
                m_viewcontent = (viewcontent)(LoadControl("controls/content/viewcontent.ascx"));
                m_viewcontent.ID = "content";
                DataHolder.Controls.Add(m_viewcontent);
                bCompleted = m_viewcontent.ViewContent();
                this.ltr_commerceCSSJS.Text = m_viewcontent.GetCommerceIncludes();
                break;
        }

        //Load Conditional JS
        if (Strings.Trim(Request.QueryString["ShowTStatusMsg"]) != null)
        {
            if (Strings.Trim(Request.QueryString["ShowTStatusMsg"]) == "1")
            {
                phShowTStatusMessage.Visible = true;
            }
        }
        if (m_bAjaxTree == true)
        {
            phShowAjaxTree.Visible = true;
        }
    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        m_refMsg = m_refContentApi.EkMsgRef;
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return;
        }
        this.ShowAjaxTreeJsValues();
        this.SetEktronContentTemplateJsValues();
        this.RegisterJs();
        this.RegisterCss();

        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        if (m_refContentApi.GetCookieValue("user_id") == "0")
        {
            if (!(Request.QueryString["callerpage"] == null))
            {
                Session["RedirectLnk"] = "Content.aspx?" + AntiXss.UrlEncode(Request.QueryString.ToString());
            }
            Response.Redirect("login.aspx?fromLnkPg=1", false);
            return;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }

        if (!string.IsNullOrEmpty(Request.QueryString["membership"]))
        {
            m_strMembership = Convert.ToString(Request.QueryString["membership"]).ToLower().Trim();
        }

        if (m_refContentApi.TreeModel == 1)
        {
            m_bAjaxTree = true;
        }

        Int32 lastValidLanguageID;
        string LastValidLanguage = m_refContentApi.GetCookieValue("LastValidLanguageID");
        if (LastValidLanguage == null || !Int32.TryParse(LastValidLanguage, out lastValidLanguageID)) lastValidLanguageID = Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED;
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                if (Request.QueryString["LangType"] != null) Int32.TryParse(Request.QueryString["LangType"], out ContentLanguage);

                if (ContentLanguage != lastValidLanguageID)
                {
                    m_bLangChange = true;
                }
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (lastValidLanguageID != Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
                {
                    ContentLanguage = lastValidLanguageID;
                }
            }
        }
        else
        {
            if (lastValidLanguageID != Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                ContentLanguage = lastValidLanguageID;
            }
        }

        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContentApi.ContentLanguage = ContentLanguage;
        }
        StyleHelper styleHelper = new StyleHelper();
        ltrStyleSheetJs.Text = styleHelper.GetClientScript();
        txtContentLanguage.Text = m_refContentApi.ContentLanguage.ToString();
        txtDefaultContentLanguage.Text = m_refContentApi.DefaultContentLanguage.ToString();
        txtEnableMultilingual.Text = m_refContentApi.EnableMultilingual.ToString();

        switch (PageAction)
        {
            case "viewarchivecontentbycategory":
            case "viewcontentbycategory":
                UniqueLiteral.Text = "viewfolder";
                m_viewfolder = (viewfolder)(LoadControl("controls/folder/viewfolder.ascx"));
                m_viewfolder.ID = "viewfolder";
                if (m_bLangChange == true)
                {
                    m_viewfolder.ResetPostData();
                }
                DataHolder.Controls.Add(m_viewfolder);
                break;
            case "deletecontentbycategory":
                UniqueLiteral.Text = "deletecontentbycategory";
                removefolderitem m_removefolderitem;
                m_removefolderitem = (removefolderitem)(LoadControl("controls/folder/removefolderitem.ascx"));
                m_removefolderitem.ID = "deletecontentbycategory";
                DataHolder.Controls.Add(m_removefolderitem);
                break;
            case "movecontentbycategory":
                UniqueLiteral.Text = "movefolderitem";
                movefolderitem m_movefolderitem;
                m_movefolderitem = (movefolderitem)(LoadControl("controls/folder/movefolderitem.ascx"));
                m_movefolderitem.ID = "movefolderitem";
                DataHolder.Controls.Add(m_movefolderitem);
                break;
            case "selectpermissions":
                UniqueLiteral.Text = "permission";
                selectpermissions m_selectpermissions;
                m_selectpermissions = (selectpermissions)(LoadControl("controls/permission/selectpermissions.ascx"));
                m_selectpermissions.ID = "permission";
                DataHolder.Controls.Add(m_selectpermissions);
                break;
        }
        EmailHelper m_mail = new EmailHelper();

        string strEmailArea;
        strEmailArea = m_mail.EmailJS();
        strEmailArea += m_mail.MakeEmailArea();
        ltrEmailAreaJs.Text = strEmailArea;

        if (PageAction.ToLower().ToString() != "viewcontentbycategory")
        {
            ShowDropUploader(false);
        }

        // resource text string tokens
        string closeDialogText = m_refMsg.GetMessage("close this dialog");
        string cancelText = m_refMsg.GetMessage("btn cancel");

        // assign resource text string values
        btnConfirmOk.Text = m_refMsg.GetMessage("lbl ok");
        btnConfirmOk.NavigateUrl = "#" + m_refMsg.GetMessage("lbl ok");
        btnConfirmCancel.Text = cancelText;
        btnConfirmCancel.NavigateUrl = "#" + cancelText;
        btnCloseSyncStatus.Text = m_refMsg.GetMessage("close title");
        btnCloseSyncStatus.NavigateUrl = "#" + m_refMsg.GetMessage("close title");
        btnStartSync.Text = m_refMsg.GetMessage("btn sync now");
        btnCloseConfigDialog.Text = m_refMsg.GetMessage("close title");
        closeDialogLink.Text = "<span class=\"ui-icon ui-icon-closethick\">" + m_refMsg.GetMessage("close title") + "</span>";
        closeDialogLink.NavigateUrl = "#" + System.Text.RegularExpressions.Regex.Replace(m_refMsg.GetMessage("close title"), "\\s+", "");
        closeDialogLink.ToolTip = closeDialogText;
        closeDialogLink2.Text = closeDialogLink.Text;
        closeDialogLink2.NavigateUrl = closeDialogLink.NavigateUrl;
        closeDialogLink2.ToolTip = closeDialogText;
        closeDialogLink3.Text = closeDialogLink.Text;
        closeDialogLink3.NavigateUrl = closeDialogLink.NavigateUrl;
        closeDialogLink3.ToolTip = closeDialogText;
        lblSyncStatus.Text = string.Format(m_refMsg.GetMessage("lbl sync status"), " <span class=\"statusHeaderProfileId\"></span>");
        m_jsResources = (SyncResources)(LoadControl("sync/SyncResources.ascx"));
        m_jsResources.ID = "jsResources";
        sync_jsResourcesPlaceholder.Controls.Add(m_jsResources);
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        bool bCompleted;
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]))
            {
                ltrEktronReloadJs.Text = ReloadClientScript();
            }
            switch (PageAction)
            {
                case "addsubfolder":
                    UniqueLiteral.Text = "addfolder";
                    addfolder m_addfolder;
                    m_addfolder = (addfolder)(LoadControl("controls/folder/addfolder.ascx"));
                    m_addfolder.ID = "adddfolder";
                    DataHolder.Controls.Add(m_addfolder);
                    bCompleted = m_addfolder.AddSubFolder();
                    break;
                case "viewfolder":
                    UniqueLiteral.Text = "viewfolder";
                    viewfolderattributes m_viewfolderattributes;
                    m_viewfolderattributes = (viewfolderattributes)(LoadControl("controls/folder/viewfolderattributes.ascx"));
                    m_viewfolderattributes.ID = "viewfolder";
                    DataHolder.Controls.Add(m_viewfolderattributes);
                    bCompleted = m_viewfolderattributes.ViewFolderAttributes();
                    break;
                case "editfolder":
                    UniqueLiteral.Text = "editfolder";
                    editfolderattributes m_editfolderattributes;
                    m_editfolderattributes = (editfolderattributes)(LoadControl("controls/folder/editfolderattributes.ascx"));
                    m_editfolderattributes.ID = "editfolder";
                    DataHolder.Controls.Add(m_editfolderattributes);
                    bCompleted = m_editfolderattributes.EditFolderAttributes();
                    break;
                case "editcontentproperties":
                    UniqueLiteral.Text = "content";
                    editcontentattributes m_editcontentattributes;
                    m_editcontentattributes = (editcontentattributes)(LoadControl("controls/content/editcontentattributes.ascx"));
                    m_editcontentattributes.ID = "content";
                    DataHolder.Controls.Add(m_editcontentattributes);
                    bCompleted = m_editcontentattributes.EditContentProperties();
                    break;
                case "movecontent":
                    UniqueLiteral.Text = "content";
                    movecontent m_movecontent;
                    m_movecontent = (movecontent)(LoadControl("controls/content/movecontent.ascx"));
                    m_movecontent.ID = "content";
                    DataHolder.Controls.Add(m_movecontent);
                    bCompleted = m_movecontent.MoveContent();
                    break;
                case "localize":
                case "localizeexport":
                    UniqueLiteral.Text = "localize";
                    localization_uc m_localization;
                    m_localization = (localization_uc)(LoadControl("controls/content/localization_uc.ascx"));
                    m_localization.ID = "localization";
                    DataHolder.Controls.Add(m_localization);
                    bCompleted = m_localization.Display();
                    break;
                case "viewpermissions":
                    UniqueLiteral.Text = "permission";
                    viewpermissions m_viewpermissions;
                    m_viewpermissions = (viewpermissions)(LoadControl("controls/permission/viewpermissions.ascx"));
                    m_viewpermissions.ID = "permission";
                    DataHolder.Controls.Add(m_viewpermissions);
                    bCompleted = m_viewpermissions.ViewPermission();
                    break;
                case "deletepermissions":
                    UniqueLiteral.Text = "permission";
                    deletepermissions m_deletepermissions;
                    m_deletepermissions = (deletepermissions)(LoadControl("controls/permission/deletepermissions.ascx"));
                    m_deletepermissions.ID = "permission";
                    DataHolder.Controls.Add(m_deletepermissions);
                    bCompleted = m_deletepermissions.DeletePermission();
                    break;
                case "editpermissions":
                    UniqueLiteral.Text = "permission";
                    editpermissions m_editpermissions_1;
                    m_editpermissions_1 = (editpermissions)(LoadControl("controls/permission/editpermissions.ascx"));
                    m_editpermissions_1.ID = "permission";
                    DataHolder.Controls.Add(m_editpermissions_1);
                    bCompleted = m_editpermissions_1.EditPermission();
                    break;
                case "addpermissions":
                    UniqueLiteral.Text = "permission";
                    editpermissions m_editpermissions;
                    m_editpermissions = (editpermissions)(LoadControl("controls/permission/editpermissions.ascx"));
                    m_editpermissions.ID = "permission";
                    DataHolder.Controls.Add(m_editpermissions);
                    bCompleted = m_editpermissions.AddPermission();
                    break;
                case "viewapprovals":
                    UniqueLiteral.Text = "approval";
                    viewapprovals m_viewapprovals;
                    m_viewapprovals = (viewapprovals)(LoadControl("controls/approval/viewapprovals.ascx"));
                    m_viewapprovals.ID = "approval";
                    DataHolder.Controls.Add(m_viewapprovals);
                    bCompleted = m_viewapprovals.ViewApproval();
                    break;
                case "editapprovalmethod":
                    UniqueLiteral.Text = "approval";
                    editapprovalmethod m_editapprovalmethod;
                    m_editapprovalmethod = (editapprovalmethod)(LoadControl("controls/approval/editapprovalmethod.ascx"));
                    m_editapprovalmethod.ID = "approval";
                    DataHolder.Controls.Add(m_editapprovalmethod);
                    bCompleted = m_editapprovalmethod.EditApprovalMethod();
                    break;
                case "editpreapprovals":
                    UniqueLiteral.Text = "preapproval";
                    editpreapproval m_editpreapproval;
                    m_editpreapproval = (editpreapproval)(LoadControl("controls/approval/editpreapproval.ascx"));
                    m_editpreapproval.ID = "preapproval";
                    DataHolder.Controls.Add(m_editpreapproval);
                    bCompleted = m_editpreapproval.EditPreApproval();
                    break;
                case "addapproval":
                    UniqueLiteral.Text = "approval";
                    addapproval m_addapproval;
                    m_addapproval = (addapproval)(LoadControl("controls/approval/addapproval.ascx"));
                    m_addapproval.ID = "approval";
                    DataHolder.Controls.Add(m_addapproval);
                    bCompleted = m_addapproval.AddApproval();
                    break;
                case "editapprovalorder":
                    UniqueLiteral.Text = "approval";
                    editapprovalorder m_editapprovalorder;
                    m_editapprovalorder = (editapprovalorder)(LoadControl("controls/approval/editapprovalorder.ascx"));
                    m_editapprovalorder.ID = "approval";
                    DataHolder.Controls.Add(m_editapprovalorder);
                    bCompleted = m_editapprovalorder.EditApprovalOrder();
                    break;
                case "deleteapproval":
                    UniqueLiteral.Text = "approval";
                    deleteapproval m_deleteapproval;
                    m_deleteapproval = (deleteapproval)(LoadControl("controls/approval/deleteapproval.ascx"));
                    m_deleteapproval.ID = "approval";
                    DataHolder.Controls.Add(m_deleteapproval);
                    bCompleted = m_deleteapproval.DeleteApproval();
                    break;
                case "enableitemprivatesetting":
                    Process_DoItemPrivateSetting(Flag.Enable);
                    break;
                case "disableitemprivatesetting":
                    Process_DoItemPrivateSetting(Flag.Disable);
                    break;
                case "enableiteminheritance":
                    Process_DoItemInheritance(Flag.Enable);
                    break;
                case "disableiteminheritance":
                    Process_DoItemInheritance(Flag.Disable);
                    break;
                case "dodeletefolder":
                    Process_DoDeleteFolder();
                    break;
                case "reestablishsession":
                    Process_DoReestablishSession();
                    break;
                case "checkin":
                    Process_CheckInContent();
                    break;
                case "submit":
                    Process_DoSubmit();
                    break;
                case "dodeletepermissions":
                    Process_DoDeletePermission();
                    break;
                case "doadditemapproval":
                    Process_DoAddItemApproval();
                    break;
                case "dodeleteitemapproval":
                    Process_DoDeleteItemApproval();
                    break;
                case "submitdelcontaction":
                    Process_DeleteContent();
                    break;
                case "submitdelcatalogaction":
                    Process_DeleteContent();
                    break;
                case "workoffline":
                    Process_WorkOffLine();
                    break;
                case "approvecontent":
                    Do_ApproveContent();
                    break;
                case "declinecontent":
                    Do_DeclineContent();
                    break;
                case "restoreinheritance":
                    Do_RestoreInheritance();
                    break;
                case "translatereadystatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
                case "translatenotreadystatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
                case "translatenotallowstatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
                case "translateneedstranslationstatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
                case "translateoutfortranslationstatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
                case "translatetranslatedstatus":
                    Do_UpdateTranslationStatus(PageAction);
                    break;
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
        AssignTextValues();
    }
    #endregion

    #region Helper Methods
    protected void ShowAjaxTreeJsValues()
    {
        litShowAjaxTreeFolderId.Text = Request.QueryString["id"];
    }
    protected void SetEktronContentTemplateJsValues()
    {
        //This method populates literals for <script id="EktronContentTemplateJs">
        litConfirmContentDeletePublish.Text = m_refMsg.GetMessage("js: confirm content delete");
        litConfirmContentDeleteSubmission.Text = m_refMsg.GetMessage("js: confirm content delete submission");
        litConfirmContentDeleteDialog.Text = m_refMsg.GetMessage("js: confirm content delete");
        litConfirmFolderDelete.Text = m_refMsg.GetMessage("js: confirm folder delete");
        litConfirmFolderDeleteBelowRoot.Text = m_refMsg.GetMessage("js: confirm delete folders below root");
        litAlertSupplyFoldername.Text = m_refMsg.GetMessage("js: alert supply foldername");
        litAlertRequiredDomain.Text = m_refMsg.GetMessage("js required domain msg");
        litAlertMissingAlternateStylesheet.Text = m_refMsg.GetMessage("js: alert stylesheet must have css");
        litConfirmDeleteGroupPermissions.Text = m_refMsg.GetMessage("js: confirm delete group permissions");
        litConfirmDeleteUserPermissions.Text = m_refMsg.GetMessage("js: confirm delete user permissions");
        litAlertCannotDisableReadonly.Text = m_refMsg.GetMessage("js: alert cannot disable readonly");
        litAlertCannotDisableLibraryReadonly.Text = m_refMsg.GetMessage("js: alert cannot disable library readonly");
        litAlertCannotDisablePostReply.Text = m_refMsg.GetMessage("js: alert cannot disable postreply");
        litAlertCannotDisableEdit.Text = m_refMsg.GetMessage("js: alert cannot disable edit");
        litAlertSelectPermission.Text = m_refMsg.GetMessage("js: alert select permission");
        litAlertReadContentPermissionRemovalEffectWarning.Text = m_refMsg.GetMessage("js: read content permission removal effect warning");
        litConfirmDisableInheritance.Text = m_refMsg.GetMessage("js: confirm disable inheritance");
        litConfirmEnableInheritance.Text = m_refMsg.GetMessage("js: confirm enable inheritance");
        litConfirmMakeContentPrivate.Text = m_refMsg.GetMessage("js: confirm make content private");
        litConfirmMakeContentPublic.Text = m_refMsg.GetMessage("js: confirm make content public");
        litAlertCannotAlterPContSetting.Text = m_refMsg.GetMessage("js: alert cannot alter pcont setting");
        litConfirmMakeFolderPrivate.Text = m_refMsg.GetMessage("js: confirm make folder private");
        litConfirmMakeFolderPublic.Text = m_refMsg.GetMessage("js: confirm make folder public");
        litConfirmAddApproverGroup.Text = m_refMsg.GetMessage("js: confirm add approver group");
        litConfirmAddApproverUser.Text = m_refMsg.GetMessage("js: confirm add approver user");
        litConfirmDeleteApproverGroup.Text = m_refMsg.GetMessage("js: confirm delete approver group");
        litConfirmDeleteApproverUser.Text = m_refMsg.GetMessage("js: confirm delete approver user");
        litAlertSelectUserOrGroup.Text = m_refMsg.GetMessage("js select user or group");
        litAlertMetadataNotCompleted.Text = m_refMsg.GetMessage("js: alert meta data not completed");
        litContentTypeFolderId.Text = Request.QueryString["id"];
        litTemplateConfigSaveFolderId.Text = Request.QueryString["id"];
        litConfirmBreakInheritanceFlagging.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        litConfirmBreakInheritanceAliasing.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        litConfirmBreakInheritIsContentSearchable.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        litConfirmBreakInheritIsDisplaySettings.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        litSelectPreapprovalGroup.Text = m_refMsg.GetMessage("js: sel preapporval group");
        if (m_strMembership.ToLower().Trim() == "true")
        {
            litDisableInheritenceIfMembershipTrue.Text = "true";
            litEnableInheritenceIfMembershipTrue.Text = "true";
            litEnableItemPrivateSettingMembershipTrue.Text = "true";
            litDisableItemPrivateSettingMembershipTrue.Text = "true";
        }
        litSelectValidTemplate.Text = m_refMsg.GetMessage("lbl please select a valid template");
    }
    private string ReloadClientScript()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        long pid = 0;
        string FolderPath = "";
        try
        {
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                pid = Convert.ToInt64(Request.QueryString["id"].ToString());
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["folder_id"]))
            {
                pid = Convert.ToInt64(Request.QueryString["folder_id"].ToString());
            }

            Ektron.Cms.Content.EkContent contObj = m_refContentApi.EkContentRef;
            if (pid >= 0)
            {
                FolderPath = contObj.GetFolderPath(pid);
            }
            contObj = null;
            result.Append("<script language=\"javascript\">" + "\r\n");
            if (m_refContentApi.TreeModel == 1 && pid != 0)
            {
                if ((!string.IsNullOrEmpty(Request.QueryString["TreeUpdated"])) && (Request.QueryString["TreeUpdated"] == "1"))
                {
                    pid = m_refContentApi.GetParentIdByFolderId(pid);
                    if (pid == -1)
                    {
                        result.Length = 0;
                        return result.ToString();
                    }
                }
                result.Append("if (typeof (reloadFolder) == \'function\'){" + "\r\n");
                result.Append("     reloadFolder(" + Convert.ToString(pid) + ");" + "\r\n");
                result.Append("}" + "\r\n");
                FolderPath = FolderPath.Replace("\\", "\\\\");
                result.Append("top.TreeNavigation(\"LibraryTree\", \"" + FolderPath + "\");" + "\r\n");
                result.Append("top.TreeNavigation(\"ContentTree\", \"" + FolderPath + "\");" + "\r\n");
                if (PageAction == "viewboard")
                {
                    result.Append("window.location.href = \'threadeddisc/addeditboard.aspx?action=View&id=" + Request.QueryString["id"] + "\';" + "\r\n");
                }
            }
            else
            {
                result.Append("<!--" + "\r\n");
                result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" + "\r\n");
                result.Append("	var m_reloadTrees = \"" + Request.QueryString["reloadtrees"] + "\";" + "\r\n");
                result.Append("	top.ReloadTrees(m_reloadTrees);" + "\r\n");
                result.Append("	self.location.href=\"" + Request.ServerVariables["path_info"] + "?" + Strings.Replace(Request.ServerVariables["query_string"], (string)("&reloadtrees=" + Request.QueryString["reloadtrees"]), "", 1, -1, 0) + "\";" + "\r\n");
                result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" + "\r\n");
                result.Append("	var strTreeNav = \"" + Request.QueryString["TreeNav"] + "\";" + "\r\n");
                result.Append("	if (strTreeNav != null) {" + "\r\n");
                result.Append("		strTreeNav = strTreeNav.replace(/\\\\\\\\/g,\"\\\\\");" + "\r\n");
                result.Append("		top.TreeNavigation(\"LibraryTree\", strTreeNav);" + "\r\n");
                result.Append("		top.TreeNavigation(\"ContentTree\", strTreeNav);" + "\r\n");
                result.Append("	}" + "\r\n");
                result.Append("//-->" + "\r\n");
            }
            result.Append("</script>" + "\r\n");
        }
        catch (Exception)
        {
        }
        return (result.ToString());
    }
    private void ShowDropUploader(bool bShow)
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();
        sJS.Append("<script language=\"Javascript\">" + "\r\n");
        if (bShow)
        {
            sJS.Append(" if (typeof top != \'undefined\') { " + "\r\n");
            sJS.Append("    top.ShowDragDropWindow();" + "\r\n");
            sJS.Append(" }" + "\r\n");
        }
        else
        {
            sJS.Append("if ((typeof(top.GetEkDragDropObject)).toLowerCase() != \'undefined\') {" + "\r\n");
            sJS.Append("	var dragDropFrame = top.GetEkDragDropObject();" + "\r\n");
            sJS.Append("		if (dragDropFrame != null) {" + "\r\n");
            sJS.Append("			dragDropFrame.location.href = \"blank.htm\";" + "\r\n");
            sJS.Append("		}" + "\r\n");
            sJS.Append("}" + "\r\n");
            sJS.Append("if ((typeof(top.GetEkDragDropObject)).toLowerCase() != \'undefined\') {" + "\r\n");
            sJS.Append("	top.HideDragDropWindow();" + "\r\n");
            sJS.Append("}" + "\r\n");
        }
        sJS.Append("</script>" + "\r\n");
        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "DragUploaderJS", sJS.ToString());
    }
    #endregion

    #region SUBMIT ACTION PROCESS
    #region ACTION - CheckIn
    private void Process_CheckInContent()
    {
        string strCallBackPage = "";
        Ektron.Cms.Content.EkContent m_refContent;
        long m_intId;
        StyleHelper m_refStyle = new StyleHelper();
        string AssetFileName = "";
        int contentType = 0;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            m_intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            if (!String.IsNullOrEmpty(Request.QueryString["content_type"]))
            {
                contentType = Convert.ToInt32(Request.QueryString["content_type"].ToString());
            }

            switch (contentType)
            {
                case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:
                    m_refCatalog = new Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef);
                    Ektron.Cms.Commerce.EntryData entry = m_refCatalog.GetItemEdit(m_intId, m_refContentApi.RequestInformationRef.ContentLanguage, false);
                    if ((entry != null) && (entry.LastEditorId != m_refContentApi.RequestInformationRef.UserId) && (entry.Status == "O"))
                    {
                        m_refCatalog.TakeOwnershipForAdminCheckIn(m_intId, m_refContentApi.RequestInformationRef.UserId, null);
                    }
                    m_refCatalog.UpdateAndCheckIn(entry);
                    break;
                default:
                    if (!string.IsNullOrEmpty(Request.QueryString["asset_assetfilename"]))
                    {
                        AssetFileName = Request.QueryString["asset_assetfilename"];
                    }
                    ContentData contData = m_refContentApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged);
                    if ((contData != null) && (contData.UserId != m_refContentApi.RequestInformationRef.UserId) && (contData.Status == "O"))
                    {
                        m_refContent.TakeOwnershipForAdminCheckIn(m_intId);
                    }

                    if ((!string.IsNullOrEmpty(Request.QueryString["content_type"])) && (int.TryParse(Request.QueryString["content_type"], out contentType)) && (Ektron.Cms.Common.EkConstants.IsAssetContentType(contentType, true)))                    {
                        ContentEditData cEditData = m_refContentApi.GetContentForEditing(m_intId);
                        cEditData.FileChanged = false;
                        m_refContentApi.SaveContent(cEditData);
                    }

                    m_refContent.CheckIn(m_intId, AssetFileName);
                    break;
            }
            strCallBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId);
            Response.Redirect(strCallBackPage, false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
        }
    }
    #endregion
    #region ACTION - Submit
    private void Process_DoSubmit()
    {
        string strCallBackPage = "";
        Ektron.Cms.Content.EkTask m_refTask;
        SettingsData settings_data;
        string strTaskName = "";
        bool IsAlreadyCreated = false;
        Ektron.Cms.Content.EkContent m_refContent;
        long m_intId;
        StyleHelper m_refStyle = new StyleHelper();
        long CurrentUserId;
        long contentType = 0;
        try
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            m_refContent = m_refContentApi.EkContentRef;
            CurrentUserId = m_refContentApi.UserId;
            m_refTask = m_refContentApi.EkTaskRef;
            if (Convert.ToString(m_intId) != "")
            {
                settings_data = (new SiteAPI()).GetSiteVariables(CurrentUserId);
                contentType = m_refContent.GetContentType(m_intId);

                switch (contentType)
                {
                    case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:
                        m_refCatalog = new Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef);
                        Ektron.Cms.Commerce.EntryData entry = m_refCatalog.GetItemEdit(m_intId, m_refContentApi.RequestInformationRef.ContentLanguage, false);
                        m_refCatalog.Publish(entry, null);
                        break;
                    default:
                        long PreapprovalGroupID;
                        Collection cPreApproval = new Collection();
                        cPreApproval = m_refContent.GetFolderPreapprovalGroup(Convert.ToInt64(Request.QueryString["fldid"].ToString()));
                        PreapprovalGroupID = System.Convert.ToInt32(cPreApproval["UserGroupID"]);

                        if (PreapprovalGroupID > 0)
                        {
                            if (ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES) //1 removed with ALL_CONTENT_LANGUAGES
                            {
                                strTaskName = (string)(Request.Form["content_title"] + m_intId + "_Task");
                            }
                            else
                            {
                                strTaskName = (string)(Request.Form["content_title"] + m_intId + "_Task" + ContentLanguage);
                            }
                            m_refTask.ContentLanguage = ContentLanguage;
                            m_refTask.LanguageID = ContentLanguage;
                            IsAlreadyCreated = m_refTask.IsTaskAlreadyCreated(m_intId);
                            if (IsAlreadyCreated == false)
                            {
                                m_refTask.LanguageID = ContentLanguage;
                                m_refTask.TaskTitle = strTaskName; // Task name would be contentname + content id + _Task
                                m_refTask.AssignToUserGroupID = PreapprovalGroupID; //Assigned to group defined by gtTaskAssignGroup
                                m_refTask.AssignedByUserID = CurrentUserId.ToString(); //Assigned by person creating the task
                                m_refTask.State = "1"; //Not started
                                m_refTask.ContentID = m_intId; //Content m_intId of the content being created
                                m_refTask.Priority = EkEnumeration.TaskPriority.Normal; //Normal
                                m_refTask.CreatedByUserID = CurrentUserId; //If task hops this will always be created user
                                m_refTask.AddTask();
                                m_refContent.SetContentState(m_intId, "T");
                            }
                            else
                            {
                                m_refContent.SubmitForPublicationv2_0(m_intId, Convert.ToInt64(Request.QueryString["fldid"].ToString()), "");
                            }
                        }
                        else
                        {
                            m_refContent.SubmitForPublicationv2_0(m_intId, Convert.ToInt64(Request.QueryString["fldid"].ToString()), "");
                        }
                        break;
                }
            }
            strCallBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "&fldid=" + Request.QueryString["fldid"]);
            Response.Redirect(strCallBackPage, false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - DoDeleteFolder
    private void Process_DoDeleteFolder()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        int m_intContentLanguage;
        long CurrentUserId;
        string FolderPath = "";
        Collection pagedata = new Collection();
        long parentid = 0;

        m_intContentLanguage = m_refContentApi.ContentLanguage;
        m_refContent = m_refContentApi.EkContentRef;
        CurrentUserId = m_refContentApi.UserId;

        if (string.IsNullOrEmpty(Request.QueryString["ParentID"]))
        {
            long.TryParse(Request.QueryString["id"], out parentid);
            if (parentid > 0)
            {
                parentid = m_refContent.GetParentIdByFolderId(parentid);
            }
        }
        else
        {
            long.TryParse(Request.QueryString["ParentID"], out parentid);
        }

        pagedata.Add(Request.QueryString["id"], "FolderID", null, null);
        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(Request.QueryString["id"].ToString()));
        if (FolderPath.Length > 0)
        {
            if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
            {
                FolderPath = FolderPath.Substring(FolderPath.Length - FolderPath.Length - 1, FolderPath.Length - 1);
            }
            // Strip off the current folder name from the path:
            int Pos;
            Pos = Strings.InStrRev(FolderPath, "\\", -1, 0);
            if (Pos > 0)
            {
                FolderPath = FolderPath.Substring(0, Pos - 1);
            }
            // Escape backslashes:
            FolderPath = FolderPath.Replace("\\", "\\\\");
        }

        CheckFolders(System.Convert.ToInt64(Request.QueryString["id"]));
        if (MasterLayouts != string.Empty)
        {
            Utilities.ShowError((string)(m_refMsg.GetMessage("com: cannot delete folder with master layout") + " " + MasterLayouts));
        }
        else
        {
            m_refContent.DeleteContentFolderv2_0(pagedata);
            if (m_refContent.RequestInformation.HttpsProtocol == "on" && m_refContent.RequestInformation.CommerceSettings.ComplianceMode == true && (HttpContext.Current != null) && (HttpContext.Current.Session != null) && HttpContext.Current.Session["ecmComplianceRequired"].ToString() == "true")
            {
                Response.Redirect((string)("content.aspx?LangType=" + m_intContentLanguage + "&action=ReestablishSession&id=" + parentid), false);
            }
            else
            {
                Response.Redirect((string)("content.aspx?LangType=" + m_intContentLanguage + "&action=ViewContentByCategory&id=" + parentid + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
            }
        }
    }
    private void Process_DoReestablishSession()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        int m_intContentLanguage;
        long CurrentUserId;
        string FolderPath = "";
        Collection pagedata = new Collection();
        long parentid = 0;

        m_intContentLanguage = m_refContentApi.ContentLanguage;
        m_refContent = m_refContentApi.EkContentRef;
        CurrentUserId = m_refContentApi.UserId;

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(Request.QueryString["id"].ToString()));
        if (FolderPath.Length > 0)
        {
            if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
            {
                FolderPath = FolderPath.Substring(FolderPath.Length - FolderPath.Length - 1, FolderPath.Length - 1);
            }
            FolderPath = FolderPath.Replace("\\", "\\\\");
        }
        m_intContentLanguage = m_refContentApi.ContentLanguage;
        Response.Redirect((string)("content.aspx?LangType=" + m_intContentLanguage + "&action=ViewContentByCategory&id=" + parentid + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
    }
    private void CheckFolders(long FolderID)
    {
        Ektron.Cms.ContentAPI contentApi = new Ektron.Cms.ContentAPI();
        CheckForMasterLayout(FolderID);
        Ektron.Cms.FolderData[] folderData = contentApi.GetChildFolders(FolderID, true, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Id);
        if ((folderData != null) && folderData.Length > 0)
        {
            int i = 0;
            for (i = 0; i <= folderData.Length - 1; i++)
            {
                CheckFolders(folderData[i].Id);
            }
        }
    }
    private void CheckForMasterLayout(long FolderID)
    {
        Ektron.Cms.ContentAPI contentApi = new Ektron.Cms.ContentAPI();
        Ektron.Cms.Content.EkContent m_refContent;
        Ektron.Cms.Common.EkContentCol ekContentColl;
        m_refContent = m_refContentApi.EkContentRef;
        Collection coll = new Collection();
        coll.Add(FolderID, "FolderID", null, null);
        coll.Add("-1", "ContentLanguage", null, null);
        coll.Add(Ektron.Cms.Common.EkEnumeration.ContentOrderBy.Id, "OrderBy", null, null);
        coll.Add(Ektron.Cms.Common.EkEnumeration.CMSContentType.Content, "ContentType", null, null);
        coll.Add(Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData, "ContentSubType", null, null);
        ekContentColl = null;
        int totalpages = 1;
        ekContentColl = m_refContent.GetAllViewableChildContentInfoV5_0(coll, 1, 10000, ref totalpages);
        if ((ekContentColl != null) && ekContentColl.Count > 0)
        {
            int j = 0;
            Ektron.Cms.FolderData masterLayoutFolder = m_refContent.GetFolderById(FolderID);
            for (j = 0; j <= ekContentColl.Count - 1; j++)
            {
                if (MasterLayouts == string.Empty)
                {
                    MasterLayouts = "Template Name: " + masterLayoutFolder.NameWithPath + ekContentColl.get_Item(j).Title;
                }
                else
                {
                    MasterLayouts += ", Template Name: " + masterLayoutFolder.NameWithPath + ekContentColl.get_Item(j).Title;
                }
            }
        }
    }
    #endregion
    #region ACTION - submitDelContAction
    private void Process_DeleteContent()
    {
        string strCallBackPage = "";
        long m_intId = -1;
        Ektron.Cms.Content.EkContent m_refContent;
        StyleHelper m_refStyle = new StyleHelper();

        m_intId = Convert.ToInt64(Request.QueryString["delete_id"].ToString());
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"].ToString());

            Ektron.Cms.Common.EkEnumeration.CMSContentSubtype subtype = m_refContent.GetContentSubType(m_intId);
            if (subtype == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
            {
                Ektron.Cms.PageBuilder.TemplateModel tm = new Ektron.Cms.PageBuilder.TemplateModel();
                TemplateData td = tm.FindByMasterLayoutID(m_intId);
                if ((td != null) && td.Id > 0)
                {
                    long[] folders = m_refContent.GetTemplateDefaultFolderUsage(td.Id);
                    Collection contentBlockInfo = m_refContent.GetTemplateContentBlockUsage(td.Id);
                    if (folders.Length > 0 || contentBlockInfo.Count > 0)
                    {
                        StringBuilder message = new StringBuilder();
                        message.Append("This master layout cannot be deleted until it is unassigned from the following folders: ");
                        if (folders.Length > 0)
                        {
                            for (int i = 0; i <= folders.Length - 1; i++)
                            {
                                message.Append(m_refContent.GetFolderById(folders[i]).NameWithPath + ", ");
                            }
                        }
                        message.Append("and the following layouts: ");
                        if (contentBlockInfo.Count > 0)
                        {

                            foreach (Collection col in contentBlockInfo)
                            {
                                ContentData content_data = m_refContentApi.EkContentRef.GetContentById(Convert.ToInt64(col["content_id"]), 0);
                                string folderpath = (string)(m_refContent.GetFolderById(content_data.FolderId).NameWithPath);
                                message.Append(folderpath + content_data.Title + ": (id=" + content_data.Id + ", lang=" + content_data.LanguageId + "), ");
                            }
                        }
                        throw (new Exception(message.ToString()));
                    }
                }
            }

            m_refContent.SubmitForDeletev2_0(m_intId, m_intFolderId);
            if (Request.QueryString["page"] == "webpage")
            {
                m_strReloadJS = "<script language=\"Javascript\">";
                m_strReloadJS += "top.opener.location.reload(true);";
                m_strReloadJS += "top.close();";
                m_strReloadJS += "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAction", m_strReloadJS);
            }
            else
            {
                strCallBackPage = m_refStyle.getCallBackupPage((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + m_intFolderId));
                Response.Redirect(strCallBackPage, false);
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    #endregion
    #region ACTION - DoDeletePermission
    private void Process_DoDeletePermission()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            Collection pagedata = new Collection();
            if (Request.QueryString["type"] == "folder")
            {
                pagedata.Add(Request.QueryString["id"], "FolderID", null, null);
                pagedata.Add("", "ContentID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["id"], "ContentID", null, null);
                pagedata.Add("", "FolderID", null, null);
            }
            if (Request.QueryString["base"] == "group")
            {
                pagedata.Add(Request.QueryString["PermID"], "UserGroupID", null, null);
                pagedata.Add("", "UserID", null, null);
                m_refContent.DeleteItemPermissionv2_0(pagedata);
            }
            else if (Request.QueryString["base"] == "user")
            {
                pagedata.Add(Request.QueryString["PermID"], "UserID", null, null);
                pagedata.Add("", "UserGroupID", null, null);
                m_refContent.DeleteItemPermissionv2_0(pagedata);
            }
            else
            {
                Process_DODeleteMultiplePermission();
            }
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"] + "&membership=" + Request.QueryString["membership"]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }

    private void Process_DODeleteMultiplePermission()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        string[] userID = null;
        string[] groupID = null;
        int _userIDs;
        int _grpIDs;
        int i = 0;
        int j = 0;

        m_refContent = m_refContentApi.EkContentRef;

        if (!string.IsNullOrEmpty(Request.QueryString["userIds"]))
        {
            userID = Request.QueryString["userIds"].Split(",".ToCharArray());
            _userIDs = userID.Length;
            for (i = 0; i <= _userIDs - 1; i++)
            {
                Collection userData = new Collection();
                if (Request.QueryString["type"] == "folder")
                {
                    userData.Add(Request.QueryString["id"], "FolderID", null, null);
                    userData.Add("", "ContentID", null, null);
                }
                else
                {
                    userData.Add(Request.QueryString["id"], "ContentID", null, null);
                    userData.Add("", "FolderID", null, null);
                }
                userData.Add(userID[i], "UserID", null, null);
                userData.Add("", "UserGroupID", null, null);
                m_refContent.DeleteItemPermissionv2_0(userData);
            }
        }
        if (!string.IsNullOrEmpty(Request.QueryString["groupIds"]))
        {
            groupID = Request.QueryString["groupIds"].Split(",".ToCharArray());
            _grpIDs = groupID.Length;
            for (j = 0; j <= _grpIDs - 1; j++)
            {
                Collection groupData = new Collection();
                if (Request.QueryString["type"] == "folder")
                {
                    groupData.Add(Request.QueryString["id"], "FolderID", null, null);
                    groupData.Add("", "ContentID", null, null);
                }
                else
                {
                    groupData.Add(Request.QueryString["id"], "ContentID", null, null);
                    groupData.Add("", "FolderID", null, null);
                }
                groupData.Add(groupID[j], "UserGroupID", null, null);
                groupData.Add("", "UserID", null, null);
                m_refContent.DeleteItemPermissionv2_0(groupData);
            }
        }

    }
    #endregion
    #region ACTION - DoItemInheritance (Disable/Enable)
    private void Process_DoItemInheritance(Flag value)
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            Collection pagedata = new Collection();
            pagedata.Add(Request.QueryString["id"], "ItemID", null, null);
            pagedata.Add(Request.QueryString["type"], "RequestType", null, null);
            if (value == Flag.Disable)
            {
                m_refContent.DisableItemInheritancev2_0(pagedata);
            }
            else
            {
                m_refContent.EnableItemInheritancev2_0(pagedata);
            }
            if (m_strMembership == "true")
            {
                Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"] + "&membership=" + m_strMembership), false);
            }
            else
            {
                Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    #endregion
    #region ACTION - DoItemPrivateSetting (Disable/Enable)
    private void Process_DoItemPrivateSetting(Flag value)
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            Collection pagedata = new Collection();
            pagedata.Add(Request.QueryString["id"], "ItemID", null, null);
            pagedata.Add(Request.QueryString["type"], "RequestType", null, null);
            if (value == Flag.Disable)
            {
                m_refContent.DisableItemPrivateSettingv2_0(pagedata);
            }
            else
            {
                m_refContent.EnableItemPrivateSettingv2_0(pagedata);
            }
            if (m_strMembership == "true")
            {
                Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"] + "&membership=" + m_strMembership), false);
            }
            else
            {
                Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    #endregion
    #region ACTION - DoAddItemApproval
    private void Process_DoAddItemApproval()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            Collection pagedata = new Collection();
            if (Request.QueryString["type"] == "folder")
            {
                pagedata.Add(Request.QueryString["id"], "FolderID", null, null);
                pagedata.Add("", "ContentID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["id"], "ContentID", null, null);
                pagedata.Add("", "FolderID", null, null);
            }
            if (Request.QueryString["base"] == "user")
            {
                pagedata.Add(Request.QueryString["item_id"], "UserID", null, null);
                pagedata.Add("", "UserGroupID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["item_id"], "UserGroupID", null, null);
                pagedata.Add("", "UserID", null, null);
            }
            m_refContent.AddItemApprovalv2_0(pagedata);
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    #endregion
    #region ACTION - DeleteItemApproval
    private void Process_DoDeleteItemApproval()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            Collection pagedata = new Collection();
            if (Request.QueryString["type"] == "folder")
            {
                pagedata.Add(Request.QueryString["id"], "FolderID", null, null);
                pagedata.Add("", "ContentID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["id"], "ContentID", null, null);
                pagedata.Add("", "FolderID", null, null);
            }
            if (Request.QueryString["base"] == "user")
            {
                pagedata.Add(Request.QueryString["item_id"], "UserID", null, null);
                pagedata.Add("", "UserGroupID", null, null);
            }
            else
            {
                pagedata.Add(Request.QueryString["item_id"], "UserGroupID", null, null);
                pagedata.Add("", "UserID", null, null);
            }
            m_refContent.DeleteItemApprovalv2_0(pagedata);
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - WorkOffLine
    private void Process_WorkOffLine()
    {
        Ektron.Cms.Content.EkContent m_refContent;
        long m_intId;
        bool ret;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            m_intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            if (Convert.ToString(m_intId) != "")
            {
                ret = m_refContent.CheckContentOutv2_0(m_intId);
            }
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + Request.QueryString["folder_id"]), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - ApproveContent
    private void Do_ApproveContent()
    {
        long m_intId;
        bool ret;
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_refContent = m_refContentApi.EkContentRef;
            m_intId = System.Convert.ToInt32(Request.QueryString["id"]);
            ret = m_refContent.Approvev2_0(m_intId);
            Response.Redirect((string)("content.aspx?action=viewcontentbycategory&id=" + Request.QueryString["fldid"]), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - DeclineContent
    private void Do_DeclineContent()
    {
        long m_intId;
        bool ret;
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            m_refContent = m_refContentApi.EkContentRef;
            string reason = "";
            if (!string.IsNullOrEmpty(Request.QueryString["comment"]))
            {
                reason = Request.QueryString["comment"];
            }
            ret = m_refContent.DeclineApproval2_0(m_intId, reason);
            Response.Redirect((string)("content.aspx?action=viewcontentbycategory&id=" + Request.QueryString["fldid"]), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - RestoreInheritance
    private void Do_RestoreInheritance()
    {
        long m_intId;
        bool ret;
        Ektron.Cms.Content.EkContent m_refContent;
        try
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            m_refContent = m_refContentApi.EkContentRef;
            ret = m_refContent.DeleteSubscriptionsForContentinFolder(m_intId);
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewFolder&id=" + m_intId), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region ACTION - UpdateTranslationStatus
    private void Do_UpdateTranslationStatus(string action)
    {
        try
        {
            long intId = Convert.ToInt64(Request.QueryString["id"].ToString());
            int intLangType = Convert.ToInt32(Request.QueryString["LangType"].ToString());
            var loc = new Ektron.Cms.Framework.Localization.LocalizationObject();
            string contType = Request.QueryString["ContType"].ToString();
            long intContType;
            Ektron.Cms.Localization.LocalizableCmsObjectType objType = Ektron.Cms.Localization.LocalizableCmsObjectType.Content;
            if (Int64.TryParse(contType, out intContType))
            {
                if (EkConstants.IsAssetContentType(intContType, false))
                {
                    objType = Ektron.Cms.Localization.LocalizableCmsObjectType.DmsAsset;
                }
                else if (intContType == EkEnumeration.CMSContentType.CatalogEntry.GetHashCode())
                {
                    objType = Ektron.Cms.Localization.LocalizableCmsObjectType.Product;
                }
            }
            switch (action)
            {
                case "translatereadystatus":
                    loc.MarkReadyForTranslation(objType, intId, intLangType);
                    break;
                case "translatenotreadystatus":
                    loc.MarkNotReadyForTranslation(objType, intId, intLangType);
                    break;
                case "translatenotallowstatus":
                    loc.MarkDoNotTranslate(objType, intId, intLangType);
                    break;
                case "translateneedstranslationstatus":
                    loc.MarkNeedsTranslation(objType, intId, intLangType);
                    break;
                case "translateoutfortranslationstatus":
                    loc.MarkOutForTranslation(objType, intId, intLangType);
                    break;
                case "translatetranslatedstatus":
                    loc.MarkTranslated(objType, intId, intLangType);
                    break;
            }
            Response.Redirect((string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + intId), false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #endregion

    #region JS, CSS
    public void RegisterJs()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronSiteData);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronContextMenuJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDetermineOfficeJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);

        //DNR doesn't seem to be compatible with jquery 1.5.  This should be phased out anyway; as in removed from
        //all Ektron code.  Ultimately, jsModal needs to be replaced with the EktronUI:Dialog server control.
        //Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/jfunct.js", "EktronJfuncJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/toolbar_roll.js", "EktronToolbarRollJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/platforminfo.js", "EktronPlatformInfoJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/designformentry.js", "EktronDesignFormEntryJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/sync/js/Ektron.Workarea.Sync.Relationships.js", "EktronSyncRelationshipsJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronWamenuJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/ektron.workarea.contextmenus.js", "EktronWorkareaContextMenusJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/Ektron.WorkArea.Content.MoveCopy.js", "EktronWorkareaContentMovieCopyJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/ektron.workarea.contextmenus.cutcopy.js", "EktronWorkareaContextMenusCutCopyJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/controls/permission/permissionsCheckHandler.ashx?action=getPermissionJsClass", "EktronPermissionJS");
        System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "dmsMenuPoundName", (string)(("$ektron().one(\"EktronDMSMenuReady\", function(){Ektron.DMSMenu.andSymbolFilename = \'") + m_refMsg.GetMessage("dmsmenu and symbol in filename warning") + "\';});"), true);
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/ektron.workarea.contextmenus.trees.js", "EktronWorkareaContextmenusTreeJS");

    }
    public void RegisterCss()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE6);
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/sync/sync.css", "EktronSyncCss");
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/sync/sync-IE.css", "EktronSyncIECss", Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/sync/css/ektron.workarea.sync.dialogs.css", "EktronSyncDialogsCss");
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/box.css", "EktronBoxCss");
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/tables/tableutil.css", "EktronTableUtilCss");
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss");
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronWamenuCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronContextMenuCss);
        SetJSVariable();
    }
    protected void AssignTextValues()
    {
        // assign the various resource text strings
        folder_jslanguage.Text = ContentLanguage.ToString();

        jsAppPath.Text = this.ApplicationPath.ToString();
        contentContextCutContent.Text = m_refMsg.GetMessage("lbl cut");
        contentContextCopyContent.Text = m_refMsg.GetMessage("lbl copy");
        contentContextAssignContentToTaxonomy.Text = m_refMsg.GetMessage("lbl assign items to taxonomy");
        contentContextAssignContentToCollection.Text = m_refMsg.GetMessage("add collection items title");
        contentContextAssignContentToMenu.Text = m_refMsg.GetMessage("add items to menu");
        contentContextDeleteContent.Text = m_refMsg.GetMessage("btn delete");
        jsIsFolderAdmin.Text = IsFolderAdmin().ToString();
        jsConfirmDelete.Text = m_refMsg.GetMessage("js:delete selected content block");
        jsConfirmChangeInheritance.Text = m_refMsg.GetMessage("js:confirm change inheritance");
    }
    private bool IsFolderAdmin()
    {
        bool _isFolderAdmin = false;
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            m_intFolderId = Convert.ToInt64(Request.QueryString["id"].ToString());
            _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers, m_intFolderId, m_refContentApi.UserId, false);
        }
        return _isFolderAdmin;
    }
    protected void SetJSVariable()
    {
        jsAlertCheckedOutSelected.Text = m_refMsg.GetMessage("js:alert checked out item selected");
        jsAlertCheckedOutSelectedCopy.Text = m_refMsg.GetMessage("js:alert checked out item selected copy");
        jsAlertSelectOneContent.Text = m_refMsg.GetMessage("js:alert select one item");
        jsAlertSelectOneContentCopy.Text = m_refMsg.GetMessage("js:alert select one item copy"); 
        jsAlertSelectNotApprovedAll.Text = m_refMsg.GetMessage("js: alert no approved item selected");
        jsConfirmMultiLingual.Text = m_refMsg.GetMessage("js: confirm multilingual");
        jsConfirmMultiLingual.ToolTip = jsConfirmMultiLingual.Text;
        selMultLangOption.Text = m_refMsg.GetMessage("select multiple language");
        lblConfirmMoveContent.Text = m_refMsg.GetMessage("js:confirm move content");
        lblConfirmMoveContent.ToolTip = lblConfirmMoveContent.Text;
        availableLanguages.Text = m_refMsg.GetMessage("generic all");
        selectedLanguages.Text = m_refMsg.GetMessage("generic selected");
        cancelButton.Text = m_refMsg.GetMessage("generic cancel");
        moveCancelButton.Text = cancelButton.Text;
        moveContentConfirmTitle.Text = m_refMsg.GetMessage("move content confirm title");
        ltrOK.Text = m_refMsg.GetMessage("continue");
        jsAppPathMultiLang.Text = this.ApplicationPath;
        ltrPaste.Text = m_refMsg.GetMessage("generic paste");
	    ltr_DelAlert_NoSelect.Text = m_refMsg.GetMessage("js:content alert del no select");
        ltr_DelAlert.Text = m_refMsg.GetMessage("js:content alert del");
        lit_restorelInheritance.Text = m_refMsg.GetMessage("js:content restore inheritance");
        ltr_SmartFormRemlbl.Text=ltr_TemplateRemLbl.Text = ltr_RemovaItemText.Text = m_refMsg.GetMessage("generic remove");
        ltrRemoveSubjectItemMsg.Text = m_refMsg.GetMessage("js:remove subject item");
        ltrTaxConfirmBrkInherit.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        ltr_addFolderBreadCrumb_breakInheri.Text=lit_template_inherit.Text = m_refMsg.GetMessage("js: confirm break inheritance");
        ltr_RemovaLastItemText.Text = m_refMsg.GetMessage("js:remove subject last item");
        ltrBlogRollLinkRemove.Text = m_refMsg.GetMessage("lbl remove blog roll link");
        ltrBlogRollEditTitle.Text = m_refMsg.GetMessage("generic edit title");
        lbl_BR_LinkName.Text = m_refMsg.GetMessage("lbl roll link name");
        lbl_BR_URL.Text = m_refMsg.GetMessage("lbl url");
        lbl_BR_ShortDesc.Text = m_refMsg.GetMessage("lbl roll short desc");
        lbl_BR_Relationship.Text = m_refMsg.GetMessage("lbl roll relationship");
        lbl_roll_RemoveLastMsg.Text = m_refMsg.GetMessage("js:remove blog roll lastroll link");
        lbl_roll_RemoveThisMsg.Text = m_refMsg.GetMessage("js:remove blog roll thisroll link");
        lbl_removeTmpWarn.Text = m_refMsg.GetMessage("js:add folder template inheri warn");
        lbl_removeSmartFromWarn.Text = m_refMsg.GetMessage("js:add folder smartform inheri warn");
        lbl_removeSmartFromAction.Text = m_refMsg.GetMessage("js:action confirm");
        lbl_removeTmpAction.Text = m_refMsg.GetMessage("js:action confirm");

        ltr_SmartFormViewlbl.Text=ltr_TemplateViewLbl.Text = m_refMsg.GetMessage("generic view");

        litInValidSmartFrom1.Text=litInValidSmartFrom.Text = m_refMsg.GetMessage("js:invlalid smartform");

        ltr_BreadcrumbNotitleMsg.Text = m_refMsg.GetMessage("js:menu crate no title msg");
        SiteAliasAddBtnText.Text =BreadcrumbAddBtnText.Text = m_refMsg.GetMessage("generic add title");
        BreadcrumbSaveBtnText.Text = m_refMsg.GetMessage("btn save");
        BreadcrumbRemoveTooltip.Text = m_refMsg.GetMessage("lbl delete path");

        SiteAliasNoNameMsg.Text = m_refMsg.GetMessage("js:site alias noname msg");
        SiteAliasDelIconTooltip.Text = m_refMsg.GetMessage("js:site alias del icon");
        
    }
    #endregion
}

