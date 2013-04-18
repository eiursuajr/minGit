using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Caching;
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

public partial class mediamanager : System.Web.UI.Page
{
    protected string AppName = "";
    protected EkMessageHelper m_refMsg;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        ContentAPI m_refApi = new ContentAPI();
        long locFolderId = 0;
        string sFullScreen = "off";
        System.Text.StringBuilder sFrameSet = new System.Text.StringBuilder();
        string actionType = Request.QueryString["actiontype"];
        string action = Request.QueryString["action"];
        string m_strEnhancedMetaSelect = "";
        string m_strMetadataFormTagId = "";
        string m_strSeparator = "";
        string m_strCurrentSelectionIds = "";
        string m_strCurrentSelectionTitles = "";
        string showThumb = "";
        bool showSelectWindow = false;
        string caller = string.Empty;

        if (!(Request.QueryString["enhancedmetaselect"] == null))
        {
            m_strEnhancedMetaSelect = Request.QueryString["enhancedmetaselect"];
        }
        if (!(Request.QueryString["metadataformtagid"] == null))
        {
            m_strMetadataFormTagId = Request.QueryString["metadataformtagid"];
        }
        if (!(Request.QueryString["separator"] == null))
        {
            m_strSeparator = Request.QueryString["separator"];
        }
        if (!(Request.QueryString["selectids"] == null))
        {
            m_strCurrentSelectionIds = Request.QueryString["selectids"];
        }
        if (!(Request.QueryString["selecttitles"] == null))
        {
            m_strCurrentSelectionTitles = Request.QueryString["selecttitles"];
        }
        if(!string.IsNullOrEmpty(Request.QueryString["caller"]))
        {
            caller = "&caller=" + Request.QueryString["caller"];
        }
        if (action == null)
        {
            action = "";
        }
        if (action == "")
        {
            action = "ViewLibraryByCategory";
        }
        string sEditorName = Request.QueryString["EditorName"];
        string LibType = "all";

        string scope = "";
        string autonav = "";
        string tempAutoNav = "";
        string sLeftFrame;
        string sRightFrame;
        string sNameId;
        string retField = "";

        AppName = m_refApi.AppName;
        int DEntryLink = 0;
        m_refMsg = m_refApi.EkMsgRef;

        scope = Request.QueryString["scope"];
        autonav = Request.QueryString["autonav"];
        retField = Request.QueryString["retfield"];
        if (retField != null)
        {
            if (retField != "")
            {
                retField = (string)("&retfield=" + retField);
            }
        }
        showThumb = true.ToString();
        if (Request.QueryString["showthumb"] != null)
        {
            if (Request.QueryString["showthumb"] == "false")
            {
                showThumb = "&showthumb=false";
            }
        }
        if (!(Request.QueryString["fullscreen"] == null))
        {
            sFullScreen = Request.QueryString["fullscreen"];
        }

        litTitle.Text = AppName + " " + m_refMsg.GetMessage("library page html title") + " " + Ektron.Cms.CommonApi.GetEcmCookie()["username"];

        if (!(Request.QueryString["defaultFolderId"] == null))
        {
            if (Request.QueryString["defaultFolderId"] != "")
            {
                locFolderId = Convert.ToInt64(Request.QueryString["defaultFolderId"]);
            }
        }
        else if (!(Request.QueryString["autonav"] == null))
        {
            tempAutoNav = Request.QueryString["autonav"];
            if (Information.IsNumeric(tempAutoNav))
            {
                Int64.TryParse(tempAutoNav, out locFolderId);
                autonav = (string)((m_refApi.EkContentRef).GetFolderPath(locFolderId));
                if (autonav.Length == 0)
                {
                    autonav = "\\";
                }
            }
            else
            {
                locFolderId = (m_refApi.EkContentRef).GetFolderID(tempAutoNav);
            }
        }
        if (locFolderId < 0)
        {
            locFolderId = 0;
        }
        if (!(Request.QueryString["ldata"] == null))
        {
            //setting to indicate a Data Entry link from within editor
            if (Request.QueryString["ldata"] != "")
            {
                DEntryLink = Convert.ToInt32(Request.QueryString["ldata"]);
            }
        }
        if (!(Request.QueryString["text"] == null))
        {
            //setting to indicate a Data Design link from within editor
            if ("imgonlyselect" == Strings.LCase(Request.QueryString["text"]))
            {
                DEntryLink = 1;
            }
            else if ("imgpopupmediaselect" == Strings.LCase(Request.QueryString["text"]))
            {
                DEntryLink = 1;
            }
        }

