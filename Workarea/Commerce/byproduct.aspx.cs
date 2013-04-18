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
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public partial class Commerce_byproduct : workareabase
{
    protected string m_selectedFolderList = "";
    protected bool GetProducts = true;
    protected bool GetBundles = true;
    protected bool GetComplexProducts = true;
    protected bool GetKits = true;
    protected int TotalPagesNumber = 1;
    protected int _currentPageNumber = 1;
    protected string strContType;
    protected LocalizationAPI objLocalizationApi = null;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
		Utilities.ValidateUserLogin();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        CatalogEntry CatalogManager = new CatalogEntry(m_refContentApi.RequestInformationRef);
        System.Collections.Generic.List<CatalogData> catalogData = new System.Collections.Generic.List<CatalogData>();

        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;

        switch (m_sPageAction)
        {
            case "crosssellselect":
            case "upsellselect":
            case "couponselect":
            case "select":
            case "reportingselect":
                Display_ViewAll();
                break;
            default:
                catalogData = CatalogManager.GetCatalogList(1, 1);
                populateFolder(catalogData);
                break;
        }
    }
    protected void Display_ViewAll()
    {
        CatalogEntryApi CatalogManager = new CatalogEntryApi();
        System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
        Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();
        string OrderBy = "";

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_iID);
        entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage);

        if (Request.QueryString["orderby"] != null)
        {
            OrderBy = Request.QueryString["orderby"];
        }

        switch (m_sPageAction)
        {
            case "couponselect":
            case "crosssellselect":
            case "upsellselect":
            case "reportingselect":
                break;
            // do nothing
            default:
                GetProducts = true;
                GetBundles = false;
                GetComplexProducts = false;
                GetKits = false;
                long[] IdList = new long[2];
                IdList[0] = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product);
                entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList);
                break;
        }

        switch (OrderBy.ToLower())
        {
            case "title":
                entryCriteria.OrderByField = EntryProperty.Title;
                break;
            case "language":
                entryCriteria.OrderByField = EntryProperty.LanguageId;
                break;
            case "id":
                entryCriteria.OrderByField = EntryProperty.Id;
                break;
            case "status":
                entryCriteria.OrderByField = EntryProperty.Status;
                break;
            case "datemodified":
                entryCriteria.OrderByField = EntryProperty.EntryType;
                break;
            case "editor":
                entryCriteria.OrderByField = EntryProperty.ListPrice;
                break;
            default:
                entryCriteria.OrderByField = EntryProperty.LastEditDate;
                break;
        }

        entryCriteria.PagingInfo.CurrentPage = this.ucPaging.SelectedPage + 1;
        entryList = CatalogManager.GetList(entryCriteria);

        if (entryCriteria.PagingInfo.TotalPages > 1)
        {
            this.ucPaging.Visible = true;
            this.ucPaging.TotalPages = entryCriteria.PagingInfo.TotalPages;
            this.ucPaging.CurrentPageIndex = this.ucPaging.SelectedPage;
        }

        if (TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {
            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(TotalPagesNumber))).ToString();
            TotalPages.ToolTip = TotalPages.Text;
            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;
        }

        Populate_ViewCatalogGrid(entryList);
    }
    protected string Util_GetLinkJS(EntryData entryList)
    {
        string sRet = "";
        switch (m_sPageAction)
        {
            case "couponselect":
                sRet = "parent.addRowToTable(null, " + entryList.Id + ", " + entryList.LanguageId + ", \'" + entryList.Title + "\', " + entryList.EntryType + "); parent.ektb_remove();";
                break;
            case "crosssellselect":
            case "upsellselect":
                sRet = "parent.addRowToTable(null, " + entryList.Id + ", \'" + entryList.Title + "\'); parent.ektb_remove();";
                break;
            default:
                sRet = "parent.location.href=\'fulfillment.aspx?action=byproduct&productid=" + entryList.Id + "\'; ";
                break;
        }
        return sRet;
    }
    private void Populate_ViewCatalogGrid(System.Collections.Generic.List<EntryData> entryList)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;

        if (Request.QueryString["action"] != null)
        {
            if (Request.QueryString["action"] == "reportingselect")
            {
                this.AddBackButton("javascript:history.go(-1);");
            }
        }

        strTag = "<a href=\"byproduct.aspx?LangType=" + ContentLanguage + "&action=" + m_sPageAction + "&orderby=";
        strtag1 = "&id=" + m_iID + (strContType != "" ? "&" + Ektron.Cms.Common.EkConstants.ContentTypeUrlParam + "=" + strContType : "") + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">";

        colBound.DataField = "TITLE";
        colBound.HeaderText = strTag + "Title" + strtag1 + m_refMsg.GetMessage("generic Title") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "LANGUAGE";
        colBound.HeaderText = strTag + "language" + strtag1 + m_refMsg.GetMessage("generic language") + "</a>";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = strTag + "ID" + strtag1 + m_refMsg.GetMessage("generic ID") + "</a>";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "STATUS";
        colBound.HeaderText = strTag + "status" + strtag1 + m_refMsg.GetMessage("generic Status") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = strTag + "DateModified" + strtag1 + m_refMsg.GetMessage("generic type") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PRICE";
        colBound.HeaderText = strTag + "editor" + strtag1 + m_refMsg.GetMessage("lbl calatog entry price") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);
        FolderDataGrid.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(long)));
        dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));
        dt.Columns.Add(new DataColumn("PRICE", typeof(string)));

        int i;

        objLocalizationApi = new LocalizationAPI();

        for (i = 0; i <= (entryList.Count - 1); i++)
        {
            dr = dt.NewRow();
            string dmsMenuGuid;
            dmsMenuGuid = (string)(System.Guid.NewGuid().ToString());

            dr[0] = dr[0] + "<a";
            dr[0] = dr[0] + " id=\"dmsViewItemAnchor" + entryList[i].Id + entryList[i].LanguageId + dmsMenuGuid + "\"";
            dr[0] = dr[0] + " class=\"dmsViewItemAnchor\"";
            dr[0] = dr[0] + " onclick=\"" + Util_GetLinkJS(entryList[i]) + "\"";
            dr[0] = dr[0] + " href=\"#\"";
            //dr(0) = dr(0) + " title=""View " & entryList(i).Title & """"
            dr[0] = dr[0] + ">";
            if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {

                dr[0] = dr[0] + "<img border=\"0\" title=\"" + entryList[i].Title + "\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/bookGreen.png" + "\">&nbsp;</img><span title=\"" + entryList[i].Title + "\">" + entryList[i].Title + "</span>";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
            {

                dr[0] = dr[0] + "<img border=\"0\" title=\"" + entryList[i].Title + "\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/bricks.png" + "\"></img>&nbsp;<span title=\"" + entryList[i].Title + "\">" + entryList[i].Title + "</span>";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
            {

                dr[0] = dr[0] + "<img border=\"0\" title=\"" + entryList[i].Title + "\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/box.png" + "\"></img>&nbsp;<span title=\"" + entryList[i].Title + "\">" + entryList[i].Title + "</span>";
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
            {

                dr[0] = dr[0] + "<img border=\"0\" title=\"" + entryList[i].Title + "\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/package.png" + "\"></img>&nbsp;<span title=\"" + entryList[i].Title + "\">" + entryList[i].Title + "</span>";
            } // Product
            else
            {

                dr[0] = dr[0] + "<img border=\"0\" title=\"" + entryList[i].Title + "\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/brick.png" + "\"></img>&nbsp;<span title=\"" + entryList[i].Title + "\">" + entryList[i].Title + "</span>";
            }
            dr[0] = dr[0] + "</a>";
            //dr(0) = dr(0) + "</p>"
            //dr(0) = dr(0) + "</div>"

            dr[1] = "<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(entryList[i].LanguageId)) + "\' border=\"0\" />";
            dr[2] = entryList[i].Id;
            dr[3] = m_refStyle.StatusWithToolTip((string)(entryList[i].Status));
            dr[4] = entryList[i].EntryType.ToString();
            dr[5] = Strings.FormatNumber(entryList[i].ListPrice, 2, Microsoft.VisualBasic.TriState.UseDefault, Microsoft.VisualBasic.TriState.UseDefault, Microsoft.VisualBasic.TriState.UseDefault);
            dt.Rows.Add(dr);
        }

        DataView dv = new DataView(dt);
        FolderDataGrid.DataSource = dv;
        FolderDataGrid.DataBind();
    }
    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        Display_ViewAll();
    }
    private void populateFolder(System.Collections.Generic.List<CatalogData> catalogData)
    {
        ltr_folder.Text = null;
        int i = 0;
        ContentAPI m_refContentApi = new ContentAPI();
        string imageFolder = m_refContentApi.AppPath + "images/ui/icons/tree/folderCollapsed.png";
        CatalogEntryApi CatalogManager = new CatalogEntryApi();
        System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();


        for (i = 0; i <= catalogData.Count - 1; i++)
        {

            //entryList = CatalogManager.GetCatalogEntries(catalogData.Item(i).Id, m_refContentApi.RequestInformationRef.ContentLanguage, New EntryTypeFilter(GetProducts, GetBundles, GetComplexProducts, GetKits))
            //If entryList.Count = 0 Then
            imageFolder = m_refContentApi.AppPath + "images/ui/icons/folderGreen.png";
            //End If
            if (m_sPageAction == "coupon")
            {
                ltr_folder.Text += "<table><tr><td><a href=\"itemselection.aspx?action=" + m_sPageAction + "select&id=" + catalogData[i].Id + "\"><img style=\"border:0px;\" src=\"" + imageFolder + "\"/></a></td>";
            }
            else
            {
                ltr_folder.Text += "<table><tr><td><a href=\"byproduct.aspx?action=" + m_sPageAction + "select&id=" + catalogData[i].Id + "\"><img style=\"border:0px;\" src=\"" + imageFolder + "\"/></a></td>";
            }
            if (m_sPageAction == "coupon")
            {
                ltr_folder.Text += "<td><a href=\"itemselection.aspx?action=" + m_sPageAction + "select&id=" + catalogData[i].Id + "\">" + catalogData[i].Name + "</a></td></tr></table>";
            }
            else
            {
                ltr_folder.Text += "<td><a href=\"byproduct.aspx?action=" + m_sPageAction + "select&id=" + catalogData[i].Id + "\">" + catalogData[i].Name + "</a></td></tr></table>";
            }
        }
    }
}


