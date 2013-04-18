using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Search;
using Microsoft.VisualBasic;

	public partial class addsynonym : System.Web.UI.UserControl
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
		protected CommonApi api = new CommonApi();
		protected string PageMode = "";
		protected string PageAction = "";
		protected Guid inputSynonymID = Guid.Empty;
		
		#region SUB: ProcessAddSynonymSubmit
		protected void ProcessAddSynonymSubmit()
		{
			// clear the Show Duplicates text
			showDuplicates.Text = "";

			// Create new TermsData instance
			SynonymData synonymSetData = new Ektron.Cms.SynonymData();            
			
			try
			{								
				// assign the values to the synonymSetData object
                synonymSetData.Name = Strings.Trim(Request.Form["addsynonym$synonymName"]);
                synonymSetData.TermsKeywords = Strings.Trim(Request.Form["addsynonym$synonymTerms"]).Trim(";".ToCharArray());
                synonymSetData.ID = Guid.Empty;
				synonymSetData.DateModified = DateTime.Now;
				synonymSetData.LanguageID = ContentLanguage;

                if (UpdateSynonym(synonymSetData))
                {
                    Response.Redirect(
                        "synonyms.aspx?action=ViewSynonyms&LangType=" + ContentLanguage.ToString(),
                        false);
                }
                else
                {
                    throw (new Exception(m_refMsg.GetMessage("msg error add synonym set unsuccessful")));
                }
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		#endregion
		
		#region SUB: ProcessEditSynonymSet
		protected void ProcessEditSynonymSet(SynonymData synonymSetData)
		{
			// clear the Show Duplicates text
			showDuplicates.Text = "";
			
			try
			{
				// get the existing data for the Synonym Set
				if (synonymSetData == null)
				{
					throw (new Exception(m_refMsg.GetMessage("msg error synonym set not found")));
				}
				
				// set up the TermData object with the correct information
				
				if (Strings.Trim(Request.Form["addsynonym$synonymName"]).Length > 0)
				{
					synonymSetData.Name = Strings.Trim(Request.Form["addsynonym$synonymName"]);
				}
				
				// assign the values to the synonymSetData object
                
				synonymSetData.TermsKeywords = (string) (Strings.Trim(Request.Form["addsynonym$synonymTerms"]).Trim(";".ToCharArray()));
				synonymSetData.DateModified = DateTime.Now;
                
                if (UpdateSynonym(synonymSetData))
                {
                    Response.Redirect(
                        "synonyms.aspx?action=ViewSynonym&LangType=" + synonymSetData.LanguageID.ToString() + "&id=" + synonymSetData.ID.ToString(),
                        false);
                }
                else
                {
                    throw (new Exception(string.Format(m_refMsg.GetMessage("msg error synonym set update failed"), "<em>" + synonymSetData.Name.ToString() + "</em>")));
                }
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		#endregion
		
		#region Page Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = (new CommonApi()).EkMsgRef;
			m_refMsg = m_refSiteApi.EkMsgRef;
			AppImgPath = m_refSiteApi.AppImgPath;
			ContentLanguage = m_refSiteApi.ContentLanguage;
			PageAction = Request.QueryString["action"].ToLower();
			RegisterResources();
			
			Ektron.Cms.SynonymData synonymSetData = new Ektron.Cms.SynonymData();

			if (Strings.Trim(Request.QueryString["id"]).Length > 0)
			{
				if (! (Strings.Trim((string) (Request.QueryString["id"].ToString())) == Guid.Empty.ToString()))
				{
					inputSynonymID = new Guid(Strings.Trim(Request.QueryString["id"]));
					// intantiate the data class for this Synonym Set
					synonymSetData = GetSynonym(inputSynonymID, ContentLanguage);
					ViewToolBar(synonymSetData.Name);
					synonymTerms.Text = synonymSetData.TermsKeywords;
					termID.Value = synonymSetData.TermID.ToString();
				}
			}
			else
			{
				ViewToolBar("");
			}
			
			checkDuplicates.Text = m_refMsg.GetMessage("btn check for duplicates");
			checkDuplicates.ToolTip = m_refMsg.GetMessage("btn title check for duplicates");
			PageMode = Request.Form["addsynonym$submitMode"];
			
			if ((Page.IsPostBack) && PageMode == "0" && PageAction == "addsynonym")
			{
				ProcessAddSynonymSubmit();
			}
			else if ((Page.IsPostBack) && PageMode == "0" && PageAction == "editsynonym")
			{
				ProcessEditSynonymSet(synonymSetData);
			}
			else if (Page.IsPostBack)
			{
				showDuplicates.Text = CheckForDuplicates();
			}
		}
		#endregion
		
		#region SUB: Toolbar
		protected void ViewToolBar(string synonymName)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			// check the Page Action and set the Title Bar Text appropriately
			if (PageAction == "addsynonym")
			{
				txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg add synonym set"));
			}
			else
			{
				txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(string.Format(m_refMsg.GetMessage("msg edit synonym set"), synonymName));
			}
			
			result.Append("<table><tr>");

			if (PageAction == "addsynonym")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("synonyms.aspx?action=ViewSynonyms&LangType=" + ContentLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "synonyms.aspx?action=ViewSynonym&LangType=" + ContentLanguage + "&id=" + inputSynonymID.ToString() + "&#38;bck=vs", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "javascript:document.forms[0].addsynonym_submitMode.value=0;checkAddSynonyms(document.forms[0]);", m_refMsg.GetMessage("alt save synonym set"), m_refMsg.GetMessage("btn save synonym"), "", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("addsynonymset", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			
			// output the result string to the htmToolBar
			htmToolBar.InnerHtml = result.ToString();
		}
		#endregion
		
		#region FUNCTION: Check Duplicates
		protected string CheckForDuplicates()
		{
			// Create new TermsData instance
			List<SynonymData> synonymSetData;
			
			string currentTerms = (string) (Strings.Trim(Request.Form["addsynonym$synonymTerms"]).Trim(";".ToCharArray()));
			List<string> arrTerms = new List<string>(currentTerms.ToString().Split(';'));

            List<string> arrTermsDistinct = new List<string>(arrTerms.Distinct<string>(new SynonymEqualityComparer()));
            if (arrTerms.Count == arrTermsDistinct.Count)
            {
                try
                {
                    synonymSetData = GetSynonyms(ContentLanguage);
                    if (synonymSetData == null)
                    {
                        throw (new Exception(m_refMsg.GetMessage("msg error synonym set equals nothing")));
                    }
                    string termOutput = "";
                    string tempTermOutput = "";
                    Array arrExistingSynonymSetTerms;
                    foreach (string term in arrTerms)
                    {
                        // check to see if any other Synonym Set contains the term
                        foreach (SynonymData synonymSet in synonymSetData)
                        {
                            // make sure that this isn't the synonym set we're currently working with
                            if (inputSynonymID != synonymSet.ID)
                            {
                                // each Synonym Set needs it's terms split into an array so we can check every term
                                arrExistingSynonymSetTerms = synonymSet.TermsKeywords.Split(';');
                                foreach (string existingTerm in arrExistingSynonymSetTerms)
                                {
                                    // if the terms match, add it to the list of matches for this term
                                    if (term.ToString().Trim().ToLower() == existingTerm.Trim().ToLower())
                                    {
                                        if (tempTermOutput.Length == 0)
                                        {
                                            // first match found for this term
                                            tempTermOutput = (string)("<li>" + string.Format(m_refMsg.GetMessage("msg synonym set is also contained in"), term.ToString().Trim(), "<a href=\"synonyms.aspx?action=ViewSynonym&LangType=" + synonymSet.LanguageID + "&id=" + synonymSet.ID.ToString() + "\" onclick=\"return VerifyFollowDupe();\">" + synonymSet.Name + "</a>"));
                                        }
                                        else
                                        {
                                            tempTermOutput += ", <a href=\"synonyms.aspx?action=ViewSynonym&LangType=" + synonymSet.LanguageID + "&id=" + synonymSet.ID.ToString() + "\"  onclick=\"return VerifyFollowDupe();\">" + synonymSet.Name + "</a>";
                                        }
                                    }
                                }
                            }
                        }
                        // if we've added any matches to the string, close the paragraph tag
                        if (tempTermOutput.Length > 0)
                        {
                            tempTermOutput += "</li>";
                        }
                        // add the string to the final output string
                        termOutput += tempTermOutput.Trim();
                        // clear the temporary string for the next loop
                        tempTermOutput = "";
                    }
                    // set up a string builder
                    System.Text.StringBuilder result = new System.Text.StringBuilder();
                    // build the final output indicating if any duplicates were found or not
                    result.Append("<ul class=\"duplicates\">");
                    if (termOutput.Trim().Length > 0)
                    {
                        result.Append(termOutput);
                    }
                    else
                    {
                        result.Append("<li>" + m_refMsg.GetMessage("msg no duplicates found") + "</li>");
                    }
                    result.Append("</ul>");
                    return result.ToString();

                }
                catch (Exception ex)
                {
                    Utilities.ShowError(ex.Message);
                    return null;
                }
            }
            else
            {
                StringBuilder result = new StringBuilder();
                result.AppendFormat(
                    "<ul class=\"duplicates\"><li>{0}</li></ul>", 
                    m_refMsg.GetMessage("msg duplicates in current set"));

                return result.ToString();
            }
		}
		#endregion
		
		protected void RegisterResources()
		{
			// register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
		}

        private SynonymData GetSynonym(Guid id, int languageId)
        {
            ISynonyms synonymsApi = ObjectFactory.GetSynonyms();
            List<SynonymSet> synonymSets = synonymsApi.GetList(ContentLanguage);
            SynonymSet synonymSet = synonymSets.Find(delegate(SynonymSet set) { return set.Id == id; });

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
                synonymData.LanguageID = languageId;
                synonymData.Name = BuildTermList(set, 3);

                if (set.Terms != null && set.Terms.Count > 0)
                {
                    synonymData.TermsKeywords = BuildTermList(set, set.Terms.Count);
                }

                synonyms.Add(synonymData);
            }

            return synonyms;
        }

        private bool UpdateSynonym(SynonymData synonymData)
        {
            bool success = false;

            if (synonymData != null && !string.IsNullOrEmpty(synonymData.TermsKeywords))
            {
                SynonymSet set = new SynonymSet();
                set.Id = synonymData.ID;
                set.LanguageId = synonymData.LanguageID;

                string[] splitTerms = synonymData.TermsKeywords.Split(
                    new string[] { ";" },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string term in splitTerms)
                {                   
                    set.Terms.Add(new SynonymSetTerm() { Term = term });
                }
                
                ISynonyms synonymsApi = ObjectFactory.GetSynonyms();

                try
                {
                    synonymsApi.Update(set);
                    success = true;
                }
                catch (Exception)
                {
                    // Squash this for now.
                }
            }

            return success;
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

        private class SynonymEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return (x == null && y == null) || (x != null && y != null && Clean(x) == Clean(y));                
            }

            public int GetHashCode(string obj)
            {
                return obj == null ? 0 : Clean(obj).GetHashCode();
            }

            private string Clean(string input)
            {
                return input == null ? null : input.Trim().ToLower();
            }
        }
	}
