<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleConversion.ascx.cs" Inherits="widgets_GoogleConversion" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Literal ID="litViewText" runat="server" Text="Google Conversion"></asp:Literal>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <asp:Image ID="imgConversion" runat="server" />
        <div class="google-conversion-script">
            <label for="converstion-script" class="google-green-label">Conversion Script:</label>
            <asp:TextBox ID="tbConversionScript" runat="server" TextMode="MultiLine" Height="116px" Width="100%"></asp:TextBox>
        </div>
        <div class="google-buttons">
            <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                onclick="btnCancel_Click" />
        </div>
    </asp:View>
</asp:MultiView>