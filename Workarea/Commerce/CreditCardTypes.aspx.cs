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
using Ektron.Cms.Common;

public partial class Commerce_cctypes : workareabase
{

    #region Member Variables

    protected string _PageName = "creditcardtypes.aspx";
    protected int _CurrentPageNumber = 1;
    protected int _TotalPagesNumber = 1;

    #endregion

    #region Events

    protected void Page_Init(object sender, System.EventArgs e)
    {

        //register page components
        this.RegisterCSS();
        this.RegisterJS();
        ChangeHeaderText(dg_cctypes);
    }
    private void ChangeHeaderText(DataGrid dg)
    {
        if (dg == null)
        {
            return;
        }

        foreach (DataGridColumn col in dg.Columns)
        {            
            if (col.HeaderText == "Id")
            {
                col.HeaderText = this.GetMessage("generic id");
            }
            if (col.HeaderText == "Name")
            {
                col.HeaderText = this.GetMessage("generic name");
            }
            if (col.HeaderText == "Accepted")
            {
                col.HeaderText = this.GetMessage("lbl cc type accepted");
            }
            if (col.HeaderText == "Image")
            {
                col.HeaderText = this.GetMessage("lbl cc type image");
            }
        }
    }
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            Util_CheckAccess();
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
            Util_SetLabels();
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Display
    protected void Display_View()
    {
        CreditCardApi ccApi = new CreditCardApi();
        CreditCardTypeData ccType = null;
        ccType = ccApi.GetItem(this.m_iID);

        txt_name.Text = ccType.Name;
        lbl_id.Text = ccType.Id.ToString();
        chk_accepted.Checked = ccType.IsAccepted;
        txt_regex.Text = ccType.Regex;

        cc_image.Text = Util_ShowImagePath(ccType.Image);
        cc_image_thumb.ImageUrl = Util_ShowImagePath(ccType.Image);
        if (cc_image_thumb.ImageUrl != "")
        {
            cc_image_thumb.ImageUrl = (Util_ShowImagePath(ccType.Image).IndexOf("/") == 0) ? (Util_ShowImagePath(ccType.Image)) : (m_refContentApi.SitePath + Util_ShowImagePath(ccType.Image));
        }
        else
        {
            cc_image_thumb.ImageUrl = AppImgPath + "spacer.gif";
        }
        pnl_edit.Visible = false;
        cc_image.Enabled = false;

        txt_name.Enabled = false;
        chk_accepted.Enabled = false;
        txt_regex.Enabled = false;
    }

    protected void Display_AddEdit()
    {
        CreditCardApi ccApi = new CreditCardApi();
        CreditCardTypeData ccType = null;
        ccType = m_iID > 0 ? (ccApi.GetItem(this.m_iID)) : (new CreditCardTypeData());

        txt_name.Text = ccType.Name;
        lbl_id.Text = ccType.Id.ToString();
        chk_accepted.Checked = ccType.IsAccepted;
        txt_regex.Text = ccType.Regex;

        cc_image.Text = Util_ShowImagePath(ccType.Image);
        cc_image_thumb.ImageUrl = Util_ShowImagePath(ccType.Image);
        if (cc_image_thumb.ImageUrl != "")
        {
            cc_image_thumb.ImageUrl = (Util_ShowImagePath(ccType.Image).IndexOf("/") == 0) ? (Util_ShowImagePath(ccType.Image)) : (m_refContentApi.SitePath + Util_ShowImagePath(ccType.Image));
        }
        else
        {
            cc_image_thumb.ImageUrl = AppImgPath + "spacer.gif";
        }

        tr_id.Visible = m_iID > 0;
    }

    protected void Display_View_All()
    {
        CreditCardApi ccApi = new CreditCardApi();
        Criteria<CreditCardTypeProperty> criteria = new Criteria<CreditCardTypeProperty>();
        List<CreditCardTypeData> ccList;

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = _CurrentPageNumber;

        ccList = ccApi.GetList(criteria);

        _TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);

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

