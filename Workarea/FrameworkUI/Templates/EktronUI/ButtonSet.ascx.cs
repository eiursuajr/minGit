namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Text;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using System.Web.UI;
    using Ektron.Cms.Interfaces.Context;

    public partial class ButtonSet : TemplateBase<EktronUI.ButtonSet>
    {
        public ButtonSet()
        {
            this.ID = "ButtonSet";
        }
        
        protected override void OnInitialize(object sender, EventArgs e)
        {
            this.ControlContainer.CssClass = "ektron-ui-buttonSet";
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            Ektron.Cms.Framework.UI.JavaScript.Register(this, cmsContextService.UIPath + "/js/Ektron/Controls/EktronUI/Ektron.Controls.EktronUI.ButtonSet.js", false);
        }
    }
}