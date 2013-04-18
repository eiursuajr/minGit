<%@ Page Language="C#" AutoEventWireup="true" Inherits="suscriptions" CodeFile="subscriptions.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Subscriptions</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

    <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
		    function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
				    var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
				    var popupwin = window.open(url, hWind, cToolBar);
				    return popupwin;
			    }

		    function VerifyForm () {
			    document.forms[0].txtName.value = Trim(document.forms[0].txtName.value);
			    if (document.forms[0].txtName.value == "")
			    {
				    alert ('<asp:Literal id="ltr_nameReq" runat="server" />');
				    document.forms[0].txtName.focus();
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
			    return confirm('<asp:Literal id="ltr_confirmDelete" runat="server" />');
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

    <asp:Literal ID="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="subscription" method="post" runat="server">
    <div id="dhtmltooltip">
    </div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="Subscriptions" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
        </div>
    </div>
    <div class="ektronPageContainer">
        <div id="TR_AddEditSubscription" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="Name">
                            <%=m_refMsg.GetMessage("subscription name input msg")%>:
                        </td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Name here" ID="txtName" runat="server" MaxLength="255" />
                        </td>
                    </tr>
                    <tr id="TD_SubscriptionID" runat="server">
                        <td class="label" title="ID">
                            <%=m_refMsg.GetMessage("generic SubscriptionID")%>:
                        </td>
                        <td class="readOnlyValue" title="ID #">
                            <asp:Literal ID="ltrAddEditID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="Enabled">
                            <%=m_refMsg.GetMessage("generic SubscriptionEnable")%>:
                        </td>
                        <td class="value">
                            <asp:CheckBox ToolTip="Enable/Disable Subscription" ID="chkEnableAddEdit" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>

            <script type="text/javascript">
				        Ektron.ready(function(){
				            document.forms[0].txtName.focus();
				        });
            </script>

        </div>
        <div id="TR_ViewSubscription" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="Name">
                            <%=m_refMsg.GetMessage("generic Subscriptionname")%>:
                        </td>
                        <td class="readOnlyValue" title="Name Value">
                            <asp:Literal ID="ltrViewName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="ID">
                            <%=m_refMsg.GetMessage("generic SubscriptionID")%>:
                        </td>
                        <td class="readOnlyValue" title="View ID">
                            <asp:Literal ID="ltrViewID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="Enabled">
                            <%=m_refMsg.GetMessage("generic SubscriptionEnable")%>:
                        </td>
                        <td class="value">
                            <asp:CheckBox ToolTip="Enable/Disable Subscription" ID="chkEnable" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="TR_ViewAllFont" runat="server">
            <div class="ektronPageGrid">
                <asp:DataGrid ID="ViewSubscriptionGrid" runat="server" AutoGenerateColumns="False"
                    Width="100%" CssClass="ektronGrid" GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
