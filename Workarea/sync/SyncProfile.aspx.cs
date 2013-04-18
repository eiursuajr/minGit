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

public partial class SyncProfile : Page, ISyncProfileView
{
    private const string FilterDelimiter = ",";
    private const string RelativeImagePath = "images/ui/icons/";
    private const string FileIcon = "content.png";
    private const string FolderIcon = "folder.png";
    private const string SiteFolderIcon = "folderSite.png";

    private readonly SyncProfilePresenter _presenter;
    private readonly SiteAPI _siteApi;
    protected ContentAPI apiContent = new ContentAPI();
    private readonly StyleHelper _styleHelper;

    private ProfileParameters _parameters;

    /// <summary>
    /// Constructor
    /// </summary>
    public SyncProfile()
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

            ProfilePresentationServices _presentationServices = new ProfilePresentationServices(Data);

            if (_presentationServices.AllowDatabaseSync)
            {
                chkDatabase.Visible = true;
                cbScope.Visible = true;
            }
            else
            {
                chkDatabase.Checked = false;
                chkDatabase.Visible = false;
                cbScope.Visible = false;
            }

            if (!IsPostBack)
            {
                txtLocalSite.Enabled = false;
                txtRemoteSite.Enabled = false;
                txtMultiSiteFolder.Enabled = false;

                txtScheduleName.Text = Data.Name;
                txtLocalSite.Text = Data.Parent.LocalSite.Connection.DatabaseName;
                txtRemoteSite.Text = Data.Parent.RemoteSite.Connection.DatabaseName;

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

                        
                    }
                }

                chkWorkarea.Checked = Data.SynchronizeWorkarea;
                chkTemplates.Checked = Data.SynchronizeTemplates;
                chkBinaries.Checked = Data.SynchronizeBinaries;

                if (Data.ConflictResolution.ToString() != "ApplicationDefined")
                {
                    rdoConflictResolution.SelectedValue = Data.ConflictResolution.ToString();
                }
                
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

                StripeRows();
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
                            tableListSB.Append(_siteApi.EkMsgRef.GetMessage("lbl table empty"));
                        }
                        ListItem lbItem = cbScope.Items.FindByValue(scopeConfig.Name);
                        //lbItem.Attributes["title"] = tableListSB.ToString();
                        lbItem.Attributes["title"] = String.Format(_siteApi.EkMsgRef.GetMessage("lbl Tables currently") + " {0} | {1}", lbItem.Text, tableListSB.ToString());
                    }
                    catch { }

                    if (scopeConfig.Name == "custom")
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

        if (Data.Schedule.GetType().Equals(typeof(Ektron.Cms.Sync.Client.WeeklySchedule)))
        {
            Data.Schedule.Frequency = ((int) ssScheduleOptions.StartTime.DayOfWeek).ToString();
        }
        else if (Data.Schedule.GetType().Equals(typeof(Ektron.Cms.Sync.Client.MonthlySchedule)))
        {
            Data.Schedule.Frequency = ssScheduleOptions.StartTime.Day.ToString();
        }

        Data.Scope = getScope();

        bool success = true;

        success = isSubDatabaseValid();

        try
        {
            if (success)
                _presenter.Save(Data);
        }
        catch (InvalidProfileNameException)
        {
            DisplayError(_siteApi.EkMsgRef.GetMessage("lbl valid profile name"));
            success = false;
        }
        catch (InvalidSyncItemsException)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.Append(_siteApi.EkMsgRef.GetMessage("lbl Items Synchronize"));
            errorMessage.Append("<ul>");
            errorMessage.Append("<li>");
            errorMessage.Append(_siteApi.EkMsgRef.GetMessage("lbl synchronization item"));
            errorMessage.Append("</li>");
            errorMessage.Append("<li>");
            errorMessage.Append(_siteApi.EkMsgRef.GetMessage("lbl Only Database and Templates"));
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
                errorMessage.Append(_siteApi.EkMsgRef.GetMessage("lbl sync file illegal character"));
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
                errorMessage.Append(_siteApi.EkMsgRef.GetMessage("lbl sync directory illegal character"));
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
                Response.Redirect("SyncProfile.aspx?action=view&id=" + Data.Id.ToString(), true);
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
                DisplayError(_siteApi.EkMsgRef.GetMessage("lbl sync select Database"));
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

		HtmlTableCell cellBackButton = new HtmlTableCell();
		cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
			_siteApi.AppPath + "images/ui/icons/" + "back.png",
			"Sync.aspx",
			_siteApi.EkMsgRef.GetMessage("alt back button text"),
			_siteApi.EkMsgRef.GetMessage("btn back"),
			string.Empty,
			StyleHelper.BackButtonCssClass,
			true);

		rowToolbarButtons.Cells.Add(cellBackButton);

		bool primaryCssApplied = false;

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellEditButton = new HtmlTableCell();
            cellEditButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "contentEdit.png",
                "SyncProfile.aspx?action=edit&id=" + profile.Id.ToString() + "&LangType=" + _parameters.Language.ToString(),
                _siteApi.EkMsgRef.GetMessage("alt edit sync button"),
                _siteApi.EkMsgRef.GetMessage("btn edit sync"),
                string.Empty,
				StyleHelper.EditButtonCssClass,
				!primaryCssApplied);

            rowToolbarButtons.Cells.Add(cellEditButton);

			primaryCssApplied = true;
        }

        HtmlTableCell cellSyncButton = new HtmlTableCell();
        cellSyncButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "sync.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            "rel=\"" + profile.Id.ToString() + "\" onclick=\"Ektron.Workarea.Sync.Relationships.Synchronize(this);\"",
			StyleHelper.SyncButtonCssClass,
			!primaryCssApplied);

        rowToolbarButtons.Cells.Add(cellSyncButton);

		primaryCssApplied = true;

        if (profile.LastFullSync > DateTime.MinValue)
        {
            // Status Button
            HtmlTableCell cellStatusButton = new HtmlTableCell();
            cellStatusButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "FileTypes/text.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("btn last status"),
                _siteApi.EkMsgRef.GetMessage("btn last status"),
                "rel=\"" + profile.Id.ToString() + "\" onclick=\"Ektron.Workarea.Sync.Relationships.ShowSyncStatus(this);\"",
				StyleHelper.LastStatusButtonCssClass);

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
                string.Empty,
                StyleHelper.PreviewStatusButtonCssNewClass);

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
                string.Empty,
				StyleHelper.SyncButtonCssClass);

            rowToolbarButtons.Cells.Add(cellFilesButton);
        }

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellDeleteButton = new HtmlTableCell();
            cellDeleteButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/delete.png",
                "SyncProfile.aspx?action=delete&id=" + profile.Id.ToString(),
                _siteApi.EkMsgRef.GetMessage("alt delete sync button"),
                _siteApi.EkMsgRef.GetMessage("btn delete sync"),
                "onclick=\"return Ektron.Workarea.Sync.Profile.ConfirmDelete();\"",
				StyleHelper.DeleteButtonCssClass);

            rowToolbarButtons.Cells.Add(cellDeleteButton);
        }

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("viewsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    private void RenderHeaderForAddMode(Profile profile)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("add schedule titlebar"));

		HtmlTableCell cellBackButton = new HtmlTableCell();
		cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
			_siteApi.AppPath + "images/ui/icons/back.png",
			"Sync.aspx",
			_siteApi.EkMsgRef.GetMessage("alt back button text"),
			_siteApi.EkMsgRef.GetMessage("btn back"),
			string.Empty,
			StyleHelper.BackButtonCssClass,
			true);

		rowToolbarButtons.Cells.Add(cellBackButton);

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellSaveButton = new HtmlTableCell();
            cellSaveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/save.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
				"onclick=\"Ektron.Workarea.Sync.Profile.Save();\"",
				StyleHelper.SaveButtonCssClass,
				true);

            rowToolbarButtons.Cells.Add(cellSaveButton);
        }

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("viewsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    private void RenderHeaderForEditMode(Profile profile)
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("edit schedule titlebar") + " \"" + profile.Name + "\"");

		HtmlTableCell cellBackButton = new HtmlTableCell();
		cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
			_siteApi.AppPath + "images/ui/icons/back.png",
			"SyncProfile.aspx?action=view&id=" + profile.Id.ToString(),
			_siteApi.EkMsgRef.GetMessage("alt back button text"),
			_siteApi.EkMsgRef.GetMessage("btn back"),
			string.Empty,
			StyleHelper.BackButtonCssClass,
			true);

		rowToolbarButtons.Cells.Add(cellBackButton);

        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellSaveButton = new HtmlTableCell();
            cellSaveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/save.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                _siteApi.EkMsgRef.GetMessage("alt save sync button text"),
                "onclick=\"Ektron.Workarea.Sync.Profile.Save();\"",
				StyleHelper.SaveButtonCssClass,
				true);

            rowToolbarButtons.Cells.Add(cellSaveButton);
        }

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

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
        lblConflictResolution.Text =  lblConflictResolution.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl Conflict Resolution Policy");        
        lblFilters.Text = lblFilters.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync include exclude files");        
        lblItemsToSynchronize.Text = lblItemsToSynchronize.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync items");        
        lblLocalSite.Text = lblLocalSite.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync local site");         
        lblMultiSiteFolder.Text =lblMultiSiteFolder.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl Multisite Folder");
        lblRemoteSite.Text =  lblRemoteSite.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync remote site");        
        lblRestoration.Text =lblRestoration.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl allow restore");        
        lblSchedule.Text = lblSchedule.ToolTip =_siteApi.EkMsgRef.GetMessage("schedule text");        
        lblScheduleName.Text = lblScheduleName.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl name");        
        lblSyncDirection.Text = lblSyncDirection.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync options");        
        lblDestinationWinsDesc.Text =lblDestinationWinsDesc.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync destination");         
        lblSourceWinsDesc.Text =lblSourceWinsDesc.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync source");         

        chkDatabase.Text =_siteApi.EkMsgRef.GetMessage("lbl sync database or resources");
        chkDatabase.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl Syncronize DatabaseResources");
        chkWorkarea.Text =_siteApi.EkMsgRef.GetMessage("workarea options label");
        chkWorkarea.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl SyncronizeWorkarea");
        chkTemplates.Text = _siteApi.EkMsgRef.GetMessage("template label");
        chkTemplates.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl Syncronize Template");
        chkBinaries.Text = _siteApi.EkMsgRef.GetMessage("lbl bin sync");
        chkBinaries.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl Synchronize bin");

        lblFileFilterHeader.Text = lblFileFilterHeader.ToolTip =  _siteApi.EkMsgRef.GetMessage("files label");
       
        divFileFilterDesc.InnerHtml = _siteApi.EkMsgRef.GetMessage("lbl sync file list");
        lblDirectoryFilterHeader.Text =lblDirectoryFilterHeader.ToolTip =_siteApi.EkMsgRef.GetMessage("sync directory filter label");
         
        divDirectoryFilterDesc.InnerHtml = _siteApi.EkMsgRef.GetMessage("lbl sync directory list");

        foreach (ListItem item in rdoFilters.Items)
        {
            if (item.Value == FilterType.None.ToString())
            {
                item.Text =_siteApi.EkMsgRef.GetMessage("lbl sync none");
            }
            else if (item.Value == FilterType.Include.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("generic Include");
            }
            else if (item.Value == FilterType.Exclude.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync exclude files");
            }
        }

        foreach (ListItem item in rdoDirection.Items)
        {
            if (item.Value == SyncDirection.Bidirectional.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync bidirectional");
            }
            else if (item.Value == SyncDirection.Upload.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync upload");
            }
            else if (item.Value == SyncDirection.Download.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync download");
            }
        }

        foreach (ListItem item in rdoConflictResolution.Items)
        {
            if (item.Value == ConflictResolutionPolicy.DestinationWins.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync destination short");
            }
            else if (item.Value == ConflictResolutionPolicy.SourceWins.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync source short");
            }
        }

        chkRestoration.Text = _siteApi.EkMsgRef.GetMessage("sync allow restore checkbox");
        chkRestoration.ToolTip = chkRestoration.Text;
        
        cbScope.Visible = false;
        //cbScope.DataSource = Enum.GetNames(typeof(SyncDBScope));
        cbScope.Items.Add(new ListItem() { Text = _siteApi.EkMsgRef.GetMessage("generic CMS Core"), Value = "ektron" });
        cbScope.Items.Add(new ListItem() { Text = _siteApi.EkMsgRef.GetMessage("generic Workflow Ecommerce"), Value = "workflow" });
        cbScope.Items.Add(new ListItem() { Text = _siteApi.EkMsgRef.GetMessage("generic Custom Tables"), Value = "custom" });
        cbScope.Items.Add(new ListItem() { Text =_siteApi.EkMsgRef.GetMessage("lbl generic history"), Value = "history" });
        cbScope.Items.Add(new ListItem() { Text =_siteApi.EkMsgRef.GetMessage("generic aspnet"), Value = "aspnet" });
        cbScope.Items.Add(new ListItem() { Text =_siteApi.EkMsgRef.GetMessage("generic search"), Value = "search" });
        cbScope.Items.Add(new ListItem() { Text =_siteApi.EkMsgRef.GetMessage("generic notification"), Value = "notification" });
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
