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
using Ektron.Cms.Common;
using Ektron.Cms.Commerce;
using Ektron.Cms.Content;

public partial class viewxml_config : System.Web.UI.UserControl
{


    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected int EnableMultilingual = 0;
    protected int ContentLanguage = 0;
    protected ContentAPI m_refContentApi;
    protected EkContent m_refContent;
    protected string m_strOrderBy = "title";
    protected long ConfigId = 0;
    protected bool m_bIsMac;
    protected ProductType ProductTypeAPI = null;
    protected bool bIsAdmin = false;


    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        RegisterResources();
        m_refContentApi = new ContentAPI();
        m_refMsg = m_refContentApi.EkMsgRef;
        AppImgPath = m_refContentApi.AppImgPath;
        ContentLanguage = m_refContentApi.ContentLanguage;
        m_refContent = m_refContentApi.EkContentRef;
        SetServerJSVariables();
        if (m_refContent.IsAllowed(0, 0, "users", "IsAdmin", 0) == true || m_refContent.IsARoleMember(11, m_refContent.RequestInformation.UserId, false) == true)
        {
            bIsAdmin = true;
        }

        if (Request.Browser.Platform.IndexOf("Win") == -1)
        {
            m_bIsMac = true;
        }
        else
        {
            m_bIsMac = false;
        }

