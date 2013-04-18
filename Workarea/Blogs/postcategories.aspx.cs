using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Caching;
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
using Ektron.Editors.JavascriptEditorControls;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Workarea;

public partial class blog_postcategories : Ektron.Cms.Workarea.workareabase
{
    #region variables

    private long blogId = 0;
    private long postId = 0;
    private string uniqueId = "";
    private string cssFilesPath = "";

    protected string TaxonomyTreeIdList = "";
    protected string TaxonomyTreeParentIdList = "";
    protected bool TaxonomyRoleExists = false;
    protected long m_intTaxFolderId = 0;
    protected long TaxonomyOverrideId = 0;
    protected List<long> TaxonomySelectIdList = new List<long>();
    //js: page function vars
    private string _JsPageFunctions_ContentEditorId = "default";
    //js: taxonomy function vars
    private string _JSTaxonomyFunctions_FolderId = "default";
    private string _JSTaxonomyFunctions_TaxonomyOverrideId = "default";
    private string _JSTaxonomyFunctions_TaxonomyTreeIdList = "default";
    private string _JSTaxonomyFunctions_TaxonomyTreeParentIdList = "default";
    private string _JSTaxonomyFunctions_ShowTaxonomy = "default";
    private string _JSTaxonomyFunctions_TaxonomyFolderId = "default";

    #endregion
    #region page functions

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        
        Util_ObtainValues();

