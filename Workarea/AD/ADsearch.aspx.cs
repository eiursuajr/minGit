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
using Ektron.Newtonsoft;
using Ektron.Newtonsoft.Json;

namespace Ektron.Workarea.ActiveDirectory
{
    public class SelectedUser
    {
        private string _Username;
        private string _Domain;

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }

        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
            }
        }

    }

    public partial class AddUsers : workareabase
    {

        #region Members

        protected CommonApi _CommonApi = new CommonApi();
        protected SiteAPI _SiteApi = new SiteAPI();
        protected UserAPI _UserApi = new UserAPI();
        protected int _GroupType = 0;
        protected long _GroupId = 2;
        protected SettingsData _SettingsData;
        protected UserData[] _UserData;
        protected string m_strUserName = "";
        protected string m_strFirstName = "";
        protected string m_strLastName = "";
        protected string _Domain = "";
        protected bool _IsSaveAdded = false;
        protected DomainData[] _DomainData;
        protected EkMessageHelper _MessageHelper;

        #endregion

        #region Events

        protected void Page_Init(object sender, System.EventArgs e)
        {

            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;

            //register js/css
            RegisterResources();

            _GroupType = System.Convert.ToInt32((string.IsNullOrEmpty(Request.QueryString["grouptype"])) ? 0 : (Convert.ToInt32(Request.QueryString["grouptype"])));
            _GroupId = (string.IsNullOrEmpty(Request.QueryString["groupid"])) ? 2 : (Convert.ToInt64(Request.QueryString["groupid"]));
            _SettingsData = _SiteApi.GetSiteVariables(_SiteApi.UserId);
            _MessageHelper = _CommonApi.EkMsgRef;

            this.litSuccess.Text = _MessageHelper.GetMessage("aduser add success");

        }
        protected override void Page_Load(object sender, System.EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if ((!Page.IsPostBack) && ddlDomainName.Items.Count == 0)
                {
                    _DomainData = _UserApi.GetDomains(0, 0);
                    if ((!(_DomainData == null)) && _CommonApi.RequestInformationRef.ADAdvancedConfig == false)
                    {
                        ddlDomainName.Items.Add(new ListItem(base.GetMessage("all domain select caption"), ""));
                    }
                    int i;
                    if (_DomainData != null)
                    {
                        for (i = 0; i <= _DomainData.Length - 1; i++)
                        {
                            ddlDomainName.Items.Add(new ListItem(_DomainData[i].Name, _DomainData[i].Name));
                        }
                    }
                }
                else
                {
                    if (!(Page.Session["user_list"] == null))
                    {
                        _UserData = (Ektron.Cms.UserData[])Page.Session["user_list"];
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
        protected override void Page_PreRender(object sender, System.EventArgs e)
        {
            base.Page_PreRender(sender, e);
            if (_UserData == null)
            {
                SetLabels("");
            }
            else
            {
                if (_UserData.Length > 0)
                {
                    SetLabels("results");
                    BindDataGrid();
                    this.dgAddADUser.Visible = true;
                }
                else
                {
                    SetLabels("");
                    this.dgAddADUser.Visible = false;
                }
            }
        }
        protected void btnSearch_Click(object sender, System.EventArgs e)
        {
            this.uxPaging.SelectedPage = 0;
            string Sort = "";
            System.Collections.Specialized.NameValueCollection sdAttributes = new System.Collections.Specialized.NameValueCollection(); //New Collection
            System.Collections.Specialized.NameValueCollection sdFilter = new System.Collections.Specialized.NameValueCollection(); //New Collection

            try
            {
                sdAttributes.Add("UserName", "UserName");
                sdAttributes.Add("FirstName", "FirstName");
                sdAttributes.Add("LastName", "LastName");
                sdAttributes.Add("Domain", "Domain");

                m_strUserName = Strings.Trim((string)username.Value);
                m_strFirstName = Strings.Trim((string)firstname.Value);
                m_strLastName = Strings.Trim((string)lastname.Value);
                _Domain = Strings.Trim((string)ddlDomainName.SelectedValue);
                Sort = "UserName";

                if ((string.IsNullOrEmpty(m_strUserName)) && (string.IsNullOrEmpty(m_strFirstName)) && (string.IsNullOrEmpty(m_strLastName)))
                {
                    sdFilter.Add("UserName", "UserName");
                    sdFilter.Add("UserNameValue", "*");
                }
                else
                {
                    if (!string.IsNullOrEmpty(m_strUserName))
                    {
                        sdFilter.Add("UserName", "UserName");
                        sdFilter.Add("UserNameValue", m_strUserName); //sdFilter.add (UserName,"UserNameValue")
                    }
                    if (!string.IsNullOrEmpty(m_strFirstName))
                    {
                        sdFilter.Add("FirstName", "FirstName");
                        sdFilter.Add("FirstNameValue", m_strFirstName);
                    }
                    if (!string.IsNullOrEmpty(m_strLastName))
                    {
                        sdFilter.Add("LastName", "LastName");
                        sdFilter.Add("LastNameValue", m_strLastName);
                    }
                }

                _UserData = _UserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, _Domain);

                if (!(Page.Session["user_list"] == null))
                {
                    Page.Session.Remove("user_list");
                }
                Page.Session.Add("user_list", _UserData);

                this.dgAddADUser.Visible = true;
                divAddUser.Visible = true;
                divUsersAdded.Visible = false;
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }
        protected void lbSave_Click(System.Object sender, EventArgs e)
        {

            try
            {

                List<SelectedUser> selectedUsers;

                selectedUsers = (List<SelectedUser>)JsonConvert.DeserializeObject(this.hdnSelectedItems.Value, typeof(List<SelectedUser>));


                Collection userNames = new Collection();
                Collection domains = new Collection();
                foreach (SelectedUser user in selectedUsers)
                {
                    if ((user.Username != "") && (user.Domain != ""))
                    {
                        userNames.Add(user.Username, userNames.Count.ToString(), null, null);
                        domains.Add(user.Domain, userNames.Count.ToString(), null, null);
                    }
                }

                if (_GroupType == 0)
                {
                    _UserApi.AddADUsersToCMSByUsername(userNames, domains);
                }
                else
                {
                    Ektron.Cms.User.EkUser ekUser;
                    ekUser = _UserApi.EkUserRef;
                    ekUser.AddADmemberUsersToCmsByUsername(userNames, domains);
                }

                Page.Session.Remove("user_list");
                _UserData = null;
                this.hdnSelectedItems.Value = string.Empty;

                divAddUser.Visible = false;
                divUsersAdded.Visible = true;

            }
            catch (Exception ex)
            {

                Utilities.ShowError(ex.Message);

            }

        }
        public void dgAddADUser_ItemDataBound(System.Object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {

            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    //cell 0 - checkbox
                    //cell 1 - username
                    //cell 2 - firstname
                    //cell 3 - lastname
                    //cell 4 - domain
                    ((Literal)(e.Item.Cells[1].FindControl("litUsernameHeader"))).Text = "Username";
                    ((Literal)(e.Item.Cells[2].FindControl("litLastNameHeader"))).Text = "Last Name";
                    ((Literal)(e.Item.Cells[3].FindControl("litFirstNameHeader"))).Text = "First Name";
                    ((Literal)(e.Item.Cells[4].FindControl("litDomainHeader"))).Text = "Domain";
                    break;
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    ((HtmlInputHidden)(e.Item.Cells[0].FindControl("hdnUsername"))).Value = ((UserData)e.Item.DataItem).Username;
                    ((HtmlInputHidden)(e.Item.Cells[0].FindControl("hdnDomain"))).Value = ((UserData)e.Item.DataItem).Domain;
                    ((Literal)(e.Item.Cells[1].FindControl("litUsername"))).Text = ((UserData)e.Item.DataItem).Username;
                    ((Literal)(e.Item.Cells[1].FindControl("litLastName"))).Text = ((UserData)e.Item.DataItem).LastName;
                    ((Literal)(e.Item.Cells[1].FindControl("litFirstName"))).Text = ((UserData)e.Item.DataItem).FirstName;
                    ((Literal)(e.Item.Cells[2].FindControl("litDomain"))).Text = ((UserData)e.Item.DataItem).Domain;
                    break;
            }
        }

        #endregion

        #region Helpers

        private void BindDataGrid()
        {

            this.dgAddADUser.PageSize = _CommonApi.RequestInformationRef.PagingSize;
            this.dgAddADUser.CurrentPageIndex = this.uxPaging.SelectedPage;
            this.dgAddADUser.DataSource = _UserData;
            this.dgAddADUser.DataBind();
            if (this.dgAddADUser.PageCount > 1)
            {
                this.uxPaging.TotalPages = this.dgAddADUser.PageCount;
                this.uxPaging.CurrentPageIndex = this.dgAddADUser.CurrentPageIndex;
                this.uxPaging.Visible = true;
            }
            else
            {
                this.uxPaging.Visible = false;
            }

        }
        private void SetLabels(string type)
        {
            base.Title = base.GetMessage("view users in active directory msg");
            base.SetTitleBarToMessage("view users in active directory msg");

			base.AddBackButton("../users.aspx?backaction=viewallusers&action=viewallusers&grouptype=" + _GroupType.ToString() + "&groupid=2&id=2&FromUsers=1");

            if (type == "results")
            {
                base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "alt add button text (users)", "btn save", " onclick=\"Ektron.Workarea.ActiveDirectory.AddUser.submit();return false;\" ", StyleHelper.SaveButtonCssClass, true);
                _IsSaveAdded = true;
            }

            base.AddHelpButton("editusers_ascx");
        }
        private bool LDAPMembers()
        {
            if (_GroupType == 1) //member
            {
                return (_UserApi.RequestInformationRef.LDAPMembershipUser);
            }
            else if (_GroupType == 0) //CMS user
            {
                return true;
            }
            return true;
        }
        #endregion

        #region JS/CSS

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

            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        }

        #endregion

    }
}


