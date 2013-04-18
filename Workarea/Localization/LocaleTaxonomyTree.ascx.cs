//-----------------------------------------------------------------------
// <copyright file="LocaleTaxonomyTree.ascx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Workarea;
using Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

/// <summary>
/// Code behind user control
/// </summary>
partial class Workarea_LocaleTaxonomyTree : WorkareaBaseControl
{
    #region Variables

    private bool taxonomyRoleExists = true;
    private long folderId = -3;
    private long overrideId = 0;
    private string idList = string.Empty;
    private string parentIdList = string.Empty;

    private string taxonomyTreeParamFolderId = "default";
    private string taxonomyTreeParamTaxonomyOverrideId = "default";
    private string taxonomyTreeParamTaxonomyTreeIdList = "default";
    private string taxonomyTreeParamTaxonomyTreeParentIdList = "default";
    private string taxonomyTreeParamShowTaxonomy = "default";
    private string taxonomyTreeParamTaxonomyFolderId = "default";

    #endregion

    /// <summary>
    /// Controls what other nodes are checked (and disabled) when a node is checked.
    /// </summary>
    public enum TreeNodeImpliedInheritance
    {
        /// <summary>
        /// No other node is checked. Each node may be checked or unchecked without affecting the others.
        /// </summary>
        None,

        /// <summary>
        /// When a node is checked, all of its ancestors (i.e., all parents) are disabled and checked.
        /// Applies to taxonomy where an item belongs to a taxonomy category, and therefore it belongs to all of its more generic categories.
        /// </summary>
        Ancestors,

        /// <summary>
        /// When a node is checked, all of its descendants (i.e., recursive children) are disabled and checked.
        /// Applies when selecting a category should select all of its more specific subcategories.
        /// </summary>
        Descendants
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value that controls what other nodes are checked (and disabled) when a node is checked.
    /// </summary>
    public TreeNodeImpliedInheritance ImpliedInheritance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether more than one taxonomy node may be selected. That is, whether check boxes appear or not.
    /// </summary>
    public bool AllowSelectMultiple { get; set; }

    /// <summary>
    /// Gets the array of selected locale taxonomy ids.
    /// </summary>
    public long[] SelectedIds
    {
        get
        {
            long[] locTaxIds = new long[0];
            try
            {
                string strIds = taxonomyselectedtree.Value;
                string[] ids = strIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                locTaxIds = Array.ConvertAll<string, long>(ids, s => Convert.ToInt64(s));
            }
            catch (Exception)
            {
                // ignore error
            }

            return locTaxIds;
        }
        set
        {
            taxonomyselectedtree.Value = string.Join(",", value.ToList<long>().ConvertAll<string>(
                new Converter<long, string>(delegate(long l)
            {
                return l.ToString();
            }
            )).ToArray());
        }
    }

    private string ApplicationPath { get; set; }

    private string SitePath { get; set; }

    #endregion

    #region Page Functions

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        this.SitePath = this.CommonApi.SitePath.TrimEnd('/');
        this.ApplicationPath = this.CommonApi.ApplicationPath.TrimEnd('/');
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        try
        {
            // hide the check box value in a hidden variable.
            ShowCheckBox.Value = this.AllowSelectMultiple.ToString();
            hdnImpliedInheritance.Value = this.ImpliedInheritance.ToString();
            //if (!Page.IsPostBack)
            //{
                this.Display_TaxonomyTab();
          //  }

            this.Util_SetJS();
            this.RegisterJs();
            this.RegisterCss();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Display - Tabs

    private void Display_TaxonomyTab()
    {
        PermissionData permissions = this.ContentApi.LoadPermissions(this.folderId, "folder", ContentAPI.PermissionResultType.Common);
        if (permissions.CanEdit || permissions.CanAdd || this.ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXliff, this.ContentApi.RequestInformationRef.UserId, false))
        {
            this.taxonomyRoleExists = true;
        }

        EditTaxonomyHtml.Text = "<div id=\"TreeOutput\"></div>";

        TaxonomyBaseData[] taxonomy_data_arr = null;
        Ektron.Cms.TaxonomyRequest taxRequest = new Ektron.Cms.TaxonomyRequest();

        taxRequest.TaxonomyId = 0;
        taxRequest.TaxonomyLanguage = this.ContentApi.ContentLanguage;
        taxRequest.TaxonomyType = EkEnumeration.TaxonomyType.Locale;
        taxonomy_data_arr = this.ContentApi.EkContentRef.ReadAllSubCategories(taxRequest);

        this.taxonomyTreeParamTaxonomyTreeIdList = EkFunctions.UrlEncode(this.idList);
        this.taxonomyTreeParamTaxonomyTreeParentIdList = EkFunctions.UrlEncode(this.parentIdList);
        this.taxonomyTreeParamTaxonomyOverrideId = this.overrideId.ToString();
        this.taxonomyTreeParamTaxonomyFolderId = this.folderId.ToString();
    }

    #endregion

    #region Util

    private void Util_SetJS()
    {
        this.taxonomyTreeParamShowTaxonomy = this.taxonomyRoleExists.ToString();
        this.taxonomyTreeParamFolderId = this.folderId.ToString();
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
        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss");

        Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/commerce/Ektron.Commerce.Session.css", "EktronCommerceSessionCss");
    }

    private void RegisterJs()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);

        // Tree Js        
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Localization/js/Taxonomy.Tree.A.aspx?folderId=" + this.taxonomyTreeParamFolderId + "&taxonomyOverrideId=" + this.taxonomyTreeParamTaxonomyOverrideId + "&taxonomyTreeIdList=" + this.taxonomyTreeParamTaxonomyTreeIdList + "&taxonomyTreeParentIdList=" + this.taxonomyTreeParamTaxonomyTreeParentIdList, "L10nTaxonomyTreeAJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Localization/js/Taxonomy.Tree.B.aspx?showTaxonomy=" + this.taxonomyTreeParamShowTaxonomy + "&taxonomyFolderId=" + this.taxonomyTreeParamTaxonomyFolderId, "L10nTaxonomyTreeBJs");
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
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.lang.exception.js", "EktronTreeLanguageExceptionJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Localization/js/com.ektron.ui.localeTaxonomytree.js", "EktronL10nTreeUiLocaleTaxonomyTreeJs");
        Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Localization/js/Taxonomy.Tree.C.js", "EktronL10nTaxonomyTreeCJs");
    }

    #endregion
}
