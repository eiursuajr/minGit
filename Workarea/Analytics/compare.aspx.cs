using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Interfaces.Analytics.Provider;

public partial class Analytics_compare : workareabase {

    private long itemId = 0;
    private long versionId1 = 0;
    private long versionId2 = 0;
    private List<AnalyticsReportData> reports1 = null;
    private List<AnalyticsReportData> reports2 = null;
    private IAnalytics dataManager = ObjectFactory.GetAnalytics();
    private string errDataManager = "";
    private DateTime _startDate1;
    private DateTime _endDate1;
    private DateTime _startDate2;
    private DateTime _endDate2;
    private string version1 = "";
    private string version2 = "";
    private string _provider = "";
    private string _providerType = "";
    private string _site = "";
    private static List<string> _updateList = null;
    private bool _hasProviderChanged = false;
    private bool _showVisits = false;
    private bool _showPageviews = false;
    private bool _showUniqueViews = false;
    private bool _showTimeOnSite = false;
    private bool _showTimeOnPage = false;
    private bool _showBounceRate = false;
    private bool _showPercentExit = false;
    private string _segmentPersistenceId = string.Empty;

    private enum DisplayMetric
    {
        Visits,
        Pageviews,
        UniqueViews,
        TimeOnSite,
        TimeOnPage,
        BounceRate,
        PercentExit
    }

    #region protected properties

    #endregion

