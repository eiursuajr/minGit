namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Text;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class TextField : ValidatingTemplateBase<EktronUI.TextField>, IValidatableTemplate, ILabelable
    {
        #region Constructor

        public TextField()
        {
            this.ID = "TextField";
        }

        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            aspInput.ToolTip = this.ControlContainer.ToolTip;
            if (this.IsPostBack)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form[aspInput.UniqueID]))
                {
                    this.ControlContainer.Text = HttpContext.Current.Request.Form[aspInput.UniqueID];
                }
            }
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
                {
                    uxTextField.Attributes.Add("class", "ektron-ui-control ektron-ui-input ektron-ui-textField  " + this.ControlContainer.CssClass);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.ControlContainer.Enabled)
            {
                aspInput.CssClass = String.Empty;
                aspInput.Enabled = true;
            }
            else
            {
                aspInput.CssClass = "ui-state-disabled";
                aspInput.Enabled = false;
            }
            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        #endregion

        #region IValidatableTemplate

        public override string Text
        {
            get
            {
                return aspInput.Text
                    ?? (this.IsPostBack ? HttpContext.Current.Request.Form[aspInput.UniqueID] : string.Empty)
                    ?? string.Empty;
            }
            set { aspInput.Text = value; }
        }

        public override Control ControlToValidate
        { 
            get 
            { 
                return aspInput; 
            }
        }

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