<%@ Page Language="C#" AutoEventWireup="true" Inherits="adreports" CodeFile="adreports.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>adreports</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
		<!--//--><![CDATA[//><!--
			function SubmitForm(FormName, Validate) {
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

    <style type="text/css">
        .itemstatus {
            background-image:url(images/UI/icons/asteriskOrange.png);
            background-position:left center;
            background-repeat:no-repeat;
            padding-left:20px;
        }
        .spacer5em {
            margin-bottom: .5em
        }
    </style>

    <asp:literal id="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
            <div class="ui-widget" id="status" runat="server" visible="false">
                <div class="ui-state-highlight ui-corner-all" style="padding: 0 0.7em; margin-top: 20px;"> 
                    <span class="ui-icon ui-icon-info" style="float: left; margin-right: 0.3em;"></span>
                        <asp:Literal ID="ltr_status" runat="server"></asp:Literal>  
                </div>
                <div class="spacer5em"></div>
            </div>
            <div id="TR_count" runat="server">
                <span id="TD_count" runat="server"></span>
            </div>
             <div class="ektronPageGrid">
                <asp:Panel ID="pnlContainer" runat="server" CssClass="ektronPageInfo">
                    <asp:DataGrid ID="AdReportsGrid" CssClass="ektronGrid"
                        runat="server"
                        AutoGenerateColumns="False"
                        Width="100%"
                        EnableViewState="False"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:DataGrid>
                </asp:Panel>
            </div>
        </div>
        <input type="hidden" name="usercount" value="0" id="usercount" runat="server" />
    </form>
</body>
</html>

