using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Interfaces.Context;

public partial class Workarea_ContentDesigner_ekformsiframe : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		AssertInternalReferrer();
        string filepath = string.Empty;
        System.Text.StringBuilder StyleSheets = new System.Text.StringBuilder();
        StyleSheets.Append(Environment.NewLine);
		//if (Request.QueryString["skin"] != null)
		//{
		//    filepath = Request.QueryString["skin"];
		//    ValidateQueryString(filepath);
		//    StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" />");
		//}
		if (Request.QueryString["eca"] != null)
		{
            filepath = Request.QueryString["eca"];
            ValidateQueryString(filepath);
            EnsureFilePathLocal(filepath);
            Ektron.Cms.API.Css.RegisterCss(this, filepath, filepath.Replace("/",":").Replace(".",":"), true);

            //StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" />");
		}
        bool defaultCssLoaded = false;
        foreach (string p in Request.QueryString.AllKeys)
		{
			if (!String.IsNullOrEmpty(p))
			{
				if (p.StartsWith("css") && Request.QueryString[p] != null)
				{
                    filepath = Request.QueryString[p];
                    ValidateQueryString(filepath);
                    EnsureFilePathLocal(filepath);
                    //#61806: need the title attribute for the "apply style" drop down. do not need the title attribute for the internal style sheets.
                    StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" title=\"" + filepath + "\" />"); 

                    string currCssName = filepath.ToLower();
                    if (-1 == currCssName.IndexOf("ektron.workarea.css") && -1 == currCssName.IndexOf("editorcontentarea.css") && -1 == currCssName.IndexOf("editorcontentarea_rtl.css"))
                    {
                        defaultCssLoaded = true;
                    }
				}
			}
		}
        bool msOfficeStyleNeeded = true;
        if (ConfigurationManager.AppSettings["ek_EditorDefaultOfficeStyle"] != null)
        {
            msOfficeStyleNeeded = ("true" == ConfigurationManager.AppSettings["ek_EditorDefaultOfficeStyle"].ToLower()); 
        }
        if (true == msOfficeStyleNeeded && false == defaultCssLoaded)
        {
            filepath = GetCommonApi().AppPath + "csslib/editor/MsOfficeStyle.css";
            Ektron.Cms.API.Css.RegisterCss(this, filepath, filepath.Replace("/", ":").Replace(".", ":"), true);
            //StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\"></link>");
        }
		StyleSheets.AppendLine();
        cssLinks.Text = StyleSheets.ToString();
		
		string BaseUrl = Request.Url.AbsoluteUri;
		int pPath = BaseUrl.IndexOf(Request.Url.AbsolutePath);
		BaseUrl = BaseUrl.Remove(pPath + 1); // Example: http://my.domain.com/
		// create BASE as literal otherwise runat=server will use long notation with closing tag
		litBase.Text = String.Format("<base href=\"{0}\" />", BaseUrl); 
		
		if (Request.QueryString["id"] != null)
		{
			string id = Request.QueryString["id"];
            ValidateQueryString(id);
            theBody.ID = id;
		}
        if (Request.QueryString["height"] != null)
        {
            string ht = Request.QueryString["height"];
            ValidateQueryString(ht);
            theBody.Style.Add("height", ht);
            theHtmlTag.Style.Add("height", ht);
        }

		Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormIe7Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

		if (null == Request.QueryString["js"])
		{
            ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
            Package js = new Package()
            {
                Components = new System.Collections.Generic.List<Component>(){
                    Packages.EktronCoreJS,
                    Packages.Ektron.RegExp,
                    Packages.Ektron.Querystring,
                    JavaScript.Create(cmsContextService.UIPath + "/js/ektron/ektron.onexception.js"),
                    JavaScript.Create(cmsContextService.UIPath + "/js/ektron/ektron.class.js"),
                    JavaScript.Create(cmsContextService.UIPath + "/js/ektron/ektron.symantec.js"),
                    JavaScript.Create(cmsContextService.UIPath + "/js/jQuery/Plugins/ektron-coreExtensions.js")
                }
            };
            js.Register(this); //switch to new method for that call because it allows us to skip the site-data auto registration. leave others as old because packages haven't been defined for them.
            //Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronAutoheightJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJS(this, GetCommonApi().AppPath + "ContentDesigner/ekxbrowser.js", "EkXBrowser");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronSmartFormJS);
		}
	}

    private void ValidateQueryString(string queryString)
    {
        queryString = queryString.ToUpper();
        queryString = Regex.Replace(queryString, @"\/\*[\w\W]*?\*\/", "");
        if ((queryString.IndexOf("<") > -1) || (queryString.IndexOf("%3C") > -1) || (queryString.IndexOf(">") > -1) || (queryString.IndexOf("%3E") > -1) || (queryString.IndexOf("\"") > -1) || (queryString.IndexOf("%22") > -1) || (queryString.IndexOf(":EXPRESSION(") > -1) || (queryString.IndexOf("JAVASCRIPT:") > -1))
        {
            throw new ArgumentException("Invalid Query String Value");
        }
    }
    private void EnsureFilePathLocal(string FilePath)
    {
        Uri testUri=null;
        try
        {
            testUri = new Uri(FilePath);
        }catch
        {
            //do nothing.
        }
        if (testUri != null)
        {
            if (testUri.Host.ToLower() != Request.Url.Host.ToLower())
                throw new ArgumentException("File path cannot be external URL");
        }
    }
}
