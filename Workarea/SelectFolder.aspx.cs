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

public partial class SelectFolder : System.Web.UI.Page
{
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected long FolderId = 0;
    protected string actName = "";
    protected string AppName = "";
    protected ContentAPI _ContentApi = new ContentAPI();
    protected StyleHelper _Style = new StyleHelper();
    protected EkMessageHelper _Msg;
    protected Collection gtNavs;
    protected Collection cTmp;
    protected object cFolders;
    protected object FolderName;
    protected object ParentFolderId;
    protected string fPath = "";
    protected EkContent _Content;
    protected string strPageAction = "";
    protected string ListFoldersFor = ""; //I guess by default reports using it.
    protected int ContentLanguage;
    protected string ExtraQuery = "";
    protected string strRedirectFromReport = "";
    protected int m_nTargetFolderIsXml = 0;
    protected int m_nWantXmlInfo = 0;
    protected bool m_bBlockBlogFolders;
    protected bool m_bSubFolder = false;
    protected EkEnumeration.FolderType FolderType = EkEnumeration.FolderType.Content;
    protected EkEnumeration.FolderType currentFolderType = EkEnumeration.FolderType.Content;
    protected bool _menuFlag = false;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        // register JS and CSS
        RegisterResources();
        try
        {
            _Msg = _ContentApi.EkMsgRef;
            CheckAccess();
            ExtraQuery = "";
            if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                _ContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = int.Parse(_ContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            ExtraQuery = (string)("&LangType=" + ContentLanguage);
            if (!(Request.QueryString["WantXmlInfo"] == null))
            {
                ExtraQuery += "&WantXmlInfo=1";
                m_nWantXmlInfo = 1;
            }
            else
            {
                m_nWantXmlInfo = 0;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["FolderType"]))
            {
                FolderType = (EkEnumeration.FolderType)Enum.Parse(typeof(EkEnumeration.FolderType), EkFunctions.UrlEncode(Request.QueryString["FolderType"]), true);
                if (Information.IsNumeric(Request.QueryString["FolderType"]))
                {
                    ExtraQuery += (string)("&FolderType=" + FolderType);
                }
            }
            if (!(Request.QueryString["menuflag"] == null))
            {
                _menuFlag = System.Convert.ToBoolean(Request.QueryString["menuflag"].ToLower() == "true");
                ExtraQuery += (string)("&menuflag=" + Request.QueryString["menuflag"]);
            }

            _Msg = _ContentApi.EkMsgRef;
            //Put user code to initialize the page here
            AppImgPath = _ContentApi.AppImgPath;
            AppPath = _ContentApi.AppPath;
            AppName = _ContentApi.AppName;
            if ((!String.IsNullOrEmpty(Request.QueryString["FolderID"])) && Information.IsNumeric(Request.QueryString["FolderID"]))
            {
                FolderId = Convert.ToInt64(Request.QueryString["FolderID"]);
            }
            StyleSheetJS.Text = _Style.GetClientScript();
            if (!(Request.QueryString["listfolderfor"] == null))
            {
                ListFoldersFor = (string)(Request.QueryString["listfolderfor"].ToString().ToLower());
                ListFoldersFor = EkFunctions.UrlEncode(ListFoldersFor);
                ExtraQuery += (string)("&listfolderfor=" + ListFoldersFor);
            }

            _Content = _ContentApi.EkContentRef;
            gtNavs = _Content.GetFolderInfoWithPath(FolderId);
            currentFolderType = (EkEnumeration.FolderType)Enum.ToObject(typeof(EkEnumeration.FolderType), Convert.ToInt32(gtNavs["FolderType"]));
            FolderName = gtNavs["FolderName"];
            ParentFolderId = gtNavs["ParentID"];
            fPath = (string)(gtNavs["Path"]);
            m_nTargetFolderIsXml = ((Collection)gtNavs["XmlConfiguration"]).Count;

            cTmp = new Collection();
            cTmp.Add("name", "OrderBy", null, null);
            cTmp.Add(FolderId, "FolderID", null, null);
            cTmp.Add(FolderId, "ParentID", null, null);

            if ((!(Request.QueryString["noblogfolders"] == null)) && Information.IsNumeric(Request.QueryString["noblogfolders"]))
            {
                if (Convert.ToInt32(Request.QueryString["noblogfolders"]) > 0)
                    m_bBlockBlogFolders = true;
                else
                    m_bBlockBlogFolders = false;
            }
            cFolders = _Content.GetAllViewableChildFoldersv2_0(cTmp);

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

            GenerateToolBar();

            // support for enhanced metadata (used with customfields.vb):
            if (!(Request.QueryString["metadataFormTagId"] == null))
            {
                ExtraQuery += (string)("&metadataFormTagId=" + EkFunctions.UrlEncode(Request.QueryString["metadataFormTagId"]));
                ExtraQuery += (string)("&separator=" + EkFunctions.UrlEncode(Request.QueryString["separator"]));
                frmFormTagId.Value = EkFunctions.UrlEncode(Request.QueryString["metadataFormTagId"]);
            }

            PopulateGridData();
            AssignJsTextStrings();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    private void CheckAccess()
    {
        bool loggedIn = _ContentApi.LoadPermissions(0, "folder", 0).IsLoggedIn;
        if ((!loggedIn) || (loggedIn && _ContentApi.RequestInformationRef.IsMembershipUser > 0))
        {
            Utilities.ShowError(_Msg.GetMessage("msg login cms user"));
            return;
        }
    }
    private void PopulateGridData()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string BlogFolderParm = (string)(m_bBlockBlogFolders ? "1" : "0");
        string currentFolderCheckbox = "";

        colBound.DataField = "ITEM1";
        colBound.HeaderText = "";
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM2";
        colBound.HeaderText = "";
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ITEM3";
        colBound.HeaderText = "";
        colBound.HeaderStyle.Height = Unit.Percentage(0);
        ContentGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
        dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));

        dr = dt.NewRow();
        dr[0] = _Msg.GetMessage("alt select sub folders by navigating") + ":" + "<br />";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "<br>";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        if ("siteupdateactivity" == strPageAction)
        {
            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" id=\"selectall\" name=\"selectall\" onclick=\"checkAll();\"/>" + _Msg.GetMessage("generic select all msg");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<input type=\"checkbox\" name=\"allsubfolders\" id=\"allsubfolders\"";
            if (m_bSubFolder)
            {
                dr[0] = dr[0] + " checked";
            }
            dr[0] = dr[0] + "/>" + _Msg.GetMessage("lbl include sub-folders");
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<br>";
            dr[1] = "remove-item";
            dr[2] = "remove-item";
            dt.Rows.Add(dr);

            currentFolderCheckbox = "<input type=\"checkbox\" id = \"selectedfolder\" name=\"selectedfolder\" value=\"" + FolderId + "\" />&nbsp;" + "<input type=\"hidden\" id=\"selfolder" + FolderId + "\" value=\"" + FolderName + "\" />&nbsp;" + "<input type=\"hidden\" id=\"rootFolder\" value=\"" + FolderId + "\" />&nbsp;";
        }

        dr = dt.NewRow();
        dr[0] = "" + _Msg.GetMessage("lbl Selected Folder") + "<span class=\"selectedFolder\">" + currentFolderCheckbox + fPath + "</span>";
        dr[1] = "remove-item";
        dr[2] = "remove-item";
        dt.Rows.Add(dr);

        if ((FolderId != 0) || (strRedirectFromReport == "report"))
        {
            dr = dt.NewRow();
            if ((string)(strRedirectFromReport) == "report")
            {
                if (FolderId != 0)
                {
                    dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;);return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "Images/ui/icons/folderUp.png" + "\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;);return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\">..</a>";
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
                        dr[0] = "<a href=# OnClick=\"return SaveSelCreateContent(&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;,&quot;0&quot;);return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "Images/ui/icons/folderUp.png" + "\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a>..<a href=# OnClick=\"return SaveSelCreateContent(&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;,&quot;0&quot;);return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\"></a>";
                    }
                }
            }
            else
            {
                dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;" + ");return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + AppPath + "Images/ui/icons/folderUp.png" + "\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" alt=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + ParentFolderId + ",&quot;" + BlogFolderParm + "&quot;" + ");return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\">..</a>";
            }

            dr[1] = "&nbsp;";
            dr[2] = "&nbsp;";
            dt.Rows.Add(dr);
        }

        string folder_image = "";

        bool bGo = false;
        foreach (Collection folder in (IEnumerable)cFolders)
        {
            bGo = false;
            int ftype = Convert.ToInt32(folder["FolderType"]);
            // Do not show blog type folders when they're explicitly unwanted:
            if (FolderType == EkEnumeration.FolderType.Catalog)
            {
                bGo = (ftype == 0 || ftype == 2 || ftype == 6 || ftype == 9 || ftype == 8) ? true : false;
            }
            else if (m_bBlockBlogFolders)
            {
                bGo = (ftype == 0 || ftype == 2 || ftype == 6 || ftype == 9 || ftype == 8) ? true : false;
            }
            else
            {
                bGo = (ftype == 0 || ftype == 1 || ftype == 2 || ftype == 6 || ftype == 8 || ftype == 3 || (_menuFlag && ftype == (int)EkEnumeration.FolderType.Catalog)) ? true : false;
            }
            if (bGo)
            {
                dr = dt.NewRow();
                folder_image = AppPath + "images/ui/icons/folder.png";
                if (ftype == 6)
                {
                    folder_image = AppPath + "images/ui/icons/folderCommunity.png";
                }
                else if (ftype == 9)
                {
                    folder_image = AppPath + "images/ui/icons/folderGreen.png";
                }
                else if (ftype == 1)
                {
                    folder_image = AppPath + "images/ui/icons/folderBlog.png";
                }
                else if (ftype == 2)
                {
                    folder_image = AppPath + "images/ui/icons/folderSite.png";
                }
                else if (ftype == 8)
                {
                    folder_image = AppPath + "images/ui/icons/folderCalendar.png";
                }
                else if (ftype == 3)
                {
                    folder_image = AppPath + "images/ui/icons/folderBoard.png";
                }

                if ((string)(strRedirectFromReport) == "report")
                {
                    if ("siteupdateactivity" == strPageAction)
                    {
                        dr[0] = "<input type=\"checkbox\" name=\"selectedfolder\" value=\"" + folder["ID"] + "\" >";
                        dr[0] = dr[0] + "<input type=\"hidden\" id=\"selfolder" + folder["ID"] + "\" value=\"" + folder["Name"] + "\" >";
                    }
                    dr[0] += "<a href=# onclick=\"RecursiveSubmit(" + folder["ID"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;);\"return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\"><img src=\"" + folder_image + "\" title=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + folder["ID"] + ",&quot;" + BlogFolderParm + "&quot;,&quot;report&quot;,&quot;" + EkFunctions.UrlEncode(strPageAction) + "&quot;);\"return false;\" title=\"" + _Msg.GetMessage("alt: generic previous dir text") + "\">" + folder["Name"] + "</a>";
                }
                else
                {
                    dr[0] = "<a href=# onclick=\"RecursiveSubmit(" + folder["ID"] + ",&quot;" + BlogFolderParm + "&quot;" + ");return false;\" title=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\"><img src=\"" + folder_image + "\" title=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\" alt=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\" align=\"absbottom\"></a><a href=# onclick=\"RecursiveSubmit(" + folder["ID"] + ",&quot;" + BlogFolderParm + "&quot;" + ");return false;\" title=\"" + _Msg.GetMessage("alt: generic view folder content text") + "\">" + folder["Name"] + "</a>";
                }
                dr[1] = "&nbsp;";
                dr[2] = "&nbsp;";
                dt.Rows.Add(dr);
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
        divTitleBar.InnerHtml = _Style.GetTitleBar(_Msg.GetMessage("lbl select folder"));
        result.Append("<table><tr>");

		//result.Append("<td><span id=\"select_folder_cancel_btn_container\" style=\"display: \"><table><tr>" + "\r\n");
		result.Append(_Style.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/cancel.png", "#", _Msg.GetMessage("alt Exit without selecting content"), _Msg.GetMessage("btn cancel"), "Onclick=\"CancelSelContent();return false;\"", StyleHelper.CancelButtonCssClass, true));
		//result.Append("</tr></table></span></td>" + "\r\n");

        //result.Append("<td><span id=\"select_folder_save_btn_container\" style=\"display: \"><table><tr>" + "\r\n");
        if (strRedirectFromReport == "report")
        {
            if ("siteupdateactivity" == strPageAction)
            {
                result.Append(_Style.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _Msg.GetMessage("lbl click here to save the event"), _Msg.GetMessage("btn save"), "OnClick=\"return SaveSelFolderList();\"", StyleHelper.SaveButtonCssClass, true));
            }
            else
            {
				result.Append(_Style.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _Msg.GetMessage("lbl click here to save the event"), _Msg.GetMessage("btn save"), "OnClick=\"return SaveSelCreateContent(\'" + EkFunctions.UrlEncode(strPageAction) + "\',\'" + FolderId + "\');return false;\"", StyleHelper.SaveButtonCssClass, true));
            }
        }
        else
        {
            if (FolderType != EkEnumeration.FolderType.Catalog || (FolderType == EkEnumeration.FolderType.Catalog && currentFolderType == EkEnumeration.FolderType.Catalog))
            {
				result.Append(_Style.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _Msg.GetMessage("alt click here to add selected items"), _Msg.GetMessage("alt add selections"), "OnClick=\"return SaveSelCreateContent();return false;\"", StyleHelper.SaveButtonCssClass, true));
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + _Style.GetHelpButton("SelFolder", "") + "</td>");

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();

    }
    public string MakeStringJSSafe(string str)
    {
        return (str.Replace("\'", "\\\'"));
    }
    public void RegisterResources()
    {
        // Register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);

        // Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
    }
    public void AssignJsTextStrings()
    {
        jsFolderId.Text = FolderId.ToString();
        jsFPath.Text = fPath.Replace("\\", "\\\\");
        jsContentLanguage.Text = ContentLanguage.ToString();
        jsListFoldersFor.Text = ListFoldersFor;
        jsTargetFolderIsXml.Text = m_nTargetFolderIsXml.ToString();
        jsExtraQuery.Text = ExtraQuery;

        if (!String.IsNullOrEmpty(Request.QueryString["TaxonomyId"])) {
            taxonomyString.Text = "&TaxonomyId=" + Request.QueryString["TaxonomyId"] + "&SelTaxonomyId=" + Request.QueryString["TaxonomyId"];
        } else if (!String.IsNullOrEmpty(Request.QueryString["SelTaxonomyId"])) {
            taxonomyString.Text = "&TaxonomyId=" + Request.QueryString["SelTaxonomyId"] + "&SelTaxonomyId=" + Request.QueryString["SelTaxonomyId"];
        } else {
            taxonomyString.Text = "";
        }

    }
}