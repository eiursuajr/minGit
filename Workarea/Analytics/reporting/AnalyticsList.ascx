<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AnalyticsList.ascx.cs" Inherits="Analytics_reporting_AnalyticsList" %>

<asp:Label ID="lblNoRecords" Visible="false" runat="server" ToolTip="No Records"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
    
<asp:Panel ID="pnlData" runat="server">  
    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
    </asp:Panel>
    <!-- control to generate the list of LIs -->
    <ol id="resultList" visible="false" runat="server"></ol>
</asp:Panel>