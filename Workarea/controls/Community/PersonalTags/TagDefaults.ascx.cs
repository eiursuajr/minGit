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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkFunctions;
using Ektron.Cms.Content;
using Ektron.Cms.Community;

	public partial class controls_Community_PersonalTags_TagDefaults : System.Web.UI.UserControl
	{
		
		string chkTag;
		bool isAdmin;
		PermissionData permissionData;
		protected Community_PersonalTags m_containerPage;
		protected string imagePath = "";
		private TagsAPI m_tagApi;
		LanguageData[] _languageDataArray;
		EkEnumeration.CMSObjectTypes defaultTagObjectType;
		
		
		/// <summary>
		/// Returns true if there are more than one languages enabled for the site.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool IsSiteMultilingual
		{
			get
			{
				if (m_containerPage.RefCommonAPI.EnableMultilingual == 0)
				{
					return false;
				}
				int languageEnabledCount = 0;
				foreach (LanguageData lang in LanguageDataArray)
				{
					if (lang.SiteEnabled)
					{
						languageEnabledCount++;
					}
					if (languageEnabledCount > 1)
					{
						break;
					}
				}
				
				return languageEnabledCount > 1;
			}
			
		}
		private LanguageData[] LanguageDataArray
		{
			get
			{
				if (_languageDataArray == null)
				{
					SiteAPI siteApi = new SiteAPI();
					_languageDataArray = siteApi.GetAllActiveLanguages();
				}
				
				return _languageDataArray;
			}
		}
		
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			m_containerPage = (Community_PersonalTags) Page;
			permissionData = m_containerPage.RefContentApi.LoadPermissions(0, "content", 0);
			m_tagApi = new Ektron.Cms.Community.TagsAPI();
			isAdmin = permissionData.IsAdmin;
			imagePath = m_tagApi.AppPath + "images/ui/icons/";
			
			// FireFox literally relies on the url object for the query string parse.
			if (Request.QueryString["objectType"] != null)
			{
				defaultTagObjectType = (EkEnumeration.CMSObjectTypes)int.Parse(Request.QueryString["objectType"]);
			}
			else
			{
                defaultTagObjectType = (EkEnumeration.CMSObjectTypes)int.Parse(Request.QueryString["amp;objectType"]);
			}
			
			bool isLanguageSiteEnabled = false;
			for (int i = 0; i <= LanguageDataArray.Length - 1; i++)
			{
				if (LanguageDataArray[i].Id == m_containerPage.ContentLanguage && LanguageDataArray[i].SiteEnabled)
				{
					isLanguageSiteEnabled = true;
					break;
				}
			}
			
			//the default tags page does not support all language - set to default.
			if (m_containerPage.ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES || isLanguageSiteEnabled == false)
			{
				m_containerPage.ContentLanguage = m_containerPage.RefCommonAPI.DefaultContentLanguage;
				m_containerPage.RefCommonAPI.SetCookieValue("LastValidLanguageID", m_containerPage.RefCommonAPI.DefaultContentLanguage.ToString());
			}
			
			error_InvalidChars.Text = m_containerPage.RefMsg.GetMessage("msg error tag invalid chars");
			error_EmptyTag.Text = m_containerPage.RefMsg.GetMessage("msg error tag empty");
			
			if (IsPostBack)
			{
				SaveDefaults();
				Response.ClearContent();
				Response.Redirect("PersonalTags.aspx?action=viewall", false);
			}
			else
			{
				LoadToolBar();
				RenderTags();
			}
		}
		
		protected void LoadToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string title = "";
			string helpBtnText = "";
			try
			{
				
				if (defaultTagObjectType == EkEnumeration.CMSObjectTypes.User)
				{
					title = m_containerPage.RefMsg.GetMessage("lbl default user tags");
					helpBtnText = "default_user_tags";
				}
				else if (defaultTagObjectType == EkEnumeration.CMSObjectTypes.CommunityGroup)
				{
					title = m_containerPage.RefMsg.GetMessage("lbl default group tags");
					helpBtnText = "default_group_tags";
				}
				else if (defaultTagObjectType == EkEnumeration.CMSObjectTypes.Content)
				{
					title = m_containerPage.RefMsg.GetMessage("lbl default content tags");
					helpBtnText = "default_content_tags";
				}
				else if (defaultTagObjectType == EkEnumeration.CMSObjectTypes.Library)
				{
					title = m_containerPage.RefMsg.GetMessage("lbl default library tags");
					helpBtnText = "default_library_tags";
				}
				else
				{
					title = m_containerPage.RefMsg.GetMessage("Default Tags");
					helpBtnText = "default_tags";
				}

                divTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar((string)(m_containerPage.TagId > 0 ? title : title));
				result.Append("<table><tr>");
				result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(imagePath + "back.png", (string) ("personaltags.aspx?action=" + (m_containerPage.TagId > 0 ? "viewtag&id=" + m_containerPage.TagId.ToString() : "viewall")), m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), "", StyleHelper.BackButtonCssClass,true));
                result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(imagePath + "save.png", "#", m_containerPage.RefMsg.GetMessage("alt save btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn save personal tag"), "onClick=\"javascript:doSubmit();return false;\"", StyleHelper.SaveButtonCssClass,true));
				
				if (1 == m_containerPage.RefCommonAPI.EnableMultilingual)
				{
					result.Append(StyleHelper.ActionBarDivider);
					result.Append("<td class=\"label\">&#160;" + m_containerPage.RefMsg.GetMessage("generic view") + ":</td>");
					result.Append(m_containerPage.RefStyle.GetShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", Convert.ToString(m_containerPage.RefCommonAPI.ContentLanguage), true));
				}

				result.Append(StyleHelper.ActionBarDivider);
				
				result.Append("<td>");
				result.Append(m_containerPage.RefStyle.GetHelpButton(helpBtnText, ""));
				result.Append("</td>");
				result.Append("</tr></table>");
				
				divToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		private void RenderTags()
		{
			
			TagData[] defaultTags = null;
			Hashtable htLanguages = new Hashtable();
			StringBuilder tagHtmlBuilder = new StringBuilder();
			StringBuilder originalTagListBuilder = new StringBuilder();
			
			error_InvalidChars.Text = m_containerPage.RefMsg.GetMessage("msg error Tag invalid chars");
			
			//Need a hashtable to track the index of each language in the language dropdown
			//this is used by the addFlagInit javascript function so that tha language dropdown value can be reset
			//when the tags are redrawn via JS.
			int langCount = 0;
			for (int i = 0; i <= this.LanguageDataArray.Length - 1; i++)
			{
				if (LanguageDataArray[i].SiteEnabled)
				{
					if (! htLanguages.Contains(LanguageDataArray[i].Id))
					{
						htLanguages.Add(LanguageDataArray[i].Id, langCount);
						langCount++;
					}
				}
			}
			
			tagHtmlBuilder.Append("<input type=\"hidden\" name=\"defaultLangIndex\" id=\"defaultLangIndex\" value=\"" + htLanguages[m_containerPage.RefCommonAPI.DefaultContentLanguage] + "\" /> ");
			tagHtmlBuilder.Append("<input type=\"hidden\" name=\"defaultLang\" id=\"defaultLangIndex\" value=\"" + m_containerPage.RefCommonAPI.DefaultContentLanguage + "\" /> ");
			
			defaultTags = m_tagApi.GetDefaultTags(defaultTagObjectType, m_containerPage.ContentLanguage);
			
			tagHtmlBuilder.Append("<p id=\"parah\"></p>");
			tagHtmlBuilder.Append("<input type=\"hidden\" id=\"Flaglength\" name=\"Flaglength\" value=\"" + defaultTags.Length.ToString() + "\" /><div id=\"pFlag\" name=\"pFlag\">" + Environment.NewLine);
			if (isAdmin)
			{
				if (defaultTags.Length > 0)
				{
					for (int i = 0; i <= (defaultTags.Length - 1); i++)
					{
						
						//keep list of original tags so we can track which ones were removed on Save
						//Format <TagText>~<LanguageID>;<TagText>~<LanguageID>
						if (originalTagListBuilder.Length > 0)
						{
							originalTagListBuilder.Append(";");
						}
						originalTagListBuilder.Append(defaultTags[i].Text);
						
						tagHtmlBuilder.Append("<script type=\"text/javascript\">addFlagInit(" + defaultTags[i].Id.ToString() + ",\'" + defaultTags[i].Text + "\'," + defaultTags[i].LanguageId + "," + htLanguages[defaultTags[i].LanguageId] + " );</script>");
						tagHtmlBuilder.Append("<input type=\"hidden\" name=\"flag_iden" + i.ToString() + "\" id=\"flag_iden" + i.ToString() + "\" value=\"" + defaultTags[i].Id.ToString() + "\" /> ");
						tagHtmlBuilder.Append("<input type=\"text\" id=\"flagdefopt" + i.ToString() + "\" name=\"flagdefopt" + i.ToString() + "\" value=\"" + EkFunctions.HtmlEncode(defaultTags[i].Text) + "\" maxlength=\"50\" onChange=\"javascript:saveFlag(" + i.ToString() + ",this.value,\'tagText\');\">");
						
						
						tagHtmlBuilder.Append("&#160;");
						
						tagHtmlBuilder.Append("<a href=\"#\" onclick=\"javascript:removeFlag(\'" + i.ToString() + "\'); return false;\"><img src=\"" + imagePath + "remove.png\" border=\"0\"/></a>");
						
						tagHtmlBuilder.Append("<div class=\"ektronTopSpaceSmall\"></div>");
					}
				}
				else
				{
					tagHtmlBuilder.Append("<script type=\"text/javascript\">addFlagInit(0, \'\', " + m_containerPage.RefCommonAPI.DefaultContentLanguage + "," + htLanguages[m_containerPage.RefCommonAPI.DefaultContentLanguage] + " );</script>");
					tagHtmlBuilder.Append("<input type=\"hidden\" name=\"flag_iden0\" id=\"flag_iden0\" value=\"0\" /> ");
					tagHtmlBuilder.Append("<input type=\"text\" id=\"flagdefopt0\" name=\"flagdefopt0\" value=\"\" maxlength=\"50\" onChange=\"javascript:saveFlag(0,this.value,\'tagText\');\">");
					
					tagHtmlBuilder.Append("&#160;");
				}
				tagHtmlBuilder.Append("</div>");
				
				tagHtmlBuilder.Append(Environment.NewLine);
				tagHtmlBuilder.Append("<script type=\"text/javascript\">" + Environment.NewLine);
				tagHtmlBuilder.Append(GetJs() + Environment.NewLine);
				tagHtmlBuilder.Append("</script>" + Environment.NewLine);
				literalTags.Text = tagHtmlBuilder.ToString();
				literalAddTag.Text = "<a href=\"#\" title=\"" + m_containerPage.RefMsg.GetMessage("btn add personal tag") + "\" onclick=\"javascript:addFlag(); return false;\"><img src=\"" + imagePath + "add.png\" border=\"0\"/></a><br/>";
				
				//orginalTaglist - for access on postback
				literalAddTag.Text += "<input type=\"hidden\" id=\"originalTags\" name=\"originalTags\" value=\"" + originalTagListBuilder.ToString() + "\"/>";
				
			}
		}
		
		
		private void SaveDefaults()
		{
			
			string newTags = "";
			if (! (Request.Form["newTags"] == null))
			{
				newTags = Request.Form["newTags"].Trim();
			}
			
			string[] newTagsArray = newTags.Split(";".ToCharArray());
			System.Collections.Generic.List<string> newTagsList = new System.Collections.Generic.List<string>(newTagsArray);
			
			Hashtable originalTagHT = new Hashtable();
			string orginalTagList;
			orginalTagList = Request.Form["originalTags"].Trim();
			
			//loop through existing default tags and make sure they havent been removed - if so delete them
			//also - build up a hashtable of original default tags so we can reference them later
			//both tag lists are stored in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>;
			string[] origTagsArray = orginalTagList.Split(";".ToCharArray());
			foreach (string tag in origTagsArray)
			{
				chkTag = tag;
				originalTagHT.Add(tag, "");
				string tagMatch = (string) (newTagsList.Find(FindTagByName));
				
				if (string.IsNullOrEmpty(tagMatch))
				{
					m_tagApi.RemoveTagAsDefault(tag, m_containerPage.ContentLanguage, defaultTagObjectType);
				}
			}
			
			// loop throug tagdata stored in newtags field
			foreach (string tag in newTagsList)
			{
				m_tagApi.SaveTagAsDefault(tag, m_containerPage.ContentLanguage, defaultTagObjectType);
			}
			
		}
		private bool FindTagByName(string str)
		{
			if (str.ToLower() == chkTag.ToLower())
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private string GetJs()
		{
			StringBuilder sbJs = new StringBuilder();
			
			sbJs.Append("function createFlag(fid,id,fname, iloc, itot) {").Append(Environment.NewLine);
			sbJs.Append("       var sRet = \"\";").Append(Environment.NewLine);
			sbJs.Append("       sRet += \"&nbsp;&nbsp;<input type=\\\"text\\\" id=\\\"flagdefopt\" + id + \"\\\" name=\\\"flagdefopt\" + id + \"\\\" value=\\\"\" + fname + \"\\\" maxlength=\\\"50\\\" onChange=\\\"javascript:saveFlag(\" + id + \",this.value,\'tagText\')\\\" />\";").Append(Environment.NewLine);
			sbJs.Append("       sRet += \"<input type=\\\"hidden\\\" name=\\\"flag_iden\" + id + \"\\\" id=\\\"flag_iden\" + id + \"\\\" value=\\\"\" + fid + \"\\\" />\";").Append(Environment.NewLine);
			sbJs.Append("       sRet += \"&#160;\"").Append(Environment.NewLine);
			
			sbJs.Append("       sRet += \"&nbsp;&nbsp;<a href=\\\"#\\\" onclick=\\\"javascript:removeFlag(\" + id + \"); return false;\\\"><img src=\\\"" + this.imagePath + "remove.png\\\" border=\\\"0\\\"/></a>\";").Append(Environment.NewLine);
			sbJs.Append("       sRet += \"<br/>\";").Append(Environment.NewLine);
			sbJs.Append("       return sRet; ").Append(Environment.NewLine);
			sbJs.Append("}").Append(Environment.NewLine);
			
			return sbJs.ToString();
		}
		
		private string GetLanguageDropDownOptionMarkup(int selectedlanguageId)
		{
			
			int i;
			StringBuilder markup = new StringBuilder();
			
			if (IsSiteMultilingual)
			{
				if (!(LanguageDataArray == null))
				{
					for (i = 0; i <= LanguageDataArray.Length - 1; i++)
					{
						if (LanguageDataArray[i].SiteEnabled)
						{
							markup.Append("<option ");
							if (LanguageDataArray[i].Id == selectedlanguageId)
							{
								markup.Append(" selected");
							}
							markup.Append(" value=" +LanguageDataArray[i].Id + ">" + LanguageDataArray[i].LocalName);
						}
					}
				}
			}
			else
			{
				//hardcode to default site language
				markup.Append(" <option selected value=" + m_containerPage.RefCommonAPI.DefaultContentLanguage + ">");
			}
			
			return markup.ToString();
		}
		
		~controls_Community_PersonalTags_TagDefaults()
		{
			//base.Finalize();
		}
		
		public controls_Community_PersonalTags_TagDefaults()
		{
			
		}
	}

