<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MySentFriends.aspx.cs" Inherits="MyWorkspace_MySentFriends" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Sent Friends</title>
    <style type="text/css">
		.friendsHeader {
			font-weight: bold;
			background-color: #dddddd;
			border-left: solid 1px white;
		}

		.friendsAltRowClass {
			background-color: #fafafa;
		}
		
		.friendsAvatar{
		    border: solid 1px #dddddd;
		}
	</style>

    <script type="text/javascript" language="javascript">
    function resetPostback()
	{
	    document.forms[0].isPostData.value = "";
	}
	
	function checkAll(obj){
	    if(obj){
		    for (var i=0;i<document.forms[0].elements.length;i++){
			    var e = document.forms[0].elements[i];
			    if (e.name=='req_selected_users'){
				    e.checked=obj.checked;
			    }
		    }
	    }
    }
    
    function searchuser()
    {
	    if(document.getElementById('txtSearch').value.indexOf('\"') != -1){
	        alert('remove all quote(s) then click search');
	        return false;
	    }
	    document.getElementById('isSearchPostData').value = '1';
	    document.forms[0].submit();
	    return true;
	}
	
	function CheckForReturn(e)
	{ 
	    var keynum;
        var keychar;

        if(window.event) // IE
        {
            keynum = e.keyCode
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }
        
        if( keynum == 13 ) {
            document.getElementById('btnSearch').focus();
        }
	}

   
    </script>

</head>
<body>
    <form id="form2" runat="server">
        <!-- asp_Literal ID="ltr_search" run_at="server"></asp_Literal -->
        <div id="block_resendUI" style="display: none;">
            <div style="margin: 20px;">
                <div style="margin-left: 20px;">
                    <asp:Literal ID="lit_optmsglbl" runat="server" ></asp:Literal></div>
                <div style="margin-left: 20px; margin-top: 5px; margin-bottom: 5px;">
                    <asp:TextBox ID="tb_optmsg" runat="server" Columns="30" Rows="5" TextMode="MultiLine" ></asp:TextBox>
                    <div style=" margin-left: 50px; margin-top: 5px; margin-bottom: 5px;">
                        <asp:Button ID="btn_resendInvites" runat="server"  OnClick="Click_ResendInvites"/>
                        <br />
                    </div>
                </div>
            </div>
            <input type="hidden" value="" id="inv1_inviteHdn" name="inv1_inviteHdn" />
        </div>
        <asp:Panel ID="pnl_viewall" runat="server">
            <asp:DataGrid ID="FriendGrid" runat="server" AutoGenerateColumns="False" BorderColor="#ffffff"
                BorderStyle="None" BackColor="#ffffff" Width="100%" OnItemDataBound="Grid_ItemDataBound"
                BorderWidth="0" AllowCustomPaging="True" PageSize="10" PagerStyle-Visible="False"
                EnableViewState="False">
                <AlternatingItemStyle CssClass="friendsAltRowClass " />
            </asp:DataGrid>
            <!-- asp_Literal ID="ltr_message" run_at="server"></asp_Literal -->
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
        </asp:Panel>
        <asp:Literal ID="ltr_js" runat="server"></asp:Literal>
        <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        <input type="hidden" runat="server" id="isDeleted" value="" name="isDeleted" />
        <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
        <input type="hidden" runat="server" id="groupID" />
    </form>
</body>
</html>

