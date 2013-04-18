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


	public partial class deletepermissions : System.Web.UI.UserControl
	{
		
		
		#region Member Variables
		
		protected ContentAPI _ContentApi = new ContentAPI();
		protected StyleHelper _StyleHelper = new StyleHelper();
		protected EkMessageHelper _MessageHelper;
		protected long _Id = 0;
		protected FolderData _FolderData;
		protected PermissionData _PermissionData;
		protected string _ApplicationImagePath = "";
		protected int _ContentType = 1;
		protected long _CurrentUserId = 0;
		protected Collection _PageData;
		protected string _PageAction = "";
		protected string _OrderBy = "";
		protected int _ContentLanguage = -1;
		protected int _EnableMultilingual = 0;
		protected string _SitePath = "";
		protected ContentData _ContentData;
		protected string _ItemType = "";
		protected bool _IsMembership = false;
		protected string _Base = "";
		
		private bool _IsBoard = false;
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			
			_IsMembership = System.Convert.ToBoolean((string.IsNullOrEmpty(Request.QueryString["membership"])) ? false : (bool.Parse(Request.QueryString["membership"])));
			
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			_MessageHelper = _ContentApi.EkMsgRef;
			RegisterResources();
			if (ddlUserType.Items.Count == 0)
			{
				AddUserTypes();
			}
		}
		
		#endregion
		
		#region Helpers
		
		public bool DeletePermission()
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
			if (!(Request.Form[ddlUserType.UniqueID] == null) && Request.Form[ddlUserType.UniqueID] == "membership")
			{
				_IsMembership = true;
				ddlUserType.SelectedIndex = 1;
			}
			if (!(Request.QueryString["base"] == null))
			{
				_Base = Request.QueryString["base"].Trim().ToLower();
			}
			_CurrentUserId = _ContentApi.UserId;
			_ApplicationImagePath = _ContentApi.AppImgPath;
			_SitePath = _ContentApi.SitePath;
			_EnableMultilingual = _ContentApi.EnableMultilingual;
			if (!(Page.IsPostBack))
			{
				Display_DeletePermissions();
			}
			else
			{
			}
			
			return false; // should this return true
		}
		
		private void AddUserTypes()
		{
			ListItem item;
			item = new ListItem(_MessageHelper.GetMessage("lbl view cms users"), "standard");
			ddlUserType.Items.Add(item);
			item = new ListItem(_MessageHelper.GetMessage("lbl view memberShip users"), "membership");
			ddlUserType.Items.Add(item);
		}
		
		#endregion
		
		#region ACTION - DeleteItemApproval
		
		private void Process_DoDeleteItemApproval()
		{
			Ektron.Cms.Content.EkContent m_refContent;
			try
			{
				m_refContent = _ContentApi.EkContentRef;
				_PageData = new Collection();
				if (Request.QueryString["type"] == "folder")
				{
					_PageData.Add(Request.QueryString["id"], "FolderID", null, null);
					_PageData.Add("", "ContentID", null, null);
				}
				else
				{
					_PageData.Add(Request.QueryString["id"], "ContentID", null, null);
					_PageData.Add("", "FolderID", null, null);
				}
				if (Request.QueryString["base"] == "user")
				{
					_PageData.Add(Request.QueryString["item_id"], "UserID", null, null);
					_PageData.Add("", "UserGroupID", null, null);
				}
				else
				{
					_PageData.Add(Request.QueryString["item_id"], "UserGroupID", null, null);
					_PageData.Add("", "UserID", null, null);
				}
				m_refContent.DeleteItemApprovalv2_0(_PageData);
				Response.Redirect((string) ("content.aspx?LangType=" + _ContentLanguage + "&action=ViewApprovals&id=" + Request.QueryString["id"] + "&type=" + Request.QueryString["type"]), false);
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message)), false);
			}
		}
		#endregion
		
		#region PERMISSION - DeletePermissions
		
		private void Display_DeletePermissions()
		{
			if (_ItemType == "folder")
			{
				_FolderData = _ContentApi.GetFolderById(_Id);
				if (_FolderData.FolderType == Convert.ToInt32( Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard))
				{
					_IsBoard = true;
				}
			}
			else
			{
				_ContentData = _ContentApi.GetContentById(_Id, 0);
			}
			DeletePermissionsToolBar();
			_PageData = new Collection();
			
			_PageData.Add("", "UserID", null, null);
			_PageData.Add("", "UserGroupList", null, null);
			_PageData.Add(_Id, "ItemID", null, null);
			_PageData.Add(_ItemType, "RequestType", null, null);
			UserPermissionData[] userpermission_data;
			if (_IsMembership)
			{
				userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.All); //GetMemShpOrderedItemPermissionsV4(pagedata)
				
			}
			else
			{
				userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.All); //GetCmsOrderedItemPermissionsV4(pagedata)
				
			}
			td_dp_title.InnerHtml = _MessageHelper.GetMessage("delete permissions msg");
			Populate_DeletePermissionsGenericGrid(userpermission_data);
			if (! _IsMembership)
			{
				phAdvancedTab.Visible = true;
				phAdvancedContent.Visible = true;
				Populate_DeletePermissionsAdvancedGrid(userpermission_data);
			}
		}
		
		private void Populate_DeletePermissionsGenericGrid(UserPermissionData[] userpermission_data)
		{
			SetServerSideJSVariable();
			
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "Select";
            colBound.HeaderText = _MessageHelper.GetMessage("generic select");
			colBound.HeaderStyle.CssClass = "narrowerColumn";
			colBound.ItemStyle.CssClass = "narrowerColumn";
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = _MessageHelper.GetMessage("user or group name title");
			colBound.HeaderStyle.CssClass = "left";
			colBound.ItemStyle.CssClass = "left";
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "READ";
			colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDIT";
			if (_IsBoard)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Edit title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ADD";
			if (_IsBoard)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DELETE";
			if (_IsBoard)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Delete title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsGenericGrid.Columns.Add(colBound);
			if (!(_IsBoard))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "RESTORE";
				colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
			}
			
			if (_IsBoard)
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDFILE";
				colBound.HeaderText = "Post/Reply";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDHYP";
				colBound.HeaderText = "Moderate";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
			}
			else
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDFILE";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDHYP";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "OVERWRITELIB";
				colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsGenericGrid.Columns.Add(colBound);
			}
			
			DataTable dt = new DataTable();
			DataRow dr;
			bool bInherited;
			if (_ItemType == "folder")
			{
				bInherited = _FolderData.Inherited;
			}
			else
			{
				bInherited = _ContentData.IsInherited;
			}
			dt.Columns.Add(new DataColumn("SELECT", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("READ", typeof(string)));
			dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
			dt.Columns.Add(new DataColumn("ADD", typeof(string)));
			dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
			dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));
			dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
			dt.Columns.Add(new DataColumn("GADD", typeof(string)));
			dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
			dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
			dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));
			int i = 0;
			
			if (!(userpermission_data == null))
			{
				for (i = 0; i <= userpermission_data.Length - 1; i++)
				{
					dr = dt.NewRow();
					
					if (userpermission_data[i].GroupId != -1)
					{
						dr[0] = "<input type=\"checkbox\" id=\"group" + userpermission_data[i].GroupId + "\" name=\"group" + userpermission_data[i].GroupId + "\"/>";
						
						if (bInherited)
						{
							dr[1] = "<span class=\"membershipGroup\">";
							dr[1] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=group&PermID=" + userpermission_data[i].GroupId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete group permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'group\',\'" + _ItemType + "\');\">" + userpermission_data[i].GroupName + "</a>";
							dr[1] += "</span>";
						}
						else
						{
							dr[1] = "<span class=\"cmsGroup\">";
							dr[1] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=group&PermID=" + userpermission_data[i].GroupId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete group permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'group\',\'" + _ItemType + "\');\">" + userpermission_data[i].GroupName + "</a>";
							dr[1] += "</span>";
						}
						
					}
					else
					{
						dr[0] = "<input type=\"checkbox\" id=\"user" + userpermission_data[i].UserId + "\" name=\"user" + userpermission_data[i].UserId + "\"/>";
						if (_IsMembership)
						{
							dr[1] = "<span class=\"membershipUser\">";
							dr[1] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=user&PermID=" + userpermission_data[i].UserId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete user permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'user\',\'" + _ItemType + "\');\">" + userpermission_data[i].DisplayUserName + "</a>";
							dr[1] += "</span>";
						}
						else
						{
							dr[1] = "<span class=\"cmsUser\">";
							dr[1] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=user&PermID=" + userpermission_data[i].UserId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete user permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'user\',\'" + _ItemType + "\');\">" + userpermission_data[i].DisplayUserName + "</a>";
							dr[1] += "</span>";
						}
					}
					
					dr[2] = CheckPermission(userpermission_data[i].IsReadOnly);
					dr[3] = CheckPermission(userpermission_data[i].CanEdit);
					dr[4] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanAdd));
					dr[5] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanDelete));
					if (!(_IsBoard))
					{
						dr[6] = CheckPermission(userpermission_data[i].CanRestore);
					}
					dr[7] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].IsReadOnlyLib));
					dr[8] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanAddToImageLib));
					if (!(_IsBoard))
					{
						dr[9] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanAddToFileLib));
						dr[10] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanAddToHyperlinkLib));
						dr[11] = CheckPermission(userpermission_data[i].CanOverwriteLib);
					}
					dt.Rows.Add(dr);
				}
			}
			DataView dv = new DataView(dt);
			PermissionsGenericGrid.DataSource = dv;
			PermissionsGenericGrid.DataBind();
		}
		
		private string CheckPermission(bool bPerm)
		{
			//This method return ("x") if bPerm is true else (" ")
			if (bPerm)
			{
				return "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
			}
			else
			{
				return "&#160;";
			}
			
		}
		
		private void Populate_DeletePermissionsAdvancedGrid(UserPermissionData[] userpermission_data)
		{
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = _MessageHelper.GetMessage("user or group name title");
			colBound.HeaderStyle.CssClass = "left";
			colBound.ItemStyle.CssClass = "left";
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			if (!(_IsBoard))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "COLLECTIONS";
				colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsAdvancedGrid.Columns.Add(colBound);
			}
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ADDFLD";
			if (_IsBoard)
			{
				colBound.HeaderText = "Add Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITFLD";
			if (_IsBoard)
			{
				colBound.HeaderText = "Edit Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DELETEFLD";
			if (_IsBoard)
			{
				colBound.HeaderText = "Delete Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
			}
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			if (!(_IsBoard))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "TRAVERSE";
				colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.ItemStyle.Wrap = false;
				PermissionsAdvancedGrid.Columns.Add(colBound);
			}
			
			SiteAPI m_refSiteApi = new SiteAPI();
			SettingsData setting_data = new SettingsData();
			setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
			if (setting_data.EnablePreApproval)
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ModifyPreapproval";
				colBound.HeaderText = "Modify Preapproval";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				PermissionsAdvancedGrid.Columns.Add(colBound);
			}
			
			DataTable dt = new DataTable();
			DataRow dr;
			bool bInherited = false;
			if (_ItemType == "folder")
			{
				bInherited = _FolderData.Inherited;
			}
			else
			{
				bInherited = _ContentData.IsInherited;
			}
			
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("COLLECTIONS", typeof(string)));
			dt.Columns.Add(new DataColumn("ADDFLD", typeof(string)));
			dt.Columns.Add(new DataColumn("EDITFLD", typeof(string)));
			dt.Columns.Add(new DataColumn("DELETEFLD", typeof(string)));
			dt.Columns.Add(new DataColumn("TRAVERSE", typeof(string)));
			
			if (setting_data.EnablePreApproval)
			{
				dt.Columns.Add(new DataColumn("ModifyPreapproval", typeof(string)));
			}
			
			int i = 0;
			
			if (!(userpermission_data == null))
			{
				for (i = 0; i <= userpermission_data.Length - 1; i++)
				{
					dr = dt.NewRow();
					
					if (userpermission_data[i].GroupId != -1)
					{
						if (bInherited)
						{
							dr[0] = "<span class=\"membershipGroup\">" + userpermission_data[i].GroupName + "</span>";
						}
						else
						{
							dr[0] = "<span class=\"cmsGroup\">";
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=group&PermID=" + userpermission_data[i].GroupId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete group permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'group\',\'" + _ItemType + "\');\">" + userpermission_data[i].DisplayGroupName + "</a>";
							dr[0] += "</span>";
						}
						
					}
					else
					{
						if (_IsMembership)
						{
							dr[0] = "<span class=\"membershipUser\">" + userpermission_data[i].DisplayUserName + "</span>";
						}
						else
						{
							dr[0] = "<span class=\"cmsUser\">";
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=DoDeletePermissions&type=" + _ItemType + "&id=" + _Id + "&base=user&PermID=" + userpermission_data[i].UserId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("delete user permissions") + "\' onclick=\"return ConfirmDeletePermissions(\'user\',\'" + _ItemType + "\');\">" + userpermission_data[i].DisplayUserName + "</a>";
							dr[0] += "</span>";
						}
					}
					
					if (!(_IsBoard))
					{
						dr[1] = CheckPermission(userpermission_data[i].IsCollections);
					}
					dr[2] = CheckPermission(userpermission_data[i].CanAddFolders);
					dr[3] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanEditFolders));
					dr[4] = CheckPermission(System.Convert.ToBoolean(userpermission_data[i].CanDeleteFolders));
					if (!(_IsBoard))
					{
						dr[5] = CheckPermission(userpermission_data[i].CanTraverseFolders);
						if (setting_data.EnablePreApproval)
						{
							dr[6] = CheckPermission(userpermission_data[i].CanEditApprovals);
						}
					}
					dt.Rows.Add(dr);
				}
			}
			DataView dv = new DataView(dt);
			PermissionsAdvancedGrid.DataSource = dv;
			PermissionsAdvancedGrid.DataBind();
		}
		
		private void DeletePermissionsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string WorkareaTitlebarTitle = "";
			
			if (_ItemType == "folder")
			{
				WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("delete folder permissions") + " \"" + _FolderData.Name + "\"");
			}
			else
			{
				WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("delete content permissions") + " \"" + _ContentData.Title + "\"");
			}
			txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle);
			result.Append("<table><tr>");
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ApplicationImagePath + "../UI/Icons/back.png", (string) ("content.aspx?action=ViewPermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true));
            result.Append(_StyleHelper.GetButtonEventsWCaption(_ApplicationImagePath + "../UI/Icons/delete.png", "javascript:Import(this);", _MessageHelper.GetMessage("delete folder permissions"), _MessageHelper.GetMessage("delete folder permissions"), "", StyleHelper.DeleteButtonCssClass,true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			if (_IsBoard)
			{
				result.Append(_StyleHelper.GetHelpButton("deleteboardperms", ""));
			}
			else
			{
				result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
			}
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterResources()
		{
			//CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
			
			//JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			
		}
		
		#endregion
		#region Utilities
		protected void SetServerSideJSVariable()
		{
			ltr_contLang.Text = _ContentLanguage.ToString();
			ltr_id.Text = _Id.ToString();
			ltr_itemType.Text = _ItemType.ToString();
			ltr_isMembership.Text = _IsMembership.ToString();
            ltr_delAlertNoSel.Text = _MessageHelper.GetMessage("js:del permission alert");
		}
		#endregion
		
		
	}
