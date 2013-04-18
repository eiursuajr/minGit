using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common.Cache;
using System.ComponentModel;

namespace Ektron
{
	[ValidationProperty("Content")]
	public partial class ContentDesignerWithValidator : WorkareaBaseControl
	{
		private const string requestCacheComponentName = "ContentDesigner";
		private SiteAPI _siteApi = new SiteAPI();

		protected override void OnInit(EventArgs e)
		{
			// WARNING: Properties may be set by caller PRIOR to OnInit or Page_Init. Init is fired when this control is added to the page, not on LoadControl.

			base.OnInit(e);

			ContentDesigner.ScriptLocation = _siteApi.ApplicationPath + "ContentDesigner/";
			ContentDesigner.ScriptResourceLocation = _siteApi.ApplicationPath + "java/";
			ContentDesigner.SkinsPath = _siteApi.ApplicationPath + "csslib/";
			ContentDesigner.Skin = "Editor";

			int userLang = _siteApi.RequestInformationRef.UserCulture;
			int contLang = _siteApi.RequestInformationRef.ContentLanguage;
            if (contLang <= 0) contLang = _siteApi.RequestInformationRef.DefaultContentLanguage;
			LanguageData objUserLangData = RequestCache<LanguageData>.Get("UserLanguageData", requestCacheComponentName);
			if (null == objUserLangData)
			{
				objUserLangData = _siteApi.GetLanguageById(userLang);
				RequestCache<LanguageData>.Insert("UserLanguageData", requestCacheComponentName, objUserLangData);
			}
			LanguageData objContentLangData = objUserLangData;
			if (contLang != userLang)
			{
				objContentLangData = RequestCache<LanguageData>.Get("ContentLanguageData", requestCacheComponentName);
				if (null == objContentLangData)
				{
					objContentLangData = _siteApi.GetLanguageById(contLang);
					RequestCache<LanguageData>.Insert("ContentLanguageData", requestCacheComponentName, objContentLangData);
				}
			}

			ContentDesigner.Language = objUserLangData.XmlLang;
			ContentDesigner.UserLanguage = objUserLangData.Id;
			ContentDesigner.ContentLanguage = objContentLangData.Id;
			ContentDesigner.SpellDictionaryLanguage = objContentLangData.XmlLang;

			if (null == ContentDesigner.CssFiles || 0 == ContentDesigner.CssFiles.Length)
			{
				this.Stylesheet = ContentDesigner.SkinsPath + "ektron.workarea.css";
			}
			this.Toolbars = _configuration;
			ContentDesigner.ShowSubmitCancelButtons = false;

			SettingsData objSiteVars = RequestCache<SettingsData>.Get("SettingsData", requestCacheComponentName);
			if (null == objSiteVars)
			{
				objSiteVars = _siteApi.GetSiteVariables(_siteApi.UserId);
				RequestCache<SettingsData>.Insert("SettingsData", requestCacheComponentName, objSiteVars);
			}
			ContentDesigner.AccessChecks = Ektron.ContentDesigner.ContentDesigner.AccessibilityCheck.None;
			if ("2" == objSiteVars.Accessibility)
				ContentDesigner.AccessChecks = Ektron.ContentDesigner.ContentDesigner.AccessibilityCheck.Enforce;
			else if ("1" == objSiteVars.Accessibility)
				ContentDesigner.AccessChecks = Ektron.ContentDesigner.ContentDesigner.AccessibilityCheck.Warn;
			else if ("0" == objSiteVars.Accessibility)
				ContentDesigner.AccessChecks = Ektron.ContentDesigner.ContentDesigner.AccessibilityCheck.None;
#if (DEBUG)
            else
                throw new ArgumentOutOfRangeException("Accessibility", String.Format("Unknown Accessibility Level: {0}", objSiteVars.Accessibility));
#endif
			ContentDesigner.StripFormattingOnPaste = Utilities.MSWordFilterOptions(objSiteVars);
			ContentDesigner.AllowScripts = _allowScripts;
			ContentDesigner.ShowHtmlMode = _showHtmlMode;
			ContentDesigner.ShowPreviewMode = _showPreviewMode;
			ContentDesigner.EnableHtmlIndentation = false;
			ContentDesigner.StripAbsoluteAnchorPaths = false;
			ContentDesigner.StripAbsoluteImagesPaths = false;
            ContentDesigner.ShowTemporaryMarkers = Utilities.GetTemporaryMarkersPreference(Request);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (this.AllowFonts)
			{
				LoadAllFonts();
			}

			Ektron.Cms.API.JS.RegisterJSBlock(this,
				String.Format("Ektron.ContentDesigner.instances[\"{0}\"] = Ektron.ContentDesigner.instances[\"{1}\"]; window[\"{0}\"] = window[\"{1}\"];", this.ID, ContentDesigner.ClientID), 
				"ContentDesignerInstance_" + ContentDesigner.ClientID);

			base.OnLoad(e);
		}

