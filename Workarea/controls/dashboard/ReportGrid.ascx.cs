using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.User;
using Ektron.Cms.API;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Widget;
using Ektron.Cms.BusinessObjects.Localization;
using Ektron.Cms.API.Content;

public partial class Workarea_Widgets_Controls_ReportGrid : System.Web.UI.UserControl, ICallbackEventHandler
{
    protected string _Error = String.Empty; // Error variable to log when exceptions arise. Good for debugging.
    private string _callbackresult = null;
    private NameValueCollection PostBackData = null;
    private long _filterContentId = -1;
    protected ContentAPI contAPI = new ContentAPI();
    protected string shortLocaleCacheKey = "reportLanguageList";
    protected ILocalizationManager l10nManager = null;
    private string _controlname = null;

    #region Public properties
    #region Filters
    public long FilterFolderId { get; set; }
    public string FilterTitle { get; set; }
    public int FilterLocale { get; set; }
    public bool FilterLocaleNotIn { get; set; }
    public Ektron.Cms.Localization.LocalizationState FilterLocStatus { get; set; }
    public string FilterContentStatus { get; set; }
    public DateTime FilterLastModifiedStart { get; set; }
    public DateTime FilterLastModifiedEnd { get; set; }
    public DateTime FilterDateCreatedStart { get; set; }
    public DateTime FilterDateCreatedEnd { get; set; }
    public long FilterContentId
    {
        get
        {
            return _filterContentId;
        }
        set
        {
            _filterContentId = value;
        }
    }
    public long FilterAuthorId { get; set; }
    public int FilterLanguageId { get; set; }
    #endregion
    /// <summary>
    /// When set, contains the name of a client-side function to be called when the value of a filter
    /// has been set (useful for programmatically setting filter control values).  The function will
    /// automatically be called during callbacks where filter values are altered.
    /// 
    /// Usage: Function(filter, value)
    /// </summary>
    public string ClientSetFilterCallback { get; set; }
    /// <summary>
    /// When set, contains the name of a client-side function to be called during callbacks.  A true/false
    /// value will be passed to indicate a busy state.  It is recommended to count the calls in case 
    /// multiple callbacks are in progress.
    /// 
    /// Usage: Function(busy)
    /// </summary>
    public string ClientShowBusyDuringCallback { get; set; }
    /// <summary>
    /// When set, contains the name of a client-side function to be called when the checkbox selection
    /// changes on the grid.  This includes when selections are changed by changes to the filters.
    /// 
    /// Usage: Function(contentid, selected)
    ///     If "contentid" is null and "selected" is false, all selections have been cleared.
    /// </summary>
    public string ClientSelectionChanged { get; set; }
    public bool UseClientIDPrefix { get; set; }
    private List<long> _selectedItems = new List<long>();
    public List<long> SelectedItems { get { return _selectedItems; } private set { _selectedItems = value; } }
    public int ItemsPerPage { get; set; }
    public int CurrentPage { get; set; }
    public int ItemsOnThisPage { get; private set; }
    public int TotalItems { get; private set; }
    public int TotalPages { get; private set; }
    public bool IncludeFolderPath { get; set; }
    public int CurrentLanguageId
    {
        get
        {
            int language = contAPI.RequestInformationRef.ContentLanguage;
            if (contAPI.RequestInformationRef.ContentLanguage == EkConstants.CONTENT_LANGUAGES_UNDEFINED ||
                contAPI.RequestInformationRef.ContentLanguage == EkConstants.ALL_CONTENT_LANGUAGES)
            {
                language = contAPI.RequestInformationRef.DefaultContentLanguage;
            }
            return language;
        }
    }
    #endregion

