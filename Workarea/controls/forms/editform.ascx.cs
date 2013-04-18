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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Common;

	public partial class editform : System.Web.UI.UserControl
	{
		private Collection pagedata;
		private FormData form_data;
		private string m_strPageAction = "";
		private int ContentLanguage = 0;
		private long m_intFormId = 0;
		private Ektron.Cms.Modules.EkModule m_refModule;
		protected ContentAPI m_refContentApi = new ContentAPI();
		private SiteAPI m_refSiteApi = new SiteAPI();
		private string AppImgPath = "";
		protected EkMessageHelper m_refMsg;
		private StyleHelper m_refStyle = new StyleHelper();
		private PermissionData security_data;
		private long m_intFolderId = 0;
		private long CurrentUserId = 0;
		private int EnableMultilingual = 0;
		protected LanguageData language_data;
        protected FolderData folderdata = new FolderData();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			m_refMsg = m_refContentApi.EkMsgRef;
			SetJSResourceStrings();
			RegisterResources();
			if (!(Request.QueryString["action"] == null))
			{
				m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["folder_id"] == null))
			{
				m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
			}
			if (!string.IsNullOrEmpty(Request.QueryString["form_id"]))
			{
				m_intFormId = Convert.ToInt64(Request.QueryString["form_id"]);
			}
			if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
			{
				ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
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
			
			ltr_formTitle.Text = m_refMsg.GetMessage("msg form title");
			ltr_msgSubmission.Text = m_refMsg.GetMessage("alert msg no of submission");
			ltr_emailReq.Text = m_refMsg.GetMessage("msg email req");
			
			CurrentUserId = m_refContentApi.UserId;
			AppImgPath = m_refContentApi.AppImgPath;
			m_refModule = m_refContentApi.EkModuleRef;
		}
		public bool AddForm()
		{
			if (!(Page.IsPostBack))
			{
				frm_folder_id.Value = Convert.ToString(m_intFolderId);
				frm_copy_lang_from.Value = Request.QueryString["back_LangType"];
				language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
				if (Convert.ToString(EnableMultilingual) == "1")
				{
					lblLangName.Text = language_data.Name;
				}
				Display_AddForm();
			}
			else
			{
				Process_DoAdd();
			}
            return true;
		}
		public bool EditForm()
		{
			if (!(Page.IsPostBack))
			{
				language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
				if (Convert.ToString(EnableMultilingual) == "1")
				{
					lblLangName.Text = language_data.Name;
				}
				Display_EditForm();
			}
			else
			{
				Process_DoUpdate();
			}
            return true;
		}
		#region ADDFORM
		private void Display_AddForm()
		{
			//PostBackPage.Text = Utilities.SetPostBackPage("cmsform.aspx?LangType=" & ContentLanguage & "&Action=doAdd")
			string FormTitle = "";
			string FormDescription = "";
			string MailTo = "";
			string MailFrom = "";
			string MailCC = "";
			string MailPreamble = "";
			string MailSubject = "";
			string FormData = "";
			bool CanCreateContent = false;
			bool bCopyFromLang = false;
			FormTitle = Request.QueryString["form_title"];
			FormDescription = Request.QueryString["form_description"];
			MailTo = Request.QueryString["mail_to"];
			MailFrom = Request.QueryString["mail_from"];
			MailCC = Request.QueryString["mail_cc"];
			MailFrom = Request.QueryString["mail_from"];
			MailSubject = Request.QueryString["mail_subject"];
			MailPreamble = Request.QueryString["mail_preamble"];
			if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]) && !string.IsNullOrEmpty(Request.Form[frm_form_db.UniqueID]))
			{
				FormData = "both";
			}
			else if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]))
			{
				FormData = Request.Form[frm_form_mail.UniqueID];
			}
			else
			{
				FormData = Request.Form[frm_form_db.UniqueID];
			}
			frm_form_db.Checked = true;
			frm_form_af.Checked = true;
			security_data = m_refContentApi.LoadPermissions(m_intFolderId, "folder", 0);
			CanCreateContent = security_data.CanAdd;
			if (!string.IsNullOrEmpty(Request.QueryString["back_LangType"]) && !string.IsNullOrEmpty(Request.QueryString["form_id"]))
			{
				//load translated form title
				m_refContentApi.ContentLanguage = Convert.ToInt32(Request.QueryString["back_LangType"]);
				FormTitle = m_refContentApi.GetFormTitleById(m_intFormId);
				FormTitle = FormTitle + "_";
				bCopyFromLang = true;
			}
			AddToolBar();
			
			DisplayAssignTaskTo();
			
			frm_folder_id.Value = Convert.ToString(m_intFolderId);
			frm_copy_lang_from.Value = Request.QueryString["back_LangType"];
			frm_form_title.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(FormTitle);
			frm_form_description.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(FormDescription);
			frm_form_mailto.Value = MailTo;
			frm_form_mailfrom.Value = MailFrom;
			frm_form_mailcc.Value = MailCC;
			frm_form_mailpreamble.Value = MailPreamble;
			frm_form_mailsubject.Value = MailSubject;
			
			frm_initial_form.Value = "";
			frm_initial_response.Value = "";
			
			if (! bCopyFromLang)
			{
				string strFormsPath;
				string strManifestFilePath;
				string strXsltFilePath;
				frm_copy_lang_from.Value = "";
				strFormsPath = (string) (Server.MapPath(m_refContentApi.AppPath) + "controls\\forms\\");
				strManifestFilePath = Utilities.QualifyURL(strFormsPath, "InitialFormsManifest.xml");
				strXsltFilePath = Utilities.QualifyURL(strFormsPath, "SelectInitialForm.xslt");
				
				System.Xml.Xsl.XsltArgumentList objXsltArgs = null;
				if (m_refContentApi.ContentLanguage > 0)
				{
					LanguageData language_data;
					string strLang;
					language_data = m_refSiteApi.GetLanguageById(m_refContentApi.ContentLanguage);
					strLang = language_data.BrowserCode;
					if (strLang != "")
					{
						objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
						objXsltArgs.AddParam("lang", string.Empty, strLang);
					}
				}
				SelectInitialForm.Text = m_refContentApi.XSLTransform(strManifestFilePath, strXsltFilePath, true, true, objXsltArgs, false);
                if (!string.IsNullOrEmpty(SelectInitialForm.Text))
                {
                    if (SelectInitialForm.Text.Contains("Blank Form"))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Blank Form", m_refMsg.GetMessage("lbl Blank Form"));
                    }
                    if (SelectInitialForm.Text.Contains("Design a new form."))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Design a new form.", m_refMsg.GetMessage("lbl new form desc"));
                    }
                    if (SelectInitialForm.Text.Contains("Blank Survey"))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Blank Survey", m_refMsg.GetMessage("lbl Blank Survey"));
                    }
                    if (SelectInitialForm.Text.Contains("Standard Poll"))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Standard Poll", m_refMsg.GetMessage("lbl Standard Poll"));
                    }
                    if (SelectInitialForm.Text.Contains("Design a new survey."))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Design a new survey.", m_refMsg.GetMessage("lbl new survey desc"));
                    }
                    if (SelectInitialForm.Text.Contains("Design a new poll."))
                    {
                        SelectInitialForm.Text = SelectInitialForm.Text.Replace("Design a new poll.", m_refMsg.GetMessage("lbl new poll desc"));
                    }
                }
			}
		}
		private void Process_DoAdd()
		{
			long result = 0;
			string strFormOutput = "";
			try
			{
				pagedata = new Collection();
				pagedata.Add(Request.Form[frm_form_title.UniqueID], "FormTitle", null, null);
				pagedata.Add(Request.Form[frm_form_description.UniqueID], "FormDescription", null, null);
				if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]) && !string.IsNullOrEmpty(Request.Form[frm_form_db.UniqueID]))
				{
					strFormOutput = "both";
				}
				else if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]))
				{
					strFormOutput = Request.Form[frm_form_mail.UniqueID];
				}
				else
				{
					strFormOutput = Request.Form[frm_form_db.UniqueID];
				}
				if (Request.Form[frm_form_limit_submission.UniqueID] == "on")
				{
					if ( Request.Form[frm_form_number_of_submission.UniqueID] != null  && Information.IsNumeric(Request.Form[frm_form_number_of_submission.UniqueID]))
					{
						strFormOutput = strFormOutput + ",$" + Request.Form[frm_form_number_of_submission.UniqueID];
					}
					else
					{
						strFormOutput = strFormOutput + ",$1"; //default to 1
					}
				}
				pagedata.Add(strFormOutput, "FormOutput", null, null);
				pagedata.Add(!string.IsNullOrEmpty(Request.Form[frm_form_af.UniqueID]), "Autofill", null, null);
				pagedata.Add(Request.Form[frm_form_mailto.UniqueID], "MailTo", null, null);
				pagedata.Add(Request.Form[frm_form_mailcc.UniqueID], "MailCc", null, null);
				pagedata.Add(Request.Form[frm_form_mailfrom.UniqueID], "MailFrom", null, null);
				pagedata.Add(Request.Form[frm_form_mailsubject.UniqueID], "MailSubject", null, null);
				pagedata.Add(Request.Form[frm_form_mailpreamble.UniqueID], "MailPreamble", null, null);
				pagedata.Add(Request.Form[frm_folder_id.UniqueID], "FolderID", null, null);
				if (!string.IsNullOrEmpty(Request.Form[frm_send_xml_packet.UniqueID]))
				{
					pagedata.Add(1, "SendXmlPacket", null, null);
				}
				else
				{
					pagedata.Add(0, "SendXmlPacket", null, null);
				}
				if (!string.IsNullOrEmpty(Request.Form[frm_multi_form_id.UniqueID]))
				{
					pagedata.Add(Request.Form[frm_multi_form_id.UniqueID], "FormId", null, null);
				}
				else
				{
					pagedata.Add("0", "FormId", null, null);
				}
				
				string strCopyLang;
				string strFormType;
				strCopyLang = Request.Form[frm_copy_lang_from.UniqueID];
				strFormType = Request.Form[frm_form_type.UniqueID];
				pagedata.Add(strCopyLang, "RefLanguage", null, null);
				
				pagedata.Add(Request.Form[assigned_to_user_id.UniqueID], "AssignTaskToUser", null, null);
				pagedata.Add(Request.Form[assigned_to_usergroup_id.UniqueID], "AssignTaskToUserGroup", null, null);
				
				string strFormsPath;
				string strInitialForm;
				string strInitialResponse;
                string errorString = "";
				strFormsPath = (string) (Server.MapPath(m_refContentApi.AppPath) + "controls\\forms\\");
				strInitialForm = Request.Form[frm_initial_form.UniqueID];
				strInitialResponse = Request.Form[frm_initial_response.UniqueID];
                strInitialForm = m_refContentApi.GetFileContents(Utilities.QualifyURL(strFormsPath, strInitialForm), ref errorString);
                strInitialResponse = m_refContentApi.GetFileContents(Utilities.QualifyURL(strFormsPath, strInitialResponse), ref errorString);
				
				pagedata.Add(strInitialForm, "InitialForm", null, null);
				pagedata.Add(strInitialResponse, "InitialResponse", null, null);
				
				result = m_refModule.AddNewFormV42(pagedata); //ret as boolean
                IsContentSearchableSection(result);
				//Response.Redirect("cmsform.aspx?folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&LangType=" & ContentLanguage & "&Action=ViewForm&form_id=" & result, False)
				if (strCopyLang.Length > 0)
				{
					strCopyLang = (string) ("&copy_lang=" + strCopyLang);
				}
				if (strFormType.Length > 0)
				{
					strFormType = (string) ("&form_type=" + strFormType);
				}
				if (!(Request.QueryString["FromEE"] == null))
				{
					Response.Redirect((string) ("edit.aspx?new=true&close=true&LangType=" + ContentLanguage + strCopyLang + strFormType + "&id=" + result + "&type=update&back_file=cmsform.aspx&back_action=ViewForm&back_folder_id=" + Request.Form[frm_folder_id.UniqueID] + "&back_form_id=" + result + "&back_LangType=" + ContentLanguage), false);
				}
				else if (!(Request.QueryString["buttonid"] == null))
				{
					Response.Redirect((string) ("edit.aspx?new=true&close=false&LangType=" + ContentLanguage + strCopyLang + strFormType + "&id=" + result + "&type=update&back_file=&back_action=ViewForm&control=cbwidget&buttonid=" + Request.QueryString["buttonid"] + "&back_folder_id=" + Request.Form[frm_folder_id.UniqueID] + "&back_form_id=" + result + "&back_LangType=" + ContentLanguage), false);
				}
				else
				{
					Response.Redirect((string) ("edit.aspx?new=true&close=false&LangType=" + ContentLanguage + strCopyLang + strFormType + "&id=" + result + "&type=update&back_file=cmsform.aspx&back_action=ViewForm&back_folder_id=" + Request.Form[frm_folder_id.UniqueID] + "&back_form_id=" + result + "&back_LangType=" + ContentLanguage), false);
				}
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
			}
		}
		private void AddToolBar()
		{
			System.Text.StringBuilder result;
			string callBackPage = "";
			result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn add form"));
			result.Append("<table><tr>");
			if (security_data.CanAdd) //CanCreateContent
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", "Click here to save this Form", m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:$ektron(\'#pleaseWait\').modalShow(); return SubmitForm(\'myform\', \'VerifyAddForm()\');$ektron(\'#pleaseWait\').modalHide();\"", StyleHelper.SaveButtonCssClass, true));
			}
			callBackPage = m_refStyle.getCallBackupPage((string)("content.aspx?action=viewcontentbycategory&ID=" + m_intFolderId));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", callBackPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		#endregion
		#region EDITFORM
		private void Display_EditForm()
		{
			string strFormOutput = "";
			string strFormSubmission = "";
			
			EditFormPanel1.Visible = true;
			form_data = m_refContentApi.GetFormById(m_intFormId);
			frm_folder_id.Value = Convert.ToString(m_intFolderId);
			frm_copy_lang_from.Value = Request.QueryString["back_LangType"];
			lblFormId.Text = Convert.ToString(form_data.Id);
            frm_form_id.Value = Convert.ToString(form_data.Id);
			frm_form_title.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(form_data.Title);
			frm_form_description.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(form_data.Description);
			frm_form_mailto.Value = form_data.MailTo;
			if (ExtractFieldName(form_data.MailTo).Length > 0)
			{
				frm_form_mailto.Attributes.Add("readOnly", "true");
			}
			frm_form_mailfrom.Value = form_data.MailFrom;
			if (ExtractFieldName(form_data.MailFrom).Length > 0)
			{
				frm_form_mailfrom.Attributes.Add("readOnly", "true");
			}
			frm_form_mailcc.Value = form_data.MailCc;
			if (ExtractFieldName(form_data.MailCc).Length > 0)
			{
				frm_form_mailcc.Attributes.Add("readOnly", "true");
			}
			frm_form_mailpreamble.Value = form_data.MailPreamble;
			if (ExtractFieldName(form_data.MailPreamble).Length > 0)
			{
				frm_form_mailpreamble.Attributes.Add("readOnly", "true");
			}
			frm_form_mailsubject.Value = form_data.MailSubject;
			if (ExtractFieldName(form_data.MailSubject).Length > 0)
			{
				frm_form_mailsubject.Attributes.Add("readOnly", "true");
			}
			
			if (form_data.StoreDataTo.IndexOf(",$") > -1)
			{
				strFormOutput = m_refModule.IsEmailOrDb(form_data.StoreDataTo);
				strFormSubmission = form_data.StoreDataTo.Substring(System.Convert.ToInt32(form_data.StoreDataTo.IndexOf(",$") + 2));
			}
			else
			{
				strFormOutput = m_refModule.IsEmailOrDb(form_data.StoreDataTo);
			}
			
			//If ((form_data.StoreDataTo = "") Or (form_data.StoreDataTo = "mail") Or (form_data.StoreDataTo = "both")) Then
			if ((strFormOutput == "mail") || (strFormOutput == "both"))
			{
				frm_form_mail.Checked = true;
			}
			//If ((form_data.StoreDataTo = "") Or (form_data.StoreDataTo = "db") Or (form_data.StoreDataTo = "both")) Then
			if ((strFormOutput == "db") || (strFormOutput == "both"))
			{
				frm_form_db.Checked = true;
			}
			frm_form_af.Checked = form_data.Autofill;
			
			if (strFormSubmission != "")
			{
				frm_form_limit_submission.Checked = true;
				frm_form_number_of_submission.Value = strFormSubmission;
			}
			else
			{
				frm_form_limit_submission.Checked = false;
			}
			
			if (form_data.SendXmlPacket)
			{
				frm_send_xml_packet.Checked = true;
			}
			EditToolBar();
			DisplayAssignTaskTo();
			DisplayMailFieldSelectors(form_data);
		}
		private void Process_DoUpdate()
		{
			string strFormOutput = "";
			try
			{
				m_intFormId = Convert.ToInt64(Request.Form[frm_form_id.UniqueID]);
				pagedata = new Collection();
				pagedata.Add(m_intFormId, "FormID", null, null);
				pagedata.Add(Request.Form[frm_form_title.UniqueID], "FormTitle", null, null);
				pagedata.Add(m_intFolderId, "FolderID", null, null);
				pagedata.Add(Request.Form[frm_form_description.UniqueID], "FormDescription", null, null);
            if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]) && !string.IsNullOrEmpty(Request.Form[frm_form_db.UniqueID]))
				{
					strFormOutput = "both";
				}
            else if (!string.IsNullOrEmpty(Request.Form[frm_form_mail.UniqueID]))
				{
					strFormOutput = Request.Form[frm_form_mail.UniqueID];
				}
            else if (!string.IsNullOrEmpty(Request.Form[frm_form_db.UniqueID]))
				{
					strFormOutput = Request.Form[frm_form_db.UniqueID];
				}
            else
            {
                strFormOutput = "";
            }
				if (Request.Form[frm_form_limit_submission.UniqueID] == "on")
				{
					if (Request.Form[frm_form_number_of_submission.UniqueID] != null && Information.IsNumeric(Request.Form[frm_form_number_of_submission.UniqueID]))
					{
						strFormOutput = strFormOutput + ",$" + Request.Form[frm_form_number_of_submission.UniqueID];
					}
					else
					{
						strFormOutput = strFormOutput + ",$1"; //default to 1
					}
				}
				pagedata.Add(strFormOutput, "FormOutput", null, null);
            pagedata.Add(!string.IsNullOrEmpty(Request.Form[frm_form_af.UniqueID]), "Autofill", null, null);
				pagedata.Add(Request.Form[frm_form_mailto.UniqueID], "MailTo", null, null);
				pagedata.Add(Request.Form[frm_form_mailcc.UniqueID], "MailCc", null, null);
				pagedata.Add(Request.Form[frm_form_mailfrom.UniqueID], "MailFrom", null, null);
				pagedata.Add(Request.Form[frm_form_mailsubject.UniqueID], "MailSubject", null, null);
				pagedata.Add(Request.Form[frm_form_mailpreamble.UniqueID], "MailPreamble", null, null);
            if (!string.IsNullOrEmpty(Request.Form[frm_send_xml_packet.UniqueID]))
				{
					pagedata.Add(1, "SendXmlPacket", null, null);
				}
				else
				{
					pagedata.Add(0, "SendXmlPacket", null, null);
				}
				
				pagedata.Add(Request.Form[assigned_to_user_id.UniqueID], "AssignTaskToUser", null, null);
				pagedata.Add(Request.Form[assigned_to_usergroup_id.UniqueID], "AssignTaskToUserGroup", null, null);
				
				m_refModule.UpdateFormInfo(pagedata); //ret
				Response.Redirect((string) ("cmsform.aspx?folder_id=" + Request.Form[frm_folder_id.UniqueID] + "&LangType=" + ContentLanguage + "&Action=ViewForm&form_id=" + m_intFormId), false);
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
			}
		}
		private void EditToolBar()
		{
			System.Text.StringBuilder result;
			string callBackPage = m_refStyle.getCallBackupPage("cmsform.aspx?LangType=" + ContentLanguage + "&folder_id=" + form_data.FolderId + "&action=ViewAllForms");
			result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar("Edit Form" + " \"" + form_data.Title + "\"");
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", callBackPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", "Click here to save this Form", m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'myform\', \'VerifyAddForm()\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		#endregion
        private void IsContentSearchableSection(long formID)
        {
            Ektron.Cms.Content.EkContent m_refContent;
            m_refContent = m_refContentApi.EkContentRef;
            folderdata = m_refContent.GetFolderById(long.Parse(Request.Form[frm_folder_id.UniqueID]),false);
            if (folderdata != null)
            {
                pagedata = new Collection();
                pagedata.Add(formID.ToString(), "ContentID", null, null);
                pagedata.Add(false, "XmlInherited", null, null);
                pagedata.Add(Request.Form["xmlconfig"], "CollectionID", null, null);
                if (folderdata.IscontentSearchable)
                {
                    pagedata.Add(1, "IsSearchable", null, null);
                }
                else
                {
                    pagedata.Add(0, "IsSearchable", null, null);
                }

                long userID = m_refContent.RequestInformation.UserId;
                m_refContent.RequestInformation.CallerId = EkConstants.InternalAdmin;
                try
                {
                    m_refContent.UpdateContentProperties(pagedata);
                }
                finally
                {
                    m_refContent.RequestInformation.CallerId = userID;
                }
            }
        }
		private void DisplayAssignTaskTo()
		{
			try
			{
				System.Text.StringBuilder sbHtml = new System.Text.StringBuilder();
				bool bUnassigned = false;
				PermissionData security_task_data;
				security_task_data = m_refContentApi.LoadPermissions(m_intFormId, "tasks", ContentAPI.PermissionResultType.Task);
				Ektron.Cms.Content.EkTasks objTasks;
				Ektron.Cms.Content.EkTask cTask;
				cTask = m_refContentApi.EkTaskRef;
				
				cTask.AssignedByUserID = Convert.ToString(CurrentUserId);
				cTask.AssignedToUserID = -1;
				cTask.AssignToUserGroupID = -1;
				cTask.ContentID = m_intFormId;
				cTask.ContentLanguage = ContentLanguage;
				if (m_intFormId > 0)
				{
					// Existing form
					
					Ektron.Cms.PageRequestData null_EktronCmsPageRequestData = null;
					objTasks = cTask.GetTasks(m_intFormId, Convert.ToInt64(EkEnumeration.TaskState.Prototype), -1,Convert.ToInt32(EkEnumeration.CMSTaskItemType.TasksByStateAndContentID), "", 0, ref null_EktronCmsPageRequestData, "");
					if (objTasks != null&& objTasks.Count > 0)
					{
						cTask = objTasks.get_Item(1);
					}
				}
				
				content_id.Value = Convert.ToString(m_intFormId);
				current_language.Value = Convert.ToString(ContentLanguage);
				assigned_to_user_id.Value = "";
				if (cTask.AssignedToUserID > -1)
				{
					assigned_to_user_id.Value = Convert.ToString(cTask.AssignedToUserID);
				}
				assigned_to_usergroup_id.Value = "";
				if (cTask.AssignToUserGroupID > -1)
				{
					assigned_to_usergroup_id.Value = Convert.ToString(cTask.AssignToUserGroupID);
				}
				current_user_id.Value = Convert.ToString(CurrentUserId);
				assigned_by_user_id.Value = Convert.ToString(CurrentUserId);
				
				
				sbHtml.Append("<span id=\"user\" style=\"display:inline;\">");
                if (cTask != null && cTask.AssignToUserGroupID == 0)
				{
					sbHtml.Append("All Authors");
				}
                else if (cTask != null && !string.IsNullOrEmpty(cTask.AssignedToUser))
				{
					sbHtml.Append("<img src=\"" + AppImgPath + "../UI/Icons/user.png\" align=\"absbottom\">" + cTask.AssignedToUser.Replace("\'", "`"));
				}
                else if (cTask != null && !string.IsNullOrEmpty(cTask.AssignedToUserGroup))
				{
					sbHtml.Append("<img src=\"" + AppImgPath + "../UI/Icons/user.png\" align=\"absbottom\">" + cTask.AssignedToUserGroup.Replace("\'", "`"));
				}
				else
				{
					sbHtml.Append(m_refMsg.GetMessage("lbl unassigned"));
					bUnassigned = true;
				}
				sbHtml.Append("</span>");
				sbHtml.Append("&nbsp;");
				if (security_task_data.CanRedirectTask)
				{
					sbHtml.Append("<a href=\"#\" onclick=\"javascript:ShowUsers();\" class=\'selusers\'>" + m_refMsg.GetMessage("select user email report") + " </a>");
				}
				sbHtml.Append("<span id=\"idUnassignTask\" style=\"display:" + (bUnassigned ? "none" : "inline") + "\">&#160;&#160;&#160;<a href=\"#\" onclick=\"unassignTask();return false;\">" + m_refMsg.GetMessage("lbl unassign") + "</a></span>");
				sbHtml.Append("<div id=\"nouser\" style=\"display:none;\"></div>");
				AssignTaskTo.Text = sbHtml.ToString();
			}
			catch (Exception ex)
			{
				EkException.ThrowException(ex);
			}
		}
		
		private void DisplayMailFieldSelectors(FormData FormInfo)
		{
			try
			{
				string strSelector;
				string strFieldList;
				// Use non-breaking spaces (&#160;) to prevent ugly wrapping
				strFieldList = m_refModule.GetFormFieldListXml(FormInfo.Id);
				if (strFieldList.Length > 0)
				{
					strSelector = Convert.ToString(CreateSelectList(strFieldList, frm_form_mailto, ExtractFieldName(FormInfo.MailTo), "email emailList",""));
					if (strSelector.Length > 0)
					{
						litMailTo.Text = " OR&#160;to&#160;addresses&#160;in&#160;field:&#160;" + strSelector;
					}
                    strSelector = Convert.ToString(CreateSelectList(strFieldList, frm_form_mailfrom, ExtractFieldName(FormInfo.MailFrom), "email",""));
					if (strSelector.Length > 0)
					{
						litMailFrom.Text = " OR&#160;from&#160;address&#160;in&#160;field:&#160;" + strSelector;
					}
					strSelector = (string) (CreateSelectList(strFieldList, frm_form_mailcc, ExtractFieldName(FormInfo.MailCc), "email emailList",""));
					if (strSelector.Length > 0)
					{
						litMailCC.Text = " OR&#160;to&#160;addresses&#160;in&#160;field:&#160;" + strSelector;
					}
					strSelector = (string) (CreateSelectList(strFieldList, frm_form_mailsubject, ExtractFieldName(FormInfo.MailSubject), "text",""));
					if (strSelector.Length > 0)
					{
						litMailSubject.Text = " OR&#160;use&#160;text&#160;in&#160;field:&#160;" + strSelector;
					}
					strSelector = (string) (CreateSelectList(strFieldList, frm_form_mailpreamble, ExtractFieldName(FormInfo.MailPreamble), "text textbox",""));
					if (strSelector.Length > 0)
					{
						litMailMessageBody.Text = " OR&#160;use&#160;text&#160;in&#160;field:&#160;" + strSelector;
					}
				}
				else
				{
					litMailTo.Text = "";
					litMailFrom.Text = "";
					litMailCC.Text = "";
					litMailSubject.Text = "";
					litMailMessageBody.Text = "";
				}
			}
			catch (Exception ex)
			{
				EkException.ThrowException(ex);
			}
		}
		
		// Duplicate copy in EkModule.vb, editform.ascx.vb. Copied to keep it private.
		private string ExtractFieldName(string Text)
		{
			// Return empty string if not a field name
			// 171 = left (double) angle quote (guillemet)
			// 187 = right (double) angle quote (guillemet)
			if (Text == null)
			{
				return "";
			}
			if (Text.StartsWith(Strings.Chr(171).ToString()) && Text.EndsWith(Strings.Chr(187).ToString()))
			{
				return Text.Substring(1, Text.Length - 2);
			}
			else
			{
				return "";
			}
		}
		
		private string CreateSelectList(string FieldListXml, HtmlInputText AssociatedControl, string CurrentValue, string DataTypes, string BaseTypes)
		{
			string strSelectList = "";
			string strOptionList;
			string strFormsPath;
			string strXsltFilePath;
			try
			{
				strFormsPath = (string) (Server.MapPath(m_refContentApi.AppPath) + "controls\\forms\\");
				strXsltFilePath = Utilities.QualifyURL(strFormsPath, "SelectFormField.xslt");
				
				System.Xml.Xsl.XsltArgumentList objXsltArgs = null;
				if (CurrentValue.Length > 0)
				{
					if (objXsltArgs == null)
					{
						objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
					}
					objXsltArgs.AddParam("value", string.Empty, CurrentValue);
				}
				if (DataTypes.Length > 0)
				{
					if (objXsltArgs == null)
					{
						objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
					}
					objXsltArgs.AddParam("datatypes", string.Empty, DataTypes);
				}
				if (BaseTypes.Length > 0)
				{
					if (objXsltArgs == null)
					{
						objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
					}
					objXsltArgs.AddParam("basetypes", string.Empty, BaseTypes);
				}
            strOptionList = m_refModule.XSLTransform(FieldListXml, strXsltFilePath, true, false, objXsltArgs);
				if (strOptionList.Length > 0)
				{
					// useFieldValue depends on "_sel"
					strSelectList = strSelectList + "<select name=\"" + AssociatedControl.Name + "_sel\" id=\"" + AssociatedControl.ClientID + "_sel\" onchange=\"useFieldValue(this)\">" + "\r\n";
					strSelectList = strSelectList + "<option value=\"\">" + "(No field selected)" + "</option>" + "\r\n";
					strSelectList = strSelectList + strOptionList + "\r\n";
					strSelectList = strSelectList + "</select>" + "\r\n";
				}
			}
			catch (Exception ex)
			{
				EkException.ThrowException(ex);
			}
			return strSelectList;
		}
		
		private int StringToInt(string strValue, int DefaultValue)
		{
			try
			{
				return int.Parse(strValue);
			}
			catch (Exception)
			{
				return DefaultValue;
			}
		}
		protected void SetJSResourceStrings()
		{
			ltr_valemailaddr.Text = m_refMsg.GetMessage("enter valid email address or leave blank");
		}
		private void RegisterResources()
		{
			

            //CSS
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);

            //JS
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
		}
	}
