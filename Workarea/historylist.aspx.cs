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

public partial class historylist : System.Web.UI.Page
{
    public historylist()
    {
        AppImgPath = m_refContApi.AppImgPath;
    }

    private long m_contentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
    private ContentAPI m_refContApi = new ContentAPI();
    private long m_intId = -1;
    private Ektron.Cms.Common.EkMessageHelper m_refMsg;
    private string AppImgPath;
    private int m_intContentLanguage = 0;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            m_refMsg = m_refContApi.EkMsgRef;

            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
                }
                else
                {
                    if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContApi.ContentLanguage = m_intContentLanguage;
            }

            if (!(Request.QueryString["id"] == null))
            {
                m_intId = Convert.ToInt64(Request.QueryString["id"]);
            }

            if (m_intId != -1)
            {

                m_contentType = m_refContApi.EkContentRef.GetContentType(m_intId);

                switch (m_contentType)
                {

                    case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:

                        Ektron.Cms.Commerce.CatalogEntryApi m_refCatalogAPI = new Ektron.Cms.Commerce.CatalogEntryApi();
                        List<Ektron.Cms.Commerce.EntryData> entry_version_list = new List<Ektron.Cms.Commerce.EntryData>();
                        //causes cms400.sln compile to fail: entry_version_list = m_refCatalogAPI.GetVersionList(m_intId, m_intContentLanguage);
                        populateEntryGrid(entry_version_list);
                        break;

                    default:

                        ContentHistoryData[] content_history_list = (ContentHistoryData[])Array.CreateInstance(typeof(ContentHistoryData), 0);
                        content_history_list = m_refContApi.GetHistoryList(m_intId);
                        populateContentGrid(content_history_list);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #region Catalog Entry

    private void populateEntryGrid(List<EntryData> entry_version_list)
    {
        if (!(entry_version_list == null))
        {
            HistoryListGrid.Columns.Add(GetColumn("EkVersion", "<a href=\"#\" onclick=\"javascript:history.go(-1);\"><img title=\"" + m_refMsg.GetMessage("btn back") + "\" alt=\"" + m_refMsg.GetMessage("btn back") + "\" src=\"" + this.m_refContApi.AppPath + "images/ui/icons/back.png\" /></a>", HorizontalAlign.Left));
            HistoryListGrid.Columns.Add(GetColumn("TITLE", (string)(m_refMsg.GetMessage("hist list title") + "(" + "<img src=\"" + this.m_refContApi.AppPath + "Images/ui/icons/forward.png\" alt=\"Published\" title=\"Published\"/>" + "=published)"), HorizontalAlign.Left));
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            int minorRev;
            int numMajors;
            int pntr;
            minorRev = 0;
            numMajors = 0;
            pntr = 0;
            int[] minorarray = new int[entry_version_list.Count + 1];
            for (int i = 0; i <= (entry_version_list.Count - 1); i++)
            {
                if (entry_version_list[i].Status == "A")
                {
                    minorarray[numMajors] = minorRev;
                    numMajors++;
                    minorRev = 0;
                }
                else
                {
                    minorRev++;
                }
            }

            minorarray[numMajors] = minorRev; // This is really fist 1
            minorRev = minorarray[pntr];
            for (int i = 0; i <= (entry_version_list.Count - 1); i++)
            {
                dr = dt.NewRow();
                dr[1] = "<a class=\"history-list\" href=\"history.aspx?LangType=" + m_intContentLanguage + "&hist_id=" + entry_version_list[i].VersionId + "&Id=" + m_intId + "\" target=\"history_frame\" title=\"" + m_refMsg.GetMessage("view this version msg") + "\">";
                if (entry_version_list[i].Status == "A")
                {
                    minorRev = 0;
                    dr[0] = numMajors + ".0";
                    pntr++;
                    minorRev = minorarray[pntr];
                    numMajors--;
                    dr[1] += "<img src=\"" + this.m_refContApi.AppPath + "Images/ui/icons/forward.png\" border=\"0\" align=\"absbottom\" alt=\"Published\" title=\"Published\">";
                    if (entry_version_list[i].DateModified != null)
                    dr[1] += entry_version_list[i].DateModified.ToString();
                }
                else
                {
                    dr[0] = numMajors + "." + minorRev;
                    minorRev--;
                    dr[1] += "<div style=\'margin-left:15px;\'>" + entry_version_list[i].DateModified + "</div>";
                }

                dr[1] += "</a>";
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            HistoryListGrid.DataSource = dv;
            HistoryListGrid.DataBind();
        }
    }

    #endregion

    #region Other

    private void populateContentGrid(ContentHistoryData[] content_history_list)
    {
        if (!(content_history_list == null))
        {
            HistoryListGrid.Columns.Add(GetColumn("EkVersion", "<a href=\"#\" onclick=\"javascript:history.go(-1);\"><img title=\"" + m_refMsg.GetMessage("btn back") + "\" alt=\"" + m_refMsg.GetMessage("btn back") + "\" src=\"" + this.m_refContApi.AppPath + "images/ui/icons/back.png\" /></a>", HorizontalAlign.Left));
            HistoryListGrid.Columns.Add(GetColumn("TITLE", (string)(m_refMsg.GetMessage("hist list title") + "(" + "<img src=\"" + this.m_refContApi.AppPath + "Images/ui/icons/forward.png\" border=\"0\" align=\"absbottom\" alt=\"Published\" title=\"Published\">" + "=published)"), HorizontalAlign.Center));
            HistoryListGrid.BorderColor = System.Drawing.Color.White;
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
            dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
            int minorRev;
            int numMajors;
            int pntr;
            minorRev = 0;
            numMajors = 0;
            pntr = 0;
            int[] minorarray = new int[content_history_list.Length + 1];
            for (int i = 0; i <= content_history_list.Length - 1; i++)
            {
                if (content_history_list[i].Status == "A")
                {
                    minorarray[numMajors] = minorRev;
                    numMajors++;
                    minorRev = 0;
                }
                else
                {
                    minorRev++;
                }
            }

            minorarray[numMajors] = minorRev; // This is really fist 1
            minorRev = minorarray[pntr];
            for (int i = 0; i <= content_history_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[1] = "<a class=\"history-list\" href=\"history.aspx?LangType=" + m_intContentLanguage + "&hist_id=" + content_history_list[i].Id + "&Id=" + m_intId + "\" target=\"history_frame\" title=\"" + m_refMsg.GetMessage("view this version msg") + "\">";
                if (content_history_list[i].Status == "A")
                {
                    minorRev = 0;
                    dr[0] = numMajors + ".0";
                    pntr++;
                    minorRev = minorarray[pntr];
                    numMajors--;
                    dr[1] += "<img src=\"" + this.m_refContApi.AppPath + "Images/ui/icons/forward.png\" border=\"0\" align=\"absbottom\" alt=\"Published\" title=\"Published\">";
                    dr[1] += content_history_list[i].DisplayDateInserted;
                }
                else
                {
                    dr[0] = numMajors + "." + minorRev;
                    minorRev--;
                    dr[1] += "<div style=\'margin-left:15px;\'>" + content_history_list[i].DisplayDateInserted + "</div>";
                }

                dr[1] += "</a>";
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            HistoryListGrid.DataSource = dv;
            HistoryListGrid.DataBind();
        }

    }

    #endregion

    #region Private Helpers

    private System.Web.UI.WebControls.BoundColumn GetColumn(string dataField, string headerText, System.Web.UI.WebControls.HorizontalAlign alignment)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = dataField;
        colBound.HeaderText = headerText;
        colBound.Initialize();
        colBound.HeaderStyle.CssClass = "ektronTitlebar";
        colBound.ItemStyle.Wrap = true;
        colBound.ItemStyle.HorizontalAlign = alignment;
        colBound.ItemStyle.CssClass = "history-list";
        colBound.HeaderStyle.Height = Unit.Empty;
        return colBound;
    }

    #endregion
}


