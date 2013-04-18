<%@ WebHandler Language="C#" Class="ektronAnalyticsTracking" %>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Microsoft.Security.Application;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Common;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;
using System.Xml;
using Ektron.Cms.Interfaces.Analytics.Provider;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Framework.Analytics.BusinessAnalytics;
using Ektron.Newtonsoft.Json;

public class ektronAnalyticsTracking : IHttpHandler {
    public bool IsReusable { get { return false; } }

    public void ProcessRequest(HttpContext context) {
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
        context.Response.Cache.SetNoStore();
        
        try {
            if (IsPageTrackingAllowed(context)) {
                BeaconEventData baseObj = GetDataObject(context);
                if (baseObj != null) {
                    /* nothing to do here; all work done internally when object is created */
                }
            }
        }
        catch (Exception ex) {
            EkException.LogException(ex);
        }
    }

    private BeaconEventData GetDataObject(HttpContext context) {
        if (context != null && context.Request != null && context.Request.Form != null && !string.IsNullOrEmpty(context.Request.Form["beacondata"])) {
            string beaconData = context.Request.Form["beacondata"];
            Type type = GetDataType(context, beaconData);
            if (type != null) {
                object[] paramObject = new object[] { context, beaconData };
                return Activator.CreateInstance(type, paramObject) as BeaconEventData;
            }
        }
        return null;
    }

    private Type GetDataType(HttpContext context, string beaconData) {
        if (!string.IsNullOrEmpty(beaconData)) {
            beaconGenericPackage genericPkg = JsonConvert.DeserializeObject<beaconGenericPackage>(context.Server.UrlDecode(beaconData));
            if (genericPkg != null && !string.IsNullOrEmpty(genericPkg.dataType)) {
                return Type.GetType(genericPkg.dataType);
            }
        }
        return null;
    }

    private bool IsPageTrackingAllowed(HttpContext context) {
        // return this.allowedPages.Contains(context.Request.UrlReferrer.AbsolutePath);
        return true; /* stubbed */
    }
}

///////////////////////////////////////////////////////////////////////////////
// JSON Classes, used to deserialize data from client
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// basic container that holds type and payload size info (serialized from client data)
/// </summary>
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class beaconGenericPackage {
    [JsonProperty]
    public string dataType;
    [JsonProperty]
    public int count;
}

/// <summary>
/// click-data container, carries a payload of click-data event-items (serialized from client data)
/// </summary>
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class beaconClickDataPackage : beaconGenericPackage {
    [JsonProperty]
    public clickDataEventItem[] payload;
}

/// <summary>
/// individual click-data event-item (serialized from client data)
/// </summary>
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class clickDataEventItem {
    [JsonProperty]
    public int x;
    [JsonProperty]
    public int y;
    [JsonProperty]
    public int width;
    [JsonProperty]
    public int height;
    [JsonProperty]
    public string locationHref;
    [JsonProperty]
    public string date;
	[JsonProperty]
    public string xml;
}

///////////////////////////////////////////////////////////////////////////////
// Beacon-Data Classes: used to auto-generate and process data
// in preporation for sending to database
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Base-type for handling event data from client; derived types do actual work of 
/// converting the client data into XML for transfer to db via Business-Analytics-EventLogger,
/// and specifying the event type. 
/// </summary>
public abstract class BeaconEventData {
    #region member variables
    private static Object padlock = new Object();
    protected static DateTime timeStamp;
    protected const int BUFFER_LIMIT = 15;
    protected const int BUFFER_TIME_LIMIT = 600; /* 600 seconds = 10 minutes */
    protected HttpContext context;
    protected string beaconData;
    #endregion

    #region constructors
    
    private BeaconEventData() { }
    public BeaconEventData(HttpContext context, string beaconData) { this.context = context; this.beaconData = beaconData; }
    
    #endregion

    #region abstract methods
    
    public abstract string eventType { get; }
    public abstract int eventTypeId { get; }
    
    #endregion

    #region helper methods
    
    public static string MakeElement(string elementName, string elementData) {
        return string.Format("<{0}>", elementName) + elementData + string.Format("</{0}>", elementName);
    }

