<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.DateField" %>
<asp:Label ID="uxDateFieldWrapper" runat="server">
    <asp:Label ID="aspInputLabel" runat="server" AssociatedControlID="aspInput" />
    <asp:TextBox ID="aspInput" runat="server" />
</asp:Label>
<ektronUI:JavaScriptBlock ID="uxJavaScriptBlock" ExecutionMode="OnEktronReady" runat="server">
    <ScriptTemplate>
        var element = $ektron("#<%=aspInput.ClientID %> ");
        element.data({"ektron-global-culture" : "<%= this.ControlContainer.OverrideDefaultCulture %>"});
        element.numeric({allow: "<%= this.ControlContainer.OverrideDefaultCulture.DateTimeFormat.DateSeparator %>"});
        $ektron("#<%= aspInputLabel.ClientID %>").addClass("inFieldLabel").inFieldLabels();
    </ScriptTemplate>
</ektronUI:JavaScriptBlock>