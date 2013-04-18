using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;

public partial class Workarea_sync2_SyncReview : System.Web.UI.Page, ISyncConflictReviewView
{
    private const string UserConflictIdName = "userConflictIds";
    private const string FolderConflictIdName = "folderConflictIds";
    private const string MetadataConflictIdName = "metadataConflictIds";
    private const string EmailConflictIdName = "emailConflictIds";

    private StyleHelper _styleHelper;
    private SiteAPI _siteApi;
    private SyncReviewPresenter _presenter;

    public Workarea_sync2_SyncReview()
    {
        _styleHelper = new StyleHelper();
        _siteApi = new SiteAPI();
        _presenter = new SyncReviewPresenter(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        Utilities.ValidateUserLogin();
        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        RenderHeader();
        RegisterResources();
        PopulateLabels();

        rptUserConflicts.ItemDataBound += ConflictItemDataBound;
        rptMetadataConflicts.ItemDataBound += ConflictItemDataBound;
        rptFolderConflicts.ItemDataBound += ConflictItemDataBound;
        rptEmailConflicts.ItemDataBound += ConflictItemDataBound;

        btnMarkEmailReviewed.Click += btnMarkEmailReviewed_Click;
        btnMarkFoldersReviewed.Click += btnMarkFoldersReviewed_Click;
        btnMarkMetadataReviewed.Click += btnMarkMetadataReviewed_Click;
        btnMarkUsersReviewed.Click += btnMarkUsersReviewed_Click;
        btnMarkUsersReviewedAndEmail.Click += btnMarkUsersReviewedAndEmail_Click;

        _presenter.IntializeView();
    }

    #region ISyncConflictReviewView Members

    public void Bind()
    {
        rptUserConflicts.DataSource = UserConflicts;
        rptUserConflicts.DataBind();

        rptFolderConflicts.DataSource = FolderConflicts;
        rptFolderConflicts.DataBind();

        rptMetadataConflicts.DataSource = MetadataConflicts;
        rptMetadataConflicts.DataBind();

        rptEmailConflicts.DataSource = EmailConflicts;
        rptEmailConflicts.DataBind();
    }

    public List<ConflictReviewData> EmailConflicts { get; set; }

    public List<ConflictReviewData> FolderConflicts { get; set; }

    public List<ConflictReviewData> MetadataConflicts { get; set; }

    public List<ConflictReviewData> UserConflicts { get; set; }

    #endregion

    #region ISyncView Members

    public void DisplayError(string message)
    {

    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConflictItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ConflictReviewData data = e.Item.DataItem as ConflictReviewData;
        if (data != null)
        {
            Label lblItemOriginalName = e.Item.FindControl("lblItemOriginalName") as Label;
            Label lblItemModifiedName = e.Item.FindControl("lblItemModifiedName") as Label;
            
            if (lblItemOriginalName != null && lblItemModifiedName != null)
            {
                lblItemOriginalName.Text = data.ObjectName.Replace(data.ObjectId.ToString(), string.Empty);
                lblItemModifiedName.Text = data.ObjectName;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMarkUsersReviewedAndEmail_Click(object sender, EventArgs e)
    {
        List<ConflictReviewData> conflicts = GetSelectedConflicts(SyncConflictType.User);

        if (conflicts != null)
        {
            _presenter.MarkConflictsReviewed(conflicts);
            _presenter.EmailAffectedUsers(conflicts);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMarkUsersReviewed_Click(object sender, EventArgs e)
    {
        List<ConflictReviewData> conflicts = GetSelectedConflicts(SyncConflictType.User);

        if (conflicts != null)
        {
            _presenter.MarkConflictsReviewed(conflicts);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMarkMetadataReviewed_Click(object sender, EventArgs e)
    {
        List<ConflictReviewData> conflicts = GetSelectedConflicts(SyncConflictType.Metadata);

        if (conflicts != null)
        {
            _presenter.MarkConflictsReviewed(conflicts);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMarkFoldersReviewed_Click(object sender, EventArgs e)
    {
        List<ConflictReviewData> conflicts = GetSelectedConflicts(SyncConflictType.Folder);

        if (conflicts != null)
        {
            _presenter.MarkConflictsReviewed(conflicts);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMarkEmailReviewed_Click(object sender, EventArgs e)
    {
        List<ConflictReviewData> conflicts = GetSelectedConflicts(SyncConflictType.Message);

        if (conflicts != null)
        {
            _presenter.MarkConflictsReviewed(conflicts);
        }
    }

    private void PopulateLabels()
    {
        lblEmail.Text = _siteApi.EkMsgRef.GetMessage("generic email");
        lblFolders.Text = _siteApi.EkMsgRef.GetMessage("generic content title");
        lblMetadata.Text = _siteApi.EkMsgRef.GetMessage("metadata text");
        lblUsers.Text = _siteApi.EkMsgRef.GetMessage("generic users");

        btnMarkUsersReviewed.Text =btnMarkUsersReviewed.ToolTip = btnMarkMetadataReviewed.Text =btnMarkMetadataReviewed.ToolTip = btnMarkFoldersReviewed.Text =btnMarkFoldersReviewed.ToolTip = btnMarkEmailReviewed.Text = btnMarkEmailReviewed.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync mark reviewed");
        
        btnMarkUsersReviewedAndEmail.Text = btnMarkUsersReviewedAndEmail.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync mark reviewed and email");
         

        lblOriginalUserName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync original username");
        lblOriginalUserName.ToolTip = lblOriginalUserName.Text;
        lblModifiedUserName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync modified username");
        lblModifiedUserName.ToolTip = lblModifiedUserName.Text;
        lblOriginalFolderName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync original folder name");
        lblOriginalFolderName.ToolTip = lblOriginalFolderName.Text;
        lblModifiedFolderName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync modified folder name");
        lblModifiedFolderName.ToolTip = lblModifiedFolderName.Text;
        lblOriginalMetadataName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync original metadata name");
        lblOriginalMetadataName.ToolTip = lblOriginalMetadataName.Text;
        lblModifiedMetadataName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync modified metadata name");
        lblModifiedMetadataName.ToolTip = lblModifiedMetadataName.Text;
        lblOriginalEmailName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync original email name");
        lblOriginalEmailName.ToolTip = lblOriginalEmailName.Text;
        lblModifiedEmailName.Text = _siteApi.EkMsgRef.GetMessage("lbl sync modified email name");
        lblModifiedEmailName.ToolTip = lblModifiedEmailName.Text;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronStringJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS, false);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Review.js", "EktronSyncReviewJS", false);

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronThickBoxCss, false);
        Css.RegisterCss(this, "css/ektron.workarea.sync.review.css", "EktronSyncReviewCss", false);

        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

    /// <summary>
    /// 
    /// </summary>
    private void RenderHeader()
    {
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("lbl sync review resolved conflicts"));

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/back.png",
            "Sync.aspx",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty,
            StyleHelper.BackButtonCssClass, 
            true);

        rowToolbarButtons.Cells.Add(cellBackButton);

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("addsync_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<ConflictReviewData> GetSelectedConflicts(SyncConflictType type)
    {
        List<ConflictReviewData> conflicts = new List<ConflictReviewData>();

        string selectedIds = null;
        switch (type)
        {
            case SyncConflictType.Message:
                selectedIds = Request.Form[EmailConflictIdName];
                break;
            case SyncConflictType.Folder:
                selectedIds = Request.Form[FolderConflictIdName];
                break;
            case SyncConflictType.Metadata:
                selectedIds = Request.Form[MetadataConflictIdName];
                break;
            case SyncConflictType.User:
                selectedIds = Request.Form[UserConflictIdName];
                break;
        }

        if (!string.IsNullOrEmpty(selectedIds))
        {
            string[] splitIds = selectedIds.Split(
                new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string id in splitIds)
            {
                long objectId;
                if (long.TryParse(id, out objectId))
                {
                    ConflictReviewData data = new ConflictReviewData();
                    data.ObjectId = objectId;
                    data.Type = type;
                    data.ObjectName = string.Empty;
                    data.IsReviewed = true;

                    conflicts.Add(data);
                }
            }
        }

        return conflicts;
    }
}
