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
//using System.DateTime;
using System.IO;
using Ektron.Cms.Content;


	public partial class viewmenu : System.Web.UI.UserControl
	{
		
		
		protected CommonApi m_refCommon = new CommonApi();
		protected StyleHelper m_refstyle = new StyleHelper();
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected EkContent m_refContent;
		protected ContentAPI m_refContentApi;
		protected long MenuId = 0;
		protected int MenuLanguage = -1;
		protected LanguageData language_data;
		protected List<AxMenuItemData> menu_item_data;
		protected long ParentId = 0;
		protected string m_strViewItem = "item";
		protected bool AddDeleteIcon = false;
		protected string m_strMenuName = "";
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected string m_strDelConfirm = "";
		protected string m_strDelItemsConfirm = "";
		protected string m_strSelDelWarning = "";
		protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
		protected string m_strBackPage = ""; // URL to use to return to the current menu page
		
		protected string m_strTitle = "";
		protected string m_strImage = "";
		protected string m_strLink = "";
		protected string m_strTemplate = "";
		protected string m_strDescription = "";
		protected string m_strFolderAssociations = "";
		protected string m_strTemplateAssociations = "";
		protected string sitePath = "";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = m_refCommon.EkMsgRef;
			AppImgPath = m_refCommon.AppImgPath;
			AppPath = m_refCommon.AppPath;
			m_strPageAction = Request.QueryString["action"];
			Utilities.SetLanguage(m_refCommon);
			MenuLanguage = m_refCommon.ContentLanguage;
			MenuId = Convert.ToInt64(Request.QueryString["menuid"]);
			if (Request.QueryString["view"] != null)
			{
				m_strViewItem = Request.QueryString["view"];
			}
			m_refContent = m_refCommon.EkContentRef;
			m_refContentApi = new ContentAPI();
			Utilities.SetLanguage(m_refContentApi);
			sitePath = m_refCommon.SitePath;
			
			m_strBackPage = Request.QueryString.ToString();
			// strip off refresh indicator
			if (m_strBackPage.EndsWith("&rf=1"))
			{
				// refresh is needed after we edit a submenu, but we don't want to keep refreshing if we use the same URL
				m_strBackPage = m_strBackPage.Substring(0, m_strBackPage.Length - 5);
			}
			
			DisplayPage();
		}
		
		private void DisplayPage()
		{
			AxMenuData menu;
			menu = m_refContentApi.EkContentRef.GetMenuDataByID(MenuId);
			
			if (menu != null)
			{
				m_strMenuName = menu.Title;
				m_strTitle = menu.Title;
				m_strImage = menu.Image;
				if (menu.Image == "")
				{
					chkOverrideImage.Visible = false;
				}
				else
				{
					chkOverrideImage.Text = m_refMsg.GetMessage("alt Use image instead of a title");
					if (menu.ImageOverride)
					{
						chkOverrideImage.Checked = true;
					}
				}
                if (menu.Link != "") 
                {
                    m_strLink = (!menu.Link.StartsWith("ftp://") && !menu.Link.StartsWith("http://") && !menu.Link.StartsWith("https://") ? sitePath : "") + menu.Link;
                }
				if (menu.Template != "")
				{
					m_strTemplate = sitePath + menu.Template;
				}
				m_strDescription = menu.Description;
				
				
				if (menu.AssociatedFolderIdList != null)
				{
					foreach (string folderid in menu.AssociatedFolderIdList.Split(";".ToCharArray()))
					{
						if (folderid != "")
						{
							FolderData folderinfo = m_refContentApi.GetFolderById(Convert.ToInt64(folderid), false);
							if (folderinfo != null)
							{
								if (m_strFolderAssociations != "")
								{
									m_strFolderAssociations = m_strFolderAssociations + "<BR>";
								}
								m_strFolderAssociations = m_strFolderAssociations + folderinfo.NameWithPath + " (ID:" + folderid + ")";
							}
						}
					}
				}
				
				
				if (menu.AssociatedTemplates != null)
				{
					foreach (string template in menu.AssociatedTemplates.Split(";".ToCharArray()))
					{
						if (template != "")
						{
							if (m_strTemplateAssociations != "")
							{
								m_strTemplateAssociations = m_strTemplateAssociations + "<BR>";
							}
							m_strTemplateAssociations = m_strTemplateAssociations + template;
						}
					}
				}
			}
			
			if (m_strPageAction != "viewcontent")
			{
				chkOverrideImage.Enabled = false;
			}
			
			MenuToolBar(menu);
		}
		
		private void MenuToolBar(AxMenuData menu)
		{
			string strDeleteMsg = "";
			
			strDeleteMsg = m_refMsg.GetMessage("alt delete button text (menu)");
			m_strDelConfirm = m_refMsg.GetMessage("delete menu confirm");
			m_strDelItemsConfirm = m_refMsg.GetMessage("delete menu items confirm");
			m_strSelDelWarning = m_refMsg.GetMessage("select menu item missing warning");
			
			divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string) (m_refMsg.GetMessage("view menu title") + " \"" + m_strMenuName + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) + "\' />"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			string backPage = EkFunctions.UrlEncode(Request.Url.ToString());

            string langParm = ((MenuLanguage > 0) ? "&LangType=" + MenuLanguage : "");

			if (m_strPageAction != "viewcontent")
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("menu.aspx?action=viewcontent&view=item&menuid=" + MenuId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", (string)("collections.aspx?action=EditMenu&nid=" + MenuId + "&folderid=" + menu.FolderID + langParm + "&back=" + backPage), m_refMsg.GetMessage("edit menu title"), m_refMsg.GetMessage("edit menu title"), "", StyleHelper.EditButtonCssClass, true));
			
			long ParentMenuId = menu.ParentID;
			long AncestorMenuId = menu.AncestorID;
			long FolderID = menu.FolderID;
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refstyle.GetHelpButton("ViewMenu", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
	}