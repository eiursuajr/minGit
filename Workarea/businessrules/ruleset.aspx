<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ruleset.aspx.cs" Inherits="businessrules_ruleset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .widthNarrow {width: 3em;}
        span.noRules {color: #666;}
    </style>

    <script type="text/javascript">
        var msgSelectRule = "<asp:Literal id="msSelectRule" runat="server" />";
        var msgRuleToRemove = "<asp:Literal id="msRuleToRemove" runat="server" />";
        Ektron.ready(function()
        {
            var rulesetPane = $ektron("#rulesetpane");
            if (rulesetPane.length > 0 && rulesetPane.html() == "")
            {
                rulesetPane.html('<span class="noRules"><asp:Literal id="noRulesInSet" runat="server" /></span>');
            }
        });
    </script>
</head>
<body onclick="MenuUtil.hide()">

    <asp:literal runat="server" ID="ltr_action_js" />
    <asp:literal runat="server" ID="ltrjs" />

    <form id="form1" runat="server">
        <div id="trgrid" class="ektronPageContainer" runat="server">
            <div class="ektronPageGrid">
                <asp:DataGrid ID="ViewRuleSetGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    EnableViewState="False"
                    GridLines="None"
                    CssClass="ektronGrid"
                    >
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </div>
        </div>
        <div id="traddruleset" class="ektronPageContainer ektronPageInfo" runat="server" visible="false">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="name"><%=m_refMsg.GetMessage("name label")%></td>
                    <td class="value"><asp:TextBox ToolTip="Enter Name here" ID="txtRulesetName" runat="server" Columns="50" /></td>
                </tr>
            </table>
            <div runat="server" id="tridentifier">
                <asp:HiddenField ID="txtIdentifier" runat="server" />
            </div>
        </div>
        <div runat="server" id="traddedit" visible="false">
            <div class="ektronPageContainer ektronPageInfo">
                <div id="rulesetContainer"></div>
            </div>
            <asp:Literal ID="ltrrulejs" runat="server" />
        </div>
        <input type="hidden" id="txtactiverules" name="txtactiverules" />
        <input type="hidden" id="txtenabledrules" name="txtenabledrules" />
        <asp:Literal ID="ltrrulesetid" runat="server" />
    </form>
</body>
</html>

