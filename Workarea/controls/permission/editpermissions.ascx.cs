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

public partial class editpermissions : System.Web.UI.UserControl
{
    #region Member Variables

    protected ContentAPI _ContentApi = new ContentAPI();
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected EkMessageHelper _MessageHelper;
    protected long _Id = 0;
    protected FolderData _FolderData;
    protected PermissionData _PermissionData;
    protected string _AppImgPath = "";
    protected int _ContentType = 1;
    protected long _CurrentUserId = 0;
    protected Collection _PageData;
    protected string _PageAction = "";
    protected string _OrderBy = "";
    protected int _ContentLanguage = -1;
    protected int _EnableMultilingual = 0;
    protected string _SitePath = "";
    protected string _ItemType = "";
    protected ContentData _ContentData;
    protected bool _IsMembership = false;
    protected string _Base = "";
    protected EkContent _EkContent;
    protected bool _EnablePreaproval = false;
    private bool _IsBoard = false;
    private bool _IsBlog = false;
    protected bool traverseFolder = false;
    #endregion

    #region Events

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
        RegisterResources();
        if (frm_transverse_folder.Value == "1")
            traverseFolder = true;
        else
            traverseFolder = false;
        if (!(Request.QueryString["type"] == null))
        {
            _ItemType = Convert.ToString(Request.QueryString["type"]).Trim().ToLower();
        }
        if (!(Request.QueryString["id"] == null))
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (!(Request.QueryString["action"] == null))
        {
            _PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (!(Request.QueryString["orderby"] == null))
        {
            _OrderBy = Convert.ToString(Request.QueryString["orderby"]);
        }
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                _ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
            }
            else
            {
                if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }
        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            _ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            _ContentApi.ContentLanguage = _ContentLanguage;
        }

        _IsMembership = false;
        bool.TryParse(Request.QueryString["membership"], out _IsMembership);

        if (!(Request.QueryString["base"] == null))
        {
            _Base = Request.QueryString["base"].Trim().ToLower();
        }
        _CurrentUserId = _ContentApi.UserId;
        _AppImgPath = _ContentApi.AppImgPath;
        _SitePath = _ContentApi.SitePath;
        _EnableMultilingual = _ContentApi.EnableMultilingual;

        SiteAPI m_refSiteApi = new SiteAPI();
        SettingsData setting_data = new SettingsData();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.EnablePreApproval)
        {
            _EnablePreaproval = true;
        }
        if (ddlUserType.Items.Count == 0)
        {
            AddUserTypes();
        }
    }

    #endregion

    #region Helpers

    private void AddUserTypes()
    {
        ListItem item;
        item = new ListItem(_MessageHelper.GetMessage("lbl view cms users"), "standard");
        ddlUserType.Items.Add(item);
        item = new ListItem(_MessageHelper.GetMessage("lbl view memberShip users"), "membership");
        ddlUserType.Items.Add(item);
    }

    public bool EditPermission()
    {
        if (!(Page.IsPostBack))
        {
            Display_EditPermissions();
        }
        else
        {
            Process_DoEditPermission();
        }
        return true;
    }

    public bool AddPermission()
    {
        if (!(Page.IsPostBack))
        {
            Display_AddPermissions();
        }
        else
        {
            Process_DoAddFolderPermission();
        }
        return true;
    }

    public string GetDisplay()
    {
        return (_IsMembership ? "style=\"display:none !important;\"" : string.Empty);
    }

    #endregion

    #region ACTION - DoAddFolderPermission

    private void Process_DoAddFolderPermission()
    {
        string[] userID = null;
        string[] groupID = null;
        int _userIDs = 0;
        int _grpIDs = 0;
        string @base = Request.Form[frm_base.UniqueID];
        string strUserID = "";
        string strGroupID = "";
        frm_itemid.Value = _Id.ToString();
        string finalUserGroupID = "";

        if (!string.IsNullOrEmpty(Request.QueryString["userIds"]))
        {
            userID = Request.QueryString["userIds"].Split(',');
            _userIDs = userID.Length;
        }
        if (!string.IsNullOrEmpty(Request.QueryString["groupIds"]))
        {
            groupID = Request.QueryString["groupIds"].Split(',');
            _grpIDs = groupID.Length;
        }
        if (_userIDs + _grpIDs == 1)
        {
            if (_userIDs == 1)
            {
                @base = "user";
                strUserID = Request.QueryString["userIds"];
            }
            else
            {
                @base = "group";
                strGroupID = Request.QueryString["groupIds"];
            }
        }
        string[] readOnlyPermission = Request.Form[frm_readonly.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] editPermission = Request.Form[frm_edit.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] addPermission = Request.Form[frm_add.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] deletePermission = Request.Form[frm_delete.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] readOnlyLibPermission = Request.Form[frm_libreadonly.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] addToImageLibPermission = Request.Form[frm_addimages.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] addToFileLibPermission = Request.Form[frm_addfiles.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] restorePermission = null;
        if (!(Request.Form[frm_restore.UniqueID] == null) && Request.Form[frm_restore.UniqueID] != "")
        {
            restorePermission = Request.Form[frm_restore.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }
        string[] addToHyperlinkLibPermission = null;
        if (!(Request.Form[frm_addhyperlinks.UniqueID] == null) && Request.Form[frm_addhyperlinks.UniqueID] != "")
        {
            addToHyperlinkLibPermission = Request.Form[frm_addhyperlinks.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }
        string[] overwriteLibPermission = null;
        if (!(Request.Form[frm_overwritelib.UniqueID] == null) && Request.Form[frm_overwritelib.UniqueID] != "")
        {
            overwriteLibPermission = Request.Form[frm_overwritelib.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }
        string[] addFoldersPermission = Request.Form[frm_add_folders.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] editFoldersPermission = Request.Form[frm_edit_folders.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] deleteFoldersPermission = Request.Form[frm_delete_folders.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        string[] transverseFoldersPermission = null;
        if (!(Request.Form[frm_transverse_folder.UniqueID] == null) && Request.Form[frm_transverse_folder.UniqueID] != "")
        {
            transverseFoldersPermission = Request.Form[frm_transverse_folder.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }
        string[] collectionsPermission = null;
        if (!(Request.Form[frm_navigation.UniqueID] == null) && Request.Form[frm_navigation.UniqueID] != "")
        {
            collectionsPermission = Request.Form[frm_navigation.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }

        string[] editApprovalssPermission = null;
        if (!(Request.Form[frm_edit_preapproval.UniqueID] == null))
        {
            editApprovalssPermission = Request.Form[frm_edit_preapproval.UniqueID].Remove(0, 1).Split(",".ToCharArray());
        }

        switch (@base)
        {
            case "":
                int userCount = 0;

                string[] userGroupId = new string[_userIDs + (_grpIDs - 1) + 1];
                if (_grpIDs > 0 && _userIDs > 0)
                {
                    groupID.CopyTo(userGroupId, 0);
                    userID.CopyTo(userGroupId, groupID.Length);
                }
                else if (_grpIDs > 0)
                {
                    groupID.CopyTo(userGroupId, 0);
                }
                else
                {
                    userID.CopyTo(userGroupId, 0);
                }
                for (userCount = 0; userCount <= userGroupId.Length - 1; userCount++)
                {
                    _EkContent = _ContentApi.EkContentRef;
                    _PageData = new Collection();
                    if (Request.Form[hmembershiptype.UniqueID] == "1")
                    {
                        if ((readOnlyPermission[userCount] == "1") || readOnlyPermission[userCount].ToString().ToLower() == "on")
                        {
                            _PageData.Add(1, "ReadOnly", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnly", null, null);
                        }
                        if ((editPermission[userCount] == "1") || (editPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Edit", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Edit", null, null);
                        }
                        if ((addPermission[userCount] == "1") || (addPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Add", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Add", null, null);
                        }
                        if ((deletePermission[userCount] == "1") || (deletePermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Delete", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Delete", null, null);
                        }
                        _PageData.Add(0, "Restore", null, null);
                        if ((readOnlyLibPermission[userCount] == "1") || (readOnlyLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "ReadOnlyLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnlyLib", null, null);
                        }
                        if ((addToImageLibPermission[userCount] == "1") || (addToImageLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "AddToImageLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToImageLib", null, null);
                        }
                        if ((addToFileLibPermission[userCount] == "1") || (addToFileLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "AddToFileLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToFileLib", null, null);
                        }
                        _PageData.Add(0, "AddToHyperlinkLib", null, null);
                        _PageData.Add(0, "OverwriteLib", null, null);
                        _PageData.Add(0, "AddFolders", null, null);
                        _PageData.Add(0, "EditFolders", null, null);
                        _PageData.Add(0, "DeleteFolders", null, null);
                        _PageData.Add(0, "Collections", null, null);
                        _PageData.Add(0, "TransverseFolder", null, null);
                        _PageData.Add(0, "EditApprovals", null, null);
                    }
                    else
                    {
                        if ((readOnlyPermission[userCount] == "1") || (readOnlyPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "ReadOnly", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnly", null, null);
                        }
                        if ((editPermission[userCount] == "1") || (editPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Edit", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Edit", null, null);
                        }
                        if ((addPermission[userCount] == "1") || (addPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Add", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Add", null, null);
                        }
                        if ((deletePermission[userCount] == "1") || (deletePermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "Delete", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Delete", null, null);
                        }
                        if (restorePermission != null && ((restorePermission[userCount] == "1") || (restorePermission[userCount].ToString().ToLower() == "on")))
                        {
                            _PageData.Add(1, "Restore", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Restore", null, null);
                        }
                        if ((readOnlyLibPermission[userCount] == "1") || (readOnlyLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "ReadOnlyLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnlyLib", null, null);
                        }
                        if ((addToImageLibPermission[userCount] == "1") || (addToImageLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "AddToImageLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToImageLib", null, null);
                        }
                        if ((addToFileLibPermission[userCount] == "1") || (addToFileLibPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "AddToFileLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToFileLib", null, null);
                        }
                        if (addToHyperlinkLibPermission != null && ((addToHyperlinkLibPermission[userCount] == "1") || (addToHyperlinkLibPermission[userCount].ToString().ToLower() == "on")))
                        {
                            _PageData.Add(1, "AddToHyperlinkLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToHyperlinkLib", null, null);
                        }
                        if (overwriteLibPermission != null && ((overwriteLibPermission[userCount] == "1") || (overwriteLibPermission[userCount].ToString().ToLower() == "on")))
                        {
                            _PageData.Add(1, "OverwriteLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "OverwriteLib", null, null);
                        }

                        if ((addFoldersPermission[userCount] == "1") || (addFoldersPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "AddFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddFolders", null, null);
                        }
                        if ((editFoldersPermission[userCount] == "1") || (editFoldersPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "EditFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "EditFolders", null, null);
                        }
                        if ((deleteFoldersPermission[userCount] == "1") || (deleteFoldersPermission[userCount].ToString().ToLower() == "on"))
                        {
                            _PageData.Add(1, "DeleteFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "DeleteFolders", null, null);
                        }
                        if (transverseFoldersPermission != null && ((transverseFoldersPermission[userCount] == "1") || (transverseFoldersPermission[userCount].ToString().ToLower() == "on")))
                        {
                            _PageData.Add(1, "TransverseFolder", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "TransverseFolder", null, null);
                        }
                        if (collectionsPermission != null && ((collectionsPermission[userCount] == "1") || (collectionsPermission[userCount].ToString().ToLower() == "on")))
                        {
                            _PageData.Add(1, "Collections", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Collections", null, null);
                        }

                        if (!(editApprovalssPermission == null))
                        {
                            if ((editApprovalssPermission[userCount] == "1") || (editApprovalssPermission[userCount].ToString().ToLower() == "on"))
                            {
                                _PageData.Add(1, "EditApprovals", null, null);
                            }
                            else
                            {
                                _PageData.Add(0, "EditApprovals", null, null);
                            }
                        }
                        else
                        {
                            _PageData.Add(0, "EditApprovals", null, null);
                        }
                    }

                    if (Request.Form[frm_type.UniqueID] == "folder")
                    {
                        _PageData.Add(Request.Form[frm_itemid.UniqueID], "FolderID", null, null);
                        _PageData.Add("", "ContentID", null, null);
                    }
                    else
                    {
                        _PageData.Add(Request.Form[frm_itemid.UniqueID], "ContentID", null, null);
                        _PageData.Add("", "FolderID", null, null);
                    }
                    if (userCount < _grpIDs)
                    {
                        _PageData.Add(userGroupId[userCount], "UserGroupID", null, null);
                        _PageData.Add("", "UserID", null, null);
                    }
                    else
                    {
                        _PageData.Add(userGroupId[userCount], "UserID", null, null);
                        _PageData.Add("", "UserGroupID", null, null);
                    }
                    bool m_bReturn;
                    m_bReturn = _EkContent.AddItemPermission(_PageData);
                }
                Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&id=" + Request.Form[frm_itemid.UniqueID] + "&type=" + Request.Form[frm_type.UniqueID] + "&membership=" + Request.Form[frm_membership.UniqueID]), false);
                break;

            default:
                try
                {
                    _EkContent = _ContentApi.EkContentRef;
                    _PageData = new Collection();
                    if (Request.Form[hmembershiptype.UniqueID] == "1")
                    {
                        if ((Request.Form["frm_readonly"] == "1") || Request.Form["frm_readonly"].ToString().ToLower() == "on")
                        {
                            _PageData.Add(1, "ReadOnly", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnly", null, null);
                        }
                        if (Request.Form[frm_edit.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Edit", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Edit", null, null);
                        }
                        if (Request.Form[frm_add.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Add", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Add", null, null);
                        }
                        if (Request.Form[frm_delete.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Delete", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Delete", null, null);
                        }
                        _PageData.Add(0, "Restore", null, null);
                        if (Request.Form[frm_libreadonly.UniqueID] == "1")
                        {
                            _PageData.Add(1, "ReadOnlyLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnlyLib", null, null);
                        }
                        if (Request.Form[frm_addimages.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddToImageLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToImageLib", null, null);
                        }
                        if (Request.Form[frm_addfiles.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddToFileLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToFileLib", null, null);
                        }
                        _PageData.Add(0, "AddToHyperlinkLib", null, null);
                        _PageData.Add(0, "OverwriteLib", null, null);
                        _PageData.Add(0, "AddFolders", null, null);
                        _PageData.Add(0, "EditFolders", null, null);
                        _PageData.Add(0, "DeleteFolders", null, null);
                        _PageData.Add(0, "Collections", null, null);
                        _PageData.Add(0, "TransverseFolder", null, null);
                        _PageData.Add(0, "EditApprovals", null, null);
                    }
                    else
                    {
                        if (Request.Form[frm_readonly.UniqueID] == "1")
                        {
                            _PageData.Add(1, "ReadOnly", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnly", null, null);
                        }
                        if (Request.Form[frm_edit.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Edit", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Edit", null, null);
                        }
                        if (Request.Form[frm_add.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Add", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Add", null, null);
                        }
                        if (Request.Form[frm_delete.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Delete", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Delete", null, null);
                        }
                        if (Request.Form[frm_restore.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Restore", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Restore", null, null);
                        }
                        if (Request.Form[frm_libreadonly.UniqueID] == "1")
                        {
                            _PageData.Add(1, "ReadOnlyLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "ReadOnlyLib", null, null);
                        }
                        if (Request.Form[frm_addimages.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddToImageLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToImageLib", null, null);
                        }
                        if (Request.Form[frm_addfiles.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddToFileLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToFileLib", null, null);
                        }
                        if (Request.Form[frm_addhyperlinks.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddToHyperlinkLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddToHyperlinkLib", null, null);
                        }
                        if (Request.Form[frm_overwritelib.UniqueID] == "1")
                        {
                            _PageData.Add(1, "OverwriteLib", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "OverwriteLib", null, null);
                        }

                        if (Request.Form[frm_add_folders.UniqueID] == "1")
                        {
                            _PageData.Add(1, "AddFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "AddFolders", null, null);
                        }
                        if (Request.Form[frm_edit_folders.UniqueID] == "1")
                        {
                            _PageData.Add(1, "EditFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "EditFolders", null, null);
                        }
                        if (Request.Form[frm_delete_folders.UniqueID] == "1")
                        {
                            _PageData.Add(1, "DeleteFolders", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "DeleteFolders", null, null);
                        }
                        if (Request.Form[frm_transverse_folder.UniqueID] == "1")
                        {
                            _PageData.Add(1, "TransverseFolder", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "TransverseFolder", null, null);
                        }
                        if (Request.Form[frm_navigation.UniqueID] == "1")
                        {
                            _PageData.Add(1, "Collections", null, null);
                        }
                        else
                        {
                            _PageData.Add(0, "Collections", null, null);
                        }

                        if (!(Request.Form[frm_edit_preapproval.UniqueID] == null))
                        {
                            if (Request.Form[frm_edit_preapproval.UniqueID] == "1")
                            {
                                _PageData.Add(1, "EditApprovals", null, null);
                            }
                            else
                            {
                                _PageData.Add(0, "EditApprovals", null, null);
                            }
                        }
                        else
                        {
                            _PageData.Add(0, "EditApprovals", null, null);
                        }
                    }

                    if (Request.Form[frm_type.UniqueID] == "folder")
                    {
                        _PageData.Add(Request.Form[frm_itemid.UniqueID], "FolderID", null, null);
                        _PageData.Add("", "ContentID", null, null);
                    }
                    else
                    {
                        _PageData.Add(Request.Form[frm_itemid.UniqueID], "ContentID", null, null);
                        _PageData.Add("", "FolderID", null, null);
                    }
                    if (Request.Form[frm_base.UniqueID] == "")
                    {
                        if (Request.QueryString["groupIds"] == "")
                        {
                            finalUserGroupID = strUserID;
                        }
                        else
                        {
                            finalUserGroupID = strGroupID;
                        }
                    }
                    else
                    {
                        finalUserGroupID = Request.Form[frm_permid.UniqueID];
                    }

                    if (Request.Form[frm_base.UniqueID] == "group")
                    {
                        _PageData.Add(finalUserGroupID, "UserGroupID", null, null);
                        _PageData.Add("", "UserID", null, null);
                    }
                    else if (Request.Form[frm_base.UniqueID] == "user")
                    {
                        _PageData.Add(finalUserGroupID, "UserID", null, null);
                        _PageData.Add("", "UserGroupID", null, null);
                    }
                    else if (Request.QueryString["groupIds"] != "")
                    {
                        _PageData.Add(finalUserGroupID, "UserGroupID", null, null);
                        _PageData.Add("", "UserID", null, null);
                    }
                    else if (Request.QueryString["userIds"] != "")
                    {
                        _PageData.Add(finalUserGroupID, "UserID", null, null);
                        _PageData.Add("", "UserGroupID", null, null);
                    }

                    bool m_bReturn;
                    m_bReturn = _EkContent.AddItemPermission(_PageData);
                    Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&id=" + Request.Form[frm_itemid.UniqueID] + "&type=" + Request.Form[frm_type.UniqueID] + "&membership=" + Request.Form[frm_membership.UniqueID]), false);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
                }
                break;
        }
    }

    #endregion

    #region ACTION - DoEditPermission

    private void Process_DoEditPermission()
    {
        try
        {
            _EkContent = _ContentApi.EkContentRef;
            _PageData = new Collection();
            if (Request.Form[hmembershiptype.UniqueID] == "1")
            {
                if (Request.Form[frm_readonly.UniqueID] == "1")
                {
                    _PageData.Add(1, "ReadOnly", null, null);
                }
                else
                {
                    _PageData.Add(0, "ReadOnly", null, null);
                }
                if (Request.Form[frm_add.UniqueID] == "1")
                {
                    _PageData.Add(1, "Add", null, null);
                }
                else
                {
                    _PageData.Add(0, "Add", null, null);
                }
                if (Request.Form[frm_edit.UniqueID] == "1")
                {
                    _PageData.Add(1, "Edit", null, null);
                }
                else
                {
                    _PageData.Add(0, "Edit", null, null);
                }
                if (Request.Form[frm_delete.UniqueID] == "1")
                {
                    _PageData.Add(1, "Delete", null, null);
                }
                else
                {
                    _PageData.Add(0, "Delete", null, null);
                }
                _PageData.Add(0, "Restore", null, null);
                if (Request.Form[frm_addimages.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddToImageLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddToImageLib", null, null);
                }
                if (Request.Form[frm_addfiles.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddToFileLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddToFileLib", null, null);
                }
                _PageData.Add(0, "AddToHyperlinkLib", null, null);
                _PageData.Add(0, "OverwriteLib", null, null);
                _PageData.Add(0, "AddFolders", null, null);
                _PageData.Add(0, "EditFolders", null, null);
                _PageData.Add(0, "DeleteFolders", null, null);
                _PageData.Add(0, "Collections", null, null);
                _PageData.Add(0, "TransverseFolder", null, null);
                _PageData.Add(0, "EditApprovals", null, null);
                if (Request.Form[frm_libreadonly.UniqueID] == "1")
                {
                    _PageData.Add(1, "ReadOnlyLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "ReadOnlyLib", null, null);
                }
            }
            else
            {
                if (Request.Form[frm_readonly.UniqueID] == "1")
                {
                    _PageData.Add(1, "ReadOnly", null, null);
                }
                else
                {
                    _PageData.Add(0, "ReadOnly", null, null);
                }
                if (Request.Form[frm_edit.UniqueID] == "1")
                {
                    _PageData.Add(1, "Edit", null, null);
                }
                else
                {
                    _PageData.Add(0, "Edit", null, null);
                }
                if (Request.Form[frm_add.UniqueID] == "1")
                {
                    _PageData.Add(1, "Add", null, null);
                }
                else
                {
                    _PageData.Add(0, "Add", null, null);
                }
                if (Request.Form[frm_delete.UniqueID] == "1")
                {
                    _PageData.Add(1, "Delete", null, null);
                }
                else
                {
                    _PageData.Add(0, "Delete", null, null);
                }
                if (Request.Form[frm_restore.UniqueID] == "1")
                {
                    _PageData.Add(1, "Restore", null, null);
                }
                else
                {
                    _PageData.Add(0, "Restore", null, null);
                }
                if (Request.Form[frm_libreadonly.UniqueID] == "1")
                {
                    _PageData.Add(1, "ReadOnlyLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "ReadOnlyLib", null, null);
                }
                if (Request.Form[frm_addimages.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddToImageLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddToImageLib", null, null);
                }
                if (Request.Form[frm_addfiles.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddToFileLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddToFileLib", null, null);
                }
                if (Request.Form[frm_addhyperlinks.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddToHyperlinkLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddToHyperlinkLib", null, null);
                }
                if (Request.Form[frm_overwritelib.UniqueID] == "1")
                {
                    _PageData.Add(1, "OverwriteLib", null, null);
                }
                else
                {
                    _PageData.Add(0, "OverwriteLib", null, null);
                }
                if (Request.Form[frm_add_folders.UniqueID] == "1")
                {
                    _PageData.Add(1, "AddFolders", null, null);
                }
                else
                {
                    _PageData.Add(0, "AddFolders", null, null);
                }
                if (Request.Form[frm_edit_folders.UniqueID] == "1")
                {
                    _PageData.Add(1, "EditFolders", null, null);
                }
                else
                {
                    _PageData.Add(0, "EditFolders", null, null);
                }
                if (Request.Form[frm_delete_folders.UniqueID] == "1")
                {
                    _PageData.Add(1, "DeleteFolders", null, null);
                }
                else
                {
                    _PageData.Add(0, "DeleteFolders", null, null);
                }
                if (Request.Form[frm_transverse_folder.UniqueID] == "1")
                {
                    _PageData.Add(1, "TransverseFolder", null, null);
                }
                else
                {
                    _PageData.Add(0, "TransverseFolder", null, null);
                }
                if (Request.Form[frm_navigation.UniqueID] == "1")
                {
                    _PageData.Add(1, "Collections", null, null);
                }
                else
                {
                    _PageData.Add(0, "Collections", null, null);
                }
                if (!(Request.Form[frm_edit_preapproval.UniqueID] == null))
                {
                    if (Request.Form[frm_edit_preapproval.UniqueID] == "1")
                    {
                        _PageData.Add(1, "EditApprovals", null, null);
                    }
                    else
                    {
                        _PageData.Add(0, "EditApprovals", null, null);
                    }
                }
                else
                {
                    _PageData.Add(0, "EditApprovals", null, null);
                }
            }
            if (Request.Form[frm_type.UniqueID] == "folder")
            {
                _PageData.Add(Request.Form[frm_itemid.UniqueID], "FolderID", null, null);
                _PageData.Add("", "ContentID", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[frm_itemid.UniqueID], "ContentID", null, null);
                _PageData.Add("", "FolderID", null, null);
            }
            if (Request.Form[frm_base.UniqueID] == "group")
            {
                _PageData.Add(Request.Form[frm_permid.UniqueID], "UserGroupID", null, null);
                _PageData.Add("", "UserID", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[frm_permid.UniqueID], "UserID", null, null);
                _PageData.Add("", "UserGroupID", null, null);
            }
            _EkContent.UpdateItemPermissionv2_0(_PageData);
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&id=" + Request.Form[frm_itemid.UniqueID] + "&type=" + Request.Form[frm_type.UniqueID] + "&membership=" + Request.Form[frm_membership.UniqueID]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }

    #endregion

    #region PERMISSION - EditPermissions

    private void Display_EditPermissions()
    {
        long nFolderId;

        if (_ItemType == "folder")
        {
            _FolderData = _ContentApi.GetFolderById(_Id);
            nFolderId = _Id;
            if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard) || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum))
            {
                _IsBoard = true;
            }
            else if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Blog))
            {
                _IsBlog = true;
            }
        }
        else
        {
            _ContentData = _ContentApi.GetContentById(_Id, 0);
            _FolderData = _ContentApi.GetFolderById(_ContentData.FolderId);
            nFolderId = _ContentData.FolderId;
        }
        EditPermissionsToolBar();
        _PageData = new Collection();
        UserPermissionData[] userpermission_data;
        UserGroupData usergroup_data;
        UserData user_data;
        UserAPI m_refUserAPI = new UserAPI();
        if (Request.QueryString["base"] == "group")
        {
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, Request.QueryString["PermID"], ContentAPI.PermissionUserType.All, ContentAPI.PermissionRequestType.All); //cTmp = ContObj.GetOrderedItemPermissionsv2_0(cTmp, retString)
            usergroup_data = m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Convert.ToInt64(Request.QueryString["PermID"]));
            _IsMembership = usergroup_data.IsMemberShipGroup;
        }
        else
        {
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, Convert.ToInt64(Request.QueryString["PermID"]), "", ContentAPI.PermissionUserType.All, ContentAPI.PermissionRequestType.All);
            user_data = m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Convert.ToInt64(Request.QueryString["PermID"]), false, false);
            _IsMembership = user_data.IsMemberShip;

        }
        frm_itemid.Value = _Id.ToString();
        frm_type.Value = Request.QueryString["type"];
        frm_base.Value = _Base;
        frm_permid.Value = Request.QueryString["PermID"];
        frm_membership.Value = Request.QueryString["membership"];


        if (_IsMembership)
        {
            td_ep_membership.Visible = false;
            hmembershiptype.Value = "1";
        }
        else
        {
            td_ep_membership.InnerHtml = _StyleHelper.GetEnableAllPrompt();
            hmembershiptype.Value = "0";
        }
        Populate_EditPermissionsGenericGrid(userpermission_data);
        Populate_EditPermissionsAdvancedGrid(userpermission_data);
    }

    private void Populate_EditPermissionsGenericGrid(UserPermissionData[] userpermission_data)
    {
        string strMsg = "";
        if (_Base == "group")
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipGroup\">" + _MessageHelper.GetMessage("generic membership user group label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic cms group label") + "</span>";
            }
        }
        else
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipUser\">" + _MessageHelper.GetMessage("generic membership user label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
            }
        }
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderStyle.CssClass = "left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = strMsg;
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "READ";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDIT";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Edit title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADD";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETE";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Delete title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "RESTORE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }

        if (_IsBoard)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply");
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil");
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate");
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        else
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Library title") + " " + _MessageHelper.GetMessage("generic read only"));
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Images"));
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDHYP";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "OVERWRITELIB";
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
            PermissionsGenericGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("READ", typeof(string)));
        dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
        dt.Columns.Add(new DataColumn("ADD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
        dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));

        if (_IsBoard)
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
        }
        else
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
        }

        dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
        dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));

        if (!(userpermission_data == null))
        {
            dr = dt.NewRow();

            if (Request.QueryString["base"] == "group")
            {
                dr[0] = userpermission_data[0].DisplayGroupName;
            }
            else
            {
                dr[0] = userpermission_data[0].DisplayUserName;
            }

            dr[1] = "<input type=\"checkbox\" name=\"frm_readonly\" ";
            if (userpermission_data[0].IsReadOnly)
            {
                dr[1] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[1] += " onclick=\"return CheckReadOnlyForMembershipUser(\'frm_readonly\');\" />";
            }
            else
            {
                dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_readonly\');\" />";
            }


            dr[2] = "<input type=\"checkbox\" name=\"frm_edit\" ";
            if (userpermission_data[0].CanEdit)
            {
                dr[2] += " checked=\"checked\" ";
            }
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBlog))
            {
                dr[2] += " disabled=\"disabled\" ";
            }
            dr[2] += "  onclick=\"return CheckPermissionSettings(\'frm_edit\');\" />";


            dr[3] = "<input type=\"checkbox\" name=\"frm_add\" ";
            if (userpermission_data[0].CanAdd)
            {
                dr[3] += " checked=\"checked\" ";
            }
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBoard || _IsBlog))
            {
                dr[3] += " disabled=\"disabled\" ";
            }
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_add\');\" />";

            dr[4] = "<input type=\"checkbox\" name=\"frm_delete\" ";
            if (userpermission_data[0].CanDelete)
            {
                dr[4] += " checked=\"checked\" ";
            }
            if (_IsMembership && !(_IsBlog))
            {
                dr[4] += " disabled=\"disabled\" ";
            }
            dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete\');\" />";

            dr[5] = "<input type=\"checkbox\" name=\"frm_restore\"  ";
            if (userpermission_data[0].CanRestore)
            {
                dr[5] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[5] += " disabled=\"disabled\" ";
            }
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_restore\');\" />";

            dr[6] = "<input type=\"checkbox\" name=\"frm_libreadonly\" ";
            if (userpermission_data[0].IsReadOnlyLib)
            {
                dr[6] += " checked=\"checked\" ";
            }
            dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_libreadonly\');\" />";

            if (_IsBoard)
            {
                // add image/file
                dr[7] = "<input type=\"checkbox\" name=\"frm_addfiles\"  ";
                if (userpermission_data[0].CanAddToFileLib)
                {
                    dr[7] += " checked=\"checked\" ";
                }
                dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
                // moderate, 
                dr[8] = "<input type=\"checkbox\" name=\"frm_addimages\" ";
                if (userpermission_data[0].CanAddToImageLib)
                {
                    dr[8] += " checked=\"checked\" ";
                }
                dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_moderate\');\" />";
            }
            else
            {
                dr[7] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                if (userpermission_data[0].CanAddToImageLib)
                {
                    dr[7] += " checked=\"checked\" ";
                }
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[7] += " disabled=\"disabled\" ";
                }
                dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";


                dr[8] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                if (userpermission_data[0].CanAddToFileLib)
                {
                    dr[8] += " checked=\"checked\" ";
                }
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[8] += " disabled=\"disabled\" ";
                }
                dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
            }

            dr[9] = "<input type=\"checkbox\" name=\"frm_addhyperlinks\" ";
            if (userpermission_data[0].CanAddToHyperlinkLib)
            {
                dr[9] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[9] += " disabled=\"disabled\" ";
            }
            dr[9] += "onclick=\"return CheckPermissionSettings(\'frm_addhyperlinks\');\" />";

            dr[10] = "<input type=\"checkbox\" name=\"frm_overwritelib\" ";
            if (userpermission_data[0].CanOverwriteLib)
            {
                dr[10] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[10] += " disabled=\"disabled\" ";
            }
            dr[10] += "onclick=\"return CheckPermissionSettings(\'frm_overwritelib\');\" />";

            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsGenericGrid.DataSource = dv;
        PermissionsGenericGrid.DataBind();
    }

    private void Populate_EditPermissionsAdvancedGrid(UserPermissionData[] userpermission_data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        if (_Base == "group")
        {
            strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic User Group Name") + "</span>";
        }
        else
        {
            strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
        }

        colBound.DataField = "TITLE";
        colBound.HeaderText = strMsg;
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "COLLECTIONS";
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
            colBound.HeaderStyle.CssClass = "title-header";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADDFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Add Forum";
            colBound.ItemStyle.CssClass = "center";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
            colBound.ItemStyle.CssClass = "center";
        }
        colBound.HeaderStyle.CssClass = "title-header";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Edit Forum";
            colBound.ItemStyle.CssClass = "center";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
            colBound.ItemStyle.CssClass = "center";
        }
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETEFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Delete Forum";
            colBound.ItemStyle.CssClass = "center";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
            colBound.ItemStyle.CssClass = "center";
        }
        colBound.HeaderStyle.CssClass = "title-header";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TRAVERSE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderStyle.CssClass = "title-header";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        SiteAPI m_refSiteApi = new SiteAPI();
        SettingsData setting_data = new SettingsData();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.EnablePreApproval)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ModifyPreapproval";
            colBound.HeaderText = "Modify Preapproval";
            colBound.ItemStyle.CssClass = "center";
            colBound.HeaderStyle.CssClass = "title-header";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("COLLECTIONS", typeof(string)));
        dt.Columns.Add(new DataColumn("ADDFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETEFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("TRAVERSE", typeof(string)));
        if (setting_data.EnablePreApproval)
        {
            dt.Columns.Add(new DataColumn("ModifyPreapproval", typeof(string)));
        }

        bool bInherited = false;
        if (_ItemType == "folder")
        {
            bInherited = _FolderData.Inherited;
        }
        else
        {
            bInherited = _ContentData.IsInherited;
        }

        int i = 0;


        if (!(userpermission_data == null))
        {
            dr = dt.NewRow();


            if (Request.QueryString["base"] == "group")
            {
                dr[0] = userpermission_data[0].DisplayGroupName;
            }
            else
            {
                dr[0] = userpermission_data[0].DisplayUserName;
            }

            frm_navigation.Value = Convert.ToString(userpermission_data[0].IsCollections);
            dr[1] = "<input type=\"checkbox\" name=\"frm_navigation\" ";
            if (userpermission_data[0].IsCollections)
            {
                dr[1] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[1] += " disabled=\"disabled\" ";
            }
            dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_navigation\');\" />";

            frm_add_folders.Value = Convert.ToString(userpermission_data[i].CanAddFolders);
            dr[2] = "<input type=\"checkbox\" name=\"frm_add_folders\" ";
            if (userpermission_data[i].CanAddFolders)
            {
                dr[2] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[2] += " disabled=\"disabled\" ";
            }
            dr[2] += "onclick=\"return CheckPermissionSettings(\'frm_add_folders\');\" />";

            frm_edit_folders.Value = Convert.ToString(userpermission_data[i].CanEditFolders);
            dr[3] = "<input type=\"checkbox\" name=\"frm_edit_folders\" ";
            if (userpermission_data[i].CanEditFolders)
            {
                dr[3] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[3] += " disabled=\"disabled\" ";
            }
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_edit_folders\');\" />";

            frm_delete_folders.Value = Convert.ToString(userpermission_data[i].CanDeleteFolders);
            dr[4] = "<input type=\"checkbox\" name=\"frm_delete_folders\" ";
            if (userpermission_data[i].CanDeleteFolders)
            {
                dr[4] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[4] += " disabled=\"disabled\" ";
            }
            dr[4] += " onclick=\"return CheckPermissionSettings(\'frm_delete_folders\');\" />";

            frm_transverse_folder.Value = Convert.ToString(userpermission_data[i].CanTraverseFolders);
            dr[5] = "<input type=\"checkbox\" name=\"frm_transverse_folder\" ";
            if (userpermission_data[i].CanTraverseFolders)
            {
                dr[5] += " checked=\"checked\" ";
            }
            if (_IsMembership)
            {
                dr[5] += " disabled=\"disabled\" ";
            }
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_transverse_folder\');\" />";

            if (setting_data.EnablePreApproval)
            {
                frm_edit_preapproval.Value = Convert.ToString(userpermission_data[i].CanEditApprovals);
                dr[6] = "<input type=\"checkbox\" name=\"frm_edit_preapproval\" ";
                if (userpermission_data[i].CanEditApprovals)
                {
                    dr[6] += " checked=\"checked\" ";
                }
                if (_IsMembership)
                {
                    dr[6] += " disabled=\"disabled\" ";
                }
                dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_edit_preapproval\');\" />";
            }
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsAdvancedGrid.DataSource = dv;
        PermissionsAdvancedGrid.DataBind();
    }

    private void EditPermissionsToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string WorkareaTitlebarTitle = "";

        if (_ItemType == "folder")
        {
            WorkareaTitlebarTitle = (string)(_MessageHelper.GetMessage("edit folder permissions") + " \"" + _FolderData.Name + "\"");
        }
        else
        {
            WorkareaTitlebarTitle = (string)(_MessageHelper.GetMessage("edit content permissions") + " \"" + _ContentData.Title + "\"");
        }
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle);

        result.Append("<table><tbody><tr>");
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&membership=" + Request.QueryString["membership"]), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (permissions)"), _MessageHelper.GetMessage("btn update"), "onclick=\"javascript:return SubmitForm(\'frmContent\', \'CheckEditPermissions()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        if (_IsBoard == true)
        {
            result.Append(_StyleHelper.GetHelpButton("editboardperms", ""));
        }
        else
        {
            result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
        }
        result.Append("</td>");
        result.Append("</tr></tbody></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    #endregion

    #region PERMISSION - AddPermissions

    private void Display_AddPermissions()
    {
        UserGroupData usergroup_data;
        System.Collections.Generic.List<UserGroupData> userGroupDataList = new System.Collections.Generic.List<UserGroupData>();
        UserData user_data;
        System.Collections.Generic.List<UserData> userDataList = new System.Collections.Generic.List<UserData>();
        UserAPI m_refUserAPI = new UserAPI();
        long nFolderId;

        frm_itemid.Value = _Id.ToString();
        frm_type.Value = Request.QueryString["type"];
        frm_base.Value = Request.QueryString["base"];
        frm_permid.Value = Request.QueryString["PermID"];
        frm_membership.Value = Request.QueryString["membership"];

        if (_ItemType == "folder")
        {
            _FolderData = _ContentApi.GetFolderById(_Id);
            nFolderId = _Id;
            if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard) || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum))
            {
                _IsBoard = true;
            }
            else if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Blog))
            {
                _IsBlog = true;
            }
        }
        else
        {
            _ContentData = _ContentApi.GetContentById(_Id, 0);
            _FolderData = _ContentApi.GetFolderById(_ContentData.FolderId);
            nFolderId = _ContentData.FolderId;
        }
        AddPermissionsToolBar();
        if (Request.QueryString["base"] == "group")
        {
            usergroup_data = m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Convert.ToInt64(Request.QueryString["PermID"]));
            Populate_AddPermissionsGenericGrid(usergroup_data);
            Populate_AddPermissionsAdvancedGrid(usergroup_data);
            _IsMembership = usergroup_data.IsMemberShipGroup;
        }
        else if (Request.QueryString["base"] == "user")
        {
            user_data = m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Convert.ToInt64(Request.QueryString["PermID"]), false, false);
            Populate_AddPermissionsGenericGrid(user_data);
            Populate_AddPermissionsAdvancedGrid(user_data);
            _IsMembership = user_data.IsMemberShip;
        }
        else
        {
            string[] Groups = Request.QueryString["groupIDS"].Split(",".ToCharArray());
            string[] Users = Request.QueryString["userIDS"].Split(",".ToCharArray());
            int groupCount = 0;
            int userCount = 0;

            if (Request.QueryString["groupIDS"] != "")
            {
                for (groupCount = 0; groupCount <= Groups.Length - 1; groupCount++)
                {
                    userGroupDataList.Add(m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Convert.ToInt64(Groups[groupCount])));
                }
                _IsMembership = userGroupDataList[0].IsMemberShipGroup;
            }
            if (Request.QueryString["userIDS"] != "")
            {
                for (userCount = 0; userCount <= Users.Length - 1; userCount++)
                {
                    userDataList.Add(m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Convert.ToInt64(Users[userCount]), false, false));
                }
                _IsMembership = userDataList[0].IsMemberShip;
            }
            Populate_AddPermissionsGenericGridForUsersAndGroup(userGroupDataList, userDataList);
            Populate_AddPermissionsAdvancedGridForUsersAndGroup(userGroupDataList, userDataList);
        }


        if (_IsMembership)
        {
            td_ep_membership.Visible = false;
            hmembershiptype.Value = "1";
        }
        else
        {
            td_ep_membership.InnerHtml = _StyleHelper.GetEnableAllPrompt();
            hmembershiptype.Value = "0";
        }
    }
    private void Populate_AddPermissionsGenericGrid(UserData data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        if (_Base == "group")
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipGroup\">" + _MessageHelper.GetMessage("generic membership user group label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic cms group label") + "</span>";
            }

        }
        else
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipUser\">" + _MessageHelper.GetMessage("generic membership user label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
            }
        }

        colBound.DataField = "TITLE";
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = strMsg;
        PermissionsGenericGrid.Columns.Add(colBound);
        colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound.DataField = "READ";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDIT";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Edit title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADD";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETE";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Delete title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "RESTORE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        if (_IsBoard)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        else
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Library title") + " " + _MessageHelper.GetMessage("generic read only"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Images"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDHYP";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "OVERWRITELIB";
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("READ", typeof(string)));
        dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
        dt.Columns.Add(new DataColumn("ADD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
        dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));
        if (_IsBoard)
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
        }
        else
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
        dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));
        if (!(data == null))
        {
            dr = dt.NewRow();
            dr[0] = data.DisplayUserName;
            dr[1] = "<input type=\"checkbox\" name=\"frm_readonly\" ";
            if (_IsMembership)
            {
                dr[1] += " checked=\"checked\" onclick=\"return CheckReadOnlyForMembershipUser(\'frm_readonly\');\">";
            }
            else
            {
                dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_readonly\');\" />";
            }

            dr[2] = "<input type=\"checkbox\" name=\"frm_edit\" ";
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBlog))
            {
                dr[2] += " disabled=\"disabled\" ";
            }
            dr[2] += "  onclick=\"return CheckPermissionSettings(\'frm_edit\');\" />";


            dr[3] = "<input type=\"checkbox\" name=\"frm_add\" ";
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBoard || _IsBlog))
            {
                dr[3] += " disabled=\"disabled\" ";
            }
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_add\');\" />";

            dr[4] = "<input type=\"checkbox\" name=\"frm_delete\" ";
            if (_IsMembership && !(_IsBlog))
            {
                dr[4] += " disabled=\"disabled\" ";
            }
            dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete\');\" />";

            dr[5] = "<input type=\"checkbox\" name=\"frm_restore\"  ";
            if (_IsMembership)
            {
                dr[5] += " disabled=\"disabled\" ";
            }
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_restore\');\" />";

            dr[6] = "<input type=\"checkbox\" name=\"frm_libreadonly\" ";
            if ((_IsMembership) && !(_IsBoard || _IsBlog || _FolderData.IsCommunityFolder))
            {
                dr[6] += " checked=\"checked\" ";
            }
            dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_libreadonly\');\" />";

            if (_IsBoard == true)
            {
                dr[7] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                dr[7] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";

                dr[8] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                dr[8] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";
            }
            else
            {
                dr[7] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[7] += " disabled=\"disabled\" ";
                }
                dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";

                dr[8] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[8] += " disabled=\"disabled\" ";
                }
                dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
            }

            dr[9] = "<input type=\"checkbox\" name=\"frm_addhyperlinks\" ";
            if (_IsMembership)
            {
                dr[9] += " disabled=\"disabled\" ";
            }
            dr[9] += "onclick=\"return CheckPermissionSettings(\'frm_addhyperlinks\');\" />";

            dr[10] = "<input type=\"checkbox\" name=\"frm_overwritelib\" ";
            if (_IsMembership)
            {
                dr[10] += " disabled=\"disabled\" ";
            }
            dr[10] += "onclick=\"return CheckPermissionSettings(\'frm_overwritelib\');\" />";

            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsGenericGrid.DataSource = dv;
        PermissionsGenericGrid.DataBind();
    }
    private void Populate_AddPermissionsGenericGrid(UserGroupData data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        if (_Base == "group")
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipGroup\">" + _MessageHelper.GetMessage("generic membership user group label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic cms group label") + "</span>";
            }

        }
        else
        {
            if (_IsMembership)
            {
                strMsg = "<span class=\"membershipUser\">" + _MessageHelper.GetMessage("generic membership user label") + "</span>";
            }
            else
            {
                strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
            }
        }

        colBound.DataField = "TITLE";
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = strMsg;
        PermissionsGenericGrid.Columns.Add(colBound);
        colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound.DataField = "READ";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDIT";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Edit title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADD";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETE";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Delete title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "RESTORE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        if (_IsBoard)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        else
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Library title") + " " + _MessageHelper.GetMessage("generic read only"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Images"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDHYP";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "OVERWRITELIB";
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("READ", typeof(string)));
        dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
        dt.Columns.Add(new DataColumn("ADD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
        dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));
        if (_IsBoard)
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
        }
        else
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
        dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));
        if (!(data == null))
        {
            dr = dt.NewRow();

            if (Request.QueryString["base"] == "group")
            {
                dr[0] = data.GroupDisplayName;
            }
            else
            {
                dr[0] = data.DisplayUserName;
            }

            dr[1] = "<input type=\"checkbox\" name=\"frm_readonly\" ";
            if (_IsMembership)
            {
                dr[1] += " checked=\"checked\" onclick=\"return CheckReadOnlyForMembershipUser(\'frm_readonly\');\">";
            }
            else
            {
                dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_readonly\');\" />";
            }

            dr[2] = "<input type=\"checkbox\" name=\"frm_edit\" ";
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBlog))
            {
                dr[2] += " disabled=\"disabled\" ";
            }
            dr[2] += "  onclick=\"return CheckPermissionSettings(\'frm_edit\');\" />";


            dr[3] = "<input type=\"checkbox\" name=\"frm_add\" ";
            if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBoard || _IsBlog))
            {
                dr[3] += " disabled=\"disabled\" ";
            }
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_add\');\" />";

            dr[4] = "<input type=\"checkbox\" name=\"frm_delete\" ";
            if (_IsMembership && !(_IsBlog))
            {
                dr[4] += " disabled=\"disabled\" ";
            }
            dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete\');\" />";

            dr[5] = "<input type=\"checkbox\" name=\"frm_restore\"  ";
            if (_IsMembership)
            {
                dr[5] += " disabled=\"disabled\" ";
            }
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_restore\');\" />";

            dr[6] = "<input type=\"checkbox\" name=\"frm_libreadonly\" ";
            if ((_IsMembership) && !(_IsBoard || _IsBlog || _FolderData.IsCommunityFolder))
            {
                dr[6] += " checked=\"checked\" ";
            }
            dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_libreadonly\');\" />";

            if (_IsBoard == true)
            {
                dr[7] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                dr[7] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";

                dr[8] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                dr[8] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";
            }
            else
            {
                dr[7] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[7] += " disabled=\"disabled\" ";
                }
                dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";

                dr[8] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[8] += " disabled=\"disabled\" ";
                }
                dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
            }

            dr[9] = "<input type=\"checkbox\" name=\"frm_addhyperlinks\" ";
            if (_IsMembership)
            {
                dr[9] += " disabled=\"disabled\" ";
            }
            dr[9] += "onclick=\"return CheckPermissionSettings(\'frm_addhyperlinks\');\" />";

            dr[10] = "<input type=\"checkbox\" name=\"frm_overwritelib\" ";
            if (_IsMembership)
            {
                dr[10] += " disabled=\"disabled\" ";
            }
            dr[10] += "onclick=\"return CheckPermissionSettings(\'frm_overwritelib\');\" />";

            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsGenericGrid.DataSource = dv;
        PermissionsGenericGrid.DataBind();
    }

    private void Populate_AddPermissionsAdvancedGrid(UserData data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        if (_Base == "group")
        {
            strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic cms group label") + "</span>";
        }
        else
        {
            strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
        }

        colBound.DataField = "TITLE";
        colBound.HeaderText = strMsg;
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "COLLECTIONS";
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADDFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Add Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Edit Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETEFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Delete Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TRAVERSE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        SiteAPI m_refSiteApi = new SiteAPI();
        SettingsData setting_data = new SettingsData();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.EnablePreApproval)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ModifyPreapproval";
            colBound.HeaderText = "Modify Preapproval";
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("COLLECTIONS", typeof(string)));
        dt.Columns.Add(new DataColumn("ADDFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETEFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("TRAVERSE", typeof(string)));
        if (setting_data.EnablePreApproval)
        {
            dt.Columns.Add(new DataColumn("ModifyPreapproval", typeof(string)));
        }
        bool bInherited = false;
        if (_ItemType == "folder")
        {
            bInherited = _FolderData.Inherited;
        }
        else
        {
            bInherited = _ContentData.IsInherited;
        }


        if (!(data == null))
        {
            dr = dt.NewRow();
            dr[0] = data.DisplayUserName;

            dr[1] = "<input type=\"checkbox\"  id=\"frm_navigation\" name=\"frm_navigation\" ";
            dr[1] += "onclick=\"return CheckPermissionSettings(\'frm_navigation\');\" />";

            dr[2] = "<input type=\"checkbox\" id=\"frm_add_folders\"  name=\"frm_add_folders\" ";
            dr[2] += "onclick=\"return CheckPermissionSettings(\'frm_add_folders\');\" />";

            dr[3] = "<input type=\"checkbox\" id=\"frm_edit_folders\" name=\"frm_edit_folders\" ";
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_edit_folders\');\" />";

            dr[4] = "<input type=\"checkbox\" id=\"frm_delete_folders\" name=\"frm_delete_folders\" ";
            dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete_folders\');\" />";

            dr[5] = "<input type=\"checkbox\" id=\"frm_transverse_folder\" name=\"frm_transverse_folder\" checked=\"" + traverseFolder + "\"  ";
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_transverse_folder\');\" />";

            if (setting_data.EnablePreApproval)
            {
                dr[6] = "<input type=\"checkbox\" id=\"frm_edit_preapproval\" name=\"frm_edit_preapproval\" ";
                dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_edit_preapproval\');\" />";
            }
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsAdvancedGrid.DataSource = dv;
        PermissionsAdvancedGrid.DataBind();
    }
    private void Populate_AddPermissionsAdvancedGrid(UserGroupData data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        if (_Base == "group")
        {
            strMsg = "<span class=\"cmsGroup\">" + _MessageHelper.GetMessage("generic cms group label") + "</span>";
        }
        else
        {
            strMsg = "<span class=\"cmsUser\">" + _MessageHelper.GetMessage("generic cms user label") + "</span>";
        }

        colBound.DataField = "TITLE";
        colBound.HeaderText = strMsg;
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "COLLECTIONS";
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADDFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Add Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Edit Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETEFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Delete Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TRAVERSE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        SiteAPI m_refSiteApi = new SiteAPI();
        SettingsData setting_data = new SettingsData();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.EnablePreApproval)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ModifyPreapproval";
            colBound.HeaderText = "Modify Preapproval";
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("COLLECTIONS", typeof(string)));
        dt.Columns.Add(new DataColumn("ADDFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETEFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("TRAVERSE", typeof(string)));
        if (setting_data.EnablePreApproval)
        {
            dt.Columns.Add(new DataColumn("ModifyPreapproval", typeof(string)));
        }
        bool bInherited = false;
        if (_ItemType == "folder")
        {
            bInherited = _FolderData.Inherited;
        }
        else
        {
            bInherited = _ContentData.IsInherited;
        }


        if (!(data == null))
        {
            dr = dt.NewRow();

            if (Request.QueryString["base"] == "group")
            {
                dr[0] = data.GroupDisplayName;
            }
            else
            {
                dr[0] = data.DisplayUserName;
            }

            dr[1] = "<input type=\"checkbox\"  id=\"frm_navigation\" name=\"frm_navigation\" ";
            dr[1] += "onclick=\"return CheckPermissionSettings(\'frm_navigation\');\" />";

            dr[2] = "<input type=\"checkbox\" id=\"frm_add_folders\"  name=\"frm_add_folders\" ";
            dr[2] += "onclick=\"return CheckPermissionSettings(\'frm_add_folders\');\" />";

            dr[3] = "<input type=\"checkbox\" id=\"frm_edit_folders\" name=\"frm_edit_folders\" ";
            dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_edit_folders\');\" />";

            dr[4] = "<input type=\"checkbox\" id=\"frm_delete_folders\" name=\"frm_delete_folders\" ";
            dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete_folders\');\" />";

            dr[5] = "<input type=\"checkbox\" id=\"frm_transverse_folder\" name=\"frm_transverse_folder\" checked=\"" + traverseFolder + "\"  ";
            dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_transverse_folder\');\" />";

            if (setting_data.EnablePreApproval)
            {
                dr[6] = "<input type=\"checkbox\" id=\"frm_edit_preapproval\" name=\"frm_edit_preapproval\" ";
                dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_edit_preapproval\');\" />";
            }
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        PermissionsAdvancedGrid.DataSource = dv;
        PermissionsAdvancedGrid.DataBind();
    }
    private void Populate_AddPermissionsGenericGridForUsersAndGroup(System.Collections.Generic.List<UserGroupData> groupData, System.Collections.Generic.List<UserData> userData)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";
        int i = 0;
        int j = 0;

        colBound.DataField = "TITLE";
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = strMsg;
        PermissionsGenericGrid.Columns.Add(colBound);
        colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound.DataField = "READ";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDIT";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Edit title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADD";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETE";
        colBound.HeaderStyle.CssClass = "center";
        colBound.ItemStyle.CssClass = "center";
        if (_IsBoard)
        {
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Delete title") + " Topic");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
        }
        PermissionsGenericGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "RESTORE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        if (_IsBoard)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }
        else
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GREAD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Library title") + " " + _MessageHelper.GetMessage("generic read only"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADD";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Images"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDFILE";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "GADDHYP";
            colBound.HeaderText = (string)(_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "OVERWRITELIB";
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
            colBound.HeaderStyle.CssClass = "center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsGenericGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("READ", typeof(string)));
        dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
        dt.Columns.Add(new DataColumn("ADD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
        dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));
        if (_IsBoard)
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
        }
        else
        {
            dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADD", typeof(string)));
            dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
        }

        dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
        dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));

        if (groupData.Count() > 0)
        {
            for (i = 0; i <= groupData.Count - 1; i++)
            {
                dr = dt.NewRow();
                if (groupData[i].IsMemberShipGroup)
                {
                    dr[0] = "<span class=\"membershipGroup\">" + groupData[i].GroupDisplayName + "</span>";
                }
                else
                {
                    dr[0] = "<span class=\"cmsGroup\">" + groupData[i].GroupDisplayName + "</span>";
                }

                dr[1] = "<input type=\"checkbox\" name=\"frm_readonly\" ";
                if (_IsMembership)
                {
                    dr[1] += " checked=\"checked\" onclick=\"return CheckReadOnlyForMembershipUser(\'frm_readonly\');\">";
                }
                else
                {
                    dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_readonly\');\" />";
                }

                dr[2] = "<input type=\"checkbox\" name=\"frm_edit\" ";
                if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBlog))
                {
                    dr[2] += " disabled=\"disabled\" ";
                }
                dr[2] += "  onclick=\"return CheckPermissionSettings(\'frm_edit\');\" />";


                dr[3] = "<input type=\"checkbox\" name=\"frm_add\" ";
                if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBoard || _IsBlog))
                {
                    dr[3] += " disabled=\"disabled\" ";
                }
                dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_add\');\" />";

                dr[4] = "<input type=\"checkbox\" name=\"frm_delete\" ";
                if (_IsMembership && !(_IsBlog))
                {
                    dr[4] += " disabled=\"disabled\" ";
                }
                dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete\');\" />";

                dr[5] = "<input type=\"checkbox\" name=\"frm_restore\"  ";
                if (_IsMembership)
                {
                    dr[5] += " disabled=\"disabled\" ";
                }
                dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_restore\');\" />";

                dr[6] = "<input type=\"checkbox\" name=\"frm_libreadonly\" ";
                if ((_IsMembership) && !(_IsBoard || _IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[6] += " checked=\"checked\" ";
                }
                dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_libreadonly\');\" />";

                if (_IsBoard == true)
                {
                    dr[7] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                    dr[7] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";

                    dr[8] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                    dr[8] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";
                }
                else
                {
                    dr[7] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                    if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                    {
                        dr[7] += " disabled=\"disabled\" ";
                    }
                    dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";

                    dr[8] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                    if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                    {
                        dr[8] += " disabled=\"disabled\" ";
                    }
                    dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
                }

                dr[9] = "<input type=\"checkbox\" name=\"frm_addhyperlinks\" ";
                if (_IsMembership)
                {
                    dr[9] += " disabled=\"disabled\" ";
                }
                dr[9] += "onclick=\"return CheckPermissionSettings(\'frm_addhyperlinks\');\" />";

                dr[10] = "<input type=\"checkbox\" name=\"frm_overwritelib\" ";
                if (_IsMembership)
                {
                    dr[10] += " disabled=\"disabled\" ";
                }
                dr[10] += "onclick=\"return CheckPermissionSettings(\'frm_overwritelib\');\" />";

                dt.Rows.Add(dr);
            }
        }
        if (userData.Count > 0)
        {
            for (j = 0; j <= userData.Count - 1; j++)
            {
                dr = dt.NewRow();
                if (userData[j].IsMemberShip)
                {
                    dr[0] = "<span class=\"membershipUser\">" + userData[j].Username + "</span>";
                }
                else
                {
                    dr[0] = "<span class=\"cmsUser\">" + userData[j].Username + "</span>";
                }

                dr[1] = "<input type=\"checkbox\" name=\"frm_readonly\" ";
                if (_IsMembership)
                {
                    dr[1] += " checked=\"checked\" onclick=\"return CheckReadOnlyForMembershipUser(\'frm_readonly\');\">";
                }
                else
                {
                    dr[1] += " onclick=\"return CheckPermissionSettings(\'frm_readonly\');\" />";
                }

                dr[2] = "<input type=\"checkbox\" name=\"frm_edit\" ";
                if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBlog))
                {
                    dr[2] += " disabled=\"disabled\" ";
                }
                dr[2] += "  onclick=\"return CheckPermissionSettings(\'frm_edit\');\" />";


                dr[3] = "<input type=\"checkbox\" name=\"frm_add\" ";
                if (_IsMembership && (!_FolderData.IsCommunityFolder) && !(_IsBoard || _IsBlog))
                {
                    dr[3] += " disabled=\"disabled\" ";
                }
                dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_add\');\" />";

                dr[4] = "<input type=\"checkbox\" name=\"frm_delete\" ";
                if (_IsMembership && !(_IsBlog))
                {
                    dr[4] += " disabled=\"disabled\" ";
                }
                dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete\');\" />";

                dr[5] = "<input type=\"checkbox\" name=\"frm_restore\"  ";
                if (_IsMembership)
                {
                    dr[5] += " disabled=\"disabled\" ";
                }
                dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_restore\');\" />";

                dr[6] = "<input type=\"checkbox\" name=\"frm_libreadonly\" ";
                if ((_IsMembership) && !(_IsBoard || _IsBlog || _FolderData.IsCommunityFolder))
                {
                    dr[6] += " checked=\"checked\" ";
                }
                dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_libreadonly\');\" />";

                if (_IsBoard == true)
                {
                    dr[7] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                    dr[7] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";

                    dr[8] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                    dr[8] += " onclick=\"return CheckPermissionSettings(\'frm_moderate\');\" />";
                }
                else
                {
                    dr[7] = "<input type=\"checkbox\" name=\"frm_addimages\"  ";
                    if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                    {
                        dr[7] += " disabled=\"disabled\" ";
                    }
                    dr[7] += " onclick=\"return CheckPermissionSettings(\'frm_addimages\');\" />";

                    dr[8] = "<input type=\"checkbox\" name=\"frm_addfiles\" ";
                    if (_IsMembership && !(_IsBlog || _FolderData.IsCommunityFolder))
                    {
                        dr[8] += " disabled=\"disabled\" ";
                    }
                    dr[8] += "onclick=\"return CheckPermissionSettings(\'frm_addfiles\');\" />";
                }

                dr[9] = "<input type=\"checkbox\" name=\"frm_addhyperlinks\" ";
                if (_IsMembership)
                {
                    dr[9] += " disabled=\"disabled\" ";
                }
                dr[9] += "onclick=\"return CheckPermissionSettings(\'frm_addhyperlinks\');\" />";

                dr[10] = "<input type=\"checkbox\" name=\"frm_overwritelib\" ";
                if (_IsMembership)
                {
                    dr[10] += " disabled=\"disabled\" ";
                }
                dr[10] += "onclick=\"return CheckPermissionSettings(\'frm_overwritelib\');\" />";

                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        PermissionsGenericGrid.DataSource = dv;
        PermissionsGenericGrid.DataBind();
    }

    private void Populate_AddPermissionsAdvancedGridForUsersAndGroup(System.Collections.Generic.List<UserGroupData> groupData, System.Collections.Generic.List<UserData> userData)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strMsg = "";

        colBound.DataField = "TITLE";
        colBound.HeaderText = strMsg;
        colBound.HeaderStyle.CssClass = "title-header left";
        colBound.ItemStyle.CssClass = "left";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "COLLECTIONS";
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADDFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Add Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Edit Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DELETEFLD";
        if (_IsBoard)
        {
            colBound.HeaderText = "Delete Forum";
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
        }
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        PermissionsAdvancedGrid.Columns.Add(colBound);

        if (!(_IsBoard))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TRAVERSE";
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        SiteAPI m_refSiteApi = new SiteAPI();
        SettingsData setting_data = new SettingsData();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.EnablePreApproval)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ModifyPreapproval";
            colBound.HeaderText = "Modify Preapproval";
            colBound.HeaderStyle.CssClass = "title-header center";
            colBound.ItemStyle.CssClass = "center";
            PermissionsAdvancedGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("COLLECTIONS", typeof(string)));
        dt.Columns.Add(new DataColumn("ADDFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("DELETEFLD", typeof(string)));
        dt.Columns.Add(new DataColumn("TRAVERSE", typeof(string)));
        if (setting_data.EnablePreApproval)
        {
            dt.Columns.Add(new DataColumn("ModifyPreapproval", typeof(string)));
        }
        bool bInherited = false;
        if (_ItemType == "folder")
        {
            bInherited = _FolderData.Inherited;
        }
        else
        {
            bInherited = _ContentData.IsInherited;
        }

        int i = 0;
        int j = 0;

        if (groupData.Count > 0)
        {
            for (i = 0; i <= groupData.Count - 1; i++)
            {

                dr = dt.NewRow();
                if (groupData[i].IsMemberShipGroup)
                {
                    dr[0] = "<span class=\"membershipGroup\">" + groupData[i].GroupDisplayName + "</span>";
                }
                else
                {
                    dr[0] = "<span class=\"cmsGroup\">" + groupData[i].GroupDisplayName + "</span>";
                }

                dr[1] = "<input type=\"checkbox\"  id=\"frm_navigation\" name=\"frm_navigation\" ";
                dr[1] += "onclick=\"return CheckPermissionSettings(\'frm_navigation\');\" />";

                dr[2] = "<input type=\"checkbox\" id=\"frm_add_folders\"  name=\"frm_add_folders\" ";
                dr[2] += "onclick=\"return CheckPermissionSettings(\'frm_add_folders\');\" />";

                dr[3] = "<input type=\"checkbox\" id=\"frm_edit_folders\" name=\"frm_edit_folders\" ";
                dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_edit_folders\');\" />";

                dr[4] = "<input type=\"checkbox\" id=\"frm_delete_folders\" name=\"frm_delete_folders\" ";
                dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete_folders\');\" />";

                dr[5] = "<input type=\"checkbox\" id=\"frm_transverse_folder\" name=\"frm_transverse_folder\" checked=\"" + traverseFolder + "\"  ";
                dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_transverse_folder\');\" />";

                if (setting_data.EnablePreApproval)
                {
                    dr[6] = "<input type=\"checkbox\" id=\"frm_edit_preapproval\" name=\"frm_edit_preapproval\" ";
                    dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_edit_preapproval\');\" />";
                }
                dt.Rows.Add(dr);
            }
        }
        if (userData.Count > 0)
        {
            for (j = 0; j <= userData.Count - 1; j++)
            {
                dr = dt.NewRow();
                if (userData[j].IsMemberShip)
                {
                    dr[0] = "<span class=\"membershipUser\">" + userData[j].Username + "</span>";
                }
                else
                {
                    dr[0] = "<span class=\"cmsUser\">" + userData[j].Username + "</span>";
                }

                dr[1] = "<input type=\"checkbox\"  id=\"frm_navigation\" name=\"frm_navigation\" ";
                dr[1] += "onclick=\"return CheckPermissionSettings(\'frm_navigation\');\">";

                dr[2] = "<input type=\"checkbox\" id=\"frm_add_folders\"  name=\"frm_add_folders\" ";
                dr[2] += "onclick=\"return CheckPermissionSettings(\'frm_add_folders\');\" />";

                dr[3] = "<input type=\"checkbox\" id=\"frm_edit_folders\" name=\"frm_edit_folders\" ";
                dr[3] += "onclick=\"return CheckPermissionSettings(\'frm_edit_folders\');\" />";

                dr[4] = "<input type=\"checkbox\" id=\"frm_delete_folders\" name=\"frm_delete_folders\" ";
                dr[4] += "onclick=\"return CheckPermissionSettings(\'frm_delete_folders\');\" />";

                dr[5] = "<input type=\"checkbox\" id=\"frm_transverse_folder\" name=\"frm_transverse_folder\" checked=\"" + traverseFolder + "\"  ";
                dr[5] += "onclick=\"return CheckPermissionSettings(\'frm_transverse_folder\');\" />";

                if (setting_data.EnablePreApproval)
                {
                    dr[6] = "<input type=\"checkbox\" id=\"frm_edit_preapproval\" name=\"frm_edit_preapproval\" ";
                    dr[6] += "onclick=\"return CheckPermissionSettings(\'frm_edit_preapproval\');\" />";
                }
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        PermissionsAdvancedGrid.DataSource = dv;
        PermissionsAdvancedGrid.DataBind();
    }

    private void AddPermissionsToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string WorkareaTitlebarTitle = "";
        if (_ItemType == "folder")
        {
            WorkareaTitlebarTitle = (string)(_MessageHelper.GetMessage("add folder permissions") + " \"" + _FolderData.Name + "\"");
        }
        else
        {
            WorkareaTitlebarTitle = (string)(_MessageHelper.GetMessage("add content permissions") + " \"" + _ContentData.Title + "\"");
        }
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle);
        result.Append("<table><tr>");
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?action=SelectPermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&membership=" + Request.QueryString["membership"] + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("add permissions"), _MessageHelper.GetMessage("btn save"), "onclick=\"javascript:return SubmitForm(\'frmContent\', \'CheckForAnyPermissions()\');\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        if (_IsBoard)
        {
            result.Append(_StyleHelper.GetHelpButton("addboardperms", ""));
        }
        else
        {
            result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    #endregion

    #region CSS, JS

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

    #endregion
}