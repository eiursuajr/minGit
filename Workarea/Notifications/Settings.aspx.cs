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
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;
using Ektron.Cms.Core;

public partial class Workarea_Notifications_Settings : System.Web.UI.Page
{
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected Ektron.Cms.Framework.Activity.Activity _activityApi = new Ektron.Cms.Framework.Activity.Activity();
    protected long _siteId;
    protected Ektron.Cms.Framework.Notifications.Notification _notificationApi = new Ektron.Cms.Framework.Notifications.Notification();
    string pageMode = "";

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronJQueryUiDefaultCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIWidgetJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUIButtonJS);

        msgHelper = _refContentApi.EkMsgRef;
        
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        //Licensing Check
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }

        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }

        lblPublishNotification.Text = msgHelper.GetMessage("lbl publish notifications");
        lblEnableEmailReply.Text = msgHelper.GetMessage("lbl enable email replies");
        Label1.Text = msgHelper.GetMessage("lbl Email Settings");
        if (!(Request.QueryString["siteId"] == null))
        {
            long.TryParse(Request.QueryString["siteId"], out _siteId);
        }

        if (IsPostBack && !(Request.Form["siteSearchItem"] == null))
        {
            long.TryParse((string)(Request.Form["siteSearchItem"]), out _siteId);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["mode"]))
        {
            pageMode = Request.QueryString["mode"];
        }
        if ((string)(pageMode) == "view")
        {
            DisplayViewNotificationSettings();
        }
        else if ((string)(pageMode) == "edit")
        {
            DisplayEditNotificationSettings();
        }
    }
    private void DisplayViewNotificationSettings()
    {
        AddToolBar("view");
        BindControls();
    }
    private void DisplayEditNotificationSettings()
    {
        AddToolBar("edit");
        if (Page.IsPostBack)
        {
            SaveSettings();
            Response.Redirect("settings.aspx?mode=view&siteId=" + _siteId.ToString());
        }
        else
        {
            ShowEditControls();
            BindControls();
        }
    }

    private void BindControls()
    {
        chkEnablePublishNotifications.Checked = _activityApi.IsSiteActivityPublishingEnabled(_siteId);
        chkEnableEmailReply.Checked = _activityApi.IsSiteActivityEmailReplyEnabled(_siteId);
        chkPrependReplyMessage.Checked = _notificationApi.IsNotificationReplyMessagePrepended;

        if (_activityApi.IsActivityEmailReplyEnabled)
        {
            ucEktronReplyDetails.Attributes.Remove("style");
        }
        MailServerData mailServer = GetNotificationEmailServer();

        lblEmailReplyAccountValue.Text = mailServer.POPUsername;
        txtEmailReplyAccount.Text = mailServer.POPUsername;

        if (!string.IsNullOrEmpty(mailServer.POPPassword))
        {
            lblEmailReplyPasswordValue.Text = "*****";
            txtEmailReplyPassword.Attributes.Add("value", "*****");
        }

        lblEmailReplyServerValue.Text = mailServer.POPServer;
        txtEmailReplyServer.Text = mailServer.POPServer;

        lblEmailReplyServerPortValue.Text = mailServer.POPPort.ToString();
        txtEmailReplyServerPort.Text = mailServer.POPPort.ToString();

        chkUseSsl.Checked = mailServer.POPSSL;
    }


    private void ShowEditControls()
    {

        lblEmailReplyAccountValue.Visible = false;
        lblEmailReplyPasswordValue.Visible = false;
        lblEmailReplyServerValue.Visible = false;
        lblEmailReplyServerPortValue.Visible = false;

        chkUseSsl.Enabled = true;
        chkEnablePublishNotifications.Enabled = true;
        chkEnableEmailReply.Enabled = true;
        chkPrependReplyMessage.Enabled = true;

        txtEmailReplyAccount.Visible = true;
        txtEmailReplyPassword.Visible = true;
        txtEmailReplyServer.Visible = true;
        txtEmailReplyServerPort.Visible = true;
        ucTestRow.Visible = true;

    }
    private void SaveSettings()
    {

        if (chkEnablePublishNotifications.Checked)
        {
            _activityApi.EnableActivityPublishing(_siteId);
        }
        else
        {
            _activityApi.DisableActivityPublishing(_siteId);
        }
        
        if (chkEnableEmailReply.Checked)
        {
            _activityApi.EnableActivityEmailReply(_siteId);
        }
        else
        {
            _activityApi.DisableActivityEmailReply(_siteId);
        }

        _notificationApi.IsNotificationReplyMessagePrepended = chkPrependReplyMessage.Checked; 

        IMailServer emailServerApi = ObjectFactory.GetMailServer();

        MailServerData mailServerData = GetNotificationEmailServer();
        mailServerData.Name = "Community Email Reply";
        mailServerData.POPUsername = Microsoft.Security.Application.AntiXss.HtmlEncode(txtEmailReplyAccount.Text);
        mailServerData.POPServer = Microsoft.Security.Application.AntiXss.HtmlEncode(txtEmailReplyServer.Text);
        mailServerData.POPPort = System.Convert.ToInt32(txtEmailReplyServerPort.Text);
        mailServerData.POPSSL = System.Convert.ToBoolean(chkUseSsl.Checked);
        mailServerData.Type = MailServerType.CommunityEmailNotification;


        if (txtEmailReplyPassword.Text != "*****")
        {
            mailServerData.POPPassword = (string)txtEmailReplyPassword.Text;
        }

        if (mailServerData.Id > 0)
        {
            emailServerApi.Update(mailServerData);
        }
        else
        {
            emailServerApi.Add(mailServerData);
        }
    }

    private MailServerData GetNotificationEmailServer()
    {
        IMailServer emailServerApi = ObjectFactory.GetMailServer();
        MailServerData mailServerData = new MailServerData();

        Criteria<MailServerProperty> criteria = new Criteria<MailServerProperty>();
        criteria.AddFilter(MailServerProperty.Type, CriteriaFilterOperator.EqualTo, MailServerType.CommunityEmailNotification);

        List<MailServerData> servers = emailServerApi.GetList(criteria);

        if (servers.Count > 0)
        {
            mailServerData = servers[0];
        }
        return mailServerData;
    }

    private void AddToolBar(string mode)
    {
        msgHelper = _refCommonApi.EkMsgRef;

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");

        if (mode == "edit")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("alt edit notification settings"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "Settings.aspx?mode=view", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "Settings.aspx?mode=view", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "onclick=\"javascript: return SubmitForm(\'form1\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(AddSiteView());
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>" + _refStyle.GetHelpButton("EditNotificationSettings", "") + "</td>");
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("alt view notification settings"));
            // result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "Settings.aspx?mode=edit", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "#", msgHelper.GetMessage("btn edit"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            result.Append(AddSiteView());
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewNotificationSettings", "") + "</td>");
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
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
    }

    private StringBuilder AddSiteView()
    {
        System.Text.StringBuilder siteResult = new System.Text.StringBuilder();
        siteResult.Append(StyleHelper.ActionBarDivider);
        siteResult.Append("<td class=\"label\">");
        siteResult.Append(_refContentApi.EkMsgRef.GetMessage("lbl site colon"));
        siteResult.Append("</td>");
        siteResult.Append("<td>");
        // siteResult.Append("<select name=\"siteSearchItem\" id=\"siteSearchItem\" ONCHANGE=\"SubmitForm(\'form1\',\'\');\"/>&nbsp;");

        string sitesselect = string.Format(
                "<select name=\"siteSearchItem\" id=\"siteSearchItem\" ONCHANGE=\"ChangeSite();\" {0}/>&nbsp;",
                (pageMode=="edit"?" disabled=\"disabled\" ": string.Empty)
        );
        
        siteResult.Append(sitesselect);

        System.Collections.Generic.Dictionary<long, string> siteDictionary;
        System.Collections.Generic.KeyValuePair<long, string> siteList;
        Ektron.Cms.UrlAliasing.UrlAliasManualApi manualAliasList = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();

        siteDictionary = manualAliasList.GetSiteList();

        foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
        {
            siteList = tempLoopVar_siteList;
            if (siteList.Key == _siteId)
            {
                siteResult.Append("<option selected=\"selected\" value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                //break;
            }
            else
            {
                siteResult.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
            }
        }
        //foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
        //{
        //    siteList = tempLoopVar_siteList;
        //    if (siteList.Key != _siteId)
        //    {
        //        siteResult.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
        //    }
        //}
        siteResult.Append("</select>");
        siteResult.Append("</td>");
        return siteResult;
    }
}