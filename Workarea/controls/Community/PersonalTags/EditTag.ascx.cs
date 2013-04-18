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

	public partial class controls_Community_PersonalTags_EditTag : System.Web.UI.UserControl
	{
		protected Community_PersonalTags m_containerPage;
		private TagsAPI m_tagApi;
		
		public controls_Community_PersonalTags_EditTag()
		{
			m_tagApi = new Ektron.Cms.Community.TagsAPI();
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			bool successFlag = false;
			
			m_containerPage = (Community_PersonalTags) Page;
			
			//If (("del" = m_containerPage.Mode) AndAlso (Not IsNothing(Request.Form("PTagsSelCBHdn"))) AndAlso (Request.Form("PTagsSelCBHdn").Trim.Length > 0)) Then
			//Dim sDelList() As String = (Request.Form("PTagsSelCBHdn").Trim.Split(","))
			//Dim idx As Integer
			//Dim delList() As Integer = Array.CreateInstance(GetType(Integer), sDelList.Length)
			//For idx = 0 To sDelList.Length - 1
			//	If (IsNumeric(sDelList(idx))) Then
			//		delList.SetValue(CType(sDelList(idx), Integer), idx)
			//	End If
			//Next
			
			if (IsPostBack)
			{
				if (! (Request.Form[tagValid.UniqueID] == null) && ("1" == Request.Form[tagValid.UniqueID]))
				{
					// TODO: If error, display failure message:
					successFlag = SaveData();
				}
				//Response.ClearContent()
				Response.Redirect("PersonalTags.aspx?action=viewall", false);
			}
			else
			{
				LoadToolBar();
				DisplayInfo();
			}
			
		}
		
		protected void LoadToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			try
			{
				txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar((string) (m_containerPage.TagId > 0 ? (m_containerPage.RefMsg.GetMessage("edit personal tag page title")) : (m_containerPage.RefMsg.GetMessage("add personal tag page title"))));
				result.Append("<table><tr>");
				result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath + "../UI/Icons/back.png", (string) ("personaltags.aspx?action=" + (m_containerPage.TagId > 0 ? "viewtag&id=" + m_containerPage.TagId.ToString() : "viewall")), m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), "", StyleHelper.BackButtonCssClass,true));
                result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath + "../UI/Icons/save.png", "Javascript:doSubmit(\'" + tagValid.UniqueID + "\');", m_containerPage.RefMsg.GetMessage("alt save btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn save personal tag"), "", StyleHelper.SaveButtonCssClass,true));

				result.Append(StyleHelper.ActionBarDivider);
				
				result.Append("<td>");
				result.Append(m_containerPage.RefStyle.GetHelpButton("EditTag_ascx", ""));
				result.Append("</td>");
				result.Append("</tr></table>");
				
				htmToolBar.InnerHtml = result.ToString();
				result = null;
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		public void DisplayInfo()
		{
			TagData td = new TagData();
			SiteAPI siteApi = new SiteAPI();
			LanguageData langData;
			int language = 0;
			
			try
			{
				
				tagLangLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic language");
				tagNameLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic name");
				
				// If editing an existing tag, show current values:
				if (0 < m_containerPage.TagId)
				{
					tagIdLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic id") + ":&#160;";
					td = m_tagApi.GetTagByID(m_containerPage.TagId);
					if (td != null)
					{
						tagIdLit.Text = "<b>" + td.Id.ToString() + "</b><br />";
						tagNameTxt.Text = td.Text;
						tagDescTxt.Text = "";
						tagLangIdHdn.Value = td.LanguageId.ToString();
						
						language = td.LanguageId;
					}
				}
				else
				{
					language = m_containerPage.ContentLanguage;
					
					
				}
				
				if (language == -1)
				{
					tagLangLit.Text = m_containerPage.RefMsg.GetMessage("generic all");
				}
				else
				{
					langData = siteApi.GetLanguageById(language);
					tagLangLit.Text = langData.Name;
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
				td = null;
			}
			
		}
		
		public bool SaveData()
		{
			bool returnValue;
			bool result = false;
			TagData td = new TagData();
			
			try
			{
				if (0 < m_containerPage.TagId)
				{
					// Editing existing tag:
					td = m_tagApi.GetTagByID(m_containerPage.TagId);
					if (td != null)
					{
						td.Text = Request.Form[(string) this.tagNameTxt.UniqueID];
						//td.Description = Request.Form.Item(Me.tagDescTxt.UniqueID)
						td = m_tagApi.EditTag(ref td);
						result = System.Convert.ToBoolean((td != null) && (td.Id == m_containerPage.TagId));
					}
				}
				else
				{
					// Adding new tag:
					td = new TagData();
					td.LanguageId = m_containerPage.RefUserApi.ContentLanguage;
					td.Type = TagTypes.All;
					td.Text = Request.Form[(string) this.tagNameTxt.UniqueID];
					//td.Description = Request.Form.Item(Me.tagDescTxt.UniqueID)
					result = Convert.ToBoolean(m_tagApi.AddTag(ref td));
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
				returnValue = result;
				td = null;
			}
			return returnValue;
		}
	}