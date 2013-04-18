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

public partial class Controls_Default : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string ConfigRoot
    {
        get
        {
            if (ViewState["ConfigRoot"] == null)
                return "";

            return (string)ViewState["ConfigRoot"];
        }
        set
        {
            ViewState["ConfigRoot"] = value;
        }
    }
}
