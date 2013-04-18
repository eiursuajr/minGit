<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImageInfoControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ImageInfoControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellpadding="3" cellspacing="0">
	<tr>
		<td nowrap class="Label" title="File Name">
			<script>localization.showText('FileName');</script>
		</td>
		<td id="radEditorImageSrc" class="Text"></td>					
	</tr>
	<tr>
		<td nowrap class="Label" title="Image Size">
			<script>localization.showText('ImageSize');</script>
		</td>
		<td id="radEditorImageSize" class="Text"></td>
	</tr>
	<tr>
		<td nowrap class="Label" title="Image Width">
			<script>localization.showText('ImageWidth');</script>
		</td>
		<td id="radEditorOriginalWidth" class="Text"></td>
	</tr>
	<tr>
		<td height="5"></td>
	</tr>
	<tr>
		<td width="80" class="Label" title="Image Heigh">
			<script>localization.showText('ImageHeight');</script>
		</td>
		<td id="radEditorOriginalHeight" class="Text"></td>
	</tr>
</table>
<script language="javascript">
	var <%=this.ClientID%> = new ImageInfoControl();
</script>