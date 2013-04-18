using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;
using Ektron.Cms.Sync.Client;

public partial class Workarea_sync_Restore : System.Web.UI.Page, ISyncRestoreView
{
    private const string RelativeImagePath = "images/ui/icons/";
    private const string FileIcon = "content.png";
    private const string FolderIcon = "folder.png";

    private SyncRestorePresenter _presenter;
    private RestoreParameters _parameters;
    private SiteAPI _siteApi;
    private StyleHelper _styleHelper;

    public Workarea_sync_Restore()
    {
        _presenter = new SyncRestorePresenter(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
        _parameters = new RestoreParameters(Request);

        RegisterResources();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        RenderHeader();

        if (!IsPostBack)
        {
            _presenter.InitializeView(_parameters.RelationshipId);
        }
        else
        {
            RestoreSelectedFiles();
        }
    }

    #region ISyncRestoreView Members

    public void Bind()
    {
        if (Files != null)
        {
            PopulateTree();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public FileSyncNode Files { get; set; }

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

    /// <summary>
    /// 
    /// </summary>
    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Restore.js", "EktronSyncRestoreJS");

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Css.RegisterCss(this, "css/ektron.workarea.sync.restore.css", "EktronSyncRestoreCss");

        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

    /// <summary>
    /// 
    /// </summary>
    private void RenderHeader()
    {
        // Set title bar message

        divTitleBar.InnerText = _siteApi.EkMsgRef.GetMessage("sync restore title");

        // Add toolbar button: 'Back'

        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "back.png",
            "Sync.aspx?action=viewallsync",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty, StyleHelper.BackButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellBackButton);

        // Add toolbar button: 'Restore'

        HtmlTableCell cellRestoreButton = new HtmlTableCell();       
        cellRestoreButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "restore.png",
            "#",
            _siteApi.EkMsgRef.GetMessage("btn sync restore alt"),
            _siteApi.EkMsgRef.GetMessage("btn sync restore"),
            "onclick=\"Ektron.Workarea.Sync.Restore.Submit();\"", StyleHelper.RestoreButtonCssClass,true);

        rowToolbarButtons.Cells.Add(cellRestoreButton);

		rowToolbarButtons.Cells.Add(StyleHelper.ActionBarDividerCell);

        // Add toolbar button: 'Help'
		HtmlTableCell cellHelpButton = new HtmlTableCell();
        cellHelpButton.InnerHtml = _styleHelper.GetHelpButton("SyncRestore_v85", string.Empty);

        rowToolbarButtons.Cells.Add(cellHelpButton);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PopulateTree()
    {
        // Load tree's header label.

        lblRestoreFiles.Text = "Restore files removed during a previous synchronization:";

        // Render file tree.
        tvFileHierarchy.Nodes.Clear();

        tvFileHierarchy.NodeIndent = 25;
        tvFileHierarchy.ShowCheckBoxes = TreeNodeTypes.All;
        tvFileHierarchy.ShowExpandCollapse = true;

        TreeNode rootNode = new TreeNode();
        rootNode.Text = "/";
        rootNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
        rootNode.Expanded = true;

        tvFileHierarchy.Nodes.Add(rootNode);

        PopulateTree(Files.Nodes, rootNode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="parent"></param>
    private void PopulateTree(List<FileSyncNode> nodes, TreeNode parent)
    {
        if (nodes != null)
        {
            foreach (FileSyncNode fileNode in nodes)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = Path.GetFileName(fileNode.Path);
                treeNode.Expanded = false;
                treeNode.Value = fileNode.Path;
                treeNode.SelectAction = TreeNodeSelectAction.None;

                if (fileNode.IsDirectory)
                {
                    treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FolderIcon;
                    PopulateTree(fileNode.Nodes, treeNode);
                }
                else
                {
                    treeNode.ImageUrl = _siteApi.AppPath + RelativeImagePath + FileIcon;
                }

                parent.ChildNodes.Add(treeNode);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RestoreSelectedFiles()
    {
        bool success = true;

        foreach (TreeNode node in tvFileHierarchy.CheckedNodes)
        {
            // Loop through each checked leaf node
            if (node.ChildNodes.Count == 0)
            {
                Debug.WriteLine(node.Value);
            }
        }

        if (success)
        {
            divStatusMessage.Attributes.Add("class", "success");
            divStatusMessage.InnerText = _siteApi.EkMsgRef.GetMessage("sync restore success");
        }
        else
        {            
            divStatusMessage.Attributes.Add("class", "error");
            divStatusMessage.InnerText = _siteApi.EkMsgRef.GetMessage("sync restore error");
        }
    }
}
