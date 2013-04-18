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

public partial class DateTimeSelector : System.Web.UI.Page
{

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        SiteAPI AppUI = new SiteAPI();
        int LangId = 0;
        DateTime startdate;
        DateTime enddate;
        Ektron.Cms.Modules.EkModule ekm;
        string display;
        //Dim fN As String
        //Dim eN As String
        string targetDateString;
        EkDTSelector ekDts = new EkDTSelector(AppUI.RequestInformationRef);
        EkMessageHelper EkMsg = new EkMessageHelper(AppUI.RequestInformationRef);

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);

        ekm = AppUI.EkModuleRef;
		if (!String.IsNullOrEmpty(Request.QueryString["LangType"]))
        {

            int.TryParse(Request.QueryString["LangType"], out LangId);
            AppUI.ContentLanguage = LangId;
            AppUI.SetCookieValue("LastValidLanguageID", System.Convert.ToString(Convert.ToInt32(Request.QueryString["LangType"])));
        }
        else
        {
            if (AppUI.GetCookieValue("LastValidLanguageID") != "")
            {
                AppUI.ContentLanguage = int.Parse(AppUI.GetCookieValue("LastValidLanguageID"));
            }
        }

        switch (Strings.LCase(Request.QueryString["type"]))
        {
            case "date":
                display = "dtselectordate";
                targetDateString = DateTime.Now.ToString("d");
                break;
            case "time":
                display = "dtselectortime";
                targetDateString = (string)(ekDts.RoundMinutes(DateTime.Now, 5).ToString("t"));
                break;
            default:
                display = "dtselectordatetime";
                targetDateString = (string)(ekDts.RoundMinutes(DateTime.Now, 5).ToString("g"));
                break;
        }

        if (!String.IsNullOrEmpty(Request.QueryString["targetdate"]) && IsDateTime(Request.QueryString["targetdate"]))
        {
            JSGlobals.Text = "targetdate = \'" + EkFunctions.HtmlEncode(Request.QueryString["targetdate"]) + "\' ;";
        }
        else
        {
            JSGlobals.Text = "targetdate = \'" + targetDateString + "\' ;";
        }

        if (!IsBlankOrAlphaNumeric(EkFunctions.HtmlEncode(Request.QueryString["spanid"])))
        {
            Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"));
        }
        JSGlobals.Text += "spanid = \'" + EkFunctions.HtmlEncode(Request.QueryString["spanid"]) + "\' ;";
        if (!IsBlankOrAlphaNumeric(EkFunctions.HtmlEncode(Request.QueryString["formname"])))
        {
            Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"));
        }
        JSGlobals.Text += "formname = \'" + EkFunctions.HtmlEncode(Request.QueryString["formname"]) + "\' ;";
        if (!IsBlankOrAlphaNumeric(EkFunctions.HtmlEncode(Request.QueryString["formelement"])))
        {
            Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"));
        }
        JSGlobals.Text += "formelement = \'" + EkFunctions.HtmlEncode(Request.QueryString["formelement"]) + "\' ;";

        ekm = AppUI.EkModuleRef;
        if (!String.IsNullOrEmpty(Request.QueryString["sdate"]))
        {
            startdate = System.Convert.ToDateTime(Request.QueryString["sdate"]);
            startdate = DateAndTime.DateSerial(DateAndTime.DatePart(DateInterval.Year, startdate, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1), DateAndTime.DatePart(DateInterval.Month, startdate, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1), 1);
        }
        else
        {
            startdate = DateAndTime.DateSerial(DateAndTime.DatePart(DateInterval.Year, DateTime.Now, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1), DateAndTime.DatePart(DateInterval.Month, DateTime.Now, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1), 1);
        }

        enddate = DateAndTime.DateAdd(DateInterval.Month, 1, startdate);

        moDisplay.Text = ekm.OutputRenderedCalendarHTML(0, display, startdate, enddate, 0, 0);

        // QueryStrings coming in:
        // targetdate
        // spanid
        // formname
        // formelement

    }

    public bool IsBlankOrAlphaNumeric(string text)
    {

        if (text == null)
        {

            return true;

        }
        else if (text != null)
        {

            return IsAlphaNumeric(text.Replace("_", ""));

        }
        else
        {

            return false;

        }

    }

    public bool IsAlphaNumeric(string text)
    {

        return Regex.Match(text.Trim(), "^[a-zA-Z0-9]*$").Success;

    }

    public bool IsDateTime(string text)
    {

        if (text != null)
        {

            DateTime result;
            return DateTime.TryParse(text, out result);

        }
        else
        {

            return false;

        }

    }

}


