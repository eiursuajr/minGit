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



	public partial class controls_analytics_Referring_url : AnalyticsBase
	{
		
		
		protected CommonApi common = new Ektron.Cms.CommonApi();
		private string Domain = "";
		
		public override void Initialize()
		{
			
			this.GridView1.PageSize = PageSize;
			this.GridView2.PageSize = PageSize;
			this.GridView3.PageSize = PageSize;
			
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
			lbl_new_vs_returning_visitors.ToolTip = common.EkMsgRef.GetMessage("new to returning visitors");
			
			this.stats_aggr.Visible = false;
			this.Image1.Visible = false;
			
			if (Request.QueryString["id"] == null)
			{
				Description = common.EkMsgRef.GetMessage("lbl Referring Domains");
				AnalyticsData.Clear();
                Fill("SELECT referring_url, COUNT(referring_url) as Referrals FROM BA_CONTENT_URL_SUMMARY " + "WHERE " + DateClause + " AND (visit_type = 0 OR visit_type=1) AND referring_url != \'\\' GROUP BY referring_url");
				
				if (AnalyticsData.Tables[0].Rows.Count == 0)
				{
					ErrMsg.Visible = true;
					ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
					ErrMsg.ToolTip = common.EkMsgRef.GetMessage("lbl no records");
				}
				
				((HyperLinkField) (GridView1.Columns[0])).DataNavigateUrlFormatString = common.ApplicationPath + "/ContentAnalytics.aspx?type=referring&id={0}";
				
				AnalyticsDataView.Table = AnalyticsData.Tables[0];
				
				GridView1.DataSource = AnalyticsDataView;
				GridView1.DataBind();
				
				Image1.Visible = false;
			}
			else
			{
				
				Domain = Request.QueryString["id"];
				Description = (string) (common.EkMsgRef.GetMessage("stats on") + " \'" + Domain + "\'");
				
				string target = common.ApplicationPath + "ContentAnalytics.aspx?type=referring";
				//Me.navBar.Text = "<ul id=""tabnav"">"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=1&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer stats") & "</a></li>"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=2&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer activity") & "</a></li>"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=3&id=" & Domain & ">" & common.EkMsgRef.GetMessage("top landing pages") & "</a></li></ul>"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=4&id=" & Domain & ">" & "Top Landing Content" & "</a></li></ul>"
				
				string t_stats = "tab_disabled";
				string t_activity = "tab_disabled";
				string t_landing = "tab_disabled";
				string t_pages = "tab_disabled";
				
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
					t_landing = "tab_actived";
				}
				else if ((string) (reportType) == "4")
				{
					t_pages = "tab_actived";
				}
				
				this.navBar.Text = "<table height=\"20\" style=\"BACKGROUND-COLOR:white\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr>";
				this.navBar.Text += "<td class=\"" + t_stats + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=1&id=" + Domain + ">" + common.EkMsgRef.GetMessage("referrer stats") + "</a></b></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_activity + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=2&id=" + Domain + ">" + common.EkMsgRef.GetMessage("referrer activity") + "</a></b></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_pages + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=4&id=" + Domain + ">" + "Paths" + "</a></b></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_landing + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=3&id=" + Domain + ">" + common.EkMsgRef.GetMessage("top landing pages") + "</a></b></td>";
				this.navBar.Text += "<td class=\"tab_last\" width=\"91%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "</tr></table>";
				
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
				
				if ((report) == 1) // Quick Statistics
				{
					stats_aggr.Visible = true;
					InitStatsAggr(Domain);
				} // Referrer Activity
				else if ((report) == 2)
				{
					Image1.Visible = true;
					Image1.ImageUrl = common.ApplicationPath + "ContentRatingGraph.aspx?type=time&view=" + CurrentView + "&res_type=referring&res=" + Domain + "&EndDate=" + EkFunctions.UrlEncode(EndDateTime.ToString());
				} // Top Landing Pages
				else if ((report) == 3)
				{
					AnalyticsData.Clear();
                    Fill("SELECT url, COUNT(url) as Landings FROM BA_CONTENT_URL_SUMMARY " + "WHERE " + DateClause + " AND referring_url = \'" + Domain + "\' AND (visit_type = 0 OR visit_type = 1) GROUP BY url");
					
					if (AnalyticsData.Tables[0].Rows.Count == 0)
					{
						ErrMsg.Visible = true;
						ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
						ErrMsg.ToolTip = common.EkMsgRef.GetMessage("lbl no records");
					}
					
					((HyperLinkField) (GridView2.Columns[0])).DataNavigateUrlFormatString = common.ApplicationPath + "ContentAnalytics.aspx?type=page&id={0}";
					
					
					AnalyticsDataView.Table = AnalyticsData.Tables[0];
					GridView2.DataSource = AnalyticsDataView;
					GridView2.DataBind();
				} // Top Landing Content
				else if ((report) == 4)
				{
					AnalyticsData.Clear();
                    Fill("SELECT referring_url_path, COUNT(referring_url_path) as Landings FROM BA_CONTENT_URL_SUMMARY " + "WHERE " + DateClause + " AND referring_url = \'" + Domain + "\' AND referring_url_path != \'\\' AND (visit_type = 0 OR visit_type = 1) GROUP BY referring_url_path");
					
					if (AnalyticsData.Tables[0].Rows.Count == 0)
					{
						ErrMsg.Visible = true;
						ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
						ErrMsg.Text = common.EkMsgRef.GetMessage("lbl no records");
					}
					
					AnalyticsDataView.Table = AnalyticsData.Tables[0];
					GridView3.DataSource = AnalyticsData;
					GridView3.DataBind();
				}
			}
			
		}
		
		private void InitStatsAggr(string ref_url)
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

            Fill("SELECT COUNT(visitor_id) AS HITS, COUNT(DISTINCT visitor_id) AS VISITORS, " + "(SELECT COUNT(DISTINCT visitor_id) FROM BA_CONTENT_URL_SUMMARY WHERE visit_type = 0 AND " + DateClause + " AND referring_url = \'" + ref_url + "\') AS NEW, " + "(SELECT COUNT(DISTINCT visitor_id) FROM BA_CONTENT_URL_SUMMARY WHERE visit_type = 1 AND " + DateClause + " AND referring_url = \'" + ref_url + "\') AS RETURNING FROM BA_CONTENT_URL_SUMMARY WHERE " + DateClause + " AND referring_url = \'" + ref_url + "\'");

            {   // hits and visits
                hitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);
                visitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][1]);
                totalHitsVisits = hitCount + visitCount;

                hitRatio = EkFunctions.GetPercent(hitCount, totalHitsVisits);
                visitRatio = EkFunctions.GetPercent(visitCount, totalHitsVisits);

                num_total_hits.Text = hitCount.ToString();
                num_total_hits.ToolTip = hitCount.ToString();
                num_total_visitors.Text = visitCount.ToString();
                num_total_visitors.ToolTip = visitCount.ToString();
            }

            {   // new and returning
                newVisitorCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][2]);
                returningVisitorCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][3]);
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
				ratio = double.Parse((string) num_total_hits.Text);
				ratio = Math.Round(ratio / double.Parse((string) num_total_visitors.Text), 2);
				if (double.Parse((string) num_total_visitors.Text) == 0)
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
			
			GridView1.PageIndex = PageIndex;
			AnalyticsDataView.Sort = SortExpression;
			
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
			GridView2.PageIndex = PageIndex;
			
			GridView2.DataSource = AnalyticsDataView;
			GridView2.DataBind();
		}
		
		protected void GridView2_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
		{
			PageIndex = e.NewPageIndex;
			
			GridView2.PageIndex = PageIndex;
			AnalyticsDataView.Sort = SortExpression;
			
			GridView2.DataSource = AnalyticsDataView;
			GridView2.DataBind();
		}
		
		protected void GridView3_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
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
			GridView3.PageIndex = PageIndex;
			
			GridView3.DataSource = AnalyticsDataView;
			GridView3.DataBind();
		}
		
		protected void GridView3_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
		{
			PageIndex = e.NewPageIndex;
			
			GridView3.PageIndex = PageIndex;
			AnalyticsDataView.Sort = SortExpression;
			
			GridView3.DataSource = AnalyticsDataView;
			GridView3.DataBind();
		}
	}
	

