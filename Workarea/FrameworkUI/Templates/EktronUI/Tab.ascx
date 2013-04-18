<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Tab.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.Tab" %>
<h3 id="aspAccordionHeader" runat="server"><a href="#<%# this.TabClientID %>" title="<%# this.TabText %>" <%# this.TabOnClick %>><%# this.TabText%></a></h3>
<div>
    <asp:PlaceHolder ID="aspContents" runat="server"></asp:PlaceHolder>
</div>
