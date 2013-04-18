using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Ektron.Cms.Localization;

public partial class localization_uc : WorkareaBaseControl
{

	#region " Web Form Designer Generated Code "

	//This call is required by the Web Form Designer.
	[System.Diagnostics.DebuggerStepThrough()]

	private void InitializeComponent()
	{
	}

	private void Page_Init(System.Object sender, System.EventArgs e)
	{
		//CODEGEN: This method call is required by the Web Form Designer
		//Do not modify it using the code editor.
		InitializeComponent();

		_locApi = new Ektron.Cms.Framework.Localization.LocaleManager();

	}

	private Ektron.Cms.Framework.Localization.LocaleManager _locApi = null;

	protected Ektron.Cms.ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
	protected StyleHelper m_refStyle = new StyleHelper();
	protected Ektron.Cms.SiteAPI m_refSiteApi = new Ektron.Cms.SiteAPI();
	protected Ektron.Cms.LocalizationAPI m_objLocalizationApi = new Ektron.Cms.LocalizationAPI();
    protected Ektron.Cms.BusinessObjects.Localization.L10nManager localizationManager = null;
	protected long m_intId = 0;
	protected Ektron.Cms.FolderData folder_data;
	protected string AppImgPath = "";
	protected string AppPath = "";
	protected int ContentType = 1;
	protected long CurrentUserId = 0;
	protected Collection pagedata;
	protected string m_strPageAction = "";
	protected string m_strOrderBy = "";
	protected int ContentLanguage = -1;
	protected int EnableMultilingual = 0;
	protected string SitePath = "";
	protected Ektron.Cms.ContentData content_data;
	protected Ektron.Cms.ContentStateData content_state_data;
	protected long m_intFolderId = -1;
	protected string CallerPage = "";
	protected bool TaskExists = false;
	private Ektron.Cms.Content.EkContent m_refContent;
	//protected string LanguageName = "";

	//protected Ektron.Cms.LanguageData language_data;
	private enum CmsTranslatableType
	{
		Content,
		Folder,
		Menu,
		Taxonomy,
        Product
	}

	private CmsTranslatableType m_Type = CmsTranslatableType.Folder;

	private bool m_bRecursive = true;
	#endregion

	public localization_uc()
	{
		Load += Page_Load;
		Init += Page_Init;
        localizationManager = new Ektron.Cms.BusinessObjects.Localization.L10nManager(this.m_refContentApi.RequestInformationRef);
	}

	private void Page_Load(System.Object sender, System.EventArgs e)
	{
		//Put user code to initialize the page here
		this.CreateChildControls();

		RegisterResources();
	}

