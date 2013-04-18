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

public partial class blogs_addeditcomment : workareabase
{
    Ektron.Cms.Controls.HtmlEditor ctlEditor = new Ektron.Cms.Controls.HtmlEditor();
    private Ektron.Cms.Content.EkContent m_refContent;
    private Ektron.Cms.Content.EkTask m_reftask;
    long m_iPostID = 0;
    long m_iBlogID = 0;
    PermissionData security_data;
    bool closeOnFinish = false;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        m_refContent = new Ektron.Cms.Content.EkContent(m_refContentApi.RequestInformationRef);
        m_reftask = m_refContentApi.EkTaskRef;
		Utilities.ValidateUserLogin();
        RegisterResources();
        if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
        {
            m_iPostID = Convert.ToInt64(Request.QueryString["contentid"]);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["blogid"]))
        {
            m_iBlogID = Convert.ToInt64(Request.QueryString["blogid"]);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["close"]))
        {
            closeOnFinish = true;
        }

        if (Page.IsPostBack)
        {
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
    }

    #region Display

    private void Display_Add()
    {
        Collection cConts = new Collection();
        UserData udME = new UserData();
        UserAPI uaUser = new UserAPI();
        if (this.m_refContentApi.UserId > 0)
        {
            udME = uaUser.GetUserById(this.m_refContentApi.UserId, false, false);
        }

        if (this.m_iPostID == 0)
        {
            this.m_iPostID = m_iID;
            m_iID = 0;
        }

        if (this.m_iPostID != 0 && this.m_iPostID != -1)
        {
            cConts = m_refContent.GetContentByIDv2_0(this.m_iPostID);
            if (cConts.Count == 0)
            {
                throw (new Exception(base.GetMessage("error: post does not exist") + "."));
            }
            else
            {
                ltr_post_data.Text = "(" + this.m_iPostID.ToString() + ") " + cConts["ContentTitle"];
            }
        }

        base.SetTitleBarToMessage("btn comment add");

		if (closeOnFinish != true)
		{
            base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iBlogID.ToString() + "&LangType=" + m_refContentApi.RequestInformationRef.ContentLanguage + "&ContType=" + ((int)Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply).ToString() + "&contentid=" + m_iPostID));
		}

        base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt save comment", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        
        base.AddHelpButton("AddComment");
        txt_displayname.Text = udME.DisplayName;
        txt_email.Text = udME.Email;
        txt_url.Text = "http://";
        SetLabels();
        rb_approved.Checked = true;
        RenderJS();
    }

    public void Display_Edit()
    {
        ContentData content_data;
        m_reftask = m_reftask.GetTaskByID(m_iID);
        content_data = m_refContentApi.GetContentById(m_iPostID, 0);
        security_data = m_refContentApi.LoadPermissions(m_iPostID, "content", 0);
        if (this.m_iPostID != 0 && this.m_iPostID != -1)
        {
            ltr_post_data.Text = "(" + this.m_iPostID.ToString() + ") " + content_data.Title;
        }

        base.SetTitleBarToMessage("lbl edit comment");
		base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + content_data.FolderId.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&LangType=" + m_refContentApi.ContentLanguage + "&contentid=" + m_iPostID.ToString()));
		base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt save comment", "btn save", "OnClick=\"javascript:SubmitForm();return false;\"", StyleHelper.SaveButtonCssClass, true);
        base.AddHelpButton("EditComment");
        SetLabels();
        if (m_reftask.State == EkEnumeration.TaskState.Pending.GetHashCode().ToString())
        {
            rb_pending.Checked = true;
        }
        else if (m_reftask.State == EkEnumeration.TaskState.Completed.GetHashCode().ToString())
        {
            rb_approved.Checked = true;
        }

        txt_displayname.Text = m_reftask.CommentDisplayName;
        txt_email.Text = m_reftask.CommentEmail;
        txt_url.Text = m_reftask.CommentURI;
        txt_comment.Text = m_reftask.Description;
        RenderJS();
    }

    #endregion

    #region Process

    private void Process_Add()
    {
        if (ContentLanguage > 0)
        {
            m_reftask.ContentLanguage = ContentLanguage;
            m_reftask.LanguageID = ContentLanguage;
        }
        else
        {
            m_reftask.ContentLanguage = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
            m_reftask.LanguageID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
        }

        m_reftask.ContentID = m_iPostID;
        m_reftask.AssignedByUserID = m_refContentApi.RequestInformationRef.UserId.ToString();
        m_reftask.CreatedByUserID = m_refContentApi.RequestInformationRef.UserId;
        m_reftask.DateCreated = (string)(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
        m_reftask.TaskTypeID = (long)EkEnumeration.TaskType.BlogPostComment;
        m_reftask.CommentDisplayName = Context.Request.Form[txt_displayname.UniqueID];
        m_reftask.CommentEmail = Context.Request.Form[txt_email.UniqueID];
        if (Context.Request.Form[txt_url.UniqueID].ToLower() == "http://")
        {
            m_reftask.CommentURI = "";
        }
        else
        {
            m_reftask.CommentURI = Ektron.Cms.Common.EkFunctions.FixExternalHyperlink(Context.Request.Form[txt_url.UniqueID]);
        }

        if (rb_pending.Checked == true)
        {
            m_reftask.State = EkEnumeration.TaskState.Pending.ToString();
        }
        else
        {
            m_reftask.State = EkEnumeration.TaskState.Completed.ToString();
        }

        m_reftask.Description = Context.Request.Form[txt_comment.UniqueID];
        m_reftask.TaskTitle = "BlogComment";
        m_reftask.ImpersonateUser = true;
        m_reftask.AddTask();
        if (closeOnFinish == true)
        {
            Response.Redirect("../close.aspx", false);
        }
        else
        {
            Response.Redirect("../content.aspx?id=" + m_iBlogID.ToString() + "&action=ViewContentByCategory&LangType=" + ContentLanguage.ToString() + "&ContType=" + (int)EkEnumeration.TaskType.TopicReply + "&contentid=" + m_iPostID.ToString());
        }
    }

    private void Process_Edit()
    {
        m_reftask = m_reftask.GetTaskByID(m_iID);
        m_reftask.CommentDisplayName = Context.Request.Form[txt_displayname.UniqueID];
        m_reftask.CommentEmail = Context.Request.Form[txt_email.UniqueID];
        if (Context.Request.Form[txt_url.UniqueID].ToLower() == "http://")
        {
            m_reftask.CommentURI = "";
        }
        else
        {
            m_reftask.CommentURI = Ektron.Cms.Common.EkFunctions.FixExternalHyperlink(Context.Request.Form[txt_url.UniqueID]);
        }

        if (rb_pending.Checked == true)
        {
            m_reftask.State = EkEnumeration.TaskState.Pending.ToString();
        }
        else
        {
            m_reftask.State = EkEnumeration.TaskState.Completed.ToString();
        }

        m_reftask.Description = Context.Request.Form[txt_comment.UniqueID];
        m_reftask.ImpersonateUser = true;
        m_reftask.UpdateTask();
        if (closeOnFinish == true)
        {
            Response.Redirect("../close.aspx", false);
        }
        else
        {
            Response.Redirect((string)("../content.aspx?id=" + m_iBlogID.ToString() + "&action=ViewContentByCategory&LangType=" + ContentLanguage.ToString() + "&ContType=" + EkEnumeration.TaskType.TopicReply.GetHashCode().ToString() + "&contentid=" + m_iPostID.ToString()));
        }
    }

    #endregion

    #region Private Helpers

    private void SetLabels()
    {
        ltr_displayname.Text = base.GetMessage("display name label");
        ltr_email.Text = base.GetMessage("generic email");
        ltr_url.Text = base.GetMessage("lbl url");
        ltr_post.Text = base.GetMessage("lbl blog post");
        ltr_status.Text = base.GetMessage("lbl state");
        ltr_comment.Text = base.GetMessage("comment text");
        rb_approved.Text = "&nbsp;" + base.GetMessage("lbl approved");
        rb_pending.Text = "&nbsp;" + base.GetMessage("lbl pending");
    }

    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script type=\"text/javascript\" >" + Environment.NewLine);
        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("   if (Trim(document.getElementById(\'" + txt_displayname.UniqueID + "\').value).length > 0) {" + Environment.NewLine);
        sbJS.Append("       if (Trim(document.getElementById(\'" + txt_comment.UniqueID + "\').value).length > 0) {" + Environment.NewLine);
        sbJS.Append("           document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("       } else {" + Environment.NewLine);
        sbJS.Append("           alert(\'" + base.GetMessage("js err comment") + "\');" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);
        sbJS.Append("   } else {" + Environment.NewLine);
        sbJS.Append("       alert(\'" + base.GetMessage("js err display name") + "\');" + Environment.NewLine);
        sbJS.Append("   } " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

    #endregion
}