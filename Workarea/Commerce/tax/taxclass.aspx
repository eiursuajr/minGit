<%@ Page Language="C#" AutoEventWireup="true" CodeFile="taxclass.aspx.cs" Inherits="Commerce_tax_taxclass" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Tax Classes</title>
        <asp:literal id="ltr_js" runat="server"/>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            function resetPostback()
            {
                document.forms[0].isPostData.value = "";
            }
            //--><!]]>
        </script>        
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="ektronPageContainer">
                <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                    <asp:DataGrid ID="dg_viewall"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="ektronGrid"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:HyperLinkColumn DataTextField="id" HeaderText="Id" DataNavigateUrlField="id" DataNavigateUrlFormatString="taxclass.aspx?action=view&id={0}"/>
                            <asp:HyperLinkColumn DataTextField="Name" HeaderText="Name" DataNavigateUrlField="id" DataNavigateUrlFormatString="taxclass.aspx?action=view&id={0}"/>
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
                <asp:Panel ID="pnl_view" runat="Server" Visible="false">
                    <div class="ektronPageInfo">
                        <table id="tblmain" runat="server" class="ektronGrid">
                            <tr>
                                <td class="label"><asp:Literal ID="ltr_name" runat="server"/>:</td>
                                <td><asp:TextBox ID="txt_name" runat="server" MaxLength="25"/></td>
                            </tr>
                            <tr id="tr_id" runat="server">
                                <td class="label"><asp:Literal ID="ltr_id" runat="server"/>:</td>
                                <td><asp:Label ID="lbl_id" runat="server"/></td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </div>
        </form>
    </body>
</html>

