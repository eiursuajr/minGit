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
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Localization;
using Ektron.Cms.Workarea;
using Ektron.Cms.API;

public partial class MSLocalization_jobs : workareabase
{
    protected string m_sPageName = "LocalizationJobs.aspx";
    protected Ektron.Cms.LocalizationAPI localApi = new Ektron.Cms.LocalizationAPI();
    protected bool isTest = false;
    protected bool isOverride = false;
    protected bool invalidDependency = false;
    protected string defaultJobTitle = "xlf" + DateTime.Now.ToString("s").Replace(":", string.Empty);
    private string xliffVersion = "1.0";
    private List<long> contentIds = new List<long>();
    private List<int> languageIds = new List<int>();
    private string pseudoLocJobTitle = "Dashboard Pseudo Localization";
    private string transMemJobTitle = "Dashboard Translation Memory";
    private long showJobId = 0;
    private bool pseudoLocJob = false;
    private bool transMemJob = false;
	private bool showFiles = false;

    private string sitePath = "";
    public string SitePath
    {
        get
        {
            return sitePath;
        }
    }
    private string appPath = "";
    public string AppPath
    {
        get
        {
            return appPath;
        }
    }

    #region Page Functions


    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            this.xliffVersion = m_refContentApi.RequestInformationRef.XliffVersion;
            sitePath = this.m_refContentApi.RequestInformationRef.SitePath;
            appPath = this.m_refContentApi.RequestInformationRef.ApplicationPath;

