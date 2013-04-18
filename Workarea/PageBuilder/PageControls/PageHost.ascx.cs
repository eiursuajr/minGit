using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Localization;
using Ektron.Cms.PageBuilder;
using Ektron.Cms.Workarea.Dms;

public partial class PageHost : System.Web.UI.UserControl
{
	private CommonApi _commonApi = new CommonApi();
    string appPath; //path to workarea
    ContentAPI _Capi;
    ContentAPI Capi
    {
        get
        {
            if (_Capi == null) _Capi = new ContentAPI();
            return _Capi;
        }
    }

    List<dmsMenuItem> _DmsOptions;
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg
    {
        get
        {
            return Capi.EkMsgRef;
        }
    }

    public Double CacheInterval
    {
        get { return (Page as PageBuilder).CacheInterval; }
        set { (Page as PageBuilder).CacheInterval = value; }
    }

    private long _folder = -1;
    public long FolderID
    {
        get
        {
            if (PageID > 0)
            {
                // always verify folder ID unless user doesn't give us a page ID
                // because user can give us a bogus folder ID
                long pagefolderid = Capi.GetFolderIdForContentId(PageID);
                if (_folder != pagefolderid)
                {
                    _folder = pagefolderid;
                }
            }
            return _folder;
        }
        set { _folder = value; }
    }

    private long _defaulttaxid = -1;
    public long SelTaxonomyID
    {
        get { return _defaulttaxid; }
        set { _defaulttaxid = value; }
    }

    public long DefaultPageID
    {
        get { return (Page as PageBuilder).DefaultPageID; }
        set { (Page as PageBuilder).DefaultPageID = value; }
    }

    public long PageID
    {
        get { return (Page as PageBuilder).Pagedata.pageID;; }
    }

    public int LangID
    {
        get { return Capi.ContentLanguage; }
    }

    private string _pagePath = "";
    public string PagePath
    {
        get
        {
            if (_pagePath == "")
            {
                _pagePath = Capi.GetPathByFolderID(FolderID);
            }
            return _pagePath;
        }
    }

    public string ThemeName
    {
        get { return (Page as PageBuilder).Theme; }
        set { (Page as PageBuilder).Theme = value; }
    }

    #region Init, Load

