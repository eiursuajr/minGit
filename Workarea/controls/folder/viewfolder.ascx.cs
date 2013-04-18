using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Site;
using Ektron.Cms.DataIO.LicenseManager;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Localization;
using Ektron.Cms.Content;
using Ektron.Cms.Controls;

public partial class viewfolder : System.Web.UI.UserControl
{
    #region Member Variables

    public const string _ContentTypeUrlParam = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam;
    public const int _CMSContentType_AllTypes = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes;
    public const int _ManagedAsset_Max = Ektron.Cms.Common.EkConstants.ManagedAsset_Max;
    public const int _ManagedAsset_Min = Ektron.Cms.Common.EkConstants.ManagedAsset_Min;

    public ContentAPI _ContentApi;
    public EkMessageHelper _MessageHelper;
    protected TaxonomyRequest taxonomy_request;
    protected CommonApi _Common = new CommonApi();

    private string _AppImgPath = "";
    private string _AppPath = "";
    private string _ApplicationPath;
    private AssetInfoData[] _AssetInfoData;
    private BlogData _BlogData = null;
    private bool _ChangeLanguage = false;
    private ContentData _ContentData;
    private long _ContentId = 0;
    private int _ContentLanguage = -1;
    private EkEnumeration.CMSContentSubtype _ContentSubTypeSelected;
    private int _ContentType = 0;
    private string _ContentTypeQuerystringParam;
    private string _ContentTypeSelected = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes.ToString();
    private EkTasks _Comments;
    private long _CurrentUserId = 0;
    private DiscussionBoard _DiscussionBoard;
    private DiscussionForum[] _DiscussionForums;
    private EkContent _EkContent;
    private EkContentCol _EkContentCol;
    private int _EnableMultilingual = 0;
    private FolderData _FolderData;
    private int _FolderType = 0;
    private string _From = "";
    private bool _HasXmlConfig = false;
    private long _Id = 0;
    private bool _CheckedInOrApproved = false;
    private bool _IsMac;
    private bool _IsMyBlog = false;
    private LocalizationAPI _LocalizationApi = new LocalizationAPI();
    private string _NextActionType = "";
    private string _OrderBy = "";
    private string _OrderByDirection = "";
    private Microsoft.VisualBasic.Collection _PageData;
    private int _PagingCurrentPageNumber = 1;
    private int _PagingTotalPagesNumber = 1;
    private int _PagingPageSize = 20;
    private string _PageAction = "";
    private long _PostID = 0;
    private PermissionData _PermissionData;
    private string _SelectedEditControl;
    protected TaxonomyData taxonomy_data;
    private string _SitePath = "";
    private StyleHelper _StyleHelper = new StyleHelper();
    private bool _TakeAction = false;
    private EkTask _Task;
    private string _TreeViewId;
    private UserAPI _UserApi = new UserAPI();
    private long _XmlConfigID = 0;
    private string _XmlConfigType = "EkXmlConfigType";
    protected bool _initIsFolderAdmin = false;
    private long checktaxid = 0;
    protected bool _isFolderAdmin = false;
    protected bool _initIsCopyOrMoveAdmin = false;
    protected bool _isCopyOrMoveAdmin = false;
    protected string _SelectedTaxonomyList = "";
    protected EkContent m_refContent;
    protected bool _IsArchivedEvent = false;
    protected long _BoardID = 0;
    protected string direction ="";

    #endregion

    #region Properties

    public string ApplicationPath
    {
        get
        {
            return _ApplicationPath;
        }
        set
        {
            _ApplicationPath = value;
        }
    }

    #endregion

    #region Events

    public viewfolder()
    {

        //set contentapi
        _ContentApi = new ContentAPI();
        _MessageHelper = _ContentApi.EkMsgRef;

        //set ApplicationPath property
        char[] endSlash = new char[] { '/' };
        this.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(endSlash.ToString().ToCharArray());
        this._SitePath = _ContentApi.SitePath.TrimEnd(endSlash.ToString().ToCharArray());

    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterJS();
        this.RegisterCSS();

        this.GetQueryStringValues();
        string contentLanguage = this.GetQueryStringValue("LangType");
        //Utilities.SetLanguage(_ContentApi);
        _ContentLanguage = (contentLanguage == string.Empty) ? _ContentApi.ContentLanguage : Convert.ToInt32(contentLanguage);
        _EkContent = _ContentApi.EkContentRef;

        if (!_ContentApi.IsLoggedIn || !_ContentApi.LoadPermissions(0, "users", 0).IsLoggedIn)
        {
            Response.Redirect((string)("reterror.aspx?info=" + _MessageHelper.GetMessage("lbl not logged in")), true);
        }

        _FolderData = _ContentApi.GetFolderById(_Id);
        if (_FolderData == null)
        {
            Response.Redirect((string)("reterror.aspx?info=" + _MessageHelper.GetMessage("com: folder does not exist")), true);
            return;
        }
        if (_FolderData.FolderType == (int)Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            Ektron.Cms.Controls.CalendarDataSource foldersource = new Ektron.Cms.Controls.CalendarDataSource();
            foldersource.defaultId = _Id;
            foldersource.sourceType = Ektron.Cms.Controls.SourceType.SystemCalendar;
            calendardisplay.DataSource.Clear();
            calendardisplay.DataSource.Add(foldersource);
            calendardisplay.LanguageID = _ContentLanguage;
            calendardisplay.AllowEventEditing = _PageAction == "viewarchivecontentbycategory" ? false : true;
            calendardisplay.Fill();

        }

    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            //retrieve querystring values
            _IsMac = System.Convert.ToBoolean((Request.Browser.Platform.IndexOf("Win") == -1) ? true : false);
            _SelectedEditControl = Utilities.GetEditorPreference(Request);
            _ChangeLanguage = false;

            string uniqueKey = _ContentApi.UserId.ToString() + _ContentApi.UniqueId.ToString() + "RejectedFiles";
            if ((Session[uniqueKey] != null) && (Session[uniqueKey].ToString().Length > 0))
            {
                string failedUpload = Convert.ToString(Session[uniqueKey]);
                failedUpload = failedUpload.Replace("\\", "\\\\");
                failedUpload = failedUpload.Replace("\'", "\\\'");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DisplayFailedUploads", "alert(\'" + _MessageHelper.GetMessage("lbl error message for multiupload") + " " + failedUpload + "\\n" + _MessageHelper.GetMessage("js:cannot add file with add and plus") + "\');", true);
                Session.Remove(uniqueKey);
            }

            uniqueKey = _ContentApi.UserId.ToString() + _ContentApi.UniqueId.ToString() + "NoFilesToUpload";
            if ((Session[uniqueKey] != null) && (Session[uniqueKey].ToString().Length > 0))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "NoFilesToUpload", "alert(\'" + _MessageHelper.GetMessage("lbl no files selected for multiupload") + "\');", true);
                Session.Remove(uniqueKey);
            }

            addContentLanguageMessage.Text = _MessageHelper.GetMessage("alert msg add content lang");
            errorLinksDisabled.Text = _MessageHelper.GetMessage("js err links disabled");
            dropuploader.Text = "";

            if (IsPostBack == false || (Request.Form.Count > 0 && !string.IsNullOrEmpty(Request.Form[hdnIsPostData.UniqueID])) || _ChangeLanguage)
            {
                switch (_PageAction)
                {
                    case "viewarchivecontentbycategory":
                    case "viewcontentbycategory":
                        switch (_TreeViewId)
                        {
                            case "0":
                                ViewContentByCategory();
                                break;
                            case "-1":
                                ViewTaxonomyContentByCategory();
                                break;
                            case "-2":
                                ViewCollectionContentByCategory();
                                break;
                            default:
                                ViewContentByCategory();
                                break;
                        }
                        break;
                }
            }

            this.hdnIsPostData.Value = "true";

