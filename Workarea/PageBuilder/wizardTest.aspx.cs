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

public partial class Workarea_PageBuilder_wizardTest : System.Web.UI.Page
{
    #region Protected Variables ===========================

    protected string AppImgPath = "";
    protected int ContentLanguage = -1;
    protected long CurrentUserID = -1;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    #endregion

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refSiteApi.EkMsgRef;
		Utilities.ValidateUserLogin();
        if (m_refSiteApi.RequestInformationRef.IsMembershipUser == 1 || m_refSiteApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(m_refSiteApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return;
        }
        // Register JS
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/js/ektron.pagebuilder.wizards.js", "EktronPageBuilderWizardsJS");
        JS.RegisterJS(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS");

        // register necessary CSS
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        Css.RegisterCss(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/css/ektron.pagebuilder.wizards.css", "EktronPageBuilderWizardsCSS");
    }
}


