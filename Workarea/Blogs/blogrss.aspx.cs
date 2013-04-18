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
using Ektron.Cms.Controls;

public partial class Blogs_blogrss : System.Web.UI.Page
{

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Response.ContentType = "text/xml";
        if (Request.QueryString["blog"] != "")
        {
            BlogEntries beEntries = new BlogEntries();
            if (!string.IsNullOrEmpty(Request.QueryString["blog"]))
                beEntries.BlogID = Convert.ToInt64(Request.QueryString["blog"]);
            beEntries.Page = this;
            ltr_rss.Text = beEntries.GetRssFeed();
        }
    }
}


