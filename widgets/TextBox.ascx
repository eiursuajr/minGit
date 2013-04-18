<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextBox.ascx.cs" Inherits="Widgets_TextBox" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
            <asp:Label ID="TextLabel" runat="server"></asp:Label>
        </asp:View>
        <asp:View ID="Edit" runat="server">
         <div id="<%=ClientID%>_edit" class="LSWidget">
            <table style="width:99%;">
                <tr>
                    <td>
                        Text:
                    </td>
                    <td>
                        <asp:TextBox ID="TextTextBox" runat="server" style="width:95%" TextMode="MultiLine"> </asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td><asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></td>
                </tr>
            </table>
            </div>
        </asp:View>
    </asp:MultiView>
</div>
