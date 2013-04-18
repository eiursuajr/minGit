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
using Ektron.Cms.Content;

public partial class Workarea_urlAutoAliasSourceSelector : System.Web.UI.Page
{
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected long FolderId = 0;
    protected EkContent _Content;
    protected string actName = "";
    protected string AppName = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected CommonApi m_refCommonApi = new CommonApi();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected Collection gtNavs;
    protected Collection cTmp;
    protected Collection cFolders;
    protected object FolderName;
    protected long ParentFolderId;
    protected string fPath = "";
    protected string prodDomain = string.Empty;
    protected EkContent m_refContent;
    protected string strPageAction = "";
    protected string ListFoldersFor = ""; //I guess by default reports using it.
    protected int ContentLanguage;
    protected TaxonomyData taxonomy_data = new TaxonomyData();
    TaxonomyRequest taxonomy_request = new TaxonomyRequest();
    protected string ExtraQuery = "";
    protected string strRedirectFromReport = "";
    protected int m_nTargetFolderIsXml = 0;
    protected int m_nWantXmlInfo = 0;
    protected bool m_bBlockBlogFolders;
    protected bool m_bSubFolder = false;
    protected string FolderType = "";
    protected TaxonomyBaseData[] taxonomyList;
    protected TaxonomyBaseData[] taxDetails;
    protected string mode = string.Empty;
    protected string parentTaxonomyPath = string.Empty;
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected long newTaxId = 0;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            RegisterResources();
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            if (!(m_refCommonApi.IsAdmin() || m_refContentApi.EkContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, m_refCommonApi.RequestInformationRef.UserId, false) || m_refContentApi.EkContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin, m_refCommonApi.RequestInformationRef.UserId, false)))
            {
                Utilities.ShowError(m_refCommonApi.EkMsgRef.GetMessage("User not authorized"));
                return;
            }

            ExtraQuery = "";
            if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
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
            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
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

            ExtraQuery = (string)("&LangType=" + ContentLanguage);
            string urlReferer = Request.UrlReferrer.ToString();

            if (urlReferer.IndexOf("taxonomy") != -1)
            {
                ltraliasOrTax.Text = "taxonomy";
            }
            else if (urlReferer.IndexOf("alias") != -1)
            {
                ltraliasOrTax.Text = "alias";
            }
            else
            {
                ltraliasOrTax.Text = aliasOrTax.Value;
            }

            if (!(Request.QueryString["WantXmlInfo"] == null))
            {
                ExtraQuery += "&WantXmlInfo=1";
                m_nWantXmlInfo = 1;
            }
            else
            {
                m_nWantXmlInfo = 0;
            }

            mode = Request.QueryString["mode"];

            m_refMsg = m_refContentApi.EkMsgRef;
            PageLabel.Text = PageLabel.ToolTip = m_refMsg.GetMessage("lbl pagecontrol page");
            OfLabel.Text = OfLabel.ToolTip = m_refMsg.GetMessage("lbl pagecontrol of");

            FirstPage.ToolTip = m_refMsg.GetMessage("lbl first page");
            PreviousPage1.ToolTip = m_refMsg.GetMessage("lbl previous page");
            NextPage.ToolTip = m_refMsg.GetMessage("lbl next page");
            LastPage.ToolTip = m_refMsg.GetMessage("lbl last page");

            FirstPage.Text = "[" + m_refMsg.GetMessage("lbl first page") + "]";
            PreviousPage1.Text = "[" + m_refMsg.GetMessage("lbl previous page") + "]";
            NextPage.Text = "[" + m_refMsg.GetMessage("lbl next page") + "]";
            LastPage.Text = "[" + m_refMsg.GetMessage("lbl last page") + "]";

            //Put user code to initialize the page here
            AppImgPath = m_refContentApi.AppImgPath;
            AppPath = m_refContentApi.AppPath;
            AppName = m_refContentApi.AppName;
            if ((!String.IsNullOrEmpty(Request.QueryString["FolderID"])) && Information.IsNumeric(Request.QueryString["FolderID"]))
            {
                FolderId = Convert.ToInt64(Request.QueryString["FolderID"]);
            }
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            if (!(Request.QueryString["listfolderfor"] == null))
            {
                ListFoldersFor = (string)(Request.QueryString["listfolderfor"].ToString().ToLower());
                ExtraQuery += (string)("&listfolderfor=" + ListFoldersFor);
            }
            m_refContent = m_refContentApi.EkContentRef;

            if ((!String.IsNullOrEmpty(Request.QueryString["noblogfolders"])) && Information.IsNumeric(Request.QueryString["noblogfolders"]))
            {
                m_bBlockBlogFolders = Convert.ToInt32(Request.QueryString["noblogfolders"]) > 0 ? true : false;
            }

            if (!(Request.QueryString["action"] == null))
            {
                strPageAction = Request.QueryString["action"];
            }

            if (!(Request.QueryString["from_page"] == null))
            {
                strRedirectFromReport = Request.QueryString["from_page"];
            }

            if (!(Request.QueryString["subfolderchk"] == null))
            {
                m_bSubFolder = System.Convert.ToBoolean("true" == Request.QueryString["subfolderchk"].ToLower());
            }
            if (Request.QueryString["aliasOrTax"] == null || Request.QueryString["aliasOrTax"] == "")
            {
                if (strPageAction == "movecopy")
                {
                    GenerateMoveCopyTaxonomyToolBar();
                }
                else if (mode == "Taxonomy")
                {
                    GenerateToolBar();
                    
                }
                else
                {
                    GenerateToolBar();
                }
            }
            else if (Request.QueryString["aliasOrTax"] == "taxonomy")
            {
                GenerateMoveCopyTaxonomyToolBar();
            }
            else if (Request.QueryString["aliasOrTax"] == "alias")
            {
                GenerateToolBar();
            }
            

            if (!(Request.QueryString["pager"] == null))
            {
                Navigation_Change(Request.QueryString["pager"]);
            }
            else
            {
                DisplayPage();
            }
            SetServerJSVariables();

            isPostData.Value = "true";
            switch (strPageAction)
            {
                case "copy":
                    MoveCopyTaxonomy(false);
                    break;
                case "move":
                    MoveCopyTaxonomy(true);
                    break;
            }

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);
        }
    }

    private void DisplayPage()
    {
        if (!(Request.QueryString["metadataFormTagId"] == null))
        {
            ExtraQuery += (string)("&metadataFormTagId=" + Request.QueryString["metadataFormTagId"]);
            ExtraQuery += (string)("&separator=" + Request.QueryString["separator"]);
            frmFormTagId.Value = Request.QueryString["metadataFormTagId"];
            System.Drawing.Color sClr;
            sClr = System.Drawing.Color.FromArgb(255, 255, 225);
            ContentGrid.BackColor = sClr;
            ContentGrid.BorderColor = sClr;
        }
        if (mode == "Taxonomy")
        {
            TaxonomyRequest requestTax = new TaxonomyRequest();
            requestTax.TaxonomyId = FolderId;
            requestTax.TaxonomyLanguage = ContentLanguage;
            requestTax.SearchText = string.Empty;
            requestTax.PageSize = m_refCommonApi.RequestInformationRef.PagingSize;
            requestTax.CurrentPage = m_intCurrentPage;
            if (Request.QueryString["CopyType"] == "Locale")
            {
                requestTax.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Locale;
            }
            taxonomyList = m_refContent.ReadAllSubCategories(requestTax);
            if ((requestTax.TotalPages > 0) && (m_intCurrentPage > requestTax.TotalPages))
            {
                m_intCurrentPage = requestTax.TotalPages;
                requestTax.CurrentPage = m_intCurrentPage;
                taxonomyList = m_refContent.ReadAllSubCategories(requestTax);
            }
            taxDetails = m_refContent.GetTaxonomyRecursiveToParent(FolderId, ContentLanguage, 1);

            if (taxDetails.Length >= 1)
            {
                fPath = taxDetails[taxDetails.Length - 1].TaxonomyPath;
                ParentFolderId = taxDetails[taxDetails.Length - 1].TaxonomyParentId;
            }

            m_intTotalPages = requestTax.TotalPages;
            PopulateTaxonomyGridData();
        }
        else
        {
            gtNavs = m_refContent.GetFolderInfoWithPath(FolderId);
            FolderName = gtNavs["FolderName"];
            ParentFolderId = (long)gtNavs["ParentID"];
            prodDomain = (string)(gtNavs["DomainProduction"]);

            if (!String.IsNullOrEmpty(prodDomain) && ParentFolderId != 0)
            {
                fPath = (string)(gtNavs["Path"]);
                int secondIndex;
                secondIndex = fPath.IndexOf("\\", 1);
                fPath = fPath.Substring(secondIndex);

            }
            else if (!String.IsNullOrEmpty(prodDomain) && ParentFolderId == 0)
            {
                fPath = "\\";
            }
            else
            {
                fPath = (string)(gtNavs["Path"]);

            }
            if (((Collection)gtNavs["XmlConfiguration"]).Count > 0)
                m_nTargetFolderIsXml = 1;
            else
                m_nTargetFolderIsXml = 0;
            cTmp = new Collection();
            cTmp.Add("name", "OrderBy", null, null);
            cTmp.Add(FolderId, "FolderID", null, null);
            cTmp.Add(FolderId, "ParentID", null, null);
            cFolders = m_refContent.GetAllViewableChildFoldersv2_0(cTmp);
            PopulateFolderGridData();
        }

        PageSettings();
    }

    private void PopulateTaxonomyGridData()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string BlogFolderParm = (string)(m_bBlockBlogFolders ? "1" : "0");
        string currentFolderCheckbox = "";
        string taxonomyImage = string.Empty;
        int index;

        colBound.DataField = "ITEM1";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM2";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM3";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));

        dr = dt.NewRow();
        if (Request.QueryString["CopyType"] == "Locale")
        {
            dr[0] = m_refMsg.GetMessage("alt select translation packages by navigating") + ":";
        }
        else
        {
            dr[0] = m_refMsg.GetMessage("alt select sub categories by navigating") + ":";
        }
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        if ("siteupdateactivity" == strPageAction)
        {
            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" id=\"selectall\" name=\"selectall\" onclick=\"checkAll();\"/>" + m_refMsg.GetMessage("generic select all msg");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" name=\"allsubfolders\" id=\"allsubfolders\"";
            if (m_bSubFolder)
            {
                dr[0] = dr[0] + " checked";
            }
            dr[0] = dr[0] + "/>" + m_refMsg.GetMessage("lbl include sub-folders");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            currentFolderCheckbox = "<input type=\"checkbox\" id = \"selectedfolder\" name=\"selectedfolder\" value=\"" + FolderId + "\" />&nbsp;" + "<input type=\"hidden\" id=\"selfolder" + FolderId + "\" value=\"" + FolderName + "\" />&nbsp;" + "<input type=\"hidden\" id=\"rootFolder\" value=\"" + FolderId + "\" />&nbsp;";
        }

        dr = dt.NewRow();
        if (Request.QueryString["CopyType"] == "Locale")
        {
            dr[0] = "" + m_refMsg.GetMessage("lbl selected locale taxonomy") + "<span class=\"selectedContent\">" + currentFolderCheckbox + fPath + "</span>";
        }
        else
        {
            dr[0] = "" + m_refMsg.GetMessage("lbl selected taxonomy") + "<span class=\"selectedContent\">" + currentFolderCheckbox + fPath + "</span>";
        }
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);


        if (strPageAction == "movecopy" && FolderId != 0)
        {

            string url = string.Empty;
            if (Request.QueryString["CopyType"] == "Locale" || Request.QueryString["view"] == "locale")
            {
                url = "urlAutoAliasSourceSelector.aspx?CopyType=Locale&FolderID=0&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy";
            }
            else
            {
                url = "urlAutoAliasSourceSelector.aspx?FolderID=0&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy";
            }
            if (taxDetails[taxDetails.Length - 1].TaxonomyParentId != 0)
            {
                if (Request.QueryString["CopyType"] == "Locale" || Request.QueryString["view"] == "locale")
                {
                    url = "urlAutoAliasSourceSelector.aspx?CopyType=Locale&action=movecopy&FolderID=" + taxDetails[taxDetails.Length - 1].TaxonomyParentId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy";
                }
                else
                {
                    url = "urlAutoAliasSourceSelector.aspx?action=movecopy&FolderID=" + taxDetails[taxDetails.Length - 1].TaxonomyParentId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy";
                }
            }
            dr = dt.NewRow();
            dr[0] = "<a href=\"" + url + "\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=\"" + url + "\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }
        else if (FolderId != 0)
        {
            dr = dt.NewRow();
            dr[0] = "<a href=\"#\" onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Taxonomy&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Taxonomy&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }

        if (strPageAction == "movecopy")
        {
            for (index = 0; index <= taxonomyList.Length - 1; index++)
            {

                dr = dt.NewRow();
                taxonomyImage = AppPath + "images/UI/Icons/taxonomy.png";
                if (Request.QueryString["CopyType"] == "Locale" || Request.QueryString["view"] == "locale")
                {
                    dr[0] = "<a href=\"urlAutoAliasSourceSelector.aspx?CopyType=Locale&FolderID=" + taxonomyList[index].TaxonomyId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + taxonomyImage + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=\"urlAutoAliasSourceSelector.aspx?CopyType=Locale&FolderID=" + taxonomyList[index].TaxonomyId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + taxonomyList[index].TaxonomyName + "</a>";
                }
                else
                {
                    dr[0] = "<a href=\"urlAutoAliasSourceSelector.aspx?FolderID=" + taxonomyList[index].TaxonomyId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + taxonomyImage + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=\"urlAutoAliasSourceSelector.aspx?FolderID=" + taxonomyList[index].TaxonomyId + "&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + taxonomyList[index].TaxonomyName + "</a>";
                }
                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";

                dt.Rows.Add(dr);
            }
        }
        else
        {
            for (index = 0; index <= taxonomyList.Length - 1; index++)
            {

                dr = dt.NewRow();
                taxonomyImage = AppPath + "images/UI/Icons/taxonomy.png";

                dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + taxonomyList[index].TaxonomyId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Taxonomy&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + taxonomyImage + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + taxonomyList[index].TaxonomyId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Taxonomy&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + taxonomyList[index].TaxonomyName + "</a>";
                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";

                dt.Rows.Add(dr);
            }
        }


        DataView dv = new DataView(dt);
        ContentGrid.DataSource = dv;
        ContentGrid.DataBind();


        // change the next/prev links to jump properly
        // because normal asp.net postbacks off the button don't pass in all these weird querystring variables
        FirstPage.OnClientClick = "RecursiveSubmit(" + FolderId + ",\'" + BlogFolderParm + "\',\'\',\'\',\'Taxonomy\'" + (",\'First\'" + ");return false;");
        PreviousPage1.OnClientClick = "RecursiveSubmit(" + FolderId + ",\'" + BlogFolderParm + "\',\'\',\'\',\'Taxonomy\'" + (",\'Prev\'" + ");return false;");
        NextPage.OnClientClick = "RecursiveSubmit(" + FolderId + ",\'" + BlogFolderParm + "\',\'\',\'\',\'Taxonomy\'" + (",\'Next\'" + ");return false;");
        LastPage.OnClientClick = "RecursiveSubmit(" + FolderId + ",\'" + BlogFolderParm + "\',\'\',\'\',\'Taxonomy\'" + (",\'Last\'" + ");return false;");
    }
    private void PopulateFolderGridData()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string BlogFolderParm = (string)(m_bBlockBlogFolders ? "1" : "0");
        string currentFolderCheckbox = "";

        colBound.DataField = "ITEM1";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM2";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM3";
        colBound.HeaderText = "";
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("alt select sub folders by navigating") + ":";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        if ("siteupdateactivity" == strPageAction)
        {
            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" id=\"selectall\" name=\"selectall\" onclick=\"checkAll();\"/>" + m_refMsg.GetMessage("generic select all msg");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" name=\"allsubfolders\" id=\"allsubfolders\"";
            if (m_bSubFolder)
            {
                dr[0] = dr[0] + " checked";
            }
            dr[0] = dr[0] + "/>" + m_refMsg.GetMessage("lbl include sub-folders");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            currentFolderCheckbox = "<input type=\"checkbox\" id = \"selectedfolder\" name=\"selectedfolder\" value=\"" + FolderId + "\" />&nbsp;" + "<input type=\"hidden\" id=\"selfolder" + FolderId + "\" value=\"" + FolderName + "\" />&nbsp;" + "<input type=\"hidden\" id=\"rootFolder\" value=\"" + FolderId + "\" />&nbsp;";
        }

        dr = dt.NewRow();
        dr[0] = "" + m_refMsg.GetMessage("lbl Selected Folder") + "<span class=\"selectedContent\">" + currentFolderCheckbox + fPath + "</span>";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        if ((FolderId != 0 && (int)gtNavs["FolderType"] != 2) || (strRedirectFromReport == "report"))
        {
            dr = dt.NewRow();
            if ((string)(strRedirectFromReport) == "report")
            {
                if (FolderId != 0)
                {
                    dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
                }
                else
                {
                    if ("siteupdateactivity" == strPageAction)
                    {
                        // SiteUpdateReport: don't show up-folder button if already at root:
                        dr[0] = "";
                    }
                    else
                    {
                        dr[0] = "<a href=# onclick=\"return SaveSelCreateContent(&quot;" + strPageAction + "&quot;,&quot;0&quot;);return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"return SaveSelCreateContent(&quot;" + strPageAction + "&quot;,&quot;0&quot;);return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
                    }
                }
            }
            else
            {
                dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Folder&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "images/ui/icons/folderUp.png" + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Folder&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">..</a>";
            }

            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }

        string folder_image = "";


        if (!String.IsNullOrEmpty(FolderType))
        {
            FolderType = Request.QueryString["FolderType"];
        }
        if (!String.IsNullOrEmpty(FolderType))
        {
            if (FolderType == "9")
            {
                System.Collections.Generic.List<Ektron.Cms.Commerce.CatalogData> catalogData = new System.Collections.Generic.List<Ektron.Cms.Commerce.CatalogData>();
                Ektron.Cms.Commerce.CatalogEntry CatalogManager = new Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef);

                catalogData = CatalogManager.GetCatalogList(1, 1);

                folder_image = m_refContentApi.AppPath + "images/ui/icons/tree/folderGreenExpanded.png";
                for (int catalogFolders = 0; catalogFolders <= catalogData.Count - 1; catalogFolders++)
                {
                    dr = dt.NewRow();
                    FolderId = catalogData[catalogFolders].Id;
                    gtNavs = m_refContent.GetFolderInfoWithPath(FolderId);
                    string Path = (string)(gtNavs["Path"]);
                    Path = Path.Replace("\\", "\\\\");
                    dr[0] += "<a href=# onclick=\"SelectCatalog(\'" + catalogData[catalogFolders].Id + "\',\'" + Path + "\');return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + folder_image + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"SelectCatalog(\'" + catalogData[catalogFolders].Id + "\',\'" + Path + "\');return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + catalogData[catalogFolders].Name + "</a>";
                    dt.Rows.Add(dr);
                }
            }

        }
        else
        {
            bool bGo = false;
            foreach (Collection folder in (IEnumerable)cFolders)
            {
                bGo = false;
                int __foldertype = (int)folder["FolderType"];
                // Do not show blog type folders when they're explicitly unwanted:
                if (m_bBlockBlogFolders)
                {

                    if (__foldertype == 0 || __foldertype == 2 || __foldertype == 6 || __foldertype == 8)
                        bGo = true;
                    else
                        bGo = false;
                }
                else
                {
                    if (__foldertype == 0 || __foldertype == 1 || __foldertype == 6 || __foldertype == 8 || __foldertype == 9)
                        bGo = true;
                    else
                        bGo = false;
                }
                if (bGo)
                {
                    dr = dt.NewRow();
                    folder_image = AppPath + "images/ui/icons/folder.png";
                    if (__foldertype == 6)
                    {
                        folder_image = AppPath + "images/ui/icons/folderCommunity.png";
                    }
                    else if (__foldertype == 9)
                    {
                        folder_image = AppPath + "images/ui/icons/folderGreen.png";
                    }
                    else if (__foldertype == 8)
                    {
                        folder_image = AppPath + "images/ui/icons/tree/folderCalendarClosed.png";
                    }

                    if ((string)(strRedirectFromReport) == "report")
                    {
                        if ("siteupdateactivity" == strPageAction)
                        {
                            dr[0] = "<input type=\"checkbox\" name=\"selectedfolder\" value=\"" + folder["id"] + "\" >";
                            dr[0] = dr[0] + "<input type=\"hidden\" id=\"selfolder" + folder["id"] + "\" value=\"" + folder["Name"] + "\" >";
                        }
                        dr[0] += "<a href=# onclick=\"RecursiveSubmit(" + folder["id"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);\"return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">" + "<img src=\"" + folder_image + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + folder["id"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + strPageAction + "&quot;);\"return false;\" title=\"" + m_refMsg.GetMessage("alt: generic previous dir text") + "\">" + folder["Name"] + "</a>";
                    }
                    else
                    {
                        dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + folder["id"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Folder&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + folder_image + "\" border=\"0\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + folder["id"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;&quot;,&quot;&quot;,&quot;Folder&quot;" + ");return false;\" title=\"" + m_refMsg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
                    }

                    dr[1] = "&nbsp;";
                    dr[2] = "&nbsp;";
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
        if (mode == "Taxonomy")
        {
            if (Request.QueryString["CopyType"] == "Locale")
            {
                divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select locale taxonomy"));
            }
            else
            {
                divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select taxonomy"));
            }

        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select folder"));
        }

        result.Append("<table><tr>");

        result.Append("<td><span id=\"select_folder_save_btn_container\" style=\"display: \"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>" + "\r\n");
        if (strRedirectFromReport == "report")
        {
            if ("siteupdateactivity" == strPageAction)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl click here to save the event"), m_refMsg.GetMessage("btn save"), "onclick=\"return SaveSelFolderList();\"", StyleHelper.SaveButtonCssClass, true));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl click here to save the event"), m_refMsg.GetMessage("btn save"), "OnClick=\"return SaveSelCreateContent(\'" + strPageAction + "\',\'" + FolderId + "\');return false;\"", StyleHelper.SaveButtonCssClass, true));
            }
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt click here to save"), m_refMsg.GetMessage("btn save"), "OnClick=\"return SaveSelCreateContent();return false;\"", StyleHelper.SaveButtonCssClass, true));
        }
        result.Append("</tr></table></span></td>" + "\r\n");

        result.Append("<td><span id=\"select_folder_insert_btn_container\" style=\"display: none;\"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>" + "\r\n");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_insert-nm.gif", "#", m_refMsg.GetMessage("alt Click here to add"), m_refMsg.GetMessage("lbl Add Selection"), "onclick=\"return SaveSelCreateContent();return false;\"", StyleHelper.InsertButtonCssClass));
        result.Append("</tr></table></span></td>" + "\r\n");

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();

    }
    private void GenerateMoveCopyTaxonomyToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        if (Request.QueryString["CopyType"] == "Locale")
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select locale taxonomy"));

            result.Append("<table><tr>");

            result.Append("<td><span id=\"select_folder_save_btn_container\" style=\"display: \"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>" + "\r\n");

            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("lbl click here to copy locale taxonomy"), m_refMsg.GetMessage("lbl click here to copy locale taxonomy"), "onclick=\"moveCopyTaxonomy(\'copy\',\'Locale\');\"", StyleHelper.CopyButtonCssClass, true));
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/forward.png", "#", m_refMsg.GetMessage("lbl click here to move locale taxonomy"), m_refMsg.GetMessage("lbl click here to move locale taxonomy"), "onclick=\"moveCopyTaxonomy(\'move\',\'Locale\');\"", StyleHelper.MoveButtonCssClass));

        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select taxonomy"));

            result.Append("<table><tr>");

            result.Append("<td><span id=\"select_folder_save_btn_container\" style=\"display: \"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>" + "\r\n");

			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("lbl click here to copy taxonomy"), m_refMsg.GetMessage("lbl click here to copy taxonomy"), "onclick=\"moveCopyTaxonomy(\'copy\',\'content\');\"", StyleHelper.CopyButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/forward.png", "#", m_refMsg.GetMessage("lbl click here to move taxonomy"), m_refMsg.GetMessage("lbl click here to move taxonomy"), "onclick=\"moveCopyTaxonomy(\'move\',\'content\');\"", StyleHelper.MoveButtonCssClass));

        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("SelectCopyMoveTaxonomy", "") + "</td>");
        result.Append("</tr></table></span></td>" + "\r\n");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    public string MakeStringJSSafe(string str)
    {
        return (str.Replace("\'", "\\\'"));
    }
    protected void SetServerJSVariables()
    {
        litAppPath.Text = AppPath;
        litFolderID.Text = FolderId.ToString();
        litFolderPath.Text = fPath.Replace("\\", "\\\\");
        litContLanguage.Text = ContentLanguage.ToString();
        litListFoldersFor.Text = ListFoldersFor;
        litTargetFolderIsXml.Text = m_nTargetFolderIsXml.ToString();
        litExtraQuery.Text = ExtraQuery;
        litLangID.Text = ContentLanguage.ToString();
        ltrFolderID.Text = FolderId.ToString();
        litSelDiffTax.Text = m_refMsg.GetMessage("js:select different taxonomy");

        litNewTaxId.Text = newTaxId.ToString();
    }

    protected void RegisterResources()
    {
        // Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);

        // Register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
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
            TotalPages.Text = (System.Math.Ceiling((double)m_intTotalPages)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                PreviousPage1.Enabled = false;
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
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    public void Navigation_Change(string cmd)
    {
        switch (cmd)
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
                if (m_intCurrentPage < 1)
                {
                    m_intCurrentPage = 1;
                }
                break;
        }
        DisplayPage();
        isPostData.Value = "true";
    }
    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        Navigation_Change(e.CommandName);
    }
    private void MoveCopyTaxonomy(bool deleteSource)
    {

        long sourceId = 0;
        long destinationId = 0;
        int languageId = 0;

        if (!String.IsNullOrEmpty(Request.QueryString["SourceId"]))
        {
            sourceId = System.Convert.ToInt32(Request.QueryString["SourceId"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["destinationId"]))
        {
            destinationId = System.Convert.ToInt32(Request.QueryString["destinationId"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            languageId = System.Convert.ToInt32(Request.QueryString["LangType"]);
        }
            if (deleteSource == true)
            {
                m_refContent.MoveTaxonomy(sourceId, destinationId, true);
                newTaxId = sourceId;
            }
            else
            {
                newTaxId = m_refContent.CloneTaxonomy(sourceId, destinationId, languageId, -1, true, deleteSource);
        }
    }
}