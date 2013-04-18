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


	public partial class adreports : System.Web.UI.Page
	{
		
		
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected string CmsUserIcon = "";
		protected string MemberShipUserIcon = "";
		protected string CmsGroupIcon = "";
		protected string MemberShipGroupIcon = "";
		protected SettingsData settings_data;
		protected SiteAPI m_refSiteApi;
		protected UserAPI m_refUserApi;
		protected AdSyncData sync_data;
		protected int m_intMax = 5;
		protected string m_strPageAction = "";
		const string INPUTCLASS = "ektronTextXXSmall";
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			try
			{
				//Put user code to initialize the page here
				TR_count.Visible = false;
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				m_refMsg = m_refSiteApi.EkMsgRef;
				AppImgPath = m_refUserApi.AppImgPath;
				CmsUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/user.png\" valign=\"absbottom\" title=\"CMS User\">";
				MemberShipUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/userMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				CmsGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/users.png\" valign=\"absbottom\" title=\"CMS Group\">";
				MemberShipGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/usersMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				AppPath = m_refUserApi.AppPath;
				CmsUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/user.png\" valign=\"absbottom\" title=\"CMS User\">";
				MemberShipUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/userMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				CmsGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/users.png\" valign=\"absbottom\" title=\"CMS Group\">";
				MemberShipGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/usersMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				CmsUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/user.png\" valign=\"absbottom\" title=\"CMS User\">";
				MemberShipUserIcon = "<img src=\"" + AppImgPath + "../UI/Icons/userMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				CmsGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/users.png\" valign=\"absbottom\" title=\"CMS Group\">";
				MemberShipGroupIcon = "<img src=\"" + AppImgPath + "../UI/Icons/usersMembership.png\" valign=\"absbottom\" title=\"MemberShip User\">";
				if (!(Request.QueryString["action"] == null))
				{
					if (Request.QueryString["action"] != "")
					{
						m_strPageAction = Request.QueryString["action"].ToLower();
					}
				}
				StyleSheetJS.Text = m_refStyle.GetClientScript();
				Utilities.ValidateUserLogin();
				RegisterResources();
				
				if (!(Page.IsPostBack))
				{
					switch (m_strPageAction)
					{
						case "desynchgroups":
							DeSynchGroups();
							break;
						case "desynchusers":
							DeSynchUsers();
							break;
						case "viewallreporttypes":
							ViewAllReportTypes();
							break;
						case "getusersforsynch":
							GetUsersForSync();
							break;
						case "getgroupsforsynch":
							GetGroupsForSync();
							break;
						case "getrelationshipsforsynch":
							GetRelationshipsForSync();
							break;
					}
				}
				else
				{
					switch (m_strPageAction)
					{
						case "getusersforsynch":
							Process_SynchCMSUsersToAD();
							break;
						case "getgroupsforsynch":
							Process_SynchCMSGroupsToAD();
							break;
						case "getrelationshipsforsynch":
							Process_SynchCMSRelationShipsToAD();
							break;
						case "desynchusers":
							Process_DeSynchUsers();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void DeSynchGroups()
		{
			try
			{
				TR_count.Visible = true;
				string usersForm = Request.Form["submitted"];
				AdDeSyncGroupData[] result;
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				if (usersForm != "")
				{
					result = m_refUserApi.DeSynchUserGroups(true);
					Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
					
				}
				else
				{
					settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
					
					result = m_refUserApi.DeSynchUserGroups(false);
					
					if (result == null)
					{
						Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
					}
					System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "CMSUSER";
					colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name");
					colBound.HeaderStyle.Width = Unit.Percentage(30);
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "AT";
					colBound.HeaderText = "@";
					colBound.HeaderStyle.Width = Unit.Percentage(2);
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "TITLE";
					colBound.HeaderText = m_refMsg.GetMessage("domain title");
					colBound.HeaderStyle.Width = Unit.Percentage(30);
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "USER";
					colBound.HeaderText = m_refMsg.GetMessage("unique group name");
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					DataTable dt = new DataTable();
					DataRow dr;
					dt.Columns.Add(new DataColumn("CMSUSER", typeof(string)));
					dt.Columns.Add(new DataColumn("AT", typeof(string)));
					dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
					dt.Columns.Add(new DataColumn("USER", typeof(string)));
					
					int i = 0;
					long currentUserID = m_refUserApi.UserId;
					if (!(settings_data.ADAuthentication == 1))
					{
						for (i = 0; i <= result.Length - 1; i++)
						{
							dr = dt.NewRow();
							dr[0] = result[i].OldGroupName;
							dr[1] = "@";
							dr[2] = result[i].OldGroupDomain;
							dr[3] = result[i].NewGroupName;
							dt.Rows.Add(dr);
						}
						
						AdReportsGrid.ShowFooter = true;
						
					}
					usercount.Value = i.ToString() + 1;
					DataView dv = new DataView(dt);
					AdReportsGrid.DataSource = dv;
					AdReportsGrid.DataBind();
				}
				
				DeSynchGroupsToolBar();
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void DeSynchGroupsToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("make groups unique"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (groups)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'desynchgroups\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void DeSynchUsers()
		{
			try
			{
				TR_count.Visible = true;
				string usersForm = Request.Form["submitted"];
				AdDeSyncUserData[] result;
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				if (usersForm != "")
				{
					result = m_refUserApi.DeSynchUsers(true);
					Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
					
				}
				else
				{
					settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
					
					result = m_refUserApi.DeSynchUsers(false);
					
					if (result == null)
					{
						Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
					}
					System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "CMSUSER";
					colBound.HeaderText = m_refMsg.GetMessage("generic Username");
					colBound.HeaderStyle.Width = Unit.Percentage(25);
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "AT";
					colBound.HeaderText = "@";
					colBound.HeaderStyle.Width = Unit.Percentage(2);
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "TITLE";
					colBound.HeaderText = m_refMsg.GetMessage("domain title");
					colBound.HeaderStyle.Width = Unit.Percentage(25);
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					colBound = new System.Web.UI.WebControls.BoundColumn();
					colBound.DataField = "USER";
					colBound.HeaderText = m_refMsg.GetMessage("unique username");
					colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
					colBound.ItemStyle.Wrap = false;
					colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
					AdReportsGrid.Columns.Add(colBound);
					
					DataTable dt = new DataTable();
					DataRow dr;
					dt.Columns.Add(new DataColumn("CMSUSER", typeof(string)));
					dt.Columns.Add(new DataColumn("AT", typeof(string)));
					dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
					dt.Columns.Add(new DataColumn("USER", typeof(string)));
					
					int i = 0;
					long currentUserID = m_refUserApi.UserId;
					
					if (!(settings_data.ADAuthentication == 1))
					{
						for (i = 0; i <= result.Length - 1; i++)
						{
							dr = dt.NewRow();
							dr[0] = result[i].OldUserName;
							dr[1] = "@";
							dr[2] = result[i].OldUserDomain;
							dr[3] = result[i].NewUserName;
							dt.Rows.Add(dr);
						}
						
						AdReportsGrid.ShowFooter = true;
						
					}
					usercount.Value = i.ToString() + 1;
					DataView dv = new DataView(dt);
					AdReportsGrid.DataSource = dv;
					AdReportsGrid.DataBind();
				}
				DeSynchUsersToolBar();
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void DeSynchUsersToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("make users unique"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (users)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'desynchusers\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void GetRelationshipsForSync()
		{
			try
			{
				TR_count.Visible = true;
				UserGroupData[] user_group_data;
				if (Request.QueryString["max"] != null)
				{
					if (Request.QueryString["max"] != "")
					{
						m_intMax = Convert.ToInt32(Request.QueryString["max"]);
					}
				}
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
				user_group_data = m_refUserApi.GetCMSRelationshipsToSync(m_intMax);
				if (user_group_data == null)
				{
					user_group_data = (Ektron.Cms.UserGroupData[])Array.CreateInstance(typeof(UserGroupData), 0);
				}
				
				if (user_group_data == null)
				{
					Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
				}
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "CMSUSER";
				colBound.HeaderText = m_refMsg.GetMessage("generic Username");
				colBound.HeaderStyle.Width = Unit.Percentage(40);
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ADUSER";
				colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name");
				colBound.HeaderStyle.Width = Unit.Percentage(50);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "DELETE";
				colBound.HeaderText = m_refMsg.GetMessage("generic Delete title");
				colBound.HeaderStyle.Width = Unit.Percentage(10);
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				DataTable dt = new DataTable();
				DataRow dr = null;
				dt.Columns.Add(new DataColumn("CMSUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("ADUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
				int e1count = 2;
				int e2count = 3;
				
				int i = 0;
				long currentUserID = m_refUserApi.UserId;
				if (user_group_data.Length == m_intMax)
				{
					TD_count.InnerHtml = m_intMax + " " + m_refMsg.GetMessage("ad relationships displayed") + " <a href=\"adreports.aspx?action=GetRelationshipsForSynch&max=0\">" + m_refMsg.GetMessage("generic Show All") + "</a><br><br>";
				}
				if (settings_data.ADIntegration)
				{
					for (i = 0; i <= user_group_data.Length - 1; i++)
					{
						dr = dt.NewRow();
						dr[0] += "<input type=\"hidden\" name=\"userid" + (i + 1) + "\" value=\"" + user_group_data[i].UserId + "\">";
						dr[0] += "<input type=\"hidden\" name=\"groupid" + (i + 1) + "\" value=\"" + user_group_data[i].GroupId + "\">";
						dr[0] += user_group_data[i].UserName;
						dr[1] = user_group_data[i].GroupName;
						dr[2] = "<input type=\"checkbox\" name=\"delete" + (i + 1) + "\" value=\"delete\">";
						dt.Rows.Add(dr);
					}
					
					AdReportsGrid.ShowFooter = true;
					
					e1count = e1count + 5;
					e2count = e2count + 5;
					
				}
				usercount.Value = i.ToString() + 1;
				DataView dv = new DataView(dt);
				AdReportsGrid.DataSource = dv;
				AdReportsGrid.DataBind();
				GetRelationshipsForSyncToolBar();
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void GetRelationshipsForSyncToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match r to ad title"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (relationships)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'synchrelationships\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
				// result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetRelationshipsForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void GetGroupsForSync()
		{
			try
			{
				TR_count.Visible = true;
				GroupData[] group_data;
				if (Request.QueryString["max"] != null)
				{
					if (Request.QueryString["max"] != "")
					{
                        m_intMax = Convert.ToInt32(Request.QueryString["max"]);
					}
				}
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
				group_data = m_refUserApi.GetCMSGroupsToSync(m_intMax);
				if (group_data == null)
				{
					Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
					return;
				}
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "CMSUSER";
				colBound.HeaderText = m_refMsg.GetMessage("cms group name");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(20);
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ADUSER";
				colBound.HeaderText = m_refMsg.GetMessage("ad group name");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(20);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "AT";
				colBound.HeaderText = "@";
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(2);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "DOMAIN";
				colBound.HeaderText = m_refMsg.GetMessage("ad domain");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(25);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "SEARCH";
				colBound.HeaderText = m_refMsg.GetMessage("generic Search");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "DELETE";
				colBound.HeaderText = m_refMsg.GetMessage("generic Delete title");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				DataTable dt = new DataTable();
				DataRow dr;
				dt.Columns.Add(new DataColumn("CMSUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("ADUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("AT", typeof(string)));
				dt.Columns.Add(new DataColumn("DOMAIN", typeof(string)));
				dt.Columns.Add(new DataColumn("SEARCH", typeof(string)));
				dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
				int e1count = 3;
				int e2count = 4;
                string[] domainArray;
				int i = 0;
				int arrayCount = 0;
				long currentUserID = m_refUserApi.UserId;
				if (group_data.Length == m_intMax)
				{
					TD_count.InnerHtml = m_intMax + " " + m_refMsg.GetMessage("ad groups displayed") + " <a href=\"adreports.aspx?action=GetGroupsForSynch&max=0\">" + m_refMsg.GetMessage("generic Show All") + "</a><br><br>";
				}
				if (settings_data.ADIntegration)
				{
					for (i = 0; i <= group_data.Length - 1; i++)
					{
						dr = dt.NewRow();
						domainArray = Strings.Split(group_data[i].AdGroupDomain, ",", -1, 0);
						if (group_data[i].IsMemberShipGroup)
						{
							dr[0] = MemberShipGroupIcon + group_data[i].GroupName;
						}
						else
						{
							dr[0] = CmsGroupIcon + group_data[i].GroupName;
						}
						dr[0] += "<input type=\"hidden\" name=\"userid" + (i + 1) + "\" value=\"" + group_data[i].GroupId + "\">";
						dr[1] = "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"username" + (i + 1) + "\" id=\"username" + (i + 1) + "\" value=\"" + group_data[i].AdGroupName + "\" maxlength=\"255\"></td>";
						dr[2] = "@";
						dr[3] += "";
						if ((domainArray.Length - 1) > 0)
						{
							dr[3] += "<select name=\"sel_domain" + (i + 1) + "\" onchange=\"javascript:document.forms[0].domain" + (i + 1) + ".value = document.forms.synchusers.sel_domain" + (i + 1) + ".options[document.forms[0].sel_domain" + (i + 1) + ".selectedIndex].value;\">";
							dr[3] += "<option value=\"\" selected>" + m_refMsg.GetMessage("multiples found");
							for (arrayCount = 0; arrayCount <= (domainArray.Length - 1); arrayCount++)
							{
								dr[3] += "<option value=\"" + domainArray[arrayCount] + "\">" + domainArray[arrayCount];
							}
							dr[3] += "</select><br>";
							dr[3] += "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"domain" + (i + 1) + "\" id=\"domain" + (i + 1) + "\" maxlength=\"255\">";
						}
						else
						{
							//dr(3)+="<input type=""hidden"">")
							dr[3] += "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"domain" + (i + 1) + "\" id=\"domain" + (i + 1) + "\" value=\"" + group_data[i].AdGroupDomain + "\" maxlength=\"255\">";
						}
						dr[4] = "<a href=\"#\" OnClick=\"javascript:PopUpWindow(\'users.aspx?action=MapCMSUserGroupToAD&id=" + group_data[i].GroupId + "&f=0&e1=" + "username" + (i + 1) + "&e2=" + "domain" + (i + 1) + "&rp=3\',\'Summary\',690,380,1,1);\">" + m_refMsg.GetMessage("generic Search") + "</a>";
						if (group_data[i].GroupId == (int) currentUserID)
						{
							dr[5] = "<input type=\"checkbox\" name=\"delete" + (i + 1) + "\" value=\"delete\" disabled onClick=\"return false;\">";
						}
						else
						{
							dr[5] = "<input type=\"checkbox\" name=\"delete" + (i + 1) + "\" value=\"delete\">";
						}
						dt.Rows.Add(dr);
						e1count = e1count + 4;
						e2count = e2count + 4;
					}
					AdReportsGrid.ShowFooter = true;
				}
				usercount.Value = i.ToString() + 1;
				DataView dv = new DataView(dt);
				AdReportsGrid.DataSource = dv;
				AdReportsGrid.DataBind();
				GetGroupsForSyncToolBar();
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void GetGroupsForSyncToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match g to ad title"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (groups)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'synchusers\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
				// result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetGroupsForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void GetUsersForSync()
		{
			try
			{
				TR_count.Visible = true;
				UserData[] user_data;
				if (Request.QueryString["max"] != null)
				{
					if (Request.QueryString["max"] != "")
					{
						m_intMax = Convert.ToInt32(Request.QueryString["max"]);
					}
				}
				m_refSiteApi = new SiteAPI();
				m_refUserApi = new UserAPI();
				settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
				user_data = m_refUserApi.GetCMSUsersToSync(m_intMax);
				if (user_data == null)
				{
					Response.Redirect("adreports.aspx?action=ViewAllReportTypes", false);
				}
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "CMSUSER";
				colBound.HeaderText = m_refMsg.GetMessage("cms username");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(22);
				colBound.ItemStyle.Width = Unit.Percentage(22);
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ADUSER";
				colBound.HeaderText = m_refMsg.GetMessage("ad username");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(23);
				colBound.ItemStyle.Width = Unit.Percentage(23);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "AT";
				colBound.HeaderText = "@";
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(2);
				colBound.ItemStyle.Width = Unit.Percentage(2);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "DOMAIN";
				colBound.HeaderText = m_refMsg.GetMessage("ad domain");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(25);
				colBound.ItemStyle.Width = Unit.Percentage(25);
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.ItemStyle.Wrap = false;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "SEARCH";
				colBound.HeaderText = m_refMsg.GetMessage("generic Search");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
				colBound.HeaderStyle.Width = Unit.Percentage(13);
				colBound.ItemStyle.Width = Unit.Percentage(13);
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				AdReportsGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "DELETE";
				colBound.HeaderText = m_refMsg.GetMessage("generic Delete title");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.HeaderStyle.Width = Unit.Percentage(15);
				colBound.ItemStyle.Width = Unit.Percentage(15);
				colBound.ItemStyle.Wrap = false;
				AdReportsGrid.Columns.Add(colBound);
				
				DataTable dt = new DataTable();
				DataRow dr;
				dt.Columns.Add(new DataColumn("CMSUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("ADUSER", typeof(string)));
				dt.Columns.Add(new DataColumn("AT", typeof(string)));
				dt.Columns.Add(new DataColumn("DOMAIN", typeof(string)));
				dt.Columns.Add(new DataColumn("SEARCH", typeof(string)));
				dt.Columns.Add(new DataColumn("DELETE", typeof(string)));
				int e1count = 3;
				int e2count = 4;
                int userCount = (user_data != null ? user_data.Length : 0);
				string[] domainArray;
				int arrayCount = 0;
				long currentUserID = m_refUserApi.UserId;
				if (user_data != null && user_data.Length == m_intMax)
				{
					TD_count.InnerHtml = m_intMax + " " + m_refMsg.GetMessage("ad users displayed") + " <a href=\"adreports.aspx?action=GetUsersForSynch&max=0\">" + m_refMsg.GetMessage("generic Show All") + "</a><br><br>";
				}
				if (settings_data.ADAuthentication == 1)
				{
					if (user_data != null)
					{
						for (int i = 0; i <= user_data.Length - 1; i++)
						{
							dr = dt.NewRow();
							domainArray = Strings.Split(user_data[i].Domain, ",", -1, 0);
							if (user_data[i].IsMemberShip)
							{
								dr[0] = MemberShipUserIcon + user_data[i].Username;
							}
							else
							{
								dr[0] = CmsUserIcon + user_data[i].Username;
							}
							dr[0] += "<input type=\"hidden\" name=\"userid" + (i + 1) + "\" value=\"" + user_data[i].Id + "\">";
							dr[1] = "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"username" + (i + 1) + "\" id=\"username" + (i + 1) + "\" value=\"" + user_data[i].AdUserName + "\" maxlength=\"255\"></td>";
							dr[2] = "@";
							dr[3] = "";
							if ((domainArray.Length - 1) > 0)
							{
								dr[3] += "<select name=\"sel_domain" + (i + 1) + "\" onchange=\"javascript:document.forms[0].domain" + (i + 1) + ".value = document.forms.synchusers.sel_domain" + (i + 1) + ".options[document.forms[0].sel_domain" + (i + 1) + ".selectedIndex].value;\">";
								dr[3] += "<option value=\"\" selected>" + m_refMsg.GetMessage("multiples found");
								for (arrayCount = 0; arrayCount <= (domainArray.Length - 1); arrayCount++)
								{
									dr[3] += "<option value=\"" + domainArray[arrayCount] + "\">" + domainArray[arrayCount];
								}
								dr[3] += "</select><br>";
								dr[3] += "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"domain" + (i + 1) + "\" id=\"domain" + (i + 1) + "\" maxlength=\"255\">";
							}
							else
							{
								//dr(3)+="<input type=""hidden"">")
								dr[3] += "<input type=\"text\" class=\"" + INPUTCLASS + "\" name=\"domain" + (i + 1) + "\" id=\"domain" + (i + 1) + "\" value=\"" + user_data[i].Domain + "\" maxlength=\"255\">";
							}
							dr[4] = "<a href=\"#\" OnClick=\"javascript:PopUpWindow(\'users.aspx?action=MapCMSUserToAD&id=" + user_data[i].Id + "&f=0&e1=" + "username" + (i + 1) + "&e2=" + "domain" + (i + 1) + "&rp=3\',\'Summary\',690,380,1,1);\">" + m_refMsg.GetMessage("generic Search") + "</a>";
							if (user_data[i].Id == (int) currentUserID)
							{
								dr[5] = "<input type=\"checkbox\" name=\"delete" + (i + 1) + "\" value=\"delete\" disabled onClick=\"return false;\">";
							}
							else
							{
								dr[5] = "<input type=\"checkbox\" name=\"delete" + (i + 1) + "\" value=\"delete\">";
							}
							dt.Rows.Add(dr);
							e1count = e1count + 4;
							e2count = e2count + 4;
						}
						
						AdReportsGrid.ShowFooter = true;
						
					}
				}
				usercount.Value = userCount.ToString();
				DataView dv = new DataView(dt);
				AdReportsGrid.DataSource = dv;
				AdReportsGrid.DataBind();
				GetUsersForSyncToolBar();
			}
			catch (Exception ex)
			{
				throw (new Exception(ex.Message));
			}
		}
		private void GetUsersForSyncToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match u to ad title"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (users)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'synchusers\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
				// result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetUsersForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void ViewAllReportTypes()
		{
			AdReportsGrid.ShowHeader = false;
			
			m_refSiteApi = new SiteAPI();
			m_refUserApi = new UserAPI();
			settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
			sync_data = m_refUserApi.GetADStatus();
			
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "";
			colBound.HeaderStyle.Height = Unit.Empty;
			if ((settings_data.ADAuthentication == 1) || (settings_data.AdValid == true))
			{
				colBound.ItemStyle.Wrap = false;
			}
			else
			{
				colBound.ItemStyle.Wrap = true;
			}
			colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
			AdReportsGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			if ((sync_data.SyncUsers) || (sync_data.SyncGroups) || (sync_data.SyncRelationships) || (sync_data.DeSyncGroups) || (sync_data.DeSyncUsers))
			{
				
				if (settings_data.ADAuthentication == 1)
				{
					ltr_status.Text += m_refMsg.GetMessage("ad enabled not configured");
				}
				else
				{
					ltr_status.Text += m_refMsg.GetMessage("ad disabled not configured");
				}
				status.Visible = true;
				
				dr = dt.NewRow();
				if (sync_data.SyncUsers)
				{
					dr = dt.NewRow();
					dr[0] = "<a class=\"itemstatus\" href=\"adreports.aspx?action=GetUsersForSynch\" title=\"" + m_refMsg.GetMessage("match cms u to ad") + "\">" + m_refMsg.GetMessage("match cms u to ad") + "</a>";
					dt.Rows.Add(dr);
				}
				if (sync_data.SyncGroups)
				{
					dr = dt.NewRow();
					dr[0] = "<a class=\"itemstatus\" href=\"adreports.aspx?action=GetGroupsForSynch\" title=\"" + m_refMsg.GetMessage("match cms g to ad") + "\">" + m_refMsg.GetMessage("match cms g to ad") + "</a>";
					dt.Rows.Add(dr);
				}
				if (sync_data.SyncRelationships)
				{
					dr = dt.NewRow();
					dr[0] = "<a class=\"itemstatus\" href=\"adreports.aspx?action=GetRelationshipsForSynch\" title=\"" + m_refMsg.GetMessage("match cms r to ad") + "\">" + m_refMsg.GetMessage("match cms r to ad") + "</a>";
					dt.Rows.Add(dr);
				}
				if (sync_data.DeSyncUsers)
				{
					dr = dt.NewRow();
					dr[0] = "<a class=\"itemstatus\" href=\"adreports.aspx?action=DeSynchUsers\" title=\"" + m_refMsg.GetMessage("make u unique") + "\">" + m_refMsg.GetMessage("make u unique") + "</a>";
					dt.Rows.Add(dr);
				}
				if (sync_data.DeSyncGroups)
				{
					dr = dt.NewRow();
					dr[0] = "<a class=\"itemstatus\" href=\"adreports.aspx?action=DeSynchGroups\" title=\"" + m_refMsg.GetMessage("make g unique") + "\">" + m_refMsg.GetMessage("make g unique") + "</a>";
					dt.Rows.Add(dr);
				}
			}
			else
			{
				
				if (settings_data.ADAuthentication == 1)
				{
					ltr_status.Text = m_refMsg.GetMessage("ad enabled and configured");
				}
				else if (settings_data.AdValid == true)
				{
					ltr_status.Text = m_refMsg.GetMessage("alt Active Directory is not enabled and configured.");
				}
				else //
				{
					ltr_status.Text = "<span class=\"important\">" + m_refMsg.GetMessage("entrprise license with AD required msg") + "</span>";
				}
				status.Visible = true;
				
			}
			DataView dv = new DataView(dt);
			AdReportsGrid.DataSource = dv;
			AdReportsGrid.DataBind();
			
			ViewAllReportTypesToolBar();
		}
		private void ViewAllReportTypesToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				result.Append("<table><tr>");
				divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("ad status"));
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton(m_strPageAction, "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void Process_DeSynchUsers()
		{
			AdDeSyncUserData[] result;
			m_refUserApi = new UserAPI();
			try
			{
				result = m_refUserApi.DeSynchUsers(true);
				//Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
				Response.Redirect("users.aspx?backaction=viewallusers&action=viewallusers&grouptype=0&groupid=2&id=2&FromUsers=1", false);
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		private void Process_SynchCMSUsersToAD()
		{
			Collection cUserIDs = new Collection();
			Collection cADUsernames = new Collection();
			Collection cADDomains = new Collection();
			Collection cActions = new Collection();
			int count;
			for (count = 1; count <= System.Convert.ToInt32(Request.Form["usercount"]); count++)
			{
                long userid = 0;
                string adusername = string.Empty;
                string addomain = string.Empty;
                string addelete = string.Empty;

                if (Request.Form["userid" + count.ToString()] != null)
                    userid = System.Convert.ToInt64(Request.Form["userid" + count.ToString()]);

                if (Request.Form["username" + count.ToString()] != null)
                    adusername = Request.Form["username" + count.ToString()].ToString();

                if (Request.Form["domain" + count.ToString()] != null)
                    addomain = Request.Form["domain" + count.ToString()].ToString(); 
 
                if (Request.Form["delete" + count.ToString()] != null)
                    addelete = Request.Form["delete" + count.ToString()].ToString();
           
				if (addelete != "")
				{
					cUserIDs.Add(userid, count.ToString(), null, null);
					cADUsernames.Add("", count.ToString(), null, null);
					cADDomains.Add("", count.ToString(), null, null);
					cActions.Add("delete", count.ToString(), null, null);
				}
				else if ((adusername != "") && (addomain != ""))
				{
					cUserIDs.Add(userid, count.ToString(), null, null);
					cADUsernames.Add(adusername, count.ToString(), null, null);
					cADDomains.Add(addomain, count.ToString(), null, null);
					cActions.Add("map", count.ToString(), null, null);
				}
			}
			m_refUserApi = new UserAPI();
			
			m_refUserApi.SynchCMSUsersToAD(cUserIDs, cADUsernames, cADDomains, cActions, 0);
			Response.Redirect("adreports.aspx?action=GetUsersForSynch", false);
		}
		private void Process_SynchCMSGroupsToAD()
		{
			Collection cUserIDs = new Collection();
			Collection cADUsernames = new Collection();
			Collection cADDomains = new Collection();
			Collection cActions = new Collection();
			int count;
			long userid = 0;
			string adusername = string.Empty;
            string addomain = string.Empty;
            string addelete = string.Empty;
			for (count = 1; count <= System.Convert.ToInt32(Request.Form["usercount"]); count++)
			{
				if (!(Request.Form["userid" + count.ToString()] == null))
				{
                    if(Request.Form["userid" + count.ToString()] != null)
        				userid = System.Convert.ToInt64(Request.Form["userid" + count.ToString()]);
                    if (Request.Form["username" + count.ToString()] != null)
                        adusername = Request.Form["username" + count.ToString()].ToString();
                    if(Request.Form["domain" + count.ToString()] != null)
                        addomain = Request.Form["domain" + count.ToString()].ToString();
                    if(Request.Form["delete" + count.ToString()] != null)
                        addelete = Request.Form["delete" + count.ToString()].ToString();

					if (addelete != "")
					{
						cUserIDs.Add(userid, count.ToString(), null, null);
						cADUsernames.Add("", count.ToString(), null, null);
						cADDomains.Add("", count.ToString(), null, null);
						cActions.Add("delete", count.ToString(), null, null);
					}
					else if ((adusername != "") && (addomain != ""))
					{
						cUserIDs.Add(userid, count.ToString(), null, null);
						cADUsernames.Add(adusername, count.ToString(), null, null);
						cADDomains.Add(addomain, count.ToString(), null, null);
						cActions.Add("map", count.ToString(), null, null);
					}
				}
			}
			
			m_refUserApi = new UserAPI();
			m_refUserApi.SynchCMSGroupsToAD(cUserIDs, cADUsernames, cADDomains, cActions);
			
			Response.Redirect("adreports.aspx?action=GetGroupsForSynch", false);
			
		}
		private void Process_SynchCMSRelationShipsToAD()
		{
			Collection cUserIDs = new Collection();
			Collection cGroupIDs = new Collection();
			Collection cActions = new Collection();
			int count;
			long userid = 0;
			int groupid = 0;
			string addelete = string.Empty;
			for (count = 1; count <= System.Convert.ToInt32(Request.Form["usercount"]); count++)
			{
                if (Request.Form["userid" + count.ToString()] != null)
                    userid = System.Convert.ToInt64(Request.Form["userid" + count.ToString()]);
                if (Request.Form["groupid" + count.ToString()] != null)
                    groupid = int.Parse(Request.Form["groupid" + count.ToString()].ToString());
                if(Request.Form["delete" + count.ToString()] != null)
                    addelete = Request.Form["delete" + count.ToString()].ToString();

				if (addelete != "")
				{
					cUserIDs.Add(userid, count.ToString(), null, null);
					cGroupIDs.Add(groupid, count.ToString(), null, null);
					cActions.Add("delete", count.ToString(), null, null);
				}
			}
			
			m_refUserApi = new UserAPI();
			m_refUserApi.SynchCMSRelationshipsToAD(cUserIDs, cGroupIDs, cActions);
			
			Response.Redirect("adreports.aspx?action=GetRelationshipsForSynch", false);
			
		}
		private void RegisterResources()
		{
			
			Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/empjsfunc.js", "EktronEmpJSFuncJS");
			Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			
		}
	}
