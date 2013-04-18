<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MessageBoard.aspx.cs" Inherits="Workarea_Community_MessageBoard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>Untitled Page</title>

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        var jsConfirmDelete = '<asp:Literal runat="server" id="litDeleteMessage" />';
        function SetObjectType(obj)
        {
            $ektron("input#selVal")[0].value = obj.selectedIndex;
            document.forms[0].submit();
            return false;
        }
        function ShowMessage(obj)
        {
            $ektron("input#selVal")[0].value = obj.id;
            location.href = "messageboard.aspx?action=viewmessage&messageid=" + $ektron(obj).attr("msgid") + "&objecttype=" + $ektron(obj).attr("objType") + "&objectId=" + $ektron(obj).attr("objId") + "";            
            return false;
        }
        function GetMessagesToApprove()
        {
            var approveIds = GetSelectedMessages();
                        
            if( approveIds.length == 0 )
            {
                alert("<asp:Literal runat='server' id='ltrSelMsgToApprove' />");
                return false;
            }
            else
            {
                location.href = "messageboard.aspx?action=approvemessage&ids=" + approveIds;
            }
        }
        function SelectAll(obj)
        {
            var selectedIds = $ektron("input[name^=selMessage]");
            if(obj.checked == "checked" || obj.checked == true)
            {
                selectedIds.each(function(i)
                {
                    this.checked = "checked";
                });
            }
            else if(!(obj.checked == "checked" || obj.checked == true))
            {
                selectedIds.each(function(i)
                {
                    this.checked = "";
                });
            }
        }
        function ApproveMessage()
        {
            location.href = "messageboard.aspx?action=approvemessage&id=" + $ektron("input#hdnMessageInfo")[0].value ;
        }
        function DeleteSelectedMessages()
        {
            var deleteIds = GetSelectedMessages();
            if ( deleteIds.length == 0 )
            {
                alert("<asp:Literal runat='server' id='ltrSelMsgToDelete' />");
                return false;
            }
            else
            {
                if(confirm(jsConfirmDelete))
                {
                    location.href = "messageboard.aspx?action=deletemessage&ids=" + deleteIds;
                }
                else
                {
                    return false;
                }
            }
        }
        function GetSelectedMessages()
        {
            var messageIds = "";
            var selectedIds = $ektron("input[name^=selMessage]");
            selectedIds.each(function(i)
            {
                var currentId = $ektron(this);
                if (currentId.attr("checked") == true)
                {
                    messageIds += currentId.attr("msgid") + ",";
                }
            });
            return messageIds;
        }
        function DeleteMessage()
        {
            if(confirm(jsConfirmDelete))
            {
                location.href = "messageboard.aspx?action=deletemessage&id=" + $ektron("input#hdnMessageInfo")[0].value;
            }
            else
            {
                return false;
            }
        }
        function VerifySelection()
        {
            if ( $ektron("input#selectAll")[0].checked)
            {
                $ektron("input#selectAll")[0].checked = "";
            }
        }
        function resetPostback()
        {
            $ektron("input#isPostData").attr("value", "");
        }
	    //--><!]]>
    </script>
    <style type="text/css">
        div#ViewUnApprovedMessage blockquote
        {
            font-size: 1.2em;
            font-style: italic;
            margin: 0.5em 0 1em;
            padding: 0;
        }
        div#ViewUnApprovedMessage span.bqStart, span.bqEnd
        {
            color: #E1E7F2;
            float:left;
            font-family:Times New Roman,Serif;
            font-size: 500%;
            height: 45px;
            margin: -30px 6px -9px -10px;
            *margin: -25px -10px -50px -10px;
        }
        div#ViewUnApprovedMessage span.bqEnd
        {
            float:right;
            height: 25px;
            margin: -30px 0 0 -10px;
            *margin: -35px -15px 0 -10px;
        }
        div.commentWrapper
        {
            overflow: hidden;
            padding: 0 0.5em;
        }
        .replyTo
        {
            color: #486AC5;
            font-size: 1em;
            font-style: italic;
        }
    </style>
    <asp:literal id="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="messageBoard" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer">
            <asp:DataGrid ID="ViewUnApprovedMessages" 
                runat="server" 
                AllowPaging="false"
                AllowCustomPaging="true"
                EnableViewState="false"
                Width="100%" 
                AutoGenerateColumns="False"
                CssClass="ektronGrid" 
                GridLines="None">
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
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback();" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback();" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback();" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback();" />
            
            <input type="hidden" runat="server" id="selVal" value="0" name="selVal" />
            
            <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            <div id="ViewUnApprovedMessage" class="ektronPageInfo" runat="Server" visible="false">
                <asp:Literal runat="server" ID="ltrMessage" />
                <input type="hidden" id="hdnMessageInfo" name="hdnMessageInfo" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
