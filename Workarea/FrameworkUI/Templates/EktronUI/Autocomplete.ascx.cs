namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Web;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;

    public partial class Autocomplete : ValidatingTemplateBase<EktronUI.Autocomplete>, IValidatableTemplate, ILabelable
    {
        //Members
        private Boolean isEnabled = true;

        //Contstructor
        public Autocomplete()
        {
            this.ID = "Autocomplete";
        }

        //Properties
        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user interaction. Default Value is true.
        /// </summary>
        public  Boolean Enabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                uxAutocompleteTextBox.Enabled = value;
            }
        }

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form[uxAutocompleteTextBox.UniqueID]))
                {
                    this.ControlContainer.Text = HttpContext.Current.Request.Form[uxAutocompleteTextBox.UniqueID];
                }
            }
            this.ControlContainer.Selector = "#" + this.ControlContainer.ClientID + " .key input";
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            uxAutocompleteTextBox.Text = this.ControlContainer.Text;
            uxAutocompleteTextBox.Enabled = this.ControlContainer.Enabled;
        }

        #endregion

        #region IValidatableControl

        public override string ValidationGroup
        {
            get
            {
                return uxAutocompleteTextBox.ValidationGroup;
            }
            set
            {
                uxAutocompleteTextBox.ValidationGroup = value;
            }
        }

        public override string Text
        {
            get
            {
                return uxAutocompleteTextBox.Text
                    ?? (this.IsPostBack ? HttpContext.Current.Request.Form[uxAutocompleteTextBox.UniqueID] : string.Empty)
                    ?? string.Empty;
            }
            set
            {
                uxAutocompleteTextBox.Text = value;
            }
        }

        public override Control ControlToValidate
        {
            get
            {
                return uxAutocompleteTextBox.ControlToValidate as Control;
            }
        }
        
        #endregion

        #region ILabelable

        public override string LabelableControlID
        {
            get
            {
                return uxAutocompleteTextBox.LabelableControlID;
            }
        }

        #endregion
    }
}