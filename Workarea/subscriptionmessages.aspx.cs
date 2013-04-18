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

public partial class suscriptionmessages : System.Web.UI.Page
{
    protected Ektron.ContentDesignerWithValidator ctlEditor;
    protected EkMessageHelper m_refMsg;
    protected SiteAPI m_refSiteAPI = new SiteAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected long m_intFontId = 0;
    protected long m_intSubscriptionId = 0;
    protected FontData font_data;
    protected EmailMessageData subscriptionmessage_data;
    protected ContentAPI m_refContApi = new ContentAPI();
    protected string m_strSubscriptionNameFromUserControl;
    protected bool m_strSubscriptionEnableFromUserControl;
    protected int EnableMultilingual;
    protected int ContentLanguage = -1;
    protected long m_intId = 0;
    protected LanguageData[] colActiveLanguages;
    const string PAGE_NAME = "subscriptionmessages.aspx";
    protected string m_strPageMode = "";
    protected string m_strModeQueryString = "";
    protected bool IsMac = false;
    private int iMaxContLength = 64000;
    private string var2 = "";
    private EkContent m_refContent;
    private CommonApi m_refAPI = null;
    protected string imagePath = "";
    // paging
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        Ektron.Cms.API.JS.RegisterJS(Page, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(Page, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
        this.InitializeEditor();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        try
        {
            int i;
            bool blnLanguageMatched = false;
            m_refAPI = new CommonApi();
            Utilities.SetLanguage(m_refSiteAPI);
            m_refMsg = m_refContApi.EkMsgRef;
            RegisterResources();
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }

            if (m_strPageMode == "commerce")
            {
                Util_CheckECommerceAccess();
            }
            else
            {
                Util_CheckAccess();
            }

            delSubScriptionMsg.Text = m_refMsg.GetMessage("js: confirm delete subscriptionmessage");
            if (m_refContApi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0) == false || m_refAPI.UserId == 0 || Convert.ToBoolean(m_refAPI.RequestInformationRef.IsMembershipUser))
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }

            StyleSheetJS.Text = m_refStyle.GetClientScript();
            EnableMultilingual = m_refContApi.EnableMultilingual;
            imagePath = this.m_refContApi.AppPath + "images/ui/icons/";
            if (!(Request.QueryString["action"] == null))
            {
                m_strPageAction = Request.QueryString["action"];
                if (m_strPageAction.Length > 0)
                {
                    m_strPageAction = m_strPageAction.ToLower();
                }
            }

            //INITIALIZE THE VARIABLES
           
