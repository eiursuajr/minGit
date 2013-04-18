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

namespace SampleSearch.Views
{
    public sealed class ContentSearchView : BaseSearchView
    {
        public ContentSearchView(SearchDataSource owner, string viewName)
            : base(owner, viewName)
        {

        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);

            SearchRequestData requestData = _owner.GetSearchRequestData();
            if (requestData == null || (((requestData.SearchText == null) || (requestData.SearchText == "")) && (requestData.MetaDataNameList == null || requestData.MetaDataNameList.Length == 0)))
            {
                return null;
            }



            requestData.ProviderName = Ektron.Cms.WebSearch.Utils.SQLIndexProvider;

            //Make query
            SearchQuery searchQ = new SearchQuery(requestData);
            searchQ.Columns = "ContentID64,ContentLanguage,FolderID64,ContentType";
            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query , HttpContext.Current, ref resultCount,ref suggestedResultTable );

            _owner.ResultCount = resultCount;
            //Make search results according to request
            return (IEnumerable)MakeSearchResponseData(table);
        }

        private IEnumerable MakeSearchResponseData(DataTable table)
        {
            List<ContentViewSearchData> searchResponseDataList = new List<ContentViewSearchData>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(table.Rows[i]["ContentID64"]))
                {
                    ContentViewSearchData contentViewData = new ContentViewSearchData();
                    contentViewData.ContentId = (long)table.Rows[i]["ContentID64"];
                    contentViewData.ContentLanguage = (int)table.Rows[i]["ContentLanguage"];
                    contentViewData.FolderId = (long)table.Rows[i]["FolderID64"];
                    searchResponseDataList.Add(contentViewData);
                }
            }
            return searchResponseDataList.ToArray();

        }

       
    }

    public class ContentViewSearchData
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
	

    }
}

