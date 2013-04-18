<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewContent.ascx.cs" Inherits="Ektron.Workarea.Reports.NewContent" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../paging/paging.ascx" %>
<div id="ReportDataGrid" class="NewContent">
    <asp:DataGrid 
            ID="dgNewContent" 
            runat="server" 
            OnItemDataBound="dgNewContent_ItemDataBound"
            AllowPaging="true"
            GridLines="None"
            PagerStyle-Visible="False"
            CssClass="ektronGrid" 
            AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litIconHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Image ID="imgContentIcon" runat="server" />
                </ItemTemplate>
                <HeaderStyle CssClass="smallCell" />
                <ItemStyle CssClass="smallCell" />
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litTitleHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aTitle" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litIdHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litIdValue" runat="server" />
                </ItemTemplate>
                <HeaderStyle CssClass="mediumCell" />
                <ItemStyle CssClass="mediumCell" />
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litLastEditorHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aLastEditor" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litModifiedDateHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litModifiedDateValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
             <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litDateModifiedHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litDateModified" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litPathHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aPathValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <ucEktron:Paging ID="ucPaging" runat="server" />
</div>