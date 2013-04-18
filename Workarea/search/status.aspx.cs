using System;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Search;
using Ektron.Cms.Search.SearchRequestService;

public partial class Workarea_search_status : System.Web.UI.Page
{
    private const string ActionParameter = "action";
    private const string IncrementalCrawlAction = "incremental";
    private const string FullCrawlAction = "full";
    private readonly DateTime MinDate = new DateTime(1899, 12, 30, 0, 0, 0);

    private SiteAPI _site;
    private StyleHelper _style;
    private SearchServerCrawler _crawler;

    public SiteAPI Sites
    {
        get
        {
            if (_site == null) _site = new SiteAPI();
            return _site;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        ltrincremental.Text = this.Sites.EkMsgRef.GetMessage("js incremental crawl");
        ltrincrementalRequest.Text = this.Sites.EkMsgRef.GetMessage("js incremental crawl request");
        ltrFullC.Text = this.Sites.EkMsgRef.GetMessage("js full crawl");
        ltrFullRequest.Text = this.Sites.EkMsgRef.GetMessage("js full crawl request");

        if (HasPermission())
        {
            _style = new StyleHelper();
            _crawler = new SearchServerCrawler();

            JS.RegisterJS(this, JS.ManagedScript.EktronStyleHelperJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaContextMenusJS);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE8);

            styleHelper.Text = _style.GetClientScript();

            RenderToolbar();
            RenderTitleBar();

            if (!string.IsNullOrEmpty(Request.QueryString[ActionParameter]))
            {
                switch (Request.QueryString[ActionParameter].ToLower())
                {
                    case IncrementalCrawlAction:
                        _crawler.StartIncrementalCrawl(false);
                        Response.Redirect("Status.aspx");
                        break;
                    case FullCrawlAction:
                        _crawler.StartFullCrawl();
                        Response.Redirect("Status.aspx");
                        break;
                }
            }

            try
            {
                RenderState();
                RenderSettings();
            }
            catch (NullReferenceException)
            {
                Utilities.ShowError(Sites.EkMsgRef.GetMessage("msg search status connection error"));
            }
        }
        else
        {
            Response.Redirect(ContentAPI.Current.ApplicationPath + "Login.aspx", true);
        }
    }

    private void RenderState()
    {
        ServerState state = _crawler.GetState();

        if (state != null)
        {
            ucContentSourceName.Text = state.ContentSourceName;
            ucCrawlDuration.Text = state.CrawlDuration.ToString("hh\\:mm\\:ss");
            ucCrawlEndTime.Text = GetTimeString(state.CrawlEndTime);
            ucCrawlInterval.Text = state.CrawlInterval.ToString();
            ucCrawlStartTime.Text = GetTimeString(state.CrawlStartTime);
            ucIsCrawlSchedule.Text = state.IsCrawlScheduled ? this.Sites.EkMsgRef.GetMessage("generic yes") : this.Sites.EkMsgRef.GetMessage("generic no");

            switch (state.CurrentAction)
            {
                case ContentSourceAction.Idle:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("txt quickdeploy idle");
                    break;
                case ContentSourceAction.IncrementalCrawl:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Incremental Crawl");
                    break;
                case ContentSourceAction.FullCrawl:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Full Crawl");
                    break;
                case ContentSourceAction.Mapping:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Property Mapping");
                    break;
                case ContentSourceAction.PreMappingCrawl:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Property Creation");
                    break;      
            }

            switch (state.PendingAction)
            {
                case ContentSourceAction.Idle:
                    ucPendingAction.Text = this.Sites.EkMsgRef.GetMessage("text decoration none");
                    break;
                case ContentSourceAction.IncrementalCrawl:
                    ucPendingAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Incremental Crawl");
                    break;
                case ContentSourceAction.FullCrawl:
                    ucPendingAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Full Crawl");
                    break;
                case ContentSourceAction.Mapping:
                    ucPendingAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Property Mapping");
                    break;
                case ContentSourceAction.PreMappingCrawl:
                    ucCurrentAction.Text = this.Sites.EkMsgRef.GetMessage("lbl Property Creation");
                    break;      
            }
        }
        else
        {
            Utilities.ShowError(Sites.EkMsgRef.GetMessage("msg search status connection error"));
        }
    }

