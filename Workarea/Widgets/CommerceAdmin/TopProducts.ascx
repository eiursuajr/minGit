<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopProducts.ascx.cs" Inherits="Workarea_Widgets_TopProducts" %>
<div style="display: none;">
    <script type="text/javascript">
        Ektron.ready(function () {
            $ektron("div.EktronPersonalization div.widget > div.content").show();
        });
    </script>
</div>
<div>
    <asp:Literal ID="ltr_period" runat="server" />
</div>
<div class="ektronTopSpace">
    <asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server">
        <asp:Literal ID="ltrlNoRecords" runat="server" /></asp:Label>
</div>
<asp:HiddenField ID="hdn_quantity" runat="server" />
<asp:HiddenField ID="hdn_filter" runat="server" />
<asp:Literal ID="ltr_periodMenu" runat="server" />

<div class="periodMenu" id="ChangeQuantityBlock_7021A0C6CFAC4BCFA7248E92DF1B12E6"
    style="display: none;">
    <p>
        <span class="quantityLabel">
            <asp:Literal ID="ltrlMaxNum" runat="server" /></span>
        <input title="Enter Quantity here" type="text" class="quantityTexBox" id="quantityTexBox"
            runat="server" />
        <asp:Literal ID="litUpdateQuantity" runat="server" />
    </p>
</div>
<asp:Panel ID="pnlData" runat="server">
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" runat="server" Width="100%" AutoGenerateColumns="false"
            EnableViewState="False" GridLines="None" CssClass="ektronGrid ektronBorder" OnItemDataBound="HandleItemDataBound">
            <HeaderStyle CssClass="title-header" />
            <Columns>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <%#Util_GetIndex()%></ItemTemplate>
                </asp:TemplateColumn>
                <asp:HyperLinkColumn DataTextField="title" DataNavigateUrlField="ProductId" DataNavigateUrlFormatString="content.aspx?action=View&id={0}" />
                <asp:HyperLinkColumn DataTextField="TotalSold" DataNavigateUrlField="ProductId" DataNavigateUrlFormatString="Commerce/reporting/analytics.aspx?action=vieworder&id={0}" />
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalValue"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</asp:Panel>
