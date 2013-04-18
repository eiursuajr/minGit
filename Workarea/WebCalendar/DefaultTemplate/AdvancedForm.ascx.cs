using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Ektron.Cms.API;
using Ektron.Cms;
using Ektron.Cms.Controls.CalendarProvider;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using Ektron.Cms.Content.Calendar;
using Ektron.Cms.Common.Calendar;
using System.Globalization;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;

namespace SchedulerTemplatesCS
{
    /// <summary>
    /// Specifies the advanced form mode.
    /// </summary>
    public enum AdvancedFormMode
    {
        Insert, Edit
    }

    public partial class AdvancedForm : Ektron.Cms.Controls.WebCalendarForms.AdvancedFormBase
    {
        #region Private members
        private static readonly string[] DayOrdinalValues = { "1", "2", "3", "4", "-1" };
        private static readonly string[] DayMaskValues = { 
            ((int) RecurrenceDay.EveryDay).ToString(),
            ((int) RecurrenceDay.WeekDays).ToString(),
            ((int) RecurrenceDay.WeekendDays).ToString(),
            ((int) RecurrenceDay.Sunday).ToString(),
            ((int) RecurrenceDay.Monday).ToString(),
            ((int) RecurrenceDay.Tuesday).ToString(),
            ((int) RecurrenceDay.Wednesday).ToString(),
            ((int) RecurrenceDay.Thursday).ToString(),
            ((int) RecurrenceDay.Friday).ToString(),
            ((int) RecurrenceDay.Saturday).ToString() };
        private string[] DayOrdinalDescriptions;
        private string[] DayMaskDescriptions;
        private readonly string[] InvariantMonthNames;
        private bool FormInitialized
        {
            get
            {
                return Request.Form["AddEventFormDisplay"] == "true";
            }
        }
        private AdvancedFormMode mode = AdvancedFormMode.Insert;
        private ContentAPI _contentApi;
        private long _FolderID = 0;
        private long _SelectedTaxId = 0;
        private bool _IsEventSearchable = true;
        private long SelectedTaxID
        {
            get
            {
                return _SelectedTaxId;
            }

            set
            {
                _SelectedTaxId = value;
            }
        }
        private WebEventData _eventData = null;
        private bool _eventDataFetched = false;
        private CultureInfo _clientCulture = null;
        protected StyleHelper m_refStyle = new StyleHelper();
        #endregion

