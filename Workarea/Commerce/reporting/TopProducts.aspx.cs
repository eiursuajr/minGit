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

public partial class Commerce_reporting_TopProducts : System.Web.UI.Page, IWidgetHost
{
    protected void Page_Load(object sender, EventArgs e) {
        RegisterResource();
        BuildToolBar();
        if(Edit != null)
           Edit += new EditDelegate(DoEdit);
		if(Minimize != null)
           Minimize += new MinimizeDelegate(DoMinimize);
		if(Maximize != null)
           Maximize += new MaximizeDelegate(DoMaximize);
		if(Load != null)
           Load += new LoadDelegate(DoLoad);
		if(Create != null)
           Create += new CreateDelegate(DoCreate);
		if(Close != null)
           Close += new CloseDelegate(DoClose);
    }

    protected void BuildToolBar()
    {
        string helpScreenAlias = "topproducts";
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        StyleHelper m_refStyle = new StyleHelper();
        Ektron.Cms.ContentAPI m_refContentAPI = new ContentAPI();
        Ektron.Cms.Common.EkMessageHelper m_refMsg = new EkMessageHelper(m_refContentAPI.RequestInformationRef);
		
		Utilities.ValidateUserLogin();
		
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl top products"));
        ltrNumericVal.Text = m_refMsg.GetMessage("js alert valid numeric");
        //result.Append("<table cellspacing=\"0\"><tbody><tr><td>");
        result.Append("<div style=\"height:22px\">");
        result.Append(m_refStyle.GetHelpButton(helpScreenAlias, "center"));
        result.Append("</div>");
        //result.Append("</td></tr></tbody></table>");
        divToolBar.InnerHtml = result.ToString();

        StyleSheetJS.Text = m_refStyle.GetClientScript();
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

    public new string Title
    {
        get { return String.Empty; }
        set { litTitle.Text = value; }
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

    protected void RegisterResource() {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.CommonApi common = new CommonApi();
        Ektron.Cms.API.Css.RegisterCss(this, common.ApplicationPath + "explorer/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
