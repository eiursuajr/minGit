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
using System.Reflection;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class ContentAnalytics : System.Web.UI.Page
{
    protected ContentAPI cAPI = new Ektron.Cms.ContentAPI();
    protected string start_date = "";
    protected string end_date = "";
    protected CommonApi common;
    protected long contentid;
    protected string action;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string ContentLanguage;
    protected ContentData content_data;
    protected const string ControlPath = "controls/analytics/";
    protected void Page_Load(object sender, System.EventArgs e)
    {
        common = new Ektron.Cms.CommonApi();
        if (Convert.ToBoolean(common.RequestInformationRef.IsMembershipUser) || common.RequestInformationRef.UserId == 0)
        {
            Response.Redirect("blank.htm", false);
        }
        else
        {
            InitPage();
            BuildDateSelectors();
            BuildToolBar();
            LoadSelectedControl();
            RegisterResources();
        }

    }
    protected void InitPage()
    {
        m_refMsg = common.EkMsgRef;
        BuildNav();
        DefineView();

        // Set days to language
      ctlDay.ToolTip=  ctlDay.Text = "[" + common.EkMsgRef.GetMessage("day") + "]";
      ctlWeek.ToolTip=  ctlWeek.Text = "[" + common.EkMsgRef.GetMessage("week") + "]";
       ctlMonth.ToolTip= ctlMonth.Text = "[" + common.EkMsgRef.GetMessage("month") + "]";
      ctlYear.ToolTip=  ctlYear.Text = "[" + common.EkMsgRef.GetMessage("year") + "]";
      linkToday.ToolTip=  linkToday.Text = "[" + common.EkMsgRef.GetMessage("today") + "]";

      Button1.ToolTip=  Button1.Text = common.EkMsgRef.GetMessage("run custom range");
    }
    protected void BuildNav()
    {
        if (Request.QueryString["id"] == null)
        {
            this.navBar.Visible = true;
            string ViewType = Request.QueryString["type"];

            if (ViewType == null)
            {
                ViewType = "global";
            }
            else
            {
                ViewType = ViewType.ToLower();
            }

            string t_global = "tab_disabled";
            string t_activity = "tab_disabled";
            string t_content = "tab_disabled";
            string t_page = "tab_disabled";
            string t_referring = "tab_disabled";

            if ((string)(ViewType) == "global")
            {
                if (Request.QueryString["report"] == "2")
                {
                    t_activity = "tab_actived";
                }
                else
                {
                    t_global = "tab_actived";
                }
            }
            else if ((string)(ViewType) == "content")
            {
                t_content = "tab_actived";
            }
            else if ((string)(ViewType) == "page")
            {
                t_page = "tab_actived";
            }
            else if ((string)(ViewType) == "referring")
            {
                t_referring = "tab_actived";
            }

            //TODO: Ross - These need to be converted to jQuery tabs
            navBar.Text += "<table width=\"100%\">";
            navBar.Text += "<tr>";
            navBar.Text += "<td class=\"" + t_global + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=ContentAnalytics.aspx?type=global&report=1><b>&nbsp;" + common.EkMsgRef.GetMessage("site stats") + "&nbsp;</b></a></td>";
            navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
            navBar.Text += "<td class=\"" + t_activity + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=ContentAnalytics.aspx?type=global&report=2><b>&nbsp;" + common.EkMsgRef.GetMessage("site activity") + "&nbsp;</b></a></td>";
            navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
            navBar.Text += "<td class=\"" + t_content + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=ContentAnalytics.aspx?type=content><b>&nbsp;" + common.EkMsgRef.GetMessage("top content") + "&nbsp;</b></a></td>";
            navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
            navBar.Text += "<td class=\"" + t_page + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=ContentAnalytics.aspx?type=page><b>&nbsp;" + common.EkMsgRef.GetMessage("lbl top templates") + "&nbsp;</b></a></td>";
            navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
            navBar.Text += "<td class=\"" + t_referring + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=ContentAnalytics.aspx?type=referring><b>&nbsp;" + common.EkMsgRef.GetMessage("top referrers") + "&nbsp;</b></a></td>";
            navBar.Text += "<td class=\"tab_last\" width=\"91%\" nowrap>&nbsp;</td>";
            navBar.Text += "</tr>";
            navBar.Text += "</table>";
        }
        else
        {
            navBar.Visible = false;
        }
    }
    private void SetToTodayAndDay()
    {
        if (Session["CurrentView"] == null)
        {
            Session.Add("CurrentView", "day");
            ctlDay.Font.Bold = true;
        }
        else
        {
            Session["CurrentView"] = "day";
            ctlDay.Font.Bold = true;
        }

        if (Session["EndDate"] == null)
        {
            Session.Add("EndDate", DateTime.Today);
        }
        else
        {
            Session["EndDate"] = DateTime.Today;
        }

        if (Session["StartDate"] == null)
        {
            Session.Add("StartDate", DateTime.Today);
        }
        else
        {
            Session["StartDate"] = DateTime.Today;
        }
    }
    protected void DefineView()
    {
        if (!(Request.QueryString["landing"] == null))
        {
            int landingType = 0;
            try
            {
                landingType = Convert.ToInt32(Request.QueryString["landing"]);
            }
            catch (Exception)
            {
                landingType = 0;
            }

            if (landingType == 1)
            {
                SetToTodayAndDay();
                Response.Redirect((string)(Request.Url.ToString().Replace("&landing=1", "")));
            }
            else if (landingType == 2)
            {
                SetToTodayAndDay();
                Response.Redirect((string)(Request.Url.ToString().Replace("&landing=2", "")));
            }
        }

        if (Session["CurrentView"] == null)
        {
            Session.Add("CurrentView", "day");
            ctlDay.Font.Bold = true;
        }

        if (Session["EndDate"] == null)
        {
            Session.Add("EndDate", DateTime.Today);
        }

        if (Session["StartDate"] == null)
        {
            Session.Add("StartDate", DateTime.Today);
        }

        start_date = Convert.ToString(Session["StartDate"]);
        end_date = Convert.ToString(Session["EndDate"]);

        string sCurrView = (string)(Session["CurrentView"]);
        switch (sCurrView)
        {
            case "day":
                ctlDay.Font.Bold = true;
                break;
            case "week":
                ctlWeek.Font.Bold = true;
                break;
            case "month":
                ctlMonth.Font.Bold = true;
                break;
            case "year":
                ctlYear.Font.Bold = true;
                break;
            default:
                ctlDay.Font.Bold = true;
                break;
        }

        linkNext.Text = "[" + m_refMsg.GetMessage("next") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        linkPrevious.Text = "[" + m_refMsg.GetMessage("previous") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
    }
    public void DeactivateAll()
    {
        this.Global1.Visible = false;
        this.ContentReports1.Visible = false;
        this.Page1.Visible = false;
        this.Referring_url1.Visible = false;
    }
    public void ActivateControl(AnalyticsBase cont)
    {
        cont.Visible = true;
        cont.StartDateTime = StartDate;
        cont.EndDateTime = EndDate;
        this.Description.Text = cont.Description;
        Description.ToolTip = Description.Text;
        cont.CurrentView = CurrentView;
        cont.Initialize();
    }
    private void LoadSelectedControl()
    {
        string ControlType = "global";
        try
        {
            ControlType = (string)(Request.QueryString["type"].ToLower());
            if ((string)(ControlType) == "global")
            {
                DeactivateAll();
                ActivateControl(this.Global1);
                Description.Text = Global1.Description;
            }
            else if ((string)(ControlType) == "content")
            {
                DeactivateAll();
                ActivateControl(this.ContentReports1);
                Description.Text = ContentReports1.Description;
            }
            else if ((string)(ControlType) == "page")
            {
                DeactivateAll();
                ActivateControl(this.Page1);
                Description.Text = Page1.Description;
            }
            else if ((string)(ControlType) == "referring")
            {
                DeactivateAll();
                ActivateControl(this.Referring_url1);
                Description.Text = Referring_url1.Description;
            }
            else
            {
                DeactivateAll();
                ActivateControl(this.Global1);
                Description.Text = Global1.Description;
            }
        }
        catch (Exception)
        {
            DeactivateAll();
            ActivateControl(this.Global1);
        }
    }
    private void BuildDateSelectors()
    {
        Ektron.Cms.EkDTSelector dateSchedule = new Ektron.Cms.EkDTSelector(common.RequestInformationRef);

        this.lblQuickView.Text = common.EkMsgRef.GetMessage("quick view lbl") + ":";
        this.lblJumpTo.Text = common.EkMsgRef.GetMessage("jump to lbl") + ":";

        StringBuilder sbHtml = new StringBuilder();
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">");
        sbHtml.Append(common.EkMsgRef.GetMessage("generic start date label"));
        sbHtml.Append("</td>");
        sbHtml.Append("<td>");
        dateSchedule.formName = "form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "start_date";
        dateSchedule.spanId = "start_date_span";
        if (start_date != "")
        {
            try
            {
                dateSchedule.targetDate = StartDate;
            }
            catch (Exception)
            {
                start_date = "";
            }
        }
        sbHtml.Append(dateSchedule.displayCultureDate(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        sbHtml.Append("<tr>");
        sbHtml.Append("<td class=\"label\">");
        sbHtml.Append(common.EkMsgRef.GetMessage("generic end date label"));
        sbHtml.Append("</td>");
        sbHtml.Append("<td>");
        dateSchedule = new Ektron.Cms.EkDTSelector(common.RequestInformationRef);
        dateSchedule.formName = "form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (end_date != "")
        {
            try
            {
                dateSchedule.targetDate = EndDate;
            }
            catch (Exception)
            {
                end_date = "";
            }
        }
        sbHtml.Append(dateSchedule.displayCultureDate(true, "", ""));
        sbHtml.Append("</td>");
        sbHtml.Append("</tr>");
        lblViewing.Text = sbHtml.ToString();
    }
    private void BuildToolBar()
    {
        string refUrl;
        string type = "";
        string helpScreenAlias = "contentanalytics";

        refUrl = (string)("ContentAnalytics.aspx?type=" + Request.QueryString["type"]);

        System.Text.StringBuilder result;
        result = new System.Text.StringBuilder();
        string AppImgPath = cAPI.AppImgPath;
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Content Analytics"));
        result.Append("<table><tr>");

        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", refUrl, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
        }


        if (!(Request.QueryString["type"] == null))
        {
            type = Request.QueryString["type"];
            switch (type)
            {
                case "global":
                    helpScreenAlias = "contentanalytics_siteactivity";
                    break;
                case "content":
                    helpScreenAlias = "contentanalytics_topcontent";
                    break;
                case "page":
                    helpScreenAlias = "contentanalytics_toppages";
                    break;
                case "referring":
                    helpScreenAlias = "contentanalytics_topreferrers";
                    break;
                default:
                    helpScreenAlias = "contentanalytics";
                    break;
            }
        }
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(helpScreenAlias, ""));
        result.Append("</td>");

        result.Append("<td>");
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        StyleSheetJS.Text = m_refStyle.GetClientScript();
    }
    protected void SetControlDates(AnalyticsBase current)
    {
        try
        {
            current.StartDateTime = DateTime.Parse(start_date);
        }
        catch (Exception)
        {
            current.StartDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        }

        try
        {
            current.EndDateTime = DateTime.Parse(end_date);
        }
        catch (Exception)
        {
            current.EndDateTime = DateTime.Now;
        }
    }
    protected void linkPrevious_Click(object sender, System.EventArgs e)
    {
        if ((string)(CurrentView) == "day")
        {
            EndDate = EndDate.Date.AddDays(-1);
            StartDate = EndDate.Date;
        }
        else if ((string)(CurrentView) == "week")
        {
            EndDate = EndDate.Date.AddDays(-7);
            StartDate = EndDate.Date.AddDays(-7).AddDays(1);
        }
        else if ((string)(CurrentView) == "month")
        {
            EndDate = EndDate.Date.AddMonths(-1);
            StartDate = EndDate.Date.AddMonths(-1).AddDays(1);
        }
        else if ((string)(CurrentView) == "year")
        {
            EndDate = EndDate.Date.AddYears(-1);
            StartDate = EndDate.Date.AddYears(-1).AddDays(1);
        }
        Session["EndDate"] = EndDate;
        Session["StartDate"] = StartDate;
        SelectView();
        LoadSelectedControl();
    }
    protected void linkNext_Click(object sender, System.EventArgs e)
    {
        if ((string)(CurrentView) == "day")
        {
            StartDate = EndDate.Date.AddDays(1);
            EndDate = StartDate.Date;
        }
        else if ((string)(CurrentView) == "week")
        {
            StartDate = EndDate.Date.AddDays(1);
            EndDate = EndDate.Date.AddDays(7);
        }
        else if ((string)(CurrentView) == "month")
        {
            StartDate = EndDate.Date.AddDays(1);
            EndDate = EndDate.Date.AddMonths(1);
        }
        else if ((string)(CurrentView) == "year")
        {
            StartDate = EndDate.Date.AddDays(1);
            EndDate = EndDate.Date.AddYears(1);
        }
        Session["EndDate"] = EndDate;
        Session["StartDate"] = StartDate;
        SelectView();
        LoadSelectedControl();
    }
    protected void linkToday_Click(object sender, System.EventArgs e)
    {
        if ((string)(CurrentView) == "day")
        {
            EndDate = DateTime.Today;
            StartDate = DateTime.Today;
        }
        else if ((string)(CurrentView) == "week")
        {
            EndDate = DateTime.Today;
            StartDate = EndDate.AddDays(-7);
        }
        else if ((string)(CurrentView) == "month")
        {
            EndDate = DateTime.Today;
            StartDate = EndDate.AddMonths(-1);
        }
        else if ((string)(CurrentView) == "year")
        {
            EndDate = DateTime.Today;
            StartDate = EndDate.AddYears(-1);
        }
        Session["EndDate"] = EndDate;
        Session["StartDate"] = StartDate;
        SelectView();
        LoadSelectedControl();
    }
    private void SelectView()
    {
        UnselectView();
        string sCurrView = (string)(Session["CurrentView"]);
        switch (sCurrView)
        {
            case "day":
                ctlDay.Font.Bold = true;
                break;
            case "week":
                ctlWeek.Font.Bold = true;
                break;
            case "month":
                ctlMonth.Font.Bold = true;
                break;
            case "year":
                ctlYear.Font.Bold = true;
                break;
        }
        BuildDateSelectors();
    }
    private void UnselectView()
    {
        ctlDay.Font.Bold = false;
        ctlWeek.Font.Bold = false;
        ctlMonth.Font.Bold = false;
        ctlYear.Font.Bold = false;
    }
    protected void ctlDay_Click(object sender, System.EventArgs e)
    {
        CurrentView = "day";
        StartDate = EndDate;
        linkNext.Text = "[" + m_refMsg.GetMessage("next") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        linkPrevious.Text = "[" + m_refMsg.GetMessage("previous") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        SelectView();
        LoadSelectedControl();
    }
    protected void ctlWeek_Click(object sender, System.EventArgs e)
    {
        CurrentView = "week";
        StartDate = EndDate.AddDays(-7);
        linkNext.Text = "[" + m_refMsg.GetMessage("next") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        linkPrevious.Text = "[" + m_refMsg.GetMessage("previous") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        SelectView();
        LoadSelectedControl();
    }
    protected void ctlMonth_Click(object sender, System.EventArgs e)
    {
        CurrentView = "month";
        StartDate = EndDate.AddMonths(-1);
        linkNext.Text = "[" + m_refMsg.GetMessage("next") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        linkPrevious.Text = "[" + m_refMsg.GetMessage("previous") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        SelectView();
        LoadSelectedControl();
    }
    protected void ctlYear_Click(object sender, System.EventArgs e)
    {
        CurrentView = "year";
        StartDate = EndDate.AddYears(-1);
        linkNext.Text = "[" + m_refMsg.GetMessage("next") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        linkPrevious.Text = "[" + m_refMsg.GetMessage("previous") + " " + common.EkMsgRef.GetMessage(CurrentView) + "]";
        SelectView();
        LoadSelectedControl();
    }
    protected string CurrentView
    {
        get
        {
            try
            {
                return Session["CurrentView"].ToString();
            }
            catch (Exception)
            {
                return "day";
            }
        }
        set
        {
            try
            {
                Session["CurrentView"] = value;
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
    protected DateTime EndDate
    {
        get
        {
            try
            {
                return Convert.ToDateTime(Session["EndDate"]);
            }
            catch (Exception)
            {
                return DateTime.Today;
            }
        }
        set
        {
            try
            {
                end_date = value.ToString();
                Session["EndDate"] = value;
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
    protected DateTime StartDate
    {
        get
        {
            try
            {
                return Convert.ToDateTime(Session["StartDate"]);
            }
            catch (Exception)
            {
                return DateTime.Today;
            }
        }
        set
        {
            try
            {
                start_date = value.ToString();
                Session["StartDate"] = value;
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
    protected string StateString
    {
        get
        {
            return "view=" + this.CurrentView + "&end=" + EkFunctions.UrlEncode(Convert.ToString(EndDate)) + "&start=" + Page.Server.UrlEncode(Convert.ToString(StartDate));
        }
    }
    protected void Button1_Click(object sender, System.EventArgs e)
    {
        string str_start_date = Request.Form["start_date_iso"];
        string str_end_date = Request.Form["end_date_iso"];
        StartDate = DateTime.Parse(str_start_date);
        EndDate = DateTime.Parse(str_end_date);
        SelectView();
        LoadSelectedControl();
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronPlatformInfoJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDesignFormEntryJS);
        Ektron.Cms.API.JS.RegisterJS(this, common.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);

        Ektron.Cms.API.Css.RegisterCss(this, common.ApplicationPath + "explorer/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}