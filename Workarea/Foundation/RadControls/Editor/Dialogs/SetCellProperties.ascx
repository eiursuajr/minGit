<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.SetCellProperties" AutoEventWireUp="false" CodeBehind="SetCellProperties.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="CellPropertiesControl" Src="../Controls/CellPropertiesControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>

<div id="MainContainer">
	    <div class="Ektron_DialogTabstrip_Container">
		<telerik:tabcontrol id="TabHolder" runat="server" resizecontrolid="MainContainer">
			<telerik:tab image="Dialogs/TabIcons/CellPropertiesTab1.gif" text="<script>localization.showText('Tab1HeaderText');</script>" selected="True" elementid="mainTable"/>
		</telerik:tabcontrol>
	</div> 
	<div class="Ektron_Dialog_Tabs_BodyContainer">
		<telerik:CellPropertiesControl
			id="theCellPropertiesControl"
			runat="server" />
	</div> 
	<div class="Ektron_DialogButtonContainer">
		<button class="Button" title="Ok" onclick="javascript:UpdateCell();">
			<script>localization.showText('Update');</script>
		</button>
	    <span class="Ektron_LeftSpaceSmall"></span>
		<button class="Button" title="Cancel" onclick="javascript:CloseDlg();">
			<script>localization.showText('Cancel');</script>
		</button>
	</div> 
</div>
<script language="javascript">
	function InitControl()
	{	
		var args = GetDialogArguments();		
		<%=theCellPropertiesControl.ClientID%>.Initialize(args.cellToModify, args.CssClasses, args.EditorObj, args.ColorsArray, args.CanAddCustomColors);
	}
	
	function UpdateCell()
	{
		<%=theCellPropertiesControl.ClientID%>.Update();
		CloseDlg();
	}
	
	AttachEvent(window, "load", InitControl);
</script>