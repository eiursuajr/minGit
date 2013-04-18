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

public partial class login : System.Web.UI.Page
{
    public login()
    {
        int.TryParse(_CommonApi.GetCookieValue("SiteLanguage"), out iSiteLanguage);
        if (iSiteLanguage == 0)
            iSiteLanguage = int.Parse(_CommonApi.GetCookieValue("DefaultLanguage"));
    }
    private string m_strAction = "";
    protected EkMessageHelper m_refMsg;
    protected ContentAPI _ContentApi = new Ektron.Cms.ContentAPI();
    protected CommonApi _CommonApi = new Ektron.Cms.CommonApi();
    protected string m_template = "";
    bool bAutoLogin = false;
    string sAutoLoginName = "";
    protected UserAPI m_refUserApi = new UserAPI();
    protected string m_PleaseLoginMsg;
    protected bool m_bMemberOnly = false;
    protected EkEnumeration.AutoAddUserTypes m_eAutoAddType = EkEnumeration.AutoAddUserTypes.Author;
    protected bool commerceAdmin = false;
    protected bool checkedCommerceAdmin = false;
    protected int iSiteLanguage;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            ErrorText.Text = "";
            m_strAction = Request.QueryString["action"];
            if (m_strAction == null)
            {
                m_strAction = "";
            }

            LoginRequestPanel.Visible = false;
            LoginSuceededPanel.Visible = false;
            LoginErrorPanel.Visible = false;

            //cancelBtn.Value = m_refMsg.GetMessage("generic cancel")
            RegisterResources();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    protected void Page_PreInit(object sender, System.EventArgs e)
    {
        m_refMsg = m_refUserApi.EkMsgRef;


        m_PleaseLoginMsg = m_refMsg.GetMessage("lbl logged in refresh");

        LoginBtn.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        LoginBtn.ToolTip = m_refMsg.GetMessageForLanguage("click here to log in", iSiteLanguage);
        loginLoginText.Text = m_refMsg.GetMessageForLanguage("generic login msg", iSiteLanguage);
        loginCancelText.Text = m_refMsg.GetMessageForLanguage("generic cancel", iSiteLanguage);

        LogoutBtn.Attributes.Add("onclick", "clearAuth()");
        LogoutBtn.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        litLogoutButtonText.Text = m_refMsg.GetMessage("generic logout msg");
        loginCancel.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        logoutCancel.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        logoutCancelText.Text = loginCancelText.Text;

        changePasswordBtn.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        changePasswordCancel.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        changePasswordBtn.ToolTip = m_refMsg.GetMessage("lbl change password alt");
        changePasswordText.Text = m_refMsg.GetMessage("lbl change password");
        changePasswordCancelText.Text = m_refMsg.GetMessage("generic cancel");
        changePasswordCancel.Text = m_refMsg.GetMessage("generic cancel");
        changePasswordCancel.ToolTip = changePasswordCancel.Text;
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        if ((!(Request.QueryString["autoaddtype"] == null)) && (Request.QueryString["autoaddtype"].ToString().ToLower() == "member"))
        {
            m_eAutoAddType = EkEnumeration.AutoAddUserTypes.Member;
        }
        if ((m_strAction.Length > 0) && ((m_strAction.ToLower() == "logout") || (m_strAction.ToLower() == "logoutnoprompt")))
        {
            LogoutUser();
        }
        else if ((m_strAction.Length > 0) && (m_strAction.ToLower() == "autologin"))
        {
            autologin.Text = "<iframe src=\"SSO/autologin.aspx?autoaddtype=" + System.Enum.GetName(typeof(Ektron.Cms.Common.EkEnumeration.AutoAddUserTypes), this.m_eAutoAddType) + "\"></iframe>";
        }
        else if (Context.Items.Count > 0 && !(HttpContext.Current.Session["ekusername"] == null))
        {
            bAutoLogin = true;
            sAutoLoginName = Convert.ToString(HttpContext.Current.Session["ekusername"]);
            HttpContext.Current.Session["ekusername"] = null;
            Login();
        }
        else if (m_strAction.Length > 0 && m_strAction.ToLower() == "toggle")
        {
            if (m_refUserApi.RequestInformationRef.ShowBorders == true)
            {
                m_refUserApi.SetShowBorders(false);
            }
            else
            {
                m_refUserApi.SetShowBorders(true);
            }

            Response.Redirect("close.aspx?toggle=true", false);
        }
        else
        {
            Login();
        }
    }

