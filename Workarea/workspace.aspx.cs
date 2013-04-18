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

	public partial class Workarea_workspace : System.Web.UI.Page
	{
		
		private int m_intContentLanguage = 0;
		private ContentAPI m_refContApi;
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			m_refContApi = new ContentAPI();
			if (Request.QueryString["LangType"] == null)
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
		}
	}