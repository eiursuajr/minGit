<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductSearchInputView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.ProductSearchInputView" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI"   %>
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-products" id="<%# this.Parent.ClientID %>">
    <div class="basicSearch">
        <ektronUI:TextField ID="uxSearchText" runat="server" Text='<%# Eval("QueryText") %>'/>
        <ektronUI:Button ID="uxBasicSearchButton" runat="server" DisplayMode="Button" Text="<%$ Resources:SearchButtonText %>" OnClick="uxSearch_Click" />
        <div class="toggleAdvancedSearchWrapper">
            <asp:HyperLink ID="aspAdvancedSearchLink" runat="server" CssClass="toggleAdvancedSearch" Text="<%$ Resources:ToggleButtonText %>" />
            <asp:HyperLink ID="aspAdvancedSearchIcon" runat="server" CssClass="toggleAdvancedSearchIcon toggleAdvancedSearch">
                <span class="ui-icon ui-icon-triangle-1-s"></span>
            </asp:HyperLink>
        </div>
    </div>
    <div class="advancedSearch ektron-ui-hidden">
        <fieldset class="advancedSearchFieldset">
            <legend>
                <asp:Literal ID="aspLegendText" runat="server" Text="<%$ Resources:AdvancedSearchLegend %>" />
            </legend>
            <ul class="ektron-ui-listStyleNone">
                <li>
                    <asp:Label ID="aspWithAllWordsLabel" runat="server" AssociatedControlID="uxWithAllWords" Text="<%$ Resources:FilterWithWords %>" />
                    <ektronUI:TextField ID="uxWithAllWords" runat="server" Text='<%# Eval("WithAllWords") %>' />
                </li>
                <li>
                    <asp:Label ID="aspWithoutWordsLabel" runat="server" AssociatedControlID="uxWithoutWords" Text="<%$ Resources:FilterWithoutWords %>" />
                    <ektronUI:TextField ID="uxWithoutWords" runat="server" Text='<%# Eval("WithoutWords") %>' />
                </li>
                <li>
                    <asp:Label ID="aspExactPhraseLabel" runat="server" AssociatedControlID="uxExactPhrase" Text="<%$ Resources:FilterExactPhrase %>" />
                    <ektronUI:TextField ID="uxExactPhrase" runat="server" Text='<%# Eval("ExactPhrase") %>' />
                </li>
                <li>
                    <asp:Label ID="aspAnyWordsLabel" runat="server" AssociatedControlID="uxAnyWords" Text="<%$ Resources:FilterAnyWord %>" />
                    <ektronUI:TextField ID="uxAnyWords" runat="server" Text='<%# Eval("WithAnyWord") %>' />
                </li>
            </ul>
        </fieldset>
        
        <ektronUI:Button ID="uxSearchButton" runat="server" DisplayMode="Button" Text="<%$ Resources:SearchButtonText %>" OnClick="uxAdvancedSearch_Click" />
    </div>
    
    <ektronUI:JavaScriptBlock ID="uxScriptBlockSearch" runat="server" ExecutionMode="OnEktronReady">
        <ScriptTemplate>
            setTimeout("Ektron.Controls.Search.SiteSearch.init({ clientId: '<%= this.Parent.ClientID%>' })", 0);
        </ScriptTemplate>
    </ektronUI:JavaScriptBlock>
</div>