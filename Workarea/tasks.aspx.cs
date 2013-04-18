using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;

public partial class tasks : System.Web.UI.Page
{
    #region private and protected members
    protected StyleHelper m_refStyle = new StyleHelper();
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected Ektron.Cms.Site.EkSite m_ekSite;
    protected PermissionData security_task_data;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper MsgHelper;
    protected EmailHelper m_refEmail = new EmailHelper();
    public ApplicationAPI AppUI = new ApplicationAPI();
    protected Ektron.Cms.Content.EkTaskType objTaskType;
    protected Collection colAllCategory = new Collection();
    protected ContentDesignerWithValidator ctlEditor;
    protected Ektron.Cms.Content.EkTask objTask;
    protected Ektron.Cms.Content.EkTask cTask;
    protected Ektron.Cms.Content.EkTasks cTasks;
    protected Ektron.Cms.Site.EkSite gtSiteObj;
    protected Collection cComments;
    protected EkContent cContObj;
    protected Ektron.Cms.User.EkUser usrObj;
    protected Collection grpObj;
    protected Collection cGroups;
    protected Collection cUserInfo;
    protected Hashtable colActiveLanguages;
    protected Hashtable cDbRecs;
    protected EmailHelper m_EmailHelper = new EmailHelper();
    protected Collection canI;
    protected Collection colTaskType = new Collection();
    protected Collection colParentCategory = new Collection();
    protected string languageName;
    protected string strIs_Child;
    protected long lngCategory_tmp;
    protected long lngType_tmp;
    protected long lngCompareCategoryID;
    protected long lngCompareTaskTypeID;
    protected string completedColor;
    protected string localeFileString;
    protected string var1;
    protected string var2;
    protected int IsMac;
    private string Action = "";
    private string iMaxContLength = "64000";
    protected int EnableMultilingual;
    protected int ContentLanguage;
    protected string AppPath;
    protected string AppImgPath;
    protected string sitePath;
    protected string AppeWebPath;
    protected string AppName;
    protected long CommentKeyId;
    protected long CommentId;
    protected long RefId;
    protected string RefType;
    protected long currentUserID;
    protected string OrderBy;
    protected string action;
    protected string actiontype;
    protected bool g_isIEFlagInitialized;
    protected bool is_child = false;
    protected int languageID;
    protected string ErrorString = string.Empty;
    protected string callBackPage = string.Empty;
    protected bool ret;
    protected string actErrorString = "";
    protected string content_title;
    protected bool IsAdmin = false;
    protected int TaskItemType;
    protected int State;
    protected string sTitleBar;
    protected string closeOnFinish = "";
    protected string fromType = string.Empty;
    protected long TaskID;
    protected string fromViewContent = string.Empty;
    protected Ektron.Cms.Framework.User.UserGroupManager groupManager = new Ektron.Cms.Framework.User.UserGroupManager();
    protected int CurrentPage = 1;
    #endregion
    #region Page Events
    protected void Page_Init(System.Object sender, System.EventArgs e)
    {
        EnableMultilingual = AppUI.EnableMultilingual;
        cContObj = new EkContent(m_refContentApi.RequestInformationRef);
        objTask = new EkTask(m_refContentApi.RequestInformationRef);
        MsgHelper = new EkMessageHelper(m_refContentApi.RequestInformationRef);
        objTaskType = new Ektron.Cms.Content.EkTaskType(m_refContentApi.RequestInformationRef);
        usrObj = new Ektron.Cms.User.EkUser(m_refContentApi.RequestInformationRef);
        GenerateJS();
        ltrStyleHelper.Text = m_refStyle.GetClientScript();
        ltrEmailScript.Text = m_EmailHelper.EmailJS();
        FillLiterals();
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
        ctlEditor.ID = "description";
        ctlEditor.AllowScripts = true;
        ctlEditor.Height = new Unit(450, UnitType.Pixel);
        ctlEditor.Width = new Unit(100, UnitType.Percentage);
        ctlEditor.ToolsFile = m_refContentApi.ApplicationPath + "ContentDesigner/configurations/InterfaceTask.xml";
        ctlEditor.AllowFonts = true;
        ctlEditor.ShowHtmlMode = false;
        ctlEditor.Stylesheet = m_refContentApi.ApplicationPath + "csslib/ewebeditprostyles.css";
        RegularExpressionValidator ctlValidator = new RegularExpressionValidator();
        Ektron.Cms.Common.EkMessageHelper m_refMsg = m_refSiteApi.EkMsgRef;
        ctlValidator.ID = "RegExpValidator";
        ctlValidator.ControlToValidate = "description";
        ctlValidator.ErrorMessage = m_refMsg.GetMessage("content size exceeded");
        ctlValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(int.Parse(iMaxContLength));
        Action = EkFunctions.HtmlEncode(Request.QueryString["action"]);
        if ("AddTask" == Action)
        {
            AddTaskValidatorHolder.Controls.Add(ctlValidator);
            AddTaskEditorHolder.Controls.Add(ctlEditor);
        }
        else if ("EditTask" == Action)
        {
            EditTaskValidatorHolder.Controls.Add(ctlValidator); 
            EditTaskEditorHolder.Controls.Add(ctlEditor); 
        }
        if (Request.QueryString["page"] != null)
        {
            CurrentPage = EkFunctions.ReadIntegerValue(Request.QueryString["page"], 1);
        }
    }
    protected void Page_Load(System.Object sender, System.EventArgs e)
    {
        m_refStyle = new StyleHelper();
        divPropertyText.Text = "Standard"; 
        divMetadataText.Text = "Advanced"; 
        checkAccess();
        g_isIEFlagInitialized = false;
        lngCompareCategoryID = 0;
        lngCompareTaskTypeID = 0;
        completedColor = "#ccffff";
        CommentKeyId = -1;
        RefType = "T";
        RefId = Convert.ToInt64(Request.QueryString["tid"]);
        CommentId = 0;
        iMaxContLength = "64000";
        localeFileString = "0000";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            OrderBy = Request.QueryString["orderby"].ToString();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (!string.IsNullOrEmpty(AppUI.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = Convert.ToInt32(AppUI.GetCookieValue("LastValidLanguageID"));
            }
        }
        AppUI.ContentLanguage = ContentLanguage;
        currentUserID = AppUI.UserId;
        AppPath = AppUI.AppPath;
        AppImgPath = AppUI.AppImgPath;
        sitePath = AppUI.SitePath;
        AppeWebPath = AppUI.ApplicationPath + AppUI.AppeWebPath;
        AppName = AppUI.AppName;
        EnableMultilingual = AppUI.EnableMultilingual;
        gtSiteObj = AppUI.EkSiteRef;
        if (AppUI.EnableMultilingual == 1)
            colActiveLanguages = gtSiteObj.GetAllActiveLanguages();
        else
        {
            colActiveLanguages = gtSiteObj.GetLanguageByID(AppUI.DefaultContentLanguage);
            if (colActiveLanguages.Count > 0)
            {
                languageName = colActiveLanguages["Name"].ToString();
            }
        }
        strIs_Child = Strings.LCase(Strings.Trim(Convert.ToString(Server.HtmlEncode(Request.QueryString["is_child"]))));
        lngCategory_tmp = Convert.ToInt64(Request.QueryString["saved_task_category"]);
        lngType_tmp = Convert.ToInt64(Request.QueryString["saved_task_type"]);

        if (!string.IsNullOrEmpty(strIs_Child))
            is_child = Convert.ToBoolean("true" == strIs_Child);
        else
            is_child = false;

        if (Request.ServerVariables["HTTP_USER_AGENT"].IndexOf("Windows") + 1 > 0)
        {
            IsMac = 0;
        }
        else
        {
            IsMac = 1;
        }
        cContObj = AppUI.EkContentRef;
        canI = cContObj.CanIv2_0(-1, "tasks");
        //var1 = Request.ServerVariables["SERVER_NAME"].ToString();
        //string ty = null;
        //ty = "tasks";
        //var2 = cContObj.GetEditorVariablev2_0(0, ty);
        if (m_refContentApi.GetCookieValue("user_id") != "0")
        {
            m_EmailHelper.MakeEmailArea();
            MakeTaskTypeArea();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["close"]))
        {
            closeOnFinish = "&close=true";
        }
        m_ekSite = AppUI.EkSiteRef;
        cDbRecs = m_ekSite.GetPermissions(0, 0, "folder");
        IsAdmin = Convert.ToBoolean(cDbRecs["IsAdmin"]);

        action = Server.HtmlEncode(Request.QueryString["action"]);
        actiontype = Server.HtmlEncode(Request.QueryString["ty"]);
        if ((string.IsNullOrEmpty(actiontype)))
        {
            actiontype = "both";
        }

    
        if (IsPostBack)
        {
            switch (Action)
            {
                case "AddTask":
                    Process_AddTask();
                    break;
                case "EditTask":
                    Process_EditTask();
                    break;
                case "DeleteAllTasks":
                    Process_DeleteAllTasks();
                    break;
                case "UpdateStatePurge":
                    Process_UpdateStatePurge();
                    break;
            }
        }
        else
        {
            switch (Action)
            {
                case "ApproveTask":
                    Process_ApproveTask();
                    break;
                case "EditTask":
                    Display_EditTask();
                    break;
                case "AddTask":
                    Display_AddTask();
                    break;
                case "ViewTasks":
                    Display_ViewTasks();
                    break;
                case "ViewTask":
                    Display_ViewTask();
                    break;
                case "viewcontenttask":
                    Display_ViewContentTask();
                    break;
                case "ViewTaskType":
                    Display_ViewTaskType();
                    break;
                case "DeleteAllTasks":
                    Display_DeleteAllTasks();
                    break;
                case "DeleteTask":
                    Process_DeleteTask();
                    break;
                case "EditTaskType":
                    Display_EditTaskType();
                    break;
                case "AddTaskType":
                    Display_AddTaskType();
                    break;
                case "DeleteTaskType":
                    Process_DeleteTaskType();
                    break;
                default:
                    Session["RedirectLnk"] = "tasks.aspx?" + Server.HtmlEncode(Request.QueryString.ToString());
                    Response.Redirect("login.aspx?fromLnkPg=1");
                    break;
            }

        }

