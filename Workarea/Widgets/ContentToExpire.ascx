<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentToExpire.ascx.cs"
    Inherits="Workarea_Widgets_ContentToExpire" %>
<asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server"><asp:Literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<asp:Panel ID="pnlData" runat="server">
    <asp:LinkButton ToolTip="View All" ID="lnkViewAll" runat="server"><asp:Literal ID="ltrlViewAll" runat="server" /></asp:LinkButton>

    <div class="ektronTopSpace"></div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" 
            runat="server" 
            Width="100%" 
            AutoGenerateColumns="False"
            EnableViewState="False"
            GridLines="None"
            CssClass="ektronGrid ektronBorder">
            <HeaderStyle CssClass="title-header" />
        </asp:DataGrid>
    </div>
</asp:Panel>

<script language='javascript'>
    function PopUpWindow_Email(url, hWind, nWidth, nHeight, nScroll, nResize) {
        var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
        var popupwin = window.open(url, hWind, cToolBar);
        return popupwin;
    }

    function LoadEmailChildPage(userGrpId) {
            PopUpWindow_Email('blankredirect.aspx?email.aspx?' + userGrpId + "&override_ie=true", 'CMSEmail', 490, 500, 1, 1);
    }
</script>

