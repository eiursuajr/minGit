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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

public partial class selectcreatecontent : System.Web.UI.Page
{

    protected string ContentIcon = "";
    protected string CalendarIcon = string.Empty;
    protected string formsIcon = "";
    protected string AppPath = "";
    protected long FolderId = 0;
    protected long StartingFolderId = 0;
    protected int ContentLanguage = -1;
    protected string actName = "";
    protected string notSupportIFrame = "";
    protected bool bRemoveAddContentBtn = false;
    protected string AppName = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected CalendarAPI m_refCalendarApi;
    protected bool CanCreateContent = false;
    protected PermissionData perm_data;
    protected EkMessageHelper m_refMsg;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected Collection gtNavs;
    protected Collection cTmp;
    protected Collection cFolders;
    protected object FolderName;
    protected object ParentFolderId;
    protected object fPath;
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected bool blnForTasks;
    protected bool blnForWiki = false;
    protected string RedirectFromPage = "";
    protected string strPageAction = "";
    protected string ReportType = "";
    protected EkContentCol cConts;
    protected List<EntryData> entryList = new List<EntryData>();
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected string overrideContentEnum = ""; // 0 = all, 1 = content, 2 = forms
    protected string pleaseSelectMsg = "";
    protected long ItemID = 0;
    protected string callerPage = "";
    protected EkEnumeration.FolderType folderType = EkEnumeration.FolderType.Content;

