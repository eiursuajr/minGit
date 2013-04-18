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
using Ektron.Cms.Widget;
using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using System.Text.RegularExpressions;
using Ektron.Cms.Widget.Multivariate;

public partial class widgets_MultivariateSection : System.Web.UI.UserControl, IMultivariateSectionView
{
    private MultivariateSectionController _controller;

    private IWidgetHost _host = null;
    private List<Guid> _columns = new List<Guid>();
    private int _selectedIndex = -1;
    private bool _updateIndex = false;
    private bool _experimentActive = false;

    public Page ViewPage
    {
        get { return Page; }
        set { Page = value; }
    }

    public MultivariateSectionController Controller
    {
        get { return _controller; }
        set { _controller = value; }
    }

    public bool UpdateIndex
    {
        get
        {
            return _updateIndex;
        }

        set
        {
            _updateIndex = value;
            int index = _updateIndex ? _selectedIndex : -1;
            Ektron.Cms.API.JS.RegisterJSBlock(this,
                String.Format("Ektron.Widget.MultivariateSection.Init('{0}',{1},{2},{3})",
                    multivariate.ClientID,
                    _columns.Count,
                    index,
                    Ektron.Newtonsoft.Json.JsonConvert.SerializeObject(_columns)),
                "jsblock" + ClientID);
        }
    }

    public List<DisplayColumnData> DisplayedColumns
    {
        get
        {
            return repColumns.DataSource as List<DisplayColumnData>;
        }

        set
        {
            repColumns.DataSource = value;
            repColumns.DataBind();
        }
    }

    public bool IsSliderVisible
    {
        get { return slider.Visible; }
        set { slider.Visible = value; }
    }

    [WidgetDataMember()]
    public List<Guid> Columns
    {
        get
        {
            return _columns;
        }

        set
        {
            _columns = value;
        }
    }

