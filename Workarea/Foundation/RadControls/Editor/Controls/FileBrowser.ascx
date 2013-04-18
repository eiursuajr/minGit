<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FileBrowser.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.FileBrowser" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<input type="hidden" id="selectedItemPathHolder" runat="server"> <input type="hidden" id="selectedItemTypeHolder" runat="server">
<input type="hidden" id="selectedItemNameHolder" runat="server"> <input type="hidden" id="selectedItemTagHolder" runat="server">
<table cellpadding="0" cellspacing="0">
	<tr>
		<td>
			<table border="0" align="left" height="10" id="FileBrowserImageButtonHolder" cellpadding="0"
				cellspacing="0" class="ImageButtonHolder">
				<tr>
					<td><a title="Refresh" href="#" id="<%=this.ClientID%>refreshButtonLink" onclick="document.forms[0].submit();">
							<button class="ImageButton" type="button">
								<telerik:editorschemeimage id="refresh" align="absMiddle" relsrc="Dialogs/refresh.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a>
						<script>localization.setAttribute("refreshButtonLink", "title", "Refresh")</script>
					</td>
					<td><button class="ImageButton" type="button" title="View Image Tree" id="treeViewButton" runat="server">
							<telerik:editorschemeimage id="treeViewImage" align="absMiddle" relsrc="Dialogs/TreeView.gif" runat="server"
								border="0"></telerik:editorschemeimage></button>
					</td>
					<td><button class="ImageButton" type="button" title="View Image List" id="listViewButton" runat="server">
							<telerik:editorschemeimage id="listViewImage" align="absMiddle" relsrc="Dialogs/ListView.gif" runat="server"
								border="0"></telerik:editorschemeimage></button>
					</td>
					<td style="<%=this.ContentProvider.CanCreateDirectory?"":"display:none;"%>"><a href="#"><button class="ImageButton" title="New Folder" id="NewFolderButton" onClick="return <%=this.ClientID%>.CreateNewFolder()"
								type=button>
								<telerik:editorschemeimage id="newFolderIcon" align="absMiddle" relsrc="Dialogs/newFolderIcon.gif" runat="server"
									border="0"></telerik:editorschemeimage></button> </a>
						<script>localization.setAttribute("NewFolderButton", "title", "NewFolder")</script>
					</td>
					<td><a href="#"> <button class="ImageButton" title="Delete" id="DeleteButton" runat="server" type="button">
								<telerik:editorschemeimage id="deleteIcon" relsrc="Dialogs/deleteIcon.gif" border="0" runat="server"></telerik:editorschemeimage></button>
						</a>
						<script>localization.setAttribute("DeleteButton", "title", "Delete")</script>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<div runat="server" id="NewFolderDiv" class="DialogUtilityArea" style="DISPLAY:none">
				<table cellpadding="0" cellspacing="0" border="0">
					<tr>
						<td width="100%">
							<asp:textbox ToolTip="Enter New Folder Name Text here" id="newFolderNameHolder" cssclass="RadETextBox" enableviewstate="False" runat="server"></asp:textbox>
						</td>
						<td>
							<button id="NewButton" title="Ok" runat="server" class="ImageButton" type="button">
								<telerik:editorschemeimage id="okIcon" align="absMiddle" relsrc="Dialogs/okIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</td>
						<td>
							<button class="ImageButton" title="Cancel New Folder Creation" onClick="return <%=this.ClientID%>.CancelNewFolderCreation()" type=button>
								<telerik:editorschemeimage id="cancelIcon" relsrc="Dialogs/cancelIcon.gif" runat="server"></telerik:editorschemeimage></button>
						</td>
					</tr>
				</table>
			</div>
			<div class="FileNodeTreeHolder">
				<table id="<%= this.ClientID %>FileList" cellpadding="0" cellspacing="0" style="WIDTH:100%">
					<thead>
						<tr>
							<td><input title="Exit" type="button"
	class="FileBrowserSortHeader"
	onclick="<%= this.ClientID %>.Sort('Extension');"
	id="<%= this.ClientID %>SortExtensionDirection"
	style="PADDING-LEFT:13px;WIDTH:100%;BACKGROUND-REPEAT:no-repeat;background-url:this.SkinPath %>Dialogs/empty.gif)"value="Ext"
	></td>
							<td><input title="Name" type="button"
	class="FileBrowserSortHeader"
	onclick="<%= this.ClientID %>.Sort('Name');"
	id="<%= this.ClientID %>SortNameDirection" 
	style="PADDING-LEFT:13px;WIDTH:100%;BACKGROUND-REPEAT:no-repeat;background-url:this.SkinPath %>Dialogs/empty.gif)"value="Name"
	></td>
							<td><input title="Size" type="button"
	class="FileBrowserSortHeader"
	onclick="<%= this.ClientID %>.Sort('Size');"
	id="<%= this.ClientID %>SortSizeDirection"
	style="PADDING-LEFT:13px;WIDTH:100%;BACKGROUND-REPEAT:no-repeat;background-url:this.SkinPath %>Dialogs/empty.gif)"value="Size"
	></td>
						</tr>
					</thead>
					<tbody>
					</tbody>
				</table>
			</div>
		</td>
	</tr>
</table>
<script type="text/javascript">
localization.setAttribute('<%= this.ClientID %>SortExtensionDirection', "value", "OrderExtension")
localization.setAttribute('<%= this.ClientID %>SortNameDirection', "value", "OrderName")
localization.setAttribute('<%= this.ClientID %>SortSizeDirection', "value", "OrderSize")

var <%= this.ClientID %> = new RadEditorNamespace.FileBrowser(
		'<%= this.ClientID %>',
		document.getElementById('<%= this.ClientID %>FileList'),
		'<%= this.SkinPath %>',
		document.getElementById('<%= selectedItemPathHolder.ClientID %>'),
		document.getElementById('<%= selectedItemTypeHolder.ClientID %>'),
		document.getElementById('<%= selectedItemNameHolder.ClientID %>'),
		document.getElementById('<%= selectedItemTagHolder.ClientID %>'),
		document.getElementById('<%= NewFolderDiv.ClientID %>'),
		document.getElementById('<%= newFolderNameHolder.ClientID %>'),
		document.getElementById('<%= NewButton.ClientID %>'),
		document.getElementById('<%= DeleteButton.ClientID %>'),
		document.getElementById('<%= this.ClientID %>SortExtensionDirection'),
		document.getElementById('<%= this.ClientID %>SortNameDirection'),
		document.getElementById('<%= this.ClientID %>SortSizeDirection'),
		document.getElementById('<%= this.ClientID %>refreshButtonLink'),
		'<%= this.UniqueID %>',
		document.getElementById('<%= this.GetForm().ClientID %>')
		);
</script>
<asp:literal id="literalInitializer" runat="server" />
