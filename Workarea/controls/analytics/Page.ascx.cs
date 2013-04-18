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



	public partial class controls_analytics_Page : AnalyticsBase
	{
		
		
		protected CommonApi common = new Ektron.Cms.CommonApi();
		
		public override void Initialize()
		{
			
			this.GridView1.PageSize = PageSize;
			this.GridView2.PageSize = PageSize;
			
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
			
			this.Image1.Visible = false;
			this.stats_aggr.Visible = false;
			Description = common.EkMsgRef.GetMessage("lbl template stats");
			this.stats_aggr.Visible = false;
			if (Request.QueryString["id"] == null)
			{
				this.Image1.Visible = false;
				GridView1.Enabled = true;
				GridView2.Enabled = false;
				AnalyticsData.Clear();
                Fill("SELECT url, COUNT(DISTINCT visitor_id) AS Visits, COUNT(visitor_id) AS Views FROM BA_CONTENT_URL_SUMMARY " + "WHERE " + DateClause + "GROUP BY url ORDER BY Visits DESC");
				
				if (AnalyticsData.Tables[0].Rows.Count == 0)
				{
					ErrMsg.Visible = true;
					ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
				}
				
				this.AnalyticsDataView.Table = AnalyticsData.Tables[0];
				GridView1.DataSource = AnalyticsData;
				GridView1.Columns[0].HeaderText = "Template";
				GridView1.DataBind();
				
				((HyperLinkField) (GridView1.Columns[0])).DataNavigateUrlFormatString = common.ApplicationPath + "ContentAnalytics.aspx?type=page&id={0}";
				
				GridView1.DataSource = AnalyticsDataView;
				GridView1.DataBind();
			}
			else
			{
				
				GridView1.Enabled = false;
				GridView2.Enabled = true;
				string page_id = Request.QueryString["id"];
				Description = "Statistics on template \'" + page_id + "\'";
				
				string target = common.ApplicationPath + "ContentAnalytics.aspx?type=page&id=" + page_id;
				//Me.navBar.Text = "<ul id=""tabnav"">"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=1>" & common.EkMsgRef.GetMessage("page stats") & "</a></li>"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=2>" & common.EkMsgRef.GetMessage("page activity") & "</a></li>"
				//Me.navBar.Text &= "<li><a href=" & target & "&report=3>" & common.EkMsgRef.GetMessage("content in page") & "</a></li></ul>"
				string t_stats = "tab_disabled";
				string t_activity = "tab_disabled";
				string t_content = "tab_disabled";
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
					t_content = "tab_actived";
				}
				navBar.Text = "<p><strong>" + page_id + "</strong></p>";
				this.navBar.Text += "<table height=\"20\" style=\"BACKGROUND-COLOR:white\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr>";
				this.navBar.Text += "<td class=\"" + t_stats + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=1>&nbsp;" + common.EkMsgRef.GetMessage("lbl template stats") + "&nbsp;</a></b></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_activity + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=2>&nbsp;" + common.EkMsgRef.GetMessage("lbl template activity") + "&nbsp;</a></b></td>";
				this.navBar.Text += "<td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "<td class=\"" + t_content + "\" width=\"1%\" nowrap><b><a border=\"0\" style=\"text-decoration:none;\" href=" + target + "&report=3>&nbsp;" + common.EkMsgRef.GetMessage("lbl content in template") + "&nbsp;</a></b></td>";
				this.navBar.Text += "<td class=\"tab_last\" width=\"91%\" nowrap>&nbsp;</td>";
				this.navBar.Text += "</tr></table>";
				
				int report = 1;
				
				if (! (Request.QueryString["report"] == null))
				{
					report = Convert.ToInt32(Request.QueryString["report"]);
				}
				
				if ((report) == 1) // Quick Statistics
				{
					Description = (string) (common.EkMsgRef.GetMessage("stats on") + " \'" + page_id + "\'");
					this.stats_aggr.Visible = true;
					InitStatsAggr(page_id);
				} // By Time
				else if ((report) == 2)
				{
					Description = (string) (common.EkMsgRef.GetMessage("activity on") + " \'" + page_id + "\'");
					this.Image1.Visible = true;
					graph_key.Text = "<table border=\"0\"><tr><td width=\"20px\" height=\"10px\" bgcolor=\"red\">&nbsp;</td><td>" + "Views" + "</td></tr><tr><td width=\"20px\" height=\"10px\" bgcolor=\"blue\">&nbsp;</td><td>" + "Visitors" + "</td></tr></table>";
					this.Image1.ImageUrl = common.ApplicationPath + "ContentRatingGraph.aspx?type=time&view=" + CurrentView + "&res_type=page&res=" + page_id + "&EndDate=" + EkFunctions.UrlEncode(EndDateTime.ToString());
				} // Content in Page
				else if ((report) == 3)
				{
					Description = (string) (common.EkMsgRef.GetMessage("content viewed on") + " \'" + page_id + "\'");
					MostPopularContent(page_id);
				}
				
				
				
			}
			
		}
		
		
		private void InitStatsAggr(string page_id)
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

			QueryGetAnalyticsInfo(DateClause, page_id);
			if (AnalyticsData.Tables[0].Rows.Count == 0)
			{
				ErrMsg.Visible = true;
				ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range");
				ErrMsg.ToolTip = common.EkMsgRef.GetMessage("lbl no records");
			}

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
		
		private void MostPopularContent(string page_id)
		{
			AnalyticsData.Clear();
			QueryGetAnalyticsInfoForLanguage(DateClause, page_id, common.DefaultContentLanguage.ToString());
			((HyperLinkField) (GridView2.Columns[0])).DataNavigateUrlFormatString = common.ApplicationPath + "ContentAnalytics.aspx?type=content&id={0}&url=" + Request.QueryString["id"];
			
			AnalyticsDataView.Table = AnalyticsData.Tables[0];
			GridView2.DataSource = AnalyticsDataView;
			GridView2.DataBind();
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
			GridView1.PageIndex = e.NewPageIndex;
			
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
			GridView1.PageIndex = PageIndex;
			
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
	}
	
