<%@ WebHandler Language="C#" Class="RotatingBannerHandler" %>

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
using System.IO;

#region RequestObject
[JsonObject(MemberSerialization.OptIn)]
public class Request
{
    [JsonProperty("action")]
    public string action = "";
    [JsonProperty("page")]
    public int page = 0;
    [JsonProperty("searchText")]
    public string searchText = "";
    [JsonProperty("objectType")]
    public string objectType = "";
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
public class ContentList
{
    [JsonProperty("contents")]
    public List<ContentResult> Contents = new List<ContentResult>();
    [JsonProperty("numpages")]
    public int Pages = 0;
    [JsonProperty("numitems")]
    public int Items = 0;
    [JsonProperty("paginglinks")]
    public string PagingLinks = "";
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
public class ContentResult
{
    [JsonProperty("title")]
    public string Title = "";
    [JsonProperty("id")]
    public long Id = 0;
    [JsonProperty("folderid")]
    public long FolderID = 0;
}

[JsonObject(MemberSerialization.OptIn)]
public class ContentDetail
{
    [JsonProperty("title")]
    public string Title = "";
    [JsonProperty("id")]
    public long Id = 0;
    [JsonProperty("folderid")]
    public long FolderID = 0;
    [JsonProperty("summary")]
    public long Summary = 0;
    [JsonProperty("link")]
    public long link = 0;
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

public class RotatingBannerHandler : IHttpHandler
{
    private int contentPageSize = 8;
    private int currentPageNo = 1;
    private int pageSize = 500;

    private Request request;
    private Ektron.Cms.ContentAPI capi;
    HttpContext _context;

    public void ProcessRequest(HttpContext context)
    {
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
                    case "getfoldercontent":
                        long fldID = request.objectID;
                        response = getfoldercontent(context.Request["filter"], fldID);
                        break;
                }
            }

            else if (context.Request["selectedContent"] != null)
            {
                long contentid = -1;
                if (long.TryParse(context.Request["selectedContent"], out contentid))
                {
                    response = getImageDetails(contentid, context) + "|";
                }

            }

            else if (context.Request["thumbnailForContentID"] != null && context.Request["libraryFolder"] != null)
            {
                long folderid = -1;
                long contentid = -1;
                long collectionID = -1;
                if (long.TryParse(context.Request["thumbnailForContentID"], out contentid) &&
                    long.TryParse(context.Request["libraryFolder"], out folderid) &&
                    long.TryParse(context.Request["collectionID"], out collectionID))
                {
                    string message = "";
                    long imageid = HandleUpload(context, folderid, collectionID, out message);
                    if (contentid > -2) //if it's -2 we are in a partial upload
                    {
                        if (contentid == -1) //exception from trying to save
                        {
                            response = message;
                        }
                        else
                        {
                            response = "|";
                        }
                    }
                }
            }
            context.Response.Write(response);
        }
        catch (Exception e)
        {
            Jsonexception ex = new Jsonexception();
            ex.message = e.Message;
            if (e.InnerException != null) ex.innerMessage = e.InnerException.Message;

            context.Response.Write(JsonConvert.SerializeObject(ex));
        }
        context.Response.End();
    }

    private string getImageDetails(long contentid, HttpContext context)
    {

        string response = "";
        Ektron.Cms.LibraryData libData = new LibraryData();
        Ektron.Cms.API.Library lib = new Library();
        libData = lib.GetLibraryItem(contentid);
        if (!ReferenceEquals(libData, null))
        {
            string title = libData.Title;
            string strFilePath = context.Server.MapPath(libData.FileName);

            FileInfo flInfo = new FileInfo(strFilePath);
            response = libData.ParentId.ToString() + "|" + libData.Id.ToString() + "|" + title + "|" + getImageDimensions(strFilePath) + "|" + "0";
        }

        response += "|None";
        return "success|" + response + "|";

    }

