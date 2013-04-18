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



	public partial class customroles : System.Web.UI.UserControl
	{
		
		//
		// Custom Role Manager
		//
		// This user control allows viewing, adding and deleting custom roles.
		//
		protected string m_strKeyWords = "";
		protected string m_strSelectedItem = "-1";
		protected string m_strSearchText = "";
		#region  Web Form Designer Generated Code
		
		
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected int ContentLanguage = -1;
		protected bool m_bEditing = false;
		protected string m_strAction = "";
		protected string m_strOperation = "";
		protected long m_nRoleId = -1;
		protected string m_strRoleName = "";
		protected string m_strRoleNames = "";
		//Protected m_strCustomRoleNames() As String
		protected UserRolePermissionData[] m_UserRolePermissionData;
		protected string m_strUpdateMode = "";
		
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			ContentLanguage = m_refSiteApi.ContentLanguage;
			
			if (! (Request.Form["manager_mode"] == null))
			{
				m_strUpdateMode = Request.Form["manager_mode"];
			}
			if (! (Request.Form["role_names"] == null))
			{
				m_strRoleNames = Request.Form["role_names"];
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
			//Select Case m_strAction
			//	'
			//	' TODO:
			//	'Case "custompermissionsadmin"
			//	'	m_nRoleId = CmsRoleIds
			//	'	m_strRoleName = "Custom Permissions-Admin"
			//	'Case "membershipadmin"
			//	'	m_nRoleId = CmsRoleIds.
			//	'	m_strRoleName = "Membership-Admin"
			//	'
			//	Case Else
			//		m_nRoleId = -1
			//End Select
			
			// Determine operation, viewing/adding/deleting Custom Role:
			if (! (Request.QueryString["operation"] == null))
			{
				m_strOperation = Request.QueryString["operation"];
			}
			
			switch (m_strOperation)
			{
				case "":
					ViewCustomRoles();
					break;
				case "addcustomroles":
					if (ProcessUpdating(false))
					{
						ViewCustomRoles();
					}
					else
					{
						AddCustomRole();
					}
					break;
				case "deletecustomroles":
					if (ProcessUpdating(true))
					{
						ViewCustomRoles();
					}
					else
					{
						DeleteCustomRole();
					}
					break;
				default:
					break;
			}
			
			RegisterResources();
			RunTest();
		}
		private void CollectSearchText()
		{
			m_strKeyWords = Request.Form["txtSearch"];
			m_strSelectedItem = Request.Form["searchlist"];
			if (m_strSelectedItem == "-1")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\' OR last_name like \'%" + Quote(m_strKeyWords) + "%\' OR user_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "last_name")
			{
				m_strSearchText = " (last_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "first_name")
			{
				m_strSearchText = " (first_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
			else if (m_strSelectedItem == "user_name")
			{
				m_strSearchText = " (user_name like \'%" + Quote(m_strKeyWords) + "%\')";
			}
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
		
		public bool ProcessUpdating(bool bDeleting)
		{
			Ektron.Cms.Content.EkContent contObj;
			string[] strUserRoleNamesArray;
			int nIndex;
			bool retValue = false;
			contObj = m_refContentApi.EkContentRef;
			
			if (m_strRoleNames.Length >0)
			{
				strUserRoleNamesArray = m_strRoleNames.Split(",".ToCharArray());
				for (nIndex = 0; nIndex <= (strUserRoleNamesArray.GetLength(0) - 1); nIndex++)
				{
					if (bDeleting)
					{
						retValue = contObj.DeleteRolePermission(strUserRoleNamesArray[nIndex]);
					}
					else
					{
						retValue = contObj.AddRolePermission(strUserRoleNamesArray[nIndex]);
					}
				}
			}
			
			contObj = null;
			return retValue;
		}
		
		public void ViewCustomRoles()
		{
			Ektron.Cms.Content.EkContent contObj;
			if (Page.IsPostBack && Request.Form[isSearchPostData.UniqueID] != "")
			{
				CollectSearchText();
			}
			contObj = m_refContentApi.EkContentRef;
			m_UserRolePermissionData = contObj.GetAllRolePermissions();
			ViewCustomRolesToolBar();
			Populate_CustomRoleListingGrid(false);
		}
		
		private void ViewCustomRolesToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles")) + m_strRoleName; // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/add.png", (string)("roles.aspx?action=" + m_strAction + "&operation=addcustomroles&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt add custom roles"), m_refMsg.GetMessage("alt add custom roles"), "", StyleHelper.AddButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/delete.png", (string) ("roles.aspx?action=" + m_strAction + "&operation=deletecustomroles&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt delete custom role"), m_refMsg.GetMessage("alt delete custom role"), "", StyleHelper.DeleteButtonCssClass));
			//result.Append("<td class=""label"">&nbsp;|&nbsp;<label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label><input type=text class=""ektronTextMedium"" id=txtSearch name=txtSearch value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">")
			//result.Append("<select id=searchlist name=searchlist>")
			//result.Append("<option value=-1" & IsSelected("-1") & ">All</option>")
			//result.Append("<option value=""last_name""" & IsSelected("last_name") & ">Last Name</option>")
			//result.Append("<option value=""first_name""" & IsSelected("first_name") & ">First Name</option>")
			//result.Append("<option value=""user_name""" & IsSelected("user_name") & ">User Name</option>")
			//result.Append("</select><input type=button class=""ektronWorkareaSearch"" value=""Search"" id=btnSearch name=btnSearch onclick=""searchuser();""></td>")
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("ViewCustomRoles", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
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
		
		private void Populate_CustomRoleListingGrid(bool bShowCheckBox)
		{
			DataTable dt = new DataTable();
			int idx;
			string strName;
			long nId;
			string strDesc;
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "CUSTOM_ROLE_NAME";
			colBound.HeaderText = "Custom Role Name";
			colBound.ItemStyle.Wrap = false;
			colBound.HeaderStyle.CssClass = "title-header";
			colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			CustomRoleListingGrid.Columns.Add(colBound);
			
			dt = new DataTable();
			DataRow dr;
			string strNameId;
			dt.Columns.Add(new DataColumn("CUSTOM_ROLE_NAME", typeof(string)));
			
			if ((m_UserRolePermissionData != null) && (m_UserRolePermissionData.GetLength(0) > 0))
			{
				
				for (idx = 1; idx <= m_UserRolePermissionData.GetLength(0) - 1; idx++)
				{
					strName = m_UserRolePermissionData[idx].RolePermissionName;
					nId = m_UserRolePermissionData[idx].RolePermissionId;
					strDesc = m_UserRolePermissionData[idx].RolePermissionDescription;
					
					//strTypeIcon = IIf(m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.User, "user.png", "users.png")
					dr = dt.NewRow();
					//dr(0) = IIf(bShowCheckBox, "<input type=""checkbox"" name=""frm_fixme"" id=""frm_fixme"">&nbsp;", "")
					if (bShowCheckBox)
					{
						strNameId = (string) ("member_user_id" + EkFunctions.HtmlEncode(strName));
						dr[0] = "&nbsp;<input type=\"checkbox\" name=\"" + strNameId + "\" id=\"" + strNameId + "\">&nbsp;" + EkFunctions.HtmlEncode(strName);
					}
					else
					{
						dr[0] = "&nbsp;<a href=\"roles.aspx?action=custompermissions&LangType=" + ContentLanguage + "&id=" + nId + "&name=" + EkFunctions.HtmlEncode(strName) + "\" title=\'" + "Manage Custom Role Members" + "\' onclick=\"return;\">" + AntiXss.HtmlEncode(strName) + "</a>";
					}
					dt.Rows.Add(dr);
				}
				
			}
			
			DataView dv = new DataView(dt);
			CustomRoleListingGrid.DataSource = dv;
			CustomRoleListingGrid.DataBind();
			
		}
		
		public void AddCustomRole()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			Ektron.Cms.Content.EkContent contObj;
			contObj = m_refContentApi.EkContentRef;
			
			AddCustomRoleToolBar();
			
			// render UI:
			sb.Append("<table class=\"ektronGrid\">");
			sb.Append("<tr>");
			sb.Append("<td class=\"label\">");
			sb.Append("Name:");
			sb.Append("</td>");
			sb.Append("<td class=\"value\">");
			sb.Append("<input type=\'text\' id=\'name_text\' name=\'name_text\' size=\'30\' />");
			sb.Append("</td>");
			sb.Append("</tr>");
			sb.Append("</table>");
			Literal1.Text = sb.ToString();
			//m_RoleMembers = contObj.GetAllNonRoleMembers(m_nRoleId)
			//Populate_Members_CustomRoleListingGrid(True)
			
			sb = null;
		}
		
		private void AddCustomRoleToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles")); // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			//result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/contentEdit.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:return true;"""))
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("btn save"), m_refMsg.GetMessage("btn save"), "OnClick=\"javascript:submitAddMembers();return true;\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("AddCustomRoles", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		public void DeleteCustomRole()
		{
			Ektron.Cms.Content.EkContent contObj;
			
			contObj = m_refContentApi.EkContentRef;
			DeleteCustomRoleToolBar();
			m_UserRolePermissionData = contObj.GetAllRolePermissions();
			Populate_CustomRoleListingGrid(true);
		}
		
		private void DeleteCustomRoleToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles")); // m_refMsg.GetMessage("view user groups msg"))
			result.Append("<table><tr>");
			//result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:return true;"""))
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("btn delete"), m_refMsg.GetMessage("btn delete"), "OnClick=\"javascript:submitdeletecustomrole();return true;\"", StyleHelper.DeleteButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("DeleteCustomRoles", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
		}
		
		private void RunTest()
		{
			long nUserId = 7; // user "vs"
			long nFolderId = 74; // folder: "ZapFolder1"
			string strRoleName = "test1";
			bool bFlag = false;
			
			bFlag = m_refContentApi.GetRolePermissionSystem(strRoleName, nUserId);
			bFlag = m_refContentApi.GetRolePermissionFolder(strRoleName, nFolderId, nUserId);
			
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
	

