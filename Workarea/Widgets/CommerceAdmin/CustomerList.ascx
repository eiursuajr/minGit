<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerList.ascx.cs"
    Inherits="Workarea_Widgets_CustomerList" %>
<div>
    <asp:Literal ID="ltr_period" runat="server" />
    <asp:Literal ID="ltr_periodMenu" runat="server" />
    <%--<ul id="periodMenu0AC4DD881609474B8F3D10B1595C5B6D" class="periodMenu">
        <li><a title="Recent" href="#mostrecent">
            <asp:Literal ID='litLabelMostRecentOrders' runat='server' /></a></li>
        <li><a title="Valuable" href="#mostvaluable">
            <asp:Literal ID='litLabelMostValuable' runat='server' /></a></li>
        <li><a title="Active" href="#mostactive">
            <asp:Literal ID='litLabelMostActive' runat='server' /></a></li>
    </ul>--%>
</div>
<div class="ektronTopSpace">
    <asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server">
        <asp:Literal ID="ltrlNoRecords" runat="server" /></asp:Label>
</div>
<asp:HiddenField ID="hdn_filter" runat="server" />
<asp:Panel ID="pnlData" runat="server">
    <div class="ektronTopSpace">
    </div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="grdData" runat="server" Width="100%" AutoGenerateColumns="false"
            EnableViewState="False" GridLines="None" CssClass="ektronGrid ektronBorder" OnItemDataBound="HandleItemDataBound">
            <HeaderStyle CssClass="title-header" />
            <Columns>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <%#Util_GetIndex()%></ItemTemplate>
                </asp:TemplateColumn>
                <asp:HyperLinkColumn DataTextField="DisplayName" DataNavigateUrlField="Id" DataNavigateUrlFormatString="Commerce/customers.aspx?action=view&id={0}" />
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "TotalOrders") %>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalOrderValue"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%#Util_FormatDate(DataBinder.Eval(Container.DataItem, "DateCreated"))%>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</asp:Panel>
