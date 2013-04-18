<%@ Control Language="C#" AutoEventWireup="true" CodeFile="personalization_widget_list.ascx.cs" Inherits="WidgetControls_widget_list" %>
<%@ Reference Control="personalization_widget.ascx" %>

<td id="tdWidgetList" runat="server" class="column widget_list">
    <div class="columnRemove">
        <p>
            <asp:LinkButton ID="lbRemoveWidgetList" OnClick="lbRemoveWidgetList_Click" OnClientClick="Ektron.Personalization.Columns.remove(this);return false;" ToolTip="Remove Column" CssClass="removeColumn clearfix" runat="server">
                <img id="imgColumnRemove" runat="server" class="normal columnRemove" src=""  />
                <img id="imgColumnRemoveHover" class="hover columnRemoveHover" runat="server" src=""   />
            </asp:LinkButton>
        </p>
    </div>
    <asp:PlaceHolder ID="phWidgets" runat="server"></asp:PlaceHolder>
</td>