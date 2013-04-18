<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BooleanField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch.BooleanField" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<tr class="boolean-field">
    <td class="field-label">
        <asp:Label ID="aspLabel" runat="server" Text='<%# Eval("Label")%>' AssociatedControlID="aspListChoices" />
    </td>
    <td>
        <asp:DropDownList ID="aspListChoices" runat="server" DataSource='<%# Enum.GetNames(typeof(BooleanOperator)) %>' SelectedValue='<%# Bind("SelectedOperator") %>' ></asp:DropDownList> 
    </td>
</tr>