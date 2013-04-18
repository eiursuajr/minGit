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

public partial class Workarea_urlAliasSettings_ : System.Web.UI.Page
{
    private UrlAliasSettingsApi _urlAliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        msgHelper = _refContentApi.EkMsgRef;
        RegisterResources();
        string pageAction;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;

        //Licensing For 7.6
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (_refCommonApi.RequestInformationRef.IsMembershipUser>0 || _refCommonApi.RequestInformationRef.UserId == 0)
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("msg login cms user"));
            return;
        }

        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }

        //Get lables from Resource File
        lblManualChkbox.Text = msgHelper.GetMessage("lbl tree url manual aliasing");
        lblManualChkbox.ToolTip = lblManualChkbox.Text;
        lblAutoChkbox.Text = msgHelper.GetMessage("lbl tree url automatic aliasing");
        lblAutoChkbox.ToolTip = lblAutoChkbox.Text;
        lblRegExp.Text = msgHelper.GetMessage("lbl tree url regex aliasing");
        lblRegExp.ToolTip = lblRegExp.Text;
        lblExt.Text = msgHelper.GetMessage("lbl extension");
        lblExt.ToolTip = lblExt.Text;
        lblTracing.Text = msgHelper.GetMessage("lbl tracing");
        lblTracing.ToolTip = lblTracing.Text;
        lblDefaultPage.Text = msgHelper.GetMessage("lbl default") + " " + msgHelper.GetMessage("page lbl");
        lblDefaultPage.ToolTip = lblDefaultPage.Text;
        lblOverrideTemplate.Text = msgHelper.GetMessage("lbl override template");
        lblOverrideTemplate.ToolTip = lblOverrideTemplate.Text;
        btnConfirmCancel.Text = msgHelper.GetMessage("btn cancel");
        btnConfirmCancel.ToolTip = btnConfirmCancel.Text;
        btnConfirmOk.Text = msgHelper.GetMessage("lbl ok");
        btnConfirmOk.ToolTip = btnConfirmOk.Text;
        btnConfirmCancel.Attributes.Add("  OnClick", "javascript:CloseConfirmModal();");
        btnConfirmCancel.Attributes.Add("href", "urlAliasSettings.aspx?action=edit");
        pageAction = Request.QueryString["action"];
        lblQueryStringAction.Text = msgHelper.GetMessage("lbl querstringaction");
        lblQueryStringAction.ToolTip = lblQueryStringAction.Text;
        lblOverrideLanguage.Text = msgHelper.GetMessage("lbl override lang");
        lblOverrideLanguage.ToolTip = lblOverrideLanguage.Text;
        lblNonAliasCache.Text = msgHelper.GetMessage("lbl non alias cache");
        lblNonAliasCache.ToolTip = lblNonAliasCache.Text;
        lblCommunity.Text = msgHelper.GetMessage("community text");
        lblCommunity.ToolTip = lblCommunity.Text;

        if ((string)(pageAction) == "edit")
        {
            DisplayEdit();
        }
        else if ((string)(pageAction) == "view")
        {
            DisplayView();
        }
        else if ((string)(pageAction) == "refresh")
        {
            DisplayRefresh();
        }


    }
    private void DisplayRefresh()
    {
        _urlAliasSettings.RefreshSettingsCache();
        DisplayView();

    }
    private void DisplayEdit()
    {
        AddToolBar("edit");

        if (Page.IsPostBack)
        {
            try
            {
                //TODO - Confirm
                _urlAliasSettings.EnableManualAliasing = System.Convert.ToBoolean(chkManualAlias.Checked);
                _urlAliasSettings.EnabledTracing = System.Convert.ToBoolean(chkTracing.Checked);
                _urlAliasSettings.EnableAutoAliasing = System.Convert.ToBoolean(chkAutoAlias.Checked);
                _urlAliasSettings.EnableRegExAliasing = System.Convert.ToBoolean(chkRegEx.Checked);
                _urlAliasSettings.EnableManualCaching = System.Convert.ToBoolean(chkCaching.Checked);
                _urlAliasSettings.EnableCommunityCaching = System.Convert.ToBoolean(chkCommunityCaching.Checked);
                _urlAliasSettings.EnableCommunityAliasing = System.Convert.ToBoolean(chkCommunity.Checked);

                if (Information.IsNumeric(txtManualCachesize.Text))
                {
                    _urlAliasSettings.ManualCacheSize = System.Convert.ToInt32(txtManualCachesize.Text);
                }
                _urlAliasSettings.EnableAutoCaching = System.Convert.ToBoolean(chkAutoCaching.Checked);
                if (Information.IsNumeric(txtCacheSize.Text))
                {
                    _urlAliasSettings.AutoCacheSize = System.Convert.ToInt32(txtCacheSize.Text);
                }
                _urlAliasSettings.EnableRegExCaching = System.Convert.ToBoolean(chkRegExCaching.Checked);
                if (Information.IsNumeric(txtRegExCacheSize.Text))
                {
                    _urlAliasSettings.RegExCacheSize = System.Convert.ToInt32(txtRegExCacheSize.Text);
                }
                _urlAliasSettings.EnableNonAliasCaching = System.Convert.ToBoolean(chkNonAlias.Checked);
                if (Information.IsNumeric(txtNonAliasCacheSize.Text))
                {
                    _urlAliasSettings.NonAliasCacheSize = System.Convert.ToInt32(txtNonAliasCacheSize.Text);
                }
                if (Information.IsNumeric(txtCommunityCacheSize.Text))
                {
                    _urlAliasSettings.CommunityCacheSize = System.Convert.ToInt32(txtCommunityCacheSize.Text);
                }
                _urlAliasSettings.DefaultPage = (string)txtDefaultPage.Text;
                List<String> list = txtExt.Text.Split(',').ToList().Distinct().ToList();
                string newlist = string.Empty;
                foreach (var i in list)
                {
                    if (!i.ToString().StartsWith(".") && !i.ToString().StartsWith("/"))
                        throw new Exception("File extension:" + i.ToString() + " is invalid, file extension must start with '.' or '/'");
                    newlist += i.ToString() + ",";
                }
                newlist = newlist.Remove(newlist.Length - 1, 1);
                _urlAliasSettings.PageExtensions = newlist;
                _urlAliasSettings.OverrideTemplate = System.Convert.ToBoolean(chkOverrideTemplate.Checked);
                if (Information.IsNumeric(txtManualCacheDuration.Text))
                {
                    _urlAliasSettings.ManualCacheDuration = System.Convert.ToInt32(txtManualCacheDuration.Text);
                }
                if (Information.IsNumeric(txtAutoCacheDuration.Text))
                {
                    _urlAliasSettings.AutoCacheDuration = System.Convert.ToInt32(txtAutoCacheDuration.Text);
                }
                if (Information.IsNumeric(txtRegExCacheDuration.Text))
                {
                    _urlAliasSettings.RegExCacheDuration = System.Convert.ToInt32(txtRegExCacheDuration.Text);
                }
                if (Information.IsNumeric(txtNonAliasCacheDuration.Text))
                {
                    _urlAliasSettings.NonAliasCacheDuration = System.Convert.ToInt32(txtNonAliasCacheDuration.Text);
                }
                if (Information.IsNumeric(txtCommunityCacheDuration.Text))
                {
                    _urlAliasSettings.CommunityCacheDuration = System.Convert.ToInt32(txtCommunityCacheDuration.Text);
                }
                _urlAliasSettings.QueryStringAction = (EkEnumeration.QueryStringActionType )ddlQueryStringAction.SelectedIndex;// Enum.Parse (typeof(EkEnumeration.QueryStringActionType ),ddlQueryStringAction.SelectedIndex.ToString (),true ;
                _urlAliasSettings.DisableLanguageAwareness = System.Convert.ToBoolean(chkOverrideLanguage.Checked);

            }
            catch (Exception ex)
            {
                Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.ContentLanguage), false);
                return;
            }

            Response.Redirect("urlAliasSettings.aspx?action=view");


        }
        else
        {
            chkManualAlias.Visible = true;
            chkAutoAlias.Visible = true;
            chkRegEx.Visible = true;
            chkTracing.Visible = true;
            chkCaching.Visible = true;
            chkOverrideTemplate.Visible = true;
            chkOverrideLanguage.Visible = true;
            chkCommunity.Visible = true;
            chkCommunityCaching.Visible = true;

            this.ShowTracingRow();

            lblTracingOnOff.Visible = false;
            lblAutoAliasOnOff.Visible = false;
            lblRegExOnOff.Visible = false;
            lblManualAliasOnOff.Visible = false;
            lblCachingonoff.Visible = false;
            lblNonAliasonoff.Visible = false;
            lblRegExCachingonoff.Visible = false;
            lblOverrideTemplateOnOff.Visible = false;
            lblOverrideLanguageOnOff.Visible = false;
            lblCommunityOnOff.Visible = false;
            lblCommunityCachingonoff.Visible = false;

            chkManualAlias.Checked = _urlAliasSettings.IsManualAliasEnabled;
            chkAutoAlias.Checked = _urlAliasSettings.IsAutoAliasEnabled;
            chkRegEx.Checked = _urlAliasSettings.IsRegExAliasEnabled;
            chkTracing.Checked = _urlAliasSettings.EnabledTracing;
            chkCaching.Checked = _urlAliasSettings.EnableManualCaching;
            chkAutoCaching.Checked = _urlAliasSettings.EnableAutoCaching;
            chkRegExCaching.Checked = _urlAliasSettings.EnableRegExCaching;
            chkNonAlias.Checked = _urlAliasSettings.EnableNonAliasCaching;
            chkCommunity.Checked = _urlAliasSettings.IsCommunityAliasingEnabled;
            chkCommunityCaching.Checked = _urlAliasSettings.EnableCommunityCaching;

            txtManualCachesize.Enabled = true;
            txtManualCachesize.Text = _urlAliasSettings.ManualCacheSize.ToString ();
            txtCacheSize.Enabled = true;
            txtCacheSize.Text = _urlAliasSettings.AutoCacheSize.ToString();
            txtRegExCacheSize.Enabled = true;
            txtRegExCacheSize.Text = _urlAliasSettings.RegExCacheSize.ToString();
            txtDefaultPage.Enabled = true;
            txtDefaultPage.Text = _urlAliasSettings.DefaultPage;
            txtExt.Text = _urlAliasSettings.PageExtensions;
            chkOverrideTemplate.Checked = _urlAliasSettings.OverrideTemplate;
            txtManualCacheDuration.Text = _urlAliasSettings.ManualCacheDuration.ToString();
            txtAutoCacheDuration.Text = _urlAliasSettings.AutoCacheDuration.ToString();
            txtRegExCacheDuration.Text = _urlAliasSettings.RegExCacheDuration.ToString();
            chkOverrideLanguage.Checked = _urlAliasSettings.DisableLanguageAwareness;
            txtNonAliasCacheSize.Text = _urlAliasSettings.NonAliasCacheSize.ToString();
            txtNonAliasCacheDuration.Text = _urlAliasSettings.NonAliasCacheDuration.ToString();
            txtCommunityCacheSize.Text = _urlAliasSettings.CommunityCacheSize.ToString();
            txtCommunityCacheDuration.Text = _urlAliasSettings.CommunityCacheDuration.ToString();
            ddlQueryStringAction.SelectedIndex = (int)_urlAliasSettings.QueryStringAction;

        }

    }
    private void DisplayView()
    {
        AddToolBar("view");

        chkManualAlias.Visible = false;
        chkAutoAlias.Visible = false;
        chkRegEx.Visible = false;
        chkTracing.Visible = false;
        chkCaching.Visible = false;
        chkAutoCaching.Visible = false;
        chkRegExCaching.Visible = false;
        chkOverrideTemplate.Visible = false;
        chkOverrideLanguage.Visible = false;
        chkNonAlias.Visible = false;
        chkCommunity.Visible = false;
        chkCommunityCaching.Visible = false;

        this.ShowTracingRow();

        lblTracingOnOff.Visible = true;
        lblAutoAliasOnOff.Visible = true;
        lblRegExOnOff.Visible = true;
        lblManualAliasOnOff.Visible = true;
        lblCachingonoff.Visible = true;
        lblAutoCachingonoff.Visible = true;
        lblRegExCachingonoff.Visible = true;
        lblOverrideTemplateOnOff.Visible = true;
        lblOverrideLanguageOnOff.Visible = true;
        lblNonAliasonoff.Visible = true;

        if (_urlAliasSettings.IsManualAliasEnabled)
        {
            lblManualAliasOnOff.Text = "On";
        }
        else
        {
            lblManualAliasOnOff.Text = "Off";
        }
        if (_urlAliasSettings.IsAutoAliasEnabled)
        {
            lblAutoAliasOnOff.Text = "On";
        }
        else
        {
            lblAutoAliasOnOff.Text = "Off";
        }
        if (_urlAliasSettings.IsRegExAliasEnabled)
        {
            lblRegExOnOff.Text = "On";
        }
        else
        {
            lblRegExOnOff.Text = "Off";
        }
        if (_urlAliasSettings.EnabledTracing)
        {
            lblTracingOnOff.Text = "On";
        }
        else
        {
            lblTracingOnOff.Text = "Off";
        }
        if (_urlAliasSettings.EnableManualCaching)
        {
            lblCachingonoff.Text = "On";
        }
        else
        {
            lblCachingonoff.Text = "Off";
        }
        if (_urlAliasSettings.EnableAutoCaching)
        {
            lblAutoCachingonoff.Text = "On";
        }
        else
        {
            lblAutoCachingonoff.Text = "Off";
        }
        if (_urlAliasSettings.EnableRegExCaching)
        {
            lblRegExCachingonoff.Text = "On";
        }
        else
        {
            lblRegExCachingonoff.Text = "Off";
        }
        if (_urlAliasSettings.OverrideTemplate)
        {
            lblOverrideTemplateOnOff.Text = "On";
        }
        else
        {
            lblOverrideTemplateOnOff.Text = "Off";
        }
        if (_urlAliasSettings.DisableLanguageAwareness)
        {
            lblOverrideLanguageOnOff.Text = "On";
        }
        else
        {
            lblOverrideLanguageOnOff.Text = "Off";
        }
        if (_urlAliasSettings.EnableNonAliasCaching)
        {
            lblNonAliasonoff.Text = "On";
        }
        else
        {
            lblNonAliasonoff.Text = "Off";
        }
        if (_urlAliasSettings.IsCommunityAliasingEnabled)
        {
            lblCommunityOnOff.Text = "On";
        }
        else
        {
            lblCommunityOnOff.Text = "Off";
        }
        if (_urlAliasSettings.EnableCommunityCaching)
        {
            lblCommunityCachingonoff.Text = "On";
        }
        else
        {
            lblCommunityCachingonoff.Text = "Off";
        }


        txtManualCachesize.Enabled = false;
        txtManualCachesize.Text = _urlAliasSettings.ManualCacheSize.ToString ();
        txtCacheSize.Enabled = false;
        txtCacheSize.Text = _urlAliasSettings.AutoCacheSize.ToString();
        txtRegExCacheSize.Enabled = false;
        txtRegExCacheSize.Text = _urlAliasSettings.RegExCacheSize.ToString();
        txtNonAliasCacheSize.Enabled = false;
        txtNonAliasCacheSize.Text = _urlAliasSettings.NonAliasCacheSize.ToString();
        txtCommunityCacheSize.Enabled = false;
        txtCommunityCacheSize.Text = _urlAliasSettings.CommunityCacheSize.ToString();

        txtDefaultPage.Enabled = false;
        txtDefaultPage.Text = _urlAliasSettings.DefaultPage;

        txtExt.Enabled = false;
        txtExt.Text = _urlAliasSettings.PageExtensions;

        txtManualCacheDuration.Enabled = false;
        txtManualCacheDuration.Text = _urlAliasSettings.ManualCacheDuration.ToString();

        txtAutoCacheDuration.Enabled = false;
        txtAutoCacheDuration.Text = _urlAliasSettings.AutoCacheDuration.ToString();

        txtRegExCacheDuration.Enabled = false;
        txtRegExCacheDuration.Text = _urlAliasSettings.RegExCacheDuration.ToString();

        txtNonAliasCacheDuration.Enabled = false;
        txtNonAliasCacheDuration.Text = _urlAliasSettings.NonAliasCacheDuration.ToString();

        ddlQueryStringAction.Enabled = false;
        ddlQueryStringAction.SelectedIndex =(int) _urlAliasSettings.QueryStringAction;

        txtCommunityCacheDuration.Enabled = false;
        txtCommunityCacheDuration.Text = _urlAliasSettings.CommunityCacheDuration.ToString();

    }


    private void AddToolBar(string mode)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("urlalias page html title"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "urlAliasSettings.aspx?action=view", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			
			if (_urlAliasSettings.IsMultiSite())
            {
                result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "urlAliasSettings.aspx?action=view", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn save"), "class=\"launchConfirmModal\" onclick=\"return false;\"", "launchConfirmModal", true));
            }
            else
            {
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "urlAliasSettings.aspx?action=view", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn save"), "onclick=\"javascript: return SubmitForm(\'form1\');\"", StyleHelper.SaveButtonCssClass, true));
            }
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + _refStyle.GetHelpButton("EditAliasSettings", "") + "</td>");
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "urlAliasSettings.aspx?action=edit", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/driveDelete.png", "urlAliasSettings.aspx?action=refresh", msgHelper.GetMessage("alt clear alias cache"), msgHelper.GetMessage("btn clear cache"), "", StyleHelper.DeleteButtonCssClass));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewAliasSettings", "") + "</td>");
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
    private void ShowTracingRow()
    {
        if (!string.IsNullOrEmpty(Request.QueryString["ShowTracing"]))
        {
            this.TracingRow.Visible = true;
        }
        else
        {
            this.TracingRow.Visible = false;
        }
    }
}