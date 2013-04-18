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
using Ektron.Cms.Workarea;

public partial class MyWorkspace_MyDocuments : workareabase
{

    private int m_intContentLanguage = 0;
    private ContentAPI m_refContApi;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        m_refContApi = new ContentAPI();
		Utilities.ValidateUserLogin();
        RegisterResources();
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refContApi.SetCookieValue("SiteLanguage", m_intContentLanguage.ToString());
            }
            else
            {
                if (m_refContApi.GetCookieValue("SiteLanguage") != "")
                {
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("SiteLanguage"));
                }
            }
        }
        else
        {
            if (m_refContApi.GetCookieValue("SiteLanguage") != "")
            {
                m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("SiteLanguage"));
            }
        }
        if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || m_intContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
        {
            m_intContentLanguage = m_refContApi.DefaultContentLanguage;
        }
        if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContApi.ContentLanguage = m_intContentLanguage;
        }
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        SetLabels();
    }

    protected void SetLabels()
    {
        this.SetTitleBarToMessage("lbl documents");
        this.AddHelpButton("mydocuments");
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
