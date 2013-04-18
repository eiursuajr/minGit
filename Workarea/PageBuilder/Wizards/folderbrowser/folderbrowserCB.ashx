<%@ WebHandler Language="C#" Class="folderbrowserCB" %>

using System;
using System.Web;
using Ektron.Cms;

public class folderbrowserCB : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        if (context.Request["folderid"] != null)
        {
            long folderid;
            if(long.TryParse(context.Request["folderid"], out folderid)){
                context.Response.Write(getchildfolders(folderid));
            }
        }
        context.Response.ContentType = "text/plain";
    }

    public string getchildfolders(long folderid)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Ektron.Cms.ContentAPI capi = new Ektron.Cms.ContentAPI();
        Ektron.Cms.FolderData[] folders = capi.GetChildFolders(folderid, true, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
        Ektron.Cms.PageBuilder.WireframeModel wfm = new Ektron.Cms.PageBuilder.WireframeModel();
        if (folders != null && folders.Length > 0)
        {
            sb.Append("<ul>");
            foreach (FolderData folder in folders)
            {
                Ektron.Cms.PageBuilder.WireframeData[] wireframes = wfm.FindByFolderID(folder.Id);
                sb.Append("<li class=\"");
                if (folder.HasChildren) sb.Append("ui-finder-folder");
                if (wireframes.Length > 0) sb.Append(" hasWireframe");
                sb.Append("\"><a href=\"");
                sb.Append(capi.AppPath + "/PageBuilder/Wizards/folderbrowser/folderbrowserCB.ashx?folderid=");
                sb.Append(folder.Id);
                sb.Append("\">");
                sb.Append(folder.Name);
                sb.Append("</a></li>");
            }
            sb.Append("</ul>");
        }
        return sb.ToString();
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }

}