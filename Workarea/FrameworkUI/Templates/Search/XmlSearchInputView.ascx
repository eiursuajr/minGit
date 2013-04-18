<%@ Control Language="C#" AutoEventWireup="true" CodeFile="XmlSearchInputView.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearchInputView" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<div class="ektron-ui-control ektron-ui-search ektron-ui-search-xml" id="<%# this.Parent.ClientID %>">
    <div class="xml-search-fields">
        <asp:ListView ID="aspFields" runat="server" DataSource='<%# Eval("Fields") %>' OnItemDataBound="aspFields_ItemDataBound" ItemPlaceholderID="aspItemPlaceholder">
            <LayoutTemplate>
                <table id="fields" class="fields-list">
                    <tbody>
                        <asp:PlaceHolder ID="aspItemPlaceholder" runat="server"></asp:PlaceHolder>
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <ektron:XmlSearchFieldView ID="uxField" runat="server" Field='<%# Container.DataItem %>'></ektron:XmlSearchFieldView>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <asp:Label ID="aspNoFields" Visible='<%# null == (List<XmlField>)Eval("Fields") || 0 == ((List<XmlField>)Eval("Fields")).Count %>' Text="<%$ Resources:NoSmartFormFound %>" runat="server" ></asp:Label>
    <ektronUI:Button ID="uxXmlSearchButton" runat="server" DisplayMode="Button" Visible='<%# (List<XmlField>)Eval("Fields") != null  && ((List<XmlField>)Eval("Fields")).Count > 0 %>' CausesValidation="true" ValidationGroup="Test" Text="<%$ Resources:Search %>" OnClick="uxXmlSearch_Click" />
    <ektronUI:JavaScriptBlock ID="uxScriptBlockSearch" runat="server" ExecutionMode="OnEktronReady">
        <ScriptTemplate>
            setTimeout("Ektron.Controls.Search.XmlSearch.init({ clientId: '<%# this.Parent.ClientID%>' })", 0);
        </ScriptTemplate>
    </ektronUI:JavaScriptBlock>
</div>
    