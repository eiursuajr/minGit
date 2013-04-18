<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewitems.ascx.cs" Inherits="viewitems" %>

<script type="text/javascript">
    function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		document.location = 'taxonomy.aspx?action=view&view=<%=m_strViewItem%>&taxonomyid='+<%=TaxonomyId%>+'&LangType=' + inVal ;
	}
	
	function TranslateTaxonomy(TaxonomyId, ParentId, LanguageId) {
		document.location = 'taxonomy.aspx?action=add&taxonomyid=' + TaxonomyId + '&LangType='+LanguageId+'&parentid=' + ParentId;
	}
	
	function DeleteItem(op){
	
        if(op=='items'){
            if(!IsSelected('selected_items')){
                alert('<%=m_strSelDelWarning%>');
                return false;
            }
            if(confirm("<%=m_strDelItemsConfirm %>")){
                document.getElementById("submittedaction").value="deleteitem";
                document.forms[0].submit();
            }
        }
        else{
            if(confirm("<%=m_strDelConfirm%>")){
                document.getElementById("submittedaction").value="delete";
                document.forms[0].submit();
            }
        }
        return false;
    }
    function LoadViewType(type){
	    document.location='taxonomy.aspx?iframe=true&action=view&view='+type+'&folderid=0&taxonomyid=<%=TaxonomyId%>&parentid=<%=TaxonomyParentId%>' ;
    }
	function resetPostback(){
        document.forms[0].taxonomy_isPostData.value = "";
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:GridView ID="TaxonomyItemList" 
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
                
<input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
