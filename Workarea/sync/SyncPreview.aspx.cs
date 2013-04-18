using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;

public partial class SyncPreview : System.Web.UI.Page, ISyncPreviewView
{
    private SiteAPI _siteApi;
    protected ContentAPI apicontent = new ContentAPI();
    private SyncPreviewPresenter _presenter;
    private StyleHelper _styleHelper;
    private PreviewParameters _parameters;

    public SyncPreview()
    {
        _siteApi = new SiteAPI();
        _presenter = new SyncPreviewPresenter(this);
        _styleHelper = new StyleHelper();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        Server.ScriptTimeout = 3600;
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }

        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        RegisterResources();

        // Localize Lables
        lblLocalDBTitle.Text = _siteApi.EkMsgRef.GetMessage("syncpreview local changes");
        lblLocalDBTitle.ToolTip = lblLocalDBTitle.Text;
        lblRemoteDBTitle.Text = _siteApi.EkMsgRef.GetMessage("syncpreview remote changes");
        lblRemoteDBTitle.ToolTip = lblRemoteDBTitle.Text;
        lblLocalFileTitle.Text = _siteApi.EkMsgRef.GetMessage("syncpreview local file changes");
        lblLocalFileTitle.ToolTip = lblLocalFileTitle.Text;
        lblRemoteFileTitle.Text = _siteApi.EkMsgRef.GetMessage("syncpreview remote file changes");
        lblRemoteFileTitle.ToolTip = lblRemoteFileTitle.Text;
        btnPreview.ToolTip = _siteApi.EkMsgRef.GetMessage("syncpreviewbtn");
        btnPreview.Text = btnPreview.Text;

        // Direction Options
        lblPreviewDirection.Text = _siteApi.EkMsgRef.GetMessage("syncpreviewdirectionlabel");
        lblPreviewDirection.ToolTip = lblPreviewDirection.Text;
        ddlDirection.ToolTip = _siteApi.EkMsgRef.GetMessage("sync preview direction");
        ddlDirection.Items.Clear();
        ddlDirection.Items.Add(new ListItem("Bidirectional", Ektron.Cms.Sync.Client.SyncDirection.Bidirectional.ToString()));
        ddlDirection.Items.Add(new ListItem("Download", Ektron.Cms.Sync.Client.SyncDirection.Download.ToString()));
        ddlDirection.Items.Add(new ListItem("Upload", Ektron.Cms.Sync.Client.SyncDirection.Upload.ToString()));

        // Preview Type Options
        lblPreviewProvider.Text = _siteApi.EkMsgRef.GetMessage("syncpreviewtypelabel");
        lblPreviewProvider.ToolTip = _siteApi.EkMsgRef.GetMessage("syncpreviewbtn");
        ddlProvider.ToolTip = _siteApi.EkMsgRef.GetMessage("syncpreviewbtn");
        ddlProvider.Items.Clear();
        ddlProvider.Items.Add(new ListItem("DB([dbo].[content])", Ektron.Cms.Sync.Client.SyncPreviewType.Database.ToString()));
        ddlProvider.Items.Add(new ListItem("AssetLibrary", Ektron.Cms.Sync.Client.SyncPreviewType.AssetLibrary.ToString()));
        ddlProvider.Items.Add(new ListItem("Assets", Ektron.Cms.Sync.Client.SyncPreviewType.Assets.ToString()));
        ddlProvider.Items.Add(new ListItem("Private Assets", Ektron.Cms.Sync.Client.SyncPreviewType.PrivateAssets.ToString()));
        ddlProvider.Items.Add(new ListItem("Uploaded Files", Ektron.Cms.Sync.Client.SyncPreviewType.UploadedFiles.ToString()));
        ddlProvider.Items.Add(new ListItem("Uploaded Images", Ektron.Cms.Sync.Client.SyncPreviewType.UploadedImages.ToString()));
        ddlProvider.Items.Add(new ListItem("Workarea", Ektron.Cms.Sync.Client.SyncPreviewType.WorkArea.ToString()));
        ddlProvider.Items.Add(new ListItem("Templates", Ektron.Cms.Sync.Client.SyncPreviewType.Templates.ToString()));

