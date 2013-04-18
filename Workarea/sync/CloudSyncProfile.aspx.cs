using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;
using Ektron.FileSync.Common;

using ConflictResolutionPolicy = Ektron.Cms.Sync.Client.ConflictResolutionPolicy;
using SyncDirection = Ektron.Cms.Sync.Client.SyncDirection;

public partial class CloudSyncProfile : Page, ISyncProfileView
{
    private const string FilterDelimiter = ",";
    private const string RelativeImagePath = "images/ui/icons/";
    private const string FileIcon = "content.png";
    private const string FolderIcon = "folder.png";
    private const string SiteFolderIcon = "folderSite.png";

    private readonly SyncProfilePresenter _presenter;
    private readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;

    private ProfileParameters _parameters;

    /// <summary>
    /// Constructor
    /// </summary>
    public CloudSyncProfile()
    {
        _presenter = new SyncProfilePresenter(this);
        _styleHelper = new StyleHelper();
        _siteApi = new SiteAPI();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Utilities.ValidateUserLogin())
            return;
        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        // Process the request parameters.
        _parameters = new ProfileParameters(Request);

        RegisterResources();

        // Load labels and assorted static UI elements from
        // the relevent resource files, etc.
        PopulateLabels();

        // Clear any pre-existing error messages.
        ClearErrors();

        // Direct the presenter to initialize the display.
        _presenter.InitializeView(_parameters.Id, _parameters.Action);

