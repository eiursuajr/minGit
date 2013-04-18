<%@ Control Language="C#" AutoEventWireup="true" Inherits="editgroups" CodeFile="editgroups.ascx.cs" %>

<asp:Literal ID="postbackpage" runat="server" />

<script type="text/javascript" >
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

    function checkAll(ControlName){
	    if(ControlName!=''){
		    var iChecked=0;
		    var iNotChecked=0;
		    for (var i=0;i<document.forms[0].elements.length;i++){
			    var e = document.forms[0].elements[i];
			    if (e.name=='selected_users'){
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
			    if (e.name=='selected_users'){
				    e.checked=document.forms[0].checkall.checked
			    }
		    }
	    }
    }

    function AddSelectedUsers(){
        var userChecked=false;
        for (var i=0;i<document.forms[0].elements.length;i++){
	        var e = document.forms[0].elements[i];
	        if (e.name=='selected_users' && e.checked){
		        userChecked=true;break;
	        }
        }
        if(!userChecked){
        alert('<%=m_refMsg.GetMessage("alt select one or more user(s) then click save button.")%>');
        return false;
        }
        if(confirm("<%= m_refMsg.GetMessage("add user to group confirmation") %>")){
	        document.forms[0].user_isSearchPostData.value = "";
	        document.forms[0].user_isAdded.value = "1";
            document.forms[0].submit();
            return true;
	    }else{
	        return false;
	    }
    }
	function resetPostback()
	{
	    document.forms[0].user_isPostData.value = "";
	}
	function searchuser(){
	    if(document.forms[0].txtSearch.value.indexOf('\"')!=-1){
	        alert('remove all quote(s) then click search');
	        return false;
	    }
	    document.forms[0].user_isSearchPostData.value = "1";
	    document.forms[0].user_isPostData.value="true";
	    $ektron("#txtSearch").clearInputLabel();
	    document.forms[0].submit();
	    return true;
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div id="TR_AddGroupDetail" class="ektronPageInfo" runat="server">
        <table class="ektronForm">
            <tr>
                <td class="label" title="Group Name"><%=m_refMsg.GetMessage("user group name label")%></td>
                <td class="value"><input title="Group Name Text" onkeypress="javascript:return CheckKeyValue(event,'34');" type="text" maxlength="255" size="50" name="UserGroupName" id="UserGroupName" runat="server" /></td>
            </tr>
            <tr id="TR_desc" runat="server">
                <td class="label" title="Group Description"><%=m_refMsg.GetMessage("description label")%></td>
                <td class="value"><input title="Group Description Text" type="text" maxlength="75" size="50" name="group_description" id="group_description" runat="server" /></td>
            </tr>
        </table>
    </div>

    <div id="TR_label" runat="server">
        <div id="TD_label" runat="server"></div>
    </div>

    <div id="TR_AddGroup" runat="server">
        <div class="ektronPageGrid">
            <asp:DataGrid ID="AddGroupGrid"
                Width="100%"
                AutoGenerateColumns="False"
                OnItemDataBound="Grid_ItemDataBound"
                runat="server"
                CssClass="ektronGrid"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
        </div>
    </div>

    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    <input type="hidden" runat="server" id="isAdded" value="" name="isAdded" />
    <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
    <input onkeypress="javascript:return CheckKeyValue(event,'34');" type="hidden" name="netscape" />
    <input type="hidden" id="addgroupcount" name="addgroupcount" value="0" runat="server" />
</div>
