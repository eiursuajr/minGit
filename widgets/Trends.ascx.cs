using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Text;
using Ektron.Cms.Framework.Analytics.BusinessAnalytics;
using System.Collections.Generic;
using System.Configuration;

public partial class widgets_Trends : System.Web.UI.UserControl, IWidget
{
    #region properties

    private string _Asc;
    private string _Desc;
    private string _Taxonomy;
    private string _Folder;
    private string _None;
    private string _NotSet;
    private string _appPath;
    private string _userLang;
    private bool _skipSetOutput = false;
    private string _sitepath;
    private Ektron.Cms.Common.EkMessageHelper _refMsg;

    private string _ReportName;
    [WidgetDataMember("Most Viewed")]
    public string ReportName { get { return _ReportName; } set { _ReportName = value; } }

    private int _Duration;
    [WidgetDataMember(7)]
    public int Duration { get { return _Duration; } set { _Duration = value; } }

    private int _PageSize;
    [WidgetDataMember(5)]
    public int PageSize { get { return _PageSize; } set { _PageSize = value; } }

    private EkEnumeration.OrderByDirection _OrderDirection;
    [WidgetDataMember(EkEnumeration.OrderByDirection.Descending)]
    public EkEnumeration.OrderByDirection OrderDirection { get { return _OrderDirection; } set { _OrderDirection = value; } }

    private string _FilterObject;
    [WidgetDataMember("")]
    public string FilterObject { get { return _FilterObject; } set { _FilterObject = value; } }

    private string _FilterObjectId;
    [WidgetDataMember("")]
    public string FilterObjectId { get { return _FilterObjectId; } set { _FilterObjectId = value; } }

    private bool _FilterObjectRecursion;
    [WidgetDataMember(false)]
    public bool FilterObjectRecursion { get { return _FilterObjectRecursion; } set { _FilterObjectRecursion = value; } }

    private string _FilterDisplayText;
    [WidgetDataMember("")]
    public string FilterDisplayText { get { return _FilterDisplayText; } set { _FilterDisplayText = value; } }

    #endregion

    IWidgetHost _host;

    #region protected
    protected void Page_Init(object sender, EventArgs e)
    {
        CommonApi api = new CommonApi();
        _sitepath = api.SitePath;
        _appPath = api.AppPath;
        _userLang = api.UserLanguage.ToString();
        SiteAPI refSiteApi = new SiteAPI();
        _refMsg = refSiteApi.EkMsgRef;
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Trends Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        string myPath = string.Empty;
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ek_helpDomainPrefix"]))
        {
            string helpDomain = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
            if ((helpDomain.IndexOf("[ek_cmsversion]") > 1))
            {
                myPath = helpDomain.Replace("[ek_cmsversion]", new CommonApi().RequestInformationRef.Version);
            }
            else
            {
                myPath = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
            }
        }
        else
        {
            myPath = _appPath + "/help";
        }
        _host.HelpFile = myPath + "/Main_Import_file/Trends_Widget.htm";
        LocalizeMarkup();
        UpdateMarkup();
        ViewSet.SetActiveView(View);
    }
    
