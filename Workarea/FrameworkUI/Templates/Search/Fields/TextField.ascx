<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch.TextField" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<tr class="text-field">
    <td class="field-label">
        <asp:Label ID="aspLabel" runat="server" Text='<%# Eval("Label")%>' AssociatedControlID="aspOperators" />
    </td>
    <td>
        <asp:DropDownList ID="aspOperators" runat="server" class="operators" DataSource='<%# Enum.GetNames(typeof(TextOperator)) %>'
            SelectedValue='<%# Bind("SelectedOperator") %>' ></asp:DropDownList>
        <ektronUI:TextField ID="uxInputText" runat="server" Text='<%# Bind("Value") %>' CssClass="value" />
    </td>
</tr>