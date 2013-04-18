<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Spacer.ascx.cs" Inherits="widgets_Spacer" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <hr runat="server" id="hrBreak" />
        <div runat="server" id="divBreak"></div>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div class="spacer-properties">
            <asp:CheckBox ID="cbIsBreak" runat="server" style="display: block;" Text="Horizontal Break" />
            <label for="spacer-height">Height:</label>
            <asp:TextBox ID="tbHeight" runat="server" Text=""></asp:TextBox>
            <asp:DropDownList ID="ddlUnit" runat="server"></asp:DropDownList>
        </div>
        <div class="spacer-buttons">
            <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
        </div>
    </asp:View>
</asp:MultiView>