    protected void Page_Init(object sender, EventArgs e)
    {
        if (ScriptManager.GetCurrent(Page) == null)
        {
            ScriptManager sMgr = new ScriptManager();
            this.Controls.AddAt(0, sMgr);
        }

        appPath = Capi.ApplicationPath.TrimEnd(new char[] { '/' });

        Packages.EktronCoreJS.Register(this);
        JavaScript.Register(this, appPath + "/java/webkitFix.js");

        /*repWidgetMenus.ItemDataBound += new RepeaterItemEventHandler(repWidgetMenus_ItemDataBound);
        repWidgetMenus.DataSource = (Page as PageBuilder).WidgetMenus;
        repWidgetMenus.DataBind();*/
        this.EnableViewState = false;

        (Page as PageBuilder).WidgetMenuRepeater = repWidgetMenus;

        lbLogout.OnClientClick = "window.open('"+ appPath +"/login.aspx?action=logout', ";
        lbLogout.OnClientClick += "'Login', 'toolbar=0,location=0,scrollbars=0,width=250,height=190');return false;";

        Ektron.Cms.API.AnalyticsTrackingCode.Register(Page);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["thumbnail"] != null && Request["thumbnail"] == "true")
        {
            Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/PageBuilder/PageControls/" + ThemeName + "CSS/PageHost.css");
            Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/PageBuilder/PageControls/" + ThemeName + "CSS/Thumbnail.css");
            EktronPersonalizationWrapper.Visible = false;
        }
        else
        {
            Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/PageBuilder/PageControls/" + ThemeName + "CSS/PageHost.css");
            Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/PageBuilder/PageControls/" + ThemeName + "CSS/PageHostIE.css", BrowserTarget.AllIE);
            Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/PageBuilder/PageControls/" + ThemeName + "CSS/PageHost.ie.7.css", BrowserTarget.LessThanEqualToIE7);
            dontAutoCloseMenu.Visible = false;
            PageBuilder p = (Page as PageBuilder);
            if (p.Status != Mode.AnonViewing)
            {
                setLabels();
                isActionAllowed(dmsMenuItemMenuItemType.DmsMenuView);
            }
            if (p.Status != Mode.AnonViewing && _DmsOptions.Count == 0)
            {
                //user do not have permissiosn
                //if they are an author and there is no layout then they still should see the menu though
                if (Capi.RequestInformationRef.IsMembershipUser == 1 || PageID > -1)
                {
                    p.Status = Mode.AnonViewing;
                }
            }
            if (p.Status != Mode.AnonViewing)
            {
                if (!IsPostBack && Request.RawUrl.Contains("ektronPageBuilderEdit=true") && Capi.IsLoggedIn) //we do it this way because aliasing apparently eats querystrings
                {
                    //if I have edit permissions then edit.
                    if (isActionAllowed(dmsMenuItemMenuItemType.DmsMenuEditLayout))
                    {
                        lbEdit_Click(null, null);
                        dontAutoCloseMenu.Visible = true;
                    }
                    else
                    {
                        SwitchMode(Mode.AuthorViewing, layoutVersion.Staged);
                        dontAutoCloseMenu.Visible = true;
                    }
                    //now redirect to the page without the querystring
                    string newUrl = Request.RawUrl.Replace('?', '&').Replace("&ektronPageBuilderEdit=true", "");
                    int first = newUrl.IndexOf('&');
                    if (first > -1) newUrl = newUrl.Remove(first, 1).Insert(first, "?");
                    //now redirect to it
                    Response.Redirect(newUrl);
                }
                else if (!IsPostBack && Request.RawUrl.Contains("view=staged") && Capi.IsLoggedIn && p.Status != Mode.AnonViewing)
                {
                    SwitchMode(Mode.AuthorViewing, layoutVersion.Staged);
                    string newUrl = Request.RawUrl.Replace('?', '&').Replace("&view=staged", "");
                    int first = newUrl.IndexOf('&');
                    if(first > -1) newUrl = newUrl.Remove(first, 1).Insert(first, "?");
                    //now redirect to it
                    Response.Redirect(newUrl);
                }
                else if (!IsPostBack && Request.RawUrl.Contains("view=published") && Capi.IsLoggedIn && p.Status != Mode.AnonViewing)
                {
                    SwitchMode(Mode.AuthorViewing, layoutVersion.Published);
                    string newUrl = Request.RawUrl.Replace('?', '&').Replace("&view=published", "");
                    int first = newUrl.IndexOf('&');
                    if (first > -1) newUrl = newUrl.Remove(first, 1).Insert(first, "?");
                    //now redirect to it
                    Response.Redirect(newUrl);
                }
            }
            setButtons();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        displayProperties();
        if ((Page as PageBuilder).Status == Mode.AuthorViewing)
        {
            makeCopyandNew();
        }

        if ((Page as PageBuilder).Status == Mode.Editing)
        {
            //keep alive
            sessionKeepalive.Text = sessionKeepalive.Text.Replace("<pagepostback>", appPath + "/PageBuilder/SessionKeepAlive.aspx");
            sessionKeepalive.Text = sessionKeepalive.Text.Replace("<millis>", ((Session.Timeout * .75) * 60 * 1000).ToString());
        }
        if ((Page as PageBuilder).Status != Mode.AnonViewing)
        {
            isMasterLayout.Visible = true;
            dontAutoCloseMenu.Visible = true;
            isMasterLayout.Text = isMasterLayout.Text.Replace("<isMasterLayout>", (Page as PageBuilder).Pagedata.IsMasterLayout.ToString().ToLower());
            isMasterLayout.Text = isMasterLayout.Text.Replace("<pathToLock>", appPath + "/PageBuilder/PageControls/Themes/TrueBlue/images/lock.png");
            isMasterLayout.Text = isMasterLayout.Text.Replace("<hasMasterLayout>", ((Page as PageBuilder).Pagedata.masterID > 0).ToString().ToLower());
        }
    }

    #endregion

    protected void SwitchMode(Mode newmode, layoutVersion newversion)
    {
        _DmsOptions = null;
        Mode oldmode = (Page as PageBuilder).Status;
        layoutVersion oldversion = (Page as PageBuilder).viewType;
        (Page as PageBuilder).Status = newmode;
        (Page as PageBuilder).viewType = newversion;

        switch (newmode)
        {
            case Mode.AnonViewing:
                Session["EkWidgetDirty"] = null;
                Session["EkWidgetBag"] = null;
                (Page as PageBuilder).Pagedata = null;
                (Page as PageBuilder).ClearView(newversion, true);
                break;
            case Mode.AuthorViewing:
                if (newversion == layoutVersion.Published)
                {
                    lbViewPublishedCheckedIn.Text = m_refMsg.GetMessage("lbl pagebuilder view staged");
                    lbViewPublishedCheckedIn.ToolTip = lbViewPublishedCheckedIn.Text;
                }
                else
                {
                    lbViewPublishedCheckedIn.Text = m_refMsg.GetMessage("lbl pagebuilder view published");
                    lbViewPublishedCheckedIn.ToolTip = lbViewPublishedCheckedIn.Text;
                }

                //clear session
                Session["EkWidgetDirty"] = null;
                Session["EkWidgetBag"] = null;
                (Page as PageBuilder).Pagedata = null;
                (Page as PageBuilder).ClearView(newversion, true);
                break;
            case Mode.Editing:
                if (oldmode != Mode.Preview || Session["EkWidgetDirty"] == null || Session["EkWidgetBag"] == null)
                {//if not the load from session
                    (Page as PageBuilder).ClearView(newversion, true);
                }
                else
                {
                    (Page as PageBuilder).View((Page as PageBuilder).Pagedata);
                }
                break;
            case Mode.Preview:
                //load from session
                (Page as PageBuilder).View((Page as PageBuilder).Pagedata);
                break;
        }

        setButtons();
    }

