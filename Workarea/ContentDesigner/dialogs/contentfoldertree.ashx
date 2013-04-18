<%@ WebHandler Language="C#" Class="foldertree" %>

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

#region JsonObject
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
        [JsonProperty("startFolder")]
        public long startFolder = 0;
        [JsonProperty("recursive")]
        public bool recursive = true;
    }

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
        [JsonProperty("idtype")]
        public string IdType = "";
        [JsonProperty("icon")]
        public string Icon = "";
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
    public class foldertree : IHttpHandler
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
				// _context.Response.Write("request=" + context.Request["request"].ToString() + "\n");
                if (context.Request["request"] != null)
                {
                    request = (Request)JsonConvert.DeserializeObject(context.Request["request"], typeof(Request));

					// _context.Response.Write("action=" + request.action + "\n");
					switch (request.action)
                    {
						case "search":
							response = search();
							break;
                        case "getchildfolders":
                            response = getchildfolders();
                            break;
						case "getfolderpath":
							response = getfolderpath();
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
                        case "gettaxonomypath":
                            response = gettaxonomypath();
                            break;
                        case "getcollectioncontent":
                            response = getcollectioncontent();
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
            //int type = (request.filter == "content") ? 1 : ((request.filter == "forms") ? 2 : 104);
            string sFilterBy = request.filter;

            string sFolderIds = ""; //only search on Content FolderType
            Ektron.Cms.Framework.Organization.FolderManager folderAPi = new Ektron.Cms.Framework.Organization.FolderManager();
            Ektron.Cms.FolderCriteria folderCriteria = new Ektron.Cms.FolderCriteria();
            folderCriteria.AddFilter(Ektron.Cms.Common.FolderProperty.FolderType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, (int)Ektron.Cms.Common.EkEnumeration.FolderType.Content);
            List<FolderData> folders = folderAPi.GetList(folderCriteria);
            foreach (FolderData f in folders)
            {
                sFolderIds += "," + f.Id;
            }
            
			Ektron.Cms.API.Search.SearchManager sm = new Ektron.Cms.API.Search.SearchManager();
			SearchRequestData srd = new SearchRequestData();
            srd.FolderID = 0; // request.startFolder;
			srd.LanguageID = capi.ContentLanguage;
            srd.MultipleFolders = sFolderIds;
            //srd.MaxResults = 900;
			srd.PageSize = 100;
			srd.SearchText = request.searchText;
            srd.Recursive = true; // request.recursive;
		    srd.SearchFor = SearchDocumentType.all;
			srd.SearchObjectType = SearchForType.Content;
			srd.SearchReturnType = WebSearchResultType.dataTable;
			srd.CurrentPage = 1;
			SearchResponseData[] results = sm.Search(srd, _context, ref resultcount);

			string contentids = "";
			foreach (SearchResponseData s in results) 
			{
                Regex rgx = new Regex(@"\b" + getContentTypeString(s.ContentType) + @"\b");
                if (rgx.IsMatch(sFilterBy))
				{
                    contentids += "," + s.ContentID;
				}
			}

			Ektron.Cms.Common.ContentRequest req = new Ektron.Cms.Common.ContentRequest();
			req.ContentType = Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes;
			req.GetHtml = false;
			req.Ids = contentids;
			req.MaxNumber = 100;
			req.RetrieveSummary = false;
			Ektron.Cms.Common.ContentResult res = capi.LoadContentByIds(ref req, null);

			List<Ektron.Cms.Common.ContentBase> items = new List<Ektron.Cms.Common.ContentBase>();
            Regex rgxSF = new Regex(@"\bcontent:smartform\b");
            foreach (Ektron.Cms.Common.ContentBase t in res.Item)
			{
                if ((t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
                    && (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData))
                {
                    if (0 == t.XMLCollectionID || (t.XMLCollectionID > 0 && rgxSF.IsMatch(sFilterBy)))
                        items.Add(t);
                }
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
                string mimeType = (items[i].XMLCollectionID > 0) ? "application/xml" : "text/html";
                string icon = "";
                if (items[i].AssetInfo != null)
                {
                    icon = items[i].AssetInfo.Icon;
                    mimeType = items[i].AssetInfo.MimeType;
                }
                my.IdType = getIdType(items[i].ContentType.ToString(), mimeType);
                //my.Icon = getContentIcon(my.IdType, icon);
                my.Icon = getContentTypeIconAspx(System.Convert.ToInt16(items[i].ContentType), System.Convert.ToInt16(items[i].ContentSubType), icon); 
				content.Contents.Add(my);
			}
			content.PagingLinks = MakePagingLinks(request, content);

			return JsonConvert.SerializeObject(content);
		}

        public string getchildfolders()
        {
            DirectoryList directoryInfo = new DirectoryList();

            //long folid = 0;
            if (request.objectType == "folder" && request.objectID > -1)
            {
                //Folder fol = new Folder();
                FolderData[] fd = capi.GetChildFolders(request.objectID, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);

                if (fd != null && fd.Length > 0)
                {
                    foreach (FolderData f in fd)
                    {
                        DirectoryResult mytd = new DirectoryResult();
                        if (0 == f.FolderType || 1 == f.FolderType || 2 == f.FolderType || 5 == f.FolderType || 9 == f.FolderType)//for content folderType(0), blog folderType(1), domain folderType(2), root folderType(5), catalog folderType(9)
                        {
                            mytd.Name = f.Name;
                            mytd.id = f.Id;
                            mytd.HasChildren = f.HasChildren;
                            mytd.type = f.FolderType;
                            directoryInfo.SubDirectories.Add(mytd);
                        }
                    }
                }
                return JsonConvert.SerializeObject(directoryInfo);
            }
            return "";
        }

		public string getfolderpath()
		{
			string strFolderPath = "";
            long folderid;
			if (request.objectType == "content" && request.objectID > -1)
			{
				ContentAPI capi = new ContentAPI();
				folderid = capi.GetFolderIdForContentId(request.objectID);
				strFolderPath = folderid.ToString();
				while (folderid != 0)
				{
					folderid = capi.GetParentIdByFolderId(folderid);
					if (folderid > 0) strFolderPath += "," + folderid.ToString();
				}
			}
            else if (request.objectType == "folder" && request.objectID > -1)
            {
                folderid = request.objectID;
                strFolderPath = folderid.ToString();
                while (folderid != 0)
                {
                    folderid = capi.GetParentIdByFolderId(folderid);
                    if (folderid > 0) strFolderPath += "," + folderid.ToString();
                }
            }
			return strFolderPath;
		}

        public string getfoldercontent()
        {
			ContentResultList results = new ContentResultList();

            if (request.objectType == "folder" && request.objectID > -1)
            {
                ContentAPI c = new ContentAPI();
                //long type = (request.filter == "content") ? 1 : ((request.filter == "forms") ? 2 : 104);
                string sFilterBy = request.filter;

                Microsoft.VisualBasic.Collection pagedata = new Microsoft.VisualBasic.Collection();
                pagedata.Add(request.objectID, "FolderID", null, null);
                pagedata.Add("title", "OrderBy", null, null);
                //pagedata.Add(capi.RequestInformationRef.ContentLanguage, "m_intContentLanguage", null, null);

                int pages = 0;
                Ektron.Cms.Common.EkContentCol ekc = capi.EkContentRef.GetAllViewableChildContentInfoV5_0(pagedata, 0, 900, ref pages);

                if (ekc != null && ekc.Count > 0)
                {
                    List<Ektron.Cms.Common.ContentBase> items = new List<Ektron.Cms.Common.ContentBase>();
                    Regex rgxSF = new Regex(@"\bcontent:smartform\b");
                    foreach (Ektron.Cms.Common.ContentBase t in ekc)
                    {
                        string contentType = getContentTypeString(getContentTypeIdx(t.ContentType.ToString()));
                        Regex rgx = new Regex(@"\b" + contentType + @"\b");
                        if ((t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
                            && (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) && rgx.IsMatch(sFilterBy))
                        {
                            if (0 == t.XMLCollectionID || (t.XMLCollectionID > 0 && rgxSF.IsMatch(sFilterBy)))
                                items.Add(t);
                        }
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
                        //my.IdType = getContentTypeString(getContentTypeIdx(items[i].ContentType.ToString()));
                        string mimeType = (items[i].XMLCollectionID > 0) ? "application/xml" : items[i].AssetInfo.MimeType;
                        my.IdType = getIdType(items[i].ContentType.ToString(), mimeType);
                        //my.Icon = getContentIcon(my.IdType, items[i].AssetInfo.Icon);
                        my.Icon = getContentTypeIconAspx(System.Convert.ToInt16(items[i].ContentType), System.Convert.ToInt16(items[i].ContentSubType), items[i].AssetInfo.Icon); 
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
				Taxonomy tax = new Taxonomy();
				TaxonomyRequest taxrequest = new TaxonomyRequest();
				taxrequest.Depth = 2;
				taxrequest.IncludeItems = false;
				taxrequest.PageSize = 1000;
				taxrequest.TaxonomyId = request.objectID;
				taxrequest.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
				taxrequest.TaxonomyLanguage = tax.RequestInformationRef.ContentLanguage;
				taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
				TaxonomyData taxdata = tax.LoadTaxonomy(ref taxrequest);

				if (taxdata != null && taxdata.HasChildren)
				{
					foreach (TaxonomyData td in taxdata.Taxonomy)
					{

						DirectoryResult mytd = new DirectoryResult();
						mytd.Name = td.Name;
						mytd.id = td.Id;
						mytd.HasChildren = td.HasChildren;
						directoryInfo.SubDirectories.Add(mytd);
					}
				}
				return JsonConvert.SerializeObject(directoryInfo);
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

				// _context.Response.Write("lang=" + capi.RequestInformationRef.ContentLanguage.ToString() + "\n");

                //int contenttype = (request.filter == "content") ? 1 : (request.filter == "forms") ? 2 : 104;
                string sFilterBy = request.filter; 

				// _context.Response.Write("objectType=" + request.objectType + " ID=" + request.objectID + " contenttype=" + contenttype.ToString() + "\n");
				// _context.Response.Write("taxdata=" + (taxdata == null ? "null" : taxdata.ToString()) + "\n");
				if (taxdata != null)
				{
					// _context.Response.Write("TaxonomyItemCount=" + taxdata.TaxonomyItemCount.ToString() + "\n");
					List<TaxonomyItemData> l = new List<TaxonomyItemData>();
					l.AddRange(taxdata.TaxonomyItems);
					// _context.Response.Write("pre findall l.Count=" + l.Count.ToString() + "\n");
					l = l.FindAll(delegate(TaxonomyItemData t)
					{
						// _context.Response.Write("FindAll t.TaxonomyItemType=" + t.TaxonomyItemType.ToString() + "\n");
                        Regex rgx = new Regex(@"\b" + getContentTypeString((int)t.ItemType) + @"\b");
                        return ((t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
                            && (t.ContentSubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                            && rgx.IsMatch(sFilterBy));
					});

					// _context.Response.Write("l.Count=" + l.Count.ToString() + "\n");
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
						my.FolderID = l[i].FolderId;
						my.Id = l[i].ItemId;
						my.Title = l[i].Title;
                        //my.IdType = getContentTypeString(l[i].TaxonomyItemType);
                        string mimeType = "text/html";
                        string icon = "";
                        if (l[i].AssetData != null)
                        {
                            icon = l[i].AssetData.Icon;
                            mimeType = l[i].AssetData.MimeType;
                        }
                        //my.Icon = getContentIcon(my.IdType, icon);
                        my.IdType = getIdType(getContentTypeString((int)l[i].ItemType), mimeType);
                        my.Icon = getContentTypeIconAspx((int)l[i].ItemType, System.Convert.ToInt16(l[i].ContentSubType), icon); 
						results.Contents.Add(my);
					}

					results.PagingLinks = MakePagingLinks(request, results);
				}
			}
			return JsonConvert.SerializeObject(results);

		}
        public string gettaxonomypath()
        {
            string strTaxonomyPath = "";
            string sTaxId;
            Ektron.Cms.Content.EkContent content = (new ContentAPI()).EkContentRef;
            TaxonomyBaseData[] TaxData = content.GetTaxonomyRecursiveToParent(request.objectID, System.Convert.ToInt16(this._context.Request.QueryString["LangType"]), 0);
            for (int i = 0; i < TaxData.Length; i++)
            {
                sTaxId = TaxData[i].Id.ToString();
                if (sTaxId.Length > 0)
                {
                    if (0 == i)
                    {
                        strTaxonomyPath = sTaxId;
                    }
                    else
                    {
                        strTaxonomyPath += "," + sTaxId;
                    }
                }
            }
            return strTaxonomyPath;
        }

        public string getcollectioncontent()
        {
            ContentResultList results = new ContentResultList();
            Ektron.Cms.Content.EkContent contentapi = (new ContentAPI()).EkContentRef;
            if (request.objectType == "collection" && request.objectID > -1)
            {
                PageRequestData p = null;
                Ektron.Cms.Common.ContentBase[] cb = contentapi.LoadCollectionItems(request.objectID, false, false, null, true, ref p);
                results.Items = cb.Length;
                results.Pages = (cb.Length / contentPageSize) + (((cb.Length % contentPageSize) > 0) ? 1 : 0);

                if (request.page > results.Pages - 1) request.page = results.Pages - 1;
                if (request.page < 0) request.page = 0;

                int startindex = request.page * contentPageSize;
                int endindex = startindex + contentPageSize;
                if (endindex > results.Items) endindex = results.Items;

                for (int i = startindex; i < endindex; i++)
                {
                    ContentResult my = new ContentResult();
                    my.FolderID = request.objectID;
                    my.Id = cb[i].Id;
                    my.Title = cb[i].Title;
                    string mimeType = (cb[i].XMLCollectionID > 0) ? "application/xml" : "text/html";
                    string icon = "";
                    if (cb[i].AssetInfo != null)
                    {
                        icon = cb[i].AssetInfo.Icon;
                        mimeType = cb[i].AssetInfo.MimeType;
                    }
                    my.IdType = getIdType(cb[i].ContentType.ToString(), mimeType);
                    my.Icon = getContentTypeIconAspx(System.Convert.ToInt16(cb[i].ContentType), System.Convert.ToInt16(cb[i].ContentSubType), icon);
                    results.Contents.Add(my);
                }
                results.PagingLinks = MakePagingLinks(request, results);
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
                            if (cb.AssetInfo != null && cb.AssetInfo.MimeType.Contains("image/"))
                            {
                                teasertext = "<img src=\"" + cb.AssetInfo.FileName + "\" alt=\"" + cb.Title + "\" height=\"100\" />";
                            }
                            else
                            {
                                System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
                                xd.LoadXml(cb.Teaser);
                                System.Xml.XmlNode xn = xd.SelectSingleNode("ektdesignpackage_forms/ektdesignpackage_form/ektdesignpackage_designs/ektdesignpackage_design");
                                if (xn != null)
                                {
                                    teasertext = xn.InnerXml;
                                }
                                else
                                {
                                    teasertext = cb.Teaser;
                                }
                            }
                        }
                        catch 
                        {
                            teasertext = cb.Teaser;
                        }
                        Ektron.Cms.Common.EkMessageHelper m_refMsg = null;
                        m_refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
                        sb.Append("<div class=\"contentDetails\"><div class=\"info\">");
                        sb.Append(m_refMsg.GetMessage("generic date label") + "&#160;");
                        sb.Append(cb.DisplayDateModified);
                        sb.Append("<br />");
                        sb.Append(m_refMsg.GetMessage("generic author label") + "&#160;");
                        sb.Append(cb.LastEditorFname + " " + cb.LastEditorLname);
                        sb.Append("</div>");
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
            string onclick = "return getResults('" + req.action + "', " + req.objectID.ToString() + ", {0}, '" + req.objectType + "', '" + req.searchText + "', this, '" + req.filter + "')";
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

        private string getContentTypeIconAspx(int ContentTypeID, int contentSubType, string imageUrl)
        {
            Microsoft.VisualBasic.Collection item = new Microsoft.VisualBasic.Collection();
            item.Add(contentSubType, "ContentSubType", null, null);
            item.Add(imageUrl, "ImageUrl", null, null);
            return Ektron.Cms.Common.EkFunctions.getContentTypeIconAspx(ContentTypeID, item, capi.ApplicationPath);
        }

        private string createIconImgTag(string imgsrc)
        {
            return "<img src=\"" + imgsrc + "\" width=\"16\" height=\"16\" border=\"0\" alt=\"" + System.Web.HttpContext.Current.Server.HtmlEncode(imgsrc) + "\" />";
        }

        private int getContentTypeIdx(string contentType)
        {
            int intContentType;
            switch (contentType.ToLower())
            {
                case "forms":
                    intContentType = 2;
                    break;
                case "assets":
                    intContentType = 8;
                    break;
                case "101":
                    intContentType = 101;
                    break;
                case "102":
                case "103":
                    intContentType = 102;
                    break;
                case "106":
                    intContentType = 106;
                    break;
                case "multimedia":
                    intContentType = 104;
                    break;
                case "product":
                    intContentType = 3333;
                    break;
                case "content":
                default:
                    intContentType = 1;
                    break;
            }
            return intContentType;
        }

        private string getIdType(string contentType, string mimeType)
        {
            string sIdType;
            switch (contentType)
            {
                case "forms":
                case "Forms":
                    sIdType = "content:htmlform";
                    break;
                case "asset":
                case "Asset":
                case "content:mso":  
                case "content:asset":  
                case "101":
                case "102":
                case "106":
                case "103":
                    sIdType = "content:asset";
                    if (mimeType.Contains("excel"))
                    {
                        sIdType = "content:mso:xls";
                    }
                    else if (mimeType.Contains("msword"))
                    {
                        sIdType = "content:mso:doc";
                    }
                    else if (mimeType.Contains("ms-powerpoint"))
                    {
                        sIdType = "content:mso:ppt";
                    }
                    else if (mimeType.Contains("application/"))
                    {
                        sIdType += ":" + mimeType.Replace("application/", "");
                    }
                    else if (mimeType.Contains("image"))
                    {
                        sIdType += ":" + mimeType.Replace("/", ":");
                    }
                    break;
                case "multimedia":
                case "Multimedia":
                case "content:multimedia":
                    sIdType = "content:multimedia";
                    break;
                case "product":
                case "Product":
                case "content:product":
                    sIdType = "content:product";
                    break;
                case "content":
                case "Content":
                case "content:htmlcontent":
                default:
                    if (mimeType.Contains("xml"))
                    {
                        sIdType = "content:smartform";
                    }
                    else
                    {
                        sIdType = "content:htmlcontent";
                    }
                    break;
            }
            return sIdType;
        }

        private string getContentTypeString(int contentType)
        {
            string sContentType;
            switch (contentType)
            {
                case 2:
                    sContentType = "content:htmlform";
                    break;
                case 101:
                    sContentType = "content:mso";
                    break;
                case 102:
                case 106:
                case 103:
                case 8:
                    sContentType = "content:asset";
                    break;
                case 104:
                    sContentType = "content:multimedia";
                    break;
                case 3333: //CatalogEntry 
                    sContentType = "content:product";
                    break;
                case 1:
                    sContentType = "content:htmlcontent";
                    break;
                case 7: //Ektron.Cms.Common.EkEnumeration.CMSContentType.LibraryItem
                    sContentType = "LibraryItem"; // not supported
                    break;
                case 3:  //Archive_Content
                case 4:  //Archive_Forms
                case 9:  //Archive_Assets    
                case 12: //Archive_Media
                case 99: //NonLibraryContent    
                case 111: //DiscussionTopic  
                default:
                    sContentType = "not supported types";
                    break;
            }
            return sContentType;   
        }
    }

