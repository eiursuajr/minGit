using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
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
using Ektron.Editors.JavascriptEditorControls;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.Workarea;
using Microsoft.Security.Application;

public partial class membership_add_content : Ektron.Cms.Workarea.workareabase
{

    private JavascriptEditor _InnerEditor;
    // Private _jsetoolbar As String = "ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyCenter,JustifyRight,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,Wiki,InsertRule,EkLibrary|Cut,Copy,Paste;Undo,Redo,Print"
    private string _jsetoolbar = "ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyCenter,JustifyRight,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage{0},InsertRule{1}|Cut,Copy,Paste;Undo,Redo,Print|InsertTable,EditTable;InsertTableRowAfter,InsertTableRowBefore,DeleteTableRow;InsertTableColumnAfter,InsertTableColumnBefore,DeleteTableColumn";
    private long ModeID = 0;
    private string cssFilesPath = "";
    private CurrentMode Mode = CurrentMode.Add;
    private Ektron.Cms.Controls.ContentBlock cb = new Ektron.Cms.Controls.ContentBlock();
    private PermissionData security_data;
    protected string TaxonomyTreeIdList = "";
    protected string TaxonomyTreeParentIdList = "";
    private int LangID = -1;
    protected bool bWithLang = false;
    protected int iOrigLang = 0;
    protected int iNewLang = 0;
    protected FolderData folder_data;
    protected string updateFieldId = "";
    protected string commparams = "";
    protected bool TaxonomyRoleExists = false;
    protected long m_intTaxFolderId = 0;
    protected long TaxonomyOverrideId = 0;
    protected bool IsForum = false;
    protected bool bDynamicBox = false;
    protected long TaxonomySelectId = 0;
    protected string m_SelectedEditControl = "";
    protected System.Web.HttpCookie objCookieObject;

    private enum CurrentMode
    {
        Add,
        Edit
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        m_SelectedEditControl = Utilities.GetEditorPreference(Request);
        objCookieObject = Ektron.Cms.CommonApi.GetEcmCookie();
        if (objCookieObject != null && !(objCookieObject.Values["editoroptions"] == null))
        {
            m_SelectedEditControl = objCookieObject.Values["editoroptions"].ToLower();
        }
        if (m_SelectedEditControl.ToLower() == "jseditor")
        {
            cdContent_teaser.Visible = false;
        }
        bool bPermissions = true;
        m_refContentApi = new Ektron.Cms.ContentAPI();
        iNewLang = m_refContentApi.ContentLanguage;
        if (!String.IsNullOrEmpty(Request.QueryString["dynamicbox"]))
        {
            bDynamicBox = Convert.ToBoolean(Request.QueryString["dynamicbox"]);
        }
        if (!String.IsNullOrEmpty(Request.QueryString["forum"]))
        {
            try
            {
                IsForum = Convert.ToBoolean(Convert.ToInt32(Request.QueryString["forum"]));
            }
            catch (Exception)
            {
                IsForum = false;
            }
        }
        setlabels();
        if (!IsForum && m_SelectedEditControl.ToLower() == "jseditor")
        {
            AddEkDoPostBack();
        }
        this.dialog_publish.Attributes.Add("onclick", "return publish_handler();");
        this.dialog_publish_top.Attributes.Add("onclick", "return publish_handler();");
        this.dialog_publish_asset.Attributes.Add("onclick", "return publish_handler();");
        if (!String.IsNullOrEmpty(Request.QueryString["mode"]))
        {
            if (Request.QueryString["mode"].Trim().ToLower() == "edit")
            {
                Mode = CurrentMode.Edit;
            }
            else if (Request.QueryString["mode"].Trim().ToLower() == "addlang")
            {
                Mode = CurrentMode.Edit;
                bWithLang = true;
                if (!String.IsNullOrEmpty(Request.QueryString["Lang"]))
                    iOrigLang = Convert.ToInt32(Request.QueryString["Lang"]);
            }
            else
            {
                Mode = CurrentMode.Add;
            }
        }

        SetCSS();
        if (!String.IsNullOrEmpty(Request.QueryString["mode_id"]))
        {
            try
            {
                ModeID = Convert.ToInt64(Request.QueryString["mode_id"]);
            }
            catch (Exception)
            {
                ModeID = 0;
            }
        }

        if (!String.IsNullOrEmpty(Request.QueryString["lang_id"]))
        {
            try
            {
                LangID = Convert.ToInt32(Request.QueryString["lang_id"]);
            }
            catch (Exception)
            {
                LangID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
            }
        }
        if (!String.IsNullOrEmpty(Request.QueryString["langtype"]))
        {
            try
            {
                LangID = Convert.ToInt32(Request.QueryString["langtype"]);
            }
            catch (Exception)
            {
                LangID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
            }
        }
        if (LangID == -1 || LangID == 0)
        {
            LangID = m_refContentApi.RequestInformationRef.DefaultContentLanguage;
        }

        m_refContentApi.RequestInformationRef.ContentLanguage = LangID;
        m_refContentApi.ContentLanguage = LangID;

