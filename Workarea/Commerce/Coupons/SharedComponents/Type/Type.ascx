<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Type.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Type.Type" %>
<div class="type ektronPageInfo">
    <input type="hidden" id="hdnTypeLocalizedStrings" runat="server" class="typeLocalizedStrings" />
    <input type="hidden" class="controlId" name="CouponType" value="<%= GetControlId() %>" />
    <input type="hidden" class="currentControl" name="CouponType" value="Type" />
    <input type="hidden" class="couponTypePublished" id="hdnCouponTypePublished" runat="server" name="CouponTypePublished" value="" />
    <input type="hidden" class="couponTypeUserChanged" id="hdnCouponTypeUserChanged" runat="server" name="CouponTypeUserChanged" value="false" />
    <input type="hidden" class="originalCouponCode" id="hdnOriginalCouponCode" name="OriginalCouponCode" value="" runat="server" />
    <table class="ektronGrid">
        <thead>
            <tr class="title-header">
                <th colspan="2"><asp:Literal ID="litCouponTypeHeader" runat="server" /></th>
            </tr>
        </thead>
        <tbody>
            <tr class="type">
            <td class="label"><asp:Literal ID="litTypeLabel" runat="server" /></td>
            <td class="content">
                <asp:MultiView ID="mvType" runat="server">
                    <asp:View ID="vwViewType" runat="server">
                        <asp:Literal ID="litViewTypeValue" runat="server" />
                    </asp:View>
                    <asp:View ID="vwEditType" runat="server">
                        <span class="typeChoice"><asp:RadioButton ID="rbAmount" runat="server" GroupName="couponTypes" Checked="true" /></span>
                        <span class="typeChoice"><asp:RadioButton ID="rbPercent" runat="server" GroupName="couponTypes" /></span>
                    </asp:View>
                </asp:MultiView>
            </td>
            </tr>
            <tr class="code">
                <asp:MultiView ID="mvCode" runat="server">
                    <asp:View ID="vwViewCode" runat="server">
                        <td class="label"><asp:Literal ID="litViewCodeLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewCodeValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditCode" runat="server">
                        <td class="label"><asp:Label ID="lblCode" runat="server" AssociatedControlID="txtCode" /></td>
                        <td class="content">
                            <asp:TextBox ID="txtCode" runat="server" CssClass="code" />
                            <input type="button" class="codeValidate" id="btnCodeValidate" runat="server" />
                            <span class="required">*</span>
                            <span class="codeValid"><asp:Literal ID="litCodeValidMessage" runat="server" /></span>
                            <span class="codeInvalid"><asp:Literal ID="litCodeInalidMessage" runat="server" /></span>
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="description">
                <asp:MultiView ID="mvDescription" runat="server">
                    <asp:View ID="vwViewDescription" runat="server">
                        <td class="label"><asp:Literal ID="litViewDescriptionLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewDescriptionValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditDescription" runat="server">
                        <td class="label"><asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" /></td>
                        <td><asp:TextBox ID="txtDescription" runat="server" CssClass="description" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="currency">
                <asp:MultiView ID="vwCurrency" runat="server">
                    <asp:View ID="vwViewCurrency" runat="server">
                    <td class="label"><asp:Literal ID="litViewCurrencyLabel" runat="server" /></td>
                    <td class="content"><asp:Literal ID="litViewCurrencyValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditCurrency" runat="server">
                    <td class="label"><asp:Label ID="lblCurrency" runat="server" AssociatedControlID="txtDescription" /></td>
                    <td class="content">
                        <span class="allCurrencies"><asp:Literal ID="litAllCurrencies" runat="server" /></span>
                        <asp:DropDownList ID="ddlCurrency" runat="server" OnInit="ddlCurrency_Init" CssClass="currencies" />
                    </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="status">
                <asp:MultiView ID="vwStatus" runat="server">
                    <asp:View ID="vwViewStatus" runat="server">
                    <td class="label"><asp:Literal ID="litViewStatusLabel" runat="server" /></td>
                    <td class="content"><asp:Literal ID="litViewStatusValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditStatus" runat="server">
                    <td class="label"><asp:Label ID="lblStatus" runat="server" AssociatedControlID="ddlStatus" /></td>
                    <td class="content"><asp:DropDownList ID="ddlStatus" runat="server" OnInit="ddlStatus_Init" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
        </tbody>
    </table>
    <p class="required"><asp:Literal ID="litRequiredFieldMessage" runat="server" /></p>
</div>