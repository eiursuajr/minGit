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
using Ektron.Cms.API;
using Ektron.Cms.Workarea;
using Ektron.Cms.Personalization;
using System.IO;
using Ektron.Cms.Widget;
using Microsoft.Security.Application;
using Ektron.Cms.Common;

public partial class Workarea_controls_widgetSettings_WidgetSpace : System.Web.UI.UserControl, System.Web.UI.ICallbackEventHandler
{

    protected ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "widgetsspace";
    protected string m_mode = "";
    protected long m_id = 0;
    protected SiteAPI m_siteApi = new SiteAPI();


    protected void Page_Init(object sender, System.EventArgs e)
    {
        m_refMsg = m_refContentApi.EkMsgRef;
        RegisterResources();
        jsWidgetHandlerPath.Text = m_refContentApi.SitePath + "Workarea/controls/widgetSettings/";

        //Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().
        if (!string.IsNullOrEmpty(Request.QueryString["mode"]))
        {
            m_mode = Request.QueryString["mode"];
        }

        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            m_id = Convert.ToInt64(Request.QueryString["id"]);
        }

        if (!Page.IsPostBack)
        {
            switch (m_mode.ToLower())
            {
                case "add":
                    AddWidgetsSpace();
                    break;
                case "edit":
                    AddWidgetsSpace();
                    break;
                case "remove":
                    RemoveWidgetsSpace();
                    break;
                default:
                    ViewAllWidgetSpaces();
                    break;
            }
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {

        //Using PageBuilder common text values
        lblSelectWidgets.Text = m_refMsg.GetMessage("lbl pagebuilder select widgets");
        widgetTitle.Text = m_refMsg.GetMessage("lbl pagebuilder widgets title");
        btnSelectNone.Text = m_refMsg.GetMessage("lbl pagebuilder select none");
        btnSelectNone.ToolTip = "#" + (btnSelectNone.Text).Replace(" ", "");
        btnSelectAll.Text = m_refMsg.GetMessage("lbl pagebuilder select all");
        btnSelectAll.ToolTip = "#" + (btnSelectAll.Text).Replace(" ", "");
        rdoGroupSpace.Text = m_refMsg.GetMessage("group space label");
        rdoUserSpace.Text = m_refMsg.GetMessage("user space label");

        Css.RegisterCss(this, "csslib/ektron.widgets.selector.css", "EktronWidgetsSelectorCss");

        //Gets all Widgets in Add mode
        if (Request.QueryString["mode"] != "" && Request.QueryString["mode"] == "add")
        {
            Ektron.Cms.Widget.WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath);
            Ektron.Cms.Widget.WidgetTypeModel model = new Ektron.Cms.Widget.WidgetTypeModel();
            Ektron.Cms.Widget.WidgetTypeData[] widgetTypes = model.FindAll();
            repWidgetTypes.DataSource = AppendWidgetPath(widgetTypes);
            repWidgetTypes.DataBind();
        }

        if (Page.IsPostBack)
        {
            switch (m_mode.ToLower())
            {
                case "add":
                    doAddWidgetSpace();
                    break;
                case "edit":
                    doUpdateWidgetSpace();
                    break;
                case "remove":
                    doRemoveWidgetSpace();
                    break;
            }
        }
    }

    private void ViewAllWidgetSpaces()
    {
        ViewSet.SetActiveView(ViewAll);
        Ektron.Cms.Personalization.WidgetSpaceData[] spaceData;
        spaceData = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll();
        if (spaceData.Length == 0)
        {
            lblNoWidgetSpaces.Text = m_refMsg.GetMessage("lbl no widget space setup");
            lblNoWidgetSpaces.ToolTip = lblNoWidgetSpaces.Text;
            lblNoWidgetSpaces.Visible = true;
        }
        else
        {
            lblNoWidgetSpaces.Visible = false;
        }
        this.ViewAllRepeater.DataSource = spaceData;
        this.ViewAllRepeater.DataBind();
        ViewAllToolbar(spaceData);
    }

    private void AddWidgetsSpace()
    {
        ViewSet.SetActiveView(ViewAdd);
        ViewAddEditToolbar();
        Page.SetFocus(txtTitle);
        DisplayAddWidgetSpace();
    }

    private void DisplayAddWidgetSpace()
    {
        lblWidgetsSpaceTitle.Text = m_refMsg.GetMessage("generic title label");
        lblWidgetsSpaceTitle.ToolTip = lblWidgetsSpaceTitle.Text;
        ltrGroupSpace.Text = m_refMsg.GetMessage("lbl widgets space");
        rdoGroupSpace.Checked = true;
    }

