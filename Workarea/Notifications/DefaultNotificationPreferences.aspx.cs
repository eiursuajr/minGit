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
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;

public partial class Workarea_DefaultNotificationPreferences : System.Web.UI.Page
{
    protected StyleHelper _refStyle = new StyleHelper();
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected Ektron.Cms.Framework.Notifications.NotificationPreference _notificationPreferenceApi = new Ektron.Cms.Framework.Notifications.NotificationPreference();
    protected System.Collections.Generic.List<NotificationPreferenceData> preferenceList;
    protected NotificationPreferenceData prefData = new NotificationPreferenceData();
    protected Ektron.Cms.Framework.Notifications.NotificationAgentSetting _notificationAgentApi = new Ektron.Cms.Framework.Notifications.NotificationAgentSetting();
    protected System.Collections.Generic.List<NotificationAgentData> agentList;
    protected Ektron.Cms.Framework.Activity.ActivityType _activityListApi = new Ektron.Cms.Framework.Activity.ActivityType();
    protected System.Collections.Generic.List<Ektron.Cms.Activity.ActivityTypeData> activityTypeList;
    protected System.Collections.Generic.List<NotificationPublishPreferenceData> publishPreferenceList;

    // paging
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;
    string pageMode = "";
    
    protected void Page_Load(object sender, System.EventArgs e)
    {
        
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;

        RegisterResources();
        agentDisabled.Visible = false;
        ektronPageHeader.Visible = true;
        msgHelper = _refContentApi.EkMsgRef;

        this.ParseCurrentPageNumber();

        //Licensing Check
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }

