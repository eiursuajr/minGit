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

namespace SampleSearch.Views
{
    public sealed class DefaultSearchView : BaseSearchView
    {

        public DefaultSearchView(SearchDataSource owner, string viewName)
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
            searchQ.Columns = " Characterization,DocLastAuthor,EDescription,DocTitle,FileName,CMSSize,DateCreated,DateModified,ContentID64,ContentLanguage,FolderID64,QuickLink,ContentType,TaxCategory	 ";

            DataTable suggestedResultTable = new DataTable();
            DataTable table = Manager.Providers[requestData.ProviderName].Search(requestData, searchQ.Query , HttpContext.Current, ref resultCount,ref suggestedResultTable );

            return (IEnumerable)table;
            //Make search results according to request
        }

       

    }

}
