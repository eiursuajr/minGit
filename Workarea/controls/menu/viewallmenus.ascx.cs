using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;

public partial class Workarea_controls_menu_viewallmenus : System.Web.UI.UserControl
{
    protected long nId = 0;
    protected long folderId = 0;
    protected string orderBy = "title";
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected CommonApi m_refApi = new CommonApi();
    ApplicationAPI AppUI = new ApplicationAPI();
    Int32 ContentLanguage = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        
        Collection gtNavs = new Collection();
        if (Request.QueryString["LangType"] != null)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                if (ContentLanguage == 0)
                    ContentLanguage = AppUI.DefaultContentLanguage;
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(m_refContentApi.GetCookieValue("LastValidLanguageID")))
                {
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(m_refContentApi.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                if (ContentLanguage == 0)
                {
                    ContentLanguage = AppUI.DefaultContentLanguage;
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
            }
        }
        AppUI.FilterByLanguage = ContentLanguage;

        nId = Convert.ToInt64(Request.QueryString["nid"]);
	    folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        if (Request.QueryString["OrderBy"] != null && Request.QueryString["OrderBy"].Length > 0)
            orderBy = Request.QueryString["OrderBy"].ToString();
        gtNavs = AppUI.EkContentRef.GetAllMenusInfo(folderId, orderBy);
        
        litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view all menu title"));
		litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "content.aspx?Action=ViewContentByCategory&id=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        litButtons.Text =  m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/add.png", "collections.aspx?action=AddMenu&folderid=" + folderId + "&LangType=" + ContentLanguage, MsgHelper.GetMessage("alt add new menu"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true);
		litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("ViewAllMenus", "") + "</td>";
        litGenericTitle.Text = MsgHelper.GetMessage("generic Title");
        litGenericId.Text = MsgHelper.GetMessage("generic ID");
        litLangId.Text = MsgHelper.GetMessage("lbl Language ID");
        litDateMod.Text = MsgHelper.GetMessage("generic Date Modified");
        string idContainer = "";
		if (gtNavs.Count > 0)
        {
	        foreach (Collection gtNav in gtNavs)
		        idContainer = idContainer + gtNav["MenuID"] + ",";
            idContainer = idContainer.Remove(0, 1);
		}
        if ((idContainer != "") && (m_refApi.EnableMultilingual == 1))
        {
            litViewLang.Text = StyleHelper.ActionBarDivider + "<td nowrap=\"nowrap\"> View In:&nbsp;" + ViewLangsForMenuID(idContainer, "", true, true, "javascript:LoadLanguage(this.value);") + "&nbsp;<br /></td>";
        }
        else
        {
            if (m_refApi.EnableMultilingual == 1)
            {
                litViewLang.Text = StyleHelper.ActionBarDivider + "<td nowrap=\"nowrap\">View In:&nbsp";
	            Hashtable colActiveLanguages = AppUI.EkSiteRef.GetAllActiveLanguages();
	            litViewLang.Text += "<select id=selLang name=selLang onchange=\"LoadLanguageMenus('frmViewMenus');\">";
	            if (ContentLanguage == -1)
		            litViewLang.Text += "<option value=" + ALL_CONTENT_LANGUAGES + " selected>" + MsgHelper.GetMessage("lbl abbreviation for all the words") + "</option>";
	            else
		            litViewLang.Text += "<option value=" + ALL_CONTENT_LANGUAGES + ">" + MsgHelper.GetMessage("lbl abbreviation for all the words") + "</option>";

                foreach (DictionaryEntry Language in colActiveLanguages)
                {
                    Hashtable langData = Language.Value as Hashtable;
                    if (langData != null)
                    {
                        if (ContentLanguage.ToString() == langData["ID"].ToString())
                            litViewLang.Text += "<option value=\"" + langData["ID"] + "\" selected>" + langData["Name"] + "</option>";
                        else
                            litViewLang.Text += "<option value=\"" + langData["ID"] + "\" >" + langData["Name"] + "</option>";
                    }
	            }
	            litViewLang.Text += "</select>";
                litViewLang.Text += "</td>";
            }
        }


        if (gtNavs.Count > 0)
        {
            DataTable dtItems = new DataTable();
            dtItems.Columns.Add("MenuTitle");
            dtItems.Columns.Add("MenuID");
            dtItems.Columns.Add("ContentLanguage");
            dtItems.Columns.Add("DisplayLastEditDate");
            dtItems.Columns.Add("FolderId");
            foreach (Collection gtNav in gtNavs)
            {
                DataRow dRow = dtItems.NewRow();
                dRow["MenuTitle"] = gtNav["MenuTitle"].ToString();
                dRow["MenuID"] = gtNav["MenuID"].ToString();
                dRow["ContentLanguage"] = gtNav["ContentLanguage"].ToString();
                dRow["DisplayLastEditDate"] = gtNav["DisplayLastEditDate"].ToString();
                dRow["FolderId"] = folderId;
                dtItems.Rows.Add(dRow);
            }
            rptMenus.DataSource = dtItems;
            rptMenus.DataBind();
        }

    }
    public string ViewLangsForMenuID(object fnMenuID, string fnBGColor, object showTranslated, object showAllOpt, string onChangeEv)
    {
        string returnValue;
        Collection TransCol;
        string outDD;
        object frmName;
        object objErrorString = "";
        if (System.Convert.ToBoolean(showTranslated))
        {
            TransCol = AppUI.EkContentRef.GetTranslatedLangsForMenuID(fnMenuID, ref objErrorString);
            frmName = "frm_translated";
        }
        else
        {
            TransCol = AppUI.EkContentRef.GetNonTranslatedLangsForMenuID(fnMenuID, ref objErrorString);
            frmName = "frm_nontranslated";
        }

        outDD = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" OnChange=\"" + onChangeEv + "\">" + "\r\n";

        if (System.Convert.ToBoolean(showAllOpt))
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

        if ((TransCol.Count > 0) && (m_refApi.EnableMultilingual == 1))
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
}
