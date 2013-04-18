using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Workarea_helpmessage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("Web config ek_helpDomainPrefix setting : ");
        if (!string.IsNullOrEmpty(Request.QueryString["error"]) && Request.QueryString["error"].ToLower() == "isfile")
        {
            sb.Append(!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ek_helpDomainPrefix"]) ? System.Configuration.ConfigurationManager.AppSettings["ek_helpDomainPrefix"] : "Value not specified");
            sb.Append("<br />");
            sb.Append("<b>Please Install the Help files or change key value of \"ek_helpDomainPrefix\" in the web.config to \"http://documentation.ektron.com/cms400/v[ek_cmsversion]/webhelp\"</b>");
        }
        else
        {
            sb.Append(System.Configuration.ConfigurationManager.AppSettings["ek_helpDomainPrefix"]);
            sb.Append("<br />");
            sb.Append("<b>Not a Valid URL, value of \"ek_helpDomainPrefix\" should be a valid URL</b>");
        }
        ltrMessage.Text = sb.ToString();
    }
}