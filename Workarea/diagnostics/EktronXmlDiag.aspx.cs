using System;
using System.Collections.Generic;
using Ektron.Cms.Framework.UI;

public partial class Workarea_diagnostics_EktronXmlDiag : System.Web.UI.Page
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