        dg_cctypes.DataSource = ccList;
        dg_cctypes.DataBind();
    }
    #endregion

    #region Process

    protected void Process_Delete()
    {
        CreditCardApi ccApi = new CreditCardApi();
        ccApi.Delete(this.m_iID);
        Response.Redirect(this._PageName, false);
    }

    protected void Process_AddEdit()
    {
        CreditCardApi ccApi = new CreditCardApi();
        CreditCardTypeData ccType = null;
        ccType = m_iID > 0 ? (ccApi.GetItem(this.m_iID)) : (new CreditCardTypeData());
        ccType.Name = (string)this.txt_name.Text;
        ccType.IsAccepted = System.Convert.ToBoolean(this.chk_accepted.Checked);
        ccType.Image = Request.Form[cc_image.UniqueID];
        if (this.txt_regex.Text != "")
        {
            ccType.Regex = (string)this.txt_regex.Text;
        }
        else
        {
            // The following regular expression checks if the value entered for credit card is 16 digit long, all numbers and not empty.
            ccType.Regex = "\\d{4}-?\\d{4}-?\\d{4}-?\\d{4}";
        }

        if (ccType.Id > 0)
        {
            ccApi.Update(ccType);
        }
        else
        {
            ccApi.Add(ccType);
        }

        Response.Redirect(this._PageName + (m_iID > 0 ? "?action=view&id=" + this.m_iID : ""), false);
    }

    #endregion

    #region Util

    protected void Util_SetLabels()
    {
        ltr_name.Text = GetMessage("lbl cc type name");
        ltr_id.Text = GetMessage("lbl cc type id");
        ltr_accepted.Text = GetMessage("lbl cc type accepted");
        ltr_image.Text = GetMessage("lbl cc type image");
        ltr_regex.Text = GetMessage("lbl cc type regex");
        switch (base.m_sPageAction)
        {
            case "view":
                this.pnl_view.Visible = true;
                this.pnl_viewall.Visible = false;
                this.AddBackButton(this._PageName);
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/contentEdit.png", this._PageName + "?action=addedit&id=" + this.m_iID.ToString(), "generic edit title", "generic edit title", "", StyleHelper.EditButtonCssClass, true);
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/delete.png", this._PageName + "?action=del&id=" + this.m_iID.ToString(), "alt del cc type button text", "btn delete", " onclick=\"return CheckDelete();\" ", StyleHelper.DeleteButtonCssClass);
                this.SetTitleBarToMessage("lbl view cc type");
                this.AddHelpButton("ViewCreditCardType");
                break;
            case "addedit":
                // Me.ltr_cmd_img_prv.Text = "<img src=""" & AppImgPath & "btn_preview-nm.gif"" border=""0"" alt=""" & GetMessage("lbl cc type img review") & """ title=""" & GetMessage("lbl cc type img preview") & """ onclick="" PreviewImage(); return false;"" />"
                this.pnl_view.Visible = true;
                this.pnl_viewall.Visible = false;
				this.AddBackButton(this._PageName + (this.m_iID > 0 ? "?action=view&id=" + this.m_iID : ""));
                this.AddButtonwithMessages(this.AppImgPath + "../UI/Icons/save.png", "#", "lbl alt edit cc type", "btn save", " onclick=\"SubmitForm(); return false;\" ", StyleHelper.SaveButtonCssClass, true);
                this.SetTitleBarToString((string)(this.m_iID > 0 ? (this.GetMessage("lbl edit cc type")) : (this.GetMessage("lbl add cc type"))));
                this.AddHelpButton((string)(this.m_iID > 0 ? ("EditCreditCardType") : ("AddCreditCardType")));
                break;
            default: // "viewall"
                workareamenu newMenu = new workareamenu("file", this.GetMessage("lbl new"), this.AppImgPath + "../UI/Icons/star.png");
                newMenu.AddLinkItem(this.AppImgPath + "/menu/card.gif", this.GetMessage("lbl cc type"), this._PageName + "?action=addedit");
                this.AddMenu(newMenu);
                this.SetTitleBarToMessage("lbl cc types");
                this.AddHelpButton("cctype");
                break;
        }

        Util_SetJs();
    }

    private void Util_SetJs()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script language=\"javascript\" type=\"text/javascript\" >" + Environment.NewLine);

        sbJS.Append("function CheckDelete()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    return confirm(\'").Append(GetMessage("js cc type confirm del")).Append("\');" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("function SubmitForm()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var objtitle = document.getElementById(\"").Append(txt_name.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("	    if (!CheckForillegalChar(objtitle.value)) {" + Environment.NewLine);
        sbJS.Append("           objtitle.focus();" + Environment.NewLine);
        sbJS.Append("       } else {" + Environment.NewLine);
        sbJS.Append("           document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("	    }" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
        sbJS.Append("    else" + Environment.NewLine);
        sbJS.Append("    {" + Environment.NewLine);
        sbJS.Append("        alert(\"" + base.GetMessage("js null cc type msg") + "\");" + Environment.NewLine);
        sbJS.Append("        objtitle.focus();" + Environment.NewLine);
        sbJS.Append("    }" + Environment.NewLine);
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

        sbJS.Append("function PreviewImage()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("   var oPreview = document.getElementById(\"").Append(cc_image.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   oPreview.innerHTML = \'").Append(GetMessage("lbl cc type img previewing")).Append("\';" + Environment.NewLine);
        sbJS.Append("   var oImg = document.getElementById(\"").Append(cc_image.UniqueID).Append("\");" + Environment.NewLine);
        sbJS.Append("   var strImg = oImg.value;" + Environment.NewLine);
        sbJS.Append("   if(strImg.length > 0) {" + Environment.NewLine);
        sbJS.Append("       strImg = strImg.replace(/\\[apppath\\]/,\"").Append(this.m_refContentApi.ApplicationPath).Append("\");" + Environment.NewLine);
        sbJS.Append("       strImg = strImg.replace(/\\[appimgpath\\]/,\"").Append(this.m_refContentApi.AppImgPath).Append("\");" + Environment.NewLine);
        sbJS.Append("       strImg = strImg.replace(/\\[sitepath\\]/,\"").Append(this.m_refContentApi.SitePath).Append("\");" + Environment.NewLine);
        sbJS.Append("       oPreview.innerHTML = \'<img src=\"\' + strImg + \'\" alt=\"\" title=\"\" border=\"0\">\';" + Environment.NewLine);
        sbJS.Append("   } else { " + Environment.NewLine);
        sbJS.Append("       oPreview.innerHTML = \'\';" + Environment.NewLine);
        sbJS.Append("   } " + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("</script>" + Environment.NewLine);
        ltr_js.Text += Environment.NewLine + sbJS.ToString();
    }

    protected string Util_ShowImagePath(string image)
    {
        string sRet = "";
        if (image != "")
        {
            image = image.Replace("[apppath]", this.m_refContentApi.ApplicationPath);
            image = image.Replace("[appimgpath]", this.m_refContentApi.AppImgPath);
            image = image.Replace("[sitepath]", this.m_refContentApi.SitePath);
            sRet = image; // "<img src=""" & image & """ alt="""" title="""" border=""0"">"
        }
        return sRet;
    }

    protected string Util_ShowImage(string image)
    {
        string sRet = "";
        if (image != "")
        {
            image = image.Replace("[apppath]", this.m_refContentApi.ApplicationPath);
            image = image.Replace("[appimgpath]", this.m_refContentApi.AppImgPath);
            image = image.Replace("[sitepath]", this.m_refContentApi.SitePath);
            if (image.IndexOf("/") == 0)
            {
                if (!image.Contains(m_refContentApi.SitePath))
                {
                    image = image.Substring(1);
                    image = m_refContentApi.SitePath + image;
                }
            }
            else
            {
                image = m_refContentApi.SitePath + image;
            }
            sRet = "<img src=\"" + image + "\" alt=\"\" title=\"\" border=\"0\">";
        }
        return sRet;
    }

    protected void Util_CheckAccess()
    {
        if (!m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
        {
            throw (new Exception(GetMessage("err not role commerce-admin")));
        }
    }

    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
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
        Display_View_All();
        isPostData.Value = "true";
    }

    #endregion

    #region JS/CSS

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS");

    }

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

    #endregion

}