    [WidgetDataMember(-1)]
    public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            multivariate.InnerHtml = "Cannot start experiments in a dashboard.";
            return;
        }

        PageBuilder pbPage = (Page as PageBuilder);
        //pbPage.Pagedata.Widgets
        IMultivariateExperimentModel expModel = new MultivariateExperimentModel();
        List<MultivariateExperimentData> experiments = expModel.FindByExperimentPageID(pbPage.Pagedata.pageID);
        _experimentActive = (experiments.Count > 0);

        Ektron.Cms.Widget.IWidgetHost host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        host.Title = "Multivariate Testing Section";
        _host = host;

        _controller = new MultivariateSectionController(this);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            return;
        }

        string sitepath = new CommonApi().SitePath;

        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronUISliderJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronJQueryUiDefaultCss);
        Ektron.Cms.API.Css.RegisterCss(this, sitepath + "widgets/MultivariateSection/css/MultivariateSection.css", "MultivariateSectionCSS");
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/MultivariateSection/js/MultivariateSection.js", "WidgetMultivariateSection");
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Page as PageBuilder == null)
        {
            return;
        }

        // sending "-1" as the current index tells the javascript to use
        // its saved index instead of updating with the one from the 
        // server side.
        // otherwise, the javascript will shift the column slider so that 
        // it points to the column with the selected index received from 
        // the server.
        // "_updateIndex" should only be set when the position of the 
        // slider should be changed (i.e. on adding or removing a 
        // variation).
        PageBuilder pbPage = (Page as PageBuilder);
        //pbPage.Pagedata.Widgets

        addVariant.Visible = btnAddVariation.Visible = !_experimentActive && (pbPage.Status == Mode.Editing) && Shared.IsABTester == true;
        _controller.PreRender();
        if (!Shared.ReportControllerLoaded && (Page as PageBuilder).Status == Mode.Editing)
        {
            DisplayedColumns = new List<DisplayColumnData>();
            IsSliderVisible = false;
            litDebugOutput.Text = "Please drop a Multivariate Experiment Widget on this page.";
        }
        else if (litDebugOutput.Text != "")
        {
            litDebugOutput.Text = "";
            Control upd = this;
            while ((upd as UpdatePanel) == null)
            {
                upd = upd.Parent;
            }
            UpdatePanel panel = upd as UpdatePanel;
            if (panel != null)
            {
                panel.Update();
            }
        }
    }

    protected void btnAddVariation_Click(object sender, EventArgs e)
    {
        _controller.AddVariation();
        _host.SaveWidgetDataMembers();
    }

    protected void btnNextVariation_Click(object sender, EventArgs e)
    {
        _controller.NextVariation();
    }

    protected void btnPreviousVariation_Click(object sender, EventArgs e)
    {
        _controller.PreviousVariation();
    }

    protected void repColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ContentAPI _api = new ContentAPI();
        string appPath = _api.AppPath;
        SiteAPI m_refSiteApi = new SiteAPI();
        Ektron.Cms.Common.EkMessageHelper m_refMsg = m_refSiteApi.EkMsgRef;

        HtmlImage imgresizecolumn = (e.Item.FindControl("imgresizecolumn") as HtmlImage);
        HtmlImage imgremcolumn = (e.Item.FindControl("imgremcolumn") as HtmlImage);
        HtmlAnchor lbResizeColumn = (e.Item.FindControl("lbResizeColumn") as HtmlAnchor);
        LinkButton btnDeleteColumn = (e.Item.FindControl("btnDeleteColumn") as LinkButton);
        Repeater controlcolumn = (e.Item.FindControl("controlcolumn") as Repeater);

        HtmlControl zonediv = (e.Item.FindControl("zone") as HtmlControl);
        HtmlControl column = (e.Item.FindControl("column") as HtmlControl);
        HtmlControl headerItem = (e.Item.FindControl("headerItem") as HtmlControl);

        //image paths
        //(e.Item.FindControl("imgleftcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_leftcorner.png";
        //(e.Item.FindControl("imgrightcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_rightcorner.png";
        imgresizecolumn.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/edit_off.png";
        imgresizecolumn.Alt = lbResizeColumn.Title = m_refMsg.GetMessage("lbl pagebuilder resize");
        imgremcolumn.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_close.png";
        imgremcolumn.Alt = btnDeleteColumn.Attributes["title"] = m_refMsg.GetMessage("generic delete title");

        lbResizeColumn.Visible = false;
        btnDeleteColumn.Visible = true;
        lbResizeColumn.Title = imgresizecolumn.Alt.ToString();

        headerItem.Visible = ((Page as PageBuilder).Status == Mode.Editing) && !_experimentActive;

        DisplayColumnData columnData = (DisplayColumnData)e.Item.DataItem;
        ColumnData thiscol = columnData.Column;
        List<Ektron.Cms.PageBuilder.WidgetData> mywidgets = columnData.Widgets;
        mywidgets.Sort(delegate(Ektron.Cms.PageBuilder.WidgetData left, Ektron.Cms.PageBuilder.WidgetData right) { return left.Order.CompareTo(right.Order); });

        btnDeleteColumn.Click += new EventHandler(delegate(object delSender, EventArgs delArgs)
        {
            _controller.RemoveVariation(columnData);
            _host.SaveWidgetDataMembers();
        });

        column.Attributes.Add("columnid", thiscol.columnID.ToString());
        column.Attributes.Add("columnguid", thiscol.Guid.ToString());

        zonediv.Style.Add("width", "100%");
   
            zonediv.Style.Remove("display");

        if((Page as PageBuilder).Status != Mode.Editing)
        {
                  zonediv.Style.Remove("width");
        }

        if ((Page as PageBuilder).Status != Mode.Editing || Shared.IsABTester == false || _experimentActive == true)
        {
            zonediv.Attributes["class"] = "PBViewing PBNonsortable";
        }
        else
        {
            zonediv.Attributes.Add("dropzoneid", columnData.DropZoneID);
            zonediv.Attributes["class"] = "PBColumn";
            zonediv.Attributes["class"] += " PBUnsizable";
        }

        controlcolumn.ItemDataBound += new RepeaterItemEventHandler(controlcolumn_ItemDataBound);

        controlcolumn.DataSource = mywidgets;
        controlcolumn.DataBind();
    }

    protected void controlcolumn_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;
        if (item.ItemType != ListItemType.Header && item.ItemType != ListItemType.Footer && (item.DataItem as Ektron.Cms.PageBuilder.WidgetData) != null)
        {
            Ektron.Cms.PageBuilder.WidgetData w = item.DataItem as Ektron.Cms.PageBuilder.WidgetData;
            WidgetHostCtrl ctrl = (WidgetHostCtrl)item.FindControl("WidgetHost");
            ctrl.ColumnID = w.ColumnID;
            ctrl.ColumnGuid = w.ColumnGuid;
            ctrl.SortOrder = w.Order;
            ctrl.WidgetHost_Load();
        }
    }
}