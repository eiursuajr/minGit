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
using System.Xml;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.DataIO.LicenseManager;
using Microsoft.VisualBasic.CompilerServices;


public partial class addfolder : UserControl
{

    #region members

    protected ContentAPI _ContentApi = new ContentAPI();
    protected CustomFieldsApi _CustomFieldsApi = new CustomFieldsApi();
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected EkMessageHelper _messageHelper;
    protected long _Id = 0;
    protected FolderData _FolderData;
    protected PermissionData _PermissionData;
    protected string _AppPath = "";
    protected string _AppImagePath = "";
    protected int _ContentType = 1;
    protected long _CurrentUserId = 0;
    protected Collection _PageData;
    protected string _PageAction = "";
    protected string _OrderBy = "";
    protected int _ContentLanguage = -1;
    protected int _EnableMultilingual = 0;
    protected string _SitePath = "";
    protected long _FolderId = -1;
    protected string _SelectedTaxonomyList = "";
    protected int _CurrentCategoryChecked = 0;
    protected Hashtable _AssignedFlags = new Hashtable();
    protected bool _IsCatalog = false;
    protected string _Type = "";
    protected ProductType _ProductType = null;
    protected string _IsPublishedAsPdf = string.Empty;

    private SubscriptionData[] _SubscriptionData;
    private SubscriptionData[] _SubscribedData;
    private SubscriptionPropertiesData _SubscriptionProperties;


    #endregion

    #region Page functions

    protected void Page_Init(object sender, System.EventArgs e)
    {
        RegisterResources();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        _Type = Request.QueryString["type"];
        phTypes.Visible = (_Type == "folder") || (_Type == "communityfolder") || (_Type == "site") || (_Type == "") || (_Type == "catalog");
        phTypesPanel.Visible = phTypes.Visible;
        phWebAlerts.Visible = _Type != "catalog";
        lblSiteAlias.Text = MessageHelper.GetMessage("lbl site alias");
        drpVisibility.Items.Add(new ListItem(MessageHelper.GetMessage("lbl public"), "0"));
        drpVisibility.Items.Add(new ListItem(MessageHelper.GetMessage("lbl private"), "1"));
        setLocalText();
       chkRequire.ToolTip= chkRequire.Text = chk_adb_ra.Text = MessageHelper.GetMessage("lbl require authentication");
       chkModerate.ToolTip= chkModerate.Text = chk_adb_mc.Text = MessageHelper.GetMessage("lbl moderate comments");
        chkEnable.ToolTip= chkEnable.Text = MessageHelper.GetMessage("lbl enable comments");
    }

