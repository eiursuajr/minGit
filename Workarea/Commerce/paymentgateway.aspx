<%@ Page Language="C#" AutoEventWireup="true" CodeFile="paymentgateway.aspx.cs" Inherits="Commerce_paymentgateway" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Payment Gateway</title>
        <script type="text/javascript">
            function resetPostback()
            {
                document.forms[0].isPostData.value = "";
            }
        </script>
        <asp:literal id="ltr_js" runat="server" />
    </head>
    <body onclick="MenuUtil.hide()">
        <form id="form1" runat="server" style="margin-top:-1px;">
            <asp:Panel ID="pnl_viewall" runat="Server">
                <div class="ektronPageContainer ektronPageTabbed">
                    <div class="tabContainerWrapper">
                        <div class="tabContainer">
                            <ul>
                                <li>
                                    <a title="Payment Options" href="#dvOptions">
                                        <asp:Literal ID="litPaymentOptions" runat="server" />
                                    </a>
                                </li>
                                <asp:PlaceHolder ID="phGatewaysTab" runat="server" Visible="true">
                                    <li>
                                        <a title="Payment Gateways" href="#dvGateways">
                                            <asp:Literal ID="litPaymentGatways" runat="server" />
                                        </a>
                                    </li>
                                </asp:PlaceHolder>
                            </ul>
                            <div id="dvOptions">
                                <table class="ektronGrid">
                                    <tr>
	                                    <td class="label" title="PayPal"><%=GetMessage("lbl commerce payment option paypal")%>&#160;</td>
	                                    <td><asp:CheckBox ToolTip="Enable or Disabe PayPal as a Payment Method" ID="chk_paypal" runat="server" Enabled="false" /></td>
                                    </tr>
                                </table>
                            </div>
                            <asp:PlaceHolder ID="phGatewaysContent" runat="server" Visible="true">
                                <div id="dvGateways">
                                    <asp:DataGrid ID="dg_gateway"
                                    runat="server"
                                    AutoGenerateColumns="false"
                                    CssClass="ektronGrid"
                                    GridLines="None">
                                    <HeaderStyle CssClass="title-header" />
                                    <Columns>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderText="&#160;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate><input type="radio" name="radio_gateway" title="Select <%#DataBinder.Eval(Container.DataItem, "Name")%> as a Paymment Gateway" id="radio_gateway" value='<%#DataBinder.Eval(Container.DataItem, "id")%>' /></ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:HyperLinkColumn DataTextField="Name" HeaderStyle-CssClass="title-header" HeaderText="Gateway" DataNavigateUrlField="id" DataNavigateUrlFormatString="paymentgateway.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                                        <asp:BoundColumn DataField="id" HeaderStyle-CssClass="title-header" HeaderText="Id" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderText="Default" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate><asp:CheckBox ToolTip="Default Option" ID="chk_def" runat="server" Enabled="false" Checked='<%#(DataBinder.Eval(Container.DataItem, "isdefault"))%>' /></ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderText="Custom" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate><asp:CheckBox ToolTip="Custom Option" ID="chk_custom" runat="server" Enabled="false" Checked='<%#(DataBinder.Eval(Container.DataItem, "iscustom"))%>' /></ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderText="UserId" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate><%#(DataBinder.Eval(Container.DataItem, "userid"))%></ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate><asp:CheckBox ToolTip="Cards Option" ID="chk_cc" runat="server" Enabled="false" Checked='<%#(DataBinder.Eval(Container.DataItem, "AllowsCreditCardPayments"))%>' /></ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate><asp:CheckBox ToolTip="Checks Option" ID="chk_check" runat="server" Enabled="false" Checked='<%#(DataBinder.Eval(Container.DataItem, "AllowsCheckPayments"))%>' /></ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                </asp:DataGrid>
                                    <p class="pageLinks">
                                        <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                                        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                                        <asp:Label Tooltip="of" runat="server" ID="OfLabel">of</asp:Label>
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
                                    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_view" Cssclass="ektronPageInfo" runat="Server" Visible="false">
                <table id="tblmain" runat="server" class="ektronGrid">
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_name" runat="server" />:</td>
                        <td><asp:DropDownList ID="drp_GatewayName" runat="server" /></td>
                    </tr>
                    <tr id="tr_id" runat="server">
                        <td class="label"><asp:Literal ID="ltr_id" runat="server" />:</td>
                        <td><asp:Label ToolTip="Id" ID="lbl_id" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_default" runat="server" />:</td>
                        <td><asp:checkbox ToolTip="Default" ID="chk_default" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_uid" runat="server" />:</td>
                        <td><asp:TextBox ID="txt_uid" runat="server" Columns="50" MaxLength="150" /></td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_pwd" runat="server" />:</td>
                        <td>
                            <asp:TextBox ID="txt_viewpwd" runat="server" Columns="50" MaxLength="150" Enabled="false" />
                            <asp:TextBox ID="txt_pwd" runat="server" Columns="50" MaxLength="150" TextMode="Password" />
                            <br />
                            <br />
                            <asp:literal ID="ltr_showcustom" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table id="tbl_custom" style="visibility:hidden; position: absolute;">
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_spare1" runat="server" />:</td>
                                    <td><asp:TextBox ID="txt_spare1" runat="server" Columns="49" MaxLength="500" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_spare2" runat="server" />:</td>
                                    <td><asp:TextBox ID="txt_spare2" runat="server" Columns="49" MaxLength="500" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
		                <td class="label"><%=GetMessage("lbl commerce payment option cc")%>&#160;</td>
		                <td class="readOnlyValue"><asp:CheckBox ToolTip="Cards" ID="chk_cc" runat="server" /></td>
	                </tr>
	                <tr>
		                <td class="label"><%=GetMessage("lbl commerce payment option check")%>&#160;</td>
		                <td class="readOnlyValue"><asp:CheckBox ToolTip="Checks" ID="chk_check" runat="server" /></td>
	                </tr>
                </table>
            </asp:Panel>
        </form>
    </body>
</html>

