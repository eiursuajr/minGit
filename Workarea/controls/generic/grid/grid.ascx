<%@ Control Language="C#" AutoEventWireup="true" CodeFile="grid.ascx.cs" Inherits="Ektron.Cms.Common.Grid" %>
<%@ Register TagPrefix="uxGrid" TagName="Paging" Src="../../paging/paging.ascx" %>
<asp:scriptmanager id="ScriptManager" runat="server"/>
<asp:UpdatePanel ID="DataGridViewUpdatePanel" runat="server">
  <ContentTemplate>
    <asp:GridView ID="DataGridView" runat="server"
        Width="100%"
        AutoGenerateColumns="False" 
        onrowdatabound="GridView_RowDataBound" 
        style="width: 100%; border-collapse: collapse; display: table;"
        >
    </asp:GridView>
    <uxGrid:Paging ID="uxGridPaging" runat="server" />
  </ContentTemplate>
</asp:UpdatePanel>
