using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.PageBuilder;

public partial class Workarea_MenuActions : System.Web.UI.Page
{
    #region Enumerations - Private

    private enum ActionTypeValue
    {
        Approve,
        CheckIn,
        Delete,
        Publish,
        Submit,
        RequestCheckIn,
        RotateCW,
        RotateCCW
    }

    private enum MenuTypeValue
    {
        Workarea,
        CommunityGroup,
        CommunityUser,
        Taxonomy
    }

    #endregion

    #region Variables - Member

    private string m_ArgumentExceptionMessage;
    private string m_ActionExceptionMessage;
    private long m_ContentId;
    private int m_LanguageId;
    private ActionTypeValue m_ActionType;
    private CommonApi m_CommonApi;
    private ContentAPI m_ContentApi;
    private EkContent m_EkContent;
    private MenuTypeValue m_MenuType;
    private EkMessageHelper m_refMsg;

    #endregion

    #region Properties - Private

    private ActionTypeValue ActionType
    {
        get { return m_ActionType; }
        set { m_ActionType = value; }
    }

    private MenuTypeValue MenuType
    {
        get { return m_MenuType; }
        set { m_MenuType = value; }
    }

    private long ContentId
    {
        get { return m_ContentId; }
        set { m_ContentId = value; }
    }

    private int LanguageId
    {
        get { return m_LanguageId; }
        set { m_LanguageId = value; }
    }

    private string ArgumentExceptionMessage
    {
        get { return m_ArgumentExceptionMessage; }
        set { m_ArgumentExceptionMessage = value; }
    }

    private string ActionExceptionMessage
    {
        get { return m_ActionExceptionMessage; }
        set { m_ActionExceptionMessage = value; }
    }

    #endregion

    #region Method - Constructor

