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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Analytics;
using Ektron.Cms.Common;

	public partial class ViewHistoryList : System.Web.UI.UserControl
	{
		
		
		#region Private members
		
		protected ContentAPI m_refContentApi;
		protected string AppName = "";
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected int ContentLanguage = 0;
		protected long m_contentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
		protected long ContentId = 0;
		protected bool _analyticsEnabled = false;
		protected bool ShowBackButton = true;
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			
			try
			{
				
				m_refContentApi = new ContentAPI();
				_analyticsEnabled = AnalyticsSecurity.Enabled(m_refContentApi.RequestInformationRef);
				RegisterResources();
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
				
				if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
				{
					m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
				}
				else
				{
					m_refContentApi.ContentLanguage = ContentLanguage;
				}
				
				m_refMsg = m_refContentApi.EkMsgRef;
				
				if (!(Request.QueryString["id"] == null))
				{
					if (Request.QueryString["id"] != "")
					{
						ContentId = Convert.ToInt64(Request.QueryString["id"]);
					}
				}
				if (!(Request.QueryString["showbackbutton"] == null))
				{
					if (Request.QueryString["showbackbutton"] != "")
					{
						ShowBackButton = Convert.ToBoolean(Request.QueryString["showbackbutton"]);
					}
				}
				
				
				if (ContentId > 0)
				{
					m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId);
				}
				
				switch (m_contentType)
				{
					
					case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:
						
						Populate_EntryHistoryListGrid(ContentId);
						DisplayEntryHistoryToolBar();
						Util_SetResources();
						break;
						
					default:
						
						Populate_HistoryListGrid(ContentId);
						DisplayHistoryToolBar();
						Util_SetResources();
						break;
						
				}
				
			}
			catch (Exception ex)
			{
				Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + ex.Message, false);
			}
			
		}
		
		
		#region Catalog Entry
		
		private void DisplayEntryHistoryToolBar()
		{
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view catalog entry history title"));
            htmToolBar.InnerHtml = "<table><tr>";
            if (ShowBackButton)
            {
                htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
            }

            if (_analyticsEnabled)
            {
                htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/contentCompareAnalytics.png", "javascript:CompareAnalytics(" + ContentId.ToString() + ", " + ContentLanguage.ToString() + ");", m_refMsg.GetMessage("lbl compare analytics"), m_refMsg.GetMessage("lbl compare analytics"), "", StyleHelper.CompareAnalyticsButtonCssClass, true);
            }

            htmToolBar.InnerHtml += "<td>" + m_refStyle.GetHelpButton("ViewContentHistoryList", "") + "</td>";

            htmToolBar.InnerHtml += "</tr></table>";
		}
		
		private void Populate_EntryHistoryListGrid(long Id)
		{
			
			Ektron.Cms.Commerce.CatalogEntryApi m_refCatalogAPI = new Ektron.Cms.Commerce.CatalogEntryApi();
			DataTable dt = new DataTable();
			string strLink = "";
            List<Ektron.Cms.Commerce.EntryVersionData> entry_version_list = new List<Ektron.Cms.Commerce.EntryVersionData>();
			int offset = 0;
			int majorVersions = 0;
			
			if (Id != -1)
			{
				
				entry_version_list = m_refCatalogAPI.GetVersionList(Id, ContentLanguage);
				
			}
			
			if (!(entry_version_list == null))
			{
				
				if (_analyticsEnabled)
				{
					offset = 1;
					HistoryListGrid.Columns.Add(GetColumn("EkCompare", m_refMsg.GetMessage("lbl compare"), false, "title-header bottom"));
				}
				HistoryListGrid.Columns.Add(GetColumn("EkVersion", m_refMsg.GetMessage("version"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("PublishDate", (string) (m_refMsg.GetMessage("hist list title") + "<div class=\"caption ektronCaption\">(<span style=\"background-image: url(\'" + m_refContentApi.AppPath + "images/ui/icons/forward.png\'); display: inline-block; width: 16px; height: 16px; background-position: center center; background-repeat: no-repeat;text-indent: -10000px\">&nbsp;</span> = " + m_refMsg.GetMessage("lbl content pd label") + ")</div>"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("TITLE", m_refMsg.GetMessage("generic title"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("Editor", m_refMsg.GetMessage("content LUE label"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("Comments", m_refMsg.GetMessage("comment text"), false, "title-header bottom contentHistoryComment"));
				
				HistoryListGrid.BorderColor = System.Drawing.Color.White;
				
				DataRow dr;
				if (_analyticsEnabled)
				{
					dt.Columns.Add(new DataColumn("EkCompare", typeof(string)));
				}
				dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
				dt.Columns.Add(new DataColumn("PublishDate", typeof(string)));
				dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
				dt.Columns.Add(new DataColumn("Editor", typeof(string)));
				dt.Columns.Add(new DataColumn("Comments", typeof(string)));
				
				int minorRev;
				int numMajors;
				int pntr;
				bool firstRadio = true;
				minorRev = 0;
				numMajors = 0;
				pntr = 0;
				
				int[] minorarray = new int[entry_version_list.Count + 1];
				for (int i = 0; i <= (entry_version_list.Count - 1); i++)
				{
					if (entry_version_list[i].Status == "A")
					{
						minorarray[numMajors] = minorRev;
						numMajors++;
						minorRev = 0;
					}
					else
					{
						minorRev++;
					}
				}
				minorarray[numMajors] = minorRev; // This is really fist 1
				minorRev = minorarray[pntr];
				for (int i = 0; i <= (entry_version_list.Count - 1); i++)
				{
					dr = dt.NewRow();
					//class=""history-list""
                    strLink = "<a href=\"history.aspx?LangType=" + entry_version_list[i].Entry.LanguageId + "&hist_id=" + entry_version_list[i].VersionId + "&Id=" + Id + "\" target=\"history_frame\" title=\"" + m_refMsg.GetMessage("view this version msg") + "\">";
					
					dr[1 + offset] = strLink;
					
					if (entry_version_list[i].Status == "A")
					{
						
						minorRev = 0;
						dr[offset] = numMajors + ".0";
						pntr++;
						minorRev = minorarray[pntr];
						numMajors--;
						
						string radiochecked = "";
						if (firstRadio)
						{
							firstRadio = false;
							radiochecked = "checked ";
						}
						
						dr[1 + offset] += "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/forward.png\" align=\"bottom\" alt=\"Published\" title=\"Published\" />";
                        dr[1 + offset] += entry_version_list[i].Date.ToShortDateString() + " " + entry_version_list[i].Date.ToShortTimeString();
						if (_analyticsEnabled)
						{
                            dr[0] = "<span class=\"compare_option_primary\"><input class=\"compare\" value=\"" + entry_version_list[i].VersionId + "\" id=\"oldid\" name=\"oldid\" type=\"radio\" " + radiochecked + "/></span>";
                            dr[0] += "<span class=\"compare_option_secondary\"><input class=\"compare\" value=\"" + entry_version_list[i].VersionId + "\" id=\"diff\" name=\"diff\" type=\"radio\" " + radiochecked + "/></span>";
						}
						
						majorVersions++;
						
					}
					else
					{
						dr[offset] = numMajors + "." + minorRev;
						minorRev--;
                        dr[1 + offset] += "<div style=\'margin-left:15px;\'>" + entry_version_list[i].Date.ToShortDateString() + " " + entry_version_list[i].Date.ToShortTimeString() + "</div>";
						
					}
					dr[1 + offset] += "</a>";
                    dr[2 + offset] = strLink + entry_version_list[i].Entry.Title.ToString() + "</a>";
                    dr[3 + offset] = "<a href=\"#\" onclick=\"EmailUser(" + entry_version_list[i].UserId.ToString() + ", \'" + m_refMsg.GetMessage("btn email") + "\'); return false;\">";
					dr[3 + offset] += "<img alt=\"" + m_refMsg.GetMessage("btn email") + "\" title=\"" + m_refMsg.GetMessage("btn email") + "\" src=\"" + m_refContentApi.AppPath + "Images/ui/icons/email.png\" />&#160;";
                    dr[3 + offset] += entry_version_list[i].Entry.LastEditorFirstName + " " + entry_version_list[i].Entry.LastEditorLastName + "</a>";
                    dr[4 + offset] = entry_version_list[i].Comment;
					
					dt.Rows.Add(dr);
				}
				
				_analyticsEnabled = System.Convert.ToBoolean(majorVersions > 1);
				
			}
			else
			{
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "EkVersion";
				colBound.HeaderText = m_refMsg.GetMessage("lbl history status");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.Initialize();
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
				colBound.ItemStyle.CssClass = "history-list";
				colBound.HeaderStyle.Height = Unit.Empty;
				HistoryListGrid.Columns.Add(colBound);
				
				dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
				
				DataRow dr;
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("msg current history");
				dt.Rows.Add(dr);
				
				_analyticsEnabled = false;
				
			}
			
			DataView dv = new DataView(dt);
			
			HistoryListGrid.DataSource = dv;
			HistoryListGrid.DataBind();
			
		}
		
		#endregion
		
		
		#region Other
		
		private void DisplayHistoryToolBar()
		{
			
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view content history title"));

			htmToolBar.InnerHtml = "<table><tr>";

			if (ShowBackButton)
			{
				htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true);
			}

			if (_analyticsEnabled)
			{
				htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/contentCompareAnalytics.png", "javascript:CompareAnalytics(" + ContentId.ToString() + ", " + ContentLanguage.ToString() + ");", m_refMsg.GetMessage("lbl compare analytics"), m_refMsg.GetMessage("lbl compare analytics"), "", StyleHelper.CompareAnalyticsButtonCssClass, true);
			}
			
			htmToolBar.InnerHtml += "<td>" + m_refStyle.GetHelpButton("ViewContentHistoryList", "") + "</td>";

			htmToolBar.InnerHtml += "</tr></table>";
		}
		
		private void Populate_HistoryListGrid(long Id)
		{
			
			DataTable dt = GetHistoryList(Id);
			DataView dv = new DataView(dt);
			HistoryListGrid.DataSource = dv;
			HistoryListGrid.DataBind();
			
		}
		
		private DataTable GetHistoryList(long id)
		{
			int i = 0;
			DataTable dt = new DataTable();
			string strLink = "";
			ContentHistoryData[] content_history_list = null;
			int offset = 0;
			int majorVersions = 0;
			
			if (id != -1)
			{
				content_history_list = m_refContentApi.GetHistoryList(id);
			}
			if (!(content_history_list == null))
			{
				
				if (_analyticsEnabled)
				{
					offset = 1;
                    HistoryListGrid.Columns.Add(GetColumn("EkCompare", m_refMsg.GetMessage("lbl compare") + "<div class=\"caption ektronCaption\">(" + m_refMsg.GetMessage("lbl web traffic analytics") + ")</div>", false, "title-header bottom"));
				}
				HistoryListGrid.Columns.Add(GetColumn("EkVersion", m_refMsg.GetMessage("lbl version"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("PublishDate", (string) (m_refMsg.GetMessage("hist list title") + "<div class=\"caption ektronCaption\">(<span style=\"background-image: url(\'" + m_refContentApi.AppPath + "images/ui/icons/forward.png\'); display: inline-block; width: 16px; height: 16px; background-position: center center; background-repeat: no-repeat;text-indent: -10000px\">&nbsp;</span> = " + m_refMsg.GetMessage("lbl content pd label") + ")</div>"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("TITLE", m_refMsg.GetMessage("generic title"), false, "title-header bottom contentHistoryTitle"));
				HistoryListGrid.Columns.Add(GetColumn("Editor", m_refMsg.GetMessage("lbl content lue label"), false, "title-header bottom"));
				HistoryListGrid.Columns.Add(GetColumn("Comments", m_refMsg.GetMessage("comment text"), false, "title-header bottom contentHistoryComment"));
				
				//HistoryListGrid.BorderColor = Drawing.Color.White
				
				DataRow dr;
				if (_analyticsEnabled)
				{
					dt.Columns.Add(new DataColumn("EkCompare", typeof(string)));
				}
				dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
				dt.Columns.Add(new DataColumn("PublishDate", typeof(string)));
				dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
				dt.Columns.Add(new DataColumn("Editor", typeof(string)));
				dt.Columns.Add(new DataColumn("Comments", typeof(string)));
				
				int minorRev;
				int numMajors;
				int pntr;
				bool firstRadio = true;
				bool nextRadio = true;
				minorRev = 0;
				numMajors = 0;
				pntr = 0;
				
				int[] minorarray = new int[content_history_list.Length + 1];
				for (i = 0; i <= content_history_list.Length - 1; i++)
				{
					if (content_history_list[i].Status == "A")
					{
						minorarray[numMajors] = minorRev;
						numMajors++;
						minorRev = 0;
					}
					else
					{
						minorRev++;
					}
				}
				minorarray[numMajors] = minorRev; // This is really fist 1
				minorRev = minorarray[pntr];
				for (i = 0; i <= content_history_list.Length - 1; i++)
				{
					dr = dt.NewRow();
					//class=""history-list""
					strLink = "<a href=\"history.aspx?LangType=" + content_history_list[i].LanguageId + "&hist_id=" + content_history_list[i].Id + "&Id=" + id + "\" target=\"history_frame\" title=\"" + m_refMsg.GetMessage("view this version msg") + "\">";
					dr[1 + offset] = strLink;
					if (content_history_list[i].Status == "A")
					{
						
						minorRev = 0;
						dr[offset] = numMajors + ".0";
						pntr++;
						minorRev = minorarray[pntr];
						numMajors--;
						
						string radiochecked = "";
						string nextradiochecked = "";
						if (firstRadio)
						{
							firstRadio = false;
							radiochecked = "checked ";
						}
						else if (nextRadio)
						{
							nextRadio = false;
							nextradiochecked = "checked ";
						}
						
						dr[1 + offset] += "<img src=\"" + m_refContentApi.AppPath + "Images/ui/icons/forward.png\" align=\"bottom\" alt=\"Published\" title=\"Published\" />";
						dr[1 + offset] += content_history_list[i].DateInserted.ToShortDateString() + " " + content_history_list[i].DateInserted.ToShortTimeString();
						if (_analyticsEnabled)
						{
							dr[0] = "<span class=\"compare_option_primary\"><input class=\"compare\" value=\"" + content_history_list[i].Id + "\" name=\"oldid\" type=\"radio\" " + radiochecked + "/></span>";
							dr[0] += "<span class=\"compare_option_secondary\"><input class=\"compare\" value=\"" + content_history_list[i].Id + "\" name=\"diff\" type=\"radio\" " + nextradiochecked + "/></span>";
						}
						
						majorVersions++;
						
					}
					else
					{
						
						dr[offset] = numMajors + "." + minorRev;
						minorRev--;
						dr[1 + offset] += "<div style=\'margin-left:15px;\'>" + content_history_list[i].DateInserted.ToShortDateString() + " " + content_history_list[i].DateInserted.ToShortTimeString() + "</div>";
						
					}
					dr[1 + offset] += "</a>";
					dr[2 + offset] = strLink + content_history_list[i].Title.ToString() + "</a>";
					if (content_history_list[i].DisplayName != "")
					{
						dr[3 + offset] = content_history_list[i].DisplayName;
					}
					else
					{
						dr[3 + offset] = content_history_list[i].UserName;
					}
					dr[4 + offset] = content_history_list[i].Comment;
					
					dt.Rows.Add(dr);
				}
				
				_analyticsEnabled = System.Convert.ToBoolean(majorVersions > 1);
				
			}
			else
			{
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "EkVersion";
				colBound.HeaderText = m_refMsg.GetMessage("lbl history status");
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.Initialize();
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
				colBound.ItemStyle.CssClass = "history-list";
				colBound.HeaderStyle.Height = Unit.Empty;
				HistoryListGrid.Columns.Add(colBound);
				
				dt.Columns.Add(new DataColumn("EkVersion", typeof(string)));
				
				DataRow dr;
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("msg current history");
				dt.Rows.Add(dr);
				
				_analyticsEnabled = false;
				
			}
			
			return dt;
		}
		
		#endregion
		
		
		#region Private Helpers
		
		
		private void Util_SetResources()
		{
			
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/history/historyList.css", "HistoryList");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/history/historylist.js", "HistoryListJs", true);
			
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
		
		private System.Web.UI.WebControls.BoundColumn GetColumn(string dataField, string headerText, bool wrap, string cssClass)
		{
			
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound.DataField = dataField;
			colBound.HeaderText = headerText;
			colBound.Initialize();
			colBound.HeaderStyle.CssClass = cssClass;
			
			if (dataField.ToLower() == "comments")
			{
				colBound.ItemStyle.Width = Unit.Parse("450");
			}
			colBound.ItemStyle.Wrap = wrap;
			colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			colBound.ItemStyle.CssClass = "history-list";
			colBound.HeaderStyle.Height = Unit.Empty;
			return colBound;
			
		}
		
		
		#endregion
		
		
	}
