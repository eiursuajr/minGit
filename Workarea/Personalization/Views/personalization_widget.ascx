<%@ Control Language="C#" AutoEventWireup="true" CodeFile="personalization_widget.ascx.cs" Inherits="WidgetControls_widget" %>

<div class="widget">
    <div class="header">
        <div class="buttons">
            <asp:LinkButton ToolTip="Restore" CssClass="restore" ID="lbRestore" runat="server" OnClick="lbRestore_Click" OnClientClick="Ektron.Personalization.update(this);">
                <img id="imgWidgetRestore" runat="server" src="" class="widgetRestore" alt="Set in code behind" />
            </asp:LinkButton>
            <asp:LinkButton ToolTip="Minimize" CssClass="minimize" ID="lbMinimize" runat="server" OnClick="lbMinimize_Click" OnClientClick="Ektron.Personalization.update(this);">
                <img id="imgWidgetMinimize" runat="server" src="" class="widgetMinimize" alt="Set in code behind"/>
            </asp:LinkButton>
            <asp:LinkButton ToolTip="Close" CssClass="close" ID="lbCloseWidget" runat="server" OnClick="lbClose_Click" OnClientClick="Ektron.Personalization.update(this);">
                <img id="imgWidgetClose" runat="server" src="" class="widgetClose"  alt="Set in code behind" />
            </asp:LinkButton>
             <asp:LinkButton ToolTip="Edit" CssClass="edit" ID="lbEdit" runat="server" OnClick="lbEdit_Click" OnClientClick="Ektron.Personalization.update(this);">
                <img id="imgWidgetEdit" runat="server" src="" class="widgetEdit" alt="Set in code behind" />
            </asp:LinkButton>
            <h4><span><asp:Literal ID="lblTitle" runat="server" Text="Widget"></asp:Literal>&#160;</span></h4>
        </div>
        <input type="hidden" class="widgetTokenImagePath" id="hdnWidgetTokenImagePath" runat="server" />
        <input type="hidden" class="widgetTitle" id="hdnWidgetTitle" runat="server" />
        <input type="hidden" class="widgetTokenTypeId" id="hdnwidgetTokenTypeId" runat="server" />
    </div>
    <div class="content">
        <asp:PlaceHolder ID="phWidgetContent" EnableViewState="true" runat="server"></asp:PlaceHolder>
    </div>
</div>
