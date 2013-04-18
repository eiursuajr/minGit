using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;
using Ektron.Cms.Workarea;

public partial class Localization_CreateReport : workareabase, IWidgetHost
{
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        RegisterResource();
        BuildToolBar();
        Edit += new EditDelegate(DoEdit);
        Minimize += new MinimizeDelegate(DoMinimize);
        Maximize += new MaximizeDelegate(DoMaximize);
        Load += new LoadDelegate(DoLoad);
        Create += new CreateDelegate(DoCreate);
        Close += new CloseDelegate(DoClose);
    }

    protected void BuildToolBar()
    {
        this.SetTitleBarToMessage("lbl localization dashboard");
        this.AddHelpButton("Loc_dashboard");
    }

    public void DoEdit(string settings) { }
    public void DoMinimize() { }
    public void DoMaximize() { }
    public void DoLoad() { }
    public void DoCreate() { }
    public void DoClose() { }

    public void Test_Click(object sender, EventArgs e) { }
    public string GetSetting(string key) { return string.Empty; }

    public WidgetData WidgetInfo
    {
        get { return null; }
    }

    

    public bool IsEditable
    {
        get { return false; }
    }

    public Expandable ExpandOptions
    {
        get { return Expandable.DontExpand; }
        set { }
    }

    public string HelpFile
    {
        get { return String.Empty; }
        set { }
    }

    public void Save(string settings) { }
    public void Save(string key, string value) { }
    public void Delete() { }
    public void SaveWidgetDataMembers() { }
    public void LoadWidgetDataMembers() { }

    public event EditDelegate Edit;
    public event MinimizeDelegate Minimize;
    public event MaximizeDelegate Maximize;
    public new event LoadDelegate Load;
    public event CreateDelegate Create;
    public event CloseDelegate Close;

    protected void RegisterResource()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.CommonApi common = new CommonApi();
        Ektron.Cms.API.Css.RegisterCss(this, common.ApplicationPath + "explorer/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
