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

public partial class Community_DistributionWizard_DistributionWizardChooseFolder : System.Web.UI.Page
{
    private ContentAPI contentAPI = null;
    private SiteAPI siteAPI = null;
    private EkMessageHelper messageHelper = null;
    

    protected void Page_Load(object sender, EventArgs e)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Step 2";
            masterPage.WizardStepTitle = "Choose a destination for the copy:";
        }

        RegisterStylesheets();

        contentAPI = new ContentAPI();
        siteAPI = new SiteAPI();
        messageHelper = contentAPI.EkMsgRef;

        sfSelectDestinationFolder.CallbackFunc = "selectFolder";
        sfSelectDestinationFolder.SitePath = siteAPI.SitePath;
        sfSelectDestinationFolder.FolderID = 0;
        sfSelectDestinationFolder.CheckAddPermissions = true;

        if (!ParseParameters())
        {
            DisplayErrorMessage(
                messageHelper.GetMessage("distribution wizard required parameters"),
                true);

        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID] != null)
        {
            long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];
            Response.Redirect(String.Format(
                "~/Workarea/Community/DistributionWizard/DistributionWizardChooseAction.aspx?OriginalContentID={0}",
                originalContentID.ToString()));
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        long selectedFolderID = -1;
        long.TryParse(inputSelectedFolderID.Value, out selectedFolderID);

        if (selectedFolderID > -1)
        {
            Session[DistributionWizardConstants.SESSION_PARAM_DEST_FOLDER_ID] = selectedFolderID;

            long originalContentID = (long)Session[DistributionWizardConstants.SESSION_PARAM_ORIG_CONTENT_ID];

            Response.Redirect(String.Format(
                "~/Workarea/Community/DistributionWizard/DistributionWizardMetaTax.aspx?Mode={0}&OriginalContentID={1}&DestFolderID={2}",
                DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY,
                originalContentID.ToString(),
                selectedFolderID.ToString()));
        }
        else
        {
            DisplayErrorMessage(
                messageHelper.GetMessage("distribution wizard select folder"),
                false);
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx",
            false);
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

    private void RegisterStylesheets()
    {
        HtmlLink linkStylesheet = new HtmlLink();
        linkStylesheet.Attributes.Add("type", "text/css");
        linkStylesheet.Attributes.Add("rel", "Stylesheet");
        linkStylesheet.Href = "DistributionWizardChooseFolder.css";

        Page.Master.Page.Header.Controls.Add(linkStylesheet);
    }

    public string SelectedFolderIDHiddenFieldName
    {
        get
        {
            return inputSelectedFolderID.ClientID;
        }
    }
}
