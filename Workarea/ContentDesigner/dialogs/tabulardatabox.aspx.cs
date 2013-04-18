using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.Workarea.Framework;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for TabularDataBox.
    /// </summary>
    public partial class TabularDataBox : WorkareaDialogPage
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
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

			this.Title.Text = this.GetMessage("lbl tabular data box");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip= this.GetMessage("tab general") + " Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab advanced");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("tab advanced") + " Tab";
            this.RadTabStrip1.Tabs[2].Text = this.GetMessage("tab relevance");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("tab relevance") + " Tab";
			this.lblRows.InnerHtml = this.GetMessage("lbl rows");
			this.lblRowDispName.InnerHtml = this.GetMessage("lbl row disp name");
			this.lblRowName.InnerHtml = this.GetMessage("lbl row name");
			this.txtRowDispName.Value = this.GetMessage("lbl field 2");
			this.txtRowName.Value = this.GetMessage("lbl field2");
			this.lblColumns.InnerHtml = this.GetMessage("lbl columns");
			this.lblCaption.InnerHtml = this.GetMessage("lbl caption c");
			this.txtCaption.Value = this.GetMessage("lbl fields");
			this.sFieldHeading.Text = this.GetMessage("lbl field heading");
			this.sInsertFieldHere.Text = this.GetMessage("lbl insert field here");
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
