<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleExperiment.ascx.cs" Inherits="widgets_GoogleExperiment" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Literal ID="litViewText" runat="server" Text="Google Experiment"></asp:Literal>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <asp:Image ID="imgControl" runat="server" />
        <div class="google-control-script">
            <label for="control-script" class="google-orange-label">Control Script:</label>
            <asp:TextBox ID="tbControlScript" runat="server" TextMode="MultiLine" Height="116px" Width="100%"></asp:TextBox>
        </div>
        <asp:Image ID="imgTracking" runat="server" />
        <div class="google-tracking-script">
            <label for="control-script" class="google-green-label">Tracking Script:</label>
            <asp:TextBox ID="tbTrackingScript" runat="server" TextMode="MultiLine" Height="116px" Width="100%"></asp:TextBox>
        </div>
        <div class="google-buttons">
            <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                onclick="btnCancel_Click" />
        </div>
    </asp:View>
</asp:MultiView>