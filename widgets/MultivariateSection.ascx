<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultivariateSection.ascx.cs" Inherits="widgets_MultivariateSection" %>
<%@ Register Src="../Workarea/PageBuilder/PageControls/WidgetHost.ascx" TagPrefix="EktronUC" TagName="WidgetHost" %>

<div id="multivariate" runat="server" class="multivariate-section">
<div id="slider" class="multivariate-buttons" runat="server">
    <asp:Button ID="btnAddVariation" Text="Add" runat="server" CssClass="add-variant-btn" OnClick="btnAddVariation_Click" />
    
    <div class="slider-container">
        <span class="item-number">0</span>
        <div class="add-variant" id="addVariant" alt="Add Variation" title="Add Variation" runat="server">+</div>
        <div class="slider"></div>
    </div>
</div>
<asp:Literal ID="litDebugOutput" runat="server"></asp:Literal>
<div class="columns-container">
<asp:Repeater ID="repColumns" runat="server" OnItemDataBound="repColumns_ItemDataBound">
    <ItemTemplate>
        <div id="zone" class="PBColumn nested" style="display: none;" runat="server">
            <ul class="columnwidgetlist" id="column" runat="server">
                <li class="header" id="headerItem" runat="server">
                    <a href="#" class="resizeColumn" onclick="Ektron.PageBuilder.WidgetHost.resizeColumn(this);return false;" runat="server" id="lbResizeColumn">
                        <img alt="" id="imgresizecolumn" runat="server" class="PBeditbutton PB-UI-icon" src="#" />
                    </a>
                    <asp:LinkButton CssClass="remColumn" ID="btnDeleteColumn" runat="server">
                        <img alt="" id="imgremcolumn" runat="server" class="PBclosebutton PB-UI-icon" src="#" />
                    </asp:LinkButton>
                </li>
                <asp:Repeater ID="controlcolumn" runat="server">
                    <ItemTemplate>
                        <li class="PBItem">
                            <EktronUC:WidgetHost ID="WidgetHost" runat="server" />
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </ItemTemplate>
</asp:Repeater>
&nbsp;
</div>
</div>