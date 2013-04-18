//-----------------------------------------------------------------------
// <copyright file="localizationjobs.aspx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using EkJobDS = Ektron.Cms.LocalizationJobDataSet;
using EkJobStates = Ektron.Cms.LocalizationJobDataSet.LocalizationJobRow.States;
using EkJobTypes = Ektron.Cms.LocalizationJobDataSet.LocalizationJobRow.Types;
using EkSklTypes = Ektron.Cms.LocalizationJobDataSet.LocalizationSkeletonRow.Types;

/// <summary>
/// Shows the history of localization export and import
/// </summary>
public partial class Workarea_localizationjobs : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    #region Member Variables

    /// <summary>
    /// The object to the SiteAPI
    /// </summary>
    private Ektron.Cms.SiteAPI siteApi = new Ektron.Cms.SiteAPI();

    /// <summary>
    /// The object to the LocalizationAPI
    /// </summary>
    private Ektron.Cms.LocalizationAPI localizationApi = new Ektron.Cms.LocalizationAPI();

    /// <summary>
    /// True if at least one job is currently running.
    /// </summary>
    private bool isAnyJobRunning;

    /// <summary>
    /// Indicates whether the job is the first job or not (used for paging)
    /// </summary>
    private bool isFirstJob = false;

    /// <summary>
    /// The maximum number of jobs to display
    /// </summary>
    private const int MaxNumJobsToShow = 25;

    /// <summary>
    /// Cache for method GetLocaleData
    /// </summary>
    private Dictionary<int, Ektron.Cms.Localization.LocaleData> methodGetLocaleDataCache = null;

    #endregion

    /// <summary>
    /// Types of messages
    /// </summary>
    public enum MessageStyleType
    {
        /// <summary>
        /// An error message
        /// </summary>
        ErrorMessage,

        /// <summary>
        /// An informational message
        /// </summary>
        Information
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        ltrConfirmDelMsg.Text = GetMessage("js: confirm delete localization history");
        tvJobs.CollapseImageToolTip = GetMessage("lbl collapse");
        tvJobs.ExpandImageToolTip = GetMessage("lbl expand");

        tvJobs.Load += this.tvJobs_Load;
        tvJobs.SelectedNodeChanged += this.tvJobs_SelectedNodeChanged;
        tvJobs.TreeNodePopulate += this.tvJobs_TreeNodePopulate;

        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(GetCommonApi().RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Xliff, false))
        {
            Utilities.ShowError(GetMessage("feature locked error"));
        }
        else
        {
            string strCancelJobID = Request.QueryString["cancel"];
            long cancelJobId = 0;
            if (long.TryParse(strCancelJobID, out cancelJobId))
            {
                this.localizationApi.CancelJob(cancelJobId);
            }

            Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(GetCommonApi().RequestInformationRef);
            string strDeleteJobID = Request.QueryString["delete"];
            long deleteJobId = 0;
            if (long.TryParse(strDeleteJobID, out deleteJobId))
            {
                l10nMgr.DeleteJob(deleteJobId);
            }

            string strDeleteAllBeforeJobID = Request.QueryString["deleteall"];
            long deleteAllBeforeJobId = 0;
            if (long.TryParse(strDeleteAllBeforeJobID, out deleteAllBeforeJobId))
            {
                l10nMgr.DeleteJobAndOlder(deleteAllBeforeJobId);
            }
        }
    }

    protected void tvJobs_Load(object sender, System.EventArgs e)
    {
        if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(GetCommonApi().RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Xliff, false))
        {
            this.isAnyJobRunning = false;
            this.AddJobs(tvJobs);
            if (this.isAnyJobRunning)
            {
                Response.AddHeader("Refresh", "3"); // in seconds
            }
        }
    }

    protected void tvJobs_SelectedNodeChanged(object sender, System.EventArgs e)
    {
        string strNodeType = null; // 3 chars
        int id = 0;

        strNodeType = tvJobs.SelectedNode.Value.Substring(0, 3).ToLower();
        string strID = tvJobs.SelectedNode.Value.Substring(3);
        if (!int.TryParse(strID, out id))
        {
            return;
        }

        switch (strNodeType)
        {
            case "pgj":
                tvJobs.Nodes.Remove(tvJobs.SelectedNode);
                this.AddJobs(tvJobs, id);
                break;
            default: 
                // Nothing to do
                break;
        }
    }

    protected void tvJobs_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        string strNodeType = null; // 3 chars
        long id = 0;

        strNodeType = e.Node.Value.Substring(0, 3).ToLower();
        string strID = e.Node.Value.Substring(3);
        if (!long.TryParse(strID, out id))
        {
            return;
        }

        switch (strNodeType)
        {
            case "run":
                this.PopulateRunNode(id, e);
                break;
            case "job":
                this.PopulateJobNode(id, e);
                break;
            case "zip":
                this.PopulateZipNode(id, e);
                break;
            case "skl":
                this.PopulateSklNode(id, e);
                break;
            case "xlf":
                this.PopulateXlfNode(id, e);
                break;
            case "msg":
                // Nothing to do
                break;
            default:
                // Nothing to do
                break;
        }
    }

    private void PopulateRunNode(long id, TreeNodeEventArgs e)
    {
        this.AddJobsInJob(id, e);
    }

    private void PopulateJobNode(long id, TreeNodeEventArgs e)
    {
        this.AddJobsInJob(id, e);
        this.AddZipsInJob(id, e);
        this.AddSklsInJob(id, e);
    }

    private void PopulateZipNode(long id, TreeNodeEventArgs e)
    {
        this.AddMsgsInZip(id, e);
        this.AddZipsInZip(id, e);
        this.AddXlfsInZip(id, e);
    }

    private void PopulateSklNode(long id, TreeNodeEventArgs e)
    {
        this.AddXlfsInSkl(id, e);
    }

    private void PopulateXlfNode(long id, TreeNodeEventArgs e)
    {
        this.AddMsgsInXlf(id, e);
    }

    private void AddJobs(TreeView tv)
    {
        this.AddJobs(tv, 0);
    }

    private void AddJobs(TreeView tv, int startAt)
    {
        EkJobDS.LocalizationJobDataTable dt = this.localizationApi.GetJobs();
        int count = dt.Count;
        bool moreJobsExist = false;

        // Check StartAt=0 to avoid a strange effect.
        // Clicking 'More...' at the end of the first set works fine.
        // Clicking 'More...' at the end of the second set resets the list to show just the first set.
        // So, rather than spend time tracking it down, we'll just show all the jobs.
        // There also seems to be a defect in the TreeView where the onmouseover will throw a JavaScript
        // error if the list is long and hasn't finished loading yet.
        if (MaxNumJobsToShow > 0 && (count > startAt + MaxNumJobsToShow) && startAt == 0)
        {
            count = startAt + MaxNumJobsToShow;
            moreJobsExist = true;
        }

        this.isFirstJob = 0 == startAt;
        for (int i = startAt; i <= count - 1; i++)
        {
            tv.Nodes.Add(this.CreateJobNode(dt[i]));
            this.isFirstJob = false;
        }

        if (moreJobsExist)
        {
            TreeNode node = new TreeNode();
            node.Value = "pgj" + count;
            node.PopulateOnDemand = false;
            node.SelectAction = TreeNodeSelectAction.Select;
            node.Text = GetMessage("alt more");
            tv.Nodes.Add(node);
        }
    }

    private void AddJobsInJob(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationJobDataTable dt = this.localizationApi.GetJobs(id);
        foreach (EkJobDS.LocalizationJobRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateJobNode(data));
        }
    }

    private TreeNode CreateJobNode(EkJobDS.LocalizationJobRow data)
    {
        TreeNode node = new TreeNode();
        StringBuilder sb = new StringBuilder();
        EkJobStates state = (EkJobStates)data.State;
        EkJobTypes jobType = (EkJobTypes)data.JobType;

        bool isJobComplete = EkJobDS.LocalizationJobRow.IsJobDone(state);
        bool jobHasErrors = EkJobStates.CompleteWithErrors == state || EkJobStates.Canceled == state;
        bool isJobRunning = !isJobComplete && !data.IsCurrentStepNull() && !data.IsTotalStepsNull() && data.CurrentStep <= data.TotalSteps;

        node.Value = "job" + data.JobID;
        node.PopulateOnDemand = true;
        node.SelectAction = TreeNodeSelectAction.Expand;

        sb.Append("<div class=\"L10nJob\">");
        sb.Append("<div class=\"L10nJobTitle\">");
        if (state == EkJobStates.CompleteWithErrors)
        {
            sb.Append(this.FormatErrorMessage(GetMessage("lbl complete with alerts")));
        }

        switch (jobType)
        {
            case EkJobTypes.CompressFiles:
                sb.Append(GetMessage("lbl downloadable zip files"));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/icons/FileTypes/zip.png";
                node.ImageToolTip = GetMessage("alt zipped file");
                if (!this.methodCreateJobNode_isFirstCompressFilesJobExpanded)
                {
                    this.methodCreateJobNode_isFirstCompressFilesJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.Export:
                sb.Append(string.Format(GetMessage("lbl exported locale taxonomy") + " \"{0}\"", data.JobTitle));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl exported locale taxonomy");
                if (!this.methodCreateJobNode_isFirstExportJobExpanded)
                {
                    this.methodCreateJobNode_isFirstExportJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.ExportContent:
                sb.Append(string.Format(GetMessage("lbl exported content") + " \"{0}\"", data.JobTitle));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl exported content");
                if (!this.methodCreateJobNode_isFirstExportJobExpanded)
                {
                    this.methodCreateJobNode_isFirstExportJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.ExportFolder:
                sb.Append(string.Format(GetMessage("lbl exported folder") + " \"{0}\"", data.JobTitle));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl exported folder");
                if (!this.methodCreateJobNode_isFirstExportJobExpanded)
                {
                    this.methodCreateJobNode_isFirstExportJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.ExportMenu:
                sb.Append(GetMessage("lbl exported menus"));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl exported menus");
                if (!this.methodCreateJobNode_isFirstExportJobExpanded)
                {
                    this.methodCreateJobNode_isFirstExportJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.ExportTaxonomy:
                sb.Append(GetMessage("lbl exported taxonomy"));
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl exported taxonomy");
                if (!this.methodCreateJobNode_isFirstExportJobExpanded)
                {
                    this.methodCreateJobNode_isFirstExportJobExpanded = true;
                    node.Expand();
                }

                break;
            case EkJobTypes.ExtractText:
                sb.Append(GetMessage("lbl extracted"));
                if (!data.IsJobTitleNull() && data.JobTitle.Length > 0)
                {
                    sb.Append(" " + data.JobTitle);
                }

                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationExport.png";
                node.ImageToolTip = GetMessage("lbl extracted");
                break;
            case EkJobTypes.Import:
                sb.Append(GetMessage("lbl imported"));
                if (!data.IsJobTitleNull() && data.JobTitle.Length > 0)
                {
                    sb.Append(" " + data.JobTitle);
                }

                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationImport.png";
                node.ImageToolTip = GetMessage("lbl imported");
                break;
            case EkJobTypes.MergeText:
                sb.Append(GetMessage("lbl merged"));
                if (!data.IsJobTitleNull() && data.JobTitle.Length > 0)
                {
                    sb.Append(" " + data.JobTitle);
                }

                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translationImport.png";
                node.ImageToolTip = GetMessage("lbl merged");
                break;
            case EkJobTypes.UncompressFiles:
                sb.Append(GetMessage("lbl uncmpresed file"));
                if (!data.IsJobTitleNull() && data.JobTitle.Length > 0)
                {
                    sb.Append(" " + data.JobTitle);
                }

                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/zip.png";
                node.ImageToolTip = GetMessage("lbl uncmpresed file");
                break;
            default:
                sb.Append(data.JobTitle);
                break;
        }

        sb.Append("</div>");
        sb.Append(this.FormatDateTime(data.StartTime, "L10nJobStartTime"));
        if (isJobRunning && data.TotalSteps > 0)
        {
            float percentProgress = data.CurrentStep / data.TotalSteps;
            int progressWidth = 0;
            progressWidth = Convert.ToInt32(Math.Ceiling(100 * percentProgress));
            //// " + String.Format("{0:0%}", percentProgress) + "
            sb.Append("<div class=\"L10nJobProgress\"><img height=\"10\" width=\"" + progressWidth + "\" src=\"" + this.GetImageUrl("dbl_grad_blue.gif") + "\" border=\"0\" /></div>");
            node.ImageUrl = this.GetImageUrl("loading_small.gif");
            node.Value = "run" + data.JobID;
            node.Expand();
        }
        else
        {
            sb.Append("<div class=\"L10nJobState\">");
            switch (state)
            {
                case EkJobStates.Canceled:
                    sb.Append(GetMessage("lbl canceled"));
                    break;
                case EkJobStates.Complete:
                    sb.Append(GetMessage("lbl complete"));
                    break;
                case EkJobStates.CompleteWithErrors:
                    sb.Append(GetMessage("lbl alert"));
                    break;
                case EkJobStates.Initializing:
                    sb.Append(GetMessage("lbl initializing"));
                    break;
                case EkJobStates.Running:
                    sb.Append(GetMessage("lbl running"));
                    break;
                default:
                    sb.Append(data.State);
                    break;
            }

            sb.Append("</div>");
        }

        if (this.isFirstJob && (!this.methodCreateJobNode_isFirstJobExpanded || jobHasErrors))
        {
            this.methodCreateJobNode_isFirstJobExpanded = true;
            node.Expand();
        }

        if (!isJobComplete)
        {
            this.isAnyJobRunning = true;
        }

        sb.Append("</div>");
        if (data.IsParentJobIDNull())
        {
            if (isJobRunning)
            {
                sb.Append(" <a href=\"?cancel=" + data.JobID + "\" class=\"L10nJobCancel\">" + GetMessage("lnk cancel") + "</a>");
            }
            else 
            {
                sb.Append(" <a href=\"?delete=" + data.JobID + "\" class=\"L10nJobDelete\" onclick=\"return confirmDeleteAction();\">" + GetMessage("lnk delete") + "</a>");
                sb.Append(" <a href=\"?deleteall=" + data.JobID + "\" class=\"L10nJobDeleteAllPrevious\" onclick=\"return confirmDeleteAction();\">" + GetMessage("lnk delete all below") + "</a>");
            }
        }

        node.Text = sb.ToString();

        return node;
    }

    private bool methodCreateJobNode_isFirstJobExpanded = false;
    
    private bool methodCreateJobNode_isFirstExportJobExpanded = false;
    
    private bool methodCreateJobNode_isFirstCompressFilesJobExpanded = false;

    private void AddZipsInJob(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationJobFileDataTable dt = this.localizationApi.GetFilesByJob(id);
        foreach (EkJobDS.LocalizationJobFileRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateZipNode(data));
        }
    }

    private void AddZipsInZip(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationJobFileDataTable dt = this.localizationApi.GetFilesInFile(id);
        foreach (EkJobDS.LocalizationJobFileRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateZipNode(data));
        }
    }

    private void AddMsgsInZip(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationJobFileRow data = this.localizationApi.GetFileByID(id);
        if (data != null && data.ErrorMessage != null && data.ErrorMessage.Length > 0)
        {
            e.Node.ChildNodes.Add(this.CreateMsgNode(data.ErrorMessage));
        }
    }

    private TreeNode CreateZipNode(EkJobDS.LocalizationJobFileRow data)
    {
        TreeNode node = new TreeNode();
        StringBuilder sb = new StringBuilder();
        string strUrl = null;
        string strName = null;
        string strFileExt = null;

        strFileExt = System.IO.Path.GetExtension(data.FileName).ToLower();
        strUrl = Ektron.Cms.Common.EkFunctions.QualifyURL(this.localizationApi.GetLocalizationUrl(), data.FileUrl);
        node.Value = "zip" + data.FileID;
        node.PopulateOnDemand = false;
        node.SelectAction = TreeNodeSelectAction.None;

        switch (strFileExt)
        {
            case ".zip":
                node.PopulateOnDemand = true;
                node.SelectAction = TreeNodeSelectAction.Expand;
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/zip.png";
                node.ImageToolTip = GetMessage("alt zip file");
                break;
            case ".xlf":
            case ".xml":
                // .xml for Trados
                if (data.ErrorMessage != null && data.ErrorMessage.Length > 0)
                {
                    node.PopulateOnDemand = true;
                    node.SelectAction = TreeNodeSelectAction.Expand;
                }

                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translation.png";
                node.ImageToolTip = GetMessage("alt xliff file");
                break;
            default:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/text.png";
                node.ImageToolTip = GetMessage("lbl generic file");
                break;
        }

        if (File.Exists(Server.MapPath(strUrl)))
        {
            node.PopulateOnDemand = false;
            node.SelectAction = TreeNodeSelectAction.Select;
            node.NavigateUrl = strUrl;
            sb.Append("<div class=\"L10nZipLink\">");
        }
        else
        {
            node.NavigateUrl = string.Empty;
            sb.Append("<div class=\"L10nZip\">");
        }

        sb.Append("<div class=\"L10nZipFileName\">");
        sb.Append(this.FormatErrorMessage(data.ErrorMessage));
        strName = data.FileName;
        if (!data.IsTargetLanguageNull())
        {
            Ektron.Cms.Localization.LocaleData sourceLocale = this.GetLocaleData(data.SourceLanguage);
            Ektron.Cms.Localization.LocaleData targetLocale = this.GetLocaleData(data.TargetLanguage);
            if (targetLocale != null)
            {
                string targetFlag = targetLocale.FlagUrl;
                if (targetFlag.Length > 0)
                {
                    if (!data.IsSourceLanguageNull() && sourceLocale != null)
                    {
                        string sourceFlag = sourceLocale.FlagUrl;
                        if (sourceFlag.Length > 0)
                        {
                            // 8594 is right arrow
                            sb.AppendFormat("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" class=\"L10nFlag\" />&#160;&#8594;&#160;", sourceFlag, sourceLocale.EnglishName);
                        }
                    }

                    sb.AppendFormat("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" class=\"L10nFlag\" />&#160;", targetFlag, targetLocale.EnglishName);
                    strName = targetLocale.CombinedName;
                }
            }
        }

        sb.Append(strName);
        sb.Append("</div>");
        sb.Append("<div class=\"L10nZipFileSize\">");
        if (data.IsFileSizeNull())
        {
            sb.Append("&#160;");
        }
        else
        {
            sb.Append(FormatFileSize(data.FileSize));
        }

        sb.Append("</div>");
        sb.Append("</div>");

        node.Text = sb.ToString();

        if (!this.methodCreateZipNode_isFirstErrorExpanded && data.ErrorMessage != null && data.ErrorMessage.Length > 0)
        {
            this.methodCreateZipNode_isFirstErrorExpanded = true;
            node.Expand();
        }

        return node;
    }

    private bool methodCreateZipNode_isFirstErrorExpanded = false;

    private void AddSklsInJob(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationSkeletonDataTable dt = this.localizationApi.GetContentItemsByJob(id);
        foreach (EkJobDS.LocalizationSkeletonRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateSklNode(data));
        }
    }

    private TreeNode CreateSklNode(EkJobDS.LocalizationSkeletonRow data)
    {
        // Special case: skeleton may be used as a placeholder to no skeletons
        EkSklTypes itemType = (EkSklTypes)data.ItemType;
        if (itemType == EkSklTypes.Information)
        {
            if (data.IsErrorMessageNull() || 0 == data.ErrorMessage.Length)
            {
                return this.CreateMsgNode(data.Title, MessageStyleType.Information);
            }
            else
            {
                return this.CreateMsgNode(data.ErrorMessage, MessageStyleType.ErrorMessage);
            }
        }

        TreeNode node = new TreeNode();
        StringBuilder sb = new StringBuilder();
        string strToolTipForItemID = string.Empty;

        node.Value = "skl" + data.SkeletonID;
        node.PopulateOnDemand = true;
        node.SelectAction = TreeNodeSelectAction.Expand;
        switch (itemType)
        {
            case EkSklTypes.Content:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/contentHtml.png";
                node.ImageToolTip = GetMessage("lbl content");
                strToolTipForItemID = GetMessage("generic content id");
                break;
            case EkSklTypes.Form:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/contentForm.png";
                node.ImageToolTip = GetMessage("form text");
                strToolTipForItemID = GetMessage("alt form id");
                break;
            case EkSklTypes.SmartFormData:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/xml.png";
                node.ImageToolTip = GetMessage("lbl smart form");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.SmartFormDesign:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/contentSmartForm.png";
                node.ImageToolTip = GetMessage("generic xml configuration");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Menu:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/menu.png";
                node.ImageToolTip = GetMessage("generic menu title");
                strToolTipForItemID = GetMessage("alt menu number");
                break;
            case EkSklTypes.MenuItem:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/menu.png";
                node.ImageToolTip = GetMessage("alt menu items");
                strToolTipForItemID = GetMessage("alt menu items number");
                break;
            case EkSklTypes.Taxonomy:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/taxonomy.png";
                node.ImageToolTip = GetMessage("generic taxonomy lbl");
                strToolTipForItemID = GetMessage("alt taxonomy number");
                break;
            case EkSklTypes.Image:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/image.png";
                node.ImageToolTip = GetMessage("generic image");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Audio:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/audio.png";
                node.ImageToolTip = GetMessage("lbl audio");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Video:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/video.png";
                node.ImageToolTip = GetMessage("lbl video");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.PDF:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/acrobat.png";
                node.ImageToolTip = GetMessage("content:asset:pdf");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.MsWord:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/word.png";
                node.ImageToolTip = GetMessage("content:mso:doc");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.MsExcel:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/excel.png";
                node.ImageToolTip = GetMessage("content:mso:xls");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.MsPowerPoint:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/powerpoint.png";
                node.ImageToolTip = GetMessage("content:mso:ppt");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.MsPublisher:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/publisher.png";
                node.ImageToolTip = GetMessage("content:mso:pub");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.MsVisio:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/visio.png";
                node.ImageToolTip = GetMessage("content:mso:vsd");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Flash:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/flash.png";
                node.ImageToolTip = GetMessage("lbl adobe flash");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Compressed:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/zip.png";
                node.ImageToolTip = GetMessage("content:asset:zip");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.Document:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/text.png";
                node.ImageToolTip = GetMessage("lbl dms documents");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.HtmlFile:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/html.png";
                node.ImageToolTip = GetMessage("lbl html");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.XmlFile:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/xml.png";
                node.ImageToolTip = GetMessage("lbl xml");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.TextFile:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/text.png";
                node.ImageToolTip = GetMessage("content:asset:txt");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.File:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/file.png";
                node.ImageToolTip = GetMessage("lbl generic file");
                strToolTipForItemID = GetMessage("generic id");
                break;
            case EkSklTypes.DmsAsset:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/contentDMSDocument.png";
                node.ImageToolTip = GetMessage("lbl dms documents");
                strToolTipForItemID = GetMessage("generic id");
                break;
            default:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/FileTypes/text.png";
                node.ImageToolTip = data.ItemType.ToString();
                strToolTipForItemID = GetMessage("generic id");
                break;
        }

        sb.Append("<div class=\"L10nSkl\">");
        sb.Append("<div class=\"L10nSklItemID\"");
        if (strToolTipForItemID.Length > 0)
        {
            sb.Append(" title=\"" + strToolTipForItemID + "\">");
            sb.Append(data.ItemID);
        }
        else
        {
            sb.Append(">");
        }

        sb.Append("</div>");
        sb.Append("<div class=\"L10nSklTitle\">");
        sb.Append(this.FormatErrorMessage(data.ErrorMessage));
        sb.Append(data.Title);
        sb.Append("</div>");
        if (data.IsLastEditDateNull())
        {
            sb.Append(this.FormatDateTime(DateTime.MinValue, "L10nSklDate"));
        }
        else
        {
            sb.Append(this.FormatDateTime(data.LastEditDate, "L10nSklDate", GetMessage("generic date modified")));
        }

        sb.Append("</div>");

        node.Text = sb.ToString();

        if (!this.methodCreateSklNode_isFirstErrorExpanded && data.ErrorMessage != null && data.ErrorMessage.Length > 0)
        {
            this.methodCreateSklNode_isFirstErrorExpanded = true;
            node.Expand();
        }

        return node;
    }

    private bool methodCreateSklNode_isFirstErrorExpanded = false;

    private void AddXlfsInZip(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationTargetDataTable dt = this.localizationApi.GetInterchangeFilesInFile(id);
        foreach (EkJobDS.LocalizationTargetRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateXlfNode(data));
        }
    }

    private void AddXlfsInSkl(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationTargetDataTable dt = this.localizationApi.GetInterchangeFilesByContentItem(id);
        foreach (EkJobDS.LocalizationTargetRow data in dt.Rows)
        {
            e.Node.ChildNodes.Add(this.CreateXlfNode(data));
        }
    }

    private void AddMsgsInXlf(long id, TreeNodeEventArgs e)
    {
        EkJobDS.LocalizationTargetRow data = this.localizationApi.GetInterchangeFileByID(id);
        if (data != null && data.ErrorMessage != null && data.ErrorMessage.Length > 0)
        {
            e.Node.ChildNodes.Add(this.CreateMsgNode(data.ErrorMessage));
        }
    }

    private TreeNode CreateXlfNode(EkJobDS.LocalizationTargetRow data)
    {
        TreeNode node = new TreeNode();
        StringBuilder sb = new StringBuilder();
        string strHover = string.Empty;
        EkJobDS.LocalizationTargetRow.States state = (EkJobDS.LocalizationTargetRow.States)data.State;

        node.Value = "xlf" + data.TargetID;
        if (data.ErrorMessage == null || 0 == data.ErrorMessage.Length)
        {
            node.PopulateOnDemand = false;
            node.SelectAction = TreeNodeSelectAction.None;
            strHover = " onmouseover=\"myTVNodeHover(this)\" onmouseout=\"myTVNodeUnhover(this)\"";
        }
        else
        {
            node.PopulateOnDemand = true;
            node.SelectAction = TreeNodeSelectAction.Expand;
        }

        node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/translation.png";
        node.ImageToolTip = GetMessage("alt xliff");
        sb.Append("<div class=\"L10nXlf\"" + strHover + ">");
        sb.Append("<div class=\"L10nXlfFileName\">");
        sb.Append(this.FormatErrorMessage(data.ErrorMessage));
        if (!data.IsTargetLanguageNull())
        {
            Ektron.Cms.Localization.LocaleData targetLocale = this.GetLocaleData(data.TargetLanguage);
            if (targetLocale != null)
            {
                string targetFlag = targetLocale.FlagUrl;
                if (targetFlag.Length > 0)
                {
                    sb.AppendFormat("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" class=\"L10nFlag\" />&#160;", targetFlag, targetLocale.EnglishName);
                }
            }
        }

        sb.Append(data.FileName);
        sb.Append("</div>");
        if (data.IsLastEditDateNull())
        {
            sb.Append(this.FormatDateTime(DateTime.MinValue, "L10nXlfDate"));
        }
        else
        {
            sb.Append(this.FormatDateTime(data.LastEditDate, "L10nXlfDate", GetMessage("generic date modified")));
        }

        sb.Append("<div class=\"L10nXlfState\">");
        if (data.IsStateNull())
        {
            sb.Append("&#160;");
        }
        else
        {
            switch (state)
            {
                case EkJobDS.LocalizationTargetRow.States.ErrorOccurred:
                    sb.Append(GetMessage("lbl error"));
                    if (!this.methodCreateXlfNode_isFirstErrorExpanded && data.ErrorMessage != null && data.ErrorMessage.Length > 0)
                    {
                        this.methodCreateXlfNode_isFirstErrorExpanded = true;
                        node.Expand();
                    }

                    break;
                case EkJobDS.LocalizationTargetRow.States.NeedsReview:
                    sb.Append(GetMessage("lbl needs review"));
                    break;
                case EkJobDS.LocalizationTargetRow.States.NeedsTranslation:
                    sb.Append(GetMessage("lbl needs translation"));
                    break;
                case EkJobDS.LocalizationTargetRow.States.SignedOff:
                    sb.Append(GetMessage("lbl signed off"));
                    break;
                case EkJobDS.LocalizationTargetRow.States.Translated:
                    sb.Append(GetMessage("lbl translated"));
                    break;
                default:
                    sb.Append(state.ToString());
                    break;
            }
        }

        sb.Append("</div>");
        sb.Append("</div>");

        node.Text = sb.ToString();

        return node;
    }

    private bool methodCreateXlfNode_isFirstErrorExpanded = false;

    private TreeNode CreateMsgNode(string message)
    {
        return this.CreateMsgNode(message, MessageStyleType.ErrorMessage);
    }

    private TreeNode CreateMsgNode(string message, MessageStyleType messageStyle)
    {
        TreeNode node = new TreeNode();
        StringBuilder sb = new StringBuilder();

        node.Value = "msg";
        node.PopulateOnDemand = false;
        node.SelectAction = TreeNodeSelectAction.None;
        switch (messageStyle)
        {
            case MessageStyleType.ErrorMessage:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/error.png";
                node.ImageToolTip = GetMessage("lbl alert");
                sb.Append("<div class=\"L10nErrorMessage\">");
                sb.Append(message);
                sb.Append("</div>");
                break;
            case MessageStyleType.Information:
                node.ImageUrl = GetCommonApi().AppPath + "images/UI/Icons/information.png";
                node.ImageToolTip = GetMessage("lbl desc");
                sb.Append("<div class=\"L10nMessage\">");
                sb.Append(message);
                sb.Append("</div>");
                break;
        }

        node.Text = sb.ToString();

        return node;
    }

    private string FormatErrorMessage(string errorMessage)
    {
        StringBuilder sb = new StringBuilder();
        if (errorMessage != null && errorMessage.Length > 0)
        {
            sb.AppendFormat("<img class=\"L10nErrorAlert\" src=\"{0}images/UI/Icons/error.png\" alt=\"{1}\" title=\"{1}\" />", GetCommonApi().AppPath, Ektron.Cms.Common.EkFunctions.HtmlEncode(errorMessage));
        }

        return sb.ToString();
    }

    private string FormatDateTime(DateTime dt, string className)
    {
        return this.FormatDateTime(dt, className, string.Empty);
    }

    private string FormatDateTime(DateTime dt, string className, string toolTip)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"" + className + "\"");
        if (dt != DateTime.MinValue)
        {
            string strToolTip = dt.ToString();
            if (toolTip != null && toolTip.Length > 0)
            {
                strToolTip = toolTip + ": " + strToolTip;
            }

            sb.Append(" title=\"" + strToolTip + "\">");
            sb.Append(dt.ToString("d"));
        }
        else
        {
            sb.Append(">&#160;");
        }

        sb.Append("</div>");
        return sb.ToString();
    }

    private static string FormatFileSize(long fileSize)
    {
        if (fileSize < 1024)
        {
            return string.Format("{0:##,##0} bytes", fileSize);
        }
        else if (fileSize < 10 * 1024 * 1024)
        {
            return string.Format("{0:N} KB", (fileSize / 1024));
        }
        else
        {
            return string.Format("{0:N} MB", (fileSize / (1024 * 1024)));
        }
    }

    private string GetImageUrl(string fileName)
    {
        return Ektron.Cms.Common.EkFunctions.QualifyURL(GetCommonApi().AppImgPath, fileName);
    }

    private Ektron.Cms.Localization.LocaleData GetLocaleData(int localeId)
    {
        Ektron.Cms.Localization.LocaleData data = null;
        if (localeId <= 0)
        {
            return data;
        }

        if (null == this.methodGetLocaleDataCache)
        {
            this.methodGetLocaleDataCache = new Dictionary<int, Ektron.Cms.Localization.LocaleData>();
        }

        if (this.methodGetLocaleDataCache.ContainsKey(localeId))
        {
            data = this.methodGetLocaleDataCache[localeId];
        }
        else
        {
            data = this.LocaleManager.GetItem(localeId);
            this.methodGetLocaleDataCache.Add(localeId, data);
        }

        return data;
    }
}
