using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Content;
using System.Collections.Generic;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;
using System.Collections.Specialized;

[JsonObject(MemberSerialization.OptIn)]
public class JsonRequest
{
    [JsonProperty]
    public string Controller;
    [JsonProperty]
    public string Action;
    [JsonProperty]
    public string[] Arguments;
}

// Action: add_widget
// Arguments: 
//     int widgetTypeId - Widget type unique identifier
//     int columnId     - Column unique identifier
//     int index        - Index after which new widget should be inserted
public struct AddWidgetAction
{
    public AddWidgetAction(string[] args)
    {
        WidgetTypeID = Int64.Parse(args[0]);
        ColumnIndex = int.Parse(args[1]);
        WidgetIndex = int.Parse(args[2]);
    }

    public Int64 WidgetTypeID;
    public int ColumnIndex;
    public int WidgetIndex;
}

// Action: move_widget
// Arguments: 
//     int startColumn - Widget type unique identifier
//     int startIndex  - Column unique identifier
//     int finalColumn - Index after which new widget should be inserted
//     int finalIndex  - Index after which new widget should be inserted
public struct MoveWidgetAction
{
    public MoveWidgetAction(string[] args)
    {
        StartColumn = int.Parse(args[0]);
        StartIndex = int.Parse(args[1]);
        FinalColumn = int.Parse(args[2]);
        FinalIndex = int.Parse(args[3]);
    }

    public int StartColumn;
    public int StartIndex;
    public int FinalColumn;
    public int FinalIndex;
}

public class RedirectQuerystringParams
{
    #region enums

    public enum ActionType
    {
        Add,
        Remove
    }

    #endregion

    #region members

    private string _key;
    private string _value;
    private ActionType _action;

    #endregion

    #region properties

    public string Key
    {
        get
        {
            return _key;
        }
        set
        {
            _key = value;
        }
    }

    public string Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }


    public ActionType Action
    {
        get
        {
            return _action;
        }
        set
        {
            _action = value;
        }
    }

    #endregion

    #region events

    public RedirectQuerystringParams(string key, string value, ActionType action)
    {
        _key = key;
        _value = value;
        _action = action;
    }

    #endregion
}

public partial class WidgetControls_WidgetSpace : System.Web.UI.UserControl, IWidgetSpaceView
{
    #region constants

    const string WidgetListContainerPath = "Views/personalization_widget_list_container.ascx";

    #endregion

    #region members

    private ContentAPI _contentApi;
    protected SiteAPI _siteApi;
    private IWidgetSpaceController _controller;
    private WidgetSpaceData _widgetSpace;
    private WidgetPageData[] _pages;
    protected bool _Editable;
    private bool _isInWorkarea;
    private string _ApplicationPath;
    private string _SitePath;
    private bool _allowAnonymous;
    private long _widgetSpaceID;
    private long _foreignID;
    protected int _activePage;
    private string _dynamicForeignIDParameter;
    private List<RedirectQuerystringParams> _rediectQuerystringParams;
    private string _appPath;

    #endregion

    #region properties

    protected bool EditDefault
    {
        get { return Request.QueryString["editDefault"] != null; }
    }
    public bool AllowAnonymous
    {
        get
        {
            return _allowAnonymous;
        }
        set
        {
            _allowAnonymous = value;
        }
    }
    public long WidgetSpaceID
    {
        get
        {
            return _widgetSpaceID;
        }

        set
        {
            _widgetSpaceID = value;
        }
    }
    public long DefaultForeignID
    {
        get
        {
            return _foreignID;
        }

        set
        {
            _foreignID = value;
        }
    }
    public string DynamicForeignIDParameter
    {
        get
        {
            return _dynamicForeignIDParameter;
        }

        set
        {
            _dynamicForeignIDParameter = value;
        }
    }
    public string WidgetsPath
    {
        get
        {
            if (_isInWorkarea)
            {
                // We are in the Workarea
                return _siteApi.RequestInformationRef.ApplicationPath + "Widgets/";
            }
            return _siteApi.RequestInformationRef.WidgetsPath;
        }
    }
    public WidgetSpaceData WidgetSpace
    {
        get
        {
            return _widgetSpace;
        }

    }

