<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addlinkcollection.ascx.cs"
    Inherits="Workarea_controls_collection_addlinkcollection" %>
<script language="javascript" type="text/javascript">
      Ektron.ready(function(){ 
         $ektron("#isPostData").attr("value", "true");
      });
    function resetPostback()
    {
        $ektron("#isPostData").attr("value", "");
    }
</script>

<form name="selections" method="post" action="<%=ActionString %>" runat="server">
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litResult" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <div class="heightFix">
        <asp:DataGrid ID="ContentGrid" runat="server" OnItemDataBound="Grid_ItemDataBound"
            AutoGenerateColumns="False" Width="100%" GridLines="None" AllowPaging="False"
            AllowCustomPaging="True" PageSize="10" EnableViewState="False" PagerStyle-Visible="False">
        </asp:DataGrid>
        <p class="pageLinks">
            <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
            <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
            <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
            <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
        </p>
        <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage"
            Text="[First Page]" OnCommand="NavigationLink_Click" CommandName="First" />
        <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage"
            Text="[Previous Page]" OnCommand="NavigationLink_Click" CommandName="Prev" />
        <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage"
            Text="[Next Page]" OnCommand="NavigationLink_Click" CommandName="Next" />
        <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage"
            Text="[Last Page]" OnCommand="NavigationLink_Click" CommandName="Last" />
    </div>
</div>

<script type="text/javascript" language="javascript">
    Collections = "<%=(cLinkArray)%>";
    Folders = "<%=(fLinkArray)%>";
</script>

<input type="hidden" name="frm_content_ids" value="" />
<input type="hidden" name="frm_content_languages" value="" />
<input type="hidden" name="frm_folder_ids" value="" />
<input type="hidden" name="frm_back" id="frm_back" runat="server" />
<input type="hidden" name="hidCollectionID" id="hidCollectionID" runat="server" />
<asp:Literal ID="postbackaction" runat="server" />
<input type="hidden" id="isPostData" name="isPostData" class="isPostData" />
</form>
