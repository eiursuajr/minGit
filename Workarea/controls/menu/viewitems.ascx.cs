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

public partial class viewmenuitems : System.Web.UI.UserControl
{
    protected CommonApi m_refCommon = new CommonApi();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected ContentAPI m_refContentApi;
    protected long MenuId = 0;
    protected int MenuLanguage = -1;
    protected LanguageData language_data;
    protected List<AxMenuItemData> menu_item_data;
    protected long ParentId = 0;
    protected long FoldId = 0;
    protected long m_AncestorMenuId = 0;
    protected string m_strViewItem = "item";
    protected bool AddDeleteIcon = false;
    protected long MenuItemCount = 0;
    protected string m_strMenuName = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strDelConfirm = "";
    protected string m_strDelItemsConfirm = "";
    protected string m_strSelDelWarning = "";
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected string m_strBackPage = ""; // URL to use to return to the current menu page
    protected bool reloadTree = false;

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
        if (!string.IsNullOrEmpty( Request.QueryString["folderid"]))
        {
            FoldId = long.Parse(Request.QueryString["folderid"].ToString());
        }
        m_refContent = m_refCommon.EkContentRef;
        m_refContentApi = new ContentAPI();
        Utilities.SetLanguage(m_refContentApi);

        m_strBackPage = Request.QueryString.ToString();
        // strip off refresh indicator
        if (m_strBackPage.EndsWith("&rf=1"))
        {
            // refresh is needed after we edit a submenu, but we don't want to keep refreshing if we use the same URL
            m_strBackPage = m_strBackPage.Substring(0, m_strBackPage.Length - 5);
        }

        reloadTree = (Request.QueryString["rf"] != null && Request.QueryString["rf"] == "1");

