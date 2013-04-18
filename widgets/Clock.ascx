<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Clock.ascx.cs" Inherits="Widgets_ClockWidget" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Label ID="lblData" runat="server"></asp:Label>
    </asp:View>
    <asp:View ID="Edit" runat="server">
    <div id="<%=ClientID%>_edit">
        <table style="width:99%;">
            <tr>
                <td>
                    TimeZone:
                </td>
                <td>
                    <asp:DropDownList ID="DropDownList1" runat="server" >
                    </asp:DropDownList> 
                </td>
            </tr>
            <tr>
                <td>
                    Location (optional):</td>
                <td>
                    <asp:TextBox ID="clockTitleTextBox" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2">
                    Leave location blank for title to be selected item.
                    </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></td>
            </tr>
        </table>
        </div>
    </asp:View>
</asp:MultiView>