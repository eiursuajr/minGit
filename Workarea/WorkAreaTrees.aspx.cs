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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Common;
using Microsoft.Security.Application;

public partial class WorkAreaTrees : System.Web.UI.Page
{
    protected ContentAPI m_refContentApi;
    protected EkMessageHelper m_refMsgApi;
    protected string m_selectedFolderList = "";
    protected string m_selectedTaxonomyList = "";
    protected string m_selectedMenuList = "";
    protected string m_selectedCollectionList = "";
    protected int ContentLanguage;
    protected CommonApi m_refApi = new CommonApi();
    protected int m_maxTreeTopNodes = 999999;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        if (m_refContentApi == null)
        {
            m_refContentApi = new ContentAPI();
        }

        Utilities.SetLanguage(m_refContentApi);
        ContentLanguage = m_refContentApi.ContentLanguage;
        contLanguage.Value = m_refContentApi.ContentLanguage.ToString();
        m_refMsgApi = m_refContentApi.EkMsgRef;
        
        if (!(Request.QueryString["method"] == null))
        {
            if (IsInCallbackMode())
            {
                return;
            }
        }

        if (!(Request.QueryString["AutoNav"] == null))
        {
            if (Request.QueryString["Tree"] == "Tax")
                m_selectedTaxonomyList = GetIdList(Request.QueryString["AutoNav"]);
            else if (Request.QueryString["Tree"] == "Menu")
                m_selectedMenuList = GetIdList(Request.QueryString["AutoNav"]);
            else if (Request.QueryString["Tree"] == "Coll")
                m_selectedCollectionList = GetIdList(Request.QueryString["AutoNav"]);
            else
                m_selectedFolderList = GetFolderList(Request.QueryString["AutoNav"]);
        }

        plContentTrees.Visible = Request.QueryString["Tree"] == "Content" || Request.QueryString["Tree"] == "Tax" || Request.QueryString["Tree"] == "Menu" || Request.QueryString["Tree"] == "Coll";

