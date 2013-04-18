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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;


public partial class adduser : System.Web.UI.UserControl
{
    public adduser()
    {
        _CalendarApi = new Ektron.Cms.Content.Calendar.WebCalendar(m_refUserApi.RequestInformationRef);
    }

    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected PermissionData security_data;
    protected LanguageData[] language_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected SettingsData setting_data;
    protected DomainData[] domain_list;
    protected string search = "";
    protected string m_strUserName = "";
    protected int ContentLanguage = -1;
    protected string m_strFirstName = "";
    protected string m_strLastName = "";
    protected string m_strDomain = "";
    protected long uId = -1;
    protected UserData[] user_list;
    protected Collection pagedata;
    protected string FromUsers = "";
    protected UserData user_data;
    protected TemplateData[] template_list;
    protected Collection cAllFolders;
    protected UserPreferenceData defaultPreferences;
    protected bool bLDAP = false;
    protected int m_intGroupType = 0;
    protected long m_intGroupId = 2;
    protected UserData userdata = new Ektron.Cms.UserData();
    protected long UserId = 0;
    Ektron.Cms.Content.Calendar.WebCalendar _CalendarApi;
    Ektron.Cms.Common.Calendar.WebCalendarData calendardata = new Ektron.Cms.Common.Calendar.WebCalendarData();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        RegisterResources();

        if (!(Request.QueryString["grouptype"] == null) && Request.QueryString["grouptype"] != "")
        {
            m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
        }

