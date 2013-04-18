<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Tabs.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.Tabs" %>
<asp:ListView ID="aspTabs" runat="server" OnItemDataBound="uxTabs_OnItemDataBound" ItemPlaceholderID="aspItemPlaceholder">
    <LayoutTemplate>
        <ul>
            <asp:PlaceHolder ID="aspItemPlaceholder" runat="server" />
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="aspDataBindHelper" runat="server" Visible="false">
            <li>
                <a href="#<%# this.TabClientID %>" title="<%# this.TabText %>" <%# this.TabOnClick %>><%# this.TabText%></a>
            </li>
        </asp:PlaceHolder>
    </ItemTemplate>
</asp:ListView>
<asp:PlaceHolder ID="aspContents" runat="server"></asp:PlaceHolder>
