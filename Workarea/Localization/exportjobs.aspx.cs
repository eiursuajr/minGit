//-----------------------------------------------------------------------
// <copyright file="exportjobs.aspx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// Creates new export jobs based on locale taxonomy or queries for content marked as not ready for translation
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.Localization;
using Ektron.Cms.Localization;

/// <summary>
/// Codebehind for exportjobs.aspx
/// </summary>
public partial class Workarea_exportjobs : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    #region Member Variables

    /// <summary>
    /// Private reference to common API
    /// </summary>
    private Ektron.Cms.Common.EkRequestInformation requestInfoRef = null;

    /// <summary>
    /// Private reference to common API
    /// </summary>
    private Ektron.Cms.CommonApi commonApi = null;

    /// <summary>
    /// Private StyleHelper reference
    /// </summary>
    private StyleHelper refStyle = new StyleHelper();

    /// <summary>
    /// Application image path
    /// </summary>
    private string appImgPath = string.Empty;

    /// <summary>
    /// Private site API reference
    /// </summary>
    private Ektron.Cms.SiteAPI siteApi = new Ektron.Cms.SiteAPI();

    /// <summary>
    /// Default job title
    /// </summary>
    private string defaultJobTitle = "xlf" + DateTime.Now.ToString("s").Replace(":", string.Empty);

    /// <summary>
    /// Private storage of content language IDs and names
    /// </summary>
    private Dictionary<int, string> contentLanguages = null;

    #endregion

    /// <summary>
    /// Page OnInit event
    /// </summary>
    /// <param name="e"><see cref="EventArgs" /> object</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!Page.IsPostBack && !string.IsNullOrEmpty(Request.QueryString["t"]))
        {
            List<long> ids = new List<long>();
            string[] qids = Request.QueryString["t"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            long qi;
            foreach (string q in qids)
            {
                if (long.TryParse(q, out qi))
                {
                    ids.Add(qi);
                }
            }

            treeTargetJob.SelectedIds = ids.ToArray();
        }
    }

    /// <summary>
    /// Page OnLoad event override
    /// </summary>
    /// <param name="e"><see cref="EventArgs" /> object</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        this.commonApi = GetCommonApi();
        this.requestInfoRef = this.commonApi.RequestInformationRef;
        this.appImgPath = this.commonApi.AppImgPath;

        this.RegisterResources();

        StyleSheetJS.Text = this.refStyle.GetClientScript();

        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(
            this.requestInfoRef,
            Ektron.Cms.DataIO.LicenseManager.Feature.Xliff,
            false))
        {
            Utilities.ShowError(GetMessage("feature locked error"));
        }
    }

    /// <summary>
    /// Page OnPreRender override
    /// </summary>
    /// <param name="e"><see cref="EventArgs" /> object</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (this.requestInfoRef.IsMembershipUser == 1 || this.requestInfoRef.UserId == 0)
        {
            Response.Redirect("../blank.htm", false);
            return;
        }

        LoadingImg.Text = GetMessage("one moment msg");
        this.GenerateToolbar();
        if (!IsPostBack)
        {
            // Source Language
            int sourceLanguageId;
            if (!int.TryParse(Request.QueryString["srcLang"], out sourceLanguageId))
            {
                sourceLanguageId = this.requestInfoRef.ContentLanguage;
            }

            this.languageSelector.Value = sourceLanguageId.ToString();

            // Job Title
            string jobTitle = Request.QueryString["job"];
            if (!String.IsNullOrEmpty(jobTitle))
            {
                this.txtJobTitle.Text = jobTitle;
            }
            // XLIFF Version
            string xliffVersion = this.commonApi.GetCookieValue("xliff");
            if (String.IsNullOrEmpty(xliffVersion))
            {
                xliffVersion = Request.QueryString["xliff"];

                // Basic security validation
                if (!String.IsNullOrEmpty(xliffVersion) && System.Text.RegularExpressions.Regex.IsMatch(xliffVersion, @"[^\w\.]"))
                {
                    xliffVersion = string.Empty;
                }

                if (String.IsNullOrEmpty(xliffVersion))
                {
                    xliffVersion = this.requestInfoRef.XliffVersion;
                }
            }

            this.txtXliffVer.SelectedValue = xliffVersion;

            // Max ZIP size
            string maxZipFileSize = this.commonApi.GetCookieValue("xliffZipSize");
            if (String.IsNullOrEmpty(maxZipFileSize))
            {
                maxZipFileSize = Request.QueryString["zip"];

                // Basic security validation
                if (!String.IsNullOrEmpty(maxZipFileSize) && System.Text.RegularExpressions.Regex.IsMatch(maxZipFileSize, @"[^[0-9]]"))
                {
                    maxZipFileSize = string.Empty;
                }

                if (String.IsNullOrEmpty(maxZipFileSize))
                {
                    maxZipFileSize = string.Empty; // unlimited
                }
            }

            this.lstMaxZipSize.SelectedValue = maxZipFileSize;

            //string includeHistory = this.commonApi.GetCookieValue("includeHistory");
            //bool toIncludeHistory = false;
            //if (String.IsNullOrEmpty(includeHistory))
            //{
            //    toIncludeHistory = false;
            //}
            //else
            //{
            //    if (false == bool.TryParse(includeHistory, out toIncludeHistory))
            //    {
            //        toIncludeHistory = false;
            //    }
            //}

            //this.chkIncludeHistory.Checked = toIncludeHistory;
        }

        SourceLangSelector.InnerHtml = this.refStyle.ShowAllActiveLanguage(false, string.Empty, "setSourceLanguage(this.value);", languageSelector.Value);

        if (IsPostBack)
        {
            bool queryNotReadyForTranslation = Request.Form["querynotreadyfortranslation"] == "1";

            divExport.Visible = false;
            
            if (queryNotReadyForTranslation)
            {
                plcXLIFFData.Visible = false;
                plcNotReadyContent.Visible = true;
                this.DoNotReadyQuery();
            }
            else
            {
                this.ExportForTranslation();
            }
        }
        else
        {
            divExport.Visible = true;
        }
    }

    /////// <summary>
    /////// Returns the combined name of a specified language ID
    /////// </summary>
    /////// <param name="languageId">The ID of the language</param>
    /////// <returns>A string containing the name of the language</returns>
    ////protected string GetContentLanguageName(int languageId)
    ////{
    ////    this.EnsureContentLanguagesLoaded();

    ////    if (!this.contentLanguages.ContainsKey(languageId))
    ////    {
    ////        this.contentLanguages.Add(languageId, GetMessage("invalid language id") + ": " + languageId.ToString());
    ////    }

    ////    return this.contentLanguages[languageId];
    ////}

    /// <summary>
    /// Takes an <see cref="ILocalizable" /> item and generates a link to its workarea editing page
    /// </summary>
    /// <param name="item">The <see cref="ILocalizable" /> item</param>
    /// <returns>A relative URL to the correct workarea page for editing the content</returns>
    protected string BuildContentLink(Ektron.Cms.BusinessObjects.Localization.ILocalizable item)
    {
        string link = string.Empty;
        switch (item.LocalizableType)
        {
            case LocalizableCmsObjectType.Menu:
            case LocalizableCmsObjectType.MenuItem:
                link = string.Format(
                    "../collections.aspx?folderid=-1&Action=ViewMenu&nid={0}&bpage=reports&LangType={1}",
                    item.Id,
                    item.LanguageId);
                break;
            case LocalizableCmsObjectType.FolderContents:
                link = string.Format("../content.aspx?action=ViewContentByCategory&id={0}&treeViewId=0", item.Id);
                break;
            case LocalizableCmsObjectType.FolderTree:
                link = string.Format("../content.aspx?action=ViewFolder&id={0}", item.Id);
                break;
            case LocalizableCmsObjectType.Taxonomy:
                link = string.Format(
                    "../taxonomy.aspx?action=view&view=item&taxonomyid={0}&treeViewId=-1&LangType={1}",
                    item.Id,
                    item.LanguageId);
                break;
            case LocalizableCmsObjectType.LocaleTaxonomy:
                link = string.Format(
                    "../Localization/LocaleTaxonomy.aspx?action=view&view=locale&taxonomyid={0}&treeViewId=-1&LangType={1}",
                    item.Id,
                    item.LanguageId);
                break;
            case LocalizableCmsObjectType.Content:
            case LocalizableCmsObjectType.DmsAsset:
                link = string.Format(
                    "../content.aspx?action=View&folder_id=-1&id={0}&LangType={1}&callerpage={2}&origurl={3}",
                    item.Id,
                    item.LanguageId,
                    EkFunctions.UrlEncode("Localization/exportjobs.aspx"),
                    EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
                break;
            default:
                break;
        }

        return link;
    }

    ////protected string GetLocalizationState(Ektron.Cms.BusinessObjects.Localization.ILocalizable item)
    ////{
    ////    switch (item.LocalizationState)
    ////    {
    ////        case Ektron.Cms.Localization.LocalizationState.DoNotTranslate:
    ////            return GetMessage("lbl not translatable");
    ////        case Ektron.Cms.Localization.LocalizationState.NotReady:
    ////            return GetMessage("lbl not ready for translation");
    ////        case Ektron.Cms.Localization.LocalizationState.Ready:
    ////            return GetMessage("lbl ready for translation");
    ////        default:
    ////            return item.LocalizationState.ToString();
    ////    }
    ////}

    /// <summary>
    /// Returns the friendly name of the content type of an <see cref="ILocalizable" /> item
    /// </summary>
    /// <param name="item">The <see cref="ILocalizable" /> item</param>
    /// <returns>A string indicating the friendly (localized) name of the content type</returns>
    protected string GetContentTypeName(Ektron.Cms.BusinessObjects.Localization.ILocalizable item)
    {
        switch (item.LocalizableType)
        {
            case LocalizableCmsObjectType.Content:
                return GetMessage("lbl html content");
            case LocalizableCmsObjectType.DmsAsset:
                return GetMessage("lbl dms documents");
            case LocalizableCmsObjectType.FolderContents:
                return GetMessage("lbl folder");
            case LocalizableCmsObjectType.FolderTree:
                return GetMessage("lbl folder");
            case LocalizableCmsObjectType.LocaleTaxonomy:
                return GetMessage("lbl locale taxonomy");
            case LocalizableCmsObjectType.Menu:
                return GetMessage("lbl menu");
            case LocalizableCmsObjectType.MenuItem:
                return GetMessage("lbl menu item");
            case LocalizableCmsObjectType.Taxonomy:
                return GetMessage("lbl taxonomy");
        }

        return string.Empty;
    }

    /// <summary>
    /// Method that renders the toolbar on the page
    /// </summary>
    private void GenerateToolbar()
    { 
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        bool queryNotReadyForTranslation = Page.IsPostBack && Request.Form["querynotreadyfortranslation"] == "1";

        string workareaTitlebarTitle = null;
        workareaTitlebarTitle = GetMessage("lbl export for translation");
        txtTitleBar.InnerHtml = this.refStyle.GetTitleBar(workareaTitlebarTitle);

        result.Append("<table><tbody><tr>");

        if (Page.IsPostBack || queryNotReadyForTranslation)
        {
			string backUrl = String.Format("ExportJobs.aspx?srcLang={0}&job={1}&t={2}&xliff={3}&zip={4}",
                this.languageSelector.Value,
                this.txtJobTitle.Text,
                String.Join(",", treeTargetJob.SelectedIds.ToList<long>().ConvertAll<string>(new Converter<long, string>(
                    delegate (long l) {
                        return l.ToString();
                    })).ToArray()),
                this.txtXliffVer.Text,
                this.lstMaxZipSize.Text);

			result.Append(this.refStyle.GetButtonEventsWCaption(
				this.siteApi.AppPath + "Images/ui/icons/back.png",
				backUrl,
				GetMessage("btn back"),
				GetMessage("btn back"),
				string.Empty,
				StyleHelper.BackButtonCssClass,
				true));
			//"onclick='history.go(-1); return false;'"));
        }
        else
        {
            // Get content "not ready" for translation
            result.Append(this.refStyle.GetButtonEventsWCaption(
                this.siteApi.AppPath + "Images/ui/icons/translationExport.png",
                "#",
                GetMessage("alt click here to create xliff files for translation"),
                GetMessage("lbl create xliff files for translation"),
                "onclick='DisplayHoldMsg(true); return SubmitForm(0,\"validate()\")'",
                StyleHelper.TranslationButtonCssClass, true));

            result.Append(this.refStyle.GetButtonEventsWCaption(
                this.siteApi.AppPath + "Images/ui/icons/caution.png",
                "#",
                GetMessage("alt click here to see content not ready for translation"),
                GetMessage("btn report of content not ready for translation"),
                "onclick='DisplayHoldMsg(true); SetQueryNotReady(); return SubmitForm(0,\"validate()\");'"));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(this.refStyle.GetHelpButton("localize", string.Empty));
        result.Append("</td>");

        result.Append("</tr></tbody></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    #region LOCALIZATION - Export
    /// <summary>
    /// Method called when job is started
    /// </summary>
    private void ExportForTranslation()
    {
        string xliffVersion = txtXliffVer.SelectedValue;
        if (xliffVersion == this.requestInfoRef.XliffVersion)
        {
            this.commonApi.SetCookieValue("xliff", string.Empty);
        }
        else
        {
            this.commonApi.SetCookieValue("xliff", xliffVersion);
        }

        string maxZipFileSize = lstMaxZipSize.SelectedValue;
        long maxCompressedFileSize = long.MaxValue;
        if (long.TryParse(maxZipFileSize, out maxCompressedFileSize))
        {
            this.commonApi.SetCookieValue("xliffZipSize", maxZipFileSize);
        }

        //bool toIncludeHistory = this.chkIncludeHistory.Checked;
        //this.commonApi.SetCookieValue("includeHistory", toIncludeHistory.ToString());

        string title = txtJobTitle.Text;

        Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.requestInfoRef);
        LocalizationExportJob exportJob = this.CreateExportJob(title, l10nMgr);
        exportJob.XliffVersion = xliffVersion;
        exportJob.MaxCompressedFileSize = maxCompressedFileSize;
        l10nMgr.StartExportForTranslation(exportJob);
    }

    /// <summary>
    /// Creates an export job
    /// </summary>
    /// <param name="title">The title of the job</param>
    /// <param name="l10nMgr">Reference to <see cref="L10nManager"/></param>
    /// <returns>An <see cref="LocalizationExportJob"/> object</returns>
    private LocalizationExportJob CreateExportJob(string title, Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr)
    {
        long[] taxonomyIds = this.GetSelectedLocaleTaxonomyIds();
        if (String.IsNullOrEmpty(title))
        {
            title = this.defaultJobTitle;
            if (taxonomyIds != null && 1 == taxonomyIds.Length)
            {
                long id = taxonomyIds[0];
                Ektron.Cms.API.Content.Taxonomy taxonomyApi = new Ektron.Cms.API.Content.Taxonomy();
                Ektron.Cms.TaxonomyRequest req = new Ektron.Cms.TaxonomyRequest();
                req.TaxonomyId = id;
                req.TaxonomyLanguage = this.commonApi.ContentLanguage;
                Ektron.Cms.TaxonomyData data = taxonomyApi.ReadTaxonomy(ref req);
                if (data != null)
                {
                    title = data.TaxonomyName;
                }
            }
        }

        LocalizationExportJob job = new LocalizationExportJob(title);
        job.SourceLanguageId = this.GetSelectedSourceLanguage();
        foreach (long id in taxonomyIds)
        {
            job.AddItem(LocalizableCmsObjectType.LocaleTaxonomy, id);
        }

        return job;
    }

    /// <summary>
    /// Queries the posted page for a list of selected locale taxonomy IDs
    /// </summary>
    /// <returns>An array of long integers</returns>
    private long[] GetSelectedLocaleTaxonomyIds()
    {
        return treeTargetJob.SelectedIds;
    }

    /// <summary>
    /// Queries the posted page for the selected source language IDs
    /// </summary>
    /// <returns>The ID of the selected source language</returns>
    private int GetSelectedSourceLanguage()
    {
        return Convert.ToInt16(Request.Form["frm_langID"]);
    }

    #endregion

    /// <summary>
    /// Queries the selected locale taxonomy and renders a list of content that is marked "Not Ready for Translation"
    /// </summary>
    private void DoNotReadyQuery()
    {
        long[] targetJobs = this.GetSelectedLocaleTaxonomyIds();

        if (0 == targetJobs.Length)
        {
            rptNotReadyContent.Visible = false;
            lblNoResultsForNotReady.Visible = true;
            return;
        }

        Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.requestInfoRef);
        LocalizationExportJob job = this.CreateExportJob("Content Not Ready for Translation", l10nMgr); // No localization necessary, as this job never gets created (it's just for querying and needs a name)

        Criteria<LocalizationExportJob> criteria = new Criteria<LocalizationExportJob>();
        criteria.PagingInfo.RecordsPerPage = 50;
        List<Ektron.Cms.BusinessObjects.Localization.ILocalizable> items = l10nMgr.GetNotReadyList(job, criteria);

        if (0 == items.Count)
        {
            rptNotReadyContent.Visible = false;
            lblNoResultsForNotReady.Visible = true;
        }
        else
        {
            rptNotReadyContent.DataSource = items;
            rptNotReadyContent.DataBind();
        }
    }
    
    /// <summary>
    /// Method that ensures the list of content languages has been loaded
    /// </summary>
    private void EnsureContentLanguagesLoaded()
    {
        if (null == this.contentLanguages)
        {
            this.contentLanguages = new Dictionary<int, string>();

            List<LocaleData> locales = this.LocaleManager.GetEnabledLocales();
            locales.ForEach(delegate(LocaleData data)
            {
                if (!this.contentLanguages.ContainsKey(data.Id))
                {
                    this.contentLanguages.Add(data.Id, data.CombinedName);
                }
            });
        }
    }

    /// <summary>
    /// Registers CSS and JS references on the page
    /// </summary>
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
