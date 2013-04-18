using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Configuration;


public partial class widgets_HelloWorld : System.Web.UI.UserControl, IWidget
{
    #region properties
    private string _HelloString;
   [WidgetDataMember("Hello World")]
    public string HelloString { get { return _HelloString; } set { _HelloString = value; } }
    #endregion

    IWidgetHost _host;
   
    protected void Page_Init(object sender, EventArgs e)
    {   
        string sitepath = new CommonApi().SitePath;
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronModalJS);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Hello World Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        string myPath = string.Empty;
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ek_helpDomainPrefix"]))
        {
            string helpDomain = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
            if ((helpDomain.IndexOf("[ek_cmsversion]") > 1))
            {
                myPath = helpDomain.Replace("[ek_cmsversion]", new CommonApi().RequestInformationRef.Version);
            }
            else
            {
                myPath = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
            }
        }
        else
        {
            myPath = sitepath + "Workarea/help";
        }
        _host.HelpFile = myPath + "/Widget Chapter/Hello World/Creating the Hello World Widget.htm";
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    { 
        HelloTextBox.Text = HelloString;
        ViewSet.SetActiveView(Edit);   
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        HelloString = HelloTextBox.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        OutputLabel.Text = HelloString;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

}

