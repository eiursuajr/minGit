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
using Ektron.Cms.Localization;
using Ektron.Cms.Framework.Localization;
using System.Collections.Generic;
using System.Threading;
using Ektron.Cms.BusinessObjects;
using Ektron.Cms.Framework.User;
using Ektron.Cms.User;


    /// <summary>
    /// Summary description for UrlAliasingModule
    /// </summary>
    public class UrlAliasingModule : IHttpModule
    {
        private const string COOKIE_NAME = "ecm";
        private const string SECURE_COOKIE_NAME = "ecmSecure";
        private const string LANG_TYPE_QUERY_PARAM = "langtype";
        private string CACHE_KEY_FORMAT = "CultureLanguageMap{0}";


        // ****************************************
        // IMPORTANT - Idle Timeout Notice:
        // The amount of time, in minutes, an Administrator or Commerce Admin account 
        // can be idle before they are logged out. The default is 15 minutes as required 
        // for PCI DSS compliance. If you change this to a length of time greater than 
        // 15 minutes, you will not meet PCI DSS compliance requirements.
        private TimeSpan IDLE_TIMEOUT = new TimeSpan(0, 15, 0);
        private long userId = 0;
        //private Ektron.Cms.UserAPI userApi;
        private EkRequestInformation RequestInfo = null;
        private String langtype = String.Empty;
        private static readonly object padlock = new object();

        // ****************************************
        public string _error = "";
        public UrlAliasingModule()
        { }

        #region IHttpModule Members

        public void Dispose()
        { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
            context.AcquireRequestState += new EventHandler(context_AcquireRequestState);
        }

        void context_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (HttpContext.Current.Session != null)
            {

                if (HttpContext.Current.Session["ecmComplianceRequired"] != null
                    && HttpContext.Current.Session["ecmLastAccessed"] != null)
                {
                    bool complianceRequired = (bool)HttpContext.Current.Session["ecmComplianceRequired"];
                    DateTime lastAccessed = (DateTime)HttpContext.Current.Session["ecmLastAccessed"];

                    if (complianceRequired
                        && lastAccessed.Add(IDLE_TIMEOUT) < DateTime.Now)
                    {
                        DeleteEcmCookie();
                        DeleteFormsCookie();
                    }
                    else
                        HttpContext.Current.Session["ecmLastAccessed"] = DateTime.Now;
                }
                else
                {

                    bool complianceMode = GetComplianceModeFromRequestInfo();
                    if (complianceMode && (ComingFromFolderAction(app) || IsCommerceAdmin()))
                    {
                        bool validFolderAction = false;
                        if (ComingFromFolderAction(app))
                        {
                            UserManager userManager = new UserManager();
                            UserData user = userManager.GetItem(userManager.RequestInformation.UserId);
                            DateTime lastAccessed = user.DateModified;
                            validFolderAction = (DateTime.Now.Subtract(lastAccessed).Seconds < 45);
                            if (validFolderAction)
                            {
                                HttpContext.Current.Session["ecmComplianceRequired"] = complianceMode;
                                HttpContext.Current.Session["ecmLastAccessed"] = lastAccessed;
                            }
                        }

                        if (!validFolderAction)
                        {
                            DeleteEcmCookie();
                            DeleteFormsCookie();
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session["ecmLastAccessed"] = DateTime.Now;
                        HttpContext.Current.Session["ecmComplianceRequired"] = false;
                    }
                }
            }
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            RequestInfo = null;

            // Parse Lang type from query string, it might be appended to alias.
            GetLanguage();

            // Process URL Alias
            UrlAliasingBase.ProcessUrl(HttpContext.Current, ref RequestInfo);

            // Again Parse lang type from query string, might have been encoded in alias.
            GetLanguage();
            // Finally set the global Culture
            SetCulture(langtype);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            string originalPath = HttpContext.Current.Items["EkOriginalPath"] as string;
            if (!string.IsNullOrEmpty(originalPath))
            {
                HttpContext.Current.RewritePath(originalPath);
            }
        }

        #endregion

        #region Private Members

        private void GetLanguage()
        {
            string embeddedLangType = String.Empty;
            if (HttpContext.Current.Request.QueryString[LANG_TYPE_QUERY_PARAM] != null && !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString[LANG_TYPE_QUERY_PARAM]))
            {
                embeddedLangType = HttpContext.Current.Request.QueryString[LANG_TYPE_QUERY_PARAM].ToString();
            }

            if (!String.IsNullOrEmpty(embeddedLangType))
            {
                this.langtype = embeddedLangType;
            }
        }

        private void SetCulture(string langType)
        {
            if (!String.IsNullOrEmpty(langType))
            {
                try
                {
                    LocaleData currentlocale = null;

                    String cacheKey = String.Format(CACHE_KEY_FORMAT, langType);
                    if (HttpContext.Current.Cache[cacheKey] == null)
                    {
                        lock (padlock)
                        {
                            if (HttpContext.Current.Cache[cacheKey] == null)
                            {
                                // Look up culture information from CMS

                                LocaleManager localeManager = new LocaleManager(Ektron.Cms.Framework.ApiAccessMode.Admin);
                                Criteria<LocaleProperty> criteria = new Criteria<LocaleProperty>();
                                criteria.AddFilter(LocaleProperty.Id, CriteriaFilterOperator.EqualTo, langType);
                                List<LocaleData> matchingLocals = localeManager.GetList(criteria);
                                if (matchingLocals != null && matchingLocals.Count > 0)
                                {
                                    currentlocale = matchingLocals[0];
                                    HttpContext.Current.Cache.Insert(cacheKey, currentlocale);
                                }
                            }
                        }
                    }

                    if (HttpContext.Current.Cache[cacheKey] != null)
                    {
                        currentlocale = (LocaleData)HttpContext.Current.Cache[cacheKey];
                        // Set asp.net culture property.
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(currentlocale.Culture);
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(currentlocale.UICulture);
                    }


                }
                catch (Exception exc)
                {
                    _error = exc.Message;
                }
            }
        }

        private bool ComingFromFolderAction(HttpApplication app)
        {
            return (app.Request.IsSecureConnection &&
                (
                    app.Request.QueryString["action"] != null &&
                    (
                    app.Request.QueryString["action"].ToLower() == "reestablishsession" ||
                    app.Request.QueryString["action"].ToLower() == "viewcontentbycategory"
                    ) &&
                    app.Request.Url.AbsolutePath.ToLower().EndsWith("workarea/content.aspx")
                ) ||
                app.Request.Url.AbsolutePath.ToLower().EndsWith("workarea/workareatrees.aspx")
                );
        }
        private void DeleteEcmCookie()
        {
            if (HttpContext.Current.Request.Cookies[COOKIE_NAME] != null)
            {
                HttpContext.Current.Request.Cookies[COOKIE_NAME].Path = "/";
                HttpContext.Current.Request.Cookies[COOKIE_NAME].Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.SetCookie(HttpContext.Current.Request.Cookies[COOKIE_NAME]);
            }

            if (HttpContext.Current.Request.Cookies[SECURE_COOKIE_NAME] != null)
            {
                HttpContext.Current.Request.Cookies[SECURE_COOKIE_NAME].Path = "/";
                HttpContext.Current.Request.Cookies[SECURE_COOKIE_NAME].Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.SetCookie(HttpContext.Current.Request.Cookies[SECURE_COOKIE_NAME]);
            }
            else
            {
                HttpCookie secureCookie = new HttpCookie(SECURE_COOKIE_NAME);
                secureCookie.Path = "/";
                secureCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.SetCookie(secureCookie);
            }
        }

        private void DeleteFormsCookie()
        {
            if (HttpContext.Current.Response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName] != null)
                HttpContext.Current.Response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Expires = DateTime.Now;
        }

        bool GetComplianceModeFromRequestInfo()
        {
            bool result = false;
            try
            {
                if (RequestInfo == null) RequestInfo = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
                userId = RequestInfo.UserId;
                result = RequestInfo.CommerceSettings.ComplianceMode;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            return result;
        }

        bool GetComplianceModeFromWebConfig()
        {
            bool result = false;
            System.Collections.Specialized.NameValueCollection CommerceSection;
            try
            {
                CommerceSection = new System.Collections.Specialized.NameValueCollection();
                //CommerceSection = Convert.ChangeType(System.Configuration.ConfigurationManager.GetSection("ektronCommerce"), typeof(System.Collections.Specialized.NameValueCollection));
                CommerceSection = (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection("ektronCommerce");
                if (CommerceSection != null && CommerceSection.Count > 0)
                {
                    bool sucess = false;
                    bool tempVal = bool.TryParse(CommerceSection["ek_ecom_ComplianceMode"], out sucess);
                    if (sucess)
                        result = tempVal;
                }
                CommerceSection = null;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            { CommerceSection = null; }
            return result;
        }

        bool IsCommerceAdmin()
        {
            bool result = false;
            try
            {
                long uid = EkFunctions.ReadDbLong(RequestInformationManager.GetEcmCookie(true)["user_id"]);
                if (uid == 0)
                    return false;
                if (uid == EkConstants.BuiltIn)
                    result = true;
                else
                {
                    IUser usermanager = ObjectFactory.GetUser();
                    result = usermanager.IsUserCommerceAdmin(uid);
                    userId = uid;
                }

            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }

            return result;
        }

        #endregion
    }
