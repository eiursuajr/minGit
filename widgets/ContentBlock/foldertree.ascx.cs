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
using Ektron.Cms.Interfaces.Context;
using Ektron.Cms.Framework.UI;

public partial class Widgets_pagebuilder_CBFolderTree : System.Web.UI.UserControl
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

            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            Ektron.Cms.Framework.UI.Css.Register(this, cmsContextService.SitePath + "/widgets/ContentBlock/ektron.treeview.css");
        }
    }
}
