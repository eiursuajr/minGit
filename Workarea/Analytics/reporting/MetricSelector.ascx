<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetricSelector.ascx.cs" Inherits="Analytics_reporting_MetricSelector" %>
<style type="text/css">
span.metric
{
	margin-left: 0em;
	margin-right: 1em;
	margin-bottom: 2px;
}
img.metric
{
	margin-right: 3px;
	margin-bottom: 2px;
}
select.metric
{
	margin-right: 1em;
	margin-bottom: 2px;
}
</style>
<asp:UpdatePanel ID="pnlChart" UpdateMode="Conditional" runat="server">
	<ContentTemplate>
<asp:Label ToolTip="Graph Display" ID="lblDisplay" runat="server" CssClass="metric" Text="Display" EnableViewState="false"></asp:Label>
<asp:Image ToolTip="Blue Graph Line" ID="Image1" runat="server" CssClass="metric" ImageUrl="css/metricBlue.gif" EnableViewState="false" />
<asp:DropDownList ToolTip="Select Data to view as Blue Graph Line from the Drop Down Menu" ID="Selector1" runat="server" CssClass="metric" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged"></asp:DropDownList>
<asp:Image ToolTip="Orange Graph Line" ID="Image2" runat="server" CssClass="metric" ImageUrl="css/metricOrange.gif" EnableViewState="false" />
<asp:DropDownList ToolTip="Select Data to view as Orange Graph Line from the Drop Down Menu" ID="Selector2" runat="server" CssClass="metric" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged"></asp:DropDownList>
	</ContentTemplate>
</asp:UpdatePanel>




