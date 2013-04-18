namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using Ektron.Cms.Interfaces.Context;
 
    public partial class DateField : ValidatingTemplateBase<EktronUI.DateField>, IValidatableTemplate, ILabelable
    {
        #region Constructor

        public DateField()
        {
            this.ID = "DateField";
        }

        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            aspInput.ToolTip = this.ControlContainer.ToolTip;
            if (this.IsPostBack && !string.IsNullOrEmpty(HttpContext.Current.Request.Form[aspInput.UniqueID]))
            {
                this.ControlContainer.Text = HttpContext.Current.Request.Form[aspInput.UniqueID];
            }
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            // register any required Javascript or CSS resources here:
            Package resources = new Package()
            {
                Components = new List<Component>()
                {
                    this.ControlContainer.RegisterCulture(this.ControlContainer.OverrideDefaultCulture),
                    Packages.jQuery.Plugins.Alphanumeric,
                    Packages.jQuery.Plugins.InfieldLabels,
                    Ektron.Cms.Framework.UI.Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-dateField.css")
                }                
            };
            resources.Register(this);
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            string css = "ektron-ui-control ektron-ui-input ektron-ui-dateField";
            if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
            {
                css += " " + this.ControlContainer.CssClass;
            }
            uxDateFieldWrapper.CssClass = css.ToString();
            //set the text of the label to the culture's short date format
            aspInputLabel.Text = this.ControlContainer.OverrideDefaultCulture.DateTimeFormat.ShortDatePattern.ToLower();
        }
       
        #endregion

        #region IValidatableTemplate

        public override string ValidationGroup
        {
            get
            {
                return aspInput.ValidationGroup;
            }
            set
            {
                aspInput.ValidationGroup = value;
            }
        }

        public override string Text
        {
            get
            {
                return aspInput.Text;
            }
            set
            {
                aspInput.Text = value;
            }
        }

        public override Control ControlToValidate
        {
            get 
            { 
                return aspInput;
            }
        }

        #endregion

        #region ILabelable

        public override string LabelableControlID
        {
            get
            {
                return aspInput.ClientID;
            }
        }

        #endregion
    }
}