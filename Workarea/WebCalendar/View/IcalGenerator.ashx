<%@ WebHandler Language="C#" Class="IcalGenerator" %>

using System;
using System.Web;

public class IcalGenerator : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        Ektron.Cms.Content.Calendar.WebEventManager wem = new Ektron.Cms.Content.Calendar.WebEventManager();
        long calid = 0;
        long eventid = 0;
		int langid = 1033;
        string fileName = "calendar.ics";
        string icalstring = "";
                
        if (context.Request.Params["calid"] != null)
        {
            if (!long.TryParse(context.Request.Params["calid"], out calid)) calid = 0;
        }

        if (context.Request.Params["eventid"] != null)
        {
            if (!long.TryParse(context.Request.Params["eventid"], out eventid)) eventid = 0;
        }

        if (context.Request.Params["langid"] != null)
        {
            int.TryParse(context.Request.Params["langid"], out langid);
        }

        if (eventid > 0)
        {
            Ektron.Cms.Common.Calendar.WebEventData wed = wem.GetItem(eventid, langid);
            if (wed != null)
            {
                fileName = "event-" + wed.Id.ToString() + ".ics";
                icalstring = wem.GetEventICalendarString(wed, true);
            }
            else
            {
                context.Response.Write("Could not retrieve event.");
                context.Response.End();
            }
        }

        if (calid > 0)
        {
            fileName = "calendar-" + calid.ToString() + ".ics";
            icalstring = wem.GetCalendarICalendarString(calid);
        }
        
        context.Response.ContentType = "text/calendar";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
        context.Response.Write(icalstring);
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}