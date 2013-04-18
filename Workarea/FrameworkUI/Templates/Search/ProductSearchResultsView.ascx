<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductSearchResultsView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.ProductSearchResultsView" %>

<ektronUI:Css ID="uxProductSearchResults" runat="server" BrowserTarget="All" Path="{UIPath}/css/Ektron/Controls/ektron-ui-search-results.css" Aggregate="True" Media="" />
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-results ektron-ui-search-results-products" id="<%# this.Parent.ClientID %>">
    <asp:Label ID="aspNoResults" runat="server" Text='<%$ Resources:NoResults %>' Visible='<%# (Ektron.Cms.Framework.UI.SearchState)Eval("State") == Ektron.Cms.Framework.UI.SearchState.NoResults %>'></asp:Label>
    
    <asp:ListView ID="aspResults" DataSource='<%# Eval("Results") %>' ItemPlaceholderID="aspPlaceholder" runat="server">
        <LayoutTemplate>
            <ul class="results">
                <asp:PlaceHolder ID="aspPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="result ektron-ui-clearfix">
                <div class="avatar">
                    <a href="<%# Eval("Url") %>"><asp:Image ID="aspAvatar" runat="server" AlternateText='<%# Eval("Title") %>' ToolTip='<%# Eval("Title") %>' ImageUrl='<%# Eval("ImageUrl") %>' Visible='<%# !String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "ImageUrl").ToString()) %>' /></a>
                </div>
                <div class="resultsInfo">
                    <h3 class="title"><a href="<%# Eval("Url") %>"><%# Eval("Title") %></a></h3>
                    <div class="summary"><%# Eval("Summary") %></div>
                    <ul class="ektron-ui-listStyleNone">
                        <li class="sku ektron-ui-quiet">
                            <asp:Literal ID="aspLegendText" runat="server" Text="<%$ Resources:SKU %>" />: <%# Eval("SKU") %>
                        </li>
                        <li class="catalogNumber ektron-ui-quiet">
                            <asp:Literal ID="aspCatalogNumber" runat="server" Text="<%$ Resources:CatalogNumber %>" />: <%# Eval("CatalogNumber") %>
                        </li>
                    </ul>
                </div>
                <ul class="prices">
                    <li><asp:label ID="aspListPrice" CssClass="listPriceLabel" Text="<%$ Resources:ListPrice %>" runat="server" />: <span class="listPrice"><%# Eval("ListPrice")%></span></li>
                    <li><asp:label ID="aspSalePrice" CssClass="salePriceLabel" Text="<%$ Resources:SalePrice %>" runat="server" />: <span class="salePrice"><%# Eval("SalePrice")%></span></li>
                </ul>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>