    protected override void OnInit(EventArgs e)
    {
        l10nManager = ObjectFactory.GetLocalizationManager(contAPI.RequestInformationRef);
        // Set the control name dynamically
        _controlname = this.GetType().Name;

        if (!Page.IsPostBack && !Page.IsCallback)
            Cache.Remove(shortLocaleCacheKey);

        base.OnInit(e);

        AddCallback("test", CallbackTest);
        AddCallback("page", CallbackDoPaging);
        AddCallback("sort", CallbackDoSort);
        AddCallback("filter", CallbackDoFilter);
        AddCallback("action", CallbackDoAction);

        // DatePickerHD.MinimumDate = DateTime.Today;
        EkDTSelector dateSchedule;
        dateSchedule = contAPI.EkDTSelectorRef;
        dateSchedule.formName = "form1";
        Ektron.Cms.API.JS.RegisterJS(this, contAPI.AppPath + "/java/internCalendarDisplayFuncs.js", "EktronInternalCalendarDisplayJs");
        
        if (!base.DesignMode && !this.Page.IsCallback)
        {
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJSInclude(
                this,
                this.Page.ClientScript.GetWebResourceUrl(
                    typeof(Ektron.Cms.Controls.EkXsltWebPart),
                    "Ektron.Cms.Controls.ajax.js"),
                "AjaxScript");
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            /*Control parent = Page;
            while (parent.Parent != null)
                parent = parent.Parent;
            System.Web.UI.HtmlControls.HtmlHead htmlHead = null;
            foreach (Control c in parent.Controls)
                if (c is System.Web.UI.HtmlControls.HtmlHead)
                {
                    htmlHead = (System.Web.UI.HtmlControls.HtmlHead)c;
                    break;
                }
            if (htmlHead != null)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl scriptControl = 
                    (System.Web.UI.HtmlControls.HtmlGenericControl)htmlHead.FindControl("ReportGrid_HeadScriptControl");
                if (scriptControl == null)
                {
                    scriptControl = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
                    scriptControl.ID = "ReportGrid_HeadScriptControl";
                    htmlHead.Controls.Add(scriptControl);
                    scriptControl.Attributes.Add("language", "javascript");
                    scriptControl.InnerHtml = string.Empty;
                }
                scriptControl.InnerHtml += "/ * #test: success! " + this.UniqueID + " * /";
            }
            List<string> children = new List<string>();
            foreach (Control c in parent.Controls)
                children.Add(c.ID + ", " + c.GetType().Name);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.UniqueID + "_test", "/ * #test: " + this.UniqueID + ", " + parent.ID + ", " + parent.GetType().Name + " || " + string.Join("; ", children.ToArray()) + " * /", true);
            */
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ReportGrid.CallbackFunctions_Shared",
                //JS.RegisterJSBlock(this, 
                "function ReportGrid_Error(message, context) { alert('Error: ' + message); alert(context); " +
                (string.IsNullOrEmpty(ClientShowBusyDuringCallback) ? string.Empty : ((UseClientIDPrefix ? (this.ClientID + "_") : string.Empty) + ClientShowBusyDuringCallback + "(false);")) +
                " };" +
                "function ReportGrid_Select(sarray, value) {\n" +
                "    var array = sarray == '' ? new Array() : sarray.split(',');\n" +
                "    var found = false;\n" +
                "    for (var i = 0; i < array.length; i++)\n" +
                "        if (array[i] == value) {\n" +
                "            found = true;\n" +
                "            break;\n" +
                "        }\n" +
                "    if (!found)\n" +
                "        array.push(value);\n" +
                "    return array.join(',');\n" +
                "}\n" +
                "function ReportGrid_Deselect(sarray, value) {\n" +
                "    var array = new Array();\n" +
                "    var oarray = sarray.split(',');\n" +
                "    for (var i = 0; i < oarray.length; i++)\n" +
                "        if (oarray[i] != value)\n" +
                "            array.push(oarray[i]);\n" +
                "    return array.join(',');\n" +
                "}\n" +
                "function ReportGrid_ParseQuery(query) {\n" +
                "    var data1 = query.split('&');\n" +
                "    var data = new Array();\n" +
                "    var keys = new Array();\n" +
                "    for (var i = 0; i < data1.length; i++) {\n" +
                "        var vals = data1[i].split('=');\n" +
                "        if (vals[0] != null && vals[0] != '') {\n" +
                "            keys.push(vals[0]);\n" +
                "            data[vals[0]] = vals.length > 1 ? unescape(vals[1].replace('+',' ')) : null;\n" +
                "        }\n" +
                "    }\n" +
                "    data._keys = keys;\n" +
                "    \n" +
                "    return data;\n" +
                "}\n" +
                "function ReportGrid_BuildQuery(data) {\n" +
                "    var query = '';\n" +
                "    var keys = null;\n" +
                "    \n" +
                "    if (data._keys != null)\n" +
                "        keys = data._keys;\n" +
                "    else {\n" +
                "        keys = new Array();\n" +
                "        for (var i in data)\n" +
                "            keys.push(i);\n" +
                "    }\n" +
                "    \n" +
                "    for (var i = 0; i < keys.length; i++)\n" +
                "        query += (query.length > 0 ? '&' : '') +\n" +
                "            keys[i] + '=' +\n" +
                "            escape(data[keys[i]]);\n" +
                "    \n" +
                "    return query;\n" +
                "}\n" +
                "function ReportGrid_SetQueryValue(data, key, value) {\n" +
                "    if (data._keys == null) {\n" +
                "        data._keys = new Array();\n" +
                "        for (var i in data)\n" +
                "            data._keys.push(i);\n" +
                "    }\n" +
                "    var found = false;\n" +
                "    for (var i = 0; i < data._keys.length; i++)\n" +
                "        if (data._keys[i] == key) {\n" +
                "            found = true;\n" +
                "            data[key] = value;\n" +
                "            break;\n" +
                "        }\n" +
                "    if (!found) {\n" +
                "        data._keys.push(key);\n" +
                "        data[key] = value;\n" +
                "    }\n" +
                "}\n",
                true);
            //  "ReportGrid.CallbackFunctions_Shared");
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }
    protected override void OnPreRender(EventArgs e)
    {
        if (!base.DesignMode && !Page.IsCallback)
            JS.RegisterJSBlock(this, this.ClientID + "_UpdateParams(" + this.CurrentPage.ToString() + ", '" +
                this.GetSortStr(SortBy, SortOrder) + "', '');", this.ClientID + "_UpdateInit");

        base.OnPreRender(e);

        Fill();

        /*
        Ektron.Cms.ContentAPI capi = new ContentAPI();

        Ektron.Cms.Framework.Core.Content.Content content = new Ektron.Cms.Framework.Core.Content.Content();
        Criteria<ContentProperty> criteria = new Criteria<ContentProperty>();
        criteria.AddFilter(ContentProperty.FolderId, CriteriaFilterOperator.EqualTo, FilterFolderId < 0 ? 0 : FilterFolderId);
        //criteria.AddFilter(ContentProperty.Status, CriteriaFilterOperator.EqualTo, null);
        List<ContentData> contentlist = content.GetList(criteria);
        contentlist = capi.EkContentRef.GetContentList(criteria, true, true);

        */
    }
    public void Fill()
    {
        Fill(false);
    }
    public void Fill(bool fillAnyway)
    {
        int itemsPerPage = ItemsPerPage;
        if (itemsPerPage <= 0)
            itemsPerPage = 20;

        ReportingCriteria criteria2 = new ReportingCriteria();
        criteria2.FolderRecursive = true;
        criteria2.AddFilter(ReportingProperty.FolderId, CriteriaFilterOperator.EqualTo, FilterFolderId < 0 ? 0 : FilterFolderId);
        if (!string.IsNullOrEmpty(FilterTitle))
            criteria2.AddFilter(ReportingProperty.Title, CriteriaFilterOperator.Contains, FilterTitle.Replace("&", "&amp;").Replace("'", "&#39;"));

        if (FilterLocale > 0)
        {
            if (!FilterLocaleNotIn)
                criteria2.AddFilter(ReportingProperty.Locale, CriteriaFilterOperator.Contains, "," + FilterLocale.ToString() + ",");
            else
                criteria2.AddFilter(ReportingProperty.Locale, CriteriaFilterOperator.DoesNotContain, "," + FilterLocale.ToString() + ",");
        }
        else if (FilterLocale == -1)
        {
            criteria2.AddFilter(ReportingProperty.Locale, CriteriaFilterOperator.EqualTo, string.Format(",{0},", CurrentLanguageId));
        }
        if (FilterLocStatus != Ektron.Cms.Localization.LocalizationState.Undefined)
            criteria2.AddFilter(ReportingProperty.TranslationStatus, CriteriaFilterOperator.EqualTo, (byte)FilterLocStatus);
        if (!string.IsNullOrEmpty(FilterContentStatus))
            criteria2.AddFilter(ReportingProperty.ContentStatus, CriteriaFilterOperator.EqualTo, FilterContentStatus);
        if (FilterLastModifiedStart != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.LastModified, CriteriaFilterOperator.GreaterThanOrEqualTo, FilterLastModifiedStart);
        if (FilterLastModifiedEnd != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.LastModified, CriteriaFilterOperator.LessThan, FilterLastModifiedEnd);
        if (FilterDateCreatedStart != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, FilterDateCreatedStart);
        if (FilterDateCreatedEnd != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.DateCreated, CriteriaFilterOperator.LessThan, FilterDateCreatedEnd);
        if (FilterContentId > -1)
            criteria2.AddFilter(ReportingProperty.ContentId, CriteriaFilterOperator.EqualTo, FilterContentId);
        if (FilterAuthorId > 0)
            criteria2.AddFilter(ReportingProperty.AuthorId, CriteriaFilterOperator.EqualTo, FilterAuthorId);
        if (FilterLanguageId > 0)
            criteria2.AddFilter(ReportingProperty.LanguageId, CriteriaFilterOperator.EqualTo, FilterLanguageId);

        criteria2.GetMetadata = true;

        criteria2.PagingInfo.CurrentPage = CurrentPage;
        criteria2.PagingInfo.RecordsPerPage = itemsPerPage;
        criteria2.OrderByField = SortBy;
        criteria2.OrderByDirection = SortOrder;

        List<ReportingData> contentlist2 = l10nManager.GetReport(criteria2);

        int pageCount = 0;
        int totalCount = criteria2.PagingInfo.TotalRecords;
        int totalPages = criteria2.PagingInfo.TotalPages;
        int currentPage = criteria2.PagingInfo.CurrentPage;
        int startIndex = (criteria2.PagingInfo.CurrentPage - 1) * criteria2.PagingInfo.RecordsPerPage + 1;
        if (contentlist2 != null)
        {
            pageCount = contentlist2.Count;
        }
        int endIndex = startIndex + pageCount - 1;

        lResultCount.Text = lResultCount2.Text = pageCount == 0 ?
            "No results" :
            ("Showing result" + (pageCount > 1 ?
                ("s " + startIndex.ToString() + " - " + endIndex.ToString()) :
                (" " + startIndex.ToString())) +
                " of " + totalCount.ToString());

        string paging = string.Empty;
        string baseUrl = "window." + this.ClientID + "_Page({0});return false;";
        /*Request.Url.PathAndQuery;
    if (baseUrl.Contains("?"))
        baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("?"));
    baseUrl += "?p={0}" +
        (string.IsNullOrEmpty(Request.QueryString["o"]) ? string.Empty : ("&o=" + GetSortStr())) +
        (string.IsNullOrEmpty(FilterTitle) ? string.Empty : ("&s=" + Server.UrlEncode(FilterTitle)));*/

        if (totalPages > 0 && totalCount > 0)
        {
            List<int> pageList = new List<int>();
            if (totalPages <= 10)
                for (int i = 1; i <= totalPages; i++)
                    pageList.Add(i);
            else
            {
                if (currentPage <= 6)
                    pageList.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, totalPages - 1, totalPages });
                else if (currentPage >= totalPages - 5)
                    pageList.AddRange(new int[] { 1, 2, totalPages - 7, totalPages - 6, totalPages - 5, totalPages - 4, totalPages - 3, totalPages - 2, totalPages - 1, totalPages });
                else
                    pageList.AddRange(new int[] { 1, 2, currentPage - 2, currentPage - 1, currentPage, currentPage + 1, currentPage + 2, totalPages - 1, totalPages });
            }

