<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Message.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.Message" %>
<div id="uxMessage" runat="server">
    <asp:Label ID="aspIcon" runat="server"></asp:Label>
    <div class="ektron-ui-clearfix ektron-ui-messageBody">
        <asp:PlaceHolder ID="aspContentTemplateControl" runat="server" />
    </div>
</div>
