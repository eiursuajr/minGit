namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;

    public partial class Datepicker : ValidatingTemplateBase<EktronUI.Datepicker>, IValidatableTemplate, ILabelable, IErrorMessage
    {
        #region members
        private string selector;
        private string wrapperCssClass;
        #endregion

        #region constructor

        public Datepicker()
        {
            this.ID = "Datepicker";
        }

        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            switch (this.ControlContainer.DisplayMode)
            {
                case EktronUI.Datepicker.MarkupMode.Default:
                    uxDateField.Visible = true;
                    uxInline.Visible = false;
                    selector = uxDateField.ControlToValidate.ClientID;
                    break;
                case EktronUI.Datepicker.MarkupMode.Inline:
                    uxDateField.Visible = true;
                    uxDateField.CssClass = "ektron-ui-hiddenOffScreen";
                    uxInline.Visible = true;
                    uxDatepicker.TagName = "div";
                    selector = uxInline.ClientID;
                    break;
            }

            //get datepicker javascript initializer
            this.ControlContainer.Selector = "#" + selector;

            uxDateField.ErrorMessage = ControlContainer.ErrorMessage;
            ControlContainer.CultureChangedHandler = this.CultureChangedHandler;
        }

        protected override void OnLoad(EventArgs e)
        {
            // need to set culture before validation occurs, but after page-init, to 
            // give customer an opportunity to set programatically (during PageInit):
            uxDateField.OverrideDefaultCulture = ControlContainer.OverrideDefaultCulture;
            base.OnLoad(e);
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            //set css class if necessary
            wrapperCssClass = "ektron-ui-control ektron-ui-datepicker ektron-ui-inline";
            if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
            {
                uxDatepicker.Attributes.Add("class", wrapperCssClass + " " + this.ControlContainer.CssClass);
            }
            else
            {
                uxDatepicker.Attributes.Add("class", wrapperCssClass);
            }
            uxDateField.Enabled = ControlContainer.Enabled;
        }

        #endregion

        #region IValidatableTemplate

        public override string ValidationGroup
        {
            get
            {
                return uxDateField.ValidationGroup;
            }
            set
            {
                uxDateField.ValidationGroup = value;
            }
        }

        public override string Text
        {
            get
            {
                return uxDateField.Value == DateTime.MinValue ? String.Empty : uxDateField.Value.ToString("d", uxDateField.OverrideDefaultCulture);
            }
            set
            {
                uxDateField.Text = value;
            }
        }

        public override Control ControlToValidate
        {
            get
            {
                return uxDateField.ControlToValidate as Control;
            }
        }

        #endregion

        #region helpers
        protected void CultureChangedHandler(CultureInfo cultureInfo)
        {
            uxDateField.OverrideDefaultCulture = cultureInfo;
        }
        #endregion

        #region ILabelable

        public override string LabelableControlID
        {
           get
            {
                return uxDateField.LabelableControlID;
            }
        }

        #endregion

        #region IErrorMessage Members

        public string ErrorMessage
        {
            get
            {
                return uxDateField.ErrorMessage;
            }
            set
            {
                uxDateField.ErrorMessage = value;
            }
        }

        #endregion
    }
}