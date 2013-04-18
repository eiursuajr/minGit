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
using Ektron.Cms;

public partial class blankredirect : System.Web.UI.Page
{
    protected EkMessageHelper MsgHelper;
    protected CommonApi m_refApi = new CommonApi();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        string[] queryInfo;
        MsgHelper = m_refApi.EkMsgRef;
        queryInfo = Request.Url.PathAndQuery.Split("?".ToCharArray());
        if (queryInfo.Length > 1)
        {
            string url = Page.ResolveUrl(Request.Url.PathAndQuery.Substring(Request.Url.PathAndQuery.IndexOf("blankredirect.aspx?") + "blankredirect.aspx?".Length));
            bool isLocal = false;
            Uri absoluteUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
            {
                isLocal = String.Equals(this.Request.Url.Host, absoluteUri.Host,
                            StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                    && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
                    && Uri.IsWellFormedUriString(url, UriKind.Relative);
            }
            if (!isLocal)
            {
                lblMessage.Text = MsgHelper.GetMessage("lbl not valid url");
            }
            else
            {
                Response.Redirect(url);
            }
        }
        else
        {
            lblMessage.Text = MsgHelper.GetMessage("lbl not valid url");
        }
    }
}
	

