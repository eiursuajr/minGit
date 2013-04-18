using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea.Framework;

public partial class ektidy : WorkAreaBasePage
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        AssertInternalReferrer();
        string output = "";

        try
        {
            string sHtml;
            sHtml = Request["html"];
            if (sHtml == null)
            {
                throw (new ArgumentException("Argument \'html\' is required."));
            }

            Ektron.Cms.Content.EkContent objEkContent = new Ektron.Cms.Content.EkContent(this.GetCommonApi().RequestInformationRef);

            output = objEkContent.ConvertHtmlContenttoXHTML(sHtml);

        }
        catch (Exception ex)
        {
            output = string.Format("<html><head><title>ekAjaxTidy Error</title></head><body class=\"ekAjaxTidyError\">{0}</body></html>", EkFunctions.HtmlEncode(ex.Message));
        }

        Response.ContentType = "application/xml";
        Response.ContentEncoding = System.Text.Encoding.UTF8; // Safari does not encode properly even though this is set
        litContent.Text = output;
    }
}
