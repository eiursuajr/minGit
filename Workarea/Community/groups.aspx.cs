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
using Ektron.Cms.ToDo;
using Ektron.Cms.Workarea;

public partial class Community_groups : Ektron.Cms.Workarea.workareabase
{
    protected CommunityGroupData cgGroup = new Ektron.Cms.CommunityGroupData();
    protected bool bAccess = false;
    protected bool bAddAccess = false;
    protected string sSearch = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 0;
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    private Ektron.Cms.Content.Calendar.WebCalendar _CalendarApi;
    Ektron.Cms.Common.Calendar.WebCalendarData calendardata = new Ektron.Cms.Common.Calendar.WebCalendarData();
    protected long _doesForumExists = -1;
    protected string groupAliasList = string.Empty;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _CalendarApi = new Ektron.Cms.Content.Calendar.WebCalendar(m_refContentApi.RequestInformationRef);
        RegisterResources();
        lblProperties.Text = m_refMsg.GetMessage("generic properties");
        lblTags.Text = m_refMsg.GetMessage("lbl personal tags");
        lblCategory.Text = m_refMsg.GetMessage("lbl category");
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            CheckAccess();
            if (!bAccess)
            {
                throw (new Exception(this.GetMessage("err communityaddedit no access")));
            }
            switch (m_sPageAction)
            {
                case "viewgroup":
                    ViewGroup();
                    break;

                case "delete":
                    Process_DeleteGroup();
                    break;

                case "addeditgroup":
                    if (Page.IsPostBack)
                    {
                        Process_EditGroup();
                    }
                    else
                    {
                        EditGroup();
                    }
                    break;

                default: // "viewallgroups"
                    if ((!Page.IsPostBack) || (Page.IsPostBack && Request.Form["hdn_search"] != ""))
                    {
                        ViewAllGroups(); // default to view all groups.
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
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

    protected void ViewAllGroups()
    {
        BuildJS();
        if (Page.IsPostBack)
        {
            sSearch = Request.Form["txtSearch"];
        }
        //if (Request.QueryString["page"] != "")
        //{
        //    m_intCurrentPage = Convert.ToInt32(Request.QueryString["page"]);
        //}
        CommunityGroupData[] aCGroups = (CommunityGroupData[])Array.CreateInstance(typeof(CommunityGroupData), 0);

        panel1.Visible = true;
        SetTitleBarToMessage("lbl view all cgroups");
        if (this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate) ||
            this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin))
        {
            bAddAccess = true;
        }
        if (bAddAccess)
        {
            base.AddButtonwithMessages(AppImgPath + "../UI/Icons/add.png", (string)("../communitygroupaddedit.aspx?action=addeditgroup&LangType=" + this.ContentLanguage), "alt add community group", "lbl add community group", "", StyleHelper.AddButtonCssClass, true);
        }
        AddSearchBox(sSearch, new ListItemCollection(), "ExecSearch");
        AddHelpButton("viewallcommunitygroups");

        CommunityGroupRequest cReq = new CommunityGroupRequest();
        cReq.CurrentPage = m_intCurrentPage;
        cReq.SearchText = sSearch;
        cReq.PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        aCGroups = this.m_refCommunityGroupApi.GetAllCommunityGroups(cReq);

        // CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""javascript:checkAll('selected_communitygroup',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("TITLE", GetMessage("lbl community group name"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("MEMBERS", GetMessage("lbl members"), "title-header", HorizontalAlign.Right, HorizontalAlign.Right, Unit.Percentage(5), Unit.Percentage(5), false, false));
        //CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("LANGUAGE", GetMessage("generic language"), "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(5), Unit.Percentage(5), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("ID", GetMessage("generic ID"), "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(5), Unit.Percentage(5), false, false));
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("ENROLL", GetMessage("lbl enrollment"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", GetMessage("lbl discussionforumtitle"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("LOCATION", GetMessage("generic location"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));

        DataTable dt = new DataTable();
        DataRow dr;
        // dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("MEMBERS", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        //dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(new DataColumn("ENROLL", typeof(string)));
        dt.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
        dt.Columns.Add(new DataColumn("LOCATION", typeof(string)));
        m_intTotalPages = cReq.TotalPages;
        PageSettings();
        if ((aCGroups != null) && aCGroups.Length > 0)
        {
            // AddDeleteIcon = True
            for (int i = 0; i <= aCGroups.Length - 1; i++)
            {
                if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
                {
                    if (!IsUserGroupAdmin(aCGroups[0].Admins))
                    {
                        continue;
                    }
                }
                dr = dt.NewRow();
                // dr("CHECK") = "<input type=""checkbox"" name=""selected_communitygroup"" id=""selected_communitygroup"" value=""" & aCGroups(i).GroupId & """ onClick=""javascript:checkAll('selected_communitygroup',true);"">"
                //dr("TITLE") = "<a href=""groups.aspx?action=viewgroup&id=" & aCGroups(i).GroupId & "&LangType=" & aCGroups(i).GroupLanguage & """>" & aCGroups(i).GroupName & "</a>"
                dr["TITLE"] = "<a href=\"groups.aspx?action=viewgroup&id=" + aCGroups[i].GroupId + "\">" + aCGroups[i].GroupName + "</a>";
                dr["MEMBERS"] = aCGroups[i].TotalMember;
                dr["ID"] = aCGroups[i].GroupId;
                //dr("LANGUAGE") = "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(aCGroups(i).GroupLanguage) & "' border=""0"" />"
                //dr("LANGUAGE") = "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(aCGroups(i).GroupLanguage) & "' border=""0"" />"
                dr["ENROLL"] = (aCGroups[i].GroupEnroll) ? "Open" : "Closed";
                dr["DESCRIPTION"] = aCGroups[i].GroupShortDescription;
                dr["LOCATION"] = aCGroups[i].GroupLocation;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            CommunityGroupList.GridLines = GridLines.None;
        }
        DataView dv = new DataView(dt);
        CommunityGroupList.DataSource = dv;
        CommunityGroupList.DataBind();
    }

    protected void ViewGroup()
    {
        DirectoryData[] m_aCategories = (DirectoryData[])Array.CreateInstance(typeof(Ektron.Cms.DirectoryData), 0);
        Ektron.Cms.Community.CommunityGroup communityGroup;
        communityGroup = new Ektron.Cms.Community.CommunityGroup(m_refContentApi.RequestInformationRef);
        List<Ektron.Cms.UserData> groupAdmins = new List<UserData>();
        
        cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        groupAdmins = communityGroup.GetAllCommunityGroupAdmins(this.m_iID);

        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 && !IsUserGroupAdmin(groupAdmins))
        {
            return;
        }
        panel3.Visible = true;
        SetLabels();
        m_aCategories = this.m_refContentApi.EkContentRef.GetAllAssignedDirectory(this.m_iID, Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Group);
        lbl_id.Text = cgGroup.GroupId.ToString();
        lbl_id.ToolTip = lbl_id.Text;
        SetTitleBarToMessage("lbl view cgroup");
        PopulateData(cgGroup,groupAdmins, m_aCategories);
        TD_personalTags.InnerHtml = GetGroupTags();
        bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || IsUserGroupAdmin(groupAdmins));
        // buttons
		base.AddBackButton("groups.aspx"); 
		if (this.bAccess == true)
        {
            base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("../communitygroupaddedit.aspx?action=addeditgroup&LangType=" + this.ContentLanguage + "&id=" + cgGroup.GroupId.ToString()), "alt edit community group", "lbl edit community group", "", StyleHelper.EditButtonCssClass, true);
            // MyBase.AddButtonwithMessages(AppImgPath & "menu/folders.gif", "workspace.aspx?groupid=" & Me.m_iID & "&LangType=" & ContentLanguage, "alt view group directory", "btn view group directory", "")
            base.AddButtonwithMessages(m_refContentApi.AppPath + "images/ui/icons/usersMemberGroups.png", (string)("groupmembers.aspx?action=viewallusers&LangType=" + ContentLanguage + "&id=" + this.m_iID), "alt view cgroup members", "btn view cgroup members", "", StyleHelper.ViewGroupMembersButtonCssClass);
            base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string)("groups.aspx?action=delete&id=" + cgGroup.GroupId.ToString()), "alt del community group", "lbl del community group", " onclick=\"javascript:return confirm(\'" + GetMessage("js confirm del community group") + "\');\" ", StyleHelper.DeleteButtonCssClass);
        }
        SetAlias(this.m_iID);
        AddHelpButton("viewcommunitygroup");
    }

    protected void EditGroup()
    {
        BuildJS();
        SetLabels();
        panel3.Visible = true;
        if (this.m_iID > 0)
        {
            cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
            lbl_id.Text = cgGroup.GroupId.ToString();
            lbl_id.ToolTip = lbl_id.Text;
            SetTitleBarToMessage("lbl edit cgroup");
			AddBackButton("groups.aspx?action=viewgroup&id=" + cgGroup.GroupId.ToString() + "");
            // PopulateData(cgGroup)
            base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "lbl alt save", "btn save", " onclick=\"javascript: SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
            // AddBackButton("groups.aspx?action=viewgroup&id=" & cgGroup.GroupId.ToString() & "&LangType=" & cgGroup.GroupLanguage & "")
            AddHelpButton("editcommunitygroup");
            SetAlias(this.m_iID);
        }
        else
        {
            this.PublicJoinYes_RB.Checked = true;
            tr_ID.Visible = false;
            this.cmd_browse.Visible = false;
            SetTitleBarToMessage("lbl add cgroup");
			AddBackButton("groups.aspx");
			base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "lbl alt save", "btn save", " onclick=\"javascript: SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
            AddHelpButton("addcommunitygroup");
        }

    }

    #region Process

    protected void Process_DeleteGroup()
    {
        cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || IsUserGroupAdmin(cgGroup.Admins));
        if (bAccess)
        {
            this.m_refCommunityGroupApi.DeleteCommunityGroupByID(this.m_iID);

            Response.Redirect("groups.aspx", false);
        }
        else
        {
            throw (new Exception(GetMessage("err no perm del cgroup")));
        }
    }

    protected void Process_EditGroup()
    {
        if (m_iID > 0)
        {
            cgGroup = this.m_refCommunityGroupApi.GetCommunityGroupByID(this.m_iID);
        }
        else
        {
            cgGroup = new CommunityGroupData();
        }
        if (this.m_iID > 0)
        {
            bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || IsUserGroupAdmin(cgGroup.Admins));
        }
        else
        {
            bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate));
        }
        if (bAccess)
        {
            cgGroup.GroupName = (string)this.GroupName_TB.Text;
            cgGroup.GroupShortDescription = (string)this.ShortDescription_TB.Text;
            cgGroup.GroupLongDescription = (string)this.Description_TB.Text;
            if (this.PublicJoinHidden_RB.Checked)
            {
                cgGroup.GroupHidden = true;
                cgGroup.GroupEnroll = false;
            }
            else
            {
                cgGroup.GroupEnroll = System.Convert.ToBoolean(this.PublicJoinYes_RB.Checked);
            }
            cgGroup.GroupLocation = (string)this.Location_TB.Text;
            cgGroup.GroupEnableDistributeToSite = System.Convert.ToBoolean(this.EnableDistributeToSite_CB.Checked);
            cgGroup.AllowMembersToManageFolders = System.Convert.ToBoolean(this.AllowMembersToManageFolders_CB.Checked);

            if (m_iID > 0)
            {
                m_refCommunityGroupApi.UpdateCommunityGroup(cgGroup);
                Response.Redirect("groups.aspx", false);
            }
            else
            {
                m_iID = m_refCommunityGroupApi.AddCommunityGroup(cgGroup);
                if (m_iID > 0)
                {
                    Response.Redirect("groups.aspx", false);
                }
                else
                {
                    EditGroup();
                    errmsg.InnerHtml = "Error occured while adding this group.  verify the group name is unique and try again, for more details check eventviewer.";
                    errmsg.Attributes.Add("class", "exception");
                    GroupName_TB.Attributes.Add("onkeypress", "ClearErr();");
                    GroupName_TB.Focus();
                }
            }
        }
        else
        {
            throw (new Exception(GetMessage("err no perm add cgroup")));
        }
    }

    #endregion

    #region Helper Functions

    protected void CheckAccess()
    {
        if (this.m_refContentApi.IsLoggedIn)
        {
            if (this.m_iID > 0 && this.m_sPageAction == "delete")
            {
                Ektron.Cms.Common.EkEnumeration.GroupMemberStatus mMemberStatus;
                mMemberStatus = this.m_refCommunityGroupApi.GetGroupMemberStatus(this.m_iID, this.m_refContentApi.UserId);
                bAccess = System.Convert.ToBoolean(this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Leader);
            }
            else // if logged in, can see this
            {
                bAccess = true;
            }
        }
    }

    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("function SubmitForm() {" + Environment.NewLine);
        sbJS.Append("   var groupName = document.getElementById(\'GroupName_TB\').value;").Append(Environment.NewLine);
        sbJS.Append("   if (groupName == \'\')").Append(Environment.NewLine);
        sbJS.Append("   {alert(\'" + GetMessage("lbl please enter group name") + "\');").Append(Environment.NewLine);
        sbJS.Append("   return false;}").Append(Environment.NewLine);
        sbJS.Append("else{ " + Environment.NewLine);
        sbJS.Append("   if (!CheckGroupForillegalChar()) {" + Environment.NewLine);
        sbJS.Append("   		return false;" + Environment.NewLine);
        sbJS.Append("   } else { document.forms[0].submit(); }" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function ExecSearch() {" + Environment.NewLine);
        sbJS.Append("   var sTerm = $ektron(\'#txtSearch\').getInputLabelValue();" + Environment.NewLine);
        sbJS.Append("   document.getElementById(\'hdn_search\').value = true;" + Environment.NewLine);
        sbJS.Append("   $ektron(\'#txtSearch\').clearInputLabel();" + Environment.NewLine);
        sbJS.Append("	document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("function resetPostback() {" + Environment.NewLine);
        sbJS.Append("   document.forms[0].isPostData.value = \"\"; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckGroupForillegalChar() {" + Environment.NewLine);
        sbJS.Append("   var val = document.forms[0]." + Strings.Replace((string)this.GroupName_TB.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbJS.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbJS.Append("   {" + Environment.NewLine);
        sbJS.Append("       alert(\"" + string.Format(GetMessage("lbl group name disallowed chars"), "(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')") + "\");" + Environment.NewLine);
        sbJS.Append("       return false;" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("   return true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("  			function LoadLanguage(FormName){ ").Append(Environment.NewLine);
        sbJS.Append("  				var num=document.forms[FormName].selLang.selectedIndex; ").Append(Environment.NewLine);
        sbJS.Append("  				window.location.href=\"groups.aspx?action=viewallgroups\"+\"&LangType=\"+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine);
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine);
        sbJS.Append("  				return false; ").Append(Environment.NewLine);
        sbJS.Append("  			} ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text = sbJS.ToString();
    }

    protected void SetLabels()
    {
        this.ltr_groupname.Text = GetMessage("lbl community group name");
        this.ltr_groupid.Text = GetMessage("generic id");
        this.ltr_admin.Text = GetMessage("lbl administrator");
        this.ltr_groupjoin.Text = GetMessage("lbl enrollment");
        this.ltr_groupavatar.Text = GetMessage("lbl group image");
        this.ltr_grouplocation.Text = GetMessage("generic location");
        this.ltr_groupsdesc.Text = GetMessage("lbl short desc");
        this.ltr_groupdesc.Text = GetMessage("generic description");
        this.ltr_enabledistribute.Text = GetMessage("lbl enable distribute");
        this.ltr_MsgBoardModeration.Text = GetMessage("lbl msgboardmoderation");
        this.ltrlEnableDocumentNotifications.Text = GetMessage("lbl Email Notifications");
        this.Literal1.Text = GetMessage("lbl Enable Group Emails");
        this.ltr_Emaildesc.Text = GetMessage("lbl Group Email");
        this.cmd_browse.Text = GetMessage("btn browse");
        PublicJoinYes_RB.Text = GetMessage("lbl enrollment open");
        PublicJoinNo_RB.Text = GetMessage("lbl enrollment restricted");
        PublicJoinHidden_RB.Text = GetMessage("lbl enrollment hidden");
        this.ltr_AllowMembersToManageFolders.Text = GetMessage("lbl allow member to manage folders");
        this.ltr_groupfeatures.Text = GetMessage("lbl features") + ":";
        FeaturesCalendar_CB.Text = GetMessage("lbl enable group calendar");
        FeaturesForum_CB.Text = GetMessage("lbl enable group forum");
        FeaturesTodo_CB.Text = GetMessage("lbl enable group todo");
    }
    private MailServerData GetNotificationEmailServer()
    {
        IMailServer emailServerApi = ObjectFactory.GetMailServer();
        MailServerData mailServerData = new MailServerData();

        Criteria<MailServerProperty> criteria = new Criteria<MailServerProperty>();
        criteria.AddFilter(MailServerProperty.Type, CriteriaFilterOperator.EqualTo, MailServerType.CommunityEmailNotification);

        List<MailServerData> servers = emailServerApi.GetList(criteria);

        if (servers.Count > 0)
        {
            mailServerData = servers[0];
        }
        return mailServerData;
    }
    protected void PopulateData(CommunityGroupData cGrp, List<UserData> groupAdmins, DirectoryData[] aCategories)
    {
        int groupAdminCount = 0;
        this.GroupName_TB.Text = cGrp.GroupName;
        this.ShortDescription_TB.Text = cGrp.GroupShortDescription;
        this.Description_TB.Text = cGrp.GroupLongDescription;
        if (cGrp.GroupHidden)
        {
            this.PublicJoinHidden_RB.Checked = true;
        }
        else
        {
            this.PublicJoinYes_RB.Checked = cGrp.GroupEnroll;
            this.PublicJoinNo_RB.Checked = !(cGrp.GroupEnroll);
        }
        this.Location_TB.Text = cGrp.GroupLocation;
        if (groupAdmins.Count == 0)
        {
            ltr_admin_name.Text = cGrp.GroupAdmin.DisplayName.ToString();
        }
        else
        {
            int order = 0;
            for (groupAdminCount = 0; groupAdminCount < groupAdmins.Count; groupAdminCount++)
            {
                if (order != 0)
                    this.ltr_admin_name.Text += ", ";
                if (groupAdmins[groupAdminCount].DisplayName != "")
                    this.ltr_admin_name.Text += groupAdmins[groupAdminCount].DisplayName;
                else
                    this.ltr_admin_name.Text += groupAdmins[groupAdminCount].FirstName;
                order += 1;
            }
        }

//this.ltr_admin_name.Text=        this.ltr_admin_name.Text.ToString().Remove(this.ltr_admin_name.Text.ToString().LastIndexOf(","), 1);
        this.GroupAvatar_TB.Text = cGrp.GroupImage;
        this.cmd_browse.Attributes.Add("onclick", "javascript:return false;");
        this.cmd_browse.Visible = false;
        this.EnableDistributeToSite_CB.Checked = cGrp.GroupEnableDistributeToSite;
        this.AllowMembersToManageFolders_CB.Checked = cGrp.AllowMembersToManageFolders;
        
        this.chkEnableDocumentNotifications.Checked = cGrp.EnableDocumentsInNotifications;
        this.chkEnableEmail.Checked = cGrp.EnableGroupEmail;
        this.lblEmailAccountValue.Text = cGrp.EmailAccountName;
        this.lblEmailAddressValue.Text = cGrp.EmailAddress;
        MailServerData mailServer = GetNotificationEmailServer();
        this.lblEmailServerPortValue.Text = mailServer.POPPort.ToString();
        this.lblEmailServerValue.Text = mailServer.POPServer;
        this.chkUseSsl.Checked = mailServer.POPSSL;
        ucEktronGroupEmailSetting.Visible = cGrp.EnableGroupEmail;

        this.ltr_avatarpath.Text = "";
        List<string> cat_list = new List<string>();
        string TaxonomyList = string.Empty;
        if ((aCategories != null) && aCategories.Length > 0)
        {
            for (int i = 0; i <= (aCategories.Length - 1); i++)
            {
                cat_list.Add(("<li>" + aCategories[i].DirectoryPath.Remove(0, 1).Replace("\\", " > ") + "</li>"));
            }
            TaxonomyList = string.Join(string.Empty, cat_list.ToArray());
        }
        else
        {
            TaxonomyList = GetMessage("lbl cgroup no cat");
        }
        ltr_cat.Text += TaxonomyList;
        calendardata = _CalendarApi.GetPublicCalendar(Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, cGrp.GroupId);
        if (calendardata != null)
        {
            this.FeaturesCalendar_CB.Checked = true;
        }
        _doesForumExists = m_refCommunityGroupApi.DoesCommunityGroupForumExists(Ektron.Cms.Common.EkEnumeration.WorkSpace.Group, cGrp.GroupId);
        if (_doesForumExists != 0)
        {
            this.FeaturesForum_CB.Checked = true;
        }

        var todoList = GetGroupTodoList();
        if (todoList != null)
        {
            FeaturesTodo_CB.Enabled = false;
            FeaturesTodo_CB.Checked = true;
        }

        if (this.m_sPageAction == "viewgroup")
        {
            this.GroupName_TB.Enabled = false;
            this.ShortDescription_TB.Enabled = false;
            this.Description_TB.Enabled = false;
            this.PublicJoinYes_RB.Enabled = false;
            this.PublicJoinNo_RB.Enabled = false;
            this.PublicJoinHidden_RB.Enabled  = false;
            this.Location_TB.Enabled = false;
            this.GroupAvatar_TB.Enabled = false;
            this.EnableDistributeToSite_CB.Enabled = false;
            this.AllowMembersToManageFolders_CB.Enabled = false;
            this.FeaturesCalendar_CB.Enabled = false;
            this.FeaturesForum_CB.Enabled = false;
            this.FeaturesTodo_CB.Enabled = false;
        }
        else if (this.m_sPageAction == "addeditgroup")
        {
            if (this.m_refContentApi.IsAdmin() == false)
            {
                this.AllowMembersToManageFolders_CB.Enabled = false;
            }
        }
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
            TTotalPages.Text = m_intTotalPages.ToString();
            TTotalPages.ToolTip = TTotalPages.Text;
            TCurrentPage.Text = m_intCurrentPage.ToString();
            TCurrentPage.ToolTip = TCurrentPage.Text;
            TPreviousPage.Enabled = true;
            TFirstPage.Enabled = true;
            TNextPage.Enabled = true;
            TLastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                TPreviousPage.Enabled = false;
                TFirstPage.Enabled = false;
            }
            else if (m_intCurrentPage == m_intTotalPages)
            {
                TNextPage.Enabled = false;
                TLastPage.Enabled = false;
            }
        }
    }
    private void VisiblePageControls(bool flag)
    {
        TTotalPages.Visible = flag;
        TCurrentPage.Visible = flag;
        TPreviousPage.Visible = flag;
        TNextPage.Visible = flag;
        TLastPage.Visible = flag;
        TFirstPage.Visible = flag;
        TPageLabel.Visible = flag;
        TOfLabel.Visible = flag;
    }
    protected void TNavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = int.Parse((string)TTotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)TCurrentPage.Text) + 1);
                break;
            case "Prev":
                m_intCurrentPage = System.Convert.ToInt32(int.Parse((string)TCurrentPage.Text) - 1);
                break;
        }
        ViewAllGroups();
        isPostData.Value = "true";
    }
    #endregion

    private void SetAlias(long groupId)
    {
        Ektron.Cms.API.UrlAliasing.UrlAliasCommunity _communityAlias = new Ektron.Cms.API.UrlAliasing.UrlAliasCommunity();
        System.Collections.Generic.List<Ektron.Cms.Common.UrlAliasCommunityData> aliasList;

        aliasList = _communityAlias.GetListGroup(groupId);
        if (aliasList.Count > 0)
        {
            foreach (Ektron.Cms.Common.UrlAliasCommunityData item in aliasList)
            {
                groupAliasList += "<a href= " + this.m_refContentApi.SitePath + item.AliasName + " target=_blank>" + this.m_refContentApi.SitePath + item.AliasName + "</a>";
                groupAliasList += "<br/>";
            }
        }
        else
        {
            phAliasTab.Visible = false;
            phAliasFrame.Visible = false;
        }
    }

    protected TodoListData GetGroupTodoList()
    {

        Ektron.Cms.Framework.ToDo.TodoList todoListApi = new Ektron.Cms.Framework.ToDo.TodoList();
        TodoListCriteria criteria = new TodoListCriteria();

        criteria.AddFilter(TodoListProperty.ObjectType, CriteriaFilterOperator.EqualTo, EkEnumeration.CMSObjectTypes.CommunityGroup);
        criteria.AddFilter(TodoListProperty.ObjectId, CriteriaFilterOperator.EqualTo, this.m_iID);

        List<TodoListData> list = todoListApi.GetList(criteria);

        if (list.Count > 0)
        {
            return list[0];
        }
        else
        {
            return null;
        }
    }
    #region Group Tags
    public string GetGroupTags()
    {
        string returnValue;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        TagData[] tdaGroup;
        Hashtable htTagsAssignedToUser;
        UserAPI m_refUserApi = new UserAPI();

        try
        {
            htTagsAssignedToUser = new Hashtable();
            result.Append("<fieldset>");
            result.Append("<legend>" + m_refMsg.GetMessage("lbl group tags") + "</legend>");
            result.Append("<div style=\"overflow: auto; height: 80px;\">");

            if (this.m_iID > 0)
            {
                tdaGroup = m_refTagsApi.GetTagsForObject(this.m_iID, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup, -1);
                if (tdaGroup.Length > 0)
                {
                    LocalizationAPI localizationApi = new LocalizationAPI();
                    for (int i = 0; i <= (tdaGroup.Length - 1); i++)
                    {
                        result.Append("<input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\">&nbsp;<img src=\'" + localizationApi.GetFlagUrlByLanguageID(tdaGroup[i].LanguageId) + "\' border=\"0\" />&nbsp;" + tdaGroup[i].Text + "<br>");
                    }
                }
            }
            result.Append("</div>");
            result.Append("</fieldset>");

        }
        catch (Exception)
        {
        }
        finally
        {
            returnValue = result.ToString();
            tdaGroup = null;
            htTagsAssignedToUser = null;
        }
        return returnValue;
    }
    #endregion
}
