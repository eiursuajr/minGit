using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Controls.CmsWebService;
using Ektron.Cms.PageBuilder;

public partial class widgets_ListSummary : System.Web.UI.UserControl, IWidget
{
    #region properties
    private long _folderid;
    private bool _teaser;
    private bool _recursive;
    private bool _enablePaging;
    private int _pageSize;
    private bool _includeIcons;
    private string _direction;
    private string _orderKey;
    private string _addText;
    private long _selTaxonomyID;
    private bool _displaySelectedContent;
	private string _contentType;
    [WidgetDataMember(0)]
    public long FolderId { get { return _folderid; } set { _folderid = value; } }
    [WidgetDataMember(true)]
    public bool Teaser { get { return _teaser; } set { _teaser = value; } }
    [WidgetDataMember(false)]
    public bool Recursive { get { return _recursive; } set { _recursive = value; } }
    [WidgetDataMember(false)]
    public bool EnablePaging { get { return _enablePaging; } set { _enablePaging = value; } }
    [WidgetDataMember(10)]
    public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
    [WidgetDataMember(false)]
    public bool IncludeIcons { get { return _includeIcons; } set { _includeIcons = value; } }
    [WidgetDataMember("Ascending")]
    public string Direction { get { return _direction; } set { _direction = value; } }
    [WidgetDataMember("Title")]
    public string OrderKey { get { return _orderKey; } set { _orderKey = value; } }
	[WidgetDataMember("AllTypes")]
    public string ContentType { get { return _contentType; } set { _contentType = value; } }
    [WidgetDataMember("Add Content")]
    public string AddText { get { return _addText; } set { _addText = value; } }
    [WidgetDataMember(0)]
    public long SelTaxonomyID { get { return _selTaxonomyID; } set { _selTaxonomyID = value; } }
    [WidgetDataMember(false)]
    public bool DisplaySelectedContent { get { return _displaySelectedContent; } set { _displaySelectedContent = value; } }
    #endregion

    private IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "List Summary Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        Load += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { if(ViewSet.GetActiveView() != Edit) SetListSummary(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        try
        {
            string sitepath = new CommonApi().SitePath;
            string webserviceURL = sitepath + "widgets/listsummary/LSHandler.ashx";

            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, sitepath + "widgets/ListSummary/behavior.js", "ListSummaryWidgetBehaviorJS");
            JS.RegisterJSBlock(this, "Ektron.PFWidgets.ListSummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ListSummary.setupAll(); ", "EktronPFWidgetsLSInit");
            Css.RegisterCss(this, sitepath + "widgets/listsummary/LSStyle.css", "LSWidgetCSS");

            folderid.Text = FolderId.ToString();
            pagesize.Text = PageSize.ToString();
            TeaserCheckBox.Checked = Teaser;
            IncludeIconsCheckBox.Checked = IncludeIcons;
            EnablePagingCheckBox.Checked = EnablePaging;
            RecursiveCheckBox.Checked = Recursive;
            OrderKeyDropDownList.SelectedValue = OrderKey;
			ContentTypeDropDownList.SelectedValue = ContentType;
            DirectionSelectDropDownList.SelectedValue = Direction;
            AddTextTextBox.Text = AddText;
            SelTaxonomyIDTextBox.Text = SelTaxonomyID.ToString();
            DisplaySelectedContentCheckBox.Checked = DisplaySelectedContent;

            ViewSet.SetActiveView(Edit);
        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            ViewSet.SetActiveView(View);
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        // Save the variables
        if (!long.TryParse(folderid.Text, out _folderid)) _folderid = 0;
        if (!int.TryParse(pagesize.Text, out _pageSize)) _pageSize = 2;
        if (!long.TryParse(SelTaxonomyIDTextBox.Text, out _selTaxonomyID)) _selTaxonomyID = 0;
        Teaser = TeaserCheckBox.Checked;
        IncludeIcons = IncludeIconsCheckBox.Checked;
        EnablePaging = EnablePagingCheckBox.Checked;
        Recursive = RecursiveCheckBox.Checked;
        Direction = DirectionSelectDropDownList.SelectedValue;
        OrderKey = OrderKeyDropDownList.SelectedValue;
		ContentType = ContentTypeDropDownList.SelectedValue; 
        AddText = (AddTextTextBox.Text.Length > 1) ? AddTextTextBox.Text : "Add Content";
        DisplaySelectedContent = DisplaySelectedContentCheckBox.Checked;

        _host.SaveWidgetDataMembers();
        SetListSummary();

        ViewSet.SetActiveView(View);
    }


    protected void SetListSummary()
    {
        ListSummary1.FolderID = FolderId;
        ListSummary1.DisplayXslt = (Teaser) ? "ecmTeaser" : "ecmNavigation";
        ListSummary1.IncludeIcons = IncludeIcons;
        ListSummary1.OrderByDirection = (Direction == "Ascending") ? EkEnumeration.OrderByDirection.Ascending : EkEnumeration.OrderByDirection.Descending;
        if (OrderKey == null) { ListSummary1.OrderBy = TeasersOrderBy.Title; }
        else { ListSummary1.OrderBy = (TeasersOrderBy)Enum.Parse(typeof(TeasersOrderBy), OrderKey); }
		 if (ContentType == null) { ListSummary1.ContentType = CMSContentType.AllTypes; }
        else { ListSummary1.ContentType = (CMSContentType)Enum.Parse(typeof(CMSContentType), ContentType); }
        ListSummary1.MaxResults = PageSize;
        ListSummary1.EnablePaging = EnablePaging;
        ListSummary1.Recursive = Recursive;
        ListSummary1.AddText = AddText;
        ListSummary1.SelTaxonomyID = SelTaxonomyID;
        ListSummary1.ContentParameter = DisplaySelectedContent ? "id" : "no_id";
        ListSummary1.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
        ListSummary1.Fill();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
        SetListSummary();
    }
}