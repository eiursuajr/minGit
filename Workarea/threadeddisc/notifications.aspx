<%@ Page Language="C#" AutoEventWireup="true" CodeFile="notifications.aspx.cs" Inherits="threadeddisc_notifications" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Notifications</title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data" method="post">
    <table style="width:100%;border-collapse:collapse;">
	<tr>
		<td>
            <asp:FileUpload ID="ul_file" runat="server" TabIndex="-1" Width="100%" /></td>
	</tr>
	</table>
    <table style="width:100%;border-collapse:collapse;">
	<tr>
		<td><table id="Table7" style="width:100%;border-collapse:collapse;">
			<tr>
				<td id="txtTitleBar" class="ektronTitlebar"><span id="WorkareaTitlebar"><asp:Literal ID="ltr_addfile" runat="server"></asp:Literal></span><span style="display:none" id="_WorkareaTitlebar"></span></td>
			</tr>
			<tr>
			<td align="right">
			    <table style="width:100%;border-collapse:collapse;">
	            <tr>
			        <td>
			            <div style="visibility: hidden;" id="dvHoldMessage">
		                    <strong><asp:Literal ID="ltr_uploadmsg" runat="Server"></asp:Literal></strong>
	                    </div><div style="visibility: hidden;" id="dvErrorMessage">
		                    <span class="important"><strong><asp:Literal ID="ltr_error" runat="Server"></asp:Literal></strong></span>
	                    </div>
			        </td>
			        <td width="32" align="right" OnClick="cmd_attach_Click"><asp:Button ToolTip="Attach" ID="cmd_attach" runat="server" Text="Attach" /></td>
                </tr>
                </table>
            </td>
			</tr>
		    </table>
		</td>
	</tr>
	<tr>
		<td><table id="Table2" class="ektronForm" style="width:100%;border-collapse:collapse;">
			<tr>
				<td id="Td1" class="ektronTitlebar"><span id="Span1"><asp:Literal ID="ltr_currentfiles" runat="Server"></asp:Literal></span><span style="display:none" id="Span2"></span></td>
			</tr>
			<tr>
			    <td>
			        <table style="width:100%;border-collapse:collapse;">
	                    <tr>
			                <td><fieldset>
			                        <table width="100%">
			                            <tr>
			                            <td><div id="ek_filelist"><asp:Literal ID="ltr_filelist" runat="server" ></asp:Literal></div></td>
			                            </tr>
			                        </table>
			                    </fieldset>
			                </td>
			                <td width="32" align="right" valign="top"><asp:Button ToolTip="Remove" ID="cmd_remove" runat="server" Text="Remove" UseSubmitBehavior="False" /></td>
			            </tr>
			            <tr>
			                    <td>&nbsp;</td>
			                    <td align="right"><asp:Button ToolTip="Close" ID="cmd_close" runat="server" Text="Close" /></td>
                        </tr>
                    </table>
                </td>
			</tr>
		    </table>
        </td>
	</tr>
    </table>
<script language="javaScript" type="text/javascript">
<asp:Literal ID="ltr_bottomjs" runat="server"></asp:Literal>
</script>
</form>
</body>
</html>

