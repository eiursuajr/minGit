using System;
using Ektron.Cms.Framework.UI;
using System.Collections.Generic;

public partial class Workarea_diagnostics_EktronDiag : System.Web.UI.Page
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