    private void RemoveWidgetsSpace()
    {
        ViewSet.SetActiveView(ViewRemove);
        Ektron.Cms.Personalization.WidgetSpaceData[] spaceData;
        spaceData = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll(true);
        ViewRemoveToolbar((spaceData.Length == 0));
        if (spaceData.Length == 0)
        {
            lblNoWidgetSpaces.Text = m_refMsg.GetMessage("lbl no widget space setup");
            lblNoWidgetSpaces.ToolTip = lblNoWidgetSpaces.Text;
            lblNoWidgetSpaces.Visible = true;
        }
        else
        {
            lblNoWidgetSpaces.Visible = false;
        }
        this.viewAllForRemove.DataSource = spaceData;
        this.viewAllForRemove.DataBind();
    }

    #region Actions
    private void doAddWidgetSpace()
    {
        WidgetSpaceData widgetSpace = null;
        string title = "";
        Ektron.Cms.Personalization.WidgetSpaceScope scope = WidgetSpaceScope.User;
        Ektron.Cms.Personalization.WidgetSpaceModel model = new Ektron.Cms.Personalization.WidgetSpaceModel();
        bool widgetSpaceCreated = false;
        if (rdoGroupSpace.Checked)
        {
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup;
        }
        else if (rdoUserSpace.Checked)
        {
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.User;
        }

        title = EkFunctions.HtmlEncode(Request.Form[txtTitle.UniqueID]);
        widgetSpaceCreated = System.Convert.ToBoolean(Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Create(title, scope, out widgetSpace));
        if (widgetSpaceCreated)
        {
            //Adding widgetSpace_To_widgets association
            foreach (string Key in Request.Form.AllKeys)
            {
                if (Key.StartsWith("widget"))
                {
                    try
                    {
                        model.AddWidgetSpaceAssociation(widgetSpace.ID, long.Parse(Key.Substring(6)));
                    }
                    catch (Exception ex)
                    {
                        EkException.ThrowException(ex);
                    }
                }
            }
        }
        Response.Redirect("widgetsettings.aspx?action=widgetspace", false);
        //ViewAllWidgetSpaces()
    }

    private void doUpdateWidgetSpace()
    {
        string title = "";
        Ektron.Cms.Personalization.WidgetSpaceScope scope = WidgetSpaceScope.User;
        Ektron.Cms.Personalization.WidgetSpaceModel model = new Ektron.Cms.Personalization.WidgetSpaceModel();
        if (rdoGroupSpace.Checked)
        {
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup;
        }
        else if (rdoUserSpace.Checked)
        {
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.User;
        }
        else
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.SmartDesktop;
        title = EkFunctions.HtmlEncode(Request.Form[txtTitle.UniqueID]);

        Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Update(m_id, scope, title);
        model.RemoveAllWidgetSpaceAssociations(m_id);
        foreach (string Key in Request.Form.AllKeys)
        {
            if (Key.StartsWith("widget"))
            {
                try
                {
                    model.AddWidgetSpaceAssociation(m_id, long.Parse(Key.Substring(6)));
                }
                catch (Exception ex)
                {
                    EkException.ThrowException(ex);
                }
            }
        }

        Response.Redirect("widgetsettings.aspx?action=widgetspace", false);
        //ViewAllWidgetSpaces()
    }

    private void doRemoveWidgetSpace()
    {
        Ektron.Cms.Personalization.WidgetSpaceData[] widgetSpaces = null;

        widgetSpaces = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll();
        foreach (Ektron.Cms.Personalization.WidgetSpaceData data in widgetSpaces)
        {
            string check = Request.Form["chkSpace" + data.ID];
            if ((check != null) && check == "on")
            {
                Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Remove(data.ID);
            }
        }
        Response.Redirect("widgetsettings.aspx?action=widgetspace", false);
        //ViewAllWidgetSpaces()
    }
    #endregion

