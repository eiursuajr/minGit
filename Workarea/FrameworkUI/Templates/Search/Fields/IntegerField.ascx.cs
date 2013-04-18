namespace Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch
{
    using System.Collections.Specialized;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class IntegerField : System.Web.UI.UserControl, IBindableTemplate, IValidatableTemplate
    {
        private string validationGroup;

        #region events

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
            dictionary["LowValue"] = uxLowValue.Value.ToString();
            dictionary["HighValue"] = uxHighValue.Value.ToString();

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

        public string Text { get; set; }

        public Control ControlToValidate
        {
            get { return null; }
        }

        #endregion

        
    }
}