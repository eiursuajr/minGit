<%@ Page Language="C#" AutoEventWireup="true" Inherits="linkview" CodeFile="linkview.aspx.cs" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>index</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<asp:literal id="StyleSheetJS" runat="server" />
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<CMS:ContentBlock id="ContentBlock1" runat="server" DynamicParameter="id" DefaultContentID="24"></CMS:ContentBlock>
			<CMS:FormBlock id="FormBlock1" runat="server" DynamicParameter="ekfrm" DefaultContentID="24"></CMS:FormBlock>
		</form>
	</body>
</html>

