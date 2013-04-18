using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;
using System.Collections.Generic;

public partial class mediainsert : System.Web.UI.Page
{

    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;

    protected string action = "";
    protected EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected long folder = 0;
    protected string LibType ="";
    protected PermissionData cPerms;
    protected FolderData cFolder;
    protected Collection TypeCounts;
    protected string SitePath = "";
    protected string sEditor = "";
    protected int DEntryLink;
    protected string[] arr_TdName;
    protected int j = 0;
    protected string m_QueryString = "";
    protected bool UseLinkItPage = true;

    private Ektron.Cms.Library.EkLibrary m_refLib=new Ektron.Cms.Library.EkLibrary();
    private int m_intContentLanguage = 0;
    private bool blnSelectAll = false;
    protected bool IsMac = false;

    protected string m_strEnhancedMetaSelect = "";
    protected string m_strMetadataFormTagId = "";
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected string sLinkText = "";
    protected bool showThumbnail = true;
    protected string contLangID = string.Empty;
    protected long autoNavFolder = 0;
    protected bool IsRootFolder = false;
    private Collection _PageData = null;
    protected string caller = string.Empty;
    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        RegisterResources();
        try
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;

            string QueryLibType = "";
            jsLink1.Text = Request.ServerVariables["PATH_INFO"] + "?" + Ektron.Cms.API.JS.Escape(Request.ServerVariables["QUERY_STRING"].Replace("LangType", "L").Replace("SelectAll=1&", "")) + "&LangType=";

            m_refMsg = m_refContentApi.EkMsgRef;
            AppImgPath = m_refContentApi.AppImgPath;
            AppPath = m_refContentApi.AppPath;
            SitePath = m_refContentApi.SitePath;
            m_refLib = m_refContentApi.EkLibraryRef;
            UseLinkItPage = m_refContentApi.RequestInformationRef.LinkManagement;
            sDeleteLibItem.Text = m_refMsg.GetMessage("js: confirm delete lib item");

