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
using Ektron.Cms.API;
using Ektron.Cms.Common;

public partial class workareatop : System.Web.UI.Page
{
    protected EkMessageHelper m_refMsg;
    protected UserAPI m_refApi = new UserAPI();
    protected ContentAPI contentAPI = new ContentAPI();
    protected CommonApi commonAPI = new CommonApi();
    protected SettingsData settings_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();

    protected void Page_Init(object sender, System.EventArgs e)
    {
        RegisterCSS();
        RegisterJS();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        m_refMsg = m_refApi.EkMsgRef;

        if (!contentAPI.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0))
        {
            Response.Redirect(contentAPI.ApplicationPath + "blank.htm");
            return;
        }
        else if (Convert.ToBoolean(contentAPI.RequestInformationRef.IsMembershipUser))
        {
            Utilities.ShowError(contentAPI.EkMsgRef.GetMessage("msg login cms user"));
            return;
        }
        else
        {
            ltrVersion.Text = m_refMsg.GetMessage("version") + "&nbsp;" + contentAPI.Version + "&nbsp;" + contentAPI.ServicePack + "<i>(" + m_refMsg.GetMessage("build") + "&nbsp;" + contentAPI.BuildNumber + ")</i>";
            try
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(m_refApi.RequestInformationRef.UserCulture);
            }
            catch (Exception)
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-us");
            }

            DesktopLink.Title=DesktopLink.InnerText = m_refMsg.GetMessage("lbl desktop");
            ContentLink.Title=ContentLink.InnerText = m_refMsg.GetMessage("lbl content");
            LibraryLink.Title=LibraryLink.InnerText = m_refMsg.GetMessage("generic library title");
            SettingsLink.Title=SettingsLink.InnerText = m_refMsg.GetMessage("administrate button text");
            ReportsLink.Title=ReportsLink.InnerText = m_refMsg.GetMessage("lbl wa mkt reports");
            HelpLink.Title=HelpLink.InnerText = m_refMsg.GetMessage("generic help");
            string strUserName = "";
            if (m_refApi.UserId > 0)
            {
                strUserName = EkFunctions.HtmlEncode(Server.UrlDecode(m_refApi.GetCookieValue("Username")));
                if (m_refApi.UserId == Ektron.Cms.Common.EkConstants.BuiltIn)
                {
                    ContentLink.Visible = false;
                    LibraryLink.Visible = false;
                    DesktopLink.Visible = false;
                    ReportsLink.Visible = false;
                    SelectSettingsTab.Text = "true";
                }
                // trim to max length and add elipsis if needed:
                if (strUserName.Length > 20)
                {
                    strUserName = (string)(strUserName.Substring(0, 20) + "...");
                }
            }

            if (m_refApi.UserId == 1 || m_refApi.UserId == 999999999)
            {             
                    settings_data = m_refSiteApi.GetSiteVariables(m_refApi.UserId, true);
                    Ektron.Cms.License.EkLicAnalyzer licAnalyzer = new Ektron.Cms.License.EkLicAnalyzer();
                    licAnalyzer.License = settings_data.LicenseKey;
                    if (licAnalyzer != null && Convert.ToDateTime(licAnalyzer.ExpirationDate) > DateTime.MinValue && (Convert.ToDateTime(licAnalyzer.ExpirationDate) - DateTime.Now).Days < 10)
                    {
                        if((Convert.ToDateTime(licAnalyzer.ExpirationDate) - DateTime.Now).Days >= 0)
                             ltrLic.Text = m_refMsg.GetMessage("lbl license warning") + " " + ((Convert.ToDateTime(licAnalyzer.ExpirationDate) - DateTime.Now).Days + 1) + " " + m_refMsg.GetMessage("lbl days") + " " + Convert.ToDateTime(licAnalyzer.ExpirationDate).ToString("MMMM dd, yyyy");
                        else
                            ltrLic.Text = m_refMsg.GetMessage("lbl license expired") + " " +Convert.ToDateTime(licAnalyzer.ExpirationDate).ToString("MMMM dd, yyyy");
                    }
            }

            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
            {
                DesktopLink.Visible = false;
            }

            lblUser.Text = string.Format("{0} {1}", m_refMsg.GetMessage("user"), strUserName);

            // notify user of any messages
            int iUnread = contentAPI.GetUnreadMessageCountForUser(commonAPI.UserId);
            string unreadAnchor = "<a href=\"" + m_refApi.AppPath.ToString() + "CommunityMessaging.aspx?action=viewall\" target=\"ek_main\" onclick=\"ChangePage(this, \'UserMessages\'); return false;\">{0} {1}</a>";
            if (iUnread == 1)
            {
                userUnreadMessages.Text = string.Format(m_refMsg.GetMessage("lbl user you have messages"), string.Format(unreadAnchor, iUnread, m_refMsg.GetMessage("lbl user message")));
            }
            else
            {
                userUnreadMessages.Text = string.Format(m_refMsg.GetMessage("lbl user you have messages"), string.Format(unreadAnchor, iUnread, m_refMsg.GetMessage("lbl user messages")));
            }

            if (Request.QueryString["tab"] == "content")
            {
                ContentLink.Attributes["class"] = "selected";
            }
            else
            {
                if (strUserName == "builtin")
                {
                    SettingsLink.Attributes["class"] = "selected";
                }
                else
                {
                    DesktopLink.Attributes["class"] = "selected";
                }
            }
        }
    }

    #region CSS/JS

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        //Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
    }

    private void RegisterJS()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
    }

    #endregion
}


