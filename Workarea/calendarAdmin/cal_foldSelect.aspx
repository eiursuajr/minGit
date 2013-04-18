<%@ Page Language="C#" AutoEventWireup="true" Inherits="cal_foldSelect" CodeFile="cal_foldSelect.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>calevtype_list</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		
		<script type="text/javascript" language="JavaScript">
		function folderClick(inId, inName) {
			// For v5.1:
			// window.parent.document.frm_calendar.frm_rootfolder_id.value = inId ;
			// For v5.0:
			window.parent.document.calendar.frm_folder_id.value = inId ;
			window.parent.document.getElementById('span_rootfolder_text').innerHTML = inName ;
		}
		</script>
	</head>
	<body>
		<form action="" id="calendar" method="post">
			<asp:Literal id="TestDate" runat="server"></asp:Literal>
			<asp:Literal id="JSInc" runat="server"></asp:Literal>
		</form>
	</body>
</html>

