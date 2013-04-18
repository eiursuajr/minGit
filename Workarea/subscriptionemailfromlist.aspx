<%@ Page Language="C#" AutoEventWireup="true" Inherits="subscriptionemailfromlist"
    CodeFile="subscriptionemailfromlist.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email From List</title>

    <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	        <asp:literal id="ltr_js" runat="server" />
		    function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
				    var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
				    var popupwin = window.open(url, hWind, cToolBar);
				    return popupwin;
			    }

		    function VerifyForm () {
			    document.forms[0].txtName.value = Trim(document.forms[0].txtName.value);
                var reEmailAddr = /^[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*$/;
		        var reSubstitutionField = new RegExp("^\xAB[^\xBB]+\xBB$");
			    if (document.forms[0].txtName.value == "")
			    {
				    alert ('<asp:Literal ID="jsEmailRequired" runat="server" />');
				    document.forms[0].txtName.focus();
				    return false;
			    }
                if(document.forms[0].txtName.value != "" && !reSubstitutionField.test(document.forms[0].txtName.value) && !reEmailAddr.test(document.forms[0].txtName.value)){
                    alert ('<asp:Literal ID="jsValidEmailAddress" runat="server" />');
				    document.forms[0].txtName.focus();
				    return false;
                }
			    return true;
		    }

		    function ConfirmFontDelete() {
			    return confirm('<asp:Literal ID="jsConfirmDeleteEmailFrom" runat="server" />');
		    }
		    function ConfirmDelete() {
		        var sIdList = "";
		        var frmObj = document.forms[0];
		        for (var i=0;i<frmObj.elements.length;i++) {
                    var e = frmObj.elements[i];
                    if ( (e.type=='checkbox') && (e.checked) ) {
                        if (sIdList != "") { sIdList = sIdList + ',' + e.value; }
                        else { sIdList = e.value; }
                    }
                }
		        if (sIdList != "") {
			        if (confirm('<asp:Literal ID="jsConfirmDeleteManyEmailFrom" runat="server" />')) { window.location = "subscriptionemailfromlist.aspx?action=delete&IDs=" + sIdList; }
			    } else {
			        alert('<asp:Literal ID="jsPleaseSelectEmailFrom" runat="server" />');
			    }
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
    <script type="text/javascript">
        $ektron(document).ready(function(){
            if($ektron(".ektronTitlebar #WorkareaTitlebar").text()=="Add Email From"){
                //$ektron(".ektronTitlebar #WorkareaTitlebar").parent().addClass("hide-title");
                //$ektron(".ektronPageHeader").addClass("no-top-padding");
            }

        });
    </script>

</head>
<body onclick="MenuUtil.hide()">
    <form id="subscription" method="post" runat="server" class="modal-content settings-email-from-list">
    <div id="dhtmltooltip">
    </div>
    <div class="ektronPageContainer">
        <div id="TR_AddEditSubscription" runat="server">
            <div class="ektronPageInfo">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="Email">
                            <%=m_refMsg.GetMessage("generic email")%>:
                        </td>
                        <td class="value">
                            <asp:TextBox ToolTip="Enter Email here" ID="txtName" runat="server" MaxLength="255" />
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
                        <td class="label" title="Email">
                            <%=m_refMsg.GetMessage("generic email")%>:
                        </td>
                        <td class="readOnlyValue" title="Email Name">
                            <asp:Literal ID="ltrViewName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="ID">
                            <%=m_refMsg.GetMessage("id label")%>
                        </td>
                        <td class="readOnlyValue" title="ID #">
                            <asp:Literal ID="ltrViewID" runat="server" />
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
                    <Columns>
                        <asp:TemplateColumn ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <input title="Select Email Address - <%#DataBinder.Eval(Container.DataItem, "email")%>"
                                    type="checkbox" name='chk_email_<%#DataBinder.Eval(Container.DataItem, "id")%>'
                                    id='chk_email_<%#DataBinder.Eval(Container.DataItem, "id")%>' value='<%#DataBinder.Eval(Container.DataItem, "id")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:HyperLinkColumn DataTextField="Email" DataNavigateUrlField="id" DataNavigateUrlFormatString="javascript:ektb_show('','subscriptionemailfromlist.aspx?action=edit&id={0}&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);">
                        </asp:HyperLinkColumn>
                        <asp:HyperLinkColumn DataTextField="id" DataNavigateUrlField="id" DataNavigateUrlFormatString="javascript:ektb_show('','subscriptionemailfromlist.aspx?action=edit&id={0}&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);">
                        </asp:HyperLinkColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </div>
    </div>
    </form>
    <style type="text/css">
        .Menu{
            margin-top: 9px !important;
        }
    </style>
</body>
</html>
