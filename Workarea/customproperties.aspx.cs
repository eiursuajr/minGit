using System;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Core.CustomProperty;
using Ektron.Cms.Framework.Core;
using Ektron.Cms.Framework.Core.CustomProperty;

public partial class Workarea_customproperties : System.Web.UI.Page
{

    #region Constants

    private const string RelativeIconPath = "/images/UI/icons/";
    private const string QueryStringID = "id";
    private const string QueryStringType = "type";
    private const string QueryStringAction = "action";
    private const string QueryStringTranslate = "Translate";
    private const string QueryStringLangType = "LangType";
    private const string QueryStringOrigLangType = "OrigLangType";
    private const string QueryStringCurrentPage = "Page";
    private const string CookieKeyLastValidLang = "LastValidLanguageID";
    private const string PageActionAdd = "addcustomproperty";
    private const string PageActionEdit = "editcustomproperty";
    private const string PageActionViewAll = "viewall";
    private const string PageActionDelete = "deletecustomprop";
    private const string CustomPropUrlFormatMask = "customproperties.aspx?action=viewall&LangType={0}&OrderBy=";
    private const string ErrorUrlFormatMask = "reterror.aspx?info={0}";
    private const string OptionFormatMask = "<option value=\"{0}\">{1}</option>";
    private const string SelectedOptionFormatMask = "<option value=\"{0}\" selected=\"selected\">{1}</option>";
    private const string ColumnIDSelect = "SELECT";
    private const string ColumnIDTitle = "TITLE";
    private const string ColumnIDId = "ID";
    private const string ColumnIDObjectType = "OBJECTTYPE";
    private const string ColumnIDValue = "VALUE";
    private const string NameValuePairFormatMask = "{0}|{1}";
    private const string ValueDelimiter = ";";
    private const string CmsObjectTypePrefix = "CmsObjectType";
    private const string PagingUrlFormat = "{0}?Action={1}&Type={2}&Page={3}";

    #endregion

    #region Data Members

    private string _pageAction;
    private long _propertyId;
    private string _appImgPath;
    private int _language;
    private string _imageIconPath;
    private int _enableMultiLanguage;
    private string _pageType;
    private bool _translate;
    private int _originalLanguage;
    private int _currentPage;

    private SiteAPI _siteApi;
    private CommonApi _commonApi;
    private CustomProperty _customPropertyApi;
    private CustomPropertyBL _customPropertyCore;
    private StyleHelper _styleHelper;
    private EkMessageHelper _messageHelper;

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public Workarea_customproperties()
    {
        _siteApi = new SiteAPI();
        _commonApi = new CommonApi();
        _customPropertyApi = new CustomProperty();
        _customPropertyCore = new CustomPropertyBL();
        _styleHelper = new StyleHelper();

        _pageAction = string.Empty;
        _propertyId = 0;
        _language = -1;
        _pageType = string.Empty;
        _translate = false;
        _originalLanguage = 0;
        _currentPage = 1;
        _messageHelper = _commonApi.EkMsgRef;
        _enableMultiLanguage = _commonApi.EnableMultilingual;
        _appImgPath = _commonApi.AppImgPath;
        _imageIconPath = _commonApi.AppPath + RelativeIconPath;
        
        _commonApi.ContentLanguage = _language;
    }

    /// <summary>
    /// Render the page in the appropriate mode (add/edit/view).
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializePage();
        RegisterResources();
        
        if (!Utilities.ValidateUserLogin())
            return;
        
        if (!_commonApi.IsARoleMember(EkEnumeration.CmsRoleIds.AdminUsers) && 
            !_commonApi.IsARoleMember(EkEnumeration.CmsRoleIds.TaxonomyAdministrator))
        {
            Response.Redirect("reterror.aspx?info=" + EkFunctions.UrlEncode(_messageHelper.GetMessage("msg login taxonomy administrator")), false);
        }

        try
        {
            if (!Page.IsPostBack)
            {
                switch (_pageAction.ToLower())
                {
                    case PageActionAdd:
                        DisplayAdd();
                        break;
                    case PageActionEdit:
                        DisplayEdit();
                        break;
                    case PageActionViewAll:
                        DisplayViewAll(Convert.ToInt32(_pageType));
                        break;
                    case PageActionDelete:
                        ProcessDelete();
                        break;
                }
            }
            else
            {
                if (_pageAction.ToLower() == PageActionAdd || _pageAction.ToLower() == PageActionEdit)
                {
                    SaveCustomProperty();
                }
                else
                {
                    ProcessDelete();
                }

                Response.Redirect(string.Format(CustomPropUrlFormatMask, _language), false);
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }

        // Render the page's toolbar

        ViewToolBar();
    }

    /// <summary>
    /// Registers resources consumed by this page.
    /// </summary>
    protected void RegisterResources()
    {
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronStyleHelperJS);
        JS.RegisterJS(this, _commonApi.AppPath + "java/metadata_selectlist.js", "EktronMetadataSelectlistJS");
        JS.RegisterJS(this, _commonApi.AppPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
        JS.RegisterJS(this, _commonApi.AppPath + "java/searchfuncsupport.js", "EktronSearchFuncSupportJS");
        JS.RegisterJS(this, _commonApi.AppPath + "java/jfunct.js", "EktronJFunctJS");
    }

