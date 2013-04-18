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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Common;

	public partial class editarea : System.Web.UI.Page
	{
        protected ContentAPI m_refContentApi = new ContentAPI();
		protected EkMessageHelper m_refMsg;

    	private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//register page components
			this.RegisterJS();
			this.RegisterCSS();

			//set javascript strings
			this.SetJavascriptStrings();

			string strTitle = "";
			string AddToCollectionType = "";
			string FromEE = "";
			int ContentLanguage = -1;
			string mycollection = "";
			string contentType = "";
			bool bShowLogin = false;
			string sXid = "";
			string updateFieldId = "";
			string taxonomyId = "";
			string seltaxonomyId = "";

			if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["TaxonomyId"]))
			{
				taxonomyId = (string) ("&TaxonomyId=" + System.Web.HttpContext.Current.Request.QueryString["TaxonomyId"].ToString());
			}
			if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["SelTaxonomyId"]))
			{
				seltaxonomyId = (string) ("&SelTaxonomyId=" + System.Web.HttpContext.Current.Request.QueryString["SelTaxonomyId"].ToString());
			}
			m_refMsg = m_refContentApi.EkMsgRef;
			if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
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
			if (Request.QueryString["ShowLogin"] == "true")
			{
				bShowLogin = true;
			}
			if (! bShowLogin)
			{
				//Make sure the user is logged in. If not forward user to login page.
                if ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", m_refContentApi.RequestInformationRef.CallerId) == false)
				{
					bShowLogin = true;
				}
			}
			if (!String.IsNullOrEmpty(Request.QueryString["mycollection"]))
			{
				mycollection = (string) ("&mycollection=" + Request.QueryString["mycollection"]);
			}
			else
			{
				if (!String.IsNullOrEmpty(Request.Form["mycollection"]))
				{
					mycollection = (string) ("&mycollection=" + Request.Form["mycollection"]);
				}
			}
			if (!String.IsNullOrEmpty(Request.QueryString["ContType"]) && Information.IsNumeric(Request.QueryString["ContType"]))
			{
				int iContentType = Ektron.Cms.Common.EkFunctions.ReadDbWholeNumber(Request.QueryString["ContType"]);
				contentType = string.Format("&ContType={0}", iContentType);
			}
			if (!String.IsNullOrEmpty(Request.QueryString["addto"]))
			{
				AddToCollectionType = (string) ("&addto=" + Request.QueryString["addto"]);
			}
			else
			{
				if (!String.IsNullOrEmpty(Request.Form["addto"]))
				{
					AddToCollectionType = (string) ("&addto=" + Request.Form["addto"]);
				}
			}
			if (!String.IsNullOrEmpty(Request.QueryString["FromEE"]))
			{
				FromEE = (string) ("&FromEE=" + Request.QueryString["FromEE"]);
			}
			else
			{
				FromEE = "";
			}
			if (!String.IsNullOrEmpty(Request.QueryString["ctlupdateid"]))
			{
				updateFieldId = (string) ("&ctlupdateid=" + Request.QueryString["ctlupdateid"] + "&ctlmarkup=" + Request.QueryString["ctlmarkup"] + "&cltid=" + Request.QueryString["cltid"] + "&ctltype=" + Request.QueryString["ctltype"]);
			}
			if (!String.IsNullOrEmpty(Request.QueryString["cacheidentifier"]))
			{
				updateFieldId = updateFieldId + "&cacheidentifier=" + Request.QueryString["cacheidentifier"];
			}
			if (Request.QueryString["type"] == "add")
			{
				string id;
				FolderData cFolder;
				id = Request.QueryString["id"];
				if (!String.IsNullOrEmpty(Request.QueryString["xid"]))
				{
					sXid = (string) ("&xid=" + Request.QueryString["xid"]);
				}
				cFolder = m_refContentApi.GetFolderById(Convert.ToInt64(id));
				strTitle = m_refMsg.GetMessage("add content page title");
				strTitle = strTitle + " \"" + cFolder.Name + "\"";
			}
			else
			{
				strTitle = m_refMsg.GetMessage("edit content page title");
			}
			Page.Title = m_refContentApi.AppName + " " + strTitle;
			if (Request.QueryString["type"] != "add")
			{
                workareatop.Attributes["src"] = "workareatop.aspx?title=workarea_edit_top.gif";
			}
			if (bShowLogin)
			{
				Session["RedirectLnk"] = "edit.aspx?content_id=" + Request.QueryString["content_id"] + "&LangType=" + Request.QueryString["LangType"] + sXid + "&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"] + "&enableFrmbar=" + Request.QueryString["enableFrmbar"] + "&pullapproval=" + Request.QueryString["pullapproval"] + "&dontcreatetask=" + Request.QueryString["dontcreatetask"] + mycollection + contentType + AddToCollectionType + FromEE + updateFieldId + taxonomyId + seltaxonomyId;
				ek_main.Attributes["src"] = "login.aspx?fromLnkPg=1";
			}
			else
			{
				ek_main.Attributes["src"] = "edit.aspx?content_id=" + Request.QueryString["content_id"] + "&LangType=" + Request.QueryString["LangType"] + sXid + "&id=" + Request.QueryString["id"] + "&folder_id=" + Request.QueryString["folder_id"] + "&type=" + Request.QueryString["type"] + "&control=" + Request.QueryString["control"] + "&buttonid=" + Request.QueryString["buttonid"] + "&enableFrmbar=" + Request.QueryString["enableFrmbar"] + "&pullapproval=" + Request.QueryString["pullapproval"] + "&dontcreatetask=" + Request.QueryString["dontcreatetask"] + mycollection + contentType + AddToCollectionType + FromEE + updateFieldId + taxonomyId + seltaxonomyId;
			}

		}

		private void SetJavascriptStrings()
		{
			ApplicationAPI AppUI = new ApplicationAPI();
			Hashtable objResult;
            objResult = AppUI.EkSiteRef.GetPermissions(0, 0, "folder");

			litPerReadOnlyLib.Text = Strings.LCase(objResult["ReadOnlyLib"].ToString());
			litLanguageId1.Text = AppUI.ContentLanguage.ToString();
			litLanguageId2.Text = AppUI.ContentLanguage.ToString();
		}

		private void RegisterJS()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
		}

		private void RegisterCSS()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
		}

	}