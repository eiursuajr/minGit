using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Widget;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Analytics;

public partial class AnalyticsReportWidget : WorkareaWidgetBaseControl, IWidget
{

    #region member variables

    private bool _editMode = false;
    private bool _dataLoaded = false;
    private bool _googledataLoaded = false;
    private bool _omnituredataLoaded = false;
    private bool _webtrendsdataLoaded = false;
    IAnalytics _dataManager = Ektron.Cms.ObjectFactory.GetAnalytics();

    #endregion

    #region properties

    private string _report = "";
    [WidgetDataMember("sam browser capabilities_sam browsers")]
    public string Report
    {
        get { return _report; }
        set { _report = value; }
    }
    
    private string _reportGUID = "";
    [WidgetDataMember("")]
    public string ReportGUID
    {
        get { return _reportGUID; }
        set { _reportGUID = value; }
    }

    private string _view = "";
    [WidgetDataMember("SmallTable")]
    public string View
    {
        get { return _view; }
        set { _view = value; }
    }

    private string _providerName = "";
    [WidgetDataMember("")]
    public string ProviderName
    {
        get { return _providerName; }
        set { _providerName = value; }
    }

    private string _siteName = "";
    [WidgetDataMember("")]
    public string SiteName
    {
        get { return _siteName; }
        set { _siteName = value; }
    }

    private string _period = string.Empty;
    [WidgetDataMember("")]
    public string Period
    {
        get { return _period; }
        set { _period = value; }
    }

    private string _providerSegments = string.Empty;
    [WidgetDataMember("")]
    public string ProviderSegments
    {
        get { return _providerSegments; }
        set { _providerSegments = value; }
    }
    #endregion

    #region protected methods

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ProviderSelect.OnProviderChanged += new Analytics_controls_ProviderSelector.ProviderChangedHandler(OnSelectedIndexChanged);

        base.Host.Edit += new EditDelegate(EditEvent);
        base.Host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        base.Host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        base.Host.Create += new CreateDelegate(delegate() { EditEvent(""); });

        AnalyticsViews.SetActiveView(ReportView);
        //preload the drop down lists for the client side event.
        List<string> siteProviders = _dataManager.GetProviderList();
        foreach (string p in siteProviders)
        {
            string providerType = _dataManager.GetProviderType(p);
            AddDataFromSubTreeControl(providerType, p);
            switch (providerType)
            {
                case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
                    SiteSelectGoogle.LoadSiteList(p);
                    break;
                case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                    SiteSelectSiteCatalyst.LoadSiteList(p);
                    break;
                case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                    SiteSelectWebTrends.LoadSiteList(p);
                    break;
            }
        }
        // add mode-select items:
        ViewSelect.Items.Add(new ListItem(GetMessage("summary text"), "Summary"));
        ViewSelect.Items.Add(new ListItem(GetMessage("generic timeline"), "TimeLine"));
        ViewSelect.Items.Add(new ListItem(GetMessage("lbl graph only"), "Graph"));
        ViewSelect.Items.Add(new ListItem(GetMessage("lbl small table"), "SmallTable"));
        ViewSelect.Items.Add(new ListItem(GetMessage("lbl large table"), "LargeTable"));

        // add items to show when viewselect is disabled (hidden), which is the case for all visitor trend reports:
        MinimalViewSelect.Items.Add(new ListItem(GetMessage("lbl bar graph"), "BarGraph"));

        // add period items:
        PeriodSelect.Items.Add(new ListItem(GetMessage("lbl last seven days"), "7"));
        PeriodSelect.Items.Add(new ListItem(GetMessage("lbl last thirty days"), "30"));
        PeriodSelect.Items.Add(new ListItem(GetMessage("lbl last ninety days"), "90"));

