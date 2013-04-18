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

public partial class Commerce_tax_taxclass : workareabase
{

    protected TaxClass m_refTaxClass = null;
    protected string m_sPageName = "taxclass.aspx";
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

        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        Util_CheckAccess();
        AppPath = m_refContentApi.ApplicationPath;

        try
        {
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

            m_refTaxClass = new TaxClass(this.m_refContentApi.RequestInformationRef);
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
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

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
        TaxClassData txClass = null;
        if (this.m_iID > 0)
        {
            txClass = m_refTaxClass.GetItem(this.m_iID);
            txClass.Name = (string)txt_name.Text;
            m_refTaxClass.Update(txClass);
            Response.Redirect(m_sPageName + "?action=view&id=" + m_iID.ToString(), false);
        }
        else
        {
            txClass = new TaxClassData(txt_name.Text);
            m_refTaxClass.Add(txClass);
            Response.Redirect(m_sPageName, false);
        }
    }
    protected void Process_Delete()
    {
        if (this.m_iID > 0)
        {
            m_refTaxClass.Delete(m_iID);
        }
        Response.Redirect(m_sPageName, false);
    }
    #endregion

    #region Display
    protected void Display_AddEdit()
    {
        TaxClassData txClass = new TaxClassData();
        if (m_iID > 0)
        {
            txClass = m_refTaxClass.GetItem(this.m_iID);
        }

        txt_name.Text = txClass.Name;
        lbl_id.Text = txClass.Id.ToString();
        tr_id.Visible = m_iID > 0;
        pnl_view.Visible = true;
        pnl_viewall.Visible = false;
    }
    protected void Display_All()
    {
        System.Collections.Generic.List<TaxClassData> TaxClassList = new System.Collections.Generic.List<TaxClassData>();
        Ektron.Cms.Common.Criteria<TaxClassProperty> criteria = new Ektron.Cms.Common.Criteria<TaxClassProperty>(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _currentPageNumber;

        TaxClassList = m_refTaxClass.GetList(criteria);

        TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);
        TotalPages.ToolTip = TotalPagesNumber.ToString();

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

        dg_viewall.DataSource = TaxClassList;
        dg_viewall.Columns[0].HeaderText = this.GetMessage("generic id");
        dg_viewall.Columns[1].HeaderText = this.GetMessage("generic name");
        dg_viewall.DataBind();
    }
    protected void Display_View()
    {
        TaxClassData txClass = null;

        txClass = m_refTaxClass.GetItem(this.m_iID);

        txt_name.Text = txClass.Name;
        lbl_id.Text = txClass.Id.ToString();

        this.txt_name.Enabled = false;
        pnl_view.Visible = true;
        pnl_viewall.Visible = false;
    }
    #endregion

    #region Private Helpers
    protected void Util_SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
				AddBackButton(m_sPageName + (m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "btn save", "btn save", " onclick=\" return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    SetTitleBarToMessage("lbl edit tax class");
                    AddHelpButton("Edittaxclass");
                }
                else
                {
                    SetTitleBarToMessage("lbl add tax class");
                    AddHelpButton("Addtaxclass");
                }
                break;
            case "view":
				AddBackButton(m_sPageName);
                this.AddButtonwithMessages(AppPath + "images/UI/Icons/contentEdit.png", m_sPageName + "?action=addedit&id=" + m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                if (!m_refTaxClass.IsUsed(m_iID) && m_iID != 5)
                {
					this.AddButtonwithMessages(AppPath + "images/UI/Icons/delete.png", m_sPageName + "?action=del&id=" + m_iID.ToString(), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm delete tax class") + "\');\" ", StyleHelper.DeleteButtonCssClass);
                }
                SetTitleBarToMessage("lbl view tax class");
                AddHelpButton("Viewtaxclass");
                break;
            default:
                workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), AppPath + "images/UI/Icons/star.png");
                newMenu.AddLinkItem(AppImgPath + "/menu/document.gif", GetMessage("lbl tax class"), m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);
                SetTitleBarToMessage("lbl tax classes");
                AddHelpButton("taxclass");
                break;
        }

        ltr_name.Text = GetMessage("generic name");
        ltr_id.Text = GetMessage("generic id");
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
        sbJS.Append("   if (sTitle == \'\') { ").Append(JSLibrary.AddErrorFunctionName).Append("(\'").Append(GetMessage("js err tax class title req")).Append("\'); } ").Append(Environment.NewLine);
        sbJS.Append("   HasIllegalChar(\'").Append(txt_name.UniqueID).Append("\',\"").Append(GetMessage("lbl tax class disallowed chars")).Append("\"); ").Append(Environment.NewLine);
        sbJS.Append(" } ").Append(Environment.NewLine);

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine);
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("(\'document.forms[0].submit();\');").Append(Environment.NewLine);
        sbJS.Append("   return false; ").Append(Environment.NewLine);
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

}


