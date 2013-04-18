namespace Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch
{
    using System.Collections.Specialized;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class TextField : System.Web.UI.UserControl, IBindableTemplate, IValidatableTemplate
    {
        #region IBindableTemplate Members

        public System.Collections.Specialized.IOrderedDictionary ExtractValues(Control container)
        {
            OrderedDictionary dictionary = new OrderedDictionary();

            dictionary["Value"] = uxInputText.Text;
            dictionary["SelectedOperator"] = aspOperators.SelectedValue;

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
                uxInputText.ValidationGroup = this.validationGroup = value;
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