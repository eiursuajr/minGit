<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch.Date" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<tr class="date-field">
    <td class="field-label">
        <asp:Label ID="aspLabel" runat="server" Text='<%# Eval("Label") %>' AssociatedControlID="aspOperators" ToolTip='<%# Eval("Label") %>' />
    </td>
    <td>
        <asp:DropDownList ID="aspOperators" runat="server" CssClass="operators" DataSource='<%# Enum.GetNames(typeof(DateOperator)) %>' 
            SelectedValue='<%# Eval("SelectedOperator") %>' meta:resourcekey="uxOperatorsResource"></asp:DropDownList>
        <ektronUI:Datepicker ID="uxLowValue" runat="server" DisplayMode="Default" Date='<%# Eval("LowValue") %>' CssClass="lowvalue" DisplayChangeMonth="true" DisplayChangeYear="true" />
        <span id="aspSeparatorContainer" runat="server" class="separator ektron-ui-hidden" >
            <asp:Literal id="aspSeparator" runat="server" Text="<%$ Resources:Separator %>" />
        </span>
        <ektronUI:Datepicker ID="uxHighValue" runat="server" DisplayMode="Default" Date='<%# Eval("HighValue") %>' CssClass="highvalue ektron-ui-hidden" DisplayChangeMonth="true" DisplayChangeYear="true" />
        <ektronUI:JavaScriptBlock ID="uxDropDownListClientUI" runat="server" ExecutionMode="OnEktronReady" meta:resourcekey="uxDropDownListClientUIResource1">
            <ScriptTemplate>
                $ektron("#<%# aspOperators.ClientID%>").change(function(){
                    var select = $ektron(this);
                    var selectedOption = select.find(":selected");
                    if(selectedOption.val().toLowerCase() === "between"){
                        select.siblings(".separator").removeClass("ektron-ui-hidden");
                        select.parent().find(".highvalue").removeClass("ektron-ui-hidden");
                    } else {
                        select.siblings(".separator").addClass("ektron-ui-hidden");
                        select.parent().find(".highvalue").addClass("ektron-ui-hidden");
                    }
                });
            </ScriptTemplate>
        </ektronUI:JavaScriptBlock>
    </td>
</tr>
