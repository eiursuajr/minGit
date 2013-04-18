<%@ Page ContentType="text/xml" Language="C#" AutoEventWireup="true" %><?xml version="1.0" encoding="utf-8"?>
<select>
<%
    string strParam = null;
    long nFolderID = 0;
    bool bRecursive = false;
    int nLangType = -1;

    strParam = Request.QueryString["id"];
    if (!string.IsNullOrEmpty(strParam) && long.TryParse(strParam, out nFolderID))
    {
        if (nFolderID < 0)
        {
            nFolderID = 0;
        }
    }
    
    strParam = Request.QueryString["recursive"];
    if (!string.IsNullOrEmpty(strParam) && ("1" == strParam || "true" == strParam.ToLower()))
    {
        bRecursive = true;
    }
    
    strParam = Request.QueryString["LangType"];
    if (!string.IsNullOrEmpty(strParam) && int.TryParse(strParam, out nLangType))
    {
        if (nLangType < -1)
        {
            nLangType = -1;
        }
    }

    Ektron.Cms.Controls.ListSummary objListSummary = new Ektron.Cms.Controls.ListSummary();
    objListSummary.Page = this;
    objListSummary.FolderID = nFolderID;
    objListSummary.Recursive = bRecursive;
    objListSummary.LanguageID = nLangType;
    // objListSummary.MaxResults = some number

    long iItem = 0;
    string strName = null;
    string strValue = null;
    for (iItem = 0; iItem <= objListSummary.EkItems.Length - 1; iItem++)
    {
        strName = objListSummary.EkItems[iItem].Title;
        strName = strName.Replace("&amp;#39;", "'");
        strValue = Convert.ToString(objListSummary.EkItems[iItem].Id);
        // strValue = objListSummary.EkItems(iItem).QuickLink
        Response.Write("<option value=\"" + strValue + "\">" + strName + "</option>\r\n");
    }
%>
</select>

