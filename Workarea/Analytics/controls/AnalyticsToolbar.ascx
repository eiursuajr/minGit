<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AnalyticsToolbar.ascx.cs" Inherits="Analytics_controls_Toolbar" %>
<%@ Register TagPrefix="ektron" TagName="DateRangePicker" Src="../../controls/generic/date/DateRangePicker.ascx" %> 
<%@ Register TagPrefix="analytics" TagName="SiteSelector" Src="SiteSelector.ascx" %> 
<div class="ektronPageGrid analyticsReport">
    <div class="ektronToolbar">
        <ektron:DateRangePicker id="DateRangePicker1" OnSelectionChanged="OnSelectionChanged" runat="server" />
		<analytics:SiteSelector ID="SiteSelector1" OnSelectionChanged="OnSelectionChanged" runat="server" />
		<asp:Literal runat="server" ID="litHelp" />
    </div>
</div>