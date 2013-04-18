<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteSelector.ascx.cs" Inherits="Analytics_controls_SiteSelector" %>
<span class="SiteSelectorContainer" ID="SiteSelectorContainer" runat="server">
    <asp:ImageButton ID="SegmentPopupBtn" CssClass="SegmentPopupBtn" runat="server" ImageUrl="../../images/UI/Icons/bricks.png" BorderStyle="None" AlternateText="Provider Segments Filter" ToolTip="Provider Segments Filter" />
    <asp:Label ID="lblSiteSelector" runat="server" EnableViewState="false" />
    <asp:DropDownList ToolTip="Select Site from the Drop Down Menu" ID="SiteSelectorList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged" />
</span>
