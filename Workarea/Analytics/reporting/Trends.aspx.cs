using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Analytics;
using Ektron.Cms.Common;
using Microsoft.Security.Application;

public partial class Analytics_Reporting_Trends : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    private string SegmentPersistenceId = string.Empty;
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();
    private string providerName = string.Empty;
    protected List<string> CookieSegments
    {
        get
        {
            List<string> segmentIds = new List<string>();
            if (string.IsNullOrEmpty(SegmentPersistenceId) && !string.IsNullOrEmpty(providerName))
            {
                SegmentPersistenceId = _dataManager.GetSegmentFilterCookieName(providerName); 
            }
            HttpCookie cookie = Request.Cookies[SegmentPersistenceId];
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
            HttpCookie cookie = new HttpCookie(SegmentPersistenceId, idList);
            cookie.Expires = System.DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            Response.Cookies.Add(cookie);
        }
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
		Utilities.ValidateUserLogin();
        AnalyticsTrend.TitleChangedEventHandler += TitleChangedHandler;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResource();
        if (!String.IsNullOrEmpty(Request.QueryString["provider"]))
        {
            providerName = AntiXss.HtmlEncode(Request.QueryString["provider"]);
            AnalyticsToolbar.ProviderName = providerName;
            SegmentPersistenceId = _dataManager.GetSegmentFilterCookieName(providerName);
        }
        AnalyticsTrend.Visible = true;
		AnalyticsTrend.ProviderName = AnalyticsToolbar.ProviderName;
		AnalyticsTrend.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsTrend.EndDate = AnalyticsToolbar.EndDate;
        AnalyticsTrend.ProviderSegments = this.CookieSegments;
		string report = Request.QueryString["report"];
        if (!String.IsNullOrEmpty(report) && false == ErrorPanel.Visible)
        {
            AnalyticsTrend.Report = (Ektron.Cms.Analytics.Reporting.ReportType)Enum.Parse(typeof(Ektron.Cms.Analytics.Reporting.ReportType), report, true); // ignore case
        }
        else
        {
            AnalyticsTrend.Visible = false;
        }
        if (!IsPostBack)
        {
            litLoadingMessage.Text = GetMessage("generic loading"); // TODO should be label w/o viewstate
        }
	}

    protected void RegisterResource()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

	protected void DateFormatError(object sender, Analytics_controls_Toolbar_EventArgs e)
	{
		litErrorMessage.Text = e.Message;
		ErrorPanel.Visible = true;
	}

	protected void TitleChangedHandler(object sender, Analytics_reporting_Trend.TitleChangedEventArgs e)
	{
        litTitle.Text = Title = e.Title;
    }
}
