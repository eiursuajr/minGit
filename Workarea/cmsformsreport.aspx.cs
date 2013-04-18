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
using Ektron.Cms.Framework.UI;

public partial class cmsformsreport : System.Web.UI.Page
{
    #region  protected members
    protected long FormId = 0;
    protected long CurrentUserId = 0;
    protected string StartDate;
    protected string EndDate;
    protected Collection gtForm;
    protected Collection gtForms;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected Ektron.Cms.CommonApi m_refAPI = new Ektron.Cms.CommonApi();
    protected EkMessageHelper m_refMsg;
    protected string DefaultFormTitle = "";
    protected string DisplayType = "";
    protected string sFormDataIds = "";
    protected string DataType = "";
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected int ContentLanguage = -1;
    protected string Flag = "false";
    protected PermissionData Security_info;
    protected string Action = "";
    protected string ResultType = "";
    protected int EnableMultilingual = 0;
    protected Ektron.Cms.Modules.EkModule objForm;
    protected string strFolderID;
    protected long SelectedhId = 0;
    protected string QueryLang = "";
    protected string sPollFieldId = "";
    protected string sExcelPrefix = "<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\"><meta name=ProgId content=Excel.Sheet></head><body>";
    protected string sExcelSuffix = "</body></html>";
    protected Ektron.Cms.FormData objFormInfo;
    //protected string ReportType = string.Empty;
    //protected string strFormsURL = string.Empty;
    //protected string strFormsPath = string.Empty;
    #endregion
    //const bool SaveXmlAsFile = false;
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        BtnExport.Visible = false;
        this.uxPaging.Visible = false;
        if (Request.Params["HTTPS"] != "on" && (!string.IsNullOrEmpty(Request.Params["HTTPS"])))
        {
            //Adding lines below causes HTTPS not to work.
            //IE bug http://support.microsoft.com/default.aspx?scid=kb;en-us;812935
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
        }
        Response.Expires = -1;

        RegisterResources();

