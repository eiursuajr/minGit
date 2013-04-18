using System;
using System.Collections.Generic;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.UI;

namespace Ektron.Cms.Commerce.Workarea.Coupons
{
    public class CouponBase : System.Web.UI.Page
    {
        #region Member Variables

        protected UserAPI _UserApi;
        protected CouponApi _CouponApi;
        protected ContentAPI _ContentApi;
		protected ContentAPI m_refContentApi = new ContentAPI();
        protected EkMessageHelper m_refMsg;
        protected SiteAPI _SiteApi;
        protected CurrencyApi _CurrencyApi;
        private Boolean _IsEditable;
        private Boolean _IsAdmin;
        private Boolean _IsCommerceAdmin;
        private String _SitePath;
        private String _ApplicationPath;

        #endregion
        
        #region Properties

        protected bool IsAdmin
        {
            get
            {
                return _IsAdmin;
            }
            set
            {
                _IsAdmin = value;
            }
        }

        protected bool IsCommerceAdmin
        {
            get
            {
                return _IsCommerceAdmin;
            }
            set
            {
                _IsCommerceAdmin = value;
            }
        }

        protected bool IsEditable
        {
            get
            {
                return _IsEditable;
            }
            set
            {
                _IsEditable = value;
            }
        }

        protected String SitePath
        {
            get
            {
                return _SitePath;
            }
            set
            {
                _SitePath = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
            set
            {
                _ApplicationPath = value;
            }
        }

        #endregion

        #region Constructor

        public CouponBase()
        {
            _UserApi = new UserAPI();
            _CouponApi = new CouponApi();
            _ContentApi = new ContentAPI();
            _SiteApi = new SiteAPI();
            _CurrencyApi = new CurrencyApi();
			 m_refMsg = m_refContentApi.EkMsgRef;

            this.IsAdmin = _ContentApi.IsAdmin();
            this.IsCommerceAdmin = _UserApi.EkUserRef.IsARoleMember_CommerceAdmin();
            this.SitePath = _ContentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _SiteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region Localized Strings

         public string GetLocalizedStringMarkForDelete()
        {
            return this.GetMessage("lbl coupon mark delete");
        }

        public string GetLocalizedStringMarkForDeleteAll()
        {
            return this.GetMessage("lbl coupon mark all page delete");
        }

        public string GetLocalizedStringRestore()
        {
            return this.GetMessage("wiget restore");
        }

        public string GetLocalizedStringRestoreAll()
        {
            return this.GetMessage("lbl coupon restore all on page");
        }

        public string GetLocalizedStringSave()
        {
            return this.GetMessage("btn save");
        }

        public string GetLocalizedStringBack()
        {
            return this.GetMessage("lbl back");
        }

        public string GetLocalizedStringOK()
        {
            return this.GetMessage("btn ok");
        }

        public string GetLocalizedStringCancel()
        {
            return this.GetMessage("btn cancel");
        }

        public string GetLocalizedStringCloseModal()
        {
            return this.GetMessage("close title");
        }
        #endregion

        #region Helpers

        public string GetStripeRow(int index)
        {
            return index % 2 == 0 ? String.Empty : @" class=""stripe""";
        }

        protected void SetCouponItems(CouponScope couponScope, CouponData couponData, ItemsClientData itemsClientData)
        {
            //coupon items list
            List<CouponEntryData> items = new List<CouponEntryData>();
            List<ItemsDataClientData> includedItems = new List<ItemsDataClientData>();
            List<ItemsDataClientData> excludedItems = new List<ItemsDataClientData>();

            //get included/excluded items
            switch (couponScope)
            {
                case CouponScope.EntireBasket:
                    couponData.CouponType = EkEnumeration.CouponType.BasketLevel;
                    break;
                case CouponScope.AllApprovedItems:
                case CouponScope.MostExpensiveItem:
                case CouponScope.LeastExpensiveItem:
                    includedItems = itemsClientData.IncludedItems;
                    excludedItems = itemsClientData.ExcludedItems;
                    couponData.CouponType = EkEnumeration.CouponType.AllItems;
                    break;
            }

            includedItems = includedItems == null ? new List<ItemsDataClientData>() : includedItems;
            excludedItems = excludedItems == null ? new List<ItemsDataClientData>() : excludedItems;

            //build coupon included items list
            foreach (ItemsDataClientData item in includedItems)
            {
                if (item.MarkedForDelete == false)
                {
                    CouponEntryData includedItem = new CouponEntryData();
                    includedItem.CouponId = couponData.Id;
                    includedItem.ObjectId = item.Id;
                    switch (item.Type.ToLower())
                    {
                        case "catalog":
                            includedItem.ObjectType = EkEnumeration.CMSObjectTypes.Folder;
                            break;
                        case "taxonomy":
                            includedItem.ObjectType = EkEnumeration.CMSObjectTypes.TaxonomyNode;
                            break;
                        case "product":
                            includedItem.ObjectType = EkEnumeration.CMSObjectTypes.CatalogEntry;
                            break;
                    }
                    includedItem.IsIncluded = true;
                    items.Add(includedItem);
                }
            }
            //build coupon excluded items list
            foreach (ItemsDataClientData item in excludedItems)
            {
                if (item.MarkedForDelete == false)
                {
                    CouponEntryData excludedItem = new CouponEntryData();
                    excludedItem.CouponId = couponData.Id;
                    excludedItem.ObjectId = item.Id;
                    switch (item.Type.ToLower())
                    {
                        case "catalog":
                            excludedItem.ObjectType = EkEnumeration.CMSObjectTypes.Folder;
                            break;
                        case "taxonomy":
                            excludedItem.ObjectType = EkEnumeration.CMSObjectTypes.TaxonomyNode;
                            break;
                        case "product":
                            excludedItem.ObjectType = EkEnumeration.CMSObjectTypes.CatalogEntry;
                            break;
                    }
                    excludedItem.IsIncluded = false;
                    items.Add(excludedItem);
                }
            }

            //add items to coupon
            _CouponApi.SaveCouponApplications(couponData.Id, items);
        }

        #endregion

        #region JS, CSS, Images

        protected virtual void RegisterCSS()
        {
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected virtual void RegisterJS()
        {
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            Packages.Ektron.JSON.Register(this);
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        }

        public string GetImagesPath(string template)
        {
            string returnValue = "You must specify 'add', 'list', 'properties', or 'shared'";
            switch(template.ToLower())
            {
                case "add":
                    returnValue = @"/Commerce/Coupons/Add/css/images";
                    break;
                case "list":
                    returnValue = @"/Commerce/Coupons/List/css/images";
                    break;
                case "properties":
                    returnValue = @"/Commerce/Coupons/Properties/css/images";
                    break;
                case "shared":
                    returnValue = @"/Commerce/Coupons/SharedComponents/css/images";
                    break;
            }
            return this.ApplicationPath + returnValue;
        }
		
		public string GetMessage(string MessageTitle)
        {
            return m_refMsg.GetMessage(MessageTitle);
        }

        #endregion
    }
}