using System;
using System.Text;
using System.Web.UI.HtmlControls;
using Ektron.Cms.API;
using Ektron.Cms;

public partial class SyncResources : System.Web.UI.UserControl
{
    private const string JavascriptResourceFormat = "Ektron.Workarea.Sync.Resources.{0} = \"{1}\"; ";

    private SiteAPI _siteApi;
    private HtmlGenericControl _resourceScriptBlock;

    public SyncResources()
    {
        _siteApi = new SiteAPI();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _resourceScriptBlock = new HtmlGenericControl("script");
        _resourceScriptBlock.Attributes.Add("type", "text/javascript");

        //Controls.Add(_resourceScriptBlock);

        JS.RegisterJS(this, JS.ManagedScript.EktronJS, false);
        JS.RegisterJS(this, _siteApi.AppPath + "sync/js/Ektron.Workarea.Sync.Resources.js", "SyncResourcesJS", false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder clientResources = new StringBuilder();
        clientResources.Append(CreateResourceString("deleteProfileDialogTitle", "Delete Profile"));
        clientResources.Append(CreateResourceString("deleteProfileDialogMessage", "Are you sure you want to delete this profile?"));
        clientResources.Append(CreateResourceString("deleteRelationshipDialogTitle", "Delete Relationship"));
        clientResources.Append(CreateResourceString("deleteRelationshipDialogMessage", _siteApi.EkMsgRef.GetMessage("js delete sync relationship message")));
        string maxValueString = "";
        if (DateTime.MaxValue.Year == 9999)
        {
            System.Globalization.CultureInfo culture = Ektron.Cms.Common.EkFunctions.GetCultureInfo(_siteApi.ContentLanguage.ToString());
            maxValueString = culture.Calendar.MaxSupportedDateTime.ToString();
        }
        else
        {
            maxValueString = DateTime.MaxValue.ToString();
        }
        clientResources.Append(CreateResourceString("dateTimeMaxValue", maxValueString));
        clientResources.Append(CreateResourceString("nextSyncTimeLabel", "Next Sync Time: "));
        clientResources.Append(CreateResourceString("nextSyncTimeNoneLabel", "None"));
        clientResources.Append(CreateResourceString("monthlyScheduleWarning", _siteApi.EkMsgRef.GetMessage("js warn day of month")));
        clientResources.Append(CreateResourceString("syncProfileDialogTitle", _siteApi.EkMsgRef.GetMessage("lbl sync confirm")));
        clientResources.Append(CreateResourceString("syncProfileDialogMessage", _siteApi.EkMsgRef.GetMessage("lbl sync confirm synchronization")));
        clientResources.Append(CreateResourceString("syncCompleteMessage", _siteApi.EkMsgRef.GetMessage("lbl syncended")));
        clientResources.Append(CreateResourceString("syncCanceledMessage", _siteApi.EkMsgRef.GetMessage("jssynccancel")));
        clientResources.Append(CreateResourceString("syncErrorMessage", "Synchronization Failed"));
        clientResources.Append(CreateResourceString("syncInProgressMessage", _siteApi.EkMsgRef.GetMessage("syncprogress")));
        clientResources.Append(CreateResourceString("retrievingStatusMessage", "Retrieving Status..."));
        clientResources.Append(CreateResourceString("resolveCancelButton", _siteApi.EkMsgRef.GetMessage("resolve cancel button")));
        clientResources.Append(CreateResourceString("resolveCloseButton", _siteApi.EkMsgRef.GetMessage("resolve close button")));
        clientResources.Append(CreateResourceString("resolveCommunicationError", _siteApi.EkMsgRef.GetMessage("resolve communication error")));
        clientResources.Append(CreateResourceString("resolveDatabaseError", _siteApi.EkMsgRef.GetMessage("resolve database error")));
        clientResources.Append(CreateResourceString("resolveResolveButton", _siteApi.EkMsgRef.GetMessage("resolve resolve button")));
        clientResources.Append(CreateResourceString("resolveSuccess", _siteApi.EkMsgRef.GetMessage("resolve success")));
        clientResources.Append(CreateResourceString("resolveSyncInProgressError", _siteApi.EkMsgRef.GetMessage("resolve sync in progress error")));
        clientResources.Append(CreateResourceString("resolveUnexpectedError", _siteApi.EkMsgRef.GetMessage("resolve unexpected error")));
        clientResources.Append(CreateResourceString("resolveDialogTitle", _siteApi.EkMsgRef.GetMessage("resolve dialog title")));
        clientResources.Append(CreateResourceString("resolveDialogMessage", _siteApi.EkMsgRef.GetMessage("resolve dialog message")));
        clientResources.Append(CreateResourceString("syncInProgressDialogTitle", _siteApi.EkMsgRef.GetMessage("lbl sync running confirm header")));
        clientResources.Append(CreateResourceString("noSyncStatusAvailable", _siteApi.EkMsgRef.GetMessage("last sync file not found msg")));
        clientResources.Append(CreateResourceString("initialSyncDialogMessage", _siteApi.EkMsgRef.GetMessage("lbl sync confirm synchronization")));
        clientResources.Append(CreateResourceString("initialSyncDialogCaption", _siteApi.EkMsgRef.GetMessage("lbl sync confirm synchronization caption")));
        clientResources.Append(CreateResourceString("noCertificatesFoundMessage", _siteApi.EkMsgRef.GetMessage("error getting certificates")));
        clientResources.Append(CreateResourceString("folderNotSyncableMessage", _siteApi.EkMsgRef.GetMessage("lbl sync folder not syncable")));
        clientResources.Append(CreateResourceString("folderNotSyncableCauseAMessage", _siteApi.EkMsgRef.GetMessage("lbl sync folder not syncable cause a")));
        clientResources.Append(CreateResourceString("contentNotSyncableMessage", _siteApi.EkMsgRef.GetMessage("lbl sync content not syncable")));
        clientResources.Append(CreateResourceString("contentNotSyncableCauseAMessage", _siteApi.EkMsgRef.GetMessage("lbl sync content not syncable cause a")));
        clientResources.Append(CreateResourceString("contentNotSyncableCauseBMessage", _siteApi.EkMsgRef.GetMessage("lbl sync content not syncable cause b")));
        clientResources.Append(CreateResourceString("contentNotSyncableCauseCMessage", _siteApi.EkMsgRef.GetMessage("lbl sync content not syncable cause c")));
        clientResources.Append(CreateResourceString("noCMSSitesFoundMessage", _siteApi.EkMsgRef.GetMessage("js no sync cms sites found")));
        clientResources.Append(CreateResourceString("serviceErrorMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception possible causes")));
        clientResources.Append(CreateResourceString("serviceErrorCauseAMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception causes a")));
        clientResources.Append(CreateResourceString("serviceErrorCauseBMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception causes b")));
        clientResources.Append(CreateResourceString("serviceErrorCauseCMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception causes c")));
        clientResources.Append(CreateResourceString("serviceErrorCauseDMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception causes d")));
        clientResources.Append(CreateResourceString("serviceErrorCauseEMessage", _siteApi.EkMsgRef.GetMessage("lbl sync windows service exception causes e")));
        clientResources.Append(CreateResourceString("enterServerNameMessage", _siteApi.EkMsgRef.GetMessage("js enter server name")));
        clientResources.Append(CreateResourceString("enterCertificateMessage", _siteApi.EkMsgRef.GetMessage("js enter certificate")));
        clientResources.Append(CreateResourceString("selectProfileMessage", _siteApi.EkMsgRef.GetMessage("lbl select sync configuration")));
        clientResources.Append(CreateResourceString("selectConfigEmptyMessage", _siteApi.EkMsgRef.GetMessage("jsselectconfigempty")));
        clientResources.Append(CreateResourceString("noSyncConfigMessage", _siteApi.EkMsgRef.GetMessage("js no sync config found")));
        clientResources.Append(CreateResourceString("relationshipReactivatedDialogTitle", _siteApi.EkMsgRef.GetMessage("jssyncrelationactivated")));
        clientResources.Append(CreateResourceString("relationshipReactivatedDialogMessage", _siteApi.EkMsgRef.GetMessage("sync rel activated")));

        clientResources.Append(CreateResourceString("isPostBack", IsPostBack.ToString()));
        clientResources.Append(CreateResourceString("rawUrl", Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.RawUrl).Replace("&amp;", "&")));
        clientResources.Append(CreateResourceString("sitePath", _siteApi.AppPath));
        clientResources.Append(CreateResourceString("syncPath", _siteApi.AppPath + "sync/"));
        clientResources.Append(CreateResourceString("syncHandlerPath", _siteApi.AppPath + "sync/SyncHandler.ashx"));
        clientResources.Append(CreateResourceString("syncStatusHandlerPath", _siteApi.AppPath + "sync/SyncStatusHandler.ashx"));

        //_resourceScriptBlock.InnerHtml = clientResources.ToString();
        Ektron.Cms.Framework.UI.JavaScript.RegisterJavaScriptBlock(this, clientResources.ToString(), false);
    }

    private string CreateResourceString(string id, string value)
    {
        return string.Format(JavascriptResourceFormat, id, value.Replace("\"", "\\\""));
    }
}
