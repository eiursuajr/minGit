namespace Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch
{
    using System.Collections.Specialized;
    using System.Web.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using System.Web.UI.WebControls;
    using System;

    public partial class ChoiceField : System.Web.UI.UserControl, IBindableTemplate, IValidatableTemplate
    {
        #region IBindableTemplate Members

        public System.Collections.Specialized.IOrderedDictionary ExtractValues(Control container)
        {
            OrderedDictionary dictionary = new OrderedDictionary();

            if (aspListChoices.SelectionMode == System.Web.UI.WebControls.ListSelectionMode.Single) {
            	dictionary["SelectedValue"] = aspListChoices.SelectedValue;
            } 
            else {
                string selection = "";
                foreach (ListItem item in aspListChoices.Items) {
                    if (item.Selected) {
                        if (selection.Length > 0) {
                            selection += ",";
                        }
                        selection += item.Value;
                    }
                }
                dictionary["SelectedValue"] = selection;
            }
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

        #endregion
        protected void aspListChoices_DataBound(object sender, EventArgs e)
        {
            
            Ektron.Cms.Framework.UI.ChoiceField field = DataBinder.GetDataItem(this.NamingContainer) as Ektron.Cms.Framework.UI.ChoiceField;
            ListBox aspListChoices = sender as ListBox;

            if (field != null && aspListChoices != null)
            {
                string[] selectedValues = field.SelectedValue.Split(new char[] { ',' });
                Array.ForEach<string>(selectedValues, selectedValue => aspListChoices.Items.FindByValue(selectedValue).Selected = true);
                if (aspListChoices.SelectionMode == ListSelectionMode.Multiple)
                { 
                    aspListChoices.Rows = aspListChoices.Items.Count;
                }
            }
        }
}
}