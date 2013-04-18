using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Reflection;
using System.Text;
using Ektron.Cms.Widget;
using Ektron.Cms;
using System.Net;
using System.Web.Caching;


public partial class Widgets_SalesForceChart : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _Username;
    private string _LinkURL;
    private string _ChartURL;
    private string _ChartTitle;
    [WidgetDataMember("")]
    public string Username { get { return _Username; } set { _Username = value; } }
    [WidgetDataMember("")]
    public string LinkURL { get { return _LinkURL; } set { _LinkURL = value; } }
    [WidgetDataMember("")]
    public string ChartURL { get { return _ChartURL; } set { _ChartURL = value; } }
    [WidgetDataMember("Salesforce Chart Widget")]
    public string ChartTitle { get { return _ChartTitle; } set { _ChartTitle = value; } }
    #endregion

    IWidgetHost _host;
    private ContentAPI _contentApi;
    private Boolean _disableLogin = false;

    #region Properties

    public long HostId
    {
        get
        {
            return _host.WidgetInfo.ID;
        }
    }

    public long OldHostId
    {
        get
        {
            return _host.WidgetInfo.ID - 1;
        }
    }

   

    public string DisableLogin
    {
        get
        {
            return _disableLogin.ToString().ToLower();
        }
    }

     

    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "SalesForce Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        _contentApi = new ContentAPI();
        ViewSet.SetActiveView(View);
    }
    
    protected override void  OnPreRender(EventArgs e)
    {
        RegisterJS();
 	     base.OnPreRender(e);
    }
    private void RegisterJS()
    {
        string sitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
        ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "EktronWidgetsSalesForceChartJs", sitePath + "/widgets/salesforcechart/js/ektron.widgets.salesforcechart.js");
        Ektron.Cms.API.JS.RegisterJSInclude(Page, sitePath + "/Workarea/PrivateData/js/Ektron.Cache.js", "EktronCache");
        Ektron.Cms.API.JS.RegisterJSInclude(Page, sitePath + "/Workarea/PrivateData/js/Ektron.Crypto.js", "EktronCrypto");
        Ektron.Cms.API.JS.RegisterJSInclude(Page, sitePath + "/Workarea/PrivateData/js/Ektron.PrivateData.aspx", "EktronPrivateData");

        StringBuilder initJs = new StringBuilder();
        initJs.AppendLine(@"
            <script type=""text/javascript"" id=""EktronSalesForceChartInit"">
                var EktronWidgetsSalesForceChart" + this.HostId + @";
		$ektron(document).one(""Ektron.Ready"", function(){
                    EktronWidgetsSalesForceChart" + this.HostId + @" = new Ektron.Widgets.salesForceChart();
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_chartUrl('" + ChartURL + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_linkUrl('" + LinkURL + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_disableLogin('" + this.DisableLogin + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_widgetId('" + this.HostId + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_widgetMarkupId('" + divSalesForceChartData.ClientID + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_username('" + this.Username + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".set_passwordFieldId('" + txtPassword.ClientID + @"');
                    EktronWidgetsSalesForceChart" + this.HostId + @".init();
                    $ektron(""#EktronSalesForceChartInit"").remove();
                });
            </script>
        ");
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "EktronWidgetsSalesForceChart" + this.HostId + @"InitJs", initJs.ToString(), false);
    }

    private void JS(Page Page, Type type, string p, string p_4)
    {
        throw new NotImplementedException();
    }

    void EditEvent(string settings)
    {
        try
        {
            txtUsername.Text = Username;
            txtChartURL.Text = ChartURL;
            txtLinkURL.Text = LinkURL;
            txtChartTitle.Text = ChartTitle;
            _disableLogin = true;
            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            divSalesForceChartData.InnerHtml = "Error loading settings";
            ViewSet.SetActiveView(View);
        }
         
        ViewSet.SetActiveView(Edit);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
          try
        {
            Username = txtUsername.Text;
            LinkURL = txtLinkURL.Text;
            ChartURL = txtChartURL.Text;
            ChartTitle = txtChartTitle.Text;
       
        }
        catch
        {
            divSalesForceChartData.InnerHtml = "Error saving settings";
            ViewSet.SetActiveView(View);
        }
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
       Boolean processWidget = false;
        string browser = Request.Browser.Browser;
        int version = Request.Browser.MajorVersion;

        switch (browser.ToLower())
        {
            case "firefox":
                if (version >= 3)
                    processWidget = true;
                break;
            case "ie":
                if (version >= 7)
                    processWidget = true;
                break;
            case "applemac-safari":
                if (version >= 3)
                    processWidget = true;
                break;
        }

        if (processWidget == true)
        {
            
            btnSave.OnClientClick = @"EktronWidgetsSalesForceChart" + this.HostId + @".save();";

            try
            {
                if (Request.Params["id"] != null && long.Parse(Request.Params["id"]) == new SiteAPI().UserId)
                {
                    HttpCookie cookie = Request.Cookies["SalesForceLogin" + _host.WidgetInfo.ID];

                    if (cookie != null && cookie.Value == "y")
                    {
                        _disableLogin = true;

                        aSalesForceLink.Visible = true;
                        aSalesForceLink.NavigateUrl = LinkURL;

                        imgSalesForceChart.Visible = true;
                        imgSalesForceChart.ImageUrl = ChartURL;
                    }
                    else
                    {
                        _disableLogin = false;
                        aSalesForceLink.Visible = false;
                        imgSalesForceChart.Visible = false;
                    }

                    _host.Title = Server.HtmlEncode(ChartTitle);
                }
                else
                {
                    divSalesForceChartData.InnerHtml = "Only the owner of this widget may view it";
                }
            }
            catch
            {
                divSalesForceChartData.InnerHtml = "Error loading widget";
            }
        }
        else
        {
            ViewSet.SetActiveView(UnsupportedBrowser);
        }  
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

     
}


