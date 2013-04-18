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
using Ektron.Cms.Content;
using Ektron.Cms.Common;
using Ektron.Cms.Search;

/// <summary>
/// This control is used to output the current Synonym Sets in the CMS to the workarea.
/// </summary>
/// <remarks></remarks>
	public partial class viewsynonyms : System.Web.UI.UserControl
	{
		
		
		protected LanguageData[] language_data;
		protected UserData user_data;
		protected PermissionData security_data;
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected UserAPI m_refUserApi = new UserAPI();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string AppPath = "";
		protected int ContentLanguage = -1;
		protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
		
		
		
		#region Sub: OutputSynonymSets
		/// <summary>
		/// This subroutine accepts an integer indicating the current ContentLanguage specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
		/// </summary>
		/// <param name="ContentLanguage">An integer representing the current user selected language for the content.</param>
		/// <remarks></remarks>
		protected void OutputSynonymSets(int ContentLanguage, Literal  objLiteral)
		{
			Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
            try
            {
                List<SynonymData> synonymSetData = GetSynonyms(ContentLanguage);

                if (synonymSetData == null)
                {
                    throw (new Exception(m_refMsg.GetMessage("generic no results found")));
                }

                objLiteral.Text = "<table id=\"viewSynonymSets\" class=\"ektronGrid\">";
                
                objLiteral.Text += "<tr class=\"title-header\">" + "\r\n";
                objLiteral.Text += "<th class=\"left\">" + m_refMsg.GetMessage("lbl synonym header set") + "</th>" + "\r\n";
                objLiteral.Text += "<th class=\"center\">" + m_refMsg.GetMessage("generic language") + "</th>" + "\r\n";
                objLiteral.Text += "</tr>" + "\r\n";

                foreach (SynonymData synonymSet in synonymSetData)
                {
                    //TODO: Pinkesh - Add row striping
                    objLiteral.Text += "<tr class=\"row\">" + "\r\n";
                    objLiteral.Text += "<td><a href=\"synonyms.aspx?id=" + synonymSet.ID.ToString() + "&#38;LangType=" + synonymSet.LanguageID + "&#38;action=ViewSynonym&#38;bck=vs\">" + EkFunctions.HtmlEncode(synonymSet.Name) + "</td>" + "\r\n";
                    objLiteral.Text += "<td  class=\"center\"><img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(synonymSet.LanguageID) + "\' title=\'" + synonymSet.LanguageID + "\' alt=\'" + synonymSet.LanguageID + "\' /></td>";                    
                    objLiteral.Text += "</tr>" + "\r\n";
                }
                objLiteral.Text += "</table>" + "\r\n";
            }
            catch
            {
                Utilities.ShowError(m_refMsg.GetMessage("msg search synonyms connection error"));
            }
		}
		#endregion
		
		#region Page Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = (new CommonApi()).EkMsgRef;
			AppPath = m_refSiteApi.AppPath;
			
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
			
			m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage;
			
			// build the toolbar
			ViewToolBar(txtTitleBar, htmToolBar);
			// Display the Synonym Sets
			OutputSynonymSets(ContentLanguage, synonymOutput);
		}
		#endregion
		
		#region Sub: ViewToolBar
        private void ViewToolBar(HtmlGenericControl objTitleBar, HtmlGenericControl objHTMToolBar)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			// place the proper title in the Title Bar
			objTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg view synonyms"));
			
			// build the string for rendering the htmToolBar
			result.Append("<table cellspacing=\"0\"><tr>");

			bool addHelpDivider = false;

			if (ContentLanguage != -1)
			{
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/add.png", (string)("synonyms.aspx?action=AddSynonym&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt add button text synonym"), m_refMsg.GetMessage("btn add synonym"), "", StyleHelper.AddButtonCssClass, true));

				addHelpDivider = true;
			}
			// if the system is
			if (m_refContentApi.EnableMultilingual == 1)
			{
				result.Append(StyleHelper.ActionBarDivider);

				SiteAPI m_refsite = new SiteAPI();
				LanguageData[] language_data = new LanguageData[1];
				language_data = m_refsite.GetAllActiveLanguages();
				
				result.Append("<td class=\"label\" id=\"viewText\">");
				
				result.Append("&nbsp;" + m_refMsg.GetMessage("generic view") + ": ");
				result.Append("<select id=\'LangType\' name=\'LangType\'OnChange=\"javascript:LoadLanguage(this.options[this.selectedIndex].value);\">");
				if (ContentLanguage == -1)
				{
					result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + " selected>" + m_refMsg.GetMessage("generic all") + "</option>");
				}
				else
				{
					result.Append("<option value=" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + ">" + m_refMsg.GetMessage("generic all") + "</option>");
				}
				
				for (int count = 0; count <= language_data.Length - 1; count++)
				{
					if (Convert.ToString((short) ContentLanguage) == Convert.ToString(language_data[count].Id))
					{
						result.Append("<option value=" + language_data[count].Id + " selected>" + language_data[count].Name + "</option>");
					}
					else
					{
						result.Append("<option value=" + language_data[count].Id + ">" + language_data[count].Name + "</option>");
					}
				}
				result.Append("</select></td>");

				addHelpDivider = true;
			}

			if (addHelpDivider)
			{
				result.Append(StyleHelper.ActionBarDivider);
			}

			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("synonymsets", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			
			// output the result string to the htmToolBar
			objHTMToolBar.InnerHtml = result.ToString();
		}
		#endregion

        /// <summary>
        /// Gets a collection of synonym data for the specified language.
        /// </summary>
        /// <param name="languageId">Content language</param>
        /// <returns>Collection of synonym data</returns>
        private List<SynonymData> GetSynonyms(int languageId)
        {
            List<SynonymData> synonyms = new List<SynonymData>();

            ISynonyms synonymsApi = ObjectFactory.GetSynonyms();
            List<SynonymSet> sets = synonymsApi.GetList(languageId);

            foreach (SynonymSet set in sets)
            {
                SynonymData synonymData = new SynonymData();
                synonymData.DateModified = DateTime.Now;
                synonymData.ID = set.Id;
                synonymData.LanguageID = set.LanguageId;
                synonymData.Name = BuildTermList(set, 7);
                
                if (set.Terms != null && set.Terms.Count > 0)
                {
                    synonymData.TermsKeywords = BuildTermList(set, set.Terms.Count);
                }

                synonyms.Add(synonymData);
            }

            return synonyms;
        }

        private static string BuildTermList(SynonymSet set, int maxTerms)
        {
            StringBuilder titleBuilder = new StringBuilder();
            for (int i = 0; i < set.Terms.Count && i < maxTerms; i++)
            {
                if (i != 0)
                {
                    titleBuilder.Append("; ");
                }

                titleBuilder.Append(set.Terms[i].Term);
            }

            if (set.Terms.Count > maxTerms)
            {
                titleBuilder.Append("...");
            }

            return titleBuilder.ToString();
        }
	}
	
