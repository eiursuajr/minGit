using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.API;

public partial class Workarea_sync_CloudSyncList : System.Web.UI.Page
{
    public readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;

    public Workarea_sync_CloudSyncList()
    {
        // DEMO POC only
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.RegisterResources();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.RenderHeader();

        List<Relationship> relationships = Relationship.GetRelationships();
        List<Relationship> azureRelationships = new List<Relationship>();
        foreach (Relationship relationship in relationships)
        {
            //relationship.Status == ProfileStatus.Active 
            if (relationship.Name == "Azure")//                && relationship.LocalSite.Address.ToLower() == System.Net.Dns.GetHostName().ToLower())
            {
                azureRelationships.Add(relationship);
                break;
            }
        }

        if (azureRelationships.Count > 0)
        {
            rptRelationshipList.DataSource = azureRelationships;
            rptRelationshipList.DataBind();

        }
    }

    private void RenderHeader()
    {
        divTitleBar.InnerHtml = "Cloud Synchronization";

        HtmlTableCell cellBackButton = new HtmlTableCell();
        string referrer = Request.QueryString["referrer"];
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "/images/ui/icons/" + "back.png",
            referrer,
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty,
            StyleHelper.BackButtonCssClass,
            true);

        rowToolbarButtons.Cells.Add(cellBackButton);
    }

    private void RegisterResources()
    {
        // JavaScript Resources

        JS.RegisterJS(this, JS.ManagedScript.EktronJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronSiteData, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS, false);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS, false);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS, false);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS, false);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS, false);

        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "SyncRelationshipsJS", false);
        JS.RegisterJS(this, "../java/jfunct.js", "EktronJFunctJS", false);
        JS.RegisterJS(this, "../java/internCalendarDisplayFuncs.js", "EktronIntercalendarDisplayFuncs", false);
        JS.RegisterJS(this, "../java/toolbar_roll.js", "EktronToolbarRollJS", false);

        // CSS Resources

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronThickBoxCss, false);
        Css.RegisterCss(this, "css/ektron.workarea.sync.ie.css", "EktronWorkareaSyncIeCss", Css.BrowserTarget.LessThanEqualToIE7, false);

        // Style Helper Client Script

        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

}