using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;
using Ektron.Cms.Analytics;
using Microsoft.Security.Application;

public partial class Analytics_reporting_Overview : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    private string _providerName = string.Empty;
    private string _providerType = string.Empty;
    private string _segmentPersistenceId = string.Empty;
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
    protected void Page_Load(object sender, EventArgs e)
    {
		Utilities.ValidateUserLogin();
        RegisterResource();
        if (!String.IsNullOrEmpty(Request.QueryString["provider"]))
        {
            IAnalytics dataManager = ObjectFactory.GetAnalytics();
            _providerName = AntiXss.HtmlEncode(Request.QueryString["provider"]);
            _providerType = dataManager.GetProviderType(_providerName);
            _segmentPersistenceId = dataManager.GetSegmentFilterCookieName(_providerName);
        }
        switch (_providerType)
        {
            case "Ektron.Cms.Analytics.Providers.SiteCatalystProvider":
                AnalyticsReport1.Report = Ektron.Cms.Analytics.Reporting.ReportType.Pageviews;
                AnalyticsReport2.Report = Ektron.Cms.Analytics.Reporting.ReportType.SiteSection;
                break;
            case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                //AnalyticsReport1.Report = Ektron.Cms.Analytics.Reporting.ReportType.wtPages;
                //AnalyticsReport2.Report = Ektron.Cms.Analytics.Reporting.ReportType.ReferringSite;
                break;
            case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
            default:
                _providerType = "google";
                AnalyticsReport1.Report = Ektron.Cms.Analytics.Reporting.ReportType.Direct;
                AnalyticsReport2.Report = Ektron.Cms.Analytics.Reporting.ReportType.TopContent;
                break;
        }
        AnalyticsToolbar.ProviderName = _providerName;
        litTitle.Text = GetMessage("sam overview");

        AnalyticsReport1.Visible = true;
        AnalyticsReport1.ProviderName = _providerName;
		AnalyticsReport1.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport1.EndDate = AnalyticsToolbar.EndDate;
        AnalyticsReport1.ProviderSegments = this.CookieSegments;

        AnalyticsReport2.Visible = true;
        AnalyticsReport2.ProviderName = _providerName;
		AnalyticsReport2.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport2.EndDate = AnalyticsToolbar.EndDate;
        AnalyticsReport2.ProviderSegments = this.CookieSegments;

        Page.Validate();
        if (!Page.IsValid || true == ErrorPanel.Visible)
        {
            AnalyticsReport1.Visible = false;
            AnalyticsReport2.Visible = false;
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

}
