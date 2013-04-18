<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Weather.ascx.cs" Inherits="Widgets_Weather" %>

<asp:MultiView ID="ViewSet" runat="server" >
    <asp:View ID="View" runat="server">
        <asp:Label ID="lblData" runat="server"></asp:Label></asp:View>
    <asp:View ID="Edit" runat="server">
     <div id="<%=ClientID%>_edit">
        <table style="width:99%;">
            <tr>
                <td>
                    Zip Code: 
                </td>
                <td>
                    <asp:TextBox ID="tbData" runat="server" style="width:95%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                </td>
            </tr>
        </table>
        </div>
    </asp:View>
</asp:MultiView>