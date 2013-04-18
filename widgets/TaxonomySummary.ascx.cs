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

public partial class widgets_TaxonomySummary : System.Web.UI.UserControl, IWidget
{
    #region properties
    private long _taxonomyid;
    private bool _teaser;
    private bool _enablePaging;
    private int _pageSize;
    private string _direction;
    private string _orderKey;
    private string _selThemes;
    private string _headerText;
    public string m_strTaxonomyPath = "/";
    public string TaxonomySelected = "selected";
    public string PropertySelected = "selected";
    protected string appPath = "";

    private Ektron.Cms.CommonApi _commonAPI = new CommonApi();
    [WidgetDataMember(0)]
    public long TaxonomyId { get { return _taxonomyid; } set { _taxonomyid = value; } }

    [WidgetDataMember(true)]
    public bool Teaser { get { return _teaser; } set { _teaser = value; } }

    [WidgetDataMember(false)]
    public bool EnablePaging { get { return _enablePaging; } set { _enablePaging = value; } }

    [WidgetDataMember(5)]
    public int PageSize { get { return _pageSize; } set { _pageSize = value; } }



    [WidgetDataMember("Ascending")]
    public string Direction { get { return _direction; } set { _direction = value; } }

    [WidgetDataMember("content_title")]
    public string OrderKey { get { return _orderKey; } set { _orderKey = value; } }

    [WidgetDataMember("default")]
    public string SelectedThemes { get { return _selThemes; } set { _selThemes = value; } }

    [WidgetDataMember("")]
    public string HeaderText { get { return _headerText; } set { _headerText = value; } }

    #endregion