        Ektron.Cms.API.Css.RegisterCss(this, CommonAPIRef.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetTitle(GetMessage("lbl analytics report"));

        Ektron.Cms.API.JS.RegisterJSBlock(this, MakeClientScript(), this.ClientID + "_ClientScriptID");
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (String.IsNullOrEmpty(this.View))
        {
            this.View = ViewSelect.SelectedValue;
        }
        else
        {
            ViewSelect.SelectedValue = this.View;
        }

        if (string.IsNullOrEmpty(this.Period))
        {
            this.Period = PeriodSelect.SelectedValue;
        }
        else
        {
            PeriodSelect.SelectedValue = this.Period;
        }

        if (String.IsNullOrEmpty(this.ProviderName))
        {
            this.ProviderName = ProviderSelect.SelectedText;
        }
        else
        {
            ProviderSelect.SelectedText = this.ProviderName;
        }

        string providerType = _dataManager.GetProviderType(this.ProviderName);
        switch (providerType)
        {
            case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                if (String.IsNullOrEmpty(this.SiteName))
                {
                    this.SiteName = SiteSelectSiteCatalyst.SelectedText;
                }
                else
                {
                    SiteSelectSiteCatalyst.SelectedText = this.SiteName;
                }
                if (String.IsNullOrEmpty(this.Report))
                {
                    this.Report = ReportSelectSiteCatalyst.SelectedItemKey;
                }
                else
                {
                    ReportSelectSiteCatalyst.SelectedItemKey = this.Report;
                }
                break;
            case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                if (String.IsNullOrEmpty(this.SiteName))
                {
                    this.SiteName = SiteSelectWebTrends.SelectedText;
                }
                else
                {
                    SiteSelectWebTrends.SelectedText = this.SiteName;
                }
                this.ReportGUID = ReportSelectWebTrends.SelectedItemKey;
                if (String.IsNullOrEmpty(this.Report))
                {
                    this.Report = ReportSelectWebTrends.SelectedItemKey;
                }
                else
                {
                    ReportSelectWebTrends.SelectedItemKey = this.Report;
                } 
                break;
            case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
            default:
                if (String.IsNullOrEmpty(this.SiteName))
                {
                    this.SiteName = SiteSelectGoogle.SelectedText;
                }
                else
                {
                    SiteSelectGoogle.SelectedText = this.SiteName;
                }
                if (String.IsNullOrEmpty(this.Report))
                {
                    this.Report = ReportSelectGoogle.SelectedItemKey;
                }
                else
                {
                    ReportSelectGoogle.SelectedItemKey = this.Report;
                }
                break;
        }

        ShowSelectedReport();

        base.OnPreRender(e);
    }
    public void OnSelectedIndexChanged(object sender, System.EventArgs e)
    {
        //need to handle it in client side due to the strange life cycle of page builder pages.
        //in page builder widget, this changed event is only reached after the save button is clicked.  
        //it becomes too late for the consequence drop down changes.
    }

