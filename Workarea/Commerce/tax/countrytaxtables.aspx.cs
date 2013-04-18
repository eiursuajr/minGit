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

public partial class Commerce_tax_countrytaxtables : workareabase
{


    #region Member Variables

    protected RegionApi _RegionApi;
    protected TaxApi _TaxApi;
    protected CountryApi _CountryApi;
    protected TaxClassApi _TaxClassApi = new Ektron.Cms.Commerce.TaxClassApi();
    protected string _PageName = "countrytaxtables.aspx";
    protected System.Collections.Generic.List<CountryData> _CountryList = new System.Collections.Generic.List<CountryData>();
    protected Criteria<CountryProperty> _CountryCriteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected int _CurrentPageNumber = 1;
    protected int _TotalPagesNumber = 1;
    protected string _SearchCriteria = "";
    protected Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults _ValidateResult = new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults();

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
        if (Page.Request.QueryString["search"] != "")
        {
            _SearchCriteria = Page.Request.QueryString["search"];
        }
        _RegionApi = new RegionApi(); //(Me.m_refContentApi.RequestInformationRef)
        _CountryApi = new CountryApi(); //(Me.m_refContentApi.RequestInformationRef)
        _CountryCriteria.PagingInfo = new PagingInfo(10000);

        hdnCurrentPage.Value = CurrentPage.Text;
        switch (this.m_sPageAction)
        {
            case "addedit":
                _CountryList = _CountryApi.GetList(_CountryCriteria);
                if (Page.IsPostBack)
                {
                    Process_AddEdit();
                }
                else
                {
                    Display_AddEdit();
                }
                break;
            case "del":
                Process_Delete();
                break;
            case "view":
                _CountryList = _CountryApi.GetList(_CountryCriteria);
                Display_View();
                break;
            default:
                _CountryCriteria.Condition = LogicalOperation.Or;
                if (_SearchCriteria != "")
                {
                    _CountryCriteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
                }
                if (_SearchCriteria != "")
                {
                    _CountryCriteria.AddFilter(CountryProperty.LongIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
                }
                if (_SearchCriteria != "")
                {
                    _CountryCriteria.AddFilter(CountryProperty.ShortIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
                }
                _CountryList = _CountryApi.GetList(_CountryCriteria);
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
        if (hdnCurrentPage.Value != "")
        {
            _CurrentPageNumber = int.Parse((string)hdnCurrentPage.Value);
        }
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
        CountryData cCountry = null;
        TaxRateData tTax = null;
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Commerce.CountryTaxRateData m_CountryTax = new Ektron.Cms.Commerce.CountryTaxRateData();
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);
        m_CountryTax = new CountryTaxRateData();
        _TaxApi = new TaxApi();

        if (this.m_iID > 0)
        {
            cCountry = _CountryApi.GetItem(Convert.ToInt32(this.m_iID));
            cCountry.Name = (string)txt_name.Text;
            cCountry.LongIsoCode = (string)txt_long.Text;
            cCountry.ShortIsoCode = (string)txt_short.Text;
            cCountry.Enabled = System.Convert.ToBoolean(chk_enabled.Checked);
            _CountryApi.Update(cCountry);

            for (int i = 0; i <= TaxClassList.Count - 1; i++)
            {
                tTax = _TaxApi.GetItemByCountryId(TaxClassList[i].Id, cCountry.Id);
                if (tTax == null)
                {
                    tTax = new CountryTaxRateData(cCountry.Id, TaxClassList[i].Id, 0);
                    if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                    {
                        tTax.Rate = System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100);
                        _TaxApi.Add(tTax);
                    }
                }
                else
                {
                    if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                    {
                        tTax.Rate = System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100);
                        _TaxApi.Update(tTax);
                    }
                }
            }

            Response.Redirect(_PageName + "?action=view&id=" + m_iID.ToString(), false);
        }
        else
        {
            cCountry = new CountryData(Convert.ToInt32(txt_numeric.Text), txt_name.Text, txt_short.Text, txt_long.Text, chk_enabled.Checked);
            _CountryApi.Add(cCountry);

            for (int i = 0; i <= TaxClassList.Count - 1; i++)
            {
                tTax = new CountryTaxRateData(cCountry.Id, TaxClassList[i].Id, 0);
                if (Information.IsNumeric(Request.Form["txtClassRate" + i]))
                {
                    tTax.Rate = System.Convert.ToDecimal(Convert.ToDecimal(Request.Form["txtClassRate" + i]) / 100);
                    _TaxApi.Add(tTax);
                }
            }

            Response.Redirect(_PageName, false);
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
        if (this.m_iID > 0)
        {
            _CountryApi.Delete(Convert.ToInt32(m_iID));
        }
        Response.Redirect(_PageName, false);
    }
    #endregion