    private string getImageDimensions(string filePath)
    {
        double width = 0;
        double height = 0;

        try
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            System.Drawing.Image pv_OriginalImage = System.Drawing.Image.FromStream(new MemoryStream(imageData));
            width = pv_OriginalImage.Width;
            height = pv_OriginalImage.Height;
        }
        catch
        {

        }
        return width.ToString() + "|" + height.ToString();
    }

    public string getchildfolders()
    {
        DirectoryList directoryInfo = new DirectoryList();

        if (request.objectType == "folder" && request.objectID > -1)
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

    public string getfoldercontent(string filter, long fldID)
    {
        ContentList results = new ContentList();
        List<string> filters = new List<string>(filter.Split(','));

        if (request.objectType == "folder" && request.objectID > -1)
        {
            ContentAPI c = new ContentAPI();

            Microsoft.VisualBasic.Collection pagedata = new Microsoft.VisualBasic.Collection();
            pagedata.Add(request.objectID, "FolderID", null, null);
            pagedata.Add("title", "OrderBy", null, null);
            pagedata.Add(capi.RequestInformationRef.ContentLanguage, "m_intContentLanguage", null, null);

            int pages = 0;
            Ektron.Cms.API.Library libApi = new Library();

            LibraryData[] ekc = libApi.GetAllChildLibItems("images", fldID, "title", currentPageNo, pageSize, ref pages);
            if (ekc != null && ekc.Length > 0)
            {
                List<LibraryData> items = new List<LibraryData>();
                foreach (LibraryData t in ekc)
                {
                    if (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
                        items.Add(t);
                }

                results.Items = items.Count;
                results.Pages = (items.Count / contentPageSize) + (((items.Count % contentPageSize) > 0) ? 1 : 0);

                if (request.page > results.Pages - 1) request.page = results.Pages - 1;
                if (request.page < 0) request.page = 0;

                int startindex = request.page * contentPageSize;
                int endindex = startindex + contentPageSize;
                if (endindex > results.Items) endindex = results.Items;

                for (int i = startindex; i < endindex; i++)
                {
                    ContentResult my = new ContentResult();
                    //my.FolderID = items[i].FolderId;
                    my.FolderID = items[i].ContentId;
                    my.Id = items[i].Id;
                    my.Title = items[i].Title;
                    results.Contents.Add(my);
                }
                results.PagingLinks = MakePagingLinks(request, results);
            }
        }
        return JsonConvert.SerializeObject(results);
    }
    private long AddAssetIntoCmsDms(FileStream file, string filename, long folderid, long collectionID, out string message)
    {
        long cid = -1;
        message = "";
        filename = System.IO.Path.GetFileName(filename);
        try
        { // create the incoming file in the dmdata folder
            AssetUpdateData aud = new AssetUpdateData();
            aud.AssetId = Guid.NewGuid().ToString();
            aud.Comment = "";
            aud.ContentId = -1;
            aud.EndDate = "";
            aud.FileName = filename;
            aud.FolderId = folderid;
            aud.GoLive = "";
            aud.LanguageId = capi.ContentLanguage;
            aud.MetaData = null;
            aud.TaxonomyTreeIds = "";
            aud.Teaser = "";

            if (filename.Contains("."))
            {
                aud.Title = filename.Substring(0, filename.LastIndexOf("."));
            }
            else
            {
                aud.Title = filename;
            }

            cid = capi.AddAsset(file, aud);

            //Add to Collection
            capi.EkContentRef.AddItemToEcmCollection(collectionID, cid, capi.ContentLanguage);
        }
        catch (Exception ex)
        {
            message = ex.Message.ToString();
            if (message.Contains("This file type")) message += ". Please see your administrator to enable it.";
            return -1;
        }

        return cid;
    }

    public long HandleUpload(HttpContext context, long folderid, long collectionID, out string message)
    {
        long contentid = -2;
        message = "";
        string realfilename = Path.GetFileName(context.Request.QueryString["filename"]);
        string tempfilename = realfilename + "_" + folderid.ToString() + "_" + new SiteAPI().UserId.ToString();
        bool complete = string.IsNullOrEmpty(context.Request.QueryString["Complete"]) ? true : bool.Parse(context.Request.QueryString["Complete"]);
        bool getBytes = string.IsNullOrEmpty(context.Request.QueryString["GetBytes"]) ? false : bool.Parse(context.Request.QueryString["GetBytes"]);
        long startByte = string.IsNullOrEmpty(context.Request.QueryString["StartByte"]) ? 0 : long.Parse(context.Request.QueryString["StartByte"]); ;
        string docFilePath = Ektron.ASM.AssetConfig.DocumentManagerData.Instance.WebSharePath;
        if (!System.IO.Path.IsPathRooted(docFilePath))
        {
            docFilePath = Ektron.ASM.AssetConfig.Utilities.UrlHelper.GetAppPhysicalPath() + docFilePath;
        }
        string destFileName = docFilePath + tempfilename;

        if (getBytes)
        {
            FileInfo fi = new FileInfo(destFileName);
            if (!fi.Exists)
                context.Response.Write("0");
            else
                context.Response.Write(fi.Length.ToString());

            context.Response.Flush();
        }
        else
        {
            if (startByte > 0 && File.Exists(destFileName))
            {
                using (FileStream fs = File.Open(destFileName, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                }
            }
            else
            {
                using (FileStream fs = File.Create(destFileName))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                }
            }
            if (complete)
            {
                using (FileStream fs = File.OpenRead(destFileName))
                {
                    contentid = AddAssetIntoCmsDms(fs, realfilename, folderid, collectionID, out message);
                    fs.Close();
                    //clean up file from temp directory
                    File.Delete(destFileName);
                }
                //add into cms
            }
        }
        return contentid;
    }

    private void SaveFile(Stream stream, FileStream fs)
    {
        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            fs.Write(buffer, 0, bytesRead);
        }
    }

    public string MakePagingLinks(Request req, ContentList res)
    {
        string linkformat = "<a href=\"#\" pageid=\"{0}\">{1}</a> ";
        StringBuilder sb = new StringBuilder();

        if (res.Pages > 1)
        {
            if (req.page != 0)
            {
                sb.Append(string.Format(linkformat, 0, "<<"));
            }
            for (int i = 0; i < res.Pages; i++)
            {
                if (i == req.page)
                {
                    sb.Append((i + 1).ToString() + " ");
                }
                else
                {
                    sb.Append(string.Format(linkformat, i, i + 1));
                }
            }
            if (req.page != res.Pages - 1)
            {
                sb.Append(string.Format(linkformat, res.Pages - 1, ">>"));
            }
        }
        return sb.ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}


