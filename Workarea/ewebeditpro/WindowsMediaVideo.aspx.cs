using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.Workarea.Framework;


public partial class Workarea_ewebeditpro_WindowsMediaVideo : WorkareaDialogPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.RegisterWorkareaCssLink();
		this.RegisterDialogCssLink();
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
		//Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
		//Ektron.Cms.API.JS.RegisterJS(this, "../ekxbrowser.js", "ekxbrowserJS");
		//Ektron.Cms.API.JS.RegisterJS(this, "../ekutil.js", "ekutilJS");

        sEditorName.Text = "";
        if (Request.QueryString["EditorName"] != null) 
        {
            sEditorName.Text = Request.QueryString["EditorName"];
        }
        
	}
}