        // Set display mode for client scripts.
        hdnDisplayMode.Value = _parameters.Action.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlProfile.Enabled = _presenter.CanEdit;
        ssScheduleOptions.IsEnabled = _presenter.CanEdit;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        switch (_parameters.Action)
        {
            case ProfilePresentationMode.Add:
            case ProfilePresentationMode.Edit:
                if (IsPostBack)
                {
                    Save();
                }
                break;
            case ProfilePresentationMode.View:
                break;
            case ProfilePresentationMode.Delete:
                Delete();
                break;
        }
    }

    #region ISyncProfileView Members

    /// <summary>
    /// Gets or sets the profile displayed on this page.
    /// </summary>
    public Profile Data { get; set; }

    /// <summary>
    /// Gets or sets the tree of synchronizable content associated 
    /// with the profile.
    /// </summary>
    public ContentNode PackageContent { get; set; }


    public List<ScopeConfiguration> ScopeConfigs { get; set; }

    /// <summary>
    /// Binds this page's profile data to the UI.
    /// </summary>
    public void Bind()
    {
        if (Data != null)
        {
            RenderHeader(Data);

            if (!IsPostBack)
            {
                ProfilePresentationServices _presentationServices = new ProfilePresentationServices(Data);

                txtLocalSite.Enabled = false;
                txtRemoteSite.Enabled = false;
                txtMultiSiteFolder.Enabled = false;

                txtScheduleName.Text = Data.Name;
                txtLocalSite.Text = Data.Parent.LocalSite.Connection.DatabaseName;
                txtRemoteSite.Text = Data.Parent.RemoteSite.Connection.DatabaseName;

                tbSQLServer.Text = Data.Parent.RemoteSite.ConnectionString;
                string[] azureData = Data.Parent.RemoteSite.Address.Split('|');
                tbAccountName.Text = azureData[1];
                tbContainerName.Text = azureData[0];
                tbAccountKey.Text = azureData[2];

                if (Data.Parent.MultiSite.IsMultiSite)
                {
                    trMultiSiteRow.Visible = true;
                    txtMultiSiteFolder.Text = Data.Parent.MultiSite.FolderId.ToString();
                }
                else
                {
                    trMultiSiteRow.Visible = false;
                }

                // Determine if database sync is allowed, if so
                // display option and populate according to the
                // current profile. Otherwise, hide and deselect.

                if (_presentationServices.AllowDatabaseSync)
                {
                    chkDatabase.Checked = Data.SynchronizeDatabase;
                    chkDatabase.Visible = true;
                    cbScope.Visible = true;

                    // Populate Scope list
                    if (Data.SynchronizeDatabase)
                    {
                        if (Data.Scope != null && Data.Scope.Count > 0)
                        {
                            foreach (SyncDBScope scope in Data.Scope)
                            {
                                cbScope.Items.FindByValue(scope.ToString()).Selected = true;
                            }
                        }
                        else
                        {
                            cbScope.Items.FindByValue("ektron").Selected = true;
                        }

                        // Show Tables lists in a tool tip
                        if (this.ScopeConfigs != null)
                        {
                            bool showCustomScope = false;
                            foreach (ScopeConfiguration scopeConfig in this.ScopeConfigs)
                            {
                                try
                                {
                                    cbScope.Items.FindByValue(scopeConfig.Name).Attributes["class"] = "addtooltip";
                                    StringBuilder tableListSB = new StringBuilder();
                                    if (scopeConfig.Tables != null && scopeConfig.Tables.Count > 0)
                                    {
                                        foreach (string tablename in scopeConfig.Tables)
                                        {
                                            tableListSB.Append(tablename + ", ");
                                        }
                                    }
                                    if (tableListSB.Length == 0)
                                    {
                                        tableListSB.Append("Table list not currently available for this scope.");
                                    }
                                    ListItem lbItem = cbScope.Items.FindByValue(scopeConfig.Name);
                                    lbItem.Attributes["title"] = String.Format("Tables currently defined in scope : {0} | {1}", lbItem.Text , tableListSB.ToString());
                                }
                                catch { }

                                if (scopeConfig.Name == "custom" )
                                {
                                    showCustomScope = true;
                                }
                            }

                            // If there are no custom tables don't show the custom scope
                            if (!showCustomScope)
                            {
                                cbScope.Items.Remove(cbScope.Items.FindByValue("custom"));
                            }
                        }
                    }

                    // Package sync should be disabled until it
                    // is completely implemented.

                    chkPackage.Visible = false;
                }
                else
                {
                    chkDatabase.Checked = false;
                    chkDatabase.Visible = false;
                    chkPackage.Visible = false;
                    cbScope.Visible = false;
                }

                chkWorkarea.Checked = Data.SynchronizeWorkarea;
                chkTemplates.Checked = Data.SynchronizeTemplates;
                chkBinaries.Checked = Data.SynchronizeBinaries;

                rdoConflictResolution.SelectedValue = Data.ConflictResolution.ToString();
                rdoDirection.SelectedValue = Data.Direction.ToString();

                rdoFilters.SelectedValue = Data.Filters.Type.ToString();

                if (Data.Filters.Files != null)
                {
                    foreach (string fileFilter in Data.Filters.Files)
                    {
                        if (txtFileFilters.Text.Length > 0)
                        {
                            txtFileFilters.Text += ", ";
                        }

                        txtFileFilters.Text += fileFilter;
                    }
                }

                if (Data.Filters.Directories != null)
                {
                    foreach (string directoryFilter in Data.Filters.Directories)
                    {
                        if (txtDirectoryFilters.Text.Length > 0)
                        {
                            txtDirectoryFilters.Text += ", ";
                        }

                        txtDirectoryFilters.Text += directoryFilter;
                    }
                }

                chkRestoration.Checked = Data.AllowRestoration;
                chkRestoration.Enabled = false;

                ssScheduleOptions.Interval = Data.Schedule.Recurrence;
                ssScheduleOptions.StartTime = Data.Schedule.StartTime;

                if (PackageContent != null)
                {
                    PopulateTree();
                }

                StripeRows();
            }
        }
    }

    /// <summary>
    /// Saves the profile with changes entered via the UI.
    /// </summary>
    public void Save()
    {
        Data.Name = txtScheduleName.Text;
        Data.Filters = GetFilters();
        Data.SynchronizeDatabase = chkDatabase.Checked;
        Data.SynchronizeWorkarea = chkWorkarea.Checked;
        Data.SynchronizeTemplates = chkTemplates.Checked;
        Data.SynchronizeBinaries = chkBinaries.Checked;
        Data.ConflictResolution = (ConflictResolutionPolicy)Enum.Parse(typeof(ConflictResolutionPolicy), rdoConflictResolution.SelectedValue);
        Data.Direction = (SyncDirection)Enum.Parse(typeof(SyncDirection), rdoDirection.SelectedValue);
        Data.Schedule = ScheduleFactory.Create(ssScheduleOptions.Interval);
        Data.Schedule.StartTime = ssScheduleOptions.StartTime;
        Data.Scope = getScope();

        bool success = true;

        success = isSubDatabaseValid();

        try
        {
            _presenter.Save(Data);
        }
        catch (InvalidProfileNameException)
        {
            DisplayError("Please specify a valid profile name. Profile names cannot be empty and must be unique.");
            success = false;
        }
        catch (InvalidSyncItemsException)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.Append("'Items to Synchronize' selection is invalid:");
            errorMessage.Append("<ul>");
            errorMessage.Append("<li>");
            errorMessage.Append("At least one synchronization item must be selected.");
            errorMessage.Append("</li>");
            errorMessage.Append("<li>");
            errorMessage.Append("Only Database and Templates can be chosen together for synchronization.");
            errorMessage.Append("</li>");
            errorMessage.Append("</ul>");

            DisplayError(errorMessage.ToString());

            success = false;
        }
        catch (InvalidFiltersException ex)
        {
            StringBuilder errorMessage = new StringBuilder();

            if (ex.InvalidFileFilters != null && ex.InvalidFileFilters.Count > 0)
            {
                errorMessage.Append("One or more file filters contain an illegal character ( \\ / : ? < > | ):<br/>");
                errorMessage.Append("<ul>");

                foreach (string fileFilter in ex.InvalidFileFilters)
                {
                    errorMessage.Append("<li>");
                    errorMessage.Append(fileFilter);
                    errorMessage.Append("</li>");
                }

                errorMessage.Append("</ul>");
            }

            if (ex.InvalidDirectoryFilters != null && ex.InvalidDirectoryFilters.Count > 0)
            {
                errorMessage.Append("One or more directory filters contain an illegal character ( \\ / ? : * < > | \" ):<br/>");
                errorMessage.Append("<ul>");

                foreach (string directoryFilter in ex.InvalidDirectoryFilters)
                {
                    errorMessage.Append("<li>");
                    errorMessage.Append(directoryFilter);
                    errorMessage.Append("</li>");
                }

                errorMessage.Append("</ul>");
            }

            DisplayError(errorMessage.ToString());
            success = false;
        }

        // Redirect the user to profile list if successfully saved.

        if (success)
        {
            if (_parameters.Action == ProfilePresentationMode.Add)
            {
                Response.Redirect("Sync.aspx", true);
            }
            else
            {
                Response.Redirect("CloudSyncProfile.aspx?action=view&id=" + Data.Id.ToString(), true);
            }
        }
    }

    /// <summary>
    /// Deletes the current profile.
    /// </summary>
    public void Delete()
    {
        _presenter.DeleteProfile(Data);

        // Redirect the user back to the relationship list
        // after deleting the profile.

        Response.Redirect("Sync.aspx", true);
    }

    #endregion

    #region ISyncView Members

    public void DisplayError(string message)
    {
        divErrorMessage.Visible = true;
        divErrorMessage.InnerHtml = message;
    }

    #endregion

    public bool isSubDatabaseValid()
    {
        bool isValid = true;
        if (chkDatabase.Checked)
        {
            isValid = false;
            bool subSelected = false;
            for (int i = 0; i < cbScope.Items.Count; i++)
            {
                if (cbScope.Items[i].Selected)
                {
                    subSelected = true;
                    isValid = true;
                    break;
                }
            }
            if (!subSelected)
            {
                DisplayError("Please select a Database sub-category under Items to Syncronize e.g. Ektron or History.");
                isValid = false;
            }
        }
        return isValid;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RenderHeader(Profile profile)
    {
        switch (_parameters.Action)
        {
            case ProfilePresentationMode.Add:
                RenderHeaderForAddMode(profile);
                break;
            case ProfilePresentationMode.Edit:
                RenderHeaderForEditMode(profile);
                break;
            case ProfilePresentationMode.View:
                RenderHeaderForViewMode(profile);
                break;
        }

        HtmlTableCell cellHelpButton = new HtmlTableCell();

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="profile"></param>
    private void RenderHeaderForViewMode(Profile profile)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("view sync titlebar") + " \"" + profile.Name + "\"");

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellEditButton = new HtmlTableCell();
            cellEditButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "contentEdit.png",
                "CloudSyncProfile.aspx?action=edit&id=" + profile.Id.ToString() + "&LangType=" + _parameters.Language.ToString(),
                _siteApi.EkMsgRef.GetMessage("alt edit sync button"),
                _siteApi.EkMsgRef.GetMessage("btn edit sync"),
                string.Empty);

            rowToolbarButtons.Cells.Add(cellEditButton);
        }

        HtmlTableCell cellSyncButton = new HtmlTableCell();
        cellSyncButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "sync.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            "rel=\"" + profile.Id.ToString() + "\" onclick=\"Ektron.Workarea.Sync.Relationships.Synchronize(this);\"");

        rowToolbarButtons.Cells.Add(cellSyncButton);

        if (profile.LastFullSync > DateTime.MinValue)
        {
            // Status Button
            HtmlTableCell cellStatusButton = new HtmlTableCell();
            cellStatusButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "FileTypes/text.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("btn last status"),
                _siteApi.EkMsgRef.GetMessage("btn last status"),
                "rel=\"" + profile.Id.ToString() + "\" onclick=\"Ektron.Workarea.Sync.Relationships.ShowSyncStatus(this);\"");

            rowToolbarButtons.Cells.Add(cellStatusButton);
        }

        if (profile.SynchronizeDatabase)
        {
            // Preview
            HtmlTableCell cellPreviewButton = new HtmlTableCell();
            cellPreviewButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "preview.png",
                "SyncPreview.aspx?id=" + profile.Id.ToString() + "&referrer=" + HttpUtility.UrlEncode(Request.RawUrl),
                _siteApi.EkMsgRef.GetMessage("btn preview status"),
                _siteApi.EkMsgRef.GetMessage("btn preview status"),
                string.Empty);

            rowToolbarButtons.Cells.Add(cellPreviewButton);
        }

        if (profile.SynchronizeTemplates)
        {
            HtmlTableCell cellFilesButton = new HtmlTableCell();
            cellFilesButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/syncFiles.png",
                "SyncFiles.aspx?id=" + profile.Id.ToString() + "&referrer=" + HttpUtility.UrlEncode(Request.RawUrl),
                _siteApi.EkMsgRef.GetMessage("alt select sync files"),
                _siteApi.EkMsgRef.GetMessage("btn select sync files"),
                string.Empty);

            rowToolbarButtons.Cells.Add(cellFilesButton);
        }

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellDeleteButton = new HtmlTableCell();
            cellDeleteButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/delete.png",
                "CloudSyncProfile.aspx?action=delete&id=" + profile.Id.ToString(),
                _siteApi.EkMsgRef.GetMessage("alt delete sync button"),
                _siteApi.EkMsgRef.GetMessage("btn delete sync"),
                "onclick=\"return Ektron.Workarea.Sync.Profile.ConfirmDelete();\"");

            rowToolbarButtons.Cells.Add(cellDeleteButton);
        }

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "back.png",
            "Sync.aspx",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty);

        rowToolbarButtons.Cells.Add(cellBackButton);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("viewsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    private void RenderHeaderForAddMode(Profile profile)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("add schedule titlebar"));

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellSaveButton = new HtmlTableCell();
            cellSaveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/save.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                "onclick=\"Ektron.Workarea.Sync.Profile.Save();\"");

            rowToolbarButtons.Cells.Add(cellSaveButton);
        }

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/back.png",
            "Sync.aspx",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty);

        rowToolbarButtons.Cells.Add(cellBackButton);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("viewsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    private void RenderHeaderForEditMode(Profile profile)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("edit schedule titlebar") + " \"" + profile.Name + "\"");

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellSaveButton = new HtmlTableCell();
            cellSaveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/save.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                "onclick=\"Ektron.Workarea.Sync.Profile.Save();\"");

            rowToolbarButtons.Cells.Add(cellSaveButton);
        }

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/back.png",
            "CloudSyncProfile.aspx?action=view&id=" + profile.Id.ToString(),
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty);

        rowToolbarButtons.Cells.Add(cellBackButton);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("viewsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ClearErrors()
    {
        divErrorMessage.InnerText = string.Empty;
        divErrorMessage.Visible = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Filter GetFilters()
    {
        Filter filter = new Filter();

        switch (rdoFilters.SelectedValue)
        {
            case "None":
                filter.Type = FilterType.None;
                break;
            case "Include":
                filter.Type = FilterType.Include;
                break;
            case "Exclude":
                filter.Type = FilterType.Exclude;
                break;
        }

        if (!string.IsNullOrEmpty(txtFileFilters.Text))
        {
            string[] splitFileFilters = txtFileFilters.Text.Split(
                new string[] { FilterDelimiter },
                StringSplitOptions.RemoveEmptyEntries);

            filter.Files = new List<string>();
            foreach (string fileFilter in splitFileFilters)
            {
                if (fileFilter.StartsWith("."))
                {
                    filter.Files.Add("*" + fileFilter.Trim());
                }
                else
                {
                    filter.Files.Add(fileFilter.Trim());
                }
            }
        }

        if (!string.IsNullOrEmpty(txtDirectoryFilters.Text))
        {
            string[] splitDirectoryFilters = txtDirectoryFilters.Text.Split(
                new string[] { FilterDelimiter },
                StringSplitOptions.RemoveEmptyEntries);

            filter.Directories = new List<string>();
            foreach (string directoryFilter in splitDirectoryFilters)
            {
                filter.Directories.Add(directoryFilter.Trim());
            }
        }

        return filter;
    }

    private List<SyncDBScope> getScope()
    {
        List<SyncDBScope> scopeList = new List<SyncDBScope>();
        foreach (ListItem cb in cbScope.Items)
        {
            if (cb.Selected)
            {
                scopeList.Add((SyncDBScope)Enum.Parse(typeof(SyncDBScope), cb.Value));
            }
        }
        return scopeList;
    }

    /// <summary>
    /// 
    /// </summary>
    private void PopulateLabels()
    {
        lblConflictResolution.Text = "Conflict Resolution Policy";
        lblConflictResolution.ToolTip = lblConflictResolution.Text;
        lblFilters.Text = "Include/Exclude Files";
        lblFilters.ToolTip = lblFilters.Text;
        lblItemsToSynchronize.Text = "Items to Synchronize";
        lblItemsToSynchronize.ToolTip = lblItemsToSynchronize.Text;
        lblLocalSite.Text = "Local Site";
        lblLocalSite.ToolTip = lblLocalSite.Text;
        lblMultiSiteFolder.Text = "Multisite Folder";
        lblMultiSiteFolder.ToolTip = lblMultiSiteFolder.Text;
        lblRemoteSite.Text = "Remote Site";
        lblRemoteSite.ToolTip = lblRemoteSite.Text;
        lblRestoration.Text = "File Restoration";
        lblRestoration.ToolTip = lblRestoration.Text;
        lblSchedule.Text = "Schedule";
        lblSchedule.ToolTip = lblSchedule.Text;
        lblScheduleName.Text = "Name";
        lblScheduleName.ToolTip = lblScheduleName.Text;
        lblSyncDirection.Text = "Synchronization Direction";
        lblSyncDirection.ToolTip = lblSyncDirection.Text;
        lblDestinationWinsDesc.Text = "The destination item is always chosen as the resolution winner. When a conflict occurs, no change will be made to the destination item.";
        lblDestinationWinsDesc.ToolTip = lblDestinationWinsDesc.Text;
        lblSourceWinsDesc.Text = "The source change is always chosen as the resolution winner. When a conflict occurs, the source change is applied to the destination, overwriting the destination item.";
        lblSourceWinsDesc.ToolTip = lblSourceWinsDesc.Text;

        chkDatabase.Text = "Database/Resources";
        chkDatabase.ToolTip = "Syncronize Database/Resources";
        chkWorkarea.Text = "Workarea";
        chkWorkarea.ToolTip = "Syncronize Workarea";
        chkTemplates.Text = "Template";
        chkTemplates.ToolTip = "Syncronize Template";
        chkBinaries.Text = "bin (precompiled sites)";
        chkBinaries.ToolTip = "Synchronize bin (precompiled sites)";
        chkPackage.Text = "Package";
        chkPackage.ToolTip = "Synchronize Package";

        lblFileFilterHeader.Text = "Files:";
        lblFileFilterHeader.ToolTip = lblFileFilterHeader.Text;
        divFileFilterDesc.InnerHtml = "Enter a comma separated list of file extensions<br/>(example: *.doc, *.gif)";
        lblDirectoryFilterHeader.Text = "Directories:";
        lblDirectoryFilterHeader.ToolTip = lblDirectoryFilterHeader.Text;
        divDirectoryFilterDesc.InnerHtml = "Enter a comma separated list of directories<br/>(example: videos, images)";

        foreach (ListItem item in rdoFilters.Items)
        {
            if (item.Value == FilterType.None.ToString())
            {
                item.Text = "None";
            }
            else if (item.Value == FilterType.Include.ToString())
            {
                item.Text = "Include";
            }
            else if (item.Value == FilterType.Exclude.ToString())
            {
                item.Text = "Exclude";
            }
        }

        foreach (ListItem item in rdoDirection.Items)
        {
            if (item.Value == SyncDirection.Bidirectional.ToString())
            {
                item.Text = "Bidirectional";
            }
            else if (item.Value == SyncDirection.Upload.ToString())
            {
                item.Text = "Upload (Local to Remote)";
            }
            else if (item.Value == SyncDirection.Download.ToString())
            {
                item.Text = "Download (Remote to Local)";
            }
        }

        foreach (ListItem item in rdoConflictResolution.Items)
        {
            if (item.Value == ConflictResolutionPolicy.DestinationWins.ToString())
            {
                item.Text = "Version on Remote Site Wins";
            }
            else if (item.Value == ConflictResolutionPolicy.SourceWins.ToString())
            {
                item.Text = "Version on Local Site Wins";
            }
        }

        chkRestoration.Text = "Allow files removed during synchronization to be restored.";
        chkRestoration.ToolTip = chkRestoration.Text;

        lblPackageDialogHeader.Text = "Synchronization Package";
        lblPackageDescription.Text = "Please choose the content to be included in the synchronization package:";
        lblPackageDescription.ToolTip = lblPackageDescription.Text;
        btnCloseSyncPackage.Text = "Close";
        btnCloseSyncPackage.ToolTip = btnCloseSyncPackage.Text;

        //cbScope.DataSource = Enum.GetNames(typeof(SyncDBScope));
        cbScope.Items.Add(new ListItem() { Text = "CMS Core", Value = "ektron" });
        cbScope.Items.Add(new ListItem() { Text = "History", Value = "history" });
        cbScope.Items.Add(new ListItem() { Text = "Workflow (Ecommerce)", Value = "workflow" });
        cbScope.Items.Add(new ListItem() { Text = "Search", Value = "search" });
        cbScope.Items.Add(new ListItem() { Text = "ASP.NET", Value = "aspnet" });
        cbScope.Items.Add(new ListItem() { Text = "Custom Tables", Value = "custom" });

    }

    /// <summary>
    /// 
    /// </summary>
    private void PopulateTree()
    {
        // Render file tree.
        tvPackageContent.Nodes.Clear();

        tvPackageContent.NodeIndent = 25;
        tvPackageContent.ShowCheckBoxes = TreeNodeTypes.All;
        tvPackageContent.ShowExpandCollapse = true;

        TreeNode rootNode = new TreeNode();
        rootNode.Text = PackageContent.Name;
        rootNode.Expanded = true;
        rootNode.SelectAction = TreeNodeSelectAction.None;

        if (PackageContent.IsMultiSiteFolder)
        {
            rootNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + SiteFolderIcon;
        }
        else
        {
            rootNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
        }

        tvPackageContent.Nodes.Add(rootNode);

        PopulateTree(PackageContent.Nodes, rootNode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="parent"></param>
    private void PopulateTree(List<ContentNode> nodes, TreeNode parent)
    {
        if (nodes != null)
        {
            foreach (ContentNode node in nodes)
            {
                if (node != null)
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode.Text = node.Name;
                    treeNode.Expanded = node.SelectedNodes.Count > 0;
                    treeNode.Value = node.Id.ToString();
                    treeNode.SelectAction = TreeNodeSelectAction.None;
                    treeNode.Checked = node.IsSelected;

                    if (node.IsFolder)
                    {
                        treeNode.Checked =
                            node.Nodes.Count > 0 &&
                            node.Nodes.Count == node.SelectedNodes.Count;

                        if (node.IsMultiSiteFolder)
                        {
                            treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + SiteFolderIcon;
                        }
                        else
                        {
                            treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
                        }

                        PopulateTree(node.Nodes, treeNode);
                    }
                    else
                    {
                        treeNode.Checked = node.IsSelected;
                        treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FileIcon;
                    }

                    parent.ChildNodes.Add(treeNode);
                }
            }
        }
    }

    /// <summary>
    /// Dynamically stripes the visible rows of the 
    /// profile table.
    /// </summary>
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

    /// <summary>
    /// Registers javascript and CSS resources.
    /// </summary>
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
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronClueTipJS, false);

        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Profile.js", "SyncProfileJS", false);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "SyncRelationshipJS", false);

        JS.RegisterJS(this, "../java/jfunct.js", "EktronJFunctJS", false);
        JS.RegisterJS(this, "../java/internCalendarDisplayFuncs.js", "EktronIntercalendarDisplayFuncs", false);
        JS.RegisterJS(this, "../java/toolbar_roll.js", "EktronToolbarRollJS", false);

        // CSS Resources

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronThickBoxCss, false);
        ektronClientScript.Text = _styleHelper.GetClientScript();
    }
}