        #region FormBase Properties
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string Title
        {
            get
            {
                return txtTitle.Text;
            }
            set
            {
                txtTitle.Text = value;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string Location
        {
            get
            {
                return txtLocation.Text;
            }
            set
            {
                txtLocation.Text = value;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string Subject
        {
            get
            {
                return ContentDesigner.Content;
            }

            set
            {
                ContentDesigner.Content = value;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override long Folder
        {
            get
            {
                _FolderID = 0;
                if (sourceSelector != null && sourceSelector.SelectedValue != null && sourceSelector.SelectedValue != "")
                {
                    long.TryParse(sourceSelector.SelectedValue.Split('|')[0], out _FolderID);
                }
                return _FolderID;
            }
            set
            {
                _FolderID = value;
                if (sourceSelector.Items.FindByValue(value.ToString() + "|" + _SelectedTaxId) != null)
                {
                    sourceSelector.SelectedValue = _FolderID.ToString() + "|" + _SelectedTaxId.ToString();
                }
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override bool AllDay
        {
            get
            {
                return AllDayEvent.Checked;
            }
            set
            {
                AllDayEvent.Checked = value;
                if (value)
                {
                    EndTime.Style.Add("display", "none");
                    EndDate.Style.Add("display", "none");
                    StartTime.Style.Add("display", "none");
                    lblEndDate.Style.Add("display", "none");
                }
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string TaxonomyIDs
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                TaxonomySelector.SelectedTaxonomies.ForEach(new Action<long>(delegate(long a) { sb.Append(a.ToString() + ","); }));
                if (sb.Length > 0) sb = sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            set
            {
                List<string> ids = new List<string>();
                ids.AddRange(value.Split(','));
                List<long> tids = ids.ConvertAll<long>(new Converter<string, long>(delegate(string i) { return long.Parse(i); }));
                TaxonomySelector.SelectedTaxonomies.AddRange(tids);
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override DateTime OriginalStartDateTime
        {
            get
            {
                DateTime retval;
                IFormatProvider ci = new CultureInfo(1033);
                if (!DateTime.TryParseExact(hdnOriginalStartDateTime.Value, "yyyy-MM-dd-T-HH:mm:ss", ci, DateTimeStyles.AllowWhiteSpaces, out retval))
                {
                    retval = DateTime.MinValue;
                }
                return retval;
            }
            set
            {
                hdnOriginalStartDateTime.Value = value.ToString("yyyy-MM-dd-T-HH:mm:ss");
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override DateTime Start
        {
            get
            {
                DateTime retval;
                if (!DateTime.TryParse(StartDate.Text + " " + StartTime.Text, ClientCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out retval)) retval = DateTime.MinValue;
                return retval;
            }
            set
            {
                StartDate.Text = value.ToString(ClientCulture.DateTimeFormat.ShortDatePattern);
                StartTime.Text = value.ToString("hh:mm tt");
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override RecurrencePattern Pattern
        {
            get
            {
                if (!RecurrentAppointment.Checked)
                {
                    return null;
                }

                RecurrencePattern submittedPattern = new RecurrencePattern();
                submittedPattern.Frequency = Frequency;
                submittedPattern.Interval = Interval;
                submittedPattern.DaysOfWeekMask = DaysOfWeekMask;
                submittedPattern.DayOfMonth = DayOfMonth;
                submittedPattern.DayOrdinal = DayOrdinal;
                submittedPattern.Month = Month;

                if (submittedPattern.Frequency == RecurrenceFrequency.Weekly)
                {
                    submittedPattern.FirstDayOfWeek = Owner.FirstDayOfWeek;
                }

                return submittedPattern;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override RecurrenceRange Range
        {
            get
            {
                DateTime startDate = Start;
                DateTime endDate = EndDateTime;

                if (AllDay)
                {
                    startDate = startDate.Date;
                    endDate = startDate.Date.AddDays(1);
                }

                RecurrenceRange range = new RecurrenceRange();
                range.Start = startDate;
                range.EventDuration = endDate - startDate;
                range.MaxOccurrences = 0;
                range.RecursUntil = DateTime.MaxValue;

                if (Owner.RecurrenceSupport)
                {
                    if (RepeatGivenOccurrences.Checked)
                    {
                        int maxOccurrences;
                        int.TryParse(RangeOccurrences.Text, out maxOccurrences);
                        range.MaxOccurrences = maxOccurrences;
                    }

                    if (RepeatUntilGivenDate.Checked && RangeEndDateTime != DateTime.MinValue)
                    {
                        range.RecursUntil = RangeEndDateTime;
                    }
                }

                return range;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string Metadata
        {
            get
            {
                StringBuilder metaXML = new StringBuilder();
                foreach (object key in MetadataSelector.Metadata.Keys)
                {
                    metaXML.Append("<meta id=\"");
                    metaXML.Append(((object[])MetadataSelector.Metadata[key])[0]);
                    metaXML.Append("\">");
                    metaXML.Append(EkFunctions.HtmlEncode(((object[])MetadataSelector.Metadata[key])[2].ToString()));
                    metaXML.Append("</meta>");
                }
                return "<metadata>" + metaXML.ToString() + "</metadata>";
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string ManualAliasName
        {
            get
            {
                return txtAliasName.Text;
            }
            set
            {
                txtAliasName.Text = value;
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override string ManualAliasExtension
        {
            get
            {
                return hdnAliasExtension.Value;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) {
                    hdnAliasExtension.Value = ddlAliasExtensions.SelectedValue = value;
                }
            }
        }
        [Bindable(BindableSupport.Yes, BindingDirection.TwoWay)]
        public override bool IsEventSearchable
        {
            get
            {
                return _IsEventSearchable;
            }
            set
            {
                _IsEventSearchable = value;
            }
        }
        public AdvancedFormMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        public string RecurrenceRuleText
        {
            get
            {
                if (Owner.RecurrenceSupport)
                {
                    RecurrenceRule rrule = RecurrenceRule.FromPatternAndRange(Pattern, Range);

                    if (rrule == null)
                    {
                        return string.Empty;
                    }

                    RecurrenceRule originalRule;
                    if (RecurrenceRule.TryParse(OriginalRecurrenceRule.Value, out originalRule))
                    {
                        rrule.Exceptions = originalRule.Exceptions;
                    }

                    return rrule.ToString();
                }

                return string.Empty;
            }

            set
            {
                OriginalRecurrenceRule.Value = value;
            }
        }
        public DateTime EndDateTime
        {
            get
            {
                DateTime retval;
                if (AllDay) return Start.Date.AddDays(1);
                if (!DateTime.TryParse(EndDate.Text + " " + EndTime.Text, ClientCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out retval)) retval = DateTime.MinValue;
                return retval;
            }
            set
            {
                EndDate.Text = value.ToString(ClientCulture.DateTimeFormat.ShortDatePattern);
                EndTime.Text = value.ToString("hh:mm tt");
            }
        }
        public DateTime RangeEndDateTime
        {
            get
            {
                DateTime retval;
                if (!DateTime.TryParse(RangeEndDate.Text + " " + RangeEndTime.Text, ClientCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out retval)) retval = DateTime.MinValue;
                return retval;
            }
            set
            {
                RangeEndDate.Text = value.ToString(ClientCulture.DateTimeFormat.ShortDatePattern);
                RangeEndTime.Text = value.ToString("hh:mm tt");
            }
        }
        public WebEventData EventData
        {
            get {
                if (_eventDataFetched == false)
                {
                    _eventData = GetEventfromAppointment();
                    _eventDataFetched = true;
                }
                return _eventData;
            }
        }
        #endregion

        #region Protected properties
        protected ContentAPI ContentApi
        {
            get { if (_contentApi == null) _contentApi = new ContentAPI(); return _contentApi; }
        }
        protected RadScheduler Owner
        {
            get
            {
                return Appointment.Owner;
            }
        }
        protected EventRadScheduleProvider Provider
        {
            get
            {
                return (Owner.Provider as EventRadScheduleProvider);
            }
        }
        protected Appointment Appointment
        {
            get
            {
                SchedulerFormContainer container = (SchedulerFormContainer)BindingContainer;
                return container.Appointment;
            }
        }

        protected RecurrenceFrequency Frequency
        {
            get
            {
                if (RecurrentAppointment != null && RecurrentAppointment.Checked)
                {
                    if (RepeatFrequencyDaily != null && RepeatFrequencyDaily.Checked)
                    {
                        return RecurrenceFrequency.Daily;
                    }

                    if (RepeatFrequencyWeekly != null && RepeatFrequencyWeekly.Checked)
                    {
                        return RecurrenceFrequency.Weekly;
                    }

                    if (RepeatFrequencyMonthly != null && RepeatFrequencyMonthly.Checked)
                    {
                        return RecurrenceFrequency.Monthly;
                    }

                    if (RepeatFrequencyYearly != null && RepeatFrequencyYearly.Checked)
                    {
                        return RecurrenceFrequency.Yearly;
                    }
                }

                return RecurrenceFrequency.None;
            }
        }
        protected int Interval
        {
            get
            {
                switch (Frequency)
                {
                    case RecurrenceFrequency.Daily:
                        if (RepeatEveryNthDay.Checked)
                        {
                            return int.Parse(DailyRepeatInterval.Text);
                        }
                        break;

                    case RecurrenceFrequency.Weekly:
                        return int.Parse(WeeklyRepeatInterval.Text);

                    case RecurrenceFrequency.Monthly:
                        if (RepeatEveryNthMonthOnDate.Checked)
                        {
                            return int.Parse(MonthlyRepeatIntervalForDate.Text);
                        }

                        return int.Parse(MonthlyRepeatIntervalForGivenDay.Text);
                }

                return 0;
            }
        }
        protected RecurrenceDay DaysOfWeekMask
        {
            get
            {
                switch (Frequency)
                {
                    case RecurrenceFrequency.Daily:
                        return (RepeatEveryWeekday.Checked) ? RecurrenceDay.WeekDays : RecurrenceDay.EveryDay;

                    case RecurrenceFrequency.Weekly:
                        RecurrenceDay finalMask = RecurrenceDay.None;
                        finalMask |= WeeklyWeekDayMonday.Checked ? RecurrenceDay.Monday : finalMask;
                        finalMask |= WeeklyWeekDayTuesday.Checked ? RecurrenceDay.Tuesday : finalMask;
                        finalMask |= WeeklyWeekDayWednesday.Checked ? RecurrenceDay.Wednesday : finalMask;
                        finalMask |= WeeklyWeekDayThursday.Checked ? RecurrenceDay.Thursday : finalMask;
                        finalMask |= WeeklyWeekDayFriday.Checked ? RecurrenceDay.Friday : finalMask;
                        finalMask |= WeeklyWeekDaySaturday.Checked ? RecurrenceDay.Saturday : finalMask;
                        finalMask |= WeeklyWeekDaySunday.Checked ? RecurrenceDay.Sunday : finalMask;

                        return finalMask;

                    case RecurrenceFrequency.Monthly:
                        if (RepeatEveryNthMonthOnGivenDay.Checked)
                        {
                            return (RecurrenceDay)Enum.Parse(typeof(RecurrenceDay), MonthlyDayMaskDropDown.SelectedValue);
                        }
                        break;

                    case RecurrenceFrequency.Yearly:
                        if (RepeatEveryYearOnGivenDay.Checked)
                        {
                            return (RecurrenceDay)Enum.Parse(typeof(RecurrenceDay), YearlyDayMaskDropDown.SelectedValue);
                        }
                        break;
                }

                return RecurrenceDay.None;
            }
        }
        protected int DayOfMonth
        {
            get
            {
                switch (Frequency)
                {
                    case RecurrenceFrequency.Monthly:
                        return (RepeatEveryNthMonthOnDate.Checked ? int.Parse(MonthlyRepeatDate.Text) : 0);

                    case RecurrenceFrequency.Yearly:
                        return (RepeatEveryYearOnDate.Checked ? int.Parse(YearlyRepeatDate.Text) : 0);
                }

                return 0;
            }
        }
        protected int DayOrdinal
        {
            get
            {
                switch (Frequency)
                {
                    case RecurrenceFrequency.Monthly:
                        if (RepeatEveryNthMonthOnGivenDay.Checked)
                        {
                            return int.Parse(MonthlyDayOrdinalDropDown.SelectedValue);
                        }
                        break;

                    case RecurrenceFrequency.Yearly:
                        if (RepeatEveryYearOnGivenDay.Checked)
                        {
                            return int.Parse(YearlyDayOrdinalDropDown.SelectedValue);
                        }
                        break;
                }

                return 0;
            }
        }
        protected RecurrenceMonth Month
        {
            get
            {
                if (Frequency == RecurrenceFrequency.Yearly)
                {
                    string selectedMonth;

                    if (RepeatEveryYearOnDate.Checked)
                    {
                        selectedMonth = YearlyRepeatMonthForDate.SelectedValue;
                    }
                    else
                    {
                        selectedMonth = YearlyRepeatMonthForGivenDay.SelectedValue;
                    }

                    return (RecurrenceMonth)Enum.Parse(typeof(RecurrenceMonth), selectedMonth);
                }

                return RecurrenceMonth.None;
            }
        }
        protected CultureInfo ClientCulture
        {
            get
            {
                if (_clientCulture == null)
                {
                    try
                    {
                        _clientCulture = new CultureInfo(ContentApi.RequestInformationRef.UserCulture);
                    }
                    catch
                    {
                        _clientCulture = new CultureInfo(ContentApi.RequestInformationRef.DefaultContentLanguage);
                    }
                }
                return _clientCulture;
            }
        }
        #endregion

        public AdvancedForm()
        {
            InvariantMonthNames = new string[12];
            Array.Copy(Enum.GetNames(typeof(RecurrenceMonth)), 1, InvariantMonthNames, 0, 12);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Provider.DataSources.Count > 0)
            {
                try
                {
                    EventRadScheduleProvider.CalendarData mysrc;
                    mysrc = Provider.DataSources.Find(new Predicate<EventRadScheduleProvider.CalendarData>(
                        delegate(EventRadScheduleProvider.CalendarData cd)
                        {
                            return cd.PermissionData.CanEdit;
                        }));
                    if (mysrc != null)
                    {
                        string content_stylesheet = this.ContentApi.GetStyleSheetByFolderID(mysrc.FolderID);
                        ContentDesigner.Stylesheet = GetFullyQualifiedURL(ContentApi.SitePath + content_stylesheet);
                    }
                }
                catch (Exception ex){
                    string _error = ex.Message;
                }
            }
            string path = "";
            if (ContentApi.RequestInformationRef.IsMembershipUser == 1)
            {
                path = ContentApi.RequestInformationRef.ApplicationPath + "WebCalendar/DefaultTemplate/MembershipInterface.xml";
            }
            else
            {
                path = ContentApi.RequestInformationRef.ApplicationPath + "WebCalendar/DefaultTemplate/CMSUserInterface.xml";
            }
            ContentDesigner.ToolsFile = GetFullyQualifiedURL(path);

            InitializeStrings();
            PopulateDescriptions();
            InitializeMonthlyRecurrenceControls();
            InitializeYearlyRecurrenceControls();
        }

        protected string GetFullyQualifiedURL(string s){
            Uri Result = new Uri(this.Page.Request.Url, s);
            return Result.ToString();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (MetadataSelector.MetadataRequired && phMetadata.Visible == false)
            {
                phMetadata.Visible = MetadataSelector.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Packages.jQuery.jQueryUI.ThemeRoller.Register(this);
            Packages.jQuery.jQueryUI.Tabs.Register(this);
            Packages.jQuery.jQueryUI.Datepicker.Register(this);

            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            JS.RegisterJSInclude(this, cmsContextService.UIPath + "/js/jQuery/Plugins/globinfo/ektron.glob." + ClientCulture.Name + ".js", "EktronGlobalCulture_" + ClientCulture.Name + "_JS");
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronBlockUiJS);
            JS.RegisterJSInclude(this, ContentApi.AppPath + "WebCalendar/DefaultTemplate/timeselector/jquery.ptTimeSelect.js", "WebCalendarAdvancedFormTimePickerJS");
            JS.RegisterJSInclude(this, ContentApi.AppPath + "WebCalendar/defaulttemplate/advancedform.js", "WebCalendarAdvancedFormJS");

            labelfortitle.Text = ContentApi.EkMsgRef.GetMessage("generic title label");
            labelforlocation.Text = ContentApi.EkMsgRef.GetMessage("generic location") + ":";

            UpdateButton.ValidationGroup = Owner.ValidationGroup;
            UpdateButton.CommandName = Mode == AdvancedFormMode.Edit ? "Update" : "Insert";
            UpdateButton.Command += new CommandEventHandler(CommandHandler);
            CancelButton.Command += new CommandEventHandler(CancelButton_Command);
            AdvancedEditCloseButton.Command += new CommandEventHandler(CancelButton_Command);

            AdvCalendarSelect.Visible = (Provider.DataSources.Count > 1);
            long lastselectedfolder = Folder;
            sourceSelector.Items.Clear();

            foreach (EventRadScheduleProvider.CalendarData cd in Provider.DataSources)
            {
                if (cd.PermissionData.CanAdd)
                {
                    FolderData fd = ContentApi.GetFolderById(cd.FolderID, false, false);
                    IsEventSearchable = fd.IscontentSearchable;
                    Ektron.Cms.API.User.User uapi = new Ektron.Cms.API.User.User();
                    Ektron.Cms.API.Community.CommunityGroup cgapi = new Ektron.Cms.API.Community.CommunityGroup();
                    if (fd != null)
                    {
                        string name = "";
                        switch (cd.sourceType)
                        {
                            case Ektron.Cms.Controls.SourceType.SystemCalendar:
                                name = "System Calendar: " + fd.Name + " (ID: " + cd.FolderID + ")";
                                break;
                            case Ektron.Cms.Controls.SourceType.GroupCalendar:
                                CommunityGroupData cgd = cgapi.GetCommunityGroupByID(cd.defaultId);
                                name = "Group Calendar: " + cgd.GroupName + " (Group ID: " + cd.defaultId + ")";
                                break;
                            case Ektron.Cms.Controls.SourceType.UserCalendar:
                                UserData thisUser = uapi.GetUser(cd.defaultId, false, true);
                                if (cd.defaultId == 0 || cd.defaultId == ContentApi.UserId)
                                {
                                    name = "My Calendar (" + thisUser.DisplayUserName + ")";
                                }
                                else
                                {
                                    name = "User Calendar: " + thisUser.DisplayUserName + " (User ID: " + cd.defaultId + ")";
                                }
                                break;
                        }
                        sourceSelector.Items.Add(new ListItem(name, cd.FolderID.ToString() + "|" + cd.SelectedTaxID.ToString()));
                    }
                }
            }
            if (Provider.DataSources.Count == 1)
            {
                SelectedTaxID = Provider.DataSources[0].SelectedTaxID;
                Folder = Provider.DataSources[0].FolderID;
            }
            if (sourceSelector.SelectedValue == string.Empty && sourceSelector.Items.Count > 0)
            {
                sourceSelector.Items[0].Selected = true;
            }
            if (lastselectedfolder != 0)
            {
                Folder = lastselectedfolder;
            }
            if (EventData != null)
            {
                Folder = EventData.FolderId;
                sourceSelector.Enabled = false;
            }
            if (Folder > 0)
            {
                TaxonomySelector.FolderID = Folder;
                MetadataSelector.FolderID = Folder;
                if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.UrlAliasing, false))
                {
                    Ektron.Cms.UrlAliasing.UrlAliasSettingsApi _aliasSettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
                    FolderData fd = ContentApi.GetFolderById(Folder, false, false);
                    if (_aliasSettings.IsManualAliasEnabled)
                    {
                        if (ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias))
                        {
                            phAliases.Visible = phAliasTab.Visible = pnlManualAlias.Visible = true;
                            aliasRequired.InnerText = fd.AliasRequired.ToString().ToLower();
                        }
                    }
                    if (_aliasSettings.IsAutoAliasEnabled)
                    {
                        System.Collections.Generic.List<UrlAliasAutoData> autoAliasList = new System.Collections.Generic.List<UrlAliasAutoData>();
                        Ektron.Cms.UrlAliasing.UrlAliasAutoApi autoAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasAutoApi();
                        phAliases.Visible = phAliasTab.Visible = pnlAutoAlias.Visible = true;
                        if (EventData != null)
                        {
                            autoAliasList = autoAliasApi.GetListForContent(EventData.Id);
                            rpAutoAlias.DataSource = autoAliasList;
                        }
                    }

                     //-------------------DisplayTabs Based on selected options from Folder properties----------------------------------
                    if (((fd.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) == (int)EkEnumeration.FolderTabDisplaySettings.AllTabs) && fd.DisplaySettings != 0)
                    {
                       
                        if ((fd.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.MetaData) == (int)EkEnumeration.FolderTabDisplaySettings.MetaData)
                        {
                          phMetadata.Visible=  MetadataSelector.Visible = true; 
                        }
                        else
                        {
                            if (!MetadataSelector.MetadataRequired)
                                phMetadata.Visible = MetadataSelector.Visible = false;
                        }
                        if ((_aliasSettings.IsManualAliasEnabled || _aliasSettings.IsAutoAliasEnabled) && _contentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias)) //And Not (m_bIsBlog)
                        {
                            if ((fd.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Aliasing) == (int)EkEnumeration.FolderTabDisplaySettings.Aliasing)
                            { phAliases.Visible = phAliasTab.Visible = pnlManualAlias.Visible = pnlAutoAlias.Visible = true; }
                            else
                            {
                                if (!fd.AliasRequired)
                                    phAliases.Visible = phAliasTab.Visible = pnlManualAlias.Visible = pnlAutoAlias.Visible = false;
                            }
                        }                                       
                                               
                        if ((fd.DisplaySettings & (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy) == (int)EkEnumeration.FolderTabDisplaySettings.Taxonomy)
                        { phTaxonomyTab.Visible = phTaxonomy.Visible = true; }
                        else
                        {
                            if (!fd.IsCategoryRequired)
                                phTaxonomyTab.Visible = phTaxonomy.Visible = false;
                        }
                
                    }
                    //-------------------DisplayTabs Based on selected options from Folder properties End-----------------------------
                }
            }

            if (!FormInitialized)
            {
                initHiddenData();
                InitializeManualAliases();
                PrefillEventControls();
                PrefillRecurrenceControls();
                UpdateResetExceptionsVisibility();
                bool failed = false;
                string initform = String.Format("Ektron.WebCalendar.AdvancedForm.init(\"{0}\", \"{1}\", {2});", Owner.ClientID, ContentApi.AppPath, ContentApi.RequestInformationRef.WorkAreaOperationMode.ToString().ToLower());
                try
                {
                    JavaScript.RegisterJavaScriptBlock(this, initform);
                }
                catch
                {
                    failed = true;
                }
                if (failed || Controls.IsReadOnly)
                {
                    //we're apparently in a full postback which doesn't care for registerjsblock
                    extrascript.Text = "<script type=\"text/javascript\" defer=\"defer\"> window.setTimeout(function(){" + initform + "}, 750); </script>";
                    extrascript.Visible = true;
                }
            }
            else
            {
                extrascript.Visible = false;
            }
            btnHelp.Text = "<li class=\"actionbarDivider\">&nbsp;</li>" + m_refStyle.GetHelpButton("editevent", "");
        }

        void CancelButton_Command(object sender, CommandEventArgs e)
        {
            //called on cancel
            if (Request["showAddEventForm"] != null)
            {
                Response.Redirect(Request.Url.PathAndQuery.Replace("&showAddEventForm=true", ""), true);
            }
            RaiseBubbleEvent(this, (EventArgs)e);
        }

        void CommandHandler(object sender, CommandEventArgs e)
        {
            //called on save
            if (Request["showAddEventForm"] != null)
            {
                Response.Redirect(Request.Url.PathAndQuery.Replace("&showAddEventForm=true", ""), false);
            }
            RaiseBubbleEvent(this, (EventArgs)e);
        }

        protected void BasicControlsPanel_DataBinding(object sender, EventArgs e)
        {
            Start = Appointment.Start;
            OriginalStartDateTime = Appointment.Start;
            EndDateTime = Appointment.End;
            if (Appointment.Start == Appointment.Start.Date && Appointment.Duration == new TimeSpan(1, 0, 0, 0) && Appointment.ID == null)
            {
                AllDay = true;
            }
        }

        protected void PrefillEventControls()
        {
            if (EventData != null)
            {
                Title = EventData.DisplayTitle;
                Location = EventData.Location;
                Subject = EventData.Description;
                AllDay = EventData.IsAllDay;

                MetadataSelector.ContentID = EventData.Id;
                MetadataSelector.MetaUpdateString = "update";

                TaxonomyBaseData[] tbd = _contentApi.ReadAllAssignedCategory(EventData.Id);
                TaxonomySelector.SelectedTaxonomies.Clear();
                foreach(TaxonomyBaseData t in tbd){
                    TaxonomySelector.PreselectedTaxonomies.Add(t.Id);
                }
                TaxonomySelector.ForceFill();

                Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliaslist = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
                Ektron.Cms.Common.UrlAliasManualData event_alias = new Ektron.Cms.Common.UrlAliasManualData(0, 0, string.Empty, string.Empty);
                event_alias = m_aliaslist.GetDefaultAlias(EventData.Id);
                if (event_alias != null)
                {
                    ManualAliasExtension = event_alias.FileExtension;
                    ManualAliasName = event_alias.AliasName;
                }
            }
            else if (Provider.DataSources.Count > 0)
            {
                TaxonomySelector.defaultTaxID = Provider.DataSources[0].SelectedTaxID;
                TaxonomySelector.ForceFill();
            }
        }

        protected void initHiddenData()
        {
            initUserCulture.Text = ClientCulture.Name;
            initTimeDisplayFormat.Text = ClientCulture.DateTimeFormat.ShortTimePattern;
            initErrTitleRequired.Text = ContentApi.EkMsgRef.GetMessage("event title required");
            initErrStartRequired.Text = ContentApi.EkMsgRef.GetMessage("event start date time invalid");
            initErrEndRequired.Text = ContentApi.EkMsgRef.GetMessage("event end date time invalid");
            initErrMetaDataRequired.Text = ContentApi.EkMsgRef.GetMessage("event metadata required");
            initErrTaxonomyRequired.Text = ContentApi.EkMsgRef.GetMessage("event taxonomy required");
            initErrAliasRequired.Text = ContentApi.EkMsgRef.GetMessage("js manual alias req");
            initErrStartBeforeEnd.Text = ContentApi.EkMsgRef.GetMessage("event start datetime before end datetime");
            initLocationMaxLength.Text = ContentApi.EkMsgRef.GetMessage("lbl location max length");
            initTitleMaxLength.Text = ContentApi.EkMsgRef.GetMessage("lbl title max length");
            initInvalidCharTitle.Text = ContentApi.EkMsgRef.GetMessage("lbl invalid chars in title");
            initInvalidCharLocation.Text = ContentApi.EkMsgRef.GetMessage("lbl invalid chars in location");

            DateTime morning = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 00, 00);
            initTime8AM.Text = morning.ToString(ClientCulture.DateTimeFormat.ShortTimePattern);
            initTime9AM.Text = morning.AddHours(1).ToString(ClientCulture.DateTimeFormat.ShortTimePattern);
            initTimeDayStart.Text = morning.Date.ToString(ClientCulture.DateTimeFormat.ShortTimePattern);
            initCalendarButton.Text = ContentApi.ApplicationPath + "images/UI/Icons/calendar.png";
            initCalendarButtonAlt.Text = ContentApi.EkMsgRef.GetMessage("dtselect: date");
            initTimePickButton.Text = ContentApi.ApplicationPath + "images/UI/Icons/clock.png";
            initTimePickButtonAlt.Text = ContentApi.EkMsgRef.GetMessage("dtselect: time");
            initErrorIcon.Text = ContentApi.ApplicationPath + "images/UI/Icons/error.png";

            if (!TaxonomySelector.HasFolderTaxonomyChoices && Provider.DataSources.Count < 2)
            {
                phTaxonomyTab.Visible = false;
                phTaxonomy.Visible = false;
            }
        }

        protected WebEventData GetEventfromAppointment()
        {
            long folderid = 0;
            long eventid = 0;
            int langid = 0;
            bool isvariance = false;

            WebEventData eventdata = null;
            object apptid = (Appointment.ID == null) ? Appointment.RecurrenceParentID : Appointment.ID;
            if (Appointment.RecurrenceParentID != null) isvariance = true;
            if (apptid != null && apptid.ToString() != "" &&
                EventRadScheduleProvider.EventInfo.ExtractContentID(apptid.ToString(), out folderid, out eventid, out langid))
            {
                WebCalendar w = new WebCalendar(_contentApi.RequestInformationRef);
                w.RequestInformation.ContentLanguage = langid;
                if (isvariance)
                {
                    WebEventVarianceDictionary dic = w.WebEventManager.GetVarianceEventList(folderid, eventid);
                    if (dic != null && dic.ContainsKey(eventid))
                    {
                        eventdata = dic[eventid].Find(delegate(WebEventData wed) { return wed.EventStart == Appointment.Start && wed.IsCancelled == false; });
                    }
                }
                if (eventdata == null)
                {
                    eventdata = w.WebEventManager.GetItem(eventid, langid);
                }
            }
            return eventdata;
        }

        protected void RecurrencePatternPanel_DataBinding(object sender, EventArgs e)
        {
            Telerik.Web.UI.SchedulerFormContainer container = this.Parent as Telerik.Web.UI.SchedulerFormContainer;
            if (container != null && container.Appointment.RecurrenceState == RecurrenceState.Exception ||
                container.Appointment.RecurrenceState == RecurrenceState.Occurrence)
            {
                RecurrentAppointment.Checked = false;
                RecurrentAppointment.Enabled = false;
            }

            RecurrenceRule rrule;
            if (!RecurrenceRule.TryParse(OriginalRecurrenceRule.Value, out rrule))
            {
                RecurrentAppointment.Checked = false;
                return;
            }

            RecurrentAppointment.Checked = true;
            RecurrencePanel.Attributes.Remove("style");

            string interval = rrule.Pattern.Interval.ToString();
            int mask = (int)rrule.Pattern.DaysOfWeekMask;

            switch (rrule.Pattern.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    RepeatFrequencyDaily.Checked = true;
                    RecurrencePatternDailyPanel.Style.Clear();

                    if (rrule.Pattern.DaysOfWeekMask == RecurrenceDay.WeekDays)
                    {
                        RepeatEveryWeekday.Checked = true;
                        RepeatEveryNthDay.Checked = false;
                    }
                    else
                    {
                        RepeatEveryWeekday.Checked = false;
                        RepeatEveryNthDay.Checked = true;
                        DailyRepeatInterval.Text = interval;
                    }
                    break;

                case RecurrenceFrequency.Weekly:
                    RepeatFrequencyWeekly.Checked = true;
                    RecurrencePatternWeeklyPanel.Style.Clear();

                    WeeklyRepeatInterval.Text = interval;

                    WeeklyWeekDayMonday.Checked = (RecurrenceDay.Monday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Monday;
                    WeeklyWeekDayTuesday.Checked = (RecurrenceDay.Tuesday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Tuesday;
                    WeeklyWeekDayWednesday.Checked = (RecurrenceDay.Wednesday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Wednesday;
                    WeeklyWeekDayThursday.Checked = (RecurrenceDay.Thursday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Thursday;
                    WeeklyWeekDayFriday.Checked = (RecurrenceDay.Friday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Friday;
                    WeeklyWeekDaySaturday.Checked = (RecurrenceDay.Saturday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Saturday;
                    WeeklyWeekDaySunday.Checked = (RecurrenceDay.Sunday & rrule.Pattern.DaysOfWeekMask) == RecurrenceDay.Sunday;
                    break;

                case RecurrenceFrequency.Monthly:
                    RepeatFrequencyMonthly.Checked = true;
                    RecurrencePatternMonthlyPanel.Style.Clear();

                    if (0 < rrule.Pattern.DayOfMonth)
                    {
                        RepeatEveryNthMonthOnDate.Checked = true;
                        RepeatEveryNthMonthOnGivenDay.Checked = false;
                        MonthlyRepeatDate.Text = rrule.Pattern.DayOfMonth.ToString();
                        MonthlyRepeatIntervalForDate.Text = interval;
                    }
                    else
                    {
                        RepeatEveryNthMonthOnDate.Checked = false;
                        RepeatEveryNthMonthOnGivenDay.Checked = true;
                        ListItem val = MonthlyDayOrdinalDropDown.Items.FindByValue(rrule.Pattern.DayOrdinal.ToString());
                        if (val != null) MonthlyDayOrdinalDropDown.SelectedValue = rrule.Pattern.DayOrdinal.ToString();
                        MonthlyDayMaskDropDown.SelectedIndex = Array.IndexOf(DayMaskValues, (mask).ToString());
                        MonthlyRepeatIntervalForGivenDay.Text = interval;
                    }
                    break;

                case RecurrenceFrequency.Yearly:
                    RepeatFrequencyYearly.Checked = true;
                    RecurrencePatternYearlyPanel.Style.Clear();

                    if (0 < rrule.Pattern.DayOfMonth)
                    {
                        RepeatEveryYearOnDate.Checked = true;
                        RepeatEveryYearOnGivenDay.Checked = false;
                        YearlyRepeatDate.Text = rrule.Pattern.DayOfMonth.ToString();
                        YearlyRepeatMonthForDate.SelectedIndex = ((int)rrule.Pattern.Month) - 1;
                    }
                    else
                    {
                        RepeatEveryYearOnDate.Checked = false;
                        RepeatEveryYearOnGivenDay.Checked = true;
                        YearlyDayOrdinalDropDown.SelectedValue = rrule.Pattern.DayOrdinal.ToString();
                        YearlyDayMaskDropDown.SelectedIndex = Array.IndexOf(DayMaskValues, (mask).ToString());
                        YearlyRepeatMonthForGivenDay.SelectedIndex = ((int)rrule.Pattern.Month) - 1;
                    }
                    break;
            }
        }

        protected void RecurrenceRangePanel_DataBinding(object sender, EventArgs e)
        {
            RecurrenceRule rrule;
            if (!RecurrenceRule.TryParse(OriginalRecurrenceRule.Value, out rrule))
            {
                return;
            }

            bool occurrencesLimit = (rrule.Range.MaxOccurrences != int.MaxValue);
            bool timeLimit = (rrule.Range.RecursUntil.Date != DateTime.MaxValue.Date);

            if (!occurrencesLimit && !timeLimit)
            {
                RepeatIndefinitely.Checked = true;
                RepeatGivenOccurrences.Checked = false;
                RepeatUntilGivenDate.Checked = false;
            }
            else
                if (occurrencesLimit)
                {
                    RepeatIndefinitely.Checked = false;
                    RepeatGivenOccurrences.Checked = true;
                    RepeatUntilGivenDate.Checked = false;

                    RangeOccurrences.Text = rrule.Range.MaxOccurrences.ToString();
                }
                else
                {
                    RepeatIndefinitely.Checked = false;
                    RepeatGivenOccurrences.Checked = false;
                    RepeatUntilGivenDate.Checked = true;

                    RangeEndDateTime = rrule.Range.RecursUntil;
                }
        }

        protected void ResetExceptions_OnClick(object sender, EventArgs e)
        {
            Owner.RemoveRecurrenceExceptions(Appointment);
            OriginalRecurrenceRule.Value = Appointment.RecurrenceRule;
            ResetExceptions.Text = Owner.Localization.AdvancedDone;
        }

        #region Private methods

        private void InitializeStrings()
        {
            ContentDesigner.Validator.ValidationGroup = Owner.ValidationGroup;
            ContentDesigner.Validator.ErrorMessage = Owner.Localization.AdvancedSubjectRequired;
            ContentDesigner.Validator.Enabled = false;
            AllDayEvent.Text = Owner.Localization.AdvancedAllDayEvent;
            RecurrentAppointment.Text = Owner.Localization.AdvancedRecurrence;
            ResetExceptions.Text = Owner.Localization.AdvancedReset;
            RepeatFrequencyDaily.Text = Owner.Localization.AdvancedDaily;
            RepeatFrequencyWeekly.Text = Owner.Localization.AdvancedWeekly;
            RepeatFrequencyMonthly.Text = Owner.Localization.AdvancedMonthly;
            RepeatFrequencyYearly.Text = Owner.Localization.AdvancedYearly;
            RepeatEveryNthDay.Text = Owner.Localization.AdvancedEvery;
            RepeatEveryWeekday.Text = Owner.Localization.AdvancedEveryWeekday;
            RepeatEveryNthMonthOnDate.Text = Owner.Localization.AdvancedDay;
            RepeatEveryNthMonthOnGivenDay.Text = Owner.Localization.AdvancedThe;
            RepeatEveryYearOnDate.Text = Owner.Localization.AdvancedEvery;
            RepeatEveryYearOnGivenDay.Text = Owner.Localization.AdvancedThe;
            RepeatIndefinitely.Text = Owner.Localization.AdvancedNoEndDate;
            RepeatGivenOccurrences.Text = Owner.Localization.AdvancedEndAfter;
            RepeatUntilGivenDate.Text = Owner.Localization.AdvancedEndByThisDate;
            WeeklyWeekDayMonday.Text = Owner.Culture.DateTimeFormat.DayNames[1];
            WeeklyWeekDayTuesday.Text = Owner.Culture.DateTimeFormat.DayNames[2];
            WeeklyWeekDayWednesday.Text = Owner.Culture.DateTimeFormat.DayNames[3];
            WeeklyWeekDayThursday.Text = Owner.Culture.DateTimeFormat.DayNames[4];
            WeeklyWeekDayFriday.Text = Owner.Culture.DateTimeFormat.DayNames[5];
            WeeklyWeekDaySaturday.Text = Owner.Culture.DateTimeFormat.DayNames[6];
            WeeklyWeekDaySunday.Text = Owner.Culture.DateTimeFormat.DayNames[0];
        }

        private void PopulateDescriptions()
        {
            DayOrdinalDescriptions = new string[5];
            DayOrdinalDescriptions[0] = Owner.Localization.AdvancedFirst;
            DayOrdinalDescriptions[1] = Owner.Localization.AdvancedSecond;
            DayOrdinalDescriptions[2] = Owner.Localization.AdvancedThird;
            DayOrdinalDescriptions[3] = Owner.Localization.AdvancedFourth;
            DayOrdinalDescriptions[4] = Owner.Localization.AdvancedLast;

            DayMaskDescriptions = new string[10];
            DayMaskDescriptions[0] = Owner.Localization.AdvancedMaskDay;
            DayMaskDescriptions[1] = Owner.Localization.AdvancedMaskWeekday;
            DayMaskDescriptions[2] = Owner.Localization.AdvancedMaskWeekendDay;
            Array.Copy(Owner.Culture.DateTimeFormat.DayNames, 0, DayMaskDescriptions, 3, 7);
        }

        private void InitializeManualAliases()
        {
            Ektron.Cms.UrlAliasing.UrlAliasManualApi m_aliaslist = new Ektron.Cms.UrlAliasing.UrlAliasManualApi();
            System.Collections.Generic.List<string> ext_alias = m_aliaslist.GetFileExtensions();
            ListItem[] listitems = new ListItem[ext_alias.Count];
            for (int i = 0; i < ext_alias.Count; i++)
            {
                listitems[i] = new ListItem();
                listitems[i].Value = ext_alias[i];
                if (i == 0) { ManualAliasExtension = ext_alias[i]; }
            }
            ddlAliasExtensions.Items.AddRange(listitems);
        }
        private void InitializeMonthlyRecurrenceControls()
        {
            MonthlyDayOrdinalDropDown.Items.AddRange(CreateComboBoxItemArray(DayOrdinalDescriptions, DayOrdinalValues));
            MonthlyDayMaskDropDown.Items.AddRange(CreateComboBoxItemArray(DayMaskDescriptions, DayMaskValues));
        }

        private void InitializeYearlyRecurrenceControls()
        {
            string[] monthNames = new string[12];
            Array.Copy(Owner.Culture.DateTimeFormat.MonthNames, monthNames, 12);

            YearlyRepeatMonthForDate.Items.AddRange(CreateComboBoxItemArray(monthNames, InvariantMonthNames));

            YearlyDayOrdinalDropDown.Items.AddRange(CreateComboBoxItemArray(DayOrdinalDescriptions, DayOrdinalValues));
            YearlyDayMaskDropDown.Items.AddRange(CreateComboBoxItemArray(DayMaskDescriptions, DayMaskValues));

            YearlyRepeatMonthForGivenDay.Items.AddRange(CreateComboBoxItemArray(monthNames, InvariantMonthNames));
        }

        private void PrefillRecurrenceControls()
        {
            DateTime start = Appointment.Start;

            switch (start.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    WeeklyWeekDaySunday.Checked = true;
                    break;

                case DayOfWeek.Monday:
                    WeeklyWeekDayMonday.Checked = true;
                    break;

                case DayOfWeek.Tuesday:
                    WeeklyWeekDayTuesday.Checked = true;
                    break;

                case DayOfWeek.Wednesday:
                    WeeklyWeekDayWednesday.Checked = true;
                    break;

                case DayOfWeek.Thursday:
                    WeeklyWeekDayThursday.Checked = true;
                    break;

                case DayOfWeek.Friday:
                    WeeklyWeekDayFriday.Checked = true;
                    break;

                case DayOfWeek.Saturday:
                    WeeklyWeekDaySaturday.Checked = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            MonthlyRepeatDate.Text = start.Day.ToString();

            YearlyRepeatMonthForDate.SelectedValue = InvariantMonthNames[start.Month - 1];
            YearlyRepeatMonthForGivenDay.SelectedValue = YearlyRepeatMonthForDate.SelectedValue;
            YearlyRepeatDate.Text = start.Day.ToString();
            OriginalRecurrenceRule.Value = Appointment.RecurrenceRule.ToString();
            DailyRepeatInterval.Text = "1";
            WeeklyRepeatInterval.Text = "1";
            MonthlyRepeatIntervalForDate.Text = "1";
            MonthlyRepeatIntervalForGivenDay.Text = "1";
        }

        private void UpdateResetExceptionsVisibility()
        {
            ResetExceptions.Visible = false;
            RecurrenceRule rrule;
            if (RecurrenceRule.TryParse(Appointment.RecurrenceRule, out rrule))
            {
                ResetExceptions.Visible = rrule.Exceptions.Count > 0;
            }
        }

        private static ListItem[] CreateComboBoxItemArray(string[] descriptions)
        {
            ListItem[] listItems = new ListItem[descriptions.Length];

            for (int i = 0; i < descriptions.Length; i++)
            {
                listItems[i] = new ListItem(descriptions[i]);
            }

            return listItems;
        }

        private static ListItem[] CreateComboBoxItemArray(string[] descriptions, string[] values)
        {
            if (descriptions.Length != values.Length)
            {
                throw new InvalidOperationException("There must be equal number of values and descriptions.");
            }

            ListItem[] listItems = CreateComboBoxItemArray(descriptions);

            for (int i = 0; i < values.Length; i++)
            {
                listItems[i].Value = values[i];
            }

            return listItems;
        }

        #endregion
    }
}
