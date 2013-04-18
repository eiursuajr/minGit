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
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Commerce;

public partial class Commerce_tax_postaltaxtables : workareabase
{
    #region Member Variables

    protected RegionApi _RegionApi;
    protected TaxApi _TaxApi;
    protected CountryApi _CountryApi;
    protected TaxClassApi _TaxClassApi = new Ektron.Cms.Commerce.TaxClassApi();
    protected string _PageName = "postaltaxtables.aspx";
    protected System.Collections.Generic.List<CountryData> _CountryList = new System.Collections.Generic.List<CountryData>();
    protected System.Collections.Generic.List<RegionData> _RegionList = new System.Collections.Generic.List<RegionData>();
    protected Criteria<CountryProperty> _Criteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected int _CurrentPageNumber = 1;
    protected int _TotalPagesNumber = 1;
    protected string AppPath = "";

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterJS();
        this.RegisterCSS();
        PageLabel.Text = PageLabel.ToolTip = GetMessage("lbl pagecontrol page");
        OfLabel.Text = OfLabel.ToolTip = GetMessage("lbl pagecontrol of");

        FirstPage.ToolTip = GetMessage("lbl first page");
        lnkBtnPreviousPage.ToolTip = GetMessage("lbl previous page");
        NextPage.ToolTip = GetMessage("lbl next page");
        LastPage.ToolTip = GetMessage("lbl last page");