	public bool Display()
	{
		m_intId = 0;
		if (((Request.QueryString["id"] != null)))
		{
			m_intId = Convert.ToInt64(Request.QueryString["id"]);
		}
		if ((IsPostBack))
		{
			m_strPageAction = Request.Form["action"].Trim().ToLower();
		}
		else if (((Request.QueryString["action"] != null)))
		{
			m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
		}
		if ((IsPostBack && (ddlSourceLanguage != null) && ddlSourceLanguage.Items.Count > 0))
		{
			// SourceLanguageList.SelectedValue doesn't work, not sure why, may be viewstate
			// or b/c its a user control that's losing state
			string strSrcLang = Request.Form[ddlSourceLanguage.UniqueID].Trim();
			if ((Information.IsNumeric(strSrcLang)))
			{
				ContentLanguage = Convert.ToInt32(strSrcLang);
			}
		}
		else if (((Request.QueryString["LangType"] != null)))
		{
			if ((!string.IsNullOrEmpty(Request.QueryString["LangType"])))
			{
				ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				this.CommonApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
			}
			else
			{
				if (!string.IsNullOrEmpty(this.CommonApi.GetCookieValue("LastValidLanguageID")))
				{
					ContentLanguage = Convert.ToInt32(this.CommonApi.GetCookieValue("LastValidLanguageID"));
				}
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(this.CommonApi.GetCookieValue("LastValidLanguageID")))
			{
				ContentLanguage = Convert.ToInt32(this.CommonApi.GetCookieValue("LastValidLanguageID"));
			}
		}
		if ((Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED == ContentLanguage | Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES == ContentLanguage))
		{
			ContentLanguage = this.CommonApi.DefaultContentLanguage;
		}
		this.CommonApi.ContentLanguage = ContentLanguage;
		m_objLocalizationApi.ContentLanguage = ContentLanguage;

		//language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
		//LanguageName = language_data.Name;
		m_refContent = this.CommonApi.EkContentRef;

		CurrentUserId = this.CommonApi.UserId;
		AppImgPath = this.CommonApi.AppImgPath;
		AppPath = this.CommonApi.AppPath;
		SitePath = this.CommonApi.SitePath;
		EnableMultilingual = this.CommonApi.EnableMultilingual;
		if ((!((Request.QueryString["callerpage"] == null))))
		{
			CallerPage = Request.QueryString["callerpage"];
		}

		if ((string.IsNullOrEmpty(CallerPage)))
		{
			if ((!((Request.QueryString["calledfrom"] == null))))
			{
				CallerPage = Request.QueryString["calledfrom"];
			}
		}
		m_intFolderId = -1;
		if ((!((Request.QueryString["folder_id"] == null))))
		{
			if ((!string.IsNullOrEmpty(Request.QueryString["folder_id"])))
			{
				m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
			}
		}
		string strType = Request.QueryString["type"];
		if (((strType == null)))
		{
			strType = "";
		}
		strType = strType.Trim().ToLower();
		if (("menu" == strType))
		{
			m_Type = CmsTranslatableType.Menu;
		}
		else if (("taxonomy" == strType))
		{
			m_Type = CmsTranslatableType.Taxonomy;
		}
		else if ((-1 == m_intFolderId))
		{
			m_intFolderId = m_intId;
			m_intId = 0;
			m_Type = CmsTranslatableType.Folder;
		}
		else if ((m_intId > 0))
		{
            if (this.m_refContent.GetContentType(m_intId) == 3333)
                m_Type = CmsTranslatableType.Product;
            else
                m_Type = CmsTranslatableType.Content;
		}
		else
		{
			m_Type = CmsTranslatableType.Folder;
		}

		if ((IsPostBack))
		{
			if (((chkRecursive != null)))
			{
				// chkRecursive.Checked doesn't work, not sure why, may be viewstate
				// or b/c its a user control that's losing state
				m_bRecursive = ((Request.Form[chkRecursive.UniqueID] != null));
				chkRecursive.Checked = m_bRecursive;
			}
			else
			{
				m_bRecursive = true;
			}
		}
		else
		{
			string strRecursive = Request.QueryString["recursive"];
			if (((strRecursive == null)))
			{
				m_bRecursive = true;
			}
			else
			{
				strRecursive = strRecursive.Trim().ToLower();
				m_bRecursive = ("true" == strRecursive || "1" == strRecursive || "yes" == strRecursive);
			}
			chkRecursive.Checked = m_bRecursive;
		}

		if ((m_intFolderId != -1))
		{
			folder_data = m_refContentApi.GetFolderById(m_intFolderId);
			if ((folder_data == null))
			{
				Response.Redirect("notify_user.aspx", false);
				return false;
			}
		}
		else
		{
			folder_data = null;
		}

		if (CmsTranslatableType.Content == m_Type ||
            CmsTranslatableType.Product == m_Type)
		{
			content_data = m_refContentApi.GetContentById(m_intId, Ektron.Cms.ContentAPI.ContentResultType.Published);
			if ((content_data == null))
			{
				Response.Redirect("notify_user.aspx", false);
				return false;
			}
		}
		else
		{
			content_data = null;
		}

		Display_Localization();

		switch (m_strPageAction)
		{
			case "localizeexport":
				Display_Select(false);
				ExportForTranslation();
				break;
			default:
				// "localize"
				m_strPageAction = "localize";
				Display_Select(true);
				break;
		}
		return true;
	}

	#region "LOCALIZATION - Select"
	private void Display_Localization()
	{
		HoldMomentMsg.Text = GetMessage("one moment msg");

		jsFolderId.Text = m_intFolderId.ToString();
		jsBackStr.Text = "back_file=content.aspx";
		if ((m_strPageAction.Length > 0))
		{
			jsBackStr.Text += "&back_action=" + m_strPageAction;
		}
		if ((Convert.ToString(m_intFolderId).Length > 0))
		{
			jsBackStr.Text += "&back_folder_id=" + m_intFolderId;
		}
		if ((Convert.ToString(m_intId).Length > 0))
		{
			jsBackStr.Text += "&back_id=" + m_intId;
		}
		if ((Convert.ToString(ContentLanguage).Length > 0))
		{
			jsBackStr.Text += "&back_LangType=" + ContentLanguage;
		}
		jsToolId.Text = m_intId.ToString();
		jsToolAction.Text = m_strPageAction;
		jsDefaultLanguage.Text = this.CommonApi.DefaultContentLanguage.ToString();
		jsSourceLanguageListID.Text = ddlSourceLanguage.UniqueID;

		GenerateToolbar();
	}

