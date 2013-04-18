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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Content;

public partial class Collections : System.Web.UI.Page
{
    protected string ContentIcon = "";
    protected string WebEventIcon = "";
    protected string pageIcon = "";
    protected string formsIcon = "";
    protected string AppImgPath = "";
    protected long FolderId = 0;
    protected string actName = "AddLink";
    protected string notSupportIFrame = "";
    protected string AppName = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected bool CanCreateContent = false;
    protected PermissionData perm_data;
    protected EkMessageHelper m_refMsg;
    protected Collection gtNavs;
    protected Collection cTmp;
    protected Collection cFolders;
    protected object FolderName;
    protected object ParentFolderId;
    protected string fPath;
    protected EkEnumeration.FolderType folderType = EkEnumeration.FolderType.Content;
    protected EkContent m_refContent;
    protected string AddType = "";
    protected EkContentCol cConts;
    protected Collection CollectionContentItems;
    protected long nId;
    protected long subfolderid;
    protected long locID;
    protected int g_ContentTypeSelected = -1;
    protected Collection cRecursive;
    protected bool rec;
    protected string MenuTitle;
    protected string CollectionTitle;
    protected AssetInfoData[] asset_data;
    protected int lContentType;
    protected int ContentLanguage;
    protected string NoWorkAreaAttribute;
    protected long backId;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected System.Text.StringBuilder result = new System.Text.StringBuilder();
    protected int count;
    protected long lAddMultiType;
    protected bool bSelectedFound;
    protected string status = "";
    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected int CONTENT_LANGUAGES_UNDEFINED = 0;
    protected object gtMsgObj;
    protected object gtMess;
    protected object CollectionID;
    protected object msgs;
    protected object currentUserID;
    protected object PageTitle;
    protected object AppPath;
    protected object sitePath;
    protected object gtNav;
    protected object lLoop;
    protected object siteObj;
    protected Hashtable cPerms;
    protected object cLinkArray;
    protected object fLinkArray;
    protected object gtObj;
    protected object gtLinks;
    protected object OrderBy;
    protected object cLanguagesArray;
    protected string action;
    protected object folder;
    protected object menuIcon;
    protected object libraryIcon;
    protected object linkIcon;
    protected object sTitleBar;
    protected object maID;
    protected object mpID;
    protected object EnableMultilingual;
    protected ApplicationAPI AppUI = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
    protected string Callbackpage = "";
    protected EkMessageHelper MsgHelper;
    protected EkContentCol contentData;
    protected CommonApi m_refApi = new CommonApi();
    protected ContentAPI contentApi = new ContentAPI();
    protected string AncestorIDParam = "";
    protected string ParentIDParam = "";
    protected string checkout = "";
    protected bool bCheckedout = false;
    protected string m_strKeyWords = "";
    protected long folderId;
    protected long CurrentUserId;
    protected string contentIcon;
    protected string SitePath = "";
    protected string myTemp = "";
    protected string TitleRequired = "";
    protected string ItemLinkRequired = "";
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        action = Request.QueryString["action"];
        MsgHelper = m_refApi.EkMsgRef;
        RegisterResources();
        SetJsStrings();
        SitePath = AppUI.SitePath;
        jsSitePath.Text = SitePath;
        hidSitePath.Value = AppUI.SitePath;
        Head1.Title = AppUI.AppName + " " + "Collections";

        ItemLinkRequired = MsgHelper.GetMessage("js: item link required msg");
        TitleRequired = MsgHelper.GetMessage("js: title required msg");
        EnableMultilingual = m_refApi.EnableMultilingual;
        if (!Utilities.ValidateUserLogin())
        {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }
        if (contentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }
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
            AppUI.FilterByLanguage = ContentLanguage;
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
        m_refContentApi.FilterByLanguage = ContentLanguage;
        m_refApi.EkContentRef.RequestInformation.ContentLanguage = ContentLanguage;
        NoWorkAreaAttribute = "";
        if (!string.IsNullOrEmpty(Request.QueryString["noworkarea"]))
        {
            NoWorkAreaAttribute = "&noworkarea=1";
        }
        status = "";
        if (!string.IsNullOrEmpty(Request.QueryString["status"]))
        {
            status = "&status=o";
        }
        if (!Page.IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
            {
                if (Request.QueryString["checkout"].ToString().ToLower() == "true")
                {
                    m_refContentApi.EkContentRef.CheckoutEcmCollection(Convert.ToInt64(Request.QueryString["nid"]));
                }
                status = status + "&checout=" + Request.QueryString["checkout"];
            }
        }
        string bPage = "";
        if (!string.IsNullOrEmpty(Request.QueryString["bPage"]))
            bPage = Request.QueryString["bPage"].ToString();
        AppImgPath = AppUI.AppImgPath;
        sitePath = AppUI.SitePath;

        hidValue.Value = myTemp;
        MsgHelper = AppUI.EkMsgRef;
        AppUI.ContentLanguage = ContentLanguage;
        EnableMultilingual = AppUI.EnableMultilingual;
        CurrentUserId = AppUI.UserId;
        AppPath = AppUI.AppPath;
        sitePath = AppUI.SitePath;

        gtMsgObj = AppUI.EkSiteRef;

        folderId = Convert.ToInt64(Request.QueryString["folderid"]);