        if (IsPostBack == false)
        {
            DisplayPage();
            RegisterResources();
        }
        else
        {
            if (Request.Form[submittedaction.Name] == "deleteitem")
            {
                // handle deleting menu items
                string contentids = Request.Form[frm_item_ids.Name];
                Server.Transfer((string)("collections.aspx?action=doDeleteMenuItem&folderid=" + MenuId + "&nid=" + MenuId + "&ids=" + contentids + "&back=" + EkFunctions.UrlEncode(Request.Url.ToString())), true);
            }
            else if (Request.Form[submittedaction.Name] == "delete")
            {
                // handle deleting the menu
                AxMenuData menu;
                menu = m_refContent.GetMenuDataByID(MenuId);
                if (FoldId == 0)
                    reloadTree = true;
                if (reloadTree)
                {
                    Server.Transfer((string)("collections.aspx?action=doDeleteMenu&reloadtrees=menu&nid=" + MenuId + "&back=" + EkFunctions.UrlEncode((string)("menu.aspx?action=deleted&title=" + menu.Title))), true);
                }
                else
                {
                    Server.Transfer((string)("collections.aspx?action=doDeleteMenu&folderid=" + FoldId + "&nid=" + MenuId + "&back=" + EkFunctions.UrlEncode((string)("menu.aspx?action=deleted&title=" + menu.Title))), true);
                }
            }
        }
        isPostData.Value = "true";
    }

    private void DisplayPage()
    {
        //taxonomy_request.IncludeItems = True
        //taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize
        //taxonomy_request.CurrentPage = m_intCurrentPage
        //taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
        //menu_item_data = m_refContent.GetMenuContent(MenuId, MenuLanguage)
        //Dim menu As Collection = m_refContent.GetMenuByID(MenuId, False)

        AxMenuData menu;
        menu = m_refContentApi.EkContentRef.GetMenuDataByID(MenuId);

        if (menu != null)
        {
            if (reloadTree)
            {
                List<string> menuIds = new List<string>();
                long parentId = menu.ParentID;
                while (parentId > 0)
                {
                    menuIds.Insert(0, parentId.ToString());
                    parentId = m_refContentApi.EkContentRef.GetMenuDataByID(parentId).ParentID;
                }
                ReloadClientScript(string.Join("/", menuIds.ToArray()));
            }

            ParentId = menu.ParentID;
            m_AncestorMenuId = menu.AncestorID;
            if (ParentId == 0)
            {
                // this matches the legacy code but it doesn't make sense for a submenu's
                // parent and grandparent to be the same ID to me...the grandparent should be root(0) :-P
                ParentId = menu.ID;
            }
            m_strMenuName = menu.Title; // menu("MenuTitle")
            //m_intTotalPages = taxonomy_request.TotalPages
        }

        PopulateContentGridData(menu);
        MenuToolBar(menu);
    }
    private void PopulateContentGridData(AxMenuData menu)
    {
        if (m_strPageAction == "removeitems")
        {
            MenuItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=\"Checkbox\" name=\"checkall\" onclick=\"checkAll(\'frm_content_ids\',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
        }
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("TITLE", m_refMsg.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), false, false));
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("LANGUAGE", m_refMsg.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("URL", m_refMsg.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(20), false, false));
        DataTable dt = new DataTable();
        DataRow dr;
        if (m_strPageAction == "removeitems")
        {
            dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("URL", typeof(string)));
        //If (m_strViewItem <> "folder") Then
        PageSettings();
        //If (menu_item_data.Count > 0) Then
        if (menu.Item.Length > 0)
        {
            AddDeleteIcon = true;
            string icon = "";
            string title = "";
            string link = "";
            string backPage = EkFunctions.UrlEncode(Request.Url.ToString());
            //For Each item As AxMenuItemData In menu_item_data
            foreach (AxMenuItemData item in menu.Item)
            {
                if (item == null)
                {
                    continue;
                }


                if ((Convert.ToInt32(item.ItemType) == 4) && (m_strPageAction == "removeitems"))
                {
                    // submenus need to be deleted individually so they shouldn't show up in this list!
                    continue;
                }

                MenuItemCount++;
                dr = dt.NewRow();
                if (m_strPageAction == "removeitems")
                {
                    dr["CHECK"] = "<input type=\"checkbox\" id=\"frm_content_ids\" name=\"frm_content_ids\" value=\"" + item.ID + "\" onclick=\"checkAll(\'frm_content_ids\',true);\">";
                }

                //backPage = EkFunctions.UrlEncode("Action=viewcontent&view=item&menuid=" & MenuId)
                //link = "<a href='content.aspx?action=View&LangType=" & item.ContentLanguage & "&id=" & item.ID & "&callerpage=menu.aspx&origurl=" & backPage & "' title='" & title & "'>" & item.ItemTitle & "</a>"

                title = (string)(m_refMsg.GetMessage("generic View") + " \"" + item.ItemTitle.Replace(" \'", "`") + "\"");
                string editmenuitemurl;
                editmenuitemurl = (string)("collections.aspx?action=EditMenuItem&nid=" + MenuId + "&id=" + item.ID + "&Ty=" + item.ItemType + "&back=" + backPage);
                link = "<a href=\'" + editmenuitemurl + " \'>" + item.ItemTitle + "</a>";

                Collection iteminfo = null;

                string assetimageurl = "";

                if ((Convert.ToInt32(item.ItemType) == 1))
                {
                    if (item.ItemSubType == 8)
                    {
                        iteminfo = m_refContentApi.EkContentRef.GetMenuItemByID(item.ItemID, item.ID, false);
                        // this is a DMS asset so we have to look up the icon for it because the menu api doesn't have this
                        Ektron.Cms.ContentData assetcontentdata = m_refContent.GetContentById(Convert.ToInt64(iteminfo["ItemID"]), Ektron.Cms.Content.EkContent.ContentResultType.Published);
                        AssetManagement.AssetManagementService service = new AssetManagement.AssetManagementService();
                        Ektron.ASM.AssetConfig.AssetFileInformation fileInfo = service.GetFileInformation(assetcontentdata.AssetData.Version);
                        assetimageurl = fileInfo.ImageUrl;
                        icon = "<img src=\"" + fileInfo.ImageUrl + "\" />&nbsp;";
                    }
                    else
                    {
                        icon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentHtml.png" + "\" />&nbsp;";
                    }
                }
                else if ((Convert.ToInt32(item.ItemType) == 2))
                {
                    icon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentForm.png" + "\" />&nbsp;";
                }
                else if ((Convert.ToInt32(item.ItemType) == 4))
                {
                    icon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/menu.png" + "\" />&nbsp;";
                    link = "<a href=\'menu.aspx?Action=viewcontent&menuid=" + item.ID + "&treeViewId=-3" + "\'>" + item.ItemTitle + "</a>";
                }
                else if ((Convert.ToInt32(item.ItemType) == 5))
                {
                    icon = "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/link.png" + "\" />&nbsp;";
                }

                if ((Convert.ToInt32(item.ItemType) == 1) || (Convert.ToInt32(item.ItemType) == 2))
                {
                    if (iteminfo == null)
                    {
                        iteminfo = m_refContentApi.EkContentRef.GetMenuItemByID(item.ItemID, item.ID, false);
                    }

                    int itemtype = (int)item.ItemType;
                    if (itemtype == 2)
                    {
                        //For Library Items , ItemID key is a libraryId instead of ContentID and ItemType has to be passed.
                        // this is contenttype 7 for the menu generator.
                        itemtype = 7;
                        dr["TITLE"] = m_refContentApi.GetDmsContextMenuHTML(Convert.ToInt64(iteminfo["ContentID"]), Convert.ToInt64(iteminfo["ContentLanguage"]), itemtype, item.ItemContentSubType, item.ItemTitle, (string)(m_refMsg.GetMessage("edit menu items title") + " " + item.ItemTitle), editmenuitemurl, "", assetimageurl);
                    }
                    else
                    {
                        dr["TITLE"] = m_refContentApi.GetDmsContextMenuHTML(Convert.ToInt64(iteminfo["ItemID"]), Convert.ToInt64(iteminfo["ContentLanguage"]), Convert.ToInt64(iteminfo["ContentType"]), item.ItemContentSubType, item.ItemTitle, (string)(m_refMsg.GetMessage("edit menu items title") + " " + item.ItemTitle), editmenuitemurl, "", assetimageurl);
                    }
                    if ((Convert.ToInt32(item.ItemType) == 1) && (Convert.ToInt32(item.ItemSubType) == 8) && (string.IsNullOrEmpty(iteminfo["ItemLink"].ToString())))
                    {
                        //Using the contentblock control to get the exact quicklink for asset which is not in contendata used above.
                        //This is because the  contentdata or assetdata has nothing to show difference btw assets and privateassets.
                        //Regarding the defect #54892
                        Ektron.Cms.Controls.ContentBlock cBlock = new Ektron.Cms.Controls.ContentBlock();
                        cBlock.Page = this.Page;
                        cBlock.DefaultContentID = Convert.ToInt64(iteminfo["ItemID"]);
                        cBlock.Fill();
                        dr["URL"] = cBlock.EkItem.QuickLink;
                    }
                    else
                    {
                        dr["URL"] = iteminfo["ItemLink"];
                    }
                }
                else
                {
                    dr["TITLE"] = icon + link;
                    dr["URL"] = item.ItemLink;
                }
                dr["ID"] = item.ID;
                dr["LANGUAGE"] = item.ContentLanguage;
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        MenuItemList.DataSource = dv;
        MenuItemList.DataBind();
    }

    private void MenuToolBar(AxMenuData menu)
    {
        string strDeleteMsg = "";

        strDeleteMsg = m_refMsg.GetMessage("alt delete button text (menu)");
        m_strDelConfirm = m_refMsg.GetMessage("delete menu confirm");
        m_strDelItemsConfirm = m_refMsg.GetMessage("delete menu items confirm");
        m_strSelDelWarning = m_refMsg.GetMessage("select menu item missing warning");


        if (m_strMenuName != null && m_strMenuName.Length > 50)
        { 
            m_strMenuName = m_strMenuName.ToString().Remove(50) + " ....";
        }
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("view menu title") + " \"" + m_strMenuName + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) + "\' />"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        string backPage = EkFunctions.UrlEncode(Request.Url.ToString());

        long ParentMenuId = menu.ParentID;
        long FolderID = menu.FolderID;

		if (m_strPageAction != "viewcontent")
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("menu.aspx?action=viewcontent&view=item&menuid=" + MenuId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (m_strPageAction != "removeitems")
        {
            // folder ID is 0 here to start the selection of content items at root!
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", (string)("collections.aspx?action=AddMenuItem&nid=" + MenuId + "&folderid=" + FolderID + "&back=" + backPage + "&parentid=" + MenuId + "&ancestorid=" + m_AncestorMenuId), m_refMsg.GetMessage("add collection items"), m_refMsg.GetMessage("add collection items"), "", StyleHelper.AddButtonCssClass, true));
            if (MenuItemCount > 0)
            {
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/remove.png", (string)("menu.aspx?action=removeitems&menuid=" + MenuId + "&parentid=" + ParentId), m_refMsg.GetMessage("remove menu items"), m_refMsg.GetMessage("remove menu items"), "", StyleHelper.RemoveButtonCssClass));
            }
            if (MenuItemCount > 1)
            {
                string treeViewIdParam = "";
                if (!string.IsNullOrEmpty(Request.QueryString["treeViewId"]))
                    treeViewIdParam = "&treeViewId=" + Request.QueryString["treeViewId"].ToString();
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/arrowUpDown.png", (string)("collections.aspx?action=ReOrderMenuItems&nid=" + MenuId + "&folderid=" + ParentId + treeViewIdParam + "&back=" + backPage), m_refMsg.GetMessage("reorder menu title"), m_refMsg.GetMessage("alt: update menu order text"), "", StyleHelper.ReOrderButtonCssClass));
            }
            //result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=EditMenu&nid=" & MenuId & "&folderid=" & ParentId & "&back=" & backPage _
            //                                                 , m_refMsg.GetMessage("edit menu title"), m_refMsg.GetMessage("edit menu title"), ""))
            string langParm = ((MenuLanguage > 0) ? "&LangType=" + MenuLanguage : "");
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/properties.png", (string)("menu.aspx?action=viewmenu&menuid=" + MenuId + "&parentid=" + ParentId + langParm), m_refMsg.GetMessage("alt menu properties button text"), m_refMsg.GetMessage("properties text"), "", StyleHelper.ViewPropertiesButtonCssClass));
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("generic delete title"), m_refMsg.GetMessage("alt delete menu"), "onclick=\"return DeleteItem();\"", StyleHelper.DeleteButtonCssClass));
        }
        else
        {
            if (AddDeleteIcon)
            {
                // deletes checked/selected menu items
                divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("remove items from menu") + " \"" + m_strMenuName + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) + "\' />"));
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "onclick=\"return DeleteItem(\'items\');\"", StyleHelper.RemoveButtonCssClass, true));
            }
        }

		
        string backpagelang = Server.UrlDecode(backPage);
        if (!backPage.Contains("LangType"))
        {
            backpagelang = backpagelang + "&LangType=" + MenuLanguage;
        }
        string addDD;
        if (menu.ParentID == 0)
        {
            addDD = ViewLangsForMenuID(MenuId, "", false, false, "javascript:addBaseMenu(" + MenuId + ", " + ParentMenuId + ", " + m_AncestorMenuId + ", " + FolderID + ", this.value, \'" + backpagelang + "\');");
            if (addDD != "")
            {
                addDD = (string)("&nbsp;" + m_refMsg.GetMessage("generic add title") + ":&nbsp;" + addDD);
            }
            if (m_refContentApi.EnableMultilingual == 1)
            {
				result.Append(StyleHelper.ActionBarDivider);

				result.Append("<td nowrap=\"true\">");
				result.Append("" + m_refMsg.GetMessage("generic view") + ":&nbsp;" + ViewLangsForMenuID(MenuId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
				result.Append("</td>");
			}
        }
        
		result.Append(StyleHelper.ActionBarDivider);
        if (m_strPageAction != "removeitems")
            result.Append("<td>" + m_refstyle.GetHelpButton("ViewMenu", "") + "</td>");
        else
            result.Append("<td>" + m_refstyle.GetHelpButton("RemoveMenuItem", "") + "</td>");
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
        contObj = m_refContent;

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
            if (MenuLanguage.ToString() == "-1")
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

        if ((TransCol.Count > 0) && (m_refContentApi.EnableMultilingual == 1))
        {
            foreach (Collection Col in TransCol)
            {
                if (MenuLanguage.ToString() == Col["LanguageID"].ToString())
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

    private string FindSelected(string chk)
    {
        string val = "";
        if (m_strViewItem == chk)
        {
            val = " selected ";
        }
        return val;
    }

    //Private Function GetLanguageForTaxonomy(ByVal TaxonomyId As Long, ByVal BGColor As String, ByVal ShowTranslated As Boolean, ByVal ShowAllOpt As Boolean, ByVal onChangeEv As String) As String
    //    Dim result As String = ""
    //    Dim frmName As String = ""
    //    Dim result_language As IList(Of LanguageData) = Nothing
    //    Dim taxonomy_language_request As New TaxonomyLanguageRequest
    //    taxonomy_language_request.TaxonomyId = TaxonomyId

    //    If (ShowTranslated) Then
    //        taxonomy_language_request.IsTranslated = True
    //        result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
    //        frmName = "frm_translated"
    //    Else
    //        taxonomy_language_request.IsTranslated = False
    //        result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
    //        frmName = "frm_nontranslated"
    //    End If

    //    result = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbCrLf

    //    If (CBool(ShowAllOpt)) Then
    //        If TaxonomyLanguage = -1 Then
    //            result = result & "<option value=""-1"" selected>All</option>"
    //        Else
    //            result = result & "<option value=""-1"">All</option>"
    //        End If
    //    Else
    //        If (ShowTranslated = False) Then
    //            result = result & "<option value=""0"">-select language-</option>"
    //        End If
    //    End If
    //    If ((result_language IsNot Nothing) AndAlso (result_language.Count > 0) AndAlso (m_refCommon.EnableMultilingual = 1)) Then
    //        For Each language As LanguageData In result_language
    //            If TaxonomyLanguage = language.Id Then
    //                result = result & "<option value=" & language.Id & " selected>" & language.Name & "</option>"
    //            Else
    //                result = result & "<option value=" & language.Id & ">" & language.Name & "</option>"
    //            End If
    //        Next
    //    Else
    //        result = ""
    //    End If
    //    If (result.Length > 0) Then
    //        result = result & "</select>"
    //    End If
    //    Return (result)
    //End Function
    private void PageSettings()
    {
        if (m_intTotalPages <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(m_intTotalPages))).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                PreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (m_intCurrentPage == m_intTotalPages)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }
    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        PreviousPage.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }
    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        DisplayPage();
        isPostData.Value = "true";
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE6);
    }

    private void ReloadClientScript(string idPath)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        try
        {
            idPath = "/" + idPath + "/";
            result.Append("top.TreeNavigation(\"MenuTree\", \"" + idPath + "\");" + "\r\n");
            Ektron.Cms.API.JS.RegisterJSBlock(this.Parent.Parent.Parent.Page.Header, result.ToString(), "ReloadClientScript");
        }
        catch (Exception)
        {
        }
    }
}