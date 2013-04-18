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
using Ektron.Cms.Workarea;
using Ektron.Cms.Commerce;
using Ektron.Cms;
using Ektron.Cms.Common;
/// <summary>
/// Inventory Reporting 
/// </summary>
public partial class Commerce_Inventory : workareabase
{

    #region Membervariables
    
    protected ContentAPI _ContentApi = new ContentAPI();
    protected CatalogEntryApi _CatalogentryApi = new CatalogEntryApi();
    protected InventoryApi _inventoryApi = new InventoryApi();
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected String _sortCriteria = String.Empty; 

    #endregion Membervariables


    #region protected methods
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (!string.IsNullOrEmpty(Page.Request.QueryString["sort"]))
        {
            _sortCriteria = Page.Request.QueryString["sort"];
        }
        try
        {
            switch (this.m_sPageAction)
            {
                case "view":
                    this.Display_View();
                    break;
                case "updateedit":
                    if (Page.IsPostBack)
                    {
                        if (Request.Form[isCPostData.UniqueID] == "")
                        {
                            this.Display_Update();
                        }
                    }
                    else
                    {
                        this.Display_Edit();
                    }
                    break;
                default:
                    if (!Page.IsPostBack)
                    {
                        this.Display_Viewall();
                    }
                    break;
            }
            this.SetLabels();
            this.Util_RegisterResources();
            this.Util_SetJS();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
        
    }
    /// <summary>
    /// DataGrid Item Bound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Datagrid_Itembound(object sender, DataGridItemEventArgs e)
    {
        InventoryData _inventoryData = (InventoryData)e.Item.DataItem;
        Literal producttypeImage = null;
        producttypeImage = e.Item.FindControl("productImage") as Literal;
        if (_inventoryData != null)
        {
            switch (_inventoryData.EntryType)
            {
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product:
                    producttypeImage.Text = "<img alt=\"\" src=\""+ m_refContentApi.AppPath+"images/UI/icons/brick.png\" />";
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct:
                    producttypeImage.Text = "<img alt=\"\" src=\""+m_refContentApi.AppPath+"images/UI/icons/bricks.png\" />";
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
                    producttypeImage.Text = "<img alt=\"\" src=\""+m_refContentApi.AppPath+"images/UI/icons/package.png\" />";
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
                    producttypeImage.Text = "<img alt=\"\" src=\""+m_refContentApi.AppPath+"images/UI/icons/box.png\" />";
                    break;
                case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct:
                    producttypeImage.Text = "<img alt=\"\" src=\"" + m_refContentApi.AppPath + "images/UI/icons/bookGreen.png\" />";
                    break;
            }
        }
    }
    /// <summary>
    /// Setting up the Labels in the Tool bar
    /// </summary>
    protected void SetLabels()
    {
        switch(this.m_sPageAction)
        {
            case"view":
                if (this.m_iID > 0)
                {
                    this.ltr_name_lbl.Text = this.GetMessage("event title");
                    this.ltr_reorderlevel_lbl.Text = this.GetMessage("lbl reorder");
                    this.ltr_unitsinorder_lbl.Text = this.GetMessage("lbl on order");
                    this.ltr_unitsinstock_lbl.Text = this.GetMessage("lbl in stock");
                    this.ltr_disabled_lbl.Text = this.GetMessage("lbl disable inventory");
					AddBackButton("inventory.aspx");
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", "inventory.aspx?action=updateedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                    SetTitleBarToMessage("lbl view inventory");
                }
                break;
            case "updateedit":
                if (this.m_iID > 0)
                {
                    this.ltr_name_lbl.Text = this.GetMessage("event title");
                    this.ltr_reorderlevel_lbl.Text = this.GetMessage("lbl reorder");
                    this.ltr_unitsinorder_lbl.Text = this.GetMessage("lbl on order");
                    this.ltr_unitsinstock_lbl.Text = this.GetMessage("lbl in stock");
                    this.ltr_disabled_lbl.Text = this.GetMessage("lbl disable inventory");
                    if (Page.IsPostBack)
                    {
                        if (Request.Form[isCPostData.UniqueID] == "")
                        {
							AddBackButton("inventory.aspx");
                            AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", "inventory.aspx?action=updateedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                            SetTitleBarToMessage("lbl view inventory");
                        }
                    }
                    else
                    {
						AddBackButton("inventory.aspx" + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
						AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "btn update", "btn update", " onclick=\"CheckInventory(); return false;\" ", StyleHelper.EditButtonCssClass, true);
                        SetTitleBarToMessage("lbl edit inventory");
                    }
                }
                break;
            default:
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl order reporting"), this.AppImgPath + "commerce/catalog_view.gif");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/brick.png", this.GetMessage("lbl report product orders"), "window.location.href=\'inventory.aspx?action=byproduct\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/box.png", this.GetMessage("lbl report Kit"), "window.location.href=\'inventory.aspx?action=bykit\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/package.png", this.GetMessage("lbl report Bundle"), "window.location.href=\'inventory.aspx?action=bybundle\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/bricks.png", this.GetMessage("lbl report Complex Product"), "window.location.href=\'inventory.aspx?action=bycomplexproduct\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/bookGreen.png", this.GetMessage("lbl report Subscription Product"), "window.location.href=\'inventory.aspx?action=bysubscriptionproduct\';");
                newMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/brick.png", this.GetMessage("generic By All"), "window.location.href=\'inventory.aspx?action=byall\'");
                this.AddMenu(newMenu);
                this.SetTitleBarToMessage("lbl inventory");
                break;
        }
        this.AddHelpButton("inventory");
    }
    /// <summary>
    /// Pagination 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        Display_Viewall();
        isPostData.Value = "true";
    }

    #endregion protected Methods

    #region Resources
    /// <summary>
    /// Method to Register the Css and Javascript Files
    /// </summary>
    protected void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");
    }
    /// <summary>
    /// Method to prepare the sort url to the Datagrid
    /// </summary>
    /// <param name="messageText"></param>
    /// <param name="sortingValue"></param>
    /// <returns></returns>
    protected string Util_SortUrl(string messageText, string sortingValue)
    {

        string urlString = "";
        if (sortingValue == _sortCriteria && _sortCriteria.IndexOf("-") == -1)
        {
            sortingValue = sortingValue + "-";
        }
        if (sortingValue == _sortCriteria && _sortCriteria.IndexOf("-") > -1)
        {
            sortingValue = sortingValue.Replace("-", "");
        }
        urlString = "<a href=\"inventory.aspx?action="+this.m_sPageAction+"&sort=" + sortingValue + "\">" + GetMessage(messageText) + "</a>";
        return urlString;

    }
	/// <summary>
    /// Method to get back the Inventory Status.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    protected string Get_InventoryStatus(bool status)
    {
        if (!status)
            return this.GetMessage("enabled");
        return this.GetMessage("disabled");
      
    }
    /// <summary>
    /// Initializing the Javascript Functions
    /// </summary>
    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("function CheckInventory() {").Append(Environment.NewLine);
        sbJS.Append("   var Unitsinstocktext = Trim(document.getElementById(\'").Append(ltr_unitsinstocktext.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var Unitsonordertext = Trim(document.getElementById(\'").Append(ltr_unitsinorder.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var Reorderleveltext = Trim(document.getElementById(\'").Append(ltr_reorderlevel.UniqueID).Append("\').value);").Append(Environment.NewLine);
        sbJS.Append("   if(isNaN(Reorderleveltext) || isNaN(Unitsonordertext) || isNaN(Unitsinstocktext))").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("js not valid inventory")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       return false; ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else if(String(Unitsinstocktext).indexOf('.') != -1 || String(Unitsonordertext).indexOf('.') != -1 || String(Reorderleveltext).indexOf('.') != -1 )").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       alert(\'").Append(GetMessage("js not valid inventory")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       return false; ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       resetCPostback();").Append(Environment.NewLine);
        sbJS.Append("       document.forms[0].submit(); ").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("} ").Append(Environment.NewLine);
        ltr_js.Text = sbJS.ToString();


    }

    #endregion 

    #region Display
    /// <summary>
    /// Method to display the List of Inventory based of the selection.
    /// </summary>
    private void Display_Viewall()
    {
        List<InventoryData> _inventoryList = new System.Collections.Generic.List<InventoryData>();
        Criteria<InventoryProperty> inventoryCriteria = new Criteria<InventoryProperty>();
        switch (this.m_sPageAction)
        {
            case "byproduct":
                inventoryCriteria.AddFilter(InventoryProperty.EntryType, CriteriaFilterOperator.EqualTo, EkEnumeration.CatalogEntryType.Product);
                break;
            case "bykit":
                inventoryCriteria.AddFilter(InventoryProperty.EntryType, CriteriaFilterOperator.EqualTo, EkEnumeration.CatalogEntryType.Kit);
                break;
            case "bycomplexproduct":
                inventoryCriteria.AddFilter(InventoryProperty.EntryType, CriteriaFilterOperator.EqualTo, EkEnumeration.CatalogEntryType.ComplexProduct);
                break;
            case "bybundle":
                inventoryCriteria.AddFilter(InventoryProperty.EntryType, CriteriaFilterOperator.EqualTo, EkEnumeration.CatalogEntryType.Bundle);
                break;
            case "bysubscriptionproduct":
                inventoryCriteria.AddFilter(InventoryProperty.EntryType, CriteriaFilterOperator.EqualTo, EkEnumeration.CatalogEntryType.SubscriptionProduct);
                break;

        }

        if (_sortCriteria.IndexOf("-") > -1)
        {
            inventoryCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending;
        }
        else 
        {
            inventoryCriteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
        }
        switch (_sortCriteria.Replace("-", ""))
        {
            case "name":
                inventoryCriteria.OrderByField = InventoryProperty.EntryTitle;
                break;
            case "unitsinstock":
                inventoryCriteria.OrderByField = InventoryProperty.UnitsInStock;
                break;
            case "reorderlevel":
                inventoryCriteria.OrderByField = InventoryProperty.ReorderLevel;
                break;
            case "unitsonorder":
                inventoryCriteria.OrderByField = InventoryProperty.UnitsOnOrder;
                break; 
            default:
                inventoryCriteria.OrderByField = InventoryProperty.EntryTitle;
                break;
        }
        
        inventoryCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize;
        inventoryCriteria.PagingInfo.CurrentPage = _currentPageNumber;
        _inventoryList = _inventoryApi.GetList(inventoryCriteria);
        
        TotalPagesNumber = System.Convert.ToInt32(inventoryCriteria.PagingInfo.TotalPages);

        if (TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {
            lnkBtnPreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            LastPage.Enabled = true;
            NextPage.Enabled = true;
            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            lnkBtnPreviousPage.Visible = true;
            NextPage.Visible = true;
            LastPage.Visible = true;
            FirstPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;

            TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(TotalPagesNumber))).ToString();
            TotalPages.ToolTip = TotalPages.Text;

            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;

            if (_currentPageNumber == 1)
            {
                lnkBtnPreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
        
        dg_inventorylines.DataSource = _inventoryList;
        dg_inventorylines.DataBind();
    }
    /// <summary>
    /// Method to display the Inidividual View
    /// </summary>
    private void Display_View()
    {
        this.dg_inventoryview.Visible = true;
        InventoryData _inventoryData = _inventoryApi.GetInventory(this.m_iID);
        this.ltr_name.Text = _inventoryData.EntryTitle;
        this.ltr_unitsinorder.Text = _inventoryData.UnitsOnOrder.ToString();
        this.ltr_unitsinstock.Text = _inventoryData.UnitsInStock.ToString();
        this.ltr_reorderlevel.Text = _inventoryData.ReorderLevel.ToString();
        if (_inventoryData.DisableEntryInventoryManagement)
        {
            this.ltr_disabled.Checked = true;
        }
        this.ltr_unitsinorder.Enabled = false;
        this.ltr_reorderlevel.Enabled = false;
        this.ltr_unitsinstocktext.Visible = false;
        this.unitsinstockdrp.Visible = false;
        this.ltr_disabled.Enabled = false;
        
        //Paging UI
        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;
        FirstPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;
    
    }
    /// <summary>
    /// Method to display the Inidividual Edit
    /// </summary>
    private void Display_Edit()
    {
        this.dg_inventoryview.Visible = true;
        InventoryData _inventoryData = _inventoryApi.GetInventory(this.m_iID);
        this.ltr_name.Text = _inventoryData.EntryTitle;
        this.ltr_unitsinorder.Text = _inventoryData.UnitsOnOrder.ToString();
        this.ltr_unitsinstock.Text = _inventoryData.UnitsInStock.ToString();
        this.ltr_reorderlevel.Text = _inventoryData.ReorderLevel.ToString();
        this.ltr_disabled.Checked = _inventoryData.DisableEntryInventoryManagement;
        this.ltr_unitsinorder.Enabled = true;
        this.ltr_reorderlevel.Enabled = true;
        this.ltr_unitsinstocktext.Visible = true;
        this.unitsinstockdrp.Visible = true;
        this.ltr_disabled.Enabled = true;
        
        //Paging UI

        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;
        FirstPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;
    
    }
    /// <summary>
    /// Method to perform the Inidividual Update
    /// </summary>
    private void Display_Update()
    {
        this.dg_inventoryview.Visible = true;
        InventoryData _inventoryData = _inventoryApi.GetInventory(this.m_iID);
        if (!string.IsNullOrEmpty(ltr_unitsinstocktext.Text) && Convert.ToInt32(ltr_unitsinstocktext.Text) > 0)
        {
            if(unitsinstockdrp.SelectedValue == "Add")
            {
              _inventoryData.UnitsInStock = Convert.ToInt32(ltr_unitsinstock.Text) + Convert.ToInt32(ltr_unitsinstocktext.Text);
            }
            else
            {
              _inventoryData.UnitsInStock = Convert.ToInt32(ltr_unitsinstock.Text) - Convert.ToInt32(ltr_unitsinstocktext.Text);
            } 
        }
        if (!string.IsNullOrEmpty(ltr_unitsinorder.Text) && Convert.ToInt32(ltr_unitsinorder.Text) > 0)
        {
            _inventoryData.UnitsOnOrder = Convert.ToInt32(ltr_unitsinorder.Text);
        }
        if (!string.IsNullOrEmpty(ltr_reorderlevel.Text) && Convert.ToInt32(ltr_reorderlevel.Text) > 0)
        {
            _inventoryData.ReorderLevel = Convert.ToInt32(ltr_reorderlevel.Text);
        }
        if (ltr_disabled.Checked)
        {
            if (!_inventoryData.DisableEntryInventoryManagement)
            {
                _CatalogentryApi.SetEntryInventorytoDisable(_inventoryData.EntryId, true);
            }
        }
        else
        {
            if (_inventoryData.DisableEntryInventoryManagement)
            {
                _CatalogentryApi.SetEntryInventorytoDisable(_inventoryData.EntryId, false);
            }
        }
        _inventoryApi.SaveInventory(_inventoryData);
        this.Display_View();
    }

    #endregion 
}