    #endregion

    #region events

    protected EkMessageHelper m_refMsg;
    // protected UserAPI m_refApi = new UserAPI();

    public WidgetControls_WidgetSpace()
    {
        _contentApi = new ContentAPI();
        _siteApi = new SiteAPI();
        _rediectQuerystringParams = new List<RedirectQuerystringParams>();
        _pages = new WidgetPageData[0];
        _dynamicForeignIDParameter = "foreignid";
        _foreignID = -1;
        _activePage = 0;
        _allowAnonymous = false;
        _ApplicationPath = _contentApi.ApplicationPath.TrimEnd(new char[] { '/' });
        _SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
        _appPath = _contentApi.ApplicationPath.TrimEnd(new char[] { '/' });
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _controller = WidgetSpaceFactory.GetController(this);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_contentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Personalization))
        {
            ulTabOptions.Visible = false;
        }
        
        //set widgetTrayHandle onclick attribute
        aWidgetTrayHandle.Attributes.Add("onclick", "Ektron.Personalization.WidgetTray.toggle();return false;");

        //set image paths
        SetImagePaths();

        // Make usercontrols here...
        if (_dynamicForeignIDParameter == "" ||
            Request.QueryString[_dynamicForeignIDParameter] == null ||
            !long.TryParse(Request.QueryString[_dynamicForeignIDParameter], out _foreignID))
        {
            _foreignID = _siteApi.UserId;
        }

        //build control tree to match viewstate
        //int.TryParse(Request.Form[hdnOriginalPage.UniqueID], out _activePage);
        if (Session["pageid"] != null)
            _activePage = (int)Session["pageid"];
        BuildControlTree();

        //register JS/CSS
        RegisterJs();
        RegisterCss();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //int.TryParse(hdnActivePage.Value, out _activePage);
        //hdnActivePage.Value = _activePage.ToString();
        //hdnOriginalPage.Value = _activePage.ToString();

        //re-build control tree to update viewstate
        //BuildControlTree();

        m_refMsg = _contentApi.EkMsgRef;
        liResetWidgets.Attributes.Add("title", m_refMsg.GetMessage("reset widgets"));
        liEditDefaultWidgets.Attributes.Add("title", m_refMsg.GetMessage("edit default widgets"));
        liDone.Attributes.Add("title", m_refMsg.GetMessage("save default"));

        imgWidgetTrayScrollLeft.Alt = m_refMsg.GetMessage("wiget list scroll left");
        imgWidgetTrayScrollRight.Alt = m_refMsg.GetMessage("wiget list scroll right");
        imgWidgetHandle.Alt = m_refMsg.GetMessage("wiget list toggle");
        aWidgetTrayHandle.ToolTip = m_refMsg.GetMessage("wiget list toggle");
        liOptions.Attributes.Add("title", m_refMsg.GetMessage("generic options"));
        lblAddTabTitle.Text = m_refMsg.GetMessage("lbl personalization add tab textbox");
        imgEktronModalClose.Alt = m_refMsg.GetMessage("lbl close window");
		
		ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(this.GetType(), "ChangeIcon", "<script type='text/javascript'>$ektron('#flickerEditImage').attr('src', '" + _appPath + "/Personalization/css/images/widgetEdit.gif');</script>");

    }

    protected void apPersonalization_Load(object sender, EventArgs e)
    {
        _siteApi = new SiteAPI();

        if (!IsPostBack)
            _controller.Show(WidgetSpaceID, _foreignID);
        else
        {
            string argument = Request.Form["__EVENTARGUMENT"];

            if (argument != null && argument != "" && argument.StartsWith("{\"Controller\""))
            {
                JsonRequest jsonRequest = JsonConvert.DeserializeObject<JsonRequest>(argument);

                switch (jsonRequest.Controller)
                {
                    case "personalization":
                        {
                            PersonalizationAction(jsonRequest.Action, jsonRequest.Arguments);
                            break;
                        }

                    case "widget_list_container":
                        {
                            WidgetListContainerAction(jsonRequest.Action, jsonRequest.Arguments);
                            break;
                        }
                }
            }
        }
    }

    protected void lbAddColumn_Click(object sender, EventArgs e)
    {
        if (_pages.Length > 0)
        {
            IWidgetListContainerView view = phWidgetPages.Controls[_activePage] as IWidgetListContainerView;
            IWidgetListContainerController controller = WidgetListContainerFactory.GetController(view);
            controller.AddWidgetList(view.WidgetListContainer.ID);
        }

        //Response.Redirect(GetRedirectUrl());
    }

    protected void lbAddTab_Click(object sender, EventArgs e)
    {
        Guid? anonymousId;


        if (IsAllowAnonymous(out anonymousId))
        {
            _controller.CreatePage(_widgetSpace.ID,
                                   anonymousId.GetValueOrDefault(),
                                   tbTitle.Text,
                                   bool.Parse(ddlScope.SelectedValue));
        }
        else
        {
            _controller.CreatePage(_widgetSpace.ID,
                                   (this.EditDefault ? 0 : _foreignID),
                                   tbTitle.Text,
                                   bool.Parse(ddlScope.SelectedValue));
            errorMessages.Controls.Clear();
        }
    }

    protected void lbRemoveTabConfirmation_Click(object sender, EventArgs e)
    {
        _controller.RemovePage(_pages[_activePage].ID);
    }

    protected void lbResetWidgets_Click(object sender, EventArgs e)
    {
        _controller.Reset(_widgetSpace, _foreignID);
    }

    protected void lbEditDefaultWidgets_Click(object sender, EventArgs e)
    {
        _rediectQuerystringParams.Clear();
        _rediectQuerystringParams.Add(new RedirectQuerystringParams("editDefault", "true", RedirectQuerystringParams.ActionType.Add));
        _rediectQuerystringParams.Add(new RedirectQuerystringParams("redirect", "true", RedirectQuerystringParams.ActionType.Add));

        Response.Redirect(GetRedirectUrl());
    }

    protected void lbDone_Click(object sender, EventArgs e)
    {
        _rediectQuerystringParams.Clear();
        _rediectQuerystringParams.Add(new RedirectQuerystringParams("editDefault", "true", RedirectQuerystringParams.ActionType.Remove));
        _rediectQuerystringParams.Add(new RedirectQuerystringParams("redirect", "true", RedirectQuerystringParams.ActionType.Add));

        Response.Redirect(GetRedirectUrl());
    }

    protected void repTabs_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        m_refMsg = _contentApi.EkMsgRef;
        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;
            case ListItemType.Item:
            case ListItemType.AlternatingItem:
                HtmlGenericControl tab = (HtmlGenericControl)e.Item.FindControl("liTab");
                tab.Attributes.Add("class", GetTabClass(e.Item.ItemIndex));

                ((LinkButton)tab.FindControl("lbRemoveTab")).Visible = _Editable;
                ((LinkButton)tab.FindControl("lbRemoveTab")).ToolTip = m_refMsg.GetMessage("lbl remove");                
                ((LinkButton)tab.FindControl("lbSelectTab")).OnClientClick = GetTabClientClick(e.Item.ItemIndex);
                ((LinkButton)tab.FindControl("lbSelectTab")).CssClass = ((WidgetPageData)e.Item.DataItem).IsPublic == true ? "label" : "label private";
                ((LinkButton)tab.FindControl("lbSelectTab")).ToolTip = ((WidgetPageData)e.Item.DataItem).IsPublic == true ? EkFunctions.HtmlEncode(EkFunctions.HtmlDecode(((WidgetPageData)e.Item.DataItem).Title)) : EkFunctions.HtmlEncode(EkFunctions.HtmlDecode(((WidgetPageData)e.Item.DataItem).Title)) + " ("+m_refMsg.GetMessage("lbl private")+")";
                ((LinkButton)tab.FindControl("lbSelectTab")).Text = EkFunctions.HtmlEncode(EkFunctions.HtmlDecode(((WidgetPageData)e.Item.DataItem).Title)) == "Desktop" ? m_refMsg.GetMessage("lbl desktop"):EkFunctions.HtmlEncode(EkFunctions.HtmlDecode(((WidgetPageData)e.Item.DataItem).Title));
                break;
            case ListItemType.Footer:
                break;
        }
    }

    protected void repTabs_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch ((e.CommandSource as Control).ID)
        {
            case "lbRemoveTab":
                RemoveTab((_pages[e.Item.ItemIndex] as WidgetPageData).ID);
                break;

            case "lbSelectTab":
                SelectTab(e.Item.ItemIndex);
                break;
        }
    }

    #endregion

    #region helpers

    void BuildControlTree()
    {
        if (this.EditDefault)
            _controller.Show(_widgetSpaceID, 0);
        else
            _controller.Show(_widgetSpaceID, _foreignID);
    }

    private string GetRedirectUrl()
    {
        string url = Request.Url.ToString();
        string prefix;
        string querystring;
        foreach (RedirectQuerystringParams item in _rediectQuerystringParams)
        {
            prefix = String.Empty;
            querystring = item.Key + "=" + item.Value;
            url = url.Replace(querystring, String.Empty);
            url = url.Replace("?&", String.Empty);
            url = url.Replace("&&", "&");
            if (item.Action == RedirectQuerystringParams.ActionType.Add)
            {
                prefix = (!url.Contains("?")) ? "?" : prefix;
                prefix = (prefix == String.Empty && url.Contains("?") && url.EndsWith("?")) ? String.Empty : prefix;
                prefix = (prefix == String.Empty && url.Contains("?") && !url.EndsWith("&")) ? "&" : prefix;
                prefix = (prefix == String.Empty && url.Contains("?") && url.EndsWith("&")) ? String.Empty : prefix;
                url = url + prefix + querystring;
            }
        }

        return url;
    }

    void RenderTabs()
    {
        repTabs.DataSource = _pages;
        repTabs.DataBind();
    }

    void SelectTab(int index)
    {
        if (index >= phWidgetPages.Controls.Count || index < 0)
        {
            index = 0;
        }

        if (phWidgetPages.Controls.Count > _activePage)
        {
            phWidgetPages.Controls[_activePage].Visible = false;
        }

        IWidgetListContainerView widgetListContainerView = phWidgetPages.Controls[index] as IWidgetListContainerView;

        if (widgetListContainerView.WidgetListContainer == null)
        {
            WidgetListContainerFactory.GetController(widgetListContainerView).Show(_pages[index].WidgetListContainerID);
        }

        phWidgetPages.Controls[index].Visible = true;
        _activePage = index;

        Session["pageid"] = _activePage;

        RenderTabs();
        apPersonalization.Update();
    }

    void RemoveTab(long id)
    {
        _controller.RemovePage(id);
    }

    private void LoadPage(WidgetPageData widgetPage, bool editable)
    {
        LoadPage(widgetPage, editable, true);
    }

    private void LoadPage(WidgetPageData widgetPage, bool editable, bool show)
    {
        WidgetControls_widget_list_container ctrl = LoadControl(WidgetListContainerPath) as WidgetControls_widget_list_container;
        ctrl.ID = "widget_page_" + widgetPage.ID.ToString();
        ctrl.Visible = false;
        IWidgetListContainerView view = ctrl as IWidgetListContainerView;
        IWidgetListContainerController controller = WidgetListContainerFactory.GetController(view);
        ctrl.Editable = editable;
        phWidgetPages.Controls.Add(ctrl);
        //if(show)
        //controller.Show(widgetPage.WidgetListContainerID);
    }

    protected void AddWidget(Int64 widgetTypeId, int columnIndex, int widgetIndex)
    {
        WidgetControls_widget_list_container container =
            phWidgetPages.Controls[_activePage] as WidgetControls_widget_list_container;

        if (_activePage > 0 && (container as IWidgetListContainerView).WidgetListContainer == null)
        {
            WidgetListContainerFactory.GetController(container as IWidgetListContainerView).Show(_pages[_activePage].WidgetListContainerID);
        }

        PlaceHolder phWidgetLists = container.FindControl("phWidgetLists") as PlaceHolder;

        if (phWidgetLists.Controls.Count <= columnIndex || columnIndex < 0)
            throw new Exception("Personalization.AddWidget: Invalid column index '" + columnIndex.ToString() + "'");

        IWidgetListView columnView = phWidgetLists.Controls[columnIndex] as IWidgetListView;
        IWidgetListController controller = WidgetListFactory.GetController(columnView);

        controller.AddWidget(columnView.WidgetList.ID, widgetTypeId, widgetIndex);
    }

    protected void MoveWidget(int startColumn, int startIndex, int finalColumn, int finalIndex)
    {
        WidgetControls_widget_list_container container =
            phWidgetPages.Controls[_activePage] as WidgetControls_widget_list_container;

        container.MoveWidget(startColumn, startIndex, finalColumn, finalIndex);
    }

    protected bool IsAllowAnonymous(out Guid? anonymousId)
    {
        if (_siteApi == null)
            _siteApi = new SiteAPI();

        if (_siteApi.UserId == 0 && AllowAnonymous)
        {
            anonymousId = new Guid(_siteApi.RequestInformationRef.ClientEktGUID);
            return true;
        }
        anonymousId = null;
        return false;
    }

    protected void PersonalizationAction(string actionName, string[] args)
    {
        switch (actionName)
        {
            case "add_widget":
                {
                    AddWidgetAction action = new AddWidgetAction(args);
                    AddWidget(action.WidgetTypeID, action.ColumnIndex, action.WidgetIndex);
                    break;
                }

            default:
                {
                    throw new Exception("Personalization: Invalid action '" + actionName + "'");
                }
        }
    }

    protected void WidgetListContainerAction(string actionName, string[] args)
    {
        switch (actionName)
        {
            case "move_widget":
                {
                    MoveWidgetAction action = new MoveWidgetAction(args);
                    MoveWidget(action.StartColumn, action.StartIndex, action.FinalColumn, action.FinalIndex);
                    break;
                }
            default:
                {
                    throw new Exception("Personalization: Invalid action '" + actionName + "'");
                }
        }
    }

    private string GetTabClass(int index)
    {
        return "tab" + ((_activePage == index) ? " selected" : String.Empty);
    }

    private string GetTabClientClick(int index)
    {
        return "Ektron.Personalization.update(this);Ektron.Personalization.Tabs.select(" + index.ToString() + ");";
    }

    #endregion

    #region js/css/images

    private void RegisterJs()
    {
        //Register Personalization JS
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUISortableJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUIEffectsJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronScrollToJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronUIEffectsHighlightJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.Framework.UI.JavaScript.RegisterJavaScriptBlock(this, BuildVariablesScript(), false);
        JS.RegisterJS(this, _appPath + "/Personalization/js/ektron.personalization.js", "EktronPersonalizationJs");
        JS.RegisterJS(this, _appPath + "/PrivateData/js/Ektron.Cache.js", "EktronCacheJs");
        JS.RegisterJS(this, _appPath + "/PrivateData/js/Ektron.Crypto.js", "EktronCryptoJs");
        JS.RegisterJS(this, _appPath + "/PrivateData/js/Ektron.PrivateData.aspx", "EktronPrivateDataJs");
        JS.RegisterJS(this, JS.ManagedScript.EktronBlockUiJS);
    }

    private void RegisterCss()
    {
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        Css.RegisterCss(this, _appPath + "/Personalization/css/ektron.personalization.css", "EktronPersonalziationCss");
        Css.RegisterCss(this, _appPath + "/Personalization/css/ektron.personalization.ie.7.css", "EktronPersonalizationIe7Css", Css.BrowserTarget.IE7);
        Css.RegisterCss(this, _appPath + "/Personalization/css/ektron.personalization.ie.6.css", "EktronPersonalizationIe6Css", Css.BrowserTarget.LessThanEqualToIE6);
    }

    private string BuildVariablesScript()
    {
        m_refMsg = _contentApi.EkMsgRef;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("var addTabPopupTitle = \"" + m_refMsg.GetMessage("add tab") + "\";");
        sb.AppendLine("var removeTabTitle = \"" + m_refMsg.GetMessage("lbl personalization remove header") + "\";");
        sb.AppendLine("var removeColumnTitle = \"" + m_refMsg.GetMessage("wiget column remove") + "\";");
        sb.AppendLine("var resetWidgetTitle = \"" + m_refMsg.GetMessage("reset widgets") + "\";");
        sb.AppendLine("var editDefaultTitle = \"" + m_refMsg.GetMessage("edit default widgets") + "\";");
        return sb.ToString();
    }

    private void SetImagePaths()
    {
        string clearImagePath = _ApplicationPath + "/images/spacer.gif";
        imgWidgetTrayScrollLeft.Src = clearImagePath;
        imgWidgetTrayScrollRight.Src = clearImagePath;
        imgWidgetHandle.Src = clearImagePath;
        imgEktronModalClose.Src = clearImagePath;
        imgAddTabEktronModalOk.Src = clearImagePath;
        imgAddTabEktronModalCancel.Src = clearImagePath;
        imgRemoveTabEktronModalOk.Src = clearImagePath;
        imgRemoveTabEktronModalCancel.Src = clearImagePath;
        imgRemoveColumnEktronModalOk.Src = clearImagePath;
        imgRemoveColumnEktronModalCancel.Src = clearImagePath;
        imgResetWidgetsEktronModalOk.Src = clearImagePath;
        imgResetWidgetsEktronModalCancel.Src = clearImagePath;
        imgEditDefaultWidgetsEktronModalCancel.Src = clearImagePath;
        imgEditDefaultWidgetsEktronModalOk.Src = clearImagePath;
    }

    #endregion

    #region IWidgetSpaceView Members

    void IWidgetSpaceView.Error(string message)
    {
        // error
        Literal lit = new Literal();
        lit.Text = message;
        phWidgetPages.Controls.Clear();
        phWidgetPages.Controls.Add(lit);
    }

    void IWidgetSpaceView.Notify(string message)
    {
        // notification
    }

    void IWidgetSpaceView.View(WidgetSpaceData widgetSpace)
    {
        _widgetSpace = widgetSpace;
        m_refMsg = _contentApi.EkMsgRef;
        // Get private pages only if the pages being retrieved 
        // belong to the user who is logged in, or if the logged in 
        // user is an admin.  
        bool bIsAdmin = _siteApi.IsAdmin();
        Ektron.Cms.API.Community.CommunityGroup cg = new Ektron.Cms.API.Community.CommunityGroup();
        bIsAdmin = bIsAdmin || cg.IsCommunityGroupAdmin(_siteApi.UserId, _foreignID);

        bool bGetPrivatePages = false;
        Guid? anonymousId;

        switch (widgetSpace.Scope)
        {
            case WidgetSpaceScope.CommunityGroup:
                bGetPrivatePages = (cg.IsUserInCommunityGroup(_foreignID, _siteApi.UserId) && _siteApi.IsLoggedIn == true) || bIsAdmin;
                break;
            case WidgetSpaceScope.User:
                bGetPrivatePages = (IsAllowAnonymous(out anonymousId)) || (_siteApi.UserId == _foreignID && _siteApi.IsLoggedIn == true) || bIsAdmin;
                break;
            case WidgetSpaceScope.SmartDesktop:
                bGetPrivatePages = true;
                _isInWorkarea = true;
                break;
        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Editable", "Ektron.Personalization.SetEditable(" + bGetPrivatePages.ToString().ToLower() + ");", true);

        if (bIsAdmin)
        {
            liDone.Visible = this.EditDefault;
            liEditDefaultWidgets.Visible = liResetWidgets.Visible = !this.EditDefault;
        }
        else
        {
            liDone.Visible = false;
            liEditDefaultWidgets.Visible = false;
            liResetWidgets.Visible = true;
        }

        ulTabOptions.Visible = liOptions.Visible = _Editable = bGetPrivatePages;

        //if the page is editable, set the widgetTrayHandle to visible
        aWidgetTrayHandle.Visible = bGetPrivatePages;
        if (IsAllowAnonymous(out anonymousId))
        {
            _pages = _widgetSpace.GetPages(new Guid(anonymousId.ToString()), bGetPrivatePages);
        }
        else
        {
            _pages = _widgetSpace.GetPages((this.EditDefault ? 0 : _foreignID), bGetPrivatePages);
        }

        repTabs.Controls.Clear();

        RenderTabs();

        phWidgetPages.Controls.Clear();
        //int index = 0;
        foreach (WidgetPageData page in _pages)
        {
            LoadPage(page, bGetPrivatePages, false);
        }

        if (_pages.Length > 0)
        {
            SelectTab(_activePage);
        }
        else
        {
            //Show No Tabs Message
            if (_siteApi.IsLoggedIn && errorMessages.Controls.Count < 1)
            {
                Literal lit = new Literal();
                lit.Text = @"<html><body ><h2 class=""error"">"+m_refMsg.GetMessage("Alert no tabs created")+"</h2></body></html>";
                errorMessages.Controls.Add(lit);
            }
        }

        if (bGetPrivatePages)
        {
            repWidgetTypes.DataSource = GetWidgetTypes(widgetSpace);
            repWidgetTypes.DataBind();
        }
    }

    private WidgetTypeData[] GetWidgetTypes(WidgetSpaceData widgetSpaceData)
    {
        if (widgetSpaceData.Scope == WidgetSpaceScope.SmartDesktop)
        {
            // pass in relevant list of roles to widget lookup
            // show commerce widgets to admins and commerceadmins only
            EkEnumeration.CmsRoleIds[] roles = new EkEnumeration.CmsRoleIds[0];
            if (_contentApi.IsAdmin() && _contentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin))
            {
                roles = new EkEnumeration.CmsRoleIds[1];
                roles[0] = EkEnumeration.CmsRoleIds.CommerceAdmin;
            }
            // We are in the Workarea
            return WidgetTypeFactory.GetModel().FindAll(WidgetSpaceScope.SmartDesktop, roles);
        }

        // We are in a site
        return WidgetTypeFactory.GetModel().FindAll(widgetSpaceData.ID);
    }

    void IWidgetSpaceView.ViewPage(WidgetPageData widgetPage)
    {
        List<WidgetPageData> pages = new List<WidgetPageData>(_pages);
        pages.Add(widgetPage);
        _pages = pages.ToArray();

        //phWidgetPages.Controls.Clear();
        LoadPage(widgetPage, _siteApi.UserId == _foreignID ||
                             _siteApi.IsAdmin(),
                             true);

        if (_pages.Length == 1)
            SelectTab(0);
        else
            SelectTab(_pages.Length - 1);

        RenderTabs();
    }

    void IWidgetSpaceView.RemovePage(long pageId)
    {
        _activePage = 0;

        for (int i = 0; i < _pages.Length; i++)
        {
            if (_pages[i].ID == pageId)
            {
                List<WidgetPageData> pages = new List<WidgetPageData>(_pages);
                pages.RemoveAt(i);
                _pages = pages.ToArray();
                phWidgetPages.Controls.RemoveAt(i);
                if (_pages.Length > 0)
                    SelectTab(0);
                RenderTabs();
                break;
            }
        }
    }

    #endregion

    #region IView Members

    void IView.Error(string message)
    {
    }

    void IView.Notify(string message)
    {
    }

    #endregion
}