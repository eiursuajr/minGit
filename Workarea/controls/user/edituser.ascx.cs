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
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;

	public partial class edituser : System.Web.UI.UserControl
	{

		protected Ektron.ContentDesignerWithValidator ctlEditor;
		protected PermissionData security_data;
		public SettingsData setting_data;
		protected EkMessageHelper m_refMsg;
		protected string FromUsers = "";
		protected LanguageData[] language_data;
		protected long uId;
		protected UserData user_data;
		protected TemplateData[] template_list;
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected UserPreferenceData defaultPreferences;
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected int ContentLanguage;
		protected int m_intGroupType = 0;
		protected long m_intGroupId = 0;
		protected string m_sSignature = "";
		
		protected StyleHelper _refStyle = new StyleHelper();
		protected Ektron.Cms.Framework.Notifications.NotificationPreference _notificationPreferenceApi = new Ektron.Cms.Framework.Notifications.NotificationPreference();
		protected System.Collections.Generic.List<NotificationPreferenceData> preferenceList;
		protected NotificationPreferenceData prefData = new NotificationPreferenceData();
		protected Ektron.Cms.Framework.Notifications.NotificationAgentSetting _notificationAgentApi = new Ektron.Cms.Framework.Notifications.NotificationAgentSetting();
		protected System.Collections.Generic.List<NotificationAgentData> agentList;

        protected Ektron.Cms.Framework.Activity.Activity _activityApi = new Ektron.Cms.Framework.Activity.Activity();
        protected Ektron.Cms.Framework.Activity.ActivityType _activityListApi = new Ektron.Cms.Framework.Activity.ActivityType();
		protected System.Collections.Generic.List<Ektron.Cms.Activity.ActivityTypeData> activityTypeList;
		protected bool _IsCmsUser = false;
		protected string groupAliasList = string.Empty;
		
		/// <summary>
		/// Returns true if there are more than one languages enabled for the site.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool IsSiteMultilingual
		{
			get
			{
				if (m_refUserApi.EnableMultilingual == 0)
				{
					return false;
				}
				int languageEnabledCount = 0;
				foreach (LanguageData lang in languageDataArray)
				{
					if (lang.SiteEnabled)
					{
						languageEnabledCount++;
					}
					if (languageEnabledCount > 1)
					{
						break;
					}
				}
				
				return languageEnabledCount > 1;
			}
			
		}
		
		public LanguageData[] languageDataArray
		{
			get
			{
				if (language_data == null)
				{
					language_data = m_refSiteApi.GetAllActiveLanguages();
				}
				
				return language_data;
			}
			
		}
		
		public bool IsCmsUser
		{
			get
			{
				return _IsCmsUser;
			}
			set
			{
				_IsCmsUser = value;
			}
		}
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			ContentLanguage = m_refSiteApi.ContentLanguage;
			ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("../Editor/ContentDesignerWithValidator.ascx");
			ltr_sig.Controls.Add(ctlEditor);
			ctlEditor.ID = "content_html";
			ctlEditor.AllowScripts = false;
			ctlEditor.Height = new Unit(200, UnitType.Pixel);
			ctlEditor.Width = new Unit(570, UnitType.Pixel);
			ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
			ctlEditor.AllowFonts = true;
			ctlEditor.ShowHtmlMode = false;
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			RegisterResources();
			if ((! (Request.QueryString["grouptype"] == null) ) && (Request.QueryString["grouptype"] != ""))
			{
				m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
			}
			if ((! (Request.QueryString["groupid"] == null) ) && (Request.QueryString["groupid"] != ""))
			{
				m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
			}
			m_refMsg = m_refContentApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			AppPath = m_refSiteApi.AppPath;
			
			this.phWorkareaTab.Visible = this.IsCmsUser;
			this.phWorkareaContent.Visible = this.IsCmsUser;
		}
		
		protected void Page_Unload(object sender, System.EventArgs e)
		{
			if (IsPostBack)
			{
				m_sSignature = (string) ctlEditor.Content;
				// Remove SCRIPT elements to prevent XSS attacks, although ContentDesigner.AllowScripts="false" should prevent from getting this far.
				m_sSignature = Regex.Replace(m_sSignature, "\\<script[\\w\\W]+\\<\\/script\\>", "", RegexOptions.IgnoreCase);
			}
		}
		
		public bool EditUser()
		{
			if (!(Page.IsPostBack))
			{
				Display_EditUser();
				Display_UserCustomProperties();
				EditUserToolBar();
			}
            return false;
		}
		
		private void Display_EditUser()
		{
			Ektron.Cms.Content.EkContent m_refContent;
						
			FromUsers = Request.QueryString["FromUsers"];
			uId = long.Parse(Request.QueryString["id"]);
			user_data = m_refUserApi.GetUserById(uId, true, false);
			template_list = m_refContentApi.GetAllTemplates("TemplateFileName");
			
			m_refContent = m_refSiteApi.EkContentRef;
			
			security_data = m_refContentApi.LoadPermissions(0, "content", 0);
			
			language.Text = GetAllLanguageDropDownMarkup("language", m_refMsg.GetMessage("app default msg"));
			
			if (m_intGroupType == 0)
			{
				
				defaultPreferences = m_refUserApi.GetUserPreferenceById(m_refSiteApi.UserId);
				if (defaultPreferences.FolderPath == null)
				{
					jsPreferenceFolderId.Text = "";
				}
				else
				{
					jsPreferenceFolderId.Text = Convert.ToString(defaultPreferences.FolderPath).Replace("\\", "\\\\");
				}
				jsPreferenceWidth.Text = defaultPreferences.Width.ToString();
				jsPreferenceHeight.Text = defaultPreferences.Height.ToString();
				jsPreferenceTemplate.Text = defaultPreferences.Template;
				jsPreferenceDispTitleTxt.Text = defaultPreferences.DisplayTitleText;
				
				TD_msg.Text += m_refMsg.GetMessage("disable email notifications msg");
				if (security_data.IsAdmin && setting_data.EnableMessaging == false)
				{
					enablemsg.Text = "<div class=\"ektronTopSpace\"></div><label class=\"ektronCaption\">" + m_refMsg.GetMessage("application emails disabled msg") + "</label>";
				}
				
				if (user_data.IsDisableMessage)
				{
					ltr_checkBox.Text = "<input type=\"checkbox\" maxlength=\"50\" size=\"25\" name=\"disable_msg\" id=\"disable_msg\" value=\"disable_msg\" CHECKED=\"True\">";
				}
				else
				{
					ltr_checkBox.Text = "<input type=\"checkbox\" name=\"disable_msg\" id=\"disable_msg\" value=\"disable_msg\">";
				}
				
				if (user_data.UserPreference.ForceSetting != true)
				{
					if (!(defaultPreferences == null))
					{
						preference.Text = "<tr><td/><td><div class=\'ektronTopSpace\'><a class=\"button buttonInlineBlock blueHover buttonDefault\" href=\"Javascript://\" onclick=\"RestoreDefault();return false;\">" + m_refMsg.GetMessage("lnk restore default") + "</a></div><div class=\'ektronTopSpace\'></div></td></tr>";
					}
				}
				folder.Text += "<td class=\"wrapText\"><label class=\"label wrapText\">" + m_refMsg.GetMessage("lbl set smart desktop") + "</label></td><td><input type=\"checkbox\"";
				if (user_data.UserPreference.ForceSetting)
				{
					folder.Text += " disabled ";
				}
				if (user_data.UserPreference.FolderId == "")
				{
					folder.Text += " checked ";
				}
				folder.Text += " id=\"chkSmartDesktop\" name=\"chkSmartDesktop\"/>";
				folder.Text += "<input type=\"hidden\" name=\"OldfolderId\" id=\"OldfolderId\" value=\"" + user_data.UserPreference.FolderId + "\"/>";
				folder.Text += "<input type=\"hidden\" name=\"folderId\" id=\"folderId\" value=\"" + user_data.UserPreference.FolderId + "\"/>";
				folder.Text += "</td>";
				
				if (user_data.UserPreference.ForceSetting)
				{
					lockedmsg.Text = "<tr><td class=\"important\" colspan=\"2\">" + m_refMsg.GetMessage("preferences locked msg") + "</td></tr>";
					
				}
				forcemsg.Text = "<td>";
				forcemsg.Text += m_refSiteApi.SitePath + "<input type=\"text\"";
				if (user_data.UserPreference.ForceSetting == true)
				{
					forcemsg.Text += " disabled ";
				}
				forcemsg.Text += " ID=\"templatefilename\" value=\"" + user_data.UserPreference.Template + "\" size=\"35\" class=\"minWidth\" name=\"templatefilename\" />";
				if (user_data.UserPreference.ForceSetting == false)
				{
					forcemsg.Text += "<a class=\"button buttonInline greenHover selectContent\" href=\"#\" onclick=\"LoadChildPage();return true;\">" + m_refMsg.GetMessage("generic select") + "</a>";
				}
				forcemsg.Text += "</td>";
				
				width.Text = "<td class=\"label\">" + m_refMsg.GetMessage("lbl imagetool resize width") + "</td><td><input class=\"minWidth\" type=\"text\" size=\"4\" id=\"txtWidth\"";
				if (user_data.UserPreference.ForceSetting)
				{
					width.Text += " disabled ";
				}
				width.Text += " value=\"" + user_data.UserPreference.Width + "\" name=\"txtWidth\">px</td>";
				
				height.Text = "<td class=\"label\">" + m_refMsg.GetMessage("lbl imagetool resize height") + "</td><td><input class=\"minWidth\" type=\"text\" size=\"4\" id=\"txtHeight\"";
				if (user_data.UserPreference.ForceSetting)
				{
					height.Text += " disabled ";
				}
				height.Text += " value=\"" + user_data.UserPreference.Height + "\" name=\"txtHeight\">px</td>";
				
                //disptext.Text = "<td class=\"label wrapText\">" + m_refMsg.GetMessage("lbl display button caption") + "</td><td><input type=\"checkbox\" id=\"chkDispTitleText\"";
                //if (user_data.UserPreference.DisplayTitleText == "1")
                //{
                //    disptext.Text += " checked ";
                //}
                //if (user_data.UserPreference.ForceSetting)
                //{
                //    disptext.Text += " disabled ";
                //}
                //disptext.Text += " name=\"chkDispTitleText\"/></td>";
                //disptext.Text += " <input type=\"hidden\" id=\"hdn_pagesize\" name=\"hdn_pagesize\" value=\"" + user_data.UserPreference.PageSize.ToString() + "\" />";
			}
			
			if (security_data.IsAdmin)
			{
				jsIsAdmin.Text = "true";
			}
			else
			{
				jsIsAdmin.Text = "false";
			}
			
			if (security_data.IsAdmin && !(
                ThirdPartyAuthenticationEnabled(user_data.IsMemberShip)
                ))
			{
                username.Text = "<input type=\"Text\" maxlength=\"255\" size=\"25\" name=\"username\" id=\"username\" value=\"" + user_data.Username + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
			}
			else
			{
                username.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"25\" name=\"username\" id=\"username\" value=\"" + user_data.Username + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">" + user_data.Username;
			}
            if (ThirdPartyAuthenticationEnabled(user_data.IsMemberShip))
			{
				TR_domain.Visible = true;
				TR_organization.Visible = false;
				TR_orgunit.Visible = false;
				TR_ldapdomain.Visible = false;
				TD_path.InnerHtml = "<input type=\"hidden\" name=\"userpath\" value=\"" + user_data.Path + "\">";
				TD_path.InnerHtml += "<input type=\"hidden\" name=\"domain\" value=\"" + user_data.Domain + "\">" + user_data.Domain;
			}
			else if (setting_data.ADAuthentication == 2)
			{
				TR_domain.Visible = false;
				TR_organization.Visible = false;
				TR_orgunit.Visible = false;
				TR_ldapdomain.Visible = true;
				//Dim arrOrg As Array
				//Dim arrCount As Long
				//Dim arrItem As Array
				//Dim strOrgUnit As String = ""
				//Dim strOrg As String = ""
				string strDC = user_data.Domain;
				//arrOrg = Split(user_data.Domain, ",")
				//For arrCount = LBound(arrOrg) To UBound(arrOrg)
				//    If (Not (arrOrg(arrCount) = "")) Then
				//        arrItem = Split(arrOrg(arrCount), "=")
				//        If (arrItem(0) = "o" Or arrItem(0) = " o") Then
				//            strOrg = arrItem(1)
				//        ElseIf (arrItem(0) = "ou" Or arrItem(0) = "ou") Then
				//            If (Not (strOrgUnit = "")) Then
				//                strOrgUnit &= ","
				//            End If
				//            strOrgUnit &= arrItem(1)
				//        ElseIf (arrItem(0) = "dc" Or arrItem(0) = " dc") Then
				//            If (Not (strDC = "")) Then
				//                strDC &= "."
				//            End If
				//            strDC &= arrItem(1)
				//        End If
				//    End If
				//Next
				//org.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""organization_text"" id=""organization_text"" value=""" & strOrg & """ onkeypress=""return CheckKeyValue(event,'34,32');"">"
				//orgunit.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""orgunit_text"" id=""orgunit_text"" value=""" & strOrgUnit & """ onkeypress=""return CheckKeyValue(event,'34,32');"">"
				ldapdomain.Text = "<input type=\"Text\" maxlength=\"255\" size=\"25\" name=\"ldapdomain_text\" id=\"ldapdomain_text\" value=\"" + strDC + "\" onkeypress=\"return CheckKeyValue(event,\'34,32\');\">";
			}
			else
			{
				TR_domain.Visible = false;
				TR_organization.Visible = false;
				TR_orgunit.Visible = false;
				TR_ldapdomain.Visible = false;
			}
			ltr_uid.Text = user_data.Id.ToString();
			if (security_data.IsAdmin && !(
                ThirdPartyAuthenticationEnabled(user_data.IsMemberShip)
                ))
			{
                firstname.Text = "<input type=\"Text\" maxlength=\"50\" size=\"25\" name=\"firstname\"  id=\"firstname\" value=\"" + (user_data.FirstName) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
				lastname.Text = "<input type=\"Text\" maxlength=\"50\" size=\"25\" name=\"lastname\" id=\"lastname\" value=\"" + (user_data.LastName) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
			}
			else
			{
                firstname.Text = "<input type=\"hidden\" name=\"specialCaseOverride\" id=\"specialCaseOverride\" value=\"1\" />";
                firstname.Text += "<input type=\"hidden\" maxlength=\"50\" size=\"25\" name=\"firstname\" id=\"firstname\" value=\"" + (user_data.FirstName) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">" + user_data.FirstName;
				lastname.Text = "<input type=\"hidden\" maxlength=\"50\" size=\"25\" name=\"lastname\" id=\"lastname\" value=\"" + (user_data.LastName) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">" + user_data.LastName;

			}
			displayname.Text = "<input type=\"Text\" maxlength=\"50\" size=\"25\" name=\"displayname\" id=\"displayname\" value=\"" + (user_data.DisplayName) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
            if (ThirdPartyAuthenticationEnabled(user_data.IsMemberShip))
			{
				hppwd.Text = "<td colspan=\"2\"><input type=\"hidden\" maxlength=\"255\" size=\"25\" name=\"pwd\" id=\"pwd\" value=\"" + user_data.Password + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
				hppwd.Text += "<input type=\"hidden\" name=\"hpwd\" id=\"hpwd\" value=\"" + user_data.Password + "\"</td>";
			}
			else
			{
				hppwd.Text = "<td class=\"label\"><span style=\"color:red;\">*</span>" + m_refMsg.GetMessage("password label") + "</td>";
				hppwd.Text += "<td class=\"value\"><input type=\"password\" maxlength=\"255\" size=\"25\" name=\"pwd\" id=\"pwd\" value=\"" + user_data.Password + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
				hppwd.Text += "<input type=\"hidden\" name=\"hpwd\" id=\"hpwd\" value=\"" + user_data.Password + "\">";
				hppwd.Text += "</td>";
			}
            if (ThirdPartyAuthenticationEnabled(user_data.IsMemberShip))
			{
				confirmpwd.Text = "<td colspan=\"2\"><input type=\"hidden\" maxlength=\"255\" size=\"25\" name=\"confirmpwd\" id=\"confirmpwd\" value=\"" + user_data.Password + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\"></td>";
			}
			else
			{
				confirmpwd.Text = "<td class=\"label\"><span style=\"color:red;\">*</span>" + m_refMsg.GetMessage("confirm pwd label") + "</td>";
				confirmpwd.Text += "<td class=\"value\"><input type=\"password\" maxlength=\"255\" size=\"25\" name=\"confirmpwd\" id=\"confirmpwd\" value=\"" + user_data.Password + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\"></td>";
			}
			
			if (ADIntegrationEnabledForUserType(user_data.IsMemberShip))
			{
				email.Text = "<td class=\"label\">" + m_refMsg.GetMessage("email address label") + "</td>";
				email.Text += "<td><input type=\"Hidden\" maxlength=\"255\" size=\"25\" name=\"email_addr1\" id=\"email_addr1\" value=\"" + user_data.Email + "\" onkeypress=\"return CheckKeyValue(event,\'34,32\');\">" + user_data.Email + "</td>";
			}
			else
			{
				email.Text = "<td class=\"label\">" + m_refMsg.GetMessage("email address label") + "</td>";
				email.Text += "<td class=\"value\"><input type=\"Text\" maxlength=\"255\" size=\"25\" name=\"email_addr1\" id=\"email_addr1\" value=\"" + user_data.Email + "\" onkeypress=\"return CheckKeyValue(event,\'34,32\');\"></td>";
			}
			if (m_intGroupType == 1)
			{
				email.Text += "<input type=hidden id=email_addr1Org name=email_addr1Org value=\"" + user_data.Email + "\">";
			}
			
			if ((this.m_refContentApi.RequestInformationRef.LoginAttempts != -1) && ((security_data.IsAdmin) || (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers))))
			{
				accountLocked.Text = "<input type=\"checkbox\" id=\"chkAccountLocked\" name=\"chkAccountLocked\" ";
				if (user_data.IsAccountLocked(this.m_refContentApi.RequestInformationRef))
				{
					accountLocked.Text += " checked ";
				}
				accountLocked.Text += " />";
			}
			else
			{
				accountLocked.Text = "<input type=\"hidden\" id=\"chkAccountLocked\" name=\"chkAccountLocked\" ";
				if (user_data.IsAccountLocked(this.m_refContentApi.RequestInformationRef))
				{
					accountLocked.Text += " value=\"on\" />";
				}
				else
				{
					accountLocked.Text += " value=\"\" />";
				}
			}
			
			ltr_avatar.Text = "<input type=\"Text\" maxlength=\"255\" size=\"19\" name=\"avatar\" id=\"avatar\" value=\"" + EkFunctions.HtmlEncode(user_data.Avatar) + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
			ltrmapaddress.Text = "<input type=\"Text\" maxlength=\"100\" size=\"19\" name=\"mapaddress\" id=\"mapaddress\" value=\"" + user_data.Address + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
			ltrmaplatitude.Text = "<input type=\"Text\" maxlength=\"100\" size=\"19\" name=\"maplatitude\" id=\"maplatitude\" value=\"" + user_data.Latitude + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\" disabled>";
			ltrmaplongitude.Text = "<input type=\"Text\" maxlength=\"100\" size=\"19\" name=\"maplongitude\" id=\"maplongitude\" value=\"" + user_data.Longitude + "\" onkeypress=\"return CheckKeyValue(event,\'34\');\" disabled>";
            ltr_upload.Text = "</asp:Literal>&nbsp;<a href=\"Upload.aspx?action=edituser&addedit=true&returntarget=avatar&EkTB_iframe=true&height=300&width=400&modal=true\" title=\"" + m_refMsg.GetMessage("upload txt") + "\" class=\"ek_thickbox button buttonInline greenHover buttonUpload btnUpload\" title=\"" + m_refMsg.GetMessage("upload txt") + "\">" + m_refMsg.GetMessage("upload txt") + "</a>";
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
                uxAvatarUploadIframe.Attributes.Add("src", context.WorkareaPath + "/Upload.aspx?action=edituser&addedit=true&returntarget=avatar&height=300&width=400&modal=true");
            }
            jsUxDialogSelectorTxt.Text = uxDialog.Selector.ToString();
            string uploadTxt = m_refMsg.GetMessage("upload txt");
            uxDialog.Title = uploadTxt;
            ltr_upload.Text = "</asp:Literal>&nbsp;<a href=\"#\" onclick=\"$ektron('" + uxDialog.Selector + "').dialog('open'); AvatarDialogInit();\" title=\"" + uploadTxt + "\" class=\"button buttonInline greenHover buttonUpload btnUpload\" title=\"" + uploadTxt + "\">" + uploadTxt + "</a>";
			
			//drp_editor.SelectedIndex = 0
			//drp_editor.Items.Add(New ListItem("eWebWP", "ewebwp"))
			drp_editor.Items.Add(new ListItem(m_refMsg.GetMessage("lbl content designer"), "contentdesigner"));
			//drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
			drp_editor.Items.Add(new ListItem("eWebEditPro", "ewebeditpro"));
			if ((string) (user_data.EditorOption.ToLower().Trim()) == "contentdesigner")
			{
				drp_editor.SelectedIndex = 0;
				//Case "jseditor"
				//    drp_editor.SelectedIndex = 1
			}
			else if ((string) (user_data.EditorOption.ToLower().Trim()) == "ewebeditpro")
			{
				drp_editor.SelectedIndex = 1;
				//Case "ewebwp"
				//    drp_editor.SelectedIndex = 0
			} // "ewebwp"
			else
			{
				drp_editor.SelectedIndex = 0; // default to contentdesigner
			}
			
			ctlEditor.Content = user_data.Signature;
			TD_personalTags.InnerHtml = GetPersonalTags();
			CreateColumns();
			if (_activityApi.IsActivityPublishingEnabled && (agentList != null)&& agentList.Count > 0 && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
			{
				LoadGrid("colleagues");
				LoadGrid("groups");
				EditPublishPreferencesGrid();
			}
			else
			{
				EkMembershipActivityTable.Visible = false;
				activitiesTab.Visible = false;
			}
			//Community alias Tab
			LoadCommunityAliasTab();
		}
		
		public string GetSignature()
		{
			if (ctlEditor != null)
			{
				m_sSignature = (string) ctlEditor.Content;
				// Remove SCRIPT elements to prevent XSS attacks, although ContentDesigner.AllowScripts="false" should prevent from getting this far.
				m_sSignature = Regex.Replace(m_sSignature, "\\<script[\\w\\W]+\\<\\/script\\>", "", RegexOptions.IgnoreCase);
			}
			return m_sSignature;
		}
        private string GetResourceText(string st)
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
		private void EditUserToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("edit user page title") + " \"" + user_data.DisplayUserName + "\""));
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "javascript:GoBackToCaller()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (user)"), m_refMsg.GetMessage("btn update"), "onclick=\"javascript:return SubmitForm(\'userinfo\', \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass,true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			
			if (m_intGroupType == 0)
			{
				result.Append(m_refStyle.GetHelpButton("edituseronly_ascx", ""));
			}
			else
			{
				result.Append(m_refStyle.GetHelpButton("EditMembershipUser", ""));
			}
			
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		#region Extending User Modal (Custom Properties)
		private void Display_UserCustomProperties()
		{
			Page.ClientScript.GetPostBackEventReference(litUCPUI, "");
			string strHtml = string.Empty;
			strHtml = m_refUserApi.EditUserCustomProperties(uId, false);
            StringBuilder sBuilder = new StringBuilder();
            Ektron.Cms.Community.MessageBoardAPI messageboardapi = new Ektron.Cms.Community.MessageBoardAPI();
            sBuilder.Append(strHtml);
            sBuilder.Append("<tr></tr><tr><td><div id=\"ek_MsgBoardModerationLabel\"><label class=\"label\">" + m_refMsg.GetMessage("lbl perm moderate") + ":" + "</label></div></td>\n");
            if (messageboardapi.IsModerated(uId, EkEnumeration.MessageBoardObjectType.User))
            {
                sBuilder.Append("<td><div id=\"ek_MsgBoardModeration\"><input type=\"checkbox\" id=\"ek_MsgBoardModerate\" name = \"ek_MsgBoardModerate\" checked=\"checked\"/>" + m_refMsg.GetMessage("lbl msgboard") + "<br/><span>" + m_refMsg.GetMessage("lbl usermsgboardnotify") + "</span></div></td></tr> \n");
            }
            else
            {
                sBuilder.Append("<td><div id=\"ek_MsgBoardModeration\"><input type=\"checkbox\" id=\"ek_MsgBoardModerate\"  name = \"ek_MsgBoardModerate\"/>" + m_refMsg.GetMessage("lbl msgboard") + "<br/><span>" + m_refMsg.GetMessage("lbl usermsgboardnotify") + "</span></div></td></tr> \n");
            }
            litUCPUI.Text = sBuilder.ToString();
		}
		#endregion
		
		private bool ADIntegrationEnabledForUserType(bool isMember)
		{
			if (! setting_data.ADIntegration)
			{
				return false;
			}
			else
			{
				if (! isMember)
				{
					return true;
				}
				else
				{
					return m_refContentApi.RequestInformationRef.LDAPMembershipUser;
				}
			}
		}

        private bool ThirdPartyAuthenticationEnabled(bool isMember)
        {
            EkEnumeration.UserTypes userType = (isMember ? EkEnumeration.UserTypes.MemberShipType : EkEnumeration.UserTypes.AuthorType);
            return (
                EkFunctions.ADAuthenticationEnabledForUserType(userType, (int)setting_data.ADAuthentication, m_refContentApi.RequestInformationRef.LDAPMembershipUser)
                ||
                EkFunctions.LDAPAuthenticationEnabledForUserType(userType, (int)setting_data.ADAuthentication, m_refContentApi.RequestInformationRef.LDAPMembershipUser)
                );
        }
		
		private string GetLocaleFileString(string localeFileNumber)
		{
			string LocaleFileString;
			if (localeFileNumber == "" || int.Parse(localeFileNumber) == 1)
			{
				LocaleFileString = "0000";
			}
			else
			{
                LocaleFileString = new string(Convert.ToChar("0"), 4 - Conversion.Hex(localeFileNumber).Length);
				LocaleFileString = LocaleFileString + Conversion.Hex(localeFileNumber);
				if (! System.IO.File.Exists(Server.MapPath(m_refContentApi.AppeWebPath + "locale" + LocaleFileString + "b.xml")))
				{
					LocaleFileString = "0000";
				}
			}
			return LocaleFileString.ToString();
		}
		
		#region Personal Tags
		public string GetPersonalTags()
		{
			string returnValue;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			TagData[] tdaUser;
			TagData[] tdaAll;
			TagData td;
			Hashtable htTagsAssignedToUser;
			
			try
			{
				
				error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag");
				error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars");
				
				htTagsAssignedToUser = new Hashtable();
				result.Append("<div id=\"newTagNameDiv\" class=\"ektronWindow\">");
				result.Append("<div></div>");
				result.Append(" <div class=\"ektronTopSpace\" style=\"margin-left: 15px !important;\">");
				result.Append("     <label class=\"tagLabel\" >" + (m_refMsg.GetMessage("name label") + "</label>&nbsp;&nbsp;<input type=\"text\" id=\"newTagName\" class=\"minWidth\" value=\"\" size=\"20\" onkeypress=\"if (event && event.keyCode && (13 == event.keyCode)) {SaveNewPersonalTag(); return false;}\" />"));
				result.Append(" </div>");
				
				if (IsSiteMultilingual)
				{
					result.Append("<div class=\"ektronTopSpace\" style=\"margin-left: 15px !important;\">");
				}
				else
				{
					result.Append("<div style=\"display:none;\" >");
				}
				result.Append("     <label class=\"tagLabel\">" + (m_refMsg.GetMessage("res_lngsel_lbl") + "</label>&nbsp;&nbsp;") + GetSiteEnabledLanguageDropDownMarkup("TagLanguage"));
				result.Append(" </div>");
				
				result.Append(" <div class=\"ektronTopSpace\">");
				result.Append("     <ul class=\"buttonWrapper ui-helper-clearfix\">");
				result.Append("         <li>");
				result.Append("             <a class=\"button redHover buttonClear buttonLeft\"  type=\"button\" value=\"" + m_refMsg.GetMessage("btn cancel") + "\" onclick=\"CancelSaveNewPersonalTag();\" >" + m_refMsg.GetMessage("btn cancel") + "</a>");
				result.Append("         </li>");
				result.Append("         <li>");
				result.Append("             <a class=\"button greenHover buttonUpdate buttonRight\"  style=\"margin-right:14px;\" type=\"button\" value=\"" + m_refMsg.GetMessage("btn save") + "\" onclick=\"SaveNewPersonalTag();\" >" + m_refMsg.GetMessage("btn save") + "</a>");
				result.Append("         </li>");
				result.Append("     </ul>");
				result.Append(" </div>");
				result.Append(" <input type=\"hidden\" id=\"newTagNameHdn\" name=\"newTagNameHdn\" value=\"\"  />");
				result.Append("</div>");
				result.Append("<div id=\"newTagNameScrollingDiv\" style=\'background-color: white;\'>");
				
				LocalizationAPI localizationApi = new LocalizationAPI();
				
				
				
				//create hidden list of current tags so we know to delete removed ones.
				foreach (LanguageData lang in languageDataArray)
				{
					result.Append("<input type=\"hidden\" id=\"flag_" + lang.Id + ("\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + "\"  />"));
				}
				result.Append("<input type=\"hidden\" id=\"flag_0\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(-1) + "\"  />");
				
				if (uId > 0)
				{
					tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForUser(uId, -1);
					StringBuilder appliedTagIds = new StringBuilder();
					
					//build up a list of tags used by user
					//add tags to hashtable for reference later when looping through defualt tag list
					if (tdaUser != null)
					{
						foreach (TagData tempLoopVar_td in tdaUser)
						{
							td = tempLoopVar_td;
							htTagsAssignedToUser.Add(td.Id, td);
							appliedTagIds.Append(td.Id.ToString() + ",");
							
							result.Append("<input checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
							result.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' border=\"0\" />");
							result.Append("&#160;" + td.Text + "<br />");
						}
					}
					
					//create hidden list of current tags so we know to delete removed ones.
					result.Append("<input type=\"hidden\" id=\"currentTags\" name=\"currentTags\" value=\"" + appliedTagIds.ToString() + "\"  />");
					
					tdaAll = (new Ektron.Cms.Community.TagsAPI()).GetDefaultTags(Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User, -1);
					if (tdaAll != null)
					{
						foreach (TagData tempLoopVar_td in tdaAll)
						{
							td = tempLoopVar_td;
							//don't add to list if its already been added with user's tags above
							if (! htTagsAssignedToUser.ContainsKey(td.Id))
							{
								result.Append("<input type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
								result.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' border=\"0\" />");
								result.Append("&#160;" + td.Text + "<br />");
							}
						}
					}
				}
				
				result.Append("<div id=\"newAddedTagNamesDiv\"></div>");
				result.Append("</div>");
				result.Append("<div class=\'ektronTopSpace\' style=\'float:left;\'>");
				result.Append(" <a class=\"button buttonLeft greenHover buttonAddTagWithText\" href=\"Javascript:ShowAddPersonalTagArea();\" title=\"" + m_refMsg.GetMessage("alt add btn text (personal tag)") + "\">" + m_refMsg.GetMessage("btn add personal tag") + "</a>");
				result.Append("</div>");
				
			}
			catch (Exception)
			{
			}
			finally
			{
				returnValue = result.ToString();
				tdaAll = null;
				tdaUser = null;
				td = null;
				htTagsAssignedToUser = null;
			}
			return returnValue;
		}
		
		private string GetSiteEnabledLanguageDropDownMarkup(string controlId)
		{
			
			int i;
			StringBuilder markup = new StringBuilder();
			
			if (IsSiteMultilingual)
			{
				markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\">");
				if (!(languageDataArray == null))
				{
					for (i = 0; i <= languageDataArray.Length - 1; i++)
					{
						if (languageDataArray[i].SiteEnabled)
						{
							markup.Append("<option ");
							if (languageDataArray[i].Id == m_refContentApi.DefaultContentLanguage)
							{
								markup.Append(" selected");
							}
							markup.Append(" value=" + languageDataArray[i].Id + ">" + languageDataArray[i].Name);
						}
					}
				}
				markup.Append("</select>");
			}
			else
			{
				//hardcode to default site language
				markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\" >");
				markup.Append(" <option selected value=" + m_refContentApi.DefaultContentLanguage + ">");
				markup.Append("</select>");
			}
			
			
			return markup.ToString();
		}
		
		private string GetAllLanguageDropDownMarkup(string controlId, string defaultMessage)
		{
			
			int i;
			StringBuilder markup = new StringBuilder();
			
			if (language_data == null)
			{
				language_data = m_refSiteApi.GetAllActiveLanguages();
			}
			
			markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\">");
			markup.Append("<option ");
			if (user_data.LanguageId == 0)
			{
				markup.Append(" selected");
			}
			markup.Append(" value=\"0\">" + defaultMessage);
			if (!(language_data == null))
			{
				for (i = 0; i <= language_data.Length - 1; i++)
				{
					markup.Append("<option ");
					if (language_data[i].Id == user_data.LanguageId)
					{
						markup.Append(" selected");
					}
					markup.Append(" value=" + language_data[i].Id + ">" + language_data[i].Name);
				}
			}
			markup.Append("</select>");
			
			return markup.ToString();
		}
		#endregion
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
			Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/ActivityStream/activityStream.css", "ActivityStream");
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");
			
		}
		private void LoadGrid(string display)
		{

            Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
			activityListCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
			
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
							dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" checked=\"checked\" /></center>";
						}
						else
						{
                            dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" /></center>";
						}
						if (CompareIds(activityTypeList[i].Id, 2))
						{
                            dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" checked=\"checked\"  /></center>";
						}
						else
						{
                            dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" /></center>";
							
						}

                        if (CompareIds(activityTypeList[i].Id, 3))
						{
                            dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" checked=\"checked\" /></center>";
						}
						else
						{
                            dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" /></center>";
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

			criteria.PagingInfo.RecordsPerPage = 10000;
			criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, uId);
			//Getting the Group Preference list
			groupPrefList = _notificationPreferenceApi.GetDefaultPreferenceList(criteria);
			//Getting the Colleagues preference list
			//need to set source to 0 because we dont want individual group prefs.
			criteria.AddFilter(NotificationPreferenceProperty.ActionSourceId, CriteriaFilterOperator.EqualTo, 0);
			preferenceList = _notificationPreferenceApi.GetList(criteria);
			//Adding the group list to Preferences
			preferenceList.AddRange(groupPrefList);
		}
		private void CreateColumns()
		{
            NotificationAgentCriteria criteria = new NotificationAgentCriteria();
			criteria.AddFilter(NotificationAgentProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
			agentList = _notificationAgentApi.GetList(criteria);
			
			if ((agentList != null)&& agentList.Count > 0)
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
		private void EditPublishPreferencesGrid()
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
					dr["ENABLED"] = "<center><input type=\"Checkbox\" name=\"pref" + prefEntry.ActivityTypeId + "\" id=\"pref" + prefEntry.ActivityTypeId + "\" checked=\"checked\"  /></center>";
				}
				else
				{
					dr["ENABLED"] = "<center><input type=\"Checkbox\" name=\"pref" + prefEntry.ActivityTypeId + "\" id=\"pref" + prefEntry.ActivityTypeId + "\" /></center>";
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