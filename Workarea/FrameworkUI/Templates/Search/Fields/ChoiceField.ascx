<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChoiceField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch.ChoiceField" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<tr class="choice-field">
    <td class="field-label">
        <asp:Label ID="aspLabel" runat="server" Text='<%# Eval("Label")%>' AssociatedControlID="aspListChoices" />
    </td>
    <td>
        <asp:ListBox ID="aspListChoices" runat="server" class="listChoices" 
            DataSource='<%# Eval("Choices") %>' DataValueField="Value" 
            DataTextField="Key" 
            SelectionMode='<%# (ListSelectionMode)Enum.Parse(typeof(ListSelectionMode), Eval("SelectionMode").ToString())%>' OnDataBound="aspListChoices_DataBound" 
            Rows="1"
            ></asp:ListBox> 
    </td>
</tr>