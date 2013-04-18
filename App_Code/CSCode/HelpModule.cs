using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for HelpModule
/// </summary>
public class HelpModule : IHttpModule
{
	public HelpModule()
	{ }
    public void Dispose()
    { }
    public void Init(HttpApplication context)
    {
        context.EndRequest += new EventHandler(context_EndRequest);
    }

    void context_EndRequest(object sender, EventArgs e)
    {
        if (HttpContext.Current != null 
            && HttpContext.Current.Request.PhysicalPath.ToLower().IndexOf("\\help\\") >= 0
            && HttpContext.Current.Response.StatusCode != 200 
            && HttpContext.Current.Response.StatusCode != 304
            && HttpContext.Current.Request.Url != null)

        {
            Ektron.Cms.ContentAPI capi = new Ektron.Cms.ContentAPI();
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/" + capi.RequestInformationRef.ApplicationPath + "/helpmessage.aspx", true);
        }
        else
            return;
    }
}