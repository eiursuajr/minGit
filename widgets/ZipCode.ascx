<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ZipCode.ascx.cs" Inherits="widgets_ZipCode" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
            <div>
                <table style="width:99%;">
                    <tr>
                        <td>
                            <asp:DropDownList ID="searchDropDown" runat="server">
                                <asp:ListItem Value="none">--Select--</asp:ListItem>
                                <asp:ListItem Value="zip">Zip Code</asp:ListItem>
                                <asp:ListItem Value="area">Area Code</asp:ListItem>
                                <asp:ListItem Value="state">State Abbreviation</asp:ListItem>
                                <asp:ListItem Value="city">City</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="searchValue" runat="server" Style="width: 95%"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="searchButton" runat="server" Text="Search" OnClick="searchButton_Click" />
                        </td>
                    </tr>
                </table>
                <div style="max-height: 300px; overflow: auto;">
                    <asp:Repeater ID="Repeater1" runat="server">
                        <HeaderTemplate>
                            <table>
                                <tr>
                                    <td>
                                        City</td>
                                    <td>
                                        State</td>
                                    <td>
                                        Area Code</td>
                                    <td>
                                        Zip Code</td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr onmouseover="this.style.fontWeight='bold';this.style.background='#BBBBBB';" onmouseout="this.style.fontWeight='normal';this.style.background='#FFFFFF';">
                                <td>
                                    <%# (Container.DataItem as ZipCodeTableElement).City %>
                                </td>
                                <td>
                                    <%# (Container.DataItem as ZipCodeTableElement).State %>
                                </td>
                                <td>
                                    <%# (Container.DataItem as ZipCodeTableElement).AreaCode %>
                                </td>
                                <td>
                                    <%# (Container.DataItem as ZipCodeTableElement).Zip %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <asp:Label ID="lblResponse" runat="server"><center>Searching may take a few seconds.</center></asp:Label>
            </div>
        </asp:View>
    </asp:MultiView>
</div>
