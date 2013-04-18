using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Ektron.Cms;
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;

public partial class library : System.Web.UI.Page
{


    #region Members

    protected string _VerfiyTrue = "";
    protected string _VerfiyFalse = "";
    protected string _AppImgPath = "";
    protected string _AppPath = "";
    protected string _AppeWebPath = "";
    protected EkMessageHelper _MessageHelper;
    protected CommonApi _CommonApi = new Ektron.Cms.CommonApi();
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected string _Direction = "";
    protected string _PageAction = "";
    protected int _ContentLanguage = 0;
    protected long _FolderId = -1;
    protected string _OrderBy = "";
    protected string _Type = "";
    protected ContentAPI _ContentApi;
    protected SiteAPI _SiteApi;
    protected string _FormAction = "library.aspx";
    protected string _SitePath = "";
    protected long _Id = -1;
    protected string _Operation = "";
    protected bool _IsMac = false;
    protected bool _AjaxTree = false;
    protected int _CurrentPageNumber = 1;
    protected int _TotalPagesNumber = 1;
    protected bool _IsAdmin = false;
    protected string _SelectedTaxonomyList = "";

    //these cannot be refactored to use naming conventions - used by inc file
    protected bool TaxonomyRoleExists = false;
    protected string TaxonomyTreeIdList = "";
    protected string TaxonomyTreeParentIdList = "";
    protected long TaxonomySelectId = 0;
    protected long TaxonomyOverrideId = 0;
    protected long m_intTaxFolderId = 0;
    protected EkMessageHelper m_refMsg;

    private PermissionData _PermissionData;
    private Collection _PageData = null;
    private string _ContentTeaser = "";
    private Ektron.Cms.Content.EkContent _EkContent;
    private long _CurrentUserID = 0;
    private string _SelectedDivStyleClass = "selected_editor";
    private string _UnSelectedDivStyleClass = "unselected_editor";
    private bool _IsIE = false;
    private ContentAPI m_refcontentapi = new ContentAPI();
     

    #endregion

    #region Events

    private void Page_Init(System.Object sender, System.EventArgs e)
    {

        _ContentApi = new ContentAPI();
        _SiteApi = new SiteAPI();
        _MessageHelper = _ContentApi.EkMsgRef;
        m_refMsg = _MessageHelper;
        _AppImgPath = _ContentApi.AppImgPath;
        _AppPath = _ContentApi.AppPath;
        _SitePath = _ContentApi.SitePath;
        _AppeWebPath = _ContentApi.ApplicationPath + _ContentApi.AppeWebPath;
        RegisterResources();

    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (_ContentApi.RequestInformationRef.IsMembershipUser == 1 || _ContentApi.RequestInformationRef.UserId == 0) {
                Response.Redirect(_AppPath + "reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms user"), false);
                return;
            }

            if (_ContentApi.TreeModel == 1)
            {
                _AjaxTree = true;
            }
            if (Request.Browser.Type.IndexOf("IE") != -1)
            {
                _IsIE = true;
            }
            if (Request.Browser.Platform.IndexOf("Win") == -1)
            {
                _IsMac = true;
            }
            jsCategoryrequired.Text = "false";