    protected void ShowSelectedReport()
    {
        if (_editMode)
            return;

		hdnSegment.Value = this.ProviderSegments;
        List<string> segmentIds = new List<string>();
        if (!string.IsNullOrEmpty(this.ProviderSegments))
        {
            foreach (string s in this.ProviderSegments.Split(','))
            {
                segmentIds.Add(s);
            }
        }

        AnalyticsReport.ProviderName = this.ProviderName;
        AnalyticsReport.ShowTable = false;
        AnalyticsReport.ShowPieChart = false;
        AnalyticsReport.ShowLineChart = false;
        AnalyticsReport.ShowSummaryChart = false;
        AnalyticsReport.ProviderSegments = segmentIds;

        AnalyticsTrend.ProviderName = this.ProviderName;
        AnalyticsTrend.ProviderSegments = segmentIds;

        int daysCount = 0;
        if (!string.IsNullOrEmpty(Period) && int.TryParse(Period, out daysCount))
        {
            DateTime startDate = DateTime.Today.AddDays(-1).AddDays(-daysCount);
            DateTime endDate = DateTime.Today.AddDays(-1);

            AnalyticsReport.StartDate = startDate;
            AnalyticsReport.EndDate = endDate;
            AnalyticsTrend.StartDate = startDate;
            AnalyticsTrend.EndDate = endDate;
        }

        switch (this.View)
        {
            case "Summary":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Detail;
                AnalyticsReport.ShowSummaryChart = true;
                break;

            case "TimeLine":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Detail;
                AnalyticsReport.ShowLineChart = true;
                break;

            case "Graph":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Percentage;
                AnalyticsReport.ShowPieChart = true;
                break;

            case "SmallTable":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Percentage;
                AnalyticsReport.ShowTable = true;
                break;

            case "LargeTable":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Table;
                AnalyticsReport.ShowTable = true;
                break;

            default:
                throw new ArgumentOutOfRangeException("ViewSelect", "Unknown view option: " + this.View);
        }

        switch (this.Report)
        {
            #region Google Analytics Reports
            ///////////////////////////////////////////////
            // no group:

            case "_sam overview":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Direct;
                break;


            ///////////////////////////////////////////////
            // Visitors:

            case "sam visitors_sam locations":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Locations;
                break;

            case "sam visitors_sam new vs returning":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.NewVsReturning;
                break;

            case "sam visitors_sam languages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Languages;
                break;

            case "sam visitors_sam user defined":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.UserDefined;
                break;


            ///////////////////////////////////////////////
            // Visitor Trending:

            case "sam visitor trending_sam visits":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.Visits;
                break;

            case "sam visitor trending_sam absolute unique visitors":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.AbsoluteUniqueVisitors;
                break;

            case "sam visitor trending_sam pageviews":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.Pageviews;
                break;

            case "sam visitor trending_sam average pageviews":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.AveragePageviews;
                break;

            case "sam visitor trending_sam time on site":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.TimeOnSite;
                break;

            case "sam visitor trending_sam bounce rate":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.BounceRate;
                break;


            ///////////////////////////////////////////////
            // Browser Capabilities:

            case "sam browser capabilities_sam browsers":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Browsers;
                break;

            case "sam browser capabilities_sam operating systems":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.OS;
                break;

            case "sam browser capabilities_sam browsers and os":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Platforms;
                break;

            case "sam browser capabilities_sam screen colors":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Colors;
                break;

            case "sam browser capabilities_sam screen resolutions":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Resolutions;
                break;

            case "sam browser capabilities_sam flash versions":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Flash;
                break;

            case "sam browser capabilities_sam java support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Java;
                break;


            ///////////////////////////////////////////////
            // Network Properties:

            case "sam network properties_sam network location":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.NetworkLocations;
                break;

            case "sam network properties_sam hostnames":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Hostnames;
                break;

            case "sam network properties_sam connection speeds":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ConnectionSpeeds;
                break;


            ///////////////////////////////////////////////
            // Traffic Sources:

            case "sam traffic sources_sam direct traffic":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Direct;
                break;

            case "sam traffic sources_sam referring sites":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Referring;
                break;

            case "sam traffic sources_sam search engines":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEngines;
                break;

            case "sam traffic sources_sam all traffic sources":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TrafficSources;
                break;

            case "sam traffic sources_sam keywords":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Keywords;
                break;

            case "sam traffic sources_sam campaigns":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Campaigns;
                break;

            case "sam traffic sources_sam ad versions":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.AdVersions;
                break;


            ///////////////////////////////////////////////
            // Content:

            case "sam content_sam top content":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TopContent;
                break;

            case "sam content_sam content by title":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ContentByTitle;
                break;

            case "sam content_sam top landing pages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TopLanding;
                break;

            case "sam content_sam top exit pages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TopExit;
                break;
            #endregion
            #region SiteCatalyst Reports
            case "sam site metrics_sam sitecatalyst trend page views":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.Pageviews;
                break;
            case "sam site metrics_sam sitecatalyst trend visits":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.Visits;
                break;
            case "sam visitors_sam sitecatalyst trend daily unique visitors":
                AnalyticsViews.SetActiveView(TrendView);
                AnalyticsTrend.Report = ReportType.DailyVisitors;
                break;
            case "sam site metrics_sam time spent per visit":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TimeVisitOnSite;
                break;
            case "sam site content_sam pages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Pages;
                break;
            case "sam site content_sam site sections":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SiteSection;
                break;
            case "sam site content_sam servers":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Server;
                break;
            case "sam links_sam exit links":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.LinkExit;
                break;
            case "sam links_sam file downloads":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.LinkDownload;
                break;
            case "sam site content_sam pages not found":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.PagesNotFound;
                break;
            case "sam mobile_sam devices":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileDeviceName;
                break;
            case "sam mobile_sam manufacturer":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileManufacturer;
                break;
            case "sam mobile_sam screen size":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileScreenSize;
                break;
            case "sam mobile_sam screen height":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileScreenHeight;
                break;
            case "sam mobile_sam screen width":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileScreenWidth;
                break;
            case "sam mobile_sam cookie support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileCookieSupport;
                break;
            case "sam mobile_sam image support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileImageSupport;
                break;
            case "sam mobile_sam color depth":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileColorDepth;
                break;
            case "sam mobile_sam audio support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileAudioSupport;
                break;
            case "sam mobile_sam video support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileVideoSupport;
                break;
            case "sam mobile_sam drm":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileDRM;
                break;
            case "sam mobile_sam net protocols":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileNetProtocols;
                break;
            case "sam mobile_sam operating system":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileOS;
                break;
            case "sam mobile_sam java version":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileJavaVM;
                break;
            case "sam mobile_sam bookmark url length":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileMaxBookmarkUrlLength;
                break;
            case "sam mobile_sam mail url length":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileMaxMailUrlLength;
                break;
            case "sam mobile_sam browser url length":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileMaxBroswerUrlLength;
                break;
            case "sam mobile_sam device number transmit (on/off)":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileDeviceNumberTransmit;
                break;
            case "sam mobile_sam ptt":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobilePushToTalk;
                break;
            case "sam mobile_sam decoration mail support":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileMailDecoration;
                break;
            case "sam mobile_sam information services":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MobileInformationServices;
                break;
            case "sam page analysis_sam reloads":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Reloads;
                break;
            case "sam page analysis_sam page depth":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.AveragePageDepth;
                break;
            case "sam page analysis_sam time spent on page":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TimeVisitOnPage;
                break;
            case "sam entries exits_sam entry pages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.EntryPage;
                break;
            case "sam entries exits_sam original entry pages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.EntryPageOriginal;
                break;
            case "sam traffic sources_sam search keywords - all":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEngineKeyword;
                break;
            case "sam traffic sources_sam search keywords - paid":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEnginePaidKeyword;
                break;
            case "sam traffic sources_sam search keywords - natural":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEngineNaturalKeyword;
                break;
            case "sam traffic sources_sam search engines - all":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SiteCatalystSearchEngine;
                break;
            case "sam traffic sources_sam search engines - paid":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEnginePaid;
                break;
            case "sam traffic sources_sam search engines - natural":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEngineNatural;
                break;
            case "sam traffic sources_sam all search page ranking":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SearchEngineNaturalPageRank;
                break;
            case "sam traffic sources_sam referring domains":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ReferringDomain;
                break;
            case "sam traffic sources_sam original referring domains":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ReferringDomainOriginal;
                break;
            case "sam traffic sources_sam referrers":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Referrer;
                break;
            case "sam traffic sources_sam referrer type":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ReferrerType;
                break;
            case "sam products_sam products":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Products;
                break;
            case "sam visitor retention_sam return frequency":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ReturnFrequency;
                break;
            case "sam visitor retention_sam visit number":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.VisitNumber;
                break;
            case "sam geosegmentation_sam countries":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.GeoCountries;
                break;
            case "sam geosegmentation_sam regions":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.GeoRegions;
                break;
            case "sam geosegmentation_sam cities":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.GeoCities;
                break;
            case "sam geosegmentation_sam u.s. dma":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.GeoDMA;
                break;
            case "sam visitor profile_sam visitor home page":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.HomePage;
                break;
            case "sam visitor profile_sam languages":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SiteCatalystLanguage;
                break;
            case "sam visitor profile_sam time zones":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TimeZone;
                break;
            case "sam visitor profile_sam domains":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Domain;
                break;
            case "sam visitor profile_sam top level domains":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.TopLevelDomain;
                break;
            case "sam technology_sam browsers":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SiteCatalystBrowsers;
                break;
            case "sam technology_sam browser types":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.BrowserType;
                break;
            case "sam technology_sam browser width":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.BrowserWidth;
                break;
            case "sam technology_sam browser height":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.BrowserHeight;
                break;
            case "sam technology_sam operating systems":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.SiteCatalystOperatingSystem;
                break;
            case "sam technology_sam monitor color depths":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MonitorColorDepth;
                break;
            case "sam technology_sam monitor resolutions":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.MonitorResolution;
                break;
            case "sam technology_sam java":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.JavaEnabled;
                break;
            case "sam technology_sam javascript":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.JavaScriptEnabled;
                break;
            case "sam technology_sam javascript version":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.JavaScriptVersion;
                break;
            case "sam technology_sam cookies":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.CookiesEnabled;
                break;
            case "sam technology_sam connection types":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.ConnectionTypes;
                break;
            case "sam visitor profile_sam visitor state":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.VisitorUSState;
                break;
            case "sam visitor profile_sam visitor zip/postal code":
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.VisitorZipCode;
                break;
            #endregion         
            default:
                if (!string.IsNullOrEmpty(this.ReportGUID))
                {
            #region WebTrends Reports
                    AnalyticsViews.SetActiveView(ReportView);
                    AnalyticsReport.ReportGUID = this.ReportGUID;
                    AnalyticsReport.Report = ReportType.WebTrendsReport;
                    break;
            #endregion
                }
            else
                {
                throw new ArgumentOutOfRangeException("this.Report", "Unknown Report: " + this.Report);
                }
            //AnalyticsViews.SetActiveView(ReportView);
            //AnalyticsReport.Report = ReportType.Direct;
            //break;
        }
    }