    /// <summary>
    /// Initializes page data, extracting query string parameters, rendering labels, etc.
    /// </summary>
    private void InitializePage()
    {
        if (Request.QueryString[QueryStringID] != null)
        {
            _propertyId = Convert.ToInt64(Request.QueryString[QueryStringID]);
        }

        if (Request.QueryString[QueryStringAction] != null)
        {
            _pageAction = Request.QueryString[QueryStringAction];
        }

        if (Request.QueryString[QueryStringType] != null)
        {
            _pageType = Request.QueryString[QueryStringType];
        }
        else
        {
            _pageType = "0";
        }

        if (Request.QueryString[QueryStringTranslate] != null)
        {
            bool.TryParse(Request.QueryString[QueryStringTranslate], out _translate);
        }

        if (Request.QueryString[QueryStringOrigLangType] != null)
        {
            int.TryParse(Request.QueryString[QueryStringOrigLangType], out _originalLanguage);
        }

        if (Request.QueryString[QueryStringLangType] != string.Empty)
        {
            _language = Convert.ToInt32(Request.QueryString[QueryStringLangType]);
            _commonApi.SetCookieValue(CookieKeyLastValidLang, _language.ToString());
        }
        else
        {
            if (_commonApi.GetCookieValue(CookieKeyLastValidLang) != string.Empty)
            {
                _language = Convert.ToInt32(_commonApi.GetCookieValue(CookieKeyLastValidLang));
            }
        }

        if (!string.IsNullOrEmpty(Request.QueryString[QueryStringCurrentPage]))
        {
            int.TryParse(Request.QueryString[QueryStringCurrentPage], out _currentPage);
        }

        if (_language == EkConstants.ALL_CONTENT_LANGUAGES || _language == EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            _language = _commonApi.DefaultContentLanguage;
            _commonApi.SetCookieValue(CookieKeyLastValidLang, _language.ToString());
        }

        // Load content for various labels and literals.
        jsStyleSheet.Text = _styleHelper.GetClientScript();
        jsAlertNumericVal.Text = _messageHelper.GetMessage("js alert valid numeric");
        jsConfirmDelete.Text = _messageHelper.GetMessage("js: delete user prop msg");
        jsSelectCustomProperty.Text = _messageHelper.GetMessage("js: select customprop");
        ltrAppPath.Text = _commonApi.AppPath;
        lblNameLabel.Text = _messageHelper.GetMessage("custom prop name label");
        lblNameLabel.ToolTip = lblNameLabel.Text;
        lblIDLabel.Text = _messageHelper.GetMessage("custom prop id label");
        lblIDLabel.ToolTip = lblIDLabel.Text;
        lblLanguageLabel.Text = _messageHelper.GetMessage("custom prop lang label");
        lblLanguageLabel.ToolTip = lblLanguageLabel.Text;
        lblEditableLabel.Text = _messageHelper.GetMessage("custom prop editable label");
        lblEditableLabel.ToolTip = lblEditableLabel.Text;
        lblEnabledLabel.Text = _messageHelper.GetMessage("custom prop enabled label");
        lblEnabledLabel.ToolTip = lblEnabledLabel.Text;
        lblObjectTypeLabel.Text = _messageHelper.GetMessage("custom prop object type label");
        lblObjectTypeLabel.ToolTip = lblObjectTypeLabel.Text;
        lblDataTypesLabel.Text = _messageHelper.GetMessage("custom prop data types label");
        lblDataTypesLabel.ToolTip = lblDataTypesLabel.Text;
        lblDisplayTypesLabel.Text = _messageHelper.GetMessage("custom prop display types label");
        lblDisplayTypesLabel.ToolTip = lblDisplayTypesLabel.Text;
        lblValueLabel.Text = _messageHelper.GetMessage("custom prop value label");
        lblValueLabel.ToolTip = lblValueLabel.Text;
        ltrAddDateCaption.Text = _messageHelper.GetMessage("custom prop add item");
        ltrAddNumericCaption.Text = _messageHelper.GetMessage("custom prop add item");
        ltrAddTextCaption.Text = _messageHelper.GetMessage("custom prop add item");
        ltrEditCaption.Text = _messageHelper.GetMessage("custom prop edit item");
        ltrRemoveCaption.Text = _messageHelper.GetMessage("custom prop remove item");
    }

    /// <summary>
    /// Prepares the page for display in 'Add' mode (including the
    /// addition of a translated property).
    /// </summary>
    private void DisplayAdd()
    {
        pnlViewAll.Visible = false;
        pnlAddEdit.Visible = true;

        // Populate drop downs with default options
        PopulateDropDownLists();

        if (_translate)
        {
            // If the requested property data does not exist and we're
            // in translation mode, retrieve the original data to
            // be translated.

            CustomPropertyData originalData = _customPropertyApi.GetItem(
                _propertyId,            // Current property ID
                _originalLanguage);     // Language of property to translate

            // If the original data exists and represents a valid
            // property, continue with the translation.

            if (originalData != null && originalData.PropertyId != 0)
            {
                // Duplicate the property data for the new language
                // and add it to the database.

                CustomPropertyData customPropertyData = new CustomPropertyData()
                {
                    PropertyId = originalData.PropertyId,
                    PropertyName = originalData.PropertyName,
                    PropertyDataType = originalData.PropertyDataType,
                    PropertyStyleType = originalData.PropertyStyleType,
                    CmsObjectType = originalData.CmsObjectType,
                    IsEditable = originalData.IsEditable,
                    IsEnabled = originalData.IsEnabled,
                    Items = originalData.Items,
                    LanguageId = _language
                };

                // Load custom property data into the form fields.

                LoadPropertyData(customPropertyData);
            }
        }
        else
        {
            // If we're not translating an existing property,
            // just display an empty form.

            idRow.Visible = false;
            lblLanguage.Text = _siteApi.GetLanguageById(_language).Name;
            lblLanguage.ToolTip = lblLanguage.Text;
            chkEditable.Checked = true;
            chkEnabled.Checked = true;
        }

        ddObjectTypes.Enabled = false;
    }

