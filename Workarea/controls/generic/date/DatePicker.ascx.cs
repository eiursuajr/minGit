using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Ektron.Cms.API;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;

namespace Ektron.Cms.Common {

	[ValidationProperty("Date")]
	public partial class DatePicker : WorkareaBaseControl
	{
        
        #region member variables

        private ContentAPI _refContentApi = null;
        private SiteAPI _refSiteApi = null;
        private string _LabelText = String.Empty;
		private DateTime _minDate = DateTime.MinValue;
		private DateTime _maxDate = DateTime.MaxValue;
		private DateTime _defaultDate = DateTime.MinValue;
        private bool _defaultDateSet = false;
		private string _cssClass = String.Empty;
        private string _persistenceId = String.Empty;
        private bool _invalidDateEntered = false;
        private string _badDateFormatMessage;
        private const string ERROR_CLASSNAME = "DatePickerContainer_BadDateError";
        private System.Globalization.CultureInfo _cult = null;

        #endregion

        #region constructors

        public DatePicker() {
            _refContentApi = new ContentAPI();
            _refSiteApi = new SiteAPI();
            if (_refContentApi.UserId > 0)
            {
                _cult = new System.Globalization.CultureInfo(_refContentApi.UserLanguage);
            }
            else
            {
                _cult = new System.Globalization.CultureInfo(_refContentApi.RequestInformationRef.DefaultContentLanguage);
            }
            LabelText = GetMessage("generic date label");
            BadDateFormatMessage = GetMessage("msg bad date format");
        }

        #endregion

        #region events and delegates

        public delegate void BadDateFormatError(string defaultMessage, string rawDate);

        public event BadDateFormatError BadDateFormatErrorHandler;

        #endregion

        #region public properties

        public string LabelText {
            get { return _LabelText; }
            set { _LabelText = value; }
        }

        public string BadDateFormatMessage {
            get { return _badDateFormatMessage; }
            set { _badDateFormatMessage = value; }
        }

		public DateTime Date {
			get {
                RemoveErrorClass();
                if (!string.IsNullOrEmpty(tbDate.Text)) {
                    DateTime date;
                    if (DateTime.TryParse(tbDate.Text, _cult, System.Globalization.DateTimeStyles.None, out date)) {
                        CookieDate = date;
                        return date;
                    } else {
                        HandleBadDateFormat();
                        return DefaultDate;
                    }
                }

                if (CookieDate.CompareTo(MinimumDate) > 0){
                    tbDate.Text = CookieDate.ToString(_cult.DateTimeFormat.ShortDatePattern);
                    return CookieDate;
                }

                if (_defaultDateSet){
                    tbDate.Text = DefaultDate.ToString(_cult.DateTimeFormat.ShortDatePattern);
                    CookieDate = DefaultDate;
                    return DefaultDate;
                }

                ExpireCookie();
                return MinimumDate;
			}

			set  {
                RemoveErrorClass();
				if (value.CompareTo(MinimumDate) >= 0) {
                    tbDate.Text = value.ToString(_cult.DateTimeFormat.ShortDatePattern);
                    CookieDate = value;
				} else {
					tbDate.Text = String.Empty;
                    ExpireCookie();
				}
			}
		}

		public DateTime MinimumDate {
			get { return _minDate; }
			set { _minDate = value; }
		}

		public DateTime MaximumDate {
			get { return _maxDate; }
			set { _maxDate = value; }
		}

        public DateTime DefaultDate {
			get { return ((_defaultDateSet) ? _defaultDate : MinimumDate); }
			set { 
                _defaultDate = value; 
                _defaultDateSet = true; 
            }
		}

		public string CssClass {
			get { return _cssClass; }
			set {
				_cssClass = value;
				DatePickerContainer.Attributes.Add("class", "DatePickerContainer " + _cssClass); 
			}
		}

		private bool _changeMonth = false;
		public bool ChangeMonth
		{
			get { return _changeMonth; }
			set { _changeMonth = value; }
		}

		private bool _changeYear = false;
		public bool ChangeYear
		{
			get { return _changeYear; }
			set { _changeYear = value; }
		}

		private Uri _buttonImage = null;
		public Uri ButtonImage
		{
			get { return _buttonImage; }
			set { _buttonImage = value; }
		}

		/// <summary>
		/// Used to store and retrieve date with client cookie.
        /// Default to none which prevents persistence.
		/// </summary>
        public string PersistenceId {
            get { return _persistenceId; }
            set { _persistenceId = value; }
        }

