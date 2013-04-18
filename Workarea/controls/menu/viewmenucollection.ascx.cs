using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Microsoft.VisualBasic;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Common;

public partial class Workarea_controls_menu_viewmenucollection : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    protected ContentAPI m_refContentApi = new ContentAPI();
    Int32 ContentLanguage = 0;
    protected long nId = 0;
    protected long folderId = 0;
    ApplicationAPI AppUI = new ApplicationAPI();
    CommonApi m_refApi = new CommonApi();
    protected long AncestorMenuId = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        EkMessageHelper MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        StyleHelper m_refStyle = new StyleHelper();
        Collection gtNavs = new Collection();
       
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (ContentLanguage == ALL_CONTENT_LANGUAGES || ContentLanguage == 0)
            {
                ContentLanguage = AppUI.DefaultContentLanguage;
            }
	        
	        AppUI.FilterByLanguage=ContentLanguage;
        }

        nId = Convert.ToInt64(Request.QueryString["nid"]);
	    folderId = Convert.ToInt64(Request.QueryString["folderid"]);
	    //if (m_refApi.TreeModel == 1)
		//    Server.Transfer("menutree.aspx?nid=" + nId + "+folderid=" + folderId + "+LangType=" + ContentLanguage);
	    //else
        //{
		    String MenuTitle  = String.Empty;
		    long ParentMenuId = 0;
		    string MenuXML = String.Empty;
            AppUI.ContentLanguage = ContentLanguage;
		    gtNavs = AppUI.EkContentRef.GetMenuByID(nId, 0, false);
		    if (gtNavs.Count > 0)
            {
				MenuXML = p_MenuXML(gtNavs, nId);
                 if (MenuXML != "")
                 {
					MenuXML = "<navigation>" +  MenuXML + "</navigation>";
					MenuXML = AppUI.TransformXSLT(MenuXML, Server.MapPath(AppUI.AppPath + "cmsmenuapi.xsl"));
                    litMenuXML.Text = MenuXML;
                }
				MenuTitle = gtNavs["MenuTitle"].ToString();
				folderId = Convert.ToInt64(gtNavs["FolderID"]);
				ParentMenuId=Convert.ToInt64(gtNavs["MenuID"]);
				AncestorMenuId=Convert.ToInt64(gtNavs["AncestorMenuId"]);
				if (AncestorMenuId == 0)
					AncestorMenuId=ParentMenuId;
			}
            litTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("View Menu Title") + " \"" + MenuTitle + "\"");
			litHelp.Text = StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton("ViewMenu", "") + "</td>";

            string Callbackpage = "";
        
    		// Toolbar
			if (Request.QueryString["bpage"] == "reports")
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			else if (Convert.ToInt64(gtNavs["ParentMenuId"]) > 0)
				litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" + gtNavs["ParentMenuId"] + "&folderid=" + Request.QueryString["pfid"], MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			else
			{
				if ((Request.QueryString["bPage"] == "ViewMenuReport") || (Request.QueryString["bPage"] == "reports"))
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
				else
					litButtons.Text = m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "collections.aspx?action=ViewAllMenus&folderid=" + folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			}

			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/add.png", "collections.aspx?LangType=" + ContentLanguage + "&action=AddMenuItem&nid=" + nId + "&folderid=" + folderId + "&parentid=" + ParentMenuId + "&ancestorid=" + AncestorMenuId, MsgHelper.GetMessage("alt add new item"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true);
            
			Collection gtN = (Collection)gtNavs["Items"];
            if (gtN.Count > 1)
            {
				litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/remove.png", "collections.aspx?action=DeleteMenuItem&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt remove item"), MsgHelper.GetMessage("btn minus"), "", StyleHelper.RemoveButtonCssClass);
				litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/arrowUpDown.png", "collections.aspx?action=ReOrderMenuItems&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt reorder items"), MsgHelper.GetMessage("btn reorder"), "", StyleHelper.ReOrderButtonCssClass);
            }
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/contentEdit.png", "collections.aspx?action=EditMenu&nid=" + nId + "&folderid=" + folderId, MsgHelper.GetMessage("alt edit menu"), MsgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass);
            if (Convert.ToInt64(gtNavs["ParentMenuId"]) > 0)
				Callbackpage = "&callbackpage=collections.aspx&parm1=action&value1=ViewMenu&parm2=nid&value2=" + gtNavs["ParentMenuId"] + "&parm3=folderid&value3=" + Request.QueryString["pfid"];
		    else if ((Request.QueryString["bpage"] == "reports") || ((Request.QueryString["bpage"] == "ViewMenuReport")))
				Callbackpage = "&callbackpage=collections.aspx&parm1=action&value1=ViewMenuReport";
			litButtons.Text += m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/delete.png", "collections.aspx?action=doDeleteMenu&nId=" + nId + "&folderid=" + folderId + Callbackpage, MsgHelper.GetMessage("alt delete menu"), MsgHelper.GetMessage("btn delete"), "onclick=\" return ConfirmMenuDelete();\"", StyleHelper.DeleteButtonCssClass);
           
			if (gtNavs.Contains("IsSubMenu"))
            {
				string addDD = ViewLangsForMenuID(nId, "", false, false, "javascript:addBaseMenu(" + nId + ", " + ParentMenuId + ", " + AncestorMenuId + ", " + folderId + ", this.value);");
                if (addDD != "")
					addDD += "&nbsp;" + MsgHelper.GetMessage("add title") + ":&nbsp;" + addDD;
                if (m_refApi.EnableMultilingual == 1)
                   addDD += "View In: &nbsp;";
                litSubMenu.Text = StyleHelper.ActionBarDivider + ViewLangsForMenuID(nId, "", true, false, "javascript:LoadLanguage(this.value);") + addDD;
            }
        //}
        litGenericTitleLabel.Text = MsgHelper.GetMessage("generic title label");
        litGenericTitle.Text = gtNavs["MenuTitle"].ToString();
        litIDLabel.Text = MsgHelper.GetMessage("id label");
        litID.Text = gtNavs["MenuID"].ToString();
        litPathLabel.Text = MsgHelper.GetMessage("generic Path");
        litPath.Text = gtNavs["Path"].ToString();
        litLUELabel.Text = MsgHelper.GetMessage("content LUE label");
        litLUE.Text = gtNavs["EditorFName"] + " " + gtNavs["EditorLName"];
        litLEDLabel.Text = MsgHelper.GetMessage("content LED label");
        litLED.Text = gtNavs["DisplayLastEditDate"].ToString();
        litDCLabel.Text = MsgHelper.GetMessage("content DC label");
        litDC.Text = gtNavs["DisplayDateCreated"].ToString();
        litDescLabel.Text = MsgHelper.GetMessage("description label");
        litDesc.Text = gtNavs["MenuDescription"].ToString();
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
    private string p_MenuXML(Collection inCol, long BaseID)
    {
        string MenuXML = string.Empty;
        if (inCol["Items"].ToString().Length > 0)
        {
            foreach (Collection SingleItem in (Collection)inCol["Items"])
            {
                switch (Convert.ToInt32(SingleItem["ItemType"]))
                {
                    case 4:
                        MenuXML = MenuXML + p_SubMenu((Collection)SingleItem["Menu"]);
                        break;
                    case 1:
                        MenuXML = MenuXML + "<item type=\"content\" id=\"" + SingleItem["ItemID"].ToString() + "\" itemid=\"" + SingleItem["ID"].ToString() + "\" title=\"" + SingleItem["ItemTitle"].ToString() + "\">";
                        MenuXML = MenuXML + "</item>	";
                        break;
                    case 5:
                        MenuXML = MenuXML + "<item type=\"link\" id=\"" + SingleItem["ItemID"].ToString() + "\" itemid=\"" + SingleItem["ID"].ToString() + "\" title=\"" + SingleItem["ItemTitle"].ToString() + "\">";
                        MenuXML = MenuXML + "</item>";
                        break;
                    case 2:
                        MenuXML = MenuXML + "<item type=\"library\" id=\"" + SingleItem["ItemID"].ToString() + "\" itemid=\"" + SingleItem["ID"].ToString() + "\" title=\"" + SingleItem["ItemTitle"].ToString() + "\">";
                        MenuXML = MenuXML + "</item>";
                        break;
                }  
            }
        }
        return MenuXML;
    }
    private string p_SubMenu(Collection SubMenu)
    {
        Collection Items = new Collection();
        string MenuXML = "";
        Items = (Collection)SubMenu["Items"];
        if (Items.Count > 0)
        {
            foreach (Collection SingleItem in Items)
            {
                switch (Convert.ToInt32(SingleItem["ItemType"]))
                {
                    case 4:
                        MenuXML = MenuXML + p_SubMenu((Collection)SingleItem["Menu"]);
                        break;
                    case 1:
                        if (Convert.ToInt32(SingleItem["ContentType"]) != 2)
                            MenuXML = MenuXML + "<item type=\"content\" id=\"" + SingleItem["ItemID"] + "\" itemid=\"" + SingleItem["ID"] + "\" title=\"" + SingleItem["ItemTitle"] + "\">";
                        else
                            MenuXML = MenuXML + "<item type=\"form\" id=\"" + SingleItem["ItemID"] + "\" itemid=\"" + SingleItem["ID"] + "\" title=\"" + SingleItem["ItemTitle"] + "\">";
                        MenuXML = MenuXML + "</item>";
                        break;
                    case 5:
                        MenuXML = MenuXML + "<item type=\"link\" id=\"" + SingleItem["ItemID"] + "\" itemid=\"" + SingleItem["ID"] + "\" title=\"" + SingleItem["ItemTitle"] + "\">";
                        MenuXML = MenuXML + "</item>";
                        break;
                    case 2:
                        MenuXML = MenuXML + "<item type=\"library\" id=\"" + SingleItem["ItemID"] + "\" itemid=\"" + SingleItem["ID"] + "\" title=\"" + SingleItem["ItemTitle"] + "\">";
                        MenuXML = MenuXML + "</item>";
                        break;
                }
            }
        }
        MenuXML = "<menu folder=\"" + SubMenu["FolderID"].ToString() + "\" id=\"" + SubMenu["MenuID"].ToString() + "\" title=\"" + SubMenu["MenuTitle"].ToString() + "\" template=\"" + SubMenu["MenuTemplate"].ToString() + "\">" + MenuXML;
        MenuXML = MenuXML + "</menu>";
        return MenuXML;
    }
}
