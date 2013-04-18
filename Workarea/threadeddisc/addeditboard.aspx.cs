using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Content;
using Ektron.Cms.Workarea;
using Microsoft.VisualBasic;

public partial class threadeddisc_addeditboard : workareabase
{

    #region Member Variables

    protected ContentDesignerWithValidator _Editor;
    protected CommonApi _cApi = new CommonApi();
    protected EkContent _EkContentRef;
    protected DiscussionCategory[] _Categories;
    protected DiscussionBoard _DiscussionBoard = new Ektron.Cms.DiscussionBoard();
    protected PermissionData _PermissionData;
    protected long _FolderId = 0;
    protected long _CategoryId = 0;
    protected string _SelectedTaxonomyList = "";
    protected string _SelectedTaxonomyParentList = "";
    protected int _CurrentCategoryChecked = 0;
    protected int _ParentCategoryChecked = 0;
    protected FolderData _FolderData = null;
    protected bool _IsTaxonomyUiEnabled = true;
    protected string _OldTemplateName = "";
    protected string _NewTemplateName = "";
    protected long _NewTemplateId = 0;
    protected bool usesModal = false;
    protected long _GroupID = -1;
    private long _CheckTaxId = 0;
    private string _SitePath;
    private string _ApplicationPath;
    private bool _editMode = false;

    #endregion

    #region Properties

    public string SitePath
    {
        get
        {
            return _SitePath;
        }
        set
        {
            _SitePath = value;
        }
    }

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

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        char[] endSlash = new char[] { '/' };
        this.ApplicationPath = m_refContentApi.ApplicationPath.TrimEnd(endSlash.ToString().ToCharArray());
        this.SitePath = m_refContentApi.SitePath.TrimEnd(endSlash.ToString().ToCharArray());
        if ((Request.QueryString["thickbox"] != null) && Request.QueryString["thickbox"] == "true")
        {
            usesModal = true;
        }

        //Register Page Components
        this.RegisterCSS();
        this.RegisterJS();
        _EkContentRef = new EkContent(_cApi.RequestInformationRef);

        _Editor = (ContentDesignerWithValidator)LoadControl(m_refContentApi.ApplicationPath + "controls/Editor/ContentDesignerWithValidator.ascx");
        _Editor.Width = System.Web.UI.WebControls.Unit.Pixel(750);
        _Editor.Visible = false;
        _Editor.ID = "content_html";
        pnlTerms.Controls.Add(_Editor);
        ((Ektron.ContentDesignerWithValidator)(pnlTerms.Controls[0])).ToolsFile = m_refContentApi.ApplicationPath + "ContentDesigner/configurations/InterfaceBlog.aspx?Translate=False&EmoticonSelect=1&WMV=1";
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            if (!usesModal)
            {
                ShowDropUploader(false);
            }

