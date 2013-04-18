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

public partial class MyWorkspace_MyFriends : workareabase
{
    protected DirectoryUserData[] m_aUsers = new List<DirectoryUserData>().ToArray();
    protected DirectoryUserData[] m_aSelectedFriends = new List<DirectoryUserData>().ToArray();
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected SettingsData setting_data;
    protected string m_strKeyWords = "";
    protected int m_PageSize = 50;
    protected string m_strSelectedItem = "-1";
    protected bool m_bAllowAdd = false;

    protected void Page_Load1(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (!String.IsNullOrEmpty(Request.Form["txtSearch"]))
        {
            m_strKeyWords = Request.Form["txtSearch"];
        }
        this.groupID.Value = m_iID.ToString();
        m_PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        try
        {
            if (CheckAccess() == false)
            {
                throw (new Exception(this.GetMessage("err myfriends no access")));
            }
            if (Page.IsPostBack)
            {
                switch (this.m_sPageAction)
                {
                    default: // "viewall"
                        if (Request.Form["isDeleted"] != "" && Request.Form["isDeleted"] == "1")
                        {
                            Process_Remove();
                        }
                        else if (Request.Form["isDeleted"] != "" && Request.Form["isDeleted"] == "2")
                        {
                            Process_UpdateSelected();
                        }
                        break;
                }
            }
            else
            {
                switch (this.m_sPageAction)
                {
                    default: // "viewall"
                        Display_View();
                        break;
                }
            }
            BuildJS();
            SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }

    #region Display

    public void Display_View()
    {
        SiteAPI m_refSiteApi = new SiteAPI();
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId);

        m_aUsers = m_refFriendsApi.GetMyFriends(m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0), ref m_intTotalPages);
        m_aSelectedFriends = m_refFriendsApi.GetSelectedFriendsForUser(this.m_refContentApi.UserId);