        FirstPage.Text = "[" + GetMessage("lbl first page") + "]";
        lnkBtnPreviousPage.Text = "[" + GetMessage("lbl previous page") + "]";
        NextPage.Text = "[" + GetMessage("lbl next page") + "]";
        LastPage.Text = "[" + GetMessage("lbl last page") + "]";
    }

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
        Util_CheckAccess();
        Util_RegisterResources();
        _RegionApi = new RegionApi(); //(Me.m_refContentApi.RequestInformationRef)
        _CountryApi = new CountryApi(); //(Me.m_refContentApi.RequestInformationRef)
        _Criteria.PagingInfo = new PagingInfo(10000);
        _Criteria.AddFilter(CountryProperty.IsEnabled, CriteriaFilterOperator.EqualTo, true);
        AppPath = m_refContentApi.AppPath;

        switch (this.m_sPageAction)
        {
            case "addedit":
                _CountryList = _CountryApi.GetList(_Criteria);
                if (Page.IsPostBack && smUpdateRegion.IsInAsyncPostBack == false)
                {
                    if (Request.Form[isCPostData.UniqueID] == "")
                    {
                        Process_AddEdit();
                    }
                }
                else
                {
                    if (smUpdateRegion.IsInAsyncPostBack == true)
                    {
                        UpdateRegions();
                    }
                    else
                    {
                        Display_AddEdit();
                    }
                }
                break;
            case "del":
                Process_Delete();
                break;
            case "view":
                _CountryList = _CountryApi.GetList(_Criteria);
                Display_View();
                break;
            default:
                _CountryList = _CountryApi.GetList(_Criteria);
                if (Page.IsPostBack == false)
                {
                    Display_All();
                }
                break;
        }
        Util_SetLabels();
        Util_SetJS();
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        //If hdnCurrentPage.Value <> "" Then
        //    _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        //End If
        switch (e.CommandName)
        {
            case "First":
                _CurrentPageNumber = 1;
                break;
            case "Last":
                _CurrentPageNumber = int.Parse((string)TotalPages.Text);
                break;
            case "Next":
                _CurrentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) + 1);
                break;
            case "Prev":
                _CurrentPageNumber = System.Convert.ToInt32(int.Parse((string)CurrentPage.Text) - 1);
                break;
        }
        Display_All();
        isPostData.Value = "true";
    }

    #endregion

    #region Process

    protected void Process_AddEdit()
    {

        TaxRateData tTax = null;
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        int page_data = _CurrentPageNumber;
        Ektron.Cms.Common.Criteria<TaxRateProperty> postalCriteria = new Ektron.Cms.Common.Criteria<TaxRateProperty>(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending);
        string postalCode = "0";
        long id = 0;
        TaxApi taxApi = new TaxApi();

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        if (Request.QueryString["postalid"] != "")
        {
            postalCode = Request.QueryString["postalid"];
        }

        if (Request.QueryString["id"] != "")
        {
            id = Convert.ToInt64(Request.QueryString["id"]);
        }
        postalCriteria.PagingInfo.CurrentPage = page_data;
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax);

        List<TaxRateData> postalRateList;
        postalRateList = taxApi.GetList(postalCriteria);

        if (this.m_iID > 0 && Page.IsPostBack)
        {
            try
            {
                for (int i = 0; i <= TaxClassList.Count - 1; i++)
                {
                    PostalCodeTaxRateData postalCodeData = new PostalCodeTaxRateData();
                    tTax = taxApi.GetItemByPostalId(TaxClassList[i].Id, id);

                    if (tTax == null)
                    {
                        tTax = new PostalCodeTaxRateData(postalCode, Convert.ToInt64(drp_region.SelectedValue), TaxClassList[i].Id, 0);
                        if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                        {
                            tTax.Rate = System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100);
                            taxApi.Add(tTax);
                        }
                    }
                    else
                    {
                        if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                        {
                            postalCodeData = new PostalCodeTaxRateData(txt_name.Text, Convert.ToInt64(drp_region.SelectedValue), TaxClassList[i].Id, System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100));
                            taxApi.Update(postalCodeData);
                        }
                    }
                }
                Response.Redirect(_PageName + "?action=view&id=" + m_iID.ToString() + "&postalid=" + postalCode, false);
            }
            catch (CmsException exc)
            {
                Utilities.ShowError(EkFunctions.GetAllValidationMessages(exc.ValidationResults));
            }
        }
        else
        {
            try
            {
                PostalCodeTaxRateData postalrate = new PostalCodeTaxRateData();

                for (int i = 0; i <= TaxClassList.Count - 1; i++)
                {
                    if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                    {
                        postalrate = new PostalCodeTaxRateData(txt_name.Text, Convert.ToInt64(drp_region.SelectedValue), TaxClassList[i].Id, System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100));
                        taxApi.Add(postalrate);
                    }
                }
                Response.Redirect(_PageName, false);
            }
            catch (CmsException exc)
            {
                Utilities.ShowError(EkFunctions.GetAllValidationMessages(exc.ValidationResults));
            }
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

    protected void Process_Delete()
    {
        _TaxApi = new TaxApi();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Commerce.CountryTaxRateData m_CountryTax = new Ektron.Cms.Commerce.CountryTaxRateData();
        TaxClasscriteria.PagingInfo.RecordsPerPage = 10;
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        if (this.m_iID > 0)
        {
            for (int j = 0; j <= TaxClassList.Count - 1; j++)
            {
                long tTax = new long();
                tTax = _TaxApi.GetItemByPostalId(TaxClassList[j].Id, m_iID).Id;
                _TaxApi.Delete(tTax);
            }
        }

        Response.Redirect(_PageName, false);
    }

    #endregion

    #region Display

    protected void Display_AddEdit()
    {
        int page_data = _CurrentPageNumber;
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Common.Criteria<TaxRateProperty> postalCriteria = new Ektron.Cms.Common.Criteria<TaxRateProperty>(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending);
        string postalCode = "0";
        long id = 0;
        TaxApi taxApi = new TaxApi();
        RegionData rRegion = new RegionData();
        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        int cCountryId = 0;
        int txtClassList = 0;

        Util_BindCountries();

        if (Request.QueryString["postalid"] != "")
        {
            postalCode = Request.QueryString["postalid"];
        }

        if (Request.QueryString["id"] != "")
        {
            id = Convert.ToInt64(Request.QueryString["id"]);
        }

        postalCriteria.PagingInfo.CurrentPage = page_data;
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax);

        List<TaxRateData> postalRateList;
        postalRateList = taxApi.GetList(postalCriteria);

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        txt_name.Text = postalCode;
        if (txt_name.Text == "0" || 0 == txt_name.Text.Length)
        {
            txt_name.Enabled = true;
        }
        else
        {
            txt_name.Enabled = false;
        }
        lbl_id.Text = id.ToString();

        _RegionList = _RegionApi.GetList(criteria);

        cCountryId = System.Convert.ToInt32(drp_country.SelectedValue);
        Util_BindRegions(cCountryId);

        ltr_txtClass.Text = "<table class=\"ektronGrid\">";
        for (txtClassList = 0; txtClassList <= TaxClassList.Count - 1; txtClassList++)
        {
            PostalCodeTaxRateData postalRegion = new PostalCodeTaxRateData();
            postalRegion = (PostalCodeTaxRateData)taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id);
            if (postalRegion != null)
            {
                criteria.AddFilter(RegionProperty.Id, CriteriaFilterOperator.EqualTo, postalRegion.RegionId);
                _RegionList = _RegionApi.GetList(criteria);
                drp_region.SelectedValue = _RegionList[0].Id.ToString();
                cCountryId = System.Convert.ToInt32(_RegionList[0].CountryId);
                drp_country.SelectedValue = cCountryId.ToString();
                Util_BindRegions(cCountryId);
            }

            ltr_txtClass.Text += "<tr>";
            ltr_txtClass.Text += "   <td class=\"label\">" + TaxClassList[txtClassList].Name + "</td>";
            if (taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id) == null)
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"0\" />%";
                ltr_txtClass.Text += "   </td>";
            }
            else
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"" + taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id).Rate * 100 + "\"/>%";
                ltr_txtClass.Text += "   </td>";
            }
            ltr_txtClass.Text += "</tr>";
        }
        ltr_txtClass.Text += "</table>";

        tr_id.Visible = m_iID > 0;
        pnl_view.Visible = true;
        pnl_viewall.Visible = false;

        if (this.m_iID > 0)
        {
            drp_country.Enabled = false;
            drp_region.Enabled = false;
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

    protected void Display_All()
    {
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        Ektron.Cms.Common.Criteria<TaxRateProperty> taxCriteria = new Ektron.Cms.Common.Criteria<TaxRateProperty>(TaxRateProperty.TaxClassName, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        Ektron.Cms.Common.Criteria<TaxRateProperty> postalCriteria = new Ektron.Cms.Common.Criteria<TaxRateProperty>(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending);
        CountryApi m_refCountryTaxRate = new CountryApi();
        int page_Data = _CurrentPageNumber;
        int i = 0;
        TaxApi taxApi = new TaxApi();
        List<TaxRateData> postalRateList;
        int iCount = 0;
        int k = 0;
        int p = 0;
        int q = 0;
        int r = 0;

        dg_viewall.AutoGenerateColumns = false;
        dg_viewall.Columns.Clear();

        _Criteria.PagingInfo.RecordsPerPage = 10;
        taxCriteria.PagingInfo.RecordsPerPage = 10;
        taxCriteria.Filters.Capacity = 1000;

        ///''

        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax);

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        int iCount1 = System.Convert.ToInt32(taxApi.GetList(postalCriteria).Count() / TaxClassList.Count);
        int totalPages = postalCriteria.PagingInfo.TotalPages;

        postalCriteria.PagingInfo.CurrentPage = _CurrentPageNumber;

        postalRateList = taxApi.GetList(postalCriteria);

        iCount = System.Convert.ToInt32(postalRateList.Count / TaxClassList.Count);

        string[] Postal = new string[postalRateList.Count - 1 + 1];
        long[] Region = new long[postalRateList.Count - 1 + 1];

        foreach (PostalCodeTaxRateData PostalRate in postalRateList)
        {
            Postal[k] = PostalRate.PostalCode;
            Region[k] = PostalRate.TypeItemId;
            k++;
        }

        string[] zipcode = new string[iCount + 1];
        long[] regionId = new long[iCount + 1];

        if (Region.Length > 0)
        {
            regionId[p] = Region[r];
        }
        if (Postal.Length > 0)
        {
            zipcode[q] = Postal[p];
        }
        q++;
        r++;

        for (p = 1; p <= postalRateList.Count - 1; p++)
        {
            if (Postal[p] != Postal[p - 1])
            {
                zipcode[q] = Postal[p];
                q++;
            }
        }

        for (p = 1; p <= postalRateList.Count - 1; p++)
        {
            if (Region[p] != Region[p - 1])
            {
                regionId[r] = Region[p];
                r++;
            }
        }
        ///'

        //_TotalPagesNumber = System.Convert.ToInt32(System.Math.Ceiling(Convert.ToDouble(iCount1 / 10)));

        if (totalPages <= 1)
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
            //TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(_TotalPagesNumber))).ToString();

            TotalPages.Text = totalPages.ToString();
            TotalPages.ToolTip = TotalPages.Text;

            CurrentPage.Text = _CurrentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;

            if (_CurrentPageNumber == 1)
            {
                lnkBtnPreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_CurrentPageNumber == _TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }

        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Id";
        colBound.HeaderText = m_refMsg.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Postal Code";
        colBound.HeaderText = m_refMsg.GetMessage("lbl address postal"); // + "(" + m_refMsg.GetMessage("lbl view tax rate for region") + ")";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dg_viewall.Columns.Add(colBound);

        dg_viewall.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("Postal Code", typeof(string)));

        if (!(postalRateList == null))
        {

            for (i = 0; i <= (zipcode.Length - 1); i++)
            {
                if (!string.IsNullOrEmpty(zipcode[i]))
                {
                    dr = dt.NewRow();
                    dr[0] = "<a href=\"postaltaxtables.aspx?action=View&postalid=" + zipcode[i].ToString() + "&id=" + regionId[i] + "\">" + regionId[i] + "</a>";
                    dr[1] = "<a onmouseover=\"expandcontent(\'sc" + i + "\')\" onmouseout=\"expandcontent(\'sc" + i + "\')\" href=\"postaltaxtables.aspx?action=View&postalid=" + zipcode[i].ToString() + "&id=" + regionId[i] + "\">" + zipcode[i] + "</a>";
                    dr[1] += "<div class=\"switchcontent\" style=\"position:absolute;\" id=\"sc" + i + "\">";
                    dr[1] += "<table>";
                    foreach (TaxClassData taxClass in TaxClassList)
                    {
                        dr[1] += "<tr><td width=\"50%\"><label id=\"" + taxClass.Name + "\">" + taxClass.Name + "</label></td>";
                        dr[1] += "<td width=\"20px\"><label id=\"value\">" + GetRate(taxClass.Id, regionId[i]) * 100 + "</label>" + "<label id=\"lblPercentage\">" + "&nbsp;%" + "</label></td></tr>";
                    }
                    dr[1] += "</table></div>";
                    dt.Rows.Add(dr);
                }
            }
        }
        DataView dv = new DataView(dt);

        dg_viewall.DataSource = dv;
        dg_viewall.DataBind();
    }

    protected void Display_View()
    {
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Common.Criteria<TaxRateProperty> postalCriteria = new Ektron.Cms.Common.Criteria<TaxRateProperty>(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending);
        int page_data = _CurrentPageNumber;
        TaxApi taxApi = new TaxApi();
        string postalCode = "0";
        long id = 0;
        List<TaxRateData> postalRateList;
        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        long cCountryId = 0;
        int txtClassList = 0;

        if (Request.QueryString["postalid"] != "")
        {
            postalCode = Request.QueryString["postalid"];
        }

        if (Request.QueryString["id"] != "")
        {
            id = Convert.ToInt64(Request.QueryString["id"]);
        }

        postalCriteria.PagingInfo.CurrentPage = page_data;
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax);

        postalRateList = taxApi.GetList(postalCriteria);
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        txt_name.Text = postalCode;
        lbl_id.Text = id.ToString();
        Util_BindCountries();
        for (txtClassList = 0; txtClassList <= TaxClassList.Count - 1; txtClassList++)
        {
            PostalCodeTaxRateData postalRegion = new PostalCodeTaxRateData();
            postalRegion = (PostalCodeTaxRateData)taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id);
            if (postalRegion != null)
            {
                criteria.AddFilter(RegionProperty.Id, CriteriaFilterOperator.EqualTo, postalRegion.RegionId);
                _RegionList = _RegionApi.GetList(criteria);
                drp_region.SelectedValue = _RegionList[0].Id.ToString();
                cCountryId = _RegionList[0].CountryId;
                drp_country.SelectedValue = cCountryId.ToString();
                Util_BindRegions(Convert.ToInt32(cCountryId));
            }
        }

        ltr_txtClass.Text = "<table class=\"ektronGrid\"><br />";
        ltr_txtClass.Text += "<tr><td class=\"label\"><b><label id=\"lbl_taxRate\">" + m_refMsg.GetMessage("lbl tax rates") + ":</label></b></td></tr>";
        for (txtClassList = 0; txtClassList <= TaxClassList.Count - 1; txtClassList++)
        {
            ltr_txtClass.Text += "<tr>";
            ltr_txtClass.Text += "   <td class=\"label\">";
            ltr_txtClass.Text += "       <label id=\"taxClass" + txtClassList + "\" value=\"" + TaxClassList[txtClassList].Name + "\">" + TaxClassList[txtClassList].Name + ":</label>";
            ltr_txtClass.Text += "   </td>";
            if (taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id) == null)
            {
                ltr_txtClass.Text += "   <td calss=\"value\">";
                ltr_txtClass.Text += "       <input disabled=\"true\" type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"0\"/>";
                ltr_txtClass.Text += "   </td>";
            }
            else
            {
                ltr_txtClass.Text += "   <td calss=\"value\">";
                ltr_txtClass.Text += "      <input disabled=\"true\" type=\"text\" id=\"txtClassRate" + txtClassList + "\" name=\"txtClassRate" + txtClassList + "\" value=\"" + (taxApi.GetItemByPostalId(TaxClassList[txtClassList].Id, id).Rate * 100) + "\"/>%  ";
                ltr_txtClass.Text += "   </td>";
            }
            ltr_txtClass.Text += "</tr>";
        }
        ltr_txtClass.Text += "</table>";

        Util_SetEnabled(false);
        pnl_view.Visible = true;
        pnl_viewall.Visible = false;

        TotalPages.Visible = false;
        CurrentPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;
        FirstPage.Visible = false;
        PageLabel.Visible = false;
        OfLabel.Visible = false;
    }

    #endregion

    #region Helpers

    protected decimal GetRate(long taxClassId, long regionId)
    {
        TaxRateData Rate = new TaxRateData();
        Ektron.Cms.Commerce.TaxApi m_refTaxRate = new Ektron.Cms.Commerce.TaxApi();
        m_refTaxRate = new TaxApi();

        try
        {
            Rate = m_refTaxRate.GetItemByPostalId(taxClassId, regionId);
            return Rate.Rate;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    protected void UpdateRegions()
    {
        Int32 cCountry = Convert.ToInt32(drp_country.SelectedValue);
        Util_BindRegions(cCountry);
    }

    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
				AddBackButton(_PageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString() + "&postalid=" + txt_name.Text) : ""));
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", _PageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\"TaxSubmitForm(); return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit postal tax rate");
                    AddHelpButton("EditPostalCodeTaxRate");
                }
                else
                {
                    SetTitleBarToMessage("lbl add postal tax rate");
                    AddHelpButton("AddPostalCodeTaxRate");
                }
                break;
            case "view":
				AddBackButton(_PageName);
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", _PageName + "?action=addedit&id=" + m_iID.ToString() + "&postalid=" + txt_name.Text, "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
				this.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", _PageName + "?action=del&id=" + m_iID.ToString() + "&postalid=" + txt_name.Text, "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete postal") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                SetTitleBarToMessage("lbl view postal tax rate");
                AddHelpButton("ViewPostalCodeTaxRate");
                break;
            default:
                workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), AppPath + "images/UI/Icons/star.png");
                newMenu.AddLinkItem(AppImgPath + "menu/document.gif", GetMessage("lbl postal"), _PageName + "?action=addedit");
                this.AddMenu(newMenu);
                SetTitleBarToMessage("lbl postal tax table");
                AddHelpButton("PostalCodeTaxRate");
                break;
        }

        ltr_name.Text = GetMessage("lbl address postal");
        ltr_id.Text = GetMessage("generic id");
        ltr_region.Text = GetMessage("lbl address region");
        ltr_country.Text = GetMessage("lbl country");
    }

    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        sbJS.Append("<script type=\'text/javascript\'>").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append(" function validate_Title() {").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var cCountryId = Trim(document.getElementById(\'").Append(drp_country.UniqueID).Append("\').value);").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\' || sTitle == 0) { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err postal code title req")).Append("\'); document.forms[\'form1\'].isCPostData.value = \'false\'; } ").Append(Environment.NewLine);
        sbJS.Append("   if(cCountryId == \'840\' ){").Append(Environment.NewLine);
        sbJS.Append("       if(!ValidatePostalCode(sTitle)&& sTitle != \'\' && sTitle != 0)").Append(Environment.NewLine);
        sbJS.Append("       {").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err postal code title req")).Append("\');  ").Append(Environment.NewLine);
        sbJS.Append("           document.forms[\"form1\"].isCPostData.value = \'false\';").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl region disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("   function ValidatePostalCode(postalCodeText){").Append(Environment.NewLine);
        sbJS.Append("       var regEx = /^\\d{5}(-\\d{4})?$/; ").Append(Environment.NewLine);
        sbJS.Append("       return (regEx.test(postalCodeText));").Append(Environment.NewLine);
        sbJS.Append("    }").Append(Environment.NewLine);

        sbJS.Append("function SubmitForm()").Append(Environment.NewLine);
        sbJS.Append("{").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   var taxClass = ").Append(TaxClassList.Count).Append(";").Append(Environment.NewLine);
        sbJS.Append("   var i = 0;").Append(Environment.NewLine);
        sbJS.Append("   var drp_region = document.getElementById(\"").Append(drp_region.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   if(drp_region.selectedIndex == -1)").Append(Environment.NewLine);
        sbJS.Append("    {").Append(Environment.NewLine);
        sbJS.Append("       alert(\"" + base.GetMessage("js null postalcode region msg") + "\");" + Environment.NewLine);
        sbJS.Append("       document.forms[\"form1\"].isCPostData.value = \'false\';").Append(Environment.NewLine);
        sbJS.Append("       return false;").Append(Environment.NewLine);
        sbJS.Append("    }").Append(Environment.NewLine);
        sbJS.Append("   for (i = 0; i < taxClass; i++)").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       var taxField = Trim(document.getElementById(\'txtClassRate\' + i)); ").Append(Environment.NewLine);
        sbJS.Append("       if(taxField.value == \'\')").Append(Environment.NewLine);
        sbJS.Append("       {").Append(Environment.NewLine);
        sbJS.Append("           taxField.value = 0;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("       if(isNaN(taxField.value) || taxField.value > 99)").Append(Environment.NewLine);
        sbJS.Append("       {").Append(Environment.NewLine);
        sbJS.Append("          ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err postal code tax value")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("           break;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\');").Append(Environment.NewLine);
        sbJS.Append("   return false; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);
        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text += sbJS.ToString();
    }

    protected void Util_SetEnabled(bool toggle)
    {
        this.txt_name.Enabled = toggle;
        //chk_enabled.Enabled = toggle
        drp_country.Enabled = toggle;
        drp_region.Enabled = toggle;
    }

    protected void Util_BindCountries()
    {
        if ((_CountryList != null) && _CountryList.Count > 0)
        {
            drp_country.DataSource = _CountryList;
            drp_country.DataTextField = "Name";
            drp_country.DataValueField = "Id";
            drp_country.DataBind();
        }
    }

    protected void Util_BindRegions(int cCountryId)
    {
        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, cCountryId);
        if (Request.QueryString["postalid"] == null || 0 == Request.QueryString["postalid"].Length)
        {
            criteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, "True");
        }
        criteria.PagingInfo.RecordsPerPage = 10000;

        _RegionList = _RegionApi.GetList(criteria);
        try
        {
            if ((_RegionList != null) && _RegionList.Count > 0)
            {
                drp_region.DataSource = _RegionList;
                drp_region.DataTextField = "Name";
                drp_region.DataValueField = "Id";
                drp_region.DataBind();
            }
            else
            {
                drp_region.DataSource = "";
                drp_region.DataTextField = "Name";
                drp_region.DataValueField = "Id";
                drp_region.DataBind();
            }
        }
        catch (Exception)
        {

        }
    }

    protected int Util_GetCountryIndex(int countryId)
    {
        int iRet = -1;
        if ((_CountryList != null) && _CountryList.Count > 0)
        {
            for (int i = 0; i <= (_CountryList.Count - 1); i++)
            {
                if (_CountryList[i].Id == countryId)
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
        if ((_CountryList != null) && _CountryList.Count > 0)
        {
            for (int i = 0; i <= (_CountryList.Count - 1); i++)
            {
                if (_CountryList[i].Id == countryId)
                {
                    sRet = (string)(_CountryList[i].Name);
                }
            }
        }
        return sRet;
    }

    protected int Util_GetRegionIndex(int regionId)
    {
        int iRet = -1;
        if ((_RegionList != null) && _RegionList.Count > 0)
        {
            for (int i = 0; i <= (_RegionList.Count - 1); i++)
            {
                if (_RegionList[i].Id == regionId)
                {
                    iRet = i;
                }
            }
        }
        return iRet;
    }

    protected string Util_GetRegionName(int RegionId)
    {
        string sRet = "";
        if ((_RegionList != null) && _RegionList.Count > 0)
        {
            for (int i = 0; i <= (_RegionList.Count - 1); i++)
            {
                if (_RegionList[i].Id == RegionId)
                {
                    sRet = (string)(_RegionList[i].Name);
                }
            }
        }
        return sRet;
    }

    protected void Util_CheckAccess()
    {

        try
        {
            if (!this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
            {
                throw (new Exception(GetMessage("err not role commerce-admin")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }

    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
    }

    #endregion

    #region JS/CSS

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);

    }

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    #endregion

}


