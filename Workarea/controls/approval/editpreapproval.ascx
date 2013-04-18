<%@ Control Language="C#" AutoEventWireup="true" Inherits="editpreapproval" CodeFile="editpreapproval.ascx.cs" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr runat="server" id="trInerhit">
            <td class="label">
                <input type="checkbox" title="Inherit Parent Configuration" runat="server" name="chkInheritPreApprovalGroup"
                    id="chkInheritPreApprovalGroup" onclick="toggleSelectGroup(this.checked);" checked="checked" />
            </td>
            <td class="value">
                <asp:Literal ID="ltInheritPreApprovalGroup" runat="server" />
            </td>
        </tr>
        <tr id="trPreApprovalGroup">
            <td class="label">Preapproval Group:</td>
            <td class="value">
                <select disabled="disabled" name="selectusergroup" id="selectusergroup">
                    <asp:Literal id="lit_select_preapproval" runat="server" />
                </select>
            </td>
        </tr>
        <tr>
            <td>
                <input type="hidden" id="hdnFolderId" runat="server" />
                <input type="hidden" id="hdnInherited" name="hdnInherited" runat="server" />
            </td>
        </tr>
    </table>							
</div>