        RegisterResources();
    }
    #endregion
    #region Display Tasks Events
    private void Display_AddTask()
    {
        ValidateCanCreateTask();
        GetAddTaskToolBar();
        Collection cConts = new Collection();
        string retString = string.Empty;
        pnlAddTask.Visible = true;
        StringBuilder sbAddTask = new StringBuilder();
        objTask = AppUI.EkTaskRef;
        jsId.Text = Convert.ToString(ContentId);
        if (ContentId > 0)
        {
            objTask.LanguageID = objTask.ContentLanguage;
            cConts = cContObj.GetContentByIDv2_0(ContentId);
            if (cConts.Count == 0)
            {
                retString = MsgHelper.GetMessage("error: content does not exist") + " " + ContentId;
            }
            else
            {
                content_title = cConts["ContentTitle"].ToString();
                objTask.ContentID = ContentId;
                objTask.ContentTitle = content_title;
            }
        }
        if (retString != string.Empty)
        {
            Response.Redirect("reterror.aspx?info=" + retString, false);
        }
        sbAddTask.Append("<tr>");
        sbAddTask.Append("   <td class=\"label\" title=\"Assigned to\"> " + MsgHelper.GetMessage("lbl Assigned to") + "</td>");
        sbAddTask.Append("   <td class=\"value\">");
        if (IsBrowserNS4())
        {
            sbAddTask.Append("<input type=\"text\" title=\"Enter Name here\" name=\"nsfourname\" size=\"15\" value=\"\" id=\"assigned_to\"/>");
            if (Convert.ToBoolean(canI["CanIRedirectTask"]))
            {
                sbAddTask.Append("<a title=\"Select user email report\" class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"ShowUsers();\">" + MsgHelper.GetMessage("select user email report") + "</a>");
            }
        }
        else
        {
            sbAddTask.Append("<div id=\"user\" style=\"display: none;\"></div>");
            if (Convert.ToBoolean(canI["CanIRedirectTask"]))
            {
                sbAddTask.Append("<a title=\"Select user or group\" href=\"#\" class=\"button buttonInline greenHover minHeight buttonCheckAll\" onclick=\"ShowUsers();\">" + MsgHelper.GetMessage("select user email report") + "</a>");
            }
            sbAddTask.Append("<div id=\"nouser\" style=\"display: block;\"></div>");
        }

        sbAddTask.Append("   </td>");
        sbAddTask.Append("</tr>");
        if (Convert.ToBoolean(AppUI.EnableMultilingual))
        {
            sbAddTask.Append("<tr>");
            sbAddTask.Append("   <td class=\"label\" title=\"Language\">" + MsgHelper.GetMessage("res_lngsel_lbl") + "</td>");
            sbAddTask.Append("   <td class=\"value\">");
            sbAddTask.Append("    <select name=\"language\" id=\"language\"");
            if (ContentId > 0)
            {
                sbAddTask.Append(" disabled=\"true\" ");
            }
            sbAddTask.Append("> ");
            for (int i = 0; i < colActiveLanguages.Count; i++)
            {
                Hashtable coll = (Hashtable)colActiveLanguages[i];
                sbAddTask.Append("   <option value=\"" + coll["ID"] + "\"");
                if (objTask.LanguageID == Convert.ToInt32(coll["ID"]))
                {
                    sbAddTask.Append(" selected ");
                }
                sbAddTask.Append(">" + coll["Name"]);
                sbAddTask.Append("   </option>");
            }
            sbAddTask.Append("    </select>");
            sbAddTask.Append("   </td>");
            sbAddTask.Append("</tr>");
        }
        if (ContentId > 0)
        {
            sbAddTask.Append("<tr>");
            sbAddTask.Append("   <td class=\"label\" title=\"Content\">" + MsgHelper.GetMessage("content content label") + "</td>");
            sbAddTask.Append("   <td class=\"value\">" + ContentId + "&nbsp;" + content_title + "</td>");
            sbAddTask.Append("</tr>");
        }
        else
        {
            sbAddTask.Append("<tr>");
            sbAddTask.Append("   <td class=\"label\" title=\"Content\">" + MsgHelper.GetMessage("content content label") + "</td>");
            sbAddTask.Append("   <td class=\"value\">");
            if (IsBrowserNS4())
            {
                sbAddTask.Append("   <input type=\"text\" title=\"Enter Content Text here\" name=\"nsfourname\" size=\"15\" value=\"Change\" id=\"contentidspan\"/><a title=\"Select All\" class=\"button buttonInline greenHover minHeight minHeight buttonCheckAll\" href=\"#\" onclick=\"LoadChildPage();return true;\">Select</a>");
            }
            else
            {
                sbAddTask.Append("   <div id=\"Div2\" style=\"display: none;\"></div><div id=\"contentidspan\" style=\"display: inline-block;\"><a title=\"Select All\" class=\"button buttonInline greenHover minHeight buttonCheckAll\"  href=\"#\" onclick=\"LoadChildPage();return true;\">" + MsgHelper.GetMessage("generic select") + "</a></div><a title=\"Change\" class=\"button buttonInline greenHover minHeight buttonEdit\" href=\"#\" id=\"a_change\" name=\"a_change\" style=\"visibility: hidden;\" onclick=\"LoadChildPage();return true;\">Change</a>&nbsp;&nbsp;<a title=\"Clear\" href=\"#\" class=\"button buttonInline redHover minHeight\" id=\"a_none\" name=\"a_none\" style=\"visibility: hidden;\" onclick=\"UnSelectContent();return true;\">Clear</a>");
            }
            sbAddTask.Append("   </td>");
            sbAddTask.Append("</tr>");
        }
        sbAddTask.Append("<tr>");
        sbAddTask.Append("   <td class=\"label\">" + MsgHelper.GetMessage("lbl priority") + "</td>");
        sbAddTask.Append("   <td class=\"value\">");
        sbAddTask.Append("    <select name=\"priority\" ID=\"priority\">");
        sbAddTask.Append("       <option value=\"1\"");
        if (Convert.ToInt32(objTask.Priority) == 1)
        {
            sbAddTask.Append(" selected ");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl low") + "</option>");
        sbAddTask.Append("       <option value=\"2\"");
        if (Convert.ToInt32(objTask.Priority) == 2)
        {
            sbAddTask.Append(" selected ");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl normal") + "</option>");
        sbAddTask.Append("       <option value=\"3\"");
        if (Convert.ToInt32(objTask.Priority) == 3)
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl high") + "</option>");
        sbAddTask.Append("    </select>");
        sbAddTask.Append("   </td>");
        sbAddTask.Append("</tr>");
        sbAddTask.Append(DisplayTaskType(action));
        sbAddTask.Append("<tr>");
        sbAddTask.Append("   <td class=\"label\">" + MsgHelper.GetMessage("lbl state") + ":</td>");
        sbAddTask.Append("   <td class=\"value\">");
        if (objTask.ContentID != -1)
        {
            sbAddTask.Append("<select name=\"state\" disabled id=\"state\">");
        }
        else
        {
            sbAddTask.Append("<select name=\"state\" id=\"state\">");
        }
        sbAddTask.Append("    <option title=\"Not started\" value=\"1\" ");
        if (objTask.State == "NotStarted")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl not started") + "</option>");
        sbAddTask.Append("<option title=\"Active\" value=\"2\"");
        if (objTask.State == "Active")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl active") + "</option>");
        sbAddTask.Append("<option title=\"Awaiting Data\" value=\"3\"");
        if (objTask.State == "AwaitingData")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl awaiting data") + "</option>");
        sbAddTask.Append("<option title=\"On Hold\" value=\"4\"");
        if (objTask.State == "OnHold")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl on hold") + "</option>");
        sbAddTask.Append("<option title=\"Pending\" value=\"5\"");
        if (objTask.State == "Pending")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl pending") + "</option>");
        sbAddTask.Append("<option title=\"Reopened\" value=\"6\"");
        if (objTask.State == "Reopened")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl reopened") + "</option>");
        sbAddTask.Append("<option title=\"Completed\" value=\"7\"");
        if (objTask.State == "Completed")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl completed") + "</option>");
        sbAddTask.Append("<option title=\"Archived\" value=\"8\"");
        if (objTask.State == "Archived")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl archived") + "</option>");
        sbAddTask.Append("<option title=\"Deleted\" value=\"9\"");
        if (objTask.State == "Deleted")
        {
            sbAddTask.Append(" selected");
        }
        sbAddTask.Append(">" + MsgHelper.GetMessage("lbl deleted") + "</option>");
        sbAddTask.Append("    </select>");
        sbAddTask.Append("   </td>");
        sbAddTask.Append("</tr>");
        ltrAddTask.Text = sbAddTask.ToString();
        usrObj = AppUI.EkUserRef;
        cUserInfo = usrObj.GetUserByIDv2_0(currentUserID, false);
        StringBuilder sbScript = new StringBuilder();
        if (Convert.ToBoolean(canI["CanIRedirectTask"]))
        {
            sbScript.Append("selectUser('1', '" + currentUserID + "', '" + cUserInfo["UserName"] + "',1);");
        }
        else
        {
            sbScript.Append("selectUser('1', '" + currentUserID + "', '" + cUserInfo["UserName"] + "',0);");
        }
        Ektron.Cms.API.JS.RegisterJSBlock(this, sbScript.ToString(), "EktronTasksSelectUserJS");
    }
    private void Display_EditTask()
    {
        ValidateCanEditTask();
        pnlEditTask.Visible = true;
        if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
        {
            TaskID = Convert.ToInt64(Request.QueryString["tid"]);
        }
        callBackPage = BuildCallBackParms("&");
        cTask = objTask.GetTaskByID(TaskID);
        ctlEditor.Content = cTask.Description;
        if (cTask.TaskTypeID == (long)EkEnumeration.TaskType.BlogPostComment)
        {
            IsMac = 1;
            ltrEditTaskScript.Text = "<script type=\"text/javascript\">" + Constants.vbCrLf + "var postcomment = true;" + Constants.vbCrLf + "</script>";
        }
        else
        {
            ltrEditTaskScript.Text = "<script type=\"text/javascript\">" + Constants.vbCrLf + "var postcomment = false;" + Constants.vbCrLf + "</script>";
        }
        StringBuilder sbTemp = new StringBuilder();
        StringBuilder sbAdvanced = new StringBuilder();
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append("<td class=\"label\">Title:</td>" + Environment.NewLine);
        sbTemp.Append("     <td><input type=\"Text\" name=\"task_title\" maxlength=\"200\" value=\"" + (cTask.TaskTitle) + "\" onkeypress=\"return CheckKeyValue(event,'34');\" ID=\"task_title\"></td>");
        sbTemp.Append("</tr>" + Environment.NewLine);
        switch (cTask.TaskTypeID)
        {
            case (long)EkEnumeration.TaskType.BlogPostComment:
                ltrNotBlogTopic.Text = sbTemp.ToString();
                break;
            case (long)EkEnumeration.TaskType.TopicReply:
                sbAdvanced.Append("<input type=\"hidden\" name=\"task_title\" id=\"task_title\" value=\"" + (Ektron.Cms.Common.EkFunctions.HtmlEncode(cTask.TaskTitle)) + "\"/>");
                break;
            default:
                ltrNotBlogTopic.Text = sbTemp.ToString();
                break;
        }
        sbTemp = new StringBuilder();
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Task ID:</td>" + Environment.NewLine);
        sbTemp.Append(" <td>" + cTask.TaskID.ToString() + "</td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Assigned To:</td>" + Environment.NewLine);
        sbTemp.Append(" <td align=\"left\">" + Environment.NewLine);
        if (IsBrowserNS4())
        {
            sbTemp.Append("<input type=\"text\" name=\"nsfourname\" size=\"15\" value=\"");
            if ((cTask.AssignToUserGroupID == 0))
            {
                sbTemp.Append("All Authors");
            }
            else if ((!string.IsNullOrEmpty(cTask.AssignedToUser)))
            {
                sbTemp.Append(GetAssignedToUserFullName(cTask) + " (" + cTask.AssignedToUser + ")".Replace("'", "`"));
            }
            else if ((!string.IsNullOrEmpty(cTask.AssignedToUserGroup)))
            {
                sbTemp.Append(cTask.AssignedToUserGroup.Replace("'", "`"));
            }
            sbTemp.Append("\" ID=\"user\" />");
            if (Convert.ToBoolean(canI["CanIRedirectTask"]))
            {
                sbTemp.Append("<a class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"ShowUsers();\">" + MsgHelper.GetMessage("select user email report") + "</a>" + Environment.NewLine);
            }
        }
        else
        {
            sbTemp.Append("<div id=\"user\" style=\"display: block;\">");
            if ((cTask.AssignToUserGroupID == 0))
            {
                sbTemp.Append("All Authors");
            }
            else if ((!string.IsNullOrEmpty(cTask.AssignedToUser)))
            {
                sbTemp.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" border=\"0\" align=\"absbottom\">" + GetAssignedToUserFullName(cTask) + " (" + cTask.AssignedToUser + ")".Replace("'", "`"));
            }
            else if ((!string.IsNullOrEmpty(cTask.AssignedToUserGroup)))
            {
                sbTemp.Append("<img src=\"" + AppPath + "images/UI/Icons/users.png\" border=\"0\" align=\"absbottom\">" + cTask.AssignedToUserGroup.Replace("'", "`"));
            }
            if (Convert.ToBoolean(canI["CanIRedirectTask"]))
            {
                sbTemp.Append("<a class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"ShowUsers();\">" + MsgHelper.GetMessage("select user email report") + " </a>");
            }
            sbTemp.Append("&nbsp;</div>");
            sbTemp.Append("<div id=\"nouser\" style=\"display: none;\"></div>" + Environment.NewLine);
        }
        sbTemp.Append(" </td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        sbTemp.Append("<tr> " + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Assigned By:</td>" + Environment.NewLine);
        sbTemp.Append(" <td align=\"left\">" + Environment.NewLine);
        if ((cTask.TaskTypeID == (long)EkEnumeration.TaskType.BlogPostComment | cTask.TaskTypeID == (long)EkEnumeration.TaskType.TopicReply) & (cTask.AssignedByUserID == Ektron.Cms.Common.EkConstants.BuiltIn.ToString()))
        {
            sbTemp.Append(" " + Environment.NewLine);
        }
        else
        {
            sbTemp.Append(" " + GetAssignedByUserFullName(cTask) + " (" + cTask.AssignedByUser + ")".Replace("'", "`") + Environment.NewLine);
        }
        sbTemp.Append(" </td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        if (Convert.ToBoolean(AppUI.EnableMultilingual))
        {
	        sbTemp.Append("<tr>" + Environment.NewLine);
	        sbTemp.Append(" <td class=\"label\">Language:</td>" + Environment.NewLine);
	        sbTemp.Append(" <td>" + Environment.NewLine);
	        sbTemp.Append("     <select name=\"language\" ID=\"language\" ");
	        if (cTask.ContentID != -1) 
            {
		        sbTemp.Append("disabled=\"true\" ");
	        }
	        sbTemp.Append(">");
            sbTemp.Append(">");
            if (cTask.LanguageID == 0)
            {
                sbTemp.Append("     <option value=\"0\" ");
                sbTemp.Append(" selected ");
                sbTemp.Append(">App Default</option>");

            }
            for (int i = 0; i < colActiveLanguages.Count; i++)
            {
                Hashtable activeLanguage = (Hashtable)colActiveLanguages[i];
                sbTemp.Append("     <option value=\"" + (activeLanguage["ID"]) + "\" ");
                if (cTask.LanguageID == Convert.ToInt64(activeLanguage["ID"]))
                {
                    sbTemp.Append(" selected ");
                }
                sbTemp.Append(">" + activeLanguage["Name"] + "</option>");

            }
	        sbTemp.Append(" </select>" + Environment.NewLine);
	        sbTemp.Append(" </td>" + Environment.NewLine);
	        sbTemp.Append("</tr>" + Environment.NewLine);
        }
        if (cTask.TaskTypeID == (long)EkEnumeration.TaskType.BlogPostComment | cTask.TaskTypeID == (long)EkEnumeration.TaskType.TopicReply)
        {
            sbAdvanced.Append(sbTemp.ToString());
        }
        else
        {
            ltrNotBlogTopic2.Text = sbTemp.ToString();
        }
        sbTemp = new StringBuilder();
        if ((cTask.ContentID != -1))
        {
            sbTemp.Append("<div id=\"div3\" style=\"display: none;\"></div><span id=\"contentidspan\" style=\"display: inline-block; background-color: #fff; margin-right: .5em; border: solid 1px #DEDEDE; padding-right: .5em; padding-left: .5em;\">(" + cTask.ContentID + ")&nbsp;" + cTask.ContentTitle + "</span>");
            if (!(cTask.TaskTypeID == (long)EkEnumeration.TaskType.BlogPostComment | cTask.TaskTypeID == (long)EkEnumeration.TaskType.TopicReply))
            {
                sbTemp.Append("<a class=\"button buttonInline greenHover minHeight buttonEdit\" href=\"#\" id=\"a_change\" name=\"a_change\" style=\"visibility: visible;\" onclick=\"LoadChildPage();return true;\">Change</a>" + "&nbsp;&nbsp;<a href=\"#\" class=\"button buttonInline redHover minHeight buttonNone\" id=\"a_none\" name=\"a_none\"  style=\"visibility: visible;\" onclick=\"UnSelectContent();return true;\">Clear</a>");
            }
        }
        else
        {
            if (IsBrowserNS4())
            {
                sbTemp.Append("<input type=\"text\" title=\"Enter Content Text here\" name=\"nsfourname\" size=\"15\" value=\"Change\" id=\"contentidspan\" /><a class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"LoadChildPage();return true;\">" + MsgHelper.GetMessage("generic select") + "</a>");
            }
            else
            {
                sbTemp.Append("<div id=\"div3\" style=\"display: none;\"></div>");
                sbTemp.Append("<div id=\"contentidspan\" style=\"display: inline-block;\">");
                sbTemp.Append(" <a title=\"Select All\" class=\"button buttonInline greenHover minHeight buttonCheckAll\" href=\"#\" onclick=\"LoadChildPage();return true;\">Select</a>");
                sbTemp.Append("</div>");
                sbTemp.Append("<a title=\"Change\" class=\"button buttonInline greenHover minHeight buttonEdit\" href=\"#\" id=\"a_change\" name=\"a_change\" style=\"visibility: hidden;\" onclick=\"LoadChildPage();return true;\">Change</a>&nbsp;&nbsp;<a title=\"Clear\" class=\"button buttonInline redHover minHeight buttonNone\" href=\"#\" id=\"a_none\" name=\"a_none\" style=\"visibility: hidden;\" onclick=\"UnSelectContent();return true;\">Clear</a>");
            }
        }
        ltrEditTaskBody1.Text = sbTemp.ToString();
        sbTemp = new StringBuilder();
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Priority:</td>" + Environment.NewLine);
        sbTemp.Append(" <td>" + Environment.NewLine);
        sbTemp.Append("     <select name=\"priority\" ID=\"priority\">" + Environment.NewLine);
        sbTemp.Append("         <option value=\"1\"");
        if ((cTask.Priority == EkEnumeration.TaskPriority.Low))
        {
            sbTemp.Append(" selected");
        }
        sbTemp.Append(">Low</option>" + Environment.NewLine);
        sbTemp.Append("     <option value=\"2\"");
        if ((cTask.Priority == EkEnumeration.TaskPriority.Normal))
        {
            sbTemp.Append(" selected");
        }
        sbTemp.Append(">Normal</option>" + Environment.NewLine);
        sbTemp.Append("     <option value=\"3\"");
        if ((cTask.Priority == EkEnumeration.TaskPriority.High))
        {
            sbTemp.Append(" selected");
        }
        sbTemp.Append(">High</option>" + Environment.NewLine);
        sbTemp.Append("     </select>" + Environment.NewLine);
        sbTemp.Append(" </td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        sbTemp.Append(DisplayTaskType(action));
        if ((Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt64(EkEnumeration.TaskType.BlogPostComment)) | (Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt64(EkEnumeration.TaskType.TopicReply)))
        {
            sbAdvanced.Append(sbTemp.ToString());
        }
        else
        {
           ltrNotBlogTopic3.Text = sbTemp.ToString();
        }
        sbTemp = new StringBuilder();
        HttpCookie cookie = Ektron.Cms.CommonApi.GetEcmCookie();
        if (cTask.TaskTypeID == (long)EkEnumeration.TaskType.TopicReply | cTask.TaskTypeID == (long)EkEnumeration.TaskType.BlogPostComment)
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option value=\"5\" ");
            if (cTask.State == "5") 
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl pending") + "</option>");
            sbTemp.Append("<option value=\"7\" ");
            if (cTask.State == "7")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl approved") + "</option>");
            sbTemp.Append("</select>");
        }
        else if (m_refContentApi.IsAdmin() | cTask.ContentID == -1)
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Not Started\" value=\"1\" ");
            if (cTask.State == "1")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl not started") + "</option>");
            sbTemp.Append("<option title=\"Active\" value=\"2\" ");
            if (cTask.State == "2")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl active") + "</option>");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            sbTemp.Append("<option title=\"Pending\" value=\"5\" ");
            if (cTask.State == "5")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl pending") + "</option>");
            sbTemp.Append("<option title=\"ReOpened\" value=\"6\" ");
            if (cTask.State == "6")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl reopened") + "</option>");
            sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
            if (cTask.State == "7")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            sbTemp.Append("<option title=\"Archived\" value=\"8\" ");
            if (cTask.State == "8")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
            sbTemp.Append("<option title=\"Deleted\" value=\"9\" ");
            if (cTask.State == "9")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl deleted") + "</option>");
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "1")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Not Started\" value=\"1\" ");
            if (cTask.State == "1")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl not started") + "</option>");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            if (cTask.AssignedToUserID == Convert.ToInt64(cookie["user_id"]) || groupManager.IsUserInGroup(Convert.ToInt64(cookie["user_id"]), cTask.AssignToUserGroupID))
            {
                sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
                if (cTask.State == "7")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            }
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "2")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Active\" value=\"2\" ");
            if (cTask.State == "2")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl active") + "</option>");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            if (cTask.AssignedToUserID == Convert.ToInt64(cookie["user_id"]))
            {
                sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
                if (cTask.State == "7")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            }
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "3" | cTask.State == "4")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            if (cTask.AssignedToUserID == Convert.ToInt64(cookie["user_id"]))
            {
                sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
                if (cTask.State == "7")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            }
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "6")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"ReOpened\" value=\"6\" ");
            if (cTask.State == "6")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl reopened") + "</option>");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            if (cTask.AssignedToUserID == Convert.ToInt64(cookie["user_id"]))
            {
                sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
                if (cTask.State == "7")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            }
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "7" & cTask.AssignedToUserID == Convert.ToInt64(cookie["user_id"]))
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
            if (cTask.State == "7")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            sbTemp.Append("<option title=\"Archived\" value=\"8\" ");
            if (cTask.State == "8")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "7" & cTask.AssignedToUserID == -1)
        {
            if(IsUserInGroup(cTask.AssignToUserGroupID))
            {
                sbTemp.Append("<select name=\"state\" id=\"state\">");
                sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
                if (cTask.State == "7")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
                sbTemp.Append("<option title=\"Archived\" value=\"8\" ");
                if (cTask.State == "8")
                {
                    sbTemp.Append("selected");
                }
                sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
                sbTemp.Append("</select>");
            }
        }
        else if (cTask.State == "7")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\" disabled>");
            sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
            if (cTask.State == "7")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            sbTemp.Append("</select>");
        }
        else if (cTask.State == "7")
        {
            sbTemp.Append("<select name=\"state\" id=\"state\" disabled>");
            sbTemp.Append("<option title=\"Archived\" value=\"8\" ");
            if (cTask.State == "8")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
            sbTemp.Append("</select>");
        }
        else if ((cTask.State == "1") | (cTask.State == "5") | (cTask.State == "6") | (cTask.State == "9"))
        {
            sbTemp.Append("<select name=\"state\" id=\"state\">");
            sbTemp.Append("<option title=\"Not Started\" value=\"1\" ");
            if (cTask.State == "1")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl not started") + "</option>");
            sbTemp.Append("<option title=\"Active\" value=\"2\" ");
            if (cTask.State == "2")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl active") + "</option>");
            sbTemp.Append("<option title=\"Awaiting Data\" value=\"3\" ");
            if (cTask.State == "3")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbTemp.Append("<option title=\"On Hold\" value=\"4\" ");
            if (cTask.State == "4")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            sbTemp.Append("<option title=\"Pending\" value=\"5\" ");
            if (cTask.State == "5")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl pending") + "</option>");
            sbTemp.Append("<option title=\"ReOpened\" value=\"6\" ");
            if (cTask.State == "6")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl reopened") + "</option>");
            sbTemp.Append("<option title=\"Completed\" value=\"7\" ");
            if (cTask.State == "7")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            sbTemp.Append("<option title=\"Archived\" value=\"8\" ");
            if (cTask.State == "8")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
            sbTemp.Append("<option title=\"Deleted\" value=\"9\" ");
            if (cTask.State == "9")
            {
                sbTemp.Append("selected");
            }
            sbTemp.Append(">" + this.MsgHelper.GetMessage("lbl deleted") + "</option>");
            sbTemp.Append("</select>");
        }
        ltrEditTaskSelect.Text = sbTemp.ToString();
        sbTemp = new StringBuilder();
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Start Date:</td>" + Environment.NewLine);
        sbTemp.Append(" <td>" + DisplayDateSelector("taskinfo", "start_date", "start_date", true, cTask.StartDate) + "</td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append(" <td class=\"label\">Due Date:</td>" + Environment.NewLine);
        sbTemp.Append(" <td>" + DisplayDateSelector("taskinfo", "due_date", "due_date", true, cTask.DueDate) + "</td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        if ((Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt32(EkEnumeration.TaskType.BlogPostComment)) | (Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt32(EkEnumeration.TaskType.TopicReply)))
        {
            sbAdvanced.Append(sbTemp.ToString());
        }
        else
        {
            ltrNotBlogTopic4.Text = sbTemp.ToString();
        }
        sbTemp = new StringBuilder();
        sbTemp.Append("<tr>" + Environment.NewLine);
        sbTemp.Append("\t<td class=\"label\">Created By:</td>" + Environment.NewLine);
        sbTemp.Append("\t<td align=\"left\">" + Environment.NewLine);
        if ((Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt32(EkEnumeration.TaskType.BlogPostComment)) | (Convert.ToInt64(cTask.TaskTypeID) == Convert.ToInt32(EkEnumeration.TaskType.TopicReply)) & (cTask.AssignedByUserID == Ektron.Cms.Common.EkConstants.BuiltIn.ToString()))
        {
            sbTemp.Append(Ektron.Cms.Common.EkFunctions.HtmlEncode(cTask.CommentDisplayName) + "&nbsp;" + EkFunctions.HtmlEncode("(" + cTask.CommentEmail + ")"));
        }
        else
        {
            sbTemp.Append(cTask.CreatedByUser);
        }
        sbTemp.Append("\t</td>" + Environment.NewLine);
        sbTemp.Append("</tr>" + Environment.NewLine);
        ltrNotBlogTopic5.Text = sbTemp.ToString();
        ltrMetaData.Text = sbAdvanced.ToString();

    }
    private void Display_ViewTasks()
    {
        ltrPageBasePath.Text = "tasks.aspx?action=ViewTasks&ty=" + actiontype + "&page={0}";
        ltrFormAction.Text = "tasks.aspx?Action=UpdateStatePurge" + closeOnFinish + callBackPage;
        pnlViewTasks.Visible = true;
        StringBuilder sbViewTasks = new StringBuilder();
        Collection cTemplates = new Collection();
        Ektron.Cms.Content.EkTask brObj = AppUI.EkTaskRef;
        PageRequestData pgrdata = new PageRequestData();
        string backButtonUrl = string.Empty;
        long selectedUser = 0;
        long selectedUserType = 0;
        long userValue = 0;
        HttpCookie httpCookie = Ektron.Cms.CommonApi.GetEcmCookie();
        if (IsAdmin)
        {
            TaskItemType = 8;
        }
        else
        {
            TaskItemType = 14;
        }
        pgrdata.CurrentPage = CurrentPage;
        pgrdata.PageSize = AppUI.RequestInformationRef.PagingSize;
        backButtonUrl = "callbackpage=tasks.aspx&parm1=action&value1=ViewTasks&parm2=ty&value2=" + actiontype + "&parm3=orderby&value3=" + Request.QueryString["orderby"];
        switch (actiontype)
        {
            case "all":
                TaskItemType = 12;
                sTitleBar = MsgHelper.GetMessage("lbl View All Open Tasks In The System");
                break;
            case "both":
                TaskItemType = 9;
                State = -1;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks Assigned By and To") + "&nbsp;" + httpCookie["userfullname"];
                break;
            case "to":
                TaskItemType = 3;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks Assigned To") + "&nbsp;" + httpCookie["userfullname"];
                break;
            case "touser":
                TaskItemType = 3;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks Assigned To Selected User") + "&nbsp;" + httpCookie["userfullname"];
                break;
            case "by":
                TaskItemType = 7;
                State = -1;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks Assigned By") + "&nbsp;" + httpCookie["userfullname"];
                break;
            case "created":
                TaskItemType = 18;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks Created By") + "&nbsp;" + httpCookie["userfullname"];
                break;
            case "notstarted":
                State = 1;
                sTitleBar = MsgHelper.GetMessage("lbl view task in nonstarted state");
                break;
            case "active":
                State = 2;
                sTitleBar = MsgHelper.GetMessage("lbl view task in active state");
                break;
            case "awaitingdata":
                State = 3;
                sTitleBar = MsgHelper.GetMessage("lbl view task in awaiting data state");
                break;
            case "onhold":
                State = 4;
                sTitleBar = MsgHelper.GetMessage("lbl View Tasks In Hold State");
                break;
            case "pending":
                State = 5;
                sTitleBar = MsgHelper.GetMessage("lbl view task in pending state");
                break;
            case "reopened":
                State = 6;
                sTitleBar = MsgHelper.GetMessage("lbl view task in reopened state");
                break;
            case "completed":
                State = 7;
                sTitleBar = MsgHelper.GetMessage("lbl view task in completed state");
                break;
            case "archived":
                State = 8;
                sTitleBar = MsgHelper.GetMessage("lbl view task in archived state");
                break;
            case "deleted":
                State = 9;
                sTitleBar = MsgHelper.GetMessage("lbl view task in deleted state");
                break;
        }

        if ((actiontype == "touser"))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["user"]))
            {
                selectedUser = Convert.ToInt64(Server.HtmlEncode(Request.QueryString["user"]));
            }
            if (selectedUser < 0)
            {
                selectedUser = currentUserID;
                selectedUserType = 1;
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["usertype"]))
                {
                    selectedUserType = Convert.ToInt64(Server.HtmlEncode(Request.QueryString["usertype"]));
                }
            }
            if (selectedUserType == 1)
            {
                cTasks = brObj.GetTasks(-1, selectedUser, State, TaskItemType, OrderBy, 0, ref pgrdata, "");
            }
            else
            {
                cTasks = brObj.GetTasks(-1, selectedUser, State, 19, OrderBy, 0, ref pgrdata, "");
            }
        }
        else if ((IsAdmin & (actiontype != "all" & actiontype != "both" & actiontype != "to" & actiontype != "by" & actiontype != "created" & actiontype != "touser")))
        {
            cTasks = brObj.GetTasks(-1, State, -1, TaskItemType, OrderBy, 0, ref pgrdata, "");
        }
        else
        {
            cTasks = objTask.GetTasks(-1, currentUserID, State, TaskItemType, OrderBy, 0, ref pgrdata, "");
        }
        if ((string.IsNullOrEmpty(ErrorString) & (actiontype == "to" | actiontype == "by" | actiontype == "touser")))
        {
            cTemplates = objTask.GetUsersForTask(currentUserID, -1);
        }
        if (ErrorString != string.Empty)
        {
            Response.Redirect("reterror.aspx?info=" + ErrorString, false);
        }

        sbViewTasks.Append("<table width=\"100%\" class=\"ektronGrid\">");
        sbViewTasks.Append("  <tr>");
        if (actiontype == "touser")
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"Assign to User\">" + MsgHelper.GetMessage("lbl Assign to User") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"selectusergroup\" id=\"selectusergroup\">");
            sbViewTasks.Append("              <optgroup label=\"Users\">");
            for (int i = 1; i < cTemplates.Count; i++)
            {
                Collection coll = (Collection)cTemplates[i];
                if (coll.Contains("DisplayUserName"))
                {
                    if (!string.IsNullOrEmpty(coll["UserID"].ToString()))
                    {
                        long c = Convert.ToInt64(coll["UserID"].ToString());
                        if (c > 0)
                        {
                            userValue = Convert.ToInt64(coll["UserID"]);
                        }
                    }
                    sbViewTasks.Append("<option value=\"" + userValue + ",1\"");
                    if ((userValue == selectedUser) & selectedUserType == 1)
                    {
                        sbViewTasks.Append(" selected");
                    }
                    sbViewTasks.Append(">" + coll["DisplayUserName"] + "</option>");
                }
                
            }
            sbViewTasks.Append("              </optgroup>");
            sbViewTasks.Append("              <optgroup label=\"Groups\">");
            for (int j = 1; j < cTemplates.Count; j++)
            {
                Collection coll = (Collection)cTemplates[j];
                if (coll.Contains("DisplayUserGroupName"))
                {
                    if(!string.IsNullOrEmpty(coll["UserGroupID"].ToString()))
                    {
                        long c = Convert.ToInt64(coll["UserGroupID"]);
                        if (c > 0)
                        {
                            userValue = Convert.ToInt64(coll["UserGroupID"]);
                        }
                    }
                    sbViewTasks.Append("<option value=\"" + userValue + ",2\"");
                    if ((userValue == selectedUser) & selectedUserType == 2)
                    {
                        sbViewTasks.Append(" selected");
                    }
                    sbViewTasks.Append(">" + coll["DisplayUserGroupName"] + "</option>");
                }
                
            }
            sbViewTasks.Append("              </optgroup>");
            sbViewTasks.Append("             </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a class=\"button buttonInline blueHover minHeight buttonViewTask\" type=\"button\" name=\"getusergrouptasks\" id=\"getusergrouptasks\" title=\"" + MsgHelper.GetMessage("btn view tasks") + "\" value=\"&nbsp;" + MsgHelper.GetMessage("btn view tasks") + "&nbsp;\" onclick=\"getTaskForUser()\">" + MsgHelper.GetMessage("generic view") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("    </table>");
            sbViewTasks.Append("  </td>");
        }
        else if (IsAdmin & (actiontype != "all" & actiontype != "both" & actiontype != "to" & actiontype != "by" & actiontype != "created"))
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"" + MsgHelper.GetMessage("lbl changestate") + "\">" + MsgHelper.GetMessage("lbl Change to state") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"state\" id=\"state\">");
            sbViewTasks.Append("                  <option title=\"Not Started\" value=\"1\"");
            if (actiontype == "notstarted")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl not started") + "</option>");
            sbViewTasks.Append("<option title=\"Active\" value=\"2\"");
            if (actiontype == "active")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl active") + "</option>");
            sbViewTasks.Append("<option title=\"Awaiting Data\" value=\"3\"");
            if (actiontype == "awaitingdata")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl awaiting data") + "</option>");
            sbViewTasks.Append("<option title=\"On Hold\" value=\"4\"");
            if (actiontype == "onhold")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl on hold") + "</option>");
            sbViewTasks.Append("<option title=\"Pending\" value=\"5\"");
            if (actiontype == "pending")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl pending") + "</option>");
            sbViewTasks.Append("<option title=\"Reopened\" value=\"6\"");
            if (actiontype == "reopened")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl reopened") + "</option>");
            sbViewTasks.Append("<option title=\"Completed\" value=\"7\"");
            if (actiontype == "completed")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl completed") + "</option>");
            sbViewTasks.Append("<option title=\"Archived\" value=\"8\"");
            if (actiontype == "archived")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl archived") + "</option>");
            sbViewTasks.Append("<option title=\"Deleted\" value=\"9\"");
            if (actiontype == "deleted")
            {
                sbViewTasks.Append(" selected");
            }
            sbViewTasks.Append(">" + this.MsgHelper.GetMessage("lbl deleted") + "</option>");
            sbViewTasks.Append("                 </select>");
            sbViewTasks.Append("             </td>");
            sbViewTasks.Append("             <td>");
            sbViewTasks.Append("                  <a title=\"" + MsgHelper.GetMessage("lbl Set State") + "\" class=\"button buttonInline blueHover minHeight buttonSet\" type=\"button\" name=\"setstate\" id=\"setstate\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(0)\" >" + MsgHelper.GetMessage("btn set") + " </a>");
            sbViewTasks.Append("             </td>");
            if (IsAdmin & actiontype == "deleted")
            {
                sbViewTasks.Append("         <td>");
                sbViewTasks.Append("              <a title=\"Purge\" class=\"button buttonInline redHover minHeight buttonDelete\" type=\"button\" name=\"purgeButton\" id=\"purgeButton\" value=\"&nbsp;" + MsgHelper.GetMessage("btn Purge") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(1)\" >" + MsgHelper.GetMessage("btn Purge") + "</a>");
                sbViewTasks.Append("         </td>");
            }
            sbViewTasks.Append("     </tr>");
            sbViewTasks.Append(" </table>");
            sbViewTasks.Append("</td>");
        }
        else if (IsAdmin & (actiontype != "all" & actiontype != "both" & actiontype != "created"))
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"Assign to User\">" + MsgHelper.GetMessage("lbl Assign to User") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"selectusergroup\" id=\"selectusergroup\">");
            sbViewTasks.Append("              <optgroup label=\"Users\">");
            for (int i = 1; i < cTemplates.Count; i++)
            {
                Collection coll = (Collection)cTemplates[i];
                if (coll.Contains("DisplayUserName"))
                {
                    if (!string.IsNullOrEmpty(coll["UserID"].ToString()))
                    {
                        long c = Convert.ToInt64(coll["UserID"]);
                        if (c > 0)
                        {
                            userValue = c;
                        }
                    }
                    sbViewTasks.Append("<option value=\"" + userValue + ",1\"");
                    sbViewTasks.Append(">" + coll["DisplayUserName"] + "</option>");
                }
            }
            sbViewTasks.Append("              </optgroup>");
            sbViewTasks.Append("              <optgroup label=\"Groups\">");
            for (int j = 1; j < cTemplates.Count; j++)
            {
                Collection coll = (Collection)cTemplates[j];
                if (coll.Contains("DisplayUserGroupName"))
                {
                    if(!string.IsNullOrEmpty(coll["UserGroupID"].ToString()))
                    {
                        long c = Convert.ToInt64(coll["UserGroupID"]);
                        if (c > 0)
                        {
                            userValue = c;
                        }
                    }
                    sbViewTasks.Append("<option value=\"" + userValue + ",2\"");
                    sbViewTasks.Append(">" + coll["DisplayUserGroupName"] + "</option>");
                }
            }
            sbViewTasks.Append("              </optgroup>");
            sbViewTasks.Append("             </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a title=\"Set User Group\" class=\"button buttonInline blueHover minHeight buttonSet\" type=\"button\" name=\"setusergroup\" id=\"setusergroup\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(2)\" >" + MsgHelper.GetMessage("btn set") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("    </table>");
            sbViewTasks.Append("  </td>");
        }
        else if (actiontype == "notstarted" | actiontype == "active")
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"" + MsgHelper.GetMessage("lbl changestate") + "\">" + MsgHelper.GetMessage("lbl Change To State") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"state\" id=\"state\">");
            sbViewTasks.Append("                  <option title=\"Awaiting Data\" value=\"3\" selected>Awaiting Data</option>");
            sbViewTasks.Append("                  <option title=\"On Hold\" value=\"4\">On Hold</option>");
            sbViewTasks.Append("              </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a title=\"" + MsgHelper.GetMessage("lbl Set State") + "\" type=\"button\" class=\"button buttonInline blueHover minHeight buttonSet\" name=\"setstate\" id=\"setstate\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(0)\" >" + MsgHelper.GetMessage("btn set") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("  </table>");
            sbViewTasks.Append("</td>");
        }
        else if (actiontype == "awaitingdata")
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"" + MsgHelper.GetMessage("lbl changestate") + "\">" + MsgHelper.GetMessage("lbl Change To State") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"state\" id=\"state\">");
            sbViewTasks.Append("                  <option title=\"Active\" value=\"2\" selected>Active</option>");
            sbViewTasks.Append("                  <option title=\"On Hold\" value=\"4\">On Hold</option>");
            sbViewTasks.Append("              </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a title=\"" + MsgHelper.GetMessage("lbl Set State") + "\" type=\"button\" class=\"button buttonInline blueHover minHeight buttonSet\" name=\"setstate\" id=\"setstate\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(0)\" >" + MsgHelper.GetMessage("btn set") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("  </table>");
            sbViewTasks.Append("</td>");
        }
        else if (actiontype == "onhold")
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table id=\"Table35\">");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"" + MsgHelper.GetMessage("lbl changestate") + "\">" + MsgHelper.GetMessage("lbl Change To State") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"state\" id=\"state\">");
            sbViewTasks.Append("                  <option title=\"Active\" value=\"2\" selected>Active</option>");
            sbViewTasks.Append("                  <option title=\"Awaiting Data\" value=\"3\">Awaiting Data</option>");
            sbViewTasks.Append("              </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a title=\""+MsgHelper.GetMessage("lbl Set State")+"\" type=\"button\" class=\"button buttonInline blueHover minHeight buttonSet\" name=\"setstate\" id=\"setstate\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(0)\" >" + MsgHelper.GetMessage("btn set") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("  </table>");
            sbViewTasks.Append("</td>");
        }
        else if (actiontype == "completed")
        {
            sbViewTasks.Append("<td>");
            sbViewTasks.Append("  <table>");
            sbViewTasks.Append("      <tr>");
            sbViewTasks.Append("          <td class=\"label\" title=\"" + MsgHelper.GetMessage("lbl changestate") + "\">" + MsgHelper.GetMessage("lbl Change To State") + "</td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <select name=\"state\" id=\"state\">");
            sbViewTasks.Append("                  <option title=\"Archived\" value=\"8\" selected>Archived</option>");
            sbViewTasks.Append("              </select>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("          <td>");
            sbViewTasks.Append("              <a title=\"" + MsgHelper.GetMessage("lbl Set State") + "\" type=\"button\" class=\"button buttonInline blueHover minHeight buttonSet\" name=\"setstate\" id=\"setstate\" value=\"&nbsp;" + MsgHelper.GetMessage("btn set") + "&nbsp;\" onclick=\"setTaskStateForSelTasks(0)\" >" + MsgHelper.GetMessage("btn set") + "</a>");
            sbViewTasks.Append("          </td>");
            sbViewTasks.Append("      </tr>");
            sbViewTasks.Append("  </table>");
            sbViewTasks.Append("</td>");
        }

        sbViewTasks.Append("      <td class=\"label\" align=\"right\" title=\"" + MsgHelper.GetMessage("lbl show tasktype") + "\">" + MsgHelper.GetMessage("lbl show task type"));
        sbViewTasks.Append("          <select name=\"show_task_type\" id=\"show_task_type\" onchange=\"RefreshTasksWithTaskType();\">");
        sbViewTasks.Append("          </select>");
        sbViewTasks.Append("      </td>");
        sbViewTasks.Append("  </tr>");
        sbViewTasks.Append("</table>");
        sbViewTasks.Append("<div class=\"\">");
        if (cTasks.Count == 0)
        {
            this.uxPaging.Visible = false;
            sbViewTasks.Append("<p title=\"Currently there is no data to report\">" + MsgHelper.GetMessage("msg no data report") + "</p>");
        }
        else
        {
            if (pgrdata.TotalPages > 1)
            {
                this.uxPaging.ClientFunction = "GoToPage(this); return false;";
                this.uxPaging.Visible = true;
                this.uxPaging.TotalPages = pgrdata.TotalPages;
                this.uxPaging.CurrentPageIndex = pgrdata.CurrentPage - 1;
            }
            else
            {
                this.uxPaging.Visible = false;
            }
            sbViewTasks.Append("  <table class=\"ektronGrid\" style=\"width:100%;\">");
            sbViewTasks.Append("      <tr class=\"title-header\">");
            if (IsAdmin & (actiontype != "all" & actiontype != "both" & actiontype != "touser"))
            {
                sbViewTasks.Append("<td width=\"1\">");
                sbViewTasks.Append("  <input title=\"Check All\" type=\"checkbox\" name=\"all\" onclick=\"checkAll(document.forms.viewtasks.all.checked);\" id=\"Checkbox3\" />");
                sbViewTasks.Append("</td>");
            }
            else if (actiontype == "notstarted" | actiontype == "active" | actiontype == "completed" | actiontype == "awaitingdata" | actiontype == "onhold")
            {
                sbViewTasks.Append("<td width=\"1\">");
                sbViewTasks.Append("  <input title=\"Check All\" type=\"checkbox\" name=\"all\" onclick=\"checkAll(document.forms.viewtasks.all.checked);\" id=\"Checkbox6\" />");
                sbViewTasks.Append("</td>");
            }
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=title&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("generic Title") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=content_id&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl CID") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=state&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl state") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=priority&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl priority") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=duedate&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl Due Date") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=assignedto&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl Assigned To") + "</a></td>");
            sbViewTasks.Append("<td><a href=\"tasks.aspx?action=ViewTasks&orderby=assignedby&ty=" + actiontype + "&user=" + Request.QueryString["user"] + "&usertype=" + Request.QueryString["usertype"] + "\" alt=\"" + MsgHelper.GetMessage("click to sort msg") + "\" title=\"" + MsgHelper.GetMessage("click to sort msg") + "\">" + MsgHelper.GetMessage("lbl Assigned By") + "</a></td>");
            sbViewTasks.Append("<td>" + MsgHelper.GetMessage("lbl Last Added comments") + "</td>");
            sbViewTasks.Append("<td>" + MsgHelper.GetMessage("lbl Create Date") + "</td>");
            sbViewTasks.Append("</tr>");
            bool bHasTask = false;
            for (int counter = 1; counter < cTasks.Count + 1; counter++)
            {
                EkTask cTask = cTasks.get_Item(counter);
                if (!(cTask.TaskTypeID == Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment) | cTask.TaskTypeID == Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply)))
                {
                    bHasTask = true;

                    sbViewTasks.Append("<tr id=\"task_" + cTask.TaskID + "_" + counter + "_");
                    if (cTask.TaskTypeID <= 0)
                    {
                        sbViewTasks.Append("NotS\">");
                    }
                    else
                    {
                        sbViewTasks.Append(cTask.TaskTypeID + "\" >");
                    }
                    sbViewTasks.Append("<script type=\"text/javascript\">");
                    sbViewTasks.Append("AddShownTaskID('task_" + cTask.TaskID + "_" + counter + "_");
                    if (cTask.TaskTypeID <= 0)
                    {
                        sbViewTasks.Append("NotS')");
                    }
                    else
                    {
                        sbViewTasks.Append(cTask.TaskTypeID + "')");
                    }
                    sbViewTasks.Append("</script>");
                    if (IsAdmin & (actiontype != "all" & actiontype != "both" & actiontype != "touser"))
                    {
                        sbViewTasks.Append("<td nowrap width=\"1\">");
                        sbViewTasks.Append("<input title=\"Task Type\" type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"" + cTask.TaskID + "\"  value=\"" + cTask.TaskID + "\" id=\"_" + cTask.TaskID + "_" + counter + "_");
                        if (cTask.TaskTypeID <= 0)
                        {
                            sbViewTasks.Append("NotS\">");
                        }
                        else
                        {
                            sbViewTasks.Append(cTask.TaskTypeID + "\" >");
                        }
                        sbViewTasks.Append("</td>");
                    }
                    else if (actiontype == "notstarted" | actiontype == "active" | actiontype == "completed" | actiontype == "awaitingdata" | actiontype == "onhold")
                    {
                        sbViewTasks.Append("<td nowrap width=\"1\">");
                        sbViewTasks.Append("<input title=\"Task ID\" type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"" + cTask.TaskID + "\"  value=\"" + cTask.TaskID + "\" id=\"_" + cTask.TaskID + "_" + counter + "_");
                        if (cTask.TaskTypeID <= 0)
                        {
                            sbViewTasks.Append("NotS\">");
                        }
                        else
                        {
                            sbViewTasks.Append(cTask.TaskTypeID + "\" >");
                        }
                        sbViewTasks.Append("</td>");
                    }
                    sbViewTasks.Append("<td nowrap=\"nowrap\">");
                    sbViewTasks.Append("  <a href=\"tasks.aspx?action=ViewTask&tid=" + cTask.TaskID + "&ty=" + actiontype + "&" + backButtonUrl + "\">" + cTask.TaskTitle + "</a>");
                    sbViewTasks.Append("</td>");
                    if (cTask.ContentID != -1)
                    {
                        if (Convert.ToInt32(cTask.ContentType) == 2 | Convert.ToInt32(cTask.ContentType) == 4)
                        {
                            sbViewTasks.Append("<td nowrap=\"nowrap\"><a href=\"cmsform.aspx?action=ViewForm&form_id=" + cTask.ContentID + "&LangType=" + cTask.LanguageID + "&callerpage=tasks.aspx&origurl=" + Server.UrlEncode(Request.ServerVariables["QUERY_STRING"]) + "\" title=\"" + MsgHelper.GetMessage("generic View") + " " + cTask.ContentTitle.Replace("'", "`") + "\">" + cTask.ContentID + "</a></td>");
                        }
                        else
                        {
                            sbViewTasks.Append("<td nowrap=\"nowrap\"><a href=\"content.aspx?action=View&id=" + cTask.ContentID + "&LangType=" + cTask.LanguageID + "&callerpage=tasks.aspx&origurl=" + Server.UrlEncode(Request.ServerVariables["QUERY_STRING"]) + "\" title=\"" + MsgHelper.GetMessage("generic View") + " " + cTask.ContentTitle.Replace("'", "`") + "\">" + cTask.ContentID + "</a></td>");
                        }
                    }
                    else
                    {
                        sbViewTasks.Append("<td>&nbsp;</td>");
                    }                    
                    switch (cTask.State)
                    {
                        case "1":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl not started")+"</td>");
                            break;
                        case "2":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl active")+"</td>");
                            break;
                        case "3":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl awaiting data")+"</td>");
                            break;
                        case "4":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl on hold")+"</td>");
                            break;
                        case "5":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl pending")+"</td>");
                            break;
                        case "6":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl reopened")+"</td>");
                            break;
                        case "7":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl completed")+"</td>");
                            break;
                        case "8":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl archived")+"</td>");
                            break;
                        case "9":
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl deleted")+"</td>");
                            break;
                    }
                    switch (Convert.ToInt32(cTask.Priority))
                    {
                        case 1:
                            sbViewTasks.Append("<td nowrap=\"true\">" + this.MsgHelper.GetMessage("lbl low") + "</td>");
                            break;
                        case 2:
                            sbViewTasks.Append("<td nowrap=\"true\">" + this.MsgHelper.GetMessage("lbl normal") + "</td>");
                            break;
                        case 3:
                            sbViewTasks.Append("<td nowrap=\"true\">"+this.MsgHelper.GetMessage("lbl high")+"</td>");
                            break;
                        case 0:
                            sbViewTasks.Append("<td nowrap=\"true\">[" + this.MsgHelper.GetMessage("dd not specified") + "]</td>");
                            break;
                    }
                    if ((!string.IsNullOrEmpty(cTask.DueDate)))
                    {
                        if ((Convert.ToDateTime(cTask.DueDate) < DateTime.Today))
                        {
                            sbViewTasks.Append("<td nowrap=\"true\" class=\"important\">" + AppUI.GetInternationalDateOnly(cTask.DueDate) + "</td>");
                        }
                        else
                        {
                            sbViewTasks.Append("<td nowrap=\"true\">" + AppUI.GetInternationalDateOnly(cTask.DueDate) + "</td>");
                        }
                    }
                    else
                    {
                        sbViewTasks.Append("<td nowrap=\"true\">[" + this.MsgHelper.GetMessage("dd not specified") + "]</td>");
                    }
                    if (cTask.AssignToUserGroupID == 0)
                    {
                        if (cTask.ContentID != -1)
                        {
                            sbViewTasks.Append("<td nowrap=\"true\">All Authors of (" + cTask.ContentID + ")</td>");
                        }
                        else
                        {
                            sbViewTasks.Append("<td nowrap=\"true\">All Authors</td>");
                        }
                    }
                    else if (cTask.AssignedToUser != "")
                    {
                        sbViewTasks.Append("<td nowrap=\"nowrap\">");
                        sbViewTasks.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" alt=\"\" align=\"absbottom\"/>");
                        sbViewTasks.Append(m_refEmail.MakeUserTaskEmailLink(cTask, false));
                        sbViewTasks.Append("</td>");
                    }
                    else if (cTask.AssignToUserGroupID != 0)
                    {
                        sbViewTasks.Append("<td nowrap=\"nowrap\">");
                        sbViewTasks.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" alt=\"\" align=\"absbottom\"/>");
                        sbViewTasks.Append(m_refEmail.MakeUserGroupTaskEmailLink(cTask));
                        sbViewTasks.Append("</td>");
                    }
                    else
                    {
                        sbViewTasks.Append("<td>&nbsp;</td>");
                    }
                    sbViewTasks.Append("<td nowrap=\"nowrap\">");
                    sbViewTasks.Append(m_refEmail.MakeByUserTaskEmailLink(cTask, false)); 
                    sbViewTasks.Append("</td>");
                    if (cTask.LastComment == "")
                    {
                        sbViewTasks.Append("<td >[" + this.MsgHelper.GetMessage("dd not specified") + "]</td>");
                    }
                    else
                    {
                        sbViewTasks.Append("<td ><div class=\"comment-block\">" + cTask.LastComment + "</div></td>");
                    }
                    sbViewTasks.Append("<td>" + AppUI.GetInternationalDateOnly(cTask.DateCreated) + "</td>");
                    sbViewTasks.Append("</tr>");
                }
            }
            sbViewTasks.Append("</table>");
            if (!bHasTask)
            {
                sbViewTasks.Append("<p>" + MsgHelper.GetMessage("msg no data report") + "</p>");
            }
        }
        sbViewTasks.Append("<input type=\"hidden\" name=\"taskids\" value=\"\" id=\"taskids\"/>");
        sbViewTasks.Append("<input type=\"hidden\" name=\"purge\" value=\"\" id=\"purge\"/>");
        sbViewTasks.Append("<input type=\"hidden\" name=\"actiontype\" value=\"" + actiontype + "\" id=\"actiontype\"/>");
        sbViewTasks.Append("<input type=\"hidden\" name=\"rptHtml\" value=\"\" id=\"rptHtml\"/>");
        sbViewTasks.Append("<input type=\"hidden\" name=\"rptTitle\" value=\"\" id=\"rptTitle\"/>");
        sbViewTasks.Append("</div>");
        ltrViewTasks.Text = sbViewTasks.ToString();
    }
    private void Display_ViewTask()
    {
        pnlViewTask.Visible = true;
        if (!string.IsNullOrEmpty(Request.QueryString["ty"]))
        {
            fromType = Request.QueryString["ty"];
        }
        if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
        {
            TaskID = Convert.ToInt64(Request.QueryString["tid"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["fromViewContent"]))
        {
            fromViewContent = Request.QueryString["fromViewContent"];
        }
        objTask = AppUI.EkTaskRef;
        objTask = objTask.GetTaskByID(TaskID);
        callBackPage = BuildCallBackParms("&");
        StringBuilder sbToolBar = new StringBuilder();

		string backpage = "";
		if (Request.QueryString["callbackpage"] != null && Request.QueryString["callbackpage"].ToString().ToLower() == "tasks.aspx")
		{
			backpage = "tasks.aspx?action=ViewTasks&orderby=" + OrderBy + "&ty=" + EkFunctions.HtmlEncode(Request.QueryString["ty"]);
		}
		else
		{
			backpage = "javascript:history.back();";
		}

		sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", backpage, MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

		bool primaryCssApplied = false;

        if (fromViewContent == "" & fromType != "created" & (fromType != "touser") | (objTask.TaskTypeID == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment)))
        {
			sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "tasks.aspx?action=EditTask&tid=" + TaskID + callBackPage, "Edit Task", MsgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;
        }
        if (Convert.ToBoolean(canI["CanIDeleteTask"]) & fromViewContent != null & fromType != "created" & fromType != "touser")
        {
            string frmContentPage = (!string.IsNullOrEmpty(Request.QueryString["fromViewContent"]) ? "&fromViewContent=1" : "");
            string frmContentId = (!string.IsNullOrEmpty(Request.QueryString["contentid"]) ? "&contentid=" + Request.QueryString["contentid"].ToString() : "");
            string frmFolderId = (!string.IsNullOrEmpty(Request.QueryString["folderid"]) ? "&folderid=" + Request.QueryString["folderid"].ToString() : "");
			sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "tasks.aspx?action=DeleteTask" + closeOnFinish + "&tid=" + TaskID + "&ty=" + actiontype + callBackPage + frmContentPage + frmContentId + frmFolderId, "Delete Task", MsgHelper.GetMessage("btn delete"), "onclick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;
        }
        
		sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/commentAdd.png", "javascript:openComment('');", "Add Comment", MsgHelper.GetMessage("btn comment add"), "", StyleHelper.AddCommentButtonCssClass, !primaryCssApplied));
		
		primaryCssApplied = true;

		if (fromType != "created" & fromType != "touser")
        {
            sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/history.png", "#", MsgHelper.GetMessage("alt history button text"), MsgHelper.GetMessage("lbl generic history"), "onclick=\"openTaskHistory('taskhistory.aspx?action=ID&tid=" + objTask.TaskID + "&title=" + objTask.TaskTitle.Replace("'", "&apos;").Replace("&#39;", "&apos;") + "');return false;\"", StyleHelper.HistoryButtonCssClass));
        }
		sbToolBar.Append(StyleHelper.ActionBarDivider);
        sbToolBar.Append("<td>" + m_refStyle.GetHelpButton(action, "") + "</td>");
        ltrViewTaskToolBar.Text = sbToolBar.ToString();
        StringBuilder sbAssignedTo = new StringBuilder();
        if (objTask.AssignToUserGroupID == 0)
        {
            sbAssignedTo.Append("All Authors");
        }
        else if (objTask.AssignedToUser != string.Empty)
        {
            sbAssignedTo.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" alt=\"\" align=\"absbottom\"/>");
            sbAssignedTo.Append(m_refEmail.MakeUserTaskEmailLink(objTask, true));
        }
        else if (objTask.AssignedToUserGroup != string.Empty)
        {
            sbAssignedTo.Append("<img src=\"" + AppPath + "images/UI/Icons/users.png\" alt=\"\" align=\"absbottom\"/>");
            sbAssignedTo.Append(m_refEmail.MakeUserGroupTaskEmailLink(objTask));
        }
        ltrAssignedTo.Text = sbAssignedTo.ToString();

        StringBuilder sbLinks = new StringBuilder();
        if (objTask.ContentID > -1)
        {
            sbLinks.Append("<tr>");
            sbLinks.Append("<td class=\"label\" title=\"Content\">" + MsgHelper.GetMessage("content text") + ":</td>");
            if (objTask.ContentType == EkEnumeration.CMSContentType.Forms | objTask.ContentType == EkEnumeration.CMSContentType.Archive_Forms)
            {
                sbLinks.Append("<td>");
                sbLinks.Append("(" + objTask.ContentID + ")&nbsp;<a href=\"cmsform.aspx?action=ViewForm&form_id=" + objTask.ContentID + "&LangType=" + objTask.LanguageID + "\" title=\"" + MsgHelper.GetMessage("generic View") + " " + objTask.ContentTitle.Replace("'", "`") + "\">" + objTask.ContentTitle + "</a>");
                sbLinks.Append("</td>");
            }
            else
            {
                sbLinks.Append("<td>");
                sbLinks.Append("(" + objTask.ContentID + ")&nbsp;<a href=\"content.aspx?action=View&id=" + objTask.ContentID + "&LangType=" + objTask.LanguageID + "\" title=\"" + MsgHelper.GetMessage("generic View") + " " + objTask.ContentTitle.Replace("'", "`") + "\">" + objTask.ContentTitle + "</a>");
                sbLinks.Append("</td>");
            }

            sbLinks.Append("</tr>");
            ltrViewTaskLinks.Text = sbLinks.ToString();
        }
        switch (objTask.Priority)
        {
            case EkEnumeration.TaskPriority.High:
                ltrViewTaskPriority.Text = "<td>"+this.MsgHelper.GetMessage("lbl high")+"</td>";
                break;
            case EkEnumeration.TaskPriority.Low:
                ltrViewTaskPriority.Text = "<td>" + this.MsgHelper.GetMessage("lbl low") + "</td>";
                break;
            case EkEnumeration.TaskPriority.Normal:
                ltrViewTaskPriority.Text = "<td>" + this.MsgHelper.GetMessage("lbl normal") + "</td>";
                break;
            default:
                ltrViewTaskPriority.Text = "<td>" + this.MsgHelper.GetMessage("dd not specified") + "</td>";
                break;
        }
        switch (objTask.State)
        {
            case "1":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl not started");
                break;
            case "2":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl active");
                break;
            case "3":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl awaiting data");
                break;
            case "4":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl on hold");
                break;
            case "5":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl pending");
                break;
            case "6":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl reopened");
                break;
            case "7":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl completed");
                break;
            case "8":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl archived");
                break;
            case "9":
                ltrViewTaskState.Text = this.MsgHelper.GetMessage("lbl deleted");
                break;

        }
        if (objTask.Description != string.Empty)
        {
            ltrViewTaskDescription.Text = objTask.Description;
        }
        else
        {
            ltrViewTaskDescription.Text = "[" + this.MsgHelper.GetMessage("dd not specified") + "]";
        }

        switch (OrderBy)
        {
            case "date_created asc":
                ltrViewTaskComments.Text = "<td><a href=\"javascript: DoSort('date_created desc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by DateTime") + "\">" + MsgHelper.GetMessage("lbl date/time") + "</a></td><td width=\"20%\"><a href=\"javascript: DoSort('last_name asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by LastName") + "\">" + MsgHelper.GetMessage("lbl added by") + "</a></td><td width=\"65%\" style=\"color: #2e6e9e;\">" + MsgHelper.GetMessage("comments label") + "</td>";
                break;
            case "date_created desc":
                ltrViewTaskComments.Text = "<td><a href=\"javascript: DoSort('date_created asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by DateTime") + "\">" + MsgHelper.GetMessage("lbl date/time") + "</a></td><td width=\"20%\"><a href=\"javascript: DoSort('last_name asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by LastName") + "\">" + MsgHelper.GetMessage("lbl added by") + "</a></td><td width=\"65%\" style=\"color: #2e6e9e;\">" + MsgHelper.GetMessage("comments label") + "</td>";
                break;
            case "last_name ascc":
                ltrViewTaskComments.Text = "<td><a href=\"javascript: DoSort('date_created asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by DateTime") + "\">" + MsgHelper.GetMessage("lbl date/time") + "</a></td><td width=\"20%\"><a href=\"javascript: DoSort('last_name desc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by LastName") + "\">" + MsgHelper.GetMessage("lbl added by") + "</a></td><td width=\"65%\" style=\"color: #2e6e9e;\">" + MsgHelper.GetMessage("comments label") + "</td>";
                break;
            case "last_name desc":
                ltrViewTaskComments.Text = "<td><a href=\"javascript: DoSort('date_created asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by DateTime") + "\">" + MsgHelper.GetMessage("lbl date/time") + "</a></td><td width=\"20%\"><a href=\"javascript: DoSort('last_name asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by LastName") + "\">" + MsgHelper.GetMessage("lbl added by") + "</a></td><td width=\"65%\" style=\"color: #2e6e9e;\">" + MsgHelper.GetMessage("comments label") + "</td>";
                break;
            default:
                ltrViewTaskComments.Text = "<td><a href=\"javascript: DoSort('date_created asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by DateTime") + "\">" + MsgHelper.GetMessage("lbl date/time") + "</a></td><td width=\"20%\"><a href=\"javascript: DoSort('last_name asc');\" title=\"" + MsgHelper.GetMessage("lbl Sort by LastName") + "\">" + MsgHelper.GetMessage("lbl added by") + "</a></td><td width=\"65%\" style=\"color: #2e6e9e;\">" + MsgHelper.GetMessage("comments label") + "</td>";
                break;
        }
    }
    private void Display_DeleteAllTasks()
    {
        ValidateCanDeleteTask();
        pnlDeleteAllTasks.Visible = true;
        string taskIDs = string.Empty;
        objTask = AppUI.EkTaskRef;
        if ((actiontype == "all"))
        {
            TaskItemType = 12;
        }
        else if ((actiontype == "both"))
        {
            TaskItemType = 9;
        }
        else if ((actiontype == "to"))
        {
            TaskItemType = 3;
        }
        else if ((actiontype == "by"))
        {
            TaskItemType = 7;
        }
        PageRequestData pgdata = new PageRequestData();
        cTasks = objTask.GetTasks(-1, currentUserID, -1, TaskItemType, OrderBy, -1, ref pgdata, "");
        HttpCookie cookie = Ektron.Cms.CommonApi.GetEcmCookie();
        if ((actiontype == "all"))
        {
            sTitleBar = "Delete All Tasks In The System";
        }
        else if ((actiontype == "to"))
        {
            sTitleBar = "Delete Tasks Assigned To " + cookie["userfullname"];
        }
        else if ((actiontype == "by"))
        {
            sTitleBar = "Delete Tasks Assigned By " + cookie["userfullname"];
        }
        else if ((actiontype == "both"))
        {
            sTitleBar = "Delete Tasks Assigned By and To " + cookie["userfullname"];
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 1; i < cTasks.Count + 1; i++)
        {
            EkTask cTask = cTasks.get_Item(i);
            taskIDs = taskIDs + cTask.TaskID + ",";
            sb.Append("<tr id=\"task_" + cTask.TaskID + "_" + i + "_");
            if (cTask.TaskTypeID <= 0)
            {
                sb.Append("NotS\">");
            }
            else
            {
                sb.Append(cTask.TaskTypeID + "\">");
            }
            sb.Append(" <script type=\"text/javascript\">");
			sb.Append(" AddShownTaskID('task_" + cTask.TaskID + "_" + i + "_");
            if (cTask.TaskTypeID <= 0)
            {
                sb.Append("NotS');");
            }
            else
            {
                sb.Append(cTask.TaskTypeID + "');");
            }
            sb.Append("</script>");
            sb.Append(" <td nowrap=\"nowrap\" width=\"1\">");
            sb.Append("     <input title=\"Task ID\" type=\"checkbox\" onclick=\"checkAllFalse();\" name=\"id_" + cTask.TaskID +"\" id=\"_" + cTask.TaskID + "_" + i + "_");
            if (cTask.TaskTypeID <= 0)
            {
                sb.Append("NotS\"/>");
            }
            else
            {
                sb.Append(cTask.TaskTypeID + "\"/>");
            }
            sb.Append(" </td>");
            sb.Append("<td><a href=\"tasks.aspx?action=ViewTask&tid=" + cTask.TaskID + "\" title=\"" + cTask.TaskTitle + "\">" + cTask.TaskTitle + "</a></td>");
            sb.Append("<td title=\"" + cTask.TaskID + "\">" + cTask.TaskID + "</td>");
            if ((actiontype == "by") | (actiontype == "all") | (actiontype == "both"))
            {
                if (cTask.AssignToUserGroupID == 0)
                {
                    sb.Append("<td>All Authors</td>");
                }
                else if (cTask.AssignedToUser != "")
                {
                    sb.Append("<td>");
					sb.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" alt=\"\" align=\"absbottom\"/>");
                    sb.Append(m_refEmail.MakeUserTaskEmailLink(cTask, false));
					sb.Append("</td>");
                }
                else if (cTask.AssignedToUserGroup != "")
                {
                    sb.Append("<td>");
                    sb.Append("<img src=\"" + AppPath + "images/UI/Icons/users.png\" alt=\"\" align=\"absbottom\"/>");
                    sb.Append(m_refEmail.MakeUserGroupTaskEmailLink(cTask));
                    sb.Append("</td>");
                }
            }
            if ((actiontype == "to") | (actiontype == "all") | (actiontype == "both"))
            {
                sb.Append("<td>");
			    sb.Append(m_refEmail.MakeByUserTaskEmailLink(cTask, false));
				sb.Append("</td>");
            }
            if ((!string.IsNullOrEmpty(cTask.DueDate)))
            {
                if ((Convert.ToDateTime(cTask.DueDate) < DateTime.Today))
                {
                    sb.Append("<td class=\"important\">" + AppUI.GetInternationalDateOnly(cTask.DueDate) + "</td>");
                }
                else
                {
                    sb.Append("<td>" + AppUI.GetInternationalDateOnly(cTask.DueDate) + "</td>");
                }
            }
            else
            {
                sb.Append("<td>[" + this.MsgHelper.GetMessage("dd not specified") + "]</td>");
            }
            switch (cTask.State)
            { 
                case "1":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl not started")+"</td>");
                    break;
                case "2":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl active")+"</td>");
                    break;
                case "3":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl awaiting data")+"</td>");
                    break;
                case "4":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl on hold")+"</td>");
                    break;
                case "5":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl pending")+"</td>");
                    break;
                case "6":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl reopened")+"</td>");
                    break;
                case "7":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl completed")+"</td>");
                    break;
                case "8":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl archived")+"</td>");
                    break;
                case "9":
                    sb.Append("<td>"+this.MsgHelper.GetMessage("lbl deleted")+"</td>");
                    break;
            }

            switch (cTask.Priority)
            {
                case EkEnumeration.TaskPriority.Low:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl low") + "</td>");
                    break;
                case EkEnumeration.TaskPriority.Normal:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl normal") + "</td>");
                    break;
                case EkEnumeration.TaskPriority.High:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl high") + "</td>");
                    break;
            }
            sb.Append("</tr>");
        }
        if (taskIDs != string.Empty)
        {
            taskIDs = taskIDs.Remove(taskIDs.Length - 1, 1);
        }
        ltrDeleteAllTasks.Text = sb.ToString();
        ltrTaskIds.Text = "<input type=\"hidden\" name=\"taskids\" value=\"" + taskIDs + "\" id=\"taskids\"/>";
    }
    private void Display_ViewContentTask()
    {
        pnlViewContentTask.Visible = true;
        objTask = AppUI.EkTaskRef;
        TaskItemType = 1;
        TaskID = ContentId;
        if (TaskID <= 0)
        {
            Response.Redirect("reterror.aspx?info=");
        }
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            languageID = Convert.ToInt32(Request.QueryString["LangType"]);
        }
        objTask.LanguageID = languageID;
        PageRequestData prdata = new PageRequestData();
        objTask.GetTasks(TaskID, -1, -1, TaskItemType, OrderBy, languageID, ref prdata, "");
        callBackPage = BuildCallBackParms("&");
        if (actiontype == "both")
        {
            sTitleBar =this.MsgHelper.GetMessage("lbl View All Tasks for content")+" (" + TaskID + ")";
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 1; i < cTasks.Count + 1; i++)
        {
            objTask = cTasks.get_Item(i);
            sb.Append("<tr>");
            sb.Append(" <td><a href=\"tasks.aspx?action=ViewTask&tid=" + objTask.TaskID + "&fromViewContent=1&ty=" + actiontype + "&LangType=" + objTask.ContentLanguage + callBackPage + "\">" + objTask.TaskTitle + "</a></td>");
            sb.Append("<td title=\"" + objTask.TaskID + "\">" + objTask.TaskID + "</td>");
                     
            switch (objTask.State)
            {
                case "1":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl not started") + "</td>");
                    break;
                case "2":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl active") + "</td>");
                    break;
                case "3":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl awaiting data") + "</td>");
                    break;
                case "4":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl on hold") + "</td>");
                    break;
                case "5":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl pending") + "</td>");
                    break;
                case "6":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl reopened") + "</td>");
                    break;
                case "7":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl completed") + "</td>");
                    break;
                case "8":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl archived") + "</td>");
                    break;
                case "9":
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl deleted") + "</td>");
                    break;
            }            
            switch (objTask.Priority)
            {
                case EkEnumeration.TaskPriority.Low:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl low") + "</td>");
                    break;
                case EkEnumeration.TaskPriority.Normal:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl normal") + "</td>");
                    break;
                case EkEnumeration.TaskPriority.High:
                    sb.Append("<td>" + this.MsgHelper.GetMessage("lbl high") + "</td>");
                    break;
            }

            if ((!string.IsNullOrEmpty(objTask.DueDate)))
            {
                if ((Convert.ToDateTime(objTask.DueDate) < DateTime.Today))
                {
                    sb.Append("<td class=\"important\">" + AppUI.GetInternationalDateOnly(objTask.DueDate) + "</td>");
                }
                else
                {
                    sb.Append("<td>" + AppUI.GetInternationalDateOnly(objTask.DueDate) + "</td>");
                }
            }
            else
            {
                sb.Append("<td>[" + this.MsgHelper.GetMessage("dd not specified") + "]</td>");
            }
            if ((actiontype == "by") | (actiontype == "all") | (actiontype == "both"))
            {
                if (objTask.AssignToUserGroupID == 0)
                {
                    sb.Append("<td>All Authors of (" + objTask.ContentID + ")</td>");
                }
                else if (objTask.AssignedToUser != "")
                {
                    sb.Append("<td>");
                    sb.Append("<img src=\"" + AppPath + "images/UI/Icons/user.png\" alt=\"\" align=\"absbottom\"/>");
                    sb.Append(m_refEmail.MakeUserTaskEmailLink(objTask, false));
                    sb.Append("</td>");
                }
                else if(objTask.AssignedToUserGroup != "")
                {
                    sb.Append("<td>");
                    sb.Append("<img src=\"" + AppPath + "images/UI/Icons/users.png\" alt=\"\" align=\"absbottom\"/>");
                    sb.Append(m_refEmail.MakeUserGroupTaskEmailLink(objTask));
                    sb.Append("</td>");
                }
            }
            if ((actiontype == "to") | (actiontype == "all") | (actiontype == "both"))
            {
                sb.Append("<td title\"" + m_refEmail.MakeByUserTaskEmailLink(objTask, false) + "\">");
                sb.Append(m_refEmail.MakeByUserTaskEmailLink(objTask, false));
                sb.Append("</td>");
            }
            if (objTask.LastComment == "")
            {
                sb.Append("<td> [" + this.MsgHelper.GetMessage("dd not specified") + "] </td>");
            }
            else
            {
                sb.Append("<td nowrap=\"nowrap\"><div class=\"comment-block\">" + objTask.LastComment + "</div></td>");
            }
            sb.Append("<td>" + AppUI.GetInternationalDateOnly(objTask.DateCreated) + "</td>");
            sb.Append("</tr>");
        }
        ltrViewContentTaskBody.Text = sb.ToString();
    }
    private void Display_ViewTaskType()
    {
        pnlViewTaskType.Visible = true;
        long lngCategoryID = 0;
        string strCategoryTitle = string.Empty;
        if(!string.IsNullOrEmpty(Request.QueryString["task_type_id"]))
        {
            lngCategoryID = Convert.ToInt64(Request.QueryString["task_type_id"]);
        }
        lngCompareCategoryID = lngCategoryID;
        if(!string.IsNullOrEmpty(Request.QueryString["task_category_title"]))
        {
            strCategoryTitle = Convert.ToString(Request.QueryString["task_category_title"]);
        }

        if (!string.IsNullOrEmpty(strCategoryTitle) & lngCategoryID > 0)
        {
            NewTaskTypeObj();
            if (false == objTaskType.UpdateTaskCategory(lngCategoryID, strCategoryTitle))
            {
                if (!string.IsNullOrEmpty(ErrorString))
                {
                    ShowErrorString(MsgHelper.GetMessage("title save task category"), action);
                }
            }
            else
            {
                lngCategoryID = 0;
                strCategoryTitle = "";
            }
        }

        callBackPage = BuildCallBackParms("&");
        StringBuilder sb = new StringBuilder();
        SelectAllCategory();
        for (int i = 1; i < colAllCategory.Count + 1; i++)
        {
            Collection coll = (Collection)colAllCategory[i];
            sb.Append("<tr>");
            sb.Append(" <td style=\"vertical-align:top !important\">");
            if (Convert.ToInt32(coll["active"]) <= 1 & (!(Convert.ToInt64(coll["task_type_id"]) == (long)EkEnumeration.TaskType.BlogPostComment) & !(Convert.ToInt64(coll["task_type_id"]) == (long)EkEnumeration.TaskType.TopicReply)))
            {
                sb.Append("<input title=\"Task Type ID\" id=\"ckb_" + coll["task_type_id"] + "\" type=\"checkbox\" name=\"ckb_" + coll["task_type_id"] + "\" onclick=\"checkTaskTypeS(this.id, this.checked);\" />");
            }
            else
            {
                sb.Append("<font color=\"gray\">" + MsgHelper.GetMessage("lbl cannot delete") + "</font>");
            }
            Collection colTaskTypeS = (Collection)coll["all_task_type"];
            sb.Append("     <td style=\"vertical-align:top !important\">");
            if (CollectionNotEmpty(colTaskTypeS)) 
            {
                sb.Append("<img id=\"img_" + coll["task_type_id"] + "\" alt=\"\" name=\"img_" + coll["task_type_id"] + "\" align=\"middle\" src=\"" + AppPath + "images/UI/Icons/TaskTypes/collapse.gif\" onclick=\"ToggleShowHideTaskType(this.id);\"/>");
            }
            else
            {
                PrintSpace(3);
            }
            if(lngCategoryID > 0)
            {
                if(lngCategoryID == Convert.ToInt64(coll["task_type_id"]))
                {
                    sb.Append(coll["task_type_id"] + "&nbsp;");
                    sb.Append("<input id=\"task_category_change\" type=\"text\" name=\"task_category_change\" maxlength=\"50\" value=\"" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll["task_type_title"].ToString()) + "\"/>");
                    sb.Append("<a title=\"Save\" id=\"task_category_save\" href=\"tasks.aspx?action=EditTaskType&ty=savetasktype\" onclick=\"return VerifyCategoryChange(this.id, " + coll["task_type_id"] + ");\" ><img alt=\"" + MsgHelper.GetMessage("lbl save category") + "\" align=\"absbottom\" src=\"" + AppPath + "images/UI/Icons/save.png\" id=\"image_save_category\"/></a>&nbsp;");
                    sb.Append("<a title=\"Cancel\" id=\"task_category_cancel\" href=\"tasks.aspx?action=ViewTaskType\" ><img alt=\"" + MsgHelper.GetMessage("btn cancel") + "\" align=\"absbottom\" src=\"" + AppPath + "images/UI/Icons/restore.png\" id=\"Img1\"/></a>");
                }
                else
                {
                    sb.Append("<a href=\"tasks.aspx?action=ViewTaskType&task_type_id=" + coll["task_type_id"] + "\" >" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll["task_type_title"].ToString()) + "</a>");
                }
            }
            else
            {
                sb.Append("<a href=\"tasks.aspx?action=ViewTaskType&task_type_id=" + coll["task_type_id"] + "\" >" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll["task_type_title"].ToString()) + "</a>");
            }
            if(CollectionNotEmpty(colTaskTypeS))
            {
                sb.Append("<div id=\"div_" + coll["task_type_id"] + "\" >");
                sb.Append(" <form id=\"frm_" + coll["task_type_id"] + "\">");
                sb.Append("     <table class=\"ektronGrid\">");
                sb.Append("         <tr class=\"title-header\">");
				sb.Append("				<td width=\"1%\" nowrap=\"nowrap\" title=\"Check to Delete\">" + MsgHelper.GetMessage("lbl check delete") + "</td>");
				sb.Append(" 			<td nowrap=\"nowrap\" title=\"Task Type Title\">" + MsgHelper.GetMessage("lbl task type title") + "</td>");
				sb.Append("				<td width=\"1%\" nowrap=\"nowrap\" title=\"Availability\">" + MsgHelper.GetMessage("lbl availability") + "</td>");
				sb.Append("			</tr>");
                for(int j = 1; j < colTaskTypeS.Count + 1; j++)
                {
                    Collection colTaskTypeItem = (Collection)colTaskTypeS[j];
                    if (colTaskTypeItem["active"].ToString() != "3")
                    {
                        sb.Append("<tr>");
                        sb.Append(" <td>");
                        if (Convert.ToInt32(colTaskTypeItem["active"]) <= 1)
                        {
                            sb.Append("<input title=\"Task Type ID\" id=\"ckb_" + coll["task_type_id"] + "_" + colTaskTypeItem["task_type_id"] + "\" type=\"checkbox\" name=\"ckb_" + coll["task_type_id"] + "_" + colTaskTypeItem["task_type_id"] + "\"  onclick=\"checkTaskType(this.id, this.checked);\"/>");
                        }
                        else
                        {
                            sb.Append("<font color=\"gray\">" + MsgHelper.GetMessage("lbl cannot delete") + "</font>");
                        }
                        sb.Append(" </td>");
                        sb.Append(" <td>");
                        sb.Append("     <a href=\"tasks.aspx?action=EditTaskType&task_type_id=" + colTaskTypeItem["task_type_id"] + "\" >" + colTaskTypeItem["task_type_title"] +"</a>");
                        sb.Append(" </td>");
                        sb.Append(" <td>");
                        if (Convert.ToInt32(colTaskTypeItem["active"]) <= 1)
                        {
                            if (Convert.ToBoolean((colTaskTypeItem["active"])) == false)
                            {
                                sb.Append(MsgHelper.GetMessage("lbl inactive"));
                            }
                            else
                            {
                                sb.Append(MsgHelper.GetMessage("lbl active"));
                            }
                        }
                        else
                        {
                            sb.Append("<font color=\"gray\">" + MsgHelper.GetMessage("lbl automatic") + "</font>");
                        }
                        sb.Append(" </td>");
                        sb.Append("</tr>");
                    }
                }
                sb.Append("     </table>");
                sb.Append(" </form>");
                sb.Append("</div>");
            }
            sb.Append("     </td>");
            sb.Append(" </td>");
            sb.Append("</tr>");
        }
        ltrViewTaskType.Text = sb.ToString();
    }
    private void Display_EditTaskType()
    {
        ValidateCanEditTask();
        pnlEditTaskType.Visible = true;
        long lngTaskTypeID = 0;
        string strTaskTypeTitle = string.Empty;
        string strTaskTypeDescription = string.Empty;
        bool blnActive;
        if (actiontype.ToLower() == "savetasktype")
        {
            callBackPage = BuildCallBackParms("&");
            if (!string.IsNullOrEmpty(Request.Form["task_type_id"]))
            {
                lngTaskTypeID = Convert.ToInt64(Request.Form["task_type_id"]);               
            }
            colParentCategory = objTaskType.GetCategoryByTaskTypeID(lngTaskTypeID);

            strTaskTypeTitle = Convert.ToString(Request.Form["task_type_title"]);
            strTaskTypeDescription = Convert.ToString(Request.Form["task_type_description"]);
            blnActive = string.IsNullOrEmpty(Convert.ToString(Request.Form["task_type_active"]));

            NewTaskTypeObj();
            if (false == objTaskType.UpdateTaskType(lngTaskTypeID, strTaskTypeTitle, strTaskTypeDescription, blnActive))
            {
                if (!string.IsNullOrEmpty(ErrorString))
                {
                    ShowErrorString(MsgHelper.GetMessage("title edit task type"), action);
                }
            }
            else
            {
                if (!is_child)
                {
                    Response.Redirect("tasks.aspx?action=ViewTaskType");
                }
                else
                {
                    Response.Redirect("tasks.aspx?action=EditTaskType&ty=savetasktype_succeed&saved_task_category=" + colParentCategory["task_type_id"] + "&saved_task_type=" + lngTaskTypeID);
                    colParentCategory = null;
                }
            }
        }
        else if (actiontype.ToLower() == "savetasktype_succeed")
        {
            ShowSuccess(MsgHelper.GetMessage("title edit task type"), lngCategory_tmp, lngType_tmp);
        }
        else
        {
            NewTaskTypeObj();
            lngTaskTypeID = Convert.ToInt64(Request.QueryString["task_type_id"]);
            
            colTaskType = objTaskType.GetTaskTypeByID(lngTaskTypeID);
            colParentCategory = objTaskType.GetCategoryByTaskTypeID(lngTaskTypeID);

            if (!string.IsNullOrEmpty(ErrorString))
            {
                ShowErrorString(MsgHelper.GetMessage("title edit task type"), action);
            }

            callBackPage = BuildCallBackParms("&");
            sTitleBar = MsgHelper.GetMessage("title edit task type");
        }
        ltrEditTaskTypeChk.Text = "<td class=\"value\"><input title=\"Enable/Disable Available\" id=\"task_type_active\" type=\"checkbox\" name=\"task_type_active\"";
        if (!(colTaskType.Count > 0))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["saved_task_type"]))
            {
                lngTaskTypeID = Convert.ToInt64(Request.QueryString["saved_task_type"]);
            }
            colTaskType = objTaskType.GetTaskTypeByID(lngTaskTypeID);
            colParentCategory = colTaskType;
        }
        if (!Convert.ToBoolean(colTaskType["active"]))
        {
            ltrEditTaskTypeChk.Text += "checked";
        }
        else
        {
            ltrEditTaskTypeChk.Text += "/></td>";
        }
      

    }
    private void Display_AddTaskType()
    {
        ValidateCanCreateTask();
        pnlAddTaskType.Visible = true;
        object lngNewID = null;
        object lngNewTypeID = null;
        string strTaskTypeTitle = string.Empty;
        string strTaskTypeDescription = string.Empty;
        string strNewCategory = string.Empty;
        long lngParentTaskTypeID = 0;
        bool blnActive = false;
        lngNewID = 0;
        lngNewTypeID = 0;
        if (actiontype.ToLower() == "savetasktype")
        {
            
            callBackPage = BuildCallBackParms("&");
            strTaskTypeTitle = Convert.ToString(Request.Form["new_task_type_title"]);
            strTaskTypeDescription = Convert.ToString(Request.Form["new_task_type_description"]);
            blnActive = Convert.ToBoolean(string.IsNullOrEmpty(Convert.ToString(Request.Form["new_active"])));

            switch (Request.Form["RadioGroup_category"])
            {
                case "new_category":
                    strNewCategory = Request.Form["new_category"];
                    NewTaskTypeObj();
                    if (false == objTaskType.AddTaskCategory(strNewCategory, ref lngNewID))
                    {
                        if (!string.IsNullOrEmpty(ErrorString))
                        {
                            ShowErrorString(MsgHelper.GetMessage("title add task type"), action);
                        }
                    }
                    lngParentTaskTypeID = objTaskType.TaskTypeID;
                    break;
                case "existing_category":
                    lngParentTaskTypeID = Convert.ToInt64(Request.Form["existing_category"]);
                    break;
            }
            NewTaskTypeObj();
            lngNewTypeID = Convert.ToInt64(lngNewTypeID);
            if (false == objTaskType.AddTaskType(strTaskTypeTitle, strTaskTypeDescription, lngParentTaskTypeID, blnActive, ref lngNewTypeID))
            {
                if (!string.IsNullOrEmpty(ErrorString))
                {
                    ShowErrorString(MsgHelper.GetMessage("title add task type"), action);
                }
            }
            else
            {
                if (!is_child)
                {
                    Response.Redirect("tasks.aspx?action=ViewTaskType");
                }
                else
                {
                    Response.Redirect("tasks.aspx?action=AddTaskType&ty=savetasktype_succeed&saved_task_category=" + lngParentTaskTypeID + "&saved_task_type=" + lngNewTypeID);
                }
            }
        }
        else if (actiontype.ToLower() == "savetasktype_succeed")
        {
            ShowSuccess(MsgHelper.GetMessage("title add task type"), lngCategory_tmp, lngType_tmp);
        }
        else
        {
            bool blnCategoryExists = false;
            SelectAllCategory();
            if (CollectionNotEmpty(colAllCategory)) 
            {
                for (int i = 1; i < colAllCategory.Count + 1; i++)
                {
                    Collection coll = (Collection)colAllCategory[i];
                    if (Convert.ToInt32(coll["active"]) == 1 & !((Convert.ToInt64(coll["task_type_id"]) == (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment) | (Convert.ToInt64(coll["task_type_id"]) == (long)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply)))
                    {
                        blnCategoryExists = true;
                    }
                }
            }
            callBackPage = BuildCallBackParms("&");
            pnlAddTaskType.Visible = true;
            StringBuilder sb = new StringBuilder();
            sb.Append("<input id=\"radio_existing_category\" type=\"radio\" value=\"existing_category\" name=\"RadioGroup_category\"");
            if (blnCategoryExists)
            {
                sb.Append(" checked=\"true\" ");
            }
            else
            {
                sb.Append(" disabled=\"true\" ");
            }
            sb.Append("/>" + MsgHelper.GetMessage("lbl add to existing category") + "");
            sb.Append("<div class=\"ektronTopSpaceSmall\"></div>");
            sb.Append("<select id=\"existing_category\" name=\"existing_category\"");
            if (blnCategoryExists)
            {
                sb.Append(" onclick=\"document.getElementById('radio_existing_category').checked=true\";> ");
                for (int i = 1; i < colAllCategory.Count + 1; i++)
                {
                    Collection coll = (Collection)colAllCategory[i];
                    if (Convert.ToInt32(coll["active"]) == 1 & !((Convert.ToInt64(coll["task_type_id"]) == (long)Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment) | (Convert.ToInt64(coll["task_type_id"]) == (long)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply)))
                    {
                        sb.Append("<option value=\"" + coll["task_type_id"] + "\" >" + coll["task_type_title"] + "</option>");
                    }
                }
            }
            else
            {
                sb.Append(" disabled=\"true\" style=\"width:256;\"> ");
            }
            sb.Append("</select>");
            sb.Append("<div class=\"ektronTopSpaceSmall\"></div>");
            sb.Append("<input id=\"radio_new_category\" type=\"radio\" value=\"new_category\" name=\"RadioGroup_category\" ");
            if (!blnCategoryExists)
            {
                sb.Append("checked=\"true\"");
            }
            sb.Append("/>" + MsgHelper.GetMessage("lbl existing category"));
            ltrAddTaskType.Text = sb.ToString();
        }
    }
    #endregion
    # region Process Tasks Events
    private void Process_AddTask()
    {
        ValidateCanCreateTask();
        objTask = AppUI.EkTaskRef;
        objTask.TaskTitle = Request.Form["task_title"];
        if (Request.Form["priority"] != "")
        {
            objTask.Priority = (EkEnumeration.TaskPriority)Convert.ToInt32(Request.Form["priority"]);
        }
        if (Request.Form["status"] != "")
        {
            objTask.Status = Convert.ToInt32(Request.Form["status"]);
        }
        objTask.StartDate = Request.Form["hdnstartdate"];
        objTask.DueDate = Request.Form["hdnduedate"];
        objTask.Description = ctlEditor.Content.Replace("'", "&#39;");
        if (!string.IsNullOrEmpty(Request.Form["assigned_to_user_id"]))
        {
            objTask.AssignedToUserID = Convert.ToInt64(Request.Form["assigned_to_user_id"]);
        }
        else if (!string.IsNullOrEmpty(Request.Form["assigned_to_usergroup_id"]))
        {
            objTask.AssignToUserGroupID = Convert.ToInt64(Request.Form["assigned_to_usergroup_id"]);
        }
        else
        {
            objTask.AssignedToUserID = currentUserID;
        }
        objTask.AssignedByUserID = currentUserID.ToString();
        if (!string.IsNullOrEmpty(Request.Form["content_id"]))
        {
            objTask.ContentID = Convert.ToInt64(Request.Form["content_id"]);
        }
        if (!string.IsNullOrEmpty(Request.Form["state"]))
        {
            objTask.State = Request.Form["state"].ToString();
        }
        else
        {
            objTask.State = "1";
        }
        objTask.ContentLanguage = ContentLanguage;

        if (EnableMultilingual == 1)
        {
            languageID = Convert.ToInt32(Request.Form["language"]);
        }
        else
        {
            languageID = ContentLanguage;
        }
        objTask.LanguageID = languageID;

        if ((Request.Form["task_type"] != "") && (System.Convert.ToInt64(Request.Form["task_type"]) > 0))
        {
            objTask.TaskTypeID = Convert.ToInt64(Request.Form["task_type"]);
        }
        ret = objTask.AddTask();

        if (Request.QueryString["callbackpage"] == "content.aspx")
        {
            callBackPage = Request.QueryString["callbackpage"] + "?action=view&id=" + ContentId + "&langtype=" + Request.QueryString["langtype"];
        }
        else if (Request.QueryString["callbackpage"] == "exit.htm")
        {
            callBackPage = "exit.htm";
        }
        else
        {
            callBackPage = "tasks.aspx?action=ViewTasks&orderby=" + OrderBy + "&ty=" + EkFunctions.HtmlEncode(Request.Form["ty"]);
        }
        if (ret)
        {
            Response.Redirect("reterror.aspx?info=" + actErrorString);
        }
        else
        {
            Response.Redirect(callBackPage);
        }

    }
    private void Process_EditTask()
    {
        ValidateCanEditTask();
        objTask = AppUI.EkTaskRef;
        objTask.TaskTitle = Request.Form["task_title"];
        if (Request.Form["priority"] != "")
        {
            objTask.Priority = (EkEnumeration.TaskPriority)Convert.ToInt32(Request.Form["priority"]);
        }
        if (Request.Form["status"] != "")
        {
            objTask.Status = Convert.ToInt32(Request.Form["status"]);
        }
        objTask.StartDate = Request.Form["hdnstartdate"];
        objTask.DueDate = Request.Form["hdnduedate"];
        objTask.Description = ctlEditor.Content;
        objTask.TaskID = Convert.ToInt64(Request.Form["task_id"]);
        if (!string.IsNullOrEmpty(Request.Form["assigned_to_user_id"]))
        {
            objTask.AssignedToUserID = Convert.ToInt64(Request.Form["assigned_to_user_id"]);
        }
        if (!string.IsNullOrEmpty(Request.Form["assigned_to_usergroup_id"]))
        {
            objTask.AssignToUserGroupID = Convert.ToInt64(Request.Form["assigned_to_usergroup_id"]);
        }
        if (!string.IsNullOrEmpty(Request.Form["assigned_by_user_id"]))
        {
            objTask.AssignedByUserID = Request.Form["assigned_by_user_id"];
        }
        if (!string.IsNullOrEmpty(Request.Form["content_id"]))
        {
            objTask.ContentID = Convert.ToInt64(Request.Form["content_id"]);
        }
        else
        {
            objTask.ContentID = -1;
        }
        if (Request.Form["state"] != null)
        {
            objTask.State = Request.Form["state"];
        }

        languageID = Convert.ToInt32(Request.Form["current_language"]);
        objTask.ContentLanguage = languageID;
        if (EnableMultilingual == 1)
        {
            languageID = Convert.ToInt32(Request.Form["language"]);
        }
        else
        {
            languageID = ContentLanguage;
        }
        objTask.LanguageID = languageID;

        if ((Request.Form["task_type"] != "") && (System.Convert.ToInt64(Request.Form["task_type"]) > 0))
        {
            objTask.TaskTypeID = Convert.ToInt64(Request.Form["task_type"]);
        }
        else if ((Request.Form["task_type_"] != "") && (System.Convert.ToInt64(Request.Form["task_type_"]) > 0)) //for blogcomments
        {
            objTask.TaskTypeID = Convert.ToInt64(Request.Form["task_type_"]);
        }
        ret = objTask.UpdateTask();
        if (ret)
        {
            Response.Redirect("reterror.aspx?info=" + actErrorString);
        }
        else
        {
            callBackPage = BuildCallBackParms("&");
            if (!string.IsNullOrEmpty(Request.QueryString["close"]))
            {
                Response.Redirect("close.aspx");
            }
            else if (!string.IsNullOrEmpty(Request.Form["blogid"]))
            {
                if (!string.IsNullOrEmpty(Request.Form["content_id"]))
                {
                    callBackPage = "content.aspx?id=" + Request.Form["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + Request.Form["content_id"];
                }
                else
                {
                    callBackPage = "content.aspx?id=" + Request.Form["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments;
                }
                Response.Redirect(callBackPage, false);
            }
            else if (Convert.ToInt64(Request.Form["assigned_by_user_id"]) != currentUserID)
            {
                Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=to" + callBackPage);
            }
            else
            {
                Response.Redirect("tasks.aspx?action=ViewTask&tid=" + Request.Form["task_id"] + callBackPage);
            }
        }
    }
    private void Process_DeleteTaskType()
    {
        ValidateCanDeleteTask();
        string[] aTaskTypeID = null;
        aTaskTypeID = Request.Form["tasktype_ids"].ToString().Split(',');
        objTaskType = AppUI.EkTaskTypeRef;
        for (int i = 0; i < aTaskTypeID.Length; i++)
        {
            objTaskType.DeleteTaskType(Convert.ToInt64(aTaskTypeID[i].ToString()));
        }
        Response.Redirect("tasks.aspx?action=ViewTaskType");
    }
    private void Process_UpdateStatePurge()
    {
        int State = Convert.ToInt32(Request.Form["state"]);
        string PurgeTasks = Request.Form["purge"];
        string actionType = Request.Form["actiontype"];
        string user = Request.Form["selectusergroup"];
        string taskids = Request.Form["taskids"];
        if ((!string.IsNullOrEmpty(taskids)))
        {
            taskids = Strings.Left(taskids, Strings.Len(taskids) - 1);
        }
        else
        {
            taskids = "";
        }
        objTask = AppUI.EkTaskRef;
        if ((PurgeTasks == "1"))
        {
            ret = objTask.DeleteTasks(taskids, 9);
            if ((!string.IsNullOrEmpty(actErrorString)))
            {
                Response.Redirect("reterror.aspx?info=" + (actErrorString));
            }

            Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=" + Request.Form["actiontype"]);
        }
        else if ((actionType == "to" | actionType == "by"))
        {
            int userType = Convert.ToInt32(Strings.Right(user, 1));
            long iuser = Convert.ToInt64(Strings.Left(user, Strings.Len(user) - 2));
            ret = objTask.SetUserForTasks(taskids, userType, iuser, 0);
            if ((!string.IsNullOrEmpty(actErrorString)))
            {
                Response.Redirect("reterror.aspx?info=" + actErrorString);
            }

            Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=" + Request.Form["actiontype"]);
        }
        else if ((actionType == "touser"))
        {
            string userType = Strings.Right(user, 1);
            user = Strings.Left(user, Strings.Len(user) - 2);
            Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=" + Request.Form["actiontype"] + "&user=" + user + "&usertype=" + userType);
        }
        else
        {
            ret = objTask.SetStateForTasks(taskids, State);
            if ((!string.IsNullOrEmpty(actErrorString)))
            {
                Response.Redirect("reterror.aspx?info=" + actErrorString);
            }

            Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=" + Request.Form["actiontype"]);
        }

    }
    private void Process_DeleteAllTasks()
    {
        ValidateCanDeleteTask();
        string taskids = string.Empty;
        string actionTy = Request.Form["ty"];
        string[] arrArray = Request.Form["taskids"].Split(',');
        for (int i = 0; i < arrArray.Length; i++)
        {
            if (Request.Form["id_" + arrArray[i]] == "on")
            {
                taskids = taskids + arrArray[i] + ",";
            }
        }

        if ((!string.IsNullOrEmpty(taskids)))
        {
            taskids = Strings.Left(taskids, Strings.Len(taskids) - 1);
        }
        else
        {
            taskids = "";
        }
        objTask = AppUI.EkTaskRef;
        if ((actionTy == "deleted"))
        {
            ret = objTask.DeleteTasks(taskids, 9);
        }
        else
        {
            ret = objTask.DeleteTasks(taskids, 0);
        }

        if ((!string.IsNullOrEmpty(actErrorString)))
        {
            Response.Redirect("reterror.aspx?info=" + (actErrorString));
        }
        Response.Redirect("tasks.aspx?action=ViewTasks&orderby=" + Request.Form["orderby"] + "&ty=" + Request.Form["ty"]);

    }
    private void Process_ApproveTask()
    {
        objTask = AppUI.EkTaskRef;
        long taskid = Convert.ToInt64(Request.QueryString["tid"]);
        objTask = objTask.GetTaskByID(taskid);
        objTask.State = Convert.ToString(Ektron.Cms.Common.EkEnumeration.TaskState.Completed);
        ret = objTask.UpdateTask();
        if (Request.QueryString["close"] == "true")
        {
            callBackPage = "close.aspx";
        }
        else if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
        {
            callBackPage = "content.aspx?id=" + Request.QueryString["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + Request.QueryString["contentid"];
        }
        else
        {
            callBackPage = "content.aspx?id=" + Request.QueryString["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments;
        }
        if ((ret))
        {
            Response.Redirect("reterror.aspx?info=" + actErrorString);
        }
        else
        {
            Response.Redirect(callBackPage);
        }
    }
    private void Process_DeleteTask()
    {
        ValidateCanDeleteTask();
        string taskid = Request.QueryString["tid"];
        string actionTy = Request.Form["ty"];
        objTask = AppUI.EkTaskRef;
        if ((actionTy == "deleted"))
        {
            ret = objTask.DeleteTasks(taskid, 9);
        }
        else
        {
            ret = objTask.DeleteTasks(taskid, 0);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["forumid"]))
        {
            long forumId = EkFunctions.ReadDbLong(Request.QueryString["forumId"]);
            if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
            {
                long topicId = EkFunctions.ReadDbLong(Request.QueryString["contentid"]);
                callBackPage = "content.aspx?id=" + forumId + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + Request.QueryString["contentid"];
                ObjectFactory.GetForum().UpdateStatisticsForTopic(topicId);
            }
        }
        else if (!string.IsNullOrEmpty(Request.QueryString["blogid"]))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
            {
                callBackPage = "content.aspx?id=" + Request.QueryString["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + Request.QueryString["contentid"];
            }
            else
            {
                callBackPage = "content.aspx?id=" + Request.QueryString["blogid"] + "&action=ViewContentByCategory&LangType=" + AppUI.ContentLanguage + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments;
            }
        }
        else if (!string.IsNullOrEmpty(Request.QueryString["fromViewContent"]))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
            {
                callBackPage = "content.aspx?id=" + Request.QueryString["folderid"] + "&action=View&LangType=" + AppUI.ContentLanguage + "&contentid=" + Request.QueryString["contentid"];
            }
            else
            {
                 callBackPage = getCallBackupPage("tasks.aspx?action=ViewTasks&ty=" + Request.QueryString["ty"]);
        
            }
        }
        else
        {
            callBackPage = getCallBackupPage("tasks.aspx?action=ViewTasks&ty=" + Request.QueryString["ty"]);
        }
        if (Strings.Len(actErrorString) > 0)
        {
            Response.Redirect("reterror.aspx?info=" + actErrorString);
        }
        else
        {
            Response.Redirect(callBackPage);
        }
    }
    #endregion
    #region ToolBar
    private void GetAddTaskToolBar()
    {
        StringBuilder sbToolBar = new StringBuilder();

		if (!string.IsNullOrEmpty(Request.QueryString["TreeVisible"]))
		{
			if (Request.QueryString["TreeVisible"] == "Content")
			{
				sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "close.aspx", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "#", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"if(IsChildWaiting()) { return true; } else { return history.back(); }\"", StyleHelper.BackButtonCssClass, true));
			}
		}
		else
		{
			if ((!string.IsNullOrEmpty(Request.ServerVariables["HTTP_REFERER"]) && !string.IsNullOrEmpty(Request.QueryString["callbackpage"]) && Request.QueryString["callbackpage"].ToString().ToLower() == "cmsform.aspx" && Request.QueryString["value1"].ToString().ToLower() == "viewform") | (Request.ServerVariables["HTTP_USER_AGENT"].IndexOf("FireFox") + 1 > 0))
			{
				sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "#", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"if(IsChildWaiting()) { return true; } else { window.location.href = '" + Request.ServerVariables["HTTP_REFERER"] + "'; return true;}\"", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "#", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"if(IsChildWaiting()) { return true; } else { history.back(); return true;}\"", StyleHelper.BackButtonCssClass, true));
			}
		}

		if (Convert.ToBoolean(canI["CanIAddTask"]))
        {
			sbToolBar.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("btn add task"), MsgHelper.GetMessage("btn save"), "onclick=\"if(IsChildWaiting()) { return false; } else { return SubmitForm('AddTask', 'VerifyForm()'); }\"", StyleHelper.SaveButtonCssClass, true));
        }
        
        ltrAddTaskToolBar.Text = sbToolBar.ToString();
    }
    #endregion
    # region helper funtions
    protected void RegisterResources()
    {
        Packages.Ektron.Workarea.Core.Register(this);
        Packages.jQuery.jQueryUI.Tabs.Register(this);
        Packages.jQuery.jQueryUI.Widget.Register(this);
    }

    private void checkAccess()
    {
        if (!Utilities.ValidateUserLogin()) {
            return;
        }

        if (Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser)) {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), false);
            return;
        }
    }

    //protected bool CanTask() {
    //    return (CanCreateTask || CanDeleteTask || CanRedirectTask);
    //}

    protected void ValidateCanCreateTask() {
        if (!CanCreateTask) {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("error: user not permitted"), false);
        }
    }

    protected void ValidateCanDeleteTask() {
        if (!CanDeleteTask) {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("error: user not permitted"), false);
        }
    }

    protected void ValidateCanEditTask() {
        if (!CanEditTask) {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("error: user not permitted"), false);
        }
    }

    protected void ValidateCanRedirectTask() {
        if (!CanRedirectTask) {
            Response.Redirect("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("error: user not permitted"), false);
        }
    }

    protected bool CanCreateTask {
        get {
            if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
            {
                TaskID = Convert.ToInt64(Request.QueryString["tid"]);
                security_task_data = m_refContentApi.LoadPermissions(TaskID, "tasks", ContentAPI.PermissionResultType.Task);
            }
            return (AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.CreateTask)
                    || AppUI.IsAdmin()
                    || AppUI.IsARoleMemberForFolder_FolderUserAdmin(FolderId)
                    || AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminFolderUsers)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.CreateTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.DeleteTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.RedirectTask)
                    || (security_task_data != null && security_task_data.CanAddTask)
                    || Convert.ToBoolean(cContObj.CanIv2_0(-1, "tasks")["CanIAddTask"]));
        }
    }

    protected bool CanDeleteTask {
        get {
            if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
            {
                TaskID = Convert.ToInt64(Request.QueryString["tid"]);
                security_task_data = m_refContentApi.LoadPermissions(TaskID, "tasks", ContentAPI.PermissionResultType.Task);
            }
            return (AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.DeleteTask)
                    || AppUI.IsAdmin()
                    || AppUI.IsARoleMemberForFolder_FolderUserAdmin(FolderId)
                    || AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminFolderUsers)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.CreateTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.DeleteTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.RedirectTask)
                    || (security_task_data != null && security_task_data.CanDeleteTask));
        }
    }

    protected bool CanEditTask {
        get { return CanCreateTask || CanDeleteTask; }
    }

    protected bool CanRedirectTask {
        get {
            return (AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.RedirectTask)
                    || AppUI.IsAdmin()
                    || AppUI.IsARoleMemberForFolder_FolderUserAdmin(FolderId)
                    || AppUI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminFolderUsers)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.CreateTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.DeleteTask)
                    || TestMaskedValue(TaskPermissionId, (int)EkEnumeration.TaskPermission.RedirectTask));
        }
    }

    protected int _taskPermissionId = -1;
    protected int TaskPermissionId {
        get { return (_taskPermissionId >= 0 ? _taskPermissionId : (_taskPermissionId = m_refSiteApi.GetTaskPermission(AppUI.UserId, 1))); }
    }

    protected bool TestMaskedValue(int testVal, int maskVal) {
        return (maskVal == (testVal & maskVal));
    }

    private long _folderId = -1;
    protected long FolderId {
        get { return (_folderId >= 0 ? _folderId : (_folderId = (ContentId > 0 ? cContObj.GetJustFolderIdByContentId(ContentId) : 0 ))); }
    }

    private long _contentId = -1;
    protected long ContentId {
        get { return (_contentId >= 0 ? _contentId : (_contentId = (null != Request.QueryString["cid"] ? Convert.ToInt64(Request.QueryString["cid"]) : 0))); }
    }

    public string getCallBackupPage(string defval)
    {
        string returnValue;
        string tmpStr;
        if (Request.QueryString["callbackpage"] != "")
        {
            tmpStr = Request.QueryString["callbackpage"];
            if ((tmpStr == "cmsform.aspx") && (Request.QueryString["fldid"] != ""))
            {
                tmpStr = tmpStr + "?folder_id=" + Request.QueryString["fldid"] + "&" + Request.QueryString["parm1"] + "=" + Request.QueryString["value1"];
            }
            else
            {
                tmpStr = tmpStr + "?" + EkFunctions.HtmlEncode(Request.QueryString["parm1"]) + "=" + Request.QueryString["value1"];
            }

            if (Request.QueryString["parm2"] != "")
            {
                tmpStr = tmpStr + "&" + EkFunctions.HtmlEncode(Request.QueryString["parm2"]) + "=" + EkFunctions.HtmlEncode(Request.QueryString["value2"]);
                if (Request.QueryString["parm3"] != "")
                {
                    tmpStr = tmpStr + "&" + EkFunctions.HtmlEncode(Request.QueryString["parm3"]) + "=" + EkFunctions.HtmlEncode(Request.QueryString["value3"]);
                    if (Request.QueryString["parm4"] != "")
                    {
                        tmpStr = tmpStr + "&" + EkFunctions.HtmlEncode(Request.QueryString["parm4"]) + "=" + EkFunctions.HtmlEncode(Request.QueryString["value4"]);
                    }
                }
            }
            returnValue = tmpStr;
        }
        else
        {
            returnValue = defval;
        }
        return returnValue;
    }
    public string BuildCallBackParms(string leadingChar)
    {
        string returnValue;
        string strTmp2;
        if (Request.QueryString["callbackpage"] != "")
        {
            strTmp2 = (string)("callbackpage=" + EkFunctions.HtmlEncode(Request.QueryString["callbackpage"]) + "&parm1=" + EkFunctions.HtmlEncode(Request.QueryString["parm1"]) + "&value1=" + EkFunctions.HtmlEncode(Request.QueryString["value1"]));
            if (Request.QueryString["parm2"] != "")
            {
                strTmp2 = strTmp2 + "&parm2=" + EkFunctions.HtmlEncode(Request.QueryString["parm2"]) + "&value2=" + EkFunctions.HtmlEncode(Request.QueryString["value2"]);
                if (Request.QueryString["parm3"] != "")
                {
                    strTmp2 = strTmp2 + "&parm3=" + EkFunctions.HtmlEncode(Request.QueryString["parm3"]) + "&value3=" + EkFunctions.HtmlEncode(Request.QueryString["value3"]);
                    if (Request.QueryString["parm4"] != "")
                    {
                        strTmp2 = strTmp2 + "&parm4=" + EkFunctions.HtmlEncode(Request.QueryString["parm4"]) + "&value4=" + EkFunctions.HtmlEncode(Request.QueryString["value4"]);
                    }
                }
            }
            strTmp2 = leadingChar + strTmp2;
            returnValue = strTmp2;
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }
    protected void SelectAllCategory()
    {
        objTaskType = AppUI.EkTaskTypeRef;
        colAllCategory = objTaskType.SelectAllCategory();
        if (colAllCategory == null)
        {
            CheckErrorString("Task Type: SelectAllCategory");
        }
    }
    protected bool CollectionNotEmpty(Collection coll)
    {
        bool retval = false;
        if (coll != null && coll.Count > 0)
        {
            retval = true;
        }
        else
        {
            retval = false;
        }
        return retval;
    }
    private void CheckErrorString(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Response.Redirect("reterror..aspx?info=" + Server.UrlEncode("Error occured. " + error));
        }
    }
    private string GetAssignedToUserFullName(EkTask task)
    {
        string retval = string.Empty;
        retval = GetUserFullName(task.AssignedToUserID);
        return retval;
    }
    private string GetAssignedByUserFullName(EkTask task)
    {
        string retval = string.Empty;
        retval = GetUserFullName(Convert.ToInt64(task.AssignedByUserID));
        return retval;
    }
    private string GetUserFullName(long userid)
    {
        string retval = string.Empty;
        UserAPI objUserAPI = new UserAPI();
        UserData objUserData = objUserAPI.GetActiveUserById(userid, false);
        retval = objUserData.FirstName + " " + objUserData.LastName;
        return retval;
    }
    private void PrintSpace(int lngAmount)
    {
        int lngIndex = 0;
        if (lngAmount > 0)
        {
            lngIndex = 1;
            while (lngIndex <= lngAmount)
            {
                Response.Write(" &nbsp;");
                lngIndex = lngIndex + 1;
            }
        }
    }
    private void NewTaskTypeObj()
    {
        objTaskType = null;
        objTaskType = AppUI.EkTaskTypeRef;
    }
    protected string DisplayDateSelector(string formName, string elementID, string spanID, bool allowEdit, string strDate)
    {
        string retval = string.Empty;
        EkDTSelector dSo;
        dSo = m_refSiteApi.EkDTSelectorRef;
        dSo.extendedMeta = true;
        dSo.formName = formName;
        dSo.SpanAttributes = "name=\"" + formName + "\"";
        dSo.formElement = spanID;
        dSo.spanId = elementID;
        if (strDate != string.Empty)
        {
            dSo.targetDate = Convert.ToDateTime(strDate);

        }
        else
        {
            dSo.targetDate = Convert.ToDateTime(null);
        }
        retval = dSo.displayCultureDate(allowEdit, spanID, formName);
        
        return retval;
    }
    protected string ListComments(int TaskLang)
    {
        StringBuilder sbListComments = new StringBuilder();
        cComments = cContObj.GetAllComments(CommentKeyId, CommentId, RefId, RefType, currentUserID, OrderBy);
        if (cComments != null)
        {
            for (int i = 1; i < cComments.Count + 1; i++)
            {
                Collection coll = (Collection)cComments[i];
                if (Convert.ToInt64(coll["USER_ID"]) == currentUserID | currentUserID == 1)
                {
                    sbListComments.Append("<tr><td><a href=javascript:openComment('taskcomment.aspx?ref_type=" + RefType + "&ty=" + actiontype + "&orderby=" + OrderBy + "&action=Edit&ref_id=" + RefId + "&commentkey_id=" + CommentKeyId + "&comment_id=" + coll["COMMENT_ID"] + "');>" + coll["DATE_CREATED"] + "</a></td><td>" + coll["FIRST_NAME"] + " " + coll["LAST_NAME"] + "</td><td>" + coll["COMMENTS_TEXT"] + "</td></tr>");
                }
                else
                {
                    sbListComments.Append("<tr><td>" + coll["DATE_CREATED"] + "</td><td>" + coll["FIRST_NAME"] + " " + coll["LAST_NAME"] + "</td><td>" + coll["COMMENTS_TEXT"] + "</td></tr>");
                }
            }
        }
        return sbListComments.ToString();
    }
    protected long GetUserIdByGroupId(long GroupID)
    {
        long retval = 0;
        usrObj = AppUI.EkUserRef;
        cGroups = usrObj.GetUsersByGroupv2_0(GroupID, "");
        if (cGroups != null)
        {
            for (int i = 1; i <= cGroups.Count; i++)
            {
                Collection coll = (Collection)cGroups[i];
                if (!string.IsNullOrEmpty(coll["UserID"].ToString()) && Convert.ToInt64(coll["UserID"]) > 0)
                {
                    if (Convert.ToInt64(coll["UserID"]) == currentUserID)
                    {
                        retval = Convert.ToInt64(coll["UserID"]);
                        break;
                    }
                }
            }
        }
        usrObj = null;
        cGroups = null;
        return retval;
    }
    protected bool IsUserInGroup(long GroupID)
    {
        bool retval = false;
        usrObj = AppUI.EkUserRef;
        cGroups = usrObj.GetGroupsUserIsInv2_0(currentUserID, EkEnumeration.GroupOrderBy.GroupName);
        if (cGroups != null)
        {
            for (int i = 1; i <= cGroups.Count; i++)
            {
                Collection coll = (Collection)cGroups[i];
                if (Convert.ToInt64(coll["UserGroupID"]) != -1 & Convert.ToInt64(coll["UserGroupID"]) != 0)
                {
                    if (Convert.ToInt64(coll["UserGroupID"]) == GroupID)
                    {
                        retval = true;
                        break;
                    }
                }
            }
        }
        return retval;
    }
    protected bool IsBrowserNS4()
    {
        bool retval = false;
        if (Request.ServerVariables["http_user_agent"].ToString().IndexOf("Mozilla") + 1 > 0 && Request.ServerVariables["http_user_agent"].ToString().IndexOf("4.7") + 1 > 0 && Request.ServerVariables["http_user_agent"].ToString().IndexOf("GECKO") < 0)
        {
            retval = true;
        }
        else
        {
            retval = false;
        }
        return retval;
    }
    protected bool IsBrowserIE()
    {
        bool retval = false;
        if (!g_isIEFlagInitialized)
        {
            if (Request.ServerVariables["HTTP_USER_AGENT"].IndexOf("MSIE") + 1 > 0)
            {
                g_isIEFlagInitialized = true;
                retval = true;
            }
        }
        return retval;
    }
    protected string MakeTaskTypeLink(string strTaskAction)
    {
        StringBuilder retval = new StringBuilder();
        retval.Append("<a href=\"#\" class=\"button buttonInline greenHover minHeight buttonAdd\" ");
        retval.Append("onclick=\"LoadTaskTypePage('action=AddTaskType&is_child=true')\" ");
        retval.Append("title=\"" + MsgHelper.GetMessage("title add task type") + "\" >" + MsgHelper.GetMessage("generic add title") + "</a>");
        retval.Append("&nbsp; <a href=\"#\" class=\"button buttonInline blueHover minHeight buttonEdit\" ");
        retval.Append("onclick=\"var tt_id=GetTaskType();if (tt_id>0) {LoadTaskTypePage('action=EditTaskType&task_type_id='+tt_id+'&is_child=true');}");
        retval.Append("else {alert('" + MsgHelper.GetMessage("alt select edit") + "')}\" title=\"" + MsgHelper.GetMessage("title edit task type") + "\" >");
        retval.Append(MsgHelper.GetMessage("generic edit title") + "</a>");
        return retval.ToString();
    }
    protected string DisplayTaskType(string strTaskAction)
    {
        if (TaskID > 0)
            objTask = objTask.GetTaskByID(TaskID);
        StringBuilder sbDisplayTaskType = new StringBuilder();
        objTaskType = null;
        Collection coll;
        if (objTask != null & objTask.TaskTypeID > 0)
        {
            objTaskType = AppUI.EkTaskTypeRef;
            coll = objTaskType.GetCategoryByTaskTypeID(objTask.TaskTypeID);
            if (CollectionNotEmpty(coll))
            {
                if (Convert.ToInt32(coll["active"]) > 1)
                {
                    strTaskAction = "ViewTask";
                }
            }
        }
        sbDisplayTaskType.Append("<tr>");
        sbDisplayTaskType.Append(" <td class=\"label\">" + MsgHelper.GetMessage("lbl task category") + ":&nbsp;</td>");
        sbDisplayTaskType.Append(" <td class=\"value\">");
        sbDisplayTaskType.Append("  <table width=\"100%\">");
        sbDisplayTaskType.Append("     <td>");

        switch (strTaskAction)
        {
            case "AddTask":
            case "EditTask":
                switch (objTask.TaskTypeID)
                {
                    case (long)EkEnumeration.TaskType.BlogPostComment:
                        sbDisplayTaskType.Append("<select name=\"task_category\" ID=\"task_category\" disabled=\"true\"><option>Blog</option></select>");
                        break;
                    case (long)EkEnumeration.TaskType.TopicReply:
                        sbDisplayTaskType.Append("<select name=\"task_category\" ID=\"task_category\" disabled=\"true\"><option>" + MsgHelper.GetMessage("lbl reply") + "</option></select>");
                        break;
                    default:
                        sbDisplayTaskType.Append("<select name=\"task_category\" ID=\"task_category\" onchange=\"DisplayTaskTypeDropDown('" + strTaskAction + "Form" + "');\">");
                        sbDisplayTaskType.Append("</select>");
                        sbDisplayTaskType.Append("<script type=\"text/javascript\">DisplayTaskCategoryDropDown('" + strTaskAction + "Form" + "');</" + "script>");
                        break;
                }
                break;
            case "ViewTask":
                if (!(objTask.TaskTypeID == ((long)EkEnumeration.TaskType.BlogPostComment | (long)EkEnumeration.TaskType.TopicReply)))
                {
                    if ((objTask != null) & objTask.TaskTypeID > 0)
                    {
                        objTaskType = AppUI.EkTaskTypeRef;
                        coll = objTaskType.GetCategoryByTaskTypeID(objTask.TaskTypeID);
                        if (CollectionNotEmpty(coll))
                        {
                            sbDisplayTaskType.Append("<input type=\"hidden\" name=\"task_category\" value=\"" + coll["task_type_id"] + "\">");
                            sbDisplayTaskType.Append(coll["task_type_title"]);
                        }
                        else
                        {
                            CheckErrorString("Task Type: GetCategoryByTaskTypeID");
                        }
                    }
                    else
                    {
                        sbDisplayTaskType.Append("[" + MsgHelper.GetMessage("dd not specified") + "]");
                    }
                }
                else
                {
                    sbDisplayTaskType.Append("Blog");
                }
                break;
        }

        if ("EditTask" == strTaskAction)
        {
            if ((objTask != null) & objTask.TaskTypeID > 0)
            {
                objTaskType = AppUI.EkTaskTypeRef;
                coll = objTaskType.GetCategoryByTaskTypeID(objTask.TaskTypeID);
                if (CollectionNotEmpty(coll))
                {
                    sbDisplayTaskType.Append("<script type=\"text/javascript\">SelectTaskCategoryDropDown('" + strTaskAction + "Form" + "', " + coll["task_type_id"] + ") ;</" + "script>");
                }
                else
                {
                    CheckErrorString("Task Type: GetCategoryByTaskTypeID");
                }
            }
        }

        sbDisplayTaskType.Append(MsgHelper.GetMessage("lbl task type") + ":");

        switch (strTaskAction)
        {
            case "AddTask":
            case "EditTask":
                switch (Convert.ToInt64(objTask.TaskTypeID))
                {
                    case (long)EkEnumeration.TaskType.BlogPostComment:
                        sbDisplayTaskType.Append("<select name=\"task_type\" ID=\"task_type\" disabled=\"true\"><option selected value=\"" + EkEnumeration.TaskType.BlogPostComment + "\">Blog Post Comment</option>\t");
                        sbDisplayTaskType.Append("</select><input type=\"hidden\" name=\"task_type_\" ID=\"task_type_\" value=\"" + EkEnumeration.TaskType.BlogPostComment + "\"/>");
                        break;
                    case (long)EkEnumeration.TaskType.TopicReply:
                        sbDisplayTaskType.Append("<select name=\"task_type\" ID=\"task_type\" disabled=\"true\"><option selected value=\"" + EkEnumeration.TaskType.TopicReply + "\">" + MsgHelper.GetMessage("lbl topicreply") + "</option>\t");
                        sbDisplayTaskType.Append("</select><input type=\"hidden\" name=\"task_type_\" ID=\"task_type_\" value=\"" + EkEnumeration.TaskType.TopicReply + "\"/>");
                        break;
                    default:
                        sbDisplayTaskType.Append("<select name=\"task_type\" ID=\"task_type\">\t");
                        sbDisplayTaskType.Append("</select>\t");
                        sbDisplayTaskType.Append("<script type=\"text/javascript\">DisplayTaskTypeDropDown('" + strTaskAction + "Form" + "');</" + "script>");
                        break;
                }
                break;
            case "ViewTask":
                if ((objTask != null) & objTask.TaskTypeID > 0)
                {
                    objTaskType = AppUI.EkTaskTypeRef;
                    coll = objTaskType.GetTaskTypeByID(objTask.TaskTypeID);
                    if (CollectionNotEmpty(coll))
                    {
                        sbDisplayTaskType.Append("<input type=\"hidden\" name=\"task_type\" value=\"" + coll["task_type_id"] + "\">");
                        sbDisplayTaskType.Append(coll["task_type_title"]);
                    }
                    else
                    {
                        CheckErrorString("Task Type: GetTaskTypeByID");
                    }
                }
                else
                {
                    sbDisplayTaskType.Append("[" + MsgHelper.GetMessage("dd not specified") + "]");
                }
                break;
        }

        if ("EditTask" == strTaskAction)
        {
            if ((objTask != null) & objTask.TaskTypeID > 0)
            {
                sbDisplayTaskType.Append("<script type=\"text/javascript\">SelectTaskTypeDropDown('" + strTaskAction + "Form" + "', " + objTask.TaskTypeID + ") ;</" + "script>");
            }
        }

        switch (strTaskAction)
        {
            case "AddTask":
            case "EditTask":
                if (!(Convert.ToInt64(objTask.TaskTypeID) == ((long)EkEnumeration.TaskType.BlogPostComment | (long)EkEnumeration.TaskType.TopicReply)))
                {
                    sbDisplayTaskType.Append(MakeTaskTypeLink(strTaskAction));
                }
                break;
        }

        sbDisplayTaskType.Append("</td>");
        sbDisplayTaskType.Append("</table>");
        sbDisplayTaskType.Append("</td>");
        sbDisplayTaskType.Append("</tr>");
        return sbDisplayTaskType.ToString();
    }
    public void ShowErrorString(string strTitle, string back_page)
    {
        string strResponse = null;
        strResponse = "" + "<table width=\"100%\" ID=\"err_Table\"> " + "<tr>" + (m_refStyle.GetTitleBar(strTitle)) + "</tr> " + "<tr>" + "<td class=\"ektronToolbar\">" + "<table ID=\"err_Table2\">" + "<tr>";

        if (IsBrowserIE())
        {
            strResponse = strResponse + m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "javascript:history.back();", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        }
        else
        {
			strResponse = strResponse + m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "tasks.aspx?action=" + back_page + "&is_child=" + is_child + "&task_type_id=" + Convert.ToInt64(Request.Form["task_type_id"]) + "&" + callBackPage, MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
        }
        strResponse = strResponse + "\t\t\t</tr>" + "\t\t</table>" + "\t</td>" + "</tr>" + "<tr>\t<td class=\"titlebar-error\">" + (ErrorString) + "</td>" + "</tr>" + "</table> ";
        Response.Write(strResponse);
    }
    public void ShowSuccess(string strTitle, object lngCategory, object lngType)
    {
        string strResponse = null;
        strResponse = "" + "<table width=\"100%\" ID=\"eTable\"> " + "<tr>" + (m_refStyle.GetTitleBar(strTitle)) + "</tr> " + "<tr>" + "\t<td class=\"ektronToolbar\">" + "\t\t<table ID=\"err_Table2\">" + "\t\t\t<tr>";
		strResponse = strResponse + m_refStyle.GetButtonEventsWCaption(AppUI.AppPath + "images/UI/Icons/back.png", "", MsgHelper.GetMessage("alt back button"), MsgHelper.GetMessage("btn back"), "onclick=\"parent.CloseTaskTypePage(true,parent.aAllCategory," + lngCategory + "," + lngType + ");\"", StyleHelper.BackButtonCssClass, true);
        strResponse = strResponse + "\t\t\t</tr>" + "\t\t</table>" + "\t</td>" + "</tr>" + "<tr>\t<td class=\"label\"></td>" + "</tr>" + "</table> ";
        strResponse = strResponse + "<script type=\"text/javascript\">parent.CloseTaskTypePage(true,parent.aAllCategory," + lngCategory + "," + lngType + ");</" + "script>";
        Response.Write(strResponse);
    }
    public void MakeTaskTypeArea()
    {
        if (IsBrowserIE())
        {
            Response.Write("<div allowtransparency=\"true\" id=\"TaskTypeOverLay\" style=\"position: absolute; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 2; background-color: transparent; \">");
            Response.Write("    <iframe allowtransparency=\"true\" id=\"TaskTypeOverLayChild\" name=\"TaskTypeOverLayChild\" frameborder=\"no\" marginheight=\"0\" marginwidth=\"0\" width=\"100%\" height=\"100%\" scrolling=\"no\" style=\"background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)\">");
            Response.Write("    </iframe>");
            Response.Write("</div>");
            Response.Write("<div id=\"TaskTypeFrameContainer\" style=\"position: absolute; top: 48px; left: 50px; width: 1px; height: 1px; display: none; z-index: 3; background-color: white; border-style: Outset\">");
            Response.Write("    <iframe id=\"TaskTypeChildPage\" name=\"TaskTypeChildPage\" frameborder=\"yes\" marginheight=\"0\" marginwidth=\"0\" width=\"100%\" height=\"100%\" scrolling=\"auto\">");
            Response.Write("    </iframe>");
            Response.Write("</div>");
        }
        else
        {
            Response.Write("<div allowtransparency=\"true\" id=\"TaskTypeOverLay\" style=\"position: absolute; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1; background-color: transparent; \">");
            Response.Write("</div>");
            Response.Write("<div id=\"TaskTypeFrameContainer\" style=\"position: absolute; top: 48px; left: 50px; width: 1px; height: 1px; display: none; z-index: 2; background-color: white; border-Style: Outset\">");
            Response.Write("    <iframe id=\"TaskTypeChildPage\" name=\"TaskTypeChildPage\" frameborder=\"yes\" marginheight=\"0\" marginwidth=\"0\" width=\"100%\" height=\"100%\" scrolling=\"auto\">");
            Response.Write("    </iframe>");
            Response.Write("</div>");
        }
    }
    #endregion
    #region JS
    private void GenerateJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("		function IsBrowserIE() " + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			// document.all is an IE only property" + Environment.NewLine);
        sbJS.Append("			return (document.all ? true : false);" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		" + Environment.NewLine);
        sbJS.Append("		function ShowPane(tabID) " + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			var arTab = new Array(\"dvContent\", \"dvSummary\", \"dvMetadata\", \"dvProperties\", \"dvComment\", \"dvTasks\", \"dvAlias\",\"dvSubscription\",\"dvCategories\",\"dvRoll\");" + Environment.NewLine);
        sbJS.Append("			" + Environment.NewLine);
        sbJS.Append("			if ((tabID == arTab[0]) && !IsBrowserIE()) " + Environment.NewLine);
        sbJS.Append("			{ " + Environment.NewLine);
        sbJS.Append("				window.location.reload(false);" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("			else" + Environment.NewLine);
        sbJS.Append("			{	" + Environment.NewLine);
        sbJS.Append("				var dvShow; //tab" + Environment.NewLine);
        sbJS.Append("				var _dvShow; //pane" + Environment.NewLine);
        sbJS.Append("				var dvHide;" + Environment.NewLine);
        sbJS.Append("				var _dvHide;" + Environment.NewLine);
        sbJS.Append("				" + Environment.NewLine);
        sbJS.Append("				for (var i=0; i < arTab.length; i++) {" + Environment.NewLine);
        sbJS.Append("					if (tabID == arTab[i]) {				" + Environment.NewLine);
        sbJS.Append("						dvShow = eval(\'document.getElementById(\"\' + arTab[i] + \'\");\');						" + Environment.NewLine);
        sbJS.Append("						_dvShow = eval(\'document.getElementById(\"_\' + arTab[i] + \'\");\');						" + Environment.NewLine);
        sbJS.Append("					} else {	" + Environment.NewLine);
        sbJS.Append("					" + Environment.NewLine);
        sbJS.Append("						dvHide = eval(\'document.getElementById(\"\' + arTab[i] + \'\");\');" + Environment.NewLine);
        sbJS.Append("						if (dvHide != null) {" + Environment.NewLine);
        sbJS.Append("							dvHide.className = \"tab_disabled\";" + Environment.NewLine);
        sbJS.Append("						}" + Environment.NewLine);
        sbJS.Append("						_dvHide = eval(\'document.getElementById(\"_\' + arTab[i] + \'\");\');" + Environment.NewLine);
        sbJS.Append("						if (_dvHide != null) {" + Environment.NewLine);
        sbJS.Append("							_dvHide.style.display = \"none\"; " + Environment.NewLine);
        sbJS.Append("						}" + Environment.NewLine);
        sbJS.Append("					}" + Environment.NewLine);
        sbJS.Append("				}" + Environment.NewLine);
        sbJS.Append("				_dvShow.style.display = \"block\"; " + Environment.NewLine);
        sbJS.Append("				dvShow.className = \"tab_actived\";" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("       var invalidFormatMsg = '" + MsgHelper.GetMessage("js: invalid date format error msg") + "';" + Environment.NewLine);
        sbJS.Append("       var invalidDateOnlyFormatMsg = 'Invalid Date Format.Please use the format dd-mmm-yyyy.Ex. 3-Mar-2001;'" + Environment.NewLine);
        sbJS.Append("       var invalidYearMsg = '" + MsgHelper.GetMessage("js: invalid year error msg") + "';" + Environment.NewLine);
        sbJS.Append("       var invalidMonthMsg = '" + MsgHelper.GetMessage("js: invalid month error msg") + "';" + Environment.NewLine);
        sbJS.Append("       var invalidDayMsg = '" + MsgHelper.GetMessage("js: invalid day error msg") + "';" + Environment.NewLine);
        sbJS.Append("       var invalidTimeMsg = '" + MsgHelper.GetMessage("js: invalid time error msg") + "';" + Environment.NewLine);
        sbJS.Append("       ecmMonths = '" + AppUI.GetEnglishMonthsAbbrev() + "';" + Environment.NewLine);
        sbJS.Append("       var jsDefaultContentLanguage = '" + MsgHelper.GetMessage("js: invalid date format error msg") + "';" + Environment.NewLine);
        sbJS.Append("       if ((typeof(top.GetEkDragDropObject)).toLowerCase() != 'undefined') {" + Environment.NewLine);
        sbJS.Append("           var dragDropFrame = top.GetEkDragDropObject();" + Environment.NewLine);
        sbJS.Append("           if (dragDropFrame != null) {" + Environment.NewLine);
        sbJS.Append("               dragDropFrame.location.href = 'blank.htm';" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);
        sbJS.Append("       if ((typeof(top.GetEkDragDropObject)).toLowerCase() != 'undefined') {" + Environment.NewLine);
        sbJS.Append("           top.HideDragDropWindow();" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);
        sbJS.Append("       var action_client = '" + Server.HtmlEncode(Request.QueryString["action"]) + "';" + Environment.NewLine);
        SelectAllCategory();
        if (CollectionNotEmpty(colAllCategory))
        {
            sbJS.Append("       var aAllCategory = new Array(" + colAllCategory.Count + ");" + Environment.NewLine);
            sbJS.Append("       var aAllExistingTaskType = new Array(" + colAllCategory.Count + ");" + Environment.NewLine);
            sbJS.Append("       var lngIndex;" + Environment.NewLine);
            int lngCategoryCount = 0;
            int lngCategoryIndex = 0;
            lngCategoryCount = colAllCategory.Count;
            for (int i = 0; i < lngCategoryCount; i++)
            {
                lngCategoryIndex = lngCategoryIndex + 1;
                Collection coll = (Collection)colAllCategory[lngCategoryIndex];
                Collection colAllTaskType = (Collection)coll["all_task_type"];
                sbJS.Append("       var aCategoryItem=new Array(4);" + Environment.NewLine);
                sbJS.Append("       var aExistingTaskType=new Array(2);" + Environment.NewLine);
                sbJS.Append("       aCategoryItem[0]='" + coll["task_type_id"] + "';" + Environment.NewLine);
                sbJS.Append("       aCategoryItem[1]='" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll["task_type_title"].ToString().Replace("'", "\"")) + "';" + Environment.NewLine);
                sbJS.Append("       aExistingTaskType[0]='[" + coll["task_type_id"] + "]';" + Environment.NewLine);
                sbJS.Append("       aExistingTaskType[1]='[" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll["task_type_title"].ToString().Replace("'", "\"")) + "]';" + Environment.NewLine);
                sbJS.Append("       var aAllTaskType=new Array(1);" + Environment.NewLine);
                sbJS.Append("       aAllTaskType[0]=null;" + Environment.NewLine);
                if (CollectionNotEmpty(colAllTaskType))
                {
                    int lngTypeIndex = 0;
                    int lngExistingIndex = 0;
                    for (int j = 1; j <= colAllTaskType.Count; j++)
                    {
                        Collection coll1 = (Collection)colAllTaskType[j];
                        if (!string.IsNullOrEmpty(coll1["active"].ToString()))
                        {
                            int activityTypeInt = 0;
                            int.TryParse(coll1["active"].ToString(), out activityTypeInt);
                            EkEnumeration.TaskTypeActivity activityType = (EkEnumeration.TaskTypeActivity)activityTypeInt;
                            if (activityType != EkEnumeration.TaskTypeActivity.FormDataTaskPrototype &&
                                activityType != EkEnumeration.TaskTypeActivity.Inactive)
                            {
                                sbJS.Append("       aAllTaskType[" + lngTypeIndex + "]=[\"" + coll1["task_type_id"] + "\",\"" + Ektron.Cms.Common.EkFunctions.HtmlEncode(coll1["task_type_title"].ToString()) + "\",true];" + Environment.NewLine);
                                lngTypeIndex = lngTypeIndex + 1;
                                lngExistingIndex = lngExistingIndex + 1;
                            }
                        }
                    }
                }
                sbJS.Append("       aCategoryItem[2]=aAllTaskType;" + Environment.NewLine);
                sbJS.Append("       aCategoryItem[3]=true;" + Environment.NewLine);
                sbJS.Append("       aCategoryItem[4] = '" + coll["active"] + "';" + Environment.NewLine);
                sbJS.Append("       aAllCategory[" + (lngCategoryIndex - 1) + "]=aCategoryItem;" + Environment.NewLine);
                sbJS.Append("       aAllExistingTaskType[" + (lngCategoryIndex - 1) + "]=aExistingTaskType;" + Environment.NewLine);
            }
        }
        else
        {
            sbJS.Append("       var aAllCategory=new Array(1);" + Environment.NewLine);
            sbJS.Append("       var aAllExistingTaskType=new Array(1);" + Environment.NewLine);
            sbJS.Append("       aAllCategory[0]=null;" + Environment.NewLine);
            sbJS.Append("       aAllExistingTaskType[0]=null;" + Environment.NewLine);
        }
        sbJS.Append("       function EkDTCompareDates(objLowDateField, objHighDateField)" + Environment.NewLine);
        sbJS.Append("       {" + Environment.NewLine);
        sbJS.Append("           var dateLow = $ektron(objLowDateField).val();" + Environment.NewLine);
        sbJS.Append("           var aDateLow = dateLow.split(\"-\");" + Environment.NewLine);
        sbJS.Append("           var myLowerDate = new Date(aDateLow[0],  aDateLow[1] - 1, aDateLow[2]);" + Environment.NewLine);
        sbJS.Append("           var dateHigh = $ektron(objHighDateField).val();" + Environment.NewLine);
        sbJS.Append("           var aDateHigh = dateHigh.split(\"-\");" + Environment.NewLine);
        sbJS.Append("           var myHighDate = new Date(aDateHigh[0],  aDateHigh[1] - 1, aDateHigh[2]);" + Environment.NewLine);
        sbJS.Append("           if (myLowerDate > myHighDate)" + Environment.NewLine);
        sbJS.Append("           {" + Environment.NewLine);
        sbJS.Append("               return false;" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("           return true;" + Environment.NewLine); 
        sbJS.Append("       }" + Environment.NewLine);
        sbJS.Append("       function LoadChildPage() {" + Environment.NewLine);
        sbJS.Append("           var languageID;" + Environment.NewLine);
        if (EnableMultilingual == 1)
        {
            sbJS.Append("       languageID = document.getElementById('language').value;" + Environment.NewLine);
        }
        else
        {
            sbJS.Append("       languageID = " + ContentLanguage + ";" + Environment.NewLine);
        }

        sbJS.Append("           PopUpWindow('SelectCreateContent.aspx?FolderID=0&LangType=' + languageID +'&browser=1&for_tasks=1','SelectContent', 490,500,1,1);" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);
        ltrGenerateJS.Text = sbJS.ToString();
    }
    #endregion
    protected void FillLiterals()
    {
        ltrAppPath.Text = AppUI.AppPath;
        ltrTitleRequired.Text = MsgHelper.GetMessage("js: alert title required");
        ltrNewTaskCatReq.Text = MsgHelper.GetMessage("alt new task category required");
        ltrAllTaskCatMax.Text = MsgHelper.GetMessage("alt task category max");
        ltrTaskTypeReq.Text = MsgHelper.GetMessage("alt task type required");
        ltrTaskTypeMax.Text = MsgHelper.GetMessage("alt task type max");
        ltrTaskTypeDescMax.Text = MsgHelper.GetMessage("alt task type description max");
        ltrTaskTypeReq2.Text = MsgHelper.GetMessage("alt task type required");
        ltrTaskTypeMax2.Text = MsgHelper.GetMessage("alt task type max");
        ltrTaskTypeDescMax2.Text = MsgHelper.GetMessage("alt task type description max");
        ltrMarkInActive.Text = MsgHelper.GetMessage("alt mark inactive");
        ltrTaskTypeReq3.Text = MsgHelper.GetMessage("alt task category required");
        ltrTaskTypeMax3.Text = MsgHelper.GetMessage("alt task type max");
        ltrEncode.Text = Ektron.Cms.Common.EkFunctions.UrlEncode(" & ");
        ltrTaskTypeExists.Text = MsgHelper.GetMessage("com: task type exists");
        ltrTaskTypeExists2.Text = MsgHelper.GetMessage("com: task type exists");
        ltrTaskExistsSameParent.Text = MsgHelper.GetMessage("com: task exists same parent");
        ltrTaskExistsSameParent2.Text = MsgHelper.GetMessage("com: task exists same parent");
        ltrDeleteTaskType.Text = MsgHelper.GetMessage("alt delete task type");
        ltrSelectOneTaskType.Text = MsgHelper.GetMessage("alt select one task type");
        ltrDDNotSpecified.Text = MsgHelper.GetMessage("dd not specified");
        ltrDDNotSpecified2.Text = MsgHelper.GetMessage("dd not specified");
        ltrGoBackNoSave.Text = MsgHelper.GetMessage("alt go back no save");
        ltrDDAll.Text = MsgHelper.GetMessage("dd all");
        ltrDDNotSpecified3.Text = MsgHelper.GetMessage("dd not specified");
        ltrvalidc.Text= MsgHelper.GetMessage("js Please select a valid content");
        ltrtaskc.Text = MsgHelper.GetMessage("js Task must assigned");
        ltrStartDatec.Text = MsgHelper.GetMessage("js start date later");
        ltrDeleteTaskc.Text = MsgHelper.GetMessage("js delete task");
        ltrAtleastTaskc.Text = MsgHelper.GetMessage("js atleast one task");
        ltrPurgeTaskc.Text = MsgHelper.GetMessage("js purge task");
        ltrAssignTaskc.Text = MsgHelper.GetMessage("js assign tasks");
        ltrChangeTaskc.Text = MsgHelper.GetMessage("js change tasks");        
    }
}

