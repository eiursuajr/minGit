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
using Ektron.Cms.Common;

public partial class Community_DistributionWizard_DistributionWizardError : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (Community_DistributionWizard_DistributionWizard masterPage =
            (Community_DistributionWizard_DistributionWizard)Page.Master)
        {
            masterPage.WizardStepHeader = "Distribution Error";
            masterPage.WizardStepTitle = "An error has occurred:";
        }

        string errorMessage = Request.QueryString["Error"];
        if (!String.IsNullOrEmpty(errorMessage))
        {
            lblErrorMessage.Text = EkFunctions.HtmlEncode(Server.UrlDecode(errorMessage));
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {        
        Response.Redirect(
            "~/Workarea/Community/DistributionWizard/DistributionWizardClose.aspx?mode=" + Request.QueryString["mode"],
            false);
    }
}
