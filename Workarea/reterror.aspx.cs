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

public partial class reterror : System.Web.UI.Page
{
    protected string AppImgPath = "";
    protected EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected CommonApi m_refApi = new CommonApi();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        m_refMsg = m_refApi.EkMsgRef;
        Page.Title = m_refMsg.GetMessage("generic error information msg");
        StyleSheetJS.Text = m_refStyle.GetClientScript();
        AppImgPath = m_refApi.AppImgPath;
        RegisterResources();
        DisplayErrorToolBar();
        td_error.InnerHtml = EkFunctions.HtmlEncode(Server.UrlDecode(Request.QueryString["info"]));
    }

    private void DisplayErrorToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("generic page error message"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refApi.AppPath + "images/UI/Icons/back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "onclick=\"history.go(-1);\"", StyleHelper.BackButtonCssClass, true));
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, m_refApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
    }
}