        if (!Page.IsPostBack)
        {
            if (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, m_refContentApi.RequestInformationRef.UserId, false))
            {
                TaxonomyRoleExists = true;
            }
            switch (Mode)
            {
                case CurrentMode.Add:
                    auto_generate_summary.Visible = false;
                    content_id.Value = "0";
                    security_data = m_refContentApi.LoadPermissions(ModeID, "folder", 0);

                    if (!security_data.CanAdd)
                    {
                        title_label.Text = "You do not have rights to add content in FolderID=" + ModeID;
                        bPermissions = false;
                    }
                    else
                    {
                        Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
                        folder_data = folderApi.GetFolder(ModeID);
                        if (folder_data != null)
                        {
                            if (cssFilesPath == "") //apply stylesheet from folderdata
                            {
                                cssFilesPath = this.m_refContentApi.RequestInformationRef.SitePath + folder_data.StyleSheet;
                            }

                            if (cssFilesPath.Length > 0)
                            {
                                cdContent_teaser.Stylesheet = cssFilesPath;
                            }
                        }
                        cdContent_teaser.FolderId = ModeID;

                        if (!IsForum && m_SelectedEditControl.ToLower() == "jseditor")
                        {
                            InnerEditor.ToolbarLayout = SetToolbar();
                            ftb_control.Text = InnerEditor.ToString();
                            ftb_control.Visible = true;
                            cdContent_teaser.Visible = false;
                        }
                        else if (!IsForum)
                        {
                            ftb_control.Visible = false;
                            cdContent_teaser.Visible = true;
                        }
                        SetTaxonomy(0, ModeID);
                    }
                    break;
                case CurrentMode.Edit:
                    auto_generate_summary.Visible = true;
                    content_id.Value = ModeID.ToString();
                    if (!String.IsNullOrEmpty(Request.QueryString["mode"]) && Request.QueryString["mode"].Trim().ToLower() == "addlang")
                    {
                        long folderid = 0;
                        if (!String.IsNullOrEmpty(Request.QueryString["folder"]))
                        {
                            long.TryParse(Request.QueryString["folder"], out folderid);
                        }
                        security_data = m_refContentApi.LoadPermissions(folderid, "folder", 0);
                        if (!security_data.CanAdd)
                        {
                            title_label.Text = "You do not have rights to add language content block ID=" + ModeID;
                            bPermissions = false;
                        }
                    }
                    else
                    {
                        security_data = m_refContentApi.LoadPermissions(ModeID, "content", 0);
                        if (!security_data.CanEdit && !IsForum)
                        {
                            title_label.Text = "You do not have rights to edit content block ID=" + ModeID;
                            bPermissions = false;
                        }
                        else
                        {
                            Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
                            folder_data = folderApi.GetFolder(ModeID);
                            if (folder_data != null)
                            {
                                if (cssFilesPath == "") //apply stylesheet from folderdata
                                {
                                    cssFilesPath = this.m_refContentApi.RequestInformationRef.SitePath + folder_data.StyleSheet;
                                }

                                if (cssFilesPath.Length > 0)
                                {
                                    cdContent_teaser.Stylesheet = cssFilesPath;
                                }
                            }
                        }
                    }

                    if (bPermissions)
                    {
                        if (Request.QueryString["ctlupdateid"] != "")
                        {
                            commparams = (string)("&ctlupdateid=" + Request.QueryString["ctlupdateid"] + "&ctlmarkup=" + Request.QueryString["ctlmarkup"] + "&cltid=" + Request.QueryString["cltid"] + "&ctltype=" + Request.QueryString["ctltype"]);
                            updateFieldId = Request.QueryString["ctlupdateid"];
                            Page.ClientScript.RegisterHiddenField("ctlupdateid", updateFieldId);
                        }
                        if (Request.QueryString["ctlmarkup"] != "")
                        {
                            Page.ClientScript.RegisterHiddenField("ctlmarkup", Request.QueryString["ctlmarkup"]);
                        }
                        if (Request.QueryString["ctltype"] != "")
                        {
                            Page.ClientScript.RegisterHiddenField("ctltype", Request.QueryString["ctltype"]);
                        }
                        if (Request.QueryString["cltid"] != "")
                        {
                            Page.ClientScript.RegisterHiddenField("cltid", Request.QueryString["cltid"]);
                        }
                        SetContentBlock();
                    }
                    break;
            }
            if (!bPermissions)
            {
                ftb_control.Visible = false;
                title_value.Visible = false;
                dialog_publish.Visible = false;
                dialog_publish_top.Visible = false;
                dialog_publish_asset.Visible = false;
                cdContent_teaser.Visible = false;
                return;
            }
            ltr_js.Text = this.EditorJS();
            if (m_SelectedEditControl.ToLower() != "jseditor")
            {
                ftb_control.Visible = false;

                //set the equavalent SetToolbar() to contentdesigner
                if (cdContent_teaser != null)
                {
                    string ToolsOption = "";
                    if (security_data.CanAdd)
                    {
                        ToolsOption = "Wiki=1";
                    }
                    bool bLibraryAllowed = false;
                    if (security_data.IsReadOnlyLib)
                    {
                        bLibraryAllowed = true;
                    }
                    ToolsOption = ToolsOption + ("&LibraryAllowed=" + bLibraryAllowed.ToString());
                    bool bCanModifyImg = false;
                    if (security_data.CanAddToImageLib)
                    {
                        bCanModifyImg = true;
                    }
                    ToolsOption = ToolsOption + ("&CanModifyImg=" + bCanModifyImg.ToString());
                    if (ToolsOption.Length > 0)
                    {
                        ToolsOption = (string)("?" + ToolsOption);
                    }
                    cdContent_teaser.SetPermissions(security_data);
                    cdContent_teaser.ToolsFile = this.m_refContentApi.RequestInformationRef.ApplicationPath + "ContentDesigner/configurations/InterfaceBlog.aspx" + ToolsOption;

                    if (!(Request.QueryString["editorVisible"] == null))
                    {
                        //To avoid the editor's onbeforeunload checks being called
                        cdContent_teaser.Visible = System.Convert.ToBoolean(Request.QueryString["editorVisible"]);
                    }
                }
            }
        }

