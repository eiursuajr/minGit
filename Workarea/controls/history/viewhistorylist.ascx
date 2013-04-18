<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewHistoryList" CodeFile="ViewHistoryList.ascx.cs" %>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div title="View Content History" class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div title="Back" class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div title="History Status" class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid id="HistoryListGrid" 
        runat="server" 
        Width="100%" 
        AutoGenerateColumns="False" 
        EnableViewState="False"
        CssClass="ektronGrid"
        GridLines="None">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
</div>

