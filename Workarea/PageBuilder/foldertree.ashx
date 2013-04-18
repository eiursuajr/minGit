<%@ WebHandler Language="C#" Class="foldertree" %>

using System;
using System.Web;
using System.IO;
using System.Web.UI;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[JsonObject(MemberSerialization.OptIn)]
public class DirectoryInfo
{
    [JsonProperty("subdirectories")]
    public List<string> SubDirectories = new List<string>();

    [JsonProperty("files")]
    public List<string> Files = new List<string>();
}

public class foldertree : IHttpHandler {
    
    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Buffer = false;

        string sitePath = new Ektron.Cms.SiteAPI().SitePath;
        string rootPath = context.Server.MapPath(sitePath);
        string path = context.Server.MapPath(sitePath + context.Request.Params["path"]);
        Regex filter = null;
        if(context.Request.Params["filter"] != null)
            filter = new Regex(context.Request.Params["filter"]);

        DirectoryInfo directoryInfo = new DirectoryInfo();

        Converter<string, string> trimPath = delegate(string str) { return str.Substring(rootPath.Length); };
        foreach (string s in Directory.GetDirectories(path)) directoryInfo.SubDirectories.Add(trimPath(s));
        foreach (string s in Directory.GetFiles(path))
        {
            if (filter != null && filter.IsMatch(s))
                directoryInfo.Files.Add(trimPath(s));
        }
        if(directoryInfo.Files.Count != 0 || directoryInfo.SubDirectories.Count != 0)
         context.Response.Write(JsonConvert.SerializeObject(directoryInfo));

        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}