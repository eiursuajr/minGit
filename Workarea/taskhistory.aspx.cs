using System;
using System.Text;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.UI.CommonUI;

public partial class taskhistory : System.Web.UI.Page
{
    StyleHelper _helper = new StyleHelper();
    ApplicationAPI appUI = new ApplicationAPI();
    Ektron.Cms.Content.EkTask taskObj;
    EkTaskHistory taskHistoryObj;
    Ektron.Cms.Content.EkTaskHistoryCol taskHistoryCollection = new Ektron.Cms.Content.EkTaskHistoryCol();
    protected EkMessageHelper MsgHelper;
    protected object taskHistoryItem;
    protected long CurrentUserID;
    protected string action = string.Empty;
    protected long TaskID = 0;
    protected string StartTime = string.Empty;
    protected string EndTime = string.Empty;
    protected string OrderBy = string.Empty;
    protected string TaskTitle;
    protected int ContentLanguage = -1;
    protected int i = 0;
    protected int EnableMultilingual;
    protected string AppPath;
    protected string AppImgPath;
    protected string SitePath;
    protected string AppeWebPath;
    protected string AppName;
    protected void Page_Init(Object sender, EventArgs e)
    {
        ltrScript.Text = _helper.GetClientScript();
    }
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        MsgHelper = new EkMessageHelper(appUI.RequestInformationRef);
        if (appUI.RequestInformationRef.IsMembershipUser == 1 || appUI.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(appUI.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(MsgHelper.GetMessage("msg login cms user")), false);
            return;
        }
        taskObj = new EkTask(appUI.RequestInformationRef);
        taskHistoryObj = new EkTaskHistory(appUI.RequestInformationRef);
        CurrentUserID = appUI.UserId;
        EnableMultilingual = appUI.EnableMultilingual;
        AppPath = appUI.AppPath;
        AppImgPath = appUI.AppImgPath;
        SitePath = appUI.SitePath;
        AppeWebPath = appUI.AppeWebPath;
        AppName = appUI.AppName;
        if(!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            appUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if(!string.IsNullOrEmpty(appUI.GetCookieValue("LastValidLanguageID")))
            {
                ContentLanguage = Convert.ToInt32(appUI.GetCookieValue("LastValidLanguageID"));
            }
        }
        appUI.ContentLanguage = ContentLanguage;
        if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
        {
            TaskID = Convert.ToInt64(Request.QueryString["tid"]);
        }
        if(!string.IsNullOrEmpty(Request.QueryString["starttime"]))
        {
            StartTime = Request.QueryString["starttime"].ToString();
        }
        if(!string.IsNullOrEmpty(Request.QueryString["endtime"]))
        {
            EndTime = Request.QueryString["endtime"].ToString();
        }
        if(!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            action = Request.QueryString["action"].ToString();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["title"]))
        {
            TaskTitle = Request.QueryString["title"].ToString();
        }
        else
        {
            TaskTitle = taskObj.GetTaskByID(TaskID).TaskTitle;
        }
        if(!string.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            OrderBy = Request.QueryString["orderby"].ToString();
        }
        taskHistoryObj = appUI.EkTaskhistoryRef;

        if (action == "ID")
        {
            taskHistoryCollection = (Ektron.Cms.Content.EkTaskHistoryCol)taskHistoryObj.GetTaskHistoryForTaskID(TaskID, OrderBy);
            ltrTitleBar.Text = "View Task History for " + TaskTitle;
        }
        else
        {
            taskHistoryCollection = (Ektron.Cms.Content.EkTaskHistoryCol)taskHistoryObj.GetTaskHistoryForTime(StartTime, EndTime, OrderBy);
            ltrTitleBar.Text = "View Task History for " + TaskTitle + " from Start time " + StartTime + " to End time " + EndTime;
        }
        if (taskHistoryCollection != null && taskHistoryCollection.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < taskHistoryCollection.Count; i++)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + appUI.GetInternationalDateOnly(taskHistoryCollection.get_Item(i).ChangeDate) + "</td>");
                sb.Append("<td>" + taskHistoryCollection.get_Item(i).InitiatorName + "</td>");
                sb.Append("<td>" + taskHistoryCollection.get_Item(i).Activity + "</td>");
                sb.Append("<td>" + taskHistoryCollection.get_Item(i).Instruction + "</td>");
                sb.Append("</tr>");
            }
            ltrTasks.Text = sb.ToString();
        }

        this.Page.Title = AppName + " TaskHistory";

        RegisterResources();
    }

    protected void RegisterResources()
    {
        Packages.Ektron.Workarea.Core.Register(this);
    }

}
	
