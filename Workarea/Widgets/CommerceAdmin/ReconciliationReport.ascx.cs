using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Reporting;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;

public partial class Workarea_Widgets_ReconciliationReport : WorkareaWidgetBaseControl, IWidget
{


    #region Private Members


    List<OrderPaymentData> _data;
    OrderApi orderApi = new OrderApi();
    CurrencyApi currencyApi = new CurrencyApi();
    DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    DateTime endDate = DateTime.Now.Date.AddDays(1);

    #endregion


    #region Properties


    //[GlobalWidgetData(true)]
    //public string PeriodFilter
    //{
    //    get { return periodFilter; }
    //    set { periodFilter = value; }
    //}


    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsCommerceAdmin)
        {
            SetTitle(GetMessage("err not role commerce-admin"));
            return;
        }

        Util_SetDates();
        dateMenu_68AF888415C5452D9A2D1CBC113AD156_start.Attributes.Add("class", "dateMenu dateMenu" + this.ClientID + "start");
        dateMenu_DD8615035E9E44B7BE30AF0A1E0C9AD4_end.Attributes.Add("class", "dateMenu dateMenu" + this.ClientID + "end");
        startdatepicker.Attributes.Add("class", "startdatepicker" + this.ClientID);
        enddatepicker.Attributes.Add("class", "enddatepicker" + this.ClientID);
        Util_RegisterResources();
        Util_BindData();

        ltrlNoRecords.Text = GetMessage("lbl no records");

    }

    private void Util_SetDates()
    {

        if (Request.Form[hdn_filter_start.UniqueID] == null &&
            Request.Form[hdn_filter_end.UniqueID] == null)
        {

            hdn_filter_start.Value = startDate.ToShortDateString();
            hdn_filter_end.Value = endDate.ToShortDateString();

        }
        else
        {

            startDate = DateTime.Parse(hdn_filter_start.Value, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
            endDate = DateTime.Parse(hdn_filter_end.Value, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);

        }

    }

    private void Util_LoadData()
    {
        grdData.DataSource = _data;

        grdData.Columns[0].HeaderText = GetMessage("lbl date captured");
        grdData.Columns[1].HeaderText = GetMessage("lbl cardnumber");
        grdData.Columns[2].HeaderText = GetMessage("generic type");
        grdData.Columns[3].HeaderText = GetMessage("lbl transactionid");
        grdData.Columns[4].HeaderText = GetMessage("lbl amount");
        grdData.Columns[5].HeaderText = GetMessage("lbl voided");

        grdData.DataBind();
    }

    public void Util_BindData()
    {

        Criteria<OrderPaymentProperty> paymentCriteria = new Criteria<OrderPaymentProperty>();

        paymentCriteria = Util_GetCriteria(paymentCriteria);

        _data = orderApi.GetOrderPaymentList(paymentCriteria);

        if (_data.Count > 0)
        {
            Util_LoadData();
            lblNoRecords.Visible = false;
            pnlData.Visible = true;
        }
        else
        {
            lblNoRecords.Visible = true;
            pnlData.Visible = false;
        }

    }

    public Criteria<OrderPaymentProperty> Util_GetCriteria(Criteria<OrderPaymentProperty> paymentCriteria)
    {
        // paymentCriteria.PagingInfo = new PagingInfo(5);
        paymentCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        paymentCriteria.OrderByField = OrderPaymentProperty.CapturedDate;

        paymentCriteria.AddFilter(OrderPaymentProperty.CapturedDate, CriteriaFilterOperator.GreaterThanOrEqualTo, EkFunctions.ReadDbDate(hdn_filter_start.Value));
        paymentCriteria.AddFilter(OrderPaymentProperty.CapturedDate, CriteriaFilterOperator.LessThanOrEqualTo, EkFunctions.ReadDbDate(hdn_filter_end.Value));

        SetTitle(GetMessage("lbl payments label"));
        ltr_period.Text = GetMessage("lbl coupon start date")
            + ":<a href=\"#dateMenu" + ClientID + "start\" class=\"commerceperiod" + ClientID + "start commerceperiodlink\" id=\"commerceperiod" + ClientID + "start\">"
            + startDate.ToShortDateString()
            + "</a>&#160;&#160;&#160;"
            + GetMessage("lbl end date")
            + ":<a href=\"#dateMenu" + ClientID + "end\" class=\"commerceperiod" + ClientID + "end commerceperiodlink\" id=\"commerceperiod" + ClientID + "end\">"
            + endDate.ToShortDateString()
            + "</a> - <a href=\"#doMenu" + ClientID + "\" class=\"commerceaction" + ClientID + " commerceactionlink\">"
            + GetMessage("lbl set new dates") + "</a>";

        return paymentCriteria;
    }

    protected void Util_RegisterResources()
    {

        string currentCultureName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern; //.Replace("yyyy", "yy");
        hdn_dt_format1.Value = currentCultureName;
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.Framework.UI.Packages.jQuery.jQueryUI.Datepicker.Register(this);
        string registerJSBlockStr = "$ektron('.commerceaction" + this.ClientID + "').bind('click', function() {    \n" +
          "     __doPostBack('', '');\n" +
          "    return false;\n" +
          "});    \n" +
          "  var DateTimeFormat = '[currentCultureFromUserControl]';" +
          " $ektron('.commerceperiod" + this.ClientID + "start').bind('click', function() {\n" +
          "    var trigger = $ektron(this);\n" +
          "    var targetMenu = trigger.attr('href');\n" +
          "    var triggerPosition = $ektron.positionedOffset(trigger);  //trigger.offset();\n" +
          "    var triggerHeight = trigger.outerHeight();\n" +
          "    var menuX = triggerPosition.left;\n" +
          "    var menuY = triggerPosition.top + triggerHeight;\n" +
          "    var menu = $ektron('.dateMenu" + this.ClientID + "start');\n" +
          "    menu.css('top', menuY + 'px').css('left', menuX + 'px');\n" +
          "    $ektron('.startdatepicker" + this.ClientID + "').datepicker({\n" +
          "        dateFormat: '[currentCultureFromUserControl]', \n" +
          "        onSelect: function(dateText, inst) {\n" +
          "           if (ValidStartDate_10084150444247DD8DEF8D17AE326E5D(dateText, '" + hdn_filter_end.ClientID + "'))\n" +
          "            { \n" +
          "                $ektron('.dateMenu" + this.ClientID + "start').toggle();\n" +
          "                $ektron('.commerceperiod" + this.ClientID + "start').text(dateText);\n" +
          "                document.getElementById('" + hdn_filter_start.ClientID + "').value = dateText;\n" +
          "            }\n" +
          "            else\n" +
          "                alert('" + GetMessage("alert msg start date") + "');\n" +
          "        }\n" +
          "    });\n" +
          "    var startdate = $ektron.datepicker.parseDate('[currentCultureFromUserControl]', $ektron('#" + hdn_filter_start.ClientID + "').val()); \n" +
          "    $ektron('.startdatepicker" + this.ClientID + "').datepicker('setDate', startdate);\n" +
          "    $ektron('.ui-datepicker').show(); menu.toggle();\n" +
          "  });\n" +
          "$ektron('.commerceperiod" + this.ClientID + "end').bind('click', function() {\n" +
          "    var trigger = $ektron(this);\n" +
          "    var targetMenu = trigger.attr('href');\n" +
          "    var triggerPosition = $ektron.positionedOffset(trigger);  //trigger.offset();\n" +
          "    var triggerHeight = trigger.outerHeight();\n" +
          "    var menuX = triggerPosition.left;\n" +
          "    var menuY = triggerPosition.top + triggerHeight;\n" +
          "    var menu = $ektron('.dateMenu" + this.ClientID + "end');\n" +
          "    menu.css('top', menuY + 'px').css('left', menuX + 'px');\n" +
          "    $ektron('.enddatepicker" + this.ClientID + "').datepicker({\n" +
          "        dateFormat: '[currentCultureFromUserControl]', \n" +
          "        onSelect: function(dateText, inst) {\n" +
          "            if (ValidEndDate_512AEFA7952244BE901A87670BB2BD6E('" + hdn_filter_start.ClientID + "',dateText))\n" +
          "            {\n" +
          "                $ektron('.dateMenu" + this.ClientID + "end').toggle();\n" +
          "                $ektron('.commerceperiod" + this.ClientID + "end').text(dateText);\n" +
          "                document.getElementById('" + hdn_filter_end.ClientID + "').value = dateText;\n" +
          "            }\n" +
          "            else\n" +
          "                alert('" + GetMessage("alert msg start date") + "');\n" +
          "        }\n" +
          "    });\n" +
          "  var endACdate = $ektron.datepicker.parseDate('[currentCultureFromUserControl]', $ektron('#" + hdn_filter_end.ClientID + "').val()); \n" +
          "    $ektron('.enddatepicker" + this.ClientID + "').datepicker('setDate', endACdate);\n" +
          "    $ektron('.ui-datepicker').show(); menu.toggle();\n" +
          "});\n";
        registerJSBlockStr = registerJSBlockStr.Replace("[currentCultureFromUserControl]", currentCultureName).ToString();
        // this has to be done as a registered JS block because it needs to run on init and also on ajax refresh

        JS.RegisterJSBlock(this, registerJSBlockStr, "INIT" + this.ClientID);

    }


    public string Util_FormatCurrency(object price, object currencyInfo)
    {

        CurrencyData currency = (CurrencyData)currencyInfo;

        if (orderApi.RequestInformationRef.CommerceSettings.CurrencyId == orderApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            return orderApi.RequestInformationRef.CommerceSettings.CurrencyAlphaISO + EkFunctions.FormatCurrency(Convert.ToDecimal(price), orderApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
        else
        {

            if (currency == null)
                currency = currencyApi.GetItem(currency.Id);

            return currency.AlphaIsoCode + EkFunctions.FormatCurrency(Convert.ToDecimal(price), currency.CultureCode);

        }

    }

    public string Util_FormatDate(object dateCreated)
    {

        return Convert.ToDateTime(dateCreated).ToShortDateString() + " " + Convert.ToDateTime(dateCreated).ToShortTimeString();

    }

    public string Util_FormatCard(object cardNumber)
    {

        if (cardNumber.ToString() != "")
            return "*" + cardNumber.ToString();
        else
            return "";

    }

    public string Util_FormatType(object paymentType)
    {

        return paymentType.ToString().Replace("Ektron.Cms.Commerce.", "");

    }

    public string Util_FormatVoided(object voided)
    {

        DateTime voidedDate = EkFunctions.ReadDbDate(voided);

        if (voidedDate != DateTime.MinValue)
            return voidedDate.ToShortDateString();
        else
            return "";

    }
}