        //filter options 
        dllChanges.Items.Clear();
        dllChanges.Items.Add(new ListItem("None", Ektron.Cms.Sync.Client.FileChangeType.None.ToString()));
        dllChanges.Items.Add(new ListItem("New", Ektron.Cms.Sync.Client.FileChangeType.New.ToString()));
        dllChanges.Items.Add(new ListItem("Rename", Ektron.Cms.Sync.Client.FileChangeType.Rename.ToString()));
        dllChanges.Items.Add(new ListItem("Overwrite", Ektron.Cms.Sync.Client.FileChangeType.Overwrite.ToString()));
        dllChanges.Items.Add(new ListItem("Delete", Ektron.Cms.Sync.Client.FileChangeType.Delete.ToString()));
        // Hide result pane on initial load.
        if (!Page.IsPostBack)
        {
            resultWrapper.Visible = false;
        }
        else
        {
            resultWrapper.Visible = true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _parameters = new PreviewParameters(Request);

        string syncDirection = ddlDirection.SelectedValue;
        string syncProvider = ddlProvider.SelectedValue;
        string syncFileChange = dllChanges.SelectedValue;
        Ektron.Cms.Sync.Client.SyncDirection direction = (Ektron.Cms.Sync.Client.SyncDirection)Enum.Parse(
            typeof(Ektron.Cms.Sync.Client.SyncDirection), syncDirection);
        Ektron.Cms.Sync.Client.SyncPreviewType previewType = (Ektron.Cms.Sync.Client.SyncPreviewType)Enum.Parse(
            typeof(Ektron.Cms.Sync.Client.SyncPreviewType), syncProvider);
        Ektron.Cms.Sync.Client.FileChangeType changeType = (Ektron.Cms.Sync.Client.FileChangeType)Enum.Parse(
            typeof(Ektron.Cms.Sync.Client.FileChangeType), syncFileChange);
        this.Direction = direction;
        this.PreviewType = previewType;
        this.ChangeType = changeType;

        if (Page.IsPostBack && String.IsNullOrEmpty(Request.Form["btnPreview"]))
        {
            _presenter.Initialize(_parameters.Id, this.Direction, this.PreviewType, true);
            this.SetView(this.Direction.ToString(), this.PreviewType.ToString());
        }

        RenderHeader();
    }

    protected void btnPreviewClick(object sender, EventArgs e)
    {
        this.resetPaging();
        pnlFileChange.Visible = this.PreviewType != SyncPreviewType.Database;
        _presenter.Initialize(_parameters.Id, this.Direction, this.PreviewType, false);
        this.SetView(this.Direction.ToString(), this.PreviewType.ToString());
    }
    protected void btnFilterClick(object sender, EventArgs e)
    {
        this.resetPaging();
        _presenter.Initialize(_parameters.Id, this.Direction, this.PreviewType, true);
        this.SetView(this.Direction.ToString(), this.PreviewType.ToString());
        pnlFileChange.Visible = this.PreviewType != SyncPreviewType.Database;
    }
    private void SetView(string direction, string provider)
    {
        // Direction
        liTabLocal.Attributes.Add("style", "visibility:visible;");
        liTabRemote.Attributes.Add("style", "visibility:visible;");
        remotedbChanges.Visible = true;
        remoteFileChanges.Visible = true;
        localdbChanges.Visible = true;
        localFileChanges.Visible = true;

        switch (direction)
        {
            case "Bidirectional":
                liTabLocal.Visible = true;
                liTabRemote.Visible = true;
                break;
            case "Download":
                liTabLocal.Visible = false;
                liTabRemote.Visible = true;
                localdbChanges.Visible = false;
                localFileChanges.Visible = false;
                break;
            case "Upload":
            default:
                liTabLocal.Visible = true;
                liTabRemote.Visible = false;
                remotedbChanges.Visible = false;
                remoteFileChanges.Visible = false;
                break;
        }

        // Db or File
        string previewType = (provider == "Database") ? "Database" : "File";
        switch (previewType)
        {
            case "File":
                mvChanges.SetActiveView(vFile);
                localdbChanges.Visible = false;
                remotedbChanges.Visible = false;
                break;
            case "Database":
            default:
                mvChanges.SetActiveView(vDB);
                localFileChanges.Visible = false;
                remoteFileChanges.Visible = false;
                break;
        }
    }

