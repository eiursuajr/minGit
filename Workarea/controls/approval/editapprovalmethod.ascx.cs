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
	public partial class editapprovalmethod : System.Web.UI.UserControl
	{
		
		
		
		#region Member Variables
		
		protected ContentAPI _ContentApi = new ContentAPI();
		protected StyleHelper _StyleHelper = new StyleHelper();
		protected EkMessageHelper _MessageHelper;
		protected long _Id = 0;
		protected FolderData _FolderData;
		protected PermissionData _PermissionData;
		protected string _AppImgPath = "";
		protected int _ContentType = 1;
		protected long _CurrentUserId = 0;
		protected Collection _PageData;
		protected string _PageAction = "";
		protected string _OrderBy = "";
		protected int _ContentLanguage = -1;
		protected int _EnableMultilingual = 0;
		protected string _SitePath = "";
		protected string _ItemType;
		protected ContentData _ContentData;
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			
			//register page components
			this.RegisterCSS();
			this.RegisterJS();
			
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			_MessageHelper = _ContentApi.EkMsgRef;
		}
		
		#endregion
		
		public bool EditApprovalMethod()
		{
			if (!(Request.QueryString["type"] == null))
			{
				_ItemType = Convert.ToString(Request.QueryString["type"]).Trim().ToLower();
			}
			if (!(Request.QueryString["id"] == null))
			{
				_Id = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (!(Request.QueryString["action"] == null))
			{
				_PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["orderby"] == null))
			{
				_OrderBy = Convert.ToString(Request.QueryString["orderby"]);
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					_ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					_ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
				}
				else
				{
					if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						_ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					_ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				_ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				_ContentApi.ContentLanguage = _ContentLanguage;
			}
			
			_CurrentUserId = _ContentApi.UserId;
			_AppImgPath = _ContentApi.AppImgPath;
			_SitePath = _ContentApi.SitePath;
			_EnableMultilingual = _ContentApi.EnableMultilingual;
			
			if (!(Page.IsPostBack))
			{
				Display_EditApprovals();
			}
			else
			{
				Process_DoEditApprovalMethod();
			}
            return true;
		}
		
		#region ACTION - EditApprovalMethod
		private void Process_DoEditApprovalMethod()
		{
			Ektron.Cms.Content.EkContent ekContentRef;
			try
			{
				ekContentRef = _ContentApi.EkContentRef;
				_PageData = new Collection();
				_PageData.Add(System.Convert.ToInt32(Request.Form[rblApprovalMethod.UniqueID]), "ApprovalMethod", null, null);
				if (_ItemType == "folder")
				{
					_PageData.Add(_Id, "FolderID", null, null);
					ekContentRef.UpdateFolderApprovalMethod(_PageData);
				}
				else
				{
					_PageData.Add(_Id, "ContentID", null, null);
					ekContentRef.UpdateContentApprovalMethod(_PageData);
				}
				Response.Redirect((string) ("content.aspx?LangType=" + _ContentLanguage + "&action=ViewApprovals&id=" + _Id + "&type=" + _ItemType), false);
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _ContentLanguage), false);
			}
		}
		#endregion
		
		#region APPROVAL - EditApprovalsMethod
		private void Display_EditApprovals()
		{
			_PermissionData = _ContentApi.LoadPermissions(_Id, _ItemType, 0);
			
			int m_intApprovalMethoad = 0;
			if (_ItemType == "folder")
			{
				_FolderData = _ContentApi.GetFolderById(_Id);
				m_intApprovalMethoad = _FolderData.ApprovalMethod;
			}
			else
			{
				_ContentData = _ContentApi.GetContentById(_Id, 0);
				m_intApprovalMethoad = _ContentData.ApprovalMethod;
			}
			EditApprovalsToolBar();
			rblApprovalMethod.Items.Add(new ListItem(_MessageHelper.GetMessage("force all approvers with description"), "1"));
			rblApprovalMethod.Items.Add(new ListItem(_MessageHelper.GetMessage("do not force all approvers with description"), "0"));
			if (m_intApprovalMethoad == 1)
			{
				rblApprovalMethod.Items[0].Selected = true;
			}
			else
			{
				rblApprovalMethod.Items[1].Selected = true;
			}
		}
		private void EditApprovalsToolBar()
		{
			
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string workareaTitlebarTitle = "";
			bool isFolderUserAdmin = false;
			
			if (_ItemType == "folder")
			{
				workareaTitlebarTitle = "Edit Approval Method For The Folder" + " \"" + _FolderData.Name + "\"";
			}
			else
			{
				workareaTitlebarTitle = "Edit Approval Method For The Content" + " \"" + _ContentData.Title + "\"";
			}
			
			txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(workareaTitlebarTitle);
			
			result.Append(" <table><tbody><tr>");
			bool arePermissionsInherited = false;
			if (_ItemType == "folder")
			{
				arePermissionsInherited = _FolderData.Inherited;
			}
			else
			{
				arePermissionsInherited = _ContentData.IsInherited;
			}
			if (!(_FolderData == null))
			{
				isFolderUserAdmin = _PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_FolderData.Id, 0, false);
			}
			else
			{
				if (!(_ContentData == null))
				{
                    isFolderUserAdmin = _PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_ContentData.FolderId, 0, false);
				}
				else
				{
					isFolderUserAdmin = _PermissionData.IsAdmin;
				}
			}

			if (_ItemType == "folder")
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewApprovals&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=View&id=" + _Id), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			if ((_PermissionData.IsAdmin || isFolderUserAdmin) && arePermissionsInherited == false)
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt save approval method button text"), _MessageHelper.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'frmContent\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
			result.Append("</td>");
			result.Append("</tr></tbody></table>");
			htmToolBar.InnerHtml = result.ToString();
			
		}
		
		#endregion
		
		
		#region JS, CSS
		
		private void RegisterJS()
		{
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			
		}
		
		private void RegisterCSS()
		{
			
		}
		
		#endregion
		
	}