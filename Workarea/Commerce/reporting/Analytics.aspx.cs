using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms.Workarea;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Reporting;

public partial class Commerce_Reporting_Analytics : workareabase
{
    protected string m_sPageName = "Analytics.aspx";
    protected long m_FolderId = 0;
    protected List<string> SiteList = new List<string>();
    protected CurrencyData defaultCurrency = null;
    protected OrderApi orderApi;
    protected Customer CustomerManager = null;
    private Ektron.Cms.Commerce.Reporting.OrderReportData reportData = new Ektron.Cms.Commerce.Reporting.OrderReportData();
    public DateTime StartDate { get; set; }
    public int Intervals { get; set; }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            orderApi = new OrderApi();
            Util_RegisterResources();
            defaultCurrency = (new CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
            System.Web.HttpCookie siteCookie = CommonApi.GetEcmCookie();
            if (siteCookie["SiteCurrency"] != defaultCurrency.Id.ToString())
            {
                defaultCurrency.Id = Convert.ToInt32(siteCookie["SiteCurrency"]);
                CurrencyApi m_refCurrencyApi = new CurrencyApi();
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id);
            }
            m_refMsg = m_refContentApi.EkMsgRef;
            CustomerManager = new Customer(m_refContentApi.RequestInformationRef);
            Util_CheckAccess();
            if (!string.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                m_FolderId = Convert.ToInt64(Request.QueryString["folder"]);
            }
            
