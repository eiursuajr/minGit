<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MediaPreviewer.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.MediaPreviewer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td>
			<table align="left" border="0" cellpadding="0" cellspacing="0" height="23">
				<tr>
					<td class="label">
						<input type="checkbox" title="Preview Mode" onclick="javascript:SwitchPreviewMode();">
						<script>localization.showText("SwitchToPreviewMode");</script>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<div class="PreviewAreaHolder" id="previewerDiv">
				<div id="PropertiesPane" style="display:none;">
					<table cellpadding="0" cellspacing="0">
						<tr>
							<td colspan="2" align="left" valign="top">
								<span id="loader" style="display:none"></span>
							</td>
						</tr>
						<tr>
							<td class="label" title="Width">
								<script>localization.showText('Width');</script>
							</td>
							<td>
								<input title="Enter the Media Width" type="text" defaultvalue="150" id="mediaWidth" class="RadETextBox" />
							</td>
						</tr>
						<tr>
							<td class="label" title="Height">
								<script>localization.showText('Height');</script>
							</td>
							<td>
								<input title="Enter the Media Height" defaultvalue="150" type="text" id="mediaHeight" class="RadETextBox" />
							</td>
						</tr>
						<tr>
							<td class="Label" title="Align">
								<script>localization.showText('Align');</script>
							</td>
							<td>
								<select defaultvalue="baseline" title="Select the Alignment of the Media" class="DropDown" id="mediaAlign">
								<script>
									document.write(
									'<option value="baseline">' +
										localization.getText('Baseline') + 
									'</option>' + 
									'<option value="bottom">' +
										localization.getText('Bottom') + 
									'</option>' + 
									'<option value="left">' +
										localization.getText('Left') + 
									'</option>' + 
									'<option value="middle">' +
										localization.getText('Middle') + 
									'</option>' + 
									'<option value="right">' +
										localization.getText('Right') + 
									'</option>' + 
									'<option value="texttop">' +
										localization.getText('TextTop') + 
									'</option>' + 
									'<option value="top">' +
										localization.getText('Top') + 
									'</option>'
									);									
								</script>
								</select>
							</td>
						</tr>
						<tr>
							<td class="Label" title="Properties">
								<script>localization.showText('Properties');</script>
							</td>
							<td>
								<select id="property" defaultvalue="" class="DropDown" onchange="changeProperty(this.value)">
								<script>
									document.write('<option value="">'
								+ localization.getText('SelectProperty') + '</option>');								
								</script>								
								</select>
							</td>
						</tr>
						<tr>
							<td class="Label" title="Value">
								<script>localization.showText('Value');</script>
							</td>
							<td class="Label" title="NA">
								<span id="propertyValue">
									<script>localization.showText('NA');</script>
								</span>
								<object id="dlgHelper" CLASSID="clsid:3050f819-98b5-11cf-bb82-00aa00bdce0b" width="0px" height="0px" VIEWASTEXT></object>
							</td>
						</tr>
					</table>
				</div>
				<div id="PreviewPane" title="Preview Pane">
					<table border="0" cellpadding="0" cellspacing="0" align="center" height="100%">
						<tr>
							<td valign="middle" id="PreviewObjectHolder"></td>
						</tr>
					</table>
				</div>
				<div id="EmptyPane" title="Empty Pane">
					<table border="0" cellpadding="0" cellspacing="0" align="center" height="100%">
						<tr>
							<td valign="middle"></td>
						</tr>
					</table>
				</div>
			</div>
		</td>
	</tr>
</table>