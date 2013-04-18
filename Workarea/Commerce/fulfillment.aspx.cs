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

public partial class Commerce_fulfillment : workareabase, ICallbackEventHandler
{
    public Commerce_fulfillment()
    {
        AddressManager = ObjectFactory.GetAddress();

    }

    protected CustomerData cCustomer = null;
    protected OrderData order = null;
    protected Customer CustomerManager = null;
    protected bool bCommerceAdmin = false;
    protected string OrderBy = "";
    protected long m_intGroupId = -1;
    protected string GroupName = "EveryOne";
    protected int m_intGroupType = -1; //0-CMS User; 1-Membership User
    protected int m_intUserActiveFlag = 0; //0-Active;1-Deleted;-1-Not verified
    protected string m_strDirection = "asc";
    protected string m_strSearchText = "";
    protected string m_strKeyWords = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected UserData[] user_list;
    protected UserAPI m_refUserApi = new UserAPI();
    protected SettingsData setting_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserData user_data;
    private bool m_bCommunityGroup = false;
    private long m_iCommunityGroup = 0;
    protected string m_strSelectedItem = "-1";
    protected OrderApi orderApi;
    protected List<string> SiteList = new List<string>();
    protected CurrencyData defaultCurrency = null;
    protected string m_strWfImgPath = string.Empty;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected OrderPaymentData payment = null;
    protected long captureOrderId = 0;
    protected RegionApi RegionManager = null;
    protected CountryApi CountryManager = null;
    protected IAddress AddressManager;
    protected string addressType = "";

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }

            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
            {
                throw (new Exception(GetMessage("feature locked error")));
            }
            dg_orders.Columns[0].HeaderText = this.GetMessage("generic id");
            dg_orders.Columns[1].HeaderText = this.GetMessage("lbl attr date");
            dg_orders.Columns[2].HeaderText = this.GetMessage("lbl site");
            dg_orders.Columns[3].HeaderText = this.GetMessage("lbl search status");
            dg_orders.Columns[4].HeaderText = this.GetMessage("lbl customer");
            dg_orders.Columns[5].HeaderText = this.GetMessage("lbl order value");
            orderApi = new OrderApi();
            defaultCurrency = (new CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
            CustomerManager = new Customer(m_refContentApi.RequestInformationRef);
            Util_CheckAccess();
            Util_RegisterResources();
            Util_SetServerJSVariable();
            switch (this.m_sPageAction)
            {

                //case "editaddress":

                //    if (Request.QueryString["addressType"] != "")
                //    {
                //        addressType = Request.QueryString["addressType"];
                //    }

                //    RegionManager = new RegionApi();
                //    CountryManager = new CountryApi();

                //    if (Page.IsPostBack && isCPostData.Value == "")
                //    {
                //        Process_EditAddress();
                //    }
                //    else
                //    {
                //        Display_ViewAddress(true);
                //    }
                //   break;


                case "vieworder":
                    if (Page.IsPostBack && Request.Form["hdn_code"] == "2")
                    {
                        Process_Capture();
                    }
                    else if (Page.IsPostBack && Request.Form["hdn_code"] == "3")
                    {
                        Process_Fraud();
                    }
                    else if (Page.IsPostBack && Request.Form["hdn_code"] == "4")
                    {
                        Process_CancelOrder();
                    }
                    else if (Page.IsPostBack && Request.Form["hdn_code"] == "5")
                    {

                        Process_CallOrderEvent();

                    }
                    else
                    {
                        Display_ViewOrder(null);
                    }
                    break;
                case "showpayment":

                    if (Page.IsPostBack && Request.Form["hdn_code"] == "2")
                    {

                        Process_Capture();

                    }
                    else if (Page.IsPostBack && Request.Form["hdn_code"] == "7")
                    {

                        Process_CaptureAndSettle();

                    }
                    else if (Page.IsPostBack && Request.Form["hdn_code"] == "8")
                    {

                        Process_Settle();

                    }
                    else
                    {

                        Display_ViewPayment();

                    }
                    break;

                case "editnotes":

                    if (Page.IsPostBack && Request.Form["hdn_code"] == "6")
                    {
                        Process_EditNotes();
                    }
                    else
                    {
                        Display_ViewNotes();
                    }
                    break;

                case "trackingnumber":
                    if (Page.IsPostBack)
                    {
                        Process_TrackingNumber();
                    }
                    else
                    {
                        Display_TrackingNumber();
                    }
                    break;
                case "mostrecent":
                    if (Page.IsPostBack == false)
                    {
                        Display_MostRecent();
                    }
                    break;
                case "bydates":
                    if (Page.IsPostBack == false)
                    {
                        Display_ByDates();
                    }
                    break;
                case "byproduct":
                    if (Page.IsPostBack == false)
                    {
                        Display_ByProduct();
                    }
                    break;
                case "bycustomer":
                    if (Page.IsPostBack == false)
                    {
                        Display_ByCustomer();
                    }
                    break;
                case "custom":
                    Display_ViewCustom();
                    break;
                case "onhold":
                    if (Page.IsPostBack == false)
                    {
                        Display_ViewOnHold();
                    }
                    break;
                case "delete":
                    if (Page.IsPostBack == false)
                    {
                        if (bCommerceAdmin)
                        {
                            if (!string.IsNullOrEmpty(Request.QueryString["orderid"]) 
                                && Convert.ToInt64(Request.QueryString["orderid"]) > 0)
                            {
                              orderApi.DeleteOrder(Convert.ToInt64(Request.QueryString["orderid"]));
                              Display_ViewAll();
                            }
                            
                        }
                        else 
                        {
                            throw (new Exception(GetMessage("err not role commerce-admin")));
                        }
                    }
                    break;
                default: // "viewall"
                    if (Page.IsPostBack == false)
                    {
                        Display_ViewAll();
                    }
                    break;
            }
            Util_SetLabels();
            Util_SetJS();

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }
    private string Quote(string KeyWords)
    {
        string result = KeyWords;
        if (KeyWords.Length > 0)
        {
            result = KeyWords.Replace("\'", "\'\'");
        }
        return result;
    }
    private void CollectSearchText()
    {
        m_strKeyWords = Request.QueryString["user"];
        m_strSelectedItem = Request.QueryString["field"];
        if (m_strSelectedItem == "-1 selected")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR user_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "last_name")
        {
            m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "first_name")
        {
            m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
        else if (m_strSelectedItem == "user_name")
        {
            m_strSearchText = " (user_name like \'%" + Quote(m_strKeyWords) + "%\')";
        }
    }
    public void ViewAllUsers()
    {
        m_strKeyWords = Request.QueryString["user"];
        if (!Page.IsPostBack)
        {
            CollectSearchText();
            DisplayUsers();
        }
        else if (IsPostBack == false)
        {
            ViewAllUsersToolBar();
        }
        isPostData.Value = "false";
    }

    protected void Display_ViewNotes()
    {

        pnl_view.Visible = false;
        pnl_viewall.Visible = false;
        pnl_notes.Visible = true;

        order = orderApi.GetItem(this.m_iID);

        txt_ordernotes.Text = order.SpecialInstructions;

    }

    protected void Display_MostRecent()
    {

        List<OrderData> orderList = new List<OrderData>();
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        orderList = orderApi.GetList(orderCriteria);

        TotalPagesNumber = System.Convert.ToInt32(orderCriteria.PagingInfo.TotalPages);
        PagingInfo(TotalPagesNumber, Toggle);

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }
    private void Display_ByCustomer()
    {
        if ((!(Request.QueryString["groupid"] == null)) && (Request.QueryString["groupid"] != ""))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
            if (m_bCommunityGroup)
            {
                m_iCommunityGroup = m_intGroupId;
                m_intGroupId = this.m_refContentApi.EkContentRef.GetCmsGroupForCommunityGroup(m_iCommunityGroup);
            }
        }
        ViewAllUsers();
    }
    private void DisplayUsers()
    {
        List<OrderData> orderList = new List<OrderData>();
        List<OrderData> tempList = new List<OrderData>();
        UserRequestData req = new UserRequestData();
        string sIds = "";
        string[] userID;
        int i = 0;
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber;
        if ((Request.QueryString["user"] != null) && Request.QueryString["user"] != "")
        {
            sIds = Request.QueryString["user"];
            sIds = sIds.TrimEnd(",".ToCharArray());
            userID = sIds.Split(",".ToCharArray());

            if (userID.Length > 0)
            {
                for (i = 0; i <= userID.Length - 1; i++)
                {
                    orderList = orderApi.GetCustomerOrderList(int.Parse(userID[i]), orderCriteria.PagingInfo);
                    tempList.AddRange(orderList);
                }
                if (tempList.Count > 0)
                {
                    TotalPagesNumber = System.Convert.ToInt32(orderCriteria.PagingInfo.TotalPages);
                    PagingInfo(TotalPagesNumber, Toggle);
                    dg_orders.DataSource = tempList;
                    dg_orders.DataBind();
                }
                else
                {
                    Toggle = false;
                    PagingInfo(TotalPagesNumber, Toggle);
                    literal1.Text = this.GetMessage("lbl no orders");
                }
            }
            else
            {
                literal1.Text = this.GetMessage("lbl no orders");
            }
        }
        else
        {
            literal1.Text = this.GetMessage("lbl no orders");
        }

    }
    private string IsSelected(string val)
    {
        if (val == m_strSelectedItem)
        {
            return (" selected ");
        }
        else
        {
            return ("");
        }
    }
    private void ViewAllUsersToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
        result.Append("<td><input type=\"button\" id=\"btnSearch\" Value=\"Filter\" onClick=\"searchuser()\" />&nbsp;");
        result.Append("<img id=\"imgClose\" src=\'../images/application/close_red_sm.jpg\'></img></td></tr></table><br />");
        result.Append("<table><tr>");
        result.Append("<td valign=\"top\">" + m_refMsg.GetMessage("lbl text") + "</td><td><input type=\"text\" size=\"25\" id=\"txtSearch\" name=\"txtSearch\" style=\'background-color:white;\' value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event);\"></td></tr>");
        result.Append("<tr><td>" + m_refMsg.GetMessage("lbl field") + ":</td><td><select style=\'BACKGROUND-COLOR: white;\' id=searchlist name=searchlist bgcolor=blue >");
        result.Append("<option value=\"-1\"" + IsSelected("-1") + ">All</option>");
        result.Append("<option value=\"user_name\"" + IsSelected("user_name") + ">" + m_refMsg.GetMessage("lbl customer username") + "</option>");
        //result.Append("<option value=""user_name""" & IsSelected("display_name") & ">" & m_refMsg.GetMessage("display name label") & "</option>")
        result.Append("<option value=\"first_name\"" + IsSelected("first_name") + ">" + m_refMsg.GetMessage("lbl first name") + "</option>");
        result.Append("<option value=\"last_name\"" + IsSelected("last_name") + ">" + m_refMsg.GetMessage("lbl last name") + "</option>");
        //result.Append("<option value=""last_name""" & IsSelected("orders") & ">" & m_refMsg.GetMessage("lbl orders") & "</option>")
        //result.Append("<option value=""last_name""" & IsSelected("value") & ">" & m_refMsg.GetMessage("lbl value") & "</option>")
        result.Append("</select></td></tr>");
        result.Append("<td>");
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #region Display

    private void Display_TrackingNumber()
    {

        OrderData orderData;

        orderData = orderApi.GetItem(this.m_iID);
        chk_markasshipped.Enabled = true;
        chk_markasshipped.Checked = false;
        if (orderData.Parts[0].TrackingNumber != null && orderData.Parts[0].TrackingNumber != "")
        {
            txt_trackingnumber.Text = orderData.Parts[0].TrackingNumber.ToString();
        }
        if (orderData.Parts[0].DateShipped != DateTime.MinValue && orderData.Parts[0].DateShipped != DateTime.MaxValue)
        {
            chk_markasshipped.Enabled = false;
            chk_markasshipped.Checked = true;
        }

    }
    private void Display_ByProduct()
    {
        long productid = Convert.ToInt64(Request.QueryString["productid"]);
        List<OrderData> orderList = new List<OrderData>();
        Criteria<OrderProperty> reportCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        reportCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        reportCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        orderList = orderApi.GetProductOrderList(productid, reportCriteria.PagingInfo);
        if (orderList.Count > 0)
        {

            TotalPagesNumber = System.Convert.ToInt32(reportCriteria.PagingInfo.TotalPages);
            PagingInfo(TotalPagesNumber, Toggle);

            dg_orders.DataSource = orderList;            
            dg_orders.DataBind();
        }
        else
        {
            Toggle = false;
            PagingInfo(TotalPagesNumber, Toggle);
            literal1.Text = this.GetMessage("lbl no orders");
        }
    }
    protected void Display_ViewOrder(OrderData capturedOrder)
    {
        pnl_viewall.Visible = false;

        if (capturedOrder != null)
        {
            order = capturedOrder;
        }
        else
        {
            order = orderApi.GetItem(this.m_iID);
        }

        if (order == null || order.Id == 0)
        {

            literal1.Text = this.GetMessage("lbl no orders");

        }
        else
        {

            ltr_orderid.Text = GetMessage("lbl order id") + ": " + order.Id.ToString();
            ltr_status.Text = "<label class=\"label\">" + GetMessage("generic status") + ": " + Util_ShowStatus(order.Status) + "</label>";
            ltr_payments.Text = "<label class=\"paymentLabel\">" + GetMessage("lbl payment(s)") + ":" + "</label>";
            ltr_coupons.Text = "<label class=\"couponLabel\">" + GetMessage("lbl coupons") + ":" + "</label>";
            Util_PopulateCustomer(order.Customer);

            if (order.SpecialInstructions != "")
            {
                ltr_notes.Text = order.SpecialInstructions;
            }
            else
            {
                ltr_notes.Text = "&#160;";
            }

            ltr_created.Text = Util_ShowDate(order.DateCreated);
            if (order.Payments.Count == 1)
            {

                ltr_authorized.Text = Util_ShowDate(order.Payments[0].AuthorizedOn);
                ltr_captured.Text = Util_ShowDate(order.Payments[0].CapturedOn);
            }
            else
            {

                tr_authorized.Visible = false;
                tr_captured.Visible = false;

            }

            Display_ViewPayments(order.Payments);
            Display_ViewCoupons(order.Coupons);

            ltr_completed.Text = Util_ShowDate(order.DateCompleted);
            ltr_required.Text = Util_ShowDate(order.DateRequired);
            ltr_shipped.Text = Util_ShowDate(order.Parts[0].DateShipped);

            m_strWfImgPath = m_refUserApi.AppPath + "workflowimage.aspx?type=order&id=" + order.Id.ToString();

            ltr_workflow_image.Text = "<div class=\"workflowimgwrapper\"><img src=\"" + m_refUserApi.AppPath + "wfactivities.png?instanceid=" + order.WorkflowId.ToString() + "\" class=\"workflowimage\" /></div>";

            ltr_order_billing.Text = Util_ShowAddress(order.BillingAddressId, false);
            ltr_order_shipping.Text = Util_ShowAddress(order.Parts[0].ShippingAddressId, true);

            ltr_subtotal.Text = order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.Subtotal, order.Currency.CultureCode);
            ltr_coupontotal.Text = "(" + order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.CouponTotal, order.Currency.CultureCode) + ")";
            ltr_taxtotal.Text = order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.TaxTotal, order.Currency.CultureCode);
            ltr_shippingtotal.Text = order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.ShippingTotal, order.Currency.CultureCode);
            ltr_shippingtotal_tax_total.Text = order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.ShippingTaxTotal, order.Currency.CultureCode);
			ltr_ordertotal.Text = order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.OrderTotal, order.Currency.CultureCode);
            ltr_lineitems.Text = "";
            for (int i = 0; i <= (order.Parts.Count - 1); i++)
            {
                for (int j = 0; j <= (order.Parts[i].Lines.Count - 1); j++)
                {
                    ltr_lineitems.Text += "<tr style=\"background-color: White;\">";
                    ltr_lineitems.Text += "<td align=\"left\" valign=\"top\">" + order.Parts[i].Lines[j].ProductTitle;
                    if (order.Parts[i].Lines[j].Configuration.KitOptions.Count > 0)
                    {
                        int k = 0;
                        for (k = 0; k <= order.Parts[i].Lines[j].Configuration.KitOptions.Count - 1; k++)
                        {
                            ltr_lineitems.Text += "<br />&nbsp;&nbsp;" + order.Parts[i].Lines[j].Configuration.KitOptions[k].GroupName + "&nbsp;:&nbsp;" + order.Parts[i].Lines[j].Configuration.KitOptions[k].GroupOptionName;
                        }
                    }
                    ltr_lineitems.Text += "</td>";
                    ltr_lineitems.Text += "<td align=\"right\" valign=\"top\">" + order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.Parts[i].Lines[j].PriceEach, order.Currency.CultureCode) + "</td>";
                    ltr_lineitems.Text += "<td align=\"right\" valign=\"top\">" + order.Parts[i].Lines[j].Quantity + "</td>";
                    ltr_lineitems.Text += "<td align=\"right\" valign=\"top\">" + order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(order.Parts[i].Lines[j].PriceSubTotal, order.Currency.CultureCode) + "</td>";
                    ltr_lineitems.Text += "</tr>";
                }
            }

        }

    }

    protected void Display_ViewPayments(List<OrderPaymentData> payments)
    {

        dg_payments.DataSource = payments;

        dg_payments.DataBind();

    }

    protected void Display_ViewCoupons(List<CouponData> coupons)
    {

        dg_coupons.DataSource = coupons;

        dg_coupons.DataBind();

    }

    private void Display_ViewPayment()
    {

        payment = orderApi.GetOrderPayment(this.m_iID);
        order = orderApi.GetItem(payment.OrderId);

        ltr_transactionId.Text = payment.TransactionId;
        ltr_gateway.Text = payment.Gateway;
        ltr_type.Text = payment.PaymentType;

        if (payment.PaymentType.ToLower() != "ektron.cms.commerce.creditcardpayment")
        {
            tr_last4.Visible = false;
        }

        ltr_last4.Text = payment.Last4Digits;
        ltr_amount.Text = Util_ShowPrice(payment.PaymentTotal, payment.Currency.Id);
        ltr_paymentdate.Text = Util_ShowDate(payment.PaymentDate);
        ltr_authorizeddate.Text = Util_ShowDate(payment.AuthorizedOn);
        ltr_captureddate.Text = Util_ShowDate(payment.CapturedOn);

        if (payment.PaymentType.ToLower() == "ektron.cms.commerce.paypalpayment" || payment.PaymentType.ToLower() == "ektron.cms.commerce.checkpayment")
        {

            tr_settled.Visible = true;
            ltr_settleddate.Text = Util_ShowDate(payment.SettledDate);

        }

    }

    protected void Display_ByDates()
    {
        DateTime startDate = Convert.ToDateTime(Request.QueryString["startdate"]);
        DateTime endDate = Convert.ToDateTime(Request.QueryString["enddate"]);
        List<OrderData> orderList = new List<OrderData>();
        Criteria<OrderProperty> reportCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        reportCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, startDate);
        reportCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.LessThanOrEqualTo, endDate);
        reportCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        reportCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        orderList = orderApi.GetList(reportCriteria);

        if (orderList.Count > 0)
        {
            TotalPagesNumber = System.Convert.ToInt32(reportCriteria.PagingInfo.TotalPages);
            PagingInfo(TotalPagesNumber, Toggle);
            this.dg_orders.DataSource = orderList;
            this.dg_orders.DataBind();
        }
        else
        {
            Toggle = false;
            PagingInfo(TotalPagesNumber, Toggle);
            literal1.Text = this.GetMessage("lbl no orders");
        }
    }
    protected void PagingInfo(int TotalPageNumber, bool Toggle)
    {

        if (Toggle == false)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else if (Toggle == true)
        {
            if (TotalPagesNumber <= 1)
            {
                TotalPages.Visible = false;
                CurrentPage.Visible = false;
                lnkBtnPreviousPage.Visible = false;
                NextPage.Visible = false;
                LastPage.Visible = false;
                FirstPage.Visible = false;
                PageLabel.Visible = false;
                OfLabel.Visible = false;
            }
            else
            {
                lnkBtnPreviousPage.Enabled = true;
                FirstPage.Enabled = true;
                LastPage.Enabled = true;
                NextPage.Enabled = true;
                TotalPages.Visible = true;
                CurrentPage.Visible = true;
                lnkBtnPreviousPage.Visible = true;
                NextPage.Visible = true;
                LastPage.Visible = true;
                FirstPage.Visible = true;
                PageLabel.Visible = true;
                OfLabel.Visible = true;

                TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
                TotalPages.ToolTip = TotalPages.Text;

                CurrentPage.Text = _currentPageNumber.ToString();
                CurrentPage.ToolTip = CurrentPage.Text;

                if (_currentPageNumber == 1)
                {
                    lnkBtnPreviousPage.Enabled = false;
                    FirstPage.Enabled = false;
                }
                else if (_currentPageNumber == TotalPagesNumber)
                {
                    NextPage.Enabled = false;
                    LastPage.Enabled = false;
                }
            }
        }

    }

    protected void Display_ViewAll()
    {
        List<OrderData> orderList = new List<OrderData>();
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        orderCriteria.OrderByField = OrderProperty.DateCreated;
        orderCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending;

        orderList = orderApi.GetList(orderCriteria);

        TotalPagesNumber = System.Convert.ToInt32(orderCriteria.PagingInfo.TotalPages);
        PagingInfo(TotalPagesNumber, Toggle);

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();

        Util_ShowSites();

    }

    protected void Display_ViewCustom()
    {
        List<OrderData> orderList = new List<OrderData>();
        Criteria<OrderProperty> reportCriteria = new Criteria<OrderProperty>();

        if (Request.QueryString["sites"] != "")
        {

            string[] sites = Request.QueryString["sites"].Split(",".ToCharArray());
            List<string> tempSiteList = new List<string>();

            for (int i = 0; i <= (sites.Length - 1); i++)
            {

                if (sites[i] != "")
                {
                    tempSiteList.Add(sites[i]);
                }

            }

            reportCriteria.AddFilter(OrderProperty.Site, CriteriaFilterOperator.In, tempSiteList.ToArray());

        }

        orderList = orderApi.GetList(reportCriteria);

        dg_orders.DataSource = orderList;
        dg_orders.DataBind();

        Util_ShowSites();

    }

    protected void Display_ViewOnHold()
    {
        List<OrderData> orderList = new List<OrderData>();
        OrderApi orderApi = new OrderApi();
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        bool Toggle = true;

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        orderList = orderApi.GetOnHoldOrderList(orderCriteria.PagingInfo);
        TotalPagesNumber = System.Convert.ToInt32(orderCriteria.PagingInfo.TotalPages);
        PagingInfo(TotalPagesNumber, Toggle);
        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
    }

    protected void Display_ViewAddress(bool WithEdit)
    {

        pnl_view.Visible = false;
        pnl_viewall.Visible = false;

        AddressData aAddress = null;
        //Dim regioncriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)

        //RegionManager = New RegionApi()
        //regioncriteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)

        //If Not Me.m_iID > 0 Then
        //    regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, drp_address_country.SelectedIndex)
        //End If
        //regioncriteria.PagingInfo.RecordsPerPage = 1000
        //drp_address_region.DataTextField = "Name"
        //drp_address_region.DataValueField = "Id"
        //drp_address_region.DataSource = RegionManager.GetList(regioncriteria)
        //drp_address_region.DataBind()

        Ektron.Cms.Common.Criteria<CountryProperty> addresscriteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        addresscriteria.AddFilter(CountryProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
        addresscriteria.PagingInfo.RecordsPerPage = 1000;
        drp_address_country.DataTextField = "Name";
        drp_address_country.DataValueField = "Id";
        drp_address_country.DataSource = CountryManager.GetList(addresscriteria);
        drp_address_country.DataBind();

        if (this.m_iID > 0)
        {

            order = orderApi.GetItem(this.m_iID);

            if (addressType == "billing")
            {

                aAddress = AddressManager.GetItem(order.BillingAddressId);

            }
            else if (addressType == "shipping")
            {

                aAddress = AddressManager.GetItem(order.Parts[0].ShippingAddressId);

            }

            if (!Page.IsPostBack)
            {

                ltr_address_id.Text = aAddress.Id.ToString();
                txt_address_name.Text = aAddress.Name;
                txt_address_company.Text = aAddress.Company;
                txt_address_line1.Text = aAddress.AddressLine1;
                txt_address_line2.Text = aAddress.AddressLine2;
                txt_address_city.Text = aAddress.City;
                drp_address_country.SelectedIndex = Util_FindItem(aAddress.Country.Id, "country");
                Util_BindRegions(aAddress.Country.Id);
                drp_address_region.SelectedValue = aAddress.Region.Id.ToString();
                txt_address_postal.Text = aAddress.PostalCode;
                txt_address_phone.Text = aAddress.Phone;

            }

        }

    }

    #endregion

    #region Process

    protected void Process_EditNotes()
    {

        orderApi.SetNotes(this.m_iID, (string)txt_ordernotes.Text);

        literal1.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
        literal1.Text += "    parent.ektb_remove(); " + Environment.NewLine;
        literal1.Text += "    parent.window.location = \'fulfillment.aspx?action=vieworder&id=" + this.m_iID.ToString() + "\';" + Environment.NewLine;
        literal1.Text += "</script>" + Environment.NewLine;

    }

    //protected void Process_EditAddress()
    //{

    //    AddressData aAddress = null;

    //    order = orderApi.GetItem(this.m_iID);

    //    if (addressType == "billing")
    //    {

    //        aAddress = AddressManager.GetItem(order.BillingAddressId);

    //    }
    //    else if (addressType == "shipping")
    //    {

    //        aAddress = AddressManager.GetItem(order.Parts[0].ShippingAddressId);

    //    }

    //    aAddress.Name = (string)txt_address_name.Text;
    //    aAddress.Company = (string)txt_address_company.Text;
    //    aAddress.AddressLine1 = (string)txt_address_line1.Text;
    //    aAddress.AddressLine2 = (string)txt_address_line2.Text;
    //    aAddress.City = (string)txt_address_city.Text;
    //    RegionData rData = new RegionData();
    //    rData.Id = Convert.ToInt64(drp_address_region.SelectedValue);
    //    aAddress.Region = rData;
    //    aAddress.PostalCode = (string)txt_address_postal.Text;
    //    CountryData cData = new CountryData();
    //    cData.Id = System.Convert.ToInt32(drp_address_country.SelectedValue);
    //    aAddress.Country = cData;
    //    aAddress.Phone = (string)txt_address_phone.Text;

    //    if (aAddress.Id > 0)
    //    {
    //        AddressManager.UpdateOrderAddress(aAddress, this.m_iID, System.Convert.ToBoolean(addressType == "shipping"), System.Convert.ToBoolean(addressType == "billing"));
    //    }

    //    literal1.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
    //    literal1.Text += "    parent.ektb_remove(); " + Environment.NewLine;
    //    literal1.Text += "    parent.window.location = \'fulfillment.aspx?action=vieworder&id=" + this.m_iID.ToString() + "\';" + Environment.NewLine;
    //    literal1.Text += "</script>" + Environment.NewLine;

    //}

    protected void Process_Payment()
    {

        // Process_Capture()

    }

    protected void Process_CallOrderEvent()
    {

        if (bCommerceAdmin)
        {

            string eventTrigger = Request.Form["hdn_event"];
            EkEnumeration.OrderWorkflowEvent orderEvent;

            switch (eventTrigger.ToLower())
            {
                case "onorderupdated":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderUpdated;
                    break;
                case "onordercancelled":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderCancelled;
                    break;
                case "onorderfraud":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderFraud;
                    break;
                case "onordercaptured":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderCaptured;
                    break;
                case "onordershipped":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderShipped;
                    break;
                case "onorderprocessed":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderProcessed;
                    break;
                case "onorderpayment":
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderPaymentReceived;
                    break;
                default:
                    return;
            }


            hdn_event.Value = "";
            hdn_code.Value = "";

            orderApi.RaiseWorkflowEvent(this.m_iID, orderEvent);

            literal1.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
            // literal1.Text &= "    ektb_show('','#EkTB_inline?height=18&width=500&inlineId=dvHoldMessage&modal=true', null, '', true); " & Environment.NewLine
            literal1.Text += "    setTimeout(\"window.location = \'fulfillment.aspx?action=vieworder&id=" + this.m_iID.ToString() + "\';\",3000);" + Environment.NewLine;
            literal1.Text += "</script>" + Environment.NewLine;

        }

    }

    protected void Process_CancelOrder()
    {

        if (bCommerceAdmin)
        {

            hdn_code.Value = "";
            orderApi.SetOrderStatus(this.m_iID, EkEnumeration.OrderStatus.Cancelled);
            Display_ViewOrder(null);

        }

    }

    protected void Process_TrackingNumber()
    {

        if (bCommerceAdmin)
        {

            orderApi.SetTrackingNumber(this.m_iID, Request.Form[txt_trackingnumber.UniqueID], System.Convert.ToBoolean(chk_markasshipped.Checked));

            ltr_js.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
            ltr_js.Text += "parent.window.location.reload(false);" + Environment.NewLine; // refresh parent
            ltr_js.Text += "parent.ektb_remove();" + Environment.NewLine; // close thickbox
            ltr_js.Text += "</script>" + Environment.NewLine;

        }

    }
    protected void Process_Fraud()
    {

        if (bCommerceAdmin)
        {

            hdn_code.Value = "";
            orderApi.MarkAsFraud(this.m_iID);
            Display_ViewOrder(null);

        }

    }
    protected void Process_Capture()
    {

        try
        {

            if (Request.QueryString["orderid"] != "")
            {
                captureOrderId = Convert.ToInt64(Request.QueryString["orderid"]);
            }

            if (bCommerceAdmin && captureOrderId > 0)
            {

                hdn_code.Value = "";

                orderApi.Capture(captureOrderId, this.m_iID);

                ltr_js.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
                ltr_js.Text += "parent.location.reload(true);" + Environment.NewLine; // refresh parent
                ltr_js.Text += "parent.ektb_remove();" + Environment.NewLine; // close thickbox
                ltr_js.Text += "</script>" + Environment.NewLine;

            }
            else
            {

                throw (new Exception(GetMessage("err not role commerce-admin")));

            }

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }

    protected void Process_CaptureAndSettle()
    {

        try
        {

            if (Request.QueryString["orderid"] != "")
            {
                captureOrderId = Convert.ToInt64(Request.QueryString["orderid"]);
            }

            if (bCommerceAdmin && captureOrderId > 0)
            {

                hdn_code.Value = "";

                orderApi.Capture(captureOrderId, this.m_iID);
                orderApi.MarkPaymentAsSettled(captureOrderId, this.m_iID);

                ltr_js.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
                ltr_js.Text += "parent.location.reload(true);" + Environment.NewLine; // refresh parent
                ltr_js.Text += "parent.ektb_remove();" + Environment.NewLine; // close thickbox
                ltr_js.Text += "</script>" + Environment.NewLine;

            }
            else
            {

                throw (new Exception(GetMessage("err not role commerce-admin")));

            }

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }

    protected void Process_Settle()
    {

        try
        {

            if (Request.QueryString["orderid"] != "")
            {
                captureOrderId = Convert.ToInt64(Request.QueryString["orderid"]);
            }

            if (bCommerceAdmin && captureOrderId > 0)
            {

                hdn_code.Value = "";

                orderApi.MarkPaymentAsSettled(captureOrderId, this.m_iID);

                ltr_js.Text = " <script type=\"text/javascript\">" + Environment.NewLine;
                ltr_js.Text += "parent.location.reload(true);" + Environment.NewLine; // refresh parent
                ltr_js.Text += "parent.ektb_remove();" + Environment.NewLine; // close thickbox
                ltr_js.Text += "</script>" + Environment.NewLine;

            }
            else
            {

                throw (new Exception(GetMessage("err not role commerce-admin")));

            }

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }


    #endregion

    #region Util

    protected void Util_SetLabels()
    {

        ltr_customerorders_lbl.Text = GetMessage("lbl orders");
        ltr_customertotal_lbl.Text = GetMessage("lbl total order value");
        ltr_customeravg_lbl.Text = GetMessage("lbl avg order value");

        ltr_notes_lbl.Text = GetMessage("lbl ecomm order notes") + (!m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled ? ("<a href=\"#\" class=\"button buttonInline greenHover buttonEdit btnEdit\" onclick=\"if (allowOpen) { $ektron('" + uxNotesDialog.Selector + "').dialog('open');EktronUiDialogInit('notes'," + m_iID.ToString() + "); }\" title=\"" + GetMessage("btn edit") + "\" >" + GetMessage("btn edit") + "</a>") : "");

        ltr_created_lbl.Text = GetMessage("generic datecreated");
        ltr_authorized_lbl.Text = GetMessage("lbl date authorized");
        ltr_captured_lbl.Text = GetMessage("lbl date captured");
        ltr_settleddate_lbl.Text = GetMessage("lbl date settled");
        ltr_shipped_lbl.Text = GetMessage("lbl date shipped");
        ltr_shippingtotal_tax_lbl.Text = GetMessage("lbl ship tax");
		ltr_required_lbl.Text = GetMessage("lbl date required");
        ltr_completed_lbl.Text = GetMessage("lbl date completed");

        ltr_order_billing_lbl.Text = GetMessage("lbl billing") ;//+ (!m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled ? ("<a href=\"#\" class=\"button buttonInline greenHover buttonEdit btnEdit\" onclick=\"if (allowOpen) { ektb_show(\'" + GetMessage("lbl edit address") + "\', \'fulfillment.aspx?addressType=billing&action=editaddress&id=" + m_iID.ToString() + "&EkTB_iframe=true&width=500&height=350&scrolling=true&modal=true\', false); } \" title=\"" + GetMessage("btn edit") + "\" >" + GetMessage("btn edit") + "</a>") : "");
        ltr_order_shipping_lbl.Text = GetMessage("lbl shipping");// + (!m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled ? ("<a href=\"#\" class=\"button buttonInline greenHover buttonEdit btnEdit\" onclick=\"if (allowOpen) { ektb_show(\'" + GetMessage("lbl edit address") + "\', \'fulfillment.aspx?addressType=shipping&action=editaddress&id=" + m_iID.ToString() + "&EkTB_iframe=true&width=500&height=350&scrolling=true&modal=true\', false); } \" title=\"" + GetMessage("btn edit") + "\" >" + GetMessage("btn edit") + "</a>") : "");

        ltr_subtotal_lbl.Text = GetMessage("lbl subtotal");
        ltr_coupontotal_lbl.Text = GetMessage("lbl coupon total");
        ltr_taxtotal_lbl.Text = GetMessage("lbl wa tax");
        ltr_shippingtotal_lbl.Text = GetMessage("lbl shipping");
        ltr_ordertotal_lbl.Text = GetMessage("lbl order total");

        ltr_trackingnumber.Text = GetMessage("lbl tracking number");
        ltr_markasshipped.Text = GetMessage("lbl mark as shipped");
        hdn_errorMessage.Value = string.Format(GetMessage("js alert field cannot include"), "< >");
        hdn_errOrderNotesMessage.Value = string.Format(GetMessage("js alert field cannot include"), "< >");

        ltr_desc_lbl.Text = GetMessage("generic description");
        ltr_saleprice_lbl.Text = GetMessage("lbl sale price");
        ltr_qty_lbl.Text = GetMessage("lbl quantity");
        ltr_total_lbl.Text = GetMessage("lbl total");

        ltr_transactionId_lbl.Text = GetMessage("lbl transactionid");
        ltr_gateway_lbl.Text = GetMessage("lbl payment gateway");
        ltr_type_lbl.Text = GetMessage("generic type");
        ltr_last4_lbl.Text = GetMessage("lbl last 4 digits");
        ltr_amount_lbl.Text = GetMessage("lbl payment total");
        ltr_paymentdate_lbl.Text = GetMessage("lbl payment date");
        ltr_authorizeddate_lbl.Text = GetMessage("lbl date authorized");
        ltr_captureddate_lbl.Text = GetMessage("lbl date captured");

        ltr_holdmsg.Text = m_refMsg.GetMessage("one moment msg");

        ltr_showidsearch.Text = GetMessage("lbl reporting id orders");
        ltr_searchid.Text = GetMessage("lbl err order id invalid");

        this.ltr_address_id_lbl.Text = this.GetMessage("generic id");
        this.ltr_address_name.Text = this.GetMessage("lbl address name");
        this.ltr_address_company.Text = this.GetMessage("lbl address company");
        this.ltr_address_line1.Text = this.GetMessage("lbl address street");
        this.ltr_address_city_lbl.Text = this.GetMessage("lbl address city");
        this.ltr_address_region.Text = this.GetMessage("lbl address state province");
        this.ltr_address_postal.Text = this.GetMessage("lbl address postal");
        this.ltr_address_country.Text = this.GetMessage("lbl address country");
        this.ltr_address_phone.Text = this.GetMessage("lbl address phone");

        HttpBrowserCapabilities browser = Request.Browser;
        Ektron.Cms.Framework.Context.CmsContextService context = new Ektron.Cms.Framework.Context.CmsContextService();
        switch (this.m_sPageAction)
        {

            case "editnotes":

                pnl_viewall.Visible = false;
                pnl_trackingnumber.Visible = false;
                pnl_payment.Visible = false;
                pnl_notes.Visible = true;

                if (browser.Type.Contains("IE") && browser.MajorVersion >= 9)
                {
                    // work around to prevent errors in IE9 when it destroys native JS objects
                    // see http://msdn.microsoft.com/en-us/library/gg622929%28v=VS.85%29.aspx
                    uxEditNotesIframe.Attributes.Add("src", "about:blank");
                }
                else
                {
                    uxEditNotesIframe.Attributes.Add("src", context.SitePath + "fulfillment.aspx?action=editnotes&id=" + m_iID.ToString() + "&width=350&height=250&scrolling=true&modal=true");
                }
                //uxNotesDialog.Title = m_refMsg.GetMessage("lbl ecomm edit order notes");

                //AddBackButton("javascript:$ektron('" + uxNotesDialog.Selector + "').dialog('close');");
                workareamenu newMenu_1 = new workareamenu("file", GetMessage("lbl action"), m_refContentApi.AppPath + "images/UI/Icons/check.png");
                newMenu_1.AddItem(m_refContentApi.AppPath + "images/ui/icons/save.png", GetMessage("btn save"), "EditNotes();");
                AddMenu(newMenu_1);
                SetTitleBarToMessage("lbl ecomm edit order notes");
                break;

            case "vieworder":

				if (Request.QueryString["page"] == "workarea")
				{
					// redirect to workarea when user clicks back button if we're in workarea
					AddBackButton("javascript:top.switchDesktopTab()");
				}
				else
				{
					AddBackButton("fulfillment.aspx");
				}

                if (bCommerceAdmin && (order != null))
                {

                    workareamenu newMenu = new workareamenu("file", GetMessage("lbl action"), m_refContentApi.AppPath + "images/UI/Icons/check.png");
                    IList<string> options = null;
                    try
                    {

                        if (!m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled && !(order.WorkflowId == Guid.Empty))
                        {
                            options = Ektron.Workflow.Runtime.WorkflowHandler.GetOrderEventQueue(order.WorkflowId);
                        }

                    }
                    catch (Exception ex)
                    {

                        if (order.WorkflowId != Guid.Empty && ex.Message.IndexOf(order.WorkflowId.ToString()) == -1)
                        {
                            throw (ex);
                        }

                    }

                    if (options != null)
                    {

                        int addedOptions = 0;

                        for (int i = 0; i <= (options.Count - 1); i++)
                        {

                            string EventImage = "arrow_right_green.gif";
                            string EventName = (string)(options[i].Replace("OnOrder", ""));
                            bool addThisItem = true;


                            switch (EventName.ToLower())
                            {

                                case "updated":

                                    EventName = GetMessage("lbl workflow updated");
                                    addThisItem = false;
                                    break;

                                case "processed":

                                    EventName = GetMessage("lbl workflow processed");
                                    addedOptions++;
                                    break;

                                case "shipped":

                                    EventName = GetMessage("lbl workflow shipped");
                                    EventImage = "commerce/bundle.gif";
                                    addThisItem = false;
                                    if (order.Status != EkEnumeration.OrderStatus.Fraud && order.Status != EkEnumeration.OrderStatus.Cancelled && order.Status != EkEnumeration.OrderStatus.Completed && Util_NeedsTrackingNumber())
                                    {
                                        newMenu.AddBreak();
                                        newMenu.AddItem(AppImgPath + "commerce/bundle.gif", GetMessage("lbl add tracking number"), "AddTrackingNumber();");
                                        addedOptions++;
                                    }
                                    else if (Util_CanEditTrackingNumber())
                                    {
                                        addThisItem = true;
                                        newMenu.AddBreak();
                                        newMenu.AddItem(AppImgPath + "commerce/bundle.gif", GetMessage("lbl edit tracking number"), "AddTrackingNumber();");
                                        addedOptions++;
                                    }
                                    break;

                                case "cancelled":

                                    EventName = GetMessage("lbl cancel order");
                                    EventImage = "commerce/cancel.gif";
                                    addThisItem = false;
                                    if (order.Status != EkEnumeration.OrderStatus.Cancelled && order.Status != EkEnumeration.OrderStatus.Fraud && order.Status != EkEnumeration.OrderStatus.Completed)
                                    {
                                        newMenu.AddBreak();
                                        newMenu.AddItem(AppImgPath + "commerce/cancel.gif", GetMessage("lbl cancel order"), "CancelOrder();");
                                        addedOptions++;
                                    }
                                    break;

                                case "fraud":

                                    EventName = GetMessage("lbl mark fraud");
                                    EventImage = "commerce/fraud.gif";
                                    addThisItem = false;
                                    if (order.Status != EkEnumeration.OrderStatus.Fraud && order.Status != EkEnumeration.OrderStatus.Cancelled && order.Status != EkEnumeration.OrderStatus.Completed)
                                    {
                                        newMenu.AddItem(AppImgPath + "commerce/fraud.gif", GetMessage("lbl mark fraud"), "MarkAsFraud();");
                                        addedOptions++;
                                    }
                                    break;

                                default:

                                    EventName = options[i];
                                    addedOptions++;
                                    break;

                            }

                            if (addThisItem)
                            {
                                newMenu.AddItem(AppImgPath + EventImage, EventName, "CallOrderEvent(\'" + options[i] + "\');");
                            }

                        }

                        if (addedOptions > 0)
                        {
                            newMenu.AddBreak();
                        }

                    }

                    if (IsCommerceAdmin)
                    {
                        newMenu.AddItem(AppImgPath + "adobe-pdf.gif", "Export As PDF", "window.open(\'" + m_refContentApi.AppPath + "commerce/export/order.aspx?id=" + m_iID.ToString() + "&type=pdf\', \'PDF\');");
                        newMenu.AddItem(AppImgPath + "ms-excel.gif", "Export As XLS", "window.open(\'" + m_refContentApi.AppPath + "commerce/export/order.aspx?id=" + m_iID.ToString() + "&type=xls\', \'XLS\');");
                        newMenu.AddItem(AppImgPath + "ms-notepad.gif", "Export As CSV", "window.open(\'" + m_refContentApi.AppPath + "commerce/export/order.aspx?id=" + m_iID.ToString() + "&type=csv\', \'CSV\');");
                        this.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", m_refContentApi.AppPath + "Commerce/fulfillment.aspx?action=delete&orderid=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js: confirm delete order") + "\');\" ", StyleHelper.DeleteButtonCssClass, true);
                    }

                    this.AddMenu(newMenu);
                }

                if (order == null)
                {
                    this.pnl_view.Visible = false;
                }
                else
                {
                    this.pnl_view.Visible = true;
                }

                this.SetTitleBarToMessage("lbl view order");
                break;


            case "showpayment":

                pnl_viewall.Visible = false;
                pnl_trackingnumber.Visible = false;
                pnl_payment.Visible = true;
                tr_orderpart.Visible = false;

                if (browser.Type.Contains("IE") && browser.MajorVersion >= 9)
                {
                    // work around to prevent errors in IE9 when it destroys native JS objects
                    // see http://msdn.microsoft.com/en-us/library/gg622929%28v=VS.85%29.aspx
                    uxPaymentIframe.Attributes.Add("src", "about:blank");
                }
                else
                {
                    uxPaymentIframe.Attributes.Add("src", context.SitePath + "fulfillment.aspx?action=showpayment&orderid=" + m_iID.ToString() + "&width=500&height=300&scrolling=true&modal=true");
                }

				//AddBackButton("javascript:parent.ektb_remove();");

                workareamenu newMenu_2 = new workareamenu("file", GetMessage("lbl action"), AppImgPath + "check20.gif");

                if (!m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled && !Page.IsPostBack)
                {

                    if (order.Status != EkEnumeration.OrderStatus.Cancelled && order.Status != EkEnumeration.OrderStatus.Fraud)
                    {

                        if (payment.CapturedOn == DateTime.MinValue)
                        {
                            newMenu_2.AddItem(AppImgPath + "commerce/Submit.gif", GetMessage("lbl capture"), "CaptureOrder();");
                            if (payment.PaymentType.ToLower() == "ektron.cms.commerce.paypalpayment" || payment.PaymentType.ToLower() == "ektron.cms.commerce.checkpayment")
                            {
                                newMenu_2.AddItem(AppImgPath + "commerce/accept.gif", GetMessage("lbl capture and settle"), "CaptureOrderAndSettle();");
                            }
                        }

                        if (payment.CapturedOn != DateTime.MinValue && payment.CapturedOn != DateTime.MaxValue && (payment.SettledDate == DateTime.MinValue || payment.SettledDate == DateTime.MaxValue) && (payment.PaymentType.ToLower() == "ektron.cms.commerce.paypalpayment" || payment.PaymentType.ToLower() == "ektron.cms.commerce.checkpayment"))
                        {
                            newMenu_2.AddItem(AppImgPath + "commerce/accept.gif", GetMessage("lbl settle"), "SettlePayment();");

                        }

                    }

                }

                if (payment != null && payment.PaymentType.ToLower() == "ektron.cms.commerce.paypalpayment")
                {
                    newMenu_2.AddItem(AppImgPath + "commerce/paypal.gif", GetMessage("lbl check payment status"), "CheckPayPalStatus();");
                }

                this.AddMenu(newMenu_2);

                
                SetTitleBarToMessage("lbl view payment");
                break;


            case "trackingnumber":

                pnl_viewall.Visible = false;
                pnl_trackingnumber.Visible = true;
                tr_orderpart.Visible = false;
                AddBackButton("javascript:parent.ektb_remove();");
                AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=\"SubmitTrackingNumber(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                break;

            //case "editaddress":
            //    pnl_viewaddress.Visible = true;
            //    this.tr_address_id.Visible = this.m_iID > 0;
            //    AddBackButton("javascript:parent.ektb_remove();");
            //    this.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=\"CheckAddress(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
            //    this.SetTitleBarToString(this.GetMessage((string)(this.m_iID > 0 ? "lbl edit address" : "lbl add address")));
            //    break;

            default: // "viewall"

                workareamenu newMenu_3 = new workareamenu("file", GetMessage("lbl order reporting"), AppImgPath + "commerce/catalog_view.gif");
                newMenu_3.AddItem(AppImgPath + "commerce/calendar_down.gif", GetMessage("lbl report most recent orders"), "window.location.href = \'fulfillment.aspx?action=mostrecent\';");
                newMenu_3.AddItem(AppImgPath + "commerce/calendar.gif", GetMessage("lbl report date orders"), "ektb_show(\'" + GetMessage("lbl reporting by dates") +"\', \'bydates.aspx?EkTB_iframe=true&width=400&height=150&scrolling=true&modal=true\', false);");
                newMenu_3.AddItem(AppImgPath + "menu/users2.gif", GetMessage("lbl report customer orders"), "ektb_show(\'" + GetMessage("lbl reporting by customer") + "\', \'bycustomer.aspx?EkTB_iframe=true&width=500&height=150&scrolling=true&modal=true\', false);");
                newMenu_3.AddItem(m_refContentApi.AppPath + "images/ui/icons/brick.png", GetMessage("lbl report product orders"), "ektb_show(\'" + GetMessage("lbl reporting by product") + "\', \'byproduct.aspx?action=reporting&EkTB_iframe=true&width=500&height=150&scrolling=true&modal=true\', false);");
                newMenu_3.AddItem(AppImgPath + "commerce/Submit.gif", GetMessage("lbl report id orders"), "ShowOrderIdSearch();");
                //This option is being hidden for now (issue# 38244), and needs to be implemented in next release, which is 7.6.0 release/maintenance.
                //newMenu.AddItem(AppImgPath & "menu/form_blue.gif", GetMessage("lbl report custom orders"), "CustomReport();")
                this.AddMenu(newMenu_3);
                this.SetTitleBarToMessage("lbl orders");
                break;


        }

        this.AddHelpButton((string)("orders" + EkFunctions.UrlEncode(m_sPageAction)));

    }

    public bool Util_NeedsTrackingNumber()
    {

        bool needsTracking = false;

        if (order != null)
        {
            for (int i = 0; i <= (order.Parts.Count - 1); i++)
            {
                if (order.Parts[i].TrackingNumber == "")
                {
                    needsTracking = true;
                    break;
                }
            }
        }

        return needsTracking;

    }

    public bool Util_CanEditTrackingNumber()
    {

        bool canEditTracking = false;

        if ((order != null) && order.Status != EkEnumeration.OrderStatus.Cancelled)
        {
            for (int i = 0; i <= (order.Parts.Count - 1); i++)
            {
                if (order.Parts[i].DateShipped == DateTime.MinValue || order.Parts[i].DateShipped == DateTime.MaxValue)
                {
                    canEditTracking = true;
                    break;
                }
            }
        }

        return canEditTracking;

    }

    public bool Util_IsCaptured()
    {

        bool isCaptured = true;

        if (order != null)
        {
            for (int i = 0; i <= (order.Payments.Count - 1); i++)
            {
                if (order.Payments[i].CapturedOn == DateTime.MinValue || order.Payments[i].CapturedOn == DateTime.MaxValue)
                {
                    isCaptured = false;
                    break;
                }
            }
        }

        return isCaptured;

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

        sRet += "<a style=\"text-decoration:underline;\" href=\"customers.aspx?action=view&id=" + Customer.Id + "\">" + Customer.FirstName + " " + Customer.LastName + " (" + Customer.DisplayName + ")</a>";
        sRet += (string)("<br/>Orders: " + Customer.TotalOrders);
        sRet += (string)("<br/>Value:  " + defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode));
        sRet += (string)("<br/>Avg Value:  " + defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode));

        return sRet;
    }

    public void Util_PopulateCustomer(CustomerData Customer)
    {

        ltr_customername.Text = "<a href=\"customers.aspx?action=view&id=" + Customer.Id + "\">" + Customer.FirstName + " " + Customer.LastName + " (" + Customer.DisplayName + ")</a>";
        if (!(Customer.IsDeleted))
        {
            ltr_customername.Text += "&#160;<a href=\"#\" onclick=\"$ektron('" + uxEmailDialog.Selector + "').dialog('open');EktronUiDialogInit('email'," + Customer.Id.ToString() + ") \"><img alt=\"" + GetMessage("btn email") + "\" title=\"" + GetMessage("btn email") + "\" src=\"" + m_refContentApi.AppPath + "Images/ui/icons/email.png\" /></a>";

            HttpBrowserCapabilities browser = Request.Browser;
            Ektron.Cms.Framework.Context.CmsContextService context = new Ektron.Cms.Framework.Context.CmsContextService();

            if (browser.Type.Contains("IE") && browser.MajorVersion >= 9)
            {
                // work around to prevent errors in IE9 when it destroys native JS objects
                // see http://msdn.microsoft.com/en-us/library/gg622929%28v=VS.85%29.aspx
                uxEmailIframe.Attributes.Add("src", "about:blank");
                uxCouponIframe.Attributes.Add("src", "about:blank");
            }
            //else
            //{
            //    uxEmailIframe.Attributes.Add("src", context.SitePath + "../email.aspx?userarray=" + Customer.Id.ToString() + "&fromModal=true&width=500&height=620&scrolling=true&modal=true");
            //}
            //uxEmailDialog.Title = m_refMsg.GetMessage("btn email");
            jsUxDialogSelectorTxt.Text = uxEmailDialog.Selector.ToString();
            jsUxCouponDlgSelectorTxt.Text = uxCouponDialog.Selector.ToString();
        }
        ltr_customerorders.Text = Customer.TotalOrders.ToString();
        ltr_customertotal.Text = defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode);
        ltr_customeravg.Text = defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode);

    }

    public string Util_ShowAddress(long ShippingAddressId, bool isShipping)
    {
        StringBuilder sbRet = new StringBuilder();
        AddressData shipAddress = null;

        shipAddress = AddressManager.GetItem(ShippingAddressId);
        sbRet.Append(shipAddress.AddressLine1).Append("<br />");
        if (shipAddress.AddressLine2.Trim().Length > 0)
        {
            sbRet.Append(shipAddress.AddressLine2).Append("<br />");
        }
        sbRet.Append(shipAddress.City).Append("<br />");
        sbRet.Append(shipAddress.Region.Name).Append(", ");
        sbRet.Append(shipAddress.PostalCode).Append("<br />");
        sbRet.Append(shipAddress.Country.Name).Append("<br />");

        if (isShipping)
        {

            if (order.Parts[0].ShippingMethod != "")
            {
                sbRet.Append("<br />Via ").Append(order.Parts[0].ShippingMethod);
            }

            if (order.Parts[0].TrackingNumber != "")
            {

                if (Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager.Provider.IsTrackingSupported)
                {

                    sbRet.Append("<br />").Append(GetMessage("lbl tracking number")).Append(": <a target=\"_blank\" href=\"").Append(Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager.Provider.GetTrackingUrl(order.Parts[0].TrackingNumber)).Append("\">").Append(order.Parts[0].TrackingNumber).Append("</a>");

                }
                else
                {

                    sbRet.Append("<br />").Append(GetMessage("lbl tracking number")).Append(": ").Append(order.Parts[0].TrackingNumber);

                }

            }

        }

        return sbRet.ToString();

    }

    protected void Util_SetJS()
    {

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        switch (m_sPageAction)
        {

            case "trackingnumber":
            case "showpayment":

                if (!Page.IsPostBack)
                {

                    sbJS.Append(" function ShowPayPalStatus(result, context) { ").Append(Environment.NewLine);

                    sbJS.Append("   document.getElementById(\'dvPaymentStatus\').innerHTML = result; ").Append(Environment.NewLine);
                    //sbJS.Append("   alert(result); ").Append(Environment.NewLine)

                    sbJS.Append(" } ").Append(Environment.NewLine);

                    string transactionId = "";
                    if (payment != null && payment.TransactionId.Length > 0)
                    {
                        transactionId = payment.TransactionId;
                    }
                    sbJS.Append(" function CheckPayPalStatus() { ").Append(Environment.NewLine);
                    sbJS.Append("   document.getElementById(\'dvPaymentStatus\').innerHTML = \'<img src=\"" + this.AppImgPath + "ajax-loader_circle_lg.gif\" />\'; ").Append(Environment.NewLine);
                    sbJS.Append("   ").Append(this.ClientScript.GetCallbackEventReference(this, "\'type=paypal&transId=" + transactionId + "\'", "ShowPayPalStatus", "null")).Append(Environment.NewLine);
                    sbJS.Append(" } ").Append(Environment.NewLine);

                    sbJS.Append("</script>").Append(Environment.NewLine);

                    ltr_js.Text = sbJS.ToString();

                }
                break;

            //case "editaddress":

                //sbJS.Append("function isValid(phone)").Append(Environment.NewLine);
                //sbJS.Append("{").Append(Environment.NewLine);
                //sbJS.Append("   return /^(1\\s*[-\\/\\.]?)?(\\((\\d{3})\\)|(\\d{3}))\\s*([\\s-./\\\\])?([0-9]*)([\\s-./\\\\])?([0-9]*)$/.test(phone);").Append(Environment.NewLine);
                //sbJS.Append("}").Append(Environment.NewLine);

                //sbJS.Append("function CheckAddress() {").Append(Environment.NewLine);
                //sbJS.Append("   var sAddrName = Trim(document.getElementById(\'").Append(txt_address_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var sCompany = Trim(document.getElementById(\'").Append(txt_address_company.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var sStrAddr = Trim(document.getElementById(\'").Append(txt_address_line1.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var sCity = Trim(document.getElementById(\'").Append(txt_address_city.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var iPostal = Trim(document.getElementById(\'").Append(txt_address_postal.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var iPhone = Trim(document.getElementById(\'").Append(txt_address_phone.UniqueID).Append("\').value); ").Append(Environment.NewLine);
                //sbJS.Append("   var drp_region = document.getElementById(\"").Append(drp_address_region.UniqueID).Append("\");" + Environment.NewLine);
                //sbJS.Append("   if(drp_region.selectedIndex == -1)").Append(Environment.NewLine);
                //sbJS.Append("    {").Append(Environment.NewLine);
                //sbJS.Append("       alert(\"" + base.GetMessage("js null postalcode region msg") + "\");" + Environment.NewLine);
                //sbJS.Append("       document.forms[\"form1\"].isCPostData.value = \'false\';").Append(Environment.NewLine);
                //sbJS.Append("       return false;").Append(Environment.NewLine);
                //sbJS.Append("    }").Append(Environment.NewLine);
                //sbJS.Append("   if(sAddrName == \'\' || sStrAddr == \'\' || sCity == \'\' || isNaN(iPostal) || iPostal == \'\' )").Append(Environment.NewLine);
                //sbJS.Append("   {").Append(Environment.NewLine);
                //sbJS.Append("       alert(\'").Append(GetMessage("js err invalid address values")).Append("\');").Append(Environment.NewLine);
                //sbJS.Append("       return false; ").Append(Environment.NewLine);
                //sbJS.Append("   }").Append(Environment.NewLine);
                //sbJS.Append("   else if( !isValid(iPhone) )").Append(Environment.NewLine);
                //sbJS.Append("   {").Append(Environment.NewLine);
                //sbJS.Append("       alert(\'").Append(GetMessage("js err invalid phone values")).Append("\');").Append(Environment.NewLine);
                //sbJS.Append("       return false; ").Append(Environment.NewLine);
                //sbJS.Append("   }").Append(Environment.NewLine);
                //sbJS.Append("   else").Append(Environment.NewLine);
                //sbJS.Append("   {").Append(Environment.NewLine);
                //sbJS.Append("       resetCPostback();").Append(Environment.NewLine);
                //sbJS.Append("       document.forms[0].submit(); ").Append(Environment.NewLine);
                //sbJS.Append("   }").Append(Environment.NewLine);
                //sbJS.Append("} ").Append(Environment.NewLine);

                sbJS.Append("</script>").Append(Environment.NewLine);

                ltr_js.Text = sbJS.ToString();
                break;

            default:

                //sbJS.Append(JSLibrary.ToggleDiv())

                sbJS.Append("function ToggleDiv(sDiv, overrd) {" + Environment.NewLine);
                sbJS.Append("   var objcustom = document.getElementById(sDiv); " + Environment.NewLine);
                sbJS.Append("   var bOverRide = (overrd != null); " + Environment.NewLine);
                sbJS.Append("   if ((bOverRide && overrd) || (!bOverRide && objcustom.style.visibility == \'hidden\')) { " + Environment.NewLine);
                // sbJS.Append("       objcustom.style.position = ''; " & Environment.NewLine)
                sbJS.Append("       objcustom.style.visibility = \'visible\';" + Environment.NewLine);
                sbJS.Append("   } else { " + Environment.NewLine);
                // sbJS.Append("       objcustom.style.position = 'absolute'; " & Environment.NewLine)
                sbJS.Append("       objcustom.style.visibility = \'hidden\';" + Environment.NewLine);
                sbJS.Append("   } " + Environment.NewLine);
                sbJS.Append("}" + Environment.NewLine);

                sbJS.Append("function CustomReport() {" + Environment.NewLine);
                sbJS.Append("   var sSites = \'\'; " + Environment.NewLine);
                sbJS.Append("   var LoopInt = 0; " + Environment.NewLine);
                sbJS.Append("   while ( document.getElementById(\'chk_site_\' + LoopInt) != null ) { " + Environment.NewLine);
                sbJS.Append("       if ( document.getElementById(\'chk_site_\' + LoopInt).checked ) { " + Environment.NewLine);
                sbJS.Append("           if (sSites == \'\') { sSites = document.getElementById(\'chk_site_\' + LoopInt).value; } " + Environment.NewLine);
                sbJS.Append("           else { sSites = sSites + \',\' + document.getElementById(\'chk_site_\' + LoopInt).value; } " + Environment.NewLine);
                sbJS.Append("       } " + Environment.NewLine);
                sbJS.Append("       LoopInt = LoopInt + 1;" + Environment.NewLine);
                sbJS.Append("   } " + Environment.NewLine);
                sbJS.Append("   window.location = \'fulfillment.aspx?action=custom&sites=\' + sSites; " + Environment.NewLine);
                sbJS.Append("}" + Environment.NewLine);

                sbJS.Append("function CancelOrder() {" + Environment.NewLine);
                sbJS.Append("   if (confirm(\'").Append(GetMessage("js confirm cancel order")).Append("\')) " + Environment.NewLine);
                sbJS.Append("   { " + Environment.NewLine);
                sbJS.Append("       document.getElementById(\'hdn_code\').value = 4; " + Environment.NewLine);
                sbJS.Append("       document.forms[0].submit(); " + Environment.NewLine);
                sbJS.Append("   }; " + Environment.NewLine);
                sbJS.Append("}" + Environment.NewLine);

                sbJS.Append("function CallOrderEvent(orderEvent) {" + Environment.NewLine);
                sbJS.Append("   if (confirm(\'").Append(GetMessage("js confirm call order event")).Append("\')) " + Environment.NewLine);
                sbJS.Append("   { " + Environment.NewLine);
                sbJS.Append("       ektb_show(\'\',\'#EkTB_inline?height=18&width=500&inlineId=dvHoldMessage&modal=true\', null, \'\', true); " + Environment.NewLine);
                sbJS.Append("       document.getElementById(\'hdn_event\').value = orderEvent; " + Environment.NewLine);
                sbJS.Append("       document.getElementById(\'hdn_code\').value = 5; " + Environment.NewLine);
                sbJS.Append("       document.forms[0].submit(); " + Environment.NewLine);
                sbJS.Append("   }; " + Environment.NewLine);
                sbJS.Append("}" + Environment.NewLine);

                sbJS.Append("</script>").Append(Environment.NewLine);

                ltr_js.Text = sbJS.ToString();
                break;

        }

    }

    protected void Util_CheckAccess()
    {
        bCommerceAdmin = this.m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin);
        try
        {
            if (!bCommerceAdmin)
            {
                throw (new Exception(GetMessage("err not role commerce-admin")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }

    protected void Util_ShowSites()
    {

        Literal literalReference = null;
        System.Web.UI.Control header = dg_orders.Controls[0].Controls[0];

        if (header.FindControl("ltr_sites") != null)
        {
            literalReference = (Literal)header.FindControl("ltr_sites");
        }

        for (int index = 0; index <= (SiteList.Count - 1); index++)
        {

            literalReference.Text += "<tr><td><input type=\"checkbox\" checked=\"checked\" id=\"chk_site_" + index.ToString() + "\" name=\"chk_site_" + index.ToString() + "\" value=\"" + EkFunctions.HtmlEncode(System.Convert.ToString(SiteList[index])) + "\" />" + SiteList[index] + "</td></tr>";

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

    protected string Util_ShowConfig(OrderKitConfigData config)
    {

        StringBuilder sbKit = new StringBuilder();

        if (config.OrderItemId > 0 && config.KitOptions.Count > 0)
        {

            for (int i = 0; i <= (config.KitOptions.Count - 1); i++)
            {

                sbKit.Append("<br />&#160;&#160;&#160;").Append(config.KitOptions[i].GroupName).Append(": ").Append(config.KitOptions[i].GroupOptionName);

            }

        }

        return sbKit.ToString();

    }

    protected string Util_ShowCouponInfo(EkEnumeration.CouponDiscountType discountType, decimal discountValue, long currencyId)
    {

        if (discountType == EkEnumeration.CouponDiscountType.Percent)
        {

            return Strings.Format(discountValue, "0.00") + " %";

        }
        else
        {

            return Util_ShowPrice(discountValue, currencyId);

        }

    }

    protected string Util_ShowPrice(decimal price)
    {

        return order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(price, order.Currency.CultureCode);

    }

    protected string Util_ShowPrice(decimal price, long currencyId)
    {

        if (currencyId == order.Currency.Id)
        {

            return order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(price, order.Currency.CultureCode);

        }
        else
        {

            return order.Currency.AlphaIsoCode + EkFunctions.FormatCurrency(price, (string)((new CurrencyApi()).GetItem(Convert.ToInt32(currencyId)).CultureCode));

        }

    }

    protected string Util_ShowIcon(DateTime authorized, DateTime captured)
    {

        if (captured == DateTime.MinValue || captured == DateTime.MaxValue)
        {

            return "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/calculator.png\" alt=\"" + GetMessage("lbl authorized") + "\" title=\"" + GetMessage("lbl authorized") + "\"/>";

        }
        else
        {

            return "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/calculatorApprove.png\" alt=\"" + GetMessage("lbl captured") + "\" title=\"" + GetMessage("lbl captured") + "\"/>";

        }

    }

    public void drp_address_country_ServerChange(object sender, System.EventArgs e)
    {

        try
        {

            Ektron.Cms.Common.Criteria<RegionProperty> regioncriteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
            int cCountryId = Convert.ToInt32(Request.Form["drp_address_country"]);

            Util_BindRegions(cCountryId);

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }

    protected void Util_BindRegions(int cCountryId)
    {
        System.Collections.Generic.List<RegionData> RegionList = new System.Collections.Generic.List<RegionData>();
        RegionApi m_refRegion;
        m_refRegion = new RegionApi(); //(Me.m_refContentApi.RequestInformationRef)

        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, cCountryId);
        criteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
        criteria.PagingInfo.RecordsPerPage = 10000;

        RegionList = m_refRegion.GetList(criteria);

        if ((RegionList != null) && RegionList.Count > 0)
        {

            drp_address_region.Items.Clear();
            drp_address_region.DataSource = RegionList;
            drp_address_region.DataTextField = "Name";
            drp_address_region.DataValueField = "Id";
            drp_address_region.DataBind();

        }
        else
        {

            drp_address_region.DataSource = "";
            drp_address_region.DataTextField = "Name";
            drp_address_region.DataValueField = "Id";
            drp_address_region.DataBind();

        }

        drp_address_country.SelectedValue = cCountryId.ToString();

    }

    protected int Util_FindItem(int Id, string droptype)
    {
        int iRet = 0;
        switch (droptype)
        {
            case "region":
                for (int i = 0; i <= (drp_address_region.Items.Count - 1); i++)
                {
                    if (drp_address_region.Items[i].Value == Id.ToString())
                    {
                        iRet = i;
                    }
                }
                break;
            case "country":
                for (int i = 0; i <= (drp_address_country.Items.Count - 1); i++)
                {
                    if (drp_address_country.Items[i].Value == Id.ToString())
                    {
                        iRet = i;
                    }
                }
                break;
        }
        return iRet;
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }

        switch (this.m_sPageAction)
        {
            case "mostrecent":
                Display_MostRecent();
                break;
            case "bydates":
                Display_ByDates();
                break;
            case "byproduct":
                Display_ByProduct();
                break;
            case "bycustomer":
                Display_ByCustomer();
                break;
            case "onhold":
                Display_ViewOnHold();
                break;
            default: // "viewall"
                Display_ViewAll();
                break;
        }
        isPostData.Value = "true";
    }
    private void Util_SetServerJSVariable()
    {
        ltr_captOrder.Text = GetMessage("js confirm capture order?");
        ltr_msgconfirmCaptureSettle.Text = GetMessage("js confirm capture settle payment?");
        ltr_msgconfirmSettle.Text = GetMessage("js confirm settled order?");
        ltr_editNotes.Text = GetMessage("js confirm edit notes?");
        ltr_markFraud.Text = GetMessage("js confirm mark fraud?");
        ltr_addTrackNumb.Text = GetMessage("lbl add tracking number");
        ltr_mIID.Text = m_iID.ToString();
        lbl_ok.Text = GetMessage("lbl ok");
        ltr_summary.Text = GetMessage("summary text");
        ltr_dvstatus.Text = GetMessage("generic status");
        ltr_addresses.Text = GetMessage("lbl map address");
        ltr_description.Text = GetMessage("lbl description");
        ltr_workflow.Text = GetMessage("lbl workflow");
        ltr_payment.Text = GetMessage("lbl payment");
        ltr_coupon.Text = GetMessage("lbl coupons");
    }
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSSFilFillment");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/box.css", "EktronBoxCSS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/pop_style.css", "EktronPopStyleCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
    }
    #endregion

    #region CallBack


    string callbackresult = "";

    public string GetCallbackResult()
    {

        return callbackresult;

    }

    public void RaiseCallbackEvent(string eventArgument)
    {

        Ektron.Cms.Commerce.PaymentMethods.IPayPal paypalManager = ObjectFactory.GetPayPal();
        Ektron.Cms.Commerce.PaymentMethods.PayPalResponse paypalResp = null;
        NameValueCollection _callBackData = null;

        try
        {

            if (!this.m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin))
            {

                throw (new Exception(GetMessage("err not role commerce-admin")));

            }

            _callBackData = System.Web.HttpUtility.ParseQueryString(eventArgument);

            paypalManager.InitializeFromGateway();

            paypalResp = paypalManager.GetTransactionDetails(_callBackData["transId"]);

            callbackresult += (string)(GetMessage("type label") + " " + paypalResp.ResponseFields["PAYMENTTYPE"]);
            callbackresult += (string)("<br />" + GetMessage("generic status") + ": " + paypalResp.ResponseFields["PAYMENTSTATUS"]);

        }
        catch (Exception ex)
        {

            callbackresult = "<img src=\"" + AppImgPath + "alert.gif\"><span class=\"important\">" + ex.Message + "</span>";

        }

    }


    #endregion

}

