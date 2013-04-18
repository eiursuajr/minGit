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
using Ektron.Cms;

public partial class linkview : System.Web.UI.Page
{

    protected ContentAPI m_refContentApi = new ContentAPI();

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        string strStatus = "";
        long lId = 0;
        ContentData cContentEdit;
        bool lsForm = false;
        string strTemp = "";

        StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
        if (Convert.ToInt64(m_refContentApi.GetCookieValue("user_id")) == 0)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["fromEmail"]))
            {
                if (!(Request.QueryString["id"] == null) || (!(Request.QueryString["ekfrm"] == null)))
                {
                    strTemp = (string)("linkview.aspx?" + Request.QueryString.ToString());
                    strTemp = strTemp.Substring(0, System.Convert.ToInt32(strTemp.IndexOf("fromEmail") - 1));
                    Session["RedirectLnk"] = strTemp;
                }
                Response.Redirect("login.aspx?fromEmail=1", false);
            }
            return;
        }
        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            lId = System.Convert.ToInt32(Request.QueryString["id"]);
            FormBlock1.Visible = false;
        }
        else if (!String.IsNullOrEmpty(Request.QueryString["ekfrm"]))
        {
            lId = System.Convert.ToInt32(Request.QueryString["ekfrm"]);
            ContentBlock1.Visible = false;
            lsForm = true;
        }

        if (!String.IsNullOrEmpty(Request.QueryString["viewStatus"]))
        {
            strStatus = Request.QueryString["viewStatus"];
            if ((strStatus == "viewcheckedin") || (strStatus == "viewcheckedout") || (strStatus == "viewApproval"))
            {
                cContentEdit = m_refContentApi.GetContentById(lId, ContentAPI.ContentResultType.Staged);
                if (cContentEdit != null)
                {
                    if (lsForm)
                    {
                        FormBlock1.Text = cContentEdit.Html;
                    }
                    else
                    {
                        ContentBlock1.Text = cContentEdit.Html;
                    }
                }
            }
        }
    }
}