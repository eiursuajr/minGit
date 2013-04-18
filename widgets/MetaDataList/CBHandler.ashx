<%@ WebHandler Language="C#" Class="CBHandler" %>

using System;
using System.Web;
using System.Text;
using Ektron.Cms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ektron.Cms.API.Search;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.WebSearch;
using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Newtonsoft.Json;


#region RequestObject
[JsonObject(MemberSerialization.OptIn)]
public class Request
{
    [JsonProperty("action")]
    public string action = "";
    [JsonProperty("objectID")]
    public long objectID = 0;
}
#endregion

#region ResponseObjects
[JsonObject(MemberSerialization.OptIn)]
public class DirectoryList
{
    [JsonProperty("subdirectories")]
    public List<DirectoryResult> SubDirectories = new List<DirectoryResult>();
}

[JsonObject(MemberSerialization.OptIn)]
public class DirectoryResult
{
    [JsonProperty("name")]
    public string Name = "";
    [JsonProperty("id")]
    public long id = 0;
    [JsonProperty("haschildren")]
    public bool HasChildren = false;
}

[JsonObject(MemberSerialization.OptIn)]
public class Jsonexception
{
    [JsonProperty("message")]
    public string message = "";
    [JsonProperty("innerMessage")]
    public string innerMessage = "";
}
#endregion

public class CBHandler : IHttpHandler {
    private Request request;
    private Ektron.Cms.ContentAPI capi;
    HttpContext _context;

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.Buffer = false;

        capi = new Ektron.Cms.ContentAPI();
        _context = context;

        try
        {
            string response = "";
            if (context.Request["request"] != null)
            {
                request = (Request)JsonConvert.DeserializeObject(context.Request["request"], typeof(Request));

                switch (request.action)
                {
                    case "getchildfolders":
                        response = getchildfolders();
                        break;
                }
            }
            context.Response.Write(response);
        }
        catch(Exception e)
        {
            Jsonexception ex = new Jsonexception();
            ex.message = e.Message;
            if (e.InnerException != null) ex.innerMessage = e.InnerException.Message;

            context.Response.Write(JsonConvert.SerializeObject(ex));
        }
        context.Response.End();
    }


    public string getchildfolders()
    {
        DirectoryList directoryInfo = new DirectoryList();

        long folid = 0;
        if (request.objectID > -1)
        {
            Folder fol = new Folder();
            FolderData[] fd = fol.GetChildFolders(request.objectID, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);

            if (fd != null && fd.Length > 0)
            {
                foreach (FolderData f in fd)
                {
                    DirectoryResult mytd = new DirectoryResult();
                    mytd.Name = f.Name;
                    mytd.id = f.Id;
                    mytd.HasChildren = f.HasChildren;
                    directoryInfo.SubDirectories.Add(mytd);
                }
            }
            return JsonConvert.SerializeObject(directoryInfo);
        }
        return "";
    }


    public bool IsReusable
    {
        get {
            return false;
        }
    }
}