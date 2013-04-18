using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Interfaces.Analytics.Provider;
using Ektron.Cms.Workarea.Framework;
using System.Web;
using Ektron.Cms.Interfaces.Context;
using Ektron.Cms.Framework.UI;

namespace Ektron.SEO
{
    public partial class seo : WorkareaDialogPage
    {
        private Ektron.Cms.ContentAPI _contentAPI;
		private string _quotesMissingMessage = "";
        private static List<string> _updateList = null;
        private bool _hasProviderChanged = false;
        private string SegmentPersistenceId = string.Empty;
        IAnalytics _dataManager = ObjectFactory.GetAnalytics();

        protected List<string> CookieSegments
        {
            get
            {
                List<string> segmentIds = new List<string>();
                HttpCookie cookie = Request.Cookies[SegmentPersistenceId];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    foreach (string s in cookie.Value.Split(','))
                    {
                        segmentIds.Add(s);
                    }
                }

                return segmentIds;
            }
            set
            {
                string idList = string.Join(",", value.ConvertAll<string>(delegate(string i) { return i; }).ToArray());
                HttpCookie cookie = new HttpCookie(SegmentPersistenceId, idList);
                cookie.Expires = System.DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                Response.Cookies.Add(cookie);
            }
        }

        protected void BadDateFormatErrorHandler(string defaultMessage, string rawDate) {
            ltr_analyticsError.Text = defaultMessage;
            errAnalyticsMsg.Visible = true;
        }

        public void BadDateRangeErrorHandler(object sender, BadDateRangeEventArgs e)
        {
            ltr_analyticsError.Text = e.Message;
            errAnalyticsMsg.Visible = true;
        }

        public void ProviderChangedHandler(object sender, Analytics_controls_ProviderSelector.ProviderChangedEventArgs e)
        {
            ProviderSelect.ProviderName = e.ProviderName;
            _hasProviderChanged = true;
            UpdateAvailableMetric(e.ProviderName);
        }