    protected List<string> CookieSegments
    {
        get
        {
            List<string> segmentIds = new List<string>();
            HttpCookie cookie = Request.Cookies[_segmentPersistenceId];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                foreach (string s in cookie.Value.Split(','))
                {
                    segmentIds.Add(s);
                }
            }
            return segmentIds;
        }
        set
        {
            string idList = string.Join(",", value.ConvertAll<string>(delegate(string i) { return i; }).ToArray());
            HttpCookie cookie = new HttpCookie(_segmentPersistenceId, idList);
            cookie.Expires = System.DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            Response.Cookies.Add(cookie);
        }
    }

    public delegate void SelectionChangedHandler(object sender, EventArgs e);
    public event SelectionChangedHandler SelectionChanged;

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        MetricSelector.MetricSelectors = Analytics_reporting_MetricSelector.SelectorCount.Single;
        ProviderSelect.OnProviderChanged += ProviderChangedHandler;
    }

    protected override void OnLoad(EventArgs e)
    {
 	    base.OnLoad(e);
        try {
            if (!IsPostBack) {
                litLoadingMessage.Text = GetMessage("generic loading"); // TODO should be label w/o viewstate
            }

            AnalyticsSecurity.Guard(RequestInformationRef);
            InitializeDatePickers();
            ObtainValues();

            //if ("localhost" == RequestInformationRef.HostUrl)
            //{
            //    ltr_error.Text = GetMessage("err hostname could not be parsed");
            //    errGAMsg.Visible = true;
            //}

            // abort if error: // TODO: Tie datepicker into ASP.NET Validation?
            if (errGAMsg.Visible)
                return;

            btnRefresh.AlternateText = GetMessage("generic refresh");
            btnRefresh.Attributes.Add("title", btnRefresh.AlternateText);
            CommonApi api = new CommonApi();
            btnRefresh.ImageUrl = api.AppImgPath + "refresh.png";
            SiteSelect.ProviderName = ProviderSelect.ProviderName;
            if (!string.IsNullOrEmpty(ProviderSelect.ProviderName))
            {
                _segmentPersistenceId = dataManager.GetSegmentFilterCookieName(ProviderSelect.ProviderName);
            }
        }
        catch (Exception ex) {
            ltr_error.Text = ex.Message;
            errGAMsg.Visible = true;
            ComparisonTimeLineChart.Visible = false;
            SelectorFilterRow.Visible = false;
            CaptionRow.Visible = false;
            SummaryRow.Visible = false;
        }

    }

    protected override void OnLoadComplete(EventArgs e)
    {
 	    base.OnLoadComplete(e);
    
        Page.Validate();
        ComparisonTimeLineChart.Visible = Page.IsValid;
        SummaryCharts1.Visible = Page.IsValid;
        SummaryCharts2.Visible = Page.IsValid;

        if (Page.IsValid)
        {
            if (!string.IsNullOrEmpty(_provider))
            {
                GetReports();
            }
            if (reports1 != null && reports1.Count > 0)
            {
                this.UpdateAvailableMetric(ProviderSelect.ProviderName);
                PopulateGraphs();
            }
        }
    }

    public void BadDateFormatErrorHandler(string defaultMessage, string rawDate) {
        ltr_error.Text = defaultMessage; // TODO: Tie datepicker into ASP.NET Validation?
        errGAMsg.Visible = true;
    }

    public void BadDateRangeErrorHandler(object sender, BadDateRangeEventArgs e)
    {
        ltr_error.Text = e.Message;
        errGAMsg.Visible = true;
    }

    public void ProviderChangedHandler(object sender, Analytics_controls_ProviderSelector.ProviderChangedEventArgs e)
    {
        ProviderSelect.ProviderName = e.ProviderName;
        _hasProviderChanged = true;
        UpdateAvailableMetric(e.ProviderName);
    }

    private void UpdateAvailableMetric(string provider)
    {
        string providerType = dataManager.GetProviderType(provider);
        switch (providerType.ToString())
        {
            case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
                _showVisits = false;
                _showPageviews = true;
                _showUniqueViews = true;
                _showTimeOnSite = false;
                _showTimeOnPage = true;
                _showBounceRate = true;
                _showPercentExit = true;
                break;
            case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                _showVisits = true;
                _showPageviews = true;
                _showUniqueViews = false;
                _showTimeOnSite = true;
                _showTimeOnPage = false;
                _showBounceRate = false;
                _showPercentExit = false;
                break;
            default:
                throw new NotImplementedException();
        }
        SummaryCharts1.ShowVisits = _showVisits;
        SummaryCharts1.ShowPagesPerVisit = false;
        SummaryCharts1.ShowPageviews = _showPageviews;
        SummaryCharts1.ShowUniqueViews = _showUniqueViews;
        SummaryCharts1.ShowTimeOnSite = _showTimeOnSite;
        SummaryCharts1.ShowTimeOnPage = _showTimeOnPage;
        SummaryCharts1.ShowBounceRate = _showBounceRate;
        SummaryCharts1.ShowPercentExit = _showPercentExit;
        SummaryCharts2.ShowVisits = _showVisits;
        SummaryCharts2.ShowPagesPerVisit = false;
        SummaryCharts2.ShowPageviews = _showPageviews;
        SummaryCharts2.ShowUniqueViews = _showUniqueViews;
        SummaryCharts2.ShowTimeOnSite = _showTimeOnSite;
        SummaryCharts2.ShowTimeOnPage = _showTimeOnPage;
        SummaryCharts2.ShowBounceRate = _showBounceRate;
        SummaryCharts2.ShowPercentExit = _showPercentExit;
    }

    protected void btnRefresh_Click(object sender, EventArgs e) 
    {
        if (SelectionChanged != null)
        {
            SelectionChanged(this, e);
        }
    } 

    protected void InitializeDatePickers() {
        DateRangePicker1.BadDateRange += BadDateRangeErrorHandler;
        DateRangePicker1.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker1.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker1.BadStartDateFormatMessage = GetMessage("msg bad start date format");
        DateRangePicker1.BadEndDateFormatMessage = GetMessage("msg bad end date format");
        DateRangePicker2.BadDateRange += BadDateRangeErrorHandler;
        DateRangePicker2.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker2.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker2.BadStartDateFormatMessage = GetMessage("msg bad start date format");
        DateRangePicker2.BadEndDateFormatMessage = GetMessage("msg bad end date format");
        DateRangePicker1.MaximumDate = DateTime.Today;
        DateRangePicker2.MaximumDate = DateTime.Today;
    }

    protected void ObtainValues() 
    {
        if (!IsPostBack) 
        {
            VersionCompare comparisonDates;
            if (Request.QueryString["itemid"] != null) 
            {
                itemId = EkFunctions.ReadDbLong(Request.QueryString["itemid"]);
                if (Request.QueryString["oldid"] != null)
                    versionId1 = EkFunctions.ReadDbLong(Request.QueryString["oldid"]);
                if (Request.QueryString["diff"] != null)
                    versionId2 = EkFunctions.ReadDbLong(Request.QueryString["diff"]);

                if (Request.QueryString["oldver"] != null)
                    version1 = EkFunctions.ReadDbString(Request.QueryString["oldver"]);
                if (Request.QueryString["ver"] != null)
                    version2 = EkFunctions.ReadDbString(Request.QueryString["ver"]);

                string versionNum = GetMessage("lbl version number");
                Caption1.Text = String.Format(versionNum, version1);
                Caption2.Text = String.Format(versionNum, version2);

                comparisonDates = dataManager.GetVersionDates(itemId, versionId1, versionId2);

                string title = String.Format(GetMessage("lbl compare analytics for"), comparisonDates.ContentTitle);
                Page.Title = title;
                this.SetTitleBarToString(title);
            } 
            else 
            {
                // for debugging when itemid is not available
                comparisonDates = new VersionCompare("dummy", new DateTime(2009, 7, 15), new DateTime(2009, 7, 25), new DateTime(2009, 7, 26), new DateTime(2009, 8, 5));
            }

            DateRangePicker1.StartDate = comparisonDates.BaseVersion.StartDate;
            DateRangePicker1.EndDate = comparisonDates.BaseVersion.EndDate;
            DateRangePicker2.StartDate = comparisonDates.ComparisonVersion.StartDate;
            DateRangePicker2.EndDate = comparisonDates.ComparisonVersion.EndDate;
        }
        _startDate1 = DateRangePicker1.StartDate;
        _endDate1 = DateRangePicker1.EndDate;
        _startDate2 = DateRangePicker2.StartDate;
        _endDate2 = DateRangePicker2.EndDate;

        _site = RequestInformationRef.HostUrl;
        if (_site.Length > 0) // && (null == _updateList || "" == ProviderSelect.SelectedText))
        {
            List<string> siteList = new List<string>();
            siteList.Add(_site);
            SiteSelect.SiteList = siteList;
            List<string> ProviderList = dataManager.GetSiteProviders(_site);
            if (!IsPostBack || null == _updateList)
            {
                _updateList = new List<string>();
                foreach (string provider in ProviderList)
                {
                    string providerType = dataManager.GetProviderType(provider);
                    if ("Ektron.Cms.Analytics.Providers.SiteCatalystProvider" != providerType)
                    {
                        _updateList.Add(provider);
                    }
                }
                if (_updateList.Count > 0)
                {
                    _updateList.Sort();
                    ProviderSelect.ProviderList = _updateList;
                    _provider = ProviderSelect.SelectedText;
                    _providerType = dataManager.GetProviderType(_provider);
                    UpdateAvailableMetric(_provider);
                }
            } 
        }
        _provider = ProviderSelect.SelectedText;
        _providerType = dataManager.GetProviderType(_provider);
    }

    protected void GetReports() 
    {
        IDimensions _dimensions = dataManager.GetDimensions(_provider);//todo: test with different provider
        AnalyticsCriteria criteria = new AnalyticsCriteria();
        criteria.DimensionFilters.Condition = LogicalOperation.Or;
        foreach (string pagePath in UrlFilter.PagePaths)
        {
            Dimension d = null;
            switch (_providerType)
            {
                case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                    d = _dimensions.pages;
                    break;
                case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
                    d = _dimensions.pagePath;
                    break;
                default:
                    throw new NotImplementedException();
                    
            }
            if (d != null)
            {
                criteria.DimensionFilters.AddFilter(d, DimensionFilterOperator.EqualTo, pagePath);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        try 
        {
            if (_provider != "" && dataManager.HasProvider(_provider))
            {
                reports1 = this.GetReportDataList(_startDate1, _endDate1, criteria);
                reports2 = this.GetReportDataList(_startDate2, _endDate2, criteria); 
            }
            else
            {
                reports1 = new List<AnalyticsReportData>();
                reports2 = new List<AnalyticsReportData>();
            }
            if (reports1.Count == 0 && reports2.Count == 0)
            {
                this.ltr_error.Text = GetMessage("err hostname no stats");
                this.errGAMsg.Visible = true;
            }
        }
        catch (Exception ex) 
        {
            if (ex.Message.Contains("(401)")) 
            {
                errDataManager = GetMessage("err analytics data provider");
            } else 
            {
                errDataManager = ex.Message;
            }

            this.ltr_error.Text = errDataManager;
            this.errGAMsg.Visible = true;
        }
    }

    private List<AnalyticsReportData> GetReportDataList(DateTime startDate, DateTime endDate, AnalyticsCriteria criteria)
    {
        AnalyticsReportData oneReport = null;
        List<AnalyticsReportData> reports = new List<AnalyticsReportData>();
        if (this.CookieSegments.Count > 0)
        {
            foreach (string segIdPair in this.CookieSegments)
            {
                string segVal = segIdPair.Substring(0, segIdPair.IndexOf("|"));
                string sSegProp = segIdPair.Replace(segVal + "|", "");
                SegmentProperty segProp = (SegmentProperty)Convert.ToInt32(sSegProp);
                criteria.SegmentFilter = new SegmentFilter(segProp, SegmentFilterOperator.EqualTo, segVal);
                oneReport = dataManager.GetContentDetail(_provider, startDate, endDate, criteria);
                if (oneReport != null)
                {
                    reports.Add(oneReport);
                }
            }
        }
        else
        {
            // no cookieSegments
            oneReport = dataManager.GetContentDetail(_provider, startDate, endDate, criteria);
            if (oneReport != null)
            {
                reports.Add(oneReport);
            }
        }
        return reports;
    }

    protected void PopulateGraphs() {
        if (!IsPostBack || _hasProviderChanged) 
        {
            List<ListItem> metricItems = new List<ListItem>();
            if (_showVisits) metricItems.Add(new ListItem(GetMessage("lbl visits"), DisplayMetric.Visits.ToString()));
            if (_showPageviews) metricItems.Add(new ListItem(GetMessage("lbl pageviews"), DisplayMetric.Pageviews.ToString()));
            if (_showUniqueViews) metricItems.Add(new ListItem(GetMessage("lbl unique views"), DisplayMetric.UniqueViews.ToString()));
            if (_showTimeOnSite) metricItems.Add(new ListItem(GetMessage("lbl time on site"), DisplayMetric.TimeOnSite.ToString()));
            if (_showTimeOnPage) metricItems.Add(new ListItem(GetMessage("lbl time on page"), DisplayMetric.TimeOnPage.ToString()));
            if (_showBounceRate) metricItems.Add(new ListItem(GetMessage("lbl bounce rate"), DisplayMetric.BounceRate.ToString()));
            if (_showPercentExit) metricItems.Add(new ListItem(GetMessage("lbl percent exit"), DisplayMetric.PercentExit.ToString()));
            MetricSelector.Items = metricItems.ToArray();
            _hasProviderChanged = false;
        }
        DisplayMetric metric;
        string strMetric = MetricSelector.SelectedValue;
        if (Enum.IsDefined(typeof(DisplayMetric), strMetric))
        {
            metric = (DisplayMetric)Enum.Parse(typeof(DisplayMetric), strMetric);
        } else {
            metric = DisplayMetric.Pageviews;
        }

        List<int>[] visitsOverTime = { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>() };
        List<int>[] pageViewsOverTime = { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>() };
        List<int>[] uniqueViewsOverTime = { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>() };
        List<TimeSpan>[] averageTimeOnSiteOverTime = { new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>() };
        List<TimeSpan>[] averageTimeOnPageOverTime = { new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>(), new List<TimeSpan>() };
        List<float>[] bounceRateOverTime = { new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>() };
        List<float>[] percentExitOverTime = { new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>() };

        int[] days = { (int)(_endDate1 - _startDate1).TotalDays + 1,
					 (int)(_endDate2 - _startDate2).TotalDays + 1};
        DateTime[] reportStartDates = {_startDate1,
						  _startDate2};
        string[] SegmentLabels = new string[8];
        string strSet1 = GetMessage("lbl set 1");
        string strSet2 = GetMessage("lbl set 2");
        if (this.CookieSegments.Count > 1)
        {
            for (int i = 0; i < this.CookieSegments.Count; i++)
            {
                string segName = reports1[i].Segment.Name;
                int labelIndex = i * 2;
                SegmentLabels[labelIndex] = segName + " " + strSet1;
                SegmentLabels[labelIndex + 1] = segName + " " + strSet2;
            }
        }

        AnalyticsReportData[] reports = this.MergeAltReportData(reports1, reports2);

        for (int iReport = 0; iReport < reports.Length; iReport++)
        {
            DateTime date = reportStartDates[iReport % 2];
            AnalyticsReportData report = reports[iReport];

            for (int i = 0; i < days[iReport % 2]; i++) 
            {
                switch (metric) 
                {
                    case DisplayMetric.Visits:
                        visitsOverTime[iReport].Add(report.DayVisits(date));
                        break;
                    case DisplayMetric.Pageviews:
                        pageViewsOverTime[iReport].Add(report.DayPageViews(date));
                        break;
                    case DisplayMetric.UniqueViews:
                        uniqueViewsOverTime[iReport].Add(report.DayUniqueViews(date));
                        break;
                    case DisplayMetric.TimeOnSite:
                        averageTimeOnSiteOverTime[iReport].Add(report.DayAverageTimeSpanOnSite(date));
                        break;
                    case DisplayMetric.TimeOnPage:
                        averageTimeOnPageOverTime[iReport].Add(report.DayAverageTimeSpanOnPage(date));
                        break;
                    case DisplayMetric.BounceRate:
                        bounceRateOverTime[iReport].Add(report.DayBounceRate(date));
                        break;
                    case DisplayMetric.PercentExit:
                        percentExitOverTime[iReport].Add(report.DayExitRate(date));
                        break;
                }
                date = date.AddDays(1);
            }
        }

        ComparisonTimeLineChart.BriefDescription = GetMessage("lbl time line chart");
        ComparisonTimeLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
        switch (metric) {
            case DisplayMetric.Visits:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, visitsOverTime);
                break;
            case DisplayMetric.Pageviews:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, pageViewsOverTime);
                break;
            case DisplayMetric.UniqueViews:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, uniqueViewsOverTime);
                break;
            case DisplayMetric.TimeOnSite:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, averageTimeOnSiteOverTime);
                break;
            case DisplayMetric.TimeOnPage:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, averageTimeOnPageOverTime);
                break;
            case DisplayMetric.BounceRate:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, bounceRateOverTime);
                break;
            case DisplayMetric.PercentExit:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], reportStartDates[1], SegmentLabels, percentExitOverTime);
                break;
        }

        //SummaryTitle1.Text = String.Format("{0:D} - {1:D}", _startDate1, _endDate1);
        //SummaryTitle2.Text = String.Format("{0:D} - {1:D}", _startDate2, _endDate2);

        SummaryCharts1.ProviderSegments = this.CookieSegments;
        SummaryCharts1.StartDate = _startDate1;
        SummaryCharts1.EndDate = _endDate1;
        SummaryCharts1.Report = reports1;
        SummaryCharts1.PopulateData();

        SummaryCharts2.ProviderSegments = this.CookieSegments;
        SummaryCharts2.StartDate = _startDate2;
        SummaryCharts2.EndDate = _endDate2;
        SummaryCharts2.Report = reports2;
        SummaryCharts2.PopulateData();

    }

    private AnalyticsReportData[] MergeAltReportData(List<AnalyticsReportData> reports1, List<AnalyticsReportData> reports2)
    {
        List<AnalyticsReportData> lstReports = new List<AnalyticsReportData>();
        lstReports.Add(reports1[0]);
        lstReports.Add(reports2[0]);
        if (reports1.Count > 1)
        {
            for (int i = 1; i < reports1.Count; i++)
            {
                lstReports.Add(reports1[i]);
                lstReports.Add(reports2[i]);
            }
        }
        return lstReports.ToArray();
    }

}
