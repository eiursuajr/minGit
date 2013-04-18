using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.DataIO.LicenseManager;
using Ektron.Cms.Analytics.Provider;

public partial class WebTrendsReportSubtree : System.Web.UI.UserControl {
    
    public System.Web.UI.HtmlControls.HtmlControl TreeContainer 
    { get { return SiteAnalytics; } }

    public System.Web.UI.HtmlControls.HtmlControl GetTreeContainer(string targetProvider)
    {
        SiteAnalytics.Controls.Clear();
        UpdateSiteAnalyticsControl(targetProvider);
        return SiteAnalytics;
    }

    private ContentAPI _contentApi = null;
    public ContentAPI ContentApi { get { return _contentApi ?? (_contentApi = new ContentAPI()); } }
    
    private EkMessageHelper _messageHelperRef = null;
    public EkMessageHelper MessageHelper { get { return (_messageHelperRef ?? (_messageHelperRef = ContentApi.EkMsgRef)); } }

    public string GetMessage(string key) 
    { return MessageHelper.GetMessage(key); }

    protected void Page_Init(object sender, EventArgs e) 
    {
        SiteAnalytics.Controls.Clear();
    }

    private void UpdateSiteAnalyticsControl(string targetProvider)
    {
        Ektron.Cms.Analytics.IAnalytics dataManager = ObjectFactory.GetAnalytics();
        List<ReportDefinition> rptLst = new List<ReportDefinition>();
        System.Collections.Generic.List<string> analyticsProviders = new System.Collections.Generic.List<string>();
        analyticsProviders = dataManager.GetProviderList();
        foreach (string providerName in analyticsProviders)
        {
            if (targetProvider == providerName)
            {
                string providerType = dataManager.GetProviderType(providerName);
                if ((string)(providerType) == "Ektron.Cms.Analytics.Providers.WebTrendsProvider")
                {
                    rptLst = dataManager.GetReportDefinitions(providerName);
                    break;
                }
            }
        }

        string treeNodes = string.Empty;
        string exCat = string.Empty;
        System.Web.UI.HtmlControls.HtmlGenericControl newGrp = null;
        foreach (ReportDefinition r in rptLst)
        {
            if (exCat != r.Category)
            {
                if (newGrp != null)
                {
                    SiteAnalytics.Controls.Add(newGrp);
                }
                newGrp = new System.Web.UI.HtmlControls.HtmlGenericControl();
                newGrp.InnerText = r.Category;
                exCat = r.Category;
            }
            if (null == newGrp)
            {
                newGrp = new System.Web.UI.HtmlControls.HtmlGenericControl();
            }
            System.Web.UI.HtmlControls.HtmlGenericControl childCtl = new System.Web.UI.HtmlControls.HtmlGenericControl();
            childCtl.InnerText = r.Name;
            childCtl.Attributes["title"] = r.Name;
            childCtl.Attributes["href"] = @"Analytics/reporting/Reports.aspx?report=" + r.Id;
            newGrp.Controls.Add(childCtl);
        }
        if (newGrp != null)
        {
            SiteAnalytics.Controls.Add(newGrp);
        }
    }

     protected void Page_Load(object sender, EventArgs e) {
    }
}
