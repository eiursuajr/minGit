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
using System.Text.RegularExpressions;


public partial class Widgets_MetaDataList : System.Web.UI.UserControl, IWidget
{

    #region properties
    private long _FolderId;
    private bool _Recursive;
    private bool _ExactPhrase;
    private bool _MatchAll;
    private string _KeywordName;
    private string _KeywordValue;
    private string _KeywordSeparator;
    private bool _Paging;
    private int _MaxResults;
    private string _NavTeaser;
    private string _SortOrder;
    private bool _IncludeIcons;
    private string _OrderKey;
    private string _contentType;

    [WidgetDataMember(0)]
    public long FolderId { get { return _FolderId; } set { _FolderId = value; } }
    [WidgetDataMember(true)]
    public bool Recursive { get { return _Recursive; } set { _Recursive = value; } }
    [WidgetDataMember(false)]
    public bool ExactPhrase { get { return _ExactPhrase; } set { _ExactPhrase = value; } }
    [WidgetDataMember(true)]
    public bool MatchAll { get { return _MatchAll; } set { _MatchAll = value; } }
    [WidgetDataMember("")]
    public string KeywordName { get { return _KeywordName; } set { _KeywordName = value; } }
    [WidgetDataMember("")]
    public string KeywordValue { get { return _KeywordValue; } set { _KeywordValue = value; } }
    [WidgetDataMember(";")]
    public string KeywordSeparator { get { return _KeywordSeparator; } set { _KeywordSeparator = value; } }
    [WidgetDataMember(false)]
    public bool Paging { get { return _Paging; } set { _Paging = value; } }
    [WidgetDataMember(10)]
    public int MaxResults { get { return _MaxResults; } set { _MaxResults = value; } }
    [WidgetDataMember("ecmNavigation")]
    public string NavTeaser { get { return _NavTeaser; } set { _NavTeaser = value; } }
    [WidgetDataMember("Ascending")]
    public string SortOrder { get { return _SortOrder; } set { _SortOrder = value; } }
    [WidgetDataMember(false)]
    public bool IncludeIcons { get { return _IncludeIcons; } set { _IncludeIcons = value; } }
    [WidgetDataMember("Title")]
    public string OrderKey { get { return _OrderKey; } set { _OrderKey = value; } }
    [WidgetDataMember("AllTypes")]
    public string ContentType { get { return _contentType; } set { _contentType = value; } }

    #endregion

    IWidgetHost _host;
    private ContentAPI _capi;
    protected string uniqueId;
    protected string appPath;



