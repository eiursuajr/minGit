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
using System.IO;

public partial class taxonomytree : System.Web.UI.UserControl
{
    protected CommonApi m_refCommon = new CommonApi();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = "";
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected long TaxonomyId = 0;
    protected int TaxonomyLanguage = -1;
    protected LanguageData language_data;
    protected TaxonomyRequest taxonomy_request;
    protected TaxonomyData taxonomy_data;
    protected long TaxonomyParentId = 0;
    protected long AncestorTaxonomyId = 0;
    protected string m_selectedTaxonomyList = "";
    protected string m_strTaxonomyName = "";
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refCommon.EkMsgRef;
        AppImgPath = m_refCommon.AppImgPath;
        m_strPageAction = Request.QueryString["action"];
        Utilities.SetLanguage(m_refCommon);
        TaxonomyLanguage = m_refCommon.ContentLanguage;
        TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
        taxonomy_request = new TaxonomyRequest();
        taxonomy_request.TaxonomyId = TaxonomyId;
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
        m_refContent = m_refCommon.EkContentRef;
        Util_RegisterResources();
        Util_SetServerJSVariables();

        litEnable.Text = m_refMsg.GetMessage("js:Confirm enable taxonomy all languages");
        litDisable.Text = m_refMsg.GetMessage("js:Confirm disable taxonomy all languages");
        if (Page.IsPostBack)
        {
            if (Request.Form["submittedaction"] == "delete")
            {
                m_refContent.DeleteTaxonomy(taxonomy_request);
                Response.Redirect((string)("taxonomy.aspx?LangType=" + TaxonomyLanguage), true);
            }
            else if (Request.Form["submittedaction"] == "deletenode")
            {
                long CurrentDeleteId = TaxonomyId;
                if ((Request.Form["LastClickedOn"] != null) && Request.Form["LastClickedOn"] != "")
                {
                    CurrentDeleteId = Convert.ToInt64(Request.Form["LastClickedOn"]);
                }
                taxonomy_request.TaxonomyId = CurrentDeleteId;
                m_refContent.DeleteTaxonomy(taxonomy_request);
                if (CurrentDeleteId == TaxonomyId)
                {
                    Response.Redirect((string)("taxonomy.aspx?taxonomyid=" + TaxonomyId), true);
                }
                else
                {
                    Response.Redirect((string)("taxonomy.aspx?action=viewtree&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage), true);
                }
            }
            else if (Request.Form["submittedaction"] == "enable")
            {
                long CurrentEnableId = TaxonomyId;
                if ((Request.Form["LastClickedOn"] != null) && Request.Form["LastClickedOn"] != "")
                {
                    CurrentEnableId = Convert.ToInt64(Request.Form["LastClickedOn"]);
                }
                if (Request.Form[alllanguages.UniqueID] == "true")
                {
                    m_refContent.UpdateTaxonomyVisible(CurrentEnableId, -1, true);
                }
                else
                {
                    m_refContent.UpdateTaxonomyVisible(CurrentEnableId, TaxonomyLanguage, true);
                }
                Response.Redirect((string)("taxonomy.aspx?action=viewtree&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage), true);
            }
            else if (Request.Form["submittedaction"] == "disable")
            {
                long CurrentDisableId = TaxonomyId;
                if ((Request.Form["LastClickedOn"] != null) && Request.Form["LastClickedOn"] != "")
                {
                    CurrentDisableId = Convert.ToInt64(Request.Form["LastClickedOn"]);
                }
                if (Request.Form[alllanguages.UniqueID] == "true")
                {
                    m_refContent.UpdateTaxonomyVisible(CurrentDisableId, -1, false);
                }
                else
                {
                    m_refContent.UpdateTaxonomyVisible(CurrentDisableId, TaxonomyLanguage, false);
                }
                Response.Redirect((string)("taxonomy.aspx?action=viewtree&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage), true);
            }
        }
        else
        {
            taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
            if (taxonomy_data != null)
            {
                TaxonomyParentId = taxonomy_data.TaxonomyParentId;
                m_strTaxonomyName = taxonomy_data.TaxonomyName;
            }
            AncestorTaxonomyId = TaxonomyId;
            m_selectedTaxonomyList = Convert.ToString(TaxonomyId);
            TaxonomyToolBar();
        }
    }

    private void TaxonomyToolBar()
    {
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("view all categories of taxonomy") + " \"" + m_strTaxonomyName + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "\' />"));
        
		System.Text.StringBuilder result = new System.Text.StringBuilder();
        
		result.Append("<table><tr>" + "\r\n");

		result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "taxonomy.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", (string)("taxonomy.aspx?backaction=viewtree&action=edit&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage), m_refMsg.GetMessage("alt edit button text (taxonomy)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (taxonomy)"), m_refMsg.GetMessage("btn delete"), "Onclick=\"javascript:return DeleteNode();\"", StyleHelper.DeleteButtonCssClass));
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "btn_exptaxo-nm.gif", "javascript:window.open(\'taxonomy_imp_exp.aspx?action=export&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage + "\',\'exptaxonomy\',\'status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=1,height=100px,width=200px\');javascript:void(0);", m_refMsg.GetMessage("alt export taxonomy"), m_refMsg.GetMessage("btn export taxonomy"), "", StyleHelper.ExportTaxonomyButtonCssClass));
        
		result.Append("<td nowrap=\"true\">");
        
		string addDD;
        addDD = GetLanguageForTaxonomy(TaxonomyId, "", false, false, "javascript:TranslateTaxonomy(" + TaxonomyId + ", " + TaxonomyParentId + ", this.value);");
        if (addDD != "")
        {
            addDD = (string)("&nbsp;" + m_refMsg.GetMessage("add title") + ":&nbsp;" + addDD);
        }
        if (m_refCommon.EnableMultilingual == 1)
        {
            result.Append("View In:&nbsp;" + GetLanguageForTaxonomy(TaxonomyId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
        }
        result.Append("</td>");
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refstyle.GetHelpButton("ViewTaxonomyTree", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    private string GetLanguageForTaxonomy(long TaxonomyId, string BGColor, bool ShowTranslated, bool ShowAllOpt, string onChangeEv)
    {
        string result = "";
        string frmName = "";
        IList<LanguageData> result_language = null;
        TaxonomyLanguageRequest taxonomy_language_request = new TaxonomyLanguageRequest();
        taxonomy_language_request.TaxonomyId = TaxonomyId;

        if (ShowTranslated)
        {
            taxonomy_language_request.IsTranslated = true;
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_translated";
        }
        else
        {
            taxonomy_language_request.IsTranslated = false;
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_nontranslated";
        }

        result = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" OnChange=\"" + onChangeEv + "\">" + "\r\n";

        if (ShowAllOpt)
        {
            if (TaxonomyLanguage == -1)
            {
                result = result + "<option value=\"-1\" selected>All</option>";
            }
            else
            {
                result = result + "<option value=\"-1\">All</option>";
            }
        }
        else
        {
            if (ShowTranslated == false)
            {
                result = result + "<option value=\"0\">-select language-</option>";
            }
        }
        if ((result_language != null) && (result_language.Count > 0) && (m_refCommon.EnableMultilingual == 1))
        {
            foreach (LanguageData language in result_language)
            {
                if (TaxonomyLanguage == language.Id)
                {
                    result = result + "<option value=" + language.Id + " selected>" + language.Name + "</option>";
                }
                else
                {
                    result = result + "<option value=" + language.Id + ">" + language.Name + "</option>";
                }
            }
        }
        else
        {
            result = "";
        }
        if (result.Length > 0)
        {
            result = result + "</select>";
        }
        return (result);
    }

    private string ApprovalText(bool flag)
    {
        string result = "";
        if (flag)
        {
            result = "All Items in this folder required approval.";
        }
        else
        {
            result = "All Items in this folder does not required approval.";
        }
        return result;
    }
    private void Util_SetServerJSVariables()
    {
        ltr_confrmDelTax.Text = m_refMsg.GetMessage("alt Are you sure you want to delete this category?");
        ltr_alrtDelTax.Text = m_refMsg.GetMessage("alt Are you sure you want to delete this taxonomy?");
    }
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refCommon.ApplicationPath + "Tree/css/com.ektron.ui.tree.css", "EktronTreeUITreeCSS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.cms.types.js", "EktronTreeCMSTypesJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.cms.parser.js", "EktronTreeCMSParserJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.cms.toolkit.js", "EktronTreeCMSToolkitJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.cms.api.js", "EktronTreeCMSApiJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUIContextMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUIIconListJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.ui.explore.js", "EktronTreeUIExploreJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.ui.taxonomytree.js", "EktronTreeUITaxonomyTreeJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.lang.exception.js", "EktronTreeLangExceptionJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDOMJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refCommon.ApplicationPath + "Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryStringJS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refCommon.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS");
    }
}