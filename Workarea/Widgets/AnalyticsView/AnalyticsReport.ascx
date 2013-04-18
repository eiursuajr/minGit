<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AnalyticsReport.ascx.cs" Inherits="AnalyticsReportWidget" %>
<%@ Register TagPrefix="analytics" TagName="Report" Src="../../Analytics/reporting/Report.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Trend" Src="../../Analytics/reporting/Trend.ascx" %>
<%@ Register TagPrefix="analytics" TagName="Detail" Src="../../Analytics/reporting/Detail.ascx" %>
<%@ Register TagPrefix="EktronWorkarea" TagName="GroupedDropDownList" Src="../../controls/generic/dropdownlist/GroupedDropDownList.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="GoogleAnalyticsReportSubtree" Src="../../controls/NavigationTrees/GoogleAnalyticsReportSubtree.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="SiteCatalystReportSubtree" Src="../../controls/NavigationTrees/SiteCatalystReportSubtree.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="WebTrendsReportSubtree" Src="../../controls/NavigationTrees/WebTrendsReportSubtree.ascx" %>
<%@ Register TagPrefix="analytics" TagName="SiteSelector" Src="../../Analytics/controls/SiteSelector.ascx" %> 
<%@ Register TagPrefix="analytics" TagName="ProviderSelector" Src="../../Analytics/controls/ProviderSelector.ascx" %> 

<style type="text/css">
    .AnalyticsReportWidget_Edit
    {
    	clear: both;
    }
    
    .AnalyticsReportWidget_Edit .ReportSelectGoogle, .AnalyticsReportWidget_Edit .ReportSelectSiteCatalyst, .AnalyticsReportWidget_Edit .ReportSelectWebTrends
    {
    	float: left;
    	margin: 0.5em;
    }
    
    /* .ViewSelect */
    .AnalyticsReportWidget_Edit .ViewSelectContainer
    {
    	float: right;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .MinimalViewSelect
    {
    	display: none;
    }
    
    .AnalyticsReportWidget_Edit .PeriodSelect
    {
    	float: left;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .ProviderSelect
    {
    	float: left;
    	margin: 0.5em;
    }
    
    .AnalyticsReportWidget_Edit .SiteSelectGoogle, .AnalyticsReportWidget_Edit .SiteSelectSiteCatalyst, .AnalyticsReportWidget_Edit .SiteSelectWebTrends
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
    
    div.analyticsReport div.InnerRegion 
    {
    	overflow: auto;
    }
    
</style>

<div class="AnalyticsReportWidget">
<asp:MultiView ID="AnalyticsViews" runat="server">
    <asp:View ID="EditView" runat="server">
        <div id="editContainer" class="AnalyticsReportWidget_Edit" runat="server" >
            <analytics:ProviderSelector ID="ProviderSelect" CssClass="ProviderSelect" AutoPostBack="false" PersistenceId="" runat="server" ChangeCausesPostback="false" />
            <br class="separator" />
                <EktronWorkarea:GroupedDropDownList id="ReportSelectGoogle" runat="server" ChangeCausesPostback="false" ClassName="ReportSelectGoogle" />
                <EktronWorkarea:GroupedDropDownList id="ReportSelectSiteCatalyst" runat="server" ChangeCausesPostback="false" ClassName="ReportSelectSiteCatalyst" />
                <EktronWorkarea:GroupedDropDownList id="ReportSelectWebTrends" runat="server" ChangeCausesPostback="false" ClassName="ReportSelectWebTrends" />
            <span class="ViewSelectContainer" >
            <asp:DropDownList ToolTip="Select the View from the Drop Down Menu" ID="ViewSelect" CssClass="ViewSelect" runat="server" />
                <asp:DropDownList ToolTip="Select the Mininmal View from the Drop Down Menu" ID="MinimalViewSelect" CssClass="MinimalViewSelect" runat="server" />
            </span>
            <br class="separator" />
            <asp:DropDownList ToolTip="Select the Period from the Drop Down Menu" ID="PeriodSelect" CssClass="PeriodSelect" runat="server" />
			    <analytics:SiteSelector ID="SiteSelectGoogle" CssClass="SiteSelectGoogle" AutoPostBack="false" PersistenceId="" runat="server" FromWidget="true" />
			    <analytics:SiteSelector ID="SiteSelectSiteCatalyst" CssClass="SiteSelectSiteCatalyst" AutoPostBack="false" PersistenceId="" runat="server" FromWidget="true" />
			    <analytics:SiteSelector ID="SiteSelectWebTrends" CssClass="SiteSelectWebTrends" AutoPostBack="false" PersistenceId="" runat="server" FromWidget="true" />
            <br class="separator" />
            <div class="buttons">
                <asp:Button ToolTip="Cancel" ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &#160;&#160;
                <asp:Button ToolTip="Save" ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                <asp:hiddenfield ID="hdnSegment" runat="server" />
            </div>
        </div>
    </asp:View>
	<asp:View ID="ReportView" runat="server">
		<analytics:Report ID="AnalyticsReport" LineChartWidth="410px" runat="server" FromWidget="true" />
	</asp:View>
	<asp:View ID="TrendView" runat="server">
		<analytics:Trend ID="AnalyticsTrend" runat="server" FromWidget="true" />
	</asp:View>
</asp:MultiView>
<div class="directory">
    <NavigationTree:GoogleAnalyticsReportSubtree id="GoogleAnalyticsContainer" runat="server" />
    <NavigationTree:SiteCatalystReportSubtree id="SiteCatalystContainer" runat="server" />
    <NavigationTree:WebTrendsReportSubtree id="WebTrendsContainer" runat="server" />
</div>
</div>
