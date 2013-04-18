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
using Ektron.Cms.Workarea;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

public partial class Commerce_Audit_Audit : workareabase
{
    #region Member Variables

    protected string m_sPageName = "audit.aspx";
    protected long m_orderId = 0;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected CommerceAuditProperty sortcriteria = CommerceAuditProperty.DateCreated;
    protected EkEnumeration.OrderByDirection sortdirection = EkEnumeration.OrderByDirection.Descending;
    protected string searchCriteria = "";
    protected CommerceAuditProperty searchField = CommerceAuditProperty.Message;

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterCSS();
        this.RegisterJS();

    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            Util_CheckAccess();
            if (!string.IsNullOrEmpty(Request.QueryString["order"]))
            {
                m_orderId = Convert.ToInt64(Request.QueryString["folder"]);
            }
            if (!string.IsNullOrEmpty(Page.Request.QueryString["search"]))
            {
                searchCriteria = Page.Request.QueryString["search"];
            }
            if (!string.IsNullOrEmpty(Page.Request.QueryString["searchfield"]))
            {
                searchField = (CommerceAuditProperty)Enum.Parse(typeof(CommerceAuditProperty), Page.Request.QueryString["searchfield"]);
            }
            switch (base.m_sPageAction)
            {
                default:
                    if (Page.IsPostBack == false)
                    {
                        Display_Audit();
                    }
                    break;
            }
            Util_SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Display


