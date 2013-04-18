namespace Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch
{
    using System;
    using System.Collections.Specialized;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;

    public partial class BooleanField : System.Web.UI.UserControl, IBindableTemplate, IValidatableTemplate
    {
        #region IBindableTemplate Members

        public System.Collections.Specialized.IOrderedDictionary ExtractValues(Control container)
        {
            OrderedDictionary dictionary = new OrderedDictionary();

            dictionary["SelectedOperator"] = aspListChoices.SelectedValue;

            return dictionary;
        }

        #endregion

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            container.Controls.Add(this);
        }

        #endregion

        public string ValidationGroup
        {
            get
            {
                return aspListChoices.ValidationGroup;
            }
            set
            {
                aspListChoices.ValidationGroup = value;
            }
        }

        public string Text { get; set; }

        public Control ControlToValidate
        {
            get { return null; }
        }
    }
}