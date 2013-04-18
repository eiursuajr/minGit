using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Web;

/// <summary>
/// SyncSchedule is a control providing a user interface for
/// entering synchronization schedules.
/// </summary>
public partial class SyncSchedule : System.Web.UI.UserControl
{
    private readonly SiteAPI _siteApi;

    /// <summary>
    /// Constructor
    /// </summary>
    public SyncSchedule()
    {
        Interval = ScheduleInterval.None;
        StartTime = DateTime.Now;
        IsEnabled = true;

        _siteApi = new SiteAPI();
    }

    /// <summary>
    /// Gets or sets the displayed schedule interval selection
    /// </summary>
    public ScheduleInterval Interval { get; set; }

    /// <summary>
    /// Gets or sets the displayed schedule start time selection.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether or not the control is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Initializes the controls UI elements, registering resources
    /// and populating input options.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        RegisterResources();

        for (int i = 0; i < rdoSchedule.Items.Count; i++)
        {
            ListItem item = rdoSchedule.Items[i];

            if (item.Value == ScheduleInterval.None.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync none");
            }
            else if (item.Value == ScheduleInterval.OneTime.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl one time");
            }
            else if (item.Value == ScheduleInterval.Hourly.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync hourly");
            }
            else if (item.Value == ScheduleInterval.Daily.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync daily");
            }
            else if (item.Value == ScheduleInterval.Weekly.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync weekly");
            }
            else if (item.Value == ScheduleInterval.Monthly.ToString())
            {
                item.Text = _siteApi.EkMsgRef.GetMessage("lbl sync monthly");
            }
        }

        // Fill 'One Time' schedule input elements.
        EkDTSelector dateTimeSelector = _siteApi.EkDTSelectorRef;
        dateTimeSelector.formName = "form1";
        dateTimeSelector.formElement = "start_date";
        dateTimeSelector.spanId = "start_date_span";

        // Fill 'Hourly' schedule input elements.
        FillMinuteDropDown(ddlHourlyMinute);

        // Fill 'Daily' schedule input elements.
        FillMinuteDropDown(ddlDailyMinute);
        FillHourDropDown(ddlDailyHour);
        FillAmPmDropDown(ddlDailyAMPM);

        // Fill 'Weekly' schedule input elements.
        FillMinuteDropDown(ddlWeeklyMinute);
        FillHourDropDown(ddlWeeklyHour);
        FillAmPmDropDown(ddlWeeklyAMPM);
        FillDaysOfWeekDropDown(ddlWeeklyDay);

        // Fill 'Monthly' schedule input elements.
        FillMinuteDropDown(ddlMonthlyMinute);
        FillHourDropDown(ddlMonthlyHour);
        FillAmPmDropDown(ddlMonthlyAMPM);
        FillDaysOfMonthDropDown(ddlMonthlyDay);

