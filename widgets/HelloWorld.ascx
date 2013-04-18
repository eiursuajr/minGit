<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HelloWorld.ascx.cs" Inherits="widgets_HelloWorld" %>

    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
            <!-- You Need To Do ..............................  -->
            <asp:Label ID="OutputLabel" runat="server"></asp:Label>
            <!-- End To Do ..............................  -->
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit">
                 <!-- You Need To Do ..............................  -->
                 <asp:TextBox ID="HelloTextBox" runat="server" Style="width: 95%"> </asp:TextBox>
                  <!-- End To Do ..............................  -->
                 <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            </div>
        </asp:View>
        
    </asp:MultiView>

