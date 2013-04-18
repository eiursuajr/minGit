using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Community_DistributionWizard_DistributionWizardClose : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new ContentAPI();

        // The action parameter is really only relevent to sharepoint
        // distribution scenarios. (Community distribution does not
        // require any special revert functionality because nothing
        // is committed until the user clicks 'Done'.)
        string actionParam = Request.QueryString["action"];

        bool revertAction = false;
        if (!String.IsNullOrEmpty(actionParam))
        {
            if (actionParam.ToLower() == "cancel")
            {
                revertAction = true;
            }
        }

        DistributionWizardEnumerations.Mode distributionMode = DistributionWizardHelperMethods.GetModeFromQueryString(Request);

        switch (distributionMode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
            case DistributionWizardEnumerations.Mode.CommunityReplace:
            case DistributionWizardEnumerations.Mode.None:
                Page.ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    "CloseThickboxScript",
                    "<script type=\"text/javascript\" language=\"javascript\">self.parent.ektb_remove();</script>");            
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                // In Sharepoint distribution scenarios, the file is uploaded
                // prior to applying metadata/taxonomy and creating the distribution
                // relationship. Therefore, if the action is canceled after folder selection
                // we must "undo" the upload -- either delete the copied file (initial distribution)
                // or revert to an older version (redistribution).
                if (revertAction)
                {
                    RollbackSharepointDistribution(distributionMode);
                }

                // Add window close script to page.
                Page.ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    "ClosePopupScript",
                    "<script type=\"text/javascript\" language=\"javascript\">window.close();</script>");
                break;
        }

        // Clear out any leftover session data
        ClearSessionData();
    }

    /// <summary>
    /// In Sharepoint distribution scenarios, the upload of the file must be reverted
    /// if the user cancels the process. 
    /// 
    /// Sharepoint Copy: Delete the content that was copied from sharepoint.
    /// Sharepoint Redistribution: Revert to previous published version.
    /// Other modes: No action is necessary.
    /// </summary>
    /// <param name="mode">Distribution mode</param>
    private void RollbackSharepointDistribution(DistributionWizardEnumerations.Mode mode)
    {
        long contentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID];

        ContentData content = contentAPI.GetContentById(contentID, ContentAPI.ContentResultType.Published);
        if (content != null)
        {
            if (mode == DistributionWizardEnumerations.Mode.SharepointRedistribute)
            {
                // Rollback to older version.
                ContentHistoryData previousVersion = GetPreviousPublishedVersion(contentID);

                if (previousVersion != null)
                {
                    int origLanguageID = contentAPI.EkContentRef.RequestInformation.ContentLanguage;
                    contentAPI.EkContentRef.RequestInformation.ContentLanguage = content.LanguageId;

                    try
                    {
                        contentAPI.RestoreHistoryContent(previousVersion.Id);
                        contentAPI.EkContentRef.SubmitForPublicationv2_0(content.Id, content.FolderId, string.Empty);
                    }
                    catch
                    {
                        DisplayErrorMessage(
                            mode,
                            contentAPI.EkMsgRef.GetMessage("distribution wizard restore error"));
                    }
                    finally
                    {
                        contentAPI.EkContentRef.RequestInformation.ContentLanguage = origLanguageID;
                    }
                }
            }
            else
            {
                try
                {
                    contentAPI.DeleteContentItemById(contentID);
                }
                catch
                {
                    DisplayErrorMessage(
                        mode,
                        contentAPI.EkMsgRef.GetMessage("distribution wizard restore error"));    
                }
            }
        }
    }

    /// <summary>
    /// Returns the ContentHistoryData for the published version of the specified content 
    /// previous to the current version.
    /// </summary>
    /// <param name="contentID">ID of content</param>
    /// <returns>ContentHistoryData for the published version of the specified content 
    /// previous to the current version</returns>
    private ContentHistoryData GetPreviousPublishedVersion(long contentID)
    {
        ContentHistoryData[] contentHistory = contentAPI.GetHistoryList(contentID);

        ContentHistoryData previousVersion = null;
        for (int i = contentHistory.Length - 2; i >= 0 && previousVersion == null; i--)
        {
            if (contentHistory[i].Status == "A")
            {
                previousVersion = contentHistory[i];
            }
        }

        return previousVersion;
    }

    /// <summary>
    /// Clears distribution related session data.
    /// </summary>
    private void ClearSessionData()
    {   
        Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_METADATA] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_CONTENT_ID] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_SHAREPOINT_GUID] = null;
        Session[DistributionWizardConstants.SESSION_PARAM_TAXONOMY] = null;
    }

    /// <summary>
    /// Displays the specifed error message
    /// </summary>
    /// <param name="errorMessage">Message to display</param>
    private void DisplayErrorMessage(DistributionWizardEnumerations.Mode mode, string errorMessage)
    {
        string errorUrl = String.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardError.aspx?Error={0}&Mode={1}",
            EkFunctions.UrlEncode(errorMessage),
            DistributionWizardHelperMethods.GetQueryStringFromMode(mode));

        Response.Redirect(errorUrl, true);
    }
}
