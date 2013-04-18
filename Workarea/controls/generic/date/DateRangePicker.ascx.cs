using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Ektron.Cms.Common
{

    public partial class DateRangePicker : WorkareaBaseControl
    {
        private string _cssClass = "";

        #region events and delegates

        public delegate void SelectionChangedHandler(object sender, EventArgs e);
        public event SelectionChangedHandler SelectionChanged;

        public delegate void BadDateFormatError(string defaultMessage, string rawDate);

        public event BadDateFormatError BadStartDateFormatErrorHandler
        {
            add
            {
                StartDatePicker.BadDateFormatErrorHandler += new DatePicker.BadDateFormatError(value);
            }
            remove
            {
                StartDatePicker.BadDateFormatErrorHandler -= new DatePicker.BadDateFormatError(value);
            }
        }

        public event BadDateFormatError BadEndDateFormatErrorHandler
        {
            add
            {
                EndDatePicker.BadDateFormatErrorHandler += new DatePicker.BadDateFormatError(value);
            }
            remove
            {
                EndDatePicker.BadDateFormatErrorHandler -= new DatePicker.BadDateFormatError(value);
            }
        }

        public delegate void BadDateRangeHandler(object sender, BadDateRangeEventArgs e);
        public event BadDateRangeHandler BadDateRange;



        #endregion

        #region public properties

        public string StartLabelText
        {
            get { return StartDatePicker.LabelText; }
            set { StartDatePicker.LabelText = value; }
        }

        public DateTime StartDate
        {
            get { return StartDatePicker.Date; }
            set { StartDatePicker.Date = value; }
        }

        public DateTime DefaultStartDate
        {
            get { return StartDatePicker.DefaultDate; }
            set { StartDatePicker.DefaultDate = value; }
        }

        public string EndLabelText
        {
            get { return EndDatePicker.LabelText; }
            set { EndDatePicker.LabelText = value; }
        }

        public DateTime EndDate
        {
            get { return EndDatePicker.Date; }
            set { EndDatePicker.Date = value; }
        }

        public DateTime DefaultEndDate
        {
            get { return EndDatePicker.DefaultDate; }
            set { EndDatePicker.DefaultDate = value; }
        }

        /// <summary>
        /// Used to store and retrieve date with client cookie.
        /// Default to none which prevents persistence.
        /// </summary>
        private string _persistenceId = String.Empty;
        public string PersistenceId
        {
            get { return _persistenceId; }
            set
            {
                _persistenceId = value;
                if (String.IsNullOrEmpty(_persistenceId))
                {
                    StartDatePicker.PersistenceId = "";
                    EndDatePicker.PersistenceId = "";
                }
                else
                {
                    StartDatePicker.PersistenceId = _persistenceId + "_StartDate";
                    EndDatePicker.PersistenceId = _persistenceId + "_EndDate";
                }
            }
        }

        public DateTime MinimumDate
        {
            get { return StartDatePicker.MinimumDate; }
            set { StartDatePicker.MinimumDate = value; EndDatePicker.MinimumDate = value; }
        }

        public DateTime MaximumDate
        {
            get { return EndDatePicker.MaximumDate; }
            set { EndDatePicker.MaximumDate = value; StartDatePicker.MaximumDate = value; }
        }

        public string CssClass
        {
            get { return _cssClass; }
            set
            {
                _cssClass = value;
                DateRangePickerContainer.Attributes.Add("class", "DateRangePickerContainer " + _cssClass);
            }
        }

        public bool InvalidStartDateEntered
        {
            get { return StartDatePicker.InvalidDateEntered; }
        }

        public bool InvalidEndDateEntered
        {
            get { return EndDatePicker.InvalidDateEntered; }
        }

        public string BadStartDateFormatMessage
        {
            get { return StartDatePicker.BadDateFormatMessage; }
            set { StartDatePicker.BadDateFormatMessage = value; }
        }

        public string BadEndDateFormatMessage
        {
            get { return EndDatePicker.BadDateFormatMessage; }
            set { EndDatePicker.BadDateFormatMessage = value; }
        }

        #endregion

        # region protected methods

        protected void Page_Init(object sender, EventArgs e)
        {
            CompareDatesValidator.ErrorMessage = GetMessage("lbl end dte cannot be before start date");

            StartDatePicker.LabelText = GetMessage("generic start date label");
            EndDatePicker.LabelText = GetMessage("generic end date label");
            btnRefresh.AlternateText = GetMessage("generic refresh");
            btnRefresh.Attributes.Add("title", btnRefresh.AlternateText);
            btnRefresh.ImageUrl = CommonApi.AppImgPath + "refresh.png";
            btnRefresh.ToolTip = GetMessage("generic refresh");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (BadDateRange != null)
                {
                    if (!CompareDatesValidator.IsValid)
                    {
                        BadDateRange(this, new BadDateRangeEventArgs(CompareDatesValidator.ErrorMessage, StartDatePicker.Date, EndDatePicker.Date));
                    }
                }
            }
        }

        protected virtual void btnRefresh_OnClick(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        #endregion
    }

    public class BadDateRangeEventArgs : EventArgs
    {
        public string Message = "";
        public DateTime StartDate;
        public DateTime EndDate;

        public BadDateRangeEventArgs() { }

        public BadDateRangeEventArgs(string message, DateTime startDate, DateTime endDate)
        {
            this.Message = message;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}
