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


	public partial class Workarea_productsearch : System.Web.UI.Page
	{
		
		
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg = null;
		protected string StyleSheetJS = "";
		protected string SearchStyleSheet = "";
		protected string SearchJScript = "";
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
					if (Request.QueryString["folderid"] != "")
					{
						m_intFolderId = Convert.ToInt64(Request.QueryString["folderid"]);
					}
				}
				if (Request.QueryString["LangType"] != "")
				{
					ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
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
				
				SearchStyleSheet = "<link rel=\'stylesheet\' type=\'text/css\' href=\'csslib/worksearch.css\'>" + "\r\n";
				StyleSheetJS = m_refStyle.GetClientScript();
				
				if (ProductSearch1 != null)
				{
					ProductSearch1.CatalogId = m_intFolderId;
					ProductSearch1.LanguageID = ContentLanguage;
					ProductSearch1.IsInWorkArea = true;
					ProductSearch1.DisplayXslt = "Xslt/WA_ProductSearch.xsl";
					//ProductSearch1.TemplateProduct = "ProductView.aspx"
				}
				if (Page.IsPostBack == false)
				{
					Display_ToolBar();
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.ToLower().IndexOf("service is not running") != -1)
				{
					Utilities.ShowError("Error: Index service is not running.  You cannot search on Products.  Restart the service.");
				}
				else
				{
					Utilities.ShowError(ex.Message);
				}
			}
		}
		private void Display_ToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl search catalog folder"));
			
			result.Append("<table><tr>");
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("SearchCatalogFolder", "") + "</td>");
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

