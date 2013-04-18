using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms;
using System.Data;
using Microsoft.VisualBasic;
using Ektron.Cms.Common;
using System.Collections;

public partial class Workarea_controls_collection_collectionreport : System.Web.UI.UserControl
{
    const Int64 ALL_CONTENT_LANGUAGES = -1;
    Int32 TotalPagesNumber;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper MsgHelper;
    protected string m_strKeyWords = "";
    protected int _currentPageNumber = 1;
    protected CommonApi m_refApi = new CommonApi();
    Int32 ContentLanguage = 0;
    ApplicationAPI AppUI = new ApplicationAPI();
    protected object EnableMultilingual;
    protected long folderID = 0;
    protected string strPath;
    protected bool report = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        strPath = Request.ServerVariables["PATH_INFO"] + "?" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.ServerVariables["QUERY_STRING"].ToString().Replace("LangType", "L").Replace("\\x", "\\\\x"));
        MsgHelper = new EkMessageHelper(AppUI.RequestInformationRef);

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
                    if (ContentLanguage == 0)
                    {
                        ContentLanguage = AppUI.DefaultContentLanguage;
                        m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                    }
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
        if (!string.IsNullOrEmpty(Request.QueryString["folderid"]))
        {
            folderID = Convert.ToInt64(Request.QueryString["folderid"]);
            if (folderID == 0)           
                report = true;           
        }
        m_refContentApi.FilterByLanguage = ContentLanguage;
        AppUI.FilterByLanguage = ContentLanguage;
        m_refApi.EkContentRef.RequestInformation.ContentLanguage = ContentLanguage;

        if ((Request.QueryString["action"] == "ViewCollectionReport" && !IsPostBack) || (Request.QueryString["action"] == "ViewCollectionReport" && IsPostBack && !string.IsNullOrEmpty(Request.Form[isSearchPostData.UniqueID]) && !string.IsNullOrEmpty(Request.Form[isCPostData.UniqueID])))
        {
            LoadCollectionList();

            if (Request.QueryString["rf"] == "1")
            {
                litRefreshAccordion.Text = "<script language=\"javascript\">" + ("\r\n" + "top.refreshCollectionAccordion(") + ContentLanguage + ");" + ("\r\n" + "</script>") + "\r\n";
            }

        }
        if ((Request.QueryString["action"] == "ViewMenuReport" && !IsPostBack) || (Request.QueryString["action"] == "ViewMenuReport" && IsPostBack && !string.IsNullOrEmpty(Request.Form[isSearchPostData.UniqueID]) && !string.IsNullOrEmpty(Request.Form[isCPostData.UniqueID])))
        {
            LoadMenuList();

            if (Request.QueryString["rf"] == "1")
            {
                litRefreshAccordion.Text = "<script language=\"javascript\">" + ("\r\n" + "top.refreshMenuAccordion(") + ContentLanguage + ");" + ("\r\n" + "</script>") + "\r\n";
            }

        }
    }
    private void LoadMenuList()
    {
        PageRequestData req = new PageRequestData();
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        req.CurrentPage = _currentPageNumber;
        m_strKeyWords = Request.Form["txtSearch"];
        if (m_strKeyWords == null)
        {
            m_strKeyWords = "";
        }
        Collection menu_list = m_refApi.EkContentRef.GetMenuReport(m_strKeyWords, ref req);
        TotalPagesNumber = req.TotalPages;
        PageSettings();
        ViewAllMenuToolBar(m_strKeyWords);
        if ((menu_list != null) && menu_list.Count > 0)
        {
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("TITLE", MsgHelper.GetMessage("generic Title"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("ID", MsgHelper.GetMessage("generic ID"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("LANGUAGE", MsgHelper.GetMessage("generic language"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", MsgHelper.GetMessage("generic Description"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(40), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("PATH", MsgHelper.GetMessage("generic Path"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
            dt.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
            dt.Columns.Add(new DataColumn("PATH", typeof(string)));


            foreach (Collection coll in menu_list)
            {
                dr = dt.NewRow();
                if (m_refApi.TreeModel == 0)
                {
                    dr["TITLE"] = "<a href=\"collections.aspx?folderid=" + coll["FolderID"].ToString() + "&Action=ViewMenu&nid=" + coll["MenuID"] + "&bpage=reports&LangType=" + coll["ContentLanguage"] + "\"  alt=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(coll["MenuTitle"]), "\'", "`", 1, -1, 0) + "\"\' title=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(coll["MenuTitle"]), "\'", "`", 1, -1, 0) + "\"\'>" + coll["MenuTitle"] + "</a>";
                }
                else
                {
                    string enableQDOparam = "";
                    if (Convert.ToInt32(coll["EnableReplication"]) == 1)
                    {
                        enableQDOparam = "&qdo=1";
                    }
                    //dr("TITLE") = "<a href=""menutree.aspx?folderid=" & menu_list(i)("FolderID") & "&nid=" & menu_list(i)("MenuID") & "&bpage=reports&LangType=" & menu_list(i)("ContentLanguage") & """  alt='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & """' title='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & enableQDOparam & """'>" & menu_list(i)("MenuTitle") & "</a>"
                    dr["TITLE"] = "<a href=\"menu.aspx?Action=viewcontent&menuid=" + coll["MenuID"] + "&LangType=" + coll["ContentLanguage"] + "&treeViewId=-3" + "\"  alt=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(coll["MenuTitle"]), "\'", "`", 1, -1, 0) + "\"\' title=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(coll["MenuTitle"]), "\'", "`", 1, -1, 0) + enableQDOparam + "\"\'>" + coll["MenuTitle"] + "</a>";
                }
                dr["ID"] = coll["MenuID"];
                dr["LANGUAGE"] = coll["ContentLanguage"];
                dr["DESCRIPTION"] = coll["MenuDescription"];
                dr["PATH"] = coll["Path"];
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            CollectionListGrid.DataSource = dv;
            CollectionListGrid.DataBind();
        }
    }
    private void LoadCollectionList()
    {
        PageRequestData req = new PageRequestData();
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        req.CurrentPage = _currentPageNumber;
        m_strKeyWords = Request.Form["txtSearch"];
        if (m_strKeyWords == null)
        {
            m_strKeyWords = "";
        }
        Collection gtNavs = null;
        CollectionListData[] collection_list =null;
        if (folderID > 0 || report)
        {           
            gtNavs = AppUI.EkContentRef.GetAllCollectionsInfo(folderID, "title");
            TotalPagesNumber =0;
            PageSettings();
        }       
        else 
        {
            collection_list = m_refApi.EkContentRef.GetCollectionList(m_strKeyWords, ref req);
            TotalPagesNumber = req.TotalPages;
            PageSettings();
        }

        ViewAllCollectionToolBar(m_strKeyWords);
        if ((collection_list != null) && collection_list.Length > 0)
        {
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("TITLE", MsgHelper.GetMessage("generic Title"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("ID", MsgHelper.GetMessage("generic ID"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", MsgHelper.GetMessage("generic Description"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(40), Unit.Percentage(40), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("PATH", MsgHelper.GetMessage("generic Path"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
            dt.Columns.Add(new DataColumn("PATH", typeof(string)));

            for (int i = 0; i <= collection_list.Length - 1; i++)
            {
                // Display all the collection in the list found. This is displayed under the Content -> Collection view.
                if (folderID == 0)
                {
                    string vAction = "";
                    if (collection_list[i].ApprovalRequired && collection_list[i].Status != "A")
                    {
                        vAction = "&Action=ViewStage";
                    }
                    else
                    {
                        vAction = "&Action=View";
                    }
                    dr = dt.NewRow();
                    dr["TITLE"] = "<a href=\"collections.aspx?folderid=" + collection_list[i].FolderId + vAction + "&nid=" + collection_list[i].Id + "&bpage=reports\" alt=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(collection_list[i].Title, "\'", "`", 1, -1, 0) + "\"\' title=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(collection_list[i].Title, "\'", "`", 1, -1, 0) + "\"\'>" + collection_list[i].Title + "</a>";
                    dr["ID"] = collection_list[i].Id;
                    dr["DESCRIPTION"] = collection_list[i].Description;
                    dr["PATH"] = collection_list[i].FolderPath;
                    dt.Rows.Add(dr);
                }
                // Display the collection that are assigned to a particular folder.
                else if (folderID == collection_list[i].FolderId)
                {
                    string vAction = "";
                    if (collection_list[i].ApprovalRequired && collection_list[i].Status != "A")
                    {
                        vAction = "&Action=ViewStage";
                    }
                    else
                    {
                        vAction = "&Action=View";
                    }
                    dr = dt.NewRow();
                    dr["TITLE"] = "<a href=\"collections.aspx?folderid=" + collection_list[i].FolderId + vAction + "&nid=" + collection_list[i].Id + "&bpage=reports\" alt=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(collection_list[i].Title, "\'", "`", 1, -1, 0) + "\"\' title=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(collection_list[i].Title, "\'", "`", 1, -1, 0) + "\"\'>" + collection_list[i].Title + "</a>";
                    dr["ID"] = collection_list[i].Id;
                    dr["DESCRIPTION"] = collection_list[i].Description;
                    dr["PATH"] = collection_list[i].FolderPath;
                    dt.Rows.Add(dr);
                }
            }
            DataView dv = new DataView(dt);
            CollectionListGrid.DataSource = dv;
            CollectionListGrid.DataBind();
        }
        if ((gtNavs != null) && gtNavs.Count > 0)
        {
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("CollectionTitle", MsgHelper.GetMessage("generic Title"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("CollectionID", MsgHelper.GetMessage("generic ID"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("DisplayLastEditDate", MsgHelper.GetMessage("generic Date Modified"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), false, false));
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("CollectionTemplate", MsgHelper.GetMessage("generic URL Link"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(40), Unit.Percentage(40), false, false));
           
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("CollectionTitle", typeof(string)));
            dt.Columns.Add(new DataColumn("CollectionID", typeof(string)));
            dt.Columns.Add(new DataColumn("DisplayLastEditDate", typeof(string)));
             dt.Columns.Add(new DataColumn("CollectionTemplate", typeof(string)));
            

            if (gtNavs.Count > 0)
            {
                DataTable dtItems = new DataTable();
                dtItems.Columns.Add("CollectionID");
                dtItems.Columns.Add("DisplayLastEditDate");
                dtItems.Columns.Add("CollectionTemplate");
                dtItems.Columns.Add("CollectionTitle");
                dtItems.Columns.Add("CollectionLink");

                foreach (Collection gtNa in gtNavs)
                {
                    string colAction = "";
                    if (Convert.ToBoolean(gtNa["ApprovalRequired"]) && gtNa["Status"].ToString().ToUpper() != "A")
                        colAction = "&action=ViewStage";
                    else
                        colAction = "&action=View";
                    DataRow dRow = dtItems.NewRow();
                    dRow["CollectionID"] = gtNa["CollectionID"].ToString();
                    dRow["DisplayLastEditDate"] = gtNa["DisplayLastEditDate"].ToString();
                    dRow["CollectionTemplate"] = gtNa["CollectionTemplate"].ToString();
                    dRow["CollectionTitle"] = "<a href=\"collections.aspx?folderid=" + folderID + colAction + "&nid=" + gtNa["CollectionID"].ToString() + "&bpage=reports\" alt=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(gtNa["CollectionTitle"].ToString(), "\'", "`", 1, -1, 0) + "\"\' title=\'" + MsgHelper.GetMessage("generic View") + " \"" + Strings.Replace(gtNa["CollectionTitle"].ToString(), "\'", "`", 1, -1, 0) + "\"\'>" + gtNa["CollectionTitle"].ToString() + "</a>"; 
                    dRow["CollectionLink"] = "collections.aspx?folderid=" + folderID + colAction + "&nid=" + gtNa["CollectionID"].ToString();
                    dtItems.Rows.Add(dRow);
                }               
                CollectionListGrid.DataSource = dtItems;
                CollectionListGrid.DataBind();
            }
           
        }
    }
    protected bool IsCollectionRoleMember()
    {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection);
    }
    protected bool IsMenuRoleMember() {
        return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
            || m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu);
    }
    private void ViewAllMenuToolBar(string searchstring)
    {
        bool canAddMenu = false;

        Hashtable cPerms = AppUI.EkSiteRef.GetPermissions(0, 0, "folder");
        if (cPerms.Contains("Collections"))
        {
            canAddMenu = Convert.ToBoolean(cPerms["Collections"]);
        }
        canAddMenu |= IsMenuRoleMember();
        if (searchstring == "")
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view all menu title"));
        }
        else
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("search menu title"));
        }
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");

        if (canAddMenu)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption((string)(AppUI.AppPath + "images/UI/Icons/add.png"), (string)("collections.aspx?action=AddMenu&folderid=0&LangType=" + ContentLanguage + "&bPage=ViewMenuReport" + "&back=" + EkFunctions.UrlEncode("collections.aspx?action=ViewMenuReport")), MsgHelper.GetMessage("alt add new menu"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
        }

        if (m_refContentApi.EnableMultilingual == 1)
        {
            if (canAddMenu)
            {
                result.Append(m_refStyle.GetExportTranslationButton("content.aspx?type=menu&id=0&LangType=" + ContentLanguage + "&action=Localize&callbackpage=Collections.aspx&parm1=action&value1=ViewMenuReport", MsgHelper.GetMessage("alt Click here to export all menus for translation"), MsgHelper.GetMessage("lbl Export For Translation")));
            }
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">");
            result.Append(MsgHelper.GetMessage("view language"));
            result.Append("</td>");
            result.Append("<td>");
            result.Append(m_refStyle.ShowAllActiveLanguage(true, "", "javascript:LoadLanguage(this.value, '" + strPath + "');", ContentLanguage.ToString()) + "&nbsp;<br>");
            result.Append("</td>");
        }
        result.Append("<td class=\"label\">&#160;");
        result.Append("<label for=\"txtSearch\">" + MsgHelper.GetMessage("generic search") + "</label>");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<input type=text class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<input type=button value=" + MsgHelper.GetMessage("btn Search") + " id=btnSearch name=btnSearch class=\"ektronWorkareaSearch\" onclick=\"searchcollection();\" title=\"Search Collections\"></td>");
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("ViewMenuReport", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void ViewAllCollectionToolBar(string searchstring)
    {
        bool canIAddCol = false;
        Hashtable cPerms = AppUI.EkSiteRef.GetPermissions(0, 0, "folder");
        if (cPerms.Contains("Collections"))
        {
            canIAddCol = Convert.ToBoolean(cPerms["Collections"]);
        }
        canIAddCol |= IsCollectionRoleMember();
        if (searchstring == "")
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view all collections title"));
        }
        else
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("search collections title"));
        }
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        if (canIAddCol)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption((string)(AppUI.AppPath + "images/UI/Icons/add.png"), (string)("collections.aspx?action=Add&folderid=0&LangType=" + ContentLanguage + "&back=" + EkFunctions.UrlEncode("collections.aspx?action=ViewCollectionReport")), MsgHelper.GetMessage("alt: add new collection text"), MsgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
        }
        result.Append("<td class=\"label\">");
        result.Append("<label for=\"txtSearch\">" + MsgHelper.GetMessage("generic search") + "</label>");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<input type=text class=\"ektronTextMedium\" id=txtSearch name=txtSearch value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\" autocomplete=\"off\">");
        result.Append("</td>");
        result.Append("<td><input type=button value=" + MsgHelper.GetMessage("btn Search") + " id=btnSearch name=btnSearch class=\"ektronWorkareaSearch\" onclick=\"searchcollection();\" title=\"Search Collections\"></td>");
        result.Append("</td>");
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("ViewCollectionReport", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void VisiblePageControls(bool flag)
    {
        cTotalPages.Visible = flag;
        cCurrentPage.Visible = flag;
        cPreviousPage.Visible = flag;
        cNextPage.Visible = flag;
        cLastPage.Visible = flag;
        cFirstPage.Visible = flag;
        cPageLabel.Visible = flag;
        cOfLabel.Visible = flag;
    }
    public void CollectionNavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)cTotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)cCurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)cCurrentPage.Text) - 1);
                break;
        }
        if (Request.QueryString["action"] == "ViewCollectionReport")
        {
            LoadCollectionList();
        }
        else
        {
            LoadMenuList();
        }
        isCPostData.Value = "true";
    }
    private void PageSettings()
    {
        if (TotalPagesNumber <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            cTotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
            cTotalPages.ToolTip = cTotalPages.Text;
            cCurrentPage.Text = _currentPageNumber.ToString();
            cCurrentPage.ToolTip = cCurrentPage.Text;
            cPreviousPage.Enabled = true;
            cFirstPage.Enabled = true;
            cNextPage.Enabled = true;
            cLastPage.Enabled = true;
            if (_currentPageNumber == 1)
            {
                cPreviousPage.Enabled = false;
                cFirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                cNextPage.Enabled = false;
                cLastPage.Enabled = false;
            }
        }
    }
}