    #region Display
    protected void Display_AddEdit()
    {
        CountryData cCountry = new CountryData();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);
        _TaxApi = new TaxApi();

        if (m_iID > 0)
        {
            cCountry = _CountryApi.GetItem(Convert.ToInt32(this.m_iID));
            txt_name.Enabled = false;
            chk_enabled.Enabled = false;
            txt_long.Enabled = false;
            txt_short.Enabled = false;
            txt_numeric.Text = this.m_iID.ToString();
            txt_numeric.Enabled = false;
        }

        Util_BindCountries();

        txt_name.Text = cCountry.Name;
        lbl_id.Text = cCountry.Id.ToString();
        chk_enabled.Checked = cCountry.Enabled;
        txt_long.Text = cCountry.LongIsoCode;
        txt_short.Text = cCountry.ShortIsoCode;

        int txtClassList = 0;

        ltr_txtClass.Text = "<table class=\"ektronGrid\">";
        for (txtClassList = 0; txtClassList <= TaxClassList.Count - 1; txtClassList++)
        {
            ltr_txtClass.Text += "<tr>";
            ltr_txtClass.Text += "   <td class=\"label\">";
            ltr_txtClass.Text += "       <label id=\"taxClass" + txtClassList + "\" value=\"" + TaxClassList[txtClassList].Name + "\">" + TaxClassList[txtClassList].Name + ":</label>";
            ltr_txtClass.Text += "   </td>";
            if (_TaxApi.GetItemByCountryId(TaxClassList[txtClassList].Id, cCountry.Id) == null)
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"0\" />%";
                ltr_txtClass.Text += "   </td>";
            }
            else
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"" + _TaxApi.GetItemByCountryId(TaxClassList[txtClassList].Id, cCountry.Id).Rate * 100 + "\"/>%";
                ltr_txtClass.Text += "   </td>";
            }
            ltr_txtClass.Text += "<td >";
            ltr_txtClass.Text += "</td>";
            ltr_txtClass.Text += "</tr>";
        }
        ltr_txtClass.Text += "</table>";


        tr_id.Visible = m_iID > 0;
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
    protected void Display_All()
    {
        System.Collections.Generic.List<TaxRateData> RateList = new System.Collections.Generic.List<TaxRateData>();
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        Ektron.Cms.Common.Criteria<CountryProperty> countryCriteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        int i = 0;
        dg_viewall.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        countryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        countryCriteria.PagingInfo.CurrentPage = _CurrentPageNumber;

        _TaxApi = new TaxApi();
	    TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        dg_viewall.AutoGenerateColumns = false;
        dg_viewall.Columns.Clear();
		
        System.Collections.Generic.List<CountryData> CountryRateList = new System.Collections.Generic.List<CountryData>();
        CountryApi m_refCountryTaxRate = new CountryApi();
        m_refCountryTaxRate = new CountryApi();

        countryCriteria.Condition = LogicalOperation.Or;
        if (_SearchCriteria != "")
        {
            countryCriteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
        }
        if (_SearchCriteria != "")
        {
            countryCriteria.AddFilter(CountryProperty.LongIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
        }
        if (_SearchCriteria != "")
        {
            countryCriteria.AddFilter(CountryProperty.ShortIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria);
        }

        CountryRateList = m_refCountryTaxRate.GetList(countryCriteria);

        _TotalPagesNumber = System.Convert.ToInt32(countryCriteria.PagingInfo.TotalPages);
        TotalPages.ToolTip = _TotalPagesNumber.ToString();

        if (_TotalPagesNumber <= 1)
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
            TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(_TotalPagesNumber))).ToString();
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
        colBound.HeaderText = this.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Name";
        colBound.HeaderText = this.GetMessage("generic name")+" (" + m_refMsg.GetMessage("lbl view tax rate for region") + ")";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Enabled";
        colBound.HeaderText = this.GetMessage("enabled");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Code";
        colBound.HeaderText = this.GetMessage("lbl code");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        dg_viewall.Columns.Add(colBound);

        dg_viewall.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("Enabled", typeof(string)));
        dt.Columns.Add(new DataColumn("Code", typeof(string)));
        dt.Columns.Add(new DataColumn("Country", typeof(string)));

        if (!(CountryRateList == null))
        {

            for (i = 0; i <= (CountryRateList.Count - 1); i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a  href=\"countrytaxtables.aspx?action=View&id=" + CountryRateList[i].Id + "\">" + CountryRateList[i].Id + "</a>";
                dr[1] = "<a href=\"#ExpandContent\" onclick=\"expandcontent(\'sc" + i + "\');return false;\">" + CountryRateList[i].Name + "</a><br />";

                dr[1] += "<div class=\"switchcontent\" id=\"sc" + i + "\"><table class=\"ektronForm\"><a onclick=\"expandcontent(\'sc" + i + "\')\" href=\"countrytaxtables.aspx?action=View&id=" + CountryRateList[i].Id + "\">" + m_refMsg.GetMessage("lbl view tax rate") + "</a><br />";
                foreach (TaxClassData taxClass in TaxClassList)
                {
                    dr[1] += "<tr><td><br/><label class=\"label\" id=\"" + taxClass.Name + "\">" + taxClass.Name + "</label></td>";
                    dr[1] += "<td><input type=\"text\" size=\"10\" align=\"right\" name=\"value\" readonly=\"true\" id=\"value\" value=\"" + GetRate(taxClass.Id, CountryRateList[i].Id) * 100 + "\"/>" + "<label id=\"lblPercentage\">" + "%" + "</label></td></tr>";
                }

                dr[1] += "</table></div>";

                dr[2] = "<input type=\"CheckBox\" ID=\"chk_enabled" + i + "\" disabled=\"true\"  " + ((CountryRateList[i].Enabled) ? "Checked=\"checked\"" : "") + "/>";
                dr[3] = "<a href=\"countrytaxtables.aspx?action=View&id=" + CountryRateList[i].Id + "\">" + CountryRateList[i].ShortIsoCode + "</a>";
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);

        dg_viewall.DataSource = dv;
        dg_viewall.DataBind();
    }

    protected void Display_View()
    {
        CountryData cCountry = null;
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();

        Util_BindCountries();

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);
        _TaxApi = new TaxApi();
        cCountry = _CountryApi.GetItem(Convert.ToInt32(this.m_iID));

        txt_name.Text = cCountry.Name;
        lbl_id.Text = cCountry.Id.ToString();
        chk_enabled.Checked = cCountry.Enabled;
        txt_long.Text = cCountry.LongIsoCode;
        txt_short.Text = cCountry.ShortIsoCode;

        if (this.m_iID > 0)
        {
            tr_numIso.Visible = false;
        }

        int txtClassList = 0;
        ltr_txtClass.Text = "<table class=\"ektronGrid\">";
        for (txtClassList = 0; txtClassList <= TaxClassList.Count - 1; txtClassList++)
        {
            ltr_txtClass.Text += "<tr>";
            ltr_txtClass.Text += "   <td class=\"label\">";
            ltr_txtClass.Text += "       <label id=\"taxClass" + txtClassList + "\" value=\"" + TaxClassList[txtClassList].Name + "\">" + TaxClassList[txtClassList].Name + ":</label>";
            ltr_txtClass.Text += "   </td>";
            if (_TaxApi.GetItemByCountryId(TaxClassList[txtClassList].Id, cCountry.Id) == null)
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input disabled=\"true\" type=\"text\" name=\"txtClassRate" + txtClassList + "\" id=\"txtClassRate" + txtClassList + "\" value=\"0\"/>%";
                ltr_txtClass.Text += "   </td>";
            }
            else
            {
                ltr_txtClass.Text += "   <td class=\"value\">";
                ltr_txtClass.Text += "       <input disabled=\"true\" type=\"text\" id=\"txtClassRate" + txtClassList + "\" name=\"txtClassRate" + txtClassList + "\" value=\"" + _TaxApi.GetItemByCountryId(TaxClassList[txtClassList].Id, cCountry.Id).Rate * 100 + "\"/>%  ";
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

    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
                AddBackButton(_PageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", _PageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\" return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit country tax rate");
                    AddHelpButton("editcountrytaxrate");
                }
                else
                {
                    SetTitleBarToMessage("lbl add country tax rate");
                    AddHelpButton("addcountrytaxrate");
                }
                break;
            case "view":
                AddBackButton(_PageName);
				this.AddButtonwithMessages(AppImgPath + "../UI/Icons/contentEdit.png", _PageName + "?action=addedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                if (_CountryApi.CanDelete(Convert.ToInt32(this.m_iID), out _ValidateResult))
                {
					this.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", _PageName + "?action=del&id=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete country") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                SetTitleBarToMessage("lbl view country tax rate");
                AddHelpButton("viewcountrytaxrate");
                break;
            default:

                this.AddSearchBox(EkFunctions.HtmlEncode(_SearchCriteria), new ListItemCollection(), "searchCountry", false);
                SetTitleBarToMessage("lbl country tax table");
                AddHelpButton("countrytaxtable");
                break;

        }

        ltr_name.Text = GetMessage("generic name");
        ltr_id.Text = GetMessage("generic id");
        ltr_enabled.Text = GetMessage("enabled");
        ltr_long.Text = GetMessage("lbl longisocode");
        ltr_numeric.Text = GetMessage("lbl numericisocode");
        ltr_short.Text = GetMessage("lbl shortisocode");
    }

    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        Ektron.Cms.Common.Criteria<TaxClassProperty> TaxClasscriteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria);

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var iNumISO = Trim(document.getElementById(\'").Append(txt_numeric.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var shortISO = Trim(document.getElementById(\'").Append(txt_short.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var slngISO = Trim(document.getElementById(\'").Append(txt_long.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\' || iNumISO == \'\' || isNaN(iNumISO) || shortISO == \'\' || slngISO == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err fields req")).Append("\'); } ").Append(Environment.NewLine);
        //sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err region title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl region disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() {").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   var taxClass = ").Append(TaxClassList.Count).Append(";").Append(Environment.NewLine);
        sbJS.Append("   var i = 0;").Append(Environment.NewLine);
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

        sbJS.Append(" function searchCountry() { ").Append(Environment.NewLine);
        sbJS.Append("   var sSearchTerm = Trim(document.getElementById(\'txtSearch\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sSearchTerm != \'\') { window.location.href = \'").Append(_PageName).Append("?search=\' + sSearchTerm;} else { alert(\'").Append(GetMessage("js err please enter text")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text += sbJS.ToString();
    }

    protected void Util_SetEnabled(bool toggle)
    {
        this.txt_name.Enabled = toggle;
        txt_long.Enabled = toggle;
        txt_numeric.Enabled = toggle;
        txt_short.Enabled = toggle;
        chk_enabled.Enabled = toggle;
    }

    protected void Util_BindCountries()
    {
        if ((_CountryList != null) && _CountryList.Count > 0)
        {
            //drp_country.DataSource = CountryList
            //drp_country.DataTextField = "Name"
            //drp_country.DataValueField = "Id"
            //drp_country.DataBind()
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

    protected decimal GetRate(long taxClassId, long regionId)
    {
        TaxRateData Rate = new TaxRateData();
        Ektron.Cms.Commerce.TaxApi m_refTaxRate = new Ektron.Cms.Commerce.TaxApi();
        m_refTaxRate = new TaxApi();

        try
        {
            Rate = m_refTaxRate.GetItemByCountryId(taxClassId, regionId);
            return Rate.Rate;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    #endregion

    #region JS/CSS

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

    }

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    #endregion

}

