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



	public partial class controls_analytics_Global : AnalyticsBase
	{
		
		
		protected CommonApi common = new Ektron.Cms.CommonApi();
		
		public override void Initialize()
		{
			
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
			
			string target = common.ApplicationPath + "ContentAnalytics.aspx?type=global";
			
			Description = common.EkMsgRef.GetMessage("site stats");
			
			int report = 1;
			
			if (! (Request.QueryString["report"] == null))
			{
				try
				{
					report = Convert.ToInt32(Request.QueryString["report"]);
				}
				catch (Exception)
				{
					report = 1;
				}
			}
			
			
			// Start off pessamistic - make everything invisible
			ByTimeGraph.Visible = false;
			stats_aggr.Visible = false;
			
			if ((report) == 1) // Quick Statistics
			{
				stats_aggr.Visible = true;
				InitStatsAggr();
			} // By Time
			else if ((report) == 2)
			{
				Description = common.EkMsgRef.GetMessage("site activity");
				ByTimeGraph.Visible = true;
				graph_key.Text = "       <table border=\"0\"><tr><td width=\"20px\" height=\"10px\" bgcolor=\"red\">&nbsp;</td><td>" + common.EkMsgRef.GetMessage("views lbl") + "</td></tr><tr><td width=\"20px\" height=\"10px\" bgcolor=\"blue\">&nbsp;</td><td>" + common.EkMsgRef.GetMessage("visitors lbl") + "</td></tr></table>";
				ByTimeGraph.Text = "<img src=\"" + common.ApplicationPath + "ContentRatingGraph.aspx?type=time&view=" + CurrentView.ToLower() + "&EndDate=" + EkFunctions.UrlEncode(EndDateTime.ToString()) + "\" />";
			} // Popular Content
			else if ((report) == 3)
			{
			} // Popular Pages
			else if ((report) == 4)
			{
			} // Popular URLs
			else if ((report) == 5)
			{
			}
			
			
			
		}
		private void InitStatsAggr()
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

			string visitorSelect = string.Empty;
			
			AnalyticsData.Tables.Clear();
			
            
            {   // hits and visits
                Fill((string) ("SELECT COUNT(hit_count) AS HITS FROM BA_CONTENT_VISITS_SUMMARY WHERE " + DateClause));
                hitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);
                AnalyticsData.Tables.Clear();
                Fill((string)("SELECT COUNT(DISTINCT visitor_id) AS VISITORS FROM BA_CONTENT_VISITS_SUMMARY WHERE " + DateClause));
                visitCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);
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
                Fill((string)("SELECT COUNT(DISTINCT visitor_id) AS NEW FROM BA_CONTENT_VISITS_SUMMARY WHERE visit_type = 0 AND " + DateClause));
                newVisitorCount = EkFunctions.ReadIntegerValue(AnalyticsData.Tables[0].Rows[0][0]);

                visitorSelect = (string)("SELECT COUNT(DISTINCT visitor_id) AS RETURNING " + " FROM BA_CONTENT_VISITS_SUMMARY " + " WHERE visit_type = 1" + " AND visitor_id NOT IN (SELECT DISTINCT visitor_id FROM BA_CONTENT_VISITS_SUMMARY WHERE visit_type = 0 AND " + DateClause + ")" + " AND " + DateClause);
                Fill(visitorSelect);
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
	}