			Utilities.ValidateUserLogin();
            if (m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            if (Request.QueryString["enhancedmetaselect"] != null)
            {
                m_strEnhancedMetaSelect = Request.QueryString["enhancedmetaselect"];
            }
            if (Request.QueryString["metadataformtagid"] != null)
            {
                m_strMetadataFormTagId = Request.QueryString["metadataformtagid"];
            }

            if (!String.IsNullOrEmpty(Request.QueryString["autonavfolder"]))
            {
                autoNavFolder = long.Parse(Request.QueryString["autonavfolder"].ToString());
                IsRootFolder = System.Convert.ToBoolean(autoNavFolder == 0 ? true : false);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["caller"]))
            {
                caller = "&caller=" + Request.QueryString["caller"];
            }
            if (Request.QueryString["LangType"] != null)
            {
                if (Request.QueryString["LangType"] != "")
                {
                    m_intContentLanguage = System.Convert.ToInt32(Request.QueryString["LangType"]);
                }
                else if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                        m_intContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
            else if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                    m_intContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
           
            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContentApi.ContentLanguage = m_intContentLanguage;
            }
            contLangID = m_refContentApi.ContentLanguage.ToString();
            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                IsMac = true;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["selected"]))
            {
                sLinkText = Request.QueryString["selected"];
            }
            if (Request.QueryString["SelectAll"] != null)
            {
                blnSelectAll = true;
            }
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            if (Request.QueryString["scope"] == "images")
            {
                LibType = "images";
                Session["LibCategory"] = LibType;
            }
            else
            {
                if (Request.QueryString["type"] != null)
                {
                    LibType = Request.QueryString["type"].ToString();
                    Session["LibCategory"] = LibType;
                }
                else
                {
                    LibType =  Session["LibCategory"].ToString();
                }

                if (LibType == "")
                {
                    LibType = "images";
                    Session["LibCategory"] = LibType;
                }
            }

            if ((!(LibType == "quicklinks" || LibType == "forms" || LibType == "hyperlinks")) && blnSelectAll == true)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["action"]))
            {
                    action = Request.QueryString["action"].ToLower();
            }

            foreach (string item in Request.QueryString.AllKeys)
            {
                if (m_QueryString == "")
                {
                    m_QueryString = item + "=" + Request.QueryString[item];
                }
                else
                {
                    m_QueryString += (string)("&" + item + "=" + Request.QueryString[item]);
                }
            }
            if (Request.QueryString["showthumb"] != null)
            {
                if (Request.QueryString["showthumb"] == "false")
                {
                    showThumbnail = false;
                }
            }
            m_QueryString = m_QueryString.Replace("SelectAll=1&", "");
            sEditor = Request.QueryString["EditorName"];            
            
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                folder = long.Parse(Request.QueryString["id"].ToString());
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                folder = long.Parse(Request.QueryString["folder"].ToString());
            }
            if (!String.IsNullOrEmpty(Request.QueryString["autonavfolder"]))
            {
                autoNavFolder = long.Parse(Request.QueryString["autonavfolder"].ToString());
                Session["AutoNavFolder" + "_" + folder] = autoNavFolder;
            }
            else
            {
                Session["AutoNavFolder" + "_" + folder] = folder;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["dentrylink"]))
            {
                DEntryLink = int.Parse(Request.QueryString["dentrylink"].ToString());
            }

            if (action == "viewlibrarycategory" && !Page.IsPostBack)
            {
                Display_ViewLibraryCategory();
            }
            else if (action == "viewlibrarybycategory" && !Page.IsPostBack)
            {
                Display_ViewLibraryByCategory();
            }
            else if (action == "viewlibrarybycategory" && (Request.Form["aliassubmit"] != null) && Request.Form["aliassubmit"].ToLower() == "ok")
            {
                Display_ViewLibraryByCategory();
            }
            else if (action == "deleteitem")
            {
                Process_DeleteItem();
            }
            else if (action == "addlibraryitem")
            {
                if (Request.QueryString["type"] == null)
                {
                    QueryLibType = (string)("&type=" + LibType);
                }
                Response.Redirect((string)("mediauploader.aspx?" + m_QueryString + QueryLibType), false);
            }

            //Adding the MediaUploaderCommon User Control
            MediaUploaderCommon m_Muc;
            m_Muc = (MediaUploaderCommon)(LoadControl("controls/library/MediaUploaderCommon.ascx"));
            m_Muc.ID = "MediaUploaderCommon";
            DataHolder.Controls.Add(m_Muc);

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    private void Display_ViewLibraryCategory()
    {
        LibraryTypeData[] gtLibraries;
        Ektron.Cms.Content.EkContent m_refcontent;
        m_refcontent = m_refContentApi.EkContentRef;
        TypeCounts = m_refcontent.GetLibraryTypeCount(folder);
        cFolder = m_refContentApi.GetFolderById(folder);
        gtLibraries = m_refContentApi.GetLibraryTypes(folder, Request.QueryString["orderby"]);
        cPerms = m_refContentApi.LoadPermissions(folder, "folder", 0);
        ViewLibraryCategoryToolBar();
        UpdFld.Text = "<script type=\'text/javascript\'>";
        UpdFld.Text += "updateFolders(" + folder + ", \"\", " + System.Convert.ToInt32(cPerms.CanAddToImageLib) + ", " + System.Convert.ToInt32(cPerms.CanAddToFileLib) + ", " + System.Convert.ToInt32(cPerms.CanOverwriteLib) + ", \"\");";
        UpdFld.Text += "</script>";
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=LibraryTypeName&id=" + folder + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        MediaListGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=LibraryTypeID&id=" + folder + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        MediaListGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));

        int i;
        bool outputfolder;

        if (!(gtLibraries == null))
        {
            for (i = 0; i <= gtLibraries.Length - 1; i++)
            {
                outputfolder = false;
                if ((Request.QueryString["scope"] != "images") || (gtLibraries[i].Name == "images"))
                {
                    outputfolder = true;
                }
                if (outputfolder)
                {
                    dr = dt.NewRow();
                    dr[0] = "<a onclick=\"updateFolders(" + folder + ", \'" + gtLibraries[i].Name + "\', " + System.Convert.ToInt32(cPerms.CanAddToImageLib) + ", " + System.Convert.ToInt32(cPerms.CanAddToFileLib) + ", " + System.Convert.ToInt32(cPerms.CanOverwriteLib) + ", \'\');\" href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&id=" + folder + "&type=" + gtLibraries[i].Name + "\" title=\'" + m_refMsg.GetMessage("generic View") + "\"" + Strings.Replace(gtLibraries[i].Name, "\'", "`", 1, -1, 0) + "\"" + ")\'>" + "<img src=\"" + AppImgPath + "folderclosed_1.gif\" border=\"0\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(gtLibraries[i].Name, "\'", "`", 1, -1, 0) + "\"" + ")\' alt=\'" + m_refMsg.GetMessage("generic View") + " \"" + Strings.Replace(gtLibraries[i].Name, "\'", "`", 1, -1, 0) + "\"" + ")\' align=\"absbottom\">" + gtLibraries[i].Name + "</a>";
                    int CountVal = 0;
                    if (TypeCounts.Count > 0)
                    {
                        foreach (Collection TypeCount in TypeCounts)
                        {
                            if (gtLibraries[i].Id == System.Convert.ToInt32(TypeCount["LIBTYPE_ID"]))
                            {
                                CountVal =Convert.ToInt32(TypeCount["LIBTYPE_COUNT"]);
                                break;
                            }
                        }
                    }
                    dr[1] = CountVal;
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        MediaListGrid.DataSource = dv;
        MediaListGrid.DataBind();
    }
    private void Display_ViewLibraryByCategory()
    {
        LibraryData[] result;
        Ektron.Cms.Content.EkContent m_refcontent;
        Collection cTmp = new Collection();
        DataTable dt = new DataTable();
        DataRow dr;
        int i = 0;
        string QLinkDisplayName;
        string TdName = "";
        string strLibraryFileName = "";
        string sDesLoc;
        string[] myfilePathArray;
        string myImagefile;
        string myNewImagefile;
        string sExt;
        string originalfile;
        System.IO.FileInfo fs = null;
        bool isQuicklinks;
        bool isForms;
        bool isFiles;
        bool isHyperlinks;
        string strEnhancedmetaselect = string.Empty;

        m_refcontent = m_refContentApi.EkContentRef;

        isQuicklinks = System.Convert.ToBoolean("quicklinks" == LibType);
        isForms = System.Convert.ToBoolean("forms" == LibType);
        isFiles = System.Convert.ToBoolean("files" == LibType);
        isHyperlinks = System.Convert.ToBoolean("hyperlinks" == LibType);

        if (autoNavFolder == 0)
        {
            if (IsRootFolder)
            {
                Session["AutoNavFolder" + "_" + folder] = null;
            }
            else
            {
                Session["AutoNavFolder" + "_" + folder] = folder;
            }
        }
        else
        {
            Session["AutoNavFolder" + "_" + folder] = autoNavFolder;
        }
        result = m_refLib.GetAllChildLibItemsByTypev5_0(LibType, folder, Request.QueryString["orderby"], _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, true, ref TotalPagesNumber);

        //Fix for Assets in a multisite enviroment [mediainsert.aspx.vb]
        Microsoft.VisualBasic.Collection fCol = new Microsoft.VisualBasic.Collection();
        Ektron.Cms.API.Folder fapi = new Ektron.Cms.API.Folder();
        Ektron.Cms.FolderData[] fdata;
        fdata = m_refContentApi.GetChildFolders(0, false, EkEnumeration.FolderOrderBy.Name);
        if (fdata != null)
        {
            foreach (FolderData thisfdata in fdata)
            {
                if (thisfdata.FolderType == 2)
                {
                    fCol = m_refcontent.GetFolderInfov2_0(thisfdata.Id);
                    foreach (LibraryData thisresult in result)
                    {
                        if (fCol["DomainProduction"].ToString() != "" && thisresult.FileName.IndexOf(fCol["DomainProduction"].ToString()) != -1)
                        {                            
                            string slashCheck = thisresult.FileName.Replace((string)("http://" + fCol["DomainProduction"]), "");
                            if (slashCheck != "" && slashCheck.Substring(0, 1) != "/")
                            {
                                thisresult.FileName = thisresult.FileName.Replace("http://" + fCol["DomainProduction"], "http://" + fCol["DomainProduction"] + "/");
                            }
                            else
                            {
                                thisresult.FileName = slashCheck;
                            }
                        }
                        thisresult.FileName = Regex.Replace(thisresult.FileName, "http://" + fCol["DomainProduction"] + "javascript", "javascript");

                    }
                }
            }
        }
        //End fix for multisite and Assets

        if (TotalPagesNumber <= 1)
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
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(TotalPagesNumber))).ToString();
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

        TypeCounts = m_refcontent.GetLibraryTypeCount(folder);
        cFolder = m_refContentApi.GetFolderById(folder);
        cPerms = m_refContentApi.LoadPermissions(folder, "folder", 0);
        ViewLibraryByCategoryToolBar();
        strEnhancedmetaselect = Request.QueryString["enhancedmetaselect"];

        // Title:
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM1";
        if (!((Request.QueryString["enhancedmetaselect"] == null)) && strEnhancedmetaselect.Length != 0)
        {
            colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&enhancedmetaselect=1" + "&scope=" + Request.QueryString["scope"].ToString() + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=LibraryTitle&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
        }
        else
        {
            colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=LibraryTitle&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
        }
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        MediaListGrid.Columns.Add(colBound);
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));

        // ID:
        if (!(isQuicklinks || isForms))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ITEM2";
            colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=LibraryID&&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic ID") + "</a>";
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            MediaListGrid.Columns.Add(colBound);
            dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        }

        // Language:
        if (isQuicklinks || isForms)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ITEM3";
            if (!((Request.QueryString["enhancedmetaselect"] == null)) && strEnhancedmetaselect.Length != 0)
            {
                colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&enhancedmetaselect=1" + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=ContentLanguage&&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + "Language" + "</a>";
            }
            else
            {
                colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=ContentLanguage&&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + "Language" + "</a>";
            }
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            MediaListGrid.Columns.Add(colBound);
            dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));
        }

        // Date modified:
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM4";
        if (!((Request.QueryString["enhancedmetaselect"] == null)) && strEnhancedmetaselect.Length != 0)
        {
            colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&enhancedmetaselect=1" + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=date&&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + "Date modified" + "</a>";
        }
        else
        {
            colBound.HeaderText = "<a href=\"mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&scope=" + Request.QueryString["scope"].ToString() + "&dentrylink=" + DEntryLink + "&fullscreen=off&orderby=date&&id=" + folder + "&type=" + LibType + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + "Date modified" + "</a>";
        }
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        MediaListGrid.Columns.Add(colBound);
        dt.Columns.Add(new DataColumn("ITEM4", typeof(string)));

        foreach (LibraryData myLibrary in result)
        {
            bool IsAsset = EkConstants.IsAssetContentType(myLibrary.ContentType, true);
            if (!(IsMac && IsAsset))
            {
                string FileExtension = System.IO.Path.GetExtension(myLibrary.FileName);
                Array.Resize(ref arr_TdName, i + 1);
                TdName = (string)("cell" + myLibrary.Id);

                QLinkDisplayName = myLibrary.Title;

                dr = dt.NewRow();

                strLibraryFileName = (string)(Convert.ToString(Server.HtmlDecode(myLibrary.FileName)));
                if (strLibraryFileName.StartsWith("/"))
                {
					if (!myLibrary.FileName.ToLower().Contains(SitePath.ToLower()))
                    {
						strLibraryFileName = strLibraryFileName.Substring(1);
						strLibraryFileName = SitePath + strLibraryFileName;
					}
                }
                if (isQuicklinks || isForms)
                {
                    // see if content is in a domain folder which always use linkit.aspx
                    //Dim domain As String = m_refContentApi.GetDomainByContentId(myLibrary.ContentId)

                    if (UseLinkItPage)
                    {
                        // if content is in a domain folder, linkit.aspx will redirect to the proper domain
                        if (isForms)
                        {
                            if (1 == DEntryLink)
                            {
                                //workaround for any form filelink added to the redirection post message from the library
                                //this would allow the application to add the AppPath when the form is redirected.
                                strLibraryFileName = (string)("linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + myLibrary.ContentId);
                            }
                            else
                            {
                                strLibraryFileName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + myLibrary.ContentId;
                            }
                        }
                        else
                        {
                            strLibraryFileName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + myLibrary.ContentId;
                        }
                    }

                }
                else if (Request.QueryString["scope"] == "" && !(isQuicklinks || isForms))
                {
                    strLibraryFileName = myLibrary.ContentId.ToString() ;
                }

                string thumbName=string.Empty;
                string strRelativeFilename;
                strRelativeFilename = myLibrary.FileName;
                if (strRelativeFilename == null)
                {
                    strRelativeFilename = "";
                }
                else if (strRelativeFilename.StartsWith(SitePath))
                {
                    strRelativeFilename = strRelativeFilename.Substring(SitePath.Length);
                }
                myLibrary.Title = Server.HtmlDecode(myLibrary.Title);
                arr_TdName[i] = TdName;
                if (isQuicklinks || isForms || isFiles || isHyperlinks)
                {
                    dr["ITEM1"] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\"ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');SubmitInsert();\"" + " >" + "\r\n";
                    dr["ITEM1"] += QLinkDisplayName + "</a>" + "\r\n";
                    dr["ITEM1"] += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "\r\n";
                    dr["ITEM1"] += strRelativeFilename;
                }
                else
                {
                    if (IsAsset && Request.QueryString["scope"] != "")
                    {
                        strRelativeFilename = strLibraryFileName;
                        thumbName = strLibraryFileName;
                    }
                    else
                    {
                        thumbName = MakeStringJSSafe(EkFunctions.GetThumbnailForContent(strLibraryFileName));
                    }
                    if (thumbName.StartsWith("/"))
                    {
						if (!thumbName.ToLower().Contains(SitePath.ToLower()))
                        {
							thumbName = thumbName.Substring(1);
							thumbName = SitePath + thumbName;
						}
                    }
                    dr["ITEM1"] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"ThumbnailForContentImage(\'" + thumbName + "\');Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');SubmitInsert();\"" + " >" + "\r\n";
                    dr["ITEM1"] += QLinkDisplayName + "  [" + strRelativeFilename + "]";
                    dr["ITEM1"] += "</a>" + "\r\n";
                }
                if (!(isQuicklinks || isForms))
                {
                    dr["ITEM2"] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"ThumbnailForContentImage(\'" + thumbName + "\');Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');SubmitInsert();\"" + " >" + "\r\n";
                    dr["ITEM2"] += Convert.ToString(myLibrary.Id) + "\r\n";
                    dr["ITEM2"] += "</a>" + "\r\n";
                }
                if (isQuicklinks || isForms)
                {
                    dr["ITEM3"] += "<a href=\"#\" title=\"double click to insert msg\" onclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');SubmitInsert();\"" + " ></a>" + "\r\n";
                    dr["ITEM3"] += "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(myLibrary.LanguageId) + "\' />" + "\r\n";
                }
                if (!(isQuicklinks || isForms))
                {
                    dr["ITEM4"] += "<a href=\"#\"  title=\"double click to insert msg\" onclick=\"ThumbnailForContentImage(\'" + thumbName + "\');Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');;Blink(\'" + TdName + "\', \'yellow\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + MakeStringJSSafe(strLibraryFileName) + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');SubmitInsert();\"" + " >" + "\r\n";
                    dr["ITEM4"] += myLibrary.DisplayLastEditDate + "\r\n";
                    dr["ITEM4"] += "</a>" + "\r\n";
                }
                dt.Rows.Add(dr);

                if (LibType == "images")
                {
                    sDesLoc = myLibrary.StagingFileName;
                    if (sDesLoc == "")
                    {
                        sDesLoc = myLibrary.FileName;
                    }
                    originalfile = myLibrary.FileName;
                    myfilePathArray = sDesLoc.Split('/');
                    myImagefile = myfilePathArray[(myfilePathArray.Length - 1)];

                    sExt = myImagefile.Substring(myImagefile.Length - 3, 3);
                    if ("gif" == sExt || "GIF" == sExt)
                    {
                        sExt = "png";
                        myNewImagefile = "thumb_" + myImagefile.Substring(0, myImagefile.Length - 3) + "png";
                    }
                    else
                    {
                        myNewImagefile = (string)("thumb_" + myImagefile);
                    }
                    try
                    {
                        sDesLoc = sDesLoc.Replace(myImagefile, myNewImagefile);

                        FolderData folder_data = this.m_refContentApi.GetFolderById(this.folder);

                        if (folder_data.IsDomainFolder)
                        {
                            fs = null;
                        }
                        else if (folder_data.DomainProduction == "")
                        {
                            fs = new FileInfo(Server.MapPath(sDesLoc));
                        }
                        if (fs == null || fs.Exists || IsAsset)
                        {
                                if (sDesLoc.StartsWith("/"))
                                {
                                    if (!sDesLoc.ToLower().Contains(SitePath.ToLower()))
                                    {
                                        sDesLoc = sDesLoc.Substring(1);
                                        sDesLoc = m_refContentApi.SitePath + sDesLoc;
                                    }  
                                }                                                             
                            
                            dr = dt.NewRow();
                            string libFileName;
                            if (Request.QueryString["scope"] == "")
                            {
                                libFileName = myLibrary.ContentId.ToString();
                            }
                            else
                            {
                                if (IsAsset)
                                {
                                    libFileName = strLibraryFileName;
                                    originalfile = libFileName;
                                }
                                else
                                {
                                    libFileName = myLibrary.FileName.Replace("\'", "\\\'");
                                    if (libFileName.StartsWith("/"))
                                    {
                                        if (!myLibrary.FileName.ToLower().Contains(SitePath.ToLower()))
                                        {
                                            libFileName = libFileName.Substring(1);
                                            libFileName = SitePath + libFileName;
                                        }
                                    }
                                }
                                System.Random rand = new System.Random();
                                if ("image" == LibType)
                                {
                                    libFileName += "?n=" + rand.Next(0, 1000);
                                }
                            }
                            if (originalfile.StartsWith("/"))
                            {
                                if (!originalfile.ToLower().Contains(SitePath.ToLower()))
                                {
                                    originalfile = originalfile.Substring(1);
                                    originalfile = SitePath + originalfile;
                                }
                            }
                            if ((m_strEnhancedMetaSelect.Length == 0) && DEntryLink == 0 && sEditor != "JSEditor")
                            {
                                dr["ITEM1"] = "<a href=\"#\"  title=\"" + m_refMsg.GetMessage("double click to insert msg") + "\" onclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + libFileName + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');ThumbnailForContentImage(\'" + thumbName + "\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + libFileName + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');ThumbnailForContentImage(\'" + thumbName + "\');SubmitInsert();\"><img src=\"" + sDesLoc + "\" border=\"0\" /></a> " + (showThumbnail ? ("<a href=\"#\" onclick=\"Insert_thumb(\'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + originalfile.Replace("\'", "\\\'") + "\',\'" + sDesLoc.Replace("\'", "\\\'") + "\')\"><img src=\"images/application/thumbnail.gif\" border=\"0\" alt=\"Insert thumb nail and pop up larger image\" /></a>") : "");
                            }
                            else
                            {
                                dr["ITEM1"] = "<a href=\"#\"  title=\"" + m_refMsg.GetMessage("double click to insert msg") + "\" onclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + libFileName + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');Blink(\'" + TdName + "\', \'yellow\');ThumbnailForContentImage(\'" + thumbName + "\');return false;\" ondblclick=\"Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + libFileName + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');ThumbnailForContentImage(\'" + thumbName + "\');SubmitInsert();\"><img src=\"" + sDesLoc + "\" border=\"0\" /></a> <hr />";
                            }

                            dr["ITEM2"] = "REMOVE_ITEM";
                            if (isQuicklinks || isForms)
                            {
                                dr["ITEM3"] = "REMOVE_ITEM";
                            }
                            dr["ITEM4"] = "REMOVE_ITEM";
                            dt.Rows.Add(dr);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                i++;
            }
        }

        DataView dv = new DataView(dt);
        MediaListGrid.DataSource = dv;
        MediaListGrid.DataBind();
    }

    private void ViewLibraryCategoryToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("library folder title bar") + " \"" + cFolder.Name + "\""));
        result.Append("<table><tr>");
		if (sEditor != "JSEditor")
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&EditorName=" + sEditor + "&id=" + cFolder.ParentId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		if (m_strEnhancedMetaSelect.Length == 0)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/magnifier.png", (string)("isearch.aspx?action=showLibdlg" + caller + "&dentrylink=" + DEntryLink + "&folderid=" + folder + "&source=mediainsert&EditorName=" + Request.QueryString["EditorName"] + "&scope=" + Request.QueryString["scope"]), "Search", m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass, true));
        }
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void ViewLibraryByCategoryToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string retField = "";
        string showthumb = "";
        if ((Request.QueryString["retfield"] != null) && Request.QueryString["retfield"] != "")
        {
            retField = (string)("&retfield=" + Request.QueryString["retfield"]);
        }
        if ((Request.QueryString["showthumb"] != null) && Request.QueryString["showthumb"] != "")
        {
            showthumb = (string)("&showthumb=" + Request.QueryString["showthumb"]);
        }
        else if (showThumbnail == false)
        {
            showthumb = (string)("&showthumb=" + showThumbnail);
        }
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("library folder title bar") + " \"" + cFolder.Name + "\\" + LibType + "\""));
        if (m_refContentApi.RequestInformationRef.EnableReplication && (cFolder.ReplicationMethod == 1))
        {
            divTitleBar.InnerHtml += "&nbsp;(QuickDeploy)";
        }
        result.Append("<table><tr>");

		if (m_strEnhancedMetaSelect.Length <= 0)
		{
			if ((sEditor != "JSEditor") && (retField == ""))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("mediainsert.aspx?EditorName=" + sEditor + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&scope=" + Request.QueryString["scope"] + "&id=" + folder + showthumb), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "onclick=\"ClearFolderInfo();\"", StyleHelper.BackButtonCssClass, true));
			}
		}

		bool primaryCssApplied = false;

        if (m_strEnhancedMetaSelect.Length == 0)
        {
            if ((cPerms.CanAddToImageLib && LibType == "images") || (cPerms.CanAddToFileLib && LibType == "files"))
            {
                string QueryLibType = "";
                if (Request.QueryString["type"] == null)
                {
                    QueryLibType = (string)("&type=" + LibType);
                }
                if (Session["LibraryProductMode"] == null || Session["LibraryProductMode"].ToString()  == "" || Session["LibraryProductMode"].ToString().ToLower() == "false")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/bookAdd.png", "#", m_refMsg.GetMessage("upload and insert msg"), m_refMsg.GetMessage("btn add library"), "onclick=\"document.location.href=\'mediauploader.aspx?" + m_QueryString + "&folder=" + Request.QueryString["id"] + QueryLibType + "\';return false\"", StyleHelper.AddBookButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
                }
            }
        }

        if (m_strEnhancedMetaSelect.Length > 0)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/bookInsert.png", "#", "Add library item to selected metadata list", m_refMsg.GetMessage("btn insert"), "onclick=\"SubmitInsert();return false;\"", StyleHelper.InsertBookButtonCssClass, !primaryCssApplied));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (library)"), m_refMsg.GetMessage("btn delete"), "onclick=\"javascript:return SubmitDelete();\"", StyleHelper.DeleteButtonCssClass));
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/bookInsert.png", "#", m_refMsg.GetMessage("alt insert button text"), m_refMsg.GetMessage("btn insert"), "onclick=\"SubmitInsert();return false;\"", StyleHelper.InsertBookButtonCssClass, !primaryCssApplied));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (library)"), m_refMsg.GetMessage("btn delete"), "onclick=\"javascript:return SubmitDelete();\"", StyleHelper.DeleteButtonCssClass));
        }

		primaryCssApplied = true;

		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/magnifier.png", (string)("isearch.aspx?action=showLibdlg" + caller + "&folderid=" + folder + "&metadefinationtype=" + Request.QueryString["scope"] + "&dentrylink=" + DEntryLink + "&source=edit&EditorName=" + Request.QueryString["EditorName"] + "&scope=" + Request.QueryString["scope"] + retField + showthumb), "Search", m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass));
        
		if ((Request.QueryString["scope"] != "images") && m_strEnhancedMetaSelect.Length == 0)
        {
            result.Append("<td>" + "\r\n");
            result.Append("<script type=\"text/javascript\" language=\"javascript\">" + "\r\n");
            result.Append("<!--" + "\r\n");
            result.Append("function ChangeLibraryType(SelObj)" + "\r\n");
            result.Append("{" + "\r\n");
            result.Append(" var autonavfolder = getQuerystringValues(\"autonavfolder\",0,parent[\"medialist\"].location.href);" + "\r\n");                                                                                                                                                                                                                                              
            result.Append("document.location.href = \"mediainsert.aspx?LangType=" + m_intContentLanguage + "&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&scope=" + Request.QueryString["scope"] + "&id=" + folder + "&type=\" + SelObj.value + \"&autonavfolder=\" + autonavfolder + \"&EditorName=" + sEditor + (!string.IsNullOrEmpty (sLinkText) ? ("&selected=" +  sLinkText.Replace("'", @"\'") ) : "") + "\";" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("// -->" + "\r\n");
            result.Append("</script>" + "\r\n");
            result.Append("<select onchange=\"ChangeLibraryType(this)\" align=\"right\" id=\"LibType\" name=\"LibType\">" + "\r\n");
            result.Append("<option " + IsSelected("files") + " value=\"files\">Files</option>" + "\r\n");
            result.Append("<option " + IsSelected("forms") + " value=\"forms\">Forms</option>" + "\r\n");
            result.Append("<option " + IsSelected("hyperlinks") + " value=\"hyperlinks\">Hyperlinks</option>" + "\r\n");
            result.Append("<option " + IsSelected("images") + " value=\"images\">Images</option>" + "\r\n");
            result.Append("<option " + IsSelected("quicklinks") + " value=\"quicklinks\">Quicklinks</option>" + "\r\n");
            result.Append("</select>" + "\r\n");
            result.Append("</td>" + "\r\n");
        }
        if ((Request.QueryString["scope"] != "images") && (Request.QueryString["scope"] == "") && m_strEnhancedMetaSelect.Length > 0)
        {
            result.Append("<td>" + "\r\n");
            result.Append("<script type=\"text/javascript\" language=\"javascript\">" + "\r\n");
            result.Append("<!--" + "\r\n");
            result.Append("function ChangeLibraryType(SelObj)" + "\r\n");
            result.Append("{" + "\r\n");
            result.Append(" var autonavfolder = getQuerystringValues(\"autonavfolder\",0,parent[\"medialist\"].location.href);" + "\r\n");
            result.Append("document.location.href = \"mediainsert.aspx?LangType=" + m_intContentLanguage + "&enhancedmetaselect=1&action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&fullscreen=off&scope=" + Request.QueryString["scope"] + "&id=" + folder + "&type=\" + SelObj.value + \"&autonavfolder=\" + autonavfolder + \"&EditorName=" + sEditor + (!string.IsNullOrEmpty(sLinkText) ? ("&selected=" + sLinkText.Replace("'", @"\'")) : "") + "\";" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("// -->" + "\r\n");
            result.Append("</script>" + "\r\n");
            result.Append("<select onchange=\"ChangeLibraryType(this)\" align=\"right\" id=\"LibType\" name=\"LibType\">" + "\r\n");
            result.Append("<option " + IsSelected("files") + " value=\"files\">Files</option>" + "\r\n");
            result.Append("<option " + IsSelected("forms") + " value=\"forms\">Forms</option>" + "\r\n");
            result.Append("<option " + IsSelected("hyperlinks") + " value=\"hyperlinks\">Hyperlinks</option>" + "\r\n");
            result.Append("<option " + IsSelected("images") + " value=\"images\">Images</option>" + "\r\n");
            result.Append("<option " + IsSelected("quicklinks") + " value=\"quicklinks\">Quicklinks</option>" + "\r\n");
            result.Append("</select>" + "\r\n");
            result.Append("</td>" + "\r\n");
        }

        if ((m_strEnhancedMetaSelect.Length == 0))
        {
            if ((LibType == "quicklinks" | LibType == "forms" | LibType == "hyperlinks"))
            {
                result.Append(m_refStyle.GetShowAllActiveLanguage(false, "", "JavaScript:SelLibLanguage(this.value)", m_intContentLanguage.ToString()));
            }
            else
            {
                if (blnSelectAll == true)
                {
                    result.Append(m_refStyle.GetShowAllActiveLanguage(true, "", "JavaScript:SelLibLanguage(this.value)", Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES.ToString()));
                }
                else
                {
                    result.Append(m_refStyle.GetShowAllActiveLanguage(false, "", "JavaScript:SelLibLanguage(this.value)", m_intContentLanguage.ToString()));
                }
            }
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    private string IsSelected(string ret)
    {
        string returnValue;
        returnValue = "";
        if (LibType == ret)
        {
            return (" selected=\"selected\" ");
        }
        return returnValue;
    }
    public string MakeStringJSSafe(string str)
    {
        str = str.Replace("\\", "\\\\");
        str = str.Replace("\'", "\\\'");
        str = str.Replace(";", "\\;");
        str = str.Replace("(", "\\(");
        str = str.Replace(")", "\\)");
        return str;
    }

    protected void MediaListGrid_ItemBound(object sender, DataGridItemEventArgs e)
    {
        bool cell3Exists = false;
        if (action.ToLower() == "viewlibrarybycategory")
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (LibType == "quicklinks" || LibType == "forms")
                    {
                        if (e.Item.Cells.Count >= 4)
                        {
                            cell3Exists = true;
                        }
                        else
                        {
                            cell3Exists = false;
                        }
                        if (e.Item.Cells[1].Text.Equals("REMOVE_ITEM") && e.Item.Cells[2].Text.Equals("REMOVE_ITEM"))
                        {
                            if (cell3Exists && e.Item.Cells[3].Text.Equals("REMOVE_ITEM"))
                            {
                                e.Item.Cells[0].ColumnSpan = 4;
                                e.Item.Cells.RemoveAt(3);
                            }
                            else
                            {
                                e.Item.Cells[0].ColumnSpan = 3;
                            }
                            e.Item.Cells.RemoveAt(2);
                            e.Item.Cells.RemoveAt(1);
                        }
                        else
                        {
                            e.Item.Cells[0].Attributes.Add("id", arr_TdName[j]);
                            e.Item.Cells[1].Attributes.Add("id", (string)(arr_TdName[j] + "_0"));
                            e.Item.Cells[2].Attributes.Add("id", (string)(arr_TdName[j] + "_2"));
                            if (cell3Exists)
                            {
                                e.Item.Cells[3].Attributes.Add("id", (string)(arr_TdName[j] + "_1"));
                            }
                            j++;
                        }
                    }
                    else
                    {
                        if (e.Item.Cells[1].Text.Equals("REMOVE_ITEM") && e.Item.Cells[2].Text.Equals("REMOVE_ITEM"))
                        {
                            e.Item.Cells[0].ColumnSpan = 3;
                            e.Item.Cells.RemoveAt(2);
                            e.Item.Cells.RemoveAt(1);
                        }
                        else
                        {
                            e.Item.Cells[0].Attributes.Add("id", arr_TdName[j]);
                            e.Item.Cells[1].Attributes.Add("id", (string)(arr_TdName[j] + "_0"));
                            e.Item.Cells[2].Attributes.Add("id", (string)(arr_TdName[j] + "_1"));
                            j++;
                        }
                    }
                    break;
            }
        }
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
        Display_ViewLibraryByCategory();
    }
    private void RegisterResources()
    {
        ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();

        // create a package that will register the UI JS and CSS we need
        Package searchResultsControlPackage = new Package()
        {
            Components = new List<Component>()
            {
                // Register JS Files
                Packages.EktronCoreJS,
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/empjsfunc.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/ewebeditpro/eweputil.js"),
                Packages.Ektron.StringObject,

                JavaScript.Create(cmsContextService.WorkareaPath + "/java/plugins/modal/ektron.modal.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/toolbar_roll.js"),
                JavaScript.Create(cmsContextService.WorkareaPath + "/java/workareahelper.js"),

                // Register CSS Files
                Css.Create(cmsContextService.WorkareaPath + "/csslib/ektron.fixedPositionToolbar.css"),
                Css.Create(cmsContextService.WorkareaPath + "/java/plugins/modal/ektron.modal.css")
            }
        };
        searchResultsControlPackage.Register(this);
    }
    
    private void Process_DeleteItem()
    {
        try
        {
            _PageData = new Collection();
            _PageData.Add(Request.QueryString["id"], "LibraryID", null, null);
            _PageData.Add(Request.QueryString["folderid"], "ParentID", null, null);
            m_refContentApi.DeleteLibraryItemById(_PageData);
            Response.Redirect("mediainsert.aspx?action=ViewLibraryByCategory&dentrylink=0&EditorName=&scope=all&enhancedmetaselect=&separator=&metadataformtagid=True&id=" + Request.QueryString["folderid"] + "&autonavfolder=0", false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + m_intContentLanguage + "&info=" + ex.Message), false);
        }
    }
}
