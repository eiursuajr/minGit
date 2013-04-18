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
using Ektron.Facebook.Rest;

public partial class Workarea_Notifications_Agents : System.Web.UI.Page
{
    protected EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected ContentAPI _refContentApi = new ContentAPI();
    protected StyleHelper _refStyle = new StyleHelper();
    protected Ektron.Cms.Framework.Notifications.NotificationAgentSetting _notificationAgentApi = new Ektron.Cms.Framework.Notifications.NotificationAgentSetting();
    protected System.Collections.Generic.List<NotificationAgentData> agentList;
    protected System.Collections.Generic.List<NotificationAgent> defaultAgentList;
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        string pageMode = "";
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = _refContentApi.EkContentRef;
        RegisterResources();
        msgHelper = _refCommonApi.EkMsgRef;
        delAgentMsg.Text = msgHelper.GetMessage("js: delete agent");

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
            DeleteAgent();
        }


    }

    private void ParseCurrentPageNumber()
    {
        int.TryParse(CurrentPage.Text, out _currentPageNumber);
        if (_currentPageNumber <= 0) _currentPageNumber = 1;
    }

    private void DisplayAdd()
    {
        NotificationAgentData data = new NotificationAgentData();
        AddAgent.Visible = true;
        ViewAgents.Visible = false;
        AddToolbar("add", 0);
        if (Page.IsPostBack)
        {
            data.Name = (string)ddlagent.SelectedItem.Text;
            data.TypeName = (string)ddlagent.SelectedValue;
            data.IsEnabled = System.Convert.ToBoolean(chkEnable.Checked);
            try
            {
                _notificationAgentApi.Add(data);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                return;
            }
            Response.Redirect("agents.aspx?mode=viewgrid");
        }
        else
        {
            GetAgentList("add");
        }
    }

    private void DisplayView()
    {
        NotificationAgentData data = null;
        data = GetAgentData();
        AddToolbar("view", data.Id);
        ViewAgents.Visible = false;
        GetAgentList("view");
        ddlagent.Enabled = false;
        chkEnable.Enabled = false;
        ddlagent.SelectedValue = data.Id.ToString();
        chkEnable.Checked = data.IsEnabled;
    }
    private void DisplayEdit()
    {
        NotificationAgentData data = null;
        data = GetAgentData();
        AddToolbar("edit", data.Id);
        ViewAgents.Visible = false;
        GetAgentList("edit");
        ddlagent.Enabled = false;
        if (Page.IsPostBack)
        {
            data.Id = Convert.ToInt64(ddlagent.SelectedValue);
            data.IsEnabled = System.Convert.ToBoolean(chkEnable.Checked);
            try
            {
                _notificationAgentApi.Update(data);
            }
            catch (Exception ex)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
                return;
            }
            Response.Redirect((string)("agents.aspx?mode=view&id=" + data.Id.ToString()));
        }
        else
        {
            ddlagent.SelectedValue = data.Id.ToString();
            chkEnable.Checked = data.IsEnabled;
        }
    }
    private void DeleteAgent()
    {
        NotificationAgentData data = null;
        data = GetAgentData();

        try
        {
            _notificationAgentApi.Delete(data.Id);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _refContentApi.GetCookieValue("LastValidLanguageID")), false);
            return;
        }
        Response.Redirect("agents.aspx?mode=viewgrid");
    }
    private void DisplayGrid()
    {
        AddAgent.Visible = false;
        ViewAgents.Visible = true;
        AddToolbar("viewgrid", 0);
        GetAgentList("viewgrid");
        
        //get totalpages
        this.TotalPagesNumber = agentList.Count / _refContentApi.RequestInformationRef.PagingSize;
        if (TotalPagesNumber * _refContentApi.RequestInformationRef.PagingSize < agentList.Count)
        {
            TotalPagesNumber++;
        }
        PageSettings();

        if ((agentList != null) && agentList.Count > 0)
        {
            ViewAgentGrid.Columns.Clear();
            ViewAgentGrid.Columns.Add(_refStyle.CreateBoundField("NAME", "" + msgHelper.GetMessage("generic name") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), false, false));
            ViewAgentGrid.Columns.Add(_refStyle.CreateBoundField("ENABLED", "" + msgHelper.GetMessage("enabled") + "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("NAME", typeof(string)));
            dt.Columns.Add(new DataColumn("ENABLED", typeof(string)));

            for (int i = 0; i <= agentList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr["NAME"] = "<a href=\"agents.aspx?mode=view&id=" + agentList[i].Id + "\">" + GetResourceText(agentList[i].Name) + "</a>";
                if (agentList[i].IsEnabled)
                {
                    dr["ENABLED"] = "<img src=\"" + _refCommonApi.AppPath + "images/UI/Icons/check.png\" alt=\"Enabled\"/>";
                }
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);

            this.ViewAgentGrid.PageSize = _refContentApi.RequestInformationRef.PagingSize;
            this.ViewAgentGrid.PageIndex = _currentPageNumber - 1;
            this.ViewAgentGrid.AllowPaging = this.TotalPages.Visible; // allow paging if paging labels are shown
            this.ViewAgentGrid.PagerSettings.Visible = false;
            
            ViewAgentGrid.DataSource = dv;
            ViewAgentGrid.DataBind();
        }

    }
    private string GetResourceText(string st)
    {
        if (st == "SMSAgent")
            st = msgHelper.GetMessage("generic SMSAgent");
        else if (st == "ActivityStream")
            st = msgHelper.GetMessage("generic ActivityStream");
        else if (st == "EktronEmail")
            st = msgHelper.GetMessage("generic EktronEmail");

        return st;
    }
    private void GetAgentList(string mode)
    {
        Criteria<NotificationAgentProperty> criteria = new Criteria<NotificationAgentProperty>();
        
        if (mode == "add")
        {
            defaultAgentList = _notificationAgentApi.GetRegisteredAgentList();
            foreach (NotificationAgent agentItem in defaultAgentList)
            {
                ddlagent.Items.Add(new ListItem(agentItem.Name, agentItem.GetType().ToString()));
            }
        }
        else
        {
            agentList = _notificationAgentApi.GetList(criteria);
            foreach (NotificationAgentData agentData in agentList)
            {
                ddlagent.Items.Add(new ListItem(agentData.Name, agentData.Id.ToString()));
            }
        }


    }
    private NotificationAgentData GetAgentData()
    {
        long id = 0;
        NotificationAgentData data = null;

        long.TryParse(Request.QueryString["id"], out id);
        if (id == 0)
        {
            throw (new ArgumentException("Agent ID does not exists."));
        }
        data = _notificationAgentApi.GetItem(id);
        if (data == null)
        {
            throw (new NullReferenceException("Message not found"));
        }
        return data;
    }
    private void AddToolbar(string mode, long id)
    {

        if (mode == "view")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view agent"));
        }
        else if (mode == "edit")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl edit agent"));
        }
        else if (mode == "viewgrid")
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl view agent grid"));
        }
        else
        {
            divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl add agent"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\r\n");

		if (mode == "edit")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "agents.aspx?mode=view&id=" + id.ToString() + "", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "view")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "agents.aspx?mode=viewgrid", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else if (mode == "add")
		{
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/back.png", "agents.aspx?mode=viewgrid", msgHelper.GetMessage("alt back button text"), msgHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (mode == "view")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/contentEdit.png", "agents.aspx?mode=edit&id=" + id.ToString() + "", msgHelper.GetMessage("alt edit agent"), msgHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            if (id > 1000)
            {
				result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/delete.png", "agents.aspx?mode=delete&id=" + id.ToString() + "", msgHelper.GetMessage("alt delete agent"), msgHelper.GetMessage("btn delete"), "Onclick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
            }
        }
        else if (mode == "viewgrid")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/add.png", "agents.aspx?mode=add", msgHelper.GetMessage("alt add agent"), msgHelper.GetMessage("alt add agent"), "", StyleHelper.AddButtonCssClass, true));

        }
        else if (mode == "edit")
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'agent\');\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"javascript: SubmitForm(\'agent\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        if (mode == "edit")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("EditAgent", "") + "</td>");
        }
        else if (mode == "viewgrid")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("viewallagents", "") + "</td>");
        }
        else if (mode == "view")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewAgent", "") + "</td>");
        }
        else if (mode == "add")
        {
            result.Append("<td>" + _refStyle.GetHelpButton("addagents", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + _refStyle.GetHelpButton("ViewAllNotificationMsg", "") + "</td>");
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
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
    }
}