    void EditEvent(string settings)
    {
        ReportTypeList.ClearSelection();
        FilterObjectsList.ClearSelection();
        try
        {
            ReportTypeList.Items.FindByValue(_ReportName).Selected = true;
            if (_FilterObject.Length > 0)
            {
                FilterObjectsList.Items.FindByValue(_FilterObject).Selected = true;
            }
        }
        catch { }
        OrderDirList.ClearSelection();
        string orderdir = "descending";
        if (EkEnumeration.OrderByDirection.Ascending == _OrderDirection)
        {
            orderdir = "ascending";
        }
        OrderDirList.Items.FindByValue(orderdir).Selected = true;
        DurationTextBox.Text = _Duration.ToString();
        PageSizeTextBox.Text = _PageSize.ToString();
        FilterIdTextBox.Value = _FilterObjectId;
        chkFilterObjectRecur.Value = _FilterObjectRecursion.ToString();
        filterDisplay.Text = (_FilterDisplayText.Length > 0 ? _FilterDisplayText : _NotSet);

        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _ReportName = ReportTypeList.SelectedItem.Value;
        _Duration = Convert.ToInt32(DurationTextBox.Text);
        if ("descending" == OrderDirList.SelectedItem.Value)
        {
            _OrderDirection = EkEnumeration.OrderByDirection.Descending;
        }
        else
        {
            _OrderDirection = EkEnumeration.OrderByDirection.Ascending;
        }
        _PageSize = Convert.ToInt32(PageSizeTextBox.Text);
        _FilterObject = FilterObjectsList.SelectedItem.Value;
        _FilterObjectId = FilterIdTextBox.Value;
        _FilterObjectRecursion = Convert.ToBoolean(chkFilterObjectRecur.Value);
        _FilterDisplayText = filterDisplay.Text;
        
        updateControl();

        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        if (!_skipSetOutput)
        {
            ReportTypeList.ClearSelection();
            FilterObjectsList.ClearSelection();
            try
            {
                ReportTypeList.Items.FindByValue(_ReportName).Selected = true;
                if (_FilterObject.Length > 0)
                {
                    FilterObjectsList.Items.FindByValue(_FilterObject).Selected = true;
                }
            }
            catch { }
            OrderDirList.ClearSelection();
            string orderdir = "descending";
            if (EkEnumeration.OrderByDirection.Ascending == _OrderDirection)
            {
                orderdir = "ascending";
            }
            OrderDirList.Items.FindByValue(orderdir).Selected = true;
            DurationTextBox.Text = _Duration.ToString();
            PageSizeTextBox.Text = _PageSize.ToString();
            FilterIdTextBox.Value = (_FilterObject != "" ? _FilterObjectId : "");
            chkFilterObjectRecur.Value = _FilterObjectRecursion.ToString();
            filterDisplay.Text = (_FilterDisplayText.Length > 0 ? _FilterDisplayText : _NotSet);
            linkDelete.Visible = false;
            if (_FilterObjectId != "")
            {
                linkDelete.Visible = true;
            }

            ContentAPI capi = new ContentAPI();
            if (_FilterObjectId.Length > 0)
            {
                switch (_FilterObject)
                {
                    case "folder":
                        //prepare for re-select folder path on the folder tree
                        string folderpath = string.Empty;
                        Int64 folderid = Convert.ToInt64(FilterIdTextBox.Value);
                        folderpath = FilterIdTextBox.Value;
                        while (folderid != 0)
                        {
                            folderid = capi.GetParentIdByFolderId(folderid);
                            if (folderid >= 0) folderpath += "," + folderid.ToString();
                        }
                        tbFolderPath.Text = folderpath;
                        break;
                    case "taxonomy":
                        //prepare for re-select taxonomy path on the folder tree
                        string taxonomyPath = string.Empty;
                        Int64 taxId = Convert.ToInt64(FilterIdTextBox.Value);
                        TaxonomyBaseData[] TaxData = capi.EkContentRef.GetTaxonomyRecursiveToParent(taxId, Convert.ToInt32(_userLang), 0);
                        for (int i = 0; i < TaxData.Length; i++)
                        {
                            taxId = TaxData[i].Id;
                            if (taxId > 0)
                            {
                                if (0 == i)
                                {
                                    taxonomyPath = taxId.ToString();
                                }
                                else
                                {
                                    taxonomyPath += "," + taxId.ToString();
                                }
                            }
                        }
                        tbTaxonomyPath.Text = taxonomyPath;
                        break;
                    default:
                        break;
                }
            }
        }
        _skipSetOutput = false;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    protected void linkPopup_Click(object sender, EventArgs e)
    {
        trends_foldertree.Visible = false;
        trends_taxtree.Visible = false;
        if (FilterObjectsList.SelectedValue != "")
        {
            string webserviceURL = string.Empty;
            string recursionLabel = string.Empty;
            switch (FilterObjectsList.SelectedValue)
            {
                case "folder":
                    webserviceURL = _sitepath + "widgets/listsummary/LSHandler.ashx";
                    JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
                    JS.RegisterJSInclude(this, _sitepath + "widgets/ListSummary/behavior.js", "ListSummaryWidgetBehaviorJS");
                    JS.RegisterJSBlock(this, "Ektron.PFWidgets.ListSummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ListSummary.setupAll(); ", "EktronPFWidgetsLSInit");
                    Css.RegisterCss(this, _sitepath + "widgets/listsummary/LSStyle.css", "LSWidgetCSS");
                    trends_foldertree.Visible = true;
                    recursionLabel = _refMsg.GetMessage("lbl include child folder");
                    uxFilterType.Value = "folder";
                    break;
                case "taxonomy":
                    webserviceURL = _sitepath + "widgets/contentblock/CBHandler.ashx";
                    JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
                    JS.RegisterJSInclude(this, _sitepath + "widgets/contentblock/behavior.js", "ContentWidgetBehaviorJS");
                    Css.RegisterCss(this, _sitepath + "widgets/contentblock/CBStyle.css", "CBWidgetCSS");
                    JS.RegisterJSBlock(this, "Ektron.PFWidgets.ContentBlock.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ContentBlock.Taxonomy.configTaxonomyTreeView();", "EktronPFWidgetsCBInit");
                    trends_taxtree.Visible = true;
                    recursionLabel = _refMsg.GetMessage("lbl include child subcategory");
                    uxFilterType.Value = "taxonomy";
                    break;
                default:
                    break;
            }
            chkRecursion.Checked = Convert.ToBoolean(chkFilterObjectRecur.Value);
            lblRecursion.Text = recursionLabel;
            ViewSet.SetActiveView(FilterPopup);
        }
    }

    protected void SavePopupButton_Click(object sender, EventArgs e)
    {
        if (uxFilterType.Value == "taxonomy" && TaxId.Value.Length > 0) //taxnomy selector
        {
            _FilterObjectId = TaxId.Value;
            _FilterObject = "taxonomy";
            _FilterObjectRecursion = chkRecursion.Checked;
        }
        else if (uxFilterType.Value == "folder" && folderId.Value.Length > 0) //folder selector
        {
            _FilterObjectId = folderId.Value;
            _FilterObject = "folder";
            _FilterObjectRecursion = chkRecursion.Checked;
        }
        if (_FilterObjectId.Length > 0)
        {
            _FilterDisplayText = getFilterDisplayValue(resTitle.Value);
        }
        else
        {
            JS.RegisterJSBlock(this, "alert(\"" + _NotSet + "\");", this.ClientID + "_SaveClickedScript");
        }
        ViewSet.SetActiveView(Edit);
    }
    protected void CancelPopupButton_Click(object sender, EventArgs e)
    {
        _skipSetOutput = true;
        ViewSet.SetActiveView(Edit);
    }

    protected void linkDelete_Click(object sender, EventArgs e)
    {
        FilterIdTextBox.Value = "";
        chkFilterObjectRecur.Value = "false";
        chkRecursion.Checked = false;
        filterDisplay.Text = "";
        TaxId.Value = "";
        tbTaxonomyPath.Text = "";
        folderId.Value = "";
        tbFolderPath.Text = "";
        tbData.Text = "";
        tbFolderPath.Text = "";
        FilterDisplayText = "";
        FilterObjectId = "";
        FilterObject = "";
    }
    #endregion

    #region private

    private void updateControl()
    {
        AnalyticList.ReportName = _ReportName;
        AnalyticList.OrderDirection = _OrderDirection;
        AnalyticList.StartDate = DateTime.Today.AddDays(-_Duration);
        AnalyticList.PageSize = _PageSize;
        if (_FilterObject != "" && _FilterObjectId != "")
        {
            AnalyticList.FilterType = _FilterObject;
            AnalyticList.FilterId = Convert.ToInt64(_FilterObjectId);
            AnalyticList.FilterRecursion = _FilterObjectRecursion;
        }
    }

    private void UpdateMarkup()
    {
        ReportTypeList.Items.Clear();
        IList<String> names = EventReporter.GetReportNameList();
        foreach (string name in names)
        {
            string localizedName = _refMsg.GetMessage("lbl " + name);
            ReportTypeList.Items.Add(new ListItem(localizedName, name));
        }
        OrderDirList.Items.Clear();
        OrderDirList.Items.Add(new ListItem(_Asc, "ascending"));
        OrderDirList.Items.Add(new ListItem(_Desc, "descending"));
        FilterObjectsList.Items.Clear();
        //FilterObjectsList.Items.Add(new ListItem(_None, ""));
        FilterObjectsList.Items.Add(new ListItem(_Folder, "folder"));
        FilterObjectsList.Items.Add(new ListItem(_Taxonomy, "taxonomy"));
        FilterIdTextBox.Value = "";
        linkDelete.ImageUrl = _appPath + "images/UI/Icons/delete.png";
        linkPopup.ImageUrl = _appPath + "images/UI/Icons/folderView.png";

        updateControl();    
    }

    private void LocalizeMarkup()
    {
        lblReportType.Text = _refMsg.GetMessage("lbl report type");
        lblDuration.Text = _refMsg.GetMessage("lbl number of days to report");
        lblOrderDir.Text = _refMsg.GetMessage("lbl order direction");
        _Asc = _refMsg.GetMessage("lbl sort ascending");
        _Desc = _refMsg.GetMessage("lbl sort descending");
        lblPageSize.Text = _refMsg.GetMessage("lbl maximum number of items");
        lblFilterBy.Text = _refMsg.GetMessage("lbl filter results by");
        string InvalidNumericMsg = _refMsg.GetMessage("js alert valid numeric");
        RegularExpressionValidator1.ErrorMessage = InvalidNumericMsg;
        RegularExpressionValidator2.ErrorMessage = InvalidNumericMsg;
        _Taxonomy = _refMsg.GetMessage("lbl taxonomy");
        _Folder = _refMsg.GetMessage("lbl folder");
        _None = _refMsg.GetMessage("lbl site wide");
        linkPopup.AlternateText = _refMsg.GetMessage("lbl show selector");
        linkDelete.AlternateText = _refMsg.GetMessage("generic delete title");
        lblRecursion.Text = _refMsg.GetMessage("lbl include child folder");
        _NotSet = _refMsg.GetMessage("lbl no filter selected");

        CancelButton.Text = _refMsg.GetMessage("btn cancel");
        SaveButton.Text = _refMsg.GetMessage("btn save");
    }

    private string getFilterDisplayValue(string resTitle)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (_FilterObject != "")
        {
            sb.Append(resTitle + " \xAB");
            sb.Append(FilterObjectsList.Items.FindByValue(_FilterObject).Text + ":" + _FilterObjectId);  
            if (true == _FilterObjectRecursion)
            {
                sb.Append( " + " + _refMsg.GetMessage("lbl children")); 
            }
            sb.Append("\xBB");
        }
	    return sb.ToString();
    }
    #endregion
}

