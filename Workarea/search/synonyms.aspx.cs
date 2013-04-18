using System;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;

	public partial class Workarea_search_synonyms : System.Web.UI.Page
	{
		// initialize variables
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected SiteAPI m_refApi = new SiteAPI();
		protected string strStyleSheetJS = "";
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
		protected string strPageAction = "";
		protected Guid synonymID;
		protected int ContentLanguage = -1;
		protected viewsynonyms objViewSets;
		protected addsynonym objAdd;
		protected viewsynonym objView;
		protected Workarea_search_deletesynonym objDelete;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Ektron.Cms.Content.EkContent objContentRef;
			objContentRef = m_refContentApi.EkContentRef;
			RegisterResources();
            styleSheetJs.Text = (new StyleHelper()).GetClientScript();
			m_refMsg = (new CommonApi()).EkMsgRef;
			
			// initialize JS text strings
			jsConfirmDeleteSet.Text = m_refMsg.GetMessage("js: confirm delete synonym set");
			jsConfirmFollowDupe.Text = m_refMsg.GetMessage("js confirm follow dupe");
			jsMinTwoTerms.Text = m_refMsg.GetMessage("js synonym min two terms");
			jsTermsNoCommas.Text = m_refMsg.GetMessage("js synonym terms no commas");
			jsTermNoParenthesis.Text = m_refMsg.GetMessage("js synonym terms no parenthesis");
			jsTermsNoLessGreater.Text = m_refMsg.GetMessage("js synonym terms no less than or greater than");
			
			if ((Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser) || m_refContentApi.RequestInformationRef.UserId == 0 || ! m_refContentApi.EkUserRef.IsAllowed(m_refContentApi.UserId, 0, "users", "IsAdmin", 0)) && !(objContentRef.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SearchAdmin, m_refContentApi.UserId, false)))
			{
				Response.Redirect(m_refContentApi.ApplicationPath + "Login.aspx", true);
				return;
			}

            if (!IsSearchConfigured())
            {
                Utilities.ShowError(m_refMsg.GetMessage("msg search synonyms connection error"));
            }
            else
            {

                if (!string.IsNullOrEmpty(Request.QueryString["action"]))
                {
                    strPageAction = Request.QueryString["action"].ToLower();
                    jsPageAction.Text = strPageAction;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    synonymID = new Guid(Request.QueryString["id"]);
                    jsSynonymId.Text = synonymID.ToString();
                }

                try
                {
                    switch (strPageAction)
                    {
                        case "viewsynonyms":
                            // reference viewsynonyms.ascx
                            objViewSets = (viewsynonyms)(LoadControl("../controls/search/viewsynonyms.ascx"));
                            objViewSets.ID = "viewsynonyms";
                            DataHolder.Controls.Add(objViewSets);
                            break;

                        case "viewsynonym":
                            // reference viewsynonym.ascx
                            objView = (viewsynonym)(LoadControl("../controls/search/viewsynonym.ascx"));
                            objView.ID = "viewsynonym";
                            DataHolder.Controls.Add(objView);
                            break;

                        case "addsynonym":
                            // reference addsynonym.ascx
                            objAdd = (addsynonym)(LoadControl("../controls/search/addsynonym.ascx"));
                            objAdd.ID = "addsynonym";
                            DataHolder.Controls.Add(objAdd);
                            break;

                        case "deletesynonym":
                            // delete the Synonym Set specified and display the remaining Synonym Sets
                            objDelete = (Workarea_search_deletesynonym)(LoadControl("../controls/search/deletesynonym.ascx"));
                            objDelete.ID = "deletesynonym";
                            DataHolder.Controls.Add(objDelete);
                            break;

                        case "editsynonym":
                            // reference addsynonym.ascx
                            objAdd = (addsynonym)(LoadControl("../controls/search/addsynonym.ascx"));
                            objAdd.ID = "addsynonym";
                            DataHolder.Controls.Add(objAdd);
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
		
		protected void RegisterResources()
		{
			// register JS
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
			
			// register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.AllIE);
			Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}
	

