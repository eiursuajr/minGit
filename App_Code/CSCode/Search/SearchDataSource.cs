// SimpleDataSource.cs
//

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.UI.Design.WebControls;
using Ektron.Cms.WebSearch.SearchData;
using System.Reflection;
using Ektron.Cms.WebSearch;

namespace SampleSearch.Views
{
    public enum SearchViews
    {
        Simple,
        Default,
        Content,
        Map,
        Taxonomy,
        NonPaged,
        MetaData,
        Paged
    }

    [
    DefaultProperty("Parameters"),
    //Designer(typeof(SimpleDataDesigner), typeof(IDesigner)),
    ParseChildren(true),
    PersistChildren(false)
    ]
    public class SearchDataSource : DataSourceControl
    {
        //private SearchRequestData _requestData;
        private bool _enablePaging;
        private int _currentPage;
        private string _category;
        private long _folderId;
        private int _languageId;
        private WebSearchOrder _orderby;
        private WebSearchOrderByDirection _orderbyDir;
        private bool _recursive;
        private SearchDocumentType _searchFor;
        private WebSearchResultType _resultType;
        private string _searchText;

        private BaseSearchView _searchView;
        private ParameterCollection _parameters;
        private SearchViews _viewName;

        public SearchDataSource()
        {
            _enablePaging = true;
            _currentPage = 0;
            _category = "";
            _folderId = 0;
            _languageId = 1033;
            _orderby = WebSearchOrder.Rank;
            _orderbyDir = WebSearchOrderByDirection.Descending;
            _recursive = true;
            _searchFor = SearchDocumentType.all;
            _resultType = WebSearchResultType.html;
            _searchText = "";
            _viewName = SearchViews.Simple ;
        }

