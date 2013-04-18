<%@ WebHandler Language="C#" Class="ajaxscript" %>

using System;
using System.Web;
using System.IO;
using System.Text;

public class ajaxscript : Ektron.Cms.Workarea.Framework.WorkareaBaseHttpHandler 
{   
    public override void ProcessRequest(HttpContext context) 
	{
		base.ProcessRequest(context);

		string filename = "";
		StringBuilder sbText = new StringBuilder();

        //hack for safari v3 with asp.net 2.0; see: http://forums.asp.net/t/1252014.aspx
		sbText.AppendLine("if (typeof Sys !== \"undefined\" && Sys.Browser) {");
		sbText.AppendLine("    if (\"undefined\" === typeof Sys.Browser.WebKit)");
		sbText.AppendLine("    {");
		sbText.AppendLine("        Sys.Browser.WebKit = {};");
		sbText.AppendLine("    }");
		sbText.AppendLine("    if (navigator.userAgent.indexOf( 'WebKit/' ) > -1)");
		sbText.AppendLine("    {");
		sbText.AppendLine("        if (Sys.Browser.agent !== Sys.Browser.WebKit)");
		sbText.AppendLine("        {");
        sbText.AppendLine("  			 Sys.Browser.agent = Sys.Browser.WebKit;");
        sbText.AppendLine("  			 Sys.Browser.version = parseFloat( navigator.userAgent.match(/WebKit\\/(\\d+(\\.\\d+)?)/)[1]);");
        sbText.AppendLine("  			 Sys.Browser.name = 'WebKit';");
		sbText.AppendLine("        }");
		sbText.AppendLine("    }");
		sbText.AppendLine("}");
        
		try
		{
			string path = context.Request.QueryString["path"];
			if (null == path || 0 == path.Length)
			{
				throw new ArgumentNullException("path", "'path' argument is required.");
			}
			string filepath = context.Server.MapPath(path);
			filename = new FileInfo(filepath).Name;
			if (!filename.EndsWith(".js"))
			{
				throw new ArgumentException("'path' must reference a JavaScript (.js) file.", "path");
			}
			sbText.Append(File.ReadAllText(filepath, Encoding.Default));
			sbText.AppendLine();
		}
		catch (Exception ex)
		{
			filename = "error.js";
			sbText.AppendLine("/* " + ex.Message + " */");
		}
		sbText.AppendLine("if(typeof(Sys) !== \"undefined\") Sys.Application.notifyScriptLoaded();");
		
		string text = sbText.ToString();

		context.Response.ContentEncoding = Encoding.Default;
		context.Response.ContentType = "application/javascript";
		context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\";", filename));
		context.Response.AddHeader("Content-Length", text.Length.ToString());

		context.Response.Write(text);
    }
}