    public string GetClientDate(string rawDate) {
        try {
            return DateTime.Parse(rawDate).ToString("yyyy-MM-dd hh:mm:ss.fff");
        }
        catch {
            return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        }
    }

    //protected bool IsPageTrackingAllowed() {
    //    // return this.allowedPages.Contains(this.context.Request.UrlReferrer.AbsolutePath);
    //    return true; /* stubbed */
    //}

    protected void AddItem(string itemData) {
        string itemElement = string.Empty;
        //if (IsPageTrackingAllowed(this.context)) {
            itemElement = MakeElement("item",
                MakeElement("eventTypeId", eventTypeId.ToString())
                + MakeElement("eventTypeName", eventType)
                + MakeElement("host", this.context.Request.UrlReferrer.Host)
                + MakeElement("applicationPath", this.context.Request.ApplicationPath)
                + itemData);
        //}

        lock (BeaconEventData.padlock) {
            if (!string.IsNullOrEmpty(itemElement)) {
                if (BeaconEventQueue.Instance.Count == 0) {
                    BeaconEventData.timeStamp = DateTime.Now;
                }

                BeaconEventQueue.Instance.Enqueue(itemElement);
            }

            if ((BeaconEventQueue.Instance.Count >= BUFFER_LIMIT)
                || (DateTime.Now > BeaconEventData.timeStamp.AddSeconds(BUFFER_TIME_LIMIT))) {
                StringBuilder sb = new StringBuilder();
                while (BeaconEventQueue.Instance.Count > 0) {
                    sb.Append(BeaconEventQueue.Instance.Dequeue());
                }
                EventLogger.Log("analyticsTrackingBeaconData", MakeElement("items", sb.ToString()));
            }
        }
    }

    protected string MakeRelativePath(string fullPath) {
        string prefix = this.context.Request.UrlReferrer.Host.Trim('/');
        if (!prefix.Contains("://")) {
            prefix = "://" + prefix;
        }
        return this.context.Server.HtmlEncode(fullPath.Contains(prefix) ? fullPath.Substring(prefix.Length + fullPath.IndexOf(prefix)) : fullPath);
    }
    
    #endregion
}

/// <summary>
/// used to generate click-data XML (will be inserted into individual rows of data in db)
/// </summary>
public class ClickEventData : BeaconEventData {
    #region constructors

    public ClickEventData(HttpContext context, string beaconData) : base(context, beaconData) {
        ProcessData(JsonConvert.DeserializeObject<beaconClickDataPackage>(this.context.Server.UrlDecode(this.beaconData)));
    }

    #endregion

    #region overrides

    public override string eventType { get { return "clickData"; } }
    public override int eventTypeId { get { return 1; } }

    #endregion

    #region worker methods
    
    private void ProcessData(beaconClickDataPackage clickDataPkg) {
        foreach (clickDataEventItem item in clickDataPkg.payload) {
            AddItem(MakeElement("locationHref", this.context.Server.HtmlEncode(item.locationHref))
                + MakeElement("relativeLocationHref", MakeRelativePath(item.locationHref))
                + MakeElement("client_date", GetClientDate(item.date))
                + MakeElement("height", item.height.ToString())
                + MakeElement("width", item.width.ToString())
                + MakeElement("x", item.x.ToString())
                + MakeElement("y", item.y.ToString())
				+ MakeElement("xml", this.context.Server.HtmlEncode(item.xml)));
        }
    }

    #endregion
}

///////////////////////////////////////////////////////////////////////////////
// Helper Classes
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Queue singleton, buffers beacon click data before sending to the data base
/// </summary>
public class BeaconEventQueue {
    #region member variables
    private static readonly Queue<string> instance = new Queue<string>();
    #endregion

    #region constructors
    /// <summary>
    /// static constructor ensures static member variables are not created until the 
    /// accessor-property is called and constructor runs (e.g. when this class is actually used)
    /// </summary>
    static BeaconEventQueue() { }
    private BeaconEventQueue() { }
    #endregion

    #region public methods
    /// <summary>
    /// provides access to the one and only queue instance
    /// </summary>
    public static Queue<string> Instance {
        get { return BeaconEventQueue.instance; }
    }
    #endregion
} 