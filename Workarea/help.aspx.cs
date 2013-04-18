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

public partial class help : System.Web.UI.Page
{
    #region  Web Form Designer Generated Code

    protected StyleHelper m_refStyle = new StyleHelper();
    protected string AppPath = "";
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    #endregion

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        AppPath = m_refSiteApi.AppPath;
        if (!string.IsNullOrEmpty(Request.QueryString["action"]) && Request.QueryString["action"].ToLower() == "helpdocuments")
        {
            pnlManuals.Visible = false;
            string myPath = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ek_helpDomainPrefix"]))
            {
                string helpDomain = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
                Uri _uri = new Uri(helpDomain);
                if (_uri != null && !_uri.IsFile)
                {
                    if ((helpDomain.IndexOf("[ek_cmsversion]") > 1))
                    {
                        myPath = helpDomain.Replace("[ek_cmsversion]", m_refSiteApi.RequestInformationRef.Version) + "EktronreferenceWeb.html";
                    }
                    else
                    {
                        myPath = ConfigurationManager.AppSettings["ek_helpDomainPrefix"] + "/index.html";
                    }
                }
                else
                {
                    myPath = AppPath + "/helpmessage.aspx?error=isfile";
                }
            }
            else
            {
                myPath = AppPath + "/help/index.html";
            }
            frmHelp.Attributes["src"] = myPath;
        }
        //Put user code to initialize the page here
/*
        StyleSheetJS.Text = m_refStyle.GetClientScript();
        m_refMsg = m_refSiteApi.EkMsgRef;
        RegisterResources();
        Page.Title = m_refSiteApi.UserId + " " + m_refMsg.GetMessage("manuals page html title");
        divToolBar.InnerHtml = m_refStyle.GetHelpButton("ViewTheManuals", "");
*/
    }

    private void RegisterResources()
    {

    }
}


