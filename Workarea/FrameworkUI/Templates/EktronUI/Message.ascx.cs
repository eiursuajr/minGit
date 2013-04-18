namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Interfaces.Context;

    public partial class Message : TemplateBase<EktronUI.Message>
    {
        #region members, enums and constants
        private List<string> Classes { get; set; }
        private List<string> IconClasses { get; set; }
        #endregion

        #region constructor
        public Message()
        {
            this.ID = "Message";
            this.Classes = new List<string>();
            this.IconClasses = new List<string>();
        }
        #endregion

        #region events

        protected override void OnInitialize(object sender, EventArgs e)
        {
            //do nothing
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();    
            // register any required Javascript or CSS resources here:
            Package interactionPackage = new Package()
            {
                Components = new List<Component>()
                {
                    Packages.jQuery.jQueryUI.ThemeRoller,
                    Packages.Ektron.CssFrameworkBase,
                    Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-message.css")
                }
            };
            interactionPackage.Register(this);
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // control classes
                this.Classes.Add("ektron-ui-control");
                this.Classes.Add("ui-corner-all");
                this.Classes.Add("ektron-ui-message");
                this.Classes.Add(this.ControlContainer.ControlClass);
                if (!String.IsNullOrEmpty(this.ControlContainer.CssClass))
                {
                    this.Classes.Add(this.ControlContainer.CssClass);
                }
                this.Classes.Add("ektron-ui-clearfix");
                uxMessage.Attributes.Add("class", String.Join(" ", this.Classes.ToArray()));

                // icon classes
                this.IconClasses.Add("ui-icon");
                this.IconClasses.Add("ektron-ui-sprite");
                this.IconClasses.Add(this.ControlContainer.IconClass);
                aspIcon.CssClass = String.Join(" ", this.IconClasses.ToArray());

                aspContentTemplateControl.DataBind();

                if (!String.IsNullOrEmpty(this.ControlContainer.ToolTip))
                {
                    uxMessage.Attributes.Add("title", this.ControlContainer.ToolTip);
                }

                if (this.ControlContainer.ContentTemplate != null)
                {
                    this.ControlContainer.ContentTemplate.InstantiateIn(aspContentTemplateControl);
                }
            }
        }
        #endregion
    }
}