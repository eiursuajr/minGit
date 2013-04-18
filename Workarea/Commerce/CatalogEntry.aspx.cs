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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Workarea;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs;
using Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData;


namespace Ektron.Cms.Commerce.Workarea.CatalogEntry
{


    public partial class CatalogEntry : workareabase
    {


        #region Variables

        private string _ApplicationPath;
        private string _SitePath;
        protected string _ContentEditorId = "";
        protected Ektron.ContentDesignerWithValidator cdEditor;
        protected string m_sEditAction = "";
        protected string editorPackage = "";
        protected ProductType m_refProductType = null;
        protected ProductTypeData prod_type_data = null;
        protected long xid = 0;
        protected bool bSuppressTemplate = false;
        protected FolderData catalog_data = new FolderData();
        protected int lValidCounter = 0;
        protected List<ContentMetaData> meta_data = new List<ContentMetaData>();
        protected EntryData entry_edit_data = null;
        protected Ektron.Cms.Site.EkSite m_refSite = null;
        protected long m_iFolder = 0;
        protected MeasurementData m_mMeasures = null;
        protected Ektron.Cms.Commerce.CatalogEntry m_refCatalog = null;
        protected TaxClass m_refTaxClass = null;
        protected Currency m_refCurrency = null;
        protected string TaxonomyTreeIdList = "";
        protected string TaxonomyTreeParentIdList = "";
        protected bool TaxonomyRoleExists = false;
        protected long m_intTaxFolderId = 0;
        protected long TaxonomyOverrideId = 0;
        protected long TaxonomySelectId = 0;
        //Varibles used for Aliasing
        private Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _urlAliasSettingApi = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        private string m_strManualAlias = string.Empty;
        private long m_manualAliasId = 0;
        private string m_strManualAliasExt = string.Empty;
        private string m_prevManualAliasName = string.Empty;
        private string m_currManualAliasName = string.Empty;
        private string m_prevManualAliasExt = string.Empty;
        private string m_currManualAliasExt = string.Empty;
        private PermissionData m_cPerms = null;
        private PermissionData UserRights;
        private bool ShowTaxonomyTab = true;
        private EkContent m_refContent;
        private SiteAPI m_refSiteApi;
        private LanguageData language_data;

        //js member vars
        //js: page function vars
        private string _JsPageFunctions_ContentEditorId = "default";
        //js: taxonomy function vars
        private string _JSTaxonomyFunctions_FolderId = "default";
        private string _JSTaxonomyFunctions_TaxonomyOverrideId = "default";
        private string _JSTaxonomyFunctions_TaxonomyTreeIdList = "default";
        private string _JSTaxonomyFunctions_TaxonomyTreeParentIdList = "default";
        private string _JSTaxonomyFunctions_ShowTaxonomy = "default";
        private string _JSTaxonomyFunctions_TaxonomyFolderId = "default";
        private bool _inContextEditing = false;
        private string _stylesheet = "";
        private string _stylesheetPath = "";
        private long backLangType = 1033;
        private long otherLangId = 0;
        private bool metadataRequired = false;

        #endregion

        #region Properties

        private string ApplicationPath
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

        private string SitePath
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

        private bool PullApproval
        {
            get
            {
                return (Request.QueryString["pullapproval"] != null &&
                    EkFunctions.GetBoolFromYesNo(Request.QueryString["pullapproval"]));
            }
        }

        #endregion

        #region Page Functions

        protected CatalogEntry()
        {

            char[] slash = new char[] { '/' };
            this.SitePath = m_refContentApi.SitePath.TrimEnd(slash.ToString().ToCharArray());
            this.ApplicationPath = m_refContentApi.ApplicationPath.TrimEnd(slash.ToString().ToCharArray());
            this.m_refSiteApi = new SiteAPI();

        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            if (Request.Browser.Type == "IE6")
            {
                contentEditor.Width = new Unit(1200, UnitType.Pixel);
                contentEditor.Height = new Unit(400, UnitType.Pixel);
            }
            else if (Request.Browser.Type.IndexOf("Firefox") != -1)
            {
                contentEditor.Width = new Unit(100, UnitType.Percentage);
                contentEditor.Height = new Unit(800, UnitType.Pixel);
            }
            else
            {
                contentEditor.Width = new Unit(100, UnitType.Percentage);
                contentEditor.Height = new Unit(635, UnitType.Pixel);
            }

            if (Request.Browser.Type == "IE6")
            {
                summaryEditor.Width = new Unit(1200, UnitType.Pixel);
                summaryEditor.Height = new Unit(400, UnitType.Pixel);
            }
            else if (Request.Browser.Type.IndexOf("Firefox") != -1)
            {
                summaryEditor.Width = new Unit(100, UnitType.Percentage);
                summaryEditor.Height = new Unit(800, UnitType.Pixel);
            }
            else
            {
                summaryEditor.Width = new Unit(100, UnitType.Percentage);
                summaryEditor.Height = new Unit(635, UnitType.Pixel);
            }            
        }

