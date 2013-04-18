<%@ Control Language="C#" AutoEventWireup="true" CodeFile="KeyPerformanceIndicators.ascx.cs" Inherits="Workarea_Widgets_KeyPerformanceIndicators" %>
<style>
    .KeyPerformanceIndicatorContainer .KPIHideBusyImage {display: none;}
</style>    
<asp:Panel ID="pnlData" runat="server">  
    <div class="KeyPerformanceIndicatorContainer" style="position: static; top: 0; left: 0;">
        <div style="position: relative; top: 0; left: 0;">
            <div style="background-color: transparent; position: absolute; top: 5px; left: 25px;">
                <asp:Image ToolTip="Busy" ID="imgBusyImage" runat="server" CssClass="KPIHideBusyImage" />
</div>
        </div>
        <div class="ektronPageGrid kpi">
            <asp:PlaceHolder ID="phTableContainer" runat="server" />
        </div>
    </div>
</asp:Panel>
