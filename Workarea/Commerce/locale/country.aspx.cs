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
using Ektron.Cms.Framework.UI.Controls.EktronUI;

public partial class Commerce_locale_country : workareabase
{
    protected CountryApi m_refCountry = null;
    protected string m_sPageName = "country.aspx";
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected string sortCriteria = "name";
    protected string searchCriteria = "";
    protected Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults validationResult = new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults();
    protected string AppPath = "";

    #region Page Functions
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        RegisterResources();
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        AppPath = m_refContentApi.AppPath;
        CommerceLibrary.CheckCommerceAdminAccess();

        if (!string.IsNullOrEmpty(Page.Request.QueryString["sort"]))
        {
            sortCriteria = Page.Request.QueryString["sort"];
        }
        if (!string.IsNullOrEmpty(Page.Request.QueryString["search"]))
        {
            searchCriteria = Page.Request.QueryString["search"];
        }
        m_refCountry = new CountryApi();
        hdnCurrentPage.Value = CurrentPage.Text;
        try
        {
            switch (this.m_sPageAction)
            {
                case "addedit":
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
                    Display_View();
                    break;
                default:
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
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion

    #region Process
    protected void Process_AddEdit()
    {
        CountryData cCountry = null;
        if (this.m_iID > 0)
        {
            cCountry = m_refCountry.GetItem(Convert.ToInt32(this.m_iID));
            cCountry.Id = System.Convert.ToInt32(txt_id.Text);
            cCountry.Name = (string)txt_name.Text;
            cCountry.LongIsoCode = (string)txt_long.Text;
            cCountry.ShortIsoCode = (string)txt_short.Text;
            cCountry.Enabled = System.Convert.ToBoolean(chk_enabled.Checked);
            m_refCountry.Update(cCountry);
            Response.Redirect(m_sPageName + "?action=view&id=" + m_iID.ToString(), false);
        }
        else
        {
            try
            {
                cCountry = m_refCountry.GetItem(System.Convert.ToInt32(txt_id.Text));
            }
            catch (Exception)
            {
                if (txt_long.Text.Length != 3)
                {
                    uxMessage.DisplayMode = Message.DisplayModes.Error;
                    uxMessage.Visible = true;
                    uxMessage.Text = GetMessage("lbl Long Iso Length");
                    return;
                }
                if (txt_short.Text.Length != 2)
                {
                    uxMessage.DisplayMode = Message.DisplayModes.Error;
                    uxMessage.Visible = true;
                    uxMessage.Text = GetMessage("lbl Short Iso Length"); ;
                    return;
                }
                cCountry = new CountryData(0, txt_name.Text, txt_short.Text, txt_long.Text, chk_enabled.Checked);
            }

            if ((cCountry != null) && cCountry.Id > 0)
            {
                throw (new Exception(GetMessage("lbl country dupe")));
            }
            else
            {
                cCountry.Id = System.Convert.ToInt32(txt_id.Text);
            }
            cCountry.Name = (string)txt_name.Text;
            try
            {
                m_refCountry.Add(cCountry);

                Response.Redirect(m_sPageName, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("unique key") > -1)
                {
                    Utilities.ShowError(GetMessage("lbl country dupe"));
                }
                else
                {
                    Utilities.ShowError(ex.Message);
                }
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
        if (this.m_iID > 0)
        {
            m_refCountry.Delete(Convert.ToInt32(m_iID));
        }
        Response.Redirect(m_sPageName, false);
    }
    #endregion

    #region Display
    protected void Display_AddEdit()
    {
        CountryData cCountry = new CountryData();
        if (m_iID > 0)
        {
            txt_id.Enabled = false;
            cCountry = m_refCountry.GetItem(Convert.ToInt32(this.m_iID));
        }

        txt_name.Text = cCountry.Name;
        txt_id.Text = cCountry.Id.ToString();
        chk_enabled.Checked = cCountry.Enabled;
        txt_long.Text = cCountry.LongIsoCode;
        txt_short.Text = cCountry.ShortIsoCode;

        // tr_id.Visible = (m_iID > 0)
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
        System.Collections.Generic.List<CountryData> CountryList = new System.Collections.Generic.List<CountryData>();
        Ektron.Cms.Common.Criteria<CountryProperty> criteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;

        if (sortCriteria.IndexOf("-") > -1)
        {
            criteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending;
        }
        switch (sortCriteria.Replace("-", ""))
        {
            case "id":
                criteria.OrderByField = CountryProperty.Id;
                break;
            case "enabled":
                criteria.OrderByField = CountryProperty.IsEnabled;
                break;
            case "longiso":
                criteria.OrderByField = CountryProperty.LongIsoCode;
                break;
            case "shortiso":
                criteria.OrderByField = CountryProperty.ShortIsoCode;
                break;
            default:
                criteria.OrderByField = CountryProperty.Name;
                break;
        }

        if (searchCriteria != "")
        {
            criteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria);
        }

        CountryList = m_refCountry.GetList(criteria);

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

        dg_viewall.DataSource = CountryList;
        dg_viewall.DataBind();
    }
    protected void Display_View()
    {
        CountryData cCountry = null;

        cCountry = m_refCountry.GetItem(Convert.ToInt32(this.m_iID));

        txt_name.Text = cCountry.Name;
        txt_id.Text = cCountry.Id.ToString();
        chk_enabled.Checked = cCountry.Enabled;
        txt_long.Text = cCountry.LongIsoCode;
        txt_short.Text = cCountry.ShortIsoCode;

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

    #region Private Helpers

    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
                AddBackButton(m_sPageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\"return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit country");
                    AddHelpButton("Editcountry");
                }
                else
                {
                    SetTitleBarToMessage("lbl add country");
                    AddHelpButton("Addcountry");
                }
                break;
            case "view":
                AddBackButton(m_sPageName);
				this.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                if (m_refCountry.CanDelete(Convert.ToInt32(this.m_iID), out validationResult))
                {
					this.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", m_sPageName + "?action=del&id=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete country") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                SetTitleBarToMessage("lbl view country");
                AddHelpButton("Viewcountry");
                break;
            default:
                workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), AppPath + "images/UI/Icons/star.png");
                newMenu.AddLinkItem(AppImgPath + "/menu/document.gif", GetMessage("lbl country"), m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);

                this.AddSearchBox(EkFunctions.HtmlEncode(searchCriteria), new ListItemCollection(), "searchCountry");
                SetTitleBarToMessage("lbl countries");
                AddHelpButton("country");
                break;
        }

        ltr_name.Text = GetMessage("generic name");
        ltr_id.Text = GetMessage("lbl numericisocode");
        ltr_enabled.Text = GetMessage("enabled");
        ltr_long.Text = GetMessage("lbl longisocode");
        ltr_short.Text = GetMessage("lbl shortisocode");
    }

    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script type=\"text/javascript\">").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err country title req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl country disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   var nISO = Trim(document.getElementById(\'").Append(txt_id.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sLISO = Trim(document.getElementById(\'").Append(txt_long.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sSISO = Trim(document.getElementById(\'").Append(txt_short.UniqueID).Append("\').value); ").Append(Environment.NewLine);

        sbJS.Append("   if (isNaN(nISO) || nISO == \'\' || nISO < 1 )").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err country iso not numeric")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   else if(sLISO.length == 0 || sSISO.length == 0)").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err long short iso empty")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\');").Append(Environment.NewLine);
        sbJS.Append("   return false; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function searchCountry() { ").Append(Environment.NewLine);
        sbJS.Append("   var sSearchTerm = $ektron(\'#txtSearch\').getInputLabelValue(); ").Append(Environment.NewLine);
        sbJS.Append("   if (sSearchTerm != \'\') { window.location.href = \'").Append(m_sPageName).Append("?search=\' + sSearchTerm;} else { alert(\'").Append(GetMessage("js err please enter text")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_js.Text += sbJS.ToString();
    }

    protected void Util_SetEnabled(bool toggle)
    {
        this.txt_name.Enabled = toggle;
        txt_long.Enabled = toggle;
        txt_short.Enabled = toggle;
        chk_enabled.Enabled = toggle;
        txt_id.Enabled = toggle;
    }

    protected string Util_SortUrl(string messageText, string sortingValue)
    {

        string urlString = "";
        if (sortingValue == sortCriteria && sortCriteria.IndexOf("-") == -1)
        {
            sortingValue = sortingValue + "-";
        }
        if (sortingValue == sortCriteria && sortCriteria.IndexOf("-") > -1)
        {
            sortingValue = sortingValue.Replace("-", "");
        }
        if (sortingValue == "enabled" && sortingValue != sortCriteria && sortCriteria.IndexOf("-") == -1)
         sortingValue = sortingValue + "-"; 

        urlString = "<a href=\"country.aspx?sort=" + sortingValue + "\">" + GetMessage(messageText) + "</a>";
        return urlString;

    }

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
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    #endregion

}


