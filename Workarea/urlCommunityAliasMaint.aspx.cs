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
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.UrlAliasing;

public partial class Workarea_urlCommunityAliasMaint : System.Web.UI.Page
{
    public Workarea_urlCommunityAliasMaint()
    {
        _AppPath = _refContentApi.ApplicationPath;

    }

    private UrlAliasCommunityApi _communityAliasAPI;
    private string langType;
    private int langId = 0;
    private System.Collections.Generic.List<string> _aliasExt;
    private System.Collections.Generic.List<EkEnumeration.CommunityAliasType> _communityType;
    protected string CTitle = string.Empty;
    protected string siteID = string.Empty;
    protected string contLangID = string.Empty;
    protected string folderId;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected string _AppPath;
    protected Ektron.Cms.UrlAliasing.UrlAliasManualApi _manualAliasAPI;


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

        _communityAliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasCommunityApi();
        _manualAliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();

        pageAction = Request.QueryString["action"];
        langType = Request.QueryString["Langtype"];
        siteID = Request.QueryString["fId"];
        ddltype.Attributes.Add("onchange", "ToggleExamplePath(this);");
        lblPrimary.Text = msgHelper.GetMessage("lbl primary");
        lblActive.Text = msgHelper.GetMessage("active label");
        lblType.Text = msgHelper.GetMessage("lbl source") + " " + msgHelper.GetMessage("generic type");
        lblAliasPath.Text = msgHelper.GetMessage("lbl alias path");
        lblExt.Text = msgHelper.GetMessage("lbl extension");
        lblReplaceChar.Text = msgHelper.GetMessage("lbl rpl char");
        lblExample.Text = msgHelper.GetMessage("lbl link ex preview");
        lblQueryStringParam.Text = msgHelper.GetMessage("lbl querystringparam");
	lblPrimary.ToolTip = lblPrimary.Text;
	lblActive.ToolTip = lblActive.Text;
	lblType.ToolTip = lblType.Text;
	lblAliasPath.ToolTip = lblAliasPath.Text;
	lblExt.ToolTip = lblExt.Text;
	lblReplaceChar.ToolTip = lblReplaceChar.Text;
	lblExample.ToolTip = lblExample.Text;
	lblQueryStringParam.ToolTip = lblQueryStringParam.Text;
        primaryChkBox.Attributes.Add("onclick", "$ektron(\'#activeChkBox\')[0].checked = true;");
        _communityType = _communityAliasAPI.GetCommunityAliasTypes();
        int.TryParse(langType, out langId);
        switch (pageAction)
        {
            case "addalias":
                DisplayAdd();
                break;
            case "view":
                DisplayView();
                break;
            case "editalias":
                DisplayEdit();
                break;
        }

    }

    private void DisplayAdd()
    {
        Toolbar("add", 0);
        int languageId;
        int.TryParse(Request.QueryString["Langtype"], out languageId);

        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                UrlAliasCommunityData addData = new UrlAliasCommunityData(0, Ektron.Cms.Common.EkEnumeration.CommunityAliasType.User, "", "");

                addData.SiteId = Convert.ToInt64(siteID);
                if (ddltype.SelectedValue.ToLower() == "user")
                {
                    addData.CommunityAliasType = Ektron.Cms.Common.EkEnumeration.CommunityAliasType.User;
                }
                else
                {
                    addData.CommunityAliasType = Ektron.Cms.Common.EkEnumeration.CommunityAliasType.Group;
                }
                addData.AliasPath = (string)tbAliasPath.Text;
                addData.FileExtension = (string)ddlExt.SelectedValue;
                addData.ReplacementCharacter = (string)txtReplaceChar.Text;
                addData.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
                addData.IsDefault = System.Convert.ToBoolean(primaryChkBox.Checked);
                addData.LanguageId = languageId;
                addData.Example = (string)hdntxtExample.Value;
                addData.SourceParmName = (string)txtQueryStringParam.Text;
                try
                {
                    _communityAliasAPI.Add(addData);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + languageId), false);
                    return;
                }
                Response.Redirect((string)("urlmanualaliaslistmaint.aspx?mode=community&fId=" + siteID));
            }
        }
        else
        {
            tbAliasPath.Attributes.Add("onkeyup", "SetPreviewLinkTextBox(this);");
            tbAliasPath.Attributes.Add("onmouseup", "SetPreviewLinkTextBox(this);");
            ddlExt.Attributes.Add("onchange", "ToggleExample(this);");
            InitializeDropDown("User");
        }
        activeChkBox.Checked = true;
        GetLinkExample(false);

    }
    private void InitializeDropDown(string src)
    {

        ddltype.DataSource = _communityType;
        ddltype.DataBind();

        _aliasExt = _manualAliasAPI.GetFileExtensions();
        ddlExt.DataSource = _aliasExt;
        ddlExt.DataBind();
    }
    private void DisplayView()
    {
        UrlAliasCommunityData data = InitializeData();
        Toolbar("view", data.Id);

        ddltype.SelectedValue = data.CommunityAliasType.ToString();
        ddltype.Enabled = false;
        InitializeDropDown(data.CommunityAliasType.ToString());
        ddlExt.SelectedValue = data.FileExtension;
        ddlExt.Enabled = false;
        primaryChkBox.Checked = data.IsDefault;
        primaryChkBox.Enabled = false;
        activeChkBox.Checked = data.IsEnabled;
        activeChkBox.Enabled = false;
        txtReplaceChar.Text = data.ReplacementCharacter;
        txtReplaceChar.Enabled = false;
        txtExample.Value = data.Example;
        txtExample.Attributes.Add("disabled", "disabled");
        txtQueryStringParam.Enabled = false;
        txtQueryStringParam.Text = data.SourceParmName.ToString();
        tbAliasPath.Text = data.AliasPath;
        tbAliasPath.Enabled = false;
        txtExample.Value = data.Example;
    }
    private void DisplayEdit()
    {
        int languageId;
        UrlAliasCommunityData data = null;
        data = InitializeData();
        Toolbar("edit", data.Id);
        int.TryParse(Request.QueryString["Langtype"], out languageId);

        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                if (ddltype.SelectedValue.ToLower() == "user")
                {
                    data.CommunityAliasType = Ektron.Cms.Common.EkEnumeration.CommunityAliasType.User;
                }
                else
                {
                    data.CommunityAliasType = Ektron.Cms.Common.EkEnumeration.CommunityAliasType.Group;
                }
                data.FileExtension = (string)ddlExt.SelectedValue;
                data.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
                data.IsDefault = System.Convert.ToBoolean(primaryChkBox.Checked);
                data.ReplacementCharacter = (string)txtReplaceChar.Text;
                data.AliasPath = (string)tbAliasPath.Text;
                //data.Example = txtExample.Value
                data.SourceParmName = (string)txtQueryStringParam.Text;
                data.Example = (string)hdntxtExample.Value;

                try
                {
                    _communityAliasAPI.Update(data);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + Request.QueryString["Langtype"]), false);
                    return;
                }
                Response.Redirect((string)("urlcommunityaliasmaint.aspx?action=view&id=" + data.Id.ToString() + "&Langtype=" + Request.QueryString["Langtype"] + "&fid=" + Request.QueryString["fId"]));
            }
        }
        else
        {
            tbAliasPath.Attributes.Add("onkeyup", "SetPreviewLinkTextBox(this);");
            tbAliasPath.Attributes.Add("onmouseup", "SetPreviewLinkTextBox(this);");
            InitializeDropDown(data.CommunityAliasType.ToString());
            ddltype.SelectedValue = data.CommunityAliasType.ToString();
            ddlExt.SelectedValue = data.FileExtension;
            primaryChkBox.Checked = data.IsDefault;
            activeChkBox.Checked = data.IsEnabled;

            if (data.IsDefault)
            {
                primaryChkBox.Enabled = false;
                activeChkBox.Enabled = false;
            }

            txtReplaceChar.Text = data.ReplacementCharacter;
            txtExample.Value = data.Example;
            txtExample.Attributes.Add("readonly", "readonly");
            txtQueryStringParam.Text = data.SourceParmName.ToString();
            txtExample.Disabled = true;
            tbAliasPath.Text = data.AliasPath;
            ddltype.Enabled = false;
            ddlExt.Attributes.Add("onchange", "ToggleExample(this);");
        }
    }
    public void ShowLinkExample(object sender, System.EventArgs e)
    {
        GetLinkExample(false);
    }
    private void GetLinkExample(bool sourceChange)
    {

    }
    private UrlAliasCommunityData InitializeData()
    {
        long id = 0;
        UrlAliasCommunityData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("Alias Id does not exists."));
        }
        data = _communityAliasAPI.GetItem(id);
        if (data == null)
        {
            throw (new NullReferenceException("Alias is not found"));
        }

        return data;
    }
    private void Toolbar(string mode, long id)
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
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlcommunityaliasmaint.aspx?mode=community&action=view&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "&Langtype=" + Request.QueryString["Langtype"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?mode=community&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?mode=community&fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "view")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/contentEdit.png", "urlcommunityaliasmaint.aspx?action=editalias&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "&Langtype=" + Request.QueryString["Langtype"] + "", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "onclick=\"javascript:SubmitForm(\'frm_communityalias\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "onclick=\"javascript:SubmitForm(\'frm_communityalias\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        if (mode == "edit")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("EditCommunityAlias", "") + "</td>");
        }
        else if (mode == "view")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewCommunityAlias", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("AddCommunityAlias", "") + "</td>");
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();

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
        ltr_noExtSel.Text = msgHelper.GetMessage("alert msg no extension selected");
        ltr_msgFollErr.Text = msgHelper.GetMessage("alert msg foll err");
        ltr_noAliasSel.Text = msgHelper.GetMessage("alert msg no alias name selected");
    }
}