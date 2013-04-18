<%@ WebHandler Language="C#" Class="FolderAction" %>
using System;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;

public class FolderAction : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        try
        {
            context.Response.ContentType = "text/plain";
            context.Server.ScriptTimeout = 6000;

            string action = "";
            if (context.Request.Form["action"] != null)
            {
                action = context.Request.Form["action"].ToString();
            }
            else if (context.Request.Form["action"] != null)
            {
                action = context.Request.Form["action"].ToString();
            }
            
            switch (action.ToLower())
                {
                    case "checkfolderexists":
                        context.Response.Write(CheckFolderExists(context));
                        break;
                }
        }
        catch
        {
        }  
    }

    public string CheckFolderExists(HttpContext context)
    {
        string returnValue = "";
        try
        {
            ContentAPI api = new ContentAPI();
            EkMessageHelper MessageHelper = api.EkMsgRef;
            string folderName = "";
            long parentid = -1;

            if (context.Request.Form["name"] != null && context.Request.Form["name"].ToString().Length > 0)
                folderName = context.Request.Form["name"].ToString();
            if (context.Request.Form["parentid"] != null && context.Request.Form["parentid"].ToString().Length > 0)
                Int64.TryParse(context.Request.Form["parentid"].ToString(), out parentid);

            if (folderName.Length > 0 && parentid != -1 && api.EkContentRef.DoesFolderExistsWithName(folderName, parentid))
                returnValue = "<error>" + MessageHelper.GetMessage("com: subfolder already exists") + "</error>";
        }
        catch
        {
        }
        return returnValue; 
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }

}