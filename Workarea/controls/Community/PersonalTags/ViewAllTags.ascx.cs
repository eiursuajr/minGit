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
using Ektron.Cms.Community;
using Ektron.Cms.Content;
	public partial class controls_Community_PersonalTags_ViewAllTags : System.Web.UI.UserControl
	{
		private TagsAPI m_tagApi;
		protected Community_PersonalTags m_containerPage;
		private int m_intTotalPages;
		private int m_intCurrentPage = 1;
		private TagOrderBy m_sortOrderBy = TagOrderBy.TaggedCount;
		private string m_sortOrder = "desc";
		
		
		public controls_Community_PersonalTags_ViewAllTags()
		{
			m_tagApi = new Ektron.Cms.Community.TagsAPI();
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_containerPage = (Community_PersonalTags) Page;
			if (m_tagApi.RequestInformationRef.IsMembershipUser == 1)
			{
				Response.Redirect(m_tagApi.ApplicationPath + "reterror.aspx?info=Please login as cms user", true);
				return;
			}
			if (("del" == m_containerPage.Mode) && (! (Request.Form["PTagsSelCBHdn"] == null) ) && (Request.Form["PTagsSelCBHdn"].Trim().Length > 0))
			{
				
				Response.ClearContent();
				Response.Redirect("PersonalTags.aspx?action=viewall", false);
			}
			else
			{
				
				ltlIsPostDataId.Text = tags_isPostData.ClientID;
				LoadToolBar();
				
				//only loadgrid if this is the first load - otherwise let paging navigation handle it.
				if ((Page.IsPostBack && Request.Form[tags_isPostData.UniqueID] != "") || IsPostBack == false)
				{
					LoadGrid();
				}
				
				tags_isPostData.Value = "true";
			}
			
		}
		
		protected void LoadToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			try
			{
				txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(m_containerPage.RefMsg.GetMessage("personal tags page title"));
				
				result.Append("<table><tr>");
                //result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/add.png", "personaltags.aspx?action=addtag", m_containerPage.RefMsg.GetMessage("alt add btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn add personal tag"), "", StyleHelper.AddButtonCssClass))
				
				if (1 == m_containerPage.RefCommonAPI.EnableMultilingual)
				{
					result.Append("<td class=\"label\">&#160;" + m_containerPage.RefMsg.GetMessage("generic view") + ":</td>");
					result.Append(m_containerPage.RefStyle.GetShowAllActiveLanguage(true, "", "javascript:SelLanguage(this.value);", Convert.ToString(m_containerPage.RefCommonAPI.ContentLanguage), true));
				}

				result.Append(StyleHelper.ActionBarDivider);

				result.Append("<td>");
				result.Append(m_containerPage.RefStyle.GetHelpButton("ViewAllTags_ascx", ""));
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
		
		protected void LoadGrid()
		{
			if (! string.IsNullOrEmpty(Request.QueryString["orderBy"]))
			{
                m_sortOrderBy = (TagOrderBy)Convert.ToInt32(Request.QueryString["orderBy"]);
			}
			
			if (! string.IsNullOrEmpty(Request.QueryString["order"]))
			{
				m_sortOrder = Convert.ToString(Request.QueryString["order"]);
			}
			
			System.Web.UI.WebControls.BoundColumn cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fId";
			cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic id"); //"ID"
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fName";
			//cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic name") ' "Name"
			cb.HeaderText = "<a href=\"personaltags.aspx?orderBy=" + TagOrderBy.Text + "&order=" + (m_sortOrderBy == TagOrderBy.Text && m_sortOrder == "asc" ? "desc" : "asc") + "\">" + m_containerPage.RefMsg.GetMessage("generic name") + "</a>";
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fTotal";
			//cb.HeaderText = m_containerPage.RefMsg.GetMessage("lbl times used") ' "Times Used"
			cb.HeaderText = "<a href=\"personaltags.aspx?orderBy=" + TagOrderBy.TaggedCount + "&order=" + (m_sortOrderBy == TagOrderBy.TaggedCount && m_sortOrder == "asc" ? "desc" : "asc") + "\">" + m_containerPage.RefMsg.GetMessage("lbl times used") + "</a>";
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			
			cb = new System.Web.UI.WebControls.BoundColumn();
			cb.DataField = "fLanguage";
			cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic language"); // "Language"
			cb.Initialize();
			_dg.Columns.Add(cb);
			
			_dg.DataSource = CreateMsgData();
			_dg.DataBind();
			
			LoadPageSettings();
		}
		
		protected ICollection CreateMsgData()
		{
			ICollection returnValue;
			DataTable dt = new DataTable();
			DataRow dr;
			int totalTags = 0;
			TagData[] tags;
			LocalizationAPI localizationApi = new LocalizationAPI();
			
			try
			{
				// header:
				dt.Columns.Add(new DataColumn("fId", typeof(string))); // 0
				dt.Columns.Add(new DataColumn("fName", typeof(string))); // 1
				dt.Columns.Add(new DataColumn("fTotal", typeof(string))); // 3
				dt.Columns.Add(new DataColumn("fLanguage", typeof(string))); // 4
				
				// data:
				TagRequestData request = new TagRequestData();
				request.PageSize = m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize;
				request.PageIndex = m_intCurrentPage;
				request.LanguageId = m_containerPage.ContentLanguage;
				request.OrderByDirection = m_sortOrder == "asc" ? EkEnumeration.OrderByDirection.Ascending : EkEnumeration.OrderByDirection.Descending;
				request.OrderBy = m_sortOrderBy;
				tags = m_tagApi.GetAllTags(request, ref totalTags);
				
				//get totalpages
				m_intTotalPages = totalTags / m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize;
				if (m_intTotalPages * m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize < totalTags)
				{
					m_intTotalPages++;
				}
				
				foreach (TagData tag in tags)
				{
					dr = dt.NewRow();
					dr[0] = tag.Id.ToString();
					dr[1] = "<a href=\"?action=viewtag&id=" + tag.Id.ToString() + "\" title=\"" + m_containerPage.RefMsg.GetMessage("btn click to view tag") + "\" target=\"_self\" >" + tag.Text + "</a>";
					dr[2] = tag.TotalUsedCount.ToString();
					dr[3] = "<img src=\'" + localizationApi.GetFlagUrlByLanguageID(tag.LanguageId) + "\' border=\"0\" />";
					dt.Rows.Add(dr);
				}
				
			}
			catch (Exception)
			{
			}
			finally
			{
				returnValue = new DataView(dt);
			}
			return returnValue;
		}
		
		private void LoadPageSettings()
		{
			if (m_intTotalPages <= 1)
			{
				SetPageControlsVisible(false);
			}
			else
			{
				SetPageControlsVisible(true);
				PageLabel.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol page");
				TotalPages.Text = (System.Math.Ceiling(Convert.ToDecimal(m_intTotalPages))).ToString();
				OfLabel.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol of");
				TotalPages.ToolTip =TotalPages.Text;
				CurrentPage.Text = m_intCurrentPage.ToString();
				CurrentPage.ToolTip = CurrentPage.Text;
				PreviousPage.Enabled = true;
				PreviousPage.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol previous page");
				FirstPage.Enabled = true;
                FirstPage.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol first page");
				NextPage.Enabled = true;
                NextPage.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol next page");
				LastPage.Enabled = true;
                LastPage.ToolTip = m_containerPage.RefMsg.GetMessage("lbl pagecontrol last page");
				if (m_intCurrentPage == 1)
				{
					PreviousPage.Enabled = false;
					FirstPage.Enabled = false;
				}
				else if (m_intCurrentPage == m_intTotalPages)
				{
					NextPage.Enabled = false;
					LastPage.Enabled = false;
				}
			}
		}
		private void SetPageControlsVisible(bool areVisible)
		{
			TotalPages.Visible = areVisible;
			CurrentPage.Visible = areVisible;
			PreviousPage.Visible = areVisible;
			NextPage.Visible = areVisible;
			LastPage.Visible = areVisible;
			FirstPage.Visible = areVisible;
			PageLabel.Visible = areVisible;
			OfLabel.Visible = areVisible;
		}
		
		public void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					m_intCurrentPage = 1;
					break;
				case "Last":
					m_intCurrentPage = int.Parse((string) TotalPages.Text);
					break;
				case "Next":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					break;
			}
			LoadGrid();
		}
	}
	

