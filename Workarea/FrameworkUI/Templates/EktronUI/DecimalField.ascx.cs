namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using System.Web;
    using System.Globalization;

    public partial class DecimalField : ValidatingTemplateBase<EktronUI.DecimalField>, IValidatableTemplate, ILabelable
    {
        protected string currentCulture;

        #region Constructor

        public DecimalField()
        {
            this.ID = "DecimalField";
        }

        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            aspInput.ToolTip = this.ControlContainer.ToolTip;
            if (String.IsNullOrEmpty(this.Text))
            {
                this.Text = "0.0";
            }
            if (this.IsPostBack && !string.IsNullOrEmpty(HttpContext.Current.Request.Form[aspInput.UniqueID]))
            {
                this.ControlContainer.Text = HttpContext.Current.Request.Form[aspInput.UniqueID];
            }
            else
            {
                this.ControlContainer.Text = aspInput.Text;
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
                decimal result = 0.0M;
                IFormatProvider provider = CultureInfo.CurrentCulture;
                if (!string.IsNullOrEmpty(value) && decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowThousands, provider, out result))
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