    /// <summary>
    /// Prepares the page for display in 'Edit' mode.
    /// </summary>
    private void DisplayEdit()
    {
        // Retrieve the specified property data.
        CustomPropertyData propertyData = _customPropertyApi.GetItem(_propertyId, _language);

        if (propertyData != null)
        {
            pnlViewAll.Visible = false;
            pnlAddEdit.Visible = true;

            // Load custom property data into the form fields.

            PopulateDropDownLists();

            ddObjectTypes.Enabled = false;
            ddDataTypes.Enabled = false;
            ddDisplayTypes.Enabled = false;

            // Load the custom property data into the form fields.

            LoadPropertyData(propertyData);
        }
        else
        {
            DisplayError(_messageHelper.GetMessage("custom prop does not exist"));
        }
    }

    /// <summary>
    /// Prepares the page for display in 'ViewAll' mode.
    /// </summary>
    /// <param name="index">Object type index</param>
    protected void DisplayViewAll(int index)
    {
        EkEnumeration.CustomPropertyObjectType displayObjectType = new EkEnumeration.CustomPropertyObjectType();
        List<CustomPropertyData> customPropertyDataList = new List<CustomPropertyData>();

        PagingInfo pageInfo = new PagingInfo();
        pageInfo.CurrentPage = _currentPage;
        pageInfo.RecordsPerPage = _siteApi.RequestInformationRef.PagingSize;

        if (index > 0)
        {
            index = index - 1;
            displayObjectType = GetCmsObjectType(index);
            customPropertyDataList = _customPropertyApi.GetList(displayObjectType, _language, pageInfo);
        }
        else
        {
            customPropertyDataList = _customPropertyApi.GetList(_language,pageInfo);
        }

        pnlViewAll.Visible = true;
        ViewAllGrid.Visible = true;
        pnlAddEdit.Visible = false;

        BoundColumn colBound = new BoundColumn();

        colBound.DataField = ColumnIDSelect;
        colBound.HeaderText = _messageHelper.GetMessage("generic select");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(10);
        colBound.ItemStyle.Width = Unit.Percentage(10);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = ColumnIDTitle;
        colBound.HeaderText = _messageHelper.GetMessage("generic title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(40);
        colBound.ItemStyle.Width = Unit.Percentage(40);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = ColumnIDId;
        colBound.HeaderText = _messageHelper.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(10);
        colBound.ItemStyle.Width = Unit.Percentage(10);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = ColumnIDObjectType;
        colBound.HeaderText = _messageHelper.GetMessage("lbl object type");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = ColumnIDValue;
        colBound.HeaderText = _messageHelper.GetMessage("lbl value");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(30);
        colBound.ItemStyle.Width = Unit.Percentage(30);
        ViewAllGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn(ColumnIDSelect, typeof(String)));
        dt.Columns.Add(new DataColumn(ColumnIDTitle, typeof(String)));
        dt.Columns.Add(new DataColumn(ColumnIDId, typeof(String)));
        dt.Columns.Add(new DataColumn(ColumnIDObjectType, typeof(String)));
        dt.Columns.Add(new DataColumn(ColumnIDValue, typeof(String)));

        if (customPropertyDataList != null)
        {
            for (int i = 0; i < customPropertyDataList.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<input class='delete' type='checkbox' id=\"selectCustProp" + i + "\" name=\"selectCustProp" + i + "\" />";
                dr[0] += "<input type='hidden' id=\"hdnSelectCustProp" + i + "\" name=\"hdnSelectCustProp" + i + "\" value=\"" + customPropertyDataList[i].PropertyId + "\" />";
                dr[1] = "<a href='customproperties.aspx?action=editcustomproperty&id=" + customPropertyDataList[i].PropertyId + "&LangType=" + _language + "'>" + EkFunctions.HtmlEncode(customPropertyDataList[i].PropertyName) + "</a>";
                dr[2] = "<a href='customproperties.aspx?action=editcustomproperty&id=" + customPropertyDataList[i].PropertyId + "&LangType=" + _language + "'>" + customPropertyDataList[i].PropertyId + "</a>";
                dr[3] = customPropertyDataList[i].CmsObjectType;
                dr[4] = GetValueUI(customPropertyDataList[i], i);
                dt.Rows.Add(dr);
            }
        }

        ViewAllGrid.DataSource = new DataView(dt);
        ViewAllGrid.DataBind();

        if (pageInfo.TotalPages > 1)
        {
            pnlPaging.Visible = true;

            // Display current and total page information.
            lblPageText.Text = _siteApi.EkMsgRef.GetMessage("page lbl");
            lblOfText.Text = _siteApi.EkMsgRef.GetMessage("lbl of");
            lblPageValue.Text = pageInfo.CurrentPage.ToString();
            lblTotalPagesValue.Text = pageInfo.TotalPages.ToString();

            // Populate paging links and target URLs
            firstPageLink.InnerText = _siteApi.EkMsgRef.GetMessage("lbl first page");
            firstPageLink.HRef = GetPagingUrl(1);

            previousPageLink.InnerText = _siteApi.EkMsgRef.GetMessage("lbl previous page");
            if (pageInfo.CurrentPage > 1)
            {
                previousPageLink.HRef = GetPagingUrl(pageInfo.CurrentPage - 1);
            }
            else
            {
                previousPageLink.HRef = GetPagingUrl(1);
            }            
            
            nextPageLink.InnerText = _siteApi.EkMsgRef.GetMessage("lbl next page");
            if (pageInfo.CurrentPage < pageInfo.TotalPages)
            {
                nextPageLink.HRef = GetPagingUrl(pageInfo.CurrentPage + 1);
            }
            else
            {
                nextPageLink.HRef = GetPagingUrl(pageInfo.TotalPages);
            }
            
            lastPageLink.InnerText = _siteApi.EkMsgRef.GetMessage("lbl last page");
            lastPageLink.HRef = GetPagingUrl(pageInfo.TotalPages);
        }
        else
        {
            pnlPaging.Visible = false;
        }
    }

