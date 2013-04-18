<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SalesTrend.ascx.cs" Inherits="Workarea_Widgets_SalesTrend" %>
<%@ Register TagPrefix="ektron" TagName="TimeLineChart" Src="../../controls/reports/TimeLineChart.ascx" %>
<asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<asp:HiddenField ID="hdn_filter" runat="server" />
    
<asp:Panel ID="pnlData" runat="server">  
    <div>
        <asp:DropDownList ToolTip="Select Sync Period from Drop Down Menu" id="drp_period" runat="server" CssClass="selectElementClass"></asp:DropDownList>
    </div>

    <div class="ektronTopSpace"><asp:Literal ID="ltr_count" runat="server"></asp:Literal></div>
    <div class="ektronTopSpace"><asp:Literal ID="ltr_value" runat="server"></asp:Literal></div>
    
    <div class="ektronPageGrid">
    
        <table width="100%">
            <tr>
                <td><ektron:TimeLineChart id="TrendTimeLineChart" width="300px" height="150px" runat="server" /></td>
                <td><asp:Literal ID="ltr_description" runat="server"/></td>
            </tr>
        </table>
    
    </div>
    
</asp:Panel>