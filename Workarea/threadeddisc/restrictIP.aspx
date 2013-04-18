<%@ Page Language="C#" AutoEventWireup="true" CodeFile="restrictIP.aspx.cs" Inherits="threadeddisc_restrictIP" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Restrict IP</title>
</head>
<body>
    <form id="form1" runat="server">
		<div class="ektronPageContainer">
			<div id="divAE" runat="server">
				<div class="ektronPageInfo">
					<table class="ektronForm">
						<tr>
							<td class="label" title="Block IP"><asp:Literal ID="ltr_mask" runat="server" />:</td>
							<td class="value"><asp:TextBox ToolTip="Enter an IP address to block here" ID="txt_mask" runat="server" Columns="40" MaxLength="50" /></td>
						</tr>
						<tr id="tr_applies" runat="server">
							<td class="label" title="Applies To"><asp:Literal ID="ltr_appliesto" runat="server" />:</td>
							<td class="value"><asp:CheckBoxList ToolTip="Check off what blocked IP address applies to here" ID="cl_boards" runat="server" /></td>
						</tr>
					</table>
				</div>
			</div>
			<div id="divList" runat="server">
				<div class="ektronPageGrid">
					<asp:datagrid ID="dgRestricted"
						runat="server"
						AutoGenerateColumns="false"
						Width="100%"
						CssClass="ektronGrid"
						GridLines="None">
						<HeaderStyle CssClass="title-header" />
					</asp:datagrid>
				</div>
			</div>
		</div>
    </form>
</body>
</html>

