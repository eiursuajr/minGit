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
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API;
using Ektron.Cms.Content;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;
using Ektron.Cms.Widget.Factories;
using Ektron.Cms.Widget.Helpers;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public partial class WidgetControls_widget : System.Web.UI.UserControl, IWidgetView, IWidgetHost
{
    #region Constants
    public const string WidgetPath = "personalization_widget.ascx";
    #endregion

    #region Member Variables

    private ContentAPI _contentApi;
    private SiteAPI _siteApi;

    IWidgetController _controller;
    UserControl _userWidget;
    private string _ApplicationPath;
    private string _SitePath;

    protected EkMessageHelper m_refMsg;
    #endregion

    #region Properties
    public bool Editable;
    #endregion

    #region Constructor

    protected WidgetControls_widget()
    {
        _contentApi = new ContentAPI();
        _siteApi = new SiteAPI();
        _ApplicationPath = _contentApi.ApplicationPath.TrimEnd(new char[] { '/' });
        _SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
    }

    #endregion

    #region Event Handlers

    protected override void OnInit(EventArgs e)
    {
        _controller = WidgetFactory.GetController(this);
        base.OnInit(e);
    }

    protected override void OnLoad(EventArgs e)
    {
        //ensure edit mode is off
        lbEdit.Attributes.Remove("data-ektron-editMode");

        string sitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });

        //set image paths
        string clearImagePath = _ApplicationPath + "/images/spacer.gif";
        m_refMsg = _contentApi.EkMsgRef;
        imgWidgetEdit.Src = clearImagePath;
        imgWidgetRestore.Src = clearImagePath;
        imgWidgetMinimize.Src = clearImagePath;
        imgWidgetClose.Src = clearImagePath;
        lbCloseWidget.ToolTip=imgWidgetClose.Alt = m_refMsg.GetMessage("wiget close");
        lbEdit.ToolTip=imgWidgetEdit.Alt = m_refMsg.GetMessage("wiget edit");
        lbMinimize.ToolTip=imgWidgetMinimize.Alt = m_refMsg.GetMessage("wiget minimize");
        lbRestore.ToolTip=imgWidgetRestore.Alt = m_refMsg.GetMessage("wiget restore");

        base.OnLoad(e);
    }
    protected override void OnPreRender(EventArgs e)
    {
        //set widget token image path
        if (_userWidget != null)
        {
            hdnWidgetTokenImagePath.Value = _userWidget.TemplateSourceDirectory + "/" + _widget.ControlURL + ".jpg";
        }

        if (_widget != null)
        {
            hdnWidgetTitle.Value = _widget.ControlURL.Substring(0, _widget.ControlURL.Length - 5);
            WidgetTypeModel wtm = new WidgetTypeModel();
            WidgetTypeData wtd = new WidgetTypeData();
            wtm.FindByControlURL(_widget.ControlURL, out wtd);
            if (wtd != null)
            {
                hdnwidgetTokenTypeId.Value = wtd.ID.ToString();
            }
        }

        base.OnPreRender(e);
    }
    protected void lbRestore_Click(object sender, EventArgs e)
    {
        if (HostMaximize != null)
            HostMaximize();

        _controller.Modify(_widget, _widget.ColumnID, _widget.Order, false);

        lbEdit.Visible = Editable && HostEdit != null && !_widget.Minimized;
        lbMinimize.Visible = true;
        lbRestore.Visible = false;
    }
    protected void lbMinimize_Click(object sender, EventArgs e)
    {
        if (HostMinimize != null)
            HostMinimize();

        lbEdit.Visible = false;
        lbMinimize.Visible = false;
        lbRestore.Visible = true;

        _controller.Modify(_widget, _widget.ColumnID, _widget.Order, true);
    }
    protected void lbEdit_Click(object sender, EventArgs e)
    {
        lbEdit.Attributes.Add("data-ektron-editMode", "true");
        _controller.Edit(_widget);
    }
    protected void lbClose_Click(object sender, EventArgs e)
    {
        _controller.Remove(_widget.ID);
    }

    #endregion

    #region IWidgetView Members

    void IWidgetView.New(WidgetData widget)
    {
        (this as IWidgetView).View(widget);
        if (HostCreate != null)
            HostCreate();
    }

    private IWidgetListView _parentView;
    IWidgetListView IWidgetView.ParentView
    {
        get
        {
            return _parentView;
        }
        set
        {
            _parentView = value;
        }
    }

    void IWidgetView.Remove()
    {
        // NOTIFY PARENT OF REMOVAL
        Parent.Controls.Remove(this);
    }

    void IWidgetView.View(WidgetData widget)
    {
        _widget = widget;
        try
        {
            bool isInWorkarea = (HttpContext.Current.Request.FilePath.ToLower().IndexOf("/workarea/") > -1);
            //string widgetsDirectoryPath = isInWorkarea ? "/Workarea/Widgets/" : "/Widgets/";
            string widgetsDirectoryPath = (isInWorkarea ? _contentApi.ApplicationPath : _contentApi.SitePath)
                                + "/Widgets/";

            _userWidget = LoadControl(/*HttpContext.Current.Request.ApplicationPath + */widgetsDirectoryPath + widget.ControlURL) as UserControl;
            (this as IWidgetHost).LoadWidgetDataMembers();
            phWidgetContent.Controls.Clear();
            phWidgetContent.Controls.Add(_userWidget);

            lbCloseWidget.Visible = Editable;
            lbEdit.Visible = Editable && HostEdit != null && !_widget.Minimized;
            lbMinimize.Visible = Editable && HostMinimize != null && !_widget.Minimized;
            lbRestore.Visible = Editable && HostMaximize != null && _widget.Minimized;

            if (_widget.Minimized == true && HostMinimize != null)
            {
                HostMinimize();
                if (HostClose != null)
                {
                    //DO nothing
                }
            }
        }
        catch (HttpException ex)
        {
            lbCloseWidget.Visible = true;
            lbEdit.Visible = false;
            lbMinimize.Visible = false;
            lbRestore.Visible = false;
            phWidgetContent.Controls.Clear();
            Literal lblWarning = new Literal();
            lblWarning.Text = ex.Message.ToString();
            phWidgetContent.Controls.Add(lblWarning);     
        }
        catch (Exception ex)
        {
            EkException.WriteToEventLog("Failed to load " + widget.ControlURL, System.Diagnostics.EventLogEntryType.Error);
            EkException.LogException(ex, System.Diagnostics.EventLogEntryType.Error);
        }
    }

    void IWidgetView.ViewSettings(string settings)
    {
        if (HostEdit != null)
            HostEdit(settings);
    }

    private WidgetData _widget;
    WidgetData IWidgetView.Widget
    {
        get { return _widget; }
    }

    #endregion

    #region IView Members

    void IView.Error(string message)
    {
        Literal lit = new Literal();
        lit.Text = message;
        phWidgetContent.Controls.Clear();
        phWidgetContent.Controls.Add(lit);
    }

    void IView.Notify(string message)
    {
    }

    #endregion

    #region IWidgetHost Members

    private event CloseDelegate HostClose;
    event CloseDelegate IWidgetHost.Close
    {
        add { HostClose += value; }
        remove { HostClose -= value; }
    }

    private event CreateDelegate HostCreate;
    event CreateDelegate IWidgetHost.Create
    {
        add { HostCreate += value; }
        remove { HostCreate -= value; }
    }

    void IWidgetHost.Delete()
    {
        _controller.Remove(_widget.ID);
    }

    private event EditDelegate HostEdit;
    event EditDelegate IWidgetHost.Edit
    {
        add { HostEdit += value; }
        remove { HostEdit -= value; }
    }

