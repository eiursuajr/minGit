using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.WebSearch;
using System.Collections;
using System.Collections.Generic;
using Ektron.Cms.Common;

namespace SampleSearch.Views
{
    public sealed class PagedSearchView : BaseSearchView
    {
        string _appPath;
        string _sitePath;
        public PagedSearchView(SearchDataSource owner, string viewName)
            : base(owner, viewName)
        {
            _sitePath = System.Configuration.ConfigurationManager.AppSettings["ek_appPath"];
            if(_sitePath == "")
                _sitePath = "/WorkArea/";

            if(!_sitePath.EndsWith("/")) 
                _sitePath += "/";
            _appPath = System.Configuration.ConfigurationManager.AppSettings["ek_sitePath"];
            if (_appPath == "")
                _appPath = "/";
         
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);

            SearchRequestData requestData = _owner.GetSearchRequestData();
            if (requestData == null || (((requestData.SearchText == null) || (requestData.SearchText == "")) && (requestData.MetaDataNameList == null || requestData.MetaDataNameList.Length == 0)))
            {
                return null;
            }

            requestData.EnablePaging = false;
            requestData.ProviderName = Ektron.Cms.WebSearch.Utils.SQLIndexProvider;

            SearchQuery searchQ = new SearchQuery(requestData);
            searchQ.Columns = "ContentID64,ContentLanguage,ContentType,FolderID64,QuickLink,DocTitle";

            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query , HttpContext.Current, ref resultCount,ref suggestedResultTable );

            return (IEnumerable)MakeSearchResponseData(table, requestData.SearchText);
        }

        private IEnumerable MakeSearchResponseData(DataTable table, string searchText)
        {
            List<PagedViewSearchData> searchResponseDataList = new List<PagedViewSearchData>();
            string strLinkIdentifier;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(table.Rows[i]["ContentID64"]))
                {
                    PagedViewSearchData PagedViewData = new PagedViewSearchData();
                    PagedViewData.ContentId = Convert.ToInt64(table.Rows[i]["ContentID64"]);
                    PagedViewData.ContentLanguage = (int)table.Rows[i]["ContentLanguage"];
                    PagedViewData.ContentType = (int)table.Rows[i]["ContentType"];
                    PagedViewData.FolderId = Convert.ToInt64(table.Rows[i]["FolderID64"]);
                    PagedViewData.Title = (string)table.Rows[i]["DocTitle"];
                    if (PagedViewData.ContentType == 2)
                        strLinkIdentifier = "ekfrm";
                    else
                        strLinkIdentifier = "id";

                    if (PagedViewData.ContentType == 1 ||
                        PagedViewData.ContentType == 2 ||
                        PagedViewData.ContentType == 105)
                    {
                        if (Convert.IsDBNull(table.Rows[i]["QuickLink"]) || (table.Rows[i]["QuickLink"] != null && table.Rows[i]["QuickLink"].ToString().Contains("linkit.aspx")))
                            PagedViewData.QuickLink = _appPath + _sitePath + "linkit.aspx?LinkIdentifier=" + strLinkIdentifier + "&ItemID=" + table.Rows[i]["ContentID64"] + "&FolderID64=" + table.Rows[i]["FolderID64"] + "&terms=" + EkFunctions.UrlEncode(searchText);
                        else
                        {
                            PagedViewData.QuickLink = (string)table.Rows[i]["QuickLink"];
                            if (PagedViewData.QuickLink.IndexOf("?") == -1)
                                PagedViewData.QuickLink = PagedViewData.QuickLink + "?terms=" + EkFunctions.UrlEncode(searchText.Replace("''", "'")); //& "&searchtype=" & oSR.SearchType & "&fragment=" & oSR.AllowFragments
                            else
                                PagedViewData.QuickLink = PagedViewData.QuickLink + "&terms=" + EkFunctions.UrlEncode(searchText.Replace("''", "'")); //& "&searchtype=" & oSR.SearchType & "&fragment=" & oSR.AllowFragments
                        }

                        //if (strTarget == "")
                        PagedViewData.QuickLink = "href=\"" + PagedViewData.QuickLink;
                        //else
                        //link = "href=\"" + link + "\" " + strTarget;

                    }
                    else if (!Convert.IsDBNull(table.Rows[i]["QuickLink"]))
                    {
                        PagedViewData.QuickLink = (string)table.Rows[i]["QuickLink"];

                        if (((string)table.Rows[i]["QuickLink"]).IndexOf("javascript:") > -1)
                        {
                            PagedViewData.QuickLink = "target=\"_self\" href=\"#\" onClick=\"" + PagedViewData.QuickLink + "\";return false";
                        }
                        else
                        {
                            PagedViewData.QuickLink = "href=\"" + PagedViewData.QuickLink;
                        }
                    }
                    //convert "&" in href to "&amp;" for XHTML compliance
                    PagedViewData.QuickLink = PagedViewData.QuickLink.Replace("&", "&amp;");
                    PagedViewData.QuickLink = string.Format("<a class=\"l\" {0}\">{1}</a>", PagedViewData.QuickLink, PagedViewData.Title);
                    searchResponseDataList.Add(PagedViewData);
                }
            }
            return searchResponseDataList.ToArray();

        }

        
        public class PagedViewSearchData
        {
            private long _contentId;

            public long ContentId
            {
                get { return _contentId; }
                set { _contentId = value; }
            }
            private int _contentLanguage;

            public int ContentLanguage
            {
                get { return _contentLanguage; }
                set { _contentLanguage = value; }
            }
            private long _folderId;

            public long FolderId
            {
                get { return _folderId; }
                set { _folderId = value; }
            }

            private string _quicklink;

            public string QuickLink
            {
                get { return _quicklink; }
                set { _quicklink = value; }
            }

            private int _contentType;

            public int ContentType
            {
                get { return _contentType; }
                set { _contentType = value; }
            }
            private string _title;

            public string Title
            {
                get { return _title; }
                set { _title = value; }
            }



        }
    }


}