<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewAllTags.ascx.cs" Inherits="controls_Community_PersonalTags_ViewAllTags" %>
<%@ Reference Page="../../../Community/PersonalTags.aspx" %>

<script type="text/javascript">
	function resetPostback()
	{
	    document.getElementById("<asp:Literal ID='ltlIsPostDataId' runat='server'/>").value = "";
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>	
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="_dg" 
        AutoGenerateColumns="false"
        Width="100%"
        GridLines="None" 
        cssclass="ektronGrid"
        runat="server">
        <HeaderStyle CssClass="title-header" />	
    </asp:DataGrid>
    <p class="pageLinks">
        <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label runat="server" ID="OfLabel">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />

    <input type="hidden" id="PTagsCBHdn" name="PTagsSelCBHdn" value="" />
    <input type="hidden" runat="server" id="tags_isPostData" value="true" name="tags_isPostData" />
</div>