    private string GetPagingUrl(int page)
    {
        return string.Format(
            PagingUrlFormat,
            ResolveUrl(Request.AppRelativeCurrentExecutionFilePath),
            _pageAction,
            _pageType,
            page.ToString());
    }

    /// <summary>
    /// Saves custom property data to the database. (Supports both
    /// add and edit modes).
    /// </summary>
    private void SaveCustomProperty()
    {
        // Retrieves custom property input from the form fields.
        CustomPropertyData customPropertyData = GetCustomPropertyDataFromInput();
        
        if (customPropertyData.PropertyId > 0)
        {
            // If the property ID is not zero, we are adding
            // a translated property or updating an existing
            // one.

            if (_translate)
            {
                _customPropertyApi.Add(customPropertyData);
            }
            else
            {
                _customPropertyApi.Update(customPropertyData);
            }
        }
        else
        {
            // Add the new property data to the database.

            _customPropertyApi.Add(customPropertyData);
        }
        
    }

    /// <summary>
    /// Retrieves custom property input data from the form fields.
    /// </summary>
    /// <returns>CustomPropertyData instance populated with form input</returns>
    private CustomPropertyData GetCustomPropertyDataFromInput()
    {
        CustomPropertyData customPropertyData = new CustomPropertyData()
        {
            PropertyName = txtPropertyName.Text,
            IsEditable = chkEditable.Checked,
            IsEnabled = chkEnabled.Checked,
            LanguageId = _language,

            PropertyDataType = (EkEnumeration.CustomPropertyItemDataType)Enum.Parse(
                typeof(EkEnumeration.CustomPropertyItemDataType),
                ddDataTypes.SelectedValue),

            PropertyStyleType = (EkEnumeration.CustomPropertyStyleType)Enum.Parse(
                typeof(EkEnumeration.CustomPropertyStyleType),
                ddDisplayTypes.SelectedValue),

            CmsObjectType = (EkEnumeration.CustomPropertyObjectType)Enum.Parse(
                typeof(EkEnumeration.CustomPropertyObjectType),
                ddObjectTypes.SelectedValue)
        };

        customPropertyData.PropertyId = _translate || _pageAction.ToLower() == PageActionEdit  ? _propertyId : 0;

        foreach (string value in GetPropertyValuesFromString(propertyValues.Value))
        {
            object propertyValue = null;

            if (!string.IsNullOrEmpty(value))
            {
                switch (customPropertyData.PropertyDataType)
                {
                    case EkEnumeration.CustomPropertyItemDataType.Boolean:
                        bool boolValue;
                        bool.TryParse(HttpUtility.UrlDecode(value), out boolValue);
                        propertyValue = boolValue;
                        break;
                    case EkEnumeration.CustomPropertyItemDataType.DateTime:
                        DateTime dateTimeValue;
                        DateTime.TryParse(HttpUtility.UrlDecode(value), out dateTimeValue);
                        propertyValue = dateTimeValue;
                        break;
                    default:
                        propertyValue = HttpUtility.UrlDecode(value);
                        break;
                }
            }

            customPropertyData.AddItem(propertyValue, false);
        }

        return customPropertyData;
    }

    /// <summary>
    /// Populates form fields with the specified custom property data.
    /// </summary>
    /// <param name="propertyData">Data to populate fields with</param>
    private void LoadPropertyData(CustomPropertyData propertyData)
    {
        txtPropertyName.Text = propertyData.PropertyName;
        lblID.Text = propertyData.PropertyId.ToString();
        lblID.ToolTip = lblID.Text;
        lblLanguage.Text = _siteApi.GetLanguageById(propertyData.LanguageId).Name;
        lblLanguage.ToolTip = lblLanguage.Text;
        chkEditable.Checked = propertyData.IsEditable;
        chkEnabled.Checked = propertyData.IsEnabled;
        ddDataTypes.SelectedValue = propertyData.PropertyDataType.ToString();
        ddDisplayTypes.SelectedValue = propertyData.PropertyStyleType.ToString();
        propertyValues.Value = GetPropertyValueString(propertyData);
    }

    /// <summary>
    /// Returns a delimited string of property values (rendered to the page
    /// via javascript).
    /// </summary>
    /// <param name="data">Custom property data from which to extract values</param>
    /// <returns>Delimited string of values</returns>
    private string GetPropertyValueString(CustomPropertyData data)
    {
        StringBuilder propertyValues = new StringBuilder();
        foreach (CustomPropertyItemData itemData in data.Items)
        {
            if (propertyValues.Length > 0)
            {
                propertyValues.Append(ValueDelimiter);
            }

            if (itemData.PropertyValue != null)
            {
                if (data.PropertyDataType == EkEnumeration.CustomPropertyItemDataType.DateTime)
                {
                    DateTime dateTimeValue = (DateTime)itemData.PropertyValue;
                    propertyValues.AppendFormat(
                        NameValuePairFormatMask,
                        Uri.EscapeDataString(dateTimeValue.ToString("F")),
                        Uri.EscapeDataString(itemData.PropertyValue.ToString()));
                }
                else
                {
                    propertyValues.Append(Uri.EscapeDataString(itemData.PropertyValue.ToString()));
                }
            }
        }

        return propertyValues.ToString();
    }

