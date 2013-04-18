<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IntegerField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.Templates.XmlSearch.IntegerField" %>
<%@ Import Namespace="Ektron.Cms.Framework.UI" %>
<tr class="integer-field">
    <td class="field-label">
        <asp:Label ID="aspLabel" runat="server" Text='<%# Eval("Label") %>' AssociatedControlID="aspOperators" />
    </td>
    <td>
        <asp:DropDownList ID="aspOperators" runat="server" CssClass="operators" DataSource='<%# Enum.GetNames(typeof(IntegerOperator)) %>' SelectedValue='<%# Bind("SelectedOperator") %>' meta:resourcekey="uxOperatorsResource1"></asp:DropDownList>
    
        <ektronUI:IntegerField ID="uxLowValue" runat="server" Text='<%# Bind("LowValue") %>' CssClass="lowvalue" />
        <span id="aspSeparatorContainer" runat="server" class="separator ektron-ui-hidden" >
            <asp:Literal id="aspSeparator" runat="server" Text="<%$ Resources:Separator %>" />
        </span>
        <ektronUI:IntegerField ID="uxHighValue" runat="server" Text='<%# Bind("HighValue") %>' CssClass="highvalue" />
    
        <ektronUI:JavaScriptBlock ID="uxDropDownListClientUI" runat="server" ExecutionMode="OnEktronReady" meta:resourcekey="uxDropDownListClientUIResource1">
            <ScriptTemplate>
                if ("undefined" == typeof (Ektron)) { Ektron = {}; }
                if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
                if ("undefined" == typeof (Ektron.Controls.EktronUI)) { Ektron.Controls.EktronUI = {}; }
                if ("undefined" == typeof (Ektron.Controls.EktronUI.IntegerField)) {
                    Ektron.Controls.EktronUI.IntegerField = {
                        hideShowFieldsForControl: function(select) {
                            if (0 == select.length) return;
                            var selectedOption = select.find(":selected");
                            if(1 == selectedOption.length && selectedOption.val().toLowerCase() === "between"){
                                select.siblings(".separator").removeClass("ektron-ui-hidden");
                                select.parent().find(".highvalue").closest(".ektron-ui-integerField").removeClass("ektron-ui-hidden");
                            } else {
                                select.siblings(".separator").addClass("ektron-ui-hidden");
                                select.parent().find(".highvalue").closest(".ektron-ui-integerField").addClass("ektron-ui-hidden").children("input").attr("value", "");
                            }
                        }
                    }
                }

                $ektron("#<%# aspOperators.ClientID%>").parent().find(".highvalue").closest(".ektron-ui-integerField").addClass("ektron-ui-hidden");
                Ektron.Controls.EktronUI.IntegerField.hideShowFieldsForControl($ektron("#<%# aspOperators.ClientID%>"));
                $ektron("#<%# aspOperators.ClientID%>").change(function(){
                    Ektron.Controls.EktronUI.IntegerField.hideShowFieldsForControl($ektron(this));
                });
            </ScriptTemplate>
        </ektronUI:JavaScriptBlock>
    </td>
</tr>