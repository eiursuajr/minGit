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

public partial class Community_DistributionWizard_DistributionWizardConfirm : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private EkContent ekContent = null;
    private EkMessageHelper messageHelper = null;

    private DistributionWizardEnumerations.Mode mode;

    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new ContentAPI();
        ekContent = contentAPI.EkContentRef;
        messageHelper = contentAPI.EkMsgRef;

        mode = DistributionWizardHelperMethods.GetModeFromQueryString(Request);

        DisplayConfirmationMode(mode);
        RegisterStylesheets();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        switch (mode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] == null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_METADATA] == null)
                {
                    Response.Redirect(
                        String.Format("~/Workarea/Community/DistributionWizard/DistributionWizardChooseFolder.aspx?mode={0}", DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY),
                        false);
                }
                else
                {
                    Response.Redirect(
                        String.Format("~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?mode={0}", DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY),
                        false);
                }
                break;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
                if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] == null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_METADATA] == null)
                {
                    Response.Redirect(
                        String.Format("~/Workarea/Community/DistributionWizard/DistributionWizardChooseContent.aspx?mode={0}", DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE),
                        false);
                }
                else
                {
                    Response.Redirect(
                        String.Format("~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?mode={0}", DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE),
                        false);
                }
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
                if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_METADATA] != null)
                {
                    string sharepointModeUrl = String.Format(
                        "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?mode={0}&contentid={1}&guid={2}&ItemID={3}",
                        DistributionWizardConstants.QUERY_MODE_SHAREPOINTCOPY,
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID].ToString(),
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID].ToString(),
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_ITEMID].ToString());

                    Response.Redirect(sharepointModeUrl, false);
                }
                break;
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_METADATA] != null)
                {
                    string sharepointModeUrl = String.Format(
                        "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?mode={0}",
                        DistributionWizardConstants.QUERY_MODE_COMMUNITYREDISTRIBUTE);

                    Response.Redirect(sharepointModeUrl, false);
                }
                break;
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_METADATA] != null)
                {
                    string sharepointModeUrl = String.Format(
                        "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?mode={0}&contentid={1}&guid={2}&ItemID={3}",
                        DistributionWizardConstants.QUERY_MODE_SHAREPOINTREDISTRIBUTE,
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID].ToString(),
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID].ToString(),
                        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_ITEMID].ToString());

                    Response.Redirect(sharepointModeUrl, false);
                }
                break;
            case DistributionWizardEnumerations.Mode.None:
                break;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string cancelUrl = string.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx?action=cancel&mode={0}",
            DistributionWizardHelperMethods.GetQueryStringFromMode(mode));

        Response.Redirect(cancelUrl, false);
    }

    protected void btnDone_Click(object sender, EventArgs e)
    {
        bool distributionSuccessful = true;

        switch (mode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                distributionSuccessful = ExecuteCommunityCopy();
                break;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
                distributionSuccessful = ExecuteCommunityReplace();
                break;
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                distributionSuccessful = ExecuteCommunityRedistribute();
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
                distributionSuccessful = ExecuteSharepointPublish();
                break;
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                distributionSuccessful = ExecuteSharepointPublish(false);
                break;
        }

        if (distributionSuccessful)
        {
            if (mode != DistributionWizardEnumerations.Mode.Sharepoint &&
                mode != DistributionWizardEnumerations.Mode.SharepointRedistribute)
            {
                Response.Redirect(
                    "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx",
                    true);
            }
            else
            {
                string closeUrl = string.Format(
                    "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx?Mode={0}",
                    DistributionWizardHelperMethods.GetQueryStringFromMode(mode));

                Response.Redirect(closeUrl, true);
            }
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard distribute failed"));
        }
    }

    /// <summary>
    /// Initializes the UI to display for the specified mode.
    /// </summary>
    /// <param name="modeToDisplay">Distribution mode</param>
    private void DisplayConfirmationMode(DistributionWizardEnumerations.Mode modeToDisplay)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            if (ValidateParameters(mode))
            {
                switch (modeToDisplay)
                {
                    case DistributionWizardEnumerations.Mode.CommunityCopy:
                        masterPage.WizardStepHeader = "Step 4";
                        masterPage.WizardStepTitle = "Copy Confirmation";
                        ltrConfirmationDetails.Text = GetCopyConfirmationDetails(
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID],
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID]);
                        break;
                    case DistributionWizardEnumerations.Mode.CommunityReplace:
                        masterPage.WizardStepHeader = "Step 3";
                        masterPage.WizardStepTitle = "Replacement Confirmation";
                        ltrConfirmationDetails.Text = GetReplaceConfirmationDetails(
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID],
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID]);
                        break;
                    case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                        masterPage.WizardStepHeader = "Step 2";
                        masterPage.WizardStepTitle = "Distribution Confirmation";

                        btnBack.Enabled = false;

                        ltrConfirmationDetails.Text = GetRedistributeConfirmationDetails(
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID],
                            (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID]);
                        break;
                    case DistributionWizardEnumerations.Mode.Sharepoint:
                    case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                        masterPage.WizardStepHeader = "Step 3";
                        masterPage.WizardStepTitle = "Publish Confirmation";

                        if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] == null &&
                            Session[DistributionWizardConstants.SESSION_PARAM_METADATA] == null)
                        {
                            btnBack.Enabled = false;
                        }

                        ltrConfirmationDetails.Text = GetSharepointConfirmationDetails(
                            (long)(long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID]);
                        break;
                }
            }
            else
            {
                DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
            }
        }
    }

    /// <summary>
    /// Validate the parameters necessary for completing the distribution process.
    /// </summary>
    /// <param name="mode">Distribution mode for which to validate</param>
    /// <returns>True if the parameters are valid for the specified mode, false otherwise</returns>
    private bool ValidateParameters(DistributionWizardEnumerations.Mode mode)
    {
        switch (mode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                return Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID] != null;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                return Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] != null;
            case DistributionWizardEnumerations.Mode.Sharepoint:
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                return Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID] != null &&
                    Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID] != null;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns the confirmation details (and markup) to confirm for
    /// community copy mode.
    /// </summary>
    /// <param name="originalContentID">ID of the original content</param>
    /// <param name="destinationFolderID">ID of the folder to which content is copied</param>
    /// <returns>Returns the confirmation details (and markup)</returns>
    private string GetCopyConfirmationDetails(long originalContentID, long destinationFolderID)
    {
        ContentData content = null;
        try
        {
            content = contentAPI.GetContentById(
                originalContentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        StringBuilder sbConfirmationDetails = new StringBuilder();

        if (content != null)
        {
            string contentTitle = content.Title;
            string destinationFolderPath = ekContent.GetFolderPath(destinationFolderID);

            sbConfirmationDetails.Append("<ul><li>Distribute:<ul class=\"ConfirmationDetailsList\"><li>");
            sbConfirmationDetails.Append(contentTitle);
            sbConfirmationDetails.Append("</li></ul></li><li>To:<ul class=\"ConfirmationDetailsList\"><li>");
            sbConfirmationDetails.Append(destinationFolderPath);

            List<long> taxonomyIDs = new List<long>(GetTaxonomyCategoryIDList());
            if (taxonomyIDs.Count > 0)
            {
                sbConfirmationDetails.Append("</li></ul></li><li>Taxonomy Breadcrumb:<ul class=\"ConfirmationDetailsList\">");
                foreach (long taxonomyID in taxonomyIDs)
                {
                    sbConfirmationDetails.Append("<li>");
                    sbConfirmationDetails.Append(GetTaxonomyBreadcrumb(taxonomyID, content.LanguageId));
                    sbConfirmationDetails.Append("</li>");
                }

            }

            sbConfirmationDetails.Append("</li></ul></li></ul>");

            FolderData destinationFolder = contentAPI.GetFolderById(destinationFolderID);
            if (destinationFolder.XmlConfiguration != null)
            {
                sbConfirmationDetails.Append("<div id=\"DistributionWizardConfirmationInformation\">Warning: ");
                sbConfirmationDetails.Append(messageHelper.GetMessage("warn txt non xml to xml"));
                sbConfirmationDetails.Append("</div>");
            }

        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return sbConfirmationDetails.ToString();
    }

    /// <summary>
    /// Returns the confirmation details (and markup) to confirm for
    /// community replace mode.
    /// </summary>
    /// <param name="originalContentID">ID of the original content</param>
    /// <param name="destinationContentID">ID of the content being replaced</param>
    /// <returns>Returns the confirmation details (and markup)</returns>
    private string GetReplaceConfirmationDetails(long originalContentID, long destinationContentID)
    {

        ContentData originalContent = null;
        ContentData destinationContent = null;
        try
        {
            originalContent = contentAPI.GetContentById(
                originalContentID,
                ContentAPI.ContentResultType.Published);

            destinationContent = contentAPI.GetContentById(
                destinationContentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        StringBuilder sbConfirmationDetails = new StringBuilder();

        if (originalContent != null && destinationContent != null)
        {
            string originalContentTitle = originalContent.Title;
            string destinationContentTitle = destinationContent.Title;
            string destinationFolderPath = ekContent.GetFolderPath(destinationContent.FolderId);

            sbConfirmationDetails.Append("<ul><li>Distribute:<ul><li>");
            sbConfirmationDetails.Append(originalContentTitle);
            sbConfirmationDetails.Append("</li></ul></li><li>To Replace:<ul><li>");
            sbConfirmationDetails.Append(destinationFolderPath);
            sbConfirmationDetails.Append("\\");
            sbConfirmationDetails.Append(destinationContentTitle);

            List<long> taxonomyIDs = new List<long>(GetTaxonomyCategoryIDList());
            if (taxonomyIDs.Count > 0)
            {
                sbConfirmationDetails.Append("</li></ul></li><li>Taxonomy Breadcrumb:<ul class=\"ConfirmationDetailsList\">");
                foreach (long taxonomyID in taxonomyIDs)
                {
                    sbConfirmationDetails.Append("<li>");
                    sbConfirmationDetails.Append(GetTaxonomyBreadcrumb(taxonomyID, originalContent.LanguageId));
                    sbConfirmationDetails.Append("</li>");
                }

            }

            sbConfirmationDetails.Append("</li></ul></li></ul>");
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return sbConfirmationDetails.ToString();
    }

    /// <summary>
    /// Returns the confirmation details (and markup) to confirm for
    /// community redistribute mode.
    /// </summary>
    /// <param name="originalContentID">ID of the original content</param>
    /// <param name="destinationContentID">ID of the original content's distributed counterpart</param>
    /// <returns>Returns the confirmation details (and markup)</returns>
    private string GetRedistributeConfirmationDetails(long originalContentID, long destinationContentID)
    {
        ContentData originalContent = null;
        ContentData destinationContent = null;
        try
        {
            originalContent = contentAPI.GetContentById(
                originalContentID,
                ContentAPI.ContentResultType.Published);

            destinationContent = contentAPI.GetContentById(
                destinationContentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        StringBuilder sbConfirmationDetails = new StringBuilder();

        if (originalContent != null && destinationContent != null)
        {
            string destinationFolderPath = ekContent.GetFolderPath(destinationContent.FolderId);

            sbConfirmationDetails.Append("<ul><li>Distribute:<ul><li>");
            sbConfirmationDetails.Append(originalContent.Title);
            sbConfirmationDetails.Append("</li></ul></li><li>To:<ul><li>");
            sbConfirmationDetails.Append(destinationFolderPath);
            sbConfirmationDetails.Append("\\");
            sbConfirmationDetails.Append(destinationContent.Title);

            List<long> taxonomyIDs = new List<long>(GetTaxonomyCategoryIDList());
            if (taxonomyIDs.Count > 0)
            {
                sbConfirmationDetails.Append("</li></ul></li><li>Taxonomy Breadcrumb:<ul class=\"ConfirmationDetailsList\">");
                foreach (long taxonomyID in taxonomyIDs)
                {
                    sbConfirmationDetails.Append("<li>");
                    sbConfirmationDetails.Append(GetTaxonomyBreadcrumb(taxonomyID, originalContent.LanguageId));
                    sbConfirmationDetails.Append("</li>");
                }

            }

            sbConfirmationDetails.Append("</li></ul></li></ul>");
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return sbConfirmationDetails.ToString();
    }

    /// <summary>
    /// Returns the confirmation details (and markup) to confirm for
    /// sharepoint publishing mode.
    /// </summary>
    /// <param name="contentID">ID of content being published</param>
    /// <returns>Confirmation details (and markup)</returns>
    private string GetSharepointConfirmationDetails(long contentID)
    {
        ContentData content = null;
        try
        {
            content = contentAPI.GetContentById(
                contentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        StringBuilder sbConfirmationDetails = new StringBuilder();

        if (content != null)
        {
            string contentTitle = content.Title;
            string folderPath = ekContent.GetFolderPath(content.FolderId);

            sbConfirmationDetails.Append("<ul><li>Publish:<ul><li>");
            sbConfirmationDetails.Append(contentTitle);
            sbConfirmationDetails.Append("</li></ul></li><li>To:<ul><li>");
            sbConfirmationDetails.Append(folderPath);

            List<long> taxonomyIDs = new List<long>(GetTaxonomyCategoryIDList());
            if (taxonomyIDs.Count > 0)
            {
                sbConfirmationDetails.Append("</li></ul></li><li>Taxonomy Breadcrumb:<ul class=\"ConfirmationDetailsList\">");
                foreach (long taxonomyID in taxonomyIDs)
                {
                    sbConfirmationDetails.Append("<li>");
                    sbConfirmationDetails.Append(GetTaxonomyBreadcrumb(taxonomyID, content.LanguageId));
                    sbConfirmationDetails.Append("</li>");
                }
            }

            sbConfirmationDetails.Append("</li></ul></li></ul>");

            FolderData destinationFolder = contentAPI.GetFolderById(content.FolderId);
            if (destinationFolder.XmlConfiguration != null)
            {
                sbConfirmationDetails.Append("<div id=\"DistributionWizardConfirmationInformation\">Warning: ");
                sbConfirmationDetails.Append(messageHelper.GetMessage("warn txt non xml to xml"));
                sbConfirmationDetails.Append("</div>");
            }
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return sbConfirmationDetails.ToString();
    }

    /// <summary>
    /// Returns a string representing the breadcrumb trail for the specified taxonomy category.
    /// </summary>
    /// <param name="taxonomyID">ID of taxonomy category</param>
    /// <param name="languageID">ID of language</param>
    /// <returns>String representing the breadcrumb trail for the specified taxonomy category</returns>
    private string GetTaxonomyBreadcrumb(long taxonomyID, int languageID)
    {
        TaxonomyBaseData[] taxonomyData = ekContent.GetTaxonomyRecursiveToParent(
            taxonomyID,
            languageID,
            (int)EkEnumeration.TaxonomyType.Content);

        StringBuilder sbTaxonomyBreadcrumb = new StringBuilder();
        for (int i = 0; i < taxonomyData.Length; i++)
        {
            if (i != 0)
            {
                sbTaxonomyBreadcrumb.Append(" > ");
            }
            sbTaxonomyBreadcrumb.Append(taxonomyData[i].TaxonomyName);
        }

        return sbTaxonomyBreadcrumb.ToString();
    }

    /// <summary>
    /// Completes the copy of community content.
    /// </summary>
    private bool ExecuteCommunityCopy()
    {
        bool distributionSuccessful = true;

        long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];
        long destinationContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID];

        ContentData originalContent = null;
        try
        {
            originalContent = contentAPI.GetContentById(
                originalContentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        if (originalContent != null &&
            DistributionWizardHelperMethods.HasDistributionPrivileges(originalContent.FolderId))
        {
            try
            {
                contentAPI.DistributeCommunityContent(
                    originalContentID,
                    destinationContentID,
                    originalContent.LanguageId,
                    GetTaxonomyCategoryIDList(),
                    GetContentMetadataListByLanguageId(originalContent.LanguageId));
            }
            catch
            {
                distributionSuccessful = false;
            }
        }
        else
        {
            distributionSuccessful = false;
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return distributionSuccessful;
    }

    /// <summary>
    /// Completes the redistribution of community content.
    /// </summary>
    private bool ExecuteCommunityRedistribute()
    {
        long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];
        long destinationContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID];

        return ReplaceExistingContent(originalContentID, destinationContentID, false);
    }

    /// <summary>
    /// Completes the replacement of site content with community content.
    /// </summary>
    private bool ExecuteCommunityReplace()
    {
        long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];
        long destinationContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID];

        return ReplaceExistingContent(originalContentID, destinationContentID, true);
    }

    /// <summary>
    /// Completes the publication of sharepoint content by applying metadata
    /// and taxonomy and creates a distribution relationship.
    /// </summary>
    /// <returns>True if publish completes successfully, false otherwise</returns>
    public bool ExecuteSharepointPublish()
    {
        return ExecuteSharepointPublish(true);
    }

    /// <summary>
    /// Completes the publication of sharepoint content by applying metadata
    /// and taxonomy.
    /// </summary>
    /// <param name="createDistributionRelationship">True if distribution relationship should be created, false otherwise</param>
    /// <returns>True if publish completes successfully, false otherwise</returns>
    private bool ExecuteSharepointPublish(bool createDistributionRelationship)
    {
        bool distributionSuccessful = true;

        long contentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID];
        Guid sharepointGuid = (Guid)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID];
        long sharepointItemId = (long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_ITEMID];

        ContentData content = null;
        try
        {
            content = contentAPI.GetContentById(
                contentID,
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
            // Store the current content language, and update the RequestInformationRef to
            // reflect the language of the content being distributed.
            int originalLanguageID = contentAPI.RequestInformationRef.ContentLanguage;
            contentAPI.RequestInformationRef.ContentLanguage = content.LanguageId;

            try
            {
                contentAPI.CheckOutContentById(contentID);

                // Update metadata and taxonomy details
                UpdateMetadataAndTaxonomy(contentID, content.LanguageId);

                // Note: PublishContentById returns false for success!
                if (!contentAPI.PublishContentById(contentID, content.FolderId, content.LanguageId, String.Empty, contentAPI.UserId, string.Empty))
                {
                    if (createDistributionRelationship)
                    {
                        ekContent.AddDistributionRelationship(
                            sharepointGuid,
                            sharepointItemId,
                            contentID,
                            content.LanguageId);
                    }
                }
                else
                {
                    distributionSuccessful = false;
                }
            }
            catch
            {
                distributionSuccessful = false;
            }

            // Restore the content language to its previous value.
            contentAPI.RequestInformationRef.ContentLanguage = originalLanguageID;
        }
        else
        {
            distributionSuccessful = false;
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        return distributionSuccessful;
    }

    /// <summary>
    /// Replace existing content with other content's data.
    /// </summary>
    /// <param name="originalContentID">ID of content to replace existing content</param>
    /// <param name="destinationContentID">ID of content to be replaced</param>
    private bool ReplaceExistingContent(long originalContentID, long destinationContentID, bool addDistributionRelationship)
    {
        bool success = true;

        ContentData originalContent = null;
        ContentData destinationContent = null;
        try
        {
            originalContent = contentAPI.GetContentById(
                originalContentID,
                ContentAPI.ContentResultType.Published);

            destinationContent = contentAPI.GetContentById(
                destinationContentID,
                ContentAPI.ContentResultType.Published);
        }
        catch
        {
            // Exception likely occurred due to the user not being logged in.
            // Unfortunately, ContentAPI.IsLoggedIn is not reliable enough (still
            // returns true if user is user is logged out from a different client)
            // to check in this case.
        }

        if (originalContent != null &&
            destinationContent != null &&
            DistributionWizardHelperMethods.HasDistributionPrivileges(destinationContent.FolderId))
        {
            if (originalContent.ContType == ((int)EkEnumeration.CMSContentType.Content))
            {
                // Store the current content language, and update the RequestInformationRef to
                // reflect the language of the content being distributed.
                int originalLanguageID = contentAPI.RequestInformationRef.ContentLanguage;
                contentAPI.RequestInformationRef.ContentLanguage = originalContent.LanguageId;

                try
                {
                    contentAPI.CheckOutContentById(destinationContentID);

                    // Edit content teaser and html
                    ContentEditData contentEditData = GetContentEditDataFromExistingContent(destinationContentID);
                    contentEditData.Title = originalContent.Title;
                    contentEditData.Teaser = originalContent.Teaser;
                    contentEditData.Html = originalContent.Html;

                    // Save updated content
                    contentAPI.SaveContent(contentEditData);

                    // Update metadata and taxonomy if any has been specified.
                    UpdateMetadataAndTaxonomy(destinationContentID, destinationContent.LanguageId);

                    // Commit the change.
                    contentAPI.PublishContentById(
                        destinationContentID,
                        destinationContent.FolderId,
                        destinationContent.LanguageId,
                        string.Empty,
                        contentAPI.UserId,
                        string.Empty);
                }
                catch
                {
                    if (contentAPI.GetContentState(destinationContentID).Status == "O")
                    {
                        contentAPI.UndoCheckoutById(destinationContentID);
                    }
                    success = false;
                }
                finally
                {
                    contentAPI.RequestInformationRef.ContentLanguage = originalLanguageID;
                }
            }
            else if (originalContent.ContType == ((int)EkEnumeration.CMSContentType.Assets))
            {
                AssetManagement.AssetManagementService assetManagementService =
                    new AssetManagement.AssetManagementService();

                Ektron.ASM.AssetConfig.AssetData assetData =
                    assetManagementService.GetAssetData(destinationContent.AssetData.Id);

                try
                {
                    ekContent.CopyReplaceDocument(
                        originalContentID.ToString(),
                        originalContent.LanguageId,
                        destinationContent.FolderId,
                        true,
                        true,
                        assetData.ID,
                        destinationContentID);
                }
                catch
                {
                    success = false;
                }
            }
        }
        else
        {
            success = false;
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }

        if (success)
        {
            // Store the current content language, and update the RequestInformationRef to
            // reflect the language of the content being distributed.
            int originalLanguageID = contentAPI.RequestInformationRef.ContentLanguage;
            contentAPI.RequestInformationRef.ContentLanguage = originalContent.LanguageId;

            try
            {
                // Update metadata and taxonomy if any has been specified.
                contentAPI.CheckOutContentById(destinationContentID);

                // Update the content's teaser.
                ContentEditData contentEditData = GetContentEditDataFromExistingContent(destinationContentID);
                contentEditData.Title = originalContent.Title;
                contentEditData.Teaser = originalContent.Teaser;
                contentEditData.FileChanged = false;

                // Save the content
                contentAPI.SaveContent(contentEditData);

                // Update metadata and taxonomy if any has been specified.
                UpdateMetadataAndTaxonomy(destinationContentID, originalContent.LanguageId);

                // Commit the changes
                contentAPI.PublishContentById(
                    destinationContentID,
                    destinationContent.FolderId,
                    destinationContent.LanguageId,
                    string.Empty,
                    contentAPI.UserId,
                    string.Empty);
            }
            catch
            {
                if (contentAPI.GetContentState(destinationContentID).Status == "O")
                {
                    contentAPI.UndoCheckoutById(destinationContentID);
                }
            }
            finally
            {
                contentAPI.RequestInformationRef.ContentLanguage = originalLanguageID;
            }

            try
            {
                // If distribution has been successful thus far
                // create any necessary distribution relationships.
                if (addDistributionRelationship)
                {
                    ekContent.AddDistributionRelationship(
                        originalContentID,
                        destinationContentID,
                        originalContent.LanguageId);
                }
            }
            catch
            {
                success = false;
                DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
            }
        }

        return success;
    }

    /// <summary>
    /// Updates metadata and taxonomy (with data stored in session) for specified content.
    /// </summary>
    /// <param name="destinationContentID">ID of content to update</param>
    /// <param name="languageID">Content language</param>
    private void UpdateMetadataAndTaxonomy(long destinationContentID, int languageID)
    {

        // Update metadata
        List<ContentMetaData> metadataList = new List<ContentMetaData>();
        metadataList.AddRange(GetContentMetadataListByLanguageId(languageID));
        if (metadataList.Count > 0)
        {
            contentAPI.UpdateContentMetaData(
                destinationContentID,
                metadataList);
        }

        // Update taxonomy
        string taxonomyIDList = ConvertCollectionToString<long>(GetTaxonomyCategoryIDList());
        if (!String.IsNullOrEmpty(taxonomyIDList))
        {
            TaxonomyContentRequest taxonomyContentRequest = new TaxonomyContentRequest();
            taxonomyContentRequest.ContentId = destinationContentID;
            taxonomyContentRequest.TaxonomyList = taxonomyIDList;
            ekContent.AddTaxonomyItem(taxonomyContentRequest);
        }
    }

    /// <summary>
    /// Parses the taxonomy category data stored in the session
    /// and returns a list of IDs.
    /// </summary>
    /// <returns>List of category IDs</returns>
    private IEnumerable<long> GetTaxonomyCategoryIDList()
    {
        IEnumerable<long> taxonomyCategoryIDs = null;
        if (Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] != null)
        {
            taxonomyCategoryIDs = (IEnumerable<long>)Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY];
        }
        else
        {
            taxonomyCategoryIDs = new List<long>();
        }

        return taxonomyCategoryIDs;
    }

    /// <summary>
    /// Returns a list of ContentMetaData populated from hashtable stored in session.
    /// </summary>
    /// <param name="languageID">Language ID associated with this metadata</param>
    /// <returns>List of ContentMetaData</returns>
    private IEnumerable<ContentMetaData> GetContentMetadataListByLanguageId(int languageID)
    {
        List<ContentMetaData> contentMetadataList = new List<ContentMetaData>();
        if (Session[DistributionWizardConstants.SESSION_PARAM_METADATA] != null)
        {
            Hashtable metadataInfo = (Hashtable)Session[DistributionWizardConstants.SESSION_PARAM_METADATA];

            Ektron.Cms.API.Metadata metadataAPI = new Ektron.Cms.API.Metadata();
            foreach (object key in metadataInfo.Keys)
            {
                object[] rawMetadata = (object[])metadataInfo[key];

                ContentMetaData contentMetadata = metadataAPI.GetMetadataType(Convert.ToInt64(rawMetadata[0]));
                if (contentMetadata != null)
                {
                    if (contentMetadata.Editable)
                    {
                        contentMetadata.Language = languageID;
                        contentMetadata.Text = rawMetadata[2].ToString().Trim();
                    }
                }
                else
                {
                    contentMetadata = new ContentMetaData();
                }

                contentMetadataList.Add(contentMetadata);
            }
        }

        return contentMetadataList;
    }

    /// <summary>
    /// Returns a ContentEditData object populated with the data of an existing
    /// piece of content.
    /// </summary>
    /// <param name="contentID">ID of content with which to populate ContentEditData object</param>
    /// <returns>ContentEditData object populated with the data of an existing piece of content</returns>
    private ContentEditData GetContentEditDataFromExistingContent(long contentID)
    {
        ContentData existingContent = contentAPI.GetContentById(contentID, ContentAPI.ContentResultType.Published);

        ContentEditData contentEditData = new ContentEditData();
        if (existingContent != null)
        {
            #region Populate contentEditdata
            contentEditData.ApprovalMethod = existingContent.ApprovalMethod;
            contentEditData.Approver = existingContent.Approver;
            contentEditData.AssetData = existingContent.AssetData;
            contentEditData.Comment = existingContent.Comment;
            contentEditData.ContType = existingContent.ContType;
            contentEditData.DateCreated = existingContent.DateCreated;
            contentEditData.DisplayDateCreated = existingContent.DisplayDateCreated;
            contentEditData.DisplayEndDate = existingContent.DisplayEndDate;
            contentEditData.DisplayGoLive = existingContent.DisplayGoLive;
            contentEditData.DisplayLastEditDate = existingContent.DisplayLastEditDate;
            contentEditData.EditorFirstName = existingContent.EditorFirstName;
            contentEditData.EditorLastName = existingContent.EditorLastName;
            contentEditData.EndDate = existingContent.EndDate;
            contentEditData.EndDateAction = existingContent.EndDateAction;
            contentEditData.FlagDefId = existingContent.FlagDefId;
            contentEditData.FolderId = existingContent.FolderId;
            contentEditData.FolderName = existingContent.FolderName;
            contentEditData.GoLive = existingContent.GoLive;
            contentEditData.HistoryId = existingContent.HistoryId;
            contentEditData.Html = existingContent.Html;
            contentEditData.HyperLink = existingContent.HyperLink;
            contentEditData.Id = existingContent.Id;
            contentEditData.Image = existingContent.Image;
            contentEditData.ImageThumbnail = existingContent.ImageThumbnail;
            contentEditData.InheritedFrom = existingContent.InheritedFrom;
            contentEditData.IsInherited = existingContent.IsInherited;
            contentEditData.IsMetaComplete = existingContent.IsMetaComplete;
            contentEditData.IsPrivate = existingContent.IsPrivate;
            contentEditData.IsPublished = existingContent.IsPublished;
            contentEditData.IsSearchable = existingContent.IsSearchable;
            contentEditData.IsXmlInherited = existingContent.IsXmlInherited;
            contentEditData.LanguageDescription = existingContent.LanguageDescription;
            contentEditData.LanguageId = existingContent.LanguageId;
            contentEditData.LastEditDate = existingContent.LastEditDate;
            contentEditData.LegacyData = existingContent.LegacyData;
            contentEditData.ManualAlias = existingContent.ManualAlias;
            contentEditData.ManualAliasId = existingContent.ManualAliasId;
            contentEditData.MediaText = existingContent.MediaText;
            contentEditData.MetaData = existingContent.MetaData;
            contentEditData.Path = existingContent.Path;
            contentEditData.Permissions = existingContent.Permissions;
            contentEditData.Quicklink = existingContent.Quicklink;
            contentEditData.Status = existingContent.Status;
            contentEditData.StyleSheet = existingContent.StyleSheet;
            contentEditData.Teaser = existingContent.Teaser;
            contentEditData.TemplateConfiguration = existingContent.TemplateConfiguration;
            contentEditData.Text = existingContent.Text;
            contentEditData.Title = existingContent.Title;
            contentEditData.Type = existingContent.Type;
            contentEditData.Updates = existingContent.Updates;
            contentEditData.UserId = existingContent.UserId;
            contentEditData.XmlConfiguration = existingContent.XmlConfiguration;
            contentEditData.XmlInheritedFrom = existingContent.XmlInheritedFrom;
            #endregion
        }

        return contentEditData;
    }

    /// <summary>
    /// Converts a list to a comma delimited string.
    /// (TaxonomyRequest objects require taxonomy ID lists as strings.)
    /// </summary>
    /// <param name="list">Strongly typed collection</param>
    /// <returns>Comma delimited string</returns>
    private string ConvertCollectionToString<T>(IEnumerable<T> list)
    {
        string retVal = string.Empty;

        List<T> collection = new List<T>(list);
        foreach (T item in collection)
        {
            if (String.IsNullOrEmpty(retVal))
            {
                retVal = item.ToString();
            }
            else
            {
                retVal += String.Format(", {0}", item.ToString());
            }
        }

        return retVal;
    }

    /// <summary>
    /// Displays the specifed error message
    /// </summary>
    /// <param name="errorMessage">Message to display</param>
    private void DisplayErrorMessage(string errorMessage)
    {
        string errorUrl = String.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardError.aspx?Error={0}&Mode={1}",
            EkFunctions.UrlEncode(errorMessage),
            DistributionWizardHelperMethods.GetQueryStringFromMode(mode));

        Response.Redirect(errorUrl, true);
    }

    /// <summary>
    /// Register stylesheets for this page with the master.
    /// </summary>
    private void RegisterStylesheets()
    {
        HtmlLink linkStylesheet = new HtmlLink();
        linkStylesheet.Attributes.Add("type", "text/css");
        linkStylesheet.Attributes.Add("rel", "Stylesheet");
        linkStylesheet.Href = "DistributionWizardConfirm.css";

        Page.Master.Page.Header.Controls.Add(linkStylesheet);
    }

    #region Properties - Public
    public string DistributionModeString
    {
        get
        {
            return DistributionWizardHelperMethods.GetQueryStringFromMode(mode);
        }
    }
    #endregion
}