    protected EkMessageHelper MessageHelper {
        get {
            return (_messageHelper ?? (_messageHelper = _ContentApi.EkMsgRef));
        }
    }

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, this._ContentApi.ApplicationPath + "/controls/folder/sitemap.js", "EktronSitemapJS");
        Ektron.Cms.API.JS.RegisterJS(this, this._ContentApi.ApplicationPath + "/controls/folder/sitealias.js", "EktronWorkareaSiteAliasJS");
        Ektron.Cms.API.JS.RegisterJS(this, this._ContentApi.ApplicationPath + "tree/js/com.ektron.utils.dom.js", "EktronWorkareaTreeUtilsJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

    #endregion

    #region ACTION - DoAddFolder
    private void Process_DoAddCalendar()
    {
        Ektron.Cms.Content.Calendar.CalendarDal calapi = new Ektron.Cms.Content.Calendar.CalendarDal(_ContentApi.RequestInformationRef);
        FolderRequest calendar = new FolderRequest();
        string FolderPath;

        calendar.FolderName = (string)(Request.Form["foldername"].Trim(".".ToCharArray()));
        calendar.FolderDescription = Request.Form["folderdescription"];
        calendar.ParentId = _Id;
        if (Request.Form["TemplateTypeBreak"] == null)
        {
            calendar.TemplateFileName = Request.Form["templatefilename"];
        }
        else
        {
            calendar.TemplateFileName = "";
        }
        calendar.StyleSheet = Request.Form["stylesheet"];
        calendar.SiteMapPathInherit = System.Convert.ToBoolean((Request.Form["hdnInheritSitemap"] != null) && (Request.Form["hdnInheritSitemap"].ToString().ToLower() == "true"));
        calendar.SiteMapPath = Utilities.DeserializeSitemapPath(Request.Form, this._ContentLanguage);
        calendar.MetaInherited = System.Convert.ToInt32(((Request.Form["break_inherit_button"] != null) && Request.Form["break_inherit_button"].ToString().ToLower() == "on") ? 1 : 0);
        calendar.MetaInheritedFrom = Convert.ToInt64(Request.Form["inherit_meta_from"]);
        calendar.FolderCfldAssignments = Request.Form["folder_cfld_assignments"];
        calendar.XmlInherited = false;
        calendar.XmlConfiguration = "0";
        calendar.StyleSheet = Request.Form["stylesheet"];
        calendar.TaxonomyInherited = System.Convert.ToBoolean((Request.Form["TaxonomyTypeBreak"] != null) && Request.Form["TaxonomyTypeBreak"].ToString().ToLower() == "on");
        calendar.CategoryRequired = System.Convert.ToBoolean((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on");
        calendar.TaxonomyInheritedFrom = Convert.ToInt64(Request.Form[inherit_taxonomy_from.UniqueID]);
        calendar.AliasInherited = System.Convert.ToBoolean((Request.Form["chkInheritAliases"] != null) && Request.Form["chkInheritAliases"].ToString().ToLower() == "on");
        calendar.AliasInheritedFrom = Convert.ToInt64(Request.Form[inherit_alias_from.UniqueID]);
        calendar.AliasRequired = System.Convert.ToBoolean((Request.Form["chkForceAliasing"] != null) && Request.Form["chkForceAliasing"].ToString().ToLower() == "on");
        string IdRequests = "";
        if ((Request.Form["taxlist"] != null) && Request.Form["taxlist"] != "")
        {
            IdRequests = Request.Form["taxlist"];
        }
        calendar.TaxonomyIdList = IdRequests;
        calendar.FolderType = Convert.ToInt16(EkEnumeration.FolderType.Calendar);
        calendar.IsDomainFolder = false;
        calendar.DomainProduction = Request.Form["DomainProduction"];
        calendar.DomainStaging = Request.Form["DomainStaging"];
        calendar.SubscriptionProperties = new SubscriptionPropertiesData();
        calendar.SubscriptionProperties.BreakInheritance = System.Convert.ToBoolean(!string.IsNullOrEmpty((Request.Form["webalert_inherit_button"])) ? false : true);
        if (Request.Form["notify_option"] == ("Always"))
        {
            calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always;
        }
        else if (Request.Form["notify_option"] == ("Initial"))
        {
            calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial;
        }
        else if (Request.Form["notify_option"] == ("Never"))
        {
            calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never;
        }
        calendar.SubscriptionProperties.SuspendNextNotification = false;
        calendar.SubscriptionProperties.SendNextNotification = false;
        calendar.SubscriptionProperties.OptOutID = Convert.ToInt64(Request.Form["notify_optoutid"]);
        calendar.SubscriptionProperties.DefaultMessageID = (!string.IsNullOrEmpty(Request.Form["use_message_button"])) ? (Convert.ToInt64(Request.Form["notify_messageid"])) : 0;
        calendar.SubscriptionProperties.SummaryID = (!string.IsNullOrEmpty(Request.Form["use_summary_button"])) ? 1 : 0;
        calendar.SubscriptionProperties.ContentID = (!string.IsNullOrEmpty(Request.Form["use_content_button"])) ? (Convert.ToInt64(Request.Form["frm_content_id"])) : 0;
        calendar.SubscriptionProperties.UnsubscribeID = Convert.ToInt64(Request.Form["notify_unsubscribeid"]);
        calendar.SubscriptionProperties.URL = (string)((!string.IsNullOrEmpty(Request.Form["notify_url"])) ? (Request.Form["notify_url"]) : (Request.ServerVariables["HTTP_HOST"]));
        calendar.SubscriptionProperties.FileLocation = Server.MapPath(_ContentApi.AppPath + "subscriptions");
        calendar.SubscriptionProperties.WebLocation = (string)((!string.IsNullOrEmpty(Request.Form["notify_weblocation"])) ? (Request.Form["notify_weblocation"]) : "subscriptions");
        calendar.SubscriptionProperties.Subject = (string)((!string.IsNullOrEmpty(Request.Form["notify_subject"])) ? (Request.Form["notify_subject"]) : "");
        calendar.SubscriptionProperties.EmailFrom = (string)((!string.IsNullOrEmpty(Request.Form["notify_emailfrom"])) ? (Request.Form["notify_emailfrom"]) : "");
        calendar.SubscriptionProperties.UseContentTitle = "";
        calendar.SubscriptionProperties.UseContentLink = System.Convert.ToInt32((!string.IsNullOrEmpty(Request.Form["use_contentlink_button"])) ? 1 : 0);
        calendar.ContentSubAssignments = Request.Form["content_sub_assignments"];
        //-----------------IscontentSearchable-----------------------
        if (Request.Form["chkInheritIscontentSearchable"] != null && Request.Form["chkInheritIscontentSearchable"].ToString().ToLower() == "on")
        {
            calendar.IsContentSearchableInherited = true;
            if (Request.Form["chkIscontentSearchable"] != null && Request.Form["chkIscontentSearchable"].ToString().ToLower() == "on")
            {
                calendar.IscontentSearchable = true;
            }
            else
            {
                calendar.IscontentSearchable = (Request.Form[current_IscontentSearchable.UniqueID] == "1");
            }
        }
        else
        {
            calendar.IsContentSearchableInherited = false;
            if (Request.Form["chkIscontentSearchable"] != null && Request.Form["chkIscontentSearchable"].ToString().ToLower() == "on")
            {
                calendar.IscontentSearchable = true;
            }
            else
            {
                calendar.IscontentSearchable = false;
            }
        }
        calendar.IsContentSearchableInheritedFrom = long.Parse(Request.Form[inherit_IscontentSearchable_from.UniqueID]);
        //-----------------IsContentSearchable End-------------------
        //-------------------DisplaySettings--------------------
        int totalTabs = 0;
        if (Request.Form["chkInheritIsDisplaySettings"] != null && Request.Form["chkInheritIsDisplaySettings"].ToString().ToLower() == "on")
        {
            calendar.IsDisplaySettingsInherited = true;
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
                calendar.DisplaySettings = totalTabs;
            }
            else
            {
                calendar.DisplaySettings = int.Parse(Request.Form[current_IsDisplaySettings.UniqueID]);               
            }
        }
        else
        {
            calendar.IsDisplaySettingsInherited = false;
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
                calendar.DisplaySettings = totalTabs;
            }
            else
            {
                calendar.DisplaySettings = 1;
            }
        }
        calendar.DisplaySettingsInheritedFrom = long.Parse(Request.Form[inherit_IsDisplaySettings_from.UniqueID]);      
        //-------------------DisplaySettingsEnd------------------
        long calendarid = calapi.AddCalendar(calendar);

        _CustomFieldsApi.ProcessCustomFields(calendarid);

        FolderPath = _ContentApi.EkContentRef.GetFolderPath(calendarid);
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + calendarid + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + calendarid + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
    }
    private bool Process_DoAddFolder()
    {
        string tmpPath;
        Collection libSettings;
        string FolderPath;
        Ektron.Cms.Content.EkContent m_refContent;
        SubscriptionPropertiesData sub_prop_data = new SubscriptionPropertiesData();
        Collection page_subscription_data = new Collection();
        Collection page_sub_temp = new Collection();
        Array arrSubscriptions;
        int isub = 0;
        List<string> siteAliasList = new List<string>();
        string[] arSiteAliasList;

        Ektron.Cms.SiteAliasApi _refSiteAliasApi;


        m_refContent = _ContentApi.EkContentRef;
        _PageData = new Collection();
        _PageData.Add(Request.Form["foldername"].Trim(".".ToCharArray()), "FolderName", null, null);
        _PageData.Add(Request.Form["folderdescription"], "FolderDescription", null, null);
        _PageData.Add(_Id, "ParentID", null, null); //pagedata.Add(Request.Form("ParentID"), "ParentID")
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
            if (Request.Form["templatefilename"].Split("(".ToCharArray()).Length > 1)
            {
                _PageData.Add(EkEnumeration.TemplateSubType.Wireframes, "TemplateSubType", null, null);
            }
            else
            {
                _PageData.Add(EkEnumeration.TemplateSubType.Default, "TemplateSubType", null, null);
            }
        }
        else
        {
            _PageData.Add("", "TemplateFileName", null, null);
        }
        _PageData.Add(Request.Form["stylesheet"], "StyleSheet", null, null);
        if ((Request.Form["hdnInheritSitemap"] != null) && (Request.Form["hdnInheritSitemap"].ToString().ToLower() == "true"))
        {
            _PageData.Add(true, "SitemapPathInherit", null, null);
        }
        else
        {
            _PageData.Add(false, "SitemapPathInherit", null, null);
        }
        _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, this._ContentLanguage), "SitemapPath", null, null);

        Ektron.Cms.Library.EkLibrary objLib;
        objLib = _ContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = Convert.ToString(libSettings["ImageDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = Convert.ToString(libSettings["FileDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);

        if (Request.Form["webalert_inherit_button"] != null && Request.Form["webalert_inherit_button"].ToString().Length > 0)
        {
            sub_prop_data.BreakInheritance = false;
        }
        else
        {
            sub_prop_data.BreakInheritance = true;
        }

        if (Request.Form["notify_option"] != null)
        {
            if (Request.Form["notify_option"] == "Always")
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always;
            }
            else if (Request.Form["notify_option"] == "Initial")
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial;
            }
            else if (Request.Form["notify_option"] == "Never")
            {
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never;
            }
        }

        sub_prop_data.SuspendNextNotification = false;
        sub_prop_data.SendNextNotification = false;

        sub_prop_data.OptOutID = Convert.ToInt64(Request.Form["notify_optoutid"]);
        if (Request.Form["use_message_button"] != null && Request.Form["use_message_button"].ToString().Length > 0)
        {
            sub_prop_data.DefaultMessageID = Convert.ToInt64(Request.Form["notify_messageid"]);
        }
        else
        {
            sub_prop_data.DefaultMessageID = 0;
        }
        if (Request.Form["use_summary_button"] != null && Request.Form["use_summary_button"].ToString().Length > 0)
        {
            sub_prop_data.SummaryID = 1;
        }
        else
        {
            sub_prop_data.SummaryID = 0;
        }
        if (Request.Form["use_content_button"] != null && Request.Form["use_content_button"].ToString().Length > 0)
        {
            sub_prop_data.ContentID = Convert.ToInt64(Request.Form["frm_content_id"]);
        }
        else
        {
            sub_prop_data.ContentID = 0;
        }
        sub_prop_data.UnsubscribeID = Convert.ToInt64(Request.Form["notify_unsubscribeid"]);

        if (Request.Form["notify_url"] != null && Request.Form["notify_url"] != "")
        {
            sub_prop_data.URL = Request.Form["notify_url"];
        }
        else
        {
            sub_prop_data.URL = Request.ServerVariables["HTTP_HOST"];
        }

        if (Request.Form["notify_weblocation"] != null && Request.Form["notify_weblocation"] != "")
        {
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath + "subscriptions");
        }
        else
        {
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath + "subscriptions");
        }
        if (Request.Form["notify_weblocation"] != null && Request.Form["notify_weblocation"] != "")
        {
            sub_prop_data.WebLocation = Request.Form["notify_weblocation"];
        }
        else
        {
            sub_prop_data.WebLocation = "subscriptions";
        }
        if (Request.Form["notify_subject"] != null && Request.Form["notify_subject"] != "")
        {
            sub_prop_data.Subject = Request.Form["notify_subject"];
        }
        else
        {
            sub_prop_data.Subject = "";
        }
        if (Request.Form["notify_emailfrom"] != null && Request.Form["notify_emailfrom"] != "")
        {
            sub_prop_data.EmailFrom = Request.Form["notify_emailfrom"];
        }
        else
        {
            sub_prop_data.EmailFrom = "";
        }

        sub_prop_data.UseContentTitle = "";

        if (Request.Form["use_contentlink_button"] != null && Request.Form["use_contentlink_button"].ToString().Length > 0)
        {
            sub_prop_data.UseContentLink = 1;
        }
        else
        {
            sub_prop_data.UseContentLink = 0;
        }

        if (Request.Form["content_sub_assignments"] != null && Request.Form["content_sub_assignments"].ToString().Length > 0)
        {
            arrSubscriptions = Strings.Split(Strings.Trim(Request.Form["content_sub_assignments"]), " ", -1, 0);
            if (arrSubscriptions.Length > 0)
            {
                for (isub = 0; isub <= (arrSubscriptions.Length - 1); isub++)
                {
                    page_sub_temp = new Collection();
                    page_sub_temp.Add(long.Parse(Strings.Mid(arrSubscriptions.GetValue(isub).ToString(), 10)), "ID", null, null);
                    page_subscription_data.Add(page_sub_temp, null, null, null);
                }
            }
        }
        else
        {
            page_subscription_data = null;
        }
        page_sub_temp = null;

        Utilities.AddLBpaths(_PageData);

        if (Request.Form["TypeBreak"] != null && Request.Form["TypeBreak"].ToString().ToLower() == "on") // old field name was frm_xmlinheritance in V7.x
        {
            _PageData.Add(true, "XmlInherited", null, null);
        }
        else
        {
            _PageData.Add(false, "XmlInherited", null, null);
        }
        _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);

        bool isPublishedAsPdf = System.Convert.ToBoolean((Request.Form["publishAsPdf"] == "on") ? true : false);
        _PageData.Add(isPublishedAsPdf, "PublishPdfActive", null, null);
        _PageData.Add(false, "PublishHtmlActive", null, null);

        // handle dynamic replication properties
        if (Request.Form["EnableReplication"] != "" || Request.QueryString["type"] == "communityfolder")
        {
            _PageData.Add(Request.Form["EnableReplication"], "EnableReplication", null, null);
        }
        else
        {
            _PageData.Add(0, "EnableReplication", null, null);
        }

        if (string.IsNullOrEmpty(Request.Form["suppress_notification"]))
        {
            _PageData.Add(sub_prop_data, "SubscriptionProperties", null, null);
            _PageData.Add(page_subscription_data, "Subscriptions", null, null);
        }

        if (Request.Form["break_inherit_button"] != null && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
        {
            _PageData.Add(0, "break_inherit_button", null, null); //inherit button is checked => Metadata is inherited from parent.
        }
        else
        {
            _PageData.Add(1, "break_inherit_button", null, null); //break inheritance, do NOT inherit from parent
        }

        _PageData.Add(Request.Form["folder_cfld_assignments"], "folder_cfld_assignments", null, null);

        // add domain properties if they're there
        if ((Request.Form["IsDomainFolder"] != null && Request.Form["IsDomainFolder"] != "") && (Request.Form["DomainProduction"] != null && Request.Form["DomainProduction"] != "") && LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.MultiSite) && !LicenseManager.IsSiteLimitReached(_ContentApi.RequestInformationRef))
        {
            _PageData.Add(true, "IsDomainFolder", null, null);
            string staging = Request.Form["DomainStaging"];
            string production = Request.Form["DomainProduction"];
            if (staging == null)
            {
                staging = "";
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
        if (Request.Form["break_inherit_button"] != null && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
        {
            _PageData.Add(1, "InheritMetadata", null, null); //break inherit button is check.
        }
        else
        {
            _PageData.Add(0, "InheritMetadata", null, null);
        }
        _PageData.Add(Request.Form["inherit_meta_from"], "InheritMetadataFrom", null, null);

        if ((Request.QueryString["type"] != null) && (Request.QueryString["type"] == "communityfolder"))
        {
            _PageData.Add(true, "IsCommunityFolder", null, null);
        }
        if (Request.Form["TaxonomyTypeBreak"] != null && Request.Form["TaxonomyTypeBreak"].ToString().ToLower() == "on")
        {
            _PageData.Add(1, "InheritTaxonomy", null, null);
            if (Request.Form["CategoryRequired"] != null && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "CategoryRequired", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[current_category_required.UniqueID], "CategoryRequired", null, null);
            }
        }
        else
        {
            _PageData.Add(0, "InheritTaxonomy", null, null);
            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "CategoryRequired", null, null);
            }
            else
            {
                _PageData.Add(0, "CategoryRequired", null, null);
            }
        }
        string IdRequests = "";
        if (Request.Form["taxlist"] != null && Request.Form["taxlist"] != "")
        {
            IdRequests = Request.Form["taxlist"];
        }
        _PageData.Add(IdRequests, "TaxonomyList", null, null);
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
        //-----------------IscontentSearchable-----------------------
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
        //-----------------IsContentSearchable End-------------------
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
                if (Request.Form["chkIsDisplaySettingsMetaData"] !=  null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on")
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
           if ((Request.Form["chkIsDisplaySettingsAllTabs"] != null && Request.Form["chkIsDisplaySettingsAllTabs"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsSummary"] != null && Request.Form["chkIsDisplaySettingsSummary"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsMetaData"] != null && Request.Form["chkIsDisplaySettingsMetaData"].ToString().ToLower() == "on" )||( Request.Form["chkIsDisplaySettingsAliasing"] != null && Request.Form["chkIsDisplaySettingsAliasing"].ToString().ToLower() == "on" )|| (Request.Form["chkIsDisplaySettingsSchedule"] != null && Request.Form["chkIsDisplaySettingsSchedule"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsComment"] != null && Request.Form["chkIsDisplaySettingsComment"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTemplates"] != null && Request.Form["chkIsDisplaySettingsTemplates"].ToString().ToLower() == "on") || (Request.Form["chkIsDisplaySettingsTaxonomy"] != null && Request.Form["chkIsDisplaySettingsTaxonomy"].ToString().ToLower() == "on"))
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
        _PageData.Add(Request.Form[inherit_taxonomy_from.UniqueID], "InheritTaxonomyFrom", null, null);

        // Update - add flagging items:
        ProcessFlaggingPostBack(_PageData);

        if (m_refContent.DoesFolderExistsWithName((string)_PageData["FolderName"], (long)_PageData["ParentID"])) {
            ShowError(MessageHelper.GetMessage("com: subfolder already exists"));
            return false;
        } else {
        m_refContent.AddContentFolderv2_0(ref _PageData);
        }

        //_CustomFieldsApi.ProcessCustomFields(_PageData("FolderID"))

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(_PageData["ParentID"]));
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }

        // find the folder_id we just created now...
        _FolderId = _ContentApi.EkContentRef.GetFolderID((string)(_ContentApi.EkContentRef.GetFolderPath(Convert.ToInt64(_PageData["ParentID"])) + "\\" + Request.Form["foldername"].Trim(".".ToCharArray())));
        if (_Type == "site")
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
            _refSiteAliasApi.Save(_FolderId, siteAliasList);
        }

        return true;
    }

    protected void ShowError(string errorMessage) {
        hdnErrorMessage.Value = errorMessage;
    }

    #endregion

    #region ACTION - DoAddBlog
    private void Process_DoAddBlog()
    {
        string tmpPath;
        Collection libSettings;
        string FolderPath;
        Ektron.Cms.Content.EkContent m_refContent;
        int i = 0;
        BlogRollItem[] abriRoll;
        string sCatTemp = "";

        m_refContent = _ContentApi.EkContentRef;
        _PageData = new Collection();
        _PageData.Add(true, "IsBlog", null, null);
        _PageData.Add(Request.Form[txtBlogName.UniqueID].Trim(".".ToCharArray()), "FolderName", null, null);
        _PageData.Add("", "FolderDescription", null, null);
        _PageData.Add(Request.Form[txtTitle.UniqueID], "BlogTitle", null, null);
        _PageData.Add(Request.Form[drpVisibility.UniqueID], "BlogVisible", null, null);
        _PageData.Add(Request.Form[chkEnable.UniqueID], "CommentEnable", null, null);
        _PageData.Add(Request.Form[chkModerate.UniqueID], "CommentModerate", null, null);
        _PageData.Add(false, "SitemapPathInherit", null, null);
        _PageData.Add(Request.Form[chkRequire.UniqueID], "CommentRequire", null, null);
        _PageData.Add(Request.Form[hdnfolderid.UniqueID], "ParentID", null, null); //pagedata.Add(Request.Form("ParentID"), "ParentID")
        if (Request.Form["TemplateTypeBreak"] == null)
        {
            _PageData.Add(Request.Form["templatefilename"], "TemplateFileName", null, null);
        }
        else
        {
            _PageData.Add("", "TemplateFileName", null, null);
        }
        if (_ContentApi.SitePath == "/")
        {
            _PageData.Add(_ContentApi.AppPath.Replace(_ContentApi.SitePath, "") + "/csslib/blogs.css", "StyleSheet", null, null);
        }
        else
        {
            _PageData.Add(_ContentApi.AppPath.Replace(_ContentApi.SitePath, "") + "csslib/blogs.css", "StyleSheet", null, null);
        }

        Ektron.Cms.Library.EkLibrary objLib;
        objLib = _ContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = Convert.ToString(libSettings["ImageDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = Convert.ToString(libSettings["FileDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);
        Utilities.AddLBpaths(_PageData);

        _PageData.Add(true, "XmlInherited", null, null);
        _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
        _PageData.Add(false, "PublishPdfActive", null, null);
        _PageData.Add(false, "PublishHtmlActive", null, null);
        _PageData.Add(false, "IsDomainFolder", null, null);
        // handle dynamic replication properties
        if (Request.Form["EnableReplication"] != "")
        {
            _PageData.Add(Request.Form["EnableReplication"], "EnableReplication", null, null);
        }
        else
        {
            _PageData.Add(0, "EnableReplication", null, null);
        }
        if (Request.Form["categorylength"] != "")
        {
            for (i = 0; i <= (Convert.ToInt32(Request.Form["categorylength"]) - 1); i++)
            {
                if (Request.Form["category" + i.ToString()] != "")
                {
                    if (i == (Convert.ToInt32(Request.Form["categorylength"]) - 1))
                    {
                        sCatTemp += Request.Form["category" + i.ToString()];
                    }
                    else
                    {
                        sCatTemp += (string)(Request.Form["category" + i.ToString()] + ";");
                    }
                }
            }
        }
        _PageData.Add(sCatTemp, "blogcategories", null, null);

        //Start Taxonomy Addition
        if ((Request.Form["TaxonomyTypeBreak"] != null) && Request.Form["TaxonomyTypeBreak"].ToString().ToLower() == "on")
        {
            _PageData.Add(1, "InheritTaxonomy", null, null);
            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "CategoryRequired", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[current_category_required.UniqueID], "CategoryRequired", null, null);
            }
        }
        else
        {
            _PageData.Add(0, "InheritTaxonomy", null, null);
            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
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
        //End Taxonomy Addition

        _PageData.Add(IdRequests, "TaxonomyList", null, null);
        _PageData.Add(Request.Form[inherit_taxonomy_from.UniqueID], "InheritTaxonomyFrom", null, null);
        if (Request.Form["rolllength"] != "")
        {
            abriRoll = new BlogRollItem[1];
            for (i = 0; i <= (Convert.ToInt32(Request.Form["rolllength"]) - 1); i++)
            {
                Array.Resize(ref abriRoll, i + 1);
                if (Request.Form["editfolder_linkname" + i.ToString()] != "" && Request.Form["editfolder_url" + i.ToString()] != "")
                {
                    //add only if we have something with a name/url
                    abriRoll[i] = new BlogRollItem();
                    abriRoll[i].LinkName = Request.Form["editfolder_linkname" + i.ToString()];
                    abriRoll[i].URL = Request.Form["editfolder_url" + i.ToString()];
                    if (Request.Form["editfolder_short" + i.ToString()] != "")
                    {
                        abriRoll[i].ShortDescription = Request.Form["editfolder_short" + i.ToString()];
                    }
                    else
                    {
                        abriRoll[i].ShortDescription = "";
                    }
                    if (Request.Form["editfolder_rel" + i.ToString()] != "")
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
        //-------------------ContentSearchable------------
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
       //-------------------ContentSearchableEnd------------------
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
        _PageData.Add("1", "break_inherit_button", null, null);
        _PageData.Add("", "folder_cfld_assignments", null, null);
        _PageData.Add(1, "InheritMetadata", null, null); //break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom", null, null);

        m_refContent.AddContentFolderv2_0(ref _PageData);

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(_PageData["ParentID"]));
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        _FolderId = Convert.ToInt64(_PageData["FolderID"]);

    }
    #endregion

    #region Action - DoAddDiscussionBoard

    private void Process_DoAddDiscussionBoard()
    {
        string tmpPath;
        Collection libSettings;
        string FolderPath;
        Ektron.Cms.Content.EkContent m_refContent;
        int i = 0;
        string sCatTemp = "";

        m_refContent = _ContentApi.EkContentRef;
        _PageData = new Collection();
        _PageData.Add(true, "IsDiscussionBoard", null, null);
        _PageData.Add(Request.Form[txt_adb_boardname.UniqueID].Trim(".".ToCharArray()), "FolderName", null, null);
        _PageData.Add(Request.Form[txt_adb_title.UniqueID], "FolderDescription", null, null);
        if (Request.Form[chk_adb_mc.UniqueID] != "")
        {
            _PageData.Add(true, "CommentModerate", null, null);
        }
        else
        {
            _PageData.Add(false, "CommentModerate", null, null);
        }
        if (Request.Form[chk_adb_ra.UniqueID] != "")
        {
            _PageData.Add(true, "CommentRequire", null, null);
        }
        else
        {
            _PageData.Add(false, "CommentRequire", null, null);
        }
        _PageData.Add(Request.Form[hdn_adb_folderid.UniqueID], "ParentID", null, null); //pagedata.Add(Request.Form("ParentID"), "ParentID")
        _PageData.Add("", "TemplateFileName", null, null);
        _PageData.Add(false, "SitemapPathInherit", null, null);
        string sJustAppPath = _ContentApi.AppPath.Replace(_ContentApi.SitePath, "");
        if (sJustAppPath.Length > 0 && !(sJustAppPath[sJustAppPath.Length - 1].ToString() == "/"))
        {
            sJustAppPath = sJustAppPath + "/";
        }
        string sCSS = "";
        if (Request.Form[txt_adb_stylesheet.UniqueID] != "")
        {
            sCSS = Request.Form[txt_adb_stylesheet.UniqueID];
        }
        _PageData.Add(sCSS, "StyleSheet", null, null); //use default forum CSS

        Ektron.Cms.Library.EkLibrary objLib;
        objLib = _ContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = Convert.ToString(libSettings["ImageDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = Convert.ToString(libSettings["FileDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);
        Utilities.AddLBpaths(_PageData);

        _PageData.Add(false, "XmlInherited", null, null);
        _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
        _PageData.Add(false, "PublishPdfActive", null, null);
        _PageData.Add(false, "PublishHtmlActive", null, null);
        _PageData.Add(false, "IsDomainFolder", null, null);
        // handle dynamic replication properties
        if (Request.Form["EnableReplication"] != "")
        {
            _PageData.Add(Request.Form["EnableReplication"], "EnableReplication", null, null);
        }
        else
        {
            _PageData.Add(0, "EnableReplication", null, null);
        }
        if (Request.Form["categorylength"] != "")
        {
            for (i = 0; i <= (Convert.ToInt32(Request.Form["categorylength"]) - 1); i++)
            {
                if (Request.Form["category" + i.ToString()] != "")
                {
                    if (i == (Convert.ToInt32(Request.Form["categorylength"]) - 1))
                    {
                        sCatTemp += Request.Form["category" + i.ToString()];
                    }
                    else
                    {
                        sCatTemp += (string)(Request.Form["category" + i.ToString()] + ";");
                    }
                }
            }
        }
        _PageData.Add(sCatTemp, "DiscussionBoardCategories", null, null);

        _PageData.Add("", "break_inherit_button", null, null);
        _PageData.Add("", "folder_cfld_assignments", null, null);
        _PageData.Add(0, "InheritMetadata", null, null); //break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom", null, null);
        m_refContent.AddContentFolderv2_0(ref _PageData);

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(_PageData["ParentID"]));
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }

        // find the folder_id we just created now...
        _FolderId = Convert.ToInt64(_PageData["FolderID"]);
    }

    #endregion

    #region Action - DoAddDiscussionForum

    private void Process_DoAddDiscussionForum()
    {
        string tmpPath;
        Collection libSettings;
        string FolderPath;
        Ektron.Cms.Content.EkContent m_refContent;


        m_refContent = _ContentApi.EkContentRef;
        _PageData = new Collection();
        _PageData.Add(true, "IsDiscussionForum", null, null);
        _PageData.Add(Request.Form[txt_adf_forumname.UniqueID].Trim(".".ToCharArray()), "FolderName", null, null);
        _PageData.Add(Request.Form[txt_adf_forumtitle.UniqueID], "FolderDescription", null, null);
        if (Information.IsNumeric(Request.Form[txt_adf_sortorder.UniqueID]) && Convert.ToInt32(Request.Form[txt_adf_sortorder.UniqueID]) > 0)
        {
            _PageData.Add(Request.Form[txt_adf_sortorder.UniqueID], "SortOrder", null, null);
        }
        else
        {
            _PageData.Add(1, "SortOrder", null, null);
        }
        if (Request.Form[chk_adf_moderate.UniqueID] != "")
        {
            _PageData.Add(true, "CommentModerate", null, null);
        }
        else
        {
            _PageData.Add(false, "CommentModerate", null, null);
        }
        if (Request.Form[chk_adf_lock.UniqueID] != null && Request.Form[chk_adf_lock.UniqueID].ToString().ToLower() == "on")
        {
            _PageData.Add(true, "LockForum", null, null);
        }
        else
        {
            _PageData.Add(false, "LockForum", null, null);
        }
        // handle dynamic replication properties
        if (Request.Form["EnableReplication"] != "")
        {
            _PageData.Add(Request.Form["EnableReplication"], "EnableReplication", null, null);
        }
        else
        {
            _PageData.Add(0, "EnableReplication", null, null);
        }
        _PageData.Add(Request.Form[drp_adf_category.UniqueID], "CategoryID", null, null);
        _PageData.Add(false, "SitemapPathInherit", null, null);
        _PageData.Add(Request.Form[hdn_adf_folderid.UniqueID], "ParentID", null, null);
        _PageData.Add("", "TemplateFileName", null, null);
        _PageData.Add(_ContentApi.AppPath.Replace(_ContentApi.SitePath, "") + "csslib/dicussionboard.css", "StyleSheet", null, null);

        Ektron.Cms.Library.EkLibrary objLib;
        objLib = _ContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = libSettings["ImageDirectory"].ToString();
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = libSettings["FileDirectory"].ToString();
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);
        Utilities.AddLBpaths(_PageData);

        _PageData.Add(false, "XmlInherited", null, null);
        _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
        _PageData.Add(false, "PublishPdfActive", null, null);
        _PageData.Add(false, "PublishHtmlActive", null, null);
        _PageData.Add(false, "IsDomainFolder", null, null);

        _PageData.Add("", "break_inherit_button", null, null);
        _PageData.Add("", "folder_cfld_assignments", null, null);
        _PageData.Add(0, "InheritMetadata", null, null); //break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom", null, null);
        m_refContent.AddContentFolderv2_0(ref _PageData);

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(_PageData["ParentID"]));
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
    }

    #endregion

    #region FOLDER - AddSubFolder

    public bool AddSubFolder()
    {
        if (Request.QueryString["id"] != null)
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (Request.QueryString["action"] != null)
        {
            _PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
        }
        if (Request.QueryString["orderby"] != null)
        {
            _OrderBy = Convert.ToString(Request.QueryString["orderby"]);
        }
        if (Request.QueryString["type"] != "")
        {
            _Type = (string)(Request.QueryString["type"].ToLower());
        }
        if (Request.QueryString["LangType"] != null)
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
        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            _ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            _ContentApi.ContentLanguage = _ContentLanguage;
        }
        _CurrentUserId = _ContentApi.UserId;
        _AppImagePath = _ContentApi.AppImgPath;
        _AppPath = _ContentApi.AppPath;
        _SitePath = _ContentApi.SitePath;
        _EnableMultilingual = _ContentApi.EnableMultilingual;
        if (!(Page.IsPostBack))
        {
            switch (_Type)
            {
                case "blog":
                    Display_AddBlog();
                    Display_BlogJS();
                    DrawFolderAliasesTable();
                    IsContentSearchableSection();
                    IsDisplaySettings();
                    break;
                case "discussionboard":
                    Display_AddDiscussionBoard();
                    Display_DiscussionBoardJS();
                    break;
                case "discussionforum":
                    Display_AddDiscussionForum();
                    Display_DiscussionForumJS();
                    break;
                case "catalog":
                    _IsCatalog = true;
                    Display_AddCatalog();
                    Display_CatalogJS();
                    DrawFolderAliasesTable();
                    IsContentSearchableSection();
                    IsDisplaySettings();
                    break;
                case "calendar":
                    Display_AddCalendar();
                    Display_FolderJS();
                    IsContentSearchableSection();
                    IsDisplaySettings();
                    break;
                default:
                    Display_FolderJS();
                    Display_AddSubFolder();
                    DrawFolderAliasesTable();
                    IsContentSearchableSection();
                    IsDisplaySettings();
                    break;
            }
        }
        else
        {
            if (Request.Form[txtBlogName.UniqueID] != null && Request.Form[txtBlogName.UniqueID] != "")
            {
                Process_DoAddBlog();
                ProcessContentTemplatesPostBack("");
            }
            else if (Request.Form[txt_adb_boardname.UniqueID] != null && Request.Form[txt_adb_boardname.UniqueID] != "")
            {
                Process_DoAddDiscussionBoard();
                ProcessContentTemplatesPostBack("forum");
            }
            else if (Request.Form[txt_adf_forumname.UniqueID] != null && Request.Form[txt_adf_forumname.UniqueID] != "")
            {
                Process_DoAddDiscussionForum();
            }
            else if (_Type == "catalog")
            {
                Process_DoAddCatalog();
                ProcessProductTemplatesPostBack("");
            }
            else if (_Type == "calendar")
            {
                Process_DoAddCalendar();
            }
            else
            {
                if (Process_DoAddFolder()) {
                ProcessContentTemplatesPostBack("");
                }
            }

            return (true);
        }
        if (_Type == "catalog")
        {
            DrawProductTypesTable();
        }
        else
        {
            DrawContentTypesTable();
        }
        DrawContentTemplatesTable();
        DrawFlaggingOptions();
        if (Request.QueryString["type"] != "" && Request.QueryString["type"].ToLower() == "communityfolder")
        {
            Display_AddCommunityFolder();
        }
        return true;
    }
    private void Display_FolderJS()
    {
        StringBuilder sbfolderjs = new StringBuilder();
        sbfolderjs.Append("Ektron.ready(function() {" + Environment.NewLine);
        sbfolderjs.Append(" document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbfolderjs.Append(" document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbfolderjs.Append(" document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbfolderjs.Append(" document.forms[0].foldername.focus();" + Environment.NewLine);
        sbfolderjs.Append("   if( $ektron(\'#webalert_inherit_button\').length > 0 ){ " + Environment.NewLine);
        sbfolderjs.Append("     if( $ektron(\'#webalert_inherit_button\')[0].checked ){ " + Environment.NewLine);
        sbfolderjs.Append("       $ektron(\'.selectContent\').css(\'display\', \'none\');" + Environment.NewLine);
        sbfolderjs.Append("       $ektron(\'.useCurrent\').css(\'display\', \'none\');" + Environment.NewLine);
        sbfolderjs.Append("     } " + Environment.NewLine);
        sbfolderjs.Append("    } " + Environment.NewLine);
        sbfolderjs.Append(" });" + Environment.NewLine);

        ltr_af_js.Text = sbfolderjs.ToString();
    }
    private void Display_AddSubFolder()
    {
        bool ShowTaxonomy = false;
        string strFolderAddType = "folder";
        if (!string.IsNullOrEmpty(Request.QueryString["type"]))
        {
            strFolderAddType = (string)(Request.QueryString["type"].ToLower());
        }

        if (strFolderAddType == "blog")
        {
            Display_AddBlog();
            Display_BlogJS();
            return;
        }
        else if (strFolderAddType == "discussionboard")
        {
            Display_AddDiscussionBoard();
            Display_DiscussionBoardJS();
            return;
        }
        else if (strFolderAddType == "discussionforum")
        {
            Display_AddDiscussionForum();
            Display_DiscussionForumJS();
            return;
        }
        else
        {
            ShowTaxonomy = true;
            Display_FolderJS();
        }
        XmlConfigData[] xmlconfig_data;
        TemplateData[] template_data;
        string backup = "";
        string strStyleSheet = "";


        ltrTypes.Text = MessageHelper.GetMessage("Smart Forms txt");
        ltInheritSitemapPath.Text = MessageHelper.GetMessage("lbl inherit parent configuration");
        long iTmpCaller = _ContentApi.RequestInformationRef.CallerId;
        try
        {
            _ContentApi.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin;
            _ContentApi.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin;

            AssetConfigInfo[] asset_config = _ContentApi.GetAssetMgtConfigInfo();
            if (asset_config[10].Value.IndexOf("ektron.com") > -1)
            {
                ltrCheckPdfServiceProvider.Text = MessageHelper.GetMessage("pdf service provider");
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

        if (_Type == "site")
        {
            lblSiteAlias.Visible = true;
            phSiteAlias.Visible = true;
            phSiteAlias2.Visible = true;
        }
        else
        {
            phSiteAlias2.Visible = false;
            phSiteAlias.Visible = false;
            lblSiteAlias.Visible = false;
        }


        _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
        AddFolderToolBar();

        template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        xmlconfig_data = _ContentApi.GetAllXmlConfigurations("title");
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));

        if (_FolderData.StyleSheet == "")
        {
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id);
        }
        TemplateData folder_template_data;
        folder_template_data = _ContentApi.GetTemplatesByFolderId(_Id); //VERIFY for dimension
        if (strFolderAddType == "site")
        {
            tdfoldernamelabel.InnerHtml = MessageHelper.GetMessage("sitename label");
        }
        else
        {
            tdfoldernamelabel.InnerHtml = MessageHelper.GetMessage("foldername label");
        }

        tdsitepath.InnerHtml = _SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\"" + " value=\"\" name=\"stylesheet\">";
        tdsitepath.InnerHtml += "<div class=\"ektronCaption\">" + MessageHelper.GetMessage("leave blank to inherit msg") + "</div>";
        ltrTemplateFilePath.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";
        ltrTemplateFilePath.Text += "<div id=\"FrameContainer\" class=\"ektronWindow ektronModalStandard\">";
        ltrTemplateFilePath.Text += "<iframe id=\"ChildPage\" name=\"ChildPage\">";
        ltrTemplateFilePath.Text += "</iframe>";
        ltrTemplateFilePath.Text += "</div>";

        if (_FolderData.PublishPdfEnabled)
        {
            phPDF.Visible = true;
            _IsPublishedAsPdf = (string)(_FolderData.PublishPdfActive ? "checked=\"checked\" " : string.Empty);
            this.lblPublishAsPdf.InnerText = MessageHelper.GetMessage("publish as pdf");
        }
        else
        {
            _IsPublishedAsPdf = string.Empty;
            phPDF.Visible = false;
        }

        // only top level folders can be domain folders
        CommonApi m_refCommonAPI = new CommonApi();
        Ektron.Cms.Common.EkRequestInformation request_info = m_refCommonAPI.RequestInformationRef;

        if (_Id == 0 && strFolderAddType == "site")
        {
            if (LicenseManager.IsSiteLimitReached(request_info))
            {
                Utilities.ShowError(MessageHelper.GetMessage("com: max sites reached"));
            }
            if (LicenseManager.IsFeatureEnable(request_info, Feature.MultiSite))
            {
                phProductionDomain.Visible = true;
                tdstagingdomain.InnerHtml = "http://&nbsp;<input type=\"text\" size=\"68\" name=\"DomainStaging\" id=\"DomainStaging\" size=\"50\" value=\"" + _FolderData.DomainStaging + "\">";
                tdproductiondomain.InnerHtml = "http://&nbsp;<input type=\"text\" size=\"68\" name=\"DomainProduction\" id=\"DomainProduction\" size=\"50\" value=\"" + _FolderData.DomainProduction + "\">";
                tdproductiondomain.InnerHtml += "<input type=\"hidden\" name=\"IsDomainFolder\" id=\"IsDomainFolder\" value=\"true\"/>";

            }
        }
        if (ShowTaxonomy)
        {
            DrawFolderTaxonomyTable();
        }
        // handle dynamic replication settings
        if (request_info.EnableReplication)
        {
            string schk = "";
            if (_FolderData.ReplicationMethod == 1)
            {
                schk = " checked";
            }

            if (!(Request.QueryString["type"] != "" && Request.QueryString["type"].ToLower() == "communityfolder"))
            {
                // see if we should warn users about inherited metadata
                bool fWarnMeta = true;
                if (_FolderData.ReplicationMethod == 1)
                {
                    // parent folder is QD enabled, so no need for warning
                    fWarnMeta = false;
                }

                ReplicationMethod.Text = "<tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\" class=\"label\">" + MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr>";
                ReplicationMethod.Text += "<tr><td colspan=\"2\"><input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\"" + schk;
                if (fWarnMeta)
                {
                    ReplicationMethod.Text += " onclick=\"if (!document.getElementById(\'break_inherit_button\').checked && document.getElementById(\'EnableReplication\').checked) {alert(\'";
                    ReplicationMethod.Text += MessageHelper.GetMessage("js: alert qd metainherit");
                    ReplicationMethod.Text += "\');}\"";
                }

                ReplicationMethod.Text += " ><label for=\"EnableReplication\">" + MessageHelper.GetMessage("replicate folder contents") + "</label></td></tr>";
            }

        }

        ParentID.Value = Convert.ToString(_Id); ;
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx");
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id);
        DisplaySubscriptionInfo();
        DisplaySitemapPath();
        DisplaySiteAlias();
    }
    private void Display_AddCalendar()
    {
        Display_FolderJS();
        TemplateData[] template_data;
        string backup = "";
        string strStyleSheet = "";


        ltInheritSitemapPath.Text = MessageHelper.GetMessage("lbl inherit parent configuration");
        ltrTypes.Text = MessageHelper.GetMessage("Smart Forms txt");

        phSiteAlias2.Visible = false;
        phSiteAlias.Visible = false;
        lblSiteAlias.Visible = false;
        phTypes.Visible = false;
        ltr_vf_types.Visible = false;

        _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
        AddCalendarToolBar();

        template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));

        if (_FolderData.StyleSheet == "")
        {
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id);
        }
        TemplateData folder_template_data;
        folder_template_data = _ContentApi.GetTemplatesByFolderId(_Id); //VERIFY for dimension
        tdfoldernamelabel.InnerHtml = MessageHelper.GetMessage("calendarname label");

        tdsitepath.InnerHtml = _SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\"" + " value=\"\" name=\"stylesheet\">";
        tdsitepath.InnerHtml += "<div class=\"ektronCaption\">" + MessageHelper.GetMessage("leave blank to inherit msg") + "</div>";
        ltrTemplateFilePath.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";

        _IsPublishedAsPdf = string.Empty;
        phPDF.Visible = false;       
        // only top level folders can be domain folders
        CommonApi m_refCommonAPI = new CommonApi();
        Ektron.Cms.Common.EkRequestInformation request_info = m_refCommonAPI.RequestInformationRef;

        DrawFolderTaxonomyTable();
        // handle dynamic replication settings
        if (request_info.EnableReplication)
        {
            string schk = string.Empty;
            if (_FolderData.ReplicationMethod == 1)
            {
                schk = " checked";
            }
        }

        ParentID.Value = Convert.ToString(_Id);
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx");
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id);
        DisplaySubscriptionInfo();
        DisplaySitemapPath();
        DisplaySiteAlias();
        DrawFolderAliasesTable();
    }
    private long checktaxid = 0;
    private void DrawFolderAliasesTable()
    {
        bool _isManualAliasEnabled = true;
        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _aliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
            if (_aliasSettings.IsManualAliasEnabled)
            {
                inherit_alias_from.Value = Convert.ToString(_FolderData.AliasInheritedFrom);
                current_alias_required.Value = Convert.ToString(Convert.ToInt32(_FolderData.AliasRequired));
                StringBuilder sb = new StringBuilder();
                sb.Append("<input type=\"checkbox\" id=\"chkInheritAliases\" name=\"chkInheritAliases\" checked=\"checked\" onclick=\"InheritAliasedChanged('chkForceAliasing')\" /> " + _messageHelper.GetMessage("lbl inherit parent configuration"));
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
                sb.Append(_messageHelper.GetMessage("lbl manual alias required"));
                sb.Append(" </td></tr>");
                sb.Append("</table>");
                ltrFolderAliases.Text = ltrBlogAliases.Text = sb.ToString();
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
        phFolderAliases.Visible = phFolderAliases2.Visible = phBlogAlias.Visible = phBlogAliases2.Visible = _isManualAliasEnabled;
    }
    private void DrawFolderTaxonomyTable()
    {
        string categorydatatemplate = "<li><input type=\"checkbox\" id=\"taxlist\" onclick=\"ValidateCatSel(this)\" name=\"taxlist\" value=\"{0}\" {1} disabled/>{2}</li>";
        StringBuilder categorydata = new StringBuilder();
        string scatcheck = "";
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
                    _SelectedTaxonomyList = Convert.ToString(_FolderData.FolderTaxonomy[i].TaxonomyId);
                }
            }
        }
        _CurrentCategoryChecked = Convert.ToInt32(_FolderData.CategoryRequired);
        inherit_taxonomy_from.Value = Convert.ToString(_FolderData.TaxonomyInheritedFrom);

        current_category_required.Value = Convert.ToString(_CurrentCategoryChecked);
        if (_FolderData.CategoryRequired)
        {
            scatcheck = " checked ";
        }
        TaxonomyBaseData[] TaxArr = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content);
        bool parent_has_configuration = false;
        if ((TaxArr != null) && TaxArr.Length > 0)
        {
            categorydata.Append("<ul style=\"list-style:none;margin:0;\">");
            int i = 0;
            while (i < TaxArr.Length)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (i < TaxArr.Length)
                    {
                        checktaxid = TaxArr[i].TaxonomyId;
                        parent_has_configuration = Array.Exists(_FolderData.FolderTaxonomy, new Predicate<TaxonomyBaseData>(TaxonomyExists));
                        categorydata.Append(string.Format(categorydatatemplate, TaxArr[i].TaxonomyId, IsChecked(parent_has_configuration), TaxArr[i].TaxonomyName));
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            categorydata.Append("</ul>");
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

        str.Append("<input name=\"TaxonomyTypeBreak\" id=\"TaxonomyTypeBreak\" type=\"checkbox\" onclick=\"ToggleTaxonomyInherit(this)\" checked/>" + MessageHelper.GetMessage("lbl inherit parent configuration"));
        str.Append("<br />");
        str.Append("<input name=\"CategoryRequired\" id=\"CategoryRequired\" type=\"checkbox\" " + scatcheck + " disabled/>" + MessageHelper.GetMessage("alt Required at least one category selection"));
        str.Append("<br />");
        str.Append("<br />");
        str.Append(categorydata.ToString());
        taxonomy_list.Text = str.ToString();
        litBlogTaxonomy.Text = str.ToString();
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
    private void DisplaySitemapPath()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        sJS.AppendLine("var clientName_chkInheritSitemapPath = \'chkInheritSitemapPath\';");
        //chkInheritSitemapPath.Checked = True
        sJS.AppendLine("document.getElementById(\"hdnInheritSitemap\").value = \'true\';");
        sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").checked = true;");
        sJS.AppendLine("document.getElementById(\"AddSitemapNode\").style.display = \'none\';");

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
        //Dim sJS As New System.Text.StringBuilder
        //Dim node As Ektron.Cms.Common.SitemapPath
        //sJS.AppendLine("var clientName_chkInheritSitemapPath = 'chkInheritSitemapPath';")
        //'chkInheritSitemapPath.Checked = True
        //sJS.AppendLine("document.getElementById(""hdnInheritSitemap"").value = 'true';")
        //sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").checked = true;")

        //If (folder_data.SitemapPath IsNot Nothing) Then
        //    sJS.Append("arSitemapPathNodes = new Array(")
        //    For Each node In folder_data.SitemapPath
        //        If (node IsNot Nothing) Then
        //            If (node.Order <> 0) Then
        //                sJS.Append(",")
        //            End If
        //            sJS.Append("new node('" & node.Title & "','" & node.Url & "','" & node.Description & "'," & node.Order & ")")
        //        End If
        //    Next
        //    sJS.AppendLine(");")
        //    sJS.AppendLine("renderSiteMapNodes();")
        //End If
        //Page.ClientScript.RegisterStartupScript(Me.GetType(), "renderSitepath", sJS.ToString(), True)

    }
    private void IsContentSearchableSection()
    {
        //-------------IscontentSearchable-------------    
        inherit_IscontentSearchable_from.Value = Convert.ToString(_FolderData.IsContentSearchableInheritedFrom);
        current_IscontentSearchable.Value = Convert.ToString(Convert.ToInt32(_FolderData.IscontentSearchable));
        StringBuilder sb = new StringBuilder();
      //  sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _messageHelper.GetMessage("lbl Content Searchable") + ":</strong></td><td colspan=\"2\">");
        sb.Append("<input type=\"checkbox\" id=\"chkInheritIscontentSearchable\" name=\"chkInheritIscontentSearchable\" checked=\"checked\" onclick=\"InheritIscontentSearchableChanged('chkIscontentSearchable')\" /> " + _messageHelper.GetMessage("lbl inherit parent configuration"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        //sb.Append("</tr></td>");
        //sb.Append(" <tr class=\"evenrow\"><td colspan=\"2\">");
        if (_FolderData.IscontentSearchable)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\" name=\"chkIscontentSearchable\" id=\"chkIscontentSearchable\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsContentSearchable required"));
        sb.Append(" <div class=\"ektronCaption\">" + _messageHelper.GetMessage("Content Searchable warning") + "</div><div class=\"ektronCaption\">*" + _messageHelper.GetMessage("Content Searchable help") + "</div></td></tr>");
       // sb.Append("</table>");
        ltrContSearch.Text = ltrContSearch2.Text = ltrContSearch1.Text = sb.ToString();
        phContSearch.Visible = phContSearch1.Visible = phContSearch2.Visible = true;
        //--------------------IscontentSearchableEnd-------------
    }
    private void IsDisplaySettings()
    {
        //-------------IsDisplaySettings-------------    
        inherit_IsDisplaySettings_from.Value = Convert.ToString(_FolderData.DisplaySettingsInheritedFrom);
        current_IsDisplaySettings.Value = Convert.ToString(_FolderData.DisplaySettings);
        StringBuilder sb = new StringBuilder();
        //  sb.Append("<table class=\"ektronForm\">");
        sb.Append("<tr ><td class=\"label\"><strong>" + _messageHelper.GetMessage("lbl Display Settings") + ":</strong></td><td colspan=\"2\">");
        sb.Append("<input type=\"checkbox\" id=\"chkInheritIsDisplaySettings\" name=\"chkInheritIsDisplaySettings\" checked=\"checked\" onclick=\"InheritIsDisplaySettingsChanged('chkIsDisplaySettingsAllTabs')\" /> " + _messageHelper.GetMessage("lbl inherit parent configuration"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        //sb.Append("</tr></td>");
        //sb.Append(" <tr class=\"evenrow\"><td colspan=\"2\">");      
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs && _FolderData.DisplaySettings == 0)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAllTabs\" id=\"chkIsDisplaySettingsAllTabs\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsAllTabs required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Summary) == (int)EkEnumeration.FolderTabDisplaySettings.Summary)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSummary\" id=\"chkIsDisplaySettingsSummary\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsSummary required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsMetaData\" id=\"chkIsDisplaySettingsMetaData\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsMetaData required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsAliasing\" id=\"chkIsDisplaySettingsAliasing\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsAliasing required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Schedule) == (int)EkEnumeration.FolderTabDisplaySettings.Schedule)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsSchedule\" id=\"chkIsDisplaySettingsSchedule\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsSchedule required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Comment) == (int)EkEnumeration.FolderTabDisplaySettings.Comment)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\"  type=\"checkbox\" name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsComment\" id=\"chkIsDisplaySettingsComment\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsComment required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Templates) == (int)EkEnumeration.FolderTabDisplaySettings.Templates)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTemplates\" id=\"chkIsDisplaySettingsTemplates\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsTemplates required"));
        sb.Append("<div class=\"ektronTopSpace\"></div>");
        if ((_FolderData.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
        }
        else
        {
            sb.Append("&nbsp;     <input disabled=\"disabled\" type=\"checkbox\"  name=\"chkIsDisplaySettingsTaxonomy\" id=\"chkIsDisplaySettingsTaxonomy\" />");
        }
        sb.Append(_messageHelper.GetMessage("lbl IsDisplaySettingsTaxonomy required"));
        sb.Append(" <div class=\"ektronCaption\">" + _messageHelper.GetMessage("Display Settings warning") + "</div> <div class=\"ektronCaption\">*" + _messageHelper.GetMessage("Display Settings help") + "</div></td></tr>");
        // sb.Append("</table>");
        ltrDisplaySettings.Text = ltrDisplaySettings1.Text = ltrDisplaySettings2.Text = sb.ToString();
        phDisplaySettings.Visible = phDisplaySettings1.Visible = phDisplaySettings2.Visible = true;
        //--------------------IsDisplaySettingsEnd-------------
    }
    private void AddCalendarToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(MessageHelper.GetMessage("btn add calendar") + " \"" + _FolderData.Name + "\""));
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        
		result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("add calendar folder"), MessageHelper.GetMessage("add calendar folder"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckFolderParameters(\\\'add\\\')\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)("calendar_" + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void AddFolderToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(MessageHelper.GetMessage("add subfolder msg") + " \"" + _FolderData.Name + "\""));
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("alt add folder button text"), MessageHelper.GetMessage("btn add folder"), "onclick=\"return SubmitAddFolderForm(\'frmContent\', \'CheckFolderParameters(\\\'add\\\')\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void DisplaySubscriptionInfo()
    {
        int i = 0;
        int findindex;
        Array arrSubscribed = null;
        string strNotifyA = "";
        string strNotifyI = "";
        string strNotifyN = "";
        long intInheritFrom;
        string strEnabled = " ";
        EmailFromData[] emailfrom_list;
        EmailMessageData[] unsubscribe_list;
        EmailMessageData[] optout_list;
        EmailMessageData[] defaultmessage_list;
        int y = 0;
        SettingsData settings_list;
        SiteAPI m_refSiteAPI = new SiteAPI();

        emailfrom_list = _ContentApi.GetAllEmailFrom();
        optout_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut);
        defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage);
        unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe);
        _SubscriptionData = _ContentApi.GetAllActiveSubscriptions(); //then get folder
        intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id);
        settings_list = m_refSiteAPI.GetSiteVariables(-1);



        _SubscribedData = _ContentApi.GetSubscriptionsForFolder(intInheritFrom);
        _SubscriptionProperties = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom);

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
        lit_vf_subscription_properties.Text += "function breakWebAlertInheritance(obj){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   if(!obj.checked){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       if(confirm(\"" + MessageHelper.GetMessage("js: confirm break inheritance") + "\")){" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           enableSubCheckboxes();" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           obj.checked = !obj.checked;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "           return false;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   } else {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "       enableSubCheckboxes();" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "   }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function enableSubCheckboxes() {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    var idx, masterBtn, tableObj, enableFlag, qtyElements, displayUseContentBtns;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    tableObj = document.getElementById(\'therows\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    tableObj = tableObj.getElementsByTagName(\'input\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    enableFlag = false;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    masterBtn = document.getElementById(\'webalert_inherit_button\');" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    if (validateObject(masterBtn)){" + Environment.NewLine;
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
        lit_vf_subscription_properties.Text += "        $ektron(\'.selectContent\').css(\'display\', displayUseContentBtns);" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        $ektron(\'.useCurrent\').css(\'display\', displayUseContentBtns);" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[0].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[1].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.forms.frmContent.notify_option[2].disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_message_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_summary_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_content_button\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'use_contentlink_button\').disabled = !enableFlag;" + Environment.NewLine;
        //lit_vf_subscription_properties.Text += ("        document.getElementById('notify_url').disabled = !enableFlag;" & Environment.NewLine)
        //lit_vf_subscription_properties.Text += ("        document.getElementById('notify_weblocation').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_emailfrom\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_optoutid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_messageid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_unsubscribeid\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "        document.getElementById(\'notify_subject\').disabled = !enableFlag;" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "    }" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function validateObject(obj) {" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "     return ((obj != null) &&" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "         ((typeof(obj)).toLowerCase() != \'undefined\') &&" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "         ((typeof(obj)).toLowerCase() != \'null\'))" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "}" + Environment.NewLine;
        lit_vf_subscription_properties.Text += "function valAndSaveCSubAssignments() {" + Environment.NewLine;
        if ((!(_SubscriptionData == null)) && (!((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (settings_list.AsynchronousLocation == ""))))
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

        if ((emailfrom_list == null) || (defaultmessage_list == null) || (unsubscribe_list == null) || (optout_list == null) || (_SubscriptionData == null) || (settings_list.AsynchronousLocation == ""))
        {
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"suppress_notification\" value=\"true\">";
            lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl web alert not setup");
            lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";

            if (emailfrom_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("lbl web alert emailfrom not setup") + "</font><br/>";
            }
            if (defaultmessage_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("lbl web alert def msg not setup") + "</font><br/>";
            }
            if (unsubscribe_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("lbl web alert unsub not setup") + "</font><br/>";
            }
            if (optout_list == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("lbl web alert optout not setup") + "</font><br/>";
            }
            if (_SubscriptionData == null)
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("alt No subscriptions are enabled on the folder.") + "</font><br/>";
            }
            if (settings_list.AsynchronousLocation == "")
            {
                lit_vf_subscription_properties.Text += "<font class=\"ektronErrorText\">" + MessageHelper.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") + "</font>";
            }
            return;
        }

        if (_SubscriptionProperties == null)
        {
            _SubscriptionProperties = new SubscriptionPropertiesData();
        }
        strEnabled = " disabled=\"disabled\" ";
        switch (_SubscriptionProperties.NotificationType.GetHashCode())
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

        lit_vf_subscription_properties.Text += "<input id=\"webalert_inherit_button\" checked=\"checked\" onclick=\"breakWebAlertInheritance(this);\" type=\"checkbox\" name=\"webalert_inherit_button\" value=\"webalert_inherit_button\">" + MessageHelper.GetMessage("lbl inherit parent configuration");
        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";

        lit_vf_subscription_properties.Text += "<table class=\"ektronGrid\">";
        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl web alert opt") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Always\" name=\"notify_option\" " + strNotifyA + " " + strEnabled + "> " + MessageHelper.GetMessage("lbl web alert notify always");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Initial\" name=\"notify_option\"" + strNotifyI + " " + strEnabled + "> " + MessageHelper.GetMessage("lbl web alert notify initial");
        lit_vf_subscription_properties.Text += "<br />";
        lit_vf_subscription_properties.Text += "<input type=\"radio\" value=\"Never\" name=\"notify_option\"" + strNotifyN + " " + strEnabled + "> " + MessageHelper.GetMessage("lbl web alert notify never");
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl web alert subject") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        if (_SubscriptionProperties.Subject != "")
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"" + _SubscriptionProperties.Subject + "\" name=\"notify_subject\" id=\"notify_subject\" " + strEnabled + ">";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input type=\"text\" maxlength=\"255\" size=\"65\" value=\"\" name=\"notify_subject\" id=\"notify_subject\" " + strEnabled + ">";
        }
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        //lit_vf_subscription_properties.Text &= "Notification Base URL:"
        //If subscription_properties_list.URL <> "" Then
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """>/"
        //Else
        //    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """>/"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl web alert emailfrom address") + ":";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_emailfrom\" id=\"notify_emailfrom\">:";
        if ((emailfrom_list != null) && emailfrom_list.Length > 0)
        {
            for (y = 0; y <= emailfrom_list.Length - 1; y++)
            {
                lit_vf_subscription_properties.Text += "<option>" + emailfrom_list[y].Email + "</option>";
            }
        }
        lit_vf_subscription_properties.Text += "</select>";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "</tr>";

        //lit_vf_subscription_properties.Text &= "Notification File Location:"
        //If subscription_properties_list.WebLocation <> "" Then
        //lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & ">"
        //Else
        //    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & ">"
        //End If

        lit_vf_subscription_properties.Text += "<tr>";
        lit_vf_subscription_properties.Text += "<td class=\"label\">";
        lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl web alert contents") + ":";
        lit_vf_subscription_properties.Text += "<img src=\"" + _AppPath + "images/UI/Icons/preview.png\" alt=\"Preview Web Alert Message\" title=\"Preview Web Alert Message\" onclick=\" PreviewWebAlert(); return false;\" />";
        lit_vf_subscription_properties.Text += "</td>";
        lit_vf_subscription_properties.Text += "<td class=\"value\">";
        lit_vf_subscription_properties.Text += "<input id=\"use_optout_button\" type=\"checkbox\" checked=\"checked\" name=\"use_optout_button\" disabled=\"disabled\">" + MessageHelper.GetMessage("lbl optout message") + "&nbsp;&nbsp;";

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_optoutid\" id=\"notify_optoutid\">";
        if ((optout_list != null) && optout_list.Length > 0)
        {
            for (y = 0; y <= optout_list.Length - 1; y++)
            {
                if (optout_list[y].Id == _SubscriptionProperties.OptOutID)
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
        if (_SubscriptionProperties.DefaultMessageID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_message_button\" type=\"checkbox\" checked=\"checked\" name=\"use_message_button\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use default message") + "&nbsp;&nbsp;";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_message_button\" type=\"checkbox\" name=\"use_message_button\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use default message") + "&nbsp;&nbsp;";
        }

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_messageid\" id=\"notify_messageid\">";
        if ((defaultmessage_list != null) && defaultmessage_list.Length > 0)
        {
            for (y = 0; y <= defaultmessage_list.Length - 1; y++)
            {
                if (defaultmessage_list[y].Id == _SubscriptionProperties.DefaultMessageID)
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
        if (_SubscriptionProperties.SummaryID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" checked=\"checked\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use summary message");
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_summary_button\" type=\"checkbox\" name=\"use_summary_button\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use summary message");
        }
        lit_vf_subscription_properties.Text += "<br />";
        if (_SubscriptionProperties.ContentID == -1)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" value=\"[[use current]]\" " + strEnabled + " size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\">" + MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"-1\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }
        else if (_SubscriptionProperties.ContentID > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" checked=\"checked\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" value=\"" + _SubscriptionProperties.UseContentTitle.ToString() + "\" size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\">" + MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"" + _SubscriptionProperties.ContentID.ToString() + "\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_content_button\" type=\"checkbox\" name=\"use_content_button\" " + strEnabled + ">" + MessageHelper.GetMessage("lbl use content message") + "&nbsp;&nbsp;";
            lit_vf_subscription_properties.Text += "<input type=\"text\" id=\"titlename\" name=\"titlename\" onkeydown=\"return false\" value=\"\" size=\"65\" disabled=\"disabled\"/>";
            lit_vf_subscription_properties.Text += "<a href=\"#\" class=\"button buttonInline greenHover selectContent\" onclick=\" QuickLinkSelectBase(" + _Id.ToString() + ",\'frmContent\',\'titlename\',0,0,0,0) ;return false;\">" + MessageHelper.GetMessage("lbl use content select") + "</a><a href=\"#\" class=\"button buttonInline  blueHover useCurrent\" onclick=\" SetMessageContenttoDefault();return false;\">Use Current</a>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" maxlength=\"20\" id=\"frm_content_id\" name=\"frm_content_id\" value=\"0\" />";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_content_langid\"/>";
            lit_vf_subscription_properties.Text += "<input type=\"hidden\" name=\"frm_qlink\"/>";
        }

        lit_vf_subscription_properties.Text += "<br />";

        if (_SubscriptionProperties.UseContentLink > 0)
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" checked=\"checked\" " + strEnabled + ">Use Content Link";
        }
        else
        {
            lit_vf_subscription_properties.Text += "<input id=\"use_contentlink_button\" type=\"checkbox\" name=\"use_contentlink_button\" " + strEnabled + ">Use Content Link";
        }

        lit_vf_subscription_properties.Text += "<div class=\"ektronTopSpace\"></div>";
        lit_vf_subscription_properties.Text += "<input id=\"use_unsubscribe_button\" type=\"checkbox\" checked=\"checked\" name=\"use_unsubscribe_button\" disabled=\"disabled\">" + MessageHelper.GetMessage("lbl unsubscribe message") + "&nbsp;&nbsp;";

        lit_vf_subscription_properties.Text += "<select " + strEnabled + " name=\"notify_unsubscribeid\" id=\"notify_unsubscribeid\">";
        if ((unsubscribe_list != null) && unsubscribe_list.Length > 0)
        {
            for (y = 0; y <= unsubscribe_list.Length - 1; y++)
            {
                if (unsubscribe_list[y].Id == _SubscriptionProperties.UnsubscribeID)
                {
                    lit_vf_subscription_properties.Text += "<option value=\"" + unsubscribe_list[y].Id + "\" SELECTED>" + unsubscribe_list[y].Title + "</option>";
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
        lit_vf_subscription_properties.Text += MessageHelper.GetMessage("lbl avail web alert");
        lit_vf_subscription_properties.Text += "</div>";

        if (!(_SubscriptionData == null))
        {
            lit_vf_subscription_properties.Text += "<table id=\"cfld_subscription_assignment\" class=\"ektronGrid\" width=\"100%\">";
            lit_vf_subscription_properties.Text += "<tbody id=\"therows\">";
            lit_vf_subscription_properties.Text += "<tr class=\"title-header\">";
            lit_vf_subscription_properties.Text += "<th width=\"50\">" + MessageHelper.GetMessage("lbl assigned") + "</th>";
            lit_vf_subscription_properties.Text += "<th>" + MessageHelper.GetMessage("lbl name") + "</th>";
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
                    lit_vf_subscription_properties.Text += "<td class=\"center\" width=\"10%\"><input type=\"checkbox\" name=\"webalert_" + _SubscriptionData[i].Id + "\"  id=\"Assigned_" + _SubscriptionData[i].Id + "\" " + strEnabled + "></td>";
                }
                else
                {
                    lit_vf_subscription_properties.Text += "<td class=\"center\" width=\"10%\"><input type=\"checkbox\" name=\"webalert_" + _SubscriptionData[i].Id + "\"  id=\"Assigned_" + _SubscriptionData[i].Id + "\" checked=\"checked\" " + strEnabled + "></td>";
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
    }
    #endregion

    #region Catalog

    #region DoAddCatalog

    private void Process_DoAddCatalog()
    {
        string tmpPath;
        Collection libSettings;
        string FolderPath;
        Ektron.Cms.Content.EkContent m_refContent;
        SubscriptionPropertiesData sub_prop_data = new SubscriptionPropertiesData();
        Collection page_subscription_data = new Collection();
        Collection page_sub_temp = new Collection();
        Array arrSubscriptions;
        int isub = 0;

        m_refContent = _ContentApi.EkContentRef;
        _PageData = new Collection();
        _PageData.Add(true, "IsCatalog", null, null);
        _PageData.Add(Request.Form["foldername"].Trim(".".ToCharArray()), "FolderName", null, null);
        _PageData.Add(Request.Form["folderdescription"], "FolderDescription", null, null);
        _PageData.Add(_Id, "ParentID", null, null); //pagedata.Add(Request.Form("ParentID"), "ParentID")
        if (string.IsNullOrEmpty(Request.Form["TemplateTypeBreak"]))
        {
            _PageData.Add(Request.Form["templatefilename"], "TemplateFileName", null, null);
        }
        else
        {
            _PageData.Add("", "TemplateFileName", null, null);
        }
        _PageData.Add(Request.Form["stylesheet"], "StyleSheet", null, null);
        if ((Request.Form["hdnInheritSitemap"] != null) && (Request.Form["hdnInheritSitemap"].ToString().ToLower() == "true"))
        {
            _PageData.Add(true, "SitemapPathInherit", null, null);
        }
        else
        {
            _PageData.Add(false, "SitemapPathInherit", null, null);
        }
        _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, this._ContentLanguage), "SitemapPath", null, null);
        Ektron.Cms.Library.EkLibrary objLib;
        objLib = _ContentApi.EkLibraryRef;
        libSettings = objLib.GetLibrarySettingsv2_0();
        tmpPath = Convert.ToString(libSettings["ImageDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory", null, null);
        tmpPath = Convert.ToString(libSettings["FileDirectory"]);
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory", null, null);

        if (!string.IsNullOrEmpty(Request.Form["webalert_inherit_button"]))
        {
            sub_prop_data.BreakInheritance = false;
        }
        else
        {
            sub_prop_data.BreakInheritance = true;
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
        if (!string.IsNullOrEmpty(Request.Form["use_message_button"]))
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
                    page_sub_temp = new Collection();
                    page_sub_temp.Add(int.Parse(Strings.Mid(arrSubscriptions.GetValue(isub).ToString(), 10)), "ID", null, null);
                    page_subscription_data.Add(page_sub_temp, null, null, null);
                }
            }
        }
        else
        {
            page_subscription_data = null;
        }
        page_sub_temp = null;

        Utilities.AddLBpaths(_PageData);

        if (Strings.LCase(Request.Form["TypeBreak"]) == "on")
        {
            _PageData.Add(true, "XmlInherited", null, null);
        }
        else
        {
            _PageData.Add(false, "XmlInherited", null, null);
        }
        _PageData.Add(Request.Form["xmlconfig"], "XmlConfiguration", null, null);
        if (Request.Form["PublishActive"] != null)
        {
            if (Request.Form["PublishActive"] == "PublishPdfActive")
            {
                _PageData.Add(true, "PublishPdfActive", null, null);
                _PageData.Add(false, "PublishHtmlActive", null, null);
                //ElseIf (Request.Form("PublishActive") = "PublishHtmlActive") Then
                //   pagedata.Add(False, "PublishPdfActive")
                //  pagedata.Add(True, "PublishHtmlActive")
            }
        }
        else
        {
            _PageData.Add(false, "PublishPdfActive", null, null);
            _PageData.Add(false, "PublishHtmlActive", null, null);
        }

        // handle dynamic replication properties
        if (Request.Form["EnableReplication"] != null || Request.QueryString["type"] == "communityfolder")
        {
            _PageData.Add(Request.Form["EnableReplication"], "EnableReplication", null, null);
        }
        else
        {
            _PageData.Add(0, "EnableReplication", null, null);
        }

        if (string.IsNullOrEmpty(Request.Form["suppress_notification"]))
        {
            _PageData.Add(sub_prop_data, "SubscriptionProperties", null, null);
            _PageData.Add(page_subscription_data, "Subscriptions", null, null);
        }

        if ((Request.Form["break_inherit_button"] != null) && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
        {
            _PageData.Add(0, "break_inherit_button", null, null); //inherit button is checked => Metadata is inherited from parent.
        }
        else
        {
            _PageData.Add(1, "break_inherit_button", null, null); //break inheritance, do NOT inherit from parent
        }
        _PageData.Add(Request.Form["folder_cfld_assignments"], "folder_cfld_assignments", null, null);

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
        if ((Request.Form["break_inherit_button"] != null) && Request.Form["break_inherit_button"].ToString().ToLower() == "on")
        {
            _PageData.Add(1, "InheritMetadata", null, null); //break inherit button is check.
        }
        else
        {
            _PageData.Add(0, "InheritMetadata", null, null);
        }
        _PageData.Add(Request.Form["inherit_meta_from"], "InheritMetadataFrom", null, null);

        if ((!(Request.QueryString["type"] == null)) && (Request.QueryString["type"] == "communityfolder"))
        {
            _PageData.Add(true, "IsCommunityFolder", null, null);
            _PageData.Remove("XmlInherited");
            _PageData.Add(false, "XmlInherited", null, null);
            _PageData.Remove("XmlConfiguration");
            _PageData.Add(null, "XmlConfiguration", null, null);
        }
        if ((Request.Form["TaxonomyTypeBreak"] != null) && Request.Form["TaxonomyTypeBreak"].ToString().ToLower() == "on")
        {
            _PageData.Add(1, "InheritTaxonomy", null, null);
            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "CategoryRequired", null, null);
            }
            else
            {
                _PageData.Add(Request.Form[current_category_required.UniqueID], "CategoryRequired", null, null);
            }
        }
        else
        {
            _PageData.Add(0, "InheritTaxonomy", null, null);
            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _PageData.Add(1, "CategoryRequired", null, null);
            }
            else
            {
                _PageData.Add(0, "CategoryRequired", null, null);
            }
        }
        string IdRequests = "";
        if ((Request.Form["taxlist"] != null) && Request.Form["taxlist"] != null)
        {
            IdRequests = Request.Form["taxlist"];
        }
        _PageData.Add(IdRequests, "TaxonomyList", null, null);
        _PageData.Add(Request.Form[inherit_taxonomy_from.UniqueID], "InheritTaxonomyFrom", null, null);
        //-------------------ContentSearchable------------
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
        //-------------------ContentSearchableEnd------------------
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

        // Update - add flagging items:
        ProcessFlaggingPostBack(_PageData);
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
        m_refContent.AddContentFolderv2_0(ref _PageData);

        //_CustomFieldsApi.ProcessCustomFields(_PageData("FolderID"))

        FolderPath = m_refContent.GetFolderPath(Convert.ToInt64(_PageData["ParentID"]));
        if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
        {
            FolderPath = FolderPath.Substring(FolderPath.Length - (FolderPath.Length - 1));
        }
        FolderPath = FolderPath.Replace("\\", "\\\\");
        string close;
        close = Request.QueryString["close"];
        if (close == "true")
        {
            Response.Redirect("close.aspx", false);
        }
        else if (Request.Form[frm_callingpage.UniqueID] == "cmsform.aspx")
        {
            Response.Redirect((string)("cmsform.aspx?LangType=" + _ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }
        else
        {
            Response.Redirect((string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewContentByCategory&id=" + _Id + "&reloadtrees=Forms,Content,Library&TreeNav=" + FolderPath), false);
        }

        // find the folder_id we just created now...
        _FolderId = _ContentApi.EkContentRef.GetFolderID((string)(_ContentApi.EkContentRef.GetFolderPath(Convert.ToInt64(_PageData["ParentID"])) + "\\" + Request.Form["foldername"].Trim(".".ToCharArray())));
    }

    #endregion

    #region AddCatalog

    private void AddCatalogToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(string.Format(MessageHelper.GetMessage("lbl add catalog to"), _FolderData.Name));
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("alt add catalog button text"), MessageHelper.GetMessage("btn add catalog"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckFolderParameters(\\\'add\\\')\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("addcatalog", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Display_AddCatalog()
    {
        bool ShowTaxonomy = false;
        ShowTaxonomy = true;
        Display_FolderJS();
        string backup = "";
        string strStyleSheet = "";

        ltrTypes.Text = MessageHelper.GetMessage("lbl product types");
        ltInheritSitemapPath.Text = MessageHelper.GetMessage("lbl Inherit Parent Configuration");

        _FolderData = _ContentApi.GetFolderById(_Id, true, true);
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
        this.AddCatalogToolBar();

        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));

        if (_FolderData.StyleSheet == "")
        {
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id);
        }

        tdfoldernamelabel.InnerHtml = MessageHelper.GetMessage("catalogname label");

        tdsitepath.InnerHtml = _SitePath + "<input type=\"text\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\"" + " value=\"\" name=\"stylesheet\">";
        tdsitepath.InnerHtml += "<div class=\"ektronCaption\">" + MessageHelper.GetMessage("leave blank to inherit msg") + "</div>";
        ltrTemplateFilePath.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";

        Ektron.Cms.Common.EkRequestInformation request_info = _ContentApi.RequestInformationRef;

        if (ShowTaxonomy)
        {
            DrawFolderTaxonomyTable();
        }
        if (request_info.EnableReplication)
        {
            string schk = "";
            if (_FolderData.ReplicationMethod == 1)
            {
                schk = " checked";
            }

            if (!(Request.QueryString["type"] != "" && Request.QueryString["type"].ToLower() == "communityfolder"))
            {
                bool fWarnMeta = true;
                if (_FolderData.ReplicationMethod == 1)
                {
                    fWarnMeta = false;
                }

                ReplicationMethod.Text = "<tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\" class=\"label\">" + MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr>";
                ReplicationMethod.Text += "<tr><td colspan=\"2\"><input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\"" + schk;
                if (fWarnMeta)
                {
                    ReplicationMethod.Text += " onclick=\"if (!document.getElementById(\'break_inherit_button\').checked && document.getElementById(\'EnableReplication\').checked) {alert(\'";
                    ReplicationMethod.Text += MessageHelper.GetMessage("js: alert qd metainherit");
                    ReplicationMethod.Text += "\');}\"";
                }
                ReplicationMethod.Text += " >&nbsp;<label for=\"EnableReplication\">" + MessageHelper.GetMessage("replicate folder contents") + "</label></td></tr>";
            }
        }

        ParentID.Value = Convert.ToString(_Id);
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx");
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id, Ektron.Cms.Common.EkEnumeration.FolderType.Catalog);
        DisplaySubscriptionInfo();
        DisplaySitemapPath();
    }

    private void Display_CatalogJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbJS.Append("document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbJS.Append("document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;" + Environment.NewLine);
        sbJS.Append("Ektron.ready(function() { document.forms[0].foldername.focus();" + Environment.NewLine);
        sbJS.Append("   if( $ektron(\'#webalert_inherit_button\').length > 0 ){ " + Environment.NewLine);
        sbJS.Append("       if( $ektron(\'#webalert_inherit_button\')[0].checked ){ " + Environment.NewLine);
        sbJS.Append("           $ektron(\'.selectContent\').css(\'display\', \'none\');" + Environment.NewLine);
        sbJS.Append("           $ektron(\'.useCurrent\').css(\'display\', \'none\');" + Environment.NewLine);
        sbJS.Append("       } " + Environment.NewLine);
        sbJS.Append("    } " + Environment.NewLine);
        sbJS.Append("});" + Environment.NewLine);
        sbJS.Append(" function PreviewSelectedProductType(sitepath,width,height) ").Append(Environment.NewLine);
        sbJS.Append(" { ").Append(Environment.NewLine);
        sbJS.Append(" 	var templar = document.getElementById(\"addContentType\") ").Append(Environment.NewLine);
        sbJS.Append(" 	if (templar.value != -1) { ").Append(Environment.NewLine);
        sbJS.Append(" 		PopUpWindow(\'commerce/producttypes.aspx?LangType=\'+jsContentLanguage+\'&action=viewproducttype&id=\' + templar.options[templar.selectedIndex].value + \'&caller=content\', \'Preview\', 700, 540, 1, 0); ").Append(Environment.NewLine);
        sbJS.Append(" 	} else { ").Append(Environment.NewLine);
        sbJS.Append(" 		alert(\'").Append(MessageHelper.GetMessage("js select valid prod type")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append(" 	} ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);
        sbJS.Append(" function PreviewProductTypeByID(xml_id) { ").Append(Environment.NewLine);
        sbJS.Append("   if (xml_id != 0) { ").Append(Environment.NewLine);
        sbJS.Append("       PopUpWindow(\'commerce/producttypes.aspx?LangType=\'+jsContentLanguage+\'&action=viewproducttype&id=\' + xml_id + \'&caller=content\', \'Preview\', 700, 540, 1, 0); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);
        ltr_af_js.Text = sbJS.ToString();
    }

    private void ProcessProductTemplatesPostBack(string type)
    {
        _ProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);

        string IsInheritingTemplates = Request.Form["TemplateTypeBreak"];
        string IsInheritingXml = Request.Form["TypeBreak"];
        List<Ektron.Cms.Commerce.ProductTypeData> prod_type_list = new List<Ektron.Cms.Commerce.ProductTypeData>();
        Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();

        prod_type_list = _ProductType.GetList(criteria);
        int default_template_id = 0;
        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        int i = 0;
        Collection active_prod_list = new Collection();
        Collection template_active_list = new Collection();
        long default_xml_id = -1;
        if (!string.IsNullOrEmpty(Request.Form[txt_adb_boardname.UniqueID]))
        {
            for (i = 0; i <= template_data.Length - 1; i++)
            {
                if (Convert.ToInt64(Request.Form["addTemplate"]) == template_data[i].Id)
                {
                    template_active_list.Add(template_data[i].Id, Convert.ToString(template_data[i].Id), null, null);
                }
            }
        }
        else
        {
            if (IsInheritingTemplates == null)
            {
                for (i = 0; i <= template_data.Length - 1; i++)
                {
                    if (!(Request.Form["tinput_" + template_data[i].Id] == null))
                    {
                        template_active_list.Add(template_data[i].Id, Convert.ToString(template_data[i].Id), null, null);
                    }
                }
            }
        }
        if (IsInheritingXml == null)
        {
            for (i = 0; i <= prod_type_list.Count - 1; i++)
            {
                if (!(Request.Form["input_" + prod_type_list[i].Id] == null))
                {
                    active_prod_list.Add(prod_type_list[i].Id, Convert.ToString(prod_type_list[i].Id), null, null);
                }
            }

            if (!(Request.Form["sfdefault"] == null))
            {
                default_xml_id = Convert.ToInt64(Request.Form["sfdefault"]);
            }

            if (Request.Form["requireSmartForms"] == null)
            {
                if (!active_prod_list.Contains("0"))
                {
                    active_prod_list.Add("0", "0", null, null);
                }
            }
        }
        if (type == "forum")
        {
            if ((Request.Form["addTemplate"] != null) && Request.Form["addTemplate"] != "")
            {
                default_template_id = Convert.ToInt32(Request.Form["addTemplate"]);
            }
            _ContentApi.UpdateForumFolderMultiConfig(_FolderId, default_xml_id, default_template_id, template_active_list, active_prod_list);
        }
        else
        {
            _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, active_prod_list);
        }
    }

    #endregion

    #region product type selection

    private void DrawProductTypesTable()
    {
        _ProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);

        List<Ektron.Cms.Commerce.ProductTypeData> prod_type_list = new List<Ektron.Cms.Commerce.ProductTypeData>();
        Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();

        criteria.PagingInfo = new PagingInfo(10000);
        prod_type_list = _ProductType.GetList(criteria);

        List<Ektron.Cms.Commerce.ProductTypeData> active_prod_list;
        active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id);

        Collection addNew = new Collection();
        int k = 0;
        int row_id = 0;

        bool smartFormsRequired = true;


        bool isParentCatalog = System.Convert.ToBoolean(!(_ContentApi.EkContentRef.GetFolderType(_FolderData.Id) == Ektron.Cms.Common.EkEnumeration.FolderType.Catalog));
        bool isInheriting = System.Convert.ToBoolean(!isParentCatalog);
        bool isEnabled = System.Convert.ToBoolean(!isInheriting);

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawProductTypesBreaker(isInheriting, isParentCatalog));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append(DrawProductTypesHeader());
        Collection ActiveXmlIdList = new Collection();
        for (k = 0; k <= active_prod_list.Count - 1; k++)
        {
            if (!ActiveXmlIdList.Contains(Convert.ToString(active_prod_list[k].Id)))
            {
                ActiveXmlIdList.Add(active_prod_list[k].Id, Convert.ToString(active_prod_list[k].Id), null, null);
            }
        }
        if (_FolderData.XmlConfiguration != null)
        {
            for (int j = 0; j <= (_FolderData.XmlConfiguration.Length - 1); j++)
            {
                if (!ActiveXmlIdList.Contains(Convert.ToString(_FolderData.XmlConfiguration[j].Id)))
                {
                    ActiveXmlIdList.Add(_FolderData.TemplateId, Convert.ToString(_FolderData.TemplateId), null, null);
                }
            }
        }


        for (k = 0; k <= prod_type_list.Count - 1; k++)
        {
            if (ActiveXmlIdList.Contains(Convert.ToString(prod_type_list[k].Id)))
            {

                str.Append(DrawProductTypesEntry(row_id, Convert.ToString(prod_type_list[k].Title), prod_type_list[k].Id, Utilities.IsDefaultXmlConfig(prod_type_list[k].Id, active_prod_list.ToArray()), isEnabled));
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
            str.Append(DrawProductTypesEntry(row_id, MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_prod_list.ToArray()), isEnabled));
        }

        str.Append("</tbody></table>");
        str.Append("</div");

        str.Append("<div class=\"ektronTopSpace\"></div>");
        str.Append("<table>");
        str.Append("<tbody>");
        str.Append("<tr>");
        str.Append("<td>");
        str.Append("<select name=\"addContentType\" id=\"addContentType\" " + (isEnabled ? "" : " disabled ") + ">");
        str.Append("<option value=\"-1\">" + "[" + MessageHelper.GetMessage("lbl select prod type") + "]" + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["xml_id"] + "\">" + row["xml_name"] + "</option>");
        }

        str.Append("</select>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' >");
        str.Append("<a href=\"#\" onclick=\"PreviewSelectedProductType(\'" + _ContentApi.SitePath + "\', 800,600);return false;\">");
        str.Append("<img src=\"" + _AppPath + "images/UI/Icons/preview.png" + "\" alt=\"" + MessageHelper.GetMessage("lbl Preview prod type") + "\" title=\"" + MessageHelper.GetMessage("lbl Preview prod type") + "\">");
        str.Append("</a>");
        str.Append("</span>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' >");
        str.Append("<a href=\"javascript:ActivateContentType(true);\">");
        str.Append("<img src=\"" + _AppPath + "images/UI/Icons/add.png" + "\" alt=\"" + MessageHelper.GetMessage("add title") + "\" title=\"" + MessageHelper.GetMessage("add title") + "\" border=\"0\" />");
        str.Append("</a>");
        str.Append("</span>");
        str.Append("</td>");
        str.Append("</tr>");
        str.Append(DrawContentTypesFooter());
        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }

        str.Append("<div style=\'display:none;\'>");
        if (smartFormsRequired)
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onclick=\"ToggleRequireSmartForms()\" checked>");
        }
        else
        {
            str.Append("<input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onclick=\"ToggleRequireSmartForms()\">");
        }

        str.Append(MessageHelper.GetMessage("lbl require prod types"));
        str.Append("</div>");
        ltr_vf_types.Text = str.ToString();
    }
    private string DrawProductTypesBreaker(bool @checked, bool IsParentCatalog)
    {
        if (IsParentCatalog)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" disabled autocomplete=\'off\' />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" checked=\'checked\' autocomplete=\'off\' />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleProductTypesInherit(\'TypeBreak\', this)\" autocomplete=\'off\' />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }

    }
    private string DrawProductTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<div>");
        str.Append("<table class=\"ektronGrid\" width=\"100%\"><tbody>");
        str.Append("<tr class=\"title-header\">");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append(MessageHelper.GetMessage("lbl default") + "</td>");
        str.Append("<td width=\"70%\" class=\"center\">");
        str.Append(MessageHelper.GetMessage("lbl prod type") + "</td>");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append(MessageHelper.GetMessage("lbl action") + "</td>");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append("&nbsp;</td>");
        str.Append("</tr>");
        str.Append("</tbody></table>");
        str.Append("<table width=\"100%\" class=\"ektronGrid\"><tbody id=\"contentTypeTable\">");
        return str.ToString();
    }

    private string DrawProductTypesEntry(int row_id, string name, long xml_id, bool isDefault, bool isEnabled)
    {
        StringBuilder str = new StringBuilder();


        str.Append("<tr id=\"row_" + xml_id + "\">");
        str.Append("<td class=\"center\" width=\"10%\">");
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
            str.Append("<td class=\"center\" width=\"10%\"><span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' ><a class=\"button greenHover minHeight buttonSearch\" href=\"javascript:PreviewProductTypeByID(" + xml_id + ")\">View</a></span></td>");
        }
        else
        {
            str.Append("<td class=\"center\" width=\"10%\">&nbsp;</td>");
        }

        str.Append("<td align=\"right\" width=\"10%\"><span class=\'hiddenWhenInheriting\' style=\'display:" + (isEnabled ? "inline;" : "none;") + "\' ><a class=\"button greenHover minHeight buttonRemove\" href=\"javascript:RemoveContentType(" + xml_id + ", \'" + name + "\')\">" + MessageHelper.GetMessage("btn remove") + "</a></span></td>");
        str.Append("</tr>");

        return str.ToString();
    }

    #endregion

    #endregion

    #region AddBlog

    private void AddBlogToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("\"" + string.Format(MessageHelper.GetMessage("lbl add a blog to folder x"), _FolderData.Name) + "\"");
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("alt add folder button text"), MessageHelper.GetMessage("btn add blog"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckBlogParameters()\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("AddBlogs", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Display_AddBlog()
    {
        phFolder.Visible = false;
        pnlFolder.Visible = false;
        phBlog.Visible = true;
        pnlBlog.Visible = true;

        ltrSelectDefTemp.Text = MessageHelper.GetMessage("js:alert select default template");

        hdnfolderid.Value = _Id.ToString();

        _FolderData = _ContentApi.GetFolderById(_Id, true);

        AddBlogToolBar();

        if (_FolderData.PrivateContent == true)
        {
            drpVisibility.SelectedIndex = 1;
        }

        ltr_ab_cat.Text += "<div id=\"parah\">";
        ltr_ab_cat.Text += "</div>";
        ltr_ab_cat.Text += "<a href=\"javascript:addInput()\" class=\"button buttonInlineBlock greenHover buttonAdd\">" + this.MessageHelper.GetMessage("lnk add new subject") + "</a><a href=\"javascript:deleteInput()\" class=\"button buttonInlineBlock redHover buttonRemove\">" + this.MessageHelper.GetMessage("lnk remove last subject") + "</a>";
        ltr_ab_cat.Text += "<input type=\"hidden\" id=\"categorylength\" name=\"categorylength\" value=\"0\" />";

        Literal ltrT = new Literal();

        ltrT.Text = "<div id=\"proll\" name=\"proll\">";
        ltrT.Text += "</div>";
        ltrT.Text += "<input type=\"hidden\" id=\"rolllength\" name=\"rolllength\" value=\"0\" />";
        ltrT.Text += "<a href=\"javascript:addRoll()\" class=\"button buttonInlineBlock greenHover buttonAdd\">" + MessageHelper.GetMessage("lbl add blog roll link") + "</a><a href=\"javascript:deleteRoll()\" class=\"button buttonInlineBlock redHover buttonRemove\">" + this.MessageHelper.GetMessage("lnk remove last subject") + "</a>";
        litBlogTemplatedata.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";
        // handle dynamic replication settings
        CommonApi m_refCommonAPI = new CommonApi();
        Ektron.Cms.Common.EkRequestInformation request_info = m_refCommonAPI.RequestInformationRef;
        if (request_info.EnableReplication)
        {
            if (_FolderData.FolderType == 6)
            {
                // community folder, so just hide a hidden field w/ checkbox enabled
                BlogEnableReplication.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"1\" />";
            }
            else
            {
                tr_enableblogreplication.Visible = true;
                BlogEnableReplication.Text = "<input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\" />";
                BlogEnableReplication.Text += "<label for=\"EnableReplication\">" + MessageHelper.GetMessage("replicate folder contents") + "</label>";
            }
        }
        DrawContentTemplatesTable();
        DrawFolderTaxonomyTable();

        lbl_ab_roll.Controls.Add(ltrT);
    }

    private void Display_BlogJS()
    {
        StringBuilder sbblogjs = new StringBuilder();
        //document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;
        ////document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;
        ////document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;
        sbblogjs.Append(AJAXcheck(GetResponseString("VerifyBlog"), "action=existingfolder&pid=" + _Id.ToString() + "&fname=\' + input + \'")).Append(Environment.NewLine);
        sbblogjs.Append(Environment.NewLine);

        sbblogjs.Append("Ektron.ready(function(){document.forms[0]." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".focus();});" + Environment.NewLine);
        sbblogjs.Append(Environment.NewLine + Environment.NewLine);
        sbblogjs.Append("function UpdateBlogCheckBoxes() {" + Environment.NewLine);
        sbblogjs.Append("   if (document.forms[0]." + Strings.Replace((string)chkEnable.UniqueID, "$", "_", 1, -1, 0) + ".checked == true) {" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkModerate.UniqueID, "$", "_", 1, -1, 0) + ".disabled = false;" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkRequire.UniqueID, "$", "_", 1, -1, 0) + ".disabled = false;" + Environment.NewLine);
        sbblogjs.Append("   } else {" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkModerate.UniqueID, "$", "_", 1, -1, 0) + ".disabled = true;" + Environment.NewLine);
        sbblogjs.Append("       document.forms[0]." + Strings.Replace((string)chkRequire.UniqueID, "$", "_", 1, -1, 0) + ".disabled = true;" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        sbblogjs.Append(Environment.NewLine + Environment.NewLine);
        sbblogjs.Append("function CheckBlogParameters() {" + Environment.NewLine);
        sbblogjs.Append("    var stext = Trim(document.getElementById(\'" + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + "\').value);" + Environment.NewLine);
        sbblogjs.Append("    checkName(stext,\'\'); " + Environment.NewLine);
        sbblogjs.Append("    TemplateConfigSave();").Append(Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        sbblogjs.Append("function VerifyBlog() {" + Environment.NewLine);

        sbblogjs.Append("   document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbblogjs.Append("   if ((document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("   	alert(\"" + _messageHelper.GetMessage("js:add blog no name msg") + "\");" + Environment.NewLine);
        sbblogjs.Append("   	ShowPane(\'dvProperties\');" + Environment.NewLine);
        sbblogjs.Append("   	document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbblogjs.Append("   	return false;" + Environment.NewLine);
        sbblogjs.Append("   }else if ((document.forms.frmContent." + Strings.Replace((string)txtTitle.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("   	ShowPane(\'dvProperties\');" + Environment.NewLine);
        sbblogjs.Append("   	alert(\"" + _messageHelper.GetMessage("js:add blog no title msg") + "\");" + Environment.NewLine);
        sbblogjs.Append("   	document.forms.frmContent." + Strings.Replace((string)txtTitle.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbblogjs.Append("   	return false;" + Environment.NewLine);
        sbblogjs.Append("   }else {" + Environment.NewLine);
        sbblogjs.Append("   	if (!CheckBlogForillegalChar()) {" + Environment.NewLine);
        sbblogjs.Append("   		return false;" + Environment.NewLine);
        sbblogjs.Append("   	}" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("   if(checkForDefaultTemplate() == false) { return false;}" + Environment.NewLine);
        sbblogjs.Append("   var regexp1 = /\"/gi;" + Environment.NewLine);
        sbblogjs.Append("   document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbblogjs.Append("	SubmitForm(\'frmContent\',\'true\'); return true;" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        sbblogjs.Append("function CheckBlogForillegalChar() {" + Environment.NewLine);
        sbblogjs.Append("   var val = document.forms.frmContent." + Strings.Replace((string)txtBlogName.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbblogjs.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("       alert(\"" + this.MessageHelper.GetMessage("js alert blog name cant include") + " (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbblogjs.Append("       return false;" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" + Environment.NewLine);
        sbblogjs.Append("   {" + Environment.NewLine);
        sbblogjs.Append("       val = Trim(arrInputValue[j]);" + Environment.NewLine);
        sbblogjs.Append("       if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbblogjs.Append("       {" + Environment.NewLine);
        sbblogjs.Append("           alert(\"" + this.MessageHelper.GetMessage("alert subject name") + " (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')\");" + Environment.NewLine);
        sbblogjs.Append("           return false;" + Environment.NewLine);
        sbblogjs.Append("       }" + Environment.NewLine);
        sbblogjs.Append("       else if (val.length == 0) {" + Environment.NewLine);
        sbblogjs.Append("           alert(\"" + this.MessageHelper.GetMessage("alert blank subject name") + "\");" + Environment.NewLine);
        sbblogjs.Append("           return false;" + Environment.NewLine);
        sbblogjs.Append("       }" + Environment.NewLine);
        sbblogjs.Append("   }" + Environment.NewLine);
        sbblogjs.Append("   return true;" + Environment.NewLine);
        sbblogjs.Append("}" + Environment.NewLine);
        ltr_af_js.Text = sbblogjs.ToString();
    }
    #endregion

    #region AddDiscussionBoard

    private void Display_AddDiscussionBoard()
    {
        phFolder.Visible = false;
        pnlFolder.Visible = false;
        phDiscussionBoard.Visible = true;
        pnlDiscussionBoard.Visible = true;

        hdn_adb_folderid.Value = _Id.ToString();

        _FolderData = _ContentApi.GetFolderById(_Id);

        AddDiscussionBoardToolBar();

        chk_adb_mc.Visible = false;
        // handle dynamic replication settings
        if (_ContentApi.RequestInformationRef.EnableReplication)
        {
            if (_FolderData.IsCommunityFolder)
            {
                // parent folder is a community folder, so always enable replication for this board
                ltr_dyn_repl.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"1\" />";
            }
            else
            {
                string schk = "";
                if (_FolderData.ReplicationMethod == 1)
                {
                    schk = " checked";
                }
                ltr_dyn_repl.Text = "<td class=\"label\">" + MessageHelper.GetMessage("lbl folderdynreplication") + "</td>";
                ltr_dyn_repl.Text += "<td class=\"value\">";
                ltr_dyn_repl.Text += "<input type=\"checkbox\" name=\"EnableReplication\" id=\"EnableReplication\" value=\"1\"" + schk + " ><label for=\"EnableReplication\">" + MessageHelper.GetMessage("replicate folder contents") + "</label>";
                ltr_dyn_repl.Text += "</td>";
            }
        }
        ltr_adb_cat.Text += "<div class=\"clearfix\">";

        ltr_adb_cat.Text += "<fieldset>";
        ltr_adb_cat.Text += "<legend>" + MessageHelper.GetMessage("subjects text") + "<span class=\"required\">*</span></legend>";
        ltr_adb_cat.Text += "<div id=\"parah\">";
        ltr_adb_cat.Text += "<p style=\"color:silver;padding:.25em;margin:0;\">"+MessageHelper.GetMessage("Generic Subjects Added")+"</p>";
        ltr_adb_cat.Text += "</div>";
        ltr_adb_cat.Text += "</fieldset>";
        ltr_adb_cat.Text += "<p class=\"required\">* " + MessageHelper.GetMessage("generic required field") + "</p>";
        ltr_adb_cat.Text += "<ul class=\"buttonWrapperLeft\">";
        ltr_adb_cat.Text += "<li>";
        ltr_adb_cat.Text += "<a href=\"#AddSubject\" title=\"" + MessageHelper.GetMessage("lnk Add New subject") + "\" onclick=\"addInput();return false;\" class=\"button buttonLeft greenHover buttonAdd\">" + MessageHelper.GetMessage("lnk Add New subject") + "</a>";
        ltr_adb_cat.Text += "</li>";
        ltr_adb_cat.Text += "</ul>";
        ltr_adb_cat.Text += "</div>";
        ltr_adb_cat.Text += "<input type=\"hidden\" id=\"categorylength\" name=\"categorylength\" value=\"0\" />";
        //css
        ltr_sitepath.Text = _ContentApi.SitePath;
        string sJustAppPath = _ContentApi.AppPath.Replace(_ContentApi.SitePath, "");
        if (sJustAppPath.Length > 0 && !(sJustAppPath[sJustAppPath.Length - 1].ToString() == "/"))
        {
            sJustAppPath = sJustAppPath + "/";
        }
        txt_adb_stylesheet.Text = sJustAppPath + "threadeddisc/themes/graysky/graysky.css";
        drp_theme.Attributes.Add("onchange", "updatetheme();");
        drp_theme.Attributes.Add("style", "width:20%");
        drp_theme.Items.Add(new ListItem("Select a theme", ""));
        drp_theme.Items.Add(new ListItem("Standard", sJustAppPath + "threadeddisc/themes/standard.css"));
        drp_theme.Items.Add(new ListItem("Chrome", sJustAppPath + "threadeddisc/themes/chrome.css"));
        drp_theme.Items.Add(new ListItem("Cool", sJustAppPath + "threadeddisc/themes/cool.css"));
        drp_theme.Items.Add(new ListItem("GraySky", sJustAppPath + "threadeddisc/themes/graysky/graysky.css"));
        drp_theme.Items.Add(new ListItem("Jungle", sJustAppPath + "threadeddisc/themes/jungle.css"));
        drp_theme.Items.Add(new ListItem("Modern", sJustAppPath + "threadeddisc/themes/modern.css"));
        drp_theme.Items.Add(new ListItem("Royal", sJustAppPath + "threadeddisc/themes/royal.css"));
        drp_theme.Items.Add(new ListItem("Slate", sJustAppPath + "threadeddisc/themes/slate.css"));
        drp_theme.Items.Add(new ListItem("Techno", sJustAppPath + "threadeddisc/themes/techno.css"));
        //css
        //page template
        template_list_cat.Text = "<table>";
        template_list_cat.Text += "<tbody id=\"templateTable\">";
        template_list_cat.Text += "</tbody>";
        template_list_cat.Text += "</table>";
        template_list_cat.Text += "<table class=\"ektronGrid\">";
        template_list_cat.Text += "<tbody>";
        template_list_cat.Text += "<tr>";
        template_list_cat.Text += "<td class=\"label\">" + MessageHelper.GetMessage("generic template") + ":</td>";
        template_list_cat.Text += "<td class=\"value\">" + _ContentApi.SitePath + "<select name=\"addTemplate\" id=\"addTemplate\"><option value=\"0\">" + MessageHelper.GetMessage("generic select template") + "</option>";
        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");
        if (!(template_data == null) && template_data.Length > 0)
        {
            for (int i = 0; i <= (template_data.Length - 1); i++)
            {
                template_list_cat.Text += "<option value=\"" + template_data[i].Id + "\" >" + template_data[i].FileName + "</option>";
            }
        }
        template_list_cat.Text += "</select><span class=\"required\">*</span>";
        template_list_cat.Text += "</td>";
        template_list_cat.Text += "</tr>";
        template_list_cat.Text += "</tbody>";
        template_list_cat.Text += "</table>";
        template_list_cat.Text += "<p class=\"required\">* " + MessageHelper.GetMessage("generic required field") + "</p>";
        template_list_cat.Text += "<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"1\" />";
        template_list_cat.Text += "<div id=\"div3\" style=\"display: none;position: block;\"></div>";
        template_list_cat.Text += "<div id=\"contentidspan\" style=\"display: block;\">";
        template_list_cat.Text += "<a href=\"#PreviewSelectedTemplate\" class=\"button buttonInlineBlock blueHover buttonPreview\" onclick=\"PreviewTemplate(\'" + _ContentApi.SitePath + "\', 800,600);return false;\">";
        template_list_cat.Text += MessageHelper.GetMessage("lbl preview selected template") + "</a>"; ;
        template_list_cat.Text += "<a href=\"#AddTemplate\" class=\"button buttonInlineBlock greenHover buttonAdd\" onclick=\"LoadChildPage();return true;\">" + MessageHelper.GetMessage("lbl add template") + "</a>";
        template_list_cat.Text += "</div>";
        template_list_cat.Text += "<div id=\"FrameContainer\" class=\"ektronWindow ektronModalStandard ektronModalWidth-50\" style=\"margin-left:-25em !important;\">";
        template_list_cat.Text += "<iframe id=\"ChildPage\" name=\"ChildPage\" style=\"width:50em;\" frameborder=\"no\">";
        template_list_cat.Text += "</iframe>";
        template_list_cat.Text += "</div>";

        lit_ef_templatedata.Text = "<input type=\"hidden\" id=\"language\" value=\"" + this._ContentLanguage + "\" />";
        lit_ef_templatedata.Text += "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - _ContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";
        //page template
    }

    private void AddDiscussionBoardToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(MessageHelper.GetMessage("lbl add discussion board header") + " \"" + _FolderData.Name + "\"");
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("alt add folder button text"), MessageHelper.GetMessage("add discussion board"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckDiscussionBoardParameters()\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("AddDiscussionBoard", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Display_DiscussionBoardJS()
    {
        StringBuilder sbdiscussionboardjs = new StringBuilder();
        sbdiscussionboardjs.Append(AJAXcheck(GetResponseString("VerifyBoard"), "action=existingfolder&pid=" + _Id.ToString() + "&fname=\' + input + \'")).Append(Environment.NewLine);
        sbdiscussionboardjs.Append(Environment.NewLine);

        sbdiscussionboardjs.Append("Ektron.ready(function() {document.forms[0]." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".focus();});" + Environment.NewLine);
        sbdiscussionboardjs.Append(Environment.NewLine + Environment.NewLine);
        sbdiscussionboardjs.Append("function CheckDiscussionBoardParameters() {" + Environment.NewLine);

        sbdiscussionboardjs.Append("    var stext = Trim(document.getElementById(\'" + Strings.Replace((string)this.txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + "\').value);" + Environment.NewLine);
        sbdiscussionboardjs.Append("    checkName(stext,\'\'); " + Environment.NewLine);
        sbdiscussionboardjs.Append("    // return bexists; " + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function VerifyBoard() {" + Environment.NewLine);

        sbdiscussionboardjs.Append("document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbdiscussionboardjs.Append("if ((document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("	alert(\"" + MessageHelper.GetMessage("js:add discussion board no name") + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    ShowPane(\'dvProperties\');").Append(Environment.NewLine);
        sbdiscussionboardjs.Append("	document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionboardjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}else if (!CheckCategory())" + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("    ShowPane(\'dvCategories\');").Append(Environment.NewLine);
        sbdiscussionboardjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}else {" + Environment.NewLine);
        sbdiscussionboardjs.Append("	if (!CheckDiscussionBoardForillegalChar()) {" + Environment.NewLine);
        sbdiscussionboardjs.Append("		return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("	}" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("if (document.getElementById(\'addTemplate\').selectedIndex == 0) " + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("    alert(\"" + _messageHelper.GetMessage("js:add discuss board no template") + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    ShowPane(\'dvTemplates\');").Append(Environment.NewLine);
        sbdiscussionboardjs.Append("	document.getElementById(\'addTemplate\').focus();" + Environment.NewLine);
        sbdiscussionboardjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("var regexp1 = /\"/gi;" + Environment.NewLine);
        sbdiscussionboardjs.Append("document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("	SubmitForm(\'frmContent\',\'true\'); return true;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function CheckDiscussionBoardForillegalChar() {" + Environment.NewLine);
        sbdiscussionboardjs.Append("   var val = document.forms.frmContent." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionboardjs.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbdiscussionboardjs.Append("   {" + Environment.NewLine);
        sbdiscussionboardjs.Append("       alert(\"" + _messageHelper.GetMessage("js:add discuss board illegal char") + " (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("       return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("   }" + Environment.NewLine);
        sbdiscussionboardjs.Append("   return true;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function CheckCategory() {" + Environment.NewLine);
        sbdiscussionboardjs.Append("   if (arrInput.length < 1)" + Environment.NewLine);
        sbdiscussionboardjs.Append("   {" + Environment.NewLine);
        sbdiscussionboardjs.Append("	   alert(\"" + _messageHelper.GetMessage("js:add discuss board no subject") + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("       return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("   }" + Environment.NewLine);
        sbdiscussionboardjs.Append("   for (var j = 0; j < arrInput.length; j++)" + Environment.NewLine);
        sbdiscussionboardjs.Append("   {" + Environment.NewLine);
        sbdiscussionboardjs.Append("        val = Trim(arrInputValue[j]);" + Environment.NewLine);
        sbdiscussionboardjs.Append("        if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbdiscussionboardjs.Append("        { " + Environment.NewLine);
        sbdiscussionboardjs.Append("            alert(\"Subject name can\'t include (\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("            return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("        } " + Environment.NewLine);
        sbdiscussionboardjs.Append("        else if (val.length == 0) " + Environment.NewLine);
        sbdiscussionboardjs.Append("        {" + Environment.NewLine);
        sbdiscussionboardjs.Append("	        alert(\"Can\'t have a blank subject.\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("            return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("        }" + Environment.NewLine);
        sbdiscussionboardjs.Append("   }" + Environment.NewLine);
        sbdiscussionboardjs.Append("   return true;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function CloseChildPage(){" + Environment.NewLine);
        sbdiscussionboardjs.Append("    $ektron(\'#FrameContainer\').modalHide();" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function LoadChildPage() {" + Environment.NewLine);
        sbdiscussionboardjs.Append("    var frameObj = document.getElementById(\"ChildPage\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    frameObj.src = \"template_config.aspx?view=add&folder_edit=1\";" + Environment.NewLine);
        sbdiscussionboardjs.Append("    $ektron(\'#FrameContainer\').modalShow();" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine + Environment.NewLine);
        sbdiscussionboardjs.Append("function updatetheme()" + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("    var mylist = document.getElementById(\"" + drp_theme.UniqueID.Replace("$", "_") + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    var tText = mylist.options[mylist.selectedIndex].value;" + Environment.NewLine);
        sbdiscussionboardjs.Append("    if (tText.length > 0) {" + Environment.NewLine);
        sbdiscussionboardjs.Append("        document.getElementById(\"" + txt_adb_stylesheet.UniqueID.Replace("$", "_") + "\").value = tText;" + Environment.NewLine);
        sbdiscussionboardjs.Append("    }" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);


        ltr_af_js.Text = sbdiscussionboardjs.ToString();
    }

    #endregion

    #region AddDiscussionForum
    private void Display_AddDiscussionForum()
    {
        pnlOuterContainer.Visible = false;
        pnlDiscussionForum.Visible = true;

        _FolderData = _ContentApi.GetFolderById(_Id);

        AddDiscussionForumToolBar();

        DiscussionCategory[] adcCategories;
        Ektron.Cms.Content.EkContent m_refContent = new Ektron.Cms.Content.EkContent();

        hdn_adf_folderid.Value = _Id.ToString();

        ltr_adf_properties.Text += "<input type=\"hidden\" id=\"EnableReplication\" name=\"EnableReplication\" value=\"" + _FolderData.ReplicationMethod + "\" />";

        m_refContent = _ContentApi.EkContentRef;
        adcCategories = m_refContent.GetCategoriesforBoard(_Id);
        if (!(adcCategories == null) && (adcCategories.Length > 0))
        {
            for (int j = 0; j <= (adcCategories.Length - 1); j++)
            {
                drp_adf_category.Items.Add(new ListItem(Convert.ToString(adcCategories[j].Name), Convert.ToString(adcCategories[j].CategoryID)));
            }
        }
        else
        {
            throw (new Exception(MessageHelper.GetMessage("err NoBoardCategories")));
        }

        ltr_adb_cat.Text += "<p id=\"parah\">";
        ltr_adb_cat.Text += "</p>";
        ltr_adb_cat.Text += "<ul class=\"buttonList\">";
        ltr_adb_cat.Text += "<li><a href=\"#AddSubject\" title=\"" + MessageHelper.GetMessage("lnk Add New subject") + "\" onclick=\"addInput();return false;\" class=\"button buttonInlineBlock greenHover\">" + MessageHelper.GetMessage("lnk Add New subject") + "</a>";
        ltr_adb_cat.Text += "</li>";
        ltr_adb_cat.Text += "</ul>";
        ltr_adb_cat.Text += "<p class=\"required\">* Required Field</p>\"";
        ltr_adb_cat.Text += "<input type=\"hidden\" id=\"categorylength\" name=\"categorylength\" value=\"0\" />";
    }

    private void AddDiscussionForumToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string backup = "";
        string close;
        close = Request.QueryString["close"];
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(MessageHelper.GetMessage("alt discussion forum to board") + "\"" + _FolderData.Name + "\""));
        backup = _StyleHelper.getCallBackupPage((string)("content.aspx?action=ViewContentByCategory&id=" + _Id + "&LangType=" + _ContentLanguage));
        result.Append("<table><tr>");

		if (close != "true")
		{
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", backup, MessageHelper.GetMessage("alt back button text"), MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		
		result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", MessageHelper.GetMessage("alt add dforum button text"), MessageHelper.GetMessage("lbl add discussion forum"), "onclick=\"return SubmitForm(\'frmContent\', \'CheckDiscussionForumParameters()\')\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton("AddDiscussionForum", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Display_DiscussionForumJS()
    {
        StringBuilder sbdiscussionforumjs = new StringBuilder();
        sbdiscussionforumjs.Append("Ektron.ready(function() {document.forms[0]." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".focus();});" + Environment.NewLine);
        sbdiscussionforumjs.Append(Environment.NewLine + Environment.NewLine);
        sbdiscussionforumjs.Append("function CheckDiscussionForumParameters() {" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.frmContent." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.frmContent." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbdiscussionforumjs.Append("var iSort = document.forms.frmContent." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionforumjs.Append("if ((document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbdiscussionforumjs.Append("{" + Environment.NewLine);
        sbdiscussionforumjs.Append("	alert(\"" + MessageHelper.GetMessage("alert msg name supply") + "\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionforumjs.Append("	return false;" + Environment.NewLine);
        //iSort
        sbdiscussionforumjs.Append("} else if (isNaN(iSort)||iSort<1)" + Environment.NewLine);
        sbdiscussionforumjs.Append("{" + Environment.NewLine);
        sbdiscussionforumjs.Append("	alert(\"" + MessageHelper.GetMessage("msg sort") + "\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	document.forms.frmContent." + Strings.Replace((string)txt_adf_sortorder.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionforumjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("}else {" + Environment.NewLine);
        sbdiscussionforumjs.Append("	if (!CheckDiscussionForumForillegalChar()) {" + Environment.NewLine);
        sbdiscussionforumjs.Append("		return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("	}" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        sbdiscussionforumjs.Append("var regexp1 = /\"/gi;" + Environment.NewLine);
        sbdiscussionforumjs.Append("document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("	return true;" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        sbdiscussionforumjs.Append("function CheckDiscussionForumForillegalChar() {" + Environment.NewLine);
        sbdiscussionforumjs.Append("   var val = document.forms.frmContent." + Strings.Replace((string)txt_adf_forumname.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionforumjs.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbdiscussionforumjs.Append("   {" + Environment.NewLine);
        sbdiscussionforumjs.Append("       alert(\"" + "Forum name can\'t include" + " " + "(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbdiscussionforumjs.Append("       return false;" + Environment.NewLine);
        sbdiscussionforumjs.Append("   }" + Environment.NewLine);
        sbdiscussionforumjs.Append("   return true;" + Environment.NewLine);
        sbdiscussionforumjs.Append("}" + Environment.NewLine);
        ltr_af_js.Text = sbdiscussionforumjs.ToString();
    }
    #endregion

    #region AddCommunityFolder

    private void Display_AddCommunityFolder()
    {
        if (_FolderData == null)
        {
            _FolderData = _ContentApi.GetFolderById(_Id, true);
        }
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Add Community Folder to Folder" + " \"" + _FolderData.Name + "\"");
        ReplicationMethod.Text = "<input type=\"hidden\" id=\"EnableReplication\" name=\"EnableReplication\" value=\"1\" />";
        ltr_vf_types.Visible = true;
        ltrTypes.Visible = true;
    }

    #endregion

    #region content type selection

    private string DrawContentTypesBreaker(bool @checked)
    {
        if (@checked)
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" checked />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TypeBreak\" id=\"TypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TypeBreak\')\" />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }

    }

    private string DrawContentTypesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<div>");
        str.Append("    <table class=\"ektronGrid\"><tbody id=\"contentTypeTable\" name=\"contentTypeTable\">");
        str.Append("        <tr class=\"title-header\">");
        str.Append("            <td width=\"10%\" class=\"center\">");
        str.Append(MessageHelper.GetMessage("lbl default") + "</td>");
        str.Append("            <td width=\"70%\" class=\"left\">");
        str.Append(MessageHelper.GetMessage("lbl Smart Form") + "</td>");
        str.Append("            <td width=\"20%\" class=\"center\" colspan=\"2\">");
        str.Append(MessageHelper.GetMessage("lbl action") + "</td>");
        str.Append("        </tr>");
        return str.ToString();
    }

    private string DrawContentTypesEntry(int row_id, string name, long xml_id, bool isDefault, bool isEnabled)
    {
        StringBuilder str = new StringBuilder();


        str.Append("<tr id=\"row_" + xml_id + "\">");
        str.Append("<td class=\"center\" width=\"10%\">");
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
        str.Append("<a class=\"button greenHover minHeight buttonRemove\" href=\"javascript:RemoveContentType(" + xml_id + ", \'" + name + "\')\">" + MessageHelper.GetMessage("btn remove"));
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
        XmlConfigData[] xml_config_list;
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy);
        XmlConfigData[] active_xml_list;
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id);
        Collection addNew = new Collection();
        int k = 0;
        int row_id = 0;

        bool smartFormsRequired = System.Convert.ToBoolean(!Utilities.IsNonFormattedContentAllowed(active_xml_list));

        bool isEnabled = System.Convert.ToBoolean(!IsInheritingMultiConfig());


        bool isInheriting = IsInheritingMultiConfig();

        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(DrawContentTypesBreaker(isInheriting));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append(DrawContentTypesHeader());
        Collection ActiveXmlIdList = new Collection();
        for (k = 0; k <= active_xml_list.Length - 1; k++)
        {
            if (!ActiveXmlIdList.Contains(Convert.ToString(active_xml_list[k].Id)))
            {
                ActiveXmlIdList.Add(active_xml_list[k].Id, Convert.ToString(active_xml_list[k].Id), null, null);
            }
        }
        if (_FolderData.XmlConfiguration != null)
        {
            for (int j = 0; j <= (_FolderData.XmlConfiguration.Length - 1); j++)
            {
                if (!ActiveXmlIdList.Contains(Convert.ToString(_FolderData.XmlConfiguration[j].Id)))
                {
                    ActiveXmlIdList.Add(_FolderData.TemplateId, Convert.ToString(_FolderData.TemplateId), null, null);
                }
            }
        }


        for (k = 0; k <= xml_config_list.Length - 1; k++)
        {
            if (ActiveXmlIdList.Contains(Convert.ToString(xml_config_list[k].Id)))
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
            str.Append(DrawContentTypesEntry(row_id, MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_xml_list), isEnabled));
        }

        str.Append("</tbody></table>");
        str.Append("</div>");
        str.Append("<div class=\"ektronTopSpace\"></div>");
        str.Append("<table width=\"100%\"><tbody>");
        str.Append("<tr><td width=\"90%\">");
        str.Append("<select name=\"addContentType\" id=\"addContentType\" disabled>");
        str.Append("<option value=\"-1\">" + MessageHelper.GetMessage("select smart form") + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["xml_id"] + "\">" + row["xml_name"] + "</option>");
        }

        str.Append("</select>");
        str.Append(" <a href=\"#\" onclick=\"PreviewSelectedXmlConfig(\'" + _ContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + _AppPath + "images/UI/Icons/preview.png" + "\" alt=\"" + MessageHelper.GetMessage("lbl Preview Smart Form") + "\" title=\"" + MessageHelper.GetMessage("lbl Preview Smart Form") + "\" border=\"0\" /></a>");
        str.Append(" <a href=\" javascript:ActivateContentType()\"><img src=\"" + _AppPath + "images/UI/Icons/add.png" + "\" alt=\"" + MessageHelper.GetMessage("add title") + "\" title=\"" + MessageHelper.GetMessage("add title") + "\" border=\"0\" /></a></td></tr>");
        str.Append(DrawContentTypesFooter());
        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"isEven\" id=\"isEven\" value=\"0\" />");
        }


        if (smartFormsRequired)
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onclick=\"ToggleRequireSmartForms()\" checked disabled>");
        }
        else
        {
            str.Append("<div><input type=\"checkbox\" id=\"requireSmartForms\" name=\"requireSmartForms\" onclick=\"ToggleRequireSmartForms()\" disabled>");
        }

        str.Append(MessageHelper.GetMessage("lbl Require Smart Forms"));
        str.Append("</div>");
        ltr_vf_types.Text = str.ToString();
    }

    #endregion

    #region multi-template selection
    private string DrawContentTemplatesBreaker(bool @checked)
    {
        if (@checked)
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" checked />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
        else
        {
            return "<input name=\"TemplateTypeBreak\" id=\"TemplateTypeBreak\" type=\"checkbox\" onclick=\"ToggleMultiXmlTemplateInherit(\'TemplateTypeBreak\')\" />" + MessageHelper.GetMessage("lbl Inherit Parent Configuration");
        }
    }

    private string DrawContentTemplatesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table class=\"ektronGrid\"><tbody id=\"templateTable\">");
        str.Append("<tr class=\"title-header\">");
        str.Append("<td width=\"10%\" class=\"center\">");
        str.Append(MessageHelper.GetMessage("lbl default"));
        str.Append("</td>");
        str.Append("<td width=\"70%\" class=\"left\">");
        str.Append(MessageHelper.GetMessage("lbl Page Template Name"));
        str.Append("</td>");
        str.Append("<td width=\"20%\" class=\"center\" colspan=\"2\">");
        str.Append(MessageHelper.GetMessage("lbl Action"));
        str.Append("</td>");
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
        if (isDefault == true && isEnabled)
        {
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" checked />");
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
            str.Append("<input type=\"radio\" id=\"tdefault\" name=\"tdefault\" value=\"" + name + "\" />");
        }

        str.Append("</td>");
        str.Append("<td width=\"70%\">");
        str.Append(name + typestring + "<input id=\"tinput_" + template_id + "\" name=\"tinput_" + template_id + "\" type=\"hidden\" value=\"" + template_id + "\" /></td>");
        string link = "";
        if (templatedata.SubType == EkEnumeration.TemplateSubType.MasterLayout)
        {
            link = _ContentApi.EkContentRef.GetContentQlink(templatedata.MasterLayoutID, _ContentApi.GetFolderIdForContentId(templatedata.MasterLayoutID));
            str.Append("<td class=\"center\" width=\"10%\"><a href=\"#\" class=\"button greenHover minHeight buttonSearch\" onclick=\"PreviewSpecificTemplate(\'" + link + "\', 800,600)\">" + MessageHelper.GetMessage("lbl View") + "</a></td>");
        }
        else
        {
            str.Append("<td class=\"center\" width=\"10%\"><a class=\"button greenHover minHeight buttonSearch\"  href=\"javascript:PreviewSpecificTemplate(\'" + _ContentApi.SitePath + name + "\', 800,600)\">" + MessageHelper.GetMessage("lbl View") + "</a></td>");
        }
        str.Append("<td class=\"center\" width=\"10%\"><a class=\"button redHover minHeight buttonRemove\" href=\"javascript:RemoveTemplate(" + template_id + ", \'" + name + "\', \'" + link + "\')\">" + MessageHelper.GetMessage("btn remove") + "</td>");
        str.Append("</tr>");

        return str.ToString();
    }

    private string DrawContentTemplatesFooter()
    {
        return "</tbody></table>";
    }

    private bool IsInheritingMultiConfig()
    {
        return true;
    }
   
    private void DrawContentTemplatesTable()
    {
        TemplateData[] active_templates;
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id);

        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");

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

        bool isInheriting = IsInheritingMultiConfig();

        //If (Not foundDefault) Then
        //    isInheriting = False
        //End If

        StringBuilder str = new StringBuilder();

        str.Append(DrawContentTemplatesBreaker(isInheriting));
        str.Append("<div class=\"ektronTopSpace\"></div>");

        str.Append("<div>");
        str.Append(DrawContentTemplatesHeader());

        //if (_FolderData.Id == 0)
        //{
        //    isInheriting = false;
        //}

        Collection ActiveTemplateIdList = new Collection();
        for (k = 0; k <= active_templates.Length - 1; k++)
        {
            if (!ActiveTemplateIdList.Contains(Convert.ToString(active_templates[k].Id)))
            {
                ActiveTemplateIdList.Add(active_templates[k].Id, Convert.ToString(active_templates[k].Id), null, null);
            }
        }

        if (!ActiveTemplateIdList.Contains(Convert.ToString(_FolderData.TemplateId)))
        {
            ActiveTemplateIdList.Add(_FolderData.TemplateId, Convert.ToString(_FolderData.TemplateId), null, null);
        }


        for (k = 0; k <= template_data.Length - 1; k++)
        {
            if (ActiveTemplateIdList.Contains(Convert.ToString(template_data[k].Id)))
            {

                string typestring = "";
                if (template_data[k].SubType == EkEnumeration.TemplateSubType.Wireframes)
                {
                    typestring = " (" + MessageHelper.GetMessage("lbl pagebuilder wireframe template") + ")";
                }
                else if (template_data[k].SubType == EkEnumeration.TemplateSubType.MasterLayout)
                {
                    typestring = " (" + MessageHelper.GetMessage("lbl pagebuilder master layouts") + ")";
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
                    type = " (" + MessageHelper.GetMessage("lbl pagebuilder wireframe template") + ")";
                }
                else if (template_data[k].SubType == EkEnumeration.TemplateSubType.MasterLayout)
                {
                    type = " (" + MessageHelper.GetMessage("lbl pagebuilder master layouts") + ")";
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
        str.Append("<select name=\"addTemplate\" id=\"addTemplate\" disabled>");
        str.Append("<option value=\"0\">" + MessageHelper.GetMessage("generic select template") + "</option>");

        foreach (Collection row in addNew)
        {
            str.Append("<option value=\"" + row["template_id"] + "\"");
            if (!string.IsNullOrEmpty(Convert.ToString(row["url"])))
            {
                str.Append(" url=\"" + row["url"] + "\"");
            }
            str.Append(">" + row["template_name"] + row["template_type"] + "</option>");
        }
        str.Append("</select>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"#\" onclick=\"PreviewTemplate(\'" + _ContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + _AppPath + "images/UI/Icons/preview.png" + "\" alt=\""+  MessageHelper.GetMessage("lbl preview template") + "\" title=\"" + MessageHelper.GetMessage("lbl preview template") + "\" /></a>");
        str.Append("</td>");
        str.Append("<td>&nbsp;</td>");
        str.Append("<td>");
        str.Append("<a href=\"javascript:ActivateTemplate(\'" + this._ContentApi.SitePath + "\')\"><img src=\"" + _AppPath + "images/UI/icons/add.png\" alt=\"" + MessageHelper.GetMessage("add title") + "\" title=\"" + MessageHelper.GetMessage("add title") + "\" /></a>");
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
        str.Append("<a href=\"javascript:OpenAddDialog()\" class=\"button buttonInlineBlock greenHover buttonAdd\">" + MessageHelper.GetMessage("lbl add new template") + "</a>");
        //str.Append("<a href=""javascript:LoadChildPage()"" class=""button buttonInlineBlock greenHover buttonAdd"">" & MessageHelper.GetMessage("lbl add new template") & "</a>")

        litBlogTemplate.Text = str.ToString();
        template_list.Text = str.ToString();
    }
    #endregion

    #region multi-xml/multi-template postback
    private void ProcessContentTemplatesPostBack(string type)
    {

        string IsInheritingTemplates = Request.Form["TemplateTypeBreak"];
        string IsInheritingXml = Request.Form["TypeBreak"];
        XmlConfigData[] xml_config_list;
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy);
        long default_template_id = 0;
        TemplateData[] template_data;
        template_data = _ContentApi.GetAllTemplates("TemplateFileName");

        int i = 0;
        Collection xml_active_list = new Collection();
        Collection template_active_list = new Collection();
        long default_xml_id = -1;

        if (!String.IsNullOrEmpty(Request.Form[txt_adb_boardname.UniqueID]))
        {
            for (i = 0; i <= template_data.Length - 1; i++)
            {
                if (Convert.ToInt64(Request.Form["addTemplate"]) == template_data[i].Id)
                {
                    template_active_list.Add(template_data[i].Id, Convert.ToString(template_data[i].Id), null, null);
                }
            }
        }
        else
        {
            if (IsInheritingTemplates == null)
            {
                for (i = 0; i <= template_data.Length - 1; i++)
                {
                    if (!(Request.Form["tinput_" + template_data[i].Id] == null))
                    {
                        template_active_list.Add(template_data[i].Id, Convert.ToString(template_data[i].Id), null, null);
                    }
                }
            }
        }

        if (IsInheritingXml == null && Request.Form[txtBlogName.UniqueID] == null)
        {
            for (i = 0; i <= xml_config_list.Length - 1; i++)
            {
                if (!(Request.Form["input_" + xml_config_list[i].Id] == null))
                {
                    xml_active_list.Add(xml_config_list[i].Id, Convert.ToString(xml_config_list[i].Id), null, null);
                }
            }

            if (!(Request.Form["sfdefault"] == null))
            {
                default_xml_id = Convert.ToInt64(Request.Form["sfdefault"]);
            }

            if (Request.Form["requireSmartForms"] == null)
            {
                if (!xml_active_list.Contains("0"))
                {
                    xml_active_list.Add("0", "0", null, null);
                }
            }
        }
        if (type == "forum")
        {
            if ((Request.Form["addTemplate"] != null) && Request.Form["addTemplate"] != "")
            {
                default_template_id = Convert.ToInt64(Request.Form["addTemplate"]);
            }
            _ContentApi.UpdateForumFolderMultiConfig(_FolderId, default_xml_id, default_template_id, template_active_list, xml_active_list);
        }
        else
        {
            _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, xml_active_list);
        }

    }
    #endregion

    #region Site Map/Breadcrumb

    #endregion

    #region Ajax functions

    private string AJAXcheck(string sResponse, string sURLQuery)
    {
        workareabase wb = new workareabase();
        wb.AJAX.ResponseJS = sResponse;
        wb.AJAX.URLQuery = sURLQuery;
        wb.AJAX.FunctionName = "checkName";
        return wb.AJAX.Render();
    }

    private string GetResponseString(string nextfunction)
    {
        System.Text.StringBuilder sbAEJS = new System.Text.StringBuilder();
        sbAEJS.Append("    if (response > 0){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'" + this.MessageHelper.GetMessage("com: subfolder already exists") + "\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else{").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine);
        sbAEJS.Append("    } ").Append(Environment.NewLine);
        return sbAEJS.ToString();
    }

    #endregion

    #region flagging section
    private void DrawFlaggingOptions()
    {
        //Dim str As New StringBuilder()

        //Try
        //          str.Append("" & m_refMsg.GetMessage("lbl flagging inherit parent config:") & "<input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged()"" />" + Environment.NewLine)
        //	str.Append("<input type=""hidden"" id=""flagging_options_inherit_hf"" value=""" + IIf(folder_data.FlagInherited, "True", "False") + """ />" + Environment.NewLine)
        //	'str.Append("<br /><br />Flagging Configuration: -HC " + Environment.NewLine)
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
        //          str.Append("" & m_refMsg.GetMessage("lbl assigned flags:") + Environment.NewLine)
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
        //          str.Append("" & m_refMsg.GetMessage("lbl avail flags:") + Environment.NewLine)
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
        inheritFlag.Text = "<input type=\"checkbox\" id=\"flagging_options_inherit_cbx\" name=\"flagging_options_inherit_cbx\" checked=\"checked\" onclick=\"InheritFlagingChanged(\'" + ddflags.ClientID + "\')\" />" + MessageHelper.GetMessage("lbl inherit parent configuration");
        ddflags.Enabled = false;
        ddflags.Items.Add(new ListItem(" -None- ", "0"));
        ddflags.Items.FindByValue("0").Selected = true;
        //Dim flag_fdata() As FolderFlagDefData = m_refContentApi.GetAllFolderFlagDef(0)
        FlagDefData[] flag_data = _ContentApi.EkContentRef.GetAllFlaggingDefinitions(false);
        if ((flag_data != null) && flag_data.Length > 0)
        {
            for (int i = 0; i <= flag_data.Length - 1; i++)
            {
                ddflags.Items.Add(new ListItem(Convert.ToString(flag_data[i].Name), Convert.ToString(flag_data[i].ID)));
                //If (flag_fdata IsNot Nothing AndAlso flag_fdata.Length > 0 AndAlso flag_fdata(0).ID = flag_data(i).ID) Then
                //    ddflags.Items.FindByValue(flag_data(i).ID).Selected = True
                //    ddflags.SelectedIndex = ddflags.Items.IndexOf(ddflags.Items.FindByValue(flag_data(i).ID))
                //End If
            }
        }
        if ((_FolderData.FolderFlags != null) && _FolderData.FolderFlags.Length > 0)
        {
            parent_flag.Value = Convert.ToString(_FolderData.FolderFlags[0].ID);
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
    //	Dim assignedDefault As Boolean = False

    //	Try
    //		flags = folder_data.FolderFlags	'flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)
    //		For Each flag In flags
    //			' until API supports reporting the default flag for a folder, assume first item is default:
    //			If (showDefault AndAlso (Not assignedDefault)) Then
    //				assignedDefault = True
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

        //Try
        //	If (Not IsNothing(Request.Form("flagging_options_inherit_cbx"))) Then
        //		inheritParentConfig = "on" = Request.Form("flagging_options_inherit_cbx").ToLower
        //	End If

        //	' Update settings to db:
        //	pagedata.Add(IIf(inheritParentConfig, 1, 0), "InheritFlag")
        //	If (Not inheritParentConfig) Then
        //		pagedata.Add(0, "InheritFlagFrom")
        //		If ((Not IsNothing(Request.Form("flagging_options_default_hdn"))) AndAlso (IsNumeric(Request.Form("flagging_options_default_hdn")))) Then
        //			pagedata.Add(CType(Request.Form("flagging_options_default_hdn"), Integer), "DefaultFlagId")
        //		Else
        //			pagedata.Add(0, "DefaultFlagId") ' TODO: Check, should we leave this key non-existant when no default is known
        //		End If
        //		If ((Not inheritParentConfig) AndAlso (Not IsNothing(Request.Form("flagging_options_hdn")))) Then
        //			pagedata.Add(Request.Form("flagging_options_hdn"), "FlagList")
        //		End If
        //	End If

        //Catch ex As Exception
        //Finally
        //End Try
        try
        {
            if (!(Request.Form["flagging_options_inherit_cbx"] == null))
            {
                if (Convert.ToString(Request.Form["flagging_options_inherit_cbx"].ToLower()) == "on")
                    inheritParentConfig = true;
                else
                    inheritParentConfig = false;
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
            else
            {
                _PageData.Add(Request.Form[parent_flag.UniqueID], "DefaultFlagId", null, null);
                _PageData.Add(Request.Form[parent_flag.UniqueID], "FlagList", null, null);
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


    public string IsPublishedAsPdf()
    {
        return _IsPublishedAsPdf;
    }
    private void setLocalText()
    {
        chkEnable.ToolTip=chkEnable.Text = MessageHelper.GetMessage("lbl add blog enable comments");
        chkModerate.ToolTip = chkModerate.Text = MessageHelper.GetMessage("lbl add blog moderate comments");
        chkRequire.ToolTip = chkRequire.Text = MessageHelper.GetMessage("lbl add blog comments require authentication");
    }

}

