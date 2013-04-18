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
using System.IO;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;


	public partial class dashboard : System.Web.UI.Page
	{

		private CommonApi _CommonApi = new CommonApi();
		private Ektron.Cms.Common.EkMessageHelper _MessageHelper;
		private StyleHelper _StyleHelper = new StyleHelper();
		private int _ContentLanguage = 0;
		private ContentAPI _ContentApi = new ContentAPI();

		private void Page_PreInit(System.Object sender, System.EventArgs e)
		{
			try
			{
				SetWidgetSpaceID();
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}

		private void Page_Init(System.Object sender, System.EventArgs e)
		{

			RegisterResources();
		}

		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			if ((_CommonApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn",_CommonApi.UserId) == false)
			{
				Response.Redirect("login.aspx?fromLnkPg=1", true);
				return;
			}
			if ((_CommonApi.RequestInformationRef.IsMembershipUser == 1) || (_CommonApi.RequestInformationRef.UserId == 0))
			{
				Literal literalError = new Literal();
				literalError.Text = "<p>Please login as a cms user.<br/><a href=\"login.aspx\">Click here to login</a></p>";
				form1.Controls.Add(literalError);
				mainDiv.Visible = false;
				return;
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				_ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				_CommonApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
			}
			else
			{
				_ContentLanguage = int.Parse(_CommonApi.GetCookieValue("LastValidLanguageID"));
			}

			if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				_CommonApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				_CommonApi.ContentLanguage = _ContentLanguage;
			}
			_CommonApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;

			try
			{
				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;

				_MessageHelper = _CommonApi.EkMsgRef;

				SetTitlebarText();
				SetHelpButtonText();
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}

		private void SetHelpButtonText()
		{
			if (_CommonApi.Debug_ShowHelpAlias)
			{
				Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
			}
			HelpButton.Text = _StyleHelper.GetHelpButton("dashboard_aspx", "");
		}

		private void SetTitlebarText()
		{
			string strUserName = "";
			if (_CommonApi.UserId > 0)
			{
				strUserName = EkFunctions.HtmlEncode(Server.UrlDecode((string) (Ektron.Cms.CommonApi.GetEcmCookie()["username"])));
				// trim to max length and add elipsis if needed:
				if (strUserName.Length > 20)
				{
					strUserName = (string) (strUserName.Substring(0, 20) + "...");
				}
			}
			divTitle.Text  = "<span id=\"ektronTitlebar\">" + _MessageHelper.GetMessage("lbl smart desktop for admin") + "</span> " + strUserName;
		}

		private void SetWidgetSpaceID()
		{
			//See if a widget scope record exists with a scope value of 2 (SmartDesktop)
			long workareaDashboardSpaceID = GetWorkareaDashboardScopeID();
			if (workareaDashboardSpaceID == -1)
			{
				//It does not exist, so programmatically add it and sync the 'Widgets' directory
				workareaDashboardSpaceID = RegisterWidgetSpaceID();
				SyncWidgets();
			}

			Personalization1.WidgetSpaceID = workareaDashboardSpaceID;
		}

		private long GetWorkareaDashboardScopeID()
		{
			foreach (WidgetSpaceData item in WidgetSpaceFactory.GetModel().FindAll())
			{
				if (item.Scope == WidgetSpaceScope.SmartDesktop)
				{
					return item.ID;
				}
			}
			return -1;
		}

		private long RegisterWidgetSpaceID()
		{
			WidgetSpaceData widgetSpaceData = null;
			string title = "SmartDesktop";
			WidgetSpaceFactory.GetModel().Create(title, WidgetSpaceScope.SmartDesktop, out widgetSpaceData);
			return widgetSpaceData.ID;
		}

		private void SyncWidgets()
		{
			if (Directory.Exists(Server.MapPath(_ContentApi.RequestInformationRef.WidgetsPath)))
			{
				WidgetTypeController.SyncWidgetsDirectory();
			}
		}

		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIResizableJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);

            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' }) + "/Personalization/css/ektron.personalization.css", "EktronPersonalziationCss");
            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' }) + "/Personalization/css/ektron.personalization.ie.7.css", "EktronPersonalizationIe7Css", Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' }) + "/csslib/ektron.workarea.personalization.css", "EktronWorkareaPersonalziationCss");
            Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath.TrimEnd(new char[] { '/' }) + "/csslib/ektron.workarea.personalization.ie.7.css", "EktronWorkareaPersonalizationIe7Css", Ektron.Cms.API.Css.BrowserTarget.IE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}

