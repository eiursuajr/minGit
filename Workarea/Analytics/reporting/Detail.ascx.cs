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
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Interfaces.Analytics.Provider;

public partial class Analytics_reporting_Detail : WorkareaBaseControl
{

	#region Private Members

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private enum DisplayMetric
	{
		Visits,
		PagesPerVisit,
		Pageviews,
		UniqueViews,
		TimeOnSite,
		TimeOnPage,
		//PercentNewVisits,
		BounceRate,
		PercentExit
	}

	#endregion

	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	#region Properties
    private bool _isChanged;
    public bool IsChanged
    {
        set { _isChanged = value; }
    }
	private bool _showVisits;
	public bool ShowVisits
	{
		get { return _showVisits; }
		set { _showVisits = value; }
	}
	private bool _showPagesPerVisit;
	public bool ShowPagesPerVisit
	{
		get { return _showPagesPerVisit; }
		set { _showPagesPerVisit = value; }
	}
	private bool _showPageviews;
	public bool ShowPageviews
	{
		get { return _showPageviews; }
		set { _showPageviews = value; }
	}
	private bool _showUniqueViews;
	public bool ShowUniqueViews
	{
		get { return _showUniqueViews; }
		set { _showUniqueViews = value; }
	}
	private bool _showTimeOnSite;
	public bool ShowTimeOnSite
	{
		get { return _showTimeOnSite; }
		set { _showTimeOnSite = value; }
	}
	private bool _showTimeOnPage;
	public bool ShowTimeOnPage
	{
		get { return _showTimeOnPage; }
		set { _showTimeOnPage = value; }
	}
	//private bool _showPercentNewVisits;
	//public bool ShowPercentNewVisits
	//{
	//    get { return _showPercentNewVisits; }
	//    set { _showPercentNewVisits = value; }
	//}
	private bool _showBounceRate;
	public bool ShowBounceRate
	{
		get { return _showBounceRate; }
		set { _showBounceRate = value; }
	}
	private bool _showPercentExit;
	public bool ShowPercentExit
	{
		get { return _showPercentExit; }
		set { _showPercentExit = value; }
	}

	public bool ShowSummaryChart
	{
		get { return AnalyticsSummary.Visible; }
		set { AnalyticsSummary.Visible = value; }
	}
	public bool ShowLineChart
	{
		get { return pnlChart.Visible; }
		set { MetricSelector.Visible = pnlChart.Visible = value; }
	}
	public Unit LineChartWidth
	{
		get { return AnalyticsLineChart.Width; }
		set { AnalyticsLineChart.Width = value; }
	}
	public Unit LineChartHeight
	{
		get { return AnalyticsLineChart.Height; }
		set { AnalyticsLineChart.Height = value; }
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

    private List<string> _providerSegments;
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
	#endregion


	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			RegisterScripts();
		}

		// TODO check permissions
		//if (!IsCommerceAdmin)
		//{
		//    // TODO display message GetMessage("err not role commerce-admin");
		//    return;
		//}

        MetricSelector.MetricSelectors = Analytics_reporting_MetricSelector.SelectorCount.Dual;
        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
        {
            MetricSelector.MetricSelectors = Analytics_reporting_MetricSelector.SelectorCount.Single;
        }

