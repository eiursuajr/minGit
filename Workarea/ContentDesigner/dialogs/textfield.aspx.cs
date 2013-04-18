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
    /// Summary description for TextField.
    /// </summary>
    public partial class TextField : WorkareaDialogPage
    {
        protected ContentAPI _ContentApi;
        protected EkMessageHelper _MessageHelper;

        private void Page_Load(object sender, System.EventArgs e)
        {
            _ContentApi = new ContentAPI();
            _MessageHelper = _ContentApi.EkMsgRef;
            if (EkFunctions.UrlDecode(Request.RawUrl).ToLower().Contains("<script") | EkFunctions.UrlDecode(Request.RawUrl).ToLower().Contains("</script"))
            {
                Utilities.ShowError(_MessageHelper.GetMessage("invalid querstring"));
                return;
            }
			this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronSmartFormJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "textfield.js", "textfieldJS");
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

			chkPassword.Style.Add(HtmlTextWriterStyle.Display, "none");

			this.Title.Text = this.GetMessage("lbl text field");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("tab general") + " Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab validation");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("tab validation") + " Tab";
			this.RadTabStrip1.Tabs[2].Text = this.GetMessage("tab data style");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("tab data style") + " Tab";
            this.RadTabStrip1.Tabs[3].Text = this.GetMessage("tab advanced");
            this.RadTabStrip1.Tabs[3].ToolTip = this.GetMessage("tab advanced") + " Tab";
			this.lblDefVal.InnerHtml = this.GetMessage("lbl default value c");
			this.chkRichArea.Text = this.GetMessage("lbl allow rich formatting");
            this.chkRichArea.ToolTip = this.GetMessage("lbl allow rich formatting");
            this.chkKeepPTag.Text = this.GetMessage("lbl keep single blocking tag");
            this.chkKeepPTag.ToolTip = this.GetMessage("lbl keep single blocking tag");
			this.chkMultiline.Text = this.GetMessage("lbl allow mult lines");
            this.chkMultiline.ToolTip = this.GetMessage("lbl allow mult lines");
			this.chkReadOnly.Text = this.GetMessage("lbl cannot be changed");
            this.chkReadOnly.ToolTip = this.GetMessage("lbl cannot be changed");
			this.chkHidden.Text = this.GetMessage("lbl invisible");
            this.chkHidden.ToolTip = this.GetMessage("lbl invisible");
			this.lblOptions.InnerHtml = this.GetMessage("generic options");
			this.sValue.Text = this.GetMessage("lbl value");
			this.sInvalidDefVal.Text = this.GetMessage("lbl default value");
            this.sWidth.Text = this.GetMessage("lbl imagetool resize width");
            this.sHeight.Text = this.GetMessage("lbl imagetool resize height");
            this.sSize.Text = this.GetMessage("lbl size colon");
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
