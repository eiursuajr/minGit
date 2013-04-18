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

public partial class MenuTree : System.Web.UI.Page
{
    protected EkMessageHelper m_refMsg;
    protected string strActionMessage = "";
    protected ContentAPI m_refContentApi;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string AppName = "";
    protected string AppImgPath = "";
    protected long nId;
    protected long ParentMenuId;
    protected long AncestorMenuId;
    protected long FolderID;
    protected AxMenuData menu;
    protected int EnableMultilingual;
    protected int ContentLanguage;
    protected string m_selectedFolderList = "";

    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        m_refContentApi = new ContentAPI();
        m_refMsg = m_refContentApi.EkMsgRef;
        AppName = m_refContentApi.AppName;
        AppImgPath = m_refContentApi.AppImgPath;
        StyleSheetJS.Value = m_refStyle.GetClientScript();
        nId = Convert.ToInt64(Request.Params["nid"]);
        menu = m_refContentApi.EkContentRef.GetMenuDataByID(nId);
        ParentMenuId = menu.ParentID;
        AncestorMenuId = menu.AncestorID;
        FolderID = menu.FolderID;
        EnableMultilingual = m_refContentApi.EnableMultilingual;
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser))
        {
            Response.Redirect((string)("reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms user")), false);
            return;
        }
        RegisterResources();
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (!string.IsNullOrEmpty(m_refContentApi.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        m_selectedFolderList = GetFolderList(nId);
        MenuToolBar();
    }

    private string GetFolderList(long menuId)
    {
        string result = "";
        AxMenuData tempMenu;
        Stack menuStack = new Stack();

        do
        {
            tempMenu = m_refContentApi.EkContentRef.GetMenuDataByID(menuId);
            if (tempMenu != null)
            {
                menuStack.Push(tempMenu.ID.ToString());
                menuId = tempMenu.ParentID;
            }
        } while (!((tempMenu == null) || (0 == tempMenu.ParentID) || (tempMenu.ID == tempMenu.ParentID)));

        while (menuStack.Count > 0)
        {
            if (result.Length > 0)
            {
                result += ",";
            }
            result += (string)(menuStack.Pop());
        }

        menuStack = null;
        return result;
    }

    private void MenuToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("View Menu Title") + " \"" + menu.Title + "\""));
        result.Append("<table><tr>");

		bool addHelpDivider = false;

        if (menu.EnableReplication)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_translate-nm.gif", (string)("DynReplication.aspx?menuid=" + nId), m_refMsg.GetMessage("alt quickdeploy menu button text"), m_refMsg.GetMessage("alt quickdeploy menu button text"), "", StyleHelper.TranslationButtonCssClass));

			addHelpDivider = true;
		}
        result.Append("<td nowrap=\"true\">&nbsp;</td>");
        result.Append("<td nowrap=\"true\">");
        string addDD;
        addDD = ViewLangsForMenuID(nId, "", false, false, "javascript:addBaseMenu(" + nId + ", " + ParentMenuId + ", " + AncestorMenuId + ", " + FolderID + ", this.value);");
        if (addDD != "")
        {
            addDD = (string)("&nbsp;" + m_refMsg.GetMessage("generic add title") + ":&nbsp;" + addDD);
        }
        if (System.Convert.ToString(EnableMultilingual) == "1")
        {
            result.Append("" + m_refMsg.GetMessage("generic view") + ":&nbsp;" + ViewLangsForMenuID(nId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");

			addHelpDivider = true;
		}
        result.Append("</td>");

		if (addHelpDivider)
		{
			result.Append(StyleHelper.ActionBarDivider);
		}

        result.Append("<td nowrap=\"true\" width=\"95%\" align=\"left\">");
        result.Append(m_refStyle.GetHelpButton("ViewMenu", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    public string ViewLangsForMenuID(long fnMenuID, string fnBGColor, bool showTranslated, bool showAllOpt, string onChangeEv)
    {
        string returnValue;
        Collection TransCol;
        string outDD;

        string frmName;
        object ErrorString = "";

        Ektron.Cms.Content.EkContent contObj;
        contObj = m_refContentApi.EkContentRef;

        if (showTranslated)
        {
            TransCol = contObj.GetTranslatedLangsForMenuID(fnMenuID, ref ErrorString);
            frmName = "frm_translated";
        }
        else
        {
            TransCol = contObj.GetNonTranslatedLangsForMenuID(fnMenuID, ref ErrorString);
            frmName = "frm_nontranslated";
        }

        outDD = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" OnChange=\"" + onChangeEv + "\">" + "\r\n";

        if (showAllOpt)
        {
            if (ContentLanguage.ToString() == "-1")
            {
                outDD = outDD + "<option value=\"-1\" selected>All</option>";
            }
            else
            {
                outDD = outDD + "<option value=\"-1\">All</option>";
            }
        }
        else
        {
            outDD = outDD + "<option value=\"0\">-select language-</option>";
        }

        if ((TransCol.Count > 0) && (EnableMultilingual == 1))
        {
            foreach (Collection Col in TransCol)
            {
                if (ContentLanguage.ToString() == Col["LanguageID"].ToString())
                {
                    outDD = outDD + "<option value=" + Col["LanguageID"] + " selected>" + Col["LanguageName"] + "</option>";
                }
                else
                {
                    outDD = outDD + "<option value=" + Col["LanguageID"] + ">" + Col["LanguageName"] + "</option>";
                }
            }
        }
        else
        {
            returnValue = "";
            return returnValue;
        }

        outDD = outDD + "</select>";

        returnValue = outDD;

        return returnValue;
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/cmsmenuapi.js", "EktronCmsMenuApiJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.JS", "EktronMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/ektron.js", "EktronJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/ektron.workarea.js", "EktronWorkareaJS");

        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "Tree/css/com.ektron.ui.tree.css", "EktronTreeUITreeCSS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/ektron.workarea.css", "EktronWorkareaCSS");

        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.search.js", "EktronTreeExplorerSearchJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.types.js", "EktronTreeCmsTypesJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.parser.js", "EktronTreeCmsParserJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.menutoolkit.js", "EktronTreeCmsMenuToolKitJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.cms.menuapi.js", "EktronTreeCmsMenuAPIJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUIContextMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUIIconListJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.tabs.js", "EktronTreeUITabsJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.search.js", "EktronTreeUISearchJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.explore.js", "EktronTreeUIExploreJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.ui.menutree.js", "EktronTreeUIMenuTreeJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.lang.exception.js", "EktronTreeLangExceptionJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS");
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryStringJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

}