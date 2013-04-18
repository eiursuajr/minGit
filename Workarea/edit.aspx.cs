using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Profile;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Ektron;
using Ektron.ASM.AssetConfig;
using Ektron.Cms;
using Ektron.Cms.BusinessObjects.Localization;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Localization;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

public partial class edit : System.Web.UI.Page
{
    #region Member Variables - Private

    private EkContent m_refContent;
    private EkTask m_refTask;
    private Ektron.Cms.Site.EkSite m_refSite;
    private SiteAPI m_refSiteApi;
    private StyleHelper m_refStyle = new StyleHelper();
    private string FormAction = ""; //Reset the form action
    private string BrowserCode = "en";
    private int EnableMultilingual = 0;
    private string Appname = "";
    private bool IsBrowserIE = false;
    private string m_strPageAction = "";
    private string m_strType = "";
    private Collection pagedata;
    private Collection page_content_data;
    private Collection page_meta_data;
    private PermissionData security_data;

    private string strMyCollection = "";
    private string strAddToCollectionType = "";
    private bool m_bClose = true;
    private long back_folder_id = 0;
    private long back_id = 0;
    private string back_file = "";
    private string back_action = "";
    private long back_form_id = 0;
    private int back_LangType = 0;
    private string back_callerpage = "";
    private string back_origurl = "";
    private int m_intContentType = 1;
    private bool bVer4Editor = false;
    private string m_strManualAlias = "";
    private long m_intManualAliasId = 0;
    private string m_strManualAliasExt = "";
    private ContentEditData content_edit_data;
    private ContentData content_data;
    private FolderData folder_data;
    private SettingsData settings_data;

    private string AppLocaleString = "";
    private string m_strSchemaFile = "";
    private string m_strNamespaceFile = "";
    private XmlConfigData xmlconfig_data;
    private string m_strContentTitle = "";
    private string m_strContentHtml = "";
    private string content_teaser = "";
    private string content_comment = "";
    private string content_stylesheet = "";
    protected long m_intContentFolder = 0;

    private string xml_config = "";
    private string save_xslt_file = "";
    private string editorPackage = "";
    private bool MetaComplete = false;
    private long m_refContentId = 0;
    private long m_intXmlConfigId = -1;
    private int iSegment = 0;
    private int iSegment2 = 0;
    private bool bIsFormDesign = false;
    private int iMaxContLength = 0;
    private int iMaxSummLength = 0;
    private PermissionData UserRights;
    private string PreviousState = "";
    private ContentMetaData[] meta_data;

    private bool ret;
    private int eWebEditProPromptOnUnload = 0; // To Do this should be a 1, but editor needs to be fixed
    protected string var2 = "";
    private string szdavfolder = "";

    //Variables used in load page for the editor
    private Hashtable endDateActionSel;
    private int endDateActionSize = 0;
    private bool UploadPrivs = false;
    private string go_live = "";
    private string end_date = "";
    private string end_date_action = "";
    private int MetaDataNumber = 0;
    private string path = "";
    private string urlxml = "";
    private LanguageData language_data;
    private string ImagePath = "";
    private string AppPath = "";
    private string SitePath = "";
    private long CurrentUserID = 0;
    private string AppeWebPath = "";
    private SubscriptionData[] subscription_data_list;
    private SubscriptionData[] subscribed_data_list;
    private SubscriptionPropertiesData subscription_properties_list;
    private long intInheritFrom = 0;
    private bool blnShowTStatusMessage = false;
    private SubscriptionData[] active_subscription_list;
    private bool blnUndoCheckOut_complete;
    private string m_sSelectedDivStyleClass = "selected_editor";
    private string m_sUnSelectedDivStyleClass = "unselected_editor";
    private bool m_bIsBlog = false;
    private BlogData blog_data;
    private BlogPostData blog_post_data;
    private bool bNewPoll = false;
    private bool bReNewPoll = false;
    private int nPollChoices = 8;
    private string myMeta;
    private string m_SelectedEditControl;
    private ContentDesignerWithValidator m_ctlContentDesigner;
    private ContentDesignerWithValidator m_ctlSummaryDesigner;
    private ContentDesignerWithValidator m_ctlFormResponseRedirect;
    private ContentDesignerWithValidator m_ctlFormResponseTransfer;
    private HtmlImage m_ctlFormSummaryReport;
    private HtmlGenericControl m_ctlContentPane;
    private HtmlGenericControl m_ctlSummaryPane;
    private HtmlGenericControl m_ctlSummaryStandard;
    private HtmlGenericControl m_ctlSummaryRedirect;
    private HtmlGenericControl m_ctlSummaryTransfer;
    private HtmlGenericControl m_ctlSummaryReport;
    private RegularExpressionValidator m_ctlContentValidator;
    private RegularExpressionValidator m_ctlSummaryValidator;
    //Set of variables added for 7.6 Aliasing
    private string m_prevManualAliasName = "";
    private string m_currManualAliasStatus = "";
    private string m_currManualAliasName = "";
    private string m_prevManualAliasExt = "";
    private string m_currManualAliasExt = "";
    private Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
    private string controlName = string.Empty;
    private UnitType editorWidthUnitType = UnitType.Percentage;
    private int editorWidthUnits = 100;
    private string DMSCookieName = "DMS_Office_ver";
    private bool metadataRequired = false;
    #endregion

    #region Member Variables - Protected

    protected string m_strAssetFileName = "";
    protected string TaxonomyTreeIdList = "";
    protected string TaxonomyTreeParentIdList = "";
    protected long m_intTaxFolderId = 0;
    protected string updateFieldId = "";
    protected string commparams = "";
    protected bool TaxonomyRoleExists = false;
    protected bool IsAdmin = false;
    protected long TaxonomyOverrideId = 0;
    protected long TaxonomySelectId = 0;
    protected bool DisplayTab = true;
    protected ContentAPI m_refContApi;
    protected EkMessageHelper m_refMsg;
    protected string DIRECTION = "";
    protected string AppImgPath = "";
    protected bool IsMac = false;
    protected int m_intContentLanguage = 0;
    protected long m_intItemId = 0;
    protected long m_intFolderId = 0;
    protected int lContentType;
    protected EkEnumeration.CMSContentSubtype lContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content;
    protected int g_ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes;
    protected Hashtable asset_info = new Hashtable();
    #endregion

    #region Events


    public edit()
    {

        m_refContApi = new ContentAPI();
        m_ctlContentPane = new HtmlGenericControl();
        m_ctlSummaryPane = new HtmlGenericControl();
        m_ctlSummaryStandard = new HtmlGenericControl();
        m_ctlSummaryRedirect = new HtmlGenericControl();
        m_ctlSummaryTransfer = new HtmlGenericControl();
        m_ctlSummaryReport = new HtmlGenericControl();
        m_ctlFormSummaryReport = new HtmlImage();

    }


    protected void Page_Init(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["EkDavSessionVal"] = null;
            Session["EkDavSessionASDList"] = new List<NameValueCollection>();
        }

