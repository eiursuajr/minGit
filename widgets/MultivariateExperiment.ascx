<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultivariateExperiment.ascx.cs" Inherits="widgets_MultivariateExperiment" %>
<%@ Register Src="~/widgets/MultivariateExperiment/controls/pagetree.ascx" TagName="PageTree" TagPrefix="UX" %>

<div id="experiment" runat="server">
<div class="multivariate-experiment-header" id="header" runat="server">
    <div id="divError" class="error" runat="server" enableviewstate="false" visible="false"></div>
    <div><asp:RequiredFieldValidator ID="valTargetPageID" runat="server" 
        ErrorMessage="Target page ID must be set" ControlToValidate="tbTargetPageID" 
        Display="Dynamic" CssClass="validation" ValidationGroup="multivariateExperiment"></asp:RequiredFieldValidator></div>
    <div><asp:RangeValidator ID="rvalTargetPageID" runat="server" 
        ErrorMessage="Target page ID must be a positive number" 
        ControlToValidate="tbTargetPageID" Display="Dynamic" 
        MinimumValue="1" MaximumValue="Infinite" CssClass="validation" ValidationGroup="multivariateExperiment"></asp:RangeValidator></div>
    
    <label id="lbl" runat="server">Target</label>
    <asp:PlaceHolder ID="phPageTree" runat="server" EnableViewState="false" />
    <span class="field"><asp:TextBox ID="tbTargetPageID" CssClass="textbox target-id tbTargetPageID" Text="0" runat="server"></asp:TextBox>
    <input type="button" value="..." id="browseTargetPage" class="target-page-browse" runat="server" />
    <asp:Button ID="btnStart" CssClass="button" runat="server" Text="Start" OnClick="btnStart_Click" ValidationGroup="multivariateExperiment" />
    <asp:Button ID="btnStop" CssClass="button" runat="server" Text="Stop" OnClick="btnStop_Click" /></span>
    <asp:PlaceHolder ID="pnlLoggedIn" runat="server">
        <div><asp:RequiredFieldValidator ID="valConversions" runat="server" 
            ErrorMessage="Conversion count must be set" ControlToValidate="tbMaxConversions" 
            Display="Dynamic" CssClass="validation" ValidationGroup="multivariateExperiment"></asp:RequiredFieldValidator></div>
        <div><asp:RangeValidator ID="rvalConversions" runat="server" 
            ErrorMessage="Conversion count must be a positive number" 
            ControlToValidate="tbMaxConversions" Display="Dynamic" 
            MinimumValue="1"  MaximumValue="Infinite" CssClass="validation" ValidationGroup="multivariateExperiment"></asp:RangeValidator></div>
        <span class="field"><label id="lblMaxConversions" runat="server">Conversions</label>
        <asp:TextBox ID="tbMaxConversions" CssClass="textbox tbMaxConversions" Text="1000" runat="server"></asp:TextBox></span>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlAnon" runat="server">
        <span class="max-conversions">(Conversions: <asp:Label ID="txtMaxConversions" runat="server"></asp:Label>)</span>
        <a class="show-report" href="#" runat="server">show report</a>
        <a class="hide-report" href="#" runat="server">hide report</a>
    </asp:PlaceHolder>
</div>
<div class="multivariate-experiment-content ui-widget" id="divContent" runat="server" enableviewstate="false">
    <ul class="reports-list">
        <asp:Repeater ID="repCombinationReports" runat="server" 
            onitemdatabound="repCombinationReports_ItemDataBound">
        <ItemTemplate>
        <li id="reportRow" runat="server">
            <a href="#" id="aPreview" runat="server"><img src="<%# _capi.SitePath + "widgets/MultivariateExperiment/css/images/Preview.png" %>" alt="Preview" title="Preview"/></a>
            <asp:LinkButton ID="btnDisable" runat="server" CommandName="disable">
                <img src="<%# _capi.SitePath + "widgets/MultivariateExperiment/css/images/enable.png" %>" alt="Disable" title="Disable"/>
            </asp:LinkButton>
            <asp:LinkButton ID="btnEnable" runat="server" CommandName="enable">
                <img src="<%# _capi.SitePath + "widgets/MultivariateExperiment/css/images/disable.png" %>" alt="Enable" title="Enable"/>
            </asp:LinkButton>
            <asp:LinkButton ID="btnPromote" runat="server" CssClass="promote-button" CommandName="promote">
                <img src="<%# _capi.SitePath + "widgets/MultivariateExperiment/css/images/promote.png" %>" alt="Promote" title="Promote"/>
            </asp:LinkButton>
            <div class="promote-dialog" title="Promote - Are you sure?">
            </div>
            <div class="progress-bar ui-corner-all"><div class="progress ui-widget-header ui-corner-all" style="width:<%# (Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Hits > 0?((Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Conversions*100/(Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Hits):0 %>%;">&nbsp;</div></div>
            <span class="progress-bar-percent"><%# (Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Hits > 0?((Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Conversions*100/(Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Hits):0 %>%</span>
            <span class="progress-bar-data"><%# (Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Conversions %>/<%# (Container.DataItem as Ektron.Cms.Widget.Multivariate.MultivariateReportData).Hits %></span>
        </li>
        </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>
</div>
