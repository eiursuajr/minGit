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
using Ektron.Cms.Commerce;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;
using Ektron.Cms.Community;
using Ektron.Facebook.Rest;
using Ektron.Cms.Framework.Activity;
using Ektron.Cms.Activity;

public partial class users : System.Web.UI.Page
{
    protected TagsAPI m_refTagsApi = new Ektron.Cms.Community.TagsAPI();
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string m_strPageAction = "";
    protected Collection pagedata;
    protected long CurrentUserID;
    protected viewusers m_viewusers;
    protected viewgroups m_viewgroups;
    protected adduser m_adduser;
    protected editgroups m_editgroups;
    protected edituser m_edituser;
    protected viewcustomproperties m_viewcustomproperties;
    protected addcustomproperty m_addeditCustomproperty;
    protected SiteAPI m_refApi = new SiteAPI();
    protected int ContentLanguage;
    protected string SitePath = "";
    protected EkMessageHelper m_refMsg;
    protected SettingsData setting_data;
    protected LibraryConfigData lib_setting_data;
    protected Ektron.Cms.Framework.Notifications.NotificationPreference _notificationPreferenceApi = new Ektron.Cms.Framework.Notifications.NotificationPreference();
    protected System.Collections.Generic.List<NotificationPreferenceData> preferenceList;
    protected Ektron.Cms.Framework.Activity.ActivityType _activityListApi = new Ektron.Cms.Framework.Activity.ActivityType();
    protected System.Collections.Generic.List<Ektron.Cms.Activity.ActivityTypeData> collActivityTypeList;
    protected System.Collections.Generic.List<ActivityTypeData> groupActivityTypeList;
    protected NotificationPreferenceData prefData = new NotificationPreferenceData();

    protected void Page_Init(object sender, System.EventArgs e)
    {
        if ((Request.QueryString["RequestedBy"] != null) && Request.QueryString["RequestedBy"] == "EktronCommerceItemsSusbscriptionsMembership")
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
        }

