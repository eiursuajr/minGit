<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserSearchResultsView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.UserSearchResults" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-results ektron-ui-search-results-users" id="<%# this.Parent.ClientID %>">
    <asp:ListView ID="aspResults" runat="server" DataSource='<%# Eval("Results") %>' ItemPlaceholderID="aspPlaceholder">
        <LayoutTemplate>
            <ul class="results">
                <asp:PlaceHolder ID="aspPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="result ektron-ui-clearfix">
                <div class="avatar">
                    <a href='<%# Eval("ProfileUrl") %>'><asp:Image ID="aspImage1" runat="server" AlternateText='<%# Eval("DisplayName") %>' ToolTip='<%# Eval("DisplayName") %>' ImageUrl='<%# Eval("Avatar") %>' Visible='<%# !String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "Avatar").ToString()) %>' /></a>
                </div>
                <h3 class="title"><a href='<%# Eval("ProfileUrl") %>'><span class="firstName"><%# Eval("FirstName") %></span> <span class="lastName"><%# Eval("LastName") %></span></a></h3>                   
                <span class="email"><a href='mailto:<%# Eval("Email") %>'><%# Eval("Email") %></a></span>
                <asp:ListView ID="aspTags" ItemPlaceholderID="aspItemPlaceholder" runat="server" DataSource='<%# Eval("Tags") %>'>
                    <LayoutTemplate>
                        <ul class="tags">
                            <li>
                                <asp:PlaceHolder ID="aspItemPlaceholder" runat="server" />
                            </li>
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="aspTagLink" runat="server" OnCommand="uxTagLink_Command" CommandArgument="<%# Container.DataItem %>"><%# Container.DataItem %></asp:LinkButton>
                    </ItemTemplate>    
                    <ItemSeparatorTemplate>, 
                        </li>
                        <li>
                    </ItemSeparatorTemplate>  
                </asp:ListView>
            </li>
        </ItemTemplate>
    </asp:ListView>
    <asp:Label ID="aspNoResults" runat="server" Visible='<%# (SearchState)Enum.Parse(typeof(SearchState), Eval("State").ToString()) == SearchState.NoResults %>' Text="<%$ Resources:NoResultsFound %>" />
</div>