            rptNotReadyContent.Visible = false;
            pnl_error.Visible = false;
            base.Page_Load(sender, e);
            Util_GetValues();
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            long jobId = 0;
            switch (this.m_sPageAction)
            {
                case "displayhandoff":
                    Util_ShowJobInfo(jobId);
                    break;
                case "pseudo" :
                    pseudoLocJob = true;
                    jobId = Process_PseudoLocalize();
                    if (jobId > 0)
                        Util_ShowJobInfo(jobId);
                    break;
                case "transmem":
                    transMemJob = true;
                    jobId = Process_TranslationMemory();
                    if (jobId > 0)
                        Util_ShowJobInfo(jobId);
                    break;
                //default:
                //    if (Page.IsPostBack == false)
                //    {
                //        Display_All();
                //    }
                //    break;
            }
            Util_SetLabels();
            Util_SetJS();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }


    #endregion


    #region Process

    protected bool Util_HasTargetLocales()
    {
        bool hasTargetLocales = true;
        long[] targetJobs = (new List<long>() { m_iID }).ToArray();
        Ektron.Cms.BusinessObjects.Localization.L10nManager businessMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.m_refContentApi.RequestInformationRef);
        string title = "Content with no Target Locale";

        LocalizationExportJob exportJob = new LocalizationExportJob(title);
        exportJob.SourceLanguageId = 1033;
        exportJob.Items = new List<LocalizableItem>();
        exportJob.Items.Add(new LocalizableItem(LocalizableCmsObjectType.LocaleTaxonomy, m_iID));
        exportJob.XliffVersion = this.xliffVersion;

        Criteria<LocalizationExportJob> criteria = new Criteria<LocalizationExportJob>();
        criteria.PagingInfo.RecordsPerPage = 5;
        List<LocalizableItem> fullList = businessMgr.GetLocaleTaxonomyContents(exportJob);

        if (fullList.Count > 0)
        {
            LocalizableItem item = fullList.ElementAt(0);  
            List<LocalizationObjectData> localeObjectData = new List<LocalizationObjectData>();
            Ektron.Cms.Framework.Localization.LocalizationObject localizationObject = new Ektron.Cms.Framework.Localization.LocalizationObject();

            localeObjectData = localizationObject.GetLocalizationObjectList(LocalizableCmsObjectType.Content, item.Id, -1);
            if (0 == localeObjectData.Count)
            {
                localeObjectData = localizationObject.GetLocalizationObjectList(LocalizableCmsObjectType.DmsAsset, item.Id, -1);
            }
            if (0 == localeObjectData.Count)
            {
                hasTargetLocales = false;
                Util_ShowError("No Content found.");
                ltr_JobStatus.Text = "Error";
            }
            else if (1 == localeObjectData.Count)
            {
                hasTargetLocales = false;
                Util_ShowError(this.GetMessage("lbl target languages are not set for this handoff package"));
                ltr_JobStatus.Text = "Error";
            }
        }
        return hasTargetLocales;
    }

    protected bool Util_HasInvalidDependency()
    {
        bool hasDependency = false;
        long[] targetJobs = (new List<long>() { m_iID }).ToArray();
        Ektron.Cms.BusinessObjects.Localization.L10nManager businessMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.m_refContentApi.RequestInformationRef);
        string title = "Content Not Ready for Translation";
        
        LocalizationExportJob exportJob = new LocalizationExportJob(title);
        exportJob.SourceLanguageId = 1033;
        exportJob.Items = new List<LocalizableItem>();
        exportJob.Items.Add(new LocalizableItem(LocalizableCmsObjectType.LocaleTaxonomy, m_iID));
        exportJob.XliffVersion = this.xliffVersion;

        Criteria<LocalizationExportJob> criteria = new Criteria<LocalizationExportJob>();
        criteria.PagingInfo.RecordsPerPage = 50;
        List<Ektron.Cms.BusinessObjects.Localization.ILocalizable> items = businessMgr.GetNotReadyList(exportJob, criteria);

        if (items.Count > 0)
        {
            hasDependency = true;
            Util_ShowError("Dependencies are not ready for localization.");
            ltr_JobStatus.Text = "Warning";
            rptNotReadyContent.Visible = true;
            rptNotReadyContent.DataSource = items;
            rptNotReadyContent.DataBind();
        }
        invalidDependency = hasDependency;
        return hasDependency;
    }
    protected long Process_PseudoLocalize()
    {
        //Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
        //List<Ektron.Cms.Localization.LocaleData> pseudoLocales = locale.GetEnabledLocales().FindAll(d => d.XmlLang.Contains("-x-pseudo"));

        //this.languageIds = pseudoLocales.ConvertAll<int>(delegate(Ektron.Cms.Localization.LocaleData l) { return l.Id; });

        if (0 == this.languageIds.Count)
        {
            return 0;
        }

        //if (this.localeMgr.FindLocale(pseudoLocales, contentData.LanguageId) != null)
        //{
        //    return;
        //}

        LocalizableCmsObjectType locType = LocalizableCmsObjectType.Content;
        LocalizationExportJob job = new LocalizationExportJob(pseudoLocJobTitle);

        job.SourceLanguageId = 1033;
        job.XliffVersion = this.xliffVersion;

        foreach (long contentId in contentIds)
        {
            long contentType = this.m_refContentApi.EkContentRef.GetContentType(contentId);
            if (Ektron.Cms.Common.EkConstants.IsAssetContentType(contentType, true))
                locType = LocalizableCmsObjectType.DmsAsset;
            job.AddItem(locType, contentId, this.languageIds);
        }
        job.TargetLanguageIds = this.languageIds;
        job.PseudoLocalize = true; // allows content "NotReady" to be exported

        Ektron.Cms.BusinessObjects.Localization.L10nManager businessMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.m_refContentApi.RequestInformationRef);
        return businessMgr.StartExportForTranslation(job);
    }
    protected long Process_TranslationMemory()
    {
        if (0 == this.languageIds.Count)
        {
            return 0;
        }
        Ektron.Cms.Framework.Localization.LocalizationObject lobjApi = new Ektron.Cms.Framework.Localization.LocalizationObject();
        LocalizableCmsObjectType locType = LocalizableCmsObjectType.Content;
        LocalizationExportJob job = new LocalizationExportJob(transMemJobTitle);

        job.SourceLanguageId = this.languageIds[0];
        job.XliffVersion = this.xliffVersion;

        foreach (long contentId in contentIds)
        {
            long contentType = this.m_refContentApi.EkContentRef.GetContentType(contentId);
            if (Ektron.Cms.Common.EkConstants.IsAssetContentType(contentType, true))
                locType = LocalizableCmsObjectType.DmsAsset;
            job.AddItem(locType, contentId, this.languageIds);
            if (this.languageIds[0] != null)
                lobjApi.MarkNeedsTranslation(locType, contentId, this.languageIds[0]);
        }
        job.TargetLanguageIds = this.languageIds;

        Ektron.Cms.BusinessObjects.Localization.L10nManager businessMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.m_refContentApi.RequestInformationRef);
        return businessMgr.StartExportForTranslation(job);
    }
    

    #endregion


    #region Display



    #endregion


    #region Private Helpers


    protected void Util_ShowError(string errorMsg)
    {
        pnl_error.Visible = true;
        ltr_Error.Text = errorMsg;
    }
    protected void Util_ShowJobInfo(long jobId)
    {
        bool suppressRedir = true;
        List<string> filePaths = new List<string>();
        string icon = "";
        int jobState = -1;

        if (jobId == 0 && showJobId > 0)
        {
            jobId = showJobId;
        }
        LocalizationJobDataSet.LocalizationJobRow jr = localApi.GetJobByID(jobId);
        if (jr == null)
        {
            jobId = (jobId - 1);
            jr = localApi.GetJobByID(jobId);
        }
        else if (jr.JobTitle == pseudoLocJobTitle)
        {
            pseudoLocJob = true;
            suppressRedir = false;
        }
        else if (jr.JobTitle == transMemJobTitle)
        {
            transMemJob = true;
            suppressRedir = false;
        }
        if (!pseudoLocJob)
        {
            long getJobId = jobId;
            if (transMemJob)
            {
                Ektron.Cms.LocalizationJobDataSet.LocalizationJobDataTable transMemJobs = localApi.GetJobs(jobId);
                foreach (Ektron.Cms.LocalizationJobDataSet.LocalizationJobRow transmemjob in transMemJobs)
                {
                    if ((int)Ektron.Cms.LocalizationJobDataSet.LocalizationJobRow.Types.CompressFiles == transmemjob.JobType)
                    {
                        getJobId = transmemjob.JobID;
                        break;
                    }
                }
            }
            LocalizationJobDataSet.LocalizationJobFileDataTable filesInJob = localApi.GetFilesByJob(getJobId);

            dg_viewjob.DataSource = filesInJob.Rows;
            dg_viewjob.DataBind();

            if (filesInJob.Rows.Count > 0)
            {
                string urlBase = Ektron.Cms.Common.EkFunctions.GetURLBase(Request.ServerVariables["Server_name"], Convert.ToInt32(Request.ServerVariables["SERVER_PORT"]), (Page.Request.Url.Scheme == "https"), 443);
                foreach (LocalizationJobDataSet.LocalizationJobFileRow jobFile in filesInJob)
                    filePaths.Add(urlBase + SitePath.Remove(0, 1) + "uploadedfiles/localization/" + jobFile.FileUrl);
            }
        }
        if (jr != null)
            jobState = jr.State;
        
        switch (jobState)
        {
            case -1:
                icon = "error.png";
                showFiles = false;
                break;
            case 3:
                icon = "translationReady.png";
                showFiles = !pseudoLocJob;
				break;
            case 4:
                icon = "translationNotTranslatable.png";
                break;
            case 5:
                icon = "translationNeedsTranslation.png";
                showFiles = !pseudoLocJob;
                break;
            case 1:
            case 2 :
                icon = "../loading_small.gif";
                if (!suppressRedir)
                {
                    if (pseudoLocJob || transMemJob)
                        ClientScript.RegisterStartupScript(this.GetType(), "Refresh", "setTimeout(\"location.href = '" + m_sPageName + "?action=displayhandoff&jobid=" + jobId.ToString() + "';\",1500);", true);
                    else
                        ClientScript.RegisterStartupScript(this.GetType(), "Refresh", "setTimeout(\"location.href = '" + m_sPageName + "?action=displayhandoff&id=" + m_iID.ToString() + "';\",1500);", true);
                }
                break;
        }
        ltr_JobStatus.Text = (string.Format("<img src=\"" + localApi.AppImgPath + "../UI/icons/{0}\"/>&#160;&#160;", icon));
        ltr_JobStatus.Text += Util_GetJobStateName(jobState);
        if (jobState == 3)
        {
            pnlCopy.Visible = false;
            hdnFilePaths.Value = string.Join(Environment.NewLine, filePaths.ToArray());
        }
    }
    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "displayhandoff":
                if (!(pseudoLocJob || transMemJob))
                {
                    workareamenu displayActionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                    displayActionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/cancel.png", "Close", "parent.CloseJobsModal();");
                    this.AddMenu(displayActionMenu);
                    SetTitleBarToString("Export status");
                }
                break;
            case "createhandoff":
                workareamenu createActionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                if (invalidDependency)
                {
                    string overrideLink = string.Format("{0}?action=createhandoff&id={1}&test={2}&override=true"
                        , m_sPageName
                        , m_iID
                        , isTest);
                    createActionMenu.AddLinkItem(m_refContentApi.AppPath + "images/UI/Icons/translationExport.png", "Export", overrideLink);
                    createActionMenu.AddBreak();
                }
                createActionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/cancel.png", "Close", "parent.CloseJobsModal();");
                this.AddMenu(createActionMenu);
                if (isTest)
                    SetTitleBarToString("Create test handoff");
                else
                    SetTitleBarToString("Create handoff");
                break;
            //default:
            //    workareamenu defaultActionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
            //    // defaultActionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), "document.forms[0].submit();");
            //    // defaultActionMenu.AddBreak();
            //    defaultActionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/cancel.png", "Close", "parent.$ektron(\'.ektronExportHandoffModal\').modalHide();");
            //    this.AddMenu(defaultActionMenu);

            //    SetTitleBarToString("lbl reorder reorder shipping methods");
            //    AddHelpButton("ReorderShippingMethods");
            //    break;
        }
    }
    protected void Util_SetJS()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
    }
    protected void Util_GetValues()
    {
        if (Request.QueryString["test"] != null)
        {
            isTest = Convert.ToBoolean(Request.QueryString["test"]);
        }
        if (Request.QueryString["override"] != null)
        {
            isOverride = Convert.ToBoolean(Request.QueryString["override"]);
        }
        if (Request.QueryString["contentIds"] != null)
        {
            string[] contents = Request.QueryString["contentIds"].Split(new char[] { ',' });
            foreach (string content in contents)
                if (Ektron.Cms.Common.EkFunctions.ReadDbLong(content) > 0)
                    contentIds.Add(Ektron.Cms.Common.EkFunctions.ReadDbLong(content));
        }
        if (Request.QueryString["languageIds"] != null)
        {
            string[] langs = Request.QueryString["languageIds"].Split(new char[] {','});
            foreach (string lang in langs)
                if (Ektron.Cms.Common.EkFunctions.ReadIntegerValue(lang, 0) > 0)
                    languageIds.Add(Ektron.Cms.Common.EkFunctions.ReadIntegerValue(lang, 0));
        }
        if (Request.QueryString["jobid"] != null)
        {
            showJobId = Ektron.Cms.Common.EkFunctions.ReadDbLong(Request.QueryString["jobid"]);
        }
            
    }
    protected string Util_GetJobStateName(int status)
    {
        string statusName = "";
        switch (status)
        {
            case -1:
                statusName = "Error";
                break;
            case 1:
                statusName = "Initializing";
                break;
            case 2:
                statusName = "Running";
                break;
            case 3:
                statusName = "Complete";
                break;
            case 4:
                statusName = "Canceled";
                break;
            case 5:
                statusName = "CompleteWithErrors";
                break;
        }
        return statusName;
    }
    protected string Util_GetItemIcon(Ektron.Cms.BusinessObjects.Localization.ILocalizable item)
    {
        string icon = "<img src=\"{0}\"/>";
        switch (item.LocalizableType)
        {
            case LocalizableCmsObjectType.Menu:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/menu.png");
                break;
            case LocalizableCmsObjectType.MenuItem:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/menuItem.png");
                break;
            case LocalizableCmsObjectType.FolderContents:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/folderView.png");
                break;
            case LocalizableCmsObjectType.FolderTree:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/folder.png");
                break;
            case LocalizableCmsObjectType.Taxonomy:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/taxonomy.png");
                break;
            case LocalizableCmsObjectType.LocaleTaxonomy:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/taxonomy.png");
                break;
            case LocalizableCmsObjectType.Content:
                icon = string.Format(
                    icon,
                    AppPath + "images/ui/icons/contenthtml.png");
                break;
            case LocalizableCmsObjectType.DmsAsset:
                Ektron.Cms.ContentData dmsAsset = (Ektron.Cms.ContentData)item.DataObject;
                icon = string.Format(
                    icon,
                    dmsAsset.AssetData.ImageUrl);
                break;
            default:
                break;
        }

        return icon;
    }
   

    #endregion
   

}


