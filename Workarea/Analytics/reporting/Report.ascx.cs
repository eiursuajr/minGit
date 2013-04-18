using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Interfaces.Analytics.Provider;


public partial class Analytics_reporting_Report : WorkareaBaseControl 
{

    #region Private Members

	private bool _bDrillDownReport = false;
	private bool _bDrillDownDetail = false;
	private string _strDrillDownArg = "for";
	private List<AnalyticsReportData> _reports = null;
	private string _reportTypeName = "";
	private string _columnName = "";
	private AnalyticsSortableField _sortField = AnalyticsSortableField.Name;
	private EkEnumeration.OrderByDirection _sortDirection = EkEnumeration.OrderByDirection.Ascending;
    private ContentAPI _refContentApi = new ContentAPI();
    private EkRequestInformation _requestInfo = null;
    IAnalytics _dataManager = ObjectFactory.GetAnalytics();
    private bool _reportGenerated = false;
    private string _reportTitle = string.Empty;
    private string SegmentPersistenceId = string.Empty;
    IMetrics _metrics = null;
    IDimensions _dimensions = null;
    private string _providerName = "";
    private string _siteName = "";
    private List<AnalyticsReportItem> _segReport;
    private List<string> _providerSegments;

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private ReportDisplayData _reportDisplayData = ReportDisplayData.SiteData;
    private enum ReportDisplayData
    {
        SiteData,
        PageData,
		LandingPageData,
		ExitPageData,
        SiteMetricData,
        SiteContentData,
        InstancesData,
        MobileData,
        ReloadData,
        VisitsData,
        VisitorsData,
        PageVisitorsData,
        ClickthroughRatesData,
        NewBuyerSalesCycleData,
        SalesCycleData,
        DynamicData,
        PageVisitsData
    }

    /// <summary>
    /// IMPORTANT: The order of this enum MUST be the same as how it is declared in the GrideView "gvDataTable"
    /// in Report.ascx. Or else the databind would not be able to match up the data with the column.
    /// </summary>
	private enum ReportTableColumn
	{
		DimensionName,
		[Description("{sam visits}")]
		Visits,
		[Description("%")]
		PercentVisits,
		[Description("{lbl pages visit}")]
		PagesPerVisit,
		[Description("{generic avg time on site}")]
		AverageTimeSpanOnSite,
		[Description("{lbl percent new visits}")]
		PercentNewVisits,
		[Description("{lbl entrances}")]
		Entrances,
		[Description("{lbl exits}")]
		Exits,
		[Description("{lbl pageviews}")]
		PageViews,
		[Description("%")]
		PercentPageviews,
		[Description("{lbl unique pageviews}")]
		UniqueViews,
		[Description("{lbl avg time on page}")]
		AverageTimeSpanOnPage,
		[Description("{lbl bounces}")]
		Bounces,
		[Description("{lbl bounce rate}")]
		BounceRate,
		[Description("{lbl percent exit}")]
		ExitRate,
        [Description("{lbl instances}")]
        Instances,
        [Description("{lbl mobile views}")]
        MobileViews,
        [Description("{sam reloads}")]
        Reloads,
        [Description("{sam visitors}")]
        Visitors,
        [Description("{sam revenue}")]
        Revenue,
        [Description("{sam product views}")]
        ProductViews,
        [Description("{sam searches}")]
        Searches
	}

    #endregion

    #region delegates, events and event arguments

    public event EventHandler<TitleChangedEventArgs> TitleChangedEventHandler;

    public class TitleChangedEventArgs : EventArgs {
        private string _title = string.Empty;

        public string Title {
            get { return _title; }
            set { _title = value; }
        }

        public TitleChangedEventArgs(string title) { _title = title; }
    }

    #endregion

    #region Properties
	public string ProviderName
	{
		get { return (ViewState["ProviderName"] as string ?? _providerName); }
		set { ViewState["ProviderName"] = _providerName = value; }
	}

    public string SiteName
    {
        get { return (ViewState["SiteName"] as string ?? _siteName); }
        set { ViewState["SiteName"] = _siteName = value; }
    }

	public DateTime StartDate
	{
		get { return (DateTime)(ViewState["StartDate"] ?? _startDate); }
		set { ViewState["StartDate"] = _startDate = value; }
	}

	public DateTime EndDate
	{
		get { return (DateTime)(ViewState["EndDate"] ?? _endDate); }
		set { ViewState["EndDate"] = _endDate = value; }
	}

    private ReportType _reportType = ReportType.TopContent;
    public ReportType Report
	{
        get { return (ReportType)(ViewState["Report"] ?? _reportType); }
		set { ViewState["Report"] = _reportType = value; }
	}
    private string _reportGUID = string.Empty;
    public string ReportGUID
	{
        get { return (ViewState["ReportGUID"] as string ?? _reportGUID); }
        set { ViewState["ReportGUID"] = _reportGUID = value; }
	}
    
	private string _forValue = "";
	/// <summary>
	/// Primary criteria condition
	/// </summary>
	public string ForValue
	{
		get { return (ViewState["ForValue"] as string ?? _forValue); }
		set { ViewState["ForValue"] = _forValue = value; }
	}
	private string _andValue = "";
	/// <summary>
	/// Secondary criteria condition
	/// </summary>
	public string AndValue
	{
		get { return (ViewState["AndValue"] as string ?? _andValue); }
		set { ViewState["AndValue"] = _andValue = value; }
	}
	private string _alsoValue = "";
	/// <summary>
	/// Tertiary criteria condition
	/// </summary>
	public string AlsoValue
	{
		get { return (ViewState["AlsoValue"] as string ?? _alsoValue); }
		set { ViewState["AlsoValue"] = _alsoValue = value; }
	}

	public enum DisplayView
	{
		Default,
		Percentage,
		Table,
		Detail,
        Bar,
        Heat
	}
	private DisplayView _defaultView = DisplayView.Default;
	private DisplayView _displayView = DisplayView.Default;
	public DisplayView View
	{
		get 
		{ 
			DisplayView view = (DisplayView)(ViewState["View"] ?? _displayView);
			if (DisplayView.Default == view) view = _defaultView;
			return view; 
		}
		set { ViewState["View"] = _displayView = value; }
	}

	private bool _showTable = true;
	public bool ShowTable
	{
		get { return _showTable; }
		set { _showTable = value; }
	}
	private bool _showPieChart = true;
	public bool ShowPieChart
	{
		get { return _showPieChart; }
		set { _showPieChart = value; }
	}
	public bool ShowSummaryChart
	{
		get { return AnalyticsDetail.ShowSummaryChart; }
		set { AnalyticsDetail.ShowSummaryChart = value; }
	}
	public bool ShowLineChart
	{
		get { return AnalyticsDetail.ShowLineChart; }
		set { AnalyticsDetail.ShowLineChart = value; }
	}
	public Unit LineChartWidth
	{
		get { return AnalyticsDetail.LineChartWidth; }
		set { AnalyticsDetail.LineChartWidth = value; }
	}
	public Unit LineChartHeight
	{
		get { return AnalyticsDetail.LineChartHeight; }
		set { AnalyticsDetail.LineChartHeight = value; }
	}

    public string ReportTitle {
        get { return _reportTitle; }
        set {
            if (_reportTitle != value
                && TitleChangedEventHandler != null) {
                TitleChangedEventHandler(this, new TitleChangedEventArgs(value));
            }
            _reportTitle = value;
        }
    }

    public bool ShowDateRange {
        get { return AnalyticsReportDateRangeDisplay.Visible; }
        set { AnalyticsReportDateRangeDisplay.Visible = value; }
    }

    public List<string> ProviderSegments
    {
        get { return _providerSegments; }
        set { _providerSegments = value; }
    }
    public bool FromWidget { get; set; }
	#endregion

    protected void Page_Load(object sender, EventArgs e)
	{
		ErrorPanel.Visible = false;
		if (null == _requestInfo) 		{
			_requestInfo = _refContentApi.RequestInformationRef;
		}

        if (!_dataManager.IsAnalyticsViewer()) {
            litErrorMessage.Text = GetMessage("com: user does not have permission");
            ErrorPanel.Visible = true;
            return;
        }

		RegisterScripts();

        Page.Validate();
        if (!Page.IsValid)
        {
            _reportGenerated = true;
        }
        this.AnalyticsDetail.ProviderSegments = this.ProviderSegments;
        
		ltrlNoRecords.Text = GetMessage("lbl no records");
        litCssTweaks.Text = String.Empty;
        
    }

	protected override void LoadViewState(object savedState)
	{
		base.LoadViewState(savedState);
		_providerName = this.ProviderName;
		_startDate = this.StartDate;
		_endDate = this.EndDate;
		_reportType = this.Report;
		_forValue = this.ForValue;
		_andValue = this.AndValue;
		_alsoValue = this.AlsoValue;
		_displayView = this.View;
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (!_reportGenerated)
		{
            if (this.ProviderSegments != null) 
            {
                int segCount = this.ProviderSegments.Count;
                if (segCount > 1)
                {
                    // should only reset once
                    this.gvDataTable.PageSize = this.gvDataTable.PageSize * (segCount + 1);
                }
            }
			RefreshAnalyticsReport(this, new EventArgs());
		}
	}

	protected virtual void RefreshAnalyticsReport(object sender, EventArgs e)
	{
		try
		{
			_reports = GetAnalyticsReport(_providerName);
			bindData();
		}
		catch (TypeInitializationException ex)
		{
			string _error = ex.Message;
			lblNoRecords.Text = GetMessage("err analyticsconfig");
			lblNoRecords.Visible = true;
			EkException.LogException(ex);
			return;
		}
	}

	private void UpdateCriteriaOrderBy(AnalyticsCriteria criteria, AnalyticsSortableField defaultField, bool allowSorting)
	{
        if (!allowSorting) return;
        _sortField = defaultField;

        string sortExpression = ViewState["SortExpression"] as string;

        if (!String.IsNullOrEmpty(sortExpression))
        {
            if (Enum.IsDefined(typeof(AnalyticsSortableField), sortExpression))
            {
                AnalyticsSortableField newSortField = (AnalyticsSortableField)Enum.Parse(typeof(AnalyticsSortableField), sortExpression, true);
                if (!((_reportType == ReportType.TopContent || _reportType == ReportType.ContentByTitle || _reportType == ReportType.TopLanding || _reportType == ReportType.TopExit) && newSortField == AnalyticsSortableField.Visits))
                    _sortField = newSortField; //TODO:
            }
        }

        _sortDirection = (_sortField == AnalyticsSortableField.Name ? EkEnumeration.OrderByDirection.Ascending : EkEnumeration.OrderByDirection.Descending);

        if (ViewState["SortDirection"] != null)
        {
            if (SortDirection.Ascending == (SortDirection)ViewState["SortDirection"])
            {
                _sortDirection = EkEnumeration.OrderByDirection.Ascending;
            }
            else
            {
                _sortDirection = EkEnumeration.OrderByDirection.Descending;
            }
        }

        ViewState["SortExpression"] = _sortField.ToString();
        ViewState["SortDirection"] = (_sortDirection == EkEnumeration.OrderByDirection.Ascending ? SortDirection.Ascending : SortDirection.Descending);

        criteria.OrderByField = _sortField;
        criteria.OrderByDirection = _sortDirection;
	}

    private bool IsMetricSupported(Metric m)
    {
        return (m != null);
    }

    private bool IsDimensionSupported(Dimension d)
    {
        return (d != null);
    }

