namespace Ektron.Cms.Framework.UI.Controls.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core.Validation;
    using Ektron.Cms.Framework.UI.Views;
    using Ektron.Cms.Interfaces.Context;

    public partial class XmlSearchInputView : BaseTemplate<ISearchView, XmlSearchController>
    {
        public string ValidationGroup 
        { 
            get
            {
                return "urn:Ektron.ValidationGroup." + this.ClientID;
            }
        }

        protected void Page_Init(object sender, EventArgs e) 
        {
            uxXmlSearchButton.ValidationGroup = this.ValidationGroup;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Visible)
            {
                ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
                // create a package that will register the UI JS and CSS we need
                Package xmlSearchControlPackage = new Package() 
                {
                    Components = new List<Component>()
                    {
                        // Register JS Files
                        Packages.EktronCoreJS,
                        UI.JavaScript.Create(cmsContextService.UIPath + "/js/Ektron/Controls/Ektron.Controls.Search.XmlSearch.js"),
                        Packages.jQuery.Plugins.BindReturnKey,
                        // Register CSS Files
                        Packages.Ektron.CssFrameworkBase,
                        Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search-xml.css")
                    }
                };
                xmlSearchControlPackage.Register(this);
            }
        }

        protected void aspFields_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            XmlSearchFieldView container = e.Item.FindControl("uxField") as XmlSearchFieldView;
            IValidatableTemplate field = container.GetTemplateInstance<IValidatableTemplate>();
            if (field != null)
            {
                field.ValidationGroup = this.ValidationGroup;
            }
        }

        protected void uxXmlSearch_Click(object sender, EventArgs e)
        {
            List<XmlField> fields = new List<XmlField>();
            foreach (ListViewDataItem item in this.aspFields.Items)
            {
                XmlSearchFieldView field = (XmlSearchFieldView)item.FindControl("uxField");
                fields.Add(field.Field);
            }

            //validate form fields
            if (!String.IsNullOrEmpty(ValidationGroup))
            {
                Page.Validate(ValidationGroup);
                if (Page.IsValid)
                {
                    this.Controller.Search(
                        ((Ektron.Cms.Framework.UI.Controls.XmlSearchInputView)NamingContainer.NamingContainer).XmlConfigId,
                        fields
                    );
                }
            }
        }

    }
}