        lblOneTimeSchedule.Text = lblOneTimeSchedule.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync start datetime");      
        lblHourlySchedule.Text =lblHourlySchedule.ToolTip =  _siteApi.EkMsgRef.GetMessage("lbl sync hourly recur on");        
        lblDailySchedule.Text = lblDailySchedule.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync start time");        
        lblWeeklySchedule.Text =lblWeeklySchedule.ToolTip =  _siteApi.EkMsgRef.GetMessage("lbl sync day of the week");        
        lblMonthlySchedule2.Text = lblMonthlySchedule2.ToolTip = lblWeeklySchedule2.Text =lblWeeklySchedule2.ToolTip =  _siteApi.EkMsgRef.GetMessage("lbl sync start time");       
        lblMonthlySchedule.Text = lblMonthlySchedule.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync day of the month");                
        lblOneTimeDescription.Text =lblOneTimeDescription.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync one time description");        
        lblHourlyDescription.Text =lblHourlyDescription.ToolTip = _siteApi.EkMsgRef.GetMessage("lbl sync hourly description");        
        lblDailyDescription.Text = lblDailyDescription.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync daily description");        
        lblWeeklyDayDescription.Text =lblWeeklyDayDescription.ToolTip =  _siteApi.EkMsgRef.GetMessage("lbl sync weekly description");        
        lblWeeklyTimeDescription.Text =lblWeeklyTimeDescription.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync daily description");        
        lblMonthlyDayDescription.Text = lblMonthlyDayDescription.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync monthly description");        
        lblMonthlyTimeDescription.Text = lblMonthlyTimeDescription.ToolTip =_siteApi.EkMsgRef.GetMessage("lbl sync daily description");        
    }

    protected void Page_PreLoad(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Loads the selected values for each input element.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // If this request is not a post back, set selected
            // values on each input element to those represented
            // by the specified 'Interval' and 'StartTime' properties.

            string currentTime = StartTime.ToString();
            string currentMinute = StartTime.Minute.ToString();
            string currentHour = (((StartTime.Hour + 11) % 12) + 1).ToString();
            string currentDayOfWeek = ((int)StartTime.DayOfWeek).ToString();
            string currentDayOfMonth = StartTime.Day.ToString();
            string currentAmPm = currentTime.Substring(currentTime.Length - 2);

            // Set the default start time selections.

            SetSelectedValue(ddlHourlyMinute, currentMinute);
            SetSelectedValue(ddlDailyMinute, currentMinute);
            SetSelectedValue(ddlDailyHour, currentHour);
            SetSelectedValue(ddlDailyAMPM, currentAmPm);
            SetSelectedValue(ddlWeeklyMinute, currentMinute);
            SetSelectedValue(ddlWeeklyHour, currentHour);
            SetSelectedValue(ddlWeeklyAMPM, currentAmPm);
            SetSelectedValue(ddlWeeklyDay, currentDayOfWeek);
            SetSelectedValue(ddlMonthlyMinute, currentMinute);
            SetSelectedValue(ddlMonthlyHour, currentHour);
            SetSelectedValue(ddlMonthlyAMPM, currentAmPm);
            SetSelectedValue(ddlMonthlyDay, currentDayOfMonth);

            EkDTSelector dateTimeSelector = _siteApi.EkDTSelectorRef;
            dateTimeSelector.targetDate = 
                StartTime.ToString() == DateTime.MaxValue.ToString() ? DateTime.Now : StartTime;

            ltrOneTimeCalendar.Text = dateTimeSelector.displayCultureDateTime(
                IsEnabled,
                string.Empty,
                string.Empty);

            // Set the default interval type selection.

            SetSelectedValue(rdoSchedule, Interval.ToString());
        }
        else
        {
            // If this request is a post back, commit the
            // schedule selections to the 'Interval' and
            // 'StartTime' properties.

            Interval = (ScheduleInterval)Enum.Parse(
                typeof(ScheduleInterval),
                rdoSchedule.SelectedValue);

            switch (Interval)
            {
                case ScheduleInterval.None:
                    StartTime = DateTime.MaxValue;
                    break;
                case ScheduleInterval.OneTime:
                    SetOneTimeStartTime();
                    break;
                case ScheduleInterval.Hourly:
                    SetHourlyStartTime();
                    break;
                case ScheduleInterval.Daily:
                    SetDailyStartTime();
                    break;
                case ScheduleInterval.Weekly:
                    SetWeeklyStartTime();
                    break;
                case ScheduleInterval.Monthly:
                    SetMonthlyStartTime();
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RegisterResources()
    {
        // Register JS resources
        JS.RegisterJS(this, JS.ManagedScript.EktronJS, false);
        JS.RegisterJS(this, "js/Ektron.Workarea.Sync.Schedule.js", "SyncScheduleJS", false);

        // Set hidden values for client scripts
        syncScheduleElementPrefix.Value = ID;
    }

    /// <summary>
    /// Fills a drop down list with minute values. (0-59)
    /// </summary>
    /// <param name="dropDown">Drop down list to be populated</param>
    private void FillMinuteDropDown(DropDownList dropDown)
    {
        for (int i = 0; i < 60; i++)
        {
            dropDown.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
        }
    }

    /// <summary>
    /// Fills a drop down list with hour values. (1-12)
    /// </summary>
    /// <param name="dropDown">Drop down list to be populated</param>
    private void FillHourDropDown(DropDownList dropDown)
    {
        for (int i = 1; i <= 12; i++)
        {
            dropDown.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }

    /// <summary>
    /// Fills a drop down list with AM and PM entries.
    /// </summary>
    /// <param name="dropDown">Drop down list to be populated</param>
    private void FillAmPmDropDown(DropDownList dropDown)
    {
        dropDown.Items.Add(new ListItem("AM", "AM"));
        dropDown.Items.Add(new ListItem("PM", "PM"));
    }

    /// <summary>
    /// Fills a drop down list 'day of the week' values. (Monday, Tuesday, etc.)
    /// </summary>
    /// <param name="dropDown">Drop down list to be populated</param>
    private void FillDaysOfWeekDropDown(DropDownList dropDown)
    {
        for (int i = 0; i < CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Length; i++)
        {
            dropDown.Items.Add(new ListItem(CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i], i.ToString()));
        }
    }

    /// <summary>
    /// Fills an drop down list with day of the month values. (1-31)
    /// </summary>
    /// <param name="dropDown">Drop down list to be populated</param>
    private void FillDaysOfMonthDropDown(DropDownList dropDown)
    {
        for (int i = 1; i <= 31; i++)
        {
            dropDown.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }

    /// <summary>
    /// Sets the selected value for the specified list control.
    /// </summary>
    /// <param name="list">List to apply selected value to</param>
    /// <param name="selectedValue">Value to select</param>
    private void SetSelectedValue(ListControl list, string selectedValue)
    {
        list.SelectedValue = selectedValue;
    }

    /// <summary>
    /// Commits the specified 'hourly' scheduled start time.
    /// </summary>
    private void SetHourlyStartTime()
    {
        int minute;
        if (int.TryParse(ddlHourlyMinute.SelectedValue, out minute))
        {
            StartTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                minute,
                0);
        }
    }

    /// <summary>
    /// Commits the specified 'one time' scheduled start time.
    /// </summary>
    private void SetOneTimeStartTime()
    {
        string startDateInput = Request.Form["start_date"];
        if (!string.IsNullOrEmpty(startDateInput))
        {
            DateTime startTime;
            if (DateTime.TryParse(startDateInput, out startTime))
            {
                StartTime = startTime;
            }
        }
        
        EkDTSelector dateTimeSelector = _siteApi.EkDTSelectorRef;
        dateTimeSelector.targetDate = StartTime;

        ltrOneTimeCalendar.Text = dateTimeSelector.displayCultureDateTime(
            IsEnabled,
            string.Empty,
            string.Empty);
    }

    /// <summary>
    /// Commits the specified 'daily' scheduled start time.
    /// </summary>
    private void SetDailyStartTime()
    {
        int minute;
        int hour;
        bool isAM;

        isAM = ddlDailyAMPM.SelectedValue == "AM";
        if (int.TryParse(ddlDailyHour.SelectedValue, out hour) &&
            int.TryParse(ddlDailyMinute.SelectedValue, out minute))
        {
            StartTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                NormalizeHour(hour, isAM),
                minute,
                0);
        }
    }

    /// <summary>
    /// Commits the specified 'weekly' scheduled start time.
    /// </summary>
    private void SetWeeklyStartTime()
    {
        int minute;
        int hour;
        bool isAM;
        int dayOfWeek;

        isAM = ddlWeeklyAMPM.SelectedValue == "AM";
        if (int.TryParse(ddlWeeklyHour.SelectedValue, out hour) &&
            int.TryParse(ddlWeeklyMinute.SelectedValue, out minute) &&
            int.TryParse(ddlWeeklyDay.SelectedValue, out dayOfWeek))
        {
            StartTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                NormalizeHour(hour, isAM),
                minute,
                0);

            while (StartTime.DayOfWeek != (DayOfWeek)dayOfWeek)
            {
                StartTime = StartTime.AddDays(1);
            }
        }
    }

    /// <summary>
    /// Commits the specified 'weekly' scheduled start time.
    /// </summary>
    private void SetMonthlyStartTime()
    {
        int minute;
        int hour;
        int day;
        bool isAM;
        

        isAM = ddlMonthlyAMPM.SelectedValue == "AM";
        if (int.TryParse(ddlMonthlyHour.SelectedValue, out hour) &&
            int.TryParse(ddlMonthlyMinute.SelectedValue, out minute) &&
            int.TryParse(ddlMonthlyDay.SelectedValue, out day))
        {
            StartTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                NormalizeHour(hour, isAM),
                minute,
                0);

            while (StartTime.Day != day)
            {
                StartTime = StartTime.AddDays(1);
            }
        }
    }

    /// <summary>
    /// Converts 12-hour time to 24-hour time.
    /// </summary>
    /// <param name="hour">Hour value</param>
    /// <param name="isAM">Flag indicating whether the hour is AM or PM</param>
    /// <returns>Hour value in 24-hour time</returns>
    private int NormalizeHour(int hour, bool isAM)
    {
        int retVal = 0;

        if (isAM)
        {
            if (hour == 12)
            {
                retVal = 0;
            }
            else
            {
                retVal = hour;
            }
        }
        else
        {
            retVal = hour + 12;
        }

        return retVal;
    }
}
