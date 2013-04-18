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

public partial class StarterApps_SEO_mobilephone : System.Web.UI.Page
{
    protected string urlgooglemobile = string.Empty;
    protected void EktronTests_modalTest_Load(object sender, System.EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["noimgages"]) && Convert.ToInt32(Request.QueryString["noimgages"]) == 1)
        {
            urlgooglemobile = "http://" + Request.QueryString["url"] + "&_gwt_noimg=1";
        }
        else
        {
            urlgooglemobile = "http://" + Request.QueryString["url"];
        }
        litIFrame.Text = @"<iframe src=""http://www.google.com/gwt/n?u="+urlgooglemobile+@"""
                        name=""wap"" align=""top"" width=""272"" height=""364"" scrolling=""auto"" frameborder=""0"">
                    </iframe>";
    }
}