        Populate_ViewFriendsGrid(m_aUsers);
    }

    #endregion

    #region Process

    protected void Process_UpdateSelected()
    {
        string[] aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    int iUserID = int.Parse(aUid[i]);
                    if (this.m_refFriendsApi.IsSelectedFriendForUser(iUserID, this.m_refContentApi.UserId))
                    {
                        this.m_refFriendsApi.DeleteSelectedFriendForUser(iUserID, this.m_refContentApi.UserId);
                    }
                    else
                    {
                        this.m_refFriendsApi.AddSelectedFriendForUser(iUserID, this.m_refContentApi.UserId);
                    }
                }
            }
        }
        Response.Redirect("MyFriends.aspx", false);
    }

    protected void Process_Remove()
    {

        string[] aUid = new List<String>().ToArray();
        if (!String.IsNullOrEmpty(Request.Form["req_deleted_users"]))
        {
            aUid = Request.Form["req_deleted_users"].Split(',');
        }
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    m_refFriendsApi.DeleteMyFriend(Convert.ToInt64(aUid[i]));
                }
            }
        }
        Response.Redirect("MyFriends.aspx", false);
    }

    #endregion

    #region Helper Functions

    public bool CheckAccess()
    {
        if (m_refContentApi.UserId > 0 && this.m_refContentApi.MemberType == 0)
        {
            return true;
        }
        else
        {
            return false;
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

    public void SetLabels()
    {
        this.AddButtonwithMessages(this.m_refContentApi.AppPath + "images/ui/icons/save.png", "#", "alt update selected friends", "btn update selected friends", " onclick=\"return CheckUpdateSel();\" ", StyleHelper.SaveButtonCssClass, true);
        this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "btn_delete-nm.gif", "#", "alt remove users from my friends", "btn remove users from my friends", " onclick=\"return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
        this.AddButtonwithMessages(this.m_refContentApi.AppPath + "images/ui/icons/magnifier.png", "FriendSearch.aspx", "generic search", "generic search", "", StyleHelper. SearchButtonCssClass);
        this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "btn_addusergroup-nm.gif", "InviteFriends.aspx", "lbl alt invite new friends", "lbl invite new friends", "", StyleHelper.AddUserGroupButtonCssClass);
        this.AddHelpButton("viewmyfriends");
        this.SetTitleBarToMessage("lbl my friends");
    }

    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("function SubmitForm() {" + Environment.NewLine);
        // TODO
        sbJS.Append("	document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function ExecSearch() {" + Environment.NewLine);
        sbJS.Append("   var sTerm = Trim(document.getElementById(\'txtSearch\').value); " + Environment.NewLine);
        sbJS.Append("   if (sTerm == \'\') {" + Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("err js no search term")).Append("\'); " + Environment.NewLine);
        sbJS.Append("   } else {" + Environment.NewLine);
        sbJS.Append("	    document.getElementById(\'hdn_search\').value = true;" + Environment.NewLine);
        sbJS.Append("	    document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("function resetPostback() {" + Environment.NewLine);
        sbJS.Append("   document.forms[0].isPostData.value = \"\"; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("     function CheckDelete() ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("         var bCheck = false; ").Append(Environment.NewLine);
        sbJS.Append("         for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append(" 	        var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append(" 			if (e.name==\'req_deleted_users\' && e.checked){ ").Append(Environment.NewLine);
        sbJS.Append(" 			    bCheck = true; ").Append(Environment.NewLine);
        sbJS.Append(" 			} ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (bCheck) { if (confirm(\'").Append(GetMessage("js confirm remove friend from my friends")).Append("\')) { document.getElementById(\'isDeleted\').value = \'1\'; document.forms[0].submit(); } else { bCheck = false; } } ").Append(Environment.NewLine);
        sbJS.Append(" 	    else { alert(\'").Append(GetMessage("js err my friends please select user remove")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" 	    return bCheck; ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("     function CheckUpdateSel() ").Append(Environment.NewLine);
        sbJS.Append("     { ").Append(Environment.NewLine);
        sbJS.Append("         var bCheck = false; ").Append(Environment.NewLine);
        sbJS.Append("         for (var i=0;i<document.forms[0].elements.length;i++){ ").Append(Environment.NewLine);
        sbJS.Append(" 	        var e = document.forms[0].elements[i]; ").Append(Environment.NewLine);
        sbJS.Append(" 			if (e.name==\'req_deleted_users\' && e.checked){ ").Append(Environment.NewLine);
        sbJS.Append(" 			    bCheck = true; ").Append(Environment.NewLine);
        sbJS.Append(" 			} ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (bCheck) { if (confirm(\'").Append(GetMessage("js confirm update selected friends")).Append("\')) { document.getElementById(\'isDeleted\').value = \'2\'; document.forms[0].submit(); } else { bCheck = false; } } ").Append(Environment.NewLine);
        sbJS.Append(" 	    else { alert(\'").Append(GetMessage("js err no sel fr update")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" 	    return bCheck; ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);
        ltr_js.Text = sbJS.ToString();
    }

    private void Populate_ViewFriendsGrid(DirectoryUserData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;
        string sAppend = "";

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKL";
        colBound.HeaderText = "<input type=\"checkbox\" onclick=\"checkAll(this);\" name=\"FriendsMasterCB\" value=\"" + "ID" + "\" runat=\"Server\"/>";
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.HeaderStyle.Width = Unit.Percentage(1);
        colBound.Initialize();
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        FriendGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "SEL";
        colBound.HeaderText = this.GetMessage("lbl selected friend");
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.HeaderStyle.Width = Unit.Percentage(1);
        colBound.Initialize();
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        FriendGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COL_AVATAR";
        colBound.HeaderText = this.GetMessage("lbl avatar");
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        FriendGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COL_DISPLAYNAME";
        colBound.HeaderText = this.GetMessage("generic display name");
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.HeaderStyle.Width = Unit.Percentage(30);
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        FriendGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COL_FIRSTNAME";
        colBound.HeaderText = this.GetMessage("generic first name");
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.HeaderStyle.Width = Unit.Percentage(30);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        colBound.ItemStyle.Wrap = false;
        FriendGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COL_LASTNAME";
        colBound.HeaderText = this.GetMessage("generic last name");
        colBound.HeaderStyle.CssClass = "friendsHeader";
        colBound.HeaderStyle.Width = Unit.Percentage(30);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Middle;
        colBound.ItemStyle.Wrap = false;
        FriendGrid.Columns.Add(colBound);

        PageSettings();

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECKL", typeof(string)));
        dt.Columns.Add(new DataColumn("SEL", typeof(string)));
        dt.Columns.Add(new DataColumn("COL_AVATAR", typeof(string)));
        dt.Columns.Add(new DataColumn("COL_DISPLAYNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("COL_FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("COL_LASTNAME", typeof(string)));
        int i = 0;


        if (!(data == null))
        {
            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                sAppend = "";
                if ((setting_data.ADAuthentication == 1) && (data[i].Domain != ""))
                {
                    sAppend = "@" + data[i].Domain;
                }
                dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].Id + "\" >";
                if (this.IsSelectedFriend(System.Convert.ToInt32(data[i].Id)))
                {
                    dr["SEL"] = "<img src=\"" + this.m_refContentApi.AppImgPath + "check20.gif\"/>";
                }
                else
                {
                    dr["SEL"] = "&nbsp;";
                }
                dr["COL_AVATAR"] = "<img class=\"friendsAvatar\" align=\"left\" src=\"" + ((data[i].Avatar != "") ? (AppendSitePathIfNone((string)(data[i].Avatar))) : this.m_refContentApi.AppImgPath + "user.gif") + "\" width=\"32\" height=\"32\"/>";
                dr["COL_DISPLAYNAME"] = data[i].DisplayName;
                dr["COL_FIRSTNAME"] = data[i].FirstName;
                dr["COL_LASTNAME"] = data[i].LastName;
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        FriendGrid.DataSource = dv;
        FriendGrid.DataBind();
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
            TotalPages.Text = (System.Math.Ceiling((double)m_intTotalPages)).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = m_intCurrentPage.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
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

    private string GetSelectedFriendChecked(int userid)
    {
        string sRet = "";
        if (IsSelectedFriend(userid))
        {
            sRet = "checked=\"true\" ";
        }
        return sRet;
    }

    private bool IsSelectedFriend(int userid)
    {
        bool bret = false;

        if ((m_aSelectedFriends != null) && m_aSelectedFriends.Length > 0)
        {
            for (int i = 0; i <= (m_aSelectedFriends.Length - 1); i++)
            {
                if (m_aSelectedFriends[i].Id == userid)
                {
                    bret = true;
                }
            }
        }

        return bret;
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
        Display_View();
    }
    #endregion

    protected string AppendSitePathIfNone(string avatar)
    {
        if ((avatar.Trim().IndexOf(m_refContentApi.SitePath) < 0) && (("/" + avatar).Trim().IndexOf(m_refContentApi.SitePath) < 0))
        {
            avatar = m_refContentApi.SitePath + avatar;
        }
        else if (avatar.Trim().IndexOf("/") != 0)
        {
            avatar = m_refContentApi.SitePath + avatar;
        }
        return avatar;
    }
}