    /// <summary>
    /// Parses a delimited string of values into a more easily consumed collection.
    /// </summary>
    /// <param name="input">Delimted string of values.</param>
    /// <returns>Collection of values</returns>
    private IEnumerable<string> GetPropertyValuesFromString(string input)
    {
        foreach (string value in input.Split(new string[]{ValueDelimiter}, StringSplitOptions.None))
        {
            yield return HttpUtility.UrlDecode(value);
        }
    }

    /// <summary>
    /// Populates the page's drop down lists with the appropriate values.
    /// </summary>
    private void PopulateDropDownLists()
    {
        // CMS object type drop down.
        foreach (EkEnumeration.CustomPropertyObjectType objectType in Enum.GetValues(typeof(EkEnumeration.CustomPropertyObjectType)))
        {
            // For now we only support TaxonomyNode of custom property, so there is no point of binding all the Custom Object Type.
            // Beside the fact that the first of the list item is content, it is selected by default in the drop down, which adds all the custom property of type content.
            ddObjectTypes.Items.Add(new ListItem(objectType.ToString(), objectType.ToString()));
            //ListItem objectTypeListItem = new ListItem();
            //objectTypeListItem.Text = GetCmsObjectTypeName(objectType);
            //objectTypeListItem.Value = objectType.ToString();

            //ddObjectTypes.Items.Add(objectTypeListItem);
        }
        ddObjectTypes.SelectedValue = EkEnumeration.CustomPropertyObjectType.TaxonomyNode.ToString();
        // Data type drop down
        foreach (EkEnumeration.CustomPropertyItemDataType dataType in Enum.GetValues(typeof(EkEnumeration.CustomPropertyItemDataType)))
        {
            ddDataTypes.Items.Add(new ListItem(dataType.ToString(), dataType.ToString()));
        }

        // Display type drop down
        ddDisplayTypes.Items.Add(
            new ListItem(
                GetDisplayTypeDescription(EkEnumeration.CustomPropertyStyleType.SingleSelect),
                EkEnumeration.CustomPropertyStyleType.SingleSelect.ToString()));
        
        ddDisplayTypes.Items.Add(
            new ListItem(
                GetDisplayTypeDescription(EkEnumeration.CustomPropertyStyleType.MultiSelect),
                EkEnumeration.CustomPropertyStyleType.MultiSelect.ToString()));        
    }

    private string GetCmsObjectTypeName(EkEnumeration.CustomPropertyObjectType type)
    {
        return _siteApi.EkMsgRef.GetMessage(CmsObjectTypePrefix + type.ToString());
    }

