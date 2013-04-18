<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Amount.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Type.Amount" %>
<div class="amount ektronPageInfo">
    <input type="hidden" class="currentControl" name="CouponAmount" value="Amount" />
    <input type="hidden" class="initialized" id="hdnInitialized" runat="server" value="false" />
    <table class="ektronGrid">
        <thead>
            <tr class="title-header">
                <th colspan="2"><asp:Literal ID="litAmountHeader" runat="server" /></th>
            </tr>
        </thead>
        <tbody>
            <tr class="amount">
                <asp:MultiView ID="mvAmount" runat="server">
                    <asp:View ID="vwViewAmount" runat="server">
                        <td class="label">
                            <asp:Literal ID="litViewAmountLabel" runat="server" />
                        </td>
                        <td>
                            <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                            <asp:Literal ID="litViewAmountValue" runat="server" />
                        </td>
                    </asp:View>
                    <asp:View ID="vwEditAmount" runat="server">
                        <td class="label">
                            <asp:Label ID="lblAmount" runat="server" AssociatedControlID="txtLeftOfDecimal" />
                        </td>
                        <td>
                            <span style="white-space:nowrap">
                                <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                                <asp:TextBox ID="txtLeftOfDecimal" CssClass="leftOfDecimal" runat="server" />
                                <span class="decimal">.</span>
                                <asp:TextBox ID="txtRightOfDecimal" CssClass="rightOfDecimal" runat="server" MaxLength="2" />
                            </span>
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
        </tbody>
    </table>
</div>
