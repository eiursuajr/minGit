using System;
using System.Collections.Generic;

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web;
using Ektron.Cms.Sync.Web.Parameters;
using SyncDirection = Ektron.Cms.Sync.Client.SyncDirection;
using Ektron.Cms.Common;
using System.Web;
using System.ServiceModel;
using System.Configuration;

public partial class Sync : System.Web.UI.Page, ISyncRelationshipsListView
{
    private const string ProfileUrlFormat = "SyncProfile.aspx?action={0}&id={1}";
    private const string CloudProfileUrlFormat = "CloudSyncProfile.aspx?action={0}&id={1}";
    private const string ProfileViewAction = "view";
    private const string AttributeRel = "rel";
    private const string AttributeClass = "class";
    private const string AttributeOnClick = "onclick";

    private readonly SyncRelationshipsPresenter _presenter;
    private readonly CommonApi _commonApi;
    private readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;

    private RelationshipsParameters _parameters;

    /// <summary>
    /// Constructor
    /// </summary>
    public Sync()
    {
        _presenter = new SyncRelationshipsPresenter(this);
        _commonApi = new CommonApi();
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!_siteApi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0))
        {
            Response.Redirect("../reterror.aspx?info=" + HttpUtility.UrlEncode(_siteApi.EkMsgRef.GetMessage("sync logged out message")));
        }

        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        _parameters = new RelationshipsParameters(Request);

        RegisterResources();
    }

    /// <summary>
    /// Handles the Page's 'Load' event, initializing the view
    /// for display.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        _presenter.InitializeView();
    }

    #region ISyncRelationshipsListView Members

    /// <summary>
    /// Gets or sets the collection of relationships to be displayed
    /// on this page.
    /// </summary>
    public List<Relationship> Relationships { get; set; }

    /// <summary>
    /// Binds the specified relationship data for display.
    /// </summary>
    public void Bind()
    {
        RenderHeader();

        if (Relationships != null)
        {
            List<Relationship> activeRelationships = new List<Relationship>();
            foreach (Relationship relationship in Relationships)
            {
                if (relationship.Status == ProfileStatus.Active &&
                    relationship.LocalSite.Address.ToLower() == System.Net.Dns.GetHostName().ToLower())
                {
                    activeRelationships.Add(relationship);
                }
            }

            if (activeRelationships.Count > 0)
            {
                rptRelationshipList.DataSource = activeRelationships;
                rptRelationshipList.DataBind();
            }
            else
            {
                litNoRelationshipsMessage.Text = _siteApi.EkMsgRef.GetMessage("lbl sync no configurations");
            }
        }
    }

    #endregion

    #region ISyncView Members

    /// <summary>
    /// Displays the specified error message to the user.
    /// </summary>
    /// <param name="message"></param>
    public void DisplayError(string message)
    {
        Response.Write("**** " + message + " ****");
    }

    #endregion

    private void RenderHeader()
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("view all sync titlebar"));

        bool primaryCssApplied = false;
        bool utilityDividerAdded = false;

        if (_presenter.IsValidVersion)
        {
            if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
            {
                // Add button
                HtmlTableCell cellAddButton = new HtmlTableCell();
                cellAddButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                    _siteApi.AppPath + "images/ui/Icons/add.png",
                    "#" + _siteApi.EkMsgRef.GetMessage("create server relationship").Replace(" ", ""),
                    _siteApi.EkMsgRef.GetMessage("create server relationship"),
                    _siteApi.EkMsgRef.GetMessage("create server relationship"),
                    String.Empty,
                    StyleHelper.MergeCssClasses(new string[]{"launchCreateModalButton", StyleHelper.AddButtonCssClass}),
                    !primaryCssApplied);

                primaryCssApplied = true;

                rowToolbarButtons.Cells.Add(cellAddButton);

                rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);
                utilityDividerAdded = true;

                if (ConfigurationManager.AppSettings["cloudsync"] != null)
                {
                    if (ConfigurationManager.AppSettings["cloudsync"].ToString() == "true")
                    {
                        // Cloud Sync
                        HtmlTableCell cellAddCloudButton = new HtmlTableCell();
                        cellAddCloudButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                            _siteApi.AppPath + "images/ui/Icons/add.png",
                            "CloudSync.aspx?referrer=Sync.aspx",
                            _siteApi.EkMsgRef.GetMessage("synccreatecloudrelationship"),
                            _siteApi.EkMsgRef.GetMessage("synccreatecloudrelationship"),
                            string.Empty,
                            StyleHelper.AddButtonCssClass);

                        rowToolbarButtons.Cells.Add(cellAddCloudButton);
                    }
                }
            }

            // Log Button
            HtmlTableCell cellLogButton = new HtmlTableCell();
            cellLogButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/chartBar.png",
                "SyncHistory.aspx?referrer=Sync.aspx",
                _siteApi.EkMsgRef.GetMessage("sync log history"),
                _siteApi.EkMsgRef.GetMessage("sync log history"),
                string.Empty,
                StyleHelper.LogHistoryButtonCssClass,
                !primaryCssApplied);

            primaryCssApplied = true;

            rowToolbarButtons.Cells.Add(cellLogButton);
        }

        if (_presenter.EnableSyncConflictResolution)
        {
            if (!utilityDividerAdded)
            {
                rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);
                utilityDividerAdded = true;
            }

            // Sync Conflict Resolution
            HtmlTableCell cellConflictButton = new HtmlTableCell();
            cellConflictButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppImgPath + "id_card_force.png",
                "#ResolveSynchronizationCollisions",
                "Resolve Synchronization Collisions",
                "Resolve Synchronization Collisions",
                String.Empty,
                StyleHelper.MergeCssClasses(new string[] { "launchResolveSyncCollisionsButton", StyleHelper.ResolveConflictButtonCssClass }),
                !primaryCssApplied);

            primaryCssApplied = true;

            rowToolbarButtons.Cells.Add(cellConflictButton);
        }

        if (_presenter.EnableSyncConflictReview)
        {
            if (!utilityDividerAdded)
            {
                rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);
                utilityDividerAdded = true;
            }

            // Sync Review
            HtmlTableCell cellReviewButton = new HtmlTableCell();
            cellReviewButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppImgPath + "id_card_warning_22x22.gif",
                "SyncReview.aspx",
                _siteApi.EkMsgRef.GetMessage("lbl sync review resolved conflicts"),
                _siteApi.EkMsgRef.GetMessage("lbl sync review resolved conflicts"),
                String.Empty,
                StyleHelper.MergeCssClasses(new string[] { "reviewConflictsButton", StyleHelper.ReviewConflictButtonCssClass }),
                !primaryCssApplied);

            primaryCssApplied = true;

            rowToolbarButtons.Cells.Add(cellReviewButton);
        }

        if (_presenter.isCustomConfigAvailable())
        {
            // Sync Custom Config
            HtmlTableCell cellCustomConfigButton = new HtmlTableCell();
            cellCustomConfigButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/Icons/wrenchOrange.png",
                "SyncCustomConfig.aspx",
                _siteApi.EkMsgRef.GetMessage("synclblmanagecustomconfig"),
                _siteApi.EkMsgRef.GetMessage("synclblmanagecustomconfig"),
                string.Empty);

            rowToolbarButtons.Cells.Add(cellCustomConfigButton);
        }

        rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        // Help Button
        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("ViewAllSync", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// Handle the ItemDataBound event for the repeater control responsible for rendering
    /// relationship data on the page.
    /// </summary>
    /// <remarks>
    /// When a relationship is bound, another child repeater will be populated with the 
    /// relationship's profiles.
    /// </remarks>
    /// <param name="source">Repeater</param>
    /// <param name="e">Repeater arguments</param>
    protected void HandleRelationshipListItemDataBound(object source, RepeaterItemEventArgs e)
    {
        try
        {
            Relationship relationship = e.Item.DataItem as Relationship;
            if (relationship != null && relationship.Status != ProfileStatus.Deleted)
            {
                RelationshipPresentationServices presentationServices =
                    new RelationshipPresentationServices(relationship);

                // Relationship Labels
                PopulateTextControl(e.Item.FindControl("lblServerName"), _siteApi.EkMsgRef.GetMessage("js server name") + ":");
                PopulateTextControl(e.Item.FindControl("lblIntegratedSecurity"), _siteApi.EkMsgRef.GetMessage("js integrated security") + ":");
                PopulateTextControl(e.Item.FindControl("lblLocalSite"),  _siteApi.EkMsgRef.GetMessage("lbl Local Site Path"));
                PopulateTextControl(e.Item.FindControl("lblRemoteSite"), _siteApi.EkMsgRef.GetMessage("lbl Remote Site Path"));

                // Relationship Details
                PopulateTextControl(e.Item.FindControl("litDatabaseName"), relationship.RemoteSite.Connection.DatabaseName);
                PopulateTextControl(e.Item.FindControl("litServerName"), relationship.RemoteSite.Connection.ServerName);
                PopulateTextControl(e.Item.FindControl("litLocalSite"), relationship.LocalSite.SitePath);
                PopulateTextControl(e.Item.FindControl("litRemoteSite"), relationship.RemoteSite.SitePath);

                // Profile List Headers
                PopulateTextControl(e.Item.FindControl("litProfileHeader"), _siteApi.EkMsgRef.GetMessage("lbl profile"));
                PopulateTextControl(e.Item.FindControl("litProfileIdHeader"), _siteApi.EkMsgRef.GetMessage("lbl ProfileId"));
                PopulateTextControl(e.Item.FindControl("litScheduleHeader"), _siteApi.EkMsgRef.GetMessage("schedule text"));
                PopulateTextControl(e.Item.FindControl("litLastRunTimeHeader"), _siteApi.EkMsgRef.GetMessage("lbl sync lastruntime"));
                PopulateTextControl(e.Item.FindControl("litButtonsHeader"), _siteApi.EkMsgRef.GetMessage("generic actions"));
                PopulateTextControl(e.Item.FindControl("litLastRunResultHeader"), _siteApi.EkMsgRef.GetMessage("lbl sync lastrunresult"));
                PopulateTextControl(e.Item.FindControl("litCurrentStatusHeader"), _siteApi.EkMsgRef.GetMessage("lbl Current Status"));

                // Relationship Buttons
                HtmlGenericControl divRelationshipButtons =
                    e.Item.FindControl("divRelationshipButtons") as HtmlGenericControl;

                if (divRelationshipButtons != null)
                {
                    if (presentationServices.AllowInitialSync &&
                        _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                    {
                        HtmlAnchor linkSyncButton = new HtmlAnchor();
                        linkSyncButton.HRef = "#";
                        linkSyncButton.Title =  _siteApi.EkMsgRef.GetMessage("generic Sync");
                        linkSyncButton.Attributes.Add(AttributeRel, relationship.Id.ToString());
                        linkSyncButton.Attributes.Add(AttributeClass, "syncButton");
                        linkSyncButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.InitialSynchronize(this);");

                        divRelationshipButtons.Controls.Add(linkSyncButton);
                    }

                    if (_presenter.EnableForceSync && presentationServices.AllowForceInitialSync &&
                        _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                    {
                        HtmlAnchor linkSyncButton = new HtmlAnchor();
                        linkSyncButton.HRef = "#";
                        linkSyncButton.Title = _siteApi.EkMsgRef.GetMessage("generic Sync");
                        linkSyncButton.Attributes.Add(AttributeRel, relationship.Id.ToString());
                        linkSyncButton.Attributes.Add(AttributeClass, "forceSyncButton");
                        linkSyncButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.Synchronize(this);");

                        divRelationshipButtons.Controls.Add(linkSyncButton);
                    }

                    if (presentationServices.AllowAddProfile &&
                        _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                    {
                        HtmlAnchor linkAddButton = new HtmlAnchor();
                        linkAddButton.HRef = "SyncProfile.aspx?action=add&id=" + relationship.Id.ToString();
                        linkAddButton.Title =  _siteApi.EkMsgRef.GetMessage("lbl Add Profile");
                        linkAddButton.Attributes.Add(AttributeClass, "addButton");

                        divRelationshipButtons.Controls.Add(linkAddButton);
                    }

                    if (presentationServices.AllowGetStatus)
                    {
                        HtmlAnchor linkStatusButton = new HtmlAnchor();
                        linkStatusButton.HRef = "#";
                        linkStatusButton.Title =  _siteApi.EkMsgRef.GetMessage("lbl Get Status");
                        linkStatusButton.Attributes.Add(AttributeRel, relationship.Id.ToString());
                        linkStatusButton.Attributes.Add(AttributeClass, "statusButton");
                        linkStatusButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.ShowSyncStatus(this);");

                        divRelationshipButtons.Controls.Add(linkStatusButton);
                    }

                    // File restoration is disabled until backend support is implemented.

                    //if (presentationServices.AllowRestore &&
                    //    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                    //{
                    //    HtmlAnchor linkRestoreButton = new HtmlAnchor();
                    //    linkRestoreButton.HRef = "Restore.aspx?id=" + relationship.Id.ToString();
                    //    linkRestoreButton.Title = "Restore Files";
                    //    linkRestoreButton.Attributes.Add(AttributeClass, "restoreButton");

                    //    divRelationshipButtons.Controls.Add(linkRestoreButton);
                    //}

                    if (presentationServices.AllowDelete &&
                        _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                    {
                        HtmlAnchor linkDeleteButton = new HtmlAnchor();
                        linkDeleteButton.HRef = "#";
                        linkDeleteButton.Title = _siteApi.EkMsgRef.GetMessage("generic delete title");
                        linkDeleteButton.Attributes.Add(AttributeRel, relationship.Id.ToString());
                        linkDeleteButton.Attributes.Add(AttributeClass, "deleteButton");
                        linkDeleteButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.DeleteRelationship(this);");

                        divRelationshipButtons.Controls.Add(linkDeleteButton);
                    }
                }

                // Relationship Information
                HtmlGenericControl liRelationship = e.Item.FindControl("liRelationship") as HtmlGenericControl;
                if (liRelationship != null)
                {
                    string classes = string.Empty;
                    string originalClasses = liRelationship.Attributes[AttributeClass];
                    if (!string.IsNullOrEmpty(originalClasses))
                    {
                        liRelationship.Attributes.Remove(AttributeClass);
                        classes = originalClasses + " ";
                    }

                    switch (relationship.DefaultProfile.Direction)
                    {
                        case SyncDirection.Download:
                            classes += "download";
                            break;
                        case SyncDirection.Upload:
                            classes += "upload";
                            break;
                    }

                    liRelationship.Attributes.Add(AttributeClass, classes);
                }

                if (relationship.Profiles.Count > 1)
                {
                    // If child profiles exist (other than the default relationship
                    // profile), bind them for display as well.

                    Repeater rptProfileList = e.Item.FindControl("rptProfileList") as Repeater;
                    if (rptProfileList != null)
                    {
                        rptProfileList.DataSource = relationship.Profiles;
                        rptProfileList.DataBind();
                    }
                }
                else
                {
                    // No child profiles exist -- hide the the current list item.
                    HtmlGenericControl profileListItem = e.Item.FindControl("profilesListItem") as HtmlGenericControl;
                    if (profileListItem != null)
                    {
                        profileListItem.Visible = false;
                    }
                }
            }
            else
            {
                // Hide list item for deleted relationships.

                e.Item.Visible = false;
            }
        }
        catch (EndpointNotFoundException)
        {
            Utilities.ShowError(SiteAPI.Current.EkMsgRef.GetMessage("windows service not running"));
        }
    }

    /// <summary>
    /// Handles the ItemDataBound event for the repeater control responsible for rendering
    /// a relationship's profile data on the page.
    /// </summary>
    /// <remarks>
    /// The default profile for a relationship is intentionally hidden.
    /// </remarks>
    /// <param name="source">Repeater</param>
    /// <param name="e">Repeater arguments</param>
    protected void HandleProfileListItemDataBound(object source, RepeaterItemEventArgs e)
    {
        try
        {
            Profile profile = e.Item.DataItem as Profile;
            if (profile != null)
            {
                ProfilePresentationServices profileServices = new ProfilePresentationServices(profile);

                if (profileServices.DisplayProfile)
                {
                    // Render the link to access profile details.
                    HtmlAnchor linkProfileName = e.Item.FindControl("linkProfileName") as HtmlAnchor;
                    if (linkProfileName != null)
                    {
                        string profileLinkHRef = "";
                        if (profile.Parent.RemoteSite.Address.Contains("|"))
                        {
                            profileLinkHRef = string.Format(CloudProfileUrlFormat, ProfileViewAction, profile.Id);
                        }
                        else
                        {
                            profileLinkHRef = string.Format(ProfileUrlFormat, ProfileViewAction, profile.Id);
                        }
                        linkProfileName.HRef = profileLinkHRef;
                        linkProfileName.InnerText = profile.Name;

                        string itemsList = string.Empty;
                        if (profile.SynchronizeDatabase)
                        {
                            itemsList = itemsList + "Database, ";
                        }
                        if (profile.SynchronizeTemplates)
                        {
                            itemsList = itemsList + "Templates, ";
                        }
                        if (profile.SynchronizeWorkarea)
                        {
                            itemsList = itemsList + "Workarea, ";
                        }
                        if (profile.SynchronizeBinaries)
                        {
                            itemsList = itemsList + "Bin, ";
                        }
                        if (itemsList.Length > 0)
                        {
                            itemsList = itemsList.Substring(0, itemsList.Length - 2);
                        }

                        PopulateTextControl(e.Item.FindControl("litCurrentStatus"),  _siteApi.EkMsgRef.GetMessage("txt quickdeploy idle"));

                        string a = e.Item.FindControl("litCurrentStatus").ToString();

                        // Get Current Status of Profiles
                        if (IsSyncRunning(Convert.ToInt64(profile.Id)))
                        {
                            PopulateTextControl(e.Item.FindControl("litCurrentStatus"),  _siteApi.EkMsgRef.GetMessage("lbl running"));
                        }


                        linkProfileName.Title = String.Format(_siteApi.EkMsgRef.GetMessage("lbl Direction")+" {0}. "+_siteApi.EkMsgRef.GetMessage("lbl Including Items")+ " {1}", profile.Direction.ToString(), itemsList);
                    }
                    PopulateTextControl(
                       e.Item.FindControl("litProfileId"), profile.Id.ToString());
                    // Populate the schedule and run time fields.
                    PopulateTextControl(
                        e.Item.FindControl("litSchedule"),
                        GetResourceText(PresentationHelper.GetScheduleIntervalString(profile.Schedule.Recurrence)));

                    // Don't display the next run time when nothing has been
                    // scheduled.
                    if (profile.Schedule.Recurrence != ScheduleInterval.None)
                    {
                        PopulateTextControl(
                            e.Item.FindControl("litNextRunTime"),
                             _siteApi.EkMsgRef.GetMessage("lbl Next Sync Time")+" " + FormatDateTime(profile.Schedule.NextRunTime));
                    }

                    if (profile.LastRunResult != SyncResult.None)
                    {
                        if (profile.LastRunResult == SyncResult.Success)
                        {
                            PopulateTextControl(
                                e.Item.FindControl("litLastRunResult"),
                                _siteApi.EkMsgRef.GetMessage("generic succsess"));
                        }
                        else
                        {
                            EwsExceptionParser exceptionParser = new EwsExceptionParser();
                            PopulateTextControl(
                                e.Item.FindControl("litLastRunResult"),
                                exceptionParser.Translate(profile.LastRunMessage, SyncHandlerAction.GetStatus));
                        }
                    }

                    // Don't display the last run time if the profile
                    // has not yet been executed.
                    if (profile.LastFullSync > DateTime.MinValue)
                    {
                        PopulateTextControl(
                            e.Item.FindControl("litLastRunTime"),
                            FormatDateTime(profile.LastFullSync));
                    }

                    // Render the action buttons for the profile.
                    HtmlGenericControl divProfileButtons =
                        e.Item.FindControl("divProfileButtons") as HtmlGenericControl;

                    if (divProfileButtons != null)
                    {
                        // Add 'Sync' button.
                        if (profileServices.AllowSync)
                        {
                            HtmlAnchor linkSyncButton = new HtmlAnchor();
                            linkSyncButton.HRef = "#";
                            linkSyncButton.Title =  _siteApi.EkMsgRef.GetMessage("generic Sync");
                            linkSyncButton.Attributes.Add(AttributeClass, "syncButton");
                            linkSyncButton.Attributes.Add(AttributeRel, profile.Id.ToString());
                            linkSyncButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.Synchronize(this, true);");

                            divProfileButtons.Controls.Add(linkSyncButton);
                        }

                        // Add 'Get Status' button.
                        if (profileServices.AllowStatusRetrieval)
                        {
                            HtmlAnchor linkStatusButton = new HtmlAnchor();
                            linkStatusButton.HRef = "#";
                            linkStatusButton.Title =  _siteApi.EkMsgRef.GetMessage("lbl Get Status");
                            linkStatusButton.Attributes.Add(AttributeClass, "statusButton");
                            linkStatusButton.Attributes.Add(AttributeRel, profile.Id.ToString());
                            linkStatusButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.ShowSyncStatus(this);");

                            divProfileButtons.Controls.Add(linkStatusButton);
                        }

                        // Add 'Sync Preview' button.
                        if (profileServices.AllowSyncPreview)
                        {
                            HtmlAnchor linkPreviewButton = new HtmlAnchor();
                            linkPreviewButton.HRef = "SyncPreview.aspx?id=" + profile.Id.ToString();
                            linkPreviewButton.Title =  _siteApi.EkMsgRef.GetMessage("btn preview status");
                            linkPreviewButton.Attributes.Add(AttributeClass, "previewButton");
                            linkPreviewButton.Attributes.Add(AttributeRel, profile.Id.ToString());

                            divProfileButtons.Controls.Add(linkPreviewButton);
                        }

                        // Add 'Pause/Resume' button.
                        if (profileServices.AllowToggleSchedule &&
                            _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                        {
                            HtmlAnchor linkToggleScheduleButton = new HtmlAnchor();
                            linkToggleScheduleButton.HRef = "#";
                            linkToggleScheduleButton.Title =  _siteApi.EkMsgRef.GetMessage("lbl PauseResume");
                            linkToggleScheduleButton.Attributes.Add(AttributeRel, profile.Id.ToString());
                            linkToggleScheduleButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.ToggleScheduleStatus(this);");

                            // Display the appropriate button (play/pause) according
                            // to the current status of the profile.
                            if (profile.Status == ProfileStatus.Active)
                            {
                                linkToggleScheduleButton.Attributes.Add(AttributeClass, "toggleScheduleButton pause");
                            }
                            else
                            {
                                linkToggleScheduleButton.Attributes.Add(AttributeClass, "toggleScheduleButton resume");
                            }

                            divProfileButtons.Controls.Add(linkToggleScheduleButton);
                        }

                        // Add 'Delete' button.
                        if (profileServices.AllowDelete &&
                            _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                        {
                            HtmlAnchor linkDeleteButton = new HtmlAnchor();
                            linkDeleteButton.HRef = "#";
                            linkDeleteButton.Title =  _siteApi.EkMsgRef.GetMessage("generic delete title");
                            linkDeleteButton.Attributes.Add(AttributeClass, "deleteButton");
                            linkDeleteButton.Attributes.Add(AttributeRel, profile.Id.ToString());
                            linkDeleteButton.Attributes.Add(AttributeOnClick, "Ektron.Workarea.Sync.Relationships.DeleteProfile(this);");

                            divProfileButtons.Controls.Add(linkDeleteButton);
                        }
                    }
                }
                else
                {
                    // Do not render the default profile or deleted 
                    // profile entries.

                    e.Item.Visible = false;
                }
            }
        }
        catch (EndpointNotFoundException)
        {
            Utilities.ShowError(SiteAPI.Current.EkMsgRef.GetMessage("windows service not running"));
        }
    }
    private string GetResourceText(string st)
    {
        if (st == "None")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync none");
        else if (st == "One Time")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync one time");
        else if (st == "Hourly")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync hourly");
        else if (st == "Daily")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync daily");
        else if (st == "Weekly")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync weekly");
        else if (st == "Monthly")
            st = _siteApi.EkMsgRef.GetMessage("lbl sync monthly");

        return st;
    }
    private bool IsSyncRunning(Int64 profID)
    {
        SyncHandlerController controller = new SyncHandlerController();
        SyncHandlerController.ResultCode result;
        Profile profile = controller.IsSyncInProgress(out result);

        if (profile != null && profID == profile.Id)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Formats the specified DateTime value for display on the page.
    /// </summary>
    /// <param name="dateTime">DateTime value to format</param>
    /// <returns>Formatted DateTime string</returns>
    private string FormatDateTime(DateTime dateTime)
    {
        string formattedDateTime = string.Empty;
        if (dateTime.ToString() == DateTime.MaxValue.ToString())
        {
            formattedDateTime =  _siteApi.EkMsgRef.GetMessage("lbl sync none");
        }
        else
        {
            formattedDateTime = dateTime.ToString();
        }

        return formattedDateTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textControl"></param>
    /// <param name="text"></param>
    private void PopulateTextControl(object control, string text)
    {
        ITextControl textControl = control as ITextControl;
        if (textControl != null)
        {
            textControl.Text = text;
        }
    }

    /// <summary>
    /// 
    /// </summary>
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
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS, false);

        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "SyncRelationshipsJS", false);
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
