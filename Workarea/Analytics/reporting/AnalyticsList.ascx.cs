using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Framework.Analytics.BusinessAnalytics;
using Ektron.Cms.Common;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;


public partial class Analytics_reporting_AnalyticsList : WorkareaBaseControl
{

	#region Private Members

	private DateTime _startDate = DateTime.Today.AddDays(-7);
    private DateTime _endDate = DateTime.Today.AddDays(1);
    private int _languageid;
    private string _scheme = string.Empty;
    private string _hostUrl = string.Empty;
    private string _appPath = string.Empty;
    private string _sitePath = string.Empty;

	#endregion

	#region Properties

    private string _reportName = "most commented";
    public string ReportName
	{
        get { return _reportName; }
        set { _reportName = value; }
	}
    private int _pageSize;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = value; }
    }
    private EkEnumeration.OrderByDirection _orderDirection;
    public EkEnumeration.OrderByDirection OrderDirection
    {
        get { return _orderDirection; }
        set { _orderDirection = value; }
    }
    private long _filterId;
    public long FilterId
    {
        get { return _filterId; }
        set { _filterId = value; }
    }
    private string _filterType = "";
    public string FilterType
    {
        get { return _filterType; }
        set { _filterType = value; }
    }
    private bool _filterRecursion = false;
    public bool FilterRecursion
    {
        get { return _filterRecursion; }
        set { _filterRecursion = value; }
    }
    private long _siteId;
    public long SiteId
    {
        get { return _siteId; }
        set { _siteId = value; }
    }
	public DateTime StartDate
	{
		get { return _startDate; }
		set { _startDate = value; }
	}

	#endregion


	protected void Page_Load(object sender, EventArgs e)
	{
        _scheme = Page.Request.Url.Scheme + "://"; // System.Web.HttpContext.Current.Current.Request.Url.Scheme + "://";
        _hostUrl = ContentApi.RequestInformationRef.HostUrl;
        _appPath = ContentApi.AppPath;
        _sitePath = ContentApi.SitePath;

        Css.RegisterCss(this, _appPath + "analytics/reporting/css/reports.css", "BAReportsCss");
        if (!Page.IsPostBack)
		{
			RegisterScripts();
            if (Page.Request.QueryString["langtype"] != null)
            {
                _languageid = Convert.ToInt32(Page.Request.QueryString["langtype"]);
            }
		}
        GetReport();
		ltrlNoRecords.Text = GetMessage("lbl no records");
	}

	public void RegisterScripts()
	{
		//JS.RegisterJS(this, JS.ManagedScript.EktronJS);
	}

    private void GetReport()
    {
        try
        {
            IContentQueryRequest queryObject = EventReporter.CreateContentQueryRequest();
            queryObject.ReportName = _reportName;
            queryObject.OrderByDirection = OrderDirection;
            queryObject.EventStartDate = _startDate;
            queryObject.EventEndDate = _endDate;
            queryObject.PagingInfo.RecordsPerPage = _pageSize;
            if (_languageid > 0)
            {
                queryObject.LanguageId = _languageid;
            }
            switch (_filterType.ToLower())
            {
                case "folder":
                    queryObject.FolderId = _filterId;
                    queryObject.IncludeChildFolders = _filterRecursion;
                    break;
                case "taxonomy":
                    queryObject.TaxonomyId = _filterId;
                    queryObject.IncludeChildTaxonomy = _filterRecursion;
                    break;
                default:
                    break;
            }

            IList<IContentEventItem> reportResults = EventReporter.GetList(queryObject);

            if (0 == reportResults.Count)
            {
                lblNoRecords.Visible = true;
            }
            else
            {
                resultList.InnerHtml = string.Empty;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                try
                {
                    foreach (IContentEventItem contentITem in reportResults)
                    {
                        ListItem item = new ListItem();
                        if (queryObject.ReportName.ToLower().Contains("rated"))
                        {
                            //item.Text = String.Format("{0} {1}", contentITem.Title, Server.HtmlDecode(GenerateStars(Convert.ToInt32(contentITem.Average))));
                            sb.Append("<li><span class=\"BA_RatingStars\">" + GenerateStars(Convert.ToInt32(contentITem.Average)));
                            //The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
                            if (contentITem.Count != 0)
                            {
                                sb.Append("<span class=\"BA_Counts\">(" + contentITem.Count + ")</span>");
                            }
                            sb.Append("</span><span class=\"BA_Link\">" + GenerateQuicklink(contentITem.Title, contentITem.Quicklink) + "</span></li>");
                        }
                        else
                        {
                            //item.Text = String.Format("{0,-10} ({1})", GenerateQuicklink(contentITem.Title, contentITem.Quicklink), contentITem.Count);
                            sb.AppendLine("<li><span class=\"BA_Link\">" + GenerateQuicklink(contentITem.Title, contentITem.Quicklink) + "</span><span class=\"BA_Scores\">(" + contentITem.Count + ")</span></li>");
                        }
                    }

                    resultList.Visible = true;
                    resultList.InnerHtml = sb.ToString();
                }
                catch (Exception ex)
                {
                    ltrlNoRecords.Text = ex.Message;
                    lblNoRecords.Visible = true;
                }

            }
        }
        catch (Exception ex)
        {
            ltrlNoRecords.Text = ex.Message;
            lblNoRecords.Visible = true;        
        }
    }

    private string GenerateQuicklink(string title, string filename)
    {
        //System.Text.StringBuilder sbRating = new System.Text.StringBuilder();
        //sbRating.Append("<a href=\"" + filename  + "\" title=\"" + title + " \">" + title + "</a>");
        //return sbRating.ToString();
        string titleHtml = EkFunctions.HtmlDecode(title);
        string filenameHtml = EkFunctions.HtmlEncode(filename);
		//Multisite links are returned as fully qualified. Skip the link formation
		if (filename.IndexOf("http://") > -1 || filename.IndexOf("https://") > -1){
            return "<a href=\"" + filename + "\" title=\"" + titleHtml + " \">" + titleHtml + "</a>";
        }
        else {
            return "<a href=\"" + _scheme + _hostUrl + filenameHtml + "\" title=\"" + titleHtml + " \">" + titleHtml + "</a>";
        }
    }

    private string GenerateStars(int irating) 
    {
        System.Text.StringBuilder sbRating = new System.Text.StringBuilder(); 
        for (int i = 1; i <= 10; i++)
        {
            sbRating.Append("<img border=\"0\" src=\"" + _scheme + _hostUrl + _appPath + "images/ui/icons/"); 
            if ((i % 2) > 0)
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyLeft.png");
                }
                else
                {
                    sbRating.Append("starLeft.png");
                }
            }
            else
            {
                if (irating < i)
                {
                    sbRating.Append("starEmptyRight.png");
                }
                else
                {
                    sbRating.Append("starRight.png");
                }
            }
            sbRating.Append("\" />");
        }
        return sbRating.ToString();
    }
}
