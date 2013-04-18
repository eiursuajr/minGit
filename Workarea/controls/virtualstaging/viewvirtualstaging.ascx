<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewvirtualstaging.ascx.cs" Inherits="viewvirtualstaging" %>
<table>
	<tr>
		<td class="info" title="Asset Location"><%=m_refMsg.GetMessage("asset location")%></td>
		<td>&nbsp;</td>
		<td align="center" id="td_asset_loc" runat="server"></td>
	</tr>
	<tr>
		<td class="info" title="Private Asset Location"><%=m_refMsg.GetMessage("private asset location")%></td>
		<td>&nbsp;</td>
		<td align="center" id="td_private_asset_loc" runat="server"></td>
	</tr>
	<tr>
		<td class="info" title="Domain/User Name"><%=m_refMsg.GetMessage("lbl domain username")%></td>
		<td>&nbsp;</td>
	    <td align="center" id="td_domain_username" runat="server"></td>
	</tr>
	
	
</table>

