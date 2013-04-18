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

	public partial class wikipopup : System.Web.UI.Page
	{
		
		protected StyleHelper m_refStyle = new StyleHelper();
		protected CommonApi m_commonApi;
		protected EkMessageHelper m_refMsg;
		protected string m_defaultFolderPath = "";
		protected bool IsMac = false;
		protected bool IsBrowserIE = false;
		protected string SelectedEditControl = "";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			long folderID = -1;
			Collection gtNavs = null;
			string fPath = "";
			string strJS = "";
			string wikiContTitle = "";
			string wikiContTarget = "";
			
			SelectedEditControl = Utilities.GetEditorPreference(Request);
			
			if ("ContentDesigner" == SelectedEditControl)
			{
				ClientScript.RegisterClientScriptInclude("RadWindow", "../ContentDesigner/RadWindow.js");
				ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);
			}
			
			if (Request.Browser.Platform.IndexOf("Win") == -1)
			{
				IsMac = true;
			}
			if (Request.Browser.Type.IndexOf("IE") != -1)
			{
				IsBrowserIE = true;
			}
			
			m_commonApi = new CommonApi();
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			m_refMsg = m_commonApi.EkMsgRef;
			Utilities.ValidateUserLogin();
			ltContentTitle.Text = m_refMsg.GetMessage("generic article title label");
			divNewContentText.Text = m_refMsg.GetMessage("lbl new content");
			divdvRelatedContentText.Text = m_refMsg.GetMessage("lbl related content");
			searchButton.Text = m_refMsg.GetMessage("lbl go");
			
			if (!String.IsNullOrEmpty(Request.QueryString["wikititle"]))
			{
				wikiContTitle = "wiki_cont_title = \'" + Request.QueryString["wikititle"].Replace("\'", "\\\'") + "\';" + "\r\n";
			}

            if (!String.IsNullOrEmpty(Request.QueryString["target"]))
			{
				wikiContTarget = "wiki_link_target = \'" + Request.QueryString["target"].Replace("\'", "\\\'") + "\';" + "\r\n";
			}
			this.jsSearchRelatedContent.Text = "<script type=\"text/javascript\">loadselectedtext();getContentByID();" + wikiContTitle + "\r\n" + wikiContTarget + "</script>";
			
			ToolBar();
            if (!String.IsNullOrEmpty(Request.QueryString["FolderID"]))
			{
				folderID = Convert.ToInt64(Request.QueryString["FolderID"]);
			}
			if (folderID > -1)
			{
				gtNavs = m_commonApi.EkContentRef.GetFolderInfoWithPath(folderID);
				fPath = gtNavs["Path"].ToString();
			}
			if (fPath != "")
			{
				fPath = fPath.Replace("\\", "\\\\");
				strJS = strJS + "ReturnChildValue(" + folderID + ",\'" + fPath + "\',\'\');" + "\r\n";
			}
			if (!String.IsNullOrEmpty(Request.QueryString["wikititle"]))
			{
				strJS = strJS + "wiki_cont_title = \'" + Request.QueryString["wikititle"].Replace("\'", "\\\'") + "\';" + "\r\n";
			}
			if (m_commonApi.RequestInformationRef.IsMembershipUser == 1)
			{
				strJS = strJS + "document.getElementById(\"a_change\").style.visibility =\"hidden\";" + "\r\n";
				strJS = strJS + "document.getElementById(\"a_none\").style.visibility =\"hidden\";" + "\r\n";
			}
			if (strJS != "")
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), "setdefaultfolderid", strJS, true);
			}
			
			RegisterResources();
			
		}
		
		private void ToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string close = Request.QueryString["close"];
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("title add edit Wiki Link"));
			result.Append("<table><tr>");

			if (close != "true")
			{
				if ("ContentDesigner" == SelectedEditControl)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("close title"), m_refMsg.GetMessage("close title"), "onclick=\"CloseDlg();\"", StyleHelper.CancelButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("close title"), m_refMsg.GetMessage("close title"), "onclick=\"self.close();\"", StyleHelper.CancelButtonCssClass, true));
				}
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("add title"), m_refMsg.GetMessage("add title"), "onclick=\"return inserthyperlink();\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("addwikilink", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void RegisterResources()
		{
			// Register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, m_commonApi.AppPath + "java/eweputil.js", "EktronWikiEWeputilJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			
			// Register CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
		}
	}
