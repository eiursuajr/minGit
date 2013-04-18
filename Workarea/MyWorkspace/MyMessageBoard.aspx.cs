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
using Ektron.Cms.Workarea;

public partial class MyWorkspace_MyMessageBoard : workareabase
{
    protected ContentAPI cAPI = new ContentAPI();
    protected void Page_Init(object sender, System.EventArgs e)
    {
		Utilities.ValidateUserLogin();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        this.SetTitleBarToMessage("lbl msgboard");

        this.AddHelpButton("mymessageboard");

        if (cAPI.UserId > 0)
        {
            this.mb1.DefaultObjectID = cAPI.UserId;
        }
        else
        {
            this.mb1.Visible = false;
        }

        mb1.MaxResults = cAPI.RequestInformationRef.PagingSize;
    }
}
