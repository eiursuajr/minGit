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

public partial class viewgroups : System.Web.UI.UserControl
{


    protected LanguageData[] language_data;
    protected UserGroupData user_data;
    protected PermissionData security_data;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected DomainData[] domain_data;
    protected string UserName = "";
    protected int ContentLanguage = -1;
    protected string FirstName = "";
    protected string LastName = "";
    protected string Domain = "";
    protected SettingsData setting_data;
    protected UserGroupData[] usergroup_data;
    protected EmailHelper m_refEmail = new EmailHelper();
    protected string search = "";
    protected string strFilter = "";
    protected string rp = "";
    protected string e1 = "";
    protected string e2 = "";
    protected string f = "";
    protected int m_intGroupType = 0;
    protected long m_intGroupId = -1;
    private bool _LoadedInIframe = false;
    private string _SubscriptionGroupType;
    private bool _ActiveDirectory = false;
    protected PagingInfo _pagingInfo = new Ektron.Cms.PagingInfo();

    //Note: this is used by Commerce >> Items Tab >> Subscriptions >> Membership
    private bool LoadedInIframe
    {
        get
        {
            return _LoadedInIframe;
        }
        set
        {
            _LoadedInIframe = value;
        }
    }

    private string SubscriptionGroupType
    {
        get
        {
            return _SubscriptionGroupType;
        }
        set
        {
            _SubscriptionGroupType = value;
        }
    }

    public bool ActiveDirectory
    {
        get
        {
            return _ActiveDirectory;
        }
        set
        {
            _ActiveDirectory = value;
        }
    }

