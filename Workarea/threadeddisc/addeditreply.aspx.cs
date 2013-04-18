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
using Ektron.Cms.Workarea;

public partial class threadeddisc_addeditreply : workareabase
{

    //Dim ctlEditor As New Ektron.Cms.Controls.HtmlEditor
    Ektron.ContentDesignerWithValidator ctlEditor;
    long m_iTopicID = 0;
    long m_iForumID = 0;
    long m_iBoardID = 0;
    PermissionData security_data;
    DiscussionTopic _Topic;
    DiscussionBoard _Board;
    Ektron.Cms.Content.EkContent m_refContent;
    Ektron.Cms.Content.EkTask m_reftask;
    bool bIsTopic = false;
    bool closeOnFinish = false;
    SettingsData settings_data;
    SiteAPI siteApi = new SiteAPI();

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        m_refContent = new Ektron.Cms.Content.EkContent(m_refContentApi.RequestInformationRef);
        m_reftask = m_refContentApi.EkTaskRef;
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("../controls/Editor/ContentDesignerWithValidator.ascx");
        pnl_message_editor.Controls.Add(ctlEditor);
        ctlEditor.Visible = false;
        ctlEditor.ID = "content_html";
        ctlEditor.ToolsFile = m_refContentApi.ApplicationPath + "ContentDesigner/configurations/InterfaceBlog.aspx?EmoticonSelect=1&WMV=1";
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
			Utilities.ValidateUserLogin();
            if (!string.IsNullOrEmpty(Request.QueryString["topicid"]))
            {
                m_iTopicID = Convert.ToInt64(Request.QueryString["topicid"]);
            }
            if (Request.QueryString["type"] == "topic")
            {
                bIsTopic = true;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["forumid"]))
            {
                m_iForumID = Convert.ToInt64(Request.QueryString["forumid"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["boardid"]) && !string.IsNullOrEmpty(Request.QueryString["boardid"]))
            {
                m_iBoardID = Convert.ToInt64(Request.QueryString["boardid"]);
                if (m_refContent.IsIPBanned(m_iBoardID, Request.ServerVariables["REMOTE_ADDR"]))
                {
                    throw (new Exception(base.GetMessage("msg ip ban")));
                }
            }

            if (m_refContent.RequestInformation.IsMembershipUser == 1)
            {
                throw (new Exception(base.GetMessage("msg login cms user")));
            }

            if (!string.IsNullOrEmpty(Request.QueryString["close"]))
            {
                closeOnFinish = true;
            }


            if (Page.IsPostBack)
            {
                m_sPageAction = Request.Form[hdn_action.UniqueID];
                m_iTopicID = Convert.ToInt64(Request.Form[hdn_topicid.UniqueID]);
                m_iID = Convert.ToInt64(Request.Form[hdn_replyid.UniqueID]);
                switch (base.m_sPageAction)
                {
                    case "add":
                        Process_Add();
                        break;
                    case "edit":
                        Process_Edit();
                        break;
                }
            }
            else
            {
                settings_data = siteApi.GetSiteVariables(siteApi.UserId);
                switch (base.m_sPageAction)
                {
                    case "add":
                        Display_Add();
                        break;
                    case "edit":
                        Display_Edit();
                        break;
                }
            }
            hdn_action.Value = m_sPageAction;
            hdn_topicid.Value = m_iTopicID.ToString();
            hdn_forumid.Value = m_iForumID.ToString();
            hdn_replyid.Value = m_iID.ToString();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #region Display

    private void Display_Add()
    {
        bool bModerate = false;
        //If security_data.CanAdd = False Then
        //    Throw New Exception("User does not have permission")
        //End If
        if (this.m_iTopicID == 0)
        {
            this.m_iTopicID = m_iID;
            m_iID = 0;
        }

        if (this.m_iTopicID != 0 && this.m_iTopicID != -1)
        {
            _Board = m_refContent.GetTopicbyIDForEdit(this.m_iTopicID.ToString());
            security_data = m_refContentApi.LoadPermissions(this.m_iTopicID, "folder", 0);
            if (!(_Board == null) && _Board.Forums.Length > 0 && !(_Board.Forums[0].Topics == null) && (_Board.Forums[0].Topics.Length > 0))
            {
                bModerate = System.Convert.ToBoolean(_Board.Forums[0].ModerateComments);
                _Topic = _Board.Forums[0].Topics[0];
                ltr_topic_data.Text = "(" + this.m_iTopicID.ToString() + ") " + _Topic.Title;
            }
            else
            {
                throw (new Exception(base.GetMessage("error: content does not exist") + " \"" + this.m_iTopicID + "\"."));
            }
        }
        base.SetTitleBarToMessage("atl btn add reply");

		if (closeOnFinish != true)
		{
			base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iForumID.ToString() + "&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage + "&ContType=" + Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply) + "&contentid=" + m_iTopicID));
		}
		
		base.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        
        base.AddHelpButton("AddReply");

        tr_createdBy.Visible = false;
        tr_createdDate.Visible = false;

        SetLabels();
        if (bModerate == true && (security_data.IsAdmin == false && security_data.CanAddToImageLib == false))
        {
            drp_state.SelectedIndex = 0; // pending
            this.drp_state.Enabled = false;
            pnl_message_editor.Controls.Add(new LiteralControl("<input type=\"hidden\" name=\"replystate\" value=\"5\" />"));
        }
        else if (bModerate == false && (security_data.IsAdmin == false && security_data.CanAddToImageLib == false))
        {
            drp_state.SelectedIndex = 1; // approved
            this.drp_state.Enabled = false;
            pnl_message_editor.Controls.Add(new LiteralControl("<input type=\"hidden\" name=\"replystate\" value=\"7\" />"));
        }
        else
        {
            drp_state.SelectedIndex = 1; // approved
        }

        _Board = m_refContent.GetTopicbyIDForEdit(m_iTopicID.ToString());

        ctlEditor.FolderId = m_iForumID;
        ctlEditor.SetPermissions(security_data);
        ctlEditor.AllowFonts = true;
        if (_Board != null && _Board.StyleSheet.Length > 0)
        {
            ctlEditor.Stylesheet = m_refContentApi.SitePath + _Board.StyleSheet;
        }
        ctlEditor.Visible = true;
        RenderJS();
        SuppressTitle();
    }

    public void Display_Edit()
    {
        ContentData content_data;
        m_reftask = m_reftask.GetTaskByID(m_iID);
        content_data = m_refContentApi.GetContentById(m_iTopicID, 0);
        security_data = m_refContentApi.LoadPermissions(m_iTopicID, "content", 0);

        if (this.m_iTopicID != 0 && this.m_iTopicID != -1)
        {
            ltr_topic_data.Text = "(" + this.m_iTopicID.ToString() + ") " + content_data.Title;
        }
        if (this.bIsTopic == true)
        {
            base.SetTitleBarToMessage("edit topicstarter msg");
        }
        else
        {
            base.SetTitleBarToMessage("lbl edit topicreply");
        }
        //If security_data.CanEdit Then
		base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + content_data.FolderId.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iTopicID.ToString()));
		base.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        //End If content.aspx?id=77&action=ViewContentByCategory&LangType=1033&ContType=13&contentid=452
        base.AddHelpButton("EditReply");

        SetLabels();
        if (this.bIsTopic == true)
        {
            tr_state.Visible = false;
        }
        else
        {
            if (m_reftask.State == (Convert.ToInt64(EkEnumeration.TaskState.Pending.GetHashCode())).ToString())
            {
                drp_state.SelectedIndex = 0;
            }
            else if (m_reftask.State == Convert.ToInt64(EkEnumeration.TaskState.Completed).ToString())
            {
                drp_state.SelectedIndex = 1;
            }
        }

        ltr_created_data.Text = m_reftask.DisplayDateCreated;
        ltr_created_by_data.Text = m_reftask.CommentDisplayName;

        if (security_data.CanEdit || m_reftask.CreatedByUserID == m_refContentApi.UserId)
        {
            DiscussionBoard board_data = null;
            Ektron.Cms.Content.EkContent brContent = m_refContentApi.EkContentRef;
            board_data = brContent.GetTopicbyIDForEdit(m_iTopicID.ToString());
            ctlEditor.FolderId = m_iForumID;
            ctlEditor.SetPermissions(security_data);
            ctlEditor.AllowFonts = true;
            if (board_data != null && board_data.StyleSheet.Length > 0)
            {
                ctlEditor.Stylesheet = m_refContentApi.SitePath + board_data.StyleSheet;
            }
            ctlEditor.Visible = true;
            ctlEditor.Content = m_reftask.Description;
            SuppressTitle();
            RenderJS();
        }
        else
        {
            //txt_topic_title.Enabled = False
            pnl_message_editor.Controls.Add(new LiteralControl(m_reftask.Description));
        }
    }

    #endregion

    #region Process
    private void Process_Add()
    {
        string strContent = "";
        if (m_refContentApi.RequestInformationRef.ContentLanguage > 0)
        {
            m_reftask.ContentLanguage = m_refContentApi.RequestInformationRef.ContentLanguage;
            m_reftask.LanguageID = m_refContentApi.RequestInformationRef.ContentLanguage;
        }
        else
        {
            m_reftask.ContentLanguage = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
            m_reftask.LanguageID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
        }
        m_reftask.ContentID = m_iTopicID;
        _Board = m_refContent.GetTopicbyIDForEdit(this.m_iTopicID.ToString());
        m_reftask.AssignedByUserID = m_refContentApi.RequestInformationRef.UserId.ToString();
        m_reftask.CreatedByUserID = m_refContentApi.RequestInformationRef.UserId;
        m_reftask.DateCreated = (string)(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
        m_reftask.TaskTypeID = Convert.ToInt64(EkEnumeration.TaskType.TopicReply);
        if (!string.IsNullOrEmpty(Request.Form[drp_state.UniqueID]))
        {
            m_reftask.State = Request.Form[drp_state.UniqueID];
            m_reftask.ParentID = m_iID;
        }
        else
        {
            m_reftask.State = Request.Form["replystate"];
            if (m_reftask.State == "5")
            {
                if (m_iID == 0)
                {
                    m_reftask.ParentID = -1;
                }
                else
                {
                    m_reftask.ParentID = m_iID * -1;
                }
            }
            else
            {
                m_reftask.ParentID = m_iID;
            }
        }
        strContent = (string)ctlEditor.Content;
        if (strContent == "")
        {
            throw (new Exception(base.GetMessage("js: null text message") + "."));
        }
        else
        {
            strContent = this.m_refContentApi.ReplaceWordsForBoardPosts(strContent, _Board.Id);
            m_reftask.Description = strContent;
            m_reftask.TaskTitle = "TopicReply";
            m_reftask.ImpersonateUser = true;
            m_reftask.CommentDisplayName = ""; //m_refContentApi.RequestInformationRef.LoggedInUsername
            m_reftask.CommentEmail = "";
            m_reftask.CommentURI = Request.ServerVariables["REMOTE_ADDR"];
            m_reftask.HostURL = Request.ServerVariables["HTTP_HOST"];
            m_reftask.URLpath = this.m_refContentApi.SitePath + _Board.TemplateFileName;
            m_reftask.AddTask();
            if (closeOnFinish == true)
            {
                Response.Redirect("../close.aspx", false);
            }
            else
            {
                Response.Redirect((string)("../content.aspx?id=" + m_iForumID.ToString() + "&action=ViewContentByCategory&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage.ToString() + "&ContType=" + Convert.ToInt64(EkEnumeration.TaskType.TopicReply) + "&contentid=" + m_iTopicID.ToString()), false);
            }
        }
    }

    private void Process_Edit()
    {
        string strContent = (string)ctlEditor.Content;
        if (m_iBoardID > 0)
            strContent = this.m_refContentApi.ReplaceWordsForBoardPosts(strContent, m_iBoardID);
        m_reftask = m_reftask.GetTaskByID(m_iID);
        if (this.bIsTopic == true)
        {
            m_reftask.State = EkEnumeration.TaskState.Completed.ToString();
        }
        else
        {
            m_reftask.State = Context.Request.Form[drp_state.UniqueID];
        }
        m_reftask.Description = strContent;
        m_reftask.ImpersonateUser = true;

        m_reftask.UpdateTask();

        if (closeOnFinish == true)
        {
            Response.Redirect("../close.aspx", false);
        }
        else
        {
            Response.Redirect((string)("../content.aspx?id=" + m_iForumID.ToString() + "&action=ViewContentByCategory&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage.ToString() + "&ContType=" + Convert.ToInt64(EkEnumeration.TaskType.TopicReply) + "&contentid=" + m_iTopicID.ToString()), false);
        }
    }
    #endregion

    #region Private Helpers

    private void SuppressTitle()
    {
        ltr_js.Text += "<style type=\"text/css\">" + Environment.NewLine + "#content_html_ContentInfo" + Environment.NewLine + "{" + Environment.NewLine + "    display:none;" + Environment.NewLine + "}" + Environment.NewLine + "</style>";
    }

    private void SetLabels()
    {
        ltr_topic.Text = base.GetMessage("topic text");
        ltr_state.Text = base.GetMessage("lbl state");
        ltr_desc.Text = base.GetMessage("lbl desc");
        ltr_created.Text = base.GetMessage("content dc label");
        ltr_created_by.Text = base.GetMessage("created by label");

        drp_state.Items.Add(new ListItem(base.GetMessage("lbl pending"), "5"));
        drp_state.Items.Add(new ListItem(base.GetMessage("lbl approved"), "7"));
    }

    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);
        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("        document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }
    #endregion

}


