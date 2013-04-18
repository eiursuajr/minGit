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
using System.Collections.Specialized;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
using Ektron.Cms.Commerce.Workarea;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons;
using Ektron.Newtonsoft.Json;

namespace Ektron.Cms.Commerce.Workarea.Coupons
{
    public partial class selectTaxonomy : workareabase, ICallbackEventHandler
    {
        #region Variables

        protected long _iFolder = -1;
        protected string _TaxonomyTreeIdList = "";
        protected string _TaxonomyTreeParentIdList = "";
        protected bool _TaxonomyRoleExists = false;
        protected long TaxonomyOverrideId = 0;
        protected List<long> _selectedTaxonomyList = new List<long>();
        private string _CallbackResult;

        #endregion

        #region Page Functions

        public selectTaxonomy()
        {
            _CallbackResult = String.Empty;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //set page not to cache
            System.Web.HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.CommerceLibrary.CheckCommerceAdminAccess();
            try
            {
                if (!this.IsPostBack)
                {
                    base.Page_Load(sender, e);
                    // init ui
                    PreSelectTaxonomies();
                    DisplayTaxonomy();
                    this.hdnUniqueId.Value = this.UniqueID;
                    _TaxonomyRoleExists = m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator,
                        m_refContentApi.RequestInformationRef.UserId, false);
                    this.RegisterCss();
                    this.RegisterJs();

                    ValidateAccess();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }

        #endregion

        #region Helpers

        private void DisplayTaxonomy()
        {
            //TaxonomyBaseData[] taxonomy_cat_arr = null;
            int languageId = Convert.ToInt32(Request.QueryString["languageId"]);
            m_refContentApi.ContentLanguage
                = languageId
                = ContentLanguage;

            if ((_selectedTaxonomyList.Count > 0))
            {
                for (int i = 0; i <= (_selectedTaxonomyList.Count - 1); i++)
                {
                    taxonomyselectedtree.Value += ((taxonomyselectedtree.Value.Length > 0) ? "," : "") + Convert.ToString(_selectedTaxonomyList[i]);
                }
            }
            _TaxonomyTreeIdList = taxonomyselectedtree.Value;
        }

        protected void PreSelectTaxonomies()
        {
            string json = String.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["idlist"]))
            {
                long id;
                string[] idList = Request.QueryString["idlist"].Split(',');
                for (int I = 0; I <= (idList.Length - 1); I++)
                {
                    if (long.TryParse(idList[I], out id))
                    {
                        _selectedTaxonomyList.Add(id);
                        json += ((json.Length > 0) ? "," : "") + GetTaxonomyJson(id, false);
                    }
                }
            }
            this.hdnData.Value = "[" + json + "]";
        }

        protected void ValidateAccess()
        {
            if ((!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)))
            {
                throw new Exception(GetMessage("feature locked error"));
            }
        }

        public string GetAppImgPath()
        {
            return m_refContentApi.AppImgPath;
        }

        #endregion

        #region Css, Js


        private void RegisterCss()
        {
            Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/Tree/css/com.Ektron.ui.tree.css", "EktronTreeCss");
        }

        private void RegisterJs()
        {
            Ektron.Cms.API.JS.RegisterJS(this, API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, API.JS.ManagedScript.EktronJsonJS, false);
            Ektron.Cms.API.JS.RegisterJS(this, API.JS.ManagedScript.EktronModalJS, false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/js/selectTaxonomy.js", "EktronCommerceCouponselectTaxonomyJs", false);

            //Tree Js
            // TODO: research whats really needed from the following...
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" + "default" + "&entrytype=" + Common.EkEnumeration.CatalogEntryType.Product + "&folder_id=" + this._iFolder, "Ektron_CatalogEntry_PageFunctions_Js", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.A.aspx?folderId=" + _iFolder.ToString() + "&taxonomyOverrideId=" + TaxonomyOverrideId.ToString() + "&taxonomyTreeIdList=" + EkFunctions.UrlEncode(_TaxonomyTreeIdList) + "&taxonomyTreeParentIdList=" + EkFunctions.UrlEncode(_TaxonomyTreeParentIdList), "Ektron_CatalogEntry_Taxonomy_A_Js", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.B.aspx?suppress_menu=true&showTaxonomy=" + _TaxonomyRoleExists.ToString() + "&taxonomyFolderId=" + _iFolder.ToString(), "Ektron_CatalogEntry_Taxonomy_B_Js", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.url.js", "EktronTreeUtilsUrlJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.init.js", "EktronTreeExplorerInitJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.js", "EktronTreeExplorerJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.config.js", "EktronTreeExplorerConfigJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.explorer.windows.js", "EktronTreeExplorerWindowsJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.types.js", "EktronTreeCmsTypesJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.parser.js", "EktronTreeCmsParserJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.toolkit.js", "EktronTreeCmsToolkitJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.cms.api.js", "EktronTreeCmsApiJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.contextmenu.js", "EktronTreeUiContextMenuJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.iconlist.js", "EktronTreeUiIconListJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.tabs.js", "EktronTreeUiTabsJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.explore.js", "EktronTreeUiExploreJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.ui.taxonomytree.js", "EktronTreeUiTaxonomyTreeJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.net.http.js", "EktronTreeNetHttpJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.lang.exception.js", "EktronTreeLanguageExceptionJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.form.js", "EktronTreeUtilsFormJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.log.js", "EktronTreeUtilsLogJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.dom.js", "EktronTreeUtilsDomJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.debug.js", "EktronTreeUtilsDebugJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.string.js", "EktronTreeUtilsStringJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.cookie.js", "EktronTreeUtilsCookieJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.Ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.C.js", "Ektron_CatalogEntry_Taxonomy_C_Js", false);
        }

        #endregion

        #region ICallbackEventHandler

        public string GetCallbackResult()
        {
            return (_CallbackResult);
        }

        public void RaiseCallbackEvent(string eventArgs)
        {
            NameValueCollection postBackData = HttpUtility.ParseQueryString(eventArgs);
            string taxid = postBackData["taxid"];
            long id;
            if (!String.IsNullOrEmpty(taxid) && long.TryParse(taxid, out id))
                _CallbackResult = GetTaxonomyJson(id, true);
        }

        protected string GetTaxonomyJson(long id, bool newlyAddedFlag)
        {
            // format: {"Id":"123",
            //          "Name":"Some Product",
            //          "Path":"\\CMS400Demo\\Furniture",
            //          "Type":"product",
            //          "SubType":"product",
            //          "TypeCode":"0",
            //          "MarkedForDelete":"false",
            //          "NewlyAdded":"false"}

            TaxonomyRequest tr = new TaxonomyRequest();
            tr.PageSize = 0;
            tr.TaxonomyId = id;
            tr.TaxonomyLanguage = Convert.ToInt32(Request.QueryString["languageId"]);
            tr.TaxonomyType = EkEnumeration.TaxonomyType.Content;
            TaxonomyData td = m_refContentApi.ReadTaxonomy(ref tr);
            if (td != null)
            {
                return "{\"Id\":\"" + id.ToString()
                    + "\",\"Name\":\"" + td.Name  + "\""
                    + ",\"Path\":\"" + td.Path.Replace("\\", "\\\\") + "\""
                    + ",\"Type\":\"Taxonomy\",\"SubType\":\"\",\"TypeCode\":\"2\",\"MarkedForDelete\":\"false\",\"NewlyAdded\":\"" + newlyAddedFlag.ToString().ToLower() + "\"}";
            }
            return String.Empty;
        }

        #endregion
    }
}