    /// <summary>
    /// Returns the markup to render the custom property value data to the page (supporting
    /// 'ViewAll' mode).
    /// </summary>
    /// <param name="propertyList">List of properties to display</param>
    /// <param name="count">Index of property to render</param>
    /// <returns>Markup for the custom property's value field</returns>
    private string GetValueUI(CustomPropertyData propertyList, int count ) 
    {
        StringBuilder result = new StringBuilder();
        int itemIndex = 0;

        if (propertyList.Items.Count == 0)
        {
            result.Append("None");
        }
        else
        {
            switch (propertyList.PropertyDataType)
            {
                case EkEnumeration.CustomPropertyItemDataType.String:
                    if (propertyList.Items.Count == 1)
                    {
                        result = new StringBuilder();
                        if (propertyList.Items[0].PropertyValue == null)
                        {
                            result.Append("None");
                        }
                        else
                        {
                            result.Append("<textarea disabled style='height: 50px; width: 150px;' onchange='javascript:updateText(this);' id='txtTextValue' name='txtTextValue'>" + EkFunctions.HtmlEncode(propertyList.Items[itemIndex].PropertyValue.ToString()) + "</textarea>");
                        }
                    }
                    else
                    {
                        result.Append("<select style='width: 150px;' disabled name=\"selCustPropVal" + count + "\" id=\"selCustPropVal" + count + "\">");
                        if (propertyList != null)
                        {
                            for (itemIndex = 0; itemIndex < propertyList.Items.Count; itemIndex++)
                            {
                                if (propertyList.Items[itemIndex].PropertyValue != null)
                                {
                                    result.Append("<option value=\"" + propertyList.Items[itemIndex].PropertyValue + "\">");
                                    result.Append(EkFunctions.HtmlEncode(propertyList.Items[itemIndex].PropertyValue.ToString()));
                                    result.Append("</option>");
                                }
                            }
                        }
                        result.Append("</select>");

                    }
                    break;
                case EkEnumeration.CustomPropertyItemDataType.Boolean:
                    if (propertyList.Items.Count > 0)
                    {
                        if (propertyList.Items[itemIndex].PropertyValue.ToString().ToLower() == "true")
                        {
                            result.Append("<label type='radio' id='radYes' name='radYes' value='Yes' /><span class='label'>" + _messageHelper.GetMessage("lbl coupon yes") + "</span>");
                        }
                        else
                        {
                            result.Append("<label type='radio' id='radYes' name='radYes' value='Yes' /><span class='label'>" + _messageHelper.GetMessage("lbl no") + "</span>");
                        }
                    }
                    break;
                case EkEnumeration.CustomPropertyItemDataType.DateTime:
                    if (propertyList.Items.Count == 1)
                    {
                        if (propertyList.Items[0].PropertyValue != null)
                        {
                            result.Append("<input style='width: 150px;' disabled value='");
                            result.Append(((DateTime)propertyList.Items[0].PropertyValue).ToString("F"));
                            result.Append("'/>");
                        }
                        else
                        {
                            result.Append("None");
                        }
                    }
                    else
                    {
                        result.Append("<select style='width: 150px;' disabled name=\"selCustPropVal" + count + "\" id=\"selCustPropVal" + count + "\">");

                        for (itemIndex = 0; itemIndex < propertyList.Items.Count; itemIndex++)
                        {
                            DateTime dateTimeValue;

                            result.Append("<option value=\"" + propertyList.Items[itemIndex].PropertyValue + "\">");

                            if (DateTime.TryParse(propertyList.Items[itemIndex].PropertyValue.ToString(), out dateTimeValue))
                            {
                                // Display long format date/time value for consistency with
                                // date/time selector.

                                result.Append(dateTimeValue.ToString("F"));
                            }
                            else
                            {
                                result.Append(propertyList.Items[itemIndex].PropertyValue);
                            }

                            result.Append("</option>");
                        }

                        result.Append("</select>");
                    }
                    break;
                default:
                    if (propertyList.Items.Count == 1)
                    {
                        if (propertyList.Items[0].PropertyValue != null)
                        {
                            result.Append("<input style='width: 150px;' disabled value='");
                            result.Append(propertyList.Items[0].PropertyValue.ToString());
                            result.Append("'/>");
                        }
                        else
                        {
                            result.Append("None");
                        }
                    }
                    else
                    {
                        result.Append("<select style='width: 150px;' disabled name=\"selCustPropVal" + count + "\" id=\"selCustPropVal" + count + "\">");
                        if (propertyList != null)
                        {
                            for (itemIndex = 0; itemIndex < propertyList.Items.Count; itemIndex++)
                            {
                                result.Append("<option value=\"" + propertyList.Items[itemIndex].PropertyValue + "\">");
                                result.Append(propertyList.Items[itemIndex].PropertyValue);
                                result.Append("</option>");
                            }
                        }
                        result.Append("</select>");
                    }

                    break;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Processes a delete action, removing the selected custom properties from
    /// the database.
    /// </summary>
    protected void ProcessDelete()
    {
        string deleteCustomProperties = deleteSelectedIds.Value;
        string[] delimiters = new string[] { ValueDelimiter };

        if (deleteCustomProperties.Contains(ValueDelimiter))
        {
            if (deleteCustomProperties.IndexOf(ValueDelimiter) == 0)
            {
                deleteCustomProperties.Replace(ValueDelimiter, string.Empty);
            }
            else
            {
                deleteCustomProperties.Remove(deleteCustomProperties.Length - 1);
            }

            string[] deleteIdsCustProp = deleteCustomProperties.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < deleteIdsCustProp.Length; i++)
            {
                if (deleteIdsCustProp[i] != string.Empty)
                {
                    _customPropertyApi.Delete(Convert.ToInt64(deleteIdsCustProp[i]),_language);
                }
            }
        }
        else
        {
            _customPropertyApi.Delete(_propertyId,_language);
        }

        Response.Redirect(string.Format(CustomPropUrlFormatMask, _language), false);
    }

    /// <summary>
    /// Renders the page's toolbar.
    /// </summary>
    private void ViewToolBar()
    {
        StringBuilder result = new StringBuilder();
        result.Append("<table><tr>");
        string myAlias = string.Empty;

        switch (_pageAction.ToLower())
        {
            case PageActionAdd:
                txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("taxonomy custom prop add"));
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "back.png", "customproperties.aspx?action=viewall&LangType=" + _language + "&OrderBy=" + Request.QueryString["OrderBy"], _messageHelper.GetMessage("alt back button text"), _messageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "save.png", "#", _messageHelper.GetMessage("alt save button text (taxonomy property)"), _messageHelper.GetMessage("btn save"), "Onclick='javascript:$ektron(\"input#isPostData\").attr(\"value\", \"true\"); return SubmitForm();'", StyleHelper.SaveButtonCssClass, true));
                myAlias = "AddCustomProperty";
                break;
            case PageActionEdit:
                if (_commonApi.DefaultContentLanguage == _language)
                {
                    txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("taxonomy custom props edit"));
                }
                else
                {
                    txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("taxonomy custom props trans"));
                }
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "back.png", "customproperties.aspx?action=viewall&LangType=" + _language + "&OrderBy=" + Request.QueryString["OrderBy"], _messageHelper.GetMessage("alt back button text"), _messageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "save.png", "#", _messageHelper.GetMessage("alt save button text (taxonomy property)"), _messageHelper.GetMessage("btn save"), "Onclick='javascript:$ektron(\"input#isPostData\").attr(\"value\", \"true\");return SubmitForm();'", StyleHelper.SaveButtonCssClass, true));
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "delete.png", "customproperties.aspx?action=DeleteCustomProp&id=" + _propertyId + "&LangType=" + _language, _messageHelper.GetMessage("alt delete button text"), _messageHelper.GetMessage("btn delete"), "onclick='if(!confirmDelete()){return false;}'", StyleHelper.DeleteButtonCssClass));
                
                // Add view / add language dropdowns.

                if (_enableMultiLanguage == 1)
                {
                    result.Append("<td align='right'>&nbsp;&nbsp;|&nbsp;&nbsp;" + _messageHelper.GetMessage("view language") + "&nbsp;" + GetLanguageDropDown("ddViewLanguage", GetAvailableTranslations(_propertyId), "javascript:EditLanguage(" + _propertyId + ", this.value);", _language, false) + "</td>");
                    result.Append("<td align='right'>&nbsp;&nbsp;" + _messageHelper.GetMessage("add language") + "&nbsp;" + GetLanguageDropDown("ddAddLanguage", GetAvailableLanguages(_propertyId), "javascript:Translate(" + _propertyId + ", " + _language + ", this.value);", _language, true) + "</td>");
                }
                myAlias = "AddCustomProperty";
                break;

            case PageActionViewAll:
                txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("taxonomy custom props view"));
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "add.png", "customproperties.aspx?action=addcustomproperty&LangType=" + _language, _messageHelper.GetMessage("alt add button text (taxonomy property)"), _messageHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
                result.Append(_styleHelper.GetButtonEventsWCaption(_imageIconPath + "delete.png", "customproperties.aspx?action=deletecustomprop", _messageHelper.GetMessage("alt delete button text"), _messageHelper.GetMessage("btn delete"), "Onclick='javascript:return GetDeleteIds();'", StyleHelper.DeleteButtonCssClass));
                if (_enableMultiLanguage == 1)
                {
					result.Append(StyleHelper.ActionBarDivider);
                    result.Append("<td align='right'>" + _messageHelper.GetMessage("view in label") + ":&nbsp;" + _styleHelper.ShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", _language.ToString()) + "</td>");
                }
                else
                {
                    result.Append("<td>&nbsp;</td>");
                }
				result.Append(StyleHelper.ActionBarDivider);
                result.Append("<td>" + _messageHelper.GetMessage("lbl object type") + ":&nbsp;" + GetObjectTypeDropDown("SetObjectType(this); return false;"));
                result.Append("</td>");
                myAlias = "viewcustomproperties";
                break;
        }

        result.Append("<td>");
        result.Append(_styleHelper.GetHelpButton(myAlias, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    /// <summary>
    /// Returns the CustomPropertyObjectType indicated by the specified index.
    /// </summary>
    /// <param name="SelectedIndex">Object type index</param>
    /// <returns>Object type value</returns>
    protected EkEnumeration.CustomPropertyObjectType GetCmsObjectType(int SelectedIndex)
    {
        //switch (SelectedIndex)
        //{
        //    case 0:
        //        return EkEnumeration.CustomPropertyObjectType.Content;
        //        break;
        //    case 1:
        //        return EkEnumeration.CustomPropertyObjectType.Folder;
        //        break;
        //    case 2:
        //        return EkEnumeration.CustomPropertyObjectType.User;
        //        break;
        //    case 3:
        //        return EkEnumeration.CustomPropertyObjectType.UserGroup;
        //        break;
        //    case 4:
        //        return EkEnumeration.CustomPropertyObjectType.Library;
        //        break;
        //    case 5:
        //        return EkEnumeration.CustomPropertyObjectType.Collection;
        //        break;
        //    case 6:
        //        return EkEnumeration.CustomPropertyObjectType.Menu;
        //        break;
        //    case 7:
        //        return EkEnumeration.CustomPropertyObjectType.Calendar;
        //        break;
        //    case 8:
        //        return EkEnumeration.CustomPropertyObjectType.Subscription;
        //        break;
        //    case 9:
        //        return EkEnumeration.CustomPropertyObjectType.Notification;
        //        break;
        //    case 10:
        //        return EkEnumeration.CustomPropertyObjectType.Blog;
        //        break;
        //    case 11:
        //        return EkEnumeration.CustomPropertyObjectType.BlogPost;
        //        break;
        //    case 12:
        //        return EkEnumeration.CustomPropertyObjectType.BlogComment;
        //        break;
        //    case 13:
        //        return EkEnumeration.CustomPropertyObjectType.DiscussionBoard;
        //        break;
        //    case 14:
        //        return EkEnumeration.CustomPropertyObjectType.DiscussionForum;
        //        break;
        //    case 15:
        //        return EkEnumeration.CustomPropertyObjectType.RestrictIP;
        //        break;
        //    case 16:
        //        return EkEnumeration.CustomPropertyObjectType.ReplaceWord;
        //        break;
        //    case 17:
        //        return EkEnumeration.CustomPropertyObjectType.UserRank;
        //        break;
        //    case 18:
        //        return EkEnumeration.CustomPropertyObjectType.DiscussionTopic;
        //        break;
        //    case 19:
        //        return EkEnumeration.CustomPropertyObjectType.CommunityGroup;
        //        break;
        //    case 20:
        //        return EkEnumeration.CustomPropertyObjectType.TaxonomyNode;
        //        break;
        //    case 21:
        //        return EkEnumeration.CustomPropertyObjectType.CatalogEntry;
        //        break;
        //    case 22:
        //        return EkEnumeration.CustomPropertyObjectType.MessageBoard;
        //        break;
        //    case 23:
        //        return EkEnumeration.CustomPropertyObjectType.CalendarEvent;
        //        break;
        //    case 24:
        //        return EkEnumeration.CustomPropertyObjectType.MicroMessage;
        //        break;
        //    default:
                return EkEnumeration.CustomPropertyObjectType.TaxonomyNode;
        //}
    }

    /// <summary>
    /// Renders the object type drop down displayed in the toolbar
    /// </summary>
    /// <param name="OnChangeEvt">OnChange event code</param>
    /// <returns>Markup for object type drop down</returns>
    public String GetObjectTypeDropDown(string OnChangeEvt)
    {
        StringBuilder result = new StringBuilder();
        
        try
        {
            
            string displayUserOption = "1";
            if (!string.IsNullOrEmpty(Request.QueryString["objtype"]))
            { 
                displayUserOption = Request.QueryString["objtype"];
            }
            result.Append("<select name='objectType' id='objectType' onchange=\"" + OnChangeEvt + "\">");
                        
            // If user is not part of Administrator group but has taxonomy administrator role, do not display User option.

            if (displayUserOption.ToString() != "0")
            {
                result.Append("<option value='0'>");
                result.Append("User");
                result.Append("</option>");
            }

            // Right now there is only one CustomPropertyObjectType that is TaxonomyNode,
            // Need to implement objectType in the future as new ones are added.

            result.Append(" <option value='1' selected='selected'>");
            result.Append(GetCmsObjectTypeName(EkEnumeration.CustomPropertyObjectType.TaxonomyNode));
            result.Append("</option>");
            result.Append("</select>");
        }
        catch (Exception ex)
        {
            EkException.WriteToEventLog(
                "CMS400: " + ex.Message + ":" + ex.StackTrace, 
                EventLogEntryType.Error);

            result.Length = 0;
        }

        return (result.ToString());
    }

    /// <summary>
    /// Returns the appropriate message for the specified CustomPropertyStyleType.
    /// </summary>
    /// <param name="type">Custom property style type</param>
    /// <returns>Message for the specified CustomPropertyStyleType</returns>
    public string GetDisplayTypeDescription(EkEnumeration.CustomPropertyStyleType type)
    {
        string message = string.Empty;

        switch (type)
        {
            case EkEnumeration.CustomPropertyStyleType.SingleSelect:
                message = _messageHelper.GetMessage("custom prop single value");
                break;
            case EkEnumeration.CustomPropertyStyleType.MultiSelect:
                message = _messageHelper.GetMessage("custom prop list value");
                break;
            default:
                throw new InvalidOperationException("No message available for the specified type: " + type.ToString());
        }

        return message;
    }

    /// <summary>
    /// Returns markup for language selection drop downs that appear in the
    /// page's title bar.
    /// </summary>
    /// <param name="dropDownId">ID associated with the drop down</param>
    /// <param name="languages">Collection of languages to appear as options</param>
    /// <param name="onChangeEvent">Contents of the onchange event handler</param>
    /// <param name="selectedValue">Selected value in the drop down</param>
    /// <param name="includeDefault">Include a default entry in the list ("Select Language")</param>
    /// <returns>Markup for language selection drop downs</returns>
    public String GetLanguageDropDown(string dropDownId, List<LanguageData> languages, string onChangeEvent, int selectedValue, bool includeDefault)
    {
        StringBuilder response = new StringBuilder();

        response.AppendFormat(
            "<select id=\"{0}\" name=\"{0}\" onchange=\"{1}\">",
            dropDownId,
            onChangeEvent);

        if (includeDefault)
        {
            response.AppendFormat(OptionFormatMask, 0, _messageHelper.GetMessage("select language"));
        }

        if (languages != null)
        {
            foreach (LanguageData language in languages)
            {
                if (language.Id == selectedValue)
                {
                    response.AppendFormat(
                        SelectedOptionFormatMask,
                        language.Id,
                        !string.IsNullOrEmpty(language.LocalName) ? language.LocalName : language.Name);
                }
                else
                {
                    response.AppendFormat(
                        OptionFormatMask,
                        language.Id,
                        !string.IsNullOrEmpty(language.LocalName) ? language.LocalName : language.Name);
                }
            }
        }

        response.AppendFormat("</select>");

        return response.ToString();
    }

    /// <summary>
    /// Redirects to the default error page and displays the
    /// specified message.
    /// </summary>
    /// <param name="message">Message to be displayed</param>
    private void DisplayError(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Response.Redirect(String.Format(ErrorUrlFormatMask, HttpUtility.UrlPathEncode(message)));
        }
    }

    /// <summary>
    /// Returns a collection of active languages to which the
    /// specified property has not yet been translated.
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <returns>Collection of available languages</returns>
    private List<LanguageData> GetAvailableTranslations(long propertyId)
    {
        List<LanguageData> translatedLanguages = new List<LanguageData>();

        try
        {
            translatedLanguages.AddRange(_customPropertyApi.GetListTranslated(propertyId));
        }
        catch(Exception ex)
        {
            EkException.WriteToEventLog(
                "CMS400: " + ex.Message + ":" + ex.StackTrace,
                EventLogEntryType.Error);
        }

        return translatedLanguages;
    }

    /// <summary>
    /// Returns a collection of active languages to which the
    /// specified property has not yet been translated.
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <returns>Collection of available languages</returns>
    private List<LanguageData> GetAvailableLanguages(long propertyId)
    {
        List<LanguageData> availableLanguages = new List<LanguageData>();

        try 
        {
            availableLanguages.AddRange(_customPropertyApi.GetListNonTranslated(propertyId));
        }
        catch(Exception ex)
        {
            EkException.WriteToEventLog(
                "CMS400: " + ex.Message + ":" + ex.StackTrace, 
                EventLogEntryType.Error);
        }

        return availableLanguages;
    }
}
