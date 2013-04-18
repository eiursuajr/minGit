<%@ Page Language="C#" AutoEventWireup="true" CodeFile="groupmembers.aspx.cs" Inherits="Community_groupmembers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Community Group Members</title>
    <script type="text/javascript" language="javascript">
        function resetPostback()
	    {
	        document.forms[0].isPostData.value = "";
	    }

	    function checkAll(ControlName){
	        if(ControlName!=''){
		        var iChecked=0;
		        var iNotChecked=0;
		        for (var i=0;i<document.forms[0].elements.length;i++){
			        var e = document.forms[0].elements[i];
			        if (e.name=='req_deleted_users'){
				        if(e.checked){iChecked+=1;}
				        else{iNotChecked+=1;}
			        }
		        }
		        if(iNotChecked>0){document.forms[0].checkall.checked=false;}
		        else{document.forms[0].checkall.checked=true;}
	        }
	        else{
		        for (var i=0;i<document.forms[0].elements.length;i++){
			        var e = document.forms[0].elements[i];
			        if (e.name=='req_deleted_users'){
				        e.checked=document.forms[0].checkall.checked
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
	        $ektron("#txtSearch").clearInputLabel();
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

        function SubmitForm()
        {
	        document.forms[0].submit();
        }

        function ExecSearch() {
           var sTerm = $ektron('#txtSearch').getInputLabelValue();
           document.getElementById('isSearchPostData').value = true;
           $ektron('#txtSearch').clearInputLabel();
           document.forms[0].submit();
        }

        function resetPostback()
        {
           document.forms[0].isPostData.value = "";
        }

        function CheckDelete()
        {
            var bCheck = false;
            for (var i=0;i<document.forms[0].elements.length;i++)
            {
                var e = document.forms[0].elements[i];
                if (e.name=='req_deleted_users' && e.checked)
                {
                    bCheck = true;
                }
            }

            if (bCheck)
            {
                if (confirm('<asp:literal id="jsConfirmRemoveMemberFromGroup" runat="server" />'))
                {
                    document.getElementById('isDeleted').value = '1';
                    document.forms[0].submit();
                }
                else
                {
                    bCheck = false;
                }
            }
            else
            {
                alert('<asp:literal id="jsSelectAtLeastOneUser" runat="server" />');
            }
            return bCheck;
        }

        function CheckPendingDelete()
        {
            var bCheck = false;
            for (var i=0;i<document.forms[0].elements.length;i++)
            {
                var e = document.forms[0].elements[i];
                if (e.name=='req_deleted_users' && e.checked)
                {
                    bCheck = true;

                }
            }
            if (bCheck)
            {
                if (confirm('<asp:literal id="jsCancelRequest" runat="server" />'))
                {
                    document.getElementById('isDeleted').value = 'decline';
                    document.forms[0].submit(); } else { bCheck = false;
                }
            }
            else
            {
                alert('<asp:literal id="jsPleaseSelectUserRemove" runat="server" />');
            }
            return bCheck;
        }

        function CheckPendingApprove()
        {
            var bCheck = false;
            for (var i=0;i<document.forms[0].elements.length;i++)
            {
                var e = document.forms[0].elements[i];
                if (e.name=='req_deleted_users' && e.checked)
                {
                    bCheck = true;
                }
            }
            if (bCheck)
            {
                if (confirm('<asp:literal id="jsApproveSelectRequestsToJoin" runat="server" />'))
                {
                    document.getElementById('isDeleted').value = 'approve';
                    document.forms[0].submit(); } else { bCheck = false;
                }
            }
            else
            {
                alert('<asp:literal id="jsPleaseSelectAtLeastOneJoinRequest" runat="server" />');
            }
            return bCheck;
        }

        function CheckAdd()
        {
            var bCheck = false;
            for (var i=0;i<document.forms[0].elements.length;i++)
            {
                var e = document.forms[0].elements[i];
                if (e.name=='req_deleted_users' && e.checked)
                {
                    bCheck = true;
                }
            }
            if (bCheck)
            {
                document.getElementById('isDeleted').value = '1';
                document.forms[0].submit(); }
            else
            {
                alert('<asp:literal id="jsPleaseSelectUserToAdd" runat="server" />');
            }
            return bCheck;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel CssClass="ektronPageContainer ektronPageInfo" ID="pnl_viewall" runat="server">
            <asp:Literal ID="ltr_search" runat="server"/>
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li><a title="Current" href="#dvCurrent" runat="server" id="anchorCurrent">Current </a></li>
                        <li><a title="Pending" href="#dvPending" runat="server" id="anchorPending">Pending </a></li>
                    </ul>
                    <div id="dvCurrent">
                        <asp:DataGrid ID="MemberGrid" 
                            runat="server" 
                            AutoGenerateColumns="False" 
                            Width="100%"
                            OnItemDataBound="Grid_ItemDataBound" 
                            AllowCustomPaging="True" 
                            PageSize="10" 
                            PagerStyle-Visible="False"
                            EnableViewState="False" 
                            GridLines="None" 
                            CssClass="ektronGrid"
                            >
                            <HeaderStyle CssClass="title-header" />
                        </asp:DataGrid>
                        <asp:Literal ID="ltr_message" runat="server" />
                        <p class="pageLinks">
                            <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                            <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server">1</asp:Label>
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
                    </div>
                    <div id="dvPending" style="display:none !important"></div>
                </div>
            </div>
        </asp:Panel>

        <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        <input type="hidden" runat="server" id="isDeleted" value="" name="isDeleted" />
        <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
        <input type="hidden" runat="server" id="groupID" />
    </form>
</body>
</html>

