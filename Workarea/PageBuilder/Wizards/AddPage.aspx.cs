using System;
using System.Data;
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
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Site;
using Ektron.Cms.API;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.UrlAliasing;


public partial class Workarea_PageBuilder_Wizards_AddPage : System.Web.UI.Page
{
    #region Protected Variables ===========================
    // Protected Variables
    protected string AppImgPath = "";
    protected string mode = "";
    protected int ContentLanguage = -1;
    protected long CurrentUserID = -1;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected UserControl m_wizardsJSResources;
    protected Int64 folderId = 0;
    protected Int64 defaulttaxid = -1;
    protected ContentAPI contentAPI = new ContentAPI();
    protected Ektron.Cms.LibraryConfigData lib_settings_data = new Ektron.Cms.LibraryConfigData();
    FolderData folderData = null;
    #endregion

    #region Private Variables ===========================
    // Private Variables

    #endregion

    #region Page Init ===========================
    protected void Page_Init(object sender, EventArgs e)
    {
        lib_settings_data = this.contentAPI.GetLibrarySettings(0);
        if (Request.QueryString["folderid"] != "" && Request.QueryString["folderid"] != null)
        {
            Int64.TryParse(Request.QueryString["folderid"].ToString(), out folderId);
            pageBuilderFolderID.Value = folderId.ToString();
        }
        folderData = contentAPI.GetFolderById(folderId, true);
        string port = "";
        if (Request.Url.Port != 80)
        {
            port = ":" + Request.Url.Port;
        }
        Summary.Stylesheet = Request.Url.Scheme + "://" + Request.Url.Host + port + contentAPI.RequestInformationRef.SitePath + folderData.StyleSheet;
        
    }
    #endregion

