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

public partial class cmsdatalist : System.Web.UI.Page
{
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        CmsDataList.Visible = false;
        CmsDataListXml.Text = Ektron.Cms.Common.EkFunctions.XSLTransform(null, null, true, false, null, false, null);
    }
}


