<%@ WebHandler Language="C#" Class="ImageHandler" %>

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

public class ImageHandler : IHttpHandler
{
    private int contentPageSize = 8;
    private int currentPageNo = 1;
    private int pageSize = 500;

    private Request request;
    private Ektron.Cms.ContentAPI capi;
    private Ektron.Cms.Content.EkContent ekcontent;
    HttpContext _context;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Buffer = false;

        capi = new Ektron.Cms.ContentAPI();
        ekcontent = capi.EkContentRef;
        
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
                if (long.TryParse(context.Request["thumbnailForContentID"], out contentid) &&
                    long.TryParse(context.Request["libraryFolder"], out folderid))
                {
                    string message = "";
                    long imageid = HandleUpload(context, folderid, out message);
                    if (contentid > -2) //if it's -2 we are in a partial upload
                    {
                        if (contentid == -1) //exception from trying to save
                        {
                            response = message;
                        }
                        else
                        {
                            response = getImageDetails(imageid, context);
                        }
                    }
                }
            }
            else if (context.Request["detail"] != null)
            {
                response = getcontenttip();

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

            if (ekcontent.IsAllowed(fldID, 0, "folder", "ReadOnlyLib", c.UserId) || ekcontent.IsAllowed(fldID, 0, "folder", "TransverseFolder", c.UserId))
            {

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

                        my.FolderID = items[i].ContentId;
                        my.Id = items[i].Id;
                        my.Title = items[i].Title;
                        results.Contents.Add(my);
                    }
                    results.PagingLinks = MakePagingLinks(request, results);
                }
            }

            else
            {
               
                return "NoPerm";
            }
        }
        return JsonConvert.SerializeObject(results);
    }

    public string getcontenttip()
    {
        Folder fol = new Folder();
        try
        {
            long objid;
            StringBuilder sb = new StringBuilder();
            if (long.TryParse(_context.Request["detail"], out objid))
            {
                //get whatever it is and render the summary               
                Ektron.Cms.API.Library apiLib = new Library();
                Ektron.Cms.LibraryData cb = apiLib.GetLibraryItem(objid);          

                if (cb != null)
                {
                    //if it's a form the teaser is some vastly complicated xml
                    string teasertext = cb.Teaser;


                    sb.Append("<div class=\"contentDetails\"><span>Image Details:</span><table><tr><td>");
                    sb.Append("Last Modified:");
                    sb.Append("</td><td>");
                    sb.Append(cb.DisplayLastEditDate);
                    sb.Append("</td></tr><tr><td>");
                    sb.Append("Last Editor:");
                    sb.Append("</td><td>");
                    sb.Append(cb.EditorFirstName + " " + cb.EditorLastName);
                    sb.Append("</td></tr><tr><td colspan=\"2\">");
                    sb.Append("Preview:");
                    sb.Append("</td></tr></table>");                 
                    sb.AppendFormat("<div class=\"teaser\" style=\"text-align:center;\">{0}</div>", GetImageWithTag(cb.FileName, 125, 100));
                    sb.Append("</div>");
                }
            }
            return sb.ToString();
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private long AddLibrary(FileStream file, string filename, string tempfilename, long folderid, out string message)
    {
        long cid = -1;
        message = "";
        filename = System.IO.Path.GetFileName("" + tempfilename);
        try
        {
            LibraryData libData = new LibraryData();
            Ektron.Cms.API.Folder fldApi = new Folder();
            string strPath = fldApi.GetPath(folderid);
            string destPath = "/uploadedimages" + strPath + "/"; ;
            libData.Teaser = "";
            libData.Title = filename;
           
            libData.ParentId = folderid;
            libData.Type = "images";
            libData.FileName = destPath + filename;

            Ektron.Cms.ContentAPI contentApi= new ContentAPI();
            cid = contentApi.AddLibraryItem(ref libData);
            
            Utilities.ProcessThumbnail(HttpContext.Current.Server.MapPath("~" + destPath), filename);
        }
        catch (Exception ex)
        {
            message = ex.Message.ToString();
            if (message.Contains("This file type")) message += ". Please see your administrator to enable it.";
            return -1;
        }

        return cid;
    }


    public long HandleUpload(HttpContext context, long folderid, out string message)
    {
        long contentid = -2;
        message = "";

        string realfilename = Path.GetFileName(context.Request.QueryString["filename"]);
        string fileNoExt = realfilename.Substring(0, realfilename.LastIndexOf('.'));
        string fileExt = realfilename.Substring(realfilename.LastIndexOf('.'), realfilename.Length - fileNoExt.Length);
        string tempfilename = fileNoExt + "_" + folderid.ToString() + "_" + new SiteAPI().UserId.ToString() + fileExt;
        
        Ektron.Cms.API.Folder fldApi = new Folder();
        string strPath = fldApi.GetPath(folderid);
        string docFilePath = HttpContext.Current.Server.MapPath("~/uploadedimages") + strPath + "\\";

        string destFileName = docFilePath + tempfilename;
        FileInfo fi = new FileInfo(destFileName);
        if (fi.Exists)
        {
            int iUnqueNameIdentifier = 0;
            while (fi.Exists)
            {
                iUnqueNameIdentifier = iUnqueNameIdentifier + 1;
                tempfilename = fileNoExt + "_" + folderid.ToString() + "_" + new SiteAPI().UserId.ToString() + "(" + iUnqueNameIdentifier + ")" + fileExt;
                destFileName = docFilePath + tempfilename;
                fi = new FileInfo(destFileName);
            }
        }
        try
        {
            using (FileStream fs = File.Create(destFileName))
            {
                SaveFile(context.Request.InputStream, fs);
                fs.Flush();
                fs.Close();
            }
            using (FileStream fs = File.OpenRead(destFileName))
            {
                contentid = AddLibrary(fs, realfilename, tempfilename, folderid, out message);
            }
        }
        catch (Exception ex)
        {
            message = ex.Message.ToString();
            if (message.Contains("This file type")) message += ". Please see your administrator to enable it.";
            return -1;
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

    private string GetImageWithTag(string imageUrl, int MaxWidth, int MaxHeight)
    {

        string sitePath = "/";
        if (HttpContext.Current.Request.ApplicationPath != "/")
        {
            sitePath = HttpContext.Current.Request.ApplicationPath + "/";
        }


        string returnUrl = sitePath + "widgets/Image/images/NoImage.jpg";
        int returnWidth = 0, returnHeight = 0;


        imageUrl = imageUrl.Replace("//", "/");
        string realUrl = HttpContext.Current.Server.MapPath(imageUrl);
        string relativeUrl = imageUrl;
        if (File.Exists(realUrl))
        {
            int lastIndex = imageUrl.LastIndexOf("/");

            if (lastIndex != -1 && lastIndex < imageUrl.Length)
            {
                imageUrl = imageUrl.Substring(0, lastIndex + 1) + "thumb_" + imageUrl.Substring(lastIndex + 1);
            }

            if (!File.Exists(HttpContext.Current.Server.MapPath(imageUrl)))
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(realUrl);
                if (img.Width > MaxWidth || img.Height > MaxHeight)
                {
                    double widthRatio = (double)img.Width / (double)MaxWidth;
                    double heightRatio = (double)img.Height / (double)MaxHeight;
                    double ratio = Math.Max(widthRatio, heightRatio);
                    returnWidth = (int)(img.Width / ratio);
                    returnHeight = (int)(img.Height / ratio);
                }
                returnUrl = relativeUrl;
            }
            else
            {
                returnUrl = imageUrl;
            }
        }


        if (returnWidth > 0)
        {
            return string.Format("<img src=\"{0}\" border='0' width=\"{1}\" height=\"{2}\"/>", returnUrl, returnWidth, returnHeight);
        }
        else
        {
            return string.Format("<img src=\"{0}\" border='0'/>", returnUrl);
        }
    }
}


 