    //SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType=1033&browser=0
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        m_refMsg = (new Ektron.Cms.CommonApi()).EkMsgRef;
        m_refMsg = m_refSiteApi.EkMsgRef;
        RegisterResources();
        pleaseSelectMsg = m_refMsg.GetMessage("js select content block");
        try
        {
            m_refCalendarApi = new CalendarAPI(m_refContentApi.RequestInformationRef);
            if (m_refContentApi.UserId == 0 || m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }
            blnForTasks = false;
            m_refMsg = m_refContentApi.EkMsgRef;
            //Put user code to initialize the page here
            AppPath = m_refContentApi.AppPath;
            AppName = m_refContentApi.AppName;
            CalendarIcon = "<img src=\"" + AppPath + "images/ui/icons/calendarViewDay.png\" alt=\"Calendar Event\" \">";
            ContentIcon = "<img src=\"" + AppPath + "images/ui/icons/contentHtml.png\"  alt=\"" + m_refMsg.GetMessage("generic content") + "\">";
            formsIcon = "<img src=\"" + AppPath + "images/ui/icons/contentForm.png\"  alt=\"" + m_refMsg.GetMessage("generic form") + "\">";
            if (!string.IsNullOrEmpty(Request.QueryString["FolderID"]))
            {
                FolderId = Convert.ToInt64(Request.QueryString["FolderID"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.QueryString["actionName"]))
            {
                actName = Request.QueryString["actionName"];
            }
            if (actName == null)
            {
                actName = "";
            }
            NextPage.Attributes.Add("onclick", "return resetPostback()");
            lnkBtnPreviousPage.Attributes.Add("onclick", "return resetPostback()");
            FirstPage.Attributes.Add("onclick", "return resetPostback()");
            LastPage.Attributes.Add("onclick", "return resetPostback()");

            if (!(Request.QueryString["ItemID"] == null))
            {
                ItemID = Convert.ToInt64(Request.QueryString["ItemID"]);
            }

            if (!(Request.QueryString["ty"] == null))
            {
                callerPage = Request.QueryString["ty"];
            }

            if (!(Request.QueryString["overrideType"] == null))
            {
                if (Request.QueryString["overrideType"] == "content")
                {
                    overrideContentEnum = "content";
                }
                else if (Request.QueryString["overrideType"].ToLower() == "forms")
                {
                    overrideContentEnum = "forms";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select form").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "blog")
                {
                    overrideContentEnum = "blog";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select blog").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "forum")
                {
                    overrideContentEnum = "forum";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select forum").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "calendar")
                {
                    overrideContentEnum = "calendar";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select calendar").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "collection")
                {
                    overrideContentEnum = "collection";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select collection").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "folder")
                {
                    overrideContentEnum = "folder";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select folder").ToString();
                }
                else if (Request.QueryString["overrideType"].ToLower() == "calfolder")
                {
                    overrideContentEnum = "calfolder";
                    pleaseSelectMsg = m_refMsg.GetMessage("js select folder").ToString();
                }
            }

            if (!(Request.QueryString["StartingFolderID"] == null))
            {
                StartingFolderId = Convert.ToInt64(Request.QueryString["StartingFolderID"]);
            }

            if (IsBrowserIE())
            {
                notSupportIFrame = "0";
            }
            else
            {
                notSupportIFrame = "1";
            }

            StyleSheetJS.Text = m_refStyle.GetClientScript();
            if (Request.QueryString["rmadd"] == "true")
            {
                bRemoveAddContentBtn = true;
            }
            if (!(Request.QueryString["for_tasks"] == null))
            {
                blnForTasks = System.Convert.ToBoolean("1" == Strings.Trim(Request.QueryString["for_tasks"]));
            }
            else if (!(Request.QueryString["for_wiki"] == null))
            {
                blnForWiki = System.Convert.ToBoolean("1" == Strings.Trim(Request.QueryString["for_wiki"]));
                overrideContentEnum = "folder";
                pleaseSelectMsg = m_refMsg.GetMessage("js select folder").ToString();
            }

            if (overrideContentEnum.Length == 0 || overrideContentEnum == "content")
            {
                Page.Title = AppName + " Select Content";
            }
            else if (overrideContentEnum == "forms")
            {
                Page.Title = AppName + " Select Form";
            }
            else if (overrideContentEnum == "blog")
            {
                Page.Title = AppName + " Select Blog";
            }
            else if (overrideContentEnum == "forum")
            {
                Page.Title = AppName + " Select Forum";
            }
            else if (overrideContentEnum == "calendar")
            {
                Page.Title = AppName + " Select Calendar";
            }
            else if (overrideContentEnum == "collection")
            {
                Page.Title = AppName + " Select Collection";
            }
            else if (overrideContentEnum == "folder")
            {
                Page.Title = AppName + " Select Folder";
            }
            else if (overrideContentEnum == "calfolder")
            {
                Page.Title = AppName + " Select Folder";
            }

            if (!(Request.QueryString["LangType"] == null))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
                {
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
            if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContentApi.ContentLanguage = ContentLanguage;
            }

            m_refContent = m_refContentApi.EkContentRef;
            gtNavs = m_refContent.GetFolderInfoWithPath(FolderId);
            FolderName = gtNavs["FolderName"];
            folderType = (EkEnumeration.FolderType)Enum.Parse(typeof(EkEnumeration.FolderType), gtNavs["FolderType"].ToString());
            ltr_folderType.Text = (string)(folderType.ToString());
            ParentFolderId = gtNavs["ParentID"];
            fPath = gtNavs["Path"];

            cTmp = new Collection();
            cTmp.Add("name", "OrderBy", null, null);
            cTmp.Add(FolderId, "FolderID", null, null);
            cTmp.Add(FolderId, "ParentID", null, null);
            cFolders = m_refContent.GetAllViewableChildFoldersv2_0(cTmp);
            if (blnForTasks == true)
            {
                cTmp.Add(true, "FilterContentAssetType", null, null);
            }

            if (!(Request.QueryString["action"] == null))
            {
                strPageAction = Request.QueryString["action"];
            }
            // RedirectFromPage: To display only child folders & NOT content.
            if (!(Request.QueryString["from_page"] == null))
            {
                RedirectFromPage = Request.QueryString["from_page"];
            }
            if (RedirectFromPage != "report" && (IsPostBack == false || isPostData.Value != ""))
            {
                FillContentInfo();
            }
            else if (isPostData.Value != "")
            {
                DrawData();
            }
            isPostData.Value = "true";
            if (!(Request.QueryString["report_type"] == null))
            {
                ReportType = Request.QueryString["report_type"];
            }

            SetJsServerVariables();

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    private void DrawData()
    {
        perm_data = m_refContentApi.LoadPermissions(FolderId, "folder", 0);
        CanCreateContent = perm_data.CanAdd;
        GenerateToolBar();
        PopulateGridData();
    }
    private void FillContentInfo()
    {
        //cConts = m_refContent.GetAllViewableChildContentInfoV4_2(cTmp)
        if (folderType == EkEnumeration.FolderType.Catalog)
        {

            CatalogEntryApi catalogAPI = new CatalogEntryApi();
            Criteria<EntryProperty> criteria = new Criteria<EntryProperty>();

            criteria.PagingInfo = new PagingInfo(m_refContentApi.RequestInformationRef.PagingSize);
            criteria.PagingInfo.CurrentPage = _currentPageNumber;
            criteria.AddFilter(EntryProperty.CatalogId, CriteriaFilterOperator.EqualTo, FolderId);
            criteria.AddFilter(EntryProperty.LanguageId, CriteriaFilterOperator.EqualTo, ContentLanguage);
            criteria.OrderByField = EntryProperty.Title;

            entryList = catalogAPI.GetList(criteria);

            TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);

        }
        else
        {
            cConts = m_refContent.GetAllViewableChildInfov5_0(cTmp, _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber, Ektron.Cms.Common.EkEnumeration.CMSContentType.NonLibraryContent, EkEnumeration.CMSContentSubtype.AllTypes);
        }

        if (overrideContentEnum == "calfolder" || TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {

            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            lnkBtnPreviousPage.Visible = true;
            NextPage.Visible = true;
            LastPage.Visible = true;
            FirstPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;

            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
            TotalPages.ToolTip = TotalPages.Text;

            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;

            if (_currentPageNumber == 1)
            {

                lnkBtnPreviousPage.Enabled = false;

                if (TotalPagesNumber > 1)
                {
                    NextPage.Enabled = true;
                }
                else
                {
                    NextPage.Enabled = false;
                }

            }
            else
            {

                lnkBtnPreviousPage.Enabled = true;

                if (_currentPageNumber == TotalPagesNumber)
                {
                    NextPage.Enabled = false;
                }
                else
                {
                    NextPage.Enabled = true;
                }

            }
        }
        DrawData();
    }

    private string ConditionalInsertSelected(long val1, string title, string quicklink, string teaser)
    {
        if (val1 == ItemID)
        {
            string tmpstr = "SetContentChoice(\'" + ItemID + "\', \'" + title + "\', \'" + quicklink + "\', " + teaser + ");";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkItemIDInitialSet", tmpstr, true);
            return " checked";
        }
        else
        {
            return "";
        }
    }

    private void PopulateGridData()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        bool bDoNotShow = false;
        colBound.DataField = "ITEM1";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM2";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM3";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));

        dr = dt.NewRow();
        if (overrideContentEnum.Length == 0 || overrideContentEnum == "content")
        {
            dr[0] = m_refMsg.GetMessage("alt Please select content by navigating the folders below") + "<br />";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);
        }
        else if (overrideContentEnum == "forms")
        {
            dr[0] = m_refMsg.GetMessage("alt Please select form by navigating the folders below") + "<br />";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);
        }
        else if (overrideContentEnum == "collection")
        {
            dr[0] = m_refMsg.GetMessage("alt Please select collection by navigating the folders below") + "<br />";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);
        }

        dr = dt.NewRow();
        if (overrideContentEnum.Length == 0 || overrideContentEnum == "content" || overrideContentEnum == "forms" || overrideContentEnum == "collection")
        {
            dr[0] = "" + m_refMsg.GetMessage("generic path") + "<span class=\"selectedContent\">" + fPath + "</span>";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);
        }

        if (overrideContentEnum.Length == 0 || overrideContentEnum == "content" || overrideContentEnum == "forms" || overrideContentEnum == "collection" || overrideContentEnum == "folder" || overrideContentEnum == "calfolder")
        {

            if ((FolderId != StartingFolderId) || (RedirectFromPage == "report"))
            {
                // for root folder do not add any folder backup arrow button for other page than reports.
                // reports need a back up button to take to the root folder content report
                dr = dt.NewRow();
                // strPageAction to differentiate between approval and other reports
                // report as one of the input of recursivesubmit to display checkboxes, those are not needed for tasks, etc.
                if ((string)(RedirectFromPage) == "report")
                {
                    if (FolderId != StartingFolderId)
                    {
                        dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + actName + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);return true;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + actName + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);return true;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
                    }
                    else
                    {
                        dr[0] = "<a href=# onclick=\"return SaveSelCreateContent(&quot;" + strPageAction + "&quot;);\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"return SaveSelCreateContent(&quot;" + strPageAction + "&quot;);\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
                    }
                }
                else
                {
                    dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + actName + "&quot;);return true;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + actName + "&quot;);return true;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
                }
                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";
                dt.Rows.Add(dr);
            }


            if (FolderId == 0 && (overrideContentEnum == "folder" || overrideContentEnum == "calfolder"))
            {
                dr = dt.NewRow();
                dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + "0" + ",&quot;" + actName + "&quot;);return true;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + AppPath + "images/ui/icons/folder.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\">" + "Root" + "</a>";
                if (overrideContentEnum == "calfolder")
                {
                    dr[0] = dr[0] + "<input type=\"radio\" name=\"content\" value=\"" + "0" + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + "0" + "\',\'Root\',\'" + "0" + "\');\"" + " />";
                }
                else
                {
                    dr[0] = dr[0] + "<input type=\"radio\" name=\"content\" value=\"" + "0" + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + "0" + "\',\'" + "0" + "\',\'" + "0" + "\');\"" + " />";
                }
                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";
                dt.Rows.Add(dr);
            }

            foreach (Collection folder in cFolders)
            {
                dr = dt.NewRow();

                if ((string)(RedirectFromPage) == "report")
                {
                    if (strPageAction.ToLower() == "siteupdateactivity")
                    {
                        dr[0] = "<input type=\"radio\" name=\"report\"  value=\"" + folder["ID"].ToString() + ":" + fPath + "\\" + folder["Name"] + "\">";
                    }
                    else
                    {
                        dr[0] = "<input type=\"radio\" name=\"report\"  value=\"" + folder["ID"].ToString() + "\">";
                    }
                    dr[0] += "<a href=# onclick=\"RecursiveSubmit(" + folder["id"].ToString() + ",&quot;" + actName + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);\"return true;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">" + folder["Name"] + "</a>";
                }
                else
                {
                    //No check boxes in case of other folders
                    if (FolderId == 0 && (overrideContentEnum == "folder" || overrideContentEnum == "calfolder"))
                    {
                        dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        dr[0] = "";
                    }
                    if (folder["FolderType"].ToString() == Convert.ToInt32(EkEnumeration.FolderType.Calendar).ToString())
                    {
                        dr[0] = dr[0] + "<a href=\"#\" onclick=\"RecursiveSubmit(" + folder["ID"].ToString() + ",\'" + actName.Replace("\'", "\\\'") + "\');return true;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + AppPath + "images/ui/icons/foldercalendar.png\" alt=\"Calendar\"\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=\"#\" onclick=\"RecursiveSubmit(" + folder["ID"].ToString() + ",\'" + actName.Replace("\'", "\\\'") + "\');return true;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
                    }
                    else 
                    {
                        dr[0] = dr[0] + "<a href=\"#\" onclick=\"RecursiveSubmit(" + folder["ID"].ToString() + ",\'" + actName.Replace("\'", "\\\'") + "\');return true;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + Utilities.GetFolderImage((EkEnumeration.FolderType)folder["FolderType"], AppPath) + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=\"#\" onclick=\"RecursiveSubmit(" + folder["ID"].ToString() + ",\'" + actName.Replace("\'", "\\\'") + "\');return true;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
                    }
                    if (overrideContentEnum == "calfolder")
                    {
                        dr[0] = dr[0] + "<input type=\"radio\" name=\"content\" value=\"" + folder["ID"].ToString() + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + folder["ID"].ToString() + "\',\'" + MakeStringJSSafe((string)(folder["Name"])) + "\',\'" + folder["ID"].ToString() + "\');\"" + ConditionalInsertSelected(Convert.ToInt64(folder["ID"].ToString()), MakeStringJSSafe(folder["ID"].ToString()), folder["ID"].ToString(), "") + " />";
                    }
                    else if (overrideContentEnum == "folder")
                    {
                        dr[0] = dr[0] + "<input type=\"radio\" name=\"content\" value=\"" + folder["ID"].ToString() + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + folder["ID"].ToString() + "\',\'" + MakeStringJSSafe(folder["ID"].ToString()) + "\',\'" + folder["ID"].ToString() + "\');\"" + ConditionalInsertSelected(Convert.ToInt64(folder["ID"].ToString()), MakeStringJSSafe(folder["ID"].ToString()), folder["ID"].ToString(), "") + " />";
                    }
                }

                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";
                dt.Rows.Add(dr);
            }

            string ContentName = "";

            // For displaying child content - exclude this for reports
            if (RedirectFromPage != "report")
            {
                if (overrideContentEnum.Length == 0 || overrideContentEnum == "content" || overrideContentEnum == "forms")
                {
                    if (folderType == EkEnumeration.FolderType.Catalog)
                    {

                        foreach (EntryData product in entryList)
                        {

                            if (m_refContentApi.RequestInformationRef.LinkManagement)
                            {
                                ContentName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + product.Id;
                            }
                            else
                            {
                                ContentName = m_refContent.GetContentQlink(product.Id, product.FolderId);
                            }

                            ContentName = ContentName.Replace("\'", "\\\'");
                            dr = dt.NewRow();
                            dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<textarea style=\"position:absolute; left:-1000px; top:-500px;\" id=\"teaser_" + product.Id + "\" >" + product.Summary + "</textarea>" + GetProductIcon(product.EntryType, AppPath) + "<input type=\"radio\" name=\"content\" value=\"" + product.Id + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + product.Id + "\',\'" + MakeStringJSSafe(product.Title) + "\',\'" + ContentName + "\',\'teaser_" + product.Id + "\');\"" + ConditionalInsertSelected(product.Id, MakeStringJSSafe(product.Title), ContentName, "\'teaser_" + product.Id + "\'") + " />" + product.Title + "<br />";
                            dr[1] = "remove-item";
                            dr[2] = "remove-item";
                            dt.Rows.Add(dr);

                        }

                    }
                    else
                    {

                        foreach (ContentBase contInfo in cConts)
                        {
                            bDoNotShow = false;
                            if (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms)
                            {
                                if (m_refContentApi.RequestInformationRef.LinkManagement)
                                {
                                    ContentName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + contInfo.Id;
                                }
                                else
                                {
                                    ContentName = m_refContent.GetContentFormlink(contInfo.Id, contInfo.FolderId);
                                }
                            }
                            else if ((((Int32)contInfo.ContentType >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min && (Int32)contInfo.ContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Assets)) && (contInfo.ContentType != Ektron.Cms.Common.EkEnumeration.CMSContentType.Multimedia))
                            {
                                if (m_refContentApi.RequestInformationRef.LinkManagement)
                                {
                                    ContentName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + contInfo.Id;
                                }
                                else
                                {
                                    ContentName = m_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId);
                                }
                            }
                            else if ((contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content) || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Multimedia))
                            {
                                if (m_refContentApi.RequestInformationRef.LinkManagement)
                                {

                                    ContentName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + contInfo.Id;
                                }
                                else
                                {
                                    ContentName = m_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId);
                                }
                            }
                            else
                            {
                                //do not show
                                bDoNotShow = true;
                            }
                            ContentName = ContentName.Replace("\'", "\\\'");
                            if (!(bDoNotShow))
                            {
                                dr = dt.NewRow();
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;<textarea style=\"position:absolute; left:-1000px; top:-500px;\" id=\"teaser_" + contInfo.Id + "\" >" + contInfo.Teaser + "</textarea>" + getContentTypeIcon(contInfo) + "<input type=\"radio\" name=\"content\" value=\"" + contInfo.Id + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + contInfo.Id + "\',\'" + MakeStringJSSafe(contInfo.Title) + "\',\'" + ContentName + "\',\'teaser_" + contInfo.Id + "\');\"" + ConditionalInsertSelected(contInfo.Id, MakeStringJSSafe(contInfo.Title), ContentName, "\'teaser_" + contInfo.Id + "\'") + " />" + contInfo.Title + "<br />";
                                dr[1] = "remove-item";
                                dr[2] = "remove-item";
                                if (overrideContentEnum.Length == 0 || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content && overrideContentEnum == "content") || (contInfo.ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms && overrideContentEnum == "forms"))
                                {
                                    dt.Rows.Add(dr);
                                }
                            }
                        }

                    }

                }
                else if (overrideContentEnum == "collection")
                {
                    int temp_called_id = Convert.ToInt32(m_refContentApi.RequestInformationRef.CallerId);
                    m_refContentApi.RequestInformationRef.CallerId = EkConstants.InternalAdmin;
                    CollectionListData[] collections = m_refContentApi.EkContentRef.GetCollectionList();
                    m_refContentApi.RequestInformationRef.CallerId = temp_called_id;
                    if (collections != null)
                    {
                        foreach (CollectionListData col in collections)
                        {
                            if (col.FolderId == FolderId)
                            {
                                dr = dt.NewRow();
                                dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + "<img src=\"" + m_refContentApi.AppPath + "images/ui/icons/collection.png\" />" + "<input type=\"radio\" name=\"content\" value=\"" + col.Id + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + col.Id + "\',\'" + MakeStringJSSafe(col.Id.ToString()) + "\',\'" + MakeStringJSSafe(col.Id.ToString()) + "\');\"" + ConditionalInsertSelected(col.Id, MakeStringJSSafe(col.Id.ToString()), col.Id.ToString(), "") + ">" + col.Title + "<br />";
                                dr[1] = "remove-item";
                                dr[2] = "remove-item";
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }

            }
        }
        else if (overrideContentEnum == "calendar")
        {
            if (RedirectFromPage != "report")
            {
                CalendarData[] calList = m_refCalendarApi.LoadAllCalendars();
                foreach (CalendarData cal in calList)
                {
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + ("<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/calendarView.png\" />") + "<input type=\"radio\" name=\"content\" value=\"" + cal.CalendarID + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + cal.CalendarID + "\',\'" + MakeStringJSSafe(cal.CalendarID.ToString()) + "\',\'" + cal.CalendarID + "\');\"" + ConditionalInsertSelected(cal.CalendarID, MakeStringJSSafe(cal.CalendarID.ToString()), cal.CalendarID.ToString(), "") + ">" + cal.CalendarTitle + "<br />";
                    dr[1] = "remove-item";
                    dr[2] = "remove-item";
                    dt.Rows.Add(dr);
                }
            }
        }
        else if (overrideContentEnum == "blog")
        {
            if (RedirectFromPage != "report")
            {
                FolderData[] blogs = m_refContentApi.GetAllBlogs();
                foreach (FolderData blog in blogs)
                {
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + ("<img src=\"" + m_refContentApi.RequestInformationRef.ApplicationPath + "/images/ui/icons/tree/folderBlog.png\" />") + "<input type=\"radio\" name=\"content\" value=\"" + blog.Id + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + blog.Id + "\',\'" + MakeStringJSSafe(blog.Id.ToString()) + "\',\'" + blog.Id + "\');\"" + ConditionalInsertSelected(blog.Id, MakeStringJSSafe(blog.Id.ToString()), blog.Id.ToString(), "") + ">" + blog.Name + "<br />";
                    dr[1] = "remove-item";
                    dr[2] = "remove-item";
                    dt.Rows.Add(dr);
                }
            }
        }
        else if (overrideContentEnum == "forum")
        {
            if (RedirectFromPage != "report")
            {
                FolderData[] forums = m_refContentApi.GetAllForums();
                foreach (FolderData forum in forums)
                {
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;&nbsp;&nbsp;" + ("<img src=\"" + m_refContentApi.RequestInformationRef.ApplicationPath + "/images/ui/icons/tree/folderBlog.png\" />") + "<input type=\"radio\" name=\"content\" value=\"" + forum.Id + "\" ID=\"content\" onclick=\"SetContentChoice(\'" + forum.Id + "\',\'" + MakeStringJSSafe(forum.Id.ToString()) + "\',\'" + forum.Id + "\');\"" + ConditionalInsertSelected(forum.Id, MakeStringJSSafe(forum.Id.ToString()), forum.Id.ToString(), "") + ">" + forum.Name + "<br />";
                    dr[1] = "remove-item";
                    dr[2] = "remove-item";
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        ContentGrid.DataSource = dv;
        ContentGrid.DataBind();
    }
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("remove-item") && e.Item.Cells[2].Text.Equals("remove-item"))
                {
                    e.Item.Cells[0].ColumnSpan = 3;
                    e.Item.Cells.RemoveAt(2);
                    e.Item.Cells.RemoveAt(1);
                }
                break;
        }

    }
    private void GenerateToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if ((RedirectFromPage == "report") || this.blnForWiki == true)
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select folder"));
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Select Content"));
        }
        result.Append("<table><tr>");

		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/cancel.png", "#", m_refMsg.GetMessage("alt exit without selecting content"), m_refMsg.GetMessage("btn cancel"), "onclick=\"CancelSelContent();\"", StyleHelper.CancelButtonCssClass, true));

		bool primaryCssApplied = false;
		
		if (bRemoveAddContentBtn)
        {
            if (CanCreateContent)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add content button text"), m_refMsg.GetMessage("btn add content"), "onclick=\"PopUpWindow(\'editarea.aspx?type=add&dontcreatetask=1&id=" + FolderId + "\', \'Edit\', 790, 580, 1, 1);return false;\" ", StyleHelper.AddButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
            }
        }
        //This condition can be removed safely as non other calls to this page has action set.
        if (RedirectFromPage == "report")
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/save.png", "#", m_refMsg.GetMessage("lbl click here to save the event"), m_refMsg.GetMessage("btn save"), "onclick=\"return SaveSelCreateContent(\'" + strPageAction + "\',\'" + FolderId + "\');\"", StyleHelper.SaveButtonCssClass, !primaryCssApplied));
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/save.png", "#", m_refMsg.GetMessage("lbl click here to save the event"), m_refMsg.GetMessage("btn save"), "onclick=\"return SaveSelCreateContent();\"", StyleHelper.SaveButtonCssClass, !primaryCssApplied));
        }

		primaryCssApplied = true;

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    public string GetProductIcon(EkEnumeration.CatalogEntryType type, string applicationImagePath)
    {

        return "<img src=\"" + Utilities.GetProductImage(type, applicationImagePath) + "\"  alt=\"Content\">" + "&nbsp;&nbsp;";

    }
    public string getContentTypeIcon(ContentBase objCont)
    {
        try
        {
            int ContentTypeID;
            string strAssetIcon;

            ContentTypeID = System.Convert.ToInt32(objCont.ContentType);
            if (ContentTypeID == 2)
            {
                return (formsIcon);
            }
            else if (ContentTypeID > Ektron.Cms.Common.EkConstants.ManagedAsset_Min && ContentTypeID < Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
            {
                try
                {
                    strAssetIcon = (string)objCont.AssetInfo.ImageUrl;
                    strAssetIcon = "<img src=\"" + strAssetIcon + "\"  alt=\"Asset\">";
                    return (strAssetIcon);
                }
                catch (Exception)
                {
                    return (ContentIcon);
                }
            }
            else if (objCont.ContentSubType == EkEnumeration.CMSContentSubtype.WebEvent)
            {
                return CalendarIcon;
            }
            else
            {
                return (ContentIcon);
            }
        }
        catch (Exception)
        {
            return (ContentIcon);
        }
    }
    public string MakeStringJSSafe(string str)
    {
        return str.Replace("&#39;", "\'").Replace("\'", "\\\'");
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        FillContentInfo();
    }

    private bool IsBrowserIE()
    {
        bool returnValue;
        string strVariable = Request.ServerVariables["HTTP_USER_AGENT"].ToLower();
        returnValue = System.Convert.ToBoolean(strVariable.IndexOf("mozilla") + 1 > 0);
        return returnValue;
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    private void SetJsServerVariables()
    {
        ltr_folderId.Text = FolderId.ToString();
        ltr_ContentLanguage.Text = ContentLanguage.ToString();
        ltr_pleaseSelectMsg.Text = pleaseSelectMsg;
        ltr_ItemID.Text = ItemID.ToString();
        ltr_callerPage.Text = callerPage;
        ltr_notSupportIFrame.Text = notSupportIFrame;
        ltr_notSupportIFrameCancel.Text = notSupportIFrame;
        ltr_overrideContentEnum.Text = overrideContentEnum;
        ltr_StartingFolderID.Text = StartingFolderId.ToString();
        ltr_ContentLanguageRecursive.Text = ContentLanguage.ToString();
    }
}
