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
using Ektron.Cms.DataIO.LicenseManager;

public partial class usersdirectory : workareabase
{
    protected UserData user_data;
    protected UserAPI m_refUserApi = new UserAPI();
    protected string FromUsers = "";
    protected int m_intGroupType = -1; //0-CMS User; 1-Membership User
    protected long m_intGroupId = -1;
    protected int m_intUserActiveFlag = 0; //0-Active;1-Deleted;-1-Not verified
    protected string m_strDirection = "asc";
    protected string m_strSearchText = "";
    protected string m_strKeyWords = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strPageAction = "";
    protected string m_strSelectedItem = "-1";
    private string m_strBackAction = "viewallgroups";

    protected void Page_Load1(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refUserApi.RequestInformationRef, Feature.SocialNetworking))
        {
            Utilities.ShowError(m_refUserApi.EkMsgRef.GetMessage("feature locked error"));
        }
        View();
        SetLabels();
    }

    protected void View()
    {
        if (!String.IsNullOrEmpty(Request.QueryString["grouptype"]))
        {
            m_intGroupType = Convert.ToInt32(Request.QueryString["grouptype"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["groupid"]))
        {
            m_intGroupId = Convert.ToInt64(Request.QueryString["groupid"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["backaction"]))
        {
            m_strBackAction = Request.QueryString["backaction"].ToLower();
        }

        FromUsers = Request.QueryString["FromUsers"];
        bool bPreference = true;
        bool bReturnDeleted = false;
        if (m_intGroupType == 1)
        {
            bPreference = false;
        }
        if (m_intUserActiveFlag == -1)
        {
            bReturnDeleted = true;
        }
        user_data = this.m_refUserApi.GetUserById(this.m_iID, bPreference, bReturnDeleted);

    }

    protected void SetLabels()
    {
        this.SetTitleBarToString((string)(this.GetMessage("view user directory msg") + " \"" + user_data.Username + "\""));
        base.AddButtonwithMessages(m_refContentApi.AppImgPath + "user.gif", "users.aspx?action=View&LangType=" + this.ContentLanguage + "&groupid=" + this.m_intGroupId + "&grouptype=" + this.m_intGroupType + "&id=" + this.m_iID + "&FromUsers=" + this.FromUsers + "&OrderBy=user_name", "alt back to user", "btn back to user", "", StyleHelper.BackButtonCssClass, true);
        base.AddHelpButton("my_workspace");
        this.ltr_documents.Text = this.GetMessage("lbl documents");
        this.ltr_cgroups.Text = this.GetMessage("lbl groups");
        this.ltr_pendingcgroups.Text = this.GetMessage("lbl my pending groups");
        this.ltr_cgroupinvites.Text = this.GetMessage("lbl my invited groups");
        this.ltr_fav.Text = this.GetMessage("lbl favorites");
        this.ltr_friends.Text = this.GetMessage("lbl friends");
        this.ltr_pendingfriends.Text = this.GetMessage("lbl pending friends");
        this.ltr_friendinvites.Text = this.GetMessage("lbl sent friend requests");
        this.ltr_journal.Text = this.GetMessage("lbl journal");
        this.ltr_wall.Text = this.GetMessage("lbl msg board");
        this.ltr_messaging.Text = this.GetMessage("lbl inbox");
        this.ltr_sentmsg.Text = this.GetMessage("lbl sent messages");
        this.ltr_photos.Text = this.GetMessage("lbl my photos");
    }

    protected bool HasAcces()
    {
        bool bRet = false;
        if (this.m_refContentApi.IsAdmin() || this.m_refContentApi.UserId == this.m_iID)
        {
            bRet = true;
        }
        return bRet;
    }

    protected void SetJS()
    {
        StringBuilder sbJS = new StringBuilder();

        ltr_js.Text = sbJS.ToString();
    }
}
