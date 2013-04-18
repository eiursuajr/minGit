namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Text;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using System.Web.UI;
    using System.Collections.Generic;

    public partial class Dialog : TemplateBase<EktronUI.Dialog>
    {
        public Dialog()
        {
            this.ID = "Dialog";
        }
        protected override void OnInitialize(object sender, EventArgs e)
        {
            string selector = "#" + uxDialogControl.ClientID;
            this.ControlContainer.Selector = selector;
            if (this.ControlContainer.ContentTemplate != null)
            {
                this.ControlContainer.ContentTemplate.InstantiateIn(aspContent);
            }
        }
         
        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {           
            this.DataBind();
            //uxDialogControl.Disabled = !(this.ControlContainer.Enabled || this.ControlContainer.AutoOpen); 
            uxDialogControl.Attributes.Add("class", "ektron-ui-dialog " + this.ControlContainer.CssClass);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }
    }
}