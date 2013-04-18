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
using Ektron.Cms;
using Ektron.Cms.Commerce;

public partial class Commerce_paymentgateway : workareabase
{

    #region Member Variables

    protected string m_sPageName = "paymentgateway.aspx";
    protected bool m_bIsDefault = false;
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;
    protected SettingsData appSettings = new SettingsData();
    protected List<bool> supportsCards = new List<bool>();
    protected List<bool> supportsChecks = new List<bool>();
    protected string imageIconsPath = "";
    protected UserAPI _uapi = new UserAPI();

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterJS();
        this.RegisterCSS();

    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }

        imageIconsPath = this.m_refContentApi.AppPath + "images/ui/icons/";
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            CommerceLibrary.CheckCommerceAdminAccess();


            switch (base.m_sPageAction)
            {
                case "markdef":
                    Process_MarkDefault();
                    break;
                case "del":
                    Process_Delete();
                    break;
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
                case "editoptions":

                    Util_SetPaymentCheckBoxes();
                    if (Page.IsPostBack)
                    {
                        Process_EditOptions();
                    }
                    else
                    {
                        Display_EditOptions();
                    }
                    this.phGatewaysContent.Visible = false;
                    this.phGatewaysTab.Visible = false;
                    break;

                case "view":
                    Display_View();
                    break;
                default: // "viewall"
                    if (Page.IsPostBack == false)
                    {

                        Util_SetPaymentCheckBoxes();
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


    #endregion

    #region Display


    protected void Display_EditOptions()
    {

        chk_paypal.Enabled = true;
        //chk_google.Enabled = True

        dg_gateway.Visible = false;
        litPaymentGatways.Visible = false;

        Util_HidePagingLinks();

    }

    protected void Display_View()
    {

        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        PaymentGatewayData paymentGateway = null;
        PaymentManager cmsPaymentManager = new PaymentManager();
        System.Collections.IEnumerator paymentProviders = cmsPaymentManager.Providers.GetEnumerator();
        int gatewayIndex = 0;

        paymentGateway = gatewayService.GetItem(this.m_iID);

         while (paymentProviders.MoveNext())
        {
            PaymentGatewayProvider currentData = paymentProviders.Current as PaymentGatewayProvider;
            drp_GatewayName.Items.Add(currentData.Name);
            if (paymentGateway.Name == currentData.Name)
            {
                drp_GatewayName.SelectedIndex = gatewayIndex;
            }
            gatewayIndex++;

        }

        lbl_id.Text = paymentGateway.Id.ToString();
        chk_default.Checked = paymentGateway.IsDefault;
        txt_uid.Text = paymentGateway.UserId;
        txt_viewpwd.Text = ProtectPassword(paymentGateway.Password);
        txt_pwd.Visible = false;
        txt_spare1.Text = paymentGateway.CustomFieldOne;
        txt_spare2.Text = paymentGateway.CustomFieldTwo;
        chk_cc.Checked = paymentGateway.AllowsCreditCardPayments;
        chk_check.Checked = paymentGateway.AllowsCheckPayments;
        drp_GatewayName.Enabled = false;
        chk_default.Enabled = false;
        txt_uid.Enabled = false;
        txt_pwd.Enabled = false;
        txt_spare1.Enabled = false;
        txt_spare2.Enabled = false;
        chk_cc.Enabled = false;
        chk_check.Enabled = false;

        m_bIsDefault = paymentGateway.IsDefault;

    }

    protected void Display_AddEdit()
    {

        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        PaymentGatewayData paymentGateway = null;
        PaymentManager cmsPaymentManager = new PaymentManager();
        System.Collections.IEnumerator paymentProviders = cmsPaymentManager.Providers.GetEnumerator();
        int gatewayIndex = 0;
        string defaultName = "";

        paymentGateway = m_iID > 0 ? (gatewayService.GetItem(this.m_iID)) : (new PaymentGatewayData());

        while (paymentProviders.MoveNext())
        {

            string currentGatewayName = "";

            PaymentGatewayProvider currentData = paymentProviders.Current as PaymentGatewayProvider;
            if (paymentProviders.Current != null)
            {

                if (paymentGateway.Name.ToLower() == currentData.Name.ToLower())
                {
                    chk_check.Enabled = cmsPaymentManager.Providers[currentData.Name].SupportsCheckPayments;
                }
                if (gatewayIndex == 0)
                {
                    defaultName = currentData.Name;
                }

                if (currentData.Name.ToLower() != "google" && currentData.Name.ToLower() != "paypal")
                {

                    supportsCards.Add(cmsPaymentManager.Providers[currentData.Name].SupportsCreditCardPayments);
                    supportsChecks.Add(cmsPaymentManager.Providers[currentData.Name].SupportsCheckPayments);

                    currentGatewayName = currentData.Name;
                    drp_GatewayName.Items.Add(currentGatewayName);
                    if (paymentGateway.Name == currentGatewayName)
                    {
                        drp_GatewayName.SelectedIndex = gatewayIndex;
                    }
                    gatewayIndex++;

                }

            }

        }

        drp_GatewayName.Attributes.Add("onchange", "UpdateOptions(this);");
        if (paymentGateway.Id == 0 && drp_GatewayName.Items.Count > 0)
        {

            if (!cmsPaymentManager.Providers[defaultName].SupportsCreditCardPayments)
            {
                chk_cc.Enabled = false;
            }
            if (!cmsPaymentManager.Providers[defaultName].SupportsCheckPayments)
            {
                chk_check.Enabled = false;
            }
            drp_GatewayName.SelectedIndex = 0;

        }

        // txt_name.Enabled = ((m_iID = 0) Or (m_iID > 0 And paymentGateway.IsCustom))
        lbl_id.Text = paymentGateway.Id.ToString();
        chk_default.Checked = paymentGateway.IsDefault;
        txt_uid.Text = paymentGateway.UserId;
        txt_viewpwd.Visible = false;
        txt_pwd.Text = ProtectPassword(paymentGateway.Password);
        txt_spare1.Text = paymentGateway.CustomFieldOne;
        txt_spare2.Text = paymentGateway.CustomFieldTwo;
        chk_cc.Checked = paymentGateway.AllowsCreditCardPayments;
        chk_check.Checked = paymentGateway.AllowsCheckPayments;
        tr_id.Visible = m_iID > 0;
        chk_default.Enabled = m_iID == 0;

    }

    protected void Display_View_All()
    {
        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        System.Collections.Generic.List<PaymentGatewayData> gatewayList;
        Ektron.Cms.Common.Criteria<PaymentGatewayProperty> paymentCriteria = new Ektron.Cms.Common.Criteria<PaymentGatewayProperty>();

        paymentCriteria.PagingInfo.CurrentPage = _currentPageNumber;
        paymentCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;

        gatewayList = gatewayService.GetList(paymentCriteria);
        TotalPagesNumber = System.Convert.ToInt32(paymentCriteria.PagingInfo.TotalPages);

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
        dg_gateway.DataSource = gatewayList;
        dg_gateway.Columns[1].HeaderText = GetMessage("generic Gateway");
        dg_gateway.Columns[2].HeaderText = GetMessage("generic id");
        dg_gateway.Columns[3].HeaderText = GetMessage("lbl gateway default");
        dg_gateway.Columns[4].HeaderText = GetMessage("lbl custom");
        dg_gateway.Columns[5].HeaderText = GetMessage("lbl gateway userid");
        dg_gateway.Columns[6].HeaderText = GetMessage("lbl commerce payment option cc");
        dg_gateway.Columns[7].HeaderText = GetMessage("lbl commerce payment option check");
        dg_gateway.DataBind();

    }


    #endregion

    #region Process


    protected void Process_EditOptions()
    {

        SiteAPI m_refSiteApi = new SiteAPI();
        PaymentSettingsData paymentOptions = new PaymentSettingsData();

        paymentOptions.PayPal = System.Convert.ToBoolean(chk_paypal.Checked);
        //paymentOptions.GoogleCheckout = chk_google.Checked

        m_refSiteApi.UpdatePaymentOptions(paymentOptions);

        Response.Redirect(this.m_sPageName + (m_iID > 0 ? "?action=view&id=" + this.m_iID : ""), false);

    }

    protected void Process_MarkDefault()
    {
        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        gatewayService.MarkAsDefault(this.m_iID);
        Response.Redirect(this.m_sPageName, false);
    }

    protected void Process_Delete()
    {
        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        gatewayService.Delete(this.m_iID);
        Response.Redirect(this.m_sPageName, false);
    }

    protected void Process_AddEdit()
    {
        IPaymentGateway gatewayService = ObjectFactory.GetPaymentGateway();
        PaymentGatewayData paymentGateway = null;
        paymentGateway = m_iID > 0 ? (gatewayService.GetItem(this.m_iID)) : (new PaymentGatewayData());
        paymentGateway.Name = (string)this.drp_GatewayName.SelectedValue;
        paymentGateway.IsDefault = System.Convert.ToBoolean(this.chk_default.Checked);
        paymentGateway.UserId = (string)this.txt_uid.Text;
        if (this.m_iID > 0 && this.txt_pwd.Text.Trim().Length == 0)
        {
            // no change to password
            // paymentGateway.Password = paymentGateway.Password
        }
        else
        {
            paymentGateway.Password = (string)this.txt_pwd.Text;
        }
        paymentGateway.CustomFieldOne = (string)txt_spare1.Text;
        paymentGateway.CustomFieldTwo = (string)txt_spare2.Text;
        paymentGateway.AllowsCreditCardPayments = System.Convert.ToBoolean(chk_cc.Checked);
        paymentGateway.AllowsCheckPayments = System.Convert.ToBoolean(chk_check.Checked);

        if (paymentGateway.Id > 0)
        {
            gatewayService.Update(paymentGateway);
        }
        else
        {
            gatewayService.Add(paymentGateway);
        }

        Response.Redirect(this.m_sPageName + (m_iID > 0 ? "?action=view&id=" + this.m_iID : ""), false);
    }

    #endregion

    #region Helpers

    protected void SetLabels()
    {

        this.litPaymentOptions.Text = GetMessage("lbl commerce payment options");
        this.litPaymentGatways.Text = GetMessage("lbl payment gateways");

        this.ltr_name.Text = this.GetMessage("lbl gateway name");
        this.ltr_id.Text = this.GetMessage("lbl gateway id");
        this.ltr_default.Text = this.GetMessage("lbl gateway default");
        this.ltr_uid.Text = this.GetMessage("lbl gateway userid");
        this.ltr_pwd.Text = this.GetMessage("lbl gateway password");
        this.ltr_showcustom.Text = "<a href=\"javascript: void(0);\" onclick=\"ToggleDiv(\'tbl_custom\');\">" + GetMessage("lbl gateway expand custom") + "</a>";
        this.ltr_spare1.Text = this.GetMessage("lbl gateway custom1");
        this.ltr_spare2.Text = this.GetMessage("lbl gateway custom2");
        switch (base.m_sPageAction)
        {
            case "view":

                this.pnl_view.Visible = true;
                this.pnl_viewall.Visible = false;
                this.AddBackButton(this.m_sPageName);
                this.AddButtonwithMessages(imageIconsPath + "contentEdit.png", this.m_sPageName + "?action=addedit&id=" + this.m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper. EditButtonCssClass, true);
                if (!m_bIsDefault)
                {
                    this.AddButtonwithMessages(this.AppImgPath + "icon_survey_enable.gif", this.m_sPageName + "?action=markdef&id=" + this.m_iID.ToString(), "lbl gateway mark def", "lbl gateway mark def", "", StyleHelper.EnableButtonCssClass);
                    this.AddButtonwithMessages(imageIconsPath + "delete.png", this.m_sPageName + "?action=del&id=" + this.m_iID.ToString(), "alt del gateway button text", "btn delete", " onclick=\"return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
                }
                this.SetTitleBarToMessage("lbl view gateway");
                this.AddHelpButton("ViewPaymentGateway");
                break;

            case "addedit":

                this.pnl_view.Visible = true;
                this.pnl_viewall.Visible = false;
                if (this.m_iID > 0)
                {
					this.AddBackButton(this.m_sPageName + "?action=view&id=" + m_iID.ToString());
					this.AddButtonwithMessages(imageIconsPath + "save.png", "#", "lbl alt save gateway", "btn save", " onclick=\"SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                    this.SetTitleBarToMessage("lbl edit gateway");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "protectpwd", "ProtectPassword();", true);
                    this.AddHelpButton("EditPaymentGateway");
                }
                else
                {
					this.AddBackButton(this.m_sPageName);
					this.AddButtonwithMessages(imageIconsPath + "save.png", "#", "lbl alt add gateway", "btn save", " onclick=\"SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                    this.SetTitleBarToMessage("lbl add gateway");
                    this.AddHelpButton("AddPaymentGateway");
                }
                break;

            case "editoptions":

                AddBackButton(this.m_sPageName);
				AddButtonwithMessages(imageIconsPath + "save.png", "#", "lbl alt save payment options", "btn save", " onclick=\"SubmitOptionsForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                SetTitleBarToMessage("lbl payment options edit");
                AddHelpButton("paymentgateway");
                break;

            default: // "viewall"

                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl new"), imageIconsPath + "star.png");
                newMenu.AddLinkItem(this.AppImgPath + "menu/card.gif", this.GetMessage("lbl payment gateway"), this.m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);

                workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), imageIconsPath + "check.png");
                actionMenu.AddItem(this.AppImgPath + "icon_survey_enable.gif", this.GetMessage("lbl gateway mark def"), "CheckMarkAsDef();");
                actionMenu.AddBreak();
                actionMenu.AddLinkItem(imageIconsPath + "contentEdit.png", this.GetMessage("lbl payment options edit"), this.m_sPageName + "?action=editoptions");
                this.AddMenu(actionMenu);

                this.SetTitleBarToMessage("lbl payment options");
                this.AddHelpButton("paymentgateway");
                break;

        }

        SetJs();
    }

    private void SetJs()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);

        sbJS.Append("function UpdateOptions(dropdown)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("   var cards = SupportsCards(dropdown.selectedIndex);" + Environment.NewLine);
        sbJS.Append("   if (!cards) { document.getElementById(\'chk_cc\').disabled = true; document.getElementById(\'chk_cc\').checked = false; } " + Environment.NewLine);
        sbJS.Append("   else { document.getElementById(\'chk_cc\').disabled = false; }" + Environment.NewLine);

        sbJS.Append("   var checks = SupportsChecks(dropdown.selectedIndex);" + Environment.NewLine);
        sbJS.Append("   if (!checks) { document.getElementById(\'chk_check\').disabled = true; document.getElementById(\'chk_check\').checked = false; } " + Environment.NewLine);
        sbJS.Append("   else { document.getElementById(\'chk_check\').disabled = false; } " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SupportsCards(idx)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var cardsupport = new Array(); " + Environment.NewLine);
        for (int i = 0; i <= (supportsCards.Count - 1); i++)
        {
            sbJS.Append("    cardsupport[" + i.ToString() + "] = " + supportsCards[i].ToString().ToLower() + ";" + Environment.NewLine);
        }
        sbJS.Append("    return cardsupport[idx]; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SupportsChecks(idx)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var checksupport = new Array(); " + Environment.NewLine);
        for (int i = 0; i <= (supportsChecks.Count - 1); i++)
        {
            sbJS.Append("    checksupport[" + i.ToString() + "] = " + supportsChecks[i].ToString().ToLower() + ";" + Environment.NewLine);
        }
        sbJS.Append("    return checksupport[idx]; " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckDelete()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    return confirm(\'").Append(GetMessage("js gateway confirm del")).Append("\');" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SubmitOptionsForm()" + Environment.NewLine);
        sbJS.Append("{ " + Environment.NewLine);
        sbJS.Append("    document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("    return false;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{ " + Environment.NewLine);

        sbJS.Append("} " + Environment.NewLine);

        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{var userID=document.getElementById(\'txt_uid\').value;var pwd=document.getElementById(\'txt_pwd\').value;" + Environment.NewLine);
        sbJS.Append("	if(userID.indexOf(\'<\') > -1 || userID.indexOf(\'>\') > -1 || pwd.indexOf(\'>\') > -1 || pwd.indexOf(\'<\') > -1) {").Append(Environment.NewLine);
        sbJS.Append("		alert(\"").Append(string.Format(GetMessage("js alert field cannot include"), "<, >")).Append("\");").Append(Environment.NewLine);
        sbJS.Append("		document.getElementById(\'").Append(txt_uid.UniqueID).Append("\').focus(); return false;").Append(Environment.NewLine);
        sbJS.Append("	} ").Append(Environment.NewLine);
        sbJS.Append("    document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("    return false;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckForillegalChar(txtName) {" + Environment.NewLine);
        sbJS.Append("   var val = txtName;" + Environment.NewLine);
        sbJS.Append("   if ((val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbJS.Append("   {" + Environment.NewLine);
        sbJS.Append("       alert(\"").Append(string.Format(GetMessage("js alert cc type name cant include"), "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')")).Append("\");" + Environment.NewLine);
        sbJS.Append("       return false;" + Environment.NewLine);
        sbJS.Append("   }" + Environment.NewLine);
        sbJS.Append("   return true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function CheckMarkAsDef() {" + Environment.NewLine);
        sbJS.Append(" 	var chosen = \'\'; ").Append(Environment.NewLine);
        sbJS.Append(" 	var len = document.forms[0].radio_gateway.length; ").Append(Environment.NewLine);
        sbJS.Append(" 	if (len > 0) { ").Append(Environment.NewLine);
        sbJS.Append(" 	    for (i = 0; i < len; i++) { ").Append(Environment.NewLine);
        sbJS.Append(" 		    if (document.forms[0].radio_gateway[i].checked) { ").Append(Environment.NewLine);
        sbJS.Append(" 			    chosen = document.forms[0].radio_gateway[i].value; ").Append(Environment.NewLine);
        sbJS.Append(" 		    } ").Append(Environment.NewLine);
        sbJS.Append(" 	    } ").Append(Environment.NewLine);
        sbJS.Append(" 	} else { ").Append(Environment.NewLine);
        sbJS.Append(" 	    if (document.forms[0].radio_gateway.checked) { chosen = document.forms[0].radio_gateway.value; } ").Append(Environment.NewLine);
        sbJS.Append(" 	} ").Append(Environment.NewLine);
        sbJS.Append(" 	if (chosen == \'\') { ").Append(Environment.NewLine);
        sbJS.Append(" 		alert(\'").Append(GetMessage("js please choose gateway")).Append("\'); ").Append(Environment.NewLine);
        sbJS.Append(" 	} else if (confirm(\'").Append(GetMessage("js gateway mark def")).Append("\')) { ").Append(Environment.NewLine);
        sbJS.Append(" 		window.location.href = \'paymentgateway.aspx?action=markdef&id=\' + chosen; ").Append(Environment.NewLine);
        sbJS.Append(" 	} ").Append(Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function ProtectPassword() {" + Environment.NewLine);
        sbJS.Append("   var objtitle = document.getElementById(\"").Append(txt_pwd.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   objtitle.value = \'          \';" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append(JSLibrary.ToggleDiv());

        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

    public string ProtectPassword(string pwd)
    {
        return "**********";
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

    protected void Util_SetPaymentCheckBoxes()
    {

        if (!Page.IsPostBack)
        {

            appSettings = (new SiteAPI()).GetSiteVariables(_uapi.UserId);

            chk_paypal.Checked = appSettings.PaymentSettings.PayPal;
            //chk_google.Checked = appSettings.PaymentSettings.GoogleCheckout

        }

    }

    protected void Util_HidePagingLinks()
    {

        PageLabel.Visible = false;
        CurrentPage.Visible = false;
        OfLabel.Visible = false;
        TotalPages.Visible = false;

        FirstPage.Visible = false;
        lnkBtnPreviousPage.Visible = false;
        NextPage.Visible = false;
        LastPage.Visible = false;

    }

    #endregion

    #region JS/CSS

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, base.m_refContentApi.ApplicationPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronMenuJs");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

    }

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, base.m_refContentApi.ApplicationPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronWaMenuCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

    }

    #endregion

}


