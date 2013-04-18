<%@ WebHandler Language="C#" Class="deviceTree" %>

using System;
using System.Web;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml;
using Net.SourceForge.WURFL.Core;
using System.Text ;

public class deviceTree : IHttpHandler
{
    private IDevice device;

    private Ektron.Cms.ContentAPI c_api;
    private Ektron.Cms.Common.EkMessageHelper m_ref;
    
    public deviceTree()
    {
        c_api = new Ektron.Cms.ContentAPI();
        m_ref = c_api.EkMsgRef;
        
    }
    
    public void ProcessRequest (HttpContext context)
    {
        IWURFLManagerProvider<IWURFLManager> wurflManagerProvider = ((WURFLManagerProvider)context.Cache["WurflManagerProvider"]) != null ? ((WURFLManagerProvider)context.Cache["WurflManagerProvider"]) : null;
        if (wurflManagerProvider == null)
        {
            wurflManagerProvider = new WURFLManagerProvider();
            context.Cache.Add("WurflManagerProvider", wurflManagerProvider, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
        }
        IWURFLManager wurflManager = wurflManagerProvider.WURFLManager;
        context.Response.ContentType = "text/plain";
        if (!String.IsNullOrEmpty(context.Request["detail"]))
        {
            device = wurflManager.GetDeviceForRequest(context.Request["detail"].ToString());
            if (device != null)
            {
                context.Response.Write(getcontenttip(device));
            }
        }
        
        context.Response.End();
    }
 
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public string getcontenttip(IDevice device)
    {
      
        try
        {
           StringBuilder sb = new StringBuilder();
                    
            sb.Append("<div class=\"deviceDetails\">");
            sb.Append("    <span> " + m_ref.GetMessage("tooltip device detail") + "</span>");
            sb.Append("     <table>");
            sb.Append("         <tr>");
            sb.Append("             <td>");
            sb.Append("               " + m_ref.GetMessage("tooltip device model") + ":");
            sb.Append("             </td>");
            sb.Append("             <td>");
            sb.Append(                  device.GetCapability("model_name"));
            sb.Append("             </td>");
            sb.Append("         </tr>");
            sb.Append("         <tr>");
            sb.Append("             <td>");
            sb.Append("                " + m_ref.GetMessage("tooltip device manufacturer") + ":");
            sb.Append("             </td>");
            sb.Append("             <td>");
            sb.Append(                  device.GetCapability("brand_name"));
            sb.Append("             </td>");
            sb.Append("         </tr>");
            sb.Append("</div>");
      
            return sb.ToString();
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

}