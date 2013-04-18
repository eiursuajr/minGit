using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Commerce.Reporting;
using Ektron.Cms.Commerce.KPI;
using Ektron.Cms.Commerce.KPI.Provider;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;

public partial class Commerce_reporting_KeyPerformanceIndicators : System.Web.UI.Page {
    #region Private Members

    private List<IKPI> _data = new List<IKPI>();
    private EkRequestInformation _requestInfo = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
    private string _periodClientIdPrefix = String.Empty;
    private string _clientPeriodSelectionTarget = String.Empty;
    private string _clientPeriodSelectionType = String.Empty;
    private EkEnumeration.KPIPeriod _clientPeriodSelectionPeriod;
    private ContentAPI _ContentApi;
    private string _wrapperId = string.Empty;

    protected ContentAPI ContentApi {
        get { return _ContentApi ?? (_ContentApi = new ContentAPI()); }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e) {
        _periodClientIdPrefix = _wrapperId + "_" + this.ClientID + "_kpiperiod_";
		Utilities.ValidateUserLogin();
        RegisterResources();
        imgBusyImage.BorderWidth = 0;
        imgBusyImage.ImageUrl = _requestInfo.AppImgPath + "ajax-loader_circle_lg.gif";
        imgBusyImage.Attributes.Add("style", "background-color: transparent;");

        if (!String.IsNullOrEmpty(Request.QueryString["clientid"]))
            _wrapperId = Request.QueryString["clientid"];
        else
            BuildToolBar();

        if (!(ContentApi.IsLoggedIn && (ContentApi.IsAdmin() || ContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin)))) {
            ShowError(GetMessage("err not role commerce-admin"));
            return;
        }
        
