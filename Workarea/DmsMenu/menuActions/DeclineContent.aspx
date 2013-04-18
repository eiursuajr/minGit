<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeclineContent.aspx.cs" Inherits="Workarea_DeclineContent" %>
<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="../../controls/Editor/ContentDesignerWithValidator.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Decline Content</title>
    <asp:literal id="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
		<div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div id="dataBox" style="border-width: 1px; display: block; z-index: 200;">
            <br />
            <asp:Literal ID="ltr_decline" runat="server">Enter a reason for declining this content (optional):</asp:Literal>
            <asp:RegularExpressionValidator ID="RegExpValidator" runat="server" ControlToValidate="DeclineText" ValidationExpression="^[\w\W]{0,250}$"></asp:RegularExpressionValidator>
            <%--            <asp:TextBox ID="DeclineText" runat="server"></asp:TextBox>--%>
            <ektron:ContentDesigner ID="DeclineText" runat="server" AllowScripts="false" Height="200"
                Width="60%" Toolbars="Minimal" ShowHtmlMode="false" />
            <br />
            <asp:Button ToolTip="Decline content" ID="btnDecline" runat="server" Text="Decline"  OnClick="btnDecline_Click"/>
            <input type="hidden" id="hdnContentId" name="hdnContentId" runat="server" />
            <input type="hidden" id="hdnLangType" name="hdnLangType" runat="server" />
            <input type="hidden" id="hdnFolderId" name="hdnFolderId" runat="server" />
        </div>
    </form>
</body>
</html>

