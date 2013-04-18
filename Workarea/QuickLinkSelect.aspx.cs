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

public partial class QuickLinkSelect : System.Web.UI.Page
{
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected SiteAPI _SiteApi = new SiteAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected bool CanCreateContent = false;
    protected EkMessageHelper m_refMsg;
    protected int ContentLanguage = -1;
    protected string QuickLink = "";
    protected EkContentCol ContentData;
    protected long folderID;
    protected string fpath;
    protected Collection cfolders;
    protected string intQStringNoFID;
    protected string parentfolderid;
    protected string forceTemplate;
    protected Ektron.Cms.Content.ektUrlRewrite ektRW;
    protected string sFormName;
    protected string sTFormElement;
    protected object iQLInkCheck;
    protected string SetBrowserState;
    protected Collection gtNavs;
    protected Collection cTmp;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string FolderName;
    protected EkEnumeration.FolderType FolderType = EkEnumeration.FolderType.Content;
    protected PermissionData cPerms;
    protected bool isMac;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        isMac = Utilities.IsMac();
        m_refMsg = _SiteApi.EkMsgRef;
        AppImgPath = _SiteApi.AppImgPath;
        AppPath = _SiteApi.AppPath;
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect((string)("reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user"))), false);
            return;
        }
        RegisterResources();
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                _SiteApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (_SiteApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(_SiteApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (_SiteApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(_SiteApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        _SiteApi.ContentLanguage = ContentLanguage;
        ektRW = _SiteApi.EkUrlRewriteRef;

        //intQString = Request.QueryString
        //folderID = Request.QueryString("FolderID")
        long.TryParse(Request.QueryString["FolderID"].ToString(), out folderID);
        sFormName = Request.QueryString["formName"];
        sTFormElement = Request.QueryString["titleFormElem"];
        iQLInkCheck = Request.QueryString["useQLinkCheck"];
        SetBrowserState = Request.QueryString["SetBrowserState"];

        intQStringNoFID = (string)("formName=" + sFormName);
        if (sTFormElement != "")
        {
            intQStringNoFID = intQStringNoFID + "&titleFormElem=" + sTFormElement;
        }

        if (!string.IsNullOrEmpty(Convert.ToString(iQLInkCheck)))
        {
            intQStringNoFID = intQStringNoFID + "&useQLinkCheck=" + iQLInkCheck;
        }

        if (SetBrowserState != "")
        {
            intQStringNoFID = intQStringNoFID + "&SetBrowserState=" + SetBrowserState;
        }

        forceTemplate = Request.QueryString["forcetemplate"];

        gtNavs = m_refContentApi.EkContentRef.GetFolderInfoWithPath(folderID);
        FolderName = gtNavs["FolderName"].ToString();
        FolderType = (EkEnumeration.FolderType)gtNavs["FolderType"];
        parentfolderid = gtNavs["ParentID"].ToString();
        fpath = gtNavs["Path"].ToString();
        gtNavs = null;

        cTmp = new Collection();
        cTmp.Add("name", "OrderBy", null, null);
        cTmp.Add(folderID, "FolderID", null, null);
        cTmp.Add(folderID, "ParentID", null, null);
        cTmp.Add(folderID, "FilterContentAssetType", null, null);
        cfolders = m_refContentApi.EkContentRef.GetAllViewableChildFoldersv2_0(cTmp);

        cPerms = m_refContentApi.LoadPermissions(folderID, "folder", 0);
        CanCreateContent = cPerms.CanAdd;
        if (Request.QueryString["disAllowAddContent"] == "1")
        {
            CanCreateContent = false;
        }
        else if (FolderType == EkEnumeration.FolderType.Catalog)
        {
            CanCreateContent = false;
        }
        else if (CanCreateContent == true && isMac == true)
        {
            XmlConfigData[] active_xml_list = m_refContentApi.GetEnabledXmlConfigsByFolder(folderID);
            if ((Utilities.IsNonFormattedContentAllowed(active_xml_list)) == false)
            {
                CanCreateContent = false;
            }
        }
        if (!Page.IsPostBack)
        {
            PopulateQLinkList();
        }
        QuickLinkSelectToolBar();
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
        PopulateQLinkList();
    }
    private void PopulateQLinkList()
    {

        //Next two lines were moved from Page_Load to implement paging
        ContentData = m_refContentApi.EkContentRef.GetAllViewableChildInfov5_0(cTmp, _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber, EkEnumeration.CMSContentType.NonLibraryContent, EkEnumeration.CMSContentSubtype.AllTypes);

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

        System.Web.UI.WebControls.BoundColumn colBound;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Q1"; //info
        QLinkGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Q2"; //info
        QLinkGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Q3";
        QLinkGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("Q1", typeof(string)));
        dt.Columns.Add(new DataColumn("Q2", typeof(string)));
        dt.Columns.Add(new DataColumn("Q3", typeof(string)));

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("msg select qlink") + ":" + "<br>";
        dr[1] = "COLSPAN";
        dr[2] = "COLSPAN";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic path") + ":<span class=\"filePath\">" + fpath + "</span>";
        dr[1] = "COLSPAN";
        dr[2] = "COLSPAN";
        dt.Rows.Add(dr);

        if (folderID != 0)
        {
            dr = dt.NewRow();
            dr[0] = "<a href=\"QuickLinkSelect.aspx?" + intQStringNoFID + "&folderid=" + parentfolderid + "&forcetemplate=" + forceTemplate + "&disAllowAddContent=" + ((System.Convert.ToInt32(CanCreateContent)) + 1) + "title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a> <a href=\"QuickLinkSelect.aspx?" + intQStringNoFID + "&folderid=" + parentfolderid + "&forcetemplate=" + forceTemplate + "&disAllowAddContent=" + ((System.Convert.ToInt32(CanCreateContent)) + 1) + "title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }

        foreach (object temp in cfolders)
        {
            Collection folder = (Collection)temp;
            dr = dt.NewRow();
            dr[0] = "<a href=\"QuickLinkSelect.aspx?" + intQStringNoFID + "&folderid=" + folder["id"] + "&forcetemplate=" + forceTemplate + "&disAllowAddContent=" + ((System.Convert.ToInt32(CanCreateContent)) + 1) + "title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"";

            if (Convert.ToString(folder["FolderType"]) == "9")  //EkEnumeration.FolderType.Catalo"g
            {
                dr[0] += AppPath + "images/ui/icons/folderGreen.png";
            }
            else
            {
                dr[0] += AppPath + "images/ui/icons/folder.png";
            }
            dr[0] += "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"> <a href=\"QuickLinkSelect.aspx?" + intQStringNoFID + "&folderid=" + folder["Id"] + "&forcetemplate=" + forceTemplate + "&disAllowAddContent=" + ((System.Convert.ToInt32(CanCreateContent)) + 1) + "title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }

        foreach (ContentBase contBase in ContentData)
        {
            if (contBase.ContentSubType == EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
            {
                continue;
            }
            QuickLink = "";
            if ((contBase.ContentType != EkEnumeration.CMSContentType.LibraryItem) && (contBase.ContentType != EkEnumeration.CMSContentType.Archive_Content) && (contBase.ContentType != EkEnumeration.CMSContentType.Archive_Forms))
            {
                if (contBase.QuickLink == "" && Utilities.IsAsset((int)contBase.ContentType, ""))
                {
                    contBase.QuickLink = (string)(Strings.Replace(this._SiteApi.AppPath, this._SiteApi.SitePath, "", 1, 1, 0) + "showcontent.aspx?id=" + contBase.Id);
                }

                QuickLink = _SiteApi.SitePath + contBase.QuickLink;
                if (Convert.ToInt32(iQLInkCheck) > 0)
                {
                    if (this._SiteApi.RequestInformationRef.LinkManagement)
                    {
                        if (contBase.ContentType == EkEnumeration.CMSContentType.Forms)
                        {
                            QuickLink = _SiteApi.AppPath + "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + contBase.Id;
                        }
                        else
                        {
                            QuickLink = _SiteApi.AppPath + "linkit.aspx?LinkIdentifier=ID&ItemID=" + contBase.Id;
                        }
                    }
                }

                //If ((forceTemplate = "1") AndAlso (QuickLink.Length > 0)) Then
                //    QuickLink = ektRW.urlAlias(QuickLink, False)
                //    QuickLink = ektRW.urlRewrite(QuickLink, False)
                //End If
                QuickLink = QuickLink.Replace("\'", "\\\'\'");
                contBase.Title = contBase.Title.Replace("\'", "&#39;"); // to Prevent accidental bad data.
                if (sFormName == "frm_urlalias")
                {
                    if (contBase.ContentStatus == "A" && !(Ektron.Cms.Common.EkFunctions.IsImage((string)("." + contBase.AssetInfo.FileExtension)))) //AndAlso Not (contBase.QuickLink.ToLower().IndexOf("blogs.aspx?") >= 0) Last condition to filter blog post to be removed in 7.6 maintenance.
                    {
                        dr = dt.NewRow();
                        //23692 - title is HTML encoded here (by ReadDbString called earlier), replace &#39; (') with \' for JScript string to work
                        dr[0] = "&nbsp;<a style=\"text-decoration:none\" href=\"#\"  onclick=\"SetQLinkChoice(\'" + contBase.Title.Replace("&#39;", "\\\'") + "\',\'" + contBase.Id + "\',\'" + contBase.Language + "\',\'" + QuickLink + "\', \'" + sFormName + "\', \'" + sTFormElement + "\', \'" + iQLInkCheck + "\', \'" + SetBrowserState + "\', \'" + contBase.ContentStatus + "\');\">";
                        if (contBase.ContentType == EkEnumeration.CMSContentType.Forms)
                        {
                            dr[0] += "<img src=\"" + getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon) + "\" border=\"0\" title=\"Select a Form\" alt=\"Select a Form\" align=\"absbottom\">&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                        }
                        else if (contBase.ContentType == EkEnumeration.CMSContentType.Content)
                        {
                            dr[0] += "<img src=\"" + getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon, (int)contBase.ContentSubType) + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\">&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                        }
                        else if (contBase.ContentType == EkEnumeration.CMSContentType.CatalogEntry)
                        {
                            dr[0] += "<img src=\"" + getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon) + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\">&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                        }
                        else
                        {
                            dr[0] += getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon, (int)contBase.ContentSubType) + "&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                        }
                        dr[1] = "&nbsp;";
                        dr[2] = "&nbsp;";
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;<a style=\"text-decoration:none\" href=\"#\"  onclick=\"SetContentChoice(\'" + contBase.Title.Replace("&#39;", "\\\'") + "\',\'" + contBase.Id + "\',\'" + contBase.Language + "\',\'" + QuickLink + "\', \'" + sFormName + "\', \'" + sTFormElement + "\', \'" + iQLInkCheck + "\', \'" + SetBrowserState + "\', \'" + contBase.ContentStatus + "\');\">";
                    if (contBase.ContentType == EkEnumeration.CMSContentType.Forms || contBase.ContentType == EkEnumeration.CMSContentType.CatalogEntry)
                    {
                        dr[0] += "<img src=\"" + getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon) + "\" border=\"0\" title=\"Select a Form\" alt=\"Select a Form\" align=\"absbottom\">&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                    }
                    else if (contBase.ContentType == EkEnumeration.CMSContentType.Content)
                    {
                        dr[0] += "<img src=\"" + getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon) + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\">&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                    }
                    else
                    {
                        dr[0] += getNewContentTypeIcon((int)contBase.ContentType, contBase.AssetInfo.Icon) + "&nbsp;&nbsp;&nbsp;" + contBase.Title + "</a>";
                    }
                    dr[1] = "&nbsp;";
                    dr[2] = "&nbsp;";
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        QLinkGrid.DataSource = dv;
        QLinkGrid.DataBind();
    }

    protected void QLinkGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("COLSPAN"))
                {
                    e.Item.Cells[0].ColumnSpan = 3;
                    e.Item.Cells[0].CssClass = "label";
                    e.Item.Cells.RemoveAt(2);
                    e.Item.Cells.RemoveAt(1);
                }
                break;
        }
    }

    private void QuickLinkSelectToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl quickLink select"));
        result.Append("<table><tr>");
        result.Append("<tr><td>");
        if (CanCreateContent && sFormName != "frm_urlalias")
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/cancel.png", "edit.aspx", m_refMsg.GetMessage("alt exit without selecting content"), m_refMsg.GetMessage("btn cancel"), "onclick=\"window.close();return false;\"", StyleHelper.CancelButtonCssClass,true));
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/add.png", "#", m_refMsg.GetMessage("alt add content button text"), m_refMsg.GetMessage("btn add content"), "onclick=\"PopUpWindow(\'editarea.aspx?type=add&id=" + folderID + "\', \'Edit\', 790, 580, 1, 1);return false;\" ", StyleHelper.AddButtonCssClass,true));
            
        }
        result.Append("</td></tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    private string getContentTypeIcon(Collection objCont)
    {
        int ContentTypeID;
        string subfields;
        string strAssetIcon;
        string ContentIcon = AppPath + "images/ui/icons/content.png";
        string formsIcon = AppPath + "images/ui/icons/contentForm.png";

        try
        {
            ContentTypeID = Convert.ToInt32(objCont["ContentType"].ToString());

            if (ContentTypeID == 2)
            {
                return (formsIcon);
            }
            else if (Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentTypeID, false))
            {
                subfields = objCont["ContentText"].ToString();
                if (subfields.Length > 0)
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                    try
                    {
                        xml.LoadXml(subfields);
                        strAssetIcon = xml.GetElementsByTagName("ImageUrl")[0].InnerText.ToString();
                        return strAssetIcon;
                    }
                    catch (Exception)
                    {
                        return ContentIcon;
                    }
                }
                else
                {
                    return ContentIcon;
                }
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

    private string getNewContentTypeIcon(int ContentTypeID, string ContentText)
    {
        return getNewContentTypeIcon(ContentTypeID, ContentText, 0);
    }
    private string getNewContentTypeIcon(int ContentTypeID, string ContentText, int contentSubTypeId)
    {
        string returnValue;
        string ContentIcon = AppPath + "images/ui/icons/contentHtml.png";
        string formsIcon = AppPath + "images/ui/icons/contentForm.png";
        string catalogsIcon = AppPath + "images/ui/icons/brick.png";
        string eventIcon = AppPath + "images/ui/icons/calendar.png";

        if (ContentTypeID == 2)
        {
            returnValue = formsIcon;
        }
        else if (ContentTypeID == 1 && contentSubTypeId == (int)EkEnumeration.CMSContentSubtype.WebEvent)
        {
            returnValue = eventIcon;
        }
        else if (ContentTypeID == 1)
        {
            returnValue = ContentIcon;
        }
        else if (ContentTypeID == 3333)
        {
            returnValue = catalogsIcon;
        }
        else if (ContentTypeID == 0 && contentSubTypeId == (int)EkEnumeration.CMSContentSubtype.WebEvent)
        {
            returnValue = eventIcon;
        }
        else if (Ektron.Cms.Common.EkConstants.IsAssetContentType(ContentTypeID, true))
        {
            if (ContentText != "")
            {
                returnValue = ContentText;
            }
            else
            {
                returnValue = ContentIcon;
            }
        }
        else
        {
            returnValue = ContentIcon;
        }
        return returnValue;
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, _SiteApi.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, _SiteApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
    }
}