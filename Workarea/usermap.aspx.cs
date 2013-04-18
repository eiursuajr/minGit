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

public partial class usermap : System.Web.UI.Page
{
    protected void form1_Load(object sender, System.EventArgs e)
    {
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (!String.IsNullOrEmpty(Request.QueryString["uid"]))
        {
            map1.EnableSearchResult = false;
            map1.ContentId = Convert.ToInt64(Request.QueryString["uid"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["map"]))
        {
            map1.MapProvider = Ektron.Cms.Controls.Map.Provider.Google;
        }
    }
}