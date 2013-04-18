<%@ Page Language="C#" AutoEventWireup="true" Inherits="ekfont" CodeFile="font.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>font</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>

		<script type="text/javascript" language="javascript">
	<!--//--><![CDATA[//><!--
	    var ResourceText = {
	        jsFontNameRequiredMsg : '<asp:Literal id="jsFontNameRequiredMsg" runat="server" />',
	        jsConfirmDeleteFont : '<asp:Literal id="jsConfirmDeleteFont" runat="server" />'
	    }

		function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
				var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
				var popupwin = window.open(url, hWind, cToolBar);
				return popupwin;
			}

		function VerifyForm () {
			document.forms[0].FontFace.value = Trim(document.forms[0].FontFace.value);
			if (document.forms[0].FontFace.value == "")
			{
				alert (ResourceText.jsFontNameRequiredMsg);
				document.forms[0].FontFace.focus();
				return false;
			}
			return true;
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

		function ConfirmFontDelete() {
			return confirm(ResourceText.jsConfirmDeleteFont);
		}

		function SubmitForm(Validate) {
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[0].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[0].submit();
				return false;
			}
		}

	//--><!]]>
		</script>
		<asp:literal id="StyleSheetJS" runat="server"></asp:literal>
	</head>
	<body>
		<form method="post" runat="server">
			<div id="dhtmltooltip"></div>
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
			</div>
            <div class="ektronPageContainer">
			    <div id="TR_AddEditFont" runat="server">
				    <div class="ektronPageInfo">
					    <table class="ektronForm">
						    <tr>
							    <td class="label"><%=m_refMsg.GetMessage("font name input msg")%></td>
							    <td class="value"><input type="text" id="FontFace" name="FontFace" size="50" maxlength="50" runat="server"><input type="hidden" id="FontID" name="FontID" runat="server"></td>
						    </tr>
					    </table>
				    </div>
				    <script type="text/javascript">
				        <!--//--><![CDATA[//><!--
				        Ektron.ready( function() {
				            document.forms[0].FontFace.focus();
				        });
				        //--><!]]>
				    </script>
			    </div>
			    <div id="TR_ViewFont" runat="server">
				    <div class="ektronPageInfo">
					    <table class="ektronForm">
						    <tr>
							    <td class="label"><%=m_refMsg.GetMessage("font name input msg")%></td>
							    <td class="readOnlyValue" id="TD_FontFace" runat="server"></td>
						    </tr>
					    </table>
				    </div>
			    </div>
			    <div id="TR_ViewAllFont" runat="server">
				    <div class="ektronPageGrid">
				        <asp:DataGrid ID="ViewFontGrid"
				            runat="server"
				            AutoGenerateColumns="False"
					        Width="100%"
					        EnableViewState="False"
					        CssClass="ektronGrid"
					        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
				    </asp:DataGrid>
				    </div>
			    </div>
			</div>
		</form>
	</body>
</html>