		/// <summary>
		/// Access to the ContentDesigner server control
		/// </summary>
		public Ektron.ContentDesigner.ContentDesigner Editor { get { return ContentDesigner; } }
		/// <summary>
		/// Access to the RegularExpressionValidator server control
		/// </summary>
		public RegularExpressionValidator Validator { get { return ContentValidator; } }

		public Unit Height { get { return ContentDesigner.Height; } set { ContentDesigner.Height = value; } }
		public Unit Width { get { return ContentDesigner.Width; } set { ContentDesigner.Width = value; } }

		public int CurrentCharCount { get { return ContentDesigner.CurrentCharCount; } set { ContentDesigner.CurrentCharCount = value; } }

		public bool ShowPasteWarning { get { return ContentDesigner.ShowPasteWarning; } set { ContentDesigner.ShowPasteWarning = value; } }

		public int WordPasteThreshold { get { return ContentDesigner.WordPasteThreshold; } set { ContentDesigner.WordPasteThreshold = value; } }

		/// <summary>
		/// Gets the path to the ~/workarea/ContentDesigner/ folder, which is the location of the editor's JavaScript and XSLT files
		/// </summary>
		public string ScriptLocation { get { return ContentDesigner.ScriptLocation; } }

		private bool _allowFonts = false;
		/// <summary>
		/// Specifies whether or not to load the list of fonts defined in the CMS workarea setup.
		/// Default is false.
		/// </summary>
		public bool AllowFonts { get { return _allowFonts; } set { _allowFonts = value; } }

		/// <summary>
		/// Some predefined toolbar configuration files for Toolbars property. 
		/// Files are located in ~/workarea/ContentDesigner/configurations/. 
		/// More exist in that same location or a file may be specified directly using the ToolsFile property.
		/// </summary>
		public enum Configuration
		{
			[Description("NoToolbars.xml")]
			NoToolbars,
			[Description("InterfaceMinimal.xml")]
			Minimal,
			[Description("InterfaceBasic.xml")]
			Basic,
			[Description("StandardEdit.aspx")]
			Standard,
			[Description("DesignerEdit.aspx")]
			Designer,
			[Description("DataDesignerEdit.aspx")]
			DataDesigner,
			[Description("DataEntryEdit.aspx")]
			DataEntry,
			[Description("XsltDesignerEdit.aspx")]
			XsltDesigner
		}
		private Configuration _configuration = Configuration.Standard;
		/// <summary>
		/// Specifies a predefined toolbar configuration file.
		/// A custom file may be specified using the ToolsFile property.
		/// </summary>
		public Configuration Toolbars 
		{ get { return _configuration; } 
			set 
			{
				_configuration = value;
				string filename = GetEnumDescription(_configuration);
				ContentDesigner.ToolsFile = _siteApi.ApplicationPath + "ContentDesigner/configurations/" + filename;
			} 
		}
		/// <summary>
		/// Specifies a custom toolbar configuration file.
		/// Several existing files are located in ~/workarea/ContentDesigner/configurations/. 
		/// A predefined toolbar configuration may be specified using the Toolbars property.
		/// </summary>
		public string ToolsFile { get { return ContentDesigner.ToolsFile; } set { ContentDesigner.ToolsFile = value; } }