    public Workarea_MenuActions()
    {
        m_CommonApi = new CommonApi();
        m_ContentApi = new ContentAPI();
        m_EkContent = m_ContentApi.EkContentRef;
        m_refMsg = m_ContentApi.EkMsgRef; 
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        //30257 - This document should never be cached.  When content status changes
        //the page is refreshed to show change in status.  This change does 
        //not appear when page is cached.
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        #region retrieve param - action

        try
        {
            this.ActionType = (ActionTypeValue)Enum.Parse(typeof(ActionTypeValue), Request.QueryString["action"], true);
        }
        catch (ArgumentException myArgumentException)
        {
            //the page request is trying to execute an action not specified by the "ActionTypeValue" enum (and is not supported)
            this.ArgumentExceptionMessage = myArgumentException.Message;
        }

        #endregion

        #region retrieve param - menu type

        try
        {
            this.MenuType = (MenuTypeValue)Enum.Parse(typeof(MenuTypeValue), Request.QueryString["menuType"], true);
        }
        catch (ArgumentException myArgumentException)
        {
            //the page request does not specify the type of menu
            this.ArgumentExceptionMessage = myArgumentException.Message;
        }

        #endregion

        #region retrieve param - content id

        try
        {
            this.ContentId = Convert.ToInt64(Request.QueryString["contentId"]);
        }
        catch (ArgumentException myArgumentException)
        {
            //the page request does not inlcude a valid contentId param
            this.ArgumentExceptionMessage = myArgumentException.Message;
        }

        #endregion

        #region retrieve param - language id

        try
        {
            this.LanguageId = Convert.ToInt32(Request.QueryString["LangType"]);
            if (this.LanguageId == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || this.LanguageId == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
            {
                this.LanguageId = m_ContentApi.DefaultContentLanguage;
            }
            m_CommonApi.ContentLanguage = this.LanguageId;
            m_ContentApi.ContentLanguage = this.LanguageId;
        }
        catch (ArgumentException myArgumentException)
        {
            //the page request does not inlcude a valid LangType param
            //Don't throw, use default language instead
            this.ArgumentExceptionMessage = myArgumentException.Message;
            m_CommonApi.ContentLanguage = m_ContentApi.DefaultContentLanguage;
            m_ContentApi.ContentLanguage = m_ContentApi.DefaultContentLanguage;
        }

        #endregion

        //after querystring params have been retrieved and validated, try to execute the specified action
        try
        {
            this.ExecuteAction();
        }
        catch (ApplicationException myApplicationException)
        {
            this.ActionExceptionMessage = myApplicationException.Message;
        }
    }

    protected bool IsValidUser() {
        if (m_ContentApi.RequestInformationRef.UserId == 0) {
            Response.Redirect(m_ContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return false;
        }
        return true;
    }

    protected void ExecuteAction()
    {
        switch (this.ActionType)
        {
            case ActionTypeValue.Approve:
                if (IsValidUser()) {
                    this.Approve();
                }
                break;
            case ActionTypeValue.CheckIn:
                if (IsValidUser()) {
                    this.CheckIn();
                }
                break;
            case ActionTypeValue.Delete:
                if (IsValidUser()) {
                    this.Delete();
                }
                break;
            case ActionTypeValue.Publish:
                if (IsValidUser()) {
                    this.Publish();
                }
                break;
            case ActionTypeValue.Submit:
                if (IsValidUser()) {
                    this.Submit();
                }
                break;
            case ActionTypeValue.RequestCheckIn:
                if (IsValidUser()) {
                    this.RequestCheckIn();
                }
                break;
            case ActionTypeValue.RotateCW:
                this.RotateCW();
                break;
            case ActionTypeValue.RotateCCW:
                this.RotateCCW();
                break;
        }
    }

    #region Methods - Protected, Action

    protected void Approve()
    {
        m_ContentApi.ApproveContent(this.ContentId);
    }

    protected void CheckIn()
    {
        try
        {
            //This switch is in here to solve bug #31390 & #31278.  This problem is specific to non-Workarea menus
            //Until the checkin for Community Documents is updated, we must call SaveContent before Checkin
            //This does not apply for Workarea menus.
            ContentEditData cEditData = null;
            ContentData contData = null;
            switch (this.MenuType)
            {
                case MenuTypeValue.Workarea:
                    contData = m_ContentApi.GetContentById(this.ContentId, ContentAPI.ContentResultType.Staged);
                    if ((contData != null) && (contData.UserId != m_ContentApi.RequestInformationRef.UserId) && (contData.Status == "O"))
                        m_EkContent.TakeOwnershipForAdminCheckIn(this.ContentId);
                    cEditData = m_ContentApi.GetContentForEditing(this.ContentId);
                    cEditData.FileChanged = false;
                    m_ContentApi.SaveContent(cEditData);
                    m_EkContent.CheckIn(this.ContentId, String.Empty);
                    break;
                default:
                    contData = m_ContentApi.GetContentById(this.ContentId, ContentAPI.ContentResultType.Staged);
                    if ((contData != null) && (contData.UserId != m_ContentApi.RequestInformationRef.UserId) && (contData.Status == "O"))
                        m_EkContent.TakeOwnershipForAdminCheckIn(this.ContentId);
                    m_EkContent.CheckContentOutv2_0(this.ContentId);
                    cEditData = m_ContentApi.GetContentForEditing(this.ContentId);
                    cEditData.FileChanged = false;
                    m_ContentApi.SaveContent(cEditData);
                    m_EkContent.CheckIn(this.ContentId, String.Empty);
                    break;
            }
        }
        catch (ApplicationException myApplicationException)
        {
            this.ActionExceptionMessage = myApplicationException.Message;
        }
    }

    protected void Delete()
    {
        long folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        try
        {
            //if this is a master layout, check if it's used anywhere. if not then go ahead and delete, but otherwise, don't.
            if (m_EkContent.GetContentSubType(this.ContentId, this.LanguageId) == EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
            {
                TemplateModel tm = new TemplateModel();
                TemplateData template = tm.FindByMasterLayoutID(this.ContentId);
                if(template != null){
                    long[] folders = m_EkContent.GetTemplateDefaultFolderUsage(template.Id);
                    Microsoft.VisualBasic.Collection contentblocks = m_EkContent.GetTemplateContentBlockUsage(template.Id);
                    if (folders.Length == 0 && contentblocks.Count == 0)
                    {
                        m_EkContent.SubmitForDeletev2_0(this.ContentId, folderId);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Cannot delete this master layout.\r\n");
                        if (folders.Length > 0)
                        {
                            sb.Append("The following folders are associated with it:\r\n");
                            for (int i = 0; i < folders.Length; i++)
                            {
                                sb.Append(m_EkContent.GetFolderPath(folders[i]).Replace('\\', '/') + '/');
                                if (i != folders.Length - 1) sb.Append(",");
                                sb.Append("\r\n");
                            }
                        }
                        if (contentblocks.Count > 0)
                        {
                            sb.Append("The following content is associated with it:\r\n");
                            for (int i = 1; i <= contentblocks.Count; i++)
                            {
                                Microsoft.VisualBasic.Collection inner = (Microsoft.VisualBasic.Collection)contentblocks[i];
                                ContentData cd = m_EkContent.GetContentById((long)inner["content_id"], EkContent.ContentResultType.Published);
                                sb.Append("/" + cd.Path + cd.Title + "   (id: " + cd.Id + ", lang: " + cd.LanguageId + ")");
                                if (i != contentblocks.Count) sb.Append(",\r\n");
                            }
                        }
                        DmsMenuActionsRepsonse.Text = "message:" + sb.ToString();
                    }
                }
            }
            else
            {
                m_EkContent.SubmitForDeletev2_0(this.ContentId, folderId);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("delete master layout"))
            {
                DmsMenuActionsRepsonse.Text = "message:" + ex.Message;
            }
            else
            {
                throw ex;
            }
        }
    }

    protected void Publish()
    {
        PermissionData myPermissionData;
        myPermissionData = m_ContentApi.LoadPermissions(this.ContentId, "content", ContentAPI.PermissionResultType.All);
        if (myPermissionData.CanPublish && myPermissionData.CanApprove)
        {
            m_EkContent.Approvev2_0(this.ContentId);
        }
        else if (myPermissionData.CanPublish == true)
        {
            long folderId = Convert.ToInt64(Request.QueryString["folderId"]);
            m_EkContent.SubmitForPublicationv2_0(this.ContentId, folderId, String.Empty);
        }
        else
        {
            this.ActionExceptionMessage = "Error: User does not have permission to publish";
            throw (new ApplicationException(this.ActionExceptionMessage));
        }
    }

    protected void Submit()
    {
       long folderId = Convert.ToInt64(Request.QueryString["folderId"]);
        m_EkContent.SubmitForPublicationv2_0(this.ContentId, folderId, String.Empty);
    }

    protected void RequestCheckIn()
    {
        #region Instantiate Local Variables

        long userIdFrom = Convert.ToInt64(Request.QueryString["userIdFrom"]);
        long userIdTo = Convert.ToInt64(Request.QueryString["userIdTo"]);

        ContentData myContentData = new ContentData();
        Folder myFolder = new Folder();
        FolderData myFolderData = new FolderData();
        myContentData = m_ContentApi.GetContentById(this.ContentId, ContentAPI.ContentResultType.Published);
        myFolderData = myFolder.GetFolder(myContentData.FolderId);

        #endregion

        #region Retrieve User Data

        UserData userFromData = new UserData();
        UserData userToData = new UserData();
        UserAPI myUserApi = new UserAPI();
		ContentAPI contAPI = new ContentAPI();
        userFromData = myUserApi.GetUserById(userIdFrom, true, true);
		 //Calling EkUserRw instead of UserApi to skip permissions check
        Ektron.Cms.DataIO.EkUserRW usrObj = new Ektron.Cms.DataIO.EkUserRW(contAPI.RequestInformationRef);
        Microsoft.VisualBasic.Collection uCol = usrObj.GetUserByIDv2_6(userIdTo, true, true);
        userToData = usrObj.ConvertUserData(uCol);

        #endregion

        #region Send "Request Check-In" Email Message

        EkMessageHelper messageHelper = new EkMessageHelper(m_ContentApi.RequestInformationRef);
        EkMailService mail = new EkMailService(m_ContentApi.RequestInformationRef);

        if (userFromData.Email != String.Empty && userToData.Email != String.Empty)
        {
            try
            {
                mail.MailFrom = userFromData.Email;
                mail.MailCC = userFromData.Email;
                mail.MailSubject = messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInSubject", this.LanguageId) + @": " + myContentData.Title;
                mail.MailBodyText = this.GetBodyText(messageHelper, myContentData, myFolderData, userFromData.FirstName, userFromData.LastName);
                mail.MailTo = userToData.Email;
                mail.SendMail();
                DmsMenuActionsRepsonse.Text = messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInSucceeded", this.LanguageId) + @" " + userToData.FirstName + @" " + userToData.LastName + @" (" + userToData.Email + @")";
            }
            catch
            {
                DmsMenuActionsRepsonse.Text = messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInFailed", this.LanguageId);
            }
        }
        else
        {
            DmsMenuActionsRepsonse.Text = messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInFailedNoEmail", this.LanguageId);
        }

        #endregion
    }

    protected void RotateCW()
    {
        m_EkContent.RotateAssetImage(this.ContentId, 90);
    }

    protected void RotateCCW()
    {
        m_EkContent.RotateAssetImage(this.ContentId, -90);
    }

    #endregion

    #region Methods - Private, Helper

    private string GetBodyText(EkMessageHelper messageHelper, ContentData myContentData, FolderData myFolderData, string userFromFirstName, string userFromLastName)
    {
        StringBuilder bodyText = new StringBuilder();
        bodyText.Append(messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInMessagePart1", this.LanguageId));
        bodyText.Append(Environment.NewLine);
        bodyText.Append(messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInMessagePart2", this.LanguageId));
        bodyText.Append(@": " + myContentData.Title);
        bodyText.Append(Environment.NewLine);
        bodyText.Append(messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInMessagePart3", this.LanguageId));
        bodyText.Append(@": " + myFolderData.Name);
        bodyText.Append(Environment.NewLine);
        bodyText.Append(messageHelper.GetMessageForLanguage("DmsMenuRequestCheckInMessagePart4", this.LanguageId));
        bodyText.Append(@": " + userFromFirstName + @" " + userFromLastName);
        return bodyText.ToString();
    }

    #endregion
}
