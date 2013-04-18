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
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Workarea;



	public partial class Roles : System.Web.UI.Page
	{
		
		
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected string m_strPageAction = "";
		protected rolemembermgr m_rolemembermgr;
		protected customroles m_customroles;
		protected SiteAPI m_refApi = new SiteAPI();
		protected int ContentLanguage;
        protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			RegisterResources();
			//StyleSheetJS.Text = (new StyleHelper()()).GetClientScript;
            StyleHelper sh = new StyleHelper();
            StyleSheetJS.Text = sh.GetClientScript();
			m_refMsg = (new CommonApi()).EkMsgRef;
			if (!(Request.QueryString["action"] == null))
			{
				if (Request.QueryString["action"] != "")
				{
					m_strPageAction = Request.QueryString["action"].ToLower();
				}
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
			
			try
			{
				Ektron.Cms.ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
				if (! Utilities.ValidateUserLogin())
				{
					return;
				}
				if (! m_refContentApi.IsAdmin())
				{
					Response.Redirect((string) ("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms administrator")), false);
					return;
				}
				switch (m_strPageAction)
				{
					case "managecustompermissions":
						m_customroles = (customroles) (LoadControl("controls/roles/customroles.ascx"));
						DataHolder.Controls.Add(m_customroles);
						break;
						
					default:
						m_rolemembermgr = (rolemembermgr) (LoadControl("controls/roles/rolemembermgr.ascx"));
						m_rolemembermgr.ID  = "role";
						DataHolder.Controls.Add(m_rolemembermgr);
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		protected void RegisterResources()
		{
			// register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			// register CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}
	

