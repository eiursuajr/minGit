<%@ WebHandler Language="C#" Class="ektron" %>
using System;
using System.Web;
using System.Web.Configuration;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;

public class ektron : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext httpContext) {
        string json = String.Empty;
        string id = httpContext.Request.QueryString["id"];
        string culture = httpContext.Request.QueryString["culture"];

        System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        if (!String.IsNullOrEmpty(culture))
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
        }
            
        if (!String.IsNullOrEmpty(id))
        {
            switch (id)
            {
                case "user":
                    Ektron.Cms.Interfaces.Context.IUserContextService userContextService = Ektron.Cms.Framework.UI.ServiceFactory.CreateUserContextService();
                    json = this.GetJson("User", jsonSerializer.Serialize(userContextService));
                    break;
                case "cms":
                    Ektron.Cms.Interfaces.Context.ICmsContextService cmsContextService = Ektron.Cms.Framework.UI.ServiceFactory.CreateCmsContextService();
                    json = this.GetJson("Cms", jsonSerializer.Serialize(cmsContextService));
                    break;
                case "localization":
                    Ektron.Cms.Interfaces.Context.ILocalizationContextService localizationContextService = Ektron.Cms.Framework.UI.ServiceFactory.CreateLocalizationContextService();
                    json = this.GetJson("Localization", jsonSerializer.Serialize(localizationContextService));
                    break;

            }
        }
        
        httpContext.Response.Write(json);
        
        //set header info
        httpContext.Response.ContentType = "application/javascript";
        httpContext.Response.ContentEncoding = Encoding.UTF8;
        httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        httpContext.Response.Cache.SetExpires(DateTime.Now);
        httpContext.Response.Cache.SetMaxAge(new TimeSpan(0, 0, 0, 0, 0));
        httpContext.Response.Cache.SetLastModified(DateTime.Now);
    }
 
    private string GetJson(string namespaceObject, string jsonString)
    {
        return @"$ektron.extend(true, Ektron, {Context:{" + namespaceObject + @":" + jsonString + @"}});";
    }
    public bool IsReusable {
        get {
            return false;
        }
    }

}