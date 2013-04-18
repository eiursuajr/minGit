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
//using Ektron.Cms.Common.EkConstants;

	public partial class viewpermissions : System.Web.UI.UserControl
	{
		
		
		#region Member Variables
		
		protected bool _IsDiscussionBoardOrDiscussionForum = false;
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
		protected string _ItemType = "";
		protected ContentData _ContentData;
		protected bool _IsMembership = false;
		protected string _Base = "";
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			_MessageHelper = _ContentApi.EkMsgRef;
			ddlUserType.Attributes.Add("style", "margin: .5em 0em .5em .5em;");
			if (this.IsPostBack)
			{
				_IsMembership = System.Convert.ToBoolean((Request.Form[ddlUserType.UniqueID] == "membership") ? true : false);
			}
			else
			{
				string membership = (string) ((string.IsNullOrEmpty(Request.QueryString["membership"])) ? "false" : (Request.QueryString["membership"]));
				bool.TryParse(membership, out _IsMembership);
			}
			
			
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			
			RegisterResources();
			liCmsUsers.Text = _MessageHelper.GetMessage("lbl view cms users");
			liCmsUsers.Value = "standard";
			liCmsUsers.Selected = ! _IsMembership ? true : false;
			liMembershipUsers.Text = _MessageHelper.GetMessage("lbl view memberShip users");
			liMembershipUsers.Value = "membership";
			liMembershipUsers.Selected = _IsMembership ? true : false;
			
			phAdvancedTab.Visible = _IsMembership ? false : true;
			phAdvancedContent.Visible = _IsMembership ? false : true;
			
		}
		
		#endregion
		
		#region Helpers
		
		public bool ViewPermission()
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
			if (!(Request.QueryString["base"] == null))
			{
				_Base = Request.QueryString["base"].Trim().ToLower();
			}
			_CurrentUserId = _ContentApi.UserId;
			_AppImgPath = _ContentApi.AppImgPath;
			_SitePath = _ContentApi.SitePath;
			_EnableMultilingual = _ContentApi.EnableMultilingual;
			Display_ViewPermissions();
            return true;
		}
		
		#endregion
		
		#region PERMISSION - ViewPermissions
		
		private bool IsAllowed()
		{
			bool bFolderUserAdmin = false;
			
			_PermissionData = _ContentApi.LoadPermissions(_Id, _ItemType, 0);
			if (_Id >= 0)
			{
				if (this._ItemType != "folder")
				{
					if (_ContentData == null)
					{
						_ContentData = _ContentApi.GetContentById(_Id, 0);
					}
					bFolderUserAdmin = _PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_ContentData.FolderId, 0, false);
				}
				else
				{
					bFolderUserAdmin = _PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false);
				}
			}
			else
			{
				bFolderUserAdmin = _PermissionData.IsAdmin;
			}
			return bFolderUserAdmin;
		}
		
		private void Display_ViewPermissions()
		{
			FolderData folder_sub_data;
			string strMsg = "";
			bool bPrivate = false;
			if (! IsAllowed())
			{
				Response.Redirect("dashboard.aspx", false);
				return;
			}
			
			if (_ItemType == "folder")
			{
				_FolderData = _ContentApi.GetFolderById(_Id);
				if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard) || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum))
				{
					_IsDiscussionBoardOrDiscussionForum = true;
				}
				if (_FolderData.Inherited)
				{
					folder_sub_data = _ContentApi.GetFolderById(_FolderData.InheritedFrom);
				}
				bPrivate = _FolderData.PrivateContent;
			}
			else
			{
				_ContentData = _ContentApi.GetContentById(_Id, 0);
				if (_ContentData.IsInherited)
				{
					folder_sub_data = _ContentApi.GetFolderById(_ContentData.InheritedFrom);
				}
				bPrivate = _ContentData.IsPrivate;
			}
			ViewPermissionsToolBar();
			_PageData = new Collection();
			
			_PageData.Add("", "UserID", null, null);
			_PageData.Add("", "UserGroupList", null, null);
			_PageData.Add(_Id, "ItemID", null, null);
			_PageData.Add(_ItemType, "RequestType", null, null);
			
			
			this.uxPaging.Visible = false;
			UserPermissionData[] userpermission_data;
			
			if (_IsMembership)
			{
				userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.All);
			}
			else
			{
				userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.All);
			}
			
			//Note:Server side paging cannot be implemented for GetUserPermissions, the result set is being
			//built with two seperate SQL queries (Refer: EkContentRw>GetOrderedItemPermissionsv2_0) and
			//needs complex changes which isn't worth for this UseCase.
			//UseCase: Very rare the folder permissions will be have 20 - 50 users and they would rather prefer assigning UserGroup.
			if (userpermission_data != null)
			{
				userpermission_data = EnableClientSidePaging(userpermission_data);
			}
			
			if (_ItemType == "folder")
			{
				if (!(_IsDiscussionBoardOrDiscussionForum))
				{
					strMsg = _MessageHelper.GetMessage("folder is private msg");
				}
				else
				{
					divPrivateContent.Visible = false;
				}
			}
			else
			{
				strMsg = _MessageHelper.GetMessage("content is private msg");
			}
			
			this.cbPrivateContent.Attributes.Add("onclick", "return CheckPrivateContent(this, " + _Id + ",\'" + _ItemType + "\');");
			if (bPrivate)
			{
				this.cbPrivateContent.Checked = true;
			}
			this.lblPrivateContent.Text = strMsg;
			
			if ((_ItemType == "folder" || _ItemType == "content") && (_Id > 0))
			{
				this.cbInheritedPermissions.Attributes.Add("onclick", "return CheckInheritance(this, " + _Id + ",\'" + _ItemType + "\');");
				this.cbInheritedPermissions.Checked = false;
				this.lblInheritedPermissions.Text = _MessageHelper.GetMessage("allow permission inheritance");
				
				if (_ItemType.ToLower() == "folder")
				{
					if (_FolderData.Inherited)
					{
						this.cbInheritedPermissions.Checked = true;
					}
				}
				else if (_ItemType.ToLower() == "content")
				{
					if (_ContentData.IsInherited)
					{
						this.cbInheritedPermissions.Checked = true;
					}
				}
			}
			else
			{
				this.cbInheritedPermissions.Visible = false;
				this.lblInheritedPermissions.Visible = false;
			}
			if (_IsDiscussionBoardOrDiscussionForum && _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard))
			{
				divInheritedPermissions.Visible = false;
			}
			
			Populate_ViewPermissionsGenericGrid(userpermission_data);
			if (! _IsMembership)
			{
				Populate_ViewPermissionsAdvancedGrid(userpermission_data);
			}
		}
		
		private void Populate_ViewPermissionsGenericGrid(UserPermissionData[] userpermission_data)
		{
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderStyle.CssClass = "left";
			colBound.ItemStyle.CssClass = "left";
			colBound.HeaderText = _MessageHelper.GetMessage("user or group name title");
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "READ";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			colBound.HeaderText = _MessageHelper.GetMessage("generic read only");
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDIT";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Edit title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title");
			}
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ADD";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Add title");
			}
			PermissionsGenericGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DELETE";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Delete title") + " Topic");
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title");
			}
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			PermissionsGenericGrid.Columns.Add(colBound);
			if (!(_IsDiscussionBoardOrDiscussionForum))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "RESTORE";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title");
				PermissionsGenericGrid.Columns.Add(colBound);
			}
			
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GREAD";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply");
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDFILE";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil");
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADD";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate");
				PermissionsGenericGrid.Columns.Add(colBound);
				
			}
			else
			{
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GREAD";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Library title") + " " + _MessageHelper.GetMessage("generic read only"));
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADD";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Images"));
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDFILE";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Files"));
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "GADDHYP";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = (string) (_MessageHelper.GetMessage("generic Add title") + " " + _MessageHelper.GetMessage("generic Hyperlinks"));
				PermissionsGenericGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "OVERWRITELIB";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title");
				PermissionsGenericGrid.Columns.Add(colBound);
			}
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("READ", typeof(string)));
			dt.Columns.Add(new DataColumn("EDIT", typeof(string)));
			dt.Columns.Add(new DataColumn("ADD", typeof(string)));
			dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
			dt.Columns.Add(new DataColumn("RESTORE", typeof(string)));
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
				dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
				dt.Columns.Add(new DataColumn("GADD", typeof(string)));
			}
			else
			{
				dt.Columns.Add(new DataColumn("GREAD", typeof(string)));
				dt.Columns.Add(new DataColumn("GADD", typeof(string)));
				dt.Columns.Add(new DataColumn("GADDFILE", typeof(string)));
			}
			dt.Columns.Add(new DataColumn("GADDHYP", typeof(string)));
			dt.Columns.Add(new DataColumn("OVERWRITELIB", typeof(string)));
			bool permissionInherited = false;
			if (_ItemType == "folder")
			{
				permissionInherited = _FolderData.Inherited;
			}
			else
			{
				permissionInherited = _ContentData.IsInherited;
			}
			
			int i;
			bool isGroup;
			if (!(userpermission_data == null))
			{
				for (i = 0; i <= userpermission_data.Length - 1; i++)
				{
					dr = dt.NewRow();
					isGroup = System.Convert.ToBoolean((userpermission_data[i].GroupId != -1) ? true : false);
					if (isGroup)
					{
						if (_IsMembership)
						{
							dr[0] = "<span class=\"membershipGroup\">";
						}
						else
						{
							dr[0] = "<span class=\"cmsGroup\">";
						}
						if (permissionInherited)
						{
							dr[0] += userpermission_data[i].DisplayGroupName;
						}
						else
						{
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=EditPermissions&type=" + _ItemType + "&id=" + _Id + "&base=group&PermID=" + userpermission_data[i].GroupId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("edit group permissions") + "\'>" + userpermission_data[i].DisplayGroupName + "</a>";
						}
						dr[0] += "</span>";
					}
					else
					{
						if (_IsMembership)
						{
							dr[0] = "<span class=\"membershipUser\">";
						}
						else
						{
							dr[0] = "<span class=\"cmsUser\">";
						}
						if (permissionInherited)
						{
							dr[0] += userpermission_data[i].DisplayUserName;
						}
						else
						{
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=EditPermissions&type=" + _ItemType + "&id=" + _Id + "&base=user&PermID=" + userpermission_data[i].UserId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("edit user permissions") + "\'>" + userpermission_data[i].DisplayUserName + "</a>";
						}
						dr[0] += "</span>";
					}
					
					if (userpermission_data[i].IsReadOnly)
					{
						dr[1] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[1] = "&#160;";
					}
					
					if (userpermission_data[i].CanEdit)
					{
						dr[2] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[2] = "&#160;";
					}
					
					if (userpermission_data[i].CanAdd)
					{
						dr[3] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[3] = "&#160;";
					}
					
					if (userpermission_data[i].CanDelete)
					{
						dr[4] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[4] = "&#160;";
					}
					
					if (! _IsDiscussionBoardOrDiscussionForum)
					{
						if (userpermission_data[i].CanRestore)
						{
							dr[5] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[5] = "&#160;";
						}
					}
					
					if (userpermission_data[i].IsReadOnlyLib)
					{
						dr[6] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[6] = "&#160;";
					}
					
					if (userpermission_data[i].CanAddToImageLib)
					{
						dr[7] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[7] = "&#160;";
					}
					if (!(_IsDiscussionBoardOrDiscussionForum))
					{
						if (userpermission_data[i].CanAddToImageLib)
						{
							dr[7] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[7] = "&#160;";
						}
						
						if (userpermission_data[i].CanAddToFileLib)
						{
							dr[8] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[8] = "&#160;";
						}
						
						if (userpermission_data[i].CanAddToHyperlinkLib)
						{
							dr[9] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[9] = "&#160;";
						}
						
						if (userpermission_data[i].CanOverwriteLib)
						{
							dr[10] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[10] = "&#160;";
						}
					}
					else
					{
						if (userpermission_data[i].CanAddToFileLib) // add image/file
						{
							dr[7] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[7] = "&#160;";
						}
						
						if (userpermission_data[i].CanAddToImageLib) //moderate
						{
							dr[8] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[8] = "&#160;";
						}
					}
					dt.Rows.Add(dr);
				}
			}
			DataView dv = new DataView(dt);
			PermissionsGenericGrid.DataSource = dv;
			PermissionsGenericGrid.DataBind();
		}
		
		private void Populate_ViewPermissionsAdvancedGrid(UserPermissionData[] userpermission_data)
		{
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound.DataField = "TITLE";
			colBound.HeaderStyle.CssClass = "left";
			colBound.ItemStyle.CssClass = "left";
			colBound.HeaderText = _MessageHelper.GetMessage("user or group name title");
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			if (!(_IsDiscussionBoardOrDiscussionForum))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "COLLECTIONS";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("generic collection title");
				PermissionsAdvancedGrid.Columns.Add(colBound);
			}
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ADDFLD";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = "Add Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title");
			}
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "EDITFLD";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = "Edit Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title");
			}
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DELETEFLD";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				colBound.HeaderText = "Delete Forum";
			}
			else
			{
				colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title");
			}
			PermissionsAdvancedGrid.Columns.Add(colBound);
			
			if (!(_IsDiscussionBoardOrDiscussionForum))
			{
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "TRAVERSE";
				colBound.HeaderStyle.CssClass = "center";
				colBound.ItemStyle.CssClass = "center";
				colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title");
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
			
			bool permissionInherited = false;
			if (_ItemType == "folder")
			{
				permissionInherited = _FolderData.Inherited;
			}
			else
			{
				permissionInherited = _ContentData.IsInherited;
			}
			
			int i;
			bool isGroup;
			if (!(userpermission_data == null))
			{
				for (i = 0; i <= userpermission_data.Length - 1; i++)
				{
					dr = dt.NewRow();
					
					isGroup = System.Convert.ToBoolean((userpermission_data[i].GroupId != -1) ? true : false);
					if (isGroup)
					{
						if (_IsMembership)
						{
							dr[0] = "<span class=\"membershipGroup\">";
						}
						else
						{
							dr[0] = "<span class=\"cmsGroup\">";
						}
						if (permissionInherited)
						{
							dr[0] += userpermission_data[i].DisplayGroupName;
						}
						else
						{
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=EditPermissions&type=" + _ItemType + "&id=" + _Id + "&base=group&PermID=" + userpermission_data[i].GroupId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("edit group permissions") + "\'>" + userpermission_data[i].DisplayGroupName + "</a>";
						}
						dr[0] += "</span>";
					}
					else
					{
						if (_IsMembership)
						{
							dr[0] = "<span class=\"membershipUser\">";
						}
						else
						{
							dr[0] = "<span class=\"cmsUser\">";
						}
						if (permissionInherited)
						{
							dr[0] += userpermission_data[i].DisplayUserName;
						}
						else
						{
							dr[0] += "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=EditPermissions&type=" + _ItemType + "&id=" + _Id + "&base=user&PermID=" + userpermission_data[i].UserId + "&membership=" + _IsMembership.ToString() + "\" title=\'" + _MessageHelper.GetMessage("edit user permissions") + "\'>" + userpermission_data[i].DisplayUserName + "</a>";
						}
						dr[0] += "</span>";
					}
					
					if (!(_IsDiscussionBoardOrDiscussionForum))
					{
						if (userpermission_data[i].IsCollections)
						{
							dr[1] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[1] = "&#160;";
						}
					}
					
					if (userpermission_data[i].CanAddFolders)
					{
						dr[2] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[2] = "&#160;";
					}
					
					if (userpermission_data[i].CanEditFolders)
					{
						dr[3] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[3] = "&#160;";
					}
					
					if (userpermission_data[i].CanDeleteFolders)
					{
						dr[4] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
					}
					else
					{
						dr[4] = "&#160;";
					}
					if (!(_IsDiscussionBoardOrDiscussionForum))
					{
						if (userpermission_data[i].CanTraverseFolders)
						{
							dr[5] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[5] = "&#160;";
						}
					}
					if (setting_data.EnablePreApproval)
					{
						if (userpermission_data[i].CanEditApprovals)
						{
							dr[6] = "<img src=\"" + this._ContentApi.ApplicationPath + "Images/ui/icons/check.png\" alt=\"x\" />";
						}
						else
						{
							dr[6] = "&#160;";
						}
					}
					dt.Rows.Add(dr);
				}
			}
			DataView dv = new DataView(dt);
			PermissionsAdvancedGrid.DataSource = dv;
			PermissionsAdvancedGrid.DataBind();
		}
		
		private void ViewPermissionsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			bool inheritedPermissions = false;
			string WorkareaTitlebarTitle = "";
			if (_ItemType == "folder")
			{
				if (_IsDiscussionBoardOrDiscussionForum)
				{
					if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard))
					{
						WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("view board permissions msg") + " \"" + _FolderData.Name + "\"");
					}
					else
					{
						WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("view forum permissions msg") + " \"" + _FolderData.Name + "\"");
					}
					
				}
				else
				{
					WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("view folder permissions msg") + " \"" + _FolderData.Name + "\"");
				}
			}
			else
			{
				WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("view content permissions msg") + " \"" + _ContentData.Title + "\"");
			}
			divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle);
			if (_ItemType == "folder")
			{
				inheritedPermissions = _FolderData.Inherited;
			}
			else
			{
				inheritedPermissions = _ContentData.IsInherited;
			}
			result.Append("<table><tbody><tr>");

			if (Request.QueryString["type"] == "folder")
			{
				if (_IsDiscussionBoardOrDiscussionForum)
				{
					result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _FolderData.Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
				else
				{
					result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?action=ViewFolder&id=" + _Id + "&LangType=" + _ContentLanguage), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
			}
			else
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=View&id=" + _Id), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			if (inheritedPermissions == false)
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/add.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=SelectPermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&membership=" + _IsMembership.ToString().ToLower()), _MessageHelper.GetMessage("alt add button text (permissions)"), _MessageHelper.GetMessage("btn add permissions"), "", StyleHelper.AddButtonCssClass, true));
				result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/delete.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=DeletePermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&membership=" + _IsMembership.ToString().ToLower()), _MessageHelper.GetMessage("alt delete button text (permissions)"), _MessageHelper.GetMessage("btn delete permissions"), "", StyleHelper.DeleteButtonCssClass));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				result.Append(_StyleHelper.GetHelpButton("viewboardperms", ""));
			}
			else
			{
                if (_ItemType.ToLower() == "content")
                    result.Append(_StyleHelper.GetHelpButton("viewpermissioncontent", ""));
                else
    				result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
			}
			result.Append("</td>");
			result.Append("</tr></tbody></table>");
			divToolBar.InnerHtml = result.ToString();
			
		}
		private UserPermissionData[] EnableClientSidePaging(UserPermissionData[] userpermission_data)
		{
			int currentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage + 1);
			int recordsPerPage = _ContentApi.EkContentRef.RequestInformation.PagingSize;
			int firstRecord = System.Convert.ToInt32((currentPage - 1) * (recordsPerPage) + 1);
			int totalRecords = userpermission_data.Length;
			int lastRecord = currentPage * recordsPerPage;
			int pageSize = _ContentApi.EkContentRef.RequestInformation.PagingSize;
			try
			{
				if (totalRecords > pageSize)
				{
					List<UserPermissionData> dataList = new List<UserPermissionData>(userpermission_data);
					if (lastRecord > totalRecords)
					{
						totalRecords = pageSize - (lastRecord - totalRecords);
                        List<UserPermissionData> _aDataList = dataList;
                        dataList = _aDataList.GetRange(firstRecord - 1, totalRecords);
					}
					else
					{
                        List<UserPermissionData> aDataList = dataList;
                        dataList = aDataList.GetRange(firstRecord - 1, pageSize);
					}
					this.uxPaging.Visible = true;
                    this.uxPaging.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((userpermission_data.Length) / _ContentApi.EkContentRef.RequestInformation.PagingSize)));
                    if (Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((userpermission_data.Length) % _ContentApi.EkContentRef.RequestInformation.PagingSize))) > 0)
                    {
                        this.uxPaging.TotalPages = this.uxPaging.TotalPages + 1;
                    }
					this.uxPaging.CurrentPageIndex = this.uxPaging.SelectedPage;
                    List<UserPermissionData> aList = dataList;
                    userpermission_data = aList.ToArray();
					return userpermission_data;
				}
				else
				{
					this.uxPaging.Visible = false;
					return userpermission_data;
				}
			}
			catch (Exception)
			{
				return userpermission_data;
			}
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterResources()
		{
			//CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
			
			//JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
		}
		
		
		#endregion
		
	}
	
