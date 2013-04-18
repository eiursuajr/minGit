<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TemplatePreviewer.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.TemplatePreviewer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td height="23px">&nbsp;</td>
	</tr>
	<tr>
		<td>
			<div class="PreviewAreaHolder">
				<iframe style="width:100%;height:98%;margin:0px;" id="<%=this.ID%>_previewArea" src="javascript:''"></iframe>
			</div>
		</td>
	</tr>
</table>
<script language="javascript">
	var <%=this.ID%> = new TemplatePreviewer('<%=this.ID%>');
</script>