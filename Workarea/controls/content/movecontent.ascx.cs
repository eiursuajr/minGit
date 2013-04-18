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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

	public partial class movecontent : System.Web.UI.UserControl
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
		protected EkContent m_refContent;
		protected System.Text.StringBuilder selectvalue;
		protected Collection cAllFolders;
		protected ContentData content_data;
		protected string CallerPage = "";
		protected int m_rootFolderIsXml = 0;
		protected int m_intContentType = 0;
		protected string m_strContentStatus = "A";
		protected bool _initIsFolderAdmin = false;
		protected bool _isFolderAdmin = false;
		protected bool _initIsCopyOrMoveAdmin = false;
		protected bool _isCopyOrMoveAdmin = false;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
			InitFolderIsXmlFlags();
			long folderid = 0;
			if (!(Request.QueryString["folder_id"] == null))
			{
				folderid = Convert.ToInt64(Request.QueryString["folder_id"]);
				folder_data = m_refContentApi.GetFolderById(folderid);
			}
			RegisterResources();
			Util_SetServerVariables();
		}
		
		private bool IsFolderAdmin()
		{
			if (_initIsFolderAdmin || (folder_data == null))
			{
				return _isFolderAdmin;
			}
			_isFolderAdmin = m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id, 0, false);
			_initIsFolderAdmin = true;
			return _isFolderAdmin;
		}
		
		private bool IsCopyOrMoveAdmin()
		{
			if (_initIsCopyOrMoveAdmin || (folder_data == null))
			{
				return _isCopyOrMoveAdmin;
			}
			_isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder( Convert.ToInt64(EkEnumeration.CmsRoleIds.MoveOrCopy), folder_data.Id, m_refContentApi.UserId, false);
			_initIsCopyOrMoveAdmin = true;
			return _isCopyOrMoveAdmin;
		}
		
		public bool MoveContent()
		{
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
			m_refContent = m_refContentApi.EkContentRef;
			if (!(Request.QueryString["page"] == null))
			{
				CallerPage = Request.QueryString["page"].Trim().ToLower();
			}
			if (!(Page.IsPostBack))
			{
				Display_MoveContent();
			}
			else
			{
				Process_DoMoveContent();
			}
            return true;
		}
		#region ACTION - DoMoveContent
		private void Process_DoMoveContent()
		{
			string strContentIds = "";
			long intMoveFolderId = 0;
			string FolderPath = "";
			
			try
			{
				strContentIds = Request.Form[contentids.UniqueID];
				m_refContent = m_refContentApi.EkContentRef;
				intMoveFolderId = m_refContent.GetFolderID(Request.Form["move_folder_id"]);
				
				if (Request.Form[hdnCopyAll.UniqueID] == "true")
				{
					bool publishContent = false;
					if (! (Request.Form["btn_PublishCopiedContent"] == null) && Request.Form["btn_PublishCopiedContent"].ToString() == "on")
					{
						publishContent = true;
					}
					m_refContent.CopyAllLanguageContent(m_intId, intMoveFolderId, publishContent);
				}
				else
				{
					if (! (Request.Form[RadBtnMoveCopyValue.UniqueID] == null) && Request.Form[RadBtnMoveCopyValue.UniqueID].ToString() == "copy")
					{
						bool bPublish = false;
						if (! (Request.Form["btn_PublishCopiedContent"] == null) && Request.Form["btn_PublishCopiedContent"].ToString() == "on")
						{
							bPublish = true;
						}
						m_refContent.CopyContentByID(strContentIds, ContentLanguage, intMoveFolderId, bPublish);
					}
					else
					{
						m_refContent.MoveContent(strContentIds, ContentLanguage, intMoveFolderId);
					}
				}
				
				FolderPath = m_refContent.GetFolderPath(intMoveFolderId);
				if ((FolderPath.Substring(FolderPath.Length - 1, 1) == "\\"))
				{
					FolderPath = FolderPath.Substring(FolderPath.Length - FolderPath.Length - 1, FolderPath.Length - 1);
				}
				FolderPath = FolderPath.Replace("\\", "\\\\");
				
				if (CallerPage.ToLower() == "webpage")
				{
					Page.ClientScript.RegisterStartupScript(this.GetType(), "CloseScript", GetCloseScript());
				}
				else if ((CallerPage != "content.aspx") && (CallerPage != "cmsform.aspx") && (CallerPage != "")) //TODO:Verify
				{
					if (CallerPage.Trim().Length == 0)
					{
						CallerPage = "Cmsform.aspx";
					}
					Response.Redirect(CallerPage + "?LangType=" + ContentLanguage + "&action=ViewAllFormsByFolderID&folder_id=" + intMoveFolderId + "&reloadtrees=Content,Library&TreeNav=" + FolderPath, false);
				}
				else
				{
					Response.Redirect((string) ("content.aspx?LangType=" + ContentLanguage + "&action=ViewContentByCategory&id=" + intMoveFolderId + "&reloadtrees=Content,Library&TreeNav=" + FolderPath), false);
				}
			}
			catch (Exception ex)
			{
				int intError;
				string strError;
				strError = "because a record with the same title exists in the destination folder";
				intError = ex.Message.IndexOf(strError);
				if (intError > -1)
				{
					strError = ex.Message.Substring(0, intError + strError.Length);
					Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(strError + ".") + "&LangType=" + ContentLanguage), false);
				}
				else
				{
					Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + ContentLanguage), false);
				}
				
			}
		}
		public string GetCloseScript()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=\"javascript\">");
			sb.Append("top.close();<");
			sb.Append("/script>");
			return sb.ToString();
		}
		#endregion
		#region CONTENT - MoveContent
		private void Display_MoveContent()
		{
			bool bIsError = false;
			try
			{
				content_data = m_refContentApi.GetContentById(m_intId, 0);
			}
			catch (Exception)
			{
				bIsError = true;
			}
			if (bIsError)
			{
				folder_data = m_refContentApi.GetFolderById(m_intId);
			}
			else
			{
				if (!(content_data == null))
				{
					folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
				}
				else
				{
					folder_data = m_refContentApi.GetFolderById(m_intId);
				}
			}
			security_data = m_refContentApi.LoadPermissions(m_intId, "content", 0);
			if (CallerPage.IndexOf("content.aspx") + 1 > 0)
			{
				CallerPage = ""; //TODO: I am setting it "" because move content works only within folders
			}
			MoveContentToolBar();
			
			selectvalue = new System.Text.StringBuilder(); //POPULATE FOLDER LIST
			selectvalue.Append("" + m_refMsg.GetMessage("lbl destination folder") + ":&nbsp;	<span style=\"white-space:nowrap\"><input id=\"move_folder_id\" name=\"move_folder_id\" size=\"50%\" value=\"\\\" readonly=\"true\"/>  <a class=\"button buttonInline greenHover buttonCheckAll\" href=\"#\" onclick=\"javascript:LoadSelectFolderChildPage();return true;\">" + m_refMsg.GetMessage("lbl select folder") + "</a></span>");
			
			m_refContent = m_refContentApi.EkContentRef;
			source_folder_is_xml.Value = (!(folder_data.XmlConfiguration == null)) ? "1" : "0";
			
			tdMoveToFolderList.InnerHtml = selectvalue.ToString();
			contentids.Value = content_data.Id.ToString();
			m_intContentType = content_data.Type;
			m_strContentStatus = content_data.Status;
			Populate_MoveContent(content_data);
		}
		private void Populate_MoveContent(ContentData content_data)
		{
			System.Web.UI.WebControls.BoundColumn colBound;
			colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = m_refMsg.GetMessage("generic Title");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ID";
			colBound.HeaderText = m_refMsg.GetMessage("generic ID");
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			MoveContentGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "STATUS";
			colBound.HeaderText = m_refMsg.GetMessage("generic Status");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			colBound.ItemStyle.Wrap = false;
			MoveContentGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DATEMODIFIED";
			colBound.HeaderText = m_refMsg.GetMessage("generic Date Modified");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITORNAME";
			colBound.HeaderText = m_refMsg.GetMessage("generic Last Editor");
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.ItemStyle.Wrap = false;
			MoveContentGrid.Columns.Add(colBound);
			
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(long)));
			dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
			dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
			dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));
			
			dr = dt.NewRow();
			dr[0] = "<a href=\"content.aspx?LangType=" + content_data.LanguageId + "&action=View&id=" + content_data.Id + "\" title=\'" + m_refMsg.GetMessage("generic View") + " \"" + content_data.Title.Replace("\'", "`") + "\")" + "\'>" + content_data.Title + "</a>";
			dr[1] = content_data.Id;
			dr[2] = content_data.Status;
			dr[3] = content_data.DisplayLastEditDate;
			dr[4] = content_data.EditorLastName;
			dt.Rows.Add(dr);
			
			DataView dv = new DataView(dt);
			MoveContentGrid.DataSource = dv;
			MoveContentGrid.DataBind();
		}
		
		private void MoveContentToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			if (folder_data.FolderType == 9)
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("btn just move Content") + " \"" + content_data.Title + "\""));
			}
			else
			{
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("btn move Content") + " \"" + content_data.Title + "\""));
			}
			result.Append("<table><tr>");

			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back();", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			
			if (security_data.IsAdmin || IsFolderAdmin() || IsCopyOrMoveAdmin())
			{
				if (folder_data.FolderType == 9)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn just move content"), m_refMsg.GetMessage("btn just move content"), "OnClick=\"javascript:checkMoveForm_Content(\'" + m_refContent.GetFolderPath(content_data.FolderId).Replace("\\", "\\\\") + "\');return false;\"", StyleHelper.CopyButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), "OnClick=\"javascript:checkMoveForm_Content(\'" + m_refContent.GetFolderPath(content_data.FolderId).Replace("\\", "\\\\") + "\');return false;\"", StyleHelper.CopyButtonCssClass, true));
				}
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		
		private void InitFolderIsXmlFlags()
		{
			FolderData fldrData;
			long nFolderID = 0;
			
			fldrData = m_refContentApi.GetFolderById(0);
			m_rootFolderIsXml = System.Convert.ToInt32((!(fldrData.XmlConfiguration == null)) ? 1 : 0);
			
			if (!(Request.QueryString["id"] == null))
			{
				nFolderID = Convert.ToInt64(Request.QueryString["id"]);
			}
			
			bool bIsError = false;
			try
			{
				content_data = m_refContentApi.GetContentById(m_intId, 0);
			}
			catch (Exception)
			{
				bIsError = true;
			}
			if (bIsError)
			{
				fldrData = m_refContentApi.GetFolderById(m_intId);
			}
			else
			{
				if (!(content_data == null))
				{
					fldrData = m_refContentApi.GetFolderById(content_data.FolderId);
				}
				else
				{
					fldrData = m_refContentApi.GetFolderById(m_intId);
				}
			}
			source_folder_is_xml.Value = (!(fldrData.XmlConfiguration == null)) ? "1" : "0";
		}
		#endregion
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
		private void Util_SetServerVariables()
		{
			jsConfirmCopyAll.Text = m_refMsg.GetMessage("jsconfirm copy all");
		}
	}
