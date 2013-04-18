using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
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
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.CustomFieldsApi;
using Ektron.Cms.Commerce;
using Ektron.Cms.DataIO.LicenseManager;
using Microsoft.VisualBasic.CompilerServices;
using Ektron.Cms.PageBuilder;

public partial class editfolderattributes : System.Web.UI.UserControl
{



    #region Member Variables

    private const string BLANK_HTML = "__EkBlankHTML";
    private SubscriptionData[] _SubscriptionDataList;
    private SubscriptionData[] _SubscribedDataList;
    private SubscriptionPropertiesData _SubscriptionPropertiesList;
    private bool _GlobalSubInherit = false;
    private int _FolderType = 0;
    private BlogData _BlogData;
    protected ContentAPI _ContentApi = new ContentAPI();
    protected CustomFieldsApi _CustomFieldsApi = new CustomFieldsApi();
    protected StyleHelper _StyleHelper = new StyleHelper();
    public EkMessageHelper _MessageHelper;
    protected long _Id = 0;
    protected FolderData _FolderData;
    protected FolderData parentfolderdata;
    protected PermissionData _PermissionData;
    protected string _AppImgPath = "";
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
    protected string _SelectedTaxonomyList = "";
    protected int _CurrentCategoryChecked = 0;
    protected string _SelectedTaxonomyParentList = "";
    protected int _ParentCategoryChecked = 0;
    protected Hashtable _AssignedFlags = new Hashtable();
    protected bool _IsUserBlog = false;
    protected ProductType _ProductType = null;
    protected bool _IsCatalog = false;
    protected string _IsPublishedAsPdf = string.Empty;

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {
        RegisterResources();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        _MessageHelper = _ContentApi.EkMsgRef;
    }

    #endregion

    #region Helpers

    public bool EditFolderAttributes()
    {
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }

