using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Reporting;
using Ektron.Cms.Common;
using System.Text;
using Ektron.Cms.Widget;

public partial class Workarea_Widgets_SalesTrend : WorkareaWidgetBaseControl, IWidget
{

    #region Private Members


    

    #endregion


    #region Properties


    public DateTime StartDate { get; set; }
    public int Intervals { get; set; }


    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsCommerceAdmin)
        {
            SetTitle(GetMessage("err not role commerce-admin"));
            return;
        }

        Util_RegisterResources();
        Util_BindData();

        ltrlNoRecords.Text = GetMessage("lbl no records");
    }

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

        ltr_count.Text = report.TotalOrders.ToString() + " " + GetMessage("lbl total orders with a value of") + " " + orderApi.RequestInformationRef.CommerceSettings.CurrencyAlphaISO + orderApi.RequestInformationRef.CommerceSettings.CurrencySymbol + report.Dates.OrderValue(orderApi.RequestInformationRef.CommerceSettings.CurrencyId);

        dataset.Add(Util_GetDateValues(report));
        labels = Util_GetDateLabels(report);

        TrendTimeLineChart.LoadSplitData(StartDate, StartDate, labels.ToArray(), dataset.ToArray());
    }

    public Criteria<OrderProperty> Util_GetDates(Criteria<OrderProperty> orderCriteria)
    {

        SetTitle(GetMessage("lbl sales trend label"));

        if (drp_period.Items.Count == 0)
        {
            drp_period.Items.Add(new ListItem(GetMessage("lbl sync daily"), "#daily"));
            drp_period.Items.Add(new ListItem(GetMessage("lbl sync monthly"), "#monthly"));
        }
        switch (hdn_filter.Value)
        {
            //case "#weekly":
            //    periodpulldown = "<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl sync weekly") + "</a>";
            //    orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(90, 0, 0, 0)));
            //    break;

            case "#monthly":
                drp_period.SelectedIndex = 1;
                Intervals = 6;
                StartDate = DateTime.Now.AddMonths(Intervals * -1);
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, StartDate);
                TrendTimeLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Month;
                break;

            //case "#quarterly":
            //    periodpulldown = "<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl quarterly") + "</a>";
            //    orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.AddYears(-1));
            //    break;

            //case "#yearly":
            //    periodpulldown = "<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl yearly") + "</a>";
            //    orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.AddYears(-3));
            //    break;

            default : // "#daily":
                drp_period.SelectedIndex = 0;
                Intervals = 7;
                StartDate = DateTime.Now.Subtract(new TimeSpan(Intervals, 0, 0, 0));
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, StartDate);
                TrendTimeLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
                break;
        }

        return orderCriteria;
    }

    protected void Util_RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);

        JS.RegisterJSBlock(this,
            "$ektron('.selectElementClass').change(function() {\n" +
            "    var action = $ektron('.selectElementClass :selected').attr('value');\n" +
            "    document.getElementById('" + hdn_filter.ClientID + "').value = action;\n" +
            "    __doPostBack('', '');\n" +
            "    });\n"
            , "INIT" + this.ClientID);
    }

    public List<int> Util_GetDateValues(OrderReportData reportData)
    {
        List<int> dateValues = new List<int>();
        
        for (int i = Intervals; i >= 0; i--)
        {
            int periodTotal = 0;

            if (hdn_filter.Value == "#monthly")
            {
                periodTotal = (int)reportData.Dates.MonthTotal((new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-i));
                dateValues.Add(periodTotal);
            }
            else
            {
                periodTotal = (int)reportData.Dates.DayTotal(DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)));
                dateValues.Add(periodTotal);
            }
        }

        return dateValues;
    }
    public List<string> Util_GetDateLabels(OrderReportData reportData)
    {
        List<string> dateLabels = new List<string>();

        for (int i = 0; i <= Intervals; i++)
        {
            string currentPeriod = "";

            if (hdn_filter.Value == "#monthly")
            {
                currentPeriod = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-i).ToString("MMM");
                dateLabels.Add(currentPeriod);
            }
            else
            {

                currentPeriod = DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).Day.ToString();
                dateLabels.Add(currentPeriod);
            }
        }

        return dateLabels;
    }

}
