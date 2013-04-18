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


public partial class widgets_RotatingBanner : System.Web.UI.UserControl, IWidget
{

    #region properties

    private bool _Teaser;
    private bool _ShowMediaControl;
    private bool _Title;
    private string _ImageWidth;
    private string _ImageHeight;
    private int _Duration;
    private int _MaxResult;
    private long _CollectionID;
    private string _CollectionName;

    [WidgetDataMember(0)]
    public long CollectionID { get { return _CollectionID; } set { _CollectionID = value; } }

    [WidgetDataMember(true)]
    public bool Teaser { get { return _Teaser; } set { _Teaser = value; } }

    [WidgetDataMember(true)]
    public bool ShowMediaControl { get { return _ShowMediaControl; } set { _ShowMediaControl = value; } }

    [WidgetDataMember(true)]
    public bool Title { get { return _Title; } set { _Title = value; } }

    [WidgetDataMember("100")]
    public string ImageWidth { get { return _ImageWidth; } set { _ImageWidth = value; } }

    [WidgetDataMember("100")]
    public string ImageHeight { get { return _ImageHeight; } set { _ImageHeight = value; } }

    [WidgetDataMember(5)]
    public int Duration { get { return _Duration; } set { _Duration = value; } }

    [WidgetDataMember(5)]
    public int MaxResult { get { return _MaxResult; } set { _MaxResult = value; } }

    public string CollectionName { get { return _CollectionName; } set { _CollectionName = value; } }
    protected string RandCallID = "";

    protected string uniqueId
    {
        get { return (ViewState[this.ID + "uniqueid"] == null) ? "" : (string)ViewState[this.ID + "uniqueid"]; }
        set { ViewState[this.ID + "uniqueid"] = value; }
    }
    #endregion

    #region page variable
    IWidgetHost _host;

    protected ContentAPI _api;
    protected string appPath;
    protected string sitePath;
    protected int langType;
    protected bool isAdmin;

    #endregion

    #region Page related Event(s)

    #region Page Init
    /// <summary>
    /// Page Init Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Rotating Banner Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.ExpandOptions = Expandable.DontExpand;
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });

        _api = new ContentAPI();
        appPath = _api.AppPath;
        sitePath = _api.SitePath;
        //sitePath = _api.SitePath.TrimEnd('/');
        langType = _api.RequestInformationRef.ContentLanguage;

        if (_api.UserId > 0)
        {
            Ektron.Cms.API.User.User userAPI = new Ektron.Cms.API.User.User();
            isAdmin = userAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu, _api.UserId, true);
        }

        CreateUniqueId();


        //Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/jquery-1.3.2.min.js", "EktronWidgetRBJQuery");
        //Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/rs_ticker.js", "EktronWidgetRBJS");
        //Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/RotatingBanner.js", "RBJS");
        //Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/RotatingBanner/css/rs_ticker.css", "EktronWidgetCSS");

        ViewSet.SetActiveView(View);        
    }
    #endregion Page Init

    #endregion Page related Event(s)

    #region  Method(s)

    #region Edit Event
    /// <summary>
    ///  Edit Event Click
    /// </summary>
    /// <param name="settings">Setting String</param>
    void EditEvent(string settings)
    {
        string webserviceURL = sitePath + "widgets/RotatingBanner/ImageHandler.ashx";
        // Register JS
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJSInclude(this, sitePath + "widgets/RotatingBanner/behavior.js", "RotatingBannerWidgetBehaviorJS");
        // Insert CSS Links
        Css.RegisterCss(this, sitePath + "widgets/RotatingBanner/ImageStyle.css", "RotatingBannerWidgetCSS"); //cbstyle will include the other req'd stylesheets


        JS.RegisterJSBlock(this, "Ektron.PFWidgets.Image.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.Image.setupAll('" + uniqueId + "');", "EktronPFWidgetsFlashInit" + this.ID);
        long folderid = -1;

        hdnFolderId.Value = folderid.ToString();
        hdnFolderPath.Value = folderid.ToString();

        //while (folderid != 0)
        //{
        //    folderid = _api.GetParentIdByFolderId(folderid);
        //    if (folderid > 0) hdnFolderPath.Value += "," + folderid.ToString();
        //}

        //this will open the properties tab in edit mode
        JS.RegisterJSBlock(this, "LoadPropertiesTab('" + uniqueId + "');", "LoadPropertiesTab" + this.ID);


        Ektron.Cms.API.Content.Content oCol = new Ektron.Cms.API.Content.Content();
        Ektron.Cms.CollectionListData[] oCollList = oCol.EkContentRef.GetCollectionList();
        uxCollections.DataSource = oCollList;
        uxCollections.DataValueField = "Id";
        uxCollections.DataTextField = "Title";
        uxCollections.DataBind();
        if (CollectionID > 0 && uxCollections.Items.FindByValue(CollectionID.ToString())!=null)
            uxCollections.SelectedValue = CollectionID.ToString();    
        CollectionID = Convert.ToInt64(uxCollections.SelectedValue);
        CollectionName = uxCollections.SelectedItem.Text;
        uxImageHeight.Text = ImageHeight;
        uxImageWidth.Text = ImageWidth;
        uxMaxResult.Text = MaxResult.ToString();
        uxRotatingDuration.Text = Duration.ToString();
        uxShowTeaser.Checked = Teaser;
        uxShowTitle.Checked = Title;
        uxShowMediaControl.Checked = ShowMediaControl;

        //this will open the properties tab in edit mode
        JS.RegisterJSBlock(this, "LoadPropertiesTab('" + uniqueId + "');", "LoadPropertiesTab" + this.ID);
        ViewSet.SetActiveView(Edit);
    }
    #endregion Edit Event

    #region SetOutput
    /// <summary>
    /// Set Output
    /// </summary>
    protected void SetOutput()
    {
        if (CollectionID > 0)
        {
            Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/jquery-1.3.2.min.js", "EktronWidgetRBJQuery");
            Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/rs_ticker.js", "EktronWidgetRBJS");
            Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/RotatingBanner/js/RotatingBanner.js", "RBJS");
            Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/RotatingBanner/css/rs_ticker.css", "EktronWidgetCSS");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), @"jsloadScript" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>rs_start();</script>", false);

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

        if (_host.IsEditable)
        {
            RandCallID = DateTime.Now.Ticks.ToString();
        }

    }
    #endregion SetOutput

    #region Create Unique Id
    /// <summary>
    /// Create Unique Id
    /// </summary>
    protected void CreateUniqueId()
    {
        if (uniqueId == null || uniqueId == "")
        {
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }
    #endregion
    #endregion protected Method(s)

    #region Postback Event(s)

    #region Save Button Click
    /// <summary>
    ///  Save Button Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void uxSave_Click(object sender, EventArgs e)
    {
        Teaser = uxShowTeaser.Checked;
        Title = uxShowTitle.Checked;
        ShowMediaControl = uxShowMediaControl.Checked;
        Duration = Convert.ToInt32(uxRotatingDuration.Text);
        MaxResult = Convert.ToInt32(uxMaxResult.Text);
        CollectionID = Convert.ToInt64(uxCollections.SelectedItem.Value);
        ImageHeight = uxImageHeight.Text;
        ImageWidth = uxImageWidth.Text;

        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);

    }
    #endregion Save Button Click

    #region Cancel Button Click
    /// <summary>
    ///  Cancel Button Click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void uxCancel_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
    #endregion Cancel Button Click

    #endregion Postback Event(s)
}