            for (int p = 0; p < pageList.Count; p++)
            {
                int i = pageList[p];
                if (i == 1 && currentPage > 1)
                {
                    paging += "<li class=\"previous\"><a href=\"#\" onclick=\"" + string.Format(baseUrl, currentPage - 1) + "\">Previous&nbsp;</a></li>";
                }
                if (totalPages > 10 && p > 0 && (i - 1 > pageList[p - 1]))
                    paging += "<li class=\"page\">...</li>";
                paging += currentPage == i ?
                    "<li class=\"page\">" + i.ToString() + "</li>" :
                    "<li class=\"page\"><a href=\"#\" onclick=\"" + string.Format(baseUrl, i) + "\">" + i.ToString() + "</a></li>";
                if (i == totalPages && currentPage < totalPages)
                {
                    paging += "<li class=\"next\"><a href=\"#\" onclick=\"" + string.Format(baseUrl, currentPage + 1) + "\">Next&nbsp;</a></li>";
                }
            }
        }

        lPaging1.Text = lPaging2.Text = paging;

        rptRows.DataSource = contentlist2;
        rptRows.DataBind();
    }
    protected string GenerateHeaderUrl(ReportingProperty prop)
    {
        ReportingProperty currentProp = SortBy;
        EkEnumeration.OrderByDirection currentOrder = SortOrder;

        EkEnumeration.OrderByDirection newOrder = currentProp == prop ?
            (currentOrder == EkEnumeration.OrderByDirection.Ascending ? EkEnumeration.OrderByDirection.Descending : EkEnumeration.OrderByDirection.Ascending) :
            currentOrder;

        string baseUrl = "window." + this.ClientID + "_Sort('" + GetSortStr(prop, newOrder) + "');return false;";

        return baseUrl;
    }
    public string GetSearchUrl()
    {
        string baseUrl = Request.Url.PathAndQuery;
        if (baseUrl.Contains("?"))
            baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("?"));
        baseUrl += "?p=" + CurrentPage.ToString() +
            "&o=" + GetSortStr() +
            "&s=@@@";

        return baseUrl;
    }
    protected string GetSortClass(ReportingProperty prop, string appendClass)
    {
        string cssClass = appendClass + (string.IsNullOrEmpty(appendClass) ? string.Empty : " ");
        if (SortBy == prop)
            cssClass += SortOrder == EkEnumeration.OrderByDirection.Ascending ?
                "hdrSortUp" :
                "hdrSortDown";

        return cssClass;
    }
    private string GetSortStr()
    {
        return GetSortStr(SortBy, SortOrder);
    }
    private string GetSortStr(ReportingProperty prop, EkEnumeration.OrderByDirection order)
    {
        string s = GetPropertyIndex(prop).ToString();

        if (order == EkEnumeration.OrderByDirection.Descending)
            s += "-";

        return s;
    }
    private ReportingProperty _sortBy = ReportingProperty.LastModified;
    private EkEnumeration.OrderByDirection _sortOrder = EkEnumeration.OrderByDirection.Descending;
    public ReportingProperty SortBy { get { return _sortBy; } set { _sortBy = value; } }
    public EkEnumeration.OrderByDirection SortOrder { get { return _sortOrder; } set { _sortOrder = value; } }
    private Pair GetSort(string fromString)
    {
        Pair p = new Pair();
        if (string.IsNullOrEmpty(fromString))
        {
            p.First = ReportingProperty.LastModified;
            p.Second = EkEnumeration.OrderByDirection.Descending;
        }
        else
        {
            string ob2 = fromString.Contains("-") ? fromString.Substring(0, fromString.IndexOf("-")) : fromString;
            int index = int.Parse(ob2);
            p.First = GetPropertyFromIndex(index);
            p.Second = fromString.Contains("-") ? EkEnumeration.OrderByDirection.Descending : EkEnumeration.OrderByDirection.Ascending;
        }

        return p;
    }

    private ReportingProperty[] PropertyArray = new ReportingProperty[] {
        ReportingProperty.Title,
        ReportingProperty.Locale,
        ReportingProperty.TranslationStatus,
        ReportingProperty.ContentStatus,
        ReportingProperty.LastModified,
        ReportingProperty.DateCreated,
        ReportingProperty.ContentId,
        ReportingProperty.AuthorId,
        ReportingProperty.FolderId,
        ReportingProperty.LanguageId};

    private int GetPropertyIndex(ReportingProperty prop)
    {
        for (int i = 0; i < PropertyArray.Length; i++)
            if (PropertyArray[i] == prop)
                return i + 1;
        return 1;
    }
    private ReportingProperty GetPropertyFromIndex(int index)
    {
        if (index > 0 && index <= PropertyArray.Length)
            return PropertyArray[index - 1];

        return PropertyArray[0];
    }
    protected string GetLocales(List<int> locales, int maxBeforeShorten)
    {
        List<string> slocales = new List<string>();
        Dictionary<int, string> localeList = GetShortLocaleList();

        foreach (int l in locales)
        {
            if (localeList.ContainsKey(l))
                slocales.Add(localeList[l]);
            else
                slocales.Add(l.ToString());
        }

        if (maxBeforeShorten > 0 && slocales.Count > maxBeforeShorten)
            return "<span title=\"" + string.Join(", ", slocales.ToArray()) + "\">" +
                slocales.Count.ToString() + " locale" + (slocales.Count == 1 ? string.Empty : "s") + "</span>";

        return string.Join(", ", slocales.ToArray());
    }
    protected string GetPolicheckStatus(bool policheck)
    {
        string policheckStatus = "";
        if (policheck)
        {
            policheckStatus = GetMessage("generic yes");
        }
        else
        {
            policheckStatus = GetMessage("generic no");
        }
        return policheckStatus;
    }
    protected string GetLocStatus(string value)
    {
        Ektron.Cms.Localization.LocalizationState state = string.IsNullOrEmpty(value) ?
            Ektron.Cms.Localization.LocalizationState.Undefined :
            (Ektron.Cms.Localization.LocalizationState)Enum.Parse(typeof(Ektron.Cms.Localization.LocalizationState), value);

        return GetLocStatus(state);
    }
    protected string GetLocStatus(Ektron.Cms.Localization.LocalizationState state)
    {
        switch (state)
        {
            case Ektron.Cms.Localization.LocalizationState.DoNotTranslate:
                return "Do not translate";
            case Ektron.Cms.Localization.LocalizationState.NeedsTranslation:
                return "Requires translation";
            case Ektron.Cms.Localization.LocalizationState.NotReady:
                return "Not ready for translation";
            case Ektron.Cms.Localization.LocalizationState.OutForTranslation:
                return "Out for translation";
            case Ektron.Cms.Localization.LocalizationState.Ready:
                return "Ready for translation";
            case Ektron.Cms.Localization.LocalizationState.Translated:
                return "Translated";
            case Ektron.Cms.Localization.LocalizationState.Undefined:
            default:
                return "Unknown";
        }
    }

    public delegate string CallbackDelegate(string key, Dictionary<string, string> data);
    private Dictionary<string, List<CallbackDelegate>> CallbackDelegates = new Dictionary<string, List<CallbackDelegate>>();
    private Dictionary<string, string> CallbackIndexes = new Dictionary<string, string>();
    private Dictionary<string, string> CallbackFunctions = new Dictionary<string, string>();

    /// <summary>
    /// Adds a callback function on the client (pre-postback, so beware) and then sets a server-side delegate
    /// </summary>
    /// <param name="key">A friendly key to internally associate with the callback (does not get passed to client)</param>
    /// <param name="callbackDelegate">The delegate method to call when this callback gets fired</param>
    /// <returns>The name of a generated client-side function that can be called from JavaScript with args and context parameters</returns>
    public string AddCallback(string key, CallbackDelegate callbackDelegate)
    {
        if (CallbackFunctions.ContainsKey(key))
        {
            if (callbackDelegate != null && !CallbackDelegates[key].Contains(callbackDelegate))
                CallbackDelegates[key].Add(callbackDelegate);
            return CallbackFunctions[key];
        }

        string str = this.Page.ClientScript.GetCallbackEventReference(
            this,
            "args",
            this.ClientID + "_Callback", //"IAjax.DisplayResult",
            "context",
            "ReportGrid_Error", //"IAjax.DisplayError",
            false);
        StringBuilder builder2 = new StringBuilder();

        // Generate a function name for this guy
        int index = CallbackDelegates.Count + 1;
        string funcName = "_fn" + index.ToString() + this.ClientID;
        builder2.Append("window." + funcName + " = function(args,context){" +
            (string.IsNullOrEmpty(ClientShowBusyDuringCallback) ? string.Empty : ((UseClientIDPrefix ? (this.ClientID + "_") : string.Empty) + ClientShowBusyDuringCallback + "(true);")) +
            "args='fn=_fn" + index.ToString() +
            "&' + (args == null ? '' : args);" + str + ";}\n");
        JS.RegisterJSBlock(this, builder2.ToString(), this.ClientID + "_JS" + index.ToString());

        CallbackDelegates.Add(
            key,
            callbackDelegate == null ? new List<CallbackDelegate>() : new List<CallbackDelegate>(new CallbackDelegate[] { callbackDelegate }));
        CallbackFunctions.Add(key, funcName);
        CallbackIndexes.Add("_fn" + index.ToString(), key);

        return funcName;
    }

    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return (this._Error + this._callbackresult);
    }

    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgs)
    {
        bool flag;
        this._callbackresult = string.Empty;
        this._Error = string.Empty;
        this.PostBackData = HttpUtility.ParseQueryString(eventArgs);

        string fn = this.PostBackData["fn"];
        StringBuilder output = new StringBuilder();
        if (CallbackIndexes.ContainsKey(fn))
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (string k in this.PostBackData.Keys)
                if (!string.IsNullOrEmpty(k) && !data.ContainsKey(k))
                    data.Add(k, this.PostBackData[k]);

            string key = CallbackIndexes[fn];
            foreach (CallbackDelegate del in CallbackDelegates[key])
                output.Append(del(key, data));
        }

        this._callbackresult = output.ToString();
        //base.callbackwrap = false;
    }

    #endregion

    protected string CallbackDoPaging(string key, Dictionary<string, string> data)
    {
        InitFromCallback(data);
        int page = int.Parse(data["page"]);
        CurrentPage = page;

        Fill(true);

        Dictionary<string, string> output = new Dictionary<string, string>();
        output["action"] = "updategrid";
        output["newpage"] = page.ToString();
        output["html"] = RenderControl(plcRenderMe);

        return ConvertDictionaryToQueryString(output);
    }
    protected string CallbackDoSort(string key, Dictionary<string, string> data)
    {
        InitFromCallback(data);
        Pair sort = GetSort(data["sort"]);
        SortBy = (ReportingProperty)sort.First;
        SortOrder = (EkEnumeration.OrderByDirection)sort.Second;

        Fill(true);

        Dictionary<string, string> output = new Dictionary<string, string>();
        output["action"] = "updategrid";
        output["newsort"] = GetSortStr(SortBy, SortOrder);
        output["html"] = RenderControl(plcRenderMe);

        return ConvertDictionaryToQueryString(output);
    }
    protected string GetFilterValue(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return null;

        switch (filter.ToLower())
        {
            case "folderid":
                return FilterFolderId.ToString();
            case "title":
                return FilterTitle;
            case "locale":
                return FilterLocale.ToString();
            case "localenotin":
                return FilterLocaleNotIn.ToString();
            case "locstatus":
                return FilterLocStatus.ToString();
            case "contentstatus":
                return FilterContentStatus;
            case "lastmodifiedstart":
                return FilterLastModifiedStart.ToShortDateString();
            case "lastmodifiedend":
                return FilterLastModifiedEnd.ToShortDateString();
            case "datecreatedstart":
                return FilterDateCreatedStart.ToShortDateString();
            case "datecreatedend":
                return FilterDateCreatedEnd.ToShortDateString();
            case "contentid":
                return FilterContentId.ToString();
            case "authorid":
                return FilterAuthorId.ToString();
            case "languageid":
                return FilterLanguageId.ToString();
        }

        return null;
    }
    protected void SetFilterValue(string filter, string value)
    {
        if (string.IsNullOrEmpty(filter))
            return;

        try
        {
            switch (filter.ToLower())
            {
                case "folderid":
                    FilterFolderId = this.ParseLong(value);
                    break;
                case "title":
                    FilterTitle = value;
                    break;
                case "locale":
                    FilterLocale = int.Parse(value.Replace("-", string.Empty));
                    break;
                case "localenotin":
                    FilterLocaleNotIn = !string.IsNullOrEmpty(value) && value.ToLower().StartsWith("t");
                    break;
                case "locstatus":
                    FilterLocStatus = (Ektron.Cms.Localization.LocalizationState)Enum.Parse(typeof(Ektron.Cms.Localization.LocalizationState), value);
                    break;
                case "contentstatus":
                    FilterContentStatus = value;
                    break;
                case "lastmodifiedstart":
                    FilterLastModifiedStart = this.ParseDateTime(value);
                    break;
                case "lastmodifiedend":
                    FilterLastModifiedEnd = this.ParseDateTime(value);
                    if (FilterLastModifiedEnd > DateTime.MinValue)
                    {
                        FilterLastModifiedEnd = FilterLastModifiedEnd.AddDays(1);
                    }
                    break;
                case "datecreatedstart":
                    FilterDateCreatedStart = this.ParseDateTime(value);
                    break;
                case "datecreatedend":
                    FilterDateCreatedEnd = this.ParseDateTime(value);
                    if (FilterDateCreatedEnd > DateTime.MinValue)
                    {
                        FilterDateCreatedEnd = FilterDateCreatedEnd.AddDays(1);
                    }
                    break;
                case "contentid":
                    if (!string.IsNullOrEmpty(value))
                        long.TryParse(value, out _filterContentId);
					else
						_filterContentId = -1;
                    break;
                case "authorid":
                    FilterAuthorId = this.ParseLong(value);
                    break;
                case "languageid":
                    FilterLanguageId = int.Parse(value);
                    break;
            }
        }
        catch
        {
        }
    }
    private DateTime ParseDateTime(string strDate)
    {
        //#59144: need to expect empty string.
        DateTime retVal = DateTime.MinValue;
        DateTime outVal;
        if (DateTime.TryParse(strDate, out outVal))
        {
            retVal = outVal;
        }
        return retVal;
    }
    private long ParseLong(string value)
    {
        //#58765: need to expect empty string.
        long retVal = 0;
        long outVal;
        if (long.TryParse(value, out outVal))
        {
            retVal = outVal;
        }
        return retVal;
    }

    private Dictionary<string, string> DefaultFilterValues = null;
    protected string GetCurrentFilterValuesAsString()
    {
        return GetCurrentFilterValuesAsString(false);
    }
    protected string GetCurrentFilterValuesAsString(bool changedOnly)
    {
        return ConvertDictionaryToQueryString(GetCurrentFilterValues(changedOnly));
    }
    protected Dictionary<string, string> GetCurrentFilterValues()
    {
        return GetCurrentFilterValues(false);
    }
    protected Dictionary<string, string> GetCurrentFilterValues(bool changedOnly)
    {
        Dictionary<string, string> newfilters = new Dictionary<string, string>();
        string[] filternames = new string[] {"folderid", "title", "locale", "localenotin", 
            "contentstatus", "lastmodifiedstart", "lastmodifiedend", "datecreatedstart", 
            "datecreatedend", "contentid", "authorid", "languageid"};
        List<string> ignorefilters = new List<string>(new string[] { "title" });
        foreach (string f in filternames)
            newfilters[f] = GetFilterValue(f);

        if (changedOnly)
            foreach (string f in filternames)
                if (!ignorefilters.Contains(f) && newfilters[f] == DefaultFilterValues[f])
                    newfilters.Remove(f);

        return newfilters;
    }
    protected string CallbackDoFilter(string key, Dictionary<string, string> data)
    {
        InitFromCallback(data);
        string sfilters = data["filters"];

        // Reset page to page 1
        CurrentPage = 1;

        NameValueCollection filters = HttpUtility.ParseQueryString(sfilters);
        foreach (string f in filters.Keys)
            SetFilterValue(f, filters[f]);

        Fill(true);

        Dictionary<string, string> output = new Dictionary<string, string>();
        output["action"] = "updategrid";
        output["newfilters"] = GetCurrentFilterValuesAsString(true);
        output["html"] = RenderControl(plcRenderMe);

        return ConvertDictionaryToQueryString(output);
    }
    protected string CallbackDoAction(string key, Dictionary<string, string> data)
    {
        InitFromCallback(data);
        string what = data["action"];
        string sselected = data["selected"];
        List<long> selected = sselected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll<long>(new Converter<string, long>(delegate(string s) { return long.Parse(s); }));

        Ektron.Cms.CommonApi commonApi = new CommonApi();
        Ektron.Cms.Content.EkContent contentManager = new Ektron.Cms.Content.EkContent(commonApi.RequestInformationRef);
        Ektron.Cms.API.Content.Content capi = new Ektron.Cms.API.Content.Content();
        Ektron.Cms.Framework.Localization.LocaleManager localeApi = new Ektron.Cms.Framework.Localization.LocaleManager();
        Ektron.Cms.Framework.Localization.LocalizationObject lobjApi = new Ektron.Cms.Framework.Localization.LocalizationObject();
        Ektron.Cms.API.Content.Taxonomy taxonomyApi = new Ektron.Cms.API.Content.Taxonomy();

        string title = string.Empty, html = string.Empty, okclick = string.Empty, action = string.Empty;

        switch (what)
        {
            case "locales":
                {
                    action = "showmodal";
                    bool? addRemoveReplace = null;
                    if (data.ContainsKey("mode"))
                        if (data["mode"] == "add")
                            addRemoveReplace = true;
                        else if (data["mode"] == "del")
                            addRemoveReplace = false;
                    title = (addRemoveReplace == null ? this.GetMessage("lbl Change selected locales") :
                        (addRemoveReplace == true ? "Add locale(s)" : "Remove locale(s)"))
                        + " " + this.GetMessage("generic for") + " " + (selected.Count == 1 ? "1 item" : (selected.Count.ToString() + " " + this.GetMessage("generic items")));
                    html = GenerateLocaleList(selected, CurrentLanguageId, addRemoveReplace);
                    okclick = this.ClientID + (addRemoveReplace == null ? "_LocaleSet" : (addRemoveReplace == true ? "_LocaleAdd" : "_LocaleDel"));
                }
                break;
            case "localeset":
                {
                    List<int> locales = data["locales"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>().ConvertAll<int>(new Converter<string, int>(delegate(string s) { return int.Parse(s); }));
                    bool? addRemoveReplace = null;
                    if (data.ContainsKey("mode"))
                        if (data["mode"] == "add")
                            addRemoveReplace = true;
                        else if (data["mode"] == "del")
                            addRemoveReplace = false;
                    SetLocales(selected, locales, CurrentLanguageId, addRemoveReplace);
                    action = "hidemodal";
                    title = "";
                    Fill();
                    html = RenderControl(plcRenderMe);
                    okclick = "";
                }
                break;
            case "transstatus":
                {
                    action = "showmodal";
                    title = "Set Translation Readiness";
                    // ##TODO: Check current translation status
                    html = RenderControl(pnlSetTransStatus);
                    okclick = this.ClientID + "_TransStatusSet";
                }
                break;
            case "settranstatus":
                {
                    string status = data["status"].ToLower();
                    foreach (long id in selected)
                    {
                        Ektron.Cms.Localization.LocalizableCmsObjectType objectType = Ektron.Cms.Localization.LocalizableCmsObjectType.Content;
                        Ektron.Cms.Framework.Localization.LocalizationObject lobj = new Ektron.Cms.Framework.Localization.LocalizationObject();
                        switch (status)
                        {
                            case "ready":
                                lobjApi.MarkReadyForTranslation(objectType, id, CurrentLanguageId);
                                break;
                            case "notready":
                                lobjApi.MarkNotReadyForTranslation(objectType, id, CurrentLanguageId);
                                break;
                            case "donottranslate":
                                lobjApi.MarkDoNotTranslate(objectType, id, CurrentLanguageId);
                                break;
                        }
                    }
                    action = "hidemodal";
                    title = string.Empty;
                    Fill();
                    html = RenderControl(plcRenderMe);
                    okclick = string.Empty;
                }
                break;
            case "notes":
                {
                    action = "showmodal";
                    title = "Add notes for " + (selected.Count == 1 ? "1 item" : (selected.Count.ToString() + " items"));
                    lNoteN.Visible = selected.Count > 1;

                    if (selected.Count == 1) // Load notes if there is only one piece of content
                    {
                        // ##TODO: Add this!
                    }
                    html = RenderControl(pnlNotes);
                    okclick = this.ClientID + "_NotesSet";
                }
                break;
            case "setnotes":
                {
                    string notes = data["notes"];

                    long metadataId = commonApi.EkContentRef.GetMetadataTypeId("XliffNote", CurrentLanguageId, 0);

                    foreach (long cid in selected)
                    {
                        capi.UpdateContentMetaData(cid, metadataId, notes);
                    }

                    action = "hidemodal";
                    title = string.Empty;
                    Fill();
                    html = RenderControl(plcRenderMe);
                    okclick = string.Empty;
                }
                break;
            case "pseudo":
                {
                    action = "showmodal";
                    title = "Pseudo Localize " + (selected.Count == 1 ? "1 item" : (selected.Count.ToString() + " items")) + ".";

                    Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
                    List<Ektron.Cms.Localization.LocaleData> pseudoLocales = locale.GetEnabledLocales().FindAll(d => d.XmlLang.Contains("-x-pseudo"));

                    if (pseudoLocales.Count > 0)
                        foreach (Ektron.Cms.Localization.LocaleData pseudoLocale in pseudoLocales)
                        {
                            CheckBox pseudoLoc = new CheckBox()
                            {
                                ID = "chkPS" + pseudoLocale.Id.ToString(),
                                Checked = true,
                                Text = pseudoLocale.Loc + " " + pseudoLocale.EnglishName
                            };
                            pnlPseudo.Controls.Add(pseudoLoc);
                            pnlPseudo.Controls.Add(new LiteralControl("<br />"));
                        }
                    else
                        lblPseudoInstructions.Text = "There are no Pseudo localization enabled locales.";

                    html = RenderControl(pnlPseudo);
                    okclick = this.ClientID + "_PseudoSet";
                }
                break;
            case "setpseudo":
                {
                    action = "showmodal";
                    title = "Pseudo Localize " + (selected.Count == 1 ? "1 item" : (selected.Count.ToString() + " items")) + ".";

                    string selectedIds = string.Join(",", selected.ConvertAll<string>(delegate(long i) { return i.ToString(); }).ToArray());
                    string pseudoLocaleIds = data["locales"];

                    ektronExportPseudoIframe.Attributes["src"] = "widgets/Modal/localizationjobs.aspx?action=pseudo&contentIds=" + selectedIds + "&languageIds=" + pseudoLocaleIds;
                    html = RenderControl(pnlSetPseudo);
                    okclick = this.ClientID + "_PseudoReset";
                }
                break;
            case "pseudocomplete":
                {
                    action = "hidemodal";
                    title = string.Empty;
                    Fill();
                    html = RenderControl(plcRenderMe);
                    okclick = string.Empty;
                }
                break;
            default:
                {
                    action = "showmodal";
                    title = "Invalid selection";
                    html = "The option that was selected is not available.";
                    okclick = this.ClientID + "_CloseModalDialog";
                }
                break;
        }

        Dictionary<string, string> output = new Dictionary<string, string>();
        output["action"] = action;
        output["title"] = title;
        output["html"] = html;
        output["okclick"] = okclick;

        return ConvertDictionaryToQueryString(output);
    }
    private void InitFromCallback(Dictionary<string, string> data)
    {
        DefaultFilterValues = GetCurrentFilterValues();

        Pair sort = GetSort(data["csort"]);
        SortBy = (ReportingProperty)sort.First;
        SortOrder = (EkEnumeration.OrderByDirection)sort.Second;

        int page = 1;
        int.TryParse(data["cpage"], out page);
        CurrentPage = page;

        NameValueCollection filters = HttpUtility.ParseQueryString(data["cfilters"]);
        foreach (string f in filters.Keys)
            SetFilterValue(f, filters[f]);

        string sselections = data["selected"];
        SelectedItems.Clear();
        if (!string.IsNullOrEmpty(sselections))
        {
            string[] sarray = sselections.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<long> selections = sarray.ToList<string>().ConvertAll<long>(new Converter<string, long>(delegate(string str)
            {
                return long.Parse(str);
            }));
            SelectedItems.AddRange(selections);
        }
    }
    private string ConvertDictionaryToQueryString(Dictionary<string, string> data)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string k in data.Keys)
        {
            if (sb.Length > 0)
                sb.Append("&");
            sb.Append(k);
            sb.Append("=");
            if (!string.IsNullOrEmpty(data[k]))
                sb.Append(HttpUtility.UrlEncodeUnicode(data[k]).Replace("+", " "));
        }
        return sb.ToString();
    }
    protected string CallbackTest(string key, Dictionary<string, string> data)
    {
        Dictionary<string, string> output = new Dictionary<string, string>();
        output["action"] = "updategrid";
        output["html"] = "Hello, <b>world</b> at <i>" + DateTime.Now.ToString() + "</i>";

        return ConvertDictionaryToQueryString(output);
    }

    protected string RenderControl(Control control)
    {
        System.IO.StringWriter sw = new System.IO.StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        control.RenderControl(htw);

        htw.Flush();
        return sw.ToString();
    }

    /// <param name="addRemoveReplace">true for add, false for remove, null for replace</param>
    protected void SetLocales(List<long> selections, List<int> selectedLocales, int defaultLocale, bool? addRemoveReplace)
    {
        Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
        Ektron.Cms.Framework.Localization.LocalizationObject lobj = new Ektron.Cms.Framework.Localization.LocalizationObject();

        List<Ektron.Cms.Localization.LocaleData> locales = locale.GetEnabledLocales();

        // Loop through each of the selected content items to try and find selected locales
        List<int> contentLocales = new List<int>();
        if (!selectedLocales.Contains(defaultLocale))
            selectedLocales.Add(defaultLocale);
        selectedLocales.Sort();

        List<int> localesToDelete = new List<int>();
        List<int> localesToAdd = new List<int>();
        List<Ektron.Cms.Localization.LocalizationObjectData> localeData = new List<Ektron.Cms.Localization.LocalizationObjectData>();

        foreach (long item in selections)
        {
            Ektron.Cms.Localization.LocalizableCmsObjectType objectType = Ektron.Cms.Localization.LocalizableCmsObjectType.Content;
            contentLocales = lobj.GetObjectLanguages(Ektron.Cms.Localization.LocalizableCmsObjectType.Content, item);
            if (contentLocales.Count == 0)
            {
                objectType = Ektron.Cms.Localization.LocalizableCmsObjectType.DmsAsset;
                contentLocales = lobj.GetObjectLanguages(Ektron.Cms.Localization.LocalizableCmsObjectType.DmsAsset, item);
            }
            bool skip = true;
            if (addRemoveReplace == null && !LocaleListsMatch(selectedLocales, contentLocales))
                skip = false;
            if (addRemoveReplace == true && !LocaleListContains(contentLocales, selectedLocales))
                skip = false;
            if (addRemoveReplace == false && LocaleListContains(contentLocales, selectedLocales))
                skip = false;

            if (!skip)
            {
                localeData = lobj.GetLocalizationObjectList(objectType, item, -1);
                localesToDelete.Clear();
                localesToAdd.Clear();
                if (addRemoveReplace == null || addRemoveReplace == true)
                    foreach (int i in selectedLocales)
                        if (!contentLocales.Contains(i))
                            localesToAdd.Add(i);
                if (addRemoveReplace == null)
                    foreach (int i in contentLocales)
                        if (!selectedLocales.Contains(i))
                            localesToDelete.Add(i);
                if (addRemoveReplace == false)
                    foreach (int i in contentLocales)
                        if (selectedLocales.Contains(i))
                            localesToDelete.Add(i);

                foreach (int i in localesToDelete)
                    foreach (Ektron.Cms.Localization.LocalizationObjectData ldata in localeData)
                        if (ldata.ObjectLanguage == i)
                            lobj.Delete(ldata.Id);

                foreach (int i in localesToAdd)
                    lobj.MarkReadyForTranslation(objectType, item, i);
            }
        }
    }
    private bool LocaleListsMatch(List<int> a, List<int> b)
    {
        List<int> a2 = a.ToList<int>();
        List<int> b2 = b.ToList<int>();
        a2.Sort();
        b2.Sort();

        Converter<int, string> conv = new Converter<int, string>(delegate(int id) { return id.ToString(); });

        string a3 = string.Join(",", a2.ConvertAll<string>(conv).ToArray());
        string b3 = string.Join(",", b2.ConvertAll<string>(conv).ToArray());

        return a3 == b3;
    }
    private bool LocaleListContains(List<int> source, List<int> checkFor)
    {
        foreach (int i in checkFor)
            if (!source.Contains(i))
                return false;

        return true;
    }
    protected DataTable GetRowsFromArray(DataRow[] drs)
    {
        if (drs == null || drs.Length == 0)
            return new DataTable();
        return drs.CopyToDataTable<DataRow>();
    }
    protected string GenerateLocaleList(List<long> selections, int defaultLocale, bool? addRemoveReplace)
    {
        plcLangsSelected.Visible = addRemoveReplace == null;

        Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
        Ektron.Cms.Framework.Localization.LocalizationObject lobj = new Ektron.Cms.Framework.Localization.LocalizationObject();

        List<Ektron.Cms.Localization.LocaleData> locales = locale.GetEnabledLocales();

        // If not adding/deleting, loop through each of the selected content items to try and find selected locales
        List<int> selectedLocales = new List<int>();
        if (addRemoveReplace == null)
        {
            List<int> contentLocales = new List<int>();
            lMultiNotice.Visible = false;
            foreach (long item in selections)
            {
                contentLocales = lobj.GetObjectLanguages(Ektron.Cms.Localization.LocalizableCmsObjectType.Content, item);
                contentLocales.Sort();
                if (selectedLocales.Count == 0)
                    selectedLocales.AddRange(contentLocales);
                if (!LocaleListsMatch(selectedLocales, contentLocales))
                {
                    selectedLocales = new List<int>();
                    lMultiNotice.Visible = true;
                    break;
                }
            }

            if (!selectedLocales.Contains(defaultLocale))
                selectedLocales.Add(defaultLocale);
        }

        DataTable dt = new DataTable("locales");
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Enabled", typeof(bool));
        dt.Columns.Add("EnglishName", typeof(string));
        dt.Columns.Add("Loc", typeof(string));
        dt.Columns.Add("CombinedName", typeof(string));
        dt.Columns.Add("FlagUrl", typeof(string));
        dt.Columns.Add("Default", typeof(bool));

        DataRow dr;

        foreach (Ektron.Cms.Localization.LocaleData ld in locales)
        {
            dr = dt.NewRow();
            dr.ItemArray = new object[] { ld.Id, selectedLocales.Contains(ld.Id) || ld.Id == CurrentLanguageId, ld.EnglishName, ld.Loc, ld.CombinedName, ld.FlagUrl, ld.Id == defaultLocale };
            dt.Rows.Add(dr);
        }

        DataTable sdt, adt;
        sdt = GetRowsFromArray(dt.Select("Enabled = true", "Loc"));
        adt = GetRowsFromArray(dt.Select("Enabled = false", "Loc"));

        // Databind
        rptSelectedLangs.DataSource = sdt;
        rptAvailableLangs.DataSource = adt;
        pnlLangSelector.DataBind();

        List<int> availableLocales = locales.ConvertAll<int>(new Converter<Ektron.Cms.Localization.LocaleData, int>(delegate(Ektron.Cms.Localization.LocaleData ld) { return ld.Id; }));
        foreach (int l in selectedLocales)
            if (availableLocales.Contains(l))
                availableLocales.Remove(l);

        // Set up the links
        plcSelectLinks.Controls.Clear();

        if (adt.Rows.Count > 0)
        {
            string links = "<a onclick=\"" + SelectAllScript(true, availableLocales) +
                " return false;\" href=\"#\" title=\"" + GetMessage("lbl select all languages") + "\">" + GetMessage("lbl select all languages") + "</a> | " +
                "<a onclick=\"" + SelectAllScript(false, availableLocales) +
                " return false;\" href=\"#\" title=\"" + GetMessage("lbl deselect all languages") + "\">" + GetMessage("lbl deselect all languages") + "</a>";

            plcSelectLinks.Controls.Add(new LiteralControl(links));
        }
        else
        {
            rptAvailableLangs.Visible = false;
        }

        // Render
        return RenderControl(pnlLangSelector);
    }
    protected string SelectAllScript(bool select, List<int> idlist)
    {
        return "var items = '" + string.Join(",", idlist.ConvertAll<string>(new Converter<int, string>(delegate(int id) { return id.ToString(); })).ToArray()) +
            "'.split(','); for (var i = 0; i < items.length; i++) {if (document.getElementById('kpi1_ReportGrid1_locale' + items[i]) != null) { document.getElementById('" + this.ClientID + "_locale' + items[i]).checked = " + (select ? "true" : "false") + "}};";
    }
    protected string FormatDate(object date)
    {
        if (date is DBNull || date == null)
            return string.Empty;

        if (date is DateTime && (DateTime)date != DateTime.MinValue)
            return ((DateTime)date).ToShortDateString();

        if (date is string)
        {
            DateTime date1 = DateTime.MinValue;
            if (!DateTime.TryParse((string)date, out date1))
                return string.Empty;
            return date1.ToShortDateString();
        }

        return string.Empty;
    }
    private Dictionary<int, string> _shortLocaleList = null;
    private object _shortLocaleListLock = new object();
    public Dictionary<int, string> GetShortLocaleList()
    {
        _shortLocaleList = (Dictionary<int, string>)Cache[shortLocaleCacheKey];
        if (_shortLocaleList == null)
            lock (_shortLocaleListLock)
                if (_shortLocaleList == null)
                {
                    Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
                    Criteria<Ektron.Cms.Localization.LocaleProperty> criteria = new Criteria<Ektron.Cms.Localization.LocaleProperty>(Ektron.Cms.Localization.LocaleProperty.LangCode, EkEnumeration.OrderByDirection.Ascending);
                    criteria.AddFilter(Ektron.Cms.Localization.LocaleProperty.Enabled, CriteriaFilterOperator.EqualTo, true);
                    criteria.PagingInfo.RecordsPerPage = int.MaxValue - 1;
                    List<Ektron.Cms.Localization.LocaleData> locales = locale.GetList(criteria);
                    _shortLocaleList = new Dictionary<int, string>();
                    foreach (Ektron.Cms.Localization.LocaleData l in locales)
                        if (!_shortLocaleList.ContainsKey(l.Id))
                            _shortLocaleList.Add(l.Id, l.Loc);
                    Cache.Add(shortLocaleCacheKey, _shortLocaleList, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }

        return _shortLocaleList;
    }
    private Dictionary<long, string> _authorList = null;
    private object _authorListLock = new object();
    public Dictionary<long, string> GetAuthorList()
    {
        string cacheKey = "reportAuthorList";
        _authorList = (Dictionary<long, string>)Cache[cacheKey];
        if (_authorList == null)
            lock (_authorListLock)
                if (_authorList == null)
                {
                    _authorList = new Dictionary<long, string>();
                    Criteria<UserProperty> userCriteria = new Criteria<UserProperty>();
                    List<UserData> users = new List<UserData>();

                    userCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
                    userCriteria.PagingInfo.RecordsPerPage = int.MaxValue - 1;
                    userCriteria.OrderByField = UserProperty.UserName;
                    users = ObjectFactory.GetUser().GetList(userCriteria);

                    users.Sort((x, y) => string.Compare(x.FirstName + " " + x.LastName, y.FirstName + " " + y.LastName));

                    foreach (UserData u in users)
                        _authorList.Add(u.Id, u.FirstName + " " + u.LastName);
                    Cache.Add(cacheKey, _authorList, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }

        return _authorList;
    }
    public string GetContentStatusIcon(string contentStatus)
    {
        string icon = "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />";
        switch (contentStatus.ToUpper())
        {
            case "A":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/check.png")
                    , "Published");
                break;
            case "P":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/clockAdd.png")
                    , "Pending Start");
                break;
            case "O":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/checkOutAndSave.png")
                    , "Checked Out");
                break;
            case "S":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/approvalSubmitFor.png")
                    , "Submitted");
                break;
            case "I":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/checkIn.png")
                    , "Checked In");
                break;
            case "M":
            case "D":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/clockDelete.png")
                    , "Submitted for Deletion Approval(s)");
                break;
            case "T":
                icon = string.Format(icon,
                    Page.ResolveClientUrl("~/workarea/images/UI/icons/task.png")
                    , "Awaiting Task Completion");
                break;
            default:
                icon = contentStatus;
                break;
        }
        return icon;
    }
    protected string GetViewAction(string contentStatus)
    {
        if (contentStatus == "O" ||
            contentStatus == "I" ||
            contentStatus == "S" ||
            contentStatus == "P")
            return "ViewStaged";
        else
            return "View";
    }
    protected string SafeString(object str)
    {
        return SafeString(str, string.Empty);
    }
    protected string SafeString(object str, string defaultValue)
    {
        if (str is DBNull || str == null)
            return defaultValue;
        if (str is string)
            return (string)str;
        return str.ToString();
    }
    protected string GetPreviewUrl(ReportingData data)
    {
        if (data.TemplateIsPageBuilder)
            return Page.ResolveClientUrl("~/" + data.TemplateFileName) + "?pageid=" + data.ContentId.ToString();
        else if (data.ContentType.ToString() == "101" || data.ContentType.ToString() == "103")
            return Page.ResolveClientUrl("~/workarea/downloadasset.aspx?id=" + data.ContentId.ToString());
        else
        {
            if (data.ContentType == EkEnumeration.CMSContentType.Forms)
                return Page.ResolveClientUrl("~/" + data.TemplateFileName) + "?ekfrm=" + data.ContentId.ToString();
            else
                return Page.ResolveClientUrl("~/" + data.TemplateFileName) + "?id=" + data.ContentId.ToString();
        }
    }
    protected string GetContentTypeIcon(ReportingData data)
    {
        StringBuilder sb = new StringBuilder("<img src=\"");
        string title = string.Empty;
        switch (data.ContentType)
        {
            case EkEnumeration.CMSContentType.Content:
                switch (data.ContentSubType)
                {
                    case EkEnumeration.CMSContentSubtype.PageBuilderData:
                    case EkEnumeration.CMSContentSubtype.PageBuilderMasterData:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/application/layout_content.png"));
                        title = "Page Layout";
                        break;
                    case EkEnumeration.CMSContentSubtype.Content:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/contenthtml.png"));
                        title = "HTML Content";
                        break;
                    default:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/UI/Icons/folderView.png"));
                        title = "Content";
                        break;
                }
                break;
            case EkEnumeration.CMSContentType.Forms:
                sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/contentform.png"));
                title = data.SmartformId > 0 ? ("Smartform: " + data.SmartformTitle) : "HTML Form/Survey";
                break;
            case EkEnumeration.CMSContentType.Archive_Content:
                sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/contentArchived.png"));
                title = "Archived Content";
                break;
            case EkEnumeration.CMSContentType.Archive_Forms:
                sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/contentFormArchived.png"));
                title = "Archived Form";
                break;
            case EkEnumeration.CMSContentType.CatalogEntry:
                switch ((int)data.ContentSubType)
                {
                    case (int)Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/brick.png"));
                        title = GetMessage("lbl commerce products");
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/bricks.png"));
                        title = GetMessage("lbl commerce products");
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/package.png"));
                        title = GetMessage("lbl commerce bundles");
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/box.png"));
                        title = GetMessage("lbl commerce kits");
                        break;
                    case -1: // Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/bookGreen.png"));
                        title = GetMessage("lbl commerce subscriptions");
                        break;
                }
                break;
            default:
                switch ((byte)data.ContentType)
                {
                    case 101:
                        switch (System.IO.Path.GetExtension(data.AssetVersion).ToLower())
                        {
                            case ".xls":
                            case ".xlsx":
                                sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/ms-excel.png"));
                                break;
                            case ".ppt":
                            case ".pptx":
                                sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/ms-powerpoint.png"));
                                break;
                            case ".doc":
                            case ".docx":
                            default:
                                sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/filetypes/word.png"));
                                break;

                        }
                        title = "Office Document";
                        break;
                    case 102:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/contentmanagedfiles.png"));
                        title = "Managed File";
                        break;
                    case 103:
                        if (Ektron.Cms.Common.EkFunctions.IsImage(System.IO.Path.GetExtension(data.AssetVersion).ToLower()))
                        {
                            sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/Image.png"));
                            title = "Image Type";
                        }
                        else if (System.IO.Path.GetExtension(data.AssetVersion).ToLower() == ".pdf")
                        {
                            sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/adobe-pdf.png"));
                            title = "Adobe Acrobat Document";
                        }
                        else if (System.IO.Path.GetExtension(data.AssetVersion).ToLower() == ".zip")
                        {
                            sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/WinZip.png"));
                            title = "Zip File";
                        }
                        else
                        {
                            sb.Append(Page.ResolveClientUrl("~/assetmanagement/images/ms-notepad.png"));
                            title = "Unknown Content Type";
                        }
                        break;
                    case 104:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/film.png"));
                        title = "Multimedia";
                        break;
                    default:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/UI/Icons/folderView.png"));
                        break;
                }
                break;
        }

        sb.Append("\" title=\"" + title + "\" />");

        return sb.ToString();
    }

    private ContentMetaData GetMetadata(ContentData content, long metadataTypeId)
    {
        foreach (ContentMetaData meta in content.MetaData)
            if (meta.TypeId == metadataTypeId)
                return meta;

        return null;
    }
    private string GetMetadataValue(ContentData content, long metadataTypeId)
    {
        ContentMetaData meta = GetMetadata(content, metadataTypeId);
        return meta == null ? null : meta.Text;
    }
    private Microsoft.VisualBasic.Collection GetCollection(ContentEditData contentEditData)
    {
        Microsoft.VisualBasic.Collection cCol = new Microsoft.VisualBasic.Collection();
        cCol.Add(contentEditData.StyleSheet, "StyleSheet", null, null);
        cCol.Add(contentEditData.Teaser, "ContentTeaser", null, null);
        cCol.Add(contentEditData.AssetData, "AssetData", null, null);
        cCol.Add(contentEditData.Comment, "Comment", null, null);
        cCol.Add(contentEditData.GoLive, "GoLive", null, null);
        cCol.Add(contentEditData.Html, "ContentHtml", null, null);
        cCol.Add(contentEditData.LastEditDate, "LastEditDate", null, null);
        cCol.Add(contentEditData.EndDate, "EndDate", null, null);
        cCol.Add(contentEditData.EndDateAction, "EndDateAction", null, null);
        cCol.Add(contentEditData.ManualAlias, "ManualAlias", null, null);
        cCol.Add(contentEditData.Html, "SearchText", null, null);
        cCol.Add(contentEditData.Title, "ContentTitle", null, null);
        cCol.Add(contentEditData.Image, "Image", null, null);
        cCol.Add(contentEditData.ImageThumbnail, "ImageThumbnail", null, null);

        cCol.Add(contentEditData.AssetData.MimeType == "application/x-shockwave-flash", "MediaAsset", null, null);
        if (EkConstants.IsAssetContentType(contentEditData.Type, true))
        {
            cCol.Add(contentEditData.AssetData.Id, "AssetID", null, null);
            cCol.Add(contentEditData.AssetData.Version, "AssetVersion", null, null);
            cCol.Add(contentEditData.AssetData.MimeType, "MimeType", null, null);
            cCol.Add(contentEditData.AssetData.FileExtension, "FileExtension", null, null);
            cCol.Add(contentEditData.Title + "." + contentEditData.AssetData.FileExtension, "AssetFilename", null, null);
        }
        cCol.Add(contentEditData.AssetData.MimeName, "MimeName", null, null);
        cCol.Add(contentEditData.AssetData.ImageUrl, "ImageUrl", null, null);
        cCol.Add(contentEditData.MediaText, "MediaText", null, null);
        cCol.Add(contentEditData.Approver, "Approver", null, null);
        cCol.Add(contentEditData.ApprovalMethod, "ApprovalMethod", null, null);
        cCol.Add(contentEditData.ContType, "ContentType", null, null);
        cCol.Add(contentEditData.DateCreated, "DateCreated", null, null);
        cCol.Add(contentEditData.DisplayDateCreated, "DisplayDateCreated", null, null);
        cCol.Add(contentEditData.DisplayEndDate, "DisplayEndDate", null, null);
        cCol.Add(contentEditData.DisplayGoLive, "DisplayGoLive", null, null);
        cCol.Add(contentEditData.DisplayLastEditDate, "DisplayLastEditDate", null, null);
        cCol.Add(contentEditData.EditorFirstName, "EditorFirstName", null, null);
        cCol.Add(contentEditData.EditorLastName, "EditorLastName", null, null);
        cCol.Add(contentEditData.EditorUserNames, "EditorUserNames", null, null);
        cCol.Add(contentEditData.FolderId, "FolderId", null, null);
        cCol.Add(contentEditData.FolderName, "FolderName", null, null);
        cCol.Add(contentEditData.HistoryId, "HistoryId", null, null);
        cCol.Add(contentEditData.HyperLink, "HyperLink", null, null);
        cCol.Add(contentEditData.Id, "ContentID", null, null);
        cCol.Add(contentEditData.InheritedFrom, "InheritedFrom", null, null);
        cCol.Add(contentEditData.IsInherited, "IsInherited", null, null);
        cCol.Add(contentEditData.IsMetaComplete, "IsMetaComplete", null, null);
        cCol.Add(contentEditData.IsPrivate, "IsPrivate", null, null);
        cCol.Add(contentEditData.IsPublished, "IsPublished", null, null);
        cCol.Add(contentEditData.IsSearchable, "IsSearchable", null, null);
        cCol.Add(contentEditData.IsXmlInherited, "IsXmlInherited", null, null);
        cCol.Add(contentEditData.LanguageDescription, "LanguageDescription", null, null);
        cCol.Add(contentEditData.LanguageId, "ContentLanguage", null, null);
        cCol.Add(contentEditData.LegacyData, "LegacyData", null, null);
        cCol.Add(contentEditData.ManualAliasId, "ManualAliasId", null, null);
        cCol.Add(contentEditData.MetaData, "MetaData", null, null);
        Microsoft.VisualBasic.Collection mCol = new Microsoft.VisualBasic.Collection();
        object[] meta;
        for (int mi = 0; mi < contentEditData.MetaData.Length; mi++)
        {
            ContentMetaData m = contentEditData.MetaData[mi];
            meta = new object[4];
            meta[1] = m.TypeId.ToString();
            meta[2] = contentEditData.Id;
            meta[3] = m.Text;
            mCol.Add(meta, (mi + 1).ToString(), null, null);
        }
        cCol.Add(mCol, "ContentMetaData", null, null);
        cCol.Add(contentEditData.Path, "Path", null, null);
        cCol.Add(contentEditData.Permissions, "Permissions", null, null);
        cCol.Add(contentEditData.Quicklink, "Quicklink", null, null);
        cCol.Add(contentEditData.Status, "ContentStatus", null, null);
        cCol.Add(contentEditData.TemplateConfiguration, "TemplateConfiguration", null, null);
        cCol.Add(contentEditData.Type, "Type", null, null);
        cCol.Add(contentEditData.Updates, "Updates", null, null);
        cCol.Add(contentEditData.XmlConfiguration, "XmlConfiguration", null, null);
        cCol.Add(contentEditData.XmlInheritedFrom, "XmlInheritedFrom", null, null);
        if (!contentEditData.FileChanged)
            cCol.Add("False", "FileChanged", null, null);

        return cCol;
    }

    public Dictionary<long, string> GetFolderList(int rootFolder, bool showNameOnly)
    {
        Dictionary<long, string> folders = new Dictionary<long, string>();

        Ektron.Cms.Framework.Core.Folder.Folder folder = new Ektron.Cms.Framework.Core.Folder.Folder();
        Criteria<FolderProperty> criteria = new Criteria<FolderProperty>();
        criteria.AddFilter(FolderProperty.Id, CriteriaFilterOperator.EqualTo, rootFolder);
        criteria.PagingInfo.RecordsPerPage = int.MaxValue - 1;
        List<FolderData> folderList = folder.GetList(criteria);

        if (folderList == null || folderList.Count == 0)
            return folders;

        string root = folderList[0].NameWithPath;
        folders.Add(rootFolder, showNameOnly ? folderList[0].Name : "/");

        List<long> orderedFolders = new List<long>();
        orderedFolders.Add(rootFolder);
        AddFolders(new long[] { rootFolder }, root, showNameOnly, ref folders, ref orderedFolders);

        // Need to sort
        Dictionary<long, string> ordered = new Dictionary<long, string>();
        for (int i = 0; i < orderedFolders.Count; i++)
            if (!ordered.ContainsKey(orderedFolders[i]))
                ordered.Add(orderedFolders[i], folders[orderedFolders[i]]);

        return ordered;
    }
    private void AddFolders(long[] parentFolders, string topPath, bool showNameOnly, ref Dictionary<long, string> folders, ref List<long> orderedFolders)
    {
        List<FolderData> folderList = null;
        Ektron.Cms.Framework.Core.Folder.Folder folder = new Ektron.Cms.Framework.Core.Folder.Folder();
        Criteria<FolderProperty> criteria = new Criteria<FolderProperty>(FolderProperty.FolderPath, EkEnumeration.OrderByDirection.Ascending);
        if (parentFolders.Length > 0)
        {
            criteria.AddFilter(FolderProperty.ParentId, CriteriaFilterOperator.In, parentFolders);
            folderList = folder.GetList(criteria);
        }
        List<long> oldIds = new List<long>(orderedFolders);

        if (folderList == null || folderList.Count == 0)
            return;

        foreach (FolderData f in folderList)
        {
            if (!folders.ContainsKey(f.Id))
            {
                folders.Add(f.Id, showNameOnly ? f.Name : ("/" + f.NameWithPath.Substring(topPath.Length - 1)));
            }
        }

        // Add in the IDs for sorting
        List<long> ids;
        foreach (long parentId in parentFolders)
        {
            ids = folderList.Where(x => x.ParentId == parentId).Select<FolderData, long>(x => x.Id).ToList();
            orderedFolders.InsertRange(orderedFolders.IndexOf(parentId) + 1, ids);
        }

        ids = folderList.Select<FolderData, long>(x => x.Id).ToList();
        ids = ids.ToList().Where(x => !oldIds.Contains(x)).ToList();
        AddFolders(ids.ToArray(), topPath, showNameOnly, ref folders, ref orderedFolders);
    }
    private string DecodeString(string encoded)
    {
        return Server.UrlDecode(encoded).Replace("'", "&#39;");
    }
    private string GetMessage(string message)
    {
        return this.contAPI.EkMsgRef.GetMessage(message);
    }
}
