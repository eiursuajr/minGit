<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleGadget.ascx.cs" Inherits="Widgets_GoogleGadget" %>

<div id="<%= ClientID %>">
    <asp:Literal ID="err" Text="" runat="server"></asp:Literal>
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
           <div style="width:480px"> <asp:Label ID="lblData" runat="server"></asp:Label></div>
        </asp:View>
        <asp:View ID="Edit" runat="server">
          <div class="gadget">
            <h1>Gadgets</h1>
            <ul class="gadget-list">
            </ul>
            <asp:TextBox ID="tbData" TextMode="MultiLine" runat="server" style="width:95%"> </asp:TextBox>
              <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />&nbsp;&nbsp;
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
          </div>
        </asp:View>
    </asp:MultiView>
</div>
