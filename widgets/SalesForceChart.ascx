<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SalesForceChart.ascx.cs" Inherits="Widgets_SalesForceChart" %>
<div class="EktronWidgetSalesForceChart">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <div id="divSalesForceChartData" runat="server">
                <asp:HyperLink ID="aSalesForceLink" runat="server" Visible="False">
                    <asp:Image ID="imgSalesForceChart" runat="server" Visible="False" />
                </asp:HyperLink>
            </div>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <table style="width:99%">
                <tr>
                    <td>Username:</td>
                    <td><asp:TextBox ID="txtUsername" runat="Server" style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td><asp:TextBox ID="txtPassword" runat="Server" TextMode="Password" style="width:95%;" ></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Chart Title:</td>
                    <td><asp:TextBox ID="txtChartTitle" runat="Server"  style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Chart Source:</td>
                    <td><asp:TextBox ID="txtChartURL" runat="Server" style="width:95%;" ></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Link To:</td>
                    <td><asp:TextBox ID="txtLinkURL" runat="Server"  style="width:95%;"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Button ID="CancelButton" CssClass="SFCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" /></td>
                    <td><asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" /></td>
                    
                </tr>
            </table>
        </asp:View>
        <asp:View ID="UnsupportedBrowser" runat="server">
            <h3>Browser Compatibility Error</h3>
            <p>The minimum browser requirements for this widget are:</p>
            <ul>
                <li>Microsoft Internet Explorer v7.0+</li>
                <li>Mozilla Firefox v3.0+</li>
                <li>Apple Safari v3.0+</li>
            </ul>
        </asp:View>
    </asp:MultiView>
</div>