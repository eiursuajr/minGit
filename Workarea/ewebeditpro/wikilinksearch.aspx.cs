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
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.API.Search;
using Ektron.Cms;


public partial class wikilinksearch : System.Web.UI.Page
{


    protected CommonApi m_commonApi = new CommonApi();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.Response.ContentType = "text/xml";
        Page.Response.Clear();
        Page.Response.BufferOutput = true;

        string text = "";
        int pageNumber = 1;
        int totalPages = 0;
        int MaxResults = 0;
        int iLoop = 1;
        string strID = "";
        StringBuilder strFields = new StringBuilder();
        string strOnclick = "";
        long contentId = 0;
        long selectedId = -1;
        int languageID = m_commonApi.RequestInformationRef.ContentLanguage;
        m_refMsg = m_commonApi.EkMsgRef;

        Ektron.Cms.ContentAPI content_api = null;
        content_api = new Ektron.Cms.ContentAPI();
        if (content_api.GetCookieValue("LastValidLanguageID") != "" && Convert.ToInt32(content_api.GetCookieValue("LastValidLanguageID")) != -1)
        {
            languageID = Convert.ToInt32(content_api.GetCookieValue("LastValidLanguageID"));
        }


        text = Request.QueryString["text"];
        if (!string.IsNullOrEmpty(Request.QueryString["pnum"]) && Convert.ToInt32(Request.QueryString["pnum"]) > 0)
        {
            pageNumber = Convert.ToInt32(Request.QueryString["pnum"]);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["cid"]))
        {
            contentId = Convert.ToInt64(Request.QueryString["cid"]);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["selectedid"]))
        {
            selectedId = Convert.ToInt64(Request.QueryString["selectedid"]);
        }

        Ektron.Cms.API.Search.SearchManager search = new Ektron.Cms.API.Search.SearchManager();
        SearchRequestData requestData = new SearchRequestData();
        requestData.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.all;
        requestData.SearchText = text;
        requestData.PageSize = 10;
        requestData.LanguageID = languageID;
        requestData.CurrentPage = pageNumber;
        int resultCount = 0;
        SearchResponseData[] result = search.Search(requestData, HttpContext.Current, ref resultCount);
        StringBuilder str = new StringBuilder();
        StringBuilder strRet = new StringBuilder();
        int tmpCount = 0;
        string strLink = "";
        string[] arLink = null;
        MaxResults = requestData.PageSize;
        if (resultCount != 0)
        {
            str.Append("<table width=\"100%\" class=\"ektronGrid\">");
            foreach (SearchResponseData data in result)
            {
                strLink = "";
                strID = (string)("ek_sel_cont" + iLoop);
                if (data.QuickLink.IndexOf("http://") > -1)
                {
                    strLink = (string)(data.QuickLink);
                    strLink = strLink.Substring(7);
                    strLink = "http://" + strLink.Replace("\'", "\\\'").Replace("//", "/");
                }
                if (strLink.ToLower().IndexOf("window.open") < 0)
                {
                    arLink = strLink.Split("?".ToCharArray());
                    if (arLink.Length > 1)
                    {
                        strLink = arLink[0];
                        arLink = arLink[1].Split("&amp;".ToCharArray());
                        foreach (string val in arLink)
                        {
                            if (val.IndexOf("terms=") == -1)
                            {
                                if (strLink == "")
                                {
                                    strLink = val;
                                }
                                else
                                {
                                    if (strLink.IndexOf("?") < 0)
                                    {
                                        strLink = strLink + "?" + val;
                                    }
                                    else
                                    {
                                        strLink = strLink + "&" + val;
                                    }
                                }
                            }
                        }
                    }
                }

                strOnclick = "SelectContent(\'" + strID + "\',\'" + strLink + "\')";
                str.Append("<tr><td valign=\"top\" style=\"width:1%;\" valign=\"top\">");
                str.Append("<input type=\"radio\" ");
                if (selectedId != -1 && selectedId == data.ContentID)
                {
                    str.Append(" checked=\"true\" ");
                }

                if (data.ContentID == contentId)
                {
                    str.Append(" disabled ");
                }

                str.Append("onclick=\"" + strOnclick + "\" id=\"");
                str.Append(strID);
                str.Append("\" name=\"ek_sel_cont\"/></td><td valign=\"top\">");
                str.Append("<span onclick=\"" + strOnclick + "\" class=\"title\">");
                str.Append(data.Title).Append("</span><br/>");
                str.Append("<span onclick=\"SelectContent(\'" + strID + "\',\'" + strLink + "\')\" class=\"summary\">");
                if (data.ContentType != 2 && data.ContentType != 4)
                {
                    str.Append(data.Summary.Replace("<p> </p>", "").Replace("<p>&nbsp;</p>", "").Replace("<p>&#160;</p>", ""));
                }

                str.Append("</span></td></tr>");
                strFields.Append(",").Append(strID);
                iLoop++;
            }
        }

        if (resultCount > 0 && MaxResults > 0)
        {
            if (resultCount % MaxResults == 0)
            {
                totalPages = resultCount / MaxResults;
            }
            else
            {
                tmpCount = System.Convert.ToInt32(resultCount / MaxResults);
                if (tmpCount * MaxResults < resultCount)
                {
                    totalPages = tmpCount + 1;
                }
                else
                {
                    totalPages = tmpCount;
                }
            }
        }

        str.Append("</table>");
        if (totalPages == 0)
        {
            strRet = new StringBuilder();
            strRet.Append("<content>");
            strRet.Append("<table style=\"width:100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
            strRet.Append("<tr><td>" + m_refMsg.GetMessage("alt no related content") + "</td></tr>");
            strRet.Append("</table>");
            strRet.Append("</content>");
            strRet.Append("<totalPages>").Append(totalPages).Append("</totalPages>");
        }
        else
        {
            strRet.Append("<content>");
            strRet.Append("<div class=\"header\">" + m_refMsg.GetMessage("lbl total") + ": ").Append(resultCount).Append("<br/>");
            strRet.Append("" + m_refMsg.GetMessage("page lbl") + ": ").Append(pageNumber).Append(" " + m_refMsg.GetMessage("lbl of") + " ").Append(totalPages).Append("</div>");
            strRet.Append(str.ToString());
            strRet.Append("</content>");
            strRet.Append("<totalPages>").Append(totalPages).Append("</totalPages>");
        }

        Response.Write(strRet.ToString());
    }
}


