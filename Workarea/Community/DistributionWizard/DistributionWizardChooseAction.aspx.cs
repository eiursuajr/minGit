using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

public partial class Community_DistributionWizard_DistributionWizardChooseAction : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private EkMessageHelper messageHelper = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new ContentAPI();
        messageHelper = contentAPI.EkMsgRef;

        RegisterStylesheets();

        if (ParseCommunityParameters())
        {
            Community_DistributionWizard_DistributionWizard masterPage = (Community_DistributionWizard_DistributionWizard)Page.Master;

            long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];

            long destinationContentID = GetDestinationContentID(originalContentID);
            if (destinationContentID > 0)
            {
                // This content is already in a distribution relationship.
                // Display UI for redistribution mode.

                masterPage.WizardStepHeader = "Step 1";
                masterPage.WizardStepTitle = "This item has a distribution relationship:";

                // Display redistribution details
                try
                {
                    ltrRedistributionDetails.Text = GetRedistributionDetails(destinationContentID);
                }
                catch
                {
                    DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
                }

                divRedistributionOptions.Visible = true;
                divDistributionOptions.Visible = false;

                Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] = destinationContentID;
            }
            else
            {
                // This content is not yet in a distribution relationship.
                // Display the UI for distribution mode.

                masterPage.WizardStepHeader = "Step 1";
                masterPage.WizardStepTitle = "Choose one:";

                divDistributionOptions.Visible = true;
                divRedistributionOptions.Visible = false;
            }
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"));
        }
    }

    protected void lbCopy_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardChooseFolder.aspx?OriginalContentID={0}",
            Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID].ToString()));
    }

    protected void lbReplace_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardChooseContent.aspx?OriginalContentID={0}",
            Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID].ToString()));
    }

    protected void lbRedistribute_Click(object sender, EventArgs e)
    {
        Response.Redirect(
            "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?Mode=" + DistributionWizardConstants.QUERY_MODE_COMMUNITYREDISTRIBUTE,
            false);
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx",
            false);
    }

    /// <summary>
    /// Determine if a distribution relation exists for the specified source content.
    /// </summary>
    /// <param name="sourceContentID">Source content ID</param>
    /// <returns>True if a distribution relationship exists for the specified content, false otherwise.</returns>
    private long GetDestinationContentID(long sourceContentID)
    {
        long destinationContentID = -1;
        try
        {
            destinationContentID = contentAPI.GetDestinationContentId(sourceContentID);
        }
        catch
        {
            // Suppress any error
        }

        return destinationContentID;
    }

    /// <summary>
    /// Returns the markup to display the redistribution details.
    /// </summary>
    /// <param name="destinationContentID">Destination content ID</param>
    /// <returns>Markup to display the redistribution details</returns>
    private string GetRedistributionDetails(long destinationContentID)
    {
        Ektron.Cms.API.Content.Content contentAPI = new Ektron.Cms.API.Content.Content();
        ContentData destinationContent = contentAPI.GetContent(
            destinationContentID, 
            ContentAPI.ContentResultType.Published);

        string destinationContentTitle = destinationContent.Title;
        string destinationFolderPath = contentAPI.EkContentRef.GetFolderPath(destinationContent.FolderId);

        StringBuilder sbDetails = new StringBuilder();
        sbDetails.Append("<ul><li>");
        sbDetails.Append("It was previously distributed to the following location:");
        sbDetails.Append("<ul><li>");
        sbDetails.Append(destinationFolderPath);
        if (destinationFolderPath.LastIndexOf("\\") != destinationFolderPath.Length - 1)
        {
            sbDetails.Append("\\");
        }
        sbDetails.Append(destinationContentTitle);
        sbDetails.Append("</li></ul></li></ul>");
        sbDetails.Append("<div id=\"DistributionWizardRedistributionInformation\">You cannot currently distribute this document to a different folder. If you want to do that, first go to ");
        sbDetails.Append(destinationFolderPath);
        sbDetails.Append(" and delete the document from there.</div>");

        return sbDetails.ToString();
    }

    /// <summary>
    /// Parses and stores community parameters.
    /// </summary>
    /// <returns>True if specified parameters are valid, false otherwise.</returns>
    private bool ParseCommunityParameters()
    {
        bool isValid = false;

        if (!String.IsNullOrEmpty(Request.QueryString["OriginalContentID"]))
        {
            long contentID = -1;
            if (long.TryParse(Request.QueryString["OriginalContentID"], out contentID))
            {
                if (contentID > 0)
                {
                    isValid = true;
                    Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] = contentID;
                }
            }
        }

        return isValid;
    }

    /// <summary>
    /// Redirects to the error page displaying the specified error message.
    /// </summary>
    /// <param name="errorMessage">Message to display</param>
    private void DisplayErrorMessage(string errorMessage)
    {
            Response.Redirect(String.Format(
                "~/Workarea/Community/DistributionWizard/DistributionWizardError.aspx?Error={0}",
                EkFunctions.UrlEncode(errorMessage)));
    }

    /// <summary>
    /// Register corresponding stylesheets with the master page.
    /// </summary>
    private void RegisterStylesheets()
    {
        HtmlLink linkStylesheet = new HtmlLink();
        linkStylesheet.Attributes.Add("type", "text/css");
        linkStylesheet.Attributes.Add("rel", "Stylesheet");
        linkStylesheet.Href = "DistributionWizardChooseAction.css";

        Page.Master.Page.Header.Controls.Add(linkStylesheet);
    }

}
