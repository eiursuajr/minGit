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

public partial class LDAP_LDAPsearch : workareabase
{
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected int m_intGroupType = 0;
    protected long m_intGroupId = 2;
    protected SettingsData setting_data;
    protected LDAPSettingsData ldapSettings;
    protected UserData[] user_list = (UserData[])Array.CreateInstance(typeof(Ektron.Cms.UserData), 0);
    protected string m_strUserName = "";
    protected string m_strFirstName = "";
    protected string m_strLastName = "";
    protected string m_strDomain = "";
    protected bool m_bSaveAdded = false;
    protected DomainData[] domain_list;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        try
        {
            if (!(Request.QueryString["grouptype"] == null) && Request.QueryString["grouptype"] != "")
            {
                m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
            }
            if (!(Request.QueryString["groupid"] == null) && Request.QueryString["groupid"] != "")
            {
                m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
            }

            RegisterResources();

            setting_data = m_refSiteApi.GetSiteVariables(-1);
            ldapSettings = setting_data.LDAPSettings;
            if ((!Page.IsPostBack) && paths.Items.Count == 0)
            {
                paths.Items.Add(new ListItem("All Paths", ""));
                foreach (string path in ldapSettings.Paths)
                    paths.Items.Add(new ListItem(path, path));
                paths.Items.Add(new ListItem(ldapSettings.Base + " (Root)", "/"));
            }
            else
            {
                if (Request.Form["rp"] != "")
                {
                    AddLDAPUsersToSystem();
                    Response.Redirect("../users.aspx?action=viewallusers&grouptype=" + m_intGroupType.ToString() + "&groupid=" + m_intGroupId.ToString() + "&id=" + m_intGroupId.ToString() + "&OrderBy=user_name", false);
                    return;
                }
                if (!(Page.Session["user_list"] == null))
                {
                    user_list = (UserData[])Page.Session["user_list"];
                }
            }
            if (user_list.Length > 0)
            {
                SetLabels("results");
            }
            else
            {
                SetLabels("");
            }
            if (LDAPMembers() && (setting_data.ADAuthentication == 1))
            {
                if (!(Page.IsPostBack))
                {
                    //Display_AddLDAPUserToSystem()
                }
                else
                {
                    //AddLDAPUsersToSystem()
                }
            }
        }
        catch (Exception ex)
        {
            string sErr = "";
            sErr = ex.Message;
            if (sErr.IndexOf("[") > -1)
            {
                sErr = sErr.Substring(0, sErr.IndexOf("["));
            }
            Utilities.ShowError(sErr);
        }
    }

    private void SetLabels(string type)
    {
        base.Title = "Search LDAP users";
        base.SetTitleBarToString("Search LDAP users");
		base.AddBackButton("../users.aspx?action=AddUserToSystem&LangType=" + this.ContentLanguage + "&grouptype=" + m_intGroupType.ToString() + "&id=2&groupid=" + m_intGroupId.ToString() + "&OrderBy=user_name&FromUsers=1");
        if (type == "results")
        {
            base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "alt add button text (users)", "btn save", " onclick=\"javascript:toggleVisibility(document.getElementById(\'dvHoldMessage\'));\" ", StyleHelper.SaveButtonCssClass, true);
            m_bSaveAdded = true;
        }
        //MyBase.AddBackButton("../users.aspx?backaction=viewallusers&action=viewallusers&grouptype=" & m_intGroupType.ToString() & "&groupid=2&id=2&FromUsers=1")
        base.AddHelpButton("editusers_ascx");
    }

    public void PageChange(object sender, DataGridPageChangedEventArgs e)
    {
        AddUserGrid.PageSize = m_refUserApi.RequestInformationRef.PagingSize;
        AddUserGrid.CurrentPageIndex = e.NewPageIndex;
        AddLDAPUserToSystemToolBarGrid();
    }

    private void AddLDAPUsersToSystem()
    {
        Collection sdUsersNames = new Collection();
        Collection sdUsersDomains = new Collection();
        int userCount = System.Convert.ToInt32(Request.Form[addusercount.UniqueID]);
        for (int i = 1; i <= userCount; i++)
        {
            if (Request.Form["adduser" + i.ToString()] != null)
            {
                string strUsername = Request.Form["adduser" + i.ToString()].ToString();
                string strDomain = Request.Form["adduserdomain" + i.ToString()].ToString();
                if (!string.IsNullOrEmpty(strUsername) && !string.IsNullOrEmpty(strDomain))
                {
                    sdUsersNames.Add(strUsername, i.ToString(), null, null);
                    sdUsersDomains.Add(strDomain, i.ToString(), null, null);
                }
            }
            //Response.Write(strUsername & " " & strDomain & "<br /")
        }
        Ektron.Cms.User.EkUser usr;
        bool ret = false;
        usr = m_refUserApi.EkUserRef;
        // m_intGroupType = 0 = authors
        ret = usr.AddLDAPUsersToCMSByUsername(sdUsersNames, sdUsersDomains, m_intGroupType);
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
        return true;
    }

    protected void cmsSearch_Click(object sender, System.EventArgs e)
    {
        string Sort = "";
        System.Collections.Specialized.NameValueCollection sdAttributes = new System.Collections.Specialized.NameValueCollection(); //New Collection
        System.Collections.Specialized.NameValueCollection sdFilter = new System.Collections.Specialized.NameValueCollection(); //New Collection

        try
        {
            sdAttributes.Add("UserName", "UserName");
            sdAttributes.Add("FirstName", "FirstName");
            sdAttributes.Add("LastName", "LastName");
            sdAttributes.Add("Path", "Path");

            m_strUserName = Strings.Trim((string)username.Value);
            m_strFirstName = Strings.Trim((string)firstname.Value);
            m_strLastName = Strings.Trim((string)lastname.Value);
            m_strDomain = Strings.Trim((string)paths.SelectedValue);
            Sort = "UserName";
            if ((m_strUserName == "") && (m_strFirstName == "") && (m_strLastName == ""))
            {
                sdFilter.Add("UserName", "UserName");
                sdFilter.Add("UserNameValue", "*");
            }
            else
            {
                if (m_strUserName != "")
                {
                    sdFilter.Add("UserName", "UserName");
                    sdFilter.Add("UserNameValue", m_strUserName); //sdFilter.add (UserName,"UserNameValue")
                }
                if (m_strFirstName != "")
                {
                    sdFilter.Add("FirstName", "FirstName");
                    sdFilter.Add("FirstNameValue", m_strFirstName);
                }
                if (m_strLastName != "")
                {
                    sdFilter.Add("LastName", "LastName");
                    sdFilter.Add("LastNameValue", m_strLastName);
                }
            }
            user_list = m_refUserApi.EkUserRef.GetAvailableLDAPUsers(sdAttributes, sdFilter, Sort, m_strDomain, 0);
            if (!(Page.Session["user_list"] == null))
            {
                Page.Session.Remove("user_list");
            }
            Page.Session.Add("user_list", user_list);
            AddUserGrid.PageSize = m_refUserApi.RequestInformationRef.PagingSize;
            AddUserGrid.CurrentPageIndex = 0;
            if (user_list.Length <= m_refUserApi.RequestInformationRef.PagingSize)
            {
                AddUserGrid.AllowPaging = false;
                FirstPage.Visible = false;
                LastPage.Visible = false;
            }
            else
            {
                AddUserGrid.AllowPaging = true;
                FirstPage.Visible = true;
                LastPage.Visible = true;
            }
            if (user_list.Length == 0)
            {
                ltr_message.Text = "<br />" + base.GetMessage("the search resulted in zero matches");
            }
            else if (user_list.Length > 0 && m_bSaveAdded == false)
            {
                ltr_message.Text = "";
                base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "alt add button text (users)", "btn save", " onclick=\"javascript:toggleVisibility(document.getElementById(\'dvHoldMessage\'));\" ", 0, StyleHelper.SaveButtonCssClass, true);
            }
            AddLDAPUserToSystemToolBarGrid();
        }
        catch (Exception ex)
        {
            string sErr = "";
            sErr = ex.Message;
            if (sErr.ToLower().IndexOf("no such object") > -1)
            {
                sErr = (string)(base.GetMessage("ldap setup err") + " " + base.GetMessage("ldap path err").Replace("@path@", "\"" + m_strDomain + "\""));
            }
            Utilities.ShowError(sErr);
        }
    }

    private void AddLDAPUserToSystemToolBarGrid()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECK";
        colBound.HeaderText = m_refMsg.GetMessage("add title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(2);
        colBound.ItemStyle.Width = Unit.Percentage(2);
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = base.GetMessage("generic Username");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTNAME";
        colBound.HeaderText = base.GetMessage("generic Firstname");
        colBound.ItemStyle.Wrap = false;
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTNAME";
        colBound.HeaderText = base.GetMessage("generic Lastname");
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Wrap = false;
        AddUserGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PATH";
        colBound.HeaderText = m_refMsg.GetMessage("generic path");
        colBound.ItemStyle.Wrap = false;
        AddUserGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("PATH", typeof(string)));

        int i = 0;

        if (!(user_list == null))
        {
            for (i = 0; i <= user_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<input type=\"CHECKBOX\" name=\"adduser" + (i + 1) + "\" value=\"" + user_list[i].Username + ("\">" + "<input type=\"HIDDEN\" name=\"adduserdomain") + (i + 1) + "\" value=\"" + user_list[i].Domain + "\">";
                dr[1] = user_list[i].Username;
                dr[2] = user_list[i].FirstName;
                dr[3] = user_list[i].LastName;
                dr[4] = user_list[i].Domain;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = "No LDAP users have been found.";
            dr[1] = "";
            dr[2] = "";
            dr[3] = "";
            dt.Rows.Add(dr);
        }
        addusercount.Value = Convert.ToString(i + 1);
        DataView dv = new DataView(dt);
        AddUserGrid.DataSource = dv;
        AddUserGrid.DataBind();
    }

    private void RegisterResources()
    {
        System.Text.StringBuilder sbJS = new System.Text.StringBuilder();

        sbJS.Append("<script language=\"JavaScript\">").Append(Environment.NewLine);
        sbJS.Append("	function toggleVisibility(me){").Append(Environment.NewLine);
        sbJS.Append("		if (me.style.visibility==\"hidden\"){").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		else {").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("	    }").Append(Environment.NewLine);
        sbJS.Append("		(document.getElementById(\'rp\')).value = \'submit\';").Append(Environment.NewLine);
        sbJS.Append("		document.forms[0].submit();").Append(Environment.NewLine);
        sbJS.Append("		return false;").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("		function CheckKeyValue(item, keys) {").Append(Environment.NewLine);
        sbJS.Append("			var keyArray = keys.split(\",\");").Append(Environment.NewLine);
        sbJS.Append("			for (var i = 0; i < keyArray.length; i++) {").Append(Environment.NewLine);
        sbJS.Append("				if ((document.layers) || ((!document.all) && (document.getElementById))) {").Append(Environment.NewLine);
        sbJS.Append("					if (item.which == keyArray[i]) {").Append(Environment.NewLine);
        sbJS.Append("						return false;").Append(Environment.NewLine);
        sbJS.Append("					}").Append(Environment.NewLine);
        sbJS.Append("				}").Append(Environment.NewLine);
        sbJS.Append("				else {").Append(Environment.NewLine);
        sbJS.Append("					if (event.keyCode == keyArray[i]) {").Append(Environment.NewLine);
        sbJS.Append("						return false;").Append(Environment.NewLine);
        sbJS.Append("					}").Append(Environment.NewLine);
        sbJS.Append("				}").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		}").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);
        sbJS.Append("").Append(Environment.NewLine);

        ltr_js.Text = sbJS.ToString();

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        int ival = 0;
        switch (e.CommandName)
        {
            case "First":
                if (AddUserGrid.AllowPaging == true)
                {
                    AddUserGrid.CurrentPageIndex = 0;
                }
                break;
            case "Last":
                if (!(user_list == null) && user_list.Length > 0)
                {

                    ival = user_list.Length / m_refUserApi.RequestInformationRef.PagingSize;
                    if (user_list.Length % m_refUserApi.RequestInformationRef.PagingSize > 0)
                    {
                        ival++;
                    }
                }
                if (ival > 0) // for 0 based index
                {
                    ival--;
                }
                if (AddUserGrid.AllowPaging == true)
                {
                    AddUserGrid.CurrentPageIndex = ival;
                }
                break;
        }
        AddLDAPUserToSystemToolBarGrid();
    }
}


