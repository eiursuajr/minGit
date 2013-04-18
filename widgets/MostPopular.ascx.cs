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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Ektron.Cms.Analytics;
using Ektron.Cms.Framework.Analytics.BusinessAnalytics;
using System.Configuration;


public partial class widgets_MostPopular : System.Web.UI.UserControl, IWidget
{
    #region properties
    private int _PageSize = 5;
    private string _EditTab1String;
    private string _EditTab2String;
    private string _EditTab3String;
    private string _EditTab4String;
    private string _Taxonomy;
    private string _Folder;
    private string _None;
    private string _NotSet;
    private string _appPath;
    private int _userLang;
    private bool _skipSetOutput = false;
    private string _sitepath;
    private Ektron.Cms.Common.EkMessageHelper _refMsg;

    private string _Tab1String;
    [WidgetDataMember("Most Viewed")]
    public string Tab1String { get { return _Tab1String; } set { _Tab1String = value; } }
    private string _Tab2String;
    [WidgetDataMember("Most Emailed")]
    public string Tab2String { get { return _Tab2String; } set { _Tab2String = value; } }
    private string _Tab3String;
    [WidgetDataMember("Most Commented")]
    public string Tab3String { get { return _Tab3String; } set { _Tab3String = value; } }
    private string _Tab4String;
    [WidgetDataMember("Highest Rated")]
    public string Tab4String { get { return _Tab4String; } set { _Tab4String = value; } }

    private int _Tab1Duration;
    [WidgetDataMember(7)]
    public int Tab1Duration { get { return _Tab1Duration; } set { _Tab1Duration = value; } }
    private int _Tab2Duration;
    [WidgetDataMember(7)]
    public int Tab2Duration { get { return _Tab2Duration; } set { _Tab2Duration = value; } }
    private int _Tab3Duration;
    [WidgetDataMember(7)]
    public int Tab3Duration { get { return _Tab3Duration; } set { _Tab3Duration = value; } }
    private int _Tab4Duration;
    [WidgetDataMember(7)]
    public int Tab4Duration { get { return _Tab4Duration; } set { _Tab4Duration = value; } }

    private string _Tab1Event;
    [WidgetDataMember("Most Viewed")]
    public string Tab1Event { get { return _Tab1Event; } set { _Tab1Event = value; } }
    private string _Tab2Event;
    [WidgetDataMember("Most Emailed")]
    public string Tab2Event { get { return _Tab2Event; } set { _Tab2Event = value; } }
    private string _Tab3Event;
    [WidgetDataMember("Most Commented")]
    public string Tab3Event { get { return _Tab3Event; } set { _Tab3Event = value; } }
    private string _Tab4Event;
    [WidgetDataMember("Highest Rated")]
    public string Tab4Event { get { return _Tab4Event; } set { _Tab4Event = value; } }

    private string _Tab1FilterObject;
    [WidgetDataMember("")]
    public string Tab1FilterObject { get { return _Tab1FilterObject; } set { _Tab1FilterObject = value; } }
    private string _Tab2FilterObject;
    [WidgetDataMember("")]
    public string Tab2FilterObject { get { return _Tab2FilterObject; } set { _Tab2FilterObject = value; } }
    private string _Tab3FilterObject;
    [WidgetDataMember("")]
    public string Tab3FilterObject { get { return _Tab3FilterObject; } set { _Tab3FilterObject = value; } }
    private string _Tab4FilterObject;
    [WidgetDataMember("")]
    public string Tab4FilterObject { get { return _Tab4FilterObject; } set { _Tab4FilterObject = value; } }

    private string _Tab1FilterObjectId;
    [WidgetDataMember("")]
    public string Tab1FilterObjectId { get { return _Tab1FilterObjectId; } set { _Tab1FilterObjectId = value; } }
    private string _Tab2FilterObjectId;
    [WidgetDataMember("")]
    public string Tab2FilterObjectId { get { return _Tab2FilterObjectId; } set { _Tab2FilterObjectId = value; } }
    private string _Tab3FilterObjectId;
    [WidgetDataMember("")]
    public string Tab3FilterObjectId { get { return _Tab3FilterObjectId; } set { _Tab3FilterObjectId = value; } }
    private string _Tab4FilterObjectId;
    [WidgetDataMember("")]
    public string Tab4FilterObjectId { get { return _Tab4FilterObjectId; } set { _Tab4FilterObjectId = value; } }

