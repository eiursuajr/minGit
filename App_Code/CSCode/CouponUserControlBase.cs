using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using System.Collections.Generic;

namespace Ektron.Cms.Commerce.Workarea.Coupons
{
   

    public enum CouponScope
    {
        EntireBasket,
        AllApprovedItems,
        Include,
        Exclude,
        LeastExpensiveItem,
        MostExpensiveItem
    }

    public interface ICouponUserControl
    {
        Object GetClientData();
    }

    public class CouponUserControlBase : System.Web.UI.UserControl
    {

        #region Enumerations

        public enum DecimalSplit
        {
            BeforeDecimal,
            AfterDecimal
        }

        #endregion

        #region Member Variables

        protected ContentAPI _ContentApi;
        protected SiteAPI _SiteApi;
        private Boolean _IsEditable;
        private String _SitePath;
        private String _ApplicationPath;
        protected CurrencyApi _CurrencyApi;
        private Boolean _IsAdmin;
        private Boolean _IsCommerceAdmin;
        protected CouponApi _CouponApi;
        protected UserAPI _UserApi;
        private CouponData _CouponPublishedData;

        #endregion

        #region Properties

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
        public CouponData CouponPublishedData
        {
            get
            {
                return _CouponPublishedData;
            }
            set
            {
                _CouponPublishedData = value;
            }
        }

        #endregion

        #region Constructor

        public CouponUserControlBase()
        {
            _UserApi = new UserAPI();
            _CouponApi = new CouponApi();
            _ContentApi = new ContentAPI();
            _SiteApi = new SiteAPI();
            _CurrencyApi = new CurrencyApi();

            this.SitePath = _ContentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _SiteApi.ApplicationPath.TrimEnd(new char[] { '/' });
            this.IsAdmin = _ContentApi.IsAdmin();
            this.IsCommerceAdmin = _UserApi.EkUserRef.IsARoleMember_CommerceAdmin();

            //check permissions - both admin and commerce admin can edit
            this.IsEditable = (this.IsCommerceAdmin == true || this.IsAdmin == true) ? true : false;
        }

        #endregion

        #region Localized Strings

        public string GetLocalizedStringCancel()
        {
            return this._SiteApi.EkMsgRef.GetMessage("resolve cancel button");
        }

        public string GetLocalizedStringOk()
        {
            return this._SiteApi.EkMsgRef.GetMessage("btn ok");
        }
        

        #endregion

        #region JS, CSS

        protected virtual void RegisterCSS()
        {
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
        }

        protected virtual void RegisterJS()
        {
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        }

        #endregion

        #region Helpers

        public int SplitDecimal(decimal value, DecimalSplit returnValue)
        {
            int splitDecimalReturnValue = 0;
            switch (returnValue)
            {
                case DecimalSplit.AfterDecimal:
                    string afterDecimalString = String.Format("{0:#.00}", value - (int)value);
                    afterDecimalString = afterDecimalString.Replace(".", "");
                    splitDecimalReturnValue = Convert.ToInt32(afterDecimalString);
                    break;
                case DecimalSplit.BeforeDecimal:
                    decimal afterDecimal = value - (int)value;
                    decimal beforeDecimal = value - afterDecimal;
                    splitDecimalReturnValue = (int)beforeDecimal;
                    break;
            }
            return splitDecimalReturnValue;
        }

        private string GetStateBagValue(string controlId, string key)
        {
            if (String.IsNullOrEmpty(key))
                return String.Empty;
            else
                return (string)ViewState[controlId + "_" + key];
        }

        private void SetStateBagValue(string controlId, string key, string value)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                return;

            ViewState[controlId + "_" + key] = value;
        }
        
        protected bool IsInitialized(string controlId)
        {
            return ("done" == GetStateBagValue(controlId, "Initialized"));
        }
        
        protected void SetInitialized(string controlId)
        {
            SetStateBagValue(controlId, "Initialized", "done");
        }

        #endregion
    }
}

namespace Ektron.Cms.Commerce.Workarea.Coupons.Type.ClientData
{
    public class TypeClientData
    {
        #region Enums

        public enum CouponTypes
        {
            Amount,
            Percent
        }

        #endregion

        #region Member Variables

        private CouponTypes _CouponType;
        private string _Code;
        private string _Description;
        private int _CurrencyID;
        private string _CurrencyName;
        private bool _Enabled;
        private string _CurrencySymbol;
        private bool _CouponTypeUserChanged;

        #endregion

        #region Properties

