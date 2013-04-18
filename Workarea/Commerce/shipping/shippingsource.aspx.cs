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
using Ektron;
using Ektron.Cms.Workarea;
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

public partial class Commerce_shipping_shippingsource : workareabase
{


    protected Criteria<CountryProperty> criteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected Criteria<RegionProperty> RegionCriteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected RegionApi m_refRegion = null;
    protected CountryApi m_refCountry = null;
    protected WarehouseApi m_refWarehouse = null;
    protected string m_sPageName = "shippingsource.aspx";
    protected int _currentPageNumber = 1;
    protected bool m_bIsDefault = false;
    protected int TotalPagesNumber = 1;
    protected System.Collections.Generic.List<CountryData> CountryList = new System.Collections.Generic.List<CountryData>();
    protected System.Collections.Generic.List<RegionData> RegionList = new System.Collections.Generic.List<RegionData>();

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        this.CommerceLibrary.CheckCommerceAdminAccess();
        Util_RegisterResources();
        m_refRegion = new RegionApi();
        m_refCountry = new CountryApi();
        criteria.PagingInfo = new PagingInfo(10000);
        RegionCriteria.PagingInfo = new PagingInfo(10000);
        CountryList = m_refCountry.GetList(criteria);
        RegionList = m_refRegion.GetList(RegionCriteria);

