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
using Ektron.Cms.Content;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Controls;
using Ektron.Cms.UI.CommonUI;

	public partial class Workarea_ewebeditpro_changecomment : System.Web.UI.Page
	{
		protected string m_strCmdAction = ""; 
		private ContentAPI m_refContentApi = new ContentAPI();
		protected EkMessageHelper m_refMsg;
		protected string m_strActionType = "";
		protected long m_iCommentKeyId = 0;
		protected long m_iCommentId = 0;
		protected string m_strCommentText = "";
		protected int m_iContentLanguage = 0;
		protected long m_iRefId = 0;
		protected string m_strRefType = "";
		protected string m_strAppeWebPath = "";
		protected string m_strLocaleFileString = "0000";
		protected ApplicationAPI m_objAppUI = null;
		protected EkContent m_objContentObj1;
		protected long m_iCurrentUserId = 0;
		protected string m_strAppName = "";
		protected string m_strServerName = "";
		protected string m_strEditorName = "";
		protected string m_strCommentType = "";
		protected string m_strOrderBy = "";
		protected string m_strvar2 = "";
		protected bool m_bNS4 = false;
		protected bool m_bResetCommentTag = false;
		protected string ErrorString = "";
		protected bool m_bInsertElementFlag = false;
        protected const int ALL_CONTENT_LANGUAGES = -1;
        protected const int CONTENT_LANGUAGES_UNDEFINED = 0;
        protected int ContentLanguage;
        protected int EnableMultilingual;
        protected long CommentKeyId;
        protected long CommentId;
        protected long RefId;
        protected string RefType;
        protected long CurrentUserID;
        protected string Action;
        protected string ActionType;
        protected bool Flag;
        protected string CommentText;
        protected string CommentType;
        protected string Messages;
        protected bool NS4;
        protected bool ResetCommentTag;
        protected string OrderBy;
        protected string AppeWebPath;
        protected string var1;
        protected string var2;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                if ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", m_refContentApi.RequestInformationRef.CallerId) == false)
                {
                    Response.Redirect("../login.aspx?fromLnkPg=1", false);
                    return;
                }
                else
                {
                    InitEnvironment();
                    ActOnEnvironment();

                    ContentLanguage = m_iContentLanguage;
                    CurrentUserID = m_iCurrentUserId;
                    AppeWebPath = m_strAppeWebPath;
                    var1 = m_strServerName;
                    Messages = " ";
                    RefType = m_strRefType;
                    Flag = m_bInsertElementFlag;
                    ResetCommentTag = false;
                    CommentKeyId = m_iCommentKeyId;
                    Action = m_strCmdAction;
                    ActionType = m_strActionType;
                    CommentId = m_iCommentId;
                    RefId = m_iRefId;
                    CommentType = m_strCommentType;
                    OrderBy = m_strOrderBy;
                    var2 = m_strvar2;
                    NS4 = m_bNS4;
                    ResetCommentTag = m_bResetCommentTag;
                    CommentText = m_strCommentText;
				    m_refMsg = m_refContentApi.EkMsgRef;
               
					
					
					
					Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
					Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
					
					lEditorName.Text = m_strEditorName;
					
					EditorPageClosed.Text = m_refMsg.GetMessage("msg editor page closed");
					if (0 == m_strCmdAction.Length || "Edit" == m_strCmdAction)
					{
						if (m_strCmdAction == "Edit")
						{
							CommentEditor.Content = m_strCommentText;
						}
					}
					if (m_strCmdAction != "Edit" && m_strCmdAction != "Update")
					{
						CommentListHtml.Text = ListAllCommentsInRow();
					}
				}
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("../../workarea/reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)));
			}
		}
		
		protected string ListAllCommentsInRow()
		{
			
			System.Text.StringBuilder sbComments = new System.Text.StringBuilder();
			int iCnt;
			Microsoft.VisualBasic.Collection cComments;
			
			
			// Ensure we have a good language
			m_objAppUI.ContentLanguage = m_iContentLanguage;
			
			cComments = m_objContentObj1.GetAllComments(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, m_strOrderBy);
			if ("" != ErrorString)
			{
				Response.Redirect((string) ("../reterror.aspx?info=" + ErrorString));
			}
			
			iCnt = 1;
			foreach (Microsoft.VisualBasic.Collection cComment in cComments)
			{
				if (iCnt / 2 == (iCnt / 2))
				{
					if (System.Convert.ToInt32(cComment["USER_ID"]) == (int) m_iCurrentUserId || ((int) m_iCurrentUserId == 1))
					{
						sbComments.Append("<tr class=evenrow><td><a href=\'changecomment.aspx?ref_type=" + m_strRefType + "&editorName=" + m_strEditorName + "&ty=" + m_strActionType + "&orderby=" + Request["orderby"] + "&action=Edit&ref_id=" + m_iRefId + "&commentkey_id=" + m_iCommentKeyId + "&comment_id=" + cComment["COMMENT_ID"] + "\'>" + cComment["DATE_CREATED"] + "</a></td><td>" + cComment["FIRST_NAME"] + " " + cComment["LAST_NAME"] + "</td><td>" + cComment["COMMENTS_TEXT"] + "</td></tr>");
					}
					else
					{
						sbComments.Append("<tr class=evenrow><td>" + cComment["DATE_CREATED"] + "</td><td>" + cComment["FIRST_NAME"] + " " + cComment["LAST_NAME"] + "</td><td>" + cComment["COMMENTS_TEXT"] + "</td></tr>");
					}
				}
				else
				{
					if (System.Convert.ToInt32(cComment["USER_ID"]) == (int) m_iCurrentUserId || ((int) m_iCurrentUserId == 1))
					{
						sbComments.Append("<tr><td><a href=\'changecomment.aspx?ref_type=" + m_strRefType + "&editorName=" + m_strEditorName + "&ty=" + m_strActionType + "&orderby=" + EkFunctions.HtmlEncode(Request.QueryString["orderby"]) + "&action=Edit&ref_id=" + m_iRefId + "&commentkey_id=" + m_iCommentKeyId + "&comment_id=" + cComment["COMMENT_ID"] + "\'>" + cComment["DATE_CREATED"] + "</a></td><td>" + cComment["FIRST_NAME"] + " " + cComment["LAST_NAME"] + "</td><td>" + cComment["COMMENTS_TEXT"] + "</td></tr>");
					}
					else
					{
						sbComments.Append("<tr><td>" + cComment["DATE_CREATED"] + "</td><td>" + cComment["FIRST_NAME"] + " " + cComment["LAST_NAME"] + "</td><td>" + cComment["COMMENTS_TEXT"] + "</td></tr>");
					}
				}
				
				iCnt++;
			}
			
			return sbComments.ToString();
		}
		
		private string GetSelectedComment(long iCommentKeyId, long iCommentId, long iRefId, string strRefType)
			{
			System.Text.StringBuilder sbComments = new System.Text.StringBuilder();
			Microsoft.VisualBasic.Collection cComments;
			
			string strCommentText = "";
			
			// Ensure we have a good language
			m_objAppUI.ContentLanguage = m_iContentLanguage;
			
			cComments = m_objContentObj1.GetAllComments(iCommentKeyId, iCommentId, iRefId, strRefType, m_iCurrentUserId, "");
			if ("" != ErrorString)
			{
				Response.Redirect((string) ("../reterror.aspx?info=" + ErrorString));
			}
			
			// This will get the last one, if there are more than one.
			foreach (Microsoft.VisualBasic.Collection cComment in cComments)
			{
				strCommentText = cComment["COMMENTS_TEXT"].ToString();
			}
			
			return strCommentText;
		}
		
		private void InitEnvironment()
		{
			m_strCmdAction = EkFunctions.HtmlEncode(Request.QueryString["Action"]);
			m_strActionType = EkFunctions.HtmlEncode(Request.QueryString["ty"]);
			m_iCommentKeyId = System.Convert.ToInt64(Request["commentkey_id"]);
			m_iCommentId = System.Convert.ToInt64(Request["Comment_Id"]);
			m_strCommentType = EkFunctions.HtmlEncode(Request["comment_type"]);
			m_strOrderBy = EkFunctions.HtmlEncode(Request["orderby"]);
			m_iRefId = System.Convert.ToInt64(Request["ref_id"]);
			m_strRefType = EkFunctions.HtmlEncode(Request["ref_type"]);
			m_strEditorName = EkFunctions.HtmlEncode(Request["editorName"]);
			m_strAppeWebPath = "";
			m_strLocaleFileString = "0000";
			m_objAppUI = new ApplicationAPI();
			m_objContentObj1 = m_objAppUI.EkContentRef;
			
			m_strAppeWebPath = m_objAppUI.ApplicationPath + m_objAppUI.AppeWebPath;
			
			if (Request["LangType"] != "")
			{
				m_iContentLanguage = Convert.ToInt32(Request["LangType"]);
				m_objAppUI.SetCookieValue("LastValidLanguageID", m_iContentLanguage.ToString());
			}
			else
			{
				if (m_objAppUI.GetCookieValue("LastValidLanguageID") != "")
				{
					m_iContentLanguage = int.Parse(m_objAppUI.GetCookieValue("LastValidLanguageID"));
				}
			}
			
			m_objAppUI.ContentLanguage = m_iContentLanguage;
			m_objContentObj1 = m_objAppUI.EkContentRef;
			m_iCurrentUserId = m_objAppUI.UserId;
			m_strAppeWebPath = m_objAppUI.ApplicationPath + m_objAppUI.AppeWebPath;
			m_strServerName = Request.ServerVariables["SERVER_NAME"];
			this.Page.Title = m_objAppUI.AppName + " Comments";
			
			m_strvar2 = m_objContentObj1.GetEditorVariablev2_0(0, "tasks");
			
			if ((Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("MOZILLA") > -1 && (Strings.UCase(Request.ServerVariables["http_user_agent"])).IndexOf("4.7") > -1 && (!(Strings.UCase(Request.ServerVariables["http_user_agent"]).IndexOf("GECKO") > -1)))
			{
			  m_bNS4 = true;
			}
			else
			{
				m_bNS4 = false;
			}
			
		}
		
		private void ActOnEnvironment()
		{
			long iNewId = 0;
			
			if (0 == m_iCommentKeyId && "NEW" != m_strCommentType)
			{
				iNewId = m_objContentObj1.AddComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, EkFunctions.HtmlEncode(Request["commentkey_text"]));
				if ("" != ErrorString)
				{
					Response.Redirect((string) ("../reterror.aspx?info=" + ErrorString));
				}
				m_iCommentKeyId = iNewId;
				m_bResetCommentTag = true;
			}
			
			switch (m_strCmdAction)
			{
				case "Add":
					m_strCommentText = (string) CommentEditor.Content;
					if (m_strCommentText != "")
					{
						iNewId = m_objContentObj1.AddComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, m_strCommentText.Replace("\'", "\'\'"));
						if ("" != ErrorString)
						{
							Response.Redirect((string) ("../reterror.aspx?info=" + ErrorString));
						}
						if ("NEW" == m_strCommentType)
						{
							m_iCommentKeyId = iNewId;
						}
						m_bInsertElementFlag = true;
					}
					break;
				case "Edit":
					m_strCommentText = GetSelectedComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType);
					break;
					
				case "Update":
					m_strCommentText = (string) CommentEditor.Content;
					ReplaceComment(m_iCommentId, m_strCommentText);
					Response.Redirect("changecomment.aspx?ref_type=" + m_strRefType + "&editorName=" + m_strEditorName + "&ref_id=" + m_iRefId + "&ty=" + Request["ty"] + "&commentkey_id=" + m_iCommentKeyId);
					break;
			}
			
		}
		
		private void ReplaceComment(long iCommentId, string strNewComment)
		{
			object objNew;
			string strComment;

			m_objAppUI.ContentLanguage = m_iContentLanguage;
			strComment = strNewComment.Replace("\'", "\'\'");
			objNew = m_objContentObj1.UpdateComment(iCommentId, strComment);
			if ("" != ErrorString)
			{
				Response.Redirect((string) ("../reterror.aspx?info=" + ErrorString));
			}
		}
	}
