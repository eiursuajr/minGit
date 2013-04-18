namespace Ektron.Cms.Framework.UI.Controls.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Views;
    using Ektron.Cms.Interfaces.Context;

    public partial class UserSearchResults : BaseTemplate<IUserSearchView, UserSearchController>
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

                // create a package that will register the UI JS and CSS we need
                Package userSearchResultsControlPackage = new Package() 
                {
                    Components = new List<Component>()
                    {
                        // Register JS Files
                        Packages.EktronCoreJS,
                        // Register CSS Files
                        Packages.Ektron.CssFrameworkBase,
                        Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search-results.css")
                    }
                };
                userSearchResultsControlPackage.Register(this);
            }
        }

        protected void uxTagLink_Command(object sender, CommandEventArgs e)
        {
            string tag = e.CommandArgument.ToString();
            this.Controller.BasicSearch(String.Format("\"{0}\"", tag.Replace("\"", ""))); 
        }
    }
}