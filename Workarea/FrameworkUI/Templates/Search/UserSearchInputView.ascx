<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserSearchInputView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.UserSearchInputView" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI"  %>

<div class="ektron-ui-control ektron-ui-search ektron-ui-search-users" id="<%# this.Parent.ClientID %>">
    <div class="basicSearch">
        <ektronUI:TextField ID="uxSearchText" runat="server" Text='<%# Eval("QueryText") %>' />
        <ektronUI:Button ID="uxBasicSearchButton" runat="server" DisplayMode="Button" Text="<%$ Resources:SearchButtonText %>" OnClick="uxBasicSearch_Click" />
        <div class="toggleDirectoryWrapper">
            <a ID="aspDirectorySearchLink" runat="server" class="toggleDirectorySearch" onclick="return false;" />
            <a ID="aspDirectorySearchIcon" runat="server" class="toggleDirectorySearchIcon toggleDirectorySearch" onclick="return false;" >
                <span class="ui-icon ui-icon-triangle-1-s"></span>
            </a>
        </div>
    </div>
    <div id="uxDirectorySearch" runat="server" class="directorySearch ektron-ui-hidden" >
        <asp:RadioButtonList ID="aspDirectorySearchFilters" runat="server" DataTextField="Label" DataValueField="Name" RepeatDirection="Horizontal" />
        <div class="ektron-ui-text-small">
            <ektronUI:ButtonSet ID="uxDirectoryButtons" runat="server"></ektronUI:ButtonSet>
        </div>
    </div>

    <ektronUI:JavaScriptBlock ID="uxScriptBlockUserSearch" runat="server" ExecutionMode="OnEktronReady">
        <ScriptTemplate>
            setTimeout("Ektron.Controls.Search.UserSearch.init({ clientId: '<%# this.Parent.ClientID %>' })", 0);
        </ScriptTemplate>
    </ektronUI:JavaScriptBlock>
</div>