            //set paging ui
            if (_PagingTotalPagesNumber > 1)
            {
                this.SetPagingUI();
            }
            else
            {
                divPaging.Visible = false;
            }

        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _ContentLanguage), false);
        }
    }

    public void AdHocPaging_Click(object sender, System.Web.UI.WebControls.CommandEventArgs eventArgs)
    {
        //Do nothing, handled client-side
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _PagingCurrentPageNumber = 1;
                break;
            case "Last":
                _PagingCurrentPageNumber = int.Parse((string)totalPages.Text);
                break;
            case "Next":
                _PagingCurrentPageNumber = System.Convert.ToInt32((int.Parse((string)currentPage.Text) + 1 <= int.Parse((string)totalPages.Text)) ? (int.Parse((string)currentPage.Text) + 1) : (int.Parse((string)currentPage.Text)));
                break;
            case "Prev":
                _PagingCurrentPageNumber = System.Convert.ToInt32((int.Parse((string)currentPage.Text) - 1 >= 1) ? (int.Parse((string)currentPage.Text) - 1) : (int.Parse((string)currentPage.Text)));
                break;
        }
        currentPage.Text = _PagingCurrentPageNumber.ToString();
        ViewContentByCategory();
        //set paging ui, postback is true here, so no need to check for IsPostBack()
        if (_PagingTotalPagesNumber > 1)
        {
            this.SetPagingUI();
        }
        else
        {
            divPaging.Visible = false;
        }
        this.hdnIsPostData.Value = "true";
    }

    #endregion

    #region FOLDER - ViewContentByCategory OR ViewArchiveContentByCategory

    private PermissionData Permissiondata
    {
        get { return (_PermissionData ?? (_PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0))); }
    }

    private bool CanDoCollections
    {
        get
        {
            return (Permissiondata.IsAdmin
                || Permissiondata.IsCollections
                || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
                || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminCollection));
        }
    }

    private bool CanDoMenus
    {
        get
        {
            return (Permissiondata.IsAdmin
                || IsFolderAdmin()
                || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
                || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminMenu));
        }
    }

    private bool IsFolderAdmin()
    {
        if (_initIsFolderAdmin)
        {
            return _isFolderAdmin;
        }
        _isFolderAdmin = _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id, 0, false);
        _initIsFolderAdmin = true;
        return _isFolderAdmin;
    }

    private bool IsCopyOrMoveAdmin()
    {
        if (_initIsCopyOrMoveAdmin)
        {
            return _isCopyOrMoveAdmin;
        }
        _isCopyOrMoveAdmin = _ContentApi.IsARoleMemberForFolder((long)EkEnumeration.CmsRoleIds.MoveOrCopy, _Id, _ContentApi.UserId, false);
        _initIsCopyOrMoveAdmin = true;
        return _isCopyOrMoveAdmin;
    }

    public bool ViewContentByCategory()
    {
        _CurrentUserId = _ContentApi.UserId;
        _AppImgPath = _ContentApi.AppImgPath;
        _AppPath = _ContentApi.AppPath;
        _SitePath = _ContentApi.SitePath;
        _EnableMultilingual = _ContentApi.EnableMultilingual;

        url_action.Text = _PageAction;
        url_id.Text = _Id.ToString();

        if (_FolderData == null)
        {
            _FolderData = _ContentApi.GetFolderById(_Id);
        }
        if (_FolderData == null)
        {
            Response.Redirect((string)("reterror.aspx?info=" + _MessageHelper.GetMessage("com: folder does not exist")), true);
            return false;
        }
        else
        {
            if (_FolderData.XmlConfiguration != null)
            {
                _HasXmlConfig = true;
            }
            _PermissionData = _ContentApi.LoadPermissions(_Id, "folder", 0);
            _FolderType = _FolderData.FolderType;
        }

        //Setting JS Variable for global use through workarea.aspx page.
        pasteFolderType.Text = Convert.ToString(_FolderData.FolderType);
        pasteFolderId.Text = Convert.ToString(_FolderData.Id);
        pasteParentId.Text = Convert.ToString(_FolderData.ParentId);

        if (!string.IsNullOrEmpty(Request.QueryString["IsArchivedEvent"]))
        {
            _IsArchivedEvent = Convert.ToBoolean(Request.QueryString["IsArchivedEvent"]);
            is_archived.Text = Convert.ToString(_IsArchivedEvent);
        }

        _AssetInfoData = _ContentApi.GetAssetSupertypes();
        if ((Ektron.Cms.Common.EkConstants.CMSContentType_Content == Convert.ToInt32(_ContentTypeSelected)) || (Ektron.Cms.Common.EkConstants.CMSContentType_Archive_Content == Convert.ToInt32(_ContentTypeSelected)) || (Ektron.Cms.Common.EkConstants.CMSContentType_XmlConfig == Convert.ToInt32(_ContentTypeSelected)))
        {
            _ContentType = int.Parse(_ContentTypeSelected);
        }
        else if (Ektron.Cms.Common.EkConstants.CMSContentType_Forms == Convert.ToInt32(_ContentTypeSelected) || Ektron.Cms.Common.EkConstants.CMSContentType_Archive_Forms == Convert.ToInt32(_ContentTypeSelected))
        {
            _ContentType = int.Parse(_ContentTypeSelected);
        }
        else if (_ManagedAsset_Min <= Convert.ToInt32(_ContentTypeSelected) && Convert.ToInt32(_ContentTypeSelected) <= _ManagedAsset_Max)
        {
            if (DoesAssetSupertypeExist(_AssetInfoData, int.Parse(_ContentTypeSelected)))
            {
                _ContentType = int.Parse(_ContentTypeSelected);
            }
        }
        else if (Convert.ToInt32(_ContentTypeSelected) == _CMSContentType_AllTypes)
        {
            _ContentType = Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryForms;
        }
        else if (_IsArchivedEvent == true && (Convert.ToInt32(_ContentTypeSelected) == EkConstants.CMSContentType_Archive_ManagedFiles || Convert.ToInt32(_ContentTypeSelected) == EkConstants.CMSContentType_Archive_OfficeDoc || Convert.ToInt32(_ContentTypeSelected) == EkConstants.CMSContentType_Archive_MultiMedia || Convert.ToInt32(_ContentTypeSelected) == EkConstants.CMSContentType_Archive_Images))
        {
            _ContentType = int.Parse(_ContentTypeSelected);
        }

        _ContentTypeSelected = _ContentType.ToString();

        _PageData = new Microsoft.VisualBasic.Collection();
        _PageData.Add(_Id, "FolderID", null, null);
        if (_FolderData.FolderType == 1) //blog
        {
            _PageData.Add("BlogPost", "OrderBy", null, null);
        }
        else
        {
            _PageData.Add(_OrderBy, "OrderBy", null, null);
        }
        if (Request.QueryString["orderbydirection"] == "desc")
            direction = "desc";
        else
            direction = "asc";
        _OrderByDirection = direction;
        _PageData.Add(_OrderByDirection, "OrderByDirection", null, null);
        _PageData.Add(_ContentLanguage, "m_intContentLanguage", null, null);
        switch ((Ektron.Cms.Common.EkEnumeration.FolderType)_FolderData.FolderType)
        {
            case Ektron.Cms.Common.EkEnumeration.FolderType.Blog:
                _ContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
                _PageData.Add(_ContentType, "ContentType", null, null);
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum:
                _ContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
                _PageData.Add(_ContentType, "ContentType", null, null);
                break;
            default:
                if (_ContentType > 0)
                {
                    _PageData.Add(_ContentType, "ContentType", null, null);
                }
                break;
        }

        if (_ContentType == 1 && _ContentSubTypeSelected != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes)
        {
            _PageData.Add(_ContentSubTypeSelected, "ContentSubType", null, null);
        }

        if ((Ektron.Cms.Common.EkEnumeration.FolderType)(_FolderData.FolderType) == Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            calendardisplay.Visible = true;
            pnlThreadedDiscussions.Visible = false;
            if ((Request.QueryString["showAddEventForm"] != null) && Request.QueryString["showAddEventForm"] == "true")
            {
                if (ViewState["AddEventFormDisplay"] == null || System.Convert.ToBoolean(ViewState["AddEventFormDisplay"]) == false) //only show once
                {
                    ViewState.Add("AddEventFormDisplay", true);
                    DateTime startDT = DateTime.Now.Date.AddHours(8);
                    if (Request.QueryString["startDT"] != null)
                    {
                        startDT = DateTime.ParseExact(Request.QueryString["startDT"], "yyyyMMddHHmm", new System.Globalization.CultureInfo(1033));
                    }
                    calendardisplay.ShowInsertForm(startDT);
                }
            }
            if (Request.QueryString["editEvent"] != null)
            {
                if (ViewState["editEvent"] == null || System.Convert.ToBoolean(ViewState["editEvent"]) == false) //only show once
                {
                    ViewState.Add("editEvent", true);
                    calendardisplay.ShowEditForm(Request.QueryString["editEvent"]);
                }
            }

            ScriptManager.RegisterClientScriptBlock(Page, typeof(UserControl), "CalendarCleanup", "try{ window.EditorCleanup(); }catch(ex){}", true);
        }
        _PagingPageSize = _ContentApi.RequestInformationRef.PagingSize;
        if ((Ektron.Cms.Common.EkEnumeration.FolderType)(_FolderData.FolderType) == Ektron.Cms.Common.EkEnumeration.FolderType.Blog)
        {
            _BlogData = _ContentApi.BlogObject(_FolderData);
        }

        //if it's a calendar then we do it on prerender
        if ((Ektron.Cms.Common.EkEnumeration.FolderType)(_FolderData.FolderType) != Ektron.Cms.Common.EkEnumeration.FolderType.Calendar)
        {
            if (_PageAction == "viewarchivecontentbycategory")
            {
                _EkContentCol = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, ref _PagingTotalPagesNumber);
                _NextActionType = "ViewContentByCategory";
            }
            else if (_PageAction == "viewcontentbycategory")
            {
                _EkContentCol = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, ref _PagingTotalPagesNumber);
                _NextActionType = "ViewArchiveContentByCategory";
            }
            //paging goes here

            int i;
            for (i = 0; i <= _EkContentCol.Count - 1; i++)
            {
                if (_EkContentCol.get_Item(i).ContentStatus == "A")
                {
                    _TakeAction = true;
                    _CheckedInOrApproved = true;
                    break;
                }
                else
                {
                    if (_EkContentCol.get_Item(i).ContentStatus == "I")
                    {
                        _CheckedInOrApproved = true;
                    }
                }
            }
        }
        else
        {
            if (_PageAction == "viewarchivecontentbycategory")
            {
                _NextActionType = "ViewContentByCategory";
            }
            else if (_PageAction == "viewcontentbycategory")
            {
                _NextActionType = "ViewArchiveContentByCategory";
            }
        }

        switch ((Ektron.Cms.Common.EkEnumeration.FolderType)(_FolderData.FolderType))
        {
            case Ektron.Cms.Common.EkEnumeration.FolderType.Catalog:
                if (_PageAction == "viewarchivecontentbycategory")
                {
                    _NextActionType = "ViewContentByCategory";
                }
                else if (_PageAction == "viewcontentbycategory")
                {
                    _NextActionType = "ViewArchiveContentByCategory";
                }

                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "objselnotice", "<script type=\"text/javascript\">var objSelSupertype = null;</script>");

                CatalogEntry CatalogManager = new CatalogEntry(_ContentApi.RequestInformationRef);
                System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
                Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();

                entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _Id);
                entryCriteria.PagingInfo.CurrentPage = Convert.ToInt32(_PagingCurrentPageNumber.ToString());
                entryCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;

                if (_ContentApi.RequestInformationRef.ContentLanguage > 0)
                {
                    entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _ContentApi.RequestInformationRef.ContentLanguage);
                }

                switch (this._ContentTypeQuerystringParam)
                {
                    case "0":
                        long[] IdList = new long[3];
                        IdList[0] = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product);
                        IdList[1] = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct);
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList);
                        break;
                    case "2":
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit);
                        break;
                    case "3":
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle);
                        break;
                    case "4":
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct);
                        break;
                }

                if (_PageAction == "viewarchivecontentbycategory")
                {
                    entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);
                }
                else
                {
                    entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, false);
                }
                if (Request.QueryString["orderbydirection"] == "desc")
                    direction = "desc";
                else
                    direction = "asc";
                if(direction == "desc")
                    entryCriteria.OrderByDirection = (EkEnumeration.OrderByDirection)OrderByDirection.Descending;
                else
                entryCriteria.OrderByDirection = (EkEnumeration.OrderByDirection)OrderByDirection.Ascending;

                switch (_OrderBy.ToLower())
                {
                    case "language":
                        entryCriteria.OrderByField = EntryProperty.LanguageId;
                        break;
                    case "id":
                        entryCriteria.OrderByField = EntryProperty.Id;
                        break;
                    case "status":
                        entryCriteria.OrderByField = EntryProperty.ContentStatus;
                        break;
                    case "entrytype":
                        entryCriteria.OrderByField = EntryProperty.EntryType;
                        break;
                    case "sale":
                        entryCriteria.OrderByField = EntryProperty.SalePrice;
                        break;
                    case "list":
                        entryCriteria.OrderByField = EntryProperty.ListPrice;
                        break;
                    default: //"title"
                        entryCriteria.OrderByField = EntryProperty.Title;
                        break;
                }

                entryList = CatalogManager.GetList(entryCriteria);

                for (int j = 0; j <= entryList.Count - 1; j++)
                {
                    if (entryList[j].Status == "A")
                    {
                        _TakeAction = true;
                        _CheckedInOrApproved = true;
                        break;
                    }
                    else
                    {
                        if (entryList[j].Status == "I")
                        {
                            _CheckedInOrApproved = true;
                        }
                    }
                }

                _PagingTotalPagesNumber = System.Convert.ToInt32(entryCriteria.PagingInfo.TotalPages);

                //paging goes here

                ViewCatalogToolBar(entryList.Count);
                Populate_ViewCatalogGrid(_EkContentCol, entryList);
                _ContentType = int.Parse(_ContentTypeSelected);
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.Blog:
                _IsMyBlog = System.Convert.ToBoolean((_BlogData.Id == _ContentApi.GetUserBlog(_ContentApi.UserId)) ? true : false);
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "objselnotice", "<script type=\"text/javascript\">var objSelSupertype = null;</script>");
                if (!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString()))
                {
                    _ContentType = System.Convert.ToInt32(Request.QueryString["ContType"]);
                    _Task = _ContentApi.EkTaskRef;
                    if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
                    {
                        _PostID = Convert.ToInt64(Request.QueryString["contentid"]);
                        _ContentData = _ContentApi.GetContentById(_PostID, 0);

                        Ektron.Cms.PageRequestData null_EktronCmsPageRequestData = null;
                        _Comments = _Task.GetTasks(_PostID, -1, -1, Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSTaskItemType.BlogCommentItem), "postcomment", 0, ref null_EktronCmsPageRequestData, "");
                    }
                    else
                    {

                        Ektron.Cms.PageRequestData null_EktronCmsPageRequestData2 = null;
                        _Comments = _Task.GetTasks(-1, -1, -1, Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSTaskItemType.BlogCommentItem), "", 0, ref null_EktronCmsPageRequestData2, "");
                    }
                    ViewBlogContentByCategoryToolBar();
                    Populate_ViewBlogCommentsByCategoryGrid(_Comments);
                }
                else
                {
                    Hashtable BlogPostCommentTally = new Hashtable();
                    BlogPostCommentTally = _EkContent.TallyCommentsForBlogPosts(_Id);
                    ViewBlogContentByCategoryToolBar();
                    Populate_ViewBlogPostsByCategoryGrid(_EkContentCol, BlogPostCommentTally);
                }
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.Media:
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "objselnotice", "<script type=\"text/javascript\">var objSelSupertype = null;</script>");
                ViewContentByCategoryToolBar();
                Populate_ViewMediaGrid(_EkContentCol);
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard:
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "objselnotice", "<script type=\"text/javascript\">var objSelSupertype = null;</script>");
                ViewDiscussionBoardToolBar();
                Populate_ViewDiscussionBoardGrid();
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum:
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "objselnotice", "<script type=\"text/javascript\">var objSelSupertype = null;</script>");
                bool bCanModerate = false;
                int itotalpages = 1;
                int icurrentpage = 1;
                if (!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString()))
                {
                    _ContentType = System.Convert.ToInt32(Request.QueryString["ContType"]);
                    if (this._ContentApi.UserId > 0 && ((!(_PermissionData == null) && _PermissionData.CanAddToImageLib == true) || _PermissionData.IsAdmin))
                    {
                        bCanModerate = true;
                    }
                    _DiscussionBoard = _ContentApi.GetTopic(_ContentId, true);
                    if (_DiscussionBoard == null)
                    {
                        throw new Exception("You may not have permission to view this object or it has been deleted.");
                    }
                    _BoardID = _DiscussionBoard.Id;
                    _ContentData = (ContentData)(_DiscussionBoard.Forums[0].Topics[0]);
                    _PermissionData = _ContentApi.LoadPermissions(_ContentId, "content", ContentAPI.PermissionResultType.All);
                    ViewRepliesToolBar();
                    _Task = _ContentApi.EkTaskRef;
                    if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
                    {
                        _PostID = Convert.ToInt64(Request.QueryString["contentid"]);
                        _Comments = _Task.GetTopicReplies(_PostID, _DiscussionBoard.Id, ref icurrentpage, 0, 0, ref itotalpages, bCanModerate);
                    }
                    else
                    {

                        Ektron.Cms.PageRequestData null_EktronCmsPageRequestData3 = null;
                        _Comments = _Task.GetTasks(-1, -1, -1, Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSTaskItemType.TopicReplyItem), "", 0, ref null_EktronCmsPageRequestData3, "");
                    }
                    Populate_ViewTopicRepliesGrid(_Comments);
                }
                else
                {
                    ArrayList ForumPostCommentTally = new ArrayList();
                    DiscussionBoard thisboard;
                    bool bModerator = false;
                    if (_PermissionData.IsAdmin == true || _PermissionData.CanAddToImageLib == true)
                    {
                        bModerator = true;
                    }
                    thisboard = _EkContent.GetForumbyID(_Id.ToString(), bModerator, _PagingCurrentPageNumber, ref this._PagingTotalPagesNumber, "", this._ContentApi.RequestInformationRef.PagingSize);

                    //paging here

                    ForumPostCommentTally = _EkContent.GetRepliesForTopics(_Id);
                    ViewDiscussionForumToolBar();
                    Populate_ViewForumPostsByCategoryGrid(thisboard.Forums[0].Topics, ForumPostCommentTally);
                }
                break;
            case Ektron.Cms.Common.EkEnumeration.FolderType.Calendar:
                ViewCalendarToolBar();
                break;
            default:
                ViewContentByCategoryToolBar();
                Populate_ViewContentByCategoryGrid(_EkContentCol);
                break;
        }

        Util_SetJs();
        return true;
    }

    public long GetAddMultiType()
    {
        long returnValue;
        // gets ID for "add multiple" asset type
        returnValue = 0;
        int count;
        _AssetInfoData = _ContentApi.GetAssetSupertypes();
        if (_AssetInfoData != null)
        {

            for (count = 0; count <= _AssetInfoData.Length - 1; count++)
            {
                if (_ManagedAsset_Min <= _AssetInfoData[count].TypeId && _AssetInfoData[count].TypeId <= _ManagedAsset_Max)
                {
                    if ("*" == _AssetInfoData[count].PluginType)
                    {
                        returnValue = _AssetInfoData[count].TypeId;
                    }
                }
            }
        }
        return returnValue;
    }
    private void ViewCalendarToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        bool bSelectedFound = false;
        bool bViewContent = System.Convert.ToBoolean("viewcontentbycategory" == _PageAction); // alternative is archived content

        if (bViewContent)
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view events in calendar msg") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("Archive Event Title");
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive event title") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("view event title");
        }
        result.Append("<table><tr>" + "\r\n");
        if ((_PermissionData.CanAdd && bViewContent) || _PermissionData.IsReadOnly == true)
        {

            if (_PermissionData.CanAdd && bViewContent)
            {
                if (!bSelectedFound)
                {
                    _ContentType = System.Convert.ToInt32(_CMSContentType_AllTypes);
                }
            }
        }

        string buttonId;

        if (_PermissionData.CanAdd && !_IsArchivedEvent)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + _MessageHelper.GetMessage("lbl New") + "</span></td>");
        }

        if (((_PermissionData.CanAdd) && bViewContent) || _PermissionData.IsReadOnly == true)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + _MessageHelper.GetMessage("lbl View") + "</span></td>");
        }

        if (_PermissionData.CanDeleteFolders && bViewContent && _Id != 0)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"delete\">" + _MessageHelper.GetMessage("lbl Delete") + "</span></td>");
        }

        buttonId = Guid.NewGuid().ToString();

        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + _MessageHelper.GetMessage("lbl Action") + "</span></td>");

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)("calendar_" + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");

        result.Append("<script type=\"text/javascript\">" + Environment.NewLine);

        if (_PermissionData.CanAdd)
        {
            result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
            result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/calendarAdd.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("add cal event") + "\", function() { AddNewEvent(); } );" + Environment.NewLine);
            result.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
        }

        if (((_PermissionData.CanAdd) && bViewContent) || _PermissionData.IsReadOnly == true)
        {
            result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);
            if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && bViewContent) || IsFolderAdmin())
            {
                result.Append("    viewmenu.addBreak();" + Environment.NewLine);
                result.Append("    viewmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/properties.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Folder Properties") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + _Id + "\' } );" + Environment.NewLine);
            }

            if (bViewContent)
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("archive content title") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&IsArchivedEvent=true" + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : (Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
            }
            else
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("unarchive event title") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected)).ToString()) : (Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected)).ToString()) : "")) + "\' } );" + Environment.NewLine);
            }
            AddLanguageMenu(result);
            result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);
        }

        // Delete Menu
        if (_PermissionData.CanDeleteFolders && bViewContent && _Id != 0)
        {
            result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
            result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'images/UI/Icons/folderDelete.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl delete calendar") + "\", function() { if( ConfirmFolderDelete(" + _Id + ") ) { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId + "\'; }} );" + Environment.NewLine);
            result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
        }

        result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/magnifier.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("generic Search") + "\", function() { window.location.href = \'isearch.aspx?LangType=" + _ContentLanguage + "&action=showdlg&folderid=" + _Id + "\'; } );" + Environment.NewLine);
        result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);

        result.Append("    </script>" + Environment.NewLine);
        result.Append("" + Environment.NewLine);

        htmToolBar.InnerHtml = result.ToString();
    }

    private void ViewContentByCategoryToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        int count = 0;
        int lAddMultiType = 0;
        bool bSelectedFound = false;
        bool bViewContent = System.Convert.ToBoolean("viewcontentbycategory" == _PageAction); // alternative is archived content
        Ektron.Cms.PageBuilder.WireframeModel wireframeModel = new Ektron.Cms.PageBuilder.WireframeModel();

        if (bViewContent)
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view contents of folder msg") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("Archive Content Title");
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive content title") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("view content title");
        }
        result.Append("<table><tr>" + "\r\n");
        if ((_PermissionData.CanAdd && bViewContent) || _PermissionData.IsReadOnly == true)
        {

            if (_PermissionData.CanAdd && bViewContent)
            {
                if (!bSelectedFound)
                {
                    _ContentType = System.Convert.ToInt32(_CMSContentType_AllTypes);
                }
            }
        }

        string buttonId;

        if ((_PermissionData.CanAdd || _PermissionData.CanAddFolders) && bViewContent)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + _MessageHelper.GetMessage("lbl New") + "</span></td>");
        }

        if ((_PermissionData.CanAdd) || _PermissionData.IsReadOnly)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + _MessageHelper.GetMessage("lbl View") + "</span></td>");
        }

        int totalPages = 1;
        if (_PageAction == "viewarchivecontentbycategory")
        {
            // no comparable method; use member variable holding page count when viewing archived items (maybe we should always do this and drop the api hit of doing it again?)
            totalPages = _PagingTotalPagesNumber;
        }
        else
        {
            _ContentApi.GetChildContentByFolderId(_Id, false, "name", 1, ref totalPages, 1);
        }

        if ((_PermissionData.CanDeleteFolders && bViewContent && _Id != 0) || ((bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin()) || _PermissionData.CanDelete) && totalPages > 0))
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"delete\">" + _MessageHelper.GetMessage("lbl Delete") + "</span></td>");
        }

        buttonId = Guid.NewGuid().ToString();

        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + _MessageHelper.GetMessage("lbl Action") + "</span></td>");

        if (_EnableMultilingual == 1)
        {
            SiteAPI m_refsite = new SiteAPI();
            LanguageData[] language_data = new LanguageData[1];
            language_data = m_refsite.GetAllActiveLanguages();
        }
        XmlConfigData[] active_xml_list;
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id);
        bool smartFormsRequired = System.Convert.ToBoolean(!Utilities.IsNonFormattedContentAllowed(active_xml_list));
        bool canAddAssets = System.Convert.ToBoolean((_PermissionData.CanAdd || _PermissionData.CanAddFolders) && bViewContent);
        if (_ContentLanguage < 1 || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Blog) || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum) || _FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard) || smartFormsRequired == true || canAddAssets == false)
        {
        }
        else
        {
            if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, false))
            {
                if (Request.Browser.Browser == "IE" && Request.UserAgent.Contains("Windows NT 6.0") && Request.ServerVariables["HTTPS"] == "on") //Vista IE Https then take the user to file upload since vista https is not supported by webdav
                {
                    result.Append("<td id='DmsMenu'>&nbsp;<a class=\"ek_thickbox " + StyleHelper.MergeCssClasses(new string[2] { StyleHelper.SecondaryFunctionCssClass, StyleHelper.AddAssetButtonCssClass }) + "\" href=\"" + _ContentApi.AppPath + "edit.aspx?id=" + _FolderData.Id + "&ContType=103&type=add&close=false&lang_id=" + _ContentLanguage.ToString() + "title=\"" + _MessageHelper.GetMessage("lbl file upload") + "\"><img id=\"DeskTopHelp\" title= \"" + _MessageHelper.GetMessage("alt add assets text") + "\" border=\"0\" src=\"images/UI/Icons/Import.png\"/></a>&nbsp;</td>");
                }
                else
                {
                    //result.Append("<td id='DmsMenu'>&nbsp;<a class=\"ek_thickbox " + StyleHelper.MergeCssClasses(new string[2] { StyleHelper.SecondaryFunctionCssClass, StyleHelper.AddAssetButtonCssClass }) + "\" href=\"" + _ContentApi.AppPath + "DragDropCtl.aspx?id=" + _Id.ToString() + "&lang_id=" + _ContentLanguage.ToString() + "&EkTB_iframe=true&height=120&width=500&refreshCaller=true&scrolling=false&modal=true\" class=\"ek_thickbox\" title=\"" + _MessageHelper.GetMessage("document management system") + "\"><img id=\"DeskTopHelp\" title= \"" + _MessageHelper.GetMessage("alt add assets text") + "\" border=\"0\" src=\"images/UI/Icons/Import.png\"/></a>&nbsp;</td>");
                    result.Append("<td id='DmsMenu'>&nbsp;<a class=\"ek_thickbox " + StyleHelper.MergeCssClasses(new string[2] { StyleHelper.SecondaryFunctionCssClass, StyleHelper.AddAssetButtonCssClass }) + "\" href=\"#\" onclick=\"ektb_show('" + _MessageHelper.GetMessage("document management system") + "', '" + _ContentApi.AppPath + "DragDropCtl.aspx?id=" + _Id.ToString() + "&lang_id=" + _ContentLanguage.ToString() + "&height=120&width=500&refreshCaller=true&scrolling=false&modal=true&EkTB_iframe=true', null, '', false);\" class=\"ek_thickbox\" title=\"" + _MessageHelper.GetMessage("document management system") + "\"><img id=\"DeskTopHelp\" title= \"" + _MessageHelper.GetMessage("alt add assets text") + "\" border=\"0\" src=\"images/UI/Icons/Import.png\"/></a>&nbsp;</td>");
                }
            }
        }

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");

        result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
        result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);

        if (_PermissionData.CanAddFolders || (_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)))
        {

            if (_PermissionData.CanAddFolders)
            {

                if (!_FolderData.IsCommunityFolder)
                {
                    result.Append("    filemenu.addItem(\"&nbsp;<img src=\'images/UI/Icons/folder.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Folder") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&type=folder&action=AddSubFolder&id=" + _Id + "\' } );" + Environment.NewLine);
                }

                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/tree/folderBlogClosed.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Blog") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=blog&id=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderBoard.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Discussion Board") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=discussionboard&id=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderCommunity.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Community Folder") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=communityfolder&id=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/tree/folderCalendarClosed.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Calendar Folder") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=calendar&id=" + _Id + "\' } );" + Environment.NewLine);

                if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
                {
                    result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderGreen.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl commerce catalog") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=catalog&id=" + _Id + "\' } );" + Environment.NewLine);
                }
                if (_Id == 0 && LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.MultiSite)) //domain folder
                {
                    result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderSite.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl site Folder") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&type=site&action=AddSubFolder&id=" + _Id + "\' } );" + Environment.NewLine);
                }
                result.Append("    filemenu.addBreak();" + Environment.NewLine);

            }
            else
            {

                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderGreen.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl commerce catalog") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=catalog&id=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addBreak();" + Environment.NewLine);

            }

        }

        if (_PermissionData.CanAdd)
        {
            TemplateData[] active_templates;
            active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id);
            bool foundWireframe = false;
            bool foundNormal = false;
            bool foundmasterlayout = false;

            foreach (TemplateData t in active_templates)
            {
                if (t.SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Wireframes)
                {
                    foundWireframe = true;
                }
                else if (t.SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
                {
                    foundmasterlayout = true;
                }
                else
                {
                    foundNormal = true;
                }
                if (foundWireframe && foundNormal && foundmasterlayout)
                {
                    break;
                }
            }
            if (Utilities.IsNonFormattedContentAllowed(active_xml_list) && foundNormal)
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentHtml.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl html content") + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content, true) + " } );" + Environment.NewLine);
                if (((!_IsMac) && (!(_AssetInfoData == null))) || ("ContentDesigner" == _SelectedEditControl))
                {
                    result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentForm.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl html formsurvey") + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, Ektron.Cms.Common.EkConstants.CMSContentType_Forms, false) + " } );" + Environment.NewLine);
                }
            }

            if (foundWireframe || foundmasterlayout) //folder has a wireframe associated
            {
                // Register JS
                Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
                Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
                Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
                Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
                Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "PageBuilder/Wizards/js/ektron.pagebuilder.wizards.js", "EktronPageBuilderWizardsJS");
                Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS");

                // register necessary CSS
                Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
                Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.AppPath + "PageBuilder/Wizards/css/ektron.pagebuilder.wizards.css", "EktronPageBuilderWizardsCSS");

                if (foundWireframe || foundmasterlayout)
                {
                    string layoutstr;
                    layoutstr = "tmpContLang = AddNewPage(); if (tmpContLang > 0) { Ektron.PageBuilder.Wizards.showAddPage({mode: \'add\', folderId: " + _FolderData.Id + ", language: tmpContLang, fromWorkarea: true}) };";
                    result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl pagebuilder layouts") + "\", function() { " + layoutstr + " } );" + Environment.NewLine);
                }

                if (foundWireframe && _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CreateMasterLayout))
                {
                    string masterstr;
                    masterstr = "tmpContLang = AddNewPage(); if (tmpContLang > 0) { Ektron.PageBuilder.Wizards.showAddMasterPage({mode: \'add\', folderId: " + _FolderData.Id + ", language: tmpContLang, fromWorkarea: true}) };";
                    result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl pagebuilder master layouts") + "\", function() { " + masterstr + " } );" + Environment.NewLine);
                }
            }

            if (!_IsMac || ("ContentDesigner" == _SelectedEditControl))
            {
                if ((foundWireframe && foundNormal) || foundNormal)
                {
                    if (active_xml_list.Length > 0 && Utilities.IsNonFormattedContentAllowed(active_xml_list))
                    {
                        if ((active_xml_list.Length == 1 && active_xml_list[0] == null) || (active_xml_list.Length == 1 && active_xml_list[0].Id == 0))
                        {

                        }
                        else
                        {
                            result.Append("    var contentTypesMenu = new Menu( \"contentTypes\" );" + Environment.NewLine);
                            result.Append("    filemenu.addBreak();" + Environment.NewLine);
                            int k;
                            for (k = 0; k <= active_xml_list.Length - 1; k++)
                            {
                                if (active_xml_list[k].Id != 0)
                                {
                                    result.Append("    contentTypesMenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/icons/contentsmartform.png" + "\'/>&nbsp;&nbsp;" + active_xml_list[k].Title + "\", function() { " + _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list[k].Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content) + " } );" + Environment.NewLine);
                                }
                            }
                            //result.Append("    contentTypesMenu.addBreak();" & Environment.NewLine)
                            result.Append("    filemenu.addMenu(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/icons/contentsmartform.png" + "\'/>&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl smart form") + "\", contentTypesMenu);" + Environment.NewLine);
                        }

                    }
                    else if (active_xml_list.Length > 0 && !Utilities.IsNonFormattedContentAllowed(active_xml_list))
                    {
                        result.Append("    filemenu.addBreak();" + Environment.NewLine);
                        int k;
                        for (k = 0; k <= active_xml_list.Length - 1; k++)
                        {
                            if (active_xml_list[k].Id != 0)
                            {
                                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/icons/contentsmartform.png" + "\'/>&nbsp;&nbsp;" + active_xml_list[k].Title + "\", function() { " + _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list[k].Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content) + " } );" + Environment.NewLine);
                            }
                        }
                    }
                }
                result.Append("    filemenu.addBreak();" + Environment.NewLine);
            }

            //If ((Not m_bIsMac) AndAlso (Not (IsNothing(asset_data))) AndAlso Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
            if ((!(_AssetInfoData == null)) && Utilities.IsNonFormattedContentAllowed(active_xml_list))
            {
                if (_AssetInfoData.Length > 0)
                {
                    for (count = 0; count <= _AssetInfoData.Length - 1; count++)
                    {
                        if (_ManagedAsset_Min <= _AssetInfoData[count].TypeId && _AssetInfoData[count].TypeId <= _ManagedAsset_Max)
                        {
                            if ("*" == _AssetInfoData[count].PluginType)
                            {
                                lAddMultiType = _AssetInfoData[count].TypeId;
                            }
                        }
                    }
                    string imgsrc = string.Empty;
                    string txtCommName = string.Empty;
                    if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, false))
                    {

                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentDMSDocument.png" + "\' />&nbsp;&nbsp;";
                        txtCommName = _MessageHelper.GetMessage("lbl dms documents");
                        result.Append("filemenu.addItem(\"" + imgsrc + "" + txtCommName + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, 103, false) + " } );" + Environment.NewLine);
                        result.Append(" if (ShowMultipleUpload() && CheckSTSUpload()) {");
                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/contentStack.png" + "\' />&nbsp;&nbsp;";
                        txtCommName = _MessageHelper.GetMessage("lbl multiple documents");
                        if (Request.Cookies["DMS_Office_ver"] == null || string.IsNullOrEmpty(Request.Cookies["DMS_Office_ver"].Value))
                        {
                            result.Append("filemenu.addItem(\"" + imgsrc + "" + txtCommName + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, 9875, false) + " } );" + Environment.NewLine);
                        }
                        else
                        {
                            result.Append("filemenu.addItem(\"" + imgsrc + "" + txtCommName + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, 9876, false) + " } );" + Environment.NewLine);
                        }

                        result.Append("}");
                    }
                }
            }
        }
        if (CanDoCollections || CanDoMenus)
        {
            result.Append("    filemenu.addBreak();" + Environment.NewLine);

            if (CanDoCollections)
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/collection.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Collection") + "\", function() { window.location.href = \'collections.aspx?LangType=" + _ContentLanguage + "&action=Add&folderid=" + _Id + "\' } );" + Environment.NewLine);
            }

            if (CanDoMenus)
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/menu.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Menu") + "\", function() { window.location.href = \'collections.aspx?LangType=" + _ContentLanguage + "&action=AddMenu&back=" + EkFunctions.UrlEncode("content.aspx?action=ViewContentByCategory&id=" + _Id) + "&folderid=" + _Id + "\' } );" + Environment.NewLine);
            }

            result.Append("" + Environment.NewLine);
        }

        if (_PermissionData.CanAdd || _PermissionData.CanAddFolders)
        {
            result.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
        }

        result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);

        if (bViewContent)
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderView.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl All Types"), 98, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + _CMSContentType_AllTypes + "); } );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentHtml.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl html content"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content + ",false" + "); } );" + Environment.NewLine);
            if (((!_IsMac) && (!(_AssetInfoData == null))) || ("ContentDesigner" == _SelectedEditControl))
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentForm.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl html formsurvey"), Ektron.Cms.Common.EkConstants.CMSContentType_Forms, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + Ektron.Cms.Common.EkConstants.CMSContentType_Forms + "); } );" + Environment.NewLine);
            }
            if (wireframeModel.FindByFolderID(_FolderData.Id).Length > 0) //folder has a wireframe associated
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl pagebuilder layouts"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData + ",false " + "); } );" + Environment.NewLine);
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl pagebuilder master layouts"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData + ",false " + "); } );" + Environment.NewLine);
            }
        }
        else
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderView.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl All Types"), 98, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateArchiveView(" + _CMSContentType_AllTypes + ",true " + "); } );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentHtml.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl html content"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content + ",true " + "); } );" + Environment.NewLine);
            if (((!_IsMac) && (!(_AssetInfoData == null))) || ("ContentDesigner" == _SelectedEditControl))
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentForm.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl html formsurvey"), Ektron.Cms.Common.EkConstants.CMSContentType_Forms, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateArchiveView(" + Ektron.Cms.Common.EkConstants.CMSContentType_Forms + ",true " + "); } );" + Environment.NewLine);
            }
            if (wireframeModel.FindByFolderID(_FolderData.Id).Length > 0) //folder has a wireframe associated
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl pagebuilder layouts"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData + ",true " + "); } );" + Environment.NewLine);
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/application/layout_content.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl pagebuilder master layouts"), Ektron.Cms.Common.EkConstants.CMSContentType_Content, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) + "\", function() { UpdateViewwithSubtype(" + Ektron.Cms.Common.EkConstants.CMSContentType_Content + ", " + (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData + ",true " + "); } );" + Environment.NewLine);
            }
        }




        result.Append("    viewmenu.addBreak();" + Environment.NewLine);
        if (((_PermissionData.CanAdd) && bViewContent) || _PermissionData.IsReadOnly == true)
        {
            if ((_AssetInfoData != null) && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, false))
            {
                if (_AssetInfoData.Length > 0)
                {
                    for (count = 0; count <= _AssetInfoData.Length - 1; count++)
                    {
                        if (_ManagedAsset_Min <= _AssetInfoData[count].TypeId && _AssetInfoData[count].TypeId <= _ManagedAsset_Max)
                        {
                            if ("*" == _AssetInfoData[count].PluginType)
                            {
                                lAddMultiType = _AssetInfoData[count].TypeId;
                            }
                            else
                            {
                                string imgsrc = string.Empty;
                                string txtCommName = string.Empty;
                                if (_IsArchivedEvent)
                                {
                                    if (_AssetInfoData[count].TypeId + 1000 == 1101)
                                    {
                                        imgsrc = "&nbsp;<img src=\'" + "images/UI/Icons/FileTypes/word.png" + "\' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Office Documents");
                                    }
                                    else if (_AssetInfoData[count].TypeId + 1000 == 1102)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentManagedFiles.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Managed Files");
                                    }
                                    else if (_AssetInfoData[count].TypeId + 1000 == 1106)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/FileTypes/image.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl image assets");
                                    }
                                    else if (_AssetInfoData[count].TypeId + 1000 == 1104)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/film.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Multimedia");
                                    }
                                    else
                                    {
                                        imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                    }
                                    if (_AssetInfoData[count].TypeId + 1000 != 1105)
                                    {
                                        result.Append("viewmenu.addItem(\"" + imgsrc + "" + MakeBold(txtCommName, System.Convert.ToInt32(_AssetInfoData[count].TypeId + 1000), Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + (_AssetInfoData[count].TypeId + 1000) + "); } );" + Environment.NewLine);
                                    }
                                }
                                else
                                {

                                    if (_AssetInfoData[count].TypeId == 101)
                                    {
                                        imgsrc = "&nbsp;<img src=\'" + "images/UI/Icons/FileTypes/word.png" + "\' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Office Documents");
                                    }
                                    else if (_AssetInfoData[count].TypeId == 102)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentManagedFiles.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Managed Files");
                                    }
                                    else if (_AssetInfoData[count].TypeId == 106)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/FileTypes/image.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl image assets");
                                    }
                                    else if (_AssetInfoData[count].TypeId == 104)
                                    {
                                        imgsrc = "&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/film.png" + " \' />&nbsp;&nbsp;";
                                        txtCommName = _MessageHelper.GetMessage("lbl Multimedia");
                                    }
                                    else
                                    {
                                        imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                    }
                                    if (_AssetInfoData[count].TypeId != 105)
                                    {
                                        result.Append("viewmenu.addItem(\"" + imgsrc + "" + MakeBold(txtCommName, _AssetInfoData[count].TypeId, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + _AssetInfoData[count].TypeId + "); } );" + Environment.NewLine);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            AddLanguageMenu(result);

            result.Append("    viewmenu.addBreak();" + Environment.NewLine);


            if (bViewContent && CanDoMenus)
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/menu.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Menu") + "\", function() { window.location.href = \'collections.aspx?LangType=" + _ContentLanguage + "&action=ViewAllMenus&folderid=" + _Id + "\' } );" + Environment.NewLine);
            }
            if (bViewContent && CanDoCollections)
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/collection.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Collection") + "\", function() { window.location.href = \'collections.aspx?LangType=" + _ContentLanguage + "&action=mainPage&folderid=" + _Id + "\' } );" + Environment.NewLine);
            }
            if (bViewContent)
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("archive content") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&IsArchivedEvent=true" + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
            }
            else
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("top Content") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + (Convert.ToInt32(Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
            }

            if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && bViewContent) || IsFolderAdmin())
            {
                result.Append("    viewmenu.addBreak();" + Environment.NewLine);
                result.Append("    viewmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/properties.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Folder Properties") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + _Id + "\' } );" + Environment.NewLine);
            }

            result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);

            // Delete Menu
            if ((_PermissionData.CanDeleteFolders && bViewContent && _Id != 0) || ((bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin()) || _PermissionData.CanDelete) && totalPages > 0))
            {

                result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
                if (_PermissionData.CanDeleteFolders && bViewContent && _Id != 0)
                {
                    string folderImgPath = "images/UI/Icons/folderDelete.png";

                    switch (_FolderType)
                    {
                        case 2: //Domain
                            folderImgPath = "images/UI/Icons/folderSiteDelete.png";
                            break;
                        case 6: //Community
                            folderImgPath = "images/UI/Icons/folderCommunityDelete.png";
                            break;
                        default:
                            break;
                        // use the default.
                    }
                    result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + folderImgPath + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl This Folder") + "\", function() { if( ConfirmFolderDelete(" + _Id + ") ) { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId + "\'; }} );" + Environment.NewLine);
                }
                if (bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin()) || _PermissionData.CanDelete)
                {
                    // get a count for the content in this folder
                    if (totalPages > 0)
                    {
                        if ((Convert.ToString(_EnableMultilingual) == "1") && (_ContentLanguage < 1))
                        {
                            result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentHtmlDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("top Content") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
                        }
                        else
                        {
                            //44595 - Delete content from the archive view should show up archived list rather than live content list.
                            if (_PageAction == "viewarchivecontentbycategory")
                            {
                                result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("top Content") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "&showarchive=true\'; } );" + Environment.NewLine);
                            }
                            else
                            {
                                result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("top Content") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "\'; } );" + Environment.NewLine);
                            }
                        }
                    }
                }
                result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
            }
        }

        result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
        if (_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXliff) && bViewContent && Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.Xliff, false))
        {
            result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/translationExport.png" + " \' />&nbsp;&nbsp;" + this._MessageHelper.GetMessage("lbl Export for translation") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=Localize&backpage=ViewContentByCategory&id=" + _Id + "\'; } );" + Environment.NewLine);
        }

        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/magnifier.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("generic Search") + "\", function() { window.location.href = \'isearch.aspx?LangType=" + _ContentLanguage + "&action=showdlg&folderid=" + _Id + "\'; } );" + Environment.NewLine);

        result.Append("    actionmenu.addBreak();" + Environment.NewLine);

        if (_CheckedInOrApproved && bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin() || IsCopyOrMoveAdmin()) && (_PermissionData.CanAdd || _PermissionData.CanEdit))
        {
            if (Convert.ToString(_EnableMultilingual) == "1" && _ContentLanguage < 1)
            {
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/cut.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl cut") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentCopy.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl copy") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
            }
            else
            {
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/cut.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl cut") + "\", function() { setClipBoard(); } );" + Environment.NewLine);
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentCopy.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl copy") + "\", function() { setCopyClipBoard(); }) ;" + Environment.NewLine);
            }
        }

        SiteAPI site = new SiteAPI();
        EkSite ekSiteRef = site.EkSiteRef;
        if (_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin) || _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncUser))
        {
            result.Append(GetSyncMenuOption());
        }
        result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);
        result.Append("" + Environment.NewLine);

        htmToolBar.InnerHtml = result.ToString();
    }

    private void ViewBlogContentByCategoryToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        bool bViewContent = System.Convert.ToBoolean("viewcontentbycategory" == _PageAction); // alternative is archived content
        bool bShowDelete = false;
        string helpAliasQualifier = "";
        bool folderIsHidden = _ContentApi.IsFolderHidden(_Id);

        if (_PageAction == "viewcontentbycategory")
        {
            altText = _MessageHelper.GetMessage("Archive Content Title");
        }
        else
        {
            altText = _MessageHelper.GetMessage("view content title");
        }
        if (_PostID > 0)
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("lbl view post comments") + " \"" + _FolderData.Name + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />"));
        }
        else
        {
            if (_PageAction == "viewcontentbycategory")
            {
                txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)((_IsMyBlog ? (_MessageHelper.GetMessage("view posts in journal msg")) : (_MessageHelper.GetMessage("view posts in blog msg") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />"));
            }
            else
            {
                txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive content title") + " \"" + _FolderData.Name + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />"));
            }

        }
        result.Append("<table><tr>" + "\r\n");

        if (Convert.ToInt32(Request.QueryString["ContType"]) == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id), _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }

        string buttonId;

        if ((_PermissionData.CanAdd) || (_PermissionData.CanEdit))
        {
            result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
            result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
            if (((_PermissionData.IsAdmin == true || _PermissionData.CanEdit == true) && _BlogData.EnableComments == true) && (_PermissionData.CanEdit && (!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString()))))
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/comment.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("comment text") + "\", function() { window.location.href = \'blogs/addeditcomment.aspx?action=Add&blogid=" + _Id + "&contentid=" + _PostID + "\'; } );" + Environment.NewLine);
            }
            XmlConfigData[] active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id);

            int xmlCount = 0;
            bool canAddHtmlPost = false;

            for (xmlCount = 0; xmlCount <= active_xml_list.Length - 1; xmlCount++)
            {
                if (active_xml_list[xmlCount].Title == "")
                {
                    canAddHtmlPost = true;
                }
            }

            //If (Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then ' we can always add normal HTML posts
            if (_PermissionData.CanAdd && canAddHtmlPost == true)
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/blog.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl blog post html") + "\", function() { " + _StyleHelper.GetAddAnchorByContentType(_Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content, true) + " } );" + Environment.NewLine);
            }
            if (active_xml_list.Length > 0 && Utilities.IsNonFormattedContentAllowed(active_xml_list))
            {
                if (active_xml_list.Length == 1 && active_xml_list[0].Id == 0)
                {

                }
                else
                {
                    result.Append("    var contentTypesMenu = new Menu( \"contentTypes\" );" + Environment.NewLine);
                    int k;
                    for (k = 0; k <= active_xml_list.Length - 1; k++)
                    {
                        if (active_xml_list[k].Id != 0)
                        {
                            result.Append("    contentTypesMenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/blog.png" + "\' />&nbsp;&nbsp;" + active_xml_list[k].Title + "\", function() { " + _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list[k].Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content) + " } );" + Environment.NewLine);
                        }
                    }
                    result.Append("    contentTypesMenu.addBreak();" + Environment.NewLine);
                    result.Append("    filemenu.addMenu(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/icons/contentsmartform.png" + "\'/>&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl smart form") + "\", contentTypesMenu);" + Environment.NewLine);
                    result.Append("    filemenu.addBreak();" + Environment.NewLine);
                }

            }
            else if (active_xml_list.Length > 0 && !Utilities.IsNonFormattedContentAllowed(active_xml_list))
            {
                int k;
                for (k = 0; k <= active_xml_list.Length - 1; k++)
                {
                    if (active_xml_list[k].Id != 0)
                    {
                        result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/blog.png" + "\' />&nbsp;&nbsp;" + active_xml_list[k].Title + "\", function() { " + _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list[k].Id, Ektron.Cms.Common.EkConstants.CMSContentType_Content) + " } );" + Environment.NewLine);
                    }
                }
                result.Append("    filemenu.addBreak();" + Environment.NewLine);
            }
            //End If
            if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && bViewContent) || IsFolderAdmin())
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/blogLink.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl roll link") + "\", function() { window.location.href = \'blogs/addblogroll.aspx?id=" + _Id.ToString() + "&LangType=" + _ContentLanguage + "\'; } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/note.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl generic subject") + "\", function() { window.location.href = \'blogs/addblogsubject.aspx?id=" + _Id.ToString() + "&LangType=" + _ContentLanguage + "\'; } );" + Environment.NewLine);
            }

            result.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
            result.Append("    </script>" + Environment.NewLine);
            if (((_PermissionData.CanAdd || _PermissionData.CanAddFolders) && bViewContent) || ((_PermissionData.IsAdmin == true || (_PermissionData.CanEdit == true && _BlogData.EnableComments == true)) && (_PermissionData.CanEdit && (!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString())))))
            {
                buttonId = Guid.NewGuid().ToString();

                result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + _MessageHelper.GetMessage("lbl New") + "</span></td>");
            }
        }

        result.Append("<script language=\"javascript\">" + Environment.NewLine);
        result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
        if (_PermissionData.CanDeleteFolders && bViewContent && _Id != 0 && !_IsMyBlog)
        {
            bShowDelete = true;
            result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/folderBlogDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl this blog") + "\", function() { if( ConfirmFolderDelete(" + _Id + ") ) { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId + "\'; }} );" + Environment.NewLine);
        }
        if (bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin()) || _PermissionData.CanDelete)
        {
            if ((Convert.ToString(_EnableMultilingual) == "1") && (_ContentLanguage < 1))
            {
                bShowDelete = true;
                result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/blogDelete.png" + " \' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl posts") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
            }
            else
            {
                bShowDelete = true;
                if (_PageAction == "viewarchivecontentbycategory")
                {
                    result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/blogDelete.png" + " \' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl posts") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "&showarchive=true\';  } );" + Environment.NewLine);
                }
                else
                {
                    result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/blogDelete.png" + " \' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl posts") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "\';  } );" + Environment.NewLine);
                }
            }
        }
        result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);

        buttonId = Guid.NewGuid().ToString();

        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + _MessageHelper.GetMessage("lbl View") + "</span></td>");

        if ((!folderIsHidden) && bShowDelete == true && !(!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString())))
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"delete\">" + _MessageHelper.GetMessage("lbl Delete") + "</span></td>");
        }
        result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
        result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);
        if (!(!string.IsNullOrEmpty(Request.QueryString["ContType"]) && Convert.ToInt32(Request.QueryString["ContType"]) == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments))
        {
            AddLanguageMenu(result);
        }
        result.Append("    viewmenu.addBreak();" + Environment.NewLine);

        if (bViewContent)
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("archive content") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
        }
        else
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("top Content") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
        }

        result.Append("    viewmenu.addBreak();" + Environment.NewLine);

        if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && bViewContent) || IsFolderAdmin())
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/properties.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Folder Properties") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + _Id + "\' } );" + Environment.NewLine);
        }

        result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);

        result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/magnifier.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("generic Search") + "\", function() { window.location.href = \'isearch.aspx?LangType=" + _ContentLanguage + "&action=showdlg&folderid=" + _Id + "\'; } );" + Environment.NewLine);
        result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);
        if (!(!string.IsNullOrEmpty(Request.QueryString["ContType"]) && (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString())))
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + _MessageHelper.GetMessage("lbl Action") + "</span></td>");
        }
        if (_ContentId > 0)
        {
            helpAliasQualifier = "_item";
        }

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction + helpAliasQualifier), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void AddLanguageMenu(StringBuilder result)
    {
        if (_EnableMultilingual == 1)
        {
            result.Append("    var languagemenu = new Menu( \"language\" );" + Environment.NewLine);
            result.Append("    viewmenu.addBreak();" + Environment.NewLine);

            string strSelectedLanguageName = "";
            string strName;
            strName = "All";
            if (_ContentLanguage == -1)
            {
                strName = "<b>" + strName + "</b>";
            }
            result.Append("    languagemenu.addItem(\"&nbsp;<img src=\'" + _ContentApi.AppImgPath + "flags/flag0000.gif\' alt=\\\"" + strName + "\\\" />&nbsp;&nbsp;" + strName + "\", function() { LoadLanguage(\'-1\'); } );" + Environment.NewLine);

            Ektron.Cms.Framework.Localization.LocaleManager locApi = new Ektron.Cms.Framework.Localization.LocaleManager();
            Ektron.Cms.Localization.LocaleData locData;
            List<Ektron.Cms.Localization.LocaleData> locDataList;

            Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy locTaxApi = new Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(_Common.RequestInformationRef);
            List<int> locList = locTaxApi.GetLocaleIdListForFolder(_FolderData.Id, this._ContentLanguage);
            if (locList.Count > 0)
            {
                locDataList = locApi.GetEnabledLocales(locList);
            }
            else
            {
                locDataList = locApi.GetEnabledLocales();
            }
            for (int i = 0; i <= (locDataList.Count - 1); i++)
            {
                locData = locDataList[i];
                strName = locData.CombinedName;
                if (_ContentLanguage == locData.Id)
                {
                    strSelectedLanguageName = locData.EnglishName;
                    strName = "<b>" + strName + "</b>";
                }
                result.AppendLine("    languagemenu.addItem(\"&nbsp;<img src=\'" + locData.FlagUrl + "\' />&nbsp;&nbsp;" + Ektron.Cms.API.JS.Escape(strName) + "\", function() { LoadLanguage(\'" + locData.Id + "\'); } );");
            }
            //result.AppendLine("    viewmenu.addMenu(""&nbsp;<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' alt=\""" & strSelectedLanguageName & "\"" />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Language") & """, languagemenu);")
            result.Append("    viewmenu.addMenu(\"&nbsp;<img src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' alt=\\\"" + strSelectedLanguageName + "\\\" />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl Language") + "\", languagemenu);" + Environment.NewLine);
        }
    }

    private bool TaxonomyExists(TaxonomyBaseData data)
    {
        if (data != null)
        {
            if (data.TaxonomyId == checktaxid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private string IsChecked(bool value)
    {
        if (value)
        {
            return " checked=\"checked\"";
        }
        else
        {
            return " ";
        }
    }
    private bool DoesAssetSupertypeExist(AssetInfoData[] asset_data, int lContentType)
    {
        int i = 0;
        bool result = false;
        if (!(asset_data == null))
        {
            for (i = 0; i <= asset_data.Length - 1; i++)
            {
                if (_ManagedAsset_Min <= asset_data[i].TypeId && asset_data[i].TypeId <= _ManagedAsset_Max)
                {
                    if (asset_data[i].TypeId == lContentType)
                    {
                        result = true;
                        break;
                    }
                }
            }
        }
        return (result);
    }

    private void Populate_ViewCalendar(EkContentCol contentdata)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;
        string imageDirection = string.Empty;
        if (Request.QueryString["orderbydirection"] == null)
            direction = "desc";
        else if (Request.QueryString["orderbydirection"] == "desc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadDownGrey.png\" \" />";
            direction = "asc";
        }
        else if (Request.QueryString["orderbydirection"] == "asc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadUpGrey.png\" \" />";
            direction = "desc";
        }
        strTag = "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=" + _PageAction + "&orderbydirection=" + direction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "TITLE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "title")
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + imageDirection + "</a> ";
        else
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FIRSTOCCURENCE";
        colBound.HeaderText = _MessageHelper.GetMessage("webcalendar first occurence");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl event type");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "language")
            colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + imageDirection + "</a>";
       else
           colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "id")
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATUS";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "status")
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATEMODIFIED";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "datemodified")
            colBound.HeaderText = strTag + "DateModified" + strtag1 + _MessageHelper.GetMessage("generic Date Modified") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "DateModified" + strtag1 + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITORNAME";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "editor")
            colBound.HeaderText = strTag + "editor" + strtag1 + _MessageHelper.GetMessage("generic Last Editor") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "editor" + strtag1 + _MessageHelper.GetMessage("generic Last Editor") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("FIRSTOCCURENCE", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(long)));
        dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
        dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));

        string ViewUrl = "";
        int i;

        for (i = 0; i <= contentdata.Count - 1; i++)
        {
            dr = dt.NewRow();

            //dmsMenuGuid is created to uniquely identify menu element component in the DOM,
            //just in case there is more than one menu that contains the same contentId & language
            //This case is known to apply in non-Workarea implementations of the DmsMenu but is
            //implemented for ALL DmsMenus, including the Workarea
            string dmsMenuGuid;
            dmsMenuGuid = (string)(System.Guid.NewGuid().ToString());
            string makeUnique = (string)(contentdata.get_Item(i).Id + contentdata.get_Item(i).Language + dmsMenuGuid);
            string contentStatus = contentdata.get_Item(i).ContentStatus;
            //If (contentdata.get_Item(i).ContentStatus = "A") Then
            dr[0] = "<div class=\"ektron dmsWrapper\"";
            dr[0] = dr[0] + " id=\"dmsWrapper" + makeUnique + "\">";
            dr[0] = dr[0] + "<p class=\"dmsItemWrapper\"";
            dr[0] = dr[0] + " id=\"dmsItemWrapper" + makeUnique + "\"";
            dr[0] = dr[0] + " title=\"View Menu\"";
            dr[0] = dr[0] + " style=\"overflow:visible;\"";
            dr[0] = dr[0] + ">";
            dr[0] = dr[0] + "<input type=\"hidden\" value=\'{\"id\":" + contentdata.get_Item(i).Id + ",";
            dr[0] = dr[0] + "\"parentId\":" + contentdata.get_Item(i).FolderId + ",";
            dr[0] = dr[0] + "\"languageId\":" + contentdata.get_Item(i).Language + ",";
            dr[0] = dr[0] + "\"status\":\"" + contentStatus + "\",";
            dr[0] = dr[0] + "\"guid\":\"" + dmsMenuGuid + "\",";
            dr[0] = dr[0] + "\"communityDocumentsMenu\":\"\",";
            dr[0] = dr[0] + "\"contentType\":" + Convert.ToInt32(contentdata.get_Item(i).ContentType) + ",";
            dr[0] = dr[0] + "\"dmsSubtype\":\"\"}\'";
            dr[0] = dr[0] + " id=\"dmsContentInfo" + makeUnique + "\" />";
            dr[0] = dr[0] + "<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/calendarViewDay.png\" onclick=\"event.cancelBubble=true;\" />";
            dr[0] = dr[0] + "<a";
            dr[0] = dr[0] + " id=\"dmsViewItemAnchor" + makeUnique + "\"";
            dr[0] = dr[0] + " class=\"dmsViewItemAnchor\"";
            dr[0] = dr[0] + " onclick=\"event.cancelBubble=true;\"";
            if (contentdata.get_Item(i).ContentStatus == "A")
            {
                ViewUrl = (string)("content.aspx?action=View&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
            }
            else
            {
                ViewUrl = (string)("content.aspx?action=ViewStaged&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
            }
            dr[0] = dr[0] + " href=\"" + ViewUrl + "\"";
            dr[0] = dr[0] + " title=\"View " + contentdata.get_Item(i).Title + "\"";
            dr[0] = dr[0] + ">";
            dr[0] = dr[0] + contentdata.get_Item(i).Title;
            dr[0] = dr[0] + "</a>";
            dr[0] = dr[0] + "</p>";
            dr[0] = dr[0] + "</div>";

            System.Xml.XmlDataDocument xd = new System.Xml.XmlDataDocument();
            try
            {
                xd.LoadXml(contentdata.get_Item(i).Html);
                System.Xml.XmlNode UTCstartDTXn = xd.SelectSingleNode("/root/StartTime");
                if (UTCstartDTXn != null)
                {
                    System.Xml.XmlNode alldayXn = xd.SelectSingleNode("/root/IsAllDay");
                    bool alldayBool = false;
                    DateTime UTCstartDT = new DateTime();
                    System.Globalization.CultureInfo ENci = new System.Globalization.CultureInfo(1033);
                    System.Globalization.CultureInfo userCi = EkFunctions.GetCultureInfo(_ContentApi.RequestInformationRef.UserCulture.ToString());
                    Ektron.Cms.Common.Calendar.TimeZoneInfo userTzi;

                    UTCstartDT = DateTime.ParseExact(UTCstartDTXn.InnerText, "s", ENci.DateTimeFormat);
                    userTzi = Ektron.Cms.Common.Calendar.TimeZoneInfo.GetTimeZoneInfo(_ContentApi.RequestInformationRef.UserTimeZone);
                    DateTime LocalstartDT = userTzi.ConvertUtcToTimeZone(UTCstartDT);
                    bool.TryParse(alldayXn.InnerText, out alldayBool);

                    if (!(LocalstartDT.Hour == 0 && LocalstartDT.Minute == 0) && !alldayBool)
                    {
                        if (userCi.DateTimeFormat.PMDesignator == string.Empty) //no ampm designator
                        {
                            dr[1] = LocalstartDT.ToString("ddd, MMM d yyyy hh:mm", userCi.DateTimeFormat) + " (" + userTzi.StandardName + ")"; //first occurence
                        }
                        else
                        {
                            dr[1] = LocalstartDT.ToString("ddd, MMM d yyyy h:mm tt", userCi.DateTimeFormat) + " (" + userTzi.StandardName + ")"; //first occurence
                        }
                    }
                    else if (alldayBool)
                    {
                        dr[1] = UTCstartDT.ToString("ddd, MMM d yyyy", userCi.DateTimeFormat); //first occurence
                    }
                    else
                    {
                        dr[1] = LocalstartDT.ToString("ddd, MMM d yyyy", userCi.DateTimeFormat) + " (" + userTzi.StandardName + ")"; //first occurence
                    }
                }
                System.Xml.XmlNode isvarianceXn = xd.SelectSingleNode("/root/IsVariance");
                System.Xml.XmlNode isCancelledXn = xd.SelectSingleNode("/root/IsCancelled");
                if (isvarianceXn != null)
                {
                    bool isvariance = bool.Parse(isvarianceXn.InnerText);
                    bool isCancelled = bool.Parse(isCancelledXn.InnerText);
                    if (isvariance && isCancelled)
                    {
                        dr[2] = "Variance - Cancelled occurence";
                    }
                    else if (isvariance && !isCancelled)
                    {
                        dr[2] = "Variance - Extra occurence";
                    }
                    else
                    {
                        dr[2] = "Original";
                    }
                }
            }
            catch
            {
                dr[1] = "Start Time could not be extracted.";
            }

            string LanguageDescription = Ektron.Cms.API.JS.Escape(contentdata.get_Item(i).LanguageDescription);
            dr[3] = "<a href=\"#ShowTip" + contentdata.get_Item(i).LanguageDescription + "\" onmouseover=\"ddrivetip(\'" + LanguageDescription + "\',\'ADC5EF\', 100);\" onmouseout=\"hideddrivetip()\" style=\"text-decoration:none;\">" + "<img src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(contentdata.get_Item(i).Language) + "\' />" + "</a>";
            dr[4] = contentdata.get_Item(i).Id;
            dr[5] = _StyleHelper.StatusWithToolTip(contentStatus);
            dr[6] = contentdata.get_Item(i).DateModified.ToString();
            dr[7] = contentdata.get_Item(i).LastEditorLname + ", " + contentdata.get_Item(i).LastEditorFname;
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
        //        _PagingTotalPagesNumber = 1
    }

    private void Populate_ViewContentByCategoryGrid(EkContentCol contentdata)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;
        string imageDirection = string.Empty;

        if (Request.QueryString["orderbydirection"] == null)
            direction = "desc";
        else if (Request.QueryString["orderbydirection"] == "desc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadDownGrey.png\" \" />";
            direction = "asc";
        }
        else if (Request.QueryString["orderbydirection"] == "asc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadUpGrey.png\" \" />";
            direction = "desc";
        }
        strTag = "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=" + _PageAction + "&orderbydirection=" + direction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "TITLE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "title")
        {
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + imageDirection + "</a>";
        }
        else
        {
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + "</a>";
        }        
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CONTENTTYPE";
        colBound.HeaderText = _MessageHelper.GetMessage("content type");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "language")
        {
            colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + imageDirection +"</a>";
        }
        else
        {
            colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + "</a>";
        }

        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "id")
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATUS";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "status")
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATEMODIFIED";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "datemodified")
            colBound.HeaderText = strTag + "DateModified" + strtag1 + _MessageHelper.GetMessage("generic Date Modified") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "DateModified" + strtag1 + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITORNAME";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "editor")
            colBound.HeaderText = strTag + "editor" + strtag1 + _MessageHelper.GetMessage("generic Last Editor") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "editor" + strtag1 + _MessageHelper.GetMessage("generic Last Editor") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("CONTENTTYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(long)));
        dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
        dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));

        string ViewUrl = "";
        int i;
        bool bAssetItem = false;
        string extension = "";

        for (i = 0; i <= contentdata.Count - 1; i++)
        {
            bAssetItem = System.Convert.ToBoolean((contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Assets) || ((Convert.ToInt32(contentdata.get_Item(i).ContentType) >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min) && (Convert.ToInt32(contentdata.get_Item(i).ContentType) <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)));
            dr = dt.NewRow();
            if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms || contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms)
            {
                if (contentdata.get_Item(i).ContentStatus == "A")
                {
                    ViewUrl = (string)("cmsform.aspx?action=ViewForm&folder_id=" + _Id + "&form_id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
                }
                else
                {
                    ViewUrl = (string)("cmsform.aspx?action=viewform&staged=true&folder_id=" + _Id + "&form_id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
                }
            }
            else
            {
                if (contentdata.get_Item(i).ContentStatus == "A")
                {
                    ViewUrl = (string)("content.aspx?action=View&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
                }
                else
                {
                    ViewUrl = (string)("content.aspx?action=ViewStaged&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
                }
            }

            //dmsMenuGuid is created to uniquely identify menu element component in the DOM,
            //just in case there is more than one menu that contains the same contentId & language
            //This case is known to apply in non-Workarea implementations of the DmsMenu but is
            //implemented for ALL DmsMenus, including the Workarea
            string dmsMenuGuid;
            dmsMenuGuid = (string)(System.Guid.NewGuid().ToString());
            string makeUnique = (string)(contentdata.get_Item(i).Id + contentdata.get_Item(i).Language + dmsMenuGuid);
            string contentStatus = contentdata.get_Item(i).ContentStatus;

            //If (contentdata.Item(i).ContentStatus = "A") Then
            dr[0] = "<div class=\"ektron dmsWrapper\"";
            dr[0] = dr[0] + " id=\"dmsWrapper" + makeUnique + "\">";
            dr[0] = dr[0] + "<p class=\"dmsItemWrapper\"";
            dr[0] = dr[0] + " id=\"dmsItemWrapper" + makeUnique + "\"";
            dr[0] = dr[0] + " title=\"View Menu\"";
            dr[0] = dr[0] + " style=\"overflow:visible;\"";
            dr[0] = dr[0] + ">";
            dr[0] = dr[0] + "<input type=\"hidden\" value=\'{\"id\":" + contentdata.get_Item(i).Id + ",";
            dr[0] = dr[0] + "\"parentId\":" + contentdata.get_Item(i).FolderId + ",";
            dr[0] = dr[0] + "\"languageId\":" + contentdata.get_Item(i).Language + ",";
            dr[0] = dr[0] + "\"status\":\"" + contentStatus + "\",";
            dr[0] = dr[0] + "\"guid\":\"" + dmsMenuGuid + "\",";
            dr[0] = dr[0] + "\"communityDocumentsMenu\": \"\",";
            dr[0] = dr[0] + "\"contentType\":" + Convert.ToInt32(contentdata.get_Item(i).ContentType) + ",";
            dr[0] = dr[0] + "\"dmsSubtype\":\"\"}\'";
            dr[0] = dr[0] + " id=\"dmsContentInfo" + makeUnique + "\" />";

            if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Content || contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Content)
            {
                if (contentdata.get_Item(i).ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData)
                {
                    dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppImgPath + "layout_content.png" + "\" onclick=\"event.cancelBubble=true;\" />";
                }
                else if (contentdata.get_Item(i).ContentSubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                {
                    dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppImgPath + "layout_content.png" + "\" onclick=\"event.cancelBubble=true;\" />";
                }
                else
                {
                    if (contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Content)
                    {
                        dr[0] = dr[0] + "<img src=\"" + _ContentApi.ApplicationPath + "Images/ui/icons/contentArchived.png\" onclick=\"event.cancelBubble=true;\" />";
                    }
                    else
                    {
                        dr[0] = dr[0] + "<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/contentHtml.png\" onclick=\"event.cancelBubble=true;\" />";
                    }

                }
            }
            else
            {
                dr[0] = dr[0] + "<span onclick=\"event.cancelBubble=true;\">" + contentdata.get_Item(i).AssetInfo.Icon + "</span>";
            }
            dr[0] = dr[0] + "<a";
            dr[0] = dr[0] + " id=\"dmsViewItemAnchor" + makeUnique + "\"";
            dr[0] = dr[0] + " class=\"dmsViewItemAnchor\"";
            dr[0] = dr[0] + " onclick=\"event.cancelBubble=true;\"";
            dr[0] = dr[0] + " href=\"" + ViewUrl + "\"";
            dr[0] = dr[0] + " title=\"View " + contentdata.get_Item(i).Title + "\"";
            dr[0] = dr[0] + ">";
            dr[0] = dr[0] + contentdata.get_Item(i).Title;
            dr[0] = dr[0] + "</a>";
            dr[0] = dr[0] + "</p>";
            dr[0] = dr[0] + "</div>";

            if (!(contentdata.get_Item(i).AssetInfo == null) && contentdata.get_Item(i).AssetInfo.Version != "")
            {
                extension = System.IO.Path.GetExtension(contentdata.get_Item(i).AssetInfo.Version);
            }

            dr[1] = GetContentTypeText(Convert.ToInt64(contentdata.get_Item(i).ContentType), contentdata.get_Item(i).XMLCollectionID, Convert.ToInt64(contentdata.get_Item(i).ContentSubType), extension);

            string LanguageDescription = Ektron.Cms.API.JS.Escape(contentdata.get_Item(i).LanguageDescription);
            dr[2] = "<a href=\"#ShowTip" + contentdata.get_Item(i).LanguageDescription + "\" onmouseover=\"ddrivetip(\'" + LanguageDescription + "\',\'ADC5EF\', 100);\" onmouseout=\"hideddrivetip()\" style=\"text-decoration:none;\">" + "<img src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(contentdata.get_Item(i).Language) + "\' />" + "</a>";
            dr[3] = contentdata.get_Item(i).Id;
            dr[4] = _StyleHelper.StatusWithToolTip(contentStatus);
            dr[5] = contentdata.get_Item(i).DateModified.ToString();
            dr[6] = contentdata.get_Item(i).LastEditorLname + ", " + contentdata.get_Item(i).LastEditorFname;
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }
    private string GetContentTypeText(long contentType, long xmlId, long contentSubType, string extension)
    {
        string result = "";

        switch (contentType)
        {
            case 1: // Content or Smart Form
                if (xmlId > 0)
                {
                    result = (string)(_MessageHelper.GetMessage("lbl smart form") + ": " + _ContentApi.GetXmlConfiguration(xmlId).Title.ToString());
                }
                else
                {
                    switch (contentSubType)
                    {
                        case 1://Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData
                            // this is a Page Layout
                            result = _MessageHelper.GetMessage("lbl pagebuilder layouts");
                            break;
                        case 3://Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData
                            // this is a Master Page Layout
                            result = _MessageHelper.GetMessage("lbl pagebuilder master layouts");
                            break;
                        case 2:// Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent:
                            // this is a web event, which indicates this is a Calendar Event entry
                            result = _MessageHelper.GetMessage("calendar event");
                            break;
                        default:
                            result = _MessageHelper.GetMessage("lbl html content");
                            break;
                    }
                }
                break;
            case 2: // HTML Form/Survey
                result = _MessageHelper.GetMessage("lbl html formsurvey");
                break;
            case 3: // Archived Content
                result = _MessageHelper.GetMessage("archive content");
                break;
            case 4: // Archived Form/Survey
                result = _MessageHelper.GetMessage("archive forms survey");
                break;
            case 7: // Library Item
                result = _MessageHelper.GetMessage("lbl library item");
                break;
            case 8: //Asset
                result = _MessageHelper.GetMessage("lbl asset");
                break;
            case 9: // Non Image Library Item
                result = _MessageHelper.GetMessage("nonimage library item");
                break;
            case 10: // PDF
                result = _MessageHelper.GetMessage("content:asset:pdf");
                break;
            case 12: // Archived Media
                result = _MessageHelper.GetMessage("lbl archived media");
                break;
            case 13: // Blog Comment
                result = _MessageHelper.GetMessage("lbl blog comment");
                break;
            case 14: // Smart Form
                if (xmlId > 0)
                {
                    result = (string)(_MessageHelper.GetMessage("lbl smart form") + ": " + _ContentApi.GetXmlConfiguration(xmlId).Title.ToString());
                }
                break;
            case 98: // Non Library Form
                result = _MessageHelper.GetMessage("nonlibrary form");
                break;
            case 99: // Non Library Content
                result = _MessageHelper.GetMessage("nonlibrary content");
                break;
            case 101: // Microsoft Office Documents
            case 1101:
                result = _MessageHelper.GetMessage("office document");
                break;
            case 102: // Managed Assets (Non-office Documents - pdf, txt, etc.)
            case 1102:
                result = _MessageHelper.GetMessage("managed asset");
                break;
            case 106: //Image assets - jpg, tif, gif
            case 1106:
                switch (extension.ToLower())
                {
                    case ".gif":
                        result = _MessageHelper.GetMessage("content:asset:image:gif");
                        break;
                    case ".jpeg":
                        result = _MessageHelper.GetMessage("content:asset:image:jpeg");
                        break;
                    case ".jpg":
                        result = _MessageHelper.GetMessage("content:asset:image:jpg");
                        break;
                    case ".png":
                        result = _MessageHelper.GetMessage("content:asset:image:png");
                        break;
                    case ".bmp":
                        result = _MessageHelper.GetMessage("content:asset:image:bmp");
                        break;
                    default:
                        //generic Image Asset label will be displayed for other image file types.
                        result = _MessageHelper.GetMessage("content:asset:image");
                        break;
                }
                break;
            case 104: // Multi Media
            case 1104:
                result = _MessageHelper.GetMessage("lbl multimedia");
                break;
            case 1111: // Discussion Topic
                result = _MessageHelper.GetMessage("discussion topic");
                break;
            case 3333: // Catalog Entry
                if (xmlId > 0)
                {
                    result = (string)(_ContentApi.GetXmlConfiguration(xmlId).Title.ToString());
                }
                else
                {
                    result = _MessageHelper.GetMessage("catalog entry");
                }
                break;
            default:
                switch (extension.ToLower())
                {
                    case ".pdf":
                        result = _MessageHelper.GetMessage("content:asset:pdf");
                        break;
                    case ".zip":
                        result = _MessageHelper.GetMessage("content:asset:zip");
                        break;
                    default:
                        result = _MessageHelper.GetMessage("unknown content type");
                        break;
                }
                break;

        }
        return result;
    }

    private void Populate_ViewMediaGrid(EkContentCol contentdata)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;

        strTag = "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=" + _PageAction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "A";
        colBound.HeaderText = "#&160;";
        colBound.ItemStyle.Width = Unit.Percentage(33);
        colBound.HeaderStyle.CssClass = "title-header";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "B";
        colBound.HeaderText = "#&160;";
        colBound.ItemStyle.Width = Unit.Percentage(33);
        colBound.HeaderStyle.CssClass = "title-header";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "C";
        colBound.HeaderText = "#&160;";
        colBound.ItemStyle.Width = Unit.Percentage(33);
        colBound.HeaderStyle.CssClass = "title-header";
        FolderDataGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr = dt.NewRow();

        dt.Columns.Add(new DataColumn("A", typeof(string)));
        dt.Columns.Add(new DataColumn("B", typeof(string)));
        dt.Columns.Add(new DataColumn("C", typeof(string)));

        string ViewUrl = "";
        string EditUrl = "";
        int i;
        bool bAssetItem = false;
        int iMod = 0;
        FolderData[] f = this._EkContent.GetChildFolders(this._Id, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
        if ((f != null) && f.Length > 0)
        {
            for (i = 0; i <= (f.Length - 1); i++)
            {
                iMod = i % 3;
                if (iMod == 0)
                {
                    dr = dt.NewRow();
                }
                dr[iMod] += "<br/><img src=\"" + this._ContentApi.AppImgPath + "thumb_folder.gif\" border=\"1\"/><br/><a href=\"content.aspx?action=ViewContentByCategory&id=" + f[i].Id.ToString() + "\">" + f[i].Name + "</a><br/><br/>";
                if (iMod == 2)
                {
                    dt.Rows.Add(dr);
                    dr = null;
                }
            }
        }
        int offset = iMod + 1;
        for (i = 0; i <= contentdata.Count - 1; i++)
        {
            bAssetItem = System.Convert.ToBoolean((contentdata.get_Item(i).ContentType == Ektron.Cms.Common.EkEnumeration.CMSContentType.Assets) || ((Convert.ToInt32(contentdata.get_Item(i).ContentType) >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min) && (Convert.ToInt32(contentdata.get_Item(i).ContentType) <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)));
            iMod = System.Convert.ToInt32((i + offset) % 3);
            if (iMod == 0)
            {
                dr = dt.NewRow();
            }
            dr[iMod] = "<br/><img src=\"" + this._ContentApi.AppImgPath + "thumb_bmp.gif\" border=\"1\"/><br/>";
            if (bAssetItem && (contentdata.get_Item(i).ContentStatus == "O") && (contentdata.get_Item(i).UserId == _CurrentUserId))
            {
                ViewUrl = System.Convert.ToString("content.aspx?action=View&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language); //& "&callerpage=content.aspx&origurl=" '& EkFunctions.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                EditUrl = System.Convert.ToString("edit.aspx?close=false&LangType=" + contentdata.get_Item(i).Language + "&id=" + contentdata.get_Item(i).Id + "&type=update&back_file=content.aspx&back_action=ViewContentByCategory&back_id=" + contentdata.get_Item(i).FolderId + "&back_LangType=" + contentdata.get_Item(i).Language);
            }
            else
            {
                if (contentdata.get_Item(i).ContentStatus == "A")
                {
                    dr[iMod] += "<a href=\"content.aspx?action=View&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]) + "\"" + "title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(contentdata.get_Item(i).Title + "\""), "\'", "`", 1, -1, 0) + "\'" + "> " + contentdata.get_Item(i).Title + " </a> ";
                }
                else
                {
                    dr[iMod] += "<a href=\"content.aspx?action=viewstaged&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]) + "\"" + "title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(contentdata.get_Item(i).Title + "\""), "\'", "`", 1, -1, 0) + "\'" + "> " + contentdata.get_Item(i).Title + " </a> ";
                }
            }
            dr[iMod] += "<br/><br/>";
            if (iMod == 2)
            {
                dt.Rows.Add(dr);
                dr = null;
            }
        }
        if (iMod < 2)
        {
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }

    private void Populate_ViewBlogPostsByCategoryGrid(EkContentCol contentdata, Hashtable commenttally)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;

        strTag = "<a href=\"content.aspx?LangType=" + _ContentApi.ContentLanguage + "&action=" + _PageAction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "TITLE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Title");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic language");
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = _MessageHelper.GetMessage("generic ID");
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATUS";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Status");
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATEMODIFIED";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Date Modified");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITORNAME";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Last Editor");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "COMMENTS";
        colBound.HeaderText = _MessageHelper.GetMessage("comments label");
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(long)));
        dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
        dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITORNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMENTS", typeof(string)));

        int i;
        string[] aValues;
        for (i = 0; i <= contentdata.Count - 1; i++)
        {
            commenttally = (Hashtable)commenttally.Clone();
            dr = dt.NewRow();

            dr[0] += "<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/blog.png\" style=\"margin-right:.25em;\" />";
            dr[0] += "<a href=\"content.aspx?action=View&folder_id=" + _Id + "&id=" + contentdata.get_Item(i).Id + "&mode=1&LangType=" + contentdata.get_Item(i).Language + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]) + "\"" + " title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(contentdata.get_Item(i).Title + "\""), "\'", "`", 1, -1, 0) + "\'" + ">";
            dr[0] += contentdata.get_Item(i).Title;
            dr[0] += "</a>";

            string LanguageDescription = Ektron.Cms.API.JS.Escape(contentdata.get_Item(i).LanguageDescription);
            dr[1] = "<a href=\"#ShowTip\" onmouseover=\"ddrivetip(\'" + LanguageDescription + "\',\'ADC5EF\', 100);\" onmouseout=\"hideddrivetip()\" style=\"text-decoration:none;\">" + "<img src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(contentdata.get_Item(i).Language) + "\' border=\"0\" />" + "</a>";
            dr[2] = contentdata.get_Item(i).Id;
            dr[3] = _StyleHelper.StatusWithToolTip(contentdata.get_Item(i).ContentStatus);
            dr[4] = contentdata.get_Item(i).DateModified.ToString();
            dr[5] = contentdata.get_Item(i).LastEditorLname + ", " + contentdata.get_Item(i).LastEditorFname;
            if (commenttally.ContainsKey((contentdata.get_Item(i).Id.ToString()) + "-" + contentdata.get_Item(i).Language.ToString()))
            {
                aValues = (string[])commenttally[(contentdata.get_Item(i).Id.ToString()) + "-" + contentdata.get_Item(i).Language.ToString()];
                string actionRequired = "";

                // let's do some math to see if any of the comments are pending admin interaction.
                // if the comment_sum/aValues(1) value is less than the number of comments times "7"
                // (the value of blog comment status complete), then at least one must be pending action.
                if (Convert.ToInt32(aValues[1]) < (Convert.ToInt32(aValues[0]) * 7))
                {
                    actionRequired = "class=\"blogCommentStatusPending\" title=\"" + _MessageHelper.GetMessage("moderator action required") + "\" ";
                }
                dr[6] += "<a " + actionRequired + "href=\"content.aspx?id=" + _Id + "&action=ViewContentByCategory&LangType=" + _ContentLanguage.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + contentdata.get_Item(i).Id.ToString() + "&viewin=" + contentdata.get_Item(i).Language.ToString() + "\">" + aValues[0].ToString() + "</a>";
            }
            else
            {
                dr[6] += "<a href=\"content.aspx?id=" + _Id + "&action=ViewContentByCategory&LangType=" + _ContentLanguage.ToString() + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + contentdata.get_Item(i).Id.ToString() + "&viewin=" + contentdata.get_Item(i).Language.ToString() + "\">" + 0 + "</a>";
            }

            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }

    private void Populate_ViewBlogCommentsByCategoryGrid(EkTasks blogcommentdata)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;
        int nBlogCommentCount;
        strTag = "<a href=\"content.aspx?LangType=" + _ContentApi.ContentLanguage + "&action=" + _PageAction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        FolderDataGrid.ShowHeader = false;

        colBound.DataField = "PREVIEW";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(145);
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLEDESCRIPTION";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        FolderDataGrid.Columns.Add(colBound);

        FolderDataGrid.BorderColor = System.Drawing.Color.White;
        FolderDataGrid.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
        FolderDataGrid.CellPadding = 6;
        FolderDataGrid.CellSpacing = 2;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("PREVIEW", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLEDESCRIPTION", typeof(string)));

        string ApproveURL = "";
        string ViewUrl = "";
        string EditUrl = "";
        string DeleteUrl = "";
        string sAppend = "";
        string _CommentDisplayName = string.Empty;
        int i;
        nBlogCommentCount = blogcommentdata.Count;
        for (i = 1; i <= blogcommentdata.Count; i++)
        {
            if (!(blogcommentdata.get_Item(i) == null))
            {
                if (_ContentLanguage == blogcommentdata.get_Item(i).ContentLanguage || (Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES == _ContentLanguage && Convert.ToInt32(Request.QueryString["viewin"]) == blogcommentdata.get_Item(i).ContentLanguage))
                {
                    if (blogcommentdata.get_Item(i).CommentDisplayName == string.Empty)
                    {
                        _CommentDisplayName = blogcommentdata.get_Item(i).CreatedByUser;
                    }
                    else
                    {
                        _CommentDisplayName = blogcommentdata.get_Item(i).CommentDisplayName;
                    }
                    dr = dt.NewRow();
                    if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
                    {
                        sAppend = (string)("&blogid=" + _Id.ToString() + "&contentid=" + Request.QueryString["contentid"]);
                    }
                    else
                    {
                        sAppend = (string)("&blogid=" + _Id.ToString());
                    }
                    ViewUrl = (string)("tasks.aspx?action=ViewTask&tid=" + blogcommentdata.get_Item(i).TaskID.ToString() + "&fromViewContent=1&ty=both&LangType=" + _ContentApi.ContentLanguage);
                    EditUrl = (string)("blogs/addeditcomment.aspx?action=Edit&id=" + blogcommentdata.get_Item(i).TaskID.ToString() + sAppend);
                    dr[0] += "<p class=\"center";
                    if (int.Parse(blogcommentdata.get_Item(i).State) == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.BlogCommentState.Pending))
                    {
                        dr[0] += " blogCommentStatusPending\" title=\"" + _MessageHelper.GetMessage("moderator action required");
                    }
                    dr[0] += "\">";
                    if (int.Parse(blogcommentdata.get_Item(i).State) == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.BlogCommentState.Pending))
                    {
                        ApproveURL = (string)("tasks.aspx?action=ApproveTask&tid=" + blogcommentdata.get_Item(i).TaskID.ToString() + "&ty=both" + sAppend);
                        dr[0] += "<img style=\"border: none; margin: .5em auto;\" src=\"" + _ContentApi.AppImgPath + "thumb_blogcomment.gif\" width=\"53\" height=\"55\"/><br/>";
                        if (_PermissionData.CanEdit == true)
                        {
                            dr[0] += "<a href=\"" + ApproveURL + "\">" + _MessageHelper.GetMessage("generic approve title") + "</a>&nbsp;|&nbsp;";
                        }
                        else
                        {
                            dr[0] += "<br/>&nbsp;";
                        }
                    }
                    else
                    {
                        dr[0] += "<img src=\"" + _ContentApi.AppImgPath + "thumb_blogcomment.gif\" width=\"53\" height=\"55\" style=\"border: none; margin: .5em auto;\"/><br />";
                    }
                    if (_PermissionData.CanEdit)
                    {
                        dr[0] += "<a href=\"" + EditUrl + "\">" + _MessageHelper.GetMessage("generic edit title") + "</a>&nbsp;|&nbsp;";
                        DeleteUrl = (string)("tasks.aspx?action=DeleteTask&tid=" + blogcommentdata.get_Item(i).TaskID.ToString() + "&ty=both" + sAppend);
                        dr[0] += "<a href=\"" + DeleteUrl + "\" onclick=\"return confirm(\'" + _MessageHelper.GetMessage("msg del comment") + "\');\">" + _MessageHelper.GetMessage("generic delete title") + "</a>&nbsp;";
                    }
                    dr[0] += "</p>";
                    dr[1] += "<font color=\"gray\">\"" + EkFunctions.HtmlEncode(blogcommentdata.get_Item(i).Description) + "\"</font><br/><font color=\"green\">" + _MessageHelper.GetMessage("lbl posted by") + " " + _CommentDisplayName + " " + _MessageHelper.GetMessage("res_isrch_on") + " " + blogcommentdata.get_Item(i).DateCreated.ToString() + "</font>";
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }

    public void ResetPostData()
    {
        _ChangeLanguage = true;
    }

    #endregion

    #region Catalog

    private void ViewCatalogToolBar(long entryCount)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        Ektron.Cms.Commerce.ProductType pProductType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);
        bool bSelectedFound = false;
        bool bViewContent = System.Convert.ToBoolean("viewcontentbycategory" == _PageAction); // alternative is archived content
        bool bCommerceAdmin = true;
        bool bFolderAdmin = false;

        bCommerceAdmin = _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin);
        bFolderAdmin = System.Convert.ToBoolean(bFolderAdmin || bCommerceAdmin);

        if (bViewContent)
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("lbl view catalog") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("Archive Content Title");
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("lbl view catalog archive") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("view content title");
        }
        result.Append("<table><tr>" + "\r\n");
        if ((_PermissionData.CanAdd && bViewContent) || _PermissionData.IsReadOnly == true)
        {
            if (_PermissionData.CanAdd && bViewContent)
            {
                if (!bSelectedFound)
                {
                    _ContentType = System.Convert.ToInt32(_CMSContentType_AllTypes);
                }
            }
        }

        string buttonId;

        if ((_PermissionData.CanAdd || _PermissionData.CanAddFolders || bCommerceAdmin) && bViewContent)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + _MessageHelper.GetMessage("lbl New") + "</span></td>");
        }
        if ((_PermissionData.CanAdd) || _PermissionData.IsReadOnly || bCommerceAdmin)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + _MessageHelper.GetMessage("lbl View") + "</span></td>");
        }
        if (bViewContent && (_PermissionData.IsAdmin || bFolderAdmin || bCommerceAdmin) || (_PermissionData.CanDelete))
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"delete\">" + _MessageHelper.GetMessage("lbl Delete") + "</span></td>");
        }

        buttonId = Guid.NewGuid().ToString();

        result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + _MessageHelper.GetMessage("lbl Action") + "</span></td>");

        if (_EnableMultilingual == 1)
        {
            SiteAPI m_refsite = new SiteAPI();
            LanguageData[] language_data = new LanguageData[1];
            language_data = m_refsite.GetAllActiveLanguages();

        }
        List<ProductTypeData> active_prod_list = new List<ProductTypeData>();
        active_prod_list = pProductType.GetFolderProductTypeList(_FolderData.Id);

        bool smartFormsRequired = System.Convert.ToBoolean(!Utilities.IsNonFormattedContentAllowed(active_prod_list.ToArray()));
        bool canAddAssets = System.Convert.ToBoolean((_PermissionData.CanAdd || _PermissionData.CanAddFolders) && bViewContent);

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton(_PageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");

        result.Append("<script type=\"text/javascript\">" + Environment.NewLine);

        result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
        if (_PermissionData.CanAddFolders || bCommerceAdmin)
        {
            result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/folderGreen.png" + "\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl commerce catalog") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=catalog&id=" + _Id + "\' } );" + Environment.NewLine);
            result.Append("    filemenu.addBreak();" + Environment.NewLine);
        }

        if (_PermissionData.CanAdd)
        {
            if (active_prod_list.Count > 0)
            {
                int k;
                for (k = 0; k <= active_prod_list.Count - 1; k++)
                {
                    if (active_prod_list[k].Id != 0)
                    {

                        result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/");

                        if (active_prod_list[k].EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
                        {

                            result.Append("bookGreen.png");
                        }
                        else if (active_prod_list[k].EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
                        {

                            result.Append("box.png");
                        }
                        else if (active_prod_list[k].EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
                        {

                            result.Append("package.png");
                        }
                        else
                        {

                            result.Append("brick.png");
                        }

                        result.Append("\' />&nbsp;&nbsp;" + active_prod_list[k].Title + "\", function() { " + _StyleHelper.GetCatalogAddAnchorType(_Id, active_prod_list[k].Id) + " } );" + Environment.NewLine);
                    }
                }
            }
        }

        if (_PermissionData.CanAdd || _PermissionData.CanAddFolders)
        {
            result.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
        }

        result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/folderGreenView.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl All Types"), -1, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + _CMSContentType_AllTypes + "); } );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "Images/ui/icons/brick.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl commerce products"), 0, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product) + "); } );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/box.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl commerce kits"), 2, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit) + "); } );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/package.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl commerce bundles"), 3, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle) + "); } );" + Environment.NewLine);
        result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/ui/icons/bookGreen.png" + "\' />&nbsp;&nbsp;" + MakeBold(_MessageHelper.GetMessage("lbl commerce subscriptions"), 4, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes) + "\", function() { UpdateView(" + Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct) + "); } );" + Environment.NewLine);

        if (((_PermissionData.CanAdd) && bViewContent) || _PermissionData.IsReadOnly == true)
        {
            AddLanguageMenu(result);

            if (bViewContent)
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/contentArchived.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl archive entry title") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
            }
            else
            {
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + "images/UI/Icons/properties.png" + "\' />&nbsp;&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl catalog view entry") + "\", function() { window.location.href = \'content.aspx?action=" + _NextActionType + "&id=" + _Id + "&LangType=" + _ContentLanguage + ((Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(_ContentTypeSelected), false)) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : ((Ektron.Cms.Common.EkConstants.IsArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) ? ("&" + _ContentTypeUrlParam + "=" + Ektron.Cms.Common.EkConstants.MakeNonArchiveAssetContentType(Convert.ToInt64(_ContentTypeSelected))) : "")) + "\' } );" + Environment.NewLine);
            }
            if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && bViewContent) || bFolderAdmin)
            {
                result.Append("    viewmenu.addBreak();" + Environment.NewLine);
                result.Append("    viewmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/properties.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl catalog Properties") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=ViewFolder&id=" + _Id + "\' } );" + Environment.NewLine);
            }
            result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);
            result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
            if ((_PermissionData.CanDeleteFolders || bCommerceAdmin) && bViewContent && _Id != 0)
            {
                result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/folderGreenDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl this catalog") + "\", function() { if( ConfirmFolderDelete(" + _Id + ") ) { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId + "\'; }} );" + Environment.NewLine);
            }
            if ((entryCount > 0) && (bViewContent && (_PermissionData.IsAdmin || bFolderAdmin) || _PermissionData.CanDelete))
            {
                if (Convert.ToString(_EnableMultilingual) == "1" && _ContentLanguage < 1)
                {
                    result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/brickDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl catalog del entry") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
                }
                else
                {
                    //44595 -  Delete content from the archive view should show up archived list rather than live content list.
                    if (_PageAction == "viewarchivecontentbycategory")
                    {
                        result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/brickDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl catalog del entry") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "&showarchive=true\'; } );" + Environment.NewLine);
                    }
                    else
                    {
                        result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/brickDelete.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl catalog del entry") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id + "\'; } );" + Environment.NewLine);
                    }
                }
            }
            result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
        }
        result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
        result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/magnifier.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("generic Search") + "\", function() { window.location.href = \'productsearch.aspx?LangType=" + _ContentLanguage + "&action=showdlg&folderid=" + _Id + "\'; } );" + Environment.NewLine);

        result.Append("    actionmenu.addBreak();" + Environment.NewLine);

        if (_CheckedInOrApproved && bViewContent && (_PermissionData.IsAdmin || IsFolderAdmin() || IsCopyOrMoveAdmin()) && (_PermissionData.CanAdd || _PermissionData.CanEdit))
        {
            if ((Convert.ToString(_EnableMultilingual) == "1") && (_ContentLanguage < 1))
            {
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/cut.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl cut") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentCopy.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl copy") + "\", function() { alert(\'A language must be selected!\'); } );" + Environment.NewLine);
            }
            else
            {
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/cut.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl cut") + "\", function() { setClipBoard(); } );" + Environment.NewLine);
                result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/UI/Icons/contentCopy.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl copy") + "\", function() { setCopyClipBoard(); }) ;" + Environment.NewLine);
            }
        }

        result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);
        result.Append("" + Environment.NewLine);
        htmToolBar.InnerHtml = result.ToString();
    }

    private string GetSyncMenuOption()
    {
        SiteAPI site = new SiteAPI();
        EkSite ekSiteRef = site.EkSiteRef;
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        if ((LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.eSync)) && (_ContentApi.RequestInformationRef.IsSyncEnabled))
        {
            if (_FolderData.IsDomainFolder)
            {
                if (_FolderData.ParentId > 0 && ServerInformation.IsStaged()) //AndAlso ekSiteRef.MultiSiteFolderSyncEnabled(folder_data.Id)) Then
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/folderSync.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("btn sync folder") + "\", function() { Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(\'" + _ContentLanguage + "\',null, null, null,\'" + _Id + "\' , false, true); return false; } );" + Environment.NewLine);
                }
            }
            else
            {
                if (ServerInformation.IsStaged()) //AndAlso ekSiteRef.FolderSyncEnabled()) Then
                {
                    result.Append("    actionmenu.addItem(\"&nbsp;<img src=\'" + "images/ui/icons/folderSync.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("btn sync folder") + "\", function() { Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(\'" + _ContentLanguage + "\', null, null, null,\'" + _Id + "\' ,false, false);return false; } );" + Environment.NewLine);
                }
            }
        }
        return result.ToString();
    }

    private void Populate_ViewCatalogGrid(EkContentCol folder_data, System.Collections.Generic.List<EntryData> entryList)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;
        string langDesc = string.Empty;
        string imageDirection = string.Empty;

        if (Request.QueryString["orderbydirection"] == null)
            direction = "desc";
        else if (Request.QueryString["orderbydirection"] == "desc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadDownGrey.png\" \" />";
            direction = "asc";
        }
        else if (Request.QueryString["orderbydirection"] == "asc")
        {
            imageDirection = "&nbsp;<img src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/arrowHeadUpGrey.png\" \" />";
            direction = "desc";
        }
        strTag = "<a href=\"content.aspx?LangType=" + _ContentLanguage + "&action=" + _PageAction + "&orderbydirection=" + direction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "TITLE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "title")
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "Title" + strtag1 + _MessageHelper.GetMessage("generic title") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CONTENTTYPE";
        colBound.HeaderText = strTag + "Type" + strtag1 + _MessageHelper.GetMessage("lbl product type xml config") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "language")
            colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "language" + strtag1 + _MessageHelper.GetMessage("generic language") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "id")
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "ID" + strtag1 + _MessageHelper.GetMessage("generic ID") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATUS";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "status")
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "status" + strtag1 + _MessageHelper.GetMessage("generic Status") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = strTag + "entrytype" + strtag1 + _MessageHelper.GetMessage("lbl product type class") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header  center";
        colBound.ItemStyle.CssClass = "center";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "SALEPRICE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "sale")
            colBound.HeaderText = strTag + "sale" + strtag1 + _MessageHelper.GetMessage("lbl sale price") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "sale" + strtag1 + _MessageHelper.GetMessage("lbl sale price") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "right";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LISTPRICE";
        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]) && Request.QueryString["orderby"].ToString().ToLower() == "list")
            colBound.HeaderText = strTag + "list" + strtag1 + _MessageHelper.GetMessage("lbl list price") + imageDirection + "</a>";
        else
            colBound.HeaderText = strTag + "list" + strtag1 + _MessageHelper.GetMessage("lbl list price") + "</a>";
        
        colBound.HeaderStyle.CssClass = "title-header center";
        colBound.ItemStyle.CssClass = "right";
        FolderDataGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("CONTENTTYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(long)));
        dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("SALEPRICE", typeof(string)));
        dt.Columns.Add(new DataColumn("LISTPRICE", typeof(string)));
        string ViewUrl = "";
        int i;

        for (i = 0; i <= (entryList.Count - 1); i++)
        {
            dr = dt.NewRow();
            if (entryList[i].ContentStatus == "A")
            {
                ViewUrl = (string)("content.aspx?action=View&folder_id=" + _Id + "&id=" + entryList[i].Id + "&LangType=" + entryList[i].LanguageId + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
            }
            else
            {
                ViewUrl = (string)("content.aspx?action=ViewStaged&folder_id=" + _Id + "&id=" + entryList[i].Id + "&LangType=" + entryList[i].LanguageId + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"]));
            }
            string dmsMenuGuid;
            dmsMenuGuid = (string)(System.Guid.NewGuid().ToString());
            string makeUnique = (string)(entryList[i].Id + entryList[i].LanguageId + dmsMenuGuid);
            long contentType = Convert.ToInt64(EkEnumeration.CMSContentType.CatalogEntry);

            dr[0] = "<div class=\"dmsWrapper\"";
            dr[0] = dr[0] + " id=\"dmsWrapper" + makeUnique + "\">";
            dr[0] = dr[0] + "<p class=\"dmsItemWrapper\"";
            dr[0] = dr[0] + " id=\"dmsItemWrapper" + makeUnique + "\"";
            dr[0] = dr[0] + " title=\"View Menu\"";
            dr[0] = dr[0] + " style=\"overflow:visible;\"";
            dr[0] = dr[0] + ">";
            dr[0] = dr[0] + "<input type=\"hidden\" value=\'{\"id\":" + entryList[i].Id + ",";
            dr[0] = dr[0] + "\"parentId\":" + entryList[i].FolderId + ",";
            dr[0] = dr[0] + "\"languageId\":" + entryList[i].LanguageId + ",";
            dr[0] = dr[0] + "\"status\":\"" + entryList[i].ContentStatus + "\",";
            dr[0] = dr[0] + "\"guid\":\"" + dmsMenuGuid + "\",";
            dr[0] = dr[0] + "\"communityDocumentsMenu\": \"\",";
            dr[0] = dr[0] + "\"contentType\":" + contentType + ",";
            dr[0] = dr[0] + "\"dmsSubtype\":\"\"}\'";
            dr[0] = dr[0] + " id=\"dmsContentInfo" + makeUnique + "\" />";
            if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {
                dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppPath + "images/ui/icons/bookGreen.png" + "\" onclick=\"event.cancelBubble=true;\" />";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product)
            {
                dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppPath + "Images/ui/icons/brick.png" + "\" onclick=\"event.cancelBubble=true;\" />";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
            {
                dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppPath + "Images/ui/icons/bricks.png" + "\" onclick=\"event.cancelBubble=true;\" />";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
            {
                dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppPath + "Images/ui/icons/box.png" + "\" onclick=\"event.cancelBubble=true;\" />";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
            {
                dr[0] = dr[0] + "<img src=\"" + _ContentApi.AppPath + "Images/ui/icons/package.png\" onclick=\"event.cancelBubble=true;\" />";
            }
            dr[0] = dr[0] + "<a";
            dr[0] = dr[0] + " id=\"dmsViewItemAnchor" + makeUnique + "\"";
            dr[0] = dr[0] + " class=\"dmsViewItemAnchor\"";
            dr[0] = dr[0] + " onclick=\"event.cancelBubble=true;\"";
            dr[0] = dr[0] + " href=\"" + ViewUrl + "\"";
            dr[0] = dr[0] + " title=\"View " + entryList[i].Title + "\"";
            dr[0] = dr[0] + ">" + entryList[i].Title;
            dr[0] = dr[0] + "</a>";
            dr[0] = dr[0] + "</p>";
            dr[0] = dr[0] + "</div>";

            Ektron.Cms.Framework.Localization.LocaleManager _locApi = new Ektron.Cms.Framework.Localization.LocaleManager();
            LocaleData langData = _locApi.GetItem(System.Convert.ToInt32(entryList[i].LanguageId));
            if (langData != null)
            {
                langDesc = langData.EnglishName;
            }

            dr[1] = GetContentTypeText(contentType, entryList[i].ProductType.Id, 0, "");
            dr[2] = "<a href=\"#Language\" onclick=\"return false;\" onmouseover=\"ddrivetip(\'" + langDesc.ToString() + "\',\'ADC5EF\', 100);\" onmouseout=\"hideddrivetip()\" style=\"text-decoration:none;\">" + "<img src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(entryList[i].LanguageId)) + "\' alt=\"Flag\" />" + "</a>";
            dr[3] = entryList[i].Id;
            dr[4] = _StyleHelper.StatusWithToolTip((string)(entryList[i].ContentStatus));
            dr[5] = entryList[i].EntryType.ToString();
            dr[6] = Ektron.Cms.Common.EkFunctions.FormatCurrency(System.Convert.ToDecimal(entryList[i].SalePrice), _ContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
            dr[7] = Ektron.Cms.Common.EkFunctions.FormatCurrency(System.Convert.ToDecimal(entryList[i].ListPrice), _ContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }

    #endregion

    #region DiscussionBoard/forum/topic/replies

    private void ViewDiscussionBoardToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        bool bSelectedFound = false;
        bool bShowViewMenu = false;
        if (_PageAction == "viewcontentbycategory")
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view contents of dboard msg") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("Archive Content Title");
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive content title") + " \"" + _FolderData.Name + "\"")) + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) + "\' />";
            altText = _MessageHelper.GetMessage("view content title");
        }
        result.Append("<table><tr>" + "\r\n");
        if (((_PermissionData.CanAdd) && (_PageAction == "viewcontentbycategory")) || _PermissionData.IsReadOnly == true)
        {
            if ((_PermissionData.CanAdd) && (_PageAction == "viewcontentbycategory"))
            {
                if (!bSelectedFound)
                {
                    _ContentType = System.Convert.ToInt32(_CMSContentType_AllTypes);
                }
            }
        }

        string buttonId;

        if (_PermissionData.CanAddFolders && _PageAction == "viewcontentbycategory")
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + _MessageHelper.GetMessage("lbl New") + "</span></td>");
            result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
            result.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
            result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/UI/Icons/folderBoard.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl add disc forum") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=AddSubFolder&type=discussionforum&id=" + _Id + "\' } );" + Environment.NewLine);
            result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/UI/Icons/users.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl discussionforumsubject") + "\", function() { window.location.href = \'threadeddisc/addeditboard.aspx?LangType=" + _ContentApi.ContentLanguage + "&action=addcat&id=" + _Id.ToString() + "\' } );" + Environment.NewLine);
            if ((_ContentApi.IsAdmin() == true) || IsFolderAdmin())
            {
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/restrictedIps.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl restricted ip") + "\", function() { window.location.href = \'threadeddisc/restrictIP.aspx?action=edit&fromboard=true&boardid=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/replaceWord.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl replace word") + "\", function() { window.location.href = \'threadeddisc/replacewords.aspx?action=edit&fromboard=true&boardid=" + _Id + "\' } );" + Environment.NewLine);
                result.Append("    filemenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/userRanks.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl user rank") + "\", function() { window.location.href = \'threadeddisc/userranks.aspx?action=edit&fromboard=true&boardid=" + _Id + "\' } );" + Environment.NewLine);
            }
            result.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
            result.Append("    </script>" + Environment.NewLine);
        }

        //The properties button should be far right.
        result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
        result.Append("    var viewmenu = new Menu( \"view\" );" + Environment.NewLine);
        if ((_ContentApi.IsAdmin() == true) || IsFolderAdmin())
        {
            bShowViewMenu = true;
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/permissions.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl permissions") + "\", function() { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&type=folder&id=" + _Id + "\' } );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/restrictedIPs.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl restricted ips") + "\", function() { window.location.href = \'threadeddisc/restrictIP.aspx?boardid=" + _Id + "\' } );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/replaceWord.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl replace words") + "\", function() { window.location.href = \'threadeddisc/replacewords.aspx?boardid=" + _Id + "\' } );" + Environment.NewLine);
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/userRanks.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl user ranks") + "\", function() { window.location.href = \'threadeddisc/userranks.aspx?boardid=" + _Id + "\' } );" + Environment.NewLine);
            if (_ContentApi.IsAdmin() == true)
                result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/notification.png" + " \' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl notifications") + "\", function() { window.location.href = \'subscriptionmessages.aspx?mode=forum&fromboard=true&boardid=" + _Id + "\' } );" + Environment.NewLine);
        }
        if (_ContentApi.IsAdmin() == true || IsFolderAdmin())
        {
            result.Append("    viewmenu.addItem(\"&nbsp;<img valign=\'middle\' src=\'" + _ContentApi.AppPath + "images/ui/Icons/properties.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("properties text") + "\", function() { window.location.href = \'threadeddisc/addeditboard.aspx?LangType=" + _ContentLanguage + "&action=View&id=" + _Id.ToString() + "\' } );" + Environment.NewLine);
            bShowViewMenu = true;
        }
        if (bShowViewMenu == true)
        {
            result.Append("    MenuUtil.add( viewmenu );" + Environment.NewLine);
        }
        result.Append("    var deletemenu = new Menu( \"delete\" );" + Environment.NewLine);
        if (_PermissionData.CanDeleteFolders && _PageAction == "viewcontentbycategory" && _Id != 0)
        {
            result.Append("    deletemenu.addItem(\"&nbsp;<img src=\'" + _ContentApi.AppPath + "images/ui/Icons/folderBoardDelete.png\' />&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl This Folder") + "\", function() { if( ConfirmFolderDelete(" + _Id + ") ) { window.location.href = \'content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId + "\'; }} );" + Environment.NewLine);
        }
        result.Append("    MenuUtil.add( deletemenu );" + Environment.NewLine);
        result.Append("    </script>" + Environment.NewLine);

        if (bShowViewMenu == true)
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'view\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"folderView\">" + _MessageHelper.GetMessage("lbl View") + "</span></td>");
        }
        if ((_PageAction == "viewcontentbycategory") && (_PermissionData.IsAdmin || IsFolderAdmin()))
        {
            buttonId = Guid.NewGuid().ToString();

            result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'delete\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"delete\">" + _MessageHelper.GetMessage("lbl Delete") + "</span></td>");
        }

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Populate_ViewDiscussionBoardGrid()
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        DiscussionCategory[] adcCategories;
        DataTable dt = new DataTable();
        DataRow dr;

        pnlThreadedDiscussions.Visible = true;
        adcCategories = _EkContent.GetCategoriesforBoard(_Id);
        _DiscussionForums = _EkContent.GetForumsforBoard(_Id);

        dt.Columns.Add(new DataColumn("name", typeof(string)));
        dt.Columns.Add(new DataColumn("id", typeof(long)));

        if (!(adcCategories == null) && (adcCategories.Length > 0))
        {
            for (int j = 0; j <= (adcCategories.Length - 1); j++)
            {
                dr = dt.NewRow();
                dr[0] = adcCategories[j].Name;
                dr[1] = adcCategories[j].CategoryID;
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        CategoryList.DataSource = dv;
        CategoryList.DataBind();
    }

    private void ViewDiscussionForumToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        int count = 0;
        bool bSelectedFound = false;

        if (_PageAction == "viewcontentbycategory")
        {
            altText = _MessageHelper.GetMessage("Archive forum Title");
        }
        else
        {
            altText = _MessageHelper.GetMessage("view forum title");
        }
        if (_PageAction == "viewcontentbycategory")
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view content of forum msg") + " \"" + _FolderData.Name + "\""));
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive forum title") + " \"" + _FolderData.Name + "\""));
        }
        result.Append("<table><tr>" + "\r\n");

        if (_From == "dashboard")
        {
            //result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "dashboard.aspx", _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        else if (Request.QueryString["ContType"] == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString())
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id), _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }

        bool primaryCssApplied = false;

        if (((_PermissionData.CanAdd) && (_PageAction == "viewcontentbycategory")) || _PermissionData.IsReadOnly == true)
        {
            if ((_PermissionData.CanAdd) && (_PageAction == "viewcontentbycategory"))
            {
                if (!bSelectedFound)
                {
                    _ContentType = System.Convert.ToInt32(_CMSContentType_AllTypes);
                }
                // Don't allow user to add content if IsMac and XML-Config assigned to this folder:
                if ((!(_IsMac && _HasXmlConfig)) || ("ContentDesigner" == _SelectedEditControl))
                {
                    if (Request.QueryString["ContType"] != Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments.ToString())
                    {
                        if (Convert.ToString(_EnableMultilingual) == "1" && _ContentLanguage < 1)
                        {
                            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/add.png", "javascript:AddNewTopic();", _MessageHelper.GetMessage("add topic msg"), _MessageHelper.GetMessage("btn add forumpost"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));
                        }
                        else
                        {
                            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/add.png", (string)("threadeddisc/addedittopic.aspx?action=add&id=" + _Id.ToString()), _MessageHelper.GetMessage("add topic msg"), _MessageHelper.GetMessage("btn add forumpost"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));
                        }

                        primaryCssApplied = true;
                    }
                }
            }
        }
        if (_PermissionData.IsAdmin && _TakeAction && _PageAction == "viewcontentbycategory")
        {
            if (Convert.ToString(_EnableMultilingual) == "1" && _ContentLanguage < 1)
            {
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/delete.png", "javascript:alert(\'A language must be selected!\');", _MessageHelper.GetMessage("alt btn deletetopics"), _MessageHelper.GetMessage("btn deletetopics"), "", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));
            }
            else
            {
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/delete.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=DeleteContentByCategory&id=" + _Id), _MessageHelper.GetMessage("alt btn deletetopics"), _MessageHelper.GetMessage("btn deletetopics"), "", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));
            }

            primaryCssApplied = true;
        }
        //The properties button should be far right.
        if (((_PermissionData.CanEditFolders || _PermissionData.CanEditApprovals) && _PageAction == "viewcontentbycategory") || IsFolderAdmin())
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/properties.png", (string)("threadeddisc/addeditforum.aspx?LangType=" + _ContentLanguage + "&action=View&id=" + _Id), _MessageHelper.GetMessage("alt forum properties button text"), _MessageHelper.GetMessage("btn properties"), "", StyleHelper.ViewPropertiesButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (_PermissionData.IsAdmin || _ContentApi.IsARoleMemberForFolder(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers), _Id, _CurrentUserId, false))
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/permissions.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=ViewPermissions&type=folder&id=" + _Id), _MessageHelper.GetMessage("alt permissions button text forum (view)"), _MessageHelper.GetMessage("btn view permissions"), "", StyleHelper.ViewPermissionsButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (((_PermissionData.IsAdmin) || (_PermissionData.CanDeleteFolders)) && _PageAction == "viewcontentbycategory" && _Id != 0)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/folderBoardDelete.png", (string)("content.aspx?LangType=" + _ContentLanguage + "&action=DoDeleteFolder&id=" + _Id + "&ParentID=" + ParentId), _MessageHelper.GetMessage("alt delete forum button text"), _MessageHelper.GetMessage("btn delete forum"), "onclick=\"return ConfirmFolderDelete(" + _Id + ");\" ", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (_EnableMultilingual == 1)
        {
            SiteAPI m_refsite = new SiteAPI();
            LanguageData[] language_data = m_refsite.GetAllActiveLanguages();

            result.Append("<td class=\"label\">&nbsp;|&nbsp;" + _MessageHelper.GetMessage("lbl View") + ": ");
            result.Append("<select id=\"selLang\" name=\"selLang\" OnChange=\"LoadLanguage(this.options[this.selectedIndex].value);\">");
            if (_ContentLanguage == -1)
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + " selected>All</option>");
            }
            else
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + ">All</option>");
            }
            for (count = 0; count <= language_data.Length - 1; count++)
            {
                if (Convert.ToString((short)_ContentLanguage) == Convert.ToString(language_data[count].Id))
                {
                    result.Append("<option value=" + language_data[count].Id + " selected>" + language_data[count].Name + "</option>");
                }
                else
                {
                    result.Append("<option value=" + language_data[count].Id + ">" + language_data[count].Name + "</option>");
                }
            }
            result.Append("</select></td>");
        }

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Populate_ViewForumPostsByCategoryGrid(DiscussionTopic[] topics, ArrayList commenttally)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        UserAPI objUserAPI = new UserAPI();
        UserData objUserData = new UserData();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "APPROVAL";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(12);
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TOPIC";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.CssClass = "left";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Topic");
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STARTER";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(80);
        colBound.HeaderText = _MessageHelper.GetMessage("topicstarter text");
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "REPLIES";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = _MessageHelper.GetMessage("lbl replies");
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VIEWS";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = _MessageHelper.GetMessage("views lbl");
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LASTPOST";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(200);
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Last Reply");
        FolderDataGrid.Columns.Add(colBound);

        FolderDataGrid.ShowHeader = true;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("APPROVAL", typeof(string)));
        dt.Columns.Add(new DataColumn("TOPIC", typeof(string)));
        dt.Columns.Add(new DataColumn("STARTER", typeof(string)));
        dt.Columns.Add(new DataColumn("REPLIES", typeof(string)));
        dt.Columns.Add(new DataColumn("VIEWS", typeof(string)));
        dt.Columns.Add(new DataColumn("LASTPOST", typeof(string)));

        string ViewUrl = "";
        string EditUrl = "";
        int i;
        bool bNewComment = false;
        int iReplyTally = 0;
        DateTime dtLastPosted = DateTime.Now;

        for (i = 0; i <= (topics.Length - 1); i++)
        {
            if (topics[i].LanguageId == _ContentLanguage | _ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
            {
                dr = dt.NewRow();
                //reset these values
                iReplyTally = 0;
                bNewComment = false;


                foreach (ArrayList ctally in commenttally)
                {
                    if (Convert.ToInt64(ctally[0]) == topics[i].Id)
                    {
                        iReplyTally = System.Convert.ToInt32(ctally[1]);
                        bNewComment = System.Convert.ToBoolean(System.Convert.ToInt32(ctally[2]) < (iReplyTally * 7));
                        if (iReplyTally > 0)
                        {
                            dtLastPosted = Convert.ToDateTime(ctally[3]);
                        }
                        commenttally.Remove(ctally); // remove so we don't need to go through this again
                        break;
                    }
                }


                iReplyTally = topics[i].Replies;
                if (iReplyTally > 0)
                {
                    dtLastPosted = topics[i].LastPostedDate;
                }
                if ((bNewComment || topics[i].Status.ToUpper() == "I") && (_PermissionData.IsAdmin || _PermissionData.CanAddToImageLib))
                {
                    dr[0] = "<img src=\"images/UI/Icons/approvalApproveItem.png\" alt=\"" + this._MessageHelper.GetMessage("lbl approval needed") + "\" title=\"" + this._MessageHelper.GetMessage("lbl approval needed") + "\" />";
                }
                else
                {
                    dr[0] = "";
                }
                switch (topics[i].Priority)
                {
                    case Ektron.Cms.Common.EkEnumeration.DiscussionObjPriority.Announcement:
                        dr[1] = "<img title=\"Announcement\" src=\"" + _ContentApi.AppPath + "images/ui/icons/asteriskRed.png\" style=\"margin-right:.25em; vertical-align: middle\" />";
                        break;
                    case Ektron.Cms.Common.EkEnumeration.DiscussionObjPriority.Sticky:
                        dr[1] = "<img title=\"Sticky Topic\" src=\"" + _ContentApi.AppPath + "images/ui/icons/asteriskYellow.png\" style=\"margin-right:.25em; vertical-align: middle\" />";
                        break;
                    default: // DiscussionObjPriority.Normal
                        dr[1] = "<img title=\"Topic\" src=\"" + _ContentApi.AppPath + "images/ui/icons/asteriskOrange.png\" style=\"margin-right:.25em; vertical-align: middle\" />";
                        break;
                }

                ViewUrl = (string)("content.aspx?id=" + _Id + "&action=ViewContentByCategory&LangType=" + topics[i].LanguageId + "&ContType=" + Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments + "&contentid=" + topics[i].Id.ToString()); //view posts
                EditUrl = (string)("content.aspx?action=View&folder_id=" + _Id + "&id=" + topics[i].Id + "&LangType=" + topics[i].LanguageId + "&callerpage=content.aspx&origurl=" + EkFunctions.UrlEncode(Request.ServerVariables["QUERY_STRING"])); //more traditional content view
                dr[1] += "<a href=\"" + ViewUrl + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace((string)(topics[i].Title + "\""), "\'", "`", 1, -1, 0) + "\'" + ">" + topics[i].Title + " </a>";

                objUserData = objUserAPI.GetActiveUserById(topics[i].UserId, false);
                if ((objUserData != null) && (objUserData.Username != ""))
                {
                    dr[2] = objUserData.Username;
                }
                else
                {
                    dr[2] = topics[i].UserId;
                }

                //replies col
                dr[3] = "<a href=\"" + ViewUrl + "\">" + iReplyTally + "</a>";
                //status col
                dr[4] = topics[i].Views;
                //last post col
                if (iReplyTally > 0)
                {
                    if (dtLastPosted.Date.Equals(DateTime.Now.Date))
                    {
                        dr[5] = _MessageHelper.GetMessage("lbl today at") + " " + dtLastPosted.ToShortTimeString();
                    }
                    else if (dtLastPosted.Date.AddDays(1).Equals(DateTime.Now.Date))
                    {
                        dr[5] = _MessageHelper.GetMessage("lbl yesterday at") + " " + dtLastPosted.ToShortTimeString();
                    }
                    else
                    {
                        dr[5] = dtLastPosted.ToLongDateString() + " " + dtLastPosted.ToShortTimeString();
                    }
                }
                else
                {
                    dr[5] = "-";
                }
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }

    private void ViewRepliesToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string altText = "";
        long ParentId = _FolderData.ParentId;
        int count = 0;

        if (_PageAction == "viewcontentbycategory")
        {
            altText = _MessageHelper.GetMessage("Archive Content Title");
        }
        else
        {
            altText = _MessageHelper.GetMessage("view content title");
        }
        if (_PageAction == "viewcontentbycategory")
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view replies in topic msg") + " \"" + _ContentData.Title + "\""));
        }
        else
        {
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string)(_MessageHelper.GetMessage("view archive content title") + " \"" + _ContentData.Title + "\""));
        }
        result.Append("<table><tr>" + "\r\n");

        if (_From == "dashboard")
        {
            //result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "dashboard.aspx", _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }
        else if (Convert.ToInt32(Request.QueryString["ContType"]) == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/back.png", (string)("content.aspx?action=ViewContentByCategory&id=" + _Id), _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }

        bool primaryCssApplied = false;

        if ((_PermissionData.CanAddTask) && _PermissionData.IsReadOnlyLib == true)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/add.png", "threadeddisc/addeditreply.aspx?action=Add&topicid=" + _ContentId + "&forumid=" + _Id + "&id=0", _MessageHelper.GetMessage("alt btn add reply"), _MessageHelper.GetMessage("btn add reply"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (_PermissionData.CanDelete && _PageAction == "viewcontentbycategory" && _Id != 0)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/delete.png", (string)("content.aspx?LangType=" + _ContentApi.ContentLanguage + "&action=submitDelContAction&delete_id=" + _ContentId + "&page=&folder_id=" + _Id), _MessageHelper.GetMessage("alt delete topic button text"), _MessageHelper.GetMessage("btn delete topic"), " OnClick=\"return ConfirmDelete(true);return false;\" ", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (_ContentData.Status.ToUpper() == "I" && (_PermissionData.CanAddToImageLib == true || _PermissionData.IsAdmin == true))
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath + "icon_verify_good.gif", (string)("threadeddisc/addedittopic.aspx?id=" + _ContentId + "&folderid=" + _Id + "&action=approve&LangType=" + _ContentLanguage.ToString()), _MessageHelper.GetMessage("alt approve topic"), _MessageHelper.GetMessage("lbl approve topic"), "", StyleHelper.ApproveButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }
        if (_ContentData.Status.ToUpper() != "I" && _PermissionData.CanEdit)
        {
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/ui/Icons/properties.png", (string)("threadeddisc/addedittopic.aspx?id=" + _ContentId + "&action=view&LangType=" + _ContentLanguage.ToString()), _MessageHelper.GetMessage("alt properties button text"), _MessageHelper.GetMessage("btn topic properties"), "", StyleHelper.ViewPropertiesButtonCssClass, !primaryCssApplied));

            primaryCssApplied = true;
        }

        if (!(this._ContentType == Ektron.Cms.Common.EkConstants.CMSContentType_BlogComments) && (_EnableMultilingual == 1))
        {
            SiteAPI m_refsite = new SiteAPI();
            LanguageData[] language_data = m_refsite.GetAllActiveLanguages();

            result.Append("<td class=\"label\">&nbsp;|&nbsp;" + _MessageHelper.GetMessage("lbl View") + ":");
            result.Append("<select id=selLang name=selLang OnChange=\"javascript:LoadLanguage(this.options[this.selectedIndex].value);\">");
            if (_ContentLanguage == -1)
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + " selected>All</option>");
            }
            else
            {
                result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + ">All</option>");
            }
            for (count = 0; count <= language_data.Length - 1; count++)
            {
                if (Convert.ToString((short)_ContentLanguage) == Convert.ToString(language_data[count].Id))
                {
                    result.Append("<option value=" + language_data[count].Id + " selected>" + language_data[count].Name + "</option>");
                }
                else
                {
                    result.Append("<option value=" + language_data[count].Id + ">" + language_data[count].Name + "</option>");
                }
            }
            result.Append("</select></td>");
        }

        result.Append(StyleHelper.ActionBarDivider);

        result.Append("<td>");
        result.Append(_StyleHelper.GetHelpButton((string)(_StyleHelper.GetHelpAliasPrefix(_FolderData) + "topics_" + _PageAction), ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void Populate_ViewTopicRepliesGrid(EkTasks replydata)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;
        int nReplyCount;
        strTag = "<a href=\"content.aspx?LangType=" + _ContentApi.ContentLanguage + "&action=" + _PageAction + "&orderby=";
        strtag1 = "&id=" + _Id + (_ContentTypeQuerystringParam != "" ? "&" + _ContentTypeUrlParam + "=" + _ContentTypeQuerystringParam : "") + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">";

        FolderDataGrid.ShowHeader = false;

        colBound.DataField = "PREVIEW";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(145);
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLEDESCRIPTION";
        //colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        FolderDataGrid.Columns.Add(colBound);

        FolderDataGrid.BorderColor = System.Drawing.Color.White;
        FolderDataGrid.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
        FolderDataGrid.CellPadding = 6;
        FolderDataGrid.CellSpacing = 2;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("PREVIEW", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLEDESCRIPTION", typeof(string)));

        string ApproveURL = "";
        string ViewUrl = "";
        string EditUrl = "";
        string DeleteUrl = "";
        string ReplyURL = "";
        string sAppend = "";
        int i;
        nReplyCount = replydata.Count;
        Ektron.Cms.ContentAPI contapi = new Ektron.Cms.ContentAPI();

        dr = dt.NewRow();

        for (i = 1; i <= replydata.Count; i++)
        {
            if (replydata.get_Item(i) != null)
            {
                if (int.Parse(replydata.get_Item(i).State) == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.BlogCommentState.Completed) || (int.Parse(replydata.get_Item(i).State) == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.BlogCommentState.Pending) && ((!(_PermissionData == null) && (_PermissionData.IsAdmin || _PermissionData.CanAddToImageLib)) || this._ContentApi.RequestInformationRef.CallerId == replydata.get_Item(i).CreatedByUserID)))
                {
                    dr = dt.NewRow();
                    if (!string.IsNullOrEmpty(Request.QueryString["contentid"]))
                    {
                        sAppend = (string)("&forumid=" + _Id.ToString() + "&contentid=" + Request.QueryString["contentid"]);
                    }
                    else
                    {
                        sAppend = (string)("&forumid=" + _Id.ToString());
                    }
                    ViewUrl = (string)("tasks.aspx?action=ViewTask&tid=" + replydata.get_Item(i).TaskID.ToString() + "&fromViewContent=1&ty=both&LangType=" + _ContentApi.ContentLanguage);
                    EditUrl = (string)("threadeddisc/addeditreply.aspx?action=Edit&topicid=" + _ContentId.ToString() + "&forumid=" + this._Id.ToString() + "&id=" + replydata.get_Item(i).TaskID.ToString() + "&boardid=" + _BoardID.ToString());
                    if (i == 1)
                    {
                        EditUrl += "&type=topic";
                    }
                    ReplyURL = (string)("threadeddisc/addeditreply.aspx?action=Add&topicid=" + _ContentId.ToString() + "&forumid=" + this._Id.ToString() + "&id=" + replydata.get_Item(i).TaskID.ToString() + "&boardid=" + _BoardID.ToString());
                    if (int.Parse(replydata.get_Item(i).State) == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.BlogCommentState.Pending))
                    {
                        ApproveURL = (string)("tasks.aspx?action=ApproveTask&tid=" + replydata.get_Item(i).TaskID.ToString() + "&ty=both" + sAppend);
                        dr[0] = "<a name=\"reply" + replydata.get_Item(i).TaskID.ToString() + "\"></a><table border=\"0\" cellspacing=\"6\" width=\"125\"><tr><td colspan=\"2\" align=\"center\">";
                        dr[0] += "<br/><img border=\"4\" style=\"border-width: 5px; border-color: gold\" src=\"" + _ContentApi.AppImgPath + "thumb_forumpost.gif\" width=\"53\" height=\"55\"/><br/>";
                        dr[0] += "</td></tr>";
                        dr[0] += "<tr><td width=\"50%\">";
                        if (_PermissionData.IsReadOnlyLib == true)
                        {
                            dr[0] += "<a href=\"" + ReplyURL + "\">" + _MessageHelper.GetMessage("lbl reply") + "</a>";
                        }
                        else
                        {
                            dr[0] += _MessageHelper.GetMessage("lbl reply");
                        }
                        dr[0] += "</td><td width=\"50%\">";
                        if (_PermissionData.CanAddToImageLib == true)
                        {
                            dr[0] += "<a href=\"" + EditUrl + "\">" + _MessageHelper.GetMessage("generic edit title") + "</a>";
                        }
                        else
                        {
                            dr[0] += _MessageHelper.GetMessage("generic edit title");
                        }
                        dr[0] += "</td></tr>";
                        if (i > 1)
                        {
                            dr[0] += "<tr><td width=\"50%\">";
                            if (!(_PermissionData == null) && (_PermissionData.IsAdmin || _PermissionData.CanAddToImageLib))
                            {
                                dr[0] += "<a href=\"" + ApproveURL + "\">approve</a>";
                            }
                            else
                            {
                                dr[0] += "approve";
                            }
                        }
                    }
                    else
                    {
                        ApproveURL = "";
                        dr[0] = "<a name=\"reply" + replydata.get_Item(i).TaskID.ToString() + "\"><table border=\"0\" cellspacing=\"6\" width=\"125\"><tr><td colspan=\"2\" align=\"center\">";
                        dr[0] += "<br/><img src=\"" + _ContentApi.AppImgPath + "thumb_forumpost.gif\" width=\"53\" height=\"55\"/><br/>";
                        dr[0] += "</td></tr>";
                        dr[0] += "<tr><td width=\"50%\">";
                        if (_PermissionData.IsReadOnlyLib == true)
                        {
                            dr[0] += "<a href=\"" + ReplyURL + "\">" + _MessageHelper.GetMessage("lbl reply") + "</a>";
                        }
                        else
                        {
                            dr[0] += _MessageHelper.GetMessage("lbl reply");
                        }
                        dr[0] += "</td><td width=\"50%\">";
                        if ((i == 1 && _PermissionData.CanEdit) || contapi.UserId == replydata.get_Item(i).CreatedByUserID)
                        {
                            dr[0] += "<a href=\"" + EditUrl + "\">" + _MessageHelper.GetMessage("generic edit title") + "</a>";
                        }
                        else
                        {
                            dr[0] += _MessageHelper.GetMessage("generic edit title");
                        }
                        dr[0] += "</td></tr>";
                        //We do not need approve button when there's no approval for that post reply
                        //If i > 1 Then
                        //    dr(0) &= "<tr><td width=""50%"">"
                        //    dr(0) &= _MessageHelper.GetMessage("btn approve")
                        //End If
                    }
                    if (i > 2)
                    {
                        DeleteUrl = (string)("tasks.aspx?action=DeleteTask&tid=" + replydata.get_Item(i).TaskID.ToString() + "&ty=both" + sAppend);
                        dr[0] += "</td><td width=\"50%\">";
                        if (_PermissionData.IsAdmin || _PermissionData.CanAddToImageLib == true || contapi.UserId == replydata.get_Item(i).CreatedByUserID)
                        {
                            dr[0] += "<a href=\"" + DeleteUrl + "\" onclick=\"return confirm(\'" + _MessageHelper.GetMessage("msg del comment") + "\');\">" + _MessageHelper.GetMessage("generic delete title") + "</a>";
                        }
                        else
                        {
                            dr[0] += _MessageHelper.GetMessage("generic delete title");
                        }
                        dr[0] += "</td></tr>";
                    }
                    dr[0] += "</table>";
                    if (replydata.get_Item(i).CreatedByUserID == -1)
                    {
                        dr[1] += "<span id=\"ReplyDesc\" class=\"ReplyDesc\" style=\"color:gray;display:block;\">" + (replydata.get_Item(i).Description) + "</span><span style=\"color:green;display:block;\">" + _MessageHelper.GetMessage("lbl posted by") + " " + _MessageHelper.GetMessage("lbl anon") + " " + _MessageHelper.GetMessage("res_isrch_on") + " " + replydata.get_Item(i).DateCreated.ToString() + "</span>";
                    }
                    else
                    {
                        dr[1] += "<span id=\"ReplyDesc\" class=\"ReplyDesc\" style=\"color:gray;display:block;\">" + (replydata.get_Item(i).Description) + "</span><span style=\"color:green;display:block;\">" + _MessageHelper.GetMessage("lbl posted by") + " " + replydata.get_Item(i).CommentDisplayName + " " + _MessageHelper.GetMessage("res_isrch_on") + " " + replydata.get_Item(i).DateCreated.ToString() + "</span>";
                    }
                    if (!(replydata.get_Item(i).FileAttachments == null) && replydata.get_Item(i).FileAttachments.Length > 0)
                    {
                        dr[1] += "<br/>";
                        dr[1] += "<br/>";
                        string filetmp = "";
                        for (int k = 0; k <= (replydata.get_Item(i).FileAttachments.Length - 1); k++)
                        {
                            if (replydata.get_Item(i).FileAttachments[k].DoesExist == true)
                            {
                                filetmp += (string)("		<img src=\'" + this._ContentApi.AppPath + "images/ui/icons/filetypes/file.png\' /> <a href=\"" + replydata.get_Item(i).FileAttachments[k].Filepath + "\" target=\"_blank\" class=\"ekattachment\">" + replydata.get_Item(i).FileAttachments[k].Filename + "</a> <span class=\'attachinfo\'>(" + replydata.get_Item(i).FileAttachments[k].FileSize.ToString() + " bytes)</span><br/>" + Environment.NewLine);
                            }
                        }
                        if (filetmp.Length > 0) // if we have at least one attachment
                        {
                            filetmp = (string)(("		<span class=\"ekattachments\">File Attachment(s):</span><br/>" + Environment.NewLine) + filetmp + ("		<br/>" + Environment.NewLine));
                            dr[1] += filetmp;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();

        totalPages.Visible = false;
        currentPage.Visible = false;

    }

    protected void CategoryList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        Repeater rptForum = new Repeater();
        HtmlInputHidden hdntmp;
        long categoryID = 0;
        DataTable dt = new DataTable();
        DataRow dr;

        hdntmp = (HtmlInputHidden)e.Item.FindControl("hdn_categoryid");
        categoryID = Convert.ToInt64(hdntmp.Value);
        rptForum = (Repeater)e.Item.FindControl("ForumList");

        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("SortOrder", typeof(string)));
        dt.Columns.Add(new DataColumn("Description", typeof(string)));
        dt.Columns.Add(new DataColumn("Topics", typeof(int)));
        dt.Columns.Add(new DataColumn("Posts", typeof(int)));
        dt.Columns.Add(new DataColumn("LastPosted", typeof(string)));

        for (int i = 0; i <= (_DiscussionForums.Length - 1); i++)
        {
            if (_DiscussionForums[i].CategoryID == categoryID && _EkContent.IsAllowed(_DiscussionForums[i].Id, _EkContent.RequestInformation.ContentLanguage, "folder", "readonly"))
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"content.aspx?action=ViewContentByCategory&id=" + _DiscussionForums[i].Id.ToString() + "\">" + _DiscussionForums[i].Name + "</a>";
                dr[1] = "-";
                dr[2] = _DiscussionForums[i].Description;
                dr[3] = _DiscussionForums[i].NumberofTopics;
                dr[4] = _DiscussionForums[i].NumberofPosts;
                if (_DiscussionForums[i].NumberofPosts > 0)
                {
                    dr[5] = _DiscussionForums[i].LastPosted.ToLongDateString() + " " + _DiscussionForums[i].LastPosted.ToShortTimeString();
                }
                else
                {
                    dr[5] = "-";
                }
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        rptForum.DataSource = dv;
        rptForum.DataBind();
    }

    protected string Util_GetPageURL(int pageid)
    {

        return "content.aspx" + Ektron.Cms.Common.EkFunctions.GetUrl(new string[] { "currentpage" }, new string[] { "pageid" }, Request.QueryString).Replace("pageid", (string)(pageid == -1 ? "\' + pageid + \'" : pageid.ToString())).Replace("&amp;", "&").Replace("showAddEventForm=true", "showAddEventForm=false");

    }

    protected void Util_SetJs()
    {
        if (_PagingTotalPagesNumber > 1)
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function GoToPage(pageid, pagetotal) { ").Append(Environment.NewLine);
            sbJS.Append("     if (pageid <= pagetotal && pageid >= 1) { ").Append(Environment.NewLine);
            sbJS.Append("         window.location.href = \'").Append(Util_GetPageURL(-1)).Append("\'; ").Append(Environment.NewLine);
            sbJS.Append("     } else { ").Append(Environment.NewLine);
            sbJS.Append("         alert(\'").Append(string.Format(_MessageHelper.GetMessage("js: err page must be between"), _PagingTotalPagesNumber)).Append("\'); ").Append(Environment.NewLine);
            sbJS.Append("     } ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            ltr_js.Text = sbJS.ToString();
        }
    }

    #endregion

    #region Helpers

    private void GetQueryStringValues()
    {

        _PageAction = (string)(this.GetQueryStringValue("action").ToLower().Trim());
        _From = (string)(this.GetQueryStringValue("from").ToLower().Trim());
        _OrderBy = this.GetQueryStringValue("orderby");
        _TreeViewId = Request.QueryString["treeViewId"];

        string pagingCurrentPageNumber = this.GetQueryStringValue("currentpage");
        pagingCurrentPageNumber = (string)(pagingCurrentPageNumber == string.Empty ? "1" : pagingCurrentPageNumber);
        int.TryParse(pagingCurrentPageNumber, out _PagingCurrentPageNumber); //_PagingCurrentPageNumber = Convert.ToInt32(pagingCurrentPageNumber)

        string id = this.GetQueryStringValue("id");
        id = (string)((id == string.Empty || id.ToLower() == "undefined") ? "0" : id);
        _Id = Convert.ToInt64(id);

        string contentId = this.GetQueryStringValue("contentid");
        contentId = (string)(contentId == string.Empty ? "0" : contentId);
        _ContentId = Convert.ToInt64(contentId);

        _ContentTypeQuerystringParam = this.GetQueryStringValue(_ContentTypeUrlParam.ToString());
        if (_ContentTypeQuerystringParam != string.Empty)
        {
            if (Information.IsNumeric(_ContentTypeQuerystringParam))
            {
                _ContentTypeSelected = _ContentTypeUrlParam.ToString();
                _ContentTypeSelected = Convert.ToString(_ContentTypeQuerystringParam);
                _ContentApi.SetCookieValue(_ContentTypeUrlParam.ToString(), _ContentTypeQuerystringParam);
            }
            else if (_ContentTypeQuerystringParam.Length > 2 && _ContentTypeQuerystringParam.Substring(0, 3) == "14_")
            {
                _ContentTypeSelected = "14";
                _XmlConfigID = int.Parse(_ContentTypeQuerystringParam.Substring(3, _ContentTypeQuerystringParam.Length - 3));
                _ContentApi.SetCookieValue(_XmlConfigType, Convert.ToString(_XmlConfigID));
            }
        }
        else if (Ektron.Cms.CommonApi.GetEcmCookie()[_ContentTypeUrlParam] != "")
        {
            if (Information.IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()[_ContentTypeUrlParam]))
            {
                _ContentTypeSelected = Convert.ToString(Ektron.Cms.CommonApi.GetEcmCookie()[_ContentTypeUrlParam]);
            }
        }

        string contentSubTypeSelected = Request.QueryString["SubType"];
        if (contentSubTypeSelected == string.Empty)
        {
            contentSubTypeSelected = (string)(Ektron.Cms.CommonApi.GetEcmCookie()["SubType"]);
        }
        else
        {
            _ContentApi.SetCookieValue("SubType", contentSubTypeSelected);
        }

        if (contentSubTypeSelected == String.Empty)
        {
            _ContentSubTypeSelected = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes;
        }
        else
        {
            switch (Convert.ToInt32(contentSubTypeSelected))
            {
                case 1://Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData
                    // this is a Page Layout
                    _ContentSubTypeSelected = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData;
                    break;
                case 3://Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData
                    // this is a Master Page Layout
                    _ContentSubTypeSelected = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData;
                    break;
                case 2:// Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent:
                    // this is a web event, which indicates this is a Calendar Event entry
                    _ContentSubTypeSelected = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent;
                    break;
                default:
                    _ContentSubTypeSelected = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content;
                    break;
            }
        }
    }

    private string GetQueryStringValue(string key)
    {
        string returnValue = string.Empty;
        if (!(Request.QueryString[key] == null))
        {
            returnValue = Request.QueryString[key];
        }
        return returnValue;
    }

    private string MakeBold(string str, int ContentType, EkEnumeration.CMSContentSubtype SubType)
    {
        if (Convert.ToInt32(_ContentTypeSelected) == ContentType)
        {
            if (SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.AllTypes || SubType == _ContentSubTypeSelected)
            {
                return "<b>" + str + "</b>";
            }
        }
        return str;
    }

    private void SetPagingUI()
    {
        //paging ui
        divPaging.Visible = true;

        litPage.Text = _MessageHelper.GetMessage("lbl pagecontrol go to page");
        currentPage.Text = _PagingCurrentPageNumber == 0 ? "1" : (_PagingCurrentPageNumber.ToString());
        currentPage.ToolTip = currentPage.Text;
        litOf.Text = _MessageHelper.GetMessage("lbl pagecontrol of");
        totalPages.Text = _PagingTotalPagesNumber.ToString();

        ibFirstPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadFirst.png";
        ibFirstPage.AlternateText = _MessageHelper.GetMessage("lbl first page");
        ibFirstPage.ToolTip = ibFirstPage.AlternateText;

        if (_PagingCurrentPageNumber == 1)
        {
            ibFirstPage.Enabled = false;
        }
        else
        {
            ibFirstPage.Enabled = true;
        }

        ibPreviousPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadLeft.png";
        ibPreviousPage.AlternateText = _MessageHelper.GetMessage("lbl previous page");
        ibPreviousPage.ToolTip = ibPreviousPage.AlternateText;

        if (_PagingCurrentPageNumber == 1)
        {
            ibPreviousPage.Enabled = false;
        }
        else
        {
            ibPreviousPage.Enabled = true;
        }

        ibNextPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadRight.png";
        ibNextPage.AlternateText = _MessageHelper.GetMessage("lbl next page");
        ibNextPage.ToolTip = ibNextPage.AlternateText;

        if (_PagingCurrentPageNumber == _PagingTotalPagesNumber)
        {
            ibNextPage.Enabled = false;
        }
        else
        {
            ibNextPage.Enabled = true;
        }

        ibLastPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadLast.png";
        ibLastPage.AlternateText = _MessageHelper.GetMessage("lbl last page");
        ibLastPage.ToolTip = ibLastPage.AlternateText;

        if (_PagingCurrentPageNumber == _PagingTotalPagesNumber)
        {
            ibLastPage.Enabled = false;
        }
        else
        {
            ibLastPage.Enabled = true;
        }

        lbPageGo.Text = _MessageHelper.GetMessage("lbl goto");
        lbPageGo.ToolTip = _MessageHelper.GetMessage("lbl pagecontrol go to page");
        lbPageGo.OnClientClick = "GoToPage(document.getElementById(\'" + this.currentPage.ClientID + "\').value, " + _PagingTotalPagesNumber.ToString() + ");return false;";

    }

    public bool ViewTaxonomyContentByCategory()
    {
        return true;
    }

    public bool ViewCollectionContentByCategory()
    {
        Ektron.Cms.UI.CommonUI.ApplicationAPI api = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
        long id = Convert.ToInt64(Request.QueryString["id"]);
        return true;
    }

    #endregion

    #region JS, CSS

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "/java/ektron.workarea.contextmenus.js", "EktronWorkareaFolderContextMenusJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "/java/ektron.workarea.contextmenus.trees.js", "EktronWorkareaFolderContextMenusTreesJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.AppPath + "/java/ektron.workarea.contextmenus.cutcopy.js", "EktronWorkareaFolderContextMenusCutCopyJS");
    }

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
    }

    #endregion

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        try
        {
            if ((this._FolderData != null) && this._FolderData.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Calendar))
            {
                if (_PageData.Contains("ContentType")) _PageData.Remove("ContentType");
                if (_PageData.Contains("ContentSubType")) _PageData.Remove("ContentSubType");
                if (_PageAction == "viewarchivecontentbycategory")
                {
                    _EkContentCol = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, ref _PagingTotalPagesNumber);
                    _NextActionType = "ViewContentByCategory";
                }
                else if (_PageAction == "viewcontentbycategory")
                {
                    _EkContentCol = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, ref _PagingTotalPagesNumber);
                    _NextActionType = "ViewArchiveContentByCategory";
                }

                //paging goes here

                Populate_ViewCalendar(_EkContentCol);
                Util_SetJs();
                if (_PagingTotalPagesNumber > 1)
                {
                    this.SetPagingUI();
                }
                else
                {
                    divPaging.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + _ContentLanguage), false);
        }
    }

}


