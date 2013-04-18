//-----------------------------------------------------------------------
// <copyright file="LocaleDetail.ascx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Localization;
using LocalizationLanguageState = Ektron.Cms.Localization.LanguageState;

/// <summary>
/// Edit properties of a locale
/// </summary>
public partial class Workarea_LocaleDetail : WorkareaBaseControl
{
    /*
     * BCP 47
     * http://tools.ietf.org/html/bcp47
     * 
     * RFC 5646 Tags for Identifying Languages
     * http://www.rfc-editor.org/rfc/rfc5646.txt
     */

    /// <summary>
    /// The locale api
    /// </summary>
    Ektron.Cms.Framework.Localization.LocaleManager locApi = null;

    /// <summary>
    /// User language iso code
    /// </summary>
    private string userLang = "en";

    /// <summary>
    /// Selected LanguageTag
    /// </summary>
    private LanguageTag selectedLanguageTag = null;

    /// <summary>
    /// Selected language code
    /// </summary>
    private string selectedLanguage = string.Empty;

    /// <summary>
    /// Likely LanguageTag based on CLDR
    /// </summary>
    private LanguageTag likelyLanguageTag = null;

    /// <summary>
    /// Whether region is assumed 
    /// </summary>
    private bool isAssumedRegion = false;

    /// <summary>
    /// Localized string
    /// </summary>
    private string msgSelectPrompt = "(Select)";

    /// <summary>
    /// Navigator for likely subtag
    /// </summary>
    private System.Xml.XPath.XPathNavigator likelySubtagsNav = null;

    /// <summary>
    /// Navigator for regions
    /// </summary>
    private System.Xml.XPath.XPathNavigator regionsNav = null;

    /// <summary>
    /// Gets or sets the ID of the default CMS locale
    /// </summary>
    public int DefaultLocaleId { get; set; }

    /// <summary>
    /// Gets or sets the LocaleData
    /// </summary>
    public LocaleData Locale { get; set; }

    /// <summary>
    /// Gets or sets the list of LocaleData objects
    /// </summary>
    public List<LocaleData> Locales { get; set; }

    /// <summary>
    /// Gets or sets the list of locales supported by this version of the Windows operating system
    /// </summary>
    public List<LocaleData> SystemLocales { get; set; }

    /// <summary>
    /// Override OnInit to assign control properties
    /// </summary>
    /// <param name="e">The event arguments</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        this.locApi = new Ektron.Cms.Framework.Localization.LocaleManager();

