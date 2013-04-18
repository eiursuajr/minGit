<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DocumentPreviewer.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.DocumentPreviewer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td height="23px">&nbsp;</td>
	</tr>
	<tr>
		<td>
			<div class="PreviewAreaHolder">
				<table cellpadding="0" cellspacing="0" border="0">
					<tr>
						<td colspan="2" align="left" valign="top">
							<span id="loader" style="display:none"></span>
						</td>
					</tr>
					<tr>
						<td class="label">
							<script>localization.showText('Tooltip');</script>
						</td>
						<td>
							<input type="text" id="tooltip" style="width:120px" class="RadETextBox">&nbsp;
							<telerik:editorschemeimage relsrc="Dialogs/Accessibility.gif" id="Editorschemeimage2" runat="server"></telerik:editorschemeimage>
						</td>
					</tr>
					<tr>
						<td class="Label">
							<script>localization.showText('Target');</script>
						</label>
						</td>
						<td>
							<input type="text" id="linkTarget" style="width:120px" value="" class="RadETextBox">&nbsp;
							<select class="DropDown" onchange="changeTarget(this)">
							<script>
							document.write(														
								'<option selected value="">' + localization.getText('Target') + '</option>' + 
								'<option value="_blank">' + localization.getText('_blank') + '</option>' + 
								'<option value="_parent">' + localization.getText('_parent') + '</option>' + 
								'<option value="_self">' + localization.getText('_self') + '</option>' + 
								'<option value="_top">' + localization.getText('_top') + '</option>' + 
								'<option value="_search">' + localization.getText('_search') + '</option>' + 
								'<option value="_media">' + localization.getText('_media') + '</option>'
							);
							</script>	
							</select>
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
</table>