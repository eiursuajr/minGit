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

public partial class Community_DistributionWizard_DistributionWizard : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string WizardStepHeader
    {
        get
        {
            return ltrWizardStepHeader.Text;
        }
        set
        {
            ltrWizardStepHeader.Text = value;
        }
    }

    public string WizardStepTitle
    {
        get
        {
            return ltrWizardStepTitle.Text;
        }

        set
        {
            ltrWizardStepTitle.Text = value;
        }
    }
}
