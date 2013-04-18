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
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

public partial class Commerce_coupons : workareabase
{
    protected string m_sCurrencyCharacter = "$";
    protected string m_sPageName = "coupons.aspx";
    protected CouponApi CouponManager;
    protected bool IsUsed = false;
    protected bool IsActive = false;
    protected Currency m_refCurrency = null;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected string AppPath = "";

    #region Page Functions
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        m_refCurrency = new Currency(m_refContentApi.RequestInformationRef);
        AppPath = m_refContentApi.AppPath;
        RegisterResources();
        try
        {
            CouponManager = new CouponApi();
            Util_CheckAccess();
            drp_discounttype.Items.Add(new ListItem(GetMessage("lbl coupon amount"), "0"));
            drp_discounttype.Items.Add(new ListItem(GetMessage("lbl coupon amount percent"), "1"));
            drp_discounttype.Items.Add(new ListItem(GetMessage("lbl free shipping"), "2"));
            drp_discounttype.Attributes.Add("onchange", "document.getElementById(\'txt_discountval\').disabled = (this.selectedIndex == 2); document.getElementById(\'sel_currency\').disabled = (this.selectedIndex == 1 || this.selectedIndex == 2);");
            rad_type.Items.Add(new ListItem(GetMessage("lbl coupon type basket"), EkEnumeration.CouponType.BasketLevel.ToString()));
            rad_type.Items.Add(new ListItem(GetMessage("lbl coupon type basket item most exp"), EkEnumeration.CouponType.MostExpensiveItem.ToString()));
            rad_type.Items.Add(new ListItem(GetMessage("lbl coupon type basket item least exp"), EkEnumeration.CouponType.LeastExpensiveItem.ToString()));
            rad_type.Items.Add(new ListItem(GetMessage("lbl coupon type basket item all"), EkEnumeration.CouponType.AllItems.ToString()));
            switch (m_sPageAction)
            {
                case "addedit":
                    Util_SetJS();
                    if (Page.IsPostBack)
                    {
                        Process_AddEdit();
                    }
                    else
                    {
                        Display_AddEdit();
                    }
                    Util_SetDropEnabled();
                    break;
                case "view":
                    Display_View();
                    break;
                case "delete":
                    Process_Delete();
                    break;
                case "deactivate":
                    Process_Deactivate();
                    break;
                default:
                    if (Page.IsPostBack == false)
                    {
                        Display_View_All();
                    }
                    break;
            }
            Util_SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    #endregion
    #region Display
    protected void Display_View_All()
    {
        List<CouponData> CouponList = new List<CouponData>();
        Criteria<CouponProperty> criteria = new Criteria<CouponProperty>();
        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;

        CouponList = CouponManager.GetList(criteria);
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
        dg_coupon.DataSource = CouponList;
        dg_coupon.DataBind();
    }
    protected void Display_View()
    {
        CouponData cCoupon;
        List<ProductCouponEntryData> AppliedList = new List<ProductCouponEntryData>();

        cCoupon = CouponManager.GetItem(m_iID);
        AppliedList = CouponManager.GetProductList(m_iID);

        pnl_view.Visible = true;
        pnl_viewall.Visible = false;

        Util_PopulateFields(cCoupon, false);
        Util_BuildAppliedTable(AppliedList);
        Util_EnableFields(false);
        para_options.Visible = false;
    }
    protected void Display_AddEdit()
    {
        CouponData cCoupon = new CouponData();
        List<ProductCouponEntryData> AppliedList = new List<ProductCouponEntryData>();

        if (m_iID > 0)
        {
            cCoupon = CouponManager.GetItem(m_iID);
            AppliedList = CouponManager.GetProductList(m_iID);
        }
        else
        {
            tr_id.Visible = false;
            tr_used.Visible = false;
        }

        pnl_view.Visible = true;
        pnl_viewall.Visible = false;

        Util_PopulateFields(cCoupon, true);
        Util_BuildAppliedTable(AppliedList);
        Util_EnableFields(true);
        Util_SetJS();
    }
    #endregion
    #region Process

    protected void Process_Deactivate()
    {
        if (m_iID > 0)
        {
            CouponManager.Deactivate(m_iID);
            Response.Redirect(m_sPageName, false);
        }
    }

    protected void Process_Delete()
    {
        if (m_iID > 0)
        {
            if (CouponManager.IsCouponUsedForOrder(m_iID))
            {
                CouponManager.Deactivate(m_iID);
            }
            else
            {
                CouponManager.Delete(m_iID);
            }
            Response.Redirect(m_sPageName, false);
        }
    }

    protected void Process_AddEdit()
    {
        CouponData cCoupon = new CouponData();
        if (m_iID > 0)
        {
            cCoupon = CouponManager.GetItem(m_iID);
        }
        cCoupon.Description = (string)txt_desc.Text;
        cCoupon.Code = (string)txt_code.Text;
        cCoupon.IsActive = System.Convert.ToBoolean(chk_active.Checked);

        if (Request.Form["go_live"] != "")
        {
            cCoupon.StartDate = EkFunctions.ReadDbDate(Strings.Trim(Request.Form["go_live"]));
        }
        if (Request.Form["end_date"] != "")
        {
            cCoupon.ExpirationDate = EkFunctions.ReadDbDate(Strings.Trim(Request.Form["end_date"]));
        }

        if (drp_discounttype.SelectedIndex == 2)
        {
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.FreeShipping;
            cCoupon.DiscountValue = 0;
        }
        else if (drp_discounttype.SelectedIndex == 1)
        {
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Percent;
            cCoupon.DiscountValue = System.Convert.ToDecimal(txt_discountval.Text);
        }
        else
        {
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Amount;
            cCoupon.DiscountValue = System.Convert.ToDecimal(txt_discountval.Text);
        }
        cCoupon.CouponType = (EkEnumeration.CouponType)Enum.Parse(typeof(EkEnumeration.CouponType), rad_type.SelectedValue);
        cCoupon.OnePerCustomer = System.Convert.ToBoolean(chk_oneper.Checked);
        cCoupon.MaximumUses = EkFunctions.ReadIntegerValue(txt_expireafter.Text, 0);
        cCoupon.MinimumAmount = EkFunctions.ReadDecimalValue(txt_minamount.Text, 0);
        cCoupon.MaximumAmount = EkFunctions.ReadDecimalValue(txt_maxamount.Text, 0);
        cCoupon.CurrencyId = System.Convert.ToInt32(hdn_currency.Value);

        if (m_iID > 0)
        {
            CouponManager.Update(cCoupon);
            Process_AppliedTo(cCoupon.Id, false);
            Response.Redirect(m_sPageName + "?id=" + m_iID.ToString() + "&action=view", false);
        }
        else
        {
            CouponManager.Add(cCoupon);
            Process_AppliedTo(cCoupon.Id, true);
            Response.Redirect(m_sPageName, false);
        }
    }
    protected void Process_AppliedTo(long couponId, bool IsAdd)
    {
        int i = 1;
        List<long> exclusionIdList = new List<long>();
        List<int> exclusionLanguageList = new List<int>();
        while (Request.Form["txtitemposidx" + i] != null)
        {
            int idx = Convert.ToInt32(Request.Form["txtitemposidx" + i]);
            long iEntryId = Convert.ToInt64(Request.Form["txtitemid" + idx]);
            long iEntryLang = Convert.ToInt64(Request.Form["txtitemlang" + idx]);
            exclusionIdList.Add(iEntryId);
            exclusionLanguageList.Add(Convert.ToInt32(iEntryLang));
            if (IsAdd || !CouponManager.IsCouponAppliedToProduct(couponId, iEntryId))
            {
                CouponManager.AddCouponToProduct(couponId, iEntryId, true);
            }
            i++;
        }
        CouponManager.DeleteCouponProducts(couponId, exclusionIdList);
    }
    #endregion
    #region Util
    protected void Util_SetLabels()
    {
        lbl_id.Text = GetMessage("generic id");
        lbl_used.Text = GetMessage("lbl coupon used");
        ltr_code.Text = GetMessage("lbl coupon code");
        ltr_desc.Text = GetMessage("lbl coupon desc");
        ltr_type.Text = GetMessage("lbl coupon type");
        ltr_oneper.Text = GetMessage("lbl coupon one per");
        ltr_active.Text = GetMessage("lbl coupon active");
        ltr_discountval.Text = GetMessage("lbl coupon discount");
        ltr_minamount.Text = GetMessage("lbl coupon min ammount");
        ltr_maxamount.Text = GetMessage("lbl coupon max ammount");
        ltr_expireafter.Text = GetMessage("lbl coupon expires after");
        ltr_startdate.Text = GetMessage("lbl coupon start date");
        ltr_enddate.Text = GetMessage("lbl coupon exp date");
        ltr_instructions.Text = GetMessage("lbl select entries for coupon");
        switch (m_sPageAction)
        {
            case "view":

                this.Tabs.On();
                this.Tabs.AddTabByMessage("properties text", "dvProp");
                Tabs.AddTabByMessage("generic type", "dvType");
                Tabs.AddTabByMessage("generic options", "dvOptions");
                this.Tabs.AddTabByMessage("lbl applies to", "dvApplies");
                SetTitleBarToMessage("lbl view coupon");
                workareamenu actionMenu_1 = new workareamenu("action", this.GetMessage("lbl action"), this.AppPath + "images/UI/Icons/check.png");
                actionMenu_1.AddLinkItem(this.AppImgPath + "edit.gif", this.GetMessage("generic edit title"), m_sPageName + "?id=" + m_iID.ToString() + "&action=addedit");
                actionMenu_1.AddBreak();
                if (IsActive)
                {
                    actionMenu_1.AddItem(this.AppImgPath + "commerce/coupon-inactive.gif", this.GetMessage("lbl coupon mark inactive"), "if(confirm(\'" + GetMessage("js confirm coupon mark inactive") + "\')) { window.location.href = \'" + m_sPageName + "?id=" + m_iID.ToString() + "&action=deactivate" + "\'; } ");
                }
                if (!IsUsed)
                {
                    actionMenu_1.AddItem(this.AppPath + "images/UI/Icons/delete.png", this.GetMessage("btn delete"), "if(confirm(\'" + GetMessage("js confirm coupon delete") + "\')) { window.location.href = \'" + m_sPageName + "?id=" + m_iID.ToString() + "&action=delete" + "\'; } ");
                }
                this.AddMenu(actionMenu_1);
                this.AddBackButton(this.m_sPageName);
                break;

            case "addedit":
                this.Tabs.On();
                if (m_iID == 0)
                {
                    Tabs.ViewAsWizard();
                    Tabs.AddTabByString("1", "dvProp");
                    Tabs.AddTabByString("2", "dvType");
                    Tabs.AddTabByString("3", "dvOptions");
                    Tabs.AddTabByString("4", "dvApplies");
                }
                else
                {
                    this.Tabs.AddTabByMessage("properties text", "dvProp");
                    Tabs.AddTabByMessage("generic type", "dvType");
                    Tabs.AddTabByMessage("generic options", "dvOptions");
                    this.Tabs.AddTabByMessage("lbl applies to", "dvApplies");
                }
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit coupon");
                }
                else
                {
                    SetTitleBarToMessage("lbl add coupon");
                }
                workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                actionMenu.AddItem(AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), "SubmitForm(" + EkEnumeration.AssetActionType.Save + ");");
                actionMenu.AddBreak();
                actionMenu.AddLinkItem(AppPath + "images/ui/icons/cancel.png", this.GetMessage("generic cancel"), (string)(m_iID > 0 ? (m_sPageName + "?id=" + m_iID.ToString() + "&action=view") : m_sPageName));
                this.AddMenu(actionMenu);
                if (this.m_iID > 0)
                {
                    this.AddHelpButton("editcoupon");
                }
                else
                {
                    this.AddHelpButton("addcoupon");
                }
                break;
            default:
                SetTitleBarToMessage("lbl coupons");
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl new"), AppPath + "images/UI/Icons/star.png");
                newMenu.AddLinkItem(this.AppImgPath + "commerce/coupon.gif", this.GetMessage("lbl coupon"), m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);
                this.AddHelpButton("coupons");
                break;
        }

    }
    protected void Util_EnableFields(bool toggle)
    {
        txt_code.Enabled = toggle;
        txt_desc.Enabled = toggle;
        chk_active.Enabled = toggle;
        chk_oneper.Enabled = toggle;
        txt_discountval.Enabled = toggle;
        txt_minamount.Enabled = toggle;
        txt_maxamount.Enabled = toggle;
        txt_expireafter.Enabled = toggle;
        drp_discounttype.Enabled = toggle;
        rad_type.Enabled = toggle;
    }
    protected void Util_PopulateFields(CouponData cCoupon, bool editable)
    {
        StringBuilder sbCurrency = new StringBuilder();
        List<CurrencyData> activeCurrencyList = m_refCurrency.GetActiveCurrencyList();

        ltr_id.Text = cCoupon.Id.ToString();
        if (m_iID > 0)
        {
            IsUsed = CouponManager.IsCouponUsedForOrder(m_iID);
        }
        IsActive = cCoupon.IsActive;
        chk_used.Checked = IsUsed;
        txt_code.Text = cCoupon.Code;
        txt_desc.Text = cCoupon.Description;
        chk_active.Checked = cCoupon.IsActive;
        chk_oneper.Checked = cCoupon.OnePerCustomer;
        if (cCoupon.DiscountType == EkEnumeration.CouponDiscountType.FreeShipping)
        {
            txt_discountval.Text = "0.00";
            txt_discountval.Enabled = false;
            drp_discounttype.SelectedIndex = 2;
        }
        else if (cCoupon.DiscountType == EkEnumeration.CouponDiscountType.Percent)
        {
            txt_discountval.Text = FormatCurrency(cCoupon.DiscountValue, "");
            drp_discounttype.SelectedIndex = 1;
        }
        else if (cCoupon.DiscountType == EkEnumeration.CouponDiscountType.Amount)
        {
            txt_discountval.Text = FormatCurrency(cCoupon.DiscountValue, "");
            drp_discounttype.SelectedIndex = 0;
        }
        switch (m_sPageAction)
        {
            case "view":
                txt_minamount.Text = FormatCurrency(cCoupon.MinimumAmount, "");
                txt_maxamount.Text = FormatCurrency(cCoupon.MaximumAmount, "");
                break;
            case "addedit":
                txt_minamount.Text = cCoupon.MinimumAmount.ToString("0.00");
                txt_maxamount.Text = cCoupon.MaximumAmount.ToString("0.00");
                break;
        }

        switch (m_sPageAction)
        {
            case "view":
                CurrencyData currencydata = m_refCurrency.GetItem(cCoupon.CurrencyId);
                if (cCoupon.DiscountType == EkEnumeration.CouponDiscountType.Amount)
                {
                    sbCurrency.Append("<select disabled=\"disabled\" id=\"sel_currency\"> ").Append(Environment.NewLine);
                    sbCurrency.Append(" <option value=\"id:ektron_Pricing_").Append(currencydata.Id).Append(";label:").Append(currencydata.Name).Append(";symbol:").Append(currencydata.ISOCurrencySymbol).Append(currencydata.CurrencySymbol).Append(" selected=\"selected\">").Append(currencydata.AlphaIsoCode).Append("</option> ").Append(Environment.NewLine);
                    sbCurrency.Append("</select> ").Append(Environment.NewLine);
                    ltr_drpCurrency.Text = sbCurrency.ToString();
                }
                break;
            case "addedit":
                if (cCoupon.CurrencyId > 0)
                {
                    if (cCoupon.DiscountType == EkEnumeration.CouponDiscountType.Amount)
                    {
                        sbCurrency.Append("<select id=\"sel_currency\" > ").Append(Environment.NewLine);
                        for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
                        {
                            sbCurrency.Append(" <option value=\"id:ektron_Pricing_").Append(activeCurrencyList[i].Id).Append(";label:").Append(activeCurrencyList[i].Name).Append(";symbol:").Append(activeCurrencyList[i].ISOCurrencySymbol).Append(activeCurrencyList[i].CurrencySymbol).Append("\" " + ((activeCurrencyList[i].Id == cCoupon.CurrencyId) ? "selected=\"selected\"" : "") + ">").Append(activeCurrencyList[i].AlphaIsoCode).Append("</option> ").Append(Environment.NewLine);
                        }
                        sbCurrency.Append("</select> ").Append(Environment.NewLine);
                    }
                    else
                    {
                        sbCurrency.Append("<select id=\"sel_currency\" disabled=\"disabled\" > ").Append(Environment.NewLine);
                        for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
                        {
                            sbCurrency.Append(" <option value=\"id:ektron_Pricing_").Append(activeCurrencyList[i].Id).Append(";label:").Append(activeCurrencyList[i].Name).Append(";symbol:").Append(activeCurrencyList[i].ISOCurrencySymbol).Append(activeCurrencyList[i].CurrencySymbol).Append("\" " + ((activeCurrencyList[i].Id == cCoupon.CurrencyId) ? "selected=\"selected\"" : "") + ">").Append(activeCurrencyList[i].AlphaIsoCode).Append("</option> ").Append(Environment.NewLine);
                        }
                        sbCurrency.Append("</select> ").Append(Environment.NewLine);
                    }
                }
                else
                {
                    sbCurrency.Append("<select id=\"sel_currency\" > ").Append(Environment.NewLine);
                    for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
                    {
                        sbCurrency.Append(" <option value=\"id:ektron_Pricing_").Append(activeCurrencyList[i].Id).Append(";label:").Append(activeCurrencyList[i].Name).Append(";symbol:").Append(activeCurrencyList[i].ISOCurrencySymbol).Append(activeCurrencyList[i].CurrencySymbol).Append("\" " + ((activeCurrencyList[i].Id == m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId) ? "selected=\"selected\"" : "") + ">").Append(activeCurrencyList[i].AlphaIsoCode).Append("</option> ").Append(Environment.NewLine);
                    }
                    sbCurrency.Append("</select> ").Append(Environment.NewLine);
                }
                ltr_drpCurrency.Text = sbCurrency.ToString();
                break;
        }

        txt_expireafter.Text = cCoupon.MaximumUses.ToString();
        if (editable)
        {
            Util_AssignDates(cCoupon.StartDate, cCoupon.ExpirationDate);
        }
        else
        {
            ltr_startdatesel.Text = cCoupon.StartDate.ToLongDateString() + " " + cCoupon.StartDate.ToLongTimeString();
            if (DateTime.Compare(cCoupon.ExpirationDate, DateTime.MaxValue) == 0)
            {
                ltr_enddatesel.Text = "-";
            }
            else
            {
                ltr_enddatesel.Text = cCoupon.ExpirationDate.ToLongDateString() + " " + cCoupon.ExpirationDate.ToLongTimeString();
            }
        }
        rad_type.SelectedValue = cCoupon.CouponType.GetHashCode().ToString();
    }
    protected void Util_AssignDates(DateTime startdate, DateTime enddate)
    {
        EkDTSelector dateSchedule;

        dateSchedule = this.m_refContentApi.EkDTSelectorRef;
        dateSchedule.formName = "form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "go_live";
        dateSchedule.spanId = "go_live_span";
        dateSchedule.targetDate = startdate;
        ltr_startdatesel.Text = dateSchedule.displayCultureDateTime(true, "", "");
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (enddate.Year == DateTime.MaxValue.Year)
        {
            dateSchedule.targetDate = DateTime.MaxValue;
        }
        else
        {
            dateSchedule.targetDate = enddate;
        }
        ltr_enddatesel.Text = dateSchedule.displayCultureDateTime(true, "", "");
    }
    protected void Util_SetDropEnabled()
    {

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);
        sbJS.Append("   document.getElementById(\'txt_discountval\').disabled = (document.getElementById(\'").Append(drp_discounttype.ID).Append("\').selectedIndex == 2);").Append(Environment.NewLine);
        sbJS.Append("</script>").Append(Environment.NewLine);

        ltr_endJS.Text = sbJS.ToString();

    }
    protected void Util_SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("    <script language=\"JavaScript\" type=\"text/javascript\" src=\"../java/internCalendarDisplayFuncs.js\"></script>").Append(Environment.NewLine);
        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   validate_Amount();").Append(Environment.NewLine);
        sbJS.Append("   getCurrency();").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\', null,null);").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("   function AddItem() { ").Append(Environment.NewLine);
        sbJS.Append("       ektb_show(\'\',\'byproduct.aspx?action=coupon&EkTB_iframe=true&height=300&width=500&modal=true\', null); ").Append(Environment.NewLine);
        sbJS.Append("   } ").Append(Environment.NewLine);
        sbJS.Append("   function DeleteItem() {").Append(Environment.NewLine);
        sbJS.Append("       var iAttr = getCheckedInt(false);").Append(Environment.NewLine);
        sbJS.Append("       if (iAttr == -1) {").Append(Environment.NewLine);
        sbJS.Append("           alert(\'").Append(GetMessage("js please sel coupon")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("       } else {").Append(Environment.NewLine);
        sbJS.Append("           deleteChecked();").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_code.ID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err entry code req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_code.ID).Append("\',\"").Append(GetMessage("lbl entry disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function validate_Amount() {").Append(Environment.NewLine);
        sbJS.Append("   var sMinAmount = Trim(document.getElementById(\'").Append(txt_minamount.ID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sMaxAmount = Trim(document.getElementById(\'").Append(txt_maxamount.ID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sDiscountVal = Trim(document.getElementById(\'").Append(txt_discountval.ID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var sMaxUses = Trim(document.getElementById(\'").Append(txt_expireafter.ID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   var iDiscountType = Trim(document.getElementById(\'").Append(drp_discounttype.ID).Append("\').selectedIndex); ").Append(Environment.NewLine);
        sbJS.Append("   var startDate = $ektron(\'input#go_live_iso\')[0].value;").Append(Environment.NewLine);
        sbJS.Append("   var endDate = $ektron(\'input#end_date_iso\')[0].value; ").Append(Environment.NewLine);
        sbJS.Append("   if ((endDate < startDate && endDate != \'[None]\') && (endDate < startDate && endDate != \'\') )").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err start end date")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   if(sMinAmount ==  \'\') { sMinAmount = 0; document.getElementById(\'").Append(txt_minamount.ID).Append("\').value = 0;} ").Append(Environment.NewLine);
        sbJS.Append("   if(sMaxAmount ==  \'\') { sMaxAmount = 0; document.getElementById(\'").Append(txt_maxamount.ID).Append("\').value = 0;} ").Append(Environment.NewLine);
        sbJS.Append("   if(sMaxUses ==  \'\') { sMaxAmount = 0; document.getElementById(\'").Append(txt_expireafter.ID).Append("\').value = 0;} ").Append(Environment.NewLine);
        sbJS.Append("   if(isInteger(sMaxUses) ==  false || sMaxUses >= 2147483648) { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err number user not integer")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   if( iDiscountType != 2 && (isNaN(sDiscountVal.replace(/,/g,\'\')) || sDiscountVal == \'\') ) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err disc amount not numeric")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   if(isNaN(sMaxUses) || sMaxUses == \'\' ) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err n uses amount not numeric")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   if(sMinAmount < 0 || isNaN(sMinAmount)) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err min amount not numeric")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   if(sMaxAmount < 0 || isNaN(sMaxAmount)) ").Append(Environment.NewLine);
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err max amount not numeric")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_minamount.ID).Append("\',\"").Append(GetMessage("lbl entry disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_maxamount.ID).Append("\',\"").Append(GetMessage("lbl entry disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_discountval.ID).Append("\',\"").Append(GetMessage("lbl entry disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append("}").Append(Environment.NewLine);

        sbJS.Append("   function isInteger(s)").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       return s.length > 0 && !(/[^0-9]/).test(s);").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);

        sbJS.Append(JSLibrary.CheckKeyValue());
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append("</script>").Append(Environment.NewLine);
        ltr_js.Text += sbJS.ToString();
    }
    protected void Util_CheckAccess()
    {
        if (!m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            throw (new Exception(GetMessage("err not role commerce-admin")));
        }
    }
    protected void Util_BuildAppliedTable(List<ProductCouponEntryData> AppliedList)
    {
        StringBuilder sbApplies = new StringBuilder();
        sbApplies.Append("                        <table border=\"0\" cellspacing=\"0\" id=\"tblApplies\" class=\"ektableutil\">").Append(Environment.NewLine);
        sbApplies.Append("                        <thead>").Append(Environment.NewLine);
        sbApplies.Append("                          <tr class=\"item_header\"><th></th><th></th><th>").Append(GetMessage("generic id")).Append("</th><th>").Append(GetMessage("generic language")).Append("</th><th>").Append(GetMessage("generic name")).Append("</th><th>&#160;</th><th>&#160;</th></tr>").Append(Environment.NewLine);
        sbApplies.Append("                        </thead>").Append(Environment.NewLine);
        sbApplies.Append("                        <tbody>").Append(Environment.NewLine);
        for (int i = 0; i <= (AppliedList.Count - 1); i++)
        {
            sbApplies.Append("<tr");
            if (i % 2 > 0)
            {
                sbApplies.Append(" class=\"itemrow0\"");
            }
            sbApplies.Append(">").Append(Environment.NewLine);
            sbApplies.Append("<td>").Append(i + 1).Append("</td>").Append(Environment.NewLine);
            sbApplies.Append("<td>").Append(Util_GetEntryImage(AppliedList[i])).Append("</td>").Append(Environment.NewLine);
            sbApplies.Append("<td>").Append(AppliedList[i].ObjectId).Append("</td>").Append(Environment.NewLine);
            sbApplies.Append("<td>").Append("1033").Append("</td>").Append(Environment.NewLine); //hardcoding for now, no language in the future
            sbApplies.Append("<td>").Append(AppliedList[i].Title).Append("</td>").Append(Environment.NewLine);
            sbApplies.Append("<td>");
            sbApplies.Append("<input name=\"txtitemid").Append(i + 1).Append("\" type=\"hidden\" id=\"txtitemid").Append(i + 1).Append("\" value=\"").Append(AppliedList[i].ObjectId).Append("\" />");
            sbApplies.Append("<input name=\"txtitemlang").Append(i + 1).Append("\" type=\"hidden\" id=\"txtitemlang").Append(i + 1).Append("\" value=\"").Append("1033").Append("\" />"); //hardcoding - removing language???
            sbApplies.Append("<input name=\"txtitemposidx").Append(i + 1).Append("\" type=\"hidden\" id=\"txtitemposidx").Append(i + 1).Append("\" value=\"").Append(i + 1).Append("\" />");
            sbApplies.Append("</td>").Append(Environment.NewLine);
            sbApplies.Append("<td><input type=\"radio\" value=\"").Append(i + 1).Append("\" name=\"radInput\" /></td>").Append(Environment.NewLine);
            sbApplies.Append("</tr>").Append(Environment.NewLine);
        }
        sbApplies.Append("                        </tbody>").Append(Environment.NewLine);
        sbApplies.Append("                        </table>").Append(Environment.NewLine);
        ltr_appliesto.Text = sbApplies.ToString();
    }
    protected string Util_GetEntryImage(ProductCouponEntryData couponitem)
    {
        string sImage = m_refContentApi.AppPath + "images/ui/icons/brick.png";
        switch (couponitem.EntryType.GetHashCode())
        {
            case 1:
                sImage = AppImgPath + "images/ui/icons/bricks.png";
                break;
            case 2:
                sImage = AppImgPath + "images/ui/icons/box.png";
                break;
            case 3:
                sImage = AppImgPath + "images/ui/icons/package.png";
                break;
        }
        return "<img src=\"" + sImage + "\" />";
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
        Display_View_All();
        isPostData.Value = "true";
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/tables/tableutil.css", "EktronTableUtilCSS");

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/dhtml/coupontableutil.js", "EktronCouponTableUtilJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/commerce/com.Ektron.Commerce.Pricing.js", "EktronCommercePricingJS");

        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/box.css", "EktronBoxCSS");

    }
    #endregion

}


