<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyFavorites.aspx.cs" Inherits="MyWorkspace_MyFavorites" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Favorites</title>
    <style type="text/css">
    .ekfrpanel
    {
        position:absolute;
        top:23px;
        left:0px;
	    border: 1px solid black; 
	    background-color:white; 
	    z-index:99;
	    width:100%;
	    text-align:left;
    }
    </style>
    <script type="text/javascript" language="javascript">
    function resetPostback()
	{
	    document.forms[0].isPostData.value = "";
	}
	function toggleVisibility(itm)
    {
      var itmref = document.getElementById(itm);
      if (itmref != null && itmref.style.display == 'none')
      {
	    document.getElementById(itm).style.display = '';
      }
      else
      {
	    document.getElementById(itm).style.display = 'none';
      }
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

    function ismaxlength(obj, key, ty){
        var mlength=obj.getAttribute? parseInt(obj.getAttribute("maxlength")) : 50;
        var spanobj = document.getElementById('ek_fcharspan' + key);
        if (spanobj != null) 
        { 
            
            if (ty == 'up') { obj.value = obj.value.substring(0, mlength); }
            spanobj.innerHTML = obj.value.length + '/' + mlength;
        }
    }
   
	</script>
</head>
<body>
    <form id="form2" runat="server">
    <asp:Literal ID="ltr_search" runat="server"></asp:Literal>
    <asp:Panel id="pnl_viewall" runat="server">
                <asp:DataGrid ID="FavGrid" runat="server" AutoGenerateColumns="False"
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

    <table id="myfavorites_addfolderpanel" class="ekfrpanel" style="display:none;"> 
    <tr>
    <td>
        <table>
          <tbody> 
            <tr> 
              <td><asp:Literal ID="ltr_addfldrname" runat="server"></asp:Literal>:</td> 
              <td> 
                <input title="Enter Favorites here" type="text" id="myfavorites_fName" name="myfavorites_fName" onkeypress="return CheckKeyValue(event);" /> 
              </td> 
            </tr> 
            <tr> 
              <td><asp:Literal ID="ltr_addfldrdesc" runat="server"></asp:Literal>:</td> 
              <td> 
                <textarea name="myfavorites_fDesc" id="myfavorites_fDesc" cols="25" rows="4" maxlength="50" onkeyup="ismaxlength(this, '', 'up')" onkeydown="ismaxlength(this, '', 'down')" ></textarea><br/><span id="ek_fcharspan" class="ekfcharspan" ><asp:Literal id="ltr_fcharspan" runat="server"></asp:Literal></span>
              </td> 
            </tr> 
            <tr> 
              <td>&#160;</td> 
              <td> 
                <a title="Ok" id="AddFolder" href="#" onclick="CheckAddFolder(); return false;" >Ok</a>&#160;|&#160;<a title="Close" href="javascript:toggleVisibility('myfavorites_addfolderpanel');">Close</a>
              </td> 
            </tr> 
          </tbody> 
          </table>
    </td>
    </tr>
    </table> 
    <table id="myfavorites_movepanel" class="ekfrpanel" style="display:none;"> 
    <tr>
    <td>
        <table>
          <tbody> 
            <tr> 
              <td colspan="2"><asp:Literal ID="ltr_move" runat="server"></asp:Literal>:</td> 
            </tr> 
            <asp:Literal ID="ltr_moverows" runat="server"></asp:Literal>
            <tr> 
              <td>&#160;</td> 
              <td> 
                <a title="Ok" id="MoveItems" href="#" onclick="CheckMoveItems(); return false;" >Ok</a>&#160;|&#160;<a title="Close" href="javascript:toggleVisibility('myfavorites_movepanel');">Close</a>
              </td> 
            </tr> 
          </tbody> 
          </table>
    </td>
    </tr>
    </table>
    </form>
</body>
</html>

