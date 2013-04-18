using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;

public partial class SyncDialogs : System.Web.UI.UserControl, ISyncDialogsView
{
    private readonly SyncDialogsPresenter _presenter;
    private readonly SiteAPI _siteApi;

    public SyncDialogs()
    {
        _presenter = new SyncDialogsPresenter(this);
        _siteApi = new SiteAPI();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        RegisterResources();

        _presenter.Initialize();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Confirm Dialog
        btnConfirmCancel.Text = _siteApi.EkMsgRef.GetMessage("generic cancel");
        btnConfirmCancel.ToolTip = btnConfirmCancel.Text;
        btnConfirmOk.Text = _siteApi.EkMsgRef.GetMessage("btn ok");
        btnConfirmOk.ToolTip = btnConfirmOk.Text;

        // Status Dialog
        lblSyncStatus.Text = string.Format(_siteApi.EkMsgRef.GetMessage("lbl sync status"), " <span class=\"statusHeaderProfileId\"></span>");
        btnCloseSyncStatus.Text = _siteApi.EkMsgRef.GetMessage("close title");
        btnCloseSyncStatus.ToolTip = btnCloseSyncStatus.Text;

        PrepareCreateRelationshipDialog();
        PrepareCreatCloudRelationshipDialog();
        PrepareResolveConflictsDialog();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PrepareCreateRelationshipDialog()
    {
        // Create Relationship Dialog
        lblCreateServerRelationship.Text = _siteApi.EkMsgRef.GetMessage("create server relationship");
        lblConnectToRemoteServer.Text = _siteApi.EkMsgRef.GetMessage("lbl connect to remote server");
        lblConnectToRemoteServer.ToolTip = lblConnectToRemoteServer.Text;
        lblRemoteServer.Text = _siteApi.EkMsgRef.GetMessage("lbl remote server");
        lblRemoteServer.ToolTip = lblRemoteServer.Text;
        lblChooseCertificate.Text = _siteApi.EkMsgRef.GetMessage("lbl choose sync certificate");
        lblChooseCertificate.ToolTip = lblChooseCertificate.Text;

        spanCloseDialog.InnerText = _siteApi.EkMsgRef.GetMessage("close title");
        lblStep1of3.Text = "Step 1 of 3";
        lblStep1of3.ToolTip = lblStep1of3.Text;
        lblStep2of3.Text = "Step 2 of 3";
        lblStep2of3.ToolTip = lblStep2of3.Text;
        lblStep3of3.Text = "Step 3 of 3";
        lblStep3of3.ToolTip = lblStep3of3.Text;
        lblChooseCmsSite.Text = "Choose a CMS Site";
        lblChooseCmsSite.ToolTip = lblChooseCmsSite.Text;
        lblRemoteServer2.Text = _siteApi.EkMsgRef.GetMessage("lbl remote server");
        lblRemoteServer2.ToolTip = lblPortNumber2.Text;
        lblPortNumber2.Text = _siteApi.EkMsgRef.GetMessage("lbl sync portnum");
        lblPortNumber2.ToolTip = lblPortNumber2.Text;
        btnConnect.Text = _siteApi.EkMsgRef.GetMessage("btn connect");
        btnConnect.ToolTip = btnConnect.Text;
        btnCancelStep1.Text = _siteApi.EkMsgRef.GetMessage("generic cancel");
        btnCancelStep1.ToolTip = btnCancelStep1.Text;
        btnCancelStep2.Text = _siteApi.EkMsgRef.GetMessage("generic cancel");
        btnCancelStep2.ToolTip = btnCancelStep2.Text;
        btnCancelStep3.Text = _siteApi.EkMsgRef.GetMessage("generic cancel");
        btnCancelStep3.ToolTip = btnCancelStep3.Text;
        btnBackStep2.Text = _siteApi.EkMsgRef.GetMessage("btn back");
        btnBackStep2.ToolTip = btnBackStep2.Text;
        btnBackStep3.Text = _siteApi.EkMsgRef.GetMessage("btn back");
        btnBackStep3.ToolTip = btnBackStep3.Text;
        btnNextStep2.Text = _siteApi.EkMsgRef.GetMessage("btn next");
        btnNextStep2.ToolTip = btnNextStep2.Text;
        btnCreate.Text = _siteApi.EkMsgRef.GetMessage("btn create");
        btnCreate.ToolTip = btnCreate.Text;
        btnToggleCreateSyncDirection.Text = _siteApi.EkMsgRef.GetMessage("btn switch sync direction");
        btnToggleCreateSyncDirection.ToolTip = btnToggleCreateSyncDirection.Text;
        lblDatabaseCopiedFrom.Text = _siteApi.EkMsgRef.GetMessage("lbl sync database copied from");
        lblReplacingDatabase.Text = _siteApi.EkMsgRef.GetMessage("lbl sync replacing database");
        lblConfigureSync.Text = _siteApi.EkMsgRef.GetMessage("lbl configure intiial sync");
        lblConfigureSync.ToolTip = lblConfigureSync.Text;
        lblLocalSiteServerNameHeader.Text = _siteApi.EkMsgRef.GetMessage("js server name") + ":";
        lblLocalSiteServerNameHeader.ToolTip = lblLocalSiteServerNameHeader.Text;
        lblRemoteSiteServerNameHeader.Text = _siteApi.EkMsgRef.GetMessage("js server name") + ":";
        lblRemoteSiteServerNameHeader.ToolTip = lblRemoteSiteServerNameHeader.Text;

        localSite.Value = LocalConnection.DatabaseName;
        localDatabaseServer.Value = LocalConnection.ServerName;
        localServer.Value = ServerName;

        litLocalSiteDatabaseName.Text = LocalConnection.DatabaseName;
        lblLocalSiteServerName.Text = LocalConnection.ServerName;

        if (MultiSites != null)
        {
            selectLocalMultiSite.Visible = true;

            // Create "All Folders" option
            ListItem allFoldersOption = new ListItem();
            allFoldersOption.Text = _siteApi.EkMsgRef.GetMessage("lbl all folders");
            allFoldersOption.Value = "-1";

            selectLocalMultiSite.Items.Add(allFoldersOption);
            foreach (FolderData site in MultiSites)
            {
                if (site.Id > 0)
                {
                    ListItem siteOption = new ListItem();
                    siteOption.Text = site.Name;
                    siteOption.Value = site.Id.ToString();
                    siteOption.Attributes.Add("title", site.Name);

                    selectLocalMultiSite.Items.Add(siteOption);
                }
            }

            lblForSelectLocalMultiSite.Text = _siteApi.EkMsgRef.GetMessage("lbl site");
            lblForSelectRemoteMultiSite.Text = _siteApi.EkMsgRef.GetMessage("lbl site path");
            lblForSelectLocalMultiSitePath.Text = _siteApi.EkMsgRef.GetMessage("lbl site path");
        }
    }

    private void PrepareCreatCloudRelationshipDialog()
    {
        // Create Relationship Dialog
        lblCreateCloudRelationship.Text = _siteApi.EkMsgRef.GetMessage("syncCloudCreateServerRelationshipLbl");
        lblCloudSQLServer.Text = _siteApi.EkMsgRef.GetMessage("syncCloudSQLServer");
        lblCloudSQLServer.ToolTip = lblCloudSQLServer.Text;
        lblCloudStep1.Text = _siteApi.EkMsgRef.GetMessage("syncCloudStep1");
        lblCloudStep1.ToolTip = lblCloudStep1.Text;
        lblConnectToCloudServer.Text = _siteApi.EkMsgRef.GetMessage("syncCloudConnectLbl");
        lblConnectToCloudServer.ToolTip = lblConnectToCloudServer.Text;
        lblIPAddress.Text =  _siteApi.EkMsgRef.GetMessage("syncCloudLocalIP");
        lblIPAddress.ToolTip = lblIPAddress.Text;
        lblBlobStorage.Text =  _siteApi.EkMsgRef.GetMessage("syncCloudBlob");
        lblBlobStorage.ToolTip = lblBlobStorage.Text;
        lblAccountName.Text = _siteApi.EkMsgRef.GetMessage("syncCloudBlobAccountName");
        lblAccountName.ToolTip = lblAccountName.Text;
        lblContainerName.Text = _siteApi.EkMsgRef.GetMessage("syncCloudBlobContainerName");
        lblContainerName.ToolTip = lblContainerName.Text;
        lblAccountKey.Text = _siteApi.EkMsgRef.GetMessage("syncCloudBlobAccountKey");
        lblAccountKey.ToolTip = lblAccountKey.Text;
        lblCloudDomain.Text = _siteApi.EkMsgRef.GetMessage("syncCloudDomain");
        lblCloudDomain.ToolTip = lblCloudDomain.Text;
        btnCloudConnect.Text = _siteApi.EkMsgRef.GetMessage("syncCloudBtnConnect");
        btnCloudConnect.ToolTip = btnCloudConnect.Text;
        btnCancelStep1Cloud.Text = _siteApi.EkMsgRef.GetMessage("generic cancel");
        btnCancelStep1Cloud.ToolTip = btnCancelStep1Cloud.Text;

        localDatabaseNameCloud.Value = LocalConnection.DatabaseName;
        localDatabaseServerCloud.Value = LocalConnection.ServerName;
        localServerNameCloud.Value = ServerName;

        txtIPAddress.Value = this.LocalServerIPAddress.ToString();

    }

    /// <summary>
    /// 
    /// </summary>
    private void PrepareResolveConflictsDialog()
    {
        btnCancelResolveSyncCollisions.Text = _siteApi.EkMsgRef.GetMessage("resolve cancel button");
        btnNextResolveSyncCollisions.Text = _siteApi.EkMsgRef.GetMessage("resolve resolve button");
        litResolveDialogTitle.Text = _siteApi.EkMsgRef.GetMessage("resolve dialog title");
        litResolveDialogMessage.Text = _siteApi.EkMsgRef.GetMessage("resolve dialog message");
    }

    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS, false);
    }

    #region ISyncDialogsView Members

    public ConnectionInfo LocalConnection { get; set; }

    public List<string> LocalSitePaths { get; set; }

    public List<FolderData> MultiSites { get; set; }

    public string ServerName { get; set; }

    public IPAddress LocalServerIPAddress { get; set; }

    #endregion
}
