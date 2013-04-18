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
using System.Collections.Specialized;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Type
{
    public partial class Type : CouponUserControlBase, ICallbackEventHandler, ICouponUserControl
    {
        #region Member Variables
        protected EkMessageHelper messageHelper;
        private string _CallbackResult;
        //private NameValueCollection _PostBackData;
        private bool _PostBackResult;
        private Criteria<CurrencyProperty> _CurrencyCriteria;
        private List<CurrencyData> _CurrencyData;


        #endregion

        #region Events

        #region Page Events

        protected Type()
        {
            _CurrencyCriteria = new Criteria<CurrencyProperty>();
            _CurrencyCriteria.AddFilter(CurrencyProperty.Enabled, CriteriaFilterOperator.EqualTo, true);
            _CurrencyData = _CurrencyApi.GetList(_CurrencyCriteria);

        }
        protected EkMessageHelper _messageHelper
        {
            get
            {
                return (messageHelper ?? (messageHelper = _ContentApi.EkMsgRef));
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            messageHelper = _ContentApi.EkMsgRef;
            //set mode
            mvType.SetActiveView(this.IsEditable ? vwEditType : vwViewType);
            mvCode.SetActiveView(this.IsEditable ? vwEditCode : vwViewCode);
            mvDescription.SetActiveView(this.IsEditable ? vwEditDescription : vwViewDescription);
            vwCurrency.SetActiveView(this.IsEditable ? vwEditCurrency : vwViewCurrency);
            vwStatus.SetActiveView(this.IsEditable ? vwEditStatus : vwViewStatus);

            // only display the required fields message fields can be changed
            litRequiredFieldMessage.Visible = this.IsEditable;

            //register page components
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
                return;

            //set values if data exists
            if (null != this.CouponPublishedData)
                this.SetViewValues();

            // initialize the original coupon-code hidden field (used for validation)
            hdnOriginalCouponCode.Value = (null != this.CouponPublishedData) ? this.CouponPublishedData.Code : "";

            //set hidden field indicating published coupon type
            string couponTypePublished = String.Empty;
            if (this.CouponPublishedData != null)
            {
                couponTypePublished = (this.CouponPublishedData.DiscountType == EkEnumeration.CouponDiscountType.Amount)
               ? GetLocalizedStringAmount()
               : GetLocalizedStringPercent();
            }
            hdnCouponTypePublished.Value = couponTypePublished;

            //set localized strings
            this.SetLocalizedControlText();

            SetInitialized(this.UniqueID);
        }

        #endregion

        #region Control Events

        protected void ddlCurrency_Init(object sender, EventArgs e)
        {
            //load currency drop-down list
            foreach (CurrencyData currency in _CurrencyData)
            {
                if (currency.Enabled)
                {
                    ddlCurrency.Items.Add(new ListItem(GetResourceText(currency.Name), currency.Id.ToString(), true));
                    ddlCurrency.Items[ddlCurrency.Items.Count - 1].Selected
                        = !this.IsPostBack 
                            && _CurrencyApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId == currency.Id;
                }
            }
        }
        protected void ddlStatus_Init(object sender, EventArgs e)
        {
            //load status drop-down list
            ddlStatus.Items.Add(new ListItem(GetEnabledTrueFriendlyName(), "true", true));
            ddlStatus.Items.Add(new ListItem(GetEnabledFalseFriendlyName(), "false", true));
            ddlStatus.Items[0].Selected = true;
        }
        public string GetResourceText(string st)
        {
            if (st == "Euro")
               st= _messageHelper.GetMessage("generic Euro");
            else if (st == "US dollar")
               st= _messageHelper.GetMessage("generic US dollar");
            else if (st == "Australian dollar")
               st= _messageHelper.GetMessage("generic Australian dollar");
            
            
            return st;
        }
        #endregion

        #endregion

        #region Helpers

        private void SetViewValues()
        {
            if (this.IsEditable)
            {
                // set the discount type radio buttons:
                rbAmount.Checked 
                    = (this.CouponPublishedData.DiscountType 
                        == EkEnumeration.CouponDiscountType.Amount);
                rbPercent.Checked = !rbAmount.Checked;
					 
                // initialize the text boxes
                txtCode.Text = this.CouponPublishedData.Code;
                txtDescription.Text = this.CouponPublishedData.Description;

                // set the currency dropdown
                foreach (ListItem item in ddlCurrency.Items)
                    item.Selected = (this.CouponPublishedData.CurrencyId.ToString() == item.Value);

                // set the status drop down
                ddlStatus.Items.FindByValue("true").Selected = (this.CouponPublishedData.IsActive);
                ddlStatus.Items.FindByValue("false").Selected = !(this.CouponPublishedData.IsActive);
            }
            else
            {
                litViewStatusValue.Text = this.CouponPublishedData.IsActive ? GetEnabledTrueFriendlyName() : GetEnabledFalseFriendlyName();
                litViewDescriptionValue.Text = this.CouponPublishedData.Description;
                litViewCodeValue.Text = this.CouponPublishedData.Code;
                litViewTypeValue.Text = 
                    (this.CouponPublishedData.DiscountType == EkEnumeration.CouponDiscountType.Amount) 
                    ? GetLocalizedStringAmount() 
                    : GetLocalizedStringPercent();

                //get currency name from currency id
                Criteria<CurrencyProperty> currencyCriteria = new Criteria<CurrencyProperty>();
                CurrencyData currencyData = _CurrencyApi.GetItem(this.CouponPublishedData.CurrencyId);
                litViewCurrencyValue.Text = currencyData.Name;
            }
        }

        public string GetControlId()
        {
            return this.UniqueID;
        }

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            TypeClientData clientData = new TypeClientData();
            TypeClientData.CouponTypes couponType;
            couponType = rbAmount.Checked ? TypeClientData.CouponTypes.Amount : TypeClientData.CouponTypes.Amount;
            couponType = rbPercent.Checked ? TypeClientData.CouponTypes.Percent : couponType;

            clientData.CouponType = couponType;
            clientData.CouponTypeUserChanged = Boolean.Parse(hdnCouponTypeUserChanged.Value);
            clientData.Code = txtCode.Text;
            clientData.Description = txtDescription.Text;
            clientData.CurrencyID = couponType == TypeClientData.CouponTypes.Percent ? _CurrencyApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId : Convert.ToInt32(ddlCurrency.Items[ddlCurrency.SelectedIndex].Value);
            clientData.CurrencyName = couponType == TypeClientData.CouponTypes.Percent ? this.GetLocalizedStringAllCurrencies() : ddlCurrency.Items[ddlCurrency.SelectedIndex].Text;

            //get currency symbol
            foreach (CurrencyData currency in _CurrencyData)
            {
                if (currency.Id == clientData.CurrencyID)
                {
                    clientData.CurrencySymbol = couponType == TypeClientData.CouponTypes.Percent ? "¤" : currency.CurrencySymbol;
                }
            }

            clientData.Enabled = ddlStatus.Items[ddlStatus.SelectedIndex].Value.ToLower() == "true";

            return clientData;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            //javascript localized strings
            string values = _messageHelper.GetMessage("Validate Code Error");
            string typeChangeAlert = _messageHelper.GetMessage("Alert Important Coupon");
            hdnTypeLocalizedStrings.Value = @"{
                        ""invalidCharacterMessage"": """ + values + @""",
                        ""typeChangeAlert"": """ + typeChangeAlert + @"""
                    }";
            
            //code valid/invalid messages
            litCodeValidMessage.Text = GetLocalizedStringCodeIsAvailable();
            litCodeInalidMessage.Text = GetLocalizedStringCodeIsAlreadyAssigned();
            litRequiredFieldMessage.Text = GetLocalizedStringRequiredField();
            btnCodeValidate.Value = _messageHelper.GetMessage("lbl Validate Code");

            //view labels
            litCouponTypeHeader.Text = GetLocalizedStringCouponType();
            litViewCodeLabel.Text = GetLocalizedStringCode();
            litViewDescriptionLabel.Text = GetLocalizedStringDescription();
            litViewCurrencyLabel.Text = GetLocalizedStringCurrency();
            litViewStatusLabel.Text = GetLocalizedStringStatus();

            //radio button labels
            rbAmount.Text = GetLocalizedStringAmount();
            rbPercent.Text = GetLocalizedStringPercent();

            //field lablels
            litTypeLabel.Text = GetLocalizedStringType();
            lblCode.Text = GetLocalizedStringCode();
            lblCurrency.Text = GetLocalizedStringCurrency();
            lblDescription.Text = GetLocalizedStringDescription();
            lblStatus.Text = GetLocalizedStringStatus();

            litAllCurrencies.Text = this.GetLocalizedStringAllCurrencies();
        }

        public string GetEnabledTrueFriendlyName()
        {
            return _messageHelper.GetMessage("enabled");
        }

        public string GetEnabledFalseFriendlyName()
        {
            return _messageHelper.GetMessage("disabled");
        }

        public string GetCodeValidFriendlyName()
        {
            return _messageHelper.GetMessage("lbl Valid Code");
        }

        public string GetCodeInvalidFriendlyName()
        {
            return _messageHelper.GetMessage("lbl Invalid Code");
        }

        public string GetLocalizedStringAmount()
        {
            return _messageHelper.GetMessage("lbl coupon amount");
        }

        public string GetLocalizedStringPercent()
        {
            return _messageHelper.GetMessage("Generic Percentage");
        }

        public string GetLocalizedStringCodeIsAvailable()
        {
            return _messageHelper.GetMessage("lbl Codeavailable");
        }

        public string GetLocalizedStringCodeIsAlreadyAssigned()
        {
            return _messageHelper.GetMessage("lbl Codeassigned");
        }

        public string GetLocalizedStringRequiredField()
        {
            return _messageHelper.GetMessage("lbl an asterisk indicates a required field");
        }

        public string GetLocalizedStringCouponType()
        {
            return _messageHelper.GetMessage("lbl coupon type");
        }

        public string GetLocalizedStringAllCurrencies()
        {
            return  _messageHelper.GetMessage("lbl All Currencies");
        }

        public string GetLocalizedStringCode()
        {
            return _messageHelper.GetMessage("lbl code");
        }

        public string GetLocalizedStringDescription()
        {
            return _messageHelper.GetMessage("lbl description");
        }

        public string GetLocalizedStringCurrency()
        {
            return _messageHelper.GetMessage("lbl currency");
        }

        public string GetLocalizedStringStatus()
        {
            return _messageHelper.GetMessage("generic status");
        }

        public string GetLocalizedStringType()
        {
            return _messageHelper.GetMessage("generic type");
        }

        #endregion

        #region JS, CSS, Images

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/type.css", "EktronCommerceCouponsTypeCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/type.ie.css", "EktronCommerceCouponsTypeIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            if (!Page.IsCallback)
            {
                JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
                JS.RegisterJS(this, this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/js/type.js", "EktronCommerceCouponsTypeJs");
            }
        }

        public string GetTypeImagesPath()
        {
            return this.ApplicationPath + "/Commerce/Coupons/SharedComponents/Type/css/images";
        }

        #endregion

        #region ICallbackEventHandler

        public string GetCallbackResult()
        {
            return (_CallbackResult);
        }

        public void RaiseCallbackEvent(string eventArgs)
        {
            _PostBackResult = true;
            if (eventArgs != String.Empty)
            {
                //set up coupon code search
                List<CouponData> couponList;
                Criteria<CouponProperty> couponCriteria = new Criteria<CouponProperty>();

                //set search criteria
                couponCriteria.AddFilter(CouponProperty.Code, CriteriaFilterOperator.EqualTo, eventArgs);

                //retreive coupon list data
                couponList = _CouponApi.GetList(couponCriteria);

                //if couponList is greater than zero the couponCode is already in use (return false)
                //if coupoinList is zero couponCode is NOT in use (return true)
                _PostBackResult = couponList.Count > 0 ? false : true;
            }

            _CallbackResult = _PostBackResult.ToString().ToLower();
        }

        #endregion
    }
}