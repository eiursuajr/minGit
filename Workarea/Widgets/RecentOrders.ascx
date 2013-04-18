<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RecentOrders.ascx.cs" Inherits="Workarea_Widgets_RecentOrders" %>

<asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<asp:Panel ID="pnlData" runat="server">
    <asp:LinkButton ToolTip="View All" id="lnkViewAll" runat="server"><asp:Literal id="ltrlViewAll" runat="server" /></asp:LinkButton>

    <div class="ektronTopSpace"></div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" 
            runat="server" 
            Width="100%"
            AutoGenerateColumns="false"         
            EnableViewState="False"
            GridLines="None"
            CssClass="ektronGrid ektronBorder"
            OnItemDataBound="HandleItemDataBound"
            >
            <HeaderStyle CssClass="title-header" />
            <Columns>
                <%--<asp:TemplateColumn HeaderText="&#160;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <input type="checkbox" id='chk_order_<%#DataBinder.Eval(Container.DataItem, "OrderId")%>' /></ItemTemplate>
                </asp:TemplateColumn>--%>
                <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="Commerce/fulfillment.aspx?action=vieworder&id={0}" />                
                <asp:HyperLinkColumn DataTextField="DateCreated" HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="Commerce/fulfillment.aspx?action=vieworder&id={0}" />
                <asp:BoundColumn DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"/>
                <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "Currency.AlphaIsoCode")%><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "OrderTotal"), DataBinder.Eval(Container.DataItem, "Currency.CultureCode"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</asp:Panel>