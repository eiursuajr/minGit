<%@ WebHandler Language="C#" Class="pagetree" %>

using System;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.API.Content;

public class pagetree : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string szFolderID = context.Request.QueryString["fid"];
        
        long folderID;
        try { folderID = long.Parse(szFolderID); }
        catch { folderID = 0; }
        
        context.Response.ContentType = "text/plain";

        // Setup Folders
        Folder folder = new Folder();
        FolderData[] folders = folder.GetChildFolders(folderID, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
        
        if (folders != null)
        {
            foreach (FolderData folderData in folders)
            {
                string folderTypeClass;

                switch (folderData.FolderType)
                {
                    case 1:
                        //blog
                        folderTypeClass = "blog";
                        break;
                    case 5:
                        //root
                        folderTypeClass = "root";
                        break;
                    case 9:
                        //product catalog
                        folderTypeClass = "catalog";
                        break;
                    case 8:
                        //calendar
                        folderTypeClass = "calendar";
                        break;
                    case 3:
                        //forum
                        folderTypeClass = "forum";
                        break;   
                    default:
                        folderTypeClass = "";
                        break;
                }
                
                context.Response.Write(string.Format("<li><span class=\"folder {2}\" folder-id=\"{1}\">{0}</span><ul class=\"EktronTreeview\"></ul></li>", folderData.Name, folderData.Id, 
                    folderTypeClass));
            }
        }
       

        // Setup Content
        Content content = new Content();
        ContentData[] contents = content.GetChildContent(folderID);
        if (contents != null)
        {
            contents = Array.FindAll<ContentData>(contents, delegate(ContentData c) { return c.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData; });

            if (contents.Length > 0)
            {
                foreach (ContentData contentData in contents)
                {
                    context.Response.Write(string.Format("<li><span class=\"file\" page-id=\"{1}\">{0}</span></li>", contentData.Title, contentData.Id));
                }
            }
            else if (folderID != 0)
            {
                context.Response.Write(string.Format("<li>{0}</li>", "No PageBuilder Content"));
            }
        }
        else if (folders == null)
        {
            context.Response.Write(string.Format("<li>{0}</li>", "No Sub Folders or PageBuilder Content"));
        }
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}