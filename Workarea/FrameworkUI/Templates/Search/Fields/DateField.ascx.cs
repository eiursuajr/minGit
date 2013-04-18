namespace Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch
{
    using System;
    using System.Collections.Specialized;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class Date : System.Web.UI.UserControl, IBindableTemplate, IValidatableTemplate
    {
        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            Packages.Ektron.CssFrameworkBase.Register(this);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            var select = aspOperators as System.Web.UI.WebControls.ListControl;
            if (select != null && select.SelectedValue == "Between")
            {
                uxHighValue.CssClass = (uxHighValue.CssClass ?? "").Replace("ektron-ui-hidden", "").Trim();
                aspSeparatorContainer.Attributes["class"] = (aspSeparatorContainer.Attributes["class"] ?? "").Replace("ektron-ui-hidden", "").Trim();
            }
        }

        #endregion

        #region IBindableTemplate Members

        public System.Collections.Specialized.IOrderedDictionary ExtractValues(Control container)
        {
            OrderedDictionary dictionary = new OrderedDictionary();

            dictionary["SelectedOperator"] = aspOperators.SelectedValue;
            dictionary["LowValue"] = uxLowValue.Text;
            dictionary["HighValue"] = uxHighValue.Text;

            return dictionary;
        }

        #endregion

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            container.Controls.Add(this);
        }

        #endregion

        #region IValidatableTemplate
        
        public string ValidationGroup
        {
            get
            {
                return this.validationGroup;
            }
            set
            {
                uxLowValue.ValidationGroup = uxHighValue.ValidationGroup = this.validationGroup = value;
            }
        }
        private string validationGroup;

        public string Text { get; set; }

        public Control ControlToValidate
        {
            get { return null; }
        }

        #endregion
    }
}