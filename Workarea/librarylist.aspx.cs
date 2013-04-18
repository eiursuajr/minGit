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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms.Common;
using Microsoft.Security.Application;

public partial class librarylist : System.Web.UI.Page
{
    protected Ektron.Cms.UI.CommonUI.ApplicationAPI AppUI = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
    protected CommonApi m_refApi = new CommonApi();
    protected string AppPath;
    protected string enableQDOparam;
    protected string AppImgPath;
    protected string sitePath;
    protected object gtMsgObj;
    protected object gtMess;
    protected long CurrentUserId;
    protected string actionType;
    protected object DbSecondObj;
    protected object cDbSecondRecs;
    protected object cDbSecondRec;
    protected Ektron.Cms.Site.EkSite SiteObj;
    protected System.Collections.Hashtable cPerms;
    protected Collection cAllFolders;
    protected string iOrderBy;
    protected Ektron.Cms.Content.EkContent cDbObj;
    protected object cDbRecs;
    protected object cDbRec;
    protected object ErrorString;
    protected string scope;
    protected int ContentLanguage;
    protected string EnableMultilingual;
    protected string RetField;
    protected Ektron.Cms.Common.EkMessageHelper MsgHelper;
    protected bool m_bAjaxTree = false;
    protected string QueryType = "";
    protected string disableLinkManage = "";
    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected const int CONTENT_LANGUAGES_UNDEFINED = 0;
    protected string myTemp;
    protected int MyButtonName = 100;
    protected bool DisplayTransText = false;

    public librarylist()
    {
        MsgHelper = AppUI.EkMsgRef;
        CurrentUserId = AppUI.UserId;
        AppImgPath = AppUI.AppImgPath;
        AppPath = AppUI.AppPath;
        sitePath = AppUI.SitePath;
    }


    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            ltrSytleScript.Text = (new StyleHelper()).GetClientScript();
            if (AppUI.UserId == 0 || AppUI.RequestInformationRef.IsMembershipUser == 1)
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }
            else
                ReadQueryString();
        }
        catch (Exception Ex)
        {

            EkException.LogException(Ex);
        }
       
    }
    public void ReadQueryString()
    {
        if ((m_refApi.TreeModel == 1))
        {
            m_bAjaxTree = true;
        }
        if (Ektron.Cms.CommonApi.GetEcmCookie().HasKeys)
        {
            CurrentUserId = Convert.ToInt64(Ektron.Cms.CommonApi.GetEcmCookie()["user_id"]);
        }
        else
        {
            CurrentUserId = 0;
        }
        RetField = Request.QueryString["RetField"];
        if (!String.IsNullOrEmpty(RetField))
        {
            RetField = (string)("&RetField=" + AntiXss.HtmlEncode(RetField).Replace("&amp;", "&"));
        }

        if (!String.IsNullOrEmpty(Request.QueryString["type"]))
        {
            QueryType = ("&type=" + Request.QueryString["type"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["disableLinkManage"]))
        {
            disableLinkManage = ("&disableLinkManage=" + Request.QueryString["disableLinkManage"]);
        }
        bool showQDContentOnly = false;
        if (!String.IsNullOrEmpty(Request.QueryString["qdo"]))
        {
            showQDContentOnly = (Request.QueryString["qdo"] == "1");
        }
        if (showQDContentOnly)
        {
            enableQDOparam = "&qdo=1";
        }
        Page.Title = (AppUI.AppName + (" " + (MsgHelper.GetMessage("library page html title") + (" " + Ektron.Cms.CommonApi.GetEcmCookie()["username"]))));
        if (!String.IsNullOrEmpty(Request.QueryString["scope"]))
        {
            scope = Convert.ToString(Request.QueryString["scope"]).ToLower();
        }
        else
        {
            scope = string.Empty;
        }
        if (!String.IsNullOrEmpty(Request.QueryString["actionType"]))
        {
            actionType = Request.QueryString["actionType"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["iOrderBy"]))
        {
            iOrderBy = Request.QueryString["iOrderBy"];
        }
        else
        {
            iOrderBy = "Title";
        }
        string AutoNavStr;
        if (!String.IsNullOrEmpty(Request.QueryString["autonav"]))
        {
            AutoNavStr = Request.QueryString["autonav"].ToString();
        }
        else
        {
            AutoNavStr = "\\";
        }
        if (!m_bAjaxTree)
        {
            Response.Write(("<script language=\"javascript\">" + "\r\n"));
            Response.Write(("<!--" + "\r\n"));
            Response.Write(("function OpenLibFolder1() {" + "\r\n"));
            Response.Write(("if (top.GetLoadStatus()) {" + "\r\n"));
            Response.Write(("OpenFolder(\""
                            + (AutoNavStr.Replace("\\", "\\\\") + ("\", true);" + "\r\n"))));
            Response.Write(("}" + "\r\n"));
            Response.Write(("else {" + "\r\n"));
            Response.Write(("setTimeout(\"OpenLibFolder1()\", 100);" + "\r\n"));
            Response.Write(("}" + "\r\n"));
            Response.Write(("}" + "\r\n"));
            Response.Write(("OpenLibFolder1();" + "\r\n"));
            Response.Write(("//--></script>" + "\r\n"));
        }
    }
    public void OutputLibraryFolders(int level, int Parent)
    {
        Ektron.Cms.Common.EkEnumeration.FolderDestinationType[] DestType = new Ektron.Cms.Common.EkEnumeration.FolderDestinationType[2];
        string[] Link = new string[2];
        string[] DestName = new string[2];
        string[] ExtParams = new string[2];

        DestType[0] = EkEnumeration.FolderDestinationType.Frame;
        Link[0] = "javascript:ClearFolderInfo();";
        DestName[0] = "medialist";
        ExtParams[0] = "";
        DestType[1] = EkEnumeration.FolderDestinationType.Frame;
        DestName[1] = "libraryinsert";
        if (scope == "all")
        {
            ExtParams[1] = "&scope=" + scope + RetField;
            Link[1] = "libraryinsert.aspx?action=ViewLibraryByCategory<%=enableQDOparam%>&id=";
        }
        else
        {
            ExtParams[1] = "&scope=" + scope + QueryType + RetField + disableLinkManage;
            Link[1] = "libraryinsert.aspx?action=ViewLibraryByCategory<%=enableQDOparam%>&id=";
        }
        Response.Write(cDbObj.OutputFolders(level, Parent, ref DestType, ref Link, ref DestName, ref ExtParams, ref cAllFolders, EkEnumeration.FolderTreeType.Library));
    }
}