        if (Request.QueryString["scope"] == "images")
        {
            LibType = "images";
            Session["LibCategory"] = LibType;
        }
        else if (Request.QueryString["scope"] == "files")
        {
            LibType = "files"; // Note: was erroneously set to "images" - this caused faulty behavior when choosing a file for enhanced metadata. -BCB, Jan/12/2005
            Session["LibCategory"] = LibType;
        }
        else
        {
            if (Request.QueryString["type"] != "")
            {
                LibType = Request.QueryString["type"];
                Session["LibCategory"] = LibType;
            }
            else
            {
                LibType = Session["LibCategory"].ToString();
            }

            if (LibType == "")
            {
                LibType = "images";
                Session["LibCategory"] = LibType;
            }
        }

        if (sFullScreen == "off")
        {
            sLeftFrame = "\'medialist.aspx?action=" + action + "&actionType=" + actionType + "&autonavfolder=" + locFolderId + "&scope=" + scope + "&autonav=" + EkFunctions.UrlEncode(autonav) + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditorName + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&selectids=" + m_strCurrentSelectionIds + "&selecttitles=" + m_strCurrentSelectionTitles + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + retField + showThumb + "\'";
            sNameId = "mediainsert";
            sFrameSet.Append("<frameset cols=\'250,*\' border=\'1\' class=\'library\' frameSpacing=\'1\'>");
            if (m_strEnhancedMetaSelect.Length > 0)
            {
                // Note: enhanced metadata gets an extra "selection" frame, to allow
                // users the ability to choose multiple metadata associations:
                showSelectWindow = true;
            }
        }
        else
        {
            LibType = Request.QueryString["type"];
            sLeftFrame = "\'\'";
            sNameId = "mediauploader";
            sFrameSet.Append("<frameset cols=\'1,*\' border=\'1\' class=\'library\' frameSpacing=\'1\'>");
        }
        if ((!string.IsNullOrEmpty(Request.QueryString["productmode"])) && Request.QueryString["productmode"].ToLower() == "true")
        {
            Session["LibraryProductMode"] = "true";
        }
        else
        {
            Session["LibraryProductMode"] = "false";
        }

        sRightFrame = "\'" + sNameId + ".aspx?scope=" + scope + caller + "&action=" + action + "&folder=" + locFolderId + "&type=" + LibType + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditorName + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&selectids=" + m_strCurrentSelectionIds + "&selecttitles=" + m_strCurrentSelectionTitles + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + retField + showThumb + "\'";
        sFrameSet.Append("<frame id=\'medialist\' name=\'medialist\' src=" + sLeftFrame + " marginwidth=\'2\' marginheight=\'2\'");
        sFrameSet.Append("	scrolling=\'auto\' frameborder=\'1\' runat=\'server\'/>");
        if (showSelectWindow)
        {
            sFrameSet.Append("<frameset rows=\'*,200\' border=\'1\' class=\'library\' frameSpacing=\'1\'>");
        }
        sFrameSet.Append("<frame id=\'" + sNameId + "\' name=\'" + sNameId + "\' src=" + sRightFrame + " marginwidth=\'2\' marginheight=\'2\'");
        sFrameSet.Append("	scrolling=\'auto\' frameborder=\'0\' runat=\'server\'/>");
        if (showSelectWindow)
        {
            sFrameSet.Append("<frame id=\'MediaSelect\' name=\'MediaSelect\' src=\'MediaSelect.aspx\' marginwidth=\'2\' marginheight=\'2\'");
            sFrameSet.Append("	scrolling=\'auto\' frameborder=\'0\' runat=\'server\'/>");
            sFrameSet.Append("</frameset>");
        }
        sFrameSet.Append("</frameset>");
        litFrameset.Text = sFrameSet.ToString();

        m_refApi = null;
    }
}