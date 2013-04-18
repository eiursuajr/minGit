<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Bookmarks.ascx.cs" Inherits="Widgets_Bookmarks" %>
<div style="padding: 12px;">

    
    <link id="EktronStockTickerCSS" href="./Widgets/StockTicker.css" type="text/css"
        rel="stylesheet" />
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="Server">
            <asp:Label ID="lbData" runat="Server"></asp:Label>
            <asp:Repeater ID="TickerRepeater" runat="server">
                <ItemTemplate>
                    <a href="<%# (Container.DataItem as LinkTitlePair).LinkHref %>" target="new">
                        <%# (Container.DataItem as LinkTitlePair).LinkTitle %>
                    </a>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <asp:PlaceHolder ID="SymbolTablePlaceHolder" runat="server"></asp:PlaceHolder>
            <table>
                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# (Container.DataItem as LinkTitlePair).LinkTitle %>
                            </td>
                            <td>
                                <asp:LinkButton ID="LinkButton1" runat="server">Remove</asp:LinkButton></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <table style="width:99%">
                <tr>
                    <td>
                        Title:
                    </td>
                    <td>
                        <asp:TextBox ID="newLinkTitle" runat="Server" style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        URL:</td>
                    <td>
                        <asp:TextBox ID="newLinkURL" runat="Server" style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
						<asp:Button ID="CancelButton" CssClass="BMCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        <asp:Button ID="AddBookmarkButton" runat="server" Text="Add Bookmark" OnClick="AddBookmarkButton_Click" />
                    </td>
                </tr>
            </table>
            <center>
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></center>
        </asp:View>
    </asp:MultiView>
</div>
