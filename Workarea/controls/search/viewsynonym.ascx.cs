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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.SuggestedResultData;


	public partial class viewsynonym : System.Web.UI.UserControl
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
		
		#region SUB: OutputSynonymSetsForID
		protected void OutputSynonymSetsForID(SynonymData synonymSetData)
		{
			synonymOutput.Text = "<table class=\"ektronForm\">" + "\r\n";
            //synonymOutput.Text += "  <tr>" + "\r\n";
            //synonymOutput.Text += "    <td class=\"label\"><label for=\"setName\">" + m_refMsg.GetMessage("lbl synonym set name") + ":</label></td>" + "\r\n";
            //synonymOutput.Text += "    <td class=\"readOnlyValue\">" + EkFunctions.HtmlEncode(synonymSetData.Name) + "</td>" + "\r\n";
            //synonymOutput.Text += "  </tr>" + "\r\n";
			synonymOutput.Text += "  <tr>" + "\r\n";
			synonymOutput.Text += "    <td class=\"label\"><label for=\"setTerms\">" + m_refMsg.GetMessage("lbl synonym header terms") + ":</label></td>" + "\r\n";
			synonymOutput.Text += "    <td class=\"readOnlyValue\"><div class=\"viewSynonymTerms\">" + EkFunctions.HtmlEncode(synonymSetData.TermsKeywords) + "</div></td>" + "\r\n";
			synonymOutput.Text += "  </tr>" + "\r\n";
			synonymOutput.Text += "  </table>";
		}
		#endregion
		
		
		#region Page Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = (new CommonApi()).EkMsgRef;
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			// get and apply the Content Language value
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
			// get the specific Synonym ID
			if (Strings.Trim(Request.QueryString["id"]).Length > 0)
			{
				if (! (Request.QueryString["id"].ToString() == Guid.Empty.ToString()))
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
			
			Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
			try
			{
				SynonymData synonymSetData = GetSynonym(synonymID, ContentLanguage);
				
				ViewToolBar(synonymSetData);
				OutputSynonymSetsForID(synonymSetData);
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		#endregion
		
		#region SUB: ViewToolBar
		private void ViewToolBar(SynonymData synonymSetData)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string referrer = Request.QueryString["bck"];
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("msg view synonym") + " \"" + synonymSetData.Name + "\""));
			
			result.Append("<table><tr>");

			if (referrer == "vs")
			{
				// we want to "back" button to take the user to the ViewSynonyms page
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("synonyms.aspx?action=ViewSynonyms&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				// anything else, and return them to where they came from
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", (string)("synonyms.aspx?action=EditSynonym&LangType=" + synonymSetData.LanguageID + "&id=" + synonymSetData.ID.ToString()), m_refMsg.GetMessage("alt edit synonym set"), m_refMsg.GetMessage("generic edit title"), "", StyleHelper.EditButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/delete.png", (string)("synonyms.aspx?action=DeleteSynonym&id=" + synonymSetData.ID.ToString() + "&LangType=" + synonymSetData.LanguageID), m_refMsg.GetMessage("alt delete synonym set"), m_refMsg.GetMessage("generic delete title"), "OnClick=\"javascript:return VerifyDeleteSynonym();\"", StyleHelper.DeleteButtonCssClass));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("viewsynonymset", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			
			// output the result string to the htmToolBar
			htmToolBar.InnerHtml = result.ToString();
		}
		#endregion

        private SynonymData GetSynonym(Guid id, int languageId)
        {
            ISynonyms synonymsApi = ObjectFactory.GetSynonyms();
            List<SynonymSet> synonymSets = synonymsApi.GetList(ContentLanguage);
            SynonymSet synonymSet = synonymSets.Find(delegate(SynonymSet set){ return set.Id == id; });

            SynonymData synonym = null;
            if (synonymSet != null)
            {
                synonym = new SynonymData();
                synonym.DateModified = DateTime.Now;
                synonym.ID = synonymSet.Id;
                synonym.LanguageID = languageId;
                synonym.Name = BuildTermList(synonymSet, 3);

                if (synonymSet.Terms != null && synonymSet.Terms.Count > 0)
                {
                    synonym.TermsKeywords = BuildTermList(synonymSet, synonymSet.Terms.Count);
                }
            }

            return synonym;
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