    public void Login()
    {
        string strUsername = "";
        string strPassword = "";
        string strDomain = Request.Form["domainname"];
        string strReload = "true";
        string strProtocol = "";
        string strGUID = "";
        UserData UserInfo = null;
        int i = 0;
        bool pwdReset = false;


        try
        {
            if (!(Request.QueryString["reload"] == null))
            {
                strReload = Request.QueryString["reload"];
            }
            if (!(Request.QueryString["onlymember"] == null))
            {
                if (Request.QueryString["onlymember"] != "")
                {
                    //We should not check to see the value is true or false to stop hacking.
                    //by defaut if the key exists then we only allow membership user all other should be logged out.
                    m_bMemberOnly = true;
                }
            }
            strProtocol = m_refUserApi.AuthProtocol;

            if ((!(IsPostBack)) && !(bAutoLogin))
            {
                loginlbl.Text = _ContentApi.AppName + " " + m_refMsg.GetMessageForLanguage("generic login msg", iSiteLanguage);
                passwordExpiredLabel.Text = _ContentApi.AppName + " " + m_refMsg.GetMessageForLanguage("generic password expired", iSiteLanguage);
                usernamelbl.InnerText = m_refMsg.GetMessageForLanguage("user", iSiteLanguage);
                passwordlbl.InnerText = m_refMsg.GetMessageForLanguage("pwd", iSiteLanguage);
                domainlbl.InnerHtml = m_refMsg.GetMessageForLanguage("domain", iSiteLanguage);
                LoginRequestPanel.Visible = true;
                TR_domain.Visible = false;
                if (!((m_bMemberOnly) && (m_refUserApi.RequestInformationRef.LDAPMembershipUser == false)))
                {
                    DomainData[] domain_list;
                    domain_list = m_refUserApi.GetDomains(0, 1);
                    if (!(domain_list == null))
                    {
                        TR_domain.Visible = true;
                        for (i = 0; i <= domain_list.Length - 1; i++)
                        {
                            domainname.Items.Add(new ListItem(domain_list[i].Name, domain_list[i].Path));
                        }
                        domainname.Items[0].Selected = true;
                    }
                }
                else if (m_bMemberOnly && m_refUserApi.RequestInformationRef.LDAPMembershipUser == false)
                {
                    DomainData[] domain_list;
                    domain_list = m_refUserApi.GetDomains(0, 1);
                    if ((!(domain_list == null)) && domain_list.Length > 0)
                    {
                        domainlbl.InnerHtml = "<input type=\"hidden\" name=\"domainname_\" id=\"domainname_\" value=\"" + domain_list[0].Path + "\" />";
                        TR_domain.Visible = true;
                    }
                    domainname.Visible = false;
                }

                if (m_refUserApi.RequestInformationRef.HttpsProtocol == "on")
                {
                    username.Attributes.Add("autocomplete", "off");
                    password.Attributes.Add("autocomplete", "off");
                }
            }
            else
            {
                if (bAutoLogin)
                {
                    string[] arrSAMUsername;
                    DomainData[] domain_list;
                    domain_list = m_refUserApi.GetDomains(1, 1);
                    strUsername = sAutoLoginName.ToString(); //Context.Items("username").ToString()
                    arrSAMUsername = strUsername.Split('\\');
                    strUsername = arrSAMUsername[1];
                    if ((!(domain_list == null)) && domain_list.Length > 0)
                    {
                        for (i = 0; i <= domain_list.Length - 1; i++)
                        {
                            if (arrSAMUsername[0].ToString().ToLower() == domain_list[i].NETBIOSName.ToString().ToLower())
                            {
                                strDomain = (string)(domain_list[i].Path.ToString());
                                break;
                            }
                        }
                    }
                    if (strDomain == "")
                    {
                        throw (new Exception("Invalid Domain"));
                    }

                }
                else if (Request.Form["hdn_action"] == "reset" && (!string.IsNullOrEmpty(HttpContext.Current.Session["resetPasswordUser"].ToString())))
                {
                    UserInfo = (Ektron.Cms.UserData)HttpContext.Current.Session["resetPasswordUser"];
                    strUsername = UserInfo.Username;

                    if (Request.Form["newpassword1"] != Request.Form["newpassword2"])
                    {
                        throw (new Exception(m_refMsg.GetMessage("js: passwords do not match")));
                    }
                    else
                    {
                        Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results;
                        if (UserInfo.IsMemberShip)
                        {
                            results = ObjectFactory.GetPasswordValidation().ValidateForMember(Request.Form["newpassword1"]);
                        }
                        else if (IsCommerceAdmin(UserInfo.Id))
                        {
                            results = ObjectFactory.GetPasswordValidation().ValidateForCommerceAdmin(Request.Form["newpassword1"]);
                        }
                        else if (IsAdmin(UserInfo.Id))
                        {
                            results = ObjectFactory.GetPasswordValidation().ValidateForAdmin(Request.Form["newpassword1"]);
                        }
                        else
                        {
                            results = ObjectFactory.GetPasswordValidation().ValidateForAuthor(Request.Form["newpassword1"]);
                        }
                        if (!results.IsValid)
                        {
                            throw (new Exception(EkFunctions.GetAllValidationMessages(results)));
                        }
                    }

                    strPassword = Request.Form["newpassword1"];
                    UserInfo.Password = strPassword;
                    string oldPAssword = Convert.ToString(HttpContext.Current.Session["resetPasswordUser_oldPassword"]);
                    if (oldPAssword == null)
                    {
                        oldPAssword = string.Empty;
                    }
                    if (!m_refUserApi.ResetUserPassword(UserInfo.Username, oldPAssword, Request.Form["newpassword1"]))
                    {
                        throw (new Exception(m_refMsg.GetMessage("lbl password must be different")));
                    }

                    // pwdReset = True
                    HttpContext.Current.Session["resetPasswordUser"] = null;
                    HttpContext.Current.Session["resetPasswordUser_oldPassword"] = null;
                    PasswordExpiredPanel.Visible = false;

                }
                else if (Request.Form["hdn_action"] == "skip" && (HttpContext.Current.Session["resetPasswordUser"] != null))
                {

                    UserInfo = (Ektron.Cms.UserData)HttpContext.Current.Session["resetPasswordUser"];
                    strUsername = UserInfo.Username;
                    pwdReset = true;
                    HttpContext.Current.Session["resetPasswordUser"] = null;
                    PasswordExpiredPanel.Visible = false;
                    HttpContext.Current.Session["ekuserid"] = UserInfo.Id;
                    HttpContext.Current.Session["ekuniqueid"] = UserInfo.LoginIdentification;

                }
                else
                {
                    strUsername = Request.Form["username"];
                    strPassword = Request.Form["password"];
                    if (!(Request.Form["domainname"] == null))
                    {
                        strDomain = Request.Form["domainname"];
                    }
                    else if (m_bMemberOnly && m_refUserApi.RequestInformationRef.LDAPMembershipUser == false)
                    {
                        strDomain = Request.Form["domainname_"];
                    }
                }
                if (string.Compare(strUsername, "") > 0 && ((string.Compare(strPassword, "") > 0) || (bAutoLogin)))
                {
                    HttpCookie cookieEktGUID;
                    if (Request.Cookies.Get("EktGUID") == null)
                    {
                        strGUID = System.Guid.NewGuid().ToString();
                        cookieEktGUID = new HttpCookie("EktGUID", strGUID);
                        cookieEktGUID.Path = "/";
                        cookieEktGUID.Expires = DateTime.Now.AddYears(1);
                        Response.Cookies.Add(cookieEktGUID);
                        m_refUserApi.RequestInformationRef.ClientEktGUID = strGUID;
                    }
                }
                //LogIn User
                if (!pwdReset)
                {

                    if (bAutoLogin)
                    {
                        UserInfo = m_refUserApi.autologInUser(strUsername, strDomain, Request.ServerVariables["SERVER_NAME"], m_eAutoAddType);
                    }
                    else
                    {
                        UserInfo = m_refUserApi.logInUser(strUsername, strPassword, Request.ServerVariables["SERVER_NAME"], strDomain, strProtocol, m_eAutoAddType);
                    }
                    if (UserInfo == null || UserInfo.Id == 0)
                    {
                        if (m_refUserApi.RequestInformationRef.LDAPMembershipUser == false && this.m_bMemberOnly == true)
                        {
                            TR_domain.Visible = false;
                        }
                        AuthenticationFailed(m_refMsg.GetMessage("com: authentication error"));
                        return;
                    }

                    if (m_bMemberOnly)
                    {
                        if ((UserInfo != null) && (UserInfo.Id > 0))
                        {
                            if (!UserInfo.IsMemberShip)
                            {
                                Response.Cookies["ecm"].Expires = DateTime.Now;
                                Response.Cookies["ecmSecure"].Expires = DateTime.Now;
                                throw (new Exception(m_refMsg.GetMessage("err members only")));
                            }
                        }
                    }

                }

                m_template = UserInfo.UserPreference.Template;
                //Set Login Cookie
                if (m_refUserApi.RequestInformationRef.CommerceSettings.ComplianceMode && IsCommerceAdmin(UserInfo.Id))
                {
                    m_PleaseLoginMsg = m_refMsg.GetMessage("lbl logged in refresh ssl");
                }
                m_refUserApi.SetAuthenticationCookie(UserInfo);

                if (strReload == "true")
                {
                    LoginSuceededPanel.Visible = true;
                }
                else
                {
                    LoginSuceededPanel.Visible = false;
                }
                WorkareaCloserJS.Text = "";
                string storeRptUrl = "";
                Session["fromLnkPg"] = "0";
                if (!(Request.QueryString["fromLnkPg"] == null))
                {
                    if (Session["RedirectLnk"] != null)
                    {
                        storeRptUrl = (string)Session["RedirectLnk"];
                        if (storeRptUrl.Length > 0)
                        {
                            Session["RedirectLnk"] = "";
                            Session["fromLnkPg"] = "1";
                            Response.Redirect(storeRptUrl);
                        }
                    }
                }
            }

        }
        catch (Ektron.Cms.Exceptions.PasswordGracePeriodException)
        {

            ShowPasswordReset(m_refUserApi.EkUserRef.ConvertUserData(m_refUserApi.EkUserRef.GetUserByUsername(strUsername, "", true)), strPassword, true);

        }
        catch (Ektron.Cms.Exceptions.PasswordExpiredException)
        {

            ShowPasswordReset(m_refUserApi.EkUserRef.ConvertUserData(m_refUserApi.EkUserRef.GetUserByUsername(strUsername, "", true)), strPassword, false);

        }
        catch (Exception ex)
        {

            if (!(Request.Form["hdn_action"] == "reset"))
            {
                HttpContext.Current.Session["resetPasswordUser"] = null;
            }
            ShowError(ex.Message);

        }
    }