    #region Load
    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        if ((!(Request.QueryString["grouptype"] == null)) && (Request.QueryString["grouptype"] != ""))
        {           
            if (!string.IsNullOrEmpty(Request.QueryString["RequestedBy"]))
            {
                this.LoadedInIframe = System.Convert.ToBoolean((Request.QueryString["RequestedBy"] == "EktronCommerceItemsSusbscriptionsMembership") ? true : false);
                this._SubscriptionGroupType = (string)((Request.QueryString["grouptype"] == "0") ? "cms" : "membership");
            }
        }
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        RegisterResources();
        if ((!(Request.QueryString["grouptype"] == null)) && (Request.QueryString["grouptype"] != ""))
        {
            m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
        }
        if ((!(Request.QueryString["groupid"] == null)) && (Request.QueryString["groupid"] != ""))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
        }
        else if ((!(Request.QueryString["id"] == null)) && (Request.QueryString["id"] != ""))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["id"]);
        }
        Utilities.SetLanguage(m_refSiteApi);
        m_refMsg = m_refSiteApi.EkMsgRef;
        AppImgPath = m_refSiteApi.AppImgPath;
        ContentLanguage = m_refSiteApi.ContentLanguage;
        _pagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        if (Page.IsPostBack)
        {
            int iCurrentPage = 1;
            if (int.TryParse(CurrentPage.Text, out iCurrentPage))
            {
                _pagingInfo.CurrentPage = iCurrentPage;
            }
        }
        else
        {
            VisiblePageControls(false);
            if (!_ActiveDirectory)
            {
                ViewUserGroups();
            }
        }
    }
    #endregion

    #region ViewUserGroup
    public bool ViewUserGroups()
    {
        string OrderBy = (string)((string.IsNullOrEmpty(Request.QueryString["OrderBy"])) ? "GroupName" : (Request.QueryString["OrderBy"]));
        Ektron.Cms.Common.EkEnumeration.UserTypes groupType = m_intGroupType == 0 ? EkEnumeration.UserTypes.AuthorType : EkEnumeration.UserTypes.MemberShipType;
        Ektron.Cms.API.User.User apiUser = new Ektron.Cms.API.User.User();

        usergroup_data = apiUser.GetAllUserGroups(groupType, OrderBy, ref _pagingInfo);

        //TOOLBAR
        ViewUserGroupsToolBar();
        Populate_ViewUserGroups();
        PageSettings();

        return true;

    }
    private void Populate_ViewUserGroups()
    {
        string Icon = "users.png";
        if (m_intGroupType == 1)
        {
            Icon = "usersMembership.png";
        }
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPNAME";
        colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(65);
        colBound.ItemStyle.Width = Unit.Percentage(65);
        MapCMSGroupToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPID";
        colBound.HeaderText = m_refMsg.GetMessage("lbl Group ID"); //m_refMsg.GetMessage("generic User Group Name")
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        MapCMSGroupToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COUNT";
        colBound.HeaderText = m_refMsg.GetMessage("generic Number of Users");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        MapCMSGroupToADGrid.Columns.Add(colBound);
        if (m_intGroupType == 0 && string.IsNullOrEmpty(this.SubscriptionGroupType))
        {
            if (m_refEmail.IsLoggedInUsersEmailValid())
            {
                colBound = new System.Web.UI.WebControls.BoundColumn();
                colBound.DataField = "EMAIL";
                colBound.HeaderText = (string)("<a href=\"#\" onclick=\"ToggleEmailCheckboxes();\" title=\"" + m_refMsg.GetMessage("alt send email to all") + "\"><input type=\"checkbox\"></a>&nbsp;" + m_refMsg.GetMessage("generic all"));
                colBound.ItemStyle.Wrap = false;
                colBound.HeaderStyle.CssClass = "title-header";
                colBound.HeaderStyle.Width = Unit.Percentage(10);
                colBound.ItemStyle.Width = Unit.Percentage(10);
                MapCMSGroupToADGrid.Columns.Add(colBound);
            }
        }
        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("GROUPNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("GROUPID", typeof(string)));
        dt.Columns.Add(new DataColumn("COUNT", typeof(string)));
        if (m_intGroupType == 0)
        {
            if (m_refEmail.IsLoggedInUsersEmailValid())
            {
                dt.Columns.Add(new DataColumn("EMAIL", typeof(string)));
            }
        }
        if (!(usergroup_data == null))
        {
            for (int i = 0; i <= usergroup_data.Length - 1; i++)
            {
                dr = dt.NewRow();
                if (this.LoadedInIframe == true)
                {
                    //This is required for Commcerce >> Items Tab >> Subscriptions >> Membership.ascx
                    dr["GROUPNAME"] = "<a href=\"#AddGroup\" onclick=\"parent.Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.add(\'" + this.SubscriptionGroupType + "\', \'" + usergroup_data[i].GroupId + "\', \'" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\');return false;\"><img src=\"" + AppImgPath + "../UI/Icons/" + Icon + "\" align=\"absbottom\" title=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\' alt=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\'></a>&nbsp;<a href=\"#AddGroup\" onclick=\"parent.Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.add(\'" + this.SubscriptionGroupType + "\', \'" + usergroup_data[i].GroupId + "\', \'" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\');return false;\">" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "</a>";
                }
                else
                {
                    dr["GROUPNAME"] = "<a href=\"users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&groupid=" + usergroup_data[i].GroupId + "&id=" + usergroup_data[i].GroupId + "\" title=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\'><img src=\"" + AppImgPath + "../UI/Icons/" + Icon + "\" align=\"absbottom\" title=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\' alt=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\'></a>&nbsp;<a href=\"users.aspx?action=viewallusers&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "&groupid=" + usergroup_data[i].GroupId + "&id=" + usergroup_data[i].GroupId + "\" title=\'" + m_refMsg.GetMessage("view user group msg") + " \"" + usergroup_data[i].GroupDisplayName.Replace("\'", "`") + "\"\'>" + usergroup_data[i].GroupDisplayName.Replace
                    ("\'", "`") + "</a>";
                }

                dr["GROUPID"] = usergroup_data[i].GroupId;
                dr["COUNT"] = usergroup_data[i].UserCount;
                if (m_intGroupType == 0 && string.IsNullOrEmpty(this.SubscriptionGroupType))
                {
                    if (m_refEmail.IsLoggedInUsersEmailValid())
                    {

                        dr["EMAIL"] = "<input type=\"checkbox\" name=\"emailcheckbox_" + usergroup_data[i].GroupId + "\" ID=\"Checkbox1\">";
                        dr["EMAIL"] += "<a href=\"#\" onclick=\"SelectEmail(\'emailcheckbox_" + usergroup_data[i].GroupId + "\');return false\">";
                        dr["EMAIL"] += m_refEmail.MakeEmailGraphic() + "</a>";
                    }
                }
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        MapCMSGroupToADGrid.DataSource = dv;
        MapCMSGroupToADGrid.DataBind();
    }
    private void ViewUserGroupsToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view user groups msg"));
        result.Append("<table><tr>");
        bool addHelpDivider = false;
        if (m_intGroupType == 0)
        {
            bool primaryCssApplied = false;

            if (string.IsNullOrEmpty(this.SubscriptionGroupType))
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/add.png", "users.aspx?action=addusergroup&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "", m_refMsg.GetMessage("alt add button text (user group2)"), m_refMsg.GetMessage("btn add user group"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

                primaryCssApplied = true;
                addHelpDivider = true;
            }

            if (m_refEmail.IsLoggedInUsersEmailValid() && string.IsNullOrEmpty(this.SubscriptionGroupType))
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected groups"), m_refMsg.GetMessage("btn email"), "onclick=\"LoadEmailChildPageEx();\"", StyleHelper.EmailButtonCssClass, !primaryCssApplied));

                primaryCssApplied = true;
                addHelpDivider = true;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(this.SubscriptionGroupType))
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/add.png", "users.aspx?action=addusergroup&grouptype=" + m_intGroupType + "&LangType=" + ContentLanguage + "", m_refMsg.GetMessage("alt add button text (user group2)"), m_refMsg.GetMessage("btn add membership usergroup"), "", StyleHelper.AddButtonCssClass, true));

                addHelpDivider = true;
            }
        }

        if (addHelpDivider)
        {
            result.Append(StyleHelper.ActionBarDivider);
        }

        result.Append("<td>");
        if (string.IsNullOrEmpty(this.SubscriptionGroupType))
        {
            if (m_intGroupType == 0)
            {
                result.Append(m_refStyle.GetHelpButton("ViewUserGroupsToolBar", ""));
            }
            else
            {
                result.Append(m_refStyle.GetHelpButton("ViewMembershipGroups", ""));
            }
        }
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion

    #region Grid Events
    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _pagingInfo.CurrentPage = 1;
                break;
            case "Last":
                _pagingInfo.CurrentPage = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _pagingInfo.CurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _pagingInfo.CurrentPage = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        ViewUserGroups();
        PageSettings();
    }

    public void LinkBtn_Click(object sender, System.EventArgs e)
    {

    }
    #endregion

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
        if (_pagingInfo.TotalPages <= 1)
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(_pagingInfo.TotalPages))).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _pagingInfo.CurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
            PreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (_pagingInfo.CurrentPage == 1)
            {
                PreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_pagingInfo.CurrentPage == _pagingInfo.TotalPages)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }

    #region MapCMSUserGroupToAD
    public bool MapCMSUserGroupToAD()
    {
        search = Request.QueryString["search"];
        if (!Page.IsPostBack || (Page.IsPostBack && !string.IsNullOrEmpty(Request.Form["domainname"])))
        {
            VisiblePageControls(false);
            Display_MapCMSUserGroupToAD();
            return true;
        }
        else
        {
            Process_MapCMSUserGroupToAD();
            return (true);
        }
    }
    private void Process_MapCMSUserGroupToAD()
    {
        string[] tempArray = Strings.Split(Request.Form["usernameanddomain"], "_@_", -1, 0);
        string strUserName = tempArray[0].ToString();
        string strDomain = tempArray[1].ToString();
        m_refUserApi.MapCMSUserGroupToAD(m_intGroupId, strUserName, strDomain);
        string returnPage = "";
        if (Request.Form["rp"] == "1")
        {
            returnPage = (string)("users.aspx?action=viewallgroups&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType);
        }
        else
        {
            returnPage = (string)("adreports.aspx?action=SynchUsers&ReportType=" + Request.Form["rt"] + "&groupid=" + m_intGroupId + "&grouptype=" + m_intGroupType);
        }
        Response.Redirect(returnPage, false);
    }
    private void Display_MapCMSUserGroupToAD()
    {
        AppImgPath = m_refSiteApi.AppImgPath;
        rp = Request.QueryString["rp"];
        e1 = Request.QueryString["e1"];
        e2 = Request.QueryString["e2"];
        f = Request.QueryString["f"];
        if (rp == "")
        {
            rp = Request.Form["rp"];
        }

        if (e1 == "")
        {
            e1 = Request.Form["e1"];
        }

        if (e2 == "")
        {
            e2 = Request.Form["e2"];
        }

        if (f == "")
        {
            f = Request.Form["f"];
        }
        language_data = m_refSiteApi.GetAllActiveLanguages();
        user_data = m_refUserApi.GetUserGroupById(m_intGroupId);

        if ((m_intGroupId == 1) && (Convert.ToInt32(rp) == 3))
        {
            domain_data = m_refUserApi.GetDomains(1, 0);
        }
        else
        {
            domain_data = m_refUserApi.GetDomains(0, 0);
        }

        security_data = m_refContentApi.LoadPermissions(0, "content", 0);
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

        if ((search == "1") || (search == "2"))
        {

            if (search == "1")
            {
                strFilter = Request.Form["groupname"];
                Domain = Request.Form["domainname"];
            }
            else
            {
                strFilter = Request.QueryString["groupname"];
                Domain = Request.QueryString["domainname"];
            }
            if (Domain == "All Domains")
            {
                Domain = "";
            }
            GroupData[] adgroup_data;
            adgroup_data = m_refUserApi.GetAvailableADGroups(strFilter, Domain);
            //TOOLBAR
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("search ad for cms group") + " \"" + user_data.GroupDisplayName + "\""));
            result.Append("<table><tr>");

            //if (Request.ServerVariables["HTTP_USER_AGENT"].ToString().IndexOf("MSIE") > -1) //defect 16045
            //{
            //    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            //}
            //else
            //{
            //}
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));


            if (rp != "1")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("close title"), "onclick=\"top.close();\"", StyleHelper.SaveButtonCssClass, true));
            }

            if (!(adgroup_data == null))
            {
                if (rp == "1")
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (associate group)"), m_refMsg.GetMessage("btn update"), "onclick=\"return SubmitForm(\'aduserinfo\', \'CheckRadio(1);\');\"", StyleHelper.SaveButtonCssClass, true));
                }
                else
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (associate group)"), m_refMsg.GetMessage("btn update"), "onclick=\"return SubmitForm(\'aduserinfo\', \'CheckReturn(1);\');\"", StyleHelper.SaveButtonCssClass, true));
                }
            }
            result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserGroupToAD", ""));
            result.Append("</td>");
            result.Append("</tr></table>");
            htmToolBar.InnerHtml = result.ToString();
            //Dim i As Integer = 0
            //If (Not (IsNothing(domain_data))) Then
            //    domainname.Items.Add(New ListItem(m_refMsg.GetMessage("all domain select caption"), ""))
            //    domainname.Items(0).Selected = True
            //    For i = 0 To domain_data.Length - 1
            //        domainname.Items.Add(New ListItem(domain_data(i).Name, domain_data(i).Name))
            //    Next
            //End If
            Populate_MapCMSGroupToADGrid(adgroup_data);
        }
        else
        {

            PostBackPage.Text = Utilities.SetPostBackPage((string)("users.aspx?Action=MapCMSUserGroupToAD&Search=1&LangType=" + ContentLanguage + "&rp=" + rp + "&e1=" + e1 + "&e2=" + e2 + "&f=" + f + "&grouptype=" + m_intGroupType + "&groupid=" + m_intGroupId));
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("search ad for cms group") + " \"" + user_data.DisplayUserName + "\""));
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append("<table><tr>");
            if (rp != "1")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=\"top.close();\""));
            }
            else
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            }
            result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserGroupToAD", ""));
            result.Append("</td>");
            result.Append("</tr></table>");
            htmToolBar.InnerHtml = result.ToString();
            Populate_MapCMSUserGroupToADGrid_Search(domain_data);
        }
    }
    private void Populate_MapCMSUserGroupToADGrid_Search(DomainData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(40);
        colBound.ItemStyle.Width = Unit.Percentage(40);
        MapCMSGroupToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DOMAINTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(40);
        colBound.ItemStyle.Width = Unit.Percentage(40);
        MapCMSGroupToADGrid.Columns.Add(colBound);
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<input type=\"hidden\" name=\"id\" value=\"" + user_data.UserId + "\">");
        result.Append("<input type=\"hidden\" name=\"rp\" value=\"" + rp + "\">");
        result.Append("<input type=\"hidden\" name=\"e1\" value=\"" + e1 + "\">");
        result.Append("<input type=\"hidden\" name=\"e2\" value=\"" + e2 + "\">");
        result.Append("<input type=\"hidden\" name=\"f\" value=\"" + f + "\">");
        result.Append("<input type=\"hidden\" name=\"adusername\">");
        result.Append("<input type=\"hidden\" name=\"addomain\">");


        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("GROUPTITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("DOMAINTITLE", typeof(string)));
        dr = dt.NewRow();
        dr[0] = "<input type=\"Text\" name=\"groupname\" maxlength=\"255\" size=\"25\" OnKeyPress=\"javascript:return CheckKeyValue(event,\'34\');\">";
        dr[0] += result.ToString();
        int i = 0;
        dr[1] = "<select name=\"domainname\">";
        if ((!(domain_data == null)) && m_refContentApi.RequestInformationRef.ADAdvancedConfig == false)
        {
            dr[1] += "<option selected value=\"All Domains\">" + m_refMsg.GetMessage("all domain select caption") + "</option>";
        }
        for (i = 0; i <= domain_data.Length - 1; i++)
        {
            dr[1] += "<option value=\"" + domain_data[i].Name + "\">" + domain_data[i].Name + "</option>";
        }
        dr[1] += "</select>";

        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = "<input type=\"submit\" name=\"search\" value=\"" + m_refMsg.GetMessage("generic Search") + "\">";
        dr[1] = "";

        dt.Rows.Add(dr);


        DataView dv = new DataView(dt);
        MapCMSGroupToADGrid.DataSource = dv;
        MapCMSGroupToADGrid.DataBind();
    }
    private void Populate_MapCMSGroupToADGrid(GroupData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("add title");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        MapCMSGroupToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "GROUPTITLE";
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(15);
        colBound.HeaderStyle.Width = Unit.Percentage(15);
        MapCMSGroupToADGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DOMAINTITLE";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = m_refMsg.GetMessage("domain title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(40);
        colBound.HeaderStyle.Width = Unit.Percentage(40);
        MapCMSGroupToADGrid.Columns.Add(colBound);


        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("GROUPTITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("DOMAINTITLE", typeof(string)));
        int i = 0;
        if (!(data == null))
        {
            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<input type=\"Radio\" name=\"usernameanddomain\" value=\"" + data[i].GroupName + "_@_" + data[i].GroupDomain + "\" onClick=\"SetUp(\'" + data[i].GroupName + "_@_" + data[i].GroupDomain + "\')\">";
                dr[1] = data[i].GroupName;
                dr[2] = data[i].GroupDomain;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("no ad groups found");
            dr[1] = "";
            dt.Rows.Add(dr);
        }
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<input type=\"hidden\" name=\"id\" value=\"" + user_data.UserId + "\">");
        result.Append("<input type=\"hidden\" name=\"rp\" value=\"" + rp + "\">");
        result.Append("<input type=\"hidden\" name=\"e1\" value=\"" + e1 + "\">");
        result.Append("<input type=\"hidden\" name=\"e2\" value=\"" + e2 + "\">");
        result.Append("<input type=\"hidden\" name=\"f\" value=\"" + f + "\">");
        result.Append("<input type=\"hidden\" name=\"adusername\">");
        result.Append("<input type=\"hidden\" name=\"addomain\">");
        dr = dt.NewRow();
        dr[0] = result.ToString();
        dr[1] = "";
        dt.Rows.Add(dr);

        DataView dv = new DataView(dt);
        MapCMSGroupToADGrid.DataSource = dv;
        MapCMSGroupToADGrid.DataBind();
    }
    #endregion
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }

}
	
