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
using Ektron.Cms.Workarea;
using Ektron.Cms.Framework.UI;
//using Ektron.Cms.Common.EkEnumeration;

public partial class Community_groupmembers : workareabase
{
    protected CommunityGroupData m_cGroup = new CommunityGroupData();
    protected DirectoryUserData[] m_aUsers = (Ektron.Cms.DirectoryUserData[])Array.CreateInstance(typeof(Ektron.Cms.DirectoryUserData), 0);
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected SettingsData setting_data;
    protected string m_strKeyWords = "";
    protected int m_PageSize = 50;
    protected string m_strSelectedItem = "-1";
    protected bool m_bAllowAdd = false;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (Request.Form["txtSearch"] != "")
        {
            m_strKeyWords = Request.Form["txtSearch"];
        }
        this.groupID.Value = m_iID.ToString();
        m_PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        try
        {
            if (CheckAccess() == false)
            {
                throw (new Exception(this.GetMessage("err communitymembers no access")));
            }
            switch (this.m_sPageAction)
            {
                case "adduser":
                    if (Page.IsPostBack)
                    {
                        CollectSearchText();
                        if (Request.Form["isDeleted"] != "")
                        {
                            Process_Add();
                        }
                        else if (Request.Form["isSearchPostData"] != "")
                        {
                            this.isSearchPostData.Value = "";
                            Display_Add();
                        }
                    }
                    else
                    {
                        Display_Add();
                    }
                    break;
                case "pending":
                    if (Page.IsPostBack && Request.Form["isDeleted"] != "")
                    {
                        Process_Pending();
                        Display_PendingView();
                        this.isDeleted.Value = "";
                    }
                    else if (Page.IsPostBack && Request.Form["isSearchPostData"] != "")
                    {
                        CollectSearchText();
                        Display_PendingView();
                    }
                    else if (!Page.IsPostBack)
                    {
                        Display_PendingView();
                    }
                    break;
                default: // "viewall"
                    if (Page.IsPostBack && Request.Form["isDeleted"] != "")
                    {
                        Process_Remove();
                        Display_View();
                        this.isDeleted.Value = "";
                    }
                    else if (Page.IsPostBack && Request.Form["isSearchPostData"] != "")
                    {
                        CollectSearchText();
                        CurrentPage.Text = "1";
                        Display_View();
                        this.isSearchPostData.Value = "";
                    }
                    else if (!Page.IsPostBack)
                    {
                        Display_View();
                    }
                    break;
            }
            BuildJS();

            if (Page.IsPostBack)
            {
                SiteAPI m_refSiteApi = new SiteAPI();
                setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

                CommunityGroupObjectRequest request = new CommunityGroupObjectRequest();
                request.CurrentPage = m_intCurrentPage;
                request.PageSize = System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0);
                request.GroupId = this.m_iID;
                request.SearchText = m_strKeyWords == null ? "" : m_strKeyWords;
                switch (m_strSelectedItem)
                {
                    case "last_name":
                        request.SearchField = "last_name";
                        break;
                    case "first_name":
                        request.SearchField = "first_name";
                        break;
                    case "user_name":
                        request.SearchField = "user_name";
                        break;
                    default: // "-1"
                        request.SearchField = "display_name";
                        break;
                }
                request.ObjectId = this.m_refContentApi.UserId;
                request.ObjectType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.User;
                request.ObjectStatus = Ektron.Cms.Common.EkEnumeration.DirectoryItemStatus.All;
                m_aUsers = this.m_refCommunityGroupApi.GetAllUnassignedCommunityGroupUsers(ref request);
                if ((m_aUsers != null) && m_aUsers.Length > 0)
                {
                    m_bAllowAdd = true;
                }
            }
            SetLabels();
            RegisterResources();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }

    }

    #region Display

    public void Display_Add()
    {
        m_cGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);

        Packages.EktronCoreJS.Register(this);
        JavaScript.Create(this.m_refContentApi.ApplicationPath + "java/plugins/inputLabel/ektron.inputLabel.js").Register(this);

        ltr_search.Text = "<br/>&#160;" + GetMessage("lbl select users") + ":<br/>";
        ltr_search.Text += "&#160;" + "<input type=text size=25 id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\">";
        ltr_search.Text += "&#160;";
        ltr_search.Text += "<select id=\"searchlist\" name=\"searchlist\">";
        ltr_search.Text += "<option value=\"-1\" " + IsSelected("-1") + ">" + GetMessage("display name label") + "</option>";
        ltr_search.Text += "<option value=\"last_name\"" + IsSelected("last_name") + ">" + GetMessage("lbl last name") + "</option>";
        ltr_search.Text += "<option value=\"first_name\"" + IsSelected("first_name") + ">" + GetMessage("lbl first name") + "</option>";
        ltr_search.Text += "<option value=\"user_name\"" + IsSelected("user_name") + ">" + GetMessage("generic username") + "</option>";
        // ltr_search.Text &= "<option value=""all"" " & IsSelected("all") & ">" & GetMessage("generic all") & "</option>"
        ltr_search.Text += "</select>";
        ltr_search.Text += "&#160;";
        ltr_search.Text += "<input type=button value=\"Search\" id=\"btnSearch\" name=\"btnSearch\" onclick=\"searchuser();\" title=\"Search Users\">";

        if (Page.IsPostBack)
        {
            SiteAPI m_refSiteApi = new SiteAPI();
            setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

            CommunityGroupObjectRequest request = new CommunityGroupObjectRequest();
            request.CurrentPage = m_intCurrentPage;
            request.PageSize = System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0);
            request.GroupId = this.m_iID;
            request.SearchText = m_strKeyWords;
            switch (m_strSelectedItem)
            {
                case "last_name":
                    request.SearchField = "last_name";
                    break;
                case "first_name":
                    request.SearchField = "first_name";
                    break;
                case "user_name":
                    request.SearchField = "user_name";
                    break;
                default: // "-1"
                    request.SearchField = "display_name";
                    break;
            }
            request.ObjectId = this.m_refContentApi.UserId;
            request.ObjectType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.User;
            request.ObjectStatus = Ektron.Cms.Common.EkEnumeration.DirectoryItemStatus.All;
            m_aUsers = this.m_refCommunityGroupApi.GetAllUnassignedCommunityGroupUsers(ref request);
            if ((m_aUsers != null) && m_aUsers.Length > 0)
            {
                m_bAllowAdd = true;
            }
            m_intTotalPages = request.TotalPages;
            Populate_ViewCommunityMembersGrid(m_aUsers);
        }
        else
        {
            PageSettings(); // to suppress the paging stuff
        }
    }
    public void Display_PendingView()
    {
        SiteAPI m_refSiteApi = new SiteAPI();
        int totalUsers = 0;
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

        m_cGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        if (Page.IsPostBack)
        {
            m_aUsers = this.m_refCommunityGroupApi.GetPendingCommunityGroupUsers(this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref totalUsers);
        }
        else
        {
            m_aUsers = this.m_refCommunityGroupApi.GetPendingCommunityGroupUsers(this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref totalUsers);
        }
        Populate_ViewCommunityMembersGrid(m_aUsers);
    }
    public void Display_View()
    {
        SiteAPI m_refSiteApi = new SiteAPI();
        int totalUsers = 0;
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

        m_cGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        int totalPending = 0;
        if (Page.IsPostBack && Request.Form["isSearchPostData"] != "")
        {
            m_aUsers = this.m_refCommunityGroupApi.GetCommunityGroupUsers(this.m_iID, this.m_strKeyWords, "display_name", m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref totalUsers, ref totalPending);
        }
        else
        {
            m_aUsers = this.m_refCommunityGroupApi.GetCommunityGroupUsers(this.m_iID, m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages, ref totalUsers, ref totalPending);
        }
        Populate_ViewCommunityMembersGrid(m_aUsers);
    }

    #endregion

    #region Process

    protected void Process_Add()
    {
        string[] aUid = (string[])Array.CreateInstance(typeof(string), 0);
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    this.m_refCommunityGroupApi.AddUserToCommunityGroup(this.m_iID, Convert.ToInt64(aUid[i]), true);
                }
            }
        }
        Response.Redirect((string)("groupmembers.aspx?action=viewallusers&LangType=" + this.ContentLanguage + "&id=" + this.m_iID), false); // txtSearch
    }

    protected void Process_Remove()
    {
        string[] aUid = (string[])Array.CreateInstance(typeof(string), 0);
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    this.m_refCommunityGroupApi.RemoveUserFromCommunityGroup(this.m_iID, Convert.ToInt64(aUid[i]));
                }
            }
        }
    }

    protected void Process_Pending()
    {
        string stype = (string)(Request.Form["isDeleted"].ToString().ToLower());
        string[] aUid = (string[])Array.CreateInstance(typeof(string), 0);
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    if (stype == "approve")
                    {
                        this.m_refCommunityGroupApi.ApprovePendingGroupUser(Convert.ToInt64(aUid[i]), this.m_iID);
                        Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string)("GroupAccess_" + m_iID.ToString() + "_" + aUid[i].ToString()));
                    }
                    else if (stype == "decline")
                    {
                        this.m_refCommunityGroupApi.DeletePendingGroupUser(Convert.ToInt64(aUid[i]), this.m_iID);
                        Ektron.Cms.Common.Cache.ApplicationCache.Invalidate((string)("GroupAccess_" + m_iID.ToString() + "_" + aUid[i].ToString()));
                    }
                }
            }
        }
    }

    #endregion

    #region Helper Functions

    public bool CheckAccess()
    {
        return true;
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

    private bool IsUserGroupAdmin(List<Ektron.Cms.UserData> groupAdmins)
    {
        foreach (UserData adminData in groupAdmins)
        {
            if (adminData.Id == m_refContentApi.UserId)
                return true;
        }
        return false;
    }

    private bool IsUserGroupAdmin(List<Ektron.Cms.UserData> groupAdmins, long userId)
    {
        foreach (UserData adminData in groupAdmins)
        {
            if (adminData.Id == userId)
                return true;
        }
        return false;
    }

    public void SetLabels()
    {
        m_cGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        string url = string.Empty;
        switch (this.m_sPageAction)
        {
            case "adduser":
                this.AddBackButton((string)("groupmembers.aspx?&id=" + this.m_iID + "&LangType=" + this.ContentLanguage));
                if (m_bAllowAdd && (m_refContentApi.IsAdmin() || IsUserGroupAdmin(m_cGroup.Admins) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin)))
                {
                    this.AddButtonwithMessages(m_refContentApi.AppPath + "images/ui/icons/add.png", "#", "alt add sel users to cgroup", "btn add sel users to cgroup", " onclick=\"return CheckAdd();\" ", StyleHelper.AddButtonCssClass, true);
                }
                this.AddHelpButton("addcommunitygroupmembers");
                this.SetTitleBarToString(string.Format(GetMessage("btn add cgroup members for"), EkFunctions.HtmlEncode(this.m_cGroup.GroupName)));
                break;

            case "pending":
                url = (string)("groupmembers.aspx?id=" + this.m_iID);
                anchorCurrent.Attributes.Add("onclick", "window.location.href =\'" + url + "\'");
                anchorPending.Attributes.Add("onclick", "");
                this.AddBackButton((string)("groupmembers.aspx?id=" + this.m_iID + "&LangType=" + this.ContentLanguage));
                if (m_refContentApi.IsAdmin() || IsUserGroupAdmin(m_cGroup.Admins) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin))
                {
					this.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/approvals.png", "#", "alt approve pending users for cgroup", "btn approve pending users for cgroup", " onclick=\"return CheckPendingApprove();\" ", StyleHelper.ApproveButtonCssClass, true);
					this.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/remove.png", "#", "alt remove pending users from cgroup", "btn remove pending users from cgroup", " onclick=\"return CheckPendingDelete();\" ", StyleHelper.RemoveButtonCssClass);
                }
                this.AddHelpButton("viewpendingcommunitygroupmembers");
                this.SetTitleBarToString(string.Format(GetMessage("btn view pending cgroup members for"), EkFunctions.HtmlEncode(this.m_cGroup.GroupName)));
                break;

            default: // "viewall"
                url = (string)("groupmembers.aspx?action=pending&id=" + this.m_iID);
                anchorPending.Attributes.Add("onclick", "window.location.href =\'" + url + "\'");
                anchorCurrent.Attributes.Add("onclick", "");
                this.AddBackButton((string)("groups.aspx?action=viewgroup&id=" + this.m_iID + "&LangType=" + this.ContentLanguage));
                if (m_refContentApi.IsAdmin() || IsUserGroupAdmin(m_cGroup.Admins) || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin))
                {
					this.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/add.png", (string)("groupmembers.aspx?action=adduser&LangType=" + this.ContentLanguage + "&id=" + this.m_iID), "alt add users to cgroup", "btn add users to cgroup", "", StyleHelper.AddButtonCssClass, true);
					this.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/remove.png", "#", "alt remove users from cgroup", "btn remove users from cgroup", " onclick=\"return CheckDelete();\" ", StyleHelper.RemoveButtonCssClass);
                }

                this.AddSearchBox(this.m_strKeyWords, new ListItemCollection(), "ExecSearch");
                this.AddHelpButton("viewcommunitygroupmembers");
                this.SetTitleBarToString(string.Format(GetMessage("btn view cgroup members for"), EkFunctions.HtmlEncode(this.m_cGroup.GroupName)));
                break;
        }
    }

    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        jsConfirmRemoveMemberFromGroup.Text = GetMessage("js confirm remove member from cgroup");
        jsSelectAtLeastOneUser.Text = GetMessage("js err communitymembers please select user remove");
        jsPleaseSelectUserRemove.Text = GetMessage("js confirm remove pending member from cgroup");
        jsApproveSelectRequestsToJoin.Text = GetMessage("js confirm approve pending member from cgroup");
        jsPleaseSelectAtLeastOneJoinRequest.Text = GetMessage("js err pending communitymembers please select user approve");
        jsPleaseSelectUserToAdd.Text = GetMessage("js err communitymembers please select user add");
        jsCancelRequest.Text = GetMessage("js confirm remove pending member from cgroup");
    }

    private void Populate_ViewCommunityMembersGrid(DirectoryUserData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;
        string sAppend = "";
        MemberGrid.Columns.Clear();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKL";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Width = Unit.Percentage(5);
        colBound.HeaderText = "<input type=\"checkbox\" name=\"checkall\" id=\"req_deleted_users\" onclick=\"checkAll(\'\');\" />";
        MemberGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LEFT";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;

        colBound.HeaderText = GetMessage("generic select all msg");
        MemberGrid.Columns.Add(colBound);

        PageSettings();

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECKL", typeof(string)));
        dt.Columns.Add(new DataColumn("LEFT", typeof(string)));
        int i = 0;

        if (!(data == null))
        {
            // add select all row.
            //dr = dt.NewRow
            //dr("CHECKL") = ""
            //dr("LEFT") = GetMessage("generic select all msg")
            //dt.Rows.Add(dr)

            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                sAppend = "";
                if ((setting_data.ADAuthentication == 1) && (data[i].Domain != ""))
                {
                    sAppend = "@" + data[i].Domain;
                }
                if (IsUserGroupAdmin(m_cGroup.Admins, data[i].Id))
                {
                    dr["CHECKL"] = "&#160;";
                }
                else
                {
                    dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].Id + "\" onclick=\"checkAll(\'req_deleted_users\');\">";
                }
                dr["LEFT"] = "<img align=\"left\" src=\"" + ((data[i].Avatar != "") ? (data[i].Avatar) : this.m_refContentApi.AppImgPath + "user.gif") + "\" width=\"32\" height=\"32\"/><span title=\"" + data[i].FirstName + " " + data[i].LastName + "\">" + data[i].DisplayName + "</span>";
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);

        MemberGrid.DataSource = dv;
        MemberGrid.DataBind();
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
            TotalPages.Text = m_intTotalPages.ToString();
            TotalPages.ToolTip = TotalPages.Text;
            PreviousPage1.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                PreviousPage1.Enabled = false;
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
        PreviousPage1.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;


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

    private void CollectSearchText()
    {
        m_strKeyWords = Request.Form["txtSearch"];
        m_strSelectedItem = Request.Form["searchlist"];
        if (m_strSelectedItem == "-1")
        {
            //m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "last_name")
        {
            //m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "first_name")
        {
            //m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        }
        else if (m_strSelectedItem == "user_name")
        {
            //m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        }
    }

    #endregion

    #region Grid Events
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (this.m_sPageAction == "view")
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM") || e.Item.Cells[1].Text.Equals("important") || e.Item.Cells[1].Text.Equals("input-box-text"))
                    {
                        e.Item.Cells[0].Attributes.Add("align", "Left");
                        e.Item.Cells[0].ColumnSpan = 2;
                        if (e.Item.Cells[0].Text.Equals("REMOVE-ITEM"))
                        {
                            //e.Item.Cells(0).CssClass = ""
                        }
                        else
                        {
                            e.Item.Cells[0].CssClass = e.Item.Cells[1].Text;
                        }
                        e.Item.Cells.RemoveAt(1);
                    }
                    break;
            }
        }
    }

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
        CurrentPage.Text = m_intCurrentPage.ToString();
        CurrentPage.ToolTip = CurrentPage.Text;

        switch (this.m_sPageAction)
        {
            case "adduser":
                Display_Add();
                break;
            default:
                Display_View();
                break;
        }

        // isPostData.Value = "true"
    }
    #endregion

    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
    }

}