            jsIsMac.Text = "false";
            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                IsMac = true;
                jsIsMac.Text = "true";
            }

            if (!(Request.QueryString["mode"] == null))
            {
                m_strPageMode = Request.QueryString["mode"];
                if (m_strPageMode.Length > 0)
                {
                    m_strPageMode = m_strPageMode.ToLower();
                }
            }

            switch (m_strPageMode.ToLower())
            {
                case "userprop":
                    m_strModeQueryString = "&mode=userprop";
                    break;
                case "forum":
                    m_strModeQueryString = "&mode=forum";
                    break;
                case "commerce":
                    m_strModeQueryString = "&mode=commerce";
                    break;
            }

            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            if (EnableMultilingual == 1)
            {
                colActiveLanguages = m_refSiteAPI.GetAllActiveLanguages();
            }

            m_refContApi.ContentLanguage = ContentLanguage;
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
                    m_refContApi.SetCookieValue("LastValidLanguageID", m_refContApi.DefaultContentLanguage.ToString());
                    m_refContApi.ContentLanguage = m_refContApi.DefaultContentLanguage;
                    ContentLanguage = m_refContApi.DefaultContentLanguage;
                }
            }

            ctlEditor.AllowFonts = true;
            ctlEditor.ErrorMessage = m_refMsg.GetMessage("content size exceeded");
            ctlEditor.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength);
            AppImgPath = m_refContApi.AppImgPath;
            TR_AddEditSubscription.Visible = false;
            TR_ViewSubscription.Visible = false;
            TR_ViewAllFont.Visible = false;
            if (!(Page.IsPostBack))
            {
                switch (m_strPageAction)
                {
                    case "viewallsubscriptions":
                        Display_ViewAllSubscriptionMessages();
                        break;
                    case "view":
                        Display_ViewSubscriptionMessage();
                        break;
                    case "edit":
                        Display_EditSubscriptionMessage();
                        break;
                    case "add":
                        Display_AddSubscriptionMessage();
                        break;
                    case "delete":
                        Process_DeleteSubscriptionMessage();
                        break;
                    default:
                        Display_ViewAllSubscriptionMessages();
                        break;
                }
            }
            else
            {
                ctlEditor.Validate();
                if (ctlEditor.IsValid)
                {
                    switch (m_strPageAction)
                    {
                        case "edit":
                            Process_EditSubscriptionMessage();
                            break;
                        case "add":
                            Process_AddSubscriptionMessage();
                            break;
                        case "delete":
                            Process_DeleteSubscriptionMessage();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private void Process_EditSubscriptionMessage()
    {
        long id = 0;
        long.TryParse(Request.QueryString["id"], out id);
        EmailMessageData emailMessage = m_refContApi.EmailMessagesRef.GetEmailMessageItem(id);
        ////EmailMessageData emailMessage = m_refContApi.EmailMessagesRef.GetEmailMessageItem(Request.QueryString["id"]);
        emailMessage.Title = Request.Form["txtName"];
        emailMessage.Text = Util_StripScript((string)ctlEditor.Content);
        if (((Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 0) || (Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 1)) || (Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 2))
        {
            emailMessage.Subject = "";
        }
        else
        {
            emailMessage.Subject = Request.Form["txtSubject"];
        }

        if (!(Request.Form["chkDefaultAddEdit"] == null))
        {
            emailMessage.DefaultMessage = System.Convert.ToInt32(chkDefaultAddEdit.Checked);
        }

        if (Request.Form["drpTypeAddEdit"] != "")
        {
            emailMessage.Type = (Ektron.Cms.Common.EkEnumeration.EmailMessageTypes)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes), Request.Form["drpTypeAddEdit"], true);
        }
        else
        {
            emailMessage.Type = Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification;
        }

        m_refContApi.EmailMessagesRef.UpdateEmailMessage(emailMessage);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions" + m_strModeQueryString), false);
    }

    private void Process_AddSubscriptionMessage()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(EkFunctions.HtmlEncode(Request.Form["txtName"]), "TITLE", null, null);
        pagedata.Add(Util_StripScript((string)ctlEditor.Content), "TEXT", null, null);
        if (((Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 0) || (Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 1)) || (Convert.ToInt32(Request.Form["drpTypeAddEdit"]) == 2))
        {
            pagedata.Add("", "SUBJECT", null, null);
        }
        else
        {
            pagedata.Add(Request.Form["txtSubject"], "SUBJECT", null, null);
        }

        pagedata.Add(Request.Form["chkDefaultAddEdit"], "ISDEFAULT", null, null);
        if (Request.Form["drpTypeAddEdit"] != "")
        {
            pagedata.Add(Request.Form["drpTypeAddEdit"], "TYPE", null, null);
        }
        else // we can assume verification
        {
            pagedata.Add(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification.GetHashCode(), "TYPE", null, null);
        }

        m_refContApi.AddSubscriptionMessage(pagedata);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions" + m_strModeQueryString), false);
    }

    private void Process_DeleteSubscriptionMessage()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(Request.QueryString["SubscriptionID"], "ID", null, null);
        m_refContApi.DeleteSubscriptionMessage(pagedata);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions" + m_strModeQueryString), false);
    }

    private void Display_EditSubscriptionMessage()
    {
        jsSetActionFunction.Text = SetActionClientScript();
        m_refContent = m_refSiteAPI.EkContentRef;
        var2 = m_refContent.GetEditorVariablev2_0(0, "tasks");
        AutoNav.Text = "";
        InitializedrpType();
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        if (!(Request.QueryString["id"] == null))
        {
            m_intSubscriptionId = Convert.ToInt64(Request.QueryString["id"]);
        }

        subscriptionmessage_data = m_refContApi.GetSubscriptionMessageById(m_intSubscriptionId);
        txtName.Text = subscriptionmessage_data.Title;
        ltrAddEditID.Text = subscriptionmessage_data.Id.ToString() + "<input type=\"hidden\" name=\"subscriptionID\" value=\"" + subscriptionmessage_data.Id.ToString() + "\"/>";
        txtSubject.Text = subscriptionmessage_data.Subject;
        ctlEditor.Content = subscriptionmessage_data.Text;
        drpTypeAddEdit.SelectedValue = subscriptionmessage_data.Type.GetHashCode().ToString();

        //----- Show the default checkbox if this is a validation email message.  Also check the box according
        //----- to what is set in the database.  If this is the default message check the box.
        if (subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.FriendInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.SiteInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ForumPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewForumTopic || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ReportForumPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewModeratedForumTopic || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.PrivateMessage || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ResetPassword || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.RequestResetPassword || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ModeratedBlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCancelled || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceived || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderShipped || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCompleted || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin)
        {
            ltrDefaultAddEdit.Text = m_refMsg.GetMessage("Default label");
            tr_default.Visible = true;
            if (subscriptionmessage_data.DefaultMessage == 1)
            {
                chkDefaultAddEdit.Checked = true;
                chkDefaultAddEdit.Enabled = false;
            }
            else
            {
                chkDefaultAddEdit.Checked = false;
                chkDefaultAddEdit.Enabled = true;
            }
        }
        else
        {
            tr_default.Visible = false;
        }

        //only friendInvite, SiteInvite supports subject right now.
        if (!ShowSubject(subscriptionmessage_data.Type))
        {
            rowSubject.Style.Add("display", "none");
        }

        EditSubscriptionToolBar();
    }

    private void Display_AddSubscriptionMessage()
    {
        jsSetActionFunction.Text = SetActionClientScript();
        m_refContent = m_refSiteAPI.EkContentRef;
        var2 = m_refContent.GetEditorVariablev2_0(0, "tasks");
        AutoNav.Text = "";
        InitializedrpType();
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        ctlEditor.Content = "";
        TD_SubscriptionID.Visible = false;

        //----- Show the default checkbox for new validation email message.
        if (m_strPageMode == "userprop" || m_strPageMode == "forum" || m_strPageMode == "commerce")
        {
            ltrDefaultAddEdit.Text = m_refMsg.GetMessage("Default label");
            tr_default.Visible = true;
            chkDefaultAddEdit.Checked = false;
            chkDefaultAddEdit.Enabled = true;
        }
        else
        {
            tr_default.Visible = false;
            chkDefaultAddEdit.Checked = false;
            chkDefaultAddEdit.Enabled = false;
        }

        //only friendInvite, SiteInvite supports subject right now.
        EkEnumeration.EmailMessageTypes currentlySelected = (EkEnumeration.EmailMessageTypes)Enum.Parse(typeof(EkEnumeration.EmailMessageTypes), drpTypeAddEdit.Items[0].Value);
        if ((
            !(this.m_strPageMode == "forum" || this.m_strPageMode == "commerce")) &&
            !ShowSubject(currentlySelected)
            )
        {
            rowSubject.Style.Add("display", "none");
        }

        AddSubscriptionToolBar();
    }

    private void Display_ViewSubscriptionMessage()
    {
        InitializedrpType();
        TR_ViewSubscription.Visible = true;
        if (!(Request.QueryString["id"] == null))
        {
            m_intSubscriptionId = Convert.ToInt64(Request.QueryString["id"]);
        }

        subscriptionmessage_data = m_refContApi.GetSubscriptionMessageById(m_intSubscriptionId);
        ltrViewName.Text = subscriptionmessage_data.Title;
        ltrViewID.Text = subscriptionmessage_data.Id.ToString();
        viewContentHTML.Text = subscriptionmessage_data.Text;
        drpType.SelectedValue = subscriptionmessage_data.Type.GetHashCode().ToString();
        literalSubject.Text = EkFunctions.HtmlEncode(subscriptionmessage_data.Subject);

        //----- Show the default checkbox if this is a validation email message.  Also check the box according
        //----- to what is set in the database.  If this is the default message check the box.
        if (subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.FriendInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.SiteInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupInvitation || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ForumPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewForumTopic || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ReportForumPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewModeratedForumTopic || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.PrivateMessage || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalPost || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ResetPassword || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.RequestResetPassword || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ModeratedBlogComment || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCancelled || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceived || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderShipped || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCompleted || subscriptionmessage_data.Type == Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin)
        {
            ltrDefault.Text = m_refMsg.GetMessage("Default label");
            tr_defaultView.Visible = true;
            if (subscriptionmessage_data.DefaultMessage == 1)
            {
                chkDefault.Checked = true;
            }

            ViewSubscriptionToolBar(System.Convert.ToBoolean(subscriptionmessage_data.DefaultMessage == 1 ? true : false));
        }
        else
        {
            tr_defaultView.Visible = false;
            ViewSubscriptionToolBar(false);
        }

        if (!ShowSubject(subscriptionmessage_data.Type))            
        {
            rowSubjectView.Style.Add("display", "none");
        }
    }

    private void Display_ViewAllSubscriptionMessages()
    {
        TR_ViewAllFont.Visible = true;
        EmailMessageData[] subscriptionMessage_data_list;
        List<EmailMessageData> messageList = new List<EmailMessageData>();
        Ektron.Cms.Common.Criteria<EmailMessageProperty> criteria = new Ektron.Cms.Common.Criteria<EmailMessageProperty>();
        List<int> typeList = new List<int>();
        bool showIsDefault = System.Convert.ToBoolean(m_strPageMode == "userprop" || m_strPageMode == "forum" || m_strPageMode == "commerce");
        switch (m_strPageMode)
        {
            case "commerce":
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceived));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCancelled));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderShipped));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCompleted));
				typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin));
                criteria.AddFilter(EmailMessageProperty.Type, CriteriaFilterOperator.In, typeList);
                criteria.AddFilter(EmailMessageProperty.LanguageId, CriteriaFilterOperator.EqualTo, m_refSiteAPI.ContentLanguage);
                m_refSiteAPI.EmailMessagesRef.GetEmailMessageList(criteria);
                subscriptionMessage_data_list = messageList.ToArray();
                break;

            case "forum":
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewForumTopic));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ForumPost));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ReportForumPost));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewModeratedForumTopic));
                break;

            case "userprop":
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.FriendInvitation));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.SiteInvitation));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupInvitation));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.PrivateMessage));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogComment));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalComment));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogPost));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogPost));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalPost));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogComment));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ModeratedBlogComment));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ResetPassword));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.RequestResetPassword));
                break;

            case "":
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe));
                typeList.Add(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut));
                break;
        }

        if (typeList.Count > 0)
        {
            criteria.AddFilter(EmailMessageProperty.Type, CriteriaFilterOperator.In, typeList);
        }

        criteria.AddFilter(EmailMessageProperty.LanguageId, CriteriaFilterOperator.EqualTo, m_refSiteAPI.ContentLanguage);
        criteria.PagingInfo.RecordsPerPage = m_refSiteAPI.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;
        criteria.OrderByField = EmailMessageProperty.Type;
        messageList = m_refSiteAPI.EmailMessagesRef.GetEmailMessageList(criteria);
        TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);
        if (TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {
            lnkBtnPreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            LastPage.Enabled = true;
            NextPage.Enabled = true;
            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            lnkBtnPreviousPage.Visible = true;
            NextPage.Visible = true;
            LastPage.Visible = true;
            FirstPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(TotalPagesNumber))).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            if (_currentPageNumber == 1)
            {
                lnkBtnPreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }

        subscriptionMessage_data_list = messageList.ToArray();
        if (!(subscriptionMessage_data_list == null))
        {
            System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TITLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic Title");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";
            colBound.HeaderStyle.VerticalAlign = VerticalAlign.Top;
            colBound.HeaderStyle.Wrap = false;
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ID";
            colBound.HeaderText = m_refMsg.GetMessage("generic SubscriptionID");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "TYPE";
            colBound.HeaderText = m_refMsg.GetMessage("generic Type");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Language";
            colBound.HeaderText = m_refMsg.GetMessage("generic SubscriptionLanguageID");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.CssClass = "title-header";
            ViewSubscriptionGrid.Columns.Add(colBound);

            if (showIsDefault)
            {
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "Default";
                colBound.HeaderText = m_refMsg.GetMessage("Default label");
                colBound.ItemStyle.Wrap = false;
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
                colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                colBound.HeaderStyle.CssClass = "title-header";
                ViewSubscriptionGrid.Columns.Add(colBound);
            }

            DataTable dt = new DataTable();
            DataRow dr;
            int i = 0;
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("Language", typeof(string)));

            //----- Show the default checkbox for new validation email message.
            if (showIsDefault)
            {
                dt.Columns.Add(new DataColumn("Default", typeof(string)));
            }

            for (i = 0; i <= subscriptionMessage_data_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"" + PAGE_NAME + "?action=View&id=" + subscriptionMessage_data_list[i].Id + m_strModeQueryString + "\" title=\'" + m_refMsg.GetMessage("alt Click Here to View the Email Message") + " \"" + Strings.Replace(subscriptionMessage_data_list[i].Title, "\'", "`", 1, -1, 0) + "\"\'>" + subscriptionMessage_data_list[i].Title + "</a>";
                dr[1] = subscriptionMessage_data_list[i].Id.ToString();
                dr[2] = GetResourceText( subscriptionMessage_data_list[i].Type.ToString());
                dr[3] = subscriptionMessage_data_list[i].LanguageId;
                if (showIsDefault && (subscriptionMessage_data_list[i].DefaultMessage == 1))
                {
                    dr[4] = "<img src=\"" + imagePath + "check.png\" alt=\"Default Message\"/>";
                }

                dt.Rows.Add(dr);
            }

            ViewSubscriptionGrid.BorderColor = System.Drawing.Color.White;
            DataView dv = new DataView(dt);
            ViewSubscriptionGrid.DataSource = dv;
            ViewSubscriptionGrid.DataBind();
        }

        ViewAllSubscriptionsToolBar();
    }

    private void AddSubscriptionToolBar()
    {
        divTitleBar.InnerHtml = m_refMsg.GetMessage("lbl Add Email Message");
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions" + m_strModeQueryString), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "save.png", "#", m_refMsg.GetMessage("lbl Add Email Message"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm( \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider); 
		switch (m_strPageMode.ToLower())
        {
            case "userprop":
                result.Append("<td>" + m_refStyle.GetHelpButton("Addcommunityemailmessage", "") + "</td>");
                break;
            case "forum":
                result.Append("<td>" + m_refStyle.GetHelpButton("AddDiscBoardlmessage", "") + "</td>");
                break;
            default:
                result.Append("<td>" + m_refStyle.GetHelpButton("Addemailmessage", "") + "</td>");
                break;
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void ViewSubscriptionToolBar(bool bIsDefaultMessage)
    {
        divTitleBar.InnerHtml = m_refMsg.GetMessage("lbl view email message") + " \"" + subscriptionmessage_data.Title + "\"";
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int i = 0;
        result.Append("<table><tr>" + "\r\n");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions" + m_strModeQueryString), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "contentEdit.png", System.Convert.ToString(PAGE_NAME + "?action=Edit&id=" + m_intSubscriptionId.ToString() + m_strModeQueryString + ""), m_refMsg.GetMessage("lbl Edit Email Message"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));

        //----- If this is the default message, do not show the delete button.
        if (!bIsDefaultMessage)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "delete.png", System.Convert.ToString(PAGE_NAME + "?action=delete&SubscriptionID=" + m_intSubscriptionId.ToString() + m_strModeQueryString + ""), m_refMsg.GetMessage("alt delete email message"), m_refMsg.GetMessage("btn delete"), "OnClick=\"javascript: return ConfirmFontDelete();\"", StyleHelper.DeleteButtonCssClass));
        }

        if (EnableMultilingual == 1)
        {
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">" + m_refMsg.GetMessage("lbl View") + ":</td>");
            result.Append("<td>");
            result.Append("<select name=\"language\" ID=\"language\" OnChange=\"JavaScript:SelLanguage(this.value);\">>");
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
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageMode + "ViewSubscriptionEmailMessage", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void ViewAllSubscriptionsToolBar()
    {
        divTitleBar.InnerHtml = m_refMsg.GetMessage("lbl View All Messages");
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int i = 0;
        bool blnSelectDefault = true;
        bool blnSelectFirst = true;
        result.Append("<table><tr>" + "\r\n");
        result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "add.png", System.Convert.ToString(PAGE_NAME + "?action=Add" + m_strModeQueryString), m_refMsg.GetMessage("lbl Add Email Message"), m_refMsg.GetMessage("lbl Add Email Message"), "", StyleHelper.AddButtonCssClass, true));
        if (EnableMultilingual == 1)
        {
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                if (ContentLanguage == colActiveLanguages[i].Id)
                {
                    blnSelectDefault = false;
                    blnSelectFirst = false;
                }
                if (m_refContApi.DefaultContentLanguage == colActiveLanguages[i].Id)
                {
                    blnSelectFirst = false;
                }
            }
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">" + m_refMsg.GetMessage("lbl View") + ":</td>");
            result.Append("<td>");
            result.Append("<select name=\"language\" ID=\"language\" OnChange=\"JavaScript:SelLanguage(this.value)\">>");
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                result.Append("<option value=\"" + (colActiveLanguages[i].Id) + "\" ");
                if ((!blnSelectDefault) && (!blnSelectFirst))
                {
                    if (ContentLanguage == colActiveLanguages[i].Id)
                    {
                        result.Append("selected ");
                    }
                }
                else if (blnSelectFirst)
                {
                    if (i == 0)
                    {
                        result.Append("selected ");
                        m_refContApi.SetCookieValue("LastValidLanguageID", System.Convert.ToString(colActiveLanguages[i].Id));
                        m_refContApi.ContentLanguage = colActiveLanguages[i].Id;
                    }
                }
                else
                {
                    if (m_refContApi.DefaultContentLanguage == colActiveLanguages[i].Id)
                    {
                        result.Append("selected ");
                        m_refContApi.SetCookieValue("LastValidLanguageID", m_refContApi.DefaultContentLanguage.ToString());
                        m_refContApi.ContentLanguage = m_refContApi.DefaultContentLanguage;
                    }
                }

                result.Append(">" + (colActiveLanguages[i].Name) + "</option>");
            }
            result.Append("</select>");
            result.Append("</td>");
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageMode + "ViewAllMessages", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void EditSubscriptionToolBar()
    {
        divTitleBar.InnerHtml = m_refMsg.GetMessage("lbl Edit Email Message") + " \"" + subscriptionmessage_data.Title + "\"";
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=View&id=" + Request.QueryString["id"] + m_strModeQueryString + ""), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "save.png", "#", m_refMsg.GetMessage("lbl update email message"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider); 
		result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageMode + "EditEmailMessage", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    private string GetResourceText(string st)
    {
        if (st == "BlogPost")
            st = m_refMsg.GetMessage("generic BlogPost");
        else if (st == "Unsubscribe")
            st = m_refMsg.GetMessage("res_mem_unsub");
        else
            st = m_refMsg.GetMessage(st);
        
        return st;
    }
    private void InitializedrpType()
    {
        ListItem liTypeItem = new ListItem();
        if (!Page.IsPostBack)
        {
            switch (m_strPageMode)
            {
                case "commerce":

                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceived.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceived.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCancelled.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCancelled.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderShipped.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderShipped.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCompleted.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderCompleted.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
					liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    break;

                case "forum":
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ForumPost.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ForumPost.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewForumTopic.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewForumTopic.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ReportForumPost.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ReportForumPost.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewModeratedForumTopic.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.NewModeratedForumTopic.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    break;

                case "userprop":

                    //Show only messages types those are for user properties
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Verification.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.FriendInvitation.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.FriendInvitation.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.SiteInvitation.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.SiteInvitation.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupInvitation.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupInvitation.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.PrivateMessage.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.PrivateMessage.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogComment.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogComment.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalComment.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalComment.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogPost.ToString());
                    liTypeItem.Value =  System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.BlogPost.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text =  GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogPost.ToString());
                    liTypeItem.Value =  System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogPost.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalPost.ToString());
                    liTypeItem.Value =  System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.UserJournalPost.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogComment.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.GroupBlogComment.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ModeratedBlogComment.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ModeratedBlogComment.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ResetPassword.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.ResetPassword.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.RequestResetPassword.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.RequestResetPassword.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    break;

                default:
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    liTypeItem = new ListItem();
                    liTypeItem.Text = GetResourceText(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage.ToString());
                    liTypeItem.Value = System.Convert.ToString(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage.GetHashCode());
                    drpType.Items.Add(liTypeItem);
                    drpTypeAddEdit.Items.Add(liTypeItem);
                    break;
            }
        }
    }

    private string SetActionClientScript()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("function VerifyForm () {" + "\r\n");
        result.Append("	document.forms[0].txtName.value = Trim(document.forms[0].txtName.value);" + "\r\n");
        result.Append("	if (document.forms[0].txtName.value == \"\")" + "\r\n");
        result.Append("	{" + "\r\n");
        result.Append("		alert (\"" + m_refMsg.GetMessage("subscription name required msg") + "\");" + "\r\n");
        result.Append(" document.forms[0].txtName.focus();" + "\r\n");
        result.Append("	return false;" + "\r\n");
        result.Append("}" + "\r\n");
        result.Append("return true;" + "\r\n");
        result.Append("}" + "\r\n");
        return (result.ToString());
    }

    private void Util_CheckAccess()
    {
        PermissionData securityData = this.m_refContApi.LoadPermissions(0, "folder", ContentAPI.PermissionResultType.All);
        if (!string.IsNullOrEmpty(Request.QueryString["boardid"]))
        {
            if (!securityData.IsLoggedIn || (!m_refContApi.IsARoleMemberForFolder_FolderUserAdmin(Convert.ToInt64(Request.QueryString["boardid"]), m_refContApi.RequestInformationRef.UserId, false) && !m_refContApi.IsAdmin()) || securityData.IsInMemberShip)
            {
                throw (new Exception(m_refMsg.GetMessage("msg login cms administrator")));
            }
        }
        else
        {
            if (!securityData.IsLoggedIn || !securityData.IsAdmin || securityData.IsInMemberShip)
            {
                throw (new Exception(m_refMsg.GetMessage("msg login cms administrator")));
            }
        }
    }

    protected void Util_CheckECommerceAccess()
    {
        if (Convert.ToBoolean(m_refContApi.RequestInformationRef.IsMembershipUser) || (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refSiteAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) && !m_refSiteAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            throw (new Exception(m_refMsg.GetMessage("err not role commerce-admin")));
        }
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
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }

        Display_ViewAllSubscriptionMessages();
        isPostData.Value = "true";
    }

    protected string Util_StripScript(string text)
    {
        text = text.Replace("<script>", "");
        text = text.Replace("</script>", "");
        return text;
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }

    private void InitializeEditor()
    {
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("Controls/Editor/ContentDesignerWithValidator.ascx");
        phEditor.Controls.Add(ctlEditor);
        ctlEditor.ID = "txtTextAddEdit";
        ctlEditor.Height = new Unit(100, UnitType.Percentage);
        ctlEditor.AllowScripts = false;
        ctlEditor.Width = new Unit(100, UnitType.Percentage);
        ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
        ctlEditor.ShowHtmlMode = true;
    }

    private bool ShowSubject(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes messageType)
    {
        List<EkEnumeration.EmailMessageTypes> showSubjectTypes =
            new List<EkEnumeration.EmailMessageTypes>()
            {
                EkEnumeration.EmailMessageTypes.Verification,
                EkEnumeration.EmailMessageTypes.FriendInvitation,
                EkEnumeration.EmailMessageTypes.SiteInvitation,
                EkEnumeration.EmailMessageTypes.GroupInvitation,
                EkEnumeration.EmailMessageTypes.ForumPost,
                EkEnumeration.EmailMessageTypes.NewForumTopic,
                EkEnumeration.EmailMessageTypes.ReportForumPost,
                EkEnumeration.EmailMessageTypes.NewModeratedForumTopic,
                EkEnumeration.EmailMessageTypes.PrivateMessage,
                EkEnumeration.EmailMessageTypes.BlogComment,
                EkEnumeration.EmailMessageTypes.UserJournalComment,
                EkEnumeration.EmailMessageTypes.BlogPost,
                EkEnumeration.EmailMessageTypes.GroupBlogPost,
                EkEnumeration.EmailMessageTypes.UserJournalPost,
                EkEnumeration.EmailMessageTypes.ResetPassword,
                EkEnumeration.EmailMessageTypes.RequestResetPassword,
                EkEnumeration.EmailMessageTypes.GroupBlogComment,
                EkEnumeration.EmailMessageTypes.ModeratedBlogComment,
                EkEnumeration.EmailMessageTypes.OrderReceived,
                EkEnumeration.EmailMessageTypes.OrderShipped,
                EkEnumeration.EmailMessageTypes.OrderCancelled,
                EkEnumeration.EmailMessageTypes.OrderCompleted,
                EkEnumeration.EmailMessageTypes.OrderReceivedToAdmin
            };
        return showSubjectTypes.Exists(x => x == messageType);
    }
}