    protected void makeCopyandNew()
    {
        if (FolderID > -1)
        {
            if ((Page as PageBuilder).Pagedata.IsMasterLayout)
            {
                if (Capi.IsAdmin() || Capi.IsARoleMember(EkEnumeration.CmsRoleIds.CreateMasterLayout) && Capi.EkContentRef.IsAllowed(FolderID, LangID, "folder", "add", Capi.RequestInformationRef.UserId))
                {
                    JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIDraggableJS);
                    JS.RegisterJS(this, appPath + "/PageBuilder/Wizards/js/ektron.pagebuilder.wizards.js", "EktronPageBuilderWizardsJS");
                    JS.RegisterJS(this, appPath + "/PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS");

                    Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
                    Ektron.Cms.API.Css.RegisterCss(this, appPath + "/PageBuilder/Wizards/css/ektron.pagebuilder.wizards.css", "EktronPageBuilderWizardsCSS");
                    lbNew.OnClientClick = "return Ektron.PageBuilder.Wizards.showAddMasterPage({mode: 'add', folderId: " + FolderID.ToString() + ", language: " + LangID.ToString() + ", defaulttaxid: " + SelTaxonomyID.ToString() + "})";
                    enableButton(lbNew, true);
                    enableButton(lbCopy, false);
                }
                else
                {
                    enableButton(lbNew, false);
                    enableButton(lbCopy, false);
                }
            }
            else
            {
                if (Capi.EkContentRef.IsAllowed(FolderID, LangID, "folder", "add", Capi.RequestInformationRef.UserId))
                {
                    JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIDraggableJS);
                    JS.RegisterJS(this, appPath + "/PageBuilder/Wizards/js/ektron.pagebuilder.wizards.js", "EktronPageBuilderWizardsJS");
                    JS.RegisterJS(this, appPath + "/PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS");

                    Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
                    Ektron.Cms.API.Css.RegisterCss(this, appPath + "/PageBuilder/Wizards/css/ektron.pagebuilder.wizards.css", "EktronPageBuilderWizardsCSS");
                    lbNew.OnClientClick = "return Ektron.PageBuilder.Wizards.showAddPage({mode: 'add', folderId: " + FolderID.ToString() + ", language: " + LangID.ToString() + ", defaulttaxid: " + SelTaxonomyID.ToString() + "})";
                    enableButton(lbNew, true);

                    if (PageID > -1)
                    {
                        lbCopy.OnClientClick = "return Ektron.PageBuilder.Wizards.showAddPage({mode: 'saveAs', folderId: " + FolderID.ToString() + ", language: " + LangID.ToString() + ", pageid: " + PageID.ToString() + ", defaulttaxid: " + SelTaxonomyID.ToString() + " })";
                        enableButton(lbCopy, true);
                    }
                }
            }
        }
    }

    protected string getActionString(dmsMenuItemMenuItemType action)
    {
        try
        {
            if (_DmsOptions == null)
            {
                _DmsOptions = new List<dmsMenuItem>();

                if (PageID > -1)
                {
                    long UserID = Capi.UserId;
                    DmsMenu mnu = new DmsMenu(ektronDmsMenuMenuType.Workarea, PageID, UserID, LangID, FolderID, EkConstants.CMSContentType_Content, false);
                    dmsMenuItem[] items = mnu.GetDmsMenuData().dmsMenu.dmsContentItem.dmsMenuItem;
                    if (items != null)
                    {
                        _DmsOptions.AddRange(items);
                    }
                }
            }

            dmsMenuItem myitem = _DmsOptions.Find(delegate(dmsMenuItem i) { return i.menuItemType == action; });
            if (myitem == null) return "";
            return myitem.label;
        }
        catch (Exception ex)
        {
            string error = ex.ToString();
            return "";
        }
    }

    protected bool isActionAllowed(dmsMenuItemMenuItemType action)
    {
        try
        {
            if (_DmsOptions == null)
            {
                _DmsOptions = new List<dmsMenuItem>();

                if (PageID > -1)
                {
                    long UserID = new ContentAPI().UserId;
                    DmsMenu mnu = new DmsMenu(ektronDmsMenuMenuType.Workarea, PageID, UserID, LangID, FolderID, EkConstants.CMSContentType_Content, false);
                    dmsMenuItem[] items = mnu.GetDmsMenuData().dmsMenu.dmsContentItem.dmsMenuItem;
                    if (items != null)
                    {
                        _DmsOptions.AddRange(items);
                    }
                }
            }
            
            dmsMenuItem myitem = _DmsOptions.Find(delegate(dmsMenuItem i) { return i.menuItemType == action; });
            if (myitem == null) return false;
            return true;
        }
        catch (Exception ex)
        {
            string error = ex.ToString();
            return false;
        }
    }

    protected void enableButton(LinkButton lb, bool enabled) {enableButton(lb as WebControl, enabled);}
    protected void enableButton(HyperLink lb, bool enabled) {enableButton(lb as WebControl, enabled);}
    protected void enableButton(WebControl lb, bool enabled)
    {
        lb.CssClass = (enabled) ? "valid" : "invalid";
        lb.Enabled = enabled;
        string actionName = lb.ID.Replace("lb", "");
        HtmlImage img = (HtmlImage)this.FindControl("img" + actionName);
    }

    protected void setLabels()
    {
        // assign translated text values
        lblFile.Text = m_refMsg.GetMessage("lbl generic file");
        lbNew.Text = m_refMsg.GetMessage("lbl pagebuilder new page");
        lbNew.ToolTip = lbNew.Text;
        lbCopy.Text = m_refMsg.GetMessage("lbl pagebuilder copy save as");
        lbCopy.ToolTip = lbCopy.Text;
        lbEdit.Text = m_refMsg.GetMessage("generic edit title");
        lbEdit.ToolTip = lbEdit.Text;
        lbSave.Text = m_refMsg.GetMessage("generic save");
        lbSave.ToolTip = lbSave.Text;
        lbCheckin.Text = m_refMsg.GetMessage("btn checkin");
        lbCheckin.ToolTip = lbCheckin.Text;
        lbPublish.Text = m_refMsg.GetMessage("generic publish");
        //Defect: 53143: Sometimes it just throws 404 error
        lbPublish.PostBackUrl = Request.RawUrl;
        lbCheckin.PostBackUrl = Request.RawUrl;
        lbSave.PostBackUrl = Request.RawUrl;

        lbPublish.ToolTip = lbPublish.Text;
        lbCancel.Text = m_refMsg.GetMessage("generic cancel");
        lbCancel.ToolTip = lbCancel.Text;
        lbProperties.Text = m_refMsg.GetMessage("generic properties");
        lbProperties.ToolTip = lbProperties.Text;
        lbPreview.Text = m_refMsg.GetMessage("lbl pagebuilder preview layout");
        lbPreview.ToolTip = lbPreview.Text;
        lbLogout.Text = m_refMsg.GetMessage("generic logout msg");
        lbLogout.ToolTip = lbLogout.Text;
        lblFilterControlList.Text = m_refMsg.GetMessage("lbl pagebuilder filter control list");
        lblFilterControlList.ToolTip = lblFilterControlList.Text;
        lbViewPublishedCheckedIn.Text = m_refMsg.GetMessage("lbl pagebuilder view published");
        lbViewPublishedCheckedIn.ToolTip = lbViewPublishedCheckedIn.Text;
        topMenuInput.Value = "";

        //set up img paths
        MenuImg.Src = appPath + "/PageBuilder/PageControls/" + ThemeName + "images/menuhandled_putback_hover.png";
        MenuTack.Src = appPath + "/PageBuilder/PageControls/" + ThemeName + "images/thumbtack_out.png";
        imgdashbottom.Src = appPath + "/PageBuilder/PageControls/" + ThemeName + "images/dashboardbottom.png";
        imgpullchain.Src = appPath + "/PageBuilder/PageControls/" + ThemeName + "images/pullchain.png";

        // populate workarea link
        lbWorkarea.Text = m_refMsg.GetMessage("workarea page title");
        lbWorkarea.ToolTip = lbWorkarea.Text;
        lbWorkarea.Attributes.Add("onclick", String.Format("window.open('{0}/workarea.aspx?LangType={1}', 'Admin400', 'width=900,height=580,scrollable=1,resizable=1');return false;", this.appPath.ToString(), LangID));
        enableButton(lbWorkarea, true);

        // populate analytics link
        lbAnalytics.Text = m_refMsg.GetMessage("lbl entry analytics");
        lbAnalytics.ToolTip = lbAnalytics.Text;
        lbAnalytics.Attributes.Add("onclick", String.Format("window.open('{0}/analytics/seo.aspx?tab=traffic&uri={1}', 'Analytics400', 'width=900,height=580,scrollable=1,resizable=1');return false;", appPath.ToString(), Request.RawUrl));
        enableButton(lbAnalytics, true);

        lbLaunchHelp.Text = m_refMsg.GetMessage("lbl launch pagebuilder help");
        lbLaunchHelp.ToolTip = lbLaunchHelp.Text;
        lbLaunchHelp.Attributes.Add("onclick", String.Format("window.open('{0}', 'SitePreview', 'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=600,height=500');return false;", _commonApi.fetchhelpLink("pagebuilder_overview")));
        enableButton(lbCreatingLayouts, true);
        enableButton(lbLayoutManagement, true);
        enableButton(lbLaunchHelp, true);
    }

    protected void setButtons()
    {
        PermissionData myPermissionData = new PermissionData();
        lbPreview.Text = m_refMsg.GetMessage("lbl pagebuilder preview layout");
        lbPreview.ToolTip = lbPreview.Text;
        lbNew.OnClientClick = "return false;";
        lbCopy.OnClientClick = "return false;";

        enableButton(lbNew, false);
        enableButton(lbCopy, false);
        enableButton(lbCancel, false);
        enableButton(lbPreview, false);
        enableButton(lbSave, false);
        enableButton(lbWorkarea, false);
        enableButton(lbAnalytics, false);
        enableButton(lbViewPublishedCheckedIn, false);

        dontAutoCloseMenu.Visible = true;
        tray.Visible = false;
        pnlSearchWidget.Visible = false;
        EktronPersonalizationWrapper.Visible = true;

        layoutVersion version = (Page as PageBuilder).viewType;

        if ((Page as PageBuilder).Status != Mode.AnonViewing)
        {
            if (!Capi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0) || Capi.RequestInformationRef.UserId == EkConstants.BuiltIn)
            {
                EktronPersonalizationWrapper.Visible = false;
                return;
            }

            //fix for when another user forces checkin and starts editing. it kicks us out here.
            if ((isActionAllowed(dmsMenuItemMenuItemType.DmsMenuForceCheckIn) || isActionAllowed(dmsMenuItemMenuItemType.DmsMenuRequestCheckIn)) && (Page as PageBuilder).Status == Mode.Editing)
            {
                SwitchMode(Mode.AuthorViewing, (Page as PageBuilder).viewType);
                return;
            }
            
            myPermissionData = Capi.LoadPermissions(PageID, "content", ContentAPI.PermissionResultType.All);
            if (!myPermissionData.CanPublish)
            {
                if (myPermissionData.CanApprove)
                {
                    lbPublish.Text = m_refMsg.GetMessage("generic approve title");
                    lbPublish.ToolTip = lbPublish.Text;
                }
                else
                {
                    lbPublish.Text = m_refMsg.GetMessage("generic submit");
                    lbPublish.ToolTip = lbPublish.Text;
                }
            }
            lbPublish.ToolTip = lbPublish.Text;

            RegisterFiles();

            string force = getActionString(dmsMenuItemMenuItemType.DmsMenuForceCheckIn);
            if (force != "")
            {
                lbCheckin.Text = force;
                lbCheckin.ToolTip = force;
            }
            string noforce = getActionString(dmsMenuItemMenuItemType.DmsMenuCheckIn);
            if (noforce != "")
            {
                lbCheckin.Text = noforce;
                lbCheckin.ToolTip = noforce;
            }

            enableButton(lbEdit, UserCanEditPage());
            if (isActionAllowed(dmsMenuItemMenuItemType.DmsMenuViewProperties))
            {
                enableButton(lbProperties, true);
                lbProperties.NavigateUrl = Capi.AppPath + "workarea.aspx?page=content.aspx&folder_id=" + FolderID.ToString();
                lbProperties.NavigateUrl += "&ContentNav=" + PagePath;
                lbProperties.NavigateUrl += "&TreeVisible=Content&action=view&id=" + PageID.ToString();
                lbProperties.NavigateUrl += "&LangType=" + LangID.ToString();
                lbProperties.NavigateUrl += "&menuItemType=viewproperties";
                string onclick = "window.open(this.href, 'Admin400', 'location=0,status=1,scrollbars=1, width:900, height:580');return false;";
                lbProperties.Attributes.Add("onclick", onclick);
            }
            else
            {
                enableButton(lbProperties, false);
            }
            enableButton(lbCheckin, isActionAllowed(dmsMenuItemMenuItemType.DmsMenuCheckIn) || isActionAllowed(dmsMenuItemMenuItemType.DmsMenuForceCheckIn));
            enableButton(lbPublish, (isActionAllowed(dmsMenuItemMenuItemType.DmsMenuPublish) || isActionAllowed(dmsMenuItemMenuItemType.DmsMenuCheckIn) || isActionAllowed(dmsMenuItemMenuItemType.DmsMenuSubmit) || isActionAllowed(dmsMenuItemMenuItemType.DmsMenuApprove))  && version == layoutVersion.Staged);
        }

        switch ((Page as PageBuilder).Status)
        {
            case Mode.AnonViewing:
                EktronPersonalizationWrapper.Visible = false;
                break;
            case Mode.AuthorViewing:
                bool showSwitchView = false;
                if (PageID > -1)
                {
                    ContentHistoryData[] history = null;
                    //get permissions for history
                    if (myPermissionData.CanHistory)
                    {
                        history = Capi.GetHistoryList(PageID);
                    }
                    bool foundcheckedin = false;
                    bool foundpublished = false;
                    if (history != null)
                    {
                        for (int i = 0; i < history.Length; i++)
                        {
                            if (history[i].Status == "I")
                            {
                                foundcheckedin = true;
                            }
                            if (history[i].Status == "A")
                            {
                                foundpublished = true;
                                break;
                            }
                        }
                    }
                    showSwitchView = foundcheckedin && foundpublished;

                    if (!showSwitchView)
                    {
                        ContentStateData state = Capi.GetContentState(PageID);
                        DateTime golive = new DateTime();
                        if(state.CurrentGoLive == "" || !DateTime.TryParse(state.CurrentGoLive, out golive)){
                            golive = DateTime.MinValue;
                        }
                        if (golive > DateTime.Now)
                            showSwitchView = true;
                    }
                }

                dontAutoCloseMenu.Visible = false;
                enableButton(lbWorkarea, true);
                enableButton(lbAnalytics, true);
                enableButton(lbViewPublishedCheckedIn, showSwitchView);
                break;
            case Mode.Preview:
                enableButton(lbEdit, false);
                enableButton(lbCancel, true);
                enableButton(lbPreview, true);
                enableButton(lbWorkarea, true);
                enableButton(lbAnalytics, true);
                lbPreview.Text = m_refMsg.GetMessage("lbl pagebuilder return to edit");
                lbPreview.ToolTip = lbPreview.Text;
                dontAutoCloseMenu.Visible = false;
                break;
            case Mode.Editing:
                if (!isActionAllowed(dmsMenuItemMenuItemType.DmsMenuForceCheckIn))
                {
                    (Page as PageBuilder).viewType = layoutVersion.Staged;
                    enableButton(lbEdit, false);
                    enableButton(lbCancel, true);
                    enableButton(lbPreview, true);
                    enableButton(lbSave, true);
                    enableButton(lbWorkarea, true);
                    enableButton(lbAnalytics, true);

                    tray.Visible = true;
                    pnlSearchWidget.Visible = true;
                    sessionKeepalive.Visible = true;
                    //register CSS
                    JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIWidgetJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUISortableJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIEffectsJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIEffectsHighlightJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronBlockUiJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIResizableJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronUIDraggableJS);
                    JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
                }
                break;
        }
    }

    protected void displayProperties()
    {
        if (PageID == -1 || (Page as PageBuilder).Status == Mode.AnonViewing)
        {
            propsmenu.Visible = false;
        }
        else
        {
            propsmenu.Visible = true;
            try
            {
                PageBuilder mypage = (Page as PageBuilder);
                WireframeModel wfm = new WireframeModel();
                ContentBase baseData = mypage.Basedata;
                if (baseData == null) baseData = Capi.EkContentRef.LoadContent(PageID, true, EkEnumeration.CMSContentSubtype.PageBuilderData);

                Ektron.Cms.Framework.Localization.LocaleManager _locApi = new Ektron.Cms.Framework.Localization.LocaleManager();

                LocaleData langData = null;
                langData = _locApi.GetItem(baseData.Language);
                ContentStateData state = null;
                WireframeData wireframe = null;
                if (PageID > -1)
                {
                    state = Capi.GetContentState(PageID);
                    wireframe = wfm.FindByPageID(PageID);
                }

                spnContentPath.InnerText = m_refMsg.GetMessage("generic path");
                spnCurrentEditor.InnerText = m_refMsg.GetMessage("status:checked out by");
                spnDateCreated.InnerText = m_refMsg.GetMessage("generic datecreated");
                spnLanguage.InnerText = m_refMsg.GetMessage("lbl language");
                spnLastEditDate.InnerText = m_refMsg.GetMessage("generic date modified");
                spnLastEditor.InnerText = m_refMsg.GetMessage("generic last editor");
                spnMode.InnerText = m_refMsg.GetMessage("lbl pagebuilder current mode");
                spnPageID.InnerText = m_refMsg.GetMessage("lbl pagebuilder page id");
                spnStatus.InnerText = m_refMsg.GetMessage("generic status");
                spnTitle.InnerText = m_refMsg.GetMessage("generic title");
                spnWireframeFile.InnerText = m_refMsg.GetMessage("lbl pagebuilder wireframe template path");

                trCheckedOut.Visible = false;

                lblDatecreated.Text = baseData.DisplayDateCreated;
                lblLanguage.Text = langData.EnglishName  + " (" + baseData.Language.ToString() + ")";
                lblLasteditdate.Text = baseData.DisplayDateModified;
                lblLasteditor.Text = baseData.LastEditorFname + " " + baseData.LastEditorLname;
                lblPath.Text = PagePath;

                if (wireframe != null)
                {
                    lblWireframe.Text = wireframe.Path;
                }
                lblPageid.Text = PageID.ToString();
                lblTitle.Text = state.Title;

                if (state != null)
                {
                    switch (state.Status)
                    {
                        case "A":
                            lblStatus.Text = m_refMsg.GetMessage("status:approved (published)");
                            break;
                        case "S":
                            lblStatus.Text = m_refMsg.GetMessage("status:submitted for approval");
                            break;
                        case "I":
                            lblStatus.Text = m_refMsg.GetMessage("status:checked in");
                            break;
                        case "M":
                            lblStatus.Text = m_refMsg.GetMessage("status:submitted for deletion");
                            break;
                        case "P":
                            lblStatus.Text = m_refMsg.GetMessage("status:approved (pgld)");
                            break;
                        case "O":
                            trCheckedOut.Visible = true;
                            lblStatus.Text = m_refMsg.GetMessage("status:checked out");
                            if (state != null)
                            {
                                lblUserCheckedOut.Text = state.CurrentEditorFirstName + " " + state.CurrentEditorLastName;
                                lblUserCheckedOut.ToolTip = lblUserCheckedOut.Text;
                            }
                            break;
                        case "T":
                            lblStatus.Text = m_refMsg.GetMessage("lbl pagebuilder pending tasks");
                            break;
                        default:
                            lblStatus.Text = "";
                            break;
                    }
                    if (state.ContType == 3)
                    {
                        lblStatus.Text = m_refMsg.GetMessage("lbl pagebuilder expired");
                    }
                }

                switch (mypage.Status)
                {
                    case Mode.AuthorViewing:
                        if ((Page as PageBuilder).viewType == layoutVersion.Staged)
                            lblMode.Text = m_refMsg.GetMessage("pagebuilder viewing staged");
                        else
                            lblMode.Text = m_refMsg.GetMessage("pagebuilder viewing published");
                        break;
                    case Mode.Editing:
                        lblMode.Text = m_refMsg.GetMessage("pagebuilder editing");
                        break;
                    case Mode.Preview:
                        lblMode.Text = m_refMsg.GetMessage("pagebuilder preview");
                        break;
                    default:
                        lblMode.Text = "";
                        break;
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();
            }
        }
    }

    protected void RegisterFiles()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronCookieJS);
        JS.RegisterJS(this, appPath + "/PageBuilder/PageControls/JS/widgetTrayResources.aspx", "EktronWidgetTrayResourcesJS");
        JS.RegisterJS(this, appPath + "/PageBuilder/PageControls/JS/WidgetTray.js", "EktronWidgetTrayJS");
    }

    protected void lbPublish_Click(object sender, EventArgs e)
    {
        IPageController _controller = PageFactory.GetController(Page as PageBuilder);
        PageData savestate = null;
        if (Session["EkWidgetDirty"] != null && (bool)Session["EkWidgetDirty"] == true && Session["EkWidgetBag"] != null)
        {
            savestate = (PageData)Session["EkWidgetBag"];
        }
        else
        {
            _controller.Get(PageID, out savestate, true);
        }
        if (savestate != null)
        {
            if (savestate.pageID == PageID)
            {
                if (!_controller.Publish(savestate))
                {
                    (Page as PageBuilder).Error("Could not save state");
                }
            }
            else
            {
                (Page as PageBuilder).Error("State Mismatch");
            }
            // check the state of the content and set the mode correctly as appropriate
            ContentStateData state = Capi.GetContentState(PageID);
            DateTime golive = new DateTime();
            if(state.CurrentGoLive == "" || !DateTime.TryParse(state.CurrentGoLive, out golive)){
                golive = DateTime.MinValue;
            }
            if (state.Status == "A" && golive < DateTime.Now)
            {
                SwitchMode(Mode.AuthorViewing, layoutVersion.Published);
            }
            else
            {
                SwitchMode(Mode.AuthorViewing, layoutVersion.Staged);
            }
        }
        Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }

    protected void lbSave_Click(object sender, EventArgs e)
    {
        PageData savestate = null;
        if (Session["EkWidgetDirty"] != null && (bool)Session["EkWidgetDirty"] == true && Session["EkWidgetBag"] != null)
        {
            savestate = (PageData)Session["EkWidgetBag"];
        }
        else
        {
            savestate = (Page as PageBuilder).Pagedata;
        }
        if (savestate != null)
        {
            IPageController _controller = PageFactory.GetController(Page as PageBuilder);
            if (savestate.pageID == PageID)
            {
                if (!_controller.Save(savestate))
                {
                    (Page as PageBuilder).Error("Could not save state");
                }
            }
            else
            {
                (Page as PageBuilder).Error("State Mismatch");
            }
        }
        Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }

    protected void lbCheckin_Click(object sender, EventArgs e)
    {
        PageData savestate = null;
        if (Session["EkWidgetDirty"] != null && (bool)Session["EkWidgetDirty"] == true && Session["EkWidgetBag"] != null)
        {
            savestate = (PageData)Session["EkWidgetBag"];
        }
        else
        {
            savestate = (Page as PageBuilder).Pagedata;
        }
        if (savestate != null)
        {
            IPageController _controller = PageFactory.GetController(Page as PageBuilder);
            if (savestate.pageID == PageID)
            {
                if (!_controller.CheckIn(savestate))
                {
                    (Page as PageBuilder).Error("Could not save state");
                }
            }
            else
            {
                (Page as PageBuilder).Error("State Mismatch");
            }
        }
        SwitchMode(Mode.AuthorViewing, layoutVersion.Staged);
        Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }

    protected bool UserCanEditPage()
    {
        return ((Page as PageBuilder).Pagedata.languageID == Capi.ContentLanguage && isActionAllowed(dmsMenuItemMenuItemType.DmsMenuEditLayout));
    }

    protected void lbEdit_Click(object sender, EventArgs e)
    {
        Ektron.Cms.ContentStateData pageStateData = Capi.GetContentState((Page as PageBuilder).Pagedata.pageID);
        long contentStateUserId = pageStateData.CurrentUserId;
        if (UserCanEditPage())
        {
            IPageController _controller = PageFactory.GetController(Page as PageBuilder);
            _controller.CheckOut((Page as PageBuilder).Pagedata);
            SwitchMode(Mode.Editing, layoutVersion.Staged);

            string newUrl = Request.RawUrl.Replace('?', '&').Replace("&ektronPageBuilderEdit=true", "");
            int first = newUrl.IndexOf('&');
            if (first > -1) newUrl = newUrl.Remove(first, 1).Insert(first, "?");

            Response.Redirect(newUrl); //use post-redirect-get method to avoid viewstate issues
        }
        else
        {
            lblStatus.Text = m_refMsg.GetMessage("status:checked out");
            
        }
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        Session["EkWidgetDirty"] = false;
        Session["EkWidgetBag"] = null;
        IPageController _controller = PageFactory.GetController(Page as PageBuilder);
        _controller.Revert((Page as PageBuilder).Pagedata);
        SwitchMode(Mode.AuthorViewing, layoutVersion.Staged);
        Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }

    protected void lbPreview_Click(object sender, EventArgs e)
    {
        //toggle editing
        if ((Page as PageBuilder).Status == Mode.Editing) 
            SwitchMode(Mode.Preview, (Page as PageBuilder).viewType);
        else
            SwitchMode(Mode.Editing, (Page as PageBuilder).viewType);
        Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }

    protected void lbViewPublishedCheckedIn_Click(object sender, EventArgs e)
    {
        //toggle staged / published
        if ((Page as PageBuilder).viewType == layoutVersion.Published)
            SwitchMode((Page as PageBuilder).Status, layoutVersion.Staged);
        else
            SwitchMode((Page as PageBuilder).Status, layoutVersion.Published);
        //Response.Redirect(Request.RawUrl); //use post-redirect-get method to avoid viewstate issues
    }
}