            if (Request.QueryString["catid"] != "")
            {
                _CategoryId = Convert.ToInt64(Request.QueryString["catid"]);
            }
            if (Request.QueryString["groupId"] != "")
            {
                _GroupID = Convert.ToInt64(Request.QueryString["groupId"]);
            }
            if (!(Page.IsPostBack))
            {
                ltr_adb_ra.Text = m_refMsg.GetMessage("lbl require authentication");
                ltr_adb_mc.Text = m_refMsg.GetMessage("lbl moderate comments");
                if (m_sPageAction == "editprop")
                {
                    _editMode = true;

                    if (_EkContentRef.RequestInformation.IsMembershipUser == 1 && _EkContentRef.IsAllowed(m_iID, 0, "folder", "EditFolders", 0) == false)
                    {
                        throw (new Exception(base.GetMessage("com: user does not have permission")));
                    }
                    base.SetTitleBarToMessage("edit board prop title");

					if (!usesModal)
					{
						base.AddBackButton((string)("addeditboard.aspx?action=View&id=" + m_iID.ToString()));
					}
					
					base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "lbl alt edit board", "lbl save board", " onclick=\"javascript:return CheckDiscussionBoardParameters();\" ", StyleHelper.SaveButtonCssClass, true);
                    if (usesModal)
                    {
                        base.AddButtonwithMessages(AppImgPath + "../UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.RemoveButtonCssClass);
                    }

                    base.AddHelpButton("EditBoard");


                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    txt_adb_boardname.Text = _DiscussionBoard.Name;
                    hdn_adb_boardname.Value = _DiscussionBoard.Name;
                    ltr_boardid_data.Text = _DiscussionBoard.Id.ToString();
                    txt_adb_title.Text = _DiscussionBoard.Description;
                    ltr_acceptedhtml.Text = base.GetMessage("lbl acceptedhtml");
                    txt_acceptedhtml.Text = _DiscussionBoard.AcceptedHTML;
                    ltr_acceptedextensions.Text = base.GetMessage("lbl acceptedextensions");
                    txt_acceptedextensions.Text = _DiscussionBoard.AcceptedExtensions;
                    chk_adb_mc.Checked = _DiscussionBoard.ModerateComments;
                    chk_adb_ra.Checked = _DiscussionBoard.RequireAuthentication;
                    chk_lock_board.Checked = _DiscussionBoard.LockBoard;
                    txt_adb_stylesheet.Text = _DiscussionBoard.StyleSheet;
                    ltr_sitepath.Text = m_refContentApi.SitePath;
                    ltr_maxfilesize.Text = base.GetMessage("lbl max file size");
                    txt_maxfilesize.Text = _DiscussionBoard.MaxFileSize.ToString();
                    ltr_bytes.Text = base.GetMessage("lbl bytes");
                    ltr_comma_ext.Text = base.GetMessage("lbl comma sep");
                    ltr_comma_html.Text = base.GetMessage("lbl comma sep");
                    drp_theme.Attributes.Add("onchange", "javascript:updatetheme();");

                    _PermissionData = m_refContentApi.LoadPermissions(m_iID, "folder", 0);
                    _Editor.ID = "content_html";
                    _Editor.FolderId = m_iID;
                    _Editor.SetPermissions(_PermissionData);
                    _Editor.AllowFonts = true;
                    _Editor.Content = _DiscussionBoard.TermsAndConditions;
                    _Editor.Visible = true;
                    pnlTerms.Controls.Add(_Editor);

                    hdn_adf_folderid.Value = m_iID.ToString();
                    hdn_adb_action.Value = "prop";
                    SetLabels("prop");
                    ShowCategories();
                    SetCSSdropdown(_DiscussionBoard.StyleSheet);
                    Display_DiscussionBoardJS("");
                    lit_ef_templatedata.Text = "<input type=\"hidden\" id=\"language\" value=\"" + this.ContentLanguage + "\" /><input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - m_refContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";
                    DrawContentTemplatesTable("edit");
                    DrawFolderTaxonomyTable();
                    DisplaySitemapPath();
                }
                else if (m_sPageAction == "viewcat")
                {
                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    base.SetTitleBarToMessage("view board subject title");

					if (usesModal)
					{
						base.AddBackButton("addeditboard.aspx?action=editprop&id=" + m_iID.ToString() + "&thickbox=true");
					}
					else
					{
						base.AddBackButton("addeditboard.aspx?action=View&id=" + m_iID.ToString());
					}

                    base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("addeditboard.aspx?LangType=" + this.ContentLanguage.ToString() + "&action=editcat&id=" + m_iID.ToString() + "&catid=" + _CategoryId.ToString()), "lbl alt edit board subject", "edit board subject title", "", StyleHelper.EditButtonCssClass, true);
                    if (_Categories.Length > 1) // do not allow delete if this is the only category in the board
                    {
                        base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string)("addeditboard.aspx?LangType=" + this.ContentLanguage.ToString() + "&action=delcat&id=" + m_iID.ToString() + "&catid=" + _CategoryId.ToString()), "lbl alt delete board subject", "delete board subject title", "", StyleHelper.DeleteButtonCssClass);
                    }

                    base.AddHelpButton("ViewBoardCategories");

                    for (int i = 0; i <= (_Categories.Length - 1); i++)
                    {
                        if (_Categories[i].CategoryID == _CategoryId)
                        {
                            txt_catname.Text = _Categories[i].Name;
                            lbl_catid.Text = _Categories[i].CategoryID.ToString();
                            txt_catsort.Text = _Categories[i].SortOrder.ToString();
                        }
                    }

                    txt_catname.Enabled = false;
                    txt_catsort.Enabled = false;

                    hdn_adf_folderid.Value = m_iID.ToString();
                    hdn_adb_action.Value = "cat";
                    SetLabels("cat");
                    ShowCategories();
                    Display_EditCatJS();
                }
                else if (m_sPageAction == "editcat")
                {
                    base.SetTitleBarToMessage("edit board subject title");
                    
                    if (usesModal)
                    {
                        base.AddBackButton("addeditboard.aspx?LangType=" + this.ContentLanguage.ToString() + "&action=editprop&id=" + m_iID.ToString() + "&thickbox=true");
                    }
                    else
                    {
                        base.AddBackButton("addeditboard.aspx?LangType=" + this.ContentLanguage.ToString() + "&action=Viewcat&id=" + m_iID.ToString() + "&catid=" + _CategoryId.ToString());
                    }

					base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "lbl alt save board subject", "lbl save board subject", " onclick=\"javascript:return CheckBoardCatParameters();\" ", StyleHelper.SaveButtonCssClass, true);

                    base.AddHelpButton("EditBoardCategories");

                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    for (int i = 0; i <= (_Categories.Length - 1); i++)
                    {
                        if (_Categories[i].CategoryID == _CategoryId)
                        {
                            txt_catname.Text = _Categories[i].Name;
                            ltr_catid.Text = _Categories[i].CategoryID.ToString();
                            txt_catsort.Text = _Categories[i].SortOrder.ToString();
                        }
                    }

                    txt_adb_boardname.Text = _DiscussionBoard.Name;
                    txt_adb_title.Text = _DiscussionBoard.Description;
                    chk_adb_mc.Checked = _DiscussionBoard.ModerateComments;
                    chk_adb_ra.Checked = _DiscussionBoard.RequireAuthentication;
                    chk_lock_board.Checked = _DiscussionBoard.LockBoard;
                    txt_adb_stylesheet.Text = _DiscussionBoard.StyleSheet;
                    ltr_sitepath.Text = m_refContentApi.SitePath;

                    hdn_adf_folderid.Value = m_iID.ToString();
                    hdn_adb_action.Value = "cat";
                    SetLabels("cat");
                    ShowCategories();
                    Display_EditCatJS();
                }
                else if (m_sPageAction == "delcat")
                {
                    int removeid = 0;
                    base.SetTitleBarToMessage("delete board subject title");

					base.AddBackButton((string)("addeditboard.aspx?LangType=" + this.ContentLanguage.ToString() + "&action=viewcat&id=" + m_iID.ToString() + "&catid=" + _CategoryId.ToString())); 
					
					base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/delete.png", "#", "lbl alt delete board", "delete board subject title", " onclick=\"javascript:return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass, true);
                    
                    base.AddHelpButton("DeleteBoardCategories");

                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    for (int i = 0; i <= (_Categories.Length - 1); i++)
                    {
                        if (_Categories[i].CategoryID == _CategoryId)
                        {
                            ltr_delcatnamedata.Text = _Categories[i].Name;
                            removeid = i;
                            break;
                        }
                    }
                    drp_movecat.DataSource = _Categories;
                    drp_movecat.DataTextField = "Name";
                    drp_movecat.DataValueField = "CategoryID";
                    drp_movecat.DataBind();
                    drp_movecat.Items.RemoveAt(removeid);
                    drp_movecat.SelectedIndex = 0;

                    hdn_adf_folderid.Value = m_iID.ToString();
                    hdn_adb_action.Value = "delcat";
                    SetLabels("delcat");
                    //ShowCategories()
                    Display_DeleteCatJS();
                }
                else if (m_sPageAction == "addcat")
                {
                    base.SetTitleBarToMessage("add board subject title");

					base.AddBackButton((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iID.ToString())); 
					
					base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "lbl alt save board subject", "lbl save board subject", " onclick=\"javascript:return CheckBoardCatParameters();\" ", StyleHelper.SaveButtonCssClass, true);
                    
                    base.AddHelpButton("AddBoardCategories");

                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    txt_catname.Text = "";
                    txt_catsort.Text = "1";
                    tr_catid.Visible = false;

                    hdn_adf_folderid.Value = m_iID.ToString();
                    hdn_adb_action.Value = "addcat";
                    SetLabels("cat");
                    ShowCategories();
                    Display_EditCatJS();
                }
                else //view
                {
                    base.Version8TabsImplemented = true;
                    base.SetTitleBarToMessage("view board prop title");
                    
                    //MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "btn_EventTypes-nm.gif", "addeditboard.aspx?LangType=" & m_refContentApi.ContentLanguage & "&action=editcat&id=" & m_iID.ToString(), "alt editboard cat", "lbl editboard cat", "")
                    if (usesModal)
                    {
                        base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("addeditboard.aspx?LangType=" + m_refContentApi.ContentLanguage + "&action=editprop&id=" + m_iID.ToString() + "&thickbox=true"), "alt editboard prop", "lbl editboard prop", "", StyleHelper.EditButtonCssClass, true);
                        base.AddButtonwithMessages(AppImgPath + "../UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.RemoveButtonCssClass);
                    }
                    else
                    {
						base.AddBackButton("../content.aspx?action=ViewContentByCategory&id=" + m_iID.ToString());
						base.AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string)("addeditboard.aspx?LangType=" + m_refContentApi.ContentLanguage + "&action=editprop&id=" + m_iID.ToString()), "alt editboard prop", "lbl editboard prop", "", StyleHelper.EditButtonCssClass, true);
					}

                    base.AddHelpButton("ViewBoardProp");

                    _DiscussionBoard = _EkContentRef.GetBoardbyID(m_iID.ToString());
                    _Categories = _DiscussionBoard.Categories;

                    txt_adb_boardname.Text = _DiscussionBoard.Name;
                    txt_adb_title.Text = _DiscussionBoard.Description;
                    ltr_acceptedhtml.Text = base.GetMessage("lbl acceptedhtml");
                    txt_acceptedhtml.Text = _DiscussionBoard.AcceptedHTML;
                    ltr_acceptedextensions.Text = base.GetMessage("lbl acceptedextensions");
                    txt_acceptedextensions.Text = _DiscussionBoard.AcceptedExtensions;
                    ltr_boardid_data.Text = _DiscussionBoard.Id.ToString();
                    chk_adb_mc.Checked = _DiscussionBoard.ModerateComments;
                    chk_adb_ra.Checked = _DiscussionBoard.RequireAuthentication;
                    chk_lock_board.Checked = _DiscussionBoard.LockBoard;
                    txt_adb_stylesheet.Text = _DiscussionBoard.StyleSheet;
                    ltr_sitepath.Text = m_refContentApi.SitePath;
                    chkInheritSitemapPath.Checked = Convert.ToBoolean(_DiscussionBoard.SitemapInherited);

                    Literal termsText = new Literal();
                    termsText.Text = _DiscussionBoard.TermsAndConditions;
                    pnlTerms.Controls.Add(termsText);

                    ltr_maxfilesize.Text = base.GetMessage("lbl max file size");
                    txt_maxfilesize.Text = _DiscussionBoard.MaxFileSize.ToString();
                    ltr_bytes.Text = base.GetMessage("lbl bytes");
                    ltr_comma_ext.Text = base.GetMessage("lbl comma sep");
                    ltr_comma_html.Text = base.GetMessage("lbl comma sep");

                    Display_DiscussionBoardJS("view");

                    hdn_adf_folderid.Value = m_iID.ToString();
                    SetLabels("");
                    ShowCategories();
                    SetCSSdropdown(_DiscussionBoard.StyleSheet);
                    SetDisabled();
                    lit_ef_templatedata.Text = "<input type=\"hidden\" maxlength=\"255\" size=\"" + (75 - m_refContentApi.SitePath.Length) + "\" value=\"\" name=\"templatefilename\" id=\"templatefilename\">";
                    DrawContentTemplatesTable("view");
                    DrawFolderTaxonomyTable();
                    ViewSitemapPath();
                }
            }
            else
            {
                Process_DoUpdate();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Helpers

    private void Process_DoUpdate()
    {
        string sTerms = "";
        if (Request.Form[hdn_adb_action.UniqueID] == "prop")
        {

            this._OldTemplateName = m_refContentApi.GetTemplatesByFolderId(m_iID).FileName;
            _DiscussionBoard.Id = m_iID;
            _DiscussionBoard.Name = (string)(Request.Form[txt_adb_boardname.UniqueID].Trim(".".ToCharArray()));
            _DiscussionBoard.Title = Request.Form[txt_adb_title.UniqueID];

            //BreadCrumb/SiteMapPath update.
            if ((Request.Form["hdnInheritSitemap"] != null) && (Request.Form["hdnInheritSitemap"].ToString().ToLower() == "true"))
            {
                _DiscussionBoard.SitemapInherited = Convert.ToInt32(true);
            }
            else
            {
                _DiscussionBoard.SitemapInherited = Convert.ToInt32(false);
                _DiscussionBoard.SitemapPath = Utilities.DeserializeSitemapPath(Request.Form, this.ContentLanguage);
            }

            _DiscussionBoard.AcceptedHTML = this.ProcessCSV(Request.Form[txt_acceptedhtml.UniqueID], "html");
            _DiscussionBoard.AcceptedExtensions = this.ProcessCSV(Request.Form[txt_acceptedextensions.UniqueID], "ext");

            if (Request.Form[chk_adb_mc.UniqueID] != null && Request.Form[chk_adb_mc.UniqueID] != "")
            {
                _DiscussionBoard.ModerateComments = true;
            }
            else
            {
                _DiscussionBoard.ModerateComments = false;
            }
            if (Request.Form[chk_adb_ra.UniqueID] != null && Request.Form[chk_adb_ra.UniqueID] != "")
            {
                _DiscussionBoard.RequireAuthentication = true;
            }
            else
            {
                _DiscussionBoard.RequireAuthentication = false;
            }
            if (Request.Form[chk_lock_board.UniqueID] != null && Request.Form[chk_lock_board.UniqueID] != "")
            {
                _DiscussionBoard.LockBoard = true;
            }
            else
            {
                _DiscussionBoard.LockBoard = false;
            }
            // handle dynamic replication properties
            if ((Request.Form[chk_repl.UniqueID] != null && Request.Form[chk_repl.UniqueID] != "") || (Request.Form["EnableReplication"] != null && Request.Form["EnableReplication"] == "1"))
            {
                _DiscussionBoard.ReplicationMethod = 1;
            }
            else
            {
                _DiscussionBoard.ReplicationMethod = 0;
            }
            sTerms = (string)_Editor.Content;
            if (!(Request.Form["content_html_action"] == null))
            {
                sTerms = Context.Server.HtmlDecode(sTerms);
            }
            _DiscussionBoard.TermsAndConditions = sTerms;
            _DiscussionBoard.StyleSheet = Request.Form[txt_adb_stylesheet.UniqueID];
            if (Information.IsNumeric(Request.Form[txt_maxfilesize.UniqueID]) && Convert.ToInt32(Request.Form[txt_maxfilesize.UniqueID]) > 0)
            {
                _DiscussionBoard.MaxFileSize = Convert.ToInt32(Request.Form[txt_maxfilesize.UniqueID]);
            }
            else
            {
                _DiscussionBoard.MaxFileSize = 0;
            }

            _DiscussionBoard.TaxonomyInherited = false;

            if ((Request.Form["CategoryRequired"] != null) && Request.Form["CategoryRequired"].ToString().ToLower() == "on")
            {
                _DiscussionBoard.CategoryRequired = true;
            }
            else
            {
                _DiscussionBoard.CategoryRequired = Convert.ToBoolean(Convert.ToInt32(Request.Form[parent_category_required.UniqueID]));
            }

            if (Request.Form["taxlist"] == null || Request.Form["taxlist"].Trim().Length == 0)
            {
                _DiscussionBoard.CategoryRequired = false;
            }

            string IdRequests = "";
            if ((Request.Form["taxlist"] != null) && Request.Form["taxlist"] != "")
            {
                IdRequests = Request.Form["taxlist"];
            }

            //dbBoard.TaxonomyInheritedFrom = Convert.ToInt32(Request.Form(inherit_taxonomy_from.UniqueID))
            if (_GroupID != -1)
            {
                _EkContentRef.UpdateBoard(_DiscussionBoard, _GroupID);
            }
            else
            {
                _DiscussionBoard = _EkContentRef.UpdateBoard(_DiscussionBoard);

            }

            FolderRequest folder_request = new FolderRequest();
            _FolderData = m_refContentApi.GetFolderById(m_iID);

            //@folderid int,
            //@foldername nvarchar(75),
            //@folderdescription nvarchar(255),
            //@stylesheet nvarchar(255),
            //@inheritmeta int,
            //@inheritmetafrom int,
            //@replicationflag int,
            //@productionhost nvarchar(510)='',
            //@staginghost nvarchar(510)='',
            //@inheritmetadata int,
            //@inheritmetadatafrom int,
            //@inherittaxonomy bit=0,
            //@inherittaxonomyfrom int=0,
            //@categoryrequired bit=0,
            //@catlanguage int=1033, ??
            //@catlist varchar(4000)=''

            folder_request.FolderId = _FolderData.Id;
            folder_request.FolderName = _FolderData.Name;
            folder_request.FolderDescription = _FolderData.Description;
            folder_request.StyleSheet = _FolderData.StyleSheet;
            folder_request.MetaInherited = _FolderData.MetaInherited;
            folder_request.MetaInheritedFrom = _FolderData.MetaInheritedFrom;
            //folder_request.EnableReplication = ??
            folder_request.DomainProduction = _FolderData.DomainProduction;
            folder_request.DomainStaging = _FolderData.DomainStaging;
            folder_request.MetaInherited = _FolderData.MetaInherited;
            folder_request.TaxonomyInherited = false;
            folder_request.TaxonomyInheritedFrom = _FolderData.MetaInheritedFrom;
            folder_request.CategoryRequired = _FolderData.CategoryRequired;

            //Updating Board folder with Sitemap information.
            folder_request.SiteMapPath = _DiscussionBoard.SitemapPath;
            folder_request.SiteMapPathInherit = System.Convert.ToBoolean(_DiscussionBoard.SitemapInherited);
            //catlanguage ??
            folder_request.TaxonomyIdList = IdRequests;
            if (_GroupID != -1)
            {
                m_refContentApi.UpdateBoardForumFolder(folder_request, _GroupID);
            }
            else
            {
                m_refContentApi.UpdateFolder(folder_request);
            }


            ProcessContentTemplatesPostBack();
            if (usesModal)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "CloseThickbox.aspx", false);
            }
            else if (Request.Form[hdn_adb_boardname.UniqueID] == Request.Form[txt_adb_boardname.UniqueID])
            {
                Response.Redirect((string)("addeditboard.aspx?action=View&id=" + _DiscussionBoard.Id.ToString()), false);
            }
            else
            {
                Response.Redirect("../content.aspx?TreeUpdated=1&LangType=" + ContentLanguage + "&action=ViewBoard&id=" + m_iID.ToString() + "&reloadtrees=Forms,Content,Library", false);
            }

            //If Not (Request.Form("suppress_notification") <> "") Then
            //    m_refcontent.UpdateSubscriptionPropertiesForFolder(m_intFolderId, sub_prop_data)
            //    m_refcontent.UpdateSubscriptionsForFolder(m_intFolderId, page_subscription_data)
            //End If

        }
        else if (Request.Form[hdn_adb_action.UniqueID] == "cat")
        {
            Ektron.Cms.DiscussionCategory[] acCat = new DiscussionCategory[1];
            acCat[0] = new Ektron.Cms.DiscussionCategory();
            acCat[0].BoardID = m_iID;
            acCat[0].CategoryID = _CategoryId;
            acCat[0].Name = Request.Form[txt_catname.UniqueID];
            acCat[0].SetSortOrder(Convert.ToInt32(Request.Form[txt_catsort.UniqueID]));

            _EkContentRef.UpdateCategory(acCat);

            if (usesModal)
            {
                Response.Redirect("addeditboard.aspx?action=View&id=" + m_iID.ToString() + "&thickbox=true", false);
            }
            else
            {
                Response.Redirect("addeditboard.aspx?action=View&id=" + m_iID.ToString(), false);
            }

        }
        else if (Request.Form[hdn_adb_action.UniqueID] == "addcat")
        {
            Ektron.Cms.DiscussionCategory[] acCat = new DiscussionCategory[1];
            acCat[0] = new Ektron.Cms.DiscussionCategory();
            acCat[0].BoardID = m_iID;
            acCat[0].CategoryID = 0;
            acCat[0].Name = Request.Form[txt_catname.UniqueID];
            acCat[0].SetSortOrder(Convert.ToInt32(Request.Form[txt_catsort.UniqueID]));

            _EkContentRef.AddCategoryforBoard(acCat);

            Response.Redirect((string)("../content.aspx?action=ViewContentByCategory&id=" + m_iID.ToString()), false);
        }
        else if (Request.Form[hdn_adb_action.UniqueID] == "delcat")
        {
            _EkContentRef.DeleteBoardCategory(_CategoryId, Convert.ToInt64(Request.Form[drp_movecat.UniqueID]));
            Response.Redirect((string)("addeditboard.aspx?action=View&id=" + m_iID.ToString()), false);
        }
    }

    private void DrawFolderTaxonomyTable()
    {
        string categorydatatemplate = "<input type=\"radio\" id=\"taxlist\" name=\"taxlist\" value=\"{0}\" {1} {2}/>{3}";
        StringBuilder categorydata = new StringBuilder();
        TaxonomyRequest catrequest = new TaxonomyRequest();
        string catdisabled = "";

        if (_FolderData == null)
        {
            _FolderData = this.m_refContentApi.GetFolderById(m_iID, true);
        }

        catrequest.TaxonomyId = 0;
        catrequest.TaxonomyLanguage = ContentLanguage;
        catrequest.SortOrder = "taxonomy_name";
        if ((_FolderData.FolderTaxonomy != null) && _FolderData.FolderTaxonomy.Length > 0)
        {
            for (int i = 0; i <= _FolderData.FolderTaxonomy.Length - 1; i++)
            {
                if (_SelectedTaxonomyList.Length > 0)
                {
                    _SelectedTaxonomyList = _SelectedTaxonomyList + "," + _FolderData.FolderTaxonomy[i].TaxonomyId;
                }
                else
                {
                    _SelectedTaxonomyList = _FolderData.FolderTaxonomy[i].TaxonomyId.ToString();
                }
            }
        }
        _CurrentCategoryChecked = Convert.ToInt32(_FolderData.CategoryRequired);
        current_category_required.Value = _CurrentCategoryChecked.ToString();
        inherit_taxonomy_from.Value = _FolderData.TaxonomyInheritedFrom.ToString();
        TaxonomyBaseData[] TaxArr = m_refContentApi.EkContentRef.ReadAllSubCategories(catrequest);
        string DisabledMsg = "";
        if (!_IsTaxonomyUiEnabled)
        {
            DisabledMsg = " disabled ";
            catdisabled = " disabled ";
        }
        bool parent_has_configuration = false;
        if ((TaxArr != null) && TaxArr.Length > 0)
        {
            categorydata.Append("<table class=\"ektronGrid ektronBorder\" width=\"100%\">");
            categorydata.Append("<tr class=\"row\"><td>");
            categorydata.Append(string.Format(categorydatatemplate, "", IsChecked(System.Convert.ToBoolean(_SelectedTaxonomyList.Length == 0)), DisabledMsg, "None"));
            categorydata.Append("<br/>");
            int i = 0;
            while (i < TaxArr.Length)
            {
                _CheckTaxId = TaxArr[i].TaxonomyId;
                if (_FolderData.FolderTaxonomy != null)
                {
                    parent_has_configuration = Array.Exists(_FolderData.FolderTaxonomy, new Predicate<TaxonomyBaseData>(TaxonomyExists));
                }
                else
                {
                    parent_has_configuration = false;
                }
                categorydata.Append("<tr><td>");

                categorydata.Append(string.Format(categorydatatemplate, TaxArr[i].TaxonomyId, IsChecked(parent_has_configuration), DisabledMsg, TaxArr[i].TaxonomyName));
                categorydata.Append("<br/>");
                categorydata.Append("</td></tr>");

                i++;
            }
            categorydata.Append("</table>");
        }

        StringBuilder str = new StringBuilder();
        str.Append("<input type=\"hidden\" id=\"TaxonomyParentHasConfig\" name=\"TaxonomyParentHasConfig\" value=\"");
        if (parent_has_configuration)
        {
            str.Append("1");
        }
        else
        {
            str.Append("0");
        }

        str.Append("\" />");

        DisabledMsg = " ";
        if (_FolderData.Id == 0)
        {
            DisabledMsg = " disabled ";
        }
        else
        {
            DisabledMsg = IsChecked(_FolderData.TaxonomyInherited);
        }
        if (!_IsTaxonomyUiEnabled)
        {
            DisabledMsg += " disabled ";
        }
        string catchecked = "";
        if (_FolderData.CategoryRequired)
        {
            catchecked = " checked ";
        }
        if (_FolderData.Id > 0)
        {
            FolderData parentfolderdata = m_refContentApi.GetFolderById(_FolderData.ParentId, true);
            if ((parentfolderdata.FolderTaxonomy != null) && parentfolderdata.FolderTaxonomy.Length > 0)
            {
                for (int i = 0; i <= parentfolderdata.FolderTaxonomy.Length - 1; i++)
                {
                    if (_SelectedTaxonomyParentList.Length > 0)
                    {
                        _SelectedTaxonomyParentList = _SelectedTaxonomyParentList + "," + parentfolderdata.FolderTaxonomy[i].TaxonomyId;
                    }
                    else
                    {
                        _SelectedTaxonomyParentList = parentfolderdata.FolderTaxonomy[i].TaxonomyId.ToString();
                    }
                }
                _ParentCategoryChecked = Convert.ToInt32(parentfolderdata.CategoryRequired);
                parent_category_required.Value = _ParentCategoryChecked.ToString();
            }
        }

        //str.Append("<input name=""TaxonomyTypeBreak"" id=""TaxonomyTypeBreak"" type=""checkbox"" onclick=""ToggleTaxonomyInherit(this)"" " & DisabledMsg & "/><b>Inherit Parent Taxonomy Configuration</b>")
        str.Append("<input name=\"CategoryRequired\" id=\"CategoryRequired\" type=\"checkbox\"" + catchecked + catdisabled + " /><span class=\"label\">"+m_refMsg.GetMessage("lbl Require category selection")+"</span>");
        str.Append("<div class=\"ektronTopSpace\"></div>");
        str.Append(categorydata.ToString());
        taxonomy_list.Text = str.ToString();
        str = new StringBuilder();

        str.Append("var taxonomytreearr=\"" + _SelectedTaxonomyList + "\".split(\",\");");
        str.Append("var taxonomyparenttreearr=\"" + _SelectedTaxonomyParentList + "\".split(\",\");");
        str.Append("var __jscatrequired=\"" + _CurrentCategoryChecked + "\";");
        str.Append("var __jsparentcatrequired=\"" + _ParentCategoryChecked + "\";");
        tax_js.Text = str.ToString();
    }

    private string IsChecked(bool value)
    {
        if (value)
        {
            return " checked=\"true\"";
        }
        else
        {
            return " ";
        }
    }

    private bool TaxonomyExists(TaxonomyBaseData data)
    {
        if (data != null)
        {
            if (data.TaxonomyId == _CheckTaxId)
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

    private void Display_DiscussionBoardJS(string smode)
    {
        StringBuilder sbdiscussionboardjs = new StringBuilder();
        sbdiscussionboardjs.Append("Ektron.ready(function() {document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".focus();});" + Environment.NewLine);
        sbdiscussionboardjs.Append(Environment.NewLine + Environment.NewLine);
        sbdiscussionboardjs.Append("function CheckDiscussionBoardParameters() {" + Environment.NewLine);
        sbdiscussionboardjs.Append("document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        // sbdiscussionboardjs.Append("document.forms.form1." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value = Trim(document.forms.form1." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value);" & Environment.NewLine)
        // sbdiscussionboardjs.Append("var iSort = document.forms.form1." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value;" & Environment.NewLine)

        //sbdiscussionboardjs.Append("if (typeof(eWebEditPro) != ""undefined"" && eWebEditPro != null)").Append(Environment.NewLine)
        //sbdiscussionboardjs.Append("{").Append(Environment.NewLine)
        //sbdiscussionboardjs.Append("    eWebEditPro.instances[0].editorGetMethod = 'getBodyHTML';").Append(Environment.NewLine)
        //sbdiscussionboardjs.Append("	eWebEditPro.instances[0].save(eWebEditPro.instances[0].linkedElement, null, new Function());").Append(Environment.NewLine)
        //sbdiscussionboardjs.Append("}").Append(Environment.NewLine)

        sbdiscussionboardjs.Append("if ((document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("	alert(\"" + _cApi.EkMsgRef.GetMessage("js:add discussion board no name") + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    ShowPane(\'dvProp\');").Append(Environment.NewLine);
        sbdiscussionboardjs.Append("	document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbdiscussionboardjs.Append("	return false;" + Environment.NewLine);
        //iSort
        //sbdiscussionboardjs.Append("} else if (isNaN(iSort)||iSort<1)" & Environment.NewLine)
        //sbdiscussionboardjs.Append("{" & Environment.NewLine)
        //sbdiscussionboardjs.Append("	alert(""Sort order must be 1 or higher."");" & Environment.NewLine)
        //sbdiscussionboardjs.Append("	document.forms.form1." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        //sbdiscussionboardjs.Append("	return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}else {" + Environment.NewLine);
        sbdiscussionboardjs.Append("	if (!CheckDiscussionBoardForillegalChar()) {" + Environment.NewLine);
        sbdiscussionboardjs.Append("		return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("	}" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("saveSitemapPath();" + Environment.NewLine);

        sbdiscussionboardjs.Append("if (document.getElementById(\'addTemplate\').selectedIndex == 0) " + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("    alert(\"You must select a template for your Discussion Board.\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    ShowPane(\'dvTemplate\');").Append(Environment.NewLine);
        sbdiscussionboardjs.Append("	document.getElementById(\'addTemplate\').focus();" + Environment.NewLine);
        sbdiscussionboardjs.Append("	return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);

        sbdiscussionboardjs.Append("var regexp1 = /\"/gi;" + Environment.NewLine);
        sbdiscussionboardjs.Append("document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("	document.forms.form1.submit();" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function CheckDiscussionBoardForillegalChar() {" + Environment.NewLine);
        sbdiscussionboardjs.Append("   var val = document.forms.form1." + Strings.Replace((string)txt_adb_boardname.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbdiscussionboardjs.Append("   if ((val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbdiscussionboardjs.Append("   {" + Environment.NewLine);
        sbdiscussionboardjs.Append("       alert(\"Discussion Board name can\'t include (\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("       return false;" + Environment.NewLine);
        sbdiscussionboardjs.Append("   }" + Environment.NewLine);
        sbdiscussionboardjs.Append("   return true;" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);
        sbdiscussionboardjs.Append("function updatetheme()" + Environment.NewLine);
        sbdiscussionboardjs.Append("{" + Environment.NewLine);
        sbdiscussionboardjs.Append("    var mylist = document.getElementById(\"" + drp_theme.UniqueID + "\");" + Environment.NewLine);
        sbdiscussionboardjs.Append("    var tText = mylist.options[mylist.selectedIndex].value;" + Environment.NewLine);
        sbdiscussionboardjs.Append("    if (tText.length > 0) {" + Environment.NewLine);
        sbdiscussionboardjs.Append("        document.getElementById(\"" + txt_adb_stylesheet.UniqueID + "\").value = tText;" + Environment.NewLine);
        sbdiscussionboardjs.Append("    }" + Environment.NewLine);
        sbdiscussionboardjs.Append("}" + Environment.NewLine);

        if (smode == "view")
        {
            sbdiscussionboardjs = new StringBuilder();
            sbdiscussionboardjs.Append("function PreviewTemplate(sitepath,width,height)" + Environment.NewLine);
            sbdiscussionboardjs.Append("{" + Environment.NewLine);
            sbdiscussionboardjs.Append("	var templar = document.getElementById(\"addTemplate\")" + Environment.NewLine);
            sbdiscussionboardjs.Append("	if (templar.value != 0) {" + Environment.NewLine);
            sbdiscussionboardjs.Append("        window.open(sitepath + templar.options[templar.selectedIndex].text,\'\',\'resizable=1, scrollbars=1\',\'toolbar,width=\' + width + \',height=\' + height);" + Environment.NewLine);
            sbdiscussionboardjs.Append("	} else {" + Environment.NewLine);
            sbdiscussionboardjs.Append("        alert(\'No template is selected.\');" + Environment.NewLine);
            sbdiscussionboardjs.Append("	}" + Environment.NewLine);
            sbdiscussionboardjs.Append("}" + Environment.NewLine);
        }
        else
        {
            sbdiscussionboardjs.Append("function PreviewTemplate(sitepath,width,height)" + Environment.NewLine);
            sbdiscussionboardjs.Append("{" + Environment.NewLine);
            sbdiscussionboardjs.Append("	var templar = document.getElementById(\"addTemplate\")" + Environment.NewLine);
            sbdiscussionboardjs.Append("	if (templar.value != 0) {" + Environment.NewLine);
            if (_GroupID != -1)
            {
                sbdiscussionboardjs.Append("        window.open(sitepath + templar.options[templar.selectedIndex].text + \'?groupid=").Append(_GroupID).Append("\',\'\',\'resizable=1, scrollbars=1\',\'toolbar,width=\' + width + \',height=\' + height);" + Environment.NewLine);
            }
            else
            {
                sbdiscussionboardjs.Append("        window.open(sitepath + templar.options[templar.selectedIndex].text,\'\',\'resizable=1, scrollbars=1\',\'toolbar,width=\' + width + \',height=\' + height);" + Environment.NewLine);
            }
            sbdiscussionboardjs.Append("	} else {" + Environment.NewLine);
            sbdiscussionboardjs.Append("        alert(\'Please select a valid template\');" + Environment.NewLine);
            sbdiscussionboardjs.Append("	}" + Environment.NewLine);
            sbdiscussionboardjs.Append("}" + Environment.NewLine);
        }

        ltr_af_js.Text = sbdiscussionboardjs.ToString();
    }

    private void Display_DeleteCatJS()
    {
        StringBuilder sbdelcatjs = new StringBuilder();
        sbdelcatjs.Append("function CheckDelete() {" + Environment.NewLine);
        sbdelcatjs.Append("	var bcon = confirm(\"Delete the subject and move its forums?\");" + Environment.NewLine);
        sbdelcatjs.Append("	if (bcon == true) {" + Environment.NewLine);
        sbdelcatjs.Append("	 document.forms.form1.submit();" + Environment.NewLine);
        sbdelcatjs.Append(" }" + Environment.NewLine);
        sbdelcatjs.Append("}" + Environment.NewLine);
        ltr_af_js.Text = sbdelcatjs.ToString();
    }

    private void Display_EditCatJS()
    {
        StringBuilder sbeditcatjs = new StringBuilder();
        if (m_sPageAction == "editcat")
        {
            sbeditcatjs.Append("Ektron.ready(function() {document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".focus();});" + Environment.NewLine);
        }
        sbeditcatjs.Append(Environment.NewLine + Environment.NewLine);
        sbeditcatjs.Append("function CheckBoardCatParameters() {" + Environment.NewLine);
        sbeditcatjs.Append("document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbeditcatjs.Append("document.forms.form1." + Strings.Replace((string)txt_catsort.UniqueID, "$", "_", 1, -1, 0) + ".value = Trim(document.forms.form1." + Strings.Replace((string)txt_catsort.UniqueID, "$", "_", 1, -1, 0) + ".value);" + Environment.NewLine);
        sbeditcatjs.Append("var iSort = document.forms.form1." + Strings.Replace((string)txt_catsort.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbeditcatjs.Append("if ((document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value == \"\"))" + Environment.NewLine);
        sbeditcatjs.Append("{" + Environment.NewLine);
        sbeditcatjs.Append("	alert(\"" + m_refMsg.GetMessage("alert msg supply name subject") + "\");" + Environment.NewLine);
        sbeditcatjs.Append("	document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbeditcatjs.Append("	return false;" + Environment.NewLine);
        sbeditcatjs.Append("} else if (isNaN(iSort)||iSort<1)" + Environment.NewLine);
        sbeditcatjs.Append("{" + Environment.NewLine);
        sbeditcatjs.Append("	alert(\"" + m_refMsg.GetMessage("msg sort") + "\");" + Environment.NewLine);
        sbeditcatjs.Append("	document.forms.form1." + Strings.Replace((string)txt_catsort.UniqueID, "$", "_", 1, -1, 0) + ".focus();" + Environment.NewLine);
        sbeditcatjs.Append("	return false;" + Environment.NewLine);
        sbeditcatjs.Append("}else {" + Environment.NewLine);
        sbeditcatjs.Append("	if (!CheckCatForillegalChar()) {" + Environment.NewLine);
        sbeditcatjs.Append("		return false;" + Environment.NewLine);
        sbeditcatjs.Append("	}" + Environment.NewLine);
        sbeditcatjs.Append("}" + Environment.NewLine);
        sbeditcatjs.Append("var regexp1 = /\"/gi;" + Environment.NewLine);
        sbeditcatjs.Append("document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value = document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value.replace(regexp1, \"\'\");" + Environment.NewLine);
        sbeditcatjs.Append("	document.forms.form1.submit();" + Environment.NewLine);
        sbeditcatjs.Append("}" + Environment.NewLine);
        sbeditcatjs.Append("function CheckCatForillegalChar() {" + Environment.NewLine);
        sbeditcatjs.Append("   var val = document.forms.form1." + Strings.Replace((string)txt_catname.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbeditcatjs.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbeditcatjs.Append("   {" + Environment.NewLine);
        sbeditcatjs.Append("       alert(\"" + m_refMsg.GetMessage("alert subject name") + " " + "(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbeditcatjs.Append("       return false;" + Environment.NewLine);
        sbeditcatjs.Append("   }" + Environment.NewLine);
        sbeditcatjs.Append("   return true;" + Environment.NewLine);
        sbeditcatjs.Append("}" + Environment.NewLine);

        ltr_af_js.Text = sbeditcatjs.ToString();
    }

    private void ShowCategories()
    {
        StringBuilder sbCat = new StringBuilder();
        sbCat.Append("<tr class=\"title-header\">");
        sbCat.Append("<td>");
        sbCat.Append(base.GetMessage("lbl discussionforumsubject"));
        sbCat.Append("</td>");
        sbCat.Append("<td class=\"value\">");
        sbCat.Append(base.GetMessage("lbl discussionforumsortorder"));
        sbCat.Append("</td>");
        sbCat.Append("</tr>");
        for (int i = 0; i <= (_Categories.Length - 1); i++)
        {
            sbCat.Append("<tr>");
            sbCat.Append("<td>");
            if (_editMode)
            {
                if (usesModal)
                {
                    sbCat.Append("<a href=\"addeditboard.aspx?LangType=" + m_refContentApi.ContentLanguage + "&action=editcat&id=" + m_iID.ToString() + "&catid=" + _Categories[i].CategoryID.ToString() + "&thickbox=true" + "\">");
                }
                else
                {
                    sbCat.Append("<a href=\"addeditboard.aspx?LangType=" + m_refContentApi.ContentLanguage + "&action=viewcat&id=" + m_iID.ToString() + "&catid=" + _Categories[i].CategoryID.ToString() + "\">");
                }

                sbCat.Append(_Categories[i].Name);
                sbCat.Append("</a>");
            }
            else
            {
                sbCat.Append(_Categories[i].Name);
            }
            sbCat.Append("</td>");
            sbCat.Append("<td>");
            sbCat.Append(_Categories[i].SortOrder.ToString());
            sbCat.Append("</td>");
            sbCat.Append("</tr>");
        }
        ltr_adb_cat.Text = sbCat.ToString();
    }

    private void SetCSSdropdown(string css)
    {
        if (css.ToLower().IndexOf("threadeddisc/themes/standard.css") > -1)
        {
            drp_theme.SelectedIndex = 1;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/chrome.css") > -1)
        {
            drp_theme.SelectedIndex = 2;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/cool.css") > -1)
        {
            drp_theme.SelectedIndex = 3;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/graysky/graysky.css") > -1)
        {
            drp_theme.SelectedIndex = 4;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/jungle.css") > -1)
        {
            drp_theme.SelectedIndex = 5;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/modern.css") > -1)
        {
            drp_theme.SelectedIndex = 6;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/royal.css") > -1)
        {
            drp_theme.SelectedIndex = 7;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/slate.css") > -1)
        {
            drp_theme.SelectedIndex = 8;
        }
        else if (css.ToLower().IndexOf("threadeddisc/themes/techno.css") > -1)
        {
            drp_theme.SelectedIndex = 9;
        }
    }

    private void SetLabels(string type)
    {
        string sJustAppPath = m_refContentApi.AppPath.Replace(m_refContentApi.SitePath, "");
        if ((sJustAppPath.Length > 0))
            if (!(sJustAppPath[sJustAppPath.Length - 1].ToString() == "/"))
                sJustAppPath = sJustAppPath + "/";
        drp_theme.Items.Add(new ListItem("Select a theme", ""));
        drp_theme.Items.Add(new ListItem("Standard", sJustAppPath + "threadeddisc/themes/standard.css"));
        drp_theme.Items.Add(new ListItem("Chrome", sJustAppPath + "threadeddisc/themes/chrome.css"));
        drp_theme.Items.Add(new ListItem("Cool", sJustAppPath + "threadeddisc/themes/cool.css"));
        drp_theme.Items.Add(new ListItem("GraySky", sJustAppPath + "threadeddisc/themes/graysky/graysky.css"));
        drp_theme.Items.Add(new ListItem("Jungle", sJustAppPath + "threadeddisc/themes/jungle.css"));
        drp_theme.Items.Add(new ListItem("Modern", sJustAppPath + "threadeddisc/themes/modern.css"));
        drp_theme.Items.Add(new ListItem("Royal", sJustAppPath + "threadeddisc/themes/royal.css"));
        drp_theme.Items.Add(new ListItem("Slate", sJustAppPath + "threadeddisc/themes/slate.css"));
        drp_theme.Items.Add(new ListItem("Techno", sJustAppPath + "threadeddisc/themes/techno.css"));
        tr_moderate.Visible = false;
        if ((type == "" || type == "prop") && m_refContentApi.RequestInformationRef.EnableReplication == true)
        {
            bool bShowDynReplication = true;
            Ektron.Cms.FolderData tmp_folder_Data = null;
            tmp_folder_Data = _EkContentRef.GetFolderById(_DiscussionBoard.ParentId);
            if (tmp_folder_Data.FolderType == Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.FolderType.Community))
            {
                bShowDynReplication = false;
            }
            if (bShowDynReplication)
            {
                ltr_dyn_repl.Text = base.GetMessage("lbl folderdynreplication");
                if (_DiscussionBoard.ReplicationMethod == 1)
                {
                    chk_repl.Checked = true;
                }
                chk_repl.Text = base.GetMessage("replicate folder contents");
            }
            else
            {
                hidden_dyn_repl.Text = "<input type=\"hidden\" name=\"EnableReplication\" value=\"1\" />";
                chk_repl.Visible = false;
            }

        }
        else
        {
            tr_repl.Visible = false;
        }

        if (type == "prop")
        {
            liTerms.Visible = true;
            dvTerms.Visible = true;
            dvBreadCrumb.Visible = true;
            liBreadCrumb.Visible = true;
            chkInheritSitemapPath.Checked = true;
            ltInheritSitemapPath.Text = m_refMsg.GetMessage("lbl Inherit Parent Configuration");
            ltr_boardid.Text = GetMessage("lbl boardid");
            liSubjects.Visible = true;
            dvSubjects.Visible = true;
        }
        else if (type == "cat")
        {
            pnlTabContainer.Visible = false;
            ltr_catname.Text = GetMessage("lbl discussionforumsubject") + ":";
            ltr_catid.Text = GetMessage("generic id") + ":";
            ltr_catsort.Text = GetMessage("lbl discussionforumsortorder") + ":";
            pnlSubjectEdit.Visible = true;
        }
        else if (type == "delcat")
        {
            pnlTabContainer.Visible = false;
            ltr_delcatname.Text = GetMessage("lbl discussionforumsubject") + ":";
            ltr_movecat.Text = GetMessage("lbl move subject forums") + ":";
            pnlSubjectDelete.Visible = true;
        }
        else
        {
            liSubjects.Visible = true;
            dvSubjects.Visible = true;
            liTerms.Visible = true;
            dvTerms.Visible = true;
            dvBreadCrumb.Visible = true;
            liBreadCrumb.Visible = true;
            ltr_boardid.Text = GetMessage("lbl boardid");
            chk_repl.Enabled = false;
            chkInheritSitemapPath.Disabled = true;
            ltrInheritSitemapPath.Text = m_refMsg.GetMessage("lbl Inherit Parent Configuration");
            pnlBreadCrumbEdit.Visible = false;
            ltInheritSitemapPath.Text = m_refMsg.GetMessage("lbl Inherit Parent Configuration");
        }
    }

    private void SetDisabled()
    {
        txt_adb_boardname.Enabled = false;
        txt_adb_title.Enabled = false;
        chk_adb_mc.Enabled = false;
        txt_acceptedhtml.Enabled = false;
        txt_acceptedextensions.Enabled = false;
        chk_adb_ra.Enabled = false;
        chk_lock_board.Enabled = false;
        txt_adb_stylesheet.Enabled = false;
        drp_theme.Enabled = false;
        txt_maxfilesize.Enabled = false;
        _IsTaxonomyUiEnabled = false;
    }

    private string ProcessCSV(string sText, string stype)
    {
        ArrayList alTmp = new ArrayList();
        string[] aValues = sText.Split(',');
        if (aValues.Length > 0)
        {
            for (int i = 0; i <= (aValues.Length - 1); i++)
            {
                aValues[i] = (string)(aValues[i].Trim().ToLower());
                if (stype == "html")
                {
                    if (aValues[i].IndexOf("<") == 0) //trim the < if someone added it.
                    {
                        aValues[i] = (string)(aValues[i].Substring(1));
                    }
                    if (aValues[i].IndexOf(">") == (aValues[i].Length - 1) && aValues[i].Length > 0) //trim the < if someone added it.
                    {
                        aValues[i] = (string)(aValues[i].Substring(0, System.Convert.ToInt32(aValues[i].Length - 1)));
                    }
                }
                else if (stype == "ext")
                {
                    if (aValues[i].IndexOf(".") == 0) //trim the . if someone added it.
                    {
                        aValues[i] = (string)(aValues[i].Substring(1));
                    }
                }
                alTmp.Add(aValues[i]);
            }
        }
        if (alTmp.Count > 0)
        {
            aValues = (string[])alTmp.ToArray(typeof(string));
            sText = string.Join(",", aValues); //re join
        }
        else
        {
            sText = "";
        }
        return sText;
    }

    private void ShowDropUploader(bool bShow)
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();
        sJS.Append("<script language=\"Javascript\">" + "\r\n");
        if (bShow)
        {
            sJS.Append("top.ShowDragDropWindow();" + "\r\n");
        }
        else
        {
            sJS.Append("var dragDropFrame = top.GetEkDragDropObject();" + "\r\n");
            sJS.Append("	if (dragDropFrame != null) {" + "\r\n");
            sJS.Append("		dragDropFrame.location.href = \"blank.htm\";" + "\r\n");
            sJS.Append("	}" + "\r\n");
            sJS.Append("top.HideDragDropWindow();" + "\r\n");
        }
        sJS.Append("</script>" + "\r\n");
        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "DragUploaderJS", sJS.ToString());
    }

    private string DrawContentTemplatesHeader()
    {
        StringBuilder str = new StringBuilder();
        str.Append("<table class=\"ektronForm\" width=\"100%\"><tbody id=\"templateTable\" name=\"templateTable\">");
        return str.ToString();
    }

    private string DrawContentTemplatesFooter()
    {
        return "</tbody></table>";
    }

    private void DrawContentTemplatesTable(string sMode)
    {
        TemplateData[] active_templates;
        active_templates = m_refContentApi.GetEnabledTemplatesByFolder(_DiscussionBoard.Id);

        TemplateData[] template_data;
        template_data = m_refContentApi.GetAllTemplates("TemplateFileName");

        int k = 0;
        int row_id = 0;
        Collection addNew = new Collection();

        StringBuilder str = new StringBuilder();

        Collection ActiveTemplateIdList = new Collection();
        for (k = 0; k <= active_templates.Length - 1; k++)
        {
            if (!ActiveTemplateIdList.Contains(active_templates[k].Id.ToString()))
            {
                ActiveTemplateIdList.Add(active_templates[k].Id, active_templates[k].Id.ToString(), null, null);
            }
        }

        if (!ActiveTemplateIdList.Contains(_DiscussionBoard.TemplateId.ToString()))
        {
            ActiveTemplateIdList.Add(_DiscussionBoard.TemplateId, _DiscussionBoard.TemplateId.ToString(), null, null);
        }

        str.Append(this.m_refContentApi.SitePath);
        if (sMode == "edit")
        {
            str.Append("<select name=\"addTemplate\" id=\"addTemplate\">");
            str.Append("<option value=\"0\">" + "[Select Template]" + "</option>");
        }
        else
        {
            str.Append("<select name=\"addTemplate\" id=\"addTemplate\" disabled>");
            str.Append("<option value=\"0\">" + "[No Template Selected]" + "</option>");
        }
        for (k = 0; k <= template_data.Length - 1; k++)
        {
            str.Append("<option value=\"" + template_data[k].Id + "\" " + ((template_data[k].Id == _DiscussionBoard.TemplateId) ? "selected" : "") + ">" + template_data[k].FileName + "</option>");
        }
        str.Append("</select> ");
        str.Append("<a href=\"#\" Onclick=\"javascript:PreviewTemplate(\'" + m_refContentApi.SitePath + "\', 800,600);return false;\"><img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/preview.png" + "\" alt=\"Preview Template\" title=\"" + m_refMsg.GetMessage("lbl preview template") + "\"/></a>");

        if (row_id % 2 == 0)
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"1\" />");
        }
        else
        {
            str.Append("<input type=\"hidden\" name=\"tisEven\" id=\"tisEven\" value=\"0\" />");
        }
        if (sMode == "edit" && m_refContentApi.RequestInformationRef.IsMembershipUser != 1)
        {
            str.Append("<div id=\"div3\" style=\"display: none;position: block;\"></div>");
            str.Append("<div id=\"contentidspan\" style=\"display: block;position: block;\">");
            str.Append("<div class=\"ektronTopSpace\"></div>");
            str.Append("<a href=\"#\" onclick=\"javascript:LoadChildPage();return true;\" class=\"button buttonInlineBlock greenHover buttonAdd\">");
            str.Append(m_refMsg.GetMessage("lbl add template"));
            str.Append("</a>");
            str.Append("</div>");
        }
        str.Append("<div id=\"FrameContainer\" class=\"ektronWindow ektronModalStandard\">");
        str.Append("<iframe id=\"ChildPage\" name=\"ChildPage\" src=\"javascript:false;\">");
        str.Append("</iframe>");
        str.Append("</div>");
        template_list.Text = str.ToString();
    }

    private void ProcessContentTemplatesPostBack()
    {
        TemplateData[] template_data;
        Collection xml_active_list = new Collection();
        Collection template_active_list = new Collection();
        long default_xml_id = -1;

        template_data = m_refContentApi.GetAllTemplates("TemplateFileName");

        for (int i = 0; i <= template_data.Length - 1; i++)
        {
            if (Convert.ToInt64(Request.Form["addTemplate"]) == template_data[i].Id)
            {
                template_active_list.Add(template_data[i].Id, template_data[i].Id.ToString(), null, null);
                _NewTemplateName = template_data[i].FileName;
                _NewTemplateId = template_data[i].Id;
            }
        }

        m_refContentApi.EkContentRef.UpdateBoardQuicklinks(this._FolderData.Id, _OldTemplateName, _NewTemplateName, _NewTemplateId);

        m_refContentApi.UpdateForumFolderMultiConfig(this.m_iID, default_xml_id, _NewTemplateId, template_active_list, xml_active_list);
    }

    #endregion

    #region JS, CSS

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.dom.js", "EktronDomUtilsJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "threadeddisc/forumsitemap.js", "EktronForumSiteMapJS");
    }

    private void RegisterCSS()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        if (usesModal)
        {
            modalCss.Text = "#FrameContainer{ margin: -150px 0 0 -280px !important; }";
        }
    }
    private void DisplaySitemapPath()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        sJS.AppendLine("var clientName_chkInheritSitemapPath = \'chkInheritSitemapPath\';");
        if (_FolderData.SitemapInherited == 1 && _FolderData.Id != 0)
        {
            //chkInheritSitemapPath.Checked = True
            sJS.AppendLine("document.getElementById(\"hdnInheritSitemap\").value = \'true\';");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").checked = true;");
            sJS.AppendLine("document.getElementById(\"AddSitemapNode\").style.display = \'none\';");
        }
        else
        {
            sJS.AppendLine("document.getElementById(\"hdnInheritSitemap\").value = \'false\';");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").checked = false;");
            //chkInheritSitemapPath.Checked = False
        }
        if (_FolderData.Id == 0)
        {
            //chkInheritSitemapPath.Disabled = True
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.dom.js", "EktronDomUtilsJS");
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "controls/folder/sitemap.js", "EktronSitemapJS");
            sJS.AppendLine("document.getElementById(\"chkInheritSitemapPath\").disable = true;");
            sJS.AppendLine("document.getElementById(\"dvInheritSitemap\").style.display = \'none\';");
        }
        if (_FolderData.SitemapPath != null)
        {
            sJS.Append("arSitemapPathNodes = new Array(");
            foreach (Ektron.Cms.Common.SitemapPath node in _FolderData.SitemapPath)
            {
                if (node != null)
                {
                    if (node.Order != 0)
                    {
                        sJS.Append(",");
                    }
                    sJS.Append("new node(\'" + node.Title + "\',\'" + node.Url + "\',\'" + node.Description + "\'," + node.Order + ")");
                }
            }
            sJS.AppendLine(");");
            sJS.AppendLine("renderSiteMapNodes();");
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "renderSitepath", sJS.ToString(), true);
    }

    private void ViewSitemapPath()
    {
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        pnlEditBreakCrumb.Visible = false;
        if (Convert.ToBoolean(_FolderData.SitemapInherited))
        {
            chk_InheritSitemapPath.Checked = true;
        }
        else
        {
            chk_InheritSitemapPath.Checked = false;
        }
        chk_InheritSitemapPath.Disabled = true;
        if (_FolderData.Id == 0)
        {
            pnlInheritSitemapPath.Visible = false;
        }
        else
        {
            pnlInheritSitemapPath.Visible = true;
        }
        if (_FolderData.SitemapPath != null)
        {
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.dom.js", "EktronDomUtilsJS");
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "controls/folder/sitemap.js", "EktronSitemapJS");

            sJS.Append("arSitemapPathNodes = new Array(");
            foreach (Ektron.Cms.Common.SitemapPath node in _FolderData.SitemapPath)
            {
                if (node != null)
                {
                    if (node.Order != 0)
                    {
                        sJS.Append(",");
                    }
                    sJS.Append("new node(\'" + Server.HtmlDecode(node.Title).Replace("\'", "\\\'") + "\',\'" + node.Url + "\',\'" + node.Description + "\'," + node.Order + ")");
                }
            }
            sJS.AppendLine(");");
            sJS.AppendLine("previewSitemapPath();");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "previewSitemapPath", sJS.ToString(), true);
        }
        else
        {
            chk_InheritSitemapPath.Visible = false;
            ltInheritSitemapPath.Visible = true;
            ltInheritSitemapPath.Text = m_refMsg.GetMessage("lbl breadcrumb not created");
        }
    }
    #endregion
}


