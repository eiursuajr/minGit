using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
using Ektron.Cms.API;

namespace Ektron.Cms.Commerce.Workarea.Coupons
{
    public partial class SelectFolderProduct : workareabase
    {


        #region Enumerations

        private enum Mode
        {
            Folder,
            Product
        }

        #endregion

        #region Variables

        private Folder _FolderApi;
        private bool _IsProductSelected;
        private Mode _Mode;
        private long _FolderId;
        private string[] _SelectedFolderIds;
        private List<long> _SelectedFolderList = new List<long>();
        private string[] _SelectedProductIds;
        private List<long> _SelectedProductList = new List<long>();

        #endregion

        #region Page Functions

        public SelectFolderProduct()
        {
            _FolderApi = new Ektron.Cms.API.Folder();
            _FolderId = 0;
            _IsProductSelected = false;
            _Mode = Mode.Folder;
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {

            try
            {

                //ensure commerce permissions
                CheckAccess();
                CommerceLibrary.CheckCommerceAdminAccess();

                //set page not to cache
                System.Web.HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
                System.Web.HttpContext.Current.Response.Expires = 0;
                System.Web.HttpContext.Current.Response.Cache.SetNoStore();
                System.Web.HttpContext.Current.Response.AddHeader("Pragma", "no-cache");

                string[] defaultId = new string[] { "" };
                if (this.IsPostBack)
                {
                    // recover previously checked items as we traverse tree
                    _SelectedFolderIds = (Request.Form["hdnFolderList"] == null) ? defaultId : (Strings.Split(Request.Form["hdnFolderList"], ",", -1, 0));
                    _SelectedProductIds = (Request.Form["hdnProductList"] == null) ? defaultId : (Strings.Split(Request.Form["hdnProductList"], ",", -1, 0));
                }
                else
                {
                    // first time load - initialize
                    if (Request.QueryString["mode"] == "catalog")
                    {
                        InitDataForFolders();
                    }
                    else
                    {
                        InitDataForProducts();
                    }
                }

                if (Request.Form["hdnFolderId"] == null)
                {
                    _FolderId = 0;
                }
                else
                {
                    _FolderId = long.Parse(Request.Form["hdnFolderId"]);
                }

                _Mode = (Request.QueryString["mode"] == "catalog") ? Mode.Folder : Mode.Product;

                //register page components
                this.RegisterJs();
                this.RegisterCss();

            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }

        protected override void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                //get and display folders
                GetFolders();
                DisplayFolders();

                //get and display products if in product mode
                if (_Mode == Mode.Product)
                {
                    GetProducts();
                    DisplayProducts();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }

        #endregion

        #region Folder

        protected void GetFolders()
        {
            long folderIdLong;
            bool convertsToLong = false;

            if (this._SelectedFolderIds != null)
            {
                foreach (string folderIdString in this._SelectedFolderIds)
                {
                    convertsToLong = long.TryParse(folderIdString, out folderIdLong);
                    if (convertsToLong)
                    {
                        this._SelectedFolderList.Add(folderIdLong);
                    }
                }
            }
        }

        private void DisplayFolders()
        {
            this.uxPaging.Visible = false;

            //get folderdata for selected folder
            FolderData curentFolderData = m_refContentApi.GetFolderById(_FolderId);

            //prepare databind data
            //(1) populate subfolder folderdata array with select folders's child folder's folderdata
            FolderData[] subFolders = m_refContentApi.GetChildFolders(_FolderId, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);

            //(2) if currenct folder is not zero (root), update databind data
            //add "go back" node as topmost/index-zero/first node in array
            if (curentFolderData.Id > 0)
            {
                FolderData goBack = new FolderData();
                List<FolderData> folderList = new List<FolderData>();

                goBack.Id = curentFolderData.ParentId;
                goBack.Name = "...";

                if (subFolders != null)
                {
                    folderList.AddRange(subFolders);
                }
                folderList.Insert(0, goBack);

                subFolders = folderList.ToArray();
            }

            //(3) databind to subfolders folderdata array
            gvFolders.DataSource = subFolders;
            gvFolders.DataBind();

            //set label with current folder's name
            litCurrentFolderName.Text = this.GetMessage("current folder")+": " + curentFolderData.Name;

        }

        protected void gvFolders_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {

            Ektron.Cms.FolderData folderData;
            string path;
            HtmlInputControl idControl;
            HtmlInputControl nameControl;
            HtmlInputControl pathControl;
            HtmlInputCheckBox checkboxControl;
            HtmlImage imageControl;
            HtmlAnchor linkControl;

            folderData = e.Row.DataItem as Ektron.Cms.FolderData;

            if (folderData != null)
            {
                //set "go up one level" tooltip text
                string title = (string)(folderData.Name == "..." ? (this.GetGoUpLevelMessage()) : folderData.Name);

                idControl = e.Row.FindControl("hdnFolderId") as HtmlInputControl;
                if (idControl != null)
                {
                    idControl.Value = folderData.Id.ToString();
                }

                nameControl = e.Row.FindControl("hdnFolderName") as HtmlInputControl;
                if (nameControl != null)
                {
                    nameControl.Value = folderData.Name;
                }

                path = _FolderApi.GetPath(folderData.Id);
                pathControl = e.Row.FindControl("hdnFolderPath") as HtmlInputControl;
                if (pathControl != null)
                {
                    pathControl.Value = path;
                }

                imageControl = e.Row.FindControl("imgFolderIcon") as HtmlImage;
                if (imageControl != null)
                {
                    imageControl.Src = GetFolderIcon(folderData.Id, (EkEnumeration.FolderType)folderData.FolderType, folderData.ParentId);
                    imageControl.Alt = folderData.Name;
                    imageControl.Attributes.Add("title", title);
                    imageControl.Attributes.Add("style", "cursor:pointer");
                    imageControl.Attributes.Add("onclick", "Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.item(this, " + folderData.Id.ToString() + ", 0);return false;");
                }

                checkboxControl = e.Row.FindControl("cbxFolder") as HtmlInputCheckBox;
                if (checkboxControl != null)
                {
                    checkboxControl.Disabled = System.Convert.ToBoolean((IsFolderDisabled((EkEnumeration.FolderType)folderData.FolderType) == true) ? true : false);
                    if (IsFolderDisabled((EkEnumeration.FolderType)folderData.FolderType))
                    {
                        checkboxControl.Style["display"] = "none";
                    }
                    checkboxControl.Checked = System.Convert.ToBoolean((IsFolderSelected(folderData.Id) == true) ? true : false);
                }

                linkControl = e.Row.FindControl("aFolder") as HtmlAnchor;
                if (linkControl != null)
                {
                    linkControl.Attributes.Add("onclick", GetFolderOnclick(folderData.Id));
                    linkControl.Attributes.Add("class", GetFolderCssClass((EkEnumeration.FolderType)folderData.FolderType));
                    linkControl.Title = title;
                    linkControl.InnerText = folderData.Name;
                }
            }
        }

        protected string GetFolderOnclick(long folderId)
        {
            if (this._IsProductSelected)
            {
                return "Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.item(this, " + folderId.ToString() + ", 0);return false;";
            }
            else
            {
                return "Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.item(this, " + folderId.ToString() + ", 1);return false;";
            }
        }

        protected string GetFolderCssClass(EkEnumeration.FolderType folderType)
        {
            string CssClass = string.Empty;
            if (folderType == EkEnumeration.FolderType.Catalog)
            {
                CssClass = "catalogFolder";
            }
            return CssClass;
        }

        protected bool IsFolderDisabled(EkEnumeration.FolderType folderType)
        {
            bool disabled = true;
            if (folderType == EkEnumeration.FolderType.Catalog && _Mode == Mode.Folder)
            {
                disabled = false;
            }
            return disabled;
        }

        protected bool IsFolderSelected(long folderId)
        {
            return _SelectedFolderList.Contains(folderId);
        }

        protected string GetFolderIcon(long folderId, EkEnumeration.FolderType folderType, long parentId)
        {
            if (folderId == parentId)
            {
                return GetAppImgPath() + "folderbackup_1.gif";
            }
            else
            {
                if (folderType == EkEnumeration.FolderType.DiscussionBoard)
                {
                    return GetAppImgPath() + "menu/users2.gif";
                }
                else if (folderType == EkEnumeration.FolderType.Blog)
                {
                    return GetAppImgPath() + "menu/pen_blue.gif";
                }
                else if (folderType == EkEnumeration.FolderType.Community)
                {
                    return GetAppImgPath() + "menu/house2.gif";
                }
                else if (folderType == EkEnumeration.FolderType.Catalog)
                {
                    return GetAppImgPath() + "commerce/catalogclosed_1.gif";
                }
                else
                {
                    return GetAppImgPath() + "folderclosed_1.gif";
                }
            }
        }

        #endregion

        #region Products

        protected void GetProducts()
        {
            long productIdLong;
            bool convertsToLong = false;

            if (this._SelectedProductIds != null)
            {
                foreach (string productIdString in this._SelectedProductIds)
                {
                    convertsToLong = long.TryParse(productIdString, out productIdLong);
                    if (convertsToLong)
                    {
                        this._SelectedProductList.Add(productIdLong);
                    }
                }
            }
        }

        private void DisplayProducts()
        {

            CatalogEntryApi CatalogManager = new CatalogEntryApi();
            System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
            Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();

            entryCriteria.PagingInfo.RecordsPerPage = this._FolderApi.EkContentRef.RequestInformation.PagingSize;
            entryCriteria.PagingInfo.CurrentPage = this.uxPaging.SelectedPage + 1;
            entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, this._FolderId);
            entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, long.Parse(Request.QueryString["languageId"]));
            entryList = CatalogManager.GetList(entryCriteria);

            if (entryCriteria.PagingInfo.TotalPages > 1)
            {
                this.uxPaging.Visible = true;
                this.uxPaging.TotalPages = entryCriteria.PagingInfo.TotalPages;
                this.uxPaging.CurrentPageIndex = this.uxPaging.SelectedPage;
            }
            else
            {
                this.uxPaging.Visible = false;
            }

            gvProducts.DataSource = entryList;
            gvProducts.DataBind();

        }

