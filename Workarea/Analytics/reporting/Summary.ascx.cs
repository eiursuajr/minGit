using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Workarea.Reports;

public partial class Analytics_reporting_Summary : WorkareaBaseControl
{

    private List<AnalyticsReportData> _report = new List<AnalyticsReportData>();
    public List<AnalyticsReportData> Report
    {
        get { return _report; }
        set { _report = value; }
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

	private DateTime _startDate = DateTime.MinValue;
    public DateTime StartDate
    {
        get { return _startDate; }
        set { _startDate = value; }
    }
    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate 
    {
        get { return _endDate; }
        set { _endDate = value; }
    }
    protected List<string> _segmentIds;
    public List<string> ProviderSegments
    {
        set
        {
            _segmentIds = value;
        }
    }

	private const int chartWidth = 75;
	private const int chartHeight = 18;

    public void PopulateData()
    {
		List<int>[] visitsOverTime = new List<int>[4];
        List<double>[] pagesPerVisitOverTime = new List<double>[4];
        List<int>[] pageViewsOverTime = new List<int>[4];
        List<int>[] uniqueViewsOverTime = new List<int>[4];
        List<TimeSpan>[] averageTimeOnSiteOverTime = new List<TimeSpan>[4];
        List<TimeSpan>[] averageTimeOnPageOverTime = new List<TimeSpan>[4];
        //List<float>[] percentNewVisitsOverTime = new List<float>[4];
        List<float>[] bounceRateOverTime = new List<float>[4];
        List<float>[] percentExitOverTime = new List<float>[4];

		int numDays = EndDate.Subtract(StartDate).Days + 1;
		int sampleRate = (numDays > 31 ? (numDays / 31) : 1);
		DateTime date = StartDate;
        string[] segmentName = new string[4];

        for (int j = 0; j < _report.Count; j++)
        {
            segmentName[j] = _report[j].Segment.Name;
            visitsOverTime[j] = new List<int>();
            pagesPerVisitOverTime[j] = new List<double>();
            pageViewsOverTime[j] = new List<int>();
            uniqueViewsOverTime[j] = new List<int>();
            averageTimeOnSiteOverTime[j] = new List<TimeSpan>();
            averageTimeOnPageOverTime[j] = new List<TimeSpan>();
            //percentNewVisitsOverTime[j] = new List<float>();
            bounceRateOverTime[j] = new List<float>();
            percentExitOverTime[j] = new List<float>();
            for (int i = 0; i < numDays; i += sampleRate)
            {
                if (_showVisits) visitsOverTime[j].Add(_report[j].DayVisits(date));
                if (_showPagesPerVisit) pagesPerVisitOverTime[j].Add(_report[j].DayPagesPerVisit(date));
                if (_showPageviews) pageViewsOverTime[j].Add(_report[j].DayPageViews(date));
                if (_showUniqueViews) uniqueViewsOverTime[j].Add(_report[j].DayUniqueViews(date));
                if (_showTimeOnSite) averageTimeOnSiteOverTime[j].Add(_report[j].DayAverageTimeSpanOnSite(date));
                if (_showTimeOnPage) averageTimeOnPageOverTime[j].Add(_report[j].DayAverageTimeSpanOnPage(date));
                //if (_showPercentNewVisits) percentNewVisitsOverTime[j].Add(_report[j]. TBD (date));
                if (_showBounceRate) bounceRateOverTime[j].Add(_report[j].DayBounceRate(date));
                if (_showPercentExit) percentExitOverTime[j].Add(_report[j].DayExitRate(date));

                date = date.AddDays(sampleRate);
            }
        }

		string strPerDay = GetMessage("lbl avg per day");
        string strVisits = GetMessage("lbl visits");
        string strPageVisit = GetMessage("lbl pages visit");
        string strPageViews = GetMessage("lbl pageviews");
        string strUniqueViews = GetMessage("lbl unique views");
        string strTimeOnSite = GetMessage("lbl time on site");
        string strTimeOnPage = GetMessage("lbl time on page");
        string strBounceRate = GetMessage("lbl bounce rate");
        string strPercentExit = GetMessage("lbl percent exit");
		ChartData data = null;
        
        rowVisits.Visible = false;
        rowPagesPerVisit.Visible = false;
        rowPageviews.Visible = false;
        rowUniqueViews.Visible = false;
        rowTimeOnSite.Visible = false;
        rowTimeOnPage.Visible = false;
        rowBounceRate.Visible = false;
        rowPercentExit.Visible = false;
		if (_showVisits)
		{
			data = ChartData.CreateChartData(visitsOverTime[0]);
			imgVisits.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            litVisits.Text = _report[0].TotalVisits.ToString("#,##0");
            litVisitsPerDay.Text = String.Format(strPerDay, (double)_report[0].TotalVisits / (double)numDays);
            lblVisits.Text = strVisits;
            lblVisits.ToolTip = strVisits;
			rowVisits.Visible = true;
		}
		if (_showPagesPerVisit)
		{
            data = ChartData.CreateChartData(pagesPerVisitOverTime[0]);
			imgPagesPerVisit.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            litPagesPerVisit.Text = _report[0].TotalPagesPerVisit.ToString("0.00");
            lblPagesPerVisit.Text = strPageVisit;
            lblPagesPerVisit.ToolTip = strPageVisit;
			rowPagesPerVisit.Visible = true;
		}
		if (_showPageviews)
		{
            data = ChartData.CreateChartData(pageViewsOverTime[0]);
			img_pageviews.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            ltr_pageviews.Text = _report[0].TotalPageViews.ToString("#,##0");
            ltr_pageviewsperday.Text = String.Format(strPerDay, (double)_report[0].TotalPageViews / (double)numDays);
            lblPageviews.Text = strPageViews;
            lblPageviews.ToolTip = strPageViews;
			rowPageviews.Visible = true;
		}
		if (_showUniqueViews)
		{
            data = ChartData.CreateChartData(uniqueViewsOverTime[0]);
			img_uniqueviews.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            ltr_uniqueviews.Text = _report[0].TotalUniqueViews.ToString("#,##0");
            ltr_uniqueviewsperday.Text = String.Format(strPerDay, (double)_report[0].TotalUniqueViews / (double)numDays);
            lblUniqueViews.Text = strUniqueViews;
            lblUniqueViews.ToolTip = strUniqueViews;
			rowUniqueViews.Visible = true;
		}
		if (_showTimeOnSite)
		{
            data = ChartData.CreateChartData(averageTimeOnSiteOverTime[0]);
			imgTimeOnSite.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            litTimeOnSite.Text = _report[0].TotalAverageTimeSpanOnSite.ToString();
            lblTimeOnSite.Text = strTimeOnSite;
            lblTimeOnSite.ToolTip = strTimeOnSite;
			rowTimeOnSite.Visible = true;
		}
		if (_showTimeOnPage)
		{
            data = ChartData.CreateChartData(averageTimeOnPageOverTime[0]);
			img_timeonpage.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            ltr_timeonpage.Text = _report[0].TotalAverageTimeSpanOnPage.ToString();
            lblTimeOnPage.Text = strTimeOnPage;
            lblTimeOnPage.ToolTip = strTimeOnPage;
			rowTimeOnPage.Visible = true;
		}
		//if (_showPercentNewVisits)
		//{
		//    data = ChartData.CreateChartData(percentNewVisitsOverTime);
		//    imgPercentNewVisits.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
		//    litPercentNewVisits.Text = _report.TotalPercentNewVisits.ToString("0.00%");
		//    lblPercentNewVisits.Text = GetMessage("lbl percent new visits");
		//    rowPercentNewVisits.Visible = true;
		//}
		if (_showBounceRate)
		{
            data = ChartData.CreateChartData(bounceRateOverTime[0]);
			img_bouncerate.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            ltr_bouncerate.Text = _report[0].TotalBounceRate.ToString("0.00%");
            lblBounceRate.Text = strBounceRate;
            lblBounceRate.ToolTip = strBounceRate;
			rowBounceRate.Visible = true;
		}
		if (_showPercentExit)
		{
            data = ChartData.CreateChartData(percentExitOverTime[0]);
			img_percentexit.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
            ltr_percentexit.Text = _report[0].TotalExitRate.ToString("0.00%");
            lblPercentExit.Text = strPercentExit;
            lblPercentExit.ToolTip = strPercentExit;
			rowPercentExit.Visible = true;
		}
        if (this._segmentIds.Count > 1)
        {
            this.rowVisits2.Visible = false;
            this.rowVisits3.Visible = false;
            this.rowVisits4.Visible = false;
            if (_showVisits)
            {
                this.segmentcolumnVisits.InnerText = segmentName[0];
                this.segmentcolumnVisits.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnVisits2.InnerText = segmentName[1];
                    this.segmentcolumnVisits2.Visible = true;
                    litVisits2.Text = _report[1].TotalVisits.ToString("#,##0");
                    litVisitsPerDay2.Text = String.Format(strPerDay, (double)_report[1].TotalVisits / (double)numDays);
                    lblVisits2.Text = strVisits; 
                    lblVisits2.ToolTip = strVisits;
                    rowVisits2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnVisits3.InnerText = segmentName[2];
                    this.segmentcolumnVisits3.Visible = true;
                    litVisits3.Text = _report[2].TotalVisits.ToString("#,##0");
                    litVisitsPerDay3.Text = String.Format(strPerDay, (double)_report[2].TotalVisits / (double)numDays);
                    lblVisits3.Text = strVisits;
                    lblVisits3.ToolTip = strVisits;
                    rowVisits3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnVisits4.InnerText = segmentName[3];
                    this.segmentcolumnVisits4.Visible = true;
                    litVisits4.Text = _report[3].TotalVisits.ToString("#,##0");
                    litVisitsPerDay4.Text = String.Format(strPerDay, (double)_report[3].TotalVisits / (double)numDays);
                    lblVisits4.Text = strVisits;
                    lblVisits4.ToolTip = strVisits;
                    rowVisits4.Visible = true;
                }
            }
            this.rowPagesPerVisit2.Visible = false;
            this.rowPagesPerVisit3.Visible = false;
            this.rowPagesPerVisit4.Visible = false;
            if (_showPagesPerVisit)
            {
                this.segmentcolumnPPV.InnerText = segmentName[0];
                this.segmentcolumnPPV.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnPPV2.InnerText = segmentName[1];
                    this.segmentcolumnPPV2.Visible = true;
                    litPagesPerVisit2.Text = _report[1].TotalPagesPerVisit.ToString("0.00");
                    lblPagesPerVisit2.Text = strPageVisit;
                    lblPagesPerVisit2.ToolTip = strPageVisit;
                    rowPagesPerVisit2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnPPV3.InnerText = segmentName[2];
                    this.segmentcolumnPPV3.Visible = true;
                    litPagesPerVisit3.Text = _report[2].TotalPagesPerVisit.ToString("0.00");
                    lblPagesPerVisit3.Text = strPageVisit;
                    lblPagesPerVisit3.ToolTip = strPageVisit;
                    rowPagesPerVisit3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnPPV4.InnerText = segmentName[3];
                    this.segmentcolumnPPV4.Visible = true;
                    litPagesPerVisit4.Text = _report[3].TotalPagesPerVisit.ToString("0.00");
                    lblPagesPerVisit4.Text = strPageVisit;
                    lblPagesPerVisit4.ToolTip = strPageVisit;
                    rowPagesPerVisit4.Visible = true;
                }
            }
            this.rowPageviews2.Visible = false;
            this.rowPageviews3.Visible = false;
            this.rowPageviews4.Visible = false;
            if (_showPageviews)
            {
                this.segmentcolumnPV.InnerText = segmentName[0];
                this.segmentcolumnPV.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnPV2.InnerText = segmentName[1];
                    this.segmentcolumnPV2.Visible = true;
                    ltr_pageviews2.Text = _report[1].TotalPageViews.ToString("#,##0");
                    ltr_pageviewsperday2.Text = String.Format(strPerDay, (double)_report[1].TotalPageViews / (double)numDays);
                    lblPageviews2.Text = strPageViews;
                    lblPageviews2.ToolTip = strPageViews;
                    rowPageviews2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnPV3.InnerText = segmentName[2];
                    this.segmentcolumnPV3.Visible = true;
                    ltr_pageviews3.Text = _report[2].TotalPageViews.ToString("#,##0");
                    ltr_pageviewsperday3.Text = String.Format(strPerDay, (double)_report[2].TotalPageViews / (double)numDays);
                    lblPageviews3.Text = strPageViews;
                    lblPageviews3.ToolTip = strPageViews;
                    rowPageviews3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnPV4.InnerText = segmentName[3];
                    this.segmentcolumnPV4.Visible = true;
                    ltr_pageviews4.Text = _report[3].TotalPageViews.ToString("#,##0");
                    ltr_pageviewsperday4.Text = String.Format(strPerDay, (double)_report[3].TotalPageViews / (double)numDays);
                    lblPageviews4.Text = strPageViews;
                    lblPageviews4.ToolTip = strPageViews;
                    rowPageviews4.Visible = true;
                }
            }
            this.rowUniqueViews2.Visible = false;
            this.rowUniqueViews3.Visible = false;
            this.rowUniqueViews4.Visible = false;
            if (_showUniqueViews)
            {
                this.segmentcolumnUV.InnerText = segmentName[0];
                this.segmentcolumnUV.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnUV2.InnerText = segmentName[1];
                    this.segmentcolumnUV2.Visible = true;
                    ltr_uniqueviews2.Text = _report[1].TotalUniqueViews.ToString("#,##0");
                    ltr_uniqueviewsperday2.Text = String.Format(strPerDay, (double)_report[1].TotalUniqueViews / (double)numDays);
                    lblUniqueViews2.Text = strUniqueViews;
                    lblUniqueViews2.ToolTip = strUniqueViews;
                    rowUniqueViews2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnUV3.InnerText = segmentName[2];
                    this.segmentcolumnUV3.Visible = true;
                    ltr_uniqueviews3.Text = _report[2].TotalUniqueViews.ToString("#,##0");
                    ltr_uniqueviewsperday3.Text = String.Format(strPerDay, (double)_report[2].TotalUniqueViews / (double)numDays);
                    lblUniqueViews3.Text = strUniqueViews;
                    lblUniqueViews3.ToolTip = strUniqueViews;
                    rowUniqueViews3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnUV4.InnerText = segmentName[3];
                    this.segmentcolumnUV4.Visible = true;
                    ltr_uniqueviews4.Text = _report[3].TotalUniqueViews.ToString("#,##0");
                    ltr_uniqueviewsperday4.Text = String.Format(strPerDay, (double)_report[3].TotalUniqueViews / (double)numDays);
                    lblUniqueViews4.Text = strUniqueViews;
                    lblUniqueViews4.ToolTip = strUniqueViews;
                    rowUniqueViews4.Visible = true;
                }
            }
            this.rowTimeOnSite2.Visible = false;
            this.rowTimeOnSite3.Visible = false;
            this.rowTimeOnSite4.Visible = false;
            if (_showTimeOnSite)
            {
                this.segmentcolumnTOS.InnerText = segmentName[0];
                this.segmentcolumnTOS.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnTOS2.InnerText = segmentName[1];
                    this.segmentcolumnTOS2.Visible = true;
                    litTimeOnSite2.Text = _report[1].TotalAverageTimeSpanOnSite.ToString();
                    lblTimeOnSite2.Text = strTimeOnSite;
                    lblTimeOnSite2.ToolTip = strTimeOnSite;
                    rowTimeOnSite2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnTOS3.InnerText = segmentName[2];
                    this.segmentcolumnTOS3.Visible = true;
                    litTimeOnSite3.Text = _report[2].TotalAverageTimeSpanOnSite.ToString();
                    lblTimeOnSite3.Text = strTimeOnSite;
                    lblTimeOnSite3.ToolTip = strTimeOnSite;
                    rowTimeOnSite3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnTOS4.InnerText = segmentName[3];
                    this.segmentcolumnTOS4.Visible = true;
                    litTimeOnSite4.Text = _report[3].TotalAverageTimeSpanOnSite.ToString();
                    lblTimeOnSite4.Text = strTimeOnSite;
                    lblTimeOnSite4.ToolTip = strTimeOnSite;
                    rowTimeOnSite4.Visible = true;
                }
            }
            this.rowTimeOnPage2.Visible = false;
            this.rowTimeOnPage3.Visible = false;
            this.rowTimeOnPage4.Visible = false;
            if (_showTimeOnPage)
            {
                this.segmentcolumnTOP.InnerText = segmentName[0];
                this.segmentcolumnTOP.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnTOP2.InnerText = segmentName[1];
                    this.segmentcolumnTOP2.Visible = true;
                    ltr_timeonpage2.Text = _report[1].TotalAverageTimeSpanOnPage.ToString();
                    lblTimeOnPage2.Text = strTimeOnPage;
                    lblTimeOnPage2.ToolTip = strTimeOnPage;
                    rowTimeOnPage2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnTOP3.InnerText = segmentName[2];
                    this.segmentcolumnTOP3.Visible = true;
                    ltr_timeonpage3.Text = _report[2].TotalAverageTimeSpanOnPage.ToString();
                    lblTimeOnPage3.Text = strTimeOnPage;
                    lblTimeOnPage3.ToolTip = strTimeOnPage;
                    rowTimeOnPage3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnTOP4.InnerText = segmentName[3];
                    this.segmentcolumnTOP4.Visible = true;
                    ltr_timeonpage4.Text = _report[3].TotalAverageTimeSpanOnPage.ToString();
                    lblTimeOnPage4.Text = strTimeOnPage;
                    lblTimeOnPage4.ToolTip = strTimeOnPage;
                    rowTimeOnPage4.Visible = true;
                }
            }
            this.rowBounceRate2.Visible = false;
            this.rowBounceRate3.Visible = false;
            this.rowBounceRate4.Visible = false;
            if (_showBounceRate)
            {
                this.segmentcolumnBR.InnerText = segmentName[0];
                this.segmentcolumnBR.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnBR2.InnerText = segmentName[1];
                    this.segmentcolumnBR2.Visible = true;
                    ltr_bouncerate2.Text = _report[1].TotalBounceRate.ToString("0.00%");
                    lblBounceRate2.Text = strBounceRate;
                    lblBounceRate2.ToolTip = strBounceRate;
                    rowBounceRate2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnBR3.InnerText = segmentName[2];
                    this.segmentcolumnBR3.Visible = true;
                    ltr_bouncerate3.Text = _report[2].TotalBounceRate.ToString("0.00%");
                    lblBounceRate3.Text = strBounceRate;
                    lblBounceRate3.ToolTip = strBounceRate;
                    rowBounceRate3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnBR4.InnerText = segmentName[3];
                    this.segmentcolumnBR4.Visible = true;
                    ltr_bouncerate4.Text = _report[3].TotalBounceRate.ToString("0.00%");
                    lblBounceRate4.Text = strBounceRate;
                    lblBounceRate4.ToolTip = strBounceRate;
                    rowBounceRate4.Visible = true;
                }
            }
            this.rowPercentExit2.Visible = false;
            this.rowPercentExit3.Visible = false;
            this.rowPercentExit4.Visible = false;
            if (_showPercentExit)
            {
                this.segmentcolumnPE.InnerText = segmentName[0];
                this.segmentcolumnPE.Visible = true;
                if (!string.IsNullOrEmpty(segmentName[1]))
                {
                    this.segmentcolumnPE2.InnerText = segmentName[1];
                    this.segmentcolumnPE2.Visible = true;
                    ltr_percentexit2.Text = _report[1].TotalExitRate.ToString("0.00%");
                    lblPercentExit2.Text = strPercentExit;
                    lblPercentExit2.ToolTip = strPercentExit;
                    rowPercentExit2.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[2]))
                {
                    this.segmentcolumnPE3.InnerText = segmentName[2];
                    this.segmentcolumnPE3.Visible = true;
                    ltr_percentexit3.Text = _report[2].TotalExitRate.ToString("0.00%");
                    lblPercentExit3.Text = strPercentExit;
                    lblPercentExit3.ToolTip = strPercentExit;
                    rowPercentExit3.Visible = true;
                }
                if (!string.IsNullOrEmpty(segmentName[3]))
                {
                    this.segmentcolumnPE4.InnerText = segmentName[3];
                    this.segmentcolumnPE4.Visible = true;
                    ltr_percentexit4.Text = _report[3].TotalExitRate.ToString("0.00%");
                    lblPercentExit4.Text = strPercentExit;
                    lblPercentExit4.ToolTip = strPercentExit;
                    rowPercentExit4.Visible = true;
                }
            }
        }
	}

    private string GoogleChartUrl(int width, int height, string simpleEncodedDataSet)
    {
		if (String.IsNullOrEmpty(simpleEncodedDataSet)) simpleEncodedDataSet = "AA"; // flatline zero
        return this.GoogleChartBaseUrl + "?cht=ls&chs=" + width + "x" + height + "&chm=B,e6f2fa,0,0,0&chco=0077cc&chd=s:" + simpleEncodedDataSet;
    }
}
