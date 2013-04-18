using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;


public partial class widgets_Blogs : System.Web.UI.UserControl, IWidget
{


    #region properties

    private long _BlogId;
    [WidgetDataMember(0)]
    public long BlogId
    {
        get { return _BlogId; }
        set { _BlogId = value; }
    }

    private bool _showcomment;
    [WidgetDataMember(true)]
    public bool ShowComment
    {
        get { return _showcomment; }
        set { _showcomment = value; }
    }

    private bool _showSummary;
    [WidgetDataMember(true)]
    public bool ShowSummary
    {
        get { return _showSummary; }
        set { _showSummary = value; }
    }

    private bool _showDate;
    [WidgetDataMember(true)]
    public bool ShowDate
    {
        get { return _showDate; }
        set { _showDate = value; }
    }

    private bool _showAuthor;
    [WidgetDataMember(true)]
    public bool ShowAuthor
    {
        get { return _showAuthor; }
        set { _showAuthor = value; }
    }

    private int _maxResult;
    [WidgetDataMember(10)]
    public int MaxResult
    {
        get { return _maxResult; }
        set { _maxResult = value; }
    }
    #endregion

    IWidgetHost _host;

    protected CommonApi _api;
    protected string appPath;
    protected int langType;
    protected string uniqueId;
    public string FolderSelected = "selected";
    public string PropertySelected = "selected";

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Blogs Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        //_host.ExpandOptions = Expandable.ExpandOnEdit;
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { MainView(); });


        Page.ClientScript.GetPostBackEventReference(SaveButton, "");
        _api = new Ektron.Cms.CommonApi();
        appPath = _api.AppPath;
        langType = _api.RequestInformationRef.ContentLanguage;
        MainView();
        ViewSet.SetActiveView(View);
        // Insert CSS Links
        Css.RegisterCss(this, _api.SitePath + "widgets/blogs/blogs.css", "blogsCSS");

    }


    protected void MainView()
    {
        if (BlogId > 0)
        {
            //get type

            Ektron.Cms.Controls.BlogEntries boe = new Ektron.Cms.Controls.BlogEntries();
            boe.BlogID = BlogId;
            boe.Page = this.Page;
            boe.MaxResults = MaxResult;
            boe.Fill();



            if (!boe.XmlDoc.InnerXml.Equals(String.Empty))
            {
                XmlDocument xdBlogs = new XmlDocument();
                xdBlogs.InnerXml = boe.XmlDoc.InnerXml;

                XsltArgumentList xsltBlogsArgList = new XsltArgumentList();

                if (ShowComment == true)
                    xsltBlogsArgList.AddParam("ShowComment", "", "Yes");
                else xsltBlogsArgList.AddParam("ShowComment", "", "No");

                if (ShowSummary == true)
                    xsltBlogsArgList.AddParam("ShowSummary", "", "Yes");
                else xsltBlogsArgList.AddParam("ShowSummary", "", "No");

                if (ShowAuthor == true)
                    xsltBlogsArgList.AddParam("ShowAuthor", "", "Yes");
                else xsltBlogsArgList.AddParam("ShowAuthor", "", "No");

                if (ShowDate == true)
                    xsltBlogsArgList.AddParam("ShowDate", "", "Yes");
                else xsltBlogsArgList.AddParam("ShowDate", "", "No");



                xmlBlogList.DocumentContent = xdBlogs.InnerXml;
                xmlBlogList.TransformSource = _api.SitePath + "widgets/blogs/Blog.xsl";
                xmlBlogList.TransformArgumentList = xsltBlogsArgList;

            }
            else xmlBlogList.DocumentContent = null;

            phContent.Visible = true;
            phHelpText.Visible = false;
        }
        else
        {
            xmlBlogList.DocumentContent = null;
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

    void SetBlogName(long lngBlogID)
    {
        try
        {
            if (lngBlogID > 0)
            {
                Ektron.Cms.API.Folder oFolder = new Folder();
                Ektron.Cms.FolderData oFData = oFolder.GetFolder(lngBlogID);
                if (!ReferenceEquals(oFData, null))
                {
                    uxBlogTitle.Text = oFData.Name;
                }
            }
            else
            {
                uxBlogTitle.Text = "";
            }

        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();


        }
    }

    void EditEvent(string settings)
    {
        try
        {
            string webserviceURL = _api.SitePath + "widgets/blogs/LSHandler.ashx";
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, _api.SitePath + "widgets/blogs/behavior.js", "BlogsWidgetBehaviorJS");

            if (BlogId > 0)
            {
                FolderSelected = "";
                JS.RegisterJSBlock(this, "Ektron.PFWidgets.ListSummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ListSummary.BlogTitleID = \"" + uxBlogTitle.ClientID + "\"; Ektron.PFWidgets.ListSummary.setupAll();Ektron.PFWidgets.ListSummary.SetTabs.init(); ", "EktronPFWidgetsLSInit");

            }
            else
            {
                PropertySelected = "";
                JS.RegisterJSBlock(this, "Ektron.PFWidgets.ListSummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ListSummary.BlogTitleID = \"" + uxBlogTitle.ClientID + "\"; Ektron.PFWidgets.ListSummary.setupAll();", "EktronPFWidgetsLSInit");

            }
            Css.RegisterCss(this, _api.SitePath + "widgets/blogs/LSStyle.css", "BlogLSStyleWidgetCSS");

            uxBlogs.Text = BlogId.ToString();
            SetBlogName(BlogId);

            ViewSet.SetActiveView(Edit);


        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            _host.Title = _host.Title + " error";
            ViewSet.SetActiveView(View);
        }
    }



    protected void SaveButton_Click(object sender, EventArgs e)
    {
        Int64 bid = 0;
        if (Int64.TryParse(uxBlogs.Text, out bid))
        {
            BlogId = bid;
            int result = 0;
            ShowComment = uxshowcomment.Checked;
            ShowSummary = uxSummary.Checked;
            ShowDate = uxshowdate.Checked;
            ShowAuthor = uxshowAuthor.Checked;
            if (int.TryParse(uxMaxResult.Text, out result))
                MaxResult = result;
            else MaxResult = 10;

            hdnblogid.Value = BlogId.ToString();
            uxMaxResult.Text = MaxResult.ToString();
            _host.SaveWidgetDataMembers();
            MainView();
        }
        else
        {

            editError.Text = "Invalid Blog Id";

        }
        ViewSet.SetActiveView(View);
    }


    protected void CancelButton_Click(object sender, EventArgs e)
    {
        MainView();
        ViewSet.SetActiveView(View);
    }

}
