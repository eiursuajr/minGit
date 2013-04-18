<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImagePropertiesControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ImagePropertiesControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="AlignmentSelector" Src="./AlignmentSelector.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ColorPicker" Src="./ColorPicker.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ImageDialogCaller" Src="./ImageDialogCaller.ascx"%>
<%@ Register TagPrefix="telerik" TagName="SpinBox" Src="./SpinBox.ascx"%>

<table style="padding-right:10px">
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Border Width">
			<script>localization.showText('BorderWidth');</script>
		</td>
		<td colspan="2">
			<telerik:spinbox
				id="borderSizeSpinBox"
				runat="server"/>
		</td>					
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Border Color">
			<script>localization.showText('BorderColor');</script>
		</td>
		<td colspan="2">
			<telerik:colorpicker
				id="ImgBorderColorPicker"
				runat="server"/>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Alt Image Text">
			<script>localization.showText('ImageAltText');</script>
		</td>
		<td colspan="2">
		    <!-- Ektron Editor starts -->
			<table>
				<tr>
					<td><input type="text" id="<%=this.ClientID%>_alt" class="Ektron_StandardTextBox"></td>
					<td width="20" style="padding-left:5px"><telerik:editorschemeimage relsrc="Dialogs/Accessibility.gif" id="Editorschemeimage2" runat="server"></telerik:editorschemeimage></td>
				</tr>
			</table>
			<!-- Ektron Editor ends -->
		</td>
	</tr>
	<!-- Ektron Editor starts -->
	<input type="hidden" id="<%=this.ClientID%>_longDescription" class="Ektron_StandardTextBox">
	<!-- Ektron Editor ends -->
	<tr>
		<td nowrap class="Ektron_StandardLabel">
			<script>localization.showText('ImageAlign');</script>
		</td>
		<td colspan="2">
			<telerik:alignmentselector
				id="ImgAlignmentSelector" 
				runat="server" 
				mode="IMG"/>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Change Image Source">
			<script>localization.showText('ChangeImageSrc');</script>
		</td>
		<td colspan="2">
			<telerik:ImageDialogCaller
				id="changeSourceImageDialogCaller"
				runat="server"/>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Horizontal Spacing">
			<script>localization.showText('HorizontalSpacing');</script>
		</td>
		<td colspan="2">
			<telerik:spinbox
				id="horizontalSpacingSpinBox"
				runat="server"/>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel" title="Vertical Spacing">
			<script>localization.showText('VerticalSpacing');</script>
		</td>
		<td colspan="2">
			<telerik:spinbox
				id="verticalSpacingSpinBox"
				runat="server"/>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel">
			<script>localization.showText('Width');</script>
		</td>
		<td>
			<input type="text" id="<%=this.ClientID%>_width" maxlength="4" style="width:40px;" onkeydown="return <%=this.ClientID%>.ValidateNumber(event);" onkeyup="return <%=this.ClientID%>.ValidateDimension(event, true);">
		</td>
		<td rowspan="3">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td height="8"><telerik:editorschemeimage relsrc="Dialogs/constrainTop.gif" id="constrainTop" runat="server" /></td>
				</tr>
				<tr>
					<td height="18" onclick="<%=this.ClientID%>.ConstrainPropotions()" style="cursor:hand" nowrap>
						<telerik:editorschemeimage relsrc="Dialogs/constrainOff.gif" id="constrainImg" runat="server" align="absmiddle" />&nbsp;
						<span class="Ektron_StandardLabel"><script>localization.showText('Constrain');</script></span>
					</td>
				</tr>
				<tr>
					<td height="8"><telerik:editorschemeimage relsrc="Dialogs/constrainBottom.gif" id="constrainBottom" runat="server" /></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td nowrap class="Ektron_StandardLabel">
			<script>localization.showText('Height');</script>
		</td>
		<td>
			<input type="text" id="<%=this.ClientID%>_height" maxlength="4" style="width:40px" onkeydown="return <%=this.ClientID%>.ValidateNumber(event);" onkeyup="return <%=this.ClientID%>.ValidateDimension(event, false);" class="Text">
		</td>
	</tr>
</table>
<script language="javascript">
	var <%=this.ClientID%> = new ImagePropertiesControl(
			'<%=this.ClientID%>',
			<%=ImgBorderColorPicker.ClientID%>,
			<%=ImgAlignmentSelector.ClientID%>,
			document.getElementById('<%=constrainImg.ClientID%>'),
			<%=borderSizeSpinBox.ClientID%>,
			<%=horizontalSpacingSpinBox.ClientID%>,
			<%=verticalSpacingSpinBox.ClientID%>,
			<%=changeSourceImageDialogCaller.ClientID%>);
</script>