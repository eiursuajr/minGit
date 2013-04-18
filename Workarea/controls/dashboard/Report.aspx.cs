using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Text;
using Ektron.Cms.BusinessObjects.Localization;

public partial class msdashboard_Report : System.Web.UI.Page
{
    protected ContentAPI contAPI = new ContentAPI();
    protected ILocalizationManager l10nManager = null;
    #region Properties
    #region Filters
    private long FilterFolderId { get; set; }
    private string FilterTitle { get; set; }
    private int FilterLocale { get; set; }
    public Ektron.Cms.Localization.LocalizationState FilterLocStatus { get; set; }
    private string FilterContentStatus { get; set; }
    private DateTime FilterLastModifiedStart { get; set; }
    private DateTime FilterLastModifiedEnd { get; set; }
    private DateTime FilterDateCreatedStart { get; set; }
    private DateTime FilterDateCreatedEnd { get; set; }
    public bool FilterLocaleNotIn { get; set; }
    private long FilterContentId { get; set; }
    private long FilterAuthorId { get; set; }
    private int FilterLanguageId { get; set; }
    #endregion
    private bool IsExcelExport = false;
    protected bool IncludeFolderPath = false;
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

    protected void Page_Load(object sender, EventArgs e)
    {
        l10nManager = ObjectFactory.GetLocalizationManager(contAPI.RequestInformationRef);
        IsExcelExport = Request.QueryString["out"] == "xls";
        IncludeFolderPath = Request.QueryString.AllKeys.Contains("fp") ?
            (Request.QueryString["fp"] == "1") : IsExcelExport;

        NameValueCollection filters = HttpUtility.ParseQueryString(Request.QueryString["f"]);
        foreach (string f in filters.Keys)
            SetFilterValue(f, filters[f]);

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
            criteria2.AddFilter(ReportingProperty.LastModified, CriteriaFilterOperator.LessThanOrEqualTo, FilterLastModifiedEnd);
        if (FilterDateCreatedStart != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, FilterDateCreatedStart);
        if (FilterDateCreatedEnd != DateTime.MinValue)
            criteria2.AddFilter(ReportingProperty.DateCreated, CriteriaFilterOperator.LessThanOrEqualTo, FilterDateCreatedEnd);
        if (FilterContentId > 0)
            criteria2.AddFilter(ReportingProperty.ContentId, CriteriaFilterOperator.EqualTo, FilterContentId);
        if (FilterAuthorId > 0)
            criteria2.AddFilter(ReportingProperty.AuthorId, CriteriaFilterOperator.EqualTo, FilterAuthorId);
        if (FilterLanguageId > 0)
            criteria2.AddFilter(ReportingProperty.LanguageId, CriteriaFilterOperator.EqualTo, FilterLanguageId);

        criteria2.GetMetadata = true;
        criteria2.PagingInfo.CurrentPage = 0;
        criteria2.PagingInfo.RecordsPerPage = int.MaxValue;
        Pair sort = GetSort(Request.QueryString["s"]);
        criteria2.OrderByField = (ReportingProperty)sort.First;
        criteria2.OrderByDirection = (EkEnumeration.OrderByDirection)sort.Second;

        List<ReportingData> contentlist2 = l10nManager.GetReport(criteria2);

        int pageCount = 0;
        int totalCount = criteria2.PagingInfo.TotalRecords;
        int totalPages = criteria2.PagingInfo.TotalPages;
        int currentPage = criteria2.PagingInfo.CurrentPage;

        if (IsExcelExport)
        {
            pnlPrint.Visible = false;
            pnlExcel.Visible = true;
            rptExcel.DataSource = contentlist2;
            rptExcel.DataBind();

            Response.Clear();
            Response.ContentType = "application/ms-excel";
            Response.AppendHeader("content-disposition", "attachment; filename=LocReport_" + DateTime.Now.Ticks.ToString() + ".xls");
            Response.Write(RenderControl(plcExcel));
            Response.End();
        }
        else
        {
            pnlPrint.Visible = true;
            pnlExcel.Visible = false;
            rptPrint.DataSource = contentlist2;
            rptPrint.DataBind();

            lResultCount.Text = totalCount.ToString() + " row" + (totalCount == 1 ? string.Empty : "s");
        }
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
                    FilterFolderId = long.Parse(value);
                    break;
                case "title":
                    FilterTitle = value;
                    break;
                case "locale":
                    FilterLocaleNotIn = value.StartsWith("!");
                    FilterLocale = int.Parse(value.Replace("!", string.Empty)); 
                    break;
                case "locstatus":
                    FilterLocStatus = (Ektron.Cms.Localization.LocalizationState)Enum.Parse(typeof(Ektron.Cms.Localization.LocalizationState), value);
                    break;
                case "contentstatus":
                    FilterContentStatus = value;
                    break;
                case "lastmodifiedstart":
                    FilterLastModifiedStart = DateTime.Parse(value);
                    break;
                case "lastmodifiedend":
                    FilterLastModifiedEnd = DateTime.Parse(value);
                    break;
                case "datecreatedstart":
                    FilterDateCreatedStart = DateTime.Parse(value);
                    break;
                case "datecreatedend":
                    FilterDateCreatedEnd = DateTime.Parse(value);
                    break;
                case "contentid":
                    FilterContentId = long.Parse(value);
                    break;
                case "authorid":
                    FilterAuthorId = long.Parse(value);
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
        ReportingProperty.ContentStatus,
        ReportingProperty.LastModified,
        ReportingProperty.DateCreated,
        ReportingProperty.ContentId,
        ReportingProperty.AuthorId,
        ReportingProperty.FolderId,
        ReportingProperty.LanguageId};

