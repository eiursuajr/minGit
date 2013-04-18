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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.CustomFieldsApi;
using Ektron.Cms.Commerce;



public partial class viewfolderattributes : System.Web.UI.UserControl
{


    #region Member Variables

    protected ContentAPI _ContentApi = new ContentAPI();
    protected SiteAPI _SiteApi = new SiteAPI();
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected EkMessageHelper _MessageHelper;
    protected long _Id = 0;
    protected FolderData _FolderData;
    protected PermissionData _PermissionData;
    protected string _AppImgPath = "";
    protected string _AppPath = "";
    protected int _ContentType = 1;
    protected long _CurrentUserId = 0;
    protected Collection _PageData;
    protected string _PageAction = "";
    protected string _OrderBy = "";
    protected int _ContentLanguage = -1;
    protected int _EnableMultilingual = 0;
    protected string _SitePath = "";
    protected long _FolderId = -1;
    protected string _ShowPane = "";
    protected ProductType _ProductType = null;
    protected bool _Catalog = false;

    private SubscriptionData[] _SubscriptionData;
    private SubscriptionData[] _SubscribedData;
    private SubscriptionPropertiesData _SubscriptionPropertiesData;
    private bool _GlobalSubInherit = false;
    private SettingsData _SettingsData = new SettingsData();
    private int _FolderType = 0;
    private BlogData _BlogData;
    private int _i = 0;

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _AppPath = _ContentApi.AppPath;
        RegisterResources();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
        _SettingsData = _SiteApi.GetSiteVariables(_SiteApi.UserId);
        ltrAliases.Text = _MessageHelper.GetMessage("lbl forcemanualaliasing");
    }

    #endregion

    #region CSS JS

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, this._AppPath + "/controls/folder/sitemap.js", "EktronSitemapJS");

    }

    #endregion


    public bool ViewFolderAttributes()
    {
        if (!(Request.QueryString["id"] == null))
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (!(Request.QueryString["action"] == null))
        {
            _PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (!(Request.QueryString["orderby"] == null))
        {
            _OrderBy = Convert.ToString(Request.QueryString["orderby"]);
        }
        if (!(Request.QueryString["showpane"] == null))
        {
            _ShowPane = Convert.ToString(Request.QueryString["showpane"]);
        }
        else
        {
            _ShowPane = "";
        }
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                _ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
            }
            else
            {
                if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }
        // Ensure that a specific language is selected, use default if needed:
        if ((_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED) || (_ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES))
        {
            _ContentLanguage = _ContentApi.RequestInformationRef.DefaultContentLanguage;
        }
        _ContentApi.ContentLanguage = _ContentLanguage;

        _CurrentUserId = _ContentApi.UserId;
        _AppImgPath = _ContentApi.AppImgPath;
        _AppPath = _ContentApi.AppPath;
        _SitePath = _ContentApi.SitePath;
        _EnableMultilingual = _ContentApi.EnableMultilingual;
        if (!(Page.IsPostBack))
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true, true);
            _FolderType = _FolderData.FolderType;
            _FolderId = _FolderData.Id;
            switch ((Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType)
            {
                case Ektron.Cms.Common.EkEnumeration.FolderType.Catalog:
                    _Catalog = true;
                    Display_ViewCatalog();
                    break;
                default:
                    phWebAlerts.Visible = true;
                    Display_ViewFolder();
                    break;
            }
        }

        if (_FolderData.IsCommunityFolder)
        {
            Display_AddCommunityFolder();
        }
        return true;
    }

    private void Display_AddCommunityFolder()
    {
        if (_FolderData == null)
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true);
        }
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Community Folder " + " \"" + _FolderData.Name + "\"");
        ltr_vf_smartforms.Visible = true;
        ltrTypes.Visible = true;
    }

    #region Folder - ViewFolder

    private void Display_ViewFolder()
    {
        Collection cPreApproval;
        bool isBlog;
        isBlog = System.Convert.ToBoolean(_FolderType == 1 ? true : false);
        if (isBlog)
        {
            _BlogData = _ContentApi.BlogObject(_FolderData);
            _FolderData.PublishPdfEnabled = false;
            phSubjects.Visible = true;
            phBlogRoll.Visible = true;
            phDescription.Visible = false;
        }

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);

        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt");
        //Sitemap Path
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration");

        ViewFolderToolBar();

        if (isBlog)
        {
            phBlogProperties1.Visible = true;
            if (_BlogData.Visibility == Ektron.Cms.Common.EkEnumeration.BlogVisibility.Public)
            {
                td_vf_visibilitytxt.InnerHtml = _MessageHelper.GetMessage("lbl public");
            }
            else
            {
                td_vf_visibilitytxt.InnerHtml = _MessageHelper.GetMessage("lbl private");
            }

            td_vf_nametxt.InnerHtml = _BlogData.Name;
            td_vf_titletxt.InnerHtml = _BlogData.Title;
        }
        else
        {
            phFolderProperties1.Visible = true;
            td_vf_foldertxt.InnerHtml = _FolderData.Name;
        }

        td_vf_idtxt.InnerHtml = _Id.ToString();

        if (isBlog)
        {
            phBlogProperties2.Visible = true;
            string sEnabled = "";
            string sModerate = "";
            string sRequire = "";
            string sNotify = "";
            if (_BlogData.EnableComments)
            {
                sEnabled = "checked=\"checked\" ";
            }
            if (_BlogData.ModerateComments)
            {
                sModerate = "checked=\"checked\" ";
            }
            if (_BlogData.RequiresAuthentication)
            {
                sRequire = "checked=\"checked\" ";
            }
            if (_BlogData.NotifyURL != "")
            {
                sNotify = "checked=\"checked\" ";
            }
            td_vf_taglinetxt.InnerHtml = _BlogData.Tagline;
            if (_BlogData.PostsVisible < 0)
            {
                td_vf_postsvisibletxt.InnerHtml = "(selected day)";
            }
            else
            {
                td_vf_postsvisibletxt.InnerHtml += _BlogData.PostsVisible.ToString();
            }
            td_vf_commentstxt.InnerHtml += "<input disabled=\"disabled\" type=\"checkbox\" name=\"enable_comments\" id=\"enable_comments\" " + sEnabled + " />" + _MessageHelper.GetMessage("lbl enable comments");
            td_vf_commentstxt.InnerHtml += "<br />";
            td_vf_commentstxt.InnerHtml += "<input disabled=\"disabled\" type=\"checkbox\" name=\"moderate_comments\" id=\"moderate_comments\" " + sModerate + " />" + _MessageHelper.GetMessage("lbl moderate comments");
            td_vf_commentstxt.InnerHtml += "<br />";
            td_vf_commentstxt.InnerHtml += "<input disabled=\"disabled\" type=\"checkbox\" name=\"require_authentication\" id=\"require_authentication\" " + sRequire + " />" + _MessageHelper.GetMessage("lbl require authentication");

            td_vf_updateservicestxt.InnerHtml += "<input type=\"checkbox\" name=\"notify_url\" id=\"notify_url\" " + sNotify + " disabled=\"disabled\" />" + _MessageHelper.GetMessage("lbl Notify blog");
            td_vf_updateservicestxt.InnerHtml += "<br />";
            td_vf_updateservicestxt.InnerHtml += _BlogData.NotifyURL;
        }
        else
        {
            td_vf_folderdesctxt.InnerHtml = _FolderData.Description;
        }

        if (_FolderData.StyleSheet == "")
        {
            td_vf_stylesheettxt.InnerHtml += _MessageHelper.GetMessage("none specified msg");
        }
        else
        {
            td_vf_stylesheettxt.InnerHtml += _SitePath + _FolderData.StyleSheet;
        }

        if (_FolderData.StyleSheetInherited)
        {
            td_vf_stylesheettxt.InnerHtml += "<div class=\"ektronCaption\">" + _MessageHelper.GetMessage("inherited style sheet msg") + "</div>";
        }
        DrawContentAliasesTable();
        IsContentSearchableSection();
        IsDisplaySettings();
        DrawContentTemplatesTable();
        DrawFolderTaxonomyTable(); //Assigned taxonomy
        long iTmpCaller = _ContentApi.RequestInformationRef.CallerId;
        try
        {
            _ContentApi.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin;
            _ContentApi.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin;

            AssetConfigInfo[] asset_config = _ContentApi.GetAssetMgtConfigInfo();
            if (asset_config[10].Value.IndexOf("ektron.com") > -1)
            {
                ltrCheckPdfServiceProvider.Text = _MessageHelper.GetMessage("pdf service provider");
            }
            else
            {
                ltrCheckPdfServiceProvider.Text = "";
            }
        }
        catch (Exception ex)
        {
            string _error = ex.Message;
        }
        finally
        {
            _ContentApi.RequestInformationRef.CallerId = iTmpCaller;
            _ContentApi.RequestInformationRef.UserId = iTmpCaller;
        }

        if (_FolderData.PublishPdfEnabled && (Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType != Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            if (_FolderData.PublishPdfActive)
            {
                td_vf_pdfactivetxt.InnerHtml += _MessageHelper.GetMessage("publish as pdf"); 
                ltrCheckPdfServiceProvider.Visible = true;
            }
            else
            {
                td_vf_pdfactivetxt.InnerHtml += _MessageHelper.GetMessage("lbl Publish native format");
                ltrCheckPdfServiceProvider.Visible = false;
            }
        }
        else
        {
            phPublishAsPdf.Visible = false;
        }

        // show domain info
        if (_FolderData.IsDomainFolder)
        {
            phProductionDomain.Visible = true;
            SettingsData settings_list;
            SiteAPI m_refSiteAPI = new SiteAPI();
            CommonApi m_refCommonAPI = new CommonApi();
            Ektron.Cms.Common.EkRequestInformation request_info;
            settings_list = m_refSiteAPI.GetSiteVariables(-1);
            request_info = m_refCommonAPI.RequestInformationRef;

            DomainFolder.Text += "<tr>";
            DomainFolder.Text += "<td class=\"label\">" + _MessageHelper.GetMessage("lbl Staging Domain") + ":</td>";
            DomainFolder.Text += "<td class=\"value\">http://" + _FolderData.DomainStaging + "</td>";
            DomainFolder.Text += "</tr>";
            DomainFolder.Text += "<tr>";
            DomainFolder.Text += "<td class=\"label\">" + _MessageHelper.GetMessage("lbl Production Domain") + ":</td>";
            DomainFolder.Text += "<td class=\"value\">http://" + _FolderData.DomainProduction + "</td>";
            DomainFolder.Text += "</tr>";

        }
        if (_FolderData.IsDomainFolder && _FolderData.ParentId == 0)
        {
            DrawFolderLocaleTaxonomyTable();
        }
		else
        {
            LocaleTaxonomy.Visible = false;
        }


        // show categories if its a blog
        if (isBlog)
        {
            if (!(_BlogData.Categories == null) && _BlogData.Categories.Length > 0)
            {
                for (this._i = 0; this._i <= _BlogData.Categories.Length - 1; this._i++)
                {
                    ltr_vf_categories.Text += _BlogData.Categories[this._i];
                    ltr_vf_categories.Text += "<br/>";
                }
            }
            else
            {
                ltr_vf_categories_lbl.Text = "No subjects";
            }

            if (!(_BlogData.BlogRoll == null) && _BlogData.BlogRoll.Length() > 0)
            {
                for (this._i = 0; this._i <= _BlogData.BlogRoll.Length() - 1; this._i++)
                {
                    Table tRoll = new Table();
                    tRoll.CssClass = "ektronGrid";
                    TableRow tRollRow = new TableRow();
                    TableCell tRollCell = new TableCell();
                    //Link Name
                    tRollCell = new TableCell();
                    tRollRow = new TableRow();
                    tRollCell.Text = "Link Name:";
                    tRollCell.CssClass = "label";
                    tRollRow.Controls.Add(tRollCell);
                    tRollCell = new TableCell();
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(this._i).LinkName;
                    tRollCell.CssClass = "readOnlyValue";
                    tRollRow.Controls.Add(tRollCell);
                    tRoll.Controls.Add(tRollRow);
                    //URL
                    tRollCell = new TableCell();
                    tRollRow = new TableRow();
                    tRollCell.Text = "URL:";
                    tRollCell.CssClass = "label";
                    tRollRow.Controls.Add(tRollCell);
                    tRollCell = new TableCell();
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(this._i).URL;
                    tRollCell.CssClass = "readOnlyValue";
                    tRollRow.Controls.Add(tRollCell);
                    tRoll.Controls.Add(tRollRow);
                    //Short Description
                    tRollCell = new TableCell();
                    tRollRow = new TableRow();
                    tRollCell.Text = "Short Description:";
                    tRollCell.CssClass = "label";
                    tRollRow.Controls.Add(tRollCell);
                    tRollCell = new TableCell();
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(this._i).ShortDescription;
                    tRollCell.CssClass = "readOnlyValue";
                    tRollRow.Controls.Add(tRollCell);
                    tRoll.Controls.Add(tRollRow);
                    //Relationship
                    tRollCell = new TableCell();
                    tRollRow = new TableRow();
                    tRollCell.Text = "Relationship:";
                    tRollCell.CssClass = "label";
                    tRollRow.Controls.Add(tRollCell);
                    tRollCell = new TableCell();
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(this._i).Relationship;
                    tRollCell.CssClass = "readOnlyValue";
                    tRollRow.Controls.Add(tRollCell);
                    tRoll.Controls.Add(tRollRow);
                    lbl_vf_roll.Controls.Add(tRoll);

                    Literal spacer = new Literal();
                    spacer.Text = "<div class=\'ektronTopSpace\'></div>";
                    lbl_vf_roll.Controls.Add(spacer);
                }
            }
        }

        if (_SettingsData.EnablePreApproval)
        {
            phPreapprovalGroup.Visible = true;
            cPreApproval = _ContentApi.EkContentRef.GetFolderPreapprovalGroup(_Id);
            if (-1 == Convert.ToInt32(cPreApproval["PreApprovalGroupID"]))
            {
                td_vf_preapprovaltxt.InnerHtml += cPreApproval["UserGroupName"] + " (Inherited)";
            }
            else if (0 == Convert.ToInt32(cPreApproval["PreApprovalGroupID"]))
            {
                td_vf_preapprovaltxt.InnerHtml += "(None)";
            }
            else
            {
                td_vf_preapprovaltxt.InnerHtml += cPreApproval["PreApprovalGroup"];
            }
        }

        // display replication settings for folder
        if (_ContentApi.RequestInformationRef.EnableReplication)
        {
            bool bShowReplicationMethod = true;
            if (_FolderData.ParentId != 0 && ((Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Blog || (Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum))
            {
                FolderData tmp_folder_data = null;
                tmp_folder_data = this._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId);
                if ((Ektron.Cms.Common.EkEnumeration.FolderType)tmp_folder_data.FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Community)
                {
                    bShowReplicationMethod = false;
                }
            }
            if (bShowReplicationMethod)
            {
                ReplicationMethod.Text = "<tr><td>&nbsp;</td></tr><tr><td class=\"label\">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr><tr><td>";
                if (_FolderData.ReplicationMethod == 1)
                {
                    ReplicationMethod.Text += _MessageHelper.GetMessage("replicate folder contents");
                }
                else
                {
                    ReplicationMethod.Text += _MessageHelper.GetMessage("generic No");
                }
                ReplicationMethod.Text += "	</td></tr>";
            }
        }

        // Show Custom-Field folder assignments:
        CustomFieldsApi customFieldsApi = new CustomFieldsApi();
        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            customFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage;
        }
        else
        {
            customFieldsApi.ContentLanguage = _ContentLanguage;
        }
        litMetadata.Text = customFieldsApi.GetEditableCustomFieldAssignments(_Id, false);
        customFieldsApi = null;
        DisplaySitemapPath();
        DisplaySubscriptionInfo();
        DrawContentTypesTable();
        if (_FolderType == 2) //OrElse m_intFolderId = 0 Avoiding root to be site aliased
        {
            phSiteAlias.Visible = true;
            DisplaySiteAlias();
        }
        Showpane();
    }

    private void DisplaySitemapPath()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        if (Convert.ToBoolean(_FolderData.SitemapInherited))
        {
            chkInheritSitemapPath.Checked = true;
        }
        else
        {
            chkInheritSitemapPath.Checked = false;
        }
        chkInheritSitemapPath.Disabled = true;
        if (_FolderData.Id == 0)
        {
            pnlInheritSitemapPath.Visible = false;
        }
        if (_FolderData.SitemapPath != null)
        {
            Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "controls/folder/sitemap.js", "EktronSitemapJS");

            sJS.Append("arSitemapPathNodes = new Array(");
            foreach (Ektron.Cms.Common.SitemapPath node in _FolderData.SitemapPath)
            {
                if (node != null)
                {
                    if (node.Order != 0)
                    {
                        sJS.Append(",");
                    }
                    sJS.Append("new node(\'" + Server.HtmlDecode(node.Title).Replace("\'", "\\\'") + "\',\'" + node.Url + "\',\'" + node.Description + "\'," + node.Order + ")");
                }
            }
            sJS.AppendLine(");");
            sJS.AppendLine("previewSitemapPath();");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "previewSitemapPath", sJS.ToString(), true);
        }
        else
        {
            chkInheritSitemapPath.Visible = false;
            ltInheritSitemapPath.Visible = true;
            ltInheritSitemapPath.Text = this._MessageHelper.GetMessage("lbl breadcrumb not created");
        }

    }

    private void DisplaySiteAlias()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();
        Ektron.Cms.SiteAliasApi _refSiteAliasApi = new Ektron.Cms.SiteAliasApi();
        System.Collections.Generic.List<Ektron.Cms.Common.SiteAliasData> siteAliasList = new System.Collections.Generic.List<Ektron.Cms.Common.SiteAliasData>();
        Ektron.Cms.PagingInfo page = new Ektron.Cms.PagingInfo();

        siteAliasList = _refSiteAliasApi.GetList(page, _FolderData.Id);
        viewSiteAliasList.InnerHtml = "<table width=\"100%\">";
        foreach (Ektron.Cms.Common.SiteAliasData item in siteAliasList)
        {
            viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml + "<tr><td><img src=\"" + _ContentApi.AppPath + "images/ui/icons/folderSite.png\" /></td>";
            viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml + "<td>" + item.SiteAliasName + "</td></tr>";
        }
        viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml + "</table>";
    }

    private void Showpane()
    {
        if (_ShowPane.Length > 0)
        {
            lbl_vf_showpane.Text += "<script language=\"Javascript\">" + Environment.NewLine;
            switch (_ShowPane)
            {
                case "blogroll":
                    lbl_vf_showpane.Text += "   ShowPane(\'dvRoll\');";
                    break;
            }
            lbl_vf_showpane.Text += "</script>" + Environment.NewLine;
        }
    }

    private void ViewFolderToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view properties for folder msg") + " \"" + _FolderData.Name + "\""));
        result.Append("<table><tr>");

		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

		bool primaryClassApplied = false;

        if (_PermissionData.CanEditFolders || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false))
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/contentEdit.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=EditFolder&id=" + _Id), _MessageHelper.GetMessage("alt edit properties button text (folder)"), _MessageHelper.GetMessage("btn edit prop"), "", StyleHelper.EditButtonCssClass, !primaryClassApplied));

			primaryClassApplied = true;
        }

        if (_PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false))
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/permissions.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt permissions button text (edit)"), _MessageHelper.GetMessage("btn view permissions"), "", StyleHelper.ViewPermissionsButtonCssClass, !primaryClassApplied));

			primaryClassApplied = true;
		}

        if (_SettingsData.EnablePreApproval)
        {
            if (_PermissionData.CanEditApprovals || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false))
            {
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/approvalPreapprove.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=EditPreApprovals&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt change preapp grp"), _MessageHelper.GetMessage("alt modify grp"), "", StyleHelper.PreApprovalButtonCssClass, !primaryClassApplied));

				primaryClassApplied = true;
			}
        }

        if (_PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false))
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/approvals.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewApprovals&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt approvals button text (edit)"), _MessageHelper.GetMessage("btn view approvals"), "", StyleHelper.ApprovalsButtonCssClass, !primaryClassApplied));
			
			primaryClassApplied = true;

            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/historyDelete.png", (string)("purgehist.aspx?LangType=" + _ContentLanguage + "&action=View&folderId=" + _Id), _MessageHelper.GetMessage("alt purge content hist"), _MessageHelper.GetMessage("btn purge history"), "", StyleHelper.DeleteHistoryButtonCssClass));
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/restore.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=RestoreInheritance&id=" + _Id), _MessageHelper.GetMessage("alt restore web alert"), _MessageHelper.GetMessage("lbl restore web"), "onclick=\"return ConfirmRestoreInheritance();\"", StyleHelper.RestoreButtonCssClass));
        }
        
		if (_EnableMultilingual == 1)
        {
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">");
            result.Append(_MessageHelper.GetMessage("view language"));
            result.Append("</td>");
            result.Append("<td>");
            result.Append(_StyleHelper.ShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", _ContentLanguage.ToString()));
            result.Append("&nbsp;</td>");
        }

		result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        if ((Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            result.Append(_StyleHelper.GetHelpButton((string)("calendar_" + _PageAction), ""));
        }
        else
        {
            result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction), ""));
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void DisplaySubscriptionInfo()
    {
        string strEnabled = " ";
        int i = 0;
        int findindex;
        Array arrSubscribed = null;
        string strNotifyA = "";
        string strNotifyI = "";
        string strNotifyN = "";
        long intInheritFrom;
        EmailFromData[] emailfrom_list;
        int y = 0;
        EmailMessageData[] defaultmessage_list;
        EmailMessageData[] unsubscribe_list;
        EmailMessageData[] optout_list;
        SettingsData settings_list;
        SiteAPI m_refSiteAPI = new SiteAPI();

        _SubscriptionData = _ContentApi.GetAllActiveSubscriptions(); //then get folder
        emailfrom_list = _ContentApi.GetAllEmailFrom();
        defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage);
        unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe);
        optout_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut);
        settings_list = m_refSiteAPI.GetSiteVariables(-1);

        intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id);
        if (intInheritFrom != _Id) //do we get settings from self
        {
            _GlobalSubInherit = true;
        }
        else
        {
            _GlobalSubInherit = false;
        }
        _SubscribedData = _ContentApi.GetSubscriptionsForFolder(intInheritFrom);
        _SubscriptionPropertiesData = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom);

        if ((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (_SubscriptionData == null) || (settings_list.AsynchronousLocation == ""))
        {
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">";
            lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert not setup");
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";

            if (emailfrom_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("lbl web alert emailfrom not setup") + "</font><br/>";
            }
            if (defaultmessage_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("lbl web alert def msg not setup") + "</font><br/>";
            }
            if (unsubscribe_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("lbl web alert unsub not setup") + "</font><br/>";
            }
            if (optout_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("lbl web alert optout not setup") + "</font><br/>";
            }
            if (_SubscriptionData == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("alt No subscriptions are enabled on the folder.") + "</font><br/>";
            }
            if (settings_list.AsynchronousLocation == "")
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") + "</font>";
            }
            return;
        }

        if (_SubscriptionPropertiesData == null)
        {
            _SubscriptionPropertiesData = new SubscriptionPropertiesData();
        }

        strEnabled = " disabled=\"disabled\" ";

        switch (_SubscriptionPropertiesData.NotificationType.GetHashCode())
        {
            case 0:
                strNotifyA = " checked=\"checked\" ";
                strNotifyI = "";
                strNotifyN = "";
                break;
            case 1:
                strNotifyA = "";
                strNotifyI = " checked=\"checked\" ";
                strNotifyN = "";
                break;
            case 2:
                strNotifyA = "";
                strNotifyI = "";
                strNotifyN = " checked=\"checked\" ";
                break;
        }

        if (_Id == 0) // root folder
        {
            lit_vf_subscription_properties.Text += "<input id=\"webalert_inherit_button\" type=\"hidden\" name=\"webalert_inherit_button\" value=\"webalert_inherit_button\" checked=\"checked\">";
        }
        else if (!_GlobalSubInherit) // not inheriting
        {
            lit_vf_subscription_properties.Text += "<input id=\"webalert_inherit_button\" type=\"checkbox\" name=\"webalert_inherit_button\" value=\"webalert_inherit_button\" disabled=\"disabled\">" + _MessageHelper.GetMessage("lbl inherit parent configuration");
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        }
        else // non root
        {
            lit_vf_subscription_properties.Text += "<input id=\"webalert_inherit_button\" type=\"checkbox\" name=\"webalert_inherit_button\" value=\"webalert_inherit_button\" checked=\"checked\" disabled=\"disabled\" >" + _MessageHelper.GetMessage("lbl inherit parent configuration");
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        }

        lit_vf_subscription_properties.Text += "<table class=\"ektronGrid\">";
        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert opt") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Always\" name=\"notify_option\" " + strNotifyA + " " + strEnabled + ">" + _MessageHelper.GetMessage("lbl web alert notify always");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Initial\" name=\"notify_option\"" + strNotifyI + " " + strEnabled + ">" + _MessageHelper.GetMessage("lbl web alert notify initial");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Never\" name=\"notify_option\"" + strNotifyN + " " + strEnabled + ">" + _MessageHelper.GetMessage("lbl web alert notify never");
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert subject") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        if (_SubscriptionPropertiesData.Subject != "")
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"" + _SubscriptionPropertiesData.Subject + "\" name=\"notify_subject\" " + strEnabled + ">";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"\" name=\"notify_subject\" " + strEnabled + ">";
        }
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        //lit_vf_subscription_properties.Text &= "Notification Base URL:"
        //If subscription_properties_list.URL <> "" Then
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """>"
        //Else
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """>"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert emailfrom address") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<select name=\"notify_emailfrom\" " + strEnabled + ">:";

        if ((emailfrom_list != null) && emailfrom_list.Length > 0)
        {
            for (y = 0; y <= emailfrom_list.Length - 1; y++)
            {
                if (emailfrom_list[y].Email == _SubscriptionPropertiesData.EmailFrom)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + emailfrom_list[y].Email + "\" SELECTED>" + emailfrom_list[y].Email + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + emailfrom_list[y].Email + "\">" + emailfrom_list[y].Email + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        //lit_vf_subscription_properties.Text &= "Notification File Location:"
        //If subscription_properties_list.WebLocation <> "" Then
        //lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" " & strEnabled & ">/"
        //Else
        //    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" " & strEnabled & ">/"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert contents") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<input id=\"use_optout_button\" type=\"checkbox\" checked=\"checked\" name=\"use_optout_button\" disabled=\"disabled\">" + _MessageHelper.GetMessage("lbl optout message") + "&nbsp;&nbsp;";

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_optoutid\">";
        if ((optout_list != null) && optout_list.Length > 0)
        {
            for (y = 0; y <= optout_list.Length - 1; y++)
            {
                if (optout_list[y].Id == _SubscriptionPropertiesData.OptOutID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + optout_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + optout_list[y].Id + "\">" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        if (_SubscriptionPropertiesData.DefaultMessageID > 0)
        {
            lit_vf_subscription_properties.Text += ("<input id=\"use_message_button\" type=\"checkbox\" checked=\"checked\" name=\"use_message_button\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use default message")) + "&nbsp;&nbsp;";
        }
        else
        {
            lit_vf_subscription_properties.Text += ("<input id=\"use_message_button\" type=\"checkbox\" name=\"use_message_button\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use default message")) + "&nbsp;&nbsp;";
        }

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_messageid\">";
        if ((defaultmessage_list != null) && defaultmessage_list.Length > 0)
        {
            for (y = 0; y <= defaultmessage_list.Length - 1; y++)
            {
                if (defaultmessage_list[y].Id == _SubscriptionPropertiesData.DefaultMessageID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + defaultmessage_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + defaultmessage_list[y].Id + "\">" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        if (_SubscriptionPropertiesData.SummaryID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" checked=\"checked\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use summary message");
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use summary message");
        }
        lit_vf_subscription_properties.Text += "<br />";
        if (_SubscriptionPropertiesData.ContentID == -1)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" name=\"titlename\" value=\"[[use current]]\" " + strEnabled + " size=\"65\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"-1\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }
        else if (_SubscriptionPropertiesData.ContentID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" name=\"titlename\" value=\"" + _SubscriptionPropertiesData.UseContentTitle.ToString() + "\" " + strEnabled + " size=\"65\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"" + _SubscriptionPropertiesData.ContentID.ToString() + "\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" " + strEnabled + ">" + _MessageHelper.GetMessage("lbl use content message");
            lit_vf_subscription_properties.Text += "<input type=\"text\" name=\"titlename\" onkeydown=\"return false\" value=\"\" " + strEnabled + " size=\"65\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"0\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }
        lit_vf_subscription_properties.Text += "<br />";
        if (_SubscriptionPropertiesData.UseContentLink > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" checked=\"checked\" " + strEnabled + ">Use Content Link";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" " + strEnabled + ">Use Content Link";
        }

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        lit_vf_subscription_properties.Text += "<input id=\"use_unsubscribe_button\" type=\"checkbox\" checked=\"checked\" name=\"use_unsubscribe_button\" disabled=\"disabled\">" + _MessageHelper.GetMessage("lbl unsubscribe message") + "&nbsp;&nbsp;";

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_unsubscribeid\">";
        if ((unsubscribe_list != null) && unsubscribe_list.Length > 0)
        {
            for (y = 0; y <= unsubscribe_list.Length - 1; y++)
            {
                if (unsubscribe_list[y].Id == _SubscriptionPropertiesData.UnsubscribeID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + unsubscribe_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + unsubscribe_list[y].Id + "\">" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";

        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";
        lit_vf_subscription_properties.Text += "</table>";

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        lit_vf_subscription_properties.Text += "<div class=\"ektronHeader\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl avail web alert");
        lit_vf_subscription_properties.Text += "</div>";

        if (!(_SubscriptionData == null))
        {
            lit_vf_subscription_properties.Text += "<table id=\"cfld_subscription_assignment\" class=\"ektronGrid\" width=\"100%\">";
            lit_vf_subscription_properties.Text += "<tbody>";
            lit_vf_subscription_properties.Text += "<tr class=\"title-header\">";
            lit_vf_subscription_properties.Text += "<th width=\"10%\">" + _MessageHelper.GetMessage("lbl assigned") + "</th>";
            lit_vf_subscription_properties.Text += "<th>" + _MessageHelper.GetMessage("lbl name") + "</th>";
            lit_vf_subscription_properties.Text += "</tr>";
            if (!(_SubscribedData == null))
            {
                arrSubscribed = Array.CreateInstance(typeof(long), _SubscribedData.Length);
                for (i = 0; i <= _SubscribedData.Length - 1; i++)
                {
                    arrSubscribed.SetValue(_SubscribedData[i].Id, i);
                }
                if (arrSubscribed != null)
                {
                    if (arrSubscribed.Length > 0)
                    {
                        Array.Sort(arrSubscribed);
                    }
                }
            }
            i = 0;

            for (i = 0; i <= _SubscriptionData.Length - 1; i++)
            {
                findindex = -1;
                if ((_SubscribedData != null) && (arrSubscribed != null))
                {
                    findindex = Array.BinarySearch(arrSubscribed, _SubscriptionData[i].Id);
                }

                lit_vf_subscription_properties.Text += "<tr>";

                if (findindex < 0)
                {
                    lit_vf_subscription_properties.Text += "<td width=\"10%\" class=\"center\"><input type=\"checkbox\" name=\"Assigned_" + _SubscriptionData[i].Id + "\"  id=\"Assigned_" + _SubscriptionData[i].Id + "\" " + strEnabled + "></td>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<td class=\"center\"><input type=\"checkbox\" name=\"Assigned_" + _SubscriptionData[i].Id + "\"  id=\"Assigned_" + _SubscriptionData[i].Id + "\" checked=\"checked\" " + strEnabled + "></td>";
                }
                lit_vf_subscription_properties.Text += "<td>" + _SubscriptionData[i].Name + "</td>";
                lit_vf_subscription_properties.Text += "</tr>";
            }
            lit_vf_subscription_properties.Text += "</tbody></table>";
        }
        else
        {
            lit_vf_subscription_properties.Text += "Nothing available.";
        }
        lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"content_sub_assignments\" id=\"content_sub_assignments\" value=\"\">";
        DrawContentTemplatesTable();
    }

    #endregion

    #region Content Type Selection

    private string DrawContentTypesBreaker(bool @checked)
    {
        if (_FolderData.Id == 0)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" checked disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
    }

    private string DrawContentTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table class=\"ektronGrid\"><tbody id=\"contentTypeTable\">");
        str.Append("<tr class=\"title-header\">");
        str.Append("<th></th>");
        str.Append("<th class=\"left\">" + this._ContentApi.EkMsgRef.GetMessage("lbl folder contenttype header") + "</th>");
        str.Append("</tr>");
        return str.ToString();
    }

    private string DrawContentTypesEntry(int row_id, string name, long xml_id, bool isDefault)
    {
        StringBuilder str = new StringBuilder();

        str.Append("<tr id=\"row_" + xml_id + "\">");

        str.Append("<td class=\"center\" width=\"10%\">");
        if (isDefault)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" disabled />");
        }
        str.Append("</td>");
        str.Append("<td width=\"90%\">");
        str.Append(name + "<input id=\"input_" + xml_id + "\" name=\"input_" + xml_id + "\" type=\"hidden\" value=\"" + xml_id + "\" />");
        str.Append("</td>");
        str.Append("</tr>");

        return str.ToString();
    }

    private string DrawContentTypesFooter()
    {
        return "</tbody></table>";
    }

    private void DrawContentTypesTable()
    {
        if ((Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            phContentType.Visible = false;
            return;
        }

        XmlConfigData[] xml_config_list;
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy);
        XmlConfigData[] active_xml_list;
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id);
        Collection addNew = new Collection();
        int k = 0;
        int row_id = 0;

        bool smartFormsRequired = System.Convert.ToBoolean(!Utilities.IsNonFormattedContentAllowed(active_xml_list));
        bool isEnabled = IsInheritingXmlMultiConfig();

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawContentTypesBreaker(isEnabled));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<div>");
        str.Append(DrawContentTypesHeader());

        Collection ActiveXmlIdList = new Collection();
        for (k = 0; k <= active_xml_list.Length - 1; k++)
        {
            if (!ActiveXmlIdList.Contains(active_xml_list[k].Id.ToString()))
            {
                ActiveXmlIdList.Add(active_xml_list[k].Id, active_xml_list[k].Id.ToString(), null, null);
            }
        }
        if (_FolderData.XmlConfiguration != null)
        {
            for (int j = 0; j <= (_FolderData.XmlConfiguration.Length - 1); j++)
            {
                if (!ActiveXmlIdList.Contains(_FolderData.XmlConfiguration[j].Id.ToString()))
                {
                    ActiveXmlIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId.ToString(), null, null);
                }
            }
        }

        for (k = 0; k <= xml_config_list.Length - 1; k++)
        {
            if (ActiveXmlIdList.Contains(xml_config_list[k].Id.ToString()))
            {
                str.Append(DrawContentTypesEntry(row_id, xml_config_list[k].Title, xml_config_list[k].Id, Utilities.IsDefaultXmlConfig(xml_config_list[k].Id, active_xml_list)));
                row_id++;
            }
            else
            {
                Collection cRow = new Collection();
                cRow.Add(xml_config_list[k].Title, "xml_name", null, null);
                cRow.Add(xml_config_list[k].Id, "xml_id", null, null);
                addNew.Add(cRow, null, null, null);
            }
        }

        if (!smartFormsRequired)
        {
            str.Append(DrawContentTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_xml_list)));
        }

        str.Append(DrawContentTypesFooter());
        str.Append("</div>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }
        ltr_vf_smartforms.Text = str.ToString();
    }

    #endregion

    #region Catalog

    private void Display_ViewCatalog()
    {
        Collection cPreApproval;

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);

        ltrTypes.Text = _MessageHelper.GetMessage("lbl product types");
        //Sitemap Path
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration");

        ViewCatalogToolBar();

        td_vf_foldertxt.InnerHtml = _FolderData.Name;
        td_vf_idtxt.InnerHtml = _Id.ToString();
        td_vf_folderdesctxt.InnerHtml = _FolderData.Description;

        if (_FolderData.StyleSheet == "")
        {
            td_vf_stylesheettxt.InnerHtml += _MessageHelper.GetMessage("none specified msg");
        }
        else
        {
            td_vf_stylesheettxt.InnerHtml += _SitePath + _FolderData.StyleSheet;
        }

        if (_FolderData.StyleSheetInherited)
        {
            td_vf_stylesheettxt.InnerHtml += " " + _MessageHelper.GetMessage("style sheet inherited");
        }

        DrawContentTemplatesTable();
        DrawFolderTaxonomyTable(); //Assigned taxonomy
        DrawContentAliasesTable();
        IsContentSearchableSection();
        IsDisplaySettings();
        if (_SettingsData.EnablePreApproval)
        {
            phPreapprovalGroup.Visible = true;
            cPreApproval = _ContentApi.EkContentRef.GetFolderPreapprovalGroup(_Id);
            if (-1 == Convert.ToInt32(cPreApproval["PreApprovalGroupID"]))
            {
                td_vf_preapprovaltxt.InnerHtml += cPreApproval["UserGroupName"] + " (Inherited)";
            }
            else if (0 == Convert.ToInt32(cPreApproval["PreApprovalGroupID"]))
            {
                td_vf_preapprovaltxt.InnerHtml += "(None)";
            }
            else
            {
                td_vf_preapprovaltxt.InnerHtml += cPreApproval["PreApprovalGroup"];
            }
        }

        // display replication settings for folder
        if (_ContentApi.RequestInformationRef.EnableReplication)
        {
            bool bShowReplicationMethod = true;
            if (_FolderData.ParentId != 0 && (Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Blog || (Ektron.Cms.Common.EkEnumeration.FolderType)_FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum)
            {
                FolderData tmp_folder_data = null;
                tmp_folder_data = this._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId);
                if ((Ektron.Cms.Common.EkEnumeration.FolderType)tmp_folder_data.FolderType == Ektron.Cms.Common.EkEnumeration.FolderType.Community)
                {
                    bShowReplicationMethod = false;
                }
            }
            if (bShowReplicationMethod)
            {
                ReplicationMethod.Text = "<tr><td>&nbsp;</td></tr><tr><td class=\"label\">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr><tr><td>";
                if (_FolderData.ReplicationMethod == 1)
                {
                    ReplicationMethod.Text += _MessageHelper.GetMessage("replicate folder contents");
                }
                else
                {
                    ReplicationMethod.Text += _MessageHelper.GetMessage("generic No");
                }
                ReplicationMethod.Text += "	</td></tr>";
            }
        }

        // Show Custom-Field folder assignments:
        CustomFieldsApi customFieldsApi = new CustomFieldsApi();
        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            customFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage;
        }
        else
        {
            customFieldsApi.ContentLanguage = _ContentLanguage;
        }
        litMetadata.Text = customFieldsApi.GetEditableCustomFieldAssignments(_Id, false, Ektron.Cms.Common.EkEnumeration.FolderType.Catalog);
        LocaleTaxonomy.Visible = false;
        officedocumentspanel.Visible = false;
		customFieldsApi = null;
        DisplaySitemapPath();
        DisplaySubscriptionInfo();
        DrawProductTypesTable();
    }
    private void ViewCatalogToolBar()
    {

        bool IsInCommerceAdminRole = System.Convert.ToBoolean(_PermissionData.IsAdmin || _ContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin));
        bool IsInFolderAdminRole = System.Convert.ToBoolean(_PermissionData.IsAdmin || IsInCommerceAdminRole || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false));
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view properties for catalog msg") + " \"" + _FolderData.Name + "\""));
        result.Append("<table><tr>");

		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

		bool primaryClassApplied = false;

        if (IsInFolderAdminRole)
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/contentEdit.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=EditFolder&id=" + _Id), _MessageHelper.GetMessage("alt edit properties button text (catalog)"), _MessageHelper.GetMessage("btn edit prop"), "", StyleHelper.EditButtonCssClass, !primaryClassApplied));

			primaryClassApplied = true;
        }

        if (IsInFolderAdminRole)
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/permissions.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt permissions button text (catalog)"), _MessageHelper.GetMessage("btn view permissions"), "", StyleHelper.ViewPermissionsButtonCssClass, !primaryClassApplied));

			primaryClassApplied = true;
        }

        if (_SettingsData.EnablePreApproval)
        {
            if (_PermissionData.CanEditApprovals || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false))
            {
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/approvalPreapprove.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=EditPreApprovals&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt change preapp grp"), _MessageHelper.GetMessage("alt modify grp"), "", StyleHelper.PreApprovalButtonCssClass, !primaryClassApplied));

				primaryClassApplied = true;
            }
        }

        if (IsInFolderAdminRole)
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/approvals.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewApprovals&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt approvals button text (catalog)"), _MessageHelper.GetMessage("btn view approvals"), "", StyleHelper.ViewApprovalsButtonCssClass, !primaryClassApplied));

			primaryClassApplied = true;

            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/historyDelete.png", (string)("purgehist.aspx?LangType=" + _ContentLanguage + "&action=View&folderId=" + _Id), _MessageHelper.GetMessage("alt purge entry hist"), _MessageHelper.GetMessage("btn purge history"), "", StyleHelper.DeleteHistoryButtonCssClass));
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/restore.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=RestoreInheritance&id=" + _Id), _MessageHelper.GetMessage("alt restore catalog web alert"), _MessageHelper.GetMessage("lbl restore web"), "onclick=\"return ConfirmRestoreInheritance();\"", StyleHelper.RestoreButtonCssClass));
        }
        
		if (_EnableMultilingual == 1)
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"right\">" + _MessageHelper.GetMessage("view language") + "&#160;" + _StyleHelper.ShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", _ContentLanguage.ToString()) + "</td>");
        }
        else
        {
            result.Append("<td>&nbsp;</td>");
        }

		result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private string DrawProductTypesBreaker(bool @checked)
    {
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" checked disabled autocomplete=\'off\' />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration") + "";
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" disabled autocomplete=\'off\' />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration") + "";
        }
    }
    private string DrawProductTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table class=\"ektronGrid\" width=\"100%\"><tbody>");
        str.Append("    <tr class=\"title-header\">");
        str.Append("        <td width=\"10%\" class=\"center\">");
        str.Append(_MessageHelper.GetMessage("lbl default"));
        str.Append("        </td>");
        str.Append("        <td width=\"90%\">");
        str.Append(_MessageHelper.GetMessage("lbl prod type"));
        str.Append("        </td>");
        str.Append("    </tr>");
        str.Append("</tbody></table>");
        str.Append("<table class=\"ektronGrid\" width=\"100%\"><tbody id=\"contentTypeTable\" name=\"contentTypeTable\">");
        return str.ToString();
    }
    private string DrawProductTypesEntry(int row_id, string name, long xml_id, bool isDefault)
    {
        StringBuilder str = new StringBuilder();

        str.Append("<tr id=\"row_" + xml_id + "\">");

        str.Append("<td class=\"center\" width=\"10%\">");
        if (isDefault)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" disabled />");
        }
        str.Append("<td width=\"90%\">");
        str.Append(name + "<input id=\"input_" + xml_id + "\" name=\"input_" + xml_id + "\" type=\"hidden\" value=\"" + xml_id + "\" /></td>");
        str.Append("</tr>");

        return str.ToString();
    }
    private void DrawProductTypesTable()
    {
        _ProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);

        List<ProductTypeData> prod_type_list = new List<ProductTypeData>();
        Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();
        criteria.PagingInfo.RecordsPerPage = 1000;
        prod_type_list = _ProductType.GetList(criteria);

        List<ProductTypeData> active_prod_list = new List<ProductTypeData>();
        active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id);
        Collection addNew = new Collection();
        int k = 0;
        int row_id = 0;

        bool smartFormsRequired = true;
        bool isEnabled = IsInheritingXmlMultiConfig();

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawProductTypesBreaker(isEnabled));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<div class=\"\">");
        str.Append(DrawProductTypesHeader());

        Collection ActiveXmlIdList = new Collection();
        for (k = 0; k <= active_prod_list.Count - 1; k++)
        {
            if (!ActiveXmlIdList.Contains(active_prod_list[k].Id.ToString()))
            {
                ActiveXmlIdList.Add(active_prod_list[k].Id, active_prod_list[k].Id.ToString(), null, null);
            }
        }
        if (_FolderData.XmlConfiguration != null)
        {
            for (int j = 0; j <= (_FolderData.XmlConfiguration.Length - 1); j++)
            {
                if (!ActiveXmlIdList.Contains(_FolderData.XmlConfiguration[j].Id.ToString()))
                {
                    ActiveXmlIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId.ToString(), null, null);
                }
            }
        }

        for (k = 0; k <= prod_type_list.Count - 1; k++)
        {
            if (ActiveXmlIdList.Contains(prod_type_list[k].Id.ToString()))
            {
                str.Append(DrawProductTypesEntry(row_id, (string)(prod_type_list[k].Title), prod_type_list[k].Id, Utilities.IsDefaultXmlConfig(prod_type_list[k].Id, active_prod_list.ToArray())));
                row_id++;
            }
            else
            {
                Collection cRow = new Collection();
                cRow.Add(prod_type_list[k].Title, "xml_name", null, null);
                cRow.Add(prod_type_list[k].Id, "xml_id", null, null);
                addNew.Add(cRow, null, null, null);
            }
        }

        if (!smartFormsRequired)
        {
            str.Append(DrawProductTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_prod_list.ToArray())));
        }

        str.Append(DrawContentTypesFooter());
        str.Append("</div>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }
        ltr_vf_smartforms.Text = str.ToString();
    }
    #endregion

    #region flagging section
    private void DrawFlaggingOptions()
    {
        StringBuilder str = new StringBuilder();

        try
        {
            str.Append("<input type=\"checkbox\" id=\"flagging_options_inherit_cbx\" name=\"flagging_options_inherit_cbx\" disabled=\"disabled\" " + ((_FolderData.FlagInherited && (!(_FolderData.Id == 0))) ? "checked=\"checked\" " : "") + " onclick=\"InheritFlagingChanged()\" />" + this._MessageHelper.GetMessage("lbl Inherit Parent Configuration"));
            str.Append("<div class=\"ektronTopSpace\"></div>");
            //If ((Not folder_data.FlagInherited) Or (folder_data.Id = 0)) Then
            //             str.Append("<table width=""100%"" >" + Environment.NewLine)
            //             str.Append("  <tr>" + Environment.NewLine)
            //             str.Append("    <td>" + Environment.NewLine)
            //             str.Append("      <table width=""100%"">" + Environment.NewLine)
            //             str.Append("        <tr>" + Environment.NewLine)
            //             str.Append("          <td>" + Environment.NewLine)
            //             str.Append("            <table class=""center"" width=""100%"">" + Environment.NewLine)
            //             str.Append("              <tr>" + Environment.NewLine)
            //             str.Append("                <td width=""50%"">" + Environment.NewLine)
            //             str.Append("                  <table width=""100%"">" + Environment.NewLine)
            //             str.Append("                    <tr>" + Environment.NewLine)
            //             str.Append("                      <td width=""50%"">" + Environment.NewLine)
            //             str.Append("                        " & m_refMsg.GetMessage("lbl assigned flags") & ": " & Environment.NewLine)
            //             str.Append("                        <select name=""flagging_options_assigned"" id=""flagging_options_assigned"" multiple=""multiple""" + Environment.NewLine)
            //             str.Append("                           disabled=""disabled"" size=""3"" style=""width: 100%"">" + Environment.NewLine)
            //             '
            //             ' Generate an option for each assigned flag:
            //             str.Append(GetAssignedFlags(True) + Environment.NewLine)
            //             str.Append("                        </select>" + Environment.NewLine)
            //             str.Append("                      </td>" + Environment.NewLine)
            //             str.Append("                      <td class=""center"">" + Environment.NewLine)
            //             str.Append("                      <td>" + Environment.NewLine)
            //             str.Append("                      </td>" + Environment.NewLine)
            //             str.Append("                    </tr>" + Environment.NewLine)
            //             str.Append("                  </table>" + Environment.NewLine)
            //             str.Append("                </td>" + Environment.NewLine)
            //             str.Append("              </tr>" + Environment.NewLine)
            //             str.Append("            </table>" + Environment.NewLine)
            //             str.Append("          </td>" + Environment.NewLine)
            //             str.Append("        </tr>" + Environment.NewLine)
            //             str.Append("      </table>" + Environment.NewLine)
            //             str.Append("    </td>" + Environment.NewLine)
            //             str.Append("  </tr>" + Environment.NewLine)
            //             str.Append("</table>" + Environment.NewLine)
            //         End If
            if ((_FolderData.FolderFlags != null) && _FolderData.FolderFlags.Length > 0)
            {
                str.Append(_FolderData.FolderFlags[0].Name);
            }
            else
            {
                str.Append(this._ContentApi.EkMsgRef.GetMessage("lbl folder no flag msg"));
            }

            flagging_options.Text = str.ToString();

        }
        catch (Exception)
        {
        }
        finally
        {
            str = null;
        }
    }

    protected string GetAssignedFlags(bool showDefault)
    {
        string returnValue;
        StringBuilder result = new StringBuilder();
        FolderFlagDefData[] flags;

        FolderFlagDefData tempFlag;
        int idx = 0;

        try
        {
            flags = (FolderFlagDefData[])_FolderData.FolderFlags; //flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)

            // reorder, placing the default first:
            for (idx = 1; idx <= flags.Length - 1; idx++)
            {
                if (flags[idx].IsDefault)
                {
                    tempFlag = flags[idx];
                    flags[idx] = flags[0];
                    flags[0] = tempFlag;
                }
            }

            foreach (FolderFlagDefData flag in flags)
            {
                if (showDefault && (flag.IsDefault))
                {
                    result.Append("<option value=\"" + flag.ID.ToString() + "\">" + flag.Name + " (default)" + "</option>" + Environment.NewLine);
                }
                else
                {
                    result.Append("<option value=\"" + flag.ID.ToString() + "\">" + flag.Name + "</option>" + Environment.NewLine);
                }
            }

        }
        catch (Exception)
        {
        }
        finally
        {
            returnValue = result.ToString();
            result = null;
        }
        return returnValue;
    }

    #endregion

    #region multi-template selection
    private string DrawContentTemplatesBreaker(bool @checked)
    {
        if (_FolderData.Id == 0)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else if (@checked)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" checked disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
    }

    private string DrawContentTemplatesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table width=\"100%\" class=\"ektronGrid\"><tbody id=\"templateTable\">");
        str.Append("<tr class=\"title-header\">");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append(_MessageHelper.GetMessage("lbl default"));
        str.Append("</td>");
        str.Append("<td width=\"90%\" class=\"left\">");
        str.Append(_MessageHelper.GetMessage("lbl Page Template Name"));
        str.Append("</td>");
        str.Append("</tr>");
        //str.Append("</tbody></table>")
        //str.Append("<table width=""100%"" class=""ektronGrid""><tbody >")
        return str.ToString();
    }

    private string DrawContentTemplatesEntry(int row_id, string name, long template_id, bool isEnabled)
    {
        StringBuilder str = new StringBuilder();
        bool isDefault = false;

        if (template_id == _FolderData.TemplateId)
        {
            isDefault = true;
        }

        str.Append("<tr id=\"trow_" + template_id + "\">");

        str.Append("<td width=\"10%\" class=\"center\">");
        if (isDefault == true && isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" checked disabled />");
        }
        else if (isDefault == true && !isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" checked disabled />");
        }
        else if (!isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" disabled />");
        }

        str.Append("</td>");
        str.Append("<td width=\"90%\" colspan=\"2\">");
        str.Append(name);
        Ektron.Cms.PageBuilder.WireframeData wfd = new Ektron.Cms.PageBuilder.WireframeModel().FindByTemplateID(template_id);
        if (wfd != null)
        {
            if (wfd.Template.SubType == EkEnumeration.TemplateSubType.Wireframes)
            {
                str.Append(" (" + _MessageHelper.GetMessage("lbl pagebuilder wireframe template") + ")");
            }
            else if (wfd.Template.SubType == EkEnumeration.TemplateSubType.MasterLayout)
            {
                str.Append(" (" + _MessageHelper.GetMessage("lbl pagebuilder master layouts") + ")");
            }
        }
        str.Append("<input id=\"tinput_" + template_id + "\" name=\"tinput_" + template_id + "\" type=\"hidden\" value=\"" + template_id + "\" />");
        str.Append("</td>");
        str.Append("</tr>");

        return str.ToString();
    }

    private string DrawContentTemplatesFooter()
    {
        return "</tbody></table>";
    }

    private void IsContentSearchableSection()
    {
        //-------------IscontentSearchable-------------           
        StringBuilder sb = new StringBuilder();
      //  sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _MessageHelper.GetMessage("lbl Content Searchable") + ":</strong></td><td colspan=\"2\">");
        if (_FolderData.IsContentSearchableInherited)
        {
            sb.Append("<input type=\"checkbox\" id=\"chkInheritIscontentSearchable\" checked=\"checked\" disabled=\"disabled\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
        }
        else
        {
            sb.Append("<input type=\"checkbox\" id=\"chkInheritIscontentSearchable\" disabled=\"disabled\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
        }       
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        //sb.Append("</tr></td>");
        //sb.Append(" <tr class=\"evenrow\"><td colspan=\"2\">");
        if (_FolderData.IscontentSearchable)
        {
            sb.Append("&nbsp;    <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
        }
        else
        {
            sb.Append("&nbsp;    <input disabled=\"disabled\" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
        }
        sb.Append("   " + _MessageHelper.GetMessage("lbl Content Searchable"));
        sb.Append(" </td></tr>");
       // sb.Append("</table>");
        ltrContSearch2.Text = sb.ToString();
         phContSearch2.Visible = true;
        //--------------------IscontentSearchableEnd-------------
    }
    private void IsDisplaySettings()
    {
        //-------------IsDisplaySettings-------------       
        StringBuilder sb = new StringBuilder();
        //  sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _MessageHelper.GetMessage("lbl Display Settings") + ":</strong></td><td colspan=\"2\">");
        if (_FolderData.IsDisplaySettingsInherited)
        {
            sb.Append("<input type=\"checkbox\" id=\"chkInheritIsDisplaySettings\" name=\"chkInheritIsDisplaySettings\" checked=\"checked\" disabled=\"disabled\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
        }
        else
        {
            sb.Append("<input type=\"checkbox\" id=\"chkInheritIsDisplaySettings\" disabled=\"disabled\" />  "+ _MessageHelper.GetMessage("lbl inherit parent configuration"));
        }
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        //sb.Append("</tr></td>");
        //sb.Append(" <tr class=\"evenrow\"><td colspan=\"2\">");      
        if (((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && _FolderData.DisplaySettings ==0)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.AllTabs + " name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.AllTabs + " name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAllTabs required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Summary + " name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Summary + " name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSummary required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.MetaData + " name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.MetaData + " name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsMetaData required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Aliasing + " name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Aliasing + " name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAliasing required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Schedule + " type=\"checkbox\" name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Schedule + " name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSchedule required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Comment + " type=\"checkbox\" name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Comment + " name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsComment required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Templates + " name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Templates + " name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTemplates required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy + " name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" value=" + (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy + " name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
        }
        sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTaxonomy required"));
       // sb.Append(" <div class=\"ektronCaption\"><strong>Please Note: </strong>If you CHECK the 'Tabs' check box, (.)</div></td></tr>");
        sb.Append(" </td></tr>");
        // sb.Append("</table>");
        ltrDisplaySettings2.Text = sb.ToString();
        phDisplaySettings2.Visible = true;
        //--------------------IsDisplaySettingsEnd-------------
    }
    private void DrawContentAliasesTable()
    {
        bool _isManualAliasEnabled = true;
        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _aliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
            if (_aliasSettings.IsManualAliasEnabled)
            {
                StringBuilder sb = new StringBuilder();
                if (_FolderData.AliasInherited)
                {
                    sb.Append("<input type=\"checkbox\" id=\"chkInheritAliases\" checked=\"checked\" disabled=\"disabled\" /> "+ _MessageHelper.GetMessage("lbl Inherit Parent Configuration"));
                }
                else
                {
                    sb.Append("<input type=\"checkbox\" id=\"chkInheritAliases\" disabled=\"disabled\" /> "+ _MessageHelper.GetMessage("lbl Inherit Parent Configuration"));
                }
                sb.Append("<div class=\"ektronTopSpace\"></div>");
                sb.Append("<table class=\"ektronForm\">");
                sb.Append(" <tr class=\"evenrow\"><td>");
                if (_FolderData.AliasRequired)
                {
                    sb.Append("     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                }
                else
                {
                    sb.Append("     <input disabled=\"disabled\" type=\"checkbox\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                }
                sb.Append("    " + _MessageHelper.GetMessage("lbl manual alias required"));
                sb.Append(" </td></tr>");
                sb.Append("</table>");
                ltrFolderAliases2.Text = sb.ToString();
            }
            else
            {
                _isManualAliasEnabled = false;
            }
        }
        else
        {
            _isManualAliasEnabled = false;
        }
        phFolderAliases.Visible = phFolderAliases2.Visible = _isManualAliasEnabled;
    }
    private void DrawContentTemplatesTable()
    {
        TemplateData[] active_templates;
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id);

        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");

        int k = 0;
        int row_id = 0;
        Collection addNew = new Collection();
        bool isEnabled = IsInheritingTemplateMultiConfig();

        StringBuilder str = new StringBuilder();

        str.Append(DrawContentTemplatesBreaker(isEnabled));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<div>");
        str.Append(DrawContentTemplatesHeader());

        DrawFlaggingOptions();

        Collection ActiveTemplateIdList = new Collection();
        for (k = 0; k <= active_templates.Length - 1; k++)
        {
            if (!ActiveTemplateIdList.Contains(active_templates[k].Id.ToString()))
            {
                ActiveTemplateIdList.Add(active_templates[k].Id, active_templates[k].Id.ToString(), null, null);
            }
        }

        if (!ActiveTemplateIdList.Contains(_FolderData.TemplateId.ToString()))
        {
            ActiveTemplateIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId.ToString(), null, null);
        }

        for (k = 0; k <= template_data.Length - 1; k++)
        {
            if (ActiveTemplateIdList.Contains(template_data[k].Id.ToString()))
            {
                str.Append(DrawContentTemplatesEntry(row_id, template_data[k].FileName, template_data[k].Id, System.Convert.ToBoolean(!isEnabled)));
                row_id++;
            }
            else
            {
                Collection cRow = new Collection();
                cRow.Add(template_data[k].FileName, "template_name", null, null);
                cRow.Add(template_data[k].Id, "template_id", null, null);
                addNew.Add(cRow, null, null, null);
            }
        }

        str.Append(DrawContentTemplatesFooter());
        str.Append("</div>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"0\" />");
        }
        template_list.Text = str.ToString();
    }
    private long checktaxid = 0;
    private void DrawFolderTaxonomyTable()
    {
        string categorydatatemplate = "<input type=\"checkbox\" id=\"taxlist\" name=\"taxlist\" value=\"{0}\" {1} disabled/>{2}";
        StringBuilder categorydata = new StringBuilder();
        TaxonomyBaseData[] TaxArr = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content);
        if ((TaxArr != null) && TaxArr.Length > 0)
        {
            int i = 0;
            while (i < TaxArr.Length)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (i < TaxArr.Length)
                    {
                        checktaxid = TaxArr[i].TaxonomyId;
                        Predicate<TaxonomyBaseData> Taxpredicate = TaxonomyExists;
                        categorydata.Append(string.Format(categorydatatemplate, TaxArr[i].TaxonomyId, IsChecked(Array.Exists(_FolderData.FolderTaxonomy, Taxpredicate)), TaxArr[i].TaxonomyName));
                    }
                    else
                    {
                        break;
                    }
                    i++;
                    categorydata.Append("<br/>");
                }
            }
        }

        StringBuilder str = new StringBuilder();
        str.Append("<input name=\"TaxonomyTypeBreak\" id=\"TaxonomyTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TaxonomyTypeBreak\')\" " + IsChecked(_FolderData.TaxonomyInherited) + " disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration"));
        str.Append("<br/>");
        str.Append("<input name=\"CategoryRequired\" id=\"CategoryRequired\" type=\"checkbox\" " + IsChecked(_FolderData.CategoryRequired) + "  disabled />" + _MessageHelper.GetMessage("alt Required at least one category selection"));
        str.Append("<br/>");
        str.Append("<br/>");
        str.Append(categorydata.ToString());
        taxonomy_list.Text = str.ToString();
    }
    private void DrawFolderLocaleTaxonomyTable()
    {
        string categorydatatemplate = "<input type=\"checkbox\" id=\"localeTaxlist\" name=\"localeTaxlist\" value=\"{0}\" {1} disabled />{2}";
        StringBuilder categorydata = new StringBuilder();
        Ektron.Cms.TaxonomyRequest taxRequest = new Ektron.Cms.TaxonomyRequest();
        taxRequest.TaxonomyId = 0;
        taxRequest.TaxonomyLanguage = this._ContentLanguage;
        taxRequest.TaxonomyType = EkEnumeration.TaxonomyType.Locale;
        TaxonomyBaseData[] TaxArr = _ContentApi.EkContentRef.ReadAllSubCategories(taxRequest);
        // Dim TaxArr As TaxonomyBaseData() = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Common.EkEnumeration.TaxonomyType.Locale)
        if ((TaxArr != null) && TaxArr.Length > 0)
        {
            int i = 0;
            while (i < TaxArr.Length)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (i < TaxArr.Length)
                    {
                        checktaxid = TaxArr[i].TaxonomyId;
                        Predicate<TaxonomyBaseData> Taxpredicate = TaxonomyExists;
                        categorydata.Append(string.Format(categorydatatemplate, TaxArr[i].TaxonomyId, IsChecked(Array.Exists(_FolderData.FolderTaxonomy, Taxpredicate)), TaxArr[i].TaxonomyName));
                    }
                    else
                    {
                        break;
                    }
                    i++;
                    categorydata.Append("<br/>");
                }
            }
        }

        StringBuilder str = new StringBuilder();
        str.Append(categorydata.ToString());
        LocaleTaxonomy.Visible = true;
       if (str.ToString() == string.Empty)
        {
            LocaleTaxonomy.Visible = false;
        }
       else
        {
            LocaleTaxonomyList.Text = str.ToString();
        }
    }

    private string IsChecked(bool value)
    {
        if (value)
        {
            return " checked=\"checked\"";
        }
        else
        {
            return " ";
        }
    }
    private bool TaxonomyExists(TaxonomyBaseData data)
    {
        if (data != null)
        {
            if (data.TaxonomyId == checktaxid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool IsInheritingXmlMultiConfig()
    {
        bool isInheriting = _ContentApi.IsInheritingXmlMultiConfig(_FolderData.Id);
        return isInheriting;
    }
    private bool IsInheritingTemplateMultiConfig()
    {
        bool isInheriting = _ContentApi.IsInheritingTemplateMultiConfig(_FolderData.Id);
        return isInheriting;
    }
    #endregion

}

