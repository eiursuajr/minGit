using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Workarea_printreport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Ektron.Cms.Framework.UI.Packages.Ektron.Workarea.Core.Register(this);
    }
}