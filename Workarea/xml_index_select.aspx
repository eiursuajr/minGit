<%@ Page Language="C#" AutoEventWireup="true" Inherits="xml_index_select" ValidateRequest="false"
    CodeFile="xml_index_select.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>xml_index_select</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    
    <asp:Literal id="StyleSheetJS" runat="server" />
    
    <script type="text/javascript" language="javascript">
		function SetAction(Button) {
			if (Button == "cancel") {			
				return true;
			}
			else if (Button == "update") {		
				document.forms[0].submit();
				return false;
			}
		}
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer">
            <table class="ektronGrid">
                <tr>
                    <td id="TD_data" runat="server">
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
