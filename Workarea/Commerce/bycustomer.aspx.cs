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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

public partial class Commerce_bycustomer : workareabase
{
    protected long m_intGroupId = -1;
    protected string m_strKeyWords = "";
    protected string m_strSelectedItem = "-1";
    protected CustomerApi CustomerManager = null;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        Utils_RegisterResources();
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }
        ltr_noCustSelected.Text = GetMessage("js alert no cust selected");
		ltr_searchuserdefault.Text = GetMessage("lbl valid customer");
        ltr_searchusertext.Text = GetMessage("alert msg search");
        MapCMSUserToADGrid.DataSource = "";
        if ((!(Request.QueryString["groupid"] == null)) && (Request.QueryString["groupid"] != ""))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
        }
        if (Page.IsPostBack)
        {
            ViewAllUsers();
        }
        ViewAllUsersToolBar();
    }

    public void ViewAllUsers()
    {
        m_strKeyWords = Request.Form["txtSearch"];
        if (Page.IsPostBack && Request.Form[isPostData.UniqueID] != "")
        {
            if (Request.Form[isSearchPostData.UniqueID] != "")
            {
                DisplayUsers();
            }
        }
        else if (IsPostBack == false)
        {
            ViewAllUsersToolBar();
        }
        isPostData.Value = "true";
    }
    private void ViewAllUsersToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table width=\"100%\"><tr class=\"ektronToolbar\">");
        result.Append("<td class=\"label\" title=" + m_refMsg.GetMessage("lbl text") + ">" + m_refMsg.GetMessage("lbl text") + "</td><td><input type=\"text\" title=\"Enter Search Text here\" class=\"minWidth\" size=\"25\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + m_strKeyWords + "\" onkeydown=\"CheckForReturn(event)\"/></td>");
        result.Append("<td class=\"label\" title=" + m_refMsg.GetMessage("lbl field") + ">" + m_refMsg.GetMessage("lbl field") + ":</td><td><select title=\"Select Field from the Drop Down Menu\" id=\"searchlist\" name=\"searchlist\">");
        //result.Append("<option value=""-1" & IsSelected("-1") & """>All</option>")
        result.Append("<option value=\"user_name\"" + IsSelected("user_name") + ">" + m_refMsg.GetMessage("lbl customer username") + "</option>");
        result.Append("<option value=\"first_name\"" + IsSelected("first_name") + ">" + m_refMsg.GetMessage("lbl first name") + "</option>");
        result.Append("<option value=\"last_name\"" + IsSelected("last_name") + ">" + m_refMsg.GetMessage("lbl last name") + "</option>");
        result.Append("</select></td><td><a class=\"button buttonInline buttonFilter blueHover btnFilter\" type=\"button\" name=\"btnSearch\" id=\"btnSearch\" Value=\"" + m_refMsg.GetMessage("btn filter") + "\" onClick=\"searchuser();\" style=\"font-size: 1em\" title=\"" + m_refMsg.GetMessage("btn filter") + "\">" + m_refMsg.GetMessage("btn filter") + "</a>");
        result.Append("<td><a type=\"button\" class=\"button buttonInline buttonOk greenHover btnOk\" name=\"btnOK\" id=\"btnOK\" Value=\"" + m_refMsg.GetMessage("lbl ok") + "\" onClick=\"getcheckedid();\" title=\"" + m_refMsg.GetMessage("lbl ok") + "\">" + m_refMsg.GetMessage("lbl ok") + "</a></td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
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
    private void DisplayUsers()
    {
        List<CustomerData> customerList = new List<CustomerData>();
        Ektron.Cms.Common.Criteria<CustomerProperty> CustomerCriteria = new Ektron.Cms.Common.Criteria<CustomerProperty>(CustomerProperty.UserName, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);

        //CustomerCriteria.AddFilter(CustomerProperty.TotalOrders, CriteriaFilterOperator.GreaterThan, 0)
        //CustomerCriteria.AddFilter(CustomerProperty.TotalOrderValue, CriteriaFilterOperator.GreaterThan, 0)

        CustomerManager = new CustomerApi();

        m_strKeyWords = Request.Form["txtSearch"];

        m_strSelectedItem = Request.Form["searchlist"];

        switch (m_strSelectedItem)
        {

            //Case "-1 selected " ' All

            //    CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords)

            //    CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords)

            //    CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords)

            //Case "-1" ' All

            //    CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords)

            //    CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords)

            //    CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords)

            case "last_name": // Last Name

                CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords);
                break;

            case "first_name": // First Name

                CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords);
                break;

            case "user_name": // User Name

                CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords);
                break;

        }

        customerList = CustomerManager.GetList(CustomerCriteria);

        ViewAllUsersToolBar();
        literal1.Text = "";
        if (customerList != null)
        {
            if (customerList.Count != 0)
            {
                if (customerList.Count > 0)
                {
                    dg_customers.DataSource = customerList;
                    dg_customers.DataBind();

                }
                else
                {
                    literal1.Text = "<br/><label style=\"color:#2E6E9E;\" id=\"lbl_noUsers\">" + this.GetMessage("lbl no users") + "</label>";
                }
            }
            else
            {
                literal1.Text = "<br/><label style=\"color:#2E6E9E;\" id=\"lbl_noUsers\">" + this.GetMessage("lbl no users") + "</label>";
            }
        }
        else
        {
            literal1.Text = "<br/><label style=\"color:#2E6E9E;\" id=\"lbl_noUsers\">" + this.GetMessage("lbl no users") + "</label>";
        }
        dg_customers.DataSource = customerList;
        dg_customers.DataBind();
    }
    protected void Utils_RegisterResources()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/box.css", "EktronBoxCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);

    }
}
