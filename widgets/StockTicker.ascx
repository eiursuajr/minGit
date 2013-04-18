<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StockTicker.ascx.cs" Inherits="Widgets_StockTickerWidget" %>
<div style="padding: 12px;">
    <link id="EktronStockTickerCSS" href="./Widgets/StockTicker.css" type="text/css"
        rel="stylesheet" />
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="Server">
            <asp:Label ID="lbData" runat="Server"></asp:Label>
            <asp:Repeater ID="TickerRepeater" runat="server">
                <ItemTemplate>
                    <div class="Stock">
                        <div id="TopBar<%# (Container.DataItem as StockQuote).Symbol %>" class="TopBar" onclick="if($ektron('#LowerPane<%# (Container.DataItem as StockQuote).Symbol %>').is(':hidden')){$ektron('#LowerPane<%# (Container.DataItem as StockQuote).Symbol %>').slideDown('slow');}else{$ektron('#LowerPane<%# (Container.DataItem as StockQuote).Symbol %>').slideUp('slow');}">
                            <%# (Container.DataItem as StockQuote).spanTag %>
                            <span class="Symbol">
                                <%# (Container.DataItem as StockQuote).Symbol %>
                            </span>
                            <%# (Container.DataItem as StockQuote).LastTradePrice %>
                            (
                            <%# (Container.DataItem as StockQuote).PriceChange %>
                            )</span></div>
                        <div id="LowerPane<%# ( Container.DataItem as StockQuote).Symbol %>" class="LowerPane"
                            style="display: none;">
                            Last Trade:
                            <%# (Container.DataItem as StockQuote).LastTradePrice.ToString() %>
                            <br />
                            Last Trade Time:
                            <%# (Container.DataItem as StockQuote).LastTradeTime.ToString() %>
                            <br />
                            Price Change:
                            <%# (Container.DataItem as StockQuote).PriceChange.ToString() %>
                            <br />
                            Day High Price:
                            <%# (Container.DataItem as StockQuote).DayHighPrice.ToString() %>
                            <br />
                            Day Low Price:
                            <%# (Container.DataItem as StockQuote).DayLowPrice.ToString() %>
                            <br />
                            Day Volume:
                            <%# (Container.DataItem as StockQuote).DayVolume.ToString() %>
                            <br />
                        </div>
                    </div>
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
                                <%#Container.DataItem as string %>
                            </td>
                            <td>
                                <asp:LinkButton ID="LinkButton1" runat="server">Remove</asp:LinkButton></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <table style="width:99%;">
                <tr>
                    <td>
                        Symbol:
                    </td>
                    <td>
                        <asp:TextBox ID="newSymbol" runat="Server" style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr><td>
                        <asp:Button ID="AddStockButton" runat="server" Text="Add Stock" OnClick="AddStockButton_Click" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
						<asp:Button ID="CancelButton" CssClass="STCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</div>
