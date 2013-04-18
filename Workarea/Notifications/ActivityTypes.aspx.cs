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
using Ektron.Cms.Activity;

public partial class Workarea_Notifications_ActivityTypes : System.Web.UI.Page
{

    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected StyleHelper _refStyle = new StyleHelper();
    protected Ektron.Cms.Framework.Activity.ActivityType _activityTypeApi = new Ektron.Cms.Framework.Activity.ActivityType();
    protected System.Collections.Generic.List<ActivityTypeData> activityTypeList;
    // paging
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;
    protected void Page_Load(object sender, System.EventArgs e)
    {
        string pageMode = "";
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        msgHelper = _refCommonApi.EkMsgRef;
        SetServerJSVariables();

        this.ParseCurrentPageNumber();

        //Licensing Check
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, false))
        {
            Utilities.ShowError(_refContentApi.EkMsgRef.GetMessage("feature locked error"));
            return;
        }
        if (!Utilities.ValidateUserLogin())
        {
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
        if ((string)(pageMode) == "viewgrid")
        {
            DisplayGrid();
        }
        else if ((string)(pageMode) == "add")
        {
            DisplayAdd();
        }
        else if ((string)(pageMode) == "edit")
        {
            DisplayEdit();
        }
        else if ((string)(pageMode) == "view")
        {
            DisplayView();
        }
        else if ((string)(pageMode) == "delete")
        {
            DeleteActivityType();
        }
    }
    private void DisplayAdd()
    {
        ActivityTypeData data = new ActivityTypeData();
        AddToolbar("add", 0);
        dvAddNewActivityType.Visible = true;
        dvActivityGrid.Visible = false;
        if (Page.IsPostBack)
        {
            data.Name = EkFunctions.HtmlEncode((string)txtName.Text);
            data.ObjectType = Ektron.Cms.Common.EkEnumeration.ActivityObjectType.Custom; //System.Enum.Parse(GetType(EkEnumeration.ActivityObjectType), ddlType.SelectedValue)
            data.ActionType = Ektron.Cms.Common.EkEnumeration.ActivityActionType.All; //ddlActionType.SelectedIndex
            data.Scope = (EkEnumeration.ActivityScope)Enum.ToObject(typeof(EkEnumeration.ActivityScope), (int)ddlActionScope.SelectedIndex);
            try
            {
                _activityTypeApi.Add(data);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                return;
            }
            Response.Redirect("activitytypes.aspx?mode=viewgrid");
        }
        else
        {
            BindEntryValues();
            rowObjecttype.Visible = false;
            rowActiontype.Visible = false;
            ddlType.Visible = false;
            ddlActionType.Visible = false;
        }
    }
    private void DisplayView()
    {
        ActivityTypeData data;
        data = GetActivityData();
        AddToolbar("view", data.Id);
        BindEntryValues();
        dvAddNewActivityType.Visible = true;
        dvActivityGrid.Visible = false;
        txtName.Enabled = false;
        ddlType.Enabled = false;
        ddlActionType.Enabled = false;
        ddlActionScope.Enabled = false;
        txtName.Text = EkFunctions.HtmlDecode(data.Name);
        ddlType.SelectedValue = Enum.GetName(typeof(EkEnumeration.ActivityObjectType), data.ObjectType);
        ddlActionType.SelectedIndex = (int)data.ActionType;
        ddlActionScope.SelectedIndex = (int)data.Scope;
    }
    private void DisplayEdit()
    {
        ActivityTypeData data;
        data = GetActivityData();
        AddToolbar("edit", data.Id);
        dvAddNewActivityType.Visible = true;
        dvActivityGrid.Visible = false;
        if (Page.IsPostBack)
        {
            data.Name = EkFunctions.HtmlEncode((string)txtName.Text);
            data.ObjectType = (EkEnumeration.ActivityObjectType)Enum.Parse(typeof(EkEnumeration.ActivityObjectType), (string)ddlType.SelectedValue);
            data.ActionType = (EkEnumeration.ActivityActionType)Enum.ToObject(typeof(EkEnumeration.ActivityActionType), ddlActionType.SelectedIndex);
            data.Scope = (EkEnumeration.ActivityScope)Enum.ToObject(typeof(EkEnumeration.ActivityScope), ddlActionScope.SelectedIndex);
            try
            {
                _activityTypeApi.Update(data);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                return;
            }
            Response.Redirect((string)("activitytypes.aspx?mode=view&id=" + Request.QueryString["id"]));

        }
        else
        {
            BindEntryValues();
            txtName.Text = EkFunctions.HtmlDecode(data.Name);
            ddlType.SelectedValue = Enum.GetName(typeof(EkEnumeration.ActivityObjectType), data.ObjectType);
            ddlActionType.SelectedIndex = (int)data.ActionType;
            ddlActionScope.SelectedIndex = (int)data.Scope;
            ddlType.Enabled = false;
            ddlActionType.Enabled = false;
            if (data.Id < 1000)
            {
                txtName.Enabled = false;
                ddlActionScope.Enabled = false;
            }
        }
    }
    private void DeleteActivityType()
    {
        ActivityTypeData data;
        data = GetActivityData();
        dvAddNewActivityType.Visible = true;
        dvActivityGrid.Visible = false;
        try
        {
            _activityTypeApi.Delete(data.Id);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
            return;
        }
        Response.Redirect("activitytypes.aspx?mode=viewgrid");
    }
    private string GetResourceText(string st)
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
        //---------------CommunityGroups--------------------------------------
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
        //----------------------ActionType---------------------------------------------------
        if (st == "Edit")
            st = msgHelper.GetMessage("btn edit");
        else if (st == "Replies")
            st = msgHelper.GetMessage("lbl replies");
        else if (st == "Add")
            st = msgHelper.GetMessage("btn add");
        //------------------ObjectType--------------------------------------------------        
        if (st == "User")
            st = msgHelper.GetMessage("generic user");
        else if (st == "CommunityGroup")
            st = msgHelper.GetMessage("generic CommunityGroup"); //content button text
        else if (st == "Content")
            st = msgHelper.GetMessage("content button text");
        else if (st == "MicroMessage")
            st = msgHelper.GetMessage("lbl Micromessage");
        else if (st == "GroupMessageBoard")
            st = msgHelper.GetMessage("generic GroupMessageBoard");
        else if (st == "UserMessageBoard")
            st = msgHelper.GetMessage("generic UserMessageBoard");
        else if (st == "ContentMessageBoard")
            st = msgHelper.GetMessage("generic ContentMessageBoard");
        else if (st == "DiscussionTopic")
            st = msgHelper.GetMessage("generic DiscussionTopic");
        else if (st == "BlogPost")
            st = msgHelper.GetMessage("generic BlogPost");     
       
        return st;
    }
    private void DisplayGrid()
    {
        PagingInfo page;
        dvAddNewActivityType.Visible = false;
        dvActivityGrid.Visible = true;
        AddToolbar("viewgrid", 0);

        Ektron.Cms.Activity.ActivityTypeCriteria criteria = new Ektron.Cms.Activity.ActivityTypeCriteria();
        page = new PagingInfo();
        page.CurrentPage = _currentPageNumber;
        criteria.PagingInfo = page;
        criteria.PagingInfo.RecordsPerPage = _refContentApi.RequestInformationRef.PagingSize;
        activityTypeList = _activityTypeApi.GetList(criteria);

        TotalPagesNumber = page.TotalPages;

        // old code
        PageSettings();
        if ((activityTypeList != null) && activityTypeList.Count > 0)
        {
            ActivityGrid.Columns.Clear();
            ActivityGrid.Columns.Add(_refStyle.CreateBoundField("NAME", msgHelper.GetMessage("generic name"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            ActivityGrid.Columns.Add(_refStyle.CreateBoundField("ID", msgHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            ActivityGrid.Columns.Add(_refStyle.CreateBoundField("ACTIONTYPE", msgHelper.GetMessage("lbl action type"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(9), Unit.Percentage(6), false, false));
            ActivityGrid.Columns.Add(_refStyle.CreateBoundField("OBJECTTYPE", msgHelper.GetMessage("lbl object type"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(11), Unit.Percentage(8), false, false));
            ActivityGrid.Columns.Add(_refStyle.CreateBoundField("SCOPE", msgHelper.GetMessage("lbl action scope"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(11), Unit.Percentage(11), false, false));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("NAME", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("ACTIONTYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("OBJECTTYPE", typeof(string)));
            dt.Columns.Add(new DataColumn("SCOPE", typeof(string)));


            for (int i = 0; i <= activityTypeList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr["NAME"] = "<a href=\"activitytypes.aspx?mode=view&id=" + activityTypeList[i].Id + "\">" +  GetResourceText(activityTypeList[i].Name) + "</a>";
                dr["ID"] = activityTypeList[i].Id.ToString();  
                dr["ACTIONTYPE"] = GetResourceText(activityTypeList[i].ActionType.ToString());
                dr["OBJECTTYPE"] = GetResourceText(activityTypeList[i].ObjectType.ToString());
                dr["SCOPE"] = GetResourceText(activityTypeList[i].Scope.ToString());
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            ActivityGrid.DataSource = dv;
            ActivityGrid.DataBind();
        }

    }

    private ActivityTypeData GetActivityData()
    {
        long id = 0;
        ActivityTypeData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("Activity ID does not exists."));
        }
        data = _activityTypeApi.GetItem(id);
        if (data == null)
        {
            throw (new NullReferenceException("Activity not found"));
        }
        return data;
    }
    private void BindEntryValues()
    {
        ddlType.DataSource = Enum.GetNames(typeof(EkEnumeration.ActivityObjectType));
        ddlType.DataBind();

        ddlActionType.DataSource = Enum.GetNames(typeof(EkEnumeration.ActivityActionType));
        ddlActionType.DataBind();

        ddlActionScope.DataSource = Enum.GetNames(typeof(EkEnumeration.ActivityScope));
        ddlActionScope.DataBind();
    }
    private void AddToolbar(string mode, long id)
    {

        if (mode == "view")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view activity type"));
        }
        else if (mode == "edit")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl edit activity type"));
        }
        else if (mode == "viewgrid")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view activity type grid"));
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl add activity type"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");

		if (mode == "edit")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "activitytypes.aspx?mode=view&id=" + id.ToString() + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "activitytypes.aspx?mode=viewgrid", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "add")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "activitytypes.aspx?mode=viewgrid", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "view")
        {
            if (id > 1000)
            {
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/contentEdit.png", "activitytypes.aspx?mode=edit&id=" + id.ToString() + "", msgHelper.GetMessage("alt edit activity type"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/delete.png", "activitytypes.aspx?mode=delete&id=" + id.ToString() + "", msgHelper.GetMessage("alt delete activity type"), msgHelper.GetMessage("btn delete"), "Onclick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
            }
        }
        else if (mode == "viewgrid")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/add.png", "activitytypes.aspx?mode=add", msgHelper.GetMessage("alt add activity type"), msgHelper.GetMessage("alt add activity type"), "", StyleHelper.AddButtonCssClass, true));

        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'ActivityTypes\',\'VerifyAddActivityType()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'ActivityTypes\',\'VerifyAddActivityType()\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        if (mode == "edit")
        {
			result.Append("<td>" + _refStyle.GetHelpButton("EditActivityTypeMsg", "") + "</td>");
        }
        else if (mode == "view")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewActivityTypeMsg", "") + "</td>");
        }
        else if (mode == "add")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("AddActivityTypeMsg", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewAllActivityTypes", "") + "</td>");
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    private void PageSettings()
    {
        if (TotalPagesNumber <= 1)
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
        DisplayGrid();
        isPostData.Value = "true";

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
    private void SetServerJSVariables()
    {
        ltr_follErr.Text = msgHelper.GetMessage("alert msg foll fields");
        ltr_nameErr.Text = msgHelper.GetMessage("js: alert name required");
        ltr_scopeErr.Text = msgHelper.GetMessage("js: alert scope required");
        delActivityTypeMsg.Text = msgHelper.GetMessage("js: delete activity type");
    }
}