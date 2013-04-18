<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Button.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.Button" %>
<asp:MultiView ID="aspButtonMarkupMode" runat="server">
    <asp:View ID="aspButtonMarkup" runat="server">
        <button id="<%# this.ButtonID %>" <%= this.GetCssClass() %> onclick="<%# this.ClientClick %>" title="<%# this.ControlContainer.ToolTip %>"><%# this.ControlContainer.Text %></button>
    </asp:View>
    <asp:View ID="aspSubmitMarkup" runat="server">
        <input type="submit" id="<%# this.ButtonID %>" <%= this.GetCssClass() %> name="<%# this.ButtonID %>"
            onclick="<%# this.ClientClick %>" title="<%# this.ControlContainer.ToolTip %>"
            value="<%# this.ControlContainer.Text %>" <%# this.AutoComplete() %> />
    </asp:View>
    <asp:View ID="aspAnchorMarkup" runat="server"><a id="<%# this.ButtonID %>" <%# this.GetCssClass() %> name="<%# this.ButtonID %>" title="<%# this.ControlContainer.ToolTip %>" onclick="<%# this.ClientClick %>" href="#<%# this.HrefHelper() %>"><%# this.ControlContainer.Text %></a></asp:View>
    <asp:View ID="aspCheckboxMarkup" runat="server">
        <asp:CheckBox ID="aspCheckbox" runat="server" ToolTip="<%# this.ControlContainer.ToolTip %>" />
        <asp:Label ID="aspCheckboxLabel" runat="server" ToolTip="<%# this.ControlContainer.ToolTip %>"
            AssociatedControlID="aspCheckbox" />
    </asp:View>
    <asp:View ID="aspRadioButtonMarkup" runat="server">
        <ektronUI:JavaScript ID="uxButtonJS" runat="server" Path="{UIPath}/js/Ektron/Controls/EktronUI/Ektron.Controls.EktronUI.Button.js" />
        <asp:PlaceHolder ID="aspRadioButtonPlaceHolder" runat="server">
            <input id="<%# this.RadioButtonID  %>" name="<%# this.RadioButtonName %>" type="radio"
                <%# this.RadioButtonOnClickValue %> <%# this.RadioButtonChecked %> title="<%# this.ControlContainer.ToolTip %>"
                class="<%# this.ControlContainer.CssClass %>" <%# this.AutoComplete() %> />
            <label title="<%# this.ControlContainer.ToolTip %>" for="<%# this.RadioButtonID %>" <%= this.GetCssClass() %>>
                <%# this.RadioButtonText %>
                <asp:HiddenField ID="aspRadioButtonValue" runat="server" />
            </label>
        </asp:PlaceHolder>
    </asp:View>
</asp:MultiView>