    private bool _Tab1FilterObjectRecursion;
    [WidgetDataMember(false)]
    public bool Tab1FilterObjectRecursion { get { return _Tab1FilterObjectRecursion; } set { _Tab1FilterObjectRecursion = value; } }
    private bool _Tab2FilterObjectRecursion;
    [WidgetDataMember(false)]
    public bool Tab2FilterObjectRecursion { get { return _Tab2FilterObjectRecursion; } set { _Tab2FilterObjectRecursion = value; } }
    private bool _Tab3FilterObjectRecursion;
    [WidgetDataMember(false)]
    public bool Tab3FilterObjectRecursion { get { return _Tab3FilterObjectRecursion; } set { _Tab3FilterObjectRecursion = value; } }
    private bool _Tab4FilterObjectRecursion;
    [WidgetDataMember(false)]
    public bool Tab4FilterObjectRecursion { get { return _Tab4FilterObjectRecursion; } set { _Tab4FilterObjectRecursion = value; } }

    private string _Tab1FilterDisplayText;
    [WidgetDataMember("")]
    public string Tab1FilterDisplayText { get { return _Tab1FilterDisplayText; } set { _Tab1FilterDisplayText = value; } }
    private string _Tab2FilterDisplayText;
    [WidgetDataMember("")]
    public string Tab2FilterDisplayText { get { return _Tab2FilterDisplayText; } set { _Tab2FilterDisplayText = value; } }
    private string _Tab3FilterDisplayText;
    [WidgetDataMember("")]
    public string Tab3FilterDisplayText { get { return _Tab3FilterDisplayText; } set { _Tab3FilterDisplayText = value; } }
    private string _Tab4FilterDisplayText;
    [WidgetDataMember("")]
    public string Tab4FilterDisplayText { get { return _Tab4FilterDisplayText; } set { _Tab4FilterDisplayText = value; } }

    private bool _Tab2Visible;
    [WidgetDataMember(true)]
    public bool Tab2Visible { get { return _Tab2Visible; } set { _Tab2Visible = value; } }
    private bool _Tab3Visible;
    [WidgetDataMember(true)]
    public bool Tab3Visible { get { return _Tab3Visible; } set { _Tab3Visible = value; } }
    private bool _Tab4Visible;
    [WidgetDataMember(true)]
    public bool Tab4Visible { get { return _Tab4Visible; } set { _Tab4Visible = value; } }
    #endregion

    IWidgetHost _host;

    #region protected
    protected void Page_Init(object sender, EventArgs e)
    {
        Err_MostPopularWidget.Visible = false;

        CommonApi api = new CommonApi();
        _sitepath = api.SitePath;
        _appPath = api.AppPath;
        _userLang = api.UserLanguage;
        SiteAPI refSiteApi = new SiteAPI();
        _refMsg = refSiteApi.EkMsgRef;
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.Framework.UI.Packages.jQuery.jQueryUI.ThemeRoller.Register(this);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronUITabsCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        JS.RegisterJSBlock(this, MakeClientScript(), this.ClientID + "_ClientScriptID");
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Most Popular Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        //string myPath = string.Empty;
        //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ek_helpDomainPrefix"]))
        //{
        //    string helpDomain = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
        //    if ((helpDomain.IndexOf("[ek_cmsversion]") > 1))
        //    {
        //        myPath = helpDomain.Replace("[ek_cmsversion]", new CommonApi().RequestInformationRef.Version.Replace(".", "0").Substring(0, 3));
        //    }
        //    else
        //   {
        //        myPath = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
        //    }
        //}
        //else
        //{
        //    myPath = _appPath + "/help";
        //}
        //_host.HelpFile = myPath + "/Main_Import_file/Most_Popular_Widget.htm";
        LocalizeMarkup();
        UpdateMarkup();
        updateControl();
        ViewSet.SetActiveView(View);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PageBuilder pbPage = (Page as PageBuilder);
        if (pbPage != null && pbPage.Pagedata != null && pbPage.Pagedata.Widgets != null)
        {
            int NumOnPage = 0;
            int idxLastMostPopularWidget = 0;
            for (int i = 0; i < pbPage.Pagedata.Widgets.Count; i++)
            {
                if ("MostPopular.ascx" == pbPage.Pagedata.Widgets[i].ControlURL)
                {
                    NumOnPage++;
                    idxLastMostPopularWidget = i;
                }
            }
            if (NumOnPage > 1)
            {
                ViewSet.Visible = false;
                Err_MostPopularWidget.Visible = true;
            }
        }
        else
        {
            ViewSet.Visible = false;
            Err_MostPopularWidget.Visible = true;
            Err_MostPopularWidget.InnerHtml = _refMsg.GetMessage("msg this widget is not available in a dashboard");
            return;
        }
    }
    
