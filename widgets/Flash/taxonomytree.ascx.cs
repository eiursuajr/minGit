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
using System.Collections.Generic;

using Ektron.Cms;
using Ektron.Cms.API;

public partial class Flash_pagebuilder_taxtree : System.Web.UI.UserControl
{
    public string Filter = "";
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        m_refMsg = m_refSiteApi.EkMsgRef;
        ContentAPI capi = new ContentAPI();
        Ektron.Cms.API.Content.Taxonomy tax = new Ektron.Cms.API.Content.Taxonomy();
        TaxonomyRequest tr = new TaxonomyRequest();
        tr.IncludeItems = false;
        tr.Depth = 1;
        tr.Page = Page;
        tr.TaxonomyId = 0;
        tr.TaxonomyLanguage = capi.RequestInformationRef.ContentLanguage;
        tr.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
        tr.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
        TaxonomyBaseData[] td = capi.EkContentRef.ReadAllSubCategories(tr);
        taxonomies.DataSource = td;
        taxonomies.DataBind();

        // Register JS
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronTreeviewJS);
    }
}