        try
        {
            switch (base.m_sPageAction)
            {
                case "markdef":
                    Process_MarkDefault();
                    break;
                case "del":
                    Process_Delete();
                    break;
                case "addedit":
                    if (Page.IsPostBack && smAddressCountry.IsInAsyncPostBack == false)
                    {
                        Process_AddEdit();
                    }
                    else if (smAddressCountry.IsInAsyncPostBack == true)
                    {
                        Util_BindRegions((string)drp_address_country.SelectedValue);
                    }
                    else
                    {
                        Display_AddEdit();
                    }
                    break;
                case "view":
                    Display_View();
                    break;
                default: // "viewall"
                    if (Page.IsPostBack == false)
                    {
                        Display_View_All();
                    }
                    break;
            }
            SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    protected void Util_Bind()
    {
        if ((CountryList != null) && CountryList.Count > 0)
        {
            drp_address_country.DataSource = CountryList;
            drp_address_country.DataTextField = "Name";
            drp_address_country.DataValueField = "Id";
            drp_address_country.DataBind();
        }
        if (m_sPageAction == "addedit")
        {
            if (m_iID > 0)
            {
                WarehouseData wareHouse = null;
                m_refWarehouse = new WarehouseApi();
                wareHouse = m_refWarehouse.GetItem(this.m_iID);

                int cCountryId = wareHouse.Address.Country.Id;

                RegionCriteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId);
                RegionList = m_refRegion.GetList(RegionCriteria);
                if ((RegionList != null) && RegionList.Count > 0)
                {
                    drp_address_region.DataSource = RegionList;
                    drp_address_region.DataTextField = "Name";
                    drp_address_region.DataValueField = "Id";
                    drp_address_region.DataBind();
                }
            }
            else
            {
                int cCountryId = System.Convert.ToInt32(drp_address_country.SelectedIndex);
                if (cCountryId == 0)
                {
                    cCountryId = System.Convert.ToInt32(CountryList[0].Id); //The first country in the contryList.
                }
                RegionCriteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId);
                RegionList = m_refRegion.GetList(RegionCriteria);
                if ((RegionList != null) && RegionList.Count > 0)
                {
                    drp_address_region.DataSource = RegionList;
                    drp_address_region.DataTextField = "Name";
                    drp_address_region.DataValueField = "Id";
                    drp_address_region.DataBind();
                }
            }
        }
        else
        {
            RegionList = m_refRegion.GetList(RegionCriteria);
            if ((RegionList != null) && RegionList.Count > 0)
            {
                drp_address_region.DataSource = RegionList;
                drp_address_region.DataTextField = "Name";
                drp_address_region.DataValueField = "Id";
                drp_address_region.DataBind();
            }
        }
    }
    #region Display
    protected void Display_View()
    {
        WarehouseData wareHouse = null;
        m_refWarehouse = new WarehouseApi();
        wareHouse = m_refWarehouse.GetItem(this.m_iID);
        Util_Bind();

        txt_address_name.Text = wareHouse.Name;
        lbl_address_id.Text = wareHouse.Id.ToString();
        txt_address_line1.Text = wareHouse.Address.AddressLine1;
        txt_address_line2.Text = wareHouse.Address.AddressLine2;
        txt_address_city.Text = wareHouse.Address.City;
        drp_address_region.SelectedIndex = Util_GetRegionIndex(Convert.ToInt32(wareHouse.Address.Region.Id));
        txt_address_postal.Text = wareHouse.Address.PostalCode;
        drp_address_country.SelectedIndex = Util_GetCountryIndex(wareHouse.Address.Country.Id);
        chk_default_warehouse.Checked = wareHouse.IsDefaultWarehouse;

        txt_address_name.Enabled = false;
        txt_address_line1.Enabled = false;
        txt_address_line2.Enabled = false;
        txt_address_city.Enabled = false;
        drp_address_region.Enabled = false;
        txt_address_postal.Enabled = false;
        drp_address_country.Enabled = false;
        chk_default_warehouse.Enabled = false;

        m_bIsDefault = wareHouse.IsDefaultWarehouse;

        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;
        FirstPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;
    }

    protected void Display_AddEdit()
    {
        WarehouseData wareHouse = null;
        m_refWarehouse = new WarehouseApi();
        wareHouse = m_refWarehouse.GetItem(this.m_iID);
        Util_Bind();

        if (this.m_iID > 0)
        {
            txt_address_name.Text = wareHouse.Name;
            lbl_address_id.Text = wareHouse.Id.ToString();
            txt_address_line1.Text = wareHouse.Address.AddressLine1;
            txt_address_line2.Text = wareHouse.Address.AddressLine2;
            txt_address_city.Text = wareHouse.Address.City;
            drp_address_region.SelectedIndex = Util_GetRegionIndex(Convert.ToInt32(wareHouse.Address.Region.Id));
            txt_address_postal.Text = wareHouse.Address.PostalCode;
            drp_address_country.SelectedIndex = Util_GetCountryIndex(wareHouse.Address.Country.Id);
            chk_default_warehouse.Checked = wareHouse.IsDefaultWarehouse;

            txt_address_name.Enabled = true;
            txt_address_line1.Enabled = true;
            txt_address_line2.Enabled = true;
            txt_address_city.Enabled = true;
            drp_address_region.Enabled = true;
            txt_address_postal.Enabled = true;
            drp_address_country.Enabled = true;
            chk_default_warehouse.Enabled = false;

            m_bIsDefault = wareHouse.IsDefaultWarehouse;
        }
        else
        {
            phAddressID.Visible = false;
            ltr_address_id.Visible = false;
            lbl_colon.Visible = false;
        }

        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;
        FirstPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;
    }

    protected void Display_View_All()
    {
        Ektron.Cms.Common.Criteria<WarehouseProperty> warehouseCriteria = new Ektron.Cms.Common.Criteria<WarehouseProperty>(WarehouseProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<WarehouseData> WarehouseList = new System.Collections.Generic.List<WarehouseData>();
        int i = 0;

        m_refWarehouse = new WarehouseApi();

        dg_warehouse.AutoGenerateColumns = false;
        dg_warehouse.Columns.Clear();

        warehouseCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        warehouseCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        WarehouseList = m_refWarehouse.GetList(warehouseCriteria);

        TotalPagesNumber = System.Convert.ToInt32(warehouseCriteria.PagingInfo.TotalPages);

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
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
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

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Radio";
        colBound.HeaderText = "";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

        dg_warehouse.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Name";
        colBound.HeaderText = this.GetMessage("generic name");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

        dg_warehouse.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = this.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

        dg_warehouse.Columns.Add(colBound);
        dg_warehouse.BorderColor = System.Drawing.Color.White;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "IsDefaultWarehouse";
        colBound.HeaderText = this.GetMessage("lbl default");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

        dg_warehouse.Columns.Add(colBound);
        dg_warehouse.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Radio", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("IsDefaultWarehouse", typeof(string)));

        if (!(WarehouseList == null))
        {
            for (i = 0; i <= WarehouseList.Count - 1; i++)
            {
                m_bIsDefault = System.Convert.ToBoolean(WarehouseList[i].IsDefaultWarehouse);
                dr = dt.NewRow();
                dr[0] = "<input type=\"radio\" id=\"radio_warehouse\" name=\"radio_warehouse\" value=\"" + WarehouseList[i].Id + "\" />";
                dr[1] = "<a href=\"shippingsource.aspx?action=View&id=" + WarehouseList[i].Id + "\">" + WarehouseList[i].Name + "</a>";
                dr[2] = "<label id=\"lbl_warehouseId\">" + WarehouseList[i].Id + "</label>";
                dr[3] = "<input type=\"CheckBox\" id=\"chk_default" + i + "\" name=\"chk_default" + i + "\" disabled=\"true\" " + (m_bIsDefault ? "Checked=\"checked\"" : "") + "/>";
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        dg_warehouse.DataSource = dv;
        dg_warehouse.DataBind();
    }
    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        //If hdnCurrentPage.Value <> "" Then
        //    _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        //End If
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
        Display_View_All();
        isPostData.Value = "true";
    }
    #endregion

    #region Process
    protected void Process_MarkDefault()
    {
        m_refWarehouse = new WarehouseApi();
        m_refWarehouse.MarkAsDefault(this.m_iID);
        Response.Redirect(this.m_sPageName, false);
    }

    protected void Process_Delete()
    {
        m_refWarehouse = new WarehouseApi();
        if (this.m_iID > 0 && !m_refWarehouse.GetItem(this.m_iID).IsDefaultWarehouse)
        {
            m_refWarehouse.Delete(this.m_iID);
            Response.Redirect(this.m_sPageName, false);
        }
        else
        {
            Response.Redirect(this.m_sPageName + "?action=view&id=" + this.m_iID.ToString(), false);
        }
    }

    protected void Process_AddEdit()
    {
        WarehouseData wareHouse = null;
        m_refWarehouse = new WarehouseApi();

        if (this.m_iID > 0)
        {
            wareHouse = m_refWarehouse.GetItem(this.m_iID);
        }

        RegionData rData;
        rData = new RegionData();
        rData = m_refRegion.GetItem(Convert.ToInt64(drp_address_region.SelectedValue));

        CountryData cData;
        cData = new CountryData();
        cData = m_refCountry.GetItem(System.Convert.ToInt32(drp_address_country.SelectedValue));


        if (this.m_iID == 0)
        {
            wareHouse = new WarehouseData(txt_address_name.Text, new AddressData());
        }

        wareHouse.Name = (string)txt_address_name.Text;

        if (this.m_iID > 0)
        {
            wareHouse.Id = Convert.ToInt64(lbl_address_id.Text);
        }

        wareHouse.Address.AddressLine1 = (string)txt_address_line1.Text;
        wareHouse.Address.AddressLine2 = (string)txt_address_line2.Text;
        wareHouse.Address.City = (string)txt_address_city.Text;
        if (wareHouse.Address.Region == null)
        {
            wareHouse.Address.Region = new RegionData();
        }
        wareHouse.Address.Region.Id = Convert.ToInt64(drp_address_region.SelectedValue);
        wareHouse.Address.PostalCode = (string)txt_address_postal.Text;
        if (wareHouse.Address.Country == null)
        {
            wareHouse.Address.Country = new CountryData();
        }
        wareHouse.Address.Country.Id = System.Convert.ToInt32(drp_address_country.SelectedValue);
        wareHouse.IsDefaultWarehouse = System.Convert.ToBoolean(chk_default_warehouse.Checked);

        if (this.m_iID > 0)
        {
            m_refWarehouse.Update(wareHouse);
            Response.Redirect(m_sPageName + "?action=view&id=" + this.m_iID.ToString(), false);
        }
        else
        {
            m_refWarehouse.Add(wareHouse);
            Response.Redirect(m_sPageName, false);
        }
    }
    #endregion

    protected void SetLabels()
    {
        this.ltr_address_name.Text = this.GetMessage("lbl address name");
        this.ltr_address_id.Text = this.GetMessage("generic id");
        this.ltr_address_line1.Text = this.GetMessage("lbl address line1");
        this.ltr_address_line2.Text = this.GetMessage("lbl address line2");
        this.ltr_address_city_lbl.Text = this.GetMessage("lbl address city");
        this.ltr_address_region.Text = this.GetMessage("lbl state province");
        this.ltr_address_postal.Text = this.GetMessage("lbl address postal");
        this.ltr_address_country.Text = this.GetMessage("lbl address country");
        this.ltr_default_warehouse.Text = this.GetMessage("lbl default warehouse");

        switch (base.m_sPageAction)
        {

            case "addedit":
                this.pnl_viewaddress.Visible = true;
                this.AddBackButton(this.m_sPageName + (this.m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + this.m_iID.ToString(), "lbl alt save warehouse", "btn save", " onclick=\" resetCPostback(); return SubmitForm(); \" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    this.SetTitleBarToMessage("lbl edit warehouse");
                    this.AddHelpButton("Editwarehouse");
                }
                else
                {
                    this.SetTitleBarToMessage("lbl add warehouse");
                    this.AddHelpButton("Addwarehouse");
                }
                break;

            case "view":
                this.pnl_viewall.Visible = false;
                this.pnl_viewaddress.Visible = true;
                this.AddBackButton(this.m_sPageName);
				this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/contentEdit.png", this.m_sPageName + "?action=addedit&id=" + this.m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                if (!m_bIsDefault)
                {

					this.AddButtonwithMessages(this.AppImgPath + "icon_survey_enable.gif", this.m_sPageName + "?action=markdef&id=" + this.m_iID.ToString(), "lbl warehouse mark def", "lbl warehouse mark def", "", StyleHelper.EnableButtonCssClass);
					this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/delete.png", this.m_sPageName + "?action=del&id=" + this.m_iID.ToString(), "alt del warehouse button text", "btn delete", " onclick=\" return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);

                }
                this.SetTitleBarToMessage("lbl view warehouse");
                this.AddHelpButton("Viewwarehouse");
                break;
            default: // "viewall"
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl new"), this.AppImgPath + "../UI/Icons/star.png");
                newMenu.AddLinkItem(this.AppImgPath + "menu/card.gif", this.GetMessage("lbl warehouse"), this.m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);
                workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                actionMenu.AddItem(this.AppImgPath + "icon_survey_enable.gif", this.GetMessage("lbl warehouse mark def"), "CheckMarkAsDef();");
                this.AddMenu(actionMenu);

                this.SetTitleBarToMessage("lbl warehouses");
                this.AddHelpButton("warehouses");
                break;
        }

        SetJs();
    }

    private void SetJs()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);

        sbJS.Append("function CheckDelete()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("if (document.forms[0].chk_default_warehouse.checked == true)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("   alert(\'").Append(GetMessage("lbl delete err default warehouse")).Append("\');" + Environment.NewLine);
        sbJS.Append("   return true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("    return confirm(\'").Append(GetMessage("js warehouse confirm del")).Append("\');" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var objtitle = document.getElementById(\"").Append(txt_address_name.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("    var rRegion = document.getElementById(\"").Append(drp_address_region.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("    var postalCode=document.getElementById(\"").Append(txt_address_postal.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("    var isValidZip=/^\\d{5}$/ ;" + Environment.NewLine);
        sbJS.Append("    if (postalCode.value.search(isValidZip)==-1 && document.getElementById(\'drp_address_country\').value == \'840\')").Append(Environment.NewLine);
        sbJS.Append("    {").Append(Environment.NewLine);
        sbJS.Append("        alert(\"" + base.GetMessage("js postal code validation") + "\");" + Environment.NewLine);
        sbJS.Append("        postalCode.focus();return false;" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    if(rRegion != null){if(rRegion.selectedIndex == -1)").Append(Environment.NewLine);
        sbJS.Append("    {").Append(Environment.NewLine);
        sbJS.Append("       alert(\"" + base.GetMessage("js null warehouse region msg") + "\");" + Environment.NewLine);
        sbJS.Append("       document.forms[\"form1\"].isCPostData.value = \'false\';").Append(Environment.NewLine);
        sbJS.Append("       return false;").Append(Environment.NewLine);
        sbJS.Append("    }}").Append(Environment.NewLine);
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("	    if (!CheckForillegalChar(objtitle.value)) {" + Environment.NewLine);
        sbJS.Append("           objtitle.focus();" + Environment.NewLine);
        sbJS.Append("       } else {" + Environment.NewLine);
        sbJS.Append("           document.forms[\"form1\"].isCPostData.value = \'\';").Append(Environment.NewLine);
        sbJS.Append("           document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("	    }" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    else" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("        alert(\"" + base.GetMessage("js null warehouse msg") + "\");" + Environment.NewLine);
        sbJS.Append("        objtitle.focus();" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    return false;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckForillegalChar(txtName) {" + Environment.NewLine);
        sbJS.Append("   var val = txtName;" + Environment.NewLine);
        sbJS.Append("   if ((val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbJS.Append("   {" + Environment.NewLine);
        sbJS.Append("       alert(\"").Append(string.Format(GetMessage("js alert warehouse name cant include"), "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')")).Append("\");" + Environment.NewLine);
        sbJS.Append("       return false;" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("   return true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckMarkAsDef() {" + Environment.NewLine);
        sbJS.Append(" 	var chosen = \'\'; ").Append(Environment.NewLine);
        sbJS.Append("   if(document.forms[0].radio_warehouse == undefined)").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append(" 		alert(\'").Append(GetMessage("js err no warehouse")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append("       return false;").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append(" 	    var len = document.forms[0].radio_warehouse.length; ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (len > 0) { ").Append(Environment.NewLine);
        sbJS.Append(" 	        for (i = 0; i < len; i++) { ").Append(Environment.NewLine);
        sbJS.Append(" 		        if (document.forms[0].radio_warehouse[i].checked) { ").Append(Environment.NewLine);
        sbJS.Append(" 			        chosen = document.forms[0].radio_warehouse[i].value; ").Append(Environment.NewLine);
        sbJS.Append(" 		        } ").Append(Environment.NewLine);
        sbJS.Append(" 	        } ").Append(Environment.NewLine);
        sbJS.Append(" 	    } else { ").Append(Environment.NewLine);
        sbJS.Append(" 	        if (document.forms[0].radio_warehouse.checked) { chosen = document.forms[0].radio_warehouse.value; } ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (chosen == \'\') { ").Append(Environment.NewLine);
        sbJS.Append(" 		    alert(\'").Append(GetMessage("js please choose warehouse")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append(" 	    } else if (confirm(\'").Append(GetMessage("js warehouse mark def")).Append("\')) { ").Append(Environment.NewLine);
        sbJS.Append(" 		    window.location.href = \'shippingsource.aspx?action=markdef&id=\' + chosen; ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	} ").Append(Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append(JSLibrary.ToggleDiv());

        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

    public string ProtectPassword(string pwd)
    {
        return "**********";
    }
    protected int Util_GetCountryIndex(int countryId)
    {
        int iRet = -1;
        if ((CountryList != null) && CountryList.Count > 0)
        {
            for (int i = 0; i <= (CountryList.Count - 1); i++)
            {
                if (CountryList[i].Id == countryId)
                {
                    iRet = i;
                }
            }
        }
        return iRet;
    }
    protected string Util_GetCountryName(int countryId)
    {
        string sRet = "";
        if ((CountryList != null) && CountryList.Count > 0)
        {
            for (int i = 0; i <= (CountryList.Count - 1); i++)
            {
                if (CountryList[i].Id == countryId)
                {
                    sRet = (string)(CountryList[i].Name);
                }
            }
        }
        return sRet;
    }
    protected int Util_GetRegionIndex(int regionId)
    {
        int iRet = -1;
        if ((RegionList != null) && RegionList.Count > 0)
        {
            for (int i = 0; i <= (RegionList.Count - 1); i++)
            {
                if (RegionList[i].Id == regionId)
                {
                    iRet = i;
                }
            }
        }
        return iRet;
    }
    protected string Util_GetRegionName(int regionId)
    {
        string sRet = "";
        if ((RegionList != null) && RegionList.Count > 0)
        {
            for (int i = 0; i <= (RegionList.Count - 1); i++)
            {
                if (RegionList[i].Id == regionId)
                {
                    sRet = (string)(RegionList[i].Name);
                }
            }
        }
        return sRet;
    }

    protected void Util_BindRegions(string countryId)
    {
        int cCountryId = System.Convert.ToInt32(drp_address_country.SelectedValue);
        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);

        drp_address_region.DataSource = "";
        criteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId);
        criteria.PagingInfo.RecordsPerPage = 10000;

        RegionList = m_refRegion.GetList(criteria);
        if ((RegionList != null) && RegionList.Count > 0)
        {
            drp_address_region.DataSource = RegionList;
            drp_address_region.DataTextField = "Name";
            drp_address_region.DataValueField = "Id";
            drp_address_region.DataBind();
        }
        else
        {

            drp_address_region.DataSource = "";
            drp_address_region.DataTextField = "";
            drp_address_region.DataValueField = "";

            drp_address_region.DataBind();
        }
    }
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
    }
}


