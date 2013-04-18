using System;
using Ektron.Cms.Workarea.Framework;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for CalendarPopup.
    /// </summary>
	public partial class CalendarPopup : WorkareaDialogPage
    {
        private void Page_Load(object sender, EventArgs e)
        {

			this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);
            hdnSelectedDate_ClientID.Text = hdnSelectedDate.ClientID;

			this.Title.Text = this.GetMessage("lbl select date");
			string titleDateFormat = this.GetMessage("title date format");
			if (!titleDateFormat.EndsWith("-HC"))
			{
				this.calDate.TitleFormat = titleDateFormat;
			}
			this.chkRemoveDate.Text = this.GetMessage("lbl remove date");
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        ///        Required method for Designer support - do not modify
        ///        the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion

    }
}