    private bool IsADUser(UserData authUser)
    {

        SettingsData appSettings = (new SiteAPI()).GetSiteVariables(_ContentApi.UserId);
        bool ADInfoPresent = System.Convert.ToBoolean(authUser.Path != "" && authUser.Domain != "");

        if (appSettings.ADIntegration && ADInfoPresent)
        {

            if (!authUser.IsMemberShip)
            {

                return true;

            }
            else if (m_refUserApi.RequestInformationRef.LDAPMembershipUser && authUser.IsMemberShip)
            {

                return true;

            }
            else
            {

                return false;

            }

        }
        else
        {

            return false;

        }


    }

    public bool IsCommerceAdmin(long userId)
    {
        if (!checkedCommerceAdmin)
        {
            commerceAdmin = m_refUserApi.EkUserRef.IsUserIdCommerceAdmin(userId);
            checkedCommerceAdmin = true;
        }
        return commerceAdmin;
    }
    public bool IsAdmin(long userId)
    {
        return m_refUserApi.EkUserRef.IsUserIdAdmin(userId);
    }

    private void ShowError(string ErrorString)
    {
        //I just truncate our method names
        if (ErrorString.Length > 0 && ErrorString.IndexOf("[") != -1)
        {
            ErrorString = ErrorString.Substring(0, ErrorString.IndexOf("["));
        }
        if (ErrorString.IndexOf(m_refMsg.GetMessage("msg:account locked")) != -1)
        {
            ErrorString = m_refMsg.GetMessage("msg:account locked");
        }
        if (ErrorString.IndexOf("user name or bad password") != -1)
        {
            ErrorString = ErrorString.Substring(0, System.Convert.ToInt32(ErrorString.IndexOf("password") + 8));
        }
        if (ErrorString.IndexOf(m_refMsg.GetMessage("com: could not retrieve ad user information")) != -1)
        {
            // this should make more sense - SMK
            ErrorString = m_refMsg.GetMessage("com: could not authenticate user against ad");
        }
        if (ErrorString.IndexOf("Object reference not set to an instance of an object") != -1)
        {
            ErrorString = m_refMsg.GetMessage("com: authentication error");
        }
        if (ErrorString.IndexOf("LDAP") != -1)
        {
            ErrorString = ErrorString.Substring(0, System.Convert.ToInt32(ErrorString.IndexOf("LDAP") + 4));
        }
        if (ErrorString.ToLower().IndexOf("maximum number of licensed users. you cannot ") != -1)
        {
            ErrorString = ErrorString.Substring(0, System.Convert.ToInt32(ErrorString.ToLower().IndexOf(". you cannot")));
        }
        if (ErrorString.IndexOf(m_refMsg.GetMessage("com:license violation")) != -1)
        {
            ErrorString = m_refMsg.GetMessage("com:license violation");
        }
        if (ErrorString.ToLower().IndexOf("at ektron.") != -1)
        {
            ErrorString = ErrorString.Substring(0, System.Convert.ToInt32(ErrorString.ToLower().IndexOf("at ektron.")));
        }
        if ((ErrorString.ToLower().IndexOf("unknown error") > -1) || (ErrorString.ToLower().IndexOf("server is not operational") > -1))
        {
            ErrorString = (string)(m_refMsg.GetMessage("com: error getting ad domains") + ". " + m_refMsg.GetMessage("com: verify ad domains"));
        }
        if (m_refUserApi.RequestInformationRef.LDAPMembershipUser == false && this.m_bMemberOnly == true)
        {
            TR_domain.Visible = false;
        }
        AuthenticationFailed(ErrorString);
    }

