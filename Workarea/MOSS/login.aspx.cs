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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.CookieEncryption;

public partial class Workarea_MOSS_login : System.Web.UI.Page
{
    private void Login(string username, string password, string domain)
    {
        //Hashtable user;
        ApplicationAPI appAPI = new ApplicationAPI();

        // if the username and password are both blank, then return 
        // whether or not the user is currently logged in, and if he 
        // is, return the userid and uniqueid
        if (username == "" && password == "")
        {
            Response.Clear();
            Response.Write("loginCallback(" + (appAPI.IsLoggedIn ? "'userid=" +
                appAPI.UserId + "&uniqueid=" + appAPI.UniqueId + "'" : "'0'") + ");");
            //Response.End();
            return;
        }

        // similarly, if the user is logged in, don't login again!
        if (appAPI.IsLoggedIn)
        {
            Response.Clear();
            Response.Write("loginCallback('userid=" + appAPI.UserId +
                "&uniqueid=" + appAPI.UniqueId + "');");
            //Response.End();
            return;
        }

        // login and create the cookies...
        Ektron.Cms.CommonApi appUI = new Ektron.Cms.CommonApi();

        System.Collections.Hashtable cUser = new Hashtable();

        string serverName = Request.ServerVariables.Get("SERVER_NAME");

        cUser = appUI.EkUserRef.logInUser(username, password, serverName, "", appUI.AuthProtocol, false, EkEnumeration.AutoAddUserTypes.Author);

        if (cUser.Count > 0)
        {
            //Response.Clear();
            HttpCookie cookEcm;

            /*if (!(Request.Cookies.Get("ecm") == null))
            {

                cookEcm = Request.Cookies.Get("ecm");

                cookEcm.Expires = DateTime.Now;

                Response.Cookies.Add(cookEcm);

            }*/

            cookEcm = new HttpCookie("ecm");

            cookEcm.Values.Add("user_id", cUser["UserID"].ToString());

            cookEcm.Values.Add("site_id", appUI.SitePath + "," + cUser["LoginNumber"].ToString());

            cookEcm.Values.Add("userfullname", cUser["UserName"].ToString());

            cookEcm.Values.Add("username", username);

            cookEcm.Values.Add("new_site", appUI.SitePath.ToString());

            cookEcm.Values.Add("unique_id", cUser["LoginNumber"].ToString());

            cookEcm.Values.Add("site_preview", "0");

            cookEcm.Values.Add("langvalue", "");

            System.Configuration.AppSettingsReader objConfigSettings = new System.Configuration.AppSettingsReader();

            string i_mLangId = objConfigSettings.GetValue("ek_DefaultContentLanguage", typeof(string)).ToString();

            cookEcm.Values.Add("DefaultLangauge", i_mLangId);

            cookEcm.Values.Add("NavLanguage", i_mLangId);

            cookEcm.Values.Add("SiteLanguage", i_mLangId);

            cookEcm.Values.Add("LastValidLanguageID", i_mLangId);

            cookEcm.Values.Add("UserCulture", i_mLangId);

            cookEcm.Expires = DateTime.Now.AddYears(1);
            bool encryptCookie = false;
            try
            {
                encryptCookie = Convert.ToBoolean(objConfigSettings.GetValue("ek_EnableCookieEncryption", typeof(bool)));
            }
            catch
            {

            }
            if (encryptCookie)
                Response.Cookies.Add(SecureCookie.Encode(cookEcm));
            else
                Response.Cookies.Add(cookEcm);

            System.Web.HttpCookie cookieEktGUID;

            if (Request.Cookies.Get("EktGUID") == null)
            { //create GUID if it doesn't exist

                string strGUID = System.Guid.NewGuid().ToString();

                cookieEktGUID = new HttpCookie("EktGUID", strGUID);

                cookieEktGUID.Path = "/";

                cookieEktGUID.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Add(cookieEktGUID);

                Ektron.Cms.UserAPI m_refUserApi = new Ektron.Cms.UserAPI();

                m_refUserApi.RequestInformationRef.ClientEktGUID = strGUID;

            }

            Response.Write("loginCallback('userid=" +
                cUser["UserID"].ToString() + "&uniqueid=" +
                cUser["LoginNumber"].ToString() + "');");
            //Response.End();
        }
        else
        {
            // if login failed, return 0
            Response.Clear();
            Response.Write("loginCallback(0)");
            //Response.End();
        }
    }

    private static string DecryptString(string value)
    {
        int len = 3 * (value.Length - 1) / 4;
        int offset = (len / 3) + (len % 3) % 2;
        string postfix = (offset != 0 ? value.Substring(value.Length - offset, offset) : "");
        value = (offset != 0 ? value.Remove(value.Length - offset) : value);
        int i;
        for (i = 0; i < value.Length; i += 2)
        {
            value = value.Remove(i, 1);
        }
        string result = value + postfix;
        string result2 = "";
        for (i = 1; i <= result.Length; i++)
        {
            result2 += System.Text.ASCIIEncoding.UTF8.GetChars(new byte[] {
                (byte)(System.Text.ASCIIEncoding.UTF8.GetBytes(new char[] {
                    (char)result[result.Length - i]
                })[0] - (byte)i + 1)
        })[0];
        }
        return result2;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string username = Request.QueryString["u"];     // username
        string password = Request.QueryString["p"];     // password
        string domain = Request.QueryString["d"];       // domain
        string action = Request.QueryString["action"];

        if (action != null && action.ToLower() == "getp")
        {
            // the action "getp" treats "u" as userid and "p" as uniqueid
            SiteAPI siteAPI = new SiteAPI();

            // impersonate the administrator to call GetUserByID
            long callerId = siteAPI.RequestInformationRef.CallerId;
            siteAPI.RequestInformationRef.CallerId = (int)EkConstants.InternalAdmin;
            Microsoft.VisualBasic.Collection userData = siteAPI.EkUserRef.GetUserByID(int.Parse(username), false, false);
            // restore the original caller
            siteAPI.RequestInformationRef.CallerId = callerId;

            // check if the given uniqueid is the same as the stored uniqueid
            if (password != userData["LoginNumber"].ToString())
            {
                // if it isn't return username and password as null
                Response.Write(",");
                Response.End();
                return;
            }

            // otherwise return username and password separated by a comma
            Response.Write((string)userData["Username"] + "," + DecryptString((string)userData["Password"]));
            Response.End();
            return;
        }

        // if the domain is null, then set it to empty string
        domain = (domain == null ? "" : domain);

        // check if we have all the needed parameters
        if (username == null || password == null || Request.ServerVariables["SERVER_NAME"] == null)
        {
            return;
        }

        // and login
        Login(username, password, domain);
    }
}