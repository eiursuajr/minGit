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
using Ektron.Cms.Common;

public partial class suscriptions : System.Web.UI.Page
{
    #region  Web Form Designer Generated Code

    protected EkMessageHelper m_refMsg;
    protected SiteAPI m_refSiteAPI = new SiteAPI();
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected long m_intFontId = 0;
    protected long m_intSubscriptionId = 0;
    protected FontData font_data;
    protected SubscriptionData subscription_data;
    protected ContentAPI m_refContApi = new ContentAPI();
    protected string m_strSubscriptionNameFromUserControl;
    protected bool m_strSubscriptionEnableFromUserControl;
    protected int EnableMultilingual;
    protected int ContentLanguage = -1;
    protected long m_intId = 0;
    protected LanguageData[] colActiveLanguages;

    #endregion

    const string PAGE_NAME = "subscriptions.aspx";
    protected string imagePath = "";
    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        try
        {
            m_refMsg = m_refContApi.EkMsgRef;
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            EnableMultilingual = m_refContApi.EnableMultilingual;
            RegisterResources();
            SetServerJSVariables();
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }

            Util_CheckAccess();
            imagePath = m_refContApi.AppPath + "images/ui/icons/";
            if (!(Request.QueryString["action"] == null))
            {
                m_strPageAction = Request.QueryString["action"];
                if (m_strPageAction.Length > 0)
                {
                    m_strPageAction = m_strPageAction.ToLower();
                }
            }

            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            if (EnableMultilingual == 1)
            {
                colActiveLanguages = m_refSiteAPI.GetAllActiveLanguages();
            }

            m_refContApi.ContentLanguage = ContentLanguage;
            AppImgPath = m_refContApi.AppImgPath;
            TR_AddEditSubscription.Visible = false;
            TR_ViewSubscription.Visible = false;
            TR_ViewAllFont.Visible = false;
            if (!(Page.IsPostBack))
            {
                switch (m_strPageAction)
                {
                    case "viewallsubscriptions":
                        Display_ViewAllSubscriptions();
                        break;
                    case "view":
                        Display_ViewSubscription();
                        break;
                    case "edit":
                        Display_EditSubscription();
                        break;
                    case "add":
                        Display_AddSubscription();
                        break;
                    case "delete":
                        Process_DeleteSubscription();
                        break;
                }
            }
            else
            {
                switch (m_strPageAction)
                {
                    case "edit":
                        Process_EditSubscription();
                        break;
                    case "add":
                        Process_AddSubscription();
                        break;
                    case "delete":
                        Process_DeleteSubscription();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
        }
    }

    private void Util_CheckAccess()
    {
        PermissionData securityData = this.m_refContApi.LoadPermissions(0, "folder", ContentAPI.PermissionResultType.All);
        if (!securityData.IsLoggedIn || !securityData.IsAdmin || securityData.IsInMemberShip)
        {
            throw (new Exception(m_refMsg.GetMessage("msg login cms administrator")));
        }
    }

    private void Process_EditSubscription()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(Request.Form["subscriptionID"], "ID", null, null);
        pagedata.Add(Request.Form["txtName"], "SubscriptionName", null, null);
        if (string.IsNullOrEmpty(Request.Form["chkEnableAddEdit"]))
        {
            pagedata.Add("False", "Enable", null, null);
        }
        else
        {
            pagedata.Add("True", "Enable", null, null);
        }