		ltrlNoRecords.Text = GetMessage("lbl no records");
        VisitPageArea.Visible = false;
	}

	public void UpdatePageUrl(string url)
	{
		hyp_visitpage.Text = GetMessage("lbl visit this page");
        hyp_visitpage.ToolTip = hyp_visitpage.Text;
		//lblAnalyzing.Text = GetMessage("lbl analyzing");
		//drp_analyze.Items[0].Text = GetMessage("opt content detail");
		lblVisitThisPage.Text = GetMessage("lbl visit this page");

		hyp_visitpage.NavigateUrl = url;
		hyp_visitpage.Attributes.Add("title", url);

		img_visitpage.ImageUrl = CommonApi.RequestInformationRef.AppImgPath + "../UI/Icons/linkGo.png";
		img_visitpage.AlternateText = hyp_visitpage.Text;
		img_visitpage.Attributes.Add("title", img_visitpage.AlternateText);
		img_visitpage.Visible = true;

		VisitPageArea.Visible = true;
	}

	public void UpdateReport(List<AnalyticsReportData> reports)
	{
		ltr_viewed.Text = String.Format(GetMessage("lbl page viewed times"), reports[0].TotalPageViews);

		AnalyticsSummary.ShowVisits = _showVisits;
		AnalyticsSummary.ShowPagesPerVisit = _showPagesPerVisit;
		AnalyticsSummary.ShowPageviews = _showPageviews;
		AnalyticsSummary.ShowUniqueViews = _showUniqueViews;
		AnalyticsSummary.ShowTimeOnSite = _showTimeOnSite;
		AnalyticsSummary.ShowTimeOnPage = _showTimeOnPage;
//		AnalyticsSummary.ShowPercentNewVisits = _showPercentNewVisits;
		AnalyticsSummary.ShowBounceRate = _showBounceRate;
		AnalyticsSummary.ShowPercentExit = _showPercentExit;
        AnalyticsSummary.ProviderSegments = this.ProviderSegments;

		AnalyticsSummary.Report = reports;
		AnalyticsSummary.StartDate = _startDate;
		AnalyticsSummary.EndDate = _endDate;
		AnalyticsSummary.PopulateData();

		//lblTrafficDateRange.Text = String.Format("{0:D} - {1:D}", _startDate, _endDate);

        if (String.IsNullOrEmpty(MetricSelector.SelectedValue) || _isChanged)
        {
            List<ListItem> metricItems = new List<ListItem>();
            if (_showVisits) metricItems.Add(new ListItem(GetMessage("lbl visits"), DisplayMetric.Visits.ToString()));
            if (_showPagesPerVisit) metricItems.Add(new ListItem(GetMessage("lbl pages visit"), DisplayMetric.PagesPerVisit.ToString()));
            if (_showPageviews) metricItems.Add(new ListItem(GetMessage("lbl pageviews"), DisplayMetric.Pageviews.ToString()));
            if (_showUniqueViews) metricItems.Add(new ListItem(GetMessage("lbl unique views"), DisplayMetric.UniqueViews.ToString()));
            if (_showTimeOnSite) metricItems.Add(new ListItem(GetMessage("lbl time on site"), DisplayMetric.TimeOnSite.ToString()));
            if (_showTimeOnPage) metricItems.Add(new ListItem(GetMessage("lbl time on page"), DisplayMetric.TimeOnPage.ToString()));
            //if (_showPercentNewVisits) metricItems.Add();
            if (_showBounceRate) metricItems.Add(new ListItem(GetMessage("lbl bounce rate"), DisplayMetric.BounceRate.ToString()));
            if (_showPercentExit) metricItems.Add(new ListItem(GetMessage("lbl percent exit"), DisplayMetric.PercentExit.ToString()));
            if (metricItems.Count > 0)
            {
            MetricSelector.Items = metricItems.ToArray();
            }
            _isChanged = false;
        }

		DisplayMetric[] metric = new DisplayMetric[1];
		string strMetric = MetricSelector.SelectedValue;
		if (Enum.IsDefined(typeof(DisplayMetric), strMetric))
		{
			metric[0] = (DisplayMetric)Enum.Parse(typeof(DisplayMetric), strMetric);
		}
		else if (_showVisits)
		{
			metric[0] = DisplayMetric.Visits;
		}
		else if (_showPageviews)
		{
			metric[0] = DisplayMetric.Pageviews;
		}
		string strMetric2 = MetricSelector.SelectedValue2;
		if (!string.IsNullOrEmpty(strMetric2) && strMetric != strMetric2)
		{
			if (Enum.IsDefined(typeof(DisplayMetric), strMetric2))
			{
				Array.Resize(ref metric, 2);
				metric[1] = (DisplayMetric)Enum.Parse(typeof(DisplayMetric), strMetric2);
			}
		}

		List<int>[] visitsOverTime = new List<int> [4];
        List<double>[] pagesPerVisitOverTime = new List<double>[4];
        List<int>[] pageViewsOverTime = new List<int>[4];
        List<int>[] uniqueViewsOverTime = new List<int>[4];
        List<TimeSpan>[] averageTimeOnSiteOverTime = new List<TimeSpan>[4];
        List<TimeSpan>[] averageTimeOnPageOverTime = new List<TimeSpan>[4];
        List<float>[] bounceRateOverTime = new List<float>[4];
        List<float>[] percentExitOverTime = new List<float>[4];

		int days = (int)(_endDate - _startDate).TotalDays + 1;
        string[] segmentName = new string[4];
        for (int j = 0; j < reports.Count; j++)
        {
            segmentName[j] = reports[j].Segment.Name;
            visitsOverTime[j] = new List<int>();
            pagesPerVisitOverTime[j] = new List<double>();
            pageViewsOverTime[j] = new List<int>();
            uniqueViewsOverTime[j] = new List<int>();
            averageTimeOnSiteOverTime[j] = new List<TimeSpan>();
            averageTimeOnPageOverTime[j] = new List<TimeSpan>();
            bounceRateOverTime[j] = new List<float>();
            percentExitOverTime[j] = new List<float>();
            for (int iMetric = 0; iMetric <= metric.Length - 1; iMetric++)
            {
                DateTime date = _startDate;

                for (int i = 0; i <= days - 1; i++)
                {
                    switch (metric[iMetric])
                    {
                        case DisplayMetric.Visits:
                            visitsOverTime[j].Add(reports[j].DayVisits(date));
                            break;

                        case DisplayMetric.PagesPerVisit:
                            pagesPerVisitOverTime[j].Add(reports[j].DayPagesPerVisit(date));
                            break;

                        case DisplayMetric.Pageviews:
                            pageViewsOverTime[j].Add(reports[j].DayPageViews(date));
                            break;

                        case DisplayMetric.UniqueViews:
                            uniqueViewsOverTime[j].Add(reports[j].DayUniqueViews(date));
                            break;

                        case DisplayMetric.TimeOnSite:
                            averageTimeOnSiteOverTime[j].Add(reports[j].DayAverageTimeSpanOnSite(date));
                            break;

                        case DisplayMetric.TimeOnPage:
                            averageTimeOnPageOverTime[j].Add(reports[j].DayAverageTimeSpanOnPage(date));
                            break;

                        case DisplayMetric.BounceRate:
                            bounceRateOverTime[j].Add(reports[j].DayBounceRate(date));
                            break;

                        case DisplayMetric.PercentExit:
                            percentExitOverTime[j].Add(reports[j].DayExitRate(date));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("metric[iMetric]", "Unknown DetailMetric: " + metric[iMetric]);
                    }
                    date = date.AddDays(1);
                }
            }
        }

		AnalyticsLineChart.BriefDescription = GetMessage("lbl time line chart");
		AnalyticsLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
		switch (metric[0])
		{
			case DisplayMetric.Visits:
                AnalyticsLineChart.LoadData(_startDate, visitsOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.PagesPerVisit:
                AnalyticsLineChart.LoadData(_startDate, pagesPerVisitOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.Pageviews:
                AnalyticsLineChart.LoadData(_startDate, pageViewsOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.UniqueViews:
                AnalyticsLineChart.LoadData(_startDate, uniqueViewsOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.TimeOnSite:
                AnalyticsLineChart.LoadData(_startDate, averageTimeOnSiteOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.TimeOnPage:
                AnalyticsLineChart.LoadData(_startDate, averageTimeOnPageOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.BounceRate:
                AnalyticsLineChart.LoadData(_startDate, bounceRateOverTime[0], segmentName[0]);
				break;

			case DisplayMetric.PercentExit:
                AnalyticsLineChart.LoadData(_startDate, percentExitOverTime[0], segmentName[0]);
				break;
			default:
                throw new ArgumentOutOfRangeException("metric[0]", "Unknown DetailMetric: " + metric[0], "");
		}

        if (this.ProviderSegments != null && this.ProviderSegments.Count > 1)
        {
            for (int j = 1; j < this.ProviderSegments.Count; j++)
            {
                switch (metric[0])
                {
                    case DisplayMetric.Visits:
                        AnalyticsLineChart.LoadData2(visitsOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.PagesPerVisit:
                        AnalyticsLineChart.LoadData2(pagesPerVisitOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.Pageviews:
                        AnalyticsLineChart.LoadData2(pageViewsOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.UniqueViews:
                        AnalyticsLineChart.LoadData2(uniqueViewsOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.TimeOnSite:
                        AnalyticsLineChart.LoadData2(averageTimeOnSiteOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.TimeOnPage:
                        AnalyticsLineChart.LoadData2(averageTimeOnPageOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.BounceRate:
                        AnalyticsLineChart.LoadData2(bounceRateOverTime[j], j, segmentName[j]);
                        break;

                    case DisplayMetric.PercentExit:
                        AnalyticsLineChart.LoadData2(percentExitOverTime[j], j, segmentName[j]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("metric[0]", "Unknown DetailMetric: " + metric[0]);
                }
            }
        }
        else
        {
		    if ((metric.Length > 1))
		    {
			    switch (metric[1])
			    {
				    case DisplayMetric.Visits:
                        AnalyticsLineChart.LoadData2(visitsOverTime[0], 0, "");
					    break;

				    case DisplayMetric.PagesPerVisit:
                        AnalyticsLineChart.LoadData2(pagesPerVisitOverTime[0], 0, "");
					    break;

				    case DisplayMetric.Pageviews:
                        AnalyticsLineChart.LoadData2(pageViewsOverTime[0], 0, "");
					    break;

				    case DisplayMetric.UniqueViews:
                        AnalyticsLineChart.LoadData2(uniqueViewsOverTime[0], 0, "");
					    break;

				    case DisplayMetric.TimeOnSite:
                        AnalyticsLineChart.LoadData2(averageTimeOnSiteOverTime[0], 0, "");
					    break;

				    case DisplayMetric.TimeOnPage:
                        AnalyticsLineChart.LoadData2(averageTimeOnPageOverTime[0], 0, "");
					    break;

				    case DisplayMetric.BounceRate:
                        AnalyticsLineChart.LoadData2(bounceRateOverTime[0], 0, "");
					    break;

				    case DisplayMetric.PercentExit:
                        AnalyticsLineChart.LoadData2(percentExitOverTime[0], 0, "");
					    break;
				    default:
					    throw new ArgumentOutOfRangeException("metric[1]", "Unknown DetailMetric: " + metric[1]);
			    }
		    }
        }
	}

	protected virtual void MetricSelector_SelectionChanged(object sender, EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	public void RegisterScripts()
	{
		JS.RegisterJS(this, JS.ManagedScript.EktronJS);
	}

}


