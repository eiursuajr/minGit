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
    /// Summary description for CalendarField.
    /// </summary>
    public partial class CalendarField : WorkareaDialogPage
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
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "calendarfield.js", "calendarfieldJS");
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

			this.Title.Text = this.GetMessage("lbl calendar field");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("tab general") + " Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab validation");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("tab validation") + " Tab";
			this.RadTabStrip1.Tabs[2].Text = this.GetMessage("tab data style");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("tab data style") + " Tab";
			this.RadTabStrip1.Tabs[3].Text = this.GetMessage("tab advanced");
            this.RadTabStrip1.Tabs[3].ToolTip = this.GetMessage("tab advanced") + " Tab";
			this.lblDefVal.InnerHtml = this.GetMessage("lbl default value c");
            this.lblCurrentDate.InnerHtml = this.GetMessage("lbl default to current date");
			
			if (Page.IsPostBack)
            {
                txtDefVal.Visible = false;   
            }
            else
            {
				txtDefVal.ButtonImage = new Uri(Request.Url, SkinControlsPath + "ContentDesigner/btncalendar.gif");
				txtDefVal.ChangeMonth = true;
				txtDefVal.ChangeYear = true;
            }
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