//    private event LoadDelegate HostLoad;
    event LoadDelegate IWidgetHost.Load
    {
        add { _parentView.Load += value; }
        remove { _parentView.Load -= value; }
    }

    private event MaximizeDelegate HostMaximize;
    event MaximizeDelegate IWidgetHost.Maximize
    {
        add { HostMaximize += value; }
        remove { HostMaximize -= value; }
    }

    private event MinimizeDelegate HostMinimize;
    event MinimizeDelegate IWidgetHost.Minimize
    {
        add { HostMinimize += value; }
        remove { HostMinimize -= value; }
    }

    void IWidgetHost.Save(string settings)
    {
        WidgetSettingsFactory.GetModel().Set(_widget.ID, WidgetSettingsHelper.DefaultSettingsKey, settings);
    }

    void IWidgetHost.Save(string key, string value)
    {
        WidgetSettingsFactory.GetModel().Set(_widget.ID, key, value);
    }

    string IWidgetHost.Title
    {
        get
        {
            return lblTitle.Text;
        }
        set
        {
            lblTitle.Text = value;
        }
    }

    bool IWidgetHost.IsEditable
    {
        get
        {
            return Editable;
        }
    }

    Expandable _expandable = Expandable.DontExpand;
    Expandable IWidgetHost.ExpandOptions
    {
        get { return _expandable; }
        set { _expandable = value; }
    }

    string _helpfile = "";
    string IWidgetHost.HelpFile
    {
        get { return _helpfile; }
        set { _helpfile = value; }
    }

    WidgetData IWidgetHost.WidgetInfo
    {
        get { return (this as IWidgetView).Widget; }
    }

    void IWidgetHost.LoadWidgetDataMembers()
    {
        if (_userWidget != null)
        {
            WidgetTypeModel widgetTypeModel = new WidgetTypeModel();
            WidgetModel widgetModel = new WidgetModel();
            WidgetTypeData widgettype = null;
            if (widgetTypeModel.FindByControlURL(_widget.ControlURL, out widgettype))
            {
                Ektron.Cms.PageBuilder.WidgetHost wh = new Ektron.Cms.PageBuilder.WidgetHost();
                wh.PopulateWidgetProperties(ref _userWidget, ref widgettype, _widget.Settings);
            }
        }
    }

    void IWidgetHost.SaveWidgetDataMembers()
    {
        if (_userWidget != null)
        {
            List<dataStore> SettingsToSave = new List<dataStore>();
            WidgetModel wm = new WidgetModel();
            Ektron.Cms.PageBuilder.WidgetHost wh = new Ektron.Cms.PageBuilder.WidgetHost();
            if (wh.SaveWidgetProperties(ref _userWidget, out SettingsToSave))
            {
                (this as IWidgetHost).Save(dataStore.Serialize(SettingsToSave));
            }
        }
    }

    string IWidgetHost.GetSetting(string key)
    {
        throw new Exception("IWidgetHost.GetSetting not implemented");
    }

    #endregion
}
