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
using Ektron.Cms.Workarea;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

public partial class Commerce_locale_region : workareabase
{
    protected RegionApi m_refRegion = null;
    protected CountryApi m_refCountry = null;
    protected string m_sPageName = "region.aspx";
    protected System.Collections.Generic.List<CountryData> CountryList = new System.Collections.Generic.List<CountryData>();
    protected Criteria<CountryProperty> criteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected string searchCriteria = "";
    protected long countryId = 0;
    protected Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults validateResult = new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults();

    #region Page Functions
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        RegisterResource();
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        Util_CheckAccess();

        if (!string.IsNullOrEmpty(Page.Request.QueryString["search"]))
        {
            searchCriteria = Page.Request.QueryString["search"];
        }
        if (!string.IsNullOrEmpty(Page.Request.QueryString["country"]))
        {
            countryId = Convert.ToInt64(Page.Request.QueryString["country"]);
        }

        try
        {

            m_refRegion = new RegionApi();
            m_refCountry = new CountryApi();
            criteria.PagingInfo = new PagingInfo(10000);
            criteria.AddFilter(CountryProperty.IsEnabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);
            if (countryId > 0)
            {
                criteria.AddFilter(CountryProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, countryId);
            }

            hdnCurrentPage.Value = CurrentPage.Text;
            switch (this.m_sPageAction)
            {
                case "addedit":
                    CountryList = m_refCountry.GetList(criteria);
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
                    CountryList = m_refCountry.GetList(criteria);
                    Display_View();
                    break;
                default:
                    CountryList = m_refCountry.GetList(criteria);
                    if (Page.IsPostBack == false)
                    {
                        Display_All();
                    }
                    break;
            }
            Util_SetLabels();
            Util_SetJS();

        }
        catch (Exception ex)
        {
            if (ex.Message.IndexOf("unique key") > -1)
            {
                Utilities.ShowError(GetMessage("lbl region dupe"));
            }
            else
            {
                Utilities.ShowError(ex.Message);
            }
        }
    }
    #endregion

    #region Process
    protected void Process_AddEdit()
    {

        RegionData rRegion = null;
        if (this.m_iID > 0)
        {
            rRegion = m_refRegion.GetItem(this.m_iID);
            rRegion.Name = (string)txt_name.Text;
            rRegion.CountryId = Convert.ToInt64(drp_country.SelectedValue);
            rRegion.Code = (string)txt_code.Text;
            rRegion.Enabled = System.Convert.ToBoolean(chk_enabled.Checked);
            m_refRegion.Update(rRegion);
            Response.Redirect(m_sPageName + "?action=view&id=" + m_iID.ToString(), false);
        }
        else
        {
            rRegion = new RegionData(txt_name.Text, Convert.ToInt64(drp_country.SelectedValue), txt_code.Text, chk_enabled.Checked);
            m_refRegion.Add(rRegion);
            if (chk_addanother.Checked)
            {
                Response.Redirect(m_sPageName + "?action=addedit&country=" + drp_country.SelectedValue, false);
            }
            else
            {
                Response.Redirect(m_sPageName, false);
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
        Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results = new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults();
        if (this.m_iID > 0)
        {
            if (!m_refRegion.CanDelete(this.m_iID, out results))
            {
                StringBuilder msg = new StringBuilder();
                Utilities.ShowError(Ektron.Cms.Common.EkFunctions.GetAllValidationMessages(results));
            }
            else
            {
                m_refRegion.Delete(m_iID);
                Response.Redirect(m_sPageName, false);
            }
        }
    }
    #endregion

    #region Display
    protected void Display_AddEdit()
    {
        RegionData rRegion = new RegionData();
        if (m_iID > 0)
        {
            rRegion = m_refRegion.GetItem(this.m_iID);
        }

        Util_BindCountries();

        txt_name.Text = rRegion.Name;
        lbl_id.Text = rRegion.Id.ToString();
        chk_enabled.Checked = (countryId > 0) || rRegion.Enabled;
        txt_code.Text = rRegion.Code;
        drp_country.SelectedIndex = Util_GetCountryIndex(Convert.ToInt32(rRegion.CountryId));
        chk_addanother.Checked = countryId > 0;

        tr_addanother.Visible = m_iID == 0;
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
        System.Collections.Generic.List<RegionData> RegionList = new System.Collections.Generic.List<RegionData>();
        Ektron.Cms.Common.Criteria<RegionProperty> criteria = new Ektron.Cms.Common.Criteria<RegionProperty>(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        int i = 0;
        dg_viewall.PageSize = m_refContentApi.RequestInformationRef.PagingSize;
        dg_viewall.AutoGenerateColumns = false;
        dg_viewall.Columns.Clear();

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;

        if (searchCriteria != "")
        {
            criteria.AddFilter(RegionProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria);
        }

        RegionList = m_refRegion.GetList(criteria);

        TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);

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
        colBound.DataField = "Id";
        colBound.HeaderText = this.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Name";
        colBound.HeaderText = this.GetMessage("lbl address name");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Enabled";
        colBound.HeaderText = this.GetMessage("lbl overlay data enabled");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Code";
        colBound.HeaderText = this.GetMessage("lbl code");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_viewall.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Country";
        colBound.HeaderText = this.GetMessage("lbl address country");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_viewall.Columns.Add(colBound);

        dg_viewall.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("Enabled", typeof(string)));
        dt.Columns.Add(new DataColumn("Code", typeof(string)));
        dt.Columns.Add(new DataColumn("Country", typeof(string)));

        if (!(RegionList == null))
        {
            for (i = 0; i <= RegionList.Count - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"region.aspx?action=View&id=" + RegionList[i].Id + "\">" + RegionList[i].Id + "</a>";
                dr[1] = "<a href=\"region.aspx?action=View&id=" + RegionList[i].Id + "\">" + RegionList[i].Name + "</a>";
                dr[2] = "<input type=\"CheckBox\" ID=\"chk_enabled" + i + "\" disabled=\"true\" " + ((RegionList[i].Enabled) ? "Checked=\"checked\"" : "") + "/>";
                dr[3] = "<a href=\"region.aspx?action=View&id=" + RegionList[i].Id + "\">" + RegionList[i].Code + "</a>";
                dr[4] = "<label id=\"lblCountry\" >" + Util_GetCountryName(System.Convert.ToInt32(RegionList[i].CountryId)) + "</label>";
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);

        dg_viewall.DataSource = dv;
        dg_viewall.DataBind();
    }

    protected void Display_View()
    {
        RegionData rRegion = null;

        Util_BindCountries();

        rRegion = m_refRegion.GetItem(this.m_iID);

        txt_name.Text = rRegion.Name;
        lbl_id.Text = rRegion.Id.ToString();
        chk_enabled.Checked = rRegion.Enabled;
        txt_code.Text = rRegion.Code;
        drp_country.SelectedIndex = Util_GetCountryIndex(Convert.ToInt32(rRegion.CountryId));

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
        tr_addanother.Visible = false;
    }
    #endregion

    #region Private Helpers
    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
                AddBackButton(m_sPageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\" return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit region");
                    AddHelpButton("EditRegion");
                }
                else
                {
                    SetTitleBarToMessage("lbl add region");
                    AddHelpButton("AddRegion");
                }
                break;

            case "view":
                AddBackButton(m_sPageName);
                this.AddButtonwithMessages(AppImgPath + "../UI/Icons/contentEdit.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                if (m_refRegion.CanDelete(this.m_iID, out validateResult))
                {
                    this.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", m_sPageName + "?action=del&id=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete Region") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                SetTitleBarToMessage("lbl view region");
                AddHelpButton("ViewRegion");
                break;
            default:
                workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), AppImgPath + "../UI/Icons/star.png");
                newMenu.AddLinkItem(AppImgPath + "/menu/document.gif", GetMessage("lbl Region"), m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);

                this.AddSearchBox(EkFunctions.HtmlEncode(searchCriteria), new ListItemCollection(), "searchRegion");
                SetTitleBarToMessage("lbl regions");
                AddHelpButton("region");
                break;
        }
        ltr_name.Text = GetMessage("generic name");
        ltr_id.Text = GetMessage("generic id");
        ltr_enabled.Text = GetMessage("enabled");
        ltr_code.Text = GetMessage("lbl code");
        ltr_country.Text = GetMessage("lbl address country");
        ltr_addanother.Text = GetMessage("lbl add another region");
    }
    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err region title req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   var sCode = Trim(document.getElementById(\'").Append(txt_code.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if ( sCode == \'\' || sCode.length > 5 || sCode.length < 1) { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err region code req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl region disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\');").Append(Environment.NewLine);
        sbJS.Append("   return false; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function searchRegion() { ").Append(Environment.NewLine);
        sbJS.Append("   var sSearchTerm = $ektron(\'#txtSearch\').getInputLabelValue(); ").Append(Environment.NewLine);
        sbJS.Append("   if (sSearchTerm != \'\') { window.location.href = \'").Append(m_sPageName).Append("?search=\' + sSearchTerm;} else { alert(\'").Append(GetMessage("js err please enter text")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text += sbJS.ToString();
    }
    protected void Util_SetEnabled(bool toggle)
    {
        this.txt_name.Enabled = toggle;
        txt_code.Enabled = toggle;
        chk_enabled.Enabled = toggle;
        drp_country.Enabled = toggle;
    }
    protected void Util_BindCountries()
    {
        if ((CountryList != null) && CountryList.Count > 0)
        {
            drp_country.DataSource = CountryList;
            drp_country.DataTextField = "Name";
            drp_country.DataValueField = "Id";
            drp_country.DataBind();
        }
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

    #endregion
    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        if (!string.IsNullOrEmpty(hdnCurrentPage.Value))
        {
            _currentPageNumber = int.Parse((string)hdnCurrentPage.Value);
        }
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
        Display_All();
        isPostData.Value = "true";
    }
    protected void RegisterResource()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}


