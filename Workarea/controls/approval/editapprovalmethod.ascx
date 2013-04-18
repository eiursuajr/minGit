<%@ Control Language="C#" AutoEventWireup="true" Inherits="editapprovalmethod" CodeFile="editapprovalmethod.ascx.cs" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div id="tdMoveToFolderList" runat="server"></div>

<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronGrid">
        <tbody>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl approval method")%></td>
                <td class="value">
                    <asp:RadioButtonList id="rblApprovalMethod" repeatcolumns="1" repeatdirection="Horizontal" RepeatLayout="Table" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
</div>
