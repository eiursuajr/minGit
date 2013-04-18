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
//using Ektron.Cms.Common.EkConstants;


	public partial class cmsform : System.Web.UI.Page
	{
		protected Collection pagedata;
		protected string PageAction = "";
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected int ContentLanguage = -1;
		protected long m_intFolderId = -1;
		protected long m_intFormId = -1;
		protected bool m_bAjaxTree = false;
		protected string m_strStyleSheetJS = "";
		protected string m_strReloadJS = "";
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected SyncResources m_jsResources;
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
         
			// resource text string tokens
			m_refMsg = m_refContentApi.EkMsgRef;
			
			string closeDialogText = (string) (m_refMsg.GetMessage("close this dialog"));
			string cancelText = (string) (m_refMsg.GetMessage("btn cancel"));
			
			// assign resource text string values
			btnConfirmOk.Text = m_refMsg.GetMessage("lbl ok");
			btnConfirmOk.NavigateUrl = "#" + m_refMsg.GetMessage("lbl ok");
			btnConfirmCancel.Text = cancelText;
			btnConfirmCancel.NavigateUrl = "#" + cancelText;
			btnCloseSyncStatus.Text = m_refMsg.GetMessage("close title");
			btnCloseSyncStatus.NavigateUrl = "#" + m_refMsg.GetMessage("close title");
			btnStartSync.Text = m_refMsg.GetMessage("btn sync now");
			
			closeDialogLink.Text = "<span class=\"ui-icon ui-icon-closethick\">" + m_refMsg.GetMessage("close title") + "</span>";
			closeDialogLink.NavigateUrl = "#" + System.Text.RegularExpressions.Regex.Replace((string) (m_refMsg.GetMessage("close title")), "\\s+", "");
			closeDialogLink.ToolTip = closeDialogText;
			closeDialogLink2.Text = closeDialogLink.Text;
			closeDialogLink2.NavigateUrl = closeDialogLink.NavigateUrl;
			closeDialogLink2.ToolTip = closeDialogText;
			closeDialogLink3.Text = closeDialogLink.Text;
			closeDialogLink3.NavigateUrl = closeDialogLink.NavigateUrl;
			closeDialogLink3.ToolTip = closeDialogText;

            lblSyncStatus.Text = string.Format(m_refMsg.GetMessage("lbl sync status"), " <span class=\"statusHeaderProfileId\"></span>");
			
			m_jsResources = (SyncResources) (LoadControl("sync/SyncResources.ascx"));
			m_jsResources.ID = "jsResources";
			sync_jsResourcesPlaceholder.Controls.Add(m_jsResources);
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			AppImgPath = (string) m_refContentApi.AppImgPath;
			AppPath = (string) m_refContentApi.AppPath;
			
			this.RegisterCSS();
			this.RegisterJS();
			
			ltr_title.Text = m_refMsg.GetMessage("lbl cmsform");
			
			if (m_refContentApi.GetCookieValue("user_id") == "0")
			{
				if (!(Request.QueryString["callerpage"] == null))
				{
					Session["RedirectLnk"] = "cmsform.aspx?" + Request.QueryString.ToString();
				}
				Response.Redirect("login.aspx?fromLnkPg=1", false);
				return;
			}
			if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
			{
				Utilities.ShowError((string) (m_refMsg.GetMessage("com: user does not have permission")));
				return;
			}
			
			if (m_refContentApi.TreeModel == 1)
			{
				m_bAjaxTree = true;
			}
			if (!(Request.QueryString["action"] == null))
			{
				PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!string.IsNullOrEmpty(Request.QueryString["folder_id"]))
			{
				m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
			}
			else if (Information.IsNumeric(Request.Form["content_folder"]))
			{
				m_intFolderId = System.Convert.ToInt64(Request.Form["content_folder"]);
			}
			else
			{
				m_intFolderId = System.Convert.ToInt64(Request.Form[folder_id.UniqueID]);
			}
			if ((!(Request.QueryString["form_id"] == null)) && Strings.Trim(Request.QueryString["form_id"]) != "")
			{
				if (Request.QueryString["form_id"] != "")
				{
					m_intFormId = Convert.ToInt64(Request.QueryString["form_id"]);
				}
			}
			if (!(Request.QueryString["LangType"] == null))
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
			if (ContentLanguage.ToString() == "CONTENT_LANGUAGES_UNDEFINED")
			{
				m_refContentApi.ContentLanguage = Convert.ToInt32("ALL_CONTENT_LANGUAGES");
			}
			else
			{
				m_refContentApi.ContentLanguage = ContentLanguage;
			}
			jsContentLanguage.Text = ContentLanguage.ToString();
			txtDefaultContentLanguage.Text = m_refContentApi.DefaultContentLanguage.ToString();
			m_strStyleSheetJS = m_refStyle.GetClientScript();
			jsAction.Text = PageAction;
			jsFormId.Text = m_intFormId.ToString();
			vFolderId.Text = m_intFolderId.ToString();
			folder_id.Value = m_intFolderId.ToString();
			txtEnableMultilingual.Text = m_refContentApi.EnableMultilingual.ToString();
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			bool bCompleted;
			try
			{
                if ((!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]))) 
				{
					m_strReloadJS = ReloadClientScript();
				}
				switch (PageAction)
				{
					case "dodelete":
					case "submitdelcontaction":
						Process_DoDelete();
						break;
						//The following action moved to content.aspx
						//Case "viewallformsbyfolderid", "viewarchivefrombycategory", "viewallforms"
						//    UniqueLiteral.Text = "form"
						//    Dim m_viewformscategory As viewformscategory
						//    m_viewformscategory = CType(LoadControl("controls/forms/viewformscategory.ascx"), viewformscategory)
						//    m_viewformscategory.ID = "form"
						//    DataHolder.Controls.Add(m_viewformscategory)
						//    m_viewformscategory.ViewFormsByFolderId()
					case "viewform":
						UniqueLiteral.Text = "form";
						viewform m_viewform;
						m_viewform = (viewform) (LoadControl("controls/forms/viewform.ascx"));
						m_viewform.ID = "form";
						DataHolder.Controls.Add(m_viewform);
						m_viewform.ViewForm();
						break;
					case "addform":
						if (!(Page.IsPostBack))
						{
                            if ((Request.QueryString["back_LangType"] == "") || (Request.QueryString["form_id"] == null) || (Request.QueryString["form_id"].ToString() == ""))
							{
								newformwizard ucNewFormWizard;
								ucNewFormWizard = (newformwizard) (LoadControl("controls/forms/newformwizard.ascx"));
								ucNewFormWizard.ID = "ProgressSteps";
								DataHolder.Controls.Add(ucNewFormWizard);
							}
						}
						UniqueLiteral.Text = "form";
						editform m_editform_1;
						m_editform_1 = (editform) (LoadControl("controls/forms/editform.ascx"));
						m_editform_1.ID = "form";
						DataHolder.Controls.Add(m_editform_1);
						bCompleted = System.Convert.ToBoolean(m_editform_1.AddForm());
						break;
					case "editform":
						UniqueLiteral.Text = "form";
						editform m_editform;
						m_editform = (editform) (LoadControl("controls/forms/editform.ascx"));
						m_editform.ID = "form";
						DataHolder.Controls.Add(m_editform);
						bCompleted = System.Convert.ToBoolean(m_editform.EditForm());
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private string ReloadClientScript()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<script type=\"text/javascript\" src=\"java/QueryStringParser.js\"></SCRIPT>" + "\r\n");
			result.Append("<script type=\"text/javascript\">" + "\r\n");
			result.Append("<!--" + "\r\n");
			result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" + "\r\n");
			result.Append("	var m_reloadTrees = \"" + Request.QueryString["reloadtrees"] + "\";" + "\r\n");
			result.Append("	top.ReloadTrees(m_reloadTrees);" + "\r\n");
			result.Append("	self.location.href=\"" + Request.ServerVariables["path_info"] + "?" + Strings.Replace(Request.ServerVariables["query_string"], (string) ("&reloadtrees=" + Request.QueryString["reloadtrees"]), "", 1, -1, 0) + "\";" + "\r\n");
			result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" + "\r\n");
			result.Append("	var strTreeNav = \"" + Request.QueryString["TreeNav"] + "\";" + "\r\n");
			result.Append("	if (strTreeNav != null) {" + "\r\n");
			result.Append("		strTreeNav = strTreeNav.replace(/\\\\\\\\/g,\"\\\\\");" + "\r\n");
			result.Append("		top.TreeNavigation(\"FormsTree\", strTreeNav);" + "\r\n");
			result.Append("	}" + "\r\n");
			result.Append("//-->" + "\r\n");
			result.Append("</script>" + "\r\n");
			return (result.ToString());
		}
		private void Process_DoDelete()
		{
			try
			{
				Ektron.Cms.Modules.EkModule m_refModule;
				if (Request.QueryString["form_id"] != "")
				{
					m_refModule = m_refContentApi.EkModuleRef;
					m_refModule.DeleteFormByID(Request.QueryString["form_id"]); //ret
					Response.Redirect("content.aspx?id=" + Request.Form["frm_folder_id"] + "&LangType=" + ContentLanguage + "&action=ViewContentByCategory", false);
				}
				else
				{
					Response.Redirect((string) ("reterror.aspx?info=" + m_refMsg.GetMessage("msg form id passed")), false);
				}
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
			}
		}
		private void RegisterCSS()
		{
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
			Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.ApplicationPath + "/sync/sync.css", "EktronSyncCss");
			Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.ApplicationPath + "/sync/css/ektron.workarea.sync.dialogs.css", "EktronSyncDialogsCss");
		}
		private void RegisterJS()
		{
			//API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronSiteData);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIWidgetJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "/sync/js/Ektron.Workarea.Sync.Relationships.js", "EktronSyncRelationshipsJS");
        }
	}