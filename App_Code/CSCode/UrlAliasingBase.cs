using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Common;
using Ektron.Cms;

/// <summary>
/// Summary description for UrlAliasingBase
/// </summary>
public class UrlAliasingBase
{
    public static void ProcessUrl(HttpContext context, ref EkRequestInformation RequestInfo)
    {

        //Check if this is a Asset request
        if (Ektron.ASM.EkHttpDavHandler.Utilities.IsAssetFile(context) || Ektron.ASM.EkHttpDavHandler.Utilities.IsPrivateAssetFile(context) || Ektron.ASM.EkHttpDavHandler.Utilities.IsDavFile(context))
        {
            return;
        }
        //Skip workarea files
        if (context != null && context.Request.PhysicalPath.ToLower().IndexOf("\\workarea\\") >= 0)
        {
            return;
        }



        string fileExtension = string.Empty;
        fileExtension = System.IO.Path.GetExtension(HttpContext.Current.Request.PhysicalPath);

        if (fileExtension == string.Empty && !context.Request.Url.LocalPath.EndsWith("/"))
        {
            context.Response.Redirect(context.Request.Url.LocalPath + "/", true);
        }
        else
        {
            Ektron.Cms.Framework.Settings.UrlAliasing.CommonAliasManager aliasmanager = new Ektron.Cms.Framework.Settings.UrlAliasing.CommonAliasManager();
            string targetUrl = aliasmanager.GetTarget(context.Request.Url);

            if (!string.IsNullOrEmpty(targetUrl))
            {
                HttpContext.Current.Items["EkOriginalPath"] = HttpContext.Current.Request.Path;
                context.RewritePath(targetUrl, false);
            }
        }

    }
}
