<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProviderSelector.ascx.cs" Inherits="Analytics_controls_ProviderSelector" %>
<span class="ProviderSelectorContainer" ID="ProviderSelectorContainer" runat="server">
    <asp:Label ID="lblProviderSelector" runat="server" EnableViewState="false" />
    <asp:DropDownList ToolTip="Select Analytics Provider from the Drop Down Menu" ID="ProviderSelectorList" CssClass="ProviderSelectorList" runat="server" OnSelectedIndexChanged="ProviderSelectorList_SelectionChanged" />
</span>
