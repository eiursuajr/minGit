using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Linq;
using System.Data;
using System.Web.Caching;
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
using System.Xml;
using Ektron.Cms;
using Ektron.Cms.Site;
using Ektron.Cms.DataIO.LicenseManager;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Localization;
using Ektron.Cms.Framework.Localization;
using Microsoft.VisualBasic.CompilerServices;
using Ektron.Cms.Content;
using Microsoft.Security.Application;


public partial class viewcontent : System.Web.UI.UserControl
{
    #region Member Variables - Private

    private string _ApplicationPath;
    private SiteAPI _SiteApi;
    private EkContent m_refContent;
    private EkTask m_refTask;
    private EkTasks cTasks;
    private EkTaskType m_refTaskType;
    private string[] arrTaskTypeID;
    private int intCount;
    private Collection colAllCategory;

    //subscription - SK
    private SubscriptionData[] subscription_data_list;
    private SubscriptionData[] subscribed_data_list;
    private SubscriptionPropertiesData subscription_properties_list;
    private SubscriptionData[] active_subscription_list;
    //END: Subscription - SK

    //blog - SK
    private bool m_bIsBlog = false;
    private BlogPostData blog_post_data;
    private string[] arrBlogPostCategories;

    private string m_SelectedEditControl;
    private ApprovalData[] approvaldata = null;
    private bool IsLastApproval = false;
    private bool IsCurrentApproval = false;

    #endregion

    #region Member Variables - Protected

    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected long m_intId = 0;
    protected FolderData folder_data;
    protected PermissionData security_data;
    protected string AppImgPath = "";
    protected int ContentType = 1;
    protected long CurrentUserId = 0;
    protected Collection pagedata;
    protected string m_strPageAction = "";
    protected string m_strOrderBy = "";
    protected int ContentLanguage = -1;
    protected int EnableMultilingual = 0;
    protected string SitePath = "";
    protected ContentData content_data;
    protected ContentStateData content_state_data;
    protected long m_intFolderId = -1;
    protected string CallerPage = "";
    protected bool TaskExists = false;
    protected string LanguageName = "";
    protected LanguageData language_data;
    protected bool m_bIsMac;
    protected long xml_id = 0;
    protected string allowHtml = "";
    protected string TaxonomyList = "";
    protected string ContentPaneHeight = "100%";
    protected EntryData entry_edit_data = null;
    protected Currency m_refCurrency = null;
    protected MediaData m_refMedia = null;
    protected CatalogEntry m_refCatalog = null;
    protected List<ContentMetaData> meta_data = new List<ContentMetaData>();
    protected EkSite m_refSite = null;
    protected long m_iFolder = 0;
    protected string m_sEditAction = "";
    protected int lValidCounter = 0;
    protected long xid = 0;
    protected FolderData catalog_data = new FolderData();
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected int lContentType = 0;
    protected bool m_bHasXmlConfig = false;
    protected long m_xmlConfigID = 0;
    protected bool bTakeAction = false;
    protected string g_ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes.ToString();
    protected string ViewImage = "images/UI/Icons/folderView.png";
    protected AssetInfoData[] asset_data;
    protected string NextActionType = "";
    protected bool bInOrApproved = false;
    protected ProductTypeData prod_type_data = null;
    protected bool showAlert = true;
    protected bool _initIsFolderAdmin = false;
    protected bool _isFolderAdmin = false;
    protected bool _initIsCopyOrMoveAdmin = false;
    protected bool _isCopyOrMoveAdmin = false;

    #endregion

    #region Properties

    private string ApplicationPath
    {
        get
        {
            return _ApplicationPath;
        }
        set
        {
            _ApplicationPath = value;
        }
    }

    #endregion

    #region Events

    protected viewcontent()
    {
        _SiteApi = new SiteAPI();

        string[] slash = new string[] { "/" };
        this.ApplicationPath = _SiteApi.ApplicationPath.TrimEnd(slash.ToString().ToCharArray());

    }

    protected void Page_Init(object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterCSS();
        this.RegisterJS();

    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        m_SelectedEditControl = Utilities.GetEditorPreference(Request);
        if (Request.Browser.Platform.IndexOf("Win") == -1)
        {
            m_bIsMac = true;
        }
        else
        {
            m_bIsMac = false;
        }

        this.CreateChildControls();

        m_refMsg = m_refContentApi.EkMsgRef;
        ApprovalScript.Visible = false;

        if (ContentLanguage == m_refContentApi.DefaultContentLanguage && File.Exists(Server.MapPath(this.ApplicationPath + "controls/content/ViewLocalizationTab.ascx")))
        {
            //TODO: should not show this tab if this is not one of the target languages.
            UserControl ucLocalizationTab;
            ucLocalizationTab = (UserControl)(LoadControl("ViewLocalizationTab.ascx"));
            ucLocalizationTab.ID = "viewLocalizationTab";
            this.phLocalization2.Controls.Add(ucLocalizationTab);
            this.phLocalization.Visible = true;
            this.phLocalization2.Visible = true;
        }
        else
        {
            this.phLocalization.Visible = false;
            this.phLocalization2.Visible = false;
        }
    }

    #endregion

    #region Helpers

    private bool IsAnalyticsViewer()
    {
        Ektron.Cms.Analytics.IAnalytics dataManager = ObjectFactory.GetAnalytics();
        return dataManager.IsAnalyticsViewer();
    }