        if (dialog_publish.Visible == true)
        {
            tr_pub.Visible = true;
            tr_asset.Visible = false;
        }
        else
        {
            tr_pub.Visible = false;
            tr_asset.Visible = true;
        }

        string _helpUrl = string.Empty;
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ek_helpDomainPrefix"]))
        {
            string helpDomain = ConfigurationManager.AppSettings["ek_helpDomainPrefix"];
            Uri _uri = new Uri(helpDomain);
            if (_uri != null && !_uri.IsFile)
            {
                if ((helpDomain.IndexOf("[ek_cmsversion]") > 1))
                {
                    //defect # 64951 - This help file (It had been its own help project in previous releases as well).
                    _helpUrl = " http://documentation.ektron.com/current/memberhelp/wwhelp/wwhimpl/js/html/wwhelp.htm";
                }
            }
            else
            {
                _helpUrl = this.m_refContentApi.RequestInformationRef.ApplicationPath + "/helpmessage.aspx?error=isfile";
            }
        }
        else
        {
            _helpUrl = this.m_refContentApi.RequestInformationRef.ApplicationPath + "/help/memberhelp/index.html";
        }
        help_button.Text = "<a href=\"#\"><img  id=\"DeskTopHelp\" title=\"" + this.m_refMsg.GetMessage("alt help button text") + "\"  border=\"0\" src=\"" + this.m_refContentApi.RequestInformationRef.ApplicationPath + "/images/application/menu/help.gif\" onclick=\"javascript:PopUpWindow(\'" + _helpUrl + "\', \'SitePreview\', 600, 500, 1, 1);return false;\"></a>";

