<%@ Page Language="C#" AutoEventWireup="true" CodeFile="customers.aspx.cs" Inherits="Commerce_customers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Customers</title>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            function resetPostback() {
                document.forms[0].isPostData.value = "";
            }
            function resetCPostback() {
                document.forms["form1"].isCPostData.value = "";
            }
            function AvatarDialogInit(basketId, customerId)
            {
                setTimeout( function() 
                {
                    $ektron(document).find(".uxViewBasketIframe").attr("src", "customers.aspx?action=viewbasket&basketid=" + basketId + "&customerid=" + customerId + "&thickox=true&height=300&width=700&modal=true&EkTB_iframe=true");
                }, 0);
            }
            //--><!]]>
        </script>
        <style type="text/css">
            div.ektronPageHeader {padding-top: 0 !important;}
        </style>
    </head>
    <body onclick="MenuUtil.hide()">
        <form id="form1" runat="server">
        <div class="ektronPageContainer ektronPageTabbed">
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_view" runat="Server" Visible="false">
            <div class="tabContainerWrapper">
            <div class="tabContainer">
                <ul>
                    <li>
                        <a title="Properties" href="#dvProp">
                            <%=this.GetMessage("properties text")%>
                        </a>
                    </li>
                    <li>
                        <a title="Orders" href="#dvOrders">
                            <%=this.GetMessage("lbl orders")%>
                        </a>
                    </li>
                    <li>
                        <a title="Addresses" href="#dvAddress">
                            <%=this.GetMessage("lbl addresses")%>
                        </a>
                    </li>
                    <li>
                        <a title="Baskets" href="#dvBaskets">
                            <%=this.GetMessage("lbl baskets")%>
                        </a>
                    </li>
                </ul>
                <div id="dvProp">
                    <table id="tblmain" runat="server" class="ektronGrid">
                        <tr id="tr_id" runat="server">
                            <td class="label">
                                <asp:Literal ID="ltr_id_label" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_id" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_uname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_uname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_fname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_fname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_lname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_lname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_dname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_dname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_ordertotal_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_ordertotal" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_orderval_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_orderval" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_pervalue_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_pervalue" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvOrders">
                    <br />
                    <asp:DataGrid ID="dg_orders"
                        runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                            <asp:HyperLinkColumn DataTextField="DateCreated" HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                            <asp:BoundColumn DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "Currency.AlphaIsoCode")%>&nbsp;<%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "OrderTotal")),DataBinder.Eval(Container.DataItem, "Currency.CultureCode").ToString())%></ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_orders" runat="server" />
                </div>
                <div id="dvAddress">
                    <br />
                    <asp:DataGrid ID="dg_address"
                        Runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Id" ItemStyle-VerticalAlign="Top"><ItemTemplate><a title="ID" href='customers.aspx?action=viewaddress&id=<%#DataBinder.Eval(Container.DataItem, "id")%>&customerid=<%#m_iID%>'><%#DataBinder.Eval(Container.DataItem, "id")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Address">
                                <ItemTemplate><%#ShowName(DataBinder.Eval(Container.DataItem, "Name").ToString(),DataBinder.Eval(Container.DataItem, "Company").ToString())%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "AddressLine1"))%><br />
                                        <%#ShowOptionalLine(DataBinder.Eval(Container.DataItem, "AddressLine2").ToString())%>
                                        <%#(DataBinder.Eval(Container.DataItem, "City"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Region"))%> <%#(DataBinder.Eval(Container.DataItem, "PostalCode"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Country"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Phone"))%>
                                        </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Billing" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><asp:checkbox ToolTip="Billing" id='chk_address_b' runat="Server" Enabled="false" checked='<%#Util_IsDefaultBilling(Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Id")))%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Shipping" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><asp:checkbox ToolTip="Shipping" id='chk_address_s' runat="Server" Enabled="false" Checked='<%#Util_IsDefaultShipping(Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Id")))%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_address" runat="server" />
                </div>
                <div id="dvBaskets">
                    <br />
                    <asp:DataGrid ID="dg_baskets"
                        runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Basket Id" ItemStyle-VerticalAlign="Top"><ItemTemplate><a title="Basket Id" href="#" onclick="$ektron('<%#uxDialog.Selector%>').dialog('open');AvatarDialogInit(<%#DataBinder.Eval(Container.DataItem, "Id")%>, <%#m_iID%>);"><%#DataBinder.Eval(Container.DataItem, "Id")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Basket Name" ItemStyle-VerticalAlign="Top"><ItemTemplate><a title="Basket Name" href="#" onclick="$ektron('<%#uxDialog.Selector%>').dialog('open');AvatarDialogInit(<%#DataBinder.Eval(Container.DataItem, "Id")%>, <%#m_iID%>);"><%#DataBinder.Eval(Container.DataItem, "Name")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Sub Total" ItemStyle-VerticalAlign="Top"><ItemTemplate><label title="Sub Total" id="lbl_SubTotal"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "SubTotal")), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Active Basket" ItemStyle-VerticalAlign="Top"><ItemTemplate><label title="Active Basket" id="lbl_DefaultBasket"><%#DataBinder.Eval(Container.DataItem, "IsDefault")%></label></ItemTemplate></asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_baskets" runat="server" />
                </div>
            </div>
            </div>
            </asp:Panel>
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                <asp:DataGrid ID="dg_customers"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:HyperLinkColumn DataTextField="id" HeaderText="Id" DataNavigateUrlField="id" DataNavigateUrlFormatString="customers.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                        <asp:HyperLinkColumn DataTextField="userName" HeaderText="Name" DataNavigateUrlField="id" DataNavigateUrlFormatString="customers.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                        <asp:BoundColumn DataField="TotalOrders" HeaderText="Total Orders" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "TotalOrderValue")),defaultCurrency.CultureCode)%></ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Per Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate><%#FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "AverageOrderValue")),defaultCurrency.CultureCode)%></ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                    OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                    OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                    OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                    OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
            </asp:Panel>
            <asp:Panel CssClass="ektronPageInfo" ID="pnl_viewaddress" runat="Server" Visible="false">
                <table class="ektronForm">
                    <tr id="tr_address_id" runat="server">
                        <td class="label">
                            <asp:Literal ID="ltr_address_id_lbl" runat="server" />:</td>
                        <td>
                            <asp:Literal ID="ltr_address_id" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_name" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_name" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_company" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_company" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_line1" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_line1" runat="server" /><br />
                            <asp:textbox ID="txt_address_line2" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_city_lbl" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_city" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_postal" runat="server" />:</td>
                        <td>
                            <asp:TextBox ID="txt_address_postal" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_country" runat="server" />:</td>
                        <td>
                            <asp:DropDownList AutoPostBack="true" ID="drp_address_country" runat="server" OnSelectedIndexChanged="drp_address_country_ServerChange"/><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_region" runat="server" />:</td>
                        <td>
                            <asp:DropDownList ID="drp_address_region" runat="server"/><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_phone" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_phone" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_default_billing" runat="server" />:</td>
                        <td>
                            <asp:CheckBox ToolTip="Billing" ID="chk_default_billing" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_default_shipping" runat="server" />:</td>
                        <td>
                            <asp:CheckBox ToolTip="Shipping" ID="chk_default_shipping" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <ektronUI:Dialog ID="uxDialog" CssClass="EktronViewBasketUI" Width="700" Height="300" Modal="true" Title="View Basket" runat="server">
                 <ContentTemplate>
                    <iframe class="uxViewBasketIframe" frameborder="0" border="0"  ID="uxViewBasketIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
                 </ContentTemplate>
           </ektronUI:Dialog>
            <asp:Panel ID="pnl_viewbasket" runat="Server" CssClass="ektronPageGrid" Visible="false">
                <asp:DataGrid ID="dg_viewbasket"
                    runat="server"
                    AutoGenerateColumns="false"
                    Width="100%"
                    GridLines="None"
                    CssClass="ektronGrid">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="Item" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <label id="lbl_item">
                                    <%#DataBinder.Eval(Container.DataItem, "ProductTitle")%>

                                    <%#showconfig((Ektron.Cms.Commerce.KitConfigData)DataBinder.Eval(Container.DataItem, "configuration")) %>

                                    <%#showvariant((Ektron.Cms.Commerce.BasketVariantData)DataBinder.Eval(Container.DataItem, "variant"))%>
                                </label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="SKU" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#DataBinder.Eval(Container.DataItem,"ProductSku") %></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Quantity" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#DataBinder.Eval(Container.DataItem,"Quantity") %></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="MSRP" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "ListPrice")), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "SalePrice")), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "AdjustedTotal")), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:Literal ID="ltr_noitems"  runat="server" />
            </asp:Panel>
        </div>
        <asp:Literal ID="ltr_js" runat="server" />
                <input type="hidden" runat="server" id="isCPostData" value="false" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        </form>
    </body>
</html>

