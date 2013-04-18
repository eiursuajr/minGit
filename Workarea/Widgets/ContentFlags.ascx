<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentFlags.ascx.cs" Inherits="Workarea_Widgets_ContentFlags" %>

<asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<asp:Panel ID="pnlData" runat="server">
    <asp:LinkButton ToolTip="View All" id="lnkViewAll" runat="server"><asp:Literal id="ltrlViewAll" runat="server" /></asp:LinkButton>

    <div class="ektronTopSpace"></div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" 
            runat="server" 
            Width="100%" 
            AutoGenerateColumns="false"
            EnableViewState="False"
            GridLines="None"
            CssClass="ektronGrid ektronBorder">
            <HeaderStyle CssClass="title-header" />
        </asp:DataGrid>
    </div>
</asp:Panel>