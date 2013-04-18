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

public partial class Workarea_Widgets_TopProducts : WorkareaWidgetBaseControl, IWidget
{
    #region Private Members

    OrderApi _orderApi = new OrderApi();
    List<OrderTopProductData> _data;
    int _qty = 5;
    int _Nindex = 0;
    //private string _periodFilter = "";

    #endregion

    #region Properties

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
       // periodMenu_BE69488E4E7C45AC87959B3746FEDF2B.Attributes.Add("class", "periodMenu periodMenu" + this.ClientID);
        Util_RegisterResources();

        if (!IsCommerceAdmin)
        {
            SetTitle(GetMessage("err not role commerce-admin"));
            return;
        }

        if (IsPostBack)
        {
            int tempInt;
            if (int.TryParse(quantityTexBox.Value, out tempInt) && tempInt >= 0)
                _qty = tempInt;
        }
        else
        { quantityTexBox.Value = _qty.ToString(); }

        Util_BindData();
        litUpdateQuantity.Text = @" <input title=""Update Quantity"" type=""button"" id=""updateQuantityButton"" class='updateQuantityEDBD213693254B16948C7E2DA314B8BC' value='" + GetMessage("btn update") + @"' onclick='var clientId = ""_7021A0C6CFAC4BCFA7248E92DF1B12E6""; var quantity = $ektron(""div#ChangeQuantityBlock"" + clientId + "" input.quantityTexBox"")[0].value; if( isNaN(quantity) || quantity == """" || quantity < 0) { alert(""" + GetMessage("js alert valid numeric") + @"""); if(window.event === undefined) { $ektron(""div#ChangeQuantityBlock"" + clientId + "" input.quantityTexBox"")[0].value = """"; } else { window.event.returnValue = null; } $ektron(""div#ChangeQuantityBlock"" + clientId + "" input.quantityTexBox"").focus(); return false; } else { $ektron(""#ChangeQuantityBlock"" + clientId).hide(); __doPostBack("""", """"); }' />";

