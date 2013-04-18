using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Search;
using Microsoft.VisualBasic;

	public partial class Workarea_search_deletesynonym : System.Web.UI.UserControl
	{
		
		
		protected LanguageData[] language_data;
		protected UserData user_data;
		protected PermissionData security_data;
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppImgPath = "";
		protected int ContentLanguage = -1;
		protected Guid synonymID = Guid.Empty ;
		
		#region Sub: DeleteSynonymSets
		
		private void DeleteSynonymSets(Guid synonymID)
		{
			bool deleteSuccessful = false;
	
			try
			{
                deleteSuccessful = DeleteSynonym(synonymID, ContentLanguage);
                if (!deleteSuccessful)
				{
					throw (new Exception(m_refMsg.GetMessage("generic error delete unsuccessful")));
				}
				
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}

			if (deleteSuccessful)
			{
				Response.Redirect("synonyms.aspx?action=ViewSynonyms");
			}
		}
		#endregion
		
		
		#region Page Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = (new CommonApi()).EkMsgRef;
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			if (Request.QueryString["LangType"] != null)
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
			m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage;
			// get the specific Synonym ID
			if (Strings.Trim(Request.QueryString["id"]).Length > 0)
			{
				if (!(Strings.Trim(Request.QueryString["id"]) == Guid.Empty.ToString()))
				{
					synonymID = new Guid(Strings.Trim(Request.QueryString["id"]));
				}
				else
				{
					synonymID = Guid.Empty ;
				}
			}
			else
			{
                synonymID = Guid.Empty;
			}
			
			// Display the Synonym Sets
			DeleteSynonymSets(synonymID);
		}
		#endregion


        private bool DeleteSynonym(Guid id, int languageId)
        {
            bool success;

            ISynonyms synonymsApi = ObjectFactory.GetSynonyms();
            try
            {
                synonymsApi.Delete(id, languageId);
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }
	}