    protected void EditEvent(string settings)
    {
        AnalyticsViews.SetActiveView(EditView);
        _editMode = true;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        this.View = ViewSelect.SelectedValue;
        this.Period = PeriodSelect.SelectedValue;
        this.ProviderName = ProviderSelect.SelectedText;
        this.ProviderSegments = hdnSegment.Value;
        string providerType = _dataManager.GetProviderType(this.ProviderName);
        switch (providerType)
        {
            case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                this.SiteName = SiteSelectSiteCatalyst.SelectedText;
                this.Report = ReportSelectSiteCatalyst.SelectedItemKey;
                break;
            case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                this.SiteName = SiteSelectWebTrends.SelectedText;
                this.Report = ReportSelectWebTrends.SelectedItemKey;
                break;
            case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
            default:
                this.SiteName = SiteSelectGoogle.SelectedText;
                this.Report = ReportSelectGoogle.SelectedItemKey;
                break;
        }
        Host.SaveWidgetDataMembers();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
    }

    protected void AddDataFromSubTreeControl(string providerType, string providerName)
    {
        if (_dataLoaded)
            return;

        List<GroupedListItem> items = new List<GroupedListItem>();
        switch (providerType)
        {
            case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                AddDataFromPageHelper("", "", SiteCatalystContainer.TreeContainer, items, true);
                ReportSelectSiteCatalyst.Items = items;
                _omnituredataLoaded = true;
                break;
            case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                AddDataFromPageHelper("", "", WebTrendsContainer.GetTreeContainer(providerName), items, false);
                ReportSelectWebTrends.Items = items;
                _webtrendsdataLoaded = true;
                break;
            case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
            default:
                AddDataFromPageHelper("", "", GoogleAnalyticsContainer.TreeContainer, items, true);
                ReportSelectGoogle.Items = items;
                _googledataLoaded = true;
                break;
        }
        _dataLoaded = (_omnituredataLoaded && _webtrendsdataLoaded && _googledataLoaded);
    }