		private string _stylesheet = "";
		/// <summary>
		/// Specifies a CSS stylesheet to apply to the content viewed in the editor. 
		/// To specify multiple stylesheets, use .Editor.CssFiles.
		/// </summary>
		public string Stylesheet { get { return _stylesheet; } set { _stylesheet = value; ContentDesigner.CssFiles = new string[] { value }; } }
		//public string[] CssFiles { get { return ContentDesigner.CssFiles; } set { ContentDesigner.CssFiles = value; } }

		/// <summary>
		/// (Optional) Specifies the ID of the folder associated with the content.
		/// </summary>
		public long FolderId { get { return ContentDesigner.FolderId; } set { ContentDesigner.FolderId = value; } }

		/// <summary>
		/// Set or get the content.
		/// </summary>
		public string Content { get { return ContentDesigner.Content; } set { ContentDesigner.Content = value; } }
		/// <summary>
		/// Gets the plain text (no tags). Valid only during post back.
		/// </summary>
		public string Text { get { return ContentDesigner.Text; } }
		/// <summary>
		/// Sets the DataEntryXslt and DataSchema properties given a Smart Form design package.
		/// Applicable when authoring smart form content.
		/// The Configuration.DataEntry toolbar (or similar) should be used to author smart form content.
		/// </summary>
		/// <param name="contentApi">Reference to the ContentAPI object</param>
		/// <param name="package">Smart Form design package (string)</param>
		public void LoadPackage(ContentAPI contentApi, string package)
		{
			//string strDataEntryXslt = contentApi.TransformXsltPackage(package, Server.MapPath(this.ScriptLocation + "unpackageView.xslt"), true);
			//string strDataSchema = contentApi.TransformXsltPackage(package, Server.MapPath(this.ScriptLocation + "unpackageSchema.xslt"), true);
			string strDesignPage = contentApi.TransformXsltPackage(package, Server.MapPath(this.ScriptLocation + "unpackageDesign.xslt"), true);
			string strDataEntryXslt = contentApi.TransformXsltPackage(strDesignPage, Server.MapPath(this.ScriptLocation + "DesignToEntryXSLT.xslt"), true);
			string strDataSchema = contentApi.TransformXsltPackage(strDesignPage, Server.MapPath(this.ScriptLocation + "DesignToSchema.xslt"), true);
			this.DataEntryXslt = strDataEntryXslt;
            this.DataSchema = strDataSchema;
		}
		/// <summary>
		/// Sets the data entry XSLT needed when authoring smart form content.
		/// Applicable when authoring smart form content.
		/// The Configuration.DataEntry toolbar (or similar) should be used to author smart form content.
		/// </summary>
		public string DataEntryXslt 
		{ 
			set 
			{ 
				ContentDesigner.EditorRenderingMode = Ektron.ContentDesigner.ContentDesigner.EditorMode.DataEntry; 
				ContentDesigner.DataEntryXslt = value; 
			} 
		}
		/// <summary>
		/// Sets the data entry XML Schema needed for validation when authoring smart form content.
		/// The schema may be omitted if validation is not required.
		/// Applicable when authoring smart form content.
		/// </summary>
		public string DataSchema { set { ContentDesigner.DataSchema = value; } }
		/// <summary>
		/// Sets the XML document. Use the Content property to read the XML document during post back.
		/// Applicable when authoring smart form content.
		/// </summary>
		public string DataDocumentXml { set { ContentDesigner.DataDocumentXml = value; } }

