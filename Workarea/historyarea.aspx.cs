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

public partial class historyarea : System.Web.UI.Page
{

    #region  Web Form Designer Generated Code

    #region DECLARATIONS

    protected string AppName = "";
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected int m_intContentLanguage = -1;

    #endregion

    #endregion

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        ContentAPI m_refContApi = new ContentAPI();
        string strAction = "";
        string showBackButton = "";
        m_refMsg = m_refContApi.EkMsgRef;
        AppName = m_refContApi.AppName;
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
            }
            else
            {
                if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
            {
                m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (Request.QueryString["action"] != null && Request.QueryString["action"] != "")
        {
            strAction = (string)("&action=" + Request.QueryString["action"]);
        }

        if (!(Request.QueryString["showbackbutton"] == null) && Request.QueryString["showbackbutton"] != "")
        {
            showBackButton = (string)("&showbackbutton=" + Request.QueryString["showBackButton"]);
        }

        //list_frame.Attributes("src") = "historylist.aspx?LangType=" & m_intContentLanguage & "&id=" & Request.QueryString("id") & strAction
        history_frame.Attributes["src"] = "history.aspx?LangType=" + m_intContentLanguage + "&id=" + Request.QueryString["id"] + strAction + showBackButton;
    }
}