    protected void AddDataFromPageHelper(string groupName, string rawGroupName, Control control, List<GroupedListItem> items, bool needLocalize)
    {
        if (null == control)
            return;

        if (control.Controls.Count > 0)
        {
            string text, rawText;
            foreach (Control subControl in control.Controls)
            {
                if (IsNodeBranch(subControl))
                {
                    text = CleanText(GetNodeText(subControl));
                    rawText = string.IsNullOrEmpty(text) ? rawGroupName : text;
                    if (true == needLocalize)
                    {
                        text = string.IsNullOrEmpty(text) ? groupName : GetMessage(text);
                    }
                    if (!string.IsNullOrEmpty(text))
                        AddDataFromPageHelper(text, rawText, subControl, items, needLocalize);
                }
                else
                {
                    if (IsNodeLeaf(subControl))
                    {
                        rawText = CleanText(GetNodeText(subControl));
                        if (!string.IsNullOrEmpty(rawText))
                        {
                            if (true == needLocalize)
                            {
                                text = GetMessage(rawText);
                                items.Add(new GroupedListItem(text, rawGroupName + "_" + rawText, true, false, null, groupName));
                            }
                            else
                            {
                                text = rawText;
                                string value = rawGroupName + "_" + rawText;
                                if (((System.Web.UI.HtmlControls.HtmlControl)control).Attributes != null
                                    && ((System.Web.UI.HtmlControls.HtmlControl)control).Attributes.Count > 0)
                                {
                                    string href = ((System.Web.UI.HtmlControls.HtmlControl)subControl).Attributes["href"].ToString();
                                    string pattern = @"([\w\W]+\?report\=)([^&]*)(&[\w\W]*)*";
                                    Regex rgx = new Regex(pattern);
                                    value = rgx.Replace(href, "$2");
                                }
                                items.Add(new GroupedListItem(text, value, true, false, null, groupName));
                            }
                        }
                    }
                }
            }
        }
    }

