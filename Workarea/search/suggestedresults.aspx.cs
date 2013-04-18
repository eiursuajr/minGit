using System;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;

public partial class Workarea_search_suggestedresults : System.Web.UI.Page
{
    // initialize variables
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected SiteAPI m_refApi = new SiteAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string strPageAction = "";
    protected string suggestedResultID;
    protected int ContentLanguage = -1;
    protected Workarea_controls_search_viewsuggestedresults objViewSuggestedResults = new Workarea_controls_search_viewsuggestedresults();
    protected Workarea_controls_search_addsuggestedresult objAdd = new Workarea_controls_search_addsuggestedresult();
    protected Workarea_controls_search_viewsuggestedresult objView = new Workarea_controls_search_viewsuggestedresult();
    protected Workarea_controls_search_deletesuggestedresults objDelete;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Ektron.Cms.Content.EkContent objContentRef;
        objContentRef = m_refContentApi.EkContentRef;

        // register JS and CSS
        RegisterResources();
        jsStyleSheet.Text = (new StyleHelper()).GetClientScript();
        m_refMsg = (new Ektron.Cms.CommonApi()).EkMsgRef;
        if ((Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser) || m_refContentApi.RequestInformationRef.UserId == 0 || !m_refContentApi.EkUserRef.IsAllowed(m_refContentApi.UserId, 0, "users", "IsAdmin", 0)) && !(objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SearchAdmin, m_refContentApi.UserId, false)))
        {
            Response.Redirect(m_refContentApi.ApplicationPath + "Login.aspx", true);
            return;
        }

        if (!IsSearchConfigured())
        {
            Utilities.ShowError(m_refMsg.GetMessage("msg search suggested results connection error"));
        }
        else
        {

            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            {
                strPageAction = Request.QueryString["action"].ToLower();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["termID"]))
            {
                suggestedResultID = Request.QueryString["termID"];
            }

            jsSuggestedResultID.Text = suggestedResultID;
            jsPageAction.Text = strPageAction;
            jsDelResultSet.Text = m_refMsg.GetMessage("js confirm delete suggested result set");
            jsDelResult.Text = m_refMsg.GetMessage("js confirm delete suggested result");
            jsDelResultEdit.Text = m_refMsg.GetMessage("js confirm delete suggested result edit");
            //jsResultTerm.Text = m_refMsg.GetMessage("lbl suggested result term");
            //jsTypeOpt2.Text = m_refMsg.GetMessage("msg suggestedresults type option2");
            jsLinkReq.Text = m_refMsg.GetMessage("js link required");
            jsTitleReq.Text = m_refMsg.GetMessage("js title required");
            jsSummaryReq.Text = m_refMsg.GetMessage("js summary required");
            jsSizeExceeded.Text = m_refMsg.GetMessage("js summary size exceeded");
            jsTermReq.Text = m_refMsg.GetMessage("js term required");
            jsTermNoCommas.Text = m_refMsg.GetMessage("js terms no commas");
            jsTermsNoParenthesis.Text = m_refMsg.GetMessage("js terms no parenthesis");
            jsSynonymSetReq.Text = m_refMsg.GetMessage("js synonym set required");
            jsSugResultReq.Text = m_refMsg.GetMessage("js one or more suggested results required");
            try
            {
                switch (strPageAction)
                {
                    case "viewsuggestedresults":
                        // reference viewsuggestedresults.ascx
                        objViewSuggestedResults = (Workarea_controls_search_viewsuggestedresults)(LoadControl("../controls/search/viewsuggestedresults.ascx"));
                        objViewSuggestedResults.ID = "viewsuggestedresults";
                        DataHolder.Controls.Add(objViewSuggestedResults);
                        break;

                    case "addsuggestedresult":
                        // reference addsuggestedresults.ascx
                        objAdd = (Workarea_controls_search_addsuggestedresult)(LoadControl("../controls/search/addsuggestedresult.ascx"));
                        objAdd.ID = "addsuggestedresult";
                        DataHolder.Controls.Add(objAdd);
                        break;

                    case "viewsuggestedresult":
                        // reference viewsuggestedresults.ascx
                        objView = (Workarea_controls_search_viewsuggestedresult)(LoadControl("../controls/search/viewsuggestedresult.ascx"));
                        objAdd.ID = "viewsuggestedresult";
                        DataHolder.Controls.Add(objView);
                        break;

                    case "editsuggestedresult":
                        // reference addsuggestedresults.ascx
                        objAdd = (Workarea_controls_search_addsuggestedresult)(LoadControl("../controls/search/addsuggestedresult.ascx"));
                        objAdd.ID = "addsuggestedresult";
                        DataHolder.Controls.Add(objAdd);
                        break;

                    case "deletesuggestedresult":
                        // reference addsuggestedresults.ascx
                        objDelete = (Workarea_controls_search_deletesuggestedresults)(LoadControl("../controls/search/deletesuggestedresults.ascx"));
                        objDelete.ID = "deletesuggestedresult";
                        DataHolder.Controls.Add(objDelete);
                        break;
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowError(ex.Message);
            }
        }
    }

    /// <summary>
    /// Returns true if the site has been configured for search.
    /// </summary>
    /// <returns>True if the site has been configured for search, false otherwise</returns>
    private bool IsSearchConfigured()
    {
        bool isSearchConfigured;
        
        try
        {
            ISearchSettings searchSettings = ObjectFactory.GetSearchSettings();
            isSearchConfigured = searchSettings.GetItem() != null;
        }
        catch
        {
            isSearchConfigured = false;
        }

        return isSearchConfigured;
    }

    public void RegisterResources()
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS");
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.AllIE);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}


