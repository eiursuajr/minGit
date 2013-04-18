<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.StyleBuilderHolder" AutoEventWireUp="false" CodeBehind="StyleBuilderHolder.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="StyleBuilder" Src="../Controls/StyleBuilder.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ OutputCache Duration="600" VaryByParam="Language;SkinPath" %>
<table id="MainTable" cellpadding="0" cellspacing="0" class="MainTable" border="0">
	<tr>
		<th height="39" valign="bottom" colspan="2">
			<telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainTable">
				<telerik:tab elementid="TabbedStyleBuilderHolder" selected="True" text="<script>localization.showText('Tab1HeaderText');</script>" image="Dialogs/TabIcons/StyleBuilderHolderTab1.gif" />
			</telerik:tabcontrol>
		</th>
	</tr>
	<tr>
		<td>
			<table id="TabbedStyleBuilderHolder" border="0" cellpadding="3" cellspacing="0" width="100%">
				<tr>
					<td>
						<telerik:stylebuilder
							id="theStyleBuilder"
							runat="server"/>
					</td>
				</tr>
			</table>
		</td>
		<td width="90" valign="top">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td>
						<button type="button" title="Ok" id="btnOk" class="Button" onclick="SetStyle();">
							<script>localization.showText('Update');</script>
						</button>
					</td>
				</tr>
				<tr>
					<td>
						<button type="button" title="Cancel" id="btnCancel" class="Button" onclick="CloseDlg();">
							<script>localization.showText('Cancel');</script>
						</button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script language="javascript">
	function InitControl()
	{
		var args = GetDialogArguments();
		<%=this.theStyleBuilder.ClientID%>.Initialize(args.StyledObject);
	}
	
	function SetStyle()
	{
		CloseDlg(<%=this.theStyleBuilder.ClientID%>.GetStyleText());
	}
	AttachEvent(window, "load", InitControl);
</script>