        [
        DefaultValue(null),
        //Editor(typeof(ParameterCollectionEditor), typeof(UITypeEditor)),
        MergableProperty(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        Category("Data")
        ]
        public ParameterCollection Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new ParameterCollection();
                    _parameters.ParametersChanged += new EventHandler(this.OnParametersChanged);

                    if (IsTrackingViewState)
                    {
                        ((IStateManager)_parameters).TrackViewState();
                    }
                }
                return _parameters;
            }
        }

        [
      DefaultValue(SearchViews.Simple ),
      MergableProperty(false),
      Category("Data")
      ]
        public SearchViews ViewName
        {
            get
            {
                if(ViewState["ViewName"] != null)
                    _viewName = (SearchViews)ViewState["ViewName"];
                return _viewName; 
            }
            set
            {
                if (_viewName != value)
                {
                    ViewState["ViewName"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        private BaseSearchView CurrentSearchView
        {
            get
            {
                if (_searchView == null)
                {
                    switch (ViewName)
                    {
                        case SearchViews.Simple:
                            _searchView = new SimpleSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Content:
                            _searchView = new ContentSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Default:
                            _searchView = new DefaultSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Map:
                            _searchView = new MapSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Taxonomy:
                            _searchView = new TaxonomySearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.NonPaged:
                            _searchView = new NonPagedSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.MetaData:
                            _searchView = new MetaDataSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Paged:
                            _searchView = new PagedSearchView(this, _viewName.ToString());
                            break;
                    }
                    if(_searchView == null)
                        _searchView = new SimpleSearchView(this, ViewName.ToString());
                    
                }

                return _searchView;
            }
        }



        private BaseSearchView SearchView
        {
            get
            {
                if (_searchView == null)
                {
                    switch ( ViewName)
                    {
                        case SearchViews.Simple :
                            _searchView = new SimpleSearchView(this,_viewName.ToString()  );
                            break;
                        case SearchViews.Content :
                            _searchView = new ContentSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Default :
                            _searchView = new DefaultSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Map :
                            _searchView = new MapSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.Taxonomy :
                            _searchView = new TaxonomySearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.NonPaged :
                            _searchView = new NonPagedSearchView(this, _viewName.ToString());
                            break;
                        case SearchViews.MetaData :
                            _searchView = new MetaDataSearchView(this, _viewName.ToString());
                            break;
                    }
                }

                return _searchView;
            }
        }

        //private bool _enablePaging;
        [
        DefaultValue(true),
        MergableProperty(false),
        Category("Data")
        ]
        public bool EnablePaging
        {
            get
            {
                if(ViewState["EnablePaging"] != null)
                _enablePaging = (bool)ViewState["EnablePaging"];
                return _enablePaging;
            }
            set
            {
                if (_enablePaging != value)
                {
                    ViewState["EnablePaging"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }
        
        //private int _currentPage;
        
        [
        DefaultValue(0),
        MergableProperty(false),
        Category("Data")
        ]
        public int CurrentPage
        {
            get
            {
                if(ViewState["CurrentPage"] != null)
                _currentPage = (int)ViewState["CurrentPage"];
                return _currentPage;
            }
            set
            {
                if (_currentPage != value)
                {
                    ViewState["CurrentPage"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private string _category;
       
        [
        DefaultValue(""),
        MergableProperty(false),
        Category("Data")
        ]
        public string Category
        {
            get
            {
                _category = (string)ViewState["Category"];
                return (_category != null) ? _category : String.Empty;
            }
            set
            {
                if (String.Compare(value, _category, StringComparison.Ordinal) != 0)
                {
                    ViewState["Category"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private int _folderID;
        [
        DefaultValue(0),
        MergableProperty(false),
        Category("Data")
        ]
        public long FolderId
        {
            get
            {
                if(ViewState["FolderId"] != null)
                _folderId = (long)ViewState["FolderId"];
                return _folderId;
            }
            set
            {
                if (_folderId != value)
                {
                    ViewState["FolderId"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private int _languageID;
        [
        DefaultValue(1033),
        MergableProperty(false),
        Category("Data")
        ]
        public int LanguageId
        {
            get
            {
                if(ViewState["LanguageId"] != null)
                _languageId = (int)ViewState["LanguageId"];
                return _languageId;
            }
            set
            {
                if (value != _languageId)
                {
                    ViewState["LanguageId"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private WebSearchOrder _orderby;
        
        [
        DefaultValue(WebSearchOrder.Rank ),
        MergableProperty(false),
        Category("Data")
        ]
        public WebSearchOrder OrderBy
        {
            get
            {
                if(ViewState["OrderBy"] != null)
                _orderby = (WebSearchOrder)ViewState["OrderBy"];
                return _orderby;
            }
            set
            {
                if (_orderby != value)
                {
                    ViewState["OrderBy"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private WebSearchOrderByDirection _orderbyDir;
        [
        DefaultValue(WebSearchOrderByDirection.Descending ),
        MergableProperty(false),
        Category("Data")
        ]
        public WebSearchOrderByDirection OrderByDirection
        {
            get
            {
                if(ViewState["OrderByDirection"] != null)
                _orderbyDir = (WebSearchOrderByDirection)ViewState["OrderByDirection"];
                return _orderbyDir;
            }
            set
            {
                if (_orderbyDir != value)
                {
                    ViewState["OrderByDirection"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        //private bool _recursive;
        [
        DefaultValue(true),
        MergableProperty(false),
        Category("Data")
        ]
        public bool Recursive
        {
            get
            {
                if(ViewState["Recursive"] != null)
                _recursive = (bool)ViewState["Recursive"];
                return _recursive;
            }
            set
            {
                if (_recursive != value)
                {
                    ViewState["Recursive"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }
        //private WebSearchFor _searchFor;
        [
        DefaultValue(SearchDocumentType.all),
        MergableProperty(false),
        Category("Data")
        ]
        public SearchDocumentType DocumentType
        {
            get
            {
                if(ViewState["DocumentType"] != null)
                    _searchFor = (SearchDocumentType)ViewState["DocumentType"];
                return _searchFor;
            }
            set
            {
                if (_searchFor != value)
                {
                    ViewState["DocumentType"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }
        //private WebSearchResultType _resultType;
        [
        DefaultValue(WebSearchResultType.html),
        MergableProperty(false),
        Category("Data")
        ]
        public WebSearchResultType ResultType
        {
            get
            {
                if(ViewState["ResultType"] != null)
                _resultType = (WebSearchResultType)ViewState["ResultType"];
                return _resultType;
            }
            set
            {
                if (_resultType != value)
                {
                    ViewState["ResultType"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }
        //private string _searchText;
        [
         DefaultValue(""),
         MergableProperty(false),
         Category("Data")
         ]
        public string SearchText
        {
            get
            {
                _searchText = (string)ViewState["SearchText"];
                return (_searchText != null) ? _searchText : String.Empty;
            }
            set
            {
                if (String.Compare(value, _searchText, StringComparison.Ordinal) != 0)
                {
                    ViewState["SearchText"] = value;
                    CurrentSearchView.RaiseChangedEvent();
                }
            }
        }

        private int _resultCount;

        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }
	
        public const string EnablePagingName = "EnablePaging";
        public const string CurrentPageName = "CurrentPage";
        public const string CategoryName = "Category";
        public const string FolderIdName = "FolderId";
        public const string LanguageIdName = "LanguageId";
        public const string OrderByName = "OrderBy";
        public const string OrderDirectionName = "OrderDirection";
        public const string RecursiveName = "Recursive";
        public const string SearchForName = "SearchFor";
        public const string SearchResultTypeName = "ResultType";
        public const string SearchTextName = "SearchText";
        
        public SearchRequestData GetSearchRequestData()
        {
            SearchRequestData requestData = new SearchRequestData(); 
            if (_parameters != null)
            {
                IOrderedDictionary parameterValues = _parameters.GetValues(Context, this);
                                
                Parameter parameter = _parameters[EnablePagingName];
                if (parameter != null)
                    requestData.EnablePaging = (bool)parameterValues[parameter.Name];
                
                parameter = _parameters[CurrentPageName];
                if (parameter != null)
                    requestData.CurrentPage = (int)parameterValues[parameter.Name];
                
                parameter = _parameters[CategoryName];
                if (parameter != null)
                   requestData.Category = (string)parameterValues[parameter.Name];
                
                parameter = _parameters[FolderIdName];
                if (parameter != null)
                    requestData.FolderID = (long)parameterValues[parameter.Name];
                
                parameter = _parameters[OrderByName];
                if (parameter != null)
                    requestData.OrderBy = (WebSearchOrder)parameterValues[parameter.Name];
                
                parameter = _parameters[OrderDirectionName];
                if (parameter != null)
                    requestData.OrderDirection  = (WebSearchOrderByDirection)parameterValues[parameter.Name];

                parameter = _parameters[RecursiveName];
                if (parameter != null)
                    requestData.Recursive  = (bool)parameterValues[parameter.Name];

                parameter = _parameters[SearchForName];
                if (parameter != null)
                    requestData.SearchFor = (SearchDocumentType)parameterValues[parameter.Name];

                parameter = _parameters[SearchResultTypeName];
                if (parameter != null)
                    requestData.SearchReturnType  = (WebSearchResultType )parameterValues[parameter.Name];

                parameter = _parameters[SearchTextName];
                if (parameter != null)
                    requestData.SearchText  = (string)parameterValues[parameter.Name];
            }

            return requestData;
        }

        protected override DataSourceView  GetView(string viewName)
        {
            if (String.IsNullOrEmpty(viewName))
                return CurrentSearchView;
            SearchViews searchView = (SearchViews)StringToEnum(typeof(SearchViews),viewName);
            switch (searchView)
            {
                case SearchViews.Content:
                    return new ContentSearchView(this, viewName); 
                case SearchViews.Default:
                    return new DefaultSearchView(this, viewName); 
                case SearchViews.Map:
                    return new MapSearchView(this, viewName);  
                case SearchViews.MetaData:
                    return new MetaDataSearchView(this, viewName);  
                case SearchViews.NonPaged:
                    return new NonPagedSearchView(this, viewName);  
                case SearchViews.Simple:
                    return new SimpleSearchView(this, viewName);  
                case SearchViews.Taxonomy:
                    return new TaxonomySearchView(this, viewName);  
                default:
                    return new SimpleSearchView(this, viewName);  

            }
                        
        }

        protected override ICollection GetViewNames()
        {
            return new string[] {
                       SearchViews.Content.ToString(),
                       SearchViews.Default.ToString(),
                       SearchViews.Map.ToString(),
                       SearchViews.MetaData.ToString(),
                       SearchViews.NonPaged.ToString(),
                       SearchViews.Simple.ToString(),
                       SearchViews.Taxonomy.ToString()                  
                   };
        }

        protected override void LoadViewState(object savedState)
        {
            object baseState = null;

            if (savedState != null)
            {
                Pair p = (Pair)savedState;
                baseState = p.First;

                if (p.Second != null)
                {
                    ((IStateManager)Parameters).LoadViewState(p.Second);
                }
            }

            base.LoadViewState(baseState);
        }

        protected override void OnInit(EventArgs e)
        {
            Debug.Assert(Page != null);
            Page.LoadComplete += new EventHandler(this.OnPageLoadComplete);
        }

        private void OnPageLoadComplete(object sender, EventArgs e)
        {
            Parameters.UpdateValues(Context, this);
        }

        private void OnParametersChanged(object sender, EventArgs e)
        {
            CurrentSearchView.RaiseChangedEvent();
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object parameterState = null;

            if (_parameters != null)
            {
                parameterState = ((IStateManager)_parameters).SaveViewState();
            }

            if ((baseState != null) || (parameterState != null))
            {
                return new Pair(baseState, parameterState);
            }

            return null;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (_parameters != null)
            {
                ((IStateManager)_parameters).TrackViewState();
            }
        }

        /// <summary>
        /// Returns an Enumeration Object from string
        /// </summary>
        /// <param name="t"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static object StringToEnum(Type t, string Value)
        {
            foreach (FieldInfo fi in t.GetFields())
            {
                if (fi.Name.ToLower() == Value.ToLower())
                {
                    return fi.GetValue(null);
                }
            }
            return null;
        }
                
    }
}