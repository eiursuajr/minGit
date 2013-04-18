using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web.Parameters;
using Ektron.FileSync.Common;
using System.Web.UI.HtmlControls;

public partial class SyncCustomConfig : Page, ISyncCustomConfigView
{
    private readonly SyncCustomConfigPresenter _presenter;
    private readonly SiteAPI _siteApi;
    private readonly StyleHelper _styleHelper;

    private CustomConfigParameters _parameters;

    public ScopeConfiguration scopeConfig { get; set; }

    public List<string> AvaillableEntities { get; set; }


    public SyncCustomConfig()
    {
        _presenter = new SyncCustomConfigPresenter(this);
        _styleHelper = new StyleHelper();
        _siteApi = new SiteAPI();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Utilities.ValidateUserLogin())
            return;
        if (!_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) &&
            !_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
        {
            Response.Redirect(_siteApi.AppPath + "login.aspx?fromLnkPg=1", true);
        }

        RegisterResources();
        PopulateLabels();
        ClearErrors();

        _parameters = new CustomConfigParameters(this.Request);
        _presenter.InitializeView(_parameters.Action);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        switch (_parameters.Action)
        {
            case CustomConfigPresentationMode.Edit:
                if (IsPostBack)
                {
                    Save();
                }
                break;
            case CustomConfigPresentationMode.View:
            default:
                lbAvaillableEntities.Attributes.Add("disabled", "disabled");
                lbSelectedEntities.Attributes.Add("disabled", "disabled");
                actionButtons.Visible = false;
                actionButtons2.Visible = false;
                break;
        }

    }

    protected override void Render(HtmlTextWriter writer)
    {
        foreach (ListItem sel in lbSelectedEntities.Items)
        {
            Page.ClientScript.RegisterForEventValidation(lbSelectedEntities.UniqueID, sel.Value);
            Page.ClientScript.RegisterForEventValidation(lbAvaillableEntities.UniqueID, sel.Value);
        }

        foreach (ListItem sel in lbAvaillableEntities.Items)
        {
            Page.ClientScript.RegisterForEventValidation(lbSelectedEntities.UniqueID, sel.Value);
            Page.ClientScript.RegisterForEventValidation(lbAvaillableEntities.UniqueID, sel.Value);
        }
        base.Render(writer);
    }

    public void Bind()
    {
        if (!Page.IsPostBack)
        {
            if (AvaillableEntities != null)
            {
                if (scopeConfig.Tables != null)
                {
                    foreach (string selected in scopeConfig.Tables)
                    {
                        AvaillableEntities.Remove(selected);
                    }
                }

                lbAvaillableEntities.DataSource = AvaillableEntities;
                lbAvaillableEntities.DataBind();

                if (scopeConfig.Tables != null)
                {
                    lbSelectedEntities.DataSource = scopeConfig.Tables;
                    lbSelectedEntities.DataBind();
                }
            }


        }

        this.RenderHeader();
    }

    public void DisplayError(string message)
    {
        divErrorMessage.Visible = true;
        divErrorMessage.InnerHtml = message;
    }

    private void Save()
    {
        List<string> selectedList = null;
        if (!String.IsNullOrEmpty(hdnSelectedEntities.Value))
        {
            selectedList = new List<string>(hdnSelectedEntities.Value.Remove(hdnSelectedEntities.Value.Length - 1).Split(','));

        }
        this.scopeConfig.Tables = selectedList;
        if (this.scopeConfig.Tables == null)
        {
            this.scopeConfig.SyncEnabled = false;
        }
        else
        {
            this.scopeConfig.SyncEnabled = true;
        }
        _presenter.Save(this.scopeConfig);
        Response.Redirect("SyncCustomConfig.aspx?action=view", true);
    }

    private void RegisterResources()
    {
        ektronClientScript.Text = _styleHelper.GetClientScript();
    }

    private void ClearErrors()
    {
        divErrorMessage.InnerText = string.Empty;
        divErrorMessage.Visible = false;
    }

    private void PopulateLabels()
    {
        lblavailable.Text = String.Format("{0}:", _siteApi.EkMsgRef.GetMessage("synccustomconfigavailablelbl"));
        lblselected.Text = String.Format("{0}:", _siteApi.EkMsgRef.GetMessage("synccustomconfigselectedlbl"));
    }

    private void RenderHeader()
    {
        switch (_parameters.Action)
        {
            case CustomConfigPresentationMode.Edit:
                RenderHeaderForEditMode();
                break;
            default:
            case CustomConfigPresentationMode.View:
                RenderHeaderForVewMode();
                break;
        }
    }

    private void RenderHeaderForVewMode()
    {
        // Title
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("synccustomconfigtitlebar"));

        // Edit
        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellEditButton = new HtmlTableCell();
            cellEditButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/" + "contentEdit.png",
                "SyncCustomConfig.aspx?action=edit&LangType=" + _parameters.Language.ToString(),
                _siteApi.EkMsgRef.GetMessage("synccustomconfigeditbtn"),
                _siteApi.EkMsgRef.GetMessage("synccustomconfigeditbtn"),
                string.Empty);

            rowToolbarButtons.Cells.Add(cellEditButton);
        }

        // Back
        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "back.png",
            "Sync.aspx",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty);

        rowToolbarButtons.Cells.Add(cellBackButton);
    }

    private void RenderHeaderForEditMode()
    {
        // Title
        divTitleBar.InnerHtml =
            _styleHelper.GetTitleBar(_siteApi.EkMsgRef.GetMessage("synccustomconfigtitlebar")); //view sync titlebar

        // Save
        if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
        {
            HtmlTableCell cellSaveButton = new HtmlTableCell();
            cellSaveButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
                _siteApi.AppPath + "images/ui/icons/save.png",
                "#",
                _siteApi.EkMsgRef.GetMessage("synccustomconfigsavebtn"),
                _siteApi.EkMsgRef.GetMessage("synccustomconfigsavebtn"),
                "onclick=\"Ektron.Workarea.Sync.CustomConfig.Save();\"");

            rowToolbarButtons.Cells.Add(cellSaveButton);
        }

        // Back
        HtmlTableCell cellBackButton = new HtmlTableCell();
        cellBackButton.InnerHtml = _styleHelper.GetButtonEventsWCaption(
            _siteApi.AppPath + "images/ui/icons/" + "back.png",
            "Sync.aspx",
            _siteApi.EkMsgRef.GetMessage("alt back button text"),
            _siteApi.EkMsgRef.GetMessage("btn back"),
            string.Empty);

        rowToolbarButtons.Cells.Add(cellBackButton);
    }

}