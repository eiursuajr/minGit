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

	public partial class addapproval : System.Web.UI.UserControl
	{
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected long m_intId = 0;
		protected FolderData folder_data;
		protected PermissionData security_data;
		protected string AppImgPath = "";
		protected int ContentType = 1;
		protected long CurrentUserId = 0;
		protected Collection pagedata;
		protected string m_strPageAction = "";
		protected string m_strOrderBy = "";
		protected int ContentLanguage = -1;
		protected int EnableMultilingual = 0;
		protected string SitePath = "";
		protected string ItemType = "";
		protected ContentData content_data;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
			RegisterResources();
		}
		public bool AddApproval()
		{
			if (!(Request.QueryString["type"] == null))
			{
				ItemType = Convert.ToString(Request.QueryString["type"]).Trim().ToLower();
			}
			if (!(Request.QueryString["id"] == null))
			{
				m_intId = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (!(Request.QueryString["action"] == null))
			{
				m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["orderby"] == null))
			{
				m_strOrderBy = Convert.ToString(Request.QueryString["orderby"]);
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				m_refContentApi.ContentLanguage = ContentLanguage;
			}
			
			CurrentUserId = m_refContentApi.UserId;
			AppImgPath = m_refContentApi.AppImgPath;
			SitePath = m_refContentApi.SitePath;
			EnableMultilingual = m_refContentApi.EnableMultilingual;
			
			//POSTBACK VALIDATION NOT REQUIRED
			Display_AddApproval();
            return true;
		}
		#region APPROVAL - AddApproval
		private void Display_AddApproval()
		{
			
			security_data = m_refContentApi.LoadPermissions(m_intId, ItemType, 0);
			
			if (ItemType == "folder")
			{
				folder_data = m_refContentApi.GetFolderById(m_intId);
			}
			else
			{
				content_data = m_refContentApi.GetContentById(m_intId, 0);
			}
			ApprovalData[] approval_data;
			approval_data = m_refContentApi.GetAllUnassignedItemApprovals(m_intId, ItemType);
			AddApprovalToolBar();
			Populate_AddApprovals(approval_data);
		}
		private void Populate_AddApprovals(ApprovalData[] approval_data)
		{
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = m_refMsg.GetMessage("user or group name title");
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			AddApprovalsGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = m_refMsg.GetMessage("generic ID");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			AddApprovalsGrid.Columns.Add(colBound);
			
			AddApprovalsGrid.BorderColor = System.Drawing.Color.White;
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(string)));
			
			
			bool bInherited = false;
			if (ItemType == "folder")
			{
				bInherited = folder_data.Inherited;
			}
			else
			{
				bInherited = content_data.IsInherited;
			}
			int i;
			if (!(approval_data == null))
			{
				for (i = 0; i <= approval_data.Length - 1; i++)
				{
					dr = dt.NewRow();
					if (approval_data[i].UserId != 0)
					{
						dr[0] = "<a href=\"content.aspx?LangType=" + ContentLanguage + "&action=DoAddItemApproval&item_id=" + approval_data[i].UserId + "&base=user&id=" + m_intId + "&type=" + ItemType + "\" title=\"" + m_refMsg.GetMessage("alt add button text (user approvals)") + "\" OnClick=\"javascript:return CheckApprovalAddition(\'user\');\">";
						dr[0] += "<img class=\"imgUsers\" src=\"" + m_refContentApi.AppPath + "images/UI/Icons/user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("alt add button text (user approvals)") + "\" title=\"" + m_refMsg.GetMessage("alt add button text (user approvals)") + "\"/>" + approval_data[i].DisplayUserName + "</a>";
						dr[1] = approval_data[i].UserId;
					}
					else
					{
						
						if (approval_data[i].GroupId != 0)
						{
							dr[0] = "<a href=\"content.aspx?LangType=" + ContentLanguage + "&action=DoAddItemApproval&item_id=" + approval_data[i].GroupId + "&base=group&id=" + m_intId + "&type=" + ItemType + "\" title=\"" + m_refMsg.GetMessage("alt add button text (usergroup approvals)") + "\" OnClick=\"javascript:return CheckApprovalAddition(\'group\');\">";
							dr[0] += "<img class=\"imgUsers\" src=\"" + m_refContentApi.AppPath + "images/UI/Icons/users.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("alt add button text (usergroup approvals)") + "\" title=\"" + m_refMsg.GetMessage("alt add button text (usergroup approvals)") + "\"/>" + approval_data[i].DisplayUserGroupName + "</a>";
							dr[1] = approval_data[i].GroupId;
						}
					}
					dt.Rows.Add(dr);
				}
			}
			
			DataView dv = new DataView(dt);
			AddApprovalsGrid.DataSource = dv;
			AddApprovalsGrid.DataBind();
		}
		private void AddApprovalToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string WorkareaTitlebarTitle = "";
			if (ItemType == "folder")
			{
				WorkareaTitlebarTitle = (string) (m_refMsg.GetMessage("add folder approvals msg") + " \"" + folder_data.Name + "\"");
			}
			else
			{
				WorkareaTitlebarTitle = (string) (m_refMsg.GetMessage("add content approvals msg") + " \"" + content_data.Title + "\"");
			}
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);
			result.Append("<table><tr>");
			// result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (approvals)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('frmContent', '');"""))
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string) ("content.aspx?LangType=" + ContentLanguage + "&action=ViewApprovals&id=" + m_intId + "&type=" + ItemType), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		#endregion
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}