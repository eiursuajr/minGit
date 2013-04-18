<%@ WebHandler Language="C#" Class="PrivateData" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.Workarea.PrivateDataModel;




public class PrivateData : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        string key;
        IPrivateDataModel model;

        switch (context.Request.QueryString["action"])
        {
            case "get":

                // Get and check query string parameters
                key = context.Request.QueryString["key"];
                if (key == null)
                    throw new Exception("Couldn't find query string parameter 'key'");
                key = key.Trim();
                if (key == "")
                    throw new Exception("Empty query string parameter 'key'");

                model = new PrivateDataModel();

                context.Response.Write(Convert.ToBase64String(model.Get(new SiteAPI().UserId, key)));

                break;

            case "set":

                // Get and check query string parameters
                key = context.Request.QueryString["key"];
                if (key == null)
                    throw new Exception("Couldn't find query string parameter 'key'");
                key = key.Trim();
                if (key == "")
                    throw new Exception("Empty query string parameter 'key'");

                model = new PrivateDataModel();

                model.Set(new SiteAPI().UserId, key, context.Request.BinaryRead(context.Request.TotalBytes));

                break;

            case null:
                throw new Exception("Couldn't find query string parameter 'action'");
                //break;

            default:
                throw new Exception("Unknown action '" + HttpUtility.HtmlEncode(context.Request.QueryString["action"]) + "'");
                //break;
        }
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}