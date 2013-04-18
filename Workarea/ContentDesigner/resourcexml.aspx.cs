using System;
public partial class ContentDesigner_resourcexml : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    private Ektron.Cms.CommonApi _api = null;

	protected void Page_PreInit(object sender, EventArgs e)
	{
		Page.Theme = ""; // EnableTheming="false" in Page directive has no effect.
	}
	protected void Page_Load(object sender, EventArgs e)
    {
		AssertInternalReferrer();
		//<data name="id" xml:space="preserve">
		//    <value>text</value>
		//</data>

		string strResourceName = Request.QueryString.Get("name");
		if (strResourceName != null)
		{
			string strResourceKey = Request.QueryString.Get("id");
			string strResourcesPath = Server.MapPath("Resources/");
			string xsltFile = Server.MapPath("resxdata.xslt");
			string strLang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string xmlFile = strResourcesPath + strResourceName + "." + strLang + ".resx";
			if (!System.IO.File.Exists(xmlFile))
			{
				strLang = System.Threading.Thread.CurrentThread.CurrentUICulture.Parent.Name;
				xmlFile = strResourcesPath + strResourceName + "." + strLang + ".resx";
				if (!System.IO.File.Exists(xmlFile))
				{
					xmlFile = strResourcesPath + strResourceName + ".resx";
				}
			}
			Ektron.Cms.Xslt.ArgumentList args = null;
			if (!String.IsNullOrEmpty(strResourceKey))
			{
				args = new Ektron.Cms.Xslt.ArgumentList();
				args.AddParam("resourceKey", "", strResourceKey);
			}
            _api = new Ektron.Cms.CommonApi();
            string strXml = Ektron.Cms.EkXml.XSLTransform(xmlFile, xsltFile, true, true, args, true, null, _api.RequestInformationRef.ApplicationPath, false);
			litOutput.Text = strXml;
		}
    }
}