		private bool _allowScripts = true;
		/// <summary>
		/// Specifies whether or not SCRIPT tags, that is, JavaScript, are allowed. 
		/// Scripts may be prohibited for improved security and control over the presentation.
		/// </summary>
		public bool AllowScripts { get { return ContentDesigner.AllowScripts; } set { ContentDesigner.AllowScripts = _allowScripts = value; } }
		private bool _showHtmlMode = true;
		/// <summary>
		/// Specifies whether or not source can be viewed within the editor.
		/// </summary>
		public bool ShowHtmlMode { get { return ContentDesigner.ShowHtmlMode; } set { ContentDesigner.ShowHtmlMode = _showHtmlMode = value; } }
		private bool _showPreviewMode = false;
		/// <summary>
        /// Specifies whether or not Data Entry Preview can be viewed within the editor.
        /// </summary>
		public bool ShowPreviewMode { get { return ContentDesigner.ShowPreviewMode; } set { ContentDesigner.ShowPreviewMode = _showPreviewMode = value; } }
		/// <summary>
		/// Specifies JavaScript to be run when a command is executed, for example, a toolbar button is pressed.
		/// </summary>
		public string OnClientCommandExecuted { get { return ContentDesigner.OnClientCommandExecuted; } set { ContentDesigner.OnClientCommandExecuted = value; } }
		/// <summary>
		/// Specifies JavaScript to be run when the editor is initialized.
		/// </summary>
		public string OnClientInit { get { return ContentDesigner.OnClientInit; } set { ContentDesigner.OnClientInit = value; } }
		/// <summary>
		/// Specifies JavaScript to be run when the editor is loaded.
		/// </summary>
		public string OnClientLoad { get { return ContentDesigner.OnClientLoad; } set { ContentDesigner.OnClientLoad = value; } }
		/// <summary>
		/// Specifies JavaScript to be run when the mode is changed, for example, when switching between WYSIWYG and source view modes.
		/// </summary>
		public string OnClientModeChange { get { return ContentDesigner.OnClientModeChange; } set { ContentDesigner.OnClientModeChange = value; } }

		/// <summary>
		/// Calls the Validate method in the validator control.
		/// </summary>
		public void Validate() { ContentValidator.Validate(); }
		/// <summary>
		/// Sets or gets the IsValid property in the validator control.
		/// </summary>
		public bool IsValid { get { return ContentValidator.IsValid; } set { ContentValidator.IsValid = value; } }
		/// <summary>
		/// Sets or gets the ValidationExpression property in the validator control.
		/// </summary>
		public string ValidationExpression { get { return ContentValidator.ValidationExpression; } set { ContentValidator.ValidationExpression = value; } }
		/// <summary>
		/// Sets or gets the ErrorMessage property in the validator control.
		/// </summary>
		public string ErrorMessage { get { return ContentValidator.ErrorMessage; } set { ContentValidator.ErrorMessage = value; } }

		/// <summary>
		/// Sets the permissions given a PermissionData object. 
		/// The permissions, among other things, disables toolbar buttons that are not allowed.
		/// </summary>
		/// <param name="permissions">A PermissionData object for the authenticated user</param>
		public void SetPermissions(PermissionData permissions)
		{
			ContentDesigner.LibraryAllowed = permissions.IsReadOnlyLib;
			ContentDesigner.CanModifyImg = permissions.CanAddToImageLib;
		}

		private void LoadAllFonts()
		{
			FontData[] aryFonts = RequestCache<FontData[]>.Get("FontData", requestCacheComponentName);
			if (null == aryFonts)
			{
				ContentAPI contentApi = new ContentAPI();
				aryFonts = contentApi.GetAllFonts();
				RequestCache<FontData[]>.Insert("FontData", requestCacheComponentName, aryFonts);
			}
			if (aryFonts != null)
			{
				for (int i = 0; i < aryFonts.Length; i++)
				{
					ContentDesigner.FontNames.Add(aryFonts[i].Face);
				}
			}
		}
	}
}