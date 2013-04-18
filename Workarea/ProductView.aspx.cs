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


	public partial class Workarea_productview : System.Web.UI.Page
	{
		
		
		protected StyleHelper m_refStyle = new StyleHelper();
        protected Ektron.Cms.Common.EkMessageHelper m_refMsg = null;
		protected string StyleSheetJS = "";
		protected string ViewStyleSheet = "";
		protected string ViewJScript = "";
		protected ContentAPI m_refContentApi;
		protected long m_intFolderId = 0;
		protected int ContentLanguage = 0;
		protected string AppImgPath = "";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				m_refContentApi = new ContentAPI();
				AppImgPath = m_refContentApi.AppImgPath;
				m_refMsg = m_refContentApi.EkMsgRef;
				Utilities.ValidateUserLogin();
				RegisterResources();
				if (!(Request.QueryString["folderid"] == null))
				{
					if (!string.IsNullOrEmpty(Request.QueryString["folderid"]))
					{
						m_intFolderId = long.Parse(Request.QueryString["folderid"].ToString()) ;
					}
				}
				if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
				{
					ContentLanguage = int.Parse(Request.QueryString["LangType"].ToString()) ;
					m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
				m_refContentApi.ContentLanguage = ContentLanguage;
				
				StyleSheetJS = m_refStyle.GetClientScript();
				
				if (Page.IsPostBack == false)
				{
					Display_ToolBar();
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void Display_ToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl catalog view product"));
			
			result.Append("<table><tr>");
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("ViewProductWorkarea", "") + "</td>");
			result.Append("</tr></table>");
			
			divToolBar.InnerHtml = result.ToString();
			
			result = null;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}

