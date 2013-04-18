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


public partial class Widgets_ContentBlock : System.Web.UI.UserControl, IWidget
{

    #region properties

    private long _ContentBlockId;
    private bool _IsGoogleMultivariate;
    private string _GoogleSectionName;
    private bool _ShowTestingTab;
    
    [WidgetDataMember(0)]
    public long ContentBlockId { get { return _ContentBlockId; } set { _ContentBlockId = value; } }

    [WidgetDataMember(false)]
    public bool IsGoogleMultivariate { get { return _IsGoogleMultivariate; } set { _IsGoogleMultivariate = value; } }

    [WidgetDataMember("")]
    public string GoogleSectionName { get { return _GoogleSectionName; } set { _GoogleSectionName = value; } }

    [GlobalWidgetData(false)]
    public bool ShowTestingTab { get { return _ShowTestingTab; } set { _ShowTestingTab = value; } }

    #endregion

    Ektron.Cms.PageBuilder.WidgetHost _host;

    protected CommonApi _api;
    protected string appPath;
    protected int langType;
    protected string uniqueId;


    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this) as Ektron.Cms.PageBuilder.WidgetHost;
        _host.Title = "Content Block Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        _host.ExpandOptions = Expandable.ExpandOnEdit;

        this.EnableViewState = false;

        if ((Page as PageBuilder)!= null && (Page as PageBuilder).Status != Mode.Editing && ShowTestingTab)
        {
            litGoogleHeader.Text = String.Format("<script>utmx_section(\"{0}\")</script>",
                                                 GoogleSectionName);
            litGoogleFooter.Text = "</noscript>";
        }
        else
        {
            litGoogleHeader.Text = "";
            litGoogleFooter.Text = "";
        }

        Page.ClientScript.GetPostBackEventReference(SaveButton, "");
        _api = new Ektron.Cms.CommonApi();
        appPath = _api.AppPath;
        langType = _api.RequestInformationRef.ContentLanguage;
        MainView();
        ViewSet.SetActiveView(View);
    }

   
    protected void MainView()
    {
        if (ContentBlockId > -1)
        {
            //get type
            PageBuilder p = Page as PageBuilder;
            CB.DefaultContentID = ContentBlockId;
            if (p != null)
            {
                CB.CacheInterval = p.CacheInterval;
            }
            CB.Fill();
           

            if (CB.EkItem.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms && (p == null || p.Status != Mode.Editing))
            {
                FB.DefaultFormID = ContentBlockId;
                if (p != null)
                {
                    FB.CacheInterval = p.CacheInterval;
                }
                FB.Fill();
                FB.Visible = true;
                CB.Visible = false;
            }
            else
            {
                CB.Visible = true;
                FB.Visible = false;
            }

            if (!CB.Title.Equals(""))
            {
                _host.Title = CB.Title;
            }
        }
		else if (Page.Request["id"] != null && long.TryParse(Page.Request["id"], out _ContentBlockId))
		{
			CB.DefaultContentID = _ContentBlockId;
            CB.Fill();
			CB.Visible = true;
			FB.Visible = false;
        }
    }

    void EditEvent(string settings)
    {
        try
        {
            tabTesting.Visible = _ShowTestingTab;

            string webserviceURL = _api.SitePath + "widgets/contentblock/CBHandler.ashx";
            // Register JS
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, _api.SitePath + "widgets/contentblock/behavior.js", "ContentBlockWidgetBehaviorJS");
            
            // Insert CSS Links
            Css.RegisterCss(this, _api.SitePath + "widgets/contentblock/CBStyle.css", "CBWidgetCSS"); //cbstyle will include the other req'd stylesheets
		Ektron.Cms.Framework.UI.Packages.jQuery.jQueryUI.ThemeRoller.Register(this); //cbstyle will include the other req'd stylesheets

            JS.RegisterJSBlock(this, "Ektron.PFWidgets.ContentBlock.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ContentBlock.setupAll('"+ ClientID  +"');", "EktronPFWidgetsCBInit");

            IsGoogleMultivariate = cbMultiVariate.Checked;
            GoogleSectionName = tbSectionName.Text;

            ViewSet.SetActiveView(Edit);

            if (ContentBlockId > 0)
            {
                tbData.Text = ContentBlockId.ToString();
                ContentAPI capi = new ContentAPI();
                long folderid = capi.GetFolderIdForContentId(ContentBlockId);
                tbFolderPath.Text = folderid.ToString();
                while (folderid != 0)
                {
                    folderid = capi.GetParentIdByFolderId(folderid);
                    if (folderid > 0) tbFolderPath.Text += "," + folderid.ToString();
                }
            }
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
        Int64 cid = 0;
        if (Int64.TryParse(tbData.Text, out cid))
        {
            IsGoogleMultivariate = cbMultiVariate.Checked;
            GoogleSectionName = tbSectionName.Text;
            ContentBlockId = cid;

            ObjectData objectData = new ObjectData();
            objectData.ObjectId = cid;
            objectData.ObjectLanguage = (CB.EkItem.Id == 0 ? CB.LanguageID : CB.EkItem.Language);
            objectData.ObjectType = EkEnumeration.CMSObjectTypes.Content;

            _host.PBWidgetInfo.Associations.Clear();
            _host.PBWidgetInfo.Associations.Add(objectData);
            
            _host.SaveWidgetDataMembers();
            MainView();
        }
        else
        {
            tbData.Text = "";
            editError.Text = "Invalid Content Block Id";

        }
        ViewSet.SetActiveView(View);
    }


    protected void CancelButton_Click(object sender, EventArgs e)
    {
        MainView();
        ViewSet.SetActiveView(View);
    }

    
}