    protected bool IsNodeLeaf(Control control)
    {
        if (control is System.Web.UI.HtmlControls.HtmlControl)
        {
            if (((System.Web.UI.HtmlControls.HtmlControl)control).TagName == "li" || ((System.Web.UI.HtmlControls.HtmlControl)control).TagName == "span")
            {
                if (((System.Web.UI.HtmlControls.HtmlControl)control).Attributes != null
                    && ((System.Web.UI.HtmlControls.HtmlControl)control).Attributes.Count > 0)
                {
                    return (!string.IsNullOrEmpty(((System.Web.UI.HtmlControls.HtmlControl)control).Attributes["href"]));
                }
            }
        }
        return false;
    }

    protected bool IsNodeBranch(Control control)
    {
        if (control.Controls != null)
            return control.Controls.Count > 1;

        return false;
    }

    protected string GetNodeText(Control control)
    {
        if (control.Controls != null && control.Controls.Count > 0)
        {
            if (control.Controls[0] is System.Web.UI.LiteralControl)
            {
                return ((System.Web.UI.LiteralControl)control.Controls[0]).Text ?? string.Empty;
            }
        }
        return string.Empty;
    }

    protected string CleanText(string rawText)
    {
        return rawText.Replace("\\n", "").Replace("\\r", "").Replace("\\t", "").Trim();
    }


