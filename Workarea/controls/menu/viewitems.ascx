<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewitems.ascx.cs" Inherits="viewmenuitems" %>

<script type="text/javascript">
    function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		top.notifyLanguageSwitch(inVal, -1);
		document.location = 'menu.aspx?action=viewcontent&menuid='+<%=MenuId%>+'&LangType=' + inVal ;
	}
	
	function TranslateMenu(MenuId, ParentId, LanguageId) {
		document.location = 'menu.aspx?action=add&menuid=' + MenuId + 
		'&LangType='+LanguageId+'&parentid=' + ParentId;
	}
	function addBaseMenu(menuID, parentID, ancestID, foldID, langID, backpage) {
		document.location = 'collections.aspx?action=AddTransMenu&nId=' + 
		menuID + '&backlang=<%=MenuLanguage%>&LangType=' + langID + 
		'&folderid=' + foldID + '&ancestorid=' + ancestID + '&parentid=' + parentID +
		'&back=' + escape(backpage);
	}
	
    function checkAll(ControlName,flag){
        if(flag==true){
	        var iChecked=0;
	        var iNotChecked=0;
	        for (var i=0;i<document.forms[0].elements.length;i++){
		        var e = document.forms[0].elements[i];
		        if (e.name==ControlName){
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
		        if (e.name==ControlName){
			        e.checked=document.forms[0].checkall.checked
		        }
	        }
        }
    }

    function IsSelected(ControlName){
        var userChecked=false;
        for (var i=0;i<document.forms[0].elements.length;i++){
            var e = document.forms[0].elements[i];
            if (e.name==ControlName && e.checked){
	            userChecked=true;
	            break;
            }
        }
        return userChecked;
    }

	function DeleteItem(op){
        if(op=='items'){
            if(!IsSelected('frm_content_ids')){
                alert('<%=m_strSelDelWarning%>');
                return false;
            }
            if(confirm("<%=m_strDelItemsConfirm %>")){
                document.getElementById("<%=submittedaction.ClientID %>").value="deleteitem";
                // get CSV list of checked items
                var checklist = document.forms[0].frm_content_ids;
                var encodedlist = "";
                if (typeof checklist.length == 'undefined') {
                    encodedlist = checklist.value + "." + "<%=MenuId %>";
                } else {
                    for (i=0;  i < checklist.length;  i++) {
                      if (checklist[i].checked) {
                        if (encodedlist != "")
                          encodedlist = encodedlist + ",";
                        encodedlist = encodedlist + checklist[i].value + "." + "<%=MenuId %>";
                      }
                    }
                }
                document.getElementById("<%=frm_item_ids.ClientID %>").value = encodedlist;
                document.forms[0].submit();
            }
        }
        else{
            if(confirm("<%=m_strDelConfirm%>")){
                document.getElementById("<%=submittedaction.ClientID %>").value="delete";
                document.forms[0].submit();
            }
        }
        return false;
    }
    
    function SelectButton(MyObj) {
    }

	function resetPostback(){
        document.forms[0].menu_isPostData.value = "";
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:GridView ID="MenuItemList" 
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
        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
</div>
                
<input type="hidden" runat="server" id="isPostData" value="true" />
<input type="hidden" runat="server" id="submittedaction" value="" />
<input type="hidden" runat="server" id="frm_item_ids" value="" />
<asp:Literal ID="litRefreshAccordion" runat="server" />

