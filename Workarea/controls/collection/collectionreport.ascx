<%@ Control Language="C#" AutoEventWireup="true" CodeFile="collectionreport.ascx.cs"
    Inherits="Workarea_controls_collection_collectionreport" %>
<form id="frmCollectionList" runat="server">
<div id="dhtmltooltip">
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server">
    </div>
    <div class="ektronToolbar" id="htmToolBar" runat="server">
    </div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <div class="heightFix">
        <asp:GridView ID="CollectionListGrid" runat="server" AutoGenerateColumns="False"
            EnableViewState="False" Width="100%" CssClass="ektronGrid" GridLines="None">
            <HeaderStyle CssClass="title-header" />
        </asp:GridView>
        <p class="pageLinks">
            <asp:Label ToolTip="Page" runat="server" ID="cPageLabel">Page</asp:Label>
            <asp:Label ID="cCurrentPage" CssClass="pageLinks" runat="server" />
            <asp:Label ToolTip="of" runat="server" ID="cOfLabel">of</asp:Label>
            <asp:Label ID="cTotalPages" CssClass="pageLinks" runat="server" />
        </p>
        <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="cFirstPage"
            Text="[First Page]" OnCommand="CollectionNavigationLink_Click" CommandName="First"
            OnClientClick="resetCPostback()" />
        <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="cPreviousPage"
            Text="[Previous Page]" OnCommand="CollectionNavigationLink_Click" CommandName="Prev"
            OnClientClick="resetCPostback()" />
        <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="cNextPage"
            Text="[Next Page]" OnCommand="CollectionNavigationLink_Click" CommandName="Next"
            OnClientClick="resetCPostback()" />
        <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="cLastPage"
            Text="[Last Page]" OnCommand="CollectionNavigationLink_Click" CommandName="Last"
            OnClientClick="resetCPostback()" />
        <input type="hidden" runat="server" id="isCPostData" name="isCPostData" class="isCPostData" value="true" />
        <input type="hidden" runat="server" id="isSearchPostData" name="isSearchPostData" class="isSearchPostData" value="" />
        <asp:Literal ID="litRefreshAccordion" runat="server" />
    </div>
</div>
</form>
