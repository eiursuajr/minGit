using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Microsoft.VisualBasic;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Site;

public partial class Community_DistributionWizard_DistributionWizardMetaTax : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private EkContent ekContent = null;    
    private EkMessageHelper messageHelper = null;

    private DistributionWizardEnumerations.Mode distributionMode = DistributionWizardEnumerations.Mode.None;

    public bool TaxonomyRequired = false;
    public bool MetadataRequired = false;

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new ContentAPI();
        ekContent = contentAPI.EkContentRef;
        messageHelper = contentAPI.EkMsgRef;

        distributionMode = DistributionWizardHelperMethods.GetModeFromQueryString(Request);

        switch (distributionMode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                if (ParseCommunityCopyParameters())
                {
                    long distributionFolderID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID];
                    long distributionContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];

                    ContentData content = contentAPI.GetContentById(
                        distributionContentID,
                        ContentAPI.ContentResultType.Published);

                    if (content != null)
                    {
                        int distributionLanguageID = content.LanguageId;
                        DisplayCommunityCopyMode(content, distributionFolderID);
                    }
                    else
                    {
                        DisplayErrorMessage(
                            messageHelper.GetMessage("distribution wizard required parameters"),
                            true);
                    }
                }
                else
                {
                    DisplayErrorMessage(
                        messageHelper.GetMessage("distribution wizard required parameters"),
                        true);
                }
                break;
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                if (ParseCommunityRedistributeParameters())
                {
                    long distributionContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID];

                    ContentData content = null;

                    try
                    {
                        content = contentAPI.GetContentById(
                            distributionContentID,
                            ContentAPI.ContentResultType.Published);
                    }
                    catch
                    {
                        // Exception likely occurred due to the user not being logged in.
                        // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
                        // returns true if user is user is logged out from a different client)
                        // to check in this case.
                    }

                    if (content != null)
                    {
                        long distributionFolderID = content.FolderId;
                        long distributionLanguageID = content.LanguageId;
                        DisplayCommunityRedistributeMode(content, content.FolderId);
                    }
                    else
                    {
                        DisplayErrorMessage(
                            messageHelper.GetMessage("distribution wizard required parameters"),
                            true);
                    }
                }
                else
                {
                    DisplayErrorMessage(
                        messageHelper.GetMessage("distribution wizard required parameters"),
                        true);
                }
                break;

            case DistributionWizardEnumerations.Mode.CommunityReplace:
                if (ParseCommunityReplaceParameters())
                {
                    long distributionContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID];

                    ContentData content = contentAPI.GetContentById(
                        distributionContentID,
                        ContentAPI.ContentResultType.Published);

                    if (content != null)
                    {
                        long distributionFolderID = content.FolderId;
                        long distributionLanguageID = content.LanguageId;
                        DisplayCommunityReplaceMode(content, content.FolderId);
                    }
                    else
                    {
                        DisplayErrorMessage(
                            messageHelper.GetMessage("distribution wizard required parameters"),
                            true);
                    }
                }
                else
                {
                    DisplayErrorMessage(
                        messageHelper.GetMessage("distribution wizard required parameters"),
                        true);
                }
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
                if (ParseSharepointParameters())
                {
                    long distributionContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID];

                    ContentData content = contentAPI.GetContentById(
                        distributionContentID,
                        ContentAPI.ContentResultType.Published);

                    if (content != null)
                    {
                        long distributionFolderID = content.FolderId;
                        int distributionLanguageID = content.LanguageId;

                        DisplaySharepointMode(content, distributionFolderID);
                    }
                    else
                    {
                        DisplayErrorMessage(
                            messageHelper.GetMessage("distribution wizard required parameters"),
                            true);
                    }
                }
                else
                {
                    DisplayErrorMessage(
                        messageHelper.GetMessage("distribution wizard required parameters"),
                        true);
                }
                break;
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                if (ParseSharepointParameters())
                {
                    long distributionContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID];

                    ContentData content = null;

                    try
                    {
                        content = contentAPI.GetContentById(
                            distributionContentID,
                            ContentAPI.ContentResultType.Published);
                    }
                    catch
                    {
                        // Exception likely occurred due to the user not being logged in.
                        // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
                        // returns true if user is user is logged out from a different client)
                        // to check in this case.
                    }

                    if (content != null)
                    {
                        long distributionFolderID = content.FolderId;
                        int distributionLanguageID = content.LanguageId;

                        DisplaySharepointMode(content, distributionFolderID);
                    }
                    else
                    {
                        DisplayErrorMessage(
                            messageHelper.GetMessage("distribution wizard required parameters"),
                            true);
                    }
                }
                else
                {
                    DisplayErrorMessage(
                        messageHelper.GetMessage("distribution wizard required parameters"),
                        true);
                }
                break;
        }

        RegisterStylesheets();
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        Session[DistributionWizardConstants.SESSION_PARAM_METADATA] = inputMetadata.Metadata;
        Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] = selectTaxonomy.SelectedTaxonomyCategoryIDs;

        RedirectToConfirmation(distributionMode);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        switch (distributionMode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                Response.Redirect("~/Workarea/Community/DistributionWizard/DistributionWizardChooseFolder.aspx");
                break;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
                Response.Redirect("~/Workarea/Community/DistributionWizard/DistributionWizardChooseContent.aspx");
                break;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string cancelUrl = string.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx?action=cancel&mode={0}",
            DistributionWizardHelperMethods.GetQueryStringFromMode(distributionMode));
        
        Response.Redirect(cancelUrl, false);
    }
    #endregion

    #region Methods - Private
    /// <summary>
    /// Display UI elements to support community distribution.
    /// </summary>
    private void DisplayCommunityCopyMode(ContentData content, long destinationFolderID)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 3";
            masterPage.WizardStepTitle = "Choose metadata and taxonomy for the copy:";
        }

        this.TaxonomyRequired = IsTaxonomyRequired(destinationFolderID, content.LanguageId);
        this.MetadataRequired = IsMetadataRequired(destinationFolderID, content.LanguageId);
        TaxonomyRequired = true;
        MetadataRequired = true;
        if ( MetadataRequired || TaxonomyRequired )
        {
            if (!IsPostBack)
            {
                InitializeMetaTaxControls(content, destinationFolderID, false);
            }
        }
        else
        {
            RedirectToConfirmation(DistributionWizardEnumerations.Mode.CommunityCopy);
        }
    }

    /// <summary>
    /// Display UI eleemtns to support community redistribution.
    /// </summary>
    private void DisplayCommunityRedistributeMode(ContentData content, long destinationFolderID)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 2";
            masterPage.WizardStepTitle = "Update metadata and taxonomy for the content:";
        }

        btnBack.Enabled = false;

        this.TaxonomyRequired = IsTaxonomyRequired(destinationFolderID, content.LanguageId);

        if (IsMetadataRequired(destinationFolderID, content.LanguageId) || TaxonomyRequired)
        {
            if (!IsPostBack)
            {
                InitializeMetaTaxControls(content, destinationFolderID, true);
            }
        }
        else
        {
            RedirectToConfirmation(DistributionWizardEnumerations.Mode.CommunityRedistribute);
        }
    }

    /// <summary>
    /// Display UI eleemtns to support community replace.
    /// </summary>
    private void DisplayCommunityReplaceMode(ContentData content, long destinationFolderID)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 3";
            masterPage.WizardStepTitle = "Update metadata and taxonomy for the content:";
        }

        this.TaxonomyRequired = IsTaxonomyRequired(destinationFolderID, content.LanguageId);

        if (IsMetadataRequired(destinationFolderID, content.LanguageId) || TaxonomyRequired)
        {
            if (!IsPostBack)
            {
                InitializeMetaTaxControls(content, destinationFolderID, true);
            }
        }
        else
        {
            RedirectToConfirmation(DistributionWizardEnumerations.Mode.CommunityReplace);
        }
    }

    /// <summary>
    /// Display UI elements to support sharepoint distribution.
    /// </summary>
    private void DisplaySharepointMode(ContentData content, long destinationFolderID)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 2";
            masterPage.WizardStepTitle = "Choose metadata and taxonomy for the copy:";
        }

        btnBack.Enabled = false;    // Sharepoint users cannot go back beyond this point.

        this.TaxonomyRequired = IsTaxonomyRequired(destinationFolderID, content.LanguageId);

        if (IsMetadataRequired(destinationFolderID, content.LanguageId) || TaxonomyRequired)
        {
            if (!IsPostBack)
            {
                InitializeMetaTaxControls(content, destinationFolderID, true);
            }
        }
        else
        {
            RedirectToConfirmation(distributionMode);
        }
    }

    /// <summary>
    /// Parse and validate parameters intended for sharepoint distribution.
    /// </summary>
    /// <returns>True if the parameters are valid, false otherwise</returns>
    private bool ParseSharepointParameters()
    {
        bool validContentID = true;
        if (!String.IsNullOrEmpty(Request.QueryString["ContentID"]))
        {
            long contentID = -1;
            if (long.TryParse(Request.QueryString["ContentID"], out contentID))
            {
                Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID] = contentID;
            }
            else
            {
                validContentID = false;
            }
        }
        else
        {
            validContentID = false;
        }

        bool validGUID = true;
        if (!String.IsNullOrEmpty(Request.QueryString["GUID"]))
        {
            Guid guid = default(Guid);
            try
            {
                guid = new Guid(Request.QueryString["GUID"]);
            }
            catch
            {
                validGUID = false;
            }

            if (guid != default(Guid))
            {
                Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID] = guid;
            }
        }
        else
        {
            validGUID = false;
        }

        bool validItemID = true;
        if (!String.IsNullOrEmpty(Request.QueryString["ItemID"]))
        {
            long itemID = -1;
            if (long.TryParse(Request.QueryString["ItemID"], out itemID))
            {
                Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_ITEMID] = itemID;
            }
            else
            {
                validItemID = false;
            }
        }

        return validContentID && validGUID && validItemID;
    }

    /// <summary>
    /// Parse and validate parameters intended for community  copy.
    /// </summary>
    /// <returns>True if parameters are valid, false otherwise</returns>
    private bool ParseCommunityCopyParameters()
    {
        bool validOrigContentID = true;
        if (Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] == null)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["OriginalContentID"]))
            {
                long originalContentID = -1;
                if (long.TryParse(Request.QueryString["OriginalContentID"], out originalContentID))
                {
                    Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] = originalContentID;
                }
                else
                {
                    validOrigContentID = false;
                }
            }
            else
            {
                validOrigContentID = false;
            }
        }

        bool validDestFolderID = true;
        if (Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID] == null)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["DestFolderID"]))
            {
                long destFolderID = -1;
                if (long.TryParse(Request.QueryString["DestFolderID"], out destFolderID))
                {
                    Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID] = destFolderID;
                }
                else
                {
                    validDestFolderID = false;
                }
            }
            else
            {
                validDestFolderID = false;
            }
        }

        return validOrigContentID && validDestFolderID;
    }

    /// <summary>
    /// Parse and validate parameters intended for community replace.
    /// </summary>
    /// <returns>True if parameters are valid, false otherwise</returns>
    private bool ParseCommunityReplaceParameters()
    {
        bool validContentID = true;
        if (Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] == null)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["DestinationContentID"]))
            {
                long contentID = -1;
                if (long.TryParse(Request.QueryString["DestinationContentID"], out contentID))
                {
                    Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] = contentID;
                }
                else
                {
                    validContentID = false;
                }
            }
            else
            {
                validContentID = false;
            }
        }

        return validContentID;
    }

    /// <summary>
    /// Parse and validate parameters intended for community redistribution.
    /// </summary>
    /// <returns>True if parameters are valid, false otherwise</returns>
    private bool ParseCommunityRedistributeParameters()
    {
        bool validDestinationContentID = true;
        if (Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] == null)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["DestinationContentID"]))
            {
                long destinationContentID = -1;
                if (long.TryParse(Request.QueryString["DestinationContentID"], out destinationContentID))
                {
                    Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] = destinationContentID;
                }
                else
                {
                    validDestinationContentID = false;
                }
            }
            else
            {
                validDestinationContentID = false;
            }
        }

        return validDestinationContentID;
    }

    /// <summary>
    /// Initialize the metadata and taxonomy input controls on the page.
    /// </summary>
    /// <param name="contentID">ID of the original content</param>
    /// <param name="folderID">ID of the destination folder</param>
    private void InitializeMetaTaxControls(ContentData content, long destinationFolderID, bool presetAssignedCategories)
    {
        inputMetadata.ContentID = content.Id;
        inputMetadata.FolderID = destinationFolderID;

        FolderData folder = contentAPI.GetFolderById(destinationFolderID, true);
        if (folder.FolderTaxonomy.Length > 0)
        {
            List<long> rootCategories = new List<long>();
            foreach (TaxonomyBaseData taxonomyBase in folder.FolderTaxonomy)
            {
                rootCategories.Add(taxonomyBase.TaxonomyId);
            }
            selectTaxonomy.RootCategories = rootCategories;

            if (presetAssignedCategories)
            {
                selectTaxonomy.SelectedTaxonomyCategoryIDs = GetAssignedTaxonomyCategories(content.Id, content.LanguageId);
            }

            selectTaxonomy.LanguageID = content.LanguageId;
        }
        else
        {
            // No taxonomy associated, hide taxonomy options.
            selectTaxonomy.Visible = false;
            lblTaxonomyMessage.Text = "There is no taxonomy associated with the destination folder.";
        }
    }

    /// <summary>
    /// Displays an error message
    /// </summary>
    /// <param name="errorMessage">Message to be displayed</param>
    /// <param name="isFatalError">True if this error is fatal, false otherwise</param>
    private void DisplayErrorMessage(string errorMessage, bool isFatalError)
    {
        if (isFatalError)
        {
            string errorUrl = String.Format(
                "~/Workarea/Community/DistributionWizard/DistributionWizardError.aspx?Error={0}&Mode={1}",
                EkFunctions.UrlEncode(errorMessage),
                DistributionWizardHelperMethods.GetQueryStringFromMode(distributionMode));

            Response.Redirect(errorUrl);
        }
        else
        {
            lblErrorMessage.Text = errorMessage;
        }
    }

    /// <summary>
    /// Register this page's stylesheets with the master.
    /// </summary>
    private void RegisterStylesheets()
    {
        HtmlLink linkStylesheet = new HtmlLink();
        linkStylesheet.Attributes.Add("type", "text/css");
        linkStylesheet.Attributes.Add("rel", "Stylesheet");
        linkStylesheet.Href = "DistributionWizardMetaTax.css";
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
		
        Page.Master.Page.Header.Controls.Add(linkStylesheet);
    }

    /// <summary>
    /// Determine if the content's destination folder requires taxonomy.
    /// </summary>
    /// <param name="folderID">ID of the destination folder</param>
    /// <param name="languageID">ID of the content language</param>
    /// <returns>True if the destination folder requires taxonomy, false otherwise</returns>
    private bool IsTaxonomyRequired(long folderID, int languageID)
    {
        return ekContent.DoesFolderRequireTaxonomy(folderID, languageID);
    }

    /// <summary>
    /// Determine if the content's destination folder has required metadata fields.
    /// </summary>
    /// <param name="folderID">ID of the destination folder</param>
    /// <param name="languageID">ID of the content language</param>
    /// <returns>True if the destination folder requires metadata, false otherwise</returns>
    public bool IsMetadataRequired(long folderID, int languageID)
    {
        bool metadataRequired = false;

        Collection metadataFields = contentAPI.GetFieldsByFolder(folderID, languageID);
        for (int i = 1; i <= metadataFields.Count && !metadataRequired; i++)
        {
            if (metadataFields[i] != null)
            {
                Collection metadataField = (Collection)metadataFields[i];
                if (metadataField.Contains("Required") && Convert.ToInt32(metadataField["Required"]) != 0)
                {
                    metadataRequired = true;
                }
            }
        }

        return metadataRequired;
    }

    /// <summary>
    /// Determine if the content's destination folder has metadata or taxonomy associated with it.
    /// </summary>
    /// <param name="folderID">ID of the destination folder</param>
    /// <param name="languageID">ID of the content language</param>
    /// <returns>True if the destination folder has metadata or taxonomy associated, false otherwise</returns>
    private bool HasMetadataOrTaxonomy(long folderID, int languageID)
    {
        return ekContent.DoesFolderHaveMetadataOrTaxonomy(folderID, languageID);
    }

    /// <summary>
    /// Gets a collection of taxonomy category IDs associated with the specified
    /// content.
    /// </summary>
    /// <param name="contentID">ID of content</param>
    /// <param name="languageID">ID of content language</param>
    /// <returns>Collection of taxonomy category IDs associated with the specified content.</returns>
    private IEnumerable<long> GetAssignedTaxonomyCategories(long contentID, int languageID)
    {
        List<long> assignedCategories = new List<long>();

        // Store RequestInformationRef's original language so that it can be restored later.
        int originalLanguageID = contentAPI.RequestInformationRef.ContentLanguage;
        contentAPI.RequestInformationRef.ContentLanguage = languageID;

        DirectoryData[] directoryData =
            contentAPI.GetAllAssignedDirectory(contentID, (int)EkEnumeration.TaxonomyItemType.Content);

        // Restore RequestInformationRef's original language.
        contentAPI.RequestInformationRef.ContentLanguage = originalLanguageID;

        foreach (DirectoryData category in directoryData)
        {
            assignedCategories.Add(category.DirectoryId);
        }

        return assignedCategories;
    }

    /// <summary>
    /// Redirect to the distribution wizard's confirmation screen.
    /// </summary>
    /// <param name="mode"></param>
    private void RedirectToConfirmation(DistributionWizardEnumerations.Mode mode)
    {
        string modeParam = string.Empty;
        
        switch( mode )
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                modeParam = DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY;
                break;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
                modeParam = DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE;
                break;
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                modeParam = DistributionWizardConstants.QUERY_MODE_COMMUNITYREDISTRIBUTE;
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
                modeParam = DistributionWizardConstants.QUERY_MODE_SHAREPOINTCOPY;
                break;
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                modeParam = DistributionWizardConstants.QUERY_MODE_SHAREPOINTREDISTRIBUTE;
                break;
        }

        Response.Redirect(String.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardConfirm.aspx?Mode={0}", modeParam),
            false);
    }
    #endregion

    #region Properties - Public
    public string DistributionModeString
    {
        get
        {
            return DistributionWizardHelperMethods.GetQueryStringFromMode(distributionMode);
        }
    }
    #endregion
}
