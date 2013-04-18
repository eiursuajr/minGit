using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms.Widget;
using Ektron.Cms;

public partial class widgets_Calculator : System.Web.UI.UserControl, IWidget
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IWidgetHost _host = WidgetHost.GetHost(this);
        _host.Title = "Calculator";
        _host.Minimize += new MinimizeDelegate(delegate() { });
        _host.Maximize += new MaximizeDelegate(delegate() { });

        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "calculator-widget", Page.ResolveClientUrl(AppRelativeTemplateSourceDirectory) + "Calculator/js/Calculator.js");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), ClientID, "new Calculator('" + ClientID + "');", true);
    }
}
