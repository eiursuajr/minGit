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

public partial class Workarea_Widgets_CustomerList : WorkareaWidgetBaseControl, IWidget
{

    #region Private Members


    List<CustomerData> _data;
    // int N = 5;
    int Nindex = 0;
    // private string periodFilter = "";
    CustomerApi customerApi = new CustomerApi();
    CurrencyApi currencyApi = new CurrencyApi();
    CurrencyData defaultCurrency = null;


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
        //litLabelMostRecentOrders.Text = GetMessage("lbl report most recent orders");
        //litLabelMostValuable.Text = GetMessage("lbl most valuable");
        //litLabelMostActive.Text = GetMessage("lbl most active");

        if (!IsCommerceAdmin)
        {
            SetTitle(GetMessage("err not role commerce-admin"));
            return;
        }

        //periodMenu0AC4DD881609474B8F3D10B1595C5B6D.Attributes.Add("class", "periodMenu periodMenu" + this.ClientID);
        Util_RegisterResources();
        Util_BindData();

        ltrlNoRecords.Text = GetMessage("lbl no records");
    }

    private void Util_LoadData()
    {

        grdData.DataSource = _data;
        grdData.DataBind();

    }

    public void Util_BindData()
    {

        Criteria<CustomerProperty> customerCriteria = new Criteria<CustomerProperty>();


        customerCriteria = Util_GetCriteria(customerCriteria);

        _data = customerApi.GetList(customerCriteria);

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

    public Criteria<CustomerProperty> Util_GetCriteria(Criteria<CustomerProperty> customerCriteria)
    {
        customerCriteria.PagingInfo = new PagingInfo(5);

        string periodstring = "";
        SetTitle(GetMessage("lbl customers label"));
        switch (hdn_filter.Value)
        {
            case "#mostvaluable":
                customerCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;
                customerCriteria.OrderByField = CustomerProperty.TotalOrderValue;
                //SetTitle("Most<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">Valuable</a> Customers");
                periodstring = " <a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl most valuable") + "</a>";
                break;

            case "#mostactive":
                customerCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;
                customerCriteria.OrderByField = CustomerProperty.TotalOrders;
                //SetTitle("Most<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">Active</a>Customers");
                periodstring = " <a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl most active") + "</a>";
                break;

            default: // "#mostrecent" :
                customerCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;
                customerCriteria.OrderByField = CustomerProperty.DateCreated;
                //SetTitle("Most<a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">Recent</a> Customers");
                periodstring = " <a href=\"#periodMenu" + ClientID + "\" class=\"commerceperiod" + ClientID + " commerceperiodlink\">" + GetMessage("lbl report most recent orders") + "</a>";
                break;
        }

        ltr_period.Text = periodstring;
        return customerCriteria;
    }

    public string Util_GetIndex()
    {

        Nindex = (Nindex + 1);
        return Nindex.ToString();

    }

    protected void Util_RegisterResources()
    {

        // register JS

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
        "});\n"
        , "INIT" + this.ClientID);

        // register CSS
        string str_periodMenu = @"<ul id=""periodMenu{0}"" class=""periodMenu"">
        <li><a title=""Recent"" href=""#mostrecent"">
        {1}</a></li>
        <li><a title=""Valuable"" href=""#mostvaluable"">
            {2}</a></li>
        <li><a title=""Active"" href=""#mostactive"">
            {3}</a></li>
    </ul>";

        ltr_periodMenu.Text = string.Format(str_periodMenu,this.ClientID,GetMessage("lbl report most recent orders"),GetMessage("lbl most valuable"),GetMessage("lbl most active"));
    }

    public string Util_FormatCurrency(object price)
    {

        if (customerApi.RequestInformationRef.CommerceSettings.CurrencyId == customerApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            return customerApi.RequestInformationRef.CommerceSettings.CurrencyAlphaISO + EkFunctions.FormatCurrency(Convert.ToDecimal(price), customerApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
        else
        {

            if (defaultCurrency == null)
                defaultCurrency = currencyApi.GetItem(customerApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);

            return defaultCurrency.AlphaIsoCode + EkFunctions.FormatCurrency(Convert.ToDecimal(price), defaultCurrency.CultureCode);

        }

    }

    public string Util_FormatDate(object dateCreated)
    {

        if (Convert.ToDateTime(dateCreated) != DateTime.MinValue)
            return Convert.ToDateTime(dateCreated).ToShortDateString() + " " + Convert.ToDateTime(dateCreated).ToShortTimeString();
        else
            return "-";

    }

    public string GetAppPath()
    {
        return customerApi.RequestInformationRef.ApplicationPath;
    }

    public void HandleItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Header)
        {
            e.Item.Cells[0].Text = ""; // blank
            e.Item.Cells[1].Text = GetMessage("generic name");
            e.Item.Cells[2].Text = GetMessage("lbl orders");
            e.Item.Cells[3].Text = GetMessage("lbl order value");
            e.Item.Cells[4].Text = GetMessage("generic datecreated");
        }
        else if (e.Item.ItemType != ListItemType.Footer)
        {
            // name column:
            string url = ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl;
			if (Request.QueryString["page"] != "normal")
                url += "&page=workarea";
            ((HyperLink)e.Item.Cells[1].Controls[0]).NavigateUrl = "javascript:top.showContentInWorkarea('" + Server.UrlDecode(url) + "', 'Settings', 'commerce\\\\')";

            // orders column:
            //((HyperLink)e.Item.Cells[2].Controls[0]).NavigateUrl = customerApi.RequestInformationRef.ApplicationPath + ((HyperLink)e.Item.Cells[2].Controls[0]).NavigateUrl;
            // <asp:HyperLinkColumn DataTextField="TotalOrders" HeaderText="Orders" DataNavigateUrlField="Id" DataNavigateUrlFormatString="Commerce/fulfillment.aspx?action=vieworder&id={0}" />
        }
    }

}