    private bool IsFolderAdmin()
    {
        if (_initIsFolderAdmin)
        {
            return _isFolderAdmin;
        }
        _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers), m_intFolderId, m_refContentApi.UserId, false);
        _initIsFolderAdmin = true;
        return _isFolderAdmin;
    }

    private bool IsCopyOrMoveAdmin()
    {
        if (_initIsCopyOrMoveAdmin)
        {
            return _isCopyOrMoveAdmin;
        }
        _isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder(Convert.ToInt64(EkEnumeration.CmsRoleIds.MoveOrCopy), m_intFolderId, m_refContentApi.UserId, false);
        _initIsCopyOrMoveAdmin = true;
        return _isCopyOrMoveAdmin;
    }

    public bool ViewContent()
    {
        if (!(Request.QueryString["id"] == null))
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"]);
            if (m_intId == 0)
            {
                if (!(Request.QueryString["contentid"] == null))
                {
                    m_intId = Convert.ToInt64(Request.QueryString["contentid"]);
                }
            }
        }

        if (!(Request.QueryString["action"] == null))
        {
            m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (!(Request.QueryString["orderby"] == null))
        {
            m_strOrderBy = Convert.ToString(Request.QueryString["orderby"]);
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
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContentApi.ContentLanguage = ContentLanguage;
        }
        if (!(Request.QueryString["cancelaction"] == null))
        {
            if (Convert.ToString(Request.QueryString["cancelaction"]).ToLower() == "undocheckout")
            {
                bool retval = false;
                m_refContent = m_refContentApi.EkContentRef;
                retval = m_refContent.UndoCheckOutv2_0(m_intId);
            }
        }
        language_data = (new SiteAPI()).GetLanguageById(ContentLanguage);
        LanguageName = language_data.Name;
        m_refContent = m_refContentApi.EkContentRef;
        TaskExists = m_refContent.DoesTaskExistForContent(m_intId);

        CurrentUserId = m_refContentApi.UserId;
        AppImgPath = m_refContentApi.AppImgPath;
        SitePath = m_refContentApi.SitePath;
        EnableMultilingual = m_refContentApi.EnableMultilingual;
        if (!(Request.QueryString["callerpage"] == null))
        {
            CallerPage = AntiXss.UrlEncode(Request.QueryString["callerpage"]);
        }

        if (CallerPage == "")
        {
            if (!(Request.QueryString["calledfrom"] == null))
            {
                CallerPage = AntiXss.UrlEncode(Request.QueryString["calledfrom"]);
            }
        }
        if (!(Request.QueryString["folder_id"] == null))
        {
            if (Request.QueryString["folder_id"] != "")
            {
                m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
            }
        }
        if (m_intFolderId == -1)
        {
            //let try again to get folder id
            if (!(Request.QueryString["fldid"] == null))
            {
                if (Request.QueryString["fldid"] != "")
                {
                    m_intFolderId = Convert.ToInt64(Request.QueryString["fldid"]);
                }
            }
        }
        Display_ViewContent();
        return true;
    }

    public string FixPath(string html, string assetFileName)
    {
        if (content_data.Status.ToUpper() != "A")
        {
            html = html.Replace(assetFileName, m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?history=true&ID=" + content_data.AssetData.Id + "&version=" + content_data.AssetData.Version);
        }
        return html;
    }

    private void Display_ViewContent()
    {
        m_refMsg = m_refContentApi.EkMsgRef;
        bool bCanAlias = false;
        PermissionData security_task_data;
        StringBuilder sSummaryText;
        Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliasname = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        Ektron.Cms.UrlAliasing.UrlAliasAutoApi m_autoaliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        Ektron.Cms.Common.UrlAliasManualData d_alias;
        System.Collections.Generic.List<Ektron.Cms.Common.UrlAliasAutoData> auto_aliaslist = new System.Collections.Generic.List<Ektron.Cms.Common.UrlAliasAutoData>();
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        int i;
        bool IsStagingServer;

        IsStagingServer = m_refContentApi.RequestInformationRef.IsStaging;

        security_task_data = m_refContentApi.LoadPermissions(m_intId, "tasks", ContentAPI.PermissionResultType.Task);
        security_data = m_refContentApi.LoadPermissions(m_intId, "content", ContentAPI.PermissionResultType.All);
        security_data.CanAddTask = security_task_data.CanAddTask;
        security_data.CanDestructTask = security_task_data.CanDestructTask;
        security_data.CanRedirectTask = security_task_data.CanRedirectTask;
        security_data.CanDeleteTask = security_task_data.CanDeleteTask;



        active_subscription_list = m_refContentApi.GetAllActiveSubscriptions();

        if ("viewstaged" == m_strPageAction)
        {
            ContentStateData objContentState;
            objContentState = m_refContentApi.GetContentState(m_intId);
            if ("A" == objContentState.Status)
            {
                // Can't view staged
                m_strPageAction = "view";
            }
        }
        try
        {
            if (m_strPageAction == "view")
            {
                content_data = m_refContentApi.GetContentById(m_intId, 0);
            }
            else if (m_strPageAction == "viewstaged")
            {
                content_data = m_refContentApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged);
            }
        }
        catch (Exception ex)
        {
            Response.Redirect("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message), true);
            return;
        }

        if ((content_data != null) && (Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64 (content_data.Type), Convert.ToBoolean (-1))))
        {
            ContentPaneHeight = "700px";
        }
        //ekrw = m_refContentApi.EkUrlRewriteRef()
        //ekrw.Load()
        if (((m_urlAliasSettings.IsManualAliasEnabled || m_urlAliasSettings.IsAutoAliasEnabled) && m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias)) && (content_data != null) && (content_data.AssetData != null) && !(Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_data.AssetData.FileExtension))))
        {
            bCanAlias = true;
        }

        blog_post_data = new BlogPostData();
        blog_post_data.Categories = (string[])Array.CreateInstance(typeof(string), 0);
        if (content_data.MetaData != null)
        {
            for (i = 0; i <= (content_data.MetaData.Length - 1); i++)
            {
                if ((string)(content_data.MetaData[i].TypeName.ToLower()) == "blog categories")
                {
                    content_data.MetaData[i].Text = content_data.MetaData[i].Text.Replace("&#39;", "\'");
                    content_data.MetaData[i].Text = content_data.MetaData[i].Text.Replace("&quot", "\"");
                    content_data.MetaData[i].Text = content_data.MetaData[i].Text.Replace("&gt;", ">");
                    content_data.MetaData[i].Text = content_data.MetaData[i].Text.Replace("&lt;", "<");
                    blog_post_data.Categories = Strings.Split((string)(content_data.MetaData[i].Text), ";", -1, 0);
                }
                else if ((string)(content_data.MetaData[i].TypeName.ToLower()) == "blog pingback")
                {
                    if (!(content_data.MetaData[i].Text.Trim().ToLower() == "no"))
                    {
                        m_bIsBlog = true;
                    }
                    blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo((string)(content_data.MetaData[i].Text));
                }
                else if ((string)(content_data.MetaData[i].TypeName.ToLower()) == "blog tags")
                {
                    blog_post_data.Tags = (string)(content_data.MetaData[i].Text);
                }
                else if ((string)(content_data.MetaData[i].TypeName.ToLower()) == "blog trackback")
                {
                    blog_post_data.TrackBackURL = (string)(content_data.MetaData[i].Text);
                }
            }
        }

        //THE FOLLOWING LINES ADDED DUE TO TASK
        //:BEGIN / PROPOSED BY PAT
        //TODO: Need to recheck this part of the code e.r.
        if (content_data == null)
        {
            if (ContentLanguage != 0)
            {
                if (ContentLanguage.ToString() != (string)(Ektron.Cms.CommonApi.GetEcmCookie()["DefaultLanguage"]))
                {
                    Response.Redirect((string)(Request.ServerVariables["URL"] + "?" + Strings.Replace(Request.ServerVariables["Query_String"], (string)("LangType=" + ContentLanguage), (string)("LangType=" + m_refContentApi.DefaultContentLanguage), 1, -1, 0)), false);
                    return;
                }
            }
            else
            {
                if (ContentLanguage.ToString() != (string)(Ektron.Cms.CommonApi.GetEcmCookie()["DefaultLanguage"]))
                {
                    Response.Redirect((string)(Request.ServerVariables["URL"] + "?" + Request.ServerVariables["Query_String"] + "&LangType=" + m_refContentApi.DefaultContentLanguage), false);
                    return;
                }
            }
        }
        //:END
        if (m_intFolderId == -1)
        {
            m_intFolderId = content_data.FolderId;
        }
        HoldMomentMsg.Text = m_refMsg.GetMessage("one moment msg");

        if ((active_subscription_list == null) || (active_subscription_list.Length == 0))
        {
            phWebAlerts.Visible = false;
            phWebAlerts2.Visible = false;
        }
        content_state_data = m_refContentApi.GetContentState(m_intId);

        jsFolderId.Text = m_intFolderId.ToString ();
        jsIsForm.Text = content_data.Type.ToString ();
        jsBackStr.Text = "back_file=content.aspx";
        if (m_strPageAction.Length > 0)
        {
            jsBackStr.Text += "&back_action=" + m_strPageAction;
        }
        if (Convert.ToString(m_intFolderId).Length > 0)
        {
            jsBackStr.Text += "&back_folder_id=" + m_intFolderId;
        }
        if (Convert.ToString(m_intId).Length > 0)
        {
            jsBackStr.Text += "&back_id=" + m_intId;
        }
        if (Convert.ToString((short)ContentLanguage).Length > 0)
        {
            jsBackStr.Text += "&back_LangType=" + ContentLanguage;
        }
        jsToolId.Text = m_intId.ToString ();
        jsToolAction.Text = m_strPageAction;
        jsLangId.Text = m_refContentApi.ContentLanguage.ToString ();
        if (content_data.Type == 3333)
        {
            ViewCatalogToolBar();
        }
        else
        {
            ViewToolBar();
        }

        if (bCanAlias && content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) //And folder_data.FolderType <> 1 Don't Show alias tab for Blogs.
        {
            string m_strAliasPageName = "";

            d_alias = m_aliasname.GetDefaultAlias(content_data.Id);
            if (d_alias.QueryString != "")
            {
                m_strAliasPageName = d_alias.AliasName + d_alias.FileExtension + d_alias.QueryString; //content_data.ManualAlias
            }
            else
            {
                m_strAliasPageName = d_alias.AliasName + d_alias.FileExtension; //content_data.ManualAlias
            }

            if (m_strAliasPageName != "")
            {

                if (IsStagingServer && folder_data.DomainStaging != string.Empty)
                {
                    m_strAliasPageName = (string)("http://" + folder_data.DomainStaging + "/" + m_strAliasPageName);
                }
                else if (folder_data.IsDomainFolder)
                {
                    m_strAliasPageName = (string)("http://" + folder_data.DomainProduction + "/" + m_strAliasPageName);
                }
                else
                {
                    m_strAliasPageName = SitePath + m_strAliasPageName;
                }
                m_strAliasPageName = "<a href=\"" + m_strAliasPageName + "\" target=\"_blank\" >" + m_strAliasPageName + "</a>";
            }
            else
            {
                m_strAliasPageName = " [Not Defined]";
            }
            tdAliasPageName.InnerHtml = m_strAliasPageName;
        }
        else
        {
            phAliases.Visible = false;
            phAliases2.Visible = false;
        }
        auto_aliaslist = m_autoaliasApi.GetListForContent(content_data.Id);
        autoAliasList.InnerHtml = "<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl automatic") + "</div>";
        autoAliasList.InnerHtml += "<div class=\"ektronBorder\">";
        autoAliasList.InnerHtml += "<table width=\"100%\">";
        autoAliasList.InnerHtml += "<tr class=\"title-header\">";
        autoAliasList.InnerHtml += "<th>" + m_refMsg.GetMessage("generic type") + "</th>";
        autoAliasList.InnerHtml += "<th>" + m_refMsg.GetMessage("lbl alias name") + "</th>";
        autoAliasList.InnerHtml += "</tr>";
        for (i = 0; i <= auto_aliaslist.Count() - 1; i++)
        {
            autoAliasList.InnerHtml += "<tr class=\"row\">";
            if (auto_aliaslist[i].AutoAliasType == Ektron.Cms.Common.EkEnumeration.AutoAliasType.Folder)
            {
                autoAliasList.InnerHtml += "<td><img src =\"" + m_refContentApi.AppPath + "images/UI/Icons/folder.png\"  alt=\"" + m_refContentApi.EkMsgRef.GetMessage("lbl folder") + "\" title=\"" + m_refContentApi.EkMsgRef.GetMessage("lbl folder") + "\"/ ></td>";
            }
            else
            {
                autoAliasList.InnerHtml += "<td><img src =\"" + m_refContentApi.AppPath + "images/UI/Icons/taxonomy.png\"  alt=\"" + m_refContentApi.EkMsgRef.GetMessage("generic taxonomy lbl") + "\" title=\"" + m_refContentApi.EkMsgRef.GetMessage("generic taxonomy lbl") + "\"/ ></td>";
            }

            if (IsStagingServer && folder_data.DomainStaging != string.Empty)
            {
                autoAliasList.InnerHtml = autoAliasList.InnerHtml + "<td> <a href = \"http://" + folder_data.DomainStaging + "/" + auto_aliaslist[i].AliasName + "\" target=\"_blank\" >" + auto_aliaslist[i].AliasName + " </a></td></tr>";
            }
            else if (folder_data.IsDomainFolder)
            {
                autoAliasList.InnerHtml += "<td> <a href = \"http://" + folder_data.DomainProduction + "/" + auto_aliaslist[i].AliasName + "\" target=\"_blank\" >" + auto_aliaslist[i].AliasName + " </a></td>";
            }
            else
            {
                autoAliasList.InnerHtml += "<td> <a href = \"" + SitePath + auto_aliaslist[i].AliasName + "\" target=\"_blank\" >" + auto_aliaslist[i].AliasName + " </a></td>";
            }
            autoAliasList.InnerHtml += "</tr>";
        }
        autoAliasList.InnerHtml += "</table>";
        autoAliasList.InnerHtml += "</div>";
        if (content_data == null)
        {
            content_data = m_refContentApi.GetContentById(m_intId, 0);
        }
        if (content_data.Type == 3333)
        {
            m_refCatalog = new CatalogEntry(m_refContentApi.RequestInformationRef);
            m_refCurrency = new Currency(m_refContentApi.RequestInformationRef);
            //m_refMedia = MediaData()
            entry_edit_data = m_refCatalog.GetItemEdit(m_intId, ContentLanguage, false);

            Ektron.Cms.Commerce.ProductType m_refProductType = new Ektron.Cms.Commerce.ProductType(m_refContentApi.RequestInformationRef);
            prod_type_data = m_refProductType.GetItem(entry_edit_data.ProductType.Id, true);

            if (prod_type_data.Attributes.Count == 0)
            {
                phAttributes.Visible = false;
                phAttributes2.Visible = false;
            }

            Display_PropertiesTab(content_data);
            Display_PricingTab();
            Display_ItemTab();
            Display_MetadataTab();
            Display_MediaTab();
        }
        else
        {
            ViewContentProperties(content_data);
            phCommerce.Visible = false;
            phCommerce2.Visible = false;
            phItems.Visible = false;
        }

        bool bPackageDisplayXSLT = false;
        string CurrentXslt = "";
        int XsltPntr;

        if ((!(content_data.XmlConfiguration == null)) && (content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry || content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_Content || content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_Forms))
        {
            if (!(content_data.XmlConfiguration == null))
            {
                if (content_data.XmlConfiguration.DefaultXslt.Length > 0)
                {
                    if (content_data.XmlConfiguration.DefaultXslt == "0")
                    {
                        bPackageDisplayXSLT = true;
                    }
                    else
                    {
                        bPackageDisplayXSLT = false;
                    }
                    if (!bPackageDisplayXSLT)
                    {
                        XsltPntr = int.Parse(content_data.XmlConfiguration.DefaultXslt);
                        if (XsltPntr > 0)
                        {
                            Collection tmpXsltColl = (Collection)content_data.XmlConfiguration.PhysPathComplete;
                            if (tmpXsltColl["Xslt" + XsltPntr] != null)
                            {
                                CurrentXslt = (string)(tmpXsltColl["Xslt" + XsltPntr]);
                            }
                            else
                            {
                                tmpXsltColl = (Collection)content_data.XmlConfiguration.LogicalPathComplete;
                                CurrentXslt = (string)(tmpXsltColl["Xslt" + XsltPntr]);
                            }
                        }
                    }
                }
                else
                {
                    bPackageDisplayXSLT = true;
                }
                //End If

                Ektron.Cms.Xslt.ArgumentList objXsltArgs = new Ektron.Cms.Xslt.ArgumentList();
                objXsltArgs.AddParam("mode", string.Empty, "preview");
                if (bPackageDisplayXSLT)
                {
                    divContentHtml.InnerHtml = m_refContentApi.XSLTransform(content_data.Html, content_data.XmlConfiguration.PackageDisplayXslt, false, false, objXsltArgs, true, true);
                }
                else
                {
                    // CurrentXslt is always obtained from the object in the database.
                    divContentHtml.InnerHtml = m_refContentApi.XSLTransform(content_data.Html, CurrentXslt, true, false, objXsltArgs, true, true);
                }
            }
            else
            {
                divContentHtml.InnerHtml = content_data.Html;
            }
        }
        else
        {
            if (content_data.Type == 104)
            {
                media_html.Value = content_data.MediaText;
                //Get Url from content
                string tPath = m_refContentApi.RequestInformationRef.AssetPath + m_refContentApi.EkContentRef.GetFolderParentFolderIdRecursive(content_data.FolderId).Replace(",", "/") + "/" + content_data.AssetData.Id + "." + content_data.AssetData.FileExtension;
                string mediaHTML = FixPath(content_data.Html, tPath);
                int scriptStartPtr = 0;
                int scriptEndPtr = 0;
                int len = 0;
                //Registering the javascript & CSS
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "linkReg", "<link href=\"" + m_refContentApi.ApplicationPath + "csslib/EktTabs.css\" rel=\"stylesheet\" type=\"text/css\" />", false);
                mediaHTML = mediaHTML.Replace("<link href=\"" + m_refContentApi.ApplicationPath + "csslib/EktTabs.css\" rel=\"stylesheet\" type=\"text/css\" />", "");
                while (1 == 1)
                {
                    scriptStartPtr = mediaHTML.IndexOf("<script", scriptStartPtr);
                    scriptEndPtr = mediaHTML.IndexOf("</script>", scriptEndPtr);
                    if (scriptStartPtr == -1 || scriptEndPtr == -1)
                    {
                        break;
                    }
                    len = scriptEndPtr - scriptStartPtr + 9;
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), (string)("scriptreg" + scriptEndPtr), mediaHTML.Substring(scriptStartPtr, len), false);
                    mediaHTML = mediaHTML.Replace(mediaHTML.Substring(scriptStartPtr, len), "");
                    scriptStartPtr = 0;
                    scriptEndPtr = 0;
                }
                media_display_html.Value = mediaHTML;
                divContentHtml.InnerHtml = "<a href=\"#\" onclick=\"document.getElementById(\'" + divContentHtml.ClientID + "\').innerHTML = document.getElementById(\'" + media_display_html.ClientID + "\').value;return false;\" alt=\"" + m_refMsg.GetMessage("alt show media content") + "\" title=\"" + m_refMsg.GetMessage("alt show media content") + "\">" + m_refMsg.GetMessage("lbl show media content") + "<br/><img align=\"middle\" src=\"" + m_refContentApi.AppPath + "images/filmstrip_ph.jpg\" /></a>";
            }
            else
            {
                if (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type, Convert .ToBoolean (-1)))
                {
                    string ver = "";
                    ver = (string)("&version=" + content_data.AssetData.Version);
                    if (IsImage(content_data.AssetData.Version))
                    {
                        divContentHtml.InnerHtml = "<img src=\"" + m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?ID=" + content_data.AssetData.Id + ver + "\" />";
                    }
                    else
                    {
                        divContentHtml.InnerHtml = "<div align=\"center\" style=\"padding:15px;\"><a style=\"text-decoration:none;\" href=\"#\" onclick=\"javascript:window.open(\'" + m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?ID=" + content_data.AssetData.Id + ver + "\',\'DownloadAsset\',\'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=1000,height=800\');return false;\"><img align=\"middle\" src=\"" + m_refContentApi.AppPath + "images/application/download.gif\" />" + m_refMsg.GetMessage("btn download") + " &quot;" + content_data.Title + "&quot;</a></div>";
                    }

                }
                else if (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                {
                    Ektron.Cms.API.UrlAliasing.UrlAliasCommon u = new Ektron.Cms.API.UrlAliasing.UrlAliasCommon();
                    FolderData fd = this.m_refContentApi.GetFolderById(content_data.FolderId);
                    string stralias = u.GetAliasForContent(content_data.Id);
                    if (stralias == string.Empty || fd.IsDomainFolder)
                    {
                        stralias = content_data.Quicklink;
                    }

                    string link = "";
                    if (content_data.ContType == (int)EkEnumeration.CMSContentType.Content || (content_data.ContType == (int)EkEnumeration.CMSContentType.Archive_Content && content_data.EndDateAction != 1))
                    {
                        string url = this.m_refContent.RequestInformation.SitePath + stralias;
                        if (url.Contains("?"))
                        {
                            url += "&";
                        }
                        else
                        {
                            url += "?";
                        }
                        if ("viewstaged" == m_strPageAction)
                        {
                            url += "view=staged";
                        }
                        else
                        {
                            url += "view=published";
                        }
                        url += (string)("&LangType=" + content_data.LanguageId.ToString());
                        link = "<a href=\"" + url + "\" onclick=\"window.open(this.href);return false;\">Click here to view the page</a><br/><br/>";
                    }
                    divContentHtml.InnerHtml = link + Ektron.Cms.PageBuilder.PageData.RendertoString(content_data.Html);
                }
                else
                {
                    if ((int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms == content_data.Type || (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms == content_data.Type)
                    {
                        divContentHtml.InnerHtml = content_data.Html.Replace("[srcpath]", m_refContentApi.ApplicationPath + m_refContentApi.AppeWebPath);
                        divContentHtml.InnerHtml = divContentHtml.InnerHtml.Replace("[skinpath]", m_refContentApi.ApplicationPath + "csslib/ContentDesigner/");
                    }
                    else
                    {
                        divContentHtml.InnerHtml = content_data.Html;
                    }
                    if (m_bIsBlog)
                    {
                        Collection blogData = m_refContentApi.EkContentRef.GetBlogData(content_data.FolderId);
                        if (blogData != null)
                        {
                            if (blogData["enablecomments"].ToString() != string.Empty)
                            {
                                litBlogComment.Text = "<div class=\"ektronTopSpace\"></div><a class=\"button buttonInline greenHover buttonNoIcon\" href=\"" + m_refContentApi.AppPath + "content.aspx?id=" + content_data.FolderId + "&action=ViewContentByCategory&LangType=" + content_data.LanguageId + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + content_data.Id + "&viewin=" + content_data.LanguageId + "\" title=\"" + m_refMsg.GetMessage("alt view comments label") + "\">" + m_refMsg.GetMessage("view comments") + "</a>";
                                litBlogComment.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        sSummaryText = new StringBuilder();
        if ((int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms == content_data.Type || (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms == content_data.Type)
        {
            if (content_data.Teaser != null)
            {
                if (content_data.Teaser.IndexOf("<ektdesignpackage_design") > -1)
                {
                    string strDesign;
                    strDesign = m_refContentApi.XSLTransform(null, null, true, false, null, true);
                    tdsummarytext.InnerHtml = strDesign;
                }
                else
                {
                    tdsummarytext.InnerHtml = content_data.Teaser;
                }
            }
            else
            {
                tdsummarytext.InnerHtml = "";
            }
        }
        else
        {
            if (m_bIsBlog)
            {
                sSummaryText.AppendLine("<table class=\"ektronGrid\">");
                sSummaryText.AppendLine("	<tr>");
                sSummaryText.AppendLine("		<td valign=\"top\" class=\"label\">");
                sSummaryText.AppendLine("			" + m_refMsg.GetMessage("generic description") + "");
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("		<td valign=\"top\">");
            }
            sSummaryText.AppendLine(content_data.Teaser);
            if (m_bIsBlog)
            {
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("	</tr>");
                sSummaryText.AppendLine("	<tr>");
                sSummaryText.AppendLine("		<td valign=\"top\" class=\"label\">");
                sSummaryText.AppendLine("			" + m_refMsg.GetMessage("lbl blog cat") + "");
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("		<td>");
                if (!(blog_post_data.Categories == null))
                {
                    arrBlogPostCategories = blog_post_data.Categories;
                    if (arrBlogPostCategories.Length > 0)
                    {
                        Array.Sort(arrBlogPostCategories);
                    }
                }
                else
                {
                    arrBlogPostCategories = null;
                }
                if (blog_post_data.Categories.Length > 0)
                {
                    for (i = 0; i <= (blog_post_data.Categories.Length - 1); i++)
                    {
                        if (blog_post_data.Categories[i].ToString() != "")
                        {
                            sSummaryText.AppendLine("				<input type=\"checkbox\" name=\"blogcategories" + i.ToString() + "\" value=\"" + blog_post_data.Categories[i].ToString() + "\" checked=\"true\" disabled>&nbsp;" + Strings.Replace((string)(blog_post_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "<br />");
                        }
                    }
                }
                else
                {
                    sSummaryText.AppendLine("No categories defined.");
                }
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("	</tr>");
                sSummaryText.AppendLine("	<tr>");
                sSummaryText.AppendLine("		<td class=\"label\" valign=\"top\">");
                sSummaryText.AppendLine("			" + m_refMsg.GetMessage("lbl personal tags") + "");
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("		<td>");
                if (!(blog_post_data == null))
                {
                    sSummaryText.AppendLine(blog_post_data.Tags);
                }
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("	</tr>");
                sSummaryText.AppendLine("	<tr>");
                sSummaryText.AppendLine("	    <td class=\"label\">");
                if (!(blog_post_data == null))
                {
                    sSummaryText.AppendLine("   <input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"" + blog_post_data.TrackBackURLID.ToString() + "\" />");
                    sSummaryText.AppendLine("   <input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/>" + m_refMsg.GetMessage("lbl trackback url") + "");
                    sSummaryText.AppendLine("		</td>");
                    sSummaryText.AppendLine("		<td>");
                    sSummaryText.AppendLine("<input type=\"text\" size=\"75\" id=\"trackback\" name=\"trackback\" value=\"" + EkFunctions.HtmlEncode(blog_post_data.TrackBackURL) + "\" disabled/>");
                    sSummaryText.AppendLine("		</td>");
                    sSummaryText.AppendLine("	</tr>");
                    sSummaryText.AppendLine("	<tr>");
                    sSummaryText.AppendLine("		<td class=\"label\">");
                    if (blog_post_data.Pingback == true)
                    {
                        sSummaryText.AppendLine("" + m_refMsg.GetMessage("lbl blog ae ping") + "");
                        sSummaryText.AppendLine("		</td>");
                        sSummaryText.AppendLine("		<td>");
                        sSummaryText.AppendLine("           <input type=\"checkbox\" name=\"pingback\" id=\"pingback\" checked disabled/>");

                    }
                    else
                    {
                        sSummaryText.AppendLine("" + m_refMsg.GetMessage("lbl blog ae ping") + "");
                        sSummaryText.AppendLine("		</td>");
                        sSummaryText.AppendLine("		<td>");
                        sSummaryText.AppendLine("           <input type=\"checkbox\" name=\"pingback\" id=\"pingback\" disabled/>");
                    }
                }
                else
                {
                    sSummaryText.AppendLine("           <input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"\" />");
                    sSummaryText.AppendLine("           <input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/>" + m_refMsg.GetMessage("lbl trackback url") + "");
                    sSummaryText.AppendLine("<input type=\"text\" size=\"75\" id=\"trackback\" name=\"trackback\" value=\"\" disabled/>");
                    sSummaryText.AppendLine("		</td>");
                    sSummaryText.AppendLine("	</tr>");
                    sSummaryText.AppendLine("	<tr>");
                    sSummaryText.AppendLine("		<td class=\"label\">" + m_refMsg.GetMessage("lbl blog ae ping") + "");
                    sSummaryText.AppendLine("		</td>");
                    sSummaryText.AppendLine("		<td>");
                    sSummaryText.AppendLine("           <input type=\"checkbox\" name=\"pingback\" id=\"pingback\" disabled/>");
                }
                sSummaryText.AppendLine("		</td>");
                sSummaryText.AppendLine("	</tr>");
                sSummaryText.AppendLine("</table>");
            }
            tdsummarytext.InnerHtml = sSummaryText.ToString();
        }


        ViewMetaData(content_data);

        tdcommenttext.InnerHtml = content_data.Comment;
        AddTaskTypeDropDown();
        ViewTasks();
        ViewSubscriptions();
        Ektron.Cms.Content.EkContent cref;
        cref = m_refContentApi.EkContentRef;
        TaxonomyBaseData[] dat;
        dat = cref.GetAllFolderTaxonomy(folder_data.Id);
        if (dat == null || dat.Length == 0)
        {
            phCategories.Visible = false;
            phCategories2.Visible = false;
        }
        ViewAssignedTaxonomy();
        if ((content_data != null) && ((content_data.Type >= EkConstants.ManagedAsset_Min && content_data.Type <= EkConstants.ManagedAsset_Max && content_data.Type != 104) || (content_data.Type >= EkConstants.Archive_ManagedAsset_Min && content_data.Type <= EkConstants.Archive_ManagedAsset_Max && content_data.Type != 1104) || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData))
        {
            showAlert = false;
        }
        if (
            (Request.QueryString["menuItemType"] != null && Request.QueryString["menuItemType"].ToLower() == "viewproperties")
            ||
            (Request.QueryString["tab"] != null && Request.QueryString["tab"].ToLower() == "properties")
            )
        {
            DefaultTab.Value = "dvProperties";
            Util_ReloadTree(content_data.Path, content_data.FolderId);
        }
    }
    public string GetCommerceIncludes()
    {
        string strReturn = "";
        //Display these following commerce related css and javascript files only if the content is of 3333 (which is under catalog folder) type.
        if ((content_data != null) && content_data.Type == 3333)
        {
            strReturn += "<script id=\"EktronCommercePricingJs\" type=\"text/javascript\" src=\"" + m_refContentApi.AppPath + "java/commerce/com.ektron.commerce.pricing.js\"></script>";
            strReturn += "<link id=\"EktronPricingCss\" type=\"text/css\" rel=\"stylesheet\" href=\"" + m_refContentApi.AppPath + "csslib/commerce/Ektron.Commerce.Pricing.css\" />";
        }
        return strReturn;
    }
    private void ViewAssignedTaxonomy()
    {
        Ektron.Cms.Content.EkContent cref;
        cref = m_refContentApi.EkContentRef;
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        List<string> result = new List<string>();
        taxonomy_cat_arr = cref.ReadAllAssignedCategory(m_intId, Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content);
        if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
        {
            result.Add("<ul class=\"assignedTaxonomyList\">");
            foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
            {
                result.Add(("<li>" + taxonomy_cat.TaxonomyPath.Remove(0, 1).Replace("\\", " > ") + "</li>"));
            }
            result.Add("</ul>");
            TaxonomyList = string.Join(string.Empty, result.ToArray());
            phCategories.Visible = true;
            phCategories2.Visible = true;
        }
        else
        {
            TaxonomyList = m_refMsg.GetMessage("lbl nocatselected");
        }
    }

    private void ViewTasks()
    {
        string actiontype = "both";
        string callBackPage = ""; //unknown
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("generic Title");
        TaskDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = m_refMsg.GetMessage("generic ID");
        TaskDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATE";
        colBound.HeaderText = m_refMsg.GetMessage("lbl state");
        TaskDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PRIORITY";
        colBound.HeaderText = m_refMsg.GetMessage("lbl priority");
        TaskDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DUEDATE";
        colBound.HeaderText = m_refMsg.GetMessage("lbl Due Date");
        TaskDataGrid.Columns.Add(colBound);

        if ((actiontype == "by") || (actiontype == "all") || (actiontype == "both"))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ASSIGNEDTO";
            colBound.HeaderText = m_refMsg.GetMessage("lbl Assigned to");
            TaskDataGrid.Columns.Add(colBound);
        }
        if ((actiontype == "to") || (actiontype == "all") || (actiontype == "both"))
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ASSIGNEDBY";
            colBound.HeaderText = m_refMsg.GetMessage("lbl Assigned By");
            TaskDataGrid.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COMMENT";
        colBound.HeaderText = m_refMsg.GetMessage("lbl Last Added comments");
        TaskDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATECREATED";
        colBound.HeaderText = m_refMsg.GetMessage("lbl Create Date");
        TaskDataGrid.Columns.Add(colBound);

        TaskDataGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("STATE", typeof(string)));
        dt.Columns.Add(new DataColumn("PRIORITY", typeof(string)));
        dt.Columns.Add(new DataColumn("DUEDATE", typeof(string)));
        dt.Columns.Add(new DataColumn("ASSIGNEDTO", typeof(string)));
        dt.Columns.Add(new DataColumn("ASSIGNEDBY", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMENT", typeof(string)));
        dt.Columns.Add(new DataColumn("DATECREATED", typeof(string)));

        if (TaskExists == true)
        {
            int TaskItemType = 1;
            m_refTask = m_refContentApi.EkTaskRef;

            Ektron.Cms.PageRequestData null_EktronCmsPageRequestData = null;
            cTasks = m_refTask.GetTasks(m_intId, -1, -1, TaskItemType, Request.QueryString["orderby"], ContentLanguage, ref null_EktronCmsPageRequestData, "");
        }

        int i = 0;
        EkTask cTask;

        if (cTasks != null)
        {
            EmailHelper m_refMail = new EmailHelper();
            while (i < cTasks.Count)
            {
                i++;
                cTask = cTasks.get_Item(i);
                if (!(cTask.TaskTypeID == (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment))
                {
                    Array.Resize(ref arrTaskTypeID, i - 1 + 1);
                    arrTaskTypeID[i - 1] = (string)("shown_task_" + i + "_" + (cTask.TaskTypeID <= 0 ? "NotS" : (cTask.TaskTypeID.ToString())));

                    dr = dt.NewRow();

                    dr["TITLE"] = "<a href=\"tasks.aspx?action=ViewTask&tid=" + cTask.TaskID + "&folderid=" + cTask.FolderId  +  "&contentid=" + cTask.ContentID +  "&fromViewContent=1&ty=both&LangType=" + cTask.ContentLanguage + callBackPage + "\">" + cTask.TaskTitle + "</a>";
                    dr["TITLE"] += "	<script language=\"JavaScript\">" + "\r\n";
                    dr["TITLE"] += "					AddShownTaskID(\'" + arrTaskTypeID[i - 1] + "\');" + "\r\n";
                    dr["TITLE"] += "				</script>	" + "\r\n";

                    dr["ID"] = cTask.TaskID;
                    int iState = System.Convert.ToInt32(cTask.State);
                    switch (iState)
                    {
                        case 1:
                            dr["STATE"] = "Not Started";
                            break;
                        case 2:
                            dr["STATE"] = "Active";
                            break;
                        case 3:
                            dr["STATE"] = "Awaiting Data";
                            break;
                        case 4:
                            dr["STATE"] = "On Hold";
                            break;
                        case 5:
                            dr["STATE"] = "Pending";
                            break;
                        case 6:
                            dr["STATE"] = "ReOpened";
                            break;
                        case 7:
                            dr["STATE"] = "Completed";
                            break;
                        case 8:
                            dr["STATE"] = "Archived";
                            break;
                        case 9:
                            dr["STATE"] = "Deleted";
                            break;
                    }
                    int iPrio = System.Convert.ToInt32(cTask.Priority);
                    switch (iPrio)
                    {
                        case 1:
                            dr["PRIORITY"] = "Low";
                            break;
                        case 2:
                            dr["PRIORITY"] = "Normal";
                            break;
                        case 3:
                            dr["PRIORITY"] = "High";
                            break;
                    }

                    if (cTask.DueDate != "")
                    {
                        if (System.Convert.ToDateTime(cTask.DueDate) < DateTime.Today) //Verify:Udai 11/22/04 Replaced Now.ToOADate - 1 with DateTime.Today
                        {
                            dr["DUEDATE"] = cTask.DueDate; //Response.Write("<td class=""important"">" & AppUI.GetInternationalDateOnly(cTask.DueDate) & "</td>")
                        }
                        else
                        {
                            dr["DUEDATE"] = cTask.DueDate; //Response.Write("<td>" & AppUI.GetInternationalDateOnly(cTask.DueDate) & "</td>")
                        }
                    }
                    else
                    {
                        dr["DUEDATE"] = "[Not Specified]";
                    }
                    EkTask tempcTask = cTask;
                    if ((actiontype == "by") || (actiontype == "all") || (actiontype == "both"))
                    {
                        if (cTask.AssignToUserGroupID == 0)
                        {
                            dr["ASSIGNEDTO"] = "All Authors of (" + cTask.ContentID.ToString() + ")";
                        }
                        else if (cTask.AssignedToUser != "")
                        {
                            dr["ASSIGNEDTO"] = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" align=\"absbottom\">" + m_refMail.MakeUserTaskEmailLink(tempcTask, false);
                        }
                        else if (cTask.AssignedToUserGroup != "")
                        {
                            dr["ASSIGNEDTO"] = "<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" align=\"absbottom\">";
                            if (cTask.AssignToUserGroupID != -1)
                            {
                                dr[5] += m_refMail.MakeUserGroupTaskEmailLink(tempcTask);
                            }
                            else
                            {
                                dr[5] += cTask.AssignedToUserGroup;
                            }
                        }
                    }
                    if ((actiontype == "to") || (actiontype == "all") || (actiontype == "both"))
                    {
                        dr["ASSIGNEDBY"] = m_refMail.MakeByUserTaskEmailLink(tempcTask, false);

                    }

                    if (cTask.LastComment == "")
                    {
                        dr["COMMENT"] = "[Not Specified]";
                    }
                    else
                    {
                        dr["COMMENT"] = "<div class=\"comment-block\">" + cTask.LastComment + "</div>";
                    }
                    dr["DATECREATED"] = cTask.DateCreated; //GetInternationalDateOnly

                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        TaskDataGrid.DataSource = dv;
        TaskDataGrid.DataBind();

    }

    //Task Type
    private void AddTaskTypeDropDown()
    {
        m_refTaskType = m_refContentApi.EkTaskTypeRef;
        colAllCategory = m_refTaskType.SelectAllCategory();
        TaskTypeJS.Visible = true;
        TaskTypeJS.Text = m_refTaskType.GetTaskTypeJS(colAllCategory, m_refMsg);
    }
    //End: Task Type

    private void ViewContentProperties(ContentData data)
    {
        //GET PROPERTY: status
        string dataStatus = "";
        switch (data.Status.ToLower())
        {
            case "a":
                dataStatus = m_refMsg.GetMessage("status:Approved (Published)");
                break;
            case "o":
                dataStatus = m_refMsg.GetMessage("status:Checked Out");
                break;
            case "i":
                dataStatus = m_refMsg.GetMessage("status:Checked In");
                break;
            case "p":
                dataStatus = m_refMsg.GetMessage("status:Approved (PGLD)");
                break;
            case "m":
                dataStatus = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
                break;
            case "s":
                dataStatus = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
                break;
            case "t":
                dataStatus = m_refMsg.GetMessage("status:Waiting Approval");
                break;
            case "d":
                dataStatus = "Deleted (Pending Start Date)";
                break;
        }

        //GET PROPERTY: start date
        string goLive;
        if (data.DisplayGoLive.Length == 0)
        {
            goLive = m_refMsg.GetMessage("none specified msg");
        }
        else
        {
            goLive = Ektron.Cms.Common.EkFunctions.FormatDisplayDate(data.DisplayGoLive, data.LanguageId);
        }

        //GET PROPERTY: end date
        string endDate;
        if (data.DisplayEndDate == "")
        {
            endDate = m_refMsg.GetMessage("none specified msg");
        }
        else
        {
            endDate = Ektron.Cms.Common.EkFunctions.FormatDisplayDate(data.DisplayEndDate, data.LanguageId);
        }

        //GET PROPERTY: action on end date
        string endDateActionTitle;
        if (data.DisplayEndDate.Length > 0)
        {
            if (data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
            {
                endDateActionTitle = m_refMsg.GetMessage("Archive display descrp");
            }
            else if (data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
            {
                endDateActionTitle = m_refMsg.GetMessage("Refresh descrp");
            }
            else
            {
                endDateActionTitle = m_refMsg.GetMessage("Archive expire descrp");
            }
        }
        else
        {
            endDateActionTitle = m_refMsg.GetMessage("none specified msg");
        }

        //GET PROPERTY: approval method
        string apporvalMethod;
        if (data.ApprovalMethod == 1)
        {
            apporvalMethod = m_refMsg.GetMessage("display for force all approvers");
        }
        else
        {
            apporvalMethod = m_refMsg.GetMessage("display for do not force all approvers");
        }

        //GET PROPERTY: approvals
        System.Text.StringBuilder approvallist = new System.Text.StringBuilder();
        int i;
        if (approvaldata == null)
        {
            approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId);
        }
        approvallist.Append(m_refMsg.GetMessage("none specified msg"));
        if (!(approvaldata == null))
        {
            if (approvaldata.Length > 0)
            {
                approvallist.Length = 0;
                for (i = 0; i <= approvaldata.Length - 1; i++)
                {
                    if (approvaldata[i].Type.ToLower() == "user")
                    {
                        approvallist.Append("<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" alt=\"" + m_refMsg.GetMessage("approver is user") + "\" title=\"" + m_refMsg.GetMessage("approver is user") + "\">");
                    }
                    else
                    {
                        approvallist.Append("<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" alt=\"" + m_refMsg.GetMessage("approver is user group") + "\" title=\"" + m_refMsg.GetMessage("approver is user group") + "\">");
                    }

                    approvallist.Append("<span");
                    if (approvaldata[i].IsCurrentApprover)
                    {
                        approvallist.Append(" class=\"important\"");
                    }
                    approvallist.Append(">");

                    if (approvaldata[i].Type.ToLower() == "user")
                    {
                        approvallist.Append(approvaldata[i].DisplayUserName);
                    }
                    else
                    {
                        approvallist.Append(approvaldata[i].DisplayUserName);
                    }

                    approvallist.Append("</span>");
                }
            }
        }

        //GET PROPERTY: smart form configuration
        string type;
        if (data.Type == 3333)
        {
            type = m_refMsg.GetMessage("lbl product type xml config");
        }
        else
        {
            type = m_refMsg.GetMessage("xml configuration label");
        }

        //GET PROPERTY: smart form title
        string typeValue;
        if (!(data.XmlConfiguration == null))
        {
            typeValue = (string)("&nbsp;" + data.XmlConfiguration.Title);
            xml_id = data.XmlConfiguration.Id;
        }
        else
        {
            typeValue = (string)(m_refMsg.GetMessage("none specified msg") + " " + m_refMsg.GetMessage("html content assumed"));
        }

        if (folder_data == null)
        {
            folder_data = m_refContentApi.EkContentRef.GetFolderById(content_data.FolderId);
        }

        //GET PROPERTY: template name
        string fileName;
        if (m_refContent.MultiConfigExists(content_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage))
        {
            TemplateData t_templateData = m_refContent.GetMultiTemplateASPX(content_data.Id);
            if (t_templateData != null)
            {
                fileName = t_templateData.FileName;
            }
            else
            {
                fileName = folder_data.TemplateFileName;
            }
        }
        else
        {
            fileName = folder_data.TemplateFileName;
        }

        //GET PROPERTY: rating
        string rating;
        Collection dataCol = m_refContentApi.GetContentRatingStatistics(data.Id, 0, null);
        int total = 0;
        int sum = 0;
        int hits = 0;
        if (dataCol.Count > 0)
        {
            total = Convert.ToInt32 (dataCol["total"]);
            sum = Convert.ToInt32 (dataCol["sum"]);
            hits = Convert.ToInt32 (dataCol["hits"]);
        }
        if (total == 0)
        {
            rating = m_refMsg.GetMessage("content not rated");
        }
        else
        {
            rating = System.Convert.ToString(Math.Round(System.Convert.ToDouble(Convert.ToDouble((short)sum) / total), 2));
        }

        NameValueCollection contentPropertyValues = new NameValueCollection();
        contentPropertyValues.Add(m_refMsg.GetMessage("content title label"), data.Title);
        contentPropertyValues.Add(m_refMsg.GetMessage("content id label"), data.Id.ToString ());
        contentPropertyValues.Add(m_refMsg.GetMessage("content language label"), LanguageName);
        contentPropertyValues.Add(m_refMsg.GetMessage("content status label"), dataStatus);
        contentPropertyValues.Add(m_refMsg.GetMessage("content LUE label"), data.EditorFirstName + " " + data.EditorLastName);
        contentPropertyValues.Add(m_refMsg.GetMessage("content LED label"), Ektron.Cms.Common.EkFunctions.FormatDisplayDate(data.DisplayLastEditDate, data.LanguageId));
        contentPropertyValues.Add(m_refMsg.GetMessage("generic start date label"),(goLive == Ektron.Cms.Common.EkFunctions.FormatDisplayDate(DateTime.MinValue.ToString(), data.LanguageId) ? m_refMsg.GetMessage("none specified msg") : goLive ));
        contentPropertyValues.Add(m_refMsg.GetMessage("generic end date label"), (endDate == Ektron.Cms.Common.EkFunctions.FormatDisplayDate(DateTime.MinValue.ToString(),  data.LanguageId) ? m_refMsg.GetMessage("none specified msg") : endDate));
        contentPropertyValues.Add(m_refMsg.GetMessage("End Date Action Title"), endDateActionTitle);
        contentPropertyValues.Add(m_refMsg.GetMessage("content DC label"), Ektron.Cms.Common.EkFunctions.FormatDisplayDate(data.DateCreated.ToString(), data.LanguageId));
        contentPropertyValues.Add(m_refMsg.GetMessage("lbl approval method"), apporvalMethod);
        contentPropertyValues.Add(m_refMsg.GetMessage("content approvals label"), approvallist.ToString());
        if (content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry || content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_Content || content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_Forms)
        {
            contentPropertyValues.Add(type, typeValue);
        }
        if (content_data.Type == Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry || content_data.Type == 1 || content_data.Type == 2 || content_data.Type == 104)
        {
            contentPropertyValues.Add(m_refMsg.GetMessage("template label"), fileName);
        }
        contentPropertyValues.Add(m_refMsg.GetMessage("generic Path"), data.Path);
        contentPropertyValues.Add(m_refMsg.GetMessage("rating label"), rating);
        contentPropertyValues.Add(m_refMsg.GetMessage("lbl content searchable"), data.IsSearchable.ToString());

        //string[] endColon = new string[] { ":" };
        string endColon = ":";
        string propertyName;
        StringBuilder propertyRows = new StringBuilder();
        for (i = 0; i <= contentPropertyValues.Count - 1; i++)
        {
            propertyName = (string)(contentPropertyValues.GetKey(i).TrimEnd(endColon.ToString().ToCharArray()));
            propertyRows.Append("<tr><td class=\"label\">");
            propertyRows.Append(propertyName + ":");
            propertyRows.Append("</td><td>");
            propertyRows.Append(contentPropertyValues[contentPropertyValues.GetKey(i)]);
            propertyRows.Append("</td></tr>");
        }

        litPropertyRows.Text = propertyRows.ToString();
    }

    private void ViewMetaData(ContentData data)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string strResult = "";
        string strImagePath = "";
        FolderData fldr_Data = new FolderData();
        ContentAPI contentapi = new ContentAPI();

        fldr_Data = contentapi.GetFolderById(data.FolderId);
        if (data != null)
        {
            List<ContentMetaData> displayMetaDataList = new List<ContentMetaData>();
            for (int i = 0; i < data.MetaData.Length; i++)
            {
                string typeName = data.MetaData[i].TypeName;
                 if (!typeName.StartsWith("L10n", StringComparison.OrdinalIgnoreCase) && !typeName.StartsWith("Xliff", StringComparison.OrdinalIgnoreCase) && data.MetaData[i].Type.ToString() != "ImageSelector")
                {
                    displayMetaDataList.Add(data.MetaData[i]);
                }
                else if (data.MetaData[i].Type.ToString() == "ImageSelector")
                {
                    data.MetaData[i].Text = data.MetaData[i].Text.Replace(SitePath + "assets/", "");
                    data.MetaData[i].Text = System.Text.RegularExpressions.Regex.Replace(data.MetaData[i].Text, "\\?.*", "");
                    displayMetaDataList.Add(data.MetaData[i]);
                }
          }

            if (displayMetaDataList.Count > 0)
            {
                strResult = Ektron.Cms.CustomFields.WriteFilteredMetadataForView(displayMetaDataList.ToArray(), m_intFolderId, false).Trim();
            }

            strImagePath = data.Image;
            if (strImagePath.IndexOf(this.AppImgPath + "spacer.gif") != -1)
            {
                strImagePath = "";
            }


            //if ((fldr_Data.IsDomainFolder == true || fldr_Data.DomainProduction != "") && SitePath != "/")
            //{
            //    if (strImagePath.IndexOf("http://") != -1)
            //    {
            //        strImagePath = strImagePath.Substring(strImagePath.IndexOf("http://"));
            //        data.ImageThumbnail = data.ImageThumbnail.Substring(data.ImageThumbnail.IndexOf("http://"));
            //    }
            //    else
            //    {
            //        if (strImagePath != "")
            //        {
            //            strImagePath = strImagePath.Replace(SitePath, "");
            //            data.ImageThumbnail = data.ImageThumbnail.Replace(SitePath, "");
            //            strImagePath = (string)("http://" + fldr_Data.DomainProduction + "/" + strImagePath);
            //            data.ImageThumbnail = (string)("http://" + fldr_Data.DomainProduction + "/" + data.ImageThumbnail);
            //        }
            //    }
            //}
            //else if ((fldr_Data.IsDomainFolder == true || fldr_Data.DomainProduction != "") && SitePath == "/")
            //{

            //    if (strImagePath.IndexOf("http://") != -1)
            //    {
            //        strImagePath = strImagePath.Substring(strImagePath.IndexOf("http://"));
            //        data.ImageThumbnail = data.ImageThumbnail.Substring(data.ImageThumbnail.IndexOf("http://"));
            //    }
            //    else
            //    {
            //        if (strImagePath != "")
            //        {
            //            strImagePath = (string)("http://" + fldr_Data.DomainProduction + "/" + strImagePath.Substring(1));
            //            data.ImageThumbnail = (string)("http://" + fldr_Data.DomainProduction + "/" + data.ImageThumbnail.Substring(1));
            //        }
            //    }
            //}
            //else if (fldr_Data.IsDomainFolder == false && strImagePath.IndexOf("http://") != -1)
            //{
            //    if (strImagePath.IndexOf(SitePath) == 0)
            //    {
            //        strImagePath = Strings.Replace(strImagePath, SitePath, "", 1, 1, 0);
            //        data.ImageThumbnail = Strings.Replace(data.ImageThumbnail, SitePath, "", 1, 1, 0);
            //    }
            //}
            //strImagePath = strImagePath;//Strings.Replace(strImagePath, SitePath, "", 1, 1, 0);
            data.ImageThumbnail = data.ImageThumbnail;// Strings.Replace(data.ImageThumbnail, SitePath, "", 1, 1, 0);
            if (fldr_Data.FolderType != 9)
            {
                // display tag info for this library item
                System.Text.StringBuilder taghtml = new System.Text.StringBuilder();
                taghtml.Append("<fieldset style=\"margin:10px\">");
                taghtml.Append("<legend>" + m_refMsg.GetMessage("lbl personal tags") + "</legend>");
                taghtml.Append("<div style=\"height: 80px; overflow: auto;\" >");
                if (content_data.Id > 0)
                {
                    LocalizationAPI localizationApi = new LocalizationAPI();
                    TagData[] tdaUser;
                    tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForObject(content_data.Id, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content, m_refContentApi.ContentLanguage);

                    if (tdaUser != null && tdaUser.Length > 0)
                    {

                        foreach (TagData td in tdaUser)
                        {
                            taghtml.Append("<input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                            taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                            taghtml.Append("&#160;" + td.Text + "<br />");
                        }
                    }
                    else
                    {
                        taghtml.Append(m_refMsg.GetMessage("lbl notagsselected"));
                    }
                }
                taghtml.Append("</div>");
                taghtml.Append("</fieldset>");
                strResult = strResult + taghtml.ToString();
                if (System.IO.Path.GetExtension(data.ImageThumbnail).ToLower().IndexOf(".gif") != -1 && data.ImageThumbnail.ToLower().IndexOf("spacer.gif") == -1)
                {

                    data.ImageThumbnail = data.ImageThumbnail.Replace(".gif", ".png");
                }
               
                strResult = strResult + "<fieldset style=\"margin:10px\"><legend>" + this.m_refMsg.GetMessage("lbl image data") + "</legend><table width=\"100%\"><tr><td class=\"info\" width=\"1%\" nowrap=\"true\" align=\"left\">" + this.m_refMsg.GetMessage("images label") + "</td><td width=\"99%\" align=\"left\">" + strImagePath + "</td></tr><tr><td class=\"info\" colomnspan=\"2\" align=\"left\"><img src=\"" + data.ImageThumbnail + "\" atl=\"Thumbnail\" /></td></tr></table></fieldset>";
               
            }
            if (strResult != "")
            {
                result.Append(strResult);
            }
            else
            {
                result.Append(this.m_refMsg.GetMessage("lbl nometadefined"));
            }
        }

        MetaDataValue.Text = result.ToString();
    }

    private void Display_PropertiesTab(ContentData data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "NAME";
        colBound.ItemStyle.CssClass = "label";
        PropertiesGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        PropertiesGrid.Columns.Add(colBound);
        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("NAME", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        int i = 0;
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic title");
        dr[1] = entry_edit_data.Title;
        dt.Rows.Add(dr);
        dr = dt.NewRow();

        content_title.Value = data.Title;

        dr[0] = m_refMsg.GetMessage("generic id");
        dr[1] = entry_edit_data.Id;
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic language");
        dr[1] = LanguageName;
        dt.Rows.Add(dr);

        // commerce

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl product type xml config");
        dr[1] = entry_edit_data.ProductType.Title;
        xml_id = entry_edit_data.ProductType.Id;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl calatog entry sku");
        dr[1] = entry_edit_data.Sku;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl number of units");
        dr[1] = entry_edit_data.QuantityMultiple;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl tax class");
        dr[1] = (new TaxClass(m_refContentApi.RequestInformationRef)).GetItem(entry_edit_data.TaxClassId).Name;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl archived");
        dr[1] = "<input type=\"checkbox\" " + (entry_edit_data.IsArchived ? "checked=\"checked\" " : "") + "disabled=\"disabled\" />";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl buyable");
        dr[1] = "<input type=\"checkbox\" " + (entry_edit_data.IsBuyable ? "checked=\"checked\" " : "") + "disabled=\"disabled\" />";
        dt.Rows.Add(dr);

        // dimensions

        string sizeMeasure = m_refMsg.GetMessage("lbl inches");
        string weightMeasure = m_refMsg.GetMessage("lbl pounds");

        if (m_refContentApi.RequestInformationRef.MeasurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.Metric)
        {

            sizeMeasure = m_refMsg.GetMessage("lbl centimeters");
            weightMeasure = m_refMsg.GetMessage("lbl kilograms");

        }

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl tangible");
        dr[1] = "<input type=\"checkbox\" " + (entry_edit_data.IsTangible ? "checked=\"checked\" " : "") + "disabled=\"disabled\" />";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl height");
        dr[1] = entry_edit_data.Dimensions.Height + " " + sizeMeasure;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl width");
        dr[1] = entry_edit_data.Dimensions.Width + " " + sizeMeasure;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl length");
        dr[1] = entry_edit_data.Dimensions.Length + " " + sizeMeasure;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl weight");
        dr[1] = entry_edit_data.Weight.Amount + " " + weightMeasure;
        dt.Rows.Add(dr);

        // dimensions

        // inventory
        InventoryApi inventoryApi = new InventoryApi();
        InventoryData inventoryData = inventoryApi.GetInventory(entry_edit_data.Id);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl disable inventory");
        dr[1] = "<input type=\"checkbox\" " + (entry_edit_data.DisableInventoryManagement ? "checked=\"checked\" " : "") + "disabled=\"disabled\" />";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl in stock");
        dr[1] = inventoryData.UnitsInStock;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl on order");
        dr[1] = inventoryData.UnitsOnOrder;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl reorder");
        dr[1] = inventoryData.ReorderLevel;
        dt.Rows.Add(dr);

        // inventory

        // end commerce

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("content status label");
        switch (entry_edit_data.ContentStatus.ToLower())
        {
            case "a":
                dr[1] = m_refMsg.GetMessage("status:Approved (Published)");
                break;
            case "o":
                dr[1] = m_refMsg.GetMessage("status:Checked Out");
                break;
            case "i":
                dr[1] = m_refMsg.GetMessage("status:Checked In");
                break;
            case "p":
                dr[1] = m_refMsg.GetMessage("status:Approved (PGLD)");
                break;
            case "m":
                dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
                break;
            case "s":
                dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
                break;
            case "t":
                dr[1] = m_refMsg.GetMessage("status:Waiting Approval");
                break;
            case "d":
                dr[1] = "Deleted (Pending Start Date)";
                break;
        }
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("content LUE label");
        dr[1] = entry_edit_data.LastEditorFirstName + " " + entry_edit_data.LastEditorLastName;
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("content LED label");
        dr[1] = entry_edit_data.DateModified;
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic start date label");
        if (entry_edit_data.GoLive == DateTime.MinValue || entry_edit_data.GoLive == DateTime.MaxValue)
        {
            dr[1] = m_refMsg.GetMessage("none specified msg");
        }
        else
        {
            dr[1] = entry_edit_data.GoLive.ToLongDateString() + " " + entry_edit_data.GoLive.ToShortTimeString();
        }
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic end date label");
        if (entry_edit_data.EndDate == DateTime.MinValue || entry_edit_data.EndDate == DateTime.MaxValue)
        {
            dr[1] = m_refMsg.GetMessage("none specified msg");
        }
        else
        {
            dr[1] = entry_edit_data.EndDate.ToLongDateString() + " " + entry_edit_data.EndDate.ToShortTimeString();
        }
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("End Date Action Title");
        if (!(entry_edit_data.EndDate == DateTime.MinValue || entry_edit_data.EndDate == DateTime.MaxValue))
        {
            if (entry_edit_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
            {
                dr[1] = m_refMsg.GetMessage("Archive display descrp");
            }
            else if (entry_edit_data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
            {
                dr[1] = m_refMsg.GetMessage("Refresh descrp");
            }
            else
            {
                dr[1] = m_refMsg.GetMessage("Archive expire descrp");
            }
        }
        else
        {
            dr[1] = m_refMsg.GetMessage("none specified msg");
        }
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("content DC label");
        dr[1] = data.DateCreated; //DisplayDateCreated
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = this.m_refMsg.GetMessage("lbl approval method");
        if (data.ApprovalMethod == 1)
        {
            dr[1] = m_refMsg.GetMessage("display for force all approvers");
        }
        else
        {
            dr[1] = m_refMsg.GetMessage("display for do not force all approvers");
        }
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("content approvals label");
        System.Text.StringBuilder approvallist = new System.Text.StringBuilder();
        if (approvaldata == null)
        {
            approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId);
        }
        approvallist.Append(m_refMsg.GetMessage("none specified msg"));
        if (!(approvaldata == null))
        {
            if (approvaldata.Length > 0)
            {
                approvallist.Length = 0;
                for (i = 0; i <= approvaldata.Length - 1; i++)
                {
                    if (approvaldata[i].Type.ToLower() == "user")
                    {
                        approvallist.Append("<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user") + "\" title=\"" + m_refMsg.GetMessage("approver is user") + "\">");
                    }
                    else
                    {
                        approvallist.Append("<img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user group") + "\" title=\"" + m_refMsg.GetMessage("approver is user group") + "\">");
                    }
                    if (approvaldata[i].IsCurrentApprover)
                    {
                        approvallist.Append("<span class=\"important\">");
                    }
                    else
                    {
                        approvallist.Append("<span>");
                    }
                    if (approvaldata[i].Type.ToLower() == "user")
                    {
                        approvallist.Append(approvaldata[i].DisplayUserName);
                    }
                    else
                    {
                        approvallist.Append(approvaldata[i].DisplayUserName);
                    }
                    approvallist.Append("</span>");
                }
            }
        }
        dr[1] = approvallist.ToString();
        dt.Rows.Add(dr);

        if (folder_data == null)
        {
            folder_data = m_refContentApi.EkContentRef.GetFolderById(entry_edit_data.FolderId);
        }

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("template label");

        if (m_refContent.MultiConfigExists(entry_edit_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage))
        {
            TemplateData t_templateData = m_refContent.GetMultiTemplateASPX(entry_edit_data.Id);
            if (t_templateData != null)
            {
                dr[1] = t_templateData.FileName;
            }
            else
            {
                dr[1] = folder_data.TemplateFileName;
            }
        }
        else
        {
            dr[1] = folder_data.TemplateFileName;
        }

        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic Path");
        dr[1] = data.Path;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("rating label");

        Collection dataCol = m_refContentApi.GetContentRatingStatistics(entry_edit_data.Id, 0, null);
        int total = 0;
        int sum = 0;
        int hits = 0;
        if (dataCol.Count > 0)
        {
            total = Convert.ToInt32 (dataCol["total"]);
            sum = Convert.ToInt32 (dataCol["sum"]);
            hits = Convert.ToInt32 (dataCol["hits"]);
        }

        if (total == 0)
        {
            dr[1] = m_refMsg.GetMessage("content not rated");
        }
        else
        {
            dr[1] = Math.Round(System.Convert.ToDouble(Convert.ToDouble((short)sum) / total), 2);
        }

        dt.Rows.Add(dr);

        //dr = dt.NewRow()
        //dr(0) = "Content Hits:"
        //dr(1) = hits

        //dt.Rows.Add(dr)

        dr = dt.NewRow();
        dr[0] = this.m_refMsg.GetMessage("lbl content searchable");
        dr[1] = data.IsSearchable.ToString();
        dt.Rows.Add(dr);

        DataView dv = new DataView(dt);
        PropertiesGrid.DataSource = dv;
        PropertiesGrid.DataBind();
    }

    private void Display_PricingTab()
    {

        Ektron.Cms.Workarea.workareabase workarearef = new Ektron.Cms.Workarea.workareabase();
        List<CurrencyData> activeCurrencyList = m_refCurrency.GetActiveCurrencyList();
        List<ExchangeRateData> exchangeRateList = new List<ExchangeRateData>();
        if (activeCurrencyList.Count > 1)
        {
            ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
            Criteria<ExchangeRateProperty> exchangeRateCriteria = new Criteria<ExchangeRateProperty>();
            List<long> currencyIDList = new List<long>();
            for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
            {
                currencyIDList.Add(activeCurrencyList[i].Id);
            }
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, CriteriaFilterOperator.In, currencyIDList.ToArray());
            exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria);
        }
        ltr_pricing.Text = workarearef.CommerceLibrary.GetPricingMarkup(entry_edit_data.Pricing, activeCurrencyList, exchangeRateList, entry_edit_data.EntryType, false, workareaCommerce.ModeType.View);

    }
    private void Display_MediaTab()
    {
        this.ucMedia.EntryEditData = this.entry_edit_data;
        this.ucMedia.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.View;
    }
    private void Display_ItemTab()
    {
        if (entry_edit_data != null)
        {
            this.ucItems.EntryEditData = entry_edit_data;
            this.ucItems.ItemsFolderId = m_iFolder;
            this.ucItems.SubscriptionControlPath = this.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/Membership.ascx";
            this.ucItems.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.View;
        }
    }
    private void Display_MetadataTab()
    {
        StringBuilder sbAttrib = new StringBuilder();
        StringBuilder sbResult = new StringBuilder();
        string strResult;
        string strAttrResult;
        string strImage = "";

        EnhancedMetadataScript.Text = CustomFields.GetEnhancedMetadataScript().Replace("src=\"java/", "src=\"../java/");
        EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea();
        if ((meta_data != null) || (prod_type_data != null))
        {
            m_refSite = new Ektron.Cms.Site.EkSite(this.m_refContentApi.RequestInformationRef);
            Hashtable hPerm = m_refSite.GetPermissions(m_iFolder, 0, "folder");

            if (prod_type_data != null)
            {
                sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(entry_edit_data.Attributes, prod_type_data.Id, false,prod_type_data.Attributes));
            }
        }
        if (m_sEditAction == "update")
        {
            strImage = entry_edit_data.Image;
            string strThumbnailPath = entry_edit_data.ImageThumbnail;
            if (entry_edit_data.ImageThumbnail == "")
            {
                strThumbnailPath = m_refContentApi.AppImgPath + "spacer.gif";
            }
            else if (catalog_data.IsDomainFolder == true)
            {
                strThumbnailPath = entry_edit_data.ImageThumbnail;
            }
            else
            {
                strThumbnailPath = m_refContentApi.SitePath + strThumbnailPath;
            }
        }
        strAttrResult = (string)(sbAttrib.ToString().Trim());
        strAttrResult = strAttrResult.Replace("src=\"java/", "src=\"../java/");
        strAttrResult = strAttrResult.Replace("src=\"images/", "src=\"../images/");

        strResult = sbResult.ToString().Trim();
        strResult = Util_FixPath(strResult);
        strResult = strResult.Replace("src=\"java/", "src=\"../java/");
        strResult = strResult.Replace("src=\"images/", "src=\"../images/");

        ltr_attrib.Text = strAttrResult;
    }
    private string Util_FixPath(string MetaScript)
    {
        int iTmp = -1;
        iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0);
        while (iTmp > -1)
        {
            iTmp = MetaScript.IndexOf(");return (false);", iTmp);
            MetaScript = MetaScript.Insert(iTmp, ", \'" + this.m_refContentApi.ApplicationPath + "\'");
            iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1);
        }
        return MetaScript;
    }
    private void ViewCatalogToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (content_data == null)
        {
            content_data = m_refContentApi.GetContentById(m_intId, 0);
        }
        long ParentId = content_data.FolderId;
        Ektron.Cms.Commerce.ProductType pProductType = new Ektron.Cms.Commerce.ProductType(m_refContentApi.RequestInformationRef);
        int count = 0;
        int lAddMultiType = 0;
        bool bSelectedFound = false;
        bool bViewContent = System.Convert.ToBoolean("view" == m_strPageAction); // alternative is archived content
        string SRC = "";
        string str;
        string backStr;
        bool bFromApproval = false;
        int type = 3333;
        bool folderIsHidden = m_refContentApi.IsFolderHidden(content_data.FolderId);
        bool IsOrdered = m_refContentApi.EkContentRef.IsOrdered(content_data.Id);

        if (type == 1)
        {
            if (bFromApproval)
            {
                backStr = "back_file=approval.aspx";
            }
            else
            {
                backStr = "back_file=content.aspx";
            }
        }
        else
        {
            backStr = "back_file=cmsform.aspx";
        }
        str = Request.QueryString["action"];
        if (str != null && str.Length > 0)
        {
            backStr = backStr + "&back_action=" + str;
        }

        if (bFromApproval)
        {
            str = Request.QueryString["page"];
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_page=" + str;
            }
        }

        if (!bFromApproval)
        {
            str = Request.QueryString["folder_id"];
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_folder_id=" + str;
            }
        }

        if (type == 1)
        {
            str = Request.QueryString["id"];
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_id=" + str;
            }
        }
        else
        {
            str = Request.QueryString["form_id"];
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_form_id=" + str;
            }
        }
        if (!(Request.QueryString["callerpage"] == null))
        {
            str = AntiXss.UrlEncode(Request.QueryString["callerpage"]);
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_callerpage=" + str;
            }
        }
        if (!(Request.QueryString["origurl"] == null))
        {
            str = Request.QueryString["origurl"];
            if (str != null && str.Length > 0)
            {
                backStr = backStr + "&back_origurl=" + EkFunctions.UrlEncode(str);
            }
        }
        str = ContentLanguage.ToString();
        if (str != null && str.Length > 0)
        {
            backStr = backStr + "&back_LangType=" + str + "&rnd=" + System.Convert.ToInt32(Conversion.Int((10 * VBMath.Rnd()) + 1));
        }

        SRC = (string)("commerce/catalogentry.aspx?close=false&LangType=" + ContentLanguage + "&id=" + m_intId + "&type=update&" + backStr);
        if (bFromApproval)
        {
            SRC += "&pullapproval=true";
        }

        if (m_strPageAction == "view" || m_strPageAction == "viewstaged")
        {
            string WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("lbl view catalog entry") + " \"" + content_data.Title + "\" ");
            if (m_strPageAction == "viewstaged")
            {
                WorkareaTitlebarTitle = WorkareaTitlebarTitle + m_refMsg.GetMessage("staged version msg");
            }
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);
        }

        result.Append("<table><tr>" + "\r\n");
        if ((security_data.CanAdd && bViewContent) || security_data.IsReadOnly == true)
        {

            if (security_data.CanAdd && bViewContent)
            {
                if (!bSelectedFound)
                {
                    lContentType = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes;
                }
            }
        }

        SetViewImage("");

		if (!folderIsHidden && content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) //hiding the move button for pagebuilder type.
		{
			if (Request.QueryString["callerpage"] == "dashboard.aspx")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
            else if (!String.IsNullOrEmpty(Request.QueryString["callerpage"]))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)(Request.QueryString["callerpage"] + "?" + HttpUtility.UrlDecode(Request.QueryString["origurl"])), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else if (Request.QueryString["backpage"] == "history")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + content_data.FolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
		}
		
		string buttonId = Guid.NewGuid().ToString();
		
        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + m_refMsg.GetMessage("lbl Action") + "</span></td>");

        if ((security_data.CanAdd) || security_data.IsReadOnly)
        {
			buttonId = Guid.NewGuid().ToString();
		
            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + m_refMsg.GetMessage("lbl View") + "</span></td>");
        }

		buttonId = Guid.NewGuid().ToString();
		
        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"chartBar\">" + m_refMsg.GetMessage("generic reports title") + "</span></td>");

        StringBuilder localizationMenuOptions = new StringBuilder();
        if (EnableMultilingual == 1)
        {
            string strViewDisplay = "";
            string strAddDisplay = "";
            LanguageData[] result_language;

            if (security_data.CanEdit || security_data.CanEditSumit)
            {
                LocalizationObject l10nObj = new LocalizationObject();
                Ektron.Cms.Localization.LocalizationState locState = l10nObj.GetContentLocalizationState(m_intId, content_data);
                if (m_refStyle.IsExportTranslationSupportedForContentType((EkEnumeration.CMSContentType)content_data.Type))
                {
                    string statusIcon = "";
                    string statusMsg = "";
                    m_refStyle.GetTranslationStatusIconAndMessage(locState, ref statusIcon, ref statusMsg);
                    // localizationMenuOptions.Append("    {0}.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + statusIcon + " \' />&nbsp;&nbsp;" + statusMsg + "\", function() { return false; } );" + Environment.NewLine);
                    // result.Append(m_refStyle.GetTranslationStatusMenu(content_data, m_refMsg.GetMessage("alt click here to update this content translation status"), m_refMsg.GetMessage("lbl mark ready for translation"), locState));
                    localizationMenuOptions.Append(m_refStyle.PopupTranslationMenu(content_data, locState, "actionmenu", statusMsg, statusIcon, false));
                    // result.Append(m_refStyle.PopupTranslationMenu(content_data, locState));

                    if (locState.IsExportableState())
                    {
                        localizationMenuOptions.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + m_refContentApi.AppPath + "images/UI/Icons/translation.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl Export for translation") + "\", function() { window.location.href=\"" + "content.aspx?LangType=" + ContentLanguage + "&action=Localize&backpage=View&id=" + m_intId + "&folder_id=" + content_data.FolderId + "\"; } );" + Environment.NewLine);
                        // result.Append(m_refStyle.GetExportTranslationButton((string)("content.aspx?LangType=" + ContentLanguage + "&action=Localize&backpage=View&id=" + m_intId + "&folder_id=" + content_data.FolderId), m_refMsg.GetMessage("alt Click here to export this content for translation"), m_refMsg.GetMessage("lbl Export for translation")));
                    }
                }
            }

            result_language = m_refContentApi.DisplayAddViewLanguage(m_intId);
            for (count = 0; count <= result_language.Length - 1; count++)
            {
                if (result_language[count].Type == "VIEW")
                {
                    if (content_data.LanguageId == result_language[count].Id)
                    {
                        strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + " selected>" + result_language[count].Name + "</option>";
                    }
                    else
                    {
                        strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
                    }
                }
            }

			bool languageDividerAdded = false;

            if (strViewDisplay != "")
            {
				result.Append(StyleHelper.ActionBarDivider);
				languageDividerAdded = true;
                result.Append("<td nowrap=\"true\">" + m_refMsg.GetMessage("lbl View") + ":");
                result.Append("<select id=viewcontent name=viewcontent OnChange=\"javascript:LoadContent(\'frmContent\',\'VIEW\');\">");
                result.Append(strViewDisplay);
                result.Append("</select></td>");
            }
            if (security_data.CanAdd)
            {
                //If (bCanAddNewLanguage) Then
                for (count = 0; count <= result_language.Length - 1; count++)
                {
                    if (result_language[count].Type == "ADD")
                    {
                        strAddDisplay = strAddDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
                    }
                }
                if (strAddDisplay != "")
                {
					if (!languageDividerAdded)
					{
						result.Append(StyleHelper.ActionBarDivider);
					}
					else
					{
						result.Append("<td>&nbsp;&nbsp;</td>");
					}
                    result.Append("<td class=\"label\">" + m_refMsg.GetMessage("add title") + ":");
                    if (folder_data == null)
                    {
                        folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
                    }
                    if (Utilities.IsNonFormattedContentAllowed(m_refContentApi.GetEnabledXmlConfigsByFolder(this.folder_data.Id)))
                    {
                        allowHtml = "&AllowHtml=1";
                    }
                    result.Append("<select id=addcontent name=addcontent OnChange=\"javascript:LoadContent(\'frmContent\',\'ADD\');\">");
                    result.Append("<option value=" + "0" + ">" + "-select language-" + "</option>");
                    result.Append(strAddDisplay);
                    result.Append("</select></td>");
                }
                //End If
            }

            //End If
        }

        bool canAddAssets = System.Convert.ToBoolean((security_data.CanAdd || security_data.CanAddFolders) && bViewContent);

		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");

        result.Append("<script language=\"javascript\">" + Environment.NewLine);

        result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
        if (security_data.CanAddFolders)
        {
            result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + "images/ui/icons/folderGreen.png" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl commerce catalog") + "\", function() { window.location.href = \'content.aspx?LangType=" + ContentLanguage + "&action=AddSubFolder&type=catalog&id=" + m_intId + "\' } );" + Environment.NewLine);
            result.Append("    filemenu.addBreak();" + Environment.NewLine);
        }

        if (security_data.IsCollections || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection))
        {
            result.Append("" + Environment.NewLine);
        }
        result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);
        if (security_data.CanHistory)
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/ui/icons/history.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("lbl content history"), 98) + "\", function() { top.document.getElementById(\'ek_main\').src=\"historyarea.aspx?action=report&LangType=" + ContentLanguage + "&id=" + m_intId + "\";return false;});" + Environment.NewLine);
        }
        if (content_data.Status != "A")
        {
            if (!((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= content_data.Type) && (content_data.Type <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)))
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/UI/Icons/contentViewDifferences.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("btn view diff"), 98) + "\", function() { PopEditWindow(\'compare.aspx?LangType=" + ContentLanguage + "&id=" + m_intId + "\', \'Compare\', 785, 500, 1, 1); } );" + Environment.NewLine);
            }
        }
        if (security_data.IsAdmin || IsFolderAdmin())
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/UI/Icons/approvals.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("btn view approvals"), 98) + "\", function() { location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&type=content&id=" + m_intId + "\";} );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/UI/Icons/permissions.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("btn view permissions"), 98) + "\", function() { location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&type=content&id=" + m_intId + "\";} );" + Environment.NewLine);
        }
        result.Append("    viewmenu.addBreak();" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + "images/ui/icons/brickLeftRight.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("lbl cross sell"), 98) + "\", function() { location.href = \"commerce/recommendations/recommendations.aspx?action=crosssell&folder=" + m_intFolderId + "&id=" + m_intId + "\";} );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + "images/ui/icons/brickUp.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("lbl up sell"), Ektron.Cms.Common.EkConstants.CMSContentType_Content) + "\", function() { location.href = \"commerce/recommendations/recommendations.aspx?action=upsell&folder=" + m_intFolderId + "&id=" + m_intId + "\";} );" + Environment.NewLine);
        if ((security_data.CanEditFolders && bViewContent) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            result.Append("    viewmenu.addBreak();" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/UI/Icons/properties.png" + "\' />&nbsp;&nbsp;" + MakeBold(m_refMsg.GetMessage("btn properties"), Ektron.Cms.Common.EkConstants.CMSContentType_Content) + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=EditContentProperties&id=" + m_intId + "\";} );" + Environment.NewLine);
        }

        if (((security_data.CanAdd) && bViewContent) || security_data.IsReadOnly == true)
        {
            if (!(asset_data == null))
            {
                if (asset_data.Length > 0)
                {
                    for (count = 0; count <= asset_data.Length - 1; count++)
                    {
                        if (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data[count].TypeId && asset_data[count].TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)
                        {
                            if ("*" == asset_data[count].PluginType)
                            {
                                lAddMultiType = asset_data[count].TypeId;
                            }
                            else
                            {
                                string imgsrc = string.Empty;
                                string txtCommName = string.Empty;
                                if (asset_data[count].TypeId == 101)
                                {
                                    imgsrc = "&nbsp;<img src=\'" + "images/UI/Icons/FileTypes/word.png" + "\' />&nbsp;&nbsp;";
                                    txtCommName = m_refMsg.GetMessage("lbl Office Documents");
                                }
                                else if (asset_data[count].TypeId == 102 || asset_data[count].TypeId == 106)
                                {
                                    imgsrc = "&nbsp;<img valign=\'center\' src=\'" + "images/UI/Icons/contentHtml.png" + " \' />&nbsp;&nbsp;";
                                    txtCommName = m_refMsg.GetMessage("lbl Managed Files");
                                }
                                else if (asset_data[count].TypeId == 104)
                                {
                                    imgsrc = "&nbsp;<img valign=\'center\' src=\'" + "images/UI/Icons/film.png" + " \' />&nbsp;&nbsp;";
                                    txtCommName = m_refMsg.GetMessage("lbl Multimedia");
                                }
                                else
                                {
                                    imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                }
                                if (asset_data[count].TypeId != 105)
                                {
                                    result.Append("viewmenu.addItem(\"" + imgsrc + "" + MakeBold(txtCommName, System.Convert.ToInt32(asset_data[count].TypeId)) + "\", function() { UpdateView(" + asset_data[count].TypeId + "); } );" + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }

            result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);

            result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
            result.Append("    deletemenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/UI/Icons/chartBar.png" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("content stats") + "\", function() { location.href = \"ContentStatistics.aspx?LangType=" + ContentLanguage + "&id=" + m_intId + "\";} );" + Environment.NewLine);
            result.Append("    deletemenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/ui/icons/chartPie.png" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl entry reports") + "\", function() { location.href = \"Commerce/reporting/analytics.aspx?LangType=" + ContentLanguage + "&id=" + m_intId + "\";} );" + Environment.NewLine);
            string quicklinkUrl = SitePath + content_data.Quicklink;
            if (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type, true) && Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_data.AssetData.FileExtension)))
            {
                quicklinkUrl = m_refContentApi.RequestInformationRef.AssetPath + content_data.Quicklink;
            }
            else if (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type, true) && SitePath != "/")
            {
                string appPathOnly = m_refContentApi.RequestInformationRef.ApplicationPath.Replace(SitePath, "");
                if (content_data.Quicklink.Contains(appPathOnly) || !content_data.Quicklink.Contains("downloadasset.aspx"))
                {
                    quicklinkUrl = SitePath + ((content_data.Quicklink.StartsWith("/")) ? (content_data.Quicklink.Substring(1)) : content_data.Quicklink);
                }
                else
                {
                    quicklinkUrl = m_refContentApi.RequestInformationRef.ApplicationPath + content_data.Quicklink;
                }
            }
            if (IsAnalyticsViewer() && ObjectFactory.GetAnalytics().HasProviders())
            {
                string modalUrl = string.Format("window.open(\"{0}/analytics/seo.aspx?tab=traffic&uri={1}\", \"Analytics400\", \"width=900,height=580,scrollable=1,resizable=1\");", ApplicationPath, quicklinkUrl);
                result.Append("    deletemenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' valign=\'center\' src=\'" + "images/ui/icons/chartBar.png" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl entry analytics") + "\", function() { " + modalUrl + " } );" + Environment.NewLine);
            }
            result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
        }

        result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
        if (security_data.CanEdit && (content_data.Status != "S" && content_data.Status != "O" || (content_data.Status == "O" && content_state_data.CurrentUserId == CurrentUserId)))
        {
            result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentEdit.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn edit") + "\", function() { javascript:top.document.getElementById(\'ek_main\').src=\'" + SRC + "\';return false;\"" + ",\'EDIT\',790,580,1,1);return false;" + "\" ; } );" + Environment.NewLine);
        }

        if (security_data.CanDelete)
        {
            string href;
            href = (string)("content.aspx?LangType=" + ContentLanguage + "&action=submitDelCatalogAction&delete_id=" + m_intId + "&page=" + Request.QueryString["calledfrom"] + "&folder_id=" + content_data.FolderId);
            if (!IsOrdered)
            {
                result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/delete.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn delete") + ("\", function() { DeleteConfirmationDialog(\'" + href) + "\');return false;} );" + Environment.NewLine);
            }
        }

        if (security_data.CanEdit)
        {

            if ((content_data.Status == "O") && ((content_state_data.CurrentUserId == CurrentUserId) || (security_data.IsAdmin || IsFolderAdmin())))
            {
                if ((content_data.Status == "O") && ((content_state_data.CurrentUserId == CurrentUserId) || (security_data.IsAdmin || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))))
                {

                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/ui/icons/checkIn.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn checkin") + "\", function() { DisplayHoldMsg(true); window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + m_intId + "&content_type=" + content_data.Type + "\" ; } );" + Environment.NewLine);

                }
                else if (IsFolderAdmin())
                {

                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/ui/icons/lockEdit.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl take ownership") + "\", function() { DisplayHoldMsg(true); window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=TakeOwnerShip&id=" + m_intId + "&content_type=" + content_data.Type + "\" ; } );" + Environment.NewLine);

                }

                if (m_strPageAction == "view")
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/preview.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view stage") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId + "\" ; } );" + Environment.NewLine);
                }
                else
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentViewPublished.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view publish") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "\" ; } );" + Environment.NewLine);
                }
            }
            else if (((content_data.Status == "I") || (content_data.Status == "T")) && (content_data.UserId == CurrentUserId))
            {
                if (security_data.CanPublish)
                {
                    bool metaRequuired = false;
                    bool categoryRequired = false;
                    bool manaliasRequired = false;
                    string msg = string.Empty;
                    m_refContentApi.EkContentRef.ValidateMetaDataTaxonomyAndAlias(content_data.FolderId, content_data.Id, content_data.LanguageId, ref metaRequuired, ref categoryRequired, ref manaliasRequired);
                    if (metaRequuired == false && categoryRequired == false && manaliasRequired == false)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/application/commerce/submit.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn publish") + "\", function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + m_intId + "\" ; } } );" + Environment.NewLine);
                    }
                    else
                    {
                        if (metaRequuired && categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and manualalias and category required");
                        }
                        else if (metaRequuired && categoryRequired && !manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and category required");
                        }
                        else if (metaRequuired && !categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and manualalias required");
                        }
                        else if (!metaRequuired && categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate manualalias and category required");
                        }
                        else if (metaRequuired)
                        {
                            msg = m_refMsg.GetMessage("validate meta required");
                        }
                        else if (manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate manualalias required");
                        }
                        else
                        {
                            msg = m_refMsg.GetMessage("validate category required");
                        }
                        result.Append("    actionmenu.addItem(\"&nbsp;<img  height=\'16px\' width=\'16px\' src=\'" + "images/application/commerce/submit.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn publish") + "\", function() { DisplayHoldMsg(true); window.location.href = \"alert(\'" + msg + "\')\"" + "; } );" + Environment.NewLine);
                    }
                }
                else
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", "content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + m_intId + "&fldid=" + content_data.FolderId + "&page=workarea", m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "onclick=\"DisplayHoldMsg(true);return CheckForMeta(" + Convert.ToInt32(security_data.CanMetadataComplete) + ");\"")); //TODO need to pass integer not boolean
                }
                if (m_strPageAction == "view")
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/preview.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view stage") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId + "&fldid=" + content_data.FolderId + "\" ; } );" + Environment.NewLine);
                }
                else
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentViewPublished.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view publish") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "&fldid=" + content_data.FolderId + "\" ; } );" + Environment.NewLine);
                }
            }
            else if ((content_data.Status == "O") || (content_data.Status == "I") || (content_data.Status == "S") || (content_data.Status == "T") || (content_data.Status == "P"))
            {

                if (m_strPageAction == "view")
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/preview.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view stage") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId + "&fldid=" + content_data.FolderId + "\" ; } );" + Environment.NewLine);
                }
                else
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentViewPublished.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view publish") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "&fldid=" + content_data.FolderId + "\" ; } );" + Environment.NewLine);
                }
            }

            if (content_data.Status == "S" || content_data.Status == "M")
            {

                Util_CheckIsCurrentApprover(CurrentUserId);

                ApprovalScript.Visible = true;
                string AltPublishMsg = "";
                string AltApproveMsg = "";
                string AltDeclineMsg = "";
                string PublishIcon = "";
                string CaptionKey = "";
                bool m_TaskExists = m_refContent.DoesTaskExistForContent(content_data.Id);
                string m_sPage = "workarea"; //To be remove not required.
                if (content_data.Status == "S")
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)");
                    PublishIcon = "commerce/submit.gif";
                    CaptionKey = "btn publish";
                }
                else
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)");
                    PublishIcon = "../UI/Icons/delete.png";
                    CaptionKey = "btn delete";
                }
                if (security_data.CanPublish && IsLastApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/" + PublishIcon + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage(CaptionKey) + "\", function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = (\'content.aspx?action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/" + PublishIcon + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage(CaptionKey) + "\", function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = \"content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "" + "\" ; } } );" + Environment.NewLine);
                    }
                }
                else if (security_data.CanApprove && IsCurrentApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/Commerce/Approve.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn approve") + "\", function() { DisplayHoldMsg(true); window.location.href = (\'content.aspx?action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/Commerce/Approve.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn approve") + "\", function() { DisplayHoldMsg(true); window.location.href = \"content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "" + "\" ; } );" + Environment.NewLine);
                    }
                }
                if ((security_data.CanPublish || security_data.CanApprove) && IsCurrentApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/DMSMenu/page_white_decline.gif" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn decline") + "\", function() { window.location.href = (\'content.aspx?action=declineContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/DMSMenu/page_white_decline.gif" + "\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn decline") + "\", function() { DeclineContent(\'" + content_data.Id + "\', \'" + content_data.FolderId + "\', \'" + m_sPage + "\', \'" + ContentLanguage + "\')" + " ; } );" + Environment.NewLine);
                    }
                }
            }
        }
        else
        {
            if (content_data.Status == "S" || content_data.Status == "M")
            {
                Util_CheckIsCurrentApprover(CurrentUserId);

                ApprovalScript.Visible = true;
                string AltPublishMsg = "";
                string AltApproveMsg = "";
                string AltDeclineMsg = "";
                string PublishIcon = "";
                string CaptionKey = "";
                bool m_TaskExists = m_refContent.DoesTaskExistForContent(content_data.Id);
                string m_sPage = "workarea"; //To be remove not required.
                if (content_data.Status == "S")
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)");
                    PublishIcon = "commerce/submit.gif";
                    CaptionKey = "btn publish";
                }
                else
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)");
                    PublishIcon = "commerce/ApproveDelete.png";
                    CaptionKey = "approvals:lbl publish msg (delete)";
                }
                if (security_data.CanPublish && IsLastApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/" + PublishIcon + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage(CaptionKey) + "\", function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = (\'content.aspx?action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/" + PublishIcon + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage(CaptionKey) + "\", function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = \"content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "" + "\" ; } } );" + Environment.NewLine);
                    }
                }
                else if (security_data.CanApprove && IsCurrentApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/Commerce/Approve.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn approve") + "\", function() { DisplayHoldMsg(true); window.location.href = (\'content.aspx?action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/Commerce/Approve.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn approve") + "\", function() { DisplayHoldMsg(true); window.location.href = \"content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "" + "\" ; } );" + Environment.NewLine);
                    }
                }
                if ((security_data.CanPublish || security_data.CanApprove) && IsCurrentApproval)
                {
                    if (m_TaskExists == true)
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/DMSMenu/page_white_decline.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn decline") + "\", function() { window.location.href = (\'content.aspx?action=declineContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\') ; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/application/DMSMenu/page_white_decline.gif" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn decline") + "\", function() { DeclineContent(\'" + content_data.Id + "\', \'" + content_data.FolderId + "\', \'" + m_sPage + "\', \'" + ContentLanguage + "\')" + " ; } );" + Environment.NewLine);
                    }
                }
                if (security_data.CanEditSumit)
                {
                    // Don't show edit button for Mac when using XML config:
                    if (!(m_bIsMac && (content_data.XmlConfiguration != null)) || m_SelectedEditControl == "ContentDesigner")
                    {
                        // result.Append(m_refStyle.GetEditAnchor(m_intId, , True))
                    }
                }
                if (m_strPageAction == "view")
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img  height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/preview.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view stage") + "\", function() { window.location.href = \"content.aspx?action=ViewStaged&id=" + m_intId + "&LangType=" + ContentLanguage + "\" ; } );" + Environment.NewLine);
                }
                else
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentViewPublished.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view publish") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "\" ; } );" + Environment.NewLine);
                }
                //End If
                //END
            }
            else
            {
                if ((content_data.Status == "O") && ((security_data.IsAdmin || IsFolderAdmin()) || (security_data.CanBreakPending)))
                {
                    if ((content_data.Status == "O") && ((content_state_data.CurrentUserId == CurrentUserId) || (security_data.IsAdmin || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))))
                    {

                        result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/ui/icons/checkIn.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn checkin") + "\", function() { DisplayHoldMsg(true); window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + m_intId + "&fldid=" + content_data.FolderId + "&page=workarea" + "&content_type=" + content_data.Type + "\" ; \"DisplayHoldMsg(true);return true;\"" + " } );" + Environment.NewLine);

                    }

                    if (m_strPageAction == "view")
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img  height=\'16px\' width=\'16px\'  src=\'" + "images/UI/Icons/preview.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view stage") + "\", function() { window.location.href = \"content.aspx?action=ViewStaged&id=" + m_intId + "&LangType=" + ContentLanguage + "\" ; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/contentViewPublished.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn view publish") + "\", function() { window.location.href = \"content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "\" ; } );" + Environment.NewLine);
                    }
                }
            }
        }
        result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/linkSearch.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn link search") + "\", function() { window.location.href = \"isearch.aspx?LangType=" + ContentLanguage + "&action=dofindcontent&folderid=0&content_id=" + m_intId + ((content_data.AssetData.MimeType.IndexOf("image") != -1) ? "&asset_name=" + content_data.AssetData.Id + "." + content_data.AssetData.FileExtension : "") + "\" ; } );" + Environment.NewLine);
        if (security_data.CanAddTask)
        {
            result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + "images/UI/Icons/taskAdd.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("btn add task") + "\", function() { window.location.href = \"tasks.aspx?LangType=" + ContentLanguage + "&action=AddTask&cid=" + m_intId + "&callbackpage=content.aspx&parm1=action&value1=" + m_strPageAction + "&parm2=id&value2=" + m_intId + "&parm3=LangType&value3=" + ContentLanguage + "\" ; } );" + Environment.NewLine);
        }
        if (localizationMenuOptions.Length > 0)
        {
            result.Append("    actionmenu.addBreak();" + Environment.NewLine);
            result.Append(localizationMenuOptions); 
        }
        result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);
        result.Append("" + Environment.NewLine);
        htmToolBar.InnerHtml = result.ToString();
    }

    private string MakeBold(string str, int ContentType)
    {
        if (g_ContentTypeSelected == ContentType.ToString())
        {
            return "" + str + "";
        }
        else
        {
            return str;
        }

    }
    private void SetViewImage(string @override)
    {
        string scheckval = "";
        if (@override != "")
        {
            scheckval = @override;
        }
        else
        {
            scheckval = g_ContentTypeSelected;
        }
        switch (scheckval)
        {
            case "101":
                ViewImage = "images/UI/Icons/FileTypes/word.png";
                break;
            case "105":
                ViewImage = "images/UI/Icons/FileTypes/text.png";
                break;
            case "102":
            case "106":
                ViewImage = "images/UI/Icons/contentDMSDocument.png";
                break;
            case "104":
                ViewImage = "images/UI/Icons/film.png";
                break;
            case "96":
                ViewImage = "images/UI/Icons/folderView.png";
                break;
            case "1":
                ViewImage = "images/UI/Icons/contentHtml.png";
                break;
            case "2":
                ViewImage = "images/UI/Icons/contentForm.png";
                break;
            default:
                break;

        }
    }
    private void ViewToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string strAssetId = content_data.AssetData.Id;
        bool bIsAsset = false;
        Hashtable asset_info = new Hashtable();
        int i;
        bool folderIsHidden = m_refContentApi.IsFolderHidden(content_data.FolderId);

        bIsAsset = Utilities.IsAsset(content_data.Type, strAssetId);
        if (bIsAsset)
        {

            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                asset_info.Add(Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i], "");
            }
            asset_info["AssetID"] = content_data.AssetData.Id; //(m_AssetInfoKeys(i))
            asset_info["AssetVersion"] = content_data.AssetData.Version;
            asset_info["AssetFilename"] = content_data.AssetData.FileName;
            asset_info["MimeType"] = content_data.AssetData.MimeType;
            asset_info["FileExtension"] = content_data.AssetData.FileExtension;
            asset_info["MimeName"] = content_data.AssetData.MimeName;
            asset_info["ImageUrl"] = content_data.AssetData.ImageUrl;

            //This code is used to pass the file name to the control to handle work-offline feature.
            if (content_data.AssetData.FileName.Trim() != "")
            {
                lblContentTitle.Text = content_data.AssetData.FileName;
            }
            else
            {
                lblContentTitle.Text = content_data.Title;
            }


            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                AssetHidden.Text += "<input type=\"hidden\" name=\"asset_" + Strings.LCase(Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i]) + "\" value=\"" + EkFunctions.HtmlEncode(asset_info[Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i]].ToString()) + "\">";
            }
            AssetHidden.Text += "<script language=\"JavaScript\" src=\"" + m_refContentApi.AppPath + "Tree/js/com.ektron.utils.string.js\"></script>" + "\r\n";
            AssetHidden.Text += "<script language=\"JavaScript\" src=\"" + m_refContentApi.AppPath + "Tree/js/com.ektron.utils.cookie.js\"></script>" + "\r\n";
            AssetHidden.Text += "<script language=\"JavaScript\" src=\"" + m_refContentApi.AppPath + "java/assetevents.js\"></script>" + "\r\n";
            AssetHidden.Text += "<script language=\"JavaScript\">" + "\r\n";
            AssetHidden.Text += "setTimeout(\"SetTraceFormName()\",1);" + "\r\n";
            AssetHidden.Text += "function SetTraceFormName()" + "\r\n";
            AssetHidden.Text += "{" + "\r\n";
            AssetHidden.Text += "if (\"object\" == typeof g_AssetHandler)" + "\r\n";
            AssetHidden.Text += "{" + "\r\n";
            AssetHidden.Text += "g_AssetHandler.formName = \"frmContent\";" + "\r\n";
            AssetHidden.Text += "}" + "\r\n";
            AssetHidden.Text += "}" + "\r\n";
            AssetHidden.Text += "</script>";
        }

        if (m_strPageAction == "view" || m_strPageAction == "viewstaged")
        {
            string WorkareaTitlebarTitle = (string)(m_refMsg.GetMessage("view content title") + " \"" + content_data.Title + "\"");
            if (m_strPageAction == "viewstaged")
            {
                WorkareaTitlebarTitle = WorkareaTitlebarTitle + m_refMsg.GetMessage("staged version msg");
            }
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);
        }

        result.Append("<table><tr>");

		if (!folderIsHidden)
		{
			if (Request.QueryString["callerpage"] == "dashboard.aspx")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else if (Request.QueryString["callerpage"] != null)
			{
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)(AntiXss.UrlEncode(Request.QueryString["callerpage"]) + "?" + EkFunctions.HtmlEncode(Request.QueryString["origurl"]).Replace("&amp;", "&")), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else if (Request.QueryString["backpage"] == "history")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)(EkFunctions.HtmlEncode("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + content_data.FolderId)), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
		}

		bool primaryCssClassApplied = false;

        if (security_data.CanEdit)
        {
            // Don't show edit button for Mac when using XML config:
            if (!(m_bIsMac && (content_data.XmlConfiguration != null)) || m_SelectedEditControl == "ContentDesigner")
            {
                if (content_data.Type == 3333)
                {
					result.Append(m_refStyle.GetCatalogEditAnchor(m_intId, 3333, false, !primaryCssClassApplied));
                }
                else
                {
                    if (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                    {
						result.Append(m_refStyle.GetEditAnchor(m_intId, 1, false, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData, !primaryCssClassApplied)); // to be commented out
                    }
                    else
                    {
						result.Append(m_refStyle.GetEditAnchor(m_intId, 1, false, EkEnumeration.CMSContentSubtype.Content, !primaryCssClassApplied)); // to be commented out
                    }

					result.Append(m_refStyle.GetPageBuilderEditAnchor(m_intId, content_data, !primaryCssClassApplied));
                }

				primaryCssClassApplied = true;
            }
        }

        if (security_data.CanEdit)
        {
            if ((content_data.Status == "O") && ((content_state_data.CurrentUserId == CurrentUserId) || (security_data.IsAdmin || IsFolderAdmin())))
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/checkIn.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + m_intId + "&content_type=" + content_data.Type), m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "", StyleHelper.CheckInButtonCssClass, !primaryCssClassApplied));

				primaryCssClassApplied = true;

				if (m_strPageAction == "view")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass));
                }
            }
            else if (((content_data.Status == "I") || (content_data.Status == "T")) && (content_data.UserId == CurrentUserId))
            {
                if (security_data.CanPublish)
                {
                    bool metaRequuired = false;
                    bool categoryRequired = false;
                    bool manaliasRequired = false;
                    string msg = string.Empty;
                    m_refContentApi.EkContentRef.ValidateMetaDataTaxonomyAndAlias(content_data.FolderId, content_data.Id, content_data.LanguageId, ref metaRequuired, ref categoryRequired, ref manaliasRequired);
                    if (metaRequuired == false && categoryRequired == false && manaliasRequired == false)
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentPublish.png", "content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + m_intId + "&fldid=" + content_data.FolderId + "&page=workarea", m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "onclick=\"DisplayHoldMsg(true);return CheckForMeta(" + Convert.ToInt32(security_data.CanMetadataComplete) + ");\"", StyleHelper.PublishButtonCssClass, !primaryCssClassApplied));
                    }
                    else
                    {
                        if (metaRequuired && categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and manualalias and category required");
                        }
                        else if (metaRequuired && categoryRequired && !manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and category required");
                        }
                        else if (metaRequuired && !categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate meta and manualalias required");
                        }
                        else if (!metaRequuired && categoryRequired && manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate manualalias and category required");
                        }
                        else if (metaRequuired)
                        {
                            msg = m_refMsg.GetMessage("validate meta required");
                        }
                        else if (manaliasRequired)
                        {
                            msg = m_refMsg.GetMessage("validate manualalias required");
                        }
                        else
                        {
                            msg = m_refMsg.GetMessage("validate category required");
                        }

						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentPublish.png", "#", m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "onclick=\"alert(\'" + msg + "\');\"", StyleHelper.PublishButtonCssClass, !primaryCssClassApplied));
                    }

					primaryCssClassApplied = true;
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", "content.aspx?LangType=" + ContentLanguage + "&action=Submit&id=" + m_intId + "&fldid=" + content_data.FolderId + "&page=workarea", m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "onclick=\"DisplayHoldMsg(true);return CheckForMeta(" + Convert.ToInt32(security_data.CanMetadataComplete) + ");\"", StyleHelper.SubmitForApprovalButtonCssClass, !primaryCssClassApplied)); //TODO need to pass integer not boolean

					primaryCssClassApplied = true;
				}

                if (m_strPageAction == "view")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId + "&fldid=" + content_data.FolderId), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "&fldid=" + content_data.FolderId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass));
                }
            }
            else if ((content_data.Status == "O") || (content_data.Status == "I") || (content_data.Status == "S") || (content_data.Status == "T") || (content_data.Status == "P"))
            {
                if (m_strPageAction == "view")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + m_intId + "&fldid=" + content_data.FolderId), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass, !primaryCssClassApplied));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId + "&fldid=" + content_data.FolderId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass, !primaryCssClassApplied));
                }

				primaryCssClassApplied = true;
            }
        }
        else
        {
            //NEW CODE IMPLEMENTATION ADDED BY UDAI On 06/16/05 FOR THE DEFECT#13694,13914
            //BEGIN
            if (content_data.Status == "S" || content_data.Status == "M")
            {
                ApprovalScript.Visible = true;
                string AltPublishMsg = "";
                string AltApproveMsg = "";
                string AltDeclineMsg = "";
                string PublishIcon = "";
                string CaptionKey = "";
                bool m_TaskExists = m_refContent.DoesTaskExistForContent(content_data.Id);
                string m_sPage = "workarea"; //To be remove not required.
				string aPublishTagClass;

                if (content_data.Status == "S")
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)");
                    PublishIcon = "../UI/Icons/contentPublish.png";
                    CaptionKey = "btn publish";
					aPublishTagClass = StyleHelper.PublishButtonCssClass;
                }
                else
                {
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)");
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)");
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)");
                    PublishIcon = "../UI/Icons/delete.png";
                    CaptionKey = "btn delete";
					aPublishTagClass = StyleHelper.DeleteButtonCssClass;
                }

                if (security_data.CanPublish && content_state_data.CurrentUserId == CurrentUserId)
                {
                    if (m_TaskExists == true)
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + PublishIcon, "#", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "Onclick=\"javascript:return LoadChildPage(\'action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\');\"", aPublishTagClass, !primaryCssClassApplied));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + PublishIcon, "content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "", aPublishTagClass, !primaryCssClassApplied));
                    }

					primaryCssClassApplied = true;
                }
                else if (security_data.CanApprove && content_state_data.CurrentUserId == CurrentUserId)
                {
                    if (m_TaskExists == true)
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvalApproveItem.png", "#", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "Onclick=\"javascript:return LoadChildPage(\'action=approveContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\');\"", StyleHelper.ApproveButtonCssClass, !primaryCssClassApplied));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvalApproveItem.png", "content.aspx?action=approvecontent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + ContentLanguage + "", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "", StyleHelper.ApproveButtonCssClass, !primaryCssClassApplied));
                    }

					primaryCssClassApplied = true;
                }

                if ((security_data.CanPublish || security_data.CanApprove) && content_state_data.CurrentUserId == CurrentUserId)
                {
                    if (m_TaskExists == true)
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_decline-nm.gif", "#", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "Onclick=\"javascript:return LoadChildPage(\'action=declineContent&id=" + content_data.Id + "&fldid=" + content_data.FolderId + "&page=" + m_sPage + "&LangType=" + content_data.LanguageId + "\');\"", StyleHelper.DeclineButtonCssClass, !primaryCssClassApplied));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_decline-nm.gif", "javascript:DeclineContent(\'" + content_data.Id + "\', \'" + content_data.FolderId + "\', \'" + m_sPage + "\', \'" + ContentLanguage + "\')", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "", StyleHelper.DeclineButtonCssClass, !primaryCssClassApplied));
                    }

					primaryCssClassApplied = true;
                }

                if (security_data.CanEditSumit)
                {
                    // Don't show edit button for Mac when using XML config:
                    if (!(m_bIsMac && (content_data.XmlConfiguration != null)) || m_SelectedEditControl == "ContentDesigner")
                    {
                        if (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                        {
							result.Append(m_refStyle.GetEditAnchor(m_intId, 1, true, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData, !primaryCssClassApplied));
                        }
                        else
                        {
							result.Append(m_refStyle.GetEditAnchor(m_intId, 1, true, content_data.SubType, !primaryCssClassApplied));
                        }

						primaryCssClassApplied = true;

                        result.Append(m_refStyle.GetPageBuilderEditAnchor(m_intId, content_data));
                    }
                }

                if (m_strPageAction == "view")
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("content.aspx?action=ViewStaged&id=" + m_intId + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass, !primaryCssClassApplied));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass, !primaryCssClassApplied));
                }
                //End If
                //END
            }
            else
            {
                if ((content_data.Status == "O") && ((security_data.IsAdmin || IsFolderAdmin()) || (security_data.CanBreakPending)))
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/checkIn.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=CheckIn&id=" + m_intId + "&fldid=" + content_data.FolderId + "&page=workarea" + "&content_type=" + content_data.Type), m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "onclick=\"DisplayHoldMsg(true);return true;\"", StyleHelper.CheckInButtonCssClass, true));
                    
					if (m_strPageAction == "view")
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/preview.png", (string)("content.aspx?action=ViewStaged&id=" + m_intId + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), "", StyleHelper.ViewStagedButtonCssClass));
                    }
                    else
                    {
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewPublished.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass));
                    }
                }
            }
        }

		if (security_data.CanHistory)
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/history.png", "#", m_refMsg.GetMessage("alt history button text"), m_refMsg.GetMessage("lbl generic history"), "onclick=\"top.document.getElementById(\'ek_main\').src=\'historyarea.aspx?action=report&LangType=" + ContentLanguage + "&id=" + m_intId + "\';return false;\"", StyleHelper.HistoryButtonCssClass));
		}

        if (security_data.CanDelete)
        {
            string href;
            href = (string)("content.aspx?LangType=" + ContentLanguage + "&action=submitDelContAction&delete_id=" + m_intId + "&page=" + Request.QueryString["calledfrom"] + "&folder_id=" + content_data.FolderId);
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "onclick=\"DeleteConfirmationDialog(\'" + href + "\');return false;\" ", StyleHelper.DeleteButtonCssClass));
        }

		result.Append(StyleHelper.ActionBarDivider);

        if (content_data.Status != "A")
        {
            if (!((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= content_data.Type) && (content_data.Type <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)))
            {
                if (content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentViewDifferences.png", "#", "View Difference", m_refMsg.GetMessage("btn view diff"), "onclick=\"PopEditWindow(\'compare.aspx?LangType=" + ContentLanguage + "&id=" + m_intId + "\', \'Compare\', 785, 650, 1, 1);\"", StyleHelper.ViewDifferenceButtonCssClass));
                }
            }
        }
      
        if (security_data.IsAdmin || IsFolderAdmin())
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/permissions.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewPermissions&type=content&id=" + m_intId), m_refMsg.GetMessage("alt permissions button text content (view)"), m_refMsg.GetMessage("btn view permissions"), "", StyleHelper.ViewPermissionsButtonCssClass));
            if (!folderIsHidden)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/approvals.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&type=content&id=" + m_intId), m_refMsg.GetMessage("alt approvals button text content (view)"), m_refMsg.GetMessage("btn view approvals"), "", StyleHelper.ViewApprovalsButtonCssClass));
            }
        }
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/linkSearch.png", (string)("isearch.aspx?LangType=" + ContentLanguage + "&action=dofindcontent&folderid=0&content_id=" + m_intId + ((content_data.AssetData.MimeType.IndexOf("image") != -1) ? "&asset_name=" + content_data.AssetData.Id + "." + content_data.AssetData.FileExtension : "")), m_refMsg.GetMessage("btn link search"), m_refMsg.GetMessage("btn link search"), "", StyleHelper.SearchButtonCssClass));

		result.Append(StyleHelper.ActionBarDivider);

        if (security_data.CanAddTask)
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/taskAdd.png", (string)("tasks.aspx?LangType=" + ContentLanguage + "&action=AddTask&cid=" + m_intId + "&callbackpage=content.aspx&parm1=action&value1=" + m_strPageAction + "&parm2=id&value2=" + m_intId + "&parm3=LangType&value3=" + ContentLanguage), m_refMsg.GetMessage("btn add task"), m_refMsg.GetMessage("btn add task"), "", StyleHelper.AddTaskButtonCssClass));
        }

		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/chartBar.png", (string)("ContentStatistics.aspx?LangType=" + ContentLanguage + "&id=" + m_intId), m_refMsg.GetMessage("click view content reports"), m_refMsg.GetMessage("click view content reports"), "", StyleHelper.ViewReportButtonCssClass));
        string quicklinkUrl = SitePath + content_data.Quicklink;
        if (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type, true) && Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_data.AssetData.FileExtension)))
        {
            quicklinkUrl = m_refContentApi.RequestInformationRef.AssetPath + content_data.Quicklink;
        }
        else if (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type, true) && SitePath != "/")
        {
            string appPathOnly = m_refContentApi.RequestInformationRef.ApplicationPath.Replace(SitePath, "");
            if (content_data.Quicklink.Contains(appPathOnly) || !content_data.Quicklink.Contains("downloadasset.aspx"))
            {
                quicklinkUrl = SitePath + ((content_data.Quicklink.StartsWith("/")) ? (content_data.Quicklink.Substring(1)) : content_data.Quicklink);
            }
            else
            {
                quicklinkUrl = m_refContentApi.RequestInformationRef.ApplicationPath + content_data.Quicklink;
            }
        }
        if (IsAnalyticsViewer() && ObjectFactory.GetAnalytics().HasProviders())
        {
            string modalUrl = string.Format("onclick=\"window.open(\'{0}/analytics/seo.aspx?tab=traffic&uri={1}\', \'Analytics400\', \'width=900,height=580,scrollable=1,resizable=1\');return false;\"", ApplicationPath, quicklinkUrl);
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/chartPie.png", "#", m_refMsg.GetMessage("lbl entry analytics"), m_refMsg.GetMessage("lbl entry analytics"), modalUrl, StyleHelper.ViewAnalyticsButtonCssClass));
        }

        if (security_data.IsAdmin || IsFolderAdmin())
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/properties.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=EditContentProperties&id=" + m_intId), m_refMsg.GetMessage("btn edit prop"), m_refMsg.GetMessage("btn edit prop"), "", StyleHelper.EditPropertiesButtonCssClass));
        }


        //Sync API needs to know folder type to display the eligible sync profiles.
        if (this.folder_data == null)
        {
            folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
        }

        SiteAPI site = new SiteAPI();
        EkSite ekSiteRef = site.EkSiteRef;
        if ((m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncUser)) && (LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Feature.eSync)) && m_refContentApi.RequestInformationRef.IsSyncEnabled)
        {
            if ((m_strPageAction == "view") && (content_data.Status.ToUpper() == "A") && ServerInformation.IsStaged())
            {
                if (content_data.SubType != EkEnumeration.CMSContentSubtype.WebEvent)
                {
                    if (folder_data.IsDomainFolder)
                    {
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=\"Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(" + ContentLanguage + "," + m_intId + ",\'" + content_data.AssetData.Id + "\',\'" + content_data.AssetData.Version + "\'," + content_data.FolderId + ",true);return false;\"", StyleHelper.SyncButtonCssClass));
                    }
                    else
                    {
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=\"Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(" + ContentLanguage + "," + m_intId + ",\'" + content_data.AssetData.Id + "\',\'" + content_data.AssetData.Version + "\'," + content_data.FolderId + ",false);return false;\"", StyleHelper.SyncButtonCssClass));
                    }
                }
            }
        }

        if (EnableMultilingual == 1)
        {
            string strViewDisplay = "";
            string strAddDisplay = "";
            LanguageData[] result_language;
            int count = 0;

            if (security_data.CanEdit || security_data.CanEditSumit)
            {
                LocalizationObject l10nObj = new LocalizationObject();
                int sourceLanguageId;
                DateTime sourceDateModified;
                Ektron.Cms.Localization.LocalizationState locState = l10nObj.GetContentLocalizationState(m_intId, content_data, out sourceLanguageId, out sourceDateModified);
                if (-1 == sourceLanguageId) sourceLanguageId = ContentLanguage;

				bool addedTranslationDivider = false;

                if (m_refStyle.IsExportTranslationSupportedForContentType((EkEnumeration.CMSContentType)content_data.Type))
                {
					var statusMenu = m_refStyle.GetTranslationStatusMenu(content_data, m_refMsg.GetMessage("alt click here to update this content translation status"), m_refMsg.GetMessage("lbl mark translation status"), locState);

					if (!String.IsNullOrEmpty(statusMenu))
					{
						if (!addedTranslationDivider)
						{
							result.Append(StyleHelper.ActionBarDivider);
							addedTranslationDivider = true;
						}

						result.Append(statusMenu);
					}

					var statusPopUpMenu = m_refStyle.PopupTranslationMenu(content_data, locState);

					if (!String.IsNullOrEmpty(statusMenu))
					{
						if (!addedTranslationDivider)
						{
							result.Append(StyleHelper.ActionBarDivider);
							addedTranslationDivider = true;
						}

						result.Append(statusPopUpMenu);
					}

                    if (locState.IsExportableState())
                    {
						var exportButton = m_refStyle.GetExportTranslationButton((string)("content.aspx?LangType=" + sourceLanguageId + "&action=Localize&backpage=View&id=" + m_intId + "&folder_id=" + content_data.FolderId), m_refMsg.GetMessage("alt Click here to export this content for translation"), m_refMsg.GetMessage("lbl Export for translation"));

						if (!String.IsNullOrEmpty(exportButton))
						{
							if (!addedTranslationDivider)
							{
								result.Append(StyleHelper.ActionBarDivider);
								addedTranslationDivider = true;
							}

							result.Append(exportButton);
						}
                    }
                }
                if (System.Configuration.ConfigurationSettings.AppSettings["ek_ContentFallback"] != null && Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["ek_ContentFallback"]))
                {
					if (!addedTranslationDivider)
					{
						result.Append(StyleHelper.ActionBarDivider);
						addedTranslationDivider = true;
					}

					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/listBullet.png", "localization/contentfallback.aspx?id=" + m_intId + "&folder_id=" + content_data.FolderId, m_refMsg.GetMessage("alt edit content fallback"), m_refMsg.GetMessage("alt edit content fallback"), "", StyleHelper.EditFallbackButtonCssClass));
                }
            }

            result_language = m_refContentApi.DisplayAddViewLanguage(m_intId);
            for (count = 0; count <= result_language.Length - 1; count++)
            {
                if (result_language[count].Type == "VIEW")
                {
                    if (content_data.LanguageId == result_language[count].Id)
                    {
                        strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + " selected>" + result_language[count].Name + "</option>";
                    }
                    else
                    {
                        strViewDisplay = strViewDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
                    }
                }
            }
            if (strViewDisplay != "")
            {
				result.Append(StyleHelper.ActionBarDivider);
                result.Append("<td class=\"label\">" + m_refMsg.GetMessage("view language") + "</td>");
                result.Append("<td>");
                result.Append("<select id=viewcontent name=viewcontent OnChange=\"javascript:LoadContent(\'frmContent\',\'VIEW\');\">");
                result.Append(strViewDisplay);
                result.Append("</select>");
                result.Append("</td>");
            }
            if (security_data.CanAdd)
            {
                //If (bCanAddNewLanguage) Then
                for (count = 0; count <= result_language.Length - 1; count++)
                {
                    if (result_language[count].Type == "ADD")
                    {
                        strAddDisplay = strAddDisplay + "<option value=" + result_language[count].Id + ">" + result_language[count].Name + "</option>";
                    }
                }
                if (strAddDisplay != "")
                {
                    result.Append("<td>&nbsp;&nbsp;</td>");
                    result.Append("<td class=\"label\">" + m_refMsg.GetMessage("add title") + ":</td>");
                    result.Append("<td>");
                    if (folder_data == null)
                    {
                        folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
                    }
                    if (Utilities.IsNonFormattedContentAllowed(m_refContentApi.GetEnabledXmlConfigsByFolder(this.folder_data.Id)))
                    {
                        allowHtml = "&AllowHtml=1";
                    }
                    result.Append("<select id=addcontent name=addcontent OnChange=\"javascript:LoadContent(\'frmContent\',\'ADD\');\">");
                    result.Append("<option value=" + "0" + ">" + "-select language-" + "</option>");
                    result.Append(strAddDisplay);
                    result.Append("</select></td>");
                }
                //End If
            }
        }

		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        if (m_strPageAction == "view")
        {
            result.Append(m_refStyle.GetHelpButton("Viewcontent", ""));
        }
        else if (m_strPageAction == "viewstaged")
        {
            result.Append(m_refStyle.GetHelpButton("Viewstaged", ""));
        }
		result.Append("</td>");
		result.Append("</tr>");
        result.Append("</table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    protected void TaskDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (!(e.Item.Cells[4].Text.Equals("[Not Specified]")))
                {
                    e.Item.Cells[4].CssClass = "important";
                }
                e.Item.Attributes.Add("id", arrTaskTypeID[intCount]);
                intCount++;
                break;
        }
    }

    private void ViewSubscriptions()
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
        System.Text.StringBuilder sbOutput = new System.Text.StringBuilder();
        SettingsData settings_list;

        intInheritFrom = m_refContentApi.GetFolderInheritedFrom(m_intFolderId);

        subscription_data_list = m_refContentApi.GetSubscriptionsForFolder(intInheritFrom); //AGofPA get subs for folder; set break inheritance flag false
        subscription_properties_list = m_refContentApi.GetSubscriptionPropertiesForContent(m_intId); //first try content
        if (subscription_properties_list == null)
        {
            subscription_properties_list = m_refContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom); //then get folder
            subscribed_data_list = subscription_data_list; // get subs for folder
        }
        else //content is populated.
        {
            subscribed_data_list = m_refContentApi.GetSubscriptionsForContent(m_intId); // get subs for folder
        }

        emailfrom_list = m_refContentApi.GetAllEmailFrom();
        defaultmessage_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage);
        unsubscribe_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe);
        optout_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut);
        settings_list = (new SiteAPI()).GetSiteVariables(-1);


        if ((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (subscription_data_list == null) || (settings_list.AsynchronousLocation == ""))
        {
            tdsubscriptiontext.Text += "<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">";
            tdsubscriptiontext.Text += "<br/>" + m_refMsg.GetMessage("lbl web alert settings") + ":<br/><br/>" + m_refMsg.GetMessage("lbl web alert not setup") + "<br/>";
            if (emailfrom_list == null)
            {
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("lbl web alert emailfrom not setup") + "</font>";
            }
            if (defaultmessage_list == null)
            {
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("lbl web alert def msg not setup") + "</font>";
            }
            if (unsubscribe_list == null)
            {
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("lbl web alert unsub not setup") + "</font>";
            }
            if (optout_list == null)
            {
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("lbl web alert optout not setup") + "</font>";
            }
            if (subscription_data_list == null)
            {
                phWebAlerts.Visible = false;
                phWebAlerts2.Visible = false;
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("alt No subscriptions are enabled on the folder.") + "</font>";
            }
            if (settings_list.AsynchronousLocation == "")
            {
                tdsubscriptiontext.Text += "<br/><font color=\"red\">" + m_refMsg.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") + "</font>";
            }
            return;
        }

        if (subscription_properties_list == null)
        {
            subscription_properties_list = new SubscriptionPropertiesData();
        }

        strEnabled = " disabled=\"true\" ";

        switch (subscription_properties_list.NotificationType.GetHashCode())
        {
            case 0:
                strNotifyA = " CHECKED=\"true\" ";
                strNotifyI = "";
                strNotifyN = "";
                break;
            case 1:
                strNotifyA = "";
                strNotifyI = " CHECKED=\"true\" ";
                strNotifyN = "";
                break;
            case 2:
                strNotifyA = "";
                strNotifyI = "";
                strNotifyN = " CHECKED=\"true\" ";
                break;
        }
        //always break inheritance because its content
        tdsubscriptiontext.Text += "<input id=\"break_sub_inherit_button\" type=\"hidden\" name=\"break_sub_inherit_button\" value=\"break_sub_inherit_button\">";

        tdsubscriptiontext.Text += "<table class=\"ektronGrid\">";
        tdsubscriptiontext.Text += "    <tr>";
        tdsubscriptiontext.Text += "        <td class=\"label\">";
        tdsubscriptiontext.Text += "            " + m_refMsg.GetMessage("lbl web alert opt") + ":";
        tdsubscriptiontext.Text += "        </td>";
        tdsubscriptiontext.Text += "        <td class=\"value\">";
        tdsubscriptiontext.Text += "            <input type=\"radio\" value=\"Always\" name=\"notify_option\" " + strNotifyA + " " + strEnabled + "> " + m_refMsg.GetMessage("lbl web alert notify always") + "<br />";
        tdsubscriptiontext.Text += "            <input type=\"radio\" value=\"Initial\" name=\"notify_option\"" + strNotifyI + " " + strEnabled + "> " + m_refMsg.GetMessage("lbl web alert notify initial") + "<br />";
        tdsubscriptiontext.Text += "            <input type=\"radio\" value=\"Never\" name=\"notify_option\"" + strNotifyN + " " + strEnabled + "> " + m_refMsg.GetMessage("lbl web alert notify never");
        tdsubscriptiontext.Text += "        </td>";
        tdsubscriptiontext.Text += "    </tr>";

        tdsubscriptiontext.Text += "    <tr>";
        tdsubscriptiontext.Text += "        <td class=\"label\">";
        tdsubscriptiontext.Text += "            " + m_refMsg.GetMessage("lbl web alert subject") + ":";
        tdsubscriptiontext.Text += "        </td>";

        tdsubscriptiontext.Text += "        <td class=\"value\">";
        if (subscription_properties_list.Subject != "")
        {
            tdsubscriptiontext.Text += "        <input type=\"text\" maxlength=\"255\" size=\"65\" value=\"" + subscription_properties_list.Subject + "\" name=\"notify_subject\" " + strEnabled + "/>";
        }
        else
        {
            tdsubscriptiontext.Text += "        <input type=\"text\" maxlength=\"255\" size=\"65\" value=\"\" name=\"notify_subject\" " + strEnabled + "/>";
        }
        tdsubscriptiontext.Text += "";

        tdsubscriptiontext.Text += "        </td>";
        tdsubscriptiontext.Text += "    </tr>";

        tdsubscriptiontext.Text += "    <tr>";
        tdsubscriptiontext.Text += "        <td class=\"label\">";
        tdsubscriptiontext.Text += "            " + m_refMsg.GetMessage("lbl web alert emailfrom address") + ":";
        tdsubscriptiontext.Text += "        </td>";
        tdsubscriptiontext.Text += "        <td class=\"value\">";
        tdsubscriptiontext.Text += "            <select name=\"notify_emailfrom\" " + strEnabled + ">:";

        if ((emailfrom_list != null) && emailfrom_list.Length > 0)
        {
            for (y = 0; y <= emailfrom_list.Length - 1; y++)
            {
                if (emailfrom_list[y].Email == subscription_properties_list.EmailFrom)
                {
                    tdsubscriptiontext.Text += "<option value=\"" + emailfrom_list[y].Id + "\" SELECTED>" + emailfrom_list[y].Email + "</option>";
                }
                else
                {
                    tdsubscriptiontext.Text += "<option value=\"" + emailfrom_list[y].Id + "\">" + emailfrom_list[y].Email + "</option>";
                }
            }
        }
        tdsubscriptiontext.Text += "            </select>";
        tdsubscriptiontext.Text += "";
        tdsubscriptiontext.Text += "        </td>";
        tdsubscriptiontext.Text += "    </tr>";

        tdsubscriptiontext.Text += "    <tr>";
        tdsubscriptiontext.Text += "        <td class=\"label\">";
        tdsubscriptiontext.Text += "            " + m_refMsg.GetMessage("lbl web alert contents") + ":";
        tdsubscriptiontext.Text += "        </td>";

        tdsubscriptiontext.Text += "        <td class=\"value\">";
        tdsubscriptiontext.Text += "           <input id=\"use_optout_button\" type=\"checkbox\" checked=\"true\" name=\"use_optout_button\" disabled=\"true\">" + m_refMsg.GetMessage("lbl optout message");

        tdsubscriptiontext.Text += "            &nbsp;<select " + strEnabled + " name=\"notify_optoutid\">";

        if ((optout_list != null) && optout_list.Length > 0)
        {
            for (y = 0; y <= optout_list.Length - 1; y++)
            {
                if (optout_list[y].Id == subscription_properties_list.OptOutID)
                {
                    tdsubscriptiontext.Text += "<option value=\"" + optout_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>";
                }
                else
                {
                    tdsubscriptiontext.Text += "<option value=\"" + optout_list[y].Id + "\">" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>";
                }
            }
        }
        tdsubscriptiontext.Text += "            </select><br />";

        if (subscription_properties_list.DefaultMessageID > 0)
        {
            tdsubscriptiontext.Text += "       <input id=\"use_message_button\" type=\"checkbox\" checked=\"true\" name=\"use_message_button\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use default message");
        }
        else
        {
            tdsubscriptiontext.Text += "       <input id=\"use_message_button\" type=\"checkbox\" name=\"use_message_button\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use default message");
        }
        tdsubscriptiontext.Text += "            &nbsp;<select " + strEnabled + " name=\"notify_messageid\">";

        if ((defaultmessage_list != null) && defaultmessage_list.Length > 0)
        {
            for (y = 0; y <= defaultmessage_list.Length - 1; y++)
            {
                if (defaultmessage_list[y].Id == subscription_properties_list.DefaultMessageID)
                {
                    tdsubscriptiontext.Text += "<option value=\"" + defaultmessage_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>";
                }
                else
                {
                    tdsubscriptiontext.Text += "<option value=\"" + defaultmessage_list[y].Id + "\">" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>";
                }
            }
        }
        tdsubscriptiontext.Text += "            </select><br />";

        if (subscription_properties_list.SummaryID > 0)
        {
            tdsubscriptiontext.Text += "       <input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" checked=\"true\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use summary message") + "<br />";
        }
        else
        {
            tdsubscriptiontext.Text += "       <input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use summary message") + "<br />";
        }
        if (subscription_properties_list.ContentID == -1)
        {
            tdsubscriptiontext.Text += "       <input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"true\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use content message");
            tdsubscriptiontext.Text += "        &nbsp;";
            tdsubscriptiontext.Text += "        <input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"" + subscription_properties_list.ContentID.ToString() + "\"/><input type=\"hidden\" name=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" value=\"[[use current]]\" " + strEnabled + " size=\"65\"/><br/>";
        }
        else if (subscription_properties_list.ContentID > 0)
        {
            tdsubscriptiontext.Text += "       <input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"true\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use content message");
            tdsubscriptiontext.Text += "        &nbsp;";
            tdsubscriptiontext.Text += "        <input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"" + subscription_properties_list.ContentID.ToString() + "\"/><input type=\"hidden\" name=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" value=\"" + subscription_properties_list.UseContentTitle.ToString() + "\" " + strEnabled + " size=\"65\"/><br/><br/>";
        }
        else
        {
            tdsubscriptiontext.Text += "       <input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" " + strEnabled + ">" + m_refMsg.GetMessage("lbl use content message");
            tdsubscriptiontext.Text += "        &nbsp;";
            tdsubscriptiontext.Text += "        <input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" value=\"0\" /><input type=\"hidden\" name=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" onkeydown=\"return false\" value=\"\" " + strEnabled + " size=\"65\"/><br/>";
        }
        if (subscription_properties_list.UseContentLink > 0)
        {
            tdsubscriptiontext.Text += "       <input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" checked=\"true\" " + strEnabled + ">Use Content Link<br />";
        }
        else
        {
            tdsubscriptiontext.Text += "       <input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" " + strEnabled + ">Use Content Link<br />";
        }
        tdsubscriptiontext.Text += "           <input id=\"use_unsubscribe_button\" type=\"checkbox\" checked=\"true\" name=\"use_unsubscribe_button\" disabled=\"true\">" + m_refMsg.GetMessage("lbl unsubscribe message");
        tdsubscriptiontext.Text += "            &nbsp;<select " + strEnabled + " name=\"notify_unsubscribeid\">";

        if ((unsubscribe_list != null) && unsubscribe_list.Length > 0)
        {
            for (y = 0; y <= unsubscribe_list.Length - 1; y++)
            {
                if (unsubscribe_list[y].Id == subscription_properties_list.UnsubscribeID)
                {
                    tdsubscriptiontext.Text += "<option value=\"" + unsubscribe_list[y].Id + "\" SELECTED>" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>";
                }
                else
                {
                    tdsubscriptiontext.Text += "<option value=\"" + unsubscribe_list[y].Id + "\">" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>";
                }
            }
        }
        tdsubscriptiontext.Text += "            </select><br />";
        tdsubscriptiontext.Text += "            </td>";
        tdsubscriptiontext.Text += "         </tr>";
        tdsubscriptiontext.Text += "     </table>";

        tdsubscriptiontext.Text += "<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl avail web alert") + "</div>";
        tdsubscriptiontext.Text += "<table class=\"ektronGrid\" cellspacing=\"1\" id=\"cfld_subscription_assignment\" id=\"cfld_folder_assignment\">";

        if (!(subscription_data_list == null))
        {
            tdsubscriptiontext.Text += "<tr class=\"title-header\"><td>" + m_refMsg.GetMessage("lbl assigned") + "</td><td align=\"left\">" + m_refMsg.GetMessage("lbl name") + "</td></tr>";
            if (!(subscribed_data_list == null))
            {
                arrSubscribed = Array.CreateInstance(typeof(long), subscribed_data_list.Length);
                for (i = 0; i <= subscribed_data_list.Length - 1; i++)
                {
                    arrSubscribed.SetValue(subscribed_data_list[i].Id, i);
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
            for (i = 0; i <= subscription_data_list.Length - 1; i++)
            {
                findindex = -1;
                if ((subscribed_data_list != null) && (arrSubscribed != null))
                {
                    findindex = Array.BinarySearch(arrSubscribed, subscription_data_list[i].Id);
                }
                tdsubscriptiontext.Text += "<tr>";
                if (findindex < 0)
                {
                    tdsubscriptiontext.Text += "<td nowrap=\"true\" align=\"center\"><input type=\"checkbox\" name=\"Assigned_" + subscription_data_list[i].Id + "\"  id=\"Assigned_" + subscription_data_list[i].Id + "\" " + strEnabled + "></td></td>";
                }
                else
                {
                    tdsubscriptiontext.Text += "<td nowrap=\"true\" align=\"center\"><input type=\"checkbox\" name=\"Assigned_" + subscription_data_list[i].Id + "\"  id=\"Assigned_" + subscription_data_list[i].Id + "\" checked=\"true\" " + strEnabled + "></td></td>";
                }
                tdsubscriptiontext.Text += "<td nowrap=\"true\" align=\"Left\">" + subscription_data_list[i].Name + "</td>";
                tdsubscriptiontext.Text += "</tr>";
            }
        }
        else
        {
            tdsubscriptiontext.Text += "<tr><td>Nothing available.</td></tr>";
        }
        tdsubscriptiontext.Text += "</table><input type=\"hidden\" name=\"content_sub_assignments\" value=\"\">";
    }

    private bool IsImage(string fileName)
    {
        string[] imageArray = new string[] { ".gif", ".jpeg", ".dib", ".jpg", ".bmp", ".tiff", ".tif", ".png", ".jpe", "jfif" };
        string extension;
        if (fileName != "")
        {
            extension = System.IO.Path.GetExtension(fileName);
            foreach (string ext in imageArray)
            {
                if (extension.ToLower() == ext)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Util_CheckIsCurrentApprover(long userId)
    {

        if (approvaldata == null)
        {
            approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId);
        }

        if ((approvaldata != null) && approvaldata.Length > 0)
        {

            IsLastApproval = System.Convert.ToBoolean(approvaldata[approvaldata.Length - 1].IsCurrentApprover && (approvaldata[approvaldata.Length - 1].UserId == CurrentUserId || new UserAPI().IsAGroupMember(CurrentUserId, approvaldata[approvaldata.Length - 1].GroupId)));

            if (IsLastApproval)
            {

                IsCurrentApproval = true;

            }
            else
            {

                for (int i = 0; i <= (approvaldata.Length - 1); i++)
                {

                    if (approvaldata[i].IsCurrentApprover)
                    {

                        IsCurrentApproval = System.Convert.ToBoolean(approvaldata[i].UserId == CurrentUserId || new UserAPI().IsAGroupMember(CurrentUserId, approvaldata[i].GroupId));

                    }

                }

            }

        }

    }

    private void Util_ReloadTree(string folderPath, long reloadFolderId)
    {
        Utilities.ReloadTree(
            this.Parent.Parent.Parent.Page,
            new Utilities.WorkareaTree[] {
                Utilities.WorkareaTree.Content
            }, 
            folderPath, 
            0
            );
    }


    #endregion

    #region CSS, JS, Images

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

    }

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);

    }

    #endregion

}
	

