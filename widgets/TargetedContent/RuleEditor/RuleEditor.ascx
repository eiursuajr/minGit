<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RuleEditor.ascx.cs" Inherits="RuleEditor" Debug="true" %>

<div id="container" class="ui-widget ektronRuleEditor" runat="server">
    <input type="hidden" id="ruleTemplateJSON" class="initRuleTemplatesData" runat="server" value="{}" />
    <input type="hidden" id="rulesJSON" class="initRulesData" runat="server" value="[]" />
    <input type="hidden" id="rulesByIdJSON" class="savedRulesData" runat="server" value="{}" />
    
    <span class="label ui-widget-header"><%=m_refMsg.GetMessage("lbl condition nc")%></span>
    <div class="ui-widget-content">
        <div class="rulesList">
        </div>
		<div class="toolBar ui-helper-clearfix">
			<a href="#add" class="ektron addRuleList ek-button ek-button-icon-left ui-corner-all ui-state-default"><span class="ui-icon ui-icon-plus"></span><%=m_refMsg.GetMessage("lbl or1")%></a>
		</div>
    </div>
    
</div>