<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Inventory.aspx.cs" Inherits="Commerce_Inventory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inventory</title>

    <script type="text/javascript">
            <!--        //--><![CDATA[//><!--
        function resetPostback() {
            document.forms[0].isPostData.value = "";
        }
        function resetCPostback() {
            document.forms["form1"].isCPostData.value = "";
        }
        //--><!]]>
    </script>

</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
    <div class="ektronPageContainer">
    <asp:Panel ID="dg_inventoryview" CssClass="ektronPageInfo" runat="server" Visible="false">
            <table class="ektronGrid">
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_name_lbl" runat="server"></asp:Literal>:
                    </td>
                    <td class="value">
                        <strong>
                            <asp:Literal ID="ltr_name" runat="server"></asp:Literal></strong>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_unitsinstock_lbl" runat="server"></asp:Literal>:
                    </td>
                    <td class="value">
                        <strong>
                            <asp:Literal ID="ltr_unitsinstock" runat="server" />&nbsp;&nbsp;</strong>
                        <asp:DropDownList ID="unitsinstockdrp" runat="server">
                            <asp:ListItem Text="+" Value="Add"></asp:ListItem>
                            <asp:ListItem Text="-" Value="Remove"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="ltr_unitsinstocktext" Columns="49" MaxLength="7" Text="0" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_unitsinorder_lbl" runat="server"></asp:Literal>:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="ltr_unitsinorder" Columns="49" MaxLength="7" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_reorderlevel_lbl" runat="server"></asp:Literal>:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="ltr_reorderlevel" Columns="49" MaxLength="7" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_disabled_lbl" runat="server"></asp:Literal>:
                    </td>
                    <td class="value">
                        <asp:CheckBox ID="ltr_disabled" runat="server" />
                    </td>
                </tr>
            </table>
    </asp:Panel>
    <asp:Panel ID="dg_inventorylist" runat="server">      
            <asp:DataGrid ID="dg_inventorylines" runat="server" OnItemDataBound="Datagrid_Itembound"
                AutoGenerateColumns="false" Width="100%" GridLines="None" CssClass="ektronGrid">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:TemplateColumn>
                        <HeaderTemplate>
                            <%#Util_SortUrl("event title","name")%></HeaderTemplate>
                        <ItemTemplate>
                            <asp:Literal ID="productImage" runat="server" />
                            <a href="inventory.aspx?action=view&id=<%#DataBinder.Eval(Container.DataItem,"EntryID")%>">
                                <%#DataBinder.Eval(Container.DataItem,"EntryTitle")%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <HeaderTemplate>
                            <%#Util_SortUrl("lbl in stock", "unitsinstock")%></HeaderTemplate>
                        <ItemTemplate>
                            <%#(DataBinder.Eval(Container.DataItem, "UnitsInStock"))%></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <HeaderTemplate>
                            <%#Util_SortUrl("lbl on order", "unitsonorder")%></HeaderTemplate>
                        <ItemTemplate>
                            <%#(DataBinder.Eval(Container.DataItem, "UnitsOnOrder"))%></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <HeaderTemplate>
                            <%#Util_SortUrl("lbl reorder", "reorderlevel")%></HeaderTemplate>
                        <ItemTemplate>
                            <%#(DataBinder.Eval(Container.DataItem, "ReorderLevel"))%></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <HeaderTemplate><%#GetMessage("generic status")%></HeaderTemplate>
                        <ItemTemplate>
                             <%#Get_InventoryStatus((bool)DataBinder.Eval(Container.DataItem, "DisableEntryInventoryManagement"))%></ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
    </asp:Panel>
    <p class="pageLinks">
        <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage"
        Text="[First Page]" OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage"
        Text="[Previous Page]" OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage"
        Text="[Next Page]" OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage"
        Text="[Last Page]" OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
    <script type="text/javascript">
        <asp:Literal ID="ltr_js" runat="server" />
    </script>
    <input type="hidden" runat="server" id="isCPostData" value="false" />
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    </div>
    </form>
</body>
</html>
