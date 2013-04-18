using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;
using Telerik.Web.UI;

public partial class SyncFiles : System.Web.UI.Page, ISyncFilesView
{
    private const string RelativeImagePath = "images/ui/icons/";
    private const string FileIcon = "menuItem.png";
    private const string FolderIcon = "folder.png";
    private const string TitleFormat = "{0} \"{1}\"";
    private const string StatusDialogScriptFormat = "onclick=\"Ektron.Workarea.Sync.Files.Submit({0});\"";

    private SyncFilesPresenter _presenter;
    private SyncFilesParameters _parameters;
    private SiteAPI _siteApi;
    private StyleHelper _styleHelper;

    /// <summary>
    /// Constructor
    /// </summary>
    public SyncFiles()
    {
        _presenter = new SyncFilesPresenter(this);
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Page_Init(object sender, EventArgs e)
    {
        if (!Utilities.ValidateUserLogin())
            return;
        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        RegisterResources();
    }

    public void Page_Load(object sender, EventArgs e)
    {
        _parameters = new SyncFilesParameters(Request);
        _presenter.InitializeView(_parameters.Id);
    }

    #region ISyncFilesView Members

    /// <summary>
    /// 
    /// </summary>
    public void Bind()
    {
        RenderHeader();

        if (!IsPostBack)
        {
            PopulateTree();
        }
    }

    /// <summary>
    /// Gets or sets the file tree to be displayed.
    /// </summary>
    public FileSyncNode Files { get; set; }

    /// <summary>
    /// Gets or sets the profile to be used in the synchronization
    /// activity.
    /// </summary>
    public Profile Data { get; set; }

    #endregion

    #region ISyncView Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void DisplayError(string message)
    {
        
    }

    #endregion

    private void RenderHeader()
    {
        // Set title bar message

        divTitleBar.InnerText = string.Format(
            TitleFormat,
            _siteApi.EkMsgRef.GetMessage("select sync files"),
            Data.Name);

        // Add toolbar button: 'Back'

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/back.png",
            _parameters.Referrer,
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty, StyleHelper.BackButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellBackButton);

        // Add toolbar button: 'Sync Now'

        HtmlTableCell cellSyncButton = new HtmlTableCell();
        cellSyncButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/sync.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            _siteApi.EkMsgRef.GetMessage("btn sync now"),
            string.Format(StatusDialogScriptFormat, _parameters.Id.ToString()), StyleHelper.SyncButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellSyncButton);

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        // Add toolbar button: 'Help'

        HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("syncfiles_ascx", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// 
    /// </summary>
    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronSiteData);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronStringJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS, false);
        JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS, false);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Relationships.js", "EktronSyncRelationshipsJS");
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Files.js", "EktronSyncFilesJS");

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss, false);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss, false);

        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PopulateTree()
    {
        if (Files != null)
        {
            lblSyncFilesHeader.Text = "Select CMS Files to Push:";
            
            rtvFiles.Nodes.Clear();
            rtvFiles.CheckBoxes = true;
            rtvFiles.EnableViewState = true;

            RadTreeNode rootNode = new RadTreeNode();
            rootNode.Text = Files.Name;
            rootNode.Value = Files.Path;
            rootNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
            rootNode.Expanded = false;
            rootNode.Checkable = false;
            rootNode.ExpandMode = TreeNodeExpandMode.ClientSide;

            rtvFiles.Nodes.Add(rootNode);

            PopulateTree(Files.Nodes, rootNode);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="parent"></param>
    private void PopulateTree(List<FileSyncNode> nodes, RadTreeNode parent)
    {
        if (nodes != null && parent != null && parent.Nodes.Count == 0)
        {           
            foreach (FileSyncNode fileNode in nodes)
            {
                RadTreeNode treeNode = new RadTreeNode();
                treeNode.Text = Path.GetFileName(fileNode.Path);
                treeNode.Value = fileNode.Path;

                if (fileNode.IsDirectory)
                {
                    treeNode.ExpandMode = TreeNodeExpandMode.ServerSide;
                    treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
                    treeNode.Expanded = false;
                    treeNode.Checkable = false;

                    PopulateTree(fileNode.Nodes, treeNode);          
                }
                else
                {
                    // Set leaf node's ExpandMode to ClientSide. This indicates to
                    // the control that all children have been loaded (none because it
                    // is a leaf) and prevents it from rendering an expand icon.
                    
                    // Note: Setting other expand properties (Expanded, etc.) will force
                    // the icon to render.

                    treeNode.ExpandMode = TreeNodeExpandMode.ClientSide;
                    treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FileIcon;
                    treeNode.Checkable = true;
                    treeNode.Checked = false;
                }

                parent.Nodes.Add(treeNode);
            }
        }
    }

    protected void rtvFiles_NodeExpand(object sender, RadTreeNodeEventArgs e)
    {
        FileSyncNode node = _presenter.GetFiles(e.Node.Value);
        if (node != null)
        {            
            PopulateTree(node.Nodes, e.Node);
        }

        e.Node.ExpandMode = TreeNodeExpandMode.ClientSide;
        e.Node.Expanded = true;
    }
}
