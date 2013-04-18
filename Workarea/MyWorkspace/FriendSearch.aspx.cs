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

public partial class MyWorkspace_FriendSearch : workareabase
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser > 0)
        {
            HttpContext.Current.Response.Redirect(m_refContentApi.AppPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), false);
            return;
        }
        try
        {
            if (!Page.IsCallback)
            {
                if (m_iID > 0)
                {
                    m_refFriendsApi.AddPendingFriend(m_iID);
                }
            }
            SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }

    public void SetLabels()
    {
        this.AddHelpButton("friendsearch");
        this.SetTitleBarToMessage("lbl user search");
    }
}