    private void RenderSettings()
    {
        ISearchSettings searchSettings = ObjectFactory.GetSearchSettings();
        SearchSettingsData searchSettingsData = searchSettings.GetItem();

        ucSearchServer.Text = searchSettingsData.SearchServer;

        string username = string.Empty;
        if (!string.IsNullOrWhiteSpace(searchSettingsData.Domain))
        {
            ucUsername.Text = searchSettingsData.Domain + "\\";
        }

        ucUsername.Text += searchSettingsData.Username;

        ucIncludeHtmlContent.Text = GetFilterString(searchSettingsData.Filter.IncludeHtmlContent);
        ucIncludeDocumentContent.Text = GetFilterString(searchSettingsData.Filter.IncludeDocuments);
        ucIncludeForumContent.Text = GetFilterString(searchSettingsData.Filter.IncludeForums);
        ucIncludeProductContent.Text = GetFilterString(searchSettingsData.Filter.IncludeProducts);
        ucIncludeCommunityMembers.Text = GetFilterString(searchSettingsData.Filter.IncludeCommunityMembers);
        ucIncludeCommunityContent.Text = GetFilterString(searchSettingsData.Filter.IncludeCommunityContent);
    }

    private void RenderTitleBar()
    {
        txtTitleBar.InnerHtml = _style.GetTitleBar(this.Sites.EkMsgRef.GetMessage("msg view search status"));
    }

    private void RenderToolbar()
    {
        tdIncrementalCrawlButton.InnerHtml = _style.GetButtonEventsWCaption(
            this.Sites.AppImgPath + "../UI/Icons/controlRepeat.png", 
            "javascript:crawlIncremental();",
            this.Sites.EkMsgRef.GetMessage("msg search incremental crawl button"),
            this.Sites.EkMsgRef.GetMessage("msg search incremental crawl button"),
            "",
			StyleHelper.CrawlIncrementalButtonCssClass);

        tdFullCrawlButton.InnerHtml = _style.GetButtonEventsWCaption(
            this.Sites.AppImgPath + "../UI/Icons/controlRepeatBlue.png",
            "javascript:crawlFull();",
            this.Sites.EkMsgRef.GetMessage("msg search full crawl button"),
            this.Sites.EkMsgRef.GetMessage("msg search full crawl button"),
            "",
			StyleHelper.CrawlFullButtonCssClass);

        tdRefreshButton.InnerHtml = _style.GetButtonEventsWCaption(
            this.Sites.AppImgPath + "../UI/Icons/refresh.png",
            "status.aspx",
            this.Sites.EkMsgRef.GetMessage("generic refresh"),
            this.Sites.EkMsgRef.GetMessage("generic refresh"),
            "",
			StyleHelper.RefreshButtonCssClass);
		
		tdSearchStatusHelpButton.InnerHtml = _style.GetHelpButton("crawl", "");
    }

    private string GetTimeString(DateTime input)
    {
        string timeString;
        if (input.Equals(MinDate))
        {
            timeString = this.Sites.EkMsgRef.GetMessage("generic NA");
        }
        else
        {
            timeString = input.ToString();
        }

        return timeString;
    }

    public string GetFilterString(bool input)
    {
        return input ? this.Sites.EkMsgRef.GetMessage("generic Included") : this.Sites.EkMsgRef.GetMessage("generic Excluded");
    }

    /// <summary>
    /// Returns true if the current user has sufficient permissions to access
    /// the functionality on this page, false otherwise.
    /// </summary>
    /// <returns>True if permissions are sufficient, false otherwise</returns>
    private static bool HasPermission()
    {
        return
            !((Convert.ToBoolean(ContentAPI.Current.RequestInformationRef.IsMembershipUser) ||
            ContentAPI.Current.RequestInformationRef.UserId == 0 ||
            !ContentAPI.Current.EkUserRef.IsAllowed(ContentAPI.Current.UserId, 0, "users", "IsAdmin", 0)) &&
            !(ContentAPI.Current.IsARoleMember((long)EkEnumeration.CmsRoleIds.SearchAdmin, ContentAPI.Current.UserId, false)));
    }
}