    private void ShowPasswordReset(UserData cmsUser, string oldPassword, bool IsInGracePeriod)
    {

        HttpContext.Current.Session["resetPasswordUser"] = cmsUser;
        HttpContext.Current.Session["resetPasswordUser_oldPassword"] = oldPassword;

        loginlbl.Text = m_refMsg.GetMessage("Invalid username or password") + "\r\n" + m_refMsg.GetMessage("Please try again");

        LoginRequestPanel.Visible = false;
        PasswordExpiredPanel.Visible = true;
        LoginErrorPanel.Visible = false;

        passwordResetlbl.Text = m_refMsg.GetMessage("lbl password expired");
        newpassword1label.InnerText = m_refMsg.GetMessage("lbl new") + ":";
        newpassword2label.InnerText = m_refMsg.GetMessage("lbl confirm") + ":";

        btn_skip.Attributes.Add("style", "padding:0.4em 1em 0.4em 20px;position:relative;text-decoration:none;");
        btn_skip.ToolTip = m_refMsg.GetMessage("lbl skip change password alt");
        skipText.Text = m_refMsg.GetMessage("lbl skip change password");

        if (IsInGracePeriod)
        {
            btn_skip.Visible = true;
            changePasswordCancel.Visible = false;
        }
        else
        {
            btn_skip.Visible = false;
            changePasswordCancel.Visible = true;
        }
        // Add support for client side validation:
        Ektron.Cms.Commerce.IPasswordValidation pv = ObjectFactory.GetPasswordValidation();
        passwordValidationString.Text = pv.GetRegexForCommerceAdmin().Replace("\"", "\\\"").Replace("\\t", "\\\\t");
        passwordMismatchErrorString.Text = m_refMsg.GetMessage("js: passwords do not match");

    }

