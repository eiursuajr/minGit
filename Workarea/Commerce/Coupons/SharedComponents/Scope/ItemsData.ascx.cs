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
    public partial class ItemsData : CouponUserControlBase, ICouponUserControl
    {
        #region Member Variables

        private CouponScope _Scope;
        private string _CatalogLabel;
        private string _ProductLabel;
        private string _CategoryLabel;
        private ICoupon _CouponService;
        private EkMessageHelper m_refMsg;
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
        public string CatalogLabel
        {
            get
            {
                return _CatalogLabel;
            }
            set
            {
                _CatalogLabel = value;
            }
        }
        public string ProductLabel
        {
            get
            {
                return _ProductLabel;
            }
            set
            {
                _ProductLabel = value;
            }
        }
        public string CategoryLabel
        {
            get
            {
                return _CategoryLabel;
            }
            set
            {
                _CategoryLabel = value;
            }
        }

        #endregion

        #region Events

        #region PageEvents

        protected void Page_Init(object sender, EventArgs e)
        {
            // init private variables
            SiteAPI refSiteApi = new SiteAPI();
            m_refMsg = refSiteApi.EkMsgRef;

            _CouponApi.RequestInformationRef.ContentLanguage = Utilities.GetLanguageId(_ContentApi);

            //make clone row visible if in edit mode
            phCloneRow.Visible = this.IsEditable == true ? true : false;

            //set mode - used for setting labels
            hdnScope.Value = Enum.GetName(typeof(CouponScope), this.Scope);

            //register page components
            this.RegisterCSS();
            this.RegisterJS();

            //set no items row attributes
            tdNoItems.Attributes.Add("colspan", this.IsEditable == true ? "5" : "4");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.CouponPublishedData != null)
            {
                List<ProductCouponEntryData> productList = this.GetProductList();
                List<CouponEntryData> catalogList = this.GetCatalogList();
                List<CouponEntryData> taxonomyList = this.GetTaxonomyList();

                if (productList.Count > 0 || catalogList.Count > 0 || taxonomyList.Count > 0)
                {
                    trNoItems.Attributes.Add("style", "display:none;");
                }

                //bind coupon list data to repeater
                rptPublishedItemsList.DataSource = GetDataView(productList, catalogList, taxonomyList);
                rptPublishedItemsList.DataBind();
            }

            hdnLanguageId.Value = _CouponApi.RequestInformationRef.ContentLanguage.ToString();

            //set localized strings
            this.SetLocalizedControlText();
        }

        protected void rptPublishedItemsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            phMarkForDeleteHeader.Visible = this.IsEditable;
            ((PlaceHolder)e.Item.FindControl("phMarkForDeleteTableCell")).Visible = this.IsEditable;
            DataRowView row = (DataRowView)e.Item.DataItem;
            EkEnumeration.CMSObjectTypes type = (EkEnumeration.CMSObjectTypes)Enum.Parse(typeof(EkEnumeration.CMSObjectTypes), Convert.ToString(DataBinder.Eval(row, m_refMsg.GetMessage("generic type"))));
            long id = long.Parse(Convert.ToString(DataBinder.Eval(row, m_refMsg.GetMessage("lbl cc type id"))));
            ((Literal)e.Item.FindControl("litObjectType")).Text = this.GetTypeFriendlyName(type, id);
        }

        #endregion

        #endregion

        #region ClientData

        public Object GetClientData()
        {
            List<ItemsDataClientData> itemsDataClientData;

            //populate itemsDataClientData with items that the user has either added or marked for delete (or both)
            itemsDataClientData = (List<ItemsDataClientData>)JsonConvert.DeserializeObject(hdnData.Value, typeof(List<ItemsDataClientData>));

            //if itemsDataClientData is null, this means that either...
            //(1) The user has not added any items to the list, or
            //(2) The user has not initialized this list by viewing published items (in the case of Properties)
            //if itemsDataClientData is null, and CouponPublishedData exists, we must return the published items
            if (itemsDataClientData == null && this.CouponPublishedData != null)
            {
                itemsDataClientData = new List<ItemsDataClientData>();
                List<ProductCouponEntryData> productList = this.GetProductList();  //get published products
                List<CouponEntryData> catalogList = this.GetCatalogList(); //get published catalogs
                List<CouponEntryData> taxonomyList = this.GetTaxonomyList(); //get published taxonomy nodes

                ItemsDataClientData publishedItemsData;
                foreach (ProductCouponEntryData product in productList)
                {
                    publishedItemsData = new ItemsDataClientData();
                    publishedItemsData.Id = product.ObjectId;
                    publishedItemsData.Type = "product";
                    publishedItemsData.MarkedForDelete = false;
                    itemsDataClientData.Add(publishedItemsData);
                }
                foreach (CouponEntryData catalog in catalogList)
                {
                    publishedItemsData = new ItemsDataClientData();
                    publishedItemsData.Id = catalog.ObjectId;
                    publishedItemsData.Type = "catalog";
                    publishedItemsData.MarkedForDelete = false;
                    itemsDataClientData.Add(publishedItemsData);
                }
                foreach (CouponEntryData taxonomy in taxonomyList)
                {
                    publishedItemsData = new ItemsDataClientData();
                    publishedItemsData.Id = taxonomy.ObjectId;
                    publishedItemsData.Type = "taxonomy";
                    publishedItemsData.MarkedForDelete = false;
                    itemsDataClientData.Add(publishedItemsData);
                }
            }
            return itemsDataClientData;
        }

        #endregion

        #region Helpers

        private List<ProductCouponEntryData> GetProductList()
        {
            //instantiate coupon service field
            _CouponService = _CouponService == null ? new Coupon(_CouponApi.RequestInformationRef) : _CouponService;

            // products
            Criteria<CouponEntryProperty> productCriteria = new Criteria<CouponEntryProperty>();
            productCriteria.AddFilter(CouponEntryProperty.CouponId, CriteriaFilterOperator.EqualTo, this.CouponPublishedData.Id);
            productCriteria.AddFilter(CouponEntryProperty.IsIncluded, CriteriaFilterOperator.EqualTo, this.Scope == CouponScope.Exclude ? false : true);
            List<ProductCouponEntryData> productList = _CouponService.GetProductList(productCriteria);

            return productList;
        }

        private List<CouponEntryData> GetTaxonomyList()
        {
            //instantiate coupon service field
            _CouponService = _CouponService == null ? new Coupon(_CouponApi.RequestInformationRef) : _CouponService;

            // taxonomies
            Criteria<CouponEntryProperty> taxonomyCriteria = new Criteria<CouponEntryProperty>();
            taxonomyCriteria.AddFilter(CouponEntryProperty.CouponId, CriteriaFilterOperator.EqualTo, this.CouponPublishedData.Id);
            taxonomyCriteria.AddFilter(CouponEntryProperty.IsIncluded, CriteriaFilterOperator.EqualTo, this.Scope == CouponScope.Exclude ? false : true);
            List<CouponEntryData> taxonomyList = _CouponService.GetTaxonomyList(taxonomyCriteria);

            return taxonomyList;
        }

        private List<CouponEntryData> GetCatalogList()
        {
            //instantiate coupon service field
            _CouponService = _CouponService == null ? new Coupon(_CouponApi.RequestInformationRef) : _CouponService;

            // catalogs
            Criteria<CouponEntryProperty> catalogCriteria = new Criteria<CouponEntryProperty>();
            catalogCriteria.AddFilter(CouponEntryProperty.CouponId, CriteriaFilterOperator.EqualTo, this.CouponPublishedData.Id);
            catalogCriteria.AddFilter(CouponEntryProperty.IsIncluded, CriteriaFilterOperator.EqualTo, this.Scope == CouponScope.Exclude ? false : true);
            List<CouponEntryData> catalogList = _CouponService.GetCatalogList(catalogCriteria);

            return catalogList;
        }

        public string GetType(Object e)
        {
            EkEnumeration.CMSObjectTypes objectType = (EkEnumeration.CMSObjectTypes)e;
            string type = String.Empty;
            switch (objectType)
            {
                case EkEnumeration.CMSObjectTypes.CatalogEntry:
                    type = "product";
                    break;
                case EkEnumeration.CMSObjectTypes.TaxonomyNode:
                    type = "taxonomy";
                    break;
                case EkEnumeration.CMSObjectTypes.Folder:
                    type = "catalog";
                    break;
            }
            return type;
        }

        public int GetTypeCode(Object e)
        {
            EkEnumeration.CMSObjectTypes objectType = (EkEnumeration.CMSObjectTypes)e;
            return objectType.GetHashCode();
        }

        public DataView GetDataView(List<ProductCouponEntryData> productList, List<CouponEntryData> catalogList, List<CouponEntryData> taxonomyList)
        {

            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();

            dt.Columns.Add(new DataColumn(m_refMsg.GetMessage("lbl cc type id"), typeof(String)));
            dt.Columns.Add(new DataColumn(m_refMsg.GetMessage("lbl cc type name"), typeof(String)));
            dt.Columns.Add(new DataColumn(m_refMsg.GetMessage("lbl path"), typeof(String)));
            dt.Columns.Add(new DataColumn(m_refMsg.GetMessage("generic type"), typeof(EkEnumeration.CMSObjectTypes)));

            for (int i = 0; i < productList.Count; i++)
            {

                dr = dt.NewRow();
                dr[0] = productList[i].ObjectId;
                dr[1] = productList[i].Title;
                dr[2] = productList[i].Path;
                dr[3] = productList[i].ObjectType;
                dt.Rows.Add(dr);

            }
            for (int i = 0; i < catalogList.Count; i++)
            {

                dr = dt.NewRow();
                dr[0] = catalogList[i].ObjectId;
                dr[1] = catalogList[i].Title;
                dr[2] = catalogList[i].Path;
                dr[3] = catalogList[i].ObjectType;
                dt.Rows.Add(dr);

            }
            for (int i = 0; i < taxonomyList.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = taxonomyList[i].ObjectId;
                dr[1] = taxonomyList[i].Title;
                dr[2] = taxonomyList[i].Path;
                dr[3] = taxonomyList[i].ObjectType;
                dt.Rows.Add(dr);
            }
            return new DataView(dt);

        }

        public string GetStripeRow(int index)
        {
            return index % 2 == 0 ? String.Empty : "stripe";
        }

        #endregion

        #region Localized Strings

        private void SetLocalizedControlText()
        {
            litNoItems.Text = m_refMsg.GetMessage("no items have been selected");

            //sorting
            litId.Text = this.GetLocalizedStringIdHeader();
            litName.Text = this.GetLocalizedStringNameHeader();
            litPath.Text = this.GetLocalizedStringPathHeader();
            litType.Text = this.GetLocalizedStringTypeHeader();
        }

        public string GetLocalizedStringMarkForDelete()
        {
            return m_refMsg.GetMessage("lbl coupon mark delete");
        }
        public string GetLocalizedStringMarkForDeleteAll()
        {
            return m_refMsg.GetMessage("lbl coupon mark all page delete");
        }
        public string GetLocalizedStringRestore()
        {
            return m_refMsg.GetMessage("generic restore title");
        }
        public string GetLocalizedStringRestoreAll()
        {
            return m_refMsg.GetMessage("lbl coupon restore all on page");
        }
        public string GetLocalizedStringCouponRowTitle()
        {
            return m_refMsg.GetMessage("lbl Click view coupon properties");
        }
        public string GetLocalizedStringAddItem()
        {
            return m_refMsg.GetMessage("generic add item");
        }
        public string GetLocalizedStringProductButton()
        {
            return this.ProductLabel;
        }
        public string GetLocalizedStringCatalogButton()
        {
            return this.CatalogLabel;
        }
        public string GetLocalizedStringCategoryButton()
        {
            return this.CategoryLabel;
        }
        public string GetLocalizedStringIdHeader()
        {
            return m_refMsg.GetMessage("lbl cc type id");
        }
        public string GetLocalizedStringNameHeader()
        {
            return m_refMsg.GetMessage("lbl cc type name");
        }
        public string GetLocalizedStringPathHeader()
        {
            return m_refMsg.GetMessage("lbl path");
        }
        public string GetLocalizedStringTypeHeader()
        {
            return m_refMsg.GetMessage("generic type");
        }
        public string GetLocalizedStringCloseModal()
        {
            return m_refMsg.GetMessage("close title");
        }
        public string GetLocalizedStringYes()
        {
            return m_refMsg.GetMessage("lbl coupon yes");
        }
        public string GetLocalizedStringNo()
        {
            return m_refMsg.GetMessage("lbl coupon no");
        }
        public string GetTypeFriendlyName(EkEnumeration.CMSObjectTypes type, long id)
        {
            string typeName = String.Empty;
            switch (type)
            {
                case EkEnumeration.CMSObjectTypes.TaxonomyNode:
                    typeName = "Taxonomy";
                    break;
                case EkEnumeration.CMSObjectTypes.Folder:
                    typeName = "Catalog";
                    break;
                case EkEnumeration.CMSObjectTypes.CatalogEntry:
                    EntryData entryData;
                    Commerce.CatalogEntryApi catalogEntryApi = new Commerce.CatalogEntryApi();
                    entryData = catalogEntryApi.GetItem(id);
                    switch (entryData.EntryType)
                    {
                        case EkEnumeration.CatalogEntryType.Bundle:
                            typeName = "Bundle";
                            break;
                        case EkEnumeration.CatalogEntryType.ComplexProduct:
                            typeName = "Complex Product";
                            break;
                        case EkEnumeration.CatalogEntryType.Kit:
                            typeName = "Kit";
                            break;
                        case EkEnumeration.CatalogEntryType.Product:
                            typeName = "Product";
                            break;
                        case EkEnumeration.CatalogEntryType.SubscriptionProduct:
                            typeName = "Subscription";
                            break;
                    }
                    break;
            }
            return typeName;
        }

        #endregion

        #region JS, CSS

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/css/itemsData.css", @"EktronCommerceCouponsItemsDataCss");
            Css.RegisterCss(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/css/itemsData.ie.css", @"EktronCommerceCouponsItemsDataIeCss", Css.BrowserTarget.LessThanEqualToIE7);
        }
        protected override void RegisterJS()
        {
            base.RegisterJS();
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + @"/Commerce/Coupons/SharedComponents/Scope/js/itemsData.js", @"EktronCommerceCouponsItemsDataJs");
        }
        public string GetItemsImagesPath()
        {
            return base.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/css/images";
        }

        #endregion
    }
}
