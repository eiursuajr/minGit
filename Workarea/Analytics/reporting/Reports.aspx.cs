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

public partial class Analytics_Reporting_Reports : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
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
        AnalyticsReport.TitleChangedEventHandler += TitleChangedHandler;
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

        AnalyticsReport.Visible = true;
		AnalyticsReport.ProviderName = AnalyticsToolbar.ProviderName;
        AnalyticsReport.SiteName = AnalyticsToolbar.SiteName;
		AnalyticsReport.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport.EndDate = AnalyticsToolbar.EndDate;
        AnalyticsReport.ProviderSegments = this.CookieSegments;
		AnalyticsReport.LoadStateFromQueryString(Request.QueryString);

        Page.Validate();
        if (!Page.IsValid || true == ErrorPanel.Visible)
        {
            AnalyticsReport.Visible = false;
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

    protected void TitleChangedHandler(object sender, Analytics_reporting_Report.TitleChangedEventArgs e){
        litTitle.Text = Title = e.Title;
    }
}