	private List<AnalyticsReportData> GetAnalyticsReport(string provider)
    {
        bool allowSorting = false;
        string cssTweak = string.Empty;
        if (FromWidget)
        {
            cssTweak = "<style type='text/css'> .EktronPersonalization .analyticsReport .SiteSelectorContainer {display: none;} </style>";
        }
        else
        {
            cssTweak = "<style type='text/css'> .SiteSelectorContainer {display: none;} </style>";
        }
        if (!String.IsNullOrEmpty(provider) && _dataManager.HasProvider(provider))
        {
            allowSorting = _dataManager.AllowSorting(provider);
            _dimensions = _dataManager.GetDimensions(provider);
            _metrics = _dataManager.GetMetrics(provider);
            string providerType = _dataManager.GetProviderType(provider);
            if (providerType != "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider")
            {
                if (FromWidget)
                {
                    cssTweak = "<style type='text/css'> .EktronPersonalization .analyticsReport input.SegmentPopupBtn {display: none;} </style>";
                }
                else
                {
                    cssTweak = "<style type='text/css'> input.SegmentPopupBtn {display: none;} </style>";
                }
            }
        }
        gvDataTable.AllowSorting = allowSorting;

        List<AnalyticsReportData> reports = null;
        AnalyticsReportData report = null;
		DateTime startDate = _startDate;
		DateTime endDate = _endDate;
        string reportSubtitle = string.Empty;
        string reportSummary = string.Empty;
        string reportSummaryShort = string.Empty;
        string reportSegSummary = string.Empty;
        bool bGeneric = false;

		if (ErrorPanel.Visible) {
            this.htmReportSummary.InnerText = string.Empty;
            return null;
        }

        AnalyticsCriteria criteria = new AnalyticsCriteria();
        // setting criteria.PagingInfo.RecordsPerPage will disable the paging in the grid.
        //criteria.PagingInfo.RecordsPerPage = gvDataTable.PageSize;
		if (DisplayView.Detail == this.View)
		{
			criteria.AggregationPeriod = AggregationTimePeriod.ByDay;
		}

        try
        {
			_defaultView = DisplayView.Percentage;
			_bDrillDownReport = false;
			_bDrillDownDetail = true;
			switch (_reportType)
            {
                #region Google Analytics Reports
                /* case ReportType.MapOverlay: */
				case ReportType.Locations:
                    ReportTitle = _reportTypeName = GetMessage("sam locations");
                    if (IsDimensionSupported(_dimensions.country))
                    {
                        reportSummary = GetMessage("lbl visit came from countries");
                        reportSegSummary = GetMessage("lbl visit came from countries in segment");
                        _columnName = GetMessage("lbl country territory");
                        _defaultView = DisplayView.Table;
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.country, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.region);
                            reportSummary = GetMessage("lbl visit came from regions");
                            reportSegSummary = GetMessage("lbl visit came from regions in segment");
                            ReportTitle = _columnName;
                            if (_forValue != "(not set)") // TODO: "(not set)" is Google-specific
                            {
                                reportSubtitle = _forValue;
                            }
                            _columnName = GetMessage("lbl region");
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.region, DimensionFilterOperator.EqualTo, _andValue);
                            criteria.Dimensions.Insert(0, _dimensions.city);
                            reportSummary = GetMessage("lbl visit came from cities");
                            reportSegSummary = GetMessage("lbl visit came from cities in segment");
                            ReportTitle = _columnName;
                            if (_andValue != "(not set)") // TODO: "(not set)" is Google-specific
                            {
                                if (reportSubtitle.Length > 0)
                                {
                                    reportSubtitle = _andValue + ", " + reportSubtitle;
                                }
                                else
                                {
                                    reportSubtitle = _andValue;
                                }
                            }
                            _columnName = GetMessage("lbl address city");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "also";
                        }
                        if (!String.IsNullOrEmpty(_alsoValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.city, DimensionFilterOperator.EqualTo, _alsoValue);
                            ReportTitle = _columnName;
                            if (_alsoValue != "(not set)") // TODO: "(not set)" is Google-specific
                            {
                                if (reportSubtitle.Length > 0)
                                {
                                    reportSubtitle = _alsoValue + ", " + reportSubtitle;
                                }
                                else
                                {
                                    reportSubtitle = _alsoValue;
                                }
                            }
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.NewVsReturning:
                    ReportTitle = _reportTypeName = GetMessage("sam new vs returning");
                    if (IsDimensionSupported(_dimensions.visitorType) && IsMetricSupported(_metrics.visitors))
                    {
                        reportSummary = GetMessage("lbl visit from visitor types");
                        reportSegSummary = GetMessage("lbl visit from visitor types in segment");
                        _columnName = GetMessage("lbl visitor type");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.visitorType, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Languages:
                    ReportTitle = _reportTypeName = GetMessage("sam languages");
                    if (IsDimensionSupported(_dimensions.language))
                    {
                        reportSummary = GetMessage("lbl visit used languages");
                        reportSegSummary = GetMessage("lbl visit used languages in segment");
                        _columnName = GetMessage("generic language");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.language, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;


                case ReportType.Browsers:
                    ReportTitle = _reportTypeName = GetMessage("sam browsers");
                    if (IsDimensionSupported(_dimensions.browser))
                    {
                        reportSummary = GetMessage("lbl visit used browsers");
                        reportSegSummary = GetMessage("lbl visit used browsers in segment");
                        _columnName = GetMessage("lbl browser");
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.browser, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.browserVersion);
                            reportSummary = GetMessage("lbl visit used browser versions");
                            reportSegSummary = GetMessage("lbl visit used browser versions in segment");
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue + " ";
                            _columnName = GetMessage("lbl browser version");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.browserVersion, DimensionFilterOperator.EqualTo, _andValue);
                            ReportTitle = _columnName;
                            reportSubtitle += _andValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.OS:
                    ReportTitle = _reportTypeName = GetMessage("sam operating systems");
                    if (IsDimensionSupported(_dimensions.operatingSystem))
                    {
                        reportSummary = GetMessage("lbl visit used operating systems");
                        reportSegSummary = GetMessage("lbl visit used operating systems in segment");
                        _columnName = GetMessage("lbl operating system");
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.operatingSystem, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.operatingSystemVersion);
                            reportSummary = GetMessage("lbl visit used os versions");
                            reportSegSummary = GetMessage("lbl visit used os versions in segment");
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue + " ";
                            _columnName = GetMessage("lbl os version");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.operatingSystemVersion, DimensionFilterOperator.EqualTo, _andValue);
                            ReportTitle = _columnName;
                            reportSubtitle += _andValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Platforms:
                    ReportTitle = _reportTypeName = GetMessage("sam browsers and os");
                    if (IsDimensionSupported(_dimensions.browser) && IsDimensionSupported(_dimensions.operatingSystem))
                    {
                        reportSummary = GetMessage("lbl visit used browser and os combinations");
                        reportSegSummary = GetMessage("lbl visit used browser and os combinations in segment");
                        _columnName = GetMessage("lbl browser and os");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            string[] values = _forValue.Split(new char[] { '/' }, 2);
                            criteria.DimensionFilters.AddFilter(_dimensions.browser, DimensionFilterOperator.EqualTo, values[0].Trim());
                            criteria.DimensionFilters.AddFilter(_dimensions.operatingSystem, DimensionFilterOperator.EqualTo, values[1].Trim());
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Colors:
                    ReportTitle = _reportTypeName = GetMessage("sam screen colors");
                    if (IsDimensionSupported(_dimensions.screenColors))
                    {
                        reportSummary = GetMessage("lbl visit used screen colors");
                        reportSegSummary = GetMessage("lbl visit used screen colors in segment");
                        _columnName = GetMessage("sam screen colors");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.screenColors, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Resolutions:
                    ReportTitle = _reportTypeName = GetMessage("sam screen resolutions");
                    if (IsDimensionSupported(_dimensions.screenResolution))
                    {
                        reportSummary = GetMessage("lbl visit used screen resolutions");
                        reportSegSummary = GetMessage("lbl visit used screen resolutions in segment");
                        _columnName = GetMessage("lbl screen resolution");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.screenResolution, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Flash:
                    ReportTitle = _reportTypeName = GetMessage("sam flash versions");
                    if (IsDimensionSupported(_dimensions.flashVersion))
                    {
                        reportSummary = GetMessage("lbl visit used flash versions");
                        reportSegSummary = GetMessage("lbl visit used flash versions in segment");
                        _columnName = GetMessage("lbl flash version");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.flashVersion, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Java:
                    ReportTitle = _reportTypeName = GetMessage("sam java support");
                    if (IsDimensionSupported(_dimensions.javaEnabled))
                    {
                        reportSummary = GetMessage("lbl visit used java support");
                        reportSegSummary = GetMessage("lbl visit used java support in segment");
                        _columnName = ReportTitle;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.javaEnabled, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;


                case ReportType.NetworkLocations:
                    ReportTitle = _reportTypeName = GetMessage("sam network location");
                    if (IsDimensionSupported(_dimensions.networkLocation))
                    {
                        reportSummary = GetMessage("lbl visit came from network locations");
                        reportSegSummary = GetMessage("lbl visit came from network locations in segment");
                        _columnName = ReportTitle;
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.networkLocation, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Hostnames:
                    ReportTitle = _reportTypeName = GetMessage("sam hostnames");
                    if (IsDimensionSupported(_dimensions.hostname))
                    {
                        reportSummary = GetMessage("lbl visit came from hostnames");
                        reportSegSummary = GetMessage("lbl visit came from hostnames in segment");
                        _columnName = GetMessage("lbl hostname");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.hostname, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.ConnectionSpeeds:
                    ReportTitle = _reportTypeName = GetMessage("sam connection speeds");
                    if (IsDimensionSupported(_dimensions.connectionSpeed))
                    {
                        reportSummary = GetMessage("lbl visit used connection speeds");
                        reportSegSummary = GetMessage("lbl visit used connection speeds in segment");
                        _columnName = GetMessage("lbl connection speed");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.connectionSpeed, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;


                case ReportType.UserDefined:
                    ReportTitle = _reportTypeName = GetMessage("sam user defined");
                    if (IsDimensionSupported(_dimensions.userDefinedValue))
                    {
                        reportSummary = GetMessage("lbl visit used user defined values");
                        reportSegSummary = GetMessage("lbl visit used user defined values in segment");
                        _columnName = GetMessage("lbl user defined value");
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.userDefinedValue, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;


                case ReportType.Direct:
                    ReportTitle = _reportTypeName = GetMessage("sam direct traffic");
                    if (IsDimensionSupported(_dimensions.source))
                    {
                        reportSummary = GetMessage("lbl visit came directly to this site");
                        reportSegSummary = GetMessage("lbl visit came directly to this site in segment");
                        _columnName = GetMessage("lbl source");
                        _defaultView = DisplayView.Detail;
                        if (DisplayView.Detail == this.View)
                        {
                            criteria.AggregationPeriod = AggregationTimePeriod.ByDay;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Referring:
                    ReportTitle = _reportTypeName = GetMessage("sam referring sites");
                    if (IsDimensionSupported(_dimensions.source))
                    {
                        reportSummary = GetMessage("lbl referring sites sent visits via sources");
                        reportSegSummary = GetMessage("lbl referring sites sent visits via sources in segment");
                        _columnName = GetMessage("lbl referring site");
                        _defaultView = DisplayView.Table;
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.source, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.referralPath);
                            reportSummary = GetMessage("lbl referring site sent visits via paths");
                            reportSegSummary = GetMessage("lbl referring site sent visits via paths");
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue + " ";
                            _columnName = GetMessage("lbl referring link");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.referralPath, DimensionFilterOperator.EqualTo, _andValue);
                            ReportTitle = _columnName;
                            reportSubtitle += ":\xA0 " + _andValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.SearchEngines:
                    ReportTitle = _reportTypeName = GetMessage("sam search engines");
                    if (IsDimensionSupported(_dimensions.source))
                    {
                        reportSummary = GetMessage("lbl search sent total visits via sources");
                        reportSegSummary = GetMessage("lbl search sent total visits via sources in segment");
                        _columnName = GetMessage("lbl search engine");
                        _defaultView = DisplayView.Table;
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.source, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.keyword);
                            reportSummary = GetMessage("lbl search sent total visits via keywords");
                            reportSegSummary = GetMessage("lbl search sent total visits via keywords in segment");
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue + " ";
                            _columnName = GetMessage("lbl keyword");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.keyword, DimensionFilterOperator.EqualTo, _andValue);
                            ReportTitle = _columnName;
                            reportSubtitle += ":\xA0 \"" + _andValue + "\"";
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.TrafficSources:
                    ReportTitle = _reportTypeName = GetMessage("sam all traffic sources");
                    if (IsDimensionSupported(_dimensions.source) && IsDimensionSupported(_dimensions.medium))
                    {
                        reportSummary = GetMessage("lbl all traffic sources sent visits via sources and mediums");
                        reportSegSummary = GetMessage("lbl all traffic sources sent visits via sources and mediums in segment");
                        _columnName = GetMessage("lbl source");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            string[] values = _forValue.Split(new char[] { '/' }, 2);
                            criteria.DimensionFilters.AddFilter(_dimensions.source, DimensionFilterOperator.EqualTo, values[0].Trim());
                                criteria.DimensionFilters.AddFilter(_dimensions.medium, DimensionFilterOperator.EqualTo, values[1].Trim());
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Keywords:
                    ReportTitle = _reportTypeName = GetMessage("sam keywords");
                    if (IsDimensionSupported(_dimensions.keyword))
                    {
                        reportSummary = GetMessage("lbl search sent total visits via keywords");
                        reportSegSummary = GetMessage("lbl search sent total visits via keywords in segment");
                        _columnName = GetMessage("lbl keyword");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.keyword, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.Campaigns:
                    ReportTitle = _reportTypeName = GetMessage("sam campaigns");
                    if (IsDimensionSupported(_dimensions.campaign))
                    {
                        reportSummary = GetMessage("lbl campaign traffic sent visits via campaigns");
                        reportSegSummary = GetMessage("lbl campaign traffic sent visits via campaigns in segment");
                        _columnName = GetMessage("lbl campaign");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.campaign, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;
                case ReportType.AdVersions:
                    ReportTitle = _reportTypeName = GetMessage("sam ad versions");
                    if (IsDimensionSupported(_dimensions.adContent))
                    {
                        reportSummary = GetMessage("lbl ads sent visits via ad contents");
                        reportSegSummary = GetMessage("lbl ads sent visits via ad contents in segment");
                        _columnName = GetMessage("lbl ad content");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.adContent, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                    }
                    break;


                case ReportType.TopContent:
                    ReportTitle = _reportTypeName = GetMessage("sam top content");
                    if (IsDimensionSupported(_dimensions.pagePath))
                    {
                        reportSummary = GetMessage("lbl pages were viewed a total of times");
                        reportSegSummary = GetMessage("lbl pages were viewed a total of times in segment");
                        reportSummaryShort = GetMessage("lbl pages were viewed");
                        _columnName = GetMessage("page lbl");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.PageData;
                    }
                    break;
                case ReportType.ContentByTitle:
                    ReportTitle = _reportTypeName = GetMessage("sam content by title");
                    if (IsDimensionSupported(_dimensions.pageTitle))
                    {
                        reportSummary = GetMessage("lbl page titles were viewed a total times");
                        reportSegSummary = GetMessage("lbl page titles were viewed a total times in segment");
                        reportSummaryShort = GetMessage("lbl page titles were viewed");
                        _columnName = GetMessage("lbl page title");
                        _defaultView = DisplayView.Table;
					    _bDrillDownReport = true;
					    _bDrillDownDetail = false;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.pageTitle, DimensionFilterOperator.EqualTo, _forValue);
                            criteria.Dimensions.Insert(0, _dimensions.pagePath);
                            reportSummary = GetMessage("lbl page visited times via pages");
                            reportSegSummary = GetMessage("lbl page visited times via pages in segment");
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue + " ";
                            _columnName = GetMessage("page lbl");
                            _bDrillDownReport = false;
                            _bDrillDownDetail = true;
                            _strDrillDownArg = "and";
                        }
                        if (!String.IsNullOrEmpty(_andValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.pagePath, DimensionFilterOperator.EqualTo, _andValue);
                            ReportTitle = _columnName;
                            reportSubtitle += ":\xA0 \"" + _andValue + "\"";
                            _defaultView = DisplayView.Detail;
                        }
                        UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews, allowSorting);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.PageData;
                    }
                    break;
                case ReportType.TopLanding:
                    ReportTitle = _reportTypeName = GetMessage("sam top landing pages");
                    if (IsDimensionSupported(_dimensions.landingPagePath) && IsMetricSupported(_metrics.bounces) && IsMetricSupported(_metrics.entrances))
                    {
                        reportSummary = GetMessage("lbl visit entered the site through pages");
                        reportSegSummary = GetMessage("lbl visit entered the site through pages in segment");
                        _columnName = GetMessage("page lbl");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;

                            UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews, allowSorting);
                            reports = this.GetReportDataList(provider, ReportType.TopContent, startDate, endDate, criteria);
                            _reportDisplayData = ReportDisplayData.PageData;
                        }
                        else
                        {
                            UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Entrances, allowSorting);
                            reports = this.GetReportDataList(provider, ReportType.TopLanding, startDate, endDate, criteria);
                            _reportDisplayData = ReportDisplayData.LandingPageData;
                        }
                    }
                    break;
                case ReportType.TopExit:
                    ReportTitle = _reportTypeName = GetMessage("sam top exit pages");
                    if (IsDimensionSupported(_dimensions.exitPagePath) && IsMetricSupported(_metrics.exits) && IsMetricSupported(_metrics.pageviews))
                    {
                        reportSummary = GetMessage("lbl visits exited from pages");
                        reportSegSummary = GetMessage("lbl visits exited from pages in segment");
                        _columnName = GetMessage("page lbl");
                        _defaultView = DisplayView.Table;
                        if (!String.IsNullOrEmpty(_forValue))
                        {
                            criteria.DimensionFilters.AddFilter(_dimensions.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                            ReportTitle = _columnName;
                            reportSubtitle = _forValue;
                            _defaultView = DisplayView.Detail;

                            UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews, allowSorting);
                            reports = this.GetReportDataList(provider, ReportType.TopContent, startDate, endDate, criteria);
                            _reportDisplayData = ReportDisplayData.PageData;
                        }
                        else
                        {
                            UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Exits, allowSorting);
                            reports = this.GetReportDataList(provider, ReportType.TopExit, startDate, endDate, criteria);
                            _reportDisplayData = ReportDisplayData.ExitPageData;
                        }
                    }
                    break;                
                #endregion
                #region SiteCatalyst Reports
                case ReportType.TimeVisitOnSite:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam time spent per visit");
                    if (IsDimensionSupported(_dimensions.timeVisit) && IsMetricSupported(_metrics.visits) && IsMetricSupported(_metrics.pageviews)) 
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteMetricData;

                    }
                    break;
                case ReportType.Pages:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = GetMessage("sam pages");
                    _columnName = GetMessage("page lbl");
                    if (IsDimensionSupported(_dimensions.pageTitle) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SiteSection:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam site sections");
                    if (IsDimensionSupported(_dimensions.siteSection) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.Server:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam servers");
                    if (IsDimensionSupported(_dimensions.hostname) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.LinkExit:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam exit links");
                    if (IsDimensionSupported(_dimensions.exitPagePath) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.LinkDownload:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam file downloads");
                    if (IsDimensionSupported(_dimensions.linkDownload) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.LinkCustom:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam custom links");
                    if (IsDimensionSupported(_dimensions.linkCustom) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.PagesNotFound:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam pages not found");
                    if (IsDimensionSupported(_dimensions.pagesNotFound) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.MobileDeviceName:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam devices");
                    if (IsDimensionSupported(_dimensions.mobileDeviceName) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileManufacturer:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam manufacturer");
                    if (IsDimensionSupported(_dimensions.mobileManufacturer) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileScreenSize:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam screen size");
                    if (IsDimensionSupported(_dimensions.mobileScreenSize) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileScreenHeight:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam screen height");
                    if (IsDimensionSupported(_dimensions.mobileScreenHeight) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileScreenWidth:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam screen width");
                    if (IsDimensionSupported(_dimensions.mobileScreenWidth) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileCookieSupport:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam cookie support");
                    if (IsDimensionSupported(_dimensions.mobileCookieSupport) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileImageSupport:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam image support");
                    if (IsDimensionSupported(_dimensions.mobileImageSupport) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileColorDepth:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam color depth");
                    if (IsDimensionSupported(_dimensions.mobileColorDepth) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileAudioSupport:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam audio support");
                    if (IsDimensionSupported(_dimensions.mobileAudioSupport) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileVideoSupport:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam video support");
                    if (IsDimensionSupported(_dimensions.mobileVideoSupport) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileDRM:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam drm");
                    if (IsDimensionSupported(_dimensions.mobileDRM) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileNetProtocols:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam net protocols");
                    if (IsDimensionSupported(_dimensions.mobileNetProtocols) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileOS:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam operating system");
                    if (IsDimensionSupported(_dimensions.mobileOS) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileJavaVM:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam java version");
                    if (IsDimensionSupported(_dimensions.mobileJavaVM) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileMaxBookmarkUrlLength:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam bookmark url length");
                    if (IsDimensionSupported(_dimensions.mobileMaxBookmarkUrlLength) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileMaxMailUrlLength:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam mail url length");
                    if (IsDimensionSupported(_dimensions.mobileMaxMailUrlLength) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileMaxBroswerUrlLength:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam browser url length");
                    if (IsDimensionSupported(_dimensions.mobileMaxBrowserUrlLength) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileDeviceNumberTransmit:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam device number transmit (on/off)");
                    if (IsDimensionSupported(_dimensions.mobileDeviceNumberTransmit) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobilePushToTalk:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam ptt");
                    if (IsDimensionSupported(_dimensions.mobilePushToTalk) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileMailDecoration:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam decoration mail support");
                    if (IsDimensionSupported(_dimensions.mobileMailDecoration) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.MobileInformationServices:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam information services");
                    if (IsDimensionSupported(_dimensions.mobileInformationServices) && IsMetricSupported(_metrics.mobileViews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.MobileData;
                    }
                    break;
                case ReportType.Reloads:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam reloads");
                    if (IsDimensionSupported(_dimensions.pageTitle) && IsMetricSupported(_metrics.reloads))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.ReloadData;
                    }
                    break;
                case ReportType.AveragePageDepth:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam page depth");
                    if (IsDimensionSupported(_dimensions.pageDepth) && IsMetricSupported(_metrics.pageviews) && IsMetricSupported(_metrics.visits))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteMetricData;
                    }
                    break;
                case ReportType.TimeVisitOnPage:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam time spent on page");
                    if (IsDimensionSupported(_dimensions.timeVisit) && IsMetricSupported(_metrics.instances))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.InstancesData;
                    }
                    break;
                case ReportType.EntryPage:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam entry pages");
                    if (IsDimensionSupported(_dimensions.landingPagePath) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.EntryPageOriginal:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam original entry pages");
                    if (IsDimensionSupported(_dimensions.entryPageOriginal) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEngineKeyword:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search keywords - all");
                    if (IsDimensionSupported(_dimensions.searchKeyword) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEnginePaidKeyword:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search keywords - paid");
                    if (IsDimensionSupported(_dimensions.searchEnginePaidKeyword) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEngineNaturalKeyword:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search keywords - natural");
                    if (IsDimensionSupported(_dimensions.searchEngineNaturalKeyword) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SiteCatalystSearchEngine:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search engines - all");
                    if (IsDimensionSupported(_dimensions.searchEngine) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEnginePaid:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search engines - paid");
                    if (IsDimensionSupported(_dimensions.searchEnginePaid) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEngineNatural:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search engines - natural");
                    if (IsDimensionSupported(_dimensions.searchEngineNatural) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.SearchEngineNaturalPageRank:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam all search page ranking");
                    if (IsDimensionSupported(_dimensions.searchEngineNaturalPageRank) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.ReferringDomain:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam referring domains");
                    if (IsDimensionSupported(_dimensions.referringDomain) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.ReferringDomainOriginal:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam search keywords - all");
                    if (IsDimensionSupported(_dimensions.searchKeyword) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.Referrer:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam referrers");
                    if (IsDimensionSupported(_dimensions.source) && IsMetricSupported(_metrics.instances))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.InstancesData;
                    }
                    break;
                case ReportType.ReferrerType:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam referrer type");
                    if (IsDimensionSupported(_dimensions.medium) && IsMetricSupported(_metrics.instances))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.InstancesData;
                    }
                    break;
                case ReportType.Products:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam products");
                    if (IsDimensionSupported(_dimensions.productName) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.ReturnFrequency:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam return frequency");
                    if (IsDimensionSupported(_dimensions.returnFrequency) && IsMetricSupported(_metrics.visits))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitsData;
                    }
                    break;
                case ReportType.VisitNumber:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam visit number");
                    if (IsDimensionSupported(_dimensions.countOfVisits) && IsMetricSupported(_metrics.pageviews) && IsMetricSupported(_metrics.visits))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.PageVisitsData;
                    }
                    break;
                case ReportType.GeoCountries:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam countries");
                    if (IsDimensionSupported(_dimensions.country) && IsMetricSupported(_metrics.visitors))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        reportSummary = "";
                        _defaultView = DisplayView.Table;
                        report = _dataManager.GetLocations(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.GeoRegions:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam regions");
                    if (IsDimensionSupported(_dimensions.region) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.GeoCities:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam cities");
                    if (IsDimensionSupported(_dimensions.city) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.GeoDMA:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam u.s. dma");
                    if (IsDimensionSupported(_dimensions.geoDMA) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.HomePage:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = GetMessage("sam visitor home page");
                    _columnName = GetMessage("page lbl");
                    if (IsDimensionSupported(_dimensions.pageTitle) && IsMetricSupported(_metrics.visits))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitsData;
                    }
                    break;
                case ReportType.SiteCatalystLanguage:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam languages");
                    if (IsDimensionSupported(_dimensions.language) && IsMetricSupported(_metrics.visitors) && IsMetricSupported(_metrics.pageviews))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        reportSummary = "";
                        _defaultView = DisplayView.Table;
                        report = _dataManager.GetLanguages(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.PageVisitorsData;
                    }
                    break;
                case ReportType.TimeZone:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam time zones");
                    if (IsDimensionSupported(_dimensions.timeVisit) && IsMetricSupported(_metrics.visitors) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.PageVisitorsData;
                    }
                    break;
                case ReportType.Domain:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam domains");
                    if (IsDimensionSupported(_dimensions.networkLocation) && IsMetricSupported(_metrics.pageviews))
                    {
                        
                        reportSummary = GetMessage("lbl visit came from network locations");
                        _defaultView = DisplayView.Table;
                        report = _dataManager.GetNetworkLocations(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.PageVisitorsData;
                    }
                    break;
                case ReportType.TopLevelDomain:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam top level domains");
                    if (IsDimensionSupported(_dimensions.topLevelDomain) && IsMetricSupported(_metrics.visitors) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.PageVisitorsData;
                    }
                    break;
                case ReportType.SiteCatalystBrowsers:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam browsers");
                    if (IsDimensionSupported(_dimensions.browser) && IsMetricSupported(_metrics.visitors) && IsMetricSupported(_metrics.pageviews))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetBrowsers(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.BrowserType:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam browser types");
                    if (IsDimensionSupported(_dimensions.browserType) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.BrowserWidth:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam browser width");
                    if (IsDimensionSupported(_dimensions.browserWidth) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.BrowserHeight:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam browser height");
                    if (IsDimensionSupported(_dimensions.browserHeight) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.SiteCatalystOperatingSystem:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam operating systems");
                    if (IsDimensionSupported(_dimensions.operatingSystem) && IsMetricSupported(_metrics.visitors) && IsMetricSupported(_metrics.pageviews))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetOperatingSystems(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.PageVisitorsData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.MonitorColorDepth:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam monitor color depths");
                    if (IsDimensionSupported(_dimensions.screenColors) && IsMetricSupported(_metrics.visitors))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetScreenColors(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.MonitorResolution:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam monitor resolutions");
                    if (IsDimensionSupported(_dimensions.screenResolution) && IsMetricSupported(_metrics.visitors))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetScreenResolutions(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.JavaEnabled:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam java");
                    if (IsDimensionSupported(_dimensions.javaEnabled) && IsMetricSupported(_metrics.visitors))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetJavaSupport(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.JavaScriptEnabled:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam javaScript");
                    if (IsDimensionSupported(_dimensions.javaScriptEnabled) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.JavaScriptVersion:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam javaScript version");
                    if (IsDimensionSupported(_dimensions.javaScriptVersion) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.CookiesEnabled:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam cookies");
                    if (IsDimensionSupported(_dimensions.cookiesEnabled) && IsMetricSupported(_metrics.visitors))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                    }
                    break;
                case ReportType.ConnectionTypes:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam connection types");
                    if (IsDimensionSupported(_dimensions.connectionSpeed) && IsMetricSupported(_metrics.visitors))
                    {
                        _bDrillDownReport = false;
                        _bDrillDownDetail = false;
                        report = _dataManager.GetConnectionSpeeds(provider, startDate, endDate, criteria);
                        _reportDisplayData = ReportDisplayData.VisitorsData;
                        this.ShowPieChart = false;
                    }
                    break;
                case ReportType.VisitorUSState:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam visitor state");
                    if (IsDimensionSupported(_dimensions.state) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                case ReportType.VisitorZipCode:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = _columnName = GetMessage("sam visitor zip/postal code");
                    if (IsDimensionSupported(_dimensions.zip) && IsMetricSupported(_metrics.pageviews))
                    {
                        bGeneric = true;
                        _reportDisplayData = ReportDisplayData.SiteContentData;
                    }
                    break;
                #endregion
                #region WebTrends Reports
                case ReportType.WebTrendsReport:
                    litCssTweaks.Text = cssTweak;
                    criteria.ReportGUID = this.ReportGUID;
                    bGeneric = true;
                    _reportDisplayData = ReportDisplayData.DynamicData;
                    break;
                #endregion
                case ReportType.CmsSearchTerms:
                    litCssTweaks.Text = cssTweak;
                    ReportTitle = _reportTypeName = GetMessage("lbl cms search terms");
                    reportSummary = GetMessage("lbl searches used search terms");
                    _columnName = GetMessage("lbl phrase");
                    _bDrillDownDetail = false; // not implemented
                    UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews, allowSorting);
                    report = _dataManager.GetCmsSearchTerms(startDate, endDate);
                    break;
                default:
                    ReportTitle = _reportTypeName = _reportType.ToString();
                    break;
            }
            if (bGeneric)
            {
                _bDrillDownReport = false;
                _bDrillDownDetail = false;
                reportSummary = "";
                _defaultView = DisplayView.Table;
                //UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Exits, allowSorting);
                report = _dataManager.GetReportRanked(provider, _reportType, startDate, endDate, criteria);
                reports = new List<AnalyticsReportData>();
                reports.Add(report);
                ltrVisualizationView.Text = GetMessage("generic view");
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Username and/or password not set"))
                litErrorMessage.Text = this.GetMessage("err google overview");
            else if (ex.Message.Contains("Web Services Username and/or Secret not set"))
                litErrorMessage.Text = this.GetMessage("err analytic report");
            else
                litErrorMessage.Text = ex.Message;
            ErrorPanel.Visible = true;
        }

        if (null == reports || 0 == reports.Count)
        {
            if (string.IsNullOrEmpty(provider))
            {
                this.htmReportSummary.InnerText = GetMessage("msg no data report");
            }
            else
            {
                string pName = provider.Substring(0, 1).ToUpper() + provider.Substring(1, provider.Length - 1);
                this.htmReportSummary.InnerText = String.Format(GetMessage("msg report not supported"), ReportTitle, pName); //"{0} report is not supported by {1}"
            }
        }
        else if (0 == reports[0].TotalResults)
        {
            this.htmReportSummary.InnerText = GetMessage("msg no data report");
            if (reports[0].ReportDataSet != null && !string.IsNullOrEmpty(reports[0].ReportDataSet.Title))
            {
                ReportTitle = _reportTypeName = reports[0].ReportDataSet.Title;
            }
            if (!string.IsNullOrEmpty(provider))
            {
                ReportTitle = _dataManager.GetProviderSiteURL(provider) + " " + ReportTitle;
            }
        }
        else
        {
            if (reports[0].ReportDataSet != null)
            {
                if (reports[0].ReportDataSet.HasDrillDownData != null)
                {
                    _bDrillDownReport = (String.IsNullOrEmpty(_forValue) ? reports[0].ReportDataSet.HasDrillDownData : false);
                }
                if (!string.IsNullOrEmpty(reports[0].ReportDataSet.Title))
                {
                    ReportTitle = _reportTypeName = reports[0].ReportDataSet.Title;
                }
            }
            if (!string.IsNullOrEmpty(provider))
            {
                ReportTitle = _dataManager.GetProviderSiteURL(provider) + " " + ReportTitle;
            }
            htmReportTitle.InnerText = ReportTitle;

            RenderDateRange();

			if (!String.IsNullOrEmpty(reportSubtitle))
			{
				htmReportSubtitle.InnerText = reportSubtitle;
				htmReportSubtitle.Visible = true;
			}

			if (DisplayView.Detail == this.View)
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
                    case ReportDisplayData.SiteMetricData:
                    case ReportDisplayData.SiteContentData:
                    case ReportDisplayData.InstancesData:
                    case ReportDisplayData.MobileData:
                    case ReportDisplayData.ReloadData:
                    case ReportDisplayData.VisitsData:
                    case ReportDisplayData.VisitorsData:
                    case ReportDisplayData.PageVisitsData:
                    case ReportDisplayData.PageVisitorsData:
                    case ReportDisplayData.DynamicData:
						this.htmReportSummary.Visible = false;
						break;
					case ReportDisplayData.PageData:
					case ReportDisplayData.LandingPageData:
					case ReportDisplayData.ExitPageData:
                        string total = (reports[0].TotalPageViews > 0) ? reports[0].TotalPageViews.ToString() : "";
                        this.htmReportSummary.InnerText = String.Format(GetMessage("lbl page viewed times"), total);
						break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
			else
			{
                string total = "";
                switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
                    case ReportDisplayData.SiteMetricData:
                        total = (reports[0].TotalVisits > 0) ? reports[0].TotalVisits.ToString() : "";
                        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSegSummary, total, reports[0].TotalResults, reports[0].Segment.Name);
                        }
                        else
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSummary, total, reports[0].TotalResults);
                        }
						break;
                    case ReportDisplayData.SiteContentData:
                    case ReportDisplayData.InstancesData:
					case ReportDisplayData.PageData:
                        total = (reports[0].TotalResults > 0) ? reports[0].TotalResults.ToString() : "";
                        if (total.Length > 0)
                        {
                            if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
                            {
                                this.htmReportSummary.InnerText = String.Format(reportSegSummary, total, reports[0].TotalPageViews, reports[0].Segment.Name);
                            }
                            else
                            {
                                this.htmReportSummary.InnerText = String.Format(reportSummary, total, reports[0].TotalPageViews);
                            }
                        }
                        else
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSummaryShort, reports[0].TotalPageViews);
                        }
						break;
					case ReportDisplayData.LandingPageData:
                        total = (reports[0].TotalEntrances > 0) ? reports[0].TotalEntrances.ToString() : "";
                        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSegSummary, total, reports[0].TotalResults, reports[0].Segment.Name);
                        }
                        else
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSummary, total, reports[0].TotalResults);
                        }
						break;
					case ReportDisplayData.ExitPageData:
                        total = (reports[0].TotalExits > 0) ? reports[0].TotalExits.ToString() : "";
                        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSegSummary, total, reports[0].TotalResults, reports[0].Segment.Name);
                        }
                        else
                        {
                            this.htmReportSummary.InnerText = String.Format(reportSummary, total, reports[0].TotalResults);
                        }
						break;
                    case ReportDisplayData.MobileData:
                    case ReportDisplayData.ReloadData:
                    case ReportDisplayData.VisitorsData:
                    case ReportDisplayData.VisitsData:
                    case ReportDisplayData.PageVisitorsData:
                    case ReportDisplayData.PageVisitsData:
                    case ReportDisplayData.DynamicData:
                        this.htmReportSummary.InnerText = "";
                        break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
        }
        _reportGenerated = true;
        return reports;
    }

    protected void RenderDateRange() {
        try {
            if (_refContentApi != null && _refContentApi.ContentLanguage > 0) {
                IFormatProvider format = new System.Globalization.CultureInfo(_refContentApi.ContentLanguage);
                AnalyticsReportDateRangeDisplay.InnerText = StartDate.ToString("d", format) + " - " + this.EndDate.ToString("d", format);
                return;
            }
        }
        catch {}

        AnalyticsReportDateRangeDisplay.InnerText = this.StartDate.ToShortDateString() + " - " + this.EndDate.ToShortDateString();
    }

    private List<AnalyticsReportData> GetReportDataList(string provider, ReportType reportType, DateTime startDate, DateTime endDate, AnalyticsCriteria criteria)
    {
        List<AnalyticsReportData> mergedReport = new List<AnalyticsReportData>();
        AnalyticsReportData oneReport = null;
        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
        {
            foreach (string segIdPair in this.ProviderSegments)
            {
                string segVal = segIdPair.Substring(0, segIdPair.IndexOf("|"));
                string sSegProp = segIdPair.Replace(segVal + "|", "");
                SegmentProperty segProp = (SegmentProperty)Convert.ToInt32(sSegProp);
                criteria.SegmentFilter = new SegmentFilter(segProp, SegmentFilterOperator.EqualTo, segVal);
                oneReport = _dataManager.GetReportRanked(provider, reportType, startDate, endDate, criteria);
                if (oneReport != null)
                {
                    mergedReport.Add(oneReport);
                }
            }
        }
        else
        {
            oneReport = _dataManager.GetReportRanked(provider, reportType, startDate, endDate, criteria);
            if (oneReport != null)
            {
                mergedReport.Add(oneReport);
            }
        }
        return mergedReport;
    }

    private void bindData()
    {
        gvDataTable.Visible = false;
        AnalyticsDetail.Visible = false;
        AnalyticsPieChart.Visible = false;
        if (null == _reports || 0 == _reports.Count) return;

		DisplayView view = this.View;

		string urlBreadcrumb = Request.Url.AbsolutePath;
		lnkBreadcrumbReport.Visible = false;
		lnkBreadcrumbFor.Visible = false;
		lnkBreadcrumbAnd.Visible = false;
		lnkBreadcrumbReport.InnerText = _reportTypeName;
		urlBreadcrumb += "?report=" + EkFunctions.UrlEncode(_reportType.ToString());
		lnkBreadcrumbReport.HRef = urlBreadcrumb + "&for=&and=&also=&view=";
		if (!String.IsNullOrEmpty(_forValue))
		{
			lnkBreadcrumbReport.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbReport);
			htmBreadcrumbSeparatorFor.Visible = true;
			lblBreadcrumbFor.Visible = true;
			lblBreadcrumbFor.Text = _forValue;
			lnkBreadcrumbFor.InnerText = lblBreadcrumbFor.Text;
			urlBreadcrumb += "&for=" + EkFunctions.UrlEncode(lnkBreadcrumbFor.InnerText);
			lnkBreadcrumbFor.HRef = urlBreadcrumb + "&and=&also=&view=";
		}
		if (!String.IsNullOrEmpty(_andValue))
		{
			lblBreadcrumbFor.Visible = false;
			lnkBreadcrumbFor.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbFor);
			htmBreadcrumbSeparatorAnd.Visible = true;
			lblBreadcrumbAnd.Visible = true;
			lblBreadcrumbAnd.Text = _andValue;
			lnkBreadcrumbAnd.InnerText = lblBreadcrumbAnd.Text;
			urlBreadcrumb += "&and=" + EkFunctions.UrlEncode(lnkBreadcrumbAnd.InnerText);
			lnkBreadcrumbAnd.HRef = urlBreadcrumb + "&also=&view=";
		}
		if (!String.IsNullOrEmpty(_alsoValue))
		{
			lblBreadcrumbAnd.Visible = false;
			lnkBreadcrumbAnd.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbAnd);
			htmBreadcrumbSeparatorAlso.Visible = true;
			lblBreadcrumbAlso.Visible = true;
			lblBreadcrumbAlso.Text = _alsoValue;
		}

		if (DisplayView.Detail == view)
		{
			AnalyticsDetail.Visible = true;
            AnalyticsDetail.ProviderSegments = this.ProviderSegments;
			AnalyticsDetail.StartDate = _startDate;
			AnalyticsDetail.EndDate = _endDate;

			switch (_reportDisplayData)
			{
				case ReportDisplayData.SiteData:
					AnalyticsDetail.ShowVisits = true;
					AnalyticsDetail.ShowPagesPerVisit = true;
					AnalyticsDetail.ShowTimeOnSite = true;
					AnalyticsDetail.ShowBounceRate = true;
					this.htmReportSummary.Visible = false;
					break;
				case ReportDisplayData.PageData:
					AnalyticsDetail.ShowPageviews = true;
					AnalyticsDetail.ShowUniqueViews = true;
					AnalyticsDetail.ShowTimeOnPage = true;
					AnalyticsDetail.ShowBounceRate = true;
					AnalyticsDetail.ShowPercentExit = true;
					break;
				case ReportDisplayData.LandingPageData:
					AnalyticsDetail.ShowBounceRate = true;
					this.htmReportSummary.Visible = false;
					break;
				case ReportDisplayData.ExitPageData:
					AnalyticsDetail.ShowPageviews = true;
					AnalyticsDetail.ShowPercentExit = true;
					this.htmReportSummary.Visible = false;
					break;
                case ReportDisplayData.DynamicData:
                    this.htmReportSummary.Visible = false;
                    break;
                case ReportDisplayData.SiteMetricData:
                case ReportDisplayData.SiteContentData:
                case ReportDisplayData.InstancesData:
                case ReportDisplayData.MobileData:
                case ReportDisplayData.ReloadData:
                case ReportDisplayData.PageVisitorsData:
                case ReportDisplayData.PageVisitsData:
                case ReportDisplayData.VisitorsData:
                case ReportDisplayData.VisitsData:
				default:
					if (this.htmReportSummary != null)
                    {
						this.htmReportSummary.InnerText = "This report does not support the summary type.";
						this.htmReportSummary.Visible = true;
						break;
					}
					else
					{
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
					}
			}

			AnalyticsDetail.UpdateReport(_reports);
		}
		else if (_showTable)
		{
			gvDataTable.Visible = true;
			ReportTableColumn[] visibleColumns = new ReportTableColumn[] { ReportTableColumn.DimensionName };
			if (DisplayView.Table == view)
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Visits,
							ReportTableColumn.PagesPerVisit,
							ReportTableColumn.AverageTimeSpanOnSite,
							ReportTableColumn.PercentNewVisits,
							ReportTableColumn.BounceRate
						};
						break;
					case ReportDisplayData.PageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.PageViews,
							ReportTableColumn.UniqueViews,
							ReportTableColumn.AverageTimeSpanOnPage,
							ReportTableColumn.BounceRate,
							ReportTableColumn.ExitRate
						};
						break;
					case ReportDisplayData.LandingPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Entrances,
							ReportTableColumn.Bounces,
							ReportTableColumn.BounceRate
						};
						break;
					case ReportDisplayData.ExitPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Exits,
							ReportTableColumn.PageViews,
							ReportTableColumn.ExitRate
						};
						break;
                    case ReportDisplayData.SiteMetricData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.Visits,
							ReportTableColumn.PageViews
						};
                        break;
                    case ReportDisplayData.SiteContentData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.PageViews
						};
                        break;
                    case ReportDisplayData.InstancesData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Instances
						};
                        break;
                    case ReportDisplayData.MobileData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.MobileViews 
                        };
                        break;
                    case ReportDisplayData.ReloadData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.Reloads
                        };
                        break;
                    case ReportDisplayData.PageVisitorsData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.PageViews,
                            ReportTableColumn.Visitors 
                        };
                        break;
                    case ReportDisplayData.VisitorsData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.Visitors 
                        };
                        break;
                    case ReportDisplayData.VisitsData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.Visits 
                        };
                        break;
                    case ReportDisplayData.PageVisitsData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.PageViews,
                            ReportTableColumn.Visits 
                        };
                        break;
                    case ReportDisplayData.DynamicData:
                        gvDataTable.Visible = false;
                        break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
            else if (DisplayView.Percentage == view)
            {
                switch (_reportDisplayData)
                {
                    case ReportDisplayData.SiteData:
                    case ReportDisplayData.SiteMetricData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Visits,
							ReportTableColumn.PercentVisits
						};
                        break;
                    case ReportDisplayData.PageData:
                        visibleColumns = new ReportTableColumn[] 
						    { 
							    ReportTableColumn.DimensionName,
							    ReportTableColumn.PageViews,
							    ReportTableColumn.PercentPageviews
						    };
                        break;
                    case ReportDisplayData.SiteContentData:
                        visibleColumns = new ReportTableColumn[] 
						    { 
							    ReportTableColumn.DimensionName,
							    ReportTableColumn.PageViews
						    };
                        break;
                    case ReportDisplayData.LandingPageData:
                        visibleColumns = new ReportTableColumn[] 
						    { 
							    ReportTableColumn.DimensionName,
							    ReportTableColumn.Entrances,
							    ReportTableColumn.BounceRate
						    };
                        break;
                    case ReportDisplayData.ExitPageData:
                        visibleColumns = new ReportTableColumn[] 
						    { 
							    ReportTableColumn.DimensionName,
							    ReportTableColumn.Exits,
							    ReportTableColumn.ExitRate
						    };
                        break;
                    case ReportDisplayData.InstancesData:
                        visibleColumns = new ReportTableColumn[] 
						    { 
							    ReportTableColumn.DimensionName,
							    ReportTableColumn.Instances
						    };
                        break;
                    case ReportDisplayData.MobileData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.MobileViews
						};
                        break;
                    case ReportDisplayData.PageVisitorsData:
                    case ReportDisplayData.VisitorsData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Visitors
						};
                        break;
                    case ReportDisplayData.VisitsData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Visits
						};
                        break;
                    case ReportDisplayData.PageVisitsData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.PageViews,
							ReportTableColumn.Visits
						};
                        break;
                    case ReportDisplayData.SalesCycleData:
                    case ReportDisplayData.NewBuyerSalesCycleData:
                        visibleColumns = new ReportTableColumn[] 
						{ 
                            ReportTableColumn.DimensionName,
							ReportTableColumn.Revenue
						};
                        break;
                    case ReportDisplayData.ReloadData:
                        visibleColumns = new ReportTableColumn[] 
                        { 
                            ReportTableColumn.DimensionName,
                            ReportTableColumn.Reloads
                        };
                        break;
                    case ReportDisplayData.DynamicData:
                        gvDataTable.Visible = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
                }
            }

            if (string.IsNullOrEmpty(this.ReportGUID))
            {
                GrdDynamic.Visible = false;
                for (int iGridCol = 0; iGridCol < gvDataTable.Columns.Count; iGridCol++)
                {
                    DataControlField column = gvDataTable.Columns[iGridCol];
                    column.Visible = false;
                    for (int iVisCol = 0; iVisCol < visibleColumns.Length; iVisCol++)
                    {
                        if (visibleColumns[iVisCol] == (ReportTableColumn)iGridCol)
                        {
                            if (iVisCol >= 0)
                            {
                                if (0 == iGridCol)
                                {
                                    column.HeaderText = _columnName;
                                }
                                else
                                {
                                    column.HeaderText = GetEnumDisplayText(visibleColumns[iVisCol]);
                                }
                                column.Visible = true;
                            }
                            break;
                        }
                    }
                }

                if (null == this.ProviderSegments || this.ProviderSegments.Count <= 1)
                {
                    gvDataTable.DataSource = _reports[0].ReportItems;
                }
                else
                {
                    this.gvDataTable.DataSource = CreateSegmentReportList();
                }
                gvDataTable.DataBind();
            }
            else
            {
                GrdDynamic.DataSource = null;
                GrdDynamic.DataBind();
                gvDataTable.Visible = false;
                if (_reports[0].ReportDataSet.ResponseDataSet != null)
                {
                    //dynamic table
                    GrdDynamic.Visible = true;
                    DataTable dt = new DataTable();
                    string tableName = "data";
                    if (!String.IsNullOrEmpty(_forValue))
                    {
                        tableName = _forValue;
                    }
                    dt = _reports[0].ReportDataSet.ResponseDataSet.Tables[tableName];
                    int maxColumns = dt.Columns.Count;
                    if (view != DisplayView.Table)
                    {
                        maxColumns = 4;
                    }
                    if (dt != null)
                    {
                        //Iterate through the columns of the datatable to set the data bound field dynamically.
                        int colCount = 0;
                        GrdDynamic.Columns.Clear();
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (colCount >= maxColumns) break;
                            //Declare the bound field and allocate memory for the bound field.
                            BoundField bfield = new BoundField();

                            //Initalize the DataField value.
                            bfield.DataField = col.ColumnName;

                            //Initialize the HeaderText field value.
                            bfield.HeaderText = col.ColumnName;

                            //Add the newly created bound field to the GridView.
                            bfield.HeaderStyle.CssClass = "headerstyle";
                            GrdDynamic.Columns.Add(bfield);
                            colCount++;
                        }
                        GrdDynamic.AlternatingRowStyle.CssClass = "alternatingrowstyle";
                    }
                    GrdDynamic.DataSource = dt;
                    int visualIndex = Request.Form[drpVisualization.UniqueID] != null ? EkFunctions.ReadIntegerValue(Request.Form[drpVisualization.UniqueID]) : 0;
                    string onchange = "javascript" + @":setTimeout('__doPostBack(\'\',\'\')', 0)";
                    if (string.IsNullOrEmpty(drpVisualization.Attributes["onchange"]))
                        drpVisualization.Attributes.Add("onchange", onchange);

                    if (!Page.IsPostBack || drpVisualization.Items.Count == 0)
                    {
                        drpVisualization.Items.Add(new ListItem(GetMessage("lbl data table"), "0"));
                        drpVisualization.Items.Add(new ListItem(GetMessage("lbl bar chart"), "4"));
                        drpVisualization.Items.Add(new ListItem(GetMessage("lbl heat map"), "5"));
                        drpVisualization.Items.Add(new ListItem(GetMessage("lbl pie chart"), "1"));
                    }
                    drpVisualization.SelectedValue = visualIndex.ToString();
                    dvVisualSelect.Visible = true;
                    this.View = (DisplayView)Enum.Parse(typeof(DisplayView), visualIndex.ToString()); 
                    GrdDynamic.DataBind();
                }
            }
			lblNoRecords.Visible = false;
			pnlData.Visible = true;
		}
		else if (_showPieChart)
		{
			AnalyticsPieChart.Visible = true;
			UpdatePieChart(AnalyticsPieChart, _reports[0]);
		}
	}

	private string GetEnumDisplayText(Enum enumValue)
	{
		System.Reflection.FieldInfo enumInfo = enumValue.GetType().GetField(enumValue.ToString());
		DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])enumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (enumAttributes != null && enumAttributes.Length > 0)
		{
			string displayText = enumAttributes[0].Description;
			if (String.IsNullOrEmpty(displayText))
			{
				return enumValue.ToString();
			}
			else if (displayText.StartsWith("{") && displayText.EndsWith("}"))
			{
				return GetMessage(displayText.Trim(new char[] { '{', '}' }));
			}
			else
			{
				return displayText;
			}
		}
		return enumValue.ToString();
	}

    protected string GetReportName(int index) {
        ReportItem item = GetReportItem(index);
        if (item == null)
            return string.Empty;

        return item.Name;
    }

    protected AnalyticsReportItem GetReportItem(int index) 
    {
        if (_reports.Count > 1)
        {
            return (_segReport != null ? ((_segReport.Count >= index) ? _segReport[index] : null) : null);
        }
        else
        {
            return (_reports[0] != null ? ((_reports[0].ReportItems.Count >= index) ? _reports[0].ReportItems[index] : null) : null);
        }
    }

    public string FormatPercent(object itemValue)
    {
        return itemValue + "%";
    }
    public string GetPercentVisits(object itemValue) {
		int value = Convert.ToInt32(itemValue);
		return EkFunctions.GetPercent(value, _reports[0].TotalVisits).ToString("0.00%");
    }

	public string GetPercentPageviews(object itemValue)
	{
        int value = Convert.ToInt32(itemValue);
		return EkFunctions.GetPercent(value, _reports[0].TotalPageViews).ToString("0.00%");
    }

	protected virtual void UpdatePieChart(controls_reports_PercentPieChart pieChart, AnalyticsReportData report)
	{
		List<float> data = new List<float>();
		List<string> colors = new List<string>();
		List<string> names = new List<string>();
		for (int i = 0; i < report.ReportItems.Count; i++)
		{
			float itemPercent = 0;
			switch (_reportDisplayData)
			{
				case ReportDisplayData.SiteData:
                case ReportDisplayData.SiteMetricData:
                case ReportDisplayData.VisitsData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Visits, report.TotalVisits);
					break;
				case ReportDisplayData.PageData:
                case ReportDisplayData.SiteContentData:
                case ReportDisplayData.PageVisitorsData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].PageViews, report.TotalPageViews);
					break;
				case ReportDisplayData.LandingPageData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Entrances, report.TotalEntrances);
					break;
				case ReportDisplayData.ExitPageData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Exits, report.TotalExits);
					break;
                case ReportDisplayData.VisitorsData:
                case ReportDisplayData.InstancesData:
                case ReportDisplayData.MobileData:
				default:
				    if (this.htmReportSummary != null)
                    {
						this.htmReportSummary.InnerText = "This report does not support the graph only type.";
						break;
					}
					else
					{
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
					}
			}
			data.Add(itemPercent);
			colors.Add(GetReportItemColor(i));
            if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
            {
                names.Add(_segReport[i * (this.ProviderSegments.Count + 1)].Name);
            }
            else
            {
                names.Add(report.ReportItems[i].Name);
            }
		}

		pieChart.BriefDescription = GetMessage("lbl contribution to total");
		pieChart.LoadData(data);
		pieChart.LoadColors(colors);
		pieChart.LoadNames(names);
	}
    protected virtual void UpdatePieChart(controls_reports_PercentPieChart pieChart, DataTable dataTable, int columnIndex)
    {
        bool timeValues = false;
        decimal columTotal = GetColumnTotal(columnIndex);
        List<float> data = new List<float>();
        List<string> colors = new List<string>();
        List<string> names = new List<string>();
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            if (dataTable.Rows[i][0].ToString().ToLower() != "total")
            {
                float itemPercent = EkFunctions.GetPercent(
                                        EkFunctions.ReadDecimalValue(dataTable.Rows[i][columnIndex])
                                        , columTotal
                                    );
                if (itemPercent > 1)
                {
                    timeValues = true;
                    break;
                }
                else
                {
                    data.Add(itemPercent);
                    colors.Add(GetReportItemColor(i));
                    names.Add(dataTable.Rows[i][0].ToString());
                }
            }
        }
        pieChart.BriefDescription = GetMessage("lbl contribution to total");
        pieChart.LoadData(data);
        pieChart.LoadColors(colors);
        pieChart.LoadNames(names);
    }
    protected void GvDataTable_RowCreated(object sender, GridViewRowEventArgs e)
    {
		if (DisplayView.Percentage == this.View && _showPieChart)
		{
			if (e.Row.RowType == DataControlRowType.Header)
			{

				TableHeaderCell tc = new TableHeaderCell();
				tc.Attributes.Add("scope", "col");
				tc.Controls.Add(new LiteralControl(GetMessage("lbl contribution to total")));
				//DropDownList contribDropdown = new DropDownList();
				//contribDropdown.ReportItems.Add(new ListItem("Visits", "0"));
				//contribDropdown.ReportItems.Add(new ListItem("Views", "1"));
				//tc.Controls.Add(contribDropdown);
				e.Row.Cells.Add(tc);

			}
			else if (e.Row.RowType == DataControlRowType.Pager)
			{

			}
			else if (e.Row.RowType == DataControlRowType.DataRow)
			{

				if (e.Row.RowIndex == 0)
				{

					TableCell tc = new TableCell();

					tc.RowSpan = 1;
					tc.VerticalAlign = VerticalAlign.Middle;
					tc.HorizontalAlign = HorizontalAlign.Center;
                    
					if (_reports != null && _reports.Count > 0)
					{
						controls_reports_PercentPieChart pieChart = (controls_reports_PercentPieChart)LoadControl(CommonApi.AppPath + "controls/reports/PercentPieChart.ascx");
						pieChart.Width = new Unit(250, UnitType.Pixel);
						pieChart.Height = new Unit(250, UnitType.Pixel);
						pieChart.Legend = controls_reports_PercentPieChart.LegendPosition.BottomVertical;

						UpdatePieChart(pieChart, _reports[0]);
                        tc.CssClass = "pieChart";
						tc.Controls.Add(pieChart);
					}

					e.Row.Cells.Add(tc);

				}
				else
				{

					TableCell tc = gvDataTable.Rows[0].Cells[gvDataTable.Rows[0].Cells.Count - 1];
					tc.RowSpan = tc.RowSpan + 1;

				}
			}
		}
    }

    protected void GvDataTable_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList dropDown = (DropDownList)sender;
        int pageSize = int.Parse(dropDown.SelectedValue);
        if (this.ProviderSegments != null)
        {
            int segCount = this.ProviderSegments.Count;
            if (segCount > 1)
            {
                pageSize = pageSize * (segCount + 1);
            }
        }
        this.gvDataTable.PageSize = pageSize;
        _reports = GetAnalyticsReport(_providerName);
        bindData();
    }

    protected void GoToPage_TextChanged(object sender, EventArgs e)
    {
        TextBox txtGoToPage = (TextBox)sender;

        int pageNumber;
        if (int.TryParse(txtGoToPage.Text.Trim(), out pageNumber) && pageNumber > 0)
        {
            if (pageNumber <= this.gvDataTable.PageCount)
                this.gvDataTable.PageIndex = pageNumber - 1;
            if (pageNumber <= this.GrdDynamic.PageCount)
                this.GrdDynamic.PageIndex = pageNumber - 1;
        }
        else
        {
            this.gvDataTable.PageIndex = 0;
            this.GrdDynamic.PageIndex = 0;
        }
        _reports = GetAnalyticsReport(_providerName);
        bindData();
    }

    protected void GvDataTable_OnSorting(object sender, GridViewSortEventArgs e)
    {
		string sortExpression = e.SortExpression;

		if (String.IsNullOrEmpty(sortExpression)) return;

		SortDirection sortDirection = (sortExpression == AnalyticsSortableField.Name.ToString() ? SortDirection.Ascending : SortDirection.Descending);

		string prevSortExpression = ViewState["SortExpression"] as string;
		if (prevSortExpression != null && ViewState["SortDirection"] != null)
		{
			// Check if the same column is being sorted.
			// Otherwise, the default value can be returned.
			if (sortExpression == prevSortExpression)
			{
				if (SortDirection.Ascending == (SortDirection)ViewState["SortDirection"])
				{
					sortDirection = SortDirection.Descending;
				}
				else
				{
					sortDirection = SortDirection.Ascending;
				}
			}
		}

		ViewState["SortExpression"] = sortExpression;
		ViewState["SortDirection"] = sortDirection;

		_reports = GetAnalyticsReport(_providerName);
        bindData();
    }

	protected void GvDataTable_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		gvDataTable.PageIndex = e.NewPageIndex;
		RefreshAnalyticsReport(sender, e);
	}

	protected void GvDataTable_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
		{
			Label lblNameValue = (Label)e.Row.FindControl("lblNameValue");
			Label lblNameValueLinked = (Label)e.Row.FindControl("lblNameValueLinked");
			if (lblNameValue != null)
			{
				int index = e.Row.DataItemIndex;
				string name = string.Empty;
                bool needDrillDownLink = true;
                if (_reports.Count > 1 && this.ProviderSegments != null)
                {
                    int rptIndex = index % (this.ProviderSegments.Count + 1);
                    if (0 == rptIndex)
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            if (!e.Row.Cells[i].CssClass.Contains("pieChart"))
                            {
                                e.Row.Cells[i].Text = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        needDrillDownLink = false;
                    }
                    name = GetReportName(index);
                }
                else
                {
                    name = GetReportName(index);
                }
                string truncatedName = string.Empty;
                if (name != null)
                {
                    truncatedName = (name.Length > 58 ? name.Substring(0, 58) : name);
                }
                //if (_showPieChart && DisplayView.Percentage == this.View)
				//{
				//    Image imgColorBox = (Image)e.Row.FindControl("imgColorBox");
				//    if (imgColorBox != null)
				//    {
				//        imgColorBox.ImageUrl = _refContentApi.AppImgPath + "transparent.gif";
				//        imgColorBox.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#" + GetReportItemColor(e.Row.RowIndex));
				//        imgColorBox.Visible = true;
				//    }
				//}
				//else 
                if (true == needDrillDownLink && ReportType.TopContent == _reportType)
				{
					HyperLink lnkGo = (HyperLink)e.Row.FindControl("lnkGo");
					if (lnkGo != null)
					{
						lnkGo.ImageUrl = _refContentApi.AppPath + "images/UI/Icons/linkGo.png";
                        string siteUrl = _dataManager.GetProviderSiteURL(_providerName);
                        lnkGo.NavigateUrl = (siteUrl.IndexOf("http") == 0 ? siteUrl : "http://" + siteUrl) + name;
						lnkGo.Attributes.Add("title", GetMessage("lbl visit this page"));
						lnkGo.Visible = true;
					}
				}
				System.Web.UI.HtmlControls.HtmlAnchor lnkDrillDown = (System.Web.UI.HtmlControls.HtmlAnchor)e.Row.FindControl("lnkDrillDown");
                if (true == needDrillDownLink && lnkDrillDown != null)
				{
					lblNameValueLinked.Text = truncatedName;
					if (_bDrillDownReport || _bDrillDownDetail)
					{
						if (Request.RawUrl.Contains("?"))
						{
							lnkDrillDown.HRef = Request.RawUrl + "&" + _strDrillDownArg + "=" + EkFunctions.UrlEncode(name);
						}
						else
						{
							StringBuilder sbUrl = new StringBuilder(Request.RawUrl);
							sbUrl.Append("?report=").Append(_reportType.ToString());
							if (!String.IsNullOrEmpty(_forValue) && _strDrillDownArg != "for")
							{
								sbUrl.Append("&for=").Append(EkFunctions.UrlEncode(_forValue));
							}
							if (!String.IsNullOrEmpty(_andValue) && _strDrillDownArg != "and")
							{
								sbUrl.Append("&and=").Append(EkFunctions.UrlEncode(_andValue));
							}
							sbUrl.Append("&").Append(_strDrillDownArg).Append("=").Append(EkFunctions.UrlEncode(name));
							lnkDrillDown.HRef = sbUrl.ToString();
						}
						if (_bDrillDownDetail)
						{
							lnkDrillDown.HRef += "&view=detail";
						}
						AddUpdatePanelTrigger(lnkDrillDown);
					}
					else
					{
						lnkDrillDown.Visible = false;
						lblNameValue.Text = truncatedName;
						lblNameValue.Visible = true;
					}
				}
				else
				{
					lblNameValue.Text = truncatedName;
					lblNameValue.Visible = true;
				}
			}
		}

        GridView gridView = (GridView)sender;

		string sortField = _sortField.ToString();
        int cellIndex = -1;
        foreach (DataControlField field in gridView.Columns)
        {
			if (field.SortExpression == sortField)
            {
                cellIndex = gridView.Columns.IndexOf(field);
                break;
            }
        }

        if (cellIndex > -1)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //  this is a header row,
                //  set the sort style
				string className = "";
				try
				{
					className = ((DataControlFieldCell)e.Row.Cells[cellIndex]).ContainingField.HeaderStyle.CssClass;
					className = System.Text.RegularExpressions.Regex.Replace(className, @"\s*sort(asc|desc)headerstyle\s*", " ");
					className += " ";
				}
				catch { }
				e.Row.Cells[cellIndex].CssClass = className + (_sortDirection == EkEnumeration.OrderByDirection.Ascending ? "sortascheaderstyle" : "sortdescheaderstyle");
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //  this is an alternating row
				string className = "";
				try
				{
					className = ((DataControlFieldCell)e.Row.Cells[cellIndex]).ContainingField.ItemStyle.CssClass;
					className = System.Text.RegularExpressions.Regex.Replace(className, @"\s*sort(alternating)?rowstyle\s*", " ");
					className += " ";
				}
				catch { }
				e.Row.Cells[cellIndex].CssClass = className + (e.Row.RowIndex % 2 == 0 ? "sortalternatingrowstyle" : "sortrowstyle");
            }
        }

        if (e.Row.RowType == DataControlRowType.Pager)
        {

            Label lblGoTo = (Label)e.Row.FindControl("lblGoTo");
            lblGoTo.ToolTip= lblGoTo.Text = GetMessage("lbl go to");

            Label lblTotalNumberOfPages = (Label)e.Row.FindControl("lblTotalNumberOfPages");
            TextBox txtGoToPage = (TextBox)e.Row.FindControl("txtGoToPage");
            txtGoToPage.Text = (gridView.PageIndex + 1).ToString();
            lblTotalNumberOfPages.Text = String.Format(GetMessage("lbl current page of total pages"), txtGoToPage.Text, gridView.PageCount);
            lblTotalNumberOfPages.ToolTip = lblTotalNumberOfPages.Text;

			Label lblShowRows = (Label)e.Row.FindControl("lblShowRows");
            lblShowRows.Text = GetMessage("lbl show rows");
            lblShowRows.ToolTip = lblShowRows.Text;

            DropDownList ddlPageSize = (DropDownList)e.Row.FindControl("ddlPageSize");
            int pageSetSize = gridView.PageSize;
            if (_reports.Count > 1 && this.ProviderSegments != null)
            {
                pageSetSize = gridView.PageSize / (_reports.Count + 1);
            }
            ddlPageSize.SelectedValue = pageSetSize.ToString();
        }
    }

	private void AddUpdatePanelTrigger(System.Web.UI.HtmlControls.HtmlAnchor linkControl)
	{
		AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
		trigger.ControlID = linkControl.UniqueID;
		trigger.EventName = "ServerClick";
		UpdatePanel1.Triggers.Add(trigger);
	}

	protected virtual void HtmlLink_OnServerClick(object sender, EventArgs e)
	{
		System.Web.UI.HtmlControls.HtmlAnchor link = sender as System.Web.UI.HtmlControls.HtmlAnchor;
		string url = link.HRef;
		url = url.Substring(url.IndexOf('?'));

		System.Collections.Specialized.NameValueCollection queryString = HttpUtility.ParseQueryString(url);

		LoadStateFromQueryString(queryString);

		RefreshAnalyticsReport(sender, e);
	}

	protected virtual void AnalyticsDetail_SelectionChanged(object sender, EventArgs e)
	{
		_reports = GetAnalyticsReport(_providerName);
		AnalyticsDetail.StartDate = _startDate;
		AnalyticsDetail.EndDate = _endDate;
		AnalyticsDetail.UpdateReport(_reports);
	}

	public virtual void LoadStateFromQueryString(System.Collections.Specialized.NameValueCollection queryString)
	{
		string report = queryString["report"];
		if (!String.IsNullOrEmpty(report))
		{
            try
            {
                this.Report = (ReportType)Enum.Parse(typeof(ReportType), report, true); // ignore case
            }
            catch
            {
                this.Report = ReportType.WebTrendsReport;
                this.ReportGUID = report;
            }
		}
		string forValue = queryString["for"];
		if (forValue != null)
		{
			this.ForValue = forValue;
		}
		string andValue = queryString["and"];
		if (andValue != null)
		{
			this.AndValue = andValue;
		}
		string alsoValue = queryString["also"];
		if (alsoValue != null)
		{
			this.AlsoValue = alsoValue;
		}
		string displayView = queryString["view"];
		if (displayView != null)
		{
			if (displayView.Length > 0)
			{
				this.View = (DisplayView)Enum.Parse(typeof(DisplayView), displayView, true); // ignore case
			}
			else
			{
				this.View = DisplayView.Default;
			}
		}
	}
    protected int GetHeatColorValue(float percentOfMax)
    {
        return GetColorValue(percentOfMax, 10);
    }
    protected int GetHeatTextColorValue(float percentOfMax)
    {
        return 255 - GetColorValue(percentOfMax, 3);
    }
    private int GetColorValue(float percentOfMax, int sections)
    {
        return (255 - Convert.ToInt32(255f * (Math.Round(percentOfMax * sections) / sections)));
    }
    protected decimal GetColumnMax(int columnIndex)
    {
        decimal columnMax = 0;
        DataTable data = (DataTable)GrdDynamic.DataSource;
        if (data != null)
        {
            for (int i = 0; i < data.Rows.Count; i++)
            {
                decimal currentValue = EkFunctions.ReadDecimalValue(data.Rows[i][columnIndex]);
                if (currentValue > columnMax && 
                    data.Rows[i][0].ToString().ToLower() != "total")
                    columnMax = currentValue;
            }
        }
        return columnMax;
    }
    protected Dictionary<string, int> GetColumns()
    {
        Dictionary<string, int> columns = new Dictionary<string,int>();
        DataTable data = (DataTable)GrdDynamic.DataSource;
        if (data != null)
        {
            for (int i = 1; i < data.Columns.Count; i++)
            {
                columns.Add(data.Columns[i].ColumnName, i);
            }
        }
        return columns;
    }
    protected decimal GetColumnTotal(int columnIndex)
    {
        decimal columnTotal = 0;
        DataTable data = (DataTable)GrdDynamic.DataSource;
        if (data != null)
        {
            for (int i = (data.Rows.Count - 1); i >= 0; i--)
            {
                decimal currentValue = EkFunctions.ReadDecimalValue(data.Rows[i][columnIndex]);
                if (data.Rows[i][0].ToString().ToLower() == "total" && currentValue > 0)
                {
                    columnTotal = currentValue;
                    break;
                }
                else
                {
                    columnTotal += currentValue;
                }
            }
        }
        return columnTotal;
    }
    protected void GrdDynamic_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (DisplayView.Percentage == this.View)
        {
            int colIndex = Request.Form["AnalyticsReport$GrdDynamic$ctl01$ctl00"] != null ? EkFunctions.ReadIntegerValue(Request.Form["AnalyticsReport$GrdDynamic$ctl01$ctl00"]) : 1;
            if (e.Row.RowType == DataControlRowType.Header)
            {

                TableHeaderCell tc = new TableHeaderCell();
                tc.Attributes.Add("scope", "col");
                tc.Controls.Add(new LiteralControl(GetMessage("lbl contribution to total") + "<br/>"));
                e.Row.Cells.Add(tc);

                DropDownList dataColumns = new DropDownList();
                foreach (KeyValuePair<string, int> col in GetColumns())
                    dataColumns.Items.Add(new ListItem(col.Key, col.Value.ToString()));
                
                dataColumns.SelectedValue = colIndex.ToString();
                string onchange = "javascript" + @":setTimeout('__doPostBack(\'\',\'\')', 0)";
                dataColumns.Attributes.Add("onchange", onchange);
                
                tc.Controls.Add(dataColumns);

            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {

                if (e.Row.RowIndex == 0)
                {

                    TableCell tc = new TableCell();

                    tc.RowSpan = 1;
                    tc.VerticalAlign = VerticalAlign.Middle;
                    tc.HorizontalAlign = HorizontalAlign.Center;
                    DataTable data = (DataTable)GrdDynamic.DataSource;
                    if (data != null && data.Rows.Count > 0)
                    {
                        controls_reports_PercentPieChart pieChart = (controls_reports_PercentPieChart)LoadControl(CommonApi.AppPath + "controls/reports/PercentPieChart.ascx");
                        pieChart.Width = new Unit(250, UnitType.Pixel);
                        pieChart.Height = new Unit(250, UnitType.Pixel);
                        pieChart.Legend = controls_reports_PercentPieChart.LegendPosition.BottomVertical;

                        UpdatePieChart(pieChart, data, colIndex);
                        tc.CssClass = "pieChart";
                        tc.Controls.Add(pieChart);
                    }

                    e.Row.Cells.Add(tc);

                }
                else
                {

                    TableCell tc = GrdDynamic.Rows[0].Cells[GrdDynamic.Rows[0].Cells.Count - 1];
                    tc.RowSpan = tc.RowSpan + 1;

                }
            }
        }
    }
    protected void GrdDynamic_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (DisplayView.Bar == this.View || DisplayView.Heat == this.View)
        {
            DataTable dt = (DataTable)GrdDynamic.DataSource;
            for (int i = (e.Row.Cells.Count - 1); i > 0; i--)
            {

                decimal columnMax = GetColumnMax(i);
                decimal currentValue = EkFunctions.ReadDecimalValue(e.Row.Cells[i].Text);
                float percentOfMax = EkFunctions.GetPercent(currentValue, columnMax);
                if (percentOfMax > 1) percentOfMax = 1;
                switch (this.View)
                {
                    case DisplayView.Bar:
                        e.Row.Cells[i].CssClass += "datavalue";
                        if (e.Row.RowType == DataControlRowType.DataRow)
                            e.Row.Cells[i].Text = string.Format(@"<div class=""value_wrap""><span class=""value"">{0}</span><span class=""indexHolder""><span style=""width: {1}%;"" class=""index"">&nbsp;</span></span></div>", e.Row.Cells[i].Text, (percentOfMax * 100).ToString(".0"));
                        break;
                    case DisplayView.Heat:
                        int heatColor = GetHeatColorValue(percentOfMax);
                        int heatTextColor = GetHeatTextColorValue(percentOfMax);
                        if (e.Row.RowType == DataControlRowType.DataRow)
                        {
                            e.Row.Cells[i].Attributes.Add("style", string.Format("background-color: rgb({0}, {0}, {0});color: rgb({1}, {1}, {1});", heatColor, heatTextColor));
                        }
                        break;
                }
            }
        }
        else if (e.Row.RowType == DataControlRowType.DataRow && _bDrillDownReport && DisplayView.Table == this.View)
        {
            string name = e.Row.Cells[0].Text; 
            System.Web.UI.HtmlControls.HtmlAnchor lnkDrillDown = new System.Web.UI.HtmlControls.HtmlAnchor(); 
            if (Request.RawUrl.Contains("?"))
            {
                string pattern = @"&for(\=[^&]*)?(?=&|$)|^for(\=[^&]*)?(&|$)";
                Regex rgx = new Regex(pattern);
                string hRef = rgx.Replace(Request.RawUrl, "");
                lnkDrillDown.HRef = hRef + "&" + _strDrillDownArg + "=" + EkFunctions.UrlEncode(name);
            }
            else
            {
                StringBuilder sbUrl = new StringBuilder(Request.RawUrl);
                sbUrl.Append("?report=").Append(_reportType.ToString());
                if (!String.IsNullOrEmpty(_forValue) && _strDrillDownArg != "for")
                {
                    sbUrl.Append("&for=").Append(EkFunctions.UrlEncode(_forValue));
                }
                if (!String.IsNullOrEmpty(_andValue) && _strDrillDownArg != "and")
                {
                    sbUrl.Append("&and=").Append(EkFunctions.UrlEncode(_andValue));
                }
                sbUrl.Append("&").Append(_strDrillDownArg).Append("=").Append(EkFunctions.UrlEncode(name));
                lnkDrillDown.HRef = sbUrl.ToString();
            }
            lnkDrillDown.InnerText = name;
            e.Row.Cells[0].Controls.Clear();
            e.Row.Cells[0].Controls.Add(lnkDrillDown);
            AddUpdatePanelTrigger(lnkDrillDown);
        }
        GridView gridView = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.Pager)
        {

            Label lblGoTo = (Label)e.Row.FindControl("lblGoTo");
            lblGoTo.ToolTip= lblGoTo.Text = GetMessage("lbl go to");

            Label lblTotalNumberOfPages = (Label)e.Row.FindControl("lblTotalNumberOfPages");
            TextBox txtGoToPage = (TextBox)e.Row.FindControl("txtGoToPage");
            txtGoToPage.Text = (gridView.PageIndex + 1).ToString();
            lblTotalNumberOfPages.Text = String.Format(GetMessage("lbl current page of total pages"), txtGoToPage.Text, gridView.PageCount);
            lblTotalNumberOfPages.ToolTip = lblTotalNumberOfPages.Text;

            Label lblShowRows = (Label)e.Row.FindControl("lblShowRows");
            lblShowRows.Text = GetMessage("lbl show rows");
            lblShowRows.ToolTip = lblShowRows.Text;

            DropDownList ddlPageSize = (DropDownList)e.Row.FindControl("ddlPageSize");
            int pageSetSize = gridView.PageSize;
            if (_reports.Count > 1 && this.ProviderSegments != null)
            {
                pageSetSize = gridView.PageSize / (_reports.Count + 1);
            }
            ddlPageSize.SelectedValue = pageSetSize.ToString();
        }
    }

    protected void GrdDynamic_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdDynamic.PageIndex = e.NewPageIndex;
        RefreshAnalyticsReport(sender, e);
    }

    protected void GrdDynamic_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList dropDown = (DropDownList)sender;
        int pageSize = int.Parse(dropDown.SelectedValue);
        if (this.ProviderSegments != null)
        {
            int segCount = this.ProviderSegments.Count;
            if (segCount > 1)
            {
                pageSize = pageSize * (segCount + 1);
            }
        }
        this.GrdDynamic.PageSize = pageSize;
        _reports = GetAnalyticsReport(_providerName);
        bindData();
    }

