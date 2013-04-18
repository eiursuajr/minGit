using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Controls.WebCalendarForms;
using Ektron.Cms.Common.Calendar;
using Ektron.Cms;
using Ektron.Cms.Controls.CalendarProvider;
using Telerik.Web.UI;
using System.Text.RegularExpressions;
using Ektron.Cms.Common;
using System.Globalization;

public partial class Workarea_WebCalendar_DefaultTemplate_Display : DisplayTemplate
{
    WebEventData _eventdata;
    PermissionData _permissions;
    EventRadScheduleProvider.CalendarData _source;
    Appointment _appt;
    protected ContentAPI _ContentApi;
    private bool displaycreated = false;

    public override WebEventData EventData
    {
        get
        {
            return _eventdata;
        }
        set
        {
            _eventdata = value;
        }
    }
    public override PermissionData Permissons
    {
        get
        {
            return _permissions;
        }
        set
        {
            _permissions = value;
        }
    }
    public override EventRadScheduleProvider.CalendarData Source
    {
        get
        {
            return _source;
        }
        set
        {
            _source = value;
        }
    }
    public override Appointment AppointmentData
    {
        get
        {
            return _appt;
        }
        set
        {
            _appt = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        this.DataBinding += new EventHandler(Display_DataBinding);
        this.PreRender += new EventHandler(Display_PreRender);
    }

    void Display_PreRender(object sender, EventArgs e)
    {
        getContainer().ToolTip = "";
        CreateDisplay();
    }

    void Display_DataBinding(object sender, EventArgs e)
    {
        CreateDisplay();
    }

    protected void CreateDisplay()
    {
        if (EventData != null && !displaycreated)
        {
            displaycreated = true;
            //EkRequestInformation reqinfo = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
            int length = 100;

            string desc = Regex.Replace(EventData.Description, "<[^>]*>", "");
            desc = Regex.Replace(desc, "[\\s\\r\\n]+", " ");
            if (desc.Length < length) length = desc.Length;
            desc = desc.Substring(0, length);
            if (desc.Length > 0) desc = "Description: " + desc + "<br />";
            if (EventData.Location != "")
            {
                desc += "Location: " + EventData.Location + "<br />";
            }
            desc += "Time: ";
            if (EventData.IsAllDay)
            {
                desc += "All day<br />";
            }
            else
            {
                desc += EventData.EventStart.ToString("t") + " - " + EventData.EventEnd.ToString("t") + "<br />";
            }
            _ContentApi = new ContentAPI();

            string url = _ContentApi.SitePath.ToString() + EventData.Quicklink;
            url += (EventData.Quicklink.Contains("?")) ? "&amp;" : "?";
            url += "dt=" + AppointmentData.Start.ToShortDateString();

            long downloadid = EventData.Id;
            if (EventData.IsVariance && EventData.ParentEventId > 0)
            {
                downloadid = EventData.ParentEventId;
            }
            string downloadurl = _ContentApi.ApplicationPath + "WebCalendar/View/IcalGenerator.ashx?eventid=" + downloadid.ToString()  + "&langid=" + EventData.LanguageId;
            string downloadlink = "<a style=\"float:right;padding-right:4px;\" href=\"" + downloadurl + "\">download</a>&nbsp;&nbsp;";

            string link = "";
            if (!_ContentApi.RequestInformationRef.WorkAreaOperationMode)
            {
                //link = "<a style=\"float:right;\" href=\"#\" onclick=\"window.open('" + url + "'); return false;\">more</a>";
				//}	
				//else
				//{
                link = "<a style=\"float:right;\" href=\"" + url + "\">more</a>";
				description.Text = "<div style=\"padding:10px;\">" + desc + link + downloadlink + "</div>";
				description.Title = EventData.DisplayTitle;
				description.TargetControlID = title.Parent.Parent.Parent.ClientID;
				description.IsClientID = true;
            }
            title.Text = EventData.DisplayTitle;
        }
    }

    protected AppointmentControl getContainer(){
        Control tmp = title;
        while ((tmp as AppointmentControl) == null) tmp = tmp.Parent;
        return tmp as AppointmentControl;
    }
}
