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
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;

//NOTE id=mapped to uid
public partial class viewusers : System.Web.UI.UserControl
{
    protected LanguageData[] language_data;
    protected UserData user_data;
    protected PermissionData security_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected DomainData[] domain_data;
    protected string UserName = "";
    protected int ContentLanguage = -1;
    protected string FirstName = "";
    protected string LastName = "";
    protected long uId = -1;
    protected SettingsData setting_data;
    protected string OrderBy = "";
    protected string FromUsers = "";
    protected UserGroupData usergroup_data;
    protected UserData[] user_list;
    protected EmailHelper m_refMailMsg = new EmailHelper();
    protected GroupData[] group_list;
    protected long CurrentUserID = -1;
    protected string PageAction = "";
    protected string search = "";
    protected string rp = "";
    protected string e1 = "";
    protected string e2 = "";
    protected string f = "";
    protected string GroupName = "EveryOne";
    protected int m_intGroupType = -1; //0-CMS User; 1-Membership User
    protected long m_intGroupId = -1;
    protected int m_intUserActiveFlag = 0; //0-Active;1-Deleted;-1-Not verified
    protected string m_strDirection = "asc";
    protected string m_strSearchText = "";
    protected string m_strKeyWords = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strPageAction = "";
    protected string m_strSelectedItem = "-1";
    private string m_strBackAction = "viewallgroups";
    private string m_strCallerPage = "";
    private bool m_bCommunityGroup;
    private long m_iCommunityGroup = 0;
    protected StyleHelper _refStyle = new StyleHelper();
    protected Ektron.Cms.Framework.Notifications.NotificationPreference _notificationPreferenceApi = new Ektron.Cms.Framework.Notifications.NotificationPreference();
    protected System.Collections.Generic.List<NotificationPreferenceData> preferenceList;
    protected NotificationPreferenceData prefData = new NotificationPreferenceData();
    protected Ektron.Cms.Framework.Notifications.NotificationAgentSetting _notificationAgentApi = new Ektron.Cms.Framework.Notifications.NotificationAgentSetting();
    protected System.Collections.Generic.List<NotificationAgentData> agentList;
    protected Ektron.Cms.Framework.Activity.Activity _activityApi = new Ektron.Cms.Framework.Activity.Activity();
    protected Ektron.Cms.Framework.Activity.ActivityType _activityListApi = new Ektron.Cms.Framework.Activity.ActivityType();
    protected System.Collections.Generic.List<Ektron.Cms.Activity.ActivityTypeData> activityTypeList;
    protected string groupAliasList = string.Empty;
    protected int fieldId = 0;

    #region Load
    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        RegisterResources();

        workareaTab.Visible = false;
        workareaDiv.Visible = false;

