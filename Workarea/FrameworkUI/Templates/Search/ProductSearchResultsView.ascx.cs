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
    using Ektron.Cms.Framework.UI.Controls;

    public partial class ProductSearchResultsView : BaseTemplate<IProductSearchView, IProductSearchController>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Visible)
            {
                Packages.Ektron.CssFrameworkBase.Register(this);
            }
        }
    }
}