        contentIcon = "<img src=\"" + AppPath + "images/UI/Icons/contentHtml.png\" alt=\"Content\">";
        pageIcon = "<img src=\"" + AppPath + "images/UI/Icons/layout.png\" alt=\"Content\">";
        formsIcon = "<img src=\"" + AppPath + "images/UI/Icons/contentForm.png\" alt=\"Content\">";
        menuIcon = "<img src=\"" + AppPath + "images/UI/Icons/menu.png\" alt=\"Content\">";
        libraryIcon = "<img src=\"" + AppPath + "images/UI/Icons/book.png\" alt=\"Content\">";
        linkIcon = "<img src=\"" + AppPath + "images/UI/Icons/link.png\" alt=\"Content\">";

        switch (action)
        {
            case "Add":
                plhCollections.Controls.Add(LoadControl("controls/collection/addcollection.ascx"));
                break;
            case "Edit":
                plhCollections.Controls.Add(LoadControl("controls/collection/editcollection.ascx"));
                break;
            case "View":
            case "ViewStage":
                plhCollections.Controls.Add(LoadControl("controls/collection/viewcollection.ascx"));
                break;
            case "ViewAttributes":
            case "ViewStageAttributes":
                plhCollections.Controls.Add(LoadControl("controls/collection/viewattributes.ascx"));
                break;
            case "ReOrderLinks":
                plhCollections.Controls.Add(LoadControl("controls/collection/reorderlinkscollection.ascx"));
                break;
            case "DeleteLink":
                plhCollections.Controls.Add(LoadControl("controls/collection/deletecollection.ascx"));
                break;
            case "AddLink":
                plhCollections.Controls.Add(LoadControl("controls/collection/addlinkcollection.ascx"));
                break;
            case "ViewCollectionReport":			   
            case "ViewMenuReport":
                plhCollections.Controls.Add(LoadControl("controls/collection/collectionreport.ascx"));
                if (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]) && (Request.QueryString["reloadtrees"].ToLower() == "menu" || Request.QueryString["reloadtrees"] == "coll"))
                {
                    if (Request.QueryString["reloadtrees"].ToLower() == "menu")
                        ReloadClientScript("", "MenuTree");
                    else
                        ReloadClientScript("", "CollTree");
                }               
                break;
            case "AddMenu":
            case "AddSubMenu":
            case "AddTransMenu":
                plhCollections.Controls.Add(LoadControl("controls/menu/addmenu.ascx"));
                break;
            case "ReOrderMenuItems":
                plhCollections.Controls.Add(LoadControl("controls/menu/reordermenuitems.ascx"));
                break;
            case "DeleteMenuItem":
                plhCollections.Controls.Add(LoadControl("controls/menu/deletemenuitem.ascx"));
                break;
            case "EditMenu":
                plhCollections.Controls.Add(LoadControl("controls/menu/editmenu.ascx"));
                break;
            case "EditMenuItem":
                plhCollections.Controls.Add(LoadControl("controls/menu/editmenuitem.ascx"));
                break;
            case "ViewAllMenus":
                plhCollections.Controls.Add(LoadControl("controls/menu/viewallmenus.ascx"));
                break;
            case "pAddMenuItem":
                plhCollections.Controls.Add(LoadControl("controls/menu/paddMenuitem.ascx"));
                break;
            case "AddMenuItem":
                plhCollections.Controls.Add(LoadControl("controls/menu/addmenuitem.ascx"));
                break;
            case "ViewMenu":
                plhCollections.Controls.Add(LoadControl("controls/menu/viewmenucollection.ascx"));
                break;
            case "doAdd":
                doAdd();
                break;
            case "doDeleteMenu":
                doDeleteMenu();
                break;
            case "doAddMenu":
            case "doAddSubMenu":
            case "doAddTransMenu":
                doAddMenu();
                break;
            case "doEditMenu":
                doEditMenu();
                break;
            case "DoUpdateOrder":
            case "DoUpdateMenuItemOrder":
                DoUpdateOrder();
                break;
            case "doAddMenuItem":
                doAddMenuItem();
                break;
            case "doUpdateMenuItem":
                doUpdateMenuItem();
                break;
            case "doDeleteMenuItem":
                doDeleteMenuItem();
                break;
            case "doDeleteLinks":
            case "doAddLinks":
                doDeleteLinks();
                break;
            case "doEdit":
                doEdit();
                break;
            case "doDelete":
                doDelete();
                break;
            case "doSubmitCol":
                doSubmitCol();
                break;
            case "doPublishCol":
                doPublishCol();
                break;
            case "doSubmitDelCol":
                doSubmitDelCol();
                break;
            case "doDeclineDelCol":
                doDeclineDelCol();
                break;
            case "doDeclineApprCol":
                doDeclineApprCol();
                break;
            case "doUndoCheckoutCol":
                doUndoCheckoutCol();
                break;
            default:
                plhCollections.Controls.Add(LoadControl("controls/collection/viewothercollection.ascx"));
                if (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]) && Request.QueryString["reloadtrees"] == "coll")
                {
                    ReloadClientScript("", "CollTree");
                }
                break;
        }
    }
    private void ReloadClientScript(string idPath, string tree)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        try
        {
            idPath = "/" + idPath + "/";
            result.Append("top.TreeNavigation(\"" + tree + "\", \"" + idPath + "\");" + "\r\n");
            Ektron.Cms.API.JS.RegisterJSBlock(this.Page.Header, result.ToString(), "ReloadClientScript");
        }
        catch (Exception)
        {
        }
    }   
    private void doAdd()
    {
        try
        {
            long folderId = Convert.ToInt64(Request.Form["frm_folder_id"]);
            Collection saveObj = new Collection();

            saveObj.Add(Request.Form["frm_nav_title"], "CollectionTitle", null, null);
            saveObj.Add(Request.Form["frm_nav_template"], "CollectionTemplate", null, null);
            saveObj.Add(Request.Form["frm_nav_description"], "CollectionDescription", null, null);

            saveObj.Add(folderId, "FolderID", null, null);
            if (Request.Form["frm_recursive"] != null && Request.Form["frm_recursive"] != "")
                saveObj.Add(1, "Recursive", null, null);
            else
                saveObj.Add(0, "Recursive", null, null);
            if (Request.Form["frm_approval_methhod"] != null && Request.Form["frm_approval_methhod"] != "")
                saveObj.Add(true, "ApprovalRequired", null, null);
            else
                saveObj.Add(false, "ApprovalRequired", null, null);
            if (Request.Form["EnableReplication"] != null && Request.Form["EnableReplication"] != "")
                saveObj.Add(1, "EnableReplication", null, null);
            else
                saveObj.Add(0, "EnableReplication", null, null);
            AppUI.EkContentRef.AddEcmCollectionItem(saveObj);
            if ((bool)saveObj["ApprovalRequired"])
                Response.Redirect("collections.aspx?action=ViewStage&nId=" + saveObj["CollectionID"] + "&folderId=" + folderId + "&rf=1", false);
            else
                Response.Redirect("collections.aspx?action=View&nId=" + saveObj["CollectionID"] + "&folderId=" + folderId + "&rf=1", false);
            saveObj = null;
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message));
        }
    }
    private void doDeleteMenu()
    {
        long folderId = 0;
        try
        {
            folderId = Convert.ToInt64(Request.QueryString["folderid"]);
            long nID = Convert.ToInt64(Request.QueryString["nId"]);
            AppUI.EkContentRef.DeleteMenu(nID);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message));
        }
        if (Request.QueryString["back"] != "" && !string.IsNullOrEmpty(Request.QueryString["reloadtrees"]))
        {
            if (Request.QueryString["reloadtrees"].ToLower() == "menu")
            {
                Response.Redirect("collections.aspx?action=ViewMenuReport&rf=1&reloadtrees=menu" + "&LangType=" + ContentLanguage);
            }
            else
            {
                Response.Redirect("collections.aspx?action=ViewMenuReport" + "&LangType=" + ContentLanguage);
            }
        }
        else
            Response.Redirect("collections.aspx?action=ViewAllMenus&folderid=" + folderId + "&LangType=" + ContentLanguage);
    }
    private void doAddMenu()
    {
        string folderList = "";
        long Ret = 0;
        long bPage = 0;
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        long folderId = Convert.ToInt64(Request.Form["frm_folder_id"]);
        Collection saveObj = new Collection();
        saveObj.Add(action, "SaveType", null, null);

        if (!string.IsNullOrEmpty(Request.Form["associated_folder_id_list"]))
        {
            folderList = Request.Form["associated_folder_id_list"];
            saveObj.Add(folderList, "AssociatedFolderIdList", null, null);
        }

        if (!string.IsNullOrEmpty(Request.Form["associated_templates"]))
            saveObj.Add(Request.Form["associated_templates"], "AssociatedTemplates", null, null);

        switch (action)
        {
            case "doAddMenu":
                //' Adding a new Base Menu, therefore ParentID should = 0 AncestorID should = Null and MenuID should = Null
                //' In the DB, the ParentID will equal zero and the AncestorID will equal whatever new ID is
                //' created for this new menu.
                saveObj.Add(null, "MenuID", null, null);
                saveObj.Add(0, "MenuParentID", null, null);
                saveObj.Add(null, "MenuAncestorID", null, null);
                break;
            case "doAddSubMenu":
                //' Add a new Sub Menu. ParentID and AncestorID should just be passed normally. MenuID is generated
                //' on the back end, so send a null for MenuID.
                saveObj.Add(null, "MenuID", null, null);
                saveObj.Add(nId, "MenuParentID", null, null);
                saveObj.Add(Request.Form["frm_menu_ancestorid"], "MenuAncestorID", null, null);
                break;
            case "doAddTransMenu":
                //' Adding a new Base Menu that is a translation of an existing menu. We want to pass the MenuID,
                //' ParentID and AncestorID
                saveObj.Add(nId, "MenuID", null, null);
                saveObj.Add(0, "MenuParentID", null, null);
                saveObj.Add(Request.Form["frm_menu_ancestorid"], "MenuAncestorID", null, null);
                break;
        }

        saveObj.Add(Request.Form["frm_menu_title"], "MenuTitle", null, null);
        saveObj.Add(Request.Form["frm_menu_link"], "MenuLink", null, null);
        saveObj.Add(Request.Form["frm_menu_template"], "MenuTemplate", null, null);
        saveObj.Add(Request.Form["frm_menu_description"], "MenuDescription", null, null);
        saveObj.Add(Request.Form["frm_menu_image"], "MenuImage", null, null);
        if (!string.IsNullOrEmpty(Request.Form["frm_menu_image_override"]) && Request.Form["frm_menu_image_override"].ToLower() == "on")
            saveObj.Add("1", "ImageOverride", null, null);
        else
            saveObj.Add("0", "ImageOverride", null, null);
        saveObj.Add(folderId, "FolderID", null, null);
        saveObj.Add(1, "Recursive", null, null);
        if (action == "doAddSubMenu")
            saveObj.Add(true, "IsSubMenu", null, null);
        else
            saveObj.Add(false, "IsSubMenu", null, null);

        if (Request.Form["EnableReplication"] == "1")
            saveObj.Add(1, "EnableReplication", null, null);
        else
            saveObj.Add(0, "EnableReplication", null, null);

        try
        {
            Ret = AppUI.EkContentRef.AddEcmMenu(saveObj);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + ex.Message);
        }

        saveObj = null;

        if (action == "doAddSubMenu")
        {
            long submenuID = Ret;
            Collection ItemObj = new Collection();
            ItemObj.Add(Ret, "ItemID", null, null);
            ItemObj.Add("submenu", "ItemType", null, null);
            ItemObj.Add("self", "ItemTarget", null, null);
            ItemObj.Add("", "ItemLink", null, null);
            ItemObj.Add("", "ItemDescription", null, null);
            ItemObj.Add("", "ItemTitle", null, null);
            ItemObj.Add("0", "LinkType", null, null);
            AppUI.EkContentRef.RequestInformation.ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            AppUI.EkContentRef.AddItemToEcmMenu(nId, ItemObj);
            if (Request.QueryString["iframe"] == "true")
            {
                Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
            }
            else if (!string.IsNullOrEmpty(NoWorkAreaAttribute))
            {
                Response.Write("<script language=\"Javascript\">");
                Response.Write(" if (window.opener && !window.opener.closed) {window.opener.document.location.reload();}");
                Response.Write(" self.close();");
                Response.Write("</script>");
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["frm_back"]))
                    Response.Redirect("menu.aspx?Action=viewcontent&menuid=" + submenuID + "&LangType=" + ContentLanguage + "&treeViewId=-3&rf=1");
                else if (m_refApi.TreeModel == 1)
                    Response.Redirect("menu.aspx?Action=viewcontent&menuid=" + submenuID + "&LangType=" + ContentLanguage + "&treeViewId=-3&rf=1&reloadtrees=Menu");
                else
                    Response.Redirect("collections.aspx?action=ViewMenu&nid=" + nId + "&folderid=" + folderId + "&LangType=" + ContentLanguage + "&bPage=" + bPage);
            }

        }
        else
        {
            nId = Ret;
            if (!string.IsNullOrEmpty(Request.Form["frm_back"]))
            {
                if(string.IsNullOrEmpty(Request.QueryString["bpage"]))
                {
                    Response.Redirect("menu.aspx?Action=viewcontent&folderid=" + folderId + "&menuid=" + nId + "&LangType=" + ContentLanguage + "&treeViewId=-3");
                }
                else { Response.Redirect("menu.aspx?Action=viewcontent&menuid=" + nId + "&LangType=" + ContentLanguage + "&treeViewId=-3&rf=1"); }
            }
            else if (m_refApi.TreeModel == 1)
                Response.Redirect("menu.aspx?Action=viewcontent&menuid=" + nId + "&folderid=" + folderId + "&LangType=" + ContentLanguage + "&treeViewId=-3");
            else
                Response.Redirect("collections.aspx?action=ViewMenu&folderid=" + folderId + "&LangType=" + ContentLanguage + "&nid=" + nId + "&bPage=" + bPage);
        }
    }
    private void doEditMenu()
    {
        bool Ret = false;
        string actErrorString = "";
        string folderList = "";
        long folderId = Convert.ToInt64(Request.Form["frm_folder_id"]);
        long MenuID = Convert.ToInt64(Request.QueryString["nId"]);
        Collection saveObj = new Collection();
        saveObj.Add(Request.Form["frm_menu_title"], "MenuTitle", null, null);
        saveObj.Add(Request.Form["frm_menu_link"], "MenuLink", null, null);
        saveObj.Add(Request.Form["frm_menu_template"], "MenuTemplate", null, null);
        saveObj.Add(Request.Form["frm_menu_description"], "MenuDescription", null, null); //it is a textarea box, should not allow rich text (i.e. any HTML tags)
        saveObj.Add(Request.Form["frm_menu_image"], "MenuImage", null, null);
        if (Request.Form["frm_menu_image_override"] != null && Request.Form["frm_menu_image_override"].ToLower() == "on")
            saveObj.Add("1", "ImageOverride", null, null);
        else
            saveObj.Add("0", "ImageOverride", null, null);
        saveObj.Add(folderId, "FolderID", null, null);
        saveObj.Add(MenuID, "MenuID", null, null);
        saveObj.Add(1, "Recursive", null, null);
        if (!string.IsNullOrEmpty(Request.Form["associated_folder_id_list"]))
        {
            folderList = Request.Form["associated_folder_id_list"];
            saveObj.Add(folderList, "AssociatedFolderIdList", null, null);
        }
        if (!string.IsNullOrEmpty(Request.Form["associated_templates"]))
            saveObj.Add(Request.Form["associated_templates"], "AssociatedTemplates", null, null);
        if (Request.Form["EnableReplication"] != "")
            saveObj.Add(1, "EnableReplication", null, null);
        else
            saveObj.Add(0, "EnableReplication", null, null);
        try
        {
            Ret = AppUI.EkContentRef.UpdateMenu(saveObj);
        }
        catch (Exception ex)
        {
            actErrorString = ex.Message;
        }
        saveObj = null;
        if (actErrorString.Length > 0)
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(actErrorString));
        else
        {
            if (Request.Form["frm_set_to_template"].ToLower() == "true")
            {
                Collection colMenu = AppUI.EkContentRef.GetMenuByID(MenuID, 0, false);
                Collection colMenuItems = (Collection)colMenu["Items"];

                if (colMenuItems.Count > 0)
                {
                    foreach (Collection colMenuItem in colMenuItems)
                    {
                        if (colMenuItem["ItemType"].ToString() == "1")
                        {
                            if (colMenuItem["LinkType"].ToString() == "0")
                            {
                                Collection colNewMenuItem = AppUI.EkContentRef.GetMenuItemByID(MenuID, colMenuItem["ID"], true);

                                colNewMenuItem.Remove("LinkType");
                                colNewMenuItem.Add("1", "LinkType", null, null);

                                colNewMenuItem.Remove("ItemID");
                                colNewMenuItem.Add(colNewMenuItem["ID"], "ItemID", null, null);

                                colNewMenuItem.Remove("ItemLink");
                                colNewMenuItem.Add("", "ItemLink", null, null);

                                Ret = AppUI.EkContentRef.UpdateMenuItem(colNewMenuItem);
                            }
                        }
                    }
                }
            }
            if (Request.Form["frm_back"] != "")
                Response.Redirect(Request.Form["frm_back"] + "&rf=1", false);
            else if (Request.QueryString["iframe"] == "true")
                Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
            else
            {
                if (m_refApi.TreeModel == 1)
                    Response.Redirect("menutree.aspx?nid=" + MenuID + "&folderid=" + folderId);
                else
                    Response.Redirect("collections.aspx?action=ViewMenu&nid=" + MenuID + "&folderid=" + folderId);
            }
        }
    }
    private void DoUpdateOrder()
    {
        string orderList = Request.Form["LinkOrder"];
        long nId = Convert.ToInt64(Request.Form["navigationid"]);
        long folderId = Convert.ToInt64(Request.Form["frm_folder_id"]);
        AppUI.FilterByLanguage = ContentLanguage;
        if (orderList != "")
        {
            if (action == "DoUpdateMenuItemOrder")
            {
                bool Ret = AppUI.EkContentRef.UpdateEcmMenuItemOrder(nId.ToString(), orderList);
                if (!Ret)
                {
                    if (Request.Form["frm_back"] != "")
                        Response.Redirect(Request.Form["frm_back"], false);
                    else if (Request.QueryString["iframe"] == "true")
                        Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
                    else
                    {
                        if (m_refApi.TreeModel == 1)
                            Response.Redirect("menutree.aspx?nid=" + nId + "&folderid=" + folderId);
                        else
                            Response.Redirect("collections.aspx?Action=ViewMenu&nid=" + nId + "&folderid=" + folderId);
                    }
                }
            }
            else
            {
                string bakAction = "action=View";
                if (!string.IsNullOrEmpty(Request.QueryString["status"]))
                {
                    if (Request.QueryString["status"].ToString().ToLower() == "o")
                        bakAction = "action=ViewStage";
                }
                bool Ret = AppUI.EkContentRef.UpdateEcmCollectionItemOrder(nId, orderList);
                if (!Ret)
                    Response.Redirect("collections.aspx?" + bakAction + "&LangType=" + ContentLanguage + "&nid=" + nId + "&folderid=" + folderId);
            }
        }
        else
        {
            if (action == "DoUpdateMenuItemOrder")
            {
                if (Request.Form["frm_back"] != "")
                    Response.Redirect(Request.Form["frm_back"], false);
                else if (Request.QueryString["iframe"] == "true")
                    Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
                else
                {
                    if (m_refApi.TreeModel == 1)
                        Response.Redirect("menutree.aspx?nid=" + nId + "&folderid=" + folderId);
                    else
                        Response.Redirect("collections.aspx?Action=ViewMenu&nid=" + nId + "&folderid=" + folderId);
                }
            }
            else
                Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&treeViewId=-2");
        }
    }
    private void doAddMenuItem()
    {
        string ItemType = Request.QueryString["type"].ToString();
        long menuId = Convert.ToInt64(Request.QueryString["nid"]);
        long folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        if (ItemType == "content")
        {
            string[] idArray = Request.Form["frm_content_ids"].Split(',');
            string[] folderArray = Request.Form["frm_folder_ids"].Split(',');
            foreach (string idArr in idArray)
            {
                Collection ItemObj = new Collection();
                ItemObj.Add(idArr, "ItemID", null, null);
                ItemObj.Add("content", "ItemType", null, null);
                ItemObj.Add("self", "ItemTarget", null, null);
                ItemObj.Add("", "ItemLink", null, null);
                ItemObj.Add("", "ItemDescription", null, null);
                ItemObj.Add("", "ItemTitle", null, null);
				ItemObj.Add(0, "LinkType", null, null);
                AppUI.ContentLanguage = Convert.ToInt32(Request.Form["frm_languages0"]);
                bool Ret = AppUI.EkContentRef.AddItemToEcmMenu(menuId, ItemObj);
                ItemObj = null;
            }
        }
        else if (ItemType == "link")
        {
            Collection ItemObj = new Collection();
            ItemObj.Add(0, "ItemID", null, null);
            ItemObj.Add("link", "ItemType", null, null);
            ItemObj.Add(Request.Form["Title"], "ItemTitle", null, null);
            ItemObj.Add(Server.HtmlEncode(Request.Form["Link"].ToString()), "ItemLink", null, null);
            ItemObj.Add("self", "ItemTarget", null, null);
            ItemObj.Add("", "ItemDescription", null, null);
            bool Ret = AppUI.EkContentRef.AddItemToEcmMenu(Request.Form["CollectionID"], ItemObj);
            ItemObj = null;
        }
        else if (ItemType == "library")
        {
            long mId = Convert.ToInt64(Request.Form["CollectionID"]);
            Collection ItemObj = new Collection();
            ItemObj.Add(Request.Form["id"], "ItemID", null, null);
            ItemObj.Add("library", "ItemType", null, null);
            ItemObj.Add(Request.Form["title"], "ItemTitle", null, null);
            ItemObj.Add("self", "ItemTarget", null, null);
            ItemObj.Add("", "ItemDescription", null, null);
            ItemObj.Add("", "ItemLink", null, null);
            bool Ret = AppUI.EkContentRef.AddItemToEcmMenu(mId, ItemObj);
            ItemObj = null;
        }

        if (!string.IsNullOrEmpty(Request.Form["frm_back"]))
            Response.Redirect(Request.Form["frm_back"] + "&rf=1", false);
        else if (Request.QueryString["iframe"] == "true")
            Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
        else if (!string.IsNullOrEmpty(NoWorkAreaAttribute))
        {
            Response.Write("<script language=\"Javascript\">");
            Response.Write("if (window.opener && !window.opener.closed) {window.opener.document.location.reload();}");
            Response.Write("document.location = '" + AppPath +"close.aspx';");
            Response.Write("</script>");
        }
        else
        {
            Response.Redirect("menu.aspx?Action=viewcontent&menuid=" + menuId + "&treeViewId=-3&LangType=" + Request.Form["frm_languages0"]);
        }
    }
    private void doUpdateMenuItem()
    {
        long MenuID = Convert.ToInt64(Request.Form["CollectionID"]);
        long ID = Convert.ToInt64(Request.QueryString["ID"]);
        string Title = Request.Form["ctl11$Title"];
        string Link = Request.Form["Link"];
        string Target = Request.Form["Target"];
        string Description = Request.Form["Description"];
        long folderId = Convert.ToInt64(Request.Form["FolderId"]);
        string ItemType = Request.QueryString["type"];
        Collection ItemObj = new Collection();
        ItemObj.Add(ID, "ItemID", null, null);
        ItemObj.Add(Title, "ItemTitle", null, null);
        if ((ItemType == "1") || (ItemType == "2") || (ItemType == "4"))
            ItemObj.Add("", "ItemLink", null, null);
        else
            ItemObj.Add(Server.HtmlEncode(Link), "ItemLink", null, null);
        if (ItemType == "1")
            ItemObj.Add(Server.HtmlEncode(Request.Form["linkType"]), "LinkType", null, null);
        else
            ItemObj.Add("", "LinkType", null, null);

        ItemObj.Add(Target, "Target", null, null);
        ItemObj.Add(Description, "ItemDescription", null, null);
        ItemObj.Add(MenuID, "MenuID", null, null);
        ItemObj.Add(folderId, "FolderID", null, null);
        ItemObj.Add(Request.Form["frm_menu_image"], "ItemImage", null, null);

		if (ItemObj.Contains("LinkType"))
		{
			if (ItemObj["LinkType"].ToString() == "")
			{
				ItemObj.Remove("LinkType");
				ItemObj.Add("0","LinkType", null,null);
			}
		}
		else
		{
			ItemObj.Add("0", "LinkType", null, null);
		}

    	if (!string.IsNullOrEmpty(Request.Form["frm_menu_image_override"]) && Request.Form["frm_menu_image_override"].ToLower() == "on")
            ItemObj.Add("1", "ImageOverride", null, null);
        else
            ItemObj.Add("0", "ImageOverride", null, null);
        bool bRet = AppUI.EkContentRef.UpdateMenuItem(ItemObj);
        if (Request.Form["frm_back"] != "")
            Response.Redirect(Request.Form["frm_back"], false);
        else if (Request.QueryString["iframe"] == "true")
            Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
        else
        {
            if (m_refApi.TreeModel == 1)
                Response.Redirect("menutree.aspx?nid=" + MenuID + "&folderid=" + folderId);
            else
                Response.Redirect("collections.aspx?Action=ViewMenu&nid=" + MenuID + "&folderid=" + folderId);
        }
    }
    private void doDeleteMenuItem()
    {
        //Dim arTmp, arInfo As Object
        string[] idArray = null;
        if (Request.QueryString["back"] != "")
            idArray = Request.QueryString["ids"].ToString().Split(',');
        else if (m_refApi.TreeModel == 1)
            idArray = Request.QueryString["frm_content_ids"].Split(',');
        else
            idArray = Request.Form["frm_content_ids"].Split(',');
        long folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        long pid = Convert.ToInt64(Request.QueryString["pid"]);
        Int32 intCounter = 0;
        foreach (string idArr in idArray)
        {
            string[] arTmp = idArray[intCounter].Split('.');
            if (arTmp.Length > 0)
            {
                string[] arInfo = arTmp[0].Split('_');
                if (arInfo.Length > 0)
                {
                    if (Request.QueryString["back"] != "")
                    {
                        if ((Convert.ToInt64(Request.QueryString["nid"]) != 0) && (arInfo[0] != ""))
                            AppUI.EkContentRef.DeleteItemFromMenu(Convert.ToInt64(Request.QueryString["nid"]), arInfo[0]);
                    }
                    else if (m_refApi.TreeModel == 1)
                    {
                        if ((Request.QueryString["CollectionID"] != "") && (arInfo[0] != ""))
                            AppUI.EkContentRef.DeleteItemFromMenu(Request.QueryString["CollectionID"], arInfo[0]);
                    }
                    else
                    {
                        if ((Request.Form["CollectionID"] != "") && (arInfo[0] != ""))
                            AppUI.EkContentRef.DeleteItemFromMenu(Request.Form["CollectionID"], arInfo[0]);
                    }
                }
                if (arTmp.Length > 1)
                {
                    if (Convert.ToInt64(arTmp[1]) == 4)
                    {
                        if (arTmp[2] != "")
                            AppUI.EkContentRef.DeleteMenu(arTmp[2]);
                    }
                }
            }
            intCounter++;
        }
        if (Request.QueryString["back"] != "")
            Response.Redirect(Request.QueryString["back"] + "&rf=1", false);
        else if (Request.Form["frm_back"] != "")
            Response.Redirect(Request.Form["frm_back"], false);
        else if (Request.QueryString["iframe"] == "true")
        {
            if (m_refApi.TreeModel == 1)
                Response.Write("<script language=\"Javascript\">parent.CloseChildPage();</script>");
            else
                Response.Redirect("collections.aspx?Action=ViewMenu&nid=" + Request.QueryString["nid"] + "&folderid=" + folderId);
        }
        else
        {
            if (m_refApi.TreeModel == 1)
                Response.Redirect("menutree.aspx?nid=" + Request.Form["pid"] + "&folderid=" + folderId);
            else
                Response.Redirect("collections.aspx?Action=ViewMenu&nid=" + Request.Form["CollectionID"] + "&folderid=" + folderId);
        }
    }
    private void doDeleteLinks()
    {
        string[] idArray = Request.Form["frm_content_ids"].Split(',');
        string[] langArray = Request.Form["frm_content_languages"].Split(',');
        string[] folderArray = Request.Form["frm_folder_ids"].Split(',');
        bool Ret = false;
        long folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        Int32 intCounter = 0;
        foreach (string idArr in idArray)
        {
            AppUI.ContentLanguage = Convert.ToInt32(langArray[intCounter]);
            if (action == "doDeleteLinks")
                Ret = AppUI.EkContentRef.DeleteItemFromEcmCollection(Convert.ToInt64(Request.QueryString["nId"]), Convert.ToInt64(idArray[intCounter]), Convert.ToInt32(langArray[intCounter]));
            else
                Ret = AppUI.EkContentRef.AddItemToEcmCollection(Convert.ToInt64(Request.QueryString["nId"]), Convert.ToInt64(idArray[intCounter]), Convert.ToInt32(langArray[intCounter]));
            intCounter++;
        }
        string bakAction = "action=View";
        if (!string.IsNullOrEmpty(Request.QueryString["status"]))
        {
            if (Request.QueryString["status"].ToString().ToLower() == "o")
                bakAction = "action=ViewStage";
        }
        if (!string.IsNullOrEmpty(NoWorkAreaAttribute))
        {
            Response.Write("<script language=\"Javascript\">");
            Response.Write(" if (window.opener && !window.opener.closed) {window.opener.document.location.href = window.opener.document.location.href;}");
            Response.Write(" self.close();");
            Response.Write("</script>");
        }
        else
        Response.Redirect("collections.aspx?" + bakAction + "&nid=" + Request.QueryString["nId"] + "&folderid=" + folderId);
    }
    private void doEdit()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        long folderId = -1;
        Collection edObj = new Collection();
        edObj.Add(Request.Form["frm_nav_title"], "CollectionTitle", null, null);
        edObj.Add(Request.Form["ctl07$frm_nav_template"], "CollectionTemplate", null, null);
        folderId = GetFolderId(folderId);
        edObj.Add(Request.Form["frm_nav_description"], "CollectionDescription", null, null);
        edObj.Add(nId, "CollectionID", null, null);
        if (folderId >= 0) {
            edObj.Add(folderId, "FolderID", null, null);
        }
        if (Request.Form["frm_recursive"] != null && Request.Form["frm_recursive"] != "")
            edObj.Add(1, "Recursive", null, null);
        else
            edObj.Add(0, "Recursive", null, null);
        if (Request.Form["frm_approval_methhod"] != null && Request.Form["frm_approval_methhod"] != "")
            edObj.Add(true, "ApprovalRequired", null, null);
        else
            edObj.Add(false, "ApprovalRequired", null, null);

        if (Request.Form["EnableReplication"] != null && Request.Form["EnableReplication"] != "")
            edObj.Add(1, "EnableReplication", null, null);
        else
            edObj.Add(0, "EnableReplication", null, null);
        bool Ret = AppUI.EkContentRef.UpdateEcmCollectionInfo(edObj);
        string bakAction = "action=View";
        if ((Request.QueryString["status"] == "o") && (edObj["ApprovalRequired"] != null))
            bakAction = "action=ViewStage";
        string actErrorString = "";
        if (actErrorString.Length > 0)
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(actErrorString));
        else
            Response.Redirect("collections.aspx?" + bakAction + "&nid=" + nId + ((folderId >= 0) ? "&folderid=" + folderId : "") + "&rf=1");
    }
    private long GetFolderId(long defaultFolderId) {
        long formFolderId = -1;
        if (!string.IsNullOrEmpty(Request.Form["frm_nav_folderid"]) && long.TryParse(Request.Form["frm_nav_folderid"], out formFolderId) && formFolderId >= 0) {
            return formFolderId;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["folderid"])) {
            return Convert.ToInt64(Request.QueryString["folderid"]);
        }

        return defaultFolderId;
    }
    private void doDelete()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        bool ret = AppUI.EkContentRef.DeleteCollectionItem(nId);
        if (ret)
        {

            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode("Bad Delete of Collection"));

        }
        else
        {
            if (folderId < 0 | (!string.IsNullOrEmpty(Request.QueryString["bpage"]) && Request.QueryString["bpage"] == "reports"))
            {
                Response.Redirect("collections.aspx?action=ViewCollectionReport&rf=1&reloadtrees=coll");
            }
            else
            {
                Response.Redirect("collections.aspx?action=mainPage&reloadtrees=coll&folderid=" + folderId);
            }
        }
    }
    private void doSubmitCol()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            AppUI.EkContentRef.SubmitEcmCollection(nId);
            Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }
    private void doPublishCol()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            AppUI.EkContentRef.PublishEcmCollection(nId);
            Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId + "&rf=1&reloadtrees=coll", false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }
    private void doSubmitDelCol()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            bool bCheckout = false;
            if (!string.IsNullOrEmpty(Request.QueryString["checkout"]))
            {
                if ((string.IsNullOrEmpty(Request.QueryString["status"])) || (!string.IsNullOrEmpty(Request.QueryString["status"]) && Request.QueryString["status"] != "o"))
                {
                    bCheckout = true;
                    AppUI.EkContentRef.CheckoutEcmCollection(nId);
                }
            }
            if (!bCheckout)
                AppUI.EkContentRef.DeleteCollectionItem(nId);
            else
                AppUI.EkContentRef.SubmitDeleteEcmCollection(nId);
            if (bCheckout)
                Response.Redirect("collections.aspx?Action=ViewStage&nid=" + nId + "&folderid=" + folderId, false);
            else
                Response.Redirect("collections.aspx?Action=ViewAll&nid=" + nId + "&folderid=" + folderId, false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }
    private void doDeclineDelCol()
    {
        string strDeleteDeclineMsg = "";
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            if (!string.IsNullOrEmpty(Request.Form["DeleteDeclineReason"]))
                strDeleteDeclineMsg = Request.Form["DeleteDeclineReason"];
            bool Ret = AppUI.EkContentRef.DeleteDeclineEcmCollection(nId, strDeleteDeclineMsg);
            Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }
    private void doDeclineApprCol()
    {
        string strApprovalDeclineMsg = "";
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            if (!string.IsNullOrEmpty(Request.Form["ApprovalDeclineReason"]))
                strApprovalDeclineMsg = Request.Form["ApprovalDeclineReason"];
            bool Ret = AppUI.EkContentRef.ApprovalDeclineEcmCollection(nId, strApprovalDeclineMsg);
            Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }
    private void doUndoCheckoutCol()
    {
        long nId = Convert.ToInt64(Request.QueryString["nId"]);
        try
        {
            bool Ret = AppUI.EkContentRef.UndoCheckoutEcmCollection(nId);
            Response.Redirect("collections.aspx?Action=View&nid=" + nId + "&folderid=" + folderId, false);
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(ex.Message.ToString()), false);
        }
    }

    protected void SetJsStrings()
    {
        jsDeleteConfirm.Text = MsgHelper.GetMessage("js: confirm delete collection");

        ltr_titleAlertMsg.Text = MsgHelper.GetMessage("js:menu crate no title msg");
        ltr_imagepathAlertMsg.Text = MsgHelper.GetMessage("js:menu crate no img path msg");
        ltr_templatenoTitleMsg.Text = MsgHelper.GetMessage("js:add menu no template title");
        jsNoItemSelectedToDelete.Text = jsNoItemSelected.Text = MsgHelper.GetMessage("js:no items selected");
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIWidgetJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS);

        Ektron.Cms.API.JS.RegisterJS(this, "java/cmsmenuapi.js", "CmsMenuApiJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchInputLabelInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);

        if (string.IsNullOrEmpty(action) || action.ToLower() != "dodeletemenu")
        {
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            Ektron.Cms.API.Css.RegisterCss(this, "csslib/ektron.widgets.selector.css", "EktronWidgetsSelectorCss");
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE6);
        }
    }
}