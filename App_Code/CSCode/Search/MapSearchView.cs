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
    public sealed class MapSearchView : BaseSearchView
    {
        public MapSearchView(SearchDataSource owner, string viewName)
            : base(owner, viewName)
        {
        }
        //Note: Note: MapSearchView only works with SQL provider since Dialect Provider always returns paged results
        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);

            SearchRequestData requestData = _owner.GetSearchRequestData();
            if (requestData == null || (((requestData.SearchText == null) || (requestData.SearchText == "")) && (requestData.MetaDataNameList == null || requestData.MetaDataNameList.Length == 0)))
            {
                return null;
            }
            requestData.ProviderName = Ektron.Cms.WebSearch.Utils.SQLIndexProvider;
            SearchQuery searchQ = new SearchQuery(requestData );
            searchQ.Columns = " ContentID64,ContentLanguage,FolderID64,ContentType,EDescription,DocTitle,QuickLink,MapLongitude,MapLatitude,MapAddress,Description,TaxCategory ";

            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query , HttpContext.Current, ref resultCount,ref suggestedResultTable );

            //Make search results according to request
            return MakeSearchResponseData(table);
        }

        private IEnumerable MakeSearchResponseData(DataTable table)
        {
            List<MapViewSearchData> searchResponseDataList = new List<MapViewSearchData>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(table.Rows[i]["ContentID64"]))
                {
                    MapViewSearchData searchViewData = new MapViewSearchData();
                    if (!Convert.IsDBNull(table.Rows[i]["EDescription"])) 
                    searchViewData.DocSubject = (string)table.Rows[i]["EDescription"];
                    if (!Convert.IsDBNull(table.Rows[i]["TaxCategory"])) 
                        searchViewData.Category = (string)table.Rows[i]["TaxCategory"];
                    if (!Convert.IsDBNull(table.Rows[i]["Description"])) 
                        searchViewData.Description = (string)table.Rows[i]["Description"];
                    if (!Convert.IsDBNull(table.Rows[i]["DocTitle"])) 
                        searchViewData.DocTitle = (string)table.Rows[i]["DocTitle"];
                    if (!Convert.IsDBNull(table.Rows[i]["MapAddress"])) 
                        searchViewData.MapAddress = (string)table.Rows[i]["MapAddress"];
                    if (!Convert.IsDBNull(table.Rows[i]["MapLatitude"])) 
                        searchViewData.MapLatitude = (double)table.Rows[i]["MapLatitude"];
                    if (!Convert.IsDBNull(table.Rows[i]["MapLongitude"])) 
                        searchViewData.MapLongitude = (double)table.Rows[i]["MapLongitude"];
                    if (!Convert.IsDBNull(table.Rows[i]["QuickLink"])) 
                        searchViewData.QuickLink = (string)table.Rows[i]["QuickLink"];
                    searchResponseDataList.Add(searchViewData);
                }
            }
            return searchResponseDataList.ToArray();

        }

        
    }

    public class MapViewSearchData
    {
        private double _mapLongitude;

        public double MapLongitude
        {
            get { return _mapLongitude; }
            set { _mapLongitude = value; }
        }
        private double _mapLatitude;

        public double MapLatitude
        {
            get { return _mapLatitude; }
            set { _mapLatitude = value; }
        }
        private string _mapAddress;

        public string MapAddress
        {
            get { return _mapAddress; }
            set { _mapAddress = value; }
        }
        private string _docSubject;

        public string DocSubject
        {
            get { return _docSubject; }
            set { _docSubject = value; }
        }
        private string _docTitle;

        public string DocTitle
        {
            get { return _docTitle; }
            set { _docTitle = value; }
        }
        private string _quickLink;

        public string QuickLink
        {
            get { return _quickLink; }
            set { _quickLink = value; }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _category;

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
	 }
    
}