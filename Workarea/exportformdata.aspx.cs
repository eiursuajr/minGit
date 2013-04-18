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
using Ektron.Cms.Modules;

public partial class exportformdata : System.Web.UI.Page
{
    protected long FormId;
    protected long CurrentUserId;
    protected string StartDate;
    protected string EndDate;
    protected Collection gtForm;
    protected Collection gtForms;
    protected EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string DefaultFormTitle = "";
    protected string DisplayType = "";
    protected string sFormDataIds = "";
    protected string DataType = "";
    protected string AppImgPath = "";
    protected int ContentLanguage = -1;
    protected string Flag = "false";
    protected PermissionData Security_info;
    protected string Action = "";
    protected string ResultType = "";
    protected int EnableMultilingual = 0;
    protected EkModule objForm;
    protected string FormTitle = "";
    protected string QueryLang = "";
    protected string FieldName = "";

    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        //Make sure the user is logged in. If not forward user to login page.
        if ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn", m_refContentApi.UserId) == false)
        {
            string strUrl;
            Session["RedirectLnk"] = Request.Url.AbsoluteUri;
            strUrl = "login.aspx?fromLnkPg=1";
            this.Response.ContentType = "";
            this.Response.Redirect(strUrl, true);
        }

        //Put user code to initialize the page here
        int ContentLanguage = m_refContentApi.ContentLanguage;
        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
            }
        }
        if (ContentLanguage == (int)Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refContentApi.ContentLanguage = ContentLanguage;
        }
        if (!(Request.QueryString["fieldname"] == null))
        {
            FieldName = Request.QueryString["fieldname"];
        }
        QueryLang = ContentLanguage.ToString();
        if (!(Request.QueryString["qlang"] == null))
        {
            QueryLang = Request.QueryString["qlang"];
        }
        if (!String.IsNullOrEmpty(Request["form_id"]))
        {
            FormId = Convert.ToInt64(Request["form_id"]);
        }
        StartDate = Request["start_date"];
        EndDate = Request["end_date"];
        Flag = Request["flag"];
        DataType = Request["data_type"];
        ResultType = Request["result_type"];
        CurrentUserId = m_refContentApi.UserId;
        DisplayType = Request["display_type"];
        FormTitle = Request["form_title"];
        m_refMsg = m_refContentApi.EkMsgRef;
        Security_info = m_refContentApi.LoadPermissions(FormId, "content", 0);
        Response.AddHeader("content-disposition", "attachment; filename=Form_Data_Export.xls");
        objForm = m_refContentApi.EkModuleRef;
        gtForms = objForm.GetAllFormInfo();

        Collection objFormData = new Collection();
        Collection cDatas;
        objFormData.Add(FormId, "FORM_ID", null, null);
        objFormData.Add(CurrentUserId, "USER_ID", null, null);
        objFormData.Add(StartDate, "START_DATE", null, null);
        objFormData.Add(EndDate, "END_DATE", null, null);
        objFormData.Add(QueryLang, "Query_Language", null, null);
        objFormData.Add(FieldName, "Field_Name", null, null);
        cDatas = objForm.GetAllFormData(objFormData);
        if (cDatas.Count == 0)
        {
            FormResult.Text = "<table><tr><td>" + m_refMsg.GetMessage("msg no data report") + "</td></tr></table>";
            return;
        }

        if (Information.IsNumeric(DisplayType))
        {
            //The following lines of code are extracted from dotnetjohn.com on "Export DataSets to Excel"
            //http://www.dotnetjohn.com/articles.aspx?articleid=36
            //first let's clean up the response.object
            Response.Clear();
            Response.Charset = "";
            //set the response mime type for excel
            Response.ContentType = "application/vnd.ms-excel";
            //create a string writer
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            //create an htmltextwriter which uses the stringwriter
            System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
            //instantiate a datagrid
            DataGrid dg = new DataGrid();
            //set the datagrid datasource to the dataset passed in
            dg.DataSource = objForm.GetAllFormRawData(objFormData).Tables[0];
            //bind the datagrid
            dg.DataBind();
            //tell the datagrid to render itself to our htmltextwriter
            dg.RenderControl(htmlWrite);
            //all that's left is to output the html
            Response.Write(stringWrite.ToString());
            Response.End();
        }
        else
        {
            FormResult.Text = LoadResult(objFormData, cDatas);
        }

    }
    private string LoadResult(Collection objFormData, Collection cDatas)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        Collection cData;

        int iCnt;
        object tmpFormId;
        object dataID;
        dataID = null;
        object strHtml;
        bool bPaste;
        strHtml = "";
        tmpFormId = 0;

        if (DisplayType == "horizontal")
        {
            Collection fd1 = null;
            Collection fds1 = null;
            Collection fd2 = null;
            Collection fds2 = null;
            PageRequestData PagingData = null;
            if (FormId.ToString() == "")
            {
                foreach (Collection tempLoopVar_gtForm in gtForms)
                {
                    gtForm = tempLoopVar_gtForm;

                    fds1 = objForm.GetFormFieldsById(FormId);
                    fds2 = objForm.TransferFormVariable(objFormData,ref PagingData);
                    if (fds2.Count > 0)
                    {
                        bPaste = false;
                        result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                        ;
                        result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                        result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                        result.Append("<tr height=20>");
                        foreach (Collection tempLoopVar_fd1 in (IEnumerable)fds1)
                        {
                            fd1 = tempLoopVar_fd1;
                            result.Append("<td class=headcell valign=top>" + fd1["form_field_name"] + "</td>");
                        }
                        result.Append("<td class=headcell valign=top>Date Created</td></tr>");
                        foreach (Collection tempLoopVar_fd2 in (IEnumerable)fds2)
                        {
                            fd2 = tempLoopVar_fd2;
                            strHtml = "";
                            strHtml = strHtml + "<tr>";
                            foreach (Collection tempLoopVar_fd1 in (IEnumerable)fds1)
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
                                strHtml = strHtml + "<td valign=top>" + fd2[fd1["form_field_name"]] + "</td>";
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
                fds2 = objForm.TransferFormVariable(objFormData, ref PagingData);
                if (fds2.Count > 0)
                {
                    bPaste = false;
                    bPaste = false;
                    result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                    result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " + FormTitle + "&nbsp;&nbsp;" + "" + "</td></tr></table>");
                    result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                    result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                    result.Append("<tr height=20>");
                    foreach (Collection tempLoopVar_fd1 in (IEnumerable)fds1)
                    {
                        fd1 = tempLoopVar_fd1;
                        result.Append("<td class=headcell valign=top>" + fd1["form_field_name"] + "</td>");
                    }
                    result.Append("<td class=headcell valign=top>Date Created</td></tr>");
                    foreach (Collection tempLoopVar_fd2 in (IEnumerable)fds2)
                    {
                        fd2 = tempLoopVar_fd2;
                        strHtml = "";
                        foreach (Collection tempLoopVar_fd1 in (IEnumerable)fds1)
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
                            strHtml = strHtml + "<td valign=top>" + fd2[fd1["form_field_name"]] + "</td>";
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
            if (FormId.ToString() == "")
            {
                foreach (Collection tempLoopVar_gtForm in gtForms)
                {
                    gtForm = tempLoopVar_gtForm;

                    result.Append("<table><tr><td>&nbsp;</td></tr></table>");
                    ;
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
                                    if (System.Convert.ToInt32(iCnt / 2) == (iCnt / 2))
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
                result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " + Request.QueryString["form_title"] + "</td></tr><tr><td align=left class=lbls>ID: " + FormId + "</td></tr></table>");
                result.Append("<table border=1 width=100% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>");
                result.Append("<tr><td><table border=0 width=100% cellspacing=0>");
                result.Append("<tr height=20>");

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
                            if (System.Convert.ToInt32(iCnt / 2) == (iCnt / 2))
                            {
                                result.Append("<tr class=evenrow><td align=left valign=top>" + cData["FORM_DATA_ID"] + "</td><td valign=top>" + cData["FORM_FIELD_NAME"] + "</td><td valign=top>" + cData["FORM_FIELD_VALUE"] + "</td><td valign=top>" + cData["DATE_CREATED"] + "</td></tr>");
                            }
                            else
                            {
                                result.Append("<tr><td align=left valign=top>" + cData["FORM_DATA_ID"] + "</td><td valign=top>" + cData["FORM_FIELD_NAME"] + "</td><td valign=top>" + cData["FORM_FIELD_VALUE"] + "</td><td valign=top>" + cData["DATE_CREATED"] + "</td></tr>");
                            }
                            iCnt++;
                        }
                    }
                    if (dataID != cData["FORM_DATA_ID"])
                    {
                        dataID = cData["FORM_DATA_ID"];
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
}