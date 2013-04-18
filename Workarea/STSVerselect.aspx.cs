using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Workarea_STSVerselect : System.Web.UI.Page
{
    private string DMSCookieName = "DMS_Office_ver";
    protected Ektron.Cms.ContentAPI contentAPI = new Ektron.Cms.ContentAPI();
    protected void Page_Load(object sender, EventArgs e)
    {
        lit_VerionSelect.Text = contentAPI.EkMsgRef.GetMessage("lbl dms office ver sel header");
        foreach (ListItem li in rbl_OfficeVersion.Items)
        {
            if (li.Value == "2010")
                li.Text = contentAPI.EkMsgRef.GetMessage("li text office 2010 name");
            else
                li.Text = contentAPI.EkMsgRef.GetMessage("li text other office ver name");
        }
    }

    protected void btn_VersionSelect_Click(object sender, EventArgs e)
    {


        //Create cookie
        HttpCookie c = new HttpCookie(DMSCookieName, rbl_OfficeVersion.SelectedValue);
        c.Expires = DateTime.Now.AddYears(50);
        Response.Cookies.Remove(c.Name);
        Response.Cookies.Add(c);

        string RetURL = string.Format("{0}?action={1}&id={2}&treeViewId=0", Request.QueryString["back_file"], Request.QueryString["back_action"], Request.QueryString["back_id"]);
        Response.Redirect(RetURL);
    }
}