    private ReportingProperty GetPropertyFromIndex(int index)
    {
        if (index > 0 && index <= PropertyArray.Length)
            return PropertyArray[index - 1];

        return PropertyArray[0];
    }
protected string RenderControl(Control control)
    {
        System.IO.StringWriter sw = new System.IO.StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        control.RenderControl(htw);

        htw.Flush();
        return sw.ToString();
    }
    protected string GetLocStatus(object value)
    {
        Ektron.Cms.Localization.LocalizationState state = string.IsNullOrEmpty(value.ToString()) ?
            Ektron.Cms.Localization.LocalizationState.Undefined :
            (Ektron.Cms.Localization.LocalizationState)Enum.Parse(typeof(Ektron.Cms.Localization.LocalizationState), value.ToString());

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
    private Dictionary<int, string> _shortLocaleList = null;
    private object _shortLocaleListLock = new object();
    public Dictionary<int, string> GetShortLocaleList()
    {
        if (_shortLocaleList == null)
            lock (_shortLocaleListLock)
                if (_shortLocaleList == null)
                {
                    Ektron.Cms.Framework.Localization.LocaleManager locale = new Ektron.Cms.Framework.Localization.LocaleManager();
                    Criteria<Ektron.Cms.Localization.LocaleProperty> criteria = new Criteria<Ektron.Cms.Localization.LocaleProperty>();
                    criteria.AddFilter(Ektron.Cms.Localization.LocaleProperty.Enabled, CriteriaFilterOperator.EqualTo, true);
                    List<Ektron.Cms.Localization.LocaleData> locales = locale.GetList(criteria);
                    _shortLocaleList = new Dictionary<int, string>();
                    foreach (Ektron.Cms.Localization.LocaleData l in locales)
                        if (!_shortLocaleList.ContainsKey(l.LCID))
                            _shortLocaleList.Add(l.LCID, l.Loc);
                }

        return _shortLocaleList;
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
    protected string GenerateAbsoluteUrl(string pagePath)
    {
        string reqUrl = Request.Url.ToString();
        string workareaRoot = Page.ResolveUrl("~/workarea");
        string baseUrl = reqUrl.Substring(0, reqUrl.ToLower().IndexOf(workareaRoot.ToLower()));
        if (baseUrl.EndsWith("/"))
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
        return baseUrl + Page.ResolveUrl(pagePath);
    }
    protected string GetContentTypeIcon(ReportingData data, bool titleOnly)
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
            default:
                switch ((byte)data.ContentType)
                {
                    case 101:
                        sb.Append(Page.ResolveClientUrl("~/workarea/images/ui/icons/filetypes/word.png"));
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

        return titleOnly ? title : sb.ToString();
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
}
