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
using System.Data.SqlClient;
using Ektron.Cms.Widget;
using Ektron.Cms;
using gnu.java.security.util;
using Ektron.Cms.Workarea.PrivateDataModel;
using Ektron.Cms.Common;

public partial class Workarea_PrivateData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string key;
        IPrivateDataModel model;

        switch (Request.QueryString["action"])
        {
            case "get":

                // Get and check query string parameters
                key = Request.QueryString["key"];
                if (key == null)
                    throw new Exception("Couldn't find query string parameter 'key'");
                key = key.Trim();
                if (key == "")
                    throw new Exception("Empty query string parameter 'key'");

                model = new PrivateDataModel();
                
                Response.Clear();
                Response.Write(Convert.ToBase64String(model.Get(new SiteAPI().UserId, key)));
                Response.End();

                break;

            case "set":
                
                // Get and check query string parameters
                key = Request.QueryString["key"];
                if (key == null)
                    throw new Exception("Couldn't find query string parameter 'key'");
                key = key.Trim();
                if (key == "")
                    throw new Exception("Empty query string parameter 'key'");

                model = new PrivateDataModel();

                Response.Clear();
                model.Set(new SiteAPI().UserId, key, Request.BinaryRead(Request.TotalBytes));
                Response.End();

                break;

            case null:
                throw new Exception("Couldn't find query string parameter 'action'");
                //break;

            default:
                throw new Exception("Unknown action '" +  Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.QueryString["action"]) + "'");
                //break;
        }
    }
}
