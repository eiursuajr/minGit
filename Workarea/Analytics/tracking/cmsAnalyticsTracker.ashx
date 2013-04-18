<%@ WebHandler Language="C#" Class="cmsAnalyticsTracker" %>
using System;
using System.Web;
using System.Web.SessionState;
using Ektron.Cms;

public class cmsAnalyticsTracker : IHttpHandler, IRequiresSessionState 
{
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    
    public void ProcessRequest(HttpContext context) 
    {
        try
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            context.Response.Cache.SetNoStore();

            Ektron.Cms.ContentAPI capi = new Ektron.Cms.ContentAPI();
            if (!string.IsNullOrEmpty(context.Request["command"]) && !string.IsNullOrEmpty(context.Request["contentIds"]))
            {
                string contentIds = context.Request["contentIds"];
                string[] aryContentId = contentIds.Split(',');
                long userId = capi.RequestInformationRef.UserId;
                string visitorId = capi.RequestInformationRef.ClientEktGUID;
                string idListHash = contentIds.Replace(",", "_").GetHashCode().ToString();
                string sessionName = "CmsAnalyticsTrackerContentIdList_";
                string uniqueKey = string.Empty;
                if (0 == userId)
                {
                    uniqueKey = idListHash + visitorId.GetHashCode().ToString();
                }
                else
                {
                    uniqueKey = idListHash + userId.ToString().GetHashCode().ToString();
                }

                sessionName += uniqueKey;
                if (context.Session != null && context.Session[sessionName] != null && contentIds == context.Session[sessionName].ToString())
                {
                    switch (context.Request["command"].ToLower())
                    {
                        case "trackvisit":
                            string url = context.Request["url"];
                            if (!string.IsNullOrEmpty(url) &&
                                !url.StartsWith("/"))
                            {
                                url = new Uri(url).AbsolutePath;
                            }
                            string referrerUrl = context.Request["referrer"];
                            foreach (string Id in aryContentId)
                            {
                                long contentId = 0;
                                if (long.TryParse(Id, out contentId))
                                {
                                    Ektron.Cms.AnalyticsAPI.TrackVisit(contentId, referrerUrl, url);
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unknown action: " + context.Request["command"]);
                    }
                }
                
                context.Session[sessionName] = null;
            }
        }
        catch (Exception ex) 
        {
            EkException.LogException(ex);
        }
    }
}