        if (_FolderData == null)
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            _PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            _OrderBy = Convert.ToString(Request.QueryString["orderby"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["showpane"]))
        {
            _ShowPane = Convert.ToString(Request.QueryString["showpane"]);
        }
        else
        {
            _ShowPane = "";
        }
        if (!string.IsNullOrEmpty(Request.QueryString["folder_id"]))
        {
            _FolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
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
        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            _ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            _CustomFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage;
        }
        else
        {
            _ContentApi.ContentLanguage = _ContentLanguage;
            _CustomFieldsApi.ContentLanguage = _ContentLanguage;
        }
        _CurrentUserId = _ContentApi.UserId;
        _AppImgPath = _ContentApi.AppImgPath;
        _SitePath = _ContentApi.SitePath;
        _EnableMultilingual = _ContentApi.EnableMultilingual;
        if (!(Page.IsPostBack))
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true, true);
            _FolderType = _FolderData.FolderType;
            switch (_FolderType)
            {
                case 9:
                    _IsCatalog = true;
                    Display_EditCatalog();
                    break;
                default:
                    Display_EditFolder();
                    break;
            }
            phWebAlerts.Visible = _FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Catalog;
        }
        else
        {
            Process_DoFolderUpdate();
            return (true);
        }
        return true;
    }

    private void Display_AddCommunityFolder()
    {
        if (_FolderData == null)
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true);
        }
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Edit Community Folder " + " \"" + _FolderData.Name + "\"");
        ltr_vf_types.Visible = true;
        ltrTypes.Visible = true;
    }

    #endregion

    #region ACTION - DoFolderUpdate
    private void Process_DoFolderUpdate()
    {
        bool bInheritanceIsDif;
        bInheritanceIsDif = false;
        int isub = 0;
        string init_xmlconfig = Request.Form["init_xmlconfig"];
        string init_frm_xmlinheritance = Request.Form["init_frm_xmlinheritance"];
        Ektron.Cms.Content.EkXmlIndexing XmlInd;
        FolderData folder_data = null;
        Ektron.Cms.Content.EkContent m_refContent;
        SubscriptionPropertiesData sub_prop_data = new SubscriptionPropertiesData();
        Collection page_subscription_data = new Collection();
        Collection page_sub_temp = new Collection();
        Array arrSubscriptions;
        int i = 0;
        BlogRollItem[] abriRoll;
        string sCatTemp = "";
        List<string> siteAliasList = new List<string>();
        string[] arSiteAliasList;

        Ektron.Cms.SiteAliasApi _refSiteAliasApi;
        bool subscriptionRestore = false;

        m_refContent = _ContentApi.EkContentRef;
        if (_FolderId == -1)
        {
            _FolderId = _Id; //i.e Request.Form(folder_id.UniqueID)
        }
        _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        _FolderType = _FolderData.FolderType;

        if (Convert.ToString(_FolderId) != "")
        {

            if (_FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Catalog)
            {
                if (!string.IsNullOrEmpty(Request.Form["web_alert_inherit_checkbox"]))
                {
                    sub_prop_data.BreakInheritance = false;
                    subscriptionRestore = true;
                }
                else
                {
                    sub_prop_data.BreakInheritance = true;
                    if (!string.IsNullOrEmpty(Request.Form["web_alert_restore_inherit_checkbox"]))
                    {
                        subscriptionRestore = true;
                    }
                }

                if (Request.Form["notify_option"] == ("Always"))
                {
                    sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always;
                }
                else if (Request.Form["notify_option"] == ("Initial"))
                {
                    sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial;
                }
                else if (Request.Form["notify_option"] == ("Never"))
                {
                    sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never;
                }

                sub_prop_data.SuspendNextNotification = false;
                sub_prop_data.SendNextNotification = false;

                sub_prop_data.OptOutID = Convert.ToInt64(Request.Form["notify_optoutid"]);
                if (!string.IsNullOrEmpty((Request.Form["use_message_button"])))
                {
                    sub_prop_data.DefaultMessageID = Convert.ToInt64(Request.Form["notify_messageid"]);
                }
                else
                {
                    sub_prop_data.DefaultMessageID = 0;
                }
                if (!string.IsNullOrEmpty(Request.Form["use_summary_button"]))
                {
                    sub_prop_data.SummaryID = 1;
                }
                else
                {
                    sub_prop_data.SummaryID = 0;
                }
                if (!string.IsNullOrEmpty(Request.Form["use_content_button"]))
                {
                    sub_prop_data.ContentID = Convert.ToInt64(Request.Form["frm_content_id"]);
                }
                else
                {
                    sub_prop_data.ContentID = 0;
                }
                sub_prop_data.UnsubscribeID = Convert.ToInt64(Request.Form["notify_unsubscribeid"]);

                if (!string.IsNullOrEmpty(Request.Form["notify_url"]))
                {
                    sub_prop_data.URL = Request.Form["notify_url"];
                }
                else
                {
                    sub_prop_data.URL = Request.ServerVariables["HTTP_HOST"];
                }

                if (!string.IsNullOrEmpty(Request.Form["notify_weblocation"]))
                {
                    sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath + "subscriptions");
                }
                else
                {
                    sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath + "subscriptions");
                }
                if (!string.IsNullOrEmpty(Request.Form["notify_weblocation"]))
                {
                    sub_prop_data.WebLocation = Request.Form["notify_weblocation"];
                }
                else
                {
                    sub_prop_data.WebLocation = "subscriptions";
                }

                if (!string.IsNullOrEmpty(Request.Form["notify_subject"]))
                {
                    sub_prop_data.Subject = Request.Form["notify_subject"];
                }
                else
                {
                    sub_prop_data.Subject = "";
                }
                if (!string.IsNullOrEmpty(Request.Form["notify_emailfrom"]))
                {
                    sub_prop_data.EmailFrom = Request.Form["notify_emailfrom"];
                }
                else
                {
                    sub_prop_data.EmailFrom = "";
                }

                sub_prop_data.UseContentTitle = "";

                if (!string.IsNullOrEmpty(Request.Form["use_contentlink_button"]))
                {
                    sub_prop_data.UseContentLink = 1;
                }
                else
                {
                    sub_prop_data.UseContentLink = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["content_sub_assignments"]))
                {
                    arrSubscriptions = Strings.Split(Strings.Trim(Request.Form["content_sub_assignments"]), " ", -1, 0);
                    if (arrSubscriptions.Length > 0)
                    {
                        for (isub = 0; isub <= (arrSubscriptions.Length - 1); isub++)
                        {
                            if (arrSubscriptions.GetValue(isub).ToString() != ",") // ignore empty value when web alerts are inherited
                            {
                                page_sub_temp = new Collection();
                                page_sub_temp.Add(long.Parse(Strings.Mid(arrSubscriptions.GetValue(isub).ToString(), 10)), "ID", null, null);
                                page_subscription_data.Add(page_sub_temp, null, null, null);
                            }
                        }
                    }
                }
                else
                {
                    page_subscription_data = null;
                }
                page_sub_temp = null;
            }

            _PageData = new Collection();
            _PageData.Add(Request.Form["foldername"].Trim(".".ToCharArray()), "FolderName", null, null);
            if (!string.IsNullOrEmpty(Request.Form["isblog"]))
            {
                _PageData.Add(Request.Form[tagline.UniqueID], "FolderDescription", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[folderdescription.UniqueID], "FolderDescription", null, null);
            }
            _PageData.Add(Request.Form[folder_id.UniqueID], "FolderID", null, null);
            if (string.IsNullOrEmpty(Request.Form["TemplateTypeBreak"]))
            {
                _PageData.Add(Request.Form["templatefilename"], "TemplateFileName", null, null);
                //Defect # 54021 - Add failed. Duplicate key value supplied.
                //string templateName = (string)(Request.Form["templatefilename"].Split("(".ToCharArray())[0].TrimEnd());
                //TemplateData[] template_data;
                //template_data = _ContentApi.GetAllTemplates("TemplateFileName");
                //int j = 0;
                //for (j = 0; j <= template_data.Length - 1; j++)
                //{
                //    if (!(Request.Form["tinput_" + template_data[j].Id] == null) && template_data[j].FileName == templateName)
                //    {
                //        _PageData.Add(template_data[i].SubType, "TemplateSubType", null, null);
                //    }
                //}

                // Defect # 56113
                // Also added new hidden field to ascx + changes to TemplateConfigSave() in content.aspx
                if (!String.IsNullOrEmpty(Request.Form["defaultTemplateID"]))
                {
                    TemplateModel templateModel = new TemplateModel();
                    TemplateData template_data = templateModel.FindByID(long.Parse(Request.Form["defaultTemplateID"].ToString()));
                    _PageData.Add(template_data.SubType, "TemplateSubType", null, null);
                }
            }
            else
            {
                _PageData.Add("", "TemplateFileName", null, null);
            }
            //_PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
            _PageData.Add(Request.Form["stylesheet"], "StyleSheet", null, null);
            if (_FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
            {
                if (Strings.LCase(Request.Form["TypeBreak"]) == "on")
                {
                    if (init_frm_xmlinheritance == "0")
                    {
                        bInheritanceIsDif = true;
                    }
                    _PageData.Add(true, "XmlInherited", null, null);
                }
                else
                {
                    if (init_frm_xmlinheritance == "1")
                    {
                        bInheritanceIsDif = true;
                    }
                    _PageData.Add(false, "XmlInherited", null, null);
                }
                _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
            }
            else
            {
                bInheritanceIsDif = false;
                _PageData.Add(false, "XmlInherited", null, null);
                _PageData.Add(Ektron.Cms.Content.Calendar.WebCalendar.WebEventSmartformId.ToString(), "XmlConfiguration", null, null);
            }

            // handle multitemplates if there are any
            i = 1;
            Collection altinfo = new Collection();
            //While (Request.Form("namealt" + CStr(i)) <> "")
            //    Dim namealt As String = Request.Form("namealt" + CStr(i))
            //    Dim xmlconfigalt As String = Request.Form("xmlconfigalt" + CStr(i))
            //    If (xmlconfigalt = "ignore") Then xmlconfigalt = -1
            //    Dim templatealt As String = Request.Form("templatealt" + CStr(i))
            //    If (templatealt = "ignore") Then templatealt = -1
            //    If ((xmlconfigalt > -1) Or (templatealt > -1)) Then
            //        ' add this multitemplate only if a template or config is selected
            //        Dim multitemplate As New Collection
            //        multitemplate.Add(m_intFolderId, "FolderID")
            //        multitemplate.Add(xmlconfigalt, "CollectionID")
            //        multitemplate.Add(templatealt, "TemplateFileID")
            //        multitemplate.Add("", "CSSFile")
            //        multitemplate.Add(namealt, "Name")
            //        altinfo.Add(multitemplate)
            //    End If
            //    i = i + 1
            //End While
            //m_refContentApi.UpdateFolderContentTemplates(m_intFolderId, altinfo)


            bool isPublishedAsPdf = System.Convert.ToBoolean((Request.Form["publishAsPdf"] == "on") ? true : false);
            _PageData.Add(isPublishedAsPdf, "PublishPdfActive", null, null);

            // handle dynamic replication properties
            if (folder_data == null)
            {
                folder_data = _ContentApi.GetFolderById(_FolderId, true, true);
            }
            if (!string.IsNullOrEmpty(Request.Form["EnableReplication"]) || folder_data.IsCommunityFolder)
            {
                _PageData.Add(1, "EnableReplication", null, null);
            }
            else
            {
                _PageData.Add(0, "EnableReplication", null, null);
            }

            // add domain properties if they're there
            if ((!string.IsNullOrEmpty(Request.Form["IsDomainFolder"])) && (!string.IsNullOrEmpty(Request.Form["DomainProduction"])))
            {
                _PageData.Add(true, "IsDomainFolder", null, null);
                string staging = Request.Form["DomainStaging"];
                string production = Request.Form["DomainProduction"];
                if (staging == null)
                {
                    staging = "";
                }
                if (production == null)
                {
                    production = "";
                }
                if (staging.EndsWith("/"))
                {
                    staging = staging.Substring(0, staging.Length - 1);
                }
                if (production.EndsWith("/"))
                {
                    production = production.Substring(0, production.Length - 1);
                }
                if (staging == "")
                {
                    staging = production;
                }
                _PageData.Add(staging, "DomainStaging", null, null);
                _PageData.Add(production, "DomainProduction", null, null);
            }
            else
            {
                _PageData.Add(false, "IsDomainFolder", null, null);
            }
            if (!string.IsNullOrEmpty(Request.Form["isblog"])) //isblog
            {
                _PageData.Add(true, "isblog", null, null);
                _PageData.Add(Request.Form["blog_visibility"], "blog_visibility", null, null);
                _PageData.Add(Request.Form["blogtitle"], "blogtitle", null, null);
                if (!string.IsNullOrEmpty(Request.Form["postsvisible"]))
                {
                    _PageData.Add(Request.Form["postsvisible"], "postsvisible", null, null);
                }
                else
                {
                    _PageData.Add(-1, "postsvisible", null, null);
                }
                if (!string.IsNullOrEmpty(Request.Form["enable_comments"]))
                {
                    _PageData.Add(true, "enablecomments", null, null);
                }
                else
                {
                    _PageData.Add(false, "enablecomments", null, null);
                }
                if (!string.IsNullOrEmpty(Request.Form["moderate_comments"]))
                {
                    _PageData.Add(true, "moderatecomments", null, null);
                }
                else
                {
                    _PageData.Add(false, "moderatecomments", null, null);
                }
                if (!string.IsNullOrEmpty(Request.Form["require_authentication"]))
                {
                    _PageData.Add(true, "requireauthentication", null, null);
                }
                else
                {
                    _PageData.Add(false, "requireauthentication", null, null);
                }
                _PageData.Add(Request.Form["notify_url"], "notifyurl", null, null);
                if (!string.IsNullOrEmpty(Request.Form["categorylength"]))
                {
                    for (i = 0; i <= (Convert.ToInt64(Request.Form["categorylength"]) - 1); i++)
                    {
                        if (Request.Form["category" + i.ToString()] != "")
                        {
                            if (i == (Convert.ToInt64(Request.Form["categorylength"]) - 1))
                            {
                                sCatTemp += Strings.Replace(Request.Form["category" + i.ToString()], ";", "~@~@~", 1, -1, 0);
                            }
                            else
                            {
                                sCatTemp += (string)(Strings.Replace(Request.Form["category" + i.ToString()], ";", "~@~@~", 1, -1, 0) + ";");
                            }
                        }
                    }
                }
                _PageData.Add(sCatTemp, "blogcategories", null, null);

                if (!string.IsNullOrEmpty(Request.Form["rolllength"]))
                {
                    abriRoll = new BlogRollItem[1];
                    for (i = 0; i <= (Convert.ToInt64(Request.Form["rolllength"]) - 1); i++)
                    {
                        Array.Resize(ref abriRoll, i + 1);
                        if (!string.IsNullOrEmpty(Request.Form["editfolder_linkname" + i.ToString()]) && !string.IsNullOrEmpty(Request.Form["editfolder_url" + i.ToString()]))
                        {
                            //add only if we have something with a name/url
                            abriRoll[i] = new BlogRollItem();
                            abriRoll[i].LinkName = Request.Form["editfolder_linkname" + i.ToString()];
                            abriRoll[i].URL = Request.Form["editfolder_url" + i.ToString()];
                            if (!string.IsNullOrEmpty(Request.Form["editfolder_short" + i.ToString()]))
                            {
                                abriRoll[i].ShortDescription = Request.Form["editfolder_short" + i.ToString()];
                            }
                            else
                            {
                                abriRoll[i].ShortDescription = "";
                            }
                            if (!string.IsNullOrEmpty(Request.Form["editfolder_rel" + i.ToString()]))
                            {
                                abriRoll[i].Relationship = Request.Form["editfolder_rel" + i.ToString()];
                            }
                            else
                            {
                                abriRoll[i].Relationship = "";
                            }
                        }
                        else
                        {
                            abriRoll[i] = null;
                        }
                    }
                    _PageData.Add(abriRoll, "blogroll", null, null);
                }
                else
                {
                    _PageData.Add(null, "blogroll", null, null);
                }
            }
            if ((!string.IsNullOrEmpty(Request.Form["hdnInheritSitemap"])) && (Request.Form["hdnInheritSitemap"].ToString().ToLower() == "true"))
            {
                _PageData.Add(true, "SitemapPathInherit", null, null);
            }
            else
            {
                _PageData.Add(false, "SitemapPathInherit", null, null);
            }

            _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, this._ContentLanguage), "SitemapPath", null, null);
            if ((!string.IsNullOrEmpty(Request.Form["break_inherit_button"])) && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "InheritMetadata", null, null); //break inherit button is check.
            }
            else
            {
                _PageData.Add(0, "InheritMetadata", null, null);
            }
            _PageData.Add(Request.Form["inherit_meta_from"], "InheritMetadataFrom", null, null);

            if ((!string.IsNullOrEmpty(Request.Form["TaxonomyTypeBreak"])) && Request.Form["TaxonomyTypeBreak"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "InheritTaxonomy", null, null);
                if ((!string.IsNullOrEmpty(Request.Form["CategoryRequired"])) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "CategoryRequired", null, null);
                }
                else
                {
                    _PageData.Add(Request.Form[parent_category_required.UniqueID], "CategoryRequired", null, null);
                }
            }
            else
            {
                _PageData.Add(0, "InheritTaxonomy", null, null);
                if ((!string.IsNullOrEmpty(Request.Form["CategoryRequired"])) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "CategoryRequired", null, null);
                }
                else
                {
                    _PageData.Add(0, "CategoryRequired", null, null);
                }
            }
            string IdRequests = "";
            if ((Request.Form["taxlist"] != null) && Request.Form["taxlist"] != "")
            {
                IdRequests = Request.Form["taxlist"];
            }
            _PageData.Add(IdRequests, "TaxonomyList", null, null);
            _PageData.Add(Request.Form[inherit_taxonomy_from.UniqueID], "InheritTaxonomyFrom", null, null);
            //--------------------IscontentSearchable-----------------------------
            if (Request.Form["chkInheritIscontentSearchable"] != null && Request.Form["chkInheritIscontentSearchable"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "IsContentSearchableInherited", null, null);
                if (Request.Form["chkIscontentSearchable"] != null && Request.Form["chkIscontentSearchable"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "IscontentSearchable", null, null);
                }
                else
                {
                    _PageData.Add(Request.Form[current_IscontentSearchable.UniqueID], "IscontentSearchable", null, null);
                }
            }
            else
            {
                _PageData.Add(0, "IsContentSearchableInherited", null, null);
                if (Request.Form["chkIscontentSearchable"] != null && Request.Form["chkIscontentSearchable"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "IscontentSearchable", null, null);
                }
                else
                {
                    _PageData.Add(0, "IscontentSearchable", null, null);
                }
            }
            _PageData.Add(Request.Form[inherit_IscontentSearchable_from.UniqueID], "IsContentSearchableInheritedFrom", null, null); 
            //--------------------IsContentSearchable End -------------------------
            //-------------------DisplaySettings--------------------
            int totalTabs = 0;
            if (Request.Form["chkInheritIsDisplaySettings"] != null && Request.Form["chkInheritIsDisplaySettings"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "IsDisplaySettingsInherited", null, null);

                if ((Request.Form["chkIsDisplaySettingsAllTabs"] != null && Request.Form["chkIsDisplaySettingsAllTabs"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsSummary"] != null && Request.Form["chkIsDisplaySettingsSummary"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsMetaData"] != null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsAliasing"] != null && Request.Form["chkIsDisplaySettingsAliasing"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsSchedule"] != null && Request.Form["chkIsDisplaySettingsSchedule"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsComment"] != null && Request.Form["chkIsDisplaySettingsComment"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTemplates"] != null && Request.Form["chkIsDisplaySettingsTemplates"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTaxonomy"] != null && Request.Form["chkIsDisplaySettingsTaxonomy"].ToString().ToLower() == "on"))
                {
                    if (Request.Form["chkIsDisplaySettingsSummary"] != null && Request.Form["chkIsDisplaySettingsSummary"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Summary;
                    }
                    if (Request.Form["chkIsDisplaySettingsMetaData"] != null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.MetaData;
                    }
                    if (Request.Form["chkIsDisplaySettingsAliasing"] != null && Request.Form["chkIsDisplaySettingsAliasing"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Aliasing;
                    }
                    if (Request.Form["chkIsDisplaySettingsSchedule"] != null && Request.Form["chkIsDisplaySettingsSchedule"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Schedule;
                    }
                    if (Request.Form["chkIsDisplaySettingsComment"] != null && Request.Form["chkIsDisplaySettingsComment"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Comment;
                    }
                    if (Request.Form["chkIsDisplaySettingsTemplates"] != null && Request.Form["chkIsDisplaySettingsTemplates"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Templates;
                    }
                    if (Request.Form["chkIsDisplaySettingsTaxonomy"] != null && Request.Form["chkIsDisplaySettingsTaxonomy"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy;
                    }
                    _PageData.Add(totalTabs, "DisplaySettings", null, null);
                }
                else
                {
                    _PageData.Add(Request.Form[current_IsDisplaySettings.UniqueID], "DisplaySettings", null, null);
                }
            }
            else
            {
                _PageData.Add(0, "IsDisplaySettingsInherited", null, null);
                if ((Request.Form["chkIsDisplaySettingsAllTabs"] != null && Request.Form["chkIsDisplaySettingsAllTabs"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsSummary"] != null && Request.Form["chkIsDisplaySettingsSummary"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsMetaData"] != null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsAliasing"] != null && Request.Form["chkIsDisplaySettingsAliasing"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsSchedule"] != null && Request.Form["chkIsDisplaySettingsSchedule"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsComment"] != null && Request.Form["chkIsDisplaySettingsComment"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTemplates"] != null && Request.Form["chkIsDisplaySettingsTemplates"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTaxonomy"] != null && Request.Form["chkIsDisplaySettingsTaxonomy"].ToString().ToLower() == "on"))
                {
                    if (Request.Form["chkIsDisplaySettingsSummary"] != null && Request.Form["chkIsDisplaySettingsSummary"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Summary;
                    }
                    if (Request.Form["chkIsDisplaySettingsMetaData"] != null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.MetaData;
                    }
                    if (Request.Form["chkIsDisplaySettingsAliasing"] != null && Request.Form["chkIsDisplaySettingsAliasing"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Aliasing;
                    }
                    if (Request.Form["chkIsDisplaySettingsSchedule"] != null && Request.Form["chkIsDisplaySettingsSchedule"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Schedule;
                    }
                    if (Request.Form["chkIsDisplaySettingsComment"] != null && Request.Form["chkIsDisplaySettingsComment"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Comment;
                    }
                    if (Request.Form["chkIsDisplaySettingsTemplates"] != null && Request.Form["chkIsDisplaySettingsTemplates"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Templates;
                    }
                    if (Request.Form["chkIsDisplaySettingsTaxonomy"] != null && Request.Form["chkIsDisplaySettingsTaxonomy"].ToString().ToLower() == "on")
                    {
                        totalTabs += (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy;
                    }
                    _PageData.Add(totalTabs, "DisplaySettings", null, null);
                }
                else
                {
                    _PageData.Add(1, "DisplaySettings", null, null);
                }
            }
            _PageData.Add(Request.Form[inherit_IsDisplaySettings_from.UniqueID], "DisplaySettingsInheritedFrom", null, null);
            //-------------------DisplaySettingsEnd------------------

            if (Request.Form["chkInheritAliases"] != null && Request.Form["chkInheritAliases"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "InheritAlias", null, null);
                if (Request.Form["chkForceAliasing"] != null && Request.Form["chkForceAliasing"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "AliasRequired", null, null);
                }
                else
                {
                    _PageData.Add(Request.Form[current_alias_required.UniqueID], "AliasRequired", null, null);
                }
            }
            else
            {
                _PageData.Add(0, "InheritAlias", null, null);
                if (Request.Form["chkForceAliasing"] != null && Request.Form["chkForceAliasing"].ToString().ToLower() == "on")
                {
                    _PageData.Add(1, "AliasRequired", null, null);
                }
                else
                {
                    _PageData.Add(0, "AliasRequired", null, null);
                }
            }
            _PageData.Add(Request.Form[inherit_alias_from.UniqueID], "InheritAliasFrom", null, null);
            // Update - add flagging items:
            ProcessFlaggingPostBack(_PageData);

            m_refContent.UpdateFolderPropertiesv2_0(_PageData);
            if (folder_data.FolderType == 2) //OrElse folder_data.Id = 0 Avoiding root to be site aliased
            {
                arSiteAliasList = Request.Form["savedSiteAlias"].TrimStart(" ".ToCharArray()).TrimStart(",".ToCharArray()).Split(",".ToCharArray());
                foreach (string aliasStr in arSiteAliasList)
                {
                    if (aliasStr != string.Empty)
                    {
                        siteAliasList.Add(aliasStr);
                    }
                }
                _refSiteAliasApi = new Ektron.Cms.SiteAliasApi();
                _refSiteAliasApi.Save(folder_data.Id, siteAliasList);
            }
            if ((string.IsNullOrEmpty(Request.Form["suppress_notification"])) && (_FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Catalog))
            {
                m_refContent.UpdateSubscriptionPropertiesForFolder(_FolderId, sub_prop_data);
                m_refContent.UpdateSubscriptionsForFolder(_FolderId, page_subscription_data);
            }
            if (subscriptionRestore)
            {
                m_refContent.DeleteSubscriptionsForContentinFolder(_FolderId);
            }

            if ((init_xmlconfig != Request.Form["xmlconfig"] || bInheritanceIsDif) && _FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
            {
                XmlInd = _ContentApi.EkXmlIndexingRef;
                if (Request.Form["xmlconfig"] != "0" && Request.Form["xmlconfig"] != "")
                {
                    XmlInd.ReIndexAllDoc(Convert.ToInt64(Request.Form["xmlconfig"]));
                }
                else // inheritance has been turned on
                {
                    if (Strings.LCase(Request.Form["frm_xmlinheritance"]) == "on")
                    {
                        folder_data = _ContentApi.GetFolderById(_FolderId, false, true);
                        if (!(folder_data.XmlConfiguration == null))
                        {
                            for (int x = 0; x <= (folder_data.XmlConfiguration.Length - 1); x++)
                            {
                                XmlInd.ReIndexAllDoc(folder_data.XmlConfiguration[x].Id);
                            }
                            //reverting 27535 - do not udpate xml_index table with new xml index search
                        }
                        else
                        {
                            XmlInd.RemoveAllIndexDoc(_FolderId);
                        }
                        //reverting 27535 - do not udpate xml_index table with new xml index search
                    }
                    else
                    {
                        XmlInd.RemoveAllIndexDoc(_FolderId);
                    }
                }

            }

            if (string.IsNullOrEmpty(Request.Form["break_inherit_button"]))
            {
                _CustomFieldsApi.ProcessCustomFields(_FolderId);
            }
            else if ((Request.Form["break_inherit_button"] != null) && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
            {
                if (folder_data.MetaInherited == 0)
                {
                    _CustomFieldsApi.ProcessCustomFields(_FolderId);
                }
            }

            //If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            //    'break inherit button is checked.
            //    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            //ElseIf folder_data.MetaInherited = 0 Then
            //    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            //ElseIf Request.Form("break_inherit_button") Is Nothing Then
            //    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            //End If
        }
        if (Request.Form["oldfoldername"] == Request.Form["foldername"])
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + Request.Form[folder_id.UniqueID]), false);
        }
        else
        {
            Response.Redirect("content.aspx?TreeUpdated=1&LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + Request.Form[folder_id.UniqueID] + "&reloadtrees=Forms,Content,Library", false);
        }
        if (folder_data.FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Catalog)
        {
            ProcessProductTemplatesPostBack();
        }
        else
        {
            ProcessContentTemplatesPostBack();
        }
    }
    #endregion

    #region FOLDER - EditFolder
    private void Display_EditFolder()
    {
        TemplateData[] template_data;
        XmlConfigData[] xmlconfig_data;
        bool isBlog = System.Convert.ToBoolean(_FolderType == 1);
        int i = 0;

        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl inherit from parent");

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);

        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt");

        if (isBlog)
        {
            _BlogData = _ContentApi.BlogObject(_FolderData);
            _IsUserBlog = _ContentApi.IsUserBlog(_BlogData.Id);
            _FolderData.PublishPdfEnabled = false;
            EditFolderToolBar();
            phSubjects.Visible = true;
            phBlogRoll.Visible = true;
            phDescription.Visible = false;
        }
        else
        {
            EditFolderToolBar();
        }

        template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        xmlconfig_data = _ContentApi.GetAllXmlConfigurations("title");

        oldfolderdescription.Value = Server.HtmlDecode(_FolderData.Description);
        folderdescription.Value = Server.HtmlDecode(_FolderData.Description);
        folder_id.Value = _FolderData.Id.ToString();
        if (_Id == 0)
        {
            phFolderProperties1.Visible = true;
            lit_ef_folder.Text = _FolderData.Name + "<input type=\"hidden\" value=\"" + _FolderData.Name + "\" name=\"foldername\"/>";
            lit_ef_folder.Text += "<input type=\"hidden\" value=\"" + _FolderData.Name + "\" name=\"oldfoldername\"/>";
        }
        else
        {
            if (isBlog)
            {
                phBlogProperties1.Visible = true;
                phBlogProperties2.Visible = true;
                td_vf_nametxt.InnerHtml = "<input type=\"text\" maxlength=\"75\" size=\"30\" value=\"" + _FolderData.Name + "\" name=\"foldername\" />";
                td_vf_nametxt.InnerHtml += "<input type=\"hidden\" value=\"" + EkFunctions.HtmlEncode(_FolderData.Name) + "\" name=\"oldfoldername\" id=\"oldfoldername\" />";
                td_vf_nametxt.InnerHtml += "<input type=\"hidden\" name=\"isblog\" id=\"isblog\" value=\"true\"/>";
                td_vf_titletxt.InnerHtml = "<input type=\"text\" maxlength=\"75\" size=\"30\" value=\"" + _BlogData.Title + "\" name=\"blogtitle\" id=\"blogtitle\" />";
                td_vf_visibilitytxt.InnerHtml = "<select name=\"blog_visibility\" id=\"blog_visibility\">";
                if (_BlogData.Visibility == Ektron.Cms.Common.EkEnumeration.BlogVisibility.Public)
                {
                    td_vf_visibilitytxt.InnerHtml += "<option value=\"0\" selected>" + _MessageHelper.GetMessage("lbl public") + "</option>";
                    td_vf_visibilitytxt.InnerHtml += "<option value=\"1\">" + _MessageHelper.GetMessage("lbl private") + "</option>";
                }
                else
                {
                    td_vf_visibilitytxt.InnerHtml += "<option value=\"0\">" + _MessageHelper.GetMessage("lbl public") + "</option>";
                    td_vf_visibilitytxt.InnerHtml += "<option value=\"1\" selected>" + _MessageHelper.GetMessage("lbl private") + "</option>";
                }
                td_vf_visibilitytxt.InnerHtml += "</select>";
                tagline.Value = Server.HtmlDecode(_BlogData.Tagline);
                if (_BlogData.PostsVisible < 0)
                {
                    td_vf_postsvisibletxt.InnerHtml = "<input type=\"text\" name=\"postsvisible\" id=\"postsvisible\" value=\"\" size=\"1\" maxlength=\"3\"/>";
                }
                else
                {
                    td_vf_postsvisibletxt.InnerHtml = "<input type=\"text\" name=\"postsvisible\" id=\"postsvisible\" value=\"" + _BlogData.PostsVisible.ToString() + "\" size=\"1\" maxlength=\"3\"/>";
                }
                td_vf_postsvisibletxt.InnerHtml += "<div class=\"ektronCaption\">"+ _MessageHelper.GetMessage("Post Visible warning")+"</div>";
                if (_BlogData.EnableComments == true)
                {
                    td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"enable_comments\" id=\"enable_comments\" checked=\"checked\" onclick=\"UpdateBlogCheckBoxes();\" />" + _MessageHelper.GetMessage("lbl enable comments");
                    td_vf_commentstxt.InnerHtml += "<br />";
                    if (_BlogData.ModerateComments)
                    {
                        td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"moderate_comments\" id=\"moderate_comments\" checked=\"checked\" />" + _MessageHelper.GetMessage("lbl moderate comments");
                    }
                    else
                    {
                        td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"moderate_comments\" id=\"moderate_comments\" />"+ _MessageHelper.GetMessage("lbl moderate comments");
                    }
                    td_vf_commentstxt.InnerHtml += "<br />";
                    if (_BlogData.RequiresAuthentication)
                    {
                        td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"require_authentication\" id=\"require_authentication\" checked=\"checked\" />" + _MessageHelper.GetMessage("lbl require authentication");
                    }
                    else
                    {
                        td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"require_authentication\" id=\"require_authentication\" />" + _MessageHelper.GetMessage("lbl require authentication");
                    }
                }
                else
                {
                    td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"enable_comments\" id=\"enable_comments\" onclick=\"UpdateBlogCheckBoxes();\" />"+_MessageHelper.GetMessage("lbl enable comments")+"<br />";
                    td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"moderate_comments\" id=\"moderate_comments\" disabled=\"disabled\"/>"+ _MessageHelper.GetMessage("lbl moderate comments")+"<br />";
                    td_vf_commentstxt.InnerHtml += "<input type=\"checkbox\" name=\"require_authentication\" id=\"require_authentication\" disabled=\"disabled\"/>" + _MessageHelper.GetMessage("lbl require authentication")+"<br />";
                }
                if (_BlogData.NotifyURL != "")
                {
                    td_vf_updateservicestxt.InnerHtml += "<input type=\"checkbox\" name=\"chknotify_url\" id=\"chknotify_url\" checked=\"checked\" />"+ _MessageHelper.GetMessage("lbl Notify blog");
                    td_vf_updateservicestxt.InnerHtml += "<br />";
                    td_vf_updateservicestxt.InnerHtml += "<input type=\"text\" maxlength=\"75\" size=\"40\" value=\"" + EkFunctions.HtmlEncode(_BlogData.NotifyURL) + "\" name=\"notify_url\" id=\"notify_url\"/>";
                }
                else
                {
                    td_vf_updateservicestxt.InnerHtml += "<input type=\"checkbox\" name=\"chknotify_url\" id=\"chknotify_url\" />" + _MessageHelper.GetMessage("lbl Notify blog");
                    td_vf_updateservicestxt.InnerHtml += "<br />";
                    td_vf_updateservicestxt.InnerHtml += "<input type=\"text\" maxlength=\"75\" size=\"40\" value=\"\" name=\"notify_url\" id=\"notify_url\"/>";
                }
            }
            else
            {
                phFolderProperties1.Visible = true;
                lit_ef_folder.Text = "<input type=\"text\" maxlength=\"100\" size=\"75\" value=\"" + _FolderData.Name + "\" name=\"foldername\"><input type=\"hidden\" value=\"\" name=\"oldfoldername\" id=\"oldfoldername\" />";
            }
        }
        if ((_FolderData.StyleSheetInherited) && (_FolderData.StyleSheet != ""))
        {
            lit_ef_ss.Text = _ContentApi.SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"stylesheet\" />";
            lit_ef_ss.Text += "<br/>";
            lit_ef_ss.Text += "<span class=\"ektronCaption\">";
            lit_ef_ss.Text += _MessageHelper.GetMessage("inherited style sheet msg") + _ContentApi.SitePath + _FolderData.StyleSheet;
            lit_ef_ss.Text += "</span>";
        }
        else
        {
            lit_ef_ss.Text = _ContentApi.SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"" + _FolderData.StyleSheet + "\" name=\"stylesheet\" />";
        }
        lit_ef_templatedata.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\" />";
        

        DrawContentTemplatesTable();
        DrawFlaggingOptions();
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

        if (_FolderData.PublishPdfEnabled && _FolderType != (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            phPDF.Visible = true;
            _IsPublishedAsPdf = (string)(_FolderData.PublishPdfActive ? "checked=\"checked\" " : string.Empty);
            this.lblPublishAsPdf.InnerText = _MessageHelper.GetMessage("publish as pdf");
        }
        else
        {
            _IsPublishedAsPdf = string.Empty;
            phPDF.Visible = false;
        }

        // only top level folders can be domain folders and only if not a blog folder already
        CommonApi m_refCommonAPI = new CommonApi();
        Ektron.Cms.Common.EkRequestInformation request_info = m_refCommonAPI.RequestInformationRef;

        if ((_FolderType != 1) && (_FolderData.ParentId == 0) && (_Id != 0))
        {
            SettingsData settings_list;
            SiteAPI m_refSiteAPI = new SiteAPI();

            settings_list = m_refSiteAPI.GetSiteVariables(-1);
            //string schk = "";
            string disdomain = "";
            //if (_FolderData.IsDomainFolder)
            //{
            //    schk = " checked ";
            //}
            //else
            //{
            //    disdomain = " disabled ";
            //}
            if (!_FolderData.IsDomainFolder)
            {
                disdomain = " disabled ";
            }
            if (_FolderType == 2 && LicenseManager.IsFeatureEnable(request_info, Feature.MultiSite))
            {
                // Domain folder checkbox replaced to hidden field.

                //DomainFolder.Text += "<tr><td colspan=""2"">&nbsp;</td></tr><tr><td colspan=""2"" class=""input-box-text"">Multi-Site Domain Configuration:</td></tr>"
                //DomainFolder.Text += "<tr><td colspan=""2""><input type=""checkbox""  disabled= ""true"" name=""IsDomainFolder"" id=""IsDomainFolder""" & schk & " onClick="""
                //If (settings_list.AsynchronousStaging) Then
                //    DomainFolder.Text += "document.forms[0].DomainStaging.disabled = !document.forms[0].IsDomainFolder.checked; "
                //End If
                //DomainFolder.Text += "document.forms[0].DomainProduction.disabled = !document.forms[0].IsDomainFolder.checked;"
                //If (Not request_info.LinkManagement) Then
                //    DomainFolder.Text += " if (document.forms[0].IsDomainFolder.checked) alert('Please set ek_LinkManagement to True in your web.config');"
                //End If
                //DomainFolder.Text += """/><label for=""IsDomainFolder"">" & m_refMsg.GetMessage("alt Domain for this folder") & "</label></td></tr>"

                DomainFolder.Text += "<input type=\"hidden\" name=\"IsDomainFolder\" id=\"IsDomainFolder\" value=\"on\"/>";

                // staging field should only show up on staging servers; production server can see production field
                DomainFolder.Text += "<tr>";
                DomainFolder.Text += "<td class=\"label\"><label for=\"DomainStaging\">" + _MessageHelper.GetMessage("lbl Staging Domain") + ":</label></td>";
                DomainFolder.Text += "<td class=\"value\">http://&nbsp;<input type=\"text\" name=\"DomainStaging\" id=\"DomainStaging\" size=\"50\" value=\"" + _FolderData.DomainStaging + ("\"" + disdomain) + "/></td>";
                DomainFolder.Text += "</tr>";

                DomainFolder.Text += "<tr>";
                DomainFolder.Text += "<td class=\"label\"><label for=\"DomainProduction\">" + _MessageHelper.GetMessage("lbl Production Domain") + ":</label></td>";
                DomainFolder.Text += "<td class=\"value\">http://&nbsp;<input type=\"text\" name=\"DomainProduction\" id=\"DomainProduction\" size=\"50\" value=\"" + _FolderData.DomainProduction + ("\"" + disdomain) + "/></td>";
                DomainFolder.Text += "</tr>";
                if (_FolderData.IsDomainFolder && _FolderData.ParentId == 0)
                {
                    StringBuilder categorydata = new StringBuilder();
                    if ((_FolderData.FolderTaxonomy != null) && _FolderData.FolderTaxonomy.Length > 0)
                    {
                        for (int d = 0; d <= _FolderData.FolderTaxonomy.Length - 1; d++)
                        {
                            if (_SelectedTaxonomyList.Length > 0)
                            {
                                _SelectedTaxonomyList = _SelectedTaxonomyList + "," + _FolderData.FolderTaxonomy[d].TaxonomyId;
                            }
                            else
                            {
                                _SelectedTaxonomyList = _FolderData.FolderTaxonomy[d].TaxonomyId.ToString();
                            }
                        }
                    }
                    _CurrentCategoryChecked = Convert.ToInt32(_FolderData.CategoryRequired);
                    current_category_required.Value = _CurrentCategoryChecked.ToString();
                    inherit_taxonomy_from.Value = _FolderData.TaxonomyInheritedFrom.ToString();
                }
            }
        }

        // handle dynamic replication settings
        if (request_info.EnableReplication && !(_FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum || _FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard))
        {
            bool bShowReplicationMethod = true;
            if (_FolderData.ParentId != 0 && (_FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Blog))
            {
                // don't show for blogs under community folder
                FolderData tmp_folder_data = null;
                tmp_folder_data = this._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId);
                if (tmp_folder_data.FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Community)
                {
                    bShowReplicationMethod = false;
                }
            }
            if (bShowReplicationMethod)
            {
                string schk = "";
                if (_FolderData.ReplicationMethod == 1)
                {
                    schk = " checked";
                }

                if (!_FolderData.IsCommunityFolder)
                {
                    ReplicationMethod.Text = _MessageHelper.GetMessage("lbl folderdynreplication");
                    ReplicationMethod.Text += "<input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\"" + schk + " ><label for=\"EnableReplication\"/>" + _MessageHelper.GetMessage("replicate folder contents") + "</label>";
                }
            }
            else
            {
                // if we're not showing it, it means replication is enabled because we're under a parent community folder
                ReplicationMethod.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"1\" />";
            }
        }

        // show categories if its a blog
        if (isBlog)
        {
            ltr_vf_categories.Text += "<div id=\"parah\">";
            if (!(_BlogData.Categories == null) && _BlogData.Categories.Length > 0 && _BlogData.Categories[0].Length > 0)
            {
                for (i = 0; i <= _BlogData.Categories.Length - 1; i++)
                {
                    ltr_vf_categories.Text += "<input type=\'text\' id=\'category" + i.ToString() + "\' name=\'category" + i.ToString() + "\' onChange=\'saveValue(" + i.ToString() + ",this.value)\' value=\'" + Strings.Replace((string)(_BlogData.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0).Replace("\'", "&#39;") + "\' maxlength=\'75\' size=\'75\'/> ";
                    ltr_vf_categories.Text += "<a href=\"#Remove\" onclick=\"removeInput(" + i.ToString() + ");return false;\" class=\"button buttonInlineBlock redHover buttonRemove\">" + _MessageHelper.GetMessage("btn remove") + "</a>";
                    ltr_vf_categories.Text += "<div class=\'ektronTopSpace\'></div>";
                    ltr_vf_categories.Text += "<script type=\"text/javascript\">addInputInit(\'" + Strings.Replace((string)(_BlogData.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0).Replace("\'", "\\\'") + "\');</script>";
                    //<p>" & blog_data.Categories(i).ToString() & "</p>"
                }
                ltr_vf_categories.Text += "</div>";
                ltr_vf_categories.Text += "<input type=\"hidden\" id=\"categorylength\" name=\"categorylength\" value=\"" + _BlogData.Categories.Length.ToString() + "\" />";
            }
            else
            {
                ltr_vf_categories.Text += "</div>";
                ltr_vf_categories.Text += "<input type=\"hidden\" id=\"categorylength\" name=\"categorylength\" value=\"0\" />";
            }
            ltr_vf_categories.Text += "<a href=\"#Add\" onclick=\"addInput();return false;\" class=\"button buttonInlineBlock greenHover buttonAdd\">" + this._MessageHelper.GetMessage("lnk add new subject") + "</a>";
            ltr_vf_categories.Text += "<a href=\"#Remove\" onclick=\"deleteInput();return false;\" class=\"button buttonInlineBlock redHover buttonRemove\">" + this._MessageHelper.GetMessage("lnk remove last subject") + "</a>";
            Literal ltrT = new Literal();
            ltrT.Text += "<div id=\"proll\" name=\"proll\">";
            if (!(_BlogData.BlogRoll == null) && _BlogData.BlogRoll.Length() > 0)
            {
                for (i = 0; i <= _BlogData.BlogRoll.Length() - 1; i++)
                {
                    ltrT.Text += "<a href=\"#\" class=\"button buttonInlineBlock redHover buttonRemove\" onClick=\"removeRoll(" + i.ToString() + ")\">Remove Roll Link</a>";
                    ltrT.Text += "<div class=\"ektronTopSpace\"></div>";
                    ltrT.Text += "<table class=\"ektronGrid\">";
                    ltrT.Text += "  <tr>";
                    ltrT.Text += "      <td class=\"label\">Link Name:</td>";
                    ltrT.Text += "      <td class=\"value\"><input name=\"editfolder_linkname" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(_BlogData.BlogRoll.RollItem(i).LinkName) + "\" size=\"55\" id=\"editfolder_linkname" + i.ToString() + "\" onChange=\"saveRoll(" + i.ToString() + ",this.value,\'linkname\')\" /></td>";
                    ltrT.Text += "  </tr>";
                    ltrT.Text += "  <tr>";
                    ltrT.Text += "      <td class=\"label\">URL:</td>";
                    ltrT.Text += "      <td class=\"value\"><input name=\"editfolder_url" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(_BlogData.BlogRoll.RollItem(i).URL) + "\" size=\"55\" id=\"editfolder_url" + i.ToString() + "\" onChange=\"saveRoll(" + i.ToString() + ",this.value,\'url\')\" /></td>";
                    ltrT.Text += "  </tr>";
                    ltrT.Text += "  <tr>";
                    ltrT.Text += "      <td class=\"label\">Short Description:</td>";
                    ltrT.Text += "      <td class=\"value\"><input name=\"editfolder_short" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(_BlogData.BlogRoll.RollItem(i).ShortDescription) + "\" size=\"55\" id=\"editfolder_short" + i.ToString() + "\" onChange=\"saveRoll(" + i.ToString() + ",this.value,\'short\')\" /></td>";
                    ltrT.Text += "  </tr>";
                    ltrT.Text += "  <tr>";
                    ltrT.Text += "      <td class=\"label\">Relationship:</td>";
                    ltrT.Text += "      <td class=\"value\">";
                    ltrT.Text += "          <input name=\"editfolder_rel" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(_BlogData.BlogRoll.RollItem(i).Relationship) + "\" size=\"45\" id=\"editfolder_rel" + i.ToString() + "\" onChange=\"saveRoll(" + i.ToString() + ",this.value,\'rel\')\" />&nbsp;\"";
                    ltrT.Text += "          <a style=\"padding-top: .25em; padding-bottom: .25em;\" class=\"button buttonInline blueHover buttonEdit\" href=\"#\" onClick=\"window.open(\'blogs/xfnbuilder.aspx?field=editfolder_rel" + i.ToString() + "&id=" + i.ToString() + "\',\'XFNBuilder\',\'location=0,status=0,scrollbars=0,width=500,height=300\');\">Edit</a>";
                    ltrT.Text += "      </td>";
                    ltrT.Text += "  </tr>";
                    ltrT.Text += "</table>";
                    ltrT.Text += "<div class=\"ektronTopSpace\"></div>";
                    ltrT.Text += "<script type=\"text/javascript\">addRollInit(\'" + Strings.Replace(_BlogData.BlogRoll.RollItem(i).LinkName, "\'", "\\\'", 1, -1, 0) + "\',\'" + Strings.Replace(_BlogData.BlogRoll.RollItem(i).URL, "\'", "\\\'", 1, -1, 0) + "\',\'" + Strings.Replace(_BlogData.BlogRoll.RollItem(i).ShortDescription, "\'", "\\\'", 1, -1, 0) + "\',\'" + Strings.Replace(_BlogData.BlogRoll.RollItem(i).Relationship, "\'", "\\\'", 1, -1, 0) + "\');</script>";
                }
            }
            ltrT.Text += "</div>";
            ltrT.Text += "<input type=\"hidden\" id=\"rolllength\" name=\"rolllength\" value=\"" + _BlogData.BlogRoll.Length().ToString() + "\" />";
            ltrT.Text += "<div class=\"ektronTopSpace\"></div>";
            ltrT.Text += "<a href=\"javascript:addRoll()\" class=\"button buttonInlineBlock greenHover buttonAdd\">" + _MessageHelper.GetMessage("lnk add new roll link") + "</a>";
            ltrT.Text += "<a href=\"javascript:deleteRoll()\" class=\"button buttonInlineBlock redHover buttonRemove\">" + _MessageHelper.GetMessage("lnk remove last roll link") + "</a>";
            lbl_vf_roll.Controls.Add(ltrT);
        }

        if (_Id == 0)
        {
            js_ef_focus.Text = "Ektron.ready(function(){document.forms.frmContent.stylesheet.focus();});";
        }
        else
        {
            if (!(Request.QueryString["showpane"] != ""))
            {
                js_ef_focus.Text = "Ektron.ready(function() { document.forms.frmContent.foldername.focus();" + Environment.NewLine;
                js_ef_focus.Text += "   if( $ektron(\'#web_alert_inherit_checkbox\').length > 0 ){" + Environment.NewLine;
                js_ef_focus.Text += "       if( $ektron(\'#web_alert_inherit_checkbox\')[0].checked ){" + Environment.NewLine;
                js_ef_focus.Text += "           $ektron(\'.selectContent\').css(\'display\', \'none\');" + Environment.NewLine;
                js_ef_focus.Text += "           $ektron(\'.useCurrent\').css(\'display\', \'none\');" + Environment.NewLine;
                js_ef_focus.Text += "       } " + Environment.NewLine;
                js_ef_focus.Text += "   } " + Environment.NewLine;
                js_ef_focus.Text += "});" + Environment.NewLine;
            }
            js_ef_focus.Text += "function UpdateBlogCheckBoxes() {" + Environment.NewLine;
            js_ef_focus.Text += "   if (document.forms[0].enable_comments.checked == true) {" + Environment.NewLine;
            js_ef_focus.Text += "       document.forms[0].moderate_comments.disabled = false;" + Environment.NewLine;
            js_ef_focus.Text += "       document.forms[0].require_authentication.disabled = false;" + Environment.NewLine;
            js_ef_focus.Text += "   } else {" + Environment.NewLine;
            js_ef_focus.Text += "       document.forms[0].moderate_comments.disabled = true;" + Environment.NewLine;
            js_ef_focus.Text += "       document.forms[0].require_authentication.disabled = true;" + Environment.NewLine;
            js_ef_focus.Text += "   }" + Environment.NewLine;
            js_ef_focus.Text += "}" + Environment.NewLine;
        }
        DrawFolderTaxonomyTable();
        DisplaySitemapPath();
        DisplayMetadataInfo();
        DisplaySubscriptionInfo();
        DrawContentTypesTable();
        DrawContentAliasesTable();
        IsContentSearchableSection();
        IsDisplaySettings();
        if (_FolderType == 2) //OrElse folder_data.Id = 0 Avoiding sitealias for root.
        {
            phSiteAlias.Visible = true;
            phSiteAlias2.Visible = true;
            DisplaySiteAlias();
        }
        Showpane();

        if (_FolderData.IsCommunityFolder)
        {
            Display_AddCommunityFolder();
        }
    }

    public string IsPublishedAsPdf()
    {
        return _IsPublishedAsPdf;
    }

    private void IsContentSearchableSection()
    {
        //-------------IscontentSearchable------------- 
        if (parentfolderdata == null)
        {
            parentfolderdata = _ContentApi.GetFolderById(_FolderData.ParentId, false);
        }
        inherit_IscontentSearchable_from.Value = Convert.ToString(parentfolderdata.IsContentSearchableInheritedFrom);
        current_IscontentSearchable.Value = Convert.ToString(Convert.ToInt32(parentfolderdata.IscontentSearchable));
        StringBuilder sb = new StringBuilder();
        //sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _MessageHelper.GetMessage("lbl Content Searchable") + ":</strong></td><td colspan=\"2\">");
        if (_FolderData.IsContentSearchableInherited)
        {
            sb.Append("<input type=\"checkbox\" name=\"chkInheritIscontentSearchable\" id=\"chkInheritIscontentSearchable\" checked=\"checked\"");
            if (_FolderData.Id == 0)
            {
                sb.Append(" disabled=\"disabled\"");
            }
            sb.Append("onclick=\"InheritIscontentSearchableChanged('chkIscontentSearchable')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if (_FolderData.IscontentSearchable)
            {
                sb.Append("&nbsp;      <input checked=\"checked\" ").Append(_FolderData.Id != 0 ? "disabled=\"disabled\"" : "").Append(" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
            }
            else
            {
                sb.Append("&nbsp;      <input type=\"checkbox\"").Append(_FolderData.Id != 0 ? "disabled=\"disabled\"" : "").Append(" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
            }
        }
        else
        {
            sb.Append("<input type=\"checkbox\" name=\"chkInheritIscontentSearchable\" id=\"chkInheritIscontentSearchable\"");
            if (_FolderData.Id == 0)
            {
                sb.Append(" disabled=\"disabled\"");
            }
            sb.Append("onclick=\"InheritIscontentSearchableChanged('chkIscontentSearchable')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if (_FolderData.IscontentSearchable)
            {
                sb.Append("&nbsp;     <input checked=\"checked\" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
            }
        }       
        sb.Append(_MessageHelper.GetMessage("lbl IsContentSearchable required"));
        sb.Append(" <div class=\"ektronCaption\">" + _MessageHelper.GetMessage("Content Searchable warning") + "</div><div class=\"ektronCaption\">*" + _MessageHelper.GetMessage("Content Searchable help") + "</div></td></tr>");
       // sb.Append("</table>");
        ltrContSearch2.Text =  sb.ToString();
        phContSearch2.Visible = true;
        //--------------------IscontentSearchableEnd-------------
    }
    private void IsDisplaySettings()
    {
        //-------------IsDisplaySettings-------------    
        if (parentfolderdata == null)
        {
            parentfolderdata = _ContentApi.GetFolderById(_FolderData.ParentId, false);
        }
        inherit_IsDisplaySettings_from.Value = Convert.ToString(parentfolderdata.DisplaySettingsInheritedFrom);
        current_IsDisplaySettings.Value = Convert.ToString(Convert.ToInt32(parentfolderdata.DisplaySettings));
        StringBuilder sb = new StringBuilder();
        //  sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _MessageHelper.GetMessage("lbl Display Settings") + ":</strong></td><td colspan=\"2\">");
        //sb.Append("<input type=\"checkbox\" id=\"chkInheritIsDisplaySettings\" name=\"chkInheritIsDisplaySettings\" checked=\"checked\" onclick=\"InheritIsDisplaySettingsChanged('chkIsDisplaySettings')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
        //sb.Append("<div class=\"ektronTopSpace\"></div>");
        //sb.Append("</tr></td>");
        //sb.Append(" <tr class=\"evenrow\"><td colspan=\"2\">");
        if (_FolderData.IsDisplaySettingsInherited)
        {          
            sb.Append("<input type=\"checkbox\" name=\"chkInheritIsDisplaySettings\" id=\"chkInheritIsDisplaySettings\" checked=\"checked\"");
            if (_FolderData.Id == 0)
            {
                sb.Append(" disabled=\"disabled\"");
            }
            sb.Append(" onclick=\"InheritIsDisplaySettingsChanged('chkIsDisplaySettingsAllTabs')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if (((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && _FolderData.DisplaySettings ==0)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAllTabs required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSummary required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsMetaData required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAliasing required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSchedule required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsComment required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTemplates required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTaxonomy required"));            
        }
        else
        {
            sb.Append("<input type=\"checkbox\" name=\"chkInheritIsDisplaySettings\" id=\"chkInheritIsDisplaySettings\" ");
            if (_FolderData.Id == 0)
            {
                sb.Append(" disabled=\"disabled\"");
            }
            sb.Append(" onclick=\"InheritIsDisplaySettingsChanged('chkIsDisplaySettingsAllTabs')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if (((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && _FolderData.DisplaySettings == 0)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAllTabs required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSummary required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
            {
                sb.Append("&nbsp;     <input checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsMetaData required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsAliasing required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
            {
                sb.Append("&nbsp;     <input checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsSchedule required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsComment required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTemplates required"));
            sb.Append("<div class=\"ektronTopSpace\"></div>");
            if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
            {
                sb.Append("&nbsp;     <input  checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
            }
            else
            {
                sb.Append("&nbsp;     <input  type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
            }
            sb.Append(_MessageHelper.GetMessage("lbl IsDisplaySettingsTaxonomy required"));  
        }
        sb.Append(" <div class=\"ektronCaption\">" + _MessageHelper.GetMessage("Display Settings warning") + "</div> <div class=\"ektronCaption\">*" + _MessageHelper.GetMessage("Display Settings help") + "</div></td></tr>");
        // sb.Append("</table>");
       ltrDisplaySettings2.Text = sb.ToString();
       phDisplaySettings2.Visible = true;
        //--------------------IsDisplaySettingsEnd-------------
    }
    private long checktaxid = 0;
    private void DrawFolderTaxonomyTable()
    {
        string categorydatatemplate = "<input onclick=\"ValidateCatSel(this)\" type=\"checkbox\" id=\"taxlist\" name=\"taxlist\" value=\"{0}\" {1} {2}/>{3}";
        StringBuilder categorydata = new StringBuilder();
        string catdisabled = "";
        if ((_FolderData.FolderTaxonomy != null) && _FolderData.FolderTaxonomy.Length > 0)
        {
            for (int i = 0; i <= _FolderData.FolderTaxonomy.Length - 1; i++)
            {
                if (_SelectedTaxonomyList.Length > 0)
                {
                    _SelectedTaxonomyList = _SelectedTaxonomyList + "," + _FolderData.FolderTaxonomy[i].TaxonomyId;
                }
                else
                {
                    _SelectedTaxonomyList = _FolderData.FolderTaxonomy[i].TaxonomyId.ToString();
                }
            }
        }
        _CurrentCategoryChecked = Convert.ToInt32(_FolderData.CategoryRequired);
        current_category_required.Value = _CurrentCategoryChecked.ToString();
        inherit_taxonomy_from.Value = _FolderData.TaxonomyInheritedFrom.ToString();
        TaxonomyBaseData[] TaxArr = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content);
        string DisabledMsg = "";
        if (_FolderData.TaxonomyInherited)
        {
            DisabledMsg = " disabled ";
            catdisabled = " disabled ";
        }
        bool parent_has_configuration = false;
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
                        parent_has_configuration = Array.Exists(_FolderData.FolderTaxonomy, new Predicate<TaxonomyBaseData>(TaxonomyExists));
                        categorydata.Append(string.Format(categorydatatemplate, TaxArr[i].TaxonomyId, IsChecked(parent_has_configuration), DisabledMsg, TaxArr[i].TaxonomyName));
                        i++;
                    }
                    else
                    {
                        break;
                    }
                    categorydata.Append("<br/>");
                }
            }
        }

        StringBuilder str = new StringBuilder();

        str.Append("<input type=\"hidden\" id=\"TaxonomyParentHasConfig\" name=\"TaxonomyParentHasConfig\" value=\"");
        if (parent_has_configuration)
        {
            str.Append("1");
        }
        else
        {
            str.Append("0");
        }

        str.Append("\" />");

        DisabledMsg = " ";
        if (_FolderData.Id == 0)
        {
            DisabledMsg = " disabled ";
        }
        else
        {
            DisabledMsg = IsChecked(_FolderData.TaxonomyInherited);
        }
        string catchecked = "";
        if (_FolderData.CategoryRequired)
        {
            catchecked = " checked ";
        }
        if (_FolderData.Id > 0)
        {
            parentfolderdata = _ContentApi.GetFolderById(_FolderData.ParentId, true);
            if ((parentfolderdata.FolderTaxonomy != null) && parentfolderdata.FolderTaxonomy.Length > 0)
            {
                for (int i = 0; i <= parentfolderdata.FolderTaxonomy.Length - 1; i++)
                {
                    if (_SelectedTaxonomyParentList.Length > 0)
                    {
                        _SelectedTaxonomyParentList = _SelectedTaxonomyParentList + "," + parentfolderdata.FolderTaxonomy[i].TaxonomyId;
                    }
                    else
                    {
                        _SelectedTaxonomyParentList = parentfolderdata.FolderTaxonomy[i].TaxonomyId.ToString();
                    }
                }
                _ParentCategoryChecked = Convert.ToInt32(parentfolderdata.CategoryRequired);
                parent_category_required.Value = _ParentCategoryChecked.ToString();
            }
        }

        str.Append("<input name=\"TaxonomyTypeBreak\" id=\"TaxonomyTypeBreak\" type=\"checkbox\" onclick=\"ToggleTaxonomyInherit(this)\" " + DisabledMsg + "/>" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration"));
        str.Append("<br/>");
        str.Append("<input name=\"CategoryRequired\" id=\"CategoryRequired\" type=\"checkbox\"" + catchecked + catdisabled + " />" + _MessageHelper.GetMessage("alt required at least one category selection"));
        str.Append("<br/>");
        str.Append("<br/>");
        str.Append(categorydata.ToString());
        taxonomy_list.Text = str.ToString();
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
    private void Showpane()
    {
        if (_ShowPane.Length > 0)
        {
            lbl_vf_showpane.Text += "<script type=\"text/javascript\">" + Environment.NewLine;
            switch (_ShowPane)
            {
                case "blogroll":
                    lbl_vf_showpane.Text += "   ShowPane(\'dvRoll\');";
                    break;
                case "blogcat":
                    lbl_vf_showpane.Text += "   ShowPane(\'dvCategories\');";
                    break;
            }
            lbl_vf_showpane.Text += "</script>" + Environment.NewLine;
        }
    }
    private void EditFolderToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("edit properties for folder msg") + " \"" + _FolderData.Name + "\""));
        result.Append("<table><tr>");
        bool isBlog = System.Convert.ToBoolean(_FolderType == 1);

		if (_ShowPane.Length > 0 && isBlog)
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewFolder&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (isBlog)
        {
            StringBuilder sbBlogjs = new StringBuilder();
            sbBlogjs.Append(Environment.NewLine);
            sbBlogjs.Append("function CheckBlogForillegalChar() {" + Environment.NewLine);
            sbBlogjs.Append("   var bret = true;" + Environment.NewLine);
            sbBlogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" + Environment.NewLine);
            sbBlogjs.Append("   {" + Environment.NewLine);
            sbBlogjs.Append("       var val = Trim(arrInputValue[j]);" + Environment.NewLine);
            sbBlogjs.Append("       if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
            sbBlogjs.Append("       {" + Environment.NewLine);
            sbBlogjs.Append("           alert(\"" + this._MessageHelper.GetMessage("alert subject name") + " (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')\");" + Environment.NewLine);
            sbBlogjs.Append("           bret = false;" + Environment.NewLine);
            sbBlogjs.Append("       }" + Environment.NewLine);
            sbBlogjs.Append("       else if (val.length == 0) {" + Environment.NewLine);
            sbBlogjs.Append("           alert(\"" + this._MessageHelper.GetMessage("alert blank subject name") + "\");" + Environment.NewLine);
            sbBlogjs.Append("           bret = false;" + Environment.NewLine);
            sbBlogjs.Append("       }" + Environment.NewLine);
            sbBlogjs.Append("   }" + Environment.NewLine);
            sbBlogjs.Append("   if (bret == true) //go on to normal code path" + Environment.NewLine);
            sbBlogjs.Append("   {" + Environment.NewLine);
            sbBlogjs.Append("       bret = CheckFolderParameters(\'edit\');" + Environment.NewLine);
            sbBlogjs.Append("   }" + Environment.NewLine);
            sbBlogjs.Append("   return bret;" + Environment.NewLine);
            sbBlogjs.Append("}" + Environment.NewLine);
            ltr_blog_js.Text = sbBlogjs.ToString();
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (folder)"), _MessageHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckBlogForillegalChar()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (folder)"), _MessageHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckFolderParameters(\\\'edit\\\')\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        switch (_FolderType)
        {
            case (int)Ektron.Cms.Common.EkEnumeration.FolderType.Blog:
                result.Append(_StyleHelper.GetHelpButton("blog_viewfolder", ""));
                break;
            case (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar:
                result.Append(_StyleHelper.GetHelpButton((string)("calendar_" + _PageAction), ""));
                break;
            default:
                result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
                break;
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void DisplayMetadataInfo()
    {
        // Show Custom-Field folder assignments:
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.GetEditableCustomFieldAssignments(_Id, true);
    }
    private void DisplaySubscriptionInfo()
    {
        string strEnabled = " ";
        int i = 0;
        int findindex;
        Array arrSubscribed;
        string strNotifyA = "";
        string strNotifyI = "";
        string strNotifyN = "";
        string strNotifyBase = "";
        long intInheritFrom;
        EmailFromData[] emailfrom_list;
        int y = 0;
        EmailMessageData[] optout_list;
        EmailMessageData[] defaultmessage_list;
        EmailMessageData[] unsubscribe_list;
        SettingsData settings_list;
        SiteAPI m_refSiteAPI = new SiteAPI();
        bool restoreAvailable = true;

        _SubscriptionDataList = _ContentApi.GetAllActiveSubscriptions(); //then get folder
        emailfrom_list = _ContentApi.GetAllEmailFrom();
        optout_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut);
        unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe);
        defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage);
        settings_list = m_refSiteAPI.GetSiteVariables(-1);

        lit_vf_subscription_properties.Text += Environment.NewLine + "<script type=\"text/javascript\">" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function SetMessageContenttoDefault() {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "     document.getElementById(\'use_content_button\').checked = true;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "     document.getElementById(\'frm_content_id\').value = -1; " + Environment.NewLine;
        lit_vf_subscription_properties.Text += "     document.getElementById(\'titlename\').value = \'[[use current]]\'; " + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function  PreviewWebAlert() {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    var contentid, defmsgid, optid, summaryid, unsubid, conttype, usecontlink;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (document.getElementById(\'use_content_button\').checked == true) {;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      contentid = document.getElementById(\'frm_content_id\').value;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      contentid = 0;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (document.getElementById(\'use_message_button\').checked == true) {;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      defmsgid = document.getElementById(\'notify_messageid\').value;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      defmsgid = 0;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    optid = document.getElementById(\'notify_optoutid\').value;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    summaryid = document.getElementById(\'use_summary_button\').checked; " + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    unsubid = document.getElementById(\'notify_unsubscribeid\').value;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    conttype = 0;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (document.getElementById(\'use_contentlink_button\').checked == true) {;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      usecontlink = 1;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "      usecontlink = 0;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    window.open(\'previewwebalert.aspx?content=-1&defmsg=\' + defmsgid + \'&optoutid=\' + optid + \'&summaryid=\' + summaryid + \'&usecontentid=\' + contentid + \'&unsubscribeid=\' + unsubid + \'&content_type=\' + conttype + \'&uselink=\' + usecontlink,\'\',\'menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes\'); " + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;

        lit_vf_subscription_properties.Text += "function CheckBaseNotifyValue(objValue){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   if (objValue == document.getElementById(\'base_notify_option\').value)" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       document.getElementById(\'web_alert_restore_inherit_checkbox\').checked = false;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   else" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       document.getElementById(\'web_alert_restore_inherit_checkbox\').checked = true;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;

        lit_vf_subscription_properties.Text += "function breakWebAlertInheritance(obj){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   if(!obj.checked){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       if(confirm(\"" + _MessageHelper.GetMessage("js: confirm break inheritance") + "\")){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           enableSubCheckboxes(true);" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           obj.checked = !obj.checked;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           return false;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       enableSubCheckboxes(true);" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;

        lit_vf_subscription_properties.Text += "function enableSubCheckboxes(enableFlag) {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    var idx, masterBtn, tableObj, qtyElements, displayUseContentBtns;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    tableObj = document.getElementById(\'therows\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    tableObj = tableObj.getElementsByTagName(\'input\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    masterBtn = document.getElementById(\'web_alert_inherit_checkbox\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (enableFlag && validateObject(masterBtn)){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        enableFlag = !masterBtn.checked;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    displayUseContentBtns = enableFlag ? \'inline\' : \'none\';" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (validateObject(tableObj)){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        qtyElements = tableObj.length;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        for(idx = 0; idx < qtyElements; idx++ ) {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    		    if (tableObj[idx].type == \'checkbox\'){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    			    tableObj[idx].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    		    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.web_alert_restore_inherit_checkbox.checked = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.web_alert_restore_inherit_checkbox.disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[0].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[1].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[2].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_message_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_summary_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_content_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_contentlink_button\').disabled = !enableFlag;" + Environment.NewLine;
        //lit_vf_subscription_properties.Text += ("        document.getElementById('notify_url').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_emailfrom\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_optoutid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_messageid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_unsubscribeid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_subject\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        $ektron(\'.selectContent\').css(\'display\', displayUseContentBtns);" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        $ektron(\'.useCurrent\').css(\'display\', displayUseContentBtns);" + Environment.NewLine;
        //lit_vf_subscription_properties.Text += ("        document.getElementById('notify_weblocation').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function validateObject(obj) {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "     return ((obj != null) &&" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "         ((typeof(obj)).toLowerCase() != \'undefined\') &&" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "         ((typeof(obj)).toLowerCase() != \'null\'))" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function valAndSaveCSubAssignments() {" + Environment.NewLine;
        if ((!(_SubscriptionDataList == null)) && (!((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (settings_list.AsynchronousLocation == ""))))
        {
            lit_vf_subscription_properties.Text += "    var idx, masterBtn, tableObj, enableFlag, qtyElements, retStr;" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    var hidnFld = document.getElementById(\'content_sub_assignments\');" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    hidnFld.value=\'\';" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    tableObj = tableObj = document.getElementById(\'therows\');" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    tableObj = tableObj.getElementsByTagName(\'input\');" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    enableFlag = true;" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    retStr = \'\';" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    if ((validateObject(tableObj)) && enableFlag){" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "        qtyElements = tableObj.length;" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "        for(idx = 0; idx < qtyElements; idx++ ) {" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    		    if ((tableObj[idx].type == \'checkbox\') && tableObj[idx].checked){" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    			    retStr = retStr + tableObj[idx].name + \' \';" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    		    }" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "        }" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
            lit_vf_subscription_properties.Text += "    hidnFld.value = retStr;" + Environment.NewLine;
        }
        lit_vf_subscription_properties.Text += "    return true; // (Note: return false to prevent form submission)" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "</script>" + Environment.NewLine;

        if ((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (_SubscriptionDataList == null) || (settings_list.AsynchronousLocation == ""))
        {
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"suppress_notification\" value=\"true\"/>";
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
            if (_SubscriptionDataList == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("alt No subscriptions are enabled on the folder.") + "</font><br/>";
            }
            if (settings_list.AsynchronousLocation == "")
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + _MessageHelper.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") + "</font>";
            }
            return;
        }

        intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id);
        if (intInheritFrom != _Id) //do we get settings from self
        {
            _GlobalSubInherit = true;
        }
        else
        {
            _GlobalSubInherit = false;
        }
        _SubscribedDataList = _ContentApi.GetSubscriptionsForFolder(intInheritFrom);
        _SubscriptionPropertiesList = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom);

        if (_SubscriptionPropertiesList == null)
        {
            _SubscriptionPropertiesList = new SubscriptionPropertiesData();
        }
        if (_GlobalSubInherit == true)
        {
            if (_Id == 0)
            {
                strEnabled = " ";
            }
            else
            {
                strEnabled = " disabled=\"disabled\" ";
            }
        }
        else
        {
            strEnabled = " ";
        }
        switch (_SubscriptionPropertiesList.NotificationType.GetHashCode())
        {
            case 0:
                strNotifyA = " checked=\"checked\" ";
                strNotifyI = "";
                strNotifyN = "";
                strNotifyBase = "Always";
                break;
            case 1:
                strNotifyA = "";
                strNotifyI = " checked=\"checked\" ";
                strNotifyN = "";
                strNotifyBase = "Initial";
                break;
            case 2:
                strNotifyA = "";
                strNotifyI = "";
                strNotifyN = " checked=\"checked\" ";
                strNotifyBase = "Never";
                break;
        }

        if (_Id == 0) // root folder or not inheriting
        {
            lit_vf_subscription_properties.Text += "<input id=\"web_alert_inherit_checkbox\" type=\"hidden\" name=\"web_alert_inherit_checkbox\" value=\"web_alert_inherit_checkbox\"/>";
        }
        else if (!_GlobalSubInherit)
        {
            lit_vf_subscription_properties.Text += "<input id=\"web_alert_inherit_checkbox\" onclick=\"breakWebAlertInheritance(this);\" type=\"checkbox\" name=\"web_alert_inherit_checkbox\" value=\"web_alert_inherit_checkbox\"/>" + _MessageHelper.GetMessage("lbl inherit parent configuration") + "";
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        }
        else // non root
        {
            lit_vf_subscription_properties.Text += "<input id=\"web_alert_inherit_checkbox\" onclick=\"breakWebAlertInheritance(this);\" type=\"checkbox\" name=\"web_alert_inherit_checkbox\" value=\"web_alert_inherit_checkbox\" checked=\"checked\"/>" + _MessageHelper.GetMessage("lbl inherit parent configuration") + "";
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
            restoreAvailable = false;
        }

        lit_vf_subscription_properties.Text += "&nbsp;&nbsp;<input id=\"web_alert_restore_inherit_checkbox\" type=\"checkbox\" name=\"web_alert_restore_inherit_checkbox\" value=\"web_alert_restore_inherit_checkbox\" " + (!restoreAvailable ? "disabled=\"disabled\"" : "") + "/>" + _MessageHelper.GetMessage("alt restore web alert") + "";
        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";

        lit_vf_subscription_properties.Text += "<table class=\"ektronGrid\">";
        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert opt") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\"><input type=\"hidden\" id=\"base_notify_option\" name=\"base_notify_option\" value=\"" + strNotifyBase + "\"/>";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Always\" name=\"notify_option\" " + strNotifyA + " " + strEnabled + " onclick=\"CheckBaseNotifyValue(this.value);\" />" + _MessageHelper.GetMessage("lbl web alert notify always");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Initial\" name=\"notify_option\"" + strNotifyI + " " + strEnabled + " onclick=\"CheckBaseNotifyValue(this.value);\" />" + _MessageHelper.GetMessage("lbl web alert notify initial");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Never\" name=\"notify_option\"" + strNotifyN + " " + strEnabled + " onclick=\"CheckBaseNotifyValue(this.value);\" />" + _MessageHelper.GetMessage("lbl web alert notify never");
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert subject") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        if (_SubscriptionPropertiesList.Subject != "")
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"" + _SubscriptionPropertiesList.Subject + "\" name=\"notify_subject\" id=\"notify_subject\" " + strEnabled + " onkeypress=\"return CheckKeyValue(event, \'34,13\');\"/>";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"\" name=\"notify_subject\" id=\"notify_subject\"  " + strEnabled + " onkeypress=\"return CheckKeyValue(event, \'34,13\');\"/>";
        }
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        //lit_vf_subscription_properties.Text &= "Notification Base URL:"
        //If subscription_properties_list.URL <> "" Then
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """/>/<br /><br />"
        //Else
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """/>/<br /><br />"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert emailfrom address") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";

        lit_vf_subscription_properties.Text += "<select name=\"notify_emailfrom\" id=\"notify_emailfrom\" " + strEnabled + ">:";
        if ((emailfrom_list != null) && emailfrom_list.Length > 0)
        {
            for (y = 0; y <= emailfrom_list.Length - 1; y++)
            {
                if (emailfrom_list[y].Email == _SubscriptionPropertiesList.EmailFrom)
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
        //lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & "/>/<br /><br />"
        //Else
        //    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" " & strEnabled & "/>/<br /><br />"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl web alert contents") + ":";
        //lit_vf_subscription_properties.Text &= "<br />"
        lit_vf_subscription_properties.Text += "<img src=\"" + _ContentApi.AppPath + "images/UI/Icons/preview.png\" alt=\"Preview Web Alert Message\" title=\"Preview Web Alert Message\" onclick=\" PreviewWebAlert(); return false;\" />";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<input id=\"use_optout_button\" type=\"checkbox\" checked=\"checked\" name=\"use_optout_button\" disabled=\"disabled\"/>" + _MessageHelper.GetMessage("lbl optout message") + "&nbsp;&nbsp;";


        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_optoutid\" id=\"notify_optoutid\">";
        if ((optout_list != null) && optout_list.Length > 0)
        {
            for (y = 0; y <= optout_list.Length - 1; y++)
            {
                if (optout_list[y].Id == _SubscriptionPropertiesList.OptOutID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + optout_list[y].Id + "\" SELECTED>" + optout_list[y].Title + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + optout_list[y].Id + "\">" + optout_list[y].Title + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        if (_SubscriptionPropertiesList.DefaultMessageID > 0)
        {
            lit_vf_subscription_properties.Text += ("<input id=\"use_message_button\" type=\"checkbox\" checked=\"checked\" name=\"use_message_button\" " + strEnabled + " />" + _MessageHelper.GetMessage("lbl use default message")) + "&nbsp;&nbsp;";
        }
        else
        {
            lit_vf_subscription_properties.Text += ("<input id=\"use_message_button\" type=\"checkbox\" name=\"use_message_button\" " + strEnabled + " />" + _MessageHelper.GetMessage("lbl use default message")) + "&nbsp;&nbsp;";
        }


        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_messageid\" id=\"notify_messageid\">";
        if ((defaultmessage_list != null) && defaultmessage_list.Length > 0)
        {
            for (y = 0; y <= defaultmessage_list.Length - 1; y++)
            {
                if (defaultmessage_list[y].Id == _SubscriptionPropertiesList.DefaultMessageID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + defaultmessage_list[y].Id + "\" SELECTED>" + defaultmessage_list[y].Title + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + defaultmessage_list[y].Id + "\">" + defaultmessage_list[y].Title + "</option>";
                }
            }
        }
        lit_vf_subscription_properties.Text += "</select>";

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        if (_SubscriptionPropertiesList.SummaryID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" checked=\"checked\" " + strEnabled + "/>" + _MessageHelper.GetMessage("lbl use summary message");
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" " + strEnabled + "/>" + _MessageHelper.GetMessage("lbl use summary message");
        }
        lit_vf_subscription_properties.Text += "<br />";
        if (_SubscriptionPropertiesList.ContentID == -1)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + "/>" + _MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" value=\"[[use current]]\" " + strEnabled + " size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\"/>" + _MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"-1\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";

        }
        else if (_SubscriptionPropertiesList.ContentID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + "/>" + _MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" value=\"" + _SubscriptionPropertiesList.UseContentTitle.ToString() + "\" " + strEnabled + " size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\">" + _MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"" + _SubscriptionPropertiesList.ContentID.ToString() + "\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";

        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" " + strEnabled + "/>" + _MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" onkeydown=\"return false\" value=\"\" " + strEnabled + " size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\"QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\">" + _MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\"SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"0\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }

        lit_vf_subscription_properties.Text += "<br />";

        if (_SubscriptionPropertiesList.UseContentLink > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" checked=\"checked\" " + strEnabled + "/>Use Content Link";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" " + strEnabled + "/>Use Content Link";
        }

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        lit_vf_subscription_properties.Text += "<input id=\"use_unsubscribe_button\" type=\"checkbox\" checked=\"checked\" name=\"use_unsubscribe_button\" disabled=\"disabled\"/>" + _MessageHelper.GetMessage("lbl unsubscribe message") + "&nbsp;&nbsp;";


        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_unsubscribeid\" id=\"notify_unsubscribeid\">";
        if ((unsubscribe_list != null) && unsubscribe_list.Length > 0)
        {
            for (y = 0; y <= unsubscribe_list.Length - 1; y++)
            {
                if (unsubscribe_list[y].Id == _SubscriptionPropertiesList.UnsubscribeID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + unsubscribe_list[y].Id + "\" selected>" + unsubscribe_list[y].Title + "</option>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + unsubscribe_list[y].Id + "\">" + unsubscribe_list[y].Title + "</option>";
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

        if (!(_SubscriptionDataList == null))
        {
            lit_vf_subscription_properties.Text += "<table class=\"ektronGrid\" id=\"cfld_subscription_assignment\" width=\"100%\">";
            lit_vf_subscription_properties.Text += "<tbody id=\"therows\">";
            lit_vf_subscription_properties.Text += "<tr class=\"title-header\">";
            lit_vf_subscription_properties.Text += "<th width=\"50\">" + _MessageHelper.GetMessage("lbl assigned") + "</th>";
            lit_vf_subscription_properties.Text += "<th>" + _MessageHelper.GetMessage("lbl name") + "</th>";
            lit_vf_subscription_properties.Text += "</tr>";
            if (!(_SubscribedDataList == null))
            {
                arrSubscribed = Array.CreateInstance(typeof(long), _SubscribedDataList.Length);
                for (i = 0; i <= _SubscribedDataList.Length - 1; i++)
                {
                    arrSubscribed.SetValue(_SubscribedDataList[i].Id, i);
                }
                if (arrSubscribed != null)
                {
                    if (arrSubscribed.Length > 0)
                    {
                        Array.Sort(arrSubscribed);
                    }
                }
            }
            else
            {
                arrSubscribed = null;
            }
            i = 0;

            for (i = 0; i <= _SubscriptionDataList.Length - 1; i++)
            {
                findindex = -1;
                if ((_SubscribedDataList != null) && (arrSubscribed != null))
                {
                    findindex = Array.BinarySearch(arrSubscribed, _SubscriptionDataList[i].Id);
                }
                lit_vf_subscription_properties.Text += "<tr>";

                if (findindex < 0)
                {
                    lit_vf_subscription_properties.Text += "<td class=\"center\" width=\"10%\"><input type=\"checkbox\" name=\"WebAlert_" + _SubscriptionDataList[i].Id + "\"  id=\"WebAlert_" + _SubscriptionDataList[i].Id + "\" " + strEnabled + "/></td>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<td class=\"center\" width=\"10%\"><input type=\"checkbox\" name=\"WebAlert_" + _SubscriptionDataList[i].Id + "\"  id=\"WebAlert_" + _SubscriptionDataList[i].Id + "\" checked=\"checked\" " + strEnabled + "/></td>";
                }
                lit_vf_subscription_properties.Text += "<td>" + _SubscriptionDataList[i].Name + "</td>";
                lit_vf_subscription_properties.Text += "</tr>";
            }
            lit_vf_subscription_properties.Text += "</tbody></table>";
        }
        else
        {
            lit_vf_subscription_properties.Text += "Nothing available.";
        }
        lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"content_sub_assignments\" id=\"content_sub_assignments\" value=\"\"/>";
    }
    #endregion

    #region Catalog
    private void Display_EditCatalog()
    {
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        ltrTypes.Text = _MessageHelper.GetMessage("lbl product types");

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
        _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        _FolderType = _FolderData.FolderType;

        EditCatalogToolBar();
        oldfolderdescription.Value = Server.HtmlDecode(_FolderData.Description);
        folderdescription.Value = Server.HtmlDecode(_FolderData.Description);
        folder_id.Value = _FolderData.Id.ToString();

        phFolderProperties1.Visible = true;
        lit_ef_folder.Text = "<input type=\"text\" maxlength=\"100\" size=\"75\" value=\"" + _FolderData.Name + "\" name=\"foldername\"/><input type=\"hidden\" value=\"\" name=\"oldfoldername\" id=\"oldfoldername\" />";
        if ((_FolderData.StyleSheetInherited) && (_FolderData.StyleSheet != ""))
        {
            lit_ef_ss.Text = "" + _ContentApi.SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"stylesheet\"/>";
            lit_ef_ss.Text += "<br />" + _MessageHelper.GetMessage("inherited style sheet msg") + _ContentApi.SitePath + _FolderData.StyleSheet;
        }
        else
        {
            lit_ef_ss.Text = _ContentApi.SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"" + _FolderData.StyleSheet + "\" name=\"stylesheet\"/>";
        }
        lit_ef_templatedata.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\"/>";

        DrawContentTemplatesTable();
        DrawFlaggingOptions();

        Ektron.Cms.Common.EkRequestInformation request_info = _ContentApi.RequestInformationRef;

        // handle dynamic replication settings
        if (request_info.EnableReplication)
        {
            bool bShowReplicationMethod = true;
            if (bShowReplicationMethod)
            {
                string schk = "";
                if (_FolderData.ReplicationMethod == 1)
                {
                    schk = " checked";
                }
                ReplicationMethod.Text = _MessageHelper.GetMessage("lbl folderdynreplication");
                ReplicationMethod.Text += "<input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\"" + schk + " ><label for=\"EnableReplication\"/>" + _MessageHelper.GetMessage("replicate folder contents") + "</label>";
            }
            else
            {
                // if we're not showing it, it means replication is enabled because we're under a parent community folder
                ReplicationMethod.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"1\" />";
            }
        }
        js_ef_focus.Text = "Ektron.ready(function() { document.forms.frmContent.foldername.focus();" + Environment.NewLine;
        js_ef_focus.Text += "   if( $ektron(\'#web_alert_inherit_checkbox\').length > 0 ){" + Environment.NewLine;
        js_ef_focus.Text += "       if( $ektron(\'#web_alert_inherit_checkbox\')[0].checked ){" + Environment.NewLine;
        js_ef_focus.Text += "           $ektron(\'.selectContent\').css(\'display\', \'none\');" + Environment.NewLine;
        js_ef_focus.Text += "           $ektron(\'.useCurrent\').css(\'display\', \'none\');" + Environment.NewLine;
        js_ef_focus.Text += "       } " + Environment.NewLine;
        js_ef_focus.Text += "    } " + Environment.NewLine;
        js_ef_focus.Text += "});" + Environment.NewLine;
        DrawFolderTaxonomyTable();
        DisplaySitemapPath();
        DisplayCatalogMetadataInfo();
        DisplaySubscriptionInfo();
        DrawProductTypesTable();
        DrawContentAliasesTable();
        IsContentSearchableSection();
        IsDisplaySettings();
    }
    private void EditCatalogToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("edit properties for folder msg") + " \"" + _FolderData.Name + "\""));
        result.Append("<table><tr>");
        bool isBlog = System.Convert.ToBoolean(_FolderType == 1);

		if (_ShowPane.Length > 0 && this._FolderType == 1)
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewFolder&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (isBlog)
        {
            StringBuilder sbBlogjs = new StringBuilder();
            sbBlogjs.Append(Environment.NewLine);
            sbBlogjs.Append("function CheckBlogForillegalChar() {" + Environment.NewLine);
            sbBlogjs.Append("   var bret = true;" + Environment.NewLine);
            sbBlogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" + Environment.NewLine);
            sbBlogjs.Append("   {" + Environment.NewLine);
            sbBlogjs.Append("       var val = Trim(arrInputValue[j]);" + Environment.NewLine);
            sbBlogjs.Append("       if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
            sbBlogjs.Append("       {" + Environment.NewLine);
            sbBlogjs.Append("           alert(\"" + this._MessageHelper.GetMessage("alert subject name") + " (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')\");" + Environment.NewLine);
            sbBlogjs.Append("           bret = false;" + Environment.NewLine);
            sbBlogjs.Append("       }" + Environment.NewLine);
            sbBlogjs.Append("       else if (val.length == 0) {" + Environment.NewLine);
            sbBlogjs.Append("           alert(\"" + this._MessageHelper.GetMessage("alert blank subject name") + "\");" + Environment.NewLine);
            sbBlogjs.Append("           bret = false;" + Environment.NewLine);
            sbBlogjs.Append("       }" + Environment.NewLine);
            sbBlogjs.Append("   }" + Environment.NewLine);
            sbBlogjs.Append("   if (bret == true) //go on to normal code path" + Environment.NewLine);
            sbBlogjs.Append("   {" + Environment.NewLine);
            sbBlogjs.Append("       bret = CheckFolderParameters(\'edit\');" + Environment.NewLine);
            sbBlogjs.Append("   }" + Environment.NewLine);
            sbBlogjs.Append("   return bret;" + Environment.NewLine);
            sbBlogjs.Append("}" + Environment.NewLine);
            ltr_blog_js.Text = sbBlogjs.ToString();
            result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (catalog)"), _MessageHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckBlogForillegalChar()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (catalog)"), _MessageHelper.GetMessage("btn update"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckFolderParameters(\\\'edit\\\')\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("catalogedit", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void DisplayCatalogMetadataInfo()
    {
        // Show Custom-Field folder assignments:
        lit_vf_customfieldassingments.Text = "<br/>" + _CustomFieldsApi.GetEditableCustomFieldAssignments(_Id, true, Ektron.Cms.Common.EkEnumeration.FolderType.Catalog);
    }
    private string DrawProductTypesBreaker(bool @checked, bool IsParentCatalog)
    {
        if (!IsParentCatalog)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" disabled autocomplete=\'off\' />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" checked autocomplete=\'off\' />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" autocomplete=\'off\' />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }

    }

    private string DrawProductTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table width=\"100%\" class=\"ektronGrid\"><tbody id=\"contentTypeTable\">");
        str.Append("    <tr class=\"title-header\">");
        str.Append("        <td width=\"10%\" class=\"center\">");
        str.Append(_MessageHelper.GetMessage("lbl default"));
        str.Append("        </td>");
        str.Append("        <td width=\"70%\">");
        str.Append(_MessageHelper.GetMessage("lbl prod type"));
        str.Append("        </td>");
        str.Append("        <td width=\"10%\" class=\"center\" colspan=\"2\">");
        str.Append(_MessageHelper.GetMessage("lbl action"));
        str.Append("        </td>");
        str.Append("    </tr>");
        return str.ToString();
    }

    private string DrawProductTypesEntry(int row_id, string name, long xml_id, bool isDefault, bool isEnabled)
    {
        StringBuilder str = new StringBuilder();
        str.Append("<tr id=\"row_" + xml_id + "\">");
        str.Append("<td class=\"center\" width=\"10%\">");
        if (this._FolderData.Id == 0)
        {
            isEnabled = true;
        }
        if (isDefault && isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked />");
        }
        else if (isDefault && !isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked disabled />");
        }
        else if (!isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" />");
        }
        str.Append("</td>");
        str.Append("<td width=\"70%\">");
        str.Append(name + "<input id=\"input_" + xml_id + "\" name=\"input_" + xml_id + "\" type=\"hidden\" value=\"" + xml_id + "\" /></td>");
        if (xml_id != 0)
        {
            str.Append("<td class=\"center\" width=\"10%\"><span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' ><a class=\"button redHover minHeight buttonSearch\" href=\"javascript:PreviewProductTypeByID(" + xml_id + ")\">" + _MessageHelper.GetMessage("lbl view") + "</a></span></td>");
        }
        else
        {
            str.Append("<td class=\"center\" width=\"10%\">&nbsp;</td>");
        }

        str.Append("<td class=\"right\" width=\"10%\"><span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' ><a class=\"button redHover minHeight buttonRemove\" href=\"javascript:RemoveContentType(" + xml_id + ", \'" + name + "\')\">" + _MessageHelper.GetMessage("btn remove") + "</a></span></td>");
        str.Append("</tr>");

        return str.ToString();
    }
    private void DrawProductTypesTable()
    {
        _ProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);

        List<Ektron.Cms.Commerce.ProductTypeData> prod_type_list = new List<Ektron.Cms.Commerce.ProductTypeData>();
        Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();

        criteria.PagingInfo = new PagingInfo(10000);
        prod_type_list = _ProductType.GetList(criteria);

        List<Ektron.Cms.Commerce.ProductTypeData> active_prod_list = new List<Ektron.Cms.Commerce.ProductTypeData>();
        active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id);
        Collection addNew = new Collection();
        int k = 0;
        int row_id = 0;

        bool smartFormsRequired = true;

        //bool broken = false;
        //if (active_prod_list.Count > 0)
        //{
        //    broken = true;
        //}

        bool isParentCatalog = System.Convert.ToBoolean(_ContentApi.EkContentRef.GetFolderType(_FolderData.ParentId) == Ektron.Cms.Common.EkEnumeration.FolderType.Catalog);
        bool isInheriting = System.Convert.ToBoolean(isParentCatalog && IsInheritingXmlMultiConfig()); // folder_data.XmlInherited
        bool isEnabled = System.Convert.ToBoolean(!isInheriting);

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawProductTypesBreaker(isInheriting, isParentCatalog));
        str.Append("<div class=\"ektronTopSpace\"></div>");

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
                str.Append(DrawProductTypesEntry(row_id, prod_type_list[k].Title, prod_type_list[k].Id, Utilities.IsDefaultXmlConfig(prod_type_list[k].Id, active_prod_list.ToArray()), isEnabled));
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
            str.Append(DrawProductTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_prod_list.ToArray()), isEnabled));
        }

        str.Append("</tbody></table>");
        str.Append("<table width=\"100%\"><tbody>");
        str.Append("<tr><td width=\"90%\">");
        str.Append("<br /><select name=\"addContentType\" id=\"addContentType\" " + (isEnabled || _FolderData.Id == 0 ? "" : " disabled ") + ">");

        str.Append("<option value=\"-1\">" + "[" + _MessageHelper.GetMessage("lbl select prod type") + "]" + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["xml_id"] + "\">" + row["xml_name"] + "</option>");
        }

        str.Append("</select>");
        str.Append("<span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline; padding: .25em;" : "none;") + "\' ><a href=\"#\" onclick=\"PreviewSelectedProductType(\'" + _ContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + _ContentApi.AppPath + "images/UI/Icons/preview.png" + "\" alt=" + _MessageHelper.GetMessage("lbl Preview Smart Form") + " title=" + _MessageHelper.GetMessage("lbl Preview Smart Form") + "></a></span>");
        str.Append("<span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline; padding: .25em;" : "none;") + "\' ><a href=\"#\" onclick=\"ActivateContentType(true);\"><img src=\"" + _ContentApi.AppPath + "images/ui/icons/Add.png" + "\" title=" + _MessageHelper.GetMessage("lbl add link") + " alt=" + _MessageHelper.GetMessage("lbl add link") + "/></a></span></td></tr>");
        str.Append(DrawContentTypesFooter());
        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }
        if (_FolderData.Id == 0)
        {
            isEnabled = true;
        }
        str.Append("<div style=\'display:none;\'>");
        if (smartFormsRequired && isEnabled)
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" checked=\"checked\" />");
        }
        else if (!smartFormsRequired && isEnabled)
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" />");
        }
        else if (smartFormsRequired && !isEnabled)
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" checked=\"checked\" disabled=\"disabled\" />");
        }
        else
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" disabled=\"disabled\" />");
        }

        str.Append(_MessageHelper.GetMessage("lbl Require Smart Forms"));
        str.Append("</div>");
        ltr_vf_types.Text = str.ToString();
    }
    #endregion

    #region content type selection

    private string DrawContentTypesBreaker(bool @checked)
    {
        if (_FolderData.Id == 0)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl inherit parent configuration");
        }
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" checked />" + _MessageHelper.GetMessage("lbl inherit parent configuration");
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" />" + _MessageHelper.GetMessage("lbl inherit parent configuration");
        }

    }

    private string DrawContentTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table width=\"100%\" class=\"ektronGrid\"><tbody id=\"contentTypeTable\">");
        str.Append("<tr class=\"title-header\">");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append(_MessageHelper.GetMessage("lbl default"));
        str.Append("</td>");
        str.Append("<td width=\"70%\">");
        str.Append(_MessageHelper.GetMessage("lbl Smart Form"));
        str.Append("</td>");
        str.Append("<td width=\"20%\" class=\"center\" colspan=\"2\">");
        str.Append(_MessageHelper.GetMessage("lbl action"));
        str.Append("</td>");
        str.Append("</tr>");
        return str.ToString();
    }

    private string DrawContentTypesEntry(int row_id, string name, long xml_id, bool isDefault, bool isEnabled)
    {
        StringBuilder str = new StringBuilder();
        str.Append("<tr id=\"row_" + xml_id + "\">");
        str.Append("<td class=\"center\" width=\"10%\">");
        if (this._FolderData.Id == 0)
        {
            isEnabled = true;
        }
        if (isDefault && isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked />");
        }
        else if (isDefault && !isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" checked disabled />");
        }
        else if (!isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"sfdefault\" name=\"sfdefault\" value=\"" + xml_id + "\" />");
        }
        str.Append("</td>");
        str.Append("<td width=\"70%\">");
        str.Append(name + "<input id=\"input_" + xml_id + "\" name=\"input_" + xml_id + "\" type=\"hidden\" value=\"" + xml_id + "\" />");
        str.Append("</td>");
        if (xml_id != 0)
        {
            str.Append("<td class=\"center\" width=\"10%\"><a class=\"button greenHover minHeight buttonSearch\" href=\"javascript:PreviewXmlConfigByID(" + xml_id + ")\">View</a></td>");
        }
        else
        {
            str.Append("<td class=\"center\" width=\"10%\">&nbsp;</td>");
        }

        str.Append("<td class=\"center\" width=\"10%\">");
        str.Append("<a class=\"button redHover minHeight buttonRemove\" href=\"javascript:RemoveContentType(" + xml_id + ", \'" + name + "\')\">" + _MessageHelper.GetMessage("btn remove"));
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
        if (_FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            ltr_vf_types.Text = "";
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

        bool isEnabled = System.Convert.ToBoolean(!IsInheritingXmlMultiConfig());

        //bool broken = false;
        //if (active_xml_list.Length > 0)
        //{
        //    broken = true;
        //}

        bool isInheriting = System.Convert.ToBoolean(!isEnabled);

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawContentTypesBreaker(isInheriting));
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

                str.Append(DrawContentTypesEntry(row_id, xml_config_list[k].Title, xml_config_list[k].Id, Utilities.IsDefaultXmlConfig(xml_config_list[k].Id, active_xml_list), isEnabled));
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
            str.Append(DrawContentTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_xml_list), isEnabled));
        }

        str.Append(DrawContentTypesFooter());
        str.Append("</div>");

        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<table><tbody>");
        str.Append("<tr>");
        str.Append("<td>");
        if ((!isInheriting) || _FolderData.Id == 0)
        {
            str.Append("<select name=\"addContentType\" id=\"addContentType\">");
        }
        else
        {
            str.Append("<select name=\"addContentType\" id=\"addContentType\" disabled>");
        }

        str.Append("<option value=\"-1\">" + _MessageHelper.GetMessage("select smart form") + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["xml_id"] + "\">" + row["xml_name"] + "</option>");
        }

        str.Append("</select>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"#\" onclick=\"PreviewSelectedXmlConfig(\'" + _ContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + _ContentApi.AppPath + "images/UI/Icons/preview.png" + "\" alt=" + _MessageHelper.GetMessage("lbl Preview Smart Form") + " title=" + _MessageHelper.GetMessage("lbl Preview Smart Form") + "></a>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"#\" onclick=\"ActivateContentType();\"><img src=\"" + _ContentApi.AppPath + "images/ui/icons/add.png" + "\" title=" + _MessageHelper.GetMessage("btn add") + " alt=" + _MessageHelper.GetMessage("btn add") + "/></a>");
        str.Append("</td>");
        str.Append("</tr>");
        str.Append("</tbody></table>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }
        if (_FolderData.Id == 0)
        {
            isEnabled = true;
        }

        str.Append("<div class=\"ektronTopSpace\"></div>");

        if (smartFormsRequired && isEnabled)
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" checked=\"checked\" />");
        }
        else if (!smartFormsRequired && isEnabled)
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" />");
        }
        else if (smartFormsRequired && !isEnabled)
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" checked=\"checked\" disabled=\"disabled\" />");
        }
        else
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onClick=\"ToggleRequireSmartForms()\" disabled=\"disabled\" />");
        }

        str.Append(_MessageHelper.GetMessage("lbl Require Smart Forms"));
        str.Append("</div>");
        ltr_vf_types.Text = str.ToString();
    }

    #endregion

    #region flagging section
    private void DrawFlaggingOptions()
    {
        //Dim str As New StringBuilder()

        //Try
        //          str.Append(m_refMsg.GetMessage("lbl flagging inherit parent config") & ": <input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged()"" />" + Environment.NewLine)
        //	str.Append("<input type=""hidden"" id=""flagging_options_inherit_hf"" value=""" + IIf(folder_data.FlagInherited, "True", "False") + """ />" + Environment.NewLine)
        //	str.Append("<table width=""100%"" >" + Environment.NewLine)
        //	str.Append("  <tr>" + Environment.NewLine)
        //	str.Append("    <td>" + Environment.NewLine)
        //	str.Append("      <table cellspacing=""4"" cellpadding=""0"" width=""100%"">" + Environment.NewLine)
        //	str.Append("        <tr>" + Environment.NewLine)
        //	str.Append("          <td>" + Environment.NewLine)
        //	str.Append("            <table class=""center"" cellspacing=""0"" cellpadding=""0"" width=""100%"">" + Environment.NewLine)
        //	str.Append("              <tr>" + Environment.NewLine)
        //	str.Append("                <td width=""50%"">" + Environment.NewLine)
        //	str.Append("                  <table width=""100%"">" + Environment.NewLine)
        //	str.Append("                    <tr>" + Environment.NewLine)
        //	str.Append("                      <td width=""45%"">" + Environment.NewLine)
        //          str.Append("                        " & m_refMsg.GetMessage("lbl assigned flags") & ": " + Environment.NewLine)
        //	str.Append("                        <select name=""flagging_options_assigned"" id=""flagging_options_assigned"" multiple=""multiple""" + Environment.NewLine)
        //	str.Append("                           " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " size=""4"" style=""width: 100%"">" + Environment.NewLine)
        //	'
        //	' Generate an option for each assigned flag:
        //	str.Append(GetAssignedFlags(True) + Environment.NewLine)
        //	str.Append("                        </select>" + Environment.NewLine)
        //	str.Append("                      </td>" + Environment.NewLine)
        //	str.Append("                      <td class=""center"">" + Environment.NewLine)
        //	str.Append("                        <table cellspacing=""0"" cellpadding=""5"">" + Environment.NewLine)
        //	str.Append("                          <tr>" + Environment.NewLine)
        //	str.Append("                            <td>" + Environment.NewLine)
        //	str.Append("                              &nbsp;" + Environment.NewLine)
        //	str.Append("                            </td>" + Environment.NewLine)
        //	str.Append("                          </tr>" + Environment.NewLine)
        //	str.Append("                          <tr>" + Environment.NewLine)
        //	str.Append("                            <td class=""center"">" + Environment.NewLine)
        //	str.Append("                              <input type=""button"" id=""flagging_options_moveLeftBtn"" onclick=""moveFlagsLeft();"" value="" &lt; "" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        //	'str.Append("                            </td>" + Environment.NewLine)
        //	'str.Append("                          </tr>" + Environment.NewLine)
        //	'str.Append("                          <tr>" + Environment.NewLine)
        //	'str.Append("                            <td class=""center"">" + Environment.NewLine)
        //	str.Append("                              <input type=""button"" id=""flagging_options_moveRighBtn"" onclick=""moveFlagsRight();"" value="" &gt; "" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        //	str.Append("                            </td>" + Environment.NewLine)
        //	str.Append("                          </tr>" + Environment.NewLine)
        //	str.Append("                          <tr>" + Environment.NewLine)
        //	str.Append("                            <td class=""center"">" + Environment.NewLine)
        //	str.Append("                              <input type=""button"" id=""flagging_options_setDefaultBtn"" onclick=""setDefaultFlag();"" value=""Default"" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        //	str.Append("                            </td>" + Environment.NewLine)
        //	str.Append("                          </tr>" + Environment.NewLine)
        //	str.Append("                        </table>" + Environment.NewLine)
        //	str.Append("                      </td>" + Environment.NewLine)
        //	str.Append("                      <td width=""45%"">" + Environment.NewLine)
        //          str.Append("                        " & m_refMsg.GetMessage("lbl avail flags") & ": " + Environment.NewLine)
        //	str.Append("                        <select name=""flagging_options_available"" id=""flagging_options_available"" multiple=""multiple""" + Environment.NewLine)
        //	str.Append("                          " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " size=""4"" style=""width: 100%"">" + Environment.NewLine)
        //	'
        //	' Generate an option for each un-assigned flag:
        //	str.Append(GetUnassignedFlags() + Environment.NewLine)
        //	str.Append("                        </select>" + Environment.NewLine)
        //	str.Append("                      </td>" + Environment.NewLine)
        //	str.Append("                    </tr>" + Environment.NewLine)
        //	str.Append("                  </table>" + Environment.NewLine)
        //	str.Append("                </td>" + Environment.NewLine)
        //	str.Append("              </tr>" + Environment.NewLine)
        //	str.Append("            </table>" + Environment.NewLine)
        //	str.Append("          </td>" + Environment.NewLine)
        //	str.Append("        </tr>" + Environment.NewLine)
        //	str.Append("      </table>" + Environment.NewLine)
        //	str.Append("    </td>" + Environment.NewLine)
        //	str.Append("  </tr>" + Environment.NewLine)
        //	'
        //	' Store currently assigned flags in a hidden field:
        //	str.Append("  <input type=""hidden"" name=""flagging_options_hdn"" id=""flagging_options_hdn"" value=""" + GetFolderFlags() + """ />" + Environment.NewLine)
        //	str.Append("  <input type=""hidden"" name=""flagging_options_default_hdn"" id=""flagging_options_default_hdn"" value=""" + GetDefaultFolderFlag() + """ />" + Environment.NewLine)
        //	str.Append("</table>" + Environment.NewLine)

        //	flagging_options.Text = str.ToString

        //Catch ex As Exception
        //Finally
        //	str = Nothing
        //End Try
        //ddflags
        //inheritFlag.Text = m_refMsg.GetMessage("lbl flagging inherit parent config") & ": <input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged('" & ddflags.ClientID & "')"" />"
        inheritFlag.Text = "<input type=\"checkbox\" id=\"flagging_options_inherit_cbx\" name=\"flagging_options_inherit_cbx\" " + ((_FolderData.Id == 0) ? "disabled=\"disabled\" " : "") + ((_FolderData.FlagInherited && (!(_FolderData.Id == 0))) ? "checked=\"checked\" " : "") + " onclick=\"InheritFlagingChanged(\'" + ddflags.ClientID + "\')\" />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration") + ""; //Fix for defect #29031
        ddflags.Items.Add(new ListItem(" "+_MessageHelper.GetMessage("lbl None")+" ", "0"));
        ddflags.Items.FindByValue("0").Selected = true;
        FolderFlagDefData flag_fdata = (new Ektron.Cms.Community.FlaggingAPI()).GetDefaultFolderFlagDef(_FolderData.Id);
        FlagDefData[] flag_data = _ContentApi.EkContentRef.GetAllFlaggingDefinitions(false);
        if ((flag_data != null) && flag_data.Length > 0)
        {
            for (int i = 0; i <= flag_data.Length - 1; i++)
            {
                ddflags.Items.Add(new ListItem(flag_data[i].Name, flag_data[i].ID.ToString()));
                if ((flag_fdata != null) && flag_fdata.ID == flag_data[i].ID)
                {
                    ddflags.Items.FindByValue(flag_data[i].ID.ToString()).Selected = true;
                    ddflags.SelectedIndex = ddflags.Items.IndexOf(ddflags.Items.FindByValue(flag_data[i].ID.ToString()));
                }
            }
        }
        if (_FolderData.FlagInherited)
        {
            ddflags.Enabled = false;
        }
    }

    //Protected Function GetFolderFlags() As String
    //	Dim result As String = ""
    //	Dim flags() As FolderFlagDefData
    //	Dim flag As FolderFlagDefData

    //	Try
    //		flags = folder_data.FolderFlags
    //		For Each flag In flags
    //			If result.Length > 0 Then
    //				result += ","
    //			End If
    //			result += flag.ID.ToString
    //		Next

    //	Catch ex As Exception
    //	Finally
    //		GetFolderFlags = result
    //	End Try
    //End Function

    //Protected Function GetDefaultFolderFlag() As String
    //	Dim result As String = ""
    //	Dim flags() As FolderFlagDefData
    //	Dim flag As FolderFlagDefData

    //	Try
    //		flags = folder_data.FolderFlags
    //		For Each flag In flags
    //			If (flag.IsDefault) Then
    //				result = flag.ID.ToString
    //			End If
    //		Next

    //	Catch ex As Exception
    //	Finally
    //		GetDefaultFolderFlag = result
    //	End Try
    //End Function

    //Protected Function GetAssignedFlags(Optional ByVal showDefault As Boolean = False) As String
    //	Dim result As New StringBuilder()
    //	Dim flags() As FolderFlagDefData
    //	Dim flag As FolderFlagDefData

    //	Try
    //		flags = folder_data.FolderFlags	' flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)

    //		For Each flag In flags
    //			If (showDefault AndAlso (flag.IsDefault)) Then
    //				result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + " (default)" + "</option>" + Environment.NewLine)
    //			Else
    //				result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + "</option>" + Environment.NewLine)
    //			End If
    //			_assignedFlags.Add(flag.Name, flag.Name)
    //		Next

    //	Catch ex As Exception
    //	Finally
    //		GetAssignedFlags = result.ToString
    //		result = Nothing
    //	End Try
    //End Function

    //Protected Function GetUnassignedFlags() As String
    //	Dim result As New StringBuilder()

    //	Try
    //		Dim aFlagSets() As FlagDefData = Array.CreateInstance(GetType(Ektron.Cms.FlagDefData), 0)
    //		aFlagSets = Me.m_refContentApi.EkContentRef.GetAllFlaggingDefinitions(False)
    //		Dim aFlagSet As FlagDefData
    //		For Each aFlagSet In aFlagSets
    //			If (Not _assignedFlags.Contains(aFlagSet.Name)) Then
    //				result.Append("                          <option value=""" + aFlagSet.ID.ToString + """>" + aFlagSet.Name + "</option>" + Environment.NewLine)
    //			End If
    //		Next

    //	Catch ex As Exception
    //	Finally
    //		GetUnassignedFlags = result.ToString
    //		result = Nothing
    //	End Try
    //End Function

    private void ProcessFlaggingPostBack(Collection pageCol)
    {
        bool inheritParentConfig = false;

        try
        {
            if (!string.IsNullOrEmpty(Request.Form["flagging_options_inherit_cbx"]))
            {
                if (Request.Form["flagging_options_inherit_cbx"].ToLower() == "on") { inheritParentConfig = true; }
                //inheritParentConfig = bool.Parse("on" == Request.Form["flagging_options_inherit_cbx"].ToLower());
            }

            // Update settings to db:
            _PageData.Add(inheritParentConfig ? true : false, "InheritFlag", null, null);
            if (!inheritParentConfig)
            {
                _PageData.Add(0, "InheritFlagFrom", null, null);
                if (Request.Form[ddflags.UniqueID] != null)
                {
                    _PageData.Add(Request.Form[ddflags.UniqueID], "DefaultFlagId", null, null);
                }
                else
                {
                    _PageData.Add(0, "DefaultFlagId", null, null); // TODO: Check, should we leave this key non-existant when no default is known
                }
                if ((!inheritParentConfig) && (Request.Form[ddflags.UniqueID] != null))
                {
                    _PageData.Add(Request.Form[ddflags.UniqueID], "FlagList", null, null);
                }
            }

        }
        catch (Exception)
        {
        }
        finally
        {
        }

    }
    #endregion

    #region multi-template selection
    private string DrawContentTemplatesBreaker(bool @checked)
    {
        if (_IsUserBlog)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration") + "<input type=\"hidden\" id=\"userblog\" name=\"userblog\" value=\"1\"/>";
        }
        else if (_FolderData.Id == 0)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" disabled />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else if (@checked)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" checked />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" />" + _MessageHelper.GetMessage("lbl Inherit Parent Configuration");
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
        str.Append("<td width=\"70%\">");
        str.Append(_MessageHelper.GetMessage("lbl Page Template Name"));
        str.Append("</td>");
        str.Append("<td width=\"10%\" class=\"center\" colspan=\"2\">");
        str.Append(_MessageHelper.GetMessage("lbl Action"));
        str.Append("</td>");
        //str.Append("<td width=""10%"" class=""center"">")
        //str.Append("&nbsp;")
        //str.Append("</td>")
        str.Append("</tr>");
        return str.ToString();
    }

    private string DrawContentTemplatesEntry(int row_id, string name, string typestring, long template_id, bool isEnabled, TemplateData templatedata)
    {
        StringBuilder str = new StringBuilder();
        bool isDefault = false;

        if (template_id == _FolderData.TemplateId)
        {
            isDefault = true;
        }

        str.Append("<tr id=\"trow_" + template_id + "\">");

        str.Append("<td width=\"10%\" class=\"center\">");
        if (isDefault && isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" checked />");
        }
        else if (isDefault && !isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" checked disabled />");
        }
        else if (!isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" disabled />");
        }
        else
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" />");
        }

        str.Append("</td>");
        str.Append("<td width=\"70%\">");
        str.Append(name + typestring + "<input id=\"tinput_" + template_id + "\" name=\"tinput_" + template_id + "\" type=\"hidden\" value=\"" + template_id + "\" /></td>");
        string link = "";
        if (templatedata.SubType == EkEnumeration.TemplateSubType.MasterLayout)
        {
            link = _ContentApi.EkContentRef.GetContentQlink(templatedata.MasterLayoutID, _ContentApi.GetFolderIdForContentId(templatedata.MasterLayoutID));
            str.Append("<td class=\"center\" width=\"10%\"><a href=\"#\" class=\"button greenHover minHeight buttonSearch\" onclick=\"PreviewSpecificTemplate(\'" + link + "\', 800,600)\">" + _MessageHelper.GetMessage("lbl View") + "</a></td>");
        }
        else
        {
            str.Append("<td class=\"center\" width=\"10%\"><a href=\"#\" class=\"button greenHover minHeight buttonSearch\" onclick=\"PreviewSpecificTemplate(\'" + _ContentApi.SitePath + name + "\', 800,600)\">" + _MessageHelper.GetMessage("lbl View") + "</a></td>");
        }
        str.Append("<td class=\"center\" width=\"10%\"><a href=\"#\" class=\"button redHover minHeight buttonRemove\"  onclick=\"RemoveTemplate(" + template_id + ", \'" + name.Replace("\\", "\\\\") + "\', \'" + link + "\')\">" + _MessageHelper.GetMessage("btn remove") + "</td>");
        str.Append("</tr>");

        return str.ToString();
    }

    private string DrawContentTemplatesFooter()
    {
        return "</tbody></table>";
    }
    private void DrawContentAliasesTable()
    {
        bool _isManualAliasEnabled = true;
        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _aliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
            if (_aliasSettings.IsManualAliasEnabled)
            {
                if (parentfolderdata == null)
                {
                    parentfolderdata = _ContentApi.GetFolderById(_FolderData.ParentId, false);
                }
                inherit_alias_from.Value = Convert.ToString(parentfolderdata.AliasInheritedFrom);
                current_alias_required.Value = Convert.ToString(Convert.ToInt32(parentfolderdata.AliasRequired));
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                sb2.Append("<div class=\"ektronTopSpace\"></div>");
                sb2.Append("<table class=\"ektronForm\">");
                sb2.Append(" <tr class=\"evenrow\"><td>");
                if (_FolderData.IsAliasInherited)
                {
                    sb.Append("<input type=\"checkbox\" name=\"chkInheritAliases\" id=\"chkInheritAliases\" checked=\"checked\"");
                    if (_FolderData.Id == 0)
                    {
                        sb.Append(" disabled=\"disabled\"");
                    }
                    sb.Append("onclick=\"InheritAliasedChanged('chkForceAliasing')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
                    if (_FolderData.AliasRequired)
                    {
                        sb2.Append("     <input checked=\"checked\" disabled=\"disabled\" type=\"checkbox\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                    }
                    else
                    {
                        sb2.Append("     <input type=\"checkbox\" disabled=\"disabled\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                    }
                }
                else
                {
                    sb.Append("<input type=\"checkbox\" name=\"chkInheritAliases\" id=\"chkInheritAliases\"");
                    if (_FolderData.Id == 0)
                    {
                        sb.Append(" disabled=\"disabled\"");
                    }
                    sb.Append("onclick=\"InheritAliasedChanged('chkForceAliasing')\" /> " + _MessageHelper.GetMessage("lbl inherit parent configuration"));
                    if (_FolderData.AliasRequired)
                    {
                        sb2.Append("     <input checked=\"checked\" type=\"checkbox\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                    }
                    else
                    {
                        sb2.Append("     <input type=\"checkbox\" name=\"chkForceAliasing\" id=\"chkForceAliasing\" />");
                    }
                }
                sb2.Append(_MessageHelper.GetMessage("lbl manual alias required"));
                sb2.Append(" </td></tr>");
                sb2.Append("</table>");


                ltrFolderAliases2.Text = sb.ToString() + sb2.ToString();
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
    private bool IsInheritingTemplateMultiConfig()
    {
        bool isInheriting = false;
        if (_IsUserBlog)
        {
            isInheriting = false;
        }
        else
        {
            isInheriting = _ContentApi.IsInheritingTemplateMultiConfig(_FolderData.Id);
        }
        return isInheriting;
    }

    private bool IsInheritingXmlMultiConfig()
    {
        bool isInheriting = _ContentApi.IsInheritingXmlMultiConfig(_FolderData.Id);
        return isInheriting;
    }

    private void DrawContentTemplatesTable()
    {
        TemplateData[] active_templates;
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id);

        TemplateData[] template_data;
        if (_IsUserBlog)
        {
            template_data = _ContentApi.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.User);
        }
        else
        {
            template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        }

        Ektron.Cms.PageBuilder.TemplateModel tmodel = new Ektron.Cms.PageBuilder.TemplateModel();
        for (int i = 0; i <= template_data.Length - 1; i++)
        {
            if (template_data[i].SubType == EkEnumeration.TemplateSubType.MasterLayout)
            {
                template_data[i] = tmodel.FindByID(template_data[i].Id);
            }
        }

        int k = 0;
        int row_id = 0;
        Collection addNew = new Collection();

        //bool broken = false;
        //if (active_templates.Length > 0)
        //{
        //    broken = true;
        //}
        //
        //bool foundDefault = false;
        //for (k = 0; k <= active_templates.Length - 1; k++)
        //{
        //    if (active_templates[k].Id == _FolderData.TemplateId)
        //    {
        //        foundDefault = true;
        //    }
        //}

        bool isInheriting = IsInheritingTemplateMultiConfig();

        //If (Not foundDefault) Then
        //    isInheriting = False
        //End If

        StringBuilder str = new StringBuilder();

        str.Append(DrawContentTemplatesBreaker(isInheriting));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<div>");
        str.Append(DrawContentTemplatesHeader());

        if (_FolderData.Id == 0)
        {
            isInheriting = false;
        }

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
                string typestring = "";
                if (template_data[k].SubType == EkEnumeration.TemplateSubType.Wireframes)
                {
                    typestring = " (" + _MessageHelper.GetMessage("lbl pagebuilder wireframe template") + ")";
                }
                else if (template_data[k].SubType == EkEnumeration.TemplateSubType.MasterLayout)
                {
                    typestring = " (" + _MessageHelper.GetMessage("lbl pagebuilder master layouts") + ")";
                }
                str.Append(DrawContentTemplatesEntry(row_id, template_data[k].FileName, typestring, template_data[k].Id, System.Convert.ToBoolean(!isInheriting), template_data[k]));
                row_id++;
            }
            else
            {
                Collection cRow = new Collection();
                string type;
                if (template_data[k].SubType == EkEnumeration.TemplateSubType.Wireframes)
                {
                    type = " (" + _MessageHelper.GetMessage("lbl pagebuilder wireframe template") + ")";
                }
                else if (template_data[k].SubType == EkEnumeration.TemplateSubType.MasterLayout)
                {
                    type = " (" + _MessageHelper.GetMessage("lbl pagebuilder master layouts") + ")";
                }
                else
                {
                    type = "";
                }
                cRow.Add(type, "template_type", null, null);
                cRow.Add(template_data[k].FileName, "template_name", null, null);
                cRow.Add(template_data[k].Id, "template_id", null, null);
                string url = "";
                if (template_data[k].SubType == EkEnumeration.TemplateSubType.MasterLayout)
                {
                    url = _ContentApi.EkContentRef.GetContentQlink(template_data[k].MasterLayoutID, _ContentApi.GetFolderIdForContentId(template_data[k].MasterLayoutID));
                }
                cRow.Add(url, "url", null, null);
                addNew.Add(cRow, null, null, null);
            }
        }

        str.Append(DrawContentTemplatesFooter());
        str.Append("</div>");

        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<table><tbody>");
        str.Append("<tr>");
        str.Append("<td>");
        if ((!isInheriting) || _FolderData.Id == 0)
        {
            str.Append("<select name=\"addTemplate\" id=\"addTemplate\">");
        }
        else
        {
            str.Append("<select name=\"addTemplate\" id=\"addTemplate\" disabled>");
        }
        str.Append("<option value=\"0\">" + _MessageHelper.GetMessage("generic select template") + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["template_id"] + "\"");
            if (!string.IsNullOrEmpty(row["url"].ToString()))
            {
                str.Append(" url=\"" + row["url"] + "\"");
            }
            str.Append(">" + row["template_name"] + row["template_type"] + "</option>");
        }
        str.Append("</select>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"#\" onclick=\"PreviewTemplate(\'" + _ContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + _ContentApi.AppPath + "images/UI/Icons/preview.png" + "\" alt=\"Preview Template\" title=\"Preview Template\">");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"#\" onclick=\"ActivateTemplate(\'" + this._ContentApi.SitePath + "\')\"><img src=\"" + _ContentApi.AppPath + "images/ui/icons/add.png" + "\" title=" + _MessageHelper.GetMessage("btn add") + " alt=" + _MessageHelper.GetMessage("btn add") + "/></a>");
        str.Append("</td>");
        str.Append("</tr>");
        str.Append("</tbody></table>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"0\" />");
        }

        str.Append("<div class=\"ektronTopSpace\"></div>");
        str.Append("<a href=\"#\" class=\"button buttonInlineBlock greenHover buttonAdd\" onclick=\"OpenAddDialog()\">" + _MessageHelper.GetMessage("lbl add new template") + "</a>");
        //str.Append("<div id=""div3"" style=""display: none;position: block;""></div><div id=""contentidspan"" style=""display: block;position: block;""><a href=""#"" onclick=""LoadChildPage();return false;"">" & m_refMsg.GetMessage("lbl add new template") & "</a></div>")
        //str.Append("<div id=""FrameContainer"" class=""ChildPageHide"">")
        //str.Append("<iframe id=""ChildPage"" name=""ChildPage"" frameborder=""no"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""auto"">")
        //str.Append("</iframe>")
        //str.Append("</div>")

        template_list.Text = str.ToString();
    }
    #endregion

    #region multi-xml/multi-template postback
    private void ProcessContentTemplatesPostBack()
    {
        string IsInheritingTemplates = Request.Form["TemplateTypeBreak"];
        string IsInheritingXml = Request.Form["TypeBreak"];
        XmlConfigData[] xml_config_list;
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy);

        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");

        int i = 0;
        Collection xml_active_list = new Collection();
        Collection template_active_list = new Collection();
        long default_xml_id = -1;

        if (IsInheritingTemplates == null)
        {
            for (i = 0; i <= template_data.Length - 1; i++)
            {
                if (!(Request.Form["tinput_" + template_data[i].Id] == null))
                {
                    template_active_list.Add(template_data[i].Id, template_data[i].Id.ToString(), null, null);
                }
            }
        }

        if (IsInheritingXml == null)
        {
            if (_FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
            {
                long WeSfId = Ektron.Cms.Content.Calendar.WebCalendar.WebEventSmartformId;
                xml_active_list.Add(WeSfId, WeSfId.ToString(), null, null);
                default_xml_id = WeSfId;
            }
            else
            {
                for (i = 0; i <= xml_config_list.Length - 1; i++)
                {
                    if (!(Request.Form["input_" + xml_config_list[i].Id] == null))
                    {
                        xml_active_list.Add(xml_config_list[i].Id, xml_config_list[i].Id.ToString(), null, null);
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["sfdefault"]))
                {
                    default_xml_id = Convert.ToInt64(Request.Form["sfdefault"]);
                }

                if (string.IsNullOrEmpty(Request.Form["requireSmartForms"]))
                {
                    if (!xml_active_list.Contains("0"))
                    {
                        xml_active_list.Add("0", "0", null, null);
                    }
                }
            }
        }

        _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, xml_active_list);
    }
    private void ProcessProductTemplatesPostBack()
    {
        _ProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);
        string IsInheritingTemplates = Request.Form["TemplateTypeBreak"];
        string IsInheritingXml = Request.Form["TypeBreak"];
        List<Ektron.Cms.Commerce.ProductTypeData> prod_type_list = new List<Ektron.Cms.Commerce.ProductTypeData>();
        TemplateData[] template_data;
        Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();

        prod_type_list = _ProductType.GetList(criteria);
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");

        int i = 0;
        Collection active_prod_list = new Collection();
        Collection template_active_list = new Collection();
        long default_xml_id = -1;

        if (IsInheritingTemplates == null)
        {
            for (i = 0; i <= template_data.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(Request.Form["tinput_" + template_data[i].Id]))
                {
                    template_active_list.Add(template_data[i].Id, template_data[i].Id.ToString(), null, null);
                }
            }
        }

        if (IsInheritingXml == null)
        {
            for (i = 0; i <= prod_type_list.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(Request.Form["input_" + prod_type_list[i].Id]))
                {
                    active_prod_list.Add(prod_type_list[i].Id, prod_type_list[i].Id.ToString(), null, null);
                }
            }

            if (!string.IsNullOrEmpty(Request.Form["sfdefault"]))
            {
                default_xml_id = Convert.ToInt64(Request.Form["sfdefault"]);
            }

            if (string.IsNullOrEmpty(Request.Form["requireSmartForms"]))
            {
                if (!active_prod_list.Contains("0"))
                {
                    active_prod_list.Add("0", "0", null, null);
                }
            }
        }

        _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, active_prod_list);
    }
    #endregion

    #region Sitemap Path
    private void DisplaySitemapPath()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        sJS.AppendLine("var clientName_chkInheritSitemapPath = \'chkInheritSitemapPath\';");
        if (_FolderData.SitemapInherited == 1 && _FolderData.Id != 0)
        {
            //chkInheritSitemapPath.Checked = True
            sJS.AppendLine("document.getElementById(\"hdnInheritSitemap\").value = \'true\';");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").checked = true;");
            sJS.AppendLine("document.getElementById(\"AddSitemapNode\").style.display = \'none\';");
        }
        else
        {
            sJS.AppendLine("document.getElementById(\"hdnInheritSitemap\").value = \'false\';");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").checked = false;");
            //chkInheritSitemapPath.Checked = False
        }
        if (_FolderData.Id == 0)
        {
            //chkInheritSitemapPath.Disabled = True
            Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "controls/folder/sitemap.js", "EktronSitemapJS");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").disable = true;");
            sJS.AppendLine("document.getElementById(\"dvInheritSitemap\").style.display = \'none\';");
        }
        if (_FolderData.SitemapPath != null)
        {
            sJS.Append("arSitemapPathNodes = new Array(");
            foreach (Ektron.Cms.Common.SitemapPath node in _FolderData.SitemapPath)
            {
                if (node != null)
                {
                    if (node.Order != 0)
                    {
                        sJS.Append(",");
                    }
                    sJS.Append("new node(\'" + node.Title + "\',\'" + node.Url + "\',\'" + node.Description + "\'," + node.Order + ")");
                }
            }
            sJS.AppendLine(");");
            sJS.AppendLine("renderSiteMapNodes();");
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "renderSitepath", sJS.ToString(), true);

    }
    private void DisplaySiteAlias()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();
        Ektron.Cms.SiteAliasApi _refSiteAliasApi = new Ektron.Cms.SiteAliasApi();

        System.Collections.Generic.List<Ektron.Cms.Common.SiteAliasData> siteAliasList;
        Ektron.Cms.PagingInfo pagingInfo = new Ektron.Cms.PagingInfo();
        int index = 0;

        siteAliasList = _refSiteAliasApi.GetList(pagingInfo, _FolderData.Id);
        if (siteAliasList != null)
        {
            sJS.Append("arSiteAliasNames = new Array(");
            foreach (Ektron.Cms.Common.SiteAliasData item in siteAliasList)
            {
                if (item != null)
                {
                    if (index != 0)
                    {
                        sJS.Append(",");
                    }
                    sJS.Append("new item(\'" + item.SiteAliasName + "\'," + index + ")");
                    index++;
                }
            }
            sJS.AppendLine(");");
            sJS.AppendLine("renderSiteAliasNames();");
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "renderSiteAliasNames", sJS.ToString(), true);
    }

    #endregion

    #region CSS, JS

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "/controls/folder/sitemap.js", "EktronSitemapJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "/controls/folder/sitealias.js", "EktronSiteAliasJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "/tree/js/com.ektron.utils.dom.js", "EktronDomUtilsJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

    #endregion

}
