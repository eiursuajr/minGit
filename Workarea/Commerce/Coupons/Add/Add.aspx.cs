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
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons.Add.ClientData;
using Ektron.Cms.Commerce.Workarea.Coupons.Type.ClientData;
using Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Add.ClientData
{
    public class AddClientData
    {
        #region Member Variables

        private TypeClientData _TypeData;
        private AmountClientData _AmountData;
        private PercentClientData _PercentClientData;
        private ScopeClientData _ScopeData;
        private ItemsClientData _ItemsData;
        #endregion

        #region Properties

        public TypeClientData TypeData
        {
            get
            {
                return _TypeData;
            }
            set
            {
                _TypeData = value;
            }
        }
        public AmountClientData AmountData
        {
            get
            {
                return _AmountData;
            }
            set
            {
                _AmountData = value;
            }
        }
        public PercentClientData PercentData
        {
            get
            {
                return _PercentClientData;
            }
            set
            {
                _PercentClientData = value;
            }
        }
        public ScopeClientData ScopeData
        {
            get
            {
                return _ScopeData;
            }
            set
            {
                _ScopeData = value;
            }
        }
        public ItemsClientData ItemsData
        {
            get
            {
                return _ItemsData;
            }
            set
            {
                _ItemsData = value;
            }
        }
        
        #endregion
    }
}
namespace Ektron.Cms.Commerce.Workarea.Coupons.Add 
{
    public partial class Add : CouponBase
    {
        #region Member Variables

        private AddClientData _ClientData;

        #endregion

        #region Properties

        public AddClientData ClientData
        {
            get
            {
                return _ClientData;
            }
            set
            {
                _ClientData = value;
            }
        }

        #endregion

        #region Events

        #region Page Events

        protected Add()
        {
            _ClientData = new AddClientData();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //check permissions - both admin and commerce admin can edit
            this.IsEditable = (this.IsCommerceAdmin == true || this.IsAdmin == true) ? true : false;

            //set main view
            phModeView.Visible = this.IsEditable == false ? true : false;
            if (this.IsEditable == false)
            {
                divCouponAdd.Attributes.Add("class", "couponAdd notLoggedIn");
            }
            phModeEdit.Visible = this.IsEditable == true ? true : false;

            //set back image
            imgBack.AlternateText = this.GetMessage("lbl back to all coupons");
            imgBack.Attributes.Add("title", GetMessage("lbl back to all coupons"));
            imgBack.ImageUrl = this.ApplicationPath + "/images/ui/icons/back.png";

            //set help link
            aHelp.HRef = "#Help";
            aHelp.Attributes.Add("onclick", "window.open('" + _ContentApi.fetchhelpLink("coupons_props") + "', 'SitePreview', 'width=800,height=500,resizable=yes,toolbar=no,scrollbars=1,location=no,directories=no,status=no,menubar=no,copyhistory=no');return false;");
            imgHelp.Src = this.GetAddImagesPath() + "/help.png";

            //register page components
            this.RegisterCSS();
            this.RegisterJS();

            //Change the default Text of buttons on Wizard control.
            wzCouponAdd.StartNextButtonText = wzCouponAdd.StepNextButtonText = this.GetMessage("btn next");
            wzCouponAdd.FinishPreviousButtonText = wzCouponAdd.StepPreviousButtonText = this.GetMessage("previous");
            wzCouponAdd.FinishCompleteButtonText = this.GetMessage("btn finish");
            wzCouponAdd.CancelButtonText = this.GetMessage("generic cancel");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //get client data from user controls
            this.GetClientData();

            if (this.ClientData != null)
            {
                //set currency details for amount user control
                ucAmount.CurrencyId = this.ClientData.TypeData.CurrencyID;
                ucAmount.CurrencyName = this.ClientData.TypeData.CurrencyName;
                ucAmount.CurrencySymbol = this.ClientData.TypeData.CurrencySymbol;
                ucAmount.CouponTypeUserChanged = this.ClientData.TypeData.CouponTypeUserChanged;

                //set currency details for percent user control
                ucPercent.CurrencyName = this.ClientData.TypeData.CurrencyName;
                ucPercent.CurrencySymbol = this.ClientData.TypeData.CurrencySymbol;
                ucPercent.CouponTypeUserChanged = this.ClientData.TypeData.CouponTypeUserChanged;

                //set currency details for scope user control
                ucScope.CurrencyId = this.ClientData.TypeData.CurrencyID;
                ucScope.CurrencyName = this.ClientData.TypeData.CurrencyName;
                ucScope.CurrencySymbol = this.ClientData.TypeData.CurrencySymbol;
				ucScope.IsCouponTypeAmount = (this.ClientData.TypeData.CouponType == TypeClientData.CouponTypes.Amount ? true : false);

                //set items selection type for items user control
                ucItems.Scope = this.ClientData.ScopeData.CouponScope;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //set localized strings
            this.SetLocalizedControlText();
        }

        protected void SideBarList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            WizardStep dataItem = e.Item.DataItem as WizardStep;
            LinkButton linkButton = e.Item.FindControl("SideBarButton") as LinkButton;
            if (dataItem != null)
            {
                linkButton.Attributes.Add("class", (dataItem.Wizard.ActiveStepIndex == e.Item.ItemIndex) ? "preventClick sidebarNavigationActive" : "preventClick");
                linkButton.Attributes.Add("onclick", "return false;");
            }
        }

