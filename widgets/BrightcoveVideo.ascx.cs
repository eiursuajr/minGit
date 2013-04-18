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
using Ektron.Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

/// <summary>
/// BrightCove Video Widget
/// </summary>
public partial class Widgets_BrightcoveVideo : System.Web.UI.UserControl, IWidget
{
    IWidgetHost _host;
    string sitepath = string.Empty;
    ViewMode currentMode = ViewMode.View;

    private const string settingsCacheKey = "BrightcoveWidgetSettingsCacheKey";
    private const string playerIDsCacheKey = settingsCacheKey + "PlayerIDs";
    private const string publisherIDCacheKey = settingsCacheKey + "PublisherID";
    private const string readTokenCacheKey = settingsCacheKey + "ReadToken";
    private const string writeTokenCacheKey = settingsCacheKey + "WriteToken";

    #region properties
    private string _playerIDs;
    private long _publisherID;
    private string _readToken;
    private string _writeToken;

    public long _videoID;
    public long _playerID;
    public long _playlistID;
    public string _embedCode;

    public int _width;
    public int _height;

    private String fileURL = string.Empty;

    public string SitePath { get { return sitepath; } set { sitepath = value; } }

    [GlobalWidgetData(true)]
    public bool showSetup { get; set; }

    [GlobalWidgetData("")]
    public string PlayerIDs { get { return _playerIDs; } set { _playerIDs = value; } }

    [GlobalWidgetData(0)]
    public long PublisherID { get { return _publisherID; } set { _publisherID = value; } }

    [GlobalWidgetData("")]
    public string ReadToken { get { return _readToken; } set { _readToken = value; } }

    [GlobalWidgetData("")]
    public string WriteToken { get { return _writeToken; } set { _writeToken = value; } }

    [WidgetDataMember(0)]
    public long VideoID { get { return _videoID; } set { _videoID = value; } }

    [WidgetDataMember(0)]
    public long PlayerID { get { return _playerID; } set { _playerID = value; } }

    [WidgetDataMember(0)]
    public long PlaylistID { get { return _playlistID; } set { _playlistID = value; } }

    [WidgetDataMember("")]
    public string EmbedCode { get { return _embedCode; } set { _embedCode = value; } }

    [WidgetDataMember(400)]
    public int Width { get { return _width; } set { _width = value; } }

    [WidgetDataMember(250)]
    public int Height { get { return _height; } set { _height = value; } }

    #endregion

