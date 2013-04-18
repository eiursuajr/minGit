<%@ Control Language="C#" AutoEventWireup="true" CodeFile="XmlSearchResultsView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearchResultsView" %>
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-results ektron-ui-search-results-xml" id="<%# this.Parent.ClientID %>">
    <div class="section no-results">
        <asp:Label ID="aspNoResults" runat="server" Visible='<%# (Ektron.Cms.Framework.UI.SearchState)Eval("State") == Ektron.Cms.Framework.UI.SearchState.NoResults %>'>
                <asp:Literal ID="aspNoResultsText" runat="server" Text="<%$ Resources:NoResultsFound %>" />
        </asp:Label>
    </div>
    <asp:ListView ID="aspResults" runat="server" DataSource='<%# Eval("Results") %>' ItemPlaceholderID="aspPlaceholder">
        <LayoutTemplate>
            <ul class="results">
                <asp:PlaceHolder ID="aspPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="result ektron-ui-clearfix">
                <h3 class="title"><a href="<%# Eval("Url") %>"><%# Eval("Title") %></a></h3>
                <div class="summary"><%# Eval("Summary") %></div>
                <span class="url ektron-ui-quiet"><%# Eval("Url") %></span>
                <span class="date ektron-ui-quiet"><%# Eval("Date") %></span>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>