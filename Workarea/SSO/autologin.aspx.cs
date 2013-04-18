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

public partial class autologin : System.Web.UI.Page
{

    #region  Web Form Designer Generated Code

    protected System.Web.UI.WebControls.Literal WorkareaCloserJS;
    protected System.Web.UI.WebControls.Panel LoginErrorPanel;
    protected System.Web.UI.HtmlControls.HtmlTableCell ErrorText;
    protected System.Web.UI.WebControls.Panel LoginSuceededPanel;

    #endregion

    private SiteAPI m_refsiteAPI = new SiteAPI();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        HttpContext.Current.Session["ekusername"] = Request.ServerVariables["LOGON_USER"];
        Session["autologin"] = true;
        if (Request.QueryString["diagnostic"] != null &&
            Request.QueryString["diagnostic"] == "true")
        {
            ltrredirect.Text = "UserName: " + Request.ServerVariables["LOGON_USER"];
        }
        else
        {
            ltrredirect.Text = "<script language=\"Javascript\">" + Environment.NewLine;
            ltrredirect.Text += "   window.parent.location.href = \'" + m_refsiteAPI.AppPath + "login.aspx?autoaddtype=" + Request.QueryString["autoaddtype"] + "\';" + Environment.NewLine;
            ltrredirect.Text += "</script>" + Environment.NewLine;
        }
    }

}