        // hide tree if user doesn't have enough privileges
        plTaxonomyTree.Visible = (Permissiondata.IsAdmin || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator));
        plCollectionTree.Visible = CanDoCollections;
        plMenuTree.Visible = CanDoMenus;

        string visibletree = string.Empty;
        if (!string.IsNullOrEmpty(Request.QueryString["TreeVisible"]))
            visibletree = (string)(Request.QueryString["TreeVisible"].ToLower());

        if (visibletree == "menu")
        {
            szAccordionIndex.Text = "3";
        }
        else if (visibletree == "collection" || visibletree == "coll")
        {
            szAccordionIndex.Text = "2";
        }
        else if (visibletree == "taxonomy")
        {
            szAccordionIndex.Text = "1";
        }
        else
        {
            szAccordionIndex.Text = "0";
        }

        AssignTextValues();
        RegisterResources();
    }

    private PermissionData _PermissionData;
    private PermissionData Permissiondata {
        get { return (_PermissionData ?? (_PermissionData = m_refContentApi.LoadPermissions(0, "content", 0))); }
    }

    private bool? _canDoCollections;
    private bool CanDoCollections {
        get {
            return (_canDoCollections ?? (bool)(_canDoCollections = (Permissiondata.IsAdmin
            || Permissiondata.IsCollections
            || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection)
            || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers)))); 
        }
    }

    private bool? _canDoMenus;
    private bool CanDoMenus {
        get {
            return (_canDoMenus ?? (bool)(_canDoMenus = (Permissiondata.IsAdmin
                || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
                || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu))));
        }
    }

    private bool? _isFolderAdmin;
    private bool IsFolderAdmin {
        get { return (_isFolderAdmin ?? (bool)(_isFolderAdmin = m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(0, 0, false))); }
    }

    private string GetFolderList(string FolderPath)
    {
        string result = "0";
        Ektron.Cms.Content.EkContent contObj = m_refContentApi.EkContentRef;
        long folderId;
        if (FolderPath.Length > 0)
        {
            folderId = contObj.GetFolderID(FolderPath);
            if (folderId >= 0)
            {
                result = contObj.GetFolderParentFolderIdRecursive(folderId);
            }
        }

        contObj = null;
        return result;
    }

    private string GetIdList(string idPath)
    {
        string result = "";
        if (!string.IsNullOrEmpty(idPath))
        {
            List<string> idList = new List<string>(idPath.Trim().Split(new char[] { '/' }));
            result = string.Join(",", idList.FindAll(x => !string.IsNullOrEmpty(x)).ToArray());
        }
        return result;
    }

    private bool IsInCallbackMode()
    {
        if (!(Request.QueryString["method"] == null))
        {
            //The following line added to support response to read as responseXml
            //Remove this if you want to read responseText
            Response.ContentType = "text/xml";
            Response.Write(RaiseCallbackEvent());
            Response.Flush();
            Response.End();
            return true;
        }

        return false;
    }

    // ****************************************************************************************************
    // Implement the callback interface
    private string RaiseCallbackEvent()
    {
        string result = "";
        FolderData[] folder_arr_data;
        FolderData folder_data;
        long m_intId;
        try
        {
            // handle language switch if needed
            int LangId = -1;
            if ((Request.Params["langid"] != null) && Information.IsNumeric(Request.Params["langid"]))
            {
                LangId = Convert.ToInt32(Request.Params["langid"]);
                if (LangId != -99)
                {
                    if (LangId > 0)
                    {
                        m_refContentApi.SetCookieValue("LastValidLanguageID", LangId.ToString());
                    }

                    m_refContentApi.ContentLanguage = LangId;
                    m_refApi.ContentLanguage = LangId;
                    //m_refApi.EkContentRef.RequestInformation.ContentLanguage = LangId
                }
            }

            if (!string.IsNullOrEmpty(Request.QueryString["method"]))
            {
                if (Request.QueryString["method"].ToLower() == "get_folder")
                {
                    m_intId = Convert.ToInt64(Request.Params["id"]);
                    folder_data = m_refContentApi.GetFolderDataWithPermission(m_intId); //GetFolderById(m_intId)
                    folder_data.XmlConfiguration = null;
                    result = SerializeAsXmlData(folder_data, folder_data.GetType());
                }
                else if (Request.QueryString["method"].ToLower() == "get_child_folders")
                {
                    m_intId = Convert.ToInt64(Request.Params["folderid"]);
                    folder_arr_data = m_refContentApi.GetChildFolders(m_intId, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
                    //when there are no folders in the content tree like CMS400min
                    if (folder_arr_data != null)
                    {
                        result = SerializeAsXmlData(folder_arr_data, folder_arr_data.GetType());
                    }
                }
                else if (Request.QueryString["method"].ToLower() == "get_child_category")
                {
                    Ektron.Cms.Content.EkContent m_refContent;
                    long TaxFolderId = -1;
                    if (Request.Params["folderid"] != null)
                    {
                        TaxFolderId = Convert.ToInt64(Request.Params["folderid"]);
                    }
                    long TaxOverrideId = -1;
                    if (Request.Params["taxonomyoverrideid"] != null)
                    {
                        TaxOverrideId = Convert.ToInt64(Request.Params["taxonomyoverrideid"]);
                    }

                    long TaxLangId = -99;
                    if (Request.Params["langid"] != null)
                    {
                        if (Request.Params["langid"] != "undefined")
                        {
                            TaxLangId = Convert.ToInt64(Request.Params["langid"]);
                        }
                    }

                    TaxonomyRequest taxonomy_request = new TaxonomyRequest();
                    TaxonomyBaseData[] taxonomy_data_arr = null;
                    Utilities.SetLanguage(m_refContentApi);
                    m_refContent = m_refContentApi.EkContentRef;
                    if (TaxFolderId == -2)
                    {
                        taxonomy_data_arr = m_refContent.GetAllTaxonomyByConfig(Ektron.Cms.Common.EkEnumeration.TaxonomyType.Group);
                    }
                    else
                    {
                        m_intId = Convert.ToInt64(Request.Params["taxonomyid"]);
                        taxonomy_request.TaxonomyId = m_intId;
                        if ((TaxFolderId > -1) && (TaxOverrideId <= 0) && (m_intId == 0))
                        {
                            taxonomy_data_arr = m_refContent.GetAllFolderTaxonomy(TaxFolderId);
                        }
                        else
                        {
                            if (TaxLangId != -99)
                            {
                                taxonomy_request.TaxonomyLanguage = Convert.ToInt32(TaxLangId);
                            }
                            else if (m_refContentApi.ContentLanguage == -1)
                            {
                                taxonomy_request.TaxonomyLanguage = m_refContentApi.DefaultContentLanguage;
                            }
                            else
                            {
                                taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
                            }
                            taxonomy_request.PageSize = m_maxTreeTopNodes; // default of 0 used to mean "everything" but storedproc changed
                            if (TaxFolderId == -3)
                            {
                                taxonomy_request.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Locale;
                            }
                            taxonomy_data_arr = m_refContent.ReadAllSubCategories(taxonomy_request);
                        }
                    }
                    result = SerializeAsXmlData(taxonomy_data_arr, taxonomy_data_arr.GetType());
                }
                else if (Request.QueryString["method"].ToLower() == "get_taxonomy")
                {
                    if (Request.Params["taxonomyid"] != "")
                    {
                        Ektron.Cms.Content.EkContent m_refContent;
                        TaxonomyRequest taxonomy_request = new TaxonomyRequest();
                        m_intId = Convert.ToInt64(Request.Params["taxonomyid"]);
                        Utilities.SetLanguage(m_refContentApi);
                        m_refContent = m_refContentApi.EkContentRef;
                        taxonomy_request.TaxonomyId = m_intId;
                        taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
                        TaxonomyData taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
                        if (taxonomy_data != null)
                        {
                            result = SerializeAsXmlData(taxonomy_data, taxonomy_data.GetType());
                        }
                    }
                }
                else if (Request.QueryString["method"].ToLower() == "get_taxonomies")
                {
                    TaxonomyRequest request = new TaxonomyRequest();
                    request.TaxonomyId = 0;
                    Utilities.SetLanguage(m_refContentApi);
                    request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
                    if (m_refContentApi.ContentLanguage == -1)
                    {
                        request.TaxonomyLanguage = m_refContentApi.DefaultContentLanguage;
                    }
                    request.PageSize = m_maxTreeTopNodes;
                    request.CurrentPage = 1;
                    TaxonomyBaseData[] taxonomy_data = m_refApi.EkContentRef.ReadAllSubCategories(request);
                    result = SerializeAsXmlData(taxonomy_data, taxonomy_data.GetType());
                }
                else if (Request.QueryString["method"].ToLower() == "get_collections")
                {
                    PageRequestData request = new PageRequestData();
                    request.PageSize = m_maxTreeTopNodes;
                    request.CurrentPage = 1;
                    CollectionListData[] collection_list = m_refApi.EkContentRef.GetCollectionList("", ref request);
                    result = SerializeAsXmlData(collection_list, collection_list.GetType());
                }
                else if (Request.QueryString["method"].ToLower() == "get_menus")
                {
                    PageRequestData request = new PageRequestData();
                    request.PageSize = m_maxTreeTopNodes;
                    request.CurrentPage = 1;
                    Collection menus = m_refApi.EkContentRef.GetMenuReport("", ref request);
                    List<AxMenuData> menuList = GetMenuList(menus);
                    result = SerializeAsXmlData(menuList, menuList.GetType());
                    if (m_refContentApi.ContentLanguage == -1)
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(result);
                        XmlNodeList nodes = xdoc.SelectNodes("//HasChildren");
                        foreach (XmlNode node in nodes)
                        {
                            node.InnerText = "false";
                        }
                        result = xdoc.InnerXml; 
                    }
                }
                else if (Request.QueryString["method"].ToLower() == "get_submenus")
                {
                    m_intId = Convert.ToInt64(Request.Params["menuid"]);
                    List<AxMenuData> items = GetSubmenuList(m_intId);
                    result = SerializeAsXmlData(items, items.GetType());
                }
            }
        }
        catch (Exception ex)
        {
            EkException.LogException(ex);
            result = "";
        }

        return result;
    }

    private List<AxMenuData> GetSubmenuList(long id)
    {
        AxMenuData item = m_refApi.EkContentRef.GetMenuDataByID(id);
        List<AxMenuData> menuList = new List<AxMenuData>();
        AxMenuData menuData;
        foreach (AxMenuItemData subitem in item.Item)
        {
            if (subitem != null && subitem.ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
            {
                menuData = new AxMenuData();
                menuData.ID = subitem.ID;
                menuData.FolderID = subitem.FolderID;
                menuData.Title = subitem.ItemTitle;
                menuData.Description = "";
                AxMenuData submenuMenus = m_refApi.EkContentRef.GetMenuDataByID(subitem.ID);
                menuData.HasChildren = false;
                foreach (AxMenuItemData submenuMenu in submenuMenus.Item)
                {
                    if (submenuMenu != null && submenuMenu.ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                    {
                        menuData.HasChildren = true;

                    }
                }

                menuData.ContentLanguage = subitem.ContentLanguage;
                menuData.ParentID = m_refApi.EkContentRef.GetParentIdByFolderId(subitem.FolderID);
                menuData.AncestorID = subitem.AncestorID;
                menuData.ItemCount = 1;
                menuData.Type = Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Menu;
                menuList.Add(menuData);
            }
        }

        return menuList;
    }

    private List<AxMenuData> GetMenuList(Collection menus)
    {
        List<AxMenuData> menuList = new List<AxMenuData>();
        AxMenuData menuData;
        //for (int i = 1; i <= menus.Count; i++)
        foreach (Collection temp_varMenus in menus)
        {
            menuData = new AxMenuData();
            menuData.ID = Convert.ToInt64(temp_varMenus["MenuId"]);
            menuData.FolderID = Convert.ToInt64(temp_varMenus["FolderId"]);
            menuData.Title = (string)(temp_varMenus["MenuTitle"]);
            menuData.Description = (string)(temp_varMenus["MenuDescription"]);
            menuData.HasChildren = System.Convert.ToBoolean(temp_varMenus["HasChildren"]);
            menuData.ContentLanguage = Convert.ToInt64(temp_varMenus["ContentLanguage"]);
            menuData.ParentID = Convert.ToInt64(temp_varMenus["ParentID"]);
            menuData.ItemCount = Convert.ToInt64(temp_varMenus["ItemCount"]);
            menuData.AncestorID = Convert.ToInt64(temp_varMenus["AncestorID"]);
            menuData.Type = Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Menu;
            menuList.Add(menuData);
        }

        return menuList;
    }

    private string SerializeAsXmlData(object data, Type datatype)
    {
        string result = "";
        System.IO.MemoryStream XmlOutStream = new System.IO.MemoryStream();
        XmlSerializer XmlSer;
        byte[] byteArr;
        System.Text.UTF8Encoding Utf8 = new System.Text.UTF8Encoding();
        XmlSer = new XmlSerializer(datatype);
        XmlSer.Serialize(XmlOutStream, data);
        byteArr = XmlOutStream.ToArray();
        result = System.Convert.ToBase64String(byteArr, 0, byteArr.Length);
        result = Utf8.GetString(Convert.FromBase64String(result));
        return result;
    }

    protected void AssignTextValues()
    {
        // assign the various resource text strings
        frameName.Text = EkFunctions.HtmlEncode(Request.QueryString["Tree"]);
        selectedFolderList.Text = m_selectedFolderList;
        selectedTaxonomyList.Text = m_selectedTaxonomyList;
        selectedMenuList.Text = m_selectedMenuList;
        selectedCollectionList.Text = m_selectedCollectionList;
        genericLibraryTitle.Text = m_refMsgApi.GetMessage("generic library title");
        genericContentTitle.Text = m_refMsgApi.GetMessage("generic content title");
        labelTaxonomies.Text = m_refMsgApi.GetMessage("lbl taxonomies");
        genericCollectionName.Text = m_refMsgApi.GetMessage("generic collection title");
        genericMenuTitle.Text = m_refMsgApi.GetMessage("generic menu title");

        // folder context menu strings
        folderContextAddFolder.Text = m_refMsgApi.GetMessage("btn add folder");
        folderContextAddBlogFolder.Text = m_refMsgApi.GetMessage("btn add blog");
        folderContextAddDiscussionBoard.Text = m_refMsgApi.GetMessage("add discussion board");
        folderContextAddCommunityFolder.Text = m_refMsgApi.GetMessage("add community folder");
        folderContextAddCalendarFolder.Text = m_refMsgApi.GetMessage("add calendar folder");
        folderContextAddEcommerceFolder.Text = m_refMsgApi.GetMessage("btn add catalog");
        folderContextAddSiteFolder.Text = m_refMsgApi.GetMessage("add site");
        folderContextPasteContent.Text = m_refMsgApi.GetMessage("lbl paste content");
        folderContextViewProperties.Text = m_refMsgApi.GetMessage("DmsMenuViewProperties");
        folderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        folderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        folderContextPasteFolder.Text = m_refMsgApi.GetMessage("paste folder");
        folderContextDeleteFolder.Text = string.Format(m_refMsgApi.GetMessage("delete x"), "<span class=\'triggerName\'></span>");
        folderContextDeleteFolderContent.Text = string.Format(m_refMsgApi.GetMessage("delete content from x"), "<span class=\'triggerName\'></span>");

        // siteFolder context menu strings
        siteFolderContextAddFolder.Text = folderContextAddFolder.Text;
        siteFolderContextAddBlogFolder.Text = folderContextAddBlogFolder.Text;
        siteFolderContextAddDiscussionBoard.Text = folderContextAddDiscussionBoard.Text;
        siteFolderContextAddCommunityFolder.Text = folderContextAddCommunityFolder.Text;
        siteFolderContextAddEcommerceFolder.Text = folderContextAddEcommerceFolder.Text;
        siteFolderContextViewProperties.Text = folderContextViewProperties.Text;
        siteFolderContextDeleteFolder.Text = folderContextDeleteFolder.Text;
        siteFolderContextDeleteFolderContent.Text = folderContextDeleteFolderContent.Text;
        siteFolderContextAddCalendarFolder.Text = folderContextAddCalendarFolder.Text;
        siteFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        siteFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        siteFolderContextPasteFolder.Text = folderContextPasteFolder.Text;
        siteFolderContextPasteContent.Text = folderContextPasteContent.Text;

        // blog context menu strings
        blogFolderContextViewProperties.Text = folderContextViewProperties.Text;
        blogFolderContextDeleteBlog.Text = folderContextDeleteFolder.Text;
        blogFolderContextDeleteBlogPosts.Text = string.Format(m_refMsgApi.GetMessage("delete posts from x"), "<span class=\'triggerName\'></span>");
        blogFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        blogFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        blogFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        //community context menu strings
        communityFolderContextAddBlog.Text = folderContextAddBlogFolder.Text;
        communityFolderContextAddBoard.Text = folderContextAddDiscussionBoard.Text;
        communityFolderContextAddCommunityFolder.Text = folderContextAddCommunityFolder.Text;
        communityFolderContextAddEcommerceFolder.Text = folderContextAddEcommerceFolder.Text;
        communityFolderContextPasteContent.Text = folderContextPasteContent.Text;
        communityFolderContextViewProperties.Text = folderContextViewProperties.Text;
        communityFolderContextDeleteFolder.Text = folderContextDeleteFolder.Text;
        communityFolderContextDeleteFolderContent.Text = folderContextDeleteFolderContent.Text;
        communityFolderContextAddCalendarFolder.Text = folderContextAddCalendarFolder.Text;
        communityFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        communityFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        communityFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        // discussion board menu strings
        boardFolderContextAddDiscussionForum.Text = m_refMsgApi.GetMessage("lbl add discussion forum");
        boardFolderContextAddSubject.Text = m_refMsgApi.GetMessage("lnk add new subject");
        boardFolderContextViewProperties.Text = folderContextViewProperties.Text;
        boardFolderContextDeleteBoard.Text = folderContextDeleteFolder.Text;
        boardFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        boardFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        boardFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        // discussion forum menu strings
        forumFolderContextAddTopic.Text = m_refMsgApi.GetMessage("add topic");
        forumFolderContextViewPermissions.Text = m_refMsgApi.GetMessage("btn view permissions");
        forumFolderContextViewProperties.Text = folderContextViewProperties.Text;
        forumFolderContextDeleteForum.Text = folderContextDeleteFolder.Text;
        forumFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        forumFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        forumFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        // ecommerce folder menu strings
        ecommerceContentAddFolder.Text = m_refMsgApi.GetMessage("btn add catalog");
        ecommercePasteCatalogEntry.Text = m_refMsgApi.GetMessage("lbl paste catalog entry");
        ecommerceContentViewProperties.Text = folderContextViewProperties.Text;
        ecommerceContentDeleteFolder.Text = folderContextDeleteFolder.Text;
        ecommerceContextDeleteContent.Text = string.Format(m_refMsgApi.GetMessage("delete entries from x"), "<span class=\'triggerName\'></span>");
        ecommerceFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        ecommerceFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        ecommerceFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        // calendar folder menu strings
        calendarViewProperties.Text = folderContextViewProperties.Text;
        calendarDeleteFolder.Text = folderContextDeleteFolder.Text;
        calendarFolderContextCutFolder.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        calendarFolderContextCopyFolder.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        calendarFolderContextPasteFolder.Text = folderContextPasteFolder.Text;

        // taxonomy menu strings
        taxonomyAdd.Text = m_refMsgApi.GetMessage("btn add category");
        taxonomyContextView.Text = folderContextViewProperties.Text;
        taxonomyAddContent.Text = m_refMsgApi.GetMessage("lbl assign items");
        taxonomyAssign.Text = m_refMsgApi.GetMessage("lbl assign selected items");
        taxonomyAddFolder.Text = m_refMsgApi.GetMessage("lbl assign folders");
        taxonomyContextCut.Text = string.Format(m_refMsgApi.GetMessage("cut folder"), "<span class=\'triggerName\'></span>");
        taxonomyContextCopy.Text = string.Format(m_refMsgApi.GetMessage("copy folder"), "<span class=\'triggerName\'></span>");
        taxonomyContextPaste.Text = m_refMsgApi.GetMessage("paste taxonomy");
        taxonomyContextDelete.Text = string.Format(m_refMsgApi.GetMessage("delete x"), "<span class=\'triggerName\'></span>");

        // collections menu strings
        collectionContextAddCollection.Text = m_refMsgApi.GetMessage("add collection title");
        collectionContextAdd.Text = m_refMsgApi.GetMessage("add collection items");
        collectionContextRemove.Text = m_refMsgApi.GetMessage("remove collection items");
        collectionContextReorder.Text = m_refMsgApi.GetMessage("reorder menu item title");
        collectionContextView.Text = folderContextViewProperties.Text;
        collectionContextDelete.Text = folderContextDeleteFolder.Text;
        collectionContextAssignSelectedItems.Text = m_refMsgApi.GetMessage("lbl assign selected items collection");

        // menu menu strings
        menuAdd.Text = m_refMsgApi.GetMessage("add menu title");
        menuContentAdd.Text = collectionContextAdd.Text;
        menuRemoveItems.Text = collectionContextRemove.Text;
        menuContextReorder.Text = collectionContextReorder.Text;
        menuContextView.Text = folderContextViewProperties.Text;
        menuContextDelete.Text = folderContextDeleteFolder.Text;
        menuContextAssignSelectedItems.Text = m_refMsgApi.GetMessage("lbl assign selected items menu");

        jsAppPath.Text = m_refApi.AppPath.ToString();
        jsConfirmFolderDelete.Text = m_refMsgApi.GetMessage("js contextmenu confirm delete folder");
        jsConfirmCollectionDelete.Text = m_refMsgApi.GetMessage("js: confirm collection deletion msg");
        jsConfirmMenuDelete.Text = m_refMsgApi.GetMessage("delete menu confirm");
        jsConfirmBreakInheritance.Text = m_refMsgApi.GetMessage("js: confirm want to break inheritance");
        jsConfirmTaxonomyDelete.Text = m_refMsgApi.GetMessage("delete taxonomy confirm");
    }
    protected void RegisterResources()
    {
        string AppPath = m_refApi.AppPath.ToString();

        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIEffectsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIAccordionJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronContextMenuJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaContextMenusJS);
        Ektron.Cms.API.JS.RegisterJS(this, "java/ektron.workarea.contextmenus.trees.js", "EktronWorkareaContextMenusTreesJS");
        Ektron.Cms.API.JS.RegisterJS(this, "controls/permission/permissionsCheckHandler.ashx?action=getPermissionJsClass", "EktronPermissionJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.explorer.init.js", "EktronExplorerInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.cms.types.js", "EktronCmsTypesJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.cms.parser.js", "EktronCmsParserJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.cms.toolkit.js", "EktronCmsToolkitJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.cms.api.js", "EktronCmsApiJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.ui.explore.js", "EktronUIExploreJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.ui.contextmenu.js", "EktronUIContextMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.ui.tree.js", "EktronUITreeJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.net.http.js", "EktronNetHttpJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.utils.log.js", "EktronUtilsLogJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.utils.dom.js", "EktronUtilsDomJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.utils.debug.js", "EktronUtilsDebugJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "Tree/js/com.ektron.utils.string.js", "EktronUtilsStringJS");
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/ektron.workareatrees.js", "EktronWorkareaTreesJS");

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, AppPath + "Tree/css/com.ektron.ui.tree.css", "EktronUITreesCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronContextMenuCss);
    }
}

