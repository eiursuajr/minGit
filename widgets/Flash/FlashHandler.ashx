<%@ WebHandler Language="C#" Class="FlashHandler" %>

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
    [JsonProperty("canadd")]
    public bool CanAdd = false; 
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

public class FlashHandler : IHttpHandler {
    private int contentPageSize = 8;
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
                    case "getfoldercontent":
                        response = getfoldercontent(context.Request["filter"]);
                        break;
                }
            }
            else if (context.Request["detail"] != null)
            {
                response = getcontenttip();
            }
            else if (context.Request["folderid"] != null)
            {
                long folderid = -1;
                if (long.TryParse(context.Request["folderid"], out folderid))
                {
                    string message = "";
                    long contentid = HandleUpload(context, folderid, out message);
                    if (contentid > -2) //if it's -2 we are in a partial upload
                    {
                        if (contentid == -1) //exception from trying to save
                        {
                            response = message;
                        }
                        else
                        {
                            response = getVideoDetails(contentid, context) + "|";
                        }
                    }
                }
            }
            else if (context.Request["selectedContent"] != null)
            {
                long contentid = -1;
                if (long.TryParse(context.Request["selectedContent"], out contentid))
                {
                    response = getVideoDetails(contentid, context) + "|";
                }
            }
            else if (context.Request["selectedThumb"] != null)
            {
                long thumbid = -1;
                if (long.TryParse(context.Request["selectedThumb"], out thumbid))
                {
                    LibraryData ld = capi.GetLibraryItemByContentID(thumbid);
                    response = ld.FileName.Replace("//","/");
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
                            //get image location
                            LibraryData ld = capi.GetLibraryItemByContentID(imageid);
                            response = getVideoDetails(contentid, context).Replace("None|", ld.FileName.Replace("//","/")) + "|" + imageid.ToString();
                        }
                    }
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
    
    private string getVideoDetails(long contentid, HttpContext context)
    {
        string response = "";
        Ektron.Cms.Common.ContentBase cb = capi.EkContentRef.LoadContent(contentid, false);
        System.Uri assetpath = new Uri(cb.AssetInfo.FileName);
        if (cb.AssetInfo.FileExtension.ToLower() == "flv")
        {
            FileStream fileStream = null;
            try
            {
                string title = cb.Title + " (flv)";
                fileStream = File.OpenRead(context.Server.MapPath(assetpath.AbsolutePath));
                response = cb.FolderId.ToString() + "|" + contentid.ToString() + "|" + title + "|" + getVideoDimensions((Stream)fileStream);
            }
            finally
            {
                fileStream.Close();
            }
        }
        else //swf
        {
            string html = cb.Html;
            string pixelwidth = "0";
            string pixelheight = "0";
            if (html.Length > 0)
            {
                int startwidth = html.IndexOf("width=\"") + 7;
                int endwidth = html.IndexOf("px", startwidth);
                int startheight = html.IndexOf("height=\"") + 8;
                int endheight = html.IndexOf("px", startheight);
                if (startwidth > 7 && endwidth > startwidth && startheight > 8 && endheight > startheight)
                {
                    pixelwidth = html.Substring(startwidth, endwidth - startwidth);
                    pixelheight = html.Substring(startheight, endheight - startheight);
                }
            }
            response = cb.FolderId.ToString() + "|" + contentid.ToString() + "|" + cb.Title + "|" + pixelwidth + "|" + pixelheight;
        }
        response += "|None";
        return "success|" + response + "|";
    }
    
    private string getVideoDimensions(Stream file)
    {
        double width = 0;
        double height = 0;
        double duration = 0;
        try
        {
            //read data from memory
            byte[] bytes = new byte[10];
            file.Seek(27, SeekOrigin.Begin);
            int result = file.Read(bytes, 0, 10);
            string onMetaData = ByteArrayToString(bytes);
            if (onMetaData == "onMetaData")
            {
                duration = GetNextDouble(file, 16, 8);
                width = GetNextDouble(file, 8, 8);
                height = GetNextDouble(file, 9, 8);
            }
            file.Seek(0, SeekOrigin.Begin);
        }
        catch
        {

        }
        if (height < 100 | height > 2000 | width < 100 | width > 2000)
        {
            height = 240;
            width = 320;
        }

        return width.ToString() + "|" + height.ToString();
    }
    
    private static string ByteArrayToString(byte[] bytes)
    {
        string byteString = string.Empty;
        foreach (byte b in bytes)
        {
            byteString += Convert.ToChar(b).ToString();
        }
        return byteString;
    }
    private static Double GetNextDouble(Stream fileStream, int offset, int length)
    {
        fileStream.Seek(offset, SeekOrigin.Current);
        byte[] bytes = new byte[length];
        int result = fileStream.Read(bytes, 0, length);
        return ByteArrayToDouble(bytes, true);
    }
    private static Double ByteArrayToDouble(byte[] bytes, bool readInReverse)
    {
        if (bytes.Length != 8)
            throw new Exception("bytes must be exactly 8 in Length");
        if (readInReverse)
            Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    public string getchildfolders()
    {
        DirectoryList directoryInfo = new DirectoryList();
        Ektron.Cms.PermissionData _permissionData = new PermissionData();
        
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
                    _permissionData = capi.LoadPermissions(f.Id, "folder", ContentAPI.PermissionResultType.All);
                    mytd.CanAdd = _permissionData.CanAdd;
                    directoryInfo.SubDirectories.Add(mytd);
                }
            }
            return JsonConvert.SerializeObject(directoryInfo);
        }
        return "";
    }

    public string getfoldercontent(string filter)
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
            Ektron.Cms.Common.EkContentCol ekc = capi.EkContentRef.GetAllViewableChildContentInfoV5_0(pagedata, 0, 900, ref pages);

            if (ekc != null && ekc.Count > 0)
            {
                List<Ektron.Cms.Common.ContentBase> items = new List<Ektron.Cms.Common.ContentBase>();
                foreach (Ektron.Cms.Common.ContentBase t in ekc)
                {
                    if (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && 
                       (filters.Contains(t.AssetInfo.FileExtension)))
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
                    my.FolderID = items[i].FolderId;
                    my.Id = items[i].Id;
                    my.Title = items[i].Title;
                    results.Contents.Add(my);
                }
                results.PagingLinks = MakePagingLinks(request, results);
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
                Ektron.Cms.Common.ContentBase cb = capi.EkContentRef.LoadContent(objid, false);
                if (cb != null)
                {
                    //if it's a form the teaser is some vastly complicated xml
                    string teasertext = "";
                    try
                    {
                        System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
                        xd.LoadXml(cb.Teaser);
                        System.Xml.XmlNode xn = xd.SelectSingleNode("ektdesignpackage_forms/ektdesignpackage_form/ektdesignpackage_designs/ektdesignpackage_design");
                        teasertext = xn.InnerXml;
                    }
                    catch 
                    {
                        teasertext = cb.Teaser;
                    }
                    
                    sb.Append("<div class=\"contentDetails\"><span>Content Details:</span><table><tr><td>");
                    sb.Append("Last Modified:");
                    sb.Append("</td><td>");
                    sb.Append(cb.DisplayDateModified);
                    sb.Append("</td></tr><tr><td>");
                    sb.Append("Last Editor:");
                    sb.Append("</td><td>");
                    sb.Append(cb.LastEditorFname + " " + cb.LastEditorLname);
                    sb.Append("</td></tr><tr><td colspan=\"2\">");
                    sb.Append("Teaser:");
                    sb.Append("</td></tr></table>");
                    sb.Append("<div class=\"teaser\">" + ConvertURLs(teasertext) + "</div>");
                    if (cb.AssetInfo.FileExtension.ToLower() == "jpg" || cb.AssetInfo.FileExtension.ToLower() == "gif")
                    {
                        sb.Append("<div class=\"image\"><img src=\"" + cb.AssetInfo.FileName + "\" style=\"width:250px; height:auto;\" /></div>");
                    }
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
                    sb.Append((i+1).ToString() + " ");
                }
                else
                {
                    sb.Append(string.Format(linkformat, i, i+1));
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
        get {
            return false;
        }
    }

    /// <summary>
    /// If the Match object passed in is a matches the falsePositivePattern Regex, the original match is returned as a string.  Otherwise it returns a formatted anchor tag based on the original match.
    /// </summary>
    /// <param name="m">The regular expression match to check.</param>
    /// <returns>The original match as text or an anchor tag based on the match.</returns>
    protected string URLLinkFormat(Match m)
    {
        String match = m.ToString();
        String result = "";
        // check the first character and preserve if needed
        String firstCharacter = match.Substring(0, 1);
        String firstCharacterSpaceOrNewLine = @"[ \t\r\n]";
        if (Regex.IsMatch(firstCharacter, firstCharacterSpaceOrNewLine))
        {
            match = match.Substring(1);
        }
        else
        {
            firstCharacter = "";
        }
        result = String.Format("<a href=\"{0}\" title=\"{0}\" class=\"EkForceWrap\" onclick=\"window.open(this.href); return false;\">{0}</a>", match);
        return firstCharacter + result;
    }

    /// <summary>
    /// Method returns a formated string for a mailto: link
    /// </summary>
    /// <param name="m">Match genersated by Regular Expression object</param>
    /// <returns>mailto anchor tag based on the original Match text.</returns>
    protected string EmailAddressFormat(Match m)
    {
        String match = m.ToString();
        String result = "";
        // check the first character and preserve if needed
        String firstCharacter = match.Substring(0, 1);
        String firstCharacterSpaceOrNewLine = @"[ \t\r\n]";
        if (Regex.IsMatch(firstCharacter, firstCharacterSpaceOrNewLine))
        {
            match = match.Substring(1);
        }
        else
        {
            firstCharacter = "";
        }
        result = String.Format("<a href=\"mailto:{0}\" title=\"{0}\" class=\"EkForceWrap\">{0}</a>", match);
        return firstCharacter + result;
    }

    /// <summary>
    /// Searches through a given string and looks for URLs embedded in the text.  Any URLs found are converted to anchor tags based on that URL.
    /// </summary>
    /// <param name="stringIn">The string to search for URLs.</param>
    /// <returns>A string where all URLs have been converted to links based on the URLs found.</returns>
    protected string ConvertURLs(string stringIn)
    {
        // create a Regular Expression instance to find URLs in the text
        //Regex urlPattern = new Regex(@"(^|[\s\n])((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.([a-zA-Z]{2,3}|[0-9]{1,3})(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\;\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]");
        Regex urlPattern = new Regex(@"(^|[ \t\r\n])((ftp|http|https|file)://(([A-Za-z0-9$_.+!*(),;/?:@&~=-])|%[A-Fa-f0-9]{2}){2,}(#([a-zA-Z0-9][a-zA-Z0-9$_.+!*(),;/?:@&~=%-]*))?(([A-Za-z0-9$_+!*();/?:~-])|%[A-Fa-f0-9]{2}))", RegexOptions.IgnoreCase);

        Regex emailPattern = new Regex(@"(^|[ \t\r\n])[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*", RegexOptions.IgnoreCase);
        MatchEvaluator urlLinks = new MatchEvaluator(this.URLLinkFormat);
        MatchEvaluator emailAddresses = new MatchEvaluator(this.EmailAddressFormat);
        String result = urlPattern.Replace(stringIn, urlLinks);
        result = emailPattern.Replace(result, emailAddresses);
        return result;
    }

    private long AddAssetIntoCmsDms(FileStream file, string filename, long folderid, out string message)
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
            aud.Title = filename;
            cid = capi.AddAsset(file, aud);
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
                    contentid = AddAssetIntoCmsDms(fs, realfilename, folderid, out message);
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
}