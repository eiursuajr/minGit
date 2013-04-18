using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analytics_controls_Toolbar : WorkareaBaseControl
{
	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	public delegate void DateFormatErrorHandler(object sender, Analytics_controls_Toolbar_EventArgs e);
	public event DateFormatErrorHandler DateFormatError;

	public string ProviderName
	{
        get { return _providerName; }
        set 
        { 
            _providerName = value;
            SiteSelector1.ProviderName = _providerName;
        }
	}
    public string SiteName
    {
        get { return SiteSelector1.SelectedText; }
    }
	public DateTime StartDate
	{
		get { return DateRangePicker1.StartDate; }
	}
	public DateTime EndDate
	{
		get { return DateRangePicker1.EndDate; }
	}

    private string _providerName = "";
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
        string helpScreenAlias = "";

        if(!String.IsNullOrEmpty(Request.QueryString["report"]))
        {
            helpScreenAlias = Request.QueryString["report"];
        }
        else
        {
            helpScreenAlias = "overview";           
        }

        //report=visits&provider=Omniture, Omni_visits
        if (Request.Url.AbsolutePath.ToLower().Contains("trends.aspx"))
        {
            if (Request.QueryString["report"] == "visits" && Request.QueryString["provider"] == "Omniture")
            {
                helpScreenAlias = "Omni_visits";
            }
        }
        //Webtrend
        //Overview.aspx?provider=WebTrends, wt_overview
        if (Request.Url.AbsolutePath.ToLower().Contains("overview.aspx"))
        {
            if (string.IsNullOrEmpty(Request.QueryString["report"]) && Request.QueryString["provider"] == "WebTrends")
            {
                helpScreenAlias = "wt_overview";
            }
        }
        StyleHelper m_refStyle = new StyleHelper();

		DateRangePicker1.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
		DateRangePicker1.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;

		DateRangePicker1.BadStartDateFormatMessage = GetMessage("msg bad start date format");
		DateRangePicker1.BadEndDateFormatMessage = GetMessage("msg bad end date format");

		DateRangePicker1.MaximumDate = DateTime.Today;
		DateRangePicker1.DefaultEndDate = DateTime.Today.AddDays(-1); // today is a partial day
		DateRangePicker1.DefaultStartDate = DateRangePicker1.DefaultEndDate.AddDays(-30);
		// set ID to make use of cookie persistence:
		DateRangePicker1.PersistenceId = "EktronWorkareaReportsAnalytics";
        litHelp.Text = m_refStyle.GetHelpButton(helpScreenAlias, "center");

        ////Defect:58466
        ////report=visits&provider=Omniture, Omni_visits
        //if (Request.Url.AbsolutePath.ToLower().Contains("trends.aspx"))
        //{
        //    if (Request.QueryString["report"] == "visits" && Request.QueryString["provider"] == "Omniture")
        //    {
        //        litHelp.Text = litHelp.Text.Replace("#visits", "#Omni_visits");
        //    }
        //}
        ////Webtrend
        ////Overview.aspx?provider=WebTrends, wt_overview
        //if (Request.Url.AbsolutePath.ToLower().Contains("overview.aspx"))
        //{
        //    if (string.IsNullOrEmpty(Request.QueryString["report"]) && Request.QueryString["provider"] == "WebTrends")
        //    {
        //        litHelp.Text = litHelp.Text.Replace("#overview", "#wt_overview");
        //    }
        //}
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
	}

	protected void BadDateFormatErrorHandler(string defaultMessage, string rawDate)
	{
		if (DateFormatError != null)
		{
			Analytics_controls_Toolbar_EventArgs e = new Analytics_controls_Toolbar_EventArgs(defaultMessage, rawDate);
			DateFormatError(this, e);
		}
	}

	protected void OnSelectionChanged(object sender, EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

}

public class Analytics_controls_Toolbar_EventArgs : EventArgs
{
	public string Message = "";
	public string RawDate = "";

	public Analytics_controls_Toolbar_EventArgs()
	{
	}
	public Analytics_controls_Toolbar_EventArgs(string message, string rawDate)
	{
		this.Message = message;
		this.RawDate = rawDate;
	}
}
