namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Interfaces.Context;

    public partial class IntegerField : ValidatingTemplateBase<EktronUI.IntegerField>, IValidatableTemplate
    {
        protected string currentCulture;

        #region Constructor
        public IntegerField()
        {
            this.ID = "IntegerField";
        }
        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            aspInput.ToolTip = this.ControlContainer.ToolTip;
            if (String.IsNullOrEmpty(this.Text))
            {
                this.Text = "0";
            }
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
            Package resources = new Package()
            {
                Components = new List<Component>()
                {
                    // JS
                    Packages.jQuery.jQueryUI.Spinner,
                    
                    // load CSS
                    Packages.Ektron.CssFrameworkBase
                }
            };

            resources.Register(this);
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
            {
                aspInput.Attributes.Add("class", this.ControlContainer.CssClass);
            }
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
                Int32 result = 0;
                IFormatProvider provider = CultureInfo.CurrentCulture;
                if (!string.IsNullOrEmpty(value) && Int32.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out result))
                {
                    aspInput.Text = result.ToString();
                    if (this.ControlContainer.Value != result)
                    {
                        this.ControlContainer.Value = result;
                    }
                }
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