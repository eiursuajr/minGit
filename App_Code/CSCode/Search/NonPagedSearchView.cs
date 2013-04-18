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
    public sealed class NonPagedSearchView : BaseSearchView
    {
        string _appPath;
        string _sitePath;
        public NonPagedSearchView(SearchDataSource owner, string viewName)
            : base(owner, viewName)
        {
            _sitePath = System.Configuration.ConfigurationManager.AppSettings["ek_appPath"];
            if (_sitePath == "")
                _sitePath = "/WorkArea/";

            if (!_sitePath.EndsWith("/"))
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
            searchQ.Columns = " ContentID64,ContentLanguage,FolderID64,ContentType,DocTitle,QuickLink ";
            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query, HttpContext.Current, ref resultCount, ref suggestedResultTable);

            return (IEnumerable)MakeSearchResponseData(table,requestData.SearchText  );
        }

        private IEnumerable MakeSearchResponseData(DataTable table,string searchText)
        {
            List<NonPagedViewSearchData> searchResponseDataList = new List<NonPagedViewSearchData>();
            string strLinkIdentifier;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(table.Rows[i]["ContentID64"]))
                {
                    NonPagedViewSearchData nonPagedViewData = new NonPagedViewSearchData();
                    nonPagedViewData.ContentId = (long)table.Rows[i]["ContentID"];
                    nonPagedViewData.ContentLanguage = (int)table.Rows[i]["ContentLanguage"];
                    nonPagedViewData.ContentType = (int)table.Rows[i]["ContentType"];
                    nonPagedViewData.FolderId = (long)table.Rows[i]["FolderID64"];
                    nonPagedViewData.Title = (string)table.Rows[i]["DocTitle"];
                    if (nonPagedViewData.ContentType == 2)
                        strLinkIdentifier = "ekfrm";
                    else
                        strLinkIdentifier = "id";

                    if (nonPagedViewData.ContentType == 1 ||
                        nonPagedViewData.ContentType == 2 ||
                        nonPagedViewData.ContentType == 105)
                    {
                        if (Convert.IsDBNull(table.Rows[i]["QuickLink"]) || (table.Rows[i]["QuickLink"] != null && table.Rows[i]["QuickLink"].ToString().Contains("linkit.aspx")))
                            nonPagedViewData.QuickLink = _appPath + _sitePath + "linkit.aspx?LinkIdentifier=" + strLinkIdentifier + "&ItemID=" + table.Rows[i]["ContentID64"] + "&FolderID64=" + table.Rows[i]["FolderID64"] + "&terms=" + EkFunctions.UrlEncode(searchText);
                        else
                        {
                            nonPagedViewData.QuickLink = (string)table.Rows[i]["QuickLink"];
                            if (nonPagedViewData.QuickLink.IndexOf("?") == -1)
                                nonPagedViewData.QuickLink = nonPagedViewData.QuickLink + "?terms=" + EkFunctions.UrlEncode(searchText.Replace("''", "'")); //& "&searchtype=" & oSR.SearchType & "&fragment=" & oSR.AllowFragments
                            else
                                nonPagedViewData.QuickLink = nonPagedViewData.QuickLink + "&terms=" + EkFunctions.UrlEncode(searchText.Replace("''", "'")); //& "&searchtype=" & oSR.SearchType & "&fragment=" & oSR.AllowFragments
                        }

                        //if (strTarget == "")
                        nonPagedViewData.QuickLink = "href=\"" + nonPagedViewData.QuickLink;
                        //else
                        //link = "href=\"" + link + "\" " + strTarget;

                    }
                    else if (!Convert.IsDBNull(table.Rows[i]["QuickLink"]))
                    {
                        nonPagedViewData.QuickLink = (string)table.Rows[i]["QuickLink"];

                        if (((string)table.Rows[i]["QuickLink"]).IndexOf("javascript:") > -1)
                        {
                            nonPagedViewData.QuickLink = "target=\"_self\" href=\"#\" onClick=\"" + nonPagedViewData.QuickLink + "\";return false";
                        }
                        else
                        {
                            nonPagedViewData.QuickLink = "href=\"" + nonPagedViewData.QuickLink;
                        }
                    }
                    nonPagedViewData.QuickLink = string.Format("<a class=l {0}\">{1}</a>", nonPagedViewData.QuickLink, nonPagedViewData.Title);
                    searchResponseDataList.Add(nonPagedViewData);
                }
            }
            return searchResponseDataList.ToArray();

        }

        internal class NonPagedSearchQuery : Query
        {

            internal NonPagedSearchQuery(SearchRequestData requestData):base(requestData,HttpContext.Current)
            {
                if (requestData.ProviderName == Ektron.Cms.WebSearch.Utils.SQLIndexProvider)
                    SelectStatement = @" SELECT  ContentID64,ContentLanguage,ContentType,FolderID64,QuickLink,DocTitle	FROM SCOPE('""{0}""') WHERE  ";
                else if (requestData.ProviderName == Ektron.Cms.WebSearch.Utils.DialectIndexProvider)
                    SelectStatement = " ContentID64,ContentLanguage,FolderID64,ContentType,QuickLink,DocTitle ";

                MakeProviderDataForQuery(requestData);
            }
        }

        public class NonPagedViewSearchData
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
	            get { return _title;}
	            set { _title = value;}
            }
	

	
        }
    }


}