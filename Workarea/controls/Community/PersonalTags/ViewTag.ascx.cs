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



	public partial class controls_Community_PersonalTags_ViewTag : System.Web.UI.UserControl
	{
		
		private TagsAPI m_tagApi;
		private UserAPI m_userApi;
		protected Community_PersonalTags m_containerPage;
		
		public controls_Community_PersonalTags_ViewTag()
		{
			m_tagApi = new Ektron.Cms.Community.TagsAPI();
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonApi refCommonAPI = new CommonApi();
			bool successFlag = false;
			if (m_tagApi.RequestInformationRef.IsMembershipUser == 1)
			{
				Response.Redirect(m_tagApi.ApplicationPath + "reterror.aspx?info=Please login as cms user", true);
				return;
			}
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
					successFlag = DeleteTag();
				}
				
				Response.ClearContent();
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
				m_userApi = new UserAPI();
				txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(m_containerPage.RefMsg.GetMessage("view tag page title"));
				result.Append("<table><tr>");
				result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath + "../UI/Icons/back.png", "personaltags.aspx?action=viewall", m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), "", StyleHelper.BackButtonCssClass, true));
				if (m_userApi.IsAdmin())
				{
					result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath + "../UI/Icons/delete.png", "Javascript:doDeleteSubmit(\'" + tagValid.ClientID + "\', \'" + m_containerPage.RefMsg.GetMessage("js: confirm delete") + "\');", m_containerPage.RefMsg.GetMessage("alt delete button text"), m_containerPage.RefMsg.GetMessage("alt delete button text"), "", StyleHelper.DeleteButtonCssClass, true));
				}
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>");
				result.Append(m_containerPage.RefStyle.GetHelpButton("view_tag", ""));
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
			TagData td;
			SiteAPI siteApi = new SiteAPI();
			LanguageData langData;
			
			try
			{
				tagIdLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic id");
				tagLangLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic language");
				tagNameLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic name");
				tagStatisticsLabel.Text = m_containerPage.RefMsg.GetMessage("lbl tag statistics");
				
				td = m_tagApi.GetTagByID(m_containerPage.TagId);
				if (td != null)
				{
					tagNameLit.Text = td.Text;
					tagIdLit.Text = td.Id.ToString();
					
					if (td.LanguageId == -1)
					{
						tagLangLit.Text = m_containerPage.RefMsg.GetMessage("generic all");
					}
					else
					{
						langData = siteApi.GetLanguageById(td.LanguageId);
						tagLangLit.Text = langData.Name;
					}
					
					tagIdHdn.Value = td.Id.ToString();
					tagLangIdHdn.Value = td.LanguageId.ToString();
					
					SetupStatisticsGrid();
					tagStatsGrid.DataSource = GetStatisticsTable();
					tagStatsGrid.DataBind();
					
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
		
		public void SetupStatisticsGrid()
		{
			
			System.Web.UI.WebControls.BoundColumn column = new System.Web.UI.WebControls.BoundColumn();
			column.DataField = "ObjectType";
			column.HeaderText = m_containerPage.RefMsg.GetMessage("lbl tagged type"); //"Type"
			column.HeaderStyle.CssClass = "ptagsHeaderType";
			column.Initialize();
			column.ItemStyle.Wrap = false;
			column.ItemStyle.VerticalAlign = VerticalAlign.Middle;
			column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			tagStatsGrid.Columns.Add(column);
			
			column = new System.Web.UI.WebControls.BoundColumn();
			column.DataField = "TaggedCount";
			column.HeaderText = m_containerPage.RefMsg.GetMessage("lbl times used"); // "Times Used"
			column.HeaderStyle.CssClass = "ptagsHeaderTagCount";
			column.Initialize();
			column.ItemStyle.Wrap = false;
			column.ItemStyle.VerticalAlign = VerticalAlign.Middle;
			column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			tagStatsGrid.Columns.Add(column);
			
		}
		
		public DataTable GetStatisticsTable()
		{
			DataTable table = new DataTable();
			DataRow row;
			System.Collections.Generic.Dictionary<EkEnumeration.CMSObjectTypes, long> statistics;
			
			string typeLbl = "";
			
			table.Columns.Add(new DataColumn("ObjectType", typeof(string)));
			table.Columns.Add(new DataColumn("TaggedCount", typeof(string)));
			
			statistics = m_tagApi.GetTagStatistics(m_containerPage.TagId);
			foreach (System.Collections.Generic.KeyValuePair<EkEnumeration.CMSObjectTypes, long> stat in statistics)
			{
				
				if (stat.Key == EkEnumeration.CMSObjectTypes.Content)
				{
					typeLbl = m_containerPage.RefMsg.GetMessage("content text");
				}
				else if (stat.Key == EkEnumeration.CMSObjectTypes.CommunityGroup)
				{
					typeLbl = m_containerPage.RefMsg.GetMessage("lbl community groups");
				}
				else if (stat.Key == EkEnumeration.CMSObjectTypes.User)
				{
					typeLbl = m_containerPage.RefMsg.GetMessage("generic users");
				}
				else if (stat.Key == EkEnumeration.CMSObjectTypes.Library)
				{
					typeLbl = m_containerPage.RefMsg.GetMessage("generic library title");
				}
				else
				{
					typeLbl = (string) (stat.Key.ToString());
				}
				
				row = table.NewRow();
				row[0] = typeLbl;
				row[1] = stat.Value.ToString();
				table.Rows.Add(row);
			}
			
			return table;
		}
		
		
		
		public bool DeleteTag()
		{
			bool returnValue;
			bool result = false;
			long tagId = 0;
			
			try
			{
				if (! (Request.Form[this.tagIdHdn.UniqueID] == null) && Ektron.Cms.Common.EkFunctions.IsNumeric(Request.Form[this.tagIdHdn.UniqueID]))
				{
					tagId = Convert.ToInt64((Request.Form[this.tagIdHdn.UniqueID]));
					if (tagId > 0)
					{
						result = m_tagApi.DeleteTagById(tagId);
					}
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
				returnValue = result;
			}
			return returnValue;
		}
	}

