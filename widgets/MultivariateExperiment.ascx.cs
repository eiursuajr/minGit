using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget.Multivariate;
using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using Ektron.Cms.Widget;
using Ektron.Cms.API;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.Framework.UI;

[JsonObject]
public class CombinationJSON
{
    [JsonProperty("guid")]
    public Guid Guid;

    [JsonProperty("variantGuids")]
    public List<Guid> VariantGuids;
}

public partial class widgets_MultivariateExperiment : System.Web.UI.UserControl, IMultivariateExperimentView
{
    protected ContentAPI _capi;
    public static class Info
    {
        private static ContentAPI capi = new ContentAPI();

        public static string SitePath {
            get { return capi.SitePath; }
        }

        public static string JSPath
        {
            get { return capi.SitePath + "widgets/MultivariateExperiment/js/MultivariateExperiment.js"; }
        }

        public static string UIDialogJSPath
        {
            get { return capi.SitePath + "widgets/MultivariateExperiment/js/ui.dialog.js"; }
        }

        public static string ConfirmJSPath
        {
            get { return capi.SitePath + "Workarea/java/plugins/confirm/ektron.confirm.js"; }
        }

        public static string CSSPath
        {
            get { return capi.SitePath + "widgets/MultivariateExperiment/css/MultivariateExperiment.css"; }
        }

        public static string IECSSPath
        {
            get { return capi.SitePath + "widgets/MultivariateExperiment/css/MultivariateExperiment.ie.css"; }
        }

        public static string JSID
        {
            get { return "JSWidgetMultivariateExperiment"; }
        }

        public static string UIDialogJSID
        {
            get { return "UIDialogJSWidgetMultivariateExperiment"; }
        }

        public static string CSSID
        {
            get { return "CSSWidgetMultivariateExperiment"; }
        }

        public static string IECSSID
        {
            get { return "IECSSWidgetMultivariateExperiment"; }
        }

        public static string UIThemeCSSID
        {
            get { return "UIThemeCSS"; }
        }

        public static string ConfirmJSID
        {
            get { return "UIThemeJS"; }
        }
    }

    private MultivariateExperimentController _controller;
    private IWidgetHost _host;

    private bool _active;

    public long PageID
    {
        get
        {
            return (Page as PageBuilder).Pagedata.pageID;
        }
    }

    public Ektron.Cms.PageBuilder.WidgetData WidgetData
    {
        get
        {
            Ektron.Cms.PageBuilder.WidgetData wd = new Ektron.Cms.PageBuilder.WidgetData();
            wd.ID = _host.WidgetInfo.ID;
            wd.Minimized = _host.WidgetInfo.Minimized;
            wd.Order = _host.WidgetInfo.Order;
            wd.ColumnID = _host.WidgetInfo.ColumnID;
            wd.ControlURL = _host.WidgetInfo.ControlURL;
            wd.ColumnGuid = (_host as Ektron.Cms.PageBuilder.WidgetHost).ColumnGuid;
            wd.DropID = (_host as Ektron.Cms.PageBuilder.WidgetHost).ZoneID;
            return wd;
        }
    }


    private long _storedTargetID;
    [WidgetDataMember(0)]
    public long StoredTargetID
    {
        get { return _storedTargetID; }
        set { _storedTargetID = value; }
    }


    public long TargetPageID
    {
        set
        {
            tbTargetPageID.Text = value.ToString();
        }

        get
        {
            try
            {
                return long.Parse(tbTargetPageID.Text);
            }
            catch
            {
                return 0;
            }
        }
    }

    public int MaxConversions
    {
        set
        {
            txtMaxConversions.Text = tbMaxConversions.Text = value.ToString();
        }

        get
        {
            return int.Parse(tbMaxConversions.Text);
        }
    }

    public bool Active
    {
        get
        {
            return _active;
        }

        set
        {
            pnlLoggedIn.Visible = btnStart.Visible = tbTargetPageID.Enabled = !value && Shared.IsABTester;
            pnlAnon.Visible = !pnlLoggedIn.Visible;
            _active = value;
            btnStop.Visible = value && Shared.IsABTester;
            divContent.Visible = (value && 
                ((Page as PageBuilder).Status != Mode.Preview &&
                   (Page as PageBuilder).Status != Mode.AnonViewing));
            phPageTree.Visible = !btnStop.Visible;
        }
    }

    public string ErrorText
    {
        get { return divError.InnerHtml; }
        set { divError.InnerHtml = value; divError.Visible = true; }
    }

