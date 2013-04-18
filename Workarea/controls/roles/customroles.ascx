<%@ Control Language="C#" AutoEventWireup="true" CodeFile="customroles.ascx.cs" Inherits="customroles" %>

<asp:Literal ID="PostBackPage" Runat="server" />


<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div id="member_checkbox_wrapper">
	    <div class="ektronPageGrid">
	        <asp:DataGrid ID="CustomRoleListingGrid"
	            runat="server"
	            AutoGenerateColumns="False"
	            Width="100%"
	            EnableViewState="False"
	            CssClass="ektronGrid"
	            GridLines="None">
                <HeaderStyle CssClass="title-header" />
	        </asp:DataGrid>
	    </div>
    </div>

    <asp:Literal ID="Literal1" Runat="server" EnableViewState="False"/>

    <input type="hidden" id="manager_mode" name="manager_mode" value="" />
    <input type="hidden" id="role_names" name="role_names" value="" />
    <asp:Literal ID="javascript_literal" runat="server" />
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    function submitAddMembers(){
        var modeObj = document.getElementById('manager_mode');
        if (typeof(modeObj) != 'undefined')
        {
            modeObj.value='add'
            //alert('add');
            moveUserValues();
        }
	    $ektron("#txtSearch").clearInputLabel();
        document.forms[0].submit();
    }

    function submitdeletecustomrole(){
        var modeObj = document.getElementById('manager_mode');
        if (typeof(modeObj) != 'undefined')
        {
            modeObj.value='drop'
            //alert('drop');
            readCheckboxValues();
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
						    saveMemberName(subElements[idx].name);
					    }
				    }
			    }
		     }
	    }
    }

    function saveMemberName(strFullName) {
	    var memberName, bGroupFlag, idx, offset;
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
	    memberName = strFullName.substring(offset);
	    //if (bGroupFlag){
	    //	hiddenObj = document.getElementById('member_group_ids');
	    //} else {
		    hiddenObj = document.getElementById('role_names');
	    //}
	    if (validateObject(hiddenObj)){
		    oldValue = hiddenObj.value;
		    if (oldValue.length){
			    oldValue += ",";
		    }
		    hiddenObj.value = oldValue + (memberName).toString();
	    }
    }

    function moveUserValues() {
        var textObj, hiddenObj;
        textObj = document.getElementById('name_text');
        if (validateObject(textObj) && validateRoleName(textObj.value)) {

            hiddenObj = document.getElementById('role_names');
            if (validateObject(hiddenObj)) {
                hiddenObj.value = textObj.value;
            }
        }
    }

    function validateRoleName(roleName) {
        if (!validateObject(roleName) || roleName.length == 0) {
            alert('<%=m_refMsg.GetMessage("js: alert name cannot be null")%>');
            return false;
        }

        if (roleName.toString().indexOf('"') >= 0 || roleName.toString().indexOf("'") >= 0) {
            alert('<%=m_refMsg.GetMessage("js: alert name cannot include single or double quotes")%>');
            return false;
        }

        return true;
    }

    function searchuser()
    {
        if(document.forms[0].txtSearch.value.indexOf('\"')!=-1)
        {
            alert('remove all quote(s) then click search');
            return false;
        }
        var spdata="<%=isSearchPostData.ClientID%>";
        var pdata="<%=isPostData.ClientID%>";
        document.getElementById(spdata).value = "1";
        document.getElementById(pdata).value="true";
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
    //--><!]]>
    </script>
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
</div>
