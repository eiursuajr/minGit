<%@ WebHandler Language="C#" Class="ektronCss" %>

using System;
using System.Web;
using System.Web.Configuration;
using System.Text;
using System.Globalization;
using System.IO;
using Microsoft.Security.Application;
using Ektron.Cms.Framework.UI;

public class ektronCss : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        bool canUseClientCache = false;
        
        try
        {
            string idParam = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["id"]);
            string[] ids = idParam.Split(new char[] { ' ' });    
            
            DateTime lastUpdate;
            DateTime.TryParse(context.Request.Headers.Get("If-Modified-Since"), new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out lastUpdate);

            foreach (string id in ids)
            {
                //get file data from cache (if possible)
                RegistrationItem item = RegisterUtilities.GetRegistrationItem(id);

                //ensure cache has not been invalidated
                if (item == null || item.Contents ==null)
                {
                    System.Web.UI.Control control = new System.Web.UI.Control();
                    control.ID = "EktronCssAshx";
                    item = RegisterUtilities.RebuildRegistration(control, id, RegisterUtilities.MinificationFormat.Css);
                }

                //if item exists and has contents, process it
                if (item != null && !String.IsNullOrEmpty(item.Contents))
                {

                    //if cache date is later than last update, invalidate client cache
                    canUseClientCache = item.ReadDateTime.CompareTo(lastUpdate) > 0 ? false : true;

                    //add js
                    context.Response.Write(item.Contents);
                    context.Response.Write(Environment.NewLine + Environment.NewLine);
                }
            }
        }
        catch
        {
            //fail quietly
        }
        finally
        {
            //set header info
            context.Response.ContentType = "text/css";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(365));
            context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0, 0));
            context.Response.Cache.SetLastModified(DateTime.Now);

            //if using client cache, set status code and suppress content
            context.Response.StatusCode = canUseClientCache ? 304 : 200;
            context.Response.SuppressContent = canUseClientCache ? true : false;
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}