        AppPath = m_refContApi.AppPath;
        m_SelectedEditControl = Utilities.GetEditorPreference(Request);
		m_refMsg = m_refContApi.EkMsgRef;
		Utilities.ValidateUserLogin();
        if (m_refContApi.RequestInformationRef.IsMembershipUser == 1 || m_refContApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(m_refContApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return;
        }

        if (System.Configuration.ConfigurationSettings.AppSettings["ek_EditorWidthUnits"] != null)
        {
            editorWidthUnitType = (UnitType)Enum.Parse(typeof(UnitType), System.Configuration.ConfigurationSettings.AppSettings["ek_EditorWidthUnits"]);
        }
        if (System.Configuration.ConfigurationSettings.AppSettings["ek_EditorWidth"] != null)
        {
            int.TryParse(System.Configuration.ConfigurationSettings.AppSettings["ek_EditorWidth"].ToString(), out editorWidthUnits);
        }
        //Register Page Components
        this.RegisterCSS();
        this.RegisterJS();

        phEditContent.Controls.Add(m_ctlContentPane);
        m_ctlContentPane.TagName = "div";
        m_ctlContentPane.ID = "dvContent";

        phEditSummary.Controls.Add(m_ctlSummaryPane);
        m_ctlSummaryPane.TagName = "div";
        m_ctlSummaryPane.ID = "dvSummary";

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryStandard);
        m_ctlSummaryStandard.TagName = "div";
        m_ctlSummaryStandard.ID = "_dvSummaryStandard";

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryRedirect);
        m_ctlSummaryRedirect.TagName = "div";
        m_ctlSummaryRedirect.ID = "_dvSummaryRedirect";

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryTransfer);
        m_ctlSummaryTransfer.TagName = "div";
        m_ctlSummaryTransfer.ID = "_dvSummaryTransfer";

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryReport);
        m_ctlSummaryReport.TagName = "div";
        m_ctlSummaryReport.ID = "_dvSummaryReport";

        // The ContentDesigner controls need to be created in the Page_Init event so the PostData
        // will be bound to them. However, they may not be displayed, so default .Visible=False.
        m_ctlContentDesigner = (ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
        m_ctlContentPane.Controls.Add(m_ctlContentDesigner);
        m_ctlContentDesigner.Visible = false;
        m_ctlContentDesigner.ID = "content_html";

        m_ctlSummaryDesigner = (ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
        m_ctlSummaryStandard.Controls.Add(m_ctlSummaryDesigner);
        m_ctlSummaryDesigner.Visible = false;
        m_ctlSummaryDesigner.ID = "content_teaser";


        m_ctlFormResponseRedirect = (ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
        m_ctlSummaryRedirect.Controls.Add(m_ctlFormResponseRedirect);
        m_ctlFormResponseRedirect.Visible = false;
        m_ctlFormResponseRedirect.ID = "forms_redirect";


        m_ctlFormResponseTransfer = (ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
        m_ctlSummaryTransfer.Controls.Add(m_ctlFormResponseTransfer);
        m_ctlFormResponseTransfer.Visible = false;
        m_ctlFormResponseTransfer.ID = "forms_transfer";

        m_ctlSummaryPane.Controls.Add(m_ctlFormSummaryReport);
        m_ctlFormSummaryReport.Visible = false;
        m_ctlFormSummaryReport.Src = "images/application/charttypes.gif";
        m_ctlFormSummaryReport.ID = "_imgFormSummaryReport";

        m_ctlContentValidator = ContentValidator; // New RegularExpressionValidator
        m_ctlContentValidator.Visible = false;
        m_ctlContentValidator.ControlToValidate = (string)m_ctlContentDesigner.ID;
        m_ctlContentValidator.EnableClientScript = true;

        m_ctlSummaryValidator = SummaryValidator; // New RegularExpressionValidator
        m_ctlSummaryValidator.Visible = false;
        m_ctlSummaryValidator.ControlToValidate = (string)m_ctlSummaryDesigner.ID;
        m_ctlSummaryValidator.EnableClientScript = true;
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        bool bAddingNew = false;
        string tempStr;
        string referrerStr;
        try
        {
            //INITIALIZE THE VARIABLES
            if (Request.Browser.Type.IndexOf("IE") != -1)
            {
                IsBrowserIE = true;
            }
            jsIsMac.Text = "false";
            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                IsMac = true;
            }
            // Ensure that this is not a browser refresh (Mac-Safari bug causes
            // the editor to load after publishing, if the browser is refreshing):
            if (IsMac && !IsBrowserIE)
            {
                referrerStr = Request.Url.LocalPath;
                if (referrerStr != null)
                {
                    tempStr = referrerStr.Substring(referrerStr.LastIndexOf("/"));
                    if (tempStr == "/workarea.aspx")
                    {
                        tempStr = referrerStr.Replace(tempStr, "/dashboard.aspx");
                        Response.Redirect(tempStr, false);
                        return;
                    }
                }
            }

            if (m_SelectedEditControl != "ContentDesigner")
            {
                m_ctlContentPane.Controls.Remove(m_ctlContentDesigner);
                m_ctlSummaryStandard.Controls.Remove(m_ctlSummaryDesigner);
                m_ctlSummaryRedirect.Controls.Remove(m_ctlFormResponseRedirect);
                m_ctlSummaryTransfer.Controls.Remove(m_ctlFormResponseTransfer);
            }

            Response.Expires = -1;
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("cache-control", "no-store");


            //THE NEXT THREE LINES MUST BE REMOVED BEFORE THE RELEASE
            if (Request.ServerVariables["Query_String"] == "")
            {
                return;
            }

            if (IsMac)
            {
                jsIsMac.Text = "true";
            }

            // Note: To fix a problem with the Ephox Editors on the
            // Mac-running-Safari (assumed if "IsMac and not IsBrowserIE")
            // we need to use different styles for the DIV-tags holding
            // the editors, etc., otherwise they frequently draw themselves
            // when they should remain hidden. These values cause problems
            // with the PC/Win/IE combination, (the summary editor fails to
            // provide a client area for the user to view/edit) so they cannot
            // cannot be used everywhere, hence our use of alternate style classes:
            // Pass class names to javascript:
            jsSelectedDivStyleClass.Text = m_sSelectedDivStyleClass;
            jsUnSelectedDivStyleClass.Text = m_sUnSelectedDivStyleClass;

            m_refContApi = new ContentAPI();
            m_refSiteApi = new SiteAPI();
            m_refContent = m_refContApi.EkContentRef;
            m_refSite = m_refContApi.EkSiteRef;
            m_refTask = m_refContApi.EkTaskRef;

            CurrentUserID = m_refContApi.UserId;
            AppImgPath = m_refContApi.AppImgPath;
            SitePath = m_refContApi.SitePath;
            Appname = m_refContApi.AppName;
            AppeWebPath = m_refContApi.ApplicationPath + m_refContApi.AppeWebPath;
            AppPath = m_refContApi.AppPath;
            EnableMultilingual = m_refContApi.EnableMultilingual;
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            EnhancedMetadataScript.Text = CustomFields.GetEnhancedMetadataScript();
            EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea();
            lbl_GenericTitleLabel.Text = m_refMsg.GetMessage("generic title label");

            if (!(Request.QueryString["id"] == null))
            {
                m_intItemId = Convert.ToInt64(Request.QueryString["id"]);
                m_intTaxFolderId = m_intItemId;
            }
            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
                }
                else
                {
                    if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                }
            }
            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || m_intContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
            {
                m_intContentLanguage = m_refContApi.DefaultContentLanguage;
            }
            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContApi.ContentLanguage = m_intContentLanguage;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["folder_id"]))
            {
                m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
            }

            if (Request.QueryString["form_type"] != null)
            {
                bNewPoll = System.Convert.ToBoolean("poll" == Convert.ToString(Request.QueryString["form_type"]).Trim().ToLower());
            }
            if (Request.QueryString["new"] != null)
            {
                bAddingNew = System.Convert.ToBoolean("true" == Convert.ToString(Request.QueryString["new"]).Trim().ToLower());
            }
            if (Request.QueryString["poll"] != null)
            {
                bReNewPoll = System.Convert.ToBoolean("renew" == Convert.ToString(Request.QueryString["poll"]).Trim().ToLower());
            }
            if (Request.Form["editaction"] != null)
            {
                m_strPageAction = Convert.ToString(Request.Form["editaction"]).ToLower().Trim();
            }
            if (Request.QueryString["translate"] != null)
            {
                translate.Value = "true";
            }
            if (Request.QueryString["type"] != null)
            {
                m_strType = Convert.ToString(Request.QueryString["type"]).ToLower().Trim();
            }
            else if (Request.Form["eType"] != null)
            {
                m_strType = Convert.ToString(Request.Form["eType"]).ToLower().Trim();
            }
            if (!String.IsNullOrEmpty(Request.QueryString["ctlupdateid"]))
            {
                commparams = (string)("&ctlupdateid=" + Request.QueryString["ctlupdateid"] + "&ctlmarkup=" + Request.QueryString["ctlmarkup"] + "&cltid=" + Request.QueryString["cltid"] + "&ctltype=" + Request.QueryString["ctltype"]);
                updateFieldId = Request.QueryString["ctlupdateid"];
                Page.ClientScript.RegisterHiddenField("ctlupdateid", updateFieldId);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["cacheidentifier"]))
            {
                Page.ClientScript.RegisterHiddenField("cacheidentifier", Request.QueryString["cacheidentifier"]);
            }
            else
            {
                if ((Request.QueryString["mycollection"] != null) && (Request.QueryString["addto"] != null) && (Request.QueryString["type"] != null))
                {
                    if (Request.QueryString["type"] == "add" && Request.QueryString["addto"] == "menu")
                    {
                        Page.ClientScript.RegisterHiddenField("cacheidentifier", "menu_" + Request.QueryString["mycollection"] + m_intContentLanguage + "_mnu");
                    }
                }
            }

            //destination.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "processMultiupload.aspx";
            //PostURL.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "processMultiupload.aspx";
            //NextUsing.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "content.aspx";

            if (Request.Cookies[DMSCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[DMSCookieName].Value))
            {
               
                if (Request.Cookies[DMSCookieName].Value == "2010")
                {
                    Ektron.Cms.Controls.ExplorerDragDrop edd = new Ektron.Cms.Controls.ExplorerDragDrop();
                    edd.ContentLanguage = this.m_intContentLanguage;
                    if (!string.IsNullOrEmpty(Request.QueryString["folderid"]))
                    {
                        destination.Value = edd.GetFolderPath(Int64.Parse(Request.QueryString["folderid"])).Replace(Page.Request.Url.GetLeftPart(UriPartial.Authority), "");
                        putopts.Value = "false";
                    }
                    //btnMUpload.OnClientClick = "return MultipleDocumentUpload(0);";
                    //lbtn_toggleVersion.Attributes.Add("onclick", string.Format(_messageHelper.GetMessage("js office version toggle confirm format"), _messageHelper.GetMessage("li text office 2010 name")));
                }
                else
                {
                    destination.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "processMultiupload.aspx";
                    PostURL.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "processMultiupload.aspx";
                    NextUsing.Value = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + AppPath + "content.aspx";
                    putopts.Value = "true";
                    //btnMUpload.OnClientClick = "return MultipleDocumentUpload(1);";
                    //lbtn_toggleVersion.Attributes.Add("onclick", string.Format(_messageHelper.GetMessage("js office version toggle confirm format"), _messageHelper.GetMessage("li text other office ver name")));

                }
                //tabMultipleDMS.Controls.Add(linebreak);
            }

            if (!String.IsNullOrEmpty(Request.QueryString["ctlmarkup"]))
            {
                Page.ClientScript.RegisterHiddenField("ctlmarkup", Request.QueryString["ctlmarkup"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["ctltype"]))
            {
                Page.ClientScript.RegisterHiddenField("ctltype", Request.QueryString["ctltype"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["cltid"]))
            {
                Page.ClientScript.RegisterHiddenField("cltid", Request.QueryString["cltid"]);
            }

            if (m_strType == "update")
            {
                m_refContentId = m_intItemId;
            }
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString["content_id"]))
                {
                    m_refContentId = Convert.ToInt64(Request.QueryString["content_id"]);
                }
            }
            
            if (Request.QueryString["xid"] != null)
            {
                m_intXmlConfigId = Convert.ToInt64(Request.QueryString["xid"]);
            }
            else if (Request.Form["SelectedXid"] != null)
            {
                m_intXmlConfigId = Convert.ToInt64(Request.Form["SelectedXid"]);
            }
            else
            {
                if (Request.QueryString["type"] == "add")
                {
                    if (Request.QueryString["AllowHTML"] != "1")
                    {
                        m_intXmlConfigId = Utilities.GetDefaultXmlConfig(Convert.ToInt64(Request.QueryString["id"]));
                        if (m_intXmlConfigId == 0)
                        {
                            m_intXmlConfigId = -1;
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(Request.QueryString["mycollection"]))
            {
                strMyCollection = Request.QueryString["mycollection"];
            }
            else if (!String.IsNullOrEmpty(Request.Form["mycollection"]))
            {
                strMyCollection = Request.Form["mycollection"];
            }
            if (!String.IsNullOrEmpty(Request.QueryString["addto"]))
            {
                strAddToCollectionType = Request.QueryString["addto"];
            }
            else if (!String.IsNullOrEmpty(Request.Form["addto"]))
            {
                strAddToCollectionType = Request.Form["addto"];
            }
            if (Request.QueryString["close"] == "false")
            {
                m_bClose = false;
            }
            if (Request.QueryString["back_folder_id"] != null)
            {
                back_folder_id = Convert.ToInt64(Request.QueryString["back_folder_id"]);
                m_intTaxFolderId = back_folder_id;
            }
            if (Request.QueryString["back_id"] != null)
            {
                back_id = Convert.ToInt64(Request.QueryString["back_id"]);
            }
            if (Request.QueryString["back_file"] != null)
            {
                back_file = Request.QueryString["back_file"];
            }
            if (Request.QueryString["back_action"] != null)
            {
                back_action = Request.QueryString["back_action"];
                if (back_action.ToLower() == "viewcontentbycategory" || back_action.ToLower() == "viewarchivecontentbycategory")
                {
                    back_folder_id = back_id;
                }
            }
            if (Request.QueryString["control"] != null)
            {
                controlName = Request.QueryString["control"];
            }
            if (Request.QueryString["buttonid"] != null)
            {
                buttonId.Value = Request.QueryString["buttonid"];
            }
            if (Request.QueryString["back_form_id"] != null)
            {
                back_form_id = Convert.ToInt64(Request.QueryString["back_form_id"]);
            }
            if (Request.QueryString["back_LangType"] != null)
            {
                back_LangType = Convert.ToInt32(Request.QueryString["back_LangType"]);
            }
            else
            {
                back_LangType = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()["DefaultLanguage"]);
            }
            if (Request.QueryString["back_callerpage"] != null)
            {
                back_callerpage = (string)("&back_callerpage=" + Request.QueryString["back_callerpage"]);
            }
            if (Request.QueryString["back_page"] != null)
            {
                back_callerpage = back_callerpage + "&back_page=" + Request.QueryString["back_page"];
            }
            if (Request.QueryString["back_origurl"] != null)
            {
                back_origurl = (string)("&back_origurl=" + EkFunctions.UrlEncode(Request.QueryString["back_origurl"]));
            }
            if (!String.IsNullOrEmpty(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
            {
                if (Ektron.Cms.Common.EkFunctions.IsNumeric(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
                {
                    g_ContentTypeSelected = System.Convert.ToInt32(Request.QueryString[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
                    m_refContApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, g_ContentTypeSelected.ToString());
                }
            }
            else if (Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam] != "")
            {
                if (Ektron.Cms.Common.EkFunctions.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]))
                {
                    g_ContentTypeSelected = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()[Ektron.Cms.Common.EkConstants.ContentTypeUrlParam]);
                }
            }
            if (Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes == g_ContentTypeSelected)
            {
                if (Request.QueryString["multi"] != null)
                {
                    if ("" == Request.QueryString["multi"])
                    {
                        lContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content; //set content type to "content" as default value
                    }
                    else
                    {
                        lContentType = Convert.ToInt32(Request.QueryString["multi"]);
                        if (lContentType == 9876)
                        {
                            lContentType = 103;
                        }
                    }
                }
                else
                {
                    lContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
                }
            }
            else
            {
                lContentType = g_ContentTypeSelected;
                if (lContentType == 9876)
                {
                    lContentType = 103;
                }
            }

            language_data = m_refSiteApi.GetLanguageById(m_intContentLanguage);
            if (this.m_strType.ToLower() == "add" && (!String.IsNullOrEmpty(Request.QueryString["SelTaxonomyId"])))
            {
                TaxonomySelectId = Convert.ToInt64(Request.QueryString["SelTaxonomyId"]);
            }
            SettingsData settings_data;
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

            int UserLocale;
            UserLocale = m_refSiteApi.RequestInformationRef.UserCulture;
            AppLocaleString = GetLocaleFileString(UserLocale.ToString());
            jsMaxLengthMsg.Text = m_refMsg.GetMessage("js err encoded title exceeds max length");
            jsContentLanguage.Text = Convert.ToString((short)m_intContentLanguage);
            jsId.Text = Convert.ToString(m_intItemId);
            jsDefaultContentLanguage.Text = Convert.ToString(m_refContApi.DefaultContentLanguage);
            jsType.Text = Convert.ToString((short)m_intContentType);
            phAlias.Visible = false;
            Page.Title = m_refContApi.AppName + " " + m_refMsg.GetMessage("edit content page title") + " \"" + Ektron.Cms.CommonApi.GetEcmCookie()["username"] + "\"";
            string editaction = "";
            if (Request.Form["editaction"] != null)
            {
                editaction = Request.Form["editaction"];
            }
            if ("workoffline" == editaction || "cancel" == editaction || ("" == Convert.ToString(m_intItemId) && "" == editaction))
            {
                if (m_strType == "update")
                {
                    ret = m_refContent.UndoCheckOutv2_0(Convert.ToInt64(Request.Form["content_id"]));
                    blnUndoCheckOut_complete = true;
                }
                if (!m_bClose)
                {
                    ClosePanel.Text = "<script language=javascript>" + "\r\n" + "ResizeFrame(1); // Show the navigation-tree frame." + "\r\n" + "</script>";
                    Response.Redirect(GetBackPage(Convert.ToInt64(Request.Form["content_id"])), false);
                }
                else
                {
                    Response.Redirect("close.aspx", false);
                }
            }
            else if ((m_strPageAction == "save") || (m_strPageAction == "checkin") || (m_strPageAction == "publish") || (m_strPageAction == "summary_save") || (m_strPageAction == "meta_save"))
            {
                Process_FormSubmit();
                if (m_bClose && m_strPageAction != "save")
                {
                    if (updateFieldId != "")
                    {
                        string strQuery = "";
                        if (TaxonomySelectId > 0)
                        {
                            strQuery = (string)("&__taxonomyid=" + TaxonomySelectId);
                        }
                        else if (TaxonomyOverrideId > 0)
                        {
                            strQuery = (string)("&__taxonomyid=" + TaxonomyOverrideId);
                        }
                        Response.Redirect((string)("close.aspx?toggle=true" + strQuery), false);
                    }
                }
            }
            else
            {
                Display_EditControls();

                if (!(Page.IsPostBack) && bAddingNew)
                {
                    if (Request.QueryString["form_type"] != null)
                    {
                        newformwizard ucNewFormWizard;
                        ucNewFormWizard = (newformwizard)(LoadControl("controls/forms/newformwizard.ascx"));
                        ucNewFormWizard.ID = "ProgressSteps";
                        phNewFormWizard.Controls.Add(ucNewFormWizard);
                        if (bNewPoll)
                        {
                            PollHtmlScript();
                        }
                    }
                }
            }

            PermissionData cPerms;
            cPerms = m_refContApi.LoadPermissions(m_intContentFolder, "folder", 0);
            m_ctlContentDesigner.FolderId = m_intContentFolder;
            if (2 == m_intContentType)
            {
                m_ctlContentDesigner.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Designer;
            }
            else if (editorPackage.Length > 0)
            {
                m_ctlContentDesigner.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.DataEntry;
            }
            else
            {
                m_ctlContentDesigner.ToolsFile = m_refContApi.ApplicationPath + "ContentDesigner/configurations/StandardEdit.aspx?wiki=1";
            }
            m_ctlContentDesigner.SetPermissions(cPerms);
            m_ctlContentDesigner.AllowFonts = true;
            m_ctlSummaryDesigner.FolderId = m_intContentFolder;
            if (2 == m_intContentType)
            {
                m_ctlSummaryDesigner.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.XsltDesigner;
            }
            else if (m_bIsBlog)
            {
                m_ctlSummaryDesigner.ToolsFile = m_refContApi.ApplicationPath + "ContentDesigner/configurations/InterfaceBlog.aspx?WMV=1";
            }
            else
            {
                m_ctlSummaryDesigner.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Standard;
            }
            m_ctlSummaryDesigner.SetPermissions(cPerms);
            m_ctlSummaryDesigner.AllowFonts = true;
            m_ctlFormResponseRedirect.FolderId = m_intContentFolder;
            m_ctlFormResponseRedirect.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.NoToolbars;
            m_ctlFormResponseRedirect.SetPermissions(cPerms);
            m_ctlFormResponseRedirect.AllowFonts = true;
            m_ctlFormResponseTransfer.FolderId = m_intContentFolder;
            m_ctlFormResponseTransfer.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.NoToolbars;
            m_ctlFormResponseTransfer.SetPermissions(cPerms);
            m_ctlFormResponseTransfer.AllowFonts = true;
            m_ctlContentValidator.Text = m_refMsg.GetMessage("content size exceeded");
            m_ctlSummaryValidator.Text = m_refMsg.GetMessage("content size exceeded");
            g_ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes;
            m_refContApi.SetCookieValue(Ektron.Cms.Common.EkConstants.ContentTypeUrlParam, g_ContentTypeSelected.ToString());
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Helpers

    private ContentMetaData[] PopulateMultiLingualMetadata(ContentMetaData[] newLanguageMetaArray)
    {
        if (content_data != null && 
            content_data.MetaData != null && 
            content_data.MetaData.Length > 0 &&
            newLanguageMetaArray != null)
        {
            List<ContentMetaData> originalLanguageMetaList = new List<ContentMetaData>(content_data.MetaData);
            foreach (ContentMetaData contentMeta in newLanguageMetaArray)
            {
                ContentMetaData match = originalLanguageMetaList.Find(x => x.TypeName == contentMeta.TypeName);
                if (match != null)
                    contentMeta.DefaultText = match.Text;
            }
        }
        return newLanguageMetaArray;
    }

    private string GetLocaleFileString(string localeFileNumber)
    {
        string LocaleFileString;
        if (localeFileNumber == "" || int.Parse(localeFileNumber) == 1)
        {
            LocaleFileString = "0000";
        }
        else
        {
            int tempCount = 4 - Conversion.Hex(localeFileNumber).Length;
            if (tempCount > 0)
            {
                LocaleFileString = new string('0', tempCount);
                LocaleFileString = LocaleFileString + Conversion.Hex(localeFileNumber);
                if (!System.IO.File.Exists(Server.MapPath(AppeWebPath + "locale" + LocaleFileString + "b.xml")))
                {
                    LocaleFileString = "0000";
                }
            }
            else
            {
                LocaleFileString = "0000";
            }
        }
        return LocaleFileString.ToString();
    }

    private string GetServerPath()
    {
        string strPath;
        if (Request.ServerVariables["SERVER_PORT_SECURE"] == "1")
        {
            strPath = (string)("https://" + Request.ServerVariables["SERVER_NAME"]);
            if (Request.ServerVariables["SERVER_PORT"] != "443")
            {
                strPath = strPath + ":" + Request.ServerVariables["SERVER_PORT"];
            }
        }
        else
        {
            strPath = (string)("http://" + Request.ServerVariables["SERVER_NAME"]);
            if (Request.ServerVariables["SERVER_PORT"] != "80")
            {
                strPath = strPath + ":" + Request.ServerVariables["SERVER_PORT"];
            }
        }
        return strPath;
    }

    #endregion

    #region DISPLAY EDITOR PAGE
    private void Display_EditControls()
    {
        int intContentLanguage = 1033;
        PermissionData security_lib_data;
        int i = 0;
        bool bEphoxSupport = false;
        string aliasContentType = string.Empty;

        folder_data = null;
        try
        {
            netscape.Value = "";
            language_data = m_refSiteApi.GetLanguageById(m_intContentLanguage);
            ImagePath = language_data.ImagePath;
            BrowserCode = language_data.BrowserCode;
            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                asset_info.Add(Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i], "");
            }
            Page.ClientScript.RegisterHiddenField("TaxonomyOverrideId", Convert.ToString(TaxonomyOverrideId));
            if (IsMac && m_SelectedEditControl != "ContentDesigner" && m_strType == "update")
            {
                //We do not support XML content and Form. Check if the content is XML or form and if it is then don't proceed further.
                ContentData cData;
                cData = m_refContApi.GetContentById(m_intItemId, 0);
                if ((cData.Type == 2) || ((cData.XmlConfiguration != null) && (cData.XmlConfiguration.PackageXslt.Length > 0)))
                {
                    bEphoxSupport = false;
                }
                else
                {
                    bEphoxSupport = true;
                }
                if (!bEphoxSupport)
                {
                    //Show not supported message
                    throw (new Exception("Forms and XML Editing is not supported on MAC."));
                }
            }
            if ((Request.QueryString["pullapproval"] == "true") && (m_strType == "update"))
            {
                ret = m_refContent.TakeOwnership(m_intItemId);
            }
            var2 = m_refContent.GetEditorVariablev2_0(m_intItemId, m_strType); //TODO:Verify info param via var1 removed
            security_data = m_refContApi.LoadPermissions(m_intItemId, "content", 0);
            endDateActionSel = GetEndDateActionStrings();
            endDateActionSize = Convert.ToInt32(endDateActionSel["SelectionSize"]);
            if (security_data != null)
            {
                IsAdmin = security_data.IsAdmin;
            }
            active_subscription_list = m_refContApi.GetAllActiveSubscriptions();
            settings_data = m_refSiteApi.GetSiteVariables(CurrentUserID);

            if (m_strType == "update")
            {
                content_edit_data = m_refContApi.GetContentForEditing(m_intItemId);
                UserRights = m_refContApi.LoadPermissions(m_intItemId, "content", ContentAPI.PermissionResultType.Content);
                lContentType = content_edit_data.Type;
                lContentSubType = content_edit_data.SubType;
                if (content_edit_data.Type == 2 || 4 == content_edit_data.Type)
                {
                    bIsFormDesign = true;
                    m_intContentType = 2;
                }
                if (!(content_edit_data == null))
                {
                    security_lib_data = m_refContApi.LoadPermissions(content_edit_data.FolderId, "folder", 0);
                    UploadPrivs = security_lib_data.CanAddToFileLib || security_lib_data.CanAddToImageLib;
                    m_strContentTitle = Server.HtmlDecode(content_edit_data.Title);
                    m_strAssetFileName = content_edit_data.AssetData.FileName;
                    m_strContentHtml = content_edit_data.Html;
                    content_teaser = content_edit_data.Teaser;
                    meta_data = content_edit_data.MetaData;


                    content_comment = Server.HtmlDecode(content_edit_data.Comment);
                    content_stylesheet = content_edit_data.StyleSheet;
                    m_intContentFolder = content_edit_data.FolderId;
                    m_intTaxFolderId = content_edit_data.FolderId;
                    intContentLanguage = content_edit_data.LanguageId;
                    go_live = content_edit_data.GoLive;
                    end_date = content_edit_data.EndDate;
                    end_date_action = content_edit_data.EndDateAction.ToString();
                    intInheritFrom = m_refContent.GetFolderInheritedFrom(m_intContentFolder);
                  
                    subscription_data_list = m_refContApi.GetSubscriptionsForFolder(intInheritFrom); 
                    subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForContent(m_refContentId); //first try content
                    if (subscription_properties_list == null)
                    {
                        subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForFolder(intInheritFrom); //then get folder
                        subscribed_data_list = subscription_data_list;
                    }
                    else //content is populated.
                    {
                        subscribed_data_list = m_refContApi.GetSubscriptionsForContent(m_refContentId); // get subs for folder
                    }

                    if (!(meta_data == null))
                    {
                        MetaDataNumber = meta_data.Length;
                    }
                    PreviousState = content_edit_data.CurrentStatus;
                    iMaxContLength = content_edit_data.MaxContentSize;
                    iMaxSummLength = content_edit_data.MaxSummarySize;
                    path = content_edit_data.Path;
                    m_intManualAliasId = content_edit_data.ManualAliasId;

                    folder_data = m_refContApi.GetFolderById(m_intContentFolder);

                    if ((path.Substring(path.Length - 1, 1) == "\\"))
                    {
                        path = path.Substring(path.Length -(path.Length - 1));
                    }
                    //Check to see if this belongs to XML configuration
                    if (lContentType != 2)
                    {
                        xmlconfig_data = content_edit_data.XmlConfiguration;
                        if (!(xmlconfig_data == null))
                        {
                            editorPackage = xmlconfig_data.PackageXslt;
                            MultiTemplateID.Text = "<input type=\"hidden\" name=\"xid\" value=\"" + content_edit_data.XmlConfiguration.Id.ToString() + "\">";
                            if (editorPackage.Length > 0)
                            {
                                bVer4Editor = true; // this means that we will be using the new Package Design for the content
                            }
                        }
                    }

                    if (m_strContentTitle != "")
                    {
                        MetaComplete = UserRights.CanMetadataComplete; //Changed from 1 to true
                    }
                    asset_info["AssetID"] = content_edit_data.AssetData.Id;
                    asset_info["AssetVersion"] = content_edit_data.AssetData.Version;
                    asset_info["MimeType"] = content_edit_data.AssetData.MimeType;
                    asset_info["FileExtension"] = content_edit_data.AssetData.FileExtension;
                }
                validTypes.Value = Convert.ToString(asset_info["FileExtension"]);
            }
            else
            {

                UserRights = m_refContApi.LoadPermissions(m_intItemId, "folder", ContentAPI.PermissionResultType.Folder);
                folder_data = m_refContApi.GetFolderById(m_intItemId);
                MetaComplete = UserRights.CanMetadataComplete;
                if (m_intXmlConfigId > -1)
                {
                    xmlconfig_data = m_refContApi.GetXmlConfiguration(m_intXmlConfigId);
                    MultiTemplateID.Text = "<input type=\"hidden\" name=\"xid\" value=\"" + m_intXmlConfigId.ToString() + "\">";
                }
                else
                {
                    if ((folder_data.XmlConfiguration != null) && (folder_data.XmlConfiguration.Length > 0) && (Request.QueryString["AllowHTML"] != "1"))
                    {
                        xmlconfig_data = folder_data.XmlConfiguration[0];
                    }
                    else
                    {
                        xmlconfig_data = null;
                    }
                }
                if (!(xmlconfig_data == null))
                {
                    editorPackage = xmlconfig_data.PackageXslt;
                    if (editorPackage.Length > 0)
                    {
                        bVer4Editor = true;
                    }
                }
                content_stylesheet = m_refContApi.GetStyleSheetByFolderID(m_intItemId);
                security_lib_data = m_refContApi.LoadPermissions(m_intItemId, "folder", 0);
                UploadPrivs = security_lib_data.CanAddToFileLib || security_lib_data.CanAddToImageLib;
                string TmpId = Request.QueryString["content_id"];
                if (!String.IsNullOrEmpty(TmpId))
                {
                    //translating asset
                    if (Request.QueryString["type"] == "add")
                    {
                        if (!String.IsNullOrEmpty(Request.QueryString["back_LangType"]))
                        {
                            m_refContApi.ContentLanguage = Convert.ToInt32(Request.QueryString["back_LangType"]);
                        }
                        else
                        {
                            m_refContApi.ContentLanguage = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()["DefaultLanguage"]);
                        }
                    }
                    content_data = m_refContApi.GetContentById(Convert.ToInt64(TmpId), 0);
                    if (content_data != null)
                    {
                        if (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
                        {
                            isOfficeDoc.Value = "true";
                        }
                        if (m_intXmlConfigId == -1)
                        {
                            if (content_data.XmlConfiguration != null)
                            {
                                m_intXmlConfigId = content_data.XmlConfiguration.Id;
                                xmlconfig_data = content_data.XmlConfiguration;
                                editorPackage = xmlconfig_data.PackageXslt;
                                if (editorPackage.Length > 0)
                                {
                                    bVer4Editor = true;
                                }
                                MultiTemplateID.Text = "<input type=\"hidden\" name=\"xid\" value=\"" + m_intXmlConfigId.ToString() + "\">";
                            }
                        }

                        m_strContentTitle = Server.HtmlDecode(content_data.Title);
                        m_strAssetFileName = content_data.AssetData.FileName;
                        m_strContentHtml = content_data.Html;
                        content_teaser = content_data.Teaser;
                        content_comment = Server.HtmlDecode(content_data.Comment);
                        go_live = content_data.GoLive;
                        end_date = content_data.EndDate;
                        end_date_action = content_data.EndDateAction.ToString();
                        lContentType = content_data.Type;
                        lContentSubType = content_data.SubType;
                        if (m_strType == "add")
                        {
                            if (Utilities.IsAssetType(lContentType))
                            {
                                m_strContentTitle = Server.HtmlDecode(content_data.Title);
                                validTypes.Value = content_data.AssetData.FileExtension;
                            }
                        }
                        else
                        {
                            asset_info["AssetID"] = content_data.AssetData.Id;
                            asset_info["AssetVersion"] = content_data.AssetData.Version;
                            asset_info["AssetFilename"] = content_data.AssetData.FileName;
                            asset_info["MimeType"] = content_data.AssetData.MimeType;
                            asset_info["FileExtension"] = content_data.AssetData.FileExtension;
                            asset_info["MimeName"] = content_data.AssetData.MimeName;
                            asset_info["ImageUrl"] = content_data.AssetData.ImageUrl;
                            if (Convert.ToString(asset_info["MimeType"]) == "application/x-shockwave-flash")
                            {
                                asset_info["MediaAsset"] = true;
                            }
                            else
                            {
                                asset_info["MediaAsset"] = false;
                            }
                            validTypes.Value = Convert.ToString(asset_info["FileExtension"]);
                            //Next
                        }
                    }
                }
                else
                {
                    //Adding new file
                    List<string> fileTypeCol = new List<string>(DocumentManagerData.Instance.FileTypes.Split(",".ToCharArray()));
                    string allTypes = "";
                    foreach (string type in fileTypeCol)
                    {
                        if (allTypes.Length > 0)
                        {
                            allTypes += (string)("," + type.Trim().Replace("*.", ""));
                        }
                        else
                        {
                            allTypes += type.Trim().Replace("*.", "");
                        }
                    }
                    validTypes.Value = allTypes;
                }
                m_intContentFolder = m_intItemId;
                intInheritFrom = m_refContent.GetFolderInheritedFrom(m_intContentFolder);
                subscription_data_list = m_refContApi.GetSubscriptionsForFolder(intInheritFrom); //AGofPA get subs for folder; set break inheritance flag false
                subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForFolder(intInheritFrom); //get folder properties
                subscribed_data_list = subscription_data_list; //get subs for folder
                intContentLanguage = m_intContentLanguage;
                m_refContApi.ContentLanguage = m_intContentLanguage;

                meta_data = m_refContApi.GetMetaDataTypes("id");
                path = m_refContApi.GetPathByFolderID(m_intContentFolder);
                if ((path.Substring(path.Length - 1, 1) == "\\"))
                {
                    path = path.Substring(path.Length - (path.Length - 1));
                }
                iMaxContLength = int.Parse(settings_data.MaxContentSize);
                iMaxSummLength = int.Parse(settings_data.MaxSummarySize);
            }
            if (folder_data.FolderType == 1)
            {
                m_bIsBlog = true;
                blog_data = m_refContApi.BlogObject(folder_data);
                if (m_strType == "update")
                {
                    blog_post_data = m_refContApi.GetBlogPostData(m_intItemId);
                }
                else if (m_strType == "add" && m_refContentId > 0) // add new lang
                {
                    blog_post_data = m_refContApi.EkContentRef.GetBlogPostDataOnly(m_refContentId, back_LangType);
                }
                else
                {
                    blog_post_data = m_refContApi.GetBlankBlogPostData();
                }
            }
            if (xmlconfig_data != null)
            {
                Collection collXmlConfigData = (Collection)xmlconfig_data.LogicalPathComplete;
                if (bVer4Editor == false) //only do this if we are using the old method
                {
                    urlxml = "?Edit_xslt=";
                    if (xmlconfig_data.EditXslt.Length > 0)
                    {
                        urlxml = urlxml + EkFunctions.UrlEncode(Convert.ToString(collXmlConfigData["EditXslt"]));
                        if (m_strContentHtml.Trim().Length == 0)
                        {
                            m_strContentHtml = "<root> </root>";
                        }
                    }
                    urlxml = urlxml + "&Save_xslt=";
                    if (xmlconfig_data.SaveXslt.Length > 0)
                    {
                        save_xslt_file = Convert.ToString(collXmlConfigData["SaveXslt"]);
                        urlxml = urlxml + EkFunctions.UrlEncode(save_xslt_file);
                    }
                    urlxml = urlxml + "&Schema=";
                    if (xmlconfig_data.XmlSchema.Length > 0)
                    {
                        m_strSchemaFile = Convert.ToString(collXmlConfigData["XmlSchema"]);
                        urlxml = urlxml + EkFunctions.UrlEncode(m_strSchemaFile);
                    }
                    xml_config = AppeWebPath + "cms_xmlconfig.aspx" + urlxml;
                    if (xmlconfig_data.XmlAdvConfig.Length > 0)
                    {
                        xml_config = Convert.ToString(collXmlConfigData["XmlAdvConfig"] + urlxml);
                    }
                    m_strSchemaFile = Convert.ToString(collXmlConfigData["XmlSchema"]);
                    m_strNamespaceFile = Convert.ToString(collXmlConfigData["XmlNameSpace"]);
                }
            }

            //DHTML RENDERING
            //ASSET CONFIG
            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                AssetHidden.Text += "<input type=\"hidden\" name=\"asset_" + Strings.LCase(Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i]) + "\" value=\"" + EkFunctions.HtmlEncode(asset_info[Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i]].ToString()) + "\">";
            }
            content_type.Value = Convert.ToString(lContentType);
            content_subtype.Value = Convert.ToString(lContentSubType);
            if (m_SelectedEditControl != "ContentDesigner")
            {
                jsEditorScripts.Text = Utilities.EditorScripts(var2, AppeWebPath, BrowserCode);
            }
            AutoNav.Text = path.Replace("\\", "\\\\");
            invalidFormatMsg.Text = m_refMsg.GetMessage("js: invalid date format error msg");
            invalidYearMsg.Text = m_refMsg.GetMessage("js: invalid year error msg");
            invalidMonthMsg.Text = m_refMsg.GetMessage("js: invalid month error msg");
            invalidDayMsg.Text = m_refMsg.GetMessage("js: invalid day error msg");
            invalidTimeMsg.Text = m_refMsg.GetMessage("js: invalid time error msg");

            if (MetaComplete)
            {
                ecmMetaComplete.Text = "1";
            }
            else
            {
                ecmMetaComplete.Text = "0";
            }
            ecmMonths.Text = "";
            jsNullContent.Text = m_refMsg.GetMessage("null content warning msg");
            jsEDWarning.Text = m_refMsg.GetMessage("js: earlier end date warning");
            jsMetaCompleteWarning.Text = m_refMsg.GetMessage("js: alert cannot submit meta incomplete") + "\\n" + m_refMsg.GetMessage("js: alert save or checkin or complete meta");
            jsSetActionFunction.Text = SetActionClientScript(folder_data.PublishHtmlActive, (xmlconfig_data != null && 1 == lContentType));
            jsSitePath.Text = m_refContApi.SitePath;
            jsEditProLocale.Text = AppeWebPath + "locale" + AppLocaleString + "b.xml";
            ValidateContentPanel.Text = " var errReason = 0;" + "\r\n";
            ValidateContentPanel.Text += "var errReasonT = 0;" + "\r\n";
            ValidateContentPanel.Text += "var errAccess = false;" + "\r\n";
            ValidateContentPanel.Text += "var errMessage = \"\";" + "\r\n";
            ValidateContentPanel.Text += "var sInvalidContent = \"Continue saving invalid document?\";" + "\r\n";
            if (m_SelectedEditControl != "ContentDesigner")
            {
                ValidateContentPanel.Text += "if (eWebEditProMessages) {" + "\r\n";
                ValidateContentPanel.Text += "  sInvalidContent = eWebEditProMessages.invalidContent;" + "\r\n";
                ValidateContentPanel.Text += "}" + "\r\n";
            }
            ValidateContentPanel.Text += "var errContent = \"" + m_refMsg.GetMessage("js: alert invalid data") + "\";" + "\r\n";
            ValidateContentPanel.Text += "var objValidateInstance = null;" + "\r\n";
            if (m_SelectedEditControl != "ContentDesigner")
            {
                ValidateContentPanel.Text += "objValidateInstance = eWebEditPro.instances[\"content_html\"];" + "\r\n";
                ValidateContentPanel.Text += "if (objValidateInstance){" + "\r\n";
                ValidateContentPanel.Text += "	if (!eWebEditPro.instances[\"content_html\"].validateContent()) {" + "\r\n";
                ValidateContentPanel.Text += "		errReason = objValidateInstance.event.reason;" + "\r\n";
                ValidateContentPanel.Text += "		if (-1001 == errReason || -1002 == errReason || 1003 == errReason || -1003 == errReason) {" + "\r\n";
                ValidateContentPanel.Text += "			errAccess = true;" + "\r\n";
                ValidateContentPanel.Text += "		}" + "\r\n";
                ValidateContentPanel.Text += "	}" + "\r\n";
                ValidateContentPanel.Text += "}" + "\r\n";
            }
            else
            {
                ValidateContentPanel.Text += "  if (\"object\" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) {" + "\r\n";
                ValidateContentPanel.Text += "      var objContentEditor = Ektron.ContentDesigner.instances[\"content_html\"];" + "\r\n";
                ValidateContentPanel.Text += "      if (objContentEditor && \"function\" == typeof objContentEditor.validateContent) {" + "\r\n";
                ValidateContentPanel.Text += "          errMessage = objContentEditor.validateContent();" + "\r\n";
                ValidateContentPanel.Text += "      }" + "\r\n";
                ValidateContentPanel.Text += "      if (errMessage != null && errMessage != \"\") {" + "\r\n";
                ValidateContentPanel.Text += "          if (\"object\" == typeof errMessage && \"undefined\" == typeof errMessage.code) {" + "\r\n";
                ValidateContentPanel.Text += "              alert(errMessage.join(\"\\n\\n\\n\"));" + "\r\n";
                ValidateContentPanel.Text += "		        return false;" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "          else if (\"object\" == typeof errMessage && \"string\" == typeof errMessage.msg) {" + "\r\n";
                ValidateContentPanel.Text += "		        errReason = errMessage.code;" + "\r\n";
                ValidateContentPanel.Text += "			    errAccess = true;" + "\r\n";
                ValidateContentPanel.Text += "              alert(\"Content is invalid.\" + \"\\n\\n\" + errMessage.msg);" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "          else if (\"string\" == typeof errMessage && errMessage.length > 0) {" + "\r\n";
                ValidateContentPanel.Text += "              alert(errMessage);" + "\r\n";
                ValidateContentPanel.Text += "		        return false;" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "      }" + "\r\n";
                ValidateContentPanel.Text += "  }" + "\r\n";
            }
            ValidateContentPanel.Text += "var objTeaserInstance = null;" + "\r\n";
            if (m_SelectedEditControl != "ContentDesigner")
            {
                ValidateContentPanel.Text += "objTeaserInstance = eWebEditPro.instances[\"content_teaser\"];" + "\r\n";
                ValidateContentPanel.Text += "if (objTeaserInstance){" + "\r\n";
                ValidateContentPanel.Text += "	if (!objTeaserInstance.validateContent()) {" + "\r\n";
                ValidateContentPanel.Text += "		errReasonT = objTeaserInstance.event.reason;" + "\r\n";
                ValidateContentPanel.Text += "		if (-1001 == errReasonT || -1002 == errReasonT || 1003 == errReasonT || -1003 == errReasonT) {" + "\r\n";
                ValidateContentPanel.Text += "			errAccess = true;" + "\r\n";
                ValidateContentPanel.Text += "		}" + "\r\n";
                ValidateContentPanel.Text += "	}" + "\r\n";
                ValidateContentPanel.Text += "}" + "\r\n";
            }
            else
            {
                ValidateContentPanel.Text += "  if (\"object\" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances && (\"\" == errMessage || null == errMessage)) {" + "\r\n";
                ValidateContentPanel.Text += "      var teaserName = \"content_teaser\";" + "\r\n";
                ValidateContentPanel.Text += "      if (document.forms[0].response) {" + "\r\n";
                ValidateContentPanel.Text += "        var iTeaser = 0;" + "\r\n";
                ValidateContentPanel.Text += "        for (var i = 0; i < document.forms[0].response.length; i++) {" + "\r\n";
                ValidateContentPanel.Text += "            if (document.forms[0].response[i].checked) {" + "\r\n";
                ValidateContentPanel.Text += "                iTeaser = i;" + "\r\n";
                ValidateContentPanel.Text += "            }" + "\r\n";
                ValidateContentPanel.Text += "        }" + "\r\n";
                ValidateContentPanel.Text += "        switch (iTeaser) {" + "\r\n";
                ValidateContentPanel.Text += "            case 2: " + "\r\n";
                ValidateContentPanel.Text += "                teaserName = \"forms_transfer\";" + "\r\n";
                ValidateContentPanel.Text += "                break;" + "\r\n";
                ValidateContentPanel.Text += "            case 1:" + "\r\n";
                ValidateContentPanel.Text += "                teaserName = \"forms_redirect\";" + "\r\n";
                ValidateContentPanel.Text += "                break;" + "\r\n";
                ValidateContentPanel.Text += "            case 0:" + "\r\n";
                ValidateContentPanel.Text += "            default:" + "\r\n";
                ValidateContentPanel.Text += "                teaserName = \"content_teaser\";" + "\r\n";
                ValidateContentPanel.Text += "                break;" + "\r\n";
                ValidateContentPanel.Text += "        }" + "\r\n";
                ValidateContentPanel.Text += "      }" + "\r\n";
                ValidateContentPanel.Text += "      var objTeaserEditor = Ektron.ContentDesigner.instances[teaserName];" + "\r\n";
                ValidateContentPanel.Text += "      if (objTeaserEditor && \"function\" == typeof objTeaserEditor.validateContent){" + "\r\n";
                ValidateContentPanel.Text += "          errMessage = objTeaserEditor.validateContent();" + "\r\n";
                ValidateContentPanel.Text += "      }" + "\r\n";
                ValidateContentPanel.Text += "      if (errMessage != null && errMessage != \"\") {" + "\r\n";
                ValidateContentPanel.Text += "          if (\"object\" == typeof errMessage && \"undefined\" == typeof errMessage.code) {" + "\r\n";
                ValidateContentPanel.Text += "              alert(errMessage.join(\"\\n\\n\\n\"));" + "\r\n";
                ValidateContentPanel.Text += "		        return false;" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "          else if (\"object\" == typeof errMessage && \"string\" == typeof errMessage.msg) {" + "\r\n";
                ValidateContentPanel.Text += "		        errReason = errMessage.code;" + "\r\n";
                ValidateContentPanel.Text += "			    errAccess = true;" + "\r\n";
                ValidateContentPanel.Text += "              alert(\"Content is invalid.\" + \"\\n\\n\" + errMessage.msg);" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "          else if (\"string\" == typeof errMessage && errMessage.length > 0) {" + "\r\n";
                ValidateContentPanel.Text += "              alert(errMessage);" + "\r\n";
                ValidateContentPanel.Text += "		        return false;" + "\r\n";
                ValidateContentPanel.Text += "          }" + "\r\n";
                ValidateContentPanel.Text += "      }" + "\r\n";
                ValidateContentPanel.Text += "  }" + "\r\n";
            }
            ValidateContentPanel.Text += "if (errReason != 0 || errReasonT != 0) {" + "\r\n";
            ValidateContentPanel.Text += "	if (errReasonT != 0 && typeof objTeaserInstance != \"undefined\" && objTeaserInstance) {" + "\r\n";
            ValidateContentPanel.Text += "		errMessage = objTeaserInstance.event.message + \"\";" + "\r\n";
            ValidateContentPanel.Text += "	}" + "\r\n";
            ValidateContentPanel.Text += "	if (errReason != 0 && typeof objValidateInstance != \"undefined\" && objValidateInstance) {" + "\r\n";
            ValidateContentPanel.Text += "		errMessage = objValidateInstance.event.message + \"\";" + "\r\n";
            ValidateContentPanel.Text += "	}" + "\r\n";
            ValidateContentPanel.Text += "	if (false == errAccess) {" + "\r\n";
            ValidateContentPanel.Text += "		alert(errContent + \"\\n\"  + errMessage);" + "\r\n";
            ValidateContentPanel.Text += "		$ektron(document).trigger(\"wizardPanelShown\");" + "\r\n";
            ValidateContentPanel.Text += "		return false;" + "\r\n";
            ValidateContentPanel.Text += "	}" + "\r\n";
            ValidateContentPanel.Text += "	else {" + "\r\n";
            if ("2" == settings_data.Accessibility)
            {
                ValidateContentPanel.Text += " if (typeof Button != \"undefined\") {" + "\r\n";
                ValidateContentPanel.Text += "		if (\"publish\" == Button.toLowerCase() || \"submit\" == Button.toLowerCase()) {" + "\r\n";
                ValidateContentPanel.Text += "			alert(errContent);" + "\r\n";
                ValidateContentPanel.Text += "			$ektron(document).trigger(\"wizardPanelShown\");" + "\r\n";
                ValidateContentPanel.Text += "			return false;" + "\r\n";
                ValidateContentPanel.Text += "		}" + "\r\n";
                ValidateContentPanel.Text += "		else { " + "\r\n";
                ValidateContentPanel.Text += "			if (confirm(errContent + \"\\n\" + sInvalidContent)) {" + "\r\n";
                ValidateContentPanel.Text += "				return true;" + "\r\n";
                ValidateContentPanel.Text += "			} " + "\r\n";
                ValidateContentPanel.Text += "			else {" + "\r\n";
                ValidateContentPanel.Text += "			    $ektron(document).trigger(\"wizardPanelShown\");" + "\r\n";
                ValidateContentPanel.Text += "			    return false;" + "\r\n";
                ValidateContentPanel.Text += "			} " + "\r\n";
                ValidateContentPanel.Text += "		}" + "\r\n";
                ValidateContentPanel.Text += " }" + "\r\n";
            }
            else if ("1" == settings_data.Accessibility)
            {
                ValidateContentPanel.Text += " if (confirm(errContent + \"\\n\" + sInvalidContent)) {" + "\r\n";
                ValidateContentPanel.Text += "	return true;" + "\r\n";
                ValidateContentPanel.Text += " } " + "\r\n";
                ValidateContentPanel.Text += " else {$ektron(document).trigger(\"wizardPanelShown\"); return false;} " + "\r\n";
            }
            ValidateContentPanel.Text += "	}" + "\r\n";
            ValidateContentPanel.Text += "}" + "\r\n";
            //Change the action page
            FormAction = (string)("edit.aspx?close=" + Request.QueryString["close"] + "&LangType=" + m_intContentLanguage + "&content_id=" + m_refContentId + (this.TaxonomyOverrideId > 0 ? ("&TaxonomyId=" + this.TaxonomyOverrideId.ToString()) : "") + (this.TaxonomySelectId > 0 ? ("&SelTaxonomyId=" + this.TaxonomySelectId.ToString()) : "") + "&back_file=" + back_file + "&back_action=" + back_action + "&back_folder_id=" + back_folder_id + "&back_id=" + back_id + "&back_form_id=" + back_form_id + "&control=" + controlName + "&buttonid=" + buttonId.Value + "&back_LangType=" + back_LangType + back_callerpage + back_origurl);
            if (Request.QueryString["pullapproval"] != null)
            {
                FormAction += (string)("&pullapproval=" + Request.QueryString["pullapproval"]);
            }
            PostBackPage.Text = "<script>document.forms[0].action = \"" + FormAction + "\";";
            if (Utilities.IsAssetType(lContentType))
            {
                PostBackPage.Text += "document.forms[0].enctype = \"multipart/form-data\";";
            }

            PostBackPage.Text += "</script>";
            LoadingImg.Text = m_refMsg.GetMessage("one moment msg");

            content_title.Value = Server.HtmlDecode(m_strContentTitle);
            if (content_title.Attributes["class"] == null)
            {
                content_title.Attributes.Add("class", "");
            }
            if (lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
            {
                content_title.Attributes["class"] = "masterlayout";
                if (!(m_strType == "update"))
                {
                    content_title.Disabled = true;
                }
                phAlias.Visible = false;
                EditAliasHtml.Visible = false;
            }
            else
            {
                content_title.Attributes.Remove("class");
            }

            if (EnableMultilingual == 1)
            {
                lblLangName.Text = "[" + language_data.Name + "]";
            }
            StringBuilder sbFolderBreadcrumb = new StringBuilder();
            string strDisabled = "";
            if (!(m_strType == "update"))
            {
                QLink_Search.Text = "<td nowrap=\"nowrap\" class=\"checkboxIsSearchable\" >";
                QLink_Search.Text += "<input type=\"hidden\" name=\"AddQlink\" value=\"AddQlink\">";

                if (Request.Cookies[DMSCookieName] != null && Request.Cookies[DMSCookieName].Value == "2010")
                {
                    if (folder_data.IscontentSearchable)
                        QLink_Search.Text += "<input type=\"hidden\" name=\"IsSearchable\" value=\"IsSearchable\">";
                }
                else
                {
                if (security_data.IsAdmin)
                {
                    if (folder_data.IscontentSearchable)
                        QLink_Search.Text += "<input type=\"checkbox\" name=\"IsSearchable\" " + strDisabled + " checked value=\"IsSearchable\" >" + m_refMsg.GetMessage("lbl content searchable"); //m_refMsg.GetMessage("Content Searchable")
                    else
                        QLink_Search.Text += "<input type=\"checkbox\" name=\"IsSearchable\" " + strDisabled + " >" + m_refMsg.GetMessage("lbl content searchable"); //m_refMsg.GetMessage("Content Searchable")
                }
                else
                {
                        //Need to inherit from parent.
                        if (folder_data.IscontentSearchable)
                    QLink_Search.Text += "<input type=\"hidden\" name=\"IsSearchable\" value=\"IsSearchable\">";

                    }
                }
                QLink_Search.Text += "</td>";
            }
            else
            {
                TR_Properties.Visible = false;
                TR_Properties.Height = new Unit(0);
            }

            if (QLink_Search.Text != "")
            {
                QLink_Search.Text = "<table><tr>" + QLink_Search.Text + "</tr></table>";
            }
            content_id.Value = Convert.ToString(m_refContentId);
            eType.Value = m_strType;
            mycollection.Value = strMyCollection;
            addto.Value = strAddToCollectionType;
            content_folder.Value = Convert.ToString(m_intContentFolder);
            content_language.Value = Convert.ToString(intContentLanguage);
            maxcontentsize.Value = iMaxContLength.ToString();
            if (bVer4Editor)
            {
                Ver4Editor.Value = "true";
            }
            else
            {
                Ver4Editor.Value = "false";
            }
            createtask.Value = Request.QueryString["dontcreatetask"];

            EnumeratedHiddenFields.Text = HideVariables();
            eWebEditProJS.Text = EditProJS();

            if (m_intContentType == 2)
            {
                divContentText.Text = m_refMsg.GetMessage("form text");
                divSummaryText.Text = m_refMsg.GetMessage("postback text");
            }
            else
            {
                divContentText.Text = m_refMsg.GetMessage("content text");
                divSummaryText.Text = m_refMsg.GetMessage("Summary text");
            }

            phMetadata.Visible = true;
            if (this.Request.QueryString["type"] == "update")
            {
                aliasContentType = this.content_edit_data.ContType.ToString();
            }

            if ((m_urlAliasSettings.IsManualAliasEnabled || m_urlAliasSettings.IsAutoAliasEnabled) && m_refContApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias) && Request.QueryString["type"] != "multiple,add" && lContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) //And Not (m_bIsBlog)
            {
                if ((content_edit_data != null) && (content_edit_data.AssetData != null) && Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_edit_data.AssetData.FileExtension)))
                {
                    phAlias.Visible = false;
                    EditAliasHtml.Visible = false;
                }
                else
                {
                    phAlias.Visible = true;
                    EditAliasHtml.Visible = true;
                }
            }
            EditContentHtmlScripts();
            EditSummaryHtmlScripts();
            EditMetadataHtmlScripts();
            EditAliasHtmlScripts();
            EditScheduleHtmlScripts();
            EditCommentHtmlScripts();
            EditSubscriptionHtmlScripts();
            EditSelectedTemplate();
            EditTaxonomyScript();

            if (eWebEditProPromptOnUnload == 1)
            {
                jsActionOnUnload.Text = "eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_PROMPT;";
            }

            if (Convert.ToString(m_intContentFolder) != "")
            {
                defaultFolderId.Text = m_intContentFolder.ToString();
            }
            else
            {
                defaultFolderId.Text = "0";
            }

            //Summary_Meta_win
            if (!String.IsNullOrEmpty(Request.QueryString["summary"]))
            {
                Summary_Meta_Win.Text = "<script language=\"JavaScript1.2\">";
                Summary_Meta_Win.Text += "PopUpWindow(\'editsummaryarea.aspx?id=" + m_intItemId + "&LangType=" + m_intContentLanguage + "&editor=true\',\'Summary\',790,580,1,1);";
                Summary_Meta_Win.Text += "</script>";
            }
            if (!String.IsNullOrEmpty(Request.QueryString["meta"]))
            {
                Summary_Meta_Win.Text += "<script language=\"JavaScript1.2\">";
                if (MetaDataNumber > 0)
                {
                    Summary_Meta_Win.Text += "PopUpWindow(\'editmeta_dataarea.aspx?id=" + m_intItemId + "&LangType=" + m_intContentLanguage + "&editor=true\',\'Metadata\',790,580,1,1);";

                }
                else
                {
                    Summary_Meta_Win.Text += "alert(\'No metadata defined\');  ";
                }
                Summary_Meta_Win.Text += "</script>";
            }
            //TAXONOMY DATA
            if (IsAdmin || m_refContApi.EkUserRef.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator), CurrentUserID, false))
            {
                TaxonomyRoleExists = true;
            }
            TaxonomyBaseData[] taxonomy_cat_arr = null;
            if (m_strType != "add" && m_strType != "multiple" && (!(m_strType.IndexOf("add", System.StringComparison.InvariantCultureIgnoreCase) > 0 || m_strType.IndexOf("multiple", System.StringComparison.InvariantCultureIgnoreCase) > 0)) || (m_strType == "add" && m_refContentId > 0))
            {
                int tmpLang = 1033;
                int originalLangID = 1033;
                if (m_strType == "add" && m_refContentId > 0) //New Language
                {
                    if (!(Request.QueryString["con_lang_id"] == null) && Request.QueryString["con_lang_id"] != "")
                    {
                        originalLangID = Convert.ToInt32(Request.QueryString["con_lang_id"]);
                    }
                    tmpLang = m_refContent.RequestInformation.ContentLanguage; //Backup the current langID
                    m_refContent.RequestInformation.ContentLanguage = originalLangID;
                    taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(m_refContentId);
                    m_refContent.RequestInformation.ContentLanguage = tmpLang;
                }
                else
                {
                    taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(m_intItemId);
                }
                if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                {
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                    {
                        if (taxonomy_cat.LanguageId == 0 || taxonomy_cat.LanguageId == m_refContent.RequestInformation.ContentLanguage)
                        {
                            if (taxonomyselectedtree.Value == "")
                            {
                                taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.Id);
                            }
                            else
                            {
                                taxonomyselectedtree.Value = taxonomyselectedtree.Value + "," + Convert.ToString(taxonomy_cat.Id);
                            }
                        }
                    }
                }
                TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
                if (TaxonomyTreeIdList.Trim().Length > 0)
                {
                    if (m_strType == "add" && m_refContentId > 0) //New Language
                    {
                        m_refContent.RequestInformation.ContentLanguage = originalLangID; //Backup the current LangID
                        TaxonomyTreeParentIdList = m_refContent.ReadDisableNodeList(m_refContentId);
                        m_refContent.RequestInformation.ContentLanguage = tmpLang;
                    }
                    else
                    {
                        TaxonomyTreeParentIdList = m_refContent.ReadDisableNodeList(m_intItemId);
                    }
                }
            }
            else
            {
                if (TaxonomySelectId > 0)
                {
                    taxonomyselectedtree.Value = TaxonomySelectId.ToString();
                    TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
                    taxonomy_cat_arr = m_refContent.GetTaxonomyRecursiveToParent(TaxonomySelectId, m_refContent.RequestInformation.ContentLanguage, 0);
                    if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                    {
                        foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                        {
                            if (TaxonomyTreeParentIdList == "")
                            {
                                TaxonomyTreeParentIdList = Convert.ToString(taxonomy_cat.Id);
                            }
                            else
                            {
                                TaxonomyTreeParentIdList = TaxonomyTreeParentIdList + "," + Convert.ToString(taxonomy_cat.Id);
                            }
                        }
                    }
                }
            }

            TaxonomyRequest taxonomy_request = new TaxonomyRequest();
            TaxonomyBaseData[] taxonomy_data_arr = null;
            Utilities.SetLanguage(m_refContApi);
            taxonomy_request.TaxonomyId = m_intContentFolder;
            taxonomy_request.TaxonomyLanguage = m_refContApi.ContentLanguage;
            taxonomy_data_arr = m_refContent.GetAllFolderTaxonomy(m_intContentFolder);
            bool HideCategoryTab = false;
            if (Request.QueryString["HideCategoryTab"] != null)
            {
                HideCategoryTab = Convert.ToBoolean(Request.QueryString["HideCategoryTab"]);
            }
            if (HideCategoryTab || (taxonomy_data_arr == null || taxonomy_data_arr.Length == 0) && (TaxonomyOverrideId == 0))
            {
                if (!HideCategoryTab && folder_data != null && folder_data.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Blog) && TaxonomySelectId > 0 && m_intTaxFolderId == folder_data.Id && TaxonomyTreeIdList.Trim().Length > 0)
                {
                    m_intTaxFolderId = 0;
                }
                else
                {
                    phTaxonomy.Visible = false;
                    EditTaxonomyHtml.Visible = false;
                    DisplayTab = false;
                    taxonomyselectedtree.Value = taxonomy_cat_arr != null && taxonomy_cat_arr.Length > 0 && (taxonomy_cat_arr[0].LanguageId == 0 | taxonomy_cat_arr[0].LanguageId == m_refContent.RequestInformation.ContentLanguage) ? taxonomyselectedtree.Value : "";
                }
            }

            //CALL THE TOOLBAR
            if (folder_data == null)
            {
                LoadToolBar("");
            }
            else
            {
                LoadToolBar(folder_data.Name);
            }

            if (lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
            {
                WebEventCont.Text = "true";
                phContent.Visible = false;
                phEditContent.Visible = false;
            }
            //-------------------DisplayTabs Based on selected options from Folder properties----------------------------------
            if (((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && folder_data.DisplaySettings != 0)
            {
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
                { phEditSummary.Visible = true; }
                else
                {
                    if (Request.QueryString["form_type"] == null && Request.QueryString["back_form_id"] == null && Request.QueryString["form_id"] == null && m_bIsBlog != true)
                    {
                        phEditSummary.Visible = false;
                        phSummary.Visible = false;
                    }
                }
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
                {if(phMetadata.Visible)
                    phMetadata.Visible = true; }
                else
                {
                    if (!metadataRequired)
                        phMetadata.Visible = false;
                }
                if ((m_urlAliasSettings.IsManualAliasEnabled || m_urlAliasSettings.IsAutoAliasEnabled) && m_refContApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias) && Request.QueryString["type"] != "multiple,add" && lContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) //And Not (m_bIsBlog)
                {
                    if (!((content_edit_data != null) && (content_edit_data.AssetData != null) && Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_edit_data.AssetData.FileExtension))))
                    {
                        if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
                        { phAlias.Visible = true; }
                        else
                        {
                            if (!folder_data.AliasRequired)
                                phAlias.Visible = false;
                        }
                    }
                }
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
                { PhSchedule.Visible = true; }
                else
                {
                    PhSchedule.Visible = false;
                }
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
                { PhComment.Visible = true; }
                else
                {
                    PhComment.Visible = false;
                }
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
                { phTemplates.Visible = true; }
                else
                {
                    phTemplates.Visible = false;
                }
                if ((folder_data.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
                { if(phTaxonomy.Visible)
                    phTaxonomy.Visible = true; }
                else
                {
                    if (!folder_data.IsCategoryRequired)
                        phTaxonomy.Visible = false;
                }
            }

            //-------------------DisplayTabs Based on selected options from Folder properties End------------------------------
        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message));
        }
    }

    protected string GetFlaggingScript()
    {
        string returnValue;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        FolderFlagDefData aFlagSets = null;
        string selectedStr = "";

        try
        {
            // Display content flagging options:
            result.Append("<span style=\"position: relative; top: 0px; left: 5px;\" ><select id=\"FlaggingDefinitionSel\" name=\"FlaggingDefinitionSel\" >" + Environment.NewLine);

            aFlagSets = (new Ektron.Cms.Community.FlaggingAPI()).GetDefaultFolderFlagDef(folder_data.Id);
            if (aFlagSets == null)
            {
                result.Append("<option value=\"0\">None Available -HC</option>" + Environment.NewLine);
            }
            else
            {
                if (aFlagSets.ID > 0)
                {
                    selectedStr = "selected=\"selected\" ";
                }
                else
                {
                    selectedStr = "";
                }
                result.Append("<option value=\"" + aFlagSets.ID.ToString() + "\" " + selectedStr + ">" + aFlagSets.Name + "</option>" + Environment.NewLine);
            }
            result.Append("</select></span>" + Environment.NewLine);
            result.Append("<span style=\"position: relative; top: -1px; left: 5px;\" >Flagging</span>" + Environment.NewLine);

        }
        catch (Exception)
        {
        }
        finally
        {
            returnValue = result.ToString();
            result = null;
            aFlagSets = null;
        }
        return returnValue;
    }

    private void EditTaxonomyScript()
    {
        EditTaxonomyHtml.Text = "<div id=\"dvTaxonomy\">";
        EditTaxonomyHtml.Text += m_refMsg.GetMessage("select taxonomy label");
        EditTaxonomyHtml.Text += "  <div id=\"TreeOutput\" class=\"ektronTreeContainer\"></div>";
        //-- The following code displays Add hover effect while hovering over the taxonomy title --//
        EditTaxonomyHtml.Text += "  <div id=\"wamm_float_menu_block_menunode\" class=\"Menu\" onmouseout=\"wamm_float_menu_block_mouseout(this)\" onmouseover=\"wamm_float_menu_block_mouseover(this)\" style=\"position: absolute; left: 223px; top: 171px; z-index: 3200; display:none; height: 20px; \">";
        EditTaxonomyHtml.Text += "      <input type=\"hidden\" name=\"LastClickedParent\" id=\"LastClickedParent\" value=\"\" />";
        EditTaxonomyHtml.Text += "      <input type=\"hidden\" name=\"ClickRootCategory\" id=\"ClickRootCategory\" value=\"\false\" />";
        EditTaxonomyHtml.Text += "      <ul style=\"padding-top:10px;\">";
        EditTaxonomyHtml.Text += "          <li class=\"MenuItem add\">";
        EditTaxonomyHtml.Text += "              <a title=\"Route Action\" href=\"#\" onclick=\"routeAction(true, 'add');\">Add</a>";
        EditTaxonomyHtml.Text += "          </li>";
        EditTaxonomyHtml.Text += "      </ul>";
        EditTaxonomyHtml.Text += "  </div>";
        //-- Ends --//
        EditTaxonomyHtml.Text += "</div>";        

    }
    private void EditContentHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        string strAssetID = "";
        string strSnippet = "";
        string strPath = "";
        string editLiveCSS = "";
        long copyContID = 0;
        bool addFileUpload = false;
        string MenuItemType;

        if (content_stylesheet.Length > 0)
        {
            strPath = (string)(GetServerPath() + SitePath + content_stylesheet);
            editLiveCSS = (string)("&css=" + content_stylesheet);
        }
        if (!String.IsNullOrEmpty (Request.QueryString["content_id"]))
        {
            //this key is also used for media asset translated.
            copyContID = Convert.ToInt64(Request.QueryString["content_id"]);
        }

        isOfficeDoc.Value = "false";
        MultiupLoadTitleMsg.Text = "";
        content_title.Visible = true;
        this.type.Value = "";

        if ((IsMac) && !(Utilities.IsAsset(lContentType, strAssetID)))
        {
            if (content_edit_data != null && (content_edit_data.Type == 1 || content_edit_data.Type == 3) && (content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent))
            {
                string typeaction = Request.QueryString["type"];
                if ((typeaction != null) && typeaction.ToLower() == "update")
                {
                    isOfficeDoc.Value = "true";
                }
                HtmlGenericControl linebreak = new HtmlGenericControl("div");
                linebreak.InnerHtml = "<br /><br />";
                m_ctlContentPane.Controls.Add(linebreak);
                HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                htmlGen.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_edit_data.Html);
                m_ctlContentPane.Controls.Add(htmlGen);
            }
            else if ("ContentDesigner" == m_SelectedEditControl)
            {
                m_ctlContentDesigner.Visible = true;
                m_ctlContentDesigner.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                m_ctlContentDesigner.Height = new Unit(635, UnitType.Pixel);
                if (content_stylesheet.Length > 0)
                {
                    m_ctlContentDesigner.Stylesheet = strPath;
                }
                if (editorPackage.Length > 0)
                {
                    m_ctlContentDesigner.LoadPackage(m_refContApi, editorPackage);
                    m_ctlContentDesigner.DataDocumentXml = m_strContentHtml;
                }
                else
                {
                    m_ctlContentDesigner.Content = m_strContentHtml;
                }
                m_ctlContentValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength);
                m_ctlContentValidator.Visible = true;
            }
            else
            {
                if (!IsBrowserIE)
                {
                    sbHtml.Append("<input type=\"hidden\" name=\"ephox\" id=\"ephox\" value=\"true\">");
                    sbHtml.Append("<input type=\"hidden\" name=\"selectedtext\" id=\"selectedtext\">");
                    sbHtml.Append("<input type=\"hidden\" name=\"selectedhtml\" id=\"selectedhtml\">");
                    System.Text.StringBuilder strJSEditLive = new System.Text.StringBuilder();
                    strJSEditLive.Append("<script language=\"JavaScript\" src=\"" + this.AppeWebPath + "editlivejava/editlivejava.js\"></script>" + "\r\n");
                    sbHtml.Append("<input type=\"hidden\" name=\"EphoxContent\" id=\"EphoxContent\" value=\"" + EkFunctions.UrlEncode(Server.HtmlDecode(m_strContentHtml)) + "\">");
                    strJSEditLive.Append("<script language=\"JavaScript\">" + "\r\n");
                    strJSEditLive.Append("      var strContent;");
                    strJSEditLive.Append("		elx1 = new EditLiveJava(\"content_html\", \"700\", \"400\");");
                    strJSEditLive.Append("		elx1.setXMLURL(\"" + this.AppeWebPath + "editlivejava/config.aspx?apppath=" + this.AppPath + "&sitepath=" + this.SitePath + editLiveCSS + "\");");
                    strJSEditLive.Append("      elx1.setOutputCharset(\"UTF-8\");");
                    strJSEditLive.Append("		elx1.setBody(document.forms[0].EphoxContent.value);");
                    strJSEditLive.Append("		elx1.setDownloadDirectory(\"" + this.AppeWebPath + "editlivejava\");");
                    strJSEditLive.Append("		elx1.setLocalDeployment(false);");
                    strJSEditLive.Append("		elx1.setCookie(\"\");");
                    strJSEditLive.Append("		elx1.show();" + "\r\n");
                    strJSEditLive.Append("	</script>" + "\r\n");
                    sbHtml.Append(strJSEditLive.ToString());
                }
                else
                {
                    sbHtml.Append("<input type=\"hidden\" name=\"ephox\" id=\"ephox\" value=\"false\">");
                    sbHtml.Append("<textarea id=\"content_html\" name=\"content_html\" cols=\"90\" rows=\"24\" ID=\"Textarea2\">" + m_strContentHtml + "</textarea>");
                }
                Literal litSnippet = new Literal();
                litSnippet.ID = "ephox_control_literal";
                litSnippet.Text = sbHtml.ToString();
                m_ctlContentPane.Controls.Add(litSnippet);
            }
        }
        else
        {
            sbHtml.Append("<input type=\"hidden\" name=\"ephox\" id=\"ephox\" value=\"false\">");
            strAssetID = asset_info["AssetID"].ToString();
            if (content_edit_data != null && (content_edit_data.Type == 1 || content_edit_data.Type == 3) && (content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent))
            {
                string typeaction = Request.QueryString["type"];

                if ((typeaction != null) && typeaction.ToLower() == "update")
                {
                    isOfficeDoc.Value = "true";
                }
                HtmlGenericControl linebreak = new HtmlGenericControl("div");
                linebreak.InnerHtml = "<br /><br />";
                m_ctlContentPane.Controls.Add(linebreak);
                HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                htmlGen.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_edit_data.Html);
                m_ctlContentPane.Controls.Add(htmlGen);
            }
            else if (Utilities.IsAsset(lContentType, strAssetID))
            {
                if (m_strType == "multiple,add")
                {
                    bool isUrlAliasRequired = false;

                    FolderData fdTmp = this.m_refContApi.EkContentRef.GetFolderById(Int64.Parse(Request.QueryString["folderid"]));
                    Ektron.Cms.UrlAliasing.UrlAliasSettingsApi m_urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();

                    if (m_urlAliasSettings.IsManualAliasEnabled)
                    {
                        if (fdTmp.AliasRequired)
                        {
                            isUrlAliasRequired = true;
                        }
                    }


                    jsManualAliasAlert.Text = m_refMsg.GetMessage("js:url aliasing is required dms mupload");// "Url aliasing is required for this folder. Non-image assets will be uploaded but unpublished.";

                    if (Request.Cookies[DMSCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[DMSCookieName].Value))
                    {
                        if (Request.Cookies[DMSCookieName].Value == "2010")
                        {
                            if (isUrlAliasRequired)
                            {
                                jsfolderRequireManualAlias2010.Text = "true";
                            }

                            Button btnMupload = new Button();
                            //btnMupload.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                            btnMupload.ID = "btnMupload";
                            btnMupload.Text = "Upload";
                            btnMupload.Attributes.Add("onclick", "return DocumentUploadOffice2010()");
                            m_ctlContentPane.Controls.Add(btnMupload);
                        }
                        else
                        {
                            if (isUrlAliasRequired)
                            {
                                jsfolderRequireManualAlias.Text = "true";
                            }
                            HiddenField hfTmp = new HiddenField();
                            hfTmp.ID = "FromMenuMultiDMS";
                            hfTmp.Value = "";
                            m_ctlContentPane.Controls.Add(hfTmp);

                        }
                    }
                    

                    HtmlGenericControl linebreak = new HtmlGenericControl("div");
                    linebreak.InnerHtml += "<br /><br />" + "\r\n";
                    linebreak.InnerHtml += "<div id=idMultipleView style=\'display:none\'>";
                    linebreak.InnerHtml += "<script type=\"text/javascript\">" + "\r\n";
                    linebreak.InnerHtml += " AC_AX_RunContent(\'id\',\'idUploadCtl\',\'name\',\'idUploadCtl\',\'classid\',\'CLSID:07B06095-5687-4d13-9E32-12B4259C9813\',\'width\',\'100%\',\'height\',\'350px\');" + "\r\n";
                    linebreak.InnerHtml += "\r\n" + " </script> </div> " + "\r\n";
                    linebreak.InnerHtml += "<br /><br />";
                    linebreak.InnerHtml += "<div> " + m_refMsg.GetMessage("lbl valid file types") + DocumentManagerData.Instance.FileTypes + "</div>";
                    m_ctlContentPane.Controls.Add(linebreak);
                    strSnippet += "\r\n" + "<script language=\"JavaScript\">" + "\r\n";
                    strSnippet += "MultipleUploadView();" + "\r\n";
                    strSnippet += "\r\n" + "</script>";
                    content_title.Visible = false;
                    MultiupLoadTitleMsg.Text = m_refMsg.GetMessage("lbl msg for multiupload title");
                    this.type.Value = "multiple,add";
                }
                else if (strAssetID.Length == 0)
                {
                    HtmlGenericControl linebreak = new HtmlGenericControl("div");
                    linebreak.InnerHtml = "<br /><br />";
                    m_ctlContentPane.Controls.Add(linebreak);
                    HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                    htmlGen.InnerHtml =  m_refMsg.GetMessage("lbl upload file");
                    m_ctlContentPane.Controls.Add(htmlGen);
                    HtmlGenericControl fileUploadWrapper = new HtmlGenericControl("span");
                    fileUploadWrapper.ID = "fileUploadWrapper";
                    System.Web.UI.WebControls.FileUpload fileUpload = new System.Web.UI.WebControls.FileUpload();
                    fileUpload.ID = "fileupload";
                    fileUploadWrapper.Controls.Add(fileUpload);
                    m_ctlContentPane.Controls.Add(fileUploadWrapper);
                    oldfilename.Value = "";
                    addFileUpload = true;
                    this.type.Value = "add";
                }
                else
                {
                    if (lContentType == Ektron.Cms.Common.EkConstants.CMSContentType_Media)
                    {

                        Multimedia_commonparams mediaParams;
                        mediaParams = (Multimedia_commonparams)(LoadControl("controls/media/commonparams.ascx"));
                        mediaParams.ID = m_strContentTitle;
                        if (m_strType == "add" && copyContID != 0)
                        {
                            mediaParams.ContentHtml = this.m_refContent.CreateMediaXML(content_data.AssetData, m_intContentFolder);
                            mediaParams.AssetID = content_data.AssetData.Id;
                            mediaParams.MimeType = content_data.AssetData.MimeType;
                            mediaParams.AssetVersion = content_data.AssetData.Version;
                            mediaParams.AssetFileName = m_refContApi.GetViewUrl(content_data.AssetData.Id, Ektron.Cms.Common.EkConstants.CMSContentType_Media);
                        }
                        else
                        {
                            mediaParams.ContentHtml = m_strContentHtml;
                            mediaParams.AssetID = strAssetID;
                            mediaParams.MimeType = asset_info["MimeType"].ToString();
                            mediaParams.AssetVersion = asset_info["AssetVersion"].ToString();
                            mediaParams.AssetFileName = m_refContApi.RequestInformationRef.AssetPath + m_refContApi.EkContentRef.GetFolderParentFolderIdRecursive(content_edit_data.FolderId).Replace(",", "/") + "/" + content_edit_data.AssetData.Id + "." + content_edit_data.AssetData.FileExtension;
                            mediaParams.AssetFileName = (content_edit_data.IsPrivate ? m_refContApi.RequestInformationRef.SitePath + "PrivateAssets/" : m_refContApi.RequestInformationRef.AssetPath) + m_refContApi.EkContentRef.GetFolderParentFolderIdRecursive(content_edit_data.FolderId).Replace(",", "/") + "/" + content_edit_data.AssetData.Id + "." + content_edit_data.AssetData.FileExtension;
                        }

                        m_ctlContentPane.Controls.Add(mediaParams);
                    }
                    else
                    {
                        //check for type = 'add' here
                        if (m_strType == "add")
                        {
                            HtmlGenericControl linebreak = new HtmlGenericControl("div");
                            linebreak.InnerHtml = "<br /><br />";
                            m_ctlContentPane.Controls.Add(linebreak);
                            HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                            htmlGen.InnerHtml = m_refMsg.GetMessage("lbl upload file");
                            HtmlGenericControl fileUploadWrapper = new HtmlGenericControl("span");
                            fileUploadWrapper.ID = "fileUploadWrapper";
                            System.Web.UI.WebControls.FileUpload fileUpload = new System.Web.UI.WebControls.FileUpload();
                            fileUpload.ID = "fileupload";
                            fileUploadWrapper.Controls.Add(fileUpload);
                            m_ctlContentPane.Controls.Add(fileUploadWrapper);
                            oldfilename.Value = "";
                            addFileUpload = true;
                        }
                        else
                        {
                            if (Ektron.ASM.AssetConfig.ConfigManager.IsOfficeDoc(content_edit_data.AssetData.FileExtension))
                            {

                                AssetManagement.AssetManagementService assetmanagementService = new AssetManagement.AssetManagementService();
                                Ektron.ASM.AssetConfig.AssetData assetData = assetmanagementService.GetAssetData(content_edit_data.AssetData.Id);
                                string strfilename;
                                strfilename = (string)(GetFolderPath(content_edit_data.FolderId) + assetData.Handle);
                                filename.Value = strfilename;
                                HtmlGenericControl linebreak = new HtmlGenericControl("div");
                                linebreak.InnerHtml = "<br /><br /> Currently uploaded file: " + assetData.Handle + " <br /><br />";
                                m_ctlContentPane.Controls.Add(linebreak);
                                m_ctlContentPane.Controls.Add(linebreak);
                                HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                                htmlGen.InnerHtml = m_refMsg.GetMessage("lbl upload file");
                                m_ctlContentPane.Controls.Add(htmlGen);

                                HtmlGenericControl fileUploadWrapper = new HtmlGenericControl("span");
                                fileUploadWrapper.ID = "fileUploadWrapper";

                                System.Web.UI.WebControls.FileUpload fileUpload = new System.Web.UI.WebControls.FileUpload();
                                fileUpload.ID = "fileupload";

                                fileUploadWrapper.Controls.Add(fileUpload);
                                m_ctlContentPane.Controls.Add(fileUploadWrapper);

                                oldfilename.Value = assetData.Handle;
                                //This hidden field is used to hide content tab if office is not installed or browser is non-ie, else show content tab with browse button
                                isOfficeDoc.Value = "true";
                            }
                            else
                            {
                                AssetManagement.AssetManagementService assetmanagementService = new AssetManagement.AssetManagementService();
                                Ektron.ASM.AssetConfig.AssetData assetData = assetmanagementService.GetAssetData(asset_info["AssetID"].ToString());
                                HtmlGenericControl linebreak = new HtmlGenericControl("div");
                                linebreak.InnerHtml = "<br /><br /> Currently uploaded file: " + assetData.Handle + " <br /><br />";
                                m_ctlContentPane.Controls.Add(linebreak);
                                m_ctlContentPane.Controls.Add(linebreak);
                                HtmlGenericControl htmlGen = new HtmlGenericControl("span");
                                htmlGen.InnerHtml = m_refMsg.GetMessage("lbl upload file");
                                m_ctlContentPane.Controls.Add(htmlGen);

                                HtmlGenericControl fileUploadWrapper = new HtmlGenericControl("span");
                                fileUploadWrapper.ID = "fileUploadWrapper";

                                System.Web.UI.WebControls.FileUpload fileUpload = new System.Web.UI.WebControls.FileUpload();
                                fileUpload.ID = "fileupload";

                                fileUploadWrapper.Controls.Add(fileUpload);
                                m_ctlContentPane.Controls.Add(fileUploadWrapper);

                                oldfilename.Value = assetData.Handle;
                            }
                            MenuItemType = Request.QueryString["menuItemType"];
                            if ((MenuItemType != null) && MenuItemType.ToLower() == "editproperties")
                            {
                                isOfficeDoc.Value = "true";
                            }
                        }
                    }
                }
                sbHtml.Append(strSnippet);

                sbHtml.Append("<input type=\"hidden\" id=\"content_html\" name=\"content_html\" value=\"" + EkFunctions.HtmlEncode(m_strContentHtml) + "\">");

                //fix for 32909 - in case of Add multimedia file to Menu, lContentType is CMSContentType_Media but
                //since it is add we show the fileupload browse button, not the DragDropExplorer control
                if ((lContentType == Ektron.Cms.Common.EkConstants.CMSContentType_Media) && (addFileUpload == false) && m_strType == "update")
                {
                    HtmlGenericControl DragDropContainer = new HtmlGenericControl("div");
                    DragDropContainer.ID = "DragDropContainer";
                    DragDropContainer.Style.Add("width", "35%");
                    string multiUploadAssetID = "&AssetID=" + m_refContentId.ToString();
                    string multiUploadTaxString = string.Empty;
                    TaxonomyBaseData[] taxonomies = m_refContent.ReadAllAssignedCategory(m_refContentId);
                    if (!((taxonomies == null)) && taxonomies.Length > 0)
                    {
                        multiUploadTaxString = "&TaxonomyId=" + taxonomies[0].Id.ToString();
                    }
                    Literal uploaderSnippet = new Literal();
                    uploaderSnippet.ID = "UploadSnippet";
                    editaction.Value = m_strType;
                    uploaderSnippet.Text = m_refMsg.GetMessage("lbl upload file replace") + ": " + "<a href=\"" + m_refContApi.AppPath + "DragDropCtl.aspx?id=" + m_intContentFolder.ToString() + multiUploadAssetID + multiUploadTaxString + "&lang_id=" + m_intContentLanguage.ToString() + "&hideMultiple=true&forceExtension=true&EkTB_iframe=true&height=120&width=500&refreshCaller=true&scrolling=false&modal=true\" class=\"ek_thickbox\" title=\"" + m_refMsg.GetMessage("Document Management System") + "\"><img id=\"DeskTopHelp\" title= \"" + m_refMsg.GetMessage("alt add assets text") + "\" border=\"0\" src=\"images/UI/Icons/Import.png\"/></a>";
                    DragDropContainer.Controls.Add(uploaderSnippet);
                    m_ctlContentPane.Controls.Add(DragDropContainer);
                }
                Literal litSnippet = new Literal();
                litSnippet.ID = "ContentHtml";
                litSnippet.Text = sbHtml.ToString();
                m_ctlContentPane.Controls.Add(litSnippet);
            }
            else
            {
                if (m_strType == "add" && (content_data != null) && (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent))
                {
                    isOfficeDoc.Value = "true";
                }
                HtmlInputHidden ctlEphox = new HtmlInputHidden();
                ctlEphox.ID = "ephox";
                ctlEphox.Value = "false";
                m_ctlContentPane.Controls.Add(ctlEphox);

                if ("ContentDesigner" == m_SelectedEditControl)
                {
                    m_ctlContentDesigner.Visible = true;
                    m_ctlContentDesigner.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                    m_ctlContentDesigner.Height = new Unit(635, UnitType.Pixel);
                    if (content_stylesheet.Length > 0)
                    {
                        m_ctlContentDesigner.Stylesheet = strPath;
                    }
                    if (editorPackage.Length > 0)
                    {
                        m_ctlContentDesigner.LoadPackage(m_refContApi, editorPackage);
                        m_ctlContentDesigner.DataDocumentXml = m_strContentHtml;
                    }
                    else
                    {
                        m_ctlContentDesigner.Content = m_strContentHtml;
                    }
                    m_ctlContentValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength);
                    m_ctlContentValidator.Visible = true;
                }
                else
                {
                    Ektron.Cms.Controls.HtmlEditor ctlEditor = new Ektron.Cms.Controls.HtmlEditor();
                    ctlEditor.WorkareaMode(2);
                    ctlEditor.ID = "content_html";
                    ctlEditor.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                    ctlEditor.Height = new Unit(100, UnitType.Percentage);
                    ctlEditor.Path = AppeWebPath;
                    ctlEditor.MaxContentSize = iMaxContLength;
                    ctlEditor.Locale = AppeWebPath + "locale" + AppLocaleString + "b.xml";
                    if (editorPackage.Length > 0)
                    {
                        Ektron.WebEditorNet2.eWebEditProField objField;
                        objField = new Ektron.WebEditorNet2.eWebEditProField();
                        objField.Name = "datadesignpackage";
                        objField.SetContentType = "datadesignpackage";
                        objField.GetContentType = "";
                        objField.Text = editorPackage;
                        ctlEditor.Fields.Add(objField);

                        objField = new Ektron.WebEditorNet2.eWebEditProField();
                        objField.Name = "datadocumentxml";
                        objField.SetContentType = "datadocumentxml";
                        objField.GetContentType = ""; // content is retrieved manually
                        objField.Text = m_strContentHtml;
                        ctlEditor.Fields.Add(objField);
                        objField = null;
                    }
                    else
                    {
                        ctlEditor.Text = m_strContentHtml;
                    }
                    System.Web.UI.HtmlControls.HtmlGenericControl eWebEditProWrapper = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                    eWebEditProWrapper.Attributes.Add("class", "ewebeditproWrapper ewebeditpro_dvContent");
                    eWebEditProWrapper.Controls.Add(ctlEditor);
                    m_ctlContentPane.Controls.Add(eWebEditProWrapper);
                }
            }
        }
        // fix for Defect: #43308, why output this tab if you are jsut going to hide it?  We always hide it for
        //   office docs, so...IE ONLY
        //if (isOfficeDoc.Value == "true" && IsBrowserIE)
        //{
        //    phContent.Visible = false;
        //    phEditContent.Visible = false;
        //}

		  if (!String.IsNullOrEmpty(m_strContentHtml))
		  {
			  if (m_ctlContentDesigner != null)
			  {
				  m_ctlContentDesigner.CurrentCharCount = m_strContentHtml.Length;

				  if (m_ctlContentDesigner.CurrentCharCount > m_ctlContentDesigner.WordPasteThreshold)
				  {
					  m_ctlContentDesigner.ShowPasteWarning = false;
					  
					  WarningMessage.Visible = true;
					  WarningMessage.Text = m_refMsg.GetMessage("WordPasteWarningMessage");
				  }
			  }
		  }
    }

	private string EncodeJavascriptString(string str)
    {
        string result;
        result = str.Replace("\'", "\\\'");
        result = result.Replace("\"", "\\\"");
        return result;
    }

    private void EditSummaryHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        bool bIsRedirect = false;
        bool bIsTransfer = false;
        string editLiveCSS = "";
        bool bIsReport = false;
        int iRptDisplayType = 1; //default to same window "_self"
        int iRptType = 4; //default to combine bar with percent
        bool bPureRedirect = false;
        bool bIsForm = false;

        if (bIsFormDesign)
        {
            bIsRedirect = (content_teaser.IndexOf("<RedirectionLink") > -1);
            if (bIsRedirect)
            {
                bIsTransfer = (content_teaser.IndexOf("EktForwardFormData") > -1);
                bIsReport = (content_teaser.IndexOf("EktReportFormData") > -1);
                if (bIsReport)
                {
                    //find out the setting for reports.
                    if ((content_teaser.IndexOf(" target") > -1) && (content_teaser.IndexOf("_self") > -1))
                    {
                        iRptDisplayType = 1;
                    }
                    else
                    {
                        iRptDisplayType = 0;
                    }
                    int iPos;
                    int SPos;
                    string sHolder;
                    string sRptType = "";
                    SPos = content_teaser.IndexOf(" id=\""); // 5 char
                    if (SPos > 0)
                    {
                        for (iPos = SPos + 5; iPos <= content_teaser.Length; iPos++)
                        {
                            sHolder = content_teaser.Substring(iPos + 1 - 1, 1);
                            if (sHolder != "\"")
                            {
                                sRptType = sRptType + sHolder;
                            }
                            else
                            {
                                break;
                            }
                        }
                        iRptType = Convert.ToInt16(sRptType);
                    }
                }
                bPureRedirect = true;
                if (bIsTransfer)
                {
                    bPureRedirect = false;
                }
                else if (bIsReport)
                {
                    bPureRedirect = false;
                }
            }
        }

        if (bNewPoll)
        {
            //default report response for poll.
            bIsRedirect = true;
            bIsTransfer = false;
            bIsReport = true;
        }

        string strPath = "";
        if (content_stylesheet.Length > 0)
        {
            strPath = (string)(GetServerPath() + SitePath + content_stylesheet);
            editLiveCSS = (string)("&css=" + content_stylesheet);
        }
        //build head of table if blog
        if (m_bIsBlog)
        {
            sbHtml.Append("<table cellpadding=\"4\">");
            sbHtml.Append("	<tr>");
            sbHtml.Append("		<td width=\"20\">&nbsp;</td>");
            sbHtml.Append("		<td valign=\"top\">");
            sbHtml.Append("			<b>" + m_refMsg.GetMessage("generic description") + "</b>");
            sbHtml.Append("		</td>");
            sbHtml.Append("		<td width=\"20\">&nbsp;</td>");
            sbHtml.Append("		<td valign=\"top\">&nbsp;");
            sbHtml.Append("		</td>");
            sbHtml.Append("	</tr>");
            sbHtml.Append("	<tr>");
            sbHtml.Append("		<td width=\"20\">&nbsp;</td>");
            sbHtml.Append("		<td valign=\"top\">");
        }

        if ("ContentDesigner" == m_SelectedEditControl)
        {
            if (!(content_edit_data == null))
            {
                if (content_edit_data.Type == 2 || 4 == content_edit_data.Type)
                {
                    bIsForm = true;
                }
            }
            m_ctlSummaryDesigner.Visible = true;
            if (m_bIsBlog)
            {
                m_ctlSummaryDesigner.Width = new Unit(484, UnitType.Pixel);
                m_ctlSummaryDesigner.Height = new Unit(200, UnitType.Pixel);
            }
            else
            {
                m_ctlSummaryDesigner.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                m_ctlSummaryDesigner.Height = new Unit(635, UnitType.Pixel);
            }
            if (content_stylesheet.Length > 0)
            {
                m_ctlSummaryDesigner.Stylesheet = strPath;
            }
            if (bIsForm == true)
            {
                if (content_teaser.IndexOf("<ektdesignpackage_forms>") > -1)
                {
                    m_ctlSummaryDesigner.Content = m_refContApi.TransformXsltPackage(content_teaser, Server.MapPath((string)(m_ctlSummaryDesigner.ScriptLocation + "unpackageDesign.xslt")), true);
                    if ("" == m_ctlSummaryDesigner.Content)
                    {
                        m_ctlSummaryDesigner.Content = "<p>" + m_refMsg.GetMessageForLanguage("lbl place post back message here", m_intContentLanguage) + "</p>";
                    }
                }
                else //new form response, no packages
                {
                    m_ctlSummaryDesigner.Content = content_teaser;
                }
            }
            else
            {
                m_ctlSummaryDesigner.Content = content_teaser;
            }
            m_ctlSummaryValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxSummLength);
            m_ctlSummaryValidator.Visible = true;
            if (bIsForm == true)
            {
                m_ctlSummaryDesigner.Height = new Unit(450, UnitType.Pixel);
                m_ctlFormResponseRedirect.Visible = true;
                m_ctlFormResponseRedirect.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                m_ctlFormResponseRedirect.Height = new Unit(200, UnitType.Pixel);
                if (content_stylesheet.Length > 0)
                {
                    m_ctlFormResponseRedirect.Stylesheet = strPath;
                }
                m_ctlFormResponseRedirect.DataEntryXslt = GenerateRedirectionPageXslt("Redirect");
                m_ctlFormResponseRedirect.DataSchema = "";
                m_ctlFormResponseRedirect.DataDocumentXml = content_teaser;
                m_ctlFormResponseTransfer.Visible = true;
                m_ctlFormResponseTransfer.Width = new Unit(editorWidthUnits, editorWidthUnitType);
                m_ctlFormResponseTransfer.Height = new Unit(200, UnitType.Pixel);
                if (content_stylesheet.Length > 0)
                {
                    m_ctlFormResponseTransfer.Stylesheet = strPath;
                }
                m_ctlFormResponseTransfer.DataEntryXslt = GenerateRedirectionPageXslt("Transfer");
                m_ctlFormResponseTransfer.DataSchema = "";
                m_ctlFormResponseTransfer.DataDocumentXml = content_teaser;
                m_ctlFormSummaryReport.Visible = true;
            }
            else
            {
                m_ctlFormResponseRedirect.Visible = false;
                m_ctlFormResponseTransfer.Visible = false;
                m_ctlFormSummaryReport.Visible = false;
            }
        }

        if (m_SelectedEditControl == "ContentDesigner")
        {
            // Because ContentDesigner is now in a user control, the name and id are different
            sbHtml.Append("<input type=\"hidden\" name=\"content_teaser\" id=\"content_teaser\" value=\"\">");
        }
        if (IsMac && m_SelectedEditControl != "ContentDesigner")
        {
            if (!IsBrowserIE)
            {
                sbHtml.Append("<input type=\"hidden\" name=\"selectedtext\" id=\"selectedtext\">");
                sbHtml.Append("<input type=\"hidden\" name=\"selectedhtml\" id=\"selectedhtml\">");
                System.Text.StringBuilder strJSEditLive = new System.Text.StringBuilder();
                sbHtml.Append("<input type=\"hidden\" name=\"content_teaser\" id=\"content_teaser\" value=\"\">");
                sbHtml.Append("<input type=\"hidden\" name=\"EphoxTeaser\" id=\"EphoxTeaser\" value=\"" + EkFunctions.UrlEncode(Server.HtmlDecode(content_teaser)) + "\">");
                strJSEditLive.Append("<script language=\"JavaScript\">" + "\r\n");
                strJSEditLive.Append("      var strContent;");
                if (m_bIsBlog)
                {
                    strJSEditLive.Append("		elx2 = new EditLiveJava(\"content_teaser22\", \"484\", \"200\");");
                }
                else
                {
                    strJSEditLive.Append("		elx2 = new EditLiveJava(\"content_teaser22\", \"700\", \"400\");");
                }
                strJSEditLive.Append("		elx2.setXMLURL(\"" + this.AppeWebPath + "editlivejava/config.aspx?apppath=" + this.AppPath + "&sitepath=" + this.SitePath + editLiveCSS + "\");");
                strJSEditLive.Append("      elx2.setOutputCharset(\"UTF-8\");");
                strJSEditLive.Append("		elx2.setBody(document.forms[0].EphoxTeaser.value);");
                strJSEditLive.Append("		elx2.setDownloadDirectory(\"" + this.AppeWebPath + "editlivejava\");");
                strJSEditLive.Append("		elx2.setLocalDeployment(false);");
                strJSEditLive.Append("		elx2.setCookie(\"\");");
                strJSEditLive.Append("		elx2.show();" + "\r\n");
                strJSEditLive.Append("	</script>" + "\r\n");
                sbHtml.Append(strJSEditLive.ToString());
            }
            else
            {
                sbHtml.Append("<textarea name=\"content_teaser\" cols=\"90\" rows=\"24\" ID=\"Textarea3\">" + EkFunctions.HtmlEncode(content_teaser) + "</textarea>");
            }
        }
        else
        {
            if (bIsFormDesign)
            {
                sbHtml.Append("<p>" + "\r\n");

                // Display a message
                sbHtml.Append("<input type=\"radio\" id=\"response_message\" name=\"response\" value=\"message\"");
                if (!bIsRedirect)
                {
                    sbHtml.Append(" checked=\"checked\"");
                    initialSummaryPane.Text = "message";
                }
                sbHtml.Append(" onclick=\"setResponseAction(\'message\')\" disabled /><label id=\"lbl_response_message\" for=\"response_message\" disabled>&#160;" + m_refMsg.GetMessage("lbl display a message") + "</label><br />" + "\r\n");

                // Redirect to a file or page
                sbHtml.Append("<input type=\"radio\" id=\"response_redirect\" name=\"response\" value=\"redirect\"");
                if (bPureRedirect)
                {
                    sbHtml.Append(" checked=\"checked\"");
                    initialSummaryPane.Text = "redirect";
                }
                sbHtml.Append(" onclick=\"setResponseAction(\'redirect\')\" disabled /><label id=\"lbl_response_redirect\" for=\"response_redirect\" disabled>&#160;" + m_refMsg.GetMessage("lbl redirect to a file or page") + "</label><br />" + "\r\n");

                // Redirect form data to an action page
                sbHtml.Append("<input type=\"radio\" id=\"response_transfer\" name=\"response\" value=\"transfer\"");
                if (bIsTransfer)
                {
                    sbHtml.Append(" checked=\"checked\"");
                    initialSummaryPane.Text = "transfer";
                }
                sbHtml.Append(" onclick=\"setResponseAction(\'transfer\')\" disabled /><label id=\"lbl_response_transfer\" for=\"response_transfer\" disabled>&#160;" + m_refMsg.GetMessage("lbl redirect form data to an action page") + "</label><br />" + "\r\n");

                // Report on the form
                sbHtml.Append("<input type=\"radio\" id=\"response_report\" name=\"response\" value=\"report\"");
                if (bIsReport)
                {
                    sbHtml.Append(" checked=\"checked\"");
                    initialSummaryPane.Text = "report";
                }
                sbHtml.Append(" onclick=\"showReportOptions()\" disabled /><label id=\"lbl_response_report\" for=\"response_report\" disabled>&#160;" + m_refMsg.GetMessage("lbl report on the form") + "</label>" + "\r\n");


                sbHtml.Append("&nbsp;&nbsp;<SELECT onchange=\"setReportOptions(\'rptDisplayType\')\" id=rptDisplayType name=\"report_display_type\"" + "\r\n");
                if (bIsReport == false)
                {
                    sbHtml.Append(" disabled");
                }
                sbHtml.Append(">" + "\r\n");
                sbHtml.Append("<OPTION value=\"1\"" + "\r\n");
                if (iRptDisplayType == 1)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl same window") + "</OPTION>" + "\r\n");
                sbHtml.Append("<OPTION value=\"0\"" + "\r\n");
                if (iRptDisplayType == 0)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl new window") + "</OPTION>" + "\r\n");
                sbHtml.Append("</SELECT>" + "\r\n");
                sbHtml.Append("&nbsp;&nbsp;&nbsp;<SELECT name=\"report_type\" onchange=\"setReportOptions(\'rptType\')\" id=\"rptType\"" + "\r\n");
                if (bIsReport == false)
                {
                    sbHtml.Append(" disabled");
                }
                sbHtml.Append(">" + "\r\n");
                sbHtml.Append("<OPTION value=\"1\"" + "\r\n");
                if (iRptType == 1)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl data table") + "</OPTION>" + "\r\n");
                sbHtml.Append("<OPTION value=\"2\"" + "\r\n");
                if (iRptType == 2)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl bar chart") + "</OPTION>" + "\r\n");
                sbHtml.Append("<OPTION value=\"3\"" + "\r\n");
                if (iRptType == 3)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl pie chart") + "</OPTION>" + "\r\n");
                sbHtml.Append("<OPTION value=\"4\"" + "\r\n");
                if (iRptType == 4)
                {
                    sbHtml.Append(" selected=\"selected\"");
                }
                sbHtml.Append(">" + m_refMsg.GetMessage("lbl combined") + "</OPTION>" + "\r\n");
                sbHtml.Append("</SELECT>" + "\r\n");
                sbHtml.Append("</p>" + "\r\n");
            }
            if (m_SelectedEditControl != "ContentDesigner")
            {
                sbHtml.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
                sbHtml.Append("<!--" + "\r\n");
                sbHtml.Append("editorEstimateContentSize = false;" + "\r\n");
                sbHtml.Append("eWebEditPro.parameters.reset();" + "\r\n");
                sbHtml.Append("eWebEditPro.parameters.baseURL = \"" + SitePath + "\";" + "\r\n");
                sbHtml.Append("eWebEditPro.parameters.locale = \"" + AppeWebPath + "locale" + AppLocaleString + "b.xml\";" + "\r\n");
                if (bIsFormDesign)
                {
                    if (bIsRedirect)
                    {
                        sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?mode=dataentry&InterfaceName=none\";" + "\r\n");
                    }
                    else
                    {
                        sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?mode=xsltdesign\";" + "\r\n");
                    }
                    sbHtml.Append("eWebEditPro.parameters.editorGetMethod = \"getDocument\";" + "\r\n");
                }
                else
                {
                    sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?wiki=0\";" + "\r\n");
                }
                sbHtml.Append("eWebEditPro.parameters.maxContentSize = " + iMaxSummLength + "; " + "\r\n");
                sbHtml.Append("eWebEditPro.parameters.xmlInfo =\'\';" + "\r\n");
                if (content_stylesheet.Length > 0)
                {

                    sbHtml.Append("eWebEditPro.parameters.styleSheet = \"" + strPath + "\"; " + "\r\n");
                }
                sbHtml.Append("//-->" + "\r\n");
                sbHtml.Append("</script>" + "\r\n");
            }
            if (bIsFormDesign)
            {
                string strRedirectionData;
                if (bIsRedirect)
                {
                    strRedirectionData = content_teaser;
                    content_teaser = "<p>" + m_refMsg.GetMessageForLanguage("lbl place post back message here", m_intContentLanguage) + "</p>";
                }
                else
                {
                    strRedirectionData = "<root><RedirectionLink></RedirectionLink></root>";
                }
                if (m_SelectedEditControl != "ContentDesigner")
                {
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "transfer_page", (string)(bIsTransfer ? "dataentryxslt" : ""), "", GenerateRedirectionPageXslt("Transfer")));
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_page", (string)(bPureRedirect ? "dataentryxslt" : ""), "", GenerateRedirectionPageXslt("Redirect")));
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "report_page", (string)(bIsReport ? "dataentryxslt" : ""), "", GenerateRedirectionPageXslt("Report")));
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_data", "datadocumentxml", (string)(bIsReport ? "" : "datadocumentxml"), strRedirectionData));
                }
                else
                {
                    m_ctlSummaryDesigner.Content = content_teaser;
                }
                if (bIsRedirect)
                {
                    // NOTE:If redirecting, then change the field name after loaded so the content will be saved to content_teaser.
                    sbHtml.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
                    sbHtml.Append("<!--" + "\r\n");
                    sbHtml.Append("g_prevResponseAction = \"" + (bIsTransfer ? "transfer" : (bIsReport ? "report" : "redirect")) + "\"" + "\r\n");
                    if (m_SelectedEditControl != "ContentDesigner")
                    {
                        sbHtml.Append("eWebEditPro.addEventHandler(\"onload\", \'changeFieldName(\"redirection_data\", \"content_teaser\")\');" + "\r\n");
                    }
                    sbHtml.Append("// -->" + "\r\n");
                    sbHtml.Append("</script>" + "\r\n");
                }
                else
                {
                    // NOTE
                    // If not redirecting, then mask the fact that these two fields are defined.
                    sbHtml.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\">" + "\r\n");
                    sbHtml.Append("<!--" + "\r\n");
                    sbHtml.Append("g_prevResponseAction = \"message\"" + "\r\n");
                    if (m_SelectedEditControl != "ContentDesigner")
                    {
                        sbHtml.Append("Ektron.ready(function()  {changeEditorNameOfFields(\"content_teaser\", \"not_redirect\");});" + "\r\n");
                    }
                    sbHtml.Append("// -->" + "\r\n");
                    sbHtml.Append("</script>" + "\r\n");
                }
            }

            if (m_SelectedEditControl != "ContentDesigner")
            {
                string EditorWidth;
                string EditorHeight;
                if (m_bIsBlog)
                {
                    EditorWidth = "484";
                    EditorHeight = "200";
                }
                else
                {
                    EditorWidth = "100%";
                    EditorHeight = "100%";
                }
                sbHtml.Append("<div class=\"ewebeditproWrapper ewebeditpro_dvSummary\">");
                sbHtml.Append(Utilities.eWebEditProEditor("content_teaser", EditorWidth, EditorHeight, content_teaser));
                sbHtml.Append("</div>");
            }
        }

        Literal litSnippet = new Literal();
        litSnippet.ID = "TeaserHtml";
        litSnippet.Text = sbHtml.ToString();
        m_ctlSummaryPane.Controls.AddAt(0, litSnippet); // place above the ContentDesigner editor
        if (m_bIsBlog)
        {
            Literal litBlogSnippet = new Literal(); //if litSnippet is re-used here, the page layout is messed up.
            sbHtml.Length = 0;
            AddBlogItems(sbHtml);
            litBlogSnippet.Text = sbHtml.ToString();
            m_ctlSummaryPane.Controls.Add(litBlogSnippet); // place below the ContentDesigner editor
        }
    }

    private void AddBlogItems(System.Text.StringBuilder sbHtml)
    {
        string[] arrBlogPostCategories;
        int i = 0;

        sbHtml.Append("			<br/><br/>");
        sbHtml.Append("			<b>" + m_refMsg.GetMessage("lbl tags") + "</b> (" + m_refMsg.GetMessage("lbl enter multiple tags") + ")");
        sbHtml.Append("			<br/>");
        if (!(blog_post_data == null))
        {
            sbHtml.Append("			<textarea cols=\"58\" rows=\"5\" id=\"blogposttags\" name=\"blogposttags\">" + blog_post_data.Tags + "</textarea><input type=\"hidden\" name=\"blogposttagsid\" id=\"blogposttagsid\" value=\"" + blog_post_data.TagsID + "\" />");
        }
        else
        {
            sbHtml.Append("			<textarea cols=\"58\" rows=\"5\" id=\"blogposttags\" name=\"blogposttags\"></textarea>");
        }
        sbHtml.Append("	<p>");
        if (!(blog_data.Categories == null) && blog_data.Categories.Length > 0)
        {
            sbHtml.Append("			<br><br><b>" + m_refMsg.GetMessage("lbl blog cat") + "</b><br/>");

            if (!(blog_post_data == null) && !(blog_post_data.Categories == null))
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
            if (blog_data.Categories.Length > 0)
            {
                for (i = 0; i <= (blog_data.Categories.Length - 1); i++)
                {
                    if (blog_data.Categories[i].ToString() != "")
                    {
                        if (!(arrBlogPostCategories == null) && Array.BinarySearch(arrBlogPostCategories, blog_data.Categories[i].ToString()) > -1)
                        {
                            sbHtml.Append("				<input type=\"checkbox\" name=\"blogcategories" + i.ToString() + "\" value=\"" + Strings.Replace((string)(blog_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "\" checked=\"true\">&nbsp;" + Strings.Replace((string)(blog_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "<br />");
                        }
                        else
                        {
                            sbHtml.Append("				<input type=\"checkbox\" name=\"blogcategories" + i.ToString() + "\" value=\"" + Strings.Replace((string)(blog_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "\">&nbsp;" + Strings.Replace((string)(blog_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "<br />");
                        }
                    }
                }
                sbHtml.Append("<input type=\"hidden\" name=\"blogpostcatlen\" id=\"blogpostcatlen\" value=\"" + blog_data.Categories.Length.ToString() + "\"/>");
            }
            else
            {
                sbHtml.Append("No categories defined.<input type=\"hidden\" name=\"blogpostcatlen\" id=\"blogpostcatlen\" value=\"0\"/>");
            }
        }
        sbHtml.Append("<input type=\"hidden\" name=\"blogpostcatid\" id=\"blogpostcatid\" value=\"" + blog_post_data.CategoryID.ToString() + "\" />");
        sbHtml.Append("		</td>");

        sbHtml.Append("		</td>");
        sbHtml.Append("		<td width=\"20\">&nbsp;</td>");
        sbHtml.Append("		<td valign=\"top\" style=\"border: 1px solid #fffff; \">");
        sbHtml.Append("	</tr>");

        sbHtml.Append("	<tr>");
        sbHtml.Append("		<td width=\"20\">&nbsp;</td>");
        sbHtml.Append("		<td valign=\"top\" colspan=\"2\">");
        sbHtml.Append("     ").Append(GetBlogControls());
        sbHtml.Append("		</td>");
        sbHtml.Append("	</tr>");

        sbHtml.Append("</table>");
    }

    private void PollHtmlScript()
    {
        StringBuilder sbHtml = new StringBuilder();
        int idx;
        sbHtml.Append("<input type=\"hidden\" name=\"numPollChoices\" id=\"numPollChoices\" value=\"" + nPollChoices + "\" />" + "\r\n");
        sbHtml.Append("<div id=\"_dvPollWizard\" style=\"position: absolute;\">" + "\r\n");
        sbHtml.Append("<table width=\"100%\" cellspacing=\"0\" cellpadding=\"5\">" + "\r\n");
        sbHtml.Append("<tr>" + "\r\n");
        sbHtml.Append("	<td></td>" + "\r\n");
        sbHtml.Append("	<td colspan=\"2\">Question:</td>" + "\r\n");
        sbHtml.Append("</tr>" + "\r\n");
        sbHtml.Append("<tr>" + "\r\n");
        sbHtml.Append("	<td colspan=\"2\"></td>" + "\r\n");
        sbHtml.Append("	<td><input name=\"frm_Question\" id=\"frm_Question\" type=\"text\" runat=\"server\" style=\"width: 717px\" maxlength=\"1000\" /></td>" + "\r\n");
        sbHtml.Append("</tr>" + "\r\n");
        sbHtml.Append("<tr>" + "\r\n");
        sbHtml.Append("	<td></td>" + "\r\n");
        sbHtml.Append("	<td colspan=\"2\">Choices:</td>" + "\r\n");
        sbHtml.Append("</tr>" + "\r\n");
        for (idx = 1; idx <= nPollChoices; idx++)
        {
            sbHtml.Append("<tr>" + "\r\n");
            sbHtml.Append("	<td></td>" + "\r\n");
            sbHtml.Append("	<td>" + idx + ".</td>" + "\r\n");
            sbHtml.Append("	<td><input name=\"frm_Choice" + idx + "\" id=\"frm_Choice" + idx + "\" type=\"text\" runat=\"server\" maxlength=\"50\" /></td>" + "\r\n");
            sbHtml.Append("</tr>" + "\r\n");
        }
        sbHtml.Append("</table>" + "\r\n");
        sbHtml.Append("</div>");
        sbHtml.Append("<input type=\"hidden\" name=\"renewpoll\" value=\"" + bReNewPoll + "\" />");
        PollPaneHtml.Text = sbHtml.ToString();
    }
    private string GenerateRedirectionPageXslt(string TransferType)
    {
        StringBuilder sbRedirectionPage = new StringBuilder();
        // TODO localize these strings
        string strCaption = "File or page";
        string strCannotBeBlank = "Cannot be blank";
        string strTransferable = "To redirect and forward form data to another page, " + "the following requirements must be met:" + "\\n  1. The page must be an .aspx page." + "\\n  2. The page must be within the same web application."; // no single or double quotes. Also, no < or > or & (unless HTML encoded)
        string strSelect = "Select";
        sbRedirectionPage.Append("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" + "\r\n");
        sbRedirectionPage.Append("<xsl:output method=\"xml\" version=\"1.0\" encoding=\"UTF-8\" indent=\"yes\" omit-xml-declaration=\"yes\" />" + "\r\n");
        sbRedirectionPage.Append("<xsl:strip-space elements=\"*\" />" + "\r\n");
        sbRedirectionPage.Append("<xsl:template match=\"/\" xml:space=\"preserve\">" + "\r\n");
        if (TransferType != "Report")
        {
            sbRedirectionPage.Append("<div style=\"padding:3px; color:black; background-color:white; font-family:Verdana, Geneva, Arial, Helvetica, sans-serif; font-size:x-small;\">" + "\r\n");
            if (TransferType == "Transfer")
            {
                sbRedirectionPage.Append("<input type=\"hidden\" name=\"EktForwardFormData\" value=\"\" ektdesignns_name=\"EktForwardFormData\" ektdesignns_nodetype=\"element\" />");
            }
            sbRedirectionPage.Append("<label for=\"RedirectionLink\">" + strCaption + ":</label>" + "\r\n");
            sbRedirectionPage.Append("<span class=\"design_filelink\" ektdesignns_content=\"element=a\" id=\"RedirectionLink\" ektdesignns_name=\"RedirectionLink\" title=\"" + strCaption + "\" ektdesignns_nodetype=\"element\"");
            if (TransferType == "Transfer")
            {
                string strDomain;
                string strSitePath;
                strDomain = m_refContApi.FullyQualifyURL("/");
                strDomain = Ektron.Cms.API.JS.EscapeRegExp(strDomain);
                strSitePath = m_refContApi.SitePath;
                if (strSitePath.StartsWith("/"))
                {
                    strSitePath = strSitePath.Substring(1); // strip leading "/"
                }
                strSitePath = Ektron.Cms.API.JS.EscapeRegExp(strSitePath);
                // content-req is special for design.js validation
                sbRedirectionPage.Append(" ektdesignns_validation=\"content-req\" onblur=\"design_validate_re(/&lt;A.*href\\s*=\\s*[\\x22\\x27\\s](" + strDomain + ")?\\/?" + strSitePath + ".*\\.aspx[\\?\\x22\\x27\\s]/i,this,\'" + strTransferable + "\');\">" + "\r\n");
            }
            else
            {
                sbRedirectionPage.Append(" ektdesignns_validation=\"content-req\" onblur=\"design_validate_re(/&lt;A/i,this,\'" + strCannotBeBlank + "\');\">" + "\r\n");
            }
            sbRedirectionPage.Append("<xsl:copy-of select=\"/root/RedirectionLink/node()\" />&#160;<img class=\"design_fieldbutton\" height=\"16\" alt=\"" + strSelect + "\" src=\"[srcpath]btnfilelink.gif\" width=\"16\" unselectable=\"on\" />" + "\r\n");
            sbRedirectionPage.Append("</span> &#160;" + "\r\n");
            sbRedirectionPage.Append("</div>" + "\r\n");
        }
        sbRedirectionPage.Append("</xsl:template>" + "\r\n");
        sbRedirectionPage.Append("</xsl:stylesheet>" + "\r\n");
        return sbRedirectionPage.ToString();
    }

    public bool IsSiteMultilingual
    {
        get
        {
            LanguageData[] languageDataArray = m_refSiteApi.GetAllActiveLanguages();
            UserAPI m_refUserApi = new UserAPI();
            if (m_refUserApi.EnableMultilingual == 0)
            {
                return false;
            }
            int languageEnabledCount = 0;
            foreach (LanguageData lang in languageDataArray)
            {
                if (lang.SiteEnabled)
                {
                    languageEnabledCount++;
                }
                if (languageEnabledCount > 1)
                {
                    break;
                }
            }

            return languageEnabledCount > 1;
        }

    }

    private string GetLanguageDropDownMarkup(string controlId)
    {

        int i;
        StringBuilder markup = new StringBuilder();
        ContentAPI m_refContentApi = new ContentAPI();

        if (IsSiteMultilingual)
        {
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\">");
            LanguageData[] languageDataArray = m_refSiteApi.GetAllActiveLanguages();
            if (!(languageDataArray == null))
            {
                for (i = 0; i <= languageDataArray.Length - 1; i++)
                {
                    if (languageDataArray[i].SiteEnabled)
                    {
                        markup.Append("<option ");
                        if (m_intContentLanguage == languageDataArray[i].Id)
                        {
                            markup.Append(" selected");
                        }
                        markup.Append(" value=" + languageDataArray[i].Id + ">" + languageDataArray[i].LocalName);
                    }
                }
            }
            markup.Append("</select>");
        }
        else
        {
            //hardcode to default site language
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\" >");
            markup.Append(" <option selected value=" + m_intContentLanguage + ">");
            markup.Append("</select>");
        }

        return markup.ToString();
    }

    private string GetTagDisplayHTML(long contentId)
    {
        string returnValue;
        // add content Tags section
        // display tag edit area
        System.Text.StringBuilder taghtml = new System.Text.StringBuilder();
        error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag");
        error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars");

        Hashtable htTagsAssignedToUser = new Hashtable();
        taghtml.Append("<div class=\"ektronTopSpace\"></div>");

        taghtml.Append("<div style=\"height:115px;\">");
        taghtml.Append("<div id=\"newTagNameDiv\" class=\"ektronWindow\">");
        taghtml.Append("    <table class=\"ektronForm\">");
        taghtml.Append("        <tr>");
        taghtml.Append("            <td class=\"label\">");
        taghtml.Append(m_refMsg.GetMessage("name label"));
        taghtml.Append("            </td>");
        taghtml.Append("            <td class=\"value\">");
        taghtml.Append("                <input type=\"text\" id=\"newTagName\" value=\"\" style=\"width:275px;\" size=\"25\" onkeypress=\"if (event && event.keyCode && (13 == event.keyCode)) {SaveNewPersonalTag(); return false;}\" />");
        taghtml.Append("            </td>");
        taghtml.Append("        </tr>");
        taghtml.Append("    </table>");

        if (IsSiteMultilingual)
        {
            taghtml.Append("<div style=\"display:none;\" >");
        }
        else
        {
            taghtml.Append("<div style=\"display:none;\" >");
        }
        taghtml.Append(m_refMsg.GetMessage("res_lngsel_lbl") + "&#160;" + GetLanguageDropDownMarkup("TagLanguage"));
        taghtml.Append("    </div>");

        taghtml.Append("<div style=\"margin-top:.5em;\">");
        taghtml.Append("    <ul class=\"buttonWrapper ui-helper-clearfix\">");
        taghtml.Append("        <li>");
        taghtml.Append("            <a style=\'margin-right: 14px;\' class=\"button redHover buttonClear buttonRight\" type=\"button\" alt=\"" + m_refMsg.GetMessage("btn cancel") + "\" title=\"" + m_refMsg.GetMessage("btn cancel") + "\" onclick=\"CancelSaveNewPersonalTag();\">");
        taghtml.Append("                <span>" + m_refMsg.GetMessage("btn cancel") + "</span>");
        taghtml.Append("            </a>");
        taghtml.Append("        </li>");

        taghtml.Append("        <li>");
        taghtml.Append("            <a class=\"button greenHover buttonUpdate buttonRight\" type=\"button\" title=\"" + m_refMsg.GetMessage("btn save") + "\" alt=\"" + m_refMsg.GetMessage("btn save") + "\" onclick=\"SaveNewPersonalTag();\">");
        taghtml.Append("                <span>" + m_refMsg.GetMessage("btn save") + "</span>");
        taghtml.Append("            </a>");
        taghtml.Append("        </li>");
        taghtml.Append("    </ul>");
        taghtml.Append("</div>");
        taghtml.Append("</div>");
        taghtml.Append("<input type=\"hidden\" id=\"newTagNameHdn\" name=\"newTagNameHdn\" value=\"\"  />");
        taghtml.Append("<fieldset style=\"margin: 10px;\">");
        taghtml.Append("    <legend>");
        taghtml.Append("        <span class=\"label\">" + m_refMsg.GetMessage("lbl personal tags") + "</span>");
        taghtml.Append("    </legend>");
        taghtml.Append("    <div id=\"newTagNameScrollingDiv\">");

        LocalizationAPI localizationApi = new LocalizationAPI();

        //create hidden list of current tags so we know to delete removed ones.
        LanguageData[] languageDataArray = m_refSiteApi.GetAllActiveLanguages();

        foreach (LanguageData lang in languageDataArray)
        {
            taghtml.Append("<input type=\"hidden\" id=\"flag_" + lang.Id + ("\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + "\"  />"));
        }
        taghtml.Append("<input type=\"hidden\" id=\"flag_0\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(-1) + "\"  />");

        TagData[] tdaUser = null;
        if (contentId > 0)
        {
            tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForObject(contentId, EkEnumeration.CMSObjectTypes.Content, m_refContApi.ContentLanguage);
        }
        StringBuilder appliedTagIds = new StringBuilder();

        //build up a list of tags used by user
        //add tags to hashtable for reference later when looping through defualt tag list
        TagData td;
        if (tdaUser != null)
        {
            foreach (TagData tempLoopVar_td in tdaUser)
            {
                td = tempLoopVar_td;
                htTagsAssignedToUser.Add(td.Id, td);
                appliedTagIds.Append(td.Id.ToString() + ",");

                taghtml.Append("<input checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                taghtml.Append("&#160;" + td.Text + "<br />");
            }
        }

        //create hidden list of current tags so we know to delete removed ones.
        taghtml.Append("<input type=\"hidden\" id=\"currentTags\" name=\"currentTags\" value=\"" + appliedTagIds.ToString() + "\"  />");

        TagData[] tdaAll;
        tdaAll = (new Ektron.Cms.Community.TagsAPI()).GetDefaultTags(EkEnumeration.CMSObjectTypes.Content, m_refContApi.ContentLanguage);
        if (tdaAll != null)
        {
            foreach (TagData tempLoopVar_td in tdaAll)
            {
                td = tempLoopVar_td;
                //don't add to list if its already been added with user's tags above
                if (!htTagsAssignedToUser.ContainsKey(td.Id))
                {
                    taghtml.Append("<input type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                    taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                    taghtml.Append("&#160;" + td.Text + "<br />");
                }
            }
        }
        taghtml.Append("<div id=\"newAddedTagNamesDiv\"></div>");

        taghtml.Append("</div>");

        taghtml.Append("<div style=\"float:left;\">");
        taghtml.Append("    <a class=\"button buttonLeft greenHover buttonAddTagWithText\" href=\"#\" onclick=\"ShowAddPersonalTagArea();\">" + m_refMsg.GetMessage("btn add personal tag") + "</a>" + "\r\n");
        taghtml.Append("</div>");
        taghtml.Append("</fieldset>");


        taghtml.Append("</div>");

        returnValue = taghtml.ToString();
        return returnValue;
    }

    private void EditMetadataHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        int lValidCounter = 0;
        StringBuilder sbResult = new StringBuilder();
        string strResult;
        string strImage = "";
        FolderData fldr_data = new FolderData();
        long contentId = new long();
        ContentData contData = new ContentData();

        if (Request.QueryString["type"] == "add")
        {
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                fldr_data = this.m_refContApi.GetFolderById(Convert.ToInt64(Request.QueryString["id"]));
            }
            if (m_strType == "add" && m_refContentId > 0)
                meta_data = PopulateMultiLingualMetadata(meta_data);
        }

        if (Request.QueryString["type"] == "update")
        {
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                contentId = Convert.ToInt64(Request.QueryString["id"]);
                contData = this.m_refContApi.GetContentById(contentId, 0);
                if (contData != null)
                {
                    fldr_data = this.m_refContApi.GetFolderById(contData.FolderId);
                }
            }
        }
        sbHtml.Append("<div id=\"dvMetadata\">");
        if ((meta_data != null) && (meta_data.Length > 0))
        {
            foreach (ContentMetaData cMeta in meta_data)
            {
                if (cMeta.Type.ToString() == "ImageSelector")
                {
                    cMeta.Text = cMeta.Text.Replace(SitePath + "assets/", "");
                    cMeta.Text = System.Text.RegularExpressions.Regex.Replace(cMeta.Text, "\\?.*", "");
                }
            }
            sbResult = CustomFields.WriteFilteredMetadataForEdit(meta_data, false, m_strType, m_intContentFolder, ref lValidCounter, m_refSite.GetPermissions(m_intContentFolder, 0, "folder"));
            if (sbResult.ToString().Contains("<span style=\"color:red\">"))
                metadataRequired = true;
        }

        // add Tag section
        sbResult.Append(GetTagDisplayHTML(contentId));

        if (m_strType == "update")
        {
            strImage = content_edit_data.Image;
            string strThumbnailPath = content_edit_data.ImageThumbnail;
            if (content_edit_data.ImageThumbnail == "")
            {
                strThumbnailPath = m_refContApi.AppImgPath + "spacer.gif";
            }
            else if (((fldr_data.IsDomainFolder || fldr_data.DomainProduction != "") && (strThumbnailPath.IndexOf("http://") != -1 || strThumbnailPath.IndexOf("https://") != -1)) || strThumbnailPath.IndexOf("http://") != -1 || strThumbnailPath.IndexOf("https://") != -1)
            {
                //Do Nothing
            }
            else
            {
                strThumbnailPath = m_refContApi.SitePath + strThumbnailPath;
            }
            if (System.IO.Path.GetExtension(strThumbnailPath).ToLower().IndexOf(".gif") != -1 && strThumbnailPath.ToLower().IndexOf("spacer.gif") == -1)
            {
                strThumbnailPath = strThumbnailPath.Replace(".gif", ".png");
            }
            sbResult.Append("<fieldset style=\"margin-top:3em; margin-left:10px; margin-right:10px;\">");
            sbResult.Append("   <legend>");
            sbResult.Append("       <span class=\"label\">" + m_refMsg.GetMessage("lbl image data") + "</span>");
            sbResult.Append("   </legend>");
            sbResult.Append("<div class=\"ektronTopSpaceSmall\"></div>");
            sbResult.Append("<ul class=\"ui-helper-clearfix\">");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <label class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl group image") + ":</label>");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <span id=\"sitepath\">" + this.m_refContApi.SitePath + "</span>");
            sbResult.Append("       <input type=\"textbox\" size=\"50\" readonly=\"true\" id=\"content_image\" name=\"content_image\" value=\"" + strImage + "\" />");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <a class=\"button buttonEdit greenHover buttonInlineBlock\" href=\"#\" onclick=\"PopUpWindow(\'mediamanager.aspx?scope=images&upload=true&retfield=content_image&showthumb=false&autonav=" + folder_data.Id + "\', \'Meadiamanager\', 790, 580, 1,1);return false;\">" + m_refMsg.GetMessage("generic edit title") + "</a>");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <a class=\"button buttonRemove redHover buttonInlineBlock\" href=\"#\" onclick=\"RemoveContentImage(\'" + m_refContApi.AppImgPath + "spacer.gif\');return false\">" + m_refMsg.GetMessage("btn remove") + "</a>");
            sbResult.Append("   </li>");
            sbResult.Append("</ul>");
            sbResult.Append("<div class=\"ektronTopSpace\"></div>");
            sbResult.Append("<img id=\"content_image_thumb\" src=\"" + strThumbnailPath + "\" />");
            sbResult.Append("</fieldset>");
        }
        else
        {
            sbResult.Append("<fieldset style=\"margin-top:3em; margin-left:10px; margin-right:10px;\">");
            sbResult.Append("   <legend>");
            sbResult.Append("       <span class=\"label\">" + m_refMsg.GetMessage("lbl image data") + "</label>");
            sbResult.Append("   </legend>");
            sbResult.Append("<div class=\"ektronTopSpaceSmall\"></div>");
            sbResult.Append("<ul class=\"ui-helper-clearfix\">");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <label class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl group image") + ":</label>");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <span id=\"sitepath\">" + this.m_refContApi.SitePath + "</span>");
            sbResult.Append("       <input type=\"textbox\" size=\"50\" readonly=\"true\" id=\"content_image\" name=\"content_image\" value=\"" + strImage + "\" />");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <a class=\"button buttonEdit greenHover buttonInlineBlock \" href=\"#\" onclick=\"PopUpWindow(\'mediamanager.aspx?scope=images&upload=true&retfield=content_image&showthumb=false&autonav=" + folder_data.Id + "\', \'Meadiamanager\', 790, 580, 1,1);return false;\">" + m_refMsg.GetMessage("generic edit title") + "</a>");
            sbResult.Append("   </li>");
            sbResult.Append("   <li class=\"inline\">");
            sbResult.Append("       <a class=\"button buttonRemove redHover buttonInlineBlock \" href=\"#\" onclick=\"RemoveContentImage(\'" + m_refContApi.AppImgPath + "spacer.gif\');return false\">" + m_refMsg.GetMessage("btn remove") + "</a>");
            sbResult.Append("   </li>");
            sbResult.Append("</ul>");
            sbResult.Append("<div class=\"ektronTopSpace\"></div>");
            sbResult.Append("<img id=\"content_image_thumb\" src=\"" + m_refContApi.AppImgPath + "spacer.gif\" />");
            sbResult.Append("</fieldset>");
        }

        strResult = sbResult.ToString().Trim();
        if (strResult != "")
        {
            sbHtml.Append(strResult);
        }
        sbHtml.Append("</div>");
        jsValidCounter.Text = lValidCounter.ToString();
        EditMetadataHtml.Text = sbHtml.ToString();
    }
    private void EditAliasHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliaslist = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
        Ektron.Cms.UrlAliasing.UrlAliasAutoApi m_autoaliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        System.Collections.Generic.List<string> ext_alias;
        string ext = "";
        int i;
        
        Ektron.Cms.Common.UrlAliasManualData d_alias = new Ektron.Cms.Common.UrlAliasManualData(0, 0, string.Empty, string.Empty);
        System.Collections.Generic.List<UrlAliasAutoData> auto_aliaslist = new System.Collections.Generic.List<UrlAliasAutoData>();

        bool IsStagingServer;

        IsStagingServer = m_refContApi.RequestInformationRef.IsStaging;

        ext_alias = m_aliaslist.GetFileExtensions();
        if (content_edit_data != null)
        {
            d_alias = m_aliaslist.GetDefaultAlias(content_edit_data.Id);
        }
        m_strManualAliasExt = d_alias.AliasName;
        m_strManualAlias = d_alias.FileExtension;

        sbHtml.Append("<div id=\"dvAlias\">");
        if (m_urlAliasSettings.IsManualAliasEnabled)
        {
            if (m_refContApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
            {
                sbHtml.Append("<input type=\"hidden\" name=\"frm_manalias_id\" value=\"" + d_alias.AliasId + "\">");
                sbHtml.Append("<input type=\"hidden\" id=\"prev_frm_manalias_name\" name=\"prev_frm_manalias_name\" value=\"" + d_alias.AliasName + "\">");
                sbHtml.Append("<input type=\"hidden\" name=\"prev_frm_manalias_ext\" value=\"" + d_alias.FileExtension + "\">");
                sbHtml.Append("<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl tree url manual aliasing") + "</div>");
                sbHtml.Append("<table class=\"ektronForm\">");
                sbHtml.Append("<tr>");
                sbHtml.Append("<td class=\"label\">");
                sbHtml.Append(m_refMsg.GetMessage("lbl primary") + " " + m_refMsg.GetMessage("lbl alias name") + ":");
                sbHtml.Append("</td>");
                sbHtml.Append("<td class=\"value\">");

                if (IsStagingServer && folder_data.DomainStaging != string.Empty)
                {
                    sbHtml.Append("<td width=\"95%\">http://" + folder_data.DomainStaging + "/<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                else if (folder_data.IsDomainFolder)
                {
                    sbHtml.Append("http://" + folder_data.DomainProduction + "/<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                else
                {
                    sbHtml.Append(SitePath + "<input type=\"text\" id=\"frm_manalias\" size=\"35\" name=\"frm_manalias\" value=\"" + d_alias.AliasName + "\">");
                }
                for (i = 0; i <= ext_alias.Count - 1; i++)
                {
                    if (ext != "")
                    {
                        ext = ext + ",";
                    }
                    ext = ext + ext_alias[i];
                }
                sbHtml.Append(m_refContApi.RenderHTML_RedirExtensionDD("frm_ManAliasExt", d_alias.FileExtension, ext));
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("</table>");
                if (m_refContApi.RedirectorManExt.IndexOf(",") + 1 <= 0)
                {
                    ast_frm_manaliasExt.Value = m_refContApi.RedirectorManExt;
                }
            }
        }
        if (m_urlAliasSettings.IsAutoAliasEnabled)
        {
            if (content_edit_data != null)
            {
                auto_aliaslist = m_autoaliasApi.GetListForContent(content_edit_data.Id);
            }
            sbHtml.Append("<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl automatic") + "</div>");
            sbHtml.Append("<div class=\"ektronBorder\" style=\"width: auto; height: auto; overflow: auto;\" id=\"autoAliasList\">");
            sbHtml.Append("<table width=\"100%\">");
            sbHtml.Append("<tr class=\"title-header\">");
            sbHtml.Append("<th>");
            sbHtml.Append(m_refMsg.GetMessage("generic type"));
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append(m_refMsg.GetMessage("lbl alias name"));
            sbHtml.Append("</th>");
            for (i = 0; i <= auto_aliaslist.Count() - 1; i++)
            {
                sbHtml.Append("<tr>");
                sbHtml.Append("<td>" + auto_aliaslist[i].AutoAliasType.ToString() + "</td>");
                sbHtml.Append("<td>" + auto_aliaslist[i].AliasName + "</td>");
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            sbHtml.Append("</div>");
        }
        sbHtml.Append("</div>");
        EditAliasHtml.Text = sbHtml.ToString();
    }

    private void EditCommentHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        sbHtml.Append("<div id=\"dvComment\">");
        sbHtml.Append("<table class=\"ektronForm\">");
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">" + m_refMsg.GetMessage("generic comment label") + "</td>");
        //sbHtml.Append("<td class=\"value\"><textarea OnKeyPress=\"return CheckKeyValue(event, \'34\');\" onkeydown=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);\" onkeyup=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);\" onMouseOut=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);\" name=\"content_comment\" rows=\"8\" cols=\"50\">" + content_comment + "</textarea><br />");
        //sbHtml.Append("<input type=\"hidden\" name=\"remainLen\" size=\"3\" maxlength=\"3\" value=\"255\">");
        //sbHtml.Append("<script language=\"javascript\">textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255)</script>");
        sbHtml.Append("<td class=\"value\"><textarea OnKeyPress=\"return CheckKeyValue(event, \'34\');\" onkeydown=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 2000);\" onkeyup=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 2000);\" onMouseOut=\"textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 2000);\" name=\"content_comment\" style=\"width: 100%;\" wrap=\"soft\" rows=\"8\" cols=\"50\">" + content_comment + "</textarea><br />");
        sbHtml.Append("<input type=\"hidden\" name=\"remainLen\" size=\"4\" maxlength=\"4\" value=\"2000\">");
        sbHtml.Append("<script language=\"javascript\">textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 2000)</script>");
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("</table>");
        sbHtml.Append("</div>");
        EditCommentHtml.Text = sbHtml.ToString();
    }

    private void EditSubscriptionHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        Array arrSubscribed = null;
        int findindex;
        int i = 0;
        string strEnabled = "";
        string strNotifyA = "";
        string strNotifyI = "";
        string strNotifyN = "";
        string strNotifySend = "";
        string strNotifySuspend = "";
        string strNotifyMessage = "";
        EmailFromData[] emailfrom_list;
        EmailMessageData[] defaultmessage_list;
        EmailMessageData[] unsubscribe_list;
        EmailMessageData[] optout_list;
        int y = 0;
        try
        {
            emailfrom_list = m_refContApi.GetAllEmailFrom();
            defaultmessage_list = m_refContApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage);
            unsubscribe_list = m_refContApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe);
            optout_list = m_refContApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut);

            sbHtml.Append(Environment.NewLine + "<script language=\"javascript\">" + Environment.NewLine);
            sbHtml.Append("function UpdateNotifyStatus() {" + Environment.NewLine);
            sbHtml.Append("if (frmMain.notify_option[0].checked == true) {" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"lbl_notification_status\").innerHTML = \'<font color=\"green\">Web Alerts are enabled.</font>\';" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"suspend_notification_button\").disabled = false;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"send_notification_button\").disabled = true;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"send_notification_button\").checked = false;" + Environment.NewLine);
            sbHtml.Append("} else if (frmMain.notify_option[1].checked == true) {" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"lbl_notification_status\").innerHTML = \'<font color=\"green\">Web Alerts are enabled.</font>\';" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"suspend_notification_button\").disabled = true;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"suspend_notification_button\").checked = false;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"send_notification_button\").disabled = false;" + Environment.NewLine);
            sbHtml.Append("} else {" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"lbl_notification_status\").innerHTML = \'<font color=\"red\">Web Alerts are disabled.</font>\';" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"suspend_notification_button\").checked = false;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"send_notification_button\").checked = false;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"suspend_notification_button\").disabled = true;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\"send_notification_button\").disabled = true;" + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("function  PreviewWebAlert() {" + Environment.NewLine);
            sbHtml.Append("    var contentid, defmsgid, optid, summaryid, unsubid, conttype, usecontlink;" + Environment.NewLine);
            sbHtml.Append("    if (document.getElementById(\'use_content_button\').checked == true) {;" + Environment.NewLine);
            sbHtml.Append("      contentid = document.getElementById(\'frm_content_id\').value;" + Environment.NewLine);
            sbHtml.Append("    } else {" + Environment.NewLine);
            sbHtml.Append("      contentid = 0;" + Environment.NewLine);
            sbHtml.Append("    }" + Environment.NewLine);
            sbHtml.Append("    if (document.getElementById(\'use_message_button\').checked == true) {;" + Environment.NewLine);
            sbHtml.Append("      defmsgid = document.getElementById(\'notify_messageid\').value;" + Environment.NewLine);
            sbHtml.Append("    } else {" + Environment.NewLine);
            sbHtml.Append("      defmsgid = 0;" + Environment.NewLine);
            sbHtml.Append("    }" + Environment.NewLine);
            sbHtml.Append("    optid = document.getElementById(\'notify_optoutid\').value;" + Environment.NewLine);
            sbHtml.Append("    summaryid = document.getElementById(\'use_summary_button\').checked; " + Environment.NewLine);
            sbHtml.Append("    unsubid = document.getElementById(\'notify_unsubscribeid\').value;" + Environment.NewLine);
            sbHtml.Append("    conttype = document.getElementById(\'content_type\').value;" + Environment.NewLine);
            sbHtml.Append("    if (document.getElementById(\'use_contentlink_button\').checked == true) {;" + Environment.NewLine);
            sbHtml.Append("      usecontlink = 1;" + Environment.NewLine);
            sbHtml.Append("    } else {" + Environment.NewLine);
            sbHtml.Append("      usecontlink = 0;" + Environment.NewLine);
            sbHtml.Append("    }" + Environment.NewLine);
            sbHtml.Append("    window.open(\'previewwebalert.aspx?content=" + m_intItemId + "&defmsg=\' + defmsgid + \'&optoutid=\' + optid + \'&summaryid=\' + summaryid + \'&usecontentid=\' + contentid + \'&unsubscribeid=\' + unsubid + \'&content_type=\' + conttype + \'&uselink=\' + usecontlink,\'\',\'menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes\'); " + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("function SetMessageContenttoDefault() {" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\'use_content_button\').checked = true;" + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\'frm_content_id\').value = -1; " + Environment.NewLine);
            sbHtml.Append("    document.getElementById(\'titlename\').value = \'[[use current]]\'; " + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("function enableCheckboxes() {" + Environment.NewLine);
            sbHtml.Append("    var idx, masterBtn, tableObj, enableFlag, qtyElements;" + Environment.NewLine);
            sbHtml.Append("    tableObj = document.getElementById(\'cfld_subscription_assignment\');" + Environment.NewLine);
            sbHtml.Append("    enableFlag = false;" + Environment.NewLine);
            sbHtml.Append("    masterBtn = document.getElementById(\'break_inherit_button\');" + Environment.NewLine);
            sbHtml.Append("    if (validateObject(masterBtn)){" + Environment.NewLine);
            sbHtml.Append("        enableFlag = masterBtn.checked;" + Environment.NewLine);
            sbHtml.Append("    }" + Environment.NewLine);
            sbHtml.Append("    if (validateObject(tableObj)){" + Environment.NewLine);
            sbHtml.Append("        qtyElements = tableObj.all.length;" + Environment.NewLine);
            sbHtml.Append("        for(idx = 0; idx < qtyElements; idx++ ) {" + Environment.NewLine);
            sbHtml.Append("    		    if (tableObj.all[idx].type == \'checkbox\'){" + Environment.NewLine);
            sbHtml.Append("    			    tableObj.all[idx].disabled = !enableFlag;" + Environment.NewLine);
            sbHtml.Append("    		    }" + Environment.NewLine);
            sbHtml.Append("        }" + Environment.NewLine);
            sbHtml.Append("    }" + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("function validateObject(obj) {" + Environment.NewLine);
            sbHtml.Append("     return ((obj != null) &&" + Environment.NewLine);
            sbHtml.Append("         ((typeof(obj)).toLowerCase() != \'undefined\') &&" + Environment.NewLine);
            sbHtml.Append("         ((typeof(obj)).toLowerCase() != \'null\'))" + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("function valAndSaveCSubAssignments() {" + Environment.NewLine);
            if ((!(active_subscription_list == null)) && (!(subscription_data_list == null)) && (!((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (settings_data.AsynchronousLocation == ""))))
            {
                sbHtml.Append("    var idx, masterBtn, tableObj, enableFlag, qtyElements, retStr;" + Environment.NewLine);
                sbHtml.Append("    var hidnFld;" + Environment.NewLine);
                sbHtml.Append("    //hidnFld = document.getElementById(\'content_sub_assignments\');" + Environment.NewLine);
                sbHtml.Append("    document.forms[0].content_sub_assignments.value = \'\'; //hidnFld.value=\'\'" + Environment.NewLine);
                sbHtml.Append("    tableObj = tableObj = document.getElementById(\'therows\');" + Environment.NewLine);
                sbHtml.Append("    tableObj = tableObj.getElementsByTagName(\'input\');" + Environment.NewLine);
                sbHtml.Append("    enableFlag = true;" + Environment.NewLine);
                sbHtml.Append("    retStr = \'\';" + Environment.NewLine);
                sbHtml.Append("    if ((validateObject(tableObj)) && enableFlag){" + Environment.NewLine);
                sbHtml.Append("        qtyElements = tableObj.length;" + Environment.NewLine);
                sbHtml.Append("        for(idx = 0; idx < qtyElements; idx++ ) {" + Environment.NewLine);
                sbHtml.Append("    		    if ((tableObj[idx].type == \'checkbox\') && tableObj[idx].checked){" + Environment.NewLine);
                sbHtml.Append("    			    retStr = retStr + tableObj[idx].name + \' \';" + Environment.NewLine);
                sbHtml.Append("    		    }" + Environment.NewLine);
                sbHtml.Append("        }" + Environment.NewLine);
                sbHtml.Append("    }" + Environment.NewLine);
                sbHtml.Append("    document.forms[0].content_sub_assignments.value = retStr; // hidnFld.value = " + Environment.NewLine);
            }
            sbHtml.Append("    return true; // (Note: return false to prevent form submission)" + Environment.NewLine);
            sbHtml.Append("}" + Environment.NewLine);
            sbHtml.Append("</script>" + Environment.NewLine);

            if (active_subscription_list == null)
            {
                sbHtml.Append("<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">");
                phSubscription.Visible = false;
                EditSubscriptionHtml.Visible = false;
                lblNotificationStatus.Text = "<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">";
            }
            else if ((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (subscription_data_list == null) || (settings_data.AsynchronousLocation == ""))
            {
                sbHtml.Append("<div id=\"dvSubscription\">");
                sbHtml.Append("<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">");
                sbHtml.Append("<br/><b>" + m_refMsg.GetMessage("lbl web alert settings") + ":</b><br/><br/>" + m_refMsg.GetMessage("lbl web alert not setup") + "<br/>");
                if (emailfrom_list == null)
                {
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("lbl web alert emailfrom not setup") + "</font>");
                }
                if (defaultmessage_list == null)
                {
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("lbl web alert def msg not setup") + "</font>");
                }
                if (unsubscribe_list == null)
                {
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("lbl web alert unsub not setup") + "</font>");
                }
                if (optout_list == null)
                {
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("lbl web alert optout not setup") + "</font>");
                }
                if (subscription_data_list == null)
                {
                    phSubscription.Visible = false;
                    EditSubscriptionHtml.Visible = false;
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("alt No subscriptions are enabled on the folder.") + "</font>");
                }
                if (settings_data.AsynchronousLocation == "")
                {
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=\"red\">" + m_refMsg.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") + "</font>");
                }
                sbHtml.Append("</div>");
            }
            else
            {
                if (subscription_properties_list == null)
                {
                    subscription_properties_list = new SubscriptionPropertiesData();
                }
                sbHtml.Append("<div id=\"dvSubscription\">");
                sbHtml.Append("<table class=\"ektronGrid\" width=\"100%\">");
                sbHtml.Append("<tr><td class=\"label\">");
                sbHtml.Append("" + m_refMsg.GetMessage("lbl web alert opt") + ":");
                sbHtml.Append("</td>");

                switch (subscription_properties_list.NotificationType.GetHashCode())
                {
                    case 0:
                        strNotifyA = " CHECKED=\"true\" ";
                        strNotifyI = "";
                        strNotifyN = "";
                        strNotifySend = " DISABLED=\"true\" ";
                        strNotifySuspend = "";
                        strNotifyMessage = "<font color=\"green\">Web Alerts are enabled.</font>";
                        break;
                    case 1:
                        strNotifyA = "";
                        strNotifyI = " CHECKED=\"true\" ";
                        strNotifyN = "";
                        strNotifySend = "";
                        strNotifySuspend = " DISABLED=\"true\" ";
                        strNotifyMessage = "<font color=\"green\">Web Alerts are enabled.</font>";
                        break;
                    case 2:
                        strNotifyA = "";
                        strNotifyI = "";
                        strNotifyN = " CHECKED=\"true\" ";
                        strNotifySend = " DISABLED=\"true\" ";
                        strNotifySuspend = " DISABLED=\"true\" ";
                        strNotifyMessage = "<font color=\"red\">Web Alerts are disabled.</font>";
                        break;
                }
                sbHtml.Append("<td class=\"value\">");
                sbHtml.Append("&nbsp;&nbsp;<input type=\"radio\" value=\"Always\" name=\"notify_option\" OnClick=\"UpdateNotifyStatus()\"  " + strNotifyA + ">&nbsp; " + m_refMsg.GetMessage("lbl web alert notify always") + "<br />");

                sbHtml.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"suspend_notification_button\" onclick=\"//;\" type=\"checkbox\" name=\"suspend_notification_button\" " + strNotifySuspend + ">");

                sbHtml.Append("&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl web alert suspend") + "<br/>");
                sbHtml.Append("&nbsp;&nbsp;<input type=\"radio\" value=\"Initial\" name=\"notify_option\" OnClick=\"UpdateNotifyStatus()\"  " + strNotifyI + ">");
                sbHtml.Append("&nbsp; " + m_refMsg.GetMessage("lbl web alert notify initial") + "<br />");

                sbHtml.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"send_notification_button\" onclick=\"//;\" type=\"checkbox\" name=\"send_notification_button\" " + strNotifySend + ">");

                sbHtml.Append("&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl web alert send") + "<br />");
                sbHtml.Append("&nbsp;&nbsp;<input type=\"radio\" value=\"Never\" name=\"notify_option\" OnClick=\"UpdateNotifyStatus()\"  " + strNotifyN + ">&nbsp; " + m_refMsg.GetMessage("lbl web alert notify never") + "<br/>");

                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");

                sbHtml.Append("<tr>");
                sbHtml.Append("<td class=\"label\">");
                sbHtml.Append("" + m_refMsg.GetMessage("lbl web alert subject") + ":");
                sbHtml.Append("</td>");
                sbHtml.Append("<td class=\"value\">");
                if (subscription_properties_list.Subject != "")
                {
                    sbHtml.Append("&nbsp;<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"" + subscription_properties_list.Subject + "\" name=\"notify_subject\" " + strEnabled + "/>&nbsp;<br />");
                }
                else
                {
                    sbHtml.Append("&nbsp;<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"\" name=\"notify_subject\" " + strEnabled + "/>&nbsp;<br />");
                }
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("<tr>");
                sbHtml.Append("<td class=\"label\">");
                sbHtml.Append("" + m_refMsg.GetMessage("lbl web alert emailfrom address") + ":");
                sbHtml.Append("</td>");
                sbHtml.Append("<td class=\"value\">");
                sbHtml.Append("<select name=\"notify_emailfrom\" id=\"notify_emailfrom\">");

                if ((emailfrom_list != null) && emailfrom_list.Length > 0)
                {
                    for (y = 0; y <= emailfrom_list.Length - 1; y++)
                    {
                        if (emailfrom_list[y].Email == subscription_properties_list.EmailFrom)
                        {
                            sbHtml.Append("<option value=\"" + EkFunctions.HtmlEncode(emailfrom_list[y].Email) + "\" selected>" + emailfrom_list[y].Email + "</option>");
                        }
                        else
                        {
                            sbHtml.Append("<option value=\"" + EkFunctions.HtmlEncode(emailfrom_list[y].Email) + "\">" + emailfrom_list[y].Email + "</option>");
                        }
                    }
                }
                sbHtml.Append("</select>");
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("<tr>");
                sbHtml.Append("<td class=\"label\">");
                sbHtml.Append("" + m_refMsg.GetMessage("lbl web alert contents") + ":&nbsp;");
                sbHtml.Append("<img src=\"" + m_refContApi.AppPath + "images/UI/Icons/preview.png\" alt=\"Preview Web Alert Message\" title=\"Preview Web Alert Message\" onclick=\" PreviewWebAlert(); return false;\" />");
                sbHtml.Append("</td>");
                sbHtml.Append("<td class=\"value\" nowrap=\"nowrap\">");
                sbHtml.Append("&nbsp;&nbsp;<input id=\"use_optout_button\" type=\"checkbox\" checked=\"true\" name=\"use_optout_button\" disabled=\"true\">&nbsp;&nbsp;Opt Out Message");

                sbHtml.Append("&nbsp;&nbsp;<select " + strEnabled + " name=\"notify_optoutid\" id=\"notify_optoutid\">");
                if ((optout_list != null) && optout_list.Length > 0)
                {
                    for (y = 0; y <= optout_list.Length - 1; y++)
                    {
                        if (optout_list[y].Id == subscription_properties_list.OptOutID)
                        {
                            sbHtml.Append("<option value=\"" + optout_list[y].Id + "\" selected>" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>");
                        }
                        else
                        {
                            sbHtml.Append("<option value=\"" + optout_list[y].Id + "\">" + EkFunctions.HtmlEncode(optout_list[y].Title) + "</option>");
                        }
                    }
                }
                sbHtml.Append("</select><br />");

                if (subscription_properties_list.DefaultMessageID > 0)
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_message_button\" type=\"checkbox\" checked=\"true\" name=\"use_message_button\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use default message"));
                }
                else
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_message_button\" type=\"checkbox\" name=\"use_message_button\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use default message"));
                }
                sbHtml.Append("&nbsp;&nbsp;<select " + strEnabled + " name=\"notify_messageid\" id=\"notify_messageid\">");

                if ((defaultmessage_list != null) && defaultmessage_list.Length > 0)
                {
                    for (y = 0; y <= defaultmessage_list.Length - 1; y++)
                    {
                        if (defaultmessage_list[y].Id == subscription_properties_list.DefaultMessageID)
                        {
                            sbHtml.Append("<option value=\"" + defaultmessage_list[y].Id + "\" selected>" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>");
                        }
                        else
                        {
                            sbHtml.Append("<option value=\"" + defaultmessage_list[y].Id + "\">" + EkFunctions.HtmlEncode(defaultmessage_list[y].Title) + "</option>");
                        }
                    }
                }
                sbHtml.Append("</select><br />");

                if (subscription_properties_list.SummaryID > 0)
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" checked=\"true\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use summary message") + "<br />");
                }
                else
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use summary message") + "<br />");
                }
                if (subscription_properties_list.ContentID == -1)
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"true\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use content message"));
                    sbHtml.Append("&nbsp;&nbsp;<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" id=\"frm_content_id\" value=\"-1\"/><input type=\"hidden\" name=\"frm_content_langid\" id=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\" id=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" id=\"titlename\" value=\"[[use current]]\" size=\"65\" disabled=\"true\"/>");
                    sbHtml.Append("<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + m_intContentFolder.ToString() + ",\'frmMain\',\'titlename\',0,0,0,0) ;return false;\">" + m_refMsg.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">" + m_refMsg.GetMessage("use current") + "</a><br/>");
                }
                else if (subscription_properties_list.ContentID > 0)
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"true\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use content message"));
                    sbHtml.Append("&nbsp;&nbsp;<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" id=\"frm_content_id\" value=\"" + subscription_properties_list.ContentID.ToString() + "\"/><input type=\"hidden\" name=\"frm_content_langid\" id=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\" id=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" id=\"titlename\" value=\"" + subscription_properties_list.UseContentTitle.ToString() + "\" size=\"65\" disabled=\"true\"/>");
                    sbHtml.Append("<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + m_intContentFolder.ToString() + ",\'frmMain\',\'titlename\',0,0,0,0) ;return false;\">" + m_refMsg.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a><br/>");
                }
                else
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" " + strEnabled + ">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl use content message"));
                    sbHtml.Append("&nbsp;&nbsp;<input type=\"hidden\" maxlength=\"20\" name=\"frm_content_id\" id=\"frm_content_id\" value=\"0\" /><input type=\"hidden\" name=\"frm_content_langid\" id=\"frm_content_langid\"/><input type=\"hidden\" name=\"frm_qlink\" id=\"frm_qlink\"/><input type=\"text\" name=\"titlename\" id=\"titlename\" onkeydown=\"return false\" value=\"\" size=\"65\" disabled=\"true\"/>");
                    sbHtml.Append("<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + m_intContentFolder.ToString() + ",\'frmMain\',\'titlename\',0,0,0,0) ;return false;\">" + m_refMsg.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a><br/>");
                }
                if (subscription_properties_list.UseContentLink > 0)
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" checked=\"true\" " + strEnabled + ">&nbsp;&nbsp;Use Content Link<br />");
                }
                else
                {
                    sbHtml.Append("&nbsp;&nbsp;<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" " + strEnabled + ">&nbsp;&nbsp;Use Content Link<br />");
                }
                sbHtml.Append("&nbsp;&nbsp;<input id=\"use_unsubscribe_button\" type=\"checkbox\" checked=\"true\" name=\"use_unsubscribe_button\" disabled=\"true\">&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl unsubscribe message"));
                sbHtml.Append("&nbsp;&nbsp;<select " + strEnabled + " name=\"notify_unsubscribeid\" id=\"notify_unsubscribeid\">");
                if ((unsubscribe_list != null) && unsubscribe_list.Length > 0)
                {
                    for (y = 0; y <= unsubscribe_list.Length - 1; y++)
                    {
                        if (unsubscribe_list[y].Id == subscription_properties_list.UnsubscribeID)
                        {
                            sbHtml.Append("<option value=\"" + unsubscribe_list[y].Id + "\" selected>" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>");
                        }
                        else
                        {
                            sbHtml.Append("<option value=\"" + unsubscribe_list[y].Id + "\">" + EkFunctions.HtmlEncode(unsubscribe_list[y].Title) + "</option>");
                        }
                    }
                }
                sbHtml.Append("</select><br /><br />");
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("</table>");
                sbHtml.Append("<div class=\"ektronHeader\">" + m_refMsg.GetMessage("lbl avail web alert") + ":</div>");
                sbHtml.Append("</td></tr>");
                sbHtml.Append("<table class=\"ektronGrid\" cellspacing=\"1\" id=\"cfld_subscription_assignment\" name=\"cfld_subscription_assignment\"><tbody id=\"therows\">");
                lblNotificationStatus.Text = "<span id=\"lbl_notification_status\">" + strNotifyMessage + "</span>";
                if (!(subscription_data_list == null) && subscription_data_list.Length > 0)
                {
                    sbHtml.Append("<tr class=\"title-header\"><td>" + m_refMsg.GetMessage("lbl assigned") + "</td><td>" + m_refMsg.GetMessage("lbl name") + "</td></tr>");
                    if (!(subscribed_data_list == null))
                    {
                        arrSubscribed = Array.CreateInstance(typeof(string), subscribed_data_list.Length);
                        for (i = 0; i <= subscribed_data_list.Length - 1; i++)
                        {
                            arrSubscribed.SetValue(subscribed_data_list[i].Name, i);
                        }
                        if (arrSubscribed.Length > 0)
                        {
                            Array.Sort(arrSubscribed);
                        }
                    }

                    for (i = 0; i <= subscription_data_list.Length - 1; i++)
                    {
                        findindex = -1;
                        if (!(subscribed_data_list == null))
                        {
                            findindex = Array.BinarySearch(arrSubscribed, subscription_data_list[i].Name);
                        }
                        sbHtml.Append("<tr>");
                        if (findindex < 0)
                        {
                            sbHtml.Append("<td nowrap=\"true\" align=\"center\"><input type=\"checkbox\" name=\"Assigned_" + subscription_data_list[i].Id + "\"  id=\"Assigned_" + subscription_data_list[i].Id + "\" " + strEnabled + "></td></td>");
                        }
                        else
                        {
                            sbHtml.Append("<td nowrap=\"true\" align=\"center\"><input type=\"checkbox\" name=\"Assigned_" + subscription_data_list[i].Id + "\"  id=\"Assigned_" + subscription_data_list[i].Id + "\" checked=\"true\" " + strEnabled + "></td></td>");
                        }
                        sbHtml.Append("<td nowrap=\"true\" align=\"Left\">" + subscription_data_list[i].Name + "</td>");
                        sbHtml.Append("</tr>");
                    }
                }
                else
                {
                    sbHtml.Append("<tr><td>Nothing available.</td></tr>");
                }

                sbHtml.Append("</tbody></table>");
                sbHtml.Append("<input type=\"hidden\" name=\"content_sub_assignments\" id=\"content_sub_assignments\" value=\"\"></td>");
                sbHtml.Append("</tr>");

                sbHtml.Append("</table>");

                sbHtml.Append("</div>");
            }
            EditSubscriptionHtml.Visible = true;
            EditSubscriptionHtml.Text = sbHtml.ToString();
        }
        catch (Exception)
        {

        }
    }

    private void EditSelectedTemplate()
    {

        StringBuilder str = new StringBuilder();
        bool bShowTemplateUI = false;
        int iContType;

        if (Request.QueryString["ContType"] == null)
        {
            iContType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
        }
        else
        {
            iContType = Convert.ToInt32(Request.QueryString["ContType"]);
        }

        if (m_strType == "add")
        {
            if (iContType == Ektron.Cms.Common.EkConstants.CMSContentType_Content || iContType == Ektron.Cms.Common.EkConstants.CMSContentType_Forms || iContType == EkConstants.CMSContentType_Media || iContType == EkConstants.CMSContentType_Archive_Content || iContType == EkConstants.CMSContentType_Archive_Forms || iContType == EkConstants.CMSContentType_Archive_Media)
            {
                bShowTemplateUI = true;
            }
        }
        else
        {
            if (content_edit_data != null)
            {
                iContType = content_edit_data.Type;
                if (iContType == Ektron.Cms.Common.EkConstants.CMSContentType_Content || iContType == Ektron.Cms.Common.EkConstants.CMSContentType_Forms || iContType == EkConstants.CMSContentType_Media || iContType == EkConstants.CMSContentType_Archive_Content || iContType == EkConstants.CMSContentType_Archive_Forms || iContType == EkConstants.CMSContentType_Archive_Media)
                {
                    bShowTemplateUI = true;
                }
            }
        }
        if (!bShowTemplateUI)
        {
            str.Append( m_refMsg.GetMessage("Generic Not Applicable"));
        }
        else
        {
            if ((m_strType == "add" && (content_data != null) && (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)) || ((content_edit_data != null) && (content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_edit_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)))
            {
                 //show available wireframes
                Ektron.Cms.PageBuilder.WireframeModel wfm = new Ektron.Cms.PageBuilder.WireframeModel();

                Ektron.Cms.PageBuilder.WireframeData[] active_template_list = wfm.FindByFolderID(folder_data.Id);

                long selected_templateid = this.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.ContentLanguage);
                if (selected_templateid == 0)
                {
                    //check if this is a translation, if so, get the one from the original language
                    if (!string.IsNullOrEmpty(Request.QueryString["translate"]) && !string.IsNullOrEmpty(Request.QueryString["con_lang_id"]))
                    {
                        int originalLang = Int32.Parse(Request.QueryString["con_lang_id"]);
                        selected_templateid = this.m_refContApi.GetSelectedTemplateByContent(m_refContentId, originalLang);
                    }
                    //check if default language has one
                    if (selected_templateid == 0 && m_refContApi.RequestInformationRef.ContentLanguage != m_refContApi.RequestInformationRef.DefaultContentLanguage)
                    {
                        selected_templateid = this.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.DefaultContentLanguage);
                    }
                    if (selected_templateid == 0)//just use the default
                    {
                        selected_templateid = folder_data.TemplateId;
                    }
                }

                if (active_template_list.Length < 1 && selected_templateid == 0)
                {
                    phTemplates.Visible = false;
                    EditTemplateHtml.Visible = false;
                }

                str.Append("<table class=\"ektronForm\">");
                str.Append("<tr>");
                str.Append("<td class=\"label\">");
                str.Append(m_refMsg.GetMessage("template label") + ":");
                str.Append("</td>");
                str.Append("<td class=\"value\">");
                str.Append("<select id=\"templateSelect\" name=\"templateSelect\">");
                foreach (Ektron.Cms.PageBuilder.WireframeData wireframe in active_template_list)
                {
                    if (wireframe.Template.Id == selected_templateid)
                    {
                        str.Append("<option value=\"" + wireframe.Template.Id + "\" selected>" + wireframe.Path + "</option>");
                    }
                    else
                    {
                        str.Append("<option value=\"" + wireframe.Template.Id + "\">" + wireframe.Path + "</option>");
                    }
                }

                if (active_template_list.Length == 0)
                {
                    Ektron.Cms.PageBuilder.TemplateModel templModel = new Ektron.Cms.PageBuilder.TemplateModel();
                    TemplateData selectedTemplate = templModel.FindByID(selected_templateid);
                    string selectedTemplateName = (selectedTemplate != null) ? selectedTemplate.TemplateName : "";

                    str.Append("<option value=\"" + selected_templateid + "\" selected>" + selectedTemplateName + "</option>");
                }
                str.Append("</select>");
                str.Append("</td>");
                str.Append("</tr>");
                str.Append("</table>");
                if (content_edit_data != null)
                {
                    if (content_edit_data.LockedContentLink)
                    {
                        str.AppendLine("<br/>");
                        str.AppendLine("<label>Quicklink Locked:</label><input type=\"checkbox\" onclick=\"DisableTemplateSelect(this.checked)\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\"" + (content_edit_data.LockedContentLink ? "checked=\"true\"" : "") + "\"/>");
                        str.AppendLine("<br/>");
                        str.AppendLine("<label>Quicklink:</label> \"" + content_edit_data.Quicklink + "\"");
                        str.AppendLine("<script language=\"Javascript\"> DisableTemplateSelect(true) </script>");
                    }
                    else
                    {
                        str.AppendLine("<input type=\"hidden\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\" value=\"false\" />");
                    }
                }
                else
                {
                    str.AppendLine("<input type=\"hidden\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\" value=\"false\" />");
                }
            }
            else
            {
                TemplateData[] active_template_list = m_refContApi.GetEnabledTemplatesByFolder(folder_data.Id);

                if (active_template_list.Length < 1)
                {
                    phTemplates.Visible = false;
                    EditTemplateHtml.Visible = false;
                }

                long selected_folder = this.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.ContentLanguage);
                if (selected_folder == 0)
                {
                    selected_folder = folder_data.TemplateId;
                }

                str.Append("<table class=\"ektronForm\">");
                str.Append("<tr>");
                str.Append("<td class=\"label\">");
                str.Append(m_refMsg.GetMessage("template label") + ":");
                str.Append("</td>");
                str.Append("<td class=\"value\">");
                str.Append("<select id=\"templateSelect\" name=\"templateSelect\">");
                foreach (TemplateData template in active_template_list)
                {
                    if (template.SubType != Ektron.Cms.Common.EkEnumeration.TemplateSubType.Wireframes)
                    {
                        if (template.Id == selected_folder)
                        {
                            str.Append("<option value=\"" + template.Id + "\" selected>" + template.FileName + "</option>");
                        }
                        else
                        {
                            str.Append("<option value=\"" + template.Id + "\">" + template.FileName + "</option>");
                        }
                    }
                }

                if (active_template_list.Length == 0)
                {
                    str.Append("<option value=\"" + folder_data.TemplateId + "\">" + folder_data.TemplateFileName + "</option>");
                }
                str.Append("</select>");
                str.Append("</td>");
                str.Append("</tr>");
                str.Append("</table>");
                if (content_edit_data != null)
                {
                    if (content_edit_data.LockedContentLink)
                    {
                        str.AppendLine("<br/>");
                        str.AppendLine("<label>Quicklink Locked:</label><input type=\"checkbox\" onclick=\"DisableTemplateSelect(this.checked)\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\"" + (content_edit_data.LockedContentLink ? "checked=\"true\"" : "") + "\"/>");
                        str.AppendLine("<br/>");
                        str.AppendLine("<label>Quicklink:</label> \"" + content_edit_data.Quicklink + "\"");
                        str.AppendLine("<script language=\"Javascript\"> DisableTemplateSelect(true) </script>");
                    }
                    else
                    {
                        str.AppendLine("<input type=\"hidden\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\" value=\"false\" />");
                    }
                }
                else
                {
                    str.AppendLine("<input type=\"hidden\" name=\"chkLockedContentLink\" id=\"chkLockedContentLink\" value=\"false\" />");
                }
            }
        }

        EditTemplateHtml.Text = "<div id=\"dvTemplates\">" + str.ToString() + "</div>";
    }

    private void EditScheduleHtmlScripts()
    {
        StringBuilder sbHtml = new StringBuilder();
        EkDTSelector dateSchedule;
        dateSchedule = this.m_refContApi.EkDTSelectorRef;

        sbHtml.Append("<div id=\"dvSchedule\">");
        sbHtml.Append("<table class=\"ektronForm\">");
        sbHtml.Append("<tr>");

        sbHtml.Append("<script language=\"javascript\">");
        sbHtml.Append("function OpenCalendar(bStartDate) {");
        sbHtml.Append("if (true == bStartDate) {");
        sbHtml.Append("document.forms[0].go_live.value = Trim(document.forms[0].go_live.value);CallCalendar(document.forms[0].go_live.value, \'calendar.aspx\', \'go_live\', \'frmMain\');");
        sbHtml.Append("} else if (false == bStartDate) {");
        sbHtml.Append("document.forms[0].end_date.value = Trim(document.forms[0].end_date.value);CallCalendar(document.forms[0].end_date.value, \'calendar.aspx\', \'end_date\', \'frmMain\');");
        sbHtml.Append("}");
        sbHtml.Append("}");
        sbHtml.Append("</script>");
        sbHtml.Append("<td class=\"label\">" + m_refMsg.GetMessage("generic start date label") + "</td>");
        sbHtml.Append("<td class=\"value\">");
        dateSchedule.formName = "frmMain";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "go_live";
        dateSchedule.spanId = "go_live_span";
        if (!string.IsNullOrEmpty(go_live))
        {
            dateSchedule.targetDate = DateTime.Parse(go_live);
        }
        sbHtml.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("<tr>");

        sbHtml.Append("<td class=\"label\">" + m_refMsg.GetMessage("generic end date label") + "</td>");
        sbHtml.Append("<td class=\"value\">");
        dateSchedule.formName = "frmMain";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (!string.IsNullOrEmpty(end_date))
        {
            dateSchedule.targetDate = DateTime.Parse(end_date);
        }
        else
        {
            dateSchedule.targetDate = DateTime.MinValue;
        }
        sbHtml.Append(dateSchedule.displayCultureDateTime(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">" + m_refMsg.GetMessage("End Date Action Title") + ":</td>");

        if (m_strType == "add" || m_strType == "multiple,add")
        {
            end_date_action = "1";
        }
        int ii;
        bool DoCheck = false;
        sbHtml.Append("<td class=\"value\">");
        for (ii = 1; ii <= endDateActionSize; ii++)
        {
            if (((Ektron.Cms.Common.EkEnumeration.FolderType)folder_data.FolderType) == Ektron.Cms.Common.EkEnumeration.FolderType.Blog && ii == 2) //blog + archive and remain
            {
                if (ii.ToString() == end_date_action)
                {
                    DoCheck = true;
                }
            }
            else
            {
                sbHtml.Append("<input type=\"radio\" name=\"end_date_action_radio\" value=\"" + ii + "\"");
                if (ii.ToString() == end_date_action || DoCheck)
                {
                    sbHtml.Append(" checked");
                    if (DoCheck)
                    {
                        DoCheck = false;
                    }
                }
                sbHtml.Append(">" + endDateActionSel[Convert.ToString(ii)] + "<br />");
            }
        }
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("</table>");
        sbHtml.Append("</div>");
        EditScheduleHtml.Text = sbHtml.ToString();
    }

    private string HideVariables()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        iSegment = 0;
        iSegment2 = 0;
        int i = 1;
        int iPackLoop = 1;
        int var1;

        if (m_strContentHtml.Length == 0)
        {
            if (editorPackage.Length > 0)
            {
                m_strContentHtml = m_refContApi.TransformXsltPackage(editorPackage, Server.MapPath(this.m_refContApi.AppeWebPath + "unpackageDocument.xslt"), true);
            }
        }

        if (m_strContentHtml.Length > iMaxContLength)
        {
            var1 = m_strContentHtml.Length;
        }
        else
        {
            var1 = iMaxContLength;
        }
        while (i <= var1)
        {
            result.Append("<input type=\"hidden\" name=\"hiddencontent" + (iSegment + 1) + "\" value=\"\">" + "\r\n");
            result.Append("<input type=\"hidden\" name=\"searchtext" + (iSegment + 1) + "\" value=\"\">" + "\r\n");
            i = System.Convert.ToInt32(i + 65000);
            iSegment = System.Convert.ToInt32(iSegment + 1);
        }
        iPackLoop = 1;
        if (editorPackage.Length > iMaxContLength)
        {
            var1 = editorPackage.Length;
        }
        else
        {
            var1 = iMaxContLength;
        }
        while (iPackLoop <= var1)
        {
            result.Append("<input type=\"hidden\" name=\"hiddenpackage" + (iSegment2 + 1) + "\" value=\"\">" + "\r\n");
            iPackLoop = System.Convert.ToInt32(iPackLoop + 65000);
            iSegment2 = System.Convert.ToInt32(iSegment2 + 1);
        }
        result.Append("<input type=\"hidden\" name=\"numberoffields\" value=\"" + iSegment + "\"> <input type=\"hidden\" name=\"hiddenPackageSize\" value=\"" + iSegment2 + "\">" + "\r\n");
        return (result.ToString());
    }

    private string SetActionClientScript(bool publishAsHtml, bool isSmartFormContent)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
        {
            result.Append("function launchLibrary(){" + "\r\n");
            result.Append(" librarySelectedText(\'\');	" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function lauchLibraryHTML(){" + "\r\n");
            result.Append(" elx1.GetSelectedText (\'librarySelectedText\');" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function lauchWikipopup(){" + "\r\n");
            result.Append(" elx1.GetSelectedText (\'wikiSelectedText\');" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function wikiSelectedText(src){" + "\r\n");
            result.Append(" document.getElementById(\'selectedhtml\').value = src;" + "\r\n");
            result.Append(" src = $ektron.removeTags(src);" + "\r\n");
            result.Append(" document.getElementById(\'selectedtext\').value = src;" + "\r\n");
            result.Append(" var remote=null;" + "\r\n");
            result.Append(" var link = \"ewebeditpro/wikipopup.aspx?FolderID=" + m_intContentFolder + "&wikititle=\" + src" + "\r\n");
            result.Append(" remote = window.open(link,\'EditWikiLink\',\'toolbar=0,location=0,directories=0,menubar=0,scrollbars=1,resizable=1,width=680,height=385\');" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function librarySelectedText(src){" + "\r\n");
            result.Append(" document.forms[0].selectedtext.value = src;" + "\r\n");
            result.Append(" var remote=null;" + "\r\n");
            result.Append(" remote = window.open(\"mediamanager.aspx?actiontype=library&scope=all&autonav=\",\'Preview\',\'width=\' + 600 + \',height=\' + 400 +\',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no\');" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function insertImage(src,linktitle){" + "\r\n");
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(\"<img src=\\\"\" + src + \"\\\" alt=\\\"\" + linktitle+ \"\\\" title=\\\"\" + linktitle + \"\\\">\"));" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function insertOther(src,linktitle){" + "\r\n");
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(\"<a href=\\\"\" + src + \"\\\">\" + linktitle + \"</a>\"));" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function insertHTML(html){" + "\r\n");
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(html));" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("function GetMACContent(src){" + "\r\n");
            result.Append(" document.forms[0].content_teaser.value = src;" + "\r\n");
            result.Append(" elx1.GetBody(\'SetAction\');" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function VerifyManagedFileName() {" + "\r\n");
            result.Append(" var fileupload = document.getElementById(\'fileupload\'); " + "\r\n");
            result.Append(" if ((fileupload != null) && (fileupload.value.length > 0)) { " + "\r\n");
            result.Append("   var objvalidTypes = document.getElementById(\'validTypes\'); " + "\r\n");
            result.Append("   var fileUploadExtIndex = fileupload.value.lastIndexOf(\'.\'); " + "\r\n");
            result.Append("   var fileUploadExt = fileupload.value.substring(fileUploadExtIndex + 1); " + "\r\n");
            result.Append("   var arrTypes = objvalidTypes.value.split(\',\');" + "\r\n");
            result.Append("   var found = false;" + "\r\n");
            result.Append("   var i = 0;" + "\r\n");
            result.Append("   for (i = 0; i < arrTypes.length; ++i) {" + "\r\n");
            result.Append("     if (arrTypes[i].toLowerCase() == fileUploadExt.toLowerCase()) { " + "\r\n");
            result.Append("         found = true;" + "\r\n");
            result.Append("         break;" + "\r\n");
            result.Append("     }" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("   if (!(found)) { " + "\r\n");
            result.Append("     alert(\'" + m_refMsg.GetMessage("lbl invalid file type") + "\');" + "\r\n");
            result.Append("     return false;" + "\r\n");
            result.Append("   } " + "\r\n");
            result.Append("   var oldfilename = document.getElementById(\'oldfilename\'); " + "\r\n");
            result.Append("   if ((oldfilename != null) && (oldfilename.value.length > 0)) { " + "\r\n");
            result.Append("          var justfilename = fileupload.value.match(/(.*)[\\/\\\\]([^\\/\\\\]+\\.\\w+)$/); " + "\r\n");
            result.Append("         if ((justfilename[2] != null) && (justfilename[2].length > 0) && (oldfilename.value.toLowerCase() != justfilename[2].toLowerCase())) { " + "\r\n");
            //-------------------Defect 65842   edit-----------------------
            //result.Append("             alert(\'" + m_refMsg.GetMessage("js:cannot replace provide original file") + "\' + oldfilename.value);" + "\r\n");
            result.Append("             var blnAnswer;" + "\r\n");
            result.Append("             blnAnswer=confirm('" + m_refMsg.GetMessage("lbl would you like to replace") + " ' + oldfilename.value + ' " + m_refMsg.GetMessage("lbl with") + " ' + fileupload.value + '?');" + "\r\n");
            result.Append("             if (false==blnAnswer) {" + "\r\n");
            //----------------------Defect 65842 end -----------------------
            result.Append("                 return false;" + "\r\n");
            result.Append("         }" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("}" + "\r\n");
            //--------------------Defect 65842   edit----------------------
            result.Append("}" + "\r\n");
            //-------------------------Defect 65842 end--------------------          
         
            result.Append("if ((fileupload != null) && (fileupload.value.length <= 0)) { " + "\r\n");
            result.Append("   var editmode = document.getElementById(\'type\'); " + "\r\n");
            result.Append("   if ((editmode != null) && (editmode.value.length > 0) && (editmode.value.toLowerCase() == \'add\')) { " + "\r\n");
            result.Append("     alert(\'" + m_refMsg.GetMessage("lbl upload file") + "\');" + "\r\n");
            result.Append("     return false;" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("return true;" + "\r\n");
            result.Append("}" + "\r\n");

            result.Append("function SetAction(src) { " + "\r\n");
            result.Append("if (src != \'cancel\') {" + "\r\n");
            result.Append("  if (false==validateContentTitle()) {return false}; " + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("if (false==VerifyManagedFileName) {return false;}} " + "\r\n");
            result.Append("if (IsBrowserIE()) {" + "\r\n");
            result.Append("if (false==IsCmsEditEnable) {return false;}}" + "\r\n");
            result.Append("Button = buttonaction;" + "\r\n");
        }
        else
        {
            result.Append("var blnAsked=false;" + "\r\n");

            result.Append("function VerifyManagedFileName() {" + "\r\n");
            result.Append(" var fileupload = document.getElementById(\'fileupload\'); " + "\r\n");
            result.Append(" if ((fileupload != null) && (fileupload.value.length > 0)) { " + "\r\n");
            result.Append("   var objvalidTypes = document.getElementById(\'validTypes\'); " + "\r\n");
            result.Append("   var fileUploadExtIndex = fileupload.value.lastIndexOf(\'.\'); " + "\r\n");
            result.Append("   var fileUploadExt = fileupload.value.substring(fileUploadExtIndex + 1); " + "\r\n");
            result.Append("   var arrTypes = objvalidTypes.value.split(\',\');" + "\r\n");
            result.Append("   var found = false;" + "\r\n");
            result.Append("   var i = 0;" + "\r\n");
            result.Append("   for (i = 0; i < arrTypes.length; ++i) {" + "\r\n");
            result.Append("     if (arrTypes[i].toLowerCase() == fileUploadExt.toLowerCase()) { " + "\r\n");
            result.Append("         found = true;" + "\r\n");
            result.Append("         break;" + "\r\n");
            result.Append("     }" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("   if (!(found)) { " + "\r\n");
            result.Append("     alert(\'" + m_refMsg.GetMessage("lbl invalid file type") + "\');" + "\r\n");
            result.Append("     return false;" + "\r\n");
            result.Append("   } " + "\r\n");
            result.Append("   var oldfilename = document.getElementById(\'oldfilename\'); " + "\r\n");
            result.Append("         var justfilename = \'\'; " + "\r\n");
            result.Append("         if (IsFireFox() && GetFireFoxVersion() >= 3) { justfilename = fileupload.value; } else {" + "\r\n");
            result.Append("             var tmpPath = fileupload.value.match(/(.*)[\\/\\\\]([^\\/\\\\]+\\.\\w+)$/); justfilename = tmpPath[2];}" + "\r\n");
            result.Append("         if ((justfilename != null) && (justfilename.length > 0) && (justfilename.indexOf(\'&\') > -1 || justfilename.indexOf(\'+\') > -1 )) { " + "\r\n");
            result.Append("             alert(\'" + m_refMsg.GetMessage("js:cannot add file with add and plus") + "\');" + "\r\n");
            result.Append("                 return false;" + "\r\n");
            result.Append("         }" + "\r\n");
            result.Append("   if ((oldfilename != null) && (oldfilename.value.length > 0)) { " + "\r\n");
            result.Append("         if ((justfilename != null) && (justfilename.length > 0) && (oldfilename.value.toLowerCase() != justfilename.toLowerCase())) { " + "\r\n");
            //-------------------Defect 65842   edit-----------------------
            // result.Append("             alert(\'" + m_refMsg.GetMessage("js:cannot replace provide original file") + "\' + oldfilename.value);" + "\r\n");
            result.Append("             var blnAnswer;" + "\r\n");
            result.Append("             blnAnswer=confirm('" + m_refMsg.GetMessage("lbl would you like to replace") + " ' + oldfilename.value + ' " + m_refMsg.GetMessage("lbl with") + " ' + fileupload.value + '?');" + "\r\n");
            result.Append("             if (false==blnAnswer) {" + "\r\n");
            //----------------------Defect 65842 end -----------------------
            result.Append("                 return false;" + "\r\n");
            result.Append("         }" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("}" + "\r\n");
            //--------------------Defect 65842   edit----------------------
            result.Append("}" + "\r\n");
            //-------------------------Defect 65842 end--------------------
           
            //in case of add new asset/translate asset verify file was selected (type=add here)
            result.Append("if ((fileupload != null) && (fileupload.value.length <= 0)) { " + "\r\n");
            result.Append("   var editmode = document.getElementById(\'type\'); " + "\r\n");
            result.Append("   if ((editmode != null) && (editmode.value.length > 0) && (editmode.value.toLowerCase() == \'add\')) { " + "\r\n");
            result.Append("     alert(\'" + m_refMsg.GetMessage("lbl upload file") + "\');" + "\r\n");
            result.Append("     return false;" + "\r\n");
            result.Append("   }" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("return true;" + "\r\n");
            result.Append("}" + "\r\n");

            //Preview Content Code Starts
            bool hasDeviceConfig = m_refContApi.EkContentRef.HasDeviceConfigurations();
            result.Append("function PreviewContent(obj, contTitle,folderId, contentId) { " + Constants.vbCrLf);
            result.Append(" SetAction('save');" + Constants.vbCrLf);
            if (hasDeviceConfig && m_refContApi.RequestInformationRef.IsDeviceDetectionEnabled)
            {
                result.Append(" window.open('").Append(m_refContApi.ApplicationPath).Append("devicepreview.aspx?cid=' + contentId + '&fid=' + folderId,'Preview400','left=300, top=200, width=400, height=150');" + Constants.vbCrLf);
            }
            else
            {
                result.Append(" window.open(obj,\"contTitle\",'scrollbars=yes,resizable=yes');" + Constants.vbCrLf);
            }

            result.Append(" return false;" + "\r\n");
            result.Append("}" + "\r\n");
            //Preview Content Code Ends

            result.Append("function SetAction(Button) { " + "\r\n");
            if (true == isSmartFormContent) //only for smart form and reset the flag to continue.
            {
                result.Append(" if (false == isReadyEditor) { " + "\r\n");
                result.Append(" alert(\"" + m_refMsg.GetMessage("lbl wait editor not loaded") + "\"); " + "\r\n");
                result.Append(" isReadyEditor = true; " + "\r\n");
                result.Append(" return false; " + "\r\n");
                result.Append(" } " + "\r\n");
            }
            result.Append(" $ektron(\'#pleaseWait\').modalShow(); " + "\r\n");
            result.Append("if (Button != \'cancel\') {" + "\r\n");
            result.Append("  if (false==validateContentTitle()) {return false}; " + "\r\n");
            result.Append("}" + "\r\n");
            if (1 == lContentType)
            {
                result.Append("if (false == bContentEditorReady || false == bTeaserEditorReady)" + "\r\n");
                result.Append("{" + "\r\n");
                result.Append("     return false; " + "\r\n");
                result.Append("}" + "\r\n");
            }
            else if (2 == lContentType)
            {
                result.Append("if (false == bFormEditorReady || false == bResponseEditorReady)" + "\r\n");
                result.Append("{" + "\r\n");
                result.Append("     return false; " + "\r\n");
                result.Append("}" + "\r\n");
            }
            else
            {
                result.Append("if (false == bTeaserEditorReady)" + "\r\n");
                result.Append("{" + "\r\n");
                result.Append("     return false; " + "\r\n");
                result.Append("}" + "\r\n");
            }
            result.Append("var blnAnswer;" + "\r\n");
            result.Append("if ((\'cancel\' == Button) && (blnAsked==false)) {" + "\r\n");
            result.Append("blnAnswer=confirm(\"" + m_refMsg.GetMessage("js: alert confirm close no save") + "\");" + "\r\n");
            result.Append("if (false==blnAnswer) {" + "\r\n");
            result.Append("$ektron(\'#pleaseWait\').modalHide(); " + "\r\n");
            result.Append("return false;" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("else {" + "\r\n");
            result.Append("if(\"undefined\" != typeof(eWebEditPro)){eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_NOSAVE;}" + "\r\n");
            result.Append(" if (\"object\" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " + "\r\n");
            result.Append("     var objContent = Ektron.ContentDesigner.instances[\"content_html\"]; " + "\r\n");
            result.Append("		if (objContent) " + "\r\n");
            result.Append("		{ " + "\r\n");
            result.Append("		    objContent.isChanged = false; " + "\r\n");
            result.Append("		} " + "\r\n");
            result.Append("     var objTeaser = Ektron.ContentDesigner.instances[\"content_teaser\"]; " + "\r\n");
            result.Append("		if (objTeaser) " + "\r\n");
            result.Append("		{ " + "\r\n");
            result.Append("		    objTeaser.isChanged = false; " + "\r\n");
            result.Append("		} " + "\r\n");
            result.Append("     var objFormT = Ektron.ContentDesigner.instances[\"forms_transfer\"]; " + "\r\n");
            result.Append("		if (objFormT) " + "\r\n");
            result.Append("		{ " + "\r\n");
            result.Append("		    objFormT.isChanged = false; " + "\r\n");
            result.Append("		} " + "\r\n");
            result.Append("     var objFormR = Ektron.ContentDesigner.instances[\"forms_redirect\"]; " + "\r\n");
            result.Append("		if (objFormR) " + "\r\n");
            result.Append("		{ " + "\r\n");
            result.Append("		    objFormR.isChanged = false; " + "\r\n");
            result.Append("		} " + "\r\n");
            result.Append(" }" + "\r\n");
            result.Append(" blnAsked=true;};" + "\r\n");
            result.Append("}" + "\r\n");
            result.Append("if (IsBrowserIE()) {" + "\r\n");
            result.Append("if (false==IsCmsEditEnable) {return false;}}" + "\r\n");
        }
        if (folder_data.CategoryRequired == true && m_refContent.GetAllFolderTaxonomy(m_intContentFolder).Length > 0)
        {
            result.Append("      if ((Button != \'cancel\') && (Trim(document.getElementById(\'taxonomyselectedtree\').value) == \'\')) { ").Append(Environment.NewLine);
            result.Append("         alert(\'" + m_refMsg.GetMessage("js tax cat req") + "\'); ").Append(Environment.NewLine);
            result.Append("         $ektron(\'.tabContainer\').tabs(\'select\', \'dvTaxonomy\'); ").Append(Environment.NewLine);
            result.Append("         $ektron(\'#dvTaxonomy\').focus(); ").Append(Environment.NewLine);
            result.Append("         $ektron(\'#pleaseWait\').modalHide(); ").Append(Environment.NewLine);
            result.Append("         return false; ").Append(Environment.NewLine);
            result.Append("      } ").Append(Environment.NewLine);
        }
        if (folder_data.AliasRequired == true && m_urlAliasSettings.IsManualAliasEnabled && m_refContApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias) && Request.QueryString["type"] != "multiple,add" && lContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) 
        {
            bool addAliasScript = true;
            if(content_edit_data != null && content_edit_data.AssetData != null)
                addAliasScript = !Ektron.Cms.Common.EkFunctions.IsImage((string)("." + content_edit_data.AssetData.FileExtension));
			
            if (addAliasScript)
            {
                result.Append("      var manualAlias = document.getElementById(\'frm_manalias\').value;").Append(Environment.NewLine);
                result.Append("      if ((Button != \'cancel\') && manualAlias == '') { ").Append(Environment.NewLine);
                result.Append("         alert(\'Manual alias name is required\'); ").Append(Environment.NewLine);
                result.Append("         $ektron(\'.tabContainer\').tabs(\'select\', \'dvAlias\'); ").Append(Environment.NewLine);
                result.Append("         $ektron(\'#dvAlias\').focus(); ").Append(Environment.NewLine);
                result.Append("         $ektron(\'#pleaseWait\').modalHide(); ").Append(Environment.NewLine);
                result.Append("         return false; ").Append(Environment.NewLine);
                result.Append("      } ").Append(Environment.NewLine);
            }
            
        }
        result.Append("		if (Button != \'cancel\') {" + "\r\n");
        result.Append("         if (!VerifyManagedFileName()) " + "\r\n");
        result.Append("		    {" + "\r\n");
        result.Append("		    	buttonPressed = false;" + "\r\n");
        result.Append("             $ektron(\'#pleaseWait\').modalHide(); " + "\r\n");
        result.Append("		    	return false;" + "\r\n");
        result.Append("		    }" + "\r\n");
        result.Append("		    if (!ValidateMeta(0))" + "\r\n");
        result.Append("		    {" + "\r\n");
        result.Append("		    	buttonPressed = false;" + "\r\n");
        result.Append("             $ektron(\'#pleaseWait\').modalHide(); " + "\r\n");
        result.Append("		    	return false;" + "\r\n");
        result.Append("		    }" + "\r\n");
        result.Append("		}" + "\r\n");

        result.Append("valAndSaveCSubAssignments();" + "\r\n");
        if (!Utilities.IsAssetType(lContentType))
        {
            result.Append("if (typeof g_AssetHandler == \"object\")" + "\r\n");
            result.Append("{" + "\r\n");
        }
        result.Append(" var sContentTitle = \"\";" + "\r\n");
        result.Append(" if (Button != \"cancel\")" + "\r\n");
        result.Append(" {" + "\r\n");
        result.Append("     if (CheckTitle())" + "\r\n");
        result.Append("     {" + "\r\n");
        //content_title is not there in add multiple dms documents.
        result.Append("         if (document.forms[0].content_title != null) { " + "\r\n");
        result.Append("            sContentTitle = document.forms[0].content_title.value.replace(/\"/gi, \"\'\");" + "\r\n");
        result.Append("            sContentTitle = document.forms[0].content_title.value.replace(/\\&/g, \"&amp;\");	" + "\r\n");
        result.Append("         }" + "\r\n");
        result.Append("     }" + "\r\n");
        result.Append("     else" + "\r\n");
        result.Append("     {" + "\r\n");
        result.Append("$ektron(\'#pleaseWait\').modalHide(); " + "\r\n");
        result.Append("         buttonPressed = false;" + "\r\n");
        result.Append("         return (false);" + "\r\n");
        result.Append("     }" + "\r\n");
        result.Append(" }" + "\r\n");
        result.Append("	  var bDMSNoEditor = false;" + "\r\n");
        if (m_SelectedEditControl != "ContentDesigner")
        {
            result.Append("   var objTeaser = eWebEditPro.instances[\"content_teaser\"];" + "\r\n");
            result.Append("             if (objTeaser && objTeaser.isEditor()){" + "\r\n");
            result.Append("					if (!objTeaser.save()) { " + "\r\n");
            result.Append("						buttonPressed = false; " + "\r\n");
            result.Append("						return (false); " + "\r\n");
            result.Append("						} " + "\r\n");
            result.Append("				} " + "\r\n");
        }
        else
        {
            result.Append("				if (\"object\" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " + "\r\n");
            result.Append("					bDMSNoEditor = true;" + "\r\n");
            result.Append("					objTeaser = Ektron.ContentDesigner.instances[\"content_teaser\"]; " + "\r\n");
            result.Append("					if (objTeaser) " + "\r\n");
            result.Append("					{ " + "\r\n");
            result.Append("					document.forms[0].content_teaser.value = objTeaser.getContent();  " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("				} " + "\r\n");
        }
        result.Append("				else { " + "\r\n");
        result.Append("					bDMSNoEditor = true;" + "\r\n");
        result.Append("                 if(document.forms[0].content_teaser && document.forms[0].ewepcontent_teaser)" + "\r\n");
        result.Append("                 {" + "\r\n");
        result.Append("					    document.forms[0].content_teaser.value = document.forms[0].ewepcontent_teaser.value;" + "\r\n");
        result.Append("                 }" + "\r\n");
        result.Append("				} " + "\r\n");



        if (m_strAssetFileName.Trim() != "")
        {
            result.Append("var strAssetTitle=\'" + m_strAssetFileName.Replace("\'", "\\\'") + "\';" + "\r\n");
        }
        else
        {
            result.Append("var strAssetTitle=sContentTitle;" + "\r\n");
        }
        if (m_strType == "update")
        {

            if ((Request.QueryString["multi"] == null || "" == Request.QueryString["multi"]) && this.content_edit_data.ContType == Ektron.Cms.Common.EkConstants.CMSContentType_Media)
            {
                result.Append("if (!saveMultimediaObjectsXML(Button)){ return false; }" + "\r\n");
            }
        }
        result.Append("		 		document.forms[0].editaction.value = Button; " + "\r\n");
        if (m_strType == "multiple,add")
        {
            result.Append("if (Button != \"cancel\")" + "\r\n");
            result.Append("{" + "\r\n");
            if (Request.Cookies[DMSCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[DMSCookieName].Value))
            {
                if (Request.Cookies[DMSCookieName].Value != "2010")
                {
                    result.Append("     DocumentUpload(); " + "\r\n");
                }
            }
            else
            {
                result.Append("     DocumentUpload(); " + "\r\n");
            }
            
            result.Append("}" + "\r\n");
        }
        result.Append("             if (Button === \"cancel\") { if (document.getElementById(\'fileUploadWrapper\') != null) { document.getElementById(\'fileUploadWrapper\').innerHTML = \'<input type=\"file\" id=\"fileupload\" />\';} }" + "\r\n");
        result.Append("             if (Button != \"cancel\")" + "\r\n");
        result.Append("				    DisplayHoldMsg_Local(true); " + "\r\n");
        string queryStr = (string)(this.m_bClose ? "?close=true" : "");
        if ((!(Request.QueryString["pullapproval"] == null)) && (Request.QueryString["pullapproval"].Length > 0))
        {
            if (queryStr.Length > 0)
            {
                queryStr += (string)("&pullapproval=" + Request.QueryString["pullapproval"]);
            }
            else
            {
                queryStr += (string)("?pullapproval=" + Request.QueryString["pullapproval"]);
            }
        }
        if ((!(Request.QueryString["taxoverride"] == null)) && (Request.QueryString["taxoverride"].Length > 0))
        {
            if (queryStr.Length > 0)
            {
                queryStr += (string)("&taxoverride=" + Request.QueryString["taxoverride"]);
            }
            else
            {
                queryStr += (string)("?taxoverride=" + Request.QueryString["taxoverride"]);
            }
        }
        if (TaxonomySelectId > 0)
        {
            if (queryStr.Length > 0)
            {
                queryStr += (string)("&SelTaxonomyId=" + TaxonomySelectId);
            }
            else
            {
                queryStr += (string)("?SelTaxonomyId=" + TaxonomySelectId);
            }
        }
        result.Append("		 		document.forms[0].action = \"processupload.aspx" + queryStr + "\"; " + "\r\n");
        if (!String.IsNullOrEmpty(Request.QueryString["FromEE"])) //If the page is opened from Ektron Explorer we need to close the page instead of returning
        {
            result.Append("document.forms[0].FromEE.value = \'true\'; ");
        }
        
        result.Append("		 		ektronFormSubmit(); " + "\r\n");
        result.Append("             return (false); " + "\r\n");
        if (!Utilities.IsAssetType(lContentType))
        {
            result.Append("}" + "\r\n");
        }
        result.Append("if (\"workoffline\" == Button)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append(" document.forms[0].elements[\"type\"].value = \"\";" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("if (\"savelocalcopy\" == Button)" + "\r\n");
        result.Append("{" + "\r\n");
        result.Append(" document.forms[0].elements[\"type\"].value = \"\";" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("    if (buttonPressed != false) { " + "\r\n");
        result.Append("	    return (false); " + "\r\n");
        result.Append("    } " + "\r\n");
        if (IsMac && m_SelectedEditControl != "ContentDesigner")
        {
            result.Append("    buttonPressed = true; " + "\r\n");
            result.Append("    if (Button == \"cancel\") { " + "\r\n");
            result.Append("    ResizeFrame(1); // Show the navigation-tree frame. " + "\r\n");
            result.Append("    for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = \'\'\"); " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				document.forms[0].editaction.value = Button; " + "\r\n");
            result.Append("				document.forms[0].submit(); " + "\r\n"); // no ektronFormSubmit needed
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("		if (!ValidateMeta(0))" + "\r\n");
            result.Append("		{" + "\r\n");
            result.Append("			buttonPressed = false;" + "\r\n");
            result.Append("			return false;" + "\r\n");
            result.Append("		}" + "\r\n");
            result.Append(" editorEstimateContentSize = false;" + "\r\n");
            result.Append(" if (false==CheckContentSize())" + "\r\n");
            result.Append(" {" + "\r\n");
            result.Append("   buttonPressed =false;" + "\r\n");
            result.Append("   return false;" + "\r\n");
            result.Append(" }" + "\r\n");
            result.Append("	if(!CheckAllRequiredFields()){ " + "\r\n");
            result.Append("     buttonPressed = false;" + "\r\n");
            result.Append("     $ektron('#pleaseWait').modalHide();" + "\r\n");
            result.Append("     return false;" + "\r\n");
            result.Append(" }" + "\r\n");
            result.Append("			if ((ecmMetaComplete == 0) && (Button == \"publish\")) { " + "\r\n");
            result.Append("				DisplayMetaIncomplete(); " + "\r\n");
            result.Append("				buttonPressed = false; " + "\r\n");
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("			if (CheckTitle()) { " + "\r\n");
            result.Append("				DisplayHoldMsg_Local(true);	 " + "\r\n");
            result.Append("				var SavePosition; " + "\r\n");
            result.Append("				var SaveContentLength; " + "\r\n");
            result.Append("				var SaveSearchLength; " + "\r\n");
            result.Append("				var HowMuchToSave; " + "\r\n");
            result.Append("				var iLoop; " + "\r\n");
            result.Append("				regexp1 = /\"/gi; " + "\r\n");
            result.Append("             if (document.forms[0].content_title != null) { " + "\r\n");
            result.Append("				   document.forms[0].content_title.value = document.forms[0].content_title.value.replace(regexp1, \"\'\"); " + "\r\n");
            result.Append("             } " + "\r\n");
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(regexp1, \"\'\"); " + "\r\n");
            result.Append("				var saveContentObj; " + "\r\n");
            result.Append("				var saveSearchObj; " + "\r\n");
            result.Append("				saveContentObj = \"\"; " + "\r\n");
            result.Append("				saveSearchObj = \"\"; " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
            {
                result.Append("				saveContentObj = src; " + "\r\n");
            }
            else
            {
                result.Append("				saveContentObj = document.forms[0].content_html.value; " + "\r\n");
            }
            result.Append("				saveSearchObj = $ektron.removeTags(saveContentObj); " + "\r\n");
            result.Append("				SaveContentLength = saveContentObj.length; " + "\r\n");
            result.Append("				SaveSearchLength = saveSearchObj.length; " + "\r\n");
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = \'\'\"); " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            result.Append("				for(SavePosition = 0; SavePosition < SaveContentLength; SavePosition += 65000) { " + "\r\n");
            result.Append("					if ((SaveContentLength - SavePosition) < 65000) { " + "\r\n");
            result.Append("						HowMuchToSave = (SaveContentLength - SavePosition); " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					else { " + "\r\n");
            result.Append("						HowMuchToSave = 65000; " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = saveContentObj.substring(\" + SavePosition + \",\" + (SavePosition + HowMuchToSave) + \");\"); " + "\r\n");
            result.Append("					iLoop += 1; " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            result.Append("				for(SavePosition = 0; SavePosition < SaveSearchLength; SavePosition += 65000) { " + "\r\n");
            result.Append("					if ((SaveSearchLength - SavePosition) < 65000) { " + "\r\n");
            result.Append("						HowMuchToSave = (SaveSearchLength - SavePosition); " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					else { " + "\r\n");
            result.Append("						HowMuchToSave = 65000; " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					eval(\"document.forms[0].searchtext\" + iLoop + \".value = saveSearchObj.substring(\" + SavePosition + \",\" + (SavePosition + HowMuchToSave) + \");\"); " + "\r\n");
            result.Append("					iLoop += 1; " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				document.forms[0].hiddencontentsize.value = SaveContentLength; " + "\r\n");
            result.Append("				document.forms[0].hiddensearchsize.value = SaveSearchLength; " + "\r\n");
            if (((Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("MSIE") + 1 == 0) && ((Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("4.7") + 1 > 0))
            {
                result.Append("					document.forms[0].netscape.value = \"\"; " + "\r\n");
            }
            result.Append("				ResizeFrame(1); // Show the navigation-tree frame. " + "\r\n");
            result.Append("				document.forms[0].editaction.value = Button; " + "\r\n");

            result.Append("		 		ektronFormSubmit(); " + "\r\n");
            result.Append("            return (false); " + "\r\n");
            result.Append("			}   " + "\r\n");
            result.Append("			else  { " + "\r\n");
            result.Append("				buttonPressed = false; " + "\r\n");
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("		} " + "\r\n");

        }
        else
        {
            ValidateContentPanel.Visible = true;
            result.Append("			buttonPressed = true; " + "\r\n");
            result.Append("			if (Button == \"cancel\") { " + "\r\n");
            result.Append("				DisplayHoldMsg_Local(true);" + "\r\n");
            result.Append("				ResizeFrame(1); // Show the navigation-tree frame. " + "\r\n");
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = \'\'\"); " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				document.forms[0].editaction.value = Button; " + "\r\n");
            if (m_SelectedEditControl != "ContentDesigner")
            {
                result.Append("				ShutdownImageEditor(); " + "\r\n");
                result.Append("				//eWebEditPro.instances[\"content_teaser\"].editor.Clear(); " + "\r\n");
                result.Append("				//eWebEditPro.instances[\"content_html\"].editor.Clear(); " + "\r\n");
            }
            // The following TRY/CATCH pair is to catch the "unspecific error" that IE6 throw when hitting the cancel button
            // in the onbeforeunload confirm box.
            // similar cases were found at http://dbforums.com/showthread.php?threadid=483187
            result.Append("             try                                                 " + "\r\n");
            result.Append("             {                                                   " + "\r\n");
            result.Append("				    document.forms[0].submit();                      " + "\r\n"); // no ektronFormSubmit needed for cancel
            result.Append("             }                                                   " + "\r\n");
            result.Append("             catch (e)                                           " + "\r\n");
            result.Append("             {                                                   " + "\r\n");
            result.Append("                 // ignore the error if it fails to submit.      " + "\r\n");
            result.Append("             }                                                   " + "\r\n");
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("			var bEditorNeeded = true;" + "\r\n");
            result.Append("			if (\"boolean\" == typeof bDMSNoEditor) {" + "\r\n");
            result.Append("				if (true == bDMSNoEditor) {bEditorNeeded = false;}" + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("			else if (\"object\" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " + "\r\n");
            result.Append("				bEditorNeeded = false;" + "\r\n");
            result.Append("			} " + "\r\n");
            if (m_SelectedEditControl != "ContentDesigner")
            {
                result.Append("			if (!eWebEditPro.isInstalled && bEditorNeeded) { " + "\r\n");
                result.Append("				if(window.navigator.userAgent.search(\"MSIE\") == -1) { " + "\r\n");
                result.Append("					alert(\"" + m_refMsg.GetMessage("js: netscape editor not loaded") + "\"); " + "\r\n");
                result.Append("				} " + "\r\n");
                result.Append("				else { " + "\r\n");
                result.Append("					if(confirm(\"" + m_refMsg.GetMessage("js: editor not loaded") + "\")) { " + "\r\n");
                result.Append("						self.location.reload(); " + "\r\n");
                result.Append("					} " + "\r\n");
                result.Append("				} " + "\r\n");
                result.Append("				buttonPressed = false; " + "\r\n");
                result.Append("				return (false); " + "\r\n");
                result.Append("			} " + "\r\n");
                result.Append("			ShutdownImageEditor();		 " + "\r\n");
            }
            result.Append(" editorEstimateContentSize = false;" + "\r\n");
            result.Append(" if (false==CheckContentSize())" + "\r\n");
            result.Append(" {" + "\r\n");
            result.Append("   buttonPressed =false;" + "\r\n");
            result.Append("   return false;" + "\r\n");
            result.Append(" }" + "\r\n");
            result.Append("	            if(!CheckAllRequiredFields()){ " + "\r\n");
            result.Append("             buttonPressed = false;" + "\r\n");
            result.Append("             $ektron('#pleaseWait').modalHide();" + "\r\n");
            result.Append("             return false;" + "\r\n");
            result.Append("         }" + "\r\n");
            result.Append("			if ((ecmMetaComplete == 0) && (Button == \"publish\")) { " + "\r\n");
            result.Append("				DisplayMetaIncomplete(); " + "\r\n");
            result.Append("				buttonPressed = false; " + "\r\n");
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("			if (CheckTitle(Button)) { " + "\r\n");
            result.Append("				DisplayHoldMsg_Local(true); " + "\r\n");
            result.Append("				var SavePosition; " + "\r\n");
            result.Append("				var SaveContentLength; " + "\r\n");
            result.Append("				var SaveSearchLength; " + "\r\n");
            result.Append("				var HowMuchToSave; " + "\r\n");
            result.Append("				var iLoop; " + "\r\n");
            result.Append("				regexp1 = /\"/gi; " + "\r\n");
            //content_title is not there in add multiple dms documents.
            result.Append("             if (document.forms[0].content_title != null) { " + "\r\n");
            result.Append("				    document.forms[0].content_title.value = document.forms[0].content_title.value.replace(regexp1, \"\'\"); " + "\r\n");
            result.Append("                 document.forms[0].content_title.value = document.forms[0].content_title.value.replace(/\\&/g, \"&amp;\");" + "\r\n");
            result.Append("             } " + "\r\n");
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(regexp1, \"\'\"); " + "\r\n");
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(/\\&/g, \"&amp;\"); " + "\r\n");
            result.Append("				var saveContentObj = new Object(); " + "\r\n");
            result.Append("				var saveSearchObj = new Object(); " + "\r\n");
            result.Append("				var saveTeaser = new Object(); " + "\r\n");
            result.Append("				saveContentObj.value = \"\"; " + "\r\n");
            result.Append("				saveSearchObj.value = \"\"; " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            if (m_SelectedEditControl != "ContentDesigner")
            {
                result.Append("             var objTeaser = eWebEditPro.instances[\"content_teaser\"];" + "\r\n");
                result.Append("             if (objTeaser){" + "\r\n");
                result.Append("					if (!objTeaser.save(undefined, undefined, undefined, false)) { " + "\r\n"); // non-validating save, already done at ValidateContent
                result.Append("				        DisplayHoldMsg_Local(false); " + "\r\n");
                result.Append("						buttonPressed = false; " + "\r\n");
                result.Append("						return (false); " + "\r\n");
                result.Append("						} " + "\r\n");
                result.Append("				} " + "\r\n");
                result.Append("             var objInstance = eWebEditPro.instances[\"content_html\"];" + "\r\n");
                result.Append("            if (objInstance){" + "\r\n");
                if (bVer4Editor)
                {
                    result.Append("					saveContentObj.value = objInstance.editor.GetContent(\"databody\");					 " + "\r\n");

                }
                else
                {
                    result.Append("					if (!objInstance.save(saveContentObj, undefined, undefined, false)) { " + "\r\n"); // non-validating save, already done at ValidateContent
                    result.Append("				        DisplayHoldMsg_Local(false); " + "\r\n");
                    result.Append("						buttonPressed = false; " + "\r\n");
                    result.Append("						return (false); " + "\r\n");
                    result.Append("						} " + "\r\n");
                }
                result.Append("}");
            }
            if (bVer4Editor == false)
            {
                if (m_strSchemaFile.Length > 0)
                {
                    result.Append("  							var objXmlDoc =  objInstance.editor.XMLProcessor(); " + "\r\n");
                    result.Append("         					var sXMLString = saveContentObj.value; " + "\r\n");
                    result.Append("							var sSchemaPath = \"" + m_strSchemaFile + "\"; " + "\r\n");
                    result.Append("   							var sNSTarget =  \"" + m_strNamespaceFile + "\"; " + "\r\n");
                    result.Append("							objXmlDoc.Validate(sXMLString, sSchemaPath, sNSTarget); " + "\r\n");
                    result.Append("    						if(objXmlDoc.getPropertyInteger(\"ErrorCode\") == 0) { " + "\r\n");
                    result.Append("	    							// alert(\"Passed!\"); " + "\r\n");
                    result.Append("   							} " + "\r\n");
                    result.Append("    						else { " + "\r\n");
                    result.Append("								DisplayHoldMsg_Local(false); " + "\r\n");
                    result.Append("     						alert(objXmlDoc.getPropertyString(\"ErrorReason\")); " + "\r\n");
                    result.Append("								buttonPressed = false; " + "\r\n");
                    result.Append("								return (false); " + "\r\n");
                    result.Append("	     					} " + "\r\n");
                }
            }
            result.Append("				//Workaround remove html and xml tags from the content." + "\r\n");
            result.Append("				saveSearchObj.value = $ektron.removeTags(saveContentObj.value);" + "\r\n");
            result.Append("				SaveContentLength = saveContentObj.value.length; " + "\r\n");
            result.Append("				SaveSearchLength = saveSearchObj.value.length; " + "\r\n");
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = \'\'\"); " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            result.Append("				for(SavePosition = 0; SavePosition < SaveContentLength; SavePosition += 65000) { " + "\r\n");
            result.Append("					if ((SaveContentLength - SavePosition) < 65000) { " + "\r\n");
            result.Append("						HowMuchToSave = (SaveContentLength - SavePosition); " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					else { " + "\r\n");
            result.Append("						HowMuchToSave = 65000; " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					eval(\"document.forms[0].hiddencontent\" + iLoop + \".value = saveContentObj.value.substring(\" + SavePosition + \",\" + (SavePosition + HowMuchToSave) + \");\"); " + "\r\n");
            result.Append("					iLoop += 1; " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				iLoop = 1; " + "\r\n");
            result.Append("				for(SavePosition = 0; SavePosition < SaveSearchLength; SavePosition += 65000) { " + "\r\n");
            result.Append("					if ((SaveSearchLength - SavePosition) < 65000) { " + "\r\n");
            result.Append("						HowMuchToSave = (SaveSearchLength - SavePosition); " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					else { " + "\r\n");
            result.Append("						HowMuchToSave = 65000; " + "\r\n");
            result.Append("					} " + "\r\n");
            result.Append("					eval(\"document.forms[0].searchtext\" + iLoop + \".value = saveSearchObj.value.substring(\" + SavePosition + \",\" + (SavePosition + HowMuchToSave) + \");\"); " + "\r\n");
            result.Append("					iLoop += 1; " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				document.forms[0].hiddencontentsize.value = SaveContentLength; " + "\r\n");
            result.Append("				document.forms[0].hiddensearchsize.value = SaveSearchLength; " + "\r\n");
            if (((Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("MSIE") + 1 == 0) && ((Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("4.7") + 1 > 0))
            {
                result.Append("					document.forms[0].netscape.value = \"\"; " + "\r\n");
            }

            result.Append("				if (Button != \"save\") { " + "\r\n");
            result.Append("					ResizeFrame(1); // Show the navigation-tree frame. " + "\r\n");
            result.Append("				} " + "\r\n");
            result.Append("				document.forms[0].editaction.value = Button; " + "\r\n");
            result.Append("if (objInstance) {" + "\r\n");
            result.Append("objInstance.editor.Clear();" + "\r\n");
            result.Append("}" + "\r\n");
            if (Utilities.IsAssetType(lContentType))
            {
                //posting done in ektexplorer, so just redirect to back_url
                if (!String.IsNullOrEmpty(Request.QueryString["FromEE"])) //If the page is opened from Ektron Explorer we need to close the page instead of returning
                {
                    result.Append("var loc = new String(location); ");
                    result.Append("var index = loc.lastIndexOf(\'?\'); ");
                    result.Append("loc = loc.substring(0, index); ");
                    result.Append("index = loc.lastIndexOf(\'/\'); ");
                    result.Append("loc = loc.substring(0, index);");
                    result.Append("loc = loc + \'/close.aspx\'; ");
                    result.Append("location = loc;");
                }
                else
                {
                    result.Append("if (Button != \"save\") {");
                    result.Append("		ResizeFrame(1); // Show the navigation-tree frame." + "\r\n");
                    if (m_SelectedEditControl != "ContentDesigner")
                    {
                        result.Append("		ShutdownImageEditor();" + "\r\n");
                    }
                    if (m_bClose)
                    {
                        result.Append("     var loc = \'close.aspx?reload=true\';" + "\r\n");
                    }
                    else
                    {
                        result.Append("     var loc = \'" + GetBackPage(m_intItemId) + "\';" + "\r\n");
                    }
                    result.Append("     if (Button == \"publish\") { loc = loc.replace(\"action=viewstaged\", \"action=view\"); } " + "\r\n");
                    result.Append("		location.replace(loc);" + "\r\n");
                    result.Append("} else { var contentid = document.forms[0].content_id.value; location.replace(\'edit.aspx?close=" + Request.QueryString["close"] + "&LangType=" + Request.QueryString["LangType"] + "&id=\'+contentid+\'" + "&type=update&mycollection=" + strMyCollection + "&addto" + strAddToCollectionType + "&back_file=" + back_file + "&back_action=" + back_action + "&back_folder_id=" + back_folder_id + "&back_id=" + back_id + "&back_form_id=" + back_form_id + "&back_LangType=" + back_LangType + back_callerpage + back_origurl + "\');}");
                }
            }
            else
            {
                result.Append("		 		ektronFormSubmit(); " + "\r\n");
            }
            result.Append("				return (false); " + "\r\n");
            result.Append("			} " + "\r\n");
            result.Append("			buttonPressed = false; " + "\r\n");
            result.Append("         $ektron(\'#pleaseWait\').modalHide(); " + "\r\n");
            result.Append("			return (false); " + "\r\n");
            result.Append("		} " + "\r\n");
        }
        result.Append("		function CheckAllRequiredFields() {		 " + "\r\n");
        result.Append("        var metafieldtype; " + "\r\n");
        result.Append("     if(typeof document.forms[0].frm_validcounter != \"undefined\"){" + "\r\n");
        result.Append("			var EndLoop = document.forms[0].frm_validcounter.value; " + "\r\n");
        result.Append("     }else{" + "\r\n");
        result.Append("     var EndLoop =0;}" + "\r\n");
        result.Append("			for (LoopCounter = 1; LoopCounter <= EndLoop; LoopCounter++) { " + "\r\n");
        result.Append("				var field = \"document.forms[0].frm_text_\" + LoopCounter + \".value\"; " + "\r\n");
        result.Append("				var field1 = \"document.forms[0].frm_meta_required_\" + LoopCounter + \".value\"; " + "\r\n");
        result.Append("				eval(field + \" = Trim(\" + field + \")\"); " + "\r\n");
        result.Append("				var meta_text = eval(field); " + "\r\n");
        result.Append("				if (meta_text.length > 2000) { " + "\r\n");
        result.Append("					alert(\"" + m_refMsg.GetMessage("js: alert meta data over limit") + "\" + \" \" + (meta_text.length - 2000)); " + "\r\n");
        result.Append("					field = \"document.forms[0].frm_text_\" + LoopCounter + \".type\"; " + "\r\n");
        result.Append("					metafieldtype = eval(field); " + "\r\n");
        result.Append("					if (metafieldtype != \"hidden\") { " + "\r\n");
        result.Append("						field = \"document.forms[0].frm_text_\" + LoopCounter + \".focus()\"; " + "\r\n");
        result.Append("						eval(field); " + "\r\n");
        result.Append("					}					 " + "\r\n");
        result.Append("					return (false); " + "\r\n");
        result.Append("				} " + "\r\n");
        result.Append("				if ((meta_text == \"\") && (eval(field1) != 0)) {		 " + "\r\n");
        result.Append("					SetMetaComplete( 0, " + m_intItemId + ");								 " + "\r\n");
        result.Append("					return (false); " + "\r\n");
        result.Append("				} " + "\r\n");
        result.Append("			}		 " + "\r\n");
        result.Append("			SetMetaComplete( 1, " + m_intItemId + ");	 " + "\r\n");
        result.Append("			return true; " + "\r\n");
        result.Append("		} " + "\r\n");
        result.Append("	function SetMetaComplete(Flag, ID) { " + "\r\n");
        result.Append("				ecmMetaComplete = Flag; " + "\r\n");
        if (m_strType == "update")
        {
            result.Append("		//this is for netscape popups " + "\r\n");
            result.Append("		if (ID != " + m_intItemId + ") { " + "\r\n");
            result.Append("			ecmMetaComplete = 0; " + "\r\n");
            result.Append("			return; " + "\r\n");
            result.Append("		} " + "\r\n");
        }
        else
        {
            result.Append("	return; " + "\r\n");
        }
        result.Append("}" + "\r\n");
        return (result.ToString());
    }
    private string EditProJS()
    {
        ToggleViewJS();
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<script language=\"JavaScript1.2\"> " + "\r\n");
        result.Append("<!-- " + "\r\n");
        if (m_SelectedEditControl != "ContentDesigner")
        {
            result.Append("eWebEditPro.parameters.reset();" + "\r\n");
            result.Append("eWebEditPro.parameters.baseURL = \"" + SitePath + "\";" + "\r\n");

            if (lContentType != 2 && (save_xslt_file.Length > 0 || bVer4Editor || editorPackage.Length > 0))
            {
                result.Append("// If we have a SAVE XSLT then we need to tell the editor to dump document, which causes the Save XSLT to run " + "\r\n");
                if (bVer4Editor || editorPackage.Length > 0)
                {
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?mode=dataentry&LangType=" + m_intContentLanguage + "\"; " + "\r\n");
                }
                else //use the old method
                {
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx\"; " + "\r\n");
                    result.Append("eWebEditPro.parameters.xmlInfo = \"" + xml_config + "\"; " + "\r\n");
                }
                result.Append("eWebEditPro.parameters.editorGetMethod = \"getDocument\"; " + "\r\n");
            }
            else if (bIsFormDesign)
            {
                if (m_strContentHtml.IndexOf("class=\"redvalidation\"") + 1 > 0 || m_strContentHtml.IndexOf(" ekv=") + 1 > 0)
                {
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?FormToolbarVisible=true\"; " + "\r\n");
                }
                else
                {
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx?mode=formdesign&LangType=" + m_intContentLanguage + "\"; " + "\r\n");
                }
            }
            else
            {
                result.Append("eWebEditPro.parameters.config = eWebEditProPath + \"cms_config.aspx\"; " + "\r\n");
                result.Append("eWebEditPro.parameters.xmlInfo = \"" + xml_config + "\"; " + "\r\n");
            }
            result.Append("eWebEditPro.parameters.maxContentSize = " + iMaxContLength + "; " + "\r\n");
            string strPath = "";
            if (content_stylesheet.Length > 0)
            {
                strPath = (string)(GetServerPath() + SitePath + content_stylesheet);

                result.Append("eWebEditPro.parameters.styleSheet = \"" + strPath + "\"; " + "\r\n");
            }

            result.Append("function loadSegments() { " + "\r\n");
            result.Append("var strContent; " + "\r\n");
            result.Append("if (eWebEditPro.instances[\"content_html\"]){" + "\r\n");
            string JsStr;
            JsStr = "";
            if (bVer4Editor)
            {
               //Do nothing
            }
            else
            {
                if (!(xmlconfig_data == null))
                {
                    if (xmlconfig_data.EditXslt.Length == 0)
                    {
                        if (m_strContentHtml.Length == 0)
                        {
                            JsStr = "var ObjXml = eWebEditPro.instances[\"content_html\"].editor.XMLProcessor();";
                            JsStr = JsStr + "strContent = ObjXml.DocumentTemplate();";
                            JsStr = JsStr + "eWebEditPro.instances[\"content_html\"].load(strContent);";
                        }
                    }
                }
            }
            result.Append(JsStr + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");

            result.Append("function DisableUpload(sEditorName) " + "\r\n");
            result.Append("{ " + "\r\n");
            result.Append("var objMedia = eWebEditPro.instances[sEditorName].editor.MediaFile(); " + "\r\n");
            result.Append("if(objMedia != null) " + "\r\n");
            result.Append("{ " + "\r\n");
            result.Append("var objAutoUpload = objMedia.AutomaticUpload(); " + "\r\n");
            result.Append("if(objAutoUpload != null) " + "\r\n");
            result.Append("{ " + "\r\n");
            result.Append("objAutoUpload.setProperty(\"TransferMethod\", \"none\"); " + "\r\n");
            result.Append("var objMenu = eWebEditPro.instances[sEditorName].editor.Toolbars(); " + "\r\n");
            result.Append("if(objMenu != null) " + "\r\n");
            result.Append("{ " + "\r\n");
            result.Append("var objCommand = objMenu.CommandItem(\"cmdmfuuploadall\"); " + "\r\n");
            result.Append("if(objCommand != null) " + "\r\n");
            result.Append("{ " + "\r\n");
            result.Append("objCommand.setProperty(\"CmdGray\", true); " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");
            result.Append("} " + "\r\n");
        }
        result.Append("function textCounter(field,cntfield,maxlimit) { " + "\r\n");
        result.Append("if (field.value.length > maxlimit) { // if too long...trim it! " + "\r\n");
        result.Append("field.value = field.value.substring(0, maxlimit); " + "\r\n");
        result.Append("// otherwise, update \'characters left\' counter " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("else " + "\r\n");
        result.Append("{ " + "\r\n");
        result.Append("cntfield.value = maxlimit - field.value.length; " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("} " + "\r\n");

        result.Append("function SetDefault(textfield) { " + "\r\n");
        result.Append("var resetfield = \"document.forms[0].\" + textfield + \".value\"; " + "\r\n");
        result.Append("var defaultfield = \"document.forms[0].\" + textfield + \"default.value\"; " + "\r\n");
        result.Append("var strTmp = eval(defaultfield); " + "\r\n");
        result.Append("if (confirm(\"" + m_refMsg.GetMessage("js: confirm restore default text") + "\")) " + "\r\n");
        result.Append("{" + "\r\n");
        result.Append("document.forms.frmMain[textfield].value = strTmp; " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("function outputSelected(selfield,textfield,seperator) { " + "\r\n");
        result.Append("var retValue; " + "\r\n");
        result.Append("var sel = getSelected(selfield); " + "\r\n");
        result.Append("var strSel = \"\"; " + "\r\n");
        result.Append("for (var item in sel) {        " + "\r\n");
        result.Append("strSel += sel[item].value + seperator + \";\"" + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("strSel = strSel.substring(0, strSel.length-2); " + "\r\n");
        result.Append("var ch = strSel.substring(0, 1); " + "\r\n");
        result.Append("if (ch == seperator) { " + "\r\n");
        result.Append("strSel = strSel.substring(1, strSel.length); " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("document.forms.frmMain[textfield].value = strSel; " + "\r\n");
        result.Append("} " + "\r\n");

        result.Append("function getSelected(opt) { " + "\r\n");
        result.Append("var selected = new Array(); " + "\r\n");
        result.Append("var index = 0; " + "\r\n");
        result.Append("for (var intLoop = 0; intLoop < opt.length; intLoop++) { " + "\r\n");
        result.Append("if ((opt[intLoop].selected) || " + "\r\n");
        result.Append("(opt[intLoop].checked)) { " + "\r\n");
        result.Append("index = selected.length; " + "\r\n");
        result.Append("selected[index] = new Object; " + "\r\n");
        result.Append("selected[index].value = opt[intLoop].value; " + "\r\n");
        result.Append("selected[index].index = intLoop; " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("return selected; " + "\r\n");
        result.Append("} " + "\r\n");
        result.Append("//--> " + "\r\n");
        result.Append("</script> " + "\r\n");
        return (result.ToString());
    }
    private Hashtable GetEndDateActionStrings()
    {
        Hashtable result = new Hashtable();
        string strMsg = m_refMsg.GetMessage("Archive expire descrp");
        result.Add("SelectionSize", 3);
        if (strMsg == "")
        {
            strMsg = "Archive and remove from site (expire)";
        }
        result.Add("1", strMsg);
        result.Add("2", m_refMsg.GetMessage("Archive display descrp"));
        result.Add("3", m_refMsg.GetMessage("Refresh descrp"));
        return (result);
    }
    private void ToggleViewJS()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();
        sJS.Append("<script language=\"Javascript\">");
        sJS.Append("function ToggleView() {" + "\r\n");
        sJS.Append("SetFullScreenView(!m_fullScreenView)" + "\r\n");
        sJS.Append("}" + "\r\n");
        sJS.Append("function SetFullScreenView(bViewFullScreen) {" + "\r\n");
        sJS.Append("	// simply return if already in proper mode:" + "\r\n");
        sJS.Append("	if (m_fullScreenView == bViewFullScreen) {	" + "\r\n");
        sJS.Append("		return;" + "\r\n");
        sJS.Append("	}" + "\r\n");

        sJS.Append("	var tabArray = new Array(\"_dvContent\", \"_dvSummary\", \"_dvAlias\", \"_dvMetadata\", \"_dvSchedule\", \"_dvComment\",\"_dvSubscription\",\"_dvTemplates\",\"_dvTaxonomy\");" + "\r\n");

        sJS.Append("	// handle add-new and update-existing conditions (added controls change offsets):" + "\r\n");
        sJS.Append("	if (!m_initializedOffsets) {" + "\r\n");
        sJS.Append("		var contentDivObj = document.getElementById(\"dvContent\");" + "\r\n");
        sJS.Append("		if (null != contentDivObj) {" + "\r\n");
        sJS.Append("			if (m_stdVertOffset != contentDivObj.offsetTop) {" + "\r\n");
        sJS.Append("				m_mainTblOffset += 3 + (contentDivObj.offsetTop - m_stdVertOffset);" + "\r\n");
        sJS.Append("				m_stdVertOffset = contentDivObj.offsetTop;" + "\r\n");
        sJS.Append("			}" + "\r\n");
        sJS.Append("		}" + "\r\n");
        sJS.Append("		m_initializedOffsets = true;" + "\r\n");
        sJS.Append("	}" + "\r\n");
        sJS.Append("	if (bViewFullScreen) {" + "\r\n");
        sJS.Append("		document.getElementById(\"ToggleViewBtn\").src=\"" + AppImgPath + "movedown.gif\";" + "\r\n");
        sJS.Append("		document.getElementById(\"ToggleViewBtn\").title=\"Goto Normal View\";" + "\r\n");
        sJS.Append("		SetObjVisible(\"upperTable\", false);" + "\r\n");
        sJS.Append("		for (var i=0; i < tabArray.length; i++) {" + "\r\n");
        sJS.Append("			SetObjAltOffset(tabArray[i], true);" + "\r\n");
        sJS.Append("		}" + "\r\n");
        sJS.Append("		m_fullScreenView = true;" + "\r\n");
        sJS.Append("	} else {" + "\r\n");
        sJS.Append("		document.getElementById(\"ToggleViewBtn\").src=\"" + AppImgPath + "moveup.gif\";" + "\r\n");
        sJS.Append("		document.getElementById(\"ToggleViewBtn\").title=\"Goto Full-Screen View\";" + "\r\n");
        sJS.Append("		SetObjVisible(\"upperTable\", true);" + "\r\n");
        sJS.Append("		for (var i=0; i < tabArray.length; i++) {" + "\r\n");
        sJS.Append("			SetObjAltOffset(tabArray[i], false);" + "\r\n");
        sJS.Append("		}" + "\r\n");
        sJS.Append("		m_fullScreenView = false;" + "\r\n");
        sJS.Append("	}" + "\r\n");
        sJS.Append("	return false;" + "\r\n");
        sJS.Append("}" + "\r\n");
        sJS.Append("</script>" + "\r\n");
        Page.ClientScript.RegisterStartupScript(typeof(Page), "ToggleView", sJS.ToString());
    }

    private string GetBlogControls()
    {
        StringBuilder sbBlogControls = new StringBuilder();
        if (m_bIsBlog)
        {
            sbBlogControls.Append("<br/><input type=\"hidden\" name=\"postupdate_notify\" id=\"postupdate_notify\" value=\"" + EkFunctions.HtmlEncode(blog_data.NotifyURL) + "\"/><input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"" + blog_post_data.TrackBackURLID.ToString() + "\" /><input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/><br/><b>" + m_refMsg.GetMessage("lbl trackback url") + "</b><br/>");
            sbBlogControls.Append("<input type=\"text\" name=\"trackback\" id=\"trackback\" size=\"75\" value=\"");
            if (!(blog_post_data == null))
            {
                sbBlogControls.Append(EkFunctions.HtmlEncode(blog_post_data.TrackBackURL));
            }
            sbBlogControls.Append("\" />");
            sbBlogControls.Append("<br/><br/>");
            sbBlogControls.Append("<input type=\"checkbox\" id=\"chkPingBack\" name=\"chkPingBack\" ");
            if (!(blog_post_data == null) && blog_post_data.Pingback == true)
            {
                sbBlogControls.Append(" checked ");
            }
            sbBlogControls.Append(" />");
            sbBlogControls.Append("&nbsp;" + m_refMsg.GetMessage("lbl blog ae ping") + "<input type=\"hidden\" name=\"blogpostchkPingBackid\" id=\"blogpostchkPingBackid\" value=\"" + blog_post_data.PingBackID.ToString() + "\" />");
        }
        return sbBlogControls.ToString();
    }

    #endregion

    #region PROCESS EDITOR PAGE

    public void ProcessTags(long Id, int langId)
    {
        Ektron.Cms.Common.EkEnumeration.CMSObjectTypes tagtype = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content;
        TagData[] defaultTags;
        TagData[] Tags;
        Ektron.Cms.API.Community.Tags m_refTagsApi = new Ektron.Cms.API.Community.Tags();
        string orginalTagIds;
        string tagIdStr = "";
        string cTags = Page.Request.Form["currentTags"];
        if (cTags != null)
        {
            orginalTagIds = (string)(cTags.Trim().ToLower());
        }
        else
        {
            orginalTagIds = "";
        }
        //Assign all default user tags that are checked:
        //Remove tags that have been unchecked
        defaultTags = m_refTagsApi.GetDefaultTags(tagtype, -1);
        Tags = m_refTagsApi.GetTagsForObject(Id, tagtype);

        //Also, copy all users tags into defaultTags list
        //so that if they were removed, they can be deleted as well.
        int originalLength = defaultTags.Length;
        Array.Resize(ref defaultTags, defaultTags.Length + Tags.Length - 1 + 1);
        Tags.CopyTo(defaultTags, originalLength);

        if (defaultTags != null)
        {

            foreach (TagData td in defaultTags)
            {
                tagIdStr = (string)("userPTagsCbx_" + td.Id.ToString());
                if (!(Page.Request.Form[tagIdStr] == null))
                {
                    if (Page.Request.Form[tagIdStr] == "on")
                    {
                        //if tag is checked, but not in current tag list, add it
                        if (!orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.AddTagToObject(td.Id, Id, tagtype, -1, langId);
                        }
                    }
                    else
                    {
                        //if tag is unchecked AND in current list, delete
                        if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0);
                        }
                    }
                }
                else
                {
                    //If tag checkbox has no postback value AND is in current tag list, delete it
                    if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                    {
                        m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0);
                    }
                }
            }

            // Now add any new custom tags, that the user created:
            // New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>

           if (Page.Request["newTagNameHdn"] != null)
            {
                string custTags = (string)(Page.Request["newTagNameHdn"].ToString());
                char[] tagsep = new char[] { ';' };
                //string[] aCustTags = custTags.Split(tagsep.ToString().ToCharArray());
                string[] aCustTags = custTags.Split(tagsep);

                int languageId = 0;
                char[] langsep = { '~' };

                foreach (string tag in aCustTags)
                {
                    string[] tagPropArray = tag.Split(langsep);
                    if (tagPropArray.Length > 1)
                    {
                        if (tagPropArray[0].Trim().Length > 0)
                        {
                            //Default language to -1.
                            //"ALL" option in drop down is 0 - switch to -1.
                            if (!Int32.TryParse(tagPropArray[1], out languageId))
                            {
                                languageId = -1;
                            }
                            if (languageId == 0)
                            {
                                languageId = -1;
                            }

                            m_refTagsApi.AddTagToObject(tagPropArray[0], Id, tagtype, -1, languageId);
                        }
                    }
                }
            }
        }
    }


    private void Process_FormSubmit()
    {
        object dontCreateTask;
        int i = 0;
        int y = 0;
        int isub = 0;
        int ValidCounter = 0;
        string go_live = "";
        string end_date = "";
        string end_date_action = "";
        string strContent = "";
        string strSearchText = "";
        bool ret = false;
        string strTaskName = "";
        bool isAlreadyCreated = false;
        SettingsData site_data;
        Collection page_subscription_data = new Collection();
        Collection page_sub_temp = new Collection();
        Array arrSubscriptions;
        SubscriptionPropertiesData sub_prop_data = new SubscriptionPropertiesData();
        string strContentTeaser = "";
        string strRptDisplay = "";
        string strRpt = "";
        bool bUpdateFormQuestions = false;
        bool bIsReportForm = false;
        bool bLockedContentLink = false;
        string strContentTitle = Request.Form["content_title"];
        string strTextFromDesigner = string.Empty;
        EkEnumeration.CMSContentSubtype subtype = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes;

        try
        {

            dontCreateTask = Request.Form["createtask"];
            m_strType = Request.Form["eType"];

            if ("ContentDesigner" == m_SelectedEditControl)
            {
                string strResponse = Request.Form["response"];
                switch (strResponse)
                {
                    case "message":
                        string strFormDesign;
                        string strXsltDesign;
                        string strFieldList;
                        string strViewXslt;
                        strFormDesign = (string)m_ctlContentDesigner.Content;
                        strXsltDesign = (string)m_ctlSummaryDesigner.Content;
                        strFieldList = m_refContApi.TransformXsltPackage(strFormDesign, Server.MapPath((string)(m_ctlSummaryDesigner.ScriptLocation + "DesignToFieldList.xslt")), true);

                        System.Xml.Xsl.XsltArgumentList objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
                        objXsltArgs.AddParam("srcPath", "", m_ctlSummaryDesigner.ScriptLocation);

                        strViewXslt = m_refContApi.XSLTransform("<root>" + strXsltDesign + "<ektdesignpackage_list>" + strFieldList + "</ektdesignpackage_list></root>", Server.MapPath((string)(m_ctlSummaryDesigner.ScriptLocation + "DesignToViewXSLT.xslt")), true, false, objXsltArgs, false);
                        StringBuilder sbFormResponse = new StringBuilder();
                        sbFormResponse.Append("<ektdesignpackage_forms><ektdesignpackage_form><ektdesignpackage_designs><ektdesignpackage_design>");
                        sbFormResponse.Append(strXsltDesign);
                        sbFormResponse.Append("</ektdesignpackage_design></ektdesignpackage_designs><ektdesignpackage_lists><ektdesignpackage_list>");
                        sbFormResponse.Append(strFieldList);
                        sbFormResponse.Append("</ektdesignpackage_list></ektdesignpackage_lists><ektdesignpackage_views><ektdesignpackage_view></ektdesignpackage_view><ektdesignpackage_view>");
                        sbFormResponse.Append(strViewXslt);
                        sbFormResponse.Append("</ektdesignpackage_view></ektdesignpackage_views></ektdesignpackage_form></ektdesignpackage_forms>");
                        strContentTeaser = sbFormResponse.ToString();
                        break;
                    case "redirect":
                        strContentTeaser = (string)m_ctlFormResponseRedirect.Content;
                        break;
                    case "transfer":
                        strContentTeaser = (string)m_ctlFormResponseTransfer.Content;
                        break;
                    default:
                        strContentTeaser = (string)m_ctlSummaryDesigner.Content;
                        break;
                }
            }
            else
            {
                strContentTeaser = Request.Form["content_teaser"];
            }

            strRptDisplay = Request.Form["report_display_type"]; // Same Window = 1 and New Window = 0
            if (!string.IsNullOrEmpty(strRptDisplay) && strRptDisplay.Substring(0, 1) == ",")
            {
                strRptDisplay = strRptDisplay.Substring(strRptDisplay.Length - (strRptDisplay.Length - 1));
            }
            strRpt = Request.Form["report_type"]; // Data Table = 1, Bar Chart = 2, Pie Chart = 3 and Combined = 4
            if (!string.IsNullOrEmpty(strRpt) && strRpt.Substring(0, 1) == ",")
            {
                strRpt = strRpt.Substring(strRpt.Length - (strRpt.Length - 1));
            }
            if (strRptDisplay == "1")
            {
                strRptDisplay = "_self";
            }
            else
            {
                strRptDisplay = "_blank";
            }

            if (Request.Form["response"] == "report")
            {
                bIsReportForm = true;
                strContentTeaser = "<root><EktReportFormData/><RedirectionLink>";
                strContentTeaser = strContentTeaser + "<a href=\"poll.aspx\" id=\"" + strRpt + "\"";
                strContentTeaser = strContentTeaser + " target = \"" + strRptDisplay + "\"";
                strContentTeaser = strContentTeaser + "></a>";
                strContentTeaser = strContentTeaser + "</RedirectionLink></root>";
            }

            object[] acMetaInfo = new object[4];
            object MetaSelect;
            object MetaSeparator;
            string MetaTextString = "";
            if (!string.IsNullOrEmpty(Request.Form["frm_validcounter"]))
            {
                ValidCounter = System.Convert.ToInt32(Request.Form["frm_validcounter"]);
            }
            else
            {
                ValidCounter = 0;
            }

            page_meta_data = new Collection();
            for (i = 1; i <= ValidCounter; i++)
            {
                acMetaInfo[1] = Request.Form["frm_meta_type_id_" + i];
                acMetaInfo[2] = Request.Form["content_id"];
                MetaSeparator = Request.Form["MetaSeparator_" + i];
                MetaSelect = Request.Form["MetaSelect_" + i];
                if (String.IsNullOrEmpty(MetaSelect.ToString()))
                {
                    MetaTextString = Strings.Replace(Request.Form["frm_text_" + i], ", ", MetaSeparator.ToString(), 1, -1, 0);
                    if (MetaTextString.ToString().Substring(0, 1) == MetaSeparator.ToString())
                    {
                        MetaTextString = MetaTextString.Substring(MetaTextString.Length - (MetaTextString.Length - 1), (MetaTextString.Length - 1));
                    }

                    acMetaInfo[3] = MetaTextString;
                }
                else
                {
                    myMeta = Request.Form["frm_text_" + i];
                    myMeta = Server.HtmlDecode(myMeta);
                    MetaTextString = myMeta.Replace(";", MetaSeparator.ToString());
                    myMeta = EkFunctions.HtmlEncode(MetaTextString);
                    acMetaInfo[3] = MetaTextString;
                }
                page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new object[4];
            }

            if (!string.IsNullOrEmpty(Request.Form["isblogpost"])) //isblogpost
            {
                i++;
                acMetaInfo[1] = Request.Form["blogposttagsid"];
                acMetaInfo[2] = Request.Form["content_id"];
                MetaSeparator = ";";
                acMetaInfo[3] = Request.Form["blogposttags"];
                page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new object[4];

                i++;
                acMetaInfo[1] = Request.Form["blogpostcatid"];
                acMetaInfo[2] = Request.Form["content_id"];
                MetaSeparator = ";";
                if (Convert.ToInt32(Request.Form["blogpostcatlen"]) > 0)
                {
                    MetaTextString = "";
                    for (y = 0; y <= Convert.ToInt32(Request.Form["blogpostcatlen"]); y++)
                    {
                        if (!String.IsNullOrEmpty(Request.Form["blogcategories" + y.ToString()]))
                        {
                            MetaTextString += (string)(Strings.Replace(Request.Form["blogcategories" + y.ToString()], ";", "~@~@~", 1, -1, 0) + ";");
                        }
                    }
                    if (MetaTextString.ToString().EndsWith(";"))
                    {
                        MetaTextString = MetaTextString.Substring(0, (MetaTextString.Length - 1));
                    }
                    acMetaInfo[3] = MetaTextString;
                }
                else
                {
                    acMetaInfo[3] = "";
                }
                page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new object[4];

                i++;
                acMetaInfo[1] = Request.Form["blogposttrackbackid"];
                acMetaInfo[2] = Request.Form["content_id"];
                MetaSeparator = ";";
                acMetaInfo[3] = Request.Form["trackback"];
                page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new object[4];

                i++;
                acMetaInfo[1] = Request.Form["blogpostchkpingbackid"];
                acMetaInfo[2] = Request.Form["content_id"];
                MetaSeparator = ";";
                if (!String.IsNullOrEmpty(Request.Form["chkpingback"]))
                {
                    acMetaInfo[3] = 1;
                }
                else
                {
                    acMetaInfo[3] = 0;
                }
                page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
                acMetaInfo = new object[4];
            }

            sub_prop_data.BreakInheritance = true;
            if (!String.IsNullOrEmpty(Request.Form["send_notification_button"]))
            {
                sub_prop_data.SendNextNotification = true;
                sub_prop_data.SuspendNextNotification = false;
            }
            else
            {
                sub_prop_data.SendNextNotification = false;
            }
            if (Request.Form["notify_option"] == ("Always"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always;
            }
            else if (Request.Form["notify_option"] == ("Initial"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial;
                if (!(m_strType == "update")) // if new, then set flag to email out
                {
                    sub_prop_data.SendNextNotification = true;
                    sub_prop_data.SuspendNextNotification = false;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Request.Form["send_notification_button"]))
                    {
                        sub_prop_data.SendNextNotification = true;
                        sub_prop_data.SuspendNextNotification = false;
                    }
                    else
                    {
                        sub_prop_data.SendNextNotification = false;
                    }
                }
            }
            else if (Request.Form["notify_option"] == ("Never"))
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never;
            }
            if (!String.IsNullOrEmpty(Request.Form["suspend_notification_button"]))
            {
                sub_prop_data.SuspendNextNotification = true;
                sub_prop_data.SendNextNotification = false;
            }
            else
            {
                sub_prop_data.SuspendNextNotification = false;
            }
            sub_prop_data.OptOutID = Convert.ToInt64(Request.Form["notify_optoutid"]);
            if (!String.IsNullOrEmpty(Request.Form["use_message_button"]))
            {
                sub_prop_data.DefaultMessageID = Convert.ToInt64(Request.Form["notify_messageid"]);
            }
            else
            {
                sub_prop_data.DefaultMessageID = 0;
            }
            if (!String.IsNullOrEmpty(Request.Form["use_summary_button"]))
            {
                sub_prop_data.SummaryID = 1;
            }
            else
            {
                sub_prop_data.SummaryID = 0;
            }
            if (!String.IsNullOrEmpty(Request.Form["use_content_button"]))
            {
                sub_prop_data.ContentID = Convert.ToInt64(Request.Form["frm_content_id"]);
            }
            else
            {
                sub_prop_data.ContentID = 0;
            }
            sub_prop_data.UnsubscribeID = Convert.ToInt64(Request.Form["notify_unsubscribeid"]);

            if (!String.IsNullOrEmpty(Request.Form["notify_url"]))
            {
                sub_prop_data.URL = Request.Form["notify_url"];
            }
            else
            {
                sub_prop_data.URL = Request.ServerVariables["HTTP_HOST"];
            }

            if (!String.IsNullOrEmpty(Request.Form["notify_weblocation"]))
            {
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath + "subscriptions");
            }
            else
            {
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath + "subscriptions");
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_weblocation"]))
            {
                sub_prop_data.WebLocation = Request.Form["notify_weblocation"];
            }
            else
            {
                sub_prop_data.WebLocation = "subscriptions";
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_subject"]))
            {
                sub_prop_data.Subject = Request.Form["notify_subject"];
            }
            else
            {
                sub_prop_data.Subject = "";
            }
            if (!String.IsNullOrEmpty(Request.Form["notify_emailfrom"]))
            {
                sub_prop_data.EmailFrom = Request.Form["notify_emailfrom"];
            }
            else
            {
                sub_prop_data.EmailFrom = "";
            }

            sub_prop_data.UseContentTitle = "";

            if (!String.IsNullOrEmpty(Request.Form["use_contentlink_button"]))
            {
                sub_prop_data.UseContentLink = 1;
            }
            else
            {
                sub_prop_data.UseContentLink = 0;
            }

            if (!String.IsNullOrEmpty(Request.Form["content_sub_assignments"]))
            {
                arrSubscriptions = Strings.Split(Strings.Trim(Request.Form["content_sub_assignments"]), " ", -1, 0);
                if (arrSubscriptions.Length > 0)
                {
                    for (isub = 0; isub <= (arrSubscriptions.Length - 1); isub++)
                    {
                        page_sub_temp = new Collection();
                        page_sub_temp.Add(Int64.Parse(Strings.Mid(arrSubscriptions.GetValue(isub).ToString(), 10)), "ID", null, null);
                        page_subscription_data.Add(page_sub_temp, null, null, null);
                    }
                }
            }
            else
            {
                page_subscription_data = null;
            }
            page_sub_temp = null;

            if (!String.IsNullOrEmpty(Request.Form["go_live"]))
            {
                go_live = DateTime.Parse(Strings.Trim(Request.Form["go_live"])).ToString();
            }
            if (!String.IsNullOrEmpty(Request.Form["end_date"]))
            {
                end_date = DateTime.Parse(Strings.Trim(Request.Form["end_date"])).ToString();
                end_date_action = Request.Form["end_date_action_radio"];
            }
            lContentType = Convert.ToInt32(Request.Form["content_type"]);
            switch (Request.Form["content_subtype"].ToLower())
            {
                case "content":
                    {
                         lContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content ;
                         break;
                    }
                case "pagebuilderdata":
                    {
                        lContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData  ;
                        break;
                    }
                case "webevent":
                    {
                        lContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent  ;
                        break;
                    }
                case "pagebuildermasterdata":
                    {
                        lContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData  ;
                        break;
                    }
                default:
                    {
                        lContentSubType = EkEnumeration.CMSContentSubtype.AllTypes;
                        break;
                    }

            }
            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                asset_info.Add(Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i], Strings.Trim(Request.Form["asset_" + Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i].ToString().ToLower()]));
            }

            page_content_data = new Collection();
            if (!String.IsNullOrEmpty(Request.Form["content_id"]))
            {
                page_content_data.Add(Request.Form["content_id"], "ContentID", null, null);
            }
            else
            {
                page_content_data.Add(0, "ContentID", null, null);
            }
            page_content_data.Add(Request.Form["content_language"], "ContentLanguage", null, null);

            if ((lContentType == 1 || lContentType == 3) && (lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent))
            {

                ContentData cb;
                if (m_strType == "add")
                {
                    if (!String.IsNullOrEmpty(Request.QueryString["back_LangType"]))
                    {
                        m_refContApi.ContentLanguage = Convert.ToInt32(Request.QueryString["back_LangType"]);
                    }
                    else
                    {
                        m_refContApi.ContentLanguage = m_refContApi.RequestInformationRef.DefaultContentLanguage;
                    }
                    cb = m_refContApi.GetContentById(Convert.ToInt64(Request.Form["content_id"]), Ektron.Cms.ContentAPI.ContentResultType.Published);
                    m_refContApi.ContentLanguage = m_intContentLanguage;
                }
                else
                {
                    cb = m_refContApi.GetContentById(Convert.ToInt64(Request.Form["content_id"]), Ektron.Cms.ContentAPI.ContentResultType.Staged);
                }
                if (cb != null)
                {
                    subtype = cb.SubType;
                    if (subtype == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || subtype == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData || subtype == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
                    {
                        strContent = cb.Html;
                        if (strContentTitle == null)
                        {
                            strContentTitle = cb.Title;
                        }
                        strSearchText = strContentTeaser;
                        page_content_data.Add(subtype, "ContentSubType", null, null);
                    }
                }
            }
            if ("ContentDesigner" == m_SelectedEditControl && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
            {
                strContent = (string)m_ctlContentDesigner.Content;
                if (string.IsNullOrEmpty(strContent) && !string.IsNullOrEmpty(Request.Form["xid"]))
                {
                    strContent = "<root></root>";	   //only for smart form content
                }
                strSearchText = (string)m_ctlContentDesigner.Text;
                strTextFromDesigner = strSearchText;
            }
            else
            {
                if (subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
                {
                    i = 1;
                    while (Strings.Len(Request.Form["hiddencontent" + i]) > 0)
                    {
                        strContent = strContent + Request.Form["hiddencontent" + i];
                        i++;
                    }
                    i = 1;
                    while (Strings.Len(Request.Form["searchtext" + i]) > 0)
                    {
                        strSearchText = strSearchText + Request.Form["searchtext" + i];
                        i = System.Convert.ToInt32(i + 1);
                    }
                }
            }

            page_content_data.Add(lContentType, "ContentType", null, null);
            if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
            {
                //Ephox outputs entity name &nbsp; which causes error in Import/Export utility.
                //If we finde more entity name being used we should use code snippet from the eWebEditPro to clean.
                strContent = strContent.Replace("&nbsp;", "&#160;");
            }
            if ((asset_info != null) && (lContentType == Ektron.Cms.Common.EkConstants.CMSContentType_Media))
            {
                strContent = Request.Form["content_html"];
                page_content_data.Add(strContent, "MediaText", null, null);
            }
            if (subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
            {
                strContent = Utilities.WikiQLink(strContent, Convert.ToInt64(Request.Form["content_folder"]));
            }

            if (subtype == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
            {
                //we need to update the inner title to match this title, so we deserialize the event, update the field, and reserialize
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Ektron.Cms.Content.Calendar.EventPersistence.root));
                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(strContent, System.Xml.XmlNodeType.Document, null);
                Ektron.Cms.Content.Calendar.EventPersistence.root ev;
                ev = (Ektron.Cms.Content.Calendar.EventPersistence.root)xs.Deserialize(reader);

                ev.DisplayTitle = strContentTitle;

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.Xml.XmlWriterSettings writersettings = new System.Xml.XmlWriterSettings();
                writersettings.OmitXmlDeclaration = true;

                using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stream, writersettings))
                {
                    xs.Serialize(xmlWriter, ev);
                }


                stream.Flush();
                stream.Position = 0;
                System.IO.StreamReader streamreader = new System.IO.StreamReader(stream);
                strContent = streamreader.ReadToEnd();
            }

            page_content_data.Add(strContent, "ContentHtml", null, null);
            if (m_strType != "update" || (strContentTeaser.ToLower() == "<br /><!-- wiki summary -->"))
            {
                string strippedTeaser = Utilities.StripHTML(strContentTeaser);
                if ((lContentType == 1 || lContentType == 3) && (subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData && subtype != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent))
                {
                    if (Request.Form["xid"] == null && (strContentTeaser.IndexOf("<img") == -1 && (strippedTeaser == "" || strippedTeaser == "&#160;" || strippedTeaser == "&nbsp;" || (strippedTeaser.ToLower() == "<!-- wiki summary -->"))))
                    {
                        if (strTextFromDesigner != string.Empty)
                        {
                            strContentTeaser = strTextFromDesigner;
                        }
                        else
                        {
                            strContentTeaser = Utilities.AutoSummary(strContent);
                        }
                        if (strContentTeaser != "")
                        {
                            strContentTeaser = "<p>" + strContentTeaser + "</p>";
                        }
                    }
                }
            }
            if ((Request.Form["chkLockedContentLink"] != null) && Request.Form["chkLockedContentLink"] == "on")
            {
                bLockedContentLink = true;
            }
            page_content_data.Add(bLockedContentLink, "LockedContentLink", null, null);
            page_content_data.Add(Request.Form["content_comment"], "Comment", null, null);
            page_content_data.Add(page_meta_data, "ContentMetadata", null, null);
            page_content_data.Add(strContentTeaser, "ContentTeaser", null, null);
            page_content_data.Add(Request.Form["content_folder"], "FolderID", null, null);

            page_content_data.Add(strSearchText, "SearchText", null, null);
            page_content_data.Add(go_live, "GoLive", null, null);
            page_content_data.Add(end_date, "EndDate", null, null);
            page_content_data.Add(end_date_action, "EndDateAction", null, null);
            int nAssetInfoArrayLBound = 0;
            int nAssetInfoArrayUBound = -1;
            int j = 1;
            string strAssetInfo = "";
            Array aryAssetInfoValue;
            int nArrayLBound;
            int nArrayUBound;
            Hashtable cAssetInfoArray = new Hashtable();
            string strKeyName = "";
            for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
            {
                strKeyName = Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i];
                strAssetInfo = Convert.ToString(asset_info[Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i]]);
                page_content_data.Add(strAssetInfo.Replace("%2C", ","), strKeyName, null, null);
                if (0 == strAssetInfo.Length && nAssetInfoArrayUBound > nAssetInfoArrayLBound)
                {
                    // This information is not provided at all, so it's not inconsistent.
                    strAssetInfo = "";
                    for (j = 1; j <= nAssetInfoArrayUBound - nAssetInfoArrayLBound; j++)
                    {
                        strAssetInfo += ",";
                    }

                }
                //' Append a space so that an empty string will produce an array of one item
                //' rather than an array with no items. The value will be Trimmed later to
                //' remove the space.
                aryAssetInfoValue = (strAssetInfo + " ").Split(',');

                nArrayLBound = 0;
                nArrayUBound = aryAssetInfoValue.Length - 1;
                if (i == 0)
                {
                    nAssetInfoArrayLBound = System.Convert.ToInt32(nArrayLBound);
                    nAssetInfoArrayUBound = System.Convert.ToInt32(nArrayUBound);
                }
                if (nAssetInfoArrayLBound == nArrayLBound && nAssetInfoArrayUBound == nArrayUBound)
                {
                    cAssetInfoArray.Add(strKeyName, aryAssetInfoValue);
                }
                else
                {
                    Response.Redirect((string)("reterror.asp?info=" + EkFunctions.UrlEncode((string)("Inconsistent number of assets. Value=" + strAssetInfo))), false);
                }
            }
            if (nAssetInfoArrayLBound == nAssetInfoArrayUBound)
            {
                if (0 == strContentTitle.Length)
                {
                    if (Strings.Len(page_content_data["AssetFilename"]) > 0)
                    {
                        strContentTitle = page_content_data["AssetFilename"].ToString();
                    }
                    else
                    {
                        strContentTitle = "No Title";
                    }
                }
            }
            if (strContentTitle.IndexOf("\'") != -1)
            {
                strContentTitle = System.Web.HttpUtility.HtmlEncode(strContentTitle);
            }
            page_content_data.Add(strContentTitle, "ContentTitle", null, null);
            m_strManualAlias = (Request.Form["frm_manalias"] != null ? Request.Form["frm_manalias"].ToString().Trim() : null);
            m_strManualAliasExt = (Request.Form["frm_manaliasExt"] != null ? Request.Form["frm_manaliasExt"].ToString() : null);

            ast_frm_manaliasExt.Value = Request.Form["frm_manaliasExt"];


            //Aliasing logic for 7.6 starts here
            m_prevManualAliasName = (Request.Form["prev_frm_manalias_name"] != null ? Request.Form["prev_frm_manalias_name"].ToString() : "");
            m_prevManualAliasExt = (Request.Form["prev_frm_manalias_ext"] != null ?Request.Form["prev_frm_manalias_ext"].ToString():"");
            m_currManualAliasName = m_strManualAlias;
            m_currManualAliasExt = m_strManualAliasExt;
            if (m_prevManualAliasName == "" && m_currManualAliasName != "" || m_prevManualAliasExt == "" && m_currManualAliasExt != "")
            {
                m_currManualAliasStatus = "New";
            }
            else if (m_prevManualAliasName != "" && m_currManualAliasName != "" && (m_currManualAliasName != m_prevManualAliasName || m_prevManualAliasExt != m_currManualAliasExt))
            {
                m_currManualAliasStatus = "Modified";
            }
            else if (m_prevManualAliasName != "" && m_currManualAliasName == "")
            {
                m_currManualAliasStatus = "Deleted";
            }
            else
            {
                m_currManualAliasStatus = "None";
            }
            if (!string.IsNullOrEmpty(Request.Form["frm_manalias_id"]))
            {
                m_intManualAliasId = System.Convert.ToInt64(Request.Form["frm_manalias_id"]);
            }

            page_content_data.Add(m_strManualAlias, "NewUrlAliasName", null, null);
            page_content_data.Add(m_intManualAliasId, "UrlAliasId", null, null);
            page_content_data.Add(m_strManualAliasExt, "NewUrlAliasExt", null, null);
            page_content_data.Add(m_currManualAliasStatus, "UrlAliasStatus", null, null);
            page_content_data.Add(m_prevManualAliasName, "OldUrlAliasName", null, null);
            page_content_data.Add(m_prevManualAliasExt, "OldUrlAliasExt", null, null);

            page_content_data.Add(m_strManualAlias, "ManualAlias", null, null);
            page_content_data.Add(m_intManualAliasId, "ManualAliasID", null, null);

            if (Request.Form["TaxonomyOverrideId"] != null && Convert.ToInt64(Request.Form["TaxonomyOverrideId"]) != 0)
            {
                TaxonomyOverrideId = Convert.ToInt64(Request.Form["TaxonomyOverrideId"]);
                TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
            }

            if (!string.IsNullOrEmpty(Request.Form[taxonomyselectedtree.UniqueID]))
            {
                TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                if (TaxonomyTreeIdList.Trim().EndsWith(","))
                {
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                }
            }
            if (TaxonomyTreeIdList.Trim() == string.Empty && TaxonomySelectId > 0)
            {
                TaxonomyTreeIdList = TaxonomySelectId.ToString();
            }
            page_content_data.Add(TaxonomyTreeIdList, "Taxonomy", null, null);
            page_content_data.Add(Request.Form["content_image"], "Image", null, null);
            long intContentId = 0;
            int iAsset = 0;
            string strAssetInfoValue = "";
            for (iAsset = nAssetInfoArrayLBound; iAsset <= nAssetInfoArrayUBound; iAsset++)
            {
                if (nAssetInfoArrayLBound < nAssetInfoArrayUBound)
                {
                    for (i = 0; i <= Ektron.Cms.Common.EkConstants.m_AssetInfoKeys.Length - 1; i++)
                    {
                        strKeyName = Ektron.Cms.Common.EkConstants.m_AssetInfoKeys[i];
                        strAssetInfoValue = (string)(cAssetInfoArray[strKeyName]); // [iAsset]
                        // Commas were escaped as %2C, so restore them now. See assetevents.js
                        strAssetInfoValue = strAssetInfoValue.Replace("%2C", ",").Trim();
                        if (Ektron.Cms.Common.EkFunctions.DoesKeyExist(page_content_data, strKeyName))
                        {
                            page_content_data.Remove(strKeyName);
                        }
                        page_content_data.Add(strAssetInfoValue, strKeyName, null, null);
                    }
                    page_content_data.Remove("ContentTitle");
                    if (strContentTitle.Length > 0)
                    {
                        page_content_data.Add(strContentTitle + " (" + page_content_data["AssetFilename"] + ")", "ContentTitle", null, null);
                    }
                    else if (Strings.Len(page_content_data["AssetFilename"]) > 0)
                    {
                        page_content_data.Add(page_content_data["AssetFilename"], "ContentTitle", null, null);
                    }
                    else
                    {
                        page_content_data.Add("No Title", "ContentTitle", null, null);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["xid"]) && !string.IsNullOrEmpty(Request.Form["templateSelect"]))
                {
                    page_content_data.Add(Request.Form["xid"], "MultiXmlID", null, null);
                    page_content_data.Add(Request.Form["templateSelect"], "MultiTemplateID", null, null);
                }
                else if (!string.IsNullOrEmpty(Request.Form["templateSelect"]))
                {
                    page_content_data.Add(0, "MultiXmlID", null, null);
                    if (lContentSubType == EkEnumeration.CMSContentSubtype.PageBuilderData || lContentSubType == EkEnumeration.CMSContentSubtype.PageBuilderMasterData )
                    {
                            ITemplateModel templmodel = ObjectFactory.GetTemplateModel();
                            TemplateData templdat = templmodel.FindByID(Convert.ToInt64(Request.Form["templateSelect"]));
                            if ((templdat != null))
                            {
                                page_content_data.Add(templdat.Id, "MultiTemplateID", null, null);
                            }  
                    }
                    else
                        page_content_data.Add(Request.Form["templateSelect"], "MultiTemplateID", null, null);
                }
                else if (!string.IsNullOrEmpty(Request.Form["xid"]) && lContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
                {
                    page_content_data.Add(Request.Form["xid"], "MultiXmlID", null, null);
                    page_content_data.Add(0, "MultiTemplateID", null, null);
                }

                if (m_strType == "update")
                {
                    m_refContent.SaveContentv2_0(page_content_data);
                    intContentId = Convert.ToInt64(Request.Form["content_id"]);
                }
                else
                {
                    if (iAsset == nAssetInfoArrayLBound)
                    {
                        if (Request.Form["AddQlink"] == "AddQlink")
                        {
                            page_content_data.Add(true, "AddToQlink", null, null);
                        }
                        else
                        {
                            page_content_data.Add(false, "AddToQlink", null, null);
                        }
                        if (Request.Form["IsSearchable"] == "IsSearchable" || Request.Form["IsSearchable"] =="on")
                        {
                            page_content_data.Add(true, "IsSearchable", null, null);
                        }
                        else
                        {
                            page_content_data.Add(false, "IsSearchable", null, null);
                        }
                    }

                    // Update content flagging:
                    object flagDefSelObj = Request.Form["FlaggingDefinitionSel"];
                    if ((flagDefSelObj != null) && Ektron.Cms.Common.EkFunctions.IsNumeric(flagDefSelObj))
                    {
                        page_content_data.Add(Convert.ToInt64(flagDefSelObj.ToString()), "FlagDefId", null, null);
                    }


                    intContentId = m_refContent.AddNewContentv2_0(page_content_data);
                }

                if (string.IsNullOrEmpty(Request.Form["suppress_notification"]))
                {
                    m_refContent.UpdateSubscriptionPropertiesForContent(intContentId, sub_prop_data);
                    m_refContent.UpdateSubscriptionsForContent(intContentId, page_subscription_data);
                }
               

                // process tag info
                ProcessTags(intContentId, m_intContentLanguage);

                if (m_strPageAction == "checkin")
                {
                    m_refContent.CheckIn(intContentId, "");
                }
                if (m_strPageAction == "publish")
                {
                    m_refContent.CheckIn(intContentId, "");
                    if (ret == false)
                    {
                        if (bIsReportForm)
                        {
                            if (!String.IsNullOrEmpty(Request.Form["renewpoll"]))
                            {
                                if ("true" == Request.Form["renewpoll"].ToLower())
                                {
                                    //this needs to be done before the histogram is updated with the new data.
                                    m_refContApi.EkModuleRef.UpdatePollRev(intContentId);
                                }
                            }
                            bUpdateFormQuestions = m_refContApi.EkModuleRef.UpdateFormFieldQuestions(System.Convert.ToInt64(Request.Form["content_id"]), strContent);
                        }

                        site_data = m_refSiteApi.GetSiteVariables(-1);

                        long PreapprovalGroupID;
                        Collection cPreApproval = new Collection();
                        cPreApproval = m_refContent.GetFolderPreapprovalGroup(Convert.ToInt64(Request.Form["content_folder"]));
                        PreapprovalGroupID = System.Convert.ToInt64(cPreApproval["UserGroupID"]);

                        if (PreapprovalGroupID > 0)
                        {
                            if (Convert.ToString(dontCreateTask) == "")
                            {
                                if (m_intContentLanguage == 1)
                                {
                                    strTaskName = (string)(Request.Form["content_title"] + intContentId + "_Task");
                                }
                                else
                                {
                                    strTaskName = (string)(Request.Form["content_title"] + intContentId + "_Task" + m_intContentLanguage);
                                }
                                m_refTask.ContentLanguage = m_intContentLanguage;
                                m_refTask.LanguageID = m_intContentLanguage;
                                isAlreadyCreated = m_refTask.IsTaskAlreadyCreated(intContentId);
                                if (isAlreadyCreated == false)
                                {
                                    m_refTask.TaskTitle = strTaskName; // Task name would be contentname + content id + _Task
                                    m_refTask.AssignToUserGroupID = PreapprovalGroupID; //Assigned to group defined by
                                    m_refTask.AssignedByUserID = Convert.ToString(CurrentUserID); //Assigned by person creating the task
                                    m_refTask.State = "1"; //Not started
                                    m_refTask.ContentID = intContentId; //Content ID of the content being created
                                    m_refTask.Priority = EkEnumeration.TaskPriority.Normal; //Normal
                                    m_refTask.CreatedByUserID = CurrentUserID; // If task is hopping this will always be created by
                                    m_refTask.ContentLanguage = m_intContentLanguage;
                                    m_refTask.LanguageID = m_intContentLanguage;
                                    ret = m_refTask.AddTask();
                                    ret = m_refContent.SetContentState(intContentId, "T");
                                }
                                else
                                {
                                    ret = m_refContent.SubmitForPublicationv2_0(intContentId, Convert.ToInt64(Request.Form["content_folder"]), "");
                                }

                            }
                            else
                            {
                                ret = m_refContent.SubmitForPublicationv2_0(intContentId, Convert.ToInt64(Request.Form["content_folder"]), "");
                            }
                        }
                        else
                        {
                            string strStatusBefore;
                            string strStatusAfter;
                            Collection colContentState;

                            colContentState = m_refContent.GetContentStatev2_0(intContentId);
                            strStatusBefore = Convert.ToString(colContentState["ContentStatus"]);
                            ret = m_refContent.SubmitForPublicationv2_0(intContentId, Convert.ToInt64(Request.Form["content_folder"]), "");
                            colContentState = m_refContent.GetContentStatev2_0(intContentId);
                            strStatusAfter = Convert.ToString(colContentState["ContentStatus"]);

                            if (strStatusBefore != strStatusAfter && "T" == strStatusAfter)
                            {
                                blnShowTStatusMessage = true;
                            }
                            string markupPath = "";
                            string cacheidentifier = "";
                            string updateContent = "";
                            markupPath = Request.Form["ctlmarkup"];
                            cacheidentifier = Request.Form["cacheidentifier"];
                            if ((markupPath != null) && markupPath.Length > 0)
                            {
                                markupPath = Request.PhysicalApplicationPath + markupPath;
                            }
                            if ((cacheidentifier != null) && cacheidentifier.Length > 0)
                            {
                                if (HttpContext.Current.Cache[cacheidentifier] != null)
                                {
                                    HttpContext.Current.Cache.Remove(cacheidentifier);
                                }
                            }
                            object ekml = null;
                            if ((markupPath != null) && (HttpContext.Current.Cache[markupPath] != null))
                            {
                                ekml = HttpContext.Current.Cache[markupPath];
                                Ektron.Cms.UI.CommonUI.ApiSupport api = new Ektron.Cms.UI.CommonUI.ApiSupport();
                                Ektron.Cms.UI.CommonUI.ApiSupport.ContentResult results = api.LoadContent(intContentId, false);
                                m_refContApi = new ContentAPI();
                                updateContent = this.m_refContApi.FormatOutput((string)ekml.GetType().GetProperty("ContentFormat").GetValue(ekml, null), Request.Form["ctltype"], results.Item);
                                updateContent = this.m_refContApi.WrapAjaxToolBar(updateContent, results.Item, commparams);
                            }
                            else
                            {
                                updateContent = Convert.ToString(colContentState["ContentHtml"]);
                            }
                            if (!String.IsNullOrEmpty(Request.Form["ctlupdateid"]))
                            {
                                Page.ClientScript.RegisterHiddenField("updatefieldcontent", updateContent);
                                StringBuilder strJs = new StringBuilder();
                                strJs.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\"> ").Append("\r\n");
                                strJs.Append(" if (top.opener != null) {").Append("\r\n");
                                strJs.Append("      var objUpdateField = top.opener.document.getElementById(\'" + Request.Form["ctlupdateid"] + "\');").Append("\r\n");
                                strJs.Append("      if (objUpdateField != null) { objUpdateField.innerHTML = document.getElementById(\"updatefieldcontent\").value; }").Append("\r\n");
                                strJs.Append(" }").Append("\r\n");
                                if ((m_bClose) && (m_strPageAction != "save"))
                                {
                                    strJs.Append("document.location.href = \"close.aspx\";").Append("\r\n");
                                }
                                strJs.Append("</script>").Append("\r\n");
                                UpdateFieldJS.Text = strJs.ToString();
                           }
                        }
                    }


                    if (strAddToCollectionType == "menu")
                    {
                        if (strMyCollection != "")
                        {
                            pagedata = new Collection();
                            pagedata.Add(intContentId, "ItemID", null, null);
                            pagedata.Add("content", "ItemType", null, null);
                            pagedata.Add("self", "ItemTarget", null, null);
                            pagedata.Add("", "ItemLink", null, null);
                            pagedata.Add("", "ItemTitle", null, null);
                            pagedata.Add("", "ItemDescription", null, null);
                            ret = m_refContent.AddItemToEcmMenu(strMyCollection, pagedata);
                        }
                    }
                    else
                    {
                        if (strMyCollection != "")
                        {
                            ret = m_refContent.AddItemToEcmCollection(Convert.ToInt64(strMyCollection), intContentId, m_intContentLanguage);
                        }
                    }
                }
            }
            if (m_strPageAction == "summary_save")
            {
                Response.Redirect((string)("edit.aspx?close=" + Request.QueryString["close"] + "&LangType=" + m_intContentLanguage + "&id=" + intContentId + "&type=update&mycollection=" + strMyCollection + "&addto" + strAddToCollectionType + "&back_file=" + back_file + "&back_action=" + back_action + "&back_folder_id=" + back_folder_id + "&back_id=" + back_id + "&back_form_id=" + back_form_id + "&back_LangType=" + back_LangType + "&summary=1" + back_callerpage + back_origurl), false);
            }
            else if (m_strPageAction == "meta_save")
            {
                Response.Redirect((string)("edit.aspx?close=" + Request.QueryString["close"] + "&LangType=" + m_intContentLanguage + "&id=" + intContentId + "&type=update&mycollection=" + strMyCollection + "&addto" + strAddToCollectionType + "&back_file=" + back_file + "&back_action=" + back_action + "&back_folder_id=" + back_folder_id + "&back_id=" + back_id + "&back_form_id=" + back_form_id + "&back_LangType=" + back_LangType + "&meta=1" + back_callerpage + back_origurl), false);
            }
            else if ((!m_bClose) && (m_strPageAction != "save"))
            {
                if (Request.QueryString["pullapproval"] == "true")
                {
                    Response.Redirect(GetBackPage(intContentId), false);
                }
                else
                {
                    if (m_strType == "add" && m_strPageAction == "checkin")
                    {
                        //leave back_action
                    }
                    else if (m_strPageAction != "publish")
                    {
                        if (back_action.ToLower() == "viewform")
                        {
                            back_action = back_action + "&staged=true";
                        }
                        else
                        {
                            back_action = "viewstaged";
                        }

                    }
                    // replaced logic added by todd 3/30/2006 - when you save then checkin content, GetBackPage() isn't aware of contentid and tries
                    // to use 0 which causes all sorts of bad things to happen - bug# 19413 - however if you just checkin before saving it goes to a different page,
                    // so don't replace that (which is the else statement)
                    if (m_strType == "update")
                    {
                        back_id = intContentId;
                        if (controlName == "cbwidget")
                        {
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UpdateContentWidget", "UpdateContentWidget(" + intContentId + ",\'" + buttonId.Value + "\');", true);
                        }
                        else
                        {
                            Response.Redirect(GetBackPage(intContentId), false);
                        }

                    }
                    else
                    {
                        Response.Redirect(GetBackPage(intContentId), false);
                    }

                }
            }
            else if (m_strPageAction == "save")
            {
                Response.Redirect((string)("edit.aspx?close=" + Request.QueryString["close"] + "&LangType=" + m_intContentLanguage + "&id=" + intContentId + (this.TaxonomyOverrideId > 0 ? ("&TaxonomyId=" + this.TaxonomyOverrideId.ToString()) : "") + (this.TaxonomySelectId > 0 ? ("&SelTaxonomyId=" + this.TaxonomySelectId.ToString()) : "") + "&type=update&mycollection=" + strMyCollection + "&addto" + strAddToCollectionType + "&back_file=" + back_file + "&back_action=" + back_action + "&back_folder_id=" + back_folder_id + "&back_id=" + back_id + "&back_form_id=" + back_form_id + "&back_LangType=" + back_LangType + back_callerpage + back_origurl + "&control=" + controlName+ "&buttonid=" + buttonId.Value), false);
            }
            if ((m_bClose) && (m_strPageAction != "save"))
            {
                //Close the editor page
                if (String.IsNullOrEmpty( Request.Form["ctlupdateid"]))
                {
                    string strQuery = "";
                    if (TaxonomySelectId > 0)
                    {
                        strQuery = (string)("&__taxonomyid=" + TaxonomySelectId);
                    }
                    else if (TaxonomyOverrideId > 0)
                    {
                        strQuery = (string)("&__taxonomyid=" + TaxonomyOverrideId);
                    }
                    if (controlName == "cbwidget")
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UpdateContentWidget", "UpdateContentWidget(" + intContentId + ",\'" + buttonId.Value + "\');", true);
                    }
                    else
                    {
                        Response.Redirect((string)("close.aspx?toggle=true" + strQuery), false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + m_intContentLanguage), false);
        }
    }

    private string GetBackPage(long contentid)
    {
        string result = "content.aspx";
        if (back_file.Length > 0)
        {
            result = back_file;
        }
        if (m_strPageAction == "publish" && back_action.ToLower() == "viewstaged")
        {
            //Fix the back page b/c when the action is "viewstaged", it is from the content.aspx
            if (result == "approval.aspx")
            {
                result = "content.aspx";
            }
            //Fix the action because staged version does not exists.
            if (!String.IsNullOrEmpty(Request.Form[submitasstagingview.UniqueID]))
            {
                result = result + "?action=viewstaged";
            }
            else
            {
                result = result + "?action=view";
            }
        }
        else if (back_action.ToLower() == "viewstaged" && blnUndoCheckOut_complete == true)
        {
            result = result + "?action=view";
        }
        else
        {
            if (m_strPageAction != "cancel" && back_action.ToLower() == "viewcontentbycategory")
            {
                // change old behavior of jumping back to view on adding new content and jump back to added content instead
                back_action = "view";
                back_id = contentid;
            }
            result = result + "?action=" + back_action;
        }
        if (back_action.ToLower() != "viewcontentbycategory" && back_action.ToLower() != "viewarchivecontentbycategory")
        {
            result = result + "&id=" + back_id + "&folder_id=" + back_folder_id;
        }
        else if (Convert.ToString(back_folder_id).Length > 0)
        {
            result = result + "&id=" + back_folder_id;
        }
        if (Convert.ToString(back_id).Length > 0)
        {
            result = result + "&contentid=" + back_id;
        }
        if (Convert.ToString(back_form_id).Length > 0)
        {
            result = result + "&form_id=" + back_form_id;
        }
        if (Convert.ToString((short)back_LangType).Length > 0)
        {
            result = result + "&LangType=" + back_LangType;
        }
        if (Convert.ToString(back_callerpage).Length > 0)
        {
            result = result + back_callerpage.Replace("&back_", "&");
        }
        if (Convert.ToString(back_origurl).Length > 0)
        {
            result = result + back_origurl.Replace("&back_", "&");
        }

        if (blnShowTStatusMessage == true)
        {
            result = result + "&ShowTStatusMsg=1";
        }
        return (result);
    }

    public string GetFolderPath(long Id)
    {
        ContentAPI contentAPI = new ContentAPI();
        SiteAPI siteAPI = new SiteAPI();

        szdavfolder = "ekdavroot";

        string sitePath = (string)(siteAPI.SitePath.ToString().TrimEnd(new char[] { '/' }).TrimStart(new char[] { '/' }));
        szdavfolder = (string)(szdavfolder.TrimEnd(new char[] { '/' }).TrimStart(new char[] { '/' }));
        if (Page.Request.Url.Host.ToLower() == "localhost")
        {
            szdavfolder = Page.Request.Url.Scheme + Uri.SchemeDelimiter + System.Net.Dns.GetHostName() + "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "")) + "/";
        }
        else
        {
            szdavfolder = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + "/" + sitePath + "/" + szdavfolder + "_" + siteAPI.UserId + "_" + siteAPI.UniqueId + (((Context.Request.QueryString["LangType"] != null) ? ("_" + Context.Request.QueryString["LangType"].ToString()) : "")) + "/";
        }

        string szFolderPath = contentAPI.EkContentRef.GetFolderPath(Id);
        szFolderPath = szFolderPath.Replace("\\", "/");
        szFolderPath = szFolderPath.TrimStart(new char[] { '/' });
        szFolderPath = szFolderPath.Replace("\\\\", "/");
        if (szFolderPath.Length > 0)
        {
            szFolderPath = szdavfolder + szFolderPath + "/";
        }
        else
        {
            szFolderPath = szdavfolder;
        }

        return szFolderPath;
    }
    #endregion

    #region TOOL BAR
    private void LoadToolBar(string FolderName)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string strMsg = string.Empty;
        if (FolderName.Length > 0)
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("alt Edit Content in Folder") + " \"" + FolderName + "\""));
        }
        else
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit content page title"));
        }
        result.Append("<table><tr>");

        //cancel
        string backurl = "action=" + HttpUtility.UrlDecode(Request.QueryString["back_action"]) + "&id=" + HttpUtility.UrlDecode(Request.QueryString["back_id"]) + "&LangType=" + HttpUtility.UrlDecode(Request.QueryString["back_LangType"]) + "&form_id=" + HttpUtility.UrlDecode(Request.QueryString["back_form_id"]);

        // edit.aspx page is opened in new window. So, back_file is not part of Request.QueryString. So, upon clicking on the back button, the window should be closed, just like how it is closed upon hitting publish button.

        if (back_file == string.Empty)
        {
            back_file = "close.aspx";
        }

        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/cancel.png", back_file + "?" + backurl, strMsg, m_refMsg.GetMessage("btn cancel"), "", StyleHelper.CancelButtonCssClass, true));

        //publish
        if (UserRights.CanPublish)
        {
            if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/contentPublish.png", "edit.aspx", m_refMsg.GetMessage("alt publish button text (save)"), m_refMsg.GetMessage("btn publish"), "onclick=\"ShowPane(\'dvContent\');buttonaction=\'publish\';elx2.GetBody(\'GetMACContent\');return false;\"", StyleHelper.PublishButtonCssClass, true));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/contentPublish.png", "edit.aspx", m_refMsg.GetMessage("alt publish button text (save)"), m_refMsg.GetMessage("btn publish"), "onclick=\"return SetAction(\'publish\');\"", StyleHelper.PublishButtonCssClass, true));
            }
        }
        else
        {
            if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", "edit.aspx", m_refMsg.GetMessage("alt submit button text (save)"), m_refMsg.GetMessage("btn submit"), "onclick=\"ShowPane(\'dvContent\');buttonaction=\'publish\';elx2.GetBody(\'GetMACContent\');return false;\"", StyleHelper.SubmitForApprovalButtonCssClass, true));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/approvalSubmitFor.png", "edit.aspx", m_refMsg.GetMessage("alt submit button text (save)"), m_refMsg.GetMessage("btn submit"), "onclick=\"return SetAction(\'publish\');\"", StyleHelper.SubmitForApprovalButtonCssClass, true));
            }
            submitasstagingview.Value = "true";
        }

        //undo checkout
        if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/contentRestore.png", "edit.aspx", m_refMsg.GetMessage("generic undocheckout"), m_refMsg.GetMessage("generic undocheckout"), "onclick=\"ShowPane(\'dvContent\');buttonaction=\'cancel\';elx2.GetBody(\'GetMACContent\');return false;\"", StyleHelper.UndoCheckout, false));
        }
        else
        {
            //don't allow cancel for multimedia
            //cancelling will force a newly dragged asset to be published.
            if (lContentType != Ektron.Cms.Common.EkConstants.CMSContentType_Media)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/contentRestore.png", "edit.aspx", m_refMsg.GetMessage("generic undocheckout"), m_refMsg.GetMessage("generic undocheckout"), "onclick=\"return SetAction(\'cancel\');\"", StyleHelper.UndoCheckout, false));
            }
        }

        //checkin
        if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/checkIn.png", "edit.aspx", m_refMsg.GetMessage("alt checkin button text (save)"), m_refMsg.GetMessage("btn checkin"), "onclick=\"ShowPane(\'dvContent\');buttonaction=\'checkin\';elx2.GetBody(\'GetMACContent\');return false;\"", StyleHelper.CheckInButtonCssClass));
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/checkIn.png", "edit.aspx", m_refMsg.GetMessage("alt checkin button text (save)"), m_refMsg.GetMessage("btn checkin"), "onclick=\"return SetAction(\'checkin\');\"", StyleHelper.CheckInButtonCssClass));
        }

        //save
        if ("" == Request.QueryString["multi"] || null == Request.QueryString["multi"])
        {
            if (IsMac && !IsBrowserIE && m_SelectedEditControl != "ContentDesigner")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/save.png", "edit.aspx", m_refMsg.GetMessage("alt save button text (content)"), m_refMsg.GetMessage("btn save"), "onclick=\"ShowPane(\'dvContent\');buttonaction=\'save\';elx2.GetBody(\'GetMACContent\');return false;\"", StyleHelper.SaveButtonCssClass));
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/save.png", "edit.aspx", m_refMsg.GetMessage("alt save button text (content)"), m_refMsg.GetMessage("btn save"), "onclick=\"return SetAction(\'save\');  \"", StyleHelper.SaveButtonCssClass));
            }
        }

        if (PreviousState == "A")
        {
			strMsg = m_refMsg.GetMessage("alt cancel button text (P)");
        }
        else
        {
			strMsg = m_refMsg.GetMessage("alt cancel button text (CI)");
        }

        if (m_refContentId > 0 && lContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && lContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData && m_strType.ToLower() != "add")
        {
            if (content_edit_data != null)
            {
                string aURL = string.Empty;
                //Check for Multisite Content
                if (content_edit_data.Quicklink.ToLower().IndexOf("http://") > -1 || content_edit_data.Quicklink.ToLower().IndexOf("https://") > -1 || content_edit_data.Quicklink.StartsWith(m_refContApi.SitePath))
                {
                    aURL = content_edit_data.Quicklink.IndexOf("?") > 1 ? content_edit_data.Quicklink + "&cmsMode=Preview" : content_edit_data.Quicklink + "?cmsMode=Preview";
                }
                else
                {
                    aURL = m_refContApi.SitePath + content_edit_data.Quicklink + "&cmsMode=Preview";
                }
                if (lContentType == Ektron.Cms.Common.EkConstants.CMSContentType_Content || lContentType == Ektron.Cms.Common.EkConstants.CMSContentType_XmlConfig)
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath + "images/UI/Icons/preview.png", "edit.aspx", m_refMsg.GetMessage("btn preview"), m_refMsg.GetMessage("btn preview"), "onclick=\"PreviewContent('" + aURL.Replace("ekfrm", "id") + "', '" + content_edit_data.Title.Replace("&#39;", "\\'") + "', ' " + content_edit_data.FolderId + "', '" + content_edit_data.Id + "'); return false;\"", StyleHelper.PreviewButtonCssClass));
                }
            }
        }

        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton((string)(m_refStyle.GetHelpAliasPrefix(folder_data) + "edittoolbar"), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion

    #region CSS, JS Registration

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
        if (m_SelectedEditControl == "eWebEditPro")
        {
            Ektron.Cms.API.JS.RegisterJS(this, this.AppPath + "java/ektron.ewebeditpro.tab.overrides.js", "EktronEWebEditProTabOverridesJS");
        }
    }

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, this.AppPath + "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        if (m_SelectedEditControl == "eWebEditPro")
        {
            Ektron.Cms.API.Css.RegisterCss(this, this.AppPath + "csslib/ektron.ewebeditpro.tab.overrides.css", "EktronEWebEditProTabOverridesCSS");
        }
    }

    #endregion
}