        public bool InvalidDateEntered {
            get { return _invalidDateEntered; }
        }

        #endregion

        #region protected properties

        protected DateTime CookieDate{
            get {
                DateTime date;
                if (PersistenceId.Length > 0){
                    HttpCookie cookie = Request.Cookies[PersistenceId];
                    if (cookie != null && DateTime.TryParse(cookie.Value, _cult, System.Globalization.DateTimeStyles.None, out date))
                        return date;
                }
                return MinimumDate;
            }
            set {
                if (PersistenceId.Length > 0){
                    HttpCookie cookie = new HttpCookie(PersistenceId, value.ToShortDateString());
                    cookie.Expires = System.DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Response.Cookies.Add(cookie);
                }
            }
        }

        #endregion

        #region protected methods

        protected void ExpireCookie(){
            if (PersistenceId.Length > 0){
                HttpCookie cookie = new HttpCookie(PersistenceId);
                cookie.Expires = DateTime.MinValue;
                Response.Cookies.Add(cookie);
            }
        }

        protected void HandleBadDateFormat() {
            _invalidDateEntered = true;
            AddErrorClass();

            if (null != BadDateFormatErrorHandler)
            {
                BadDateFormatErrorHandler(BadDateFormatMessage, tbDate.Text);
            }
            else
            {
                if (!string.IsNullOrEmpty(BadDateFormatMessage))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DatePickerContainer_ClientScriptBlock", "alert(\"" + Ektron.Cms.API.JS.Escape(BadDateFormatMessage) + "\");", true);
                }
            }
            
        }

        protected void AddErrorClass() {
            string className = DatePickerContainer.Attributes["class"];
            if (!className.Contains(ERROR_CLASSNAME)){
                    DatePickerContainer.Attributes["class"] += (string.IsNullOrEmpty(className) ? "" : " " + ERROR_CLASSNAME);
                }
        }

        protected void RemoveErrorClass() {
            string className = DatePickerContainer.Attributes["class"];
            if (className.Contains(ERROR_CLASSNAME)) {
                className = className.Replace(ERROR_CLASSNAME, "").Trim();
                DatePickerContainer.Attributes["class"] = className;
            }
        }

        protected void Page_Load(object sender, EventArgs e) 
		{
            if (!Page.IsCallback)
                RegisterIncludes();

			lblDatePicker.Text = LabelText;
		}

        protected void OnPreRender(object sender, EventArgs e){
            if (0 == tbDate.Text.Length){
                // date shown is blank, attempt to recover by forcing a get:
                DateTime mydate = Date;
            }
        }

        protected void RegisterIncludes() 
        {
            string langGroup = "en";
            LanguageData objLangData = _refSiteApi.GetLanguageById(_refContentApi.UserLanguage);

	        Packages.jQuery.jQueryUI.Datepicker.Register(this);
            JS.RegisterJSBlock(this, SetCalendarRegion(langGroup), "EktronDatePickerInitializationJs");
            // CSS
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Packages.jQuery.jQueryUI.ThemeRoller.RegisterCss(this);
        }

