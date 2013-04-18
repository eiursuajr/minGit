<%@ Control Language="c#" AutoEventWireUp="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="Help.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.Help" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>

<div class="Ektron_DialogTabstrip_Container">
	<telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainTable">
		<telerik:tab elementid="HelpDialogTable" selected="True" text="<script>localization.showText('Tab1HeaderText');</script>" />
	</telerik:tabcontrol>
</div>

<div class="Ektron_Dialog_Body_Container Ektron_TopSpaceVeryVerySmall">
    <div id="HelpDialogTable" style="width:542px;height:283px;overflow:auto;">
	    <asp:placeholder id="LocalizedHelp" runat="server"/><br/>
    </div>
</div>

<div class="Ektron_Dialogs_ButtonContainer Ektron_TopSpaceSmall">
    <button id="btnClose" title="Close" onclick="javascript:CloseDlg();" class="Ektron_StandardButton"><script>localization.showText('Close');</script></button>
</div>    