        //Put user code to initialize the page here
        StyleSheetJS.Text = m_refStyle.GetClientScript();
        m_refMsg = m_refContentApi.EkMsgRef;
        BtnExport.Text = m_refMsg.GetMessage("btn export");
        BtnExport.ToolTip = BtnExport.Text;
        lblStartDate.Text = m_refMsg.GetMessage("generic start date label");
        lblEndDate.Text = m_refMsg.GetMessage("generic end date label");
        litGetResult.Text = m_refMsg.GetMessage("lbl get report");
        ltrWaitMsg.Text = String.Format(m_refMsg.GetMessage("lbl please wait while we prepare your report"), this.totalPages.Value); //Please wait while we prepare your report of {0} pages. It might take a few minutes.
        CurrentUserId = m_refContentApi.UserId;
        ContentLanguage = m_refContentApi.ContentLanguage;
        AppImgPath = m_refContentApi.AppImgPath;
        AppPath = m_refContentApi.AppPath;
        //strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/");
        //strFormsPath = Server.MapPath(strFormsURL);
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            Action = EkFunctions.HtmlEncode(Request.QueryString["action"]);
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            FormId = Convert.ToInt64(Request.QueryString["id"]);
        if (!string.IsNullOrEmpty(Request["start_date"]))
            StartDate = Request["start_date"];
        if (!string.IsNullOrEmpty(Request["end_date"]))
            EndDate = Request["end_date"];
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"));
            return;
        }
        if ((StartDate != "") && (EndDate == null))
        {
            EndDate = Strings.FormatDateTime(DateTime.Now, DateFormat.ShortDate);
        }
        if (!string.IsNullOrEmpty(Request["flag"]))
            Flag = Request["flag"];
        if (Page.IsPostBack)
            Flag = "true";
        if (!string.IsNullOrEmpty(Request["data_type"]))
            DataType = Request["data_type"];
        if (!string.IsNullOrEmpty(Request["display_type"]))
            DisplayType = Request["display_type"];
        if (!string.IsNullOrEmpty(Request.QueryString["folder_id"]))
        {
            strFolderID = Request.QueryString["folder_id"];
        }
        if (!string.IsNullOrEmpty(Request.QueryString["hid"]))
        {
            SelectedhId = System.Convert.ToInt64(Request.QueryString["hid"]);
        }

        if (DisplayType == "")
        {
            DisplayType = "horizontal";
        }

        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }
        EnableMultilingual = m_refContentApi.EnableMultilingual;
        m_refContentApi.ContentLanguage = ContentLanguage;

        Security_info = m_refContentApi.LoadPermissions(FormId, "content", 0);
        objForm = m_refContentApi.EkModuleRef;
        if (SelectedhId > 0)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["FormTitle"]))
                DefaultFormTitle = Request.QueryString["FormTitle"];
        }
        else if ((Convert.ToString(FormId) != "") && (FormId > 0))
        {
            DefaultFormTitle = objForm.GetFormTitleById(FormId);
        }
        else
        {
            if (!string.IsNullOrEmpty(Request.QueryString["FormTitle"]))
                DefaultFormTitle = Request.QueryString["FormTitle"];
        }
        gtForms = objForm.GetAllFormInfo();
        if (Action == "delete")
        {
            string DelDataID = string.Empty;
            bool ret;
            if (!string.IsNullOrEmpty(Request.Form["delete_data_id"]))
                DelDataID = Request.Form["delete_data_id"];
            ret = objForm.PurgeFormData(FormId, DelDataID);
            Flag = "true";
        }
        Collection cHistData;

        ArrayList aReportHistoryId = null;
        ArrayList aReportTitle = null;
        cHistData = m_refContentApi.EkContentRef.GetHistoryListv2_0(FormId);
        if (cHistData.Count > 0)
        {
            aReportHistoryId = new ArrayList();
            aReportTitle = new ArrayList();
            foreach (Collection cData in cHistData)
            {
                if ("A" == cData["ContentStatus"].ToString())
                {
                    aReportHistoryId.Add(cData["HistoryID"]);
                    aReportTitle.Add(cData["ContentTitle"]);
                }
            }
        }

        if (Flag == "true")
        {
            FormResult.Text = LoadResult();
            if ("" == ExportResult.Text)
            {
                ExportResult.Text = FormResult.Text;  
            }
            ExportResult.Visible = false;
        }
        FormsReportToolBar();
        DisplayDateFields();
        DisplayHistoryOption(aReportHistoryId, aReportTitle);
        DisplaySelectReport(DisplayType, FormId);
        FillLiterals();
    }
    private void DisplayHistoryOption(ArrayList HistoryId, ArrayList HistoryTitle)
    {
        StringBuilder sbSelect = new StringBuilder();
        int i;
        if (HistoryId != null && HistoryId.Count > 1)
        {
            sbSelect.Append("<tr>" + "\r\n");
            sbSelect.Append("<td class=\"label\"><input type=\"hidden\" id=\"selhid\" name=\"selhid\" value=\"\"/>" + m_refMsg.GetMessage("lbl select legacy report") + ":</td>" + "\r\n");
            sbSelect.Append("<td colspan=\"3\">" + "\r\n");
            sbSelect.Append("<select name=\"selhistory\">" + "\r\n");
            for (i = 0; i <= HistoryId.Count - 1; i++)
            {
                sbSelect.Append("<option value=\"" + HistoryId[i] + "\"");
                if (SelectedhId == Convert.ToInt64(HistoryId[i].ToString()))
                {
                    sbSelect.Append(" selected ");
                }
                sbSelect.Append(">" + HistoryTitle[i]);
                sbSelect.Append(" (ver." + (HistoryId.Count - i) + ")");
                sbSelect.Append("</option>" + "\r\n");
            }
            sbSelect.Append("</select>" + "\r\n");
            for (i = 0; i <= HistoryId.Count - 1; i++)
            {
                sbSelect.Append("<input type=\"hidden\" id=\"hid_" + HistoryId[i] + "\" name=\"hid_" + HistoryId[i] + "\"");
                sbSelect.Append(" value=\"" + HistoryTitle[i] + "\"/>" + "\r\n");
            }
            sbSelect.Append("</td>" + "\r\n");
            sbSelect.Append("</tr>" + "\r\n");
        }
        else
        {
            sbSelect.Append("<input type=\"hidden\" id=\"selhid\" name=\"selhid\" value=\"none\"/>" + "\r\n");
        }
        SelectHistoryReport.Text = sbSelect.ToString();

    }
    private void DisplayDateFields()
    {
        EkDTSelector dateSchedule;
        dateSchedule = this.m_refContentApi.EkDTSelectorRef;
        dateSchedule.formName = "frmReport";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "start_date";
        dateSchedule.spanId = "start_date_span";
        if (!string.IsNullOrEmpty(StartDate))
        {
            dateSchedule.targetDate = DateTime.Parse(StartDate);
        }
        dtStart.Text = dateSchedule.displayCultureDate(true, dateSchedule.spanId, dateSchedule.formElement);
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (!string.IsNullOrEmpty(EndDate))
        {
            dateSchedule.targetDate = DateTime.Parse(EndDate);
        }
        dtEnd.Text = dateSchedule.displayCultureDate(true, dateSchedule.spanId, dateSchedule.formElement);
        if (string.IsNullOrEmpty(StartDate))
        {
            dtStart.Text += "<script>var oForm = document.forms[\'frmReport\']; clearDTvalue(oForm.start_date, \'start_date_span\', \'" + m_refMsg.GetMessage("dtselect: no date") + "\'); oForm.start_date_dow.value = \'\'; oForm.start_date_dom.value = \'\'; oForm.start_date_monum.value = \'\'; oForm.start_date_yrnum.value = \'\';</script>";
        }
        if (string.IsNullOrEmpty(EndDate))
        {
            dtEnd.Text += "<script>var oForm = document.forms[\'frmReport\']; clearDTvalue(oForm.end_date, \'end_date_span\', \'" + m_refMsg.GetMessage("dtselect: no date") + "\'); oForm.end_date_dow.value = \'\'; oForm.end_date_dom.value = \'\'; oForm.end_date_monum.value = \'\'; oForm.end_date_yrnum.value = \'\';</script>";
        }
    }
    private void FormsReportToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar("" + m_refMsg.GetMessage("alt view forms report") + " " + "\"" + DefaultFormTitle + "\"");
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("cmsform.aspx?action=ViewForm&form_id=" + FormId + "&LangType=" + ContentLanguage + "&folder_id=" + strFolderID), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		if (Flag == "true")
        {
            //If (Not Utilities.IsMac()) Then
            //	result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_export-nm.gif", "javascript:export_result()", m_refMsg.GetMessage("btn export raw data"), m_refMsg.GetMessage("btn export raw data"), ""))
            //End If
            if (Security_info.CanDelete)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt msg del form data"), m_refMsg.GetMessage("btn delete"), "onclick=\"return ConfirmDelete();return false;\" ", StyleHelper.DeleteButtonCssClass, true));
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("formreport", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;

    }
    private void DisplaySelectReport(string DisplayType, long FormId)
    {
        string strFormsURL;
        string strFormsPath;
        string strManifestFilePath;
        string strManifestURL;
        string strXsltFilePath;
        string strFieldList;
        StringBuilder sbSelect = new StringBuilder();

        strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/");
        strFormsPath = Server.MapPath(strFormsURL);
		//****Defect#54493****
		//If in web.config ek_UseSSL value is turned off the FormReportManifest.xml is timming out 
		//as it's considering URI schema as http instead of https.
		//In below if loop I've checked if URI schema is https then set RequestInformationRef.HttpsProtocol "on"
		if ((HttpContext.Current.Request.Url.Scheme.ToLower() == "https")) {
			m_refContentApi.RequestInformationRef.HttpsProtocol = "on";
		}
        strManifestURL = m_refContentApi.FullyQualifyURL(strFormsURL + "FormReportsManifest.xml");
        strManifestFilePath = strFormsPath + "FormReportsManifest.xml";
        strXsltFilePath = strFormsPath + "SelectFormReport.xslt";
        strFieldList = "";
        if (FormId > 0)
        {
            strFieldList = m_refContentApi.EkModuleRef.GetFormFieldListXml(FormId);
        }

        System.Xml.Xsl.XsltArgumentList objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
        if (Information.IsNumeric(DisplayType))
        {
            objXsltArgs.AddParam("selectedIndex", string.Empty, DisplayType);
        }
        if (m_refContentApi.ContentLanguage > 0)
        {
            LanguageData language_data;
            string strLang;
            language_data = (new SiteAPI()).GetLanguageById(m_refContentApi.ContentLanguage);
            strLang = language_data.BrowserCode;
            if (strLang != "")
            {
                objXsltArgs.AddParam("lang", string.Empty, strLang);
            }
        }
        if (strFieldList.Length > 0)
        {
            objXsltArgs.AddParam("manifest", string.Empty, strManifestURL);
        }
        sbSelect.Append("<select name=\"seldisplay\">");
        if (strFieldList.Length > 0)
        {
            sbSelect.Append(m_refContentApi.XSLTransform(strFieldList, strXsltFilePath, true, false, objXsltArgs, false));
        }
        else
        {
            sbSelect.Append(m_refContentApi.XSLTransform(strManifestFilePath, strXsltFilePath, true, true, objXsltArgs, false));
        }
        sbSelect.Append("</select>");
        SelectFormReport.Text = sbSelect.ToString();
    }
    private void PopulateForms()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        foreach (Collection tempLoopVar_gtForm in gtForms)
        {
            gtForm = tempLoopVar_gtForm;
            if (DefaultFormTitle == gtForm["FormTitle"].ToString())
            {
                result.Append("<option selected value=" + gtForm["FormID"] + ">" + gtForm["FormTitle"] + "</option>");
            }
            else
            {
                result.Append("<option value=" + gtForm["FormID"] + ">" + gtForm["FormTitle"] + "</option>");
            }
        }
    }

    private string DisplayReport(string ReportID, FormData FormInfo, FormSubmittedData[] Data, bool CanDelete)
    {
        return GeneratePartReportData(ReportID, FormInfo, Data, CanDelete, true);
    }

    private string GeneratePartReportData(string ReportID, FormData FormInfo, FormSubmittedData[] Data, bool CanDelete, bool NeedHeading)
    {
        string strFormsURL;
        string strFormsPath;
        string strManifestURL;
        string strManifestFilePath;
        string strXsltFilePath;
        System.Xml.XmlDocument objReport;
        System.Xml.XmlNode objNode;

        try
        {
            if (ReportID == "")
            {
                return "ERROR: Please select a report.";
            }
            else if (!Information.IsNumeric(ReportID))
            {
                return "ERROR: Invalid report ID: " + ReportID;
            }

            strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/");
            strFormsPath = Server.MapPath(strFormsURL);
            strManifestURL = m_refContentApi.FullyQualifyURL(strFormsURL + "FormReportsManifest.xml");
            strManifestFilePath = strFormsPath + "FormReportsManifest.xml";

            objReport = new System.Xml.XmlDocument();
            objReport.Load(strManifestFilePath);
            objNode = objReport.SelectSingleNode("/*/Reports/Report[" + ReportID + "]");
            if (objNode == null)
            {
                return "ERROR: Could not find report in FormReportsManifest.xml. Report: " + ReportID;
            }

            objNode = objNode.SelectSingleNode("xslt/@src");
            if (objNode == null)
            {
                return "ERROR: The report does not have a specified XSLT file in FormReportsManifest.xml. Report: " + ReportID;
            }
            strXsltFilePath = m_refContentApi.QualifyURL(strFormsPath, objNode.Value);

            string strXml;
            strXml = m_refContentApi.EkModuleRef.SerializeFormData(FormInfo, Data, null);

            System.Xml.Xsl.XsltArgumentList objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
            //'fill in the dynamic data for the fieldlist for this report (if apply).
            string sUpdatedXSLT = "";
            string sUpdatedFieldList = strFormsPath + "UpdateFieldList.xslt";
            objXsltArgs.AddParam("baseURL", string.Empty, m_refContentApi.FullyQualifyURL(""));
            objXsltArgs.AddParam("LangType", string.Empty, Convert.ToString(m_refContentApi.ContentLanguage));
            sUpdatedXSLT = m_refContentApi.XSLTransform(strXml, sUpdatedFieldList, true, false, objXsltArgs, false);
            strXml = m_refContentApi.XSLTransform("<root/>", sUpdatedXSLT, false, false, objXsltArgs, false, true);

            if (true == CanDelete)
            {
                //a version for export report - first page of a short report
                objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
                objXsltArgs.AddParam("canDelete", string.Empty, "false");
                objXsltArgs.AddParam("checkmarkUrl", string.Empty, m_refContentApi.FullyQualifyURL(m_refContentApi.QualifyURL(m_refContentApi.AppImgPath, "../UI/Icons/check.png")));
                objXsltArgs.AddParam("includeHeading", string.Empty, "true");
                string sExport;
                sExport = m_refContentApi.XSLTransform(strXml, strXsltFilePath, true, false, objXsltArgs, false);
                sExport = Regex.Replace(sExport, "</?(?i:pre)(.|\\n)*?>", string.Empty); //Defect # 45861 - Removing PRE tags
                ExportResult.Text = sExport; // sExcelPrefix + sExport + sExcelSuffix;
            }
            else
            {
                ExportResult.Text = "";
            }
            ExportResult.Visible = false;

            objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
            objXsltArgs.AddParam("canDelete", string.Empty, CanDelete ? "true" : "false");
            objXsltArgs.AddParam("checkmarkUrl", string.Empty, m_refContentApi.FullyQualifyURL(m_refContentApi.QualifyURL(m_refContentApi.AppImgPath, "../UI/Icons/check.png")));
            objXsltArgs.AddParam("includeHeading", string.Empty, NeedHeading ? "true" : "false");

            //if (SaveXmlAsFile)
            //{
            //    // Save XML as file for debugging purposes.
            //    string strXmlFilePath;
            //    strXmlFilePath = "Sample" + FormInfo.Title.Replace(" ", "") + "Data.xml";
            //    strXmlFilePath = m_refContentApi.QualifyURL(strFormsPath, strXmlFilePath);

            //    System.IO.StreamWriter sw = new System.IO.StreamWriter(strXmlFilePath);
            //    sw.Write(strXml);
            //    sw.Close();
            //    sw = null;
            //}
            return m_refContentApi.XSLTransform(strXml, strXsltFilePath, true, false, objXsltArgs, false);

        }
        catch (Exception)
        {
            return "";
            //				EkException.ThrowException(ex);
        }
        finally
        {
            objNode = null;
            objReport = null;
        }
    }
    private string escapeXML(string inString)
    {
        return m_refContentApi.EkModuleRef.escapeXML(inString);
    }
    private string LoadResult()
    {

        string msgNoData = m_refMsg.GetMessage("msg no data report");
        PageRequestData pagingData = new PageRequestData();
        pagingData.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        pagingData.CurrentPage = System.Convert.ToInt32(this.uxPaging.SelectedPage) + 1;  // pagingData is 1-based and uxPaging is 0-based
        if (Information.IsNumeric(DisplayType))
        {
            if (SelectedhId > 0)
            {
                objFormInfo = m_refContentApi.GetFormByHistoryId(SelectedhId);
            }
            else
            {
                objFormInfo = m_refContentApi.GetFormById(FormId);
            }
            resultsMessage.Text = "";
            if (objFormInfo == null)
            {
                resultsMessage.Text = "<p class=\"ui-state-highlight warningError\">ERROR: Could not find form. Form ID: " + FormId + "</p>";
                return "";
            }
            Hashtable objHistData;
            objHistData = m_refContentApi.EkModuleRef.GetFormFieldQuestionsById(FormId);
            if (objHistData.Count > 0)
            {
                //only provide the QueryLang as allContenLanguage (-1) for poll and survey reports.
                //only need data from all languages to match up the poll result on the site.
                QueryLang = "-1";

                if (SelectedhId > 0)
                {
                    string sTmp = "<ektdesignns_choices id=\"";
                    int iPos = -1;
                    int iPos2 = -2;

                    iPos = System.Convert.ToInt32(objFormInfo.Html.ToString().ToLower().IndexOf(sTmp));
                    if (iPos > -1)
                    {
                        iPos = iPos + sTmp.Length;
                        iPos2 = System.Convert.ToInt32(objFormInfo.Html.ToString().ToLower().IndexOf("\"", iPos));
                        if (iPos2 > -1)
                        {
                            sPollFieldId = (string)(objFormInfo.Html.ToString().ToLower().Substring(iPos, iPos2 - iPos));
                        }
                    }
                }
            }
            Ektron.Cms.FormSubmittedData[] aryData;            
            aryData = m_refContentApi.EkModuleRef.GetFormFieldDataById(FormId, StartDate, EndDate, -1, QueryLang, sPollFieldId, ref pagingData);
            if ((aryData == null) || 0 == aryData.Length)
            {
                this.uxPaging.Visible = false;
                resultsMessage.Text = "<p class=\"ui-state-highlight warningError\">" + msgNoData + "</p>";
                return "";
            }
            else
            {
                BtnExport.Visible = true;
                this.uxPaging.TotalPages = pagingData.TotalPages;
                if (this.uxPaging.TotalPages > 1)
                {
                    // only show paging if there are more than 1 page in total.
                    this.uxPaging.Visible = true;
                    this.totalPages.Value = this.uxPaging.TotalPages.ToString();
                    this.uxPaging.CurrentPageIndex = pagingData.CurrentPage - 1; // uxPaging is 0-based and pagingData is 1-based 
                }
            }

            int iData;
            sFormDataIds = "\'" + aryData[0].FormDataID + "\'";
            for (iData = 1; iData <= aryData.Length - 1; iData++)
            {
                sFormDataIds = sFormDataIds + ",\'" + aryData[iData].FormDataID + "\'";
            }

            return DisplayReport(DisplayType, objFormInfo, aryData, Security_info.CanDelete);
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        Collection objFormData;
        Collection cDatas;
        Collection cData;
        objForm = m_refContentApi.EkModuleRef;
        objFormData = new Collection();
        objFormData.Add(FormId, "FORM_ID", null, null);
        objFormData.Add(CurrentUserId, "USER_ID", null, null);
        objFormData.Add(StartDate, "START_DATE", null, null);
        objFormData.Add(EndDate, "END_DATE", null, null);
        cDatas = objForm.GetAllFormData(objFormData);
        if (cDatas.Count == 0)
        {
            result.Append("<table><tr><td>" + msgNoData + "</td></tr></table>");
            return (result.ToString());
        }
        long iCnt;
        long tmpFormId;
        long dataID = 0;
        string strHtml;
        bool bPaste;
        strHtml = "";
        tmpFormId = 0;
        if (DisplayType == "horizontal")
        {
            Collection fd1;
            //object fds1;
            Collection fds1;
            Collection fd2;
            //object fds2;
            Collection fds2;
            if (FormId.ToString() == "")
            {
                foreach (Collection tempLoopVar_gtForm in gtForms)
                {
                    gtForm = tempLoopVar_gtForm;

                    fds1 = objForm.GetFormFieldsById(FormId);
                    fds2 = objForm.TransferFormVariable(objFormData, ref pagingData);
                    if (fds2.Count > 0)
                    {
                        bPaste = false;
                        result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                        ;
                        result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                        result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                        result.Append("<tr height=20>");
                        foreach (Collection tempLoopVar_fd1 in fds1)
                        {
                            fd1 = tempLoopVar_fd1;
                            if (IsValidCol((string)(fd1["form_field_name"])))
                            {
                                result.Append("<td class=headcell valign=top>" + fd1["form_field_name"] + "</td>");
                            }
                        }
                        result.Append("<td class=headcell valign=top>Date Created</td></tr>");
                        foreach (Collection tempLoopVar_fd2 in fds2)
                        {
                            fd2 = tempLoopVar_fd2;
                            strHtml = "";
                            strHtml = strHtml + "<tr>";
                            foreach (Collection tempLoopVar_fd1 in fds1)
                            {
                                fd1 = tempLoopVar_fd1;
                                if (!Information.IsDBNull(fd2[fd1["form_field_name"]]))
                                {
                                    if (CheckDataType(fd2[fd1["form_field_name"]].ToString(), DataType) == true)
                                    {
                                        if (fd2[fd1["form_field_name"]].ToString() != "")
                                        {
                                            bPaste = true;
                                        }
                                    }
                                }
                                if (IsValidCol((string)(fd1["form_field_name"])))
                                {
                                    strHtml = strHtml + "<td valign=top>" + fd2[fd1["form_field_name"]] + "</td>";
                                }
                            }
                            if (bPaste == true)
                            {
                                strHtml = strHtml + "<td valign=top>" + fd2["date_created"] + "</td>";
                                strHtml = strHtml + "</tr>";
                                result.Append(strHtml);
                                bPaste = false;
                            }
                        }
                        result.Append("</table></td></tr></table></td></tr></table><hr>");
                    }
                }
            }
            else
            {
                fds1 = objForm.GetFormFieldsById(FormId);
                fds2 = objForm.TransferFormVariable(objFormData, ref pagingData);
                if (fds2.Count > 0)
                {
                    bPaste = false;
                    result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                    if (!string.IsNullOrEmpty(Request.Form["Form_Title"]))
                        result.Append("<table class=\"ektronGrid\" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " + Request.Form["Form_Title"] + "&nbsp;&nbsp;" + "" + "</td></tr></table>");
                    result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                    result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                    result.Append("<tr height=20>");
                    if (Security_info.CanDelete)
                    {
                        result.Append("<td class=headcell align=\"center\" valign=top nowrap=\"true\">(Delete)<br><input type=\"checkbox\" name=\"chkSelectAll\" onClick=\"SelectAll(this)\"></td>");
                    }
                    foreach (Collection tempLoopVar_fd1 in fds1)
                    {
                        fd1 = tempLoopVar_fd1;
                        if (IsValidCol((string)(fd1["form_field_name"])))
                        {
                            result.Append("<td class=headcell valign=top>" + fd1["form_field_name"] + "</td>");
                        }
                    }
                    result.Append("<td class=headcell valign=top>Date Created</td></tr>");
                    foreach (Collection tempLoopVar_fd2 in fds2)
                    {
                        fd2 = tempLoopVar_fd2;
                        strHtml = "";
                        //strHtml = strHtml & "<tr>"
                        //strHtml = strHtml & "<td valign=top><input type=""checkbox""/></td>"
                        foreach (Collection tempLoopVar_fd1 in fds1)
                        {
                            fd1 = tempLoopVar_fd1;
                            if (!Information.IsDBNull(fd2[fd1["form_field_name"]]))
                            {
                                if (CheckDataType(fd2[fd1["form_field_name"]].ToString(), DataType) == true)
                                {
                                    if (fd2[fd1["form_field_name"]].ToString() != "")
                                    {
                                        bPaste = true;
                                    }
                                }
                            }
                            if (IsValidCol((string)(fd1["form_field_name"])))
                            {
                                strHtml = strHtml + "<td valign=top>" + fd2[fd1["form_field_name"]] + "</td>";
                            }
                        }

                        if (bPaste == true)
                        {
                            if (sFormDataIds != "")
                            {
                                sFormDataIds = sFormDataIds + ",\'" + fd2["form_data_id"] + "\'";
                            }
                            else
                            {
                                sFormDataIds = "\'" + fd2["form_data_id"] + "\'";
                            }
                            if (Security_info.CanDelete)
                            {
                                strHtml = (string)("<tr><td align=\"center\" valign=top><input onClick=\"CheckIt(this)\" type=\"checkbox\" name=\"ektChk" + fd2["form_data_id"] + "\" id=\"ektChk" + fd2["form_data_id"] + "\"/></td>" + strHtml);
                            }
                            else
                            {
                                strHtml = (string)("<tr>" + strHtml); // we do not need <td>s around this.
                            }
                            strHtml = strHtml + "<td valign=top>" + fd2["date_created"] + "</td>";
                            strHtml = strHtml + "</tr>";
                            result.Append(strHtml);
                            bPaste = false;
                        }
                    }
                    result.Append("</table></td></tr></table></td></tr></table><hr>");
                }
            }
        }
        else if (DisplayType == "vertical")
        {
            if (FormId.ToString() == "")
            {
                foreach (Collection tempLoopVar_gtForm in gtForms)
                {
                    gtForm = tempLoopVar_gtForm;

                    result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                    result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                    result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                    result.Append("<tr height=20><td class=headcell align=center width=5% >Id</td><td class=headcell width=20% >Variable Name</td><td class=headcell width=55% >Value</td><td class=headcell width=25% >Date Submited</td></tr>");
                    iCnt = 1;
                    foreach (Collection tempLoopVar_cData in cDatas)
                    {
                        cData = tempLoopVar_cData;
                        if (CheckDataType(cData["FORM_FIELD_VALUE"].ToString(), DataType) == true)
                        {
                            bPaste = true;
                            if (DataType.ToLower() != "all")
                            {
                                if (cData["FORM_FIELD_VALUE"].ToString() == "")
                                {
                                    bPaste = false;
                                }
                                else
                                {
                                    bPaste = true;
                                }
                            }
                            if (bPaste)
                            {
                                if (tmpFormId.ToString() == cData["FORM_ID"].ToString())
                                {
                                    if ((int)(iCnt / 2) == (iCnt / 2))
                                    {
                                        result.Append("<tr class=evenrow><td valign=top align=center>" + cData["FORM_DATA_ID"] + "</td><td>" + cData["FORM_FIELD_NAME"] + "</td><td>" + cData["FORM_FIELD_VALUE"] + "</td><td>" + cData["DATE_CREATED"] + "</td></tr>");
                                    }
                                    else
                                    {
                                        result.Append("<tr><td valign=top align=center>" + cData["FORM_DATA_ID"] + "</td><td>" + cData["FORM_FIELD_NAME"] + "</td><td>" + cData["FORM_FIELD_VALUE"] + "</td><td>" + cData["DATE_CREATED"] + "</td></tr>");
                                    }
                                    iCnt++;
                                }
                            }
                        }
                    }
                    result.Append("</table></td></tr></table></td></tr></table><hr>");
                }
            }
            else
            {
                result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                if (!string.IsNullOrEmpty(Request.Form["form_title"]))
                    result.Append("<table class=\"ektronGrid\" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " + Request.Form["form_title"] + "</td></tr><tr><td align=left class=lbls>ID: " + FormId + "</td></tr></table>");
                result.Append("<table border=1 width=100% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                result.Append("<tr height=20>");
                if (Security_info.CanDelete)
                {
                    result.Append("<td class=headcell align=\"center\" width=1% nowrap=\"true\">(Delete)<br><input type=\"checkbox\" name=\"chkSelectAll\" onClick=\"SelectAll(this)\"></td>");
                }
                result.Append("<td class=headcell align=center width=5% >Id</td><td class=headcell width=20% >Variable Name</td><td class=headcell width=55% >Value</td><td class=headcell >Date Submited</td></tr>");
                iCnt = 1;
                foreach (Collection tempLoopVar_cData in cDatas)
                {
                    cData = tempLoopVar_cData;
                    strHtml = "";
                    if (CheckDataType(cData["FORM_FIELD_VALUE"].ToString(), DataType) == true)
                    {
                        bPaste = true;
                        if (DataType.ToLower() != "all")
                        {
                            if (cData["FORM_FIELD_VALUE"].ToString() == "")
                            {
                                bPaste = false;
                            }
                            else
                            {
                                bPaste = true;
                            }
                        }
                        if (bPaste)
                        {
                            if (sFormDataIds != "")
                            {
                                sFormDataIds = sFormDataIds + ",\'" + cData["form_data_id"] + "\'";
                            }
                            else
                            {
                                sFormDataIds = "\'" + cData["form_data_id"] + "\'";
                            }
                            if ((int)(iCnt / 2) == (iCnt / 2))
                            {
                                strHtml = "<tr class=evenrow>";
                            }
                            else
                            {
                                strHtml = "<tr>";
                            }
                            if (Security_info.CanDelete)
                            {
                                if (dataID != Convert.ToInt64(cData["FORM_DATA_ID"].ToString()))
                                {
                                    strHtml = strHtml + "<td align=\"center\" valign=top><input onClick=\"CheckIt(this)\" type=\"checkbox\" name=\"ektChk" + cData["FORM_DATA_ID"] + "\" id=\"ektChk" + cData["FORM_DATA_ID"] + "\"/></td>";
                                }
                                else
                                {
                                    strHtml = strHtml + "<td valign=top></td>";
                                }
                            }
                            if (IsValidCol(cData["FORM_FIELD_NAME"].ToString()))
                            {
                                result.Append(strHtml + "<td valign=top align=center>" + cData["FORM_DATA_ID"] + "</td><td valign=top>" + cData["FORM_FIELD_NAME"].ToString() + "</td><td valign=top>" + cData["FORM_FIELD_VALUE"].ToString() + "</td><td valign=top>" + cData["DATE_CREATED"] + "</td></tr>");
                            }

                            iCnt++;
                        }
                    }
                    if (dataID != Convert.ToInt64(cData["FORM_DATA_ID"].ToString()))
                    {
                        dataID = Convert.ToInt64(cData["FORM_DATA_ID"].ToString());
                    }
                }
                result.Append("</table></td></tr></table></td></tr></table><hr>");
            }
        }
        return (result.ToString());
    }
    private bool CheckDataType(string TEXT, string DataType)
    {
        bool returnValue;
        TEXT = TEXT.ToLower();
        returnValue = false;
        if (DataType == "All")
        {
            returnValue = true;
        }
        else if (DataType == "Date")
        {
            if (Information.IsDate(TEXT))
            {
                returnValue = true;
            }
        }
        else if (DataType == "Boolean")
        {
            if (TEXT == "1" || TEXT == "yes" || TEXT == "no" || TEXT == "0" || TEXT == "on" || TEXT == "off" || TEXT == "true" || TEXT == "false")
            {
                returnValue = true;
            }
        }
        else if (DataType == "Numeric")
        {
            if (Information.IsNumeric(TEXT))
            {
                returnValue = true;
            }
        }
        else if (DataType == "Text")
        {
            if (TEXT.Length > 0)
            {
                returnValue = true;
            }
        }
        return returnValue;
    }
    private bool IsValidCol(string VariableName)
    {
        bool returnValue;
        returnValue = true;
        if (VariableName.Length == 0)
        {
            return returnValue;
        }
        VariableName = VariableName.ToLower();

        if (".x" == VariableName.Substring(VariableName.Length - 2, 2))
        {
            returnValue = false;
        }
        else if (".y" == VariableName.Substring(VariableName.Length - 2, 2))
        {
            returnValue = false;
        }
        else if ("ecm" == VariableName.Substring(0, 3))
        {
            returnValue = false;
        }
        return returnValue;
    }
    protected void BtnExport_Click(object sender, System.EventArgs e)
    {
        if (this.result_type.Value != "export") return;
        Server.ScriptTimeout = 86400; 

        Ektron.Cms.FormSubmittedData[] aryData;
        int totalPages = 0;
        bool useFile = false;
        bool needHeading = true;
        string partReportData = string.Empty;
        string strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/");
        string strFormsPath = Server.MapPath(strFormsURL);
        string strDataFilePath = "ReportData" + DefaultFormTitle.Replace(" ", "") + "Data.htm";
        strDataFilePath = m_refContentApi.QualifyURL(strFormsPath, strDataFilePath);
        // make sure it start from a blank file.
        System.IO.File.Delete(EkFunctions.UrlEncode(strDataFilePath));
        if (Int32.TryParse(this.totalPages.Value, out totalPages))
        {
            if (totalPages > 1)
            {
                useFile = true;
                PageRequestData pagingData = new PageRequestData();
                pagingData.PageSize = 10000;          
                for (int i = 0; i < totalPages; i++)
                {
                    pagingData.CurrentPage = i + 1;
                    aryData = m_refContentApi.EkModuleRef.GetFormFieldDataById(FormId, StartDate, EndDate, -1, QueryLang, sPollFieldId, ref pagingData);
                    if (aryData != null && aryData.Length > 0)
                    {
                        string datastring = GeneratePartReportData(DisplayType, objFormInfo, aryData, false, needHeading);
                        partReportData += Regex.Replace(datastring, "</?(?i:pre)(.|\\n)*?>", string.Empty); //Defect # 45861 - Removing PRE tags

                        // Save long reporting data as file.
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strDataFilePath, true))
                        {
                            sw.Write(partReportData);
                        }
                        partReportData = string.Empty;

                        if (i == 0)
                        {
                            //reset the total page to the new paging data b/c page size has changed in the exprot.
                            totalPages = pagingData.TotalPages;
                            needHeading = false; // only the first page of export data need heading.
                        }
                    }
                }
            }
        }

        HttpContext.Current.ApplicationInstance.Response.Clear();
        HttpContext.Current.ApplicationInstance.Response.AddHeader("content-disposition", "attachment;filename=form_data_export.xls");
        Response.Charset = "";
        Response.ContentType = "application/vnd.xls";

        if (true == useFile)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(strDataFilePath))
            {
                Response.Write(sExcelPrefix + sr.ReadLine() + sExcelSuffix);
            }
        }
        else
        {
            ExportResult.Text = sExcelPrefix + ExportResult.Text + sExcelSuffix;
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            ExportResult.Visible = true;
            ExportResult.RenderControl(htmlWrite);
            ExportResult.Visible = false;
            Response.Write(stringWrite.ToString());
            Response.AddHeader("Accept-Header", System.Convert.ToString(System.Text.Encoding.ASCII.GetByteCount(ExportResult.Text)));
        }
        try
        {
            //****Note:User might get THreadAbortException error or "Internet Explorer Cannot Download" Error Message when they use an HTTPS.
            //Microsoft Recommends to call HttpContext.Current.ApplicationInstance.CompleteRequest method instead of Response.End.
            //However using CompleteRequest method appends content of the page apart from HTML representation of the XLS data.
            //It's up the user to pick the option to either go with Response.End or CompleteRequest method and can be changed
            //according to their requirement in the file [Workarea\cmsformsreport.aspx.vb]
            //http://support.microsoft.com/default.aspx?scid=kb;en-us;812935
            //http://support.microsoft.com/kb/312629
            if (Request.Params["HTTPS"] != "on" && (!string.IsNullOrEmpty(Request.Params["HTTPS"])))
            {
                Response.End();
            }
            else
            {
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            Response.Flush();
            HttpContext.Current.ApplicationInstance.Response.Clear();
            this.result_type.Value = "show";
        }
    }
    private void RegisterResources()
    {
        Packages.EktronCoreJS.Register(this);
        Packages.Ektron.Workarea.Core.Register(this);
        Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
    }
    private void FillLiterals()
    {
        ltrInvalidErrorMsg.Text = m_refMsg.GetMessage("js: invalid date format error msg");
        StringBuilder sb = new StringBuilder();
        string[] lists = sFormDataIds.Split(',');
        if (lists.Length >= 1 && lists[0] != string.Empty)
        {
            sb.Append("var arFormDataId = new Array(" + lists.Length + ");" + Environment.NewLine);
            for (int i = 0; i < lists.Length; i++)
            {
                sb.Append("arFormDataId[" + i + "] = " + lists[i] + ";" + Environment.NewLine);
            }
            ltrFormDataids.Text = sb.ToString();
        }
        ltrAlertStartDate.Text = m_refMsg.GetMessage("alert msg start date");
        ltrStrFolderID.Text = strFolderID;
        ltrFormID.Text = FormId.ToString();
        ltrContentLanguage.Text = ContentLanguage.ToString();
        ltrDefaultFormTitle.Text = DefaultFormTitle;
        ltrReportDel.Text = m_refMsg.GetMessage("alert msg report del");
        ltrDelFormData.Text = m_refMsg.GetMessage("alert msg del form data");
        ltrDelSelFormData.Text = m_refMsg.GetMessage("alert msg del sel form data");
    }
}