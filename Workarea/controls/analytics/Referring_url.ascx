<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Referring_url.ascx.cs" Inherits="controls_analytics_Referring_url" %>
<%@ Register TagPrefix="ektron" TagName="PercentPieChart" Src="../reports/PercentPieChart.ascx" %> 
<asp:Literal ID="navBar" runat="server"/>
<asp:Image ID="Image1" runat="server" />

<asp:GridView ID="GridView1" 
    runat="server" 
    AutoGenerateColumns="False" 
    Width="100%" 
    BorderColor="White"
    AllowPaging="True" 
    AllowSorting="True" 
    PageSize="3"  OnSorting="GridView1_Sorting" OnPageIndexChanging="GridView1_PageIndexChanging">
    <HeaderStyle CssClass="title-header" />
    <Columns>
        <asp:HyperLinkField HeaderText="Page" DataNavigateUrlFields="referring_url" DataTextField="referring_url" ShowHeader="False" SortExpression="referring_url" />
        <asp:BoundField DataField="referrals" HeaderText="Referrals" SortExpression="referrals" />
    </Columns>
</asp:GridView>

<asp:GridView ID="GridView2" 
    runat="server" 
    AutoGenerateColumns="False" 
    Width="100%" 
    BorderColor="White"
    AllowPaging="True" 
    AllowSorting="True" 
    PageSize="3"  OnSorting="GridView2_Sorting" OnPageIndexChanging="GridView2_PageIndexChanging">
    <HeaderStyle CssClass="title-header" />
    <Columns>
        <asp:HyperLinkField HeaderText="Page" DataNavigateUrlFields="url" DataTextField="url" ShowHeader="False" SortExpression="url" />
        <asp:BoundField DataField="Landings" HeaderText="Landings" SortExpression="Landings" />
    </Columns>
</asp:GridView>

<asp:GridView ID="GridView3" 
    runat="server" 
    AutoGenerateColumns="False" 
    Width="100%" 
    AllowPaging="True" 
    BorderColor="White"     
    AllowSorting="True" 
    PageSize="3"  OnSorting="GridView3_Sorting" OnPageIndexChanging="GridView3_PageIndexChanging">
    <HeaderStyle CssClass="title-header" />
    <Columns>
        <asp:BoundField DataField="referring_url_path" HeaderText="Referring URL" SortExpression="referring_url_path" />
        <asp:BoundField DataField="Landings" HeaderText="Landings" SortExpression="Landings" />
    </Columns>
</asp:GridView>
<asp:Label ID="ErrMsg" runat="server" EnableViewState="False" Visible="False"/>
<blockquote>
<div id="stats_aggr" runat="server" style="width: 50%">
    <table border="0" width="95%">
        <tr>
            <td>
                <asp:Label ID="lbl_total_hits" runat="server" Font-Bold="True" Text="Total Views of Page"/></td>
            <td>
                <asp:Label ID="num_total_hits" runat="server" Text="100"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_total_visitors" runat="server" Font-Bold="True" Text="Total Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_total_visitors" runat="server" Text="20"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_hits_per_visitor" runat="server" Font-Bold="True" Text="Page Views/Visitor"/></td>
            <td>
                <asp:Label ID="num_hits_per_visitor" runat="server" Text="5"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_new_visitors" runat="server" Font-Bold="True" Text="New Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_new_visitors" runat="server" Text="10"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_returning_visitors" runat="server" Font-Bold="True" Text="Returning Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_returning_visitors" runat="server" Text="10"/></td>
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
</div>
</blockquote>
