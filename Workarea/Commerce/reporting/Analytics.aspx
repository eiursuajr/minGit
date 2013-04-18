<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Analytics.aspx.cs" Inherits="Commerce_Reporting_Analytics" %>
<%@ Register TagPrefix="ektron" TagName="TimeLineChart" Src="../../controls/reports/TimeLineChart.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Analytics</title>
    <asp:literal id="ltr_js" runat="server"></asp:literal>
    <script language="javascript" type="text/javascript">
        function ToggleDiv(sDiv, overrd) 
        {
            var objcustom = document.getElementById(sDiv); 
            var bOverRide = (overrd != null); 
            if ((bOverRide && overrd) || (!bOverRide && objcustom.style.visibility == 'hidden')) 
            {
                //objcustom.style.position = '';
                objcustom.style.visibility = 'visible';
            } 
            else
            { 
                //objcustom.style.position = 'absolute'; 
                objcustom.style.visibility = 'hidden';
            }
        }
    </script>
</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="pnl_viewall" runat="Server">        
            <table width="100%">
                <tr>
                    <td><ektron:TimeLineChart id="TrendTimeLineChart" width="300px" height="150px" runat="server" /></td>
                    <td><asp:Literal ID="ltr_description" runat="server"/></td>
                </tr>
            </table>
            <asp:literal id="ltr_noOrders" runat="server"></asp:literal>
            <div class="ektronPageContainer ektronPageGrid">
                <asp:DataGrid ID="dg_cctypes" 
                    runat="server" 
                    AutoGenerateColumns="false" 
                    Width="100%"
                    GridLines="None"
                    CssClass="ektronGrid">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="../fulfillment.aspx?action=vieworder&id={0}"/>
                        <asp:HyperLinkColumn DataTextField="DateCreated" HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="../fulfillment.aspx?action=vieworder&id={0}"/>
                        <asp:TemplateColumn>
                            <HeaderTemplate>
                                <a title="Toggle Site" href="javascript: void(0);" onclick="ToggleDiv('dvSites');">Site</a><br />
                                <div id="dvSites" style="position: absolute; visibility: hidden; border: 1px solid black;
                                    background-color: white; padding: 4px;">
                                    <table>
                                        <asp:Literal ID="ltr_sites" runat="server"/>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Util_AddSite(Convert.ToString(DataBinder.Eval(Container.DataItem, "httphost")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Status">
                            <ItemTemplate>
                                <%#Util_ShowStatus((Ektron.Cms.Common.EkEnumeration.OrderStatus)DataBinder.Eval(Container.DataItem, "Status"))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Customer">
                            <ItemTemplate>
                                <%#Util_ShowCustomerType((Ektron.Cms.Common.EkEnumeration.CustomerType)DataBinder.Eval(Container.DataItem, "CustomerType"))%><br />
                                <%#Util_ShowCustomer((Ektron.Cms.Commerce.CustomerData)(DataBinder.Eval(Container.DataItem, "Customer")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>                            
                                <%#DataBinder.Eval(Container.DataItem, "Currency.AlphaIsoCode") + "&nbsp;" + Ektron.Cms.Common.EkFunctions.FormatCurrency(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "OrderTotal")),Convert.ToString(DataBinder.Eval(Container.DataItem, "Currency.CultureCode")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Coupon Used">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chk_couponused" Enabled="false" Checked='<%#DataBinder.Eval(Container.DataItem, "CouponUsed")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>                    
                    </Columns>
                </asp:DataGrid>
            </div>
        </asp:Panel>
    </div>
    </form>
</body>
</html>

