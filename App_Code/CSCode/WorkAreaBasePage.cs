//-----------------------------------------------------------------------
// <copyright file="WorkAreaBasePage.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Ektron.Cms.Workarea.Framework
{
    using System;
    using System.Collections.Generic;
	using System.Web;

	/// <summary>
	/// Alias for WorkareaBasePage
	/// </summary>
	public abstract class WorkareaDialogPage : WorkAreaBasePage
	{
        public WorkareaDialogPage() {
            base.ValidateUser();
        }
	}

	/// <summary>
	/// Base class for all workarea pages - *.aspx pages
	/// Security should be enforced here
	/// </summary>
	public abstract class WorkAreaBasePage : System.Web.UI.Page 
	{
        /// <summary>
        /// Reference for GetMessage method
        /// </summary>
        private Ektron.Cms.Common.EkMessageHelper refMsg = null;

        /// <summary>
        /// Reference to Common API object for frequently called methods
        /// </summary>
        private Ektron.Cms.CommonApi api = null;

        /// <summary>
        /// Reference to locale manager object
        /// </summary>
        private Ektron.Cms.Framework.Localization.LocaleManager localeMgr = null;

        /// <summary>
        /// Holds the reference to the ContentApi object.
        /// </summary>
        private Ektron.Cms.ContentAPI _contentApi = null;

        /// <summary>
        /// Initializes a new instance of the WorkAreaBasePage class.
        /// </summary>
		public WorkAreaBasePage()
		{
		}

        #region Properties

		/// <summary>
		/// Gets the path to the Script directory
		/// </summary>
		public string ScriptPath
		{
			get
			{
                return ResolveUrl(this.GetCommonApi().AppPath + "java/");
			}
		}

		/// <summary>
		/// Gets the path to the Controls directory for the current page's theme
		/// </summary>
		public string SkinControlsPath
		{
			get
			{
                return ResolveUrl(this.GetCommonApi().AppPath + "csslib/");
            }
        }

        protected ContentAPI ContentApi { get { return ((null == _contentApi) ? _contentApi = new ContentAPI() : _contentApi); } }

		#endregion

        /// <summary>
        /// Sets the .NET Culture and UICulture for this page.
        /// </summary>
        protected override void InitializeCulture()
        {
            base.InitializeCulture();

            Ektron.Cms.CommonApi api = this.GetCommonApi();
            Ektron.Cms.Localization.LocaleData data = api.GetRequestedLocale(api.UserLanguage);
            if (data != null)
            {
                System.Globalization.CultureInfo currentCulture = Ektron.Cms.Common.EkFunctions.GetCultureInfo(data.Culture);
                System.Globalization.CultureInfo currentUICulture = 
                    (data.Culture == data.UICulture ? currentCulture : Ektron.Cms.Common.EkFunctions.GetCultureInfo(data.UICulture));
                this.Culture = currentCulture.Name;
                this.UICulture = currentUICulture.Name;
            }
        }

        /// <summary>
        /// Common API object for frequently called methods
        /// </summary>
        /// <returns>Reference to Common API object for frequently called methods</returns>
        protected Ektron.Cms.CommonApi GetCommonApi()
        {
            if (null == this.api)
            {
                this.api = new Ektron.Cms.CommonApi();
            }

            return this.api;
        }

        /// <summary>
        /// Locale manager object
        /// </summary>
        /// <returns>Reference to locale manager object</returns>
        protected Ektron.Cms.Framework.Localization.LocaleManager LocaleManager
        {
            get
            {
                if (null == this.localeMgr)
                {
                    this.localeMgr = new Ektron.Cms.Framework.Localization.LocaleManager();
                }

                return this.localeMgr;
            }
        }

        /// <summary>
        /// Gets localized string given a key. The localized string is from the Ektron CMS resource.
        /// </summary>
        /// <param name="key">ID of string</param>
        /// <returns>Localized string</returns>
        protected string GetMessage(string key)
        {
            if (null == this.refMsg)
            {
                this.refMsg = this.GetCommonApi().EkMsgRef;
            }

            return this.refMsg.GetMessage(key);
        }

        /// <summary>
        /// Ensures workarea pages are accessed from other pages within the same site.
        /// This protects web service-like pages (e.g., ekajaxtransform) from DOS attacks.
        /// </summary>
        protected void AssertInternalReferrer(string sessionId)
        {
            if (sessionId != Session.SessionID)
            {
                throw new System.Exception("This page may be used only from within this site.");
            }
            AssertInternalReferrer();
        }
        /// <summary>
        /// Ensures workarea pages are accessed from other pages within the same site.
        /// This protects web service-like pages (e.g., ekajaxtransform) from DOS attacks.
        /// </summary>
        protected void AssertInternalReferrer()
        {
            if (Request.IsLocal || Request.Url.IsLoopback)
            {
                return;
            }

            if (null == Request.UrlReferrer || Request.Url.Authority != Request.UrlReferrer.Authority)
            {
                throw new System.Exception("This page may be used only from within this site.");
            }
        }

        /// <summary>
        /// Registers CSS for ContentDesigner dialogs
        /// </summary>
        protected void RegisterWorkareaCssLink()
        {
            Ektron.Cms.API.Css.RegisterCss(this, this.SkinControlsPath + "Editor/WorkArea.css", "WorkAreaCSS");
        }

        /// <summary>
        /// Registers CSS for ContentDesigner dialogs
        /// </summary>
        protected void RegisterDialogCssLink()
        {
            Ektron.Cms.API.Css.RegisterCss(this, this.SkinControlsPath + "Editor/Dialogs.css", "EditorDialogsCSS");
        }

        /// <summary>
        /// Ensures user is a logged in CMS user, redirects to log-in page if not.
        /// </summary>
        protected void ValidateUser() {
            if (!ContentApi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", ContentApi.UserId)) {
                HttpContext.Current.Response.Redirect(ContentApi.AppPath + "login.aspx?fromLnkPg=1", false);
            }
        }

	}
}