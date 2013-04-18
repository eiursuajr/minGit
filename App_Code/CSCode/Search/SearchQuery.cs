using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.WebSearch;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.WebSearch.SearchQueryTypes;
using Ektron.Cms.WebSearch.SearchQueryTypes.MSIndex;

/// <summary>
/// Summary description for SearchQuery
/// </summary>
public class SearchQuery 
{

    public SearchQuery(SearchRequestData requestData)
    {
        if (requestData.ProviderName == Ektron.Cms.WebSearch.Utils.SQLIndexProvider)
        {
            _query = new MSIndexQuery(requestData,HttpContext.Current );
        }
        else
        {
            _query = new MSIndexDialectQuery(requestData);
        }

    }
    private IQuery _query;

    public IQuery Query
    {
        get { return _query; }
        set { _query = value; }
    }



    public string Columns
    {
        set 
        {
            Query.SelectStatement =  value;
         }
    }
	
	
}
