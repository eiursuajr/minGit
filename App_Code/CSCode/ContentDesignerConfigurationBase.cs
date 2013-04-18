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


public abstract class ContentDesignerConfigurationBase : System.Web.UI.Page
{
    protected Ektron.Cms.SettingsData settings_data;
    protected bool bCanModifyImg = false;

    protected virtual void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            Ektron.Cms.UserAPI refUserAPI = new Ektron.Cms.UserAPI();
            Ektron.Cms.SiteAPI refSiteApi = new Ektron.Cms.SiteAPI();
            Ektron.Cms.UserData user_info = new Ektron.Cms.UserData();
            Ektron.Cms.Common.EkRequestInformation RequestInfo;
            settings_data = refSiteApi.GetSiteVariables(-1);
            RequestInfo = refUserAPI.RequestInformationRef;
            if ((RequestInfo == null) && 0 == RequestInfo.IsMembershipUser)
            {
                bCanModifyImg = true;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["CanModifyImg"]))
            {
                bCanModifyImg = Convert.ToBoolean(Request.QueryString["CanModifyImg"]);
            }
        }
        catch (Exception)
        {
            // Mostly likely not logged and file is being browsed directly
            settings_data = null;
        }
        Response.ContentType = "text/xml";
    }
}