    private IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Taxonomy Summary Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _host.ExpandOptions = Expandable.ExpandOnEdit;
        appPath = _commonAPI.AppPath;
        Load += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { if (ViewSet.GetActiveView() != Edit) SetTaxonomySummary(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        try
        {
            string sitepath = _commonAPI.SitePath;
            string webserviceURL = sitepath + "widgets/taxonomysummary/TSHandler.ashx";
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, sitepath + "widgets/taxonomysummary/behavior.js", "TaxonomySummaryWidgetBehaviorJS");

            if (TaxonomyId > 0)
            {
                TaxonomySelected = "";
                JS.RegisterJSBlock(this, "Ektron.PFWidgets.TaxonomySummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.TaxonomySummary.setupAll();Ektron.PFWidgets.TaxonomySummary.SetTabs.init(); ", "EktronPFWidgetsTSInit");
            }
            else
            {
                PropertySelected = "";
                JS.RegisterJSBlock(this, "Ektron.PFWidgets.TaxonomySummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.TaxonomySummary.setupAll(); ", "EktronPFWidgetsTSInit");
            }

            Css.RegisterCss(this, sitepath + "widgets/taxonomysummary/TSStyle.css", "TSWidgetCSS");

            taxonomyid.Text = TaxonomyId.ToString();
            pagesize.Text = PageSize.ToString();
            TeaserCheckBox.Checked = Teaser;

            EnablePagingCheckBox.Checked = EnablePaging;

            OrderKeyDropDownList.SelectedValue = OrderKey;
            DirectionSelectDropDownList.SelectedValue = Direction;

            ViewSet.SetActiveView(Edit);


            string[] dirs = System.IO.Directory.GetDirectories(Server.MapPath("~/widgets/TaxonomySummary/themes"));
            uxThemes.Items.Clear();
            uxThemes.Attributes.Add("onchange", "javascript:Ektron.PFWidgets.TaxonomySummary.themesPreview(this.id,'" + uxFramePreview.ClientID + "','" + sitepath + "')");



            if (!(dirs == null || dirs.Length == 0))
            {
                for (int themeC = 0; themeC < dirs.Length; themeC++)
                {
                    string folderName = dirs[themeC];
                    folderName = folderName.Substring(folderName.LastIndexOf('\\') + 1);
                    if (string.Compare(folderName, "default", true) != 0)
                    {
                        uxThemes.Items.Insert(0, new ListItem(folderName, folderName.ToLower()));
                    }

                }
            }


            uxThemes.Items.Insert(0, new ListItem("Default", "default"));

            object obj = uxThemes.Items.FindByValue(SelectedThemes);
            if (obj != null)
            {
                uxThemes.SelectedIndex = -1;
                ((ListItem)obj).Selected = true;
            }

            //uxFramePreview.Src = ResolveUrl("~/widgets/TaxonomySummary/themes/") + SelectedThemes + "/preview.jpg";

            headertext.Text = HeaderText;

            //set taxonomy path
            Ektron.Cms.API.Content.Taxonomy _apiTaxonomy = new Ektron.Cms.API.Content.Taxonomy();
            Ektron.Cms.TaxonomyRequest taxRequest = new Ektron.Cms.TaxonomyRequest();
            taxRequest.TaxonomyId = TaxonomyId;
            taxRequest.IncludeItems = false;
            taxRequest.TaxonomyLanguage = _apiTaxonomy.ContentLanguage;
            Ektron.Cms.TaxonomyData taxData = _apiTaxonomy.LoadTaxonomy(ref taxRequest);

            if (!(taxData == null || string.IsNullOrEmpty(taxData.Path)))
            {
                taxonomypath.Text = taxData.Path ;
                m_strTaxonomyPath = taxData.Path;
            }
            else
            {
                taxonomypath.Text = "";
                m_strTaxonomyPath = "";
            }
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
        if (!long.TryParse(taxonomyid.Text, out _taxonomyid)) _taxonomyid = 0;
        int.TryParse(pagesize.Text, out _pageSize);
        if (_pageSize <= 0)
        {
            _pageSize = 5;
        }

        Teaser = TeaserCheckBox.Checked;
        EnablePaging = EnablePagingCheckBox.Checked;
        Direction = DirectionSelectDropDownList.SelectedValue;
        OrderKey = OrderKeyDropDownList.SelectedValue;
        HeaderText = headertext.Text;

        if (!string.IsNullOrEmpty(uxThemes.SelectedValue))
        {
            SelectedThemes = uxThemes.SelectedValue;
        }

        _host.SaveWidgetDataMembers();
        SetTaxonomySummary();

        ViewSet.SetActiveView(View);
    }


    protected void SetTaxonomySummary()
    {
        if (TaxonomyId > 0)
        {

            //set header text
            if (!string.IsNullOrEmpty(HeaderText))
            {
                uxHeaderText.InnerHtml = HeaderText;
                uxHeaderText.Visible = true;
            }
            else
            {
                uxHeaderText.Visible = false;
            }

            //add themes css
            Css.RegisterCss(this, _commonAPI.SitePath + "widgets/taxonomysummary/themes/" + SelectedThemes + "/style.css", "TSWidgetStyleCSS_" + SelectedThemes);

            //set taxonomy id
            TaxonomySummary1.TaxonomyId = TaxonomyId;

            //set display xsl
            if (Teaser)
            {
                TaxonomySummary1.DisplayXslt = "~/widgets/TaxonomySummary/ecmTeaser.xsl";
            }
            else
            {
                TaxonomySummary1.DisplayXslt = "~/widgets/TaxonomySummary/ecmNavigation.xsl";
            }

            // TaxonomySummary1.DisplayXslt = "~/widgets/TaxonomySummary/themes" + themespath + "TaxonomySummary.xsl";

            //set sort Order 
            TaxonomySummary1.SortDirection = (Direction == "Ascending") ? Ektron.Cms.Controls.OrderByDirection.Ascending : Ektron.Cms.Controls.OrderByDirection.Descending;

            //set order by field
            if (!string.IsNullOrEmpty(OrderKey))
            {
                TaxonomySummary1.OrderItemsBy = (Ektron.Cms.Controls.EkWebControl.TaxonomyItemSortOrder)Enum.Parse(typeof(Ektron.Cms.Controls.EkWebControl.TaxonomyItemSortOrder), OrderKey, true);
            }
            else
            {
                TaxonomySummary1.OrderItemsBy = Ektron.Cms.Controls.EkWebControl.TaxonomyItemSortOrder.content_title;
            }

            TaxonomySummary1.MaxResults = PageSize;
            TaxonomySummary1.EnablePaging = EnablePaging;
            TaxonomySummary1.CacheInterval = ((Page as PageBuilder) != null) ? (Page as PageBuilder).CacheInterval : 0;
            TaxonomySummary1.EnableSearch = false;
            TaxonomySummary1.Fill();

            phContent.Visible = true;
            phHelpText.Visible = false;
        }
        else
        {
            phContent.Visible = false;
            phHelpText.Visible = true;
        }

        if (!(_host == null || _host.IsEditable == false))
        {
            divHelpText.Visible = true;
        }
        else
        {
            divHelpText.Visible = false;
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
        SetTaxonomySummary();
    }


}