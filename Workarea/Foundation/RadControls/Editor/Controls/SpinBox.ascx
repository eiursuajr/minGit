<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SpinBox.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.SpinBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td><input tabindex="<%=this.TabIndex%>" type="Text" id="<%=this.ClientID%>_textBox" name="<%=this.ClientID%>_textBox" onkeydown="<%=this.ClientID%>.OnTextBoxKeyDown(event);" onkeyup="<%=this.ClientID%>.OnTextBoxKeyUp();"></td>
		<td>
			<table>
				<tr>
					<td class="SizeButtonHolder"><img title="Increse Border Size" alt="Increse Border Size" id="incBorderSize" onclick="<%=this.ClientID%>.ModifyBorderSize(true);" onmouseover="<%=this.ClientID%>.ButtonOver(this);" onmouseout="<%=this.ClientID%>.ButtonOut(this);" src="<%=this.SkinPath%>Dialogs/plus2.gif"></td>
				</tr>
				<tr>
					<td height="1"></td>
				</tr>				
				<tr>
					<td class="SizeButtonHolder"><img title="Decrease Border Size" alt="Decrease Border Size" id="decBorderSize" onclick="<%=this.ClientID%>.ModifyBorderSize(false);" onmouseover="<%=this.ClientID%>.ButtonOver(this);" onmouseout="<%=this.ClientID%>.ButtonOut(this);" src="<%=this.SkinPath%>Dialogs/minus2.gif"></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script language="javascript">
	var <%=this.ClientID%> = new SpinBox('<%=this.ClientID%>');
</script>