    protected void Display_Audit()
    {

        CommerceAuditApi auditApi = new CommerceAuditApi();
        List<CommerceAuditData> auditLog = new List<CommerceAuditData>();

        Ektron.Cms.Common.Criteria<CommerceAuditProperty> auditCriteria = new Ektron.Cms.Common.Criteria<CommerceAuditProperty>();

        auditCriteria.OrderByField = sortcriteria;
        auditCriteria.OrderByDirection = sortdirection;

        auditCriteria.PagingInfo.CurrentPage = _currentPageNumber;
        auditCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;

        if (!string.IsNullOrEmpty(searchCriteria))
        {
            switch (searchField)
            {
                case CommerceAuditProperty.DateCreated:
                    DateTime searchDate = DateTime.Now;
                    if (DateTime.TryParse(searchCriteria, out searchDate) && !(searchDate == DateTime.MinValue))
                    {
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.GreaterThanOrEqualTo, searchDate.Date);
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.LessThan, searchDate.Date.AddDays(1));
                        auditLog = auditApi.GetList(auditCriteria);
                    }
                    break;
                case CommerceAuditProperty.OrderId:
                case CommerceAuditProperty.UserId:
                    long searchId = 0;
                    if (long.TryParse(searchCriteria, out searchId))
                    {
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.EqualTo, searchId);
                        auditLog = auditApi.GetList(auditCriteria);
                    }
                    break;
                default: // CommerceAuditProperty.IPAddress, CommerceAuditProperty.FormattedMessage
                    auditCriteria.AddFilter(searchField, CriteriaFilterOperator.Contains, searchCriteria);
                    auditLog = auditApi.GetList(auditCriteria);
                    break;
            }
        }
        else
        {
            auditLog = auditApi.GetList(auditCriteria);
        }

        if (auditLog.Count == 0 || auditCriteria.PagingInfo.TotalRecords == 0)
            ltr_noEntries.Visible = true;

        dg_audit.DataSource = auditLog;
        TotalPagesNumber = auditCriteria.PagingInfo.TotalPages;

        if (TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {
            lnkBtnPreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            LastPage.Enabled = true;
            NextPage.Enabled = true;
            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            lnkBtnPreviousPage.Visible = true;
            NextPage.Visible = true;
            LastPage.Visible = true;
            FirstPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;

            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
            TotalPages.ToolTip = TotalPages.Text;

            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;

            if (_currentPageNumber == 1)
            {
                lnkBtnPreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }

        // dg_audit.Columns(0).HeaderText = GetMessage("generic id")
        dg_audit.Columns[0].HeaderText = GetMessage("lbl generic date");
        // dg_audit.Columns(1).HeaderText = GetMessage("generic type")
        dg_audit.Columns[1].HeaderText = GetMessage("lbl ip address");
        dg_audit.Columns[2].HeaderText = GetMessage("lbl desc");
        dg_audit.Columns[3].HeaderText = GetMessage("lbl order id");
        dg_audit.Columns[4].HeaderText = GetMessage("lbl user id");

        dg_audit.DataBind();
    }


    #endregion

    #region Process


    protected void Process_()
    {



    }


    #endregion

    #region Util


    protected string Util_ShowLocal(DateTime GMTDatetime)
    {

        DateTime localDateTime = GMTDatetime.ToLocalTime();

        return localDateTime.ToShortDateString() + " " + localDateTime.ToShortTimeString();

    }

    protected void Util_SetLabels()
    {

        switch (base.m_sPageAction)
        {

            default:
                ltr_noEntries.Text = GetMessage("generic no results found");
                ListItemCollection items = new ListItemCollection();
                items.Add(new ListItem(GetMessage("lbl generic date"), CommerceAuditProperty.DateCreated.GetHashCode().ToString()));
                items.Add(new ListItem(GetMessage("lbl order id"), CommerceAuditProperty.OrderId.GetHashCode().ToString()));
                items.Add(new ListItem(GetMessage("lbl user id"), CommerceAuditProperty.UserId.GetHashCode().ToString()));
                items.Add(new ListItem(GetMessage("lbl ip address"), CommerceAuditProperty.IPAddress.GetHashCode().ToString()));
                items.Add(new ListItem(GetMessage("lbl desc"), CommerceAuditProperty.FormattedMessage.GetHashCode().ToString()));

                if (searchField == CommerceAuditProperty.DateCreated)
                {
                    items[0].Selected = true;
                }
                if (searchField == CommerceAuditProperty.OrderId)
                {
                    items[1].Selected = true;
                }
                if (searchField == CommerceAuditProperty.UserId)
                {
                    items[2].Selected = true;
                }
                if (searchField == CommerceAuditProperty.IPAddress)
                {
                    items[3].Selected = true;
                }
                if (searchField == CommerceAuditProperty.FormattedMessage)
                {
                    items[4].Selected = true;
                }

                this.AddSearchBox(searchCriteria, items, "searchAudit", false);
                this.SetTitleBarToMessage("lbl commerce audit");
                break;

        }

        AddHelpButton("commerceaudit");

        Util_SetJs();

    }

    private void Util_SetJs()
    {

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);

        sbJS.Append(" function searchAudit() { ").Append(Environment.NewLine);
        sbJS.Append("   var sSearchTerm = $ektron(\'#txtSearch\').val(); ").Append(Environment.NewLine);
        sbJS.Append("   var iSearchField = $ektron(\'#searchlist\').val(); ").Append(Environment.NewLine);
        sbJS.Append("   window.location.href = \'").Append(m_sPageName).Append("?searchfield=\' + iSearchField + \'&search=\' + sSearchTerm; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("</script>" + Environment.NewLine);

        ltr_js.Text += Environment.NewLine + sbJS.ToString();

    }

    protected void Util_CheckAccess()
    {

        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(GetMessage("feature locked error"));
        }

        if (!m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            throw (new Exception(GetMessage("err not role commerce-admin")));
        }

    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }

        Util_GetSortValue((string)(dg_audit.Attributes["SortExpression"]));

        Display_Audit();

        isPostData.Value = "true";

    }

    protected void Util_HidePagingLinks()
    {

        PageLabel.Visible = false;
        CurrentPage.Visible = false;
        OfLabel.Visible = false;
        TotalPages.Visible = false;

        FirstPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;

    }

    protected void Util_DG_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
    {

        Util_GetSortExpression(dg_audit, e);

        Display_Audit();

        isPostData.Value = "true";

    }

    private void Util_GetSortValue(string sortExpression)
    {

        switch (sortExpression.ToLower())
        {

            case "ip":

                sortcriteria = CommerceAuditProperty.IPAddress;
                sortdirection = EkEnumeration.OrderByDirection.Ascending;
                break;

            case "fmessage":

                sortcriteria = CommerceAuditProperty.Message;
                sortdirection = EkEnumeration.OrderByDirection.Ascending;
                break;

            case "orderid":

                sortcriteria = CommerceAuditProperty.OrderId;
                sortdirection = EkEnumeration.OrderByDirection.Ascending;
                break;

            case "userid":

                sortcriteria = CommerceAuditProperty.UserId;
                sortdirection = EkEnumeration.OrderByDirection.Ascending;
                break;

            default:

                sortcriteria = CommerceAuditProperty.DateCreated;
                sortdirection = EkEnumeration.OrderByDirection.Descending;
                break;

        }

    }

    private void Util_GetSortExpression(DataGrid dg, DataGridSortCommandEventArgs e)
    {
        string[] sortColumns = null;
        string sortAttribute = dg.Attributes["SortExpression"];
        //Check to See if we have an existing Sort Order already in the Grid.
        //If so get the Sort Columns into an array
        if (e.SortExpression != sortAttribute)
        {
            sortAttribute = e.SortExpression;
        }
        if (sortAttribute != string.Empty)
        {
            sortColumns = sortAttribute.Split(",".ToCharArray());
        }
        //if User clicked on the columns in the existing sort sequence.
        //Toggle the sort order or remove the column from sort appropriately
        if (sortAttribute.IndexOf(e.SortExpression) > 0 || sortAttribute.StartsWith(e.SortExpression))
        {
            sortAttribute = Util_ModifySortExpression(sortColumns, e.SortExpression);
        }
        else
        {
            sortAttribute += e.SortExpression + " ASC,";
        }
        dg.Attributes["SortExpression"] = sortAttribute;

        // Return sortAttribute
        Util_GetSortValue(sortAttribute);

    }

    private string Util_ModifySortExpression(string[] sortColumns, string sortExpression)
    {

        string ascSortExpression = string.Concat(sortExpression, " ASC");
        string descSortExpression = string.Concat(sortExpression, " DESC");

        for (int i = 0; i <= sortColumns.Length - 1; i++)
        {

            if (ascSortExpression.Equals(sortColumns[i]))
            {
                sortColumns[i] = descSortExpression;

            }
            else if (descSortExpression.Equals(sortColumns[i]))
            {
                Array.Clear(sortColumns, i, 1);
            }
        }

        return string.Join(",", sortColumns).Replace(",,", ",").TrimStart(",".ToCharArray());

    }


    #endregion

    #region JS, CSS

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronMenuCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronMenuJs");

    }

    #endregion

}
