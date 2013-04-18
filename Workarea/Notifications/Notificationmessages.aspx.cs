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
using Ektron.Cms.Content;
using Ektron.Cms.Messaging;
using Ektron.Cms.Framework.Messaging;
using Ektron.Cms.API;

public partial class Workarea_Notificationmessages : System.Web.UI.Page
{
    #region MemberVaribles
    protected Ektron.Cms.Framework.Settings.CmsMessageTypeManager _MessageTypeApi = new Ektron.Cms.Framework.Settings.CmsMessageTypeManager();
    private string langType;
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected SiteAPI _refSiteAPI = new SiteAPI();
    protected Ektron.Cms.Framework.Settings.CmsMessageManager _NotificationMessageApi = new Ektron.Cms.Framework.Settings.CmsMessageManager();
    protected Ektron.ContentDesignerWithValidator ctlEditor;
    protected int ContentLanguage = -1;
    protected LanguageData[] colActiveLanguages;
    protected int EnableMultilingual;
    protected int index;
    protected long messageId = 0;
    protected string strtokenList = string.Empty;
    protected string pageMode = string.Empty;
    protected bool isDefaultMessage = false;
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;
    protected long _siteId;
    #endregion
    protected void Page_Init(object sender, System.EventArgs e)
    {
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("../Controls/Editor/ContentDesignerWithValidator.ascx");
        phContentDesigner.Controls.Add(ctlEditor);
        ctlEditor.ID = "txtTextAddEdit";
        ctlEditor.AllowScripts = false;
        ctlEditor.Height = new Unit(100, UnitType.Percentage);
        ctlEditor.Width = new Unit(98, UnitType.Percentage);
        ctlEditor.Stylesheet = _refSiteAPI.AppPath + "csslib/ewebeditprostyles.css";
        ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
        ctlEditor.AllowFonts = true;
        ctlEditor.ShowHtmlMode = false;
    }
    protected void Page_Load(object sender, System.EventArgs e)
    {
        int i;
        bool blnLanguageMatched = false;
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        msgHelper = _refCommonApi.EkMsgRef;
        EnableMultilingual = _refContentApi.EnableMultilingual;

        this.ParseCurrentPageNumber();

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

        if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            _refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (_refContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (!String.IsNullOrEmpty(Request.QueryString["siteId"]))
        {
            long.TryParse(Request.QueryString["siteId"], out _siteId);
        }

        if (IsPostBack && !(Request.Form["siteSearchItem"] == null))
        {
            long.TryParse((string)(Request.Form["siteSearchItem"]), out _siteId);
        }

        if (EnableMultilingual == 1)
        {
            colActiveLanguages = _refSiteAPI.GetAllActiveLanguages();
        }

        _refContentApi.ContentLanguage = ContentLanguage;

        if (EnableMultilingual == 1)
        {
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                if (ContentLanguage == colActiveLanguages[i].Id)
                {
                    blnLanguageMatched = true;
                }
            }
            if (!blnLanguageMatched)
            {
                _refContentApi.SetCookieValue("LastValidLanguageID", _refContentApi.DefaultContentLanguage.ToString());
                _refContentApi.ContentLanguage = _refContentApi.DefaultContentLanguage;
                ContentLanguage = _refContentApi.DefaultContentLanguage;
            }
        }

        //Strings from Resource Files.
        SetServerJSVariables();
        ltrDefault.Text = msgHelper.GetMessage("lbl default");
        lblId.Text = msgHelper.GetMessage("generic SubscriptionID") + ":";

        pageMode = Request.QueryString["mode"];
        langType = Request.QueryString["Langtype"];

        if ((string)(pageMode) == "addnotificationmsg")
        {
            DisplayAddNotificationMessage();
        }
        else if ((string)(pageMode) == "viewnotificationmsg")
        {
            DisplayViewNotificationMessage();
        }
        else if ((string)(pageMode) == "editnotificationmsg")
        {
            DisplayEditNotificationMessage();
        }
        else if ((string)(pageMode) == "viewnotificationmsggrid")
        {
            DisplayGrid();
        }
        else if ((string)(pageMode) == "deletenotificationmessage")
        {
            DeleteNotificationMessage();
        }
        ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Basic;
        ctlEditor.AllowFonts = true;
    }
    private void DisplayAddNotificationMessage()
    {
        CmsMessageData MessageData = new CmsMessageData();
        Toolbar("addnotificationmsg", 0);

        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                MessageData.Title = EkFunctions.HtmlEncode((string)txtTitle.Text);
                MessageData.MessageType.Id = Convert.ToInt64(ddlSubType.SelectedValue);
                MessageData.IsDefaultMessage = System.Convert.ToBoolean(chkDefault.Checked);
                MessageData.Subject = (string)txtSubject.Text;
                MessageData.HtmlBody = Util_StripScript((string)ctlEditor.Content);
                MessageData.TextBody = (string)txtPlainText.Text;
                MessageData.LanguageId = int.Parse(_refContentApi.GetCookieValue("LastValidLanguageID"));
                MessageData.SiteId = _siteId;
                try
                {
                    _NotificationMessageApi.Add(MessageData);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                    return;
                }
                Response.Redirect((string)("notificationmessages.aspx?mode=viewnotificationmsggrid&siteId=" + _siteId));
            }
        }
        else
        {
            VisiblePageControls(false);
            lblId.Visible = false;
            LoadMessageSubType();
            LoadTokenList();
            ViewAllMessageGrid.Visible = false;
        }
    }
    private void DisplayViewNotificationMessage()
    {
        CmsMessageData data = null;
        data = GetNotificationMessageData();
        isDefaultMessage = data.IsDefaultMessage;
        Toolbar("viewnotificationmsg", data.Id);

        //Disabling all the fields in view mode
        txtTitle.Enabled = false;
        ddlType.Enabled = false;
        ddlSubType.Enabled = false;
        chkDefault.Enabled = false;
        txtSubject.Enabled = false;
        txtPlainText.Enabled = false;

        txtTitle.Text = EkFunctions.HtmlDecode(data.Title);
        ltrViewID.Text = data.Id.ToString();
        ddlType.SelectedValue = data.MessageType.Scope;

        LoadMessageSubType();
        VisiblePageControls(false);
        ddlSubType.SelectedValue = data.MessageType.Id.ToString();
        chkDefault.Checked = data.IsDefaultMessage;
        txtSubject.Text = data.Subject;
        ctlEditor.Visible = false;
        viewContentHTML.Text = data.HtmlBody;
        ctlEditor.Visible = false;
        txtPlainText.Text = data.TextBody;
        LoadTokenList();
        ViewAllMessageGrid.Visible = false;
    }
    private void DisplayEditNotificationMessage()
    {
        CmsMessageData data = null;
        data = GetNotificationMessageData();
        Toolbar("editnotificationmsg", data.Id);
        if (Page.IsPostBack)
        {
            if (Request.Form[isCPostData.UniqueID] == "")
            {
                data.Title = EkFunctions.HtmlEncode((string)txtTitle.Text);
                data.MessageType.Id = Convert.ToInt64(ddlSubType.SelectedValue);
                data.IsDefaultMessage = System.Convert.ToBoolean(chkDefault.Checked);
                data.Subject = (string)txtSubject.Text;
                data.HtmlBody = Util_StripScript((string)ctlEditor.Content);
                data.TextBody = (string)txtPlainText.Text;
                data.LanguageId = int.Parse(_refContentApi.GetCookieValue("LastValidLanguageID"));
                data.SiteId = _siteId;
                try
                {
                    _NotificationMessageApi.Update(data);
                }
                catch (Exception ex)
                {
                    Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                    return;
                }
                Response.Redirect((string)("notificationmessages.aspx?mode=viewnotificationmsg&id=" + data.Id.ToString() + "&siteId=" + _siteId.ToString()));
            }
        }
        else
        {
            txtTitle.Text = EkFunctions.HtmlDecode(data.Title);
            ltrViewID.Text = data.Id.ToString();
            ddlType.SelectedValue = data.MessageType.Scope;
            LoadMessageSubType();
            VisiblePageControls(false);
            ddlSubType.SelectedValue = data.MessageType.Id.ToString();
            chkDefault.Checked = data.IsDefaultMessage;
            if (data.IsDefaultMessage)
            {
                chkDefault.Enabled = false;
            }
            txtSubject.Text = data.Subject;
            ctlEditor.Content = data.HtmlBody;
            txtPlainText.Text = data.TextBody;
            viewContentHTML.Visible = false;
            ViewAllMessageGrid.Visible = false;
        }
        LoadTokenList();
    }
    public void LoadSubTypeList(object sender, System.EventArgs e)
    {
        LoadMessageSubType();
        LoadTokenList();
    }
    public void LoadMessageSubType()
    {
        System.Collections.Generic.List<CmsMessageTypeData> messageTypeList = new System.Collections.Generic.List<CmsMessageTypeData>();
        System.Collections.Generic.List<string> strMessageTypeList = new System.Collections.Generic.List<string>();
        CmsMessageTypeCriteria criteria = new CmsMessageTypeCriteria();

        criteria.AddFilter(CmsMessageTypeProperty.Scope, CriteriaFilterOperator.Contains, ddlType.SelectedValue);
        messageTypeList = _MessageTypeApi.GetList(criteria);
        ddlSubType.Items.Clear();
        for (index = 0; index <= messageTypeList.Count - 1; index++)
        {
            ddlSubType.Items.Add(new ListItem(messageTypeList[index].Name.ToString(), messageTypeList[index].Id.ToString()));
        }
    }
    public void LoadAllTokenList(object sender, System.EventArgs e)
    {
        LoadTokenList();
    }
    public void LoadTokenList()
    {
        System.Collections.Generic.List<string> tokenList = new System.Collections.Generic.List<string>();
        tokenList = _MessageTypeApi.GetTokenList(Convert.ToInt64(ddlSubType.SelectedValue));
        strtokenList = string.Empty;
        for (index = 0; index <= tokenList.Count - 1; index++)
        {
            strtokenList += (string)(tokenList[index].ToString());
            if (index != tokenList.Count - 1)
            {
                strtokenList += ", ";
            }
        }
    }
    private void Toolbar(string mode, long id)
    {
        if (mode == "viewnotificationmsg")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view notify msg"));
        }
        else if (mode == "editnotificationmsg")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl edit notify msg"));
        }
        else if (mode == "viewnotificationmsggrid")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view notify grid"));
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl add notify msg"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");

		if (mode == "editnotificationmsg")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "notificationmessages.aspx?mode=viewnotificationmsg&id=" + id.ToString() + "&siteId=" + _siteId.ToString() + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "viewnotificationmsg")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", (string)("notificationmessages.aspx?mode=viewnotificationmsggrid" + "&siteId=" + _siteId.ToString()), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "addnotificationmsg")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", (string)("notificationmessages.aspx?mode=viewnotificationmsggrid" + "&siteId=" + _siteId.ToString()), msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "viewnotificationmsg")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/contentEdit.png", "notificationmessages.aspx?mode=editnotificationmsg&id=" + id.ToString() + "&Langtype=" + Request.QueryString["Langtype"] + "&siteId=" + _siteId.ToString() + "", msgHelper.GetMessage("alt edit notify message"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            if (!isDefaultMessage)
            {
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/delete.png", "notificationmessages.aspx?mode=deletenotificationmessage&id=" + id.ToString() + "", msgHelper.GetMessage("alt delete notify message"), msgHelper.GetMessage("btn delete"), "Onclick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
            }
        }
        else if (mode == "viewnotificationmsggrid")
        {
            result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/add.png", "notificationmessages.aspx?mode=addnotificationmsg&Langtype=" + Request.QueryString["Langtype"] + "&siteId=" + _siteId.ToString() + "", msgHelper.GetMessage("alt add notify message"), msgHelper.GetMessage("btn Add Message"), "", StyleHelper.AddButtonCssClass, true));

        }
        else if (mode == "editnotificationmsg")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'form1\',\'VerifyAddNotificationMsg()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'form1\',\'VerifyAddNotificationMsg()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		
        if (mode == "editnotificationmsg")
        {
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("EditNotificationMsg", "") + "</td>");
        }
        else if (mode == "viewnotificationmsg")
        {
            result.Append(AddLanguageView());
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + _refStyle.GetHelpButton("ViewNotificationMsg", "") + "</td>");
        }
        else if (mode == "addnotificationmsg")
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + _refStyle.GetHelpButton("AddNotificationMsg", "") + "</td>");
        }
        else
        {
            result.Append(AddLanguageView());
            result.Append(AddSiteView());
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + _refStyle.GetHelpButton("ViewAllNotificationMsg", "") + "</td>");
        }


        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();

    }


    private void DisplayGrid()
    {
        System.Collections.Generic.List<CmsMessageData> NotificationMessageList;
        PagingInfo page;
        CmsMessageCriteria criteria = new CmsMessageCriteria();
        LocalizationAPI objLocalizationApi = new LocalizationAPI();
        LanguageData[] languageData = _refSiteAPI.GetAllActiveLanguages();
        string strSelectedLanguageName = "";
        string strName;

        Toolbar("viewnotificationmsggrid", 0);
        AddNewMessage.Visible = false;

        page = new PagingInfo();
        page.CurrentPage = _currentPageNumber;
        criteria.PagingInfo = page;
        criteria.PagingInfo.RecordsPerPage = _refContentApi.RequestInformationRef.PagingSize;

        string[] scopes = new string[] { "UserActivity", "GroupActivity", "Notifications" };
        criteria.AddFilter(CmsMessageProperty.MessageTypeScope, CriteriaFilterOperator.In, scopes);
        criteria.AddFilter(CmsMessageProperty.LanguageId, CriteriaFilterOperator.EqualTo, _refContentApi.GetCookieValue("LastValidLanguageID"));
        criteria.AddFilter(CmsMessageProperty.SiteId, CriteriaFilterOperator.EqualTo, _siteId);

        AddNewMessage.Visible = false;
        NotificationMessageList = _NotificationMessageApi.GetList(criteria);
        TotalPagesNumber = page.TotalPages;
        PageSettings();

        if (NotificationMessageList != null)
        {
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("TITLE", "" + msgHelper.GetMessage("event title") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("ID", "" + msgHelper.GetMessage("generic subscriptionid") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", "" + msgHelper.GetMessage("generic Type") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(9), Unit.Percentage(9), false, false));
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("SUBTYPE", "" + msgHelper.GetMessage("generic subtype") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(11), Unit.Percentage(11), false, false));
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("LANGUAGE", "" + msgHelper.GetMessage("lbl language") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            ViewMessageGrid.Columns.Add(_refStyle.CreateBoundField("DEFAULT", "" + msgHelper.GetMessage("lbl default") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(2), Unit.Percentage(2), false, false));


            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("SUBTYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
            dt.Columns.Add(new DataColumn("DEFAULT", typeof(string)));
            for (int i = 0; i <= NotificationMessageList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr["TITLE"] = "<a href=\"notificationmessages.aspx?mode=viewnotificationmsg&id=" + NotificationMessageList[i].Id + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID") + "&siteId=" + _siteId.ToString() + "\">" + NotificationMessageList[i].Title + "</a>";
                dr["ID"] = NotificationMessageList[i].Id;
                dr["TYPE"] = NotificationMessageList[i].MessageType.Scope;
                dr["SUBTYPE"] = NotificationMessageList[i].MessageType.Name;
                for (int iLang = 0; iLang <= languageData.Length - 1; iLang++)
                {
                    strName = languageData[iLang].LocalName;
                    if (NotificationMessageList[i].LanguageId == languageData[iLang].Id)
                    {
                        strSelectedLanguageName = strName;
                    }
                }
                dr["LANGUAGE"] = "<center><img src=" + objLocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(NotificationMessageList[i].LanguageId)) + " alt=\"" + strSelectedLanguageName + "\" title=\"" + strSelectedLanguageName + "\" /></center>";
                if (NotificationMessageList[i].IsDefaultMessage)
                {
                    dr["DEFAULT"] = "<img src=\"" + _refCommonApi.AppPath + "images/UI/Icons/check.png\" alt=\"Default Message\"/>";
                }
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            ViewMessageGrid.DataSource = dv;
            ViewMessageGrid.DataBind();
        }
    }

    private void DeleteNotificationMessage()
    {
        CmsMessageData data = null;
        data = GetNotificationMessageData();
        try
        {
            _NotificationMessageApi.Delete(data.Id);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
        }
        Response.Redirect((string)("notificationmessages.aspx?mode=viewnotificationmsggrid&siteId=" + _siteId.ToString()), false);
    }

    private CmsMessageData GetNotificationMessageData()
    {
        CmsMessageData data = null;
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            long.TryParse(Request.QueryString["id"], out messageId);
        }
        if (messageId == 0)
        {
            throw (new ArgumentException("Message ID does not exists."));
        }
        data = _NotificationMessageApi.GetItem(messageId);
        if (data == null)
        {
            throw (new NullReferenceException("Message not found"));
        }
        return data;
    }
    protected string Util_StripScript(string text)
    {
        text = text.Replace("<script>", "");
        text = text.Replace("</script>", "");
        return text;
    }
    private void PageSettings()
    {
        if (TotalPagesNumber <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling((double)TotalPagesNumber)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (_currentPageNumber == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }
    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    private void ParseCurrentPageNumber()
    {
        int.TryParse(CurrentPage.Text, out _currentPageNumber);
        if (_currentPageNumber <= 0) _currentPageNumber = 1;
    }


    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber++;
                break;
            case "Prev":
                _currentPageNumber--;
                break;
        }
        DisplayGrid();
        isPostData.Value = "true";
    }
    private void SetServerJSVariables()
    {
        ltr_follErr.Text = msgHelper.GetMessage("alert msg foll fields");
        ltr_titleErr.Text = msgHelper.GetMessage("js: alert title required");
        ltr_subErr.Text = msgHelper.GetMessage("js err blog subject");
        ltr_bodyErr.Text = msgHelper.GetMessage("js err body required");
        delSubScriptionMsg.Text = msgHelper.GetMessage("js: confirm delete subscriptionmessage");
    }
    public StringBuilder AddLanguageView()
    {
        int i;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (EnableMultilingual == 1)
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">" + msgHelper.GetMessage("lbl View") + ":</td>");
            result.Append("<td>");
            result.Append("<select name=\"language\" ID=\"language\" onchange=\"SelLanguage(this.value)\">>");
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                result.Append("<option value=\"" + (colActiveLanguages[i].Id) + "\" ");
                if (ContentLanguage == colActiveLanguages[i].Id)
                {
                    result.Append("selected ");
                }
                result.Append(">" + (colActiveLanguages[i].Name) + "</option>");
            }
            result.Append("</select>");
            result.Append("</td>");
        }
        return result;
    }

    private StringBuilder AddSiteView()
    {
        System.Text.StringBuilder siteResult = new System.Text.StringBuilder();
		siteResult.Append(StyleHelper.ActionBarDivider);
        siteResult.Append("<td class=\"label\">");
        siteResult.Append(_refContentApi.EkMsgRef.GetMessage("lbl site colon"));
        siteResult.Append("</td>");
        siteResult.Append("<td>");
        siteResult.Append("<select name=\"siteSearchItem\" id=\"siteSearchItem\" ONCHANGE=\"SubmitForm(\'form1\',\'\');\"/>&nbsp;");

        System.Collections.Generic.Dictionary<long, string> siteDictionary;
        System.Collections.Generic.KeyValuePair<long, string> siteList;
        Ektron.Cms.UrlAliasing.UrlAliasManualApi manualAliasList = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();

        siteDictionary = manualAliasList.GetSiteList();

        foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
        {
            siteList = tempLoopVar_siteList;
            if (siteList.Key == _siteId)
            {
                siteResult.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
                break;
            }
        }
        foreach (System.Collections.Generic.KeyValuePair<long, string> tempLoopVar_siteList in siteDictionary)
        {
            siteList = tempLoopVar_siteList;
            if (siteList.Key != _siteId)
            {
                siteResult.Append("<option value=\"" + siteList.Key.ToString() + "\">" + siteList.Value + "</option>");
            }
        }

        siteResult.Append("</select>");
        siteResult.Append("</td>");

        return siteResult;
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}