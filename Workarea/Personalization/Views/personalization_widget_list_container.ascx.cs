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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;

public partial class WidgetControls_widget_list_container : System.Web.UI.UserControl, IWidgetListContainerView
{
    #region Constants
    
    const string WidgetListPath = "personalization_widget_list.ascx";

    #endregion

    private WidgetListContainerData _widgetListContainer;

    public void MoveWidget(int startColumnIndex, int startIndex, int finalColumnIndex, int finalIndex)
    {
        UserControl widgetControl = null;
        WidgetData widget = null;
        if (startColumnIndex == finalColumnIndex)
        {
            List<WidgetData> Column = new List<WidgetData>(_widgetListContainer.Lists[startColumnIndex].Widgets);

            UserControl ColumnControl = phWidgetLists.Controls[startColumnIndex] as UserControl;
            PlaceHolder phWidgets = ColumnControl.FindControl("phWidgets") as PlaceHolder;
            widgetControl = phWidgets.Controls[startIndex] as UserControl;

            widget = Column[startIndex];

            Column.RemoveAt(startIndex);
            Column.Insert(finalIndex, widget);

            _widgetListContainer.Lists[startColumnIndex].Widgets = Column.ToArray();
        }
        else
        {
            List<WidgetData> startColumn = new List<WidgetData>(_widgetListContainer.Lists[startColumnIndex].Widgets);
            List<WidgetData> finalColumn = new List<WidgetData>(_widgetListContainer.Lists[finalColumnIndex].Widgets);

            UserControl startColumnControl = phWidgetLists.Controls[startColumnIndex] as UserControl;
            UserControl finalColumnControl = phWidgetLists.Controls[finalColumnIndex] as UserControl;
            PlaceHolder phStartWidgets = startColumnControl.FindControl("phWidgets") as PlaceHolder;
            PlaceHolder phFinalWidgets = finalColumnControl.FindControl("phWidgets") as PlaceHolder;
            widgetControl = phStartWidgets.Controls[startIndex] as UserControl;

            widget = startColumn[startIndex];

            startColumn.RemoveAt(startIndex);
            finalColumn.Insert(finalIndex, widget);

            _widgetListContainer.Lists[startColumnIndex].Widgets = startColumn.ToArray();
            _widgetListContainer.Lists[finalColumnIndex].Widgets = finalColumn.ToArray();
        }
        WidgetFactory.GetController(widgetControl as IWidgetView).Modify(
            widget, 
            _widgetListContainer.Lists[finalColumnIndex].ID, finalIndex, 
            widget.Minimized);

        phWidgetLists.Controls.Clear();
        (this as IWidgetListContainerView).View(_widgetListContainer);
    }

    #region Properties

    public bool Editable;

    #endregion

    #region Event Handlers

    #endregion

    #region IWidgetListContainerView Members

    private event LoadDelegate HostLoad;
    event LoadDelegate IWidgetListContainerView.Load
    {
        add { HostLoad += value; }
        remove { HostLoad -= value; }
    }

    void IWidgetListContainerView.View(WidgetListContainerData widgetListContainer)
    {
        _widgetListContainer = widgetListContainer;

        foreach (WidgetListData widgetList in widgetListContainer.Lists)
        {
            (this as IWidgetListContainerView).ViewWidgetList(widgetList);
        }

        if(HostLoad != null)
            HostLoad();
    }

    void IWidgetListContainerView.ViewWidgetList(WidgetListData widgetList)
    {
        WidgetControls_widget_list widgetListControl = LoadControl(WidgetListPath) as WidgetControls_widget_list;
        widgetListControl.Editable = Editable;
        widgetListControl.ID = "widget_list_" + widgetList.ID.ToString();
        phWidgetLists.Controls.Add(widgetListControl as Control);
        (widgetListControl as IWidgetListView).View(widgetList);
    }

    WidgetListContainerData IWidgetListContainerView.WidgetListContainer
    {
        get { return _widgetListContainer; }
    }

    #endregion

    #region IView Members

    void IView.Error(string message)
    {
        Literal lit = new Literal();
        lit.Text = message;
        phWidgetLists.Controls.Clear();
        phWidgetLists.Controls.Add(lit);
    }

    void IView.Notify(string message)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion
}