    /// <summary>
    /// Page_Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        sitepath = new CommonApi().SitePath;
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);

        if (_host != null)
        {
            if (string.IsNullOrEmpty(_writeToken))
            {
                //this.tabUpdate.Visible = false;
                //this.tabUpload.Visible = false;
                //this.pnlUpload.Visible = false;
            }

            _host.Title = "BrightCove";
            _host.Edit += new EditDelegate(EditEvent);
            _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
            _host.ExpandOptions = Expandable.ExpandOnEdit;
            _host.HelpFile = Ektron.Site.SiteData.Current.Cms.SitePath + "widgets/BrightcoveVideo/help/BrightcoveVideo.htm";

            // Set initial View mode, might change on post back
            currentMode = getViewMode();

            // Cache the global properties for the handler to use
            this.CacheGlobalProperties();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetOutput();
    }

    /// <summary>
    /// Edit event of the widget
    /// </summary>
    /// <param name="settings">settings of the widget as a string</param>
    void EditEvent(string settings)
    {
        if (showSetup || (String.IsNullOrEmpty(ReadToken) || PublisherID == 0 || String.IsNullOrEmpty(PlayerIDs)))
        {
            currentMode = ViewMode.EditSettings;

            // Settings
            if (this.PublisherID > 0)
            {
                tbPublisherID.Text = this.PublisherID.ToString();
            }
            tbReadToken.Text = this.ReadToken;
            tbWriteToken.Text = this.WriteToken;
            tbPlayerIds.Text = this.PlayerIDs;
        }
        else
        {
            currentMode = ViewMode.Edit;
            if (PlayerID > 0)
            {
                hdnPlayerId.Value = PlayerID.ToString();
            }
            if (VideoID > 0)
            {
                hdnVideoId.Value = VideoID.ToString();
            }
            if (PlaylistID > 0)
            {
                hdnPlaylistId.Value = PlaylistID.ToString();
            }
            if (!String.IsNullOrEmpty(EmbedCode))
            {
                tbEmbed.Text = EmbedCode;
            }
        }
    }

    protected void btnSaveSettingsClk(object sender, EventArgs e)
    {
        // TODO add in checks
        if (Page.IsValid)
        {
            this.PublisherID = long.Parse(tbPublisherID.Text);
            this.ReadToken = tbReadToken.Text;
            this.WriteToken = tbWriteToken.Text;
            this.PlayerIDs = tbPlayerIds.Text; // needs to be comma seperated, add in checks
            this.showSetup = false;
            this.saveGlobalWidgetProperties();
            currentMode = ViewMode.NoVideo;
            // execute edit

            // Display our instructions for selecting your video
            phNoVideoInstructions.Visible = true;
        }
        else
        {
            currentMode = ViewMode.EditSettings;
        }
    }

    protected void btnSaveClk(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            VideoID = 0;
            PlaylistID = 0;
            EmbedCode = "";
            if (!String.IsNullOrEmpty(hdnVideoId.Value))
            {
                long.TryParse(hdnVideoId.Value, out _videoID);
            }
            if (!String.IsNullOrEmpty(hdnPlaylistId.Value))
            {
                long.TryParse(hdnPlaylistId.Value, out _playlistID);
            }
            PlayerID = long.Parse(ddlPlayers.SelectedValue);
            int width;
            if (int.TryParse(tbWidth.Text, out width))
            {

                Width = width;
            }
            else
            {
                Width = 400;
            }
            int height;
            if (int.TryParse(tbHeight.Text, out height))
            {
                Height = height;
            }
            else
            {
                Height = 250;
            }

            _host.SaveWidgetDataMembers();
            currentMode = ViewMode.View;

        }
        else
        {
            currentMode = ViewMode.Edit;
        }
    }

    protected void btnSaveEmbedClk(object sender, EventArgs e)
    {
        EmbedCode = this.ReplaceEncodeBrackets(tbEmbed.Text);
        VideoID = 0;
        PlayerID = 0;
        _host.SaveWidgetDataMembers();

        currentMode = ViewMode.ViewEmbed;

        Response.Redirect(Request.Url.ToString());
    }

    protected void btnCancelSettingsClk(object sender, EventArgs e)
    {
        // Clear any fields that may have been populated prior to cancelling
        tbPublisherID.Text = "";
        tbReadToken.Text = "";
        tbWriteToken.Text = "";
        tbPlayerIds.Text = "";

        currentMode = ViewMode.Initial;
    }

    protected void btnCancelClk(object sender, EventArgs e)
    {

    }

    private void saveGlobalWidgetProperties()
    {
        List<GlobalWidgetPropertySettings> settingsList = new List<GlobalWidgetPropertySettings>();
        PropertyInfo[] globalProperties = this.GetType().GetProperties();
        foreach (PropertyInfo pi in globalProperties)
        {
            GlobalWidgetPropertySettings setting = new GlobalWidgetPropertySettings();

            GlobalWidgetData[] propertyAttributes = (GlobalWidgetData[])pi.GetCustomAttributes(typeof(GlobalWidgetData), true);
            if (propertyAttributes != null && propertyAttributes.Length > 0)
            {
                setting.PropertyName = pi.Name;
                setting.Type = pi.PropertyType;
                setting.value = pi.GetValue(this, null);
                settingsList.Add(setting);
            }
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<GlobalWidgetPropertySettings>));
        StringWriter s = new StringWriter();
        serializer.Serialize(s, settingsList);

        WidgetTypeData widgetData = new WidgetTypeData();
        WidgetTypeModel m_refWidgetModel = new WidgetTypeModel();
        m_refWidgetModel.FindByID(_host.WidgetInfo.ID, out widgetData);

        m_refWidgetModel.Update(widgetData.ID, widgetData.Title, widgetData.Title, widgetData.ControlURL, s.GetStringBuilder().ToString(), true);
    }

    protected ViewMode getViewMode()
    {
        ViewMode vMode = ViewMode.None;
        // Set up view
        if (showSetup || (String.IsNullOrEmpty(ReadToken) || PublisherID == 0 || String.IsNullOrEmpty(PlayerIDs)))
        {
            vMode = ViewMode.Initial;
            showSetup = true;

            // Show 'Edit Instructions' if the page is in Edit Mode
            if ((Page as PageBuilder) != null && (Page as PageBuilder).Status == Mode.Editing)
            {
                phInitialInstructions.Visible = true;
            }
        }
        else if (VideoID == 0 && String.IsNullOrEmpty(EmbedCode) && PlaylistID == 0)
        {
            vMode = ViewMode.NoVideo;

            // Show 'Edit Instructions' if the page is in Edit Mode
            if ((Page as PageBuilder) != null && (Page as PageBuilder).Status == Mode.Editing)
            {
                phNoVideoInstructions.Visible = true;
            }
        }
        else if (!String.IsNullOrEmpty(EmbedCode))
        {
            vMode = ViewMode.ViewEmbed;
        }
        else
        {
            vMode = ViewMode.View;
        }

        return vMode;
    }

    private void setActiveView(ViewMode vmode)
    {
        switch (vmode)
        {
            case ViewMode.Initial:
                BCViewSet.SetActiveView(Initial);
                break;
            case ViewMode.NoVideo:
                BCViewSet.SetActiveView(NoVideo);
                break;
            case ViewMode.View:
                BCViewSet.SetActiveView(View);
                break;
            case ViewMode.ViewEmbed:
                BCViewSet.SetActiveView(EmbedView);
                break;
            case ViewMode.EditSettings:
                BCViewSet.SetActiveView(Settings);
                break;
            case ViewMode.Edit:
            default:
                BCViewSet.SetActiveView(Edit);
                break;
        }
    }

    private void registerResources(ViewMode vmode)
    {
        switch (vmode)
        {
            case ViewMode.Initial:
                RegisterViewResources();
                break;
            case ViewMode.NoVideo:
                RegisterViewResources();
                break;
            case ViewMode.ViewEmbed:
                RegisterViewResources();
                break;
            case ViewMode.EditSettings:
                RegisterEditResources(false);
                break;
            case ViewMode.Edit:
                RegisterEditResources(true);
                break;
            case ViewMode.View:
            default:
                RegisterViewResources();
                break;
        }
    }

    private void RegisterViewResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS, true);
        //Ektron.Cms.API.JS.RegisterJSInclude(this, "http://admin.brightcove.com/js/BrightcoveExperiences_all.js", "BrightcoveExperiences_all");
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/BrightcoveVideo/js/brightcovevideoview.js", "EktronWidgetBrightcoveViewJS");
        string targetSelector = "#" + this.ClientID + "reloadVideo";
        string sourceSelector = "#" + this.ClientID + "myExperience";
        Ektron.Cms.API.JS.RegisterJSBlock(this, String.Format("Ektron.Widget.BrightcoveVideoView.ShowVideo('{0}','{1}');", sourceSelector, targetSelector), "jsblockview" + this.ClientID);
    }

    private void RegisterEditResources(bool isMainEditMode)
    {
        //Ektron.Cms.API.JS.RegisterJSInclude(this, "http://admin.brightcove.com/js/BrightcoveExperiences_all.js", "BrightcoveExperiences_allEdit");
        // Waiting on 8.5 for external ajax registration
        Ektron.Cms.API.Css.RegisterCss(this, sitepath + "widgets/BrightcoveVideo/css/brightcovevideo.css", "brightcovecss");
        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronClueTipJS);
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/BrightcoveVideo/js/jquery-1.5.2.min.js", "JQuery152Min");
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/BrightcoveVideo/js/jquery.tmpl.js", "JQTemplate");
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/BrightcoveVideo/js/ajaxfileupload.js", "AjaxFileLib");
        Ektron.Cms.API.JS.RegisterJSInclude(this, sitepath + "widgets/BrightcoveVideo/js/brightcovevideo.js", "EktronWidgetBrightcoveJS");
        string startUpScript = String.Format("Ektron.Widget.BrightcoveVideo.AddWidget('{0}',{1},'{2}','{3}',{4});", this.ClientID, isMainEditMode.ToString().ToLower(), _host.WidgetInfo.ID, this.sitepath, (this.WriteToken.Length > 0).ToString().ToLower());
        Ektron.Cms.API.JS.RegisterJSBlock(this, startUpScript, "jsblock" + this.ClientID);
    }

    protected string ReplaceEncodeBrackets(string encodetext)
    {
        encodetext = Regex.Replace(encodetext, "&lt;", "<");
        encodetext = Regex.Replace(encodetext, "&gt;", ">");
        return encodetext;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void SetOutput()
    {
        if (!String.IsNullOrEmpty(EmbedCode) && currentMode == ViewMode.ViewEmbed)
        {
            ltrEmbedCode.Text = EmbedCode;
        }

        if (!String.IsNullOrEmpty(PlayerIDs) && currentMode == ViewMode.Edit)
        {
            ddlPlayers.DataSource = PlayerIDs.Split(',');
            ddlPlayers.DataBind();
            if (PlayerID > 0)
            {
                ddlPlayers.SelectedValue = PlayerID.ToString();
            }

            tbWidth.Text = Width.ToString();
            tbHeight.Text = Height.ToString();
        }

        setActiveView(currentMode);
        registerResources(currentMode);

        if (!String.IsNullOrEmpty(WriteToken))
        {
            uploadLink.Visible = true;
        }
        else
        {
            uploadLink.Visible = false;
        }
    }

    private void CacheGlobalProperties()
    {
        Cache[playerIDsCacheKey] = this.PlayerIDs;
        Cache[publisherIDCacheKey] = this.PublisherID;
        Cache[readTokenCacheKey] = this.ReadToken;
        Cache[writeTokenCacheKey] = this.WriteToken;
    }

    protected enum ViewMode
    {
        None = 0,
        Initial = 1,
        NoVideo = 2,
        View = 3,
        ViewEmbed = 4,
        EditSettings = 5,
        Edit = 6
    }

}
