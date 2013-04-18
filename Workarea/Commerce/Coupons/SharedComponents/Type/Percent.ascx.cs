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
    public partial class Percent : CouponUserControlBase, ICouponUserControl
    {
        #region Member Variables

        private string _CurrencyName;
        private string _CurrencySymbol;
        private TypeClientData.CouponTypes _UnpublishedCouponType;
        private bool _CouponTypeUserChanged;

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
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set { _CurrencyName = value; }
        }
        public string CurrencySymbol
        {
            get { return _CurrencySymbol; }
            set { _CurrencySymbol = value; }
        }

        #endregion

        #region Events

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
            //set views
            mvPercent.SetActiveView(base.IsEditable ? vwEditPercent : vwViewPercent);
            mvMaxAmount.SetActiveView(base.IsEditable ? vwEditMaxAmount : vwViewMaxAmount);

            //register page components
            if (!this.Page.IsCallback)
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
                        txtHundreds.Text = "0";
                        txtHundredths.Text = "00";
                        txtDollars.Text = "0";
                        txtCents.Text = "00";
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

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            PercentClientData clientData = new PercentClientData();

            //get percent
            string publishedHundreds = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.DiscountValue, DecimalSplit.BeforeDecimal).ToString() : "0";
            string hundreds = hdnInitialized.Value == "true" ? txtHundreds.Text : publishedHundreds;
            string publishedHundredths = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.DiscountValue, DecimalSplit.AfterDecimal).ToString() : "00";
            string hundredths = hdnInitialized.Value == "true" ? txtHundredths.Text : publishedHundredths;
            decimal precent = Decimal.Parse(hundreds + "." + hundredths);
            clientData.Percent = precent;

            //get max redemption amount
            string publishedDollars = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.MaximumAmount, DecimalSplit.BeforeDecimal).ToString() : "0";
            string dollars = hdnInitialized.Value == "true" ? txtDollars.Text : publishedDollars;
            string publishedCents = this.CouponPublishedData != null ? base.SplitDecimal(this.CouponPublishedData.MaximumAmount, DecimalSplit.AfterDecimal).ToString() : "00";
            string cents = hdnInitialized.Value == "true" ? txtCents.Text : publishedCents;
            decimal maxRedemptionAmount = Decimal.Parse(dollars + "." + cents);
            clientData.MaxRedemptionAmount = maxRedemptionAmount;

            return clientData;
        }

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
                    string maxAmountDollars = "0";
                    string maxAmountCents = "00";

                    // percent field
                    // view-only
                    litViewPercentValue.Text = wholeValue.ToString() + "." + fractionalValue;
                    // editable
                    txtHundreds.Text = wholeValue.ToString();
                    txtHundredths.Text = fractionalValue;

                    // maximum redemption ammount field
                    wholeValue = Decimal.Truncate(this.CouponPublishedData.MaximumAmount);
                    fractionalValue = String.Format("{0:D2}", (int)((this.CouponPublishedData.MaximumAmount - wholeValue) * 100));

                    //max amount with a value of zero is unlimited
                    //if max amount is zero, do not show "0.00"
                    //instead show blank fields
                    if (this.CouponPublishedData.MaximumAmount != 0)
                    {
                        maxAmountDollars = wholeValue.ToString();
                        maxAmountCents = fractionalValue;
                    }

                    // view-only
                    litViewMaxAmountValue.Text = maxAmountDollars + "." + maxAmountCents;

                    // editable
                    txtDollars.Text = maxAmountDollars;
                    txtCents.Text = maxAmountCents;
                }
                else
                {
                        txtHundreds.Text = "0";
                        txtHundredths.Text = "00";
                        txtDollars.Text = "0";
                        txtCents.Text = "00";
                }
            }
            else
            {
                txtHundreds.Text = "0";
                txtHundredths.Text = "00";
                txtDollars.Text = "0";
                txtCents.Text = "00";
            }
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            litPercentHeader.Text = this._ContentApi.EkMsgRef.GetMessage("generic Discount Percentage");
            litViewMaxAmountLabel.Text = this.GetMaxAmountLabel();
            litViewPercentLabel.Text = this.GetPercentLabel();
            lblPercent.Text = this.GetPercentLabel();
            lblMaxAmount.Text = this.GetMaxAmountLabel();
            pMaxAmountMessage.InnerText = this._ContentApi.EkMsgRef.GetMessage("generic note")+":";
            spanMessageCart.InnerHtml = this._ContentApi.EkMsgRef.GetMessage("lbl coupon applies");
            spanMessageItems.InnerHtml = this._ContentApi.EkMsgRef.GetMessage("lbl coupon individual items");
        }

        public string GetJavascriptLocalizedStrings()
        {
            return @"{
                        ""over100percentWarning"": ""Warning: Discount exceeds 100%!""
                    }";
        }
        public string GetPercentLabel()
        {
            return this._ContentApi.EkMsgRef.GetMessage("Generic Percentage");
        }
        public string GetMaxAmountLabel()
        {
            return this._ContentApi.EkMsgRef.GetMessage("lbl coupon max ammount");
        }
        public string GetCouponMaxMessage()
        {
            return this._ContentApi.EkMsgRef.GetMessage("lbl maximum number coupons");
        }
        public string GetPercentMessage()
        {
            return this._ContentApi.EkMsgRef.GetMessage("lbl percentage discount");
        }
        public string GetCurrencyName()
        {
            return this.CurrencyName;
        }
        public string GetCurrencySymbol()
        {
            return this.CurrencySymbol;
        }
        public string GetHelpMessage()
        {
            return String.Empty;
        }
        public string GetPercentName()
        {
            return this._ContentApi.EkMsgRef.GetMessage("lbl coupon amount percent");
        }

        #endregion

        #region JS, CSS, Images

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/percent.css", "EktronCommerceCouponsPercentCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/percent.ie.css", "EktronCommerceCouponsPercentIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            if (!Page.IsCallback)
            {
                JS.RegisterJS(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/js/percent.js", "EktronCommerceCouponsPercentJs");
                JS.RegisterJS(this, JS.ManagedScript.EktronStringJS);
            }
        }

        public string GetPercentImagesPath()
        {
            return base.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/images";
        }

        #endregion
    }
}