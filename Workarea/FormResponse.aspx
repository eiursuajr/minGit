<%@ Page Language="C#" AutoEventWireup="true" Inherits="formresponse" CodeFile="formresponse.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	    <title  runat="server" />
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<asp:literal id="StyleSheetJS" runat="server" />
	</head>
	<body>
		<form id="frmReport" name="frmReport" method="post" runat="server">
			<table>
			    <tr>
			         <!-- <td><asp:datagrid id="FormReportGrid" runat="server" HorizontalAlign="Left" Width="100%" AutoGenerateColumns="False"
							BorderStyle="None" BorderWidth="0"  Visible="true">							
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
							<HeaderStyle CssClass="title-header"></HeaderStyle>
						</asp:datagrid></td> -->
						<asp:label id="lblTbl" Runat="server" Visible="False"></asp:label>
			    </tr>
			    <tr>
			        <td>
			        <asp:image id="chart" Runat="server" Visible="False" ImageUrl="chart.aspx"></asp:image>
			        </td>
			    </tr>
			</table>
		</form>
	</body>
</html>