        GetKPIs();
        BuildTable();
    }

    protected void ShowError(string msg){
            contentPanel.Visible = false;
            errorPanel.Visible = true;
            litError.Text = msg;
    }

    protected void BuildToolBar() {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<div style=\"height:22px\">");
        result.Append((new StyleHelper()).GetHelpButton("keyperformanceindicators", "center"));
        result.Append("</div>");
        divToolBar.InnerHtml = result.ToString();

        divTitleBar.InnerText = GetMessage("lbl key performance indicators");
        headerPanel.Visible = true;
    }

    protected void BuildTable() {
        HtmlTable mainTable = new HtmlTable();
        mainTable.Attributes.Add("class", "ektronGrid ektronBorder grdData");
        mainTable.Attributes.Add("style", "width: 100%; border-collapse: collapse; display: table;");
        mainTable.CellSpacing = 0;
        mainTable.Border = 0;

        ////////////////////       
        // add table header
        HtmlTableRow tr = new HtmlTableRow();
        tr.Attributes.Add("class", "title-header");

        HtmlTableCell td = new HtmlTableCell();
        td.InnerText = GetMessage("lbl indicator");
        tr.Cells.Add(td);

        td = new HtmlTableCell();
        td.Align = "right";
        td.InnerText = GetMessage("lbl period");
        tr.Cells.Add(td);

        td = new HtmlTableCell();
        td.Align = "right";
        td.InnerText = GetMessage("lbl current");
        tr.Cells.Add(td);

        td = new HtmlTableCell();
        td.Align = "right";
        td.InnerText = GetMessage("previous");
        tr.Cells.Add(td);

        td = new HtmlTableCell();
        td.Align = "right";
        td.InnerText = GetMessage("btn change");
        tr.Cells.Add(td);

        mainTable.Rows.Add(tr);

        ////////////////////       
        // add table data:
        if (null != _data) {
            bool stripe = false;
            foreach (Ektron.Cms.Commerce.IKPI ikpi in _data) {
                tr = new HtmlTableRow();
                if (stripe)
                    tr.Attributes.Add("class", "stripe");
                stripe = !stripe;

                string st = ikpi.Indicator;
                if (st == "Sales")
                    st = this.GetMessage("lbl sales");
                else if (st == "Orders")
                    st = this.GetMessage("lbl orders");

                td = new HtmlTableCell();
                td.InnerHtml = st + " " + GetRefreshButton(null);
                tr.Cells.Add(td);

                td = new HtmlTableCell();
                td.Align = "right";
                td.InnerHtml = BuildPeriodSelector(ikpi.Indicator, "Current", ikpi.CurrentPeriod) + this.GetMessage("lbl versus") + " " + BuildPeriodSelector(ikpi.Indicator, "Previous", ikpi.PreviousPeriod);
                tr.Cells.Add(td);

                td = new HtmlTableCell();
                td.Align = "right";
                td.InnerText = ikpi.FormatedCurrentValue.ToString();
                tr.Cells.Add(td);

                td = new HtmlTableCell();
                td.Align = "right";
                td.InnerText = ikpi.FormatedPreviousValue.ToString();
                tr.Cells.Add(td);

                td = new HtmlTableCell();
                td.Align = "right";
                td.InnerHtml = BuildChangeDisplay(ikpi.CurrentValue, ikpi.PreviousValue);
                tr.Cells.Add(td);

                mainTable.Rows.Add(tr);

            }
        }

        phTableContainer.Controls.Add(mainTable);
    }

    protected string BuildPeriodSelector(string indicator, string type, EkEnumeration.KPIPeriod period) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string nameId = _periodClientIdPrefix + type + "_" + indicator;
        sb.Append("<select class='kpiPeriod' name='" + nameId + "' onchange='" + GetClientClickScript(nameId) + "' >" + Environment.NewLine);

        foreach (EkEnumeration.KPIPeriod kpiperiod in Enum.GetValues(typeof(EkEnumeration.KPIPeriod))) {
            sb.Append("<option ");
            if (period == kpiperiod) {
                sb.Append("selected='selected' ");
            }

            sb.Append("value='" + kpiperiod.ToString() + "' ");
            sb.Append(">" + GetLocalizedEnumName(kpiperiod) + "</option>" + Environment.NewLine);
        }

        sb.Append("</select>" + Environment.NewLine);
        return sb.ToString();
    }

    public List<IKPI> GetKPIs() {
        KPIManager kpiManager = new KPIManager();
        KPIProviderCollection kpiProviders = kpiManager.Providers;
        System.Collections.IEnumerator iterator = kpiProviders.GetEnumerator();

        while (iterator.MoveNext()) {
            IKPI kpi = (IKPI)iterator.Current;

            // set default periods
            kpi.CurrentPeriod = EkEnumeration.KPIPeriod.ThisMonth;
            kpi.PreviousPeriod = EkEnumeration.KPIPeriod.LastMonth;

            // recover from session
            kpi.CurrentPeriod = GetPeriod((string)Session[_wrapperId + "_Current_" + kpi.Indicator], kpi.CurrentPeriod);
            kpi.PreviousPeriod = GetPeriod((string)Session[_wrapperId + "_Previous_" + kpi.Indicator], kpi.PreviousPeriod);

            RecoverUserPeriodSelection();
            if (_clientPeriodSelectionTarget == kpi.Indicator) {
                if (_clientPeriodSelectionType == "Current")
                    kpi.CurrentPeriod = _clientPeriodSelectionPeriod;
                else if (_clientPeriodSelectionType == "Previous")
                    kpi.PreviousPeriod = _clientPeriodSelectionPeriod;
            }

            // store to session:
            Session[_wrapperId + "_Current_" + kpi.Indicator] = kpi.CurrentPeriod.ToString();
            Session[_wrapperId + "_Previous_" + kpi.Indicator] = kpi.PreviousPeriod.ToString();

            kpi.Calculate(null);
            _data.Add(kpi);
        }

        return _data;
    }

    protected EkEnumeration.KPIPeriod GetPeriod(string rawValue, EkEnumeration.KPIPeriod defaultPeriod) {
        try {
            if (!String.IsNullOrEmpty(rawValue))
                return (EkEnumeration.KPIPeriod)Enum.Parse(typeof(EkEnumeration.KPIPeriod), rawValue, true);
        }
        catch { }
        return defaultPeriod;
    }

    protected void RecoverUserPeriodSelection() {
        try {
            if (IsPostBack
                && Request.Form["__EVENTTARGET"] != null
                && Request.Form["__EVENTTARGET"].StartsWith(_periodClientIdPrefix)) {
                string[] clientSelection = Request.Form["__EVENTTARGET"].Replace(_periodClientIdPrefix, "").Split('_');
                _clientPeriodSelectionType = clientSelection[0];
                _clientPeriodSelectionTarget = clientSelection[1];
                _clientPeriodSelectionPeriod = GetPeriod(Request.Form[Request.Form["__EVENTTARGET"]], EkEnumeration.KPIPeriod.ThisMonth);
            }
        }
        catch { _clientPeriodSelectionTarget = String.Empty; }
    }

    protected string GetClientClickScript(string passedText) {
        return "$ektron(\"#" + imgBusyImage.ClientID + "\").show();__doPostBack(\"" + passedText + "\", \"\");";
    }

    protected void RegisterResources() {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, _requestInfo.ApplicationPath + "Personalization/css/ektron.personalization.css", "ektron_personalization_css", true);
    }

    public string GetRefreshButton(object indicator) {
        return "<img class='refresh' src='" + _requestInfo.AppImgPath + "../UI/Icons/refresh.png' alt='Refresh KPI' title='" + GetMessage("generic refresh") + "' onclick='" + GetClientClickScript("") + "' style='cursor: pointer;' />";
    }

    public string BuildChangeDisplay(object currentValue, object previousValue) {
        string classString = "<span class=\"{0}\">{1}</span>";
        decimal currentVal = Convert.ToDecimal(currentValue);
        decimal previousVal = Convert.ToDecimal(previousValue);
        decimal changeVal = (previousVal == 0) ? 0 : (((100 * currentVal) / previousVal) - 100);

        if (previousVal == 0) {
            if (currentVal > 0)
                return string.Format(classString, "KPIup", GetMessage("lbl indeterminate"));
            else if (currentVal < 0)
                return string.Format(classString, "KPIdown", GetMessage("lbl indeterminate"));
            else
                return string.Format(classString, "KPIna", GetMessage("lbl indeterminate"));
        } else {
            if (changeVal > 0)
                return string.Format(classString, "KPIup", changeVal.ToString("0.00") + "%");
            else if (changeVal < 0)
                return string.Format(classString, "KPIdown", changeVal.ToString("0.00") + "%");
            else
                return string.Format(classString, "KPIna", changeVal.ToString("0.00") + "%");
        }
    }

    private string GetLocalizedEnumName(EkEnumeration.KPIPeriod kpiPeriod) {
        string key = String.Empty;
        switch (kpiPeriod) {
            case EkEnumeration.KPIPeriod.Today:
                key = "today";
                break;

            case EkEnumeration.KPIPeriod.Yesterday:
                key = "lbl yesterday";
                break;

            case EkEnumeration.KPIPeriod.SameDayLastWeek:
                key = "lbl same day last week";
                break;

            case EkEnumeration.KPIPeriod.SameDayLastMonth:
                key = "lbl same day last month";
                break;

            case EkEnumeration.KPIPeriod.SameDayLastQuarter:
                key = "lbl same day last quarter";
                break;

            case EkEnumeration.KPIPeriod.SameDayLastYear:
                key = "lbl same day last year";
                break;

            case EkEnumeration.KPIPeriod.ThisWeek:
                key = "lbl this week";
                break;

            case EkEnumeration.KPIPeriod.LastWeek:
                key = "lbl last week";
                break;

            case EkEnumeration.KPIPeriod.LastWeekToDate:
                key = "lbl last week to date";
                break;

            case EkEnumeration.KPIPeriod.ThisMonth:
                key = "lbl this month";
                break;

            case EkEnumeration.KPIPeriod.LastMonth:
                key = "lbl last month";
                break;

            case EkEnumeration.KPIPeriod.LastMonthToDate:
                key = "lbl last month to date";
                break;

            case EkEnumeration.KPIPeriod.SameMonthLastQuarter:
                key = "lbl same month last quarter";
                break;

            case EkEnumeration.KPIPeriod.SameMonthLastYear:
                key = "lbl same month last year";
                break;

            case EkEnumeration.KPIPeriod.ThisYear:
                key = "lbl this year";
                break;

            case EkEnumeration.KPIPeriod.LastYear:
                key = "lbl last year";
                break;

            case EkEnumeration.KPIPeriod.LastYearToDate:
                key = "lbl last year to date";
                break;
        }
        return GetMessage(key);
    }

    protected string GetMessage(string key) {
        return ContentApi.EkMsgRef.GetMessage(key);
    }

    protected void btnForSubmit_Click(object sender, EventArgs e) {

    }

    protected void RegisterResource()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.CommonApi common = new CommonApi();
        Ektron.Cms.API.Css.RegisterCss(this, common.ApplicationPath + "explorer/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
   
}
