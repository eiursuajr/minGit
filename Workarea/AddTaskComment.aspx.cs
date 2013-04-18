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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Content;
using Ektron.Cms.Common;

public partial class AddTaskComment : System.Web.UI.Page
{
    protected Ektron.ContentDesignerWithValidator ctlEditor;
    protected Ektron.Cms.Content.EkTask taskObj;
    protected SiteAPI m_siteRef = new SiteAPI();
    protected string AppName;
    protected void Page_Init(Object sender, EventArgs e)
    {
        ltrScript.Text = (new StyleHelper()).GetClientScript();
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("Controls/Editor/ContentDesignerWithValidator.ascx");
        ltr_sig.Controls.Add(ctlEditor);
        ctlEditor.ID = "commenttext";
        ctlEditor.AllowScripts = false;
        ctlEditor.Height = new Unit(350, UnitType.Pixel);
        ctlEditor.Width = new Unit(90, UnitType.Percentage);
        ctlEditor.Stylesheet = m_siteRef.AppPath + "csslib/ewebeditprostyles.css";
        ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
        ctlEditor.AllowFonts = true;
        ctlEditor.ShowHtmlMode = false;
    }
    protected void Page_Load(System.Object sender, System.EventArgs e)
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        EkContent cContObj = new EkContent(AppUI.RequestInformationRef);
        taskObj = new EkTask(AppUI.RequestInformationRef);
        long cid;
        string[] tasksArray;
        int lCounter;
        string RefType;
        long CurrentUserID;
        string AppPath;
        string AppImgPath;
        string SitePath;
        string AppeWebPath;
        long CommentKeyId = 0;
        long CommentId = 0;
        string Action;
        object ActionType;
        object IsMac;
        object platform;
        bool Flag;
        object retVal;
        object CommentText;
        object NS4;
        object OrderBy;
        object iMaxContLength;
        object localeFileString;
        object var1;
        object var2;
        string taskIDs;
        object height;
        object width;
        int EnableMultilingual;
        int ContentLanguage;
        Ektron.Cms.Common.EkMessageHelper MsgHelper;
        System.Text.StringBuilder sbScript = new System.Text.StringBuilder();