        protected override void Page_Load(object sender, System.EventArgs e)
        {

            try
            {
                base.Page_Load(sender, e);
                if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
                {
                    throw (new Exception(GetMessage("feature locked error")));
                }
                Util_ObtainValues();
                Util_CheckAccess();
                m_refCatalog = new Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef);
                m_refCurrency = new Currency(m_refContentApi.RequestInformationRef);
                m_refContent = m_refContentApi.EkContentRef;
                hdn_defaultCurrency.Value = m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId.ToString();

                switch (this.m_sEditAction)
                {
                    case "add":
                    case "addlang":
                        m_iFolder = this.m_iID;
                        if (!Page.IsPostBack)
                        {
                            UserRights = m_refContentApi.LoadPermissions(m_iFolder, "folder", ContentAPI.PermissionResultType.Folder);
                            ContentMetaData[] defaultMeta;
                            Util_CheckFolderType();
                            if (m_sEditAction == "addlang")
                            {
                                entry_edit_data = m_refCatalog.GetItem(otherLangId, backLangType);
                                if (entry_edit_data.ProductType.Id > 0)
                                {
                                    m_refProductType = new ProductType(m_refContentApi.RequestInformationRef);
                                    prod_type_data = m_refProductType.GetItem(entry_edit_data.ProductType.Id, true);
                                    editorPackage = prod_type_data.PackageXslt;
                                    xid = prod_type_data.Id;
                                    Util_SetXmlId(xid);
                                    hdn_entrytype.Value = entry_edit_data.EntryType.ToString();
                                }
                            }
                            if (entry_edit_data == null)
                            {
                                Util_GetEntryType();
                            }
                            defaultMeta = m_refContentApi.GetMetaDataTypes("id");
                            if ((defaultMeta != null) && defaultMeta.Length > 0)
                            {
                                meta_data.AddRange(defaultMeta);
                            }
                            Display_ContentTab();
                            Display_SummaryTab();
                            Display_EntryTab();
                            Display_PricingTab();
                            Display_MediaTab();
                            Display_ItemTab();
                            Display_MetadataTab();
                            Display_ScheduleTab();
                            Display_TaxonomyTab();
                            Display_CommentTab();
                            Display_TemplateTab();
                            if ((_urlAliasSettingApi.IsManualAliasEnabled || _urlAliasSettingApi.IsAutoAliasEnabled) && m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
                            {
                                Display_AliasTab();
                            }
                            Util_SetLabels();
                        }
                        else
                        {
                            Process_Add();
                        }
                        break;
                    case "update":
                        if (!Page.IsPostBack)
                        {
                            UserRights = m_refContentApi.LoadPermissions(m_iID, "content", ContentAPI.PermissionResultType.Content);
                            if (PullApproval)
                                this.m_refContent.TakeOwnership(m_iID);
                            entry_edit_data = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, true);
                            if (entry_edit_data.ProductType.Id > 0)
                            {
                                m_refProductType = new ProductType(m_refContentApi.RequestInformationRef);
                                prod_type_data = m_refProductType.GetItem(entry_edit_data.ProductType.Id, true);
                                editorPackage = prod_type_data.PackageXslt;
                                xid = prod_type_data.Id;
                                Util_SetXmlId(xid);
                                hdn_entrytype.Value = entry_edit_data.EntryType.ToString();
                            }
                            meta_data = entry_edit_data.Metadata;
                            m_iFolder = entry_edit_data.FolderId;
                            Util_CheckFolderType();
                            Display_ContentTab();
                            Display_SummaryTab();
                            Display_EntryTab();
                            Display_PricingTab();
                            Display_MediaTab();
                            Display_ItemTab();
                            Display_MetadataTab();
                            Display_ScheduleTab();
                            Display_TaxonomyTab();
                            Display_CommentTab();
                            Display_TemplateTab();
                            if ((_urlAliasSettingApi.IsManualAliasEnabled || _urlAliasSettingApi.IsAutoAliasEnabled) && m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
                            {
                                Display_AliasTab();
                            }
                            Util_SetLabels();
                        }
                        else
                        {
                            Process_Edit();
                        }
                        break;
                }
                Util_SetJS();

                if (prod_type_data != null)
                {
                    hdn_productType.Value = prod_type_data.EntryClass.ToString();
                }

                this.RegisterJs();
                this.RegisterCss();
                if (catalog_data != null)
                {
                    chk_searchable.Checked = catalog_data.IscontentSearchable;
                }

                //-------------------DisplayTabs Based on selected options from Folder properties----------------------------------
                if (((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && catalog_data.DisplaySettings != 0)
                {
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
                    { divSummary.Visible = true; }
                    else
                    {
                        divSummary.Visible = false;
                        liSummary.Visible = false;
                    }
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
                    { divMetadata.Visible = true; }
                    else
                    {
                        if (!metadataRequired)
                        {
                            divMetadata.Visible = false;
                            liMetadata.Visible = false;
                        }
                    }
                    if ((_urlAliasSettingApi.IsManualAliasEnabled || _urlAliasSettingApi.IsAutoAliasEnabled) && m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
                    {

                        if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
                        { divAlias.Visible = true; }
                        else
                        {
                            if (!catalog_data.AliasRequired)
                            {
                                divAlias.Visible = false;
                                liAlias.Visible = false;
                            }
                        }
                    }
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
                    { divSchedule.Visible = true; }
                    else
                    {
                        divSchedule.Visible = false;
                        liSchedule.Visible = false;
                    }
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
                    { divComment.Visible = true; }
                    else
                    {
                        divComment.Visible = false;                       
                    }
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
                    { divTemplates.Visible = true; }
                    else
                    {
                        divTemplates.Visible = false;
                    }
                    if ((catalog_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
                    { divCategories.Visible = true; }
                    else
                    {
                        if (!catalog_data.IsCategoryRequired)
                        {
                            divCategories.Visible = false;
                            liCategory.Visible = false;
                        }
                    }
                }

                //-------------------DisplayTabs Based on selected options from Folder properties End------------------------------
            }
            catch (Exception ex)
            {

                Utilities.ShowError(ex.Message);

            }

        }

        #endregion

        #region Process


        #region Add

        public void Process_Add()
        {
            if (!string.IsNullOrEmpty(Request.Form["hdn_xmlid"]) && Convert.ToInt64(Request.Form["hdn_xmlid"]) > 0)
            {
                m_refProductType = new ProductType(m_refContentApi.RequestInformationRef);
                prod_type_data = m_refProductType.GetItem(Convert.ToInt64(Request.Form["hdn_xmlid"]), true);
                xid = prod_type_data.Id;
                Util_SetXmlId(xid);
            }
            if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct.ToString())
            {
                Process_AddSubscription();
            }
            else if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle.ToString())
            {
                Process_AddBundle();
            }
            else if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit.ToString())
            {
                Process_AddKit();
            }
            else if ((Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product.ToString()) || (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct.ToString()))
            {
                Process_AddProduct();
            }
        }

        public void Process_AddSubscription()
        {

            SubscriptionProductData entry = new SubscriptionProductData();
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();

            entry = (SubscriptionProductData)Process_GetEntryAddValues(entry);

            entry.SubscriptionInfo = Process_GetSubscriptionInfo(entry);

            urlAliasInfo = Process_Alias();

            
            try
            {
                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.AddAndSave(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.AddAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.AddAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iFolder.ToString())); // goes to folder
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_AddBundle()
        {
            BundleData entry = new BundleData();
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();

            entry = (BundleData)Process_GetEntryAddValues(entry);

            entry.BundledItems = Process_GetBundledItems();

            urlAliasInfo = Process_Alias();

            try
            {
                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.AddAndSave(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.AddAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.AddAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iFolder.ToString())); // goes to folder
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_AddKit()
        {
            KitData entry = new KitData();
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();

            entry = (KitData)Process_GetEntryAddValues(entry);

            entry.OptionGroups = Process_GetKitGroups();

            urlAliasInfo = Process_Alias();

            try
            {
                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.AddAndSave(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.AddAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.AddAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iFolder.ToString())); // goes to folder
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_AddProduct()
        {

            ProductData entry = new ProductData();
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();

            entry = (ProductData)Process_GetEntryAddValues(entry);

            this.ucItem.EntryEditData = entry;
            this.ucItem.ItemsFolderId = m_iFolder;
            entry.Variants = Process_GetVariants();

            if (entry.Variants.Count > 0)
            {
                entry.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct;
            }

            urlAliasInfo = Process_Alias();

            try
            {
                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.AddAndSave(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.AddAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.AddAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id, entry.FolderId);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iFolder.ToString())); // goes to folder
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public EntryData Process_GetEntryAddValues(EntryData entry)
        {

            if (Request.QueryString["content_id"] != "")
            {
                entry.Id = Convert.ToInt64(Request.QueryString["content_id"]);
            }

            entry.Title = Request.Form["content_title"];
            entry.IsSearchable = chk_searchable.Checked;
            entry.LanguageId = ContentLanguage;
            entry.Html = (string)contentEditor.Content;
            entry.Summary = (string)summaryEditor.Content;
            entry.Image = Process_GetDefaultImage(); // Request.Form("entry_image")
            entry.Comment = (string)hdnComment.Value;
            entry.FolderId = m_iFolder;
            entry.ProductType.Id = Convert.ToInt64(hdn_xmlid.Value);
            entry.TemplateId = Convert.ToInt64(drp_tempsel.SelectedValue);

            entry.Sku = (string)txt_sku.Text;
            entry.QuantityMultiple = EkFunctions.ReadIntegerValue(txt_quantity.Text, 1);
            entry.EntryType = (EkEnumeration.CatalogEntryType)Enum.Parse(typeof(EkEnumeration.CatalogEntryType), Request.Form["hdn_entrytype"]);
            entry.TaxClassId = Convert.ToInt64(drp_taxclass.SelectedValue);
            entry.IsArchived = System.Convert.ToBoolean(chk_avail.Checked);
            // entry.IsMarkedForDeletion = chk_markdel.Checked
            entry.IsBuyable = System.Convert.ToBoolean(chk_buyable.Checked);

            if (!chk_tangible.Checked)
            {
                entry.Dimensions.Height = 0;
                entry.Dimensions.Length = 0;
                entry.Dimensions.Width = 0;
                entry.Weight.Amount = 0;
            }
            else
            {
                if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English)
                {
                    entry.Dimensions.Units = LinearUnit.Inches;
                }
                else
                {
                    entry.Dimensions.Units = LinearUnit.Centimeters;
                }
                entry.Dimensions.Height = (float)(EkFunctions.ReadDecimalValue(txt_height.Text, 0));
                entry.Dimensions.Length = (float)(EkFunctions.ReadDecimalValue(txt_length.Text, 0));
                entry.Dimensions.Width = (float)(EkFunctions.ReadDecimalValue(txt_width.Text, 0));
                if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English)
                {
                    entry.Weight.Units = WeightUnit.Pounds;
                }
                else
                {
                    entry.Weight.Units = WeightUnit.Kilograms;
                }
                entry.Weight.Amount = (float)(EkFunctions.ReadDecimalValue(txt_weight.Text, 0));
            }

            entry.Pricing = Process_GetPricing(null);

            entry.Media = Process_GetMedia(null);

            entry.Metadata = Process_GetMetaData();

            entry.Attributes = Process_GetAttributes();

            if (Request.Form["end_date"] != null && Request.Form["end_date"] != "")
            {
                entry.EndDate = DateTime.Parse(Strings.Trim(Request.Form["end_date"]));
                entry.EndDateAction = System.Convert.ToInt32(rblaction.SelectedValue);
                if (entry.EndDateAction == 2)
                {
                    entry.IsBuyable = false;
                    chk_buyable.Checked = false;
                }
            }
            else
            {
                entry.EndDate = DateTime.MinValue;
                entry.EndDateAction = 0;
            }

            if (Request.Form["go_live"] != null && Request.Form["go_live"] != "")
            {
                entry.GoLive = Convert.ToDateTime(Request.Form["go_live"]);
            }
            else
            {
                entry.GoLive = DateTime.MinValue;
            }

            entry.DisableInventoryManagement = System.Convert.ToBoolean(chk_disableInv.Checked);

            return entry;

        }

        #endregion


        #region Edit

        public void Process_Edit()
        {
            if (!string.IsNullOrEmpty(Request.Form["hdn_xmlid"]) && Convert.ToInt64(Request.Form["hdn_xmlid"]) > 0)
            {
                m_refProductType = new ProductType(m_refContentApi.RequestInformationRef);
                prod_type_data = m_refProductType.GetItem(Convert.ToInt64(Request.Form["hdn_xmlid"]), true);
                xid = prod_type_data.Id;
                Util_SetXmlId(xid);
            }
            if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct.ToString())
            {
                Process_EditSubscription();
            }
            else if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle.ToString())
            {
                Process_EditBundle();
            }
            else if (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit.ToString())
            {
                Process_EditKit();
            }
            else if ((Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product.ToString()) || (Request.Form["hdn_entrytype"] == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct.ToString()))
            {
                Process_EditProduct();
            }
        }

        public EntryData Process_GetEntryValues(EntryData entry)
        {

            entry.Title = Request.Form["content_title"];
            entry.Html = (string)contentEditor.Content;
            entry.Summary = (string)summaryEditor.Content;
            entry.Image = Process_GetDefaultImage(); // Request.Form("entry_image")
            entry.Comment = (string)hdnComment.Value;
            //entry.FolderId = m_iFolder
            entry.ProductType.Id = Convert.ToInt64(hdn_xmlid.Value);
            entry.TemplateId = Convert.ToInt64(drp_tempsel.SelectedValue);

            entry.Sku = (string)txt_sku.Text;
            entry.QuantityMultiple = EkFunctions.ReadIntegerValue(txt_quantity.Text, 1);
            // entry.EntryType = Request.Form("hdn_entrytype")
            entry.TaxClassId = Convert.ToInt64(drp_taxclass.SelectedValue);
            entry.IsArchived = System.Convert.ToBoolean(chk_avail.Checked);
            // entry.IsMarkedForDeletion = chk_markdel.Checked
            entry.IsBuyable = System.Convert.ToBoolean(chk_buyable.Checked);

            if (!chk_tangible.Checked)
            {
                entry.Dimensions.Height = 0;
                entry.Dimensions.Length = 0;
                entry.Dimensions.Width = 0;
                entry.Weight.Amount = 0;
            }
            else
            {
                if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English)
                {
                    entry.Dimensions.Units = LinearUnit.Inches;
                }
                else
                {
                    entry.Dimensions.Units = LinearUnit.Centimeters;
                }
                entry.Dimensions.Height = (float)(EkFunctions.ReadDecimalValue(txt_height.Text, 0));
                entry.Dimensions.Length = (float)(EkFunctions.ReadDecimalValue(txt_length.Text, 0));
                entry.Dimensions.Width = (float)(EkFunctions.ReadDecimalValue(txt_width.Text, 0));
                if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English)
                {
                    entry.Weight.Units = WeightUnit.Pounds;
                }
                else
                {
                    entry.Weight.Units = WeightUnit.Kilograms;
                }
                entry.Weight.Amount = (float)(EkFunctions.ReadDecimalValue(txt_weight.Text, 0));
            }

            entry.Pricing = Process_GetPricing(entry.Pricing);

            entry.Media = Process_GetMedia(null);

            entry.Metadata = Process_GetMetaData();

            entry.Attributes = Process_GetAttributes();

            if (Request.Form["end_date"] != null && Request.Form["end_date"] != "")
            {
                entry.EndDate = DateTime.Parse(Strings.Trim(Request.Form["end_date"]));
                entry.EndDateAction = System.Convert.ToInt32(rblaction.SelectedValue);
                if (entry.EndDateAction == 2)
                {
                    entry.IsBuyable = false;
                    chk_buyable.Checked = false;
                }
            }
            else
            {
                entry.EndDate = DateTime.MinValue;
                entry.EndDateAction = 0;
            }

            if (Request.Form["go_live"] != null && Request.Form["go_live"] != "")
            {
                entry.GoLive = Convert.ToDateTime(Request.Form["go_live"]);
            }
            else
            {
                entry.GoLive = DateTime.MinValue;
            }
            entry.DisableInventoryManagement = System.Convert.ToBoolean(chk_disableInv.Checked);

            return entry;

        }

