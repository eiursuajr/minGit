<%@ Page Language="C#" AutoEventWireup="true" CodeFile="shippingsource.aspx.cs"
    Inherits="Commerce_shipping_shippingsource" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Shipping Source</title>
        <asp:literal id="ltr_js" runat="server"/>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            function resetPostback()
            {
                document.forms[0].isPostData.value = "";
            }
            function resetCPostback()
		    {
                document.forms["form1"].isCPostData.value = "false";
            }
            //--><!]]>
        </script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
            form {position:relative;margin-top:-1px;}
            /*]]>*/-->
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
		<div class="ektronPageContainer">
            <div>
                <asp:Panel cssclass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                    <asp:DataGrid ID="dg_warehouse"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="ektronGrid"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                       <%-- <Columns><asp:BoundColumn DataField="id" HeaderText="Id"></asp:BoundColumn></Columns>--%>
                    </asp:DataGrid>
                </asp:Panel>
                <asp:Panel ID="pnl_viewaddress" runat="Server" Visible="false">
                    <div class="ektronPageInfo">
                        <asp:Literal ID="ltr_errdelete" runat="server"/>
                        <table class="ektronGrid">
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_name" runat="server" />:</td>
                                <td>
                                    <asp:TextBox ID="txt_address_name" runat="server" />
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="phAddressID" runat="server" Visible="true">
                                <tr>
                                    <td class="label">&#160;
                                        <asp:Literal ID="ltr_address_id" runat="server" />
                                        <asp:Label ID="lbl_colon" Visible="true" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="lbl_address_id" runat="server" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_line1" runat="server" />:</td>
                                <td>
                                    <asp:TextBox ID="txt_address_line1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_line2" runat="server" />:</td>
                                <td>
                                    <asp:TextBox ID="txt_address_line2" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_city_lbl" runat="server" />:</td>
                                <td>
                                    <asp:TextBox ID="txt_address_city" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_postal" runat="server"/>:</td>
                                <td>
                                    <asp:TextBox ID="txt_address_postal" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_country" runat="server" />:</td>
                                <td>
                                    <asp:DropDownList ID="drp_address_country" AutoPostBack="true" runat="server" />
                                </td>
                            </tr>                            
                             <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_address_region" runat="server" />:</td>
                                <td>
                                    <asp:ScriptManager ID="smAddressCountry" runat="server"/>                                    
                                    <asp:UpdatePanel ID="upAddressCountry" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drp_address_region" runat="server" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drp_address_country" />
                                        </Triggers>
                                    </asp:UpdatePanel>                                        
                                </td>
                            </tr>                            
                            <tr>
                                <td class="label">
                                    <asp:Literal ID="ltr_default_warehouse" runat="server" />:</td>
                                <td>
                                    <asp:CheckBox ID="chk_default_warehouse" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="Of" runat="server" ID="OfLabel">of</asp:Label>
                    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                    <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                    OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                    OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                    OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                    OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
            </div>
            <input type="hidden" runat="server" id="isCPostData" value="false" />
            <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
			</div>
        </form>
    </body>
</html>