        if (!(Request.QueryString["groupid"] == null) && Request.QueryString["groupid"] != "")
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
        }

        m_refMsg = m_refContentApi.EkMsgRef;
        AppImgPath = m_refSiteApi.AppImgPath;
        ContentLanguage = m_refSiteApi.ContentLanguage;
    }

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
        return false;
    }


    public void AddUserToSystem()
    {
        try
        {
            search = Request.QueryString["search"];
            setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

            if (LDAPMembers() && (setting_data.ADAuthentication == 1) && ((string.IsNullOrEmpty(search)) || (search == "0")))
            {
                Response.Redirect((string)("AD/ADsearch.aspx?grouptype=" + m_intGroupType.ToString() + "&groupid=" + m_intGroupId.ToString()), false);
                return;
                //					TR_AddUserDetail.Visible = false;
                //					TR_AddLDAPDetail.Visible = false;
                //					PostBackPage.Text = Utilities.SetPostBackPage((string) ("users.aspx?Action=AddUserToSystem&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&Search=1&LangType=" + ContentLanguage));
                //					Display_AddUserToSystem_Search();
            }
            else if (LDAPMembers() && (setting_data.ADAuthentication == 1) && ((search == "1") || (search == "2")))
            {

                TR_AddUserDetail.Visible = false;
                TR_AddLDAPDetail.Visible = false;
                if (!(Page.IsPostBack))
                {
                    Display_AddADUserToSystem();
                }
                else
                {
                    AddADUsersToSystem();
                }
            }
            else if (LDAPMembers() && (setting_data.ADAuthentication == 2))
            {
                TR_AddUserList.Visible = false;
                TR_AddUserDetail.Visible = false;
                if (!(Page.IsPostBack))
                {
                    Display_AddLDAPUserToSystem();
                    bLDAP = true;
                }
                else
                {
                    AddLDAPUsersToSystem();
                }
            }
            else
            {
                TR_AddUserList.Visible = false;
                TR_AddLDAPDetail.Visible = false;
                if (!Utilities.IsInternalPostback)
                {
                    Display_AddUserToSystem();
                }
                else
                {
                    Process_AddUserToSystem();
                }
            }

            if (!(Page.IsPostBack))
            {
                Display_UserCustomProperties(bLDAP);
            }
            else
            {
                if (m_intGroupType == 1)
                {
                    FromUsers = "1";
                    m_intGroupId = 888888; //default
                }
                if (Request.QueryString["search"] == "1" || Request.QueryString["search"] == "2")
                {
                    Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&id=2&FromUsers=" + FromUsers), false);
                }
                else
                {
                    Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId + "&id=2&FromUsers=" + FromUsers + "&OrderBy=" + Request.Form["OrderBy"]), false);
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.ToString().IndexOf("Username") > -1)
            {
                err_msg.Text = " <tr><td class=\"label\">&nbsp;</td><td style=\"color:red;\">" + m_refContentApi.EkMsgRef.GetMessage("com: duplicate username") + "</td></tr>";
                Display_AddUserToSystem();
                Display_UserCustomProperties(bLDAP);
                //Response.Redirect("reterror.aspx?info=" & EkFunctions.UrlEncode(ex.Message.ToString()), False)
            }
            else if (ex.ToString().IndexOf("already exists") > -1)
            {
                err_msg.Text = " <tr><td class=\"label\">&nbsp;</td><td style=\"color:red;\">" + m_refContentApi.EkMsgRef.GetMessage("com: duplicate displayname") + "</td></tr>";
                Display_AddUserToSystem();
                Display_UserCustomProperties(bLDAP);
            }
            else
            {
                throw (ex);
            }
        }
    }

    private void AddADUsersToSystem()
    {
        Collection sdUsersNames = new Collection();
        Collection sdUsersDomains = new Collection();
        int lcount;
        string strUsername = "";
        string strDomain = "";
        for (lcount = 1; lcount <= System.Convert.ToInt32(Request.Form[addusercount.UniqueID]); lcount++)
        {
            strUsername = "";
            strDomain = "";
            strUsername = Request.Form["adduser" + lcount.ToString()].ToString();
            strDomain = Request.Form["adduserdomain" + lcount.ToString()].ToString();
            if ((strUsername != "") && (strDomain != ""))
            {
                sdUsersNames.Add(strUsername, lcount.ToString(), null, null);
                sdUsersDomains.Add(strDomain, lcount.ToString(), null, null);
            }
        }
        if (m_intGroupType == 0)
        {
            m_refUserApi.AddADUsersToCMSByUsername(sdUsersNames, sdUsersDomains);
        }
        else
        {
            Ektron.Cms.User.EkUser usr;
            bool ret = false;
            usr = m_refUserApi.EkUserRef;
            ret = usr.AddADmemberUsersToCmsByUsername(sdUsersNames, sdUsersDomains);
        }
    }
    private void AddLDAPUsersToSystem()
    {
        try
        {
            pagedata = new Collection();
            pagedata.Add(Request.Form[LDAP_username.UniqueID], "UserName", null, null);
            pagedata.Add(Request.Form[LDAP_firstname.UniqueID], "FirstName", null, null);
            pagedata.Add(Request.Form[LDAP_lastname.UniqueID], "LastName", null, null);
            pagedata.Add(Request.Form[LDAP_displayname.UniqueID], "DisplayName", null, null);
            pagedata.Add(Request.Form[LDAP_username.UniqueID], "Password", null, null);
            pagedata.Add(Request.Form[LDAP_language.UniqueID], "Language", null, null);
            pagedata.Add(Request.Form[drp_LDAPeditor.UniqueID], "EditorOptions", null, null);
            string Org = "";
            // Dim arrOrgU As Array
            //long arrCount = 0;
            //string strDC = "";
            if (Request.Form[LDAP_ldapdomain.UniqueID] != "")
            {
                //If (Request.Form(LDAP_orgunit.UniqueID) <> "") Then
                //    arrOrgU = Split(CStr(Request.Form(LDAP_orgunit.UniqueID)), ",")
                //    For arrCount = LBound(arrOrgU) To UBound(arrOrgU)
                //        If (Not (arrOrgU(arrCount) = "")) Then
                //            If (Not (Org = "")) Then
                //                Org &= ","
                //            End If
                //            Org &= "ou="
                //            Org &= arrOrgU(arrCount)
                //        End If
                //    Next
                //End If
                //If (Not (CStr(Request.Form(LDAP_organization.UniqueID)) = "")) Then
                //    Org &= ",o="
                //    Org &= CStr(Request.Form(LDAP_organization.UniqueID))
                //End If
                //If (Not (CStr(Request.Form(LDAP_ldapdomain.UniqueID)) = "")) Then
                //    arrOrgU = Split(CStr(Request.Form(LDAP_ldapdomain.UniqueID)), ".")
                //    For arrCount = LBound(arrOrgU) To UBound(arrOrgU)
                //        strDC &= ",dc="
                //        strDC &= arrOrgU(arrCount)
                //    Next
                //    Org &= strDC
                //End If
                Org = Request.Form[LDAP_ldapdomain.UniqueID];
                pagedata.Add(Org, "Domain", null, null);
            }
            if (Request.Form[LDAP_email_addr1.UniqueID] != "")
            {
                pagedata.Add(Request.Form[LDAP_email_addr1.UniqueID], "EmailAddr1", null, null);
            }
            else
            {
                pagedata.Add("", "EmailAddr1", null, null);
            }
            if (Request.Form[LDAP_disable_msg.UniqueID] != "")
            {
                pagedata.Add(1, "DisableMsg", null, null);
            }
            else
            {
                pagedata.Add(0, "DisableMsg", null, null);
            }
            pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty", null, null);
            if (m_intGroupType == 0)
            {
                m_refUserApi.AddUser(pagedata);
            }
            else
            {
                Ektron.Cms.User.EkUser usr;
                bool ret = false;
                usr = m_refUserApi.EkUserRef;
                ret = usr.AddMemberShipUserV4(ref pagedata);
            }
        }
        catch (Exception ex)
        {
            if (EkFunctions.DoesKeyExist(pagedata, "UserName"))
            {
                this.LDAP_username.Value = pagedata["UserName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "FirstName"))
            {
                this.LDAP_firstname.Value = pagedata["FirstName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "LastName"))
            {
                this.LDAP_lastname.Value = pagedata["LastName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Language") && pagedata["Language"] != null)
            {
                if (LDAP_language.Items.FindByValue(pagedata["Language"].ToString()) != null)
                {
                    LDAP_language.Items.FindByValue(pagedata["Language"].ToString()).Selected = true;
                }
            }
            if (EkFunctions.DoesKeyExist(pagedata, "EditorOptions"))
            {
                drp_LDAPeditor.SelectedIndex = drp_editor.Items.IndexOf(drp_editor.Items.FindByValue(pagedata["EditorOptions"].ToString()));
            }
            if (EkFunctions.DoesKeyExist(pagedata, "DisplayName"))
            {
                this.LDAP_displayname.Value = pagedata["DisplayName"].ToString();
            }

            if (EkFunctions.DoesKeyExist(pagedata, "EmailAddr1"))
            {
                LDAP_email_addr1.Value = pagedata["EmailAddr1"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "DisableMsg"))
            {
                LDAP_disable_msg.Checked = System.Convert.ToBoolean(pagedata["DisableMsg"].ToString());
            }
            throw (ex);
        }
    }
    private void AddLDAPUserToSystemToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user msg"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", (string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&FromUsers=" + Request.QueryString["FromUsers"] + "&groupid=" + m_intGroupId.ToString() + "&id=" + m_intGroupId.ToString() + "&OrderBy=" + Request.QueryString["OrderBy"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'ldapuserinfo\', \'VerifyLDAPForm()\');\"", StyleHelper.SaveButtonCssClass, true));
        //result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/loadBalance.png", "#", "Browse LDAP for a user.", "Browse LDAP", "Onclick=""javascript:window.open('LDAP/browse.aspx?from=users&uniqueid=' + UniqueID,null,'height=300,width=400,status=yes,toolbar=no,menubar=no,scrollbars=yes,location=no');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/loadBalance.png", (string)("LDAP/browse.aspx?from=users&method=select&uniqueid=&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId.ToString()), "Browse LDAP for a user.", "Browse LDAP", "", StyleHelper.BrowseButtonCssClass));
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/magnifier.png", (string)("LDAP/LDAPsearch.aspx?from=users&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId.ToString()), "Search LDAP for a user.", "Search LDAP", "", StyleHelper.SearchButtonCssClass));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("editusers_ascx", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }


    private void Process_AddUserToSystem()
    {
        Ektron.Cms.Commerce.IPasswordValidation pv = ObjectFactory.GetPasswordValidation();
        string validationString = (string)(pv.GetRegexForMember().Replace("\"", "\\\"").Replace("\\t", "\\\\t"));
        ValidationResults results;
        bool beIntranet = false;

        try
        {
            pagedata = new Collection();
            pagedata.Add(Request.Form[username.UniqueID], "UserName", null, null);
            pagedata.Add(Request.Form[firstname.UniqueID], "FirstName", null, null);
            pagedata.Add(Request.Form[lastname.UniqueID], "LastName", null, null);
            pagedata.Add(Request.Form[pwd.UniqueID], "Password", null, null);
            pagedata.Add(Request.Form[language.UniqueID], "Language", null, null);
            pagedata.Add(Request.Form[drp_editor.UniqueID], "EditorOptions", null, null);
            if (Request.Form[displayname.UniqueID] != "")
            {
                pagedata.Add(Request.Form[displayname.UniqueID], "DisplayName", null, null);
            }
            else
            {
                pagedata.Add("", "DisplayName", null, null);
            }
            pagedata.Add(Request.Form[avatar.UniqueID], "Avatar", null, null);
            pagedata.Add(Request.Form[mapaddress.UniqueID], "Address", null, null);
            pagedata.Add(Request.Form[maplatitude.UniqueID], "Latitude", null, null);
            pagedata.Add(Request.Form[maplongitude.UniqueID], "Longitude", null, null);
            if (Request.Form[email_addr1.UniqueID] != "")
            {
                pagedata.Add(Request.Form[email_addr1.UniqueID], "EmailAddr1", null, null);
            }
            else
            {
                pagedata.Add("", "EmailAddr1", null, null);
            }
            if (!string.IsNullOrEmpty(Request.Form[disable_msg.UniqueID]))
            {
                pagedata.Add(1, "DisableMsg", null, null);
            }
            else
            {
                pagedata.Add(0, "DisableMsg", null, null);
            }
            pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty", null, null);

            results = pv.ValidateForAuthor(Request.Form[pwd.UniqueID]);
            if (results.IsValid)
            {
                if (m_intGroupType == 1)
                {
                    Ektron.Cms.User.EkUser objUser;
                    bool ret;
                    objUser = m_refUserApi.EkUserRef;
                    ret = objUser.AddMemberShipUserV4(ref pagedata);
                    UserId = Convert.ToInt64(pagedata["userid"].ToString());
                }
                else
                {
                    userdata = m_refUserApi.AddUser(pagedata);
                    UserId = userdata.Id;
                }
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
            if (UserId > 0)
            {
                ProcessUserMessageBoardModeration(UserId);
            }
            beIntranet = System.Convert.ToBoolean((!(Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refUserApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers))) ? false : true);
            //'Adding calendar after the user is created.
            if (beIntranet && (Request.Form["ek_MsgBoardFeatures_Calendar"] != null))
            {
                string calendarChecked = Request.Form["ek_MsgBoardFeatures_Calendar"];
                if (calendarChecked == "on")
                {
                    calendardata.Name = "ek_calendar";
                    calendardata.Description = "";
                    _CalendarApi.Add(calendardata, EkEnumeration.WorkSpace.User, UserId);
                }
            }

        }
        catch (Exception ex)
        {
            if (EkFunctions.DoesKeyExist(pagedata, "UserName"))
            {
                this.username.Value = pagedata["UserName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "FirstName"))
            {
                this.firstname.Value = pagedata["FirstName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "LastName"))
            {
                this.lastname.Value = pagedata["LastName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Language") && pagedata["Language"] != null)
            {
                if (language.Items.FindByValue(pagedata["Language"].ToString()) != null)
                {
                    language.Items.FindByValue(pagedata["Language"].ToString()).Selected = true;
                }
            }
            if (EkFunctions.DoesKeyExist(pagedata, "EditorOptions"))
            {
                drp_editor.SelectedIndex = drp_editor.Items.IndexOf(drp_editor.Items.FindByValue(pagedata["EditorOptions"].ToString()));
            }
            if (EkFunctions.DoesKeyExist(pagedata, "DisplayName"))
            {
                this.displayname.Value = pagedata["DisplayName"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Avatar"))
            {
                this.avatar.Value = pagedata["Avatar"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Address"))
            {
                this.mapaddress.Value = pagedata["Address"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Latitude") && pagedata["Latitude"] != null)
            {
                this.maplatitude.Value = Ektron.Cms.Common.EkFunctions.ReadDbString(pagedata["Latitude"]);
            }
            if (EkFunctions.DoesKeyExist(pagedata, "Longitude") && pagedata["Longitude"] != null)
            {
                this.maplongitude.Value = Ektron.Cms.Common.EkFunctions.ReadDbString(pagedata["Longitude"]);
            }
            if (EkFunctions.DoesKeyExist(pagedata, "EmailAddr1"))
            {
                this.email_addr1.Value = pagedata["EmailAddr1"].ToString();
            }
            if (EkFunctions.DoesKeyExist(pagedata, "DisableMsg"))
            {
                this.disable_msg.Checked = Ektron.Cms.Common.EkFunctions.ReadDbString(pagedata["DisableMsg"]) == "1";
            }
            throw (ex);
        }
    }

    #region AddUserToSystem
    private void Display_AddUserToSystem()
    {
        this.Controls.AddAt(0, Utilities.GetUniqueIdField(m_refContentApi.RequestInformationRef.UniqueId));
        language_data = m_refSiteApi.GetAllActiveLanguages();
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        AddUserToSystemToolBar();
        int i;
        // drp_editor.Items.Add(New ListItem("eWebWP", "ewebwp"))
        drp_editor.Items.Add(new ListItem(m_refMsg.GetMessage("lbl content designer"), "contentdesigner"));
        //drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
        drp_editor.Items.Add(new ListItem("eWebEditPro", "ewebeditpro"));

        // All users default to contentdesigner since jsEditor is no longer supported
        //If m_intGroupType = 0 Then
        drp_editor.SelectedIndex = 0;
        //Else
        //drp_editor.SelectedIndex = 1
        //End If
        language.Items.Add(new ListItem(m_refMsg.GetMessage("app default msg"), "0"));
        if (!(language_data == null))
        {
            for (i = 0; i <= language_data.Length - 1; i++)
            {
                language.Items.Add(new ListItem(language_data[i].Name, language_data[i].Id.ToString()));
            }
        }
        if (security_data.IsAdmin && setting_data.EnableMessaging == false)
        {
            msg.Text = m_refMsg.GetMessage("application emails disabled msg");
        }
        litDisableMessage.Text = m_refMsg.GetMessage("disable email notifications msg");
        HttpBrowserCapabilities browser = Request.Browser;
        Ektron.Cms.Framework.Context.CmsContextService context = new Ektron.Cms.Framework.Context.CmsContextService();

        if (browser.Type.Contains("IE") && browser.MajorVersion >= 9)
        {
            // work around to prevent errors in IE9 when it destroys native JS objects
            // see http://msdn.microsoft.com/en-us/library/gg622929%28v=VS.85%29.aspx
            uxAvatarUploadIframe.Attributes.Add("src", "about:blank");
        }
        else
        {
            uxAvatarUploadIframe.Attributes.Add("src", context.WorkareaPath + "/Upload.aspx?action=edituser&addedit=true&returntarget=user_avatar&height=300&width=400&modal=true");
        }
        jsUxDialogSelectorTxt.Text = uxDialog.Selector.ToString();
        string uploadTxt = m_refMsg.GetMessage("upload txt");
        uxDialog.Title = uploadTxt;
        ltr_upload.Text = "</asp:Literal>&nbsp;<a href=\"#\" onclick=\"$ektron('" + uxDialog.Selector + "').dialog('open'); AvatarDialogInit();\" title=\"" + uploadTxt + "\" class=\"button buttonInline greenHover buttonUpload btnUpload\" title=\"" + uploadTxt + "\">" + uploadTxt + "</a>";
    }
    private void AddUserToSystemToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user msg"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", (string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&FromUsers=" + Request.QueryString["FromUsers"] + "&groupid=" + m_intGroupId + "&OrderBy=" + Request.QueryString["OrderBy"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'userinfo\', \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        if (m_intGroupType == 0)
        {
            result.Append(m_refStyle.GetHelpButton("editusers_ascx", ""));
        }
        else
        {
            result.Append(m_refStyle.GetHelpButton("AddUserMemberToSystem", ""));
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion
    #region Display_AddADUserToSystem
    private void Display_AddADUserToSystem()
    {
        string Sort = "";
        System.Collections.Specialized.NameValueCollection sdAttributes = new System.Collections.Specialized.NameValueCollection(); //New Collection
        System.Collections.Specialized.NameValueCollection sdFilter = new System.Collections.Specialized.NameValueCollection(); //New Collection

        sdAttributes.Add("UserName", "UserName");
        sdAttributes.Add("FirstName", "FirstName");
        sdAttributes.Add("LastName", "LastName");
        sdAttributes.Add("Domain", "Domain");

        if (search == "1")
        {
            m_strUserName = Request.Form["username"];
            m_strFirstName = Request.Form["firstname"];
            m_strLastName = Request.Form["lastname"];
            m_strDomain = Request.Form["domainname"];
            Sort = "UserName";
        }
        else
        {
            m_strUserName = Request.QueryString["username"];
            m_strFirstName = Request.QueryString["firstname"];
            m_strLastName = Request.QueryString["lastname"];
            m_strDomain = Request.QueryString["domainname"];
            Sort = Request.QueryString["OrderBy"];
        }

        if ((m_strUserName == "") && (m_strFirstName == "") && (m_strLastName == ""))
        {
            sdFilter.Add("UserName", "UserName");
            sdFilter.Add("UserNameValue", "*");
        }
        else
        {
            if (m_strUserName != "")
            {
                sdFilter.Add("UserName", "UserName");
                sdFilter.Add("UserNameValue", m_strUserName); //sdFilter.add (UserName,"UserNameValue")
            }
            if (m_strFirstName != "")
            {
                sdFilter.Add("FirstName", "FirstName");
                sdFilter.Add("FirstNameValue", m_strFirstName);
            }
            if (m_strLastName != "")
            {
                sdFilter.Add("LastName", "LastName");
                sdFilter.Add("LastNameValue", m_strLastName);
            }
        }
        user_list = m_refUserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, m_strDomain);

        AddADUserToSystemToolBar();
        AddADUserToSystemToolBarGrid();
    }
    private void AddADUserToSystemToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view users in active directory msg"));
        result.Append("<table><tr>");
		
		if (Request.ServerVariables["HTTP_USER_AGENT"].ToString().IndexOf("MSIE") > -1) //defect 16045
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if ((!(user_list == null)) && (user_list.Length > 0))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (users)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'aduserinfo\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("editusers_ascx", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void AddADUserToSystemToolBarGrid()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECK";
        colBound.HeaderText = m_refMsg.GetMessage("add title");
        colBound.HeaderStyle.Width = Unit.Percentage(2);
        colBound.ItemStyle.Width = Unit.Percentage(2);
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=UserName&LangType=" + ContentLanguage + "&username=" + m_strUserName + "&lastname=" + m_strLastName + "&firstname=" + m_strFirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Username") + "</a>";
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=LastName&LangType=" + ContentLanguage + "&username=" + m_strUserName + "&lastname=" + m_strLastName + "&firstname=" + m_strFirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Lastname") + "</a>";
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = "<a href=\"users.aspx?action=AddUserToSystem&OrderBy=FirstName&LangType=" + ContentLanguage + "&username=" + m_strUserName + "&lastname=" + m_strLastName + "&firstname=" + m_strFirstName + "&id=" + uId + "&search=2\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Firstname") + "</a>";
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DOMAIN";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        AddUserGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DOMAIN", typeof(string)));

        int i = 0;

        if (!(user_list == null))
        {
            for (i = 0; i <= user_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<input type=\"CHECKBOX\" name=\"adduser" + (i + 1) + "\" value=\"" + user_list[i].Username + "\">";
                dr[0] += "<input type=\"HIDDEN\" name=\"adduserdomain" + (i + 1) + "\" value=\"" + user_list[i].Domain + "\">";
                dr[1] = user_list[i].Username;
                dr[2] = user_list[i].LastName;
                dr[3] = user_list[i].FirstName;
                dr[4] = user_list[i].Domain;
                dt.Rows.Add(dr);
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
        addusercount.Value = i.ToString() + 1;
        DataView dv = new DataView(dt);
        AddUserGrid.DataSource = dv;
        AddUserGrid.DataBind();
    }
    #endregion
    #region Display_AddLDAPUserToSystem
    private void Display_AddLDAPUserToSystem()
    {
        language_data = m_refSiteApi.GetAllActiveLanguages();
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        AddLDAPUserToSystemToolBar();
        int i;
        // drp_LDAPeditor.Items.Add(New ListItem("eWebWP", "ewebwp"))
        //drp_LDAPeditor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
        drp_LDAPeditor.Items.Add(new ListItem("eWebEditPro", "ewebeditpro"));
        //If m_intGroupType = 0 Then
        //    drp_LDAPeditor.SelectedIndex = 1
        //Else
        drp_LDAPeditor.SelectedIndex = 0;
        //End If
        LDAP_language.Items.Add(new ListItem(m_refMsg.GetMessage("app default msg"), "0"));
        if (!(language_data == null))
        {
            for (i = 0; i <= language_data.Length - 1; i++)
            {
                LDAP_language.Items.Add(new ListItem(language_data[i].Name, language_data[i].Id.ToString()));
            }
        }
        if (security_data.IsAdmin && setting_data.EnableMessaging == false)
        {
            LDAP_msg.Text = "<tr><td colspan=\"2\" class=\"important\">" + m_refMsg.GetMessage("application emails disabled msg") + "</td></tr>";
        }
        LDAP_disable_msg.Text = m_refMsg.GetMessage("disable email notifications msg");
        // <input type="CHECKBOX" name="disable_msg" value="disable_msg" >
        //<%= (m_refMsg.GetMessage("disable email notifications msg")) %>&nbsp;
    }
    #endregion
    #region Display_AddUserToSystem_Search
    private void Display_AddUserToSystem_Search()
    {
        language_data = m_refSiteApi.GetAllActiveLanguages();
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        domain_list = m_refUserApi.GetDomains(0, 0);
        AddUserToSystemToolBar_Search();
        AddUserToSystemGrid_Search();
    }
    private void AddUserToSystemToolBar_Search()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("active directory search msg"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("editusers_ascx", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void AddUserToSystemGrid_Search()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic Username");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic Firstname");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Height = Unit.Empty;
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = m_refMsg.GetMessage("generic Lastname");
        colBound.ItemStyle.Wrap = false;
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        AddUserGrid.Columns.Add(colBound);



        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));


        dr = dt.NewRow();
        dr[0] = "<input type=\"Text\" name=\"username\" maxlength=\"255\" size=\"15\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[1] = "<input type=\"Text\" name=\"firstname\" maxlength=\"50\" size=\"15\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[2] = "<input type=\"Text\" name=\"lastname\" maxlength=\"50\" size=\"15\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[2] += "<input type=\"hidden\" id=\"uid\" name=\"uid\" value=\"\"> <input type=\"hidden\" id=\"rp\" name=\"rp\" value=\"\">";
        dr[2] += "<input type=\"hidden\" id=\"ep\" name=\"e1\" value=\"\"> <input type=\"hidden\" id=\"e2\" name=\"e2\" value=\"\">";
        dr[2] += "<input type=\"hidden\" id=\"f\" name=\"f\" value=\"\">";
        dr[3] = "<select name=\"domainname\">";
        if ((!(domain_list == null)) && m_refContentApi.RequestInformationRef.ADAdvancedConfig == false)
        {
            dr[3] += "<option selected value=\"\">" + m_refMsg.GetMessage("all domain select caption") + "</option>";
        }
        int i;
        for (i = 0; i <= domain_list.Length - 1; i++)
        {
            dr[3] += "<option value=\"" + domain_list[i].Name + "\">" + domain_list[i].Name + "</option>";
        }
        dr[3] += "</select>";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "<input type=\"submit\" name=\"search\" value=\"" + m_refMsg.GetMessage("generic Search") + ">";
        dr[1] = "";
        dr[2] = "";
        dr[3] = "";
        dt.Rows.Add(dr);
        DataView dv = new DataView(dt);
        AddUserGrid.DataSource = dv;
        AddUserGrid.DataBind();
    }
    #endregion
    #region Extending User Modal (Custom Properties)
    private void Display_UserCustomProperties(bool LDAP)
    {
        string strHtml = string.Empty;
        strHtml = m_refUserApi.EditUserCustomProperties(0, false);
        if (LDAP)
        {
            LDAP_litUCPUI.Visible = true;
            LDAP_litUCPUI.Text = strHtml;
        }
        else
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append(strHtml + "\n");
            sBuilder.Append("<tr></tr><tr><td><div id=\"ek_MsgBoardModerationLabel\"><label class=\"label\">" + m_refMsg.GetMessage("lbl perm moderate") + ":" + "</label></div></td>\n");
            sBuilder.Append("<td><div id=\"ek_MsgBoardModeration\"><input type=\"checkbox\" id=\"ek_MsgBoardModerate\"  name = \"ek_MsgBoardModerate\"/>" + m_refMsg.GetMessage("lbl msgboard") + "<br/><span>" + m_refMsg.GetMessage("lbl usermsgboardnotify") + "</span></div></td></tr> \n");
            
            litUCPUI.Text = sBuilder.ToString();//strHtml;
        }
    }
    #endregion

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");
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
}