        revLoc.ValidationExpression = @"[A-Za-z0-9_\-]{1,20}";
        valLoc.ValidateEmptyText = false;
        valLoc.ServerValidate += new ServerValidateEventHandler(this.ServerValidateHandler);
        revPrivateUseSubtag.ValidationExpression = @"[A-Za-z0-9]{1,8}"; // http://www.rfc-editor.org/rfc/rfc5646.txt
        revNativeName.ValidationExpression = @"[^\<\>\x22\r\n]{1,50}";
        revEnglishName.ValidationExpression = revNativeName.ValidationExpression;
    }

    /// <summary>
    /// Page load to handle post back and more
    /// </summary>
    /// <param name="sender">the page object</param>
    /// <param name="e">The event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            string language = this.Request.Form["Language"];
            if (language != null)
            {
                string scriptSubtag = this.Request.Form["ScriptSubtag"];
                string regionSubtag = this.Request.Form["Territory"];
                string likelyLanguageTag = this.Request.Form["LikelyLanguageTag"];
                if (!String.IsNullOrEmpty(likelyLanguageTag))
                {
                    this.likelyLanguageTag = new LanguageTag(likelyLanguageTag);
                }

                string loc = txtLoc.Text;

                int id = -1;
                Int32.TryParse(hdnLocaleId.Value, out id);

                int fallbackId = 0;
                Int32.TryParse(lstFallbackLoc.SelectedValue, out fallbackId);

                string privateUseSubtag = txtPrivateUseSubtag.Text.Trim();

                LanguageTag objLangTag = new LanguageTag(language, scriptSubtag, regionSubtag, privateUseSubtag);
                objLangTag.RegistryUri = new Uri(Request.Url, "language-subtag-registry.xml");
                string languageTag = objLangTag.ToString();

                string culture = lstCulture.SelectedValue;
                string uiCulture = lstUICulture.SelectedValue;

                int lcid = 1033;
                System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo(uiCulture);
                if (info != null)
                {
                    lcid = info.LCID;
                }

                string flagFile = rblFlag.SelectedValue;
                string flagUrl = CommonApi.AppImgPath + "flags/" + flagFile;

                string nativeName = txtNativeName.Text;
                string englishName = txtEnglishName.Text;

                bool isRightToLeft = chkIsRightToLeft.Checked;

                LocalizationLanguageState localeState = LocalizationLanguageState.Active;
                if (Enum.IsDefined(typeof(LocalizationLanguageState), hdnState.Value))
                {
                    localeState = (LocalizationLanguageState)Enum.Parse(typeof(LocalizationLanguageState), hdnState.Value);
                    if (LocalizationLanguageState.Defined == localeState)
                    {
                        localeState = LocalizationLanguageState.Active;
                    }
                }

                this.Locale = new LocaleData(id, lcid, englishName, nativeName, isRightToLeft, loc, culture, uiCulture, language, languageTag, flagFile, flagUrl, fallbackId, localeState);
            }
        }
        else
        {
            LanguageDesc.Text = String.Format(LanguageDesc.Text, this.GetDateOfCLDR());
            System.Text.StringBuilder bcp47HyperlinkMarkup = new System.Text.StringBuilder();
            using (HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter(bcp47HyperlinkMarkup)))
            {
                BCP47.RenderControl(writer);
            }

            BCP47.Visible = false;
            System.Text.StringBuilder rfc5646HyperlinkMarkup = new System.Text.StringBuilder();
            using (HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter(rfc5646HyperlinkMarkup)))
            {
                RFC5646.RenderControl(writer);
            }

            RFC5646.Visible = false;
            XmlLangDescP1.Text = String.Format(XmlLangDescP1.Text, bcp47HyperlinkMarkup.ToString(), rfc5646HyperlinkMarkup.ToString());

            Ektron.Cms.BusinessObjects.Localization.L10nManager l10nMgr = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.CommonApi.RequestInformationRef);
            MachineTranslationUndefinedPanel.Visible = !l10nMgr.IsMachineTranslationSupported;
            PseudoLocalizationUndefinedPanel.Visible = !l10nMgr.IsPseudoLocalizationSupported;
        }
    }

    /// <summary>
    /// Override OnPreRender to build the page sections
    /// </summary>
    /// <param name="e">The event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        this.userLang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        this.msgSelectPrompt = GetMessage("lbl first display text on choices option");

        this.isAssumedRegion = false;

        if (this.Locale != null)
        {
            hdnState.Value = this.Locale.LanguageState.ToString();
            hdnLocaleId.Value = this.Locale.Id.ToString();
            chkIsRightToLeft.Checked = this.Locale.IsRightToLeft;
            this.selectedLanguage = this.Locale.LangCode;
            this.selectedLanguageTag = new LanguageTag(this.Locale.XmlLang);

            if (!String.IsNullOrEmpty(this.selectedLanguage) && !this.selectedLanguage.Contains("-"))
            {
                string strLikelyTag = this.GetLikelyTag(this.selectedLanguage);
                LanguageTag likelyTag = new LanguageTag(strLikelyTag);
                this.isAssumedRegion = likelyTag.RegionSubtag == this.selectedLanguageTag.RegionSubtag;
            }

            if (this.likelyLanguageTag != null)
            {
                if (this.likelyLanguageTag.ScriptSubtag == this.selectedLanguageTag.ScriptSubtag &&
                    this.likelyLanguageTag.RegionSubtag == this.selectedLanguageTag.RegionSubtag)
                {
                    this.selectedLanguageTag.ScriptSubtag = string.Empty;
                }
            }
        }
        else
        {
            hdnState.Value = LocalizationLanguageState.Active.ToString();
            hdnLocaleId.Value = "-1";
            chkIsRightToLeft.Checked = false;
            this.selectedLanguage = string.Empty;
            this.selectedLanguageTag = new LanguageTag();
        }

        this.selectedLanguageTag.RegistryUri = new Uri(Request.Url, "language-subtag-registry.xml");

        string cldrMainXmlRelPath = String.Format("CLDR/common/main/{0}.xml", this.userLang);
        if (!System.IO.File.Exists(MapPath(cldrMainXmlRelPath)))
        {
            cldrMainXmlRelPath = "CLDR/common/main/en.xml";
        }

        string cldrMainXmlPath = new Uri(Request.Url, cldrMainXmlRelPath).AbsoluteUri;

        this.BuildLanguageSection(cldrMainXmlPath);
        this.BuildRegionSection(cldrMainXmlPath);
        this.BuildLocaleSection();
        this.BuildFallbackSection();
        this.BuildLanguageTagSection(cldrMainXmlPath);
        this.BuildCultureSections();
        this.BuildFlagSection();
        this.BuildDisplayNameSections();

        base.OnPreRender(e);
    }

    /// <summary>
    /// Gets the name of the locale from the CLDR
    /// </summary>
    /// <param name="cldrMainXmlPath">File path to the main file in the CLDR</param>
    /// <param name="language">Language code, may include region</param>
    /// <param name="regionSubtag">Region subtag</param>
    /// <param name="localeLanguageName">Locale language name</param>
    /// <param name="localeRegionName">Locale region name</param>
    /// <param name="isRightToLeft">Indicates whether language is bi-directional</param>
    /// <returns>The name of the locale</returns>
    private static string GetLocaleName(string cldrMainXmlPath, string language, string regionSubtag, ref string localeLanguageName, ref string localeRegionName, ref bool isRightToLeft)
    {
        string localeName = string.Empty;
        string localeFormat = "{0} ({1})";
        const string LocaleFormatXPath = "/ldml/localeDisplayNames/localeDisplayPattern/localePattern";
        string localeLanguageNameXPath = String.Format("/ldml/localeDisplayNames/languages/language[@type='{0}']", language.Replace('-', '_'));
        string localeTerritoryNameXPath = String.Format("/ldml/localeDisplayNames/territories/territory[@type='{0}']", regionSubtag);
        const string IsRightToLeftXPath = "/ldml/layout/orientation[@characters='right-to-left']";

        System.Xml.XPath.XPathNavigator localeFormatNode = null;
        System.Xml.XPath.XPathNavigator localeLanguageNode = null;
        System.Xml.XPath.XPathNavigator localeTerritoryNode = null;
        System.Xml.XPath.XPathNavigator orientationNode = null;

        try
        {
            System.Xml.XPath.XPathDocument docMain = new System.Xml.XPath.XPathDocument(cldrMainXmlPath);
            System.Xml.XPath.XPathNavigator navMain = docMain.CreateNavigator();
            if (navMain != null)
            {
                localeFormatNode = navMain.SelectSingleNode(LocaleFormatXPath);
                if (localeFormatNode != null && !String.IsNullOrEmpty(localeFormatNode.Value))
                {
                    localeFormat = localeFormatNode.Value;
                }

                localeLanguageNode = navMain.SelectSingleNode(localeLanguageNameXPath);
                if (localeLanguageNode != null)
                {
                    localeLanguageName = localeLanguageNode.Value;
                }

                localeTerritoryNode = navMain.SelectSingleNode(localeTerritoryNameXPath);
                if (localeTerritoryNode != null)
                {
                    localeRegionName = localeTerritoryNode.Value;
                }

                orientationNode = navMain.SelectSingleNode(IsRightToLeftXPath);
                isRightToLeft = orientationNode != null;
            }

            if (!String.IsNullOrEmpty(localeLanguageName))
            {
                if (!String.IsNullOrEmpty(localeRegionName))
                {
                    localeName = String.Format(localeFormat, localeLanguageName, localeRegionName);
                }
                else
                {
                    localeName = localeLanguageName;
                }
            }
        }
        catch
        {
        }

        return localeName;
    }

    /// <summary>
    /// Handles the ServerValidate event
    /// </summary>
    /// <param name="source">The object raising the event</param>
    /// <param name="args">ServerValidate event args</param>
    private void ServerValidateHandler(object source, ServerValidateEventArgs args)
    {
        string loc = args.Value;

        // Ensure uniqueness
        Ektron.Cms.Common.Criteria<LocaleProperty> criteria = new Ektron.Cms.Common.Criteria<LocaleProperty>();

        criteria.AddFilter(LocaleProperty.Loc, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, loc); // case-insensitive
        if (this.Locale != null)
        {
            criteria.AddFilter(LocaleProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, this.Locale.Id); // may be updating existing locale
        }

        List<LocaleData> locales = this.locApi.GetList(criteria);

        args.IsValid = null == locales || 0 == locales.Count; // valid if not other locale with this loc exists
    }

    /// <summary>
    /// Gets the value from the regions.xml file
    /// </summary>
    /// <param name="regionSubtag">Region subtag</param>
    /// <param name="regionAttribute">Region attribute name</param>
    /// <returns>The region value</returns>
    private string GetRegionValue(string regionSubtag, string regionAttribute)
    {
        string regionValue = string.Empty;
        if (null == this.regionsNav)
        {
            string path = new Uri(Request.Url, "regions.xml").AbsoluteUri;
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(path);
            this.regionsNav = doc.CreateNavigator();
        }

        if (this.regionsNav != null)
        {
            System.Xml.XPath.XPathNavigator node = this.regionsNav.SelectSingleNode(String.Format("regions/region[@type='{0}']/@{1}", regionSubtag, regionAttribute));
            if (node != null)
            {
                regionValue = node.Value;
            }
        }

        return regionValue;
    }

    /// <summary>
    /// Builds the language section
    /// </summary>
    /// <param name="cldrMainXmlPath">The file path to the main CLDR file</param>
    private void BuildLanguageSection(string cldrMainXmlPath)
    {
        string xsltPath = new Uri(Request.Url, "languageList.xsl").AbsoluteUri;
        Ektron.Cms.Xslt.ArgumentList args = new Ektron.Cms.Xslt.ArgumentList();
        args.AddParam("controlId", string.Empty, LanguageSelector.ClientID);
        args.AddParam("subset", string.Empty, LanguageSet.Text);
        args.AddParam("selectedLanguage", string.Empty, this.selectedLanguage);
        if (!String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag))
        {
            args.AddParam("selectedScript", string.Empty, this.selectedLanguageTag.ScriptSubtag);
        }

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            args.AddParam("selectedTerritory", string.Empty, this.selectedLanguageTag.RegionSubtag);
        }

        args.AddParam("lang", string.Empty, this.userLang);
        if (String.IsNullOrEmpty(this.selectedLanguage))
        {
            args.AddParam("prompt", string.Empty, this.msgSelectPrompt);
        }

        LanguageSelector.Text = ContentApi.XSLTransform(cldrMainXmlPath, xsltPath, true, true, args, false);
    }

    /// <summary>
    /// Builds the region section
    /// </summary>
    /// <param name="cldrMainXmlPath">The file path to the main CLDR file</param>
    private void BuildRegionSection(string cldrMainXmlPath)
    {
        const string RegionFilenameFormat = "{0}.gif";
        string xsltPath = new Uri(Request.Url, "territoryList.xsl").AbsoluteUri;
        Ektron.Cms.Xslt.ArgumentList args = new Ektron.Cms.Xslt.ArgumentList();
        args.AddParam("controlId", string.Empty, RegionSelector.ClientID);
        args.AddParam("subset", string.Empty, RegionSet.Text);
        args.AddParam("selectedLanguage", string.Empty, this.selectedLanguage);
        if (!String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag))
        {
            args.AddParam("selectedScript", string.Empty, this.selectedLanguageTag.ScriptSubtag);
        }

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            args.AddParam("selectedTerritory", string.Empty, this.selectedLanguageTag.RegionSubtag);
        }

        args.AddParam("lang", string.Empty, this.userLang);
        RegionSelector.Text = ContentApi.XSLTransform(cldrMainXmlPath, xsltPath, true, true, args, false);

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            string mapArea = this.GetRegionValue(this.selectedLanguageTag.RegionSubtag, "chtm");
            bool isMultinationalRegion = !String.IsNullOrEmpty(mapArea);
            CountryMap.Visible = isMultinationalRegion;
            LocatorMap.Visible = !isMultinationalRegion;
            if (isMultinationalRegion)
            {
                // Get list of country codes to highlight in CountryMap
                args.Clear();
                args.AddParam("countriesInTree", string.Empty, true);
                args.AddParam("selectedLanguage", string.Empty, this.selectedLanguage);
                if (!String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag))
                {
                    args.AddParam("selectedScript", string.Empty, this.selectedLanguageTag.ScriptSubtag);
                }

                if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
                {
                    args.AddParam("selectedTerritory", string.Empty, this.selectedLanguageTag.RegionSubtag);
                }

                args.AddParam("lang", string.Empty, this.userLang);
                xsltPath = new Uri(Request.Url, "territoryTree.xsl").AbsoluteUri;
                string regionTree = ContentApi.XSLTransform(cldrMainXmlPath, xsltPath, true, true, args, false);

                System.Xml.XPath.XPathDocument docRegionTree = new System.Xml.XPath.XPathDocument(new System.Xml.XmlTextReader(new System.IO.StringReader(regionTree)));
                System.Xml.XPath.XPathNavigator navRegionTree = docRegionTree.CreateNavigator();
                if (navRegionTree != null)
                {
                    string countryCodesXPath = String.Format("//li[@id='{0}']/descendant-or-self::li[not(ul)]/@id", this.selectedLanguageTag.RegionSubtag);
                    System.Xml.XPath.XPathNodeIterator countryNodes = navRegionTree.Select(countryCodesXPath);
                    if (countryNodes != null)
                    {
                        string[] countryCodes = new string[countryNodes.Count];
                        for (int i = 0; i < countryNodes.Count; i++)
                        {
                            countryNodes.MoveNext();
                            countryCodes[i] = countryNodes.Current.Value;
                        }

                        CountryMap.CountryCodes = countryCodes;
                    }
                }

                // Determine map area to display, e.g., whole world, europe, africa, in CountryMap
                CountryMap.MapArea = controls_reports_CountryMap.GeographicalArea.World;
                if (String.IsNullOrEmpty(mapArea))
                {
                    // Single country regions are handled elsewhere, but this will handle single country regions should the need arise.
                    System.Xml.XPath.XPathNavigator countryNode = navRegionTree.SelectSingleNode(String.Format("//li[@id='{0}']/../../@id", this.selectedLanguageTag.RegionSubtag));
                    if (countryNode != null)
                    {
                        string countryRegion = countryNode.Value;
                        mapArea = this.GetRegionValue(countryRegion, "chtm");
                    }
                }

                controls_reports_CountryMap.GeographicalArea[] areas = (controls_reports_CountryMap.GeographicalArea[])Enum.GetValues(typeof(controls_reports_CountryMap.GeographicalArea));
                foreach (controls_reports_CountryMap.GeographicalArea area in areas)
                {
                    if (mapArea == EnumExtensions.ToDescriptionString(area))
                    {
                        CountryMap.MapArea = area;
                        break; // exit, we found it
                    }
                }
            }
            else
            {
                // Single country
                LocatorMap.ImageUrl = CommonApi.AppImgPath + "locators/" + String.Format(RegionFilenameFormat, this.selectedLanguageTag.RegionSubtag);
            }
        }
    }

    /// <summary>
    /// Builds the locale section
    /// </summary>
    private void BuildLocaleSection()
    {
        if (this.Locale != null)
        {
            txtLoc.Text = this.Locale.Loc;
        }

        bool useRecommended = String.IsNullOrEmpty(txtLoc.Text) || txtLoc.Text == RecommendedLoc.Text;

        string recommendedLoc = this.selectedLanguage;
        string regionSubtag = this.selectedLanguageTag.RegionSubtag;
        if (!String.IsNullOrEmpty(regionSubtag))
        {
            // Note: decided to keep the numeric region codes, so this section is commented out.
            ////string suggestedRegion = GetRegionValue(regionSubtag, "loc");
            ////if (!String.IsNullOrEmpty(suggestedRegion))
            ////{
            ////    // substitute 3-digit region code w/ mnemonic
            ////    regionSubtag = suggestedRegion;
            ////}
            if (!String.IsNullOrEmpty(recommendedLoc))
            {
                // Note: decided to keep region subtag in 'loc'
                ////if (!this.isAssumedRegion && !recommendedLoc.EndsWith("-" + regionSubtag))
                if (!recommendedLoc.EndsWith("-" + regionSubtag))
                {
                    if (!recommendedLoc.EndsWith("-" + this.selectedLanguageTag.ScriptSubtag))
                    {
                        recommendedLoc = recommendedLoc.Replace("-", "_");
                    }

                    recommendedLoc += "-" + regionSubtag;
                }
            }
            else
            {
                recommendedLoc = "und-" + regionSubtag;
            }
        }

        if (!String.IsNullOrEmpty(recommendedLoc))
        {
            RecommendedLoc.Text = recommendedLoc;
            if (useRecommended)
            {
                txtLoc.Text = recommendedLoc;
            }
        }
        else
        {
            RecommendedLoc.Text = "?";
        }
    }

    /// <summary>
    /// Builds the locale fallback section
    /// </summary>
    private void BuildFallbackSection()
    {
        if (null == this.Locales)
        {
            return;
        }

        int fallbackId = this.DefaultLocaleId;
        lstFallbackLoc.DataTextField = "EnglishName"; // can't use CombinedName b/c it is mixed languages
        lstFallbackLoc.DataValueField = "Id";
        if (this.Locale != null)
        {
            if (this.Locale.FallbackId > 0)
            {
                fallbackId = this.Locale.FallbackId;
            }

            lstFallbackLoc.Items.Clear();
            lstFallbackLoc.Items.Add(new ListItem(GetMessage("none w prths"), this.Locale.Id.ToString())); // fallback==id means "no fallback"
            lstFallbackLoc.AppendDataBoundItems = true;
            lstFallbackLoc.DataSource = this.Locales.FindAll(d => d.Enabled && d.Id != this.Locale.Id);
        }
        else
        {
            lstFallbackLoc.DataSource = this.Locales.FindAll(d => d.Enabled);
        }

        lstFallbackLoc.DataBind();
        try
        {
            lstFallbackLoc.SelectedValue = fallbackId.ToString();
        }
        catch (Exception)
        {
            lstFallbackLoc.SelectedValue = this.DefaultLocaleId.ToString();
        }

        bool useRecommended = lstFallbackLoc.SelectedValue == this.DefaultLocaleId.ToString() ||
            (lstFallbackLoc.SelectedItem != null && lstFallbackLoc.SelectedItem.Text == RecommendedFallbackLoc.Text);

        LocaleData recommendedLocale = this.GetRecommendedFallbackLocale();
        if (recommendedLocale != null)
        {
            RecommendedFallbackLoc.Text = recommendedLocale.EnglishName;
            if (useRecommended)
            {
                lstFallbackLoc.SelectedValue = recommendedLocale.Id.ToString();
            }
        }
        else
        {
            RecommendedFallbackLoc.Text = "?";
        }
    }

    /// <summary>
    /// Gets the recommended fallback locale
    /// </summary>
    /// <returns>Returns a LocaleData object of the recommended fallback locale</returns>
    private LocaleData GetRecommendedFallbackLocale()
    {
        LocaleData recommendedLocale = null;
        if (this.Locale != null)
        {
            int id = this.Locale.Id;
            string lang = this.Locale.LangCode;

            List<LocaleData> recommendations = null;
            int p = lang.IndexOf('-');
            if (p >= 0)
            {
                // Search for exact match of LangCode, which includes script or region subtag.
                recommendations = this.Locales.FindAll(d => d.Enabled && d.LangCode == lang && d.Id != id);
            }

            if (null == recommendations || 0 == recommendations.Count)
            {
                // Search for match of language subtag
                lang = p >= 0 ? lang.Substring(0, p) : lang;
                recommendations = this.Locales.FindAll(d => d.Enabled && (d.LangCode == lang || d.LangCode.StartsWith(lang + "-")) && d.Id != id);
            }

            if (recommendations != null && recommendations.Count > 0)
            {
                // Take the one with the lowest ID, which should be a good fallback choice
                recommendations.Sort(delegate(LocaleData d1, LocaleData d2) { return d1.Id.CompareTo(d2.Id); });
                recommendedLocale = recommendations[0];
            }
        }

        return recommendedLocale;
    }

    /// <summary>
    /// Builds the language tag section
    /// </summary>
    /// <param name="cldrMainXmlPath">The file path to the main CLDR file</param>
    private void BuildLanguageTagSection(string cldrMainXmlPath)
    {
        txtXmlLang.Text = this.selectedLanguageTag.ToString();
        txtPrivateUseSubtag.Text = this.selectedLanguageTag.PrivateUseSubtag;

        string xsltPath = new Uri(Request.Url, "scriptList.xsl").AbsoluteUri;
        Ektron.Cms.Xslt.ArgumentList args = new Ektron.Cms.Xslt.ArgumentList();
        args.AddParam("controlId", string.Empty, ScriptSubtagSelector.ClientID);
        args.AddParam("subset", string.Empty, ScriptSubtagSet.Text);
        args.AddParam("selectedLanguage", string.Empty, this.selectedLanguage);
        if (!String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag))
        {
            args.AddParam("selectedScript", string.Empty, this.selectedLanguageTag.ScriptSubtag);
        }
        else
        {
            args.AddParam("prompt", string.Empty, this.msgSelectPrompt);
        }

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            args.AddParam("selectedTerritory", string.Empty, this.selectedLanguageTag.RegionSubtag);
        }

        args.AddParam("lang", string.Empty, this.userLang);
        ScriptSubtagSelector.Text = ContentApi.XSLTransform(cldrMainXmlPath, xsltPath, true, true, args, false);
    }

    /// <summary>
    /// Builds the Culture sections, i.e., Culture and UICulture.
    /// </summary>
    private void BuildCultureSections()
    {
        if (this.SystemLocales != null)
        {
            LocaleData selectedCultureData = null;
            LocaleData selectedUICultureData = null;
            if (this.Locale != null)
            {
                selectedCultureData = this.SystemLocales.Find(d => d.Culture == this.Locale.Culture);
                selectedUICultureData = this.SystemLocales.Find(d => d.UICulture == this.Locale.UICulture);
            }

            if (null == selectedCultureData)
            {
                selectedCultureData = this.locApi.FindLocale(this.SystemLocales, this.DefaultLocaleId);
            }

            if (null == selectedUICultureData)
            {
                selectedUICultureData = selectedCultureData;
            }

            bool useRecommendedCulture = 
                    null == selectedCultureData ||
                    selectedCultureData.Id == this.DefaultLocaleId ||
                    selectedCultureData.EnglishName == RecommendedCulture.Text;
            bool useRecommendedUICulture = 
                    null == selectedUICultureData ||
                    selectedUICultureData.Id == this.DefaultLocaleId ||
                    selectedUICultureData.EnglishName == RecommendedUICulture.Text;

            List<LocaleData> systemLocalesSorted = this.SystemLocales.OrderBy(d => d.EnglishName).ToList();

            lstCulture.DataTextField = "EnglishName";
            lstCulture.DataValueField = "Culture";
            lstCulture.DataSource = systemLocalesSorted;
            lstCulture.DataBind();
            if (selectedCultureData != null)
            {
                lstCulture.SelectedValue = selectedCultureData.Culture;
            }

            lstUICulture.DataTextField = "EnglishName";
            lstUICulture.DataValueField = "UICulture";
            lstUICulture.DataSource = systemLocalesSorted;
            lstUICulture.DataBind();
            if (selectedUICultureData != null)
            {
                lstUICulture.SelectedValue = selectedUICultureData.UICulture;
            }

            LocaleData cultureData = this.GetRecommendedCulture();
            LocaleData uiCultureData = this.GetRecommendedUICulture();
            if (null == cultureData)
            {
                cultureData = uiCultureData;
            }

            if (null == uiCultureData)
            {
                uiCultureData = cultureData;
            }

            if (cultureData != null)
            {
                RecommendedCulture.Text = cultureData.EnglishName;
                if (useRecommendedCulture)
                {
                    lstCulture.SelectedValue = cultureData.Culture;
                }
            }
            else
            {
                RecommendedCulture.Text = "?";
            }

            if (uiCultureData != null)
            {
                RecommendedUICulture.Text = uiCultureData.EnglishName;
                if (useRecommendedUICulture)
                {
                    lstUICulture.SelectedValue = uiCultureData.UICulture;
                }
            }
            else
            {
                RecommendedUICulture.Text = "?";
            }
        }
    }

    /// <summary>
    /// Gets the recommended Culture
    /// </summary>
    /// <returns>The LocaleData matching the recommended Culture</returns>
    private LocaleData GetRecommendedCulture()
    {
        LocaleData data = null;
        if (String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            return null;
        }

        // Find .NET Culture mostly closely associated with the selected REGION.
        string selLang = string.Empty;
        string likelyTag = string.Empty;
        string likelyRegion = this.selectedLanguageTag.RegionSubtag;
        if (null == this.SystemLocales.Find(d => d.Culture.EndsWith("-" + likelyRegion)))
        {
            // find likely territory (ie, country) for this region (probably multinational)
            likelyRegion = this.GetRegionValue(this.selectedLanguageTag.RegionSubtag, "likely");
            if (null == this.SystemLocales.Find(d => d.Culture.EndsWith("-" + likelyRegion)))
            {
                likelyRegion = "US";
            }
        }

        int p = this.selectedLanguage.IndexOf('-');
        selLang = p >= 0 ? this.selectedLanguage.Substring(0, p) : this.selectedLanguage; // ignore any region in the language

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag))
        {
            // search with most specificity: gg-Ssss-CC
            data = this.SystemLocales.Find(d => d.Culture == selLang + "-" + this.selectedLanguageTag.ScriptSubtag + "-" + likelyRegion);
        }

        if (null == data)
        {
            // search without script subtag: gg-CC
            data = this.SystemLocales.Find(d => d.Culture == selLang + "-" + likelyRegion);
        }

        if (null == data)
        {
            // find likely locale for this region (language undefined)
            likelyTag = this.GetLikelyTag("und-" + likelyRegion);
            if (!String.IsNullOrEmpty(likelyTag))
            {
                // search with most specificity: gg-Ssss-CC
                data = this.SystemLocales.Find(d => d.Culture == likelyTag);
                if (null == data)
                {
                    string[] subtags = likelyTag.Split('-');
                    if (3 == subtags.Length)
                    {
                        // search without script subtag: gg-CC
                        likelyTag = subtags[0] + "-" + subtags[2];
                        data = this.SystemLocales.Find(d => d.Culture == likelyTag);
                    }
                }
            }
        }

        if (null == data)
        {
            // search for anything in this region: *-CC
            data = this.SystemLocales.Find(d => d.Culture.EndsWith("-" + likelyRegion));
        }

        return data;
    }

    /// <summary>
    /// Gets the recommended UICulture
    /// </summary>
    /// <returns>The LocaleData matching the recommended UICulture</returns>
    private LocaleData GetRecommendedUICulture()
    {
        LocaleData data = null;

        // Find .NET UICulture mostly closely associated with the selected LANGUAGE.
        string selLang = string.Empty;
        string likelyTag = string.Empty;
        int p = this.selectedLanguage.IndexOf('-');
        selLang = this.selectedLanguage;
        if (p >= 0)
        {
            selLang = this.selectedLanguage;

            // search given language-country combo: gg-CC or gg-Ssss
            data = this.SystemLocales.Find(d => d.UICulture == selLang);
            selLang = this.selectedLanguage.Substring(0, p); // ignore the region because it was not found
        }

        if (null == data && !String.IsNullOrEmpty(this.selectedLanguageTag.ScriptSubtag) && !String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            // search with most specificity: gg-Ssss-CC
            data = this.SystemLocales.Find(d => d.UICulture == selLang + "-" + this.selectedLanguageTag.ScriptSubtag + "-" + this.selectedLanguageTag.RegionSubtag);
        }

        if (null == data && !String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            // search without script subtag: gg-CC
            data = this.SystemLocales.Find(d => d.UICulture == selLang + "-" + this.selectedLanguageTag.RegionSubtag);
        }

        if (null == data)
        {
            // find likely locale for this language (region undefined)
            likelyTag = this.GetLikelyTag(selLang);
            if (!String.IsNullOrEmpty(likelyTag))
            {
                // search with most specificity: gg-Ssss-CC
                data = this.SystemLocales.Find(d => d.UICulture == likelyTag);
                if (null == data)
                {
                    string[] subtags = likelyTag.Split('-');
                    if (3 == subtags.Length)
                    {
                        // search without script subtag: gg-CC
                        likelyTag = subtags[0] + "-" + subtags[2];
                        data = this.SystemLocales.Find(d => d.UICulture == likelyTag);
                    }
                }
            }
        }

        if (null == data && !String.IsNullOrEmpty(selLang))
        {
            // search for anything of this language: gg-*
            data = this.SystemLocales.Find(d => d.UICulture.StartsWith(selLang));
        }

        return data;
    }

    /// <summary>
    /// Gets the revision date of the CLDR
    /// </summary>
    /// <returns>The data as a DateTime</returns>
    private DateTime GetDateOfCLDR()
    {
        DateTime dateOfCLDR = new DateTime(2010, 3, 26);
        string path = new Uri(Request.Url, "CLDR/common/supplemental/likelySubtags.xml").AbsoluteUri;
        System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(path);
        System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
        if (nav != null)
        {
            // <generation date="$Date: 2010-03-26 21:19:06 -0500 (Fri, 26 Mar 2010) $"/>
            System.Xml.XPath.XPathNavigator node = nav.SelectSingleNode("supplementalData/generation/@date");
            if (node != null && !String.IsNullOrEmpty(node.Value))
            {
                string date = node.Value;
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(date, @"[0-9]{4}\-[0-9]{2}\-[0-9]{2}");
                if (match != null && match.Success)
                {
                    date = match.Value;
                    dateOfCLDR = DateTime.Parse(date);
                }
            }
        }

        return dateOfCLDR;
    }

    /// <summary>
    /// Gets the likely language tag given a partial language tag.
    /// </summary>
    /// <param name="partialTag">A partial language tag</param>
    /// <returns>The likely language tag</returns>
    private string GetLikelyTag(string partialTag)
    {
        if (String.IsNullOrEmpty(partialTag))
        {
            return string.Empty;
        }

        string likelyTag = string.Empty;
        partialTag = partialTag.Replace('-', '_');
        if (null == this.likelySubtagsNav)
        {
            string path = new Uri(Request.Url, "CLDR/common/supplemental/likelySubtags.xml").AbsoluteUri;
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(path);
            this.likelySubtagsNav = doc.CreateNavigator();
        }

        if (this.likelySubtagsNav != null)
        {
            System.Xml.XPath.XPathNavigator node = this.likelySubtagsNav.SelectSingleNode(String.Format("supplementalData/likelySubtags/likelySubtag[@from='{0}']/@to", partialTag));
            if (node != null)
            {
                likelyTag = node.Value.Replace('_', '-');
            }
        }

        return likelyTag;
    }

    /// <summary>
    /// Builds the flag icon section
    /// </summary>
    private void BuildFlagSection()
    {
        const string FlagnameLcidFormat = "flag{0:x4}.gif";
        const string FlagnameFormat = "flag_{0}.gif";
        const string FlagnameIetfFormat = "flag_{0}-{1}.gif"; // 0=language, 1=region
        string flagFilename = string.Empty;
        string flagUrl = string.Empty;
        bool isDefaultSelected = false;
        rblFlag.Items.Clear();
        ListItem item = null;
        int lcid = 0;
        bool useRecommended = true;
        if (this.Locale != null)
        {
            lcid = this.Locale.LCID;

            // Note: flagFilename may be empty even when flagUrl is not b/c Url takes best guess when flagFilename is empty.
            flagFilename = this.Locale.FlagFile;
            flagUrl = this.Locale.FlagUrl;
            useRecommended = String.IsNullOrEmpty(flagFilename) || flagFilename == hdnRecommendedFlag.Value;
            if (!useRecommended)
            {
                this.AddFlagItem(flagFilename, ref isDefaultSelected);
            }
        }

        // Flag based on Loc 
        string loc = txtLoc.Text;
        if (!String.IsNullOrEmpty(loc) && loc != "und")
        {
            flagFilename = String.Format(FlagnameFormat, loc);
            this.AddFlagItem(flagFilename, ref isDefaultSelected);
        }

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.LanguageSubtag) && this.selectedLanguageTag.LanguageSubtag != "und")
        {
            if (!this.isAssumedRegion && !String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
            {
                // Flag based on direct LANGUAGE and REGION subtag
                flagFilename = String.Format(FlagnameIetfFormat, this.selectedLanguageTag.LanguageSubtag, this.selectedLanguageTag.RegionSubtag);
                this.AddFlagItem(flagFilename, ref isDefaultSelected);
            }

            // Flag based on direct LANGUAGE subtag
            flagFilename = String.Format(FlagnameFormat, this.selectedLanguageTag.LanguageSubtag);
            this.AddFlagItem(flagFilename, ref isDefaultSelected);
        }

        // Flag based on LCID
        if ((null == this.Locale || useRecommended) && !String.IsNullOrEmpty(lstUICulture.SelectedValue))
        {
            System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo(lstUICulture.SelectedValue);
            if (info != null)
            {
                lcid = info.LCID;
            }
        }

        flagFilename = String.Format(FlagnameLcidFormat, lcid);
        item = this.AddFlagItem(flagFilename, ref isDefaultSelected);
        if (null == item || !item.Enabled)
        {
            Ektron.Cms.API.Localization api = new Ektron.Cms.API.Localization();
            flagUrl = api.GetFlagUrl(lcid); // format: flagNNNN.gif where NNNN is hex LCID
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(flagUrl, @"flag[0-9a-fA-F]{4}\.gif");
            if (match != null && match.Success)
            {
                flagFilename = match.Value;
                this.AddFlagItem(flagFilename, ref isDefaultSelected);
            }
        }

        if (!String.IsNullOrEmpty(this.selectedLanguageTag.RegionSubtag))
        {
            // Flag based on MULTINATIONAL REGION flag suggested in regions.xml
            flagFilename = this.GetRegionValue(this.selectedLanguageTag.RegionSubtag, "flag");
            this.AddFlagItem(flagFilename, ref isDefaultSelected);

            // Flag based on direct REGION subtag
            flagFilename = String.Format(FlagnameIetfFormat, "und", this.selectedLanguageTag.RegionSubtag);
            this.AddFlagItem(flagFilename, ref isDefaultSelected);
        }

        hdnRecommendedFlag.Value = useRecommended ? rblFlag.SelectedValue : string.Empty;

        lblFlagFolder.Text = GetMessage("generic folder") + ": " + CommonApi.AppImgPath + "flags/";
    }

    /// <summary>
    /// Adds a flag to the list of possible flags
    /// </summary>
    /// <param name="flagFilename">The name of the file of the flag image</param>
    /// <param name="isDefaultSelected">Indicates whether the one of the items is selected</param>
    /// <returns>Returns a new ListItem control</returns>
    private ListItem AddFlagItem(string flagFilename, ref bool isDefaultSelected)
    {
        // Selects the item unless isDefaultSelected==true
        if (String.IsNullOrEmpty(flagFilename) || ("flag0000.gif" == flagFilename))
        {
            return null;
        }

        ListItem item = null;
        const string FlagFormatString = "<img src=\"{1}\" alt=\"{2}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /> {0}"; // 0=filename, 1=url, 2=title
        string flagsFolder = CommonApi.AppImgPath + "flags/";
        string flagFileMissingUrl = flagsFolder + "flagwhite.gif";

        if (null == rblFlag.Items.FindByValue(flagFilename))
        {
            string flagUrl = flagsFolder + flagFilename;
            bool exists = System.IO.File.Exists(Server.MapPath(flagUrl));
            if (!exists)
            {
                flagUrl = flagFileMissingUrl;
            }

            item = new ListItem(String.Format(FlagFormatString, flagFilename, flagUrl, exists ? flagFilename : string.Empty), flagFilename);
            if (exists)
            {
                if (!isDefaultSelected)
                {
                    item.Selected = true;
                    isDefaultSelected = true;
                }
            }
            else
            {
                item.Enabled = false;
            }

            rblFlag.Items.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Builds the display name sections, i.e., Native and English
    /// </summary>
    private void BuildDisplayNameSections()
    {
        if (this.Locale != null)
        {
            txtNativeName.Text = this.Locale.NativeName;
            txtEnglishName.Text = this.Locale.EnglishName;
        }

        bool useRecommendedNativeName = String.IsNullOrEmpty(txtNativeName.Text) || txtNativeName.Text == RecommendedNativeName.Text;
        bool useRecommendedEnglishName = String.IsNullOrEmpty(txtEnglishName.Text) || txtEnglishName.Text == RecommendedEnglishName.Text;

        // Get recommended display names
        string localeLanguageName = string.Empty;
        string localeRegionName = string.Empty;
        bool isRightToLeft = false;

        // English language
        AlternateEnglishNameContainer.Visible = false;
        string englishLocaleName = GetLocaleName(
            new Uri(Request.Url, "CLDR/common/main/en.xml").AbsoluteUri,
            this.selectedLanguage, 
            this.selectedLanguageTag.RegionSubtag, 
            ref localeLanguageName, 
            ref localeRegionName, 
            ref isRightToLeft);
        if (this.isAssumedRegion && englishLocaleName != localeLanguageName)
        {
            AlternateEnglishNameContainer.Visible = true;
            AlternateEnglishName.Text = englishLocaleName;
            englishLocaleName = localeLanguageName;
        }

        if (!String.IsNullOrEmpty(englishLocaleName))
        {
            RecommendedEnglishName.Text = englishLocaleName;
            if (useRecommendedEnglishName)
            {
                txtEnglishName.Text = englishLocaleName;
            }
        }
        else
        {
            RecommendedEnglishName.Text = "?";
        }

        // Native names default to English names

        // Native language
        string nativeLocaleName = string.Empty;
        AlternateNativeNameContainer.Visible = false;
        if (!String.IsNullOrEmpty(this.selectedLanguage))
        {
            int p = this.selectedLanguage.IndexOf('-');
            string selLangOnly = p >= 0 ? this.selectedLanguage.Substring(0, p) : this.selectedLanguage; // ignore any region in the language
            nativeLocaleName = GetLocaleName(
                new Uri(Request.Url, String.Format("CLDR/common/main/{0}.xml", selLangOnly)).AbsoluteUri,
                this.selectedLanguage, 
                this.selectedLanguageTag.RegionSubtag, 
                ref localeLanguageName, 
                ref localeRegionName, 
                ref isRightToLeft);
            if (this.isAssumedRegion && nativeLocaleName != localeLanguageName)
            {
                AlternateNativeNameContainer.Visible = true;
                AlternateNativeName.Text = nativeLocaleName;
                nativeLocaleName = localeLanguageName;
            }
        }

        chkIsRightToLeft.Checked = isRightToLeft;
        if (isRightToLeft)
        {
            txtNativeName.Attributes.Add("dir", "rtl");
            txtNativeName.Style.Add("text-align", "right");
            RecommendedNativeName.Attributes.Add("dir", "rtl");
            RecommendedNativeName.Style.Add("text-align", "right");
            AlternateNativeName.Attributes.Add("dir", "rtl");
            AlternateNativeName.Style.Add("text-align", "right");
        }

        if (String.IsNullOrEmpty(nativeLocaleName))
        {
            nativeLocaleName = englishLocaleName;
        }

        if (!String.IsNullOrEmpty(nativeLocaleName))
        {
            RecommendedNativeName.Text = nativeLocaleName;
            if (useRecommendedNativeName)
            {
                txtNativeName.Text = nativeLocaleName;
            }
        }
        else
        {
            RecommendedNativeName.Text = "?";
        }
    }
}
