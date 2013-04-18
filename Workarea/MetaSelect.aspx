<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MetaSelect.aspx.cs" Inherits="MetaSelect" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <style type="text/css">
        .action-bar-rollover-indicator
        {
            display: none;
        }
    </style>
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--		
		setTimeout(AdjustParentContainerSize, 100);
		function AdjustParentContainerSize()
		{
			if ((typeof(parent) != "undefined")
				&& (typeof(parent.ek_ma_AdjustContainerSize) != "undefined"))
			{
					parent.ek_ma_AdjustContainerSize(document.getElementById('ContainingCell').offsetHeight + 2);
			}
			
		}
		
		function IsBrowserIE() 
		{
			// document.all is an IE only property
			return (document.all ? true : false);
		}

		// Hides (or closes) the window:
		function closeChildPage() 
		{
			if ((typeof(parent) != "undefined") 
				&& (typeof(parent.ek_ma_CloseMetaChildPage) != "undefined"))
			{
					parent.ek_ma_CloseMetaChildPage();
			}
			else if (!IsBrowserIE())
			{
				window.close(); // For Netscape, this is running in a popup-window.
			}
		}
		
		// Called when the user clicks any hyperlinked title:
		function UpdateFormData(selectedId, title, metadataFormTagId) 
		{
			var parentWindow = window.parent;
			if (parentWindow && parentWindow.addMetaSelectRow)
			{
				parentWindow.addMetaSelectRow(selectedId, title, metadataFormTagId);
			}
		}
	

	    function searchuser(){
	        if(document.forms[0].txtSearch.value.indexOf('\"')!=-1){
	            alert('remove all quote(s) then click search');
	            return false;
	        }
	        document.forms[0].user_isSearchPostData.value = "1";
	        document.forms[0].user_isPostData.value="true";
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
</head>
  <body>
    <form id="Form1" method="post" runat="server">
		<table id="MetaSelectContainer" style="height:auto;width:100%" width="100%">
		    <tbody>
		        <tr>
		            <td id="ContainingCell">
		                <asp:Literal ID="Literal1" Runat="Server" EnableViewState="false"/>
		                <asp:HiddenField ID="SelectedId" runat="Server" value=""/>
		            </td>
		        </tr>
		        
		        
			    
		    </tbody>
		    </table>
		     <p class="pageLinks">
			    <asp:Label ToolTip="Page" runat="server" id="PageLabel">Page</asp:Label>
			    <asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
			    <asp:Label ToolTip="of" runat="server" id="OfLabel">of</asp:Label>
			    <asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
		      </p>
		     <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" id="FirstPage" Text="[First Page]" OnCommand="NavigationLink_Click"
			    CommandName="First" />
		        <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" id="lnkBtnPreviousPage" Text="[Previous Page]" OnCommand="NavigationLink_Click"
			    CommandName="Prev" />
		        <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" id="NextPage" Text="[Next Page]" OnCommand="NavigationLink_Click"
			    CommandName="Next" />
		         <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" id="LastPage" Text="[Last Page]" OnCommand="NavigationLink_Click"
			    CommandName="Last" />
		     
		<input type="hidden" id="user_isSearchPostData" value="" />
		<input type="hidden" id="user_isPostData" value="" />
	</form>
  </body>
</html>
