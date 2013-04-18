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
    public sealed class SimpleSearchView : BaseSearchView
    {
        public SimpleSearchView(SearchDataSource owner, string viewName)
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
            SearchQuery searchQ = new SearchQuery(requestData);
            searchQ.Columns = "ContentID64,ContentLanguage,FolderID64,ContentType";
            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query , HttpContext.Current, ref resultCount,ref suggestedResultTable );

            //Make search results according to request
            return (IEnumerable)MakeSearchResponseData(table);
        }

        private IEnumerable MakeSearchResponseData(DataTable table)
        {
            List<SimpleViewSearchData> searchResponseDataList = new List<SimpleViewSearchData>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(table.Rows[i]["ContentID64"]))
                {
                    SimpleViewSearchData searchViewData = new SimpleViewSearchData();
                    searchViewData.ContentId = (long)table.Rows[i]["ContentID64"];
                    searchResponseDataList.Add(searchViewData); 
                }
            }
            return searchResponseDataList.ToArray(); 

        }

      
    }

    public class SimpleViewSearchData
    {
        private long _contentId;

        public long ContentId
        {
            get { return _contentId; }
            set { _contentId = value; }
        }
       
    }


}