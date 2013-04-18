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

public partial class workareanavigationtree : System.Web.UI.Page
{
    protected bool m_bAjaxTree = false;

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        //register page components
        this.RegisterJS();
        this.RegisterCSS();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        CommonApi m_refApi = new CommonApi();
        if (m_refApi.TreeModel == 1)
        {
            m_bAjaxTree = true;
        }

        m_refApi = null;

        //set javascript strings
        this.SetJavascriptStrings();
    }

    private void SetJavascriptStrings()
    {
        litszContentNav.Text = Strings.Replace(Request.QueryString["ContentNav"], "\\", "\\\\", 1, -1, 0);
        litszTaxNav.Text = Strings.Replace(Request.QueryString["TaxNav"], "\\", "\\\\", 1, -1, 0);
        litszMenuNav.Text = Strings.Replace(Request.QueryString["MenuNav"], "\\", "\\\\", 1, -1, 0);
        litszCollNav.Text = Strings.Replace(Request.QueryString["CollNav"], "\\", "\\\\", 1, -1, 0);
        litszFormsNav.Text = Strings.Replace(Request.QueryString["FormsNav"], "\\", "\\\\", 1, -1, 0);
        litszLibNav.Text = Strings.Replace(Request.QueryString["LibNav"], "\\", "\\\\", 1, -1, 0);
        litszAdminNav.Text = Strings.Replace(Request.QueryString["AdminNav"], "\\", "\\\\", 1, -1, 0);
        litszReportNav.Text = Strings.Replace(Request.QueryString["ReportNav"], "\\", "\\\\", 1, -1, 0);
        litszVisibleStartTree.Text = Request.QueryString["TreeVisible"];

        if (m_bAjaxTree == true)
        {
            litLinkFileName.Text = "WorkAreaTrees.aspx?" + "TreeVisible=" + litszVisibleStartTree.Text + "&tree=";
        }
        else
        {
            litLinkFileName.Text = "workareanavigationtrees.aspx?tree=";
        }
    }

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
    }

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
    }
}


