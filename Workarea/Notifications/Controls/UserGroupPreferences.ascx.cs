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
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;

	public partial class Workarea_Notifications_UserGroupPreferences : System.Web.UI.UserControl
	{
		
		protected StyleHelper _refStyle = new StyleHelper();
		protected EkMessageHelper msgHelper;
		protected CommonApi _refCommonApi = new CommonApi();
		protected ContentAPI _refContentApi = new ContentAPI();
        protected Ektron.Cms.Framework.Notifications.NotificationPreference _notificationPreferenceApi = new Ektron.Cms.Framework.Notifications.NotificationPreference();
		protected System.Collections.Generic.List<NotificationPreferenceData> preferenceList;
		protected NotificationPreferenceData prefData;
        protected Ektron.Cms.Framework.Notifications.NotificationAgentSetting _notificationAgentApi = new Ektron.Cms.Framework.Notifications.NotificationAgentSetting();
		protected System.Collections.Generic.List<NotificationAgentData> agentList;

        protected Ektron.Cms.Framework.Activity.ActivityType _activityListApi = new Ektron.Cms.Framework.Activity.ActivityType();
        protected System.Collections.Generic.List<Ektron.Cms.Activity.ActivityTypeData> activityTypeList;
		
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			msgHelper = _refContentApi.EkMsgRef;
			Ektron.Cms.Content.EkContent objContentRef;
			objContentRef = _refContentApi.EkContentRef;
			string pageMode;
			string strgroupId;
			long groupId;
			//Licensing Check
			if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
			{
				Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
				return;
			}
			//Logged in user check
			if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember(Convert.ToInt64( Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin), _refCommonApi.RequestInformationRef.UserId, false) || _refCommonApi.RequestInformationRef.UserId > 0))
			{
				Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
				return;
			}
			//RegisterResources()
			agentDisabled.Visible = false;
			pageMode = Request.QueryString["mode"];
			strgroupId = Request.QueryString["id"];
			long.TryParse(strgroupId, out groupId);
			string ctrlname = Page.Request.Params["__EVENTTARGET"];
			if (Page.IsPostBack)
			{
				EditGroupsSettings(groupId);
			}
			else
			{
				DisplayGroupSettings(groupId);
			}
			
		}
		private void DisplayGroupSettings(long groupId)
		{
            NotificationAgentCriteria criteria = new NotificationAgentCriteria();
            Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
            activityListCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending; 
			agentList = _notificationAgentApi.GetList(criteria);
			activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
			activityTypeList = _activityListApi.GetList(activityListCriteria);
			UserGroupPrefGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
			if ((agentList != null)&& agentList.Count > 0)
			{
				foreach (NotificationAgentData agentData in agentList)
				{
					if (agentData.IsEnabled)
					{
						if ((agentData.Id) == 1)
						{
							UserGroupPrefGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + msgHelper.GetMessage("sync conflict email") + " </center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
						}
						else if ((agentData.Id) == 2)
						{
							UserGroupPrefGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + msgHelper.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
						}
						else if ((agentData.Id) == 3)
						{
							UserGroupPrefGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + msgHelper.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
						}
					}
				}
				DataTable dt = new DataTable();
				DataRow dr;
				dt.Columns.Add(new DataColumn("EMPTY", typeof(string)));
				dt.Columns.Add(new DataColumn("EMAIL", typeof(string)));
				dt.Columns.Add(new DataColumn("SMS", typeof(string)));
				dt.Columns.Add(new DataColumn("NEWSFEED", typeof(string)));
				LoadPreferenceList(groupId);
				for (int i = 0; i <= activityTypeList.Count - 1; i++)
				{
					dr = dt.NewRow();
					dr["EMPTY"] = activityTypeList[i].Name;
					if (preferenceList.Count > 0)
					{
						foreach (NotificationPreferenceData tempLoopVar_prefData in preferenceList)
						{
							prefData = tempLoopVar_prefData;
							if (CompareIds(activityTypeList[i].Id, 1))
							{
								dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" checked=\"checked\" /></center>";
							}
							else
							{
								dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" /></center>";
							}
							if (CompareIds(activityTypeList[i].Id, 2))
							{
								dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" checked=\"checked\" /></center>";
							}
							else
							{
								dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" /></center>";
								
							}
							
							if (CompareIds(activityTypeList[i].Id, 3))
							{
								dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" checked=\"checked\" /></center>";
							}
							else
							{
								dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" /></center>";
							}
							
						}
						dt.Rows.Add(dr);
					}
					else
					{
						dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\"/></center>";
						dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\"/></center>";
						dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\"/></center>";
						dt.Rows.Add(dr);
					}
				}
				DataView dv = new DataView(dt);
				UserGroupPrefGrid.DataSource = dv;
				UserGroupPrefGrid.DataBind();
			}
			else
			{
				agentDisabled.Visible = true;
			}
		}
		private bool CompareIds(long prefActivityTypeId, int prefAgentId)
		{
			foreach (NotificationPreferenceData tempLoopVar_prefData in preferenceList)
			{
				prefData = tempLoopVar_prefData;
				if (prefData.ActivityTypeId == prefActivityTypeId && prefAgentId == prefData.AgentId)
				{
					return true;
				}
			}
			return false;
		}
		
		private void EditGroupsSettings(long groupId)
		{
            Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
			activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
			activityTypeList = _activityListApi.GetList(activityListCriteria);
			LoadPreferenceList(groupId);
			if (Page.IsPostBack)
			{
				preferenceList.Clear();
				for (int i = 0; i <= activityTypeList.Count - 1; i++)
				{
					
					if ((Page.Request.Form["email" + activityTypeList[i].Id] != null)&& Page.Request.Form["email" + activityTypeList[i].Id] == "on")
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.AgentId = 1;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					else
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
						prefData.AgentId = 1;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					if ((Page.Request.Form["sms" + activityTypeList[i].Id] != null)&& Page.Request.Form["sms" + activityTypeList[i].Id] == "on")
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.AgentId = 3;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					else
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
						prefData.AgentId = 3;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					if ((Page.Request.Form["feed" + activityTypeList[i].Id] != null)&& Page.Request.Form["feed" + activityTypeList[i].Id] == "on")
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.AgentId = 2;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					else
					{
						prefData = new NotificationPreferenceData();
						prefData.ActivityTypeId = activityTypeList[i].Id;
						prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
						prefData.AgentId = 2;
						prefData.UserId = _notificationPreferenceApi.RequestInformation.UserId;
						prefData.ActionSourceId = groupId;
						preferenceList.Add(prefData);
					}
					
				}
				_notificationPreferenceApi.SaveUserPreferences(preferenceList);
			}
		}
		
		private void LoadPreferenceList(long groupId)
		{
            NotificationPreferenceCriteria criteria = new NotificationPreferenceCriteria();
			criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, _notificationPreferenceApi.RequestInformation.UserId);
			criteria.AddFilter(NotificationPreferenceProperty.ActionSource, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
			criteria.AddFilter(NotificationPreferenceProperty.ActionSourceId, CriteriaFilterOperator.EqualTo, groupId);
			preferenceList = _notificationPreferenceApi.GetList(criteria);
		}
		private void RegisterResources()
		{
			//Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
			//Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
			//Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
			//Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
			//Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
			//Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss)
		}
		
		
	}
	
	

