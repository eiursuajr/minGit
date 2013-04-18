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
using System.Text;
using System.Collections.Generic;
//using ASP;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API;
using Ektron.Cms.Content;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;

public partial class WidgetControls_widget_list : System.Web.UI.UserControl, IWidgetListView
{
    #region Constants
    public const string WidgetPath = "personalization_widget.ascx";
    #endregion

    #region Member Variables

    IWidgetListController _controller;
    private WidgetListData _widgetList;
    private ContentAPI _contentApi;
    private string _ApplicationPath;
    private string _SitePath;
    protected EkMessageHelper m_refMsg;
    #endregion

    #region Properties
    public bool Editable;
    #endregion

    #region Init, Load

    public WidgetControls_widget_list()
    {
        _contentApi = new ContentAPI();
        _ApplicationPath = _contentApi.ApplicationPath.TrimEnd(new char[] { '/' });
        _SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
    }

    protected override void OnInit(EventArgs e)
    {
        

        _controller = WidgetListFactory.GetController(this as IWidgetListView);

        base.OnInit(e);
    }

    protected override void OnLoad(EventArgs e)
    {

        m_refMsg = _contentApi.EkMsgRef;

        //set image paths
        string clearImagePath = _ApplicationPath + "/images/spacer.gif";
        imgColumnRemove.Src = clearImagePath;
        imgColumnRemoveHover.Src = clearImagePath;

        imgColumnRemove.Alt = m_refMsg.GetMessage("wiget column remove");
        imgColumnRemoveHover.Alt = m_refMsg.GetMessage("wiget column remove");
        base.OnLoad(e);
    }

    #endregion

    #region Event Handlers

    protected void lbRemoveWidgetList_Click(object sender, EventArgs e)
    {
        _controller.Remove(_widgetList.ID);
    }

    #endregion

    #region Helper Functions

    void LoadWidget(WidgetData widget)
    {
        WidgetControls_widget ctrl = LoadControl(WidgetPath) as WidgetControls_widget;
        ctrl.Editable = Editable;
        ctrl.ID = "widget_" + widget.ID.ToString();
        phWidgets.Controls.Add(ctrl as Control);
        (ctrl as IWidgetView).View(widget);
    }

    void CreateWidget(WidgetData widget)
    {
        WidgetControls_widget ctrl = LoadControl(WidgetPath) as WidgetControls_widget;
        ctrl.Editable = Editable;
        ctrl.ID = "widget_" + widget.ID.ToString();
        phWidgets.Controls.AddAt((int)widget.Order, ctrl as Control);
        (ctrl as IWidgetView).View(widget);
    }

    #endregion

    #region IWidgetListView Members

    event LoadDelegate IWidgetListView.Load
    {
        add { _parentView.Load += value; }
        remove { _parentView.Load += value; }
    }

    IWidgetListContainerView _parentView;
    IWidgetListContainerView IWidgetListView.ParentView
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

    void IWidgetListView.Remove()
    {
        // NOTIFY PARENT
        Parent.Controls.Remove(this);
    }

    void IWidgetListView.View(WidgetListData widgetList)
    {
        _widgetList = widgetList;

        foreach (WidgetData widget in widgetList.Widgets)
        {
            LoadWidget(widget);
        }
    }

    void IWidgetListView.ViewWidget(WidgetData widget)
    {
        CreateWidget(widget);
    }

    WidgetListData IWidgetListView.WidgetList
    {
        get { return _widgetList; }
    }

    #endregion

    #region IView Members

    void IView.Error(string message)
    {
        Literal lit = new Literal();
        lit.Text = message;
        phWidgets.Controls.Clear();
        phWidgets.Controls.Add(lit);
    }

    void IView.Notify(string message)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion
}