        MsgHelper = AppUI.EkMsgRef;
        AppPath = AppUI.AppPath;
        AppImgPath = AppUI.AppImgPath;
        SitePath = AppUI.SitePath;
        AppeWebPath = AppUI.AppeWebPath;
        AppPath = AppUI.AppPath;
        AppName = AppUI.AppName;
        EnableMultilingual = AppUI.EnableMultilingual;
        ContentLanguage = 1033; //set default value
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (Convert.ToBoolean(AppUI.RequestInformationRef.IsMembershipUser))
        {
            Response.Redirect("reterror.aspx?info=" + MsgHelper.GetMessage("msg login cms user"), false);
            return;
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

        platform = Request.ServerVariables["HTTP_USER_AGENT"];
        if (platform.ToString().IndexOf("Windows") + 1 > 0)
        {
            IsMac = 0;
        }
        else
        {
            IsMac = 1;
        }

        RefType = "T";
        Flag = false;
        iMaxContLength = 65000;
        localeFileString = "0000";
        var1 = Request.ServerVariables["SERVER_NAME"];
        if (!string.IsNullOrEmpty(Request.QueryString["commentkey_id"]))
        {
            CommentKeyId = Convert.ToInt64(Request.QueryString["commentkey_id"]);
        }
        Action = Request.QueryString["action"];
        ActionType = Request.QueryString["ty"];
        OrderBy = Request.QueryString["orderby"];
        cid = Convert.ToInt64(Request.QueryString["id"]);
        if (!string.IsNullOrEmpty(Request.QueryString["Comment_Id"]))
        {
            CommentId = Convert.ToInt64(Request.QueryString["Comment_Id"]);
        }
        if (!string.IsNullOrEmpty((Request.QueryString["height"])))
        {
            height = Convert.ToDouble(Request.QueryString["height"]);
        }
        if (!string.IsNullOrEmpty((Request.QueryString["width"])))
        {
            width = Convert.ToDouble(Request.QueryString["width"]);
        }
        lCounter = 0;
        CurrentUserID = AppUI.UserId;

	    ltrComments.Text = MsgHelper.GetMessage("lbl task comment") + ":";
        if (Request.QueryString["action"] != null && Request.QueryString["action"].ToString().ToLower() == "declinecontentaction")
            ltrComments.Text = MsgHelper.GetMessage("reason to decline");

        cContObj = AppUI.EkContentRef;

        if (Request.ServerVariables["http_user_agent"].ToString().IndexOf("Mozilla") + 1 > 0 && Request.ServerVariables["http_user_agent"].ToString().IndexOf("4.7") + 1 > 0 && Request.ServerVariables["http_user_agent"].ToString().IndexOf("GECKO") < 0)
        {
            NS4 = true;
        }
        else
        {
            NS4 = false;
        }

        var2 = cContObj.GetEditorVariablev2_0(0, "tasks");
        ctlEditor.Validate();
        if (Action == "Add" && ctlEditor.IsValid)
        {
            CommentText = this.ctlEditor.Content;
            if (cid != 0)
            {
                //Get all tasks associated with the content and add same comment
                taskObj = AppUI.EkTaskRef;
                object strStates;
                strStates = EkEnumeration.TaskState.NotStarted.ToString() + "," + EkEnumeration.TaskState.Active.ToString() + "," + EkEnumeration.TaskState.AwaitingData.ToString() + "," + EkEnumeration.TaskState.OnHold.ToString() + "," + EkEnumeration.TaskState.Pending.ToString() + "," + EkEnumeration.TaskState.Reopened.ToString();
                taskIDs = taskObj.GetTaskIDs(cid, strStates, -1, (int)EkEnumeration.CMSTaskItemType.TasksByStateAndContentID);

                if (taskIDs != "")
                {
                    tasksArray = Strings.Split(taskIDs.ToString(), ",", -1, 0);
                    while (lCounter <= (tasksArray.Length - 1))
                    {
                        retVal = cContObj.AddComment(Convert.ToInt64(CommentKeyId), Convert.ToInt64(CommentId), Convert.ToInt64(tasksArray.GetValue(lCounter)), RefType, CurrentUserID, Strings.Replace(CommentText.ToString(), "\'", "\'\'", 1, -1, 0));
                        lCounter++;
                    }
                }

            }
            Flag = true;
        }
        if (true == Flag)
        {
            sbScript.Append("<script language=\"JavaScript\" type=\"text/javascript\">" + "\r\n");
            sbScript.Append("<!--");
            sbScript.Append("if (IsBrowserIE())");
            sbScript.Append("{");
            sbScript.Append("   parent.ReturnChildValue(\"action=\" + document.getElementById(\"actionName\").value + \"&id=\" + document.getElementById(\"cid\").value + \"&fldid=\" + document.getElementById(\"fldid\").value + \"&page=\" + document.getElementById(\"page\").value );");
            sbScript.Append("}");
            sbScript.Append("else");
            sbScript.Append("{");
            sbScript.Append("   top.opener.ReturnChildValue(\"action=\" + document.getElementById(\"actionName\").value + " + ID == " + document.getElementById(\"cid\").value + \"&fldid=\" + document.getElementById(\"fldid\").value + \"&page=\" + document.getElementById(\"page\").value );");
            sbScript.Append("   close();");
            sbScript.Append("}");
            sbScript.Append("//-->");
            sbScript.Append("</script>" + "\r\n");
            ClosePanel.Text = sbScript.ToString();
        }

        if ((Request.QueryString["action"]) == "Add")
        {
            actionName.Value = Request.QueryString["actionName"];
        }
        else
        {
            actionName.Value = Request.QueryString["action"];
        }

        this.ctlEditor.AllowFonts = true;
        ctlEditor.ErrorMessage = MsgHelper.GetMessage("content size exceeded");
        ctlEditor.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(System.Convert.ToInt32(iMaxContLength));
    }
}

