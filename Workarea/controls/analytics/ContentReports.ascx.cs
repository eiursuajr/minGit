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
using Ektron.Cms.Common;
using Ektron.Cms;


	public partial class controls_analytics_ContentReports : AnalyticsBase
	{
		
		
		protected CommonApi common = new Ektron.Cms.CommonApi();
		protected ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
        protected string mainQuery = "SELECT content.content_title as content_title, COUNT(DISTINCT BA_CONTENT_Visits_SUMMARY.visitor_id) as Visits, " + "SUM(BA_CONTENT_Visits_SUMMARY.hit_count) as Views, BA_CONTENT_Visits_SUMMARY.content_id as content_id FROM BA_CONTENT_Visits_SUMMARY " + "LEFT JOIN content ON content.content_id = BA_CONTENT_Visits_SUMMARY.content_id WHERE content.content_language = {0} AND {1} GROUP BY BA_CONTENT_Visits_SUMMARY.content_id, content.content_title ORDER BY Visits DESC";
        protected string newVisitorQuery = "SELECT COUNT(DISTINCT visitor_id) as new FROM BA_CONTENT_Visits_SUMMARY WHERE BA_CONTENT_Visits_SUMMARY.language_id = {0} and BA_CONTENT_Visits_SUMMARY.content_id = {1} and visit_type = 0 AND {2}";
        protected string returningVisitorQuery = "SELECT COUNT(DISTINCT visitor_id) as returning FROM BA_CONTENT_Visits_SUMMARY WHERE BA_CONTENT_Visits_SUMMARY.language_id = {0} and BA_CONTENT_Visits_SUMMARY.content_id = {1} and visit_type in (1,2) AND {2}";
		
		public override void Initialize()
		{
			navBar.Visible = false;
			Image1.Visible = false;
			stats_aggr.Visible = false;
			
			GridView1.PageSize = PageSize;
			auditContent.PageSize = PageSize;
			
			lbl_total_hits.Text = common.EkMsgRef.GetMessage("total views");
			lbl_total_hits.ToolTip = common.EkMsgRef.GetMessage("total views");
			lbl_total_visitors.Text = common.EkMsgRef.GetMessage("total visitors");
			lbl_total_visitors.ToolTip = common.EkMsgRef.GetMessage("total visitors");
			lbl_hits_per_visitor.Text = common.EkMsgRef.GetMessage("views to visitors");
			lbl_hits_per_visitor.ToolTip = common.EkMsgRef.GetMessage("views to visitors");
			lbl_new_visitors.Text = common.EkMsgRef.GetMessage("new visitors");
			lbl_new_visitors.ToolTip = common.EkMsgRef.GetMessage("new visitors");
			lbl_returning_visitors.Text = common.EkMsgRef.GetMessage("returning visitors");
			lbl_returning_visitors.ToolTip = common.EkMsgRef.GetMessage("returning visitors");
			lbl_hits_vs_visitors.Text = common.EkMsgRef.GetMessage("views to visitors");
			lbl_hits_vs_visitors.ToolTip = common.EkMsgRef.GetMessage("views to visitors");
			
			lbl_new_vs_returning_visitors.Text = common.EkMsgRef.GetMessage("new to returning visitors");
			
			if (Request.QueryString["id"] == null)
			{
				Description = common.EkMsgRef.GetMessage("top content");
				AnalyticsData.Clear();
				Fill(string.Format(mainQuery, common.DefaultContentLanguage, DateClause));
				
				if (AnalyticsData.Tables[0].Rows.Count == 0)
				{
					ErrMsg.Visible = true;
					ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
				}
				
				((HyperLinkField) (GridView1.Columns[0])).DataNavigateUrlFormatString = common.ApplicationPath + "ContentAnalytics.aspx?type=content&id={0}";
				((HyperLinkField) (GridView1.Columns[0])).SortExpression = "content_title";
				
				AnalyticsDataView.Table = AnalyticsData.Tables[0];
				
				GridView1.DataSource = AnalyticsDataView;
				GridView1.DataBind();
				
				this.Image1.Visible = false;
			}
			else
			{
				long content_id;
				
				try
				{
					content_id = Convert.ToInt64(Request.QueryString["id"]);
				}
				catch (Exception)
				{
					content_id = 0;
				}
				
				string t_stats = "tab_disabled";
				string t_activity = "tab_disabled";
				string t_audit = "tab_disabled";
				
				string reportType;
				reportType = Request.QueryString["report"];
				if (reportType == null)
				{
					reportType = "1";
				}
				
				if ((string) (reportType) == "1")
				{
					t_stats = "tab_actived";
				}
				else if ((string) (reportType) == "2")
				{
					t_activity = "tab_actived";
				}
				else if ((string) (reportType) == "3")
				{
					t_audit = "tab_actived";
				}
				
				//TODO: Ross - These need to be converted to jQuery tabs
				navBar.Visible = true;
				string target = common.ApplicationPath + "ContentAnalytics.aspx?type=content";
				//navBar.Text = "<ul id=""tabnav"">"
				//navBar.Text &= "<li><a href=" & target & "&report=1&id=" & content_id & ">" & common.EkMsgRef.GetMessage("content stats") & "</a></li>"
				//navBar.Text &= "<li><a href=" & target & "&report=2&id=" & content_id & ">" & common.EkMsgRef.GetMessage("content activity") & "</a></li>"
				//navBar.Text &= "<li><a href=" & target & "&report=3&id=" & content_id & ">" & common.EkMsgRef.GetMessage("audit content") & "</a></li></ul>"
				Ektron.Cms.ContentData content_data = m_refContentApi.GetContentById(content_id, 0);
				this.navBar.Text = "<p>" + content_data.Title + "</p>";
				this.navBar.Text += "<table height=\"20\" width=\"100%\">";
				this.navBar.Text += "<tr>";
				this.navBar.Text += "<td class=\"" + t_stats + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=" + target + "&report=1&id=" + content_id + ">&nbsp;" + common.EkMsgRef.GetMessage("content stats") + "&nbsp;</a></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_activity + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=" + target + "&report=2&id=" + content_id + ">&nbsp;" + common.EkMsgRef.GetMessage("content activity") + "&nbsp;</a></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_audit + "\" width=\"1%\" nowrap><a style=\"text-decoration:none;\" href=" + target + "&report=3&id=" + content_id + ">&nbsp;" + common.EkMsgRef.GetMessage("audit content") + "&nbsp;</a></td>";
				this.navBar.Text += "<td class=\"tab_last\" width=\"91%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "</tr>";
				this.navBar.Text += "</table>";
				
				int report = 1;
				
				if (! (Request.QueryString["report"] == null))
				{
					try
					{
						report = Convert.ToInt32(Request.QueryString["report"]);
					}
					catch (Exception)
					{
						
					}
				}
				Description = (string) ("ContentID=" + content_id);
				
				if ((report) == 1) // Quick Statistics
				{
					Description = (string) ("Statistics for ContentID=" + content_id);
					stats_aggr.Visible = true;
					InitStatsAggr(content_id);
				} // By Time
				else if ((report) == 2)
				{
					Description = (string) ("Activity by time for ContentID=" + content_id);
					Image1.Visible = true;
					graph_key.Text = "       <table border=\"0\"><tr><td width=\"20px\" height=\"10px\" bgcolor=\"red\">&nbsp;</td><td>" + "Views" + "</td></tr><tr><td width=\"20px\" height=\"10px\" bgcolor=\"blue\">&nbsp;</td><td>" + "Visitors" + "</td></tr></table>";
					Image1.ImageUrl = common.ApplicationPath + "ContentRatingGraph.aspx?type=time&view=" + CurrentView + "&res_type=content&res=" + content_id + "&EndDate=" + EkFunctions.UrlEncode(EndDateTime.ToString());
				} // Audit Content
				else if ((report) == 3)
				{
					Description = (string) ("User Views for ContentID=" + content_id);
					InitAuditContent(content_id);
				}
			}
		}
		
		private void InitStatsAggr(long content_id)
        {
            int newVisitorCount = 0;
            int returningVisitorCount = 0;
            int totalNewReturningCount = 0;
            float newVisitorRatio = 0;
            float returningVisitorRatio = 0;

            int hitCount = 0;
            int visitCount = 0;
            int totalHitsVisits = 0;
            float hitRatio = 0;
            float visitRatio = 0;

            string whereParam = DateClause + string.Format(" and BA_CONTENT_Visits_SUMMARY.content_id = {0} ", content_id);
            Fill(string.Format(mainQuery, common.DefaultContentLanguage, whereParam));
            
            if (AnalyticsData.Tables[0].Rows.Count == 0)
            {
                ErrMsg.Visible = true;
                ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
                ErrMsg.ToolTip = common.EkMsgRef.GetMessage("lbl no records");
            }

            {   // hits and visits
                visitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][1]);
                hitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][2]);
                totalHitsVisits = hitCount + visitCount;

                hitRatio = EkFunctions.GetPercent(hitCount, totalHitsVisits);
                visitRatio = EkFunctions.GetPercent(visitCount, totalHitsVisits);

                num_total_hits.Text = hitCount.ToString();
                num_total_hits.ToolTip = hitCount.ToString();
                num_total_visitors.Text = visitCount.ToString();
                num_total_visitors.ToolTip = visitCount.ToString();
            }

            {   // new and returning
                AnalyticsData.Tables.Clear();
                Fill(string.Format(newVisitorQuery, common.DefaultContentLanguage, content_id, DateClause));
                newVisitorCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);

                AnalyticsData.Tables.Clear();
                Fill(string.Format(returningVisitorQuery, common.DefaultContentLanguage, content_id, DateClause));
                returningVisitorCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);

                totalNewReturningCount = newVisitorCount + returningVisitorCount;

                newVisitorRatio = EkFunctions.GetPercent(newVisitorCount, totalNewReturningCount);
                returningVisitorRatio = EkFunctions.GetPercent(returningVisitorCount, totalNewReturningCount);

                num_new_visitors.Text = newVisitorCount.ToString();
                num_new_visitors.ToolTip = newVisitorCount.ToString();
                num_returning_visitors.Text = returningVisitorCount.ToString();
                num_returning_visitors.ToolTip = returningVisitorCount.ToString();
            }

            double ratio = 0;

            try
            {
                ratio = double.Parse((string)num_total_hits.Text);
                ratio = Math.Round(ratio / double.Parse((string)num_total_visitors.Text), 2);
                if (double.Parse((string)num_total_visitors.Text) == 0)
                {
                    ratio = 0;
                }
            }
            catch (Exception)
            {
                ratio = 0;
            }

            if (ratio == 0)
            {
                num_hits_per_visitor.Text = "N/A";
            }
            else
            {
                num_hits_per_visitor.Text = ratio.ToString();
            }

            {
                graph_hits_per_visitor.BriefDescription = common.EkMsgRef.GetMessage("views to visitors");
                graph_hits_per_visitor.LoadData(new List<float>() { hitRatio, visitRatio });
                graph_hits_per_visitor.LoadColors(new List<string>() { "FF0000", "0000FF" });
                graph_hits_per_visitor.LoadNames(new List<string>() { common.EkMsgRef.GetMessage("total views"), common.EkMsgRef.GetMessage("total visitors") });
            }
            {
                graph_new_vs_returning_visitors.BriefDescription = common.EkMsgRef.GetMessage("new to returning visitors");
                graph_new_vs_returning_visitors.LoadData(new List<float>() { newVisitorRatio, returningVisitorRatio });
                graph_new_vs_returning_visitors.LoadColors(new List<string>() { "FF0000", "0000FF" });
                graph_new_vs_returning_visitors.LoadNames(new List<string>() { common.EkMsgRef.GetMessage("new visitors"), common.EkMsgRef.GetMessage("returning visitors") });
            }
        }
		
		private void InitAuditContent(long content_id)
		{
			AnalyticsData.Tables.Clear();
			Fill("SELECT DISTINCT users.user_name as UserName, users.first_name as FirstName, users.last_name as LastName, MAX(content_hits_tbl.hit_date) as HitDate " + "FROM content_hits_tbl INNER JOIN users ON content_hits_tbl.user_id = users.user_id WHERE content_hits_tbl.content_id = " + content_id + "GROUP BY users.user_name, users.last_name, users.first_name");
			
			AnalyticsDataView.Table = AnalyticsData.Tables[0];
			
			auditContent.DataSource = AnalyticsDataView;
			auditContent.DataBind();
		}
		
		protected void GridView1_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
		{
			if (SortOrder == SortDirection.Descending)
			{
				SortExpression = e.SortExpression + " DESC";
			}
			else
			{
				SortExpression = e.SortExpression + " ASC";
			}
			
			AnalyticsDataView.Sort = SortExpression;
			GridView1.PageIndex = PageIndex;
			
			GridView1.DataSource = AnalyticsDataView;
			GridView1.DataBind();
		}
		
		protected void GridView1_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
		{
			PageIndex = e.NewPageIndex;
			
			AnalyticsDataView.Sort = SortExpression;
			GridView1.PageIndex = PageIndex;
			
			GridView1.DataSource = AnalyticsDataView;
			GridView1.DataBind();
		}
		
		protected void GridView2_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
		{
			if (SortOrder == SortDirection.Descending)
			{
				SortExpression = e.SortExpression + " DESC";
			}
			else
			{
				SortExpression = e.SortExpression + " ASC";
			}
			
			AnalyticsDataView.Sort = SortExpression;
			auditContent.PageIndex = PageIndex;
			
			auditContent.DataSource = AnalyticsDataView;
			auditContent.DataBind();
		}
		
		protected void GridView2_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
		{
			PageIndex = e.NewPageIndex;
			
			AnalyticsDataView.Sort = SortExpression;
			auditContent.PageIndex = PageIndex;
			
			auditContent.DataSource = AnalyticsDataView;
			auditContent.DataBind();
		}
		
	}
	
