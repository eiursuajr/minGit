using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Workarea;
using Microsoft.VisualBasic;

public partial class threadeddisc_userranks : workareabase
{
    protected EkContent cContent;
    protected bool bIsAdmin;
    protected DiscussionBoard[] aAllBoards;
    protected UserRank urUserRank;
    protected UserRank[] aUserRank;
    protected UserData[] aUserData;
    protected UserData uUser = new UserData();
    protected long m_iBoardID = 0;
    protected bool bfromboard = false;
    protected long m_iUserid = 0;
    protected long m_iRankId = 0;
    protected string AppPath = "";

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            cContent = m_refContentApi.EkContentRef;
            bIsAdmin = m_refContentApi.IsAdmin();
            aAllBoards = (DiscussionBoard[])Array.CreateInstance(typeof(DiscussionBoard), 0);
            aUserRank = (Ektron.Cms.UserRank[])Array.CreateInstance(typeof(Ektron.Cms.UserRank), 0);
            aUserData = (Ektron.Cms.UserData[])Array.CreateInstance(typeof(Ektron.Cms.UserData), 0);
            base.Page_Load(sender, e);
            Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();

            if (Request.QueryString["boardid"] != "")
            {
                m_iBoardID = Convert.ToInt64(Request.QueryString["boardid"]);
            }

