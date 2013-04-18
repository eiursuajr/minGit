<%@ WebHandler Language="C#" Class="ektronOverlay" %>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Microsoft.Security.Application;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Common;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;
using System.Xml;
using Ektron.Cms.Interfaces.Analytics.Provider;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Framework.Analytics.BusinessAnalytics;

public class ektronOverlay : IHttpHandler {

    private IAnalytics _analyticsManager = ObjectFactory.GetAnalytics();
    private Ektron.Cms.Analytics.ReportAnalyticsApi _analyticsApi;
    string _errorMessage = string.Empty;
    private string _providerName = string.Empty;
    private bool _providerExists = false;
    private string _startPage = string.Empty;
    private string _linkContext = string.Empty;
    private int _range = -30;
    private DateTime _startDate = DateTime.MinValue;
    private DateTime _endDate = DateTime.MinValue;
    private SiteAPI _siteApi;
    private JavaScriptSerializer _serializer;
    private const int MAX_CLICK_DATA_RECORDS = 1000;

    protected SiteAPI SiteApiRef {
        get { return (_siteApi ?? (_siteApi = new SiteAPI())); }
    }    

    protected EkMessageHelper MessageHelperRef {
        get { return SiteApiRef.EkMsgRef; }
    }
    
    protected Ektron.Cms.Analytics.ReportAnalyticsApi ReportAnalyticsApiRef {
        get { return (_analyticsApi ?? (_analyticsApi = new ReportAnalyticsApi())); }
    }

    protected JavaScriptSerializer Serializer {
        get { return (_serializer ?? (_serializer = new JavaScriptSerializer())); }
    }

    public void ProcessRequest (HttpContext context) 
    {
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
        context.Response.Cache.SetNoStore();

        if (AnalyticsSecurity.Enabled(SiteApiRef.RequestInformationRef)) {
            string overlayType = context.Request["type"] != null ? context.Request["type"] : "clicks";
            if (context.Request["range"] != null)
                _range = EkFunctions.ReadIntegerValue(context.Request["range"], _range);
            if (context.Request["provider"] != null)
                _providerName = context.Request["provider"];

            InitializeDates(context);
            _providerName = GetProviderName("", context);
            _providerExists = (!string.IsNullOrEmpty(_providerName) ? _analyticsManager.HasProvider(_providerName) : false);
            _startPage = GetStartPage(context);
            _linkContext = GetLinkContext(_startPage);

            context.Response.Write(GetOverlayJSON(overlayType, context));
        }
    }
    public bool IsReusable { get { return false; } }

    protected EktronOverlayClickMapData GetClickEventData(HttpContext context) {
        EktronOverlayClickMapData data = new EktronOverlayClickMapData();
        data.items = new List<EktronOverlayClickMapItemData>();
        Hashtable ht = null;
        try {
            IAnalyticsQueryRequest queryObject = EventReporter.CreateQueryRequest();
            queryObject.ReportName = "Click Event";
            queryObject.Xml = "<requestParameters><location>" + context.Request.UrlReferrer.PathAndQuery + "</location><languageId>-1</languageId><eventTypeId>1</eventTypeId></requestParameters>";
            queryObject.PagingInfo.RecordsPerPage = MAX_CLICK_DATA_RECORDS;
            queryObject.EventStartDate = this._startDate;
            queryObject.EventEndDate = this._endDate;

            // TODO: CHECK CACHE
            
            if (ht == null) { ht = new Hashtable(); }
            
            
            IList<IAnalyticsEventItem> reportResults = EventReporter.GetList(queryObject);
            foreach (IAnalyticsEventItem item in reportResults) {
                ProcessClickData(item.Xml, ht);
            }

            foreach (string name in ht.Keys) {
                data.items.Add(((EktronOverlayClickMapItemData)ht[name]));
            }

            // TODO: INSERT TO CACHE
            
        }
        catch (Exception ex) {
            _errorMessage = "Error: " + ex.Message;
            data.items.Clear();
        }
        return data;
    }

