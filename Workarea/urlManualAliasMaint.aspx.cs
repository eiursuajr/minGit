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
public partial class Workarea_urlManualAliasMaint : System.Web.UI.Page
{
    public Workarea_urlManualAliasMaint()
    {
        _AppPath = _refContentApi.ApplicationPath;

    }
    private UrlAliasManualApi _manualAliasAPI;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected string CTitle = string.Empty;
    protected string apppath = string.Empty;
    protected string contID = string.Empty;
    protected string contLangID = string.Empty;
    protected string lblContentBlk = string.Empty;
    protected string siteid = string.Empty;
    private string workareaDir = string.Empty;
    protected string _AppPath;

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

        _manualAliasAPI = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();

        //Labels got from resource file
        lblaliasname.Text = msgHelper.GetMessage("lbl alias");
        lblContentBlk = msgHelper.GetMessage("content block label");
        lblTitle.Text = msgHelper.GetMessage("generic title");
        lblLink.Text = msgHelper.GetMessage("lbl quick link") + ":";
        lblTemplates.Text = msgHelper.GetMessage("lbl quick link") + ":";
        lblPrimary.Text = msgHelper.GetMessage("lbl primary");
        lblActive.Text = msgHelper.GetMessage("active label");
        lblAddVar.Text = msgHelper.GetMessage("lbl alias additional variables");
        lblQueryStringAction.Text = msgHelper.GetMessage("lbl querstringaction");
	lblaliasname.ToolTip = lblaliasname.Text;
	lblTitle.ToolTip = lblTitle.Text;
	lblLink.ToolTip = lblLink.Text;
	lblTemplates.ToolTip = lblTemplates.Text;
	lblPrimary.ToolTip = lblPrimary.Text;
	lblActive.ToolTip = lblActive.Text;
	lblAddVar.ToolTip = lblAddVar.Text;
	lblQueryStringAction.ToolTip = lblQueryStringAction.Text;
        pageAction = Request.QueryString["action"];
        siteid = Request.QueryString["fId"];
        workareaDir = _refContentApi.RequestInformationRef.WorkAreaDir;
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
        System.Collections.Generic.List<string> ext_alias = null;
        ext_alias = _manualAliasAPI.GetFileExtensions();
        listAliasExtension.DataSource = ext_alias;
        listAliasExtension.DataBind();
    }

    private void DisplayAdd()
    {
        Toolbar("add", 0);
        UrlAliasManualData add_alias = new UrlAliasManualData(0, 0, "", "");
        int languageId;

        int.TryParse(Request.QueryString["Langtype"], out languageId);

        if (Page.IsPostBack)
        {
            CTitle = Request.Form["frm_content_title"];
            apppath = _refContentApi.SitePath;
            contID = Request.Form["frm_content_id"];
            contLangID = Request.Form["frm_content_langid"];
            //strContentStatus = Request.Form("frm_content_status")

            add_alias.AliasName = (string)txtAliasName.Text;
            add_alias.ContentId = Convert.ToInt64(Request.Form["frm_content_id"]);
            add_alias.FileExtension = (string)listAliasExtension.Text;
            add_alias.LibraryId = Convert.ToInt64(Request.Form["templateList"]);
            add_alias.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
            add_alias.IsDefault = System.Convert.ToBoolean(primaryChkBox.Checked);
            add_alias.QueryString = (string)txtAddVar.Text;
            add_alias.QueryStringAction = (EkEnumeration.QueryStringActionType)ddlQueryStringAction.SelectedIndex;
            try
            {
                add_alias = _manualAliasAPI.Add(add_alias, _refContentApi.UserId);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + contLangID), false);
                return;
            }
            Response.Redirect((string)("urlmanualaliaslistmaint.aspx?&fId=" + siteid));

        }
        tr_links.Visible = false;

        if (languageId == -1)
        {
            this.quickLinkSelect.Visible = false;
        }
    }

    private void DisplayView()
    {

        UrlAliasManualData data = InitializeData();
        Toolbar("view", data.AliasId);
        txtAliasName.Text = data.AliasName;
        txtAliasName.Enabled = false;
        txtContentTitle.Text = data.ContentTitle;
        listAliasExtension.Text = data.FileExtension;
        listAliasExtension.Enabled = false;
        if (data.Target.ToLower().IndexOf("downloadasset.aspx?") != -1)
        {
            frm_qlinkdis.Text = workareaDir + data.Target;
        }
        else
        {
            frm_qlinkdis.Text = data.Target;
        }

        primaryChkBox.Checked = data.IsDefault;
        primaryChkBox.Enabled = false;
        activeChkBox.Checked = data.IsEnabled;
        activeChkBox.Enabled = false;
        txtAddVar.Text = data.QueryString;
        quickLinkSelect.Visible = false;
        lblTemplates.Visible = false;
        lblTemplateList.Visible = false;
        txtAddVar.Enabled = false;

        ddlQueryStringAction.Enabled = false;
        ddlQueryStringAction.SelectedIndex = (int)data.QueryStringAction;
    }

    private void DisplayEdit()
    {
        int languageId;
        UrlAliasManualData data = null;
        System.Collections.Generic.List<LibraryData> quickLink = null;
        int i;
        data = InitializeData();
        Toolbar("edit", data.AliasId);

        int.TryParse(Request.QueryString["Langtype"], out languageId);

        if (Page.IsPostBack)
        {
            //data = New UrlAliasManualData(0, 0, "", "")

            data.AliasName = (string)txtAliasName.Text;
            data.FileExtension = (string)listAliasExtension.Text;

            data.Target = Request.Form["frm_qlink"];
            data.IsDefault = System.Convert.ToBoolean(primaryChkBox.Checked);
            data.IsEnabled = System.Convert.ToBoolean(activeChkBox.Checked);
            data.QueryString = (string)txtAddVar.Text;
            if (!String.IsNullOrEmpty(Request.Form["frm_content_id"]))
            {
                data.ContentId = Convert.ToInt64(Request.Form["frm_content_id"]);
            }
            if (!String.IsNullOrEmpty(Request.Form["templateList"]))
            {
                data.LibraryId = Convert.ToInt64(Request.Form["templateList"]);
            }

            data.QueryStringAction = (EkEnumeration.QueryStringActionType)Enum.Parse(typeof(EkEnumeration.QueryStringActionType), Convert.ToString(ddlQueryStringAction.SelectedIndex), true);
            try
            {
                data = _manualAliasAPI.Update(data, _refContentApi.UserId);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.ContentLanguage), false);
                return;
            }
            Response.Redirect((string)("urlmanualaliasmaint.aspx?action=view&id=" + data.AliasId.ToString() + "&LangType=" + _refContentApi.ContentLanguage));

        }
        else
        {
            //get data and fill values

            txtAliasName.Text = data.AliasName;
            txtContentTitle.Text = data.ContentTitle;
            listAliasExtension.Text = data.FileExtension;
            //frm_qlink.Text = data.Target
            primaryChkBox.Checked = data.IsDefault;
            if (primaryChkBox.Checked == true)
            {
                primaryChkBox.Enabled = false;
                activeChkBox.Enabled = false;
            }
            activeChkBox.Checked = data.IsEnabled;
            txtAddVar.Text = data.QueryString;
            quickLink = _manualAliasAPI.GetTemplateList(data.ContentId);
            for (i = 0; i <= quickLink.Count - 1; i++)
            {
                if (data.Target != quickLink[i].FileName)
                {
                    if (quickLink[i].FileName.ToLower().IndexOf("downloadasset.aspx?") != -1)
                    {
                        templateList.Items.Add(new ListItem(workareaDir + quickLink[i].FileName, quickLink[i].Id.ToString()));
                    }
                    else
                    {
                        templateList.Items.Add(new ListItem(quickLink[i].FileName, quickLink[i].Id.ToString()));
                    }
                }
                else
                {
                    if (data.Target.ToLower().IndexOf("downloadasset.aspx?") != -1)
                    {
                        templateList.Items.Add(new ListItem(workareaDir + data.Target, quickLink[i].Id.ToString()));
                    }
                    else
                    {
                        templateList.Items.Add(new ListItem(data.Target, quickLink[i].Id.ToString()));
                    }
                    templateList.Value = quickLink[i].Id.ToString();
                }
            }
        }
        frm_qlinkdis.Visible = false;
        lblLink.Visible = false;


        if (languageId == -1)
        {
            this.quickLinkSelect.Visible = false;
        }

        ddlQueryStringAction.SelectedIndex = (int)data.QueryStringAction;
    }

    private UrlAliasManualData InitializeData()
    {
        long id = 0;
        UrlAliasManualData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("Alias Id does not exists."));
        }
        data = _manualAliasAPI.GetItem(id);
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
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlmanualaliasmaint.aspx?action=view&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "&Langtype=" + Request.QueryString["Langtype"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "urlmanualaliaslistmaint.aspx?fId=" + Request.QueryString["fId"] + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "view")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/contentEdit.png", "urlmanualaliasmaint.aspx?action=editalias&id=" + id.ToString() + "&fId=" + Request.QueryString["fId"] + "&Langtype=" + Request.QueryString["Langtype"] + "", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "onclick=\"javascript:SubmitForm(\'form1\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "onclick=\"javascript:SubmitForm(\'form1\',\'VerifyAddAlias()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        if (mode == "edit")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("EditManAlias", "") + "</td>");
        }
        else if (mode == "view")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewManAlias", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("AddManAlias", "") + "</td>");
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

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
    }
    private void SetServerJSVariables()
    {
        ltr_noAliasEntered.Text = msgHelper.GetMessage("alert msg no alias entered");
        ltr_noContBlck.Text = msgHelper.GetMessage("alert msg no content block");
        ltr_follErr.Text = msgHelper.GetMessage("alert msg foll err");
    }
}