        if (!(_refCommonApi.IsAdmin() || objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityAdmin, _refCommonApi.RequestInformationRef.UserId, false)))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("User not authorized"));
            return;
        }
        if (!String.IsNullOrEmpty(Request.QueryString["mode"]))
        {
            pageMode = Request.QueryString["mode"];
        }

        DisplayData();
    }

    private void DisplayData()
    {
        if ((string)(pageMode) == "colleagues")
        {
            DisplayGrid("colleagues", "DISABLED");
            AddToolBar("viewcolleagues");
        }
        else if ((string)(pageMode) == "groups")
        {
            DisplayGrid("groups", "DISABLED");
            AddToolBar("viewgroups");
        }
        else if ((string)(pageMode) == "editcolleagues")
        {
            EditSettings("colleagues");
            AddToolBar("editcolleagues");
        }
        else if ((string)(pageMode) == "editgroups")
        {
            EditSettings("groups");
            AddToolBar("editgroups");
        }
        else if ((string)(pageMode) == "privacy")
        {
            DisplayPersonalPrefGrid("DISABLED");
            AddToolBar("viewprivacy");
        }
        else if ((string)(pageMode) == "editprivacy")
        {
            EditPersonalSettings();
            AddToolBar("editprivacy");
        }
    }

    private void DisplayGrid(string display, string mode)
    {
        NotificationAgentCriteria criteria = new NotificationAgentCriteria();
        Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
        activityListCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
        criteria.AddFilter(NotificationAgentProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
        
        agentList = _notificationAgentApi.GetList(criteria);
        if ((agentList != null) && agentList.Count > 0)
        {
            if (display == "colleagues")
            {
                activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague);
            }
            else
            {
                activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
            }
            activityTypeList = _activityListApi.GetList(activityListCriteria);
            DefaultPrefGrid.Columns.Clear();
            DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));

            foreach (NotificationAgentData agentData in agentList)
            {
                if (agentData.IsEnabled)
                {
                    if ((agentData.Id) == 1)
                    {
                        DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + msgHelper.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                    else if ((agentData.Id) == 2)
                    {
                        DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + msgHelper.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                    else if ((agentData.Id) == 3)
                    {
                        DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + msgHelper.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
                    }
                }
            }
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("EMPTY", typeof(string)));
            dt.Columns.Add(new DataColumn("EMAIL", typeof(string)));
            dt.Columns.Add(new DataColumn("SMS", typeof(string)));
            dt.Columns.Add(new DataColumn("NEWSFEED", typeof(string)));
            LoadPreferenceList();
            for (int i = 0; i <= activityTypeList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr["EMPTY"] = GetMessageText(activityTypeList[i].Name);                             
                if (preferenceList.Count > 0)
                {
                    foreach (NotificationPreferenceData tempLoopVar_prefData in preferenceList)
                    {
                        prefData = tempLoopVar_prefData;
                        if (CompareIds(activityTypeList[i].Id, 1))
                        {
                            dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\" checked=\"checked\"" + mode + " /></center>";
                        }
                        else
                        {
                            dr["EMAIL"] = "<center><input type=\"Checkbox\" name = \"email" + activityTypeList[i].Id + "\" id=\"email" + activityTypeList[i].Id + "\"" + mode + " /></center>";
                        }
                        if (CompareIds(activityTypeList[i].Id, 2))
                        {
                            dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\" checked=\"checked\" " + mode + " /></center>";
                        }
                        else
                        {
                            dr["NEWSFEED"] = "<center><input type=\"Checkbox\" name=\"feed" + activityTypeList[i].Id + "\" id=\"feed" + activityTypeList[i].Id + "\"" + mode + " /></center>";

                        }

                        if (CompareIds(activityTypeList[i].Id, 3))
                        {
                            dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\" checked=\"checked\"" + mode + " /></center>";
                        }
                        else
                        {
                            dr["SMS"] = "<center><input type=\"Checkbox\" name =\"sms" + activityTypeList[i].Id + "\" id=\"sms" + activityTypeList[i].Id + "\"" + mode + "/></center>";
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

            //get totalpages
            this.TotalPagesNumber = dt.Rows.Count / _refContentApi.RequestInformationRef.PagingSize;
            if (TotalPagesNumber * _refContentApi.RequestInformationRef.PagingSize < dt.Rows.Count)
            {
                TotalPagesNumber++;
            }
            PageSettings();

            DataView dv = new DataView(dt);
            DefaultPrefGrid.DataSource = dv;
            SetGridPagingProperties();
            DefaultPrefGrid.DataBind();
        }
        else
        {
            agentDisabled.Visible = true;
        }
    }
    private string GetMessageText(string st)
    {
        if (st == "Blog Post")
            st = msgHelper.GetMessage("lbl BlogPost");
        else if (st == "Blog Comment")
            st = msgHelper.GetMessage("lbl blog comment");
        else if (st == "Forum Post")
            st = msgHelper.GetMessage("lbl Forum Post");
        else if (st == "Forum Reply")
            st = msgHelper.GetMessage("lbl Forum Reply");
        else if (st == "Add User Workspace Content")
            st = msgHelper.GetMessage("lbl Add User Workspace");
        else if (st == "Edit User Workspace Content")
            st = msgHelper.GetMessage("lbl Edit User Workspace");
        else if (st == "Content Messageboard Post")
            st = msgHelper.GetMessage("lbl Content Messageboard");
        else if (st == "User Messageboard Post")
            st = msgHelper.GetMessage("lbl User Messageboard");
        else if (st == "Micro-message")
            st = msgHelper.GetMessage("lbl Micromessage");
        else if (st == "Add Site Content")
            st = msgHelper.GetMessage("lbl Add Site Content");
        else if (st == "Edit Content")
            st = msgHelper.GetMessage("edit content page title");
        else if (st == "Create Community Group")
            st = msgHelper.GetMessage("lbl CommunityGroup");
        else if (st == "Join Community Group")
            st = msgHelper.GetMessage("lbl Join Community Group");
        else if (st == "Add Colleague")
            st = msgHelper.GetMessage("lbl Add Colleague");
        else if (st == "Add Calendar Event")
            st = msgHelper.GetMessage("add cal event");
        else if (st == "Update Calendar Event")
            st = msgHelper.GetMessage("lbl Update Calendar Event");
        //------Community Groups----------------
        else if (st == "Group Blog Post")
            st = msgHelper.GetMessage("lbl Group Blog Post");
        else if (st == "Group Blog Comment")
            st = msgHelper.GetMessage("lbl Group Blog Comment");
        else if (st == "Group Forum Post")
            st = msgHelper.GetMessage("lbl Group Forum Post");
        else if (st == "Group Forum Reply")
            st = msgHelper.GetMessage("lbl Group Forum Reply");
        else if (st == "Add Group Content")
            st = msgHelper.GetMessage("lbl Add Group Content");
        else if (st == "Edit Group Content")
            st = msgHelper.GetMessage("lbl Edit Group Content");
        else if (st == "Group Messageboard Post")
            st = msgHelper.GetMessage("lbl Group Messageboard Post");
        else if (st == "Add Group Calendar Event")
            st = msgHelper.GetMessage("lbl Add Group Calendar Event");
        else if (st == "Update Group Calendar Event")
            st = msgHelper.GetMessage("lbl Update Group Calendar Event");


        return st;
    }
    private void PageSettings()
    {
        if (TotalPagesNumber <= 1 ||  pageMode.Contains("edit"))
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling((double)TotalPagesNumber)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (_currentPageNumber == 1)
            {
                PreviousPage1.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
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
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    private void ParseCurrentPageNumber()
    {
        int.TryParse(CurrentPage.Text, out _currentPageNumber);
        if (_currentPageNumber <= 0) _currentPageNumber = 1;
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber++;
                break;
            case "Prev":
                _currentPageNumber--;
                break;
        }
        DisplayData();
        // isPostData.Value = "true";

    }

    private void DisplayPersonalPrefGrid(string mode)
    {
        Ektron.Cms.Framework.Settings.Notifications.NotificationPublishPreferenceManager _publishPrefApi =
            new Ektron.Cms.Framework.Settings.Notifications.NotificationPublishPreferenceManager();

        publishPreferenceList = _publishPrefApi.GetDefaultList();

        //get totalpages
        this.TotalPagesNumber = publishPreferenceList.Count / _refContentApi.RequestInformationRef.PagingSize;
        if (TotalPagesNumber * _refContentApi.RequestInformationRef.PagingSize < publishPreferenceList.Count)
        {
            TotalPagesNumber++;
        }
        PageSettings();

        if ((publishPreferenceList != null) && publishPreferenceList.Count > 0)
        {
            DefaultPrefGrid.Columns.Clear();
            DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("NAME", msgHelper.GetMessage("generic actions"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            DefaultPrefGrid.Columns.Add(_refStyle.CreateBoundField("PUBLISH", "<center>" + msgHelper.GetMessage("generic publish") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), false, false));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("NAME", typeof(string)));
            dt.Columns.Add(new DataColumn("PUBLISH", typeof(string)));
            for (int i = 0; i <= publishPreferenceList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr["NAME"] = GetMessageText(publishPreferenceList[i].ActivityTypeName);  
                if (publishPreferenceList[i].IsEnabled)
                {
                    dr["PUBLISH"] = "<center><input type=\"Checkbox\" name = \"activitytype" + publishPreferenceList[i].ActivityTypeId + "\" id=\"activitytype" + publishPreferenceList[i].ActivityTypeId + "\" checked=\"checked\"" + mode + " /></center>";
                }
                else
                {
                    dr["PUBLISH"] = "<center><input type=\"Checkbox\" name = \"activitytype" + publishPreferenceList[i].ActivityTypeId + "\" id=\"activitytype" + publishPreferenceList[i].ActivityTypeId + "\"" + mode + " /></center>";
                }
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            this.DefaultPrefGrid.DataSource = dv;

            SetGridPagingProperties();

            DefaultPrefGrid.DataBind();
        }
    }

    private void SetGridPagingProperties()
    {
        this.DefaultPrefGrid.PageSize = _refContentApi.RequestInformationRef.PagingSize;
        this.DefaultPrefGrid.PageIndex = _currentPageNumber - 1;
        this.DefaultPrefGrid.AllowPaging = this.TotalPages.Visible; // allow paging if paging labels are shown
        this.DefaultPrefGrid.PagerSettings.Visible = false;
    }
    private void EditSettings(string setting)
    {
        Ektron.Cms.Activity.ActivityTypeCriteria activityListCriteria = new Ektron.Cms.Activity.ActivityTypeCriteria();

        if (setting == "colleagues")
        {
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague);
        }
        else
        {
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup);
        }
        activityTypeList = _activityListApi.GetList(activityListCriteria);
        LoadPreferenceList();
        if (Page.IsPostBack)
        {
            preferenceList.Clear();
            for (int i = 0; i <= activityTypeList.Count - 1; i++)
            {

                if ((Page.Request.Form["email" + activityTypeList[i].Id] != null) && Page.Request.Form["email" + activityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.AgentId = 1;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 1;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["sms" + activityTypeList[i].Id] != null) && Page.Request.Form["sms" + activityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.AgentId = 3;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 3;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
                if ((Page.Request.Form["feed" + activityTypeList[i].Id] != null) && Page.Request.Form["feed" + activityTypeList[i].Id] == "on")
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.AgentId = 2;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
                else
                {
                    prefData = new NotificationPreferenceData();
                    prefData.ActivityTypeId = activityTypeList[i].Id;
                    prefData.DataState = Ektron.Cms.Common.EkEnumeration.DataState.Deleted;
                    prefData.AgentId = 2;
                    prefData.UserId = -1;
                    preferenceList.Add(prefData);
                }
            }
            _notificationPreferenceApi.SaveUserPreferences(preferenceList);
            if (setting == "colleagues")
            {
                Response.Redirect("DefaultNotificationPreferences.aspx?mode=colleagues");
            }
            else
            {
                Response.Redirect("DefaultNotificationPreferences.aspx?mode=groups");
            }
        }
        else
        {
            DisplayGrid(setting, "");
        }
    }
    private void EditPersonalSettings()
    {
        if (Page.IsPostBack)
        {
            Ektron.Cms.Framework.Notifications.NotificationPublishPreference _publishPrefApi = new Ektron.Cms.Framework.Notifications.NotificationPublishPreference();
            System.Collections.Generic.List<long> activityIds = new System.Collections.Generic.List<long>();
            publishPreferenceList = _publishPrefApi.GetList(-1);
            for (int i = 0; i <= publishPreferenceList.Count - 1; i++)
            {
                if ((Page.Request.Form["activitytype" + publishPreferenceList[i].ActivityTypeId] != null) && Page.Request.Form["activitytype" + publishPreferenceList[i].ActivityTypeId] == "on")
                {
                    activityIds.Add(publishPreferenceList[i].ActivityTypeId);
                }
            }
            try
            {
                _publishPrefApi.UpdateDefaultPreferences(activityIds);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
            }
            Response.Redirect("DefaultNotificationPreferences.aspx?mode=privacy");
        }
        else
        {
            DisplayPersonalPrefGrid("");
        }
    }
    private bool CompareIds(long prefActivityTypeId, long prefAgentId)
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
    private void LoadPreferenceList()
    {
        NotificationPreferenceCriteria criteria = new NotificationPreferenceCriteria();
        criteria.PagingInfo.RecordsPerPage = 1000;
        criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, -1);
        preferenceList = _notificationPreferenceApi.GetDefaultPreferenceList(criteria);
    }
    private void AddToolBar(string mode)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");
        if (mode == "editcolleagues")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl friends"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "DefaultNotificationPreferences.aspx?mode=colleagues", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "#", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn save"), "onclick=\"return SubmitForm(\'notificationPreferences\');\"", StyleHelper.SaveButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("EditColleaguesSettings", "") + "</td>");
        }
        else if (mode == "viewcolleagues")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl friends"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "DefaultNotificationPreferences.aspx?mode=editcolleagues", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewColleaguesSettings", "") + "</td>");
        }
        else if (mode == "viewgroups")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl groups"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "DefaultNotificationPreferences.aspx?mode=editgroups", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewGroupsSettings", "") + "</td>");
        }
        else if (mode == "editgroups")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl groups"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "DefaultNotificationPreferences.aspx?mode=groups", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "DefaultNotificationPreferences.aspx?mode=groups", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn save"), "onclick=\"javascript: return SubmitForm(\'notificationPreferences\');\"", StyleHelper.SaveButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("EditGroupSettings", "") + "</td>");
        }
        else if (mode == "viewprivacy")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl privacy"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/contentEdit.png", "DefaultNotificationPreferences.aspx?mode=editprivacy", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("ViewPrivacySettings", "") + "</td>");
        }
        else if (mode == "editprivacy")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl privacy"));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/back.png", "DefaultNotificationPreferences.aspx?mode=privacy", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppImgPath + "../UI/Icons/save.png", "DefaultNotificationPreferences.aspx?mode=privacy", msgHelper.GetMessage("alt edit settings button text"), msgHelper.GetMessage("btn save"), "onclick=\"javascript: return SubmitForm(\'notificationPreferences\');\"", StyleHelper.SaveButtonCssClass, true));
            result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _refStyle.GetHelpButton("EditPrivacySettings", "") + "</td>");
        }
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}