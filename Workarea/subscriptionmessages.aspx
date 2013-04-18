<%@ Page Language="C#" AutoEventWireup="true" Inherits="suscriptionmessages" ValidateRequest="false"
    CodeFile="subscriptionmessages.aspx.cs" %>

<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>Subscription Messages</title>

    <script type="text/javascript">
            <!--//--><![CDATA[//><!--	
		var jsIsMac='<asp:literal id="jsIsMac" runat="server"/>';
		var AutoNav = '<asp:literal id="AutoNav" runat="server"/>';
		var defaultFolderId='<asp:literal id="defaultFolderId" runat="server"/>';
		var invalidFormatMsg = '<asp:literal id="invalidFormatMsg" runat="server"/>';
		var invalidYearMsg = '<asp:literal id="invalidYearMsg" runat="server"/>';
		var invalidMonthMsg = '<asp:literal id="invalidMonthMsg" runat="server"/>';
		var invalidDayMsg = '<asp:literal id="invalidDayMsg" runat="server"/>';
		var invalidTimeMsg = '<asp:literal id="invalidTimeMsg" runat="server"/>';
		var buttonPressed = false;

		function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
				var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
				var popupwin = window.open(url, hWind, cToolBar);
				return popupwin;
			}

		<asp:literal id="jsSetActionFunction" runat="server"/>

		function resetPostback()
        {
            document.forms[0].isPostData.value = "";
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
			return confirm('<asp:literal id="delSubScriptionMsg" runat="server"/>');
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

		//Hide subject field for unsupported types.
		//Right now, Subject is only supported for Site and Friend Invitations.
		//all of this can be removed once all messages support subject.
		function OnMessageTypeChanged(typeDropDown){
        
		    var selectedValue = parseInt(typeDropDown.options[typeDropDown.selectedIndex].value);
		    var subjectRow = document.getElementById("rowSubject");

		    if((selectedValue >= 3 && selectedValue <= 19) || selectedValue >= 49 ){
		        subjectRow.style.visibility="visible";
                //subjectRow.style.display = "table-row";
                $ektron(subjectRow).css("display", "table-row")
		    }
		    else{
		        subjectRow.style.visibility="hidden";
                subjectRow.style.display="none";
		    }
		}
	         //--><!]]>	
    </script>

    <asp:Literal ID="StyleSheetJS" runat="server" />
    <asp:Literal ID="jsEditorScripts" runat="server" />
</head>
<body>
    <form id="subscription" runat="server">
    <div id="dhtmltooltip">
    </div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="View & Edit Messages" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
        </div>
    </div>
    <div class="ektronPageContainer">
        <div id="TR_AddEditSubscription" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="Title">
                            <%=m_refMsg.GetMessage("generic Title")%>:
                        </td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Title of Message here" ID="txtName" runat="server" MaxLength="255" />
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
                        <td class="label" title="Type">
                            <%=m_refMsg.GetMessage("generic Type")%>:
                        </td>
                        <td class="value">
                            <asp:DropDownList ToolTip="Select Message Type from the Drop Down Menu" ID="drpTypeAddEdit"
                                runat="server" OnChange="OnMessageTypeChanged(this);" runat="server" />
                        </td>
                    </tr>
                    <tr id="tr_default" visible="false" runat="server">
                        <td class="label" title="Default Add/Edit">
                            <asp:Literal ID="ltrDefaultAddEdit" runat="server" />
                        </td>
                        <td class="value">
                            <asp:CheckBox ToolTip="Default Add/Edit Option" ID="chkDefaultAddEdit" runat="server"
                                Enabled="false" />
                        </td>
                    </tr>
                    <tr id="rowSubject" runat="server">
                        <td class="label" title="Subject">
                            <%=m_refMsg.GetMessage("generic subject label")%>
                        </td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Subject here" ID="txtSubject" runat="server" MaxLength="255"
                                Rows="50" />
                        </td>
                    </tr>
                </table>
                <div class="ektronTopSpace">
                </div>
                <div class="ektronHeader" title="Text">
                    <%=m_refMsg.GetMessage("lbl Text")%></div>
                <input type="hidden" name="ephox" id="ephox" value="false" />
                <asp:PlaceHolder ID="phEditor" runat="server"></asp:PlaceHolder>
                <%--<ektron:ContentDesigner ID="txtTextAddEdit" runat="server" AllowScripts="false" Height="100%"
                    Width="100%" Toolbars="Minimal" ShowHtmlMode="true" />--%>
                <br />

                <script type="text/javascript">
					        Ektron.ready( function(){
						        document.forms[0].txtName.focus();
						    });
                </script>

            </div>
        </div>
        <div id="TR_ViewSubscription" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="Title">
                            <%=m_refMsg.GetMessage("generic title")%>:
                        </td>
                        <td class="readOnlyValue" title="Title Value">
                            <asp:Literal ID="ltrViewName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="ID">
                            <%=m_refMsg.GetMessage("generic SubscriptionID")%>:
                        </td>
                        <td class="readOnlyValue" title="ID #">
                            <asp:Literal ID="ltrViewID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="Type">
                            <%=m_refMsg.GetMessage("type label")%><%= m_refMsg.GetMessage("generic SubscriptionEnable")%>
                        </td>
                        <td class="value">
                            <asp:DropDownList ToolTip="Select Type from Drop Down Menu" ID="drpType" runat="server"
                                Enabled="False" />
                        </td>
                    </tr>
                    <tr id="tr_defaultView" visible="false" runat="server">
                        <td class="label" title="Default">
                            <asp:Literal ID="ltrDefault" runat="server" />
                        </td>
                        <td class="value">
                            <asp:CheckBox ToolTip="Default Option" ID="chkDefault" runat="server" Enabled="False" />
                        </td>
                    </tr>
                    <tr id="rowSubjectView" runat="server">
                        <td class="label" title="Subject">
                            <%=m_refMsg.GetMessage("generic subject label")%>
                        </td>
                        <td class="readOnlyValue" title="Subject">
                            <asp:Literal ID="literalSubject" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="Text">
                            <%=m_refMsg.GetMessage("lbl Text")%>
                        </td>
                        <td class="readOnlyValue" title="Text Value">
                            <asp:Literal ID="viewContentHTML" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="TR_ViewAllFont" runat="server">
            <div class="ektronPageGrid">
                <asp:DataGrid ID="ViewSubscriptionGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                    CssClass="ektronGrid" GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                    <input type="hidden" runat="server" name="hdnUnit" value="hidden" id="hdnUnit" />
                    <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage"
                    Text="[First Page]" OnCommand="NavigationLink_Click" CommandName="First" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage"
                    Text="[Previous Page]" OnCommand="NavigationLink_Click" CommandName="Prev" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage"
                    Text="[Next Page]" OnCommand="NavigationLink_Click" CommandName="Next" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage"
                    Text="[Last Page]" OnCommand="NavigationLink_Click" CommandName="Last" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