    #region Page Load ===========================
    protected void Page_Load(object sender, EventArgs e)
    {
        // initialize additional variables for later use
        m_refMsg = m_refSiteApi.EkMsgRef;
        AppImgPath = m_refSiteApi.AppImgPath;
        ContentLanguage = m_refSiteApi.ContentLanguage;

        // register necessary JS
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJS(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/js/ektron.pagebuilder.wizards.addpage.js", "EktronPageBuilderWizardsAddPageJS");
        JS.RegisterJS(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS");
        JS.RegisterJS(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/folderbrowser/ui.finder.js", "EktronPageBuilderUIFinderJS");
        JS.RegisterJSBlock(this, "try{ parent.Ektron.PageBuilder.Wizards.Status.doneLoading(); } catch(junk) { }", System.Guid.NewGuid().ToString());

        // register necessary CSS
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.AllIE);
        Css.RegisterCss(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/css/ektron.pagebuilder.wizards.addpage.css", "EktronPageBuilderWizardsAddPageCSS");
        Css.RegisterCss(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/folderbrowser/ui.finder.css", "EktronPageBuilderUIFinderCSS");
        Css.RegisterCss(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/folderbrowser/ui.finder.ie.css", "EktronPageBuilderUIFinderIECSS", Css.BrowserTarget.AllIE);
        Css.RegisterCss(this, m_refSiteApi.AppPath + "PageBuilder/Wizards/folderbrowser/ui.theme.css", "EktronPageBuilderUIThemeCSS");

        finder.InnerHtml = getchildfolders(0);


        // assign resource text as needed
        this.Title = m_refMsg.GetMessage("lbl pagebuilder add page");

        btnFinish.Text = m_refMsg.GetMessage("btn finish");
        btnFinish.ToolTip = btnFinish.Text;
        lblMetaDataTab.Text = m_refMsg.GetMessage("metadata text");
        lblMetaDataTab.ToolTip = lblMetaDataTab.Text;
        lblSummaryTab.Text = m_refMsg.GetMessage("summary text");
        lblSummaryTab.ToolTip = lblSummaryTab.Text;

        //ektronPageBuilderPageLayoutsLabel.Text = m_refMsg.GetMessage("lbl pagebuilder layouts");
        //TODO
        ektronPageBuilderPageLayoutsLabel.Text = ektronPageBuilderPageLayoutsLabel.ToolTip = m_refMsg.GetMessage("lbl wizard guide");
        
        ektronPageBuilderPageLayoutsFolderLabel.Text =m_refMsg.GetMessage("generic folder")+":";
        ektronPageBuilderPageLayoutsFolderLabel.ToolTip = m_refMsg.GetMessage("generic folder");

        ektronPageBuilderPleaseSelectLayout.Text = m_refMsg.GetMessage("lbl pagebuilder please select layout");
        ektronPageBuilderPleaseSelectLayout.ToolTip = ektronPageBuilderPleaseSelectLayout.Text;
        pageBuilderWizardPageTitleLabel.Text = m_refMsg.GetMessage("lbl pagebuilder page title");
        pageBuilderWizardPageTitleLabel.ToolTip = pageBuilderWizardPageTitleLabel.Text;
        pageBuilderWizardAliasLabel.Text = m_refMsg.GetMessage("lbl pagebuilder url alias");
        pageBuilderWizardAliasLabel.ToolTip = pageBuilderWizardAliasLabel.Text;
        pageBuilderWizardAliasPreviewLabel.Text = m_refMsg.GetMessage("lbl pagebuilder url alias preview");
        pageBuilderWizardAliasPreviewLabel.ToolTip = pageBuilderWizardAliasPreviewLabel.Text;
        pageCreationSuccess.Text = m_refMsg.GetMessage("lbl pagebuilder page creation success");
        MetadataTaxonomyIntro.Text = m_refMsg.GetMessage("lbl pagebuilder metadata taxonomy intro");
        redirectPrompt.Text = m_refMsg.GetMessage("lbl pagebuilder redirect prompt");
        redirectPrompt.ToolTip = redirectPrompt.Text;
        lbInvalidAlias.InnerText = m_refMsg.GetMessage("lbl pagebuilder invalid alias");

        lblSitePath.Text = m_refSiteApi.SitePath;

        redirectMessage.Visible = false;

        // Check is aliasing is enabled or not and display urlAlis if needed
        checkAliasingEnabled();

        // Initialize metadata and taxonomy user controls
        InitializeMetaTaxControls();

        // Initialize Folder selector
        if (folderData != null)
        {
            ektronPageBuilderPageLayoutsSelectedFolderPath.Text = folderData.NameWithPath;
        }

        // Populate the templates with the list of available wireframes
        PopulateTemplateList();

        // initialize the Summary Editor
        InitializeEditor();
    }
    #endregion

    #region Public Methods  =============================================

    #endregion
    #region Private Methods =============================================

    private void checkAliasingEnabled()
    {
        // AliasRow is the table row holding the Url / Alias textfield and label.
        Ektron.Cms.UrlAliasing.UrlAliasSettingsApi settingsAliasApi = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
        Ektron.Cms.UrlAliasing.UrlAliasAutoApi autoAliasApi = new UrlAliasAutoApi();

        if (settingsAliasApi.IsManualAliasEnabled){
            if (contentAPI.IsAdmin() || contentAPI.IsARoleMember(EkEnumeration.CmsRoleIds.UrlAliasingAdmin, m_refSiteApi.UserId, false)
                || contentAPI.IsARoleMember(EkEnumeration.CmsRoleIds.EditAlias, m_refSiteApi.UserId, false))
            {
                ManualAliasing.SetActiveView(ManualAliasingEnabled);
                if(folderData == null)
                {
                    folderData = contentAPI.GetFolderById(folderId);
                }
                pageBuilderCreateManualAlias.Enabled = !folderData.AliasRequired;
            }
            else
            {
                ManualAliasing.SetActiveView(ManualAliasingUnallowed);
            }
        }else{
            ManualAliasing.SetActiveView(ManualAliasingDisabled);
        }

        if (settingsAliasApi.IsAutoAliasEnabled)
        {
            //for list of applicable aliases, we need to go up to root, checking for site folders on the way
            List<FolderData> parents = new List<FolderData>();
            FolderData curFolder = folderData;
            long siterootID = 0;
            while(curFolder != null && curFolder.Id != 0){
                if(curFolder.IsDomainFolder) siterootID = curFolder.Id;
                parents.Add(curFolder);
                curFolder = contentAPI.GetFolderById(curFolder.ParentId);
            }

            List<UrlAliasAutoData> aliasList = autoAliasApi.GetList(new PagingInfo(200), siterootID, ContentLanguage, EkEnumeration.AutoAliasSearchField.All, "", EkEnumeration.AutoAliasOrderBy.Active);
            List<string> folderAliases = new List<string>();
            List<UrlAliasAutoData> taxonomyAliases = new List<UrlAliasAutoData>();

            foreach (UrlAliasAutoData alias in aliasList)
            {
                if (alias.IsEnabled)
                {
                    if (alias.AutoAliasType == EkEnumeration.AutoAliasType.Folder)
                    {
                        FolderData aliasrootfolder = parents.Find(delegate(FolderData f) { return (f.Id == alias.SourceId); });
                        if (aliasrootfolder != null)
                        {
                            string folderpath = folderData.NameWithPath;
                            if (alias.ExcludedPath.Length > 0 && alias.ExcludedPath.Trim() != "Please Select")
                            {
                                int index = folderpath.IndexOf(alias.ExcludedPath);
                                if (index >= 0)
                                {
                                    folderpath = folderpath.Remove(index, alias.ExcludedPath.Length);
                                }
                            }
                            if (alias.PageNameType == EkEnumeration.AutoAliasNameType.ContentId) folderpath += "354";
                            if (alias.PageNameType == EkEnumeration.AutoAliasNameType.ContentIdAndLanguage) folderpath += "354/1033";
                            if (alias.PageNameType == EkEnumeration.AutoAliasNameType.ContentTitle) folderpath += "Title";
                            folderpath += alias.FileExtension;
                            folderAliases.Add(folderpath);
                        }
                    }
                    else //taxonomy
                    {
                        if (folderData != null && folderData.FolderTaxonomy != null)
                        {
                            foreach (TaxonomyBaseData cat in folderData.FolderTaxonomy)
                            {
                                if (cat.TaxonomyId == alias.SourceId)
                                {
                                    taxonomyAliases.Add(alias);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (taxonomyAliases.Count > 0)
                TaxonomyAliasing.SetActiveView(TaxonomyAliasingEnabled);
            else
                TaxonomyAliasing.SetActiveView(TaxonomyAliasingNoAliases);
            if (folderAliases.Count > 0)
                FolderAliasing.SetActiveView(FolderAliasingEnabled);
            else
                FolderAliasing.SetActiveView(FolderAliasingNoAliases);
            folderAliasRepeater.DataSource = folderAliases;
            folderAliasRepeater.DataBind();
            taxonomyAliasRepeater.DataSource = taxonomyAliases;
            taxonomyAliasRepeater.DataBind();
        }
        else
        {
            FolderAliasing.SetActiveView(FolderAliasingDisabled);
            TaxonomyAliasing.SetActiveView(TaxonomyAliasingDisabled);
        }

        if (!IsPostBack)
        {
            Ektron.Cms.UrlAliasing.UrlAliasSettingsApi urlsettings = new Ektron.Cms.UrlAliasing.UrlAliasSettingsApi();
            string[] allowedextensions = urlsettings.PageExtensions.Split(',');
            ExtensionDropdown.Items.Clear();
            ListItem li = new ListItem();

            foreach (string ext in allowedextensions)
            {
                li = new ListItem();
                li.Enabled = true;
                li.Selected = false;
                li.Text = ext;
                li.Value = ext;
                ExtensionDropdown.Items.Add(li);
            }
        }
    }

    /// <summary>
    /// Initialize the metadata and taxonomy input controls on the page.
    /// </summary>
    /// <param name="folderID">ID of the destination folder</param>
    private void InitializeMetaTaxControls()
    {
        if (Request.QueryString["folderid"] != "" && Request.QueryString["folderid"] != null)
        {
            folderId = Convert.ToInt64(Request.QueryString["folderid"]);
            metadata.FolderID = folderId;
            selectTaxonomy.FolderID = folderId;
        }
        if (Request.QueryString["taxonomyid"] != null && Request.QueryString["taxonomyid"] != "")
        {
            if (Int64.TryParse(Request.QueryString["taxonomyid"], out defaulttaxid))
            {
                selectTaxonomy.defaultTaxID = defaulttaxid;
            }
        }
    }

    private void InitializeEditor()
    {
        if (!Page.IsPostBack)
        {
            Summary.Visible = true;
			Summary.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
        }
    }

    private void PopulateFullAlias(string alias, string quicklink)
    {
        if (folderData == null)
        {
            folderData = contentAPI.GetFolderById(folderId);
        }
        string redirect = "";

        redirect = (alias != "" && !folderData.IsDomainFolder) ? alias : quicklink;
        redirect = contentAPI.SitePath + redirect;
        redirect += (redirect.Contains("?")) ? "&ektronPageBuilderEdit=true" : "?ektronPageBuilderEdit=true";

        fullAlias.Value = redirect;
    }

    private void PopulateTemplateList()
    {
        WireframeModel model = new WireframeModel();
        WireframeData[] wireframes = model.FindByFolderID(folderId);
        long defaultTemplateId = folderData.TemplateId;

        StringBuilder result = new StringBuilder();

        result.AppendLine(@"<ul class=""ektronPageBuilderWizardsTemplateList ektronTemplateList"">");
        string thumb = string.Empty;
        foreach (WireframeData wireframe in wireframes)
        {
            thumb = contentAPI.AppPath + "pagebuilder/wizards/images/imageUnavailable.gif";
            if (wireframe.Template.Thumbnail.ToString() != String.Empty)
            {
                thumb = lib_settings_data.ImageDirectory + "wireframesthumbnails/" + wireframe.Template.Thumbnail.ToString();
            }
            // TODO:  Add If/Else statement to use wireframe.thumbnail property for image path if available.
            result.Append(@"    <li title=""");
            result.Append(wireframe.Path + "\"");
            result.Append(" class=\"ektronTemplate clearfix");
	        if (wireframe.Template.Id == defaultTemplateId)
	        {
	    	    result.Append(" defaultTemplate");
                ektronSelectedTemplate.Value = wireframe.ID.ToString();
	        }
            result.Append("\" data-ektron-id=\"" + wireframe.ID);
            result.Append("\">" + Environment.NewLine);
            result.AppendLine(@"        <div class=""templateInnerWrapper"">");
            result.AppendLine(@"            <a onclick=""return false;"" href=""#"">");
            result.AppendLine(@"                <span style=""background-image: url('" + thumb + @"');"" title=""Template Name"" class=""ektronTemplateThumb""></span>");
            result.AppendLine(@"                <strong>" + wireframe.Path + "</strong>");
            result.AppendLine(@"                <span class=""checked""></span>");
            result.AppendLine(@"            </a>");
            result.AppendLine(@"        </div>");
            result.AppendLine(@"    </li>");
        }
        result.AppendLine(@"</ul>");
        templates.Text = result.ToString();
    }

    public string getchildfolders(long folderid)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Ektron.Cms.ContentAPI capi = new Ektron.Cms.ContentAPI();
        Ektron.Cms.FolderData[] folders = capi.GetChildFolders(folderid, true, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
        Ektron.Cms.PageBuilder.WireframeModel wfm = new WireframeModel();
        if (folders != null && folders.Length > 0)
        {
            foreach (FolderData folder in folders)
            {
                Ektron.Cms.PageBuilder.WireframeData[] wireframes = wfm.FindByFolderID(folder.Id);
                sb.Append("<li class=\"");
                if (folder.HasChildren) sb.Append("ui-finder-folder");
                if (wireframes.Length > 0) sb.Append(" hasWireframe");
                sb.Append("\"><a href=\"");
                sb.Append(capi.AppPath + "/PageBuilder/Wizards/folderbrowser/folderbrowserCB.ashx?folderid=");
                sb.Append(folder.Id);
                sb.Append("\">");
                sb.Append(folder.Name);
                sb.Append("</a></li>");
            }
        }
        return sb.ToString();
    }


    #endregion
    protected void btnFinish_Click(object sender, EventArgs e)
    {
        string title = (pageBuilderWizardPageTitle.Text).Trim();
        string aliasName = String.Empty;
        string extension = String.Empty;
        long pageId = 0;
        int languageId = 1033;
        long folderId = -1;
        long wireFrameId = 0;
        // prep some variables for later use
        PageModel pageModel = new PageModel();
        PageData pageInfo = new PageData();
        WireframeModel model = new WireframeModel();
        WireframeData wireframeInfo = new WireframeData();

        if (ManualAliasing.GetActiveView() == ManualAliasingEnabled && pageBuilderCreateManualAlias.Checked)
        {
            // if url aliasing is enabled, pre-pop using urlAlias field
            aliasName = (pageBuilderWizardAlias.Text).Trim();
            extension = ExtensionDropdown.SelectedValue;
            if (extension != "none")
            {
                if (!aliasName.EndsWith(extension, true, null))
                {
                    aliasName += extension;
                }
            }
        }

        if (Request.QueryString["mode"] != "" && Request.QueryString["mode"] != null)
        {
            mode = Request.QueryString["mode"];
        }

        if (Request.QueryString["pageid"] != "" && Request.QueryString["pageid"] != null)
        {
            pageId = Convert.ToInt64(Request.QueryString["pageid"]);
        }

        if (Request.Form["ektronSelectedTemplate"] != "" && Request.Form["ektronSelectedTemplate"] != null)
        {
            wireFrameId = Convert.ToInt64(Request.Form["ektronSelectedTemplate"]);
        }
        
        string summary = Summary.Content;

        if (Request.QueryString["folderid"] != "" && Request.QueryString["folderid"] != null)
        {
            folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        }

        if (Request.QueryString["language"] != "" && Request.QueryString["language"] != null)
        {
            languageId = Convert.ToInt32(Request.QueryString["language"]);
        }
        if (languageId == -1)
        {
            languageId = contentAPI.RequestInformationRef.ContentLanguage;
        }

        System.Collections.Hashtable meta = metadata.Metadata;

        List<long> selTaxonomy = selectTaxonomy.SelectedTaxonomies;

        string metaXML = "";
        string Quicklink = "";

        //<metadata><meta id="3">Title</meta></metadata>
        foreach (object key in meta.Keys)
        {
            metaXML += @"<meta id=""" + ((object[])meta[key])[0] + @""">" + EkFunctions.HtmlEncode(((object[])meta[key])[2].ToString()) + "</meta>";
        }
        metaXML = "<metadata>" + metaXML + "</metadata>";

        //  create or copy as needed
        if (mode == "add")
        {
            // no pageId was passed, so we're in add mode
            // create the wireframe
            pageModel.Create(title, folderId, aliasName, languageId, wireFrameId, metaXML, summary, out pageInfo);
        }
        else
        {
            // we're in copy mode
            // let's get the wireframeId based on the current template
            pageModel.Get(pageId, out pageInfo, false);
            wireframeInfo = model.FindByPageID(pageInfo.pageID);
            wireFrameId = wireframeInfo.ID;
            pageInfo.title = title;
            // now we'll make the copy
            pageModel.Copy(pageInfo, folderId, aliasName, languageId, wireFrameId, metaXML, summary, out  pageInfo);
        }

        if (selTaxonomy.Count > 0)
        {
            TaxonomyContentRequest tcr = new TaxonomyContentRequest();
            tcr.ContentId = pageInfo.pageID;
            tcr.TaxonomyList = String.Join(",", selTaxonomy.ConvertAll<string>(delegate(long l) { return l.ToString(); }).ToArray());
            contentAPI.AddTaxonomyItem(tcr);
        }

        // we need to get the quicklink for the redirect just in case
        wireframeInfo = model.FindByID(wireFrameId);
        if (wireframeInfo.Template.MasterLayoutID > 0)
        {
            wireframeInfo = model.FindByPageID(wireframeInfo.Template.MasterLayoutID);
        }
        Quicklink = wireframeInfo.Path + (wireframeInfo.Path.IndexOf("?") > 0 ? "&Pageid=" : "?Pageid=") + pageInfo.pageID;

        if (aliasName.Length > 0 && aliasName.IndexOf("LangType=") == -1)
        {
            if (aliasName.IndexOf("?") > 0)
            {
                aliasName += "&LangType=" + languageId.ToString();
            }
            else
            {
                aliasName += "?LangType=" + languageId.ToString();
            }
        }
        if (Quicklink.IndexOf("LangType=") == -1)
        {
            if (Quicklink.IndexOf("?") > 0)
            {
                Quicklink += "&LangType=" + languageId.ToString();
            }
            else
            {
                Quicklink += "?LangType=" + languageId.ToString();
            }
        }
        ektronWizardStepWrapper.Visible = false;
        redirectMessage.Visible = true;

        // Populate the fullAlias hidden field
        PopulateFullAlias(aliasName, Quicklink);

        // build script to modify the modal and display the correct buttons, etc.
        StringBuilder scriptString = new StringBuilder();
        scriptString.AppendLine("parent.$ektron('.ektronPageBuilderAddPage').hide();");
        scriptString.AppendLine("Ektron.ready(function(){");
        scriptString.AppendLine("  parent.Ektron.PageBuilder.Wizards.Buttons.showPromptButtons();");
        scriptString.AppendLine("  parent.$ektron('.ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe').height('6em');");
        scriptString.AppendLine("  parent.$ektron('.ektronPageBuilderAddPage').fadeIn('slow');");
        scriptString.AppendLine(" });");
        // insert the script into the page
        ClientScript.RegisterClientScriptBlock(this.GetType(), "EktronPageBuilderRedirectPrompt", scriptString.ToString(), true);
    }
}
