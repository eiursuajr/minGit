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

public partial class editgroups : System.Web.UI.UserControl
{
    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected PermissionData security_data;
    protected LanguageData[] language_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected SettingsData setting_data;
    protected DomainData[] domain_list;
    protected string search = "";
    protected string m_strUserName = "";
    protected int ContentLanguage = -1;
    protected string m_strFirstName = "";
    protected string m_strLastName = "";
    protected string m_strDomain = "";
    protected long uId = -1;
    protected UserData[] user_list;
    protected Collection pagedata;
    protected string m_strFilter = "";
    protected GroupData[] group_list;
    protected UserGroupData group_data;
    protected string OrderBy = "";
    protected string PageAction = "";
    protected int m_intGroupType = 0;

    protected string m_strDirection = "asc";
    protected string m_strSearchText = "";
    protected string m_strKeyWords = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strSelectedItem = "-1";
    protected string m_strPageAction = "";
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        if (!(Request.QueryString["grouptype"] == null) && Request.QueryString["grouptype"] != "")
        {
            m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
        }
        if ((!(Request.QueryString["action"] == null)) && (Request.QueryString["action"] != ""))
        {
            m_strPageAction = Request.QueryString["action"].ToLower();
        }
        if ((!(Request.QueryString["id"] == null)) && (Request.QueryString["id"] != ""))
        {
            uId = Convert.ToInt64(Request.QueryString["id"]);
        }

