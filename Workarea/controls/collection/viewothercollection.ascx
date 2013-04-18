<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewothercollection.ascx.cs"
    Inherits="Workarea_controls_collection_viewothercollection" %>
<div id="divError" runat="server" visible="false">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="View Collections in Folder">
            <asp:Literal ID="litErrorViewCollectionTitle" runat="server"></asp:Literal>
        </div>
        <div class="ektronToolbar">
            <table>
                <tr>
                    <asp:Literal ID="litErrorButtons" runat="server"></asp:Literal>
                    <asp:Literal ID="litErrorHelp" runat="server"></asp:Literal>
                </tr>
            </table>
        </div>
    </div>
    <asp:Literal ID="litGenericError" runat="server"></asp:Literal>
    <div class="titlebar-error" id="titlebarerror" runat="server">
    </div>
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litCollectionTitle" runat="server"></asp:Literal>
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
<div class="ektronPageContainer ektronPageGrid">
    <table width="100%" class="ektronGrid">
        <tr class="title-header">
            <td width="30%">
                <a id="aGenericTitle" runat="server" href=""></a>
            </td>
            <td width="5%">
                <a id="aGenericId" runat="server" href=""></a>
            </td>
            <td>
                <a id="aGenericLast" runat="server"></a>
            </td>
            <td>
                <a id="aGenericURL" runat="server"></a>
            </td>
        </tr>
        <asp:Repeater ID="rptInfo" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <a href='<%# DataBinder.Eval(Container.DataItem, "CollectionLink") %>'>
                            <%# DataBinder.Eval(Container.DataItem, "CollectionTitle")%></a>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "CollectionID")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "DisplayLastEditDate")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "CollectionTemplate")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