            if (Convert.ToBoolean(api.RequestInformationRef.IsMembershipUser) || api.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(api.SitePath + "login.aspx", true);
                return;
            }
            else if (!m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_iBoardID, api.RequestInformationRef.UserId, false) && !m_refContentApi.IsAdmin())
            {
                Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"));
                return;
            }

            RegisterResources();

            AppPath = m_refContentApi.AppPath;

            base.SetTitleBarToMessage("lbl user ranks");
            if (!string.IsNullOrEmpty(Request.QueryString["fromboard"]))
            {
                bfromboard = true;
            }

            if (Request.QueryString["uid"] != "")
            {
                m_iUserid = Convert.ToInt64(Request.QueryString["uid"]);
            }
            if (Request.QueryString["rankid"] != "")
            {
                m_iRankId = Convert.ToInt64(Request.QueryString["rankid"]);
            }

            SetLabels();

            switch (this.m_sPageAction)
            {
                case "edit":
                    if (Page.IsPostBack)
                    {
                        Process_AddEdit();
                    }
                    else
                    {
                        AddEdit();
                    }
                    break;
                case "view":
                    View();
                    break;
                case "delete":
                    Delete();
                    break;
                case "viewuser":
                    if (Page.IsPostBack)
                    {
                        Process_ViewUser();
                    }
                    else
                    {
                        ViewUser();
                    }
                    break;
                default: //display
                    Display();
                    break;
            }

            GenerateJS();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #region Display
    public void Display()
    {
        Display("", 0);
    }
    public void Display(string AsType, long rankid)
    {
        try
        {
            divAE.Visible = false;
            if (m_iBoardID > 0)
            {
                aUserRank = cContent.SelectUserRankByBoard(this.m_iBoardID);
            }
            else
            {
                aUserRank = cContent.SelectUserRank(0);
            }
            dgUserRank.DataSource = CreateDataView(aUserRank, AsType, rankid);
            dgUserRank.DataBind();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private DataView CreateDataView(UserRank[] aUR, string AsType, long rankid)
    {
        // for userrank
        DataView dv = new DataView();
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        if (AsType == "change")
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "Blank";
            colBound.HeaderText = "&nbsp;<input type=\"hidden\" name=\"ekuserrankold\" id=\"ekuserrankold\" value=\"" + rankid.ToString() + "\"/>";
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            dgUserRank.Columns.Add(colBound);
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Name";
        colBound.HeaderText = base.GetMessage("generic name");
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "IsStart";
        colBound.HeaderText = base.GetMessage("lbl isstart");
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "IsLadder";
        colBound.HeaderText = base.GetMessage("lbl isladder");
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = base.GetMessage("generic id");
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Posts";
        colBound.HeaderText = base.GetMessage("lbl num posts");
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dgUserRank.Columns.Add(colBound);

        if (m_iBoardID == 0)
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "AppliesTo";
            colBound.HeaderText = base.GetMessage("lbl applies to");
            dgUserRank.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;

        if (AsType == "change")
        {
            dt.Columns.Add(new DataColumn("Blank", typeof(string)));
        }
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("IsStart", typeof(string)));
        dt.Columns.Add(new DataColumn("IsLadder", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("Posts", typeof(string)));
        dt.Columns.Add(new DataColumn("AppliesTo", typeof(string)));
        if (AsType == "change")
        {
            bool bHasBeenSelected = false;
            for (int i = 0; i <= (aUR.Length - 1); i++)
            {
                dr = dt.NewRow();
                if (aUR[i].ID == rankid || (i == (aUR.Length - 1) && bHasBeenSelected == false))
                {
                    dr[0] = "<input type=\"radio\" name=\"ekuserrank\" id=\"ekuserrank\" value=\"" + aUR[i].ID.ToString() + "\" checked=\"true\" />";
                    bHasBeenSelected = true;
                }
                else
                {
                    dr[0] = "<input type=\"radio\" name=\"ekuserrank\" id=\"ekuserrank\" value=\"" + aUR[i].ID.ToString() + "\"/>";
                }
                dr[1] = aUR[i].Name + ((aUR[i].Posts > -1) ? " *" : "");
                dr[2] = "<input type=\"checkbox\" " + ((aUR[i].StartGroup) ? " checked=\"true\" " : "") + " disabled=\"true\" />";

                dr[3] = "<input type=\"checkbox\" " + ((aUR[i].Posts > -1) ? " checked=\"true\" " : "") + " disabled=\"true\" />";

                dr[4] = aUR[i].ID.ToString();
                if (aUR[i].Posts > -1)
                {
                    dr[5] = aUR[i].Posts + " Posts";
                }
                else
                {
                    //dr(3) = ""
                }
                if (m_iBoardID == 0)
                {
                    for (int j = 0; j <= (aUR[i].AppliesTo.Length - 1); j++)
                    {
                        dr[6] += "<img valign=\'absbottom\' src=\'" + AppPath + "images/UI/Icons/users.png" + "\' />&nbsp;" + aUR[i].AppliesTo[j].Name;
                        if (j < aUR[i].AppliesTo.Length)
                        {
                            dr[6] += "<br/>";
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
        }
        else
        {
            for (int i = 0; i <= (aUR.Length - 1); i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"userranks.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aUR[i].ID.ToString() + "\">" + aUR[i].Name + "</a>";
                dr[1] = "<input type=\"checkbox\" " + ((aUR[i].StartGroup) ? " checked=\"true\" " : "") + " disabled=\"false\" />";
                dr[2] = "<input type=\"checkbox\" " + ((aUR[i].Posts > -1) ? " checked=\"true\" " : "") + " disabled=\"false\" />";
                dr[3] = "<a href=\"userranks.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aUR[i].ID.ToString() + "\">" + aUR[i].ID.ToString() + "</a>";
                if (aUR[i].Posts > -1)
                {
                    dr[4] = "<a href=\"userranks.aspx?action=view&boardid=" + this.m_iBoardID.ToString() + "&id=" + aUR[i].ID.ToString() + "\">" + aUR[i].Posts + ((aUR[i].Posts > 1) ? (" " + GetMessage("lbl posts")) : (" " + GetMessage("lbl post"))) + "</a>";
                }
                else
                {
                    //dr(3) = ""
                }
                if (m_iBoardID == 0)
                {
                    for (int j = 0; j <= (aUR[i].AppliesTo.Length - 1); j++)
                    {
                        dr[5] += "<img valign=\'absbottom\' src=\'" + AppPath + "images/UI/Icons/users.png" + "\' />&nbsp;<a href=\"../content.aspx?action=ViewContentByCategory&id=" + aUR[i].AppliesTo[j].Id.ToString() + "\">" + aUR[i].AppliesTo[j].Name + "</a>";
                        if (j < aUR[i].AppliesTo.Length)
                        {
                            dr[5] += "<br/>";
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
        }
        dv = new DataView(dt);
        return dv;
    }

    private DataView CreateDataView(UserData[] aUD)
    {
        // for userdata
        DataView dv = new DataView();
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = base.GetMessage("generic username");
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DISPLAYNAME";
        colBound.HeaderText = base.GetMessage("display name label");
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = base.GetMessage("generic id");
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRST";
        colBound.HeaderText = base.GetMessage("generic firstname");
        dgUserRank.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LAST";
        colBound.HeaderText = base.GetMessage("generic lastname");
        dgUserRank.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DISPLAYNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRST", typeof(string)));
        dt.Columns.Add(new DataColumn("LAST", typeof(string)));

        for (int i = 0; i <= (aUD.Length - 1); i++)
        {
            dr = dt.NewRow();
            if (m_iBoardID > 0)
            {
                dr[0] = "<a href=\"?action=viewuser&uid=" + aUD[i].Id.ToString() + "&boardid=" + m_iBoardID.ToString() + "&rankid=" + this.m_iID.ToString() + "\">" + EkFunctions.HtmlEncode(aUD[i].Username) + "</a>";
            }
            else
            {
                dr[0] = EkFunctions.HtmlEncode(aUD[i].Username);
            }
            dr[1] = EkFunctions.HtmlEncode(aUD[i].DisplayName);
            dr[2] = aUD[i].Id.ToString();
            dr[3] = EkFunctions.HtmlEncode(aUD[i].FirstName);
            dr[4] = EkFunctions.HtmlEncode(aUD[i].LastName);
            dt.Rows.Add(dr);
        }
        dv = new DataView(dt);
        return dv;
    }
    #endregion

    #region View
    public void View()
    {
        pnlList.CssClass = "ektronPageInfo";
        pnlGrid.CssClass = "ektronBorder";
        aUserData = cContent.SelectUsersByRank(this.m_iID);
        aUserRank = cContent.SelectUserRank(this.m_iID);
        aAllBoards = cContent.GetAllBoards();
        txt_name.Text = HttpContext.Current.Server.HtmlDecode(aUserRank[0].Name);
        txt_name.Enabled = false;
        if (aUserRank[0].Posts > -1)
        {
            txt_posts.Text = aUserRank[0].Posts.ToString();
        }
        else
        {
            txt_posts.Text = "";
        }
        txt_icon_image.Text = aUserRank[0].IconImage;
        txt_posts.Enabled = false;
        txt_icon_image.Enabled = false;
        chk_isstart.Checked = aUserRank[0].StartGroup;
        chk_isladder.Checked = aUserRank[0].StartGroup || aUserRank[0].Posts > -1;
        chk_isladder.Enabled = false;
        chk_isstart.Enabled = false;
        drp_boards.Enabled = false;
        if (aUserRank[0].IconImage != "")
        {
            ltr_preview.Text = "<a href=\"#\" onclick=\"javascript: window.open(\'" + Strings.Replace((string)(aUserRank[0].IconImage.Replace("\\", "\\\\")), "\'", "\\\'", 1, -1, 0) + "\',null,\'\');\" ><img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/preview.png\" class=\"ektronClickableImage\" title=\"" + base.GetMessage("generic preview title") + "\" /></a>";
        }
        if (m_iBoardID > 0)
        {
            tr_applies.Visible = false;
        }
        else
        {
            for (int i = 0; i <= (aAllBoards.Length - 1); i++)
            {
                bool bChecked = false;
                for (int j = 0; j <= (aUserRank[0].AppliesTo.Length - 1); j++)
                {
                    if (aUserRank[0].AppliesTo[j].Id == aAllBoards[i].Id)
                    {
                        bChecked = true;
                        break;
                    }
                }
                drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                if (bChecked == true)
                {
                    drp_boards.Items[i].Selected = true;
                }
            }
        }
        dgUserRank.DataSource = CreateDataView(aUserData);
        dgUserRank.DataBind();
    }
    #endregion

    #region Delete
    public void Delete()
    {
        try
        {
            if (this.m_iID > 0)
            {
                cContent.DeleteUserRank(this.m_iID);
                Response.Redirect((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString()), false);
            }
            else
            {
                throw (new Exception(base.GetMessage("err no del userrank")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion

    #region AddEdit
    public void AddEdit()
    {
        pnlList.Visible = false;
        txt_name.Enabled = true;
        txt_posts.Enabled = true;
        chk_isstart.Enabled = true;
        chk_isladder.Enabled = true;
        aAllBoards = cContent.GetAllBoards();
        ltr_upload.Text = "</asp:Literal>&nbsp;<a href=\"../Upload.aspx?action=userranks&addedit=true&returntarget=txt_icon_image&EkTB_iframe=true&height=300&width=400&modal=true\" title=\"" + GetMessage("upload txt") + "\" class=\"ek_thickbox button buttonInline greenHover buttonUpload btnUpload\" title=\"" + GetMessage("upload txt") + "\">" + GetMessage("upload txt") + "</a>";
        if (this.m_iID > 0) //edit
        {
            drp_boards.Enabled = false;
            aUserRank = cContent.SelectUserRank(this.m_iID);
            txt_name.Text = HttpContext.Current.Server.HtmlDecode(aUserRank[0].Name);
            if (aUserRank[0].Posts > -1)
            {
                txt_posts.Text = aUserRank[0].Posts.ToString();
            }
            else
            {
                txt_posts.Text = "";
            }
            txt_icon_image.Text = aUserRank[0].IconImage;
            chk_isstart.Checked = aUserRank[0].StartGroup;
            chk_isladder.Checked = aUserRank[0].StartGroup || aUserRank[0].Posts > -1;
            if (!chk_isladder.Checked)
            {
                ltr_init_js.Text = "<script type=\"text/javascript\" >document.getElementById(\'" + chk_isstart.UniqueID + "\').disabled = true;" + Environment.NewLine + "</script>";
                txt_posts.Enabled = false;
            }
            if (chk_isstart.Checked)
            {
                ltr_init_js.Text = "<script  type=\"text/javascript\" >document.getElementById(\'" + txt_posts.UniqueID + "\').disabled = true;" + Environment.NewLine + "</script>";
                txt_posts.Enabled = false;
            }
            if (m_iBoardID > 0)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    bool bChecked = false;
                    for (int j = 0; j <= (aUserRank[0].AppliesTo.Length - 1); j++)
                    {
                        if (aUserRank[0].AppliesTo[j].Id == aAllBoards[i].Id)
                        {
                            bChecked = true;
                            break;
                        }
                    }
                    drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                    if (bChecked == true)
                    {
                        drp_boards.Items[i].Selected = true;
                    }
                }
            }
        }
        else
        {
            chk_isladder.Checked = true;
            if (m_iBoardID > 0)
            {
                tr_applies.Visible = false;
            }
            else
            {
                for (int i = 0; i <= (aAllBoards.Length - 1); i++)
                {
                    drp_boards.Items.Add(new ListItem(aAllBoards[i].Name + " - " + GetPath(aAllBoards[i].Path), aAllBoards[i].Id.ToString()));
                }
            }
        }
    }

    public void Process_AddEdit()
    {
        if (this.m_iID > 0)
        {
            aUserRank = cContent.SelectUserRank(this.m_iID);
            urUserRank = aUserRank[0];
        }
        else
        {
            urUserRank = new UserRank();
        }
        urUserRank.Name = EkFunctions.HtmlEncode(Request.Form[txt_name.UniqueID]).Trim();
        urUserRank.IconImage = Strings.Trim(Request.Form[txt_icon_image.UniqueID]);
        urUserRank.StartGroup = System.Convert.ToBoolean(chk_isstart.Checked);
        if (chk_isladder.Checked) // IsNumeric(Request.Form(txt_posts.UniqueID)) Then
        {
            if (urUserRank.StartGroup == true)
            {
                urUserRank.Posts = 1;
            }
            else
            {
                urUserRank.Posts = Convert.ToInt32(Request.Form[txt_posts.UniqueID]);
            }
        }
        else
        {
            urUserRank.Posts = -1;
        }
        urUserRank.AppliesTo = (FolderData[])Array.CreateInstance(typeof(FolderData), 1);
        urUserRank.AppliesTo[0] = new FolderData();
        if (m_iBoardID > 0)
        {
            urUserRank.AppliesTo[0].Id = this.m_iBoardID;
        }
        else
        {
            urUserRank.AppliesTo[0].Id = Convert.ToInt64(drp_boards.SelectedValue);
        }
        urUserRank = cContent.AddEditUserRank(urUserRank);
        if (this.m_iID > 0) // edit
        {
            Response.Redirect((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString()), false);
        }
        else // add
        {
            Response.Redirect((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString()), false);
        }
    }
    #endregion

    #region ViewUser
    public void ViewUser()
    {
        pnlList.CssClass = "ektronPageInfo";
        pnlGrid.CssClass = "ektronBorder";
        divAE.Visible = false;
        divViewUser.Visible = true;
        uUser = this.cContent.SelectUserWithRank(this.m_iUserid, this.m_iBoardID);
        ltr_vud_username.Text = uUser.Username;
        ltr_vud_displayname.Text = uUser.DisplayName;
        ltr_vud_firstname.Text = uUser.FirstName;
        ltr_vud_lastname.Text = uUser.LastName;
        Display("change", uUser.Rank.ID);
    }

    public void Process_ViewUser()
    {
        long iold = 0;
        long inew = 0;

        iold = Convert.ToInt64(Request.Form["ekuserrankold"]);
        inew = Convert.ToInt64(Request.Form["ekuserrank"]);

        cContent.UpdateUserRank(iold, inew, m_iUserid);

        Response.Redirect((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + inew.ToString()), false);
    }
    #endregion


    #region Helper Functions
    private void SetLabels()
    {
        base.SetTitleBarToMessage("lbl user ranks");
        switch (this.m_sPageAction)
        {
            case "view":
            case "edit":
                if (this.m_sPageAction == "view")
                {
					base.AddBackButton((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString())); 
					base.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", (string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit&id=" + this.m_iID.ToString()), (string)("btn alt edit userrank"), (string)("btn edit"), "", StyleHelper.EditButtonCssClass, true);
                    //MyBase.AddButtonwithMessages(AppImgPath & "folders/twohead.gif", "", "", "", "")
                    base.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", (string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=delete&id=" + this.m_iID.ToString()), (string)("btn alt del userrank"), (string)("btn delete"), " onclick=\"javascript:return confirm(\'" + base.GetMessage("js conf del userrank") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                else if (this.m_sPageAction == "edit")
                {
					if (bfromboard == true)
					{
						base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString()));
					}
					else if (this.m_iID > 0)
					{
						base.AddBackButton((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + this.m_iID.ToString()));
					}
					else
					{
						base.AddBackButton((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString()));
					}
					
					base.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", (string)("#" + this.m_iID.ToString()), (string)("btn alt save userrank"), (string)("btn save"), " onclick=\"return submitform();\" ", StyleHelper.SaveButtonCssClass, true);
                }
                ltr_name.Text = base.GetMessage("generic name");
                ltr_icon_image.Text = base.GetMessage("lbl icon image");
                ltr_posts.Text = base.GetMessage("lbl num posts");
                ltr_isstart.Text = base.GetMessage("lbl isstart");
                ltr_isladder.Text = base.GetMessage("lbl isladder");
                ltr_appliesto.Text = base.GetMessage("lbl applies to");
                break;
            case "viewuser":
                base.SetTitleBarToMessage("lbl change user rank");
                divViewUser.Visible = true;
                ltr_vu_username.Text = base.GetMessage("generic username");
                ltr_vu_displayname.Text = base.GetMessage("display name label");
                ltr_vu_firstname.Text = base.GetMessage("generic firstname");
                ltr_vu_lastname.Text = base.GetMessage("generic lastname");
                ltr_vu_select.Text = base.GetMessage("generic select") + " " + base.GetMessage("lbl user rank");
                base.AddBackButton((string)("userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=view&id=" + m_iRankId.ToString()));
                base.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", (string)("#" + this.m_iID.ToString()), (string)("btn alt save userrank"), (string)("btn save"), " onclick=\"javascript:document.forms[0].submit(); return true;\" ", StyleHelper.SaveButtonCssClass, true);
                break;
            default:
                if (this.m_iBoardID > 0)
                {
                    base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + this.m_iBoardID.ToString()));
                }
				base.AddButtonwithMessages(AppPath + "images/UI/Icons/add.png", "userranks.aspx?boardid=" + this.m_iBoardID.ToString() + "&action=edit", (string)("btn alt add userrank"), (string)("generic add title"), "", StyleHelper.AddButtonCssClass, true);
                break;
        }
        base.AddHelpButton("userranks");
    }

    private void GenerateJS()
    {
        if (this.m_sPageAction == "edit")
        {
            StringBuilder sbJS = new StringBuilder();

            sbJS.Append("<script  type=\"text/javascript\">").Append(Environment.NewLine);
            sbJS.Append("   <!--//--><![CDATA[//><!--").Append(Environment.NewLine);
            sbJS.Append(AJAXcheck(GetResponseString("VerifyUserRank"), "action=existinguserrank&boardid=\' + input[2] + \'&urname=\' + input[0] + \'&urid=" + m_iID.ToString() + "&posts=\' + input[1] + \'&isstart=\' + input[3] + \'")).Append(Environment.NewLine);

            sbJS.Append("function submitform()").Append(Environment.NewLine);
            sbJS.Append("{").Append(Environment.NewLine);
            sbJS.Append("    var bVal = true;").Append(Environment.NewLine);
            sbJS.Append("    var nText = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    var iText = Trim(document.getElementById(\'").Append(txt_icon_image.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    var iNumObj = document.getElementById(\'").Append(txt_posts.UniqueID).Append("\');").Append(Environment.NewLine);
            sbJS.Append("    if ((!(isInteger(iNumObj.value))) || parseInt(iNumObj.value, 10) < 0){alert('" + base.GetMessage("js alert error num posts") + "'); iNumObj.focus(); return false;}").Append(Environment.NewLine);
            sbJS.Append("    var iNum = parseInt(Trim(document.getElementById(\'").Append(txt_posts.UniqueID).Append("\').value),10);").Append(Environment.NewLine);
            sbJS.Append("    if (isNaN(iNum)) {iNum = 0}").Append(Environment.NewLine);
            sbJS.Append("    var bStart = Trim(document.getElementById(\'").Append(chk_isstart.UniqueID).Append("\').checked);").Append(Environment.NewLine);
            if (this.m_iBoardID > 0)
            {
                sbJS.Append("    var iBoardVal = " + this.m_iBoardID.ToString() + ";").Append(Environment.NewLine);
            }
            else // we need to get it from the dropdown
            {
                /// If there is no Board to which the user ranks gets applied to, set its value to -1, so that the value of the selected index is not undefined.
                sbJS.Append("    var appliesToSelectedIndex = document.getElementById(\'").Append(this.drp_boards.UniqueID).Append("\').selectedIndex").AppendLine();
                sbJS.Append("    var iBoardVal='';").AppendLine();
                sbJS.Append("    if(appliesToSelectedIndex == -1)").AppendLine();
                sbJS.Append("    { ").AppendLine();
                sbJS.Append("       iBoardVal = -1; ").AppendLine();
                sbJS.Append("        alert(\'").Append(base.GetMessage("js:alert no discussion board to apply user rank to")).Append("\');").Append(Environment.NewLine);
                sbJS.Append("       return false; ").AppendLine();
                sbJS.Append("    } ").AppendLine();
                sbJS.Append("    else ").AppendLine();
                sbJS.Append("    { ").AppendLine();
                sbJS.Append("       iBoardVal = Trim(document.getElementById(\'").Append(this.drp_boards.UniqueID).Append("\').options[document.getElementById(\'").Append(this.drp_boards.UniqueID).Append("\').selectedIndex].value);").Append(Environment.NewLine);
                sbJS.Append("    } ").AppendLine();
            }
            sbJS.Append("    var atext = new Array(nText, iNum, iBoardVal, bStart); " + Environment.NewLine);
            sbJS.Append("    checkUserRank(atext,\'\'); " + Environment.NewLine);
            sbJS.Append("    return bexists; " + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);

            sbJS.Append("function VerifyUserRank()" + Environment.NewLine);
            sbJS.Append("{").Append(Environment.NewLine);
            sbJS.Append("    var bVal = true;").Append(Environment.NewLine);
            sbJS.Append("    var nText = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    var iText = Trim(document.getElementById(\'").Append(txt_icon_image.UniqueID).Append("\').value);").Append(Environment.NewLine);
            sbJS.Append("    var iNum = parseInt(Trim(document.getElementById(\'").Append(txt_posts.UniqueID).Append("\').value),10);").Append(Environment.NewLine);
            sbJS.Append("    if (isNaN(iNum)) {iNum = 0}").Append(Environment.NewLine);
            sbJS.Append("    var bStart = Trim(document.getElementById(\'").Append(chk_isstart.UniqueID).Append("\').checked);").Append(Environment.NewLine);
            sbJS.Append("    var bLadder = Trim(document.getElementById(\'").Append(chk_isladder.UniqueID).Append("\').checked);").Append(Environment.NewLine);
            sbJS.Append("    if (iText.toLowerCase() == \"http://\")").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("       document.getElementById(\'").Append(txt_icon_image.UniqueID).Append("\').value = \'\';").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    if (nText.length == 0)").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        alert(\'").Append(base.GetMessage("err ruleset need name")).Append("\');").Append(Environment.NewLine);
            sbJS.Append("        bVal = false;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    if (bStart == true)").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        // ignore, cause we handle this on the backend. ").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    else if (bLadder == false || (bLadder == true && bStart == false && (iNum > 1 || Trim(iNum).length == 0))) // must be either > 1 or blank").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        // ignore ").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    else ").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        alert(\'").Append(base.GetMessage("js alert num posts rank")).Append("\');").Append(Environment.NewLine);
            sbJS.Append("        bVal = false;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    if (bVal == false)").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        return false;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("    else").Append(Environment.NewLine);
            sbJS.Append("    {").Append(Environment.NewLine);
            sbJS.Append("        document.forms[0].submit();").Append(Environment.NewLine);
            sbJS.Append("        return true;").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);

            sbJS.Append("function noenter() {" + Environment.NewLine);
            sbJS.Append("    if (window.event && window.event.keyCode == 13) {" + Environment.NewLine);
            sbJS.Append("        return submitform(); " + Environment.NewLine);
            sbJS.Append("    }" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);

            sbJS.Append("function UpdateCheck(chkLadder) {" + Environment.NewLine);
            sbJS.Append("    if (chkLadder.checked) {" + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + chk_isstart.UniqueID + "\').disabled = false;" + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + txt_posts.UniqueID + "\').disabled = false; " + Environment.NewLine);
            sbJS.Append("    } else {" + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + chk_isstart.UniqueID + "\').disabled = true;" + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + chk_isstart.UniqueID + "\').checked = false; " + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + txt_posts.UniqueID + "\').value = \'\'; " + Environment.NewLine);
            sbJS.Append("       document.getElementById(\'" + txt_posts.UniqueID + "\').disabled = true; " + Environment.NewLine);
            sbJS.Append("    }" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);

            sbJS.Append("function updateposts(iVal) {" + Environment.NewLine);
            sbJS.Append("    if (document.getElementById(\'" + chk_isstart.UniqueID + "\').checked == true) {" + Environment.NewLine);
            sbJS.Append("        document.getElementById(\'" + txt_posts.UniqueID + "\').value = iVal; " + Environment.NewLine);
            sbJS.Append("        document.getElementById(\'" + txt_posts.UniqueID + "\').disabled = true; " + Environment.NewLine);
            sbJS.Append("    } else {" + Environment.NewLine);
            sbJS.Append("        document.getElementById(\'" + txt_posts.UniqueID + "\').disabled = false; " + Environment.NewLine);
            sbJS.Append("    }" + Environment.NewLine);
            sbJS.Append("    return true;" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);

            txt_name.Attributes.Add("onkeypress", "return noenter();");
            txt_posts.Attributes.Add("onkeypress", "return noenter();");
            txt_icon_image.Attributes.Add("onkeypress", "return noenter();");
            chk_isstart.Attributes.Add("onclick", "return updateposts(1);");
            chk_isladder.Attributes.Add("onclick", "UpdateCheck(this);");
            sbJS.Append("   //--><!]]>").Append(Environment.NewLine);
            sbJS.Append("</script>").Append(Environment.NewLine);

            LiteralControl ltr_de_css = new LiteralControl(sbJS.ToString());
            if (!(Page.Header == null))
            {
                Page.Header.Controls.Add(ltr_de_css);
            }
        }
    }

    private string AJAXcheck(string sResponse, string sURLQuery)
    {
        base.AJAX.ResponseJS = sResponse;
        base.AJAX.URLQuery = sURLQuery;
        base.AJAX.FunctionName = "checkUserRank";
        return base.AJAX.Render();
    }

    private string GetResponseString(string nextfunction)
    {
        System.Text.StringBuilder sbAEJS = new System.Text.StringBuilder();
        sbAEJS.Append("    if (response == \'1\'){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'A User Rank with this number of posts already exists.\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else if (response == \'2\'){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'This User Rank already exists. Please change the name.\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else if (response == \'3\'){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'A starting User Rank already exists.\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else{").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine);
        sbAEJS.Append("    } ").Append(Environment.NewLine);
        return sbAEJS.ToString();
    }

    //Private Sub Util_CheckAccess()
    //    If m_refContentApi.RequestInformationRef.IsMembershipUser Or m_refContentApi.RequestInformationRef.UserId = 0 Or Not m_refContentApi.IsAdmin() Then
    //        Response.Redirect(m_refContentApi.ApplicationPath & "Login.aspx", True)
    //        Exit Sub
    //    End If
    //End Sub
    protected void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/validation.js", "EktronValidationJS");
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");
    }
    protected string GetPath(string path)
    {
        path = ("/" + path);
        string[] paths = path.Split(new char[] { '/' });

        Array.Resize(ref paths, (paths.Length - 2));
        path = string.Join("/", paths) + "/";

        return path;
    }

    #endregion
}