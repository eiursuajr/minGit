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

public partial class Community_DistributionWizard_SelectFolder : System.Web.UI.UserControl
{
    public string CallbackFunc = "selectFolder";
    public string SitePath = "http://localhost/cms400min";
    public long FolderID = 0;
    public bool CheckAddPermissions = false;
    public bool ShowSpecialFolders = false;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
