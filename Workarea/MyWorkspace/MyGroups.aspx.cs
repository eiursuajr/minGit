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

public partial class MyWorkspace_MyGroups : workareabase
{
    protected CommunityGroupData[] m_aGroups = new List<CommunityGroupData>().ToArray();
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
        if (Request.Form["txtSearch"] != "")
        {
            m_strKeyWords = Request.Form["txtSearch"];
        }
        m_PageSize = this.m_refContentApi.RequestInformationRef.PagingSize;
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
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
                        if (Request.Form["isDeleted"] != "")
                        {
                            Process_Remove();
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
        m_aGroups = m_refCommunityGroupApi.GetMyCommunityGroups(m_intCurrentPage, System.Convert.ToInt32(m_PageSize > 0 ? m_PageSize : 0));
        Populate_ViewCommunityGroupsGrid(m_aGroups);
    }

    #endregion

    #region Process

    protected void Process_Remove()
    {
        string[] aUid = new List<string>().ToArray();
        aUid = Strings.Split(Request.Form["req_deleted_users"], ",", -1, 0);
        if ((aUid != null) && aUid.Length > 0)
        {
            for (int i = 0; i <= (aUid.Length - 1); i++)
            {
                if (Information.IsNumeric(aUid[i].Trim()))
                {
                    m_refCommunityGroupApi.RemoveUserFromCommunityGroup(Convert.ToInt64(aUid[i]), this.m_refContentApi.UserId);
                }
            }
        }
        Response.Redirect("MyGroups.aspx", false);
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
        switch (this.m_sPageAction)
        {
            default: // "viewall"
                this.AddButtonwithMessages(this.m_refContentApi.AppImgPath + "btn_delete-nm.gif", "#", "alt remove my cgroup", "btn remove my cgroup", " onclick=\"javascript:return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass, true);
                this.AddHelpButton("viewmycommunitygroups");
                this.SetTitleBarToMessage("lbl my groups");
                break;
        }
    }

    protected void BuildJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("function SubmitForm() {" + Environment.NewLine);
        // TODO
        sbJS.Append("	document.forms[0].submit();" + Environment.NewLine);
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
        sbJS.Append(" 	    if (bCheck) { if (confirm(\'").Append(GetMessage("js confirm leave cgroup")).Append("\')) { document.getElementById(\'isDeleted\').value = \'1\'; document.form1.submit(); } else { bCheck = false; } } ").Append(Environment.NewLine);
        sbJS.Append(" 	    else { alert(\'").Append(GetMessage("js err please select cgroup leave")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" 	    return bCheck; ").Append(Environment.NewLine);
        sbJS.Append("     } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text = sbJS.ToString();
    }

    private void Populate_ViewCommunityGroupsGrid(CommunityGroupData[] data)
    {
        System.Web.UI.WebControls.BoundColumn colBound;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKL";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        GroupGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LEFT";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        GroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECKR";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Width = Unit.Percentage(5);
        GroupGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RIGHT";
        colBound.ItemStyle.Width = Unit.Percentage(45);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.Wrap = false;
        GroupGrid.Columns.Add(colBound);

        PageSettings();

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECKL", typeof(string)));
        dt.Columns.Add(new DataColumn("LEFT", typeof(string)));
        dt.Columns.Add(new DataColumn("CHECKR", typeof(string)));
        dt.Columns.Add(new DataColumn("RIGHT", typeof(string)));
        int i = 0;


        if (!(data == null))
        {
            // add select all row.
            dr = dt.NewRow();
            dr["CHECKL"] = "<input type=\"checkbox\" name=\"checkall\" id=\"req_deleted_users\" onClick=\"javascript:checkAll(\'\');\">";
            dr["LEFT"] = GetMessage("generic select all msg") + "<br/><br/>";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["CHECKL"] = "&#160;";
            dr["LEFT"] = "&#160;";
            dt.Rows.Add(dr);

            for (i = 0; i <= data.Length - 1; i++)
            {
                dr = dt.NewRow();
                if (data[i].GroupAdmin.Id == this.m_refContentApi.UserId)
                {
                    dr["CHECKL"] = "&#160;";
                }
                else
                {
                    dr["CHECKL"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].GroupId + "\" onClick=\"javascript:checkAll(\'req_deleted_users\');\">";
                }
                dr["LEFT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "group.gif" + "\" width=\"32\" height=\"32\"/>" + data[i].GroupName;
                if (i < (data.Length - 1))
                {
                    i++;
                    if (data[i].GroupAdmin.Id == this.m_refContentApi.UserId)
                    {
                        dr["CHECKR"] = "&#160;";
                    }
                    else
                    {
                        dr["CHECKR"] = "<input type=\"checkbox\" name=\"req_deleted_users\" id=\"req_deleted_users\" value=\"" + data[i].GroupId + "\" onClick=\"javascript:checkAll(\'req_deleted_users\');\">";
                    }
                    dr["RIGHT"] = "<img align=\"left\" src=\"" + this.m_refContentApi.AppImgPath + "group.gif" + "\"/>" + data[i].GroupName;
                }
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        GroupGrid.DataSource = dv;
        GroupGrid.DataBind();
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
        // isPostData.Value = "true"
    }
    #endregion

}
