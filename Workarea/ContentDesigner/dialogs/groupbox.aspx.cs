using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.Workarea.Framework;
using Ektron.Cms;
using Ektron.Cms.Common;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for GroupBox.
    /// </summary>
    public partial class GroupBox : WorkareaDialogPage
    {
        protected ContentAPI _ContentApi;
        protected EkMessageHelper _MessageHelper;

        private void Page_Load(object sender, System.EventArgs e)
        {
            _ContentApi = new ContentAPI();
            _MessageHelper = _ContentApi.EkMsgRef;
            if (Request.RawUrl.ToLower().Contains("<script"))
            {
                Utilities.ShowError(_MessageHelper.GetMessage("invalid querstring"));
                return;
            } 
            this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

			this.Title.Text = this.GetMessage("lbl group box");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.RadTabStrip1.Tabs[0].Text + "Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab advanced");
            this.RadTabStrip1.Tabs[1].ToolTip = this.RadTabStrip1.Tabs[1].Text + "Tab";
            this.RadTabStrip1.Tabs[2].Text = this.GetMessage("tab relevance");
            this.RadTabStrip1.Tabs[2].ToolTip = this.RadTabStrip1.Tabs[2].Text + "Tab";
			this.lblAppearance.InnerHtml = this.GetMessage("lbl appearance");
			this.lblNoBorder.InnerHtml = this.GetMessage("lbl no border");
			this.lblShowBorder.InnerHtml = this.GetMessage("lbl show border");
			this.lblCaption.InnerHtml = this.GetMessage("lbl caption c");
			this.txtCaption.Value = this.GetMessage("lbl fields");
			this.sEnterCaption.Text = this.GetMessage("msg enter caption");
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
