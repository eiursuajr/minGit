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

public partial class Workarea_controls_content_contentmessage : System.Web.UI.Page
{
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string m_strPageAction = "";
    protected long m_intId = 0;
    protected string m_strStyleSheetJS = "";
    protected void Page_Init(object sender, System.EventArgs e)
    {
        ltStyle.Text = m_refStyle.GetClientScript();
        MessageBoard1.MaxResults = m_refContentApi.RequestInformationRef.PagingSize;
    }
    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refContentApi.EkMsgRef;
		Utilities.ValidateUserLogin();
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
            return;
        } 
        AppImgPath = m_refContentApi.AppImgPath;
        RegisterResources();
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            m_intId = Convert.ToInt64(Request.QueryString["id"]);
        }
        ToolBar();
    }
    private void ToolBar()
    {
        System.Text.StringBuilder result;
        result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view messages for content"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("content.aspx?&action=View&id=" + m_intId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}

