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
using Ektron.Cms.API;

public partial class Commerce_shipping_methods : workareabase, ICallbackEventHandler
{
    protected ShippingMethodApi m_refShipping = null;
    protected string m_sPageName = "shippingmethods.aspx";
    protected int _currentPageNumber = 1;
    protected int TotalPagesNumber = 1;

    #region Page Functions


    protected override void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            base.Page_Load(sender, e);
            if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
            {
                throw (new Exception(GetMessage("feature locked error")));
            }
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            CommerceLibrary.CheckCommerceAdminAccess();

            m_refShipping = new ShippingMethodApi();
            dg_viewall.Columns[0].HeaderText = this.GetMessage("generic id");
            dg_viewall.Columns[1].HeaderText = this.GetMessage("generic name");
            dg_viewall.Columns[2].HeaderText = this.GetMessage("sam order");
            dg_viewall.Columns[3].HeaderText = this.GetMessage("generic Service");
            switch (this.m_sPageAction)
            {
                case "addedit":
                    if (Utilities.IsInternalPostback && !(Page.IsCallback))
                    {
                        Process_AddEdit();
                    }
                    else if (!(Page.IsCallback))
                    {
                        Display_AddEdit();
                    }
                    break;

                case "reorder":

                    Reorder1.Initialize(m_refShipping.RequestInformationRef);
                    if (Page.IsPostBack && !(Page.IsCallback))
                    {
                        Process_Reorder();
                    }
                    else
                    {
                        Display_Reorder();
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

    protected void Process_Reorder()
    {

        string[] itemList = Strings.Split(Request.Form["LinkOrder"], ",", -1, 0);

        for (int i = 0; i <= (itemList.Length - 1); i++)
        {

            string[] itemArray = (itemList[i]).Split('|');

            if (itemArray.Length > 0 && Information.IsNumeric(itemArray[0]))
            {

                ShippingMethodData shipOption = m_refShipping.GetItem(Convert.ToInt64(itemArray[0]));

                shipOption.DisplayOrder = System.Convert.ToInt32(i + 1);

                m_refShipping.Update(shipOption);

            }

        }

        Page.Response.Write("<script language=\"javascript\">parent.location.href = \'shippingmethods.aspx\';</script>");

    }
    protected void Process_AddEdit()
    {
        ShippingMethodData shipOption = new ShippingMethodData();
        if (m_iID > 0)
        {
            shipOption = m_refShipping.GetItem(this.m_iID);
        }

        shipOption.Name = (string)txt_name.Text;
        shipOption.IsActive = System.Convert.ToBoolean(chk_active.Checked);
        shipOption.ProviderService = (string)txt_provservice.Text;
        if (this.m_iID > 0)
        {
            m_refShipping.Update(shipOption);
            Response.Redirect(m_sPageName + "?action=view&id=" + m_iID.ToString(), false);
        }
        else
        {
            m_refShipping.Add(shipOption);
            Response.Redirect(m_sPageName, false);
        }
    }
    protected void Process_Delete()
    {
        if (this.m_iID > 0)
        {
            m_refShipping.Delete(m_iID);
        }
        Response.Redirect(m_sPageName, false);
    }
    #endregion

    #region Display
    protected void Display_Reorder()
    {

        List<ShippingMethodData> optionList;
        Criteria<ShippingMethodProperty> criteria = new Criteria<ShippingMethodProperty>();

        criteria.PagingInfo.RecordsPerPage = 1000;
        criteria.PagingInfo.CurrentPage = 1;
        criteria.OrderByField = ShippingMethodProperty.DisplayOrder;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;

        optionList = m_refShipping.GetList(criteria);

        for (int i = 0; i <= (optionList.Count - 1); i++)
        {

            Reorder1.AddItem(optionList[i].Name, optionList[i].Id, 0);

        }

        pnl_reorder.Visible = true;
        pnl_viewall.Visible = false;

    }
    protected void Display_AddEdit()
    {
        ShippingMethodData shipOption = new ShippingMethodData();
        if (m_iID > 0)
        {
            shipOption = m_refShipping.GetItem(this.m_iID);
        }

        txt_name.Text = shipOption.Name;
        lbl_id.Text = shipOption.Id.ToString();
        tr_id.Visible = m_iID > 0;
        chk_active.Checked = shipOption.IsActive;
        txt_provservice.Text = shipOption.ProviderService;

        pnl_view.Visible = true;
        pnl_viewall.Visible = false;
        ltr_viewopt.Text = "&nbsp;<a href=\"#\" onclick=\"GetServiceOptions();\">" + m_refMsg.GetMessage("lbl view options") + "</a>";
    }
    protected void Display_All()
    {

        List<ShippingMethodData> optionList;
        Criteria<ShippingMethodProperty> criteria = new Criteria<ShippingMethodProperty>();

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;
        criteria.OrderByField = ShippingMethodProperty.DisplayOrder;
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;

        optionList = m_refShipping.GetList(criteria);

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

        dg_viewall.DataSource = optionList;
        dg_viewall.DataBind();

    }
    protected void Display_View()
    {
        ShippingMethodData shipOption = new ShippingMethodData();

        shipOption = m_refShipping.GetItem(this.m_iID);

        txt_name.Text = shipOption.Name;
        lbl_id.Text = shipOption.Id.ToString();
        chk_active.Checked = shipOption.IsActive;
        txt_provservice.Text = shipOption.ProviderService;

        txt_name.Enabled = false;
        chk_active.Enabled = false;
        txt_provservice.Enabled = false;

        pnl_view.Visible = true;
        pnl_viewall.Visible = false;
    }
    #endregion

    #region Private Helpers
    protected void Util_SetLabels()
    {

        ltr_appPath.Text = this.m_refContentApi.AppPath;

        switch (this.m_sPageAction)
        {
            case "addedit":
                AddBackButton(m_sPageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(AppImgPath + "../UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\" return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit shipping method");
                    AddHelpButton("EditShippingMethod");
                }
                else
                {
                    SetTitleBarToMessage("lbl add shipping method");
                    AddHelpButton("AddShippingMethod");
                }
                break;
            case "view":
                AddBackButton(m_sPageName);
				this.AddButtonwithMessages(AppImgPath + "../UI/Icons/contentEdit.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
				this.AddButtonwithMessages(AppImgPath + "../UI/Icons/delete.png", m_sPageName + "?action=del&id=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete shipping method") + "\');\" ", StyleHelper.DeleteButtonCssClass, true);
                SetTitleBarToMessage("lbl view shipping method");
                AddHelpButton("ViewShippingMethod");
                break;

            case "reorder":

                workareamenu actionMenu_1 = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                actionMenu_1.AddItem(m_refContentApi.AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), "document.forms[0].submit();");
                actionMenu_1.AddBreak();
                actionMenu_1.AddItem(m_refContentApi.AppPath + "images/ui/icons/cancel.png", this.GetMessage("generic cancel"), "parent.$ektron(\'.ektronShippingReorderModal\').modalHide();");
                this.AddMenu(actionMenu_1);

                SetTitleBarToMessage("lbl reorder reorder shipping methods");
                AddHelpButton("ReorderShippingMethods");
                break;

            default:
                workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), m_refContentApi.AppPath + "images/UI/Icons/star.png");
                newMenu.AddLinkItem(m_refContentApi.AppPath + "images/UI/Icons/filetypes/text.png", GetMessage("lbl shipping method"), m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);

                workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), m_refContentApi.AppPath + "images/UI/Icons/check.png");
                actionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/collection.png", this.GetMessage("btn reorder"), "OpenReorder();");
                this.AddMenu(actionMenu);

                SetTitleBarToMessage("lbl shipping methods");
                AddHelpButton("ShippingMethods");
                break;
        }

        ltr_name.Text = GetMessage("generic name");
        ltr_id.Text = GetMessage("generic id");
        ltr_active.Text = GetMessage("lbl active");
        ltr_provservice.Text = GetMessage("lbl provider service");
    }
    protected void Util_SetJS()
    {

        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "controls/Reorder/js/Reorder.js", "EktronReorderJs");

        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);

        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javascript\">").Append(Environment.NewLine);

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine);
        sbJS.Append(JSLibrary.AddError("aSubmitErr"));
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"));
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"));
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection));

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine);
        sbJS.Append("   var sTitle = Trim(document.getElementById(\'").Append(txt_name.UniqueID).Append("\').value); ").Append(Environment.NewLine);
        sbJS.Append("   if (sTitle == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err shipping method title req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl shipping method disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_provservice.UniqueID).Append("\',\"").Append(GetMessage("lbl shipping method provider service disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\');").Append(Environment.NewLine);
        sbJS.Append("   return false; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function UpdateOptions(result, context) { ").Append(Environment.NewLine);
        sbJS.Append("   document.getElementById(\'dvOptions\').innerHTML = result; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function UpdateService(selvalue) { ").Append(Environment.NewLine);
        sbJS.Append("   document.getElementById(\'").Append(txt_provservice.UniqueID).Append("\').value = selvalue; ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function GetServiceOptions() { ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(this.ClientScript.GetCallbackEventReference(this, "", "UpdateOptions", "null")).Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);
        ltr_js.Text += sbJS.ToString();
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
        Display_All();
        isPostData.Value = "true";
    }
    #endregion
    #region CallBack
    string callbackresult = "";
    public string GetCallbackResult()
    {
        return callbackresult;
    }
    public void RaiseCallbackEvent(string eventArgument)
    {
        try
        {
            List<string> aServiceTypes;
            Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager shipProvider = new Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager();

            aServiceTypes = Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager.Provider.GetServiceTypes();
            callbackresult = "<label class=\"label\">" + m_refMsg.GetMessage("lbl generic from") + " " + Ektron.Cms.Commerce.Shipment.Provider.ShipmentProviderManager.Provider.Name + "</label>:<br /><br /><select id=\'drp_options\' onchange=\'UpdateService(this.value);\'>";
            callbackresult += "<option value=\'\'>" + GetMessage("generic select") + "</option>";
            for (int i = 0; i <= (aServiceTypes.Count - 1); i++)
            {
                callbackresult += "<option value=\'" + aServiceTypes[i] + "\'>" + aServiceTypes[i] + "</option>";
            }
            callbackresult += "</select>";
        }
        catch (Exception ex)
        {
            callbackresult = "<img src=\"" + AppImgPath + "alert.gif\"><span class=\"important\">" + ex.Message + "</span>";
        }

    }
    #endregion

}


