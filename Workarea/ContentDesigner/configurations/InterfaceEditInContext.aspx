<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InterfaceEditInContext.aspx.cs" Inherits="ContentDesigner_configurations_InterfaceEditInContext" Theme="" %>
<root>
	<tools name="General" dockable="false">
		<tool name="EkInContextSave" />
		<tool separator="true"/>
		<tool name="AjaxSpellCheck" />
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<%if (!string.IsNullOrEmpty(Request.QueryString["LibraryAllowed"]))
        { %>
		    <%if (Convert.ToBoolean(Request.QueryString["LibraryAllowed"]) == true){ %>
		    <tool name="EkLibrary" />
		    <%}%>
		<%}%>
		<tool separator="true"/>
	    <tool name="Bold" />
	    <tool name="Italic" />
	</tools>
	<tools name="Cancel" dockable="false">
	    <tool name="EkInContextCancel" />
	</tools>
	<contextMenus>
		<contextMenu forElement="IMG">
			<tool name="SetImageProperties"/>
		</contextMenu>
	</contextMenus >
</root>

