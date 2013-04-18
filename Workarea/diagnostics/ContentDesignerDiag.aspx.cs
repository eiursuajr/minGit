using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Framework.UI;

public partial class Workarea_diagnostics_ContentDesignerDiag : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResources();
    }

    protected void RegisterResources()
    {
        Package diagnosticsResources = new Package()
        {
            Components = new List<Component>
            {
                Packages.Ektron.StringObject,
                Packages.jQuery.Plugins.Cookie    
            }
        };
        diagnosticsResources.Register(this);
    }
}