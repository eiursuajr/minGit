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
public partial class Commerce_itemselection : workareabase
{

    protected string m_sPageName = "itemselection.aspx";
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected string strContType;
    protected string ContentTypeUrlParam = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam;
    protected LocalizationAPI objLocalizationApi = null;
    protected PermissionData security_data;
    protected bool GetProducts = true;
    protected bool GetComplexProducts = true;
    protected bool GetKits = true;
    protected bool GetBundles = true;
    protected long excludeId = 0;
    protected bool bTabUseCase = false;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        if ((Request.QueryString["SelectedTab"] != null) && Request.QueryString["SelectedTab"] != "")
        {
            bTabUseCase = true;
        }
        if (!string.IsNullOrEmpty(Request.QueryString["exclude"]))
        {
            excludeId = Convert.ToInt64(Request.QueryString["exclude"]);
        }

        try
        {

            if (!Util_CheckAccess())
            {
                throw (new Exception("No permission"));
            }
            if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), false);
                return;
            }

            if (Request.QueryString[ContentTypeUrlParam] != "")
            {
                if (Information.IsNumeric(Request.QueryString[ContentTypeUrlParam]))
                {
                    strContType = Request.QueryString[ContentTypeUrlParam];
                }
            }
            
			//paging ui
            this.divPaging.Visible = false;
			
            if (Page.IsPostBack == false)
            {
                Display_ViewAll();
            }			
            SetLabels();

        }
        catch (Exception ex)
        {

            Utilities.ShowError(ex.Message);

        }

    }

    #region Display

    protected void Display_ViewAll()
    {
        CatalogEntry CatalogManager = new CatalogEntry(m_refContentApi.RequestInformationRef);
        System.Collections.Generic.List<EntryData> entryList = new System.Collections.Generic.List<EntryData>();
        Ektron.Cms.Common.Criteria<EntryProperty> entryCriteria = new Ektron.Cms.Common.Criteria<EntryProperty>();

        entryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        entryCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_iID);
        entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage);
        entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, false);
        entryCriteria.AddFilter(EntryProperty.IsPublished, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);

        entryCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
        entryCriteria.OrderByField = Util_GetSortColumn();

        switch (m_sPageAction)
        {
            case "browsecrosssell":
            case "browseupsell":
            case "couponselect":

                // If m_sPageAction = "couponselect" Then entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, CatalogEntryType.SubscriptionProduct)
                entryList = CatalogManager.GetList(entryCriteria);
                break;

            case "browse":

                long[] IdList = new long[3];

                IdList[0] = Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product);
                // IdList(1) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct
                entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList);
                if (excludeId > 0)
                {
                    entryCriteria.AddFilter(EntryProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, excludeId);
                }
                entryList = CatalogManager.GetList(entryCriteria);
                break;

            default:

                pnl_catalogs.Visible = true;
                pnl_viewall.Visible = false;
                System.Collections.Generic.List<CatalogData> catalogList = new System.Collections.Generic.List<CatalogData>();
                catalogList = CatalogManager.GetCatalogList(1, 1);
                Util_ShowCatalogs(catalogList);
                break;

        }

        TotalPagesNumber = System.Convert.ToInt32(entryCriteria.PagingInfo.TotalPages);

        if (TotalPagesNumber > 1) { 
            SetPagingUI(); 
        }

        Populate_ViewCatalogGrid(entryList);
    }

    #endregion


    #region Private Helpers

    private void Util_ShowCatalogs(System.Collections.Generic.List<CatalogData> catalogList)
    {

        ContentAPI m_refContentApi = new ContentAPI();

        CatalogGrid.DataSource = catalogList;
        CatalogGrid.DataBind();

        //For i = 0 To CatalogData.Count - 1
        //    ltr_folder.Text += "<table><tr><td><img src=""" & m_refContentApi.AppPath & "Tree/images/xp/catalogplusclosefolder.gif""/></td>"
        //    ltr_folder.Text += "<td><a href=""byproduct.aspx?action=" & m_sPageAction & "select&id=" & CatalogData.Item(i).CatalogId & """>" & CatalogData.Item(i).CatalogName & "</a></td></tr></table>"
        //Next
    }
    protected void SetLabels()
    {
        // Me.SetTitleBarToMessage("lbl entry selection")
        if (Request.QueryString["action"] != null)
        {
            if (Request.QueryString["action"] == "browse" || Request.QueryString["action"] == "browsecrosssell" || Request.QueryString["action"] == "browseupsell")
            {
                this.AddBackButton("javascript:history.go(-1);");
            }
        }
        this.AddHelpButton("itemselection");
    }
    private void Populate_ViewCatalogGrid(System.Collections.Generic.List<EntryData> entryList)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        string strTag;
        string strtag1;

        if (m_sPageAction == "couponselect")
        {
            this.AddBackButton("javascript:history.go(-1);");
        }

        strTag = "<a href=\"itemselection.aspx?LangType=" + ContentLanguage + (bTabUseCase ? "&SelectedTab=Items" : "") + "&action=" + m_sPageAction + "&orderby=";
        strtag1 = "&id=" + m_iID + (strContType != "" ? "&" + ContentTypeUrlParam + "=" + strContType : "") + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">";

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
        colBound.HeaderText = m_refMsg.GetMessage("generic Status");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = strTag + "type" + strtag1 + m_refMsg.GetMessage("generic type") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "PRICE";
        colBound.HeaderText = strTag + "price" + strtag1 + m_refMsg.GetMessage("lbl calatog entry price") + "</a>";
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        FolderDataGrid.Columns.Add(colBound);

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
            dr[0] = dr[0] + " title=\"View " + entryList[i].Title + "\"";
            dr[0] = dr[0] + ">";
            if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {

                dr[0] = dr[0] + "<img border=\"0\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/bookGreen.png" + "\"></img>" + entryList[i].Title;
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct)
            {

                dr[0] = dr[0] + "<img border=\"0\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/bricks.png" + "\"></img>" + entryList[i].Title;
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
            {

                dr[0] = dr[0] + "<img border=\"0\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/box.png" + "\"></img>" + entryList[i].Title;
            }
            else if (entryList[i].EntryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
            {

                dr[0] = dr[0] + "<img border=\"0\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/package.png" + "\"></img>" + entryList[i].Title;
            } // Product
            else
            {

                dr[0] = dr[0] + "<img border=\"0\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/brick.png" + "\"></img>" + entryList[i].Title;
            }
            dr[0] = dr[0] + "</a>";
            //dr(0) = dr(0) + "</p>"
            //dr(0) = dr(0) + "</div>"

            dr[1] = "<a href=\"Javascript://\" style=\"text-decoration:none;\">" + "<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(System.Convert.ToInt32(entryList[i].LanguageId)) + "\' border=\"0\" />" + "</a>";
            dr[2] = entryList[i].Id;
            dr[3] = entryList[i].Status;
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
                _currentPageNumber = int.Parse((string)litTotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)txtCurrentPage.Text) + 1);
                break;
            case "Prev":
                _currentPageNumber = System.Convert.ToInt32(int.Parse((string)txtCurrentPage.Text) - 1);
                break;
        }
        Display_ViewAll();
    }

    protected void AdHocPaging_Click(object sender, CommandEventArgs e) {
        _currentPageNumber = int.Parse((string)this.txtCurrentPage.Text);

        txtCurrentPage.Text = _currentPageNumber.ToString();
        hdnCurrentPage.Value = _currentPageNumber.ToString();

        Display_ViewAll();
    }

    private void SetPagingUI() {

        //paging ui
        divPaging.Visible = true;

        litPage.Text = "Page";
        txtCurrentPage.Text = _currentPageNumber == 0 ? "1" : (_currentPageNumber.ToString());
        litOf.Text = "of";
        litTotalPages.Text = TotalPagesNumber.ToString();

        hdnCurrentPage.Value = _currentPageNumber == 0 ? "1" : (_currentPageNumber.ToString());

        ibFirstPage.ImageUrl = m_refContentApi.ApplicationPath + "/images/ui/icons/arrowheadFirst.png";
        ibFirstPage.AlternateText = "First Page";
        ibFirstPage.ToolTip = "First Page";

        ibPreviousPage.ImageUrl = m_refContentApi.ApplicationPath + "/images/ui/icons/arrowheadLeft.png";
        ibPreviousPage.AlternateText = "Previous Page";
        ibPreviousPage.ToolTip = "Previous Page";

        ibNextPage.ImageUrl = m_refContentApi.ApplicationPath + "/images/ui/icons/arrowheadRight.png";
        ibNextPage.AlternateText = "Next Page";
        ibNextPage.ToolTip = "Next Page";

        ibLastPage.ImageUrl = m_refContentApi.ApplicationPath + "/images/ui/icons/arrowheadLast.png";
        ibLastPage.AlternateText = "Last Page";
        ibLastPage.ToolTip = "Last Page";

        ibPageGo.ImageUrl = m_refContentApi.ApplicationPath + "/images/ui/icons/forward.png";
        ibPageGo.AlternateText = "Go To Page";
        ibPageGo.ToolTip = "Go To Page";
        ibPageGo.OnClientClick = " return GoToPage(this);";

    }

    protected bool Util_CheckAccess()
    {
        security_data = m_refContentApi.LoadPermissions(this.m_iID, "folder", 0);
        return security_data.IsReadOnly;
    }
    protected string Util_GetLinkJS(EntryData entryList)
    {
        string sRet = "";

        if (bTabUseCase == true)
        {
            if ((string)(Request.QueryString["SelectedTab"].ToLower()) == "items")
            {
                sRet = "parent.Ektron.Commerce.CatalogEntry.Items.DefaultView.Add.addItem(\'" + entryList.Id + "\', \'" + entryList.Title + "\');";
            }
        }
        else
        {
            switch (m_sPageAction)
            {
                case "couponselect":
                    sRet = "parent.addRowToTable(null, " + entryList.Id + ", " + entryList.LanguageId + ", \'" + entryList.Title + "\', " + entryList.EntryType + "); parent.ektb_remove();";
                    break;
                case "coupon":
                    sRet = "";
                    break;
                case "crosssell":
                case "upsell":
                    sRet = "parent.addRowToTable(null, " + entryList.Id + ", \'" + entryList.Title + "\'); parent.ektb_remove();";
                    break;
                default:
                    sRet = "parent.addRowToTable(null, " + entryList.Id + ", \'" + entryList.Title + "\'); parent.ektb_remove();";
                    break;
            }
        }

        return sRet;
    }

    protected EntryProperty Util_GetSortColumn()
    {
        EntryProperty returnValue;

        string sort = "";

        if (!string.IsNullOrEmpty(Request.QueryString["orderby"]))
        {
            sort = (string)(Request.QueryString["orderby"].ToLower());
        }

        switch (sort)
        {
            case "language":
                returnValue = EntryProperty.LanguageId;
                break;
            case "id":
                returnValue = EntryProperty.Id;
                break;
            case "type":
                returnValue = EntryProperty.EntryType;
                break;
            case "price":
                returnValue = EntryProperty.SalePrice;
                break;
            default:
                returnValue = EntryProperty.Title;
                break;
        }

        return returnValue;
    }

    #endregion

}