        private void UpdateAvailableMetric(string provider)
        {
            if (string.IsNullOrEmpty(provider)) return;

            string providerType = _dataManager.GetProviderType(provider);
            switch (providerType)
            {
                case "Ektron.Cms.Analytics.Providers.GoogleAnalyticsProvider":
                    AnalyticsReport.ShowVisits = false;
                    AnalyticsReport.ShowPagesPerVisit = false;
                    AnalyticsReport.ShowPageviews = true;
                    AnalyticsReport.ShowUniqueViews = true;
                    AnalyticsReport.ShowTimeOnSite = false;
                    AnalyticsReport.ShowTimeOnPage = true;
                    AnalyticsReport.ShowBounceRate = true;
                    AnalyticsReport.ShowPercentExit = true;
                    break;
                case "Ektron.Cms.Analytics.Providers.WebTrendsProvider":
                    AnalyticsReport.ShowVisits = true;
                    AnalyticsReport.ShowPagesPerVisit = false;
                    AnalyticsReport.ShowPageviews = true;
                    AnalyticsReport.ShowUniqueViews = false;
                    AnalyticsReport.ShowTimeOnSite = true;
                    AnalyticsReport.ShowTimeOnPage = false;
                    AnalyticsReport.ShowBounceRate = false;
                    AnalyticsReport.ShowPercentExit = false;
                    break;
                default:
                    ltr_error.Text = GetMessage("err hostname no stats");
                    break;
            }

            if (_hasProviderChanged)
            {
                AnalyticsReport.IsChanged = true;
                _hasProviderChanged = false; 
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            if ((_contentAPI.UserId > 0) && (_contentAPI.IsLoggedIn) && 0 == _contentAPI.RequestInformationRef.IsMembershipUser)
            {
                Page.Validate();
                AnalyticsReport.Visible = (Page.IsValid && !errAnalyticsMsg.Visible);
                if (Page.IsValid && !errAnalyticsMsg.Visible)
                {
                    UpdateAvailableMetric(ProviderSelect.SelectedText);
                    MakeLinks();
                }
            }
            else
            {
                setErrorMessage(GetMessage("msg login cms user"));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // register JS/CSS files
            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            Package resources = new Package() {
                Components = new List<Component>()
                    {
                        Packages.EktronCoreJS,
                        Packages.jQuery.jQueryUI.Tabs,
                        Packages.jQuery.Plugins.Cookie,
                        JavaScript.Create(cmsContextService.WorkareaPath + "/java/plugins/modal/ektron.modal.js"),
                        Ektron.Cms.Framework.UI.Css.Create(cmsContextService.WorkareaPath + "/java/plugins/modal/ektron.modal.css")
                    }
            };
            resources.Register(this);


            if (!Page.IsCallback && Request.QueryString["tab"] != null && Request.QueryString["tab"].ToString() == "traffic")
            {
                ltrShowTrafficTab.Visible = true;
            }

            _contentAPI = new Ektron.Cms.ContentAPI();

			_quotesMissingMessage = GetMessage("lbl quotes are missing");

			DateRangePicker1.BadStartDateFormatMessage = GetMessage("msg bad start date format");
            DateRangePicker1.BadEndDateFormatMessage = GetMessage("msg bad end date format");
            DateRangePicker1.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
            DateRangePicker1.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;
            DateRangePicker1.BadDateRange += BadDateRangeErrorHandler;
            DateRangePicker1.MaximumDate = DateTime.Today;
            DateRangePicker1.DefaultEndDate = DateTime.Today.AddDays(-1); // today is a partial day
            DateRangePicker1.DefaultStartDate = DateRangePicker1.DefaultEndDate.AddDays(-30);

            ProviderSelect.OnProviderChanged += ProviderChangedHandler;
            if (!string.IsNullOrEmpty(ProviderSelect.ProviderName))
            {
                SegmentPersistenceId = _dataManager.GetSegmentFilterCookieName(ProviderSelect.ProviderName);
            }
            AnalyticsReport.ProviderSegments = this.CookieSegments;

            //page
            lblClose.InnerText = GetMessage("close title");
            lblSEO.InnerText = GetMessage("generic seo");
            lblGoogle.InnerText = GetMessage("generic google");
            lblW3c.InnerText = GetMessage("generic w3c");
            lblAlexa.InnerText = GetMessage("generic alexa");
            lblImages.InnerText = GetMessage("generic images");
            lblText.InnerText = GetMessage("text");
            lblMeta.InnerText = GetMessage("generic meta");
			ChangeURLButton.Text = GetMessage("btn change");
            ChangeURLButton.ToolTip = ChangeURLButton.Text;
            lblTraffic.InnerText = GetMessage("generic traffic");
			
            if(AnalyticsSecurity.Enabled(_contentAPI.RequestInformationRef) == false)
            {
                liTrafficTab.Visible = false;
                tabTraffic.Visible = false;
                tabSEO.Visible = true;
            }
            else
            {
                liTrafficTab.Visible = true;
                tabTraffic.Visible = true;
            }
			
            string sDescription = GetMessage("generic description");
            string sPageLooksOnMobile = GetMessage("lbl what page looks like in mobile device");
            //tabSEO
            lblUrl.Text = GetMessage("lbl url");
            lblUrl.ToolTip = lblUrl.Text;
            lblDescSEO.InnerText = GetMessage("lbl seo seo tab desc");
            lblTitle.Text = GetMessage("generic title");
            lblDesc.Text = sDescription;
            lblKeywords.Text = GetMessage("sam keywords");
            LblLang.Text = GetMessage("generic language");
            lblCharSet.Text = GetMessage("lbl character set");
            lblH1Tag.Text = GetMessage("lbl first h1 tag");
            //tabGoogle
            lblDescGoogle.InnerText = GetMessage("lbl seo google tab desc");
            lblLinkThisPage.Text = GetMessage("lbl pages that links to this page");
            lblIndexedPages.Text = GetMessage("lbl indexed pages in your site");
            lblCachedVersion.Text = GetMessage("lbl current cached version of this page");
            lblAboutThisPage.Text = GetMessage("lbl information google has about this page");
            lblSimilarToPage.Text = GetMessage("lbl pages that are similar to this page");
            lblMobileImgLooks.Text = sPageLooksOnMobile;
            lblMobileNoImgLooks.Text = sPageLooksOnMobile;
            //tabW3C
            lblDescW3c.InnerText = GetMessage("lbl seo w3c tab desc");
            lblCheckMarkup.Text = GetMessage("lbl check markup");
            lblCheckLinks.Text = GetMessage("lbl checks broken links");
            lblCheckCss.Text = GetMessage("lbl checks cascading style sheets");
            lblCheckMobilePhones.Text = GetMessage("lbl checks mobile phones");
            //tabAlexa
            lblDescAlexa.InnerText = GetMessage("lbl seo alexa tab desc");
            lblOverviewAlexa.Text = GetMessage("lbl overview from alexa");
            lblTrafficDetails.Text = GetMessage("lbl traffic details");
            lblRelatedSites.Text = GetMessage("lbl related sites");
			lblAlexaKeywords.Text = GetMessage("lbl alexa search terms");
			lblAlexaClickstream.Text = GetMessage("lbl alexa upstream");
			lblLinkToSites.Text = GetMessage("lbl link to sites");
			//tabImages
            lblDescImg.InnerText = GetMessage("lbl seo img tab desc");
            lblStatus.Text = GetMessage("generic status");
            lblAltTag.Text = GetMessage("generic alt tag");
            lblImg.Text = GetMessage("generic image");
            //tabText
            lblDescText.InnerText = GetMessage("lbl seo text tab desc");
            //tabMeta
            lblDescMeta.InnerText = GetMessage("lbl seo meta tab desc");
            //tabTraffic
            if (!IsPostBack)
            {
                litLoadingMessage.Text = GetMessage("generic loading"); // TODO should be label w/o viewstate
            }
        }

        protected void MakeLinks()
        {
            string domainName = String.Empty;
            string completeURL = String.Empty;
            string pathAndQuery = String.Empty;
            string exceptionMsg = String.Empty;
            string urlSheme = "http";
            bool isAnalyticsError = false;

            if (!Page.IsPostBack)
            {
                string url = Request.QueryString["url"];

                if (url != null)
                {
					URLTextbox.Text = url;
                }
            }

			if (URLTextbox.Text.Length > 1)
            {
				DateTime dtStartDate = DateRangePicker1.StartDate;
				DateTime dtEndDate = DateRangePicker1.EndDate;

				Uri url = null;
                try
                {
                    url = new Uri(URLTextbox.Text);
                }
                catch (System.UriFormatException)
                {
                    try
                    {
                        url = new Uri("http://" + URLTextbox.Text.Replace("\\", "/"));
                    }
                    catch (System.UriFormatException)
                    {
                        exceptionMsg = GetMessage("err hostname could not be parsed");
                        isAnalyticsError = true;
                    }
                }
                catch
                {
                    url = new Uri("http://" + URLTextbox.Text);
                }
                if (url != null) 
                {
                    if ("localhost" == url.Host)
                    {
                        exceptionMsg = GetMessage("err localhost is not a valid url");
                        isAnalyticsError = true;
                    }
                    else
                    {
                        domainName = url.Host;
                        completeURL = url.Authority;
                        pathAndQuery = url.PathAndQuery;
                        if (url.PathAndQuery != "/")
                        {
                            completeURL += url.PathAndQuery;
                        }
                        if ("https" == url.Scheme)
                        {
                            urlSheme = "https"; // #49010: for SSL connection in parsepage()
                        }
                    }
                }
                if (isAnalyticsError)
                {
                    ltr_error.Text = exceptionMsg;
                    errMsg.Visible = true;
                }
                else
                {
                    List<AnalyticsReportData> reports = null;
                    string errDataManager = "";
                    //load providers to drop list except SiteCatalyst
                    //IAnalytics dataManager = ObjectFactory.GetAnalytics();
                    if (true == ltrShowTrafficTab.Visible)
                    {
                        try
                        {
                            List<string> siteProviders = _dataManager.GetSiteProviders(domainName);
                            string provider = "";
                            if (siteProviders.Count > 0)
                            {
                                //if (null == _updateList || !IsPostBack)
                                {
                                    _updateList = new List<string>();
                                    foreach (string p in siteProviders)
                                    {
                                        string providerType = _dataManager.GetProviderType(p);
                                        if ("Ektron.Cms.Analytics.Providers.SiteCatalystProvider" != providerType)
                                        {
                                            _updateList.Add(p);
                                        }
                                    }
                                    if (_updateList.Count > 0)
                                    {
                                        _hasProviderChanged = true;
                                        _updateList.Sort();
                                        ProviderSelect.ProviderList = _updateList;
                                        UpdateAvailableMetric(ProviderSelect.SelectedText);
                                    }
                                } 
                                provider = ProviderSelect.SelectedText.ToLower();
                                IDimensions _dimensions = _dataManager.GetDimensions(provider);
                                AnalyticsCriteria criteria = new AnalyticsCriteria();
                                Dimension d = _dimensions.pagePath;
                                if (d != null)
                                {
                                    criteria.DimensionFilters.Condition = LogicalOperation.Or;
                                    criteria.DimensionFilters.AddFilter(d, DimensionFilterOperator.EqualTo, pathAndQuery);
                                    AnalyticsReportData oneReport = null;
                                    reports = new List<AnalyticsReportData>();
                                    if (this.CookieSegments.Count > 0)
                                    {
                                        foreach (string segIdPair in this.CookieSegments)
                                        {
                                            string segVal = segIdPair.Substring(0, segIdPair.IndexOf("|"));
                                            string sSegProp = segIdPair.Replace(segVal + "|", "");
                                            SegmentProperty segProp = (SegmentProperty)Convert.ToInt32(sSegProp);
                                            criteria.SegmentFilter = new SegmentFilter(segProp, SegmentFilterOperator.EqualTo, segVal);
                                            oneReport = _dataManager.GetContentDetail(provider, dtStartDate, dtEndDate, criteria);
                                            if (oneReport != null)
                                            {
                                                reports.Add(oneReport);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        oneReport = _dataManager.GetContentDetail(provider, dtStartDate, dtEndDate, criteria);
                                        if (oneReport != null)
                                        {
                                            reports.Add(oneReport);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                ltr_error.Text = GetMessage("err hostname no stats"); ;
                                errMsg.Visible = true;
                                reports = new List<AnalyticsReportData>();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("(401)"))
                            {
                                errDataManager = GetMessage("err analytics data provider");
                            }
                            else
                            {
                                //errDataManager = ex.Message;
                                errDataManager = GetMessage("msg no data report");
                            }
                            ltr_error.Text = errDataManager;
                        }
                        if (reports == null || 0 == reports.Count)
                        {
                            errMsg.Visible = true;
                            AnalyticsReport.ShowSummaryChart = false;
                        }
                        else
                        {
                            AnalyticsReport.ProviderSegments = this.CookieSegments;
                            AnalyticsReport.StartDate = dtStartDate;
                            AnalyticsReport.EndDate = dtEndDate;
						    AnalyticsReport.UpdatePageUrl(URLTextbox.Text);
                            AnalyticsReport.UpdateReport(reports);
                            AnalyticsReport.ShowSummaryChart = true;
                        }
                    }
                    if (!isAnalyticsError)
                    {
                        string urlEncodedUrl = EkFunctions.UrlEncode(completeURL);
                        string htmlEncodedUrl = EkFunctions.HtmlEncode(completeURL);

                        MakeLink(hGoogleLink, GetMessage("lbl google link"), "http://www.google.com/search?hl=en&q=link:" + urlEncodedUrl + "&btnG=Google+Search&aq=f&oq=");
                        MakeLink(hGoogleSite, GetMessage("lbl google site") + " (" + domainName + ")", "http://www.google.com/search?hl=en&q=site:" + domainName + "&btnG=Google+Search&aq=f&oq=");
                        MakeLink(hGoogleCache, GetMessage("lbl goggle cache"), "http://www.google.com/search?hl=en&q=cache:" + urlEncodedUrl + "&btnG=Google+Search&aq=f&oq=");
                        MakeLink(hGoogleInfo, GetMessage("lbl google info"), "http://www.google.com/search?hl=en&q=info:" + urlEncodedUrl + "&btnG=Google+Search&aq=f&oq=");
                        MakeLink(hGoogleRelated, GetMessage("lbl google related"), "http://www.google.com/search?hl=en&q=related:" + urlEncodedUrl + "&btnG=Google+Search&aq=f&oq=");
                        MakeLink(hGoogleRobots, GetMessage("lbl google robots txt"), "http://" + domainName + "/robots.txt");
                        MakeLink(hGoogleMobileImages, GetMessage("lbl google mobile images"), "mobilephone.aspx?url=" + urlEncodedUrl);
                        MakeLink(hGoogleMobileNoImages, GetMessage("lbl google mobile no images"), "mobilephone.aspx?noimgages=1&url=" + urlEncodedUrl);

                        MakeLink(hW3CValidation, GetMessage("lbl w3c validation"), "http://validator.w3.org/check?uri=http://" + urlEncodedUrl);
                        MakeLink(hW3CLinkCheck, GetMessage("lbl w3c link check"), "http://validator.w3.org/checklink?uri=http://" + urlEncodedUrl);
                        MakeLink(hW3CCSS, GetMessage("lbl w3c css"), "http://jigsaw.w3.org/css-validator/validator?uri=http://" + urlEncodedUrl);
                        MakeLink(hW3CMobile, GetMessage("lbl w3c mobile"), "http://validator.w3.org/mobile/?docAddr=http://" + urlEncodedUrl);

                        MakeLink(hAlexaSiteInfo, GetMessage("lbl alexa overview") + " (" + domainName + ")", "http://www.alexa.com/siteinfo/" + domainName);
                        MakeLink(hAlexaTrafficStats, GetMessage("lbl alexa ranking traffic"), "http://www.alexa.com/siteinfo/" + domainName + "#trafficstats");
                        MakeLink(hAlexaRelatedLinks, GetMessage("lbl alexa related"), "http://www.alexa.com/siteinfo/" + domainName + "#relatedlinks");
                        MakeLink(hAlexaKeywords, GetMessage("lbl alexa keywords"), "http://www.alexa.com/siteinfo/" + domainName + "#keywords");
                        MakeLink(hAlexaClickstream, GetMessage("lbl alexa clickstream"), "http://www.alexa.com/siteinfo/" + domainName + "#clickstream");
                        MakeLink(hAlexaLinksIn, GetMessage("lbl alexa linked to"), "http://www.alexa.com/site/linksin/" + domainName);
                        
                        parsepage(urlSheme + "://" + completeURL); 
                    } 
                }
            }
            else
            {
                setErrorMessage(GetMessage("js err roll url"));
            }
        }

		private void MakeLink(HyperLink link, string text, string location)
		{
			link.Text = text;
			link.Attributes.Add("data-ektron-location", location);
			link.NavigateUrl = "#";
			link.ToolTip = location;
			link.CssClass = "viewIframeTrigger";
		}


        private void  ChangeURLButton_Click(object sender, EventArgs e)
        {
            // MakeLinks()

        }

        protected void parsepage(string url)
        {
            string TempString = "";
            try
            {
                WebRequest req = WebRequest.Create(url);
                HttpCookie cookie = (this.Page.Request.Cookies != null && this.Page.Request.Cookies.Count > 0 ? this.Page.Request.Cookies["ecm"] : null);
                if (cookie != null) {
                    System.Net.Cookie netCookie = new Cookie(cookie.Name, cookie.Value.Replace(",", "-"), cookie.Path, this.Request.Url.Host);
                    System.Net.CookieCollection cookieCollection = new CookieCollection();
                    cookieCollection.Add(netCookie);
                    CookieContainer container = new CookieContainer();
                    container.Add(cookieCollection);
                    (req as HttpWebRequest).CookieContainer = container;
                }
                WebResponse resp = req.GetResponse();

                int iStart;
                int iEnd;
                string BodyContent;
                string SrcString;
                string AltString;
                string[] wordsStringArray;
                string imageAltString;
                string keyWordString;
                string DescriptionString;
                int ipointer;
                int wordcount;
                int fontsizeInt;
                string wordString;
                string LastwordString;
                int npointer;
				Match result = null;
				MatchCollection results = null;
				bool noisewordmatch;
                string[] noiseWords = new string[] { "a", "and", "are", "as", "at", "be", "for", "has", "have", "in", 
			    "it", "of", "on", "is", "that", "the", "to", "you", "we" };

                Stream s = resp.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.UTF8);
                string strDoc = sr.ReadToEnd();
                sr.Close();

				strDoc = Regex.Replace(strDoc, "[\r\n\t]", "");

				string strHead = "";
				result = Regex.Match(strDoc, @"<head[^>]*>([\w\W]+?)</head>", RegexOptions.IgnoreCase);
				if (result.Success)
				{
					strHead = result.Groups[1].Value.Trim();
				}

				string strBody = "";
				result = Regex.Match(strDoc, @"<body[^>]*>([\w\W]+?)</body>", RegexOptions.IgnoreCase);
				if (result.Success)
				{
					strBody = result.Groups[1].Value.Trim();
				}

				keyWordString = "";
                DescriptionString = "";

				// Find Title
				string strTitle = "";
				if (strHead.Length > 0)
				{
					result = Regex.Match(strHead, @"<title[^>]*>([\w\W]+?)</title>", RegexOptions.IgnoreCase);
					if (result.Success)
					{
						strTitle = result.Groups[1].Value.Trim();
					}
				}
				if (strTitle.Length > 0)
				{
					titleLiteral.Text = strTitle;
					titleOkImage.ImageUrl = "valid.png";
					titleOkImage.AlternateText = GetMessage("lbl valid title msg");
				}
				else
				{
					titleLiteral.Text = GetMessage("lbl no title");
					titleOkImage.ImageUrl = "notvalid.png";
					titleOkImage.AlternateText = GetMessage("lbl invalid title msg");
				}

                // Find Language
				string strLang = "";
				result = Regex.Match(strDoc, @"<html[^>]*? lang\s*=\s*[""']?([^""'\s>]+)[^>]*?>", RegexOptions.IgnoreCase);
				if (result.Success)
				{
					strLang = result.Groups[1].Value.Trim();
				}
				if (strLang.Length > 0)
				{
					languageLiteral.Text = strLang;
                    languageOkImage.ImageUrl = "valid.png";
                    languageOkImage.AlternateText = GetMessage("lbl valid lang msg");
				}
				else
				{
                    languageLiteral.Text = GetMessage("lbl html tag missing lang");
                    languageOkImage.ImageUrl = "notvalid.png";
                    languageOkImage.AlternateText = GetMessage("lbl invalid lang msg");
				}

                // Check for H1
				string strH1 = "";
				if (strBody.Length > 0)
				{
					result = Regex.Match(strBody, @"<h1[^>]*>([\w\W]+?)</h1>", RegexOptions.IgnoreCase);
					if (result.Success)
					{
						strH1 = result.Groups[1].Value.Trim();
					}
				}
				if (strH1.Length > 0)
				{
					strH1 = Regex.Replace(strH1, @"<[^>]*>", ""); // RemoveTags
					H1Literal.Text = strH1;
                    FirstH1OkImage.ImageUrl = "valid.png";
                    FirstH1OkImage.AlternateText = GetMessage("lbl valid h1 tag msg");
				}
				else
				{
                    H1Literal.Text = GetMessage("lbl no h1 tag");
                    FirstH1OkImage.ImageUrl = "notvalid.png";
                    FirstH1OkImage.AlternateText = GetMessage("lbl invalid h1 tag msg");
				}
				

                // Find all meta data
                MetaDataLiteral.Text = "";
                charSetLiteral.Text = GetMessage("lbl no charset");
                charSetOkImage.ImageUrl = "notvalid.png";
                charSetOkImage.AlternateText = GetMessage("lbl no chartset alt msg");

                descriptionLiteral.Text = GetMessage("lbl no description");
                descriptionOkImage.ImageUrl = "notvalid.png";
                descriptionOkImage.AlternateText = GetMessage("lbl no description alt msg");

                keywordLiteral.Text = GetMessage("lbl no keyword");
                keywordsOkImage.ImageUrl = "notvalid.png";
                keywordsOkImage.AlternateText = GetMessage("lbl no keyword alt msg");


                if (strHead.Length > 0)
                {
					string strContentAttributeRegex = @" content\s*=\s*([""'])(.+?)\1";

					// Content-Type
					result = Regex.Match(strHead, @"<meta[^>]*? http-equiv\s*=\s*[""']?content-type[""'\s\/>][^>]*>", RegexOptions.IgnoreCase);
					if (result.Success)
					{
						TempString = result.Value;
						result = Regex.Match(TempString, strContentAttributeRegex, RegexOptions.IgnoreCase);
						if (result.Success)
						{
							TempString = result.Groups[2].Value.Trim();
							charSetLiteral.Text = TempString;
							charSetOkImage.ImageUrl = "valid.png";
							charSetOkImage.AlternateText = GetMessage("lbl charset alt msg");
						}
					}

					// Description
					result = Regex.Match(strHead, @"<meta[^>]*? name\s*=\s*[""']?description[""'\s\/>][^>]*>", RegexOptions.IgnoreCase);
					if (result.Success)
					{
						TempString = result.Value;
						result = Regex.Match(TempString, strContentAttributeRegex, RegexOptions.IgnoreCase);
						if (result.Success)
						{
							TempString = result.Groups[2].Value.Trim();
							descriptionLiteral.Text = TempString;
							DescriptionString = descriptionLiteral.Text + " ";
							descriptionOkImage.ImageUrl = "valid.png";
							descriptionOkImage.AlternateText = GetMessage("lbl description alt msg");
						}
					}

					// Keywords
					result = Regex.Match(strHead, @"<meta[^>]*? name\s*=\s*[""']?keywords[""'\s\/>][^>]*>", RegexOptions.IgnoreCase);
					if (result.Success)
					{
						TempString = result.Value;
						result = Regex.Match(TempString, strContentAttributeRegex, RegexOptions.IgnoreCase);
						if (result.Success)
						{
							TempString = result.Groups[2].Value.Trim();
							keywordLiteral.Text = TempString;
							keyWordString = keywordLiteral.Text + " ";
							keywordsOkImage.ImageUrl = "valid.png";
							keywordsOkImage.AlternateText = GetMessage("lbl keyword alt msg");
						}
					}

					results = Regex.Matches(strHead, @"<meta[^>]*?>", RegexOptions.IgnoreCase);
					if (results != null && results.Count > 0)
					{
                        // Is meta data closed correctly
						StringBuilder sbMeta = new StringBuilder();
						foreach (Match matchMeta in results)
						{
							TempString = matchMeta.Value;
                            sbMeta.Append(EkFunctions.HtmlEncode(TempString));
							if (!TempString.EndsWith("/>"))
							{
								sbMeta.Append(" <img src=\"notvalid.png\" /> ");
                                sbMeta.Append(EkFunctions.HtmlEncode(GetMessage("lbl should end with self closing tag")));
							}
							sbMeta.AppendLine("<br /><br />");
						}
						MetaDataLiteral.Text = sbMeta.ToString();
					}
                }

                // Text
                if (strBody.Length > 0)
                {
                    BodyContent = strBody.ToLower();
                    // Images
					TempString = BodyContent;
                    ImageLiteral.Text = "";
                    imageAltString = "";
                    while (TempString.Length > 0 && TempString.Contains("<img ")) // TODO case-insensitive
                    {

                        iStart = TempString.IndexOf("<img ");
                        //Start of image
                        iEnd = TempString.IndexOf(">", iStart);
                        //End of image tag
                        TempString = TempString.Substring(iStart, iEnd - iStart);
                        //Temp has entire image tag
                        SrcString = GetContent(TempString, "src=");

                        if (TempString.Contains("alt="))
                        {
                            AltString = GetContent(TempString, "alt=");
							// may be _quotesMissingMessage
                        }
                        else
                        {
                            AltString = "";
                        }

						if (_quotesMissingMessage == AltString)
                        {
							ImageLiteral.Text += "<tr><td><img src=\"notvalid.png\" alt=\"Alt attribute missing quotes\" style=\"border-width:0px;\" /></td><td>Alt attribute missing quotes</td><td>" + SrcString + "</td></tr>";
                        }
						else if (AltString.Length > 0)
						{
							imageAltString += AltString + " ";
							ImageLiteral.Text += "<tr><td><img src=\"valid.png\" alt=\"Alt attribute exists\" style=\"border-width:0px;\" /></td><td>" + AltString + "</td><td>" + SrcString + "</td></tr>";
						}
                        else
                        {
							ImageLiteral.Text += "<tr><td><img src=\"notvalid.png\" alt=\"No alt attribute\" style=\"border-width:0px;\" /></td><td>No alt text</td><td>" + SrcString + "</td></tr>";
                        }

                        TempString = BodyContent.Substring(iEnd, BodyContent.Length - iEnd);
                        BodyContent = TempString;

                    }


					TempString = strBody + imageAltString + keyWordString + " " + strTitle + DescriptionString;
					TempString = TempString.ToLower();
                    TempString = Regex.Replace(TempString, @"<!--[\s\S]*?-->", ""); // remove html comments
                    TempString = Regex.Replace(TempString, @"<script[\s\S]*?</script>", ""); // remove script elements
					TempString = Regex.Replace(TempString, @"<style[\s\S]*?</style>", " "); // remove style elements
                    TempString = Regex.Replace(TempString, @"<[^>]*>", " "); // remove tags
                    TempString = Regex.Replace(TempString, @"&\w+;", " "); // remove entity names
                    TempString = Regex.Replace(TempString, @"[^\w\s\-]+", " "); // remove punctuation
                    TempString = Regex.Replace(TempString, @"[\d_]+", " "); // remove digits and _
                    TempString = Regex.Replace(TempString, @"\s\-+\s", " "); // remove isolated '-' but keep hyphenated words
                    TempString = Regex.Replace(TempString, @"\s+", " "); // reduce multiple spaces to a single space
                    wordsStringArray = TempString.Split();
                    string[] sortWordsStringArray = new string[wordsStringArray.Length];
                    for (ipointer = 0; ipointer <= wordsStringArray.Length - 1; ipointer++)
                    {
                        sortWordsStringArray[ipointer] = wordsStringArray[ipointer];
                    }

                    Array.Sort(sortWordsStringArray, new System.Collections.CaseInsensitiveComparer());




                    wordcount = 1;
                    LastwordString = sortWordsStringArray[0];
                    BodyTextLiteral.Text = "";
                    noisewordmatch = false;

                    for (ipointer = 1; ipointer <= sortWordsStringArray.Length - 1; ipointer++)
                    {
                        wordString = sortWordsStringArray[ipointer];
                        if (wordString == LastwordString)
                        {
                            wordcount += 1;
                        }
                        else
                        {
                            if (wordcount > 7)
                            {
                                fontsizeInt = 7;
                            }
                            else
                            {
                                fontsizeInt = wordcount;
                            }
                            for (npointer = 0; npointer <= noiseWords.Length - 1; npointer++)
                            {
                                if (LastwordString == noiseWords[npointer])
                                {
                                    noisewordmatch = true;
                                }
                            }
                            if (noisewordmatch | LastwordString.Length == 1)
                            {
                                noisewordmatch = false;
                            }
                            else
                            {
                                BodyTextLiteral.Text += "<font size=\"" + fontsizeInt + "\">" + LastwordString + " </font> ";
                            }
                            LastwordString = wordString;
                            wordcount = 1;
                        }
                    }

                    for (ipointer = 1; ipointer <= wordsStringArray.Length - 1; ipointer++)
                    {
                        sortWordsStringArray[ipointer - 1] = wordsStringArray[ipointer - 1] + "_" + wordsStringArray[ipointer];
                    }
                    Array.Sort(sortWordsStringArray, new System.Collections.CaseInsensitiveComparer());

                    wordcount = 1;
                    LastwordString = sortWordsStringArray[0];
                    noisewordmatch = false;

                    BodyTextLiteral.Text += "<br /> <hr /> <h2><font color=\"red\">" + GetMessage("lbl two word phrases") + "</font></h2> <br />";

                    for (ipointer = 1; ipointer <= sortWordsStringArray.Length - 1; ipointer++)
                    {
                        wordString = sortWordsStringArray[ipointer];
                        if (wordString == LastwordString)
                        {
                            wordcount += 1;
                        }
                        else
                        {
                            if (wordcount > 7)
                            {
                                fontsizeInt = 7;
                            }
                            else
                            {
                                fontsizeInt = wordcount;
                            }
                            for (npointer = 0; npointer <= noiseWords.Length - 1; npointer++)
                            {
                                if (LastwordString == noiseWords[npointer])
                                {
                                    noisewordmatch = true;
                                }
                            }
                            if (noisewordmatch | LastwordString.Length == 1)
                            {
                                noisewordmatch = false;
                            }
                            else
                            {
                                if (fontsizeInt > 1)
                                {
                                    BodyTextLiteral.Text += "<font size=\"" + fontsizeInt + "\">" + LastwordString + " </font> ";
                                }
                            }
                            LastwordString = wordString;
                            wordcount = 1;
                        }
                    }


                    for (ipointer = 2; ipointer <= wordsStringArray.Length - 1; ipointer++)
                    {
                        sortWordsStringArray[ipointer - 2] = wordsStringArray[ipointer - 2] + "_" + wordsStringArray[ipointer - 1] + "_" + wordsStringArray[ipointer];
                    }
                    Array.Sort(sortWordsStringArray, new System.Collections.CaseInsensitiveComparer());

                    wordcount = 1;
                    LastwordString = sortWordsStringArray[0];
                    noisewordmatch = false;

                    BodyTextLiteral.Text += "<br /> <hr /> <h2><font color=\"red\">" + GetMessage("lbl three word phrases") + "</font></h2> <br />";

                    for (ipointer = 1; ipointer <= sortWordsStringArray.Length - 1; ipointer++)
                    {
                        wordString = sortWordsStringArray[ipointer];
                        if (wordString == LastwordString)
                        {
                            wordcount += 1;
                        }
                        else
                        {
                            if (wordcount > 7)
                            {
                                fontsizeInt = 7;
                            }
                            else
                            {
                                fontsizeInt = wordcount;
                            }
                            for (npointer = 0; npointer <= noiseWords.Length - 1; npointer++)
                            {
                                if (LastwordString == noiseWords[npointer])
                                {
                                    noisewordmatch = true;
                                }
                            }
                            if (noisewordmatch | LastwordString.Length == 1)
                            {
                                noisewordmatch = false;
                            }
                            else
                            {
                                if (fontsizeInt > 1)
                                {
                                    BodyTextLiteral.Text += "<font size=\"" + fontsizeInt + "\">" + LastwordString + " </font> ";
                                }
                            }
                            LastwordString = wordString;
                            wordcount = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                setErrorMessage(ex.Message);
            }
        }

        string GetContent(string DataString, string Attribute)
        {
            // content=" or single quotes
            int StartInterger;
            string TempString;
            string compareDoubleQuoteString;
            string compareSingleQuoteString;
            string compareEnd;
            string FoundContentString;
            TempString = DataString;
            compareDoubleQuoteString = Attribute + "\"";
            compareSingleQuoteString = Attribute + "'";
            compareEnd = "\"";
			FoundContentString = _quotesMissingMessage;

            if (TempString.IndexOf(compareDoubleQuoteString) > 0)
            {
                StartInterger = TempString.IndexOf(compareDoubleQuoteString) + compareDoubleQuoteString.Length;
                compareEnd = "\"";
            }
            else if (TempString.IndexOf(compareSingleQuoteString) > 0)
            {
                StartInterger = TempString.IndexOf(compareSingleQuoteString) + compareSingleQuoteString.Length;
                compareEnd = "'";
            }
            // No quotes
            else
            {
                return FoundContentString;
            }
            if (TempString.IndexOf(compareEnd, StartInterger) > 0)
            {
                FoundContentString = TempString.Substring(StartInterger, TempString.IndexOf(compareEnd, StartInterger) - StartInterger);
            }
            return FoundContentString;
        }


        void setErrorMessage(string message)
        {
			ltr_error.Text = message;
			errMsg.Visible = true;
			titleLiteral.Text = "";
            descriptionLiteral.Text = " ";
            keywordLiteral.Text = " ";
            languageLiteral.Text = " ";
            charSetLiteral.Text = " ";
            H1Literal.Text = "";
            descriptionOkImage.ImageUrl = "notvalid.png";
            keywordsOkImage.ImageUrl = "notvalid.png";
            languageOkImage.ImageUrl = "notvalid.png";
            charSetOkImage.ImageUrl = "notvalid.png";
            titleOkImage.ImageUrl = "notvalid.png";
            FirstH1OkImage.ImageUrl = "notvalid.png";
            ImageLiteral.Text = "";
			hGoogleLink.Visible = false;
			hGoogleCache.Visible = false;
			hGoogleInfo.Visible = false;
			hGoogleRelated.Visible = false;
			hGoogleSite.Visible = false;
			hGoogleRobots.Visible = false;
			hW3CCSS.Visible = false;
			hW3CLinkCheck.Visible = false;
			hW3CMobile.Visible = false;
			hW3CValidation.Visible = false;
			hAlexaClickstream.Visible = false;
			hAlexaKeywords.Visible = false;
			hAlexaLinksIn.Visible = false;
			hAlexaRelatedLinks.Visible = false;
			hAlexaSiteInfo.Visible = false;
			hAlexaTrafficStats.Visible = false;
            BodyTextLiteral.Text = "";
            MetaDataLiteral.Text = "";
        }

    }
}