        if (Page.IsPostBack)
        {
            Process_Selection();
        }
        else
        {
            Display_Taxonomy();
            Display_Buttons();
            Util_RegisterResources();
        }
        
    }

    #endregion
    #region display

    private void Display_Taxonomy()
    {

        EditTaxonomyHtml.Text = "<p class=\"info\">" + this.m_refMsg.GetMessage("lbl select categories entry") + "</p><div id=\"TreeOutput\"></div>";
        lit_add_string.Text = m_refMsg.GetMessage("generic add title");

        TaxonomyBaseData[] taxonomy_cat_arr = null;
        m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage;
        m_refContentApi.ContentLanguage = ContentLanguage;

        TaxonomyRequest taxonomy_request = new TaxonomyRequest();
        TaxonomyBaseData[] taxonomy_data_arr = null;

        {
            List<long> recursiveTaxonomyList = new List<long>();
            foreach (long taxonomyId in this.TaxonomySelectIdList)
            {
                taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(taxonomyId, m_refContentApi.ContentLanguage, 0);
                if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                        if (!this.TaxonomySelectIdList.Contains(taxonomy_cat.Id))
                            recursiveTaxonomyList.Add(taxonomy_cat.Id);
            }
            this.TaxonomySelectIdList.AddRange(recursiveTaxonomyList);
            taxonomyselectedtree.Value = Util_GetCommaSeperatedList(this.TaxonomySelectIdList);
            TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
            if (TaxonomyTreeIdList.Trim().Length > 0)
            {
                TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(m_iID);
            }
        }
        taxonomy_request.TaxonomyId = blogId;
        taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(blogId);

        m_intTaxFolderId = blogId;
        
        //set CatalogEntry_Taxonomy_A_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.A.aspx under CatalogEntry/js
        this._JSTaxonomyFunctions_TaxonomyTreeIdList = EkFunctions.UrlEncode(TaxonomyTreeIdList);
        this._JSTaxonomyFunctions_TaxonomyTreeParentIdList = EkFunctions.UrlEncode(TaxonomyTreeParentIdList);
        this._JSTaxonomyFunctions_TaxonomyOverrideId = TaxonomyOverrideId.ToString();
        this._JSTaxonomyFunctions_TaxonomyFolderId = blogId.ToString();

    }
    private void Display_Buttons()
    {
        SetTitleBarToMessage("select categories blogpost");
        AddButtonwithMessages(AppImgPath + "../UI/Icons/cancel.png", "#", "btn cancel", "btn cancel", "Onclick=\"Close();return false;\"", StyleHelper.CancelButtonCssClass, true);
        AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"document.forms[0].submit();return false;\"", StyleHelper.SaveButtonCssClass, true);
    }

    #endregion
    #region process

    private void Process_Selection()
    {
        List<long> taxonomyTreeIdList = Util_GetLongList(Request.Form[taxonomyselectedtree.UniqueID]);
        string taxonomyTreeIds = Util_GetCommaSeperatedList(taxonomyTreeIdList);
        List<string> taxonomyTreeNames = Util_GetTaxonomyTreeNames(taxonomyTreeIdList);
        taxonomyTreeNames.Sort();
        string saveFunction = string.Format("Save('{0}','{1}', [{2}]);",
            uniqueId,
            taxonomyTreeIds,
            string.Join(",", taxonomyTreeNames.ConvertAll<string>(x => "'" + x + "'").ToArray())
            );
        Ektron.Cms.API.JS.RegisterJSBlock(this, saveFunction, "SaveSelectionJS");
    }

    #endregion
    #region util

    private List<string> Util_GetTaxonomyTreeNames(List<long> IdList)
    {
        List<TaxonomyData> taxonomyList = new List<TaxonomyData>();
        Ektron.Cms.Organization.TaxonomyCriteria criteria = new Ektron.Cms.Organization.TaxonomyCriteria();
        if (IdList.Count > 0)
        {
            criteria.AddFilter(Ektron.Cms.Organization.TaxonomyProperty.Id, CriteriaFilterOperator.In, IdList);
            taxonomyList = Ektron.Cms.ObjectFactory.GetTaxonomyManager().GetList(criteria);
        }
        return taxonomyList.ConvertAll<string>(x => x.Path.Remove(0, 1).Replace("\\", " > "));
    }
    private string Util_GetCommaSeperatedList(List<long> IdList)
    {
        return string.Join(",", IdList.ConvertAll<string>(x => x.ToString()).ToArray());
    }
    private List<long> Util_GetLongList(string values)
    {
        List<long> idList = new List<long>();
        if (!string.IsNullOrEmpty(values))
        {
            string[] taxonomyTreeIdArray = values.Split(new char[] { ',' });
            if (taxonomyTreeIdArray != null)
            {
                idList = new List<string>(taxonomyTreeIdArray)
                    .FindAll(x => EkFunctions.ReadLongValue(x, 0) > 0)
                    .ConvertAll<long>(x => Convert.ToInt64(x))
                    ;
            }
        }
        return idList;
    }
    private void Util_ObtainValues()
    {
        if (Request.QueryString["blogid"] != null)
            blogId = EkFunctions.ReadLongValue(Request.QueryString["blogid"], 0);

        if (Request.QueryString["postId"] != null)
            postId = EkFunctions.ReadLongValue(Request.QueryString["postId"], 0);

        if (Request.QueryString["uniqueId"] != null)
            uniqueId = Microsoft.Security.Application.AntiXss.GetSafeHtmlFragment(Request.QueryString["uniqueId"]);

        if (Request.QueryString["SelTaxonomyId"] != null)
            TaxonomySelectIdList = Util_GetLongList(Request.QueryString["SelTaxonomyId"]);
    }
    private void Util_RegisterResources()
    {
        ltrNoCatSelected.Text = GetMessage("lbl nocatselected");
        Util_RegisterCss();
        Util_RegisterJS();
    }
    private void Util_RegisterCss()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss");
    }
    private void Util_RegisterJS()
    {
        //set CatalogEntry_Taxonomy_B_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.B.aspx under CatalogEntry/js
        this._JSTaxonomyFunctions_ShowTaxonomy = true.ToString();
        this._JSTaxonomyFunctions_FolderId = blogId.ToString();

        //Tree Js
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" + _JsPageFunctions_ContentEditorId + "&entrytype=" + Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product + "&folder_id=" + this.blogId + "&aliasRequired=false", "Ektron_CatalogEntry_PageFunctions_Js");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.A.aspx?folderId=" + _JSTaxonomyFunctions_FolderId + "&taxonomyOverrideId=" + _JSTaxonomyFunctions_TaxonomyOverrideId + "&taxonomyTreeIdList=" + _JSTaxonomyFunctions_TaxonomyTreeIdList + "&taxonomyTreeParentIdList=" + _JSTaxonomyFunctions_TaxonomyTreeParentIdList, "Ektron_CatalogEntry_Taxonomy_A_Js");

        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.B.aspx?suppress_menu=true&showTaxonomy=" + _JSTaxonomyFunctions_ShowTaxonomy + "&taxonomyFolderId=" + _JSTaxonomyFunctions_TaxonomyFolderId, "Ektron_CatalogEntry_Taxonomy_B_Js");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.cms.types.js", "EktronTreeCmsTypesJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.cms.parser.js", "EktronTreeCmsParserJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.cms.toolkit.js", "EktronTreeCmsToolkitJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.cms.api.js", "EktronTreeCmsApiJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUiContextMenuJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUiIconListJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.ui.tabs.js", "EktronTreeUiTabsJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.ui.explore.js", "EktronTreeUiExploreJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.ui.taxonomytree.js", "EktronTreeUiTaxonomyTreeJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.lang.exception.js", "EktronTreeLanguageExceptionJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.C.js", "EktronCatalogEntryTaxonomyCJs");
    }

    #endregion
}