        protected void gvProducts_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {

            Ektron.Cms.Commerce.EntryData entryData;
            string path;
            HtmlInputControl idControl;
            HtmlInputControl nameControl;
            HtmlInputControl pathControl;
            HtmlInputControl subTypeControl;
            HtmlInputCheckBox checkboxControl;
            HtmlImage imageControl;
            HtmlGenericControl titleControl;

            entryData = e.Row.DataItem as Ektron.Cms.Commerce.EntryData;
            if (entryData != null)
            {
                idControl = e.Row.FindControl("hdnProductId") as HtmlInputControl;
                if (idControl != null)
                {
                    idControl.Value = entryData.Id.ToString();
                }

                nameControl = e.Row.FindControl("hdnProductName") as HtmlInputControl;
                if (nameControl != null)
                {
                    nameControl.Value = entryData.Title;
                }

                path = _FolderApi.GetPath(entryData.FolderId);
                pathControl = e.Row.FindControl("hdnProductPath") as HtmlInputControl;
                if (pathControl != null)
                {
                    pathControl.Value = path;
                }

                subTypeControl = e.Row.FindControl("hdnProductSubType") as HtmlInputControl;
                if (subTypeControl != null)
                {
                    subTypeControl.Value = this.GetProductTypeName(entryData.EntryType);
                }

                checkboxControl = e.Row.FindControl("cbxProduct") as HtmlInputCheckBox;
                if (checkboxControl != null)
                {
                    checkboxControl.Checked = System.Convert.ToBoolean((IsProductSelected(entryData.Id) == true) ? true : false);
                }

                imageControl = e.Row.FindControl("imgProduct") as HtmlImage;
                if (imageControl != null)
                {
                    imageControl.Src = GetProductIcon(entryData.EntryType);
                    imageControl.Alt = entryData.Title;
                    imageControl.Attributes.Add("title", entryData.Title);
                }

                titleControl = e.Row.FindControl("spanProduct") as HtmlGenericControl;
                if (titleControl != null)
                {
                    titleControl.InnerText = entryData.Title;
                }
            }
        }

