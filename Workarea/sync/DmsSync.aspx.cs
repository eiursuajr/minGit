using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms.API;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_sync_DmsSync : System.Web.UI.Page
{
    private const string ParamContentId = "contentId";
    private const string ParamContentAssetId = "contentAssetId";
    private const string ParamContentLanguage = "contentLanguage";
    private const string ParamFolderId = "folderId";
    private const string ParamContentAssetVersion = "contentAssetVersion";
    private const string ParamIsMultiSite = "isMultisite";

    private SiteAPI _siteApi;

    /// <summary>
    /// Constructor
    /// </summary>
    public Workarea_sync_DmsSync()
    {
        _siteApi = new SiteAPI();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!_siteApi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0) ||
            (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) && !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser)))
        {
            pnlMessage.Visible = true;
            pnlDmsSync.Visible = false;

            lblErrorMessage.Text = _siteApi.EkMsgRef.GetMessage("sync dms logged out");
            hdnShowDialog.Value = bool.FalseString.ToLower();
        }
        else
        {
            pnlMessage.Visible = false;
            pnlDmsSync.Visible = true;
            hdnShowDialog.Value = bool.TrueString.ToLower();
        }

        RegisterResources();
        PopulateLabels();
        Utilities.ValidateUserLogin();
        // Populate hidden fields with parameter values
        // for client-side consumption.

        PopulateHiddenField(hdnContentAssetId, ParamContentAssetId);
        PopulateHiddenField(hdnContentAssetVersion, ParamContentAssetVersion);
        PopulateHiddenField(hdnContentId, ParamContentId);
        PopulateHiddenField(hdnContentLanguage, ParamContentLanguage);
        PopulateHiddenField(hdnFolderId, ParamFolderId);
        PopulateHiddenField(hdnIsMultisite, ParamIsMultiSite);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PopulateLabels()
    {
        btnCloseConfigDialog.Text = _siteApi.EkMsgRef.GetMessage("close title");
        btnStartSync.Text = _siteApi.EkMsgRef.GetMessage("btn sync now");
    }

    /// <summary>
    /// Populates the value of the specified hidden field with the data
    /// associated with the specified querystring param. Populates the 
    /// field with an empty string if parameter does not exist.
    /// </summary>
    /// <param name="hiddenField">HiddenField to populate</param>
    /// <param name="queryStringParam">Query string parameter</param>
    private void PopulateHiddenField(HiddenField hiddenField, string queryStringParam)
    {
        if (!string.IsNullOrEmpty(Request.QueryString[queryStringParam]))
        {
            hiddenField.Value = Request.QueryString[queryStringParam];
        }
        else
        {
            hiddenField.Value = string.Empty;
        }
    }

    /// <summary>
    /// Registers javascript and CSS resources.
    /// </summary>
    private void RegisterResources()
    {
        // JS Resources

        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronSiteData);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronStringJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS);

        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "SyncRelationshipsJS");
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.DmsSync.js", "SyncDmsJS");
        JS.RegisterJS(this, "../java/jfunct.js", "EktronJFunctJS");
        JS.RegisterJS(this, "../java/internCalendarDisplayFuncs.js", "EktronIntercalendarDisplayFuncs");
        JS.RegisterJS(this, "../java/toolbar_roll.js", "EktronToolbarRollJS");

        // CSS Resources

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronThickBoxCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        Css.RegisterCss(this, "css/ektron.workarea.sync.ie.css", "EktronWorkareaSyncIeCss", Css.BrowserTarget.LessThanEqualToIE7);
    }
}
