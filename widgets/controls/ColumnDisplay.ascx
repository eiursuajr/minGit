<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ColumnDisplay.ascx.cs" Inherits="ColumnDisplay" %>
<%@ Register Src="~/Workarea/PageBuilder/PageControls/WidgetHost.ascx" TagPrefix="EktronUC" TagName="WidgetHost" %>

<div class="columns-container clearfix" >
<asp:Repeater ID="repColumns" runat="server" OnItemDataBound="repColumns_ItemDataBound">
    <ItemTemplate>
        <div id="zone" class="PBColumn nested" runat="server">
            <ul class="columnwidgetlist" id="column" runat="server">
                <li class="header" id="headerItem" runat="server">
					<asp:Label ID="HeaderCaption" CssClass="columnwidgetcaption" EnableViewState="false" runat="server" />
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
</div>