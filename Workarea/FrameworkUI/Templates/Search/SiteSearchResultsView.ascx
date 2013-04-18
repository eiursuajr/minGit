<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteSearchResultsView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.SiteSearchResultsView" %>
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-results ektron-ui-search-results-site" id="<%# this.Parent.ClientID %>">
    <asp:ListView ID="aspSuggestedSpellings" runat="server" DataSource='<%# Eval("SuggestedSpellings") %>' ItemPlaceholderID="aspPlaceholder">
        <LayoutTemplate>
            <div class="section suggested-spelling">
                <p><asp:Literal ID="aspDidYouMean" runat="Server" Text="<%$ Resources:DidYouMean %>"></asp:Literal>&#160;</p>
                <ul>
                    <asp:PlaceHolder ID="aspPlaceholder" runat="server"></asp:PlaceHolder>
                </ul>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="suggested-spelling ektron-ui-clearfix">
                <asp:LinkButton ID="aspSuggestedSpellingsSearch" runat="server" Text='<%# Container.DataItem %>' OnCommand="aspSuggestedSpellingsSearch_Click" CommandArgument=' <%# Container.DataItem %>' ToolTip=' <%# Container.DataItem %>' />
            </li>
        </ItemTemplate>
    </asp:ListView>
    <div class="section suggested-results">
        <asp:ListView ID="aspSuggestedResults" runat="server" DataSource='<%# Eval("SuggestedResults") %>' ItemPlaceholderID="aspPlaceholder">
            <LayoutTemplate>
                <ul>
                    <asp:PlaceHolder ID="aspPlaceholder" runat="server"></asp:PlaceHolder>
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li class="suggested-result ektron-ui-clearfix">
                    <h3 class="title"><a href="<%# Eval("Url") %>"><%# Eval("Title") %></a></h3>
                    <div class="summary"><%# Eval("Summary") %></div>
                    <span class="url ektron-ui-quiet"><%# Eval("Url") %></span>
                </li>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <div class="section no-results">
        <asp:Label ID="aspNoResults" runat="server" 
            Visible='<%# (Ektron.Cms.Framework.UI.SearchState)Eval("State") == Ektron.Cms.Framework.UI.SearchState.NoResults %>' 
            meta:resourcekey="aspNoResultsResource"></asp:Label>
    </div>
    <div class="section results">
        <asp:ListView ID="aspResults" runat="server" DataSource='<%# Eval("Results") %>' ItemPlaceholderID="aspPlaceholder">
            <LayoutTemplate>
                <ul>
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
</div>