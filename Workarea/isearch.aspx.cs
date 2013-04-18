using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;
using System.Collections.Generic;
using Microsoft.Security.Application;

public partial class isearch : System.Web.UI.Page
{
    #region Member Variables - Private
    private LocalizationAPI _LocalizationApi = new LocalizationAPI(); 
    private Collection pagedata = null;
    private bool isPostBackData = false;
    #endregion

    #region Member Variables - Protected

    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg = null;
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected int EnableMultilingual = 0;
    protected int ContentLanguage = 0;
    protected ContentAPI m_refContentApi;
    protected long m_intFolderId = 0;
    protected string m_strLibType = "";
    protected string AppName = "";
    protected LibraryConfigData lib_setting_data = null;
    protected string m_strSource = "";
    protected SearchContentItem[] search_result = null;
    protected SearchResultData[] search_result_lib = null;
    protected string[] arr_TdName;
    protected int item = 0;
    protected bool m_bLibrary = false;
    protected bool IsBrowserIE = false;
    protected bool IsBrowserIE6Plus = false;
    protected bool IsMac = false;
    protected string SearchJScript = "";
    protected string StyleSheetJS = "";
    protected string SearchStyleSheet = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected int m_intTotalRecords = 1;
    protected SearchCacheData[] m_arySearchResults = null;
    protected string sEditor = "";
    protected string sLinkText = "";
    protected bool showThumbnail = true;
    protected string SitePath = "";
    protected string imagePath = "";
    protected FolderData folder_data = new FolderData();
    protected EkEnumeration.FolderType folderType = EkEnumeration.FolderType.Content;
    protected string caller = string.Empty;
	protected string contLangID = string.Empty;
	
    #endregion

    #region Events

    private void Page_Init(System.Object sender, System.EventArgs e)
    {

        Page.EnableViewState = false;
        m_refContentApi = new ContentAPI();
        m_refMsg = m_refContentApi.EkMsgRef;
        //Register Page Components
        this.RegisterPackages();

        uxTabBasic.Text = m_refMsg.GetMessage("search text");
        uxTabAdvanced.Text = m_refMsg.GetMessage("advanced search text");
        hdnSelectedTab.Value = "0";
        imagePath = m_refContentApi.AppPath + "images/ui/icons/";

    }
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            if (m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect((string)("reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
			contLangID = ContentLanguage.ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["caller"]))
            {
                caller = Request.QueryString["caller"];
            }
            m_refContentApi.ContentLanguage = ContentLanguage;
            if (!(Request.QueryString["action"] == null))
            {
                m_strPageAction = AntiXss.UrlEncode(Request.QueryString["action"]);
                if (m_strPageAction == "showLibdlg")
                {
                    hdnSelectedTab.Value = "1";
                }
            }
            ltrAction.Text = m_strPageAction;
            if (!(Request.QueryString["folderid"] == null))
            {
                if (Request.QueryString["folderid"] != "")
                {
                    m_intFolderId = Convert.ToInt64(Request.QueryString["folderid"]);
                }
            }

            folder_data = m_refContentApi.GetFolderById(m_intFolderId);
            folderType = (EkEnumeration.FolderType)folder_data.FolderType;

            sEditor = Request.QueryString["EditorName"];
            if (!String.IsNullOrEmpty(Request.QueryString["selected"]))
            {
                sLinkText = Request.QueryString["selected"];
            }
            if (!(Request.QueryString["type"] == null))
            {
                m_strLibType = Request.QueryString["type"];
            }

            if (!(Request.QueryString["source"] == null))
            {
                m_strSource = EkFunctions.HtmlEncode(Request.QueryString["source"]);
            }
            if (Request.QueryString["showthumb"] != null)
            {
                if (Request.QueryString["showthumb"] == "false")
                {
                    showThumbnail = false;
                }
            }
            if (Request.Browser.Type.IndexOf("IE") != -1)
            {
                IsBrowserIE = true;
                if (Request.Browser.MajorVersion >= 6)
                {
                    IsBrowserIE6Plus = true;
                }
            }

            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                IsMac = true;
            }

            StyleSheetJS = m_refStyle.GetClientScript();
            ctrlFirstPage.Text = "[" + m_refMsg.GetMessage("lbl first page") + "]";
            ctrlPreviousPage.Text = "[" + m_refMsg.GetMessage("lbl previous page") + "]";
            ctrlNextPage.Text = "[" + m_refMsg.GetMessage("lbl next page") + "]";
            ctrlLastPage.Text = "[" + m_refMsg.GetMessage("lbl last page") + "]";
            // the following literal are defined in the include javascript file, mediaupldr_common
            jsEditorClosed.Text = m_refMsg.GetMessage("js: alert editor closed");
            jsScope.Text = Request.QueryString["scope"];
            jsEditorName.Text = Request.QueryString["EditorName"];
            jsDEntrylink.Text = Request.QueryString["dentrylink"];

            AppImgPath = m_refContentApi.AppImgPath;
            AppName = m_refContentApi.AppName;
            EnableMultilingual = m_refContentApi.EnableMultilingual;
            SitePath = m_refContentApi.SitePath;
            lib_setting_data = m_refContentApi.GetLibrarySettings(m_intFolderId);
            Page.Title = AppName + " " + "Collections";
            IsExtensionValid.Text = ClientScript_IsExtensionValid(lib_setting_data);
            if (m_strSource == "edit" || m_strSource == "libinsert" || m_strSource == "mediainsert")
            {
                body.Attributes.Add("class", "library");
            }
            else
            {
                body.Attributes.Add("class", "UiMain");
            }
            frm_folder_id.Value = Convert.ToString(m_intFolderId);
            frm_object_type.Value = Request.QueryString["ObjectType"];

            CustFieldsContentLit.Text = "";
            CustFieldsLibraryLit.Text = "";
            HiddenData.Text = "";

            //If coming to search first time, show 1st tab -
            //unless coming first time from mediainsert.aspx, only show the 2nd tab in that case.
            if (Page.IsPostBack == false && m_strSource != "mediainsert" && m_strSource != "edit")
            {
                //Make the first tab selected
                hmenuSelected.Value = "0";
                uxSearchTabs.SetActiveTab(uxTabBasic);  
                //mvSearch.SetActiveView(vwSearchPublished);
                Display_ShowDlg_ToolBar();
            }

            //If coming from MediaInsert.aspx (inserting library items while adding/editing content, show only the second tab
            if (Page.IsPostBack == false && (m_strSource == "edit" || m_strSource == "mediainsert"))
            {
                hmenuSelected.Value = "1";
                //mvSearch.SetActiveView(vwSearchAdvanced);
                uxSearchTabs.SetActiveTab(uxTabAdvanced);
                Display_ShowLibdlg_ToolBar();
            }
            else if (Page.IsPostBack == true && (m_strSource == "edit" || m_strSource == "mediainsert"))
            {
                hdnSelectedTab.Value = "1";
            }
            
            object obj = HttpContext.Current.Request.Form["__EVENTTARGET"];
            if (obj != null && (obj.ToString().ToLower() == "uxsearchtabs$uxtabbasic" || obj.ToString().ToLower() == "uxsearchtabs$uxtabadvanced"))
            {
                isPostBackData = true;
            }

            if (websearch1 != null)
            {
                websearch1.Language = ContentLanguage;
                websearch1.FolderID = m_intFolderId;
                websearch1.IsInWorkArea = true;
            }

            if (Page.IsCallback)
            {
                return;
            }

