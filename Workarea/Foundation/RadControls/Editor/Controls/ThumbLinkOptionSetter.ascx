<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ThumbLinkOptionSetter.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ThumbLinkOptionSetter" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table id="MainTable_<%=this.ClientID%>" cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td>
			<input type="checkbox" id="cbLink_<%=this.ClientID%>"/>
		</td>
		<td class="label" width="100%">
			<script language="javascript">localization.showText("LinkToOriginal");</script>
		</td>
	</tr>
	<tr>
		<td>
			<input type="checkbox" id="cbTarget_<%=this.ClientID%>"/>
		</td>
		<td class="label">
			<script language="javascript">localization.showText("OpenOriginalInBlank");</script>
		</td>
	</tr>
</table>

<script>
	var <%=this.ID%> = new ThumbLinkOptionSetter('<%=this.ClientID%>');
</script>