    protected void ProcessClickData(string xml, Hashtable ht) {
        if (string.IsNullOrEmpty(xml))
            return;

        try {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            int width = GetIntNodeValue(xmlDoc, "width");
            int scaledX = ScaledValue(GetIntNodeValue(xmlDoc, "x"), width);
            int height = GetIntNodeValue(xmlDoc, "height");
            int scaledY = ScaledValue(GetIntNodeValue(xmlDoc, "y"), height);
            int eventCount = GetIntNodeValue(xmlDoc, "event_count");

            string key = scaledX.ToString() + "_" + scaledY.ToString();
            if (!ht.Contains(key)) { ht.Add(key, new EktronOverlayClickMapItemData()); }
            var dataItem = new EktronOverlayClickMapItemData();
            ((EktronOverlayClickMapItemData)ht[key]).scaledX = scaledX;
            ((EktronOverlayClickMapItemData)ht[key]).scaledY = scaledY;
            ((EktronOverlayClickMapItemData)ht[key]).clicks += eventCount;
        }
        catch { }
    }

    protected int ScaledValue(int position, int max) {
        if (max == 0) { return 0; }
        
        int result = (100 * position) / max;
        if (result > 100) { result = 100; }
        return result;
    }

    protected string GetStringNodeValue(XmlDocument xmlDoc, string name) {
        try {
            if (xmlDoc != null) {

                XmlNodeList nodes = xmlDoc.SelectNodes("//event/" + name + "/text()");
                if (nodes != null && nodes.Count > 0) {
                    return nodes[0].Value;
                }
            }
        }
        catch { }
        return string.Empty;
    }

    protected int GetIntNodeValue(XmlDocument xmlDoc, string name) {
        try {
            string rawVal = GetStringNodeValue(xmlDoc, name);
            if (!string.IsNullOrEmpty(rawVal)) {
                int intVal = 0;
                if (int.TryParse(rawVal, out intVal)) {
                    return intVal;
                }
            }
        }
        catch { }
        return 0;
    }

    protected void InitializeDates(HttpContext context) {
        string rawDate;
        DateTime date;

        if (!string.IsNullOrEmpty(rawDate = context.Request["startdate"]) && DateTime.TryParse(rawDate, out date)) {
            _startDate = date;
        } else if ((rawDate = GetCookieKeyValue("startdate", context)) != null && DateTime.TryParse(rawDate, out date)) {
            _startDate = date;
        }

        if (!string.IsNullOrEmpty(rawDate = context.Request["enddate"]) && DateTime.TryParse(rawDate, out date)) {
            _endDate = date;
        } else if ((rawDate = GetCookieKeyValue("enddate", context)) != null && DateTime.TryParse(rawDate, out date)) {
            _endDate = date;
        }

        if (_startDate == DateTime.MinValue) {
            _startDate = DateTime.Now.AddDays(_range < 0 ? _range : -1);
        }

        if (_endDate == DateTime.MinValue) {
            _endDate = DateTime.Now;
        }

        // ADJUST END TIME TO 23:59:59.999
        _endDate = new DateTime(this._endDate.Year, this._endDate.Month, this._endDate.Day, 23, 59, 59);

        if (_startDate > _endDate) {
            _errorMessage = "Error: Start date cannot be greater than end date!";
        }
    }
    
    protected AnalyticsReportData GetClicksData(){
        AnalyticsReportData result = null;

        AnalyticsCriteria criteria = new AnalyticsCriteria();
        IAnalytics dataManager = ObjectFactory.GetAnalytics();
        IDimensions dimensions = dataManager.GetDimensions(_providerName);

        criteria.DimensionFilters.AddFilter(dimensions.previousPagePath, DimensionFilterOperator.EqualTo, _startPage);
        criteria.OrderByField = AnalyticsSortableField.PageViews;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;
        result = ReportAnalyticsApiRef.GetNextPagePaths(_providerName, _startDate, _endDate, criteria);
            
        return result;
    }

    public string GetPercent(int itemViews, int totalViews) {
        decimal percent = 0.0M;
        if (totalViews > 0) {
            percent = (Convert.ToDecimal(itemViews) / Convert.ToDecimal(totalViews)) * 100M;
        }

        if (percent >= 10.0M && percent < 100.0M)
            return percent.ToString("00");

        if (percent >= 1.0M && percent < 10.0M)
            return percent.ToString("0.0");

        if (percent < 1.0M)
            return percent.ToString("0.00");

        return "100";
    }
    
