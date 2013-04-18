<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addeditreply.aspx.cs" Inherits="threadeddisc_addeditreply" ValidateRequest="false"%>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Reply to Post</title>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
			    function CanNavigate() {
				    // Block navigation while this page loaded (called from top window-object):
				    return false;
			    }
                function CanShowNavTree() {
				    // Block displaying the navigation tree while this page loaded (called from top window-object):
				    return false;
			    }
              //--><!]]>
        </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltr_js" runat="server" />
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Topic"><asp:Literal ID="ltr_topic" runat="server" />:</td>
                    <td class="readOnlyValue"><asp:Literal ID="ltr_topic_data" runat="server" /></td>
                </tr>
                <tr id="tr_state" runat="Server">
                    <td class="label" title="State"><asp:Literal ID="ltr_state" runat="server" />:</td>
                    <td class="value"><asp:DropDownList ToolTip="Select State from Drop Down Menu" ID="drp_state" runat="server" /></td>
                </tr>
                <tr id="tr_createdBy" runat="server">
                    <td class="label" title="Created By"><asp:Literal ID="ltr_created_by" runat="server" /></td>
                    <td class="readOnlyValue"><asp:Literal ID="ltr_created_by_data" runat="server" /></td>
                </tr>               
                <tr id="tr_createdDate" runat="server">
                    <td class="label" title="Date Created"><asp:Literal ID="ltr_created" runat="server" /></td>
                    <td class="readOnlyValue"><asp:Literal ID="ltr_created_data" runat="server" /></td>
                </tr>         
                <tr>
                    <td class="label" title="Message"><asp:Literal ID="ltr_desc" runat="server" />:</td>
                    <td class="value"><asp:Panel ID="pnl_message_editor" runat="server" /></td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hdn_action" runat="server" />
        <asp:HiddenField ID="hdn_replyid" runat="server" />
        <asp:HiddenField ID="hdn_forumid" runat="server" />
        <asp:HiddenField ID="hdn_topicid" runat="server" />
    </form>
</body>
</html>

