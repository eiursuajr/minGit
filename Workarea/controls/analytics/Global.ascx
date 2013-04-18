<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Global.ascx.cs" Inherits="controls_analytics_Global" %>
<%@ Register TagPrefix="ektron" TagName="PercentPieChart" Src="../reports/PercentPieChart.ascx" %> 

<blockquote>
<div id="stats_aggr" style="width:50%" runat=server>
<table border="0" width="95%">
    <tr>
        <td>
            <asp:Label ID="lbl_total_hits" runat="server" Text="Total Page Views" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_total_hits" runat="server" Text="100"></asp:Label></td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lbl_total_visitors" runat="server" Text="Total Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_total_visitors" runat="server" Text="20"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_hits_per_visitor" runat="server" Text="Page Views/Visitor" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_hits_per_visitor" runat="server" Text="5"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_new_visitors" runat="server" Text="New Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_new_visitors" runat="server" Text="10"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_returning_visitors" runat="server" Text="Returning Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_returning_visitors" runat="server" Text="10"></asp:Label></td>
    </tr>
</table>
<table id="TABLE1" width="100%">
    <tr>
        <td>
            <asp:Label ID="lbl_hits_vs_visitors" runat="server" Text="Content Views vs. Visitors"
                    Font-Bold="True" />
                    <br />
            <ektron:PercentPieChart ID="graph_hits_per_visitor" Width="175px" Height="150px"
                Legend="BottomHorizontal" runat="server" Visible="true" />
        </td>
        <td>
            <asp:Label ID="lbl_new_vs_returning_visitors" runat="server" Text="New Vs. Returning Visitors"
                    Font-Bold="True" />
                    <br />
            <ektron:PercentPieChart ID="graph_new_vs_returning_visitors" Width="175px" Height="150px"
                Legend="BottomHorizontal" runat="server" Visible="true" />
        </td>
    </tr>
</table>
</div></blockquote>

<asp:Label ID="ByTimeGraph" runat="server" Text="Time Graph Here"></asp:Label>
<asp:Label ID="graph_key" runat="server"></asp:Label>
