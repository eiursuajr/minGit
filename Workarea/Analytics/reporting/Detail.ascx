<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Detail.ascx.cs" Inherits="Analytics_reporting_Detail" %>
<%@ Register TagPrefix="analytics" TagName="summary" Src="Summary.ascx" %>
<%@ Register TagPrefix="analytics" TagName="MetricSelector" Src="MetricSelector.ascx" %>
<%@ Register TagPrefix="ektron" TagName="TimeLineChart" Src="../../controls/reports/TimeLineChart.ascx" %>

<asp:Label ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
    
<asp:Panel ID="pnlData" runat="server">  
    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
    </asp:Panel>
    
	<%--<div id="TrafficDateRange"><asp:label ID="lblTrafficDateRange" runat="server" EnableViewState="false" /></div>--%>
	<div class="dataTable">
	
		<analytics:MetricSelector id="MetricSelector" OnSelectionChanged="MetricSelector_SelectionChanged" runat="server" />
		
		<asp:UpdatePanel ID="pnlChart" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="MetricSelector" EventName="SelectionChanged" />
			</Triggers>
			<ContentTemplate>
				<ektron:TimeLineChart id="AnalyticsLineChart" CssClass="AnalyticsDetailLineChart" width="820px" height="125px" runat="server" />
			</ContentTemplate>
		</asp:UpdatePanel>
		
		<asp:PlaceHolder ID="VisitPageArea" runat="server" Visible="false">
			<p class="pbox">
				<asp:Literal ID="ltr_viewed" runat="server" />
				<br />
				<asp:Image ID="img_visitpage" runat="server" Visible="false" EnableViewState="false" />&#160;
				<asp:HyperLink ID="hyp_visitpage" runat="server" Target="_blank" EnableViewState="false"><asp:Literal ID="lblVisitThisPage" runat="server">Visit this page</asp:Literal></asp:HyperLink>&#160;
				<%--|&#160;<asp:Label ID="lblAnalyzing" runat="server" />&#160;
				<asp:DropDownList ID="drp_analyze" runat="server" Enabled="false">
					<asp:ListItem>Content Detail</asp:ListItem>
				</asp:DropDownList>--%>
			</p>
		</asp:PlaceHolder>
	</div>
    
	<analytics:summary id="AnalyticsSummary" runat="server" />

</asp:Panel>