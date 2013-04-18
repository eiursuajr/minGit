using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting; 
using Ektron.Cms.Common;
using Ektron.Cms.Interfaces.Analytics.Provider;
using Ektron.Cms.Analytics.Providers;

public partial class Analytics_reporting_Trend : WorkareaBaseControl
{

    #region Private Members

    private ReportType _reportType = ReportType.Pageviews;
	private List<TrendReportData> _reports = null;
    private double maxValue = 0.0;
    private string _reportTitle = "";
    private bool _reportGenerated = false;

    private ContentAPI _refContentApi = new ContentAPI();
    private DataType dataType = DataType.Percentage;
    private EkRequestInformation _requestInfo = null;
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();
    IMetrics _metrics = null;
    private string SegmentPersistenceId = string.Empty;
    private List<string> _providerSegments;

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private enum DataType
    {
        Percentage,
        Value,
        Rate,
        Time
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

	private string _providerName = ""; 
	public string ProviderName
	{
		get { return _providerName; }
		set { _providerName = value; }
	}

	public DateTime StartDate
	{
		get { return _startDate; }
		set { _startDate = value; }
	}

	public DateTime EndDate
	{
		get { return _endDate; }
		set { _endDate = value; }
	}

	public ReportType Report
	{
		get { return _reportType; }
		set { _reportType = value; }
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
        get
        {
            return _providerSegments;
        }
        set
        {
            _providerSegments = value;
        }
    }
    public bool FromWidget { get; set; }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
	{
		ErrorPanel.Visible = false;
		if (null == _requestInfo) {
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
        if (!String.IsNullOrEmpty(_providerName) && _dataManager.HasProvider(_providerName))
        {
            SegmentPersistenceId = _dataManager.GetSegmentFilterCookieName(_providerName);
        }
		ltrlNoRecords.Text = GetMessage("lbl no records");
	}

	protected override void OnPreRender(EventArgs e)
	{
		try
		{
            if (!_reportGenerated)
            {
                _reports = GetAnalyticsReport(_providerName, "Views", SortDirection.Descending);
                if (_reports != null && _reports.Count > 0)
                {
                    if (null == this.ProviderSegments || _reports.Count <= 1)
                    {
                        grdData.DataSource = _reports[0].Items;
                    }
                    else
                    {
                        grdSegmentData.DataSource = MergeReportItems();
                    }
                }
            }
		}
		catch (TypeInitializationException ex)
		{
			string _error = ex.Message;
			lblNoRecords.Text = GetMessage("err analyticsconfig");
			lblNoRecords.Visible = true;
			EkException.LogException(ex);
			return;
		}
		catch (Exception ex)
		{
            if (ex.Message.Contains("Username and/or password not set."))
                litErrorMessage.Text = this.GetMessage("err google overview");
            else if (ex.Message.Contains("Web Services Username and/or Secret not set"))
                litErrorMessage.Text = this.GetMessage("err analytic report");
            else
                litErrorMessage.Text = ex.Message;
			ErrorPanel.Visible = true;
		}
		Util_BindData();

		base.OnPreRender(e);
	}

    private DataTable CreateSegmentDataTable()
    {
        DataTable segDataTable = new DataTable();

        DataColumn dataColumn;

        dataColumn = new DataColumn();
        dataColumn.DataType = Type.GetType("System.String");
        dataColumn.ColumnName = "Title";
        segDataTable.Columns.Add(dataColumn);

        dataColumn = new DataColumn();
        dataColumn.DataType = Type.GetType("System.String");
        dataColumn.ColumnName = "Value";
        segDataTable.Columns.Add(dataColumn);

        return segDataTable;
    } 

    private DataTable MergeReportItems()
    {
        DataTable segTable = CreateSegmentDataTable();
        DataRow row;
        for (int i = 0; i < _reports[0].Items.Count; i++)
        {
            row = segTable.NewRow();
            row["Title"] = String.Format("{0:D}", _reports[0].Items[i].Date);
            row["Value"] = "";
            segTable.Rows.Add(row);
            for (int j = 0; j < _reports.Count; j++)
            {
                row = segTable.NewRow();
                row["Title"] = _reports[j].Segment.Name;
                row["Value"] = _reports[j].Items[i].Value;
                segTable.Rows.Add(row);
            }
        }
        return segTable;
    }

    private bool IsMetricSupported(Metric m)
    {
        return (m != null);
    }

    private List<TrendReportData> GetAnalyticsReport(string provider, string sortExpression, SortDirection sortDirection)
    {
        _metrics = _dataManager.GetMetrics(provider);
        
        List<TrendReportData> reports = new List<TrendReportData>();
        TrendReportData report = null;
		DateTime startDate = _startDate;
		DateTime endDate = _endDate;
		string reportSummary = "";

		if (ErrorPanel.Visible)
		{
			this.htmReportSummary.InnerText = "";
            return null;
        }

        string cssTweak = string.Empty;
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
        litCssTweaks.Text = cssTweak;

        AnalyticsCriteria criteria = new AnalyticsCriteria();

        try
        {
            switch (_reportType)
            {

                case ReportType.Visits:
                    ReportTitle = GetMessage("lbl visits for all visitors");
                    if (IsMetricSupported(_metrics.visits))
                    {
                        //report = _dataManager.GetVisitsTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        reportSummary = String.Format(GetMessage("lbl visits or visits per day"), reports[0].Total, EkFunctions.GetRatio(reports[0].Total, reports[0].Items.Count));
                    }
                    break;

                case ReportType.AbsoluteUniqueVisitors:
                    ReportTitle = GetMessage("sam absolute unique visitors");
                    if (IsMetricSupported(_metrics.visitors))
                    {
                        //report = _dataManager.GetAbsoluteUniqueVisitorsTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        reportSummary = String.Format(GetMessage("lbl absolute unique visitors report"), reports[0].Total, 0);
                    }
                    break;

                case ReportType.Pageviews:
                    ReportTitle = GetMessage("lbl pageviews for all visitors");
                    if (IsMetricSupported(_metrics.pageviews))
                    {
                        //report = _dataManager.GetPageViewsTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        reportSummary = String.Format(GetMessage("lbl pageviews trend"), reports[0].Total, 0);
                    }
                    break;

                case ReportType.AveragePageviews:
                    ReportTitle = GetMessage("lbl average pageviews for all visitors");
                    if (IsMetricSupported(_metrics.pageviews) && IsMetricSupported(_metrics.visits))
                    {
                        //report = _dataManager.GetAveragePageViewsTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        reportSummary = String.Format(GetMessage("lbl pages per visit"), reports[0].Total, 0);
                        dataType = DataType.Value;
                    }
                    break;

                case ReportType.TimeOnSite:
                    ReportTitle = GetMessage("lbl time on site for all visitors");
                    if (IsMetricSupported(_metrics.timeOnSite) && IsMetricSupported(_metrics.visits))
                    {
                        //report = _dataManager.GetTimeOnSiteTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        reportSummary = String.Format(GetMessage("lbl avg time on site"), TimeSpan.FromSeconds(Math.Round(Convert.ToDouble(reports[0].Total))), 0);
                        dataType = DataType.Time;
                    }
                    break;

                case ReportType.BounceRate:
                    ReportTitle = GetMessage("lbl bounce rate for all visitors");
                    if (IsMetricSupported(_metrics.bounces) && IsMetricSupported(_metrics.entrances))
                    {
                        //report = _dataManager.GetBounceRateTrend(provider, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        if (reports != null && reports.Count > 0)
                        {
                            reportSummary = String.Format(GetMessage("lbl bounce rate report"), reports[0].Total, 0);
                            dataType = DataType.Rate;
                        }
                    }
                    break;

                case ReportType.DailyVisitors:
                    ReportTitle = reportSummary = GetMessage("lbl daily unique visitor"); 
                    if (IsMetricSupported(_metrics.dailyVisitors))
                    {
                        //report = _dataManager.GetReportTrend(provider, _reportType, startDate, endDate, criteria);
                        reports = this.GetReportDataList(provider, _reportType, startDate, endDate, criteria);
                        dataType = DataType.Value;
                    }
                    break;
                case ReportType.pageViewsTrends:
                    ReportTitle = reportSummary = GetMessage("sam page views trends");
                    if (IsMetricSupported(_metrics.pageviews))
                    {
                        report = _dataManager.GetPageViewsTrend(provider, startDate, endDate, criteria);
                        reports.Add(report); 
                        reportSummary = String.Format(GetMessage("lbl pageviews trend"), reports[0].Total, 0);
                    }
                    break;
                case ReportType.kbytesTrend:
                    ReportTitle = reportSummary = GetMessage("sam bandwidth: kbytes transferred trend");
                    if (IsMetricSupported(_metrics.kBytes))
                    {
                        report = _dataManager.GetReportTrend(provider, _reportType, startDate, endDate, criteria);
                        reports.Add(report); 
                        dataType = DataType.Value;
                    }
                    break;
            }

            if (reports != null && reports.Count > 0)
            {
                maxValue = reports[0].MaximumValue;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); 
        }

        if (null == reports || 0 == reports.Count)
        {
            this.htmReportSummary.InnerText = String.Format(GetMessage("msg report not supported"), ReportTitle, provider); 
        }
        else
        {
            reports[0].Items.Sort(new Comparison<ReportItem>(ReportItem.CompareDate));

			this.htmReportTitle.Visible = true;
			this.htmReportTitle.InnerText = reportSummary;
            RenderDateRange();
            this.htmReportSummary.Visible = false;
			//this.htmReportSubtitle.Visible = true;
			//this.htmReportSubtitle.InnerText = reportSummary;
			//this.htmReportSummary.InnerText = reportSummary;            
        }
        _reportGenerated = true;
        return reports;
    }

    private List<TrendReportData> GetReportDataList(string provider, ReportType reportType, DateTime startDate, DateTime endDate, AnalyticsCriteria criteria)
    {
        List<TrendReportData> mergedReport = new List<TrendReportData>();
        TrendReportData oneReport = null;
        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
        {
            foreach (string segIdPair in this.ProviderSegments)
            {
                string segVal = segIdPair.Substring(0, segIdPair.IndexOf("|"));
                string sSegProp = segIdPair.Replace(segVal + "|", "");
                SegmentProperty segProp = (SegmentProperty)Convert.ToInt32(sSegProp);
                criteria.SegmentFilter = new SegmentFilter(segProp, SegmentFilterOperator.EqualTo, segVal);
                oneReport = _dataManager.GetReportTrend(provider, reportType, startDate, endDate, criteria);
                if (oneReport != null)
                {
                    mergedReport.Add(oneReport);
                }
            }
        }
        else
        {
            oneReport = _dataManager.GetReportTrend(provider, reportType, startDate, endDate, criteria);
            if (oneReport != null)
            {
                mergedReport.Add(oneReport);
            }
        }
        return mergedReport;
    }

    protected void RenderDateRange() {
        try {
            if (_refContentApi != null && _refContentApi.ContentLanguage > 0) {
                IFormatProvider format = new System.Globalization.CultureInfo(_refContentApi.ContentLanguage);
                AnalyticsReportDateRangeDisplay.InnerText = StartDate.ToString("d", format) + " - " + this.EndDate.ToString("d", format);
                return;
            }
        }
        catch { }

        AnalyticsReportDateRangeDisplay.InnerText = this.StartDate.ToShortDateString() + " - " + this.EndDate.ToShortDateString();
    }

    public void Util_BindData()
    {
        if (null == this.ProviderSegments || this.ProviderSegments.Count <= 1)
        {
            //this.grdData.Columns[0].HeaderText = "Date";
            //this.grdData.Columns[1].HeaderText = "Values";

            this.grdData.DataBind();
            this.grdData.Visible = true;
            this.grdSegmentData.Visible = false;
        }
        else
        {
            this.grdSegmentData.DataBind();
            this.grdSegmentData.Visible = true;
            this.grdData.Visible = false;
        }

        lblNoRecords.Visible = false;
        pnlData.Visible = true;
    }

	public string Util_ShowValue(object value, object itemIndex)
    {

        string valueDisplay = "";
        if (!string.IsNullOrEmpty(value.ToString()))
        {
            int index = Convert.ToInt32(itemIndex);
            int reportNum = 0;
            int segNum = 0;
            int rptItemIdx = index;
            string segmentCss = " segment0";
            if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
            {
                segNum = this.ProviderSegments.Count + 1;
                reportNum = (index % segNum) - 1; // reports is 0-based.
                rptItemIdx = Math.Abs(index / segNum);
                segmentCss = " segment" + (index % segNum);
            }
            //valueDisplay = "<div style=\"display:inline;float:left;height:1.1em;background:#1A87D5;width:" + getWidthPercent(Convert.ToDouble(value)) + ";\">&#160;</div>&#160;";
            valueDisplay = "<div class=\"bar" + segmentCss + "\" style=\"width:" + getWidthPercent(Convert.ToDouble(value)) + ";\">&#160;</div>";
            switch (dataType)
            {
                case DataType.Rate:
                    valueDisplay += String.Format("{0:0.00%}", Convert.ToSingle(value));
                    break;
                case DataType.Time:
                    valueDisplay += TimeSpan.FromSeconds(Math.Round(Convert.ToDouble(value))).ToString();
                    break;
                case DataType.Value:
                    valueDisplay += String.Format("{0:0.00}", Convert.ToDouble(value));
                    break;
                default:
                    valueDisplay += String.Format("{0:0.00%} ({1:#,##0})", _reports[reportNum].GetItemPercent(rptItemIdx), Convert.ToInt32(value));
                    break;
            }
        }

        return valueDisplay;

    }

	private string getWidthPercent(double value)
    {
		if (0.0 == maxValue) return "0%";
        return ((value / maxValue) * 0.75).ToString("0%");

    }

    protected void Util_RegisterResources()
	{ RegisterScripts(); }


    public void RegisterScripts() {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
		Ektron.Cms.API.Css.RegisterCss(this, CommonApi.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
	}
}


