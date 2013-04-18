<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.SetTableProperties" AutoEventWireUp="false" CodeBehind="SetTableProperties.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="TablePropertiesControl" Src="../Controls/TablePropertiesControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="AccessibleTable" Src="../Controls/AccessibleTable.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ OutputCache Duration="600" VaryByParam="Language;SkinPath" %>

<div id="MainContainer">
	<div class="Ektron_DialogTabstrip_Container">
			<telerik:tabcontrol id="TabHolder" runat="server" resizecontrolid="MainContainer">
				<telerik:tab elementid="TabbedTableProperties" selected="True" text="<script>localization.showText('Tab1HeaderText');</script>" />
				<telerik:tab elementid="Tabbed508" text="<script>localization.showText('Tab2HeaderText');</script>" />
			</telerik:tabcontrol>
	</div> 
	<div class="Ektron_Dialog_Tabs_BodyContainer">
			<table id="Tabbed508" cellspacing="0" cellpadding="0">
				<tr>
					<td><telerik:accessibletable id="theAccessibleTable" runat="server"></telerik:accessibletable></td>
				</tr>
			</table>
			<table id="TabbedTableProperties" border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td>
						<telerik:TablePropertiesControl
							id="theTablePropertiesControl"
							runat="server"/>
					</td>
				</tr>
			</table>
	</div> 
	<div class="Ektron_DialogButtonContainer">
		<button class="Button" title="Ok" onclick="javascript:doUpdate();" type="button">
			<script>localization.showText('Update');</script>
		</button>
		<span class="Ektron_LeftSpaceSmall"></span>
		<button class="Button" title="Cancel" onclick="javascript:CloseDlg();" type="button">
			<script>localization.showText('Cancel');</script>
		</button>
	</div> 
</div> 
<script language="javascript">
	function InitControl()
	{
		var arguments = GetDialogArguments();
		var accessibleTableControl = <%=this.theAccessibleTable.ClientID%>;
		accessibleTableControl.Initialize(arguments.tableToModify, arguments.tableDocument);
		var tablePropertiesControl = <%=theTablePropertiesControl.ClientID%>;
		tablePropertiesControl.Initialize(arguments.tableToModify, arguments.tableCssClasses, arguments.EditorObj, arguments.ColorsArray, arguments.CanAddCustomColors);

		tablePropertiesControl.OnCancel = CloseDlg;
	}
	
	function doUpdate()
	{
		<%=this.theAccessibleTable.ClientID%>.UpdateTable();
		<%=theTablePropertiesControl.ClientID%>.UpdateTable();
		CloseDlg();
	}

	AttachEvent(window, "load", InitControl);
</script>