        if (!string.IsNullOrEmpty(Request.QueryString["grouptype"]))
        {
            m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["communitygroup"]))
        {
            m_bCommunityGroup = true;
        }
        if (!string.IsNullOrEmpty(Request.QueryString["groupid"]))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
            if (m_bCommunityGroup)
            {
                m_iCommunityGroup = m_intGroupId;
                m_intGroupId = this.m_refContentApi.EkContentRef.GetCmsGroupForCommunityGroup(m_iCommunityGroup);
            }
        }
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            uId = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            m_strPageAction = Request.QueryString["action"].ToLower();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["backaction"]))
        {
            m_strBackAction = Request.QueryString["backaction"].ToLower();
        }
        if ((!string.IsNullOrEmpty(Request.QueryString["ty"])) && (Request.QueryString["ty"] == "nonverify"))
        {
            m_intUserActiveFlag = -1;
            m_strBackAction = m_strBackAction + "&ty=nonverify";
        }
        m_strDirection = Request.QueryString["direction"];

        if (m_strDirection == "asc")
        {
            m_strDirection = "desc";
        }
        else
        {
            m_strDirection = "asc";
        }

        //VisiblePageControls(False)
        this.uxPaging.Visible = false;

        Utilities.SetLanguage(m_refSiteApi);
        Utilities.SetLanguage(m_refUserApi);
        Utilities.SetLanguage(m_refContentApi);


        m_refMsg = m_refSiteApi.EkMsgRef;
        AppImgPath = m_refSiteApi.AppImgPath;
        AppPath = m_refSiteApi.AppPath;
        ContentLanguage = m_refSiteApi.ContentLanguage;

        if (m_strPageAction == "viewallusers")
        {
            if (!string.IsNullOrEmpty(Request.QueryString["callerpage"]))
            {

                m_strCallerPage = Request.QueryString["callerpage"];

            }
            ViewAllUsers();
        }
    }
    #endregion

    #region VIEW
    public bool View()
    {
        //VisiblePageControls(False)
        this.uxPaging.Visible = false;
        PageAction = "view";
        CurrentUserID = m_refSiteApi.UserId;

        FromUsers = Request.QueryString["FromUsers"];
        bool bPreference = true;
        bool bReturnDeleted = false;
        if (m_intGroupType == 1)
        {
            bPreference = false;
        }
        if (m_intUserActiveFlag == -1)
        {
            bReturnDeleted = true;
        }
        user_data = m_refUserApi.GetUserById(uId, bPreference, bReturnDeleted);
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        ViewToolBar();
        Populate_ViewGrid();
        CreateColumns();
        if (_activityApi.IsActivityPublishingEnabled && (agentList != null) && agentList.Count > 0 && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
        {
            LoadGrid("colleagues");
            LoadGrid("groups");
            ViewUserPublishPreferences();
        }
        else
        {
            EkMembershipActivityTable.Visible = false;
            activitiesTab.Visible = false;
        }
        //community aliasing Tab
        LoadCommunityAliasTab();
        return true;
    }

    private void Populate_ViewGrid()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.ItemStyle.CssClass = "label";
        FormGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VALUE";
        colBound.ItemStyle.CssClass = "readOnlyValue";
        FormGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("username label");
        dr[1] = user_data.Username;
        dt.Rows.Add(dr);

        if (LDAPMembers() && setting_data.ADAuthentication == 1)
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("domain title");
            dr[1] = user_data.Domain;
            dt.Rows.Add(dr);
        }
        else if (LDAPMembers() && setting_data.ADAuthentication == 2)
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("generic path") + ":";
            dr[1] = user_data.Domain;
            dt.Rows.Add(dr);
        }

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic id") + ":";
        dr[1] = user_data.Id.ToString();
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("first name label");
        dr[1] = user_data.FirstName;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("last name label");
        dr[1] = user_data.LastName;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("display name label") + ":";
        dr[1] = user_data.DisplayName;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("user language label");
        if (user_data.LanguageId == 0)
        {
            dr[1] = m_refMsg.GetMessage("app default msg");
        }
        else
        {
            dr[1] = user_data.LanguageName;
        }
        dt.Rows.Add(dr);


        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("email address label");
        if (user_data.Email.Length == 0)
        {
            dr[1] = m_refMsg.GetMessage("none specified msg");
        }
        else
        {
            dr[1] = user_data.Email;
        }
        dt.Rows.Add(dr);
        if ((this.m_refContentApi.RequestInformationRef.LoginAttempts != -1) && ((security_data.IsAdmin) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin)))
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("account locked") + ": ";
            dr[1] = "<input type=\"checkbox\" id=\"accLocked_" + user_data.Id + "\" disabled ";
            if (user_data.IsAccountLocked(this.m_refContentApi.RequestInformationRef))
            {
                dr[1] += " checked ";
            }
            dr[1] += " />";
            dt.Rows.Add(dr);
        }

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl last login date") + ": ";
        dr[1] = user_data.LastLoginDate;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl editor") + ":";
        dr[1] = "<select disabled>";
        //dr(1) &= "<option value=""contentdesigner"">" & m_refMsg.GetMessage("lbl content designer") & "</option>"
        if ((string)(user_data.EditorOption.ToLower().Trim()) == "ewebeditpro")
        {
            //dr(1) &= "<option value=""ewebwp"" >eWebWP</option>"
            //dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
            dr[1] += "<option value=\"ewebeditpro\" selected>eWebEditPro</option>";
            //Case "ewebwp"
            //    dr(1) &= "<option value=""ewebwp"" selected>eWebWP</option>"
            //    dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
            //    dr(1) &= "<option value=""ewebeditpro"" >eWebEditPro</option>"
        }
        else if ((string)(user_data.EditorOption.ToLower().Trim()) == "jseditor")
        {
            //    dr(1) &= "<option value=""jseditor"" selected>" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
            //    dr(1) &= "<option value=""ewebeditpro"">eWebEditPro</option>"
        } // "jseditor" or "ewebwp"
        else
        {
            //dr(1) &= "<option value=""ewebwp"">eWebWP</option>"
            dr[1] += "<option value=\"contentdesigner\" selected>" + m_refMsg.GetMessage("lbl content designer") + "</option>";
            //dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
            dr[1] += "<option value=\"ewebeditpro\">eWebEditPro</option>";
        }
        dr[1] += "</select>";
        dt.Rows.Add(dr);

        if (m_intGroupType == 0)
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl workflow and task email");
            if (user_data.IsDisableMessage)
            {
                dr[1] = m_refMsg.GetMessage("email disabled msg");
            }
            else
            {
                dr[1] = m_refMsg.GetMessage("email enabled msg");
            }

            if (security_data.IsAdmin && setting_data.EnableMessaging == false)
            {
                dr[1] += "<br /><label class=\"ektronCaption\">" + m_refMsg.GetMessage("application emails disabled msg") + "</label>"; //application emails disabled msg
            }

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl avatar") + ":";
            dr[1] = user_data.Avatar.Length > 0 ? ("<a href=\"" + EkFunctions.HtmlEncode(user_data.Avatar) + "\" target=\"_blank\">" + EkFunctions.HtmlEncode(user_data.Avatar) + "</a>") : "";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl user add address")+":";
            dr[1] = user_data.Address;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl user add latitude")+":";
            dr[1] = user_data.Latitude;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl user add longitude")+":";
            dr[1] = user_data.Longitude;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl signature") + ":";
            dr[1] = user_data.Signature;
            dt.Rows.Add(dr);

            // Personal Tags:
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl personal tags") + ":";
            dr[1] = GetPersonalTags();
            dt.Rows.Add(dr);

            if (!(user_data.UserPreference == null))
            {
                if (user_data.UserPreference.ForceSetting)
                {
                    dr = dt.NewRow();
                    dr[0] = "Preferences are locked by the CMS.";
                    dr[1] = "important"; //class=important
                    dt.Rows.Add(dr);
                }
            }

            Display_WorkPage();
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl avatar") + ":";
            dr[1] = user_data.Avatar.Length > 0 ? ("<a href=\"" + EkFunctions.HtmlEncode(user_data.Avatar) + "\" target=\"_blank\">" + EkFunctions.HtmlEncode(user_data.Avatar) + "</a>") : "";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl signature") + ":";
            dr[1] = user_data.Signature;
            dt.Rows.Add(dr);

            // Personal Tags:
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl personal tags") + ":";
            dr[1] = GetPersonalTags();
            dt.Rows.Add(dr);
        }

        Display_Groups();

        DataView dv = new DataView(dt);
        FormGrid.DataSource = dv;
        FormGrid.DataBind();

        Display_CustomProperties();

        //ViewUser().Visible = true;
        viewUser.Visible = true;
    }

    private void Display_Groups()
    {
        Ektron.Cms.User.IUserGroup userGroupManager = Ektron.Cms.ObjectFactory.GetUserGroup();
        Ektron.Cms.UserGroupCriteria criteria = new UserGroupCriteria();
        
        criteria.PagingInfo = new PagingInfo(m_refContentApi.RequestInformationRef.PagingSize, uxUserGroupsGrid.CurrentPage + 1);
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        criteria.OrderByField = UserGroupProperty.Name;

        uxUserGroupsGrid.AddColumn(
            this.m_refMsg.GetMessage("generic name")
            , "[GroupName]");

        uxUserGroupsGrid.Bind(
            userGroupManager.GetListForUser(uId, criteria)
            , criteria.PagingInfo);
    }

    private void Display_WorkPage()
    {
        workareaTab.Visible = true;
        workareaDiv.Visible = true;

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.ItemStyle.CssClass = "label";
        WorkPage.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VALUE";
        colBound.ItemStyle.CssClass = "readOnlyValue";
        WorkPage.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl fullscreen") + ":";
        if (user_data.UserPreference.Height == 9999 && user_data.UserPreference.Width == 9999)
        {
            dr[1] = "<input type=\"checkbox\" disabled=\"disabled\" id=\"chkFullScreen\" name=\"chkFullScreen\" checked=\"on\" />";
        }
        else
        {
            dr[1] = "<input type=\"checkbox\" disabled=\"disabled\" id=\"chkFullScreen\" name=\"chkFullScreen\" />";
        }
        dt.Rows.Add(dr);

        if (user_data.UserPreference.Height != 9999 && user_data.UserPreference.Width != 9999)
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl Width") + ":";
            dr[1] = user_data.UserPreference.Width + "px";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl height") + ":";
            dr[1] = user_data.UserPreference.Height + "px";
            dt.Rows.Add(dr);
        }

        //dr = dt.NewRow();
        //dr[0] += m_refMsg.GetMessage("lbl display button text in the title bar") + ":";
        //dr[1] = "<input type=\"checkbox\" id=\"chkDispTitleText\" disabled";
        //if (user_data.UserPreference.DisplayTitleText == "1")
        //{
        //    dr[1] += " checked";
        //}
        //dr[1] += " name=\"chkDispTitleText\">";
        //dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl Landing Page after login") + ":";
        if (user_data.UserPreference.Template == "")
        {
            dr[1] = m_refMsg.GetMessage("refresh login page msg");
        }
        else
        {
            dr[1] = m_refSiteApi.SitePath + user_data.UserPreference.Template;
        }
        dt.Rows.Add(dr);

        dr = dt.NewRow();

        dr[0] = m_refMsg.GetMessage("alt set smart desktop as the start location in the workarea") + ":";
        dr[1] = "<input type=\"checkbox\" disabled";
        if (user_data.UserPreference.FolderId == "")
        {
            dr[1] += " checked";
        }
        dr[1] += "   id=\"checkbox\" name=\"chkSmartDesktop\"> ";

        dt.Rows.Add(dr);
        DataView dv = new DataView(dt);
        WorkPage.DataSource = dv;
        WorkPage.DataBind();
    }
    protected void Display_CustomProperties()
    {
        string strHtml = "";
        strHtml = m_refUserApi.EditUserCustomProperties(uId, true);
        StringBuilder sBuilder = new StringBuilder();
        Ektron.Cms.Community.MessageBoardAPI messageboardapi = new Ektron.Cms.Community.MessageBoardAPI();
        sBuilder.Append(strHtml);
        sBuilder.Append("<tr></tr><tr><td><div id=\"ek_MsgBoardModerationLabel\"><label class=\"label\">" + m_refMsg.GetMessage("lbl perm moderate") + ":" + "</label></div></td>\n");
        if (messageboardapi.IsModerated(uId, EkEnumeration.MessageBoardObjectType.User))
        {
            sBuilder.Append("<td><div id=\"ek_MsgBoardModeration\"><input disabled=\"true\" type=\"checkbox\" id=\"ek_MsgBoardModerate\" name = \"ek_MsgBoardModerate\" checked=\"checked\"/>" + m_refMsg.GetMessage("lbl msgboard") + "<br/><span>" + m_refMsg.GetMessage("lbl usermsgboardnotify") + "</span></div></td></tr> \n");
        }
        else
        {
            sBuilder.Append("<td><div id=\"ek_MsgBoardModeration\"><input disabled=\"true\" type=\"checkbox\" id=\"ek_MsgBoardModerate\"  name = \"ek_MsgBoardModerate\"/>" + m_refMsg.GetMessage("lbl msgboard") + "<br/><span>" + m_refMsg.GetMessage("lbl usermsgboardnotify") + "</span></div></td></tr> \n");
        }
        ltr_CustomProperty.Text = sBuilder.ToString();
    }

    public string GetPersonalTags()
    {
        string returnValue;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        TagData[] tdaUser;
        TagData td;
        Hashtable htTagsAssignedToUser;

        try
        {
            htTagsAssignedToUser = new Hashtable();
            result.Append("<div>");
            if (uId > 0)
            {

                LocalizationAPI localizationApi = new LocalizationAPI();
                tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForUser(uId, -1);

                if (tdaUser != null)
                {
                    foreach (TagData tempLoopVar_td in tdaUser)
                    {
                        td = tempLoopVar_td;
                        result.Append("<input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                        result.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' border=\"0\" />");
                        result.Append("&#160;" + td.Text + "<br />");
                    }
                }

            }
            result.Append("</div>");

        }
        catch (Exception)
        {
        }
        finally
        {
            returnValue = result.ToString();
            tdaUser = null;
            td = null;
            htTagsAssignedToUser = null;
        }
        return returnValue;
    }

    public void ViewToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string tempTy;
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("view user information msg") + " \"" + user_data.DisplayUserName + "\""));
        result.Append("<table><tr>");

		bool showAdminTools = security_data.IsAdmin || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers, CurrentUserID, false) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin, CurrentUserID, false);

		if (showAdminTools)
		{
			if (m_intGroupId == 0 && (Request.QueryString["callbackpage"] != null))
			{
				if ((Request.QueryString["callbackpage"] != null) && (Request.QueryString["folderid"] != null) && (Request.QueryString["taxonomyid"] != null) && (Request.QueryString["parentid"] != null))
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)(Request.QueryString["callbackpage"] + "&view=user&folderid=" + Request.QueryString["folderid"] + "&taxonomyid=" + Request.QueryString["taxonomyid"] + "&parentid=" + Request.QueryString["parentid"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("users.aspx?action=viewallusers&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + Request.QueryString["GroupID"] + "&FromUsers=" + FromUsers), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
		}

		bool primaryStyleApplied = false;

        if (m_intUserActiveFlag != -1)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", (string)("users.aspx?action=EditUser&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + uId + "&FromUsers=" + FromUsers), (string)(m_refMsg.GetMessage("alt edit button text (user)") + " " + user_data.FirstName + " " + user_data.LastName + ""), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));

			primaryStyleApplied = true;
        }

		if (showAdminTools)
        {
            if (m_intUserActiveFlag == -1)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/approvals.png", (string)("users.aspx?action=activateuseraccount&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + uId + "&FromUsers=" + FromUsers), "Click here to activate the user.", "Activate", "onclick=\"return ConfirmActivateUser();\"", StyleHelper.ApprovalsButtonCssClass, !primaryStyleApplied));

				primaryStyleApplied = true;
            }

            if (Request.QueryString["GroupID"] == "2" || Request.QueryString["GroupID"] == "888888")
            {
                if (uId != CurrentUserID)
                {
                    if ((!(Request.QueryString["ty"] == null)) && (Request.QueryString["ty"] == "nonverify"))
                    {
                        tempTy = "&ty=nonverify";
                    }
                    else
                    {
                        tempTy = "";
                    }
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", (string)("users.aspx?action=DeleteUserFromSystem&groupid=" + m_intGroupId + tempTy + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + uId + "&OrderBy=" + Request.QueryString["OrderBy"] + "&FromUsers=" + FromUsers), m_refMsg.GetMessage("alt delete button text (user)"), m_refMsg.GetMessage("btn delete"), "onclick=\"return ConfirmDeleteUser();\"", StyleHelper.DeleteButtonCssClass, !primaryStyleApplied));

					primaryStyleApplied = true;
                }
            }
            else
            {
                if (uId != CurrentUserID)
                {
                    string strUserParam = "action=DeleteUserFromGroup";
                    if (m_intUserActiveFlag == -1)
                    {
                        strUserParam = "action=deleteuserfromsystem&ty=nonverify";
                    }
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", (string)("users.aspx?" + strUserParam + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + uId + "&OrderBy=" + Request.QueryString["OrderBy"] + "&FromUsers=" + FromUsers), m_refMsg.GetMessage("alt delete button text (user2)"), m_refMsg.GetMessage("btn delete"), "onclick=\"return ConfirmDeleteUserFromGroup();\"", StyleHelper.DeleteButtonCssClass, !primaryStyleApplied));

					primaryStyleApplied = true;
                }
            }
            if ((setting_data.ADIntegration) && (user_data.Domain != ""))
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/refresh.png", (string)("users.aspx?action=UpdateADUser&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + user_data.Id + "&username=" + user_data.Username + "&domain=" + user_data.Domain + "&FromUsers=" + FromUsers), "Refresh", m_refMsg.GetMessage("btn refresh"), "", StyleHelper.RefreshButtonCssClass, !primaryStyleApplied));

				primaryStyleApplied = true;
            }
            if (setting_data.ADAuthentication == 1)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", (string)("users.aspx?action=MapCMSUserToAD&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + user_data.Id + "&rp=1&FromUsers=" + FromUsers), m_refMsg.GetMessage("alt browse button text (user)"), m_refMsg.GetMessage("alt browse button text (user)"), "", StyleHelper.BrowseButtonCssClass, !primaryStyleApplied));

				primaryStyleApplied = true;
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        if (m_intGroupType == 0)
        {
            result.Append(m_refStyle.GetHelpButton("viewusers_ascx", ""));
        }
        else
        {
            result.Append(m_refStyle.GetHelpButton("ViewMembershipUser", ""));
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion

    #region ViewUsers
    public void ViewAllUsers()
    {
        if (Page.IsPostBack && Request.Form[isPostData.UniqueID] != "")
        {
            if (Request.Form[isSearchPostData.UniqueID] != "")
            {
                CollectSearchText();
                DisplayUsers();
            }
            else
            {

                if (m_intUserActiveFlag == -1)
                {
                    Ektron.Cms.User.EkUser objUser;
                    objUser = m_refSiteApi.EkUserRef;
                    // Paging link for Users Not Verified.
                    if (Request.Form["req_deleted_users"] == null)
                    {
                        DisplayUsers();
                    }
                    else
                    {
                        if (Request.Form[isDeleted.UniqueID] != "")
                        {
                            objUser.DeleteMembershipUsers(Request.Form["req_deleted_users"]);
                        }
                        else
                        {
                            objUser.ActivateUserAccounts(Request.Form["req_deleted_users"]);
                        }
                        Response.Redirect((string)("users.aspx?ty=nonverify&action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&OrderBy=" + Request.QueryString["OrderBy"]), false);
                    }
                }
                else
                {
                    if (Request.Form[isDeleted.UniqueID] != "")
                    {
                        m_refUserApi.DeleteUserByIds(Request.Form["req_deleted_users"]);
                        //after delete do a full postback to recalculate #TotalPages
                        Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&OrderBy=" + Request.QueryString["OrderBy"]), false);
                    }
                    else
                    {
                        //Page link selected
                        DisplayUsers();
                    }
                }
            }
        }
        else if (IsPostBack == false)
        {
            DisplayUsers();
        }
        isPostData.Value = "true";
    }
    private void CollectSearchText()
    {
        m_strKeyWords = Request.Form["txtSearch"];
        m_strSelectedItem = Request.Form["searchlist"];
        if (m_strSelectedItem == "-1")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR user_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "last_name")
        {
            m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "first_name")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "user_name")
        {
            m_strSearchText = " (user_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
    }
    private string Quote(string KeyWords)
    {
        string result = KeyWords;
        if (KeyWords.Length > 0)
        {
            result = KeyWords.Replace("\'", "\'\'");
        }
        return result;
    }

    private void DisplayUsers()
    {
        UserRequestData req = new UserRequestData();
        if (Request.QueryString["OrderBy"] == "" || Request.QueryString["OrderBy"] == string.Empty || Request.QueryString["OrderBy"] == null)
        {
            OrderBy = "user_name";
        }
        else
        {
            OrderBy = Request.QueryString["OrderBy"];
        }
        if (m_intGroupId == 888888)
        {
            GroupName = "All_Members";
        }
        if (m_intGroupId != 888888 || m_intGroupId != 2)
        {
            usergroup_data = m_refUserApi.GetUserGroupById(m_intGroupId);
            if (!(usergroup_data == null))
            {
                GroupName = usergroup_data.GroupName;
            }
        }

        ltr_groupsubscription.Text = m_refUserApi.EkUserRef.IsGroupPartOfSubscriptionProduct(m_intGroupId).ToString().ToLower();

        m_intCurrentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage);

        req.Type = System.Convert.ToInt32(m_intGroupType == 3 ? 0 : m_intGroupType);
        req.Group = m_intGroupId;
        req.RequiredFlag = m_intUserActiveFlag;
        req.OrderBy = OrderBy;
        req.OrderDirection = m_strDirection;
        req.SearchText = m_strSearchText;
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        req.CurrentPage = m_intCurrentPage + 1;
        user_list = m_refUserApi.GetAllUsers(ref req);
        m_intTotalPages = req.TotalPages;
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        ViewAllUsersToolBar();
        if (this.m_bCommunityGroup)
        {
            Populate_ViewCommunityMembersGrid(user_list);
        }
        else
        {
            Populate_ViewAllUsersGrid(user_list);
        }
    }
    private void Populate_ViewCommunityMembersGrid(UserData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;
        string sAppend = "";
        string m_strTyAction = "";

        if ((!(Request.QueryString["ty"] == null)) && (Request.QueryString["ty"] == "nonverify"))
        {
            m_strTyAction = "&ty=nonverify";
        }
        string HeaderText = "<a href=\"users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&direction=" + m_strDirection + "&OrderBy={0}&LangType=" + ContentLanguage + "&id=" + uId + ((FromUsers == "" ? "" : ("&FromUsers=" + FromUsers))) + m_strTyAction + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">{1}</a>";

        if (m_intUserActiveFlag == -1 || this.m_bCommunityGroup)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "CHECKL";
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.HeaderStyle.Width = Unit.Percentage(5);
            colBound.ItemStyle.Width = Unit.Percentage(5);
            MapCMSUserToADGrid.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LEFT";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        MapCMSUserToADGrid.Columns.Add(colBound);

        if (m_intUserActiveFlag == -1 || this.m_bCommunityGroup)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "CHECKR";
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.Width = Unit.Percentage(5);
            MapCMSUserToADGrid.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RIGHT";
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        if (m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888 || this.m_bCommunityGroup)
        {
            dt.Columns.Add(new DataColumn("CHECKL", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("LEFT", typeof(string)));
        if (m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888 || this.m_bCommunityGroup)
        {
            dt.Columns.Add(new DataColumn("CHECKR", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("RIGHT", typeof(string)));
        int i = 0;
        if (!(data == null))
        {
            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                sAppend = "";
                if ((setting_data.ADAuthentication == 1) && (data[i].Domain != ""))
                {
                    sAppend = "@" + data[i].Domain;
                }
                if (m_intUserActiveFlag == -1 || this.m_bCommunityGroup)
                {
                    dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].Id + "\" onclick=\"checkAll(\'req_deleted_users\');\">";
                }
                dr["LEFT"] = "<img align=\"left\" src=\"" + ((data[i].Avatar != "") ? (data[i].Avatar) : this.m_refContentApi.AppPath + "images/UI/Icons/user.png") + "\" />" + data[i].DisplayName;
                if (i < (data.Length - 1))
                {
                    i++;
                    sAppend = "";
                    if ((setting_data.ADAuthentication == 1) && (data[i].Domain != ""))
                    {
                        sAppend = "@" + data[i].Domain;
                    }
                    if (m_intUserActiveFlag == -1 || this.m_bCommunityGroup)
                    {
                        dr["CHECKR"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].Id + "\" onclick=\"checkAll(\'req_deleted_users\');\">";
                    }
                    dr["RIGHT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppPath + "images/UI/Icons/user.png\"/>" + data[i].DisplayName;

                }
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        MapCMSUserToADGrid.PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        MapCMSUserToADGrid.DataSource = dv;
        MapCMSUserToADGrid.CurrentPageIndex = m_intCurrentPage;
        MapCMSUserToADGrid.DataBind();

        if (m_intTotalPages > 1)
        {
            this.uxPaging.Visible = true;
            this.uxPaging.TotalPages = m_intTotalPages;
            this.uxPaging.CurrentPageIndex = m_intCurrentPage;
        }
        else
        {
            this.uxPaging.Visible = false;
        }

    }
    private void Populate_ViewAllUsersGrid(UserData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;
        string sAppend = "";
        string Icon = "user.png";
        string m_strTyAction = "";
        if (m_intGroupType == 1)
        {
            Icon = "userMembership.png";
        }
        if ((!(Request.QueryString["ty"] == null)) && (Request.QueryString["ty"] == "nonverify"))
        {
            m_strTyAction = "&ty=nonverify";
        }
        string HeaderText = "<a href=\"users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&direction=" + m_strDirection + "&OrderBy={0}&LangType=" + ContentLanguage + "&id=" + uId + ((FromUsers == "" ? "" : ("&FromUsers=" + FromUsers))) + m_strTyAction + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">{1}</a>";

        if (m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888 || this.m_bCommunityGroup)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "CHECK";
            colBound.HeaderText = "<input type=checkbox name=checkall id=checkall onclick=\"checkAll(\'\');\">";
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.Width = Unit.Percentage(5);
            colBound.ItemStyle.Width = Unit.Percentage(5);
            MapCMSUserToADGrid.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = string.Format(HeaderText, "user_name", m_refMsg.GetMessage("generic Username"));
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(23);
        colBound.ItemStyle.Width = Unit.Percentage(23);
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = string.Format(HeaderText, "last_name", m_refMsg.GetMessage("generic lastname"));
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(23);
        colBound.ItemStyle.Width = Unit.Percentage(23);
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FirstName";
        colBound.HeaderText = string.Format(HeaderText, "first_name", m_refMsg.GetMessage("generic firstname"));
        colBound.HeaderStyle.Width = Unit.Percentage(23);
        colBound.ItemStyle.Width = Unit.Percentage(23);
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        colBound.HeaderText = m_refMsg.GetMessage("generic Language"); //String.Format(HeaderText, "language", m_refMsg.GetMessage("generic Language"))
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTLOGINDATE";
        colBound.HeaderText = m_refMsg.GetMessage("generic lastlogindate"); //String.Format(HeaderText, "last_login_date", m_refMsg.GetMessage("generic lastlogindate"))
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        if (m_intGroupType == 1)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "DATECREATED";
            colBound.HeaderText = m_refMsg.GetMessage("generic datecreated"); //String.Format(HeaderText, "date_created", m_refMsg.GetMessage("generic datecreated"))
            colBound.ItemStyle.Wrap = false;
            MapCMSUserToADGrid.Columns.Add(colBound);
        }
        else
        {
            if (m_refMailMsg.IsLoggedInUsersEmailValid())
            {
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "EMAILAREA";
                colBound.HeaderText = (string)("<a href=\"#\" onclick=\"ToggleEmailCheckboxes();\" title=\"" + m_refMsg.GetMessage("alt send email to all") + "\"><input type=\"checkbox\"></a>&nbsp;" + m_refMsg.GetMessage("generic all"));
                colBound.ItemStyle.Wrap = false;
                MapCMSUserToADGrid.Columns.Add(colBound);
            }
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ACCOUNTLOCK";
        colBound.HeaderText = m_refMsg.GetMessage("generic locked"); //String.Format(HeaderText, "last_login_date", m_refMsg.GetMessage("generic lastlogindate"))
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        if (m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888 || this.m_bCommunityGroup)
        {
            dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTLOGINDATE", typeof(string)));
        if (m_intGroupType == 1)
        {
            dt.Columns.Add(new DataColumn("DATECREATED", typeof(string)));
        }
        else
        {
            if (m_refMailMsg.IsLoggedInUsersEmailValid())
            {
                dt.Columns.Add(new DataColumn("EMAILAREA", typeof(string)));
            }
        }
        dt.Columns.Add(new DataColumn("ACCOUNTLOCK", typeof(string)));
        int i = 0;
        if (!(data == null))
        {
            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                sAppend = "";
                if ((setting_data.ADAuthentication == 1) && (data[i].Domain != ""))
                {
                    sAppend = "@" + data[i].Domain;
                }
                if ((m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888 || this.m_bCommunityGroup) && (!(data[i].Id == m_refUserApi.UserId || data[i].Id == 1 || data[i].Id == 999999999)))
                {
                    dr["CHECK"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].Id + "\" onclick=\"checkAll(\'req_deleted_users\');\">";
                }

                string AltText = "";
                if (data[i].Domain != "")
                {
                    AltText = m_refMsg.GetMessage("view information on msg") + " " + data[i].Username + "@" + data[i].Domain;
                }
                else
                {
                    AltText = m_refMsg.GetMessage("view information on msg") + " " + data[i].DisplayUserName;
                }

                if (m_intUserActiveFlag == -1)
                {
                    dr["USERNAME"] = "<a href=\"users.aspx?action=View&ty=nonverify&LangType=" + ContentLanguage + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&id=" + data[i].Id + "&FromUsers=" + FromUsers + "&OrderBy=" + OrderBy + "\" title=\"" + AltText + "\"><img src=\"" + AppPath + "images/UI/Icons/" + Icon + "\" border=\"0\" align=\"absbottom\" title=\"" + AltText + "\" alt=\"" + AltText + "\"></a> <a href=\"users.aspx?action=View&ty=nonverify&LangType=" + ContentLanguage + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&id=" + data[i].Id + "&FromUsers=" + FromUsers + "&OrderBy=" + OrderBy + "\" title=\"" + AltText + "\">" + data[i].Username + sAppend + "</a>";
                }
                else
                {
                    dr["USERNAME"] = "<a href=\"users.aspx?action=View&LangType=" + ContentLanguage + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&id=" + data[i].Id + "&FromUsers=" + FromUsers + "&OrderBy=" + OrderBy + "\" title=\"" + AltText + "\"><img src=\"" + AppPath + "images/UI/Icons/" + Icon + "\" border=\"0\" align=\"absbottom\" title=\"" + AltText + "\" alt=\"" + AltText + "\"></a> <a href=\"users.aspx?action=View&LangType=" + ContentLanguage + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&id=" + data[i].Id + "&FromUsers=" + FromUsers + "&OrderBy=" + OrderBy + "\" title=\"" + AltText + "\">" + data[i].Username + sAppend + "</a>";
                }

                dr["LASTNAME"] = data[i].LastName;
                dr["FIRSTNAME"] = data[i].FirstName;
                if (data[i].LanguageId == 0)
                {
                    dr["LANGUAGE"] = m_refMsg.GetMessage("app default msg");
                }
                else
                {
                    dr["LANGUAGE"] = data[i].LanguageName;
                }
                dr["LASTLOGINDATE"] = data[i].LastLoginDate;
                if (m_intGroupType == 1)
                {
                    dr["DATECREATED"] = data[i].DateCreated;
                }
                else
                {
                    if (m_refMailMsg.IsLoggedInUsersEmailValid())
                    {
                        dr["EMAILAREA"] = "<input type=\"checkbox\" name=\"emailcheckbox_" + data[i].Id + "\" ID=\"EmailTargetCheckboxes\">";
                        dr["EMAILAREA"] += "<a href=\"#\" onclick=\"SelectEmail(\'emailcheckbox_" + data[i].Id + "\');return false\">";
                        dr["EMAILAREA"] += m_refMailMsg.MakeEmailGraphic() + "</a>";
                    }
                }
                dr["ACCOUNTLOCK"] = "<input type=\"checkbox\" name=\"accLocked_" + data[i].Id + "\" ID=\"accLocked_" + data[i].Id + "\" disabled ";
                if (data[i].IsAccountLocked(this.m_refContentApi.RequestInformationRef))
                {
                    dr["ACCOUNTLOCK"] += " checked ";
                }
                dr["ACCOUNTLOCK"] += " >";
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        MapCMSUserToADGrid.PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        MapCMSUserToADGrid.DataSource = dv;
        MapCMSUserToADGrid.CurrentPageIndex = m_intCurrentPage;
        MapCMSUserToADGrid.DataBind();
        if (m_intTotalPages > 1)
        {
            this.uxPaging.Visible = true;
            this.uxPaging.TotalPages = m_intTotalPages;
            this.uxPaging.CurrentPageIndex = m_intCurrentPage;
        }
        else
        {
            this.uxPaging.Visible = false;
        }


    }

    private void ViewAllUsersToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (m_intUserActiveFlag == -1)
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("view non verified users in group msg") + " \"" + GroupName + "\""));
        }
        else
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("view users in group msg") + " \"" + GroupName + "\""));
        }

        result.Append("<table width=\"100%\"><tr>");

		if (m_strCallerPage != "")
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", m_strCallerPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			if (this.m_bCommunityGroup)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("Community/groups.aspx?action=viewgroup&id=" + this.m_iCommunityGroup + "&LangType=" + this.ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else if ((!(m_intGroupType == 0 && m_intGroupId == 2 && Request.QueryString["FromUsers"] == "1")) && (Request.QueryString["backaction"] != Request.QueryString["action"]))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("users.aspx?action=" + m_strBackAction + "&backaction=" + m_strBackAction + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
		}

		bool primaryStyleApplied = false;
        string buttonId = "";
        if (m_intGroupType == 0) // cms authors
        {
            if (m_intGroupId > 2)
            {
                if (!setting_data.ADAutoUserToGroup)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "users.aspx?action=EditUserGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&Groupid=" + uId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt edit button text (user group)"), m_refMsg.GetMessage("btn edit"), ""));
                }
                else
                {
                    buttonId = Guid.NewGuid().ToString();
                    result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + m_refMsg.GetMessage("lbl Action") + "</span></td>");
                    result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
                    result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
                    result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + AppImgPath + "btn_adbrowse-nm.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("alt browse button text (group)") + "\", function() { window.location.href = \'users.aspx?action=MapCMSUserGroupToAD&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&rp=1\'; } );" + Environment.NewLine);
                    result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
                    result.Append("    </script>" + Environment.NewLine);
                }

				primaryStyleApplied = true;

                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "users.aspx?action=DeleteGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&Groupid=" + uId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt delete button text (user group)"), m_refMsg.GetMessage("btn delete"), "onclick=\"return VerifyDeleteGroup();\"", StyleHelper.DeleteButtonCssClass));
            }
            else if (m_intGroupId == 2)
            {
                if (setting_data.ADAutoUserToGroup && setting_data.ADIntegration == true && this.m_refContentApi.RequestInformationRef.LDAPMembershipUser == true)
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", "AD/adsearch.aspx", m_refMsg.GetMessage("search ad for cms user"), m_refMsg.GetMessage("search ad for cms user"), "", StyleHelper.AddBrowseButtonCssClass, !primaryStyleApplied));
                }
                else
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", (string)("users.aspx?action=AddUserToSystem&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&OrderBy=" + OrderBy + "&FromUsers=" + Request.QueryString["FromUsers"]), m_refMsg.GetMessage("alt add button text (user3)"), m_refMsg.GetMessage("btn add user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                }

				primaryStyleApplied = true;
            }
            else
            {
                if (!setting_data.ADAutoUserToGroup)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                }
                else
                {
                    buttonId = Guid.NewGuid().ToString();
                    result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + m_refMsg.GetMessage("lbl Action") + "</span></td>");
                    result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
                    result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
                    result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + AppImgPath + "btn_adbrowse-nm.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("alt browse button text (group)") + "\", function() { window.location.href = \'users.aspx?action=MapCMSUserGroupToAD&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&rp=1\'; } );" + Environment.NewLine);
                    result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
                    result.Append("    </script>" + Environment.NewLine);
                    result.Append("" + Environment.NewLine);                    
                }

				primaryStyleApplied = true;
            }

            if ((new EmailHelper()).IsLoggedInUsersEmailValid())
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected users"), m_refMsg.GetMessage("btn email"), "onclick=\"LoadEmailChildPageEx();\"", StyleHelper.EmailButtonCssClass, !primaryStyleApplied));

				primaryStyleApplied = true;
            }
        }
        else if (this.m_bCommunityGroup && this.m_iCommunityGroup > 0) // community group
        {
            if (setting_data.ADIntegration == false)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&id=" + uId + "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), "", StyleHelper.AddBrowseButtonCssClass, !primaryStyleApplied));
            }

			primaryStyleApplied = true;

            if (setting_data.ADAutoUserToGroup && setting_data.ADIntegration == true && this.m_refContentApi.RequestInformationRef.LDAPMembershipUser == true)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&groupid=" + usergroup_data.GroupId + "&grouptype=" + m_intGroupType + "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""));
            }
            else
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&id=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), ""));
            }

            if ((new EmailHelper()).IsLoggedInUsersEmailValid())
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected users"), m_refMsg.GetMessage("btn email"), "onclick=\"LoadEmailChildPageEx();\""));
            }
        }
        else // members
        {
            if (m_intUserActiveFlag != -1)
            {
                if (m_intGroupId != 888888)
                {
                    if (setting_data.ADAutoUserToGroup && setting_data.ADIntegration == true && this.m_refContentApi.RequestInformationRef.LDAPMembershipUser == true)
                    {
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&groupid=" + usergroup_data.GroupId + "&grouptype=" + m_intGroupType + "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("alt browse button text (group)"), ""));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&id=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "users.aspx?action=EditUserGroup&Groupid=" + uId + "&grouptype=" + m_intGroupType + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt edit button text (user group)"), m_refMsg.GetMessage("btn edit"), ""));
                    }

					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "users.aspx?action=DeleteGroup&Groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt delete button text (user group)"), m_refMsg.GetMessage("btn delete"), "onclick=\" return VerifyDeleteGroup();\"", StyleHelper.DeleteButtonCssClass));
                }
                else if (m_intGroupId == 888888)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToSystem&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user3)"), m_refMsg.GetMessage("btn add membership user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                }
                else
                {
                    if (setting_data.ADIntegration == false)
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&OrderBy=" + OrderBy + "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), "", StyleHelper.AddButtonCssClass, !primaryStyleApplied));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" + ContentLanguage + "&grouptype=" + m_intGroupType + "&groupid=" + usergroup_data.GroupId + "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), "", StyleHelper.AddBrowseButtonCssClass, !primaryStyleApplied));
                    }
                }
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/approvals.png", "#", m_refMsg.GetMessage("alt activate users"), m_refMsg.GetMessage("lbl activate users"), "onclick=\"ActivateUsers();\"", StyleHelper.ApproveButtonCssClass, !primaryStyleApplied));
                //result.Append(m_refStyle.GetButtonEventsWCaption(apppath & "images/UI/Icons/delete.png", "#", "click here to delete selected users", m_refMsg.GetMessage("btn save"), "onclick=""DeleteSelectedUsers();"""))
            }

			primaryStyleApplied = true;
        }
        if ((!this.m_bCommunityGroup) && (m_intUserActiveFlag == -1 || m_intGroupId == 2 || m_intGroupId == 888888))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt click here to delete selected users"), m_refMsg.GetMessage("btn delete"), "onclick=\"DeleteSelectedUsers();\"", StyleHelper.DeleteButtonCssClass));
        }
        
        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td width=\"100%\" align=\"right\"><label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label><input type=text class=\"ektronTextMedium\" id=txtSearch name=txtSearch value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\" />");
        result.Append(" <select id=searchlist name=searchlist>");
        result.Append("     <option value=-1" + IsSelected("-1") + ">" + m_refMsg.GetMessage("generic all") + "</option>");
        result.Append("     <option value=\"last_name\"" + IsSelected("last_name") + ">" + m_refMsg.GetMessage("generic lastname") + "</option>");
        result.Append("     <option value=\"first_name\"" + IsSelected("first_name") + ">" + m_refMsg.GetMessage("generic firstname") + "</option>");
        result.Append("     <option value=\"user_name\"" + IsSelected("user_name") + ">" + m_refMsg.GetMessage("generic username") + "</option>");
        result.Append(" </select>");
        result.Append(" <input type=button value=" + m_refMsg.GetMessage("btn search") + " id=btnSearch name=btnSearch onclick=\"searchuser();\" class=\"ektronWorkareaSearch\" title=\"" + m_refMsg.GetMessage("lbl Search Users") + "\" />");

		result.Append("</td>");

		result.Append(StyleHelper.ActionBarDivider);

		result.Append("<td>");

        //Help
        if (m_intGroupType == 0)
        {
            result.Append(m_refStyle.GetHelpButton("ViewUsersByGroupToolBar", ""));
        }
        else
        {
            if (-1 == m_intUserActiveFlag)
            {
                result.Append(m_refStyle.GetHelpButton("Viewnotverifiedusers", ""));
            }
            else
            {
                result.Append(m_refStyle.GetHelpButton("ViewMembershipUsers", ""));
            }
        }

        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private string IsSelected(string val)
    {
        if (val == m_strSelectedItem)
        {
            return (" selected ");
        }
        else
        {
            return ("");
        }
    }
    #endregion

    #region MapCMSUserToAD
    public bool MapCMSUserToAD()
    {
        search = Request.QueryString["search"];
        if (!Page.IsPostBack || (Page.IsPostBack && !string.IsNullOrEmpty(Request.Form[isPostData.UniqueID])))
        {
            Display_MapCMSUserToAD();
        }
        else if (Page.IsPostBack)
        {
            Process_MapCMSUserToAD();
            return (true);
        }
        return false;
    }
    private void Process_MapCMSUserToAD()
    {
        long uID = System.Convert.ToInt64(Request.Form["id"]);
        string[] tempArray = Strings.Split(Request.Form["usernameanddomain"], "_@_", -1, 0);
        string strUserName = tempArray[0].ToString();   //. tempArray0).ToString();
        string strDomain = tempArray[1].ToString();
        m_refUserApi.RemapCMSUserToAD(uID, strUserName, strDomain, 0);
        string returnPage = "";
        if (Request.Form["rp"] == "1")
        {
            returnPage = (string)("users.aspx?action=View&id=" + uID);
        }
        else
        {
            returnPage = (string)("adreports.aspx?action=SynchUsers&ReportType=" + Request.Form["rt"]);
        }
        Response.Redirect(returnPage, false);
    }
    private void Display_MapCMSUserToAD()
    {
        AppImgPath = m_refSiteApi.AppImgPath;
        f = Request.QueryString["f"];
        rp = Request.QueryString["rp"];
        e1 = Request.QueryString["e1"];
        e2 = Request.QueryString["e2"];
        if (string.IsNullOrEmpty(rp))
        {
            rp = Request.Form["rp"];
        }

        if (string.IsNullOrEmpty(e1))
        {
            e1 = Request.Form["e1"];
        }

        if (string.IsNullOrEmpty(e2))
        {
            e2 = Request.Form["e2"];
        }

        if (!string.IsNullOrEmpty(e1))
        {
            fieldId = EkFunctions.ReadIntegerValue(e1.Replace("username", ""), 0);
        }

        if (string.IsNullOrEmpty(f))
        {
            f = Request.Form["f"];
        }
        language_data = m_refSiteApi.GetAllActiveLanguages();
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {

            uId = Convert.ToInt64(Request.QueryString["id"]);
            if (uId == -1)
            {
                uId = Convert.ToInt64(Request.Form["id"]);
            }
        }


        user_data = m_refUserApi.GetUserById(uId, false, false);
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

        if ((setting_data.ADAuthentication == 1) && (string.IsNullOrEmpty(search) || search == "0"))
        {
            PostBackPage.Text = Utilities.SetPostBackPage((string)("users.aspx?Action=MapCMSUserToAD&Search=1&LangType=" + ContentLanguage + "&rp=" + rp + "&e1=" + e1 + "&e2=" + e2 + "&f=" + f + "&id=" + uId));
            domain_data = m_refUserApi.GetDomains(0, 0);
            //TOOLBAR
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("search ad for cms user") + " \"" + user_data.DisplayUserName + "\""));
            result.Append("<table><tr>");
            if (rp != "1")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=\"top.close();\"", StyleHelper.CancelButtonCssClass, true));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            }
            result.Append("</tr></table>");
            htmToolBar.InnerHtml = result.ToString();
            Populate_MapCMSUserToADGrid();
        }
        else
        {
            string Domain = "";
            string Sort = "";

            System.Collections.Specialized.NameValueCollection sdAttributes = new System.Collections.Specialized.NameValueCollection(); //New Collection
            System.Collections.Specialized.NameValueCollection sdFilter = new System.Collections.Specialized.NameValueCollection(); //New Collection

            sdAttributes.Add("UserName", "UserName");
            sdAttributes.Add("FirstName", "FirstName");
            sdAttributes.Add("LastName", "LastName");
            sdAttributes.Add("Domain", "Domain");

            if (search == "1")
            {
                UserName = Request.Form["username"];
                FirstName = Request.Form["firstname"];
                LastName = Request.Form["lastname"];
                Domain = Request.Form["domainname"];
                Sort = "UserName";
            }
            else
            {
                UserName = Request.QueryString["username"];
                FirstName = Request.QueryString["firstname"];
                LastName = Request.QueryString["lastname"];
                Domain = Request.QueryString["domainname"];
                Sort = Request.QueryString["OrderBy"];
            }

            if ((string.IsNullOrEmpty(UserName)) && (string.IsNullOrEmpty(FirstName)) && (string.IsNullOrEmpty(LastName)))
            {
                sdFilter.Add("UserName", "UserName");
                sdFilter.Add("UserNameValue", "*");
            }
            else
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    sdFilter.Add("UserName", "UserName");
                    sdFilter.Add("UserNameValue", UserName); //sdFilter.add (UserName,"UserNameValue")
                }
                if (!string.IsNullOrEmpty(FirstName))
                {
                    sdFilter.Add("FirstName", "FirstName");
                    sdFilter.Add("FirstNameValue", FirstName);
                }
                if (!string.IsNullOrEmpty(LastName))
                {
                    sdFilter.Add("LastName", "LastName");
                    sdFilter.Add("LastNameValue", LastName);
                }
            }
            UserData[] result_data;
            result_data = m_refUserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, Domain);
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("search ad for cms user"));
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append("<table><tr>");

            if (rp == "3")
            {
                string backPage = string.Format("users.aspx?action=MapCMSUserToAD&id={0}&f=0&e1=username{1}&e2=domain{1}&rp=3", uId, fieldId);
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", backPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            }
			else if (Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains("msie"))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:window.location.reload( false );", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			if (rp != "1")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=\"top.close();\"", "", true));
			}

            if (!(result_data == null))
            {
                if (rp == "1")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update button text (associate user)"), m_refMsg.GetMessage("alt update button text (associate user)"), "onclick=\"document.forms[0].user_isPostData.value=\'\'; return SubmitForm(\'aduserinfo\', \'CheckRadio(0);\');\"", StyleHelper.EditButtonCssClass, true));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update button text (associate user)"), m_refMsg.GetMessage("alt update button text (associate user)"), "onclick=\"document.forms[0].user_isPostData.value=\'\'; return SubmitForm(\'aduserinfo\', \'CheckReturn(0);\');\"", StyleHelper.EditButtonCssClass, true));
                }
            }
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserToAD", ""));
            result.Append("</td>");
            result.Append("</tr></table>");
            htmToolBar.InnerHtml = result.ToString();
            Populate_MapCMSUserToADGrid_Search(result_data);
        }
    }
    private void Populate_MapCMSUserToADGrid_Search(UserData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ADD";
        colBound.HeaderText = m_refMsg.GetMessage("add title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(2);
        colBound.ItemStyle.Width = Unit.Percentage(2);
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=UserName&LangType=" + ContentLanguage + "&username=" + UserName + "&lastname=" + LastName + "&firstname=" + FirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Username") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=LastName&LangType=" + ContentLanguage + "&username=" + UserName + "&lastname=" + LastName + "&firstname=" + FirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Lastname") + "</a>";
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=FirstName&LangType=" + ContentLanguage + "&username=" + UserName + "&lastname=" + LastName + "&firstname=" + FirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Firstname") + "</a>";
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ADD", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));

        int i = 0;
        if (!(data == null))
        {
            if (data.Length > 0)
            {
                ltr_message.Text = "";
                for (i = 0; i <= data.Length - 1; i++)
                {
                    dr = dt.NewRow();
                    dr[0] = "<input type=\"Radio\" name=\"usernameanddomain\" value=\"" + data[i].Username + "_@_" + data[i].Domain + "\" onClick=\"SetUp(\'" + data[i].Username.Replace("\'", "\\\'") + "_@_" + data[i].Domain + "\')\">";
                    dr[1] = data[i].Username;
                    dr[2] = data[i].LastName;
                    dr[3] = data[i].FirstName;
                    dr[4] = data[i].Domain;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                ltr_message.Text = "<br />" + m_refMsg.GetMessage("the search resulted in zero matches");
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("no ad users found");
            dr[1] = "";
            dr[2] = "";
            dr[3] = "";
            dt.Rows.Add(dr);
        }
        dr = dt.NewRow();
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<input type=\"hidden\" name=\"id\" value=\"" + uId + "\">");
        result.Append("<input type=\"hidden\" name=\"rp\" value=\"" + rp + "\">");
        result.Append("<input type=\"hidden\" name=\"e1\" value=\"" + e1 + "\">");
        result.Append("<input type=\"hidden\" name=\"e2\" value=\"" + e2 + "\">");
        result.Append("<input type=\"hidden\" name=\"f\" value=\"" + f + "\">");
        result.Append("<input type=\"hidden\" name=\"adusername\">");
        result.Append("<input type=\"hidden\" name=\"addomain\">");
        dr[0] = result.ToString();
        dt.Rows.Add(dr);
        DataView dv = new DataView(dt);
        MapCMSUserToADGrid.DataSource = dv;
        MapCMSUserToADGrid.DataBind();
    }
    private void Populate_MapCMSUserToADGrid()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic Username");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic Firstname");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic Lastname");
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        MapCMSUserToADGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = "<input type=\"Text\" name=\"username\" maxlength=\"255\" class=\"ektronTextXSmall\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[1] = "<input type=\"Text\" name=\"firstname\" maxlength=\"50\" class=\"ektronTextXSmall\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[2] = "<input type=\"Text\" name=\"lastname\" maxlength=\"50\" class=\"ektronTextXSmall\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[2] += "<input type=\"hidden\" id=\"uid\" name=\"uid\" value=\"\"> <input type=\"hidden\" id=\"rp\" name=\"rp\" value=\"\">";
        dr[2] += "<input type=\"hidden\" id=\"ep\" name=\"e1\" value=\"\"> <input type=\"hidden\" id=\"e2\" name=\"e2\" value=\"\">";
        dr[2] += "<input type=\"hidden\" id=\"f\" name=\"f\" value=\"\">";
        dr[3] = "<select name=\"domainname\">";
        if ((!(domain_data == null)) && m_refContentApi.RequestInformationRef.ADAdvancedConfig == false)
        {
            dr[3] += "<option selected value=\"\">" + m_refMsg.GetMessage("all domain select caption") + "</option>";
        }
        int i;
        for (i = 0; i <= domain_data.Length - 1; i++)
        {
            dr[3] += "<option value=\"" + domain_data[i].Name + "\">" + domain_data[i].Name + "</option>";
        }
        dr[3] += "</select>";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "<input type=\"submit\" name=\"search\" value=\"" + m_refMsg.GetMessage("generic Search") + "\">";
        dr[1] = "";
        dr[2] = "";
        dr[3] = "";
        dt.Rows.Add(dr);
        DataView dv = new DataView(dt);
        MapCMSUserToADGrid.DataSource = dv;
        MapCMSUserToADGrid.DataBind();
    }
    #endregion

    private bool LDAPMembers()
    {
        if (m_intGroupType == 1) //member
        {
            return (m_refUserApi.RequestInformationRef.LDAPMembershipUser);
        }
        else if (m_intGroupType == 0) //CMS user
        {
            return true;
        }
        return true;
    }
    protected void RegisterResources()
    {
        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronWamenuJs");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/ActivityStream/activityStream.css", "ActivityStream");
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronWamenuCss");
    }
    private void LoadGrid(string display)
    {

        Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
        activityListCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;

        if (display == "colleagues")
        {
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague);
        }
        else
        {
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
        }
        activityTypeList = _activityListApi.GetList(activityListCriteria);

        System.Data.DataTable dt = new System.Data.DataTable();
        System.Data.DataRow dr;
        dt.Columns.Add(new System.Data.DataColumn("EMPTY", typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn("EMAIL", typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn("SMS", typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn("NEWSFEED", typeof(string)));
        LoadPreferenceList();
        for (int i = 0; i <= activityTypeList.Count - 1; i++)
        {
            dr = dt.NewRow();
            dr["EMPTY"] = GetResourceText(activityTypeList[i].Name);            
            if (preferenceList.Count > 0)
            {
                foreach (NotificationPreferenceData tempLoopVar_prefData in preferenceList)
                {
                    prefData = tempLoopVar_prefData;
                    if (CompareIds(activityTypeList[i].Id, 1))
                    {
                        dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" checked=\"checked\" DISABLED /></center>";
                    }
                    else
                    {
                        dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\"  DISABLED /></center>";
                    }
                    if (CompareIds(activityTypeList[i].Id, 2))
                    {
                        dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" checked=\"checked\" DISABLED  /></center>";
                    }
                    else
                    {
                        dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" DISABLED /></center>";

                    }

                    if (CompareIds(activityTypeList[i].Id, 3))
                    {
                        dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" checked=\"checked\" DISABLED /></center>";
                    }
                    else
                    {
                        dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" DISABLED /></center>";
                    }

                }
                dt.Rows.Add(dr);
            }
            else
            {
                dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\"/></center>";
                dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\"/></center>";
                dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\"/></center>";
                dt.Rows.Add(dr);
            }

        }
        System.Data.DataView dv = new System.Data.DataView(dt);
        if (display == "colleagues")
        {
            CollGrid.DataSource = dv;
            CollGrid.DataBind();
        }
        else
        {
            GroupGrid.DataSource = dv;
            GroupGrid.DataBind();
        }

    }
    private bool CompareIds(long prefActivityTypeId, long prefAgentId)
    {
        foreach (NotificationPreferenceData tempLoopVar_prefData in preferenceList)
        {
            prefData = tempLoopVar_prefData;
            if (prefData.ActivityTypeId == prefActivityTypeId && prefAgentId == prefData.AgentId)
            {
                return true;
            }
        }
        return false;
    }
    private void LoadPreferenceList()
    {
        System.Collections.Generic.List<NotificationPreferenceData> groupPrefList;
        NotificationPreferenceCriteria criteria = new NotificationPreferenceCriteria();
        criteria.PagingInfo.RecordsPerPage = 1000;
        criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, uId);
        //Getting the Group Preference list
        groupPrefList = _notificationPreferenceApi.GetDefaultPreferenceList(criteria);
        //need to set source to 0 because we dont want individual group prefs.
        criteria.AddFilter(NotificationPreferenceProperty.ActionSourceId, CriteriaFilterOperator.EqualTo, 0);
        //Getting the Colleagues preference list
        preferenceList = _notificationPreferenceApi.GetList(criteria);

        //Adding the group list to Preferences
        preferenceList.AddRange(groupPrefList);
    }
    private void CreateColumns()
    {
        NotificationAgentCriteria criteria = new NotificationAgentCriteria();
        criteria.AddFilter(NotificationAgentProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
        agentList = _notificationAgentApi.GetList(criteria);

        if ((agentList != null) && agentList.Count > 0)
        {
            CollGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            foreach (NotificationAgentData agentData in agentList)
            {
                if (agentData.IsEnabled)
                {
                    if ((agentData.Id) == 1)
                    {
                        CollGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                        GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                    else if ((agentData.Id) == 2)
                    {
                        CollGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                        GroupGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                    else if ((agentData.Id) == 3)
                    {
                        CollGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                        GroupGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                }
            }
        }
    }
    private string  GetResourceText(string st)
    {
        //------------------My Activities-----------------------------------------
        if (st == "Blog Post")
            st = m_refMsg.GetMessage("lbl BlogPost");
        else if (st == "Blog Comment")
            st = m_refMsg.GetMessage("lbl blog comment");
        else if (st == "Forum Post")
            st = m_refMsg.GetMessage("lbl Forum Post");
        else if (st == "Forum Reply")
            st = m_refMsg.GetMessage("lbl Forum Reply");
        else if (st == "Add User Workspace Content")
            st = m_refMsg.GetMessage("lbl Add User Workspace");
        else if (st == "Edit User Workspace Content")
            st = m_refMsg.GetMessage("lbl Edit User Workspace");
        else if (st == "Content Messageboard Post")
            st = m_refMsg.GetMessage("lbl Content Messageboard");
        else if (st == "User Messageboard Post")
            st = m_refMsg.GetMessage("lbl User Messageboard");
        else if (st == "Micro-message")
            st = m_refMsg.GetMessage("lbl Micromessage");
        else if (st == "Add Site Content")
            st = m_refMsg.GetMessage("lbl Add Site Content");
        else if (st == "Edit Content")
            st = m_refMsg.GetMessage("edit content page title");
        else if (st == "Create Community Group")
            st = m_refMsg.GetMessage("lbl CommunityGroup");
        else if (st == "Join Community Group")
            st = m_refMsg.GetMessage("lbl Join Community Group");
        else if (st == "Add Colleague")
            st = m_refMsg.GetMessage("lbl Add Colleague");
        else if (st == "Add Calendar Event")
            st = m_refMsg.GetMessage("add cal event");
        else if (st == "Update Calendar Event")
            st = m_refMsg.GetMessage("lbl Update Calendar Event");
        //---------------CommunityGroups--------------------------------------
        else if (st == "Group Blog Post")
            st = m_refMsg.GetMessage("lbl Group Blog Post");
        else if (st == "Group Blog Comment")
            st = m_refMsg.GetMessage("lbl Group Blog Comment");
        else if (st == "Group Forum Post")
            st = m_refMsg.GetMessage("lbl Group Forum Post");
        else if (st == "Group Forum Reply")
            st = m_refMsg.GetMessage("lbl Group Forum Reply");
        else if (st == "Add Group Content")
            st = m_refMsg.GetMessage("lbl Add Group Content");
        else if (st == "Edit Group Content")
            st = m_refMsg.GetMessage("lbl Edit Group Content");
        else if (st == "Group Messageboard Post")
            st = m_refMsg.GetMessage("lbl Group Messageboard Post");
        else if (st == "Add Group Calendar Event")
            st = m_refMsg.GetMessage("lbl Add Group Calendar Event");
        else if (st == "Update Group Calendar Event")
            st = m_refMsg.GetMessage("lbl Update Group Calendar Event");

        return st;
    }
    private void ViewUserPublishPreferences()
    {
        Ektron.Cms.Framework.Notifications.NotificationPublishPreference _publishPrefApi = new Ektron.Cms.Framework.Notifications.NotificationPublishPreference();
        System.Collections.Generic.List<NotificationPublishPreferenceData> publishPrefList = new System.Collections.Generic.List<NotificationPublishPreferenceData>();

        publishPrefList = _publishPrefApi.GetList(uId);
        publishPrefList.Sort(new NotificationPublishPreferenceData());
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", m_refMsg.GetMessage("generic actions"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("ENABLED", "<center>" + m_refMsg.GetMessage("generic publish") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        System.Data.DataTable dt = new System.Data.DataTable();
        System.Data.DataRow dr;
        dt.Columns.Add(new System.Data.DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn("ENABLED", typeof(string)));
        foreach (NotificationPublishPreferenceData prefEntry in publishPrefList)
        {
            dr = dt.NewRow();
            dr["TYPE"] = GetResourceText(prefEntry.ActivityTypeName);          
            if (prefEntry.IsEnabled)
            {
                dr["ENABLED"] = "<center><input type=\"Checkbox\" name=\"pref" + prefEntry.ActivityTypeId + "\" id=\"pref" + prefEntry.ActivityTypeId + "\" checked=\"checked\" DISABLED /></center>";
            }
            else
            {
                dr["ENABLED"] = "<center><input type=\"Checkbox\" name=\"pref" + prefEntry.ActivityTypeId + "\" id=\"pref" + prefEntry.ActivityTypeId + "\" DISABLED /></center>";
            }
            dt.Rows.Add(dr);
        }
        System.Data.DataView dv = new System.Data.DataView(dt);
        PrivacyGrid.DataSource = dv;
        PrivacyGrid.DataBind();
    }
    private void LoadCommunityAliasTab()
    {
        Ektron.Cms.API.UrlAliasing.UrlAliasCommunity _communityAlias = new Ektron.Cms.API.UrlAliasing.UrlAliasCommunity();
        System.Collections.Generic.List<Ektron.Cms.Common.UrlAliasCommunityData> aliasList;

        aliasList = _communityAlias.GetListUser(uId);
        if (aliasList.Count > 0)
        {
            foreach (Ektron.Cms.Common.UrlAliasCommunityData item in aliasList)
            {
                groupAliasList += "<a href= " + this.m_refContentApi.SitePath + item.AliasName + " target=_blank>" + this.m_refContentApi.SitePath + item.AliasName + "</a>";
                groupAliasList += "<br/>";
            }
        }
        else
        {
            aliasTab.Visible = false;
            tblAliasList.Visible = false;
        }
    }
}