<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Percent.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Type.Percent" %>
<div class="percent ektronPageInfo">
    <input type="hidden" class="percentLocalizedStrings" name="CouponPercent" value='<%= GetJavascriptLocalizedStrings() %>' />
    <input type="hidden" class="currentControl" name="CouponPercent" value="Percent" />
    <input type="hidden" class="initialized" id="hdnInitialized" runat="server" value="false" />
    <table class="ektronGrid">
        <colgroup>
            <col class="label" />
            <col class="data" />
        </colgroup>
        <thead>
            <tr class="title-header">
                <th colspan="2"><asp:Literal ID="litPercentHeader" runat="server" /></th>
            </tr>
        </thead>
        <tbody>
            <tr class="percent">
                <asp:MultiView ID="mvPercent" runat="server">
                    <asp:View ID="vwViewPercent" runat="server">
                        <td class="label">
                            <asp:Literal ID="litViewPercentLabel" runat="server" />
                        </td>
                        <td class="content">
                            <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                            <asp:Literal ID="litViewPercentValue" runat="server" />
                        </td>
                    </asp:View>
                    <asp:View ID="vwEditPercent" runat="server">
                        <td class="label">
                            <asp:Label ID="lblPercent" runat="server" AssociatedControlID="txtHundreds" />
                        </td>
                        <td class="content">
                            <span style="white-space:nowrap">
                                <asp:TextBox ID="txtHundreds" CssClass="hundreds" runat="server" MaxLength="3" />
                                <span class="decimal">.</span>
                                <asp:TextBox ID="txtHundredths" CssClass="hundredths" runat="server" MaxLength="2" />
                                <span class="percentSymbol" title="<%= GetPercentName() %>">%</span>
                            </span>
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="maxAmount stripe">
                <asp:MultiView ID="mvMaxAmount" runat="server">
                    <asp:View ID="vwViewMaxAmount" runat="server">
                        <td class="label">
                            <asp:Literal ID="litViewMaxAmountLabel" runat="server" />
                        </td>
                        <td class="content">
                            <span style="white-space:nowrap">
                                <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                                <asp:Literal ID="litViewMaxAmountValue" runat="server" />
                            </span>
                        </td>
                    </asp:View>
                    <asp:View ID="vwEditMaxAmount" runat="server">
                        <td class="label">
                            <asp:Label ID="lblMaxAmount" runat="server" AssociatedControlID="txtDollars" />
                        </td>
                        <td class="content">
                            <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                            <asp:TextBox ID="txtDollars" CssClass="dollars" runat="server" />
                            <span class="decimal">.</span>
                            <asp:TextBox ID="txtCents" CssClass="cents" runat="server" MaxLength="2" />
                            <div class="maxAmountMessage">
                                <p id="pMaxAmountMessage" runat="server"></p>
                                <ul class="additionalInformation">
                                    <li class="cart"><span id="spanMessageCart" runat="server"></span><span class="calculation"></span></li>
                                    <li class="item"><span id="spanMessageItems" runat="server"></span><span class="calculation"></span></li>
                                </ul>
                            </div>
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
        </tbody>
    </table>
</div>