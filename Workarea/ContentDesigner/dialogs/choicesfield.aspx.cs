using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.API;
using Ektron.Cms.Workarea.Framework;
using Ektron.Cms;
using Ektron.Cms.Common;
namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for ChoicesField.
    /// </summary>
    public partial class ChoicesField : WorkareaDialogPage
    {
		private Ektron.Cms.CommonApi _api = null;
        protected EkMessageHelper _MessageHelper;

		protected string DataListSpecXml = "../../ContentDesigner/DataListSpec.xml";

        private void Page_Load(object sender, System.EventArgs e)
        {
            Utilities.AddSessionIdField(this.Form);
			_api = new Ektron.Cms.CommonApi();
            _MessageHelper = _api.EkMsgRef;
            if (Request.RawUrl.ToLower().Contains("<script"))
            {
                Utilities.ShowError(_MessageHelper.GetMessage("invalid querstring"));
                return;
            }
			this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();

            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormIe7Css, Css.BrowserTarget.LessThanEqualToIE7);

			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronSmartFormJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS"); // include after smartForm.js
			Ektron.Cms.API.JS.RegisterJSInclude(this, "choicesfield.js", "choicesfieldJS");
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

			string sSelected = this.GetMessage("lbl selected");
			string sText = this.GetMessage("lbl text nc");
			string sCannotBeBlank = this.GetMessage("lbl cannot be blank");
			string sValidation = @"design_validate_re(/\S+/,this,'" + sCannotBeBlank + @"');";
			string sValue = this.GetMessage("lbl value");
			string sDisabled = this.GetMessage("lbl disabled");

			this.Title.Text = this.GetMessage("lbl choices field");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("tab general") + " Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab validation");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("tab validation") + " Tab";
			this.RadTabStrip1.Tabs[2].Text = this.GetMessage("tab data style");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("tab data style") + " Tab";
			this.RadTabStrip1.Tabs[3].Text = this.GetMessage("tab advanced");
            this.RadTabStrip1.Tabs[3].ToolTip = this.GetMessage("tab advanced") + " Tab";

			this.lblList.InnerHtml = this.GetMessage("lbl list c");
			this.lblList.Attributes["for"] = this.cboList.ClientID;
			this.lblAllowSelection.InnerHtml = this.GetMessage("lbl allow selection");
			this.lblOnlyOne.InnerHtml = this.GetMessage("lbl only one");
			this.lblMoreThanOne.InnerHtml = this.GetMessage("lbl more than one");
			this.lblSelectionReqd.InnerHtml = this.GetMessage("lbl selection reqd");
			this.lblFirstNotValid.InnerHtml = this.GetMessage("lbl first not valid");
			this.lblAppearance.InnerHtml = this.GetMessage("lbl appearance");
			this.lblVertList.InnerHtml = this.GetMessage("lbl vert list");
			this.lblHorzList.InnerHtml = this.GetMessage("lbl horz list");
			this.lblListBox.InnerHtml = this.GetMessage("lbl list box");
			this.lblDropList.InnerHtml = this.GetMessage("lbl drop list");
			this.lblItemList.InnerHtml = this.GetMessage("lbl item list");
			this.lblSelected.InnerHtml = sSelected;
			this.lblDisplayText.InnerHtml = this.GetMessage("lbl display text");
			this.lblValue.InnerHtml = sValue;
			this.lblDisabled.InnerHtml = sDisabled;
			this.lblOption.Text = this.GetMessage("lbl option");
			this.lblOption2.Text = this.lblOption.Text;

			this.selected.Attributes["title"] = sSelected;
			this.Text.Attributes["title"] = sText;
			this.Text.Attributes["alt"] = sText;
			this.Text.Attributes["onblur"] = sValidation;
			this.Text.Attributes["ektdesignns_invalidmsg"] = sCannotBeBlank;
			this.value.Attributes["title"] = sValue;
			this.value.Attributes["alt"] = sValue;
			this.disabled.Attributes["title"] = sDisabled;

			this.selected1.Attributes["title"] = sSelected;
			this.Text1.Attributes["title"] = sText;
			this.Text1.Attributes["alt"] = sText;
			this.Text1.Attributes["onblur"] = sValidation;
			this.Text1.Attributes["ektdesignns_invalidmsg"] = sCannotBeBlank;
			this.value1.Attributes["title"] = sValue;
			this.value1.Attributes["alt"] = sValue;
			this.disabled1.Attributes["title"] = sDisabled;

			this.sOptionsReqd.Text = this.GetMessage("msg options reqd");
			this.sFirstNotValid.Text = this.GetMessage("msg first not valid");
			this.sCannotBeBlank.Text = sCannotBeBlank;
            this.sFirstRowText.Text = this.GetMessage("lbl first display text on choices option");

            if (!Page.IsPostBack)
            {
                BindComboBox();
            }
        }

        private void BindComboBox()
        {
			// Need fully qualified URL so XSLT can get dynamic (i.e., .aspx) localeUrl
			string xmlPath = new Uri(Request.Url, DataListSpecXml).AbsoluteUri; 
			string xsltPath = new Uri(Request.Url, "../../ContentDesigner/DataListSpecToDataListCombo.xslt").AbsoluteUri;

			Ektron.Cms.Xslt.ArgumentList objXsltArgs = new Ektron.Cms.Xslt.ArgumentList();
			objXsltArgs.AddParam("LangType", "", _api.UserLanguage);
            string comboItems = Ektron.Cms.EkXml.XSLTransform(xmlPath, xsltPath, true, true, objXsltArgs, false, null, _api.RequestInformationRef.ApplicationPath, false);

            cboList.LoadXmlString(comboItems);
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
