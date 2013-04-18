<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Import.ascx.cs" Inherits="Controls_Import" %>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr class="title-header">
            <td>
                <p class="label">
                    Import files are text files, having one word per line with no leading or trailing
                    whitespace.&nbsp; Old TDF files are suitable too.</p>
            </td>
        </tr>
        <tr>
            <td>
                <span title="Pick a file to import" class="label">Pick a file to import:</span>
                <input title="Import File" id="importedFile" runat="server" type="file" />
                <asp:Button ToolTip="Import" ID="importButton" runat="server" AccessKey="i" CssClass="button" Text="Import"
                    OnClick="importButton_Click" />
                <asp:Panel ID="errorPanel" runat="server" Visible="False">
                    <asp:Label ToolTip="Error Message" ID="errorMessage" runat="server" ForeColor="Red" />
                </asp:Panel>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <b><span title="Available Dictionaries" class="label">Available Dictionaries:</span></b>
                <div class="ektronPageGrid">
                    <asp:DataGrid ID="importedFiles"
                        runat="server"
                        AutoGenerateColumns="False"
                        GridLines="none"
                        Height="40px"
                        OnItemCommand="importedFiles_ItemCommand"
                        OnItemDataBound="importedFiles_ItemDataBound"
                        ShowHeader="False"
                        CssClass="ektronGrid"
                        Width="100%">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:Label ToolTip='<%# Container.DataItem %>' ID="fileLink" Text='<%# Container.DataItem %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
    </table>
</div>
