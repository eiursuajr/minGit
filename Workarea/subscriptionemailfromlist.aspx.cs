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
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;

public partial class subscriptionemailfromlist : workareabase
{
    protected SiteAPI m_refSiteAPI = new SiteAPI();
    protected long m_intFontId = 0;
    protected long m_intEmailFromId = 0;
    protected FontData font_data;
    protected EmailFromData emailfrom_data;
    protected ContentAPI m_refContApi = new ContentAPI();
    protected string m_strSubscriptionNameFromUserControl;
    protected bool m_strSubscriptionEnableFromUserControl;
    protected int EnableMultilingual;
    protected long m_intId = 0;
    protected LanguageData[] colActiveLanguages;
    const string PAGE_NAME = "subscriptionemailfromlist.aspx";
    protected string imagePath = "";

    protected override void Page_Load(System.Object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        RegisterResources();

        // populate JS literals
        jsEmailRequired.Text = m_refMsg.GetMessage("alert msg email required");
        jsConfirmDeleteEmailFrom.Text = m_refMsg.GetMessage("js: confirm delete emailfrom");
        jsConfirmDeleteManyEmailFrom.Text = m_refMsg.GetMessage("js: confirm delete many emailfrom");
        jsPleaseSelectEmailFrom.Text = m_refMsg.GetMessage("js: please sel emailfrom");
        jsValidEmailAddress.Text = m_refMsg.GetMessage("js:enter valid email address");
        imagePath = m_refContentApi.AppPath + "images/ui/icons/";
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }

            Util_CheckAccess();
            TR_AddEditSubscription.Visible = false;
            TR_ViewSubscription.Visible = false;
            TR_ViewAllFont.Visible = false;
            if (!(Page.IsPostBack))
            {
                switch (m_sPageAction)
                {
                    case "viewallsubscriptions":
                        Display_ViewAllEmailFrom();
                        break;
                    case "view":
                        Display_ViewEmailFrom();
                        break;
                    case "edit":
                        Display_EditEmailFrom();
                        break;
                    case "add":
                        Display_AddEmailFrom();
                        break;
                    case "delete":
                        Process_DeleteEmailFrom();
                        break;
                    default:
                        Display_ViewAllEmailFrom();
                        break;
                }
            }
            else
            {
                switch (m_sPageAction)
                {
                    case "edit":
                        Process_EditEmailFrom();
                        break;
                    case "add":
                        Process_AddEmailFrom();
                        break;
                    case "delete":
                        Process_DeleteEmailFrom();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
        }
    }

