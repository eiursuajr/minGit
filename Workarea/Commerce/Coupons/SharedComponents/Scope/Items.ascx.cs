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
using Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData;

namespace Ektron.Cms.Commerce.Workarea.Coupons.Scope
{
    public partial class Items : CouponUserControlBase
    {
        #region Enumerations

        private enum ActiveTab
        {
            Include,
            Exclude
        }

        #endregion

        #region Member Variables
        protected EkMessageHelper messageHelper;
        private CouponScope _Scope;
        private ActiveTab _ActiveTab;

        #endregion

        #region Properties

        public CouponScope Scope
        {
            get
            {
                return _Scope;
            }
            set
            {
                _Scope = value;
            }
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

        #region PageEvents

        protected void Page_Init(object sender, EventArgs e)
        {
            //register page components
            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ucExclude.CouponPublishedData = this.CouponPublishedData;
            ucInclude.CouponPublishedData = this.CouponPublishedData;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //set view to entire cart or to approved items
            switch (this.Scope)
            {
                case CouponScope.EntireBasket:
                    mvScope.SetActiveView(vwEntireCart);
                    break;
                case CouponScope.AllApprovedItems:
                case CouponScope.Exclude:
                case CouponScope.Include:
                case CouponScope.LeastExpensiveItem:
                case CouponScope.MostExpensiveItem:
                    mvScope.SetActiveView(vwApprovedItems);
                    //set view to include or exclude
                    _ActiveTab = lbExclude.CssClass == "selected" ? ActiveTab.Exclude : ActiveTab.Include;
                    switch (_ActiveTab)
                    {
                        case ActiveTab.Include:
                            mvIncludeExclude.SetActiveView(vwInclude);
                            break;
                        case ActiveTab.Exclude:
                            mvIncludeExclude.SetActiveView(vwExclude);
                            break;
                    }
                    break;
            }

            //set localized strings
            this.SetLocalizedControlText();
        }

        #endregion

        #region User Events

        protected void ItemsData_Click(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Include":
                    lbInclude.CssClass = "selected";
                    lbExclude.CssClass = String.Empty;
                    _ActiveTab = ActiveTab.Include;
                    break;
                case "Exclude":
                    lbInclude.CssClass = String.Empty;
                    lbExclude.CssClass = "selected";
                    _ActiveTab = ActiveTab.Exclude;
                    break;
            }
        }

        #endregion

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            ItemsClientData itemsClientData = new ItemsClientData();
            itemsClientData.IncludedItems = (List<ItemsDataClientData>)ucInclude.GetClientData();
            itemsClientData.ExcludedItems = (List<ItemsDataClientData>)ucExclude.GetClientData();
            return itemsClientData;
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            //set include/exclude link buttons
            lbInclude.Text = _messageHelper.GetMessage("generic Include");
            lbExclude.Text = _messageHelper.GetMessage("lbl sync exclude files");

            //set header & description
            litDescriptionConflictMessage.Visible = false;
            string header = String.Empty;
            string description = String.Empty;
            switch (this.Scope)
            {
                case CouponScope.AllApprovedItems:
                case CouponScope.MostExpensiveItem:
                case CouponScope.LeastExpensiveItem:
                    switch (_ActiveTab)
                    {
                        case ActiveTab.Include:
                            header =  _messageHelper.GetMessage("lbl Items Included");
                            description = _messageHelper.GetMessage("lbl accepted coupon");
                            ucInclude.CatalogLabel = _messageHelper.GetMessage("lbl Include Catalog");
                            ucInclude.CategoryLabel = _messageHelper.GetMessage("lbl Include Category");
                            ucInclude.ProductLabel = _messageHelper.GetMessage("lbl Include Product");
                            litDescriptionConflictMessage.Visible = true;
                            liInclude.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active ui-state-focus");
                            liExclude.Attributes.Add("class", "ui-state-default ui-corner-top");
                            break;
                        case ActiveTab.Exclude:
                            header = _messageHelper.GetMessage("lbl Items Excluded");
                            description = _messageHelper.GetMessage("lbl coupon rejected");
                            ucExclude.CatalogLabel = _messageHelper.GetMessage("lbl Exclude Catalog");
                            ucExclude.CategoryLabel = _messageHelper.GetMessage("lbl Exclude Category");
                            ucExclude.ProductLabel = _messageHelper.GetMessage("lbl Exclude Product");
                            litDescriptionConflictMessage.Visible = true;
                            liExclude.Attributes.Add("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active ui-state-focus");
                            liInclude.Attributes.Add("class", "ui-state-default ui-corner-top");
                            break;
                    }
                    break;
                case CouponScope.EntireBasket:
                    litEntireCartHeader.Text = _messageHelper.GetMessage("lbl Items Entire Basket");
                    litEntireCartDescription.Text = _messageHelper.GetMessage("lbl coupon accepted");
                    break;
            }
            litDescription.Text = description;
            litDescriptionConflictMessage.Text = _messageHelper.GetMessage("lbl coupon excluded warning");
            litItemsHeader.Text = header;
        }

        #endregion

        #region JS, CSS

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/css/items.css", @"EktronCommerceCouponsItemsCss");
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/css/items.ie7.css", @"EktronCommerceCouponsItemsIe7Css", Css.BrowserTarget.IE7);
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/css/items.ie6.css", @"EktronCommerceCouponsItemsIe6Css", Css.BrowserTarget.IE6);
        }
        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS);
            JS.RegisterJS(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/js/items.js", @"EktronCommerceCouponsItemsJs");
        }
        public string GetItemsImagesPath()
        {
            return base.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/css/images";
        }

        #endregion
    }
}