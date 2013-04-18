<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AnalyticsReportHelper.aspx.cs" Inherits="Widgets_AnalyticsView_AnalyticsReportHelper" %>
<%@ Register TagPrefix="analytics" TagName="Report" Src="../../Analytics/reporting/Report.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Trend" Src="../../Analytics/reporting/Trend.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Detail" Src="../../Analytics/reporting/Detail.ascx" %>
<%@ Register TagPrefix="EktronWorkarea" TagName="GroupedDropDownList" Src="../../controls/generic/dropdownlist/GroupedDropDownList.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="AnalyticReportSubtree" Src="../../controls/NavigationTrees/GoogleAnalyticsReportSubtree.ascx" %>
<%@ Register TagPrefix="analytics" TagName="SiteSelector" Src="../../Analytics/controls/SiteSelector.ascx" %> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" rel="Stylesheet" href="../../Personalization/css/ektron.personalization.css" />

<style type="text/css">
    body { margin: 0px; }
    
    .AnalyticsReportWidget_Edit
    {
    	clear: both;
    }
    
    .AnalyticsReportWidget_Edit .ReportSelect
    {
    	float: left;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .ViewSelect
    {
    	float: right;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .PeriodSelect
    {
    	float: left;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .SiteSelect
    {
    	float: right;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .separator
    {
    	clear: both;
    }

    .AnalyticsReportWidget_Edit .buttons
    {
    	clear: both;
    	margin: 0.5em;
    }
    
    div.AnalyticsReportHelper .InnerRegion
    {
    	padding: 2px;
    }
    
    div.AnalyticsReportHelper .AnalyticsReportTitle,
    div.AnalyticsReportHelper .AnalyticsReportDateRangeDisplay,
    div.AnalyticsReportHelper .AnalyticsReportSummary {margin-top: 8px; margin-bottom: 8px;}
    
    div.AnalyticsReportHelper .ektronPageGrid .InnerRegion { margin-left: 2px; margin-right: 2px; }
</style>

<script type="text/javascript" language="javascript">
</script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div id="AnalyticsReportHelper_Container" class="AnalyticsReportHelper" runat="server" >
        <asp:MultiView ID="AnalyticsViews" runat="server">
	        <asp:View ID="ReportView" runat="server">
		        <analytics:Report ID="AnalyticsReport" LineChartWidth="410px" runat="server" />
	        </asp:View>
	        <asp:View ID="TrendView" runat="server">
		        <analytics:Trend ID="AnalyticsTrend" runat="server" />
	        </asp:View>
        </asp:MultiView>    
    </div>
    </form>
</body>
</html>
