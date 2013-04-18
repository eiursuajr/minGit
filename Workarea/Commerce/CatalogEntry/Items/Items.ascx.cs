using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions;

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData
{
    public abstract class ItemData
    {
        private int _Order;
        private long _Id;
        private string _Title;
        private bool _MarkedForDelete;

        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
            }
        }
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
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
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
    }

    public class BundleData : ItemData
    {
    }
    public class ProductData : ItemData
    {
    }
    public class ComplexProductData : ItemData
    {
    }
    public class KitData : ItemData
    {
        private string _Description;
        private List<KitDataItems> _Items;

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
        public List<KitDataItems> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }
    }
    public class KitDataItems
    {
        private int _Order;
        private long _Id;
        private string _Title;
        private string _ExtraText;
        private string _PriceModifierPlusMinus;
        private string _PriceModifierDollars;
        private string _PriceModifierCents;
        private bool _MarkedForDelete;

        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
            }
        }
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
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        public string ExtraText
        {
            get
            {
                return _ExtraText;
            }
            set
            {
                _ExtraText = value;
            }
        }
        public string PriceModifierPlusMinus
        {
            get
            {
                return _PriceModifierPlusMinus;
            }
            set
            {
                _PriceModifierPlusMinus = value;
            }
        }
        public string PriceModifierDollars
        {
            get
            {
                return _PriceModifierDollars;
            }
            set
            {
                _PriceModifierDollars = value;
            }
        }
        public string PriceModifierCents
        {
            get
            {
                return _PriceModifierCents;
            }
            set
            {
                _PriceModifierCents = value;
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

    }
}

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items
{
    public partial class Item : System.Web.UI.UserControl
    {
        #region Member Variables

        private ContentAPI _contentApi;
        protected SiteAPI _siteApi;
        private String _sitePath;
        private String _applicationPath;
        private String _SubscriptionControlPath;
        private EntryData _EntryEditData;
        private bool _IsEditable;
        private int _ItemCount;
        private long _ItemsFolderId;
        private List<Object> _itemData;
        private DisplayModeValue _DisplayMode;
        private Ektron.Cms.Commerce.Currency _Currency;

        #endregion

        #region Enumerations

        public enum DisplayModeValue
        {
            View,
            Edit
        }

        public enum ViewMode
        {
            Default,
            Kit,
            Subscription
        }

        #endregion

        #region Properties

        public DisplayModeValue DisplayMode
        {
            get
            {
                return _DisplayMode;
            }
            set
            {
                _DisplayMode = value;
            }
        }

        public Object ItemData
        {
            get
            {
                return _itemData;
            }
        }

        public EntryData EntryEditData
        {
            get
            {
                return _EntryEditData;
            }
            set
            {
                _EntryEditData = value;
            }
        }

        public String SubscriptionControlPath
        {
            get
            {
                return _SubscriptionControlPath;
            }
            set
            {
                _SubscriptionControlPath = value;
            }
        }

        public bool IsEditable
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
                return _sitePath;
            }
            set
            {
                _sitePath = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _applicationPath;
            }
            set
            {
                _applicationPath = value;
            }
        }

        protected int ItemCount
        {
            get
            {
                return _ItemCount;
            }
            set
            {
                _ItemCount = value;
            }
        }

        [Description("Must be set for 'Default View' Items.  ItemsFolderId value is used when user is adding an item to default view.")]
        public long ItemsFolderId
        {
            get
            {
                return _ItemsFolderId;
            }
            set
            {
                _ItemsFolderId = value;
            }
        }

        #endregion

        #region Constructor

        protected Item()
        {
            _contentApi = new ContentAPI();
            _siteApi = new SiteAPI();
            _itemData = null;
            _Currency = new Currency();

            this.SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _siteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region Init, Page_Load

        protected override void OnInit(EventArgs e)
        {
            string itemTabJsonData = Request.Form["ItemData"] ?? String.Empty;
            string itemTabDeserializationMode = Request.Form["ItemDataDeserializationMode"] ?? String.Empty;
            if (itemTabJsonData != String.Empty && itemTabDeserializationMode != String.Empty)
            {
                this.DeserializeJsonData(itemTabJsonData, itemTabDeserializationMode);
            }
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.IsEditable = (this.DisplayMode == DisplayModeValue.Edit) ? true : false;

            //set item editable hidden field to true/false - picked up by js init()
            hdnItemEditable.Value = this.IsEditable.ToString().ToLower();

            this.RegisterJS();
            this.RegisterCSS();

            if (this.EntryEditData != null)
            {
                switch (this.EntryEditData.EntryType)
                {
                    case EkEnumeration.CatalogEntryType.Kit:
                        hdnItemView.Value = "kit"; //set item view hidden field to default or kit - picked up by js init()
                        hdnJsonMode.Value = "Kit";
                        KitData kitData = (KitData)this.EntryEditData;
                        this.ItemCount = kitData.OptionGroups.Count;
                        rptKitGroup.DataSource = kitData.OptionGroups;
                        rptKitGroup.DataBind();
                        this.SetKitView();
                        break;
                    case EkEnumeration.CatalogEntryType.Bundle:
                        hdnItemView.Value = "default"; //set item view hidden field to default or kit - picked up by js init()
                        hdnJsonMode.Value = "Bundle";
                        BundleData bundleData = (BundleData)this.EntryEditData;
                        this.ItemCount = bundleData.BundledItems.Count;
                        rptDefault.DataSource = bundleData.BundledItems;
                        rptDefault.DataBind();
                        this.SetDefaultView();
                        break;
                    case EkEnumeration.CatalogEntryType.Product:
                        hdnItemView.Value = "default"; //set item view hidden field to default or kit - picked up by js init()
                        hdnJsonMode.Value = "Product";
                        ProductData productData = (ProductData)this.EntryEditData;
                        this.ItemCount = productData.Variants.Count;
                        rptDefault.DataSource = productData.Variants;
                        rptDefault.DataBind();
                        this.SetDefaultView();
                        break;
                    case EkEnumeration.CatalogEntryType.ComplexProduct:
                        hdnItemView.Value = "default"; //set item view hidden field to default or kit - picked up by js init()
                        hdnJsonMode.Value = "ComplexProduct";
                        ProductData complexProductData = (ProductData)this.EntryEditData;
                        this.ItemCount = complexProductData.Variants.Count;
                        rptDefault.DataSource = complexProductData.Variants;
                        rptDefault.DataBind();
                        this.SetDefaultView();
                        break;
                    case EkEnumeration.CatalogEntryType.SubscriptionProduct:
                        hdnItemView.Value = "subscription"; //set item view hidden field to default or kit - picked up by js init()
                        hdnJsonMode.Value = "SubscriptionProduct";
                        this.ItemCount = 0;
                        SubscriptionProductData subscriptionProductData = (SubscriptionProductData)this.EntryEditData;
                        this.SetSubscriptionView(subscriptionProductData.SubscriptionInfo);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Kit View Helpers

        protected void rptKitGroup_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                Repeater kitItemRepeater = (Repeater)e.Item.FindControl("rptKitItem");
                OptionGroupItemCollection kitItems = ((OptionGroupData)e.Item.DataItem).Options;
                kitItemRepeater.DataSource = kitItems;
                kitItemRepeater.DataBind();
            }
        }

        protected void SetKitViewLocalizedStrings()
        {
            hdnBlankNameFieldErrorMessage.Value = "Error: name field cannot be blank";
            litEmptyKitListLabel.Text = "No Kit Items Exist";
        }

        public string GetCurrencySymbol()
        {
            return _contentApi.RequestInformationRef.CommerceSettings.CurrencySymbol;
        }

        protected string GetPriceModifierPlusMinus(decimal priceModifier)
        {
            return (priceModifier >= 0) ? "+" : "-";
        }

	protected string GetPriceModifierDollars(decimal priceModifier)
        {
            return priceModifier < 0 ? Decimal.Negate(Decimal.Floor(priceModifier)).ToString() : Decimal.Floor(priceModifier).ToString();
        }

        protected bool GetKitGroupAddKitGroupItem()
        {
            return this.IsEditable == true ? true : false;
        }

        protected string GetPriceModifierCents(decimal priceModifier)
        {
            string cents = Decimal.Subtract(Decimal.Parse(priceModifier.ToString("#.00")), Decimal.Floor(priceModifier)).ToString();
            return cents.Substring(cents.IndexOf(".") + 1);
        }

        private void SetKitView()
        {
            //show or hide kit empty row if items exist (or don't)
            phKitListEmpty.Visible = ((this.DisplayMode == DisplayModeValue.View) && (this.ItemCount == 0)) ? true : false;

            //show or hide kit rows row if items exist (or don't)
            phKitList.Visible = (this.DisplayMode == DisplayModeValue.Edit || (this.ItemCount > 0)) ? true : false;

            //set image paths for images in kit view
            this.SetKitImagesPath();

            //set-up modal
            this.SetModal(ViewMode.Kit);

            //set editable fields to render or not
            this.SetKitViewEditable();

            //set localizable strings
            this.SetKitViewLocalizedStrings();

            mvItemType.SetActiveView(vwKit);
        }

        private void SetKitImagesPath()
        {
            //kit edit button icons
            imgKitCancel.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/toggleDelete.gif";
            imgKitOK.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/addItem.gif";
            imgKitMarkForDelete.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/toggleDelete.gif";
            imgKitRestore.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/toggleDeleteUndo.gif";
            imgKitEdit.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/24-tag-pencil.gif";
            imgKitViewModalOk.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/addItem.gif";
            imgKitViewModalCancel.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/toggleDelete.gif";
        }

        private void SetKitViewEditable()
        {
            // NOTE: liKitGroupAddKitItem is inside a nested repeater so is set via GetKitGroupAddKitGroupItem()
            liKitGroupAddKitGroup.Visible = this.IsEditable == true ? true : false;
            liKitGroupKitGroupClone.Visible = this.IsEditable == true ? true : false;
            phModal.Visible = this.IsEditable == true ? true : false;
            pKitGroupEditButtons.Visible = this.IsEditable == true ? true : false;
        }

        #endregion

        #region Default View Helpers

        #region Repeater "Get" Methods

        protected bool GetDefaultDeleteLinkVisibility()
        {
            return (this.IsEditable) ? true : false;
        }

        protected string GetDefaultItemSortTitle()
        {
            return "Drag Item To Sort";
        }

        protected string GetDefaultItemEditClass()
        {
            return (this.IsEditable) ? " editMode" : String.Empty;
        }

        #endregion

        private void SetDefaultImagesPath()
        {
            imgDefaultAddItem.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/addItem.gif";
            imgCloseAddItemModal.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/toggleDelete.gif";
        }

        private void SetDefaultView()
        {
            //hide default empty row if items exist
            liDefaultEmptyRow.Visible = (this.ItemCount > 0) ? false : true;

            //set image paths for images in default view
            this.SetDefaultImagesPath();

            //show add items button if editable
            pDefaultAddItems.Visible = (this.IsEditable == true) ? true : false;

            //show clone item if editable
            liDefaultCloneItem.Visible = (this.IsEditable == true) ? true : false;

            //set-up modal
            this.SetModal(ViewMode.Default);

            //set custom attributes on inline elements
            this.SetDefaultViewElementAttributes();

            //show default view
            mvItemType.SetActiveView(vwDefault);
        }

        private void SetDefaultViewElementAttributes()
        {
            //set title for default view items
            liDefaultCloneItem.Attributes.Add("title", this.GetDefaultItemSortTitle());
        }

        #endregion

        #region Subscription View Helpers

        private void SetSubscriptionView(Object subscriptionInfo)
        {
            //hide modal - any modals are handled in the subscription ascx
            phModal.Visible = false;

            //load subscription control
            UserControl subscriptionControl = LoadControl(this.SubscriptionControlPath) as UserControl;
            subscriptionControl.ID = "controlSubscription";

            IEktronSubscriptionControl iSubscriptionControl;
            try
            {
                //cast as IEktronSubscriptionControl, and set subscription data and editable properties
                iSubscriptionControl = (IEktronSubscriptionControl)subscriptionControl;
                iSubscriptionControl.SubscriptionData = subscriptionInfo;
                iSubscriptionControl.IsEditable = this.IsEditable;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Subscription control must implement interface IEktronSubscriptionControl" + ex.Message);
            }
            //recast as control and add to subscription placeholder
            phSubscription.Controls.Add((Control)iSubscriptionControl);
            
            //show subscription view
            mvItemType.SetActiveView(vwSubscription);
        }

        public void SetClientData(List<Object> clientData)
        {
            _itemData = clientData;
        }

        #endregion

        #region General Helper Methods

        private void DeserializeJsonData(string itemTabJsonData, string itemTabDeserializationMode)
        {
            _itemData = new List<Object>();
			itemTabJsonData = itemTabJsonData.Replace("MarkedForDelete\":\"\"", "MarkedForDelete\":\"false\"");
            switch (itemTabDeserializationMode)
            {
                case "Kit":
                    List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.KitData> kitViewData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.KitData>)JsonConvert.DeserializeObject(itemTabJsonData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.KitData>));
                    foreach (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData i in kitViewData) { _itemData.Add(i); }
                    break;
                case "Bundle":
                    List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.BundleData> bundleViewData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.BundleData>)JsonConvert.DeserializeObject(itemTabJsonData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.BundleData>));
                    foreach (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData i in bundleViewData) { _itemData.Add(i); }
                    break;
                case "Product":
                    List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ProductData> productViewData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ProductData>)JsonConvert.DeserializeObject(itemTabJsonData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ProductData>));
                    foreach (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData i in productViewData) { _itemData.Add(i); }
                    break;
                case "ComplexProduct":
                    List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ComplexProductData> complexProductViewData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ComplexProductData>)JsonConvert.DeserializeObject(itemTabJsonData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ComplexProductData>));
                    foreach (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData i in complexProductViewData) { _itemData.Add(i); }
                    break;
                case "Subscription":
                    //handle subscription deserialization in subscription control
                    break;
            }
        }

        private void SetModal(ViewMode viewMode)
        {
            if (this.IsEditable == true)
            {
                //set modal close button image path
                imgCloseModal.ImageUrl = this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/images/closeButton.gif";

                switch (viewMode)
                {
                    case ViewMode.Default:

                        spanDefaultModalTitle.InnerHtml = "Select Item to Add";
                        spanDefaultModalTitle.Visible = true;

                        //add default view modal iframe src (for adding items)
                        iframeAddItemsModal.Attributes.Add("src", "itemselection.aspx?exclude=" + EntryEditData.Id.ToString() + "&id=" + this.ItemsFolderId.ToString() + @"&SelectedTab=Items");

                        //set default modal content placeholder to true
                        mvItemsModalContent.SetActiveView(vwDefaultModalContent);
                        break;
                    case ViewMode.Kit:
                        //set modal add group header text
                        spanKitModalGroupTitle.InnerHtml = "Enter Group Information";
                        spanKitModalGroupTitle.Visible = true;

                        //set modal add item header text
                        spanKitModalItemTitle.InnerHtml = "Enter Item Information";
                        spanKitModalItemTitle.Visible = true;

                        //set kit modal content placeholder to true
                        mvItemsModalContent.SetActiveView(vwKitModalContent);
                        break;
                }
            }
            else
            {
                //editable is false - do not display modal markup
                phModal.Visible = false;
            }
        }

        #endregion

        #region JS, CSS, Images

        private void RegisterJS()
        {
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/Items/js/Ektron.Commerce.Workarea.Items.js", "EktronCommerceWorkareaItemsJs");
            JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronUISortableJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            if (this.EntryEditData != null)
            {
                if (this.EntryEditData.EntryType == EkEnumeration.CatalogEntryType.Kit)
                {
                    JS.RegisterJS(this, JS.ManagedScript.EktronTreeviewJS);
                }
            }
        }

        private void RegisterCSS()
        {
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            if (this.EntryEditData != null)
            {
                if (this.EntryEditData.EntryType == EkEnumeration.CatalogEntryType.Kit)
                {
                    Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);
                }
            }
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/Ektron.Commerce.Workarea.Items.css", "EktronCommerceWorkareaItemsCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/Items/css/Ektron.Commerce.Workarea.Items.Ie.css", "EktronCommerceWorkareaItemsIeCss", Css.BrowserTarget.AllIE);
        }

        #endregion
    }
}