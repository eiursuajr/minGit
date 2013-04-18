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
using Ektron.Cms.API;
using Ektron.Cms;

public partial class pagetree : System.Web.UI.UserControl
{
    private string _callback = "alert";
    public string Callback
    {
        get { return _callback; }
        set { _callback = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ContentAPI capi = new ContentAPI();
        
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronTreeviewJS);
        JS.RegisterJSInclude(this, capi.SitePath + "widgets/MultivariateExperiment/controls/pagetree.js", "PageTreeJS");

        Css.RegisterCss(this, capi.SitePath + "widgets/MultivariateExperiment/controls/treeview.css", "PageTreeCSS");

        string handlerPath = capi.SitePath + "widgets/MultivariateExperiment/controls/pagetree.ashx";
        JS.RegisterJSBlock(this,String.Format("Ektron.PageTree.Init(\"{0}\", \"{1}\", {2});", root.ClientID, handlerPath, _callback), root.ClientID + "JS");
    }
}