	private void Display_Select(bool Visible)
	{
		ddlSourceLanguage.Items.Clear();
		pnlForm.Visible = Visible;
		if ((Visible))
		{
			if ((CmsTranslatableType.Folder == m_Type))
			{
				chkRecursive.Visible = true;
			}
			else
			{
				chkRecursive.Visible = false;
			}
			Ektron.Cms.LanguageData[] aryLangs = null;
			switch (m_Type)
			{
                case CmsTranslatableType.Product:
				case CmsTranslatableType.Content:
					aryLangs = m_objLocalizationApi.DisplayAddViewLanguage(m_intId);
					break;
				case CmsTranslatableType.Menu:
					aryLangs = m_objLocalizationApi.DisplayAddViewLanguageForMenus();
					MenuWarning.InnerHtml = "On import content titles are empty when content is not translated in selected languages.";
					MenuWarning.Visible = true;
					break;
				case CmsTranslatableType.Folder:
					// This isn't based on the folder (it would take too long), but is
					// at least a decent estimate.
					aryLangs = m_objLocalizationApi.DisplayAddViewLanguageForAllContent();
					break;
				default:
					aryLangs = m_refSiteApi.GetAllActiveLanguages();
					break;
			}

			if (((aryLangs != null) && aryLangs.Length > 0))
			{
				for (int iLang = 0; iLang <= aryLangs.Length - 1; iLang++)
				{
					{
						if (("VIEW" == aryLangs[iLang].Type))
						{
							ddlSourceLanguage.Items.Add(new ListItem(aryLangs[iLang].LocalName, aryLangs[iLang].Id.ToString()));
						}
					}
				}
				ddlSourceLanguage.SelectedValue = ContentLanguage.ToString();
			}

			GenerateTargetLanguageList(ContentLanguage);
		}
		else
		{
			chkRecursive.Visible = false;
		}
	}

	private void GenerateToolbar()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();

