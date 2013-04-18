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
using Ektron.Cms.Controls;

public partial class Workarea_EditUserProfile : System.Web.UI.Page
{


    protected void Page_Load(object sender, System.EventArgs e)
    {
        long userID = 0;
        long.TryParse(Request.QueryString["id"], out userID);
        Ektron.Cms.API.User.User userAPI = new Ektron.Cms.API.User.User();
        if (userAPI.UserId == 0)
        {
            Membership1.Visible = false;
            ltrMessage.Visible = true;
            ltrMessage.Text = "Please login to see your Profile.";
            return;
        }
        if (Page.IsPostBack && Membership1.LocalizeString(Membership1.UserUpdateSuccessMessage).Trim() == Membership1.Text.Trim())
        {

            Session.Remove("Ektron.eIntranet." + userID.ToString() + ".userdata");
            StringBuilder sbJScript = new StringBuilder();
            sbJScript.Append("if (window.parent.document.getElementById(\'Ek_MemberEditRedirectUrlValue\') != null){").AppendLine(Environment.NewLine);
            sbJScript.Append("  parent.location.href = window.parent.document.getElementById(\'Ek_MemberEditRedirectUrlValue\').value").AppendLine(Environment.NewLine);
            sbJScript.Append("}else{").AppendLine(Environment.NewLine);
            sbJScript.Append("  parent.location.href = parent.location.href").AppendLine(Environment.NewLine);
            sbJScript.Append("}").AppendLine(Environment.NewLine);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refreshpage", sbJScript.ToString(), true);
        }
        if ((Request.QueryString["taxonomyId"] != null) && Request.QueryString["taxonomyId"] != "")
        {
            this.Membership1.TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyId"]);
        }
    }
}


