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
using Ektron.Cms.Content;

public partial class Community_DistributionWizard_DistributionWizardChooseContent : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private EkMessageHelper messageHelper = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 2";
            masterPage.WizardStepTitle = "Select item to replace:";
        }

        contentAPI = new ContentAPI();
        messageHelper = contentAPI.EkMsgRef;

        if (ParseParameters())
        {
            InitializeSelectContent();
        }
        else
        {
            DisplayErrorMessage(
                messageHelper.GetMessage("distribution wizard required parameters"),
                true);
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        if (scSelectContent.SelectedContentID > 0)
        {
            // If relationship already exists, alert the user.
            if (contentAPI.GetSourceContentId(scSelectContent.SelectedContentID) != 0)
            {
                DisplayErrorMessage(
                    messageHelper.GetMessage("distribution wizard relationship exists"),
                    false);
            }
            else
            {
                Session[DistributionWizardConstants.SESSION_PARAM_DEST_CONTENT_ID] = scSelectContent.SelectedContentID;
                Response.Redirect(
                    "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?Mode=" + DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE,
                    false);
            }
        }
        else
        {
            DisplayErrorMessage(
                messageHelper.GetMessage("distribution wizard select content"),
                false);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        string previousWizardStepUrl = String.Format(
            "~/Workarea/Community/DistributionWizard/DistributionWizardChooseAction.aspx?OriginalContentID={0}",
            Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID].ToString());

        Response.Redirect(previousWizardStepUrl, false);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx",
            false);
    }

    private void InitializeSelectContent()
    {
        ContentData originalContent = null;
        try
        {
            originalContent = contentAPI.GetContentById(
                 (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID],
                 ContentAPI.ContentResultType.Published);
        }
        catch
        {
            
        }

        if (originalContent != null)
        {
            scSelectContent.AssetExtension = originalContent.AssetData.FileExtension;
            scSelectContent.ContentTypeID = originalContent.ContType;
            scSelectContent.LanguageID = originalContent.LanguageId;
            scSelectContent.ShowRelatedContent = false;
        }
        else
        {
            DisplayErrorMessage(messageHelper.GetMessage("distribution wizard required parameters"), true);
        }
    }

    private Boolean ParseParameters()
    {
        bool retVal = true;

        if (Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] == null)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["OriginalContentID"]))
            {
                long originalContentID = -1;
                if (long.TryParse(Request.QueryString["OriginalContentID"], out originalContentID))
                {
                    if (contentAPI.GetContentById(originalContentID, ContentAPI.ContentResultType.Published) != null)
                    {
                        Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] = originalContentID;
                    }
                    else
                    {
                        retVal = false;
                    }
                }
                else
                {
                    retVal = false;
                }
            }
            else
            {
                retVal = false;
            }
        }

        return retVal;
    }


    private void DisplayErrorMessage(string errorMessage, bool isFatalError)
    {
        if (isFatalError)
        {
            Response.Redirect(String.Format(
                "~/Workarea/Community/DistributionWizard/DistributionWizardError.aspx?Error={0}",
                EkFunctions.UrlEncode(errorMessage)));
        }
        else
        {
            lblErrorMessage.Text = EkFunctions.HtmlEncode(errorMessage);
        }
    }
}