		string WorkareaTitlebarTitle = null;
		if (("localizeexport" == m_strPageAction))
		{
			WorkareaTitlebarTitle = GetMessage("lbl Download Files");
		}
		else
		{
			switch (m_Type)
			{
                case CmsTranslatableType.Product:
                case CmsTranslatableType.Content:
					WorkareaTitlebarTitle = string.Format(GetMessage("alt Export for Translation Content") + "\"{0}\"", content_data.Title);
					break;
				case CmsTranslatableType.Folder:
					WorkareaTitlebarTitle = string.Format(GetMessage("alt Export for Translation Folder") + "\"{0}\"", folder_data.Name);
					break;
				case CmsTranslatableType.Menu:
					if ((0 == m_intId))
					{
						WorkareaTitlebarTitle = GetMessage("alt Export All Menus for Translation");
					}
					else
					{
						WorkareaTitlebarTitle = string.Format(GetMessage("alt Export for Translation Menu") + "\"{0}\"", m_intId);
					}
					break;
				case CmsTranslatableType.Taxonomy:
					if ((0 == m_intId))
					{
						WorkareaTitlebarTitle = GetMessage("alt export all taxos for translation");
					}
					else
					{
						WorkareaTitlebarTitle = string.Format(GetMessage("alt export for translation taxonomy") + "\"{0}\"", m_intId);
					}
					break;
				default:
					return;
			}
		}
		txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);

		string strBackAction = Request.QueryString["backpage"];
		if ((strBackAction == string.Empty))
		{
			strBackAction = "Localize";
		}
		string strBackId = "";
		switch (m_Type)
		{
            case CmsTranslatableType.Product:
            case CmsTranslatableType.Content:
				strBackId = "&id=" + m_intId.ToString() + "&folder_id=" + m_intFolderId.ToString();
				break;
			case CmsTranslatableType.Folder:
				strBackId = "&id=" + m_intFolderId.ToString();
				break;
			case CmsTranslatableType.Menu:
				strBackId = "&id=" + m_intId.ToString();
				break;
			case CmsTranslatableType.Taxonomy:
				strBackId = "&id=" + m_intId.ToString();
				break;
			default:
				return;
		}

		result.Append("<table><tr>");

		string strBackPage = "";
		if ((!string.IsNullOrEmpty(Request.QueryString["callerpage"])))
		{
			strBackPage = Request.QueryString["callerpage"] + "?" + System.Web.HttpUtility.UrlDecode(Request.QueryString["origurl"]);
		}
		else if ((Request.QueryString["backpage"] == "history"))
		{
			strBackPage = "javascript:history.back()";
		}
		else
		{
			strBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" + ContentLanguage + "&action=" + strBackAction + strBackId);
		}

		result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", strBackPage, GetMessage("alt back button text"), GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

		if (("localizeexport" != m_strPageAction))
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/icons/translationExport.png", "#", GetMessage("alt Click here to create XLIFF files for translation"), GetMessage("lbl Create XLIFF Files for Translation"), "onclick='DisplayXLIFFPanel(false); DisplayHoldMsg(true); return SubmitForm(0,\"validate()\")'", StyleHelper.TranslationButtonCssClass, true));
		}
		result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
		result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
		result.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
		result.Append("</tr></table>");
		htmToolBar.InnerHtml = result.ToString();
	}

	private void GenerateTargetLanguageList(int ContentLanguage)
	{
		Ektron.Cms.Common.Criteria<LocaleProperty> criteria = new Ektron.Cms.Common.Criteria<LocaleProperty>(
		LocaleProperty.EnglishName, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
		criteria.PagingInfo.RecordsPerPage = Int32.MaxValue;
		criteria.AddFilter(LocaleProperty.Enabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);

        List<LocaleData> locales = _locApi.GetList(criteria);
        List<LocaleData> displayLocales = new List<LocaleData>();

        if (m_intId > 0 &&
            (CmsTranslatableType.Content == m_Type || CmsTranslatableType.Product == m_Type)
            )
        {
            LocalizableCmsObjectType locObjType = LocalizableCmsObjectType.Content;
            switch (content_data.Type)
            {
                case (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Content:
                    locObjType = LocalizableCmsObjectType.Content;
                    break;
                case (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.CatalogEntry:
                    locObjType = LocalizableCmsObjectType.Product;
                    break;
                default:
                    locObjType = LocalizableCmsObjectType.DmsAsset;
                    break;
            }
            List<LocalizationObjectData> localeObjectData = new List<LocalizationObjectData>();
            Ektron.Cms.Framework.Localization.LocalizationObject localizationObject = new Ektron.Cms.Framework.Localization.LocalizationObject();
            localeObjectData = localizationObject.GetLocalizationObjectList(locObjType, m_intId, -1);
            List<LocaleData> preselectedLocales = new List<LocaleData>();
            locales.ForEach(delegate(LocaleData loc)
            {
                LanguageState enabled = (localeObjectData.FindAll(x => x.ObjectLanguage == loc.Id).Count > 0 ? LanguageState.Active : LanguageState.Undefined);
                LocaleData uiDisplayLocale = new LocaleData(loc.Id, loc.LCID, loc.EnglishName, loc.NativeName, loc.IsRightToLeft, loc.Loc, loc.Culture, loc.UICulture, loc.LangCode, loc.XmlLang, loc.FlagFile, loc.FlagUrl, loc.FallbackId, enabled);
                displayLocales.Add(uiDisplayLocale);
            });
        }
        else
        {
            displayLocales = locales;
        }

		BoundField field = default(BoundField);
                
		LanguageGrid.Columns.Clear();

		// Selected?
		field = new BoundField();
		field.DataField = "";
		//.HeaderText = "Export"
		field.HeaderText = "<input type=\"checkbox\" name=\"chkAll\" onclick=\"onCheckAll(this)\" checked=\"checked\" />";
		field.HtmlEncode = false;
		field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
		field.HeaderStyle.Width = new Unit(20, UnitType.Pixel);
		field.ItemStyle.Wrap = false;
		field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
		LanguageGrid.Columns.Add(field);

		// Flag Icon
		field = new BoundField();
		field.DataField = "FlagFile";
		field.HeaderText = "";
		field.HeaderStyle.Width = new Unit(20, UnitType.Pixel);
		field.ItemStyle.Wrap = false;
		field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
		LanguageGrid.Columns.Add(field);

		// Language Name
		field = new BoundField();
		field.DataField = "CombinedName";
		field.HtmlEncode = false;
		field.SortExpression = LocaleProperty.EnglishName.ToString();
		field.HeaderText = GetMessage("generic name");
		field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
		field.ItemStyle.Wrap = false;
		LanguageGrid.Columns.Add(field);

		// Loc
		field = new BoundField();
		field.DataField = "Loc";
		field.SortExpression = LocaleProperty.Loc.ToString();
		field.HeaderText = GetMessage("lbl loc");
		field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
		field.HeaderStyle.Width = new Unit(6, UnitType.Em);
		field.ItemStyle.Wrap = false;
		field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
		LanguageGrid.Columns.Add(field);

		//// Language Code
		//field = new BoundField();
		//field.DataField = "XmlLang";
		//// or "BrowserCode"
		//field.SortExpression = EkDS.SortBy.XmlLang.ToString();
		//// or .BrowserCode
		//field.HeaderText = GetMessage("lbl code");
		//field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
		//field.HeaderStyle.Width = new Unit(6, UnitType.Em);
		//field.ItemStyle.Wrap = false;
		//field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
		//LanguageGrid.Columns.Add(field);

		// Language ID (decimal)
		field = new BoundField();
		field.DataField = "Id";
		field.SortExpression = LocaleProperty.Id.ToString();
		field.HeaderText = GetMessage("generic ID");
		field.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
		field.HeaderStyle.CssClass = "right";
		field.HeaderStyle.Width = new Unit(8, UnitType.Em);
		field.ItemStyle.Wrap = false;
		field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
		field.ItemStyle.CssClass = "right";
		LanguageGrid.Columns.Add(field);

		//// Language ID (hex)
		//field = new BoundField();
		//field.DataField = "LanguageID";
		//field.HtmlEncode = false;
		//// necessary to make DataFormatString effective
		//field.DataFormatString = "{0:x4}";
		//field.HeaderText = GetMessage("lbl hex");
		//field.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
		//field.HeaderStyle.Width = new Unit(4, UnitType.Em);
		//field.ItemStyle.Wrap = false;
		//field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
		//LanguageGrid.Columns.Add(field);

		// FireFox: border between cells is a result of the <table rules="all" attribute, which I do not know how to eliminate.

		LanguageGrid.RowDataBound += LanguageGrid_RowDataBound;

        LanguageGrid.DataSource = displayLocales;
		LanguageGrid.DataBind();
	}

	protected void LanguageGrid_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridView gv = (GridView)sender;
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			LocaleData data = (LocaleData)e.Row.DataItem;
			int iColumn = 0;

			if (data.Id == ContentLanguage)
			{
				e.Row.Visible = false;
			}

			HtmlInputCheckBox chkSelect = new HtmlInputCheckBox();
			chkSelect.ID = "ExportLang";
			chkSelect.Value = data.Id.ToString();
			chkSelect.Attributes.Add("title", "Check to export");
			chkSelect.Checked = data.Enabled;

			e.Row.Cells[iColumn].Controls.Add(chkSelect);
			iColumn += 1;

			// Flag Icon
			System.Web.UI.HtmlControls.HtmlImage objImg = new System.Web.UI.HtmlControls.HtmlImage();
			objImg.Src = data.FlagUrl;
			objImg.Alt = data.FlagFile;
			objImg.Attributes.Add("title", objImg.Alt);
			objImg.Width = 16;
			objImg.Height = 16;

			e.Row.Cells[iColumn].Controls.Add(objImg);
			iColumn += 1;
		}
	}

	#endregion

	#region "LOCALIZATION - Export"
	private void ExportForTranslation()
	{
		string strTargetLanguages = "";
		// comma-delimited list
		StringBuilder sbHtml = new StringBuilder();
		strTargetLanguages = GetTargetLanguages();

		switch (m_Type)
		{
            case CmsTranslatableType.Product:
                localizationManager.StartExportProductForTranslation(m_intId.ToString(), strTargetLanguages);
                break;
			case CmsTranslatableType.Content:
				m_objLocalizationApi.StartExportContentForTranslation(m_intId.ToString(), strTargetLanguages);
				break;
			case CmsTranslatableType.Folder:
				m_objLocalizationApi.StartExportFolderForTranslation(m_intFolderId, Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes, m_bRecursive, strTargetLanguages);
				break;
			case CmsTranslatableType.Menu:
				m_objLocalizationApi.StartExportMenusForTranslation(strTargetLanguages);
				break;
			case CmsTranslatableType.Taxonomy:
				m_objLocalizationApi.StartExportTaxonomyForTranslation(strTargetLanguages);
				break;
			default:
				return;
		}
	}

	private string GetTargetLanguages()
	{
		string strLanguages = "";
		try
		{
			strLanguages = Request.Form["TargetLanguages"];
		}
		catch (Exception)
		{
			// ignore error
		}
		return strLanguages;
	}
	#endregion

	private void RegisterResources()
	{
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
	}
}
