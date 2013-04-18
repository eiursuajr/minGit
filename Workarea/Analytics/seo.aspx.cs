using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Common;

public partial class Analytics_seo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string uri = Request.QueryString["uri"] != null ? Request.QueryString["uri"].ToString() : "";
        uri = "http://" + Request.ServerVariables["HTTP_HOST"] + uri;
        Response.Redirect("../SEO/seo.aspx?tab=traffic&url=" + EkFunctions.UrlEncode(uri), false);       

    }
}
