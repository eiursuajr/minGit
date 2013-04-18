<%@ Control Language="C#" AutoEventWireup="true" CodeFile="rolemembermgr.ascx.cs"
    Inherits="rolemembermgr" %>

<script language="javascript" type="text/javascript">
function resetPostback(){
    document.forms[0].role_isPostData.value = "";
}
function submitform(){
    //document.forms[0].role_isPostData.value = "";
    $ektron("#txtSearch").clearInputLabel();
    document.forms[0].submit();
}
function searchuser(){
        if(document.forms[0].txtSearch.value.indexOf('\"')!=-1){
	        alert('remove all quote(s) then click search');
	        return false;
	    }

	    var spdata="<%=isSearchPostData.ClientID%>";
        var pdata="<%=isPostData.ClientID%>";
        document.getElementById(spdata).value = "1";
        document.getElementById(pdata).value="true";
	    submitform();
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

<div id="dhtmltooltip"></div>
<div class="ektronTitlebar" title="Manage Members For Role" id="txtTitleBar" runat="server"></div>
<div class="ektronToolbar" id="htmToolBar" runat="server"></div>

<div id="member_checkbox_wrapper">
    <div class="ektronPageContainer">
        <asp:DataGrid ID="RoleMemberGrid"
            runat="server"
            AutoGenerateColumns="False"
            Width="100%"
            AllowCustomPaging="True"
            PageSize="10"
            PagerStyle-Visible="False"
            EnableViewState="False"
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

<input type="hidden" id="Hidden1" name="groupMarker" value="true" />
<input type="hidden" id="manager_mode" name="manager_mode" value="" />
<input type="hidden" id="member_user_ids" name="member_user_ids" value="" />
<input type="hidden" id="member_group_ids" name="member_group_ids" value="" />
<input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
<asp:Literal ID="javascript_literal" runat="server" />

<script type="text/javascript">
//<!--
function submitAddMembers(){
    var modeObj = document.getElementById('manager_mode');
    if (typeof(modeObj) != 'undefined')
    {
        modeObj.value='add'
        //alert('add');
        if (0 == readCheckboxValues())
        {
            alert('<%=m_refMsg.GetMessage ("alert msg add sel user")%>');
        }
    }
    $ektron("#txtSearch").clearInputLabel();
    document.forms[0].submit();
}

function submitDropMembers(){
    var modeObj = document.getElementById('manager_mode');
    if (typeof(modeObj) != 'undefined')
	{
		modeObj.value='drop'
		//alert('drop');
		if (0 == readCheckboxValues())
		{
		    alert('<%=m_refMsg.GetMessage ("alert msg del sel user")%>');
		}
	}
	$ektron("#txtSearch").clearInputLabel();
    document.forms[0].submit();
}

function IsBrowserIE() {
  return (document.all ? true : false);
}
function validateObject(obj) {
    return ((obj != null) &&
        ((typeof(obj)).toLowerCase() != 'undefined') &&
        ((typeof(obj)).toLowerCase() != 'null'))
}

function readCheckboxValues() {
    var idx, tdObj, qtyElements, subElements;
    var qtySelected = 0;
    tdObj = document.getElementById('member_checkbox_wrapper');
    if (validateObject(tdObj)){
        if (IsBrowserIE()) {
            subElements = tdObj.all;
        } else {
            subElements = tdObj.getElementsByTagName('*');
        }
        qtyElements = subElements.length;
        for(idx = 0; idx < qtyElements; idx++ ) {
			if (subElements[idx].type == 'checkbox'){
				if (subElements[idx].checked){
					if (subElements[idx].name.indexOf('member_') >= 0){
						saveMemberId(subElements[idx].name);
						++qtySelected;
					}
				}
			}
		 }
	}
	return (qtySelected);
}

function saveMemberId(strFullName) {
	var memberId, bGroupFlag, idx, offset;
	var bGroupFlag = false;
	var strGroup = 'member_group_id';
	var strUser = 'member_user_id';
	var hiddenObj, oldValue;
	idx = strFullName.indexOf(strGroup);
	if (idx >= 0){
		bGroupFlag=true;
		offset = idx + strGroup.length;
	} else {
		idx = strFullName.indexOf(strUser);
		if (idx < 0){
			return;
		}
		offset = idx + strUser.length;
	}
	memberId = parseInt((strFullName.substring(offset)),10);
	if (bGroupFlag){
		hiddenObj = document.getElementById('member_group_ids');
	} else {
		hiddenObj = document.getElementById('member_user_ids');
	}
	if (validateObject(hiddenObj)){
		oldValue = hiddenObj.value;
		if (oldValue.length){
		oldValue += ",";
		}
		hiddenObj.value = oldValue + (memberId).toString();
	}
}

//-->
</script>

<input type="hidden" runat="server" id="Hidden2" value="true" name="isPostData" />
<input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