        m_refMsg = m_refContentApi.EkMsgRef;
        AppImgPath = m_refSiteApi.AppImgPath;
        ContentLanguage = m_refSiteApi.ContentLanguage;
        m_strDirection = Request.QueryString["direction"];
        if (m_strDirection == "asc")
        {
            m_strDirection = "desc";
        }
        else
        {
            m_strDirection = "asc";
        }
        VisiblePageControls(false);
        if (m_strPageAction == "addusertogroup")
        {
            AddUserToGroup();
        }
        RegisterResources();
    }

    private bool LDAPMembers()
    {
        if (m_intGroupType == 1) //member
        {
            return (m_refUserApi.RequestInformationRef.LDAPMembershipUser);
        }
        else if (m_intGroupType == 0) //CMS user
        {
            return true;
        }
        return false;
    }

    public bool AddUserToGroup()
    {
        if (Page.IsPostBack && Request.Form[isPostData.UniqueID] != "")
        {
            if (Request.Form[isSearchPostData.UniqueID] != "")
            {
                isSearchPostData.Value = "";
                CollectSearchText();
                DisplayUsers();
            }
            else
            {
                if (Request.Form[isAdded.UniqueID] != "")
                {
                    m_refUserApi.AddUserToGroup(Request.Form["selected_users"].ToString(), uId);
                }
                Response.Redirect((string)("users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&groupid=" + uId + "&id=" + uId + "&OrderBy=" + Request.QueryString["OrderBy"]), false);
                //Response.Redirect("users.aspx?action=AddUserToGroup&grouptype=" & m_intGroupType & "&id=" & Request.QueryString("id") & "&OrderBy=" & Request.QueryString("OrderBy"), False)
            }
        }
        else if (IsPostBack == false)
        {
            DisplayUsers();
        }
        isPostData.Value = "true";
        return true;
    }
    public void DisplayUsers()
    {
        TR_AddGroupDetail.Visible = false;
        if (Request.QueryString["OrderBy"] == "")
        {
            OrderBy = "UserName";
        }
        else
        {
            OrderBy = Request.QueryString["OrderBy"];
        }
        GroupRequest req = new GroupRequest();
        req.GroupType = m_intGroupType;
        req.GroupId = uId;
        req.SortOrder = OrderBy;
        req.SortDirection = m_strDirection;
        req.SearchText = m_strSearchText;
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        req.CurrentPage = m_intCurrentPage;
        user_list = m_refUserApi.GetUsersNotInGroup(req);
        m_intTotalPages = req.TotalPages;
        group_data = m_refUserApi.GetUserGroupById(uId);
        AddUserToGroupToolBar();
        Populate_AddUserToGroupGrid();
    }

    private void Process_AddADUserGroup()
    {
        System.Collections.Specialized.NameValueCollection sdGroups = new System.Collections.Specialized.NameValueCollection();
        int lcount;
        string strGrouppath = "";
        for (lcount = 1; lcount <= System.Convert.ToInt32(Request.Form[addgroupcount.UniqueID]); lcount++)
        {
            strGrouppath = "";
            strGrouppath = !string.IsNullOrEmpty(Request.Form["addgroup" + lcount.ToString()]) ? Request.Form["addgroup" + lcount.ToString()] : "";
            if (strGrouppath != "")
            {
                sdGroups.Add(lcount.ToString(), strGrouppath);
            }
        }
        if (m_intGroupType == 0)
        {
            m_refUserApi.AddADGroupToCMS(sdGroups);
        }
        else
        {
            bool ret = false;
            Ektron.Cms.User.EkUser usr;
            usr = m_refUserApi.EkUserRef;
            ret = usr.AddADMemberShipGroupToCmsV4(sdGroups);
        }
        Response.Redirect((string)("users.aspx?action=viewallgroups&grouptype=" + m_intGroupType), false);
    }

    public void EditUserGroup()
    {
        TR_label.Visible = false;
        if (!(Page.IsPostBack))
        {
            TR_desc.Visible = false;
            Display_EditUserGroup();
        }
        else
        {
            Process_EditUserGroup();
        }
    }
    public void AddUserGroup()
    {
        TD_label.Visible = false;
        PageAction = "addusergroup";
        search = Request.QueryString["search"];
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);
        if ((!(Page.IsPostBack)) || (Page.IsPostBack && ((search == "1") || (search == "2")) && setting_data.ADIntegration == true && Request.Form["domainname"] != null))
        {
            if ((!(LDAPMembers())) || (setting_data.ADIntegration == false))
            {
                TR_AddGroup.Visible = false;
                Display_AddUserGroup();
            }
            else
            {
                domain_list = m_refUserApi.GetDomains(0, 0);
                TR_AddGroupDetail.Visible = false;
                if ((search == "1") || (search == "2"))
                {
                    Display_AddUserGroup_Search();
                }
                else
                {
                    Display_AddUserGroup_None();
                }
            }
        }
        else
        {
            if ((setting_data.ADIntegration) && (search == "1" || search == "2"))
            {
                Process_AddADUserGroup();
            }
            else
            {
                Process_AddUserGroup();
            }
        }
    }
    private void Display_EditUserGroup()
    {
        uId = Convert.ToInt64(Request.QueryString["GroupID"]);
        group_data = m_refUserApi.GetUserGroupById(uId);
        EditUserGroupToolBar();
        UserGroupName.Value = Server.HtmlDecode(group_data.GroupName);
    }
    private void EditUserGroupToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit user group msg"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "users.aspx?action=viewallgroups&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&groupid=" + uId + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (user group)"), m_refMsg.GetMessage("btn update"), "onclick=\"return SubmitForm(\'UserGroupInfo\', \'VerifyGroupName()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("EditUserGroupToolBar", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void Process_EditUserGroup()
    {
        pagedata = new Collection();
        pagedata.Add(Request.Form[UserGroupName.UniqueID], "UserGroupName", null, null);
        pagedata.Add(Request.QueryString["groupid"], "UserGroupID", null, null);
        if (m_intGroupType == 1)
        {
            bool ret = false;
            Ektron.Cms.User.EkUser objUser;
            objUser = m_refUserApi.EkUserRef;
            ret = objUser.UpDateUserGroupv2_0(pagedata);
        }
        else
        {
            m_refUserApi.UpDateUserGroup(pagedata);
        }
        Response.Redirect((string)("users.aspx?action=viewallgroups&grouptype=" + m_intGroupType + "&group8id=" + Request.QueryString["groupid"]), false);
    }
    private void AddUserToGroupToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("add user to group msg") + " \"" + group_data.GroupDisplayName + "\""));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", "click here to add selected users to group", m_refMsg.GetMessage("btn save"), "onclick=\"AddSelectedUsers();\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("AddMembershipUserToGroup", ""));
        result.Append("</td>");
        result.Append("<td>&nbsp;|&nbsp;</td>");
        result.Append("<td>");
        result.Append("<label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label>");
        result.Append("<input type=text class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<select id=\"searchlist\" name=searchlist>");
        result.Append("<option value=-1" + IsSelected("-1") + ">All</option>");
        result.Append("<option value=\"last_name\"" + IsSelected("last_name") + ">Last Name</option>");
        result.Append("<option value=\"first_name\"" + IsSelected("first_name") + ">First Name</option>");
        result.Append("<option value=\"user_name\"" + IsSelected("user_name") + ">User Name</option>");
        result.Append("</select><input type=button value=\"Search\" class=\"ektronWorkareaSearch\" id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();\" title=\"Search Users\">");
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void Populate_AddUserToGroupGrid()
    {
        string HeaderText = "<a href=\"users.aspx?action=AddUserToGroup&OrderBy={0}&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&id=" + uId + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">{1}</a>";
        string Icon = "user.png";
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        if (m_intGroupType == 1)
        {
            Icon = "userMembership.png";
        }
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECK";
        colBound.HeaderText = "<input type=checkbox name=checkall id=checkall onclick=\"checkAll(\'\');\">";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        AddGroupGrid.Columns.Add(colBound);
        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = string.Format(HeaderText, "user_name", m_refMsg.GetMessage("generic username"));
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = string.Format(HeaderText, "last_name", m_refMsg.GetMessage("generic lastname"));
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = string.Format(HeaderText, "first_name", m_refMsg.GetMessage("generic firstname"));
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        colBound.HeaderText = m_refMsg.GetMessage("generic Language");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        AddGroupGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        PageSettings();
        int i;
        if (!(user_list == null))
        {
            for (i = 0; i <= user_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_users\" id=\"selected_users\" value=\"" + user_list[i].Id + "\" onclick=\"checkAll(\'selected_users\');\">";
                dr["USERNAME"] = "<img src=\"" + AppImgPath + "../UI/Icons/" + Icon + "\" border=\"0\" align=\"absbottom\">" + user_list[i].Username + "</img>";
                dr["LASTNAME"] = user_list[i].LastName;
                dr["FIRSTNAME"] = user_list[i].FirstName;
                if (user_list[i].LanguageId == 0)
                {
                    dr["LANGUAGE"] = m_refMsg.GetMessage("app default msg");
                }
                else
                {
                    dr["LANGUAGE"] = user_list[i].LanguageName;
                }
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        AddGroupGrid.DataSource = dv;
        AddGroupGrid.DataBind();

    }
    private void Process_AddUserGroup()
    {
        if (m_intGroupType == 0)
        {
            m_refUserApi.AddUserGroup(Request.Form[UserGroupName.UniqueID], "", "");
        }
        else
        {
            Ektron.Cms.User.EkUser objUser;
            objUser = m_refSiteApi.EkUserRef;
            pagedata = new Collection();
            pagedata.Add(Request.Form[UserGroupName.UniqueID], "UserGroupName", null, null);
            pagedata.Add(Request.Form[group_description.UniqueID], "Description", null, null);
            objUser.AddMemberShipGroupV4(pagedata, "", "");
            pagedata = null;
            objUser = null;
        }

        Response.Redirect((string)("users.aspx?action=viewallgroups&grouptype=" + m_intGroupType), false);
    }
    #region AddUserGroup
    private void Display_AddUserGroup()
    {
        language_data = m_refSiteApi.GetAllActiveLanguages();
        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        AddUserGroupToolBar();
    }
    private void AddUserGroupToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user group msg"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("users.aspx?action=viewallgroups&grouptype=" + m_intGroupType + "&OrderBy=" + Request.QueryString["OrderBy"] + "&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user group)"), m_refMsg.GetMessage("btn save"), "onclick=\"return SubmitForm(\'UserGroupInfo\', \'VerifyGroupName()\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");

        if (m_intGroupType == 0)
        {
            result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar", ""));
        }
        else
        {
            result.Append(m_refStyle.GetHelpButton("AddMembershipGroup", ""));
        }

        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion
    #region AddUserGroup_Search
    private void Display_AddUserGroup_Search()
    {
        if (search == "1")
        {
            m_strFilter = Request.Form["groupname"];
            m_strDomain = Request.Form["domainname"];
        }
        else
        {
            m_strFilter = Request.QueryString["groupname"];
            m_strDomain = Request.QueryString["domainname"];
        }
        if (m_strDomain == "All Domains")
        {
            m_strDomain = "";
        }
        group_list = m_refUserApi.GetAvailableADGroups(m_strFilter, m_strDomain);
        AddUserGroupToolBar_Search();
        AddUserGroupGrid_Search();
    }
    private void AddUserGroupToolBar_Search()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view groups in active directory msg"));
        result.Append("<table><tr>");
		if (Request.ServerVariables["HTTP_USER_AGENT"].ToString().IndexOf("MSIE") > -1) //defect 16045
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		if (!(group_list == null))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (groups)"), m_refMsg.GetMessage("btn save"), "onclick=\"return SubmitForm(\'aduserinfo\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar_Search", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void AddUserGroupGrid_Search()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECK";
        colBound.HeaderText = "<input type=\"Checkbox\" name=\"checkall\" onclick=\"CheckAll();\">";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(15);
        colBound.ItemStyle.Width = Unit.Percentage(15);
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DOMAINTITLE";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        AddGroupGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("GROUPTITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("DOMAINTITLE", typeof(string)));

        int i = 0;
        if (!(group_list == null))
        {
            for (i = 0; i <= group_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<input type=\"CHECKBOX\" name=\"addgroup" + (i + 1) + "\" value=\"" + group_list[i].GroupPath + "\">";
                dr[1] = group_list[i].GroupName;
                dr[2] = group_list[i].GroupDomain;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("no ad groups found");
            dr[1] = "";
            dr[2] = "";
            dt.Rows.Add(dr);
        }
        addgroupcount.Value = i.ToString() + 1;
        DataView dv = new DataView(dt);
        AddGroupGrid.DataSource = dv;
        AddGroupGrid.DataBind();
    }
    #endregion
    #region AddUserGroup_None
    private void Display_AddUserGroup_None()
    {
        postbackpage.Text = Utilities.SetPostBackPage((string)("users.aspx?Action=AddUserGroup&grouptype=" + m_intGroupType + "&Search=1&LangType=" + ContentLanguage));
        AddUserGroupToolBar_None();
        AddUserGroupGrid_None();
    }
    private void AddUserGroupGrid_None()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        AddGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DOMAINTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        AddGroupGrid.Columns.Add(colBound);


        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("GROUPTITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("DOMAINTITLE", typeof(string)));

        dr = dt.NewRow();
        int i;
        dr[0] = "<input type=\"Text\" name=\"groupname\" maxlength=\"255\" size=\"25\" onkeypress=\"return CheckKeyValue(event,\'34\');\">";
        dr[1] = "<select name=\"domainname\">";
        if (!(domain_list == null))
        {
            if (m_refContentApi.RequestInformationRef.ADAdvancedConfig == false)
            {
                dr[1] += "<option selected value=\"All Domains\">" + m_refMsg.GetMessage("all domain select caption") + "</option>";
            }
            for (i = 0; i <= domain_list.Length - 1; i++)
            {
                dr[1] += "<option value=\"" + domain_list[i].Name + "\">" + domain_list[i].Name + "</option>";
            }
        }
        dr[1] += "</select>";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "<input type=\"submit\" name=\"search\" value=\"" + m_refMsg.GetMessage("generic Search") + "\">";
        dr[1] = "";
        dt.Rows.Add(dr);
        DataView dv = new DataView(dt);
        AddGroupGrid.DataSource = dv;
        AddGroupGrid.DataBind();
    }
    private void AddUserGroupToolBar_None()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("active directory group search msg"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar_None", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (PageAction == "addusergroup")
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (e.Item.Cells[0].Text.Equals(m_refMsg.GetMessage("no ad groups found")))
                    {
                        e.Item.Cells[0].ColumnSpan = 3;
                        e.Item.Cells.RemoveAt(2);
                        e.Item.Cells.RemoveAt(1);
                    }
                    break;
            }
        }
    }
    #endregion

    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        CollectSearchText();
        DisplayUsers();
        isPostData.Value = "true";
    }
    private void CollectSearchText()
    {
        m_strKeyWords = Request.Form["txtSearch"];
        m_strSelectedItem = Request.Form["searchlist"];
        if (m_strSelectedItem == "-1")
        {
            m_strSearchText = " (first_name like \'%" + m_strKeyWords + "%\' OR last_name like \'%" + m_strKeyWords + "%\' OR user_name like \'%" + m_strKeyWords + "%\')";
        }
        else if (m_strSelectedItem == "last_name")
        {
            m_strSearchText = " (last_name like \'%" + m_strKeyWords + "%\')";
        }
        else if (m_strSelectedItem == "first_name")
        {
            m_strSearchText = " (first_name like \'%" + m_strKeyWords + "%\')";
        }
        else if (m_strSelectedItem == "user_name")
        {
            m_strSearchText = " (user_name like \'%" + m_strKeyWords + "%\')";
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
    private void PageSettings()
    {
        if (m_intTotalPages <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(m_intTotalPages)).ToString());
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
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

    private void RegisterResources()
    {
        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronSearchBoxInputLabelInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        // register CSS
    }
}