        protected bool IsProductSelected(long entryId)
        {
            return _SelectedProductList.Contains(entryId);
        }

        protected string GetProductIcon(EkEnumeration.CatalogEntryType entryType)
        {
            string productImage;
            if (entryType == Common.EkEnumeration.CatalogEntryType.Bundle)
            {
                productImage = m_refContentApi.ApplicationPath + "images/ui/icons/package.png";
            }
            else if (entryType == Common.EkEnumeration.CatalogEntryType.ComplexProduct)
            {
                productImage = m_refContentApi.ApplicationPath + "images/ui/icons/bricks.png";
            }
            else if (entryType == Common.EkEnumeration.CatalogEntryType.Kit)
            {
                productImage = m_refContentApi.ApplicationPath + "images/ui/icons/box.png";
            }
            else if (entryType == Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {
                productImage = m_refContentApi.ApplicationPath + "images/ui/icons/bookGreen.png";
            }
            else
            {
                productImage = m_refContentApi.ApplicationPath + "images/ui/icons/brick.png";
            }

            return productImage;
        }

        #endregion

        #region Helpers

        protected void CheckAccess()
        {
            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
            {
                throw (new Exception(GetMessage("feature locked error")));
            }

        }

        protected string GetAppImgPath()
        {
            return m_refContentApi.AppImgPath;
        }


        protected void InitDataForFolders()
        {
            hdnFolderList.Value = Request.QueryString["idlist"];
            _SelectedFolderIds = Request.QueryString["idlist"].Split(",".ToCharArray());

            string json = "";
            long folderIdLong;
            FolderData fd;

            if (this._SelectedFolderIds != null)
            {
                foreach (string folderIdString in this._SelectedFolderIds)
                {
                    if (long.TryParse(folderIdString, out folderIdLong))
                    {
                        fd = m_refContentApi.GetFolderById(folderIdLong);
                        json += (string)(json.Length > 0 ? "," : "[");
                        json += "{\"Id\":\"" + fd.Id.ToString() + "\",\"Name\":\"" + fd.Name + "\",\"Path\":\"" + fd.NameWithPath.TrimEnd("/".ToCharArray()).Replace("/", "\\\\") + "\",\"Type\":\"catalog\",\"SubType\":\"catalog\",\"TypeCode\":\"1\",\"MarkedForDelete\":\"false\",\"NewlyAdded\":\"false\"}";
                    }
                }
            }

            json += (string)(json.Length > 0 ? "]" : "");
            hdnData.Value = json;
        }

        protected void InitDataForProducts()
        {
            hdnProductList.Value = Request.QueryString["idlist"];
            _SelectedProductIds = Request.QueryString["idlist"].Split(",".ToCharArray());

            string json = "";
            long contentIdLong;
            ContentData cd;
            EntryData entryData;
            Ektron.Cms.Commerce.CatalogEntryApi catalogEntryApi = new Ektron.Cms.Commerce.CatalogEntryApi();
            int origLang = m_refContentApi.ContentLanguage;

            try
            {
                if (this._SelectedProductIds != null)
                {
                    m_refContentApi.ContentLanguage = Utilities.GetLanguageId(m_refContentApi);
                    foreach (string contentIdString in this._SelectedProductIds)
                    {
                        if (long.TryParse(contentIdString, out contentIdLong))
                        {
                            cd = m_refContentApi.GetContentById(contentIdLong, ContentAPI.ContentResultType.Published);
                            entryData = catalogEntryApi.GetItem(contentIdLong);
                            json += (string)(json.Length > 0 ? "," : "[");
                            json += "{\"Id\":\"" + cd.Id.ToString() + "\",\"Name\":\"" + cd.Title + "\",\"Path\":\"" + cd.Path.TrimEnd("/".ToCharArray()).Replace("\\", "\\\\").Replace("/", "\\\\") + "\",\"Type\":\"product\",\"SubType\":\"" + GetProductTypeName(entryData.EntryType) + "\",\"TypeCode\":\"0\",\"MarkedForDelete\":\"false\",\"NewlyAdded\":\"false\"}";
                        }
                    }
                }

            }
            finally
            {
                m_refContentApi.ContentLanguage = origLang;
            }

            json += (string)(json.Length > 0 ? "]" : "");
            hdnData.Value = json;
        }

        #endregion

        #region Localized Strings

        public string GetLocalizedJavascriptStrings()
        {
            string selectedFolderMessage;
            string selectedProductMessage;

            selectedFolderMessage = "You have selected this folder and all its decendants.  To select among this folder&#39;s descendants, you must first unselect this folder.";
            selectedProductMessage = "This item has no children.";

            return "{\"selectedFolderClickMessage\": \"" + selectedFolderMessage + "\", \"selectedProductClickMessage\": \"" + selectedProductMessage + "\"}";
        }

        public string GetGoUpLevelMessage()
        {
            return this.GetMessage("go up one level");
        }

        public string GetProductTypeName(EkEnumeration.CatalogEntryType subType)
        {
            string localizedType = "";
            if (subType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
            {
                localizedType = "Bundle";
            }
            else if (subType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
            {
                localizedType = "Complex Product";
            }
            else if (subType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
            {
                localizedType = "Kit";
            }
            else if (subType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product)
            {
                localizedType = "Product";
            }
            else if (subType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {
                localizedType = "Subscription";
            }
            return localizedType;
        }
        #endregion

        #region Css, Js

        private void RegisterCss()
        {
            Ektron.Cms.API.Css.RegisterCss(this, (string)(this.m_refContentApi.ApplicationPath.TrimEnd("/".ToCharArray()) + "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/css/selectFolderProduct.css"), "EktronCommerceCouponsScopeItemsSelectFolderProductCss");
        }

        private void RegisterJs()
        {
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
            Ektron.Cms.API.JS.RegisterJS(this, (string)(this.m_refContentApi.ApplicationPath.TrimEnd("/".ToCharArray()) + "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/js/selectFolderProduct.js"), "EktronCommerceCouponsScopeItemsSelectFolderProductJs");
        }

        #endregion

    }


}