    int previousPageVariations
    {
        get
        {
            int ret = 0;
            if (Session["vs_previousPageVariations"] != null)
                ret = (int)Session["vs_previousPageVariations"];
            return ret;
        }
        set
        {
            Session["vs_previousPageVariations"] = value;
        }
    }

    // private List<MultivariateReportData> _reports;
    public List<MultivariateReportData> Reports
    {
        set
        {
            List<MultivariateReportData> reports = value;
            reports.Sort(new Comparison<MultivariateReportData>(delegate(MultivariateReportData a, MultivariateReportData b)
            {
                if (a == b) return 0;
                if (a.Hits == 0 && b.Hits == 0) return (a.ID < b.ID) ? 1 : -1;
                if (a.Hits == 0) return 1;
                if (b.Hits == 0) return -1;

                int percentA = (a.Conversions * 100 / a.Hits);
                int percentB = (b.Conversions * 100 / b.Hits);
                if (percentA < percentB) return 1;
                if (percentA > percentB) return -1;
                
                return (a.ID < b.ID) ? 1 : -1;
            }));

            repCombinationReports.DataSource = reports;
            repCombinationReports.DataBind();
        }
    }

    bool reportBTNClicked = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            experiment.InnerHtml = "Can't run experiments in a dashboard!";
            return;
        }

        _capi = new ContentAPI();
        AddPageTree();

       

        header.Visible = divContent.Visible = ((Page as PageBuilder).Status == Mode.Editing);

        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Multivariate Experiment";

        TargetPageID = StoredTargetID;
        if (StoredTargetID == 0 && !string.IsNullOrEmpty(this.Request.Form[tbTargetPageID.UniqueID]))
        {
            tbTargetPageID.Text = this.Request.Form[tbTargetPageID.UniqueID];
        }

        _controller = new MultivariateExperimentController(this);

        
    }

    private void registerJSCSS()
    {
        Packages.jQuery.jQueryUI.ThemeRoller.Register(this);
        Packages.EktronCoreJS.Register(this);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Info.JSPath, Info.JSID);
        Packages.jQuery.jQueryUI.Dialog.Register(this);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Info.ConfirmJSPath, Info.ConfirmJSID);

        Ektron.Cms.API.JS.RegisterJSBlock(this, String.Format("Ektron.Widget.MultivariateExperiment.Init('{0}');", experiment.ClientID), "EKMV" + ClientID);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"MultivariateExperiment_InitFilterKeys_" + ClientID, GetFilterKeysClientCode(), false);

        Ektron.Cms.API.Css.RegisterCss(this, Info.CSSPath, Info.CSSID);
        Ektron.Cms.API.Css.RegisterCss(this, Info.IECSSPath, Info.IECSSID, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
    }
    private string GetFilterKeysClientCode() {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<script type='text/javascript'>");
        sb.AppendLine("$ektron(document).ready(function() { ");
        sb.AppendLine("$ektron('.tbTargetPageID, .tbMaxConversions').bind('contextmenu', function(e) { return false; }).bind('keydown', function(evt) {");
        sb.AppendLine("        var evt = (evt) ? evt : window.event;");
        sb.AppendLine("        var charCode = (evt.which != null) ? evt.which : evt.keyCode;");
        sb.AppendLine("        return (charCode == 8 || charCode == 9 || charCode == 35 || charCode == 36 ");
        sb.AppendLine("            || charCode == 37 || charCode == 39 || charCode == 46");
        sb.AppendLine("            || (charCode >= 48 && charCode <= 57) ");
        sb.AppendLine("            || (charCode >= 96 && charCode <= 105));");
        sb.AppendLine("    });");
        sb.AppendLine("});");
        sb.AppendLine("</script>");

        return sb.ToString();
    }

    public void AddPageTree() {

        // to fix defect 47604, removed the following block from the ascx page. 
        // It's now added dynamically only when page is first loaded (see below):
        //
        //<div class="browse-dialog" runat="server" visible="true" title="Browse Target Page">
        //  <UX:PageTree ID="pageTree" runat="server" />
        //</div>

        System.Web.UI.HtmlControls.HtmlGenericControl divCtl = new HtmlGenericControl();
        divCtl.TagName = "div";
        divCtl.Attributes.Add("class", "browse-dialog");
        divCtl.Attributes.Add("title", "Browse Target Page");

        // add an element that we can search for (the dialog plugin wont copy additional attributes of the wrapping div-tag):
        System.Web.UI.HtmlControls.HtmlGenericControl spanCtl = new HtmlGenericControl();
        spanCtl.TagName = "span";
        spanCtl.InnerText = ".";
        spanCtl.Attributes.Add("style", "display: none;");
        spanCtl.Attributes.Add("class", experiment.ClientID);
        divCtl.Controls.Add(spanCtl);

        pagetree pt = (pagetree)LoadControl(Info.SitePath + "widgets/MultivariateExperiment/controls/pagetree.ascx");
        pt.Callback = String.Format("function(id,cb){{return Ektron.Widget.MultivariateExperiment.widgets[\"{0}\"].SelectTarget(id,cb);}}", experiment.ClientID);

        divCtl.Controls.Add(pt);
        phPageTree.Controls.Clear();
        phPageTree.Controls.Add(divCtl);

        // add dialog opener to button click:
        browseTargetPage.Attributes.Add("onclick", "$ektron('div.ui-dialog:has(span." + experiment.ClientID + ")').show();");
    }

    public void Preview(Guid combinationGuid, List<Guid> variantGuids)
    {
        CombinationJSON combinationJson = new CombinationJSON();
        combinationJson.Guid = combinationGuid;
        combinationJson.VariantGuids = variantGuids;
        JS.RegisterJSBlock(this, String.Format("Ektron.Widget.MultivariateExperiment.AddCombination({0});",
                                               JsonConvert.SerializeObject(combinationJson)), combinationGuid.ToString());
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            experiment.InnerHtml = "Can't run experiments in a dashboard!";
            return;
        }

        Shared sh=new Shared();
        //only when new cuont is less than previous count, then it means the section is being removed.
        if (sh.PageVariations.Count<previousPageVariations && _controller.started)
        {
            btnStop_Click(sender, e);
            return;
        }
        _controller.Load();

        if (reportBTNClicked)
        {
            Ektron.Cms.API.JS.RegisterJSBlock(this,"isShowReport=true;","ShowReportCMD");
        }
        else
        {
            Ektron.Cms.API.JS.RegisterJSBlock(this, "isShowReport=false;","ShowReportCMD" );
        }
        registerJSCSS();
        previousPageVariations = sh.PageVariations.Count;
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        // remove the pagetree, it's already got a dialog on the page:
        //phPageTree.Controls.Clear();
        bool sucess= _controller.Start();
        StoredTargetID = TargetPageID;
        _host.SaveWidgetDataMembers();

        if(sucess)
            JS.RegisterJSBlock(this, "window.location = window.location", "refreshid");
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        // remove the pagetree, it's already got a dialog on the page:
        //phPageTree.Controls.Clear();

        _controller.Stop();
        StoredTargetID = TargetPageID;
        _host.SaveWidgetDataMembers();

        JS.RegisterJSBlock(this, "window.location = window.location", "refreshid");
    }

    // public event PreviewFunc PreviewEvent;

    protected void repCombinationReports_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        MultivariateReportData report = e.Item.DataItem as MultivariateReportData;

        LinkButton btnPromote = e.Item.FindControl("btnPromote") as LinkButton;
        LinkButton btnEnable = e.Item.FindControl("btnEnable") as LinkButton;
        LinkButton btnDisable = e.Item.FindControl("btnDisable") as LinkButton;
        HtmlAnchor aPreview = e.Item.FindControl("aPreview") as HtmlAnchor;

        btnDisable.Visible = !report.Disabled;
        btnEnable.Visible = report.Disabled;

        btnPromote.Visible = ((Page as PageBuilder).Status == Mode.Editing) && Shared.IsABTester;
        aPreview.Attributes["onclick"] = String.Format("return Ektron.Widget.MultivariateExperiment.Preview('{0}')",
                                                       report.CombinationGuid);
        
        btnPromote.Click += new EventHandler(delegate(object s, EventArgs ea)
        {
            //phPageTree.Controls.Clear();
            _controller.Promote(report.CombinationGuid);
        });

        btnDisable.Click += new EventHandler(delegate(object s, EventArgs ea)
        {
            //phPageTree.Controls.Clear();
            _controller.Disable(report.CombinationGuid);
            reportBTNClicked = true;
            
        });

        btnEnable.Click += new EventHandler(delegate(object s, EventArgs ea)
        {
            //phPageTree.Controls.Clear();
            reportBTNClicked = true;
            _controller.Enable(report.CombinationGuid);
        });
    }

    
}