        m_refContApi.UpdateSubscription(pagedata);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions"), false);
    }

    private void Process_AddSubscription()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(Request.Form["txtName"], "SubscriptionName", null, null);
        if (string.IsNullOrEmpty(Request.Form["chkEnableAddEdit"]))
        {
            pagedata.Add("False", "Enable", null, null);
        }
        else
        {
            pagedata.Add("True", "Enable", null, null);
        }

        m_refContApi.AddSubscription(pagedata);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions"), false);
    }

    private void Process_DeleteSubscription()
    {
        Collection pagedata;
        pagedata = new Collection();
        pagedata.Add(Request.QueryString["SubscriptionID"], "ID", null, null);
        m_refContApi.DeleteSubscription(pagedata);
        Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewallsubscriptions"), false);
    }

    private void Display_EditSubscription()
    {
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        chkEnableAddEdit.Enabled = true;
        if (!(Request.QueryString["id"] == null))
        {
            m_intSubscriptionId = Convert.ToInt64(Request.QueryString["id"]);
        }

        subscription_data = m_refContApi.GetSubscriptionById(m_intSubscriptionId);
        txtName.Text = Server.HtmlDecode(subscription_data.Name);
        chkEnableAddEdit.Checked = subscription_data.Enable;
        ltrAddEditID.Text = subscription_data.Id.ToString() + "<input type=\"hidden\" name=\"subscriptionID\" value=\"" + subscription_data.Id.ToString() + "\"/>";
        EditSubscriptionToolBar();
    }

    private void Display_AddSubscription()
    {
        TR_ViewSubscription.Visible = false;
        TR_AddEditSubscription.Visible = true;
        chkEnableAddEdit.Enabled = true;
        TD_SubscriptionID.Visible = false;
        AddSubscriptionToolBar();
    }

    private void Display_ViewSubscription()
    {
        TR_ViewSubscription.Visible = true;
        if (!(Request.QueryString["id"] == null))
        {
            m_intSubscriptionId = Convert.ToInt64(Request.QueryString["id"]);
        }

        subscription_data = m_refContApi.GetSubscriptionById(m_intSubscriptionId);
        if (subscription_data != null)
        {
            ltrViewName.Text = EkFunctions.HtmlEncode(subscription_data.Name);
            ltrViewID.Text = subscription_data.Id.ToString();
            chkEnable.Enabled = false;
            chkEnable.Checked = subscription_data.Enable;
        }

        ViewSubscriptionToolBar();
    }

    private void Display_ViewAllSubscriptions()
    {
        TR_ViewAllFont.Visible = true;
        SubscriptionData[] subscription_data_list;
        subscription_data_list = m_refContApi.GetAllSubscriptions();
        if (!(subscription_data_list == null))
        {
            System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "NAME";
            colBound.HeaderText = m_refMsg.GetMessage("generic Subscriptionname");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            colBound.HeaderStyle.VerticalAlign = VerticalAlign.Top;
            colBound.HeaderStyle.Wrap = false;
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ID";
            colBound.HeaderText = m_refMsg.GetMessage("generic SubscriptionID");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "ENABLE";
            colBound.HeaderText = m_refMsg.GetMessage("generic Subscriptionenable");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            ViewSubscriptionGrid.Columns.Add(colBound);
            colBound = new System.Web.UI.WebControls.BoundColumn();
            colBound.DataField = "LanguageId";
            colBound.HeaderText = m_refMsg.GetMessage("generic SubscriptionLanguageID");
            colBound.ItemStyle.Wrap = false;
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            ViewSubscriptionGrid.Columns.Add(colBound);
            DataTable dt = new DataTable();
            DataRow dr;
            int i = 0;
            dt.Columns.Add(new DataColumn("NAME", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));
            dt.Columns.Add(new DataColumn("ENABLE", typeof(string)));
            dt.Columns.Add(new DataColumn("LanguageId", typeof(string)));
            for (i = 0; i <= subscription_data_list.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"" + PAGE_NAME + "?action=View&id=" + subscription_data_list[i].Id + "\" title=\'" + m_refMsg.GetMessage("click to view subscription msg") + " \"" + Strings.Replace(EkFunctions.HtmlEncode(subscription_data_list[i].Name), "\'", "`", 1, -1, 0) + "\"\'>" + EkFunctions.HtmlEncode(subscription_data_list[i].Name) + "</a>";
                dr[1] = subscription_data_list[i].Id.ToString();
                if (subscription_data_list[i].Enable)
                {
                    dr[2] = "<input type=\"checkbox\" name=\"chkEnable\" CHECKED DISABLED />";
                }
                else
                {
                    dr[2] = "<input type=\"checkbox\" name=\"chkEnable\" DISABLED />";
                }

                dr[3] = subscription_data_list[i].LanguageId;
                dt.Rows.Add(dr);
            }

            ViewSubscriptionGrid.BorderColor = System.Drawing.Color.White;
            DataView dv = new DataView(dt);
            ViewSubscriptionGrid.DataSource = dv;
            ViewSubscriptionGrid.DataBind();
        }

        ViewAllSubscriptionsToolBar();
    }

    private void AddSubscriptionToolBar()
    {
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add subscription page title"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "save.png", "#", m_refMsg.GetMessage("alt add button text (subscriptions)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm( \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider); 
		result.Append("<td>" + m_refStyle.GetHelpButton("AddSubscription", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void ViewSubscriptionToolBar()
    {
        string name = "";
        if (subscription_data != null)
        {
            name = EkFunctions.HtmlEncode(subscription_data.Name);
        }

        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("view system subscription msg") + " \"" + name + "\""));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int i = 0;
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAllSubscriptions"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "contentEdit.png", System.Convert.ToString(PAGE_NAME + "?action=Edit&id=" + m_intSubscriptionId.ToString() + ""), m_refMsg.GetMessage("alt edit button text (subscription)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
        result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "delete.png", System.Convert.ToString(PAGE_NAME + "?action=delete&SubscriptionID=" + m_intSubscriptionId.ToString() + ""), m_refMsg.GetMessage("alt delete button text (subscription)"), m_refMsg.GetMessage("btn delete"), "OnClick=\"javascript: return ConfirmFontDelete();\"", StyleHelper.DeleteButtonCssClass));
        if (EnableMultilingual == 1)
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">" + m_refMsg.GetMessage("lbl view") + ":</td>");
            result.Append("<td>");
            result.Append("<select name=\"language\" ID=\"language\" onchange=\"JavaScript:SelLanguage(this.value)\">>");
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                result.Append("<option value=\"" + (colActiveLanguages[i].Id) + "\" ");
                if (ContentLanguage == colActiveLanguages[i].Id)
                {
                    result.Append("selected ");
                }

                result.Append(">" + (colActiveLanguages[i].Name) + "</option>");
            }

            result.Append("</select>");
            result.Append("</td>");
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("viewsubscriptions", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void ViewAllSubscriptionsToolBar()
    {
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view system subscriptions msg"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int i = 0;
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "add.png", System.Convert.ToString(PAGE_NAME + "?action=Add"), m_refMsg.GetMessage("alt add button text (subscriptions)"), m_refMsg.GetMessage("btn add subscription"), "", StyleHelper.AddButtonCssClass, true));
        if (EnableMultilingual == 1)
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td class=\"label\">" + m_refMsg.GetMessage("lbl view") + ":</td>");
            result.Append("<td>");
            result.Append("<select name=\"language\" ID=\"language\" onchange=\"JavaScript:SelLanguage(this.value)\">>");
            for (i = 0; i <= colActiveLanguages.Length - 1; i++)
            {
                result.Append("<option value=\"" + (colActiveLanguages[i].Id) + "\" ");
                if (ContentLanguage == colActiveLanguages[i].Id)
                {
                    result.Append("selected ");
                }

                result.Append(">" + (colActiveLanguages[i].Name) + "</option>");
            }

            result.Append("</select>");
            result.Append("</td>");
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refStyle.GetHelpButton("ViewAllSubscriptions", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void EditSubscriptionToolBar()
    {
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("edit subscription page title") + " \"" + EkFunctions.HtmlEncode(subscription_data.Name) + "\""));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", System.Convert.ToString(PAGE_NAME + "?action=View&id=" + Request.QueryString["id"] + ""), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "save.png", "#", m_refMsg.GetMessage("alt update button text (subscription)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider); 
		result.Append("<td>" + m_refStyle.GetHelpButton("EditSubscription", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }

    private void SetServerJSVariables()
    {
        ltr_nameReq.Text = m_refMsg.GetMessage("alert msg subscription name reqd");
        ltr_confirmDelete.Text = m_refMsg.GetMessage("js: confirm delete subscription");
    }

    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }
}