        public CouponTypes CouponType
        {
            get
            {
                return _CouponType;
            }
            set
            {
                _CouponType = value;
            }
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
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        public int CurrencyID
        {
            get
            {
                return _CurrencyID;
            }
            set
            {
                _CurrencyID = value;
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
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
            }
        }

        #endregion
    }

    public class AmountClientData
    {
        #region Member Variables

        private decimal _Amount;

        #endregion

        #region Properties

        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
            }
        }

        #endregion
    }

    public class PercentClientData
    {
        #region Member Variables

        private decimal _Percent;
        private decimal _MaxRedemptionAmount;

        #endregion

        #region Properties

        public decimal Percent
        {
            get { return _Percent; }
            set { _Percent = value; }
        }
        public decimal MaxRedemptionAmount
        {
            get { return _MaxRedemptionAmount; }
            set { _MaxRedemptionAmount = value; }
        }

        #endregion
    }
}

namespace Ektron.Cms.Commerce.Workarea.Coupons.Scope.ClientData
{
    public class ItemsClientData
    {
        #region Member Variables

        private List<ItemsDataClientData> _MostExpensiveItems;
        private List<ItemsDataClientData> _LeastExpensiveItems;
        private List<ItemsDataClientData> _IncludedItems;
        private List<ItemsDataClientData> _ExcludedItems;

        #endregion

        #region Properties

        public List<ItemsDataClientData> MostExpensiveItems
        {
            get
            {
                return _MostExpensiveItems;
            }
            set
            {
                _MostExpensiveItems = value;
            }
        }
        public List<ItemsDataClientData> LeastExpensiveItems
        {
            get
            {
                return _LeastExpensiveItems;
            }
            set
            {
                _LeastExpensiveItems = value;
            }
        }
        public List<ItemsDataClientData> IncludedItems
        {
            get
            {
                return _IncludedItems;
            }
            set
            {
                _IncludedItems = value;
            }
        }
        public List<ItemsDataClientData> ExcludedItems
        {
            get
            {
                return _ExcludedItems;
            }
            set
            {
                _ExcludedItems = value;
            }
        }

        #endregion
    }

    public class ItemsDataClientData
    {
        #region Member Variables

        private long _Id;
        private string _Name;
        private string _Path;
        private string _Type;
        private int _TypeCode;
        private string _SubType;
        private bool _MarkedForDelete;

        #endregion

        #region Properties

        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public int TypeCode
        {
            get
            {
                return _TypeCode;
            }
            set
            {
                _TypeCode = value;
            }
        }
        public string SubType
        {
            get
            {
                return _SubType;
            }
            set
            {
                _SubType = value;
            }
        }
        public bool MarkedForDelete
        {
            get
            {
                return _MarkedForDelete;
            }
            set
            {
                _MarkedForDelete = value;
            }
        }

        #endregion
    }

    public class ScopeClientData
    {
        #region Member Variables

        private CouponScope _CouponScope;
        private bool _IsCombinable;
        private bool _IsOnePerCustomer;
		private bool _IsApplicabletoSubscriptions;
		private bool _IsApplicabletoallquantities;
        private int _MaxRedemptions;
        private decimal _MinBasketValue;
        private DateTime _StartDate;
        private DateTime _ExpirationDate;

        #endregion

        #region Properties

        public CouponScope CouponScope
        {
            get
            {
                return _CouponScope;
            }
            set
            {
                _CouponScope = value;
            }
        }
        public bool IsCombinable
        {
            get
            {
                return _IsCombinable;
            }
            set
            {
                _IsCombinable = value;
            }
        }
		public bool IsApplicabletoSubscriptions
        {
            get
            {
                return _IsApplicabletoSubscriptions;
            }
            set
            {
                _IsApplicabletoSubscriptions = value;
            }
        }
		public bool IsApplicabletoallQuantities
        {
            get { return _IsApplicabletoallquantities; }
            set { _IsApplicabletoallquantities = value; }
        }
        public bool IsOnePerCustomer
        {
            get
            {
                return _IsOnePerCustomer;
            }
            set
            {
                _IsOnePerCustomer = value;
            }
        }
        public int MaxRedemptions
        {
            get
            {
                return _MaxRedemptions;
            }
            set
            {
                _MaxRedemptions = value;
            }
        }
        public decimal MinBasketValue
        {
            get
            {
                return _MinBasketValue;
            }
            set
            {
                _MinBasketValue = value;
            }
        }
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }
        public DateTime ExpirationDate
        {
            get
            {
                return _ExpirationDate;
            }
            set
            {
                _ExpirationDate = value;
            }
        }

        #endregion
    }
}