            switch (m_strPageAction)
            {
                case "showdlg":
                    Display_ShowDlg();
                    SearchAssetDispayRequest sadReq = new SearchAssetDispayRequest();
                    sadReq.StartingFolder = "/";
                    sadReq.Recursive = true;
                    sadReq.TargetPage = "isearch.asp?Action=searchallassetsdisplay";
                    sadReq.ButtonText = m_refMsg.GetMessage("res_isrch_btn");
                    sadReq.FontFace = "Verdana";
                    sadReq.FontColor = "#808080";
                    sadReq.FontSize = "2";
                    sadReq.DynamicIncludeAssetTypes = true;
                    sadReq.DynamicIncludeCmsTypes = true;
                    sadReq.EnableShowLibrary = false; // remove library search from workareas-content-search.
                    sadReq.EnableBasicSearchLink = false; // disable the basic link.
                    sadReq.SearchHeaderText = m_refMsg.GetMessage("lbl srch text");
                    CustFieldsContentLit.Text = CustomFields.ecmSearchAllAssets(sadReq).ToString();
                    break;
                case "showLibdlg":
                    Display_ShowLibdlg();
                    CustFieldsLibraryLit.Text = CustomFields.WriteLibrarySearchExtended(m_intFolderId).ToString();
                    break;
                case "dofindcontent":
                    TR_showLibdlg.Visible = false;
                    TR_showdlg.Visible = false;
                    PostBack_DoFindContent();
                    break;
                case "dofindlibrary": //This action is routed from form details page
                    TR_showLibdlg.Visible = false;
                    TR_showdlg.Visible = false;
                    PostBack_DoFindLibrary();
                    break;
            }
            //End If
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().IndexOf("service is not running") != -1)
            {
                Utilities.ShowError("Error: Index service is not running.  You cannot search on Documents.  Restart the service or perform only on HTML content search");
            }
            else
            {
                Utilities.ShowError(ex.Message);
            }
        }
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = int.Parse((string)hTotalPages.Value);
                break;
            case "Next":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)hCurrentPage.Value) + 1);
                break;
            case "Prev":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)hCurrentPage.Value) - 1);
                break;
        }
        PostBack_DoFindContent();
        CustFieldsContentLit.Text = "";
        //Reset pageLink so search can run next time
        pageLink.Value = "";
    }

    protected void lbSearchPublished_Click(object sender, EventArgs e)
    {
        hdnSelectedTab.Value = "0";
        //mvSearch.SetActiveView(vwSearchPublished);
        uxSearchTabs.SetActiveTab(uxTabBasic);  
        if (m_strPageAction == "showLibdlg")
        {
            Display_ShowLibdlg_ToolBar();
        }
        else
        {
            Display_ShowDlg_ToolBar();
        }
    }

    protected void lbSearchAdvanced_Click(object sender, EventArgs e)
    {
        hdnSelectedTab.Value = "1";
        //mvSearch.SetActiveView(vwSearchAdvanced);
        uxSearchTabs.SetActiveTab(uxTabAdvanced);  
        if (m_strPageAction == "showLibdlg")
        {
            Display_ShowLibdlg_ToolBar();
        }
        else
        {
            Display_ShowDlg_ToolBar();
        }
    }

    #endregion

    #region Helpers

    private void Display_ShowLibdlg()
    {
        if ((!(Page.IsPostBack)) || isPostBackData == true)
        {
            TR_showLibdlg.Visible = true;
            TR_showdlg.Visible = false;
            Display_ShowLibdlg_ToolBar();
        }
        else
        {
            TR_showLibdlg.Visible = false;
            TR_showdlg.Visible = false;
            PostBack_DoFindLibrary();
        }
    }
    private void PostBack_DoFindLibrary()
    {
        SearchAssetRequest sar = new SearchAssetRequest();
        string strViewMode = "";

        pagedata = new Collection();
        pagedata.Add(Request.Form["frm_library_title"], "SearchTerm", null, null);
        pagedata.Add(System.Convert.ToInt32(Request.Form["frm_libtype_id"]), "LibTypeID", null, null);
        if (Request.Form["frm_library_link"] == "1")
        {
            pagedata.Add(1, "SearchLink", null, null);
        }
        else
        {
            pagedata.Add(0, "SearchLink", null, null);
        }
        if (Request.Form["frm_user_only_content"] == "1")
        {
            pagedata.Add(1, "UserOnlyAssets", null, null);
        }
        else
        {
            pagedata.Add(0, "UserOnlyAssets", null, null);
        }
        if (Request.Form["frm_library_description"] == "1")
        {
            pagedata.Add(1, "SearchTeaser", null, null);
        }
        else
        {
            pagedata.Add(0, "SearchTeaser", null, null);
        }
        if (Request.Form["frm_library_tags"] == "1")
        {
            pagedata.Add(1, "SearchTags", null, null);
        }
        else
        {
            pagedata.Add(0, "SearchTags", null, null);
        }

        sar = CustomFields.Populate_AssetRequestObjectFromForm(null);
        sar.isWorkareaSearch = true;

        search_result_lib = m_refContentApi.InternalLibrarySearch(pagedata, sar);

        //If there is no dropdown on the page ie. clicked on a page link in results,
        //use old value of dropdown from the hidden field pageMode
        //else use dropdown value and, update pageMode
        if (!(Request.Form["selLibDisplayMode"] == null))
        {
            strViewMode = (string)(Request.Form["selLibDisplayMode"].ToLower());
            pageMode.Value = strViewMode;
        }
        else
        {
            strViewMode = (string)pageMode.Value;
        }

        //Since viewstate is false for the page, ensure the active tab is set correctly.
        if (Convert.ToInt32(hdnSelectedTab.Value) == 0)
            uxSearchTabs.SetActiveTab(uxTabBasic); 
        else
            uxSearchTabs.SetActiveTab(uxTabAdvanced); 
        hmenuSelected.Value = hdnSelectedTab.Value;


        if ((Request["source"] == "edit") || (Request["source"] == "libinsert") || (Request["source"] == "mediainsert"))
        {
            Populate_SearchResultGrid_Library();
            preview_type.Value = m_strLibType;
        }
        else
        {
            if ("graphical" == strViewMode)
            {
                BuildGraphicalResults_WorkareaLibrary(sar);
            }
            else
            {
                Populate_SearchResultGrid(System.Convert.ToBoolean("text" == strViewMode));
            }
        }
        PostBack_DoFindLibraryToolBar(search_result_lib);
    }
    private void Display_ShowLibdlg_ToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string TextSelected = "";
        string MixedSelected = "";

        string UserDefaultSelected = "mixed"; // m_refContentApi.WorkareaLibrarySearchResultMode
        if ("text" == UserDefaultSelected)
        {
            TextSelected = "selected";
        }
        else if ("mixed" == UserDefaultSelected)
        {
            MixedSelected = "selected";
        }
        else
        {
            TextSelected = "selected"; // default
        }

        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl search library folder")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";

        result.Append("<table><tr>");
        if (Request["source"] == "libinsert")
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", (string)("libraryinsert.aspx?action=ViewLibraryByCategory&id=" + m_intFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        else if ((Request["source"] == "mediainsert") || (Request["source"] == "edit"))
        {
            //return to media-inserter
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        else
        {
            //source is blank when u are not coming from mediainsert
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", (string)("library.aspx?action=ViewLibraryByCategory&id=" + m_intFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true)); 
			
			if (!(Request["source"] == "edit"))
            {
                //do not show the dropdown of text, mixed in the first tab
                if (hdnSelectedTab.Value == "1")
                {
                    // Show search result-mode select-control for the Workarea Library only:
                    result.Append("<td nowrap>");
                    result.Append("<select id=\"selLibDisplayMode\" style=\"display:none !important;\" name=\"selLibDisplayMode\" OnChange=\";\">");
                    result.Append("<option value=\'" + m_refMsg.GetMessage("lbl mixed") + "\' " + MixedSelected + ">" + m_refMsg.GetMessage("lbl mixed") + "</option>");
                    result.Append("<option value=\'" + m_refMsg.GetMessage("text") + "\' " + TextSelected + ">" + m_refMsg.GetMessage("text") + "</option>");
                    result.Append("</select>&nbsp;</td>");
                }
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("SearchLibraryFolder", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void Display_ShowDlg()
    {
        TR_showLibdlg.Visible = false;
        if ((!(Page.IsPostBack)) || isPostBackData == true)
        {
            TR_showdlg.Visible = true;
            Display_ShowDlg_ToolBar();
        }
        else
        {
            //If one of the page links has been clicked do not execute search as it executed when the
            //linkbutton's event handler is called.
            if (pageLink.Value != "pageLink")
            {
                TR_showdlg.Visible = false;
                PostBack_DoFindContent();
            }
        }
    }
    private void Display_ShowDlg_ToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string TextSelected = "";

        string GraphSelected = "";
        string UserDefaultSelected = m_refContentApi.WorkareaSearchResultMode;

        if ("text" == UserDefaultSelected)
        {
            TextSelected = "selected";
        }
        else if (("graphical" == UserDefaultSelected) && IsBrowserIE6Plus)
        {
            GraphSelected = "selected";
        }
        else
        {
            TextSelected = "selected"; // default
        }
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl search content folder")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";
        result.Append("<table><tr>");

        if (folderType == EkEnumeration.FolderType.Catalog)
        {
            uxTabBasic.Visible = false;
        }
        //do not show the dropdown of text, graphical in the first tab
        if (hdnSelectedTab.Value == "0" || folder_data.FolderType == Convert.ToInt32(EkEnumeration.FolderType.Catalog))
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + m_intFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            result.Append("<td>" + m_refStyle.GetHelpButton("SearchContentFolder", "") + "</td>");
            result.Append("</tr></table>");
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + m_intFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            result.Append("<td nowrap>");
            result.Append("<select style=\"display: none !important;\" id=\"selDisplayMode\" name=\"selDisplayMode\" bgcolor=\"blue\" OnChange=\";\">");
            // Graphical mode only supported on IE:
            if (IsBrowserIE6Plus)
            {
                result.Append("<option value=\'" + "Graphical" + "\' " + GraphSelected + ">" + m_refMsg.GetMessage("lbl graphical") + "</option>");
            }
            result.Append("<option value=\'" + "Text" + "\' " + TextSelected + ">" + m_refMsg.GetMessage("text") + "</option>");
            result.Append("</select>&nbsp;</td>");
			result.Append("<td>" + m_refStyle.GetHelpButton("SearchContentFolder", "") + "</td>");
            result.Append("</tr></table>");

        }

        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void PostBack_DoFindContent()
    {
        pagedata = new Collection();
        try
        {
            SearchAssetRequest sar = new SearchAssetRequest();
            Ektron.Cms.Content.EkContent ekc;
            Ektron.Cms.UrlAliasing.UrlAliasManualApi _manualAliasApi;
            Ektron.Cms.UrlAliasing.UrlAliasAutoApi _autoAliasApi;
            System.Collections.Generic.List<UrlAliasManualData> manualAliasList;
            System.Collections.Generic.List<UrlAliasAutoData> autoAliasList;
            string tempURL;
            string strLibPath = "";
            string strContentID = "";
            long contentID = 0;
            string strFormID = "";
            string strAssetName = "";
            string strLibtype = "";
            PagingInfo page;
            int index = 0;
            int contentType = 0;
            ekc = m_refContentApi.EkContentRef;

            strLibPath = (string)((!(Request.QueryString["libpath"] == null)) ? (Request.QueryString["libpath"]) : "");
            strContentID = (string)((!(Request.QueryString["content_id"] == null)) ? (Request.QueryString["content_id"]) : "");
            strFormID = (string)((!(Request.QueryString["form_id"] == null)) ? (Request.QueryString["form_id"]) : "");
            strAssetName = (string)((!(Request.QueryString["asset_name"] == null)) ? (Request.QueryString["asset_name"]) : "");
            strLibtype = (string)((!(Request.QueryString["libtype"] == null)) ? (Request.QueryString["libtype"]) : "");
            contentID = EkFunctions.ReadDbLong(strContentID);
            contentType = Convert.ToInt32(m_refContentApi.EkContentRef.GetContentType(contentID));
            if (strLibPath != "")
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }

            // Fixed #13770, search links from a form
            if ((strLibPath != "") || (strContentID != "") || (strFormID != "") || (strAssetName != ""))
            {
                // Checking links from known content block
                sar = new SearchAssetRequest();
                sar.FolderID = 0;
                sar.Recursive = true;
                sar.SearchContent = true;
                sar.AllowFragments = true;
                sar.SearchInHTML = true;
                sar.SearchForms = true;
                sar.SearchCatalog = System.Convert.ToBoolean(contentType == EkConstants.CMSContentType_CatalogEntry);

                string domain;
                domain = "";
                if ((strFormID != "") || (strContentID != ""))
                {
                    // see if content is in a domain folder which always uses linkit.aspx
                    string id;
                    if (strFormID != "")
                    {
                        id = strFormID;
                    }
                    else
                    {
                        id = strContentID;
                    }
                    domain = m_refContentApi.GetDomainByContentId(Convert.ToInt64(id));

                }

                if (strAssetName != "" && !m_refContentApi.RequestInformationRef.LinkManagement)
                {
                    sar.SearchText = strAssetName.Replace("\'", "\'");
                    sar.SearchType = EkEnumeration.SearchTypes.AndWords;
                }
                else if ((strAssetName != "") && (strLibtype != "images"))
                {
                    sar.SearchType = EkEnumeration.SearchTypes.OrWords; //AndWords
                    sar.SearchText = (string)("linkit.aspx?LinkIdentifier=id&amp;ItemId=" + strContentID + ",showcontent.aspx?id=" + strContentID);
                    //If searching for content linked to DMS image (from library)
                }
                else if ((strAssetName != "") && (strLibtype == "images"))
                {
                    sar.SearchType = EkEnumeration.SearchTypes.OrWords; //AndWords
                    sar.SearchText = strAssetName;
                }
                else if (strLibPath != "")
                {
                    if ((m_refContentApi.RequestInformationRef.LinkManagement || (domain != null)) && ((strLibtype != "images") && strLibtype != "files") && (strLibtype != "hyperlinks"))
                    {
                        //This is only for quicklink search
                        //if content is in a domain folder, linkit.aspx will redirect to the proper domain
                        if (strFormID != "")
                        {
                            sar.SearchText = (string)("linkit.aspx?LinkIdentifier=ekfrm&amp;ItemId=" + strFormID);
                        }
                        else
                        {
                            sar.SearchText = (string)("linkit.aspx?LinkIdentifier=id&amp;ItemId=" + strContentID);
                        }
                    }
                    else
                    {
                        sar.SearchText = strLibPath;
                    }

                    sar.SearchType = EkEnumeration.SearchTypes.AndWords;
                }
                else if ((m_refContentApi.RequestInformationRef.LinkManagement || (domain != null)) && (strFormID != ""))
                {
                    // if content is in a domain folder, linkit.aspx will redirect to the proper domain
                    sar.SearchText = (string)("linkit.aspx?LinkIdentifier=ekfrm&amp;ItemId=" + strFormID);
                    sar.SearchType = EkEnumeration.SearchTypes.AndWords;
                }
                else if (strContentID != "")
                {

                    _manualAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
                    _autoAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
                    page = new Ektron.Cms.PagingInfo();
                    long.TryParse(strContentID, out contentID);
                    manualAliasList = _manualAliasApi.GetList(page, contentID, true, EkEnumeration.UrlAliasingOrderBy.None);
                    autoAliasList = _autoAliasApi.GetListForContent(contentID);
                    tempURL = string.Empty;
                    for (index = 0; index <= manualAliasList.Count - 1; index++)
                    {
                        tempURL += (string)(manualAliasList[index].DisplayAlias + " ");
                    }
                    for (index = 0; index <= autoAliasList.Count - 1; index++)
                    {
                        tempURL += (string)(autoAliasList[index].AliasName + " ");
                    }

                    // We check the alias and add that as the or phrase.
                    sar.SearchText = (string)("id=" + strContentID);
                    if (tempURL != "" && domain != "")
                    {
                        sar.SearchText += (string)(" " + domain + "/" + tempURL);
                    }
                    else if (tempURL != "" && domain == "")
                    {
                        sar.SearchText += (string)(" " + m_refContentApi.SitePath + tempURL);
                    }
                    sar.SearchType = EkEnumeration.SearchTypes.OrWords;
                }
                else
                {
                    // Not sure if GetAllAliasedPageNameByCID needs form id or content id,
                    // but all we've got at this point is form id.
                    // We check the alias and add that as the or phrase.
                    _manualAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
                    _autoAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
                    page = new Ektron.Cms.PagingInfo();
                    long.TryParse(strContentID, out contentID);
                    manualAliasList = _manualAliasApi.GetList(page, contentID, true, EkEnumeration.UrlAliasingOrderBy.None);
                    autoAliasList = _autoAliasApi.GetListForContent(contentID);
                    tempURL = string.Empty;
                    for (index = 0; index <= manualAliasList.Count - 1; index++)
                    {
                        tempURL += (string)(manualAliasList[index].DisplayAlias + " ");
                    }
                    for (index = 0; index <= autoAliasList.Count - 1; index++)
                    {
                        tempURL += (string)(autoAliasList[index].AliasName + " ");
                    }
                    sar.SearchText = (string)("ekfrm=" + strFormID);
                    if (tempURL != "" && domain != "")
                    {
                        sar.SearchText += (string)(" " + domain + "/" + tempURL);
                    }
                    else if (tempURL != "" && domain == "")
                    {
                        sar.SearchText += (string)(" " + m_refContentApi.SitePath + tempURL);
                    }
                    sar.SearchType = EkEnumeration.SearchTypes.OrWords;
                }

            }
            else
            {
                sar = CustomFields.Populate_AssetRequestObjectFromForm(null);
            }

            sar.isWorkareaSearch = true;
            sar.ItemLanguageID = this.ContentLanguage;

            SearchContentItem[] sic;
            sar.CurrentPage = m_intCurrentPage;

            string cacheKey;
            if (sar.SearchText != "")
            {
                cacheKey = sar.SearchText + sar.SearchType.ToString() + sar.SearchAssets.ToString() + sar.Teaser_SearchText + sar.Title_SearchText + sar.FolderID.ToString();
            }
            else
            {
                cacheKey = (string)("Blank" + sar.SearchType.ToString() + sar.SearchAssets.ToString() + sar.Teaser_SearchText + sar.Title_SearchText + sar.FolderID.ToString());
            }
            if (!(Cache[cacheKey] == null))
            {
                sar.SearchResults = (SearchCacheData[])Cache[cacheKey];
            }

            //cms_LoadSearchResult stored proc doesn't return SearchAssetRequest.RecordsAffected value if we do not pass pagesize: Defect:46642
            if ((!(Request.Form[isPostData.UniqueID] == null)) || (m_strPageAction == "dofindcontent"))
            {
                sar.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
            }
            sic = ekc.SearchAssets(ref sar);

            if ((Cache[cacheKey] == null) && sar.SearchResults != null)
            {
                Cache.Add(cacheKey, sar.SearchResults, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero, CacheItemPriority.Normal, null);
            }

            m_intTotalPages = sar.TotalPages;
            m_intTotalRecords = sar.RecordsAffected;
            PageSettings();
            search_result = sic;

            StringBuilder strHiddenText = new StringBuilder();

            if (!((Request.Form[isPostData.UniqueID] == null)) && m_intTotalPages > 1)
            {
                foreach (object AvailableItem in Request.Form)
                {
                    if (AvailableItem.ToString().ToLower().IndexOf("ecm", 0, 3) != -1)
                    {
                        strHiddenText.Append("<input type=\"hidden\" id=\"" + AvailableItem + "\" name=\"" + AvailableItem + "\" value=\"" + Request.Form[AvailableItem.ToString()] + "\">");
                    }
                }
                HiddenData.Text = strHiddenText.ToString();
            }

            //Since viewstate is false for the page, ensure the active tab is set correctly.
            //mvSearch.ActiveViewIndex = Convert.ToInt32(hdnSelectedTab.Value);
            if (Convert.ToInt32(hdnSelectedTab.Value) == 0)
                uxSearchTabs.SetActiveTab(uxTabBasic);
            else
                uxSearchTabs.SetActiveTab(uxTabAdvanced);
            hmenuSelected.Value = hdnSelectedTab.Value;

            string strViewMode;
            //If there is no dropdown on the page ie. clicked on a page link in results,
            //use old value of dropdown from the hidden field pageMode
            //else use dropdown value and, update pageMode
            if (!(Request.Form["selDisplayMode"] == null))
            {
                strViewMode = (string)(Request.Form["selDisplayMode"].ToLower());
                pageMode.Value = strViewMode;
            }
            else
            {
                strViewMode = (string)pageMode.Value;
            }

            if ((strViewMode != null) && (strViewMode.CompareTo("graphical") == 0))
            {
                iconListOutputLit.Text = "&nbsp;&nbsp;Processing...";
                BuildGraphicalResults(sar);
            }
            else if ((strViewMode != null) && (strViewMode.CompareTo("mixed") == 0))
            {
                Populate_SearchResultGrid_Content_Mixed(sar);
            }
            else
            {
                // Default to text mode:
                Populate_SearchResultGrid_Content(sar);
            }

            if (strLibPath != "")
            {
                m_refContentApi.ContentLanguage = ContentLanguage;
            }
            //mvSearch.SetActiveView(vwSearchAdvanced);
            uxSearchTabs.SetActiveTab(uxTabAdvanced);  
        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->Postback_DoFindContent] " + ex.Message));
        }
    }
    private string GetLink(SearchContentItem item)
    {
        string returnValue;
        returnValue = null;
        if (item.ID > 0)
        {
            if (item.ContentType == EkEnumeration.CMSContentType.Forms)
            {
                returnValue = m_refContentApi.AppPath + "cmsform.aspx?action=ViewForm&LangType=" + item.LanguageID + "&form_id=" + item.ID + "&backpage=history";
            }
            else
            {
                returnValue = m_refContentApi.AppPath + "content.aspx?action=View&LangType=" + item.LanguageID + "&folder_id=" + item.FolderID + "&id=" + item.ID + "&backpage=history";
            }
        }
        return returnValue;
    }

    // Create HTML for Graphical (fractionally-zoomed iframes of URL) Content search results:
    private void BuildGraphicalResults(SearchAssetRequest sar)
    {
        long idx;
        string linkStr;
        string imageStr;
        System.Text.StringBuilder result;
        string firstPartStr = "";
        long itemsCount = 0;
        string dimStr;
        TemplateData templateDataObj;
        string templateStr;

        try
        {
            // obtain template to use for rendering into iframe:
            templateDataObj = m_refContentApi.GetTemplatesByFolderId(sar.FolderID);
            if (Convert.ToString(templateDataObj.FileName).IndexOf("?") >= 0)
            {
                templateStr = m_refContentApi.SitePath + templateDataObj.FileName + "&";
            }
            else
            {
                templateStr = m_refContentApi.SitePath + templateDataObj.FileName + "?";
            }


            SearchStyleSheet = "<link rel=\'stylesheet\' type=\'text/css\' href=\'csslib/worksearch.css\'>" + "\r\n";

            result = new System.Text.StringBuilder();
            result.Append("<script language=\"javascript\" src=\"java//jfunct.js\"></script>" + "\r\n");
            result.Append("<script language=\"javascript\" src=\"java//com.ektron.ui.mod_iconlist.js\"></script>" + "\r\n");
            result.Append("<script language=\"javascript\" src=\"java/com.ektron.utils.string.js\"></script>" + "\r\n");
            result.Append("<script language=\"javascript\" type=\"text/javascript\">" + "\r\n");
            result.Append("var g_searchResult;" + "\r\n");
            result.Append("var g_retryDisplayResultsTimer;" + "\r\n");
            result.Append("function displayResults( searchResult )" + "\r\n");
            result.Append("{" + "\r\n");
            result.Append("	if( searchResult ) {" + "\r\n");
            result.Append("		var resultContainer = document.getElementById( \'iconListOutput\' );" + "\r\n");
            result.Append("		if( resultContainer ) {" + "\r\n");
            result.Append("			var iconList = new IconList( searchResult.assets );" + "\r\n");
            result.Append("			iconList.setResultContainer( resultContainer );" + "\r\n");
            result.Append("			iconList.display();" + "\r\n");
            result.Append("		} else {" + "\r\n");
            result.Append("			g_searchResult = searchResult;" + "\r\n");
            result.Append("			if (g_retryDisplayResultsTimer){" + "\r\n");
            result.Append("				clearTimeout(g_retryDisplayResultsTimer);" + "\r\n");
            result.Append("			}" + "\r\n");
            result.Append("			setTimeout(retryDisplayResults, 500);" + "\r\n");
            result.Append("		}" + "\r\n");
            result.Append("	}" + "\r\n");
            result.Append("	document.body.style.cursor = \'default\';" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function retryDisplayResults(){" + "\r\n");
            result.Append("	if (g_searchResult){" + "\r\n");
            result.Append("		displayResults(g_searchResult);" + "\r\n");
            result.Append("	}" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function myAssets(){" + "\r\n");
            result.Append("	this.type = null;" + "\r\n");
            result.Append("	this.quickLink = null;" + "\r\n");
            result.Append("	this.imageLink = null;" + "\r\n");
            result.Append("	this.__previewurl = null;" + "\r\n");
            result.Append("	this.lastEditorFirstName = null;" + "\r\n");
            result.Append("	this.lastEditorLastName = null;" + "\r\n");
            result.Append("	this.dateModified = null;" + "\r\n");
            result.Append("	this.teaser = null;" + "\r\n");
            result.Append("	this.content = null;" + "\r\n");
            result.Append("	this.lastModified = null;" + "\r\n");
            result.Append("	this.editorFirstName = null;" + "\r\n");
            result.Append("	this.editorLastName = null;" + "\r\n");
            result.Append("	this.title = null;" + "\r\n");
            result.Append("	this.thumbnailUrl = null;" + "\r\n");
            result.Append("}" + "\r\n");
            if (search_result == null)
            {
                iconListOutputLit.Text = "";
            }
            else
            {
                result.Append("function goDisplay(){" + "\r\n");
                result.Append("	var testResult = new mySearchResult;" + "\r\n");
                result.Append("	displayResults(testResult);" + "\r\n");
                result.Append("}" + "\r\n");
                result.Append("function mySearchResult(){" + "\r\n");
                firstPartStr = result.ToString();
                result.Length = 0;
                for (idx = 0; idx <= search_result.Length - 1; idx++)
                {
                    if (search_result[idx].ContentType == EkEnumeration.CMSContentType.LibraryItem)
                    {
                        result.Append("this.assets[" + idx.ToString() + "] = new myAssets;" + "\r\n");
                        result.Append("this.assets[" + idx.ToString() + "].type = \'c\'; // if value is numerical, signals type is a document otherwise content." + "\r\n");
                        result.Append("this.assets[" + idx.ToString() + "].thumbnailUrl = \'\';" + "\r\n");

                        imageStr = m_refContentApi.AppPath + "/DisplayResult.aspx?libid=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history";
                        result.Append("this.assets[" + idx.ToString() + "].imageLink = " + PrepTextForJava(imageStr));
                        linkStr = m_refContentApi.AppPath + "library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history";
                        result.Append("this.assets[" + idx.ToString() + "].quickLink = " + PrepTextForJava(linkStr));
                        result.Append("this.assets[" + idx.ToString() + "].__previewurl = null;" + "\r\n");
                        result.Append("this.assets[" + idx.ToString() + "].lastEditorFirstName = " + PrepTextForJava((string)(search_result[idx].LastEditorFname)));
                        result.Append("this.assets[" + idx.ToString() + "].lastEditorLastName = " + PrepTextForJava(search_result[idx].LastEditorLname));
                        result.Append("this.assets[" + idx.ToString() + "].dateModified = " + PrepTextForJava((string)(search_result[idx].DisplayDateModified)));
                        result.Append("this.assets[" + idx.ToString() + "].teaser = " + PrepTextForJava(CleanText(search_result[idx].Teaser)));
                        result.Append("this.assets[" + idx.ToString() + "].content = \'\';" + "\r\n");
                        result.Append("this.assets[" + idx.ToString() + "].lastModified = \'\';" + "\r\n");
                        result.Append("this.assets[" + idx.ToString() + "].editorFirstName = " + PrepTextForJava(search_result[idx].LastEditorFname));
                        result.Append("this.assets[" + idx.ToString() + "].editorLastName = " + PrepTextForJava(search_result[idx].LastEditorLname));
                        result.Append("this.assets[" + idx.ToString() + "].title = " + PrepTextForJava(search_result[idx].LibraryTitle));
                        itemsCount++;
                    }
                    else
                    {
                        // Non library items (also filter out duplicates from the library):
                        if (search_result[idx].LibraryTypeID == 0)
                        {
                            result.Append("this.assets[" + idx.ToString() + "] = new myAssets;" + "\r\n");
                            if (EkConstants.IsAssetContentType(Convert.ToInt64(search_result[idx].ContentType), true) || (search_result[idx].ContentType == EkEnumeration.CMSContentType.Assets))
                            {
                                result.Append("this.assets[" + idx.ToString() + "].type = \'1\'; // value is numerical, signals type is a document." + "\r\n");
                                result.Append("this.assets[" + idx.ToString() + "].thumbnailUrl = " + PrepTextForJava(GetThumbnailUrl(search_result[idx].ContentText)));
                            }
                            else
                            {
                                result.Append("this.assets[" + idx.ToString() + "].type = \'c\'; // value non-numerical, signals type is content." + "\r\n");
                                result.Append("this.assets[" + idx.ToString() + "].thumbnailUrl = \'\';" + "\r\n");
                            }
                            //don't use standard quicklink, db may point to wrong template... result.Append("this.assets[" & idx.ToString & "].quickLink = '" & search_result(idx).QuickLink & "';" & vbCrLf)
                            linkStr = GetLink(search_result[idx]);
                            if (templateStr.Length > 0)
                            {
                                imageStr = templateStr + "id=" + search_result[idx].ID + "&folder=" + search_result[idx].FolderID;
                            }
                            else
                            {
                                imageStr = "";
                            }
                            result.Append("this.assets[" + idx.ToString() + "].quickLink = " + PrepTextForJava(linkStr));
                            result.Append("this.assets[" + idx.ToString() + "].imageLink = " + PrepTextForJava(imageStr));
                            result.Append("this.assets[" + idx.ToString() + "].__previewurl = null;" + "\r\n");
                            result.Append("this.assets[" + idx.ToString() + "].lastEditorFirstName = " + PrepTextForJava(search_result[idx].LastEditorFname));
                            result.Append("this.assets[" + idx.ToString() + "].lastEditorLastName = " + PrepTextForJava(search_result[idx].LastEditorLname));
                            result.Append("this.assets[" + idx.ToString() + "].dateModified = " + PrepTextForJava(search_result[idx].DisplayDateModified));
                            result.Append("this.assets[" + idx.ToString() + "].teaser = " + PrepTextForJava(CleanText(search_result[idx].Teaser)));
                            result.Append("this.assets[" + idx.ToString() + "].content = \'\';" + "\r\n");
                            result.Append("this.assets[" + idx.ToString() + "].lastModified = \'\';" + "\r\n");
                            result.Append("this.assets[" + idx.ToString() + "].editorFirstName = " + PrepTextForJava((string)(search_result[idx].LastEditorFname)));
                            result.Append("this.assets[" + idx.ToString() + "].editorLastName = " + PrepTextForJava((string)(search_result[idx].LastEditorLname)));
                            result.Append("this.assets[" + idx.ToString() + "].title = " + PrepTextForJava(search_result[idx].Title));
                            itemsCount++;
                        }
                    }
                }
                result.Append("}" + "\r\n");
                result.Append("goDisplay();");
            }
            result.Append("</script>");
            if (itemsCount > 0)
            {
                dimStr = "this.assets = new Array(" + itemsCount.ToString() + ");" + "\r\n";
                resultLit.Text = firstPartStr + dimStr + result.ToString();
            }
            else
            {
                resultLit.Text = m_refMsg.GetMessage("lbl search results");
            }
            PostBack_DoFindContentToolBar(Convert.ToInt32(itemsCount));


        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->BuildGraphicalResults] " + ex.Message));
        }
    }
    private string GetThumbnailUrl(string inStr)
    {
        string returnValue;
        string retStr = "";
        string fileExt = "";
        string startTag = "<FileExtension>";
        string endTag = "</FileExtension>";
        int startOffset;
        int endOffset;

        try
        {
            if (inStr.Length > 0)
            {
                startOffset = inStr.IndexOf(startTag);
                if (startOffset >= 0)
                {
                    startOffset += startTag.Length;
                    endOffset = inStr.IndexOf(endTag, startOffset);
                    if (endOffset > startOffset)
                    {
                        fileExt = inStr.Substring(startOffset, System.Convert.ToInt32(endOffset - startOffset));
                    }
                }
            }

            if ((string)(fileExt.ToLower()) == "doc")
            {
                retStr = "thumb_doc.gif";
            }
            else if ((string)(fileExt.ToLower()) == "dot")
            {
                retStr = "thumb_dot.gif";
            }
            else if ((string)(fileExt.ToLower()) == "gif")
            {
                retStr = "thumb_gif.gif";
            }
            else if ((string)(fileExt.ToLower()) == "bmp")
            {
                retStr = "thumb_bmp.gif";
            }
            else if ((string)(fileExt.ToLower()) == "jpeg")
            {
                retStr = "thumb_jpeg.gif";
            }
            else if ((string)(fileExt.ToLower()) == "jpg")
            {
                retStr = "thumb_jpg.gif";
            }
            else if ((string)(fileExt.ToLower()) == "log")
            {
                retStr = "thumb_log.gif";
            }
            else if ((string)(fileExt.ToLower()) == "pdf")
            {
                retStr = "thumb_pdf.gif";
            }
            else if ((string)(fileExt.ToLower()) == "ppt")
            {
                retStr = "thumb_ppt.gif";
            }
            else if ((string)(fileExt.ToLower()) == "txt")
            {
                retStr = "thumb_txt.gif";
            }
            else if ((string)(fileExt.ToLower()) == "vsd")
            {
                retStr = "thumb_vsd.gif";
            }
            else if ((string)(fileExt.ToLower()) == "word")
            {
                retStr = "thumb_word.gif";
            }
            else if ((string)(fileExt.ToLower()) == "xls")
            {
                retStr = "thumb_xls.gif";
            }
            else if ((string)(fileExt.ToLower()) == "zip")
            {
                retStr = "thumb_zip.gif";
            }
            else
            {
                retStr = "thumb_ektron.gif";
            }

            if (retStr.Length > 0)
            {
                retStr = AppImgPath + retStr;
            }

        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->GetThumbnailUrl] " + ex.Message));
        }
        finally
        {
            returnValue = retStr;
        }
        return returnValue;
    }
    private string GetImageUrl(string inStr)
    {
        string returnValue;
        string retStr = "";
        string startTag = "<ImageUrl>";
        string endTag = "</ImageUrl>";
        int startOffset;
        int endOffset;

        try
        {
            if (inStr.Length > 0)
            {
                startOffset = inStr.IndexOf(startTag);
                if (startOffset >= 0)
                {
                    startOffset += startTag.Length;
                    endOffset = inStr.IndexOf(endTag, startOffset);
                    if (endOffset > startOffset)
                    {
                        retStr = inStr.Substring(startOffset, System.Convert.ToInt32(endOffset - startOffset));
                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->GetImageUrl] " + ex.Message));
        }
        finally
        {
            returnValue = retStr;
        }
        return returnValue;
    }
    private string PrepTextForJava(string inText)
    {
        string retStr;

        retStr = " unescape(\'" + EkFunctions.HtmlEncode(inText.Replace("\'", "%27").Replace("&", "%26")) + "\');" + "\r\n";
        return retStr;
    }
    private string CleanText(string inText)
    {
        System.Text.StringBuilder result;
        int idx;
        char ch;

        result = new System.Text.StringBuilder();
        for (idx = 0; idx <= inText.Length - 1; idx++)
        {
            ch = inText[idx];
            if (!char.IsControl(ch))
            {
                result.Append(ch);
            }
            else
            {
                int dmycnt = 0;
                dmycnt++;
            }
        }
        return (StripHtmlTags(result.ToString()));
    }
    private string StripHtmlTags(string inText)
    {
        string retText;
        retText = Regex.Replace(inText, "<[^>]*>", "");
        return (retText);
    }

    // Display Mixed-Mode content search results:
    private void Populate_SearchResultGrid_Content_Mixed(SearchAssetRequest sar)
    {
        try
        {
            bool bIsDms;
            long itemsCount = 0;
            string strDestLoc = "";
            string strFileName = "";
            string strImageFile = "";
            string strNewImageFile = "";
            string strExtn = "";
            string[] arrFilePath;
            FileInfo fs = null;
            long contentId;
            DataTable dt = new DataTable();
            DataRow dr;
            int idx = 0;
            int iDR;


            bIsDms = false;//sar.SearchAssets;

            System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TITLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic title");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "LastEditDate";
            colBound.HeaderText = m_refMsg.GetMessage("lbl last edit date");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "VALUE";
            colBound.HeaderText = m_refMsg.GetMessage("lbl folder name");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            if (bIsDms)
            {
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "Size";
                colBound.HeaderText = m_refMsg.GetMessage("lbl size");
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.Wrap = false;
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.HeaderStyle.CssClass = "title-header";
                SearchResultGrid.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DMSRank";
                colBound.HeaderText = m_refMsg.GetMessage("lbl dms rank");
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.Wrap = false;
                colBound.HeaderStyle.CssClass = "title-header";
                SearchResultGrid.Columns.Add(colBound);
            }

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Language";
            colBound.HeaderText = m_refMsg.GetMessage("generic language");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Status";
            colBound.HeaderText = m_refMsg.GetMessage("generic status");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("LastEditDate", typeof(string)));
            dt.Columns.Add(new DataColumn("VALUE", typeof(string)));
            if (bIsDms)
            {
                dt.Columns.Add(new DataColumn("Size", typeof(string)));
                dt.Columns.Add(new DataColumn("DMSRank", typeof(string)));
            }
            dt.Columns.Add(new DataColumn("Language", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));

            if (!(search_result == null))
            {

                // first show all image-items:
                for (idx = 0; idx <= search_result.Length - 1; idx++)
                {
                    if (search_result[idx].ContentType == EkEnumeration.CMSContentType.LibraryItem)
                    {
                        if (search_result[idx].LibraryTypeID == 1)
                        {
                            dr = dt.NewRow();
                            strDestLoc = search_result[idx].LibraryFileName; // Link
                            strFileName = strDestLoc;
                            arrFilePath = strDestLoc.Split('/');
                            strImageFile = (string)(arrFilePath[arrFilePath.Length - 1]);

                            strExtn = strImageFile.Substring(strImageFile.Length - 3, 3);
                            if ("gif" == strExtn)
                            {
                                strExtn = "png";
                                strNewImageFile = "thumb_" + strImageFile.Substring(0, strImageFile.Length - 3) + "png";
                            }
                            else
                            {
                                strNewImageFile = (string)("thumb_" + strImageFile);
                            }

                            strDestLoc = strDestLoc.Replace(strImageFile, strNewImageFile);
                            fs = new FileInfo(Server.MapPath(strDestLoc));
                            if ((fs.Exists) == true)
                            {
                                dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history\" title=\"" + search_result[idx].LibraryTitle + "\">";
                                dr[0] += "<img src=\"" + strDestLoc + "\" border=\"0\" width=\"125\"></a>";
                            }
                            else
                            {
                                dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history\">" + search_result[idx].LibraryTitle + "</a>";
                            }
                            if (search_result[idx].ID > 0)
                            {
                                contentId = search_result[idx].ID;
                                if (contentId > 0)
                                {
                                    dr[1] = search_result[idx].Teaser;
                                }
                            }
                            dt.Rows.Add(dr);
                            itemsCount++;
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                    }
                }

                // now do non-images:
                for (idx = 0; idx <= search_result.Length - 1; idx++)
                {
                    if (search_result[idx].ContentType == EkEnumeration.CMSContentType.LibraryItem)
                    {
                        if (search_result[idx].LibraryTypeID != 1)
                        {
                            dr = dt.NewRow();
                            dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history\">" + search_result[idx].LibraryTitle + "</a>";
                            dr[1] = m_strLibType + "&nbsp;" + search_result[idx].LibraryFileName; //.Link
                            dt.Rows.Add(dr);
                            itemsCount++;
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        // filter out duplicates from the library:
                        if (search_result[idx].LibraryTypeID == 0)
                        {
                            dr = dt.NewRow();
                            string link = GetLink(search_result[idx]);
                            dr[0] = "<a href=\"" + link + "\">" + search_result[idx].Title + "</a>";
                            dr[1] = search_result[idx].DisplayDateModified;
                            dr[2] = search_result[idx].FolderPath;
                            if (bIsDms)
                            {
                                dr[3] = search_result[idx].Size;
                                dr[4] = search_result[idx].Rank;
                                iDR = 5;
                            }
                            else
                            {
                                iDR = 3;
                            }
                            dr[iDR] = search_result[idx].LanguageID;
                            iDR++;
                            dr[iDR] = search_result[idx].ItemStatus;
                            iDR++;
                            dt.Rows.Add(dr);
                            itemsCount++;
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                    }
                }

            }
            else
            {
                dr = dt.NewRow();
                dr[0] = m_refMsg.GetMessage("lbl search results");
                dr[1] = "REMOVE-ITEM";
                dr[2] = "REMOVE-ITEM";
                dr[3] = "REMOVE-ITEM";
                dr[4] = "REMOVE-ITEM";
                if (bIsDms)
                {
                    dr[5] = "REMOVE-ITEM";
                    dr[6] = "REMOVE-ITEM";
                }
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            SearchResultGrid.DataSource = dv;
            SearchResultGrid.DataBind();
            PostBack_DoFindContentToolBar(Convert.ToInt32(itemsCount));

        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->Populate_SearchResultGrid_Content_Mixed] " + ex.Message));
        }
    }
    private void PageSettings()
    {
        if (m_intTotalPages <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {

            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(m_intTotalPages))).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            hTotalPages.Value = (System.Math.Ceiling(Convert.ToDecimal(m_intTotalPages))).ToString();
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            hCurrentPage.Value = m_intCurrentPage.ToString();
            if (m_intCurrentPage == 1)
            {
                ctrlPreviousPage.Enabled = false;
                ctrlFirstPage.Enabled = false;
                if (m_intTotalPages > 1)
                {
                    ctrlNextPage.Enabled = true;
                }
                else
                {
                    ctrlNextPage.Enabled = false;
                }
            }
            else
            {
                ctrlPreviousPage.Enabled = true;
                if (m_intCurrentPage == m_intTotalPages)
                {
                    ctrlNextPage.Enabled = false;
                    ctrlLastPage.Enabled = false;
                }
                else
                {
                    ctrlNextPage.Enabled = true;
                }

            }
        }
    }
    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        ctrlPreviousPage.Visible = flag;
        ctrlNextPage.Visible = flag;
        ctrlLastPage.Visible = flag;
        ctrlFirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    // Display Text-Mode content search results:
    private void Populate_SearchResultGrid_Content(SearchAssetRequest sar)
    {
        try
        {
            bool bIsDms;
            long itemsCount = 0;
            DataTable dt = new DataTable();
            DataRow dr;
            int idx = 0;
            int iDR;

            string strDestLoc = "";
            string strFileName = "";
            string strImageFile = "";
            string strNewImageFile = "";
            string strExtn = "";
            string[] arrFilePath;

            long contentId;

            bIsDms = false;// sar.SearchAssets;

            System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TITLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic title");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "LastEditDate";
            colBound.HeaderText = m_refMsg.GetMessage("lbl last edit date");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "VALUE";
            colBound.HeaderText = m_refMsg.GetMessage("lbl folder name");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            if (bIsDms)
            {
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "Size";
                colBound.HeaderText = m_refMsg.GetMessage("lbl size");
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.Wrap = false;
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.HeaderStyle.CssClass = "title-header";
                SearchResultGrid.Columns.Add(colBound);

                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "DMSRank";
                colBound.HeaderText = m_refMsg.GetMessage("lbl dms rank");
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                colBound.ItemStyle.Wrap = false;
                colBound.HeaderStyle.CssClass = "title-header";
                SearchResultGrid.Columns.Add(colBound);
            }

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Language";
            colBound.HeaderText = m_refMsg.GetMessage("generic language");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Status";
            colBound.HeaderText = m_refMsg.GetMessage("generic status");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("LastEditDate", typeof(string)));
            dt.Columns.Add(new DataColumn("VALUE", typeof(string)));
            if (bIsDms)
            {
                dt.Columns.Add(new DataColumn("Size", typeof(string)));
                dt.Columns.Add(new DataColumn("DMSRank", typeof(string)));
            }
            dt.Columns.Add(new DataColumn("Language", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));

            if (!(search_result == null))
            {
                for (idx = 0; idx <= search_result.Length - 1; idx++)
                {
                    dr = dt.NewRow();
                    if (search_result[idx].ContentType == EkEnumeration.CMSContentType.LibraryItem)
                    {
                        if (search_result[idx].LibraryTypeID == 1)
                        {
                            strDestLoc = m_refContentApi.AppPath + search_result[idx].LibraryFileName; //.Link
                            strFileName = strDestLoc;
                            arrFilePath = strDestLoc.Split('/');
                            strImageFile = (string)(arrFilePath[arrFilePath.Length - 1]);

                            strExtn = strImageFile.Substring(strImageFile.Length - 3, 3);
                            if ("gif" == strExtn)
                            {
                                strExtn = "png";
                                strNewImageFile = "thumb_" + strImageFile.Substring(0, strImageFile.Length - 3) + "png";
                            }
                            else
                            {
                                strNewImageFile = (string)("thumb_" + strImageFile);
                            }

                            strDestLoc = strDestLoc.Replace(strImageFile, strNewImageFile);
                            dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history\">" + search_result[idx].LibraryTitle + "</a>";

                            if (search_result[idx].ID > 0)
                            {
                                contentId = search_result[idx].ID;
                                if (contentId > 0)
                                {
                                    dr[1] = search_result[idx].Teaser;
                                }
                            }
                            dt.Rows.Add(dr);
                            itemsCount++;
                        }
                        else
                        {
                            dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result[idx].LibraryID + "&parent_id=" + search_result[idx].LibraryParentID + "&backpage=history\">" + search_result[idx].LibraryTitle + "</a>";
                            dr[1] = m_strLibType + "&nbsp;" + search_result[idx].LibraryFileName; //.Link
                            dt.Rows.Add(dr);
                            itemsCount++;
                        }
                    }
                    else
                    {
                        // filter out duplicates from the library:
                        if (search_result[idx].LibraryTypeID == 0)
                        {
                            string link = GetLink(search_result[idx]);
                            dr[0] = "<a href=\"" + link + "\">" + search_result[idx].Title + "</a>";
                            dr[1] = search_result[idx].DisplayDateModified;
                            dr[2] = search_result[idx].FolderPath;
                            if (bIsDms)
                            {
                                dr[3] = search_result[idx].Size;
                                dr[4] = search_result[idx].Rank;
                                iDR = 5;
                            }
                            else
                            {
                                iDR = 3;
                            }
                            dr[iDR] = search_result[idx].LanguageID;
                            iDR++;
                            dr[iDR] = search_result[idx].ItemStatus;
                            iDR++;
                            dt.Rows.Add(dr);
                            itemsCount++;
                        }
                    }
                    dr = dt.NewRow();
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                dr = dt.NewRow();
                dr[0] = m_refMsg.GetMessage("lbl search results");
                dr[1] = "REMOVE-ITEM";
                dr[2] = "REMOVE-ITEM";
                dr[3] = "REMOVE-ITEM";
                dr[4] = "REMOVE-ITEM";
                if (bIsDms)
                {
                    dr[5] = "REMOVE-ITEM";
                    dr[6] = "REMOVE-ITEM";
                }
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            SearchResultGrid.DataSource = dv;
            SearchResultGrid.DataBind();
            PostBack_DoFindContentToolBar(m_intTotalRecords); //itemsCount
        }
        catch (Exception ex)
        {
            throw (new Exception("[iSearch.asx.vb->Populate_SearchResultGrid_Content] " + ex.Message));
        }
    }
    private void PostBack_DoFindContentToolBar(int ItemCount)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (Request.QueryString["content_id"] != null && Request.QueryString["content_id"] != "" && Request.QueryString["content_id"].ToString().Length > 0)
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(System.Convert.ToString(ItemCount + " content block(s) reference this item")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("lbl search found") + " " + ItemCount + " " + m_refMsg.GetMessage("alt result matched"))) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";
        }
        result.Append("<table><tr>");
        if (Request.QueryString["content_id"] != "")
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("LinkSearch", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    protected void SearchResultGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM"))
                {
                    e.Item.Cells[0].ColumnSpan = e.Item.Cells.Count;
                    e.Item.Cells[0].CssClass = "info";

                    int i;
                    for (i = e.Item.Cells.Count - 1; i >= 1; i--)
                    {
                        e.Item.Cells.RemoveAt(i);
                    }
                }
                else if (m_bLibrary)
                {
                    e.Item.Cells[0].Attributes.Add("id", arr_TdName[item]);
                    e.Item.Cells[1].Attributes.Add("id", (string)(arr_TdName[item] + "_0"));
                    e.Item.Cells[2].Attributes.Add("id", (string)(arr_TdName[item] + "_2"));
                    e.Item.Cells[3].Attributes.Add("id", (string)(arr_TdName[item] + "_1"));
                    item++;
                }
                break;
        }
    }
    private void PostBack_DoFindLibraryToolBar(SearchResultData[] search_result)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int Length = 0;
        if (!(search_result == null))
        {
            Length = search_result.Length;
        }
        if (Request.QueryString["content_id"] != null && Request.QueryString["content_id"] != "" && Request.QueryString["content_id"].ToString().Length > 0)
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage((string)(m_refMsg.GetMessage("lbl search found") + " " + Length + " " + m_refMsg.GetMessage("alt result matched")))) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("lbl search found") + " " + Length + " " + m_refMsg.GetMessage("alt result matched"))) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) + "\' />";
        }
        result.Append("<table><tr>");
        if ((Request["source"] == "edit") || (Request["source"] == "libinsert") || (Request["source"] == "mediainsert"))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_insert-nm.gif", "#", m_refMsg.GetMessage("alt add button text (library)"), m_refMsg.GetMessage("btn insert"), "onclick=\"SubmitInsert();return false;\"", StyleHelper.InsertButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_preview-nm.gif", "#", m_refMsg.GetMessage("alt preview button text (library)"), m_refMsg.GetMessage("btn preview"), "onclick=\"previewImage(\'\');return false;\"", StyleHelper.PreviewButtonCssClass));
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        //help button was not showing up
        result.Append("<td>" + m_refStyle.GetHelpButton("LinkSearch", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    // Pop-up-Window, display Library search results:
    private void Populate_SearchResultGrid_Library()
    {
        PermissionData security_info;
        security_info = m_refContentApi.LoadPermissions(m_intFolderId, "folder", 0);
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        m_bLibrary = true;
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("generic title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        SearchResultGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = m_refMsg.GetMessage("generic type");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        SearchResultGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = m_refMsg.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        SearchResultGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "NAME";
        colBound.HeaderText = m_refMsg.GetMessage("lbl file name");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        SearchResultGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        int i = 0;

        string strLibraryLink = "";
        string ImageOnClick = "";

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("NAME", typeof(string)));
        string TdName = "";
        int count = 0;
        string strFormID = "";
        strFormID = (string)((!(Request.QueryString["form_id"] == null)) ? (Request.QueryString["form_id"]) : "");
        i = 0;
        if (!(search_result_lib == null))
        {
            for (count = 0; count <= search_result_lib.Length - 1; count++)
            {
                try
                {
                    ImageOnClick = "";
                    TdName = "cell" + search_result_lib[count].LibraryId;
                    if (search_result_lib[count].TypeId == 1)
                    {
                        m_strLibType = "images";
                    }
                    else if (search_result_lib[count].TypeId == 2)
                    {
                        m_strLibType = "files";
                    }
                    else if (search_result_lib[count].TypeId == 3)
                    {
                        m_strLibType = "hyperlinks";
                    }
                    else if (search_result_lib[count].TypeId == 4)
                    {
                        m_strLibType = "Quicklinks";
                    }
                    else if (search_result_lib[count].TypeId == 5)
                    {
                        m_strLibType = "Form link";
                    }
                    else
                    {
                        m_strLibType = "unkown";
                    }
                    strLibraryLink = (string)(Convert.ToString(Server.HtmlDecode(search_result_lib[count].Link)).Replace("\'", "\\\'"));
                    if (m_strLibType == "images")
                    {
                        ImageOnClick = "ThumbnailForContentImage(\'" + EkFunctions.GetThumbnailForContent(strLibraryLink) + "\');";
                    }
                    if (m_strLibType == "Quicklinks" || m_strLibType == "Form link" || Request.QueryString["metadefinationtype"] == "")
                    {
                        if (this.m_refContentApi.RequestInformationRef.LinkManagement | caller == "editor" && (m_strLibType == "Quicklinks" || m_strLibType == "Form link"))
                        {
                            strLibraryLink = m_refContentApi.RequestInformationRef.ApplicationPath + "linkit.aspx?LinkIdentifier=" + (strFormID != "" ? "ekfrm" : "id") + "&amp;ItemID=" + search_result_lib[count].ContentId.ToString();
                        }
                        else
                        {
                            strLibraryLink = (string)(Convert.ToString(Server.HtmlDecode(search_result_lib[count].Link)).Replace("\'", "\\\'"));
                        }
                    }

                    search_result_lib[count].Title = Server.HtmlDecode(search_result_lib[count].Title);
                    Array.Resize(ref arr_TdName, i + 1);
                    arr_TdName[i] = TdName;
                    dr = dt.NewRow();
                    dr[0] = "<ILAYER name=\"layer" + TdName + "\">";
                    dr[0] += "<LAYER width=\"100%\">";
                    dr[0] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                    dr[0] += search_result_lib[count].Title;
                    dr[0] += "</a>";
                    dr[0] += "</LAYER>";
                    dr[0] += "</ILAYER>";


                    dr[1] = "<ILAYER name=\"layer" + TdName + "\">";
                    dr[1] += "<LAYER width=\"100%\">";
                    dr[1] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                    dr[1] += m_strLibType;
                    dr[1] += "</a>";
                    dr[1] += "</LAYER>";
                    dr[1] += "</ILAYER>";


                    dr[2] = "<ILAYER name=\"layer" + TdName + "\">";
                    dr[2] += "<LAYER width=\"100%\">";
                    dr[2] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                    dr[2] += search_result_lib[count].LibraryId.ToString();
                    dr[2] += "</a>";
                    dr[2] += "</LAYER>";
                    dr[2] += "</ILAYER>";

                    dr[3] = "<ILAYER name=\"layer" + TdName + "\">";
                    dr[3] += "<LAYER width=\"100%\">";
                    dr[3] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + strLibraryLink + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                    dr[3] += search_result_lib[count].Link;
                    dr[3] += "</a>";
                    dr[3] += "</LAYER>";
                    dr[3] += "</ILAYER>";

                    dt.Rows.Add(dr);
                    i++;
                    string strDestLoc = "";
                    string strFileName = "";
                    string strImageFile = "";
                    string strNewImageFile = "";
                    string strExtn = "";
                    string[] arrFilePath;
                    FileInfo fs = null;
                    if (m_strLibType == "images")
                    {
                        strDestLoc = search_result_lib[count].Link;
                        strFileName = strDestLoc;
                        arrFilePath = strDestLoc.Split('/');
                        strImageFile = (string)(arrFilePath[arrFilePath.Length - 1]);

                        strExtn = strImageFile.Substring(strImageFile.Length - 3, 3);
                        if ("gif" == strExtn)
                        {
                            strExtn = "png";
                            strNewImageFile = "thumb_" + strImageFile.Substring(0, strImageFile.Length - 3) + "png";
                        }
                        else
                        {
                            strNewImageFile = (string)("thumb_" + strImageFile);
                        }
                        strDestLoc = strDestLoc.Replace(strImageFile, strNewImageFile);
                        fs = new FileInfo(Server.MapPath(strDestLoc));
                        if ((fs.Exists) == true)
                        {
                            Array.Resize(ref arr_TdName, i + 1);
                            arr_TdName[i] = TdName;
                            dr = dt.NewRow();
                            if (Request.QueryString["scope"] == "")
                            {
                                dr[0] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + search_result_lib[count].ContentId + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + search_result_lib[count].ContentId + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                            }
                            else
                            {
                                dr[0] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"" + ImageOnClick + "Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + search_result_lib[count].Link + "\', \'" + m_strLibType + "\', " + search_result_lib[count].LibraryId + ");updateFolders(" + m_intFolderId + ", \'" + m_strLibType + "\'," + System.Convert.ToInt32(security_info.CanAddToImageLib) + "," + System.Convert.ToInt32(security_info.CanAddToFileLib) + "," + System.Convert.ToInt32(security_info.CanOverwriteLib) + ", \'" + search_result_lib[count].LibraryId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + search_result_lib[count].LibraryId + "\', \'" + m_intFolderId + "\', \'" + search_result_lib[count].Title.Replace("\'", "\\\'") + "\', \'" + search_result_lib[count].Link + "\', \'" + m_strLibType + "\', \'" + search_result_lib[count].ContentId + "\');SubmitInsert();\"" + " >";
                            }
                            dr[0] += "<img src=\"" + strDestLoc + "\" border=\"0\" width=\"125\"></a> ";
                            if (sEditor.ToLower() != "jseditor" && showThumbnail)
                            {
                                dr[0] += "<a href=\"#\" onclick=\"Insert_thumb(\'" + strFileName + "\',\'" + strDestLoc + "\')\" ><img src=\"images/application/thumbnail.gif\" border=\"0\" alt=\"Insert thumb nail and pop up larger image\"></a>";
                            }
                            dr[1] = "";
                            dr[2] = "";
                            dr[3] = "";
                            dt.Rows.Add(dr);
                            i++;
                        }
                    }
                }
                catch (Exception)
                {
                    // do nothing (silently eat unusable items for this release)...
                }
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl search results");
            dr[1] = "REMOVE-ITEM";
            dr[2] = "REMOVE-ITEM";
            dr[3] = "REMOVE-ITEM";
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        SearchResultGrid.DataSource = dv;
        SearchResultGrid.DataBind();
        item = 0;
        m_bLibrary = false;
    }

    // Workarea display Library search results:
    private void Populate_SearchResultGrid(bool btextMode)
    {
        PermissionData security_info;
        security_info = m_refContentApi.LoadPermissions(m_intFolderId, "folder", 0);

        DataTable dt = new DataTable();
        DataRow dr;
        int idx = 0;

        string strDestLoc = "";
        string strFileName = "";
        string strImageFile = "";
        string strNewImageFile = "";
        string strExtn = "";
        string[] arrFilePath;
        FileInfo fs = null;
        int contentId;

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        if (btextMode)
        {
            colBound.DataField = "TITLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic title");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TYPE";
            colBound.HeaderText = m_refMsg.GetMessage("generic type");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "NAME";
            colBound.HeaderText = m_refMsg.GetMessage("lbl link");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("NAME", typeof(string)));
        }
        else
        {
            colBound.DataField = "TITLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic title");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TEASER";
            colBound.HeaderText = m_refMsg.GetMessage("lbl Teaser");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            SearchResultGrid.Columns.Add(colBound);

            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("TEASER", typeof(string)));
        }

        if (!(search_result_lib == null))
        {
            for (idx = 0; idx <= search_result_lib.Length - 1; idx++)
            {

                if (search_result_lib[idx].TypeId == 1)
                {
                    m_strLibType = "images";
                }
                else if (search_result_lib[idx].TypeId == 2)
                {
                    m_strLibType = "files";
                }
                else if (search_result_lib[idx].TypeId == 3)
                {
                    m_strLibType = "hyperlinks";
                }
                else if (search_result_lib[idx].TypeId == 4)
                {
                    m_strLibType = "Quicklinks";
                }
                else if (search_result_lib[idx].TypeId == 5)
                {
                    m_strLibType = "Form link";
                }
                else
                {
                    m_strLibType = "unkown";
                }

                if (btextMode)
                {
                    dr = dt.NewRow();
                    dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result_lib[idx].LibraryId + "&parent_id=" + search_result_lib[idx].ParentId + "&backpage=history\">" + search_result_lib[idx].Title + "</a>";
                    dr[1] = m_strLibType;
                    dr[2] = "&nbsp;" + search_result_lib[idx].Link;
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dt.Rows.Add(dr);
                }
                else
                {
                    try
                    {

                        dr = dt.NewRow();
                        if (m_strLibType == "images")
                        {

                            strDestLoc = search_result_lib[idx].Link;
                            strFileName = strDestLoc;
                            arrFilePath = strDestLoc.Split('/');
                            strImageFile = (string)(arrFilePath[arrFilePath.Length - 1]);

                            strExtn = strImageFile.Substring(strImageFile.Length - 3, 3);
                            if ("gif" == strExtn)
                            {
                                strExtn = "png";
                                strNewImageFile = "thumb_" + strImageFile.Substring(0, strImageFile.Length - 3) + "png";
                            }
                            else
                            {
                                strNewImageFile = (string)("thumb_" + strImageFile);
                            }

                            strDestLoc = strDestLoc.Replace(strImageFile, strNewImageFile);
                            fs = new FileInfo(Server.MapPath(strDestLoc));
                            if ((fs.Exists) == true)
                            {
                                dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result_lib[idx].LibraryId + "&parent_id=" + search_result_lib[idx].ParentId + "&backpage=history\" title=\"" + search_result_lib[idx].Title + "\">";
                                dr[0] += "<img src=\"" + strDestLoc + "\" border=\"0\" width=\"125\"></a>";
                            }
                            else
                            {
                                dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result_lib[idx].LibraryId + "&parent_id=" + search_result_lib[idx].ParentId + "&backpage=history\">" + search_result_lib[idx].Title + "</a>";
                            }
                            if (search_result_lib[idx] != null)
                            {
                                contentId = Convert.ToInt32(search_result_lib[idx].ContentId);
                                if (contentId > 0)
                                {
                                    dr[1] = search_result_lib[idx].Teaser;
                                }
                            }
                            dt.Rows.Add(dr);
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            dr[0] = "<a href=\"library.aspx?action=ViewLibraryItem&id=" + search_result_lib[idx].LibraryId + "&parent_id=" + search_result_lib[idx].ParentId + "&backpage=history\">" + search_result_lib[idx].Title + "</a>";
                            dr[1] = m_strLibType + "&nbsp;" + search_result_lib[idx].Link;
                            dt.Rows.Add(dr);
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing (silently eat unusable items for this release)...
                    }
                }
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("lbl search results");
            dr[1] = "";
            if (btextMode)
            {
                dr[2] = "";
            }
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        SearchResultGrid.DataSource = dv;
        SearchResultGrid.DataBind();
    }

    // Create HTML for Graphical (fractionally-zoomed iframes of URL) Library search results:
    private void BuildGraphicalResults_WorkareaLibrary(SearchAssetRequest sar)
    {
        // Not implemented yet, throw error!
        throw (new Exception("[iSearch.asx.vb->BuildGraphicalResults] Functionality is Not Yet Supported!"));
    }
    private string ClientScript_IsExtensionValid(LibraryConfigData lib_setting_data)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("function IsExtensionValid(libType, filename) {" + "\r\n");

        result.Append("	if (libType == \"images\") {" + "\r\n");
        if (!(lib_setting_data == null))
        {
            result.Append("		var ExtensionList = \"" + lib_setting_data.ImageExtensions + "\";" + "\r\n");
        }
        else
        {
            result.Append("		var ExtensionList = \"\";" + "\r\n");
        }
        result.Append("	}" + "\r\n");
        result.Append("	else if (libType == \"files\") {" + "\r\n");
        if (!(lib_setting_data == null))
        {
            result.Append("		var ExtensionList = \"" + lib_setting_data.FileExtensions + "\";" + "\r\n");
        }
        else
        {
            result.Append("		var ExtensionList = \"\";" + "\r\n");
        }
        result.Append("	}" + "\r\n");
        result.Append("	else if (libType == \"all\") {" + "\r\n");
        if (!(lib_setting_data == null))
        {
            result.Append("		var ExtensionList = \"" + lib_setting_data.ImageExtensions + "," + lib_setting_data.FileExtensions + "\";" + "\r\n");
        }
        else
        {
            result.Append("		var ExtensionList = \";" + "\r\n");
        }
        result.Append("		alert(ExtensionList);" + "\r\n");
        result.Append("	}" + "\r\n");
        result.Append("	if (ExtensionList.length > 0) {" + "\r\n");
        result.Append("		var ExtensionArray = ExtensionList.split(\",\");" + "\r\n");
        result.Append("		var FileExtension = filename.split(\".\");" + "\r\n");
        result.Append("		for (var i = 0; i < ExtensionArray.length; i++) {" + "\r\n");
        result.Append("			if (FileExtension[FileExtension.length - 1].toLowerCase() == Trim(ExtensionArray[i].toLowerCase())) {" + "\r\n");
        result.Append("				return true;" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("		return false;" + "\r\n");
        result.Append("	}" + "\r\n");
        result.Append("}" + "\r\n");
        return (result.ToString());
    }

    #endregion

    #region JS, CSS
    private void RegisterPackages()
    {
        ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();

        // create a package that will register the UI JS and CSS we need
        Package searchResultsControlPackage = new Package()
        {
            Components = new List<Component>()
            {
                // Register JS Files
                Packages.Ektron.Workarea.Core,
                Packages.jQuery.jQueryUI.Core,
                Packages.Ektron.Xml,
                Packages.Ektron.StringObject,
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/internCalendarDisplayFuncs.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/searchfuncsupport.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/empjsfunc.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/toolbar_roll.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "ewebeditpro/eweputil.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/" + "java/workareahelper.js"),
                
                // Register CSS Files
                Css.Create(cmsContextService.WorkareaPath + "/" + "csslib/ektron.fixedPositionToolbar.css")
            }
        };
        searchResultsControlPackage.Register(this);
    }
    
    #endregion
}