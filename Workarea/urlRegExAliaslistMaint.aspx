<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlRegExAliaslistMaint.aspx.cs"
    Inherits="Workarea_urlregexaliaslistmaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Url Aliasing - RegEx</title>
    <asp:literal id="StyleSheetJS" runat="server" />
    <script type="text/javascript">

	function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) {
			if (eval(Validate)) {
				$ektron("#txtSearch").clearInputLabel();
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
			$ektron("#txtSearch").clearInputLabel();
			document.forms[FormName].submit();
			return false;
		}
	}
	function ConfirmDelete() {
		var itemChecked = false ;
		if(document.forms.form1.deleteRegexId)
		{
		 if(document.forms.form1.deleteRegexId.length){
			for(var x=0;x<document.forms.form1.deleteRegexId.length;x++) {
				//alert(document.forms.frm_deletealiases.deletealiasid[x].checked) ;
				if(document.forms.form1.deleteRegexId[x].checked == true) {
					itemChecked = true ;
				}
			}
		} else {
			if(document.forms.form1.deleteRegexId.checked) { itemChecked = true ; }
		}
       }
		if(!itemChecked) {
			alert('<asp:Literal id="ltr_selItem" runat="server" />') ;
			return(false) ;
		}

		return(confirm('<asp:Literal id="ltr_selRegex" runat="server" />')) ;

	}
	function removeRegex(){
	    var dropDownBox = document.forms[0].siteSearchItem;
    var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
    document.location.href = location.pathname + "?" + "action=removealias&fId="+fId ;
    return false;
    }
    function addRegex(){

        var dropDownBox = document.forms[0].siteSearchItem;
    var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
    document.location.href = "urlregexaliasmaint.aspx?action=addregex&fId="+fId ;

    }
    function resetCPostback(){
    document.forms["form1"].isCPostData.value = "";
    }
     function searchuser()
     {
	    if(document.forms.form1.txtSearch.value.indexOf('\"')!=-1 || document.forms.form1.txtSearch.value.indexOf('\'')!=-1 ){
	        alert('remove all quote(s) then click search');
	        return false;
	    }
	    document.forms.form1.submit();
	    return true;
	}

	function clearCache()
	{
	    var dropDownBox=document.forms[0].siteSearchItem;
        var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
        document.location.href = "urlRegExaliaslistmaint.aspx?action=clear&fId="+fId ;

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
    <form id="form1" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
            <asp:GridView ID="CollectionListGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                EnableViewState="False"
                CssClass="ektronGrid"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:GridView>
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="First" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="Prev" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="Next" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="Last" />
            <input type="hidden" runat="server" id="isCPostData" value="false" />
        </div>
    </form>
</body>
</html>

