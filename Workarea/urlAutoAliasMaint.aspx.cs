using System;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.UrlAliasing;

public partial class Workarea_urlAutoAliasMaint : System.Web.UI.Page
{
    private string langType;
    private int langId = 0;
    protected string folderId;
    private long fId = 0;
    private System.Collections.Generic.List<EkEnumeration.AutoAliasType> _autoType;
    private System.Collections.Generic.List<string> _aliasExt;
    private System.Collections.Generic.List<EkEnumeration.AutoAliasNameType> _aliasNameType;
    private System.Collections.Generic.List<TaxonomyData> _autoTaxSource;
    private System.Collections.Generic.List<FolderData> _autoFolderSource;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected UrlAliasAutoApi _autoAliasAPI;
    protected UrlAliasManualApi _manualAliasAPI;
    protected long sourceID = 0;
    protected string sourcePath = string.Empty;
    protected string excludedPath = string.Empty;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        string pageAction;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        SetServerJSVariables();
        //Licensing For 7.6
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }

        _autoAliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
        _manualAliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();

        //Labels got from resource file
        lblPrimary.Text = msgHelper.GetMessage("lbl primary");
        lblPrimary.ToolTip = lblPrimary.Text;
        lblActive.Text = msgHelper.GetMessage("active label");
        lblActive.ToolTip = lblActive.Text;
        lblType.Text = msgHelper.GetMessage("lbl source") + " " + msgHelper.GetMessage("generic type");
        lblType.ToolTip = lblType.Text;
        lblSource.Text = msgHelper.GetMessage("lbl alias root");
        lblSource.ToolTip = lblSource.Text;
        lblExt.Text = msgHelper.GetMessage("lbl extension");
        lblExt.ToolTip = lblExt.Text;
        lblOriginal.Text = msgHelper.GetMessage("lbl original url");
        lblOriginal.ToolTip = lblOriginal.Text;
        lblExample.Text = msgHelper.GetMessage("lbl link ex preview");
        lblExample.ToolTip = lblExample.Text;
        lblNameSrc.Text = msgHelper.GetMessage("lbl alias format");
        lblNameSrc.ToolTip = lblNameSrc.Text;
        lblReplaceChar.Text = msgHelper.GetMessage("lbl rpl char");
        lblReplaceChar.ToolTip = lblReplaceChar.Text;
        lblPathList.Text = msgHelper.GetMessage("lbl exclude path");
        lblPathList.ToolTip = lblPathList.Text;
        lblQueryStringParam.Text = msgHelper.GetMessage("lbl querystringparam");
        lblQueryStringParam.ToolTip = lblQueryStringParam.Text;

        pageAction = Request.QueryString["action"];
        langType = Request.QueryString["Langtype"];
        folderId = Request.QueryString["fId"];

        int.TryParse(langType, out langId);
        _autoType = _autoAliasAPI.GetAutoAliasTypes();

        if ((string)(pageAction) == "addalias")
        {
            DisplayAdd();
        }
        else if ((string)(pageAction) == "view")
        {
            DisplayView();
        }
        else if ((string)(pageAction) == "editalias")
        {
            DisplayEdit();
        }
    }

    private void DisplayAdd()
    {
        ToolBar("add", 0);
        UrlAliasAutoData add_AutoAlias = new UrlAliasAutoData(0, 0, 0, "");
        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                long.TryParse(folderId, out fId);
                add_AutoAlias.SiteId = fId;
                if (ddltype.SelectedValue == "Taxonomy")
                {
                    add_AutoAlias.AutoAliasType = Ektron.Cms.Common.EkEnumeration.AutoAliasType.Taxonomy;
                }
                else
                {
                    add_AutoAlias.AutoAliasType = Ektron.Cms.Common.EkEnumeration.AutoAliasType.Folder;
                }
                if (!String.IsNullOrEmpty(Request.Form["frm_folder_id"]))
                {
                    sourceID = Convert.ToInt64(Request.Form["frm_folder_id"]);
                }
                add_AutoAlias.SourceId = sourceID;
                add_AutoAlias.FileExtension = (string)ddlExt.SelectedValue;
                if (ddlNameSrc.SelectedValue == "ContentTitle")
                {
                    add_AutoAlias.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentTitle;
                }
                else if (ddlNameSrc.SelectedValue == "ContentId")
                {
                    add_AutoAlias.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentId;
                }
                else
                {
                    add_AutoAlias.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentIdAndLanguage;
                }

                add_AutoAlias.Example = Request.Form["txtExample"];
                add_AutoAlias.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
                add_AutoAlias.ReplacementCharacter = (string)txtReplaceChar.Text;
                add_AutoAlias.SelectedPath = (string)txtFolderPath.Text;
                add_AutoAlias.SourceParmName = (string)txtQueryStringParam.Text;
                if (Request.Form["pathList"].ToString().ToLower() != "please select")
                {
                    add_AutoAlias.ExcludedPath = Request.Form["pathList"];
                }
                else
                {
                    add_AutoAlias.ExcludedPath = string.Empty;
                }
                try
                {
                    add_AutoAlias = _autoAliasAPI.Add(add_AutoAlias, _refContentApi.UserId);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + langType), false);
                    return;
                }
                Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=auto&fId=" + fId));
            }
        }
        else
        {
            if (langId == -1)
            {
                _autoType.Remove(Ektron.Cms.Common.EkEnumeration.AutoAliasType.Taxonomy);
                InitializeDropDown("Folder");
                lblFolderSelect.Visible = true;
                lblTaxonomySelect.Visible = false;
            }
            else
            {
                InitializeDropDown("Taxonomy");
                lblTaxonomySelect.Visible = true;
                lblFolderSelect.Visible = false;
            }
            ddlSource.Visible = false;
            activeChkBox.Checked = true;
            txtOriginal.Text = "/CMS400Demo/default.aspx?id=354";
            GetLinkExample(false);
        }
    }

    private void DisplayView()
    {
        UrlAliasAutoData data = InitializeData();
        ToolBar("view", data.AutoId);
        ddltype.SelectedValue = data.AutoAliasType.ToString();
        ddltype.Enabled = false;
        InitializeDropDown(data.AutoAliasType.ToString());
        ddlSource.Visible = false;
        txtFolderPath.Text = data.FormattedSelectedPath;
        excludedPath = data.ExcludedPath;
        ddlSource.Enabled = false;
        ddlExt.SelectedValue = data.FileExtension;
        ddlExt.Enabled = false;
        ddlNameSrc.SelectedValue = data.PageNameType.ToString();
        ddlNameSrc.Enabled = false;
        primaryChkBox.Enabled = false;
        activeChkBox.Checked = data.IsEnabled;
        activeChkBox.Enabled = false;
        txtReplaceChar.Text = data.ReplacementCharacter;
        txtReplaceChar.Enabled = false;
        txtExample.Value = data.Example;
        txtExample.Attributes.Add("disabled", "disabled");
        txtOriginal.Text = "/CMS400Demo/default.aspx?id=354";
        txtQueryStringParam.Enabled = false;
        if (data.SourceParmName != string.Empty)
        {
            txtQueryStringParam.Text = data.SourceParmName.ToString();
            txtOriginal.Text = txtOriginal.Text + "&" + data.SourceParmName + "=123";
        }
        lblFolderSelect.Visible = false;
        lblTaxonomySelect.Visible = false;
        pathList.Disabled = true;
        sourceID = data.SourceId;
        sourcePath = data.SelectedPath;
        excludedPath = data.ExcludedPath;
    }

    private void DisplayEdit()
    {
        ToolBar("edit", 0);
        UrlAliasAutoData data = null;

        data = InitializeData();
        ToolBar("edit", data.AutoId);
        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                if (ddltype.SelectedValue == "Taxonomy")
                {
                    data.AutoAliasType = Ektron.Cms.Common.EkEnumeration.AutoAliasType.Taxonomy;
                }
                else
                {
                    data.AutoAliasType = Ektron.Cms.Common.EkEnumeration.AutoAliasType.Folder;
                }
                if (!String.IsNullOrEmpty(Request.Form["frm_folder_id"]))
                {
                    sourceID = Convert.ToInt64(Request.Form["frm_folder_id"]);
                }
                data.SourceId = sourceID;
                data.FileExtension = (string)ddlExt.SelectedValue;
                if (ddlNameSrc.SelectedValue == "ContentTitle")
                {
                    data.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentTitle;
                }
                else if (ddlNameSrc.SelectedValue == "ContentId")
                {
                    data.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentId;
                }
                else
                {
                    data.PageNameType = Ektron.Cms.Common.EkEnumeration.AutoAliasNameType.ContentIdAndLanguage;
                }
                data.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
                data.Example = Request.Form["txtExample"];
                data.ReplacementCharacter = (string)txtReplaceChar.Text;
                data.SelectedPath = Request.Form["frm_folder_path"];
                data.SourceParmName = (string)txtQueryStringParam.Text;
                if (Request.Form["pathList"] != null)
                {
                    data.ExcludedPath = Request.Form["pathList"];
                }

                try
                {
                    data = _autoAliasAPI.Update(data, _refContentApi.UserId);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + Request.QueryString["Langtype"]), false);
                    return;
                }
                Response.Redirect((string)("urlautoaliasmaint.aspx?action=view&id=" + data.AutoId.ToString() + "&Langtype=" + Request.QueryString["Langtype"] + "&fid=" + Request.QueryString["fId"]));
            }
        }
        else
        {
            InitializeDropDown(data.AutoAliasType.ToString());
            if (data.AutoAliasType == Ektron.Cms.Common.EkEnumeration.AutoAliasType.Taxonomy && this.langId == -1)
            {
                lblTaxonomySelect.Visible = false;
            }
            ddlSource.Visible = false;
            txtFolderPath.Text = data.SourceName;
            ddltype.SelectedValue = data.AutoAliasType.ToString();
            ddlNameSrc.SelectedValue = data.PageNameType.ToString();
            ddlExt.SelectedValue = data.FileExtension;
            activeChkBox.Checked = data.IsEnabled;
            txtReplaceChar.Text = data.ReplacementCharacter;
            txtExample.Value = data.Example;
            txtExample.Attributes.Add("readonly", "readonly");
            txtOriginal.Text = "/CMS400Demo/default.aspx?id=354";
            if (data.SourceParmName != string.Empty)
            {
                txtQueryStringParam.Text = data.SourceParmName.ToString();
                txtOriginal.Text = txtOriginal.Text + "&" + data.SourceParmName + "=123";
            }
            txtFolderPath.Text = data.FormattedSelectedPath;
            excludedPath = data.ExcludedPath;
            sourceID = data.SourceId;
            sourcePath = data.SelectedPath;
        }
    }

    private UrlAliasAutoData InitializeData()
    {
        long id = 0;
        UrlAliasAutoData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("Alias Id does not exists."));
        }
        data = _autoAliasAPI.GetItem(id);
        if (data == null)
        {
            throw (new NullReferenceException("Alias is not found"));
        }

        return data;
    }

    private void ToolBar(string mode, long id)
    {
        if (mode == "view")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view alias"));
        }
        else if (mode == "edit")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl edit alias"));
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("msg add alias"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");

		if (mode == "edit")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", (string)("urlautoaliasmaint.aspx?action=view&Langtype=" + Request.QueryString["Langtype"] + ("&id=" + id.ToString() + "&fid=" + Request.QueryString["fId"] + "")), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?mode=auto&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?mode=auto&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		
		if (mode == "view")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/contentEdit.png", "urlautoaliasmaint.aspx?action=editalias&Langtype=" + Request.QueryString["Langtype"] + "&id=" + id.ToString() + "&fid=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewAutoAlias", "") + "</td>");
        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'frm_autoalias\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("EditAutoAlias", "") + "</td>");
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'frm_autoalias\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("AddAutoAlias", "") + "</td>");
        }
       
		result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }

    public void GetSourceList(object sender, System.EventArgs e)
    {
        string type = "";
        fId = int.Parse(folderId);
        type = (string)ddltype.SelectedValue;
        langId = int.Parse(langType);

        if (type == "Taxonomy")
        {
            lblTaxonomySelect.Visible = true;
            lblFolderSelect.Visible = false;
            txtFolderPath.Visible = false;
            ddlSource.Visible = false;
            txtFolderPath.Visible = true;
        }
        else
        {
            lblFolderSelect.Visible = true;
            ddlSource.Visible = false;
            txtFolderPath.Visible = true;
            lblTaxonomySelect.Visible = false;
        }
        GetLinkExample(true);
    }

    public void ShowLinkExample(object sender, System.EventArgs e)
    {
        GetLinkExample(false);
    }

    private void GetLinkExample(bool sourceChange)
    {
        string source = string.Empty;
        string ext = string.Empty;
        string aliasNameType = string.Empty;

        GetSourceValues();
        if (sourceChange)
        {
            sourceID = 0;
            sourcePath = string.Empty;
            txtFolderPath.Text = string.Empty;
            pathList.Items.Clear();
        }
        if (txtFolderPath.Text != string.Empty)
        {
            if (txtFolderPath.Text.Substring(1) + "/" == Request.Form["pathList"])
            {
                source = string.Empty;
            }
            else
            {
                source = (string)(txtFolderPath.Text.Replace(Request.Form["pathList"], " "));
            }
        }
        else
        {
            source = (string)txtFolderPath.Text;
        }

        source = source.Replace("\\", "/");
        ext = (string)ddlExt.SelectedValue;
        aliasNameType = (string)ddlNameSrc.SelectedItem.Text;
        if (ddlNameSrc.SelectedValue == "ContentTitle")
        {
            aliasNameType = "ContentTitle";
        }
        else if (ddlNameSrc.SelectedValue == "ContentId")
        {
            aliasNameType = "354";
        }
        else
        {
            aliasNameType = "354/1033";
        }
        txtExample.Value = source + "/" + aliasNameType + ext;
    }

    private void InitializeDropDown(string src)
    {
        ddltype.DataSource = _autoType;
        ddltype.DataBind();
        long.TryParse(folderId, out fId);
        if (!String.IsNullOrEmpty(Request.Form["frm_folder_id"]))
        {
            sourceID = Convert.ToInt64(Request.Form["frm_folder_id"]);
        }
        sourcePath = Request.Form["frm_folder_path"];
        txtFolderPath.Text = sourcePath;
        if (src == "Taxonomy")
        {
            lblFolderSelect.Visible = false;
            _autoTaxSource = _autoAliasAPI.GetTaxonomySource(langId);
            ddlSource.DataSource = _autoTaxSource;
            ddlSource.DataTextField = "TaxonomyName";
            ddlSource.DataValueField = "TaxonomyId";
            ddlSource.DataBind();
        }
        else
        {
            lblFolderSelect.Visible = true;
            ddlSource.Visible = false;
            txtFolderPath.Visible = true;
            lblTaxonomySelect.Visible = false;
            _autoFolderSource = _autoAliasAPI.GetFolderSource(fId);
            ddlSource.DataSource = _autoFolderSource;
            ddlSource.DataTextField = "Name";
            ddlSource.DataValueField = "Id";
            ddlSource.DataBind();
        }

        _aliasNameType = _autoAliasAPI.GetAutoAliasNameTypes();
        ddlNameSrc.DataSource = _aliasNameType;
        ddlNameSrc.DataBind();

        _aliasExt = _manualAliasAPI.GetFileExtensions();
        ddlExt.DataSource = _aliasExt;
        ddlExt.DataBind();
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

    private void SetServerJSVariables()
    {
        ltr_noSrcSel.Text = msgHelper.GetMessage("alert msg no source selected");
        ltr_noExtSel.Text = msgHelper.GetMessage("alert msg no extension selected");
        ltr_msgFollErr.Text = msgHelper.GetMessage("alert msg foll err");
        ltr_rootNotAliased.Text = msgHelper.GetMessage("alert msg root cannot be aliased");
    }

    protected void txtQueryStringParam_TextChanged(object sender, System.EventArgs e)
    {
        GetSourceValues();
        excludedPath = Request.Form["pathList"];
        txtOriginal.Text = string.Empty;
        txtOriginal.Text = "/CMS400Demo/default.aspx?id=354";
        if (!String.IsNullOrEmpty(txtQueryStringParam.Text))
        {
            txtOriginal.Text = txtOriginal.Text + "&" + txtQueryStringParam.Text + "=123";
        }
    }

    protected void GetSourceValues() // Sub to reterive source id and path values after internal postbacks.
    {
        if (!String.IsNullOrEmpty(Request.Form["frm_folder_id"]))
        {
            sourceID = Convert.ToInt64(Request.Form["frm_folder_id"]);
        }
        sourcePath = Request.Form["frm_folder_path"];
        if (!String.IsNullOrEmpty(sourcePath))
        {
            txtFolderPath.Text = sourcePath;
        }
    }
}
