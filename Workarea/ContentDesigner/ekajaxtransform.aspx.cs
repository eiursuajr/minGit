using System;
using Ektron.Cms.Common;

public partial class ektransform : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    Ektron.Cms.ContentAPI refContentApi = new Ektron.Cms.ContentAPI();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        string sessionId = "";
        if (!string.IsNullOrEmpty(Request["sessionid"]))
        {
            sessionId = Request["sessionid"];
        }
        AssertInternalReferrer(sessionId);
        string output = "<root></root>";

        try
        {
            string sXml = string.Empty;
            string sXslt = string.Empty;
            long xmlConfigId = 0;
            bool isPackageXsl = false;

            if (!string.IsNullOrEmpty(Request["xml"]))
            {
                sXml = Request["xml"];
            }

            if (!string.IsNullOrEmpty(Request["xslt"]))
            {
                sXslt = Request["xslt"];
            }
            if (!string.IsNullOrEmpty(Request["xid"]))
            {
                xmlConfigId = EkFunctions.ReadDbLong(Request["xid"]);
            }

            Ektron.Cms.Xslt.ArgumentList args = null;
            int iArg;
            string sArg = string.Empty;
            int pEqu;
            string sName;
            string sValue;
            iArg = 0;
            if (!string.IsNullOrEmpty(Request["arg0"]))
            {
                sArg = Request["arg0"]; // format: "arg0=" + escape(name + "=" + value)
            }
            
            while (!string.IsNullOrEmpty(sArg) && sArg.Length > 0)
            {
                pEqu = sArg.IndexOf("="); // separator b/n name and value
                if (pEqu >= 1)
                {
                    sName = sArg.Substring(0, pEqu);
                    sValue = sArg.Substring(pEqu + 1);
                    if (args == null)
                    {
                        args = new Ektron.Cms.Xslt.ArgumentList();
                    }
                    args.AddParam(sName, "", sValue);
                }
                iArg++;
                if (!string.IsNullOrEmpty(Request["arg" + iArg]))
                {
                    sArg = Request["arg" + iArg];
                }
                else
                {
                    sArg = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(sXml) && Ektron.Cms.Common.EkFunctions.IsURL(sXml))
            {
                try
                {
                    sXml = new Uri(Request.Url, sXml).AbsoluteUri;
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            // Can't MapPath b/c the transform will lose its context and relative paths will fail.
            if (!string.IsNullOrEmpty(sXslt) && !sXslt.StartsWith("<"))
            {
                bool whitelist = Ektron.Cms.EkXml.IsWhiteListFile(sXslt, refContentApi.ApplicationPath, System.Web.HttpContext.Current.Request.PhysicalApplicationPath, System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"]);
                // either the file will be whitelisted, or the user is logged in and a member of the smart form admins group.
                if (whitelist || Utilities.ValidateUserRole(EkEnumeration.CmsRoleIds.AdminXmlConfig, false))
                {
                    try
                    {
                        sXslt = new Uri(Request.Url, sXslt).AbsoluteUri;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
                else
                {
                    sXslt = string.Empty;
                    return;
                }
            }
            else if (xmlConfigId > 0) // smart form editor
            {
                if (Utilities.ValidateUserRole(EkEnumeration.CmsRoleIds.AdminXmlConfig, false) ||
                    !Ektron.Cms.UserContext.GetCurrentUser().IsMemberShip)
                    isPackageXsl = true;
                else
                    sXslt = "";
            }
            output = refContentApi.XSLTransform(sXml, sXslt, false, false, args, true, isPackageXsl);
        }
        catch (Exception ex)
        {
            output = string.Format("<html><head><title>ekAjaxTransform Error</title></head><body class=\"ekAjaxTransformError\">{0}</body></html>", EkFunctions.HtmlEncode(ex.Message));
        }
        Response.ContentType = "application/xml";
        Response.ContentEncoding = System.Text.Encoding.UTF8; // Safari does not encode properly even though this is set
        litContent.Text = output;
    }
}		