        if (IsForum)
        {
            SetForumMode();
        }
    }

    private void SetContentBlock()
    {
        long ifolderid = 0;
        //cb.EnableMembershipEditing = True
        if (this.bWithLang == true)
        {
            cb.LanguageID = this.iOrigLang;
        }
        cb.DefaultContentID = ModeID;
        cb.LanguageID = m_refContentApi.ContentLanguage;
        cb.Page = this;
        cb.Fill();
        ifolderid = cb.EkItem.FolderId;
        if (m_SelectedEditControl == "jseditor")
        {
            InnerEditor.Text = cb.EkItem.Html;
        }
        else
        {
            cdContent_teaser.Content = cb.EkItem.Html;
            cdContent_teaser.FolderId = ifolderid;

        }

        if (this.bWithLang == true)
        {
            m_refContentApi.ContentLanguage = this.iOrigLang;
        }
        security_data = m_refContentApi.LoadPermissions(cb.EkItem.FolderId, "folder", 0);

        if (m_SelectedEditControl.ToLower() == "jseditor")
        {
            InnerEditor.ToolbarLayout = SetToolbar();
            ftb_control.Text = InnerEditor.ToString();
        }
        title_value.Text = Server.HtmlDecode(cb.EkItem.Title);
        // content_data = m_refContentApi.GetContentById(ModeID)


        if (IsForum)
        {
            ifolderid = m_refContentApi.GetParentIdByFolderId(cb.EkItem.FolderId);
        }
        if (m_refContentApi.UserId > 0)
        {
            folder_data = m_refContentApi.GetFolderById(ifolderid);
        }
        else
        {
            folder_data = new FolderData();
        }
        if (cb.EkItem.Teaser.Trim() == "")
        {
            auto_generate_summary.Visible = false;
        }
        if (cb.EkItem.AssetInfo != null && cb.EkItem.AssetInfo.Id.Length > 0)
        {
            Ektron.Cms.UI.CommonUI.ApplicationAPI AppUI = new Ektron.Cms.UI.CommonUI.ApplicationAPI();
            ltr_js.Visible = false;
            title_label.Visible = false;
            title_value.Visible = false;
            change_notification.Visible = false;
            ftb_control.Visible = false;
            dialog_publish.Visible = false;
            dialog_publish_top.Visible = false;
            content_block_view.Visible = true;
            content_block_overwrite.Visible = true;
            ContentBase _ekItem = cb.EkItem;
            content_block_view.Text = AppUI.ecmNextGenContentBlock(ref _ekItem, "", 0);
            content_block_overwrite.DefaultAssetID = ModeID;
            cdContent_teaser.Visible = false;
        }

        SetTaxonomy(ModeID, ifolderid);
    }

    private JavascriptEditor InnerEditor
    {
        get
        {
            if (_InnerEditor == null)
            {
                _InnerEditor = new JavascriptEditor();
                _InnerEditor.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                _InnerEditor.ID = "EkInnerEditor";
                _InnerEditor.ToolbarLayout = _jsetoolbar;
                // If you want the enter key to create a <br/> tag instead of a <p> tag in IE, then uncomment this line.
                // _innereditor.BreakMode = BreakMode.LineBreak
                _InnerEditor.EnableHtmlMode = true;
                _InnerEditor.Height = new Unit(400, UnitType.Pixel);
                _InnerEditor.Page = Page;
                _InnerEditor.DesignModeCss = m_refContentApi.SitePath + "default.css";
                _InnerEditor.FormatHtmlTagsToXhtml = true;
            }
            return _InnerEditor;
        }
    }

    private void AddEkDoPostBack()
    {
        StringBuilder js = new StringBuilder();
        js.Append("function __EkDoPostBack() {");
        js.Append(Page.ClientScript.GetPostBackEventReference(InnerEditor, "1"));
        js.Append("}");
        Page.ClientScript.RegisterClientScriptBlock(InnerEditor.GetType(), "__EkDoPostBack", js.ToString(), true);
    }
    private void SetTaxonomy(long contentid, long ifolderid)
    {
        EditTaxonomyHtml.Text = "<table class=\"ektrongrid\"><tr><td class=\"info\" style=\"text-align: left !important;\">" + this.m_refMsg.GetMessage("select categories content") + "</td></tr><tr><td id=\"TreeOutput\"></td></tr></table>";
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        m_refContentApi.RequestInformationRef.ContentLanguage = LangID;
        m_refContentApi.ContentLanguage = LangID;
        if (contentid == 0)
        {

        }
        TaxonomyRequest taxonomy_request = new TaxonomyRequest();
        TaxonomyBaseData[] taxonomy_data_arr = null;
        if (this.Mode == CurrentMode.Add)
        {
            if ((Request.QueryString["SelTaxonomyId"] != null) && Request.QueryString["SelTaxonomyId"] != "")
            {
                TaxonomySelectId = Convert.ToInt64(Request.QueryString["SelTaxonomyId"]);
            }
            if (TaxonomySelectId > 0)
            {
                taxonomyselectedtree.Value = TaxonomySelectId.ToString();
                TaxonomyTreeIdList = AntiXss.UrlEncode((string)taxonomyselectedtree.Value);
                taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(TaxonomySelectId, m_refContentApi.ContentLanguage, 0);
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
        else
        {
            taxonomy_cat_arr = m_refContentApi.EkContentRef.ReadAllAssignedCategory(contentid);
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
            TaxonomyTreeIdList = AntiXss.UrlEncode((string)taxonomyselectedtree.Value);
            if (TaxonomyTreeIdList.Trim().Length > 0)
            {
                TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(contentid);
            }
        }

        taxonomy_request.TaxonomyId = ifolderid;
        taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage;
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(ifolderid);

        if ((taxonomy_data_arr == null || taxonomy_data_arr.Length == 0) && (TaxonomyOverrideId == 0))
        {
            EditTaxonomyHtml.Text = "";
            base.Tabs.RemoveAt(1);
        }

        m_intTaxFolderId = ifolderid;
        //If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
        //    TaxonomyOverrideId = Convert.ToInt32(Request.QueryString("TaxonomyId"))
        //End If
        js_taxon.Text = Environment.NewLine;
        js_taxon.Text += "var taxonomytreearr=\"" + TaxonomyTreeIdList + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var taxonomytreedisablearr=\"" + TaxonomyTreeParentIdList + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var __TaxonomyOverrideId=\"" + TaxonomyOverrideId + "\".split(\",\");" + Environment.NewLine;
        js_taxon.Text += "var m_fullScreenView=false;var __EkFolderId = " + ifolderid + ";" + Environment.NewLine;
    }
    protected void Button1_Click(object sender, System.EventArgs e)
    {
        click_handler();
    }
    protected void dialog_publish_top_Click(object sender, System.EventArgs e)
    {
        click_handler();
    }
    protected void click_handler()
    {
        string strContent = "";
        string strSearchText = "";
        string strContentTeaser = "";
        bool bUpdateField = false;

        //If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
        //    TaxonomyOverrideId = Convert.ToInt32(Request.QueryString("TaxonomyId"))
        //End If
        if ((Request.QueryString["SelTaxonomyId"] != null) && Request.QueryString["SelTaxonomyId"] != "")
        {
            TaxonomySelectId = Convert.ToInt64(Request.QueryString["SelTaxonomyId"]);
        }

        switch (Mode)
        {
            case CurrentMode.Add:
                try
                {
                    if (m_SelectedEditControl.ToLower() == "jseditor")
                    {
                        strContent = Page.Server.HtmlDecode((string)(Request.Form["EkInnerEditor"].ToString()));
                    }
                    else
                    {
                        strContent = (string)cdContent_teaser.Content;
                    }
                    strContent = Utilities.WikiQLink(strContent, ModeID);
                    strContentTeaser = Utilities.AutoSummary(strContent);
                    if (strContentTeaser != "")
                    {
                        strContentTeaser = "<p>" + strContentTeaser + "</p>";
                    }
                    Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
                    folder_data = folderApi.GetFolder(ModeID);
                    if (folder_data != null)
                    {
                        ModeID = m_refContentApi.AddContent((string)this.title_value.Text, "", strContent, "", strContentTeaser, m_refContentApi.RequestInformationRef.ContentLanguage.ToString(), ModeID, "", "", "", 0, folder_data.TemplateId);
                    }
                    else
                    {
                        ModeID = m_refContentApi.AddContent((string)this.title_value.Text, "", strContent, "", strContentTeaser, m_refContentApi.RequestInformationRef.ContentLanguage.ToString(), ModeID, "", "", "");
                    }
                    if (TaxonomyOverrideId > 0)
                    {
                        TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
                    }
                    if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
                    {
                        TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                        if (TaxonomyTreeIdList.Trim().EndsWith(","))
                        {
                            TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                        }
                    }

                    TaxonomyContentRequest cotnent_request = new TaxonomyContentRequest();
                    cotnent_request.ContentId = ModeID;
                    cotnent_request.TaxonomyList = TaxonomyTreeIdList;
                    m_refContentApi.AddTaxonomyItem(cotnent_request);
                    Mode = CurrentMode.Edit;
                    ftb_control.Visible = false;
                    cdContent_teaser.Visible = false;
                    this.title_value.Visible = false;
                    this.title_label.Text = "";
                    this.change_notification.Text = "Content Added";
                    this.change_notification.ToolTip = this.change_notification.Text;

                    if (this.TaxonomySelectId > 0)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkReloadTop", "RefreshPage(\'" + TaxonomySelectId.ToString() + "\', " + bDynamicBox.ToString().ToLower() + ");self.close()", true);
                    }
                    else if (this.TaxonomyOverrideId > 0)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkReloadTop", "RefreshPage(\'" + TaxonomyOverrideId.ToString() + "\', " + bDynamicBox.ToString().ToLower() + ");self.close()", true);
                    }
                    else
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkReloadTop", "RefreshPage(\'\', " + bDynamicBox.ToString().ToLower() + ");self.close()", true);
                    }
                }
                catch (Exception ex)
                {
                    ftb_control.Visible = false;
                    title_value.Visible = false;
                    cdContent_teaser.Visible = false;
                    if (ex.Message.IndexOf("Invalid License") == 0)
                    {
                        title_label.Text = ex.Message.ToString();
                    }
                    else
                    {
                        title_label.Text = "You do not have rights to add content in FolderID=" + ModeID;
                    }
                }
                break;
            case CurrentMode.Edit:
                if (bWithLang == true)
                {
                    Ektron.Cms.Content.EkContent brContent;
                    Collection page_content_data = new Collection();

                    m_refContentApi.ContentLanguage = iOrigLang;
                    brContent = m_refContentApi.EkContentRef;
                    page_content_data = brContent.GetContentByIDv2_0(ModeID);
                    m_refContentApi.ContentLanguage = iNewLang;
                    brContent = m_refContentApi.EkContentRef;

                    if (m_SelectedEditControl.ToLower() == "jseditor")
                    {
                        strContent = Page.Server.HtmlDecode((string)(Page.Request.Form["EkInnerEditor"].ToString()));
                    }
                    else
                    {
                        strContent = (string)cdContent_teaser.Content;
                    }
                    strSearchText = Utilities.StripHTML(strContent);
                    page_content_data.Remove("ContentHtml");
                    strContent = Utilities.WikiQLink(strContent, Convert.ToInt64(page_content_data["FolderID"]));
                    page_content_data.Add(strContent, "ContentHtml", null, null);
                    page_content_data.Remove("ContentLanguage");
                    page_content_data.Add(iNewLang, "ContentLanguage", null, null);
                    if (page_content_data.Contains("ContentTeaser") && (page_content_data["ContentTeaser"].ToString() == ""))
                    {

                        strContentTeaser = Utilities.AutoSummary(strContent);



                        if (strContentTeaser != "")
                        {
                            strContentTeaser = "<p>" + strContentTeaser + "</p>";
                        }
                        page_content_data.Remove("ContentTeaser");
                        page_content_data.Add(strContentTeaser, "ContentTeaser", null, null);
                    }
                    else
                    {
                        if (auto_generate_summary.Checked == true)
                        {
                            // strContentTeaser = Utilities.WikiQLink(strContent, ModeID)
                            strContentTeaser = Utilities.AutoSummary(strContent);



                            if (strContentTeaser != "")
                            {
                                strContentTeaser = "<p>" + strContentTeaser + "</p>";
                            }
                            page_content_data.Remove("ContentTeaser");
                            page_content_data.Add(strContentTeaser, "ContentTeaser", null, null);
                        }
                    }
                    if (page_content_data.Contains("SearchText"))
                    {
                        page_content_data.Remove("SearchText");
                    }
                    page_content_data.Add(strSearchText, "SearchText", null, null);
                    if (page_content_data.Contains("ContentTitle"))
                    {
                        page_content_data.Remove("ContentTitle");
                    }
                    page_content_data.Add(title_value.Text, "ContentTitle", null, null);
                    if (page_content_data.Contains("IsSearchable"))
                    {
                        page_content_data.Remove("IsSearchable");
                    }
                    page_content_data.Add(false, "IsSearchable", null, null);
                    page_content_data.Add(true, "AddToQlink", null, null);
                    if (page_content_data.Contains("Taxonomy"))
                    {
                        page_content_data.Remove("Taxonomy");
                    }

                    if (TaxonomyOverrideId > 0)
                    {
                        TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
                    }
                    if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
                    {
                        TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                        if (TaxonomyTreeIdList.Trim().EndsWith(","))
                        {
                            TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                        }
                    }

                    page_content_data.Add(TaxonomyTreeIdList, "Taxonomy", null, null);

                    ModeID = brContent.AddNewContentv2_0(page_content_data);
                    brContent.SaveContentv2_0(page_content_data);
                    brContent.CheckIn(ModeID, "");
                    brContent.SubmitForPublicationv2_0(ModeID, Convert.ToInt64(page_content_data["FolderID"]), "");

                    Mode = CurrentMode.Edit;
                    ftb_control.Visible = false;
                    cdContent_teaser.Visible = false;
                    this.title_value.Visible = false;
                    this.title_label.Text = "";
                    this.change_notification.Text = "Content Added";
                    this.change_notification.ToolTip = this.change_notification.Text;

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkReloadTop", "top.opener.location.reload();self.close()", true);
                }
                else
                {
                    ContentData cData = m_refContentApi.GetContentById(ModeID, ContentAPI.ContentResultType.Published);
                    Collection page_content_data;
                    bool bError = false;
                    if ((cData != null) && (cData.Status != "O"))
                    {
                        try
                        {

                            m_refContentApi.CheckOutContentById(ModeID);

                            ContentEditData data = m_refContentApi.GetContentForEditing(ModeID);
                            if (m_SelectedEditControl.ToLower() == "jseditor")
                            {
                                data.Html = Page.Server.HtmlDecode((string)(Page.Request.Form["EkInnerEditor"].ToString()));
                            }
                            else
                            {
                                data.Html = (string)cdContent_teaser.Content;
                            }
                            data.Html = Utilities.WikiQLink(data.Html, data.FolderId);
                            data.Title = (string)title_value.Text;
                            if (data.Teaser == "" || data.Teaser.ToLower() == "<br /><!-- wiki summary -->")
                            {

                                //data.Teaser = Utilities.WikiQLink(data.Html, ModeID)
                                data.Teaser = Utilities.AutoSummary(data.Html);



                                if (data.Teaser != "")
                                {
                                    data.Teaser = "<p>" + data.Teaser + "</p>";
                                }
                            }
                            else
                            {
                                if (auto_generate_summary.Checked == true)
                                {
                                    //data.Teaser = Utilities.WikiQLink(data.Html, ModeID)
                                    data.Teaser = Utilities.AutoSummary(data.Html);


                                    if (data.Teaser != "")
                                    {
                                        data.Teaser = "<p>" + data.Teaser + "</p>";
                                    }
                                }
                            }
                            page_content_data = StepConvert(data, cData);
                            if (TaxonomyOverrideId > 0)
                            {
                                TaxonomyTreeIdList = TaxonomyOverrideId.ToString();
                            }
                            if ((Request.Form[taxonomyselectedtree.UniqueID] != null) && Request.Form[taxonomyselectedtree.UniqueID] != "")
                            {
                                TaxonomyTreeIdList = Request.Form[taxonomyselectedtree.UniqueID];
                                if (TaxonomyTreeIdList.Trim().EndsWith(","))
                                {
                                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1);
                                }
                            }

                            page_content_data.Add(TaxonomyTreeIdList, "Taxonomy", null, null);
                            // m_refContentApi.SaveContent(data)
                            m_refContentApi.EkContentRef.SaveContentv2_0(page_content_data);
                            m_refContentApi.PublishContentById(ModeID, data.FolderId, data.LanguageId, "", m_refContentApi.UserId, "");

                            string markupPath = "";
                            string updateContent = "";
                            markupPath = Request.Form["ctlmarkup"];
                            if ((markupPath != null) && markupPath.Length > 0)
                            {
                                markupPath = Request.PhysicalApplicationPath + markupPath;
                            }
                            object ekml = null;
                            if ((markupPath != null) && (HttpContext.Current.Cache[markupPath] != null))
                            {
                                ekml = HttpContext.Current.Cache[markupPath];
                                ContentBase results = m_refContentApi.EkContentRef.LoadContent(ModeID, false);
                                updateContent = this.m_refContentApi.FormatOutput((string)ekml.GetType().GetProperty("ContentFormat").GetValue(ekml, null), Request.Form["ctltype"], results);
                                updateContent = this.m_refContentApi.WrapAjaxToolBar(updateContent, results, commparams);
                            }
                            else
                            {
                                updateContent = data.Html;
                            }
                            if ((Request.Form["ctlupdateid"] != null) && Request.Form["ctlupdateid"] != "")
                            {
                                Page.ClientScript.RegisterHiddenField("updatefieldcontent", updateContent);
                                StringBuilder strJs = new StringBuilder();
                                strJs.Append("<script language=\"JavaScript1.2\" type=\"text/javascript\"> ").Append("\r\n");
                                strJs.Append(" if (top.opener != null) { ").Append("\r\n");
                                strJs.Append("      var objUpdateField = top.opener.document.getElementById(\'" + Request.Form["ctlupdateid"] + "\');").Append("\r\n");
                                strJs.Append("      if (objUpdateField != null) { objUpdateField.innerHTML = document.getElementById(\"updatefieldcontent\").value; }").Append("\r\n");
                                strJs.Append(" }").Append("\r\n");
                                strJs.Append("self.close();").Append("\r\n");
                                strJs.Append("</script>").Append("\r\n");
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "objUpdateField", strJs.ToString(), false);
                                bUpdateField = true;
                            }
                            this.change_notification.Text = "Changes Published";
                            this.change_notification.ToolTip = this.change_notification.Text;
                        }
                        catch
                        {
                            this.change_notification.Text = "Another user is editing this content - Your changes could not be saved.";
                            this.change_notification.ToolTip = this.change_notification.Text;
                            bError = true;
                        }
                    }
                    else
                    {
                        this.change_notification.Text = "Content is checked out - no changes made.";
                        this.change_notification.ToolTip = this.change_notification.Text;
                    }
                    if (!bUpdateField)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "__EkReloadTop", (string)((bError ? "alert(\'" + this.change_notification.Text + "\');" : "") + "RefreshPage(\'\', " + bDynamicBox.ToString().ToLower() + ");self.close()"), true);
                    }
                    SetContentBlock();
                }
                break;
        }
    }
    private string SetToolbar()
    {
        _jsetoolbar = string.Format(_jsetoolbar, security_data.CanAdd ? ",Wiki" : "", security_data.IsReadOnlyLib ? ",EkLibrary" : "");
        return _jsetoolbar;
    }
    private string EditorJS()
    {
        StringBuilder sbJS = new StringBuilder();
        long iObj = 0;
        if (this.Mode == CurrentMode.Add)
        {
            iObj = ModeID;
        }
        else if (this.Mode == CurrentMode.Edit)
        {
            iObj = cb.EkItem.FolderId;
        }
        sbJS.Append(Environment.NewLine);
        sbJS.Append("<input type=\"hidden\" name=\"_ekfolderid\" id=\"_ekfolderid\" value=\"" + iObj.ToString() + "\" />").Append(Environment.NewLine);
        sbJS.Append("   <script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("   var _eWebEditProPath = \'" + this.m_refContentApi.RequestInformationRef.ApplicationPath + this.m_refContentApi.RequestInformationRef.AppeWebPath + "\';").Append(Environment.NewLine);
        sbJS.Append("   function JSEInsert(mytext) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(Environment.NewLine);
        sbJS.Append("       FTB_API[\'EkInnerEditor\'].InsertHtml(mytext); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   function JSEURLInsert(myurl, mytext) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(Environment.NewLine);
        sbJS.Append("       FTB_API[\'EkInnerEditor\'].CreateURLLink(myurl, mytext); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   function JSEIMGInsert(myurl, mytitle) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(Environment.NewLine);
        sbJS.Append("       FTB_API[\'EkInnerEditor\'].CreateIMGtag(myurl, mytitle); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   function EkLibrary_locationhandler(selectedtext) { ").Append(Environment.NewLine);
        sbJS.Append("       return \'" + m_refContentApi.RequestInformationRef.ApplicationPath + "mediainsert.aspx?scope=all&action=ViewLibraryByCategory&folder=" + iObj.ToString() + "&type=images&dentrylink=0&EditorName=JSEditor&enhancedmetaselect=&selectids=&selecttitles=&separator=&metadataformtagid=&selected=\' + selectedtext; ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   function EkLibrary_closehandler() { ").Append(Environment.NewLine);
        sbJS.Append("       try {ewebchildwin.close();} catch(ex) {} ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   function publish_handler() {").Append(Environment.NewLine);
        sbJS.Append("     var hiddenText = document.getElementById(\'hdnSearchText\');").Append(Environment.NewLine);
        sbJS.Append("     var searchText = document.getElementById(\'title_value\');").Append(Environment.NewLine);
        sbJS.Append("     var updateText = searchText.value;").Append(Environment.NewLine);
        //sbJS.Append("     if(updateText.indexOf(\'<\') != -1)").Append(Environment.NewLine);
        //sbJS.Append("     {").Append(Environment.NewLine);
        //sbJS.Append("         updateText = updateText.replace(/</gi, \'\');").Append(Environment.NewLine);
        //sbJS.Append("     }").Append(Environment.NewLine);
        sbJS.Append("     if(updateText.indexOf(\'<\') != -1 || updateText.indexOf(\'>\') != -1 || updateText.indexOf(\'*\') != -1 || updateText.indexOf(\'\"\') != -1 || updateText.indexOf(\'|\') != -1)").Append(Environment.NewLine);
        sbJS.Append("     {").Append(Environment.NewLine);
        sbJS.Append("         alert(\'Content Title can not contain \", *, <, > or |.\');").Append(Environment.NewLine);
        sbJS.Append("         return false;").Append(Environment.NewLine);
        sbJS.Append("     }").Append(Environment.NewLine);
        //sbJS.Append("     if(updateText.indexOf(\'>\') != -1)").Append(Environment.NewLine);
        //sbJS.Append("     {").Append(Environment.NewLine);
        //sbJS.Append("         updateText = updateText.replace(/>/gi, \'\');").Append(Environment.NewLine);
        //sbJS.Append("     }").Append(Environment.NewLine);
        sbJS.Append("     searchText.value = updateText;").Append(Environment.NewLine);
        sbJS.Append("     hiddenText.value = updateText;").Append(Environment.NewLine);
        sbJS.Append("       EkLibrary_closehandler(); ").Append(Environment.NewLine);
        if (folder_data.CategoryRequired == true && m_refContentApi.EkContentRef.GetAllFolderTaxonomy(folder_data.Id).Length > 0)
        {
            sbJS.Append("   if (Trim(document.getElementById(\'taxonomyselectedtree\').value) == \'\') { ").Append(Environment.NewLine);
            sbJS.Append("       alert(\'" + base.GetMessage("js tax cat req") + "\'); ").Append(Environment.NewLine);
            sbJS.Append("       ShowPane(\'dvCategory\'); ").Append(Environment.NewLine);
            sbJS.Append("       return false; ").Append(Environment.NewLine);
            sbJS.Append("   } else { ").Append(Environment.NewLine);
            sbJS.Append("       Wait(true);").Append(Environment.NewLine);
            sbJS.Append("     if (click) return false;").Append(Environment.NewLine);
            sbJS.Append("     click = true;").Append(Environment.NewLine);
            sbJS.Append("     document.forms[0].submit();").Append(Environment.NewLine);
            sbJS.Append("       return true; ").Append(Environment.NewLine);
            sbJS.Append("   } ").Append(Environment.NewLine);
        }
        else
        {
            sbJS.Append("       Wait(true);").Append(Environment.NewLine);
            sbJS.Append("     if (click) return false;").Append(Environment.NewLine);
            sbJS.Append("     click = true;").Append(Environment.NewLine);
            sbJS.Append("       return true; ").Append(Environment.NewLine);
        }

        sbJS.Append("   } ").Append(Environment.NewLine);

        sbJS.Append("   </script>").Append(Environment.NewLine);

        sbJS.Append(Environment.NewLine);
        return sbJS.ToString();
    }

    protected void setlabels()
    {
        base.Tabs.On();
        base.Tabs.DivPreface = "d";
        base.Tabs.AddTabByString("Content", "dvContent");
        base.Tabs.AddTabByString("Category", "dvCategory");
    }

    protected void dialog_publish_asset_Click(object sender, System.EventArgs e)
    {
        click_handler();
    }

    protected Collection StepConvert(ContentEditData cEditData, ContentData oldData)
    {
        Collection cCol = new Collection();
        cCol.Add(cEditData.StyleSheet, "StyleSheet", null, null);
        cCol.Add(cEditData.Teaser, "ContentTeaser", null, null);
        cCol.Add(cEditData.AssetData, "AssetData", null, null);
        cCol.Add(cEditData.Comment, "Comment", null, null);
        cCol.Add(cEditData.GoLive, "GoLive", null, null);
        cCol.Add(cEditData.Html, "ContentHtml", null, null);
        cCol.Add(cEditData.LastEditDate, "LastEditDate", null, null);
        cCol.Add(cEditData.EndDate, "EndDate", null, null);
        cCol.Add(cEditData.EndDateAction, "EndDateAction", null, null);
        cCol.Add(cEditData.ManualAlias, "ManualAlias", null, null);
        cCol.Add(cEditData.Html, "SearchText", null, null);
        cCol.Add(cEditData.Title, "ContentTitle", null, null);

        cCol.Add(oldData.MediaText, "MediaText", null, null);
        cCol.Add(oldData.Approver, "Approver", null, null);
        cCol.Add(oldData.ApprovalMethod, "ApprovalMethod", null, null);
        cCol.Add(oldData.ContType, "ContentType", null, null);
        cCol.Add(oldData.DateCreated, "DateCreated", null, null);
        cCol.Add(oldData.DisplayDateCreated, "DisplayDateCreated", null, null);
        cCol.Add(oldData.DisplayEndDate, "DisplayEndDate", null, null);
        cCol.Add(oldData.DisplayGoLive, "DisplayGoLive", null, null);
        cCol.Add(oldData.DisplayLastEditDate, "DisplayLastEditDate", null, null);
        cCol.Add(oldData.EditorFirstName, "EditorFirstName", null, null);
        cCol.Add(oldData.EditorLastName, "EditorLastName", null, null);
        //cCol.Add(oldData.EditorUserNames, "EditorUserNames")
        cCol.Add(oldData.FolderId, "FolderId", null, null);
        cCol.Add(oldData.FolderName, "FolderName", null, null);
        cCol.Add(oldData.HistoryId, "HistoryId", null, null);
        cCol.Add(oldData.HyperLink, "HyperLink", null, null);
        cCol.Add(oldData.Id, "ContentID", null, null);
        cCol.Add(oldData.InheritedFrom, "InheritedFrom", null, null);
        cCol.Add(oldData.IsInherited, "IsInherited", null, null);
        cCol.Add(oldData.IsMetaComplete, "IsMetaComplete", null, null);
        cCol.Add(oldData.IsPrivate, "IsPrivate", null, null);
        cCol.Add(oldData.IsPublished, "IsPublished", null, null);
        cCol.Add(oldData.IsSearchable, "IsSearchable", null, null);
        cCol.Add(oldData.IsXmlInherited, "IsXmlInherited", null, null);
        cCol.Add(oldData.LanguageDescription, "LanguageDescription", null, null);
        cCol.Add(oldData.LanguageId, "ContentLanguage", null, null);
        cCol.Add(oldData.LegacyData, "LegacyData", null, null);
        cCol.Add(oldData.ManualAliasId, "ManualAliasId", null, null);
        cCol.Add(oldData.MetaData, "MetaData", null, null);
        cCol.Add(oldData.Path, "Path", null, null);
        cCol.Add(oldData.Permissions, "Permissions", null, null);
        cCol.Add(oldData.Quicklink, "Quicklink", null, null);
        cCol.Add(oldData.Status, "ContentStatus", null, null);
        cCol.Add(oldData.TemplateConfiguration, "TemplateConfiguration", null, null);
        cCol.Add(oldData.Type, "Type", null, null);
        cCol.Add(oldData.Updates, "Updates", null, null);
        cCol.Add(oldData.XmlConfiguration, "XmlConfiguration", null, null);
        cCol.Add(oldData.XmlInheritedFrom, "XmlInheritedFrom", null, null);

        return cCol;
    }

    private void SetForumMode()
    {
        title_label.Visible = false;
        title_value.Visible = false;
        auto_generate_summary.Visible = false;
        help_button.Visible = false;
        dialog_publish.Visible = false;
    }

    private void SetCSS()
    {
        StringBuilder sbCSS = new StringBuilder();

        if (IsForum)
        {
            sbCSS.Append("<link type=\"text/css\" href=\"csslib/memberaddcontent_forum.css\" rel=\"Stylesheet\" />").Append(Environment.NewLine);
            sbCSS.Append("<style type=\"text/css\">#ActionButtons {display:none;}</style>").Append(Environment.NewLine);
        }
        else
        {
            sbCSS.Append("<link type=\"text/css\" href=\"csslib/memberaddcontent.css\" rel=\"Stylesheet\" />").Append(Environment.NewLine);
        }

        ltr_css.Text = sbCSS.ToString();
        sbCSS = null;
    }
}


