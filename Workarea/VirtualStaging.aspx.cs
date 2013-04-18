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
using Microsoft.Security.Application;
using Ektron.Cms.Common;

	public partial class Workarea_VirtualStaging : System.Web.UI.Page
	{
		private viewvirtualstaging m_viewvirtualstaging;
		private editvirtualstaging m_editvirtualstaging;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected string AppImgPath = "";
		protected string AppName = "";
		protected string SITEPATH = "";
		protected bool m_blnRefreshFrame;
		protected SiteAPI m_refSiteApi = new SiteAPI();
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			m_refMsg = m_refSiteApi.EkMsgRef;
			if (!Utilities.ValidateUserLogin())
			{
				return;
			}
			if (!m_refSiteApi.IsAdmin())
			{
				Response.Redirect((string) ("reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms administrator")));
				return;
			}
			AppImgPath = m_refSiteApi.AppImgPath;
			AppName = m_refSiteApi.AppName;
			SITEPATH = m_refSiteApi.SitePath;
			ltr_title.Text = AppName + " " + m_refMsg.GetMessage("config page html title");
			RegisterResources();
			if (Request.QueryString["action"] != null)
			{
				m_strPageAction = Request.QueryString["action"];
				if (m_strPageAction.Length > 0)
				{
					m_strPageAction = m_strPageAction.ToLower();
				}
			}
			
			if (Request.QueryString["RefreshFrame"] != null)
			{
				if (Request.QueryString["RefreshFrame"].ToLower() == "true")
				{
					m_blnRefreshFrame = true;
				}
			}
			
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("virtual staging page title"));
			divToolBar.InnerHtml = ConfigToolBar();
		}
		private string ConfigToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>");
			if (m_strPageAction == "edit")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "VirtualStaging.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update virtual staging button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'virtualstaging\', \'VerifyForm()\');\"", StyleHelper.EditButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", "VirtualStaging.aspx?action=edit", m_refMsg.GetMessage("alt update virtual staging button text"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton((string) ("VirtualStagingSetup" + EkFunctions.HtmlEncode(m_strPageAction)), "") + "</td>");
			result.Append("</tr></table>");
			return (result.ToString());
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			bool bCompleted = false;
			try
			{
				switch (m_strPageAction)
				{
					case "edit":
						m_editvirtualstaging = (editvirtualstaging) (LoadControl("controls/virtualstaging/editvirtualstaging.ascx"));
						DataHolder.Controls.Add(m_editvirtualstaging);
						bCompleted = System.Convert.ToBoolean(m_editvirtualstaging.EditVirtualStagingControl());
						if (bCompleted == true)
						{
							if (!IsPostBack)
							{
								Response.Redirect("VirtualStaging.aspx", false);
							}
							else
							{
								Response.Redirect("VirtualStaging.aspx?RefreshFrame=true", false);
							}
						}
						break;
					default:
						m_viewvirtualstaging = (viewvirtualstaging) (LoadControl("controls/virtualstaging/viewvirtualstaging.ascx"));
						m_viewvirtualstaging.ID = "viewvirtualstaging";
						DataHolder.Controls.Add(m_viewvirtualstaging);
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.ToString());
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/empjsfunc.js", "EktronEmpJSFunctJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
		}
	}