        EnableMultilingual = m_refContentApi.EnableMultilingual;
        if (!(Request.QueryString["action"] == null))
        {
            m_strPageAction = Request.QueryString["action"];
            if (m_strPageAction.Length > 0)
            {
                m_strPageAction = m_strPageAction.ToLower();
            }
        }
        if (Request.QueryString["orderby"] != "")
        {
            m_strOrderBy = Request.QueryString["orderby"];
        }
    }

    #region XmlConfigs
    public bool ViewXmlConfiguration()
    {
        XmlConfigData xml_config_data;
        TR_ViewAll.Visible = false;
        TR_View.Visible = true;
        if (!(Request.QueryString["id"] == null))
        {
            ConfigId = Convert.ToInt64(Request.QueryString["id"]);
        }
        xml_config_data = m_refContentApi.GetXmlConfiguration(ConfigId);

        ViewXmlConfigToolBar(xml_config_data);

        PopulatePropertiesGrid(xml_config_data);
        PopulateDisplayGrid(xml_config_data);
        if (xml_config_data.PackageDisplayXslt.Length > 0)
        {
            PopulatePreviewGrid(xml_config_data);
        }
        return false;
    }
    private void PopulatePropertiesGrid(XmlConfigData xml_config_data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.ItemStyle.CssClass = "label";
        PropertiesGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VALUE";
        colBound.ItemStyle.CssClass = "readOnlyValue";
        PropertiesGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = "<strong class=\'headerRow\'>" + m_refMsg.GetMessage("general information") + "</strong>";
        dr[1] = "REMOVE-ITEM";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("generic title label");
        dr[1] = xml_config_data.Title;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("id label");
        dr[1] = xml_config_data.Id;
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("description label");
        dr[1] = xml_config_data.Description;
        dt.Rows.Add(dr);

        if (xml_config_data.PackageDisplayXslt.Length > 0)
        {
            // do nothing
        }
        else
        {
            Collection collLpath = (Collection)xml_config_data.LogicalPathComplete;
            dr = dt.NewRow();
            dr[0] = "<strong class=\'headerRow\'>" + m_refMsg.GetMessage("editor info label") + "</strong>";
            dr[1] = "REMOVE-ITEM";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("edit xslt label");
            dr[1] = collLpath["EditXslt"];
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("save xslt label");
            dr[1] = collLpath["SaveXslt"];
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("advanced config label");
            dr[1] = collLpath["XmlAdvConfig"];
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "<strong class=\'headerRow\'>" + m_refMsg.GetMessage("validation info label") + "</strong>";
            dr[1] = "REMOVE-ITEM";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("xml schema label");
            dr[1] = collLpath["XmlSchema"];
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = m_refMsg.GetMessage("target namespace label");
            dr[1] = collLpath["XmlNameSpace"];
            dt.Rows.Add(dr);
        }


        DataView dv = new DataView(dt);
        PropertiesGrid.DataSource = dv;
        PropertiesGrid.DataBind();
    }
    private void PopulatePreviewGrid(XmlConfigData xml_config_data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        colBound.ItemStyle.Height = Unit.Empty;
        PreviewGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = "<strong> " + m_refMsg.GetMessage("lbl Preview XSLT on empty XML document") + "</strong>";
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refContentApi.XSLTransform("<root></root>", xml_config_data.PackageDisplayXslt, false, false, null, false, true);
        dt.Rows.Add(dr);

        DataView dv = new DataView(dt);
        PreviewGrid.DataSource = dv;
        PreviewGrid.DataBind();
    }
    private void PopulateDisplayGrid(XmlConfigData xml_config_data)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        colBound.ItemStyle.Height = Unit.Empty;
        colBound.ItemStyle.CssClass = "label";
        DisplayGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VALUE";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Height = Unit.Empty;
        colBound.ItemStyle.Height = Unit.Empty;
        DisplayGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        bool bValidDefaultXslt = false;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("xslt 1 label");
        Collection collLPath = (Collection)xml_config_data.LogicalPathComplete;
        if (xml_config_data.DefaultXslt == "1")
        {

            if (collLPath["Xslt1"].ToString() != "")
            {
                bValidDefaultXslt = true;
                dr[0] += "*";
            }
        }
        dr[1] = collLPath["Xslt1"];
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("xslt 2 label");
        if (xml_config_data.DefaultXslt == "2")
        {
            if (!string.IsNullOrEmpty(collLPath["Xslt2"].ToString()))
            {
                bValidDefaultXslt = true;
                dr[0] += "*";
            }
        }
        dr[1] = collLPath["Xslt2"];
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("xslt 3 label");
        if (xml_config_data.DefaultXslt == "3")
        {
            if (!string.IsNullOrEmpty(collLPath["Xslt3"].ToString()))
            {
                bValidDefaultXslt = true;
                dr[0] += "*";
            }
        }
        dr[1] = collLPath["Xslt3"];
        dt.Rows.Add(dr);

        dr = dt.NewRow();
        dr[0] = m_refMsg.GetMessage("lbl XSLT packaged");
        if (xml_config_data.DefaultXslt == "0")
        {
            dr[0] += "*";
        }
        else
        {
            if (!(bValidDefaultXslt))
            {
                dr[0] += "*";
            }
        }
        dr[1] = "&nbsp;";
        dt.Rows.Add(dr);

        if (xml_config_data.PackageXslt.Length > 0)
        {
            dr = dt.NewRow();

            dr = dt.NewRow();
            dr[0] = "<strong class=\'headerRow\'>" + m_refMsg.GetMessage("lbl xpaths") + "</strong>";
            dr[1] = "REMOVE-ITEM";
            dt.Rows.Add(dr);


            dr = dt.NewRow();
            dr[0] = "";
            foreach (object item in m_refContentApi.GetXPaths(xml_config_data.PackageXslt))
            {
                dr[0] += "<label class=\"addLeft\">" + Convert.ToString(item) + "</label><br/>";
            }
            dr[1] = "REMOVE-ITEM";
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        DisplayGrid.DataSource = dv;
        DisplayGrid.DataBind();
    }
    protected void DisplayGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM"))
                {
                    e.Item.Cells[0].ColumnSpan = 2;
                    e.Item.Cells.RemoveAt(1);
                }
                break;
        }
    }
    private void ViewXmlConfigToolBar(XmlConfigData xml_config_data)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string pkDisplay = xml_config_data.PackageDisplayXslt; //cXmlCollection("PackageDisplayXslt")
        string PackageXslt = xml_config_data.PackageXslt; //cXmlCollection("PackageXslt")
        string caller = Request.QueryString["caller"];
        bool eIntranet = false;
        result.Append("<table><tr>");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("view xml config msg") + " \"" + xml_config_data.Title + "\""));
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers) && xml_config_data.Id == 15)
        {
            eIntranet = true;
        }
        if (caller == null)
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "xml_config.aspx?action=ViewAllXmlConfigurations", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

			bool primaryCssApplied = false;

            if (bIsAdmin)
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/add.png", "xml_config.aspx?action=NewInheritConfiguration&id=" + ConfigId + "", m_refMsg.GetMessage("alt Create a new xml configuration based on this configuration"), m_refMsg.GetMessage("btn add xml"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
				
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", "xml_config.aspx?action=EditXmlConfiguration&id=" + ConfigId + "", m_refMsg.GetMessage("alt edit button text (xml config)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass));
                if ((xml_config_data.EditXslt.Length == 0) || pkDisplay.Length > 0)
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/contentSmartFormEdit.png", "editdesign.aspx?action=EditPackage&id=" + ConfigId + "", m_refMsg.GetMessage("alt Design mode Package"), m_refMsg.GetMessage("btn data design"), "", StyleHelper.EditSmartformButtonCssClass));
                }
            }
            if (pkDisplay.Length > 0)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/FileTypes/xsl.png", "viewXslt.aspx?id=" + ConfigId + "", m_refMsg.GetMessage("alt View the presentation Xslt"), m_refMsg.GetMessage("btn view xslt"), "", StyleHelper.ViewXslButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
            }
            if (bIsAdmin && !eIntranet)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", "xml_config.aspx?action=DeleteXmlConfiguration&id=" + ConfigId + "", m_refMsg.GetMessage("alt delete button text (xml config)"), m_refMsg.GetMessage("btn delete"), "OnClick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
            }
        }
        else
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn cancel"), "OnClick=\"javascript:self.close();\"", StyleHelper.CancelButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    public bool ViewAllXmlConfigurations()
    {
        TR_ViewAll.Visible = true;
        TR_View.Visible = false;
        XmlConfigData[] xml_config_list;
        xml_config_list = m_refContentApi.GetAllXmlConfigurations(m_strOrderBy);
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = "<a href=\"xml_config.aspx?action=ViewAllXmlConfigurations&orderby=title\">" + m_refMsg.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        XMLList.Columns.Add(colBound);


        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = "<a href=\"xml_config.aspx?action=ViewAllXmlConfigurations&orderby=id\">" + m_refMsg.GetMessage("generic ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(1);
        colBound.ItemStyle.Width = Unit.Percentage(1);
        XMLList.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "DATE";
        colBound.HeaderText = "<a href=\"xml_config.aspx?action=ViewAllXmlConfigurations&orderby=LastEditDate\">" + m_refMsg.GetMessage("generic Date Modified") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = Unit.Percentage(10);
        colBound.ItemStyle.Width = Unit.Percentage(10);
        XMLList.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EDITOR";
        colBound.HeaderText = "<a href=\"xml_config.aspx?action=ViewAllXmlConfigurations&orderby=editor\">" + m_refMsg.GetMessage("generic Last Editor") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        XMLList.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("DATE", typeof(string)));
        dt.Columns.Add(new DataColumn("EDITOR", typeof(string)));
        int i = 0;
        System.Text.StringBuilder strTemp = new System.Text.StringBuilder();
        if (!(xml_config_list == null))
        {
            for (i = 0; i <= xml_config_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"xml_config.aspx?action=ViewXmlConfiguration&id=" + xml_config_list[i].Id + "\" title=\"" + m_refMsg.GetMessage("view xml config props") + "\">" + xml_config_list[i].Title + "</a>";
                dr[1] = xml_config_list[i].Id;
                dr[2] = xml_config_list[i].DisplayLastEditDate;
                dr[3] = xml_config_list[i].EditorLastName + ", " + xml_config_list[i].EditorFirstName;
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        XMLList.DataSource = dv;
        XMLList.DataBind();
        ViewAllXmlToolBar();
        return false;
    }
    private void ViewAllXmlToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        try
        {
            result.Append("<table><tr>");
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view xml configs msg"));
            if (bIsAdmin)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/add.png", "xml_config.aspx?action=AddXmlConfigurationV4", m_refMsg.GetMessage("alt add button text (xml config)"), m_refMsg.GetMessage("btn add xml"), "", StyleHelper.AddButtonCssClass, true));
            
				result.Append(StyleHelper.ActionBarDivider);
			}
            result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
            result.Append("</td>");
            result.Append("</tr></table>");
            htmToolBar.InnerHtml = result.ToString();
            result = null;
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion

    #region Product Types
    string m_sProductTypePage = "producttypes.aspx";

    public bool ViewProductType()
    {
        XmlConfigData product_type_data;
        TR_ViewAll.Visible = false;
        TR_View.Visible = true;
        if (!(Request.QueryString["id"] == null))
        {
            ConfigId = Convert.ToInt64(Request.QueryString["id"]);
        }
        product_type_data = m_refContentApi.GetXmlConfiguration(ConfigId);

        ViewProductTypeToolBar(product_type_data);

        PopulatePropertiesGrid(product_type_data);
        PopulateDisplayGrid(product_type_data);
        if (product_type_data.PackageDisplayXslt.Length > 0)
        {
            PopulatePreviewGrid(product_type_data);
        }
        return false;
    }
    private void ViewProductTypeToolBar(XmlConfigData product_type_data)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string pkDisplay = product_type_data.PackageDisplayXslt; //cXmlCollection("PackageDisplayXslt")
        string PackageXslt = product_type_data.PackageXslt; //cXmlCollection("PackageXslt")
        string caller = Request.QueryString["caller"];
        result.Append("<table><tr>");
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("lbl view product type msg") + " \"" + product_type_data.Title + "\""));
        if (caller == "")
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", m_sProductTypePage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", m_sProductTypePage + "?action=editproducttype&id=" + ConfigId + "", m_refMsg.GetMessage("alt edit button text (xml config)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
            if ((product_type_data.EditXslt.Length == 0) || pkDisplay.Length > 0)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/contentSmartFormEdit.png", "../editdesign.aspx?action=EditPackage&type=product&id=" + ConfigId + "", m_refMsg.GetMessage("alt Design mode Package"), m_refMsg.GetMessage("btn data design"), "", StyleHelper.EditSmartformButtonCssClass));
            }
            // result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/add.png", m_sProductTypePage & "?action=newinheritproducttype&id=" & ConfigId & "", m_refMsg.GetMessage("alt Create a new xml configuration based on this configuration"), m_refMsg.GetMessage("btn add xml"), ""))
            if (pkDisplay.Length > 0)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/FileTypes/xsl.png", "../viewXslt.aspx?id=" + ConfigId + "", m_refMsg.GetMessage("alt View the presentation Xslt"), m_refMsg.GetMessage("btn view xslt"), "", StyleHelper.ViewXslButtonCssClass));
            }
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/delete.png", m_sProductTypePage + "?action=deleteproducttype&id=" + ConfigId + "", m_refMsg.GetMessage("alt delete button text (xml config)"), m_refMsg.GetMessage("btn delete"), "OnClick=\"return ConfirmDelete();\"", StyleHelper.DeleteButtonCssClass));
        }
        else
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn cancel"), "OnClick=\"javascript:self.close();\"", StyleHelper.CancelButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }
    #endregion
    private void RegisterResources()
    {
        //CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }
    private void SetServerJSVariables()
    {
        ltr_delXMLConfig.Text = m_refMsg.GetMessage("js: confirm xml config delete");
    }
}
	
