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
using System.Collections.Specialized;
using System.Collections.Generic;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons;
using Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData;
using Ektron.Cms.Common;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Scope
{
    public partial class Scope : CouponUserControlBase, ICouponUserControl
    {
        #region Member Variables

        private string _CurrencyName;
        private string _CurrencySymbol;
        private long _CurrencyId;
        private DateTime _MinDate;
        private EkMessageHelper m_refMsg;
		private bool _coupondiscounttype;

        #endregion

        #region Properties

        public long CurrencyId
        {
            get
            {
                return _CurrencyId;
            }
            set
            {
                _CurrencyId = value;
            }
        }
        public string CurrencyName
        {
            get
            {
                return _CurrencyName;
            }
            set
            {
                _CurrencyName = value;
            }
        }
        public string CurrencySymbol
        {
            get
            {
                return _CurrencySymbol;
            }
            set
            {
                _CurrencySymbol = value;
            }
        }
		
		public bool IsCouponTypeAmount
        {
            get
            {
                return _coupondiscounttype;
            }
            set
            {
                _coupondiscounttype = value;
            }       
        }

        #endregion

        #region Events

        #region Page Events

        public Scope()
        {
            // prevent sql exceptions by placing a reasonable minimum date as 1/1/2000 12:00:00 AM
            _MinDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // init private variables
            SiteAPI refSiteApi = new SiteAPI();
            m_refMsg = refSiteApi.EkMsgRef;

            //set mode
            mvApplication.SetActiveView(this.IsEditable == true ? vwEditApplication : vwViewApplication);
            mvCustomerLimit.SetActiveView(this.IsEditable == true ? vwEditCustomerLimit : vwViewCustomerLimit);
            mvCombination.SetActiveView(this.IsEditable == true ? vwEditCombination : vwViewCombination);
            mvApplicabletosubcriptions.SetActiveView(this.IsEditable == true ? vwEditApplicabletosubcriptions : vwViewApplicabletosubcriptions);
            mvApplicabletoallquantities.SetActiveView(this.IsEditable == true ? vwEditApplicabletoallquantities : vwViewApplicabletoallquantities);
            vwMaxRedemptions.SetActiveView(this.IsEditable == true ? vwEditMaxRedemptions : vwViewMaxRedemptions);
            vwMinRequiredValue.SetActiveView(this.IsEditable == true ? vwEditMinRequiredValue : vwViewMinRequiredValue);
            phViewStartDate.Visible = this.IsEditable == false ? true : false;
            phEditStartDate.Visible = this.IsEditable == true ? true : false;
            phViewEndDate.Visible = this.IsEditable == false ? true : false;
            phEditEndDate.Visible = this.IsEditable == true ? true : false;


            if (this.IsEditable == false)
            {
                //set values if view mode
                this.SetViewValues();
            }
            else
            {
                //set modal to appear if edit mode
                phModal.Visible = true;
            }

            //register page components
            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
		    //setting the quantities view
            mvApplicabletoallquantities.Visible = (IsCouponTypeAmount == true ? true : false);
			
            // skip the following initialization code if already done
            if (IsInitialized(this.UniqueID))
                return;

            //set localized strings
            this.SetLocalizedControlText();

            //set published values
            this.SetPublishedValues();

            SetInitialized(this.UniqueID);
        }

        #endregion

        #endregion

        #region Helpers

        private void SetPublishedValues()
        {
            if (this.CouponPublishedData == null)
            {
                //NOTE: this is the add use-case

                //set entire basket as default
                rbEntireBasket.Checked = true;

                //get date mask
                string dateMask = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

                //set default start date
                hdnTodaysDate.Value = String.Format("{0:" + dateMask + "}", DateTime.Now).ToString();
                hdnStartDate.Value = hdnTodaysDate.Value;
                hdnStartTime.Value = "12:00 AM";

                //set default end date
                DateTime maxYearDate = DateTime.Now.AddYears(10);
                hdnEndDate.Value = maxYearDate.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
                hdnEndTime.Value = "12:00 AM";
            }
            else
            {
                // create whole and fractional monetary values
                decimal wholeValue = Decimal.Truncate(this.CouponPublishedData.MinimumAmount);
                string fractionalValue = String.Format("{0:D2}", (int)((this.CouponPublishedData.MinimumAmount - wholeValue) * 100));

                // set view-only fields
                switch (this.CouponPublishedData.CouponType)
                {
                    case EkEnumeration.CouponType.AllItems:
                        litViewApplicationValue.Text = GetLocalizedStringAllItems();
                        break;

                    case EkEnumeration.CouponType.BasketLevel:
                        litViewApplicationValue.Text = GetLocalizedStringBasketLevel();
                        break;

                    case EkEnumeration.CouponType.LeastExpensiveItem:
                        litViewApplicationValue.Text = GetLocalizedStringLeastExpensiveItem();
                        break;

                    case EkEnumeration.CouponType.MostExpensiveItem:
                        litViewApplicationValue.Text = GetLocalizedStringMostExpensiveItem();
                        break;
                }
                litViewCustomerLimitValue.Text = this.CouponPublishedData.OnePerCustomer ? GetLocalizedStringYes() : GetLocalizedStringNo();
                litViewCombinationValue.Text = this.CouponPublishedData.IsCombinable ? GetLocalizedStringYes() : GetLocalizedStringNo();
                litviewApplicabletosubcriptionsValue.Text = this.CouponPublishedData.IsApplicabletoSubscriptions ? GetLocalizedStringYes() : GetLocalizedStringNo();
                litviewApplicabletoallquantitiesValue.Text = this.CouponPublishedData.AppliesToQuantity ? GetLocalizedStringYes() : GetLocalizedStringNo();
                litViewMaxRedemptionsValue.Text = this.CouponPublishedData.MaximumUses.ToString();
                litViewMinRequiredValueValue.Text = wholeValue.ToString() + "." + fractionalValue;
                litViewStartDateValue.Text = this.CouponPublishedData.StartDate.ToString();
                litViewEndDateValue.Text = this.CouponPublishedData.ExpirationDate.ToString();

                // set editable fields
                rbEntireBasket.Checked = this.CouponPublishedData.CouponType == EkEnumeration.CouponType.BasketLevel;
                rbAllApprovedItems.Checked = this.CouponPublishedData.CouponType == EkEnumeration.CouponType.AllItems;
                rbMostExpensiveItem.Checked = this.CouponPublishedData.CouponType == EkEnumeration.CouponType.MostExpensiveItem;
                rbLeastExpensiveItem.Checked = this.CouponPublishedData.CouponType == EkEnumeration.CouponType.LeastExpensiveItem;
                cbCombination.Checked = this.CouponPublishedData.IsCombinable;
                cbApplicabletosubcriptions.Checked = this.CouponPublishedData.IsApplicabletoSubscriptions;
                cbApplicabletoallquantities.Checked = this.CouponPublishedData.AppliesToQuantity;
                cbCustomerLimit.Checked = this.CouponPublishedData.OnePerCustomer;
                txtMaxRedemptions.Text = this.CouponPublishedData.MaximumUses == 0 ? String.Empty : this.CouponPublishedData.MaximumUses.ToString();

                txtDollars.Text = wholeValue.ToString();
                txtCents.Text = fractionalValue;

                hdnTodaysDate.Value = DateTime.Parse(String.Format("{0:" + System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", DateTime.Now)).ToString();
                DateTime maxYear = DateTime.Now.AddYears(10);
                hdnStartDate.Value = this.CouponPublishedData.StartDate == _MinDate ? DateTime.Parse(String.Format("{0:" + System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", DateTime.Now)).ToString() : this.CouponPublishedData.StartDate.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
                hdnStartTime.Value = this.CouponPublishedData.StartDate == _MinDate ? "12:00 AM" : this.CouponPublishedData.StartDate.ToString("h:mm tt");
                hdnEndDate.Value = this.CouponPublishedData.ExpirationDate == _MinDate ? maxYear.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern) : this.CouponPublishedData.ExpirationDate.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
                hdnEndTime.Value = this.CouponPublishedData.ExpirationDate == _MinDate ? "12:00 AM" : this.CouponPublishedData.ExpirationDate.ToString("h:mm tt");
            }
        }

        private void SetViewValues()
        {
            litApplicationLabel.Text = this.GetApplicationLabel();
            litViewApplicationValue.Text = "";
            litViewCombinationLabel.Text = this.GetCombinationLabel();
            litViewCombinationValue.Text = "";
            litviewApplicabletosubcriptionsLabel.Text = this.GetApllicabletoSubcriptionLabel();
            litviewApplicabletosubcriptionsValue.Text = "";
            litViewCustomerLimitLabel.Text = this.GetCustomerLimitLabel();
            litViewCustomerLimitValue.Text = "";
            litViewMaxRedemptionsLabel.Text = this.GetMaxRedemptionsLabel(); ;
            litViewMaxRedemptionsValue.Text = "";
            litViewMinRequiredValueLabel.Text = this.GetMinRequiredValueLabel();
            litViewMinRequiredValueValue.Text = "";
            litViewStartDateLabel.Text = this.GetStartDateLabel();
            litViewStartDateValue.Text = "";
            litViewEndDateLabel.Text = this.GetEndDateLabel();
            litViewEndDateValue.Text = "";
        }

        public string GetControlId()
        {
            return this.UniqueID;
        }

        public string GetStartDateTimeClass()
        {
            return "disable";
        }

        public string GetEndDateTimeClass()
        {
            return "disable";
        }

        public string GetTodaysDate()
        {
            return DateTime.Parse(String.Format("{0:" + System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", DateTime.Now)).ToString();
        }

        public string GetStartClearClass()
        {
            string clearClass = String.Empty;
            if (this.CouponPublishedData != null)
            {
                if (this.CouponPublishedData.StartDate != null)
                {
                    clearClass = " hide";
                }
            }
            else
            {
                clearClass = " hide";
            }
            return clearClass;
        }

        public string GetCurrencySymbol()
        {
            return this.CurrencySymbol;
        }

        public string GetEndClearClass()
        {
            string clearClass = String.Empty;
            if (this.CouponPublishedData != null)
            {
                if (this.CouponPublishedData.ExpirationDate != null)
                {
                    clearClass = " hide";
                }
            }
            else
            {
                clearClass = " hide";
            }
            return clearClass;
        }

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            ScopeClientData scopeClientData = new ScopeClientData();

            //Apply coupon to...
            EkEnumeration.CouponType publishedCouponType = this.CouponPublishedData != null ? this.CouponPublishedData.CouponType : EkEnumeration.CouponType.AllItems;
            CouponScope couponScope = this.ConvertToCouponScope(publishedCouponType);
            couponScope = rbEntireBasket.Checked == true ? CouponScope.EntireBasket : couponScope;
            couponScope = rbAllApprovedItems.Checked == true ? CouponScope.AllApprovedItems : couponScope;
            couponScope = rbMostExpensiveItem.Checked == true ? CouponScope.MostExpensiveItem : couponScope;
            couponScope = rbLeastExpensiveItem.Checked == true ? CouponScope.LeastExpensiveItem : couponScope;
            scopeClientData.CouponScope = couponScope;

            //One per customer
            bool publishedIsCombinable = this.CouponPublishedData != null ? this.CouponPublishedData.IsCombinable : false;
            scopeClientData.IsCombinable = hdnInitialized.Value == "true" ? cbCombination.Checked : publishedIsCombinable;

            //Applicable to Subscriptions
            bool IsapplicabletoSubscriptions = this.CouponPublishedData != null ? this.CouponPublishedData.IsApplicabletoSubscriptions : false;
            scopeClientData.IsApplicabletoSubscriptions = hdnInitialized.Value == "true" ? cbApplicabletosubcriptions.Checked : IsapplicabletoSubscriptions;

            //Applicable to all quantities
            bool Isapplicabletoallquantities = this.CouponPublishedData != null ? this.CouponPublishedData.AppliesToQuantity : false;
            scopeClientData.IsApplicabletoallQuantities = hdnInitialized.Value == "true" ? cbApplicabletoallquantities.Checked : Isapplicabletoallquantities;
            
            
            //Can be combined with other coupons...
            bool publishedIsOnePerCustomer = this.CouponPublishedData != null ? this.CouponPublishedData.OnePerCustomer : false;
            scopeClientData.IsOnePerCustomer = hdnInitialized.Value == "true" ? cbCustomerLimit.Checked : publishedIsOnePerCustomer;

            //Maximum redemptions
            int publishedMaxRedemptions = this.CouponPublishedData != null ? this.CouponPublishedData.MaximumUses : 0;
            int maxRedemptions = txtMaxRedemptions.Text != String.Empty ? Convert.ToInt32(txtMaxRedemptions.Text) : publishedMaxRedemptions;
            scopeClientData.MaxRedemptions = maxRedemptions;

            //minimum required cart value
            decimal publishedMinBasketValue = this.CouponPublishedData != null ? this.CouponPublishedData.MinimumAmount : 0;
            string dollars = txtDollars.Text == String.Empty ? "0" : txtDollars.Text;
            string cents = txtCents.Text == String.Empty ? "00" : txtCents.Text;
            decimal minBasketValue = Decimal.Parse(dollars + "." + cents);
            if (txtDollars.Text == String.Empty && txtCents.Text == String.Empty)
            {
                minBasketValue = publishedMinBasketValue;
            }
            scopeClientData.MinBasketValue = minBasketValue;

            //start date
            DateTime defaultStartDate = DateTime.Parse(String.Format("{0:" + System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", DateTime.Now).ToString() + " " + "12:00 AM");
            DateTime publishedStartDate = this.CouponPublishedData != null ? this.CouponPublishedData.StartDate : defaultStartDate;
            string startDate = hdnStartDate.Value + " " + hdnStartTime.Value;
            scopeClientData.StartDate = startDate.Trim() == String.Empty ? publishedStartDate : DateTime.Parse(startDate);

            //end date
            DateTime maxYear = DateTime.Now.AddYears(10);
            string val = maxYear.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern) + " " + "12:00 AM";
            string[] dateTimeFormat = {System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern};
           
            DateTime myDate;
            DateTime.TryParseExact(val, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out myDate);
            DateTime defaultEndDate = myDate;
            DateTime publishedEndDate = this.CouponPublishedData != null ? this.CouponPublishedData.ExpirationDate : defaultEndDate;
            string endDate = hdnEndDate.Value + " " + hdnEndTime.Value;
            scopeClientData.ExpirationDate = endDate.Trim() == String.Empty ? publishedEndDate : DateTime.Parse(endDate);

            //
            // prevent sql exceptions by placing a minimum date as 1/1/2000 
            // note: actual sql minumum is 1753, while DateTime.MinValue is 1/1/0001 12:00:00 AM
            //
            if (scopeClientData.StartDate.CompareTo(_MinDate) < 0)
                scopeClientData.StartDate = new DateTime(_MinDate.Ticks);
            if (scopeClientData.ExpirationDate.CompareTo(_MinDate) < 0)
                scopeClientData.ExpirationDate = new DateTime(_MinDate.Ticks);

            return scopeClientData;
        }

        private CouponScope ConvertToCouponScope(EkEnumeration.CouponType couponType)
        {
            CouponScope couponScope = CouponScope.EntireBasket;
            switch (couponType)
            {
                case EkEnumeration.CouponType.AllItems:
                    couponScope = CouponScope.AllApprovedItems;
                    break;
                case EkEnumeration.CouponType.BasketLevel:
                    couponScope = CouponScope.EntireBasket;
                    break;
                case EkEnumeration.CouponType.MostExpensiveItem:
                    couponScope = CouponScope.MostExpensiveItem;
                    break;
                case EkEnumeration.CouponType.LeastExpensiveItem:
                    couponScope = CouponScope.LeastExpensiveItem;
                    break;
            }
            return couponScope;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            //javascript localized strings
            string invalidStartEndDateMessage = m_refMsg.GetMessage("err coupon start date"); // @"Error!  Start date may not follow end date!";
            hdnScopeLocalizedJavascriptStrings.Value = @"{
                        ""invalidStartEndDateMessage"": """ + invalidStartEndDateMessage + @"""
                    }";

            //header
            litCouponScopeHeader.Text = m_refMsg.GetMessage("lbl coupon scope");  // "Coupon Scope";
            //application
            litApplicationLabel.Text = this.GetApplicationLabel();
            rbEntireBasket.Text = m_refMsg.GetMessage("lbl coupon entire cart"); // "Entire Shopping Cart";
            rbAllApprovedItems.Text = m_refMsg.GetMessage("lbl coupon all items"); // "All Approved Items";
            rbMostExpensiveItem.Text = m_refMsg.GetMessage("lbl coupon most expensive approved item"); // "Most Expensive Approved Item";
            rbLeastExpensiveItem.Text = m_refMsg.GetMessage("lbl coupon least expensive approved item"); // "Least Expensive Approved Item";
            //customer limit
            lblCustomerLimit.Text = this.GetCustomerLimitLabel();
            //combination
            lblCombination.Text = this.GetCombinationLabel();
            // Applicable to Subscriptions
            lblApplicabletosubcriptions.Text = this.GetApllicabletoSubcriptionLabel();

            //Applicable to Quantities
            lblApplicabletoallquantities.Text = this.GetApplicabletoallQuantities();

            //max redemptions
            lblMaxRedemptionsLabel.Text = this.GetMaxRedemptionsLabel();
            //minimum required basked value label
            lblMinRequiredValue.Text = this.GetMinRequiredValueLabel();
            //start date label
            lblStartDate.Text = this.GetStartDateLabel();
            //end date label
            lblEndDate.Text = this.GetEndDateLabel();

            //set date format
            hdnDateFormat.Value = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

            //modal header
            litModalHeaderLabel.Text = m_refMsg.GetMessage("lbl coupon select date"); //"Select Date";
        }

        private string GetApplicationLabel()
        {
            return m_refMsg.GetMessage("lbl coupon applyto");   // "Apply coupon to...";
        }
        private string GetCustomerLimitLabel()
        {
            return m_refMsg.GetMessage("lbl coupon one per customer");     // "One per customer";
        }
        private string GetCombinationLabel()
        {
            return m_refMsg.GetMessage("lbl coupon combineable");     // "Can be combined with other coupons";
        }
        private string GetApplicabletoallQuantities()
        {
            return m_refMsg.GetMessage("lbl coupon isapplicabletoallquantities");//"can be applied to allquantities";
        }
        private string GetApllicabletoSubcriptionLabel()             
        {
            return m_refMsg.GetMessage("lbl coupon isapplicabletosubscriptions"); // "Can be applied to subscriptions";
        }
        private string GetMaxRedemptionsLabel()
        {
            return m_refMsg.GetMessage("lbl coupon max redemptions");     // "Maximum redemptions";
        }
        private string GetMinRequiredValueLabel()
        {
            return m_refMsg.GetMessage("lbl coupon min cart");     // "Minimum required cart value";
        }
        private string GetStartDateLabel()
        {
            return m_refMsg.GetMessage("lbl coupon start date");     // "Start Date";
        }
        private string GetEndDateLabel()
        {
            return m_refMsg.GetMessage("lbl coupon end date");     // "End Date";
        }
        public string GetLocalizedStringClear()
        {
            return m_refMsg.GetMessage("lbl coupon clear");     // "clear";
        }
        public string GetLocalizedStringModalClose()
        {
            return m_refMsg.GetMessage("lbl coupon closewindow");     // "Close Window";
        }
        public string GetSetStartDateTimeLabel()
        {
            return m_refMsg.GetMessage("lbl coupon set starttime");     // "Set Start Date/Time";
        }
        public string GetClearStartDateTimeLabel()
        {
            return m_refMsg.GetMessage("lbl coupon clear starttime");     // "Clear Start Date/Time";
        }
        public string GetSetEndDateTimeLabel()
        {
            return m_refMsg.GetMessage("lbl coupon set endtime");     // "Set End Date/Time";
        }
        public string GetClearEndDateTimeLabel()
        {
            return m_refMsg.GetMessage("lbl coupon clear endtime");     // "Clear End Date/Time";
        }
        public string GetCurrencyName()
        {
            return this.CurrencyName;
        }
        public string GetLocalizedStringAllItems()
        {
            return m_refMsg.GetMessage("lbl coupon allitems");     // "All Approved Items";
        }
        public string GetLocalizedStringBasketLevel()
        {
            return m_refMsg.GetMessage("lbl coupon entire basket");     // "Entire Basket";
        }
        public string GetLocalizedStringLeastExpensiveItem()
        {
            return m_refMsg.GetMessage("lbl coupon least expensive item");     // "Least Expensive Item";
        }
        public string GetLocalizedStringMostExpensiveItem()
        {
            return m_refMsg.GetMessage("lbl coupon most expensive item");     // "Most Expensive Item";
        }
        public string GetLocalizedStringNo()
        {
            return m_refMsg.GetMessage("lbl coupon no");     // "No";
        }
        public string GetLocalizedStringYes()
        {
            return m_refMsg.GetMessage("lbl coupon yes");     // "Yes";
        }

        #endregion

        #region JS, CSS, Images

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/css/scope.css", "EktronCommerceCouponScopeCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/css/scope.ie.css", "EktronCommerceCouponScopeIeCss", Css.BrowserTarget.AllIE);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            Ektron.Cms.Framework.UI.Packages.jQuery.jQueryUI.Datepicker.Register(this);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + "/java/ektron.dateformat.js", "EktronDateFormatJs");
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/js/scope.js", "EktronCommerceCouponsScopeJs");
        }

        public string GetScopeImagesPath()
        {
            return this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/css/images";
        }

        #endregion
    }
}