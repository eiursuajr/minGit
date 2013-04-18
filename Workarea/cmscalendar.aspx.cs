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
using Ektron.Editors.JavascriptEditorControls;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Workarea;
using Ektron.Cms.UI.CommonUI;

public partial class cmscalendar : System.Web.UI.Page
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Response.Redirect("reterror.aspx?info=classic calendar functionality is depricated.  Please use the new functionality", true);
    }
}