        public void Process_EditSubscription()
        {
            SubscriptionProductData entry = null;
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();
            entry = (SubscriptionProductData)m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, false);
            if (hdn_publishaction.Value != Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
            {

                entry = (SubscriptionProductData)Process_GetEntryValues(entry);

                entry.SubscriptionInfo = Process_GetSubscriptionInfo(entry);

                urlAliasInfo = Process_Alias();

            }
            try
            {


                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.Save(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.SaveAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
                {
                    m_refCatalog.UndoCheckOut(entry.Id);
                    Util_UndoCheckoutResponseHandler(entry.Id, entry.FolderId, entry.LanguageId, ContentLanguage);
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_EditBundle()
        {
            BundleData entry = null;
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();
            entry = (BundleData)m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, false);
            if (hdn_publishaction.Value != Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
            {

                entry = (BundleData)Process_GetEntryValues(entry);

                entry.BundledItems = Process_GetBundledItems();

                urlAliasInfo = Process_Alias();

            }
            try
            {


                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.Save(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.SaveAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
                {
                    m_refCatalog.UndoCheckOut(entry.Id);
                    Util_UndoCheckoutResponseHandler(entry.Id, entry.FolderId, entry.LanguageId, ContentLanguage);
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_EditKit()
        {
            KitData entry = null;
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();
            entry = (KitData)m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, false);
            if (hdn_publishaction.Value != Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
            {

                entry = (KitData)Process_GetEntryValues(entry);

                entry.OptionGroups = Process_GetKitGroups();

                urlAliasInfo = Process_Alias();

            }
            try
            {


                if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
                {
                    m_refCatalog.Save(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
                {
                    m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
                {
                    m_refCatalog.SaveAndPublish(entry, urlAliasInfo);
                    Process_Taxonomy(entry.Id);
                    Process_Inventory(entry.Id);
                    Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
                }
                else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
                {
                    m_refCatalog.UndoCheckOut(entry.Id);
                    Util_UndoCheckoutResponseHandler(entry.Id, entry.FolderId, entry.LanguageId, ContentLanguage);
                }
            }
            catch (Exception ex)
            {
                Util_ResponseHandler((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + entry.LanguageId));
            }
        }

        public void Process_EditProduct()
        {
            ProductData entry = null;
            UrlAliasInfo urlAliasInfo = new UrlAliasInfo();
            entry = (ProductData)m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, false);
            if (hdn_publishaction.Value != EkEnumeration.AssetActionType.UndoCheckout.ToString())
            {

                entry = (ProductData)Process_GetEntryValues(entry);

                this.ucItem.EntryEditData = entry;
                this.ucItem.ItemsFolderId = m_iFolder;
                entry.Variants = Process_GetVariants();

                urlAliasInfo = Process_Alias();

                if (entry.Variants.Count > 0)
                {
                    entry.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct;
                }
                if (entry.Variants.Count == 0)
                {
                    entry.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product;
                }

            }
            if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Save).ToString())
            {
                m_refCatalog.Save(entry, urlAliasInfo);
                Process_Taxonomy(entry.Id, entry.FolderId);
                Process_Inventory(entry.Id);
                Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" + entry.LanguageId + "&id=" + entry.Id + "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" + entry.FolderId + "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "&back_LangType=" + entry.LanguageId + "&rnd=6"); // goes to edit screen.
            }
            else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Checkin).ToString())
            {
                m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo);
                Process_Taxonomy(entry.Id, entry.FolderId);
                Process_Inventory(entry.Id);
                Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
            }
            else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.Submit).ToString())
            {
                m_refCatalog.SaveAndPublish(entry, urlAliasInfo);
                Process_Taxonomy(entry.Id, entry.FolderId);
                Process_Inventory(entry.Id);
                Util_ResponseHandler((string)("../content.aspx?action=View&folder_id=" + entry.FolderId + "&id=" + entry.Id + "&LangType=" + entry.LanguageId + "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" + entry.FolderId + "%26contentid%3d0%26form_id%3d0%26LangType%3d" + entry.LanguageId)); // goes to content view screen
            }
            else if (hdn_publishaction.Value == Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout).ToString())
            {
                m_refCatalog.UndoCheckOut(entry.Id);
                Util_UndoCheckoutResponseHandler(entry.Id, entry.FolderId, entry.LanguageId, ContentLanguage);
            }
        }

        #endregion


        #region Other

        public List<EntryAttributeData> Process_GetAttributes()
        {
            List<EntryAttributeData> attributeList = new List<EntryAttributeData>();

            int iValidCounter = 101;
            while (!string.IsNullOrEmpty(Request.Form["frm_meta_type_id_" + iValidCounter]))
            {
                int attributeIndex = -1;
                EntryAttributeData attribute = new EntryAttributeData();
                string MetaSelect;
                string separater = "";
                attribute.AttributeTypeId = Convert.ToInt64(Request.Form["frm_meta_type_id_" + iValidCounter]);
                if (prod_type_data != null)
                {
                    for (int j = 0; j <= (prod_type_data.Attributes.Count - 1); j++)
                    {
                        if (prod_type_data.Attributes[j].Id == attribute.AttributeTypeId)
                        {
                            attribute.ActiveStatus = prod_type_data.Attributes[j].ActiveStatus;
                            attribute.DataType = prod_type_data.Attributes[j].DataType;
                            attribute.DefaultValue = prod_type_data.Attributes[j].DefaultValue;
                            attribute.DisplayOrder = prod_type_data.Attributes[j].DisplayOrder;
                            attribute.Name = prod_type_data.Attributes[j].Name;
                            attributeIndex = j;

                            separater = Request.Form["MetaSeparator_" + iValidCounter];
                            MetaSelect = Request.Form["MetaSelect_" + iValidCounter];
                            if (!string.IsNullOrEmpty(MetaSelect))
                            {
                                if (attribute.DataType == Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Boolean)
                                {
                                    if (Strings.Replace(Request.Form["frm_text_" + iValidCounter], ", ", separater, 1, -1, 0) != "")
                                    {
                                        if (Request.Form["frm_text_" + iValidCounter] == "Yes")
                                        {
                                            attribute.CurrentValue = true;
                                        }
                                        else if (Request.Form["frm_text_" + iValidCounter] == "No")
                                        {
                                            attribute.CurrentValue = false;
                                        }
                                    }
                                    else
                                    {
                                        attribute.CurrentValue = false;
                                    }
                                }
                                else
                                {
                                    if (Request.Form["frm_text_" + iValidCounter] != null)
                                    {
                                        attribute.CurrentValue = Request.Form["frm_text_" + iValidCounter];
                                    }
                                    else
                                    {
                                        attribute.CurrentValue = attribute.DefaultValue;
                                    }
                                }
                            }
                            else
                            {
                                string myMeta = "";
                                myMeta = Request.Form["frm_text_" + iValidCounter];
                                myMeta = Server.HtmlDecode(myMeta);
                                attribute.CurrentValue = myMeta.Replace(";", separater);
                            }
                            attributeList.Add(attribute);

                            break;
                        }
                    }
                }
                iValidCounter++;
            }
            return attributeList;
        }

        public void Process_Taxonomy(long entryId)
        {
            Process_Taxonomy(entryId, -1);
        }
        public void Process_Taxonomy(long entryId, long folderID)
        {
            if (TaxonomyOverrideId > 0)
            {
                TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
            }
            if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
            {
                TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                if (TaxonomyTreeIdList.Trim().EndsWith(","))
                {
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                }
            }
            TaxonomyContentRequest entry_request = new TaxonomyContentRequest();
            if (folderID != -1)
            {
                entry_request.FolderID = folderID;
            }
            entry_request.ContentId = entryId;
            entry_request.TaxonomyList = TaxonomyTreeIdList;
            m_refContentApi.AddTaxonomyItem(entry_request);
        }

        public void Process_Inventory(long entryId)
        {

            if (!chk_disableInv.Checked)
            {

                InventoryApi inventoryApi = new InventoryApi();
                InventoryData inventoryData = new InventoryData();

                inventoryData.EntryId = entryId;
                inventoryData.UnitsInStock = EkFunctions.ReadIntegerValue(txt_instock.Text, 0);
                inventoryData.UnitsOnOrder = EkFunctions.ReadIntegerValue(txt_onorder.Text, 0);
                inventoryData.ReorderLevel = EkFunctions.ReadIntegerValue(txt_reorder.Text, 0);

                inventoryApi.SaveInventory(inventoryData);

            }

        }

        public List<ContentMetaData> Process_GetMetaData()
        {
            System.Collections.Generic.List<ContentMetaData> lMeta = new System.Collections.Generic.List<ContentMetaData>();
            int iValidCounter = 0;

            if (Request.Form["frm_validcounter"] != "")
            {
                iValidCounter = System.Convert.ToInt32(Request.Form["frm_validcounter"]);
            }
            else
            {
                iValidCounter = 0;
            }
            for (int i = 1; i <= iValidCounter; i++)
            {
                ContentMetaData eMeta = new ContentMetaData();
                string MetaSelect;
                eMeta.TypeId = Convert.ToInt64(Request.Form["frm_meta_type_id_" + i]);
                eMeta.Separator = Request.Form["MetaSeparator_" + i];
                MetaSelect = Request.Form["MetaSelect_" + i];
                if (!string.IsNullOrEmpty(MetaSelect))
                {
                    eMeta.Text = Strings.Replace(Request.Form["frm_text_" + i], ", ", eMeta.Separator, 1, -1, 0);
                    if (eMeta.Text != null && eMeta.Text.Substring(0, 1) == eMeta.Separator)
                    {
                        eMeta.Text = eMeta.Text.Substring(eMeta.Text.Length - (eMeta.Text.Length - 1), (eMeta.Text.Length - 1));
                    }
                }
                else
                {
                    string myMeta = "";
                    myMeta = Request.Form["frm_text_" + i];
                    myMeta = Server.HtmlDecode(myMeta);
                    eMeta.Text = myMeta.Replace(";", eMeta.Separator);
                }
                lMeta.Add(eMeta);
            }
            return lMeta;
        }

        public List<EntryData> Process_GetBundledItems()
        {

            List<EntryData> aProducts = new List<EntryData>();
            if (this.ucItem.ItemData != null)
            {
                List<Object> newItems = new List<Object>();
                newItems = (List<Object>)this.ucItem.ItemData;

                for (int i = 0; i <= (newItems.Count - 1); i++)
                {
                    Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData newProduct = (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData)newItems[i];
                    if (newProduct != null && newProduct.MarkedForDelete == false)
                    {
                        ProductData BundleProduct = new ProductData();
                        BundleProduct.Id = newProduct.Id;
                        BundleProduct.LanguageId = this.ContentLanguage;
                        aProducts.Add(BundleProduct);
                    }
                }
            }

            return aProducts;

        }

        public List<ProductVariantData> Process_GetVariants()
        {

            List<ProductVariantData> aVariants = new List<ProductVariantData>();

            if (this.ucItem.ItemData != null)
            {
                List<Object> newItems = new List<Object>();
                newItems = (List<Object>)this.ucItem.ItemData;

                for (int i = 0; i <= (newItems.Count - 1); i++)
                {
                    Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData newProduct = (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData)newItems[i];
                    if (newProduct != null && newProduct.MarkedForDelete == false)
                    {
                        ProductVariantData ProductVariant = new ProductVariantData();
                        ProductVariant.Id = newProduct.Id;
                        ProductVariant.LanguageId = this.ContentLanguage;
                        aVariants.Add(ProductVariant);
                    }
                }
            }

            return aVariants;

        }

        public List<OptionGroupData> Process_GetKitGroups()
        {
            List<OptionGroupData> aGroups = new List<OptionGroupData>();

            if (this.ucItem.ItemData != null)
            {
                List<Object> newItems = new List<Object>();
                newItems = (List<Object>)this.ucItem.ItemData;
                for (int i = 0; i <= (newItems.Count - 1); i++)
                {
                    Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData newProduct = (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.ItemData)newItems[i];
                    if (newProduct != null && newProduct.MarkedForDelete == false)
                    {

                        OptionGroupData OptionGroup = new OptionGroupData();
                        Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.KitData kitGroup = (Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.KitData)newProduct;

                        OptionGroup.Id = kitGroup.Id;
                        OptionGroup.Name = (string)kitGroup.Title;
                        OptionGroup.Image = "";
                        OptionGroup.Description = (string)kitGroup.Description;
                        OptionGroup.DisplayOrder = System.Convert.ToInt32(kitGroup.Order);

                        OptionGroupItemCollection aOptions = new OptionGroupItemCollection();

                        for (int j = 0; j <= (kitGroup.Items.Count - 1); j++)
                        {

                            if (!kitGroup.Items[j].MarkedForDelete)
                            {

                                OptionGroupItemData OptionItem = new OptionGroupItemData();

                                OptionItem.DisplayOrder = System.Convert.ToInt32(kitGroup.Items[j].Order);
                                OptionItem.Name = (string)(kitGroup.Items[j].Title);
                                OptionItem.GroupId = OptionGroup.Id;
                                OptionItem.Id = kitGroup.Items[j].Id;
                                OptionItem.ExtraText = (string)(kitGroup.Items[j].ExtraText);
                                OptionItem.PriceModification = Convert.ToDecimal(kitGroup.Items[j].PriceModifierDollars + "." + kitGroup.Items[j].PriceModifierCents);

                                if (kitGroup.Items[j].PriceModifierPlusMinus == "-")
                                {
                                    OptionItem.PriceModification = OptionItem.PriceModification * -1;
                                }

                                aOptions.Add(OptionItem);

                            }

                        }

                        OptionGroup.Options = aOptions;
                        aGroups.Add(OptionGroup);

                    }

                }

            }

            return aGroups;

        }

        private UrlAliasInfo Process_Alias()
        {

            //Aliasing logic for 7.6 starts here
            UrlAliasInfo _manualAliasInfo = new UrlAliasInfo();
            if (!string.IsNullOrEmpty(Request.Form["frm_manalias"]))
            {
                m_strManualAlias = Request.Form["frm_manalias"].ToString().Trim();
            }
            if (!string.IsNullOrEmpty(Request.Form["frm_manaliasExt"]))
            {
                m_strManualAliasExt = Request.Form["frm_manaliasExt"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Form["prev_frm_manalias_name"]))
            {
                m_prevManualAliasName = Request.Form["prev_frm_manalias_name"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Form["prev_frm_manalias_ext"]))
            {
                m_prevManualAliasExt = Request.Form["prev_frm_manalias_ext"].ToString();
            }
            m_currManualAliasName = m_strManualAlias;
            m_currManualAliasExt = m_strManualAliasExt;

            if (!string.IsNullOrEmpty(Request.Form["frm_manalias_id"]))
            {
                m_manualAliasId = System.Convert.ToInt32(Request.Form["frm_manalias_id"]);
            }
            _manualAliasInfo.AliasId = m_manualAliasId;
            _manualAliasInfo.CurrentAliasName = m_currManualAliasName;
            _manualAliasInfo.CurrentAliasExtension = m_currManualAliasExt;
            _manualAliasInfo.PreviousAliasName = m_prevManualAliasName;
            _manualAliasInfo.PreviousAliasExtension = m_prevManualAliasExt;
            return _manualAliasInfo;
        }

        protected object Process_GetSubscriptionInfo(EntryData entry)
        {

            Ektron.Cms.Commerce.Subscriptions.MembershipSubscriptionInfo subscriptionInfo = new Ektron.Cms.Commerce.Subscriptions.MembershipSubscriptionInfo();
            long authorGroupId = 0;
            long memberGroupId = 0;

            try
            {

                if (Request.Form["EktronSusbscriptionCmsGroupMarkedForDelete"] == "false")
                {
                    authorGroupId = Convert.ToInt64(Request.Form["EktronSusbscriptionCmsGroupId"]);
                }
                else
                {
                    authorGroupId = 0;
                }
                memberGroupId = Convert.ToInt64(Request.Form["EktronSusbscriptionMembershipGroupId"]);

                subscriptionInfo.EntryId = entry.Id;
                subscriptionInfo.AuthorGroupId = authorGroupId;
                subscriptionInfo.MemberGroupId = memberGroupId;

            }
            catch (Exception)
            {

            }


            return subscriptionInfo;

        }

        protected string Process_GetDefaultImage()
        {

            string defaultImage = "";

            if (this.m_sEditAction == "addlang")
            {

                defaultImage = Request.Form["entry_image"];

            }
            else
            {

                if (ucMedia.ImageData != null)
                {

                    for (int i = 0; i <= (ucMedia.ImageData.Count - 1); i++)
                    {

                        if (ucMedia.ImageData[i].MarkedForDelete == false && ucMedia.ImageData[i].Default)
                        {
                            defaultImage = (string)(ucMedia.ImageData[i].Path);
                        }

                    }

                }

            }

            return defaultImage;

        }

        protected MediaGalleryData Process_GetMedia(MediaGalleryData media)
        {

            List<ImageMediaData> ImageList = new List<ImageMediaData>();
            if (media == null)
            {
                media = new MediaGalleryData();
            }

            if (ucMedia.ImageData != null)
            {

                for (int i = 0; i <= (ucMedia.ImageData.Count - 1); i++)
                {

                    ImageMediaData image = new ImageMediaData();

                    if (ucMedia.ImageData[i].MarkedForDelete == false && ucMedia.ImageData[i].Id > 0)
                    {
                        image.Id = ucMedia.ImageData[i].Id;
                        image.FileName = (string)(ucMedia.ImageData[i].Title);
                        image.FilePath = (string)(ucMedia.ImageData[i].Path);
                        image.Alt = (string)(ucMedia.ImageData[i].AltText);
                        image.Height = EkFunctions.ReadIntegerValue(ucMedia.ImageData[i].Height);
                        image.Width = EkFunctions.ReadIntegerValue(ucMedia.ImageData[i].Width);
                        image.IncludedInGallery = System.Convert.ToBoolean(ucMedia.ImageData[i].Gallery);
                        if (prod_type_data.DefaultThumbnails.Count > 0)
                        {

                            if (ucMedia.ImageData[i].Thumbnails != null)
                            {

                                for (int j = 0; j <= ucMedia.ImageData[i].Thumbnails.Count - 1; j++)
                                {

                                    image.Thumbnails.Add(new ThumbnailData());

                                    if (ucMedia.ImageData[i].Thumbnails[j].Path.IndexOf(m_refContentApi.SitePath) > -1 && m_refContentApi.SitePath != "/")
                                    {
                                        image.Thumbnails[j].FilePath = (string)(ucMedia.ImageData[i].Thumbnails[j].Path.Substring(ucMedia.ImageData[i].Thumbnails[j].Path.IndexOf(m_refContentApi.SitePath) + m_refContentApi.SitePath.Length));
                                    }
                                    else if (m_refContentApi.SitePath == "/" && ucMedia.ImageData[i].Thumbnails[j].Path.StartsWith("/"))
                                    {
                                        image.Thumbnails[j].FilePath = Strings.Replace((string)(ucMedia.ImageData[i].Thumbnails[j].Path), "/", "", 1, 1, 0);
                                    }
                                    else
                                    {
                                        image.Thumbnails[j].FilePath = (string)(ucMedia.ImageData[i].Thumbnails[j].Path);
                                    }

                                    image.Thumbnails[j].FilePath = (string)(image.Thumbnails[j].FilePath.Replace("\\", "/").TrimEnd(new char[] { '/' }) + "/" + ucMedia.ImageData[i].Thumbnails[j].ImageName);

                                    if (ucMedia.ImageData[i].Thumbnails[j].Path.LastIndexOf("/") > -1)
                                    {
                                        image.Thumbnails[j].FileName = (string)(ucMedia.ImageData[i].Thumbnails[j].ImageName.Substring(ucMedia.ImageData[i].Thumbnails[j].ImageName.LastIndexOf("/") + 1));
                                    }
                                    else
                                    {
                                        image.Thumbnails[j].FileName = (string)(ucMedia.ImageData[i].Thumbnails[j].ImageName);
                                    }
                                    image.Thumbnails[j].Height = (int)(ucMedia.ImageData[i].Thumbnails[j].Height);
                                    image.Thumbnails[j].Width = (int)(ucMedia.ImageData[i].Thumbnails[j].Width);
                                }
                            }
                        }

                        ImageList.Add(image);
                    }
                }
            }

            media.Images = ImageList;
            return media;
        }

        protected PricingData Process_GetPricing(PricingData currentPricing)
        {
            PricingData updatedPricing = new PricingData();
            // If currentPricing Is Nothing Then currentPricing = New PricingData()
			
            List<CurrencyData> currencyList;
            List<CurrencyPricingData> currencyPriceList = new List<CurrencyPricingData>();

            currencyList = m_refCurrency.GetActiveCurrencyList();

            for (int i = 0; i <= (currencyList.Count - 1); i++)
            {
                if (!(!string.IsNullOrEmpty(Request.Form["ektron_UnitPricing_Float_" + currencyList[i].Id.ToString()])))
                {
                    CurrencyPricingData currencyPrice = new CurrencyPricingData();
                    List<TierPriceData> tierPriceList = new List<TierPriceData>();
                    int tierIndex = 0;
                    TierPriceData defaultTierPrice = new TierPriceData();

                    //currencyPrice.ActualCost = EkFunctions.ReadDecimalValue(Request.Form("ektron_UnitPricing_ActualPrice_" & currencyList(i).Id.ToString()))
                    currencyPrice.AlphaIsoCode = (string)(currencyList[i].AlphaIsoCode);
                    currencyPrice.CurrencyId = currencyList[i].Id;
                    currencyPrice.ListPrice = EkFunctions.ReadDecimalValue(Request.Form["ektron_UnitPricing_ListPrice_" + currencyList[i].Id.ToString()], 0);
                    currencyPrice.PricingType = Ektron.Cms.Common.EkEnumeration.PricingType.Fixed;

                    defaultTierPrice.Quantity = 1;
                    defaultTierPrice.Id = EkFunctions.ReadDbLong(Request.Form["hdn_ektron_UnitPricing_DefaultTier_" + currencyList[i].Id.ToString()]);
                    defaultTierPrice.SalePrice = EkFunctions.ReadDecimalValue(Request.Form["ektron_UnitPricing_SalesPrice_" + currencyList[i].Id.ToString()], 0);
                    currencyPrice.TierPrices.Add(defaultTierPrice);

                    while ((Request.Form["ektron_TierPricing_TierPrice_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()] != null) && Information.IsNumeric(Request.Form["ektron_TierPricing_TierPrice_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()]))
                    {
                        TierPriceData tierPrice = new TierPriceData();
                        if ((Request.Form["ektron_TierPricing_TierQuantity_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()] == "0" || Request.Form["ektron_TierPricing_TierQuantity_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()] == "1") && (tierPrice.Id == 0 || tierPrice.Id == defaultTierPrice.Id))
                        {
                            break;
                        }
                        if (Convert.ToInt32(Request.Form["ektron_TierPricing_TierQuantity_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()]) > 1 && tierPrice.Id == 0)
                        {
                            tierPrice.Id = EkFunctions.ReadDbLong(Request.Form["hdn_ektron_TierPricing_TierId_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()]);
                            tierPrice.Quantity = System.Convert.ToInt32(EkFunctions.ReadDecimalValue(Request.Form["ektron_TierPricing_TierQuantity_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()], 0));
                            tierPrice.SalePrice = EkFunctions.ReadDecimalValue(Request.Form["ektron_TierPricing_TierPrice_" + currencyList[i].Id.ToString() + "_" + tierIndex.ToString()], 0);
                           
						    currencyPrice.TierPrices.Add(tierPrice);
                        }
                        tierIndex++;
                    }
                    currencyPriceList.Add(currencyPrice);
                }

            }

            updatedPricing.CurrencyPricelist = currencyPriceList;

            if (Request.Form["PricingTabRecurringBillingUseRecurrentBilling"] == "true")
            {
                Ektron.Cms.Common.RecurrenceData pricingRecurrance;

                if (Request.Form["PricingTabRecurringBillingBillingCycle"] == "month")
                {
                    pricingRecurrance = Ektron.Cms.Common.RecurrenceData.CreateMonthlyByDayRecurrence(1, Ektron.Cms.Common.RecurrenceDayOfMonth.First, Ektron.Cms.Common.RecurrenceDaysOfWeek.Tuesday);
                }
                else
                {
                    pricingRecurrance = Ektron.Cms.Common.RecurrenceData.CreateYearlyRecurrence(1, 4, 15);
                }

                pricingRecurrance.StartDateUtc = DateTime.Now;
                pricingRecurrance.EndDateUtc = DateTime.Now;
                pricingRecurrance.Intervals = Convert.ToInt32(Request.Form["PricingTabRecurringBillingInterval"]);
				
                updatedPricing.Recurrence = pricingRecurrance;
            }

            return updatedPricing;

        }

        #endregion


        #endregion

        #region Display - Tabs

        private void Display_ContentTab()
        {
            content_title.Value = entry_edit_data.Title;
            if (Util_GetMode() == workareaCommerce.ModeType.Add)
                tr_Properties.Visible = true;
            if (this.m_sEditAction == "addlang")
            {
                language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
                lblLangName.Text = "[" + language_data.Name + "]";
            }
            else
            {
                lblLangName.Text = string.Empty;
            }
            contentEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.DataEntry;
            contentEditor.SetPermissions(m_cPerms);
            contentEditor.AllowFonts = true;
            if (_stylesheet != "")
            {
                contentEditor.Stylesheet = _stylesheetPath;
            }

            contentEditor.LoadPackage(m_refContentApi, editorPackage);
            string strXml = "";
            if (entry_edit_data != null)
            {
                strXml = entry_edit_data.Html;
            }
            if (Strings.Trim(strXml.Length.ToString()) == "0")
            {
                if (editorPackage.Length > 0)
                {
                    strXml = m_refContentApi.TransformXsltPackage(editorPackage, Server.MapPath((string)(contentEditor.ScriptLocation + "unpackageDocument.xslt")), true);
                }
            }
            contentEditor.DataDocumentXml = strXml;

            //set CatalogEntry_PageFunctions_Js vars - see RegisterJS() and CatalogEntry.PageFunctions.aspx under CatalogEntry/js
            _JsPageFunctions_ContentEditorId = "contentEditor";
        }

        private void Display_SummaryTab()
        {
            contentEditor.SetPermissions(m_cPerms);
            contentEditor.AllowFonts = true;
            if (_stylesheet != "")
            {
                summaryEditor.Stylesheet = _stylesheetPath;
            }
            if (entry_edit_data != null)
            {
                summaryEditor.Content = entry_edit_data.Summary;
            }
        }

        private void Display_EntryTab()
        {

            System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
            Ektron.Cms.Common.Criteria<TaxClassProperty> criteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Id, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
            
			//Excluding the shipping Tax class that comes in to the system.5 is the shipping tax id.
            criteria.AddFilter(TaxClassProperty.Id, CriteriaFilterOperator.NotEqualTo, 5);
            m_refTaxClass = new TaxClass(this.m_refContentApi.RequestInformationRef);
            TaxClassList = m_refTaxClass.GetList(criteria);

            drp_taxclass.DataTextField = "name";
            drp_taxclass.DataValueField = "id";
            drp_taxclass.DataSource = TaxClassList;
            drp_taxclass.DataBind();

            if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English)
            {

                ltr_heightmeasure.Text = GetMessage("lbl inches");
                ltr_lengthmeasure.Text = GetMessage("lbl inches");
                ltr_widthmeasure.Text = GetMessage("lbl inches");
                ltr_weightmeasure.Text = GetMessage("lbl pounds");

            }
            else
            {

                ltr_heightmeasure.Text = GetMessage("lbl centimeters");
                ltr_lengthmeasure.Text = GetMessage("lbl centimeters");
                ltr_widthmeasure.Text = GetMessage("lbl centimeters");
                ltr_weightmeasure.Text = GetMessage("lbl kilograms");

            }

            Util_BindFieldList();

            if (entry_edit_data != null)
            {
                txt_sku.Text = entry_edit_data.Sku;
                txt_quantity.Text = entry_edit_data.QuantityMultiple.ToString();
                drp_taxclass.SelectedValue = entry_edit_data.TaxClassId.ToString();
                chk_avail.Checked = entry_edit_data.IsArchived;
                // chk_markdel.Checked = entry_edit_data.IsMarkedForDeletion
                if (entry_edit_data.Id == 0)
                {
                    chk_buyable.Checked = true;
                }
                if (entry_edit_data.IsArchived)
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "chk_buyable", "document.getElementById(\'chk_buyable\').disabled = true;", true);
                }
                if (!entry_edit_data.IsArchived)
                {
                    chk_buyable.Checked = entry_edit_data.IsBuyable;
                }
                else
                {
                    chk_buyable.Checked = false;
                }

                chk_tangible.Checked = entry_edit_data.IsTangible;
                chk_disableInv.Enabled = Util_IsEditable();
                if (Util_IsEditable() == false || !entry_edit_data.IsTangible)
                {
                    txt_height.Enabled = false;
                    txt_length.Enabled = false;
                    txt_width.Enabled = false;
                    txt_weight.Enabled = false;
                }

                txt_height.Text = entry_edit_data.Dimensions.Height.ToString();
                txt_length.Text = entry_edit_data.Dimensions.Length.ToString();
                txt_width.Text = entry_edit_data.Dimensions.Width.ToString();
                txt_weight.Text = entry_edit_data.Weight.Amount.ToString();

                InventoryApi inventoryApi = new InventoryApi();
                InventoryData inventoryData = inventoryApi.GetInventory(entry_edit_data.Id);

                chk_disableInv.Checked = entry_edit_data.DisableInventoryManagement;
                chk_disableInv.Enabled = Util_IsEditable();
                if (Util_IsEditable() == false || entry_edit_data.DisableInventoryManagement)
                {
                    txt_instock.Enabled = false;
                    txt_onorder.Enabled = false;
                    txt_reorder.Enabled = false;
                }

                txt_instock.Text = inventoryData.UnitsInStock.ToString();
                txt_onorder.Text = inventoryData.UnitsOnOrder.ToString();
                txt_reorder.Text = inventoryData.ReorderLevel.ToString();
            }
            else
            {

                txt_height.Enabled = false;
                txt_length.Enabled = false;
                txt_width.Enabled = false;
                txt_weight.Enabled = false;

                txt_instock.Enabled = false;
                txt_onorder.Enabled = false;
                txt_reorder.Enabled = false;

            }

            Util_ToggleProperties(Util_IsEditable());

        }

        private void Display_PricingTab()
        {

            List<CurrencyData> activeCurrencyList = m_refCurrency.GetActiveCurrencyList();
            List<ExchangeRateData> exchangeRateList = new List<ExchangeRateData>();
            if (activeCurrencyList.Count > 1)
            {
                ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
                Criteria<ExchangeRateProperty> exchangeRateCriteria = new Criteria<ExchangeRateProperty>();
                List<long> currencyIDList = new List<long>();
                for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
                {
                    currencyIDList.Add(activeCurrencyList[i].Id);
                }
                exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
                exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, CriteriaFilterOperator.In, currencyIDList.ToArray());
                exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria);
            }

            bool showPricingTier = this.ShowPricingTier();
            ltr_pricing.Text = this.CommerceLibrary.GetPricingMarkup(entry_edit_data.Pricing, activeCurrencyList, exchangeRateList, entry_edit_data.EntryType, showPricingTier, Util_GetMode());

        }

        private bool ShowPricingTier()
        {
            bool returnValue = true;
            switch (entry_edit_data.EntryType)
            {
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
                    BundleData bundleData = (BundleData)entry_edit_data;
                    returnValue = System.Convert.ToBoolean(bundleData.BundledItems.Count > 0 ? false : true);
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct:
                    ProductData complexProductData = (ProductData)entry_edit_data;
                    returnValue = System.Convert.ToBoolean(complexProductData.Variants.Count > 0 ? false : true);
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
                    KitData kitData = (KitData)entry_edit_data;
                    returnValue = System.Convert.ToBoolean(kitData.OptionGroups.Count > 0 ? false : true);
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product:
                    ProductData productData = (ProductData)entry_edit_data;
                    returnValue = System.Convert.ToBoolean(productData.Variants.Count > 0 ? false : true);
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct:
                    returnValue = false;
                    break;
            }

            return returnValue;
        }

        private void Display_ItemTab()
        {

            if (entry_edit_data != null)
            {
                this.ucItem.EntryEditData = entry_edit_data;
                this.ucItem.ItemsFolderId = m_iFolder;
                this.ucItem.SubscriptionControlPath = this.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/Membership.ascx";
                if (Util_IsEditable() == true)
                {
                    this.ucItem.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.Edit;
                }
                else
                {
                    this.ucItem.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.View;
                }
            }
        }

        private void Display_MediaTab()
        {

            ucMedia.EntryEditData = entry_edit_data;
            ucMedia.ProductId = xid;
            ucMedia.FolderId = m_iFolder;
            if (Util_IsEditable() == true)
            {
                ucMedia.DisplayMode = Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.Edit;
            }
            else
            {
                ucMedia.DisplayMode = Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.View;
            }
        }

        private void Display_MetadataTab()
        {

            StringBuilder sbAttrib = new StringBuilder();
            StringBuilder sbResult = new StringBuilder();
            string strResult;
            string strAttrResult;
            string strImage = "";

            //Dim enhancedMetadataScript As New Literal
            //enhancedMetadataScript.Text = Replace(CustomFields.GetEnhancedMetadataScript(), "src=""java/", "src=""../java/")
            //Me.Page.Header.Controls.Add(enhancedMetadataScript)
            EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea();
            if ((meta_data != null) || (prod_type_data != null))
            {
                m_refSite = new Ektron.Cms.Site.EkSite(this.m_refContentApi.RequestInformationRef);
                Hashtable hPerm = m_refSite.GetPermissions(m_iFolder, 0, "folder");
                if (meta_data != null)
                {
                    sbResult = CustomFields.WriteFilteredMetadataForEdit(meta_data.ToArray(), false, m_sEditAction, m_iFolder, ref lValidCounter, hPerm);
                    if (sbResult.ToString().Contains("<span style=\"color:red\">"))
                        metadataRequired = true;
                }
                if (prod_type_data != null)
                {

                    if (Util_IsEditable())
                    {

                        sbAttrib = CustomFields.WriteFilteredAttributesForEdit(entry_edit_data.Attributes, m_sEditAction, xid, prod_type_data.Attributes, ref lValidCounter, hPerm);
                    }
                    else
                    {
                        sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(entry_edit_data.Attributes, xid, false, prod_type_data.Attributes));
                    }
                }
            }
            if (m_sEditAction == "update")
            {
                strImage = entry_edit_data.Image;
                string strThumbnailPath = entry_edit_data.ImageThumbnail;
                if (entry_edit_data.ImageThumbnail == "")
                {
                    strThumbnailPath = m_refContentApi.AppImgPath + "spacer.gif";
                }
                else
                {
                    strThumbnailPath = m_refContentApi.SitePath + strThumbnailPath;
                }
                if (System.IO.Path.GetExtension(strThumbnailPath).ToLower().IndexOf(".gif") != -1 && strThumbnailPath.ToLower().IndexOf("spacer.gif") == -1)
                {
                    strThumbnailPath = strThumbnailPath.Replace(".gif", ".png");
                }
                // sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""info"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""entry_image"" name=""entry_image"" value=""" & strImage & """ /> <a href=""#"" onclick=""PopUpWindow('../mediamanager.aspx?scope=images&upload=true&retfield=entry_image&showthumb=false&autonav=" & catalog_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">Change</a>&nbsp;<a href=""#"" onclick=""RemoveEntryImage('" & m_refContentApi.AppImgPath & "spacer.gif');return false"">Remove</a></td></tr><tr><td colomnspan=""2""><img id=""entry_image_thumb"" src=""" & strThumbnailPath & """ /></td></tr></table></fieldset>")
            }
            else
            {
                // sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""info"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""entry_image"" name=""entry_image"" value=""" & strImage & """ /> <a href=""#"" onclick=""PopUpWindow('../mediamanager.aspx?scope=images&upload=true&retfield=entry_image&showthumb=false&autonav=" & catalog_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">Change</a>&nbsp;<a href=""#"" onclick=""RemoveEntryImage('" & m_refContentApi.AppImgPath & "spacer.gif');return false"">Remove</a></td></tr><tr><td colomnspan=""2""><img id=""entry_image_thumb"" src=""" & m_refContentApi.AppImgPath & "spacer.gif"" /></td></tr></table></fieldset>")
            }

            if (this.m_sEditAction == "addlang")
            {
                sbResult.Append("<input type=\"hidden\" id=\"entry_image\" name=\"entry_image\" value=\"" + entry_edit_data.Image + "\" />");
            }

            strAttrResult = (string)(sbAttrib.ToString().Trim());
            strAttrResult = strAttrResult.Replace("src=\"java/", "src=\"../java/");
            strAttrResult = strAttrResult.Replace("src=\"images/", "src=\"../images/");

            strResult = sbResult.ToString().Trim();
            strResult = Util_FixPath(strResult);
            strResult = strResult.Replace("src=\"java/", "src=\"../java/");
            strResult = strResult.Replace("src=\"images/", "src=\"../images/");

            ltr_meta.Text = strResult;
            ltr_attrib.Text = strAttrResult;
        }

        private void Display_ScheduleTab()
        {

            EkDTSelector dateSchedule;
            int end_date_action = 1;
            string go_live = "";
            string end_date = "";

            if (entry_edit_data != null)
            {
                go_live = entry_edit_data.GoLive.ToString();
                if (!(entry_edit_data.EndDate == DateTime.MinValue || entry_edit_data.EndDate == DateTime.MaxValue))
                {
                    end_date = entry_edit_data.EndDate.ToString();
                }
                end_date_action = entry_edit_data.EndDateAction;
            }

            dateSchedule = this.m_refContentApi.EkDTSelectorRef;
            dateSchedule.formName = "frmMain";
            dateSchedule.extendedMeta = true;
            // start
            dateSchedule.formElement = "go_live";
            dateSchedule.spanId = "go_live_span";
            if (go_live != "")
            {
                dateSchedule.targetDate = DateTime.Parse(go_live);
            }
            ltr_startdatesel.Text = dateSchedule.displayCultureDateTime(true, dateSchedule.spanId, dateSchedule.formElement);
            dateSchedule.formElement = "end_date";
            dateSchedule.spanId = "end_date_span";
            if (end_date != "")
            {
                dateSchedule.targetDate = DateTime.Parse(end_date);
            }
            else
            {
                dateSchedule.targetDate = DateTime.MinValue;
            }

            ltr_enddatesel.Text = dateSchedule.displayCultureDateTime(true, dateSchedule.spanId, dateSchedule.formElement);

            // end
            // action
            rblaction.Items.Add(new ListItem("Archive and remove from site (expire)", "1"));
            rblaction.Items.Add(new ListItem("Archive and remain on site", "2"));
            rblaction.Items.Add(new ListItem("Add to the CMS Refresh Report", "3"));
            // action
            if (this.m_sEditAction == "add")
            {
                rblaction.SelectedIndex = 0;
            }
            else
            {
                switch (end_date_action)
                {
                    case 3:
                        rblaction.SelectedIndex = 2;
                        break;
                    case 2:
                        rblaction.SelectedIndex = 1;
                        break;
                    default:
                        rblaction.SelectedIndex = 0;
                        break;
                }
            }

        }

        private void Display_TaxonomyTab()
        {

            if (m_cPerms.CanEdit || m_cPerms.CanAdd || (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, m_refContentApi.RequestInformationRef.UserId, false)))
            {
                TaxonomyRoleExists = true;
            }
            EditTaxonomyHtml.Text = "<p class=\"info\">" + this.m_refMsg.GetMessage("lbl select categories entry") + "</p><div id=\"TreeOutput\"></div>";
            lit_add_string.Text = m_refMsg.GetMessage("generic add title");

            TaxonomyBaseData[] taxonomy_cat_arr = null;
            m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage;
            m_refContentApi.ContentLanguage = ContentLanguage;

            TaxonomyRequest taxonomy_request = new TaxonomyRequest();
            TaxonomyBaseData[] taxonomy_data_arr = null;
            if (m_sEditAction == "add")
            {
                if ((Request.QueryString["SelTaxonomyId"] != null) && Request.QueryString["SelTaxonomyId"] != "")
                {
                    TaxonomySelectId = Convert.ToInt64(Request.QueryString["SelTaxonomyId"]);
                }
                if (TaxonomySelectId > 0)
                {
                    taxonomyselectedtree.Value = TaxonomySelectId.ToString();
                    TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
                    taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(TaxonomySelectId, m_refContentApi.ContentLanguage, 0);
                    if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                    {
                        foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                        {
                            if (TaxonomyTreeParentIdList == "")
                            {
                                TaxonomyTreeParentIdList = Convert.ToString(taxonomy_cat.TaxonomyId);
                            }
                            else
                            {
                                TaxonomyTreeParentIdList = TaxonomyTreeParentIdList + "," + Convert.ToString(taxonomy_cat.TaxonomyId);
                            }
                        }
                    }
                }
            }
            else
            {
                taxonomy_cat_arr = m_refContentApi.EkContentRef.ReadAllAssignedCategory(m_iID);
                if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                {
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                    {
                        if (taxonomyselectedtree.Value == "")
                        {
                            taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                        else
                        {
                            taxonomyselectedtree.Value = taxonomyselectedtree.Value + "," + Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                    }
                }
                TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
                if (TaxonomyTreeIdList.Trim().Length > 0)
                {
                    TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(m_iID);
                }
            }

            taxonomy_request.TaxonomyId = m_iFolder;
            taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
            taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(m_iFolder);

            if ((taxonomy_data_arr == null || taxonomy_data_arr.Length == 0) && (TaxonomyOverrideId == 0))
            {
                ShowTaxonomyTab = false;
            }

            m_intTaxFolderId = m_iFolder;
            //If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            //    TaxonomyOverrideId = Convert.ToInt32(Request.QueryString("TaxonomyId"))
            //End If

            //set CatalogEntry_Taxonomy_A_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.A.aspx under CatalogEntry/js
            this._JSTaxonomyFunctions_TaxonomyTreeIdList = EkFunctions.UrlEncode(TaxonomyTreeIdList);
            this._JSTaxonomyFunctions_TaxonomyTreeParentIdList = EkFunctions.UrlEncode(TaxonomyTreeParentIdList);
            this._JSTaxonomyFunctions_TaxonomyOverrideId = TaxonomyOverrideId.ToString();
            this._JSTaxonomyFunctions_TaxonomyFolderId = m_iFolder.ToString();

        }

        private void Display_CommentTab()
        {

            if (entry_edit_data != null)
            {
                txt_comment.Text = entry_edit_data.Comment;
            }

        }

        private void Display_TemplateTab()
        {



            TemplateData[] active_template_list = m_refContentApi.GetEnabledTemplatesByFolder(catalog_data.Id);
            long default_template = 0;

            if (active_template_list.Length < 1)
            {
                bSuppressTemplate = true;
            }
            if (this.m_sEditAction == "update")
            {
                default_template = entry_edit_data.TemplateId;
            }
            if (default_template == 0)
            {
                default_template = catalog_data.TemplateId;
            }

            drp_tempsel.DataValueField = "Id";
            drp_tempsel.DataTextField = "FileName";
            drp_tempsel.DataSource = active_template_list;
            drp_tempsel.DataBind();

            for (int i = 0; i <= (active_template_list.Length - 1); i++)
            {

                if (active_template_list[i].Id == default_template)
                {
                    drp_tempsel.SelectedIndex = i;
                    break;
                }

            }

        }

        private void Display_AliasTab()
        {

            StringBuilder sbHtml = new StringBuilder();
            Ektron.Cms.UrlAliasing.UrlAliasManualApi _manualAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
            Ektron.Cms.UrlAliasing.UrlAliasAutoApi _autoaliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
            System.Collections.Generic.List<string> aliasExtensions;
            string ext = "";
            int i;

            Ektron.Cms.Common.UrlAliasManualData defaultManualAlias = new Ektron.Cms.Common.UrlAliasManualData(0, 0, string.Empty, string.Empty);
            System.Collections.Generic.List<UrlAliasAutoData> autoAliasList = new System.Collections.Generic.List<UrlAliasAutoData>();

            aliasExtensions = _manualAliasApi.GetFileExtensions();
            if (entry_edit_data != null)
            {
                defaultManualAlias = _manualAliasApi.GetDefaultAlias(entry_edit_data.Id);
            }
            sbHtml.Append("<div>");
            if (_urlAliasSettingApi.IsManualAliasEnabled)
            {
                if (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
                {
                    sbHtml.Append("<fieldset><legend><strong>" + m_refMsg.GetMessage("lbl tree url manual aliasing") + "</strong></legend>");
                    sbHtml.Append("<table width=\"100%\" border=\"0\" cellpadding=\"2\" cellspacing=\"2\">");
                    sbHtml.Append("<tr><td colspan=4>&nbsp;<br></td></tr>");
                    sbHtml.Append("<tr><td>&nbsp;</td><td class=\"info\" nowrap=\"true\">" + m_refMsg.GetMessage("lbl primary") + " " + m_refMsg.GetMessage("lbl alias name") + ":&nbsp;");
                    sbHtml.Append("<td>&nbsp;<input type=\"hidden\" name=\"frm_manalias_id\" value=\"" + defaultManualAlias.AliasId + "\"></td>");
                    sbHtml.Append("<td>&nbsp;<input type=\"hidden\" name=\"prev_frm_manalias_name\" value=\"" + defaultManualAlias.AliasName + "\"></td>");
                    sbHtml.Append("<td>&nbsp;<input type=\"hidden\" name=\"prev_frm_manalias_ext\" value=\"" + defaultManualAlias.FileExtension + "\"></td>");
                    if (catalog_data.IsDomainFolder)
                    {
                        sbHtml.Append("<td width=\"95%\">http://" + catalog_data.DomainProduction + "/<input type=\"text\"  size=\"35\" id=\"frm_manalias\" name=\"frm_manalias\" value=\"" + defaultManualAlias.AliasName + "\">");
                    }
                    else
                    {
                        sbHtml.Append("<td width=\"95%\">" + m_refContentApi.SitePath + "<input type=\"text\"  size=\"35\" id=\"frm_manalias\" name=\"frm_manalias\" value=\"" + defaultManualAlias.AliasName + "\">");
                    }

                    for (i = 0; i <= aliasExtensions.Count - 1; i++)
                    {
                        if (ext != "")
                        {
                            ext = ext + ",";
                        }
                        ext = ext + aliasExtensions[i];
                    }
                    sbHtml.Append(m_refContentApi.RenderHTML_RedirExtensionDD("frm_ManAliasExt", defaultManualAlias.FileExtension, ext));
                    sbHtml.Append("<br/></td>");
                    sbHtml.Append("</tr></table></fieldset>");
                    sbHtml.Append("<br/><br/><br/>");
                }
                else
                {
                    sbHtml.Append("<input type=\"hidden\" name=\"frm_manalias_id\" value=\"" + defaultManualAlias.AliasId + "\">");
                    sbHtml.Append("<input type=\"hidden\" name=\"frm_manalias\" value=\"" + defaultManualAlias.AliasName + defaultManualAlias.FileExtension + "\">");
                }

            }
            if (_urlAliasSettingApi.IsAutoAliasEnabled)
            {
                if (entry_edit_data != null)
                {
                    autoAliasList = _autoaliasApi.GetListForContent(entry_edit_data.Id);
                }
                sbHtml.Append("<div class=\"autoAlias\" style=\"width: auto; height: auto; overflow: auto;\" id=\"autoAliasList\">");
                sbHtml.Append("<fieldset><legend><strong>" + m_refMsg.GetMessage("lbl automatic") + "</strong></legend><br/>");
                sbHtml.Append("<table width=\"100%\" border=\"0\" cellpadding=\"2\" cellspacing=\"2\">");
                sbHtml.Append("<tr><td><u><strong>" + m_refMsg.GetMessage("generic type") + "</strong></u></td>");
                sbHtml.Append("<td><u><strong>" + m_refMsg.GetMessage("lbl alias name") + "</strong></u></td></tr>");
                for (i = 0; i <= autoAliasList.Count() - 1; i++)
                {
                    sbHtml.Append("<tr><td>" + autoAliasList[i].AutoAliasType.ToString() + "</td>");
                    sbHtml.Append("<td>" + autoAliasList[i].AliasName + "</td></tr>");
                }
                sbHtml.Append("</table></fieldset></div>");
            }
            sbHtml.Append("</div>");
            ltrEditAlias.Text = sbHtml.ToString();
        }

        #endregion

        #region Util

        private void Util_SetXmlId(long xmlId)
        {
            hdn_xmlid.Value = xmlId.ToString();
            ltr_xmlid.Text = string.Format(@"<input type=""hidden"" name=""xid"" id=""xid"" value=""{0}"" />", xmlId);
        }
        private void Util_SetLabels()
        {

            base.Version8TabsImplemented = true;
            //session expiration
            lbl_SessionExpiringLabel.Text = GetMessage("editor session expiring 10");
            lbl_ContinueEditingLabel.Text = GetMessage("continue editing");

            //set title
            lbl_GenericTitleLabel.Text = m_refMsg.GetMessage("generic title label");
            //set searchable
            chk_searchable.Text = m_refMsg.GetMessage("lbl searchable");

            //content tab
            liContent.Visible = true;
            divContent.Visible = true;
            litTabContentLabel.Text = base.m_refMsg.GetMessage("content text");

            //summary tab
            liSummary.Visible = true;
            divSummary.Visible = true;
            litTabSummaryLabel.Text = base.m_refMsg.GetMessage("summary text");

            //properties tab
            liProperties.Visible = true;
            divProperties.Visible = true;
            litTabPropertiesLabel.Text = base.m_refMsg.GetMessage("properties text");

            //comment tab - not implemented
            //liComment.Visible = True
            //divComment.Visible = True
            //litTabCommentLabel.Text = MyBase.m_refMsg.GetMessage("comment text")

            //pricing tab
            liPricing.Visible = true;
            divPricing.Visible = true;
            litTabPricingLabel.Text = base.m_refMsg.GetMessage("lbl pricing");

            //attributes tab
            if (prod_type_data.Attributes.Count > 0)
            {
                liAttributes.Visible = true;
                divAttributes.Visible = true;
                litTabAttributesLabel.Text = base.m_refMsg.GetMessage("lbl entry attrib tab");
            }

            //items tab
            liItems.Visible = true;
            divItems.Visible = true;
            litTabItemsLabel.Text = base.m_refMsg.GetMessage("lbl variants");

            //media tab
            liMedia.Visible = true;
            divMedia.Visible = true;
            litTabMediaLabel.Text = base.m_refMsg.GetMessage("lbl media");

            //metadata tab
            liMetadata.Visible = true;
            divMetadata.Visible = true;
            litTabMetadataLabel.Text = base.m_refMsg.GetMessage("metadata text");

            //schedule tab
            liSchedule.Visible = true;
            divSchedule.Visible = true;
            litTabScheduleLabel.Text = base.m_refMsg.GetMessage("schedule text");

            //category tab
            if (ShowTaxonomyTab)
            {
                liCategory.Visible = true;
                divCategories.Visible = true;
                litTabCateogoryLabel.Text = base.m_refMsg.GetMessage("lbl category");
            }

            //alias tab
            if ((_urlAliasSettingApi.IsManualAliasEnabled || _urlAliasSettingApi.IsAutoAliasEnabled) && m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
            {
                liAlias.Visible = true;
                divAlias.Visible = true;
                litTabAliasLabel.Text = base.m_refMsg.GetMessage("lbl alias");
            }

            //templates tab - not implemented
            //If Not bSuppressTemplate Then
            //liTemplates.Visible = True
            //divTemplates.Visible = True
            //litTabTemplatesLabel.Text = MyBase.m_refMsg.GetMessage("template label")
            //End If

            this.MenuCheckVariable = "checkVariable";

            this.AddButton(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "#", this.GetMessage("generic undocheckout"), this.GetMessage("generic cancel"), "onclick=\"SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Cancel) + ");\"", StyleHelper.CancelButtonCssClass, true);

            switch (this.m_sEditAction)
            {
                case "update":
                    workareamenu actionMenu_1 = new workareamenu("action", this.GetMessage("lbl action"), m_refContentApi.AppPath + "images/UI/Icons/check.png"); // check2.gif
                    actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/save.png", this.GetMessage("btn save"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Save) + "); ");
                    string aURL = entry_edit_data.Quicklink;
                    // determine if there is a querystring or not
                    if (aURL.IndexOf("?") > -1)
                    {
                        aURL += "&cmsMode=Preview";
                    }
                    else
                    {
                        aURL += "?cmsMode=Preview";
                    }
                    actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/preview.png", this.GetMessage("btn preview"), " PreviewContent(\'" + aURL + "\', " + Convert.ToInt32(EkEnumeration.AssetActionType.Save) + ", \'" + entry_edit_data.Title + "\'); return false; ");

                    actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/checkIn.png", this.GetMessage("dmsmenucheckin"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Checkin) + "); ");
                    if (UserRights.CanPublish)
                    {
                        actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentPublish.png", this.GetMessage("generic publish"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Submit) + "); ");
                    }
                    else
                    {
                        actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentPublish.png", this.GetMessage("btn submit"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Submit) + "); ");
                    }
                    actionMenu_1.AddBreak();
                    actionMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/cancel.png", this.GetMessage("generic undocheckout"), "SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.UndoCheckout) + "); ");
                    this.AddMenu(actionMenu_1);

                    workareamenu miscMenu_1 = new workareamenu("misc", this.GetMessage("btn change"), this.AppImgPath + "menu/product.gif"); // check2.gif $ektron('" + uxDialog.Selector + "').dialog('open');
                    miscMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/comment.png", this.GetMessage("comment text"), (string)("$ektron(\'" + divComment.Selector + "\').dialog('open');"));
                    if (!bSuppressTemplate)
                    {
                        miscMenu_1.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentTemplate.png", this.GetMessage("template label"), (string)("$ektron(\'" + divTemplates.Selector + "\').dialog('open');"));
                    }
                    this.AddMenu(miscMenu_1);

                    this.SetTitleBarToMessage("lbl edit catalog entry");
                    this.AddHelpButton("editcatentry");
                    break;
                case "add":
                case "addlang":
                    workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), m_refContentApi.AppPath + "images/UI/Icons/check.png"); // check2.gif
                    actionMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/checkIn.png", this.GetMessage("dmsmenucheckin"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Checkin) + "); ");
                    if (UserRights.CanPublish)
                    {
                        actionMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentPublish.png", this.GetMessage("generic publish"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Submit) + "); ");
                    }
                    else
                    {
                        actionMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentPublish.png", this.GetMessage("btn submit"), " SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Submit) + "); ");
                    }
                    actionMenu.AddBreak();
                    actionMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/cancel.png", this.GetMessage("generic undocheckout"), "SubmitForm(" + Convert.ToInt32(EkEnumeration.AssetActionType.Cancel) + "); ");
                    this.AddMenu(actionMenu);

                    workareamenu miscMenu = new workareamenu("misc", this.GetMessage("btn change"), m_refContentApi.AppImgPath + "menu/product.gif"); // check2.gif
                    miscMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/comment.png", this.GetMessage("comment text"), (string)("$ektron(\'" + divComment.Selector + "\').dialog('open');"));
                    if (!bSuppressTemplate)
                    {
                        miscMenu.AddItem(m_refContentApi.AppPath + "Images/ui/icons/contentTemplate.png", this.GetMessage("template label"), (string)("$ektron(\'" + divTemplates.Selector + "\').dialog('open');"));
                    }
                    this.AddMenu(miscMenu);

                    this.SetTitleBarToMessage("lbl add catalog entry");
                    this.AddHelpButton("addcatentry");
                    break;
            }

            //Dim tc As New TableCell()
            //Dim tr As New TableRow()
            //tc.Controls.Add(New LiteralControl(Util_GetTitleField()))
            //tr.Controls.Add(tc)
            //Me.AddTableRow(tr)

            // labels
            ltr_sku.Text = GetMessage("lbl calatog entry sku");
            ltr_quantity.Text = GetMessage("lbl number of units");
            ltr_avail.Text = GetMessage("lbl archived");
            chk_avail.Attributes.Add("onclick", "ToggleAvail(this);");
            // ltr_markdel.Text = GetMessage("lbl deleted")
            ltr_buyable.Text = GetMessage("lbl buyable");
            ltr_taxclass.Text = GetMessage("lbl tax class");
            ltr_tangible.Text = GetMessage("lbl tangible");
            chk_tangible.Attributes.Add("onclick", "ToggleTangible(this);");
            ltr_height.Text = GetMessage("lbl height");
            ltr_width.Text = GetMessage("lbl width");
            ltr_length.Text = GetMessage("lbl length");
            // ltr_weightmeasure.Text = GetMessage("lbl weight measure")
            ltr_weight.Text = GetMessage("lbl weight");
            ltr_disableInv.Text = GetMessage("lbl disable inventory");
            ltr_instock.Text = GetMessage("lbl in stock");
            ltr_onorder.Text = GetMessage("lbl on order");
            ltr_reorder.Text = GetMessage("lbl reorder");
            // ltr_currency.Text = GetMessage("lbl currency")
            ltr_comment.Text = GetMessage("comment text");
            //ltr_tempsel.Text = GetMessage("lbl template selection")
            ltr_actionend.Text = GetMessage("end date action title");
            ltr_startdate.Text = GetMessage("generic go live");
            ltr_enddate.Text = GetMessage("generic end date");
            ltr_ship.Text = GetMessage("lbl dimensions");
            ltr_inv.Text = GetMessage("lbl inventory");
            chk_disableInv.Attributes.Add("onclick", "ToggleInventory(this);");

            string lblOk = GetMessage("lbl ok");
            cmdCommentOk.Text = " " + lblOk + " ";
            cmdCommentOk.ToolTip = lblOk;
            cmdCommentOk.Attributes.Add("onclick", "$ektron('#" + hdnComment.ClientID + "').val($ektron('#" + txt_comment.ClientID + "').val());$ektron('" + divComment.Selector + "').dialog('close'); ; return false;");
            cmdTemplateOk.Text = " " + lblOk + " ";
            cmdTemplateOk.ToolTip = lblOk;
            cmdTemplateOk.Attributes.Add("onclick", "$ektron('" + divTemplates.Selector + "').dialog('close'); ; return false;");

            ltr_holdmsg.Text = m_refMsg.GetMessage("one moment msg");
        }

        protected string Util_GetTitleField()
        {

            StringBuilder sbTitle = new StringBuilder();
            LocalizationAPI objLocalizationApi = new LocalizationAPI();
            LanguageData language_data = m_refContentApi.EkSiteRef.GetLanguageDataById(ContentLanguage);

            sbTitle.Append(" <table border=\"0\" cellpadding=\"2\" cellspacing=\"2\"> ").Append(Environment.NewLine);
            sbTitle.Append("     <tr> ").Append(Environment.NewLine);
            sbTitle.Append("         <td>").Append(GetMessage("generic title")).Append(":</td> ").Append(Environment.NewLine);
            sbTitle.Append("         <td nowrap=\"nowrap\" align=\"left\"> ").Append(Environment.NewLine);
            sbTitle.Append("             <input name=\"content_title\" type=\"text\" id=\"content_title\" size=\"50\" maxlength=\"200\" onkeypress=\"return CheckKeyValue(event, \'34,13\');\" value=\"");
            if (entry_edit_data != null)
            {
                sbTitle.Append(EkFunctions.HtmlEncode(entry_edit_data.Title));
            }
            sbTitle.Append("\" /> [").Append(language_data.Name).Append("] ").Append(Environment.NewLine);
            sbTitle.Append("<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' border=\"0\" />");
            sbTitle.Append("         </td> ").Append(Environment.NewLine);
            sbTitle.Append("         <td>&nbsp;</td> ").Append(Environment.NewLine);
            sbTitle.Append("     </tr> ").Append(Environment.NewLine);
            sbTitle.Append(" </table> ").Append(Environment.NewLine);

            return sbTitle.ToString();

        }

        protected void Util_CheckFolderType()
        {
            catalog_data = m_refContentApi.GetFolderById(m_iFolder);
            if (catalog_data.FolderType != Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Catalog))
            {
                throw (new Exception("Not a catalog"));
            }
            _stylesheet = m_refContentApi.GetStyleSheetByFolderID(catalog_data.Id);
            _stylesheetPath = (string)(Util_GetServerPath() + m_refContentApi.SitePath + _stylesheet);
        }

        protected void Util_CheckAccess()
        {

        }

        protected void Util_ObtainValues()
        {
            if (Request.QueryString["back_LangType"] != "")
            {
                backLangType = Convert.ToInt64(Request.QueryString["back_LangType"]);
            }
            if (Request.QueryString["content_id"] != "")
            {
                otherLangId = Convert.ToInt64(Request.QueryString["content_id"]);
            }
            if (Request.QueryString["type"] != "")
            {
                m_sEditAction = Request.QueryString["type"];
            }
            if (!(Request.QueryString["folder_id"] == null)) // add
            {
                m_iFolder = Convert.ToInt64(Request.QueryString["folder_id"]);
            }
            if (!(Request.QueryString["back_folder_id"] == null)) // edit
            {
                m_iFolder = Convert.ToInt64(Request.QueryString["back_folder_id"]);
            }
            if (Request.QueryString["xid"] != "")
            {
                xid = Convert.ToInt64(Request.QueryString["xid"]);
                if (xid > 0)
                {
                    m_refProductType = new ProductType(m_refContentApi.RequestInformationRef);
                    prod_type_data = m_refProductType.GetItem(xid, true);
                    this.editorPackage = prod_type_data.PackageXslt;
                    hdn_entrytype.Value = prod_type_data.EntryClass.ToString();
                }
            }
            Util_SetXmlId(xid);

            m_cPerms = m_refContentApi.LoadPermissions(m_iFolder, "folder", 0);

            if (Request.QueryString["incontext"] != "")
            {
                _inContextEditing = Convert.ToBoolean(Request.QueryString["incontext"]);
            }

            //m_mMeasures = New Measurements(m_refContentApi.RequestInformationRef).GetMeasurements()
        }

        private void Util_SetJS()
        {
            string id = this.m_iID.ToString();
            //set CatalogEntry_Taxonomy_B_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.B.aspx under CatalogEntry/js
            this._JSTaxonomyFunctions_ShowTaxonomy = TaxonomyRoleExists.ToString();
            this._JSTaxonomyFunctions_FolderId = m_intTaxFolderId.ToString();
        }

        private string Util_FixPath(string MetaScript)
        {
            int iTmp = -1;
            iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0);
            while (iTmp > -1)
            {
                iTmp = MetaScript.IndexOf(");return false;", iTmp);
                MetaScript = MetaScript.Insert(iTmp, ", \'" + this.m_refContentApi.ApplicationPath + "\'");
                iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1);
            }
            return MetaScript;
        }

        public void Util_BindFieldList()
        {
            if (prod_type_data != null)
            {
                System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                System.Xml.XmlNodeList xList;
                if (prod_type_data.FieldList != "")
                {
                    xDoc.LoadXml(prod_type_data.FieldList);
                    xList = xDoc.SelectNodes("/fieldlist/field/@xpath");
                    for (int i = 0; i <= (xList.Count - 1); i++)
                    {
                        drp_field.Items.Add(xList[i].Value);
                        drp_field2.Items.Add(xList[i].Value);
                    }
                }
            }
            chk_field.Visible = false;
            drp_field.Visible = false;
            chk_field2.Visible = false;
            drp_field2.Visible = false;
        }

        public void Util_GetEntryType()
        {
            switch (prod_type_data.EntryClass)
            {

                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct:

                    entry_edit_data = new SubscriptionProductData();
                    entry_edit_data.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct;
                    break;

                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
                    entry_edit_data = new BundleData();
                    entry_edit_data.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle;
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
                    entry_edit_data = new KitData();
                    entry_edit_data.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit;
                    break;
                default:
                    entry_edit_data = new ProductData();
                    entry_edit_data.EntryType = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product;
                    break;
            }
        }

        private string GetLocaleFileString(string localeFileNumber)
        {
            string LocaleFileString;
            if (localeFileNumber == "" || int.Parse(localeFileNumber) == 1)
            {
                LocaleFileString = "0000";
            }
            else
            {
                LocaleFileString = new string('0', 4 - Conversion.Hex(localeFileNumber).Length);
                LocaleFileString = LocaleFileString + Conversion.Hex(localeFileNumber);
                if (!System.IO.File.Exists(Server.MapPath(m_refContentApi.AppeWebPath + "locale" + LocaleFileString + "b.xml")))
                {
                    LocaleFileString = "0000";
                }
            }
            return LocaleFileString.ToString();
        }

        private workareaCommerce.ModeType Util_GetMode()
        {

            workareaCommerce.ModeType mode = workareaCommerce.ModeType.Edit;

            if (!Util_IsEditable())
            {

                mode = workareaCommerce.ModeType.View;

            }
            else if (m_sEditAction == "add" || m_sEditAction == "addlang")
            {

                mode = workareaCommerce.ModeType.Add;

            }

            return mode;

        }

        private bool Util_IsEditable()
        {

            bool editable = true;

            if (m_sEditAction == "addlang")
            {

                editable = false;

            }
            else if (entry_edit_data != null)
            {

                editable = System.Convert.ToBoolean(entry_edit_data.StatusLanguage == 0 || (entry_edit_data.StatusLanguage == ContentLanguage));

            }

            return editable;

        }

        private void Util_ToggleProperties(bool editable)
        {

            txt_sku.Enabled = editable;
            txt_quantity.Enabled = editable;

            drp_taxclass.Enabled = editable;

            chk_avail.Enabled = editable;
            // chk_markdel.Enabled = editable
            chk_buyable.Enabled = editable;

            //txt_height.Enabled = editable
            //txt_width.Enabled = editable
            //txt_length.Enabled = editable
            //txt_weight.Enabled = editable

            //txt_instock.Enabled = editable
            //txt_onorder.Enabled = editable
            //txt_reorder.Enabled = editable
            chk_tangible.Enabled = editable;

        }

        private void Util_UndoCheckoutResponseHandler(long entryId, long entryFolderId, int entryLanguageId, int currentLanguageId)
        {
            string undoCheckoutUrl = "";
            if (PullApproval)
            {
                undoCheckoutUrl = string.Format(
                   "../approval.aspx?action=viewContent&page=tree&id={0}&LangType={1}&rptType={2}",
                   entryId,
                   currentLanguageId,
                   EkEnumeration.CMSContentType.CatalogEntry.GetHashCode()
                   );
            }
            else
            {   // goes to content view screen
                undoCheckoutUrl = string.Format(
                    "../content.aspx?action=View&folder_id={1}&id={0}&LangType={3}&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d{1}%26contentid%3d0%26form_id%3d0%26LangType%3d{2}",
                    entryId,
                    entryFolderId,
                    entryLanguageId,
                    currentLanguageId
                    );
            }
            Util_ResponseHandler(undoCheckoutUrl);
        }

        private void Util_ResponseHandler(string redirectUrl)
        {

            if (_inContextEditing)
            {

                Page.ClientScript.RegisterStartupScript(typeof(Page), "ReloadAndClose", "opener.location.href = opener.location; self.close();", true);

            }
            else
            {

                Response.Redirect(redirectUrl, false);

            }

        }

        private string Util_GetServerPath()
        {

            string strPath;

            if (Request.ServerVariables["SERVER_PORT_SECURE"] == "1")
            {
                strPath = (string)("https://" + Request.ServerVariables["SERVER_NAME"]);
                if (Request.ServerVariables["SERVER_PORT"] != "443")
                {
                    strPath = strPath + ":" + Request.ServerVariables["SERVER_PORT"];
                }
            }
            else
            {
                strPath = (string)("http://" + Request.ServerVariables["SERVER_NAME"]);
                if (Request.ServerVariables["SERVER_PORT"] != "80")
                {
                    strPath = strPath + ":" + Request.ServerVariables["SERVER_PORT"];
                }
            }

            return strPath;

        }

        #endregion

        #region Css, Js

        private void RegisterCss()
        {
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/box.css", "EktronBoxCss");
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/tables/tableutil.css", "EktronTableUtilCss");
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/commerce/Ektron.Commerce.Pricing.css", "EktronCommercePricingCss");
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/commerce/Ektron.Commerce.Pricing.ie6.css", "EktronCommercePricingCss", Ektron.Cms.API.Css.BrowserTarget.IE6);
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/css/CatalogEntry.css", "EktronCommerceCatalogEntryCss");
            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss");

            Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/commerce/Ektron.Commerce.Session.css", "EktronCommerceSessionCss");

        }

        private void RegisterJs()
        {
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
            //
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/internCalendarDisplayFuncs.js", "EktronInternalCalendarDisplayJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/searchfuncsupport.js", "EktronSearchFunctionSupportJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/dhtml/tableutil.js", "EktronTableUtilitiesJsCatalogEntry", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/metadata_selectlist.js", "EktronMetadataSelectListJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/toolbar_roll.js", "EktronToolbarRollJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/commerce/com.Ektron.Commerce.Pricing.js", "EktronPricingJs", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/ContentDesigner/EkRadEditor.js", "EktronContentDesignerJsCatalogEntry", false);
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/metadata_associations.js", "Ektron_Metadata_Association.js", false);

            if (Request.IsSecureConnection && ((Session["ecmComplianceRequired"] != null) && Convert.ToBoolean(Session["ecmComplianceRequired"]) == true))
            {
                Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/commerce/com.ektron.commerce.session.js", "EktronSessionJs");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Session", "timeoutWarning=setTimeout(showWarning, timeoutPeriod * 60000);", true);
            }

            //Tree Js
            if (entry_edit_data != null)
            {
                Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" + _JsPageFunctions_ContentEditorId + "&entrytype=" + entry_edit_data.EntryType + "&folder_id=" + this.m_iFolder + "&taxonomyRequired=" + catalog_data.CategoryRequired + "&aliasRequired=" + catalog_data.AliasRequired, "Ektron_CatalogEntry_PageFunctions_Js");
            }
            else
            {
                Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" + _JsPageFunctions_ContentEditorId + "&entrytype=" + Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product + "&folder_id=" + this.m_iFolder + "&aliasRequired=" + catalog_data.AliasRequired, "Ektron_CatalogEntry_PageFunctions_Js");
            }
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.A.aspx?folderId=" + _JSTaxonomyFunctions_FolderId + "&taxonomyOverrideId=" + _JSTaxonomyFunctions_TaxonomyOverrideId + "&taxonomyTreeIdList=" + _JSTaxonomyFunctions_TaxonomyTreeIdList + "&taxonomyTreeParentIdList=" + _JSTaxonomyFunctions_TaxonomyTreeParentIdList, "Ektron_CatalogEntry_Taxonomy_A_Js");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.B.aspx?showTaxonomy=" + _JSTaxonomyFunctions_ShowTaxonomy + "&taxonomyFolderId=" + _JSTaxonomyFunctions_TaxonomyFolderId, "Ektron_CatalogEntry_Taxonomy_B_Js");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.cms.types.js", "EktronTreeCmsTypesJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.cms.parser.js", "EktronTreeCmsParserJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.cms.toolkit.js", "EktronTreeCmsToolkitJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.cms.api.js", "EktronTreeCmsApiJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUiContextMenuJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUiIconListJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.ui.tabs.js", "EktronTreeUiTabsJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.ui.explore.js", "EktronTreeUiExploreJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.ui.taxonomytree.js", "EktronTreeUiTaxonomyTreeJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.lang.exception.js", "EktronTreeLanguageExceptionJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs");
            Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.C.js", "EktronCatalogEntryTaxonomyCJs");
        }

        #endregion

    }


}
