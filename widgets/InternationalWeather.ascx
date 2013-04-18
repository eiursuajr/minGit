<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InternationalWeather.ascx.cs" Inherits="InternationalWidgets_Weather" %>

<asp:MultiView ID="ViewSet" runat="server" >
    <asp:View ID="View" runat="server">        
        <asp:Label ID="lblData" runat="server"></asp:Label></asp:View>
    <asp:View ID="Edit" runat="server">
     <div id="<%=ClientID%>_edit">
        <table style="width:99%;">
            <tr>
                <td>
                    Street: 
                </td>
                <td>
                    <asp:TextBox ID="txtStreet" runat="server" style="width:95%"></asp:TextBox> 
                </td>
            </tr>
            <tr>
                <td>
                    City: 
                </td>
                <td>
                    <asp:TextBox ID="txtCity" runat="server" style="width:95%"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="validateStreet" ErrorMessage="Required !" ControlToValidate="txtCity" />
                </td>
            </tr>
            <tr>
                <td>
                    State: 
                </td>
                <td>
                    <asp:TextBox ID="txtState" runat="server" style="width:95%"></asp:TextBox>
               </td>
            </tr>
            <tr>
                <td>
                    Country: 
                </td>
                <td>
                    <asp:TextBox ID="txtCountry" runat="server" style="width:95%"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="validateCountry" ErrorMessage="Required !" ControlToValidate="txtCountry" />
                </td>
            </tr>
            <tr>
                <td>
                    Zip Code: 
                </td>
                <td>
                    <asp:TextBox ID="txtZipCode" runat="server" style="width:95%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="unitsDropDown" runat="server">                          
                          <asp:ListItem Value="Metric Units" Text="Metric Units"></asp:ListItem>
                          <asp:ListItem Value="English Units" Text="English Units"></asp:ListItem>                          
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:TextBox ID="hdnAddr" Visible="false" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        </div>
    </asp:View>
</asp:MultiView>