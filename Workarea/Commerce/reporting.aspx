<%@ Page Language="C#" AutoEventWireup="true" CodeFile="reporting.aspx.cs" Inherits="Commerce_reporting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Most Recent</title>
</head>
<body onclick="MenuUtil.hide()" style="min-height:30em;height:auto;padding-bottom:15em;background-color:White;">
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="pnl_view" runat="Server" Visible="false">
            <table width="550" id="tblmain" runat="server">
                    <tr>
                        <td align="left" class="input-box-text" style="white-space:nowrap; width: 140px" valign="top">
                        </td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            &nbsp;</td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" class="input-box-text" valign="bottom" >
                        </td>
                    </tr>
                    <tr id="tr_id" runat="server">
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_id_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_id" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_customer_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_customer" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_created_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_created" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_required_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_required" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_completed_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_completed" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_orderstatus_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_orderstatus" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_ordertotal_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_ordertotal" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                    <tr>
                        <td align="left" style="white-space:nowrap; width: 140px" valign="top" class="input-box-text">
                            <asp:Literal ID="ltr_pipelinestage_lbl" runat="server"></asp:Literal>:</td>
                        <td align="left" style="white-space:nowrap; width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            <asp:Literal ID="ltr_pipelinestage" runat="server"></asp:Literal><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" >
                            </td>
                    </tr>
                </table>
            <table width="100%">
            <tr>
                <td colspan="2">
                    <div class="ektronPageGrid">
                        <asp:DataGrid ID="dg_orderparts"
                            runat="server"
                            AutoGenerateColumns="false"
                            Width="100%"
                            GridLines="None">
                            <HeaderStyle CssClass="title-header" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="Address">
                                    <ItemTemplate><%#Util_ShowAddress(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "ShipperId")))%></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="SubTotal" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "SubTotal")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="TaxRate" HeaderText="Tax Rate" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"/>
                                <asp:TemplateColumn HeaderText="Tax" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "TaxCharge")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Shipping" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "ShippingCharge")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Part Total" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "PartTotal")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </div>
                </td>
            </tr>
            <tr>
                <td valign="top">&#160;</td>
                <td>
                    <div class="ektronPageContainer ektronPageGrid">
                        <asp:DataGrid ID="dg_orderlines"
                            runat="server"
                            AutoGenerateColumns="false"
                            Width="100%"
                            CssClass="ektronForm">
                            <HeaderStyle CssClass="title-header" />
                            <Columns>
                                <asp:BoundColumn DataField="ProductName" HeaderText="Name"/>
                                <asp:TemplateColumn HeaderText="Each" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "PriceEach")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="Quantity" HeaderText="Quantity" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Total" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "PriceTotal")),defaultCurrency.CultureCode)%></ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </div>
                </td>
            </tr>
            </table>
            <br /><asp:Literal ID="ltr_orders" runat="server"></asp:Literal>
        </asp:Panel>
        <asp:Panel ID="pnl_viewall" runat="Server">
            <br />
            <div class="ektronPageGrid">
                <asp:DataGrid ID="dg_orders"
                    runat="server"
                    AutoGenerateColumns="false"
                    Width="100%"
                    CssClass="ektronForm">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="&#160;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate><input type="checkbox" id='chk_order_<%#DataBinder.Eval(Container.DataItem, "Id")%>' /></ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                        <asp:HyperLinkColumn DataTextField="DateCreated" HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                        <asp:BoundColumn DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "OrderTotal")),defaultCurrency.CultureCode)%></ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </asp:Panel>
    </div>
    </form>
</body>
</html>

