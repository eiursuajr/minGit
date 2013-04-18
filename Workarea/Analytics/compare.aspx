<%@ Page Language="C#" AutoEventWireup="true" CodeFile="compare.aspx.cs" Inherits="Analytics_compare" %>
<%@ Register TagPrefix="analytics" TagName="summary" Src="reporting/Summary.ascx" %>
<%@ Register TagPrefix="analytics" TagName="MetricSelector" Src="../Analytics/reporting/MetricSelector.ascx" %>
<%@ Register TagPrefix="ektron" TagName="TimeLineChart" Src="../controls/reports/TimeLineChart.ascx" %>
<%@ Register TagPrefix="analytics" TagName="UrlFilterControl" Src="controls/UrlFilterControl.ascx" %>
<%@ Register TagPrefix="ektron" TagName="DateRangePicker" Src="../controls/generic/date/DateRangePicker.ascx" %>
<%@ Register TagPrefix="analytics" TagName="SiteSelector" Src="controls/SiteSelector.ascx" %> 
<%@ Register TagPrefix="analytics" TagName="ProviderSelector" Src="controls/ProviderSelector.ascx" %> 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        td.comparisonChart {padding-bottom: 1em !important; width: 100%;}
        td.summaryTitle {font-weight: bold; background-color: white !important; width: 50%;}
        td.summaryTitle1 {color: #0077CC;}
        td.summaryTitle2 {color: #FF9900;}
        td.summaryCharts {width: 50%; background-color: white !important;}
        .errGAMsg { background-color: #FBE3E4; border: 1px solid #FBC2C4; color: #D12F19; display: block; margin: 0.25em; padding: 5px;}
        .AnalyticsContentBlockCompare{position: relative; top: 0; left: 0;}
        .AnalyticsContentBlockCompare .UpdateProgressContainer { position: absolute; top: 6em; left: 12em; border: solid 1px #D7E0E7; margin: 1em; padding: 2em; background-color: #E7F0F7;	z-index:1; }
        .selectorWrapper {background: #e7f0f7; padding: .5em; margin-bottom: .5em; text-align: center;}
        .toolbarRight {text-align: left; float: right; display: inline-block;}
        .toolbarMiddle {text-align: left; display: inline-block; margin: auto;}
        .toolbarMiddle span {display: inline-block;}
        .toolbarLeft {text-align: left; float: left; display: inline-block;}
    </style>
    
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {
            // hook ASP.NET Ajax begin and end events, to show busy signal when update panel makes a callback:
            if ("undefined" != typeof Sys.WebForms.PageRequestManager.getInstance) {
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginCallbackHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndCallbackHandler);
            }
        });
        
        function BeginCallbackHandler() {
            $(".UpdateProgressContainer").show();
        }

        function EndCallbackHandler() {
            $(".UpdateProgressContainer").hide();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
		<asp:ScriptManager runat="server" EnablePartialRendering="true" ID="ScriptManager1" />
        <div class="ektronPageContainer ektronPageTabbed AnalyticsContentBlockCompare">
			<div class="UpdateProgressContainer" style="display: none;">
			        <h2><asp:Literal ID="litLoadingMessage" runat="server" /></h2>
			</div>
            <div id="SelectorFilterRow" class="selectorWrapper" runat="server">
                
			    <asp:UpdatePanel ID="pnlMetric" class="toolbarLeft" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="ProviderSelect" EventName="OnProviderChanged" />
					</Triggers>
					<ContentTemplate>
					    <span><analytics:MetricSelector id="MetricSelector" runat="server" /></span>
					</ContentTemplate>
				</asp:UpdatePanel>	
                <span class="toolbarRight">
				    <analytics:UrlFilterControl ID="UrlFilter" IsLocalhostValid="false" runat="server" />
				</span> 						
				<span class="toolbarMiddle">
				    <analytics:SiteSelector ID="SiteSelect" runat="server" AutoPostBack="false" />
				    <analytics:ProviderSelector ID="ProviderSelect" runat="server" AutoPostBack="true" />
				    <asp:ImageButton ID="btnRefresh" CssClass="RefreshButton" runat="server" />
				</span>
                
            </div>
            <table id="Table1" runat="server" class="ektronGrid">
				<tbody>
					<tr>
						<td colspan="2" class="comparisonChart">
							<asp:UpdatePanel ID="pnlChart" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
								<Triggers>
									<asp:AsyncPostBackTrigger ControlID="MetricSelector" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="UrlFilter" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="DateRangePicker1" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="DateRangePicker2" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="SiteSelect" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="ProviderSelect" EventName="OnProviderChanged" />
									<asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
								</Triggers>
								<ContentTemplate>
								    <div id="errGAMsg" runat="server" class="errGAMsg" visible="false" EnableViewState="false"><asp:Literal ID="ltr_error" EnableViewState="false" runat="server" /></div>                
									<ektron:TimeLineChart id="ComparisonTimeLineChart" width="840px" height="400px" runat="server" />
								</ContentTemplate>
							</asp:UpdatePanel>
						</td>
					</tr>
					<tr id="CaptionRow" runat="server">
						<td class="summaryTitle summaryTitle1">
	                        <asp:Label id="Caption1" runat="server" />    
						</td>
						<td class="summaryTitle summaryTitle2">
	                        <asp:Label id="Caption2" runat="server" />
						</td>
					</tr>
					<tr id="SummaryRow" runat="server">
						<td class="summaryTitle summaryTitle1">
	                        
							<ektron:DateRangePicker id="DateRangePicker1" runat="server" />
	                        
						</td>
						<td class="summaryTitle summaryTitle2">
	                    
							<ektron:DateRangePicker id="DateRangePicker2" runat="server" />
	                        
						</td>
					</tr>
					<tr>
						<td class="summaryCharts summaryCharts1">
	                        
							<asp:UpdatePanel ID="UpdatePanel1" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
								<Triggers>
									<asp:AsyncPostBackTrigger ControlID="UrlFilter" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="DateRangePicker1" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="SiteSelect" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="ProviderSelect" EventName="OnProviderChanged" />
									<asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
								</Triggers>
								<ContentTemplate>
									<analytics:summary id="SummaryCharts1" runat="server" />
								</ContentTemplate>
							</asp:UpdatePanel>
	                        
						</td>
						<td class="summaryCharts summaryCharts2">
	                        
							<asp:UpdatePanel ID="UpdatePanel2" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
								<Triggers>
									<asp:AsyncPostBackTrigger ControlID="UrlFilter" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="DateRangePicker2" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="SiteSelect" EventName="SelectionChanged" />
									<asp:AsyncPostBackTrigger ControlID="ProviderSelect" EventName="OnProviderChanged" />
									<asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
								</Triggers>
								<ContentTemplate>
									<analytics:summary id="SummaryCharts2" runat="server" />
								</ContentTemplate>
							</asp:UpdatePanel>
	                        
						</td>
					</tr>
                </tbody>
            </table>

        </div>
    
    </form>
</body>
</html>
