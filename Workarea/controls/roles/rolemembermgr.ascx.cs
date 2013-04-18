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
//using Ektron.Cms.Common.EkEnumeration;
using Microsoft.Security.Application;
using Ektron.Cms.Common;



	public partial class rolemembermgr : System.Web.UI.UserControl
	{
		
		//
		// Role Member Manger
		//
		// This user control will all viewing the members (users nd/or groups)
		// of a particular role, as well as adding or deleting members.
		//
		
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected int ContentLanguage = -1;
		protected string OrderBy = "";
		protected RoleMemberData[] m_RoleMembers;
		protected bool m_bEditing = false;
		protected string m_strAction = "";
		protected string m_strOperation = "";
		protected long m_nRoleId = -1;
		protected string m_strRoleName = "";
		protected string m_strUpdateMode = "";
		protected string m_strUserIds = "";
		protected string m_strGroupIds = "";
		protected string m_strCustomString = "";
		protected bool m_IncludeMembershipItems = false;
		protected RoleRequest role_request;
		protected string m_strSelectedItem = "1";
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected string m_strKeyWords = "";
		protected string m_strSearchText = "";
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{  
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			ContentLanguage = m_refSiteApi.ContentLanguage;
			
			if (! (Request.Form["manager_mode"] == null))
			{
				m_strUpdateMode = Request.Form["manager_mode"];
			}
			if (! (Request.Form["member_user_ids"] == null))
			{
				m_strUserIds = Request.Form["member_user_ids"];
			}
			if (! (Request.Form["member_group_ids"] == null))
			{
				m_strGroupIds = Request.Form["member_group_ids"];
			}
			
			if ((! (Request.QueryString["edit"] == null)) && (Request.QueryString["edit"] == "1"))
			{
				m_bEditing = true;
			}
			else
			{
				m_bEditing = false;
			}
			
			// Determine the role-id based on the action string:
			if (! (Request.QueryString["action"] == null))
			{
				m_strAction = Request.QueryString["action"];
			}
			switch (m_strAction)
			{
				case "aliasedit":
					m_nRoleId = Convert.ToInt64( Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias);
					m_strRoleName = m_refMsg.GetMessage("lbl Alias-Edit");
					break;
				case "aliasadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.UrlAliasingAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl Alias-Admin");
					break;
				case "calendaradmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCalendar);
					m_strRoleName = m_refMsg.GetMessage("lbl Calendar-Admin");
					break;
				case "collectionmenuadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu);
					m_strRoleName = m_refMsg.GetMessage("lbl Collection and Menu Admin");
					break;
                case "collectionadmin":
                    m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection);
                    m_strRoleName = m_refMsg.GetMessage("lbl collection admin");
                    break;
                case "menuadmin":
                    m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu);
                    m_strRoleName = m_refMsg.GetMessage("lbl menu admin");
                    break;
                case "metadataadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMetadata);
					m_strRoleName = m_refMsg.GetMessage("lbl Metadata-Admin");
					break;
				case "masterlayoutcreate":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CreateMasterLayout);
					m_strRoleName = m_refMsg.GetMessage("lbl masterlayout-create");
					break;
				case "ruleedit":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor);
					m_strRoleName = m_refMsg.GetMessage("lbl Business Rule Editor");
					break;
				case "taskcreate":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CreateTask);
					m_strRoleName = m_refMsg.GetMessage("lbl Task-Create");
					break;
				case "taskredirect":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.RedirectTask);
					m_strRoleName = m_refMsg.GetMessage("lbl Task-Redirect");
					break;
				case "taskdelete":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.DeleteTask);
					m_strRoleName = m_refMsg.GetMessage("lbl Task-Delete");
					break;
				case "useradmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers);
					m_strRoleName = m_refMsg.GetMessage("lbl User Admin");
					break;
				case "folderuseradmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers);
					m_strRoleName = m_refMsg.GetMessage("lbl Folder User Admin");
					break;
				case "xliffadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXliff);
					m_strRoleName = m_refMsg.GetMessage("lbl XLIFF admin");
					break;
				case "translationstateadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminTranslationState);
					m_strRoleName = m_refMsg.GetMessage("lbl translation state admin");
					break;
				case "syncadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl sync admin");
					break;
				case "syncuser":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncUser);
					m_strRoleName = m_refMsg.GetMessage("lbl sync user");
					break;
				case "xmlconfigadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXmlConfig);
					m_strRoleName = m_refMsg.GetMessage("lbl Smart Forms Admin");
					break;
				case "templateconfig":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TemplateConfigurations);
					m_strRoleName = m_refMsg.GetMessage("lbl Template Configuration");
					break;
					
				case "personalizationadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize);
					m_strRoleName = m_refMsg.GetMessage("lbl Personalization - Admin");
					m_IncludeMembershipItems = true;
					break;
				case "personalization":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.Personalize);
					m_strRoleName = m_refMsg.GetMessage("lbl Personalization");
					m_IncludeMembershipItems = true;
					break;
				case "personalizationaddonly":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.PersonalizeAddOnly);
					m_strRoleName = m_refMsg.GetMessage("lbl personalization-add/pick webparts from catalog");
					m_IncludeMembershipItems = true;
					break;
				case "personalizationmoveonly":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.PersonalizeMoveOnly);
					m_strRoleName = m_refMsg.GetMessage("lbl personalization: move web parts");
					m_IncludeMembershipItems = true;
					break;
				case "personalizationeditonly":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.PersonalizeEditOnly);
					m_strRoleName = m_refMsg.GetMessage("lbl Personalization - Edit WebParts");
					m_IncludeMembershipItems = true;
					break;
				case "collectionapprovers":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers);
					m_strRoleName = m_refMsg.GetMessage("lbl collection approver");
					m_IncludeMembershipItems = false;
					break;
				case "multivariatetester":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.MultivariateTester);
					m_strRoleName = m_refMsg.GetMessage("lbl multivariate tester");
					m_IncludeMembershipItems = false;
					break;
				case "custompermissions":
					if (! (Request.QueryString["id"] == null))
					{
						m_nRoleId = System.Convert.ToInt64(Request.QueryString["id"]);
						m_strCustomString = (string) ("&id=" + m_nRoleId.ToString());
					}
					if (! (Request.QueryString["name"] == null))
					{
						m_strRoleName = EkFunctions.HtmlEncode(Request.QueryString["name"]);
						m_strCustomString += (string) ("&name=" + m_strRoleName);
					}
					m_IncludeMembershipItems = true;
					break;
				case "taxonomyadministrator":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator);
					m_strRoleName = "Taxonomy Administrator(s)";
					m_IncludeMembershipItems = false;
					break;
				case "messageboardadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.MessageBoardAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl messageboard-admin");
					m_IncludeMembershipItems = true;
					break;
				case "communitygroupadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl role communitygroup-admin");
					m_IncludeMembershipItems = true;
					break;
				case "communitygroupcreate":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate);
					m_strRoleName = m_refMsg.GetMessage("lbl role communitygroup-create");
					m_IncludeMembershipItems = true;
					break;
				case "commerceadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl role commerce-admin");
					m_IncludeMembershipItems = false;
					break;
				case "moveorcopy":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.MoveOrCopy);
					m_strRoleName = m_refMsg.GetMessage("lbl move or copy");
					break;
				case "analyticsviewer":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AnalyticsViewer);
					m_strRoleName = m_refMsg.GetMessage("lbl role analytics-viewer");
					m_IncludeMembershipItems = false;
					break;
				case "communityadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl role community-admin");
					m_IncludeMembershipItems = false;
					break;
				case "searchadmin":
					m_nRoleId = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SearchAdmin);
					m_strRoleName = m_refMsg.GetMessage("lbl role search-admin");
					break;
				default:
					m_nRoleId = -1;
					break;
			}
			
			// Determine operation, viewing/adding/deleting role-members:
			if (! (Request.QueryString["operation"] == null))
			{
				m_strOperation = Request.QueryString["operation"];
			}
			if (Page.IsPostBack)
			{
				m_strSelectedItem = Convert.ToString(Request.Form["selecttype"]);
				m_strAction += (string) ("&selecttype=" + m_strSelectedItem);
				if (Request.Form[isPostData.UniqueID] != "")
				{
					ProcessAction();
					isPostData.Value = "true";
				}
			}
			else
			{
				if (! (Request.QueryString["selecttype"] == null))
				{
					m_strSelectedItem = Request.QueryString["selecttype"];
				}
				ProcessAction();
			}
			
			RegisterResources();
			
		}
		private void ProcessAction()
		{
			switch (m_strOperation)
			{
				case "":
					ViewRoleMembers();
					break;
				case "addmembers":
					if (ProcessUpdating(false))
					{
						ViewRoleMembers();
					}
					else
					{
						AddMembers();
					}
					break;
				case "dropmembers":
					if (ProcessUpdating(true))
					{
						ViewRoleMembers();
					}
					else
					{
						DropMembers();
					}
					break;
				default:
					break;
			}
		}
		public bool ProcessUpdating(bool bDropping)
		{
			RoleMemberData roleMember = new RoleMemberData();
			Ektron.Cms.Content.EkContent contObj;
			contObj = m_refContentApi.EkContentRef;
			string[] strIds;
			int nIndex;
			
			if ((m_nRoleId < 0) || ((m_strUserIds.Length == 0) && (m_strGroupIds.Length == 0)))
			{
				return false; // no processing to do...
			}
			
			// add the selected user members
			if (m_strUserIds.Length >0)
			{
				strIds = m_strUserIds.Split(",".ToCharArray());
				for (nIndex = 0; nIndex <= (strIds.GetLength(0) - 1); nIndex++)
				{
					roleMember.MemberId =Convert.ToInt64( strIds[nIndex]);
					//roleMember.MemberName = ""
					roleMember.MemberType = RoleMemberData.RoleMemberType.User;
					if (bDropping)
					{
						contObj.DropRoleMember(m_nRoleId, ref roleMember);
					}
					else
					{
						contObj.AddRoleMember(m_nRoleId, ref roleMember);
					}
				}
			}
			
			// add the selected group members
			if (m_strGroupIds.Length>0)
			{
				strIds = m_strGroupIds.Split(",".ToCharArray());
				for (nIndex = 0; nIndex <= (strIds.GetLength(0) - 1); nIndex++)
				{
					roleMember.MemberId = Convert.ToInt64( strIds[nIndex]);
					//roleMember.MemberName = ""
					roleMember.MemberType = RoleMemberData.RoleMemberType.Group;
					if (bDropping)
					{
						contObj.DropRoleMember(m_nRoleId, ref roleMember);
					}
					else
					{
						contObj.AddRoleMember(m_nRoleId, ref roleMember);
					}
				}
			}
			
			roleMember = null;
			if (m_strOperation != "")
			{
				Response.Redirect((string) ("roles.aspx?action=" + m_strAction + "&id=" + m_nRoleId + "&name=" + m_strRoleName), true);
			}
			return true;
		}
		
		public void ViewRoleMembers()
		{
			if (Page.IsPostBack && Request.Form[isSearchPostData.UniqueID] != "")
			{
				CollectSearchText();
				DisplayUsers();
			}
			else
			{
				Ektron.Cms.Content.EkContent contObj;
				contObj = m_refContentApi.EkContentRef;
				role_request = new RoleRequest();
				role_request.RoleId = m_nRoleId;
				//role_request.IncludeMember = True
				role_request.SearchText = m_strSearchText;
				role_request.IsAssigned = true;
				role_request.RoleType = Convert.ToInt32(m_strSelectedItem);
				role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				role_request.CurrentPage = m_intCurrentPage;
				m_RoleMembers = contObj.GetAllRoleMembers(ref  role_request); //contObj.GetAllRoleMembers(m_nRoleId)
				m_intTotalPages = role_request.TotalPages;
				ViewRoleMembersToolBar();
				Populate_Members_RoleMemberGrid(false);
			}
		}
		
		private void ViewRoleMembersToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl manage members for role")) + ":" + m_strRoleName; // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/add.png", (string)("roles.aspx?action=" + m_strAction + "&operation=addmembers&LangType=" + ContentLanguage + m_strCustomString), m_refMsg.GetMessage("lbl add role member"), m_refMsg.GetMessage("lbl add role member"), " ", StyleHelper.AddButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/delete.png", (string) ("roles.aspx?action=" + m_strAction + "&operation=dropmembers&LangType=" + ContentLanguage + m_strCustomString), m_refMsg.GetMessage("lbl drop role member"), m_refMsg.GetMessage("lbl drop role member"), " ", StyleHelper.DeleteButtonCssClass));
			result.Append(AppendUserGroupDD());
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td nowrap valign=\"top\">&nbsp;&nbsp;&nbsp;<label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label>  <input type=\"text\" class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event);\">");
			result.Append("<input type=\"button\" value=" + m_refMsg.GetMessage("generic Search") + " id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();\" class=\"ektronWorkareaSearch\" title=\"Search Users\" /></td>");
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			if (! (Request.QueryString["action"] == null))
			{
				result.Append(m_refStyle.GetHelpButton((string) ("viewrolemembers_" + Request.QueryString["action"]), ""));
			}
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		private void Populate_Members_RoleMemberGrid(bool bShowCheckBox)
		{
			DataTable dt;
			int idx;
			
			if (!(m_RoleMembers == null))
			{
				RoleMemberGrid.Columns.Clear();
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ROLE_MEMBER_NAME";
				colBound.HeaderText = m_refMsg.GetMessage("alt role member name");
				RoleMemberGrid.Columns.Add(colBound);
				PageSettings();
				dt = new DataTable();
				DataRow dr;
				string strTypeIcon = "";
				string strNameId;
				dt.Columns.Add(new DataColumn("ROLE_MEMBER_NAME", typeof(string)));
				
				for (idx = 0; idx <= m_RoleMembers.Length - 1; idx++)
				{
					strTypeIcon = LoadIcon(m_RoleMembers[idx].MemberType); //IIf(m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.User, "user.png", "users.png")
					dr = dt.NewRow();
					//dr(0) = IIf(bShowCheckBox, "<input type=""checkbox"" name=""frm_fixme"" id=""frm_fixme"">&nbsp;", "")
					if (bShowCheckBox)
					{
						if (m_RoleMembers[idx].MemberType == RoleMemberData.RoleMemberType.User || m_RoleMembers[idx].MemberType == RoleMemberData.RoleMemberType.MemberUser)
						{
							strNameId = (string) ("member_user_id" + m_RoleMembers[idx].MemberId.ToString());
						}
						else
						{
							strNameId = (string) ("member_group_id" + m_RoleMembers[idx].MemberId.ToString());
						}
						dr[0] = "<input type=\"checkbox\" name=\"" + strNameId + "\" id=\"" + strNameId + "\">";
					}
					else
					{
						dr[0] = "";
					}
					dr[0] += "<img src=\"" + m_refSiteApi.AppPath + "images/UI/Icons/" + strTypeIcon + "\" align=\"absbottom\">";
					dr[0] += m_RoleMembers[idx].MemberName;
					dt.Rows.Add(dr);
				}
				DataView dv = new DataView(dt);
				RoleMemberGrid.DataSource = dv;
				RoleMemberGrid.DataBind();
			}
		}
		private void CollectSearchText()
		{
			m_strKeyWords = Request.Form["txtSearch"];
			m_strSearchText = Quote(m_strKeyWords);
		}
		private string Quote(string KeyWords)
		{
			string result = KeyWords;
			if (KeyWords.Length > 0)
			{
				result = KeyWords.Replace("\'", "\'\'");
			}
			return result;
		}
		public void AddMembers()
		{
			Ektron.Cms.Content.EkContent contObj;
			contObj = m_refContentApi.EkContentRef;
			if (Page.IsPostBack && Request.Form[isSearchPostData.UniqueID] != "")
			{
				AddRoleMembersToolBar();
				CollectSearchText();
				DisplayUsers();
			}
			else
			{
				AddRoleMembersToolBar();
				role_request = new RoleRequest();
				role_request.RoleId = m_nRoleId;
				//role_request.IncludeMember = True
				role_request.IsAssigned = false;
				role_request.RoleType = Convert.ToInt32(m_strSelectedItem);
				role_request.SortDirection = "";
				role_request.SortOrder = "";
				role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
				role_request.CurrentPage = m_intCurrentPage;
				//m_RoleMembers = contObj.GetAllNonRoleMembers(m_nRoleId, m_IncludeMembershipItems)
				m_RoleMembers = contObj.GetAllRoleMembers(ref role_request);
				m_intTotalPages = role_request.TotalPages;
				Populate_Members_RoleMemberGrid(true);
			}
		}
		
		private void AddRoleMembersToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Manage Role Members")); // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			//result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/contentEdit.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "onclick=""return true;"""))
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn save"), "onclick=\"submitAddMembers();return true;\"", StyleHelper.EditButtonCssClass, true));
			result.Append(AppendUserGroupDD());
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>&nbsp;&nbsp;&nbsp;<label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label><input type=\"text\" class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event);\">");
			result.Append("<input type=\"button\" value=\"Search\" id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();\" class=\"ektronWorkareaSearch\" title=\"Search Users\" /></td>");
			result.Append(StyleHelper.ActionBarDivider); 
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton((string) ("addrolemembers_" + m_strAction), ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		public void DropMembers()
		{
			Ektron.Cms.Content.EkContent contObj;
			role_request = new RoleRequest();
			role_request.RoleId = m_nRoleId;
			//role_request.IncludeMember = True
			role_request.IsAssigned = true;
			role_request.RoleType = Convert.ToInt32(m_strSelectedItem);
			role_request.SortDirection = "";
			role_request.SortOrder = "";
			role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
			role_request.CurrentPage = m_intCurrentPage;
			contObj = m_refContentApi.EkContentRef;
			DropRoleMembersToolBar();
			// m_RoleMembers = contObj.GetAllRoleMembers(m_nRoleId)
			m_RoleMembers = contObj.GetAllRoleMembers(ref role_request);
			m_intTotalPages = role_request.TotalPages;
			Populate_Members_RoleMemberGrid(true);
		}
		
		private void DropRoleMembersToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Manage Role Members")); // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			//result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "onclick=""return true;"""))
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("btn delete"), m_refMsg.GetMessage("btn delete"), "onclick=\"submitDropMembers();return true;\"", StyleHelper.DeleteButtonCssClass, true));
			result.Append(AppendUserGroupDD());
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton((string) ("droprolemembers_" + m_strAction), ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		private string AppendUserGroupDD()
		{
			StringBuilder result = new StringBuilder();
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td class=\"label\">" + m_refMsg.GetMessage("lbl show") + ":</td>");
			result.Append("<td>");
			result.Append("<select id=\"selecttype\" name=\"selecttype\" onchange=\"submitform();\">");
			result.Append("<option value=\"1\"" + IsSelected("1") + ">" + m_refMsg.GetMessage("generic cms user label") + "</option>");
			result.Append("<option value=\"2\"" + IsSelected("2") + ">" + m_refMsg.GetMessage("cms group title") + "</option>");
			
			if (m_IncludeMembershipItems)
			{
				
				result.Append("<option value=\"3\"" + IsSelected("3") + ">" + m_refMsg.GetMessage("lbl member user") + "</option>");
				result.Append("<option value=\"4\"" + IsSelected("4") + ">" + m_refMsg.GetMessage("lbl member group") + "</option>");
				
			}
			
			result.Append("</select>");
			result.Append("</td>");
			return result.ToString();
		}
		
		private string IsSelected(string val)
		{
			if (val == m_strSelectedItem)
			{
				return (" selected ");
			}
			else
			{
				return ("");
			}
		}
		
		private string LoadIcon(RoleMemberData.RoleMemberType val)
		{
			string result = "user.png";
			if (val == RoleMemberData.RoleMemberType.User)
			{
				result = "user.png";
			}
			else if (val == RoleMemberData.RoleMemberType.Group)
			{
				result = "users.png";
			}
			else if (val == RoleMemberData.RoleMemberType.MemberUser)
			{
				result = "userMembership.png";
			}
			else if (val == RoleMemberData.RoleMemberType.MemberGroup)
			{
				result = "usersMembership.png";
			}
			return result;
		}
		
		private void PageSettings()
		{
			if (m_intTotalPages <= 1)
			{
				VisiblePageControls(false);
			}
			else
			{
				VisiblePageControls(true);
			    TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble (m_intTotalPages))).ToString();
				CurrentPage.Text = m_intCurrentPage.ToString();
				PreviousPage.Enabled = true;
				FirstPage.Enabled = true;
				NextPage.Enabled = true;
				LastPage.Enabled = true;
				if (m_intCurrentPage == 1)
				{
					PreviousPage.Enabled = false;
					FirstPage.Enabled = false;
				}
				else if (m_intCurrentPage == m_intTotalPages)
				{
					NextPage.Enabled = false;
					LastPage.Enabled = false;
				}
			}
		}
		
		private void VisiblePageControls(bool flag)
		{
			TotalPages.Visible = flag;
			CurrentPage.Visible = flag;
			PreviousPage.Visible = flag;
			NextPage.Visible = flag;
			LastPage.Visible = flag;
			FirstPage.Visible = flag;
			PageLabel.Visible = flag;
			OfLabel.Visible = flag;
		}
		
		public void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					m_intCurrentPage = 1;
					break;
				case "Last":
					m_intCurrentPage = int.Parse((string) TotalPages.Text);
					break;
				case "Next":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					break;
			}
			ProcessAction();
			isPostData.Value = "true";
		}
		private void DisplayUsers()
		{
			Ektron.Cms.Content.EkContent contObj;
			
			if (Request.QueryString["OrderBy"] == "")
			{
				OrderBy = "user_name";
			}
			else
			{
				OrderBy = Request.QueryString["OrderBy"];
			}
			
			contObj = m_refContentApi.EkContentRef;
			role_request = new RoleRequest();
			role_request.RoleId = m_nRoleId;
			role_request.SearchText = m_strSearchText;
			if (m_strOperation == "addmembers")
			{
				role_request.IsAssigned = false;
			}
			else
			{
				role_request.IsAssigned = true;
			}
			role_request.RoleType = Convert.ToInt32(m_strSelectedItem);
			role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
			role_request.CurrentPage = m_intCurrentPage;
			m_RoleMembers = contObj.GetAllRoleMembers(ref role_request);
			m_intTotalPages = role_request.TotalPages;
			
			if (m_strOperation == "")
			{
				ViewRoleMembersToolBar();
			}
			
			if (m_strOperation == "addmembers")
			{
				Populate_Members_RoleMemberGrid(true);
			}
			else
			{
				Populate_Members_RoleMemberGrid(false);
			}
			
		}
		
		private void RegisterResources()
		{
			// register JS
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareSearchBoxInputLabelInitJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			
			// register CSS
		}
		
	}
	
	
	public class RoleHelper
	{
		
		protected string m_formalName = "";
		
	}

