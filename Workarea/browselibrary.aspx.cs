using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Site;
using Ektron.Cms.User;
using Ektron.Cms.Content;
using Ektron.Cms;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;

public partial class Workarea_BrowseLibrary : System.Web.UI.Page
{
    protected object gtMsgObj;
    protected object gtMess;
    protected long locFolderID;
    protected string strQueryString = string.Empty;
    protected string lScope;
    protected string actionType = string.Empty;
    protected string autonav;
    protected object CurrentUserId;
    protected object ErrorString;
    protected ApplicationAPI AppUI = new ApplicationAPI();
    protected string AppPath;
    protected string AppImgPath;
    protected string sitePath;
    protected string QueryType = "";
    protected string disableLinkManage = "";
    protected object ContentLanguage;
    protected object EnableMultilingual;
    protected string RetField;
    protected Ektron.Cms.Common.EkMessageHelper MsgHelper = null;
    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected const int CONTENT_LANGUAGES_UNDEFINED = 0;
    public Workarea_BrowseLibrary()
	{
        MsgHelper = AppUI.EkMsgRef;
        CurrentUserId = AppUI.UserId;
        AppImgPath = AppUI.AppImgPath;
        AppPath = AppUI.AppPath;
        sitePath = AppUI.SitePath;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(Request.QueryString["defaultFolderId"]))
        {
            locFolderID = Convert.ToInt64(Request.QueryString["defaultFolderId"]);
        }

        if (!String.IsNullOrEmpty(Request.QueryString["scope"]))
        {
            lScope = Request.QueryString["scope"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["actiontype"]))
        {
            actionType = Request.QueryString["actiontype"];
        }
        if (!String.IsNullOrEmpty(Request.QueryString["autonav"]))
        {
            autonav = Request.QueryString["autonav"];
        }
        if (autonav != "")
        {
            autonav = ("&autonav=" + autonav);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["RetField"]))
        {
            RetField = Request.QueryString["RetField"];
        }

        if ((RetField != ""))
        {
            RetField = ("&RetField=" + RetField);
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
            if (Convert.ToString(Request.QueryString["qdo"]) == "1")
            {
                showQDContentOnly = true;
            }
        }
        string enableQDOparam = string.Empty;
        if (showQDContentOnly)
        {
            enableQDOparam = "&qdo=1";
        }
        strQueryString = "actionType=" + actionType + "&scope=" + lScope + autonav + RetField + QueryType + disableLinkManage + enableQDOparam;

        frmMediaList.Attributes.Add("src","librarylist.aspx?" + strQueryString);
        frmLibraryInsert.Attributes.Add("src", "libraryinsert.aspx?" + strQueryString);
    }
}
