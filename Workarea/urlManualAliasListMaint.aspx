<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlManualAliasListMaint.aspx.cs" Inherits="Workarea_urlmanualaliaslistmaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
     <asp:literal id="StyleSheetJS" runat="server" />
	<script type="text/javascript">
	function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) 
		{
			if (eval(Validate)) 
			{
			    $ektron("#txtSearch").clearInputLabel();
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else 
		{
		    $ektron("#txtSearch").clearInputLabel();
			document.forms[FormName].submit();
			return false;
		}
	}
		function ConfirmDelete() {
		var itemChecked = false ;
		if(document.forms.form1.deleteAliasId)
		{
		 if(document.forms.form1.deleteAliasId.length){
			for(var x=0;x<document.forms.form1.deleteAliasId.length;x++) {
				//alert(document.forms.frm_deletealiases.deletealiasid[x].checked) ;
				if(document.forms[0].deleteAliasId[x].checked == true) {
					itemChecked = true ;
				}
			}
		} else {
			if(document.forms.form1.deleteAliasId.checked) { itemChecked = true ; }
		}
       }
		if(!itemChecked) {
			alert('<asp:Literal id="ltr_msgSelItem" runat="server" />') ;
			return(false) ;
		}

		return(confirm('<asp:Literal id="ltr_msgDelAlias" runat="server" />')) ;

	}
	function resetCPostback(){
    document.forms["form1"].isCPostData.value = "";
    }
    function removeAlias(mode){
    var dropDownBox=document.forms.form1.siteSearchItem;
    var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
    if(mode=="auto")
     {
        document.location.href = location.pathname + "?action=removealias&mode=auto&fId="+fId ;
     }
     else if(mode=="community")
     {
        document.location.href = location.pathname + "?action=removealias&mode=community&fId="+fId ;
     }
     else
     {
        document.location.href = location.pathname + "?action=removealias&fId="+fId ;
     }

    }
    function searchuser(){
	    if(document.forms.form1.txtSearch.value.indexOf('\"')!=-1 || document.forms.form1.txtSearch.value.indexOf('\'')!=-1)
	    {
	        alert('remove all quote(s) then click search');
	        return (false);
	    }
	    document.forms.form1.submit();
	    return(true);
	   
	    
	}
	function addAlias(langid,mode){

    var dropDownBox=document.forms.form1.siteSearchItem;
    var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
    if(mode=="auto")
     {
        document.location.href = "urlautoaliasmaint.aspx?action=addalias&&LangType="+ langid +"&fId="+fId ;
     }
     else if(mode=="community")
     {
        document.location.href = "urlcommunityaliasmaint.aspx?action=addalias&&LangType="+ langid +"&fId="+fId ;
     }
     else
     {
        document.location.href = "urlmanualaliasmaint.aspx?action=addalias&LangType=" + langid +"&fId="+fId;
     }
    }

	function clearCache(mode)
	{
	    var dropDownBox=document.forms.form1.siteSearchItem;
        var fId = dropDownBox.options[dropDownBox.selectedIndex].value;
        if(mode=="manual")
         {
            document.location.href = "urlmanualaliaslistmaint.aspx?action=refresh&fId="+fId ;
         }
         else if(mode=="community")
         {
          document.location.href = "urlmanualaliaslistmaint.aspx?mode=community&action=clearcommunitycache&fId="+fId ;
         }
         else
         {
          document.location.href = "urlmanualaliaslistmaint.aspx?mode=auto&action=clearcache&fId="+fId;
         }

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
	<%--Needed for stying GridView in FireFox--%>
	<style type="text/css">
    {
        white-space:normal !important;
     }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
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
					<asp:Label ToolTip="Page" runat="server" id="PageLabel" >Page</asp:Label>
					<asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
					<asp:Label ToolTip="of" runat="server" id="OfLabel" >of</asp:Label>
					<asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
				</p>
				<asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks"
				id="FirstPage" Text="[First Page]"
				OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="First" />
				<asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks"
				id="PreviousPage1" Text="[Previous Page]"
				OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="Prev" />
				<asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks"
				id="NextPage" Text="[Next Page]"
				OnCommand="NavigationLink_Click" OnClientClick="resetCPostback()" CommandName="Next" />
				<asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks"
				id="LastPage" Text="[Last Page]"
				OnCommand="NavigationLink_Click"  OnClientClick="resetCPostback()" CommandName="Last" />
		     <input type="hidden" runat="server" id="isCPostData" value="false" />
        </div>
    </form>
</body>
</html>

