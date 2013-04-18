using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Analytics;
using Ektron.Cms;

public partial class Analytics_controls_SiteSelector : WorkareaBaseControl
{
	private string _cookieName = "";
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();
	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	private string _persistenceId = "SiteAnalyticsSelectedSite";
	/// <summary>
	/// Used to store and retrieve date with client cookie.
	/// Default to none which prevents persistence.
	/// </summary>
	public string PersistenceId
	{
		get { return _persistenceId; }
		set { _persistenceId = value; }
	}
    public bool FromWidget { get; set; }
    private string _providerName = "";
    /// <summary>
    /// Used to store and retrieve site name.
    /// Default to google provider as string.
    /// </summary>
    public string ProviderName
    {
        get { return _providerName; }
        set 
        { 
            _providerName = value;
            LoadSiteList(_providerName);
        }
    }
    private List<string> _sitelist = null;
    public List<string> SiteList
    {
        set
        {
            _sitelist = value;
            reLoadSiteList();
        }
    }

    private void reLoadSiteList()
    {
        if (_sitelist.Count > 0)
        {
            SiteSelectorList.DataSource = _sitelist;
            SiteSelectorList.DataBind();
        }
    }
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

        if (_dataManager.IsAnalyticsViewer())
        {
			if (_persistenceId.Length > 0)
			{
				_cookieName = CommonApi.RequestInformationRef.UserId + "_" + _persistenceId;
			}

            if (this.ProviderName != "" && (SiteSelectorList.Items == null || SiteSelectorList.Items.Count == 0))
			{
                LoadSiteList(this.ProviderName);
                //List<string> siteList = _dataManager.GetProviderSiteList(this.ProviderName);

                //SiteSelectorList.DataSource = siteList;
                //SiteSelectorList.DataBind();

				if (_persistenceId.Length > 0)
				{
					// if found the selected site cookie, set the SiteSelectorList text.           
					HttpCookie selectedSiteCookie = Request.Cookies[_cookieName];
					if (selectedSiteCookie != null)
					{
						this.SelectedText = selectedSiteCookie.Value;
					}
				}
			}
        }
        lblSiteSelector.Text = GetMessage("lbl site");
        lblSiteSelector.ToolTip = lblSiteSelector.Text;
        SiteSelectorList.ToolTip = GetMessage("lbl Drop Down Menu");
        string strSegmentFilter = GetMessage("lbl provider segment filter");
        this.SegmentPopupBtn.AlternateText = strSegmentFilter;
        this.SegmentPopupBtn.ToolTip = strSegmentFilter;

        RegisterResources();
	}

    protected override void OnLoad(EventArgs e)
    {
        UpdateSegmentBtn();
    }

	protected virtual void DropDownList_SelectionChanged(object sender, EventArgs e)
	{
		if (_persistenceId.Length > 0)
		{
			//remember the selection in cookie
			HttpCookie selectedSiteCookie = new HttpCookie(_cookieName);
			selectedSiteCookie.Value = SiteSelectorList.SelectedItem.Value.ToString();
			selectedSiteCookie.Expires = DateTime.MaxValue; // Never Expires
			Response.Cookies.Add(selectedSiteCookie);
		}
		//reload the report
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	public virtual string SelectedText
	{
		get { return (SiteSelectorList != null && SiteSelectorList.SelectedItem != null) ? SiteSelectorList.SelectedItem.Text : String.Empty; }
		set
		{
			if (SiteSelectorList != null && SiteSelectorList.Items != null && SiteSelectorList.Items.Count > 0)
			{
				if (SiteSelectorList.Items.Contains(new ListItem(value)))
				{
					SiteSelectorList.SelectedValue = value;
				}
			}
		}
	}

	public string CssClass
	{
		get { return SiteSelectorContainer.Attributes["class"]; }
		set { SiteSelectorContainer.Attributes["class"] = value; }
	}

	public bool AutoPostBack
	{
		get { return SiteSelectorList.AutoPostBack; }
		set { SiteSelectorList.AutoPostBack = value; }
	}

    public void LoadSiteList(string providerName)
    {
        if (providerName != "")
        {
            List<string> siteList = _dataManager.GetProviderSiteList(providerName);

            SiteSelectorList.DataSource = siteList;
            SiteSelectorList.DataBind();
        }

    }

    private void UpdateSegmentBtn()
    {
        this.SegmentPopupBtn.OnClientClick = "OpenSegments(this); return false;";
    }

    private void RegisterResources()
    {
        string updateScript = "location.reload();";
        if (FromWidget)
        {
            updateScript = "$ektron('#' + widgetId + '_hdnSegment').val(segmentId);";
        }
        Ektron.Cms.Framework.UI.JavaScript.RegisterJavaScriptBlock(this, "function UpdateSegments(segmentId, widgetId) { " + updateScript + " }", false);

        string segmentPopupPath = CommonApi.AppPath + "analytics/reporting/segments.aspx?identity=' + uniqueId + '";
        string functionScript = "function OpenSegments(obj) {";
        functionScript += "var segmentIds = \"\";";
        functionScript += "var uniqueId = \"\";";
        if (!string.IsNullOrEmpty(Request.QueryString["provider"]))
        {
            string provider = Ektron.Cms.API.JS.EscapeAndEncode(Request.QueryString["provider"]);
            functionScript += string.Format("var provider = '{0}';", provider);
        }
        else if (FromWidget)
        {
            functionScript += "var provider = $ektron(obj).parents('.AnalyticsReportWidget_Edit').find('.ProviderSelectorList').val();";
            functionScript += "var re = new RegExp(\"_SiteSelect\" + provider + \"$\", \"i\");";
            functionScript += "uniqueId = obj.id.replace(/_SegmentPopupBtn$/i, \"\").replace(re, \"\");";
            functionScript += "segmentIds = $ektron('#' + uniqueId + '_hdnSegment').val();";
        }
        else
        {
            functionScript += "var provider = \"\";";
        }
        functionScript += "window.open('" + segmentPopupPath + "&provider=' + provider + '&updatecookie=" + !FromWidget + "&segmentIds=' + segmentIds, null,'height=300,width=650,status=no,toolbar=no,menubar=no,location=no'); }";
        Ektron.Cms.Framework.UI.JavaScript.RegisterJavaScriptBlock(this, functionScript, false);
    }
}