    private void AuthenticationFailed(string errorMessage)
    {
        //If (errorMessage = "") Then
        //    loginlbl.Text = m_refMsg.GetMessage("Invalid username or password") & vbCrLf & m_refMsg.GetMessage("Please try again")
        //Else
        //    loginlbl.Text = ""
        //End If

        if (Request.Form["hdn_action"] == "reset")
        {

            passwordResetlbl.Text = "";
            LoginRequestPanel.Visible = false;
            PasswordExpiredPanel.Visible = true;

        }
        else
        {

            LoginRequestPanel.Visible = true;
            PasswordExpiredPanel.Visible = false;

        }

        LoginErrorPanel.Visible = true;
        usernamelbl.InnerText = m_refMsg.GetMessage("user");
        passwordlbl.InnerText = m_refMsg.GetMessage("pwd");
        domainlbl.InnerHtml = m_refMsg.GetMessage("domain");
        ErrorText.Text = errorMessage;
    }

    private void LogoutUser()
    {

        if (!IsPostBack && m_strAction.ToLower() != "logoutnoprompt")
        {

            LanguageData language_data;
            SiteAPI m_refSiteApi = new SiteAPI();
            language_data = m_refSiteApi.GetLanguageById(m_refSiteApi.UserLanguage);
            //logoutmsg.InnerHtml = m_refMsg.GetMessage("logout message")
            logoutmsg.Text = _ContentApi.AppName + " " + m_refMsg.GetMessage("generic logout msg");
            LogoutPanel.Visible = true;
            //fixed defect # 19069
            System.Text.StringBuilder strCloser = new System.Text.StringBuilder();
            strCloser.AppendLine("<script type=\"text/javascript\">");
            strCloser.AppendLine("document.forms[0].action = \'login.aspx?action=logout&i=19069\';");
            strCloser.AppendLine("</script>");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "formsubmit", strCloser.ToString());
            ltr_olduser.Text = "<input type=\"hidden\" id=\"hdn_olduser\" name=\"hdn_olduser\" value=\"" + m_refUserApi.UserId + "\" />";

        }
        else
        {

            long oldUserId = 0;
            if (!string.IsNullOrEmpty(Request.Form["hdn_olduser"]) && Information.IsNumeric(Request.Form["hdn_olduser"]))
            {
                oldUserId = Convert.ToInt64(Request.Form["hdn_olduser"]);
            }

            if (oldUserId > 0)
            {
                m_refUserApi.clearUserUniqueIdentification(oldUserId);
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["ekismembershipuser"] = 0;
                }
            }