        if (!(Request.QueryString["action"] == null))
        {
            if (Request.QueryString["action"] != "")
            {
                m_strPageAction = Request.QueryString["action"].ToLower();
            }
        }
        if ("edituser" == m_strPageAction)
        {
            m_edituser = (edituser)(LoadControl("controls/user/edituser.ascx"));
            m_edituser.ID = "user";
            m_edituser.IsCmsUser = (Request.QueryString["grouptype"] == "1") ? false : true;
            DataHolder.Controls.Add(m_edituser);
        }
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        jsStyleSheet.Text = (new StyleHelper()).GetClientScript();
        m_refMsg = (new CommonApi()).EkMsgRef;
        if ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", m_refContentApi.UserId) == false)
        {
            Response.Redirect("login.aspx?fromLnkPg=1", false);
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser > 0 || m_refContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect("reterror.aspx?info=Please login as cms user", false);
            return;
        }
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (m_refApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (m_refApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(m_refApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        lib_setting_data = m_refContentApi.GetLibrarySettings(0); //Used in the scripting layer

        SetServerJSVariables();
        RegisterResources();

        Ektron.Cms.Commerce.IPasswordValidation pv = ObjectFactory.GetPasswordValidation();
        string validationString = (string)(pv.GetRegexForMember().Replace("\"", "\\\"").Replace("\\t", "\\\\t"));

        passwordValidationString.Text = validationString;

        MakeEmailArea.Text = (new EmailHelper()).MakeEmailArea();
        if (!String.IsNullOrEmpty(Request.QueryString["reloadtrees"]))
        {
            CloseScriptJS.Text = ClientCloseScriptJS();
        }
        jsADIntegration.Text = "false";
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if (setting_data.ADIntegration)
        {
            jsADIntegration.Text = "true";
        }
        SitePath = m_refSiteApi.SitePath;
        jsSitePath.Text = SitePath;
        if (m_strPageAction == "viewallusers")
        {
            m_viewusers = (viewusers)(LoadControl("controls/user/viewusers.ascx"));
            m_viewusers.ID = "user";
            jsUniqueId.Text = "user_";
            DataHolder.Controls.Add(m_viewusers);
        }
        else if (m_strPageAction == "addusertogroup")
        {
            m_editgroups = (editgroups)(LoadControl("controls/user/editgroups.ascx"));
            m_editgroups.ID = "user";
            jsUniqueId.Text = "user_";
            DataHolder.Controls.Add(m_editgroups);
        }
        else if (m_strPageAction == "viewallgroups")
        {
            m_viewgroups = (viewgroups)(LoadControl("controls/user/viewgroups.ascx"));
            m_viewgroups.ID = "user";
            jsUniqueId.Text = "user_";
            DataHolder.Controls.Add(m_viewgroups);
        }
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        try
        {
            switch (m_strPageAction)
            {
                case "addusergroup":
                    m_editgroups = (editgroups)(LoadControl("controls/user/editgroups.ascx"));
                    m_editgroups.ID = "user";
                    jsUniqueId.Text = "user_";
                    DataHolder.Controls.Add(m_editgroups);
                    m_editgroups.AddUserGroup();
                    break;

                case "editusergroup":
                    m_editgroups = (editgroups)(LoadControl("controls/user/editgroups.ascx"));
                    m_editgroups.ID = "user";
                    jsUniqueId.Text = "user_";
                    DataHolder.Controls.Add(m_editgroups);
                    m_editgroups.EditUserGroup();
                    break;

                case "view":
                    m_viewusers = (viewusers)(LoadControl("controls/user/viewusers.ascx"));
                    DataHolder.Controls.Add(m_viewusers);
                    m_viewusers.View();
                    break;

                case "addusertosystem":
                    m_adduser = (adduser)(LoadControl("controls/user/adduser.ascx"));
                    m_adduser.ID = "user";
                    jsUniqueId.Text = "user_";
                    DataHolder.Controls.Add(m_adduser);
                    m_adduser.AddUserToSystem();
                    break;

                case "edituser":
                    if (!(Page.IsPostBack))
                    {
                        jsUniqueId.Text = "";
                        m_edituser.setting_data = setting_data;
                        m_edituser.EditUser();
                    }
                    else
                    {
                        EditUser();
                    }
                    break;

                case "mapcmsusergrouptoad":
                    m_viewgroups = (viewgroups)(LoadControl("controls/user/viewgroups.ascx"));
                    m_viewgroups.ID = "user";
                    m_viewgroups.ActiveDirectory = true;
                    jsUniqueId.Text = "user_";
                    DataHolder.Controls.Add(m_viewgroups);
                    m_viewgroups.MapCMSUserGroupToAD();
                    break;

                case "mapcmsusertoad":
                    m_viewusers = (viewusers)(LoadControl("controls/user/viewusers.ascx"));
                    m_viewusers.ID = "user";
                    jsUniqueId.Text = "user_";
                    DataHolder.Controls.Add(m_viewusers);
                    m_viewusers.MapCMSUserToAD();
                    break;

                case "reorderproperties":
                    m_viewcustomproperties = (viewcustomproperties)(LoadControl("controls/user/viewcustomproperties.ascx"));
                    m_viewcustomproperties.ID = "userProperties";
                    DataHolder.Controls.Add(m_viewcustomproperties);
                    break;

                case "viewcustomprop":
                    m_viewcustomproperties = (viewcustomproperties)(LoadControl("controls/user/viewcustomproperties.ascx"));
                    m_viewcustomproperties.ID = "userProperties";
                    DataHolder.Controls.Add(m_viewcustomproperties);
                    break;

                case "addcustomprop":
                    m_addeditCustomproperty = (addcustomproperty)(LoadControl("controls/user/addcustomproperty.ascx"));
                    m_addeditCustomproperty.ID = "addCustomProp";
                    DataHolder.Controls.Add(m_addeditCustomproperty);
                    break;

                case "editcustomprop":
                    m_addeditCustomproperty = (addcustomproperty)(LoadControl("controls/user/addcustomproperty.ascx"));
                    m_addeditCustomproperty.ID = "editCustomProp";
                    DataHolder.Controls.Add(m_addeditCustomproperty);
                    break;

                case "deletecustomprop":
                    m_addeditCustomproperty = (addcustomproperty)(LoadControl("controls/user/addcustomproperty.ascx"));
                    m_addeditCustomproperty.ID = "delCustomProp";
                    DataHolder.Controls.Add(m_addeditCustomproperty);
                    break;

                case "deleteuserfromsystem":
                    DeleteUserFromSystem();
                    break;

                case "deleteuserfromgroup":
                    DeleteUserFromGroup();
                    break;

                case "doaddusertogroup":
                    AddUserToGroup();
                    break;

                case "deletegroup":
                    DeleteGroup();
                    break;

                case "updateaduser":
                    UpdateADUser();
                    break;

                case "activateuseraccount":
                    ActivateUserAccount();
                    break;
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private void ActivateUserAccount()
    {
        bool ret = false;
        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            ret = m_refUserApi.ActivateUserAccount(Convert.ToInt64(Request.QueryString["id"]));
        }
        Response.Redirect("users.aspx?action=view&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["groupid"] + "&id=" + Request.QueryString["id"] + "&OrderBy=UserName", false);
    }

    private void DeleteUserFromSystem()
    {
        string err = "";
        try
        {
            string strPendingVerifyUserQueryString = "&FromUsers=1";
            if ((!(Request.QueryString["ty"] == null)) && (Request.QueryString["ty"] == "nonverify"))
            {
                strPendingVerifyUserQueryString = "&ty=nonverify"; // membership user not verifed yet
                Ektron.Cms.User.EkUser objUser;
                objUser = m_refSiteApi.EkUserRef;
                objUser.DeleteMembershipUsers(EkFunctions.HtmlEncode(Request.QueryString["id"]));
            }
            else
            {
                m_refUserApi.DeleteUserByID(Convert.ToInt64(Request.QueryString["id"]));
            }
            Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["groupid"] + "&id=2&OrderBy=" + Request.QueryString["OrderBy"] + strPendingVerifyUserQueryString), false);

        }
        catch (Exception ex)
        {
            err = EkFunctions.UrlEncode(ex.Message);
            Response.Redirect((string)("reterror.aspx?info=" + err), false);
        }
    }

    private void DeleteUserFromGroup()
    {
        long uID = Convert.ToInt64(Request.QueryString["id"]);
        long GroupID = Convert.ToInt64(Request.QueryString["GroupID"]);
        m_refUserApi.DeleteUserFromGroup(uID, GroupID, false);
        Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["groupid"] + "&id=" + GroupID + "&OrderBy=" + Request.QueryString["OrderBy"]), false);
    }

    private string ClientCloseScriptJS()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<script language=\"javascript\" src=\"java/QueryStringParser.js\"></script>" + "\r\n");
        result.Append("<script language=\"javascript\">" + "\r\n");
        result.Append("<!--" + "\r\n");
        result.Append("var m_reloadTrees = QueryString(\"reloadtrees\");" + "\r\n");
        result.Append("top.ReloadTrees(m_reloadTrees);" + "\r\n");
        result.Append("self.location.href=\"" + Request.ServerVariables["path_info"] + "?" + Strings.Replace(Request.ServerVariables["query_string"], (string)("&reloadtrees=" + Request.QueryString["reloadtrees"]), "", 1, -1, 0) + "\";" + "\r\n");
        result.Append("//-->" + "\r\n");
        result.Append("</script>");
        return (result.ToString());
    }

    private void AddUserToGroup()
    {
        m_refUserApi.AddUserToGroup(Convert.ToInt64(Request.QueryString["UserID"]), Convert.ToInt64(Request.QueryString["GroupID"]));
        Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["GroupID"] + "&id=" + Request.QueryString["GroupID"] + "&OrderBy=" + Request.QueryString["OrderBy"]), false);
    }
    private void EditUser()
    {
        if (Request.QueryString["grouptype"] == "1")
        {
            EditMemberShipUser();
        }
        else
        {
            EditCmsUser();
        }
    }
    private void EditCmsUser()
    {
        Collection UserPrefs;
        string strFolderID;
        string DisplayBorders;
        string DisplayTitleText;
        string FromUsers;
        string Org = "";
        string[] arrOrgU = null;
        long arrCount;
        string[] arrDC = null;
        string strDC = "";
        long lDC;
        pagedata = new Collection();
        UserPrefs = new Collection();
        Ektron.Cms.Commerce.IPasswordValidation pv = ObjectFactory.GetPasswordValidation();
        ValidationResults results;

        FromUsers = Request.QueryString["FromUsers"];
        CurrentUserID = Convert.ToInt64(Request.QueryString["id"]); //Request.Form("id")  POST BACK RETURNS THE SAME ID
        pagedata.Add(CurrentUserID, "UserID", null, null);
        pagedata.Add(Request.Form["username"], "UserName", null, null);
        if (!String.IsNullOrEmpty(Request.Form["organization_text"]))
        {
            if (!String.IsNullOrEmpty(Request.Form["orgunit_text"]))
            {
                arrOrgU = (Request.Form["orgunit_text"].ToString()).Split(',');
                for (arrCount = 0; arrCount <= (arrOrgU.Length - 1); arrCount++)
                {
                    if (!(arrOrgU[arrCount] == ""))
                    {
                        if (!(Org == ""))
                        {
                            Org += ",";
                        }
                        Org += "ou=";
                        Org += arrOrgU[arrCount];
                    }
                }
            }
            if (!String.IsNullOrEmpty(Request.Form["organization_text"]))
            {
                Org += ",o=";
                Org += Request.Form["organization_text"].ToString();
            }
            if (!String.IsNullOrEmpty(Request.Form["ldapdomain_text"]))
            {
                arrDC = (Request.Form["ldapdomain_text"].ToString()).Split('.');
                for (lDC = 0; lDC <= (arrDC.Length - 1); lDC++)
                {
                    strDC += ",dc=";
                    strDC += arrDC[lDC];
                }
                Org += strDC;
            }
            pagedata.Add(Org, "Domain", null, null);
        }
        else if (!String.IsNullOrEmpty(Request.Form["ldapdomain_text"]))
        {
            Org = Request.Form["ldapdomain_text"].ToString();
            pagedata.Add(Org, "Domain", null, null);
        }
        else
        {
            pagedata.Add(Request.Form["domain"], "Domain", null, null);
        }
        pagedata.Add(Request.Form["userpath"], "UserPath", null, null);
        pagedata.Add(Request.Form["firstname"], "FirstName", null, null);
        pagedata.Add(Request.Form["lastname"], "LastName", null, null);
        pagedata.Add(Request.Form["displayname"], "DisplayName", null, null);
        pagedata.Add(Request.Form["mapaddress"], "Address", null, null);
        if (m_edituser != null)
        {
            pagedata.Add(m_edituser.GetSignature(), "Signature", null, null);
        }
        else
        {
            pagedata.Add("", "Signature", null, null);
        }
        if (Request.Form["avatar"] != "")
        {
            pagedata.Add(Request.Form["avatar"], "Avatar", null, null);
        }
        else
        {
            pagedata.Add("", "Avatar", null, null);
        }
        pagedata.Add(Request.Form["pwd"], "Password", null, null);
        pagedata.Add(Request.Form["language"], "Language", null, null);
        if (!String.IsNullOrEmpty(Request.Form["user$drp_editor"]))
        {
            pagedata.Add(Request.Form["user$drp_editor"], "EditorOptions", null, null);
        }
        else
        {
            pagedata.Add("", "EditorOptions", null, null);
        }
        if (Request.Form["chkFullScreen"] == "on")
        {
            UserPrefs.Add(Request.Form["hdnWidth"], "width", null, null);
            UserPrefs.Add(Request.Form["hdnHeight"], "height", null, null);
        }
        else
        {
            if (Request.Form["txtWidth"] == null || Information.IsNumeric(Request.Form["txtWidth"]) == false)
            {
                UserPrefs.Add(400, "width", null, null);
            }
            else
            {
                UserPrefs.Add(Request.Form["txtWidth"], "width", null, null);
            }
            if (Request.Form["txtHeight"] == null || Information.IsNumeric(Request.Form["txtHeight"]) == false)
            {
                UserPrefs.Add(300, "height", null, null);
            }
            else
            {
                UserPrefs.Add(Request.Form["txtHeight"], "height", null, null);
            }
        }
        string templateFilename = Request.Form["templatefilename"];
        if (templateFilename != null && templateFilename.Length > 0)
        {
            UserPrefs.Add(templateFilename, "template", null, null);
        }
        else
        {
            UserPrefs.Add("", "template", null, null);
        }
        if (!String.IsNullOrEmpty(Request.Form["email_addr1"]))
        {
            pagedata.Add(Request.Form["email_addr1"], "EmailAddr1", null, null);
        }
        else
        {
            pagedata.Add("", "EmailAddr1", null, null);
        }
        if (Request.Form["chkAccountLocked"] != null && Request.Form["chkAccountLocked"].ToString().ToLower() == "on")
        {
            pagedata.Add(254, "LoginAttempts", null, null);
        }
        else
        {
            pagedata.Add(0, "LoginAttempts", null, null);
        }
        if (Request.Form["disable_msg"] != null && Request.Form["disable_msg"].ToString().ToLower() == "disable_msg")
        {
            pagedata.Add(1, "DisableMsg", null, null);
        }
        else
        {
            pagedata.Add(0, "DisableMsg", null, null);
        }
        pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty", null, null);
        if (Request.Form["chkSmartDesktop"] == "on")
        {
            strFolderID = "";
        }
        else
        {
            if (Request.Form["folderId"] != "")
            {
                strFolderID = Request.Form["folderId"];
            }
            else
            {
                strFolderID = "0";
            }
        }
        UserPrefs.Add(strFolderID, "folderid", null, null);
        DisplayBorders = "1";
        UserPrefs.Add(DisplayBorders, "dispborders", null, null);
        if (Request.Form["chkDispTitleText"] == "on")
        {
            DisplayTitleText = "1";
        }
        else
        {
            DisplayTitleText = "0";
        }
        UserPrefs.Add(Request.Form["hdn_pagesize"], "pagesize", null, null);
        UserPrefs.Add(DisplayTitleText, "disptitletext", null, null);

        if (m_refUserApi.IsAGroupMember(CurrentUserID, (long)Ektron.Cms.Common.EkEnumeration.UserGroups.AdminGroup))
        {
            results = pv.ValidateForAdmin(Request.Form["pwd"]);
        }
        else if (m_refUserApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin, CurrentUserID, false))
        {
            results = pv.ValidateForCommerceAdmin(Request.Form["pwd"]);
        }
        else
        {
            results = pv.ValidateForAuthor(Request.Form["pwd"]);
        }
        if (results.IsValid)
        {

            m_refUserApi.UpdateUser(pagedata);
            HttpCookie cookEcm = Ektron.Cms.CommonApi.GetEcmCookie();
            if (Request.Form["user$drp_editor"] != "")
            {
                cookEcm.Values["editoroptions"] = Request.Form["user$drp_editor"];
            }
            else
            {
                cookEcm.Values["editoroptions"] = "";
            }
            Response.AppendCookie(cookEcm);
            m_refUserApi.UpdateUserPreferences(Convert.ToInt64(Request.QueryString["id"]), UserPrefs);
            ProcessUserMessageBoardModeration(Convert.ToInt64(Request.QueryString["id"]));
            UpdatePersonalTags();
            UpdateActivityPreferences();
            UpdatePublishPreferences();
            Response.Redirect((string)("users.aspx?action=view&grouptype=" + Request.QueryString["grouptype"] + "&id=" + CurrentUserID + "&FromUsers=" + FromUsers + "&groupid=" + Request.QueryString["groupid"] + "&OrderBy=" + Request.QueryString["OrderBy"]), false);

        }
        else
        {
            string msg = string.Empty;

            foreach (ValidationResult result in results)
            {
                msg = result.Message;
                break;
            }
            throw (new Exception(msg));
        }
    }
    private void EditMemberShipUser()
    {
        bool bEmailChanged = false;
        Ektron.Cms.User.EkUser objUser;
        bool ret = false;
        pagedata = new Collection();
        CurrentUserID = Convert.ToInt64(Request.QueryString["id"]);
        pagedata.Add(CurrentUserID, "UserID", null, null);
        pagedata.Add(Request.Form["username"], "UserName", null, null);
        string Org = "";
        string[] arrOrgU;
        long arrCount;
        string[] arrDC;
        string strDC = "";
        long lDC;
        if (!string.IsNullOrEmpty(Request.Form["organization_text"]))
        {
            if (!string.IsNullOrEmpty(Request.Form["orgunit_text"]))
            {
                arrOrgU = (Request.Form["orgunit_text"].ToString()).Split(',');
                for (arrCount = 0; arrCount <= (arrOrgU.Length - 1); arrCount++)
                {
                    if (!(arrOrgU[arrCount] == ""))
                    {
                        if (!(Org == ""))
                        {
                            Org += ",";
                        }
                        Org += "ou=";
                        Org += arrOrgU[arrCount];
                    }
                }
            }
            if (!string.IsNullOrEmpty(Request.Form["organization_text"]))
            {
                Org += ",o=";
                Org += Request.Form["organization_text"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Form["ldapdomain_text"]))
            {
                arrDC = (Request.Form["ldapdomain_text"].ToString()).Split('.');
                for (lDC = 0; lDC <= (arrDC.Length - 1); lDC++)
                {
                    strDC += ",dc=";
                    strDC += arrDC[lDC];
                }
                Org += strDC;
            }
            pagedata.Add(Org, "Domain", null, null);
        }
        else if (!string.IsNullOrEmpty(Request.Form["ldapdomain_text"]))
        {
            Org = Request.Form["ldapdomain_text"].ToString();
            pagedata.Add(Org, "Domain", null, null);
        }
        else
        {
            pagedata.Add(Request.Form["domain"], "Domain", null, null);
        }
        pagedata.Add(Request.Form["userpath"], "UserPath", null, null);
        pagedata.Add(Request.Form["firstname"], "FirstName", null, null);
        pagedata.Add(Request.Form["lastname"], "LastName", null, null);
        pagedata.Add(Request.Form["pwd"], "Password", null, null);
        pagedata.Add(Request.Form["language"], "Language", null, null);
        pagedata.Add(Request.Form["displayname"], "DisplayName", null, null);
        string forumSignature;
        forumSignature = (string)(m_edituser.GetSignature());
        pagedata.Add(forumSignature, "Signature", null, null);

        if (!string.IsNullOrEmpty(Request.Form["avatar"]))
        {
            pagedata.Add(Request.Form["avatar"], "Avatar", null, null);
        }
        else
        {
            pagedata.Add("", "Avatar", null, null);
        }
        if (!string.IsNullOrEmpty(Request.Form["user$drp_editor"]))
        {
            pagedata.Add(Request.Form["user$drp_editor"], "EditorOptions", null, null);
        }
        else
        {
            pagedata.Add("", "EditorOptions", null, null);
        }
        if (!String.IsNullOrEmpty(Request.Form["chkAccountLocked"]))
        {
            pagedata.Add(254, "LoginAttempts", null, null);
        }
        else
        {
            pagedata.Add(0, "LoginAttempts", null, null);
        }
        if (!string.IsNullOrEmpty(Request.Form["email_addr1"]))
        {
            if (Request.Form["email_addr1Org"] != Request.Form["email_addr1"])
            {
                bEmailChanged = true;
            }
            pagedata.Add(Request.Form["email_addr1"], "EmailAddr1", null, null);
        }
        else
        {
            pagedata.Add("", "EmailAddr1", null, null);
        }
        if (Request.Form["disable_msg"] != null && Request.Form["disable_msg"].ToString().ToLower() == "disable_msg")
        {
            pagedata.Add(1, "DisableMsg", null, null);
        }
        else
        {
            pagedata.Add(0, "DisableMsg", null, null);
        }
        if ((Request.Form["mapaddress"] != null) && Request.Form["mapaddress"] != "")
        {
            pagedata.Add(Request.Form["mapaddress"], "Address", null, null);
        }
        else
        {
            pagedata.Add("", "Address", null, null);
        }
        pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty", null, null);

        objUser = m_refSiteApi.EkUserRef;

        ret = objUser.UpDateUserv2_0(pagedata);
        ProcessUserMessageBoardModeration(Convert.ToInt64(Request.QueryString["id"]));
        UpdatePersonalTags();
        UpdateActivityPreferences();
        UpdatePublishPreferences();
        if (bEmailChanged)
        {
            Response.Redirect("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&OrderBy=" + Request.Form["OrderBy"] + "&groupid=888888", false);
        }
        else
        {
            Response.Redirect((string)("users.aspx?action=view&grouptype=" + Request.QueryString["grouptype"] + "&id=" + CurrentUserID + "&groupid=" + Request.QueryString["groupid"]), false);
        }
    }
    private void ProcessUserMessageBoardModeration(long userId)
    {
        Ektron.Cms.Community.MessageBoardAPI messageboardapi = new Ektron.Cms.Community.MessageBoardAPI();
        if (Page.Request.Form["ek_MsgBoardModerate"] != null || messageboardapi.IsModerated(userId, EkEnumeration.MessageBoardObjectType.User) != false)
        {
            string moderateStatus = Page.Request.Form["ek_MsgBoardModerate"];
            if (moderateStatus == "on")
            {
                messageboardapi.EnableModeration(userId, EkEnumeration.MessageBoardObjectType.User, messageboardapi.RequestInformationRef.UserId);
            }
            else
            {
                messageboardapi.DisableModeration(userId, EkEnumeration.MessageBoardObjectType.User, messageboardapi.RequestInformationRef.UserId);
            }
        }
    }
    private void DeleteGroup()
    {
        m_refUserApi.DeleteUserGroup(Convert.ToInt64(Request.QueryString["GroupID"]));
        Response.Redirect((string)("users.aspx?action=viewallgroups&grouptype=" + Request.QueryString["grouptype"]), false);
    }

    private void UpdateADUsersGroup()
    {
        m_refUserApi.UpdateADUsersGroups(Request.QueryString["username"], Request.QueryString["domain"]);
        Response.Redirect("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["groupid"] + "&id=2&FromUsers=1", false);
    }

    private void UpdateADUser()
    {
        long uID = Convert.ToInt64(Request.QueryString["id"]);
        long GroupID = Convert.ToInt64(Request.QueryString["GroupID"]);
        m_refUserApi.UpdateUsersInfoFromAD(Request.QueryString["username"], Request.QueryString["domain"]);
        Ektron.Cms.UserData userdata = m_refUserApi.GetUserById(uID, false, false);
        m_refUserApi.UpdateADUsersGroups(userdata.Username, userdata.Domain);
        Response.Redirect("users.aspx?action=View&id=" + uID + "&GroupID=" + GroupID + "&grouptype=" + Request.QueryString["grouptype"] + "&FromUsers=&OrderBy=user_name", false);
    }

    private void MapCMSUserToAD()
    {
        long uID = System.Convert.ToInt64(Request.Form["id"]);
        string[] tempArray = Strings.Split(Request.Form["usernameanddomain"], "_@_", -1, 0);
        string strUserName = tempArray[0].ToString();
        string strDomain = tempArray[1].ToString();
        string returnPage = "";
        if (Request.Form["rp"] == "1")
        {
            returnPage = (string)("users.aspx?action=View&id=" + uID);
        }
        else
        {
            returnPage = (string)("adreports.aspx?action=SynchUsers&ReportType=" + Request.Form["rt"]);
        }

        m_refUserApi.MapCMSUserToAD(uID, strUserName, strDomain, 0);
        Response.Redirect(returnPage, false);
    }

    private void MapCMSUserGroupToAD()
    {
        long GroupID = System.Convert.ToInt64(Request.Form["id"]);
        string[] tempArray = Strings.Split(Request.Form["usernameanddomain"], "_@_", -1, 0);
        string GroupName = tempArray[0].ToString();
        string strDomain = tempArray[1].ToString();
        string returnPage = "";
        if (Request.Form["rp"] == "1")
        {
            returnPage = (string)("users.aspx?action=viewallusers&grouptype=" + Request.QueryString["grouptype"] + "&groupid=" + Request.QueryString["groupid"] + "&id=" + GroupID);
        }
        else
        {
            returnPage = (string)("adreports.aspx?action=SynchGroups&ReportType=" + Request.Form["rt"]);
        }
        m_refUserApi.MapCMSUserGroupToAD(GroupID, GroupName, strDomain);
        Response.Redirect(returnPage, false);
    }

    private void EditPrefrence()
    {
        long UserID;
        Collection cPreferences;
        string returnPage;
        UserID = System.Convert.ToInt32(Request.QueryString["uid"]);
        if (UserID == 0)
        {
            returnPage = "configure.aspx";
        }
        else
        {
            returnPage = (string)("users.aspx?action=View&id=" + UserID);
        }
        cPreferences = new Collection();
        cPreferences.Add(Request.Form["txtWidth"], "width", null, null);
        cPreferences.Add(Request.Form["txtHeight"], "height", null, null);
        cPreferences.Add(Request.Form["templatefilename"], "template", null, null);
        cPreferences.Add(Request.Form["folderid"], "folderid", null, null);
        if (Request.Form["forcePrefs"] != "")
        {
            if (Request.Form["forcePrefs"] == "on")
            {
                cPreferences.Add("1", "forcesetting", null, null);
            }
            else
            {
                cPreferences.Add("0", "forcesetting", null, null);
            }
        }
        m_refUserApi.UpdateUserPreferences(UserID, cPreferences);
        Response.Redirect(returnPage, false);
    }

    #region Personal Tags
    public bool UpdatePersonalTags()
    {
        bool returnValue;
        bool result = false;
        TagData[] defaultUserTags;
        TagData[] usersTags;
        TagData td;
        string tagIdStr = "";
        long uId = 0;
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Information.IsNumeric(Request.QueryString["id"]))
            {
                uId = Convert.ToInt64((Request.QueryString["id"]));
            }
            if (uId > 0)
            {

                string orginalTagIds;
                orginalTagIds = Request.Form["currentTags"].Trim().ToLower();

                // Assign all default user tags that are checked:
                // Remove tags that have been unchecked
                defaultUserTags = m_refTagsApi.GetDefaultTags(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User, -1);
                usersTags = m_refTagsApi.GetTagsForUser(uId);

                //Also, copy all users tags into defaultUserTags list
                //so that if they were removed, they can be deleted as well.
                int originalLength = defaultUserTags.Length;
                Array.Resize<TagData>(ref defaultUserTags, defaultUserTags.Length + usersTags.Length);
                usersTags.CopyTo(defaultUserTags, originalLength);

                if (defaultUserTags != null)
                {
                    foreach (TagData tempLoopVar_td in defaultUserTags)
                    {
                        td = tempLoopVar_td;
                        tagIdStr = (string)("userPTagsCbx_" + td.Id.ToString());
                        if (!(Request.Form[tagIdStr] == null))
                        {

                            if (Request.Form[tagIdStr].Trim().ToLower() == "on")
                            {
                                //if tag is checked, but not in current tag list, add it
                                if (!orginalTagIds.Contains(td.Id.ToString() + ","))
                                {
                                    m_refTagsApi.AddTagToUser(td.Id, uId);
                                }
                            }
                            else
                            {
                                //if tag is unchecked AND in current list, delete
                                if (orginalTagIds.Contains(td.Id.ToString() + ","))
                                {
                                    m_refTagsApi.DeleteTagOnObject(td.Id, uId, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User, uId);
                                }
                            }
                        }
                        else
                        {
                            //if tag checkbox has no postback value AND is in current tag list, delete it
                            if (orginalTagIds.Contains(td.Id.ToString() + ","))
                            {
                                m_refTagsApi.DeleteTagOnObject(td.Id, uId, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User, uId);
                            }
                        }
                    }
                }
            }

            // Now add any new custom tags, that the user created:
            // New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>;
            if (!(Request.Form["newTagNameHdn"] == null))
            {
                string custTags = Request.Form["newTagNameHdn"];
                string[] aCustTags = custTags.Split(";".ToCharArray());

                int languageId;

                foreach (string tag in aCustTags)
                {

                    string[] tagPropArray = tag.Split("~".ToCharArray());
                    if (tagPropArray.Length > 1)
                    {
                        if (tagPropArray[0].Trim().Length > 0)
                        {

                            //Default language to -1.
                            //"ALL" option in drop down is 0 - switch to -1.
                            if (!int.TryParse(tagPropArray[1], out languageId))
                            {
                                languageId = -1;
                            }
                            if (languageId == 0)
                            {
                                languageId = -1;
                            }

                            m_refTagsApi.AddTagToUser(tagPropArray[0], uId, languageId);
                        }
                    }
                }
            }
            result = true;
        }
        catch (Exception)
        {
            result = false;
        }
        finally
        {
            returnValue = result;
            defaultUserTags = null;
            td = null;
        }
        return returnValue;
    }

    #endregion

    #region Activity Preferences
    private void UpdateActivityPreferences()
    {
        Ektron.Cms.Activity.ActivityTypeCriteria activityCollListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
        Ektron.Cms.Activity.ActivityTypeCriteria activityGroupListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
        long uId = 0;

        if ((!(Request.QueryString["id"] == null)) && Information.IsNumeric(Request.QueryString["id"]))
        {
            long.TryParse(Request.QueryString["id"], out uId);
        }

        activityCollListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague);
        activityGroupListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);

        collActivityTypeList = _activityListApi.GetList(activityCollListCriteria);
        groupActivityTypeList = _activityListApi.GetList(activityGroupListCriteria);
        if (uId > 0)
        {
            //Colleague Preferences
            preferenceList = new System.Collections.Generic.List<NotificationPreferenceData>();
            preferenceList.Clear();
            for (int i = 0; i <= collActivityTypeList.Count - 1; i++)
            {
                if ((Page.Request.Form["email" + collActivityTypeList[i].Id] != null) && Page.Request.Form["email" + collActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.AgentId = 1;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 1;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["sms" + collActivityTypeList[i].Id] != null) && Page.Request.Form["sms" + collActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.AgentId = 3;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 3;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["feed" + collActivityTypeList[i].Id] != null) && Page.Request.Form["feed" + collActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.AgentId = 2;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = collActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 2;
                    prefData.UserId = uId;
                    preferenceList.Add(prefData);
                }

            }
            //Group Preferences

            for (int i = 0; i <= groupActivityTypeList.Count - 1; i++)
            {

                if ((Page.Request.Form["email" + groupActivityTypeList[i].Id] != null) && Page.Request.Form["email" + groupActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.AgentId = 1;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 1;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["feed" + groupActivityTypeList[i].Id] != null) && Page.Request.Form["feed" + groupActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.AgentId = 2;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 2;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["sms" + groupActivityTypeList[i].Id] != null) && Page.Request.Form["sms" + groupActivityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.AgentId = 3;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = groupActivityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 3;
                    prefData.UserId = uId;
                    prefData.ActionSourceId = -1;
                    preferenceList.Add(prefData);
                }
            }
            _notificationPreferenceApi.SaveUserPreferences(preferenceList);
        }
    }

    #endregion
    #region Publish Preferences
    private void UpdatePublishPreferences() //To Publish Preferences for activities i.e privacy settings.
    {
        long uId = 0;
        Ektron.Cms.Framework.Notifications.NotificationPublishPreference _publishPrefApi = new Ektron.Cms.Framework.Notifications.NotificationPublishPreference();
        System.Collections.Generic.List<NotificationPublishPreferenceData> publishActivityTypeList;
        System.Collections.Generic.List<long> publishPreferenceIdList = new System.Collections.Generic.List<long>();

        if ((!(Request.QueryString["id"] == null)) && Information.IsNumeric(Request.QueryString["id"]))
        {
            long.TryParse(Request.QueryString["id"], out uId);
        }
        publishActivityTypeList = _publishPrefApi.GetList(uId);
        if (uId > 0)
        {
            for (int i = 0; i <= publishActivityTypeList.Count - 1; i++)
            {
                if ((Page.Request.Form["pref" + publishActivityTypeList[i].ActivityTypeId] != null) && Page.Request.Form["pref" + publishActivityTypeList[i].ActivityTypeId] == "on")
                {
                    publishPreferenceIdList.Add(publishActivityTypeList[i].ActivityTypeId);
                }
            }
            _publishPrefApi.Add(uId, publishPreferenceIdList);
        }
    }

    #endregion
    protected void SetServerJSVariables()
    {
        jsContentLanguage.Text = ContentLanguage.ToString();
        jsImageExtensions.Text = lib_setting_data.ImageExtensions;
        jsUsernameReq.Text = m_refMsg.GetMessage("js: alert username required");
        jsFirstnameReq.Text = m_refMsg.GetMessage("js: alert first name required");
        jsLastnameReq.Text = m_refMsg.GetMessage("js: alert last name required");
        jsEmailBlank.Text = m_refMsg.GetMessage("js: alert ad email blank");
        jsEmailAddress.Text = m_refMsg.GetMessage("js: alert enter email address");
        jsEmailInvalid.Text = m_refMsg.GetMessage("js: alert ad email invalid");
        jsValidEmail.Text = m_refMsg.GetMessage("js: alert enter valid email");
        jsValidEmailOrBlank.Text = m_refMsg.GetMessage("enter valid email address or leave blank");
        jsErrDisplayName.Text = m_refMsg.GetMessage("js err display name");
        jsPasswordReq.Text = m_refMsg.GetMessage("js: alert password required");
        jsPasswordInvalid.Text = m_refMsg.GetMessage("js: alert user cannot confirm password");
        jsDeleteUser.Text = m_refMsg.GetMessage("js: confirm delete user");
        jsAddUser.Text = m_refMsg.GetMessage("js: confirm add user");
        jsDelUserFromGroup.Text = m_refMsg.GetMessage("js: confirm delete user from group");
        jsUserGroupNameReq.Text = m_refMsg.GetMessage("js: alert user group name required");
        jsDelUserGroupSubscription.Text = m_refMsg.GetMessage("js: confirm delete user group subscription");
        jsDelUserGroup.Text = m_refMsg.GetMessage("js: confirm delete user group");
        jsMakeSelection.Text = m_refMsg.GetMessage("js: alert make selection");
        jsInvalidPageWidth.Text = m_refMsg.GetMessage("js: page width out of range");
        jsInvalidPageHeight.Text = m_refMsg.GetMessage("js: page height out of range");
        jsEmailNoUserMsg.Text = m_refMsg.GetMessage("email error: No users selected to receive email");
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
    }
}