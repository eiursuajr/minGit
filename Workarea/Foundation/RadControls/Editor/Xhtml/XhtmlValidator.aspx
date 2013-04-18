<%@ Page ValidateRequest="false" language="c#" CodeFile="XhtmlValidator.aspx.cs" EnableEventValidation="false" EnableViewStateMac="false" EnableViewState="false" AutoEventWireup="false" Inherits="Ektron.Telerik.WebControls.RadEditorUtils.EkXhtmlValidator" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>XhtmlValidator</title>
		<style>
		A{cursor: default}
		</style>
		<script language="JavaScript" type="text/javascript">
        <!-- 
        function disableMouseClick(mousebutton)
        {
            if (typeof mousebutton != "undefined")
            {
                if (3 == mousebutton.which || 1 == mousebutton.which)
                {
                    return false;
                }
            }
            else if (typeof event != "undefined")
            {
                if (1 == event.button || 2 == event.button)
                {
                    return false;
                }
            }
            return true;
        }
        document.onmousedown = disableMouseClick;
        // -->
        </script>
        <asp:literal id="sBaseTag" runat="server"></asp:literal>
	</head>
	<body style="font-family:Tahoma,Arial;font-size:12px;">
	    <div id="divReportPane" runat="server" onclick="return false;" ></div>
		<form id="RadEditorXhtmlForm" method="post" runat="server">
			<input id="EditorContent" type="hidden" value="" name="EditorContent" runat="server" />
			<input id="EditorDoctype" type="hidden" value="" name="EditorDoctype" runat="server" />
			<asp:Button ToolTip="Submit" ID="Submit" Runat="server" Text="Click" Visible="false"></asp:Button>
			<div id="Disclaimer" runat="server" visible="false">Note: This module uses  <a title="validate content for XHTML compliance" href="http://validator.w3.org" target=\"_blank\">http://validator.w3.org</a> to validate content for XHTML compliance.</div>
		</form>
	</body>
</html>