            Display_Commerce();
            Util_SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }
    #region Display

    protected void Display_Commerce()
    {

        Ektron.Cms.Commerce.OrderApi orderApi = new Ektron.Cms.Commerce.OrderApi();
        Ektron.Cms.Common.Criteria<OrderProperty> reportCriteria = new Ektron.Cms.Common.Criteria<OrderProperty>();

        reportCriteria.AddFilter(OrderProperty.ProductId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_iID);

        reportData = orderApi.GetReport(reportCriteria);

        if (reportData.Orders.Count > 0)
        {
            if (Request.Url.Scheme.ToLower() != "https")
            {

                this.Util_BindData();
                this.ltr_description.Text = "<img src=\"http://chart.apis.google.com/chart?cht=p3&chd=t:" + reportData.WithCoupons + "," + reportData.WithoutCoupons + "&chs=250x75&chl=Coupon-" + reportData.WithCoupons + "|No Coupon-" + reportData.WithoutCoupons + "\"/>";
                this.ltr_description.Text += "<br/><img src=\"http://chart.apis.google.com/chart?cht=p3&chd=t:" + reportData.ByNewCustomers + "," + reportData.ByReturningCustomers + "&chs=250x75&chl=New-" + reportData.ByNewCustomers + "|Returning-" + reportData.ByReturningCustomers + "\"/>";
            }
            else
            {
                TrendTimeLineChart.Visible = false;
                ltr_description.Visible = false;
            }
            dg_cctypes.DataSource = reportData.Orders;
            dg_cctypes.DataBind();

            Util_ShowSites();
        }
        else
        {

            ltr_noOrders.Text = m_refMsg.GetMessage("lbl no orders");
            TrendTimeLineChart.Visible = false;
            ltr_description.Visible = false;

        }

    }

    #endregion

    #region Util

    
    protected void Util_SetLabels()
    {

        this.SetTitleBarToMessage("lbl catalog entry analytics");
        if (Request.QueryString["callerpage"] == "dashboard.aspx")
        {
            AddBackButton("javascript:top.switchDesktopTab()");
        }
        else
        {
            AddBackButton("javascript:history.go(-1);");
        }
        AddHelpButton("entryanalytics");

        Util_SetJs();

    }

    private void Util_SetJs()
    {

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        sbJS.Append(JSLibrary.ToggleDiv());
        sbJS.Append("</script>" + Environment.NewLine);

        ltr_js.Text += Environment.NewLine + sbJS.ToString();

    }

    protected void Util_CheckAccess()
    {

        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }

        if (!m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            throw (new Exception(GetMessage("err not role commerce-admin")));
        }

    }

    protected string Util_AddSite(string orderSite)
    {

        if (!SiteList.Contains(orderSite))
        {
            SiteList.Add(orderSite);
        }
        return orderSite;

    }
    protected void Util_ShowSites()
    {

        Literal literalReference = null;
        System.Web.UI.Control header = dg_cctypes.Controls[0].Controls[0];

        if (header.FindControl("ltr_sites") != null)
        {
            literalReference = (Literal)header.FindControl("ltr_sites");
        }

        for (int index = 0; index <= (SiteList.Count - 1); index++)
        {
            literalReference.Text += "<tr><td><input type=\"checkbox\" checked=\"checked\" id=\"chk_site_" + index.ToString() + "\" name=\"chk_site_" + index.ToString() + "\" value=\"" + EkFunctions.HtmlEncode(System.Convert.ToString(SiteList[index])) + "\" />" + SiteList[index] + "</td></tr>";
        }

    }

    public string Util_ShowStatus(EkEnumeration.OrderStatus status)
    {
        string statusText = "";
        if (status == EkEnumeration.OrderStatus.Fraud)
        {
            statusText = "<img src=\"" + AppImgPath + "alert.gif\"/><span class=\"important\">" + System.Enum.GetName(typeof(EkEnumeration.OrderStatus), status) + "</span>";
        }
        else if (status == EkEnumeration.OrderStatus.Cancelled)
        {
            statusText = "<img src=\"" + AppImgPath + "commerce/cancel.gif\"/><span class=\"important\">" + System.Enum.GetName(typeof(EkEnumeration.OrderStatus), status) + "</span>";
        }
        else
        {
            statusText = System.Enum.GetName(typeof(EkEnumeration.OrderStatus), status);
        }
        return statusText;
    }

    public string Util_ShowCustomer(CustomerData Customer)
    {
        string sRet = "";

        sRet += "<a href=\"../customers.aspx?action=view&id=" + Customer.Id + "\">" + Customer.FirstName + " " + Customer.LastName + " (" + Customer.DisplayName + ")</a>";
        sRet += (string)("<br/>Orders: " + Customer.TotalOrders);
        sRet += (string)("<br/>Value:  " + defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode));
        sRet += (string)("<br/>Avg Value:  " + defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode));

        return sRet;
    }

    public string Util_ShowCustomerType(EkEnumeration.CustomerType CustomerType)
    {
        string sRet = "";

        if (CustomerType == EkEnumeration.CustomerType.New)
        {
            sRet = "<img alt=\"" + m_refMsg.GetMessage("alt new customer") + "\" src=\"" + AppImgPath + "commerce/newcust.gif\" >" + m_refMsg.GetMessage("lbl new") + "";
        }
        else if (CustomerType == EkEnumeration.CustomerType.Returning)
        {
            sRet = "<img alt=\"" + m_refMsg.GetMessage("alt returning customer") + "\" height=\"16px\" width=\"16px\" src=\"" + AppImgPath + "commerce/cust.gif\" >" + m_refMsg.GetMessage("lbl returning") + "";
        }

        return sRet;
    }
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCss");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
    }
    #endregion

    #region Google Graph

    public void Util_BindData()
    {
        DateTime firstItemDate = DateTime.Now;
        List<List<int>> dataset = new List<List<int>>();
        List<string> labels = new List<string>();
        OrderApi orderApi = new OrderApi();
        OrderReportData report = new OrderReportData();
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        orderCriteria.PagingInfo = new PagingInfo(orderApi.RequestInformationRef.PagingSize);
        orderCriteria = Util_GetDates(orderCriteria);
        report = orderApi.GetReport(orderCriteria);
        dataset.Add(Util_GetDateValues(report));
        labels = Util_GetDateLabels(report);
        TrendTimeLineChart.LoadSplitData(StartDate, StartDate, labels.ToArray(), dataset.ToArray());
    }

    public Criteria<OrderProperty> Util_GetDates(Criteria<OrderProperty> orderCriteria)
    { 
        Intervals = 7;
        StartDate = DateTime.Now.Subtract(new TimeSpan(Intervals, 0, 0, 0));
        orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, StartDate);
        TrendTimeLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
        return orderCriteria;
    }


    public List<int> Util_GetDateValues(OrderReportData reportData)
    {
        List<int> dateValues = new List<int>();

        for (int i = Intervals; i >= 0; i--)
        {
            int periodTotal = 0;
            periodTotal = (int)reportData.Dates.DayTotal(DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)));
            dateValues.Add(periodTotal);
        }

        return dateValues;
    }
    public List<string> Util_GetDateLabels(OrderReportData reportData)
    {
        List<string> dateLabels = new List<string>();

        for (int i = 0; i <= Intervals; i++)
        {
            string currentPeriod = "";
            currentPeriod = DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).Day.ToString();
            dateLabels.Add(currentPeriod);
          
        }

        return dateLabels;
    }

    #endregion
}

