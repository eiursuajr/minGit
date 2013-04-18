<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IFrame.ascx.cs" Inherits="Widgets_IFrame" %>
<!--[if gt IE 6]> 
    <style>
    .hidableiframeproperty{
        display:none;
    }
    </style>
<![endif]--> 
<div>
  <asp:Label ID="lblNote" runat="server" ></asp:Label>
</div>
<div >
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <asp:Label ID="lbData" runat="Server"></asp:Label>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <table style="width:99%;">
                <tr>
                    <td>
                        URL:</td>
                    <td>
                        <asp:TextBox ID="IFrameURLTextBox" runat="Server" style="width:95%"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        Widget Title:</td>
                    <td>
                        <asp:TextBox ID="WidgetTitleTextBox" runat="Server" style="width:95%"></asp:TextBox></td>
                </tr> 
                <tr  >
                    <td>
                        Horizontal Scroll Bar:</td>
                    <td>
                        <asp:CheckBox ID="ScrollXCheckBox" runat="server" /></td>
                </tr>
                <tr  >
                    <td>
                        Vertical Scroll Bar:</td>
                    <td>
                        <asp:CheckBox ID="ScrollYCheckBox" runat="server" /></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td> <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></td>
                </tr>
                
            </table>
        </asp:View>
    </asp:MultiView>
</div>
