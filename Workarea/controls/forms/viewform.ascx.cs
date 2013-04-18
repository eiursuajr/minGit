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
using System.Xml;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.DataIO.LicenseManager;
using Ektron.Cms.Site;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Localization;
using Ektron.Cms.Modules;


	public partial class viewform : System.Web.UI.UserControl
	{
		
		private FormData form_data;
		private string m_strPageAction = "";
		private int ContentLanguage = 0;
		private long m_intFormId = 0;
		private EkContent m_refContent;
		private EkModule m_refModule;
		private ContentAPI m_refContentApi = new ContentAPI();
		private SiteAPI m_refSiteApi = new SiteAPI();
		private string AppImgPath = "";
		protected EkMessageHelper m_refMsg;
		private StyleHelper m_refStyle = new StyleHelper();
		private FolderData folder_data;
		private PermissionData security_data;
		private long m_intFolderId = 0;
		private long CurrentUserId = 0;
		private string VerifyTrue = "";
		private string VerifyFalse = "";
		private int EnableMultilingual = 0;
		private ContentData content_data;
		private bool TaskExists = false;
		protected EmailHelper m_refMailMsg = new EmailHelper();
		private bool m_bIsMac = false;
		private bool m_bIsMacInit = false;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			this.CreateChildControls();
			m_refMsg = m_refContentApi.EkMsgRef;
			RegisterResources();
			if (Request.QueryString["action"] != null)
			{
				if (Request.QueryString["action"] != "")
				{
					m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
				}
			}
			if (Request.QueryString["folder_id"] != null)
			{
				if (Request.QueryString["folder_id"] != "")
				{
					m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
				}
			}
			if (Request.QueryString["form_id"] != null)
			{
				if (Request.QueryString["form_id"] != "")
				{
					m_intFormId = Convert.ToInt64(Request.QueryString["form_id"]);
				}
			}
			if (Request.QueryString["LangType"] != null)
			{
				if (Request.QueryString["LangType"] != "")
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
			m_refModule = m_refContentApi.EkModuleRef;
			CurrentUserId = m_refContentApi.UserId;
			AppImgPath = m_refContentApi.AppImgPath;
			EnableMultilingual = m_refContentApi.EnableMultilingual;
			VerifyTrue = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/check.png\" border=\"0\" alt=\"Enabled\" title=\"Enabled\" />";
			VerifyFalse = "<img src=\"" + AppImgPath + "icon_redx.gif\" border=\"0\" alt=\"Disabled\" title=\"Disabled\" />";
			
			EmailArea.Text = m_refMailMsg.EmailJS();
			EmailArea.Text += m_refMailMsg.MakeEmailArea();
			
		}
		
		private void RegisterResources()
		{
			//'CSS
			//API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)
			Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronWamenuCss");
			//'JS
			//API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
			//API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
			//API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronWamenuJs");
		}
		
		public void ViewForm()
		{
			Display_ViewForm();
		}
		
		#region VIEWFORM
		private void Display_ViewForm()
		{
			bool CanIAddLang = true;
			bool TaskExists = false;
			PermissionData security_task_data;
			m_refContent = m_refContentApi.EkContentRef;
			//Check  to see if it's passing it by content-id
			if (Request.QueryString["form_content_id"] != null && Request.QueryString["form_content_id"] != "")
			{
				m_intFormId = Convert.ToInt64(Request.QueryString["form_content_id"]);
				form_data = m_refContentApi.GetFormById(m_intFormId);
			}
			else
			{
				form_data = m_refContentApi.GetFormById(m_intFormId);
			}
			if (form_data == null)
			{
				throw (new Exception("Unable to view form. ID=" + m_intFormId));
			}
			
			if (Request.QueryString["staged"] != null && Request.QueryString["staged"] != "")
			{
				content_data = m_refContentApi.GetContentById(form_data.Id, ContentAPI.ContentResultType.Staged);
			}
			else
			{
				content_data = m_refContentApi.GetContentById(form_data.Id, 0);
			}
			TaskExists = m_refContent.DoesTaskExistForContent(form_data.Id);
			security_task_data = m_refContentApi.LoadPermissions(form_data.Id, "tasks", ContentAPI.PermissionResultType.Task);
			security_data = m_refContentApi.LoadPermissions(form_data.Id, "content", ContentAPI.PermissionResultType.All);
			security_data.CanAddTask = security_task_data.CanAddTask;
			security_data.CanDestructTask = security_task_data.CanDestructTask;
			security_data.CanRedirectTask = security_task_data.CanRedirectTask;
			security_data.CanDeleteTask = security_task_data.CanDeleteTask;
			CanIAddLang = security_data.CanAdd;
			// Replace [srcpath] with actual path. [srcpath] is used in calendar field.
			if (content_data.Html != null&& content_data.Html.Length > 0)
			{
				td_vf_content.InnerHtml = content_data.Html.Replace("[srcpath]", m_refContentApi.ApplicationPath + m_refContentApi.AppeWebPath);
				td_vf_content.InnerHtml = td_vf_content.InnerHtml.Replace("[skinpath]", m_refContentApi.ApplicationPath + "csslib/ContentDesigner/");
			}
			else
			{
				td_vf_content.InnerHtml = content_data.Html;
			}
			if (content_data.Teaser.Length > 0)
			{
				td_vf_summary.InnerHtml = DisplayFormDesign(content_data.Teaser);
			}
			else
			{
				td_vf_summary.InnerHtml = "<p>" + m_refMsg.GetMessage("lbl place post back message here") + "</p>";
			}
			ViewToolBar();
			Populate_ViewForm(form_data);
		}
		
		private string DisplayFormDesign(string Teaser)
		{
			bool bIsRedirect = false;
			bIsRedirect = System.Convert.ToBoolean(Teaser.IndexOf("<RedirectionLink") > -1);
			if (bIsRedirect)
			{
				if (Teaser.IndexOf("<EktReportFormData") > -1)
				{
					XmlDocument doc = new XmlDocument();
					XmlNode node;
					XmlAttribute attr;
					string sDisplayType = "";
					string sReportType = "bar chart";
					StringBuilder sbHtml = new StringBuilder();
					sbHtml.Append(m_refMsg.GetMessage("lbl report on the form") + ":" + "<br />");
					doc.LoadXml(Teaser);
					node = doc.SelectSingleNode("//a[1]");
					attr = node.Attributes["id"];
					if (attr != null)
					{
  
                        if ((attr.Value.ToLower()) == Convert.ToString(EkEnumeration.CMSFormReportType.Pie))
						{
							sReportType = "Pie chart";
						}
						else if ((attr.Value.ToLower()) == Convert.ToString(EkEnumeration.CMSFormReportType.DataTable))
						{
							sReportType = "Data table";
						}
						else if ((attr.Value.ToLower()) == Convert.ToString(EkEnumeration.CMSFormReportType.Combined))
						{
							sReportType = "Combined chart";
						}
						else if ((attr.Value.ToLower()) == Convert.ToString(EkEnumeration.CMSFormReportType.Bar))
						{
							sReportType = "Bar chart";
						}
						else
						{
							sReportType = "Bar chart";
						}
						sbHtml.Append(sReportType + " " + m_refMsg.GetMessage("lbl will be displayed on") + " ");
					}
					attr = null;
					attr = node.Attributes["target"];
					if (attr != null)
					{
						if ("_self" == attr.Value.ToLower())
						{
							sDisplayType = m_refMsg.GetMessage("lbl the same window");
						}
						else
						{
							sDisplayType = m_refMsg.GetMessage("lbl a new window");
						}
						sbHtml.Append(sDisplayType + ".");
					}
					return sbHtml.ToString();
				}
				else
				{
                    return m_refMsg.GetMessage("lbl form response redirection");
				}
				//Return Teaser.Replace("<a", "<a id=""RedirectionLink""") & vbCrLf & _
				//"<script language=""JavaScript"" type=""text/javascript"" src=""" & m_refContentApi.AppeWebPath() & "java/redirectlink.js""></script>" & vbCrLf
			}
			else
			{
				string strDesign;
				//Dim strDisplay As String
				if (Teaser.IndexOf("<ektdesignpackage_design") > -1)
				{
                    strDesign = m_refContentApi.XSLTransform(Teaser, Server.MapPath(m_refContentApi.AppeWebPath + "unpackageDesign.xslt"), true, false, null, true);
					return strDesign;
				}
				else
				{
					return Teaser;
				}
			}
		}
		
		private void Populate_ViewForm(FormData form_data)
		{
			ViewFormPropertiesGrid.AutoGenerateColumns = false;
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "NAME";
			colBound.Initialize();
			colBound.ItemStyle.CssClass = "label";
			colBound.HeaderStyle.Height = Unit.Empty;
			ViewFormPropertiesGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			ViewFormPropertiesGrid.Columns.Add(colBound);
			ViewFormPropertiesGrid.BackColor = System.Drawing.Color.White;
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("NAME", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl form title") + ":";
			dr[1] = form_data.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl formid") + ":";
			dr[1] = form_data.Id;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("tab linkcheck status") + ":";
			switch (form_data.Status.ToLower())
			{
				case "a":
					dr[1] = m_refMsg.GetMessage("status:Approved (Published)");
					break;
				case "o":
					dr[1] = m_refMsg.GetMessage("status:Checked Out");
					break;
				case "i":
					dr[1] = m_refMsg.GetMessage("status:Checked In");
					break;
				case "p":
					dr[1] = m_refMsg.GetMessage("status:Approved (PGLD)");
					break;
				case "m":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
					break;
				case "s":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
					break;
				case "t":
					dr[1] = m_refMsg.GetMessage("status:Waiting Approval");
					break;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("description label");
			dr[1] = form_data.Description;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl form data") + ":";
			bool bMail = false;
			bool bDatabase = false;
			bool bAutofillForm = false;
			int lPos = 0;
			string strFormData = "";
			string strFormSubmission = "";
			
			bAutofillForm = form_data.Autofill;
			lPos = form_data.StoreDataTo.IndexOf(",$");
			if (lPos > -1)
			{
				strFormData = m_refModule.IsEmailOrDb(form_data.StoreDataTo);
				strFormSubmission = form_data.StoreDataTo.Substring(lPos + 2);
			}
			else
			{
				strFormData = form_data.StoreDataTo;
			}
			
			//If (strFormData = "") Then
			//    bMail = True
			//End If
			if (strFormData == "mail")
			{
				bMail = true;
			}
			if (strFormData == "db")
			{
				bDatabase = true;
			}
			if (strFormData == "both")
			{
				bMail = true;
				bDatabase = true;
			}
			
			if (bMail == true)
			{
				dr[1] = VerifyTrue;
			}
			else
			{
				dr[1] = VerifyFalse;
			}
			
			dr[1] += "" + m_refMsg.GetMessage("lbl mail") + "&nbsp;&nbsp;&nbsp;";
			
			if (bDatabase == true)
			{
				dr[1] += VerifyTrue;
			}
			else
			{
				dr[1] += VerifyFalse;
			}
			dr[1] += m_refMsg.GetMessage("lbl database");
			
			if (bDatabase == true)
			{
				dr[1] += "&nbsp;&nbsp;&nbsp;";
				if (bAutofillForm == true)
				{
					dr[1] += VerifyTrue;
				}
				else
				{
					dr[1] += VerifyFalse;
				}
				dr[1] += m_refMsg.GetMessage("lbl autofill form values");
			}
			
			dt.Rows.Add(dr);
			
			// Display Limit submission information if there is any
			if (strFormSubmission != "")
			{
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("lbl form submission");
				dr[1] = strFormSubmission;
				dt.Rows.Add(dr);
			}
			
			EkTasks objTasks;
			EkTask cTask;
			cTask = m_refContentApi.EkTaskRef;
			
			Ektron.Cms.PageRequestData null_EktronCmsPageRequestData = null;
			objTasks = cTask.GetTasks(form_data.Id, Convert.ToInt64(EkEnumeration.TaskState.Prototype), -1, Convert.ToInt32(EkEnumeration.CMSTaskItemType.TasksByStateAndContentID), "", 0, ref null_EktronCmsPageRequestData, "");
			if (objTasks != null&& objTasks.Count > 0)
			{
				cTask = objTasks.get_Item(1);
			}
			
			string strAssignTaskTo;
			if (cTask.AssignToUserGroupID == 0)
			{
				strAssignTaskTo = m_refMsg.GetMessage("lbl all authors");
			}
            else if (!string.IsNullOrEmpty(cTask.AssignedToUser))
			{
				strAssignTaskTo = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" align=\"absbottom\" />";
				strAssignTaskTo += m_refMailMsg.MakeUserTaskEmailLink(cTask, false);
			}
			else if (!string.IsNullOrEmpty(cTask.AssignedToUserGroup))
			{
				strAssignTaskTo = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" align=\"absbottom\" />";
				strAssignTaskTo += m_refMailMsg.MakeUserGroupTaskEmailLink(cTask);
			}
			else
			{
				strAssignTaskTo = m_refMsg.GetMessage("lbl unassigned");
			}
			if (strAssignTaskTo.Length > 0)
			{
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("lbl assign task to") + ":";
				dr[1] = strAssignTaskTo;
				dt.Rows.Add(dr);
			}
			
			if (strFormData == "mail" || strFormData == "both")
			{
				dr = dt.NewRow();
				dr[0] = "<hr />";
				dr[1] = "info-header";
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = " " + m_refMsg.GetMessage("lbl mailproperties"); //"<span class=""info-header""> Mail Properties</span>"
				dr[1] = "info-header";
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic to label");
				dr[1] = form_data.MailTo;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic from label");
				dr[1] = form_data.MailFrom;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic cc label");
				dr[1] = form_data.MailCc;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic subject label");
				dr[1] = form_data.MailSubject;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl preamble") + ":";
				dr[1] = form_data.MailPreamble;
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("alt send data in xml format") + ":";
				if (form_data.SendXmlPacket)
				{
					dr[1] = m_refMsg.GetMessage("generic yes");
				}
				else
				{
					dr[1] = m_refMsg.GetMessage("generic no");
				}
				dt.Rows.Add(dr);
			}
			
			dr = dt.NewRow();
			dr[0] = "<hr />";
			dr[1] = "info-header";
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = " " + m_refMsg.GetMessage("btn content properties"); //"<span class=""info-header""> Content Properties</span>"
			dr[1] = "info-header";
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("content title label");
			dr[1] = form_data.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("content id label");
			dr[1] = form_data.Id;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("tab linkcheck status") + ":";
			switch (form_data.Status.ToLower())
			{
				case "a":
					dr[1] = m_refMsg.GetMessage("status:Approved (Published)");
					break;
				case "o":
					dr[1] = m_refMsg.GetMessage("status:Checked Out");
					break;
				case "i":
					dr[1] = m_refMsg.GetMessage("status:Checked In");
					break;
				case "p":
					dr[1] = m_refMsg.GetMessage("status:Approved (PGLD)");
					break;
				case "m":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
					break;
				case "s":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
					break;
				case "t":
					dr[1] = m_refMsg.GetMessage("status:Waiting Approval");
					break;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic last editor") + ":";
			dr[1] = form_data.EditorFirstName + " " + form_data.EditorLastName;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic start date label");
			if (string.IsNullOrEmpty(form_data.DisplayGoLive))
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = form_data.DisplayGoLive;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("generic end date label");
			if (string.IsNullOrEmpty(form_data.DisplayEndDate))
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = form_data.DisplayEndDate;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("End Date Action Title") + ":";
            if (content_data != null)
            {
                if (content_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
                {
                    dr[1] = m_refMsg.GetMessage("Archive display descrp");
                }
                else if (content_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
                {
                    dr[1] = m_refMsg.GetMessage("Refresh descrp");
                }
                else
                {
                    dr[1] = m_refMsg.GetMessage("Archive expire descrp");
                }
            }
            else if (form_data.DisplayEndDate.Length > 0)
            {

                if (form_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
                {
                    dr[1] = m_refMsg.GetMessage("Archive display descrp");
                }
                else if (form_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
                {
                    dr[1] = m_refMsg.GetMessage("Refresh descrp");
                }
                else
                {
                    dr[1] = m_refMsg.GetMessage("Archive expire descrp");
                }
            }
            else
            {
                dr[1] = m_refMsg.GetMessage("none specified msg");
            }
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("content dc label");
			if (string.IsNullOrEmpty(form_data.DisplayDateCreated))
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = form_data.DisplayDateCreated;
			}
			dt.Rows.Add(dr);
			
			DataView dv = new DataView(dt);
			ViewFormPropertiesGrid.DataSource = dv;
			ViewFormPropertiesGrid.DataBind();
		}
		protected void ViewFormPropertiesGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			switch (e.Item.ItemType)
			{
				case ListItemType.AlternatingItem:
				case ListItemType.Item:
					if (e.Item.Cells[1].Text.Equals("info-header"))
					{
						e.Item.Cells[0].Attributes.Add("align", "Left");
						e.Item.Cells[0].ColumnSpan = 2;
						e.Item.Cells[0].CssClass = "info-header";
						e.Item.Cells.RemoveAt(1);
					}
					break;
			}
		}
		private void ViewToolBar()
		{
			System.Text.StringBuilder result;
			string strBackPage = "";
			ContentStateData content_state_data;
			result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view forms title") + " \"" + form_data.Title + "\""));
			result.Append("<table><tr>");
			strBackPage = (string) ("LangType=" + ContentLanguage + "&Action=ViewForm&form_id=" + m_intFormId);
			strBackPage = EkFunctions.UrlEncode(strBackPage);

			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + form_data.FolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

			bool appliedPrimaryCss = false;

			if (security_data.CanEdit)
			{
				// Currently, we do not support editing forms on the Mac:
				//If (Not IsMac()) Then  'Editing forms is now supported on MAC so making the change rquested in #34748
				result.Append(m_refStyle.GetEditAnchor(form_data.Id, 2, false, EkEnumeration.CMSContentSubtype.Content, !appliedPrimaryCss) + "\r\n");
				appliedPrimaryCss = true;
				
				if (form_data.Status == "O")
				{
					if (security_data.IsAdmin)
					{
						//this is the adim so allow for the check in button
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/checkIn.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + form_data.Id + "&fldid=" + form_data.FolderId + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + form_data.Id), m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "OnClick=\"DisplayHoldMsg(true);return true;\"", StyleHelper.CheckInButtonCssClass, !appliedPrimaryCss));

						appliedPrimaryCss = true;
					}
					else
					{
						// go find out the state of this contet to see it this user can check it in.
						content_state_data = m_refContentApi.GetContentState(form_data.Id);
						if (content_state_data.CurrentUserId == m_refContentApi.UserId)
						{
							result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/checkIn.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + form_data.Id + "&fldid=" + form_data.FolderId + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + form_data.Id), m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "OnClick=\"DisplayHoldMsg(true);return true;\"", StyleHelper.CheckInButtonCssClass, !appliedPrimaryCss));

							appliedPrimaryCss = true;
						}
					}
				}
				else if (((form_data.Status == "I") || (form_data.Status == "T")) && (content_data.UserId == m_refContentApi.UserId))
				{
					if (security_data.CanPublish)
					{
                        bool metaRequuired = false;
                        bool categoryRequired = false;
                        bool manaliasRequired = false;
                        string msg = string.Empty;
                        m_refContentApi.EkContentRef.ValidateMetaDataTaxonomyAndAlias(content_data.FolderId, content_data.Id, content_data.LanguageId, ref metaRequuired, ref categoryRequired, ref manaliasRequired);
                        if (metaRequuired == false && categoryRequired == false && manaliasRequired == false)
                        {
							result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentPublish.png", (string) ("content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + form_data.Id + "&fldid=" + form_data.FolderId + "&page=workarea&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId + "&parm3=LangType&value3=" + ContentLanguage), m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "OnClick=\"DisplayHoldMsg(true);return true;\"", StyleHelper.PublishButtonCssClass, !appliedPrimaryCss));

							appliedPrimaryCss = true;
						}
						else
						{
							if (metaRequuired && categoryRequired && manaliasRequired)
							{
								msg = m_refMsg.GetMessage("validate meta and manualalias and category required");
							}
							else if (metaRequuired && categoryRequired && !manaliasRequired)
							{
								msg = m_refMsg.GetMessage("validate meta and category required");
							}
							else if (metaRequuired && !categoryRequired && manaliasRequired)
							{
								msg = m_refMsg.GetMessage("validate meta and manualalias required");
							}
							else if (!metaRequuired && categoryRequired && manaliasRequired)
							{
								msg = m_refMsg.GetMessage("validate manualalias and category required");
							}
							else if (metaRequuired)
							{
								msg = m_refMsg.GetMessage("validate meta required");
							}
							else if (manaliasRequired)
							{
								msg = m_refMsg.GetMessage("validate manualalias required");
							}
							else
							{
								msg = m_refMsg.GetMessage("validate category required");
							}
							result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentPublish.png", "#", m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "onclick=\"alert(\'" + msg + "\');\"", StyleHelper.PublishButtonCssClass, !appliedPrimaryCss));

							appliedPrimaryCss = true;
						}
					}
					else
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + form_data.Id + "&fldid=" + form_data.FolderId + "&page=workarea&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId + "&parm3=LangType&value3=" + ContentLanguage), m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "OnClick=\"DisplayHoldMsg(true);return true;\"", StyleHelper.SubmitForApprovalButtonCssClass, !appliedPrimaryCss));

						appliedPrimaryCss = true;
					}
				}

				if (form_data.Status == "S" || form_data.Status == "I" || form_data.Status == "T" || form_data.Status == "O" || form_data.Status == "P")
				{
					if (Request.QueryString["staged"] != "")
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("cmsform.aspx?LangType=" + ContentLanguage + "&action=ViewForm&form_id=" + m_intFormId + "&folder_id=" + form_data.FolderId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass, !appliedPrimaryCss));
					}
					else
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("cmsform.aspx?LangType=" + ContentLanguage + "&action=ViewForm&form_id=" + m_intFormId + "&folder_id=" + form_data.FolderId + "&staged=true&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&value2=form_id&value2=" + m_intFormId), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass, !appliedPrimaryCss));
					}

					appliedPrimaryCss = true;
				}
			}
			if (form_data.Status == "S" || form_data.Status == "M")
			{
				if (security_data.CanEditSumit)
				{
					// Don't show edit button for Mac when using XML config:
					string SelectedEditControl = Utilities.GetEditorPreference(Request);
					if (!(m_bIsMac && (content_data.XmlConfiguration != null)) || SelectedEditControl == "ContentDesigner")
					{
						result.Append(m_refStyle.GetEditAnchor(form_data.Id, 2, true, EkEnumeration.CMSContentSubtype.Content, !appliedPrimaryCss) + "\r\n");

						appliedPrimaryCss = true;
					}
				}
			}
			if (security_data.CanHistory)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/history.png", "#", m_refMsg.GetMessage("alt history button text"), m_refMsg.GetMessage("lbl generic history"), "OnClick=\"top.document.getElementById(\'ek_main\').src=\'historyarea.aspx?action=report&LangType=" + ContentLanguage + "&id=" + form_data.Id + "\';return false;\"", StyleHelper.HistoryButtonCssClass, !appliedPrimaryCss));

				appliedPrimaryCss = true;
			}
			if (form_data.Status == "S" || form_data.Status == "I" || form_data.Status == "T" || form_data.Status == "O")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewDifferences.png", "#", m_refMsg.GetMessage("alt view difference"), m_refMsg.GetMessage("btn view diff"), "onclick=\"PopEditWindow(\'compare.aspx?LangType=" + ContentLanguage + "&id=" + form_data.Id + "\', \'Compare\', 785, 650, 1, 1);\"", StyleHelper.ViewDifferenceButtonCssClass, !appliedPrimaryCss));

				appliedPrimaryCss = true;
			}
			if (security_data.CanEdit)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("cmsform.aspx?LangType=" + ContentLanguage + "&action=Editform&form_id=" + m_intFormId + "&folder_id=" + form_data.FolderId + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId), m_refMsg.GetMessage("alt form prop"), m_refMsg.GetMessage("btn edit prop"), "", StyleHelper.EditButtonCssClass, !appliedPrimaryCss));

				appliedPrimaryCss = true;
			}
			
			if (security_data.CanDelete)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=submitDelContAction&delete_id=" + form_data.Id + "&folder_id=" + form_data.FolderId + "&form_id=" + form_data.Id + "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=id&value2=" + form_data.FolderId), m_refMsg.GetMessage("alt del form"), m_refMsg.GetMessage("btn delete"), "onclick=\"return ConfirmFormDelete();\"", StyleHelper.DeleteButtonCssClass, !appliedPrimaryCss));
			}
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/linkSearch.png", (string)("isearch.aspx?LangType=" + ContentLanguage + "&action=dofindcontent&folderid=0&form_id=" + m_intFormId + "&ObjectType=forms" + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId), m_refMsg.GetMessage("alt Check for content that is linked to this"), m_refMsg.GetMessage("btn link search"), "", StyleHelper.SearchButtonCssClass, !appliedPrimaryCss));
			appliedPrimaryCss = true;

			result.Append(StyleHelper.ActionBarDivider);

			if (security_data.CanAddTask)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/taskAdd.png", (string) ("tasks.aspx?action=AddTask&cid=" + form_data.Id + "&LangType=" + ContentLanguage + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId + "&parm3=folder_id&value3=" + form_data.FolderId + "&parm4=LangType&value4=" + ContentLanguage), m_refMsg.GetMessage("btn add task"), m_refMsg.GetMessage("btn add task"), "", StyleHelper.AddTaskButtonCssClass));
			}
			if (TaskExists == true)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_viewtask-nm.gif", (string) ("tasks.aspx?LangType=" + ContentLanguage + "&action=viewcontenttask&ty=both&cid=" + form_data.Id + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId + "&parm3=folder_id&value3=" + form_data.FolderId + "&parm4=LangType&value4=" + ContentLanguage), m_refMsg.GetMessage("btn view task"), m_refMsg.GetMessage("btn view task"), "", StyleHelper.ViewTaskButtonCssClass));
			}
			
			// Prep-work for adding move-forms capability:
			//If (security_data.IsAdmin) And (content_data.Status = "A") Then
			//    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & ../UI/Icons/contentCopy.png.png, "content.aspx?LangType=" & ContentLanguage & "&action=MoveContent&id=" & m_intFolderId & "&folder_id=" & content_data.FolderId, "Move Content", m_refMsg.GetMessage("btn move content"), ""))
			//End If
			
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/chartBar.png", (string) ("cmsformsreport.aspx?LangType=" + ContentLanguage + "&id=" + m_intFormId + "&FormTitle=" + form_data.Title + "&folder_id=" + form_data.FolderId), m_refMsg.GetMessage("alt report"), m_refMsg.GetMessage("btn report"), "", StyleHelper.ViewReportButtonCssClass));
			string strAction;
			string propertiesCssClass;
			if (Utilities.IsMac())
			{
				strAction = "EditContentProperties";
				propertiesCssClass = StyleHelper.EditPropertiesButtonCssClass;
			}
			else
			{
				strAction = "View";
				propertiesCssClass = StyleHelper.ViewPropertiesButtonCssClass;
			}
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/properties.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=" + strAction + "&id=" + form_data.Id + "&callerpage=cmsform.aspx&folder_id=" + form_data.FolderId + "&origurl=" + strBackPage), m_refMsg.GetMessage("generic form prop"), m_refMsg.GetMessage("btn content properties"), "", propertiesCssClass));
			
			//Sync API needs to know folder type to display the eligible sync profiles.
			if (folder_data == null)
			{
				folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
			}
			
			SiteAPI site = new SiteAPI();
			EkSite ekSiteRef = site.EkSiteRef;
			if ((m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncUser)) && LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Feature.eSync) && m_refContentApi.RequestInformationRef.IsSyncEnabled)
			{
				if ((m_strPageAction == "viewform") && (content_data.Status.ToUpper() == "A") && ServerInformation.IsStaged())
				{
					if (folder_data.IsDomainFolder)
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=\"Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(" + ContentLanguage + "," + m_intFormId + ",\'" + content_data.AssetData.Id + "\',\'" + content_data.AssetData.Version + "\'," + content_data.FolderId + ",true);return false;\"", StyleHelper.SyncButtonCssClass));
					}
					else
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=\"Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(" + ContentLanguage + "," + m_intFormId + ",\'" + content_data.AssetData.Id + "\',\'" + content_data.AssetData.Version + "\'," + content_data.FolderId + ",false);return false;\"", StyleHelper.SyncButtonCssClass));
					}
				}
			}
			
			if (EnableMultilingual == 1)
			{
				string strViewDisplay = "";
				string strAddDisplay = "";
				LanguageData[] result_language;
				int count = 0;
				ContentAPI m_refAPI = new ContentAPI();
				
				if (security_data.CanEdit)
				{
					result.Append(StyleHelper.ActionBarDivider);
					var l10nObj = new Ektron.Cms.Framework.Localization.LocalizationObject();
					LocalizationState locState = l10nObj.GetContentLocalizationState(m_intFormId, form_data);
					result.Append(m_refStyle.GetTranslationStatusMenu(form_data, m_refMsg.GetMessage("alt click here to update this content translation status"), m_refMsg.GetMessage("lbl mark ready for translation"), locState));
                    result.Append(m_refStyle.PopupTranslationMenu(form_data, locState));
                    if (locState.IsExportableState())
					{
						result.Append(m_refStyle.GetExportTranslationButton((string) ("content.aspx?LangType=" + ContentLanguage + "&action=Localize&id=" + m_intFormId + "&folder_id=" + form_data.FolderId + "&ContentType=" + EkConstants.CMSContentType_Forms + "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" + m_intFormId + "&parm3=folder_id&value3=" + form_data.FolderId + "&parm4=LangType&value4=" + ContentLanguage), m_refMsg.GetMessage("alt form trans"), this.m_refMsg.GetMessage("lbl Export for translation")));
					}
				}
				
				result_language = m_refAPI.DisplayAddViewLanguage(m_intFormId);
				for (count = 0; count <= result_language.Length - 1; count++)
				{
					if (result_language[count].Type == "VIEW")
					{
						if (form_data.LanguageId == result_language[count].Id)
						{
							strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + " selected=\"selected\">" + result_language[count].Name + "</option>";
						}
						else
						{
							strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
						}
					}
				}

				bool languageDividerAdded = false;

				if (strViewDisplay != "")
				{
					result.Append(StyleHelper.ActionBarDivider);
					languageDividerAdded = true;
					
					result.Append("<td class=\"label\">");
					result.Append(m_refMsg.GetMessage("lbl View") + ":");
					result.Append("</td>");
					result.Append("<td>");
					result.Append("<select id=\"viewcontent\" name=\"viewcontent\" onchange=\"LoadContent(\'frmContent\',\'VIEW\');\">");
					result.Append(strViewDisplay);
					result.Append("</select>");
					result.Append("</td>");
				}
				if (security_data.CanAdd)
				{
					for (count = 0; count <= result_language.Length - 1; count++)
					{
						if (result_language[count].Type == "ADD")
						{
							strAddDisplay = strAddDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
						}
					}
					if (strAddDisplay != "")
					{
						if (!languageDividerAdded)
						{
							result.Append(StyleHelper.ActionBarDivider);
						}
						else
						{
							result.Append("<td>&nbsp;&nbsp;</td>");
						}
						
						result.Append("<td class=\"label\">" + m_refMsg.GetMessage("add title") + ":</td>");
						result.Append("<td>");
						result.Append("<select id=\"addcontent\" name=\"addcontent\" onchange=\"LoadContent(\'frmContent\',\'ADD\');\">");
						result.Append("<option value=" + "0" + ">" + "-select language-" + "</option>");
						result.Append(strAddDisplay);
						result.Append("</select></td>");
					}
				}
			}

			result.Append(StyleHelper.ActionBarDivider);

			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		private bool IsMac()
		{
			if (! m_bIsMacInit)
			{
				if (Request.Browser.Platform.IndexOf("Win") == -1)
				{
					m_bIsMac = true;
				}
				else
				{
					m_bIsMac = false;
				}
				m_bIsMacInit = true;
			}
			return (m_bIsMac);
			
		}
		
		#endregion
	}
	
