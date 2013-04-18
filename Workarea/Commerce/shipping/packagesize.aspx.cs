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

public partial class Commerce_shipping_packagesize : workareabase
{
    public Commerce_shipping_packagesize()
    {
        measurementSystem = this.m_refContentApi.RequestInformationRef.MeasurementSystem.ToString();
    }

    protected Criteria<CountryProperty> criteria = new Ektron.Cms.Common.Criteria<CountryProperty>(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
    protected PackageApi m_refPackage = null;
    protected string m_sPageName = "packagesize.aspx";
    protected int _currentPageNumber = 1;
    protected bool m_bIsDefault = false;
    protected int TotalPagesNumber = 1;
    protected string measurementSystem;

    #region Page Function
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
        CommerceLibrary.CheckCommerceAdminAccess();

        try
        {
            Util_RegisterResources();

            switch (base.m_sPageAction)
            {
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
    #endregion
    #region Display
    protected void Display_View_All()
    {
        Ektron.Cms.Common.Criteria<PackageProperty> PackageCriteria = new Ektron.Cms.Common.Criteria<PackageProperty>(PackageProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        System.Collections.Generic.List<PackageData> packagelist = new System.Collections.Generic.List<PackageData>();
        m_refPackage = new PackageApi();
        int i = 0;

        dg_package.AutoGenerateColumns = false;
        dg_package.Columns.Clear();

        PackageCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        PackageCriteria.PagingInfo.CurrentPage = _currentPageNumber;

        packagelist = m_refPackage.GetList(PackageCriteria);

        TotalPagesNumber = System.Convert.ToInt32(PackageCriteria.PagingInfo.TotalPages);

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
        colBound.HeaderText = m_refMsg.GetMessage("generic id");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Name";
        colBound.HeaderText = m_refMsg.GetMessage("generic name");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Length";
        colBound.HeaderText = m_refMsg.GetMessage("lbl length");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);
        dg_package.BorderColor = System.Drawing.Color.White;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Height";
        colBound.HeaderText = m_refMsg.GetMessage("lbl height");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);
        dg_package.BorderColor = System.Drawing.Color.White;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Width";
        colBound.HeaderText = m_refMsg.GetMessage("lbl width");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);
        dg_package.BorderColor = System.Drawing.Color.White;

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "Weight";
        colBound.HeaderText = m_refMsg.GetMessage("lbl maxweight");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
        colBound.HeaderStyle.CssClass = "title-header";
        dg_package.Columns.Add(colBound);
        dg_package.BorderColor = System.Drawing.Color.White;

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("Length", typeof(string)));
        dt.Columns.Add(new DataColumn("Height", typeof(string)));
        dt.Columns.Add(new DataColumn("Width", typeof(string)));
        dt.Columns.Add(new DataColumn("Weight", typeof(string)));

        if (!(packagelist == null))
        {
            for (i = 0; i <= packagelist.Count - 1; i++)
            {
                dr = dt.NewRow();
                string dimensionUnit = "";
                string weightUnit = "";
                if (measurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English.ToString())
                {
                    dimensionUnit = m_refMsg.GetMessage("lbl inches");
                    weightUnit = m_refMsg.GetMessage("lbl pounds");
                }
                else
                {
                    dimensionUnit = m_refMsg.GetMessage("lbl centimeters");
                    weightUnit = m_refMsg.GetMessage("lbl kilograms");
                }

                dr[0] = "<a href=\"packagesize.aspx?action=View&id=" + packagelist[i].Id + "\">" + packagelist[i].Id + "</a>";
                dr[1] = "<a href=\"packagesize.aspx?action=View&id=" + packagelist[i].Id + "\">" + packagelist[i].Name + "</a>";
                dr[2] = "<label id=\"length\">" + packagelist[i].Dimensions.Length + "&nbsp;" + dimensionUnit + "</label>";
                dr[3] = "<label id=\"height\">" + packagelist[i].Dimensions.Height + "&nbsp;" + dimensionUnit + "</label>";
                dr[4] = "<label id=\"width\">" + packagelist[i].Dimensions.Width + "&nbsp;" + dimensionUnit + "</label>";
                dr[5] = "<label id=\"weight\">" + packagelist[i].MaxWeight.Amount + "&nbsp;" + weightUnit + "</label>";

                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        dg_package.DataSource = dv;
        dg_package.DataBind();
    }
    protected void Display_View()
    {
        PackageData package = null;
        m_refPackage = new PackageApi();
        package = m_refPackage.GetItem(this.m_iID);

        txt_package_name.Text = package.Name;
        lbl_package_id.Text = package.Id.ToString();
        txt_package_length.Text = package.Dimensions.Length.ToString();
        txt_package_height.Text = package.Dimensions.Height.ToString();
        txt_package_width.Text = package.Dimensions.Width.ToString();
        txt_package_weight.Text = package.MaxWeight.Amount.ToString();

        if (measurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English.ToString())
        {
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl pounds");
        }
        else
        {
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl kilograms");
        }

        txt_package_name.Enabled = false;
        txt_package_length.Enabled = false;
        txt_package_height.Enabled = false;
        txt_package_width.Enabled = false;
        txt_package_weight.Enabled = false;

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
        PackageData package = null;
        m_refPackage = new PackageApi();
		
	    ltr_package_id_tr.Visible = this.m_iID > 0 ? true : false;

        if (this.m_iID > 0)
        {
            package = m_refPackage.GetItem(this.m_iID);
            txt_package_name.Text = package.Name;
            lbl_package_id.Text = package.Id.ToString();
            txt_package_length.Text = package.Dimensions.Length.ToString();
            txt_package_height.Text = package.Dimensions.Height.ToString();
            txt_package_width.Text = package.Dimensions.Width.ToString();
            txt_package_weight.Text = package.MaxWeight.Amount.ToString();
        }

        txt_package_name.Enabled = true;
        txt_package_length.Enabled = true;
        txt_package_height.Enabled = true;
        txt_package_width.Enabled = true;
        txt_package_weight.Enabled = true;

        if (measurementSystem == Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English.ToString())
        {
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl inches");
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl pounds");
        }
        else
        {
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl centimeters");
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl kilograms");
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
    #endregion

    #region Process
    protected void Process_AddEdit()
    {
        PackageData package = null;
        m_refPackage = new PackageApi();

        if (this.m_iID > 0)
        {
            package = m_refPackage.GetItem(this.m_iID);
            package.Name = (string)txt_package_name.Text;
            package.Dimensions.Length = Convert.ToSingle(txt_package_length.Text);
            package.Dimensions.Height = Convert.ToSingle(txt_package_height.Text);
            package.Dimensions.Width = Convert.ToSingle(txt_package_width.Text);
            package.MaxWeight.Amount = Convert.ToSingle(txt_package_weight.Text);
            if ((string)ltr_height_unit.Text == "Inches")
            {
                package.Dimensions.Units = LinearUnit.Inches;
            }
            else if ((string)ltr_height_unit.Text == "Centimeters")
            {
                package.Dimensions.Units = LinearUnit.Centimeters;
            }
            if ((string)ltr_weight_unit.Text == "Pounds")
            {
                package.MaxWeight.Units = WeightUnit.Pounds;
            }
            else if ((string)ltr_weight_unit.Text == "Kilograms")
            {
                package.MaxWeight.Units = WeightUnit.Kilograms;
            }

            m_refPackage.Update(package);
            Response.Redirect(m_sPageName + "?action=view&id=" + this.m_iID.ToString(), false);
        }
        else
        {
            Ektron.Cms.Commerce.Dimensions Dimension = new Ektron.Cms.Commerce.Dimensions();
            Ektron.Cms.Commerce.Weight maxWeight = new Ektron.Cms.Commerce.Weight();

            Dimension.Length = Convert.ToSingle(txt_package_length.Text);
            Dimension.Height = Convert.ToSingle(txt_package_height.Text);
            Dimension.Width = Convert.ToSingle(txt_package_width.Text);

            package = new PackageData(txt_package_name.Text, Dimension, maxWeight);

            if ((string)ltr_length_unit.Text == "Inches")
            {
                package.Dimensions.Units = LinearUnit.Inches;
            }
            else if ((string)ltr_length_unit.Text == "Centimeters")
            {
                package.Dimensions.Units = LinearUnit.Centimeters;
            }
            if ((string)ltr_weight_unit.Text == "Pounds")
            {
                package.MaxWeight.Units = WeightUnit.Pounds;
            }
            else if ((string)ltr_weight_unit.Text == "Kilograms")
            {
                package.MaxWeight.Units = WeightUnit.Kilograms;
            }
            package.MaxWeight.Amount = Convert.ToSingle(txt_package_weight.Text);

            m_refPackage.Add(package);
            Response.Redirect(m_sPageName, false);
        }
    }
    protected void Process_Delete()
    {
        PackageApi package = null;
        package = new PackageApi();
        if (this.m_iID > 0)
        {
            package.Delete(this.m_iID);
            Response.Redirect(this.m_sPageName, false);
        }
    }
    #endregion
    #region Set And Navigation

    protected void SetLabels()
    {
        this.ltr_package_name.Text = this.GetMessage("generic name");
        this.ltr_package_id.Text = this.GetMessage("generic id");
        this.ltr_package_length.Text = this.GetMessage("lbl length");
        this.ltr_package_width.Text = this.GetMessage("lbl width");
        this.ltr_package_height.Text = this.GetMessage("lbl height");
        this.ltr_package_weight.Text = this.GetMessage("lbl maxweight");

        switch (base.m_sPageAction)
        {

            case "addedit":
                this.pnl_viewaddress.Visible = true;
                this.AddBackButton(this.m_sPageName + (this.m_iID > 0 ? ("?action=view&id=" + this.m_iID.ToString()) : ""));
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/save.png", m_sPageName + "?action=addedit&id=" + this.m_iID.ToString(), "lbl alt save package", "btn save", " onclick=\" return SubmitForm(); \" ", StyleHelper.SaveButtonCssClass, true);
                if (this.m_iID > 0)
                {
                    this.SetTitleBarToMessage("lbl edit package");
                    this.AddHelpButton("EditPackages");
                }
                else
                {
                    this.SetTitleBarToMessage("lbl add package");
                    this.AddHelpButton("Addpackage");
                }
                break;

            case "view":
                this.pnl_viewall.Visible = false;
                this.pnl_viewaddress.Visible = true;
                this.AddBackButton(this.m_sPageName);
				this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/contentEdit.png", this.m_sPageName + "?action=addedit&id=" + this.m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
				this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/delete.png", this.m_sPageName + "?action=del&id=" + this.m_iID.ToString(), "alt del package button text", "btn delete", " onclick=\" return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
                this.SetTitleBarToMessage("lbl view package");
                this.AddHelpButton("Viewpackage");
                break;

            default: // "viewall"
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl new"), this.AppImgPath + "../UI/Icons/star.png");
                newMenu.AddLinkItem(this.AppImgPath + "menu/card.gif", this.GetMessage("lbl package"), this.m_sPageName + "?action=addedit");
                this.AddMenu(newMenu);

                this.SetTitleBarToMessage("lbl packages");
                this.AddHelpButton("packages");
                break;
        }
        SetJs();
    }
    private void SetJs()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);

        sbJS.Append(" var deletePackageMsg = \'").Append(GetMessage("js package confirm del")).Append("\';" + Environment.NewLine);
        sbJS.Append(" var badPackageDimensionMsg = \'").Append(GetMessage("js alert package dimension value 5_2")).Append("\';" + Environment.NewLine);
        sbJS.Append(" var emptyPackageMsg = \'").Append(GetMessage("js null package msg")).Append("\';" + Environment.NewLine);
        sbJS.Append(" var badPackageNameMsg = \"").Append(GetMessage("js alert package name cant include")).Append(" (\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')\";" + Environment.NewLine);

        sbJS.Append(JSLibrary.ToggleDiv());

        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
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
    private void Util_RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
    }
}

