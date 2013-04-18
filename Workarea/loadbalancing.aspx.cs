using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.API;
using Ektron.Cms;
using System.Web.UI.HtmlControls;
using System.Text;
using Ektron.Cms.Content;

public partial class Workarea_loadbalancing : System.Web.UI.Page
{
    private const string LoadBalancingCssPath = "csslib/ektron.workarea.loadbalancing.css";
    private const string LoadBalancingCssId = "LoadBalancingCss";
    private const string LoadBalancingJsPath = "java/ektron.workarea.loadbalancing.js";
    private const string LoadBalancingJsId = "LoadBalancingJs";
    private const string LoadBancingResourcesJsId = "LoadBalancingResourcesJs";
    private const string ResourceScriptFormat = "Ektron.Workarea.LoadBalancing.Resources.{0}='{1}'; ";
    private const string HelpTopicKey = "LoadBalancingRefresh_v801";

    private SiteAPI _siteApi;
    private StyleHelper _styleHelper;
    private EkContent m_refContent = null;

    private ContentAPI capi = null;
    

    protected void Page_Init(object sender, EventArgs e)
    {
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
       
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        capi = new ContentAPI();
        m_refContent = new EkContent(capi.RequestInformationRef);
        if (!(_siteApi.IsAdmin() || m_refContent.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin, capi.RequestInformationRef.UserId, false)))
        {       
            
                Response.Redirect("blank.htm", false);
        }
        else
        {
            InitializeHeader();
            InitializeTitleBar();
            InitializeContent();
        }
    }

    /// <summary>
    /// Initializes the page's HEAD content.
    /// </summary>
    private void InitializeHeader()
    {
        // Prepare page title content.

        Title = _siteApi.EkMsgRef.GetMessage("lbl load balancing");
        
        // Load style helper scripts.

        ltrStyleSheetJS.Text = _styleHelper.GetClientScript();

        // Register CSS and JS assets

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Css.RegisterCss(this, LoadBalancingCssPath, LoadBalancingCssId);

        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);        
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronStyleHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJS(this, LoadBalancingJsPath, LoadBalancingJsId, true);

        JS.RegisterJSBlock(this, BuildStringResourceScript(), LoadBancingResourcesJsId);
    }

    private void InitializeTitleBar()
    {
        // Prepare title bar content

        divTitleBar.InnerText = _siteApi.EkMsgRef.GetMessage("lbl load balancing");

        // Add toolbar buttons

        HtmlTableCell statusButtonCell = new HtmlTableCell();
        HtmlTableCell helpButtonCell = new HtmlTableCell();

        statusButtonCell.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            "images/UI/Icons/history.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("load balance view status"),
            "",
            " onclick=\"Ektron.Workarea.LoadBalancing.manualStatus();\"", StyleHelper.HistoryButtonCssClass);

        helpButtonCell.InnerHtml = _styleHelper.GetHelpButton(HelpTopicKey, string.Empty);

        ektronToolbarRow.Cells.Add(statusButtonCell);
		ektronToolbarRow.Cells.Add(StyleHelper.ActionBarDividerCell);
        ektronToolbarRow.Cells.Add(helpButtonCell);
    }

    private void InitializeContent()
    {
        lblForceLBSyncHeader.Text = _siteApi.EkMsgRef.GetMessage("force load balance button");
        spanForceLoadBalancedSyncDesc.InnerText = _siteApi.EkMsgRef.GetMessage("force load balance confirm");
        spanStatusDialogHeader.InnerText = _siteApi.EkMsgRef.GetMessage("load balance refresh");

        switch (GetLoadBalanceStatus())
        {
            case LoadBalanceStatus.OK:
                linkStart.InnerText = _siteApi.EkMsgRef.GetMessage("load balance button start");
                linkStart.Attributes.Add("class", "button buttonNoIcon greenHover forceLoadBalancedSyncButton");
                break;
            case LoadBalanceStatus.InProgress:
                linkStart.InnerText = _siteApi.EkMsgRef.GetMessage("load balance button in progress");
                linkStart.Attributes.Add("class", "button buttonNoIcon forceLoadBalancedSyncButton");
                break;
            case LoadBalanceStatus.Disabled:
                linkStart.InnerText = _siteApi.EkMsgRef.GetMessage("load balance button disabled");
                linkStart.Attributes.Add("class", "button buttonNoIcon forceLoadBalancedSyncButton");
                break;
        }
    }

    private string BuildStringResourceScript()
    {
        StringBuilder resourceScript = new StringBuilder();

        resourceScript.AppendFormat(
            ResourceScriptFormat, 
            "noStatus",
            _siteApi.EkMsgRef.GetMessage("force load balance no status"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "success",
            _siteApi.EkMsgRef.GetMessage("force load balance success"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "failed",
            _siteApi.EkMsgRef.GetMessage("force load balance failed"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "notAuthorized",
            _siteApi.EkMsgRef.GetMessage("force load balance auth"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "inProgress",
            _siteApi.EkMsgRef.GetMessage("force load balance in progress"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "buttonInProgress",
            _siteApi.EkMsgRef.GetMessage("load balance button in progress"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "buttonStart",
            _siteApi.EkMsgRef.GetMessage("load balance button start"));

        resourceScript.AppendFormat(
            ResourceScriptFormat,
            "retrievingStatus",
            _siteApi.EkMsgRef.GetMessage("force load balance retrieving status"));

        return resourceScript.ToString();
    }

    private LoadBalanceStatus GetLoadBalanceStatus()
    {
        LoadBalanceStatus status = LoadBalanceStatus.OK;

        LoadBalanceManager manager = new LoadBalanceManager(HttpContext.Current);
        if (!manager.CanForceLoadBalancedSync)
        {
            status = LoadBalanceStatus.Disabled;
        }
        else
        {
            LoadBalanceStatusResponse response = manager.GetStatus();
            if (response.Entries.Count > 0 && !response.IsComplete)
            {
                status = LoadBalanceStatus.InProgress;
            }
        }

        return status;
    }

    private enum LoadBalanceStatus
    {
        InProgress,
        Disabled,
        OK
    }
}
