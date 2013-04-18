<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ThumbnailCreator.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ThumbnailCreator" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<fieldset>
	<legend>
		<script>localization.showText('CreateThumbnail');</script>
	</legend>
	<table border="0" cellpadding="0" cellspacing="0">
		<tr id="<%=this.ClientID%>_htrMessage">
			<td class="ErrorMessage" colspan="2" id="<%=this.ClientID%>_htcMessage"></td>
		</tr>
		<tr>
			<td class="Label" title="New Image Name">
				<script language="javascript">localization.showText('NewImageName');</script>:
			</td>
			<td>
				<asp:TextBox ToolTip="Enter New Image Name" id="txtNewImageName" runat="server" CssClass="text"/>
				<input type="hidden" id="hdOriginalImageLocation" runat="server" NAME="hdOriginalImageLocation"/>
			</td>
		</tr>
		<tr>
			<td class="Label" title="Width">
				<script language="javascript">localization.showText('Width');</script>
			</td>
			<td>
				<asp:TextBox ToolTip="Enter Width" id="txtWidth" runat="server" CssClass="text"/>
			</td>
		</tr>
		<tr>
			<td class="Label" title="Height">
				<script language="javascript">localization.showText('Height');</script>
			</td>
			<td>
				<asp:TextBox ToolTip="Enter Height" id="txtHeight" runat="server" CssClass="text"/>
			</td>
		</tr>
		<tr>
			<td class="Label" title="Dimenson Unit Type">
				<script language="javascript">localization.showText('DimentionUnit');</script>
			</td>
			<td>
				<asp:DropDownList ToolTip="Enter the Dimension Unit Type From the Drop Down Menu" ID="ddlDimentionUnit" Runat="server" CssClass="DropDown">
					<asp:ListItem Value="pixel"></asp:ListItem>
					<asp:ListItem Value="percent"></asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<asp:CheckBox ToolTip="Constrain Proportions" id="cbConstrainProportions" runat="server" text="<script language='javascript'>localization.showText('ConstrainProportions');</script>" checked="True"/>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<asp:CheckBox ToolTip="Overwrite Existing" id="cbOverwriteExisting" runat="server" text="<script language='javascript'>localization.showText('OverwriteExisting');</script>"/>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<button title="Create" class="Button" id="btnCreate" runat="server" type="button"><script language='javascript'>localization.showText('Create');</script></button>
			</td>
		</tr>
	</table>
</fieldset>
<script language="javascript">
	var <%=this.ClientID%> = new ThumbnailCreator(
		'<%=this.ClientID%>',
		document.getElementById("<%=this.txtWidth.ClientID%>"),
		document.getElementById("<%=this.txtHeight.ClientID%>"),
		document.getElementById("<%=this.txtNewImageName.ClientID%>"),
		document.getElementById("<%=this.hdOriginalImageLocation.ClientID%>"),
		document.getElementById("<%=this.cbConstrainProportions.ClientID%>"),
		document.getElementById("<%=this.ddlDimentionUnit.ClientID%>"),
		document.getElementById("<%=this.btnCreate.ClientID%>"),
		document.getElementById("<%=this.cbOverwriteExisting.ClientID%>")
		);
</script>