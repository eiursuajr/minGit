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
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
public partial class Commerce_customers : workareabase
{

    protected string m_sPageName = "customers.aspx";
    protected long m_iCustomerId = 0;
    protected AddressApi AddressManager = null;
    protected CustomerApi CustomerManager = null;
    protected RegionApi RegionManager = null;
    protected CountryApi CountryManager = null;
    protected CustomerData cCustomer = null;
    protected CurrencyData defaultCurrency = null;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected BasketApi basketApi = new BasketApi();
    protected string AppPath = "";

    protected void Page_Init(object sender, System.EventArgs e)
    {
        ChangeHeaderText(dg_customers);
    }
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);

        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        RegisterResources();
        AppPath = m_refContentApi.ApplicationPath;

        try
        {

            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            CommerceLibrary.CheckCommerceAdminAccess();

            System.Web.HttpCookie siteCookie = CommonApiBase.GetEcmCookie();
            Ektron.Cms.Commerce.CurrencyApi m_refCurrencyApi = new Ektron.Cms.Commerce.CurrencyApi();
            defaultCurrency = (new CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);

            if (siteCookie["SiteCurrency"] != defaultCurrency.Id.ToString())
            {
                defaultCurrency.Id = Convert.ToInt32(siteCookie["SiteCurrency"]);
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["customerid"]))
            {
                m_iCustomerId = Convert.ToInt64(Request.QueryString["customerid"]);
            }
            defaultCurrency = (new CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
            CustomerManager = new CustomerApi();
            AddressManager = new AddressApi();

            if (siteCookie["SiteCurrency"] != defaultCurrency.Id.ToString())
            {
                defaultCurrency.Id = Convert.ToInt32(siteCookie["SiteCurrency"]);
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id);
            }

            switch (base.m_sPageAction)
            {
                case "addeditaddress":
                    RegionManager = new RegionApi();
                    CountryManager = new CountryApi();
                    if (Page.IsPostBack)
                    {
                        if (Request.Form[isCPostData.UniqueID] == "")
                        {
                            Process_ViewAddress();
                        }
                    }
                    else
                    {
                        Display_ViewAddress(true);
                    }
                    break;
                case "viewaddress":
                    RegionManager = new RegionApi();
                    CountryManager = new CountryApi();
                    Display_ViewAddress(false);
                    break;
                case "viewbasket":
                    Display_ViewBaskets(false);
                    break;
                case "view":
                    Display_View();
                    break;
                case "deleteaddress":
                    Process_AddressDelete();
                    break;
                case "deletebasket":
                    Process_BasketDelete();
                    break;
                default: // "viewall"
                    if (Page.IsPostBack == false)
                    {
                        Display_View_All();
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

    #region Display
    private void ChangeHeaderText(DataGrid dg)
    {
        if (dg == null)
        {
            return;
        }      

        foreach (DataGridColumn col in dg.Columns)
        {
            if (col.HeaderText == "Id")
            {
                col.HeaderText = this.GetMessage("generic id");
            }
            if (col.HeaderText == "Name")
            {
                col.HeaderText = this.GetMessage("generic name");
            }
            if (col.HeaderText == "Total Orders")
            {
                col.HeaderText = this.GetMessage("lbl Total Orders");
            }
            if (col.HeaderText == "Per Order Value")
            {
                col.HeaderText = this.GetMessage("lbl per order value");
            }
        }
    }
    protected void Display_ViewAddress(bool WithEdit)
    {
        pnl_view.Visible = false;
        pnl_viewall.Visible = false;
        AddressData aAddress = null;
        RegionManager = new RegionApi();

        Ektron.Cms.Common.Criteria<RegionProperty> regioncriteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        if (!(this.m_iID > 0))
        {
            regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, drp_address_country.SelectedIndex);
        }
        regioncriteria.PagingInfo.RecordsPerPage = 1000;
        drp_address_region.DataTextField = "Name";
        drp_address_region.DataValueField = "Id";
        drp_address_region.DataSource = RegionManager.GetList(regioncriteria);
        drp_address_region.DataBind();

        Ektron.Cms.Common.Criteria<CountryProperty> addresscriteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        addresscriteria.PagingInfo.RecordsPerPage = 1000;
        drp_address_country.DataTextField = "Name";
        drp_address_country.DataValueField = "Id";
        drp_address_country.DataSource = CountryManager.GetList(addresscriteria);
        drp_address_country.DataBind();

        if (this.m_iID > 0)
        {
            cCustomer = CustomerManager.GetItem(this.m_iCustomerId);
            aAddress = AddressManager.GetItem(this.m_iID);
            regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, aAddress.Country.Id);

            drp_address_region.DataSource = RegionManager.GetList(regioncriteria);
            ltr_address_id.Text = aAddress.Id.ToString();
            txt_address_name.Text = aAddress.Name;
            txt_address_company.Text = aAddress.Company;
            txt_address_line1.Text = aAddress.AddressLine1;
            txt_address_line2.Text = aAddress.AddressLine2;
            txt_address_city.Text = aAddress.City;
            drp_address_country.SelectedIndex = FindItem(aAddress.Country.Id, "country");
            Util_BindRegions(aAddress.Country.Id);
            drp_address_region.SelectedValue = aAddress.Region.Id.ToString();
            txt_address_postal.Text = aAddress.PostalCode;
            txt_address_phone.Text = aAddress.Phone;
            chk_default_billing.Checked = aAddress.Id == cCustomer.BillingAddressId;
            chk_default_shipping.Checked = aAddress.Id == cCustomer.ShippingAddressId;
        }
        ToggleAddressFields(WithEdit);
    }

    protected void Display_ViewBaskets(bool WithEdit)
    {

        Ektron.Cms.Commerce.Basket currentBasket;
        long basketId = 0;
        CouponApi _CouponApi = new CouponApi();
        BasketCalculatorData basketCouponData;

        if ((Request.QueryString["basketid"] != null) && Request.QueryString["basketid"] != "")
        {
            basketId = Convert.ToInt64(Request.QueryString["basketid"]);
        }

        if (basketId > 0)
        {

            HttpBrowserCapabilities browser = Request.Browser;
            Ektron.Cms.Framework.Context.CmsContextService context = new Ektron.Cms.Framework.Context.CmsContextService();

            if (browser.Type.Contains("IE") && browser.MajorVersion >= 9)
            {
                // work around to prevent errors in IE9 when it destroys native JS objects
                // see http://msdn.microsoft.com/en-us/library/gg622929%28v=VS.85%29.aspx
                uxViewBasketIframe.Attributes.Add("src", "about:blank");
            }
            else
            {
                uxViewBasketIframe.Attributes.Add("src", context.SitePath + "customers.aspx?action=viewbasket&basketid=" + basketId + "&customerid=" + m_iCustomerId + "&thickox=true&height=300&width=700&modal=true&EkTB_iframe=true");
            }
            uxDialog.Title = m_refMsg.GetMessage("lbl view basket");

            currentBasket = basketApi.GetItem(basketId);
            if ((currentBasket != null) && currentBasket.Items.Count > 0)
            {
                dg_viewbasket.DataSource = currentBasket.Items;
                dg_viewbasket.DataBind();
                BasketCalculator basketCalc = new BasketCalculator(currentBasket, this.m_refContentApi.RequestInformationRef);

                basketCouponData = _CouponApi.CalculateBasketCoupons(basketId);

                if (basketCouponData.BasketCoupons.Count > 0)
                {
                    ltr_noitems.Text = "<hr/><table width=\"100%\">";
                    ltr_noitems.Text += "<tr><td align=\"right\" width=\"90%\">" + GetMessage("lbl coupon discount") + "</td><td align=\"right\">(" + Ektron.Cms.Common.EkFunctions.FormatCurrency(basketCouponData.TotalCouponDiscount, defaultCurrency.CultureCode) + ")</td></tr>";
                    ltr_noitems.Text += "<tr><td align=\"right\" width=\"90%\">" + GetMessage("lbl total") + ": </td><td align=\"right\">" + Ektron.Cms.Common.EkFunctions.FormatCurrency(basketCouponData.BasketTotal, defaultCurrency.CultureCode) + "</td></tr>";
                    ltr_noitems.Text += "</table>";
                }
            }
            else
            {
                ltr_noitems.Text = this.GetMessage("lbl no items");
            }
        }
        else
        {
            ltr_noitems.Text = this.GetMessage("lbl no items");
        }

    }
    protected string showconfig(KitConfigData kitconfig)
    {
        if (kitconfig != null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table>").Append(Environment.NewLine);
            for (int i = 0; i <= kitconfig.Groups.Count - 1; i++)
            {
                if (kitconfig.Groups[i].Options[0].Name != "")
                {

                    sb.Append("<tr><td>&nbsp;&nbsp;&nbsp;<label id=\"kit" + i + "\">" + kitconfig.Groups[i].Name + "</label>").Append(Environment.NewLine);
                    sb.Append("<label id=\"lbl_colon\">:&nbsp;</label><label id=\"lbl_desc" + i + "\">" + kitconfig.Groups[i].Options[0].Name + "</label></td></tr>").Append(Environment.NewLine);
                    sb.Append("</td></tr>").Append(Environment.NewLine);

                }
            }
            sb.Append("</table>").Append(Environment.NewLine);
            return sb.ToString();
        }
        else
        {
            return System.String.Empty;
        }
    }

    protected string showvariant(BasketVariantData variantconfig)
    {

        if ((variantconfig != null) && variantconfig.Id > 0)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("<table>").Append(Environment.NewLine);
            sb.Append("<tr><td>&nbsp;&nbsp;&nbsp;<label id=\"variant\">" + variantconfig.Title + ":</label>" + variantconfig.Sku + "</td></tr>").Append(Environment.NewLine);
            sb.Append("</table>").Append(Environment.NewLine);

            return sb.ToString();

        }
        else
        {

            return System.String.Empty;

        }

    }

    protected void Display_View()
    {
        pnl_viewall.Visible = false;
        List<OrderData> orderList = new List<OrderData>();
        List<AddressData> aAddreses = new List<AddressData>();
        List<Ektron.Cms.Commerce.Basket> basketList;

        OrderApi orderApi = new OrderApi();
        BasketApi basketApi = new BasketApi();
        // customer
        cCustomer = CustomerManager.GetItem(this.m_iID);
        m_iCustomerId = cCustomer.Id;
        this.ltr_id.Text = cCustomer.Id.ToString();
        this.ltr_uname.Text = cCustomer.UserName;
        this.ltr_fname.Text = cCustomer.FirstName;
        this.ltr_lname.Text = cCustomer.LastName;

        this.ltr_dname.Text = cCustomer.DisplayName;
        this.ltr_ordertotal.Text = cCustomer.TotalOrders.ToString();
        this.ltr_orderval.Text = defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(cCustomer.TotalOrderValue, defaultCurrency.CultureCode);
        this.ltr_pervalue.Text = defaultCurrency.ISOCurrencySymbol + EkFunctions.FormatCurrency(cCustomer.AverageOrderValue, defaultCurrency.CultureCode);
        // customer
        // orders
        Criteria<OrderProperty> orderCriteria = new Criteria<OrderProperty>();
        orderCriteria.AddFilter(OrderProperty.CustomerId, CriteriaFilterOperator.EqualTo, m_iID);
        orderList = orderApi.GetList(orderCriteria);
        if (orderList.Count == 0)
        {
            ltr_orders.Text = this.GetMessage("lbl no orders");
        }
        dg_orders.DataSource = orderList;
        dg_orders.DataBind();
        // orders
        // addresses
        aAddreses = AddressManager.GetList(m_iID);
        if (aAddreses.Count == 0)
        {
            ltr_address.Text = this.GetMessage("lbl no addresses");
        }
        dg_address.DataSource = aAddreses;
        dg_address.DataBind();
        // addresses
        // baskets
        if (this.m_iID > 0)
        {
            basketList = basketApi.GetList(this.m_iID);
            if (basketList.Count == 0)
            {
                ltr_baskets.Text = this.GetMessage("lbl no baskets");
            }
            dg_baskets.DataSource = basketList;
            dg_baskets.DataBind();
        }
    }

    protected void Display_View_All()
    {
        System.Collections.Generic.List<CustomerData> aCustomers = new System.Collections.Generic.List<CustomerData>();
        Ektron.Cms.Common.Criteria<CustomerProperty> cCriteria = new Ektron.Cms.Common.Criteria<CustomerProperty>();
        cCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        cCriteria.PagingInfo.CurrentPage = _currentPageNumber;
        // aCustomers = Customer.GetAllCustomers(1, 1, 1, 1, Me.m_refContentApi.RequestInformationRef)

        aCustomers = CustomerManager.GetList(cCriteria);
        TotalPagesNumber = System.Convert.ToInt32(cCriteria.PagingInfo.TotalPages);

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

            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(TotalPagesNumber))).ToString();
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
        dg_customers.DataSource = aCustomers;
        dg_customers.DataBind();
    }
    #endregion

    #region Process

    protected void Process_ViewAddress()
    {
        AddressData aAddress = null;
        long originalAddressId = this.m_iID;

        //need to get customer before address update to see if default addresses have changed.
        cCustomer = CustomerManager.GetItem(m_iCustomerId);
        aAddress = this.m_iID > 0 ? (AddressManager.GetItem(this.m_iID)) : (new AddressData());

        aAddress.Name = (string)txt_address_name.Text;
        aAddress.Company = (string)txt_address_company.Text;
        aAddress.AddressLine1 = (string)txt_address_line1.Text;
        aAddress.AddressLine2 = (string)txt_address_line2.Text;
        aAddress.City = (string)txt_address_city.Text;
        RegionData rData = new RegionData();
        rData.Id = Convert.ToInt64(drp_address_region.SelectedValue);
        aAddress.Region = rData;
        aAddress.PostalCode = (string)txt_address_postal.Text;
        CountryData cData = new CountryData();
        cData.Id = System.Convert.ToInt32(drp_address_country.SelectedValue);
        aAddress.Country = cData;
        aAddress.Phone = (string)txt_address_phone.Text;

        if (this.m_iID > 0)
        {
            AddressManager.Update(aAddress);
        }
        else
        {
            AddressManager.Add(aAddress, m_iCustomerId);
        }

        this.m_iID = aAddress.Id;

        bool updateBilling = false;
        bool updateShipping = false;

        if (chk_default_billing.Checked)
        {
            cCustomer.BillingAddressId = aAddress.Id;
            updateBilling = true;
        }

        if (chk_default_shipping.Checked)
        {
            cCustomer.ShippingAddressId = aAddress.Id;
            updateShipping = true;
        }

        //if the default addresses have been unchecked - need to reset them to 0.
        if (!chk_default_billing.Checked && cCustomer.BillingAddressId == originalAddressId)
        {
            cCustomer.BillingAddressId = 0;
            updateBilling = true;
        }

        if (!chk_default_shipping.Checked && cCustomer.ShippingAddressId == originalAddressId)
        {
            cCustomer.ShippingAddressId = 0;
            updateShipping = true;
        }

        if (updateBilling)
        {
            CustomerManager.ChangeBillingAddress(m_iCustomerId, cCustomer.BillingAddressId);
        }
        if (updateShipping)
        {
            CustomerManager.ChangeShippingAddress(m_iCustomerId, cCustomer.ShippingAddressId);
        }

        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        Response.Redirect(this.m_sPageName + "?action=viewaddress&id=" + this.m_iID.ToString() + "&customerid=" + this.m_iCustomerId.ToString() + pagemode, false);
    }
    protected void Process_AddressDelete()
    {
        AddressData aAddress = null;
        aAddress = this.m_iID > 0 ? (AddressManager.GetItem(this.m_iID)) : (new AddressData());

        if (this.m_iID > 0)
        {
            AddressManager.Delete(this.m_iID);
            Response.Redirect(this.m_sPageName, false);
        }
    }
    protected void Process_BasketDelete()
    {
        Basket bBasket = null;

        bBasket = this.m_iID > 0 ? (basketApi.GetItem(this.m_iID)) : (Basket)(new BasketData());

        if (this.m_iID > 0)
        {
            basketApi.Delete(this.m_iID);
            Response.Redirect(this.m_sPageName, false);
        }
    }
    #endregion

    protected void Util_SetLabels()
    {
        this.ltr_id_label.Text = this.GetMessage("lbl customer id");
        this.ltr_uname_lbl.Text = this.GetMessage("lbl customer username");
        this.ltr_fname_lbl.Text = this.GetMessage("lbl customer firstname");
        this.ltr_lname_lbl.Text = this.GetMessage("lbl customer lastname");
        this.ltr_dname_lbl.Text = this.GetMessage("lbl customer displayname");
        this.ltr_ordertotal_lbl.Text = this.GetMessage("lbl order total");
        this.ltr_orderval_lbl.Text = this.GetMessage("lbl order value");
        this.ltr_pervalue_lbl.Text = this.GetMessage("lbl per order value");
        this.ltr_address_id_lbl.Text = this.GetMessage("generic id");
        this.ltr_address_name.Text = this.GetMessage("lbl address name");
        this.ltr_address_company.Text = this.GetMessage("lbl address company");
        this.ltr_address_line1.Text = this.GetMessage("lbl address street");
        this.ltr_address_city_lbl.Text = this.GetMessage("lbl address city");
        this.ltr_address_region.Text = this.GetMessage("lbl address state province");
        this.ltr_address_postal.Text = this.GetMessage("lbl address postal");
        this.ltr_address_country.Text = this.GetMessage("lbl address country");
        this.ltr_address_phone.Text = this.GetMessage("lbl address phone");
        this.ltr_default_billing.Text = this.GetMessage("lbl default billing address");
        this.ltr_default_shipping.Text = this.GetMessage("lbl default shipping address");
        string pagemode = (string)("&page=" + Request.QueryString["page"]);
        switch (base.m_sPageAction)
        {
            case "addeditaddress":
                pnl_viewaddress.Visible = true;
                this.tr_address_id.Visible = this.m_iID > 0;
                this.AddBackButton(this.m_sPageName + (m_iID > 0 ? ("?action=viewaddress&id=" + this.m_iID.ToString() + "&customerid=" + this.m_iCustomerId.ToString()) : ("?action=view&id=" + this.m_iCustomerId.ToString() + pagemode + "")));
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=\"CheckAddress(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                this.SetTitleBarToString(this.GetMessage((string)(this.m_iID > 0 ? "lbl edit address" : "lbl add address")));
                this.AddHelpButton((string)(this.m_iID > 0 ? ("editaddress") : ("addaddress")));
                break;
            case "viewaddress":
                pnl_viewaddress.Visible = true;
                this.AddBackButton(this.m_sPageName + "?action=view&id=" + this.m_iCustomerId.ToString() + pagemode);
				this.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", this.m_sPageName + "?action=addeditaddress&id=" + this.m_iID + "&customerid=" + this.m_iCustomerId.ToString() + pagemode, "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
				this.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", this.m_sPageName + "?action=deleteaddress&id=" + this.m_iID.ToString() + pagemode, "alt del address button text", "btn delete", " onclick=\" return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
                this.SetTitleBarToMessage("lbl view address");
                this.AddHelpButton("viewaddress");
                break;
            case "viewbasket":
                pnl_viewbasket.Visible = true;
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", this.m_sPageName + "?action=deletebasket&id=" + Request.QueryString["basketid"].ToString(), "alt del basket button text", "btn delete", " onclick=\" if(CheckDeleteBasket()){ $ektron('" + uxDialog.Selector + "').dialog('close');  history.go(-1); return true;} else {return false; };\" ", StyleHelper.DeleteButtonCssClass, true);
                PageLabel.Visible = false;
                OfLabel.Visible = false;
                FirstPage.Visible = false;
                lnkBtnPreviousPage.Visible = false;
                NextPage.Visible = false;
                LastPage.Visible = false;
                //Me.AddBackButton(Me.m_sPageName & "?action=view&id=" & Me.m_iCustomerId.ToString())
                this.SetTitleBarToMessage("lbl view basket");
                this.AddHelpButton("viewcart");
                break;
            case "view":
                this.pnl_view.Visible = true;
                //MyBase.Tabs.On()
                //MyBase.Tabs.AddTabByMessage("properties text", "dvProp")
                //MyBase.Tabs.AddTabByMessage("lbl orders", "dvOrders")
                //Me.Tabs.AddTabByMessage("lbl addresses", "dvAddress")
                //Me.Tabs.AddTabByMessage("lbl baskets", "dvBaskets")
                StringBuilder result = new StringBuilder();

				if (Request.QueryString["page"] == "workarea")
				{
					// redirect to workarea when user clicks back button if we're in workarea
					base.AddButtonwithMessages(AppPath + "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"javascript:top.switchDesktopTab()\" ", StyleHelper.BackButtonCssClass, true);
				}
				else
				{
					this.AddBackButton(AppPath + "commerce/reporting/customerreports.aspx?page=normal");
				}

                result.Append("<script language=\"javascript\"> ");
                result.Append("var filemenu = new Menu( \"file\" ); ");
                result.Append("filemenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + AppPath + "images/ui/icons/vcard.png" + "\' />&nbsp;&nbsp;" + this.GetMessage("lbl address") + "\", function() { window.location.href = \'" + this.m_sPageName + "?action=addeditaddress&customerid=" + m_iCustomerId.ToString() + pagemode + "\'; } ); ");
                result.Append("MenuUtil.add( filemenu ); ");
                result.Append("</script>" + Environment.NewLine);
				string buttonId = Guid.NewGuid().ToString();
                result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + this.GetMessage("lbl New") + "</span></td>");
                this.AddButtonText(result.ToString());
                
                this.SetTitleBarToMessage("lbl view customer");
                this.AddHelpButton("viewcustomer");
                break;
            default: // "viewall"
                this.SetTitleBarToMessage("lbl customers");
                this.AddHelpButton("customers", false);
                break;
        }

    }

    protected void Util_SetJS()
    {

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        sbJS.Append("function CheckDelete()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    return confirm(\'").Append(GetMessage("js address confirm del")).Append("\');" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckDeleteBasket()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    return confirm(\'").Append(GetMessage("js basket confirm del")).Append("\');" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function isValid(phone)").Append(Environment.NewLine);
        sbJS.Append("{").Append(Environment.NewLine);
        sbJS.Append("   return /^(1\\s*[-\\/\\.]?)?(\\((\\d{3})\\)|(\\d{3}))\\s*([\\s-./\\\\])?([0-9]*)([\\s-./\\\\])?([0-9]*)$/.test(phone);").Append(Environment.NewLine);
        sbJS.Append("}").Append(Environment.NewLine);

        sbJS.Append("function CheckAddress() {").Append(Environment.NewLine);
        sbJS.Append("   var sAddrName = Trim(document.getElementById(\'").Append(txt_address_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sCompany = Trim(document.getElementById(\'").Append(txt_address_company.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sStrAddr = Trim(document.getElementById(\'").Append(txt_address_line1.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sStrAddr2 = Trim(document.getElementById(\'").Append(txt_address_line2.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sCity = Trim(document.getElementById(\'").Append(txt_address_city.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var iPostal = Trim(document.getElementById(\'").Append(txt_address_postal.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var iPhone = Trim(document.getElementById(\'").Append(txt_address_phone.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var drp_region = document.getElementById(\"").Append(drp_address_region.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   var drp_country = document.getElementById(\"").Append(drp_address_country.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   var drp_selected = drp_country.options[drp_country.selectedIndex].text.toLowerCase();" + Environment.NewLine);
        sbJS.Append("   if(!(drp_selected == \'canada\'))").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("    if(isNaN(iPostal))").Append(Environment.NewLine);
        sbJS.Append("     {").Append(Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("js err invalid address values")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       return false; ").Append(Environment.NewLine);
        sbJS.Append("     }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   if(sAddrName.indexOf(\'<\') > -1 ||sAddrName.indexOf(\'>\') > -1 ||sCompany.indexOf(\'>\') > -1 || sCompany.indexOf(\'<\') > -1 || sStrAddr.indexOf(\'<\') > -1 ||sStrAddr.indexOf(\'>\') > -1 || sStrAddr2.indexOf(\'<\') > -1 || sStrAddr2.indexOf(\'>\') > -1 || sCity.indexOf(\'<\') > -1 || sCity.indexOf(\'>\') > -1 ){").Append(Environment.NewLine);
        sbJS.Append("        alert(\"").Append(string.Format(GetMessage("js: alert address cant include"), "<, >")).Append("\");").Append(Environment.NewLine);
        sbJS.Append("			document.getElementById(\'").Append(txt_address_name.UniqueID).Append("\').focus(); return false;").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("   if(drp_region.selectedIndex == -1)").Append(Environment.NewLine);
        sbJS.Append("    {").Append(Environment.NewLine);
        sbJS.Append("       alert(\"" + base.GetMessage("js null postalcode region msg") + "\");" + Environment.NewLine);
        sbJS.Append("       document.forms[\"form1\"].isCPostData.value = \'false\';").Append(Environment.NewLine);
        sbJS.Append("       return false;").Append(Environment.NewLine);
        sbJS.Append("    }").Append(Environment.NewLine);
        sbJS.Append("   if(sAddrName == \'\' || sStrAddr == \'\' || sCity == \'\'  || iPostal == \'\' )").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("js err invalid address values")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       return false; ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else if( !isValid(iPhone) )").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("js err invalid phone values")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       return false; ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       resetCPostback();").Append(Environment.NewLine);
        sbJS.Append("       document.forms[0].submit(); ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("} ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text += sbJS.ToString();

    }

    protected string ShowName(string Name, string Company)
    {
        string sRet = "";
        sRet += (string)(Name != "" ? (Name + (Company != "" ? "<br />" + Company : "")) : Company);
        return sRet;
    }

    protected string ShowOptionalLine(string textValue)
    {
        return ((textValue != null && textValue.Length > 0) ? textValue + " <br /> " : "");
    }

    protected int FindItem(int Id, string droptype)
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

    protected bool Util_IsDefaultShipping(long addressId)
    {
        return (addressId == cCustomer.ShippingAddressId);
    }

    protected bool Util_IsDefaultBilling(long addressId)
    {
        return (addressId == cCustomer.BillingAddressId);
    }

    protected void ToggleAddressFields(bool Toggle)
    {
        txt_address_name.Enabled = Toggle;
        txt_address_company.Enabled = Toggle;
        txt_address_line1.Enabled = Toggle;
        txt_address_line2.Enabled = Toggle;
        txt_address_city.Enabled = Toggle;
        drp_address_region.Enabled = Toggle;
        txt_address_postal.Enabled = Toggle;
        drp_address_country.Enabled = Toggle;
        txt_address_phone.Enabled = Toggle;
        chk_default_shipping.Enabled = Toggle;
        chk_default_billing.Enabled = Toggle;
    }
    public void drp_address_country_ServerChange(object sender, System.EventArgs e)
    {
        Ektron.Cms.Common.Criteria<RegionProperty> regioncriteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        int cCountryId = Convert.ToInt32(Request.Form["drp_address_country"]);
        Util_BindRegions(cCountryId);
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
        Display_View_All();
        isPostData.Value = "true";
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/box.css", "EktronBoxCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSSCustomers");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);

    }
}


