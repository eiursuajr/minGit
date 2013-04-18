<%@ Page ContentType="text/xml" Language="C#" AutoEventWireup="true" %>

<?xml version="1.0" encoding="utf-8" ?>
<select>
    <%
        string strParam = null;
        long nFolderID = 0;
        bool bRecursive = false;
        int nLangType = 0;
        nFolderID = 0;
        bRecursive = false;
        nLangType = -1;
        strParam = Request.QueryString["id"];
        if ((strParam != null) && Microsoft.VisualBasic.Information.IsNumeric(strParam))
        {
            nFolderID = Convert.ToInt64(strParam);
            if (nFolderID < 0)
                nFolderID = 0;
        }

        strParam = Request.QueryString["recursive"];
        if ((strParam != null) && ("1" == strParam || "true" == strParam.ToLower()))
        {
            bRecursive = true;
        }

        strParam = Request.QueryString["LangType"];
        if ((strParam != null) && Microsoft.VisualBasic.Information.IsNumeric(strParam))
        {
            nLangType = Convert.ToInt32(strParam);
            if (nFolderID < -1)
                nFolderID = -1;
        }

        Ektron.Cms.Controls.ListSummary objListSummary = new Ektron.Cms.Controls.ListSummary();
        objListSummary.Page = this;
        objListSummary.FolderID = nFolderID;
        objListSummary.Recursive = bRecursive;
        objListSummary.LanguageID = nLangType;
        int iItem = 0;
        string strName = null;
        string strValue = null;
        for (iItem = 0; iItem <= objListSummary.EkItems.Length - 1; iItem++)
        {
            strName = objListSummary.EkItems[iItem].Title;
            strName = strName.Replace("&#39;", "'");
            strValue = Convert.ToString(objListSummary.EkItems[iItem].Id);
            Response.Write("<option value=\"" + strValue + "\">" + strName + "</option>" + "/n");
        }

    %>
</select>
