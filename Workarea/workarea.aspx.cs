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
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

public partial class workarea : System.Web.UI.Page
{
    protected CommonApi m_refApi = new CommonApi();

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
		EkContent _EkContent = new EkContent(m_refApi.RequestInformationRef);
        EkMessageHelper m_refMsg = new EkMessageHelper(m_refApi.RequestInformationRef);
        if (!_EkContent.IsAllowed(m_refApi.RequestInformationRef.CallerId, m_refApi.RequestInformationRef.UserCulture, "users", "IsLoggedIn"))
            Response.Redirect(m_refApi.RequestInformationRef.SitePath + "workarea/login.aspx");
			
        //Register Page Components
        this.RegisterPackages();

        //set javascript strings
        this.SetJavascriptStrings();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        //Put user code to initialize the page here
        Ektron.Cms.Common.EkMessageHelper m_refMsg;
        UserAPI m_refUserApi = new UserAPI();
        m_refMsg = m_refApi.EkMsgRef;
        string strUserName = Server.UrlDecode(m_refApi.GetCookieValue("Username"));
        if (Convert.ToBoolean(m_refApi.RequestInformationRef.IsMembershipUser) || m_refApi.RequestInformationRef.UserId == 0)
        {
            Response.Write("Please login as a cms user.");
            ek_nav_bottom.Attributes["src"] = "blank.htm";
            ek_main.Attributes["src"] = "blank.htm";
        }
        else
        {
            Page.Title = m_refApi.AppName + " " + m_refMsg.GetMessage("workarea page html title") + " " + HttpUtility.UrlDecode(Ektron.Cms.CommonApi.GetEcmCookie()["username"]);
            ek_nav_bottom.Attributes["src"] = ek_nav_bottom.Attributes["src"] + "?" + Request.ServerVariables["query_string"];
            if (!(Request.QueryString["page"] == null))
            {
                if (Request.QueryString["page"] != "")
                {
                    litMainPage.Text = Request.QueryString["page"] + "?" + Request.ServerVariables["query_string"];
                    ek_main.Attributes["src"] = litMainPage.Text;
                }
            }
            else
            {
                if (strUserName == "builtin")
                {
                    ek_main.Attributes["src"] = "blank.htm";
                }
                else
                {
                    Ektron.Cms.UserPreferenceData preference_data = m_refUserApi.GetUserPreferenceById(0);
                    if (Convert.ToString(preference_data.FolderId) == "" && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
                    {
                        ek_main.Attributes["src"] = "dashboard.aspx" + "?" + Request.ServerVariables["query_string"];
                    }
                    else
                    {
                        litMainPage.Text = "content.aspx?action=ViewContentByCategory&id=0" + "&" + Request.ServerVariables["query_string"];
                        ek_main.Attributes["src"] = litMainPage.Text;
                    }
                }
            }
        }

        litWorkareaPrefix.Text = m_refApi.ApplicationPath;
    }

    protected string userstring()
    {
        return "groupid=2&grouptype=0&LangType=" + (m_refApi.ContentLanguage > 0 ? m_refApi.ContentLanguage : m_refApi.DefaultContentLanguage) + "&id=" + m_refApi.UserId + "&FromUsers=";
    }

    private void SetJavascriptStrings()
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        Hashtable objResult;
        Ektron.Cms.Site.EkSite SiteObj;
        SiteObj = AppUI.EkSiteRef;
        objResult = SiteObj.GetPermissions(0, 0, "folder");
        litPerReadOnlyLib.Text = Convert.ToString(objResult["ReadOnlyLib"]).ToLower();
        litLanguageId1.Text = AppUI.ContentLanguage.ToString();
        litLanguageId2.Text = AppUI.ContentLanguage.ToString();
    }

    private void RegisterPackages()
    {
        ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();

        // create a package that will register the UI JS and CSS we need
        Package searchResultsControlPackage = new Package()
        {
            Components = new List<Component>()
            {
                // Register JS Files
                Packages.EktronCoreJS,
                JavaScript.Create(cmsContextService.WorkareaPath + "/" +"java/ektron.workarea.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/thickbox.js"),
                
                // Register CSS Files
                Css.Create(cmsContextService.WorkareaPath + "/" + "csslib/ektron.workarea.css"),
                Css.Create(cmsContextService.WorkareaPath + "/" + "csslib/ektron.workarea.ie.css", BrowserTarget.LessThanEqualToIE7),
                Css.Create(cmsContextService.WorkareaPath + "/" + "csslib/box.css")
            }
        };
        searchResultsControlPackage.Register(this);
    }
}


