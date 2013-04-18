<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Roles.aspx.cs" Inherits="Roles" validateRequest="false" MaintainScrollPositionOnPostback="True"%>
<%@ Reference Control ="controls/roles/rolemembermgr.ascx" %>
<%@ Reference Control ="controls/roles/customroles.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Edit Task Roles</title>
		<asp:literal id="StyleSheetJS" runat="server" />

		<script type="text/javascript">
	<!--//--><![CDATA[//><!--
		var UniqueID="<asp:literal id="jsUniqueId" runat="server"/>";

		function SubmitForm(FormName, Validate) {
		    $ektron("#txtSearch").clearInputLabel();
			document.forms[0].submit();
			return false;
		}

		function Trim (string) {
			if (string.length > 0) {
				string = RemoveLeadingSpaces (string);
			}
			if (string.length > 0) {
				string = RemoveTrailingSpaces(string);
			}
			return string;
		}

		function RemoveLeadingSpaces(string) {
			while(string.substring(0, 1) == " ") {
				string = string.substring(1, string.length);
			}
			return string;
		}

		function RemoveTrailingSpaces(string) {
			while(string.substring((string.length - 1), string.length) == " ") {
				string = string.substring(0, (string.length - 1));
			}
			return string;
		}

	//--><!]]>
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<asp:PlaceHolder ID="DataHolder" Runat="server" />
		</form>
	</body>
</html>

