<%@ Page Language="C#" AutoEventWireup="true" Inherits="previewwebalert" CodeFile="previewwebalert.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Preview Web Alert</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<asp:literal id="StyleSheetJS" runat="server"/>
	</head>
	<body>
		<form method="post" runat="server">
            <div id="dhtmltooltip"></div>
		
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
            </div>
            <div class="ektronPageContainer ektronPageInfo">
			  <table class="ektronForm">
				<tr id="TR_Preview" runat="server">
					<td>
						<asp:literal id="ltrJS" runat="server"/>
						<asp:literal id="ltrOptOut" runat="server"/>
						<asp:literal id="ltrDefault" runat="server"/>
						<asp:literal id="ltrSummary" runat="server"/>
						<asp:literal id="ltrContent" runat="server"/>
						<asp:literal id="ltrContentLink" runat="server"/>
						<asp:literal id="ltrUnsubscribe" runat="server"/>
					</td>
				</tr>
			  </table>
		    </div>
		</form>
	</body>
</html>