        private string SetCalendarRegion(string regionalLang)
        {
            //http://jqueryui.com/demos/datepicker/localization.html
            StringBuilder js = new StringBuilder();
			bool bHasOptions = false;
			bool bHasMinDate = (_minDate != DateTime.MinValue);
			bool bHasMaxDate = (_maxDate != DateTime.MaxValue);
            bool bHasRegional = (regionalLang != "en" && regionalLang != "");
			string strRegionalObject = "";
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			if (bHasRegional)
			{
                cultureInfo = new System.Globalization.CultureInfo(regionalLang);
                Package culturePackage = new Package();
                culturePackage.Components.Add(Packages.jQuery.Plugins.Globalization);
                string currentCultureName = "en-US";
                ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
                if (!String.IsNullOrEmpty(cultureInfo.Name))
                {
                    currentCultureName = cultureInfo.Name;
                }
                JavaScript currentCultureJavaScript = JavaScript.Create(cmsContextService.UIPath + "/js/jQuery/Plugins/globinfo/ektron.glob." + currentCultureName + ".js");
                culturePackage.Components.Add(currentCultureJavaScript);
                culturePackage.Register(this);
                strRegionalObject = "{culture: '" + cultureInfo.Name + "'}";
			}

            js.Append("$ektron('.DatePickerContainer input.DatePicker_input').datepicker(");
			if (_buttonImage != null || _changeMonth || _changeYear || bHasMinDate || bHasMaxDate || "" == regionalLang)
			{
				if (bHasRegional)
				{
					js.Append("$ektron.extend(");
				}
                
				js.Append("{ ");
				if (_buttonImage != null)
				{
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
					js.AppendLine("showOn: \"button\"");
					js.Append(", buttonImage: \"").Append(_buttonImage.AbsoluteUri).AppendLine("\"");
					js.AppendLine(", buttonImageOnly: true");
				}
				if (_changeMonth)
				{
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
					js.AppendLine("changeMonth: true");
				}
				if (_changeYear)
				{
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
					js.AppendLine("changeYear: true");
				}
				if (bHasMinDate)
				{
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
                    js.AppendFormat("minDate: new Date({0:0000}, {1:00}, {2:00})", _minDate.Year, _minDate.Month - 1, _minDate.Day);
				}
				if (bHasMaxDate)
				{
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
					js.AppendFormat("maxDate: new Date({0:0000}, {1:00}, {2:00})", _maxDate.Year, _maxDate.Month - 1, _maxDate.Day);
				}
                if ("" == regionalLang)
                {
					if (bHasOptions) js.Append(", ");
					bHasOptions = true;
					js.Append("dateFormat: 'yy-mm-dd'"); //ISO-8061
				}
                js.Append(" }");

                if (bHasRegional)
                {
                    js.Append(", ");
                    js.Append(strRegionalObject);
                    js.Append(")");
                }

			} 
			else if (bHasRegional)
            {
				js.Append(strRegionalObject);
			}
			js.AppendLine(");");
			// by default it is initially positioned off screen, but may cause scroll bars to appear, so hide it
			js.AppendLine("$ektron(\".ui-datepicker-div\").hide();"); 
            return js.ToString();
        }

        private string[] GetAvailableLocaleJS()
        {
            string[] lang = null;
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(_refContentApi.AppPath + "java/plugins/ui/i18n"));
                FileInfo[] jsfiles = dir.GetFiles("ui.datepicker-*.js");
                int counter = 0;
                lang = new string[jsfiles.Length];
                foreach (FileInfo f in jsfiles)
                {
                    string filename = f.Name;
                    filename = filename.Substring(14, filename.Length - 17); // "ui.datepicker-" == 14 chars, ".js" == 3 chars
                    lang[counter] = filename;
                    counter++;
                }
            }
            else
            {
                lang = new string[1];
                lang[0] = ""; //"en" is not in the list of file.
            }
            return lang;
        }

        private string DetermineTheLanguage(string strLangDefined, string[] arrLang)
        {
            string theLang = "";
            string langCode = "";
            strLangDefined = strLangDefined.ToLower(); //compare everything in lowercase.
            int lDash = strLangDefined.IndexOf("-");
            if (lDash > 0)
            {
                langCode = strLangDefined.Substring(0, lDash);
            }
            else
            {
                langCode = strLangDefined;
            }

            string defLang = "";
            if ("en" == langCode)
            {
                defLang = "en-gb"; // default to GB english
            }
            else if ("zh" == langCode)
            {
                //jquery.ui.datepicker has zh-CN for simplified Chinese and zh-TW for Traditional Chinese
                defLang = "zh-cn"; //use special default for chinese 
            }
            else
            {
                defLang = langCode + "-" + langCode; // use general default e.g. fr-FR
            }

            int iMatch = -1;
            for (int i = 0; i < arrLang.Length; i++)
            {
                string currLang = arrLang[i].ToLower();
                if (strLangDefined == currLang) // use the exact match
                {
                    theLang = arrLang[i];
                    break;
                }
                else if (defLang == currLang) //use default one if available
                {
                    iMatch = i; 
                }
                else if (-1 == iMatch)
                {
                    lDash = currLang.IndexOf("-");
                    string currCode = "";
                    if (lDash > 0)
                    {
                        currCode = currLang.Substring(0, lDash);
                    }
                    else
                    {
                        currCode = currLang;
                    }
                    if (langCode == currCode)
                    {
                        iMatch = i; // first of its type on the list
                    }
                }
            }
            if ("" == theLang)
            {
                if (iMatch > -1)
                {
                    theLang = arrLang[iMatch]; // use first one on the list 
                }
            }
            return theLang;
        }
        #endregion

    }
}