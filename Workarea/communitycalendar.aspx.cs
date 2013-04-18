using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_communitycalendar : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        int langid = 0;
        ContentAPI m_refContApi = new ContentAPI();
        if (Request.QueryString["LangType"] != null)
        {
            if (Request.QueryString["LangType"] != string.Empty)
            {
                langid = int.Parse(Request.QueryString["LangType"]);
                m_refContApi.SetCookieValue("SiteLanguage", langid.ToString());
            }
            else
            {
                if (m_refContApi.GetCookieValue("SiteLanguage") != string.Empty)
                {
                    langid = int.Parse(m_refContApi.GetCookieValue("SiteLanguage"));
                }
            }
        }
        else
        {
            if (m_refContApi.GetCookieValue("SiteLanguage") != string.Empty)
            {
                langid = int.Parse(m_refContApi.GetCookieValue("SiteLanguage"));
            }
        }
        if (langid == EkConstants.CONTENT_LANGUAGES_UNDEFINED || langid == EkConstants.ALL_CONTENT_LANGUAGES)
        {
            langid = m_refContApi.DefaultContentLanguage;
        }
        if (langid == EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContApi.ContentLanguage = EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContApi.ContentLanguage = langid;
        }
    }
}
