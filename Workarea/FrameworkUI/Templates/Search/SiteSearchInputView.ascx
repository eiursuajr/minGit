<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteSearchInputView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.SiteSearchInputView" %>

<div class="ektron-ui-control ektron-ui-search ektron-ui-search-site" id="<%# this.Parent.ClientID %>">
    <div class="basicSearch">
        <ektronUI:TextField ID="uxSearchText" runat="server" Text='<%# Eval("QueryText") %>'/>
        <ektronUI:Button ID="uxBasicSearchButton" runat="server" DisplayMode="Button" Text="<%$ Resources:SearchButtonText %>" OnClick="uxBasicSearch_Click" />
        <div class="toggleAdvancedSearchWrapper">
            <a ID="aspAdvancedSearchLink" runat="server" class="toggleAdvancedSearch" onclick="return false;" />
            <a ID="aspAdvancedSearchIcon" runat="server" class="toggleAdvancedSearchIcon toggleAdvancedSearch" onclick="return false;" >
                <span class="ui-icon ui-icon-triangle-1-s"></span>
            </a>
        </div>
    </div>
    <div class="advancedSearch ektron-ui-hidden">
        <fieldset class="advancedSearchFieldset">
            <legend>
                <asp:Literal ID="aspLegendText" runat="server" Text="<%$ Resources:AdvancedSearchLegend %>" />
            </legend>
            <ul class="ektron-ui-listStyleNone">
                <li>
                    <asp:Label ID="aspWithAllWordsLabel" runat="server" AssociatedControlID="uxWithAllWords" Text="<%$ Resources:FilterWithWords %>" meta:resourcekey="uxWithAllWordsLabelResource1" />
                    <ektronUI:TextField ID="uxWithAllWords" runat="server" Text='<%# Eval("WithAllWords") %>' meta:resourcekey="uxWithAllWordsResource1" />
                </li>
                <li>
                    <asp:Label ID="aspWithoutWordsLabel" runat="server" AssociatedControlID="uxWithoutWords" Text="<%$ Resources:FilterWithoutWords %>" meta:resourcekey="uxWithoutWordsLabelResource1" />
                    <ektronUI:TextField ID="uxWithoutWords" runat="server" Text='<%# Eval("WithoutWords") %>' meta:resourcekey="uxWithoutWordsResource1" />
                </li>
                <li>
                    <asp:Label ID="aspExactPhraseLabel" runat="server" AssociatedControlID="uxExactPhrase" Text="<%$ Resources:FilterExactPhrase %>" meta:resourcekey="uxExactPhraseLabelResource1" />
                    <ektronUI:TextField ID="uxExactPhrase" runat="server" Text='<%# Eval("ExactPhrase") %>' meta:resourcekey="uxExactPhraseResource1" />
                </li>
                <li>
                    <asp:Label ID="aspAnyWordsLabel" runat="server" AssociatedControlID="uxAnyWords" Text="<%$ Resources:FilterAnyWord %>" meta:resourcekey="uxAnyWordsLabelResource1" />
                    <ektronUI:TextField ID="uxAnyWords" runat="server" Text='<%# Eval("WithAnyWord") %>' meta:resourcekey="uxAnyWordsResource1" />
                </li>
            </ul>
        </fieldset>
        
        <ektronUI:Button ID="uxSearchButton" runat="server" DisplayMode="Button" Text="<%$ Resources:SearchButtonText %>" OnClick="uxAdvancedSearch_Click" />
    </div>
    
    <ektronUI:JavaScriptBlock ID="uxScriptBlockSearch" runat="server" ExecutionMode="OnEktronReady" meta:resourcekey="uxScriptBlockSearchResource1">
        <ScriptTemplate>
            setTimeout("Ektron.Controls.Search.SiteSearch.init({ clientId: '<%# this.Parent.ClientID%>' })", 0);
        </ScriptTemplate>
    </ektronUI:JavaScriptBlock>
</div>