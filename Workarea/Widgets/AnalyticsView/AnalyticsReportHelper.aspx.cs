using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Analytics.Reporting;

public partial class Widgets_AnalyticsView_AnalyticsReportHelper : System.Web.UI.Page {

    #region member variables

    private string _view = "Summary";
    private string _report = "_sam overview";
    private string _providerName = "Google";
    private DateTime _startDate = DateTime.Today.AddDays(-30);
    private DateTime _endDate = DateTime.Today.AddDays(-1);

    #endregion

    #region properties

    #endregion

    #region protected methods

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);

        // recover querystring parameters:
        string temp;
        DateTime tempDate;
        temp = Request.QueryString["provider"];
        if (!string.IsNullOrEmpty(temp))
            _providerName = Page.Server.UrlDecode(temp);

        temp = Request.QueryString["report"];
        if (!string.IsNullOrEmpty(temp))
            _report = Page.Server.UrlDecode(temp);

        temp = Request.QueryString["view"];
        if (!string.IsNullOrEmpty(temp))
            _view = Page.Server.UrlDecode(temp);

        temp = Request.QueryString["startdate"];
        if (!string.IsNullOrEmpty(temp)) {
            if (DateTime.TryParse(Page.Server.UrlDecode(temp), out tempDate))
                _startDate = tempDate;
        }

        temp = Request.QueryString["enddate"];
        if (!string.IsNullOrEmpty(temp)) {
            if (DateTime.TryParse(Page.Server.UrlDecode(temp), out tempDate))
                _endDate = tempDate;
        }

        //Ektron.Cms.API.Css.RegisterCss(this, CommonAPIRef.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
    }

    protected void Page_Load(object sender, EventArgs e) {
        AnalyticsReport.ShowDateRange = AnalyticsTrend.ShowDateRange = true;
        ShowSelectedReport();
    }

    protected void ShowSelectedReport() {
        AnalyticsReport.ProviderName = _providerName;
        AnalyticsTrend.ProviderName = _providerName;
        AnalyticsReport.ShowTable = false;
        AnalyticsReport.ShowPieChart = false;
        AnalyticsReport.ShowLineChart = false;
        AnalyticsReport.ShowSummaryChart = false;

        AnalyticsReport.StartDate = _startDate;
        AnalyticsReport.EndDate = _endDate;
        AnalyticsTrend.StartDate = _startDate;
        AnalyticsTrend.EndDate = _endDate;

        switch (_view) {
            case "Summary":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Detail;
                AnalyticsReport.ShowSummaryChart = true;
                AnalyticsReportHelper_Container.Style.Add("min-width", "360px");
                break;

            case "TimeLine":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Detail;
                AnalyticsReport.ShowLineChart = true;
                AnalyticsReportHelper_Container.Style.Add("min-width", "420px");
                break;

            case "Graph":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Percentage;
                AnalyticsReport.ShowPieChart = true;
                AnalyticsReportHelper_Container.Style.Add("min-width", "380px");
                break;

            case "SmallTable":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Percentage;
                AnalyticsReport.ShowTable = true;
                AnalyticsReportHelper_Container.Style.Add("min-width", "600px");
                break;

            case "LargeTable":
                AnalyticsReport.View = Analytics_reporting_Report.DisplayView.Table;
                AnalyticsReport.ShowTable = true;
                AnalyticsReportHelper_Container.Style.Add("min-width", "600px");
                break;

            default:
                throw new ArgumentOutOfRangeException("ViewSelect", "Unknown view option: " + _view);
        }

        switch (_report) {

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


            default:
                AnalyticsViews.SetActiveView(ReportView);
                AnalyticsReport.Report = ReportType.Direct;
                break;
        }
    }
    #endregion
}
