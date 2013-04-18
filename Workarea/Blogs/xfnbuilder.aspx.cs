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

public partial class blogs_xfnbuilder : System.Web.UI.Page
{
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string m_strStyleSheetJS = "";

    private void XFNToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar("Edit Relationship");
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", "Cancel", "Cancel", "onclick=\"self.close();return false;\" ", StyleHelper.CancelButtonCssClass, true));
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", "Accept", "Accept", "onclick=\"WriteBack();\" ", StyleHelper.SaveButtonCssClass, true));
        result.Append("<td>");
        //result.Append(m_refStyle.GetHelpButton("XFN Builder"))
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
		Utilities.ValidateUserLogin();
        RegisterResources();
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            m_id.Text = EkFunctions.HtmlEncode(Request.QueryString["id"]);
        }

        if (!string.IsNullOrEmpty(Request.QueryString["field"]))
        {
            m_field.Text = EkFunctions.HtmlEncode(Request.QueryString["field"]);
        }

        m_refMsg = m_refContentApi.EkMsgRef;
        AppImgPath = m_refContentApi.AppImgPath;
        AppPath = m_refContentApi.AppPath;
        if (m_refContentApi.UserId > 0)
        {
            XFNToolBar();
        }
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
    }
}