    #region Toolbars
    private void ViewAllToolbar(Ektron.Cms.Personalization.WidgetSpaceData[]  spaceData)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/add.png", "Widgetsettings.aspx?action=widgetsspace&mode=add", m_refMsg.GetMessage("lbl add widgets space title"), m_refMsg.GetMessage("lbl add widgets space alt"), "", StyleHelper.AddButtonCssClass, true));
        if (spaceData.Length > 1)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/delete.png", "Widgetsettings.aspx?action=widgetsspace&mode=remove", m_refMsg.GetMessage("lbl remove widgets space title"), m_refMsg.GetMessage("lbl remove widgets space alt"), "", StyleHelper.RemoveButtonCssClass));
        }
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void ViewAddEditToolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string toolTip1 = m_refMsg.GetMessage("lbl add widgets space title");
        string toolTip2 = m_refMsg.GetMessage("lbl add widgets space alt");
        if (m_mode.ToLower() != "add")
        {
            toolTip1 = m_refMsg.GetMessage("lbl save new widgets space title");
            toolTip2 = m_refMsg.GetMessage("lbl save new widgets space alt");
        }
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/back.png", "Widgetsettings.aspx?action=widgetsspace", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("alt back button text"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", toolTip1, toolTip2, "onclick=\"return VerifyWidgetsSpace(\'" + m_mode + "\', " + m_id + ");\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void ViewRemoveToolbar(bool hideDeleteButton)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/back.png", "Widgetsettings.aspx?action=widgetsspace", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("alt back button text"), "", StyleHelper.BackButtonCssClass, true));
		if (!hideDeleteButton)
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("lbl delete widgets space title"), m_refMsg.GetMessage("lbl delete widgets space alt"), "onclick=\"return VerifyWidgetsSpace(\'remove\', 0);\"", StyleHelper.DeleteButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion

    public string GetCallbackResult()
    {
        return "";
    }

    public void RaiseCallbackEvent(string eventArgument)
    {

    }

    protected void editButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        m_mode = "edit";
        m_id = Convert.ToInt64(((ImageButton)sender).CommandArgument);
        AddWidgetsSpace();
        WidgetSpaceData spaceData = null;
        Ektron.Cms.Personalization.WidgetSpaceModel model = new Ektron.Cms.Personalization.WidgetSpaceModel();
        Ektron.Cms.Widget.WidgetTypeData[] widgetTypes;
        WidgetSpaceFactory.GetModel().FindByID(m_id, out spaceData);
        //Get widgets based on scope in Edit Mode
        if (spaceData.Scope == Ektron.Cms.Personalization.WidgetSpaceScope.SmartDesktop)
        {
            tr_groupSpace.Visible = false;
            widgetTypes = WidgetTypeFactory.GetModel().FindAll(WidgetSpaceScope.SmartDesktop);
        }
        else
        {
            Ektron.Cms.Widget.WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath);
            widgetTypes = WidgetTypeFactory.GetModel().FindAll();
        }
        repWidgetTypes.DataSource = AppendWidgetPath(widgetTypes);
        repWidgetTypes.DataBind();
        if (spaceData != null)
        {
            txtTitle.Text = Server.HtmlDecode(spaceData.Title);

            rdoGroupSpace.Checked = spaceData.Scope == Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup ? true : false;
            rdoUserSpace.Checked = spaceData.Scope == Ektron.Cms.Personalization.WidgetSpaceScope.User  ? true : false;
            ViewSet.SetActiveView(this.ViewAdd);
        }

        //Sync earlier selected widgets during edit mode
        Ektron.Cms.Widget.WidgetTypeData[] selectedWidgets = model.GetAssociatedWidgetTypesByWidgetSpaceID(m_id);
        List<string> widgetIds = new List<string>();
        foreach (Ektron.Cms.Widget.WidgetTypeData widget in selectedWidgets)
        {
            if (spaceData.Scope == Ektron.Cms.Personalization.WidgetSpaceScope.SmartDesktop && widget.Scope == Ektron.Cms.Personalization.WidgetSpaceScope.SmartDesktop)
            {
                widgetIds.Add(widget.ID.ToString());
            }
            else if (spaceData.Scope != Ektron.Cms.Personalization.WidgetSpaceScope.SmartDesktop)
            {
                widgetIds.Add(widget.ID.ToString());
            }
        }
        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "widgetSpaceSelectedIds", "Ektron.ready(function(){SelectWidgets([" + string.Join(", ", widgetIds.ToArray()) + "]);});", true);
    }
    public WidgetTypeData[] AppendWidgetPath(WidgetTypeData[] widgetTypes)
    {
        List<WidgetTypeData> widgetTypeList = new List<WidgetTypeData>();
        foreach (Ektron.Cms.Widget.WidgetTypeData widget in widgetTypes)
        {
            if (widget.Scope == WidgetSpaceScope.SmartDesktop)
            {
                widget.ControlURL = m_refContentApi.RequestInformationRef.ApplicationPath + "widgets/" + widget.ControlURL.Replace("\\", "/");
            }
            else
            {
                widget.ControlURL = m_refContentApi.RequestInformationRef.WidgetsPath + widget.ControlURL;
            }
            Ektron.Cms.Widget.WidgetTypeData widgettypeData = new Ektron.Cms.Widget.WidgetTypeData();
            Ektron.Cms.Widget.IWidget chkIsIWidget;
            chkIsIWidget = Page.LoadControl(widget.ControlURL) as Ektron.Cms.Widget.IWidget;
            if (chkIsIWidget != null)
            {
                widgettypeData.Active = widget.Active;
                widgettypeData.ButtonText = widget.ButtonText;
                widgettypeData.ControlURL = widget.ControlURL;
                widgettypeData.ID = widget.ID;
                widgettypeData.Scope = widget.Scope;
                widgettypeData.Settings = widget.Settings;
                widgettypeData.Title = widget.Title;
                widgetTypeList.Add(widgettypeData);
            }
        }
        return widgetTypeList.ToArray();
    }

    private void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, "controls/widgetsettings/ektron.widgetSpace.js", "EktronWidgetSpaceJS");
    }
}