        #endregion

        #region User Events

        protected void wsDiscount_OnLoad(object sender, EventArgs e)
        {
            if (this.ClientData.TypeData != null)
            {
                switch (this.ClientData.TypeData.CouponType)
                {
                    case TypeClientData.CouponTypes.Amount:
                        mvDiscount.SetActiveView(vwAmount);
                        break;
                    case TypeClientData.CouponTypes.Percent:
                        mvDiscount.SetActiveView(vwPercent);
                        break;
                }
            }
        }

        public void wzCouponAdd_OnFinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            CouponData couponData = new CouponData();
            //type data
            couponData.Code = this.ClientData.TypeData.Code;
            couponData.CurrencyId = this.ClientData.TypeData.CurrencyID;
            couponData.Description = this.ClientData.TypeData.Description.Replace(">", "").Replace("<", "");
            couponData.IsActive = this.ClientData.TypeData.Enabled;
            couponData.DiscountType = this.ClientData.TypeData.CouponType == TypeClientData.CouponTypes.Amount ? EkEnumeration.CouponDiscountType.Amount : EkEnumeration.CouponDiscountType.Percent;

            //discount
            switch(this.ClientData.TypeData.CouponType) 
            {
                case TypeClientData.CouponTypes.Amount:
                    couponData.DiscountValue = this.ClientData.AmountData.Amount;
                    couponData.MaximumAmount = 0; //set maximum amount to zero since discount type is amount
                    break;
                case TypeClientData.CouponTypes.Percent:
                    couponData.DiscountValue = this.ClientData.PercentData.Percent;
                    couponData.MaximumAmount = this.ClientData.PercentData.MaxRedemptionAmount;
                    break;
            }

            //scope type
            switch (this.ClientData.ScopeData.CouponScope)
            {
                case CouponScope.EntireBasket:
                    couponData.CouponType = EkEnumeration.CouponType.BasketLevel;
                    break;
                case CouponScope.AllApprovedItems:
                    couponData.CouponType = EkEnumeration.CouponType.AllItems;
                    break;
                case CouponScope.MostExpensiveItem:
                    couponData.CouponType = EkEnumeration.CouponType.MostExpensiveItem;
                    break;
                case CouponScope.LeastExpensiveItem:
                    couponData.CouponType = EkEnumeration.CouponType.LeastExpensiveItem;
                    break;
            }

            //scope
            couponData.ExpirationDate = this.ClientData.ScopeData.ExpirationDate;
            couponData.StartDate = this.ClientData.ScopeData.StartDate;
            couponData.OnePerCustomer = this.ClientData.ScopeData.IsOnePerCustomer;
            couponData.MinimumAmount = this.ClientData.ScopeData.MinBasketValue;
            couponData.MaximumUses = this.ClientData.ScopeData.MaxRedemptions;
            couponData.IsCombinable = this.ClientData.ScopeData.IsCombinable;
			couponData.IsApplicabletoSubscriptions = this.ClientData.ScopeData.IsApplicabletoSubscriptions;
			couponData.AppliesToQuantity = this.ClientData.ScopeData.IsApplicabletoallQuantities;

            //add coupon
            _CouponApi.Add(couponData);

            //set coupon id for finish ascx
            ucFinish.CouponId = couponData.Id;

            //set items associated with coupon
            base.SetCouponItems(this.ClientData.ScopeData.CouponScope, couponData, this.ClientData.ItemsData);
        }

        #endregion

        #endregion

        #region Client Data

        private void GetClientData()
        {
            this.ClientData.TypeData = (TypeClientData)this.ucType.GetClientData();
            this.ClientData.AmountData = (AmountClientData)this.ucAmount.GetClientData();
            this.ClientData.PercentData = (PercentClientData)this.ucPercent.GetClientData();
            this.ClientData.ScopeData = (ScopeClientData)this.ucScope.GetClientData();
            this.ClientData.ItemsData = (ItemsClientData)this.ucItems.GetClientData();
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {

            litAddCouponHeader.Text = this.GetMessage("lbl add coupon");
            litInvalidPermissions.Text = this.GetMessage("lbl coupon permission");

            //wizard step labels
            wsType.Title = this.GetMessage("generic type");
            wsDiscount.Title = this.GetMessage("lbl coupon discount");
            wsScope.Title = this.GetMessage("generic scope");
            wsItems.Title = this.GetMessage("generic items");
            wsFinish.Title = this.GetMessage("btn finish");

            //help link
            aHelp.Title = this.GetMessage("generic help");
            imgHelp.Alt = this.GetMessage("generic help");
            imgHelp.Attributes.Add("title", this.GetMessage("generic help"));
        }
        #endregion

        #region JS, CSS

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/Add/css/add.css", @"EktronCommerceCouponAddCss");
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/Add/css/add.ie.css", @"EktronCommerceCouponAddIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, this.ApplicationPath + @"/Commerce/Coupons/Add/js/add.js", @"EktronCommerceCouponAddJs");
        }

        private string GetAddImagesPath()
        {
            return this.ApplicationPath + @"/Commerce/Coupons/Add/css/images";
        }

        #endregion
    }
}
