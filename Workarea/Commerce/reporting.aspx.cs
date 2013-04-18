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
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

public partial class Commerce_reporting : workareabase
{
    protected CustomerData cCustomer = null;
    protected OrderData order = null;
    protected Customer CustomerManager = null;
    protected CurrencyData defaultCurrency = null;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        Util_RegisterResources();
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }
        CustomerManager = new Customer(m_refContentApi.RequestInformationRef);
        switch (this.m_sPageAction)
        {
            case "mostrecent":
                Display_MostRecent();
                break;
            case "onhold":
                Display_OnHold();
                break;
            case "bydates":
                Display_ByDates();
                break;
            case "bycustomer":
                Display_ByCustomer();
                break;
            case "byproduct":
                Display_ByProduct();
                break;
            case "custom":
                Display_Custom();
                break;
        }
        defaultCurrency = (new CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
        Util_SetLabels();
    }


    #region Display
    protected void Display_OnHold()
    {
        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();

        orderList = orderApi.GetOnHoldOrderList(new PagingInfo());
        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }
    protected void Display_ViewOrder()
    {
        OrderApi orderApi = new OrderApi();
        order = orderApi.GetItem(this.m_iID);

        this.ltr_id.Text = order.Id.ToString();
        this.ltr_customer.Text = Util_ShowCustomer(order.Customer);
        this.ltr_created.Text = Util_ShowDate(order.DateCreated);
        this.ltr_completed.Text = Util_ShowDate(order.DateCompleted);
        this.ltr_required.Text = Util_ShowDate(order.DateRequired);
        this.ltr_orderstatus.Text = System.Enum.GetName(typeof(EkEnumeration.OrderStatus), order.Status);
        this.ltr_ordertotal.Text = FormatCurrency(order.OrderTotal, "");
        this.ltr_pipelinestage.Text = order.StageName;

        this.dg_orderparts.DataSource = order.Parts;
        this.dg_orderparts.DataBind();
        this.dg_orderlines.DataSource = order.Parts[0].Lines;
        this.dg_orderlines.DataBind();
    }

    protected void Display_MostRecent()
    {

        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();

        orderList = orderApi.GetList(new Criteria<OrderProperty>());

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }

    protected void Display_ByDates()
    {

        DateTime startDate = Convert.ToDateTime(Request.QueryString["startdate"]);
        DateTime endDate = Convert.ToDateTime(Request.QueryString["enddate"]);
        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();

        orderList = orderApi.GetList(startDate, endDate, new PagingInfo());

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }

    protected void Display_ByCustomer()
    {

        Response.Redirect("customers.aspx");
        //Dim aOrders() As Order = Array.CreateInstance(GetType(Order), 0)
        //aOrders = Order.GetAllOrders(1, 0, 0, 0, Me.m_refContentApi.RequestInformationRef)

        //dg_orders.DataSource = aOrders
        //dg_orders.DataBind()
    }

    protected void Display_ByProduct()
    {
        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();

        orderList = orderApi.GetList(new Criteria<OrderProperty>());

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }

    protected void Display_Custom()
    {
        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();

        orderList = orderApi.GetList(new Criteria<OrderProperty>());

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }

    #endregion

    #region Util
    protected void Util_SetLabels()
    {
        this.ltr_id_lbl.Text = this.GetMessage("lbl order id");
        this.ltr_customer_lbl.Text = this.GetMessage("lbl customer");
        this.ltr_created_lbl.Text = this.GetMessage("generic datecreated");
        this.ltr_required_lbl.Text = this.GetMessage("lbl date required");
        this.ltr_completed_lbl.Text = this.GetMessage("lbl date completed");
        this.ltr_orderstatus_lbl.Text = this.GetMessage("lbl order status");
        this.ltr_ordertotal_lbl.Text = this.GetMessage("lbl order total");
        this.ltr_pipelinestage_lbl.Text = this.GetMessage("lbl order pipeline stage");
        switch (this.m_sPageAction)
        {
            case "vieworder":
                this.pnl_view.Visible = true;
                this.SetTitleBarToMessage("lbl view order");
                break;
            default: // "viewall"
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl order reporting"), this.AppImgPath + "commerce/catalog_view.gif");
                newMenu.AddItem(this.AppImgPath + "/commerce/calendar_down.gif", this.GetMessage("lbl report most recent orders"), "window.location.href=\'reporting.aspx?action=mostrecent\';");
                newMenu.AddItem(this.AppImgPath + "/commerce/calendar.gif", this.GetMessage("lbl report date orders"), "window.location.href=\'reporting.aspx?action=bydates\';");
                newMenu.AddItem(this.AppImgPath + "/menu/users2.gif", this.GetMessage("lbl report customer orders"), "window.location.href=\'reporting.aspx?action=bycustomer\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/brick.png", this.GetMessage("lbl report product orders"), "window.location.href=\'reporting.aspx?action=byproduct\';");
                newMenu.AddItem(this.AppImgPath + "/menu/form_blue.gif", this.GetMessage("lbl report custom orders"), "window.location.href=\'reporting.aspx?action=custom\';");
                this.AddMenu(newMenu);
                this.SetTitleBarToMessage("lbl orders");
                break;
        }
        this.AddHelpButton("orders");
    }
    public string Util_ShowDate(DateTime dtDate)
    {
        string sRet = "";
        if (dtDate == DateTime.MinValue)
        {
            sRet = "-";
        }
        else
        {
            sRet = dtDate.ToShortDateString() + " " + dtDate.ToShortTimeString();
        }
        return sRet;
    }
    public string Util_ShowCustomer(CustomerData Customer)
    {
        string sRet = "";

        // sRet &= cCustomer.CustomerId & "<br/>"
        sRet += (string)("<a href=\"customers.aspx?action=view&id=" + Customer.Id);
        sRet += (string)("<br/>Value:  " + FormatCurrency(Customer.TotalOrderValue, ""));

        return sRet;
    }
    public string Util_ShowAddress(int ShippingAddressId)
    {
        StringBuilder sbRet = new StringBuilder();
        AddressData shipAddress = null;
        IAddress AddressManager = ObjectFactory.GetAddress();

        shipAddress = AddressManager.GetItem(cCustomer.ShippingAddressId);
        sbRet.Append(shipAddress.AddressLine1).Append("<br />");
        if (shipAddress.AddressLine2.Trim().Length > 0)
        {
            sbRet.Append(shipAddress.AddressLine2).Append("<br />");
        }
        sbRet.Append(shipAddress.City).Append("<br />");
        sbRet.Append(shipAddress.Region.Name).Append(", ");
        sbRet.Append(shipAddress.PostalCode).Append("<br />");
        sbRet.Append(shipAddress.Country.Name).Append("<br />");

        return sbRet.ToString();
    }
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
    }
    #endregion

}
