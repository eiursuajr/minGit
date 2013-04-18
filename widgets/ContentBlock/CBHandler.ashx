<%@ WebHandler Language="C#" Class="Ektron.PFWidgets.ContentBlock.CBHandler" %>

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

namespace Ektron.PFWidgets.ContentBlock
{

    #region RequestObject
    [JsonObject(MemberSerialization.OptIn)]
    public class Request
    {
        [JsonProperty("action")]
        public string action = "";
        [JsonProperty("filter")]
        public string filter = "";
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
    public class ContentResultList
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
        [JsonProperty("type")]
        public int type = 0;
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

    public class CBHandler : IHttpHandler
    {
        private int contentPageSize = 8;
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
                        case "search":
                            response = search();
                            break;
                        case "getchildfolders":
                            response = getchildfolders();
                            break;
                        case "getfoldercontent":
                            response = getfoldercontent();
                            break;
                        case "getchildtaxonomy":
                            response = getchildtaxonomy();
                            break;
                        case "gettaxonomycontent":
                            response = gettaxonomycontent();
                            break;
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

        public string search()
        {
            ContentResultList content = new ContentResultList();
            int resultcount = 0;
            int type = (request.filter == "content") ? 1 : ((request.filter == "forms") ? 2 : 104);

            Ektron.Cms.API.Search.SearchManager sm = new Ektron.Cms.API.Search.SearchManager();
            SearchRequestData srd = new SearchRequestData();
            srd.FolderID = 0;
            srd.LanguageID = capi.ContentLanguage;
            srd.MaxResults = 900;
            srd.PageSize = 100;
            srd.SearchText = request.searchText;
            srd.Recursive = true;
            if (type == 1 || type == 2)
            {
                srd.SearchFor = SearchDocumentType.html;
            }
            else if (type == 104)
            {
                srd.SearchFor = SearchDocumentType.multiMedia;
            }
            else
            {
                srd.SearchFor = SearchDocumentType.all;
            }
            srd.SearchObjectType = SearchForType.Content;
            srd.SearchReturnType = WebSearchResultType.dataTable;
            srd.CurrentPage = 1;
            SearchResponseData[] results = sm.Search(srd, _context, ref resultcount);
            
            string contentids = "";
            foreach (SearchResponseData s in results)
            {
                if (s.ContentType == type)
                { contentids += "," + s.ContentID; }
            }
            
            Ektron.Cms.Common.ContentRequest req = new Ektron.Cms.Common.ContentRequest();           
            req.ContentType = Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes;
            req.GetHtml = false;
            req.Ids = contentids;
            req.MaxNumber = 100;
            req.RetrieveSummary = false;
            Ektron.Cms.Common.ContentResult res = capi.LoadContentByIds(ref req, null);

            List<Ektron.Cms.Common.ContentBase> items = new List<Ektron.Cms.Common.ContentBase>();
            foreach (Ektron.Cms.Common.ContentBase t in res.Item)
            {
                if (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                    items.Add(t);
            }

            content.Items = items.Count;
            content.Pages = (items.Count / contentPageSize) + (((items.Count % contentPageSize) > 0) ? 1 : 0);

            if (request.page > content.Pages - 1) request.page = content.Pages - 1;
            if (request.page < 0) request.page = 0;

            int startindex = request.page * contentPageSize;
            int endindex = startindex + contentPageSize;
            if (endindex > content.Items) endindex = content.Items;

            for (int i = startindex; i < endindex; i++)
            {
                ContentResult my = new ContentResult();
                my.FolderID = items[i].FolderId;
                my.Id = items[i].Id;
                my.Title = items[i].Title;
                content.Contents.Add(my);
            }
            content.PagingLinks = MakePagingLinks(request, content);

            return JsonConvert.SerializeObject(content);
        }

        public string getchildfolders()
        {
            DirectoryList directoryInfo = new DirectoryList();
            Ektron.Cms.PermissionData _permissionData = new PermissionData();
            long folid = 0;
            if (request.objectType == "folder" && request.objectID > -1)
            {
                //Folder fol = new Folder();
                FolderData[] fd = capi.GetChildFolders(request.objectID, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);

                if (fd != null && fd.Length > 0)
                {
                    foreach (FolderData f in fd)
                    {
                        DirectoryResult mytd = new DirectoryResult();
                        if (f.FolderType != 9)
                        {
                            mytd.Name = f.Name;
                            mytd.id = f.Id;
                            mytd.HasChildren = f.HasChildren;
                            mytd.type = f.FolderType;
                            _permissionData = capi.LoadPermissions(f.Id, "folder", ContentAPI.PermissionResultType.All);
                            mytd.CanAdd = _permissionData.CanAdd;
                            directoryInfo.SubDirectories.Add(mytd);
                        }
                    }
                }
                return JsonConvert.SerializeObject(directoryInfo);
            }
            return "";
        }

        public string getfoldercontent()
        {
            ContentResultList results = new ContentResultList();

            if (request.objectType == "folder" && request.objectID > -1)
            {
                ContentAPI c = new ContentAPI();
                long type = (request.filter == "content") ? 1 : ((request.filter == "forms") ? 2 : 104);

                Microsoft.VisualBasic.Collection pagedata = new Microsoft.VisualBasic.Collection();
                pagedata.Add(request.objectID, "FolderID", null, null);
                pagedata.Add("title", "OrderBy", null, null);
                pagedata.Add(capi.RequestInformationRef.ContentLanguage, "m_intContentLanguage", null, null);
                pagedata.Add(type, "ContentType", null, null);

                int pages = 0;
                Ektron.Cms.Common.EkContentCol ekc = capi.EkContentRef.GetAllViewableChildContentInfoV5_0(pagedata, 0, 900, ref pages);

                if (ekc != null && ekc.Count > 0)
                {
                    List<Ektron.Cms.Common.ContentBase> items = new List<Ektron.Cms.Common.ContentBase>();
                    foreach (Ektron.Cms.Common.ContentBase t in ekc)
                    {
                        if (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
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

        public string getchildtaxonomy()
        {
            DirectoryList directoryInfo = new DirectoryList();

            if (request.objectType == "taxonomy" && request.objectID > -1)
            {
                TaxonomyRequest taxrequest = new TaxonomyRequest();
                taxrequest.Depth = 1;
                taxrequest.IncludeItems = false;
                taxrequest.PageSize = 1000;
                taxrequest.TaxonomyId = request.objectID;
                taxrequest.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
                taxrequest.TaxonomyLanguage = capi.RequestInformationRef.ContentLanguage;
                taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;

                TaxonomyBaseData[] tbd = this.capi.EkContentRef.ReadAllSubCategories(taxrequest);
                StringBuilder output = new StringBuilder();
                if (tbd != null && tbd.Length > 0)
                {
                    for(int i=0; i<tbd.Length; i++)
                    {
                        output.Append("<li class=\"closed");
                        if (i == tbd.Length - 1)
                        {
                            output.Append(" last");
                        }
                        output.Append("\"><span class=\"folder\" data-ektron-taxid=\"");
                        output.Append(tbd[i].TaxonomyId.ToString());
                        output.Append("\">");
                        output.Append(tbd[i].TaxonomyName);
                        output.Append("</span>");
                        if (tbd[i].TaxonomyHasChildren)
                        {
                            output.Append("<ul data-ektron-taxid=\"");
                            output.Append(tbd[i].TaxonomyId.ToString());
                            output.Append("\"></ul>");
                        }
                        output.Append("</li>");
                    }
                }
                return output.ToString();
            }
            return "";
        }

        public string gettaxonomycontent()
        {
            ContentResultList results = new ContentResultList();

            if (request.objectType == "taxonomy" && request.objectID > -1)
            {
                TaxonomyRequest taxrequest = new TaxonomyRequest();
                taxrequest.Depth = 2;
                taxrequest.IncludeItems = true;
                taxrequest.PageSize = 1000;
                taxrequest.TaxonomyId = request.objectID;
                taxrequest.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
                taxrequest.TaxonomyLanguage = capi.RequestInformationRef.ContentLanguage;
                taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
                TaxonomyData taxdata = capi.LoadTaxonomy(ref taxrequest);

                int contenttype = (request.filter == "content") ? 1 : (request.filter == "forms") ? 2 : 104;

                if (taxdata != null)
                {
                    List<TaxonomyItemData> l = new List<TaxonomyItemData>();
                    l.AddRange(taxdata.TaxonomyItems);
                    l = l.FindAll(delegate(TaxonomyItemData t)
                    {
                        return (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                            && (contenttype > 0) ? t.TaxonomyItemType == contenttype : t.TaxonomyItemType > 2;
                    });

                    results.Items = l.Count;
                    results.Pages = l.Count / contentPageSize + ((l.Count % contentPageSize > 0) ? 1 : 0);

                    if (request.page > results.Pages - 1) request.page = results.Pages - 1;
                    if (request.page < 0) request.page = 0;

                    int startindex = request.page * contentPageSize;
                    int endindex = startindex + contentPageSize;
                    if (endindex > results.Items) endindex = results.Items;

                    for (int i = startindex; i < endindex; i++)
                    {
                        ContentResult my = new ContentResult();
                        my.FolderID = l[i].TaxonomyItemFolderId;
                        my.Id = l[i].TaxonomyItemId;
                        my.Title = l[i].TaxonomyItemTitle;
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
                        catch (Exception e)
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

        public string MakePagingLinks(Request req, ContentResultList res)
        {
            string onclick = "return Ektron.PFWidgets.ContentBlock.getResults('" + req.action + "', " + req.objectID.ToString() + ", {0}, '" + req.objectType + "', '" + req.searchText + "',this)";
            string linkformat = "<a href=\"#\" onclick=\"" + onclick + "\">{1}</a> ";
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
    }
}