    private void resetPaging()
    {
        ucPagingLocalDB.SelectedPage = 0;
        ucPagingRemoteDB.SelectedPage = 0;
        ucPagingLocalFile.SelectedPage = 0;
        ucPagingRemoteFile.SelectedPage = 0;
        ucPagingLocalDB.CurrentPageIndex = 0;
        ucPagingRemoteDB.CurrentPageIndex = 0;
        ucPagingLocalFile.CurrentPageIndex = 0;
        ucPagingRemoteFile.CurrentPageIndex = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronSiteData);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
        JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "SyncRelationshipJS");
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Preview.js", "SyncPreviewJS");

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronThickBoxCss);
        Css.RegisterCss(this, "css/ektron.workarea.sync.preview.css", "SyncPreviewCss");

        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

    /// <summary>
    /// 
    /// </summary>
    private void RenderHeader()
    {
        string backUrl = _parameters.Referrer ?? "Sync.aspx";
        txtTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("lbl sync preview header"));

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/back.png",
            backUrl,
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty, StyleHelper.BackButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellBackButton);

        HtmlTableCell cellSyncButton = new HtmlTableCell();
        cellSyncButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/sync.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            "rel=\"" + _parameters.Id.ToString() + "\" onclick=\"Ektron.Workarea.Sync.Relationships.Synchronize(this);\"", StyleHelper.SyncButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellSyncButton);

        
    }

    #region ISyncPreviewView Members

    /// <summary>
    /// 
    /// </summary>
    public void Bind()
    {
        if (Data != null)
        {
            ddlDirection.SelectedValue = this.Direction.ToString();
            ddlProvider.SelectedValue = this.PreviewType.ToString();

            setDataForLocalDB();
            setDataForRemoteDB();
            setDataForLocalFile();
            setDataForRemoteFile();
        }
    }

    private void setDataForLocalDB()
    {
        // Local DB Changes Grid
        gvLocalDBChanges.DataSource = Data.LocalChanges.databaseChanges;
        gvLocalDBChanges.PageSize = _siteApi.RequestInformationRef.PagingSize;
        gvLocalDBChanges.PageIndex = ucPagingLocalDB.SelectedPage;
        gvLocalDBChanges.DataBind();

        if (Data.LocalChanges.databaseChanges != null && Data.LocalChanges.databaseChanges.Count > 0)
        {
            if (gvLocalDBChanges.PageCount > 1)
            {
                ucPagingLocalDB.TotalPages = gvLocalDBChanges.PageCount;
                ucPagingLocalDB.CurrentPageIndex = gvLocalDBChanges.PageIndex;
                ucPagingLocalDB.Visible = true;
            }
            else
            {
                ucPagingLocalDB.Visible = false;
            }

            lblLocalDBTitle.Visible = true;
            ltrLocalDBPreview.Visible = false;
        }
        else
        {
            ltrLocalDBPreview.Visible = true;
            ltrLocalDBPreview.Text = _siteApi.EkMsgRef.GetMessage("no preview items");
            lblLocalDBTitle.Visible = false;
            ucPagingLocalDB.Visible = false;
        }
    }

    private void setDataForRemoteDB()
    {
        // Remote DB Changes Grid
        gvRemoteDBChanges.DataSource = Data.RemoteChanges.databaseChanges;
        gvRemoteDBChanges.PageSize = _siteApi.RequestInformationRef.PagingSize;
        gvRemoteDBChanges.PageIndex = ucPagingRemoteDB.SelectedPage;
        gvRemoteDBChanges.DataBind();

        if (Data.RemoteChanges.databaseChanges != null && Data.RemoteChanges.databaseChanges.Count > 0)
        {
            if (gvRemoteDBChanges.PageCount > 1)
            {
                ucPagingRemoteDB.TotalPages = gvRemoteDBChanges.PageCount;
                ucPagingRemoteDB.CurrentPageIndex = gvRemoteDBChanges.PageIndex;
                ucPagingRemoteDB.Visible = true;
            }
            else
            {
                ucPagingRemoteDB.Visible = false;
            }

            lblRemoteDBTitle.Visible = true;
            ltrRemoteDBPreview.Visible = false;
        }
        else
        {
            ltrRemoteDBPreview.Visible = true;
            ltrRemoteDBPreview.Text = _siteApi.EkMsgRef.GetMessage("no preview items");
            lblRemoteDBTitle.Visible = false;
            ucPagingRemoteDB.Visible = false;
        }
    }

    private void setDataForLocalFile()
    {
        // Local File Changes Grid
        System.Collections.Generic.List<FileChange> fileChanges = null;
        if (this.ChangeType != FileChangeType.None)
        {
            fileChanges = Data.LocalChanges.fileChanges.FindAll(delegate(FileChange c) { return c.FileChangeType == this.ChangeType; });
        }
        else
        {
            fileChanges = Data.LocalChanges.fileChanges;
        }
        gvLocalFileChanges.DataSource = fileChanges;
        gvLocalFileChanges.PageSize = _siteApi.RequestInformationRef.PagingSize;
        gvLocalFileChanges.PageIndex = ucPagingLocalFile.SelectedPage;
        gvLocalFileChanges.DataBind();

        if (Data.LocalChanges.fileChanges != null && Data.LocalChanges.fileChanges.Count > 0)
        {
            if (gvLocalFileChanges.PageCount > 1)
            {
                ucPagingLocalFile.TotalPages = gvLocalFileChanges.PageCount;
                ucPagingLocalFile.CurrentPageIndex = gvLocalFileChanges.PageIndex;
                ucPagingLocalFile.Visible = true;
            }
            else
            {
                ucPagingLocalFile.Visible = false;
            }

            lblLocalFileTitle.Visible = true;
            ltrLocalFilePreview.Visible = false;
        }
        else
        {
            ltrLocalFilePreview.Visible = true;
            ltrLocalFilePreview.Text = _siteApi.EkMsgRef.GetMessage("no preview items");
            lblLocalFileTitle.Visible = false;
            ucPagingLocalFile.Visible = false;
        }
    }

    private void setDataForRemoteFile()
    {
        // Remote file Changes Grid
        System.Collections.Generic.List<FileChange> fileChanges = null;
        if (this.ChangeType != FileChangeType.None)
        {
            fileChanges = Data.RemoteChanges.fileChanges.FindAll(delegate(FileChange c) { return c.FileChangeType == this.ChangeType; });
        }
        else
        {
            fileChanges = Data.RemoteChanges.fileChanges;
        }
        gvRemoteFileChanges.DataSource = fileChanges;
        gvRemoteFileChanges.PageSize = _siteApi.RequestInformationRef.PagingSize;
        gvRemoteFileChanges.PageIndex = ucPagingRemoteFile.SelectedPage;
        gvRemoteFileChanges.DataBind();

        if (Data.RemoteChanges.fileChanges != null && Data.RemoteChanges.fileChanges.Count > 0)
        {
            if (gvRemoteFileChanges.PageCount > 1)
            {
                ucPagingRemoteFile.TotalPages = gvRemoteFileChanges.PageCount;
                ucPagingRemoteFile.CurrentPageIndex = gvRemoteFileChanges.PageIndex;
                ucPagingRemoteFile.Visible = true;
            }
            else
            {
                ucPagingRemoteFile.Visible = false;
            }

            lblRemoteFileTitle.Visible = true;
            ltrRemoteFilePreview.Visible = false;
        }
        else
        {
            ltrRemoteFilePreview.Visible = true;
            ltrRemoteFilePreview.Text = _siteApi.EkMsgRef.GetMessage("no preview items");
            lblRemoteFileTitle.Visible = false;
            ucPagingRemoteFile.Visible = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public SyncPreviewData Data { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Ektron.Cms.Sync.Client.SyncDirection Direction { get; set; }

    public Ektron.Cms.Sync.Client.SyncPreviewType PreviewType { get; set; }

    public Ektron.Cms.Sync.Client.FileChangeType ChangeType { get; set; }

    #endregion

    #region ISyncView Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    public void DisplayError(string error)
    {
        divErrorMessage.InnerText = error;
        pnlPreview.Visible = false;
        pnlError.Visible = true;
    }
    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null && ((Ektron.Cms.Sync.Client.FileChange)(e.Row.DataItem)).FileChangeType == FileChangeType.Delete)
                e.Row.CssClass = "delete";
    }
 
    #endregion
}
