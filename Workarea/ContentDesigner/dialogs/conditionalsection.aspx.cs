//-----------------------------------------------------------------------
// <copyright file="conditionalsection.aspx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Ektron.ContentDesigner.Dialogs
{
    using System.Collections.Generic;
    using System.Data;
    using System.Web.UI.WebControls;
    using Ektron.Cms;
    using Ektron.Cms.Workarea.Framework;
    using System.Text;
    using Ektron.Cms.Common;
    /// <summary>
    /// Adds Locale Ids to the selected text from content designer in workarea.
    /// </summary>  
    public partial class ConditionalSection : WorkareaDialogPage
    {
        protected EkMessageHelper _MessageHelper;
        protected StyleHelper _StyleHelper;
        protected ContentAPI _ContentApi = new ContentAPI();
        /// <summary>
        /// register Workarea CSS,Dialog CSS
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            _MessageHelper = _ContentApi.EkMsgRef;
            _StyleHelper = new StyleHelper();

            this.RegisterWorkareaCssLink();
            this.RegisterDialogCssLink();

            ConditionalToolBar();

            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            this.Title.Text = this.GetMessage("lbl conditional section");

        }

        private void ConditionalToolBar()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table><tbody><tr>");
            sb.Append("<td>");
            sb.Append(_StyleHelper.GetHelpButton("conditionalsection", ""));
            sb.Append("</td>");
            sb.Append("</tr></tbody></table>");

            divToolBar.InnerHtml = sb.ToString();
        }
    }
}
