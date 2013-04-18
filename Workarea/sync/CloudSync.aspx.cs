using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using System.Web.UI.HtmlControls;
using System.Net;
using Ektron.FileSync.Common;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.API;
using System.Xml;

/*
 * This page is for a demo/poc only
 */

public partial class Workarea_sync_CloudSync : System.Web.UI.Page
{
    private readonly CommonApi _commonApi;
    public readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;
    public Relationship cloudRelationship;
    public long cloudRelationshipId;

    public Workarea_sync_CloudSync()
    {
        _commonApi = new CommonApi();
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
        this.StripeRows();
        cloudRelationship = getCloudRelationship();
        if (!Page.IsPostBack)
        {
            tbIPAddress.Text = GetServerIP();
            tbLocalSQL.Text = _siteApi.RequestInformationRef.ConnectionString;
            tbLocalSitePath.Text = _siteApi.RequestInformationRef.SitePath;

            if (cloudRelationship != null)
            {
                cloudRelationshipId = cloudRelationship.Id;
                tbLocalSitePath.Text = cloudRelationship.LocalSite.SitePath;
                tbSQLServer.Text = cloudRelationship.RemoteSite.ConnectionString;
                string[] data = cloudRelationship.RemoteSite.Address.Split('|');
                tbIPAddress.Text = "";// data[0];
                tbBlobStorage.Text = "";//data[1];
                tbAccountName.Text = data[1];
                tbContainerName.Text = data[0];
                tbAccountKey.Text = data[2];
                tbCloudDomain.Text = "";//data[5];
            }
        }
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        // Demo POC Only
        SyncHandlerController.CreateRelationshipResult result;
        SyncHandlerController _controller = new SyncHandlerController();
        ConnectionInfo LocalConnection = new ConnectionInfo(_siteApi.RequestInformationRef.ConnectionString);

        if (this.cloudRelationship == null)
        {
            Relationship relationship = _controller.CreateCloudRelationship(
                LocalConnection.DatabaseName,
                LocalConnection.ServerName,
                tbLocalSitePath.Text,
                -1,
                tbSQLServer.Text,
                tbIPAddress.Text,
                tbBlobStorage.Text,
                tbAccountName.Text,
                tbContainerName.Text,
                tbAccountKey.Text,
                tbCloudDomain.Text,
                "",//parameters.Certificate,
                Ektron.Cms.Sync.Client.SyncDirection.Upload,
                out result);

            cloudRelationship = getCloudRelationship();
            //cloudRelationship.Delete();
        }
        else
        {
            cloudRelationship.LocalSite.SitePath = tbLocalSitePath.Text;
            cloudRelationship.RemoteSite.ConnectionString = tbSQLServer.Text;
            cloudRelationship.RemoteSite.Address =  tbContainerName.Text +
                 "|" + tbAccountName.Text +
                "|" + tbAccountKey.Text;
               
            cloudRelationship.Save("Azure");
        }

        ltrMessages.Text = "Relationship Saved";
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("sync.aspx");
    }

    private Relationship getCloudRelationship()
    {
        Relationship azureRelationship = null;

        List<Relationship> relationships = Relationship.GetRelationships();
        foreach (Relationship relationship in relationships)
        {
            if (relationship.Name == "Azure")
            {
                azureRelationship = relationship;
                break;
            }
        }

        return azureRelationship;
    }


    private void RegisterResources()
    {
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
        ektronClientScript.Text = _styleHelper.GetClientScript();
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

    private void StripeRows()
    {
        bool lastRowStriped = false;
        foreach (HtmlTableRow row in tblProfile.Rows)
        {
            if (row.Visible)
            {
                if (!lastRowStriped)
                {
                    row.Attributes.Add("class", "stripe");
                    lastRowStriped = true;
                }
                else
                {
                    lastRowStriped = false;
                }
            }
        }
    }

    private string GetServerIP()
    {
        string ipaddress = String.Empty;
        string strHostName = System.Net.Dns.GetHostName();
        IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
        foreach (IPAddress address in ipHostInfo.AddressList)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipaddress = address.ToString();
            }
        }
        return ipaddress;
    }

}