    protected void Page_Init(object sender, EventArgs e)
    {
        CreateUniqueId();
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "MetaDataList Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
		_host.ExpandOptions = Expandable.ExpandOnEdit;
        Load += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { if(ViewSet.GetActiveView() != Edit) MainView(); });
        _capi = new ContentAPI();
        appPath = _capi.ApplicationPath;
        ViewSet.SetActiveView(View);
    }
        
    void EditEvent(string settings)
    {
		string webserviceURL = _capi.SitePath + "widgets/MetaDataList/CBHandler.ashx";
        Css.RegisterCss(this, _capi.SitePath + "widgets/MetaDataList/MDStyle.css", "MDWidgetCSS"); //MDStyle will include the other req'd stylesheets
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);

        // Register JS
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, _capi.SitePath + "widgets/MetaDataList/behavior.js", "MetaDataListWidgetBehaviorJS");
        JS.RegisterJSBlock(this, "Ektron.PFWidgets.MetadataList.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.MetadataList.setupAll('" + uniqueId + "');", "EktronPFWidgetsMDInit");       
    
        //populate hdnfolderpath
        if (FolderId > 0)
        {
            long folderid = FolderId;
            hdnFolderPath.Value = folderid.ToString();
            while (folderid != 0)
            {
                folderid = _capi.GetParentIdByFolderId(folderid);
                if (folderid > 0) hdnFolderPath.Value += "," + folderid.ToString();
            }
        }
        else
        {
            hdnFolderPath.Value = "";
        }

        //populate keywordname list
        ContentMetaData[] meta = _capi.GetMetaDataTypes("");
        optKeywordName.Items.Clear();
        foreach (ContentMetaData me in meta)
        {
            ListItem li = new ListItem();
            li.Text = me.TypeName;
            li.Value = me.TypeName;
            if (me.TypeName == KeywordName)
            {
                li.Selected = true;
            }
            if (me.TagType == 100)
            {
            optKeywordName.Items.Add(li);
            }
        }

        //optNavTeaser.Items.FindByValue(NavTeaser).Selected = true;
        optNavTeaser.SelectedValue = NavTeaser;
        optSortOrder.SelectedValue = SortOrder;
        optFolderId.Text = FolderId.ToString();
        optRecursive.Checked = Recursive;
        optExactPhrase.Checked = ExactPhrase;
        optMatchAll.Checked = MatchAll;
        optKeywordVal.Text = KeywordValue;
        optSeperator.Text = KeywordSeparator;
        optPaging.Checked = Paging;
        optMaxNum.Text = MaxResults.ToString();
        optIncludeIcons.Checked = IncludeIcons;
        OrderKeyDropDownList.SelectedValue = OrderKey;
        ContentTypeDropDownList.SelectedValue = ContentType;

       
        
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
       ExactPhrase = optExactPhrase.Checked;
        long.TryParse(optFolderId.Text, out _FolderId);
        IncludeIcons = optIncludeIcons.Checked;
        KeywordName = optKeywordName.SelectedValue;
        KeywordSeparator = optSeperator.Text;
        KeywordValue = optKeywordVal.Text;
        MatchAll = optMatchAll.Checked;
        int.TryParse(optMaxNum.Text, out _MaxResults);
        NavTeaser = optNavTeaser.SelectedValue;
        Paging = optPaging.Checked;
        Recursive = optRecursive.Checked;
        OrderKey = OrderKeyDropDownList.SelectedValue;
        SortOrder = optSortOrder.SelectedValue;
        OrderKey = OrderKeyDropDownList.SelectedValue; 
        ContentType = ContentTypeDropDownList.SelectedValue;
        _host.SaveWidgetDataMembers();
        MainView();
        ViewSet.SetActiveView(View);
    }

    protected void MainView()
    {
        if (KeywordName != "")
        {
            MDList.FolderID = FolderId;
            MDList.Recursive = Recursive;
            MDList.ExactPhrase = ExactPhrase;
            MDList.KeyWordValueMatchAll = MatchAll;
            MDList.KeyWordName = KeywordName;
            MDList.KeyWordValue = KeywordValue;
            MDList.KeyWordValueSeparator = KeywordSeparator;
            MDList.EnablePaging = Paging;
            MDList.MaxNumber = MaxResults;
            MDList.DisplayXslt = NavTeaser;
            MDList.SortOrder = (SortOrder == "Ascending") ? Ektron.Cms.Controls.CmsWebService.OrderByDirection.Ascending : Ektron.Cms.Controls.CmsWebService.OrderByDirection.Descending;
            switch (OrderKey)
            {
                case "Title":
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.Title;
                    break;
                case "DateModified":
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.DateModified;
                    break;
                case "DateCreated":
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.DateCreated;
                    break;
                case "LastEditorFname":
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.LastEditorFname;
                    break;
                case "LastEditorLname":
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.LastEditorLname;
                    break;
                default: //ID
                    MDList.OrderBy = Ektron.Cms.Controls.CmsWebService.ContentOrderBy.Id;
                    break;
            }
            MDList.IncludeIcons = IncludeIcons;

            Ektron.Cms.PageBuilder.PageBuilder page = (Page as Ektron.Cms.PageBuilder.PageBuilder);
            if (page != null) MDList.CacheInterval = page.CacheInterval;
            if (ContentType == null) { MDList.ContentType = Ektron.Cms.Controls.CmsWebService.CMSContentType.AllTypes; }
            else { MDList.ContentType = (Ektron.Cms.Controls.CmsWebService.CMSContentType)Enum.Parse(typeof(Ektron.Cms.Controls.CmsWebService.CMSContentType), ContentType); }

            MDList.Fill();
        }
    }
    protected void CreateUniqueId()
    {
        uniqueId = System.Guid.NewGuid().ToString();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        MainView();
        ViewSet.SetActiveView(View);
    }

    
}






