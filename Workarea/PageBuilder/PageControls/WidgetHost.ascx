<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WidgetHost.ascx.cs" Inherits="WidgetHostCtrl" %>

<div class="widget" id="dropcontainer" runat="server">
    <asp:Label ID="lblErrorMessage" runat="server" Text="" EnableViewState="false"></asp:Label>
    <asp:UpdatePanel ID="updatepanel" runat="server" OnLoad="widgetPanelLoad" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="header" id="toolbar" runat="server" enableviewstate="false">
                <div class="buttons">
                    <span><asp:Literal ID="lblTitle" runat="server" Text="Widget"></asp:Literal>&#160;</span>
                    
                    <a href="#" class="help" id="lbHelpWidget" target="_blank" runat="server" visible="false">
                        <img id="imgPBhelpbutton" runat="server" alt="Help" class="PBhelpbutton PB-UI-icon" src="#" />
                    </a>
                    <a href="#" class="expand" id="lbExpandWidget" runat="server" onclick="Ektron.PageBuilder.WidgetHost.openAsModal(this); return false;" visible="false">
                        <img id="imgPBexpandbutton" runat="server" alt="Expand" class="PBexpandbutton PB-UI-icon" src="#" />
                    </a>
                    <asp:LinkButton CssClass="edit" ID="lbEditWidget" runat="server" OnClick="lbEdit_Click" Visible="false">
                        <img id="imgPBeditbutton" runat="server" alt="Edit" class="PBeditbutton PB-UI-icon" src="#" />
                    </asp:LinkButton>
                    <asp:LinkButton CssClass="delete" ID="lbDeleteWidget" runat="server" OnClick="lbDelete_Click" Visible="false">
                        <img id="imgPBclosebutton" runat="server" alt="Delete" class="PBclosebutton PB-UI-icon" src="#" />
                    </asp:LinkButton>
                </div>
            </div>
            <div class="content">
                <asp:PlaceHolder ID="phWidgetContent" runat="server"></asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>