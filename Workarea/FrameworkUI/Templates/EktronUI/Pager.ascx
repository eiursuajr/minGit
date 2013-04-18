<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Pager.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.Pager" %>

<div class="ektron-ui-control ektron-ui-pager ektron-ui-clearfix" id="<%# this.Parent.ClientID %>">
    <span class="previous alignLeft">
        <ektronUI:Button ID="uxPrevious" PrimaryIcon="SeekPrevious" runat="server" OnClick="uxPrevious_Click" DisplayMode="Anchor" Text="<%$ Resources:PreviousText %>" />
    </span>
    <span class="page-buttons alignLeft">
        <asp:PlaceHolder ID="aspPages" runat="server"></asp:PlaceHolder>
    </span>
    <span class="next alignLeft">
        <ektronUI:Button ID="uxNext" runat="server" SecondaryIcon="SeekNext" OnClick="uxNext_Click" DisplayMode="Anchor" Text="<%$ Resources:NextText %>" />
    </span>
    <ektronUI:CssBlock ID="uxIE7Rules" runat="server" BrowserTarget="IE7">
        <CssTemplate>
            /* only float left for ie7 */
            .ektron-ui-pager .previous {padding-right:4px;}
            .ektron-ui-pager .alignLeft { float:left;}
            .ektron-ui-pager .next {padding-left:4px;}
        </CssTemplate>
    </ektronUI:CssBlock>
    <asp:HiddenField ID="aspCurrentPage" runat="server" Value='<%# Eval("CurrentPageIndex") %>' />
</div>