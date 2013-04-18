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
    using Ektron.Cms.Interfaces.Context;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class Button : TemplateBase<EktronUI.Button>
    {
        #region Members

        #endregion

        #region Constructor

        public Button()
        {
            this.ID = "Button";
        }

        #endregion

        #region Properties

        private string CssClass
        {
            get
            {
                string containerClass = " class=\"ektron-ui-control ektron-ui-button";
                if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
                {
                    containerClass += " " + this.ControlContainer.CssClass;
                }
                containerClass += "\"";
                return containerClass;
            }
        }
        public string RadioButtonID
        {
            get
            {
                return this.ControlContainer.ClientID + "_RadioButton";
            }
        }

        public string RadioButtonChecked
        { get { return ButtonChecked ? "checked=\"checked\"" : String.Empty; } }

        public bool ButtonChecked
        {
            get
            {
                bool isChecked = false;
                if (this.Page.IsPostBack)
                {
                    Boolean.TryParse(aspRadioButtonValue.Value, out isChecked);
                }
                else
                {
                    aspRadioButtonValue.Value = this.ControlContainer.Checked.ToString().ToLower();
                    isChecked = this.ControlContainer.Checked;
                }
                return isChecked;
            }
        }

        public string RadioButtonText { get; set; }

        public string RadioButtonOnClickValue
        {
            get
            {
                string attribute = String.Empty;
                string radioButtonValue = "Ektron.Controls.EktronUI.Button.RadioButton.click(this);";
                attribute = "onclick=\"" + radioButtonValue + this.ClientClick + "\"";
                return attribute;
            }
        }

        public string ButtonID
        {
            get
            {
                return this.ControlContainer.ClientID + "_Button";
            }
        }

        public string ClientClick
        {
            get
            {
                string onclick = String.Empty;
                if (!String.IsNullOrEmpty(this.ControlContainer.OnClientClick))
                {
                    onclick += this.ControlContainer.OnClientClick.TrimEnd(new char[] { ';' }) + ";";
                }
                if (!String.IsNullOrEmpty(this.ControlContainer.PostBackTrigger))
                {
                    if (this.ControlContainer.DisplayMode == EktronUI.Button.MarkupMode.Anchor)
                    {
                        onclick += "if ($ektron(this).is('.ui-state-disabled, .ui-state-disabled > span')){ return false;};";
                    }
                    onclick += this.ControlContainer.PostBackTrigger.TrimEnd(new char[] { ';' }) + ";";
                    if (this.ControlContainer.DisplayMode == EktronUI.Button.MarkupMode.Button)
                    {
                        // in IE8, without a return false, the form will be submitted twice.  
                        onclick += "return false;";
                    }
                }
                return onclick.Replace("\"", "'");
            }
        }

        public string RadioButtonName { get; set; }
        public bool Checked { get; set; }
        public bool Value { get; set; }

        #endregion

        #region Events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            this.ControlContainer.Selector = this.ButtonID;
            switch (this.ControlContainer.DisplayMode)
            {
                case EktronUI.Button.MarkupMode.Anchor:
                    aspButtonMarkupMode.SetActiveView(aspAnchorMarkup);
                    aspAnchorMarkup.DataBind();
                    break;
                case EktronUI.Button.MarkupMode.Button:
                    aspButtonMarkupMode.SetActiveView(aspButtonMarkup);
                    aspButtonMarkup.DataBind();
                    break;
                case EktronUI.Button.MarkupMode.Submit:
                    aspButtonMarkupMode.SetActiveView(aspSubmitMarkup);
                    aspSubmitMarkup.DataBind();
                    break;
                case EktronUI.Button.MarkupMode.Checkbox:
                    aspButtonMarkupMode.SetActiveView(aspCheckboxMarkup);
                    aspCheckboxLabel.Text = this.ControlContainer.Text;
                    this.ControlContainer.Selector = aspCheckbox.ClientID;
                    this.ControlContainer.CheckableControlClientID = aspCheckbox.ClientID;
                    this.ControlContainer.CheckableControlUniqueID = aspCheckbox.UniqueID;
                    aspCheckbox.Checked = this.ButtonChecked;
                    if (!String.IsNullOrEmpty(this.ClientClick))
                    {
                        aspCheckbox.Attributes.Add("onclick", this.ClientClick);
                    }
                    if (IsFireFox)
                    {
                        aspCheckbox.Attributes.Add("autocomplete", "off");
                    }
                    break;
                case EktronUI.Button.MarkupMode.RadioButton:
                    aspButtonMarkupMode.SetActiveView(aspRadioButtonMarkup);
                    this.RadioButtonText = this.ControlContainer.Text;
                    this.RadioButtonName = this.ControlContainer.Parent.ClientID + "_RadioButton";
                    this.ControlContainer.Selector = this.RadioButtonID;
                    this.ControlContainer.CheckableControlClientID = aspRadioButtonValue.ClientID;
                    this.ControlContainer.CheckableControlUniqueID = aspRadioButtonValue.UniqueID;
                    break;
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
                this.DataBind();
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.ControlContainer.IsButtonSet)
            {
                // checkboxes get the ControlContainer's CSS class applied directly
                aspCheckbox.CssClass = this.CssClass;
                base.Render(writer);
            }
            else
            {
                writer.Write("<span id=\"" + this.ControlContainer.ClientID + "\"" + this.CssClass + ">");
                base.Render(writer);
                writer.Write("</span>");
            }
        }

        #endregion

        #region Helpers

        public string GetCssClass()
        {
            if (!this.ControlContainer.IsButtonSet)
            {
                return String.Empty;
            }
            else
            {
                return this.CssClass;
            }
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }
            return builder.ToString();
        }

        public string AutoComplete()
        {
            if (IsFireFox)
            {
                return "autocomplete='off'";
            }
            return string.Empty;
        }

        private bool IsFireFox { get { return Request.Browser.Browser.ToLower().Contains("firefox"); } }

        protected string HrefHelper()
        {
            return String.IsNullOrEmpty(this.ControlContainer.Text) ? String.Empty : this.ControlContainer.Text.Replace(" ", String.Empty);
        }

        #endregion
    }
}