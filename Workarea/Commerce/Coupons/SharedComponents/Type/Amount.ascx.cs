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
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce.Workarea.Coupons;
using Ektron.Cms.Commerce.Workarea.Coupons.Type.ClientData;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Type
{
    public partial class Amount : CouponUserControlBase, ICouponUserControl
    {
        #region Member Variables
        protected EkMessageHelper messageHelper;
        private string _CurrencyName;
        private string _CurrencySymbol;
        private long _CurrencyId;
        private TypeClientData.CouponTypes _UnpublishedCouponType;
        private bool _CouponTypeUserChanged;
		private decimal _discountamount = 0.0M;

        #endregion

        #region Properties

        public TypeClientData.CouponTypes UnpublishedCouponType
        {
            get { return _UnpublishedCouponType; }
            set { _UnpublishedCouponType = value; }
        }
        public bool CouponTypeUserChanged
        {
            get
            {
                return _CouponTypeUserChanged;
            }
            set
            {
                _CouponTypeUserChanged = value;
            }
        }
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

        public decimal DiscountAmount
        {
            get { return _discountamount; }
            set { _discountamount = value; }       
        }
        protected EkMessageHelper _messageHelper
        {
            get
            {
                return (messageHelper ?? (messageHelper = _ContentApi.EkMsgRef));
            }
        }
        #endregion

        #region Events

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
            //set mode to edit or view
            mvAmount.SetActiveView(this.IsEditable == true ? vwEditAmount : vwViewAmount);

            //set page components
            if (!Page.IsCallback)
            {
                this.RegisterCSS();
                this.RegisterJS();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // skip the following initialization code if already done
            if (IsInitialized(this.UniqueID))
            {
                if (this.CouponTypeUserChanged == true)
                {
                        txtLeftOfDecimal.Text = "0";
                        txtRightOfDecimal.Text = "00";
                }
                return;
            }

            //set published values
            this.SetPublishedValues();

            //set localized strings
            this.SetLocalizedControlText();

            SetInitialized(this.UniqueID);
        }

        #endregion

        #region Control/User Events
        #endregion

        #endregion
        
        #region Helpers

        private void SetPublishedValues()
        {
            //NOTE: Labels are set in region "Localized Strings" >> SetLocalizedControlText()
            if (this.CouponPublishedData != null)
            {
                if (this.CouponTypeUserChanged == false)
                {
                    decimal wholeValue = Decimal.Truncate(this.CouponPublishedData.DiscountValue);
                    string fractionalValue = String.Format("{0:D2}", (int)((this.CouponPublishedData.DiscountValue - wholeValue) * 100));

                    // set view-only fields
                    litViewAmountValue.Text = wholeValue.ToString() + "." + fractionalValue;

                    // set editable fields
                    txtLeftOfDecimal.Text = wholeValue.ToString();
                    txtRightOfDecimal.Text = fractionalValue;
                }
                else
                {
                        txtLeftOfDecimal.Text = "0";
                        txtRightOfDecimal.Text = "00";
                }
            }
            else
            {
                txtLeftOfDecimal.Text = "0";
                txtRightOfDecimal.Text = "00";
            }
        }

        public string GetCurrencySymbol()
        {
            return this.CurrencySymbol;
        }

        public string GetCurrencyName()
        {
            return this.CurrencyName;
        }

        public string GetAmountMessage()
        {
            return  _messageHelper.GetMessage("js Enter Amount Discount");
        }

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            AmountClientData clientData = new AmountClientData();
            string publishedDollars = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.DiscountValue, DecimalSplit.BeforeDecimal).ToString() : "0";
            string dollars = hdnInitialized.Value == "true" ? txtLeftOfDecimal.Text : publishedDollars;
            string publishedCents = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.DiscountValue, DecimalSplit.AfterDecimal).ToString() : "00";
            string cents = hdnInitialized.Value == "true" ? txtRightOfDecimal.Text : publishedCents;
            decimal amount = Decimal.Parse(dollars + "." + cents);
            clientData.Amount = amount;

            return clientData;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            //view labels     
            litAmountHeader.Text = _messageHelper.GetMessage("lbl Discount Amount");
            litViewAmountLabel.Text = _messageHelper.GetMessage("lbl coupon amount");
            lblAmount.Text = _messageHelper.GetMessage("lbl coupon amount");
        }

        #endregion

        #region JS, CSS, ImagePath

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/amount.css", "EktronCommerceCouponsAmountCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/amount.ie.css", "EktronCommerceCouponsAmountIeCss", Css.BrowserTarget.AllIE);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/js/amount.js", "EktronCommerceCouponsAmountJs");
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        }

        public string GetAmountImagePath()
        {
            return this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/images";
        }

        #endregion
    }
}