    private void Process_EditEmailFrom()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(m_iID, "ID", null, null);
        pagedata.Add(Request.Form["txtName"], "Email", null, null);
        m_refContApi.UpdateEmailFrom(pagedata);
        ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();";
    }

    private void Process_AddEmailFrom()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(Request.Form["txtName"], "Email", null, null);
        m_refContApi.AddEmailFrom(pagedata);
        ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();";
    }

    private void Process_DeleteEmailFrom()
    {
        string[] IdList = Strings.Split(Request.QueryString["Ids"], ",", -1, 0);
        if (IdList.Length > 0)
        {
            for (int i = 0; i <= (IdList.Length - 1); i++)
            {
                if (Information.IsNumeric(IdList[i]))
                {
                    Collection pagedata = new Collection();
                    pagedata.Add(IdList[i], "ID", null, null);
                    m_refContApi.DeleteEmailFrom(pagedata);
                    pagedata = null;
                }
            }
        }

        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions"), false);
    }

    private void Display_EditEmailFrom()
    {
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        emailfrom_data = m_refContApi.GetEmailFromById(m_iID);
        txtName.Text = emailfrom_data.Email;
        ltrAddEditID.Text = emailfrom_data.Id.ToString() + "<input type=\"hidden\" name=\"subscriptionID\" value=\"" + emailfrom_data.Id.ToString() + "\"/>";
        EditEmailFromToolBar();
    }

    private void Display_AddEmailFrom()
    {
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        TD_SubscriptionID.Visible = false;
        AddEmailFromToolBar();
    }

    private void Display_ViewEmailFrom()
    {
        TR_ViewSubscription.Visible = true;
        emailfrom_data = m_refContApi.GetEmailFromById(m_iID);
        if (emailfrom_data != null)
        {
            ltrViewName.Text = emailfrom_data.Email;
            ltrViewID.Text = emailfrom_data.Id.ToString();
        }
        else
        {
            ltrViewName.Text = string.Empty;
            ltrViewID.Text = string.Empty;
        }

        ViewEmailFromToolBar();
    }

    private void Display_ViewAllEmailFrom()
    {
        TR_ViewAllFont.Visible = true;
        EmailFromData[] emailfrom_data_list;
        emailfrom_data_list = m_refContApi.GetAllEmailFrom();
        if (!(emailfrom_data_list == null))
        {
            // set the header text (column zero is checkbox):
            ViewSubscriptionGrid.Columns[1].HeaderText = GetMessage("generic email"); // email
            ViewSubscriptionGrid.Columns[2].HeaderText = GetMessage("generic id"); // id
            ViewSubscriptionGrid.DataSource = emailfrom_data_list;
            ViewSubscriptionGrid.DataBind();
        }

        ViewAllEmailFromToolBar();
    }

    private void AddEmailFromToolBar()
    {
        SetTitleBarToMessage("lbl add email from");
		AddButtonwithMessages(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions"), "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
		AddButtonwithMessages(imagePath + "save.png", "#", "lbl Add Email From Address", "btn save", "Onclick=\"javascript:return SubmitForm( \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true);
        AddHelpButton("AddEmailFrom");
    }

    private void ViewEmailFromToolBar()
    {
        SetTitleBarToMessage("lbl view email from");
		AddButtonwithMessages(imagePath + "contentEdit.png", System.Convert.ToString(PAGE_NAME + "?action=Edit&id=" + m_iID.ToString() + ""), "alt edit email from address", "btn edit", "", StyleHelper.EditButtonCssClass, true);
		AddButtonwithMessages(imagePath + "delete.png", System.Convert.ToString(PAGE_NAME + "?action=delete&SubscriptionID=" + m_iID.ToString() + ""), "alt delete email message", "btn delete", "OnClick=\"javascript: return ConfirmFontDelete();\"", StyleHelper.DeleteButtonCssClass);
        AddBackButton(System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions"));
        AddHelpButton("ViewEmailFrom");
    }

    private void ViewAllEmailFromToolBar()
    {
        SetTitleBarToMessage("alt view all email from addresses");
        workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), imagePath + "star.png");
        newMenu.AddItem(imagePath + "email.png", GetMessage("lbl email from"), "ektb_show(\'\',\'" + PAGE_NAME + "?action=Add&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true\', null);");
        AddMenu(newMenu);
        workareamenu actionMenu = new workareamenu("action", GetMessage("lbl action"), imagePath + "check.png");
        actionMenu.AddItem(imagePath + "delete.png", GetMessage("lbl del sel"), "ConfirmDelete();");
        AddMenu(actionMenu);
        AddHelpButton("ViewAllEmailFromAddresses");
    }

    private void EditEmailFromToolBar()
    {
        SetTitleBarToMessage("alt edit email from address");
		AddButtonwithMessages(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions"), "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
		AddButtonwithMessages(imagePath + "save.png", "#", "lbl update email address", "btn update", "Onclick=\"javascript:return SubmitForm(\'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true);
        AddHelpButton("EditSubscriptionEmailMessage");
    }

    private void Util_CheckAccess()
    {

        PermissionData securityData = this.m_refContentApi.LoadPermissions(0, "folder", ContentAPI.PermissionResultType.All);
        if (!securityData.IsLoggedIn || !securityData.IsAdmin || securityData.IsInMemberShip)
        {
            throw (new Exception(GetMessage("msg login cms administrator")));
        }

    }

    protected void RegisterResources()
    {
        // register necessary JS and CSS files
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
    }
}