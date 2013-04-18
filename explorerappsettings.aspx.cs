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

public partial class explorerappsettings : System.Web.UI.Page
{
    protected string AppPath = "";
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        System.Configuration.AppSettingsReader m_objConfigSettings;
        m_objConfigSettings = new System.Configuration.AppSettingsReader();
        AppPath = (string)(m_objConfigSettings.GetValue("ek_appPath", typeof(string)));
    }
}