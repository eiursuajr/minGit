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
using Ektron.Cms.Widget;


public partial class Workarea_controls_widgetSettings_WidgetSync : System.Web.UI.UserControl
{


    protected ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string m_strPageAction = "syncwidgets";

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refContentApi.EkMsgRef;
        RegisterResources();
        Toolbar();
        grdWidgets.Columns[0].HeaderText = m_refMsg.GetMessage("lbl widgets");

        lblNoWidgets.Visible = false;
        if (Page.IsPostBack)
        {
            if (System.IO.Directory.Exists(Server.MapPath(m_refContentApi.RequestInformationRef.WidgetsPath)))
            {
                WidgetTypeController.SyncWidgetsDirectory();
                WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath);
            }
            else
            {
                lblNoWidgets.Text = m_refMsg.GetMessage("com: folder does not exist") + " " + m_refContentApi.RequestInformationRef.WidgetsPath;
                lblNoWidgets.ToolTip = lblNoWidgets.Text;
                lblNoWidgets.Visible = true;
                grdWidgets.Visible = false;
            }
        }
        WidgetTypeData[] widgetTypes = WidgetTypeFactory.GetModel().FindAll();
        Array.Sort(widgetTypes, new SortWidgetComparer());

        if (widgetTypes.Length == 0)
        {
            lblNoWidgets.Text = m_refMsg.GetMessage("lbl no widgets in cms");
            lblNoWidgets.ToolTip = lblNoWidgets.Text;
            lblNoWidgets.Visible = true;
        }
        else
        {
            grdWidgets.DataSource = widgetTypes;
            grdWidgets.DataBind();
        }
    }

    private void Toolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(string.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath + "widgets/"));
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath + "../UI/Icons/refresh.png", "#", string.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath + "widgets/"), string.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath + "widgets/"), "onclick=\"return SyncWidgets();\"", StyleHelper.RefreshButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
    }

}

public class SortWidgetComparer : System.Collections.Generic.IComparer<WidgetTypeData>
{


    public int Compare(WidgetTypeData x, WidgetTypeData y)
    {
        return x.Title.CompareTo(y.Title);
    }
}