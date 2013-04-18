using System;
using Ektron.Cms.Framework.UI;

public partial class Security_SampleData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResources();
    }

    protected void RegisterResources()
    {
        Packages.EktronCoreJS.Register(this);
    }
}