    public string GetOverlayJSON(string overlayType, HttpContext context)
    {
        EktronOverlayData overlayData = new EktronOverlayData();
        
        if (_errorMessage.Length == 0) 
        {
            bool logEx = true;
            try 
            {
                if (!_providerExists)
                {
                    logEx = false;
                    throw new Exception(
                       string.Format(new EkMessageHelper(_analyticsManager.RequestInformation).GetMessage("err analytics no provider"), context.Request.UrlReferrer.Host)
                       );
                }
                switch (overlayType) {
                    case "clicks":
                        overlayData = new EktronOverlayClickData();
                        AnalyticsReportData data = GetClicksData();
                        if (data != null && data.ReportItems != null) {
                            ((EktronOverlayClickData)overlayData).items = new List<EktronOverlayClickItemData>();
                            for (int n = 0; n < data.ReportItems.Count; n++) {
                                EktronOverlayClickItemData item = new EktronOverlayClickItemData();
                                
                                item.url = data.ReportItems[n].Name;
                                item.clicks = data.ReportItems[n].PageViews;
                                item.percent = GetPercent(data.ReportItems[n].PageViews, data.TotalPageViews);
                                ((EktronOverlayClickData)overlayData).items.Add(item);
                            }
                            overlayData.count = data.TotalPageViews;
                        }
                        break;
                    
                    case "clickmap":
                        overlayData = GetClickEventData(context);
                        // TODO: BUILD RESULT IMAGE ...
                        break;

                    case "abandon":
                        break;
                }
            }
            catch (Exception ex) {
                if (logEx)
                    EkException.LogException(ex);
                _errorMessage = context.Server.HtmlEncode(ex.Message);
            }
        }
        
        overlayData.errorMessage = _errorMessage;
        overlayData.errorFlag = (_errorMessage.Length > 0);
        overlayData.dateRangeData = GetClientDateRangeData();
        overlayData.linkContext = _linkContext;

        return SerializeOverlayData(overlayData);
    }
    
    public string SerializeOverlayData(EktronOverlayData overlayData) {
        return Serializer.Serialize(overlayData);
    }
    
    protected string GetProviderName(string defaultProvider, HttpContext context) {
        if (context != null && context.Request != null && context.Request.UrlReferrer != null && !string.IsNullOrEmpty(context.Request.UrlReferrer.AbsolutePath)) {
            List<string> providers = _analyticsManager.GetSiteProviders(context.Request.UrlReferrer.Host);
            if (providers != null && providers.Count > 0 && !string.IsNullOrEmpty(providers[0]))
                return providers[0];
        }
        return defaultProvider;
    }

    protected string GetStartPage(HttpContext context) {
        string result = string.Empty;
        
        if (context != null && context.Request != null && context.Request.UrlReferrer != null && !string.IsNullOrEmpty(context.Request.UrlReferrer.AbsolutePath))
            result = context.Request.UrlReferrer.AbsolutePath;

        if (context != null && context.Request != null && !string.IsNullOrEmpty(context.Request["startpage"]))
            result = context.Request["startpage"];
        
        if (!result.StartsWith("/")) {
            result = "/" + result;
        }

        return result;
    }

    protected string GetLinkContext(string startPage) {
        return startPage.Substring(0, startPage.LastIndexOf("/") + 1);
    }

    protected EktronOverlayDateRangeData GetClientDateRangeData() {
        EktronOverlayDateRangeData result = new EktronOverlayDateRangeData();
        result.fromText = "From";
        result.toText = "To";
        result.startDate = _startDate.ToString("MM") + "/" + _startDate.ToString("dd") + "/" + _startDate.ToString("yyyy");
        result.endDate = _endDate.ToString("MM") + "/" + _endDate.ToString("dd") + "/" + _endDate.ToString("yyyy");
        return result;
    }

    protected string GetCookieKeyValue(string keyName, HttpContext context) {
        return ((context.Request.Cookies != null && context.Request.Cookies["ektron_analytics_overlay"] != null) ? context.Request.Cookies["ektron_analytics_overlay"][keyName] : null);
    }
}

public class EktronOverlayDateRangeData {
    public string fromText { get; set; }
    public string toText { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
}

public class EktronOverlayData {
    public EktronOverlayDateRangeData dateRangeData { get; set; }
    public string linkContext { get; set; }
    public int count { get; set; }
    public string errorMessage { get; set; }
    public bool errorFlag { get; set; }
}

public class EktronOverlayClickItemData {
    public string url { get; set; }
    public int clicks { get; set; }
    public string percent { get; set; }
}

public class EktronOverlayClickData : EktronOverlayData {
    public List<EktronOverlayClickItemData> items { get; set; }
}

public class EktronOverlayClickMapItemData {
    public int scaledX { get; set; }
    public int scaledY { get; set; }
    public int clicks { get; set; }
}

public class EktronOverlayClickMapData : EktronOverlayData {
    public List<EktronOverlayClickMapItemData> items { get; set; }
}
