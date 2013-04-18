<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmbedHTML.ascx.cs" Inherits="widgets_EmbedHTML" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        
        <asp:View ID="View" runat="server">         
           <asp:Label ID="TextLabel" runat="server"></asp:Label><br />
        </asp:View>
        
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>">
                  
               Embed HTML code:<br />
                <asp:TextBox ID="TextTextBox" TextMode="MultiLine" runat="server" style="width:99%;max-width:300px"> </asp:TextBox>
               <br /> Remove fixed size <asp:CheckBox ID="RemovFixSizeCheckBox" runat="server" Checked="true" />
                <br /><br />
                 <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            </div>
        </asp:View>
        
    </asp:MultiView>
</div>
