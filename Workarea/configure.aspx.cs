using System;
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkConstants;


	public partial class configure : System.Web.UI.Page
	{
		
		
		
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected string AppName = "";
		protected string SITEPATH = "";
		protected bool m_blnRefreshFrame;
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			SiteAPI m_refSiteApi = new SiteAPI();
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			AppPath = m_refSiteApi.AppPath;
			AppName = m_refSiteApi.AppName;
			SITEPATH = m_refSiteApi.SitePath;
			litTitle.Text = AppName + " " + m_refMsg.GetMessage("config page html title");
            
			if ((m_refSiteApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn",0) == false)
			{
				Response.Redirect("login.aspx?fromLnkPg=1", true);
				return;
			}
			if ( Convert.ToBoolean(m_refSiteApi.RequestInformationRef.IsMembershipUser) || m_refSiteApi.RequestInformationRef.UserId == 0)
			{
				Response.Redirect("reterror.aspx?info=Please login as cms user", true);
				return;
			}
			RegisterResources();
			if (!(Request.QueryString["action"] == null))
			{
				m_strPageAction = Request.QueryString["action"];
				if (m_strPageAction.Length > 0)
				{
					m_strPageAction = m_strPageAction.ToLower();
				}
			}
			
			if (!(Request.QueryString["RefreshFrame"] == null))
			{
				if (Request.QueryString["RefreshFrame"].ToLower() == "true")
				{
					m_blnRefreshFrame = true;
				}
			}
			
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("config page title"));
			divToolBar.InnerHtml = ConfigToolBar();
		}
		private string ConfigToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>");
			if (m_strPageAction == "edit")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "configure.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'config\', \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "configure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton((string) ("ApplicationSetup" + m_strPageAction), "") + "</td>");
			result.Append("</tr></table>");
			return (result.ToString());
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			bool bCompleted;
			try
			{
				switch (m_strPageAction)
				{
					case "edit":
						ViewSet.SetActiveView(Edit);
						bCompleted = System.Convert.ToBoolean(editconfiguration1.EditConfigurationControl());
						if (bCompleted == true)
						{
							if (!(IsPostBack))
							{
								Response.Redirect("configure.aspx", false);
							}
							else
							{
								Response.Redirect("configure.aspx?RefreshFrame=true", false);
							}
						}
						break;
					default:
						ViewSet.SetActiveView(View);
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronEmpJSFuncJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
		}
	}
