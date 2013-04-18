using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Ektron.Cms.Localization;

public partial class Workarea_Languages : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    private string styleSheetJS = string.Empty;
    private StyleHelper styleHelper = new StyleHelper();
    private Ektron.Cms.SiteAPI siteApi = new Ektron.Cms.SiteAPI();

    private Ektron.Cms.CommonApi commonApi = null;
    private Ektron.Cms.BusinessObjects.Localization.LocaleManager localeMgrBO = null;
    private string appImgPath;

    private int defaultLangId = 1033;
    private string defaultLangLoc = "en-US";

    private string pageAction = string.Empty;
    private bool hasPermissionToView = false;
    private bool hasPermissionToEnable = false;
    private bool hasPermissionToEdit = false;
    private bool hasPermissionToAdd = false;
    private bool viewing = false;
    private bool enabling = false;
    private bool editingGrid = false;
    private bool editingDetail = false;
    private int editIndex = -1;

    private GridView visibleGrid = null;
    private SortDirection sortDirection = SortDirection.Descending;
    private string sortExpression = "LanguageState";

    private int siteEnabledIndex = -1;
    private int enabledIndex = -1;
    private int languageStateIndex = -1;
    private int editDetailIndex = -1;
    private int commandIndex = -1;
    private int flagFileIndex = -1;
    private int nativeNameIndex = -1;

    private string msgNone = "(None)";

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.commonApi = GetCommonApi();
        this.localeMgrBO = new Ektron.Cms.BusinessObjects.Localization.LocaleManager(this.commonApi.RequestInformationRef);
        this.appImgPath = this.commonApi.AppImgPath;

        this.msgNone = GetMessage("none w prths");

        LanguageGrid.RowDataBound += new GridViewRowEventHandler(this.Grid_RowDataBound);
        LanguageGrid.Sorting += new GridViewSortEventHandler(this.Grid_Sorting);
        EditableGrid.RowDataBound += new GridViewRowEventHandler(this.Grid_RowDataBound);
        EditableGrid.Sorting += new GridViewSortEventHandler(this.Grid_Sorting);
        EditableGrid.RowCommand += new GridViewCommandEventHandler(this.EditableGrid_RowCommand);
        EditableGrid.RowEditing += new GridViewEditEventHandler(this.EditableGrid_RowEditing);
        EditableGrid.RowCancelingEdit += new GridViewCancelEditEventHandler(this.EditableGrid_RowCancelingEdit);
        EditableGrid.RowUpdating += new GridViewUpdateEventHandler(this.EditableGrid_RowUpdating);
    }

    private void Page_Load(object sender, EventArgs e)
    {
        Ektron.Cms.ContentAPI refAPI = new Ektron.Cms.ContentAPI();
        Ektron.Cms.PermissionData security_data = refAPI.LoadPermissions(0, "folder", 0);
        this.hasPermissionToView = security_data.IsAdmin;
        this.hasPermissionToEnable = this.hasPermissionToView;
        this.hasPermissionToEdit = this.hasPermissionToEnable;
        this.hasPermissionToAdd = this.hasPermissionToEdit && this.localeMgrBO.AddEnabled();

        if (!this.hasPermissionToView)
        {
            Response.Redirect("../blank.htm", false);
            return;
        }

        ltrStyleSheet.Text = this.styleHelper.GetClientScript();
		Utilities.ValidateUserLogin();
        this.RegisterResources();

        // Determine if EDITING or UPDATING
        this.pageAction = Request.QueryString["action"];
        if (IsPostBack)
        {
            string eventTarget = Request.Form["__EVENTTARGET"];
            string eventArgument = Request.Form["__EVENTARGUMENT"];
            if ("action" == eventTarget)
            {
                this.pageAction = eventArgument;
            }
            else if ("LocaleDetail$Save" == eventTarget)
            {
                this.pageAction = "save_detail";
            }
            else if ("LocaleDetail$Update" == eventTarget)
            {
                this.pageAction = "edit_detail";
            }
            else if (!String.IsNullOrEmpty(eventTarget) && eventTarget.StartsWith("LocaleDetail$"))
            {
                this.pageAction = "edit_detail";
            }
        }

        if (!String.IsNullOrEmpty(this.pageAction))
        {
            this.pageAction = this.pageAction.ToLower();
        }

        object sortDir = Request.QueryString["sortdir"];
        if (sortDir != null)
        {
            try
            {
                this.sortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), (string)sortDir);
            }
            catch 
            { 
                // ignore
            }
        }
        else
        {
            sortDir = ViewState[this.UniqueID + "_SortDirection"];
            if (sortDir != null)
            {
                this.sortDirection = (SortDirection)sortDir;
            }
        }

        string sortExpr = Request.QueryString["sort"];
        if (String.IsNullOrEmpty(sortExpr))
        {
            sortExpr = ViewState[this.UniqueID + "_SortExpression"] as string;
        }

        if (!String.IsNullOrEmpty(sortExpr))
        {
            try
            {
                if (typeof(LocaleData).GetProperty(sortExpr) != null)
                {
                    this.sortExpression = sortExpr;
                }
            }
            catch
            {
                // ignore
            }
        }
    }

    private void EditableGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView gv = (GridView)sender;
        if ("EditDetail" == e.CommandName)
        {
            this.editIndex = -1;
            if (Int32.TryParse((string)e.CommandArgument, out this.editIndex))
            {
                this.pageAction = "edit_detail";
            }
        }
    }

    private void EditableGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = e.NewEditIndex;
    }

    private void EditableGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv = (GridView)sender;
        GridViewRow row = gv.Rows[e.RowIndex];

        // Get the data for this field
        LocaleData data = null;
        for (int i = 0; i < row.Cells.Count; i++)
        {
            DataControlFieldCell cell = row.Cells[i] as DataControlFieldCell;
            if (null == cell)
            {
                continue;
            }

            BoundField field = cell.ContainingField as BoundField;
            if (null == field)
            {
                continue;
            }

            if ("Id" == field.DataField)
            {
                int id = 0;
                if (Int32.TryParse(cell.Text, out id))
                {
                    data = this.LocaleManager.GetItem(id);
                }

                break; // exit for
            }
        }

        if (null == data)
        {
            throw new Exception("Field for 'Id' is missing.");
        }

        // Get the updated values for this field.
        for (int i = 0; i < row.Cells.Count; i++)
        {
            DataControlFieldCell cell = row.Cells[i] as DataControlFieldCell;
            if (null == cell || 0 == cell.Controls.Count)
            {
                continue;
            }

            BoundField field = cell.ContainingField as BoundField;
            if (null == field)
            {
                continue;
            }

            TextBox tb = cell.Controls[0] as TextBox;
            if (null == tb)
            {
                continue;
            }

            switch (field.DataField)
            {
                case "NativeName":
                    data.NativeName = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "EnglishName":
                    data.EnglishName = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "Loc":
                    data.Loc = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "LangCode":
                    data.LangCode = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "XmlLang":
                    data.XmlLang = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "Culture":
                    data.Culture = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "UICulture":
                    data.UICulture = Ektron.Cms.Common.EkFunctions.HtmlEncode(tb.Text);
                    break;
                case "LCID":
                    int lcid = 0;
                    if (Int32.TryParse(tb.Text, out lcid))
                    {
                        data.LCID = lcid;
                    }

                    break;
            }
        }

        DropDownList fallbackLocList = row.FindControl("lstFallbackLoc") as DropDownList;
        if (fallbackLocList != null)
        {
            int fallbackId = 0;
            if (Int32.TryParse(fallbackLocList.SelectedValue, out fallbackId))
            {
                data.FallbackId = fallbackId;
            }
        }

        this.LocaleManager.Update(data);

        gv.EditIndex = -1;
    }

    private void EditableGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = -1;
    }

    private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;
        TableCell cell;
        if (DataControlRowType.DataRow == e.Row.RowType)
        {
            LocaleData data = (LocaleData)e.Row.DataItem;

            if (this.editingGrid)
            {
                // Edit Detail button
                if (this.editDetailIndex >= 0)
                {
                    Image imgEdit = e.Row.Cells[this.editDetailIndex].Controls[0] as Image;
                    if (imgEdit != null)
                    {
                        imgEdit.Attributes.Add("title", imgEdit.AlternateText);
                    }
                }

                // Command buttons
                if (this.commandIndex >= 0)
                {
                    for (int i = 0; i < e.Row.Cells[this.commandIndex].Controls.Count; i++)
                    {
                        Image imgEdit = e.Row.Cells[this.commandIndex].Controls[i] as Image;
                        if (imgEdit != null)
                        {
                            imgEdit.Attributes.Add("title", imgEdit.AlternateText);
                        }
                    }
                }

                // Fallback Loc
                DropDownList lstFallback = e.Row.FindControl("lstFallbackLoc") as DropDownList;
                if (lstFallback != null)
                {
                    List<LocaleData> ds = this.SortLanguages((List<LocaleData>)gv.DataSource, "EnglishName", SortDirection.Ascending);
                    ds.Remove(data); // remove current locale
                    lstFallback.DataTextField = "EnglishName";
                    lstFallback.DataValueField = "Id";
                    lstFallback.Items.Clear();
                    lstFallback.Items.Add(new ListItem(this.msgNone, data.Id.ToString())); // fallback==id means "no fallback"
                    lstFallback.AppendDataBoundItems = true;
                    lstFallback.DataSource = ds;
                    lstFallback.DataBind();
                    lstFallback.SelectedValue = data.FallbackId.ToString();
                }
            }

            if (this.enabling)
            {
                if (this.siteEnabledIndex >= 0 && this.enabledIndex >= 0)
                {
                    CheckBox chkSite = e.Row.Cells[this.siteEnabledIndex].Controls[0] as CheckBox;
                    CheckBox chkActive = e.Row.Cells[this.enabledIndex].Controls[0] as CheckBox;
                    if (chkSite != null && chkActive != null)
                    {
                        bool isFallbackLocale = this.LocaleManager.IsFallbackLocale(data.Id);
                        if (isFallbackLocale)
                        {
                            chkActive.Checked = true;
                            chkActive.CssClass = "fallbackLocale";
                        }

                        if (data.Id == this.defaultLangId)
                        {
                            chkActive.Checked = true;
                            chkSite.Checked = true;
                        }

                        chkSite.ID = "site";
                        chkActive.ID = "active";
                        chkSite.Enabled = (data.Id != this.defaultLangId);
                        chkActive.Enabled = (data.Id != this.defaultLangId && !data.SiteEnabled && !isFallbackLocale);
                        chkSite.InputAttributes.Add("value", data.Id.ToString());
                        chkActive.InputAttributes.Add("value", data.Id.ToString());
                        chkSite.InputAttributes.Add("title", GetMessage("alt Enable on web site"));
                        chkActive.InputAttributes.Add("title", GetMessage("alt Enable in workarea only"));
                        chkSite.InputAttributes.Add("onclick", "onSiteClick(this,'" + chkActive.ClientID + "')");
                    }
                }
            }
            else
            {
                // Language state icon
                if (this.languageStateIndex >= 0)
                {
                    Image langState = e.Row.Cells[this.languageStateIndex].Controls[0] as Image;
                    if (langState != null)
                    {
                        switch (data.LanguageState)
                        {
                            case Ektron.Cms.Localization.LanguageState.Active:
                                langState.ImageUrl = this.appImgPath + "../UI/Icons/caution.png";
                                langState.AlternateText = GetMessage("alt Available in workarea only");
                                langState.Attributes.Add("title", langState.AlternateText);
                                langState.Width = 16;
                                langState.Height = 16;
                                break;
                            case Ektron.Cms.Localization.LanguageState.SiteEnabled:
                                langState.ImageUrl = this.appImgPath + "../UI/Icons/check.png";
                                langState.AlternateText = GetMessage("alt Available on web site");
                                langState.Attributes.Add("title", langState.AlternateText);
                                langState.Width = 16;
                                langState.Height = 16;
                                break;
                            default:
                                langState.ImageUrl = this.appImgPath + "spacer.gif";
                                langState.Width = 16;
                                langState.Height = 16;
                                break;
                        }
                    }
                }
            }

            // Flag Icon
            if (this.flagFileIndex >= 0)
            {
                Image imgFlag = e.Row.Cells[this.flagFileIndex].Controls[0] as Image;
                if (imgFlag != null)
                {
                    imgFlag.Attributes.Add("title", imgFlag.AlternateText);
                }
            }

            if (this.nativeNameIndex >= 0)
            {
                cell = e.Row.Cells[this.nativeNameIndex];
                cell.Attributes.Add("lang", data.XmlLang);

                // bi-di languages
                if (data.IsRightToLeft)
                {
                    cell.Attributes.Add("dir", data.Direction);
                    cell.Style.Add("text-align", "right");
                }
            }
        }
    }

    private void Grid_Sorting(object sender, GridViewSortEventArgs e)
    {
        this.sortDirection = e.SortDirection;
        this.sortExpression = e.SortExpression;
        ViewState[this.UniqueID + "_SortDirection"] = this.sortDirection;
        ViewState[this.UniqueID + "_SortExpression"] = this.sortExpression;
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        bool updateEnable = this.hasPermissionToEnable && ("update" == this.pageAction);
        bool saveDetail = this.hasPermissionToEdit && ("save_detail" == this.pageAction);
        this.enabling = this.hasPermissionToEnable && ("enable" == this.pageAction);
        this.editingGrid = this.hasPermissionToEdit && ("edit_grid" == this.pageAction);
        this.editingDetail = this.hasPermissionToEdit && ("edit_detail" == this.pageAction);
        this.viewing = this.hasPermissionToView && !this.enabling && !this.editingGrid && !this.editingDetail && !saveDetail;

        if (this.enabling)
        {
            this.Title = GetMessage("lbl enable languages");
        }
        else if (this.editingGrid)
        {
            this.Title = GetMessage("lbl edit languages");
        }
        else if (this.editingDetail)
        {
            this.Title = GetMessage("lbl edit language details");
        }
        else if (this.viewing)
        {
            this.Title = GetMessage("lbl view languages");
        }
        else
        {
            this.Title = GetMessage("lbl Language settings");
        }

        // UPDATE language state
        if (updateEnable)
        {
            this.UpdateLanguageState();
        }
        else if (saveDetail)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                this.SaveLocale(LocaleDetail.Locale);
                this.editingGrid = true;
            }
            else
            {
                saveDetail = false;
                this.editingDetail = true;
            }
        }

        LanguageGrid.Visible = !this.editingGrid && !this.editingDetail;
        EditableGrid.Visible = this.editingGrid && !this.editingDetail;
        LocaleDetail.Visible = this.editingDetail;
        if (!this.editingDetail)
        {
            this.visibleGrid = this.editingGrid ? EditableGrid : LanguageGrid;

            // find columns
            for (int i = 0; i < this.visibleGrid.Columns.Count; i++)
            {
                DataControlField field = this.visibleGrid.Columns[i];
                if (field != null)
                {
                    ButtonField buttonField = field as ButtonField;
                    CommandField commandField = field as CommandField;
                    BoundField boundField = field as BoundField;
                    ImageField imageField = field as ImageField;
                    if (buttonField != null)
                    {
                        buttonField.ImageUrl = Ektron.Cms.Common.EkFunctions.QualifyURL(this.appImgPath, "../UI/icons/propertiesEdit.png");
                        if ("EditDetail" == buttonField.CommandName)
                        {
                            this.editDetailIndex = i;
                        }
                    }
                    else if (commandField != null)
                    {
                        commandField.EditImageUrl = Ektron.Cms.Common.EkFunctions.QualifyURL(this.appImgPath, "../UI/icons/pencil.png");
                        commandField.UpdateImageUrl = Ektron.Cms.Common.EkFunctions.QualifyURL(this.appImgPath, "../UI/icons/save.png");
                        commandField.CancelImageUrl = Ektron.Cms.Common.EkFunctions.QualifyURL(this.appImgPath, "../UI/icons/cancel.png");
                        this.commandIndex = i;
                    }
                    else if (imageField != null)
                    {
                        switch (imageField.DataImageUrlField)
                        {
                            case "LanguageState":
                                this.languageStateIndex = i;
                                break;
                            case "FlagFile":
                            case "FlagUrl":
                                this.flagFileIndex = i;
                                break;
                        }
                    }
                    else if (boundField != null)
                    {
                        switch (boundField.DataField)
                        {
                            case "SiteEnabled":
                                this.siteEnabledIndex = i;
                                break;
                            case "Enabled":
                                this.enabledIndex = i;
                                break;
                            case "NativeName":
                                this.nativeNameIndex = i;
                                break;
                        }
                    }
                }
            }

            if (!IsPostBack || updateEnable || saveDetail)
            {
                // show/hide columns
                if (this.siteEnabledIndex >= 0)
                {
                    this.visibleGrid.Columns[this.siteEnabledIndex].Visible = this.enabling;
                }

                if (this.enabledIndex >= 0)
                {
                    this.visibleGrid.Columns[this.enabledIndex].Visible = this.enabling;
                }

                if (this.languageStateIndex >= 0)
                {
                    this.visibleGrid.Columns[this.languageStateIndex].Visible = !this.enabling;
                }
            }

            // headers
            for (int i = 0; i < this.visibleGrid.Columns.Count; i++)
            {
                DataControlField field = this.visibleGrid.Columns[i];
                if (field != null)
                {
                    string reskey = field.HeaderText;
                    if (!String.IsNullOrEmpty(reskey) && reskey.StartsWith("reskey:"))
                    {
                        reskey = reskey.Substring("reskey:".Length);
                        field.HeaderText = GetMessage(reskey);
                    }

                    string url = field.HeaderImageUrl;
                    if (!String.IsNullOrEmpty(url) && !url.StartsWith(this.appImgPath))
                    {
                        field.HeaderImageUrl = this.appImgPath + url;
                    }
                }
            }
        } // !this._editDetail

        // Get list of locales
        Ektron.Cms.Common.Criteria<LocaleProperty> criteria = new Ektron.Cms.Common.Criteria<LocaleProperty>(
                LocaleProperty.EnglishName, 
                Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.PagingInfo.RecordsPerPage = Int32.MaxValue;

        if (this.editingGrid || this.editingDetail)
        {
            criteria.AddFilter(LocaleProperty.Enabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);
        }

        List<LocaleData> locales = this.LocaleManager.GetList(criteria);

        List<LocaleData> systemLocales = null;
        if (!IsPostBack && this.viewing)
        {
            // Compare list of locales in database with list of system cultures
            bool listChanged = false;

            systemLocales = this.localeMgrBO.GetSystemCultures();
            List<LocaleData> listInvalidFallback = systemLocales.FindAll(data => data.FallbackId > 0 && !systemLocales.Exists(d => d.Id == data.FallbackId));
            System.Diagnostics.Debug.Assert(0 == listInvalidFallback.Count, "At least one fallback language is invalid");

            List<Ektron.Cms.Localization.LocaleDataReport> report = Ektron.Cms.Localization.LocaleDataReport.CompareLocaleLists(locales, systemLocales);
            
            // Remove disabled custom locales
            foreach (Ektron.Cms.Localization.LocaleDataReport reportData in report.Where(d => d.Type == Ektron.Cms.Localization.LocaleDataReport.ReportType.NotSystemCulture && !d.Data.Enabled))
            {
                try
                {
                    if (!this.LocaleManager.IsLocaleUsed(reportData.Data.Id))
                    {
                        this.LocaleManager.Delete(reportData.Data.Id);
                        listChanged = true;
                    }
                }
                catch (Exception)
                {
                    // ignore, must be in use
                }
            }

            foreach (Ektron.Cms.Localization.LocaleDataReport reportData in report.Where(d => d.Type == Ektron.Cms.Localization.LocaleDataReport.ReportType.SystemCulture))
            {
                this.LocaleManager.Add(reportData.Data);
                listChanged = true;
            }

            foreach (Ektron.Cms.Localization.LocaleDataReport reportData in report.Where(d => d.Type == Ektron.Cms.Localization.LocaleDataReport.ReportType.Different))
            {
                this.LocaleManager.Update(reportData.Data);
                listChanged = true;
            }

            if (listChanged)
            {
                locales = this.LocaleManager.GetList(criteria);
            }
        }

        this.defaultLangId = this.commonApi.DefaultContentLanguage;
        LocaleData defaultData = this.LocaleManager.FindLocale(locales, this.defaultLangId);
        this.defaultLangLoc = (defaultData != null ? defaultData.Loc : string.Empty);

        if (this.visibleGrid != null)
        {
            this.visibleGrid.DataSource = this.SortLanguages(locales, this.sortExpression, this.sortDirection);
            this.visibleGrid.DataBind();
        }
        else if (this.editingDetail)
        {
            LocaleDetail.Locales = locales;
            if (null == systemLocales)
            {
                systemLocales = this.localeMgrBO.GetSystemCultures();
            }

            LocaleDetail.SystemLocales = systemLocales;
            LocaleDetail.DefaultLocaleId = this.defaultLangId;

            if (this.editIndex >= 0)
            {
                List<LocaleData> sortedLocales = this.SortLanguages(locales, this.sortExpression, this.sortDirection);
                LocaleData data = sortedLocales[this.editIndex];
                LocaleDetail.Locale = data;
            }
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        divTitleBar.InnerHtml = this.styleHelper.GetTitleBar(this.Title);
        divToolBar.InnerHtml = this.LanguageToolBar();
        base.OnPreRender(e);
    }

    private List<LocaleData> SortLanguages(List<LocaleData> locales, string expression, SortDirection direction)
    {
        Func<LocaleData, object> sortBy = (data => data.LanguageState);
        switch (expression)
        {
            case "LanguageState": 
                sortBy = (data => data.LanguageState); 
                direction = SortDirection.Descending; 
                break;
            case "EnglishName": 
                sortBy = (data => data.EnglishName); 
                break;
            case "Loc": 
                sortBy = (data => data.Loc); 
                break;
            case "LangCode": 
                sortBy = (data => data.LangCode); 
                break;
            case "XmlLang": 
                sortBy = (data => data.XmlLang); 
                break;
            case "Culture": 
                sortBy = (data => data.Culture); 
                break;
            case "UICulture": 
                sortBy = (data => data.UICulture); 
                break;
            case "Id": 
                sortBy = (data => data.Id); 
                break;
            case "FallbackId": 
                sortBy = (data => this.GetFallbackLoc(locales, data)); 
                break;
            case "LCID": 
                sortBy = (data => data.LCID); 
                break;
            default: 
                System.Diagnostics.Debug.Assert(false, "Unknown SortExpression: " + expression); 
                break;
        }

        if (direction == SortDirection.Ascending)
        {
            return locales.OrderBy(sortBy).ThenBy(d => d.EnglishName).ToList();
        }
        else
        {
            return locales.OrderByDescending(sortBy).ThenBy(d => d.EnglishName).ToList();
        }
    }

    private string GetFallbackLoc(List<LocaleData> locales, LocaleData data)
    {
        string fallbackLoc = string.Empty;
        if (0 == data.FallbackId)
        {
            if (this.defaultLangId == data.Id)
            {
                fallbackLoc = this.msgNone;
            }
            else
            {
                fallbackLoc = this.defaultLangLoc;
            }
        }
        else if (data.FallbackId == data.Id)
        {
            fallbackLoc = this.msgNone;
        }
        else
        {
            LocaleData fallbackData = this.LocaleManager.FindLocale(locales, data.FallbackId);
            fallbackLoc = (fallbackData != null ? fallbackData.Loc : data.FallbackId.ToString());
        }

        return fallbackLoc;
    }

    protected string GetFallbackLoc(object item) // used in aspx 
    {
        string fallbackLoc = string.Empty;
        LocaleData data = (LocaleData)item;
        List<LocaleData> locales = this.visibleGrid.DataSource as List<LocaleData>;
        fallbackLoc = this.GetFallbackLoc(locales, data);
        return fallbackLoc;
    }

    private void UpdateLanguageState()
    {
        StringBuilder activeLangIDs = new StringBuilder();
        StringBuilder siteLangIDs = new StringBuilder();
        string strLangID = string.Empty;
        int langId = 0;
        foreach (string strName in Request.Form.Keys)
        {
            if (strName.EndsWith("active"))
            {
                strLangID = Request.Form[strName];
                if (null == strLangID)
                {
                    strLangID = string.Empty;
                }

                strLangID = strLangID.Trim();
                if (strLangID.Length > 0 && Int32.TryParse(strLangID, out langId))
                {
                    if (activeLangIDs.Length > 0)
                    {
                        activeLangIDs.Append(",");
                    }

                    activeLangIDs.Append(langId.ToString());
                    this.EnsureFallbackIsEnabled(langId, activeLangIDs);
                }
            }
            else if (strName.EndsWith("site"))
            {
                strLangID = Request.Form[strName];
                if (null == strLangID)
                {
                    strLangID = string.Empty;
                }

                strLangID = strLangID.Trim();
                if (strLangID.Length > 0 && Int32.TryParse(strLangID, out langId))
                {
                    if (siteLangIDs.Length > 0)
                    {
                        siteLangIDs.Append(",");
                    }

                    siteLangIDs.Append(langId.ToString());
                    if (activeLangIDs.Length > 0)
                    {
                        activeLangIDs.Append(",");
                    }

                    activeLangIDs.Append(langId.ToString());
                    this.EnsureFallbackIsEnabled(langId, activeLangIDs);
                }
            }
        }

        // Add the default because it's checkboxes were disabled and therefore not submitted.
        strLangID = this.commonApi.DefaultContentLanguage.ToString();
        if (siteLangIDs.Length > 0)
        {
            siteLangIDs.Append(",");
        }

        siteLangIDs.Append(strLangID);
        if (activeLangIDs.Length > 0)
        {
            activeLangIDs.Append(",");
        }

        activeLangIDs.Append(strLangID); 
        
        // csv of language ids
        this.siteApi.EkContentRef.SaveAllSystemLanguages(activeLangIDs.ToString());

        // csv of language ids
        this.siteApi.UpdateSiteEnabledLanguages(siteLangIDs.ToString());
    }

    private void EnsureFallbackIsEnabled(int id, StringBuilder activeLangIDs)
    {
        LocaleData locale = this.LocaleManager.GetItem(id);
        int fallbackId = locale.FallbackId;
        LocaleData fallbackLocale = this.LocaleManager.GetItem(fallbackId);
        while (fallbackId > 0 && fallbackLocale != null && fallbackLocale.FallbackId != fallbackId && !fallbackLocale.Enabled)
        {
            if (activeLangIDs.Length > 0)
            {
                activeLangIDs.Append(",");
            }

            activeLangIDs.Append(fallbackId.ToString());
            fallbackId = fallbackLocale.FallbackId;
            fallbackLocale = this.LocaleManager.GetItem(fallbackId);
        }
    }

    private void SaveLocale(LocaleData data)
    {
        if (data != null)
        {
            if (data.Id > 0)
            {
                this.LocaleManager.Update(data);
            }
            else
            {
                this.LocaleManager.Add(data);
            }
        }
    }

    private string LanguageToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        string onlineHelpKey = "LanguageAdmin";

        string sortParams = String.Format("&sort={0}&sortdir={1}", Ektron.Cms.Common.EkFunctions.UrlEncode(this.sortExpression), Ektron.Cms.Common.EkFunctions.UrlEncode(this.sortDirection.ToString()));

        result.Append("<table><tbody><tr>");

        if (this.editingDetail)
        {
			result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/back.png", "languages.aspx?action=edit_grid" + sortParams, GetMessage("alt back button text"), GetMessage("btn back"), string.Empty, StyleHelper.BackButtonCssClass, true));
            
            onlineHelpKey = "LanguageAdmin_add";
            if (this.hasPermissionToEdit)
            {
                result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/save.png", "#", GetMessage("alt save changes"), GetMessage("btn save"), "onclick=\"return SubmitForm('frmLanguage','save_detail');\"", StyleHelper.SaveButtonCssClass, true));
            }

			result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/refresh.png", "#", GetMessage("generic refresh"), GetMessage("btn refresh"), "onclick=\"return SubmitForm('frmLanguage','update_detail');\"", StyleHelper.RefreshButtonCssClass, !this.hasPermissionToEdit));
        }
        else if (this.editingGrid)
        {
			result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/back.png", "languages.aspx?action=view" + sortParams, GetMessage("alt back button text"), GetMessage("btn back"), string.Empty, StyleHelper.BackButtonCssClass, true));

            onlineHelpKey = "LanguageAdmin_edit";
            if (this.hasPermissionToAdd)
            {
				result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/add.png", "languages.aspx?action=edit_detail" + sortParams, GetMessage("generic add new language"), GetMessage("btn add"), string.Empty, StyleHelper.AddButtonCssClass, true));
            }
        }
        else if (this.enabling)
        {
			result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/back.png", "languages.aspx?action=view" + sortParams, GetMessage("alt back button text"), GetMessage("btn back"), string.Empty, StyleHelper.BackButtonCssClass, true));

            onlineHelpKey = "LanguageAdmin_enable";
            if (this.hasPermissionToEnable)
            {
                result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/save.png", "#", GetMessage("alt update settings button text"), GetMessage("btn update"), "onclick=\"return SubmitForm('frmLanguage','update');\"", StyleHelper.SaveButtonCssClass, true));
            }
        }
        else
        {
            if (this.hasPermissionToEnable)
            {
				result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/approvalApproveItem.png", "languages.aspx?action=enable" + sortParams, GetMessage("alt enable languages and regions"), GetMessage("lbl enable"), string.Empty, StyleHelper.ApproveButtonCssClass, true));
            }

            if (this.hasPermissionToEdit)
            {
				result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/contentEdit.png", "languages.aspx?action=edit_grid" + sortParams, GetMessage("alt edit settings button text"), GetMessage("btn edit"), string.Empty, StyleHelper.EditButtonCssClass));
                if (this.hasPermissionToAdd)
                {
					result.Append(this.styleHelper.GetButtonEventsWCaption(this.appImgPath + "../UI/Icons/add.png", "languages.aspx?action=edit_detail" + sortParams, GetMessage("generic add new language"), GetMessage("btn add"), string.Empty, StyleHelper.AddButtonCssClass));
                }
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td width=\"95%\" align=\"left\">");
        result.Append(this.styleHelper.GetHelpButton(onlineHelpKey, string.Empty));
        result.Append("</td>");
        result.Append("</tr></tbody></table>");
        return result.ToString();
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.siteApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}
