namespace Ektron.Cms.Framework.UI.Controls.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI.Views;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using Ektron.Cms.Interfaces.Context;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;

    public partial class SiteSearchInputView : BaseTemplate<ISearchView, SiteSearchController>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Visible)
            {
                ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
                aspAdvancedSearchIcon.HRef = aspAdvancedSearchLink.HRef = cmsContextService.WorkareaPath + "/JavascriptRequired.aspx";
                aspAdvancedSearchLink.InnerText = GetLocalResourceObject("ToggleButtonText") as string; // meta:resourcekey="uxAdvancedSearchLinkResource1" 

                // create a package that will register the UI JS and CSS we need
                Package searchControlPackage = new Package() {
                    Components = new List<Component>()
                {
                    // Register JS Files
                    Packages.EktronCoreJS,
                    Packages.jQuery.jQueryUI.Position,
                    Packages.jQuery.Plugins.BGIframe,
                    Packages.jQuery.Plugins.ToTop,
                    UI.JavaScript.Create(cmsContextService.UIPath + "/js/Ektron/Controls/Ektron.Controls.Search.SiteSearch.js"),
                    Packages.jQuery.Plugins.BindReturnKey,
                    // Register CSS Files
                    Packages.Ektron.CssFrameworkBase,
                    UI.Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search.css")
                }
                };
                searchControlPackage.Register(this);
            }
        }

        protected void uxBasicSearch_Click(object sender, EventArgs e)
        {
            this.Controller.Search(uxSearchText.Text);
        }

        protected void uxAdvancedSearch_Click(object sender, EventArgs e)
        {
            this.Controller.Search(uxWithAllWords.Text, uxWithoutWords.Text, uxAnyWords.Text, uxExactPhrase.Text);
        }
    }
}