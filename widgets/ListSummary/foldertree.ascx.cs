using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms;
using Ektron.Cms.API;

public partial class ListSummary_Widgets_pagebuilder_foldertree : System.Web.UI.UserControl
{
    public string Filter = "";
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_refMsg = m_refSiteApi.EkMsgRef;
        ContentAPI capi = new ContentAPI();

        // Register JS
        if (capi.IsLoggedIn)
        {
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronTreeviewJS);
        }
    }
}