    protected string MakeClientScript()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("    Ektron.ready(function () { Initialize_" + this.ClientID + "(); });" + Environment.NewLine);
        sb.Append("" + Environment.NewLine);
        sb.Append("    function Initialize_" + this.ClientID + "() {" + Environment.NewLine);
        sb.Append("        var editId = '" + editContainer.ClientID + "';" + Environment.NewLine);
        sb.Append("        var provider = $ektron('#" + editContainer.ClientID + " .ProviderSelectorList');" + Environment.NewLine);
        sb.Append("        $ektron('#" + editContainer.ClientID + " .ReportSelectGoogle').change(function () { ControlShowingPeriodSelect(editId, 'google'); });" + Environment.NewLine);
        sb.Append("        ControlShowingPeriodSelect(editId, 'google');" + Environment.NewLine);
        sb.Append("        $ektron('#" + editContainer.ClientID + " .ReportSelectSiteCatalyst').change(function () { ControlShowingPeriodSelect(editId, 'sitecatalyst'); });" + Environment.NewLine);
        sb.Append("        ControlShowingPeriodSelect(editId, 'sitecatalyst');" + Environment.NewLine);
        sb.Append("        $ektron('#" + editContainer.ClientID + " .ReportSelectWebTrends').change(function () { ControlShowingPeriodSelect(editId, 'webtrends'); });" + Environment.NewLine);
        sb.Append("        ControlShowingPeriodSelect(editId, 'webtrends');" + Environment.NewLine);
        sb.Append("        $ektron('#" + editContainer.ClientID + " .ProviderSelectorList').change(function () { ShowHideOtherSelectors(editId); });" + Environment.NewLine);
        sb.Append("        ShowHideOtherSelectors(editId);" + Environment.NewLine);
        sb.Append("    }" + Environment.NewLine);
        sb.Append("" + Environment.NewLine);
        sb.Append("    function ControlShowingPeriodSelect(editId, provider) {" + Environment.NewLine);
        sb.Append("         var select = null;" + Environment.NewLine);
        sb.Append("         switch (provider.toLowerCase()) {" + Environment.NewLine);
        sb.Append("             case \"sitecatalyst\":" + Environment.NewLine);
        sb.Append("                 select = $ektron('#' + editId + '.ReportSelectSiteCatalyst');" + Environment.NewLine);
        sb.Append("                 break;" + Environment.NewLine);
        sb.Append("             case \"webtrends\":" + Environment.NewLine);
        sb.Append("                 select = $ektron('#' + editId + '.ReportSelectWebTrends');" + Environment.NewLine);
        sb.Append("                 break; " + Environment.NewLine);
        sb.Append("             case \"google\":" + Environment.NewLine);
        sb.Append("             default:" + Environment.NewLine);
        sb.Append("                 select = $ektron('#' + editId + '.ReportSelectGoogle');" + Environment.NewLine);
        sb.Append("                 break; " + Environment.NewLine);
        sb.Append("         }" + Environment.NewLine);
        sb.Append("        if (0 == select.length)" + Environment.NewLine);
        sb.Append("            return;" + Environment.NewLine);
        //sb.Append("alert(\"1\" + select.val());" + Environment.NewLine);
        sb.Append("        if (select.val().indexOf('trend') > 3) {" + Environment.NewLine);// always starts with 'sam '
        sb.Append("            $ektron('#' + editId + '.ViewSelect').hide();" + Environment.NewLine);
        sb.Append("            $ektron('#' + editId + '.MinimalViewSelect').show();" + Environment.NewLine);
        sb.Append("        } else {" + Environment.NewLine);
        sb.Append("            $ektron('#' + editId + '.MinimalViewSelect').hide();" + Environment.NewLine);
        sb.Append("            $ektron('#' + editId + '.ViewSelect').show();" + Environment.NewLine);
        sb.Append("        }" + Environment.NewLine);
        sb.Append("    }" + Environment.NewLine);
        sb.Append("" + Environment.NewLine);
        sb.Append("    function ShowHideOtherSelectors(editId) {" + Environment.NewLine);
        sb.Append("        var eContainer = $ektron('div#' + editId);" + Environment.NewLine);
        sb.Append("        var select = $ektron('select.ProviderSelectorList', eContainer);" + Environment.NewLine);
        sb.Append("        if (0 == select.length)" + Environment.NewLine);
        sb.Append("            return;" + Environment.NewLine);
        sb.Append("" + Environment.NewLine);
        //sb.Append("alert(\"2\" + editId);" + Environment.NewLine);
        sb.Append("        switch (select.val()) {" + Environment.NewLine);
        List<string> siteProviders = _dataManager.GetProviderList();
        foreach (string p in siteProviders)
        {
            string providerType = _dataManager.GetProviderType(p);
            switch (providerType)
            {
                case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
                    sb.Append("             case \"" + p + "\":" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectGoogle', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectSiteCatalyst', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectWebTrends', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectGoogle', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectSiteCatalyst', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectWebTrends', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('input.SegmentPopupBtn', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 break;" + Environment.NewLine);
                    break;
                case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                    sb.Append("             case \"" + p + "\":" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectSiteCatalyst', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectGoogle', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectWebTrends', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectSiteCatalyst', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectGoogle', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectWebTrends', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('input.SegmentPopupBtn', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 break;" + Environment.NewLine);
                    break;
                case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                    sb.Append("             case \"" + p + "\":" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectWebTrends', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectGoogle', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('select.ReportSelectSiteCatalyst', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectWebTrends', eContainer).show();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectGoogle', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('span.SiteSelectSiteCatalyst', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 $ektron('input.SegmentPopupBtn', eContainer).hide();" + Environment.NewLine);
                    sb.Append("                 break; " + Environment.NewLine);
                    break;
            }
        }
        sb.Append("         }" + Environment.NewLine);
        sb.Append("    }" + Environment.NewLine);
        sb.Append("" + Environment.NewLine);
        sb.Append("    // ensure that initialization runs, even when loaded by dragging onto a pagebuilder page:" + Environment.NewLine);
        sb.Append("    Initialize_" + this.ClientID + "();" + Environment.NewLine);
        return sb.ToString();
    }


    #endregion
}
