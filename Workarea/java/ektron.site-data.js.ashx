<%@ WebHandler Language="C#" Class="ektron" %>
using System;
using System.Web;
using System.Web.Configuration;
using System.Text;
using System.Globalization;
using System.IO;

public class ektron : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {        
        //set ektron.site.sitedata
        string siteData = @"
            if ('undefined' === typeof(Ektron)) { Ektron = {}; }            
            if ('undefined' === typeof(Ektron.Site)) { Ektron.Site = {}; }
            Ektron.Site.SiteData = " + Ektron.Site.Services.Current.JsonSerializer.Serialize(Ektron.Site.SiteData.Current) + @";
        ";
        context.Response.Write(siteData);
        
        //set header info
        context.Response.ContentType = "application/javascript";
        context.Response.ContentEncoding = Encoding.UTF8;
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.Now);
        context.Response.Cache.SetMaxAge(new TimeSpan(0, 0, 0, 0, 0));
        context.Response.Cache.SetLastModified(DateTime.Now);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}