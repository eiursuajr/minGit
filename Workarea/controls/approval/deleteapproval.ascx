<%@ Control Language="C#" AutoEventWireup="true" Inherits="deleteapproval" CodeFile="deleteapproval.ascx.cs" %>

<div id="dhtmltooltip"></div>			
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid id="DeleteApprovalGrid" 
        runat="server" 
        AutoGenerateColumns="False"
        Width="100%" 
        EnableViewState="False"
        CssClass="ektronGrid"
        GridLines="None">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
</div>
