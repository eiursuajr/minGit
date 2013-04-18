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

public partial class history : System.Web.UI.Page
{
    #region Member Variables

    protected string AppName = "";
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected ContentData content_data;
    protected long ContentId = -1;
    protected long HistoryId = -1;
    protected string AppImgPath = "";
    protected ContentAPI m_refContentApi;
    protected PermissionData security_data;
    protected int ContentLanguage = -1;
    protected bool bXmlContent = false;
    protected bool bApplyXslt = false;
    protected ContentData hist_content_data;

    #endregion

    #region Events

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        m_refContentApi = new ContentAPI();
		  Utilities.ValidateUserLogin();
        //register page components
        this.RegisterJS();
        this.RegisterCSS();
    }

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }
            if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContentApi.ContentLanguage = ContentLanguage;
            }

            m_refMsg = m_refContentApi.EkMsgRef;
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            if (Request.QueryString["action"] != null && Request.QueryString["action"] == "report")
            {
                ShowHistoryListFrame(false);
                ViewHistoryList m_viewHistoryList = new ViewHistoryList();
                m_viewHistoryList = (ViewHistoryList)(LoadControl("controls/history/ViewHistoryList.ascx"));
                m_viewHistoryList.ID = "ViewHistory";
                DataHolder.Controls.Add(m_viewHistoryList);
            }
            else
            {
                ShowHistoryListFrame(true);
                ViewHistory m_viewHistory = new ViewHistory();
                m_viewHistory = (ViewHistory)(LoadControl("controls/history/ViewHistory.ascx"));
                m_viewHistory.ID = "ViewHistory";
                DataHolder.Controls.Add(m_viewHistory);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    #endregion

    #region Helpers

    private void ShowHistoryListFrame(bool bShow)
    {
        if (bShow)
        {
            SetFrame.Text = "<script type=\"text/javascript\">try { top.ShowListWindow(); } catch(e) { }</script>";
        }
        else
        {
            SetFrame.Text = "<script type=\"text/javascript\">try { top.HideListWindow(); } catch(e) { }</script>";
        }
    }

    private void ShowError(string ex)
    {
        Response.Redirect((string)("reterror.aspx?info=" + ex), false);
    }

    #endregion

    #region JS, CSS

    private void RegisterJS()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
    }

    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/tables/tableutil.css", "EktronTablesUtilsCss");
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "csslib/commerce/Ektron.Commerce.Pricing.css", "EktronCommercePricingCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

    #endregion

}