    public void RegisterScripts() {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
		Ektron.Cms.API.Css.RegisterCss(this, CommonApi.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
	}

	// sequential multihue from http://colorbrewer2.org/
	//private string[] _colors = new string[] { "E31A1C", "FD8D3C", "FEB24C", "FED976", "CC4C02", "EC7014", "FE9929", "FEC44F" };

	private string[] _colors = new string[] { "E58F3B", "EAB733", "6DAD46", "6BABE9", "A16BAD", "B0B0B0", "656565" };
	protected virtual string GetReportItemColor(int index)
	{
		if (0 == index) return "EA5F3F";
		int i = (index - 1) % _colors.Length;
		return _colors[i];
	}

	//private Random _random = null;

	//private string RandomHexColor()
	//{
	//    if (null == _random)
	//    {
	//        _random = new Random(System.DateTime.Now.Millisecond);
	//    }
	//    string hex = Convert.ToString(_random.Next(0, 255), 16).ToUpper();
	//    if (hex.Length < 2) hex = "0" + hex;
	//    return hex;
	//}

    private List<AnalyticsReportItem> CreateSegmentReportList()
    {
        List<AnalyticsReportItem> segReport = new List<AnalyticsReportItem>();
        string[] segmentName = new string[4];
        string origItemName = string.Empty;
        for (int k = 0; k < _reports[0].ReportItems.Count; k++)
        {
            origItemName = _reports[0].ReportItems[k].Name;
            for (int j = 0; j < _reports.Count; j++)
            {
                segmentName[j] = _reports[j].Segment.Name;
                AnalyticsReportItem items = new AnalyticsReportItem();
                if (0 == j)
                {
                    items.Name = origItemName;
                    segReport.Add(items);//add the title row
                    items = new AnalyticsReportItem();
                    items = _reports[j].ReportItems[k];
                    items.Name = segmentName[j];
                    segReport.Add(items);//add the first (default) report
                }
                else
                {
                    for (int l = 0; l < _reports[j].ReportItems.Count; l++)
                    {
                        if (origItemName == _reports[j].ReportItems[l].Name)
                        {
                            items = _reports[j].ReportItems[l];
                            break;
                        }
                    }

                    items.Name = segmentName[j];
                    segReport.Add(items);
                }
            }
        }

        _segReport = segReport;
        return _segReport;
    }
}


