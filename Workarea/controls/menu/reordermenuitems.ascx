<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reordermenuitems.ascx.cs" Inherits="Workarea_controls_menu_reordermenuitems" %>
<form name="link_order" method="post" action="collections.aspx?action=DoUpdateMenuItemOrder&nid=<%=nId %>&iframe=<%=Request.QueryString["iframe"] %>">
<input type="hidden" id="frmfolderid" runat="server" name="frm_folder_id"/>
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table>
        <tr>
            <td>
                <select id="OrderList" runat="server" name="OrderList">
                </select>
            </td>
            <td>
                &nbsp;&nbsp;
            </td>
            <td>
                <a href="javascript:Move('up', document.getElementById('<%=OrderList.ClientID %>'), document.link_order.LinkOrder)">
                    <img id="UP" src="" runat="server" /></a>
                <br />
                <a href="javascript:Move('dn', document.getElementById('<%=OrderList.ClientID %>'), document.link_order.LinkOrder)">
                    <img id="DOWN" src="" runat="server"/></a>
            </td>
        </tr>
    </table>
</div>
<input type="hidden" name="LinkOrder" value="<%=reOrderList %>" />
<input type="hidden" name="navigationid" value="<%=nId %>" />
<input type="hidden" name="frm_back" value="<%=Request.QueryString["back"] %>" />
</form>
