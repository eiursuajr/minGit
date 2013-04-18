<%@ WebHandler Language="C#" Class="Ektron.Site.Logout" %>

using System;
using System.Web;

namespace Ektron.Site
{
    public class Logout : IHttpHandler 
    {    
        public void ProcessRequest (HttpContext context) 
        {
            bool bRet;
            if (SiteData.Current.User.IsLoggedIn)
            {
                Ektron.Cms.User.EkUser userObj;
                userObj = Ektron.Cms.ContentAPI.Current.EkUserRef;
                
                try
                {
                    bRet = userObj.LogOutUser(Ektron.Cms.ContentAPI.Current.UserId, userObj.RequestInformation.CookieSite);
                }
                catch(Exception e)
                {
                    return;
                }
            }
            context.Response.Redirect(Ektron.Site.SiteData.Current.Cms.SitePath + "default.aspx");
        }
     
        public bool IsReusable {
            get {
                return false;
            }
        }

    }
}