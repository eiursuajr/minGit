using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API;
using Ektron.Cms.Content;
using Ektron.Cms.Controls;

partial class SiteLoginPanel : System.Web.UI.UserControl
{
    #region Private Member Variables
    private Common _Common;
    private ContentAPI _ContentApi;
    private EkMessageHelper _MessageHelper;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        _ContentApi = new ContentAPI();
        _Common = new Common();
        _MessageHelper = _ContentApi.EkMsgRef;

        // Use ASP.Net Login Control
        // Ektron Login features and Ektron Membership Provide (MembershipProvider="EktronMembershipProvider")
        Ektron.Cms.Controls.Login ektronLogin = new Ektron.Cms.Controls.Login();
        ektronLogin.AutoLogin = true;
        ektronLogin.AutoAddType = EkEnumeration.AutoAddUserTypes.Author;
        ektronLogin.Visible = false;

        // set translatable text values
        ((Literal)loginControl.FindControl("introText")).Text = _MessageHelper.GetMessage("first login message");
        ((Label)loginControl.FindControl("UserNameLabel")).Text = _MessageHelper.GetMessage("username label");
        ((Label)loginControl.FindControl("PasswordLabel")).Text = _MessageHelper.GetMessage("password label");

        loginControl.LoginButtonText = _MessageHelper.GetMessage("generic login msg");
        loginControl.TitleText = loginControl.LoginButtonText;
        
        loginControl.FailureText = String.Format(@"<div class='ui-widget errorMessage'><div class='ui-state-error ui-corner-all ui-helper-clearfix'><span class='ui-icon ui-icon-alert errorIcon'></span><h2>{0}</h2><div>{1}</div></div></div>", _MessageHelper.GetMessage("invalid username or password"), _MessageHelper.GetMessage("login helper text"));

        RegisterResouces();

        //Check if user is logged in
        if (_Common.UserId > 0)
        {
            //HttpCookie cookEcm = Request.Cookies.Get("ecm");
            //if (cookEcm != null && cookEcm.Expires.Ticks == 0)
            //{
            //    Response.Cookies["ecm"].Value = cookEcm.Value;
            //    //Response.Cookies["ecm"].Expires = DateTime.Now.Date.AddDays(30);
            //}
            LoginSuceededPanel.Visible = true;
            LoginRequestPanel.Visible = false;
        }
        else
        {
            if (test.Value == "loginAttempt")
            {
                test.Value = "";
            }
            LoginSuceededPanel.Visible = false;
            LoginRequestPanel.Visible = true;
        }
    }

    private bool RegisterResouces()
    {
        // CSS

        // JS
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, _ContentApi.AppPath + "controls/login/ektron.ux.login.js", "EktronUXLoginJS");
        JS.RegisterJSBlock(this, String.Format("Ektron.UX.Login.fieldsRequired = '<h2>{0}</h2><div>{1}</div>'", _MessageHelper.GetMessage("invalid username or password"), _MessageHelper.GetMessage("login required fields")), "EktronUXLoginFieldRequiredJS");
        return true;

    }
}
