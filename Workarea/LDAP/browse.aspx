<%@ Page Language="C#" AutoEventWireup="true" CodeFile="browse.aspx.cs" Inherits="LDAPbrowse" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<head runat="server">
		<title>search</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<asp:literal id="StyleSheetJS" runat="server" />
		<script language="JavaScript" src="../java/jfunct.js">
</script>
		<script language="JavaScript" src="../java/toolbar_roll.js">
</script>
		<asp:literal id="ltrBrowseJS" runat="server" />
	</head>
	<body>
		<FORM id="Form1" method="post" runat="server">
			
						<TABLE id="Table1">
							<TR>
								<TD class="input-box-text" style="WIDTH: 128px">Path:</TD>
								<TD>
									<asp:Literal id="ltrpath" runat="server"></asp:Literal></TD>
							</TR>
							<TR>
								<TD class="input-box-text" style="WIDTH: 128px">Org/Domain:</TD>
								<TD>
									<asp:Literal id="ltrorgdomain" runat="server"></asp:Literal></TD>
							</TR>
							<TR>
								<TD colspan="2">
									<asp:literal id="ltrMain" runat="server"></asp:literal></TD>
							</TR>
						</TABLE>
						<input type="hidden" id="cn_name" name="cn_name" value="" />
						<input type="hidden" id="cn_path" name="cn_path" value="" />
						<div style="visibility: hidden; z-index: 2; width: 100%; position: absolute; top: 95px;
                height: 1px; background-color: white" id="dvHoldMessage" align="center">

                <table style="height: 1px" border="2" cellpadding="0" cellspacing="0" bgcolor="white"
                    width="80%" id="Table5">
                    <tr>
                        <td valign="top" nowrap align="center">
                            <h3 style="color: red">
                                <strong>
                                    One Moment Please....</strong></h3>
                            <img align="top" style="width: 95%; height: 8px" src="../images/application/loading.gif"
                                ondragstart="return false;" border="0"></td>
                    </tr>

                </table>
                <br>
                &nbsp;<br>
                &nbsp;<br>
                &nbsp;
            </div>
		</FORM>
	</body>
</HTML>

