<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reorderlinkscollection.ascx.cs" Inherits="Workarea_controls_collection_reorderlinkscollection" %>
<div class="ektronPageHeader" id="divErrors" runat="server" visible="false">
    <div class="ektronTitlebar" title="Reorder Collection">
        <asp:Literal ID="litReOrder" runat="server"></asp:Literal>
    </div>
    <div class="titlebar-error" id="divError" runat="server">
    </div>
</div>
<form name="link_order" method="post" action="collections.aspx?LangType=<%=ContentLanguage %>&action=DoUpdateOrder&nid=<%=nId%><%=bAction%>">
<input type="hidden" name="frm_folder_id" id="frm_folder_id" runat="server" />
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litReOrderTitle" runat="server"></asp:Literal>
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
    <div class="heightFix">
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
                        <img id="Up" runat="server" />
                    </a>
                    <br />
                    <a href="javascript:Move('dn', document.getElementById('<%=OrderList.ClientID %>'), document.link_order.LinkOrder)">
                        <img id="Down" runat="server" />
                    </a>
                </td>
            </tr>
        </table>
        <input type="hidden" name="LinkOrder" value="<%=reOrderList%>" />
        <input type="hidden" name="navigationid" value="<%=nId%>"/>
    </div>
</div>
</form>