            if (m_refUserApi.RequestInformationRef.CommerceSettings.ComplianceMode && oldUserId > 0 && m_refUserApi.EkUserRef.IsUserIdCommerceAdmin(oldUserId))
            {

                Ektron.Cms.Instrumentation.Log.WriteMessage(string.Format(m_refMsg.GetMessage("log commerce auth-commerceadmin logout"), oldUserId), new string[] { "CommerceAuthentication" }, GetLogProperties(oldUserId));

            }

            System.Text.StringBuilder strCloser = new System.Text.StringBuilder();
            // Render script to close the Workarea:
            strCloser.Append("<script type=\"text/javascript\">" + "\r\n");
            strCloser.Append("window.open(\'close.aspx?logout=true\',\'Admin400\',\'toolbar=0,location=0,scrollbars=0,width=1,height=1\');" + "\r\n");
            strCloser.Append("</script>" + "\r\n");
            WorkareaCloserJS.Text = strCloser.ToString();

            if (!(Session["DDSnip"] == null))
            {
                Session["DDSnip"] = null;
            }
            if (!(Session["LibCategory"] == null))
            {
                Session["LibCategory"] = null;
            }
            if (Response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName] != null)
            {
                Response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Expires = DateTime.Now;
            }

            Response.Cookies["ecm"].Expires = DateTime.Now;
            Response.Cookies["ecmSecure"].Expires = DateTime.Now;
            LoginSuceededPanel.Visible = true;
        }
    }

    protected void LogoutBtn_Click(object sender, System.EventArgs e)
    {

    }

    private System.Collections.Generic.Dictionary<string, object> GetLogProperties(long userId)
    {

        System.Collections.Generic.Dictionary<string, object> logProperties = new System.Collections.Generic.Dictionary<string, object>();

        logProperties.Add("UserId", userId);
        logProperties.Add("IPAddress", m_refUserApi.RequestInformationRef.RemoteIP);

        return logProperties;

    }

    private void RegisterResources()
    {
        // Register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refUserApi.AppPath + "PrivateData/js/Ektron.Crypto.js", "EktronCryptoJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refUserApi.AppPath + "PrivateData/js/Ektron.Cache.js", "EktronCacheJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refUserApi.AppPath + "PrivateData/js/Ektron.PrivateData.aspx", "EktronPrivateDataJS");

        // Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
    }

}