    void EditEvent(string settings)
    {
        tab1TextBox.Text = _Tab1String;
        tab2TextBox.Text = _Tab2String;
        tab3TextBox.Text = _Tab3String;
        tab4TextBox.Text = _Tab4String;
        durationTextBox1.Text = _Tab1Duration.ToString();
        durationTextBox2.Text = _Tab2Duration.ToString();
        durationTextBox3.Text = _Tab3Duration.ToString();
        durationTextBox4.Text = _Tab4Duration.ToString();
        ReportTypeList1.ClearSelection();
        ReportTypeList2.ClearSelection();
        ReportTypeList3.ClearSelection();
        ReportTypeList4.ClearSelection();
        FilterObjectsList1.ClearSelection();
        FilterObjectsList2.ClearSelection();
        FilterObjectsList3.ClearSelection();
        FilterObjectsList4.ClearSelection();
        try
        {
            ReportTypeList1.Items.FindByValue(_Tab1Event).Selected = true;
            ReportTypeList2.Items.FindByValue(_Tab2Event).Selected = true;
            ReportTypeList3.Items.FindByValue(_Tab3Event).Selected = true;
            ReportTypeList4.Items.FindByValue(_Tab4Event).Selected = true;
            if (_Tab1FilterObject.Length > 0)
                FilterObjectsList1.Items.FindByValue(_Tab1FilterObject).Selected = true;
            if (_Tab2FilterObject.Length > 0)
                FilterObjectsList2.Items.FindByValue(_Tab2FilterObject).Selected = true;
            if (_Tab3FilterObject.Length > 0)
                FilterObjectsList3.Items.FindByValue(_Tab3FilterObject).Selected = true;
            if (_Tab4FilterObject.Length > 0)
                FilterObjectsList4.Items.FindByValue(_Tab4FilterObject).Selected = true;
        }
        catch { }
        FilterIdTextBox1.Value = (_Tab1FilterObject != "" ? _Tab1FilterObjectId : "");
        FilterIdTextBox2.Value = (_Tab2FilterObject != "" ? _Tab2FilterObjectId : "");
        FilterIdTextBox3.Value = (_Tab3FilterObject != "" ? _Tab3FilterObjectId : "");
        FilterIdTextBox4.Value = (_Tab4FilterObject != "" ? _Tab4FilterObjectId : "");
        chkFilterObjectRecur1.Value = _Tab1FilterObjectRecursion.ToString();
        chkFilterObjectRecur2.Value = _Tab2FilterObjectRecursion.ToString();
        chkFilterObjectRecur3.Value = _Tab3FilterObjectRecursion.ToString();
        chkFilterObjectRecur4.Value = _Tab4FilterObjectRecursion.ToString();
        filterDisplay1.Text = (_Tab1FilterDisplayText.Length > 0 ? _Tab1FilterDisplayText : _NotSet); 
        filterDisplay2.Text = (_Tab2FilterDisplayText.Length > 0 ? _Tab2FilterDisplayText : _NotSet);
        filterDisplay3.Text = (_Tab3FilterDisplayText.Length > 0 ? _Tab3FilterDisplayText : _NotSet);
        filterDisplay4.Text = (_Tab4FilterDisplayText.Length > 0 ? _Tab4FilterDisplayText : _NotSet);
        chkTab2.Checked = _Tab2Visible;
        chkTab3.Checked = _Tab3Visible;
        chkTab4.Checked = _Tab4Visible;

        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        _Tab1String = tab1TextBox.Text;
        _Tab2String = tab2TextBox.Text;
        _Tab3String = tab3TextBox.Text;
        _Tab4String = tab4TextBox.Text;
        _Tab1Duration = Convert.ToInt32(durationTextBox1.Text);
        _Tab2Duration = Convert.ToInt32(durationTextBox2.Text);
        _Tab3Duration = Convert.ToInt32(durationTextBox3.Text);
        _Tab4Duration = Convert.ToInt32(durationTextBox4.Text);
        _Tab1Event = ReportTypeList1.SelectedItem.Value;
        _Tab2Event = ReportTypeList2.SelectedItem.Value;
        _Tab3Event = ReportTypeList3.SelectedItem.Value;
        _Tab4Event = ReportTypeList4.SelectedItem.Value;
        _Tab1FilterObject = FilterObjectsList1.SelectedItem.Value;
        _Tab2FilterObject = FilterObjectsList2.SelectedItem.Value;
        _Tab3FilterObject = FilterObjectsList3.SelectedItem.Value;
        _Tab4FilterObject = FilterObjectsList4.SelectedItem.Value;
        _Tab1FilterObjectId = FilterIdTextBox1.Value;
        _Tab2FilterObjectId = FilterIdTextBox2.Value;
        _Tab3FilterObjectId = FilterIdTextBox3.Value;
        _Tab4FilterObjectId = FilterIdTextBox4.Value;
        _Tab1FilterObjectRecursion = Convert.ToBoolean(chkFilterObjectRecur1.Value);
        _Tab2FilterObjectRecursion = Convert.ToBoolean(chkFilterObjectRecur2.Value);
        _Tab3FilterObjectRecursion = Convert.ToBoolean(chkFilterObjectRecur3.Value);
        _Tab4FilterObjectRecursion = Convert.ToBoolean(chkFilterObjectRecur4.Value);
        _Tab1FilterDisplayText = filterDisplay1.Text;
        _Tab2FilterDisplayText = filterDisplay2.Text;
        _Tab3FilterDisplayText = filterDisplay3.Text;
        _Tab4FilterDisplayText = filterDisplay4.Text;
        _Tab2Visible = chkTab2.Checked;
        _Tab3Visible = chkTab3.Checked;
        _Tab4Visible = chkTab4.Checked;
        updateControl();
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        tab1.InnerHtml = "<a href=\"#fragment-1\" title=\"" + _Tab1String + "\">" + _Tab1String + "</a>";
        tab2.InnerHtml = "<a href=\"#fragment-2\" title=\"" + _Tab2String + "\">" + _Tab2String + "</a>";
        tab3.InnerHtml = "<a href=\"#fragment-3\" title=\"" + _Tab3String + "\">" + _Tab3String + "</a>";
        tab4.InnerHtml = "<a href=\"#fragment-4\" title=\"" + _Tab4String + "\">" + _Tab4String + "</a>";
        tab1_edit.InnerHtml = "<a href=\"#editfragment-1\" title=\"" + _EditTab1String + "\">" + _EditTab1String + "</a>";
        tab2_edit.InnerHtml = "<a href=\"#editfragment-2\" title=\"" + _EditTab2String + "\">" + _EditTab2String + "</a>";
        tab3_edit.InnerHtml = "<a href=\"#editfragment-3\" title=\"" + _EditTab3String + "\">" + _EditTab3String + "</a>";
        tab4_edit.InnerHtml = "<a href=\"#editfragment-4\" title=\"" + _EditTab4String + "\">" + _EditTab4String + "</a>";

        if (!_skipSetOutput && tabNum.Value != "")
        {
            string filterObjectId = string.Empty;
            string filterObject = string.Empty;
            switch (tabNum.Value)
            {
                case "2":
                    filterObjectId = FilterIdTextBox2.Value;
                    filterObject = FilterObjectsList2.SelectedValue; 
                    break;
                case "3":
                    filterObjectId = FilterIdTextBox3.Value;
                    filterObject = FilterObjectsList3.SelectedValue;
                    break;
                case "4":
                    filterObjectId = FilterIdTextBox4.Value;
                    filterObject = FilterObjectsList4.SelectedValue;
                    break;
                case "1":
                default:
                    filterObjectId = FilterIdTextBox1.Value;
                    filterObject = FilterObjectsList1.SelectedValue;
                    break;
            }
            ContentAPI capi = new ContentAPI();
            //if (filterObjectId.Length > 0)
            {
                switch (filterObject)
                {
                    case "folder":
                        //prepare for re-select folder path on the folder tree
                        string folderpath = string.Empty;
                        if (filterObjectId.Length > 0)
                        {
                            Int64 folderid = Convert.ToInt64(filterObjectId);
                            folderpath = filterObjectId;
                            while (folderid != 0)
                            {
                                folderid = capi.GetParentIdByFolderId(folderid);
                                if (folderid >= 0) folderpath += "," + folderid.ToString();
                            }
                        }
                        tbFolderPath.Text = folderpath;
                        folderId.Value = filterObjectId;
                        break;
                    case "taxonomy":
                        //prepare for re-select taxonomy path on the folder tree
                        string taxonomyPath = string.Empty;
                        if (filterObjectId.Length > 0)
                        {
                            Int64 taxId = Convert.ToInt64(filterObjectId);
                            TaxonomyBaseData[] TaxData = capi.EkContentRef.GetTaxonomyRecursiveToParent(taxId, _userLang, 0);
                            for (int i = 0; i < TaxData.Length; i++)
                            {
                                taxId = TaxData[i].TaxonomyId;
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
                        }
                        tbTaxonomyPath.Text = taxonomyPath;
                        TaxId.Value = filterObjectId;
                        break;
                    default:
                        break;
                }
            }
        }
        linkDelete1.Visible = false;
        if (Tab1FilterObjectId != "")
        {
            linkDelete1.Visible = true;
        }
        linkDelete2.Visible = false;
        if (Tab2FilterObjectId != "")
        {
            linkDelete2.Visible = true;
        }
        linkDelete3.Visible = false;
        if (Tab3FilterObjectId != "")
        {
            linkDelete3.Visible = true;
        }
        linkDelete4.Visible = false;
        if (Tab4FilterObjectId != "")
        {
            linkDelete4.Visible = true;
        }
        _skipSetOutput = false;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    protected void linkPopup_Click(object sender, EventArgs e)
    {
        mostpopular_foldertree.Visible = false;
        mostpopular_taxtree.Visible = false;
        ImageButton btn = (ImageButton)sender;
        tabNum.Value = btn.AlternateText.Substring(0, 1);
        DropDownList ddl = null;
        bool recusionFlag = false;
        switch (tabNum.Value)
        {
            case "2":
                ddl = FilterObjectsList2;
                recusionFlag = Convert.ToBoolean(chkFilterObjectRecur2.Value);
                break;
            case "3":
                ddl = FilterObjectsList3;
                recusionFlag = Convert.ToBoolean(chkFilterObjectRecur3.Value);
                break;
            case "4":
                ddl = FilterObjectsList4;
                recusionFlag = Convert.ToBoolean(chkFilterObjectRecur4.Value);
                break;
            case "1":
            default:
                ddl = FilterObjectsList1;
                recusionFlag = Convert.ToBoolean(chkFilterObjectRecur1.Value);
                break;
        }
        if (ddl.SelectedValue != "")
        {
            string webserviceURL = string.Empty;
            string recursionLabel = string.Empty;
            switch (ddl.SelectedValue)
            {
                case "folder":
                    webserviceURL = _sitepath + "widgets/listsummary/LSHandler.ashx";
                    JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
                    JS.RegisterJSInclude(this, _sitepath + "widgets/ListSummary/behavior.js", "ListSummaryWidgetBehaviorJS");
                    JS.RegisterJSBlock(this, "Ektron.PFWidgets.ListSummary.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ListSummary.setupAll(); ", "EktronPFWidgetsLSInit");
                    Css.RegisterCss(this, _sitepath + "widgets/listsummary/LSStyle.css", "LSWidgetCSS");
                    mostpopular_foldertree.Visible = true;
                    recursionLabel = _refMsg.GetMessage("lbl include child folder");
                    break;
                case "taxonomy":
                    webserviceURL = _sitepath + "widgets/contentblock/CBHandler.ashx";
                    JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS); 
                    JS.RegisterJSInclude(this, _sitepath + "widgets/contentblock/behavior.js", "ContentWidgetBehaviorJS");
                    Css.RegisterCss(this, _sitepath + "widgets/contentblock/CBStyle.css", "CBWidgetCSS");
                    JS.RegisterJSBlock(this, "Ektron.PFWidgets.ContentBlock.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ContentBlock.Taxonomy.configTaxonomyTreeView();", "EktronPFWidgetsCBInit");
                    mostpopular_taxtree.Visible = true;
                    recursionLabel = _refMsg.GetMessage("lbl include child subcategory");
                    break;
                default:
                    break;
            }
            chkRecursion.Checked = recusionFlag;
            lblRecursion.Text = recursionLabel;
            ViewSet.SetActiveView(FilterPopup);
        }
        else
        {
            JS.RegisterJSBlock(this, "$ektron(\"a[title='Tab " + tabNum.Value + "']\").click();", this.ClientID + "_TabClickedScriptPopup");
        }
    }
    protected void SavePopupButton_Click(object sender, EventArgs e)
    {
        string objName = string.Empty;
        string objId = string.Empty;
        bool objRecursive = false;
        if (mostpopular_taxtree.Visible && TaxId.Value.Length > 0) //taxnomy selector
        {
            objId = TaxId.Value;
            objName = "taxonomy";
            objRecursive = chkRecursion.Checked;
        }
        else //folder selector
        {
            objId = folderId.Value;
            objName = "folder";
            objRecursive = chkRecursion.Checked;
        }
        if (objId.Length > 0)
        {
            string objTitle = resTitle.Value;
            switch (tabNum.Value)
            {
                case "2":
                    _Tab2FilterObject = objName;
                    FilterObjectsList2.SelectedValue = objName;
                    _Tab2FilterObjectId = objId;
                    FilterIdTextBox2.Value = objId;
                    _Tab2FilterObjectRecursion = objRecursive;
                    chkFilterObjectRecur2.Value = objRecursive.ToString();
                    _Tab2FilterDisplayText = getFilterDisplayValue(FilterObjectsList2.Items.FindByValue(objName).Text, objId, objRecursive, resTitle.Value);
                    filterDisplay2.Text = _Tab2FilterDisplayText;
                    break;
                case "3":
                    _Tab3FilterObject = objName;
                    FilterObjectsList3.SelectedValue = objName;
                    _Tab3FilterObjectId = objId;
                    FilterIdTextBox3.Value = objId;
                    _Tab3FilterObjectRecursion = objRecursive;
                    chkFilterObjectRecur3.Value = objRecursive.ToString();
                    _Tab3FilterDisplayText = getFilterDisplayValue(FilterObjectsList3.Items.FindByValue(objName).Text, objId, objRecursive, resTitle.Value);
                    filterDisplay3.Text = _Tab3FilterDisplayText;
                    break;
                case "4":
                    _Tab4FilterObject = objName;
                    FilterObjectsList4.SelectedValue = objName;
                    _Tab4FilterObjectId = objId;
                    FilterIdTextBox4.Value = objId;
                    _Tab4FilterObjectRecursion = objRecursive;
                    chkFilterObjectRecur4.Value = objRecursive.ToString();
                    _Tab4FilterDisplayText = getFilterDisplayValue(FilterObjectsList4.Items.FindByValue(objName).Text, objId, objRecursive, resTitle.Value);
                    filterDisplay4.Text = _Tab4FilterDisplayText;
                    break;
                case "1":
                default:
                    _Tab1FilterObject = objName;
                    FilterObjectsList1.SelectedValue = objName;
                    _Tab1FilterObjectId = objId;
                    FilterIdTextBox1.Value = objId;
                    _Tab1FilterObjectRecursion = objRecursive;
                    chkFilterObjectRecur1.Value = objRecursive.ToString();
                    _Tab1FilterDisplayText = getFilterDisplayValue(FilterObjectsList1.Items.FindByValue(objName).Text, objId, objRecursive, resTitle.Value);
                    filterDisplay1.Text = _Tab1FilterDisplayText;
                    break;
            }
            JS.RegisterJSBlock(this, "$ektron(\"a[title='Tab " + tabNum.Value + "']\").click();", this.ClientID + "_TabClickedScriptSave");
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
        JS.RegisterJSBlock(this, "$ektron(\"a[title='Tab " + tabNum.Value + "']\").click();", this.ClientID + "_TabClickedScriptSave");
        ViewSet.SetActiveView(Edit);
    }

    protected void linkDelete_Click(object sender, EventArgs e)
    {
        ImageButton btn = (ImageButton)sender;
        tabNum.Value = btn.AlternateText.Substring(0, 1);
        switch (tabNum.Value)
        {
            case "2":
                //FilterObjectsList2.SelectedValue = "";
                FilterIdTextBox2.Value = "";
                chkFilterObjectRecur2.Value = "false";
                filterDisplay2.Text = _NotSet;
                Tab2FilterDisplayText = "";
                Tab2FilterObjectId = "";
                Tab2FilterObject = "";
                break;
            case "3":
                //FilterObjectsList3.SelectedValue = "";
                FilterIdTextBox3.Value = "";
                chkFilterObjectRecur3.Value = "false";
                filterDisplay3.Text = _NotSet;
                Tab3FilterDisplayText = "";
                Tab3FilterObjectId = "";
                Tab3FilterObject = "";
                break;
            case "4":
                //FilterObjectsList4.SelectedValue = "";
                FilterIdTextBox4.Value = "";
                chkFilterObjectRecur4.Value = "false";
                filterDisplay4.Text = _NotSet;
                Tab4FilterDisplayText = "";
                Tab4FilterObjectId = "";
                Tab4FilterObject = "";
                break;
            case "1":
            default:
                //FilterObjectsList1.SelectedValue = "";
                FilterIdTextBox1.Value = "";
                chkFilterObjectRecur1.Value = "false";
                filterDisplay1.Text = _NotSet;
                Tab1FilterDisplayText = "";
                Tab1FilterObjectId = "";
                Tab1FilterObject = "";
                break;
        }
        chkRecursion.Checked = false;
        TaxId.Value = "";
        tbTaxonomyPath.Text = "";
        folderId.Value = "";
        tbFolderPath.Text = "";
        tbData.Text = "";
        JS.RegisterJSBlock(this, "$ektron(\"a[title='Tab " + tabNum.Value + "']\").click();", this.ClientID + "_TabClickedScriptDelete");
    }
    #endregion

    #region private
    private void UpdateMarkup()
    {
        ReportTypeList1.Items.Clear();
        ReportTypeList2.Items.Clear();
        ReportTypeList3.Items.Clear();
        ReportTypeList4.Items.Clear();
        IList<String> names = EventReporter.GetReportNameList();
        foreach (string name in names)
        {
            string localiedName = _refMsg.GetMessage("lbl " + name);
            ReportTypeList1.Items.Add(new ListItem(localiedName, name));
            ReportTypeList2.Items.Add(new ListItem(localiedName, name));
            ReportTypeList3.Items.Add(new ListItem(localiedName, name));
            ReportTypeList4.Items.Add(new ListItem(localiedName, name));
        }
        FilterObjectsList1.Items.Clear();
        FilterObjectsList2.Items.Clear();
        FilterObjectsList3.Items.Clear();
        FilterObjectsList4.Items.Clear();
        //FilterObjectsList1.Items.Add(new ListItem(_None, ""));
        //FilterObjectsList2.Items.Add(new ListItem(_None, ""));
        //FilterObjectsList3.Items.Add(new ListItem(_None, ""));
        //FilterObjectsList4.Items.Add(new ListItem(_None, ""));
        FilterObjectsList1.Items.Add(new ListItem(_Folder, "folder"));
        FilterObjectsList2.Items.Add(new ListItem(_Folder, "folder"));
        FilterObjectsList3.Items.Add(new ListItem(_Folder, "folder"));
        FilterObjectsList4.Items.Add(new ListItem(_Folder, "folder"));
        FilterObjectsList1.Items.Add(new ListItem(_Taxonomy, "taxonomy"));
        FilterObjectsList2.Items.Add(new ListItem(_Taxonomy, "taxonomy"));
        FilterObjectsList3.Items.Add(new ListItem(_Taxonomy, "taxonomy"));
        FilterObjectsList4.Items.Add(new ListItem(_Taxonomy, "taxonomy"));
        FilterIdTextBox1.Value = "";
        FilterIdTextBox2.Value = "";
        FilterIdTextBox3.Value = "";
        FilterIdTextBox4.Value = "";
        string imgDelete = _appPath + "images/UI/Icons/delete.png";
        linkDelete1.ImageUrl = imgDelete;
        linkDelete2.ImageUrl = imgDelete;
        linkDelete3.ImageUrl = imgDelete;
        linkDelete4.ImageUrl = imgDelete;
        string imgView = _appPath + "images/UI/Icons/folderView.png";
        linkPopup1.ImageUrl = imgView;
        linkPopup2.ImageUrl = imgView;
        linkPopup3.ImageUrl = imgView;
        linkPopup4.ImageUrl = imgView;
    }

    private void updateControl()
    {
        List1.StartDate = DateTime.Today.AddDays(-_Tab1Duration);
        List1.ReportName = _Tab1Event;
        List1.PageSize = _PageSize;
        if (_Tab1FilterObject != "" && _Tab1FilterObjectId != "")
        {
            List1.FilterType = _Tab1FilterObject;
            List1.FilterId = Convert.ToInt64(_Tab1FilterObjectId);
            List1.FilterRecursion = _Tab1FilterObjectRecursion;
        }

        if (true == _Tab2Visible)
        {
            List2.StartDate = DateTime.Today.AddDays(-_Tab2Duration);
            List2.ReportName = _Tab2Event;
            List2.PageSize = _PageSize;
            if (_Tab2FilterObject != "" && _Tab2FilterObjectId != "")
            {
                List2.FilterType = _Tab2FilterObject;
                List2.FilterId = Convert.ToInt64(_Tab2FilterObjectId);
                List2.FilterRecursion = _Tab2FilterObjectRecursion;
            }
        }
        else
        {
            tab2.Visible = false;
            List2.Visible = false;
        }

        if (true == _Tab3Visible)
        {
            List3.StartDate = DateTime.Today.AddDays(-_Tab3Duration);
            List3.ReportName = _Tab3Event;
            List3.PageSize = _PageSize;
            if (_Tab3FilterObject != "" && _Tab3FilterObjectId != "")
            {
                List3.FilterType = _Tab3FilterObject;
                List3.FilterId = Convert.ToInt64(_Tab3FilterObjectId);
                List3.FilterRecursion = _Tab3FilterObjectRecursion;
            }
        }
        else
        {
            tab3.Visible = false;
            List3.Visible = false;
        }

        if (true == _Tab4Visible)
        {
            List4.StartDate = DateTime.Today.AddDays(-_Tab4Duration);
            List4.ReportName = _Tab4Event;
            List4.PageSize = _PageSize;
            if (_Tab4FilterObject != "" && _Tab4FilterObjectId != "")
            {
                List4.FilterType = _Tab4FilterObject;
                List4.FilterId = Convert.ToInt64(_Tab4FilterObjectId);
                List4.FilterRecursion = _Tab4FilterObjectRecursion;
            }
        }
        else
        {
            tab4.Visible = false;
            List4.Visible = false;
        }
    }

    private string MakeClientScript()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("$ektron(\"#tabsMostPopular\").tabs();");
        sb.AppendLine("$ektron(\"#tabsMostPopular_edit\").tabs();");
        return sb.ToString();
    }

    private void LocalizeMarkup()
    {
        string sCaption = _refMsg.GetMessage("lbl tab text c");
        lblTab1.Text = sCaption;
        lblTab2.Text = sCaption;
        lblTab3.Text = sCaption;
        lblTab4.Text = sCaption;
        _EditTab1String = _refMsg.GetMessage("lbl most viewed");
        _EditTab2String = _refMsg.GetMessage("lbl most emailed");
        _EditTab3String = _refMsg.GetMessage("lbl most commented");
        _EditTab4String = _refMsg.GetMessage("lbl highest rated");
        string sReportLast = _refMsg.GetMessage("lbl number of days to report");
        lblDuration1.Text = sReportLast;
        lblDuration2.Text = sReportLast;
        lblDuration3.Text = sReportLast;
        lblDuration4.Text = sReportLast;
        string sReport = _refMsg.GetMessage("lbl report c");
        lblEvent1.Text = sReport;
        lblEvent2.Text = sReport;
        lblEvent3.Text = sReport;
        lblEvent4.Text = sReport;
        string sVisible = _refMsg.GetMessage("lbl visible");
        lblCheckVisible2.Text = sVisible;
        lblCheckVisible3.Text = sVisible;
        lblCheckVisible4.Text = sVisible;
        Err_MostPopularWidget.InnerHtml = _refMsg.GetMessage("lbl exceed most popular widget limit");
        string sFilterBy = _refMsg.GetMessage("lbl filter results by");
        lblFilterBy1.Text = sFilterBy;
        lblFilterBy2.Text = sFilterBy;
        lblFilterBy3.Text = sFilterBy;
        lblFilterBy4.Text = sFilterBy;
        _Taxonomy = _refMsg.GetMessage("lbl taxonomy");
        _Folder = _refMsg.GetMessage("lbl folder");
        _None = _refMsg.GetMessage("lbl site wide");
        _NotSet = _refMsg.GetMessage("lbl no filter selected");
        string selFilter = _refMsg.GetMessage("lbl show selector");
        linkPopup1.AlternateText = "1:" + selFilter;
        linkPopup2.AlternateText = "2:" + selFilter;
        linkPopup3.AlternateText = "3:" + selFilter;
        linkPopup4.AlternateText = "4:" + selFilter;
        string selDelete = _refMsg.GetMessage("generic delete title");
        linkDelete1.AlternateText = "1:" + selDelete;
        linkDelete2.AlternateText = "2:" + selDelete;
        linkDelete3.AlternateText = "3:" + selDelete;
        linkDelete4.AlternateText = "4:" + selDelete;
        lblRecursion.Text = _refMsg.GetMessage("lbl include child folder");
        string InvalidNumericMsg = _refMsg.GetMessage("js alert valid numeric");
        RegularExpressionValidator1.ErrorMessage = InvalidNumericMsg;
        RegularExpressionValidator2.ErrorMessage = InvalidNumericMsg;
        RegularExpressionValidator3.ErrorMessage = InvalidNumericMsg;
        RegularExpressionValidator4.ErrorMessage = InvalidNumericMsg;

        CancelButton.Text = _refMsg.GetMessage("btn cancel");
        SaveButton.Text = _refMsg.GetMessage("btn save");
    }

    private string getFilterDisplayValue(string objectType, string objectId, bool bChild, string resTitle)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (objectType != "")
        {
            sb.Append(resTitle + " \xAB");
            sb.Append(objectType + ":" + objectId);
            if (true == bChild)
            {
                sb.Append(" + " + _refMsg.GetMessage("lbl children"));
            }
            sb.Append("\xBB");
        }
        return sb.ToString();
    }
    #endregion
}

