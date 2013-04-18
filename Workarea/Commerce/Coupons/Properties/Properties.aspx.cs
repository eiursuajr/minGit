using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Ektron.Cms.Commerce;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Workarea;
using Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Properties
{
    public partial class Properties : CouponBase
    {
        #region Member Variables

        private CouponData _RequestedCoupon;
        private CouponScope _CouponScope;
        private ItemsClientData _ItemsData;
        private List<ItemsDataClientData> _IncludedItems;
        private List<ItemsDataClientData> _ExcludedItems;

        #endregion

        #region constants

        protected const string HOME_PAGE_URL = "../List/List.aspx";

        #endregion

        #region Events

        #region Page Events

        public Properties()
        {
            _RequestedCoupon = null;
            _ItemsData = new ItemsClientData();
            _IncludedItems = new List<ItemsDataClientData>();
            _ExcludedItems = new List<ItemsDataClientData>();
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            // check coupon id querystring - if exists, process. If not will show message
            long id;
            if (long.TryParse(Request.QueryString.Get("couponId"), out id))
                _RequestedCoupon = _CouponApi.GetItem(id);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //check permissions - both admin and commerce admin can edit
            mvPermissions.SetActiveView((this.IsCommerceAdmin == true || this.IsAdmin == true) ? vwValidPermissions : vwInavlidPersmissions);

            // set coupon's published values
            ucTypeControl.CouponPublishedData
                = ucAmountControl.CouponPublishedData
                = ucPercentControl.CouponPublishedData
                = ucScopeControl.CouponPublishedData
                = ucItemsControl.CouponPublishedData
                = _RequestedCoupon;

            // skip the following if in a postback (and not ms-ajax callback)
            if (this.IsPostBack && !smCouponPropeties.IsInAsyncPostBack)
                return;

            //set help link
            aHelp.HRef = "#Help";
            aHelp.Attributes.Add("onclick", "window.open('" + _ContentApi.fetchhelpLink("coupons_props") + "', 'SitePreview', 'width=800,height=500,resizable=yes,toolbar=no,scrollbars=1,location=no,directories=no,status=no,menubar=no,copyhistory=no');return false;");
            imgHelp.Src = this.GetPropertiesImagesPath() + "/help.png";

            //register page components
            this.RegisterCSS();
            this.RegisterJS();

            // select proper view and init components if data valid
            if (null == _RequestedCoupon)
            {
                if (String.IsNullOrEmpty(Request.QueryString.Get("deleted")))
                    MainView.SetActiveView(InalidCouponView);
                else
                {

                    MainView.SetActiveView(DeletedCouponView);
                    divActions.Visible = false;
                    aList.ToolTip = this.GetMessage("lbl View All Coupons");
                    aList.NavigateUrl = "../List/List.aspx";
                    aList.Text = this.GetMessage("lbl View All Coupons");

                }

                // nothing to edit
                this.IsEditable = false;
            }
            else
            {
                MainView.SetActiveView(NormalView);

                //check permissions - both admin and commerce admin can edit
                this.IsEditable = (this.IsCommerceAdmin || this.IsAdmin);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //get client data from user controls
            // gather coupon type info (from ui), and transfer into CouponData object
            Coupons.Type.ClientData.TypeClientData typeData
                = (Coupons.Type.ClientData.TypeClientData)ucTypeControl.GetClientData();


            if (typeData != null)
            {
                //set currency details for amount user control
                ucAmountControl.CurrencyId = long.Parse(typeData.CurrencyID.ToString());
                ucAmountControl.CurrencyName = typeData.CurrencyName;
                ucAmountControl.CurrencySymbol = typeData.CurrencySymbol;
                ucAmountControl.CouponTypeUserChanged = typeData.CouponTypeUserChanged;

                //set currency details for percent user control
                ucPercentControl.CurrencyName = typeData.CurrencyName;
                ucPercentControl.CurrencySymbol = typeData.CurrencySymbol;
                ucPercentControl.UnpublishedCouponType = typeData.CouponType;
                ucPercentControl.CouponTypeUserChanged = typeData.CouponTypeUserChanged;

                //set currency details for scope user control
                ucScopeControl.CurrencyId = long.Parse(typeData.CurrencyID.ToString());
                ucScopeControl.CurrencyName = typeData.CurrencyName;
                ucScopeControl.CurrencySymbol = typeData.CurrencySymbol;
				ucScopeControl.IsCouponTypeAmount = (typeData.CouponType == Type.ClientData.TypeClientData.CouponTypes.Amount ? true : false);
            }

            if (Request.QueryString["fromModal"] != null && Convert.ToBoolean(Request.QueryString["fromModal"]))
            {

                lbCancel.Attributes.Add("onclick", "closeModal(); return false;");
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //set localized strings
            this.SetLocalizedControlText();

            ManageTabs();
        }

        #endregion

        #region User Events

        protected void Save_Click(object sender, CommandEventArgs e)
        {
            SaveCoupon();
        }

        protected void Delete_Click(object sender, CommandEventArgs e)
        {
            DeleteCoupon();
        }

        protected void Cancel_Click(object sender, CommandEventArgs e)
        {
            CancelCouponActions();
        }

        protected void Exit_Click(object sender, CommandEventArgs e)
        {
            CancelCouponActions();
        }

        protected void Type_Click(object sender, CommandEventArgs e)
        {
            Tabs.SetActiveView(this.TypeTabView);
        }

        protected void Discount_Click(object sender, CommandEventArgs e)
        {
            Tabs.SetActiveView(this.DiscountTabView);
        }

        protected void Scope_Click(object sender, CommandEventArgs e)
        {
            Tabs.SetActiveView(this.ScopeTabView);
        }

        protected void Items_Click(object sender, CommandEventArgs e)
        {
            Tabs.SetActiveView(this.ItemsTabView);
        }

        #endregion

        #endregion

        #region Helpers

        protected void ManageTabs()
        {
            // Tabs are individual views within a multiview control, and
            // are activated in their corresponding onclick event handler.

            // preset all tab buttons' CSS to inactive
            this.lbType.CssClass = this.lbDiscount.CssClass
                = this.lbScope.CssClass = this.lbItems.CssClass = "InactiveTab";

            // set the CSS for the active tab
            if (null == Tabs.GetActiveView())
            {
                // ensure that a tab is selected (default to Type)
                Tabs.SetActiveView(this.TypeTabView);
                this.lbType.CssClass = "ActiveTab";
            }
            else
            {
                switch ((Tabs.GetActiveView()).ID)
                {
                    case "TypeTabView":
                        this.liType.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                        this.liDiscount.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liScope.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liItems.Attributes.Add("class", "ui-state-default ui-corner-top");
                        break;

                    case "DiscountTabView":
                        this.liType.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liDiscount.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                        this.liScope.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liItems.Attributes.Add("class", "ui-state-default ui-corner-top");
                        SelectDiscountControl();
                        break;

                    case "ScopeTabView":
                        this.liType.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liDiscount.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liScope.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                        this.liItems.Attributes.Add("class", "ui-state-default ui-corner-top");
                        SelectScopeControl();
                        break;

                    case "ItemsTabView":
                        this.liType.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liDiscount.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liScope.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liItems.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                        SelectItemsControl();
                        break;
                    default:
                        this.liType.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                        this.liDiscount.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liScope.Attributes.Add("class", "ui-state-default ui-corner-top");
                        this.liItems.Attributes.Add("class", "ui-state-default ui-corner-top");
                        break;
                }
            }
        }

        protected void SelectDiscountControl()
        {
            // select the proper discount control, based on the types' setting
            // (if editing, get the current setting from the type control, 
            // otherwise use the coupon data from the db)
            Coupons.Type.ClientData.TypeClientData typeClientData = (Coupons.Type.ClientData.TypeClientData)ucTypeControl.GetClientData();

            ucAmountControl.CurrencyId = typeClientData.CurrencyID;
            ucAmountControl.CurrencyName = typeClientData.CurrencyName;
            ucAmountControl.CurrencySymbol = typeClientData.CurrencySymbol;
            ucAmountControl.Visible
                = this.IsEditable
                    ? typeClientData.CouponType
                        == Coupons.Type.ClientData.TypeClientData.CouponTypes.Amount
                    : this._RequestedCoupon.DiscountType == EkEnumeration.CouponDiscountType.Amount;

            ucPercentControl.CurrencyName = typeClientData.CurrencyName;
            ucPercentControl.CurrencySymbol = typeClientData.CurrencySymbol;
            ucPercentControl.Visible = !ucAmountControl.Visible;



        }

        protected void SelectScopeControl()
        {
            Coupons.Type.ClientData.TypeClientData typeClientData = (Coupons.Type.ClientData.TypeClientData)ucTypeControl.GetClientData();
            ucScopeControl.CurrencyId = typeClientData.CurrencyID;
            ucScopeControl.CurrencyName = typeClientData.CurrencyName;
            ucScopeControl.CurrencySymbol = typeClientData.CurrencySymbol;
			ucScopeControl.IsCouponTypeAmount = (typeClientData.CouponType == Type.ClientData.TypeClientData.CouponTypes.Amount ? true : false);
        }

        protected void SelectItemsControl()
        {
            ucItemsControl.Scope = ((Coupons.Scope.ClientData.ScopeClientData)ucScopeControl.GetClientData()).CouponScope;
        }

        protected void SaveCoupon()
        {
            if (_RequestedCoupon != null)
            {
                // store the new values
                GetCouponTypeData();
                GetCouponDiscountData();
                GetCouponScopeData();
                this._CouponApi.Update(_RequestedCoupon);

                //update associated items - catalog, product, taxonomy
                GetItemsData();
                base.SetCouponItems(_CouponScope, _RequestedCoupon, _ItemsData);
            }

            RedirectToHomePage();
        }

        protected void GetCouponTypeData()
        {
            // gather coupon type info (from ui), and transfer into CouponData object
            Coupons.Type.ClientData.TypeClientData typeData
                = (Coupons.Type.ClientData.TypeClientData)ucTypeControl.GetClientData();

            _RequestedCoupon.Code = typeData.Code;
            _RequestedCoupon.Description = typeData.Description.Replace("<", "").Replace(">", "");
            _RequestedCoupon.CurrencyId = typeData.CurrencyID;
            _RequestedCoupon.IsActive = typeData.Enabled;

            _RequestedCoupon.DiscountType
                = (typeData.CouponType == Coupons.Type.ClientData.TypeClientData.CouponTypes.Amount)
                    ? EkEnumeration.CouponDiscountType.Amount
                    : EkEnumeration.CouponDiscountType.Percent;
        }

        protected void GetCouponDiscountData()
        {
            // gather discount info (from ui), and transfer into CouponData object
            if (this._RequestedCoupon.DiscountType == EkEnumeration.CouponDiscountType.Amount)
            {
                _RequestedCoupon.DiscountValue
                    = ((Coupons.Type.ClientData.AmountClientData)this.ucAmountControl.GetClientData()).Amount;
                _RequestedCoupon.MaximumAmount = 0;  //set maximum amount to zero since discount type is amount
            }
            else
            {
                Coupons.Type.ClientData.PercentClientData percentData
                    = (Coupons.Type.ClientData.PercentClientData)this.ucPercentControl.GetClientData();
                _RequestedCoupon.DiscountValue = percentData.Percent;
                _RequestedCoupon.MaximumAmount = percentData.MaxRedemptionAmount;
            }
        }

        protected void GetCouponScopeData()
        {
            // gather scope info (from ui), and transfer into CouponData object
            Coupons.Scope.ClientData.ScopeClientData scopeData
                = (Coupons.Scope.ClientData.ScopeClientData)this.ucScopeControl.GetClientData();

            
			_RequestedCoupon.AppliesToQuantity = scopeData.IsApplicabletoallQuantities;
			_RequestedCoupon.IsApplicabletoSubscriptions = scopeData.IsApplicabletoSubscriptions;
			_RequestedCoupon.OnePerCustomer = scopeData.IsOnePerCustomer;
            _RequestedCoupon.MaximumUses = scopeData.MaxRedemptions;
            _RequestedCoupon.MinimumAmount = scopeData.MinBasketValue;
            _RequestedCoupon.IsCombinable = scopeData.IsCombinable;
            _RequestedCoupon.StartDate = scopeData.StartDate;
            _RequestedCoupon.ExpirationDate = scopeData.ExpirationDate;

            // determine application scope (translate enumerations):
            switch (scopeData.CouponScope)
            {
                case Ektron.Cms.Commerce.Workarea.Coupons.CouponScope.AllApprovedItems:
                    _RequestedCoupon.CouponType = EkEnumeration.CouponType.AllItems;
                    break;

                case Ektron.Cms.Commerce.Workarea.Coupons.CouponScope.EntireBasket:
                    _RequestedCoupon.CouponType = EkEnumeration.CouponType.BasketLevel;
                    break;

                case Ektron.Cms.Commerce.Workarea.Coupons.CouponScope.LeastExpensiveItem:
                    _RequestedCoupon.CouponType = EkEnumeration.CouponType.LeastExpensiveItem;
                    break;

                case Ektron.Cms.Commerce.Workarea.Coupons.CouponScope.MostExpensiveItem:
                    _RequestedCoupon.CouponType = EkEnumeration.CouponType.MostExpensiveItem;
                    break;
            }

            //set coupon scope - used by SaveCoupon()
            _CouponScope = scopeData.CouponScope;
        }

        protected void GetItemsData()
        {
            // gather items info (from ui), and transfer into CouponData object
            _ItemsData = (Coupons.Scope.ClientData.ItemsClientData)this.ucItemsControl.GetClientData();
        }

        protected void DeleteCoupon()
        {
            this._CouponApi.Delete(_RequestedCoupon.Id);

            // force coupon-deleted view
            Response.Redirect(Request.RawUrl
                + (Request.RawUrl.Contains("?") ? "&" : "?")
                + "deleted=1", false);
        }

        protected void CancelCouponActions()
        {
                RedirectToHomePage();
        }

        protected void RedirectToHomePage()
        {
            Response.Redirect(HOME_PAGE_URL, false);
        }
        
        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            // initialize ui text values
            litCouponHeader.Text = GetLocalizedStringCouponProperties();
            spanSaveDisabled.InnerText = lbSave.Text = GetLocalizedStringSave();
            spanDeleteDisabled.InnerText = lbDelete.Text = GetLocalizedStringDelete();
            lbCancel.Text = GetLocalizedStringCancel();
            lbExit.Text = GetLocalizedStringExit();
            litInvalidCoupon.Text = GetLocalizedStringInvalidCoupon();
            litDeletedCoupon.Text = GetLocalizedStringCouponDeleted();
            lbType.Text = GetLocalizedStringType();
            lbDiscount.Text = GetLocalizedStringDiscount();
            lbScope.Text = GetLocalizedStringScope();
            lbItems.Text = GetLocalizedStringItems();

            // modal - delete
            litConfirmDeleteHeader.Text = GetLocalizedStringConfirmDelete();
            litConfirmDeleteMessage.Text = GetLocalizedStringDeleteMessage();

            // modal - save
            litConfirmSaveHeader.Text = GetLocalizedStringConfirmSave();
            litConfirmSaveMessage.Text = GetLocalizedStringSaveMessage();

            //help link
            aHelp.Title = this.GetMessage("generic help");
            imgHelp.Alt = this.GetMessage("generic help");
            imgHelp.Attributes.Add("title", this.GetMessage("generic help"));

            //set invalid persmissions text
            litInvalidPermissions.Text = this.GetMessage("Coupon Permissions warning");
        }

        #region Localized Table Strings

        public string GetLocalizedStringNo()
        {
            return this.GetMessage("generic no");
        }

        public string GetLocalizedStringYes()
        {
            return this.GetMessage("generic yes");
        }

        public string GetLocalizedStringIdHeader()
        {
            return this.GetMessage("generic id");
        }

        public string GetLocalizedStringCodeHeader()
        {
            return this.GetMessage("lbl code");
        }

        public string GetLocalizedStringCurrencyHeader()
        {
            return this.GetMessage("lbl currency");
        }

        public string GetLocalizedStringDescriptionHeader()
        {
            return this.GetMessage("lbl coupon desc");
        }

        public string GetLocalizedStringInvalidCoupon()
        {
            return this.GetMessage("lbl Invalid Coupon");
        }

        public string GetLocalizedStringCouponDeleted()
        {
            return this.GetMessage("lbl coupon deleted");
        }

        public string GetLocalizedStringType()
        {
            return this.GetMessage("generic type");
        }

        public string GetLocalizedStringDiscount()
        {
            return this.GetMessage("lbl coupon discount");
        }

        public string GetLocalizedStringScope()
        {
            return this.GetMessage("generic scope");
        }

        public string GetLocalizedStringItems()
        {
            return this.GetMessage("generic items");
        }

        public string GetLocalizedStringCouponProperties()
        {
            return this.GetMessage("lbl Coupon Properties");
        }

        public string GetLocalizedStringDelete()
        {
            return this.GetMessage("generic delete title");
        }

        public string GetLocalizedStringExit()
        {
            return this.GetMessage("generic cancel");
        }

        public string GetLocalizedStringConfirmCancel()
        {
            return this.GetMessage("lbl Confirm Cancel");
        }

        public string GetLocalizedStringConfirmDelete()
        {
            return this.GetMessage("lbl Confirm Delete");
        }

        public string GetLocalizedStringConfirmSave()
        {
            return this.GetMessage("lbl Confirm Save");
        }

        public string GetLocalizedStringCancelMessage()
        {
            return this.GetMessage("lbl skip properties");
        }

        public string GetLocalizedStringDeleteMessage()
        {
            return this.GetMessage("lbl delete coupon");
        }

        public string GetLocalizedStringSaveMessage()
        {
            return this.GetMessage("lbl save coupon property");
        }

        #endregion

        #endregion

        #region JS, CSS

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/Properties/css/properties.css", @"EktronCommerceCouponPropertiesCss");
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/Properties/css/properties.ie.css", @"EktronCommerceCouponPropertiesIeCss", Css.BrowserTarget.AllIE);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, this.ApplicationPath + @"/Commerce/Coupons/Properties/js/Properties.js", @"EktronCommerceCouponPropertiesJs");
        }

        private string GetPropertiesImagesPath()
        {
            return this.ApplicationPath + @"/Commerce/Coupons/Properties/css/images";
        }

        #endregion
    }
}