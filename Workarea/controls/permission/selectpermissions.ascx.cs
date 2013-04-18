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


	public class ApprovedUsersAndGroups
	{
		
		
		#region members
		
		private long _id;
		private string _userOrGroup;
		private string _cmsOrMembership;
		
		#endregion
		
		#region properties
		
		public long ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		
		public string UserOrGroup
		{
			get
			{
				return _userOrGroup;
			}
			set
			{
				_userOrGroup = value;
			}
		}
		
		public string CmsOrMembership
		{
			get
			{
				return _cmsOrMembership;
			}
			set
			{
				_cmsOrMembership = value;
			}
		}
		
		#endregion
		
	}
	
	public partial class selectpermissions : System.Web.UI.UserControl
	{
		
		
		#region enums
		
		private enum FilterType
		{
			Group,
			User
		}
		
		#endregion
		
		#region members
		
		private ContentAPI _ContentApi = new ContentAPI();
		private StyleHelper _StyleHelper = new StyleHelper();
		private EkMessageHelper _MessageHelper;
		private long _Id = 0;
		private FolderData _FolderData;
		private string _AppImgPath = "";
		private long _CurrentUserId = 0;
		private string _PageAction = "";
		private string _OrderBy = "";
		private int _ContentLanguage = -1;
		private int _EnableMultilingual = 0;
		private string _ItemType = "";
		private ContentData _ContentData;
		private string _UserIcon = "";
		private string _GroupIcon = "";
		private bool _IsMembership = false;
		private string _Base = "";
		private bool _IsDiscussionBoardOrDiscussionForum = false;
		private int _CurrentPage = 1;
		private int _TotalPages = 1;
		private string _PermissionSelectType = "1";
		private string m_strKeyWords = "";
		private UserAPI _UserApi = new UserAPI();
		private UserGroupData[] _UserGroupData;
		private string _ApplicationPath;
		private string _SitePath;
		private bool _IsSearch = false;
		private string _SearchText;
		private string[] _AssignedUsers;
		
		#endregion
		
		#region properties
		
		public string SitePath
		{
			get
			{
				return _SitePath;
			}
			set
			{
				_SitePath = value;
			}
		}
		
		public string ApplicationPath
		{
			get
			{
				return _ApplicationPath;
			}
			set
			{
				_ApplicationPath = value;
			}
		}
		
		#endregion
		
		#region events
		
		public selectpermissions()
		{
			
			char[] endSlash = new char[] {'/'};
			this.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(endSlash.ToString().ToCharArray());
			this.SitePath = _ContentApi.SitePath.TrimEnd(endSlash.ToString().ToCharArray());
			_MessageHelper = _ContentApi.EkMsgRef;
			
		}
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//set  up page data
			SetMemberValues();
			
			//register page components
			RegisterJS();
			RegisterCSS();
			
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			
			if (hdnStopExecution.Value == "false")
			{
				hdnStopExecution.Value = "";
				return;
			}
			//set selected users and groups to viewstate
			this.hdnApprovedUsersAndGroups.Value = Request.Form[this.hdnApprovedUsersAndGroups.UniqueID];
			
			//determine if user is searching or loading all users and groups
			_IsSearch = System.Convert.ToBoolean((!string.IsNullOrEmpty(Request.Form["txtSearch"])) ? true : false);
			_IsSearch = System.Convert.ToBoolean(hdnRetrievalMode.Value == "search" ? true : _IsSearch);
			_SearchText = (string) (_IsSearch ? (Request.Form["txtSearch"]) : string.Empty);
			_SearchText = (string) (_SearchText == string.Empty ? this.hdnSearchTerms.Value : _SearchText);
			_PermissionSelectType = (string) (_IsSearch ? (Convert.ToString(Request.Form["selecttype"])) : "1");
			
			//set retrieval mode and type
			this.hdnRetrievalMode.Value = _IsSearch ? "search" : "full";
			this.hdnUserOrGroups.Value = (_PermissionSelectType == "2" || _PermissionSelectType == "4") ? "user" : "group";
			this.hdnSearchTerms.Value = _SearchText;
			if (Request.QueryString["selectType"] != "" && (Request.QueryString["selectType"] != null))
			{
				if (Request.QueryString["selectType"] == "-1")
				{
					_PermissionSelectType = "1";
				}
				else
				{
					_PermissionSelectType = Request.QueryString["selectType"];
				}
				
			}
			//set up datagrid
			_CurrentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage);
			this.GetPermissionData();
			this.uxPermissionsGrid.DataSource = _UserGroupData;
			this.uxPermissionsGrid.DataBind();
			
			
			if (_TotalPages > 1)
			{
				this.uxPaging.TotalPages = _TotalPages;
				this.uxPaging.CurrentPageIndex = this._CurrentPage;
			}
			else
			{
				this.uxPaging.Visible = false;
			}
			
			//set toolbar
			SelectPermissionsToolBar();
		}
		
		protected void uxPermissionsGrid_OnItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			
			UserGroupData userGroupDataItem = (UserGroupData) e.Item.DataItem;
			if (userGroupDataItem != null)
			{
				switch (e.Item.ItemType)
				{
					case ListItemType.Header:
						((HtmlInputCheckBox) (e.Item.Cells[0].FindControl("uxSelectAll"))).Attributes.Add("title", "Select All");
						((Literal) (e.Item.Cells[1].FindControl("uxNameHeader"))).Text = "User or Group Name";
						((Literal) (e.Item.Cells[2].FindControl("uxTypeHeader"))).Text = "User or Group Type";
						break;
					case ListItemType.Item:
					case ListItemType.AlternatingItem:
						((HtmlInputControl) (e.Item.Cells[0].FindControl("uxId"))).Value = GetUserOrGroupId(userGroupDataItem);
						((HtmlInputControl) (e.Item.Cells[0].FindControl("uxIsGroup"))).Value = GetIsGroup(userGroupDataItem);
						((Image) (e.Item.Cells[1].FindControl("uxIcon"))).ImageUrl = GetUserOrGroupIcon(userGroupDataItem);
						((Image) (e.Item.Cells[1].FindControl("uxIcon"))).Attributes.Add("style", "vertical-align:text-top;");
						((HyperLink) (e.Item.Cells[1].FindControl("uxName"))).Text = GetUserOrGroupName(userGroupDataItem);
						((HyperLink) (e.Item.Cells[1].FindControl("uxName"))).ToolTip = "Set permissions for " + GetUserOrGroupName(userGroupDataItem);
						((HyperLink) (e.Item.Cells[1].FindControl("uxName"))).NavigateUrl = GetUserOrGroupLink(userGroupDataItem);
						((Literal) (e.Item.Cells[2].FindControl("uxType"))).Text = GetUserOrGroupType(userGroupDataItem);
						break;
				}
			}
			else if (e.Item.ItemType == ListItemType.Header)
			{
				((HtmlInputCheckBox) (e.Item.Cells[0].FindControl("uxSelectAll"))).Attributes.Add("title", "Select All");
				((Literal) (e.Item.Cells[1].FindControl("uxNameHeader"))).Text = "User or Group Name";
				((Literal) (e.Item.Cells[2].FindControl("uxTypeHeader"))).Text = "User or Group Type";
			}
			else
			{
				e.Item.Visible = false;
			}
		}
		
		#endregion
		
		#region helpers
		
		private void SetMemberValues()
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
			if (!(Request.QueryString["membership"] == null))
			{
				if (Request.QueryString["membership"].Trim().ToLower() != "")
				{
					_IsMembership = Convert.ToBoolean(Request.QueryString["membership"].Trim().ToLower());
				}
			}
			
			if (_IsMembership)
			{
				_UserIcon = "userMembership.png";
				_GroupIcon = "usersMembership.png";
			}
			else
			{
				_UserIcon = "user.png";
				_GroupIcon = "users.png";
			}
			
			if (!(Request.QueryString["base"] == null))
			{
				_Base = Request.QueryString["base"].Trim().ToLower();
			}
			
			_CurrentUserId = _ContentApi.UserId;
			_AppImgPath = _ContentApi.AppImgPath;
			_EnableMultilingual = _ContentApi.EnableMultilingual;
			
		}
		
		private string GetUserOrGroupId(UserGroupData userGroupDataItem)
		{
			return (userGroupDataItem.GroupId != -1 ? (userGroupDataItem.GroupId.ToString()) : (userGroupDataItem.UserId.ToString()));
		}
		
		private string GetIsGroup(UserGroupData userGroupDataItem)
		{
			return (userGroupDataItem.GroupId != -1 ? "true" : "false");
		}
		
		private string GetUserOrGroupIcon(UserGroupData userGroupDataItem)
		{
			bool isGroup = System.Convert.ToBoolean(userGroupDataItem.GroupId != -1 ? true : false);
			return this.ApplicationPath + "/images/ui/icons/" + (isGroup ? _GroupIcon : _UserIcon);
		}
		
		private string GetUserOrGroupName(UserGroupData userGroupDataItem)
		{
			return (userGroupDataItem.GroupId != -1 ? userGroupDataItem.GroupName : userGroupDataItem.UserName);
		}
		
		private string GetUserOrGroupLink(UserGroupData userGroupDataItem)
		{
			string group = this.ApplicationPath + "/content.aspx?LangType=" + _ContentLanguage + "&action=AddPermissions&id=" + _Id + "&type=" + _ItemType + "&base=group&PermID=" + userGroupDataItem.GroupId + "&membership=" + _IsMembership.ToString();
			string user = this.ApplicationPath + "/content.aspx?LangType=" + _ContentLanguage + "&action=AddPermissions&id=" + _Id + "&type=" + _ItemType + "&base=user&PermID=" + userGroupDataItem.UserId + "&membership=" + _IsMembership.ToString();
			return (userGroupDataItem.GroupId != -1 ? group : user);
		}
		
		private string GetUserOrGroupType(UserGroupData userGroupDataItem)
		{
			string type;
			type = (string) (userGroupDataItem.IsMemberShipUser ? "Membership " : "Cms ");
			type = (string) (userGroupDataItem.IsMemberShipGroup ? "Membership " : type);
			return (userGroupDataItem.GroupId != -1 ? type + "Group" : type + "User");
		}
		
		private void GetPermissionData()
		{
			if (_IsSearch)
			{
				GetPermissionDataSearch();
			}
			else
			{
				GetPermissionDataAll();
			}
			
		}
		
		private void GetPermissionDataAll()
		{
			
			//set paging size
			Ektron.Cms.PagingInfo pageInfo = new Ektron.Cms.PagingInfo();
			pageInfo.CurrentPage = _CurrentPage + 1;
			pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
			
			switch (_PermissionSelectType)
			{
				case "1": //Default
					int permissionUserType = System.Convert.ToInt32(_IsMembership ? ContentAPI.PermissionUserType.Membership : ContentAPI.PermissionUserType.Cms);
                    if(_IsMembership)
                        _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "All",ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
                    else
                        _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "All",ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
					break;
				case "2": //CMS User
					_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Users", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
					break;
					//UserGroupDataFilter(FilterType.User)
				case "3": //CMS Group
					_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Groups", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
					break;
					//UserGroupDataFilter(FilterType.Group)
				case "4": //Member User
					_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Users", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
					break;
					//UserGroupDataFilter(FilterType.User)
				case "5": //Member Group
					_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Groups", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.UnAssigned, pageInfo);
					break;
					//UserGroupDataFilter(FilterType.Group)
			}
			
			_TotalPages = pageInfo.TotalPages;
		}
		
		private void GetPermissionDataSearch()
		{
			//search for user or group
			
			switch (_PermissionSelectType)
			{
				case "1": //CMS or Membership User
				case "2":
				case "4":
					_UserGroupData = GetSearchedUsers();
					break;
				case "3": //CMS or Membership Group
				case "5":
					_UserGroupData = GetSearchedGroups();
					break;
			}
			
		}
		
		private void ValidatePermissions()
		{
			if (_ContentApi.IsAdmin() || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers))
			{
				return;
			}
			
			long id;
			if (long.TryParse(Request.QueryString["id"], out id) && _ContentApi.IsARoleMemberForFolder(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers),id,id,false))
			{
				return;
			}
			
			EkException.ThrowException(new Ektron.Cms.Common.CmsException(_MessageHelper.GetMessage("com: user does not have permission")));
		}
		
		private UserGroupData[] GetSearchedUsers()
		{
			ValidatePermissions();
			
			List<UserData> userDataList;
            Ektron.Cms.Common.Criteria<Ektron.Cms.User.UserProperty> userCriteria = new Ektron.Cms.Common.Criteria<Ektron.Cms.User.UserProperty>();
			
			Ektron.Cms.Framework.Users.User userFrameworkApi = new Ektron.Cms.Framework.Users.User(Ektron.Cms.Framework.ApiAccessMode.Admin);
			int i = 0;
			int j = 0;
			int count = 0;
			UserGroupData[] userGroupData = null;
			int permissionUserType = System.Convert.ToInt32(_IsMembership ? ContentAPI.PermissionUserType.Membership : ContentAPI.PermissionUserType.Cms);
			Ektron.Cms.UserGroupData[] emptyUserGroupData = null;
			string assignedUser = (string) hdnAssignedUserGroupIds.Value;
			_AssignedUsers = assignedUser.Split(",".ToCharArray());
			//set paging size
			Ektron.Cms.PagingInfo pageInfo = new Ektron.Cms.PagingInfo();
			if (! this._IsSearch)
			{
				pageInfo.CurrentPage = _CurrentPage + 1;
				pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
			}
			
			//_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", permissionUserType, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
			
			userCriteria.PagingInfo = pageInfo;
			userCriteria.AddFilter(Ektron.Cms.User.UserProperty.UserName, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchText);
            userCriteria.AddFilter(Ektron.Cms.User.UserProperty.IsMemberShip, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _IsMembership);
            userCriteria.AddFilter(Ektron.Cms.User.UserProperty.UserName, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, "internaladmin");
            userCriteria.AddFilter(Ektron.Cms.User.UserProperty.UserName, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, "builtin");
            userCriteria.AddFilter(Ektron.Cms.User.UserProperty.IsDeleted, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, true);
			userDataList = userFrameworkApi.GetList(userCriteria);
			
			while (i < userDataList.Count)
			{
				for (j = 0; j <= _AssignedUsers.Length - 2; j++)
				{
					if (Convert.ToInt64(_AssignedUsers[j]) == userDataList[i].Id)
					{
                        userDataList.Remove(userDataList[i]);
					}
					if (userDataList.Count == 0)
					{
						goto endOfWhileLoop;
					}
				}
				i++;
			}
endOfWhileLoop:
			i = 0;
			if (userDataList.Count > 0)
			{
				if (userDataList.Count == 1)
				{
					userGroupData = new UserGroupData[userDataList.Count + 1];
				}
				else
				{
					userGroupData = new UserGroupData[userDataList.Count - 1 + 1];
				}
				//isAssignedUser = isAssigned(_UserGroupData, userDataList)
				//If (isAssignedUser = False) Then
				foreach (UserData user in userDataList)
				{
                    userGroupData[i] = new UserGroupData();
					userGroupData[i].UserId = user.Id;
					userGroupData[i].UserName = user.Username;
					userGroupData[i].GroupId = -1;
					userGroupData[i].IsMemberShipUser = user.IsMemberShip;
					i++;
				}
				//End If
			}
			//Return IIf(userGroupDataItem.GroupId <> -1, userGroupDataItem.GroupId.ToString(), userGroupDataItem.UserId.ToString())
			if (userGroupData != null)
			{
				for (j = 0; j <= userGroupData.Length - 1; j++)
				{
					if (userGroupData[j] == null)
					{
						count++;
					}
				}
				if (count == userGroupData.Length)
				{
					return emptyUserGroupData;
				}
				else
				{
					return userGroupData;
				}
			}
			_TotalPages = pageInfo.TotalPages;
			
			return emptyUserGroupData;
		}
		private bool isAssigned(UserGroupData[] userGroupData, List<UserData> userDataList)
		{
			
			
			bool returnVal = true;
			foreach (UserGroupData userGroup in userGroupData)
			{
				foreach (UserData user in userDataList)
				{
					if (userGroup.UserId == user.Id)
					{
						returnVal = false;
					}
				}
			}
			return returnVal;
			
		}
		private UserGroupData[] GetSearchedGroups()
		{
			ValidatePermissions();
			
			//set paging size
			Ektron.Cms.PagingInfo pageInfo = new Ektron.Cms.PagingInfo();
			System.Collections.Generic.List<Ektron.Cms.UserGroupData> groupList = new System.Collections.Generic.List<Ektron.Cms.UserGroupData>();
			int i = 0;
			int j;
			pageInfo.CurrentPage = _CurrentPage + 1;
			pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
			UserGroupData[] userGroupData = null;
			
			
			Ektron.Cms.Common.Criteria<Ektron.Cms.Common.UserGroupProperty> userGroupCriteria = new Ektron.Cms.Common.Criteria<Ektron.Cms.Common.UserGroupProperty>();
			userGroupCriteria.PagingInfo = pageInfo;
			userGroupCriteria.AddFilter(Ektron.Cms.Common.UserGroupProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchText);
			userGroupCriteria.AddFilter(Ektron.Cms.Common.UserGroupProperty.IsMembershipGroup, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _IsMembership);
			
			string assignedUser = (string) hdnAssignedUserGroupIds.Value;
			_AssignedUsers = assignedUser.Split(",".ToCharArray());
			groupList = _UserApi.GetUserGroupList(userGroupCriteria);
			
			if (groupList.Count > 0)
			{
				while (i < groupList.Count)
				{
					for (j = 0; j <= _AssignedUsers.Length - 2; j++)
					{
						if (Convert.ToInt64(_AssignedUsers[j]) == groupList[i].GroupId)
						{
							groupList.Remove(groupList[i]);
						}
						if (groupList.Count == 0)
						{
							goto endOfWhileLoop;
						}
					}
					i++;
				}
endOfWhileLoop:
				if (groupList.Count == 1)
				{
					userGroupData = new UserGroupData[groupList.Count + 1];
				}
				else
				{
					userGroupData = new UserGroupData[groupList.Count - 1 + 1];
				}
				i = 0;
				
				//isAssignedUser = isAssigned(_UserGroupData, userDataList)
				//If (isAssignedUser = False) Then
				foreach (UserGroupData groupData in groupList)
				{
                    userGroupData[i] = new UserGroupData();
					userGroupData[i].GroupId = groupData.GroupId;
					userGroupData[i].GroupName = groupData.GroupName;
					userGroupData[i].UserId = -1;
					userGroupData[i].IsMemberShipUser = groupData.IsMemberShipGroup;
					i++;
				}
				//End If
			}
			_TotalPages = pageInfo.TotalPages;
			return userGroupData;
		}
		
		private void UserGroupDataFilter(FilterType KeepOnly)
		{
			int i = 0;
			switch (KeepOnly)
			{
				case FilterType.Group:
					for (i = 0; i <= _UserGroupData.Length - 1; i++)
					{
						if (_UserGroupData[i].UserId != -1)
						{
							Array.Clear(_UserGroupData, i, 1);
						}
					}
					break;
				case FilterType.User:
					for (i = 0; i <= _UserGroupData.Length - 1; i++)
					{
						if (_UserGroupData[i].GroupId != -1)
						{
							Array.Clear(_UserGroupData, i, 1);
						}
					}
					break;
			}
			
		}
		
		private void SelectPermissionsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string WorkareaTitlebarTitle = "";
			
			if (_ItemType == "folder")
			{
				_FolderData = _ContentApi.GetFolderById(_Id);
				if (_FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard))
				{
					_IsDiscussionBoardOrDiscussionForum = true;
				}
				if (_IsDiscussionBoardOrDiscussionForum)
				{
					WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("add board permissions") + " \"" + _FolderData.Name + "\"");
				}
				else
				{
					WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("add folder permissions") + " \"" + _FolderData.Name + "\"");
				}
			}
			else
			{
				_ContentData = _ContentApi.GetContentById(_Id, 0);
				WorkareaTitlebarTitle = (string) (_MessageHelper.GetMessage("add content permissions") + " \"" + _ContentData.Title + "\"");
			}
			txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle);
			result.Append("<table><tr>").Append(Environment.NewLine);
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/back.png", (string) ("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&id=" + _Id + "&type=" + Request.QueryString["type"] + "&membership=" + Request.QueryString["membership"]), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass,true)).Append(Environment.NewLine);
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "../UI/Icons/save.png", "javascript:Ektron.Workarea.Permissions.Add.submit(this);", _MessageHelper.GetMessage("btn save"), _MessageHelper.GetMessage("btn save"), "", StyleHelper.SaveButtonCssClass,true)).Append(Environment.NewLine);
			
			result.Append(AppendUserGroupDD()).Append(Environment.NewLine);
			result.Append("<td nowrap valign=\"top\">&nbsp;|&nbsp;<label for=\"txtSearch\">" + _MessageHelper.GetMessage("generic search") + "</label>").Append(Environment.NewLine);
			result.Append(" <input type=\"text\" class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"Ektron.Workarea.Permissions.Add.Search.checkForReturn(event);\" /></td>").Append(Environment.NewLine);
			result.Append(" <td><input type=\"button\" value=" + _MessageHelper.GetMessage("generic Search") + " id=\"btnSearch\" name=\"btnSearch\" onclick=\"Ektron.Workarea.Permissions.Add.Search.submit();\" class=\"ektronWorkareaSearch\" /></td>").Append(Environment.NewLine);
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			if (_IsDiscussionBoardOrDiscussionForum)
			{
				result.Append(_StyleHelper.GetHelpButton("selectboardperms", ""));
			}
			else
			{
				result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
			}
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
			
		}
		
		private string AppendUserGroupDD()
		{
			StringBuilder result = new StringBuilder();
			result.Append("<td class=\"label\">&nbsp;|&nbsp;" + _MessageHelper.GetMessage("lbl show") + ":</td>");
			result.Append("<td>");
			result.Append(" <select id=\"selecttype\" name=\"selecttype\" onchange=\"setUserGroupView(this);\">");
			result.Append("     <option value=\"-1\"" + IsSelected("1") + ">" + _MessageHelper.GetMessage("lbl select user or group") + "</option>");
			
			if (_IsMembership == false)
			{
				result.Append("     <option value=\"2\"" + IsSelected("2") + ">" + _MessageHelper.GetMessage("generic cms user label") + "</option>");
				result.Append("     <option value=\"3\"" + IsSelected("3") + ">" + _MessageHelper.GetMessage("cms group title") + "</option>");
			}
			else if (_IsMembership == true)
			{
				result.Append("     <option value=\"4\"" + IsSelected("4") + ">" + _MessageHelper.GetMessage("lbl member user") + "</option>");
				result.Append("     <option value=\"5\"" + IsSelected("5") + ">" + _MessageHelper.GetMessage("lbl member group") + "</option>");
			}
			
			result.Append(" </select>");
			result.Append("</td>");
			return result.ToString();
		}
		
		private string IsSelected(string val)
		{
			if (val == _PermissionSelectType)
			{
				return (" selected ");
			}
			else
			{
				return ("");
			}
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterCSS()
		{
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			
		}
		
		private void RegisterJS()
		{
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
			Ektron.Cms.API.JS.RegisterJS(this, this._ContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS");
			
			this.litLanguageId.Text = this._ContentLanguage.ToString();
			ltrLanguageId.Text = this._ContentLanguage.ToString();
            this.ltrId.Text = this._Id.ToString();
			this.litItemType.Text = this._ItemType;
			this.ltrItemType.Text = this._ItemType.ToString();
            this.ltrIsMembership.Text = this._IsMembership.ToString() ;
			this.litIsMembership.Text = this._IsMembership.ToString();
			this.litId.Text = this._Id.ToString();
			this.litNoItemSelected.Text = _MessageHelper.GetMessage("js:no items selected");
		}
		
		#endregion
		
	}