            if (!string.IsNullOrEmpty (Request.QueryString["action"] ))
            {
                _PageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
                ValidateParam(_PageAction);
            }
           
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    _Id = Convert.ToInt64(Request.QueryString["id"]);
                }
           
              if (!string.IsNullOrEmpty(Request.QueryString["folder"]))
                {
                    _FolderId = Convert.ToInt64(Request.QueryString["folder"]);
                }
            
           
                if (!string.IsNullOrEmpty(Request.QueryString["type"]))
                {
                    _Type = Request.QueryString["type"];
                    ValidateParam(_Type);
                    Session["LibCategory"] = _Type;
                }
                else
                {
                    if (Session["LibCategory"] == null)
                    {
                        _Type = "images";
                        Session["LibCategory"] = _Type;
                    }
                    else
                    {
                        _Type = Convert.ToString(Session["LibCategory"]);
                    }
                }
          
            if (!string.IsNullOrEmpty(Request.QueryString["orderby"]))
            {
                _OrderBy = Request.QueryString["orderby"];
                ValidateParam(_OrderBy);
            }
            
                if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
                {
                    _ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    if (1 == _ContentLanguage)
                    {
                        _ContentLanguage = _ContentApi.DefaultContentLanguage;
                        _ContentApi.ContentLanguage = _ContentLanguage;
                    }
                    _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage.ToString());
                }
                else
                {
                    if (_ContentApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"));
                        if (1 == _ContentLanguage)
                        {
                            _ContentLanguage = _ContentApi.DefaultContentLanguage;
                            _ContentApi.ContentLanguage = _ContentLanguage;
                        }
                    }
                }
           
            if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                _ContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                _ContentApi.ContentLanguage = _ContentLanguage;
            }
            _VerfiyTrue = "<img src=\"" + _AppPath + "images/UI/Icons/check.png\" alt=\"" + _MessageHelper.GetMessage("alt green check button text (lbpath)") + "\" title=\"" + _MessageHelper.GetMessage("alt green check button text (lbpath)") + "\">";
            _VerfiyFalse = "<img src=\"" + _AppImgPath + "icon_redx.gif\" alt=\"" + _MessageHelper.GetMessage("alt red x button text (lbpath)") + "\" title=\"" + _MessageHelper.GetMessage("alt red x button text (lbpath)") + "\">";

            StyleSheetJS.Text = _StyleHelper.GetClientScript();

            // Note: To fix a problem with the Ephox Editors on the
            // Mac-running-Safari (assumed if "IsMac and not IsBrowserIE")
            // we need to use different styles for the DIV-tags holding
            // the editors, etc., otherwise they frequently draw themselves
            // when they should remain hidden. These values cause problems
            // with the PC/Win/IE combination, (the summary editor fails to
            // provide a client area for the user to view/edit) so they cannot
            // cannot be used everywhere, hence our use of alternate style classes:
            if (_IsMac && (!_IsIE))
            {
                _SelectedDivStyleClass = "mac_safari_selected_editor";
                _UnSelectedDivStyleClass = "mac_safari_unselected_editor";
            }
            else
            {
                _SelectedDivStyleClass = "selected_editor";
                _UnSelectedDivStyleClass = "unselected_editor";
            }

            // Pass class names to javascript:
            jsSelectedDivStyleClass.Text = _SelectedDivStyleClass;
            jsUnSelectedDivStyleClass.Text = _UnSelectedDivStyleClass;

            string currentFolderId = "";
            if (!string.IsNullOrEmpty(Request.QueryString["folder"] ))
            {
                currentFolderId = Request.QueryString["folder"];
            }
            else
            {
                currentFolderId = Request.QueryString["id"];
            }
            ValidateParam(currentFolderId);
            jsCurrentFolderId.Text = currentFolderId;
            jsIsAjaxTree.Text = _AjaxTree.ToString().ToLower();
            jsMyUrl.Text = Strings.LCase(Request.ServerVariables["http_host"]);

            sDemoEktronComDetected.Text = _MessageHelper.GetMessage("js: alert demo.ektron.com detected");
            sTitleRequired.Text = _MessageHelper.GetMessage("js: alert title required (library)");
            sTaxCatReq.Text = _MessageHelper.GetMessage("js tax cat req");
            sUrlLinkReq.Text = _MessageHelper.GetMessage("js: alert url link required");
            sFilenameReq.Text = _MessageHelper.GetMessage("js: alert filename is required");
            sIdReq.Text = _MessageHelper.GetMessage("js: alert ID required (library)");
            sDeleteLibItem.Text = _MessageHelper.GetMessage("js: confirm delete lib item");
            sRemoveLibItem.Text = _MessageHelper.GetMessage("js: alert remove lib item for filesystem");
            sValidPathReq.Text = _MessageHelper.GetMessage("js: alert valid path required");
            sNoItemsSelected.Text = _MessageHelper.GetMessage("js:no items selected");
            sLibPathDeletion.Text = _MessageHelper.GetMessage("js: alert confirm lb path deletion");
            sSupplyValidImagePath.Text = _MessageHelper.GetMessage("js: alert supply valid image path");
            sSupplyValidFilePath.Text = _MessageHelper.GetMessage("js: alert supply valid file path");
            sMissingLibPathStartSlash.Text = _MessageHelper.GetMessage("js: alert confirm missing lib path start slash");

            //Setting the class names for the Category and taxonomy div's

            dvCategory.Attributes.Add("class", jsUnSelectedDivStyleClass.Text);
            dvMetadata.Attributes.Add("class", jsUnSelectedDivStyleClass.Text);
            dvSummary.Attributes.Add("class", jsUnSelectedDivStyleClass.Text);

            if (_PageAction == "viewlibrarycategory" && !Page.IsPostBack)
            {
                Display_ViewLibraryByCategory();
            }
            else if (_PageAction == "viewlibrarybycategory")
            {
                Display_ViewLibraryByCategory();
            }
            else if (_PageAction == "updateqlinktemplatebycategory")
            {
                if (!(Page.IsPostBack) || hdnIsPostBack.Value == "false")
                {
                    Display_UpdateQlinkTemplateByCategory();
                }
                else
                {
                    Process_UpdateQLinkTemplateByCategory();
                }
            }
            else if (_PageAction == "addlibraryitem")
            {
                if (!(Page.IsPostBack))
                {
                    jsDisableNav.Text = "true";
                    Display_AddLibraryItem();
                }
                else
                {
                    Process_AddLibraryItem();
                }
            }
            else if (_PageAction == "viewlibraryitem")
            {
                Display_ViewLibraryItem();
            }
            else if (_PageAction == "removelibraryitem")
            {
                Process_DeleteLibraryItem();
            }
            else if (_PageAction == "deletelibraryitem")
            {
                if (!(Page.IsPostBack))
                {
                    Display_DeleteLibraryItem();
                }
                else
                {
                    if (Request.Form["remove"] != "")
                    {
                        Process_DeleteRemoveItem();
                    }
                    else
                    {
                        Process_DeleteLibraryItem();
                    }
                }
            }
            else if (_PageAction == "editlibraryitem")
            {
                if (!(Page.IsPostBack))
                {
                    jsDisableNav.Text = "true";
                    Display_EditLibraryItem();
                }
                else
                {
                    Process_UpdateLibraryItem();
                }
            }
            else if (_PageAction == "viewlibrarysettings")
            {
                Display_ViewLibrarySettings();
            }
            else if (_PageAction == "viewloadbalance")
            {
                Display_ViewLoadBalance();
            }
            else if (_PageAction == "addloadbalance")
            {
                if (!(Page.IsPostBack))
                {
                    Display_AddLoadBalance();
                }
                else
                {
                    Process_AddOrUpdateLoadBalanceSettings();
                }
            }
            else if (_PageAction == "editloadbalancesettings")
            {
                if (!(Page.IsPostBack))
                {
                    Display_EditLoadBalanceSettings();
                }
                else
                {
                    Process_AddOrUpdateLoadBalanceSettings();
                }

            }
            else if (_PageAction == "removeloadbalance")
            {
                if (!(Page.IsPostBack))
                {
                    Display_RemoveLoadBalance();
                }
                else
                {
                    Process_RemoveLoadBalance();
                }

            }
            else if (_PageAction == "editlibrarysettings")
            {
                if (!(Page.IsPostBack))
                {
                    Display_EditLibrarySettings();
                }
                else
                {
                    Process_UpdateLibrarySettings();
                }
            }

            jsOperation.Text = _Operation;
            jsType.Text = _Type;

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
        finally
        {
            _ContentApi = null;
            _SiteApi = null;
        }
    }
    protected void ViewLibraryCategoryGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if ((_PageAction == "viewlibrarybycategory") || (_PageAction == "viewlibrarycategory"))
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (e.Item.Cells[1].Text.Equals("IMG-NONE") && e.Item.Cells[2].Text.Equals("IMG-NONE") && e.Item.Cells[3].Text.Equals("IMG-NONE") && e.Item.Cells[4].Text.Equals("IMG-NONE") && e.Item.Cells[5].Text.Equals("IMG-NONE"))
                    {
                        // e.Item.Cells(0).Attributes.Add("align", "Left")
                        e.Item.Cells[0].ColumnSpan = 6;
                        //If (e.Item.Cells(1).Text.Equals("MailProperties")) Then
                        //    e.Item.Cells(0).CssClass = "info-header"
                        //End If
                        e.Item.Cells.RemoveAt(5);
                        e.Item.Cells.RemoveAt(4);
                        e.Item.Cells.RemoveAt(3);
                        e.Item.Cells.RemoveAt(2);
                        e.Item.Cells.RemoveAt(1);
                    }
                    else if (e.Item.Cells[2].Text.Equals("IMG-NONE") && e.Item.Cells[3].Text.Equals("IMG-NONE") && e.Item.Cells[4].Text.Equals("IMG-NONE") && e.Item.Cells[5].Text.Equals("IMG-NONE"))
                    {
                        // e.Item.Cells(0).Attributes.Add("align", "Left")
                        e.Item.Cells[1].ColumnSpan = 5;
                        //If (e.Item.Cells(1).Text.Equals("MailProperties")) Then
                        //    e.Item.Cells(0).CssClass = "info-header"
                        //End If
                        e.Item.Cells.RemoveAt(5);
                        e.Item.Cells.RemoveAt(4);
                        e.Item.Cells.RemoveAt(3);
                        e.Item.Cells.RemoveAt(2);
                    }
                    break;
            }
        }
    }
    protected void DeleteLibraryItemGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("HEADER-ITEM"))
                {
                    e.Item.Cells[0].ColumnSpan = 2;
                    e.Item.Cells[0].CssClass = "checkboxqustion";
                    e.Item.Cells.RemoveAt(1);
                }
                break;
        }
    }

    #endregion

    #region Display

    private void Display_EditLibrarySettings()
    {
        LibraryConfigData lib_settings_data;
        string Directory = "";
        EditLibrarySettingsPanel.Visible = true;
        _FolderId = Convert.ToInt64(Request.QueryString["id"]);
        lib_settings_data = _ContentApi.GetLibrarySettings(_FolderId);

        imageextensions.Value = lib_settings_data.ImageExtensions;
        fileextensions.Value = lib_settings_data.FileExtensions;
        relativeimages.Text = _MessageHelper.GetMessage("make dir rel to site") + " " + _SitePath;
        if (!string.IsNullOrEmpty(lib_settings_data.RelativeImages))
        {
            relativeimages.Checked = true;
        }
        if (_FolderId != 0)
        {
            relativeimages.Enabled = true;
        }

        Directory = lib_settings_data.ImageDirectory;

        if ((Directory.Length > 0) && (!string.IsNullOrEmpty(lib_settings_data.RelativeImages)) && (Strings.InStr(1, Directory, _SitePath, CompareMethod.Binary) > 0))
        {
            Directory = Strings.Mid(Directory, Strings.Len(_SitePath) + 1, 255);
        }

        if (Convert.ToInt64(Request.QueryString["id"]) == 0)
        {
            td_els_imgdirectory.InnerHtml += "<input type=\"text\" size=\"75\" maxlength=\"255\" name=\"imagedirectory\" value=\"" + Directory + "\"/>";
        }
        else
        {
            td_els_imgdirectory.InnerHtml += Directory + "<input type=\"hidden\" name=\"imagedirectory\" value=\"" + Directory + "\"/>";
        }

        if (!string.IsNullOrEmpty(lib_settings_data.RelativeImages))
        {
            relativefiles.Checked = true;
        }
        if (_FolderId != 0)
        {
            relativefiles.Disabled = true;
        }
        Directory = lib_settings_data.FileDirectory;
        if ((Directory.Length > 0) && (!string.IsNullOrEmpty(lib_settings_data.RelativeImages)) && (Strings.InStr(1, Directory, _SitePath, CompareMethod.Binary) > 0))
        {
          
            Directory = Strings.Mid(Directory, Strings.Len(_SitePath) + 1, 255);
        }

        if (_FolderId == 0)
        {
            td_els_directory.InnerHtml += "<input type=\"text\" size=\"75\" maxlength=\"255\" name=\"filedirectory\" value=\"" + Directory + "\"/>";
        }
        else
        {
            td_els_directory.InnerHtml += Directory + "<input type=\"hidden\" name=\"filedirectory\" value=\"" + Directory + "\"/>";
        }
        librarytoolbar m_libraryToolBar;
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_RemoveLoadBalance()
    {
        RemoveLoadBalancePanel.Visible = true;
        LoadBalanceData[] load_balance_images;
        LoadBalanceData[] load_balance_files;
        librarytoolbar m_libraryToolBar;
        Ektron.Cms.Library.EkLibrary m_refLib;
        m_refLib = _ContentApi.EkLibraryRef;
        int i = 0;
        load_balance_images = _ContentApi.GetAllLoadBalancePaths("all");
        load_balance_files = _ContentApi.GetAllLoadBalancePaths("files");

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "REMOVE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Remove");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "WEBPATH";
        colBound.HeaderText = _MessageHelper.GetMessage("generic web path");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic type");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RELATIVE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic relative");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VERIFY";
        colBound.HeaderText = _MessageHelper.GetMessage("generic verified");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PATH";
        colBound.HeaderText = _MessageHelper.GetMessage("generic physical path");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        RemoveLoadBalanceGrid.Columns.Add(colBound);

        RemoveLoadBalanceGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("REMOVE", typeof(string)));
        dt.Columns.Add(new DataColumn("WEBPATH", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("RELATIVE", typeof(string)));
        dt.Columns.Add(new DataColumn("VERIFY", typeof(string)));
        dt.Columns.Add(new DataColumn("PATH", typeof(string)));
        string Directory = "";
        string AbsPath = "";
        System.Text.StringBuilder strTemp = new System.Text.StringBuilder();
        if (!(load_balance_images == null))
        {
            for (i = 0; i <= load_balance_images.Length - 1; i++)
            {
                AbsPath = "";
                dr = dt.NewRow();
                Directory = load_balance_images[i].Path;
                dr[0] = "<input type=\"checkbox\" name=\"MakeRelative_" + (i + 1) + "\" value = \"" + load_balance_images[i].MakeRelative + "\"/>";
                dr[1] = Directory;
                dr[2] = load_balance_images[i].Type; //center

                if (load_balance_images[i].MakeRelative > 0)
                {
                    dr[3] = "x";
                }
                else
                {
                    dr[3] = "&nbsp;";
                }


                try
                {
                    AbsPath = Server.MapPath(load_balance_images[i].Path);
                }
                catch
                {
                }
                if (m_refLib.IsFolderIO_OK(AbsPath))
                {
                    dr[4] = _VerfiyTrue;
                }
                else
                {
                    dr[4] = _VerfiyFalse;
                }
                dr[5] = AbsPath;
                strTemp.Append("<input type=\"hidden\" name=\"loadBalanceID_" + (i + 1) + "\" value=\"" + load_balance_images[i].Id + "\"/>");
                dt.Rows.Add(dr);
            }
        }
        lbPathCount.Value = Convert.ToString(i + 1);
        RLB_Hidden.Text = strTemp.ToString();

        DataView dv = new DataView(dt);
        RemoveLoadBalanceGrid.DataSource = dv;
        RemoveLoadBalanceGrid.DataBind();

        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.LoadBalanceInfo = load_balance_images;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_EditLoadBalanceSettings()
    {
        EditLoadBalanceSettingsPanel.Visible = true;
        LoadBalanceData load_balance_data;
        long lb_id;
        string strTemp = "";
        string Directory;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        lb_id = Convert.ToInt64(Request.QueryString["lbid"]);
        _FolderId = Convert.ToInt64(Request.QueryString["id"]);

        load_balance_data = _ContentApi.GetLBPathByID(lb_id);

        result.Append("<table class=\"ektronGrid\">");
        result.Append("<tr>");
        result.Append("<td class=\"label\">");
        if (load_balance_data.LibTypeID == 1) //
        {
            result.Append(_MessageHelper.GetMessage("images lbpath label"));
        }
        else
        {
            result.Append(_MessageHelper.GetMessage("files lbpath label"));
        }
        result.Append("</td>");
        strTemp = load_balance_data.Path;
        if (load_balance_data.MakeRelative > 0)
        {
            if ((load_balance_data.Path.IndexOf(_SitePath) + 1) > 0)
            {
                Directory = strTemp.Substring(_SitePath.Length + 1 - 1, 255);
            }
            else
            {
                Directory = strTemp;
            }
        }
        else
        {
            Directory = strTemp;
        }
        result.Append(strTemp);
        result.Append("<td class=\"value\">");
        result.Append("<input type=\"text\" size=\"75\" maxlength=\"255\" name=\"loadBalancePath\" value=\"" + Directory + "\"/>");
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("</table>");
        result.Append("<div class=\"ektronTopSpaceSmall\"></div>");
        strTemp = "<input type=\"checkbox\" name=\"MakeRelative\" value = \"" + load_balance_data.MakeRelative + "\"";
        if (load_balance_data.MakeRelative > 0)
        {
            strTemp += " checked ";
        }
        strTemp += "/>";
        result.Append(strTemp);
        result.Append(_MessageHelper.GetMessage("make dir rel to site") + " " + _SitePath);
        result.Append("<input type=\"hidden\" name=\"loadBalanceID\" value=\"" + load_balance_data.Id + "\"/>");
        DisplayEditLBSettingsData.Text = result.ToString();

        librarytoolbar m_libraryToolBar;
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_AddLoadBalance()
    {
        AddLoadBalancePanel.Visible = true;
        AssetType.Items.Add(new ListItem(_MessageHelper.GetMessage("images label") + " " + _MessageHelper.GetMessage("image path description"), "images")); //
        AssetType.Items.Add(new ListItem(_MessageHelper.GetMessage("files label") + " " + _MessageHelper.GetMessage("file path description"), "files")); //
        AssetType.Items[0].Selected = true;
        librarytoolbar m_libraryToolBar;
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        //m_libraryToolBar.SecurityInfo = security_data
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_ViewLoadBalance()
    {
        ViewLoadBalancePanel.Visible = true;
        PermissionData security_data;
        LibraryConfigData lib_settings_data;
        LoadBalanceData[] load_balance_data;
        librarytoolbar m_libraryToolBar;
        Ektron.Cms.Library.EkLibrary m_refLib;
        m_refLib = _ContentApi.EkLibraryRef;
        _FolderId = Convert.ToInt64(Request.QueryString["id"]);
        lib_settings_data = _ContentApi.GetLibrarySettings(-1);
        load_balance_data = _ContentApi.GetAllLoadBalancePaths("all");
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);


        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "WEBPATH";
        colBound.HeaderText = _MessageHelper.GetMessage("generic web path");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic type");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "RELATIVE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic relative");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VERIFY";
        colBound.HeaderText = _MessageHelper.GetMessage("generic verified");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLoadBalanceGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PATH";
        colBound.HeaderText = _MessageHelper.GetMessage("generic physical path");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLoadBalanceGrid.Columns.Add(colBound);

        ViewLoadBalanceGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("WEBPATH", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("RELATIVE", typeof(string)));
        dt.Columns.Add(new DataColumn("VERIFY", typeof(string)));
        dt.Columns.Add(new DataColumn("PATH", typeof(string)));
        string AbsPath = "";
        if (!(load_balance_data == null))
        {
            int i = 0;
            for (i = 0; i <= load_balance_data.Length - 1; i++)
            {
                AbsPath = "";
                dr = dt.NewRow();
                dr[0] = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=EditLoadBalanceSettings&lbid=" + load_balance_data[i].Id + "&id=" + _FolderId + "\" title=\"" + _MessageHelper.GetMessage("alt edit button text (lbpath)") + "\">" + load_balance_data[i].Path + "</a>";
                dr[1] = load_balance_data[i].Type;
                if (load_balance_data[i].MakeRelative > 0)
                {
                    dr[2] = "x";
                }
                try
                {
                    AbsPath = Server.MapPath(load_balance_data[i].Path);
                }
                catch
                {
                }
                if (m_refLib.IsFolderIO_OK(AbsPath))
                {
                    dr[3] = _VerfiyTrue;
                }
                else
                {
                    dr[3] = _VerfiyFalse;
                }

                dr[4] = AbsPath;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "";
            dr[2] = "";
            dr[3] = "";
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        ViewLoadBalanceGrid.DataSource = dv;
        ViewLoadBalanceGrid.DataBind();

        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.LoadBalanceInfo = load_balance_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_ViewLibrarySettings()
    {
        ViewLibrarySettingsPanel.Visible = true;
        PermissionData security_data;
        LibraryConfigData lib_settings_data;
        Ektron.Cms.Library.EkLibrary m_refLib;
        m_refLib = _ContentApi.EkLibraryRef;
        _FolderId = Convert.ToInt64(Request.QueryString["id"]);
        lib_settings_data = _ContentApi.GetLibrarySettings(_FolderId);
        security_data = _ContentApi.LoadPermissions(0, "folder", 0);

        System.Web.UI.WebControls.BoundColumn colLabel = new System.Web.UI.WebControls.BoundColumn();
        colLabel.DataField = "TITLE";
        ViewLibrarySettingsGrid.Columns.Add(colLabel);

        System.Web.UI.WebControls.BoundColumn colValue = new System.Web.UI.WebControls.BoundColumn();
        colValue.DataField = "VALUE";
        ViewLibrarySettingsGrid.Columns.Add(colValue);

        string Directory = "";

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("image extensions label") + "</span>";
        dr[1] = lib_settings_data.ImageExtensions;
        dt.Rows.Add(dr);

        if (security_data.IsAdmin)
        {
            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("image upload path label") + "</span>";
            if (!string.IsNullOrEmpty(lib_settings_data.RelativeImages))
            {
                dr[1] += _MessageHelper.GetMessage("path relative to site msg") + " \"" + _SitePath + "\"";
            }
            else
            {
                dr[1] += _MessageHelper.GetMessage("path relative to ws root");
            }
            Directory = lib_settings_data.ImageDirectory;
          
            if (!string.IsNullOrEmpty(lib_settings_data.RelativeImages))
            {
                if ((Strings.InStr(1, lib_settings_data.ImageDirectory, _SitePath,CompareMethod.Binary)) > 0)
                {
                    Directory = Strings.Mid(lib_settings_data.ImageDirectory, Strings.Len(_SitePath) + 1, 255);
                }
                else
                {
                    Directory = lib_settings_data.ImageDirectory;
                }
            }
            else
            {
                Directory = lib_settings_data.ImageDirectory;
            }
            dr[1] += Directory;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("physical path label") + "</span>";
            try
            {
                Directory = Server.MapPath(lib_settings_data.ImageDirectory);
                dr[1] = Directory;
            }
            catch
            {

            }
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("verified label") + "</span>";
            if (m_refLib.IsFolderIO_OK(Directory))
            {
                dr[1] += _VerfiyTrue;
            }
            else
            {
                dr[1] += _VerfiyFalse;
            }
            dt.Rows.Add(dr);
        }

        dr = dt.NewRow();
        dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("file extensions label") + "</span>";
        dr[1] = lib_settings_data.FileExtensions;
        dt.Rows.Add(dr);

        if (security_data.IsAdmin)
        {
            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("file upload path label") + "</span>";
            if (!string.IsNullOrEmpty(lib_settings_data.RelativeFiles))
            {
                dr[1] += _MessageHelper.GetMessage("path relative to site msg") + " \"" + _SitePath + "\"";
            }
            else
            {
                dr[1] += _MessageHelper.GetMessage("path relative to ws root");
            }
           
            if (!string.IsNullOrEmpty(lib_settings_data.RelativeFiles))
            {
                if ((Strings.InStr(1, lib_settings_data.FileDirectory, _SitePath, CompareMethod.Binary)) > 0)
                {
                    Directory = Strings.Mid(lib_settings_data.FileDirectory, Strings.Len(_SitePath) + 1, 255);
                }
                else
                {
                    Directory = lib_settings_data.FileDirectory;
                }
            }
            else
            {
                Directory = lib_settings_data.FileDirectory;
            }

            dr[1] += Directory;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("physical path label") + "</span>";
            try
            {
                Directory = Server.MapPath(lib_settings_data.FileDirectory);
                dr[1] = Directory;
            }
            catch
            {
            }
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("verified label") + "</span>";
            if (m_refLib.IsFolderIO_OK(Directory))
            {
                dr[1] += _VerfiyTrue;
            }
            else
            {
                dr[1] += _VerfiyFalse;
            }
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        ViewLibrarySettingsGrid.DataSource = dv;
        ViewLibrarySettingsGrid.DataBind();
        librarytoolbar m_libraryToolBar;
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
    }
    private void Display_EditLibraryItem()
    {
        EditLibraryItemPanel.Visible = true;
        FolderData folder_data;
        PermissionData security_data;
        LibraryData library_data;
        librarytoolbar m_libraryToolBar;
        string strPath = "";
        string[] tmpAr;
        _Id = Convert.ToInt64(Request.QueryString["id"]);
        _FolderId = Convert.ToInt64(Request.QueryString["parent_id"]);
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);
        if (!(library_data == null))
        {
            _Type = library_data.Type;
        }
        else
        {
            //ErrorString = "Item not found -HC"
        }


        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = _MessageHelper.GetMessage("generic Title");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        EditLibraryItemGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = _MessageHelper.GetMessage("generic ID");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        EditLibraryItemGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        if ((_Type == "files") || (_Type == "images"))
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic Filename");
        }
        else
        {
            colBound.HeaderText = _MessageHelper.GetMessage("generic URL Link");
        }
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        EditLibraryItemGrid.Columns.Add(colBound);

        if (_Type == "quicklinks")
        {
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "CONTENTID";
            colBound.HeaderText = _MessageHelper.GetMessage("generic Content ID");
            colBound.ItemStyle.Wrap = false;
            colBound.HeaderStyle.CssClass = "title-header";
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            EditLibraryItemGrid.Columns.Add(colBound);
        }

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        if (_Type == "quicklinks")
        {
            dt.Columns.Add(new DataColumn("CONTENTID", typeof(string)));
        }
        if (!(library_data == null))
        {
            dr = dt.NewRow();
            dr[0] = "<input type=\"text\" size=\"25\" maxlength=\"200\" name=\"frm_title\"  id=\"frm_title\" value=\"" + library_data.Title + "\" onkeypress = \"javascript:return CheckKeyValue(event, \'34,13\');\"/>";
            dr[1] = library_data.Id;

            if (_Type == "hyperlinks")
            {
                dr[2] = "<input type=\"text\" size=\"50\" maxlength=\"255\" name=\"frm_filename\" value=\"" + library_data.FileName + "\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
            }
            else if (((_Type == "quicklinks") || (_Type == "forms")) && (Ektron.Cms.Common.EkConstants.IsAssetContentType(library_data.ContentType, true) == false)) //   (library_data.FileName.IndexOf("javascript:void window.open") < 0) Then
            {
                if (folder_data.IsDomainFolder || folder_data.DomainProduction != "")
                {
                    tmpAr = library_data.FileName.Split(folder_data.DomainProduction.ToCharArray()[0]);
                    dr[2] = strPath + "<input type=\"text\" size=\"50\" maxlength=\"255\" name=\"frm_filename\" value=\"" + tmpAr[1] + "\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
                }
                else
                {
                    if (_SitePath == "/")
                    {
                        tmpAr = Strings.Split(library_data.FileName, _SitePath, 2, 0);
                    }
                    else
                    {
                        tmpAr = Strings.Split(library_data.FileName, _SitePath);
                    }

                    strPath = (string)(tmpAr[0] + _SitePath);
                    dr[2] = strPath + "<input type=\"text\" size=\"50\" maxlength=\"255\" name=\"frm_filename\" value=\"" + tmpAr[1] + "\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
                }
            }
            else
            {
                // dr(2) = library_data.FileName
                dr[2] += "<input type=\"text\" disabled=\"true\" size=\"50\" maxlength=\"255\" name=\"frm_filename\" value=\"" + library_data.FileName + "\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
            }

            if (_Type == "quicklinks")
            {
                dr[3] = "<input type=\"hidden\" size=\"9\" maxlength=\"19\" name=\"frm_content_id\" value=\"" + library_data.ContentId + "\"/>" + library_data.ContentId;
            }
            else
            {
                dr[2] += "<input type=\"hidden\" name=\"frm_content_id\" value=\"\"/>";
            }
            dr[2] += "<input type=hidden name=frm_libtype id=frm_libtype value=\"" + _Type + "\"/>";
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        EditLibraryItemGrid.DataSource = dv;
        EditLibraryItemGrid.DataBind();


        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        m_libraryToolBar.LibraryInfo = library_data;
        if (_Type != "quicklinks" && _Type != "forms")
        {
            editTabs.Visible = true;
            dvSummary.Attributes.Add("class", _SelectedDivStyleClass);
            bool bManagedAsset = false;
            ContentData content_data = null;
            ContentAPI m_refcontentapi = new ContentAPI();
            CustomFields cFieldsO = new CustomFields();
            ContentMetaData[] meta_data;

            if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refcontentapi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refcontentapi.ContentLanguage = _ContentLanguage;
            }


            if (library_data.ContentId != 0)
            {
                content_data = m_refcontentapi.GetContentById(library_data.ContentId, 0);
            }
            if (!(content_data == null))
            {
                meta_data = content_data.MetaData;
                _ContentTeaser = content_data.Teaser;
                if (content_data.Type != Ektron.Cms.Common.EkConstants.CMSContentType_Library)
                {
                    bManagedAsset = true;
                }
            }
            else
            {
                meta_data = m_refcontentapi.GetMetaDataTypes("id");
            }

            // Setting the titles for tabs
            EditdvSummaryTxt.Text = _MessageHelper.GetMessage("Summary text");
            EditdvMetadataTxt.Text = _MessageHelper.GetMessage("metadata text");
            EditdvCategoryTxt.Text = _MessageHelper.GetMessage("viewtaxonomytabtitle");

            RenderSummaryEditor();

            //Populating the category
            string Action = "Edit";
            PopulateCategory(Action);

            if (meta_data != null)
            {
                if (meta_data.Length > 0)
                {
                    if (!bManagedAsset)
                    {
                        if (m_refcontentapi.ContentLanguage != Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                        {
                            int v = 0;
                            ShowMeta.Text = CustomFields.WriteFilteredMetadataForEdit(meta_data, true, "update", _FolderId, ref v, null).ToString();

                            ShowTagEditArea(m_refcontentapi, library_data);
                        }
                        else
                        {
                            string strLink;
                            strLink = "<a href=\"library.aspx?LangType=" + meta_data[0].Language.ToString() + "&action=" + _PageAction + "&id=" + _Id + "&parent_id=" + _FolderId + "\">";
                            ShowMeta.Text = "<span style=\"COLOR: red\">*Note - Related metadata/tags will be displayed only if a specific language was selected. You may either go back to the page and select a language, or click " + strLink + "here</a> to view the metadata with the language selected automatically.</span>";
                        }
                    }

                }
            }
        }
    }
    private void Display_DeleteLibraryItem()
    {
        DeleteLibraryItemPanel.Visible = true;
        FolderData folder_data;
        PermissionData security_data;
        LibraryData library_data;
        librarytoolbar m_libraryToolBar;
        string strLibraryTitle = "";
        string strLibraryFileName = "";
        _Id = Convert.ToInt64(Request.QueryString["item_id"]); //Request.QueryString("id")
        _FolderId = Convert.ToInt64(Request.QueryString["parent_id"]);
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);
        if (!(library_data == null))
        {
            _Type = library_data.Type;
            strLibraryFileName = library_data.FileName;
            strLibraryTitle = library_data.Title;
        }
        else
        {
            //ErrorString = "Item not found -HC"
        }
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.Height = Unit.Point(0);
        colBound.ItemStyle.CssClass = "label";
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        DeleteLibraryItemGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TEXT";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Point(0);
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        DeleteLibraryItemGrid.Columns.Add(colBound);

        DeleteLibraryItemGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("TEXT", typeof(string)));

        DeleteItemHiddenFields.Text = "<input type=\"hidden\" name=\"frm_item_id\" value=\"" + _Id + "\"/>";
        DeleteItemHiddenFields.Text += "<input type=\"hidden\" name=\"frm_folder_id\" value=\"" + _FolderId + "\" /> ";
        DeleteItemHiddenFields.Text += "<input type=\"hidden\" name=\"frm_filename\" value=\"" + strLibraryFileName + "\"/>";
        DeleteItemHiddenFields.Text += "<input type=\"hidden\" name=\"frm_title\" value=\"" + strLibraryTitle + "\"/> ";
        DeleteItemHiddenFields.Text += "<input type=\"hidden\" name=\"frm_libtype\" value=\"" + _Type + "\"/>";

        dr = dt.NewRow();
        dr[0] = "<input type=\"checkbox\" name=\"remove\"/>" + " " + _MessageHelper.GetMessage("remove file from server");
        dr[1] = "HEADER-ITEM";
        dt.Rows.Add(dr);
        if (!(library_data == null))
        {
            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("generic title label") + "</span>";
            dr[1] = library_data.Title;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            if (library_data.Type == "quicklinks" || library_data.Type == "hyperlinks")
            {
                dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("url link label") + "</span>";
            }
            else
            {
                dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("filename label") + "</span>";
            }
            dr[1] = library_data.FileName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("library id label") + "</span>";
            dr[1] = library_data.Id;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("parent folder label") + "</span>";
            dr[1] = library_data.FolderName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content LUE label") + "</span>";
            dr[1] = library_data.EditorLastName + ", " + library_data.EditorFirstName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content LED label") + "</span>";
            dr[1] = library_data.DisplayLastEditDate;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content DC label") + "</span>";
            dr[1] = library_data.DisplayDateCreated;
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        DeleteLibraryItemGrid.DataSource = dv;
        DeleteLibraryItemGrid.DataBind();

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LINK";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        DeleteLibraryItemLinkGrid.Columns.Add(colBound);
        dt = new DataTable();
        dt.Columns.Add(new DataColumn("LINK", typeof(string)));
        dr = dt.NewRow();
        if (!(library_data == null))
        {

            if (library_data.Type == "images")
            {
                // Do not show image on final delete page to prevent IIS Lock
                //dr(0) = "<img src=""" & Replace(library_data.FileName, " ", "%20") & """>"
                dr[0] = "";
            }
            else if (library_data.Type == "quicklinks")
            {
                if ((library_data.FileName.IndexOf("?") + 1) > 0)
                {
                    dr[0] = "<a href=\"" + library_data.FileName.Replace(" ", "%20") + "\"&Preview=True" + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
                else
                {
                    dr[0] = "<a href=\"" + library_data.FileName.Replace(" ", "%20") + "\"?Preview=True" + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
            }
            else if ((library_data.Type == "hyperlinks") && ((!library_data.FileName.Contains("http://") && (!library_data.FileName.Contains("https://")))))
            {
                dr[0] = "<a href=\"" + library_data.FileName.Replace(" ", "%20") + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
            }
            else
            {
                dr[0] = "<a href=\"" + library_data.FileName.Replace(" ", "%20") + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
            }
        }
        dt.Rows.Add(dr);

        dv = new DataView(dt);
        DeleteLibraryItemLinkGrid.DataSource = dv;
        DeleteLibraryItemLinkGrid.DataBind();

        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        m_libraryToolBar.LibraryInfo = library_data;
    }
    private void Display_ViewLibraryItem()
    {
        ViewLibraryItemPanel.Visible = true;
        FolderData folder_data;
        PermissionData security_data;
        LibraryData library_data;
        librarytoolbar m_libraryToolBar;
        _Id = Convert.ToInt64(Request.QueryString["id"]);
        _FolderId = Convert.ToInt64(Request.QueryString["parent_id"]);
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);

        //StagingFileName property would be empty for non-multisite links and as well as when the site is not in Staging Mode
        if (library_data.StagingFileName == "")
        {
            library_data.StagingFileName = library_data.FileName;
        }

        if (!(library_data == null))
        {
            _Type = library_data.Type;
        }
        else
        {
            //ErrorString = "Item not found -HC"
        }

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.ItemStyle.CssClass = "label";
        ViewLibraryItemGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TEXT";
        System.Web.UI.WebControls.Unit percUnit;
        percUnit = System.Web.UI.WebControls.Unit.Percentage(95);
        colBound.HeaderStyle.Width = percUnit;
        ViewLibraryItemGrid.Columns.Add(colBound);
        percUnit = System.Web.UI.WebControls.Unit.Empty;

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("TEXT", typeof(string)));

        if (!(library_data == null))
        {
            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("generic title label") + "</span>";
            dr[1] = library_data.Title;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            if (library_data.Type == "quicklinks" || library_data.Type == "hyperlinks")
            {
                dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("url link label") + "</span>";
            }
            else
            {
                dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("filename label") + "</span>";
            }
            dr[1] = library_data.FileName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("library id label") + "</span>";
            dr[1] = library_data.Id;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("parent folder label") + "</span>";
            dr[1] = library_data.FolderName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content LUE label") + "</span>";
            dr[1] = library_data.EditorLastName + ", " + library_data.EditorFirstName;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content LED label") + "</span>";
            dr[1] = library_data.DisplayLastEditDate;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<span class=\"label\">" + _MessageHelper.GetMessage("content DC label") + "</span>";
            dr[1] = library_data.DisplayDateCreated;

            dt.Rows.Add(dr);
        }
        // Don't tbind the data yet, may have teaser data to include...

        //SEARCH METADATA''''''''''''
        ContentData content_data = null;
        if (_Type != "quicklinks" && _Type != "forms")
        {
            bool bManagedAsset = false;

            ContentAPI m_refcontentapi = new ContentAPI();
            CustomFields cFieldsO = new CustomFields();
            ContentMetaData[] meta_data;

            if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refcontentapi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refcontentapi.ContentLanguage = _ContentLanguage;
            }

            if (library_data.ContentId != 0)
            {
                content_data = m_refcontentapi.GetContentById(library_data.ContentId, 0);
            }
            if (!(content_data == null))
            {
                meta_data = content_data.MetaData;
                _ContentTeaser = content_data.Teaser;
                _ContentTeaser = _ContentTeaser.Replace("<p> </p>", string.Empty);
                if (content_data.Type != Ektron.Cms.Common.EkConstants.CMSContentType_Library)
                {
                    bManagedAsset = true;
                }
            }
            else
            {
                meta_data = m_refcontentapi.GetMetaDataTypes("id");
            }

            // Add the Teaser-Data for this Library item:
            dr = dt.NewRow();
            dr[0] = _MessageHelper.GetMessage("description label");
            dr[1] = _ContentTeaser;
            dt.Rows.Add(dr);

            if (meta_data != null)
            {
                if (meta_data.Length > 0)
                {
                    if (!bManagedAsset)
                    {
                        //ViewLibraryMeta.Text = cFieldsO.WriteMetadataForView(meta_data).ToString
                        if (m_refcontentapi.ContentLanguage != Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                        {
                            ViewLibraryMeta.Text = CustomFields.WriteFilteredMetadataForView(meta_data, _FolderId, true).ToString();

                            // display tag info for this library item
                            System.Text.StringBuilder taghtml = new System.Text.StringBuilder();
                            taghtml.Append("<fieldset style=\"margin:10px\">");
                            taghtml.Append("<legend>" + _MessageHelper.GetMessage("lbl personal tags") + "</legend>");
                            taghtml.Append("<div style=\"height: 80px; overflow: auto;\" >");
                            if (library_data.Id > 0)
                            {
                                LocalizationAPI localizationApi = new LocalizationAPI();
                                TagData[] tdaUser;
                                tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForObject(library_data.Id, EkEnumeration.CMSObjectTypes.Library, m_refcontentapi.ContentLanguage);

                                if (tdaUser != null && tdaUser.Length > 0)
                                {

                                    foreach (TagData td in tdaUser)
                                    {
                                        taghtml.Append("<input disabled=\"disabled\" checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                                        taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                                        taghtml.Append("&#160;" + td.Text + "<br />");
                                    }
                                }
                                else
                                {
                                    taghtml.Append(_MessageHelper.GetMessage("lbl notagsselected"));
                                }
                            }
                            taghtml.Append("</div>");
                            taghtml.Append("</fieldset>");
                            ViewLibraryTags.Text = taghtml.ToString();
                        }
                        else
                        {
                            string strLink;
                            strLink = "<a href=\"library.aspx?LangType=" + meta_data[0].Language.ToString() + "&action=" + _PageAction + "&id=" + _Id + "&parent_id=" + _FolderId + "\">";
                            ViewLibraryMeta.Text = "<span style=\"COLOR: red\">*Note - Related metadata/tags will be displayed only if a specific language was selected on the previous page. You may either go back to the previous page to select a language, or click " + strLink + "here</a> to view the metadata with the language selected automatically.</span>";
                        }
                    }
                }
            }
        }
        ///'''''''''''''''''''''''''

        //Binding the Taxonomy
        if (_Type != "quicklinks" && _Type != "forms")
        {
            if (library_data.ContentId != 0)
            {
                TaxonomyBaseData[] data = null;
                ContentAPI m_refcontentapi = new ContentAPI();
                Ektron.Cms.Content.EkContent cref;
                cref = m_refcontentapi.EkContentRef;
                data = cref.ReadAllAssignedCategory(library_data.ContentId);
                ViewTaxonomy.Text = "<fieldset style=\"margin: 10px;\"><legend>Category</legend>";
                ViewTaxonomy.Text += "<table width=\"100%\">";
                ViewTaxonomy.Text += "<tr><td>";
                if ((data != null) && data.Length > 0)
                {
                    foreach (TaxonomyBaseData tax_data in data)
                    {
                        ViewTaxonomy.Text = ViewTaxonomy.Text + "<li>" + tax_data.TaxonomyPath.Remove(0, 1).Replace("\\", ">") + "</li>";
                    }
                }
                else
                {
                    ViewTaxonomy.Text += _MessageHelper.GetMessage("lbl nocatselected");
                }
                ViewTaxonomy.Text += "</td>" + "</tr>" + "</table>" + "</fieldset>";
            }
        }

        // now bind data, possibly with teaser:
        DataView dv = new DataView(dt);
        ViewLibraryItemGrid.DataSource = dv;
        ViewLibraryItemGrid.DataBind();


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LINK";
        colBound.HeaderStyle.Width = Unit.Empty;
        colBound.HeaderStyle.Height = Unit.Empty;
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        ViewLibraryItemLinkGrid.Columns.Add(colBound);
        ViewLibraryItemLinkGrid.BorderColor = System.Drawing.Color.White;
        dt = new DataTable();
        dt.Columns.Add(new DataColumn("LINK", typeof(string)));

        if (!(library_data == null))
        {
            library_data.FileName = Server.HtmlDecode(library_data.FileName);
            if (library_data.TypeId == 1 || library_data.TypeId == 2)
            {
                library_data.FileName = library_data.FileName.Replace("%", "%25");
                library_data.FileName = library_data.FileName.Replace("#", "%23");
                library_data.FileName = library_data.FileName.Replace("$", "%24");
                library_data.FileName = library_data.FileName.Replace("&", "%26");
                library_data.FileName = library_data.FileName.Replace("^", "%5E");
            }

            dr = dt.NewRow();
            if (library_data.Type == "images")
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                if (content_data != null)
                {
                    if (content_data.AssetData.Id != "" && content_data.Status == "I")
                    {
                        dr[0] = content_data.Html;
                    }
                    else
                    {
                        dr[0] = "<img src=\"" + library_data.StagingFileName.Replace(" ", "%20") + "?n=" + r.Next(1, 5000) + "\">";
                    }
                }
                else
                {
                    dr[0] = "<img src=\"" + library_data.FileName.Replace(" ", "%20") + "?n=" + r.Next(1, 5000) + "\">";
                }

            }
            else if (library_data.Type == "quicklinks")
            {
                if (Ektron.Cms.Common.EkConstants.IsAssetContentType(library_data.ContentType, true) && library_data.ContentType != Ektron.Cms.Common.EkConstants.CMSContentType_Media)
                {
                    if ((library_data.FileName.ToString().ToLower().IndexOf("javascript:") == -1) && library_data.FileName.ToString().ToLower().IndexOf("downloadasset.aspx") == -1)
                    {
                        library_data.FileName = this._SiteApi.SitePath + library_data.FileName;
                    }
                    dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "&LangType=" + library_data.LanguageId + "\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
                else if ((library_data.FileName.IndexOf("?") + 1) > 0)
                {
                    if (library_data.FileName.ToString().ToLower().IndexOf("downloadasset.aspx") > -1)
                    {
                        dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "&LangType=" + library_data.LanguageId + "\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                    }
                    else
                    {
                        dr[0] = "<a href=\"" + ((library_data.StagingFileName.Replace(" ", "%20").StartsWith(_SitePath)) ? (library_data.StagingFileName.Replace(" ", "%20")) : (((library_data.StagingFileName.Substring(0, 7) != "http://") && (library_data.StagingFileName.Substring(0, 8) != "https://")) ? _SitePath + library_data.StagingFileName.Replace(" ", "%20") : library_data.StagingFileName.Replace(" ", "%20"))) + "\"&Preview=True&LangType=" + library_data.LanguageId + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                    }
                }
                else
                {
                    dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "\"&Preview=True&LangType=" + library_data.LanguageId + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
            }
            else if (library_data.Type == "forms")
            {
                if ((library_data.FileName.IndexOf("?") + 1) > 0)
                {
                    dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "\"&Preview=True&LangType=" + library_data.LanguageId + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
                else
                {
                    dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "\"&Preview=True&LangType=" + library_data.LanguageId + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
                }
            }
            else if ((library_data.Type == "hyperlinks") && (!library_data.FileName.Contains("http://")) && (!library_data.FileName.Contains("https://")))
            {
                dr[0] = "<a href=\"" + library_data.FileName.Replace(" ", "%20") + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";
            }
            else
            {

                dr[0] = "<a href=\"" + library_data.StagingFileName.Replace(" ", "%20") + "\" target=\"Preview\" title=\"" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "\">" + _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title + "</a>";

            }
            dt.Rows.Add(dr);
        }


        dv = new DataView(dt);
        ViewLibraryItemLinkGrid.DataSource = dv;
        ViewLibraryItemLinkGrid.DataBind();

        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        m_libraryToolBar.ContentType = library_data.ContentType;
        m_libraryToolBar.LibraryInfo = library_data;
    }
    private void Display_AddLibraryItem()
    {
        FolderData folder_data = null;
        PermissionData security_data = null;
        LibraryData library_data = null;
        LibraryConfigData lib_setting_data = null;
        librarytoolbar m_libraryToolBar = null;
        //int i = 0;
        string Action = "add";
        EditLibraryItemPanel.Visible = true;

        //Id, FolderId differes in this action
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            _Id = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (!string.IsNullOrEmpty(Request.QueryString["folder"]))
        {
            _FolderId = Convert.ToInt64(Request.QueryString["folder"]);
        }
        if (_Type == "images" || _Type == "files")
        {
            LibraryItem.Enctype = "multipart/form-data";
        }

        if (!string.IsNullOrEmpty(Request.QueryString["operation"]))
        {
            _Operation = Request.QueryString["operation"].ToLower();
        }
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        if (_Operation == "overwrite")
        {
            library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);
        }
        lib_setting_data = _ContentApi.GetLibrarySettings(_FolderId);
        jsImageExtension.Text = lib_setting_data.ImageExtensions;
        jsFileExtension.Text = lib_setting_data.FileExtensions;
        if (security_data.CanAddToFileLib)
        {
            jsAddToFileLib.Text = "true";
        }
        else
        {
            jsAddToFileLib.Text = "true";
        }
        if (security_data.CanAddToImageLib)
        {
            jsAddToImageLib.Text = "true";
        }
        else
        {
            jsAddToImageLib.Text = "true";
        }
        frm_folder_id.Value = Convert.ToString(_FolderId);
        frm_libtype.Value = _Type;
        frm_operation.Value = _Operation;
        frm_library_id.Value = Convert.ToString(_Id);
        if (_Type == "images")
        {
            upload_directory.Value = lib_setting_data.ImageDirectory;
        }
        else if (_Type == "files")
        {
            upload_directory.Value = lib_setting_data.FileDirectory;
        }
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        m_libraryToolBar.Operation = _Operation;
        AddLibraryItemPanel.Visible = true;

        tr1_td1_ali.InnerHtml = _MessageHelper.GetMessage("generic Title");
        if ((_Type == "files") || (_Type == "images"))
        {
            tr1_td2_ali.InnerHtml = _MessageHelper.GetMessage("generic Filename");
        }
        else
        {
            tr1_td2_ali.InnerHtml = _MessageHelper.GetMessage("generic URL Link");
        }
        if (_Type == "quicklinks")
        {
            tr1_td3_ali.InnerHtml = _MessageHelper.GetMessage("generic Content ID");
        }
        else
        {
            tr1_td3_ali.Visible = false;
        }
        if ((_Type == "quicklinks") || (_Type == "Forms"))
        {
            tr2_td3_ali.InnerHtml = "<input type=\"text\" style=\"width:75px\" size=\"9\" maxlength=\"19\" name=\"frm_content_id\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
        }
        else
        {
            tr2_td3_ali.InnerHtml = "<input type=\"hidden\" name=\"frm_content_id\" value=\"\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
            tr2_td3_ali.Attributes.Add("style", "display:none;");
        }


        if (_Operation == "overwrite")
        {
            tr2_td1_ali.InnerHtml = library_data.Title + "<input type=\"hidden\" size=\"25\" maxlength=\"200\" name=\"frm_title\" value=\"" + library_data.Title + "\"/>";
        }
        else
        {
            tr2_td1_ali.InnerHtml = "<input type=\"text\" size=\"15\" maxlength=\"200\" name=\"frm_title\"  id=\"frm_title\" onkeypress = \"javascript:return CheckKeyValue(event, \'34,13\');\"/>";
        }

        if (_Operation == "overwrite")
        {
            frm_oldfilename.Value = library_data.FileName;
            OverwriteSubPanel1.Visible = true;
            OverwriteSubPanel2.Visible = true;
            TD_filename.InnerHtml = "<input type=\"file\" size=\"40\" maxlength=\"255\" id=\"frm_filename\" name=\"frm_filename\"/>";
            tr2_td2_ali_controls.Text = library_data.FileName;
            if (library_data.Type == "images")
            {
                Overwrite_Image.ImageUrl = library_data.FileName.Replace(" ", "%20");
                Overwrite_Image.ImageUrl += "?id=" + EkFunctions.Random(1, 1000).ToString();
                Overwrite_Image.Visible = true;
            }
            else
            {
                Overwrite_link.Visible = true;
                Overwrite_link.Text = _MessageHelper.GetMessage("generic Preview title") + " " + library_data.Title;
                Overwrite_link.NavigateUrl = library_data.FileName;
                Overwrite_link.Target = "CurrentPreview";
                Overwrite_link.ToolTip = _MessageHelper.GetMessage("generic Preview title");
            }
            AddItemFocus.Text = "document.forms[0].frm_filename.focus();";
        }
        else
        {
            OverwriteSubPanel0.Visible = true;

            if (_Type == "hyperlinks")
            {
                tr2_td2_ali_controls.Text = "<input type=\"text\" size=\"50\" maxlength=\"255\" name=\"frm_filename\" value=\"http://\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/>";
                frm_filename.Visible = false;
            }
            else if (_Type == "quicklinks")
            {
                tr2_td2_ali_controls.Text = "<span style=\"white-space:nowrap;\">" + _SitePath + "<input type=\"text\" size=\"" + (50 - Strings.Len(_SitePath)) + "\" maxlength=\"255\" name=\"frm_filename\" value=\"\" onkeypress=\"javascript:return CheckKeyValue(event,\'34\');\"/></span>";
                frm_filename.Visible = false;
            }
            AddItemFocus.Text = "document.forms[0].frm_title.focus();";
        }

        if (_Type != "quicklinks" && _Type != "forms")
        {
            addTabs.Visible = false;
            editTabs.Visible = true;

            dvSummary.Attributes.Add("class", _SelectedDivStyleClass);
            CustomFields cFieldsO = new CustomFields();
            ContentMetaData[] meta_data = null;
            ContentAPI m_refcontentapi = new ContentAPI();
            ContentData content_data = new ContentData();
            bool bManagedAsset = false;
            string ty = "";

            if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refcontentapi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refcontentapi.ContentLanguage = _ContentLanguage;
            }

            if (_Operation == "overwrite")
            {
                Action = "Edit";
                ty = "update";
                if (library_data.ContentId != 0)
                {
                    content_data = m_refcontentapi.GetContentById(library_data.ContentId, 0);

                    if (!(content_data == null))
                    {
                        meta_data = content_data.MetaData;
                        _ContentTeaser = content_data.Teaser;
                        if (content_data.Type != Ektron.Cms.Common.EkConstants.CMSContentType_Library)
                        {
                            bManagedAsset = true;
                        }
                    }
                    else
                    {
                        meta_data = m_refcontentapi.GetMetaDataTypes("id");
                    }
                }
            }
            else
            {
                if (_PageAction == "addlibraryitem")
                {
                    ty = "add";
                }
                meta_data = m_refcontentapi.GetMetaDataTypes("id");
            }


            RenderSummaryEditor();
            // Setting the titles for tabs
            AdddvSummaryTxt.Text = _MessageHelper.GetMessage("Summary text");
            AdddvMetadataTxt.Text = _MessageHelper.GetMessage("metadata text");
            AdddvCategoryTxt.Text = _MessageHelper.GetMessage("viewtaxonomytabtitle");

            EditdvSummaryTxt.Text = _MessageHelper.GetMessage("Summary text");
            EditdvMetadataTxt.Text = _MessageHelper.GetMessage("metadata text");
            EditdvCategoryTxt.Text = _MessageHelper.GetMessage("viewtaxonomytabtitle");


            //Populating the category Tab

            PopulateCategory(Action);

            if (meta_data != null)
            {
                if (meta_data.Length > 0)
                {
                    if (!bManagedAsset)//  bManagedAsset)
                    {
                        int c = 0;
                        ShowMeta.Text = CustomFields.WriteFilteredMetadataForEdit(meta_data, true, ty, _FolderId, ref c, null).ToString();

                        ShowTagEditArea(m_refcontentapi, library_data);
                    }
                }
            }
        }
    }
    private void Display_UpdateQlinkTemplateByCategory()
    {
        hdnIsPostBack.Value = "false";

        //FormAction = "LangType=" & m_intContentLanguage & "&action=DoUpdateQlinkTemplateByCategory"
        //SetPostBackPage()
        FolderData folder_data;
        PermissionData security_data;
        LibraryData[] library_data;
        librarytoolbar m_libraryToolBar;
        int i = 0;

        _FolderId = Convert.ToInt64(Request.QueryString["id"]);
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        _CurrentPageNumber = System.Convert.ToInt32(this.uxPagingUpdateLink.SelectedPage);

        library_data = _ContentApi.GetAllChildLibItemsByType(_Type, _FolderId, _OrderBy, System.Convert.ToInt32(_CurrentPageNumber + 1), m_refcontentapi.RequestInformationRef.PagingSize, ref _TotalPagesNumber);

        //Fix for Assets in a multisite enviroment [workarea/library.aspx.vb]
        Microsoft.VisualBasic.Collection fCol = new Microsoft.VisualBasic.Collection();
        Ektron.Cms.API.Folder fapi = new Ektron.Cms.API.Folder();
        Ektron.Cms.FolderData[] fdata;
        Ektron.Cms.Content.EkContent m_refContent;
        ContentAPI m_refContentApi = new ContentAPI();

        m_refContent = m_refContentApi.EkContentRef;

        fdata = m_refContentApi.GetChildFolders(0, false, EkEnumeration.FolderOrderBy.Name);
        if (fdata != null)
        {
            foreach (FolderData thisfdata in fdata)
            {
                if (thisfdata.FolderType == 2)
                {
                    fCol = m_refContent.GetFolderInfov2_0(thisfdata.Id);
                    foreach (LibraryData thisresult in library_data)
                    {
                        thisresult.FileName = Regex.Replace(thisresult.FileName, "http://" + fCol["DomainProduction"] + "javascript", "javascript");
                    }
                }
            }
        }
        //End fix for multisite and Assets


        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        m_libraryToolBar.LibType = _Type;
        UpdateQlinkTemplateByCategoryPanel.Visible = true;

        qlinkfrom.InnerHtml = _SitePath + "<input type=\"textbox\" size=\"50\" maxlength=\"255\" value=\"" + folder_data.TemplateFileName + "\" name=\"template_from\"/>";
        qlinkto.InnerHtml = _SitePath + "<input type=\"textbox\" size=\"50\" maxlength=\"255\" value=\"" + folder_data.TemplateFileName + "\" name=\"template_to\"/>";

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CHECK";
        colBound.HeaderText = "<input type=\"checkbox\" name=\"all\" onclick=\"javascript:checkAll(document.forms[0].all.checked);\"/>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryTitle&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryID&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATEMODIFIED";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=date&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FILENAME";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryFilename&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic URL Link") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CONTENTID";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=ContentID&id=" + _FolderId + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Content ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        QlinkTemplateByCategoryGrid.Columns.Add(colBound);

        QlinkTemplateByCategoryGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;
        //int intItemLanguage = -1;
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
        dt.Columns.Add(new DataColumn("FILENAME", typeof(string)));
        dt.Columns.Add(new DataColumn("CONTENTID", typeof(string)));

        if (!(library_data == null))
        {
            string strLibIds = "";
            for (i = 0; i <= library_data.Length - 1; i++)
            {
                strLibIds = strLibIds + library_data[i].Id + ",";
                dr = dt.NewRow();
                dr[0] = "<input type=\"checkbox\" name=\"id_" + library_data[i].Id + "\"/>";
                dr[1] = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryItem&id=" + library_data[i].Id + "&parent_id=" + _FolderId + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(library_data[i].Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + library_data[i].Title + "</a>";
                dr[2] = library_data[i].Id;
                dr[3] = library_data[i].DisplayLastEditDate;
                dr[4] = library_data[i].FileName;
                dr[5] = library_data[i].ContentId;
                dt.Rows.Add(dr);
            }
            if (strLibIds != "")
            {
                strLibIds = strLibIds.Substring(0, strLibIds.Length - 1);
            }
            libids.Value = strLibIds;
        }
        folder_id.Value = Convert.ToString(_FolderId);
        DataView dv = new DataView(dt);
        QlinkTemplateByCategoryGrid.DataSource = dv;
        QlinkTemplateByCategoryGrid.DataBind();

        if (_TotalPagesNumber > 1)
        {
            this.uxPagingUpdateLink.TotalPages = _TotalPagesNumber;
            this.uxPagingUpdateLink.CurrentPageIndex = _CurrentPageNumber;
        }
        else
        {
            this.uxPagingUpdateLink.Visible = false;
        }
    }
    private void Display_ViewLibraryByCategory()
    {
        FolderData folder_data;
        PermissionData security_data;
        LibraryData[] library_data;
        librarytoolbar m_libraryToolBar;
        int i = 0;
        string strOldDesLoc;
        System.IO.FileInfo fs2;
        string strTeaser = "";

        //allow navigation away from Library
        jsDisableNav.Text = "false";

        _CurrentPageNumber = System.Convert.ToInt32(this.uxPaging.SelectedPage);

        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            _FolderId = Convert.ToInt64(Request.QueryString["id"]);
        }
        else
        {
            _FolderId = Convert.ToInt64(Request.QueryString["amp;id"]);
        }

        if (_ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            m_refcontentapi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
        }
        else
        {
            m_refcontentapi.ContentLanguage = _ContentLanguage;
        }

        folder_data = m_refcontentapi.GetFolderById(_FolderId);
        if (folder_data == null)
        {
            Response.Redirect((string)("notify_user.aspx?Tree=LibraryTree&folder_id=" + Convert.ToInt64(_FolderId)), false);
            return; //Not required
        }

        security_data = m_refcontentapi.LoadPermissions(_FolderId, "folder", 0);
        library_data = m_refcontentapi.GetAllChildLibItemsByType(_Type, _FolderId, _OrderBy, System.Convert.ToInt32(_CurrentPageNumber + 1), m_refcontentapi.RequestInformationRef.PagingSize, ref _TotalPagesNumber);

        //Fix for Assets in a multisite enviroment [workarea/library.aspx.vb]
        Microsoft.VisualBasic.Collection fCol = new Microsoft.VisualBasic.Collection();
        Ektron.Cms.API.Folder fapi = new Ektron.Cms.API.Folder();
        Ektron.Cms.FolderData[] fdata;
        Ektron.Cms.Content.EkContent m_refContent;

        m_refContent = m_refcontentapi.EkContentRef;
        fdata = m_refcontentapi.GetChildFolders(0, false, EkEnumeration.FolderOrderBy.Name);
        if (fdata != null)
        {
            foreach (FolderData thisfdata in fdata)
            {
                if (thisfdata.FolderType == 2)
                {
                    fCol = m_refContent.GetFolderInfov2_0(thisfdata.Id);
                    foreach (LibraryData thisresult in library_data)
                    {
                        string domain_Prod = (string)("http://" + fCol["DomainProduction"]);
                        if ((_Type == "quicklinks" || _Type == "forms") &&(! string.IsNullOrEmpty(fCol["DomainProduction"].ToString())) && thisresult.FileName.IndexOf((String)fCol["DomainProduction"]) != -1 && !thisresult.FileName.Substring(System.Convert.ToInt32(thisresult.FileName.IndexOf(domain_Prod) + domain_Prod.Length)).StartsWith("/"))
                        {
                            thisresult.FileName = thisresult.FileName.Replace("http://" + fCol["DomainProduction"], "http://" + fCol["DomainProduction"] + "/");
                        }
                        thisresult.FileName = Regex.Replace(thisresult.FileName, "http://" + fCol["DomainProduction"] + "javascript", "javascript");
                    }
                }
            }
        }
        //End fix for multisite and Assets

        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.FolderId = _FolderId;
        m_libraryToolBar.LibType = _Type;
        m_libraryToolBar.SecurityInfo = security_data;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        ViewLibraryCategoryPanel.Visible = true;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryTitle&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryID&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATEMODIFIED";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=date&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Date Modified") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "FILENAME";
        if ((_Type == "files") || (_Type == "images"))
        {
            colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryFilename&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Filename") + "</a>";
        }
        else
        {
            colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=LibraryFilename&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic URL Link") + "</a>";
        }
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "CONTENTID";
        if (_Type == "quicklinks")
        {
            colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=ContentID&id=" + _FolderId + "&type=" + _Type + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Content ID") + "</a>";
        }
        else
        {
            colBound.HeaderText = "";
            colBound.Visible = false;
        }
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        if ((_Type == "forms") || (_Type == "quicklinks"))
        {
            colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&orderby=contentlanguage&id=" + _FolderId + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + "Language" + "</a>";
        }
        else
        {
            colBound.HeaderText = "";
            colBound.Visible = false;
        }
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);

        ViewLibraryCategoryGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;
        int intItemLanguage = -1;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("DATEMODIFIED", typeof(string)));
        dt.Columns.Add(new DataColumn("FILENAME", typeof(string)));
        dt.Columns.Add(new DataColumn("CONTENTID", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        if (!(library_data == null))
        {
            FileInfo fs = null;
            string strDesLoc = "";
            string myImageFile = "";
            string myNewImageFile = "";
            string sExt = "";
            string[] myfilePathArray;
            for (i = 0; i <= library_data.Length - 1; i++)
            {




                if (library_data[i].StagingFileName == "")
                {
                    library_data[i].StagingFileName = library_data[i].FileName;
                }


                if ((_Type == "forms") || (_Type == "quicklinks"))
                {

                    intItemLanguage = library_data[i].LanguageId;
                }
                else
                {
                    intItemLanguage = _ContentLanguage;
                }

                dr = dt.NewRow();
                dr[0] = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryItem&id=" + library_data[i].Id + "&parent_id=" + _FolderId + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(library_data[i].Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + library_data[i].Title + "</a>";
                dr[1] = library_data[i].Id;
                dr[2] = library_data[i].DisplayLastEditDate;
                dr[3] = library_data[i].FileName;
                if (_Type == "quicklinks")
                {
                    dr[4] = library_data[i].ContentId;
                }
                else
                {
                    dr[4] = "";
                }
                if ((_Type == "forms") || (_Type == "quicklinks"))
                {
                    dr[5] = library_data[i].LanguageId;
                }
                else
                {
                    dr[5] = "";
                }
                dt.Rows.Add(dr);

                if (_Type == "images")
                {
                    strDesLoc = library_data[i].StagingFileName;
                    myfilePathArray = strDesLoc.Split('/');
                    myImageFile = myfilePathArray[myfilePathArray.Length - 1];

                    sExt = (string)(myImageFile.Substring(myImageFile.Length - 3, 3).ToLower());
                    if ("gif" == sExt)
                    {
                        sExt = "png";
                        myNewImageFile = "thumb_" + myImageFile.Substring(0, myImageFile.Length - 3) + "png";
                    }
                    else
                    {
                        myNewImageFile = (string)("thumb_" + myImageFile);
                    }
                    try
                    {
                        strOldDesLoc = strDesLoc;
                        strDesLoc = strDesLoc.Replace(myImageFile, myNewImageFile);
                        if (!folder_data.IsDomainFolder && folder_data.DomainProduction == "")
                        {
                            fs = new FileInfo(Server.MapPath(strDesLoc));
                            if (!fs.Exists)
                            {
                                if (strDesLoc.ToLower().Contains("/assets/"))
                                {
                                    fs2 = new FileInfo(Server.MapPath(strOldDesLoc));
                                    if (fs2.Exists)
                                    {
                                        Utilities.ProcessThumbnail(Server.MapPath(strDesLoc.Substring(0, Strings.InStrRev(strDesLoc, "/", -1, 0))), myImageFile);
                                    }
                                }
                                fs = new FileInfo(Server.MapPath(strDesLoc));
                            }
                        }

                        if (fs == null || fs.Exists)
                        {
                            Random r = new Random(System.DateTime.Now.Millisecond);
                            //dr = dt.NewRow
                            dr[0] += "<br /><a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryItem&id=" + library_data[i].Id + "&parent_id=" + _FolderId + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(library_data[i].Title, "\'", "`", 1, -1, 0) + "\"" + "\'>" + "<img src=\"" + strDesLoc + "?n=" + r.Next(1, 5000) + "\"></a>";
                            if (library_data[i].ContentId >0)
                            {
                                strTeaser = m_refcontentapi.GetJustTeaserByContentId(library_data[i].ContentId, library_data[i].LanguageId);
                                strTeaser = strTeaser.Replace("<p> </p>", string.Empty);
                                if (strTeaser != "")
                                {
                                    dr[1] += "<table width=\'89%\'><tr><td style=\'white-space: normal;\' nowrap=\'true\'><div>" + strTeaser + "</div></td></tr></table>";
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }
        }
        DataView dv = new DataView(dt);


        this.ViewLibraryCategoryGrid.PageSize = _CommonApi.RequestInformationRef.PagingSize;
        this.ViewLibraryCategoryGrid.DataSource = dv;
        this.ViewLibraryCategoryGrid.CurrentPageIndex = _CurrentPageNumber;
        this.ViewLibraryCategoryGrid.DataBind();
        if (_TotalPagesNumber > 1)
        {
            this.uxPaging.TotalPages = _TotalPagesNumber;
            this.uxPaging.CurrentPageIndex = _CurrentPageNumber;
        }
        else
        {
            this.uxPaging.Visible = false;
        }

    }
    private void Display_ViewLibraryCategory()
    {
        ViewLibraryCategoryPanel.Visible = true;
        FolderData folder_data;
        PermissionData security_data;
        LibraryTypeData[] lib_type_data;
        int i = 0;
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                _FolderId = Convert.ToInt64(Request.QueryString["id"]);
            }
        }
        folder_data = _ContentApi.GetFolderById(_FolderId);
        security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
        lib_type_data = _ContentApi.GetLibraryTypes(_FolderId, _OrderBy);
        librarytoolbar m_libraryToolBar;
        m_libraryToolBar = (librarytoolbar)(LoadControl("controls/library/librarytoolbar.ascx"));
        ToolBarHolder.Controls.Add(m_libraryToolBar);
        m_libraryToolBar.AppImgPath = _AppImgPath;
        m_libraryToolBar.PageAction = _PageAction;
        m_libraryToolBar.FolderInfo = folder_data;
        m_libraryToolBar.ContentLanguage = _ContentLanguage;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryCategory&orderby=LibraryTypeName&id=" + _FolderId + "\" title=\"" + _MessageHelper.GetMessage("click to sort msg") + "\">" + _MessageHelper.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        ViewLibraryCategoryGrid.Columns.Add(colBound);
        ViewLibraryCategoryGrid.BorderColor = System.Drawing.Color.White;
        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        if (!(lib_type_data == null))
        {
            for (i = 0; i <= lib_type_data.Length - 1; i++)
            {
                if (security_data.IsReadOnlyLib)
                {
                    dr = dt.NewRow();
                    dr[0] = "<a href=\"library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + _FolderId + "&type=" + lib_type_data[i].Name + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(lib_type_data[i].Name, "\'", "`", 1, -1, 0) + "\"" + "\'><img src=\"" + _AppImgPath + "folderclosed_1.gif" + "\" title=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(lib_type_data[i].Name, "\'", "`", 1, -1, 0) + "\"" + "\' alt=\'" + _MessageHelper.GetMessage("generic View") + " \"" + Strings.Replace(lib_type_data[i].Name, "\'", "`", 1, -1, 0) + "\"" + "\' align=\"absbottom\">" + lib_type_data[i].Name + "</a>";
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);
        ViewLibraryCategoryGrid.DataSource = dv;
        ViewLibraryCategoryGrid.DataBind();
    }

    #endregion

    #region Helpers

    public bool IsSiteMultilingual
    {
        get
        {
            LanguageData[] languageDataArray = _SiteApi.GetAllActiveLanguages();
            UserAPI m_refUserApi = new UserAPI();
            if (m_refUserApi.EnableMultilingual == 0)
            {
                return false;
            }
            int languageEnabledCount = 0;
            foreach (LanguageData lang in languageDataArray)
            {
                if (lang.SiteEnabled)
                {
                    languageEnabledCount++;
                }
                if (languageEnabledCount > 1)
                {
                    break;
                }
            }

            return languageEnabledCount > 1;
        }

    }
    private string GetLanguageDropDownMarkup(string controlId)
    {

        int i;
        StringBuilder markup = new StringBuilder();
        ContentAPI m_refContentApi = new ContentAPI();

        if (IsSiteMultilingual)
        {
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\">");
            LanguageData[] languageDataArray = _SiteApi.GetAllActiveLanguages();
            if (!(languageDataArray == null))
            {
                for (i = 0; i <= languageDataArray.Length - 1; i++)
                {
                    if (languageDataArray[i].SiteEnabled)
                    {
                        markup.Append("<option ");
                        if (m_refContentApi.DefaultContentLanguage == languageDataArray[i].Id)
                        {
                            markup.Append(" selected");
                        }
                        markup.Append(" value=" + languageDataArray[i].Id + ">" + languageDataArray[i].LocalName);
                    }
                }
            }
            markup.Append("</select>");
        }
        else
        {
            //hardcode to default site language
            markup.Append("<select id=\"" + controlId + "\" name=\"" + controlId + "\" selectedindex=\"0\" >");
            markup.Append(" <option selected value=" + m_refContentApi.DefaultContentLanguage + ">");
            markup.Append("</select>");

        }

        return markup.ToString();
    }
    private void ShowTagEditArea(ContentAPI m_refcontentapi, LibraryData library_data)
    {
        // display tag edit area
        System.Text.StringBuilder taghtml = new System.Text.StringBuilder();
        error_TagsCantBeBlank.Text = _MessageHelper.GetMessage("msg error Blank Tag");
        error_InvalidChars.Text = _MessageHelper.GetMessage("msg error Tag invalid chars");

        Hashtable htTagsAssignedToUser = new Hashtable();
        taghtml.Append("<div class=\"ektronTopSpace\"></div>");
        taghtml.Append("<div class=\"ektronHeader\">" + _MessageHelper.GetMessage("lbl personal tags") + "</div>");
        taghtml.Append("<div style=\"position: relative; top: 0px; left: 0px;\" >");

        taghtml.Append("    <div id=\"newTagNameDiv\" class=\"ektronWindow\">");
        taghtml.Append("        <div style=\"position: relative; top: 0px; left: 0px;\" >");
        taghtml.Append("            <div style=\"margin-top: 6px; margin-left: 10px; \" >");
        taghtml.Append("                <table class=\"ektronForm\">");
        taghtml.Append("                    <tr>");
        taghtml.Append("                        <td class=\"label\">");
        taghtml.Append(_MessageHelper.GetMessage("name label"));
        taghtml.Append("                        </td>");
        taghtml.Append("                        <td class=\"value\">");
        taghtml.Append("                            <input type=\"text\" style=\"width:275px;\" id=\"newTagName\" value=\"\" size=\"18\" onkeypress=\"if (event && event.keyCode && (13 == event.keyCode)) {SaveNewPersonalTag(); return false;}\" />");
        taghtml.Append("                        </td>");
        taghtml.Append("                    </tr>");
        taghtml.Append("                </table>");
        taghtml.Append("            </div>");

        if (IsSiteMultilingual)
        {
            //taghtml.Append("<div style=""margin-top: 6px; margin-left: 10px; "" >")
            taghtml.Append("<div style=\"display:none;\" >");
        }
        else
        {
            taghtml.Append("<div style=\"display:none;\" >");
        }
        taghtml.Append(_MessageHelper.GetMessage("res_lngsel_lbl") + "&#160;" + GetLanguageDropDownMarkup("TagLanguage"));
        taghtml.Append("    </div>");

        taghtml.Append("        <div style=\"margin-top:.5em;\">");
        taghtml.Append("            <ul class=\"buttonWrapper ui-helper-clearfix\">");
        taghtml.Append("                <li>");
        taghtml.Append("                    <a class=\"button buttonRight buttonInlineBlock redHover buttonClear\" type=\"button\" alt=\"" + _MessageHelper.GetMessage("btn cancel") + "\" title=\"" + _MessageHelper.GetMessage("btn cancel") + "\" onclick=\"CancelSaveNewPersonalTag();\">");
        taghtml.Append("                        <span>" + _MessageHelper.GetMessage("btn cancel") + "</span>");
        taghtml.Append("                    </a>");
        taghtml.Append("                </li>");

        taghtml.Append("                <li>");
        taghtml.Append("                    <a class=\"button buttonRight buttonInlineBlock greenHover buttonUpdate\" type=\"button\" title=\"" + _MessageHelper.GetMessage("btn save") + "\" alt=\"" + _MessageHelper.GetMessage("btn save") + "\" onclick=\"SaveNewPersonalTag();\" />");
        taghtml.Append("                        <span>" + _MessageHelper.GetMessage("btn save") + "</span>");
        taghtml.Append("                    </a>");
        taghtml.Append("                </li>");
        taghtml.Append("            </ul>");
        taghtml.Append("        </div>");

        taghtml.Append("        <input type=\"hidden\" id=\"newTagNameHdn\" name=\"newTagNameHdn\" value=\"\"  />");
        taghtml.Append("        </div>");
        taghtml.Append("    </div>");
        taghtml.Append("</div>");
        taghtml.Append("<div id=\"newTagNameScrollingDiv\" style=\"height: 80px; overflow: auto; border: solid 1px #aaaaaa; z-index: 1;\" >");

        LocalizationAPI localizationApi = new LocalizationAPI();

        //create hidden list of current tags so we know to delete removed ones.
        LanguageData[] languageDataArray = _SiteApi.GetAllActiveLanguages();

        foreach (LanguageData lang in languageDataArray)
        {
            taghtml.Append("<input type=\"hidden\" id=\"flag_" + lang.Id + ("\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + "\"  />"));
        }
        taghtml.Append("<input type=\"hidden\" id=\"flag_0\"  value=\"" + localizationApi.GetFlagUrlByLanguageID(-1) + "\"  />");

        TagData[] tdaUser = null;
        if (library_data != null)
        {
            if (library_data.Id > 0)
            {
                tdaUser = (new Ektron.Cms.Community.TagsAPI()).GetTagsForObject(library_data.Id, EkEnumeration.CMSObjectTypes.Library, m_refcontentapi.ContentLanguage);
            }
        }
        StringBuilder appliedTagIds = new StringBuilder();

        //build up a list of tags used by user
        //add tags to hashtable for reference later when looping through defualt tag list
        TagData td;
        if (tdaUser != null)
        {
            foreach (TagData tempLoopVar_td in tdaUser)
            {
                td = tempLoopVar_td;
                htTagsAssignedToUser.Add(td.Id, td);
                appliedTagIds.Append(td.Id.ToString() + ",");

                taghtml.Append("<input checked=\"checked\" type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                taghtml.Append("&#160;" + td.Text + "<br />");
            }
        }

        //create hidden list of current tags so we know to delete removed ones.
        taghtml.Append("<input type=\"hidden\" id=\"currentTags\" name=\"currentTags\" value=\"" + appliedTagIds.ToString() + "\"  />");

        TagData[] tdaAll;
        tdaAll = (new Ektron.Cms.Community.TagsAPI()).GetDefaultTags(EkEnumeration.CMSObjectTypes.Library, m_refcontentapi.ContentLanguage);
        if (tdaAll != null)
        {
            foreach (TagData tempLoopVar_td in tdaAll)
            {
                td = tempLoopVar_td;
                //don't add to list if its already been added with user's tags above
                if (!htTagsAssignedToUser.ContainsKey(td.Id))
                {
                    taghtml.Append("<input type=\"checkbox\" id=\"userPTagsCbx_" + td.Id.ToString() + "\" name=\"userPTagsCbx_" + td.Id.ToString() + "\" />&#160;");
                    taghtml.Append("<img src=\'" + localizationApi.GetFlagUrlByLanguageID(td.LanguageId) + "\' />");
                    taghtml.Append("&#160;" + td.Text + "<br />");
                }
            }
        }

        taghtml.Append("<div id=\"newAddedTagNamesDiv\"></div>");

        taghtml.Append("</div>");

        taghtml.Append("<div style=\"float:left;\">");
        taghtml.Append("    <a class=\"button buttonLeft greenHover buttonAddTagWithText\" href=\"#\" onclick=\"ShowAddPersonalTagArea();\">" + _MessageHelper.GetMessage("btn add personal tag") + "</a>" + "\r\n");
        taghtml.Append("</div>");

        ShowTags.Text = taghtml.ToString();

    }
    private Collection CollectMetaField()
    {
        object[] acMetaInfo = new object[4];
       string MetaSelect;
       string MetaSeparator;
       String  MetaTextString;
        int ValidCounter = 0;
        if (Request.Form["frm_validcounter"] != "")
        {
            ValidCounter = System.Convert.ToInt32(Request.Form["frm_validcounter"]);

        }
        Collection page_meta_data = new Collection();
        int i;
        for (i = 1; i <= ValidCounter; i++)
        {
            acMetaInfo[1] = Request.Form["frm_meta_type_id_" + i];
            acMetaInfo[2] = Request.Form["content_id"];
            MetaSeparator = Request.Form["MetaSeparator_" + i];
            MetaSelect = Request.Form["MetaSelect_" + i];
            if (!string.IsNullOrEmpty(MetaSelect))
            {
                MetaTextString = Strings.Replace(Request.Form["frm_text_" + i], ", ", MetaSeparator.ToString(), 1, -1, 0);
                if (Strings.Left(MetaTextString, 1).ToString() == MetaSeparator)
                {
                    MetaTextString = Strings.Right(MetaTextString.ToString(), System.Convert.ToInt32(Strings.Len(MetaTextString) - 1));
                }
                acMetaInfo[3] = MetaTextString;
            }
            else
            {
                acMetaInfo[3] = Request.Form["frm_text_" + i];
            }
            page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
            acMetaInfo = new object[4];
        }
        return (page_meta_data);
    }
    private string GetFormTeaserData()
    {
        string returnValue;
        string retVal = "";
        try
        {
            retVal = (string)cdContent_teaser.Content;

        }
        catch (Exception)
        {
            // no teaser
        }
        finally
        {
            returnValue = retVal;
        }
        return returnValue;
    }
    private void PopulateCategory(string Action)
    {
        LibraryData library_data;
        FolderData fold_data;

        m_intTaxFolderId = _FolderId;
        _EkContent = _ContentApi.EkContentRef;
        _CurrentUserID = _ContentApi.UserId;

        _PermissionData = _ContentApi.LoadPermissions(_FolderId, "content", 0);
        library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);
        fold_data = _ContentApi.GetFolderById(_FolderId);
        if (_Type == "images" || _Type == "files")
        {
            if (fold_data.CategoryRequired == true && _EkContent.GetAllFolderTaxonomy(_FolderId).Length > 0)
            {
                jsCategoryrequired.Text = "true";
            }
        }

        if (_PermissionData.IsAdmin || _EkContent.IsARoleMember(Convert.ToInt64( Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator),_CurrentUserID,false))
        {
            TaxonomyRoleExists = true;
        }
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        if (Action != "add")
        {
            taxonomy_cat_arr = _EkContent.ReadAllAssignedCategory(library_data.ContentId);
            if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
            {
                foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                {
                    if (taxonomyselectedtree.Value == "")
                    {
                        taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.TaxonomyId);
                    }
                    else
                    {
                        taxonomyselectedtree.Value = taxonomyselectedtree.Value + "," + Convert.ToString(taxonomy_cat.TaxonomyId);
                    }
                }
            }
            TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
            if (TaxonomyTreeIdList.Trim().Length > 0)
            {
                TaxonomyTreeParentIdList = _EkContent.ReadDisableNodeList(library_data.ContentId);
            }
        }
        else
        {
            if (TaxonomySelectId > 0)
            {
                taxonomyselectedtree.Value = Convert.ToString( TaxonomySelectId);
                TaxonomyTreeIdList = (string)taxonomyselectedtree.Value;
                taxonomy_cat_arr = _EkContent.GetTaxonomyRecursiveToParent(TaxonomySelectId, _EkContent.RequestInformation.ContentLanguage, 0);
                if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
                {
                    foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
                    {
                        if (TaxonomyTreeParentIdList == "")
                        {
                            TaxonomyTreeParentIdList = Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                        else
                        {
                            TaxonomyTreeParentIdList = TaxonomyTreeParentIdList + "," + Convert.ToString(taxonomy_cat.TaxonomyId);
                        }
                    }
                }
            }
        }

        TaxonomyRequest taxonomy_request = new TaxonomyRequest();
        TaxonomyBaseData[] taxonomy_data_arr = null;
        Utilities.SetLanguage(_ContentApi);
        taxonomy_request.TaxonomyId = _FolderId;
        taxonomy_request.TaxonomyLanguage = _ContentApi.ContentLanguage;
        taxonomy_data_arr = _EkContent.GetAllFolderTaxonomy(_FolderId);
        foreach (TaxonomyBaseData tax_node in taxonomy_data_arr)
        {
            _SelectedTaxonomyList = _SelectedTaxonomyList + tax_node.TaxonomyId;
            _SelectedTaxonomyList += ",";
        }
        //Hiding the Category tab if no taxonomy is applied for the folder or if user requires the category tab to be hidden
        bool HideCategoryTab = false;
        if (!string.IsNullOrEmpty(Request.QueryString["HideCategoryTab"] ))
        {
            HideCategoryTab = Convert.ToBoolean(Request.QueryString["HideCategoryTab"]);
        }
        if (HideCategoryTab || (taxonomy_data_arr == null || taxonomy_data_arr.Length == 0) && (TaxonomyOverrideId == 0))
        {
            //TODO: Ross - Not sure why, but the tab was set to non-visible in either case...odd!!
            //if (Action == "add" || _Operation == "overwrite")
            //{
                //TODO: Ross - Don't have "add" tabs yet
                phAddCategoryTab.Visible = false;
                phCategory.Visible = false;
                phCategory2.Visible = false;
            //}
        }
    }
    private void RenderSummaryEditor()
    {
        System.Text.StringBuilder retText = new System.Text.StringBuilder();

        retText.Append("<table class=\'ektronGrid\' width=\'100%\'>");
        retText.Append("	<tr>");
        retText.Append("		<td nowrap class=\'label\' style=\'text-align: left !important;\'>" + _MessageHelper.GetMessage("description label"));
        retText.Append("		</td>");
        retText.Append("	</tr>");
        retText.Append("</table>");

        EditSummaryHtml.Text = retText.ToString();
        cdContent_teaser.Content = _ContentTeaser;
        cdContent_teaser.Visible = true;
    }
    private string getPhysicalPath(string path)
    {
        try
        {
            return (Server.MapPath(path));
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + ex.Message.Replace("\r\n", " ").Replace("\'", " ")), false);
        }
        return null;
    }

    #endregion

    #region Process
    private void ValidateParam(string Param)
    {
        if ((Param != null) && ((Param.IndexOf("<") > -1) || (Param.IndexOf("%3C") > -1)))
        {
            throw (new ArgumentException("Invalid Query String Value"));
        }
    }
    private void Process_DeleteLibraryItem() //DeleteLibraryItem
    {
        try
        {
            _PageData = new Collection();
            _PageData.Add(Request.QueryString["item_id"], "LibraryID", null, null);
            _PageData.Add(Request.QueryString["parent_id"], "ParentID", null, null);
            _ContentApi.DeleteLibraryItemById(_PageData);
            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + Request.QueryString["parent_id"] + "&type=" + Request.QueryString["type"]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + ex.Message), false);
        }
    }
    private void Process_DeleteRemoveItem() //DeleteRemoveItem
    {
        string szPhysicalPath = "";
        string absolute_path = "";
        object myimagepath;
       string myImagefile;
        string sExt;
        string myNewImagefile;
        myImagefile = null;
        myimagepath = null;
        FileInfo fs;
        try
        {
            _PageData = new Collection();
            _PageData.Add(Request.QueryString["item_id"], "LibraryID", null, null);
            _PageData.Add(Request.QueryString["parent_id"], "ParentID", null, null);
            _PageData.Add(Request.Form["frm_libtype"], "LibraryType", null, null);
            _PageData.Add(EkFunctions.HtmlEncode(Request.Form["frm_title"]), "LibraryTitle", null, null);
            _PageData.Add(Request.Form["frm_filename"], "LibraryFilename", null, null);
            myNewImagefile = "";
            if (Strings.Len(Request.Form["remove"])>0)
            {
                _PageData.Add(1, "RemoveFromServer", null, null);
                if (Request.Form["frm_libtype"] == "images")
                {
                    if (Request.Form["frm_filename"].IndexOf("http") != -1 || Request.Form["frm_filename"].IndexOf("https") != -1)
                    {
                        Uri uri = new Uri(Request.Form["frm_filename"]);
                        myImagefile = Path.GetFileName(uri.AbsolutePath);
                        absolute_path = uri.AbsolutePath;
                    }
                    else
                    {
                        myImagefile = Path.GetFileName(Request.Form["frm_filename"]);
                        absolute_path = Request.Form["frm_filename"];
                    }
                    sExt = Path.GetExtension(myImagefile.ToString());
                    if (".gif" == sExt)
                    {
                        sExt = ".png";
                        myNewImagefile = "thumb_" + Path.GetFileNameWithoutExtension(absolute_path) + sExt;
                    }
                    else
                    {
                        myNewImagefile = "thumb_" + myImagefile;
                    }
                    myimagepath = Path.GetDirectoryName(absolute_path).Replace("\\", "/") + "/" + myNewImagefile;
                    fs = new FileInfo(Server.MapPath(myimagepath.ToString()));
                    if (fs.Exists)
                    {
                        fs.Delete(); //File(server.MapPath(myimagepath))
                    }
                    fs = null;
                }
            }
            else
            {
                _PageData.Add(0, "RemoveFromServer", null, null);
            }

            szPhysicalPath = getPhysicalPath("/");
            _PageData.Add(szPhysicalPath, "MapPathValue", null, null);

            LibraryConfigData lib_config_data;
            LoadBalanceData[] load_balance_data;
            load_balance_data = _ContentApi.GetAllLoadBalancePathsExtn(Convert.ToInt64( Request.Form["frm_folder_id"]), Request.Form["frm_libtype"]);
            lib_config_data = _ContentApi.GetLibrarySettings(Convert.ToInt64(Request.Form["frm_folder_id"]));
            szPhysicalPath = getPhysicalPath(lib_config_data.ImageDirectory);
            _PageData.Add(szPhysicalPath, "PhysicalImagePath", null, null);
            szPhysicalPath = getPhysicalPath(lib_config_data.FileDirectory);
            _PageData.Add(szPhysicalPath, "PhysicalFilePath", null, null);
            if (Request.Form["frm_filename"].IndexOf("http") != -1 || Request.Form["frm_filename"].IndexOf("https") != -1)
            {
                Uri uri = new Uri(Request.Form["frm_filename"]);
                _PageData.Add(Server.MapPath(uri.AbsolutePath), "MappedFilePath", null, null);
            }
            else
            {
                _PageData.Add(Server.MapPath(Request.Form["frm_filename"]), "MappedFilePath", null, null);
            }
            int i = 0;
            if (!(load_balance_data == null))
            {
                _PageData.Add(load_balance_data.Length, "LoadBalanceCount", null, null);
                if (load_balance_data.Length > 0)
                {
                    for (i = 0; i <= load_balance_data.Length - 1; i++)
                    {
                        szPhysicalPath = getPhysicalPath((string)(load_balance_data[i].Path));
                        _PageData.Add(szPhysicalPath, (string)("LoadBalancePath_" + (i + 1)), null, null);
                    }
                }
            }
            else
            {
                _PageData.Add(0, "LoadBalanceCount", null, null);
            }

            string strKey = "";
            string absPath = "";


            _ContentApi.DeleteLibraryItemByIDExt(_PageData);

            //Delete thumbnail of the image from load balance path
            if ((Ektron.Cms.Common.EkFunctions.DoesKeyExist(_PageData, "RemoveFromServer")) && (Ektron.Cms.Common.EkFunctions.DoesKeyExist(_PageData, "LibraryType")))
            {
                if ((_PageData["RemoveFromServer"].ToString() == "1") && (_PageData["LibraryType"].ToString() == "images"))
                {
                    if ((Ektron.Cms.Common.EkFunctions.DoesKeyExist(_PageData, "LoadBalanceCount")) && (!string.IsNullOrEmpty(_PageData["LoadBalanceCount"].ToString())))
                    {
                        for (i = 1; i <= Convert.ToInt32( _PageData["LoadBalanceCount"]); i++)
                        {
                            strKey = (string)("LoadBalancePath_" + i);
                            absPath = Convert.ToString( _PageData[strKey]);
                            absPath = absPath + "\\" + myNewImagefile;
                            if (!string.IsNullOrEmpty(myNewImagefile))
                            {
                                if ((!absPath.StartsWith("\\\\")))
                                {
                                    absPath = absPath.Replace("\\\\", "\\");
                                }
                                absPath = EkFunctions.HtmlDecode(absPath);
                                if (File.Exists(absPath))
                                {
                                    FileSystem.Kill(absPath);
                                }
                            }
                        }
                    }
                }
            }

            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + Request.Form["frm_folder_id"] + "&type=" + Request.Form["frm_libtype"]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + ex.Message), false);
        }
    }
    private void ProcessTags(long Id, int langId)
    {
        TagData[] defaultTags;
        TagData[] Tags;
        Ektron.Cms.API.Community.Tags m_refTagsApi = new Ektron.Cms.API.Community.Tags();
        string orginalTagIds;
        string tagIdStr = "";
        string cTags = Page.Request.Form["currentTags"];
        if (cTags != null)
        {
            orginalTagIds = (string)(cTags.Trim().ToLower());
        }
        else
        {
            orginalTagIds = "";
        }
        //Assign all default user tags that are checked:
        //Remove tags that have been unchecked
        defaultTags = m_refTagsApi.GetDefaultTags(EkEnumeration.CMSObjectTypes.Library, -1);
        Tags = m_refTagsApi.GetTagsForObject(Id, EkEnumeration.CMSObjectTypes.Library);

        //Also, copy all users tags into defaultTags list
        //so that if they were removed, they can be deleted as well.
        int originalLength = defaultTags.Length;
        Array.Resize(ref defaultTags, defaultTags.Length + Tags.Length - 1 + 1);
        Tags.CopyTo(defaultTags, originalLength);

        if (defaultTags != null)
        {

            foreach (TagData td in defaultTags)
            {
                tagIdStr = (string)("userPTagsCbx_" + td.Id.ToString());
                if (!(Page.Request.Form[tagIdStr] == null))
                {
                    if (Page.Request.Form[tagIdStr] == "on")
                    {
                        //if tag is checked, but not in current tag list, add it
                        if (!orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.AddTagToObject(td.Id, Id, EkEnumeration.CMSObjectTypes.Library, -1, langId);
                        }
                    }
                    else
                    {
                        //if tag is unchecked AND in current list, delete
                        if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                        {
                            m_refTagsApi.DeleteTagOnObject(td.Id, Id, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Library, 0);
                        }
                    }
                }
                else
                {
                    //If tag checkbox has no postback value AND is in current tag list, delete it
                    if (orginalTagIds.Contains((string)(td.Id.ToString() + ",")))
                    {
                        m_refTagsApi.DeleteTagOnObject(td.Id, Id, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Library, 0);
                    }
                }
            }

            // Now add any new custom tags, that the user created:
            // New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>

            if (Page.Request["newTagNameHdn"] != null)
            {
                string custTags = Page.Request["newTagNameHdn"].ToString();
                char[] tagsep = new char[] { ';' };
                string[] aCustTags = custTags.Split(tagsep);

                int languageId;
                char[] langsep = new char[] { '~' };

                foreach (string tag in aCustTags)
                {
                    string[] tagPropArray = tag.Split(langsep);
                    if (tagPropArray.Length > 1)
                    {
                        if (tagPropArray[0].Trim().Length > 0)
                        {
                            //Default language to -1.
                            //"ALL" option in drop down is 0 - switch to -1.
                            if (!int.TryParse(Convert.ToString( tagPropArray[1]),out  languageId))
                            {
                                languageId = -1;
                            }
                            if (languageId == 0)
                            {
                                languageId = -1;
                            }

                            m_refTagsApi.AddTagToObject(tagPropArray[0], Id, EkEnumeration.CMSObjectTypes.Library, -1, languageId);
                        }
                    }
                }
            }
        }
    }
    private void Process_AddLibraryItem()
    {
        PermissionData security_data;
        //Dim folder_data As FolderData
        LibraryConfigData lib_setting_data;
        Collection pagedata;
        string strOperation = "";
        string strLibType = "";
        string strContentId = "";
        long CurrentUserID = 0;
        Ektron.Cms.Library.EkLibrary m_refLibrary;
        string FileName = "";
        string NewFilename = "";
        string MediaPath = "";
        string Extensions = "";
       string[] FilenameArray;
        bool ret = false;
        bool uploadok = false;
      string[] ExtensionArray; //, gtLibraries
       int iLoop;
        bool bThumbnail = true;
        object szPhysicalPath; // ,ThumbLBString
       string actErrorString;

        CurrentUserID = _ContentApi.UserId;
        m_refLibrary = _ContentApi.EkLibraryRef;

        Ektron.Cms.DataIO.EkLibraryRW dataLibObj;
        dataLibObj = new Ektron.Cms.DataIO.EkContentRW(_ContentApi.RequestInformationRef);
        Collection cItemInfo = new Collection();

        try
        {
            if ((_Type == "images") || (_Type == "files"))
            {

                int fileLength = frm_filename.PostedFile.ContentLength;
                byte[] fileData = new byte[fileLength];
                frm_filename.PostedFile.InputStream.Read(fileData, 0, fileLength);
                Stream stream = new MemoryStream(fileData);
                if (EkFunctions.IsImage(Path.GetExtension(frm_filename.PostedFile.FileName)))
                {
                    if (!EkFunctions.isImageStreamValid(stream))
                    {
                        throw new Exception("The image is corrupted or not in correct format.");
                    }
                }

                _FolderId = Convert.ToInt64(Request.Form["frm_folder_id"]);
                strLibType = Request.Form["frm_libtype"];
                strOperation = Request.Form["frm_operation"];
                FileName = (string)(frm_filename.PostedFile.FileName.Substring((frm_filename.PostedFile.FileName).LastIndexOf("\\") + 1));
                FileName = FileName.Replace("%", "");
                lib_setting_data = _ContentApi.GetLibrarySettings(_FolderId);
                security_data = _ContentApi.LoadPermissions(_FolderId, "folder", 0);
                strContentId = Request.Form["frm_content_id"];
                pagedata = new Collection();
                pagedata.Add(_FolderId, "ParentID", null, null);
                pagedata.Add(Request.Form["frm_library_id"], "LibraryID", null, null);
                pagedata.Add(strLibType, "LibraryType", null, null);
                pagedata.Add(Request.Form["frm_title"], "LibraryTitle", null, null);
                pagedata.Add(strContentId, "ContentID", null, null);
                NewFilename = Strings.Trim(Request.Form["frm_oldfilename"]);


                if (((strOperation.ToLower() != "overwrite") && (((security_data.CanAddToImageLib) && (strLibType == "images")) || ((security_data.CanAddToFileLib) && (strLibType == "files")) || ((security_data.CanAddToHyperlinkLib) && (strLibType == "hyperlinks")) || ((security_data.CanAddToQuicklinkLib) && (strLibType == "quicklinks")))) || ((security_data.CanOverwriteLib) && (strOperation.ToLower() == "overwrite")))
                {
                    pagedata.Add(CurrentUserID, "UserID", null, null);
                    FilenameArray = FileName.Split('.');
                    if (strLibType == "images")
                    {
                        Extensions = lib_setting_data.ImageExtensions;
                        MediaPath = lib_setting_data.ImageDirectory;
                    }
                    else
                    {
                        Extensions = lib_setting_data.FileExtensions;
                        MediaPath = lib_setting_data.FileDirectory;
                    }
                    if ((FilenameArray.Length - 1) >= 1)
                    {
                        ExtensionArray = Extensions.Split(',');
                        for (iLoop = 0; iLoop <= (ExtensionArray.Length - 1); iLoop++)
                        {
                            if (Strings.LCase(FilenameArray[FilenameArray.Length - 1]).Trim() == Strings.LCase((string)(ExtensionArray[iLoop])).Trim())
                            {
                                uploadok = true;
                                break;
                            }
                        }
                        if (uploadok == true)
                        {
                            if (strOperation.ToLower() == "overwrite")
                            {
                                pagedata.Add("OverwriteLib", "UpdateLibData", null, null);
                                strOperation = "overwrite";
                            }
                            else
                            {
                                strOperation = "makeunique";
                            }

                            szPhysicalPath = Server.MapPath(MediaPath);
                            if (!Directory.Exists(szPhysicalPath.ToString()))
                            {
                                Directory.CreateDirectory(szPhysicalPath.ToString());
                            }

                            if (strOperation == "overwrite")
                            {
                                actErrorString = NewFilename;
                            }
                            else
                            {
                                actErrorString = FileName;
                            }

                            string strTmpFilename;
                            string strTmpFileExtn;
                            int iUnqueNameIdentifier =0 ;
                            FileInfo CheckFile;
                            actErrorString = Strings.Replace(actErrorString.ToString(), "/", "\\", 1, -1, 0);
                            string[] st = Strings.Split(actErrorString.ToString(), "\\", -1, 0);
                            strTmpFilename = st[st.Length-1];
                            if ((Strings.Right(szPhysicalPath.ToString(), 1) != "\\"))
                            {
                                szPhysicalPath = szPhysicalPath + "\\";
                            }
                        
                            actErrorString = strTmpFilename;
                            strTmpFileExtn = actErrorString.Substring(actErrorString.LastIndexOf("."));
                            strTmpFilename = Strings.Replace(actErrorString.ToString(), strTmpFileExtn.ToString(), "", 1, -1, 0);
                            if (strOperation == "makeunique")
                            {
                                CheckFile = new FileInfo(szPhysicalPath + actErrorString);
                                if (CheckFile.Exists)
                                {
                                    while (CheckFile.Exists)
                                    {
                                        iUnqueNameIdentifier = iUnqueNameIdentifier + 1;
                                        actErrorString = strTmpFilename + "(" + iUnqueNameIdentifier + ")" + strTmpFileExtn;
                                        CheckFile = new FileInfo(szPhysicalPath + actErrorString);
                                    }
                                }
                            }

                            if (!Directory.Exists(szPhysicalPath.ToString()))
                            {
                                Directory.CreateDirectory(szPhysicalPath.ToString());
                            }

                            try
                            {
                                if ((strOperation.ToLower() != "overwrite") && (strLibType == "images" || strLibType == "files"))
                                {
                                    cItemInfo = dataLibObj.GetChildLibraryItemByTitlev2_0(Convert.ToString( Request.Form["frm_title"]), _FolderId, strLibType,Convert.ToInt32( EkEnumeration.CMSContentType.LibraryItem));
                                    if ((cItemInfo.Count > 0) && (Convert.ToInt32( cItemInfo["ContentLanguage"]) == _ContentApi.RequestInformationRef.ContentLanguage))
                                    {
                                        Utilities.ShowError(_MessageHelper.GetMessage("com: library entry already exists"));
                                        return;
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }

                            frm_filename.PostedFile.SaveAs(szPhysicalPath + actErrorString);

                            if (strLibType == "images")
                            {
                                try
                                {
                                    Utilities.ProcessThumbnail(Server.MapPath(MediaPath), actErrorString.ToString());
                                }
                                catch (Exception)
                                {
                                    bThumbnail = false;
                                }
                            }
                            else
                            {
                                bThumbnail = false;
                            }
                            //----------------- Load Balance ------------------------------------------------------
                            LoadBalanceData[] loadbalance_data;
                            int i = 0;
                            loadbalance_data = _ContentApi.GetAllLoadBalancePathsExtn(_FolderId, strLibType);
                            if (!(loadbalance_data == null))
                            {
                                for (i = 0; i <= loadbalance_data.Length - 1; i++)
                                {
                                    szPhysicalPath = Server.MapPath(loadbalance_data[i].Path);
                                    if ((Strings.Right(szPhysicalPath.ToString(), 1) != "\\"))
                                    {
                                        szPhysicalPath = szPhysicalPath + "\\";
                                    }
                                    frm_filename.PostedFile.SaveAs(szPhysicalPath + actErrorString);
                                    if (bThumbnail)
                                    {
                                        Utilities.ProcessThumbnail(szPhysicalPath.ToString(), actErrorString.ToString());
                                    }
                                   
                                }
                            }
                            pagedata.Add(MediaPath + actErrorString, "LibraryFilename", null, null);
                        }
                        else
                        {
                            throw (new Exception(_MessageHelper.GetMessage("error: invalid extension")));
                        }
                    }
                    else
                    {
                        throw (new Exception(_MessageHelper.GetMessage("error: invalid extension")));
                    }
                }
                else
                {
                    throw (new Exception(_MessageHelper.GetMessage("com: user does not have permission")));
                }
            }
            else
            {
                pagedata = new Collection();
                pagedata.Add(Request.Form["frm_folder_id"], "ParentID", null, null);
                pagedata.Add(Request.Form["frm_libtype"], "LibraryType", null, null);
                pagedata.Add(Request.Form["frm_title"], "LibraryTitle", null, null);
                pagedata.Add(Request.Form["frm_filename"], "LibraryFilename", null, null);
                pagedata.Add(Request.Form["frm_content_id"], "ContentID", null, null);
                pagedata.Add(_ContentLanguage, "ContentLanguage", null, null);
                pagedata.Add(CurrentUserID, "UserID", null, null);
            }
            if (_Type != "quicklinks" && _Type != "forms")
            {
                pagedata.Add(GetFormTeaserData(), "ContentTeaser", null, null);
                pagedata.Add(CollectMetaField(), "ContentMetadata", null, null);
            }

            //Adding the Taxonomy category info
            if ((Request.Form["TaxonomyOverrideId"] != null) && Convert.ToInt64(Request.Form["TaxonomyOverrideId"]) != 0)
            {
                TaxonomyOverrideId = Convert.ToInt64( Request.Form["TaxonomyOverrideId"]);
                TaxonomyTreeIdList = Convert.ToString( TaxonomyOverrideId);
            }

            if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
            {
                TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                if (TaxonomyTreeIdList.Trim().EndsWith(","))
                {
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                }
            }
            if (TaxonomyTreeIdList.Trim() == string.Empty && TaxonomySelectId > 0)
            {
                TaxonomyTreeIdList = Convert.ToString( TaxonomySelectId);
            }
            pagedata.Add(TaxonomyTreeIdList, "Taxonomy", null, null);
            ///'

            if (strOperation == "overwrite")
            {
                ret = m_refLibrary.UpdateLibraryItemByIDv2_0(pagedata, 0, -1);
                FolderData folder_data = _ContentApi.GetFolderById(_FolderId);
                if (folder_data.FolderType == Convert.ToInt32( EkEnumeration.FolderType.Catalog))
                {
                    LibraryData library_data = null;
                    library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);
                    FileName = library_data.FileName.Substring(System.Convert.ToInt32((library_data.FileName).LastIndexOf("/") + 1));
                    Ektron.Cms.Commerce.ProductTypeData productTypeData = new Ektron.Cms.Commerce.ProductTypeData();
                    Ektron.Cms.Commerce.ProductType productType = new Ektron.Cms.Commerce.ProductType(_ContentApi.RequestInformationRef);
                    long xmlConfigurationId = folder_data.XmlConfiguration[0].Id;

                    productTypeData = productType.GetItem(xmlConfigurationId);
                    szPhysicalPath = Server.MapPath(MediaPath);

                    if (productTypeData.DefaultThumbnails.Count > 0)
                    {
                        EkFileIO thumbnailCreator = new EkFileIO();
                        bool thumbnailResult = false;
                        string sourceFile = (string)(szPhysicalPath + FileName);
                        foreach (ThumbnailDefaultData thumbnail in productTypeData.DefaultThumbnails)
                        {
                            string fileNameNoExtension = FileName.Replace(System.IO.Path.GetExtension(FileName), "");
                            string fileNameExtension = System.IO.Path.GetExtension(FileName);
                            string thumbnailFile = (string)(szPhysicalPath + "\\" + fileNameNoExtension + thumbnail.Title + fileNameExtension);
                            thumbnailResult = thumbnailCreator.CreateThumbnail(sourceFile, thumbnailFile, thumbnail.Width, thumbnail.Height);
                        }
                    }
                    LibraryData librarydata = new LibraryData();
                    int iThumbnail = 0;
                    if (productTypeData.DefaultThumbnails.Count > 0)
                    {
                        string sourceFile = (string)(szPhysicalPath + FileName);
                        foreach (ThumbnailDefaultData thumbnail in productTypeData.DefaultThumbnails)
                        {
                            iThumbnail++;
                            string fileNameNoExtension = FileName.Replace(System.IO.Path.GetExtension(FileName), "");
                            string fileNameExtension = System.IO.Path.GetExtension(FileName);
                            string thumbnailPath = library_data.FileName.Replace(System.IO.Path.GetFileName(library_data.FileName), "");
                            string thumbnailFile = fileNameNoExtension + thumbnail.Title + fileNameExtension;
                            NewFilename = Strings.Trim(Request.Form["frm_oldfilename"]);

                            librarydata =  new LibraryData();
                            librarydata.Type = "images";
                            librarydata.FileName = thumbnailPath + thumbnailFile;
                            librarydata.Title = library_data.Title + thumbnail.Title;
                            librarydata.ParentId = _FolderId;
                            librarydata.Id = _Id + iThumbnail;
                            actErrorString = NewFilename;

                            Collection ThumbnailData = new Collection();
                            ThumbnailData.Add(librarydata.ParentId, "ParentID", null, null);
                            ThumbnailData.Add(librarydata.Id, "LibraryID", null, null);
                            ThumbnailData.Add(librarydata.Type, "LibraryType", null, null);
                            ThumbnailData.Add(librarydata.Title, "LibraryTitle", null, null);
                            ThumbnailData.Add(strContentId, "ContentID", null, null);
                            ThumbnailData.Add(CurrentUserID, "UserID", null, null);
                            ThumbnailData.Add("OverwriteLib", "UpdateLibData", null, null);
                            ThumbnailData.Add(librarydata.FileName, "LibraryFilename", null, null);
                            ThumbnailData.Add(librarydata.OriginalLibraryId, "OriginalLibraryId", null, null);

                            _ContentApi.UpdateLibraryItemByID(ThumbnailData);
                        }
                    }

                }
            }
            else
            {
                if (_ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                {
                    _ContentApi.ContentLanguage = _ContentApi.DefaultContentLanguage;
                }
                ret = m_refLibrary.AddLibraryItemv2_0(pagedata, 0);

                // process tag info
                ProcessTags(Convert.ToInt64( pagedata["LibraryID"]), _ContentApi.ContentLanguage);
            }

            if (strOperation.ToLower() == "overwrite")
            {
                Response.Redirect("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryItem&id=" + pagedata["LibraryID"] + "&parent_id=" + pagedata["ParentID"] + "&reload=true", false);
            }
            else
            {
                Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + _FolderId + "&type=" + strLibType), false);
            }
        }
        catch (Exception ex)
        {
            EkException.ThrowException(ex);
        }
    }

    private void Process_UpdateLibraryItem() //UpdateLibraryItem
    {
        try
        {
            LibraryData library_data;

            _Id = Convert.ToInt64(Request.QueryString["id"]);
            _FolderId = Convert.ToInt64(Request.QueryString["parent_id"]);
            library_data = _ContentApi.GetLibraryItemByID(_Id, _FolderId);

            _PageData = new Collection();
            _PageData.Add(Request.QueryString["id"], "LibraryID", null, null); //Request.Form("frm_item_id"), "LibraryID")
            _PageData.Add(Request.QueryString["parent_id"], "ParentID", null, null); //Request.Form("frm_folder_id"), "ParentID")
            _PageData.Add(Request.Form["frm_libtype"], "LibraryType", null, null);
            _PageData.Add(Request.Form["frm_title"], "LibraryTitle", null, null);

            if ((library_data.Type == "files" || library_data.Type == "images") && _PageAction == "editlibraryitem")
            {
                if (library_data.Type == "images" || library_data.Type == "files")
                {
                    if (library_data.FileName.IndexOf("http://") == 0)
                    {
                        FolderData folder_data = _ContentApi.GetFolderById(_FolderId);
                        string temp_str = library_data.FileName;
                        // If (m_refContApi.RequestInformationRef.IsStaging) Then
                        //   temp_str = temp_str.Replace("http://" & folder_data.DomainStaging, "")
                        //  Else
                        temp_str = temp_str.Replace((string)("http://" + folder_data.DomainProduction), "");
                        //  End If
                        string temp_url = temp_str;
                        int start_index = temp_str.IndexOf(_ContentApi.RequestInformationRef.SitePath);
                        if (start_index > -1)
                        {
                            int string_length = temp_str.Length - temp_str.IndexOf(_ContentApi.RequestInformationRef.SitePath);
                            temp_url = temp_str.Substring(start_index, string_length);
                        }
                        library_data.FileName = temp_url;
                    }

                    // process tag info
                    ProcessTags(_Id, _ContentLanguage);
                }
                _PageData.Add(library_data.FileName, "LibraryFilename", null, null);

            }
            else if (library_data.Type == "quicklinks" && library_data.ContentType != 1111 && ((Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= library_data.ContentType && library_data.ContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) || (Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Min <= library_data.ContentType && library_data.ContentType <= Ektron.Cms.Common.EkConstants.Archive_ManagedAsset_Max)))
            {
                _PageData.Add(library_data.FileName, "LibraryFilename", null, null);

            }
            else
            {
                _PageData.Add(Request.Form["frm_filename"], "LibraryFilename", null, null);
                _PageData.Add("true", "LockedContentLink", null, null);
            }

            _PageData.Add(Request.Form["frm_content_id"], "ContentID", null, null);

            if (library_data.Type == "quicklinks")
            {
                ContentData content_data;
                content_data = _ContentApi.GetContentById(Convert.ToInt64( Request.Form["frm_content_id"]), 0);
                _PageData.Add(content_data.Type, "ContentContentType", null, null);
            }


            _PageData.Add(library_data.ContentType, "ContentType", null, null);
            if (_ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
            {
                _PageData.Add(_ContentApi.DefaultContentLanguage, "ContentLanguage", null, null);
            }
            else
            {
                _PageData.Add(_ContentLanguage, "ContentLanguage", null, null);
            }

            if (_Type != "quicklinks" && _Type != "forms")
            {
                _PageData.Add(GetFormTeaserData(), "ContentTeaser", null, null);
                _PageData.Add(CollectMetaField(), "ContentMetadata", null, null);
            }

            //Adding the Taxonomy category info
            if ((Request.Form["TaxonomyOverrideId"] != null) && Convert.ToInt64( Request.Form["TaxonomyOverrideId"] )!= 0)
            {
                TaxonomyOverrideId =  Convert.ToInt64(Request.Form["TaxonomyOverrideId"]);
                TaxonomyTreeIdList =  Convert.ToString( TaxonomyOverrideId);
            }

            if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
            {
                TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                if (TaxonomyTreeIdList.Trim().EndsWith(","))
                {
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                }
            }
            if (TaxonomyTreeIdList.Trim() == string.Empty && TaxonomySelectId > 0)
            {
                TaxonomyTreeIdList = Convert.ToString( TaxonomySelectId);
            }
            _PageData.Add(TaxonomyTreeIdList, "Taxonomy", null, null);
            ///'
            _ContentApi.UpdateLibraryItemByID(_PageData);
            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + Request.QueryString["parent_id"] + "&type=" + Request.Form["frm_libtype"]), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + ex.Message), false);
        }
    }
    private void Process_AddOrUpdateLoadBalanceSettings() //(action = "UpdateLoadBalanceSettings") Or (action = "AddLoadBalanceSettings")
    {
        object newType; //, test1
        string MediaPath = "";
        string szPhysicalPath = "";
        try
        {
            if ((Strings.LCase(Request.ServerVariables["http_host"])).IndexOf("demo.ektron.com") + 1 == 0)
            {
                _PageData = new Collection();
                MediaPath = Request.Form["loadBalancePath"];
                if (MediaPath.Substring(MediaPath.Length - 1, 1) == "/")
                {
                    MediaPath = MediaPath.Substring(0, MediaPath.Length - 1);
                }
                if (Strings.Len(Request.Form["MakeRelative"]) == 0)
                {
                    szPhysicalPath = getPhysicalPath(MediaPath);
                    _PageData.Add(szPhysicalPath, "MapPathValue", null, null);
                    _PageData.Add(0, "MakeRelative", null, null);
                }
                else
                {
                    if (MediaPath.Substring(0, 1) == "/")
                    {
                        MediaPath = MediaPath.Substring(1, MediaPath.Length - 1);
                    }
                    szPhysicalPath = getPhysicalPath(_SitePath + MediaPath);
                    _PageData.Add(szPhysicalPath, "MapPathValue", null, null);
                    _PageData.Add(1, "MakeRelative", null, null);
                }
                _PageData.Add(MediaPath, "LoadBalancePath", null, null);
                if (_PageAction == "addloadbalance")
                {
                    newType = Request.Form[AssetType.UniqueID];
                    _ContentApi.AddLoadBalanceItem(newType.ToString(), _PageData);
                }
                else
                {
                    _ContentApi.UpdateLoadBalanceSettingsByID(Request.Form["loadBalanceID"], _PageData);
                }
            }
            else
            {
                throw (new Exception(_MessageHelper.GetMessage("js: alert demo.ektron.com detected")));
            }
            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLoadBalance&id=" + _Id), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + ex.Message), false);
        }
    }
    private void Process_RemoveLoadBalance() //RemoveLoadBalance
    {
        int lbCount = 0;
        int lLoop = 0;
        string tmp;
       string lb_id;
        try
        {
            if ((Strings.LCase(Request.ServerVariables["http_host"])).IndexOf("demo.ektron.com") + 1 == 0)
            {
                lbCount = Convert.ToInt32(Request.Form["lbPathCount"]);
                if (lbCount>0)
                {
                    for (lLoop = 1; lLoop <= lbCount; lLoop++)
                    {
                        tmp = "loadBalanceID_" + lLoop;
                        lb_id = Request.Form[tmp];
                        tmp = "MakeRelative_" + lLoop;
                        if (Strings.Len(Request.Form[tmp]) != 0) //check to see if the check box is checked
                        {
                            _ContentApi.DeleteLoadBalanceItemByID(Convert.ToInt64( lb_id));
                        }
                    }
                }
            }
            else
            {
                throw (new Exception(_MessageHelper.GetMessage("js: alert demo.ektron.com detected")));
            }
            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLoadBalance&id=" + _Id), false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }
    private void Process_UpdateLibrarySettings() //UpdateLibrarySettings
    {
        try
        {
            if ((Strings.LCase(Request.ServerVariables["http_host"])).IndexOf("demo.ektron.com") + 1 == 0)
            {
                object tmpPath;

                _PageData = new Collection();
                _PageData.Add(Request.Form["fileextensions"], "FileExtensions", null, null);
                _PageData.Add(Request.Form["imageextensions"], "ImageExtensions", null, null);
                _PageData.Add(Request.Form["filedirectory"], "FileDirectory", null, null);
                _PageData.Add(Request.Form["imagedirectory"], "ImageDirectory", null, null);
                _PageData.Add(Request.Form["relativefiles"], "RelativeFiles", null, null);
                _PageData.Add(Request.Form["relativeimages"], "RelativeImages", null, null);
                _PageData.Add(Request.Form["id"], "FolderID", null, null);
                if ((_PageData["RelativeImages"].ToString() == "true") || (_PageData["RelativeImages"].ToString() == "on"))
                {
                    tmpPath = _SitePath + Request.Form["imagedirectory"].Replace("//", "/");
                    _PageData.Add(getPhysicalPath(tmpPath.ToString()), "AbsImageDirectory", null, null);
                }
                else
                {
                    tmpPath = "/" + Request.Form["imagedirectory"].Replace("//", "/");
                    _PageData.Add(getPhysicalPath(tmpPath.ToString()), "AbsImageDirectory", null, null);
                }
                if (_PageData["RelativeFiles"].ToString() == "true" || _PageData["RelativeFiles"].ToString() == "on")
                {
                    tmpPath = _SitePath + Request.Form["filedirectory"].Replace("//", "/");
                    _PageData.Add(getPhysicalPath(tmpPath.ToString()), "AbsFileDirectory", null, null);
                }
                else
                {
                    tmpPath = "/" + Request.Form["filedirectory"].Replace("//", "/");
                    //tmpPath = Strings.Replace("/" + Request.Form["filedirectory"], "//", "/",1,-1,CompareMethod.Binary);
                    _PageData.Add(getPhysicalPath(tmpPath.ToString()), "AbsFileDirectory", null, null);
                 
                }
                _ContentApi.UpdateLibrarySettings(_PageData);
            }
            else
            {
                throw (new Exception(_MessageHelper.GetMessage("js: alert demo.ektron.com detected")));
            }
            Response.Redirect((string)("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibrarySettings&id=" + _Id), false);
        }
        catch (Exception)
        {
            string strMsg;
            strMsg = _MessageHelper.GetMessage("com: undefined folder path");

           
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + strMsg), false);
           
        }
    }
    private void Process_UpdateQLinkTemplateByCategory() //UpdateQlinkTemplateByCategory
    {
        string LibIds = "";
        object folderid;
        string[] arrArray;
       int arrCount;
        try
        {
            folderid = Request.Form["folder_id"];
            arrArray = Strings.Split(Request.Form["libids"], ",", -1, 0);
            for (arrCount = 0; arrCount <= (arrArray.Length - 1); arrCount++)
            {
                if (Convert.ToString( Request.Form["id_" + arrArray[arrCount]] )== "on")
                {
                    LibIds = LibIds + arrArray[arrCount] + ",";
                }
            }
            if (LibIds != "")
            {
                LibIds = LibIds.Substring(0, LibIds.Length - 1);
            }
            else
            {
                LibIds = "";
            }
            _PageData = new Collection();
            _PageData.Add(LibIds, "LibIDs", null, null);
            _PageData.Add(folderid, "ParentID", null, null);
            _PageData.Add(Request.Form["template_from"], "TemplateFrom", null, null);
            _PageData.Add(Request.Form["template_to"], "TemplateTo", null, null);
            _PageData.Add(true, "FixLinks", null, null);
            _ContentApi.UpdateQlinkTemplates(_PageData);
            Response.Redirect("library.aspx?LangType=" + _ContentLanguage + "&action=ViewLibraryByCategory&id=" + folderid + "&type=quicklinks", false);
        }
        catch (Exception ex)
        {
            Response.Redirect((string)("reterror.aspx?LangType=" + _ContentLanguage + "&info=" + EkFunctions.UrlEncode(ex.Message)), false);
        }
    }

    #endregion

    #region JS/CSS

    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.ApplicationPath + "Tree/css/com.ektron.ui.tree.css", "EktronUITreeCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, _ContentApi.AppPath + "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCss");


        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
	Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.init.js", "EktronExplorerInitJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/searchfuncsupport.js", "EktronSearchFuncSupportJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/optiontransfer.js", "EktronOptionTransferJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.url.js", "EktronUtilsUrlJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.js", "EktronExplorerJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.config.js", "EktronExplorerConfigJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.explorer.windows.js", "EktronExplorerWindowsJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.cms.types.js", "EktronCmsTypesJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.cms.parser.js", "EktronCmsParserJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.cms.toolkit.js", "EktronCmsToolkitJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.cms.api.js", "EktronCmsApiJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.ui.contextmenu.js", "EktronUIContentmenuJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.ui.iconlist.js", "EktronUIIconlistJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.ui.explore.js", "EktronUIExplorerJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.ui.taxonomytree.js", "EktronUITaxonomytreeJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.net.http.js", "EktronNetHttpJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.lang.exception.js", "EktronLangExceptionJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.form.js", "EktronUtilsFormJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.log.js", "EktronUtilsLogJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.dom.js", "EktronUtilsDomJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.debug.js", "EktronUtilsDebugJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.string.js", "EktronUtilsStringJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.cookie.js", "EktronUtilsCookieJS");
        Ektron.Cms.API.JS.RegisterJS(this, _ContentApi.ApplicationPath + "Tree/js/com.ektron.utils.querystring.js", "EktronUtilsQuerystringJS");
    }

    #endregion

}