        ltrlNoRecords.Text = GetMessage("lbl no records");
        //ltrlThisID1.Text = this.ClientID;
        //ltrlThisID2.Text = this.ClientID;
        //ltrlThisID3.Text = this.ClientID;
        //ltrlThisID4.Text = this.ClientID;
        //ltrlThisID5.Text = this.ClientID;
        //ltrlThisID6.Text = this.ClientID;
        //ltrlThisID7.Text = this.ClientID;
        //ltrlThisID8.Text = this.ClientID;
        //ltrlThisID9.Text = this.ClientID;
        //ltrlThisID10.Text = this.ClientID;
        //ltrlThisID11.Text = this.ClientID;
        //ltrlThisID12.Text = this.ClientID;
        //ltrlHdnFilterID.Text = hdn_filter.ClientID;
        //ltrlToday.Text = GetMessage("today");
        //ltrlYesterday.Text = GetMessage("lbl yesterday");
        //ltrlThisWeek.Text = GetMessage("lbl this week");
        //ltrlLastWeek.Text = GetMessage("lbl last seven days");
        //ltrlThisMonth.Text = GetMessage("lbl this month");
        //ltrlLastMonth.Text = GetMessage("lbl last thirty days");
        //ltrlThisYear.Text = GetMessage("lbl this year");
        ltrlMaxNum.Text = GetMessage("lbl max num");
        //ltrlUpdateAlt.Text = GetMessage("btn update");
    }

    private void Util_LoadData()
    {
        grdData.DataSource = _data;

        grdData.Columns[0].HeaderText = ""; // blank
        grdData.Columns[1].HeaderText = GetMessage("lbl product");
        grdData.Columns[2].HeaderText = GetMessage("lbl sold");
        grdData.Columns[3].HeaderText = GetMessage("lbl value");

        grdData.DataBind();
    }

    public void Util_BindData()
    {
        OrderReportData report = new OrderReportData();
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        orderCriteria.PagingInfo = new PagingInfo(_orderApi.RequestInformationRef.PagingSize);
        orderCriteria = Util_GetDates(orderCriteria);
        report = _orderApi.GetReport(orderCriteria, _qty, EkEnumeration.TopProductsSortType.TotalSold);
        _data = report.TopProducts;

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

    public Criteria<OrderProperty> Util_GetDates(Criteria<OrderProperty> orderCriteria)
    {
        string periodText = String.Empty;

        switch (hdn_filter.Value)
        {
            case "#yesterday":
                periodText = GetMessage("lbl yesterday");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.LessThanOrEqualTo, DateTime.Now.Date);
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)));
                break;

            case "#thisweek":
                periodText = GetMessage("lbl this week");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(DateTime.Now.DayOfWeek.GetHashCode(), 0, 0, 0)));
                break;

            case "#last7days":
                periodText = GetMessage("lbl last seven days");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
                break;

            case "#thismonth":
                periodText = GetMessage("lbl this month");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(DateTime.Now.Date.Day, 0, 0, 0)));
                break;

            case "#last30days":
                periodText = GetMessage("lbl last thirty days");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)));
                break;

            case "#thisyear":
                periodText = GetMessage("lbl this year");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Subtract(new TimeSpan(DateTime.Now.DayOfYear, 0, 0, 0)));
                break;

            default: // "#today":
                periodText = GetMessage("today");
                orderCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, DateTime.Now.Date);
                break;
        }

        SetTitle(GetMessage("lbl top products label"));
        ltr_period.Text = "<a href=\"#periodMenu" + this.ClientID + "\" class=\"commerceperiod" + this.ClientID + " commerceperiodlink\">"
            + periodText + "</a>" + "&#160;&#160;&#160;"
            + GetMessage("lbl quantity") + "&#160;"
            + "<a href=\"#\" class=\"quantitySection" + this.ClientID + " commerceactionlink\" >"
            + _qty.ToString() + "</a>";

        return orderCriteria;
    }

    public string Util_GetIndex()
    { return (++_Nindex).ToString(); }

    protected void Util_RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);

        // this has to be done as a registered JS block because it needs to run on init and also on ajax refresh
        JS.RegisterJSBlock(this,
        "$ektron('.commerceperiod" + this.ClientID + "').bind('click', function() {\n" +
        "    var trigger = $ektron(this);\n" +
        "    var targetMenu = trigger.attr('href');\n" +
        "    var triggerPosition = $ektron.positionedOffset(trigger);  //trigger.offset();\n" +
        "    var triggerHeight = trigger.outerHeight();\n" +
        "    var menuX = triggerPosition.left;\n" +
        "    var menuY = triggerPosition.top + triggerHeight;\n" +
        "    var menu = $ektron(targetMenu);\n" +
        "    menu.css('top', menuY + 'px').css('left', menuX + 'px');\n" +
        "    menu.toggle();\n" +
        "});\n" +
        "$ektron('#periodMenu" + this.ClientID + " li a').bind('click', function() {\n" +
        "    var trigger = $ektron(this);\n" +
        "    var action = trigger.attr('href');\n" +
        "    $ektron('#periodMenu" + this.ClientID + "').toggle();\n" +
        "    document.getElementById('" + hdn_filter.ClientID + "').value = action;\n" +
        "    __doPostBack('', '');\n" +
        "    return false;\n" +
        "});\n" +
        "$ektron('.quantitySection" + this.ClientID + "').bind('click', function() {\n" +
        "    var dialogObj = $ektron('#ChangeQuantityBlock_7021A0C6CFAC4BCFA7248E92DF1B12E6');\n" +
        "    dialogObj.closest('.widget').css('position','relative').css('top','0').css('left','0');\n" +
        "    dialogObj.show();\n" +
        "    return false;\n" +
        "});\n"
        , "INIT" + this.ClientID);

        //ltrNumericVal.Text = GetMessage("js alert valid numeric");

        string str_periodMenu= @"<ul id=""periodMenu{0}""  class=""periodMenu"">
    <li class=""today""><a title=""Today"" href=""#today"">
{1}</a></li>
    <li class=""yesterday""><a title=""Yesterday"" href=""#yesterday"">
{2}</a></li>
    <li class=""thisweek""><a title=""This Week"" href=""#thisweek"">
{3}</a></li>
    <li class=""last7days""><a title=""Last Week"" href=""#last7days"">
{4}</a></li>
    <li class=""thismonth""><a title=""This Month"" href=""#thismonth"">
{5}</a></li>
    <li class=""last30days""><a title=""Last Month"" href=""#last30days"">
{6}</a></li>
    <li class=""thisyear""><a title=""This Year"" href=""#thisyear"">
{7}</a></li>
</ul>";

   ltr_periodMenu.Text = string.Format( str_periodMenu,this.ClientID,GetMessage("today"), GetMessage("lbl yesterday"), GetMessage("lbl this week"),GetMessage("lbl last seven days"), GetMessage("lbl this month"), GetMessage("lbl last thirty days"), GetMessage("lbl this year"));
    }

    public string Util_FormatCurrency(object price)
    {
        return _orderApi.RequestInformationRef.CommerceSettings.CurrencyAlphaISO + EkFunctions.FormatCurrency(Convert.ToDecimal(price), _orderApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
    }

    public string GetAppPath()
    {
        return _orderApi.RequestInformationRef.ApplicationPath;
    }

    public void HandleItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Header
            && e.Item.ItemType != ListItemType.Footer)
        {
            string produrl = ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl + "&callerpage=dashboard.aspx";
            int idindex = produrl.IndexOf("&id=") + 4;
            string productidstr = produrl.Substring(idindex, produrl.IndexOf("&", idindex) - idindex);
            long contentid = long.Parse(productidstr);
            long folderid = EkContentRef.GetFolderIdForContentId(contentid);
            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(folderid);

            // product column:
            ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl = "javascript:top.showContentInWorkarea('" + Server.UrlDecode(produrl) + "', 'Content', '" + foldercsvpath + "')";

            string reporturl = ((HyperLink)e.Item.Cells[2].Controls[0]).NavigateUrl + "&callerpage=dashboard.aspx";
            // sold column:
            ((HyperLink)e.Item.Cells[2].Controls[0]).NavigateUrl = "javascript:top.showContentInWorkarea('" + Server.UrlDecode(reporturl) + "', 'Content', '" + foldercsvpath + "')";
        }
    }

}
