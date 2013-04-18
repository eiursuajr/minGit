<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewMessages.ascx.cs" Inherits="Messaging_ViewMessages" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="_dg"
        AutoGenerateColumns="false"
        CssClass="ektronGrid"
        runat="server">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
</div>
<input type="hidden" id="MsgInboxSelCBHdn" name="MsgInboxSelCBHdn" value="" />

