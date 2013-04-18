<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ButtonSet.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.ButtonSet" %>
<ektronUI:JavaScriptBlock ID="uxInitializationScript" runat="server" ExecutionMode="OnEktronReady">
    <ScriptTemplate>
        Ektron.Controls.EktronUI.ButtonSet.init('#<%# this.ControlContainer.ClientID %>');
    </ScriptTemplate>
</ektronUI:JavaScriptBlock>