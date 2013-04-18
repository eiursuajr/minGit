<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyGroups.aspx.cs" Inherits="MyWorkspace_MyGroups" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Groups</title>
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
      
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_search" runat="server"></asp:Literal>
    <asp:Panel id="pnl_viewall" runat="server">
                <asp:DataGrid ID="GroupGrid" runat="server" AutoGenerateColumns="False"
                BorderColor="#ffffff" BorderStyle="None" BackColor="#ffffff" Width="100%" OnItemDataBound="Grid_ItemDataBound"
                BorderWidth="0" AllowCustomPaging="True" PageSize="10" PagerStyle-Visible="False"
                EnableViewState="False" />
                <asp:Literal ID="ltr_message" runat="server"></asp:Literal>
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


