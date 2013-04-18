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

	public partial class xml_config : System.Web.UI.Page
	{
		protected string m_strPageAction = "";
		protected editxml_config m_editxml_config;
		protected viewxml_config m_viewxml_config;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected int ContentLanguage = -1;
		protected CommonApi m_refApi = new CommonApi();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected Ektron.Cms.Content.EkContent m_refContent;
		protected EkMessageHelper m_refMsg;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			RegisterResources();
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			m_refContent = m_refContentApi.EkContentRef;
			m_refMsg = m_refContentApi.EkMsgRef;
			if (m_refContent.IsAllowed(0, 0, "users", "IsLoggedIn", 0) == false || m_refContent.IsAllowed(0, 0, "users", "IsAdmin", 0) == false)
			{
				if (!m_refContent.IsARoleMember(Convert.ToInt64(EkEnumeration.CmsRoleIds.AdminXmlConfig), m_refContent.RequestInformation.UserId, false))
				{
					Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"));
				}
			}
			if (!String.IsNullOrEmpty(Request.QueryString["action"]))
			{
                m_strPageAction = Request.QueryString["action"].ToLower();			
			}
			if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
			{
				ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				m_refApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
			}
			else
			{
				if (m_refApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = int.Parse(m_refApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			m_refApi.ContentLanguage = ContentLanguage;
			StyleSheetJS.Text = m_refStyle.GetClientScript();
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			bool bCompleted = false;
			long intRetVal = 0;
			try
			{
				switch (m_strPageAction)
				{
					case "viewallxmlconfigurations":
						m_viewxml_config = (viewxml_config) (LoadControl("controls/xmlconfig/viewxml_config.ascx"));
						DataHolder.Controls.Add(m_viewxml_config);
						bCompleted = m_viewxml_config.ViewAllXmlConfigurations();
						if (bCompleted)
						{
							Response.Redirect("xml_config.aspx?action=ViewAllXmlConfigurations", false);
						}
						break;
					case "viewxmlconfiguration":
						m_viewxml_config = (viewxml_config) (LoadControl("controls/xmlconfig/viewxml_config.ascx"));
						DataHolder.Controls.Add(m_viewxml_config);
						bCompleted = m_viewxml_config.ViewXmlConfiguration();
						if (bCompleted)
						{
							Response.Redirect("xml_config.aspx?action=ViewAllXmlConfigurations", false);
						}
						break;
					case "addxmlconfigurationv4":
						m_editxml_config = (editxml_config) (LoadControl("controls/xmlconfig/editxml_config.ascx"));
						DataHolder.Controls.Add(m_editxml_config);
						intRetVal = m_editxml_config.AddXmlConfig();
						if (intRetVal > 0)
						{
							Response.Redirect((string) ("editdesign.aspx?action=EditPackage&id=" + intRetVal), false);
						}
						break;
					case "editxmlconfiguration":
						m_editxml_config = (editxml_config) (LoadControl("controls/xmlconfig/editxml_config.ascx"));
						DataHolder.Controls.Add(m_editxml_config);
						bCompleted = m_editxml_config.EditXmlConfig();
						if (bCompleted)
						{
							Response.Redirect((string) ("xml_config.aspx?action=ViewXmlConfiguration&id=" + Request.QueryString["id"]), false);
						}
						break;
					case "deletexmlconfiguration":
						m_refContentApi.DeleteXmlConfiguration(Convert.ToInt64(Request.QueryString["id"]));
						Response.Redirect("xml_config.aspx?action=ViewAllXmlConfigurations", false);
						break;
					case "newinheritconfiguration":
						m_editxml_config = (editxml_config) (LoadControl("controls/xmlconfig/editxml_config.ascx"));
						DataHolder.Controls.Add(m_editxml_config);
						bCompleted = m_editxml_config.EditXmlConfig();
						break;
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.ToString().IndexOf("xml_index_FK00") != -1)
				{
					string ErrorMsg = "";
					ErrorMsg = "There is published content associated with the smart form. Please delete the content before trying to delete the smart form";
					Utilities.ShowError(ErrorMsg);
				}
				else
				{
					Utilities.ShowError(ex.ToString());
				}
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}
