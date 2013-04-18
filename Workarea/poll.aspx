<%@ Page Language="C#" AutoEventWireup="true" Inherits="poll" CodeFile="poll.aspx.cs" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>CMS Poll</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<asp:literal id="StyleSheetJS" runat="server" />
	</head>
	<body>
		<form id="PollForm" method="post" runat="server">
            <cms:Poll id="Poll1" runat="server" DynamicParameter="ekfrm"></cms:Poll>
		</form>

		<form id="ReportForm" name="frmReport" method="post" runat="server">
			<table>
			<tbody>
				<tr>
					<asp:label id="lblReport" Runat="server" Visible="true"></asp:label>
				</tr>
			</tbody>
			</table>
		</form>
	</body>
</html>

