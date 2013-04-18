<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditDictionary.ascx.cs" Inherits="Controls_EditDictionary" %>
<b title="Select Dictionary">Select Dictionary: </b>
<asp:dropdownlist ToolTip="Select Dictionary" id="dictionarySelector" runat="server">
</asp:dropdownlist><br />
<asp:label id="messageLabel" runat="server" width="265px"></asp:label>
<table width="100%">
    <tr>
        <td valign="top">
            <asp:panel id="searchPanel" runat="server" class="module">
                <asp:label ToolTip="Find word" id="Label2" runat="server">Find word:</asp:label>
                <asp:textbox ToolTip="Find word" id="findWordBox" runat="server"></asp:textbox>
                <asp:button ToolTip="Find" id="findButton" runat="server" cssclass="button" onclick="findButton_Click"
                    text="Find" />
                <p>
                    <asp:listbox ToolTip="Selected for Delete" id="wordsFound" runat="server" height="164px" selectionmode="Multiple"
                        width="298px"></asp:listbox><br />
                    <asp:button ToolTip="Delete selected" id="deleteButton" runat="server" cssclass="button" onclick="deleteButton_Click"
                        text="Delete selected" width="298" />
                </p>
            </asp:panel>
        </td>
        <td valign="top">
            <asp:panel id="importPanel" runat="server" class="module" style="height: 124px; margin-bottom: 6px;">
                <br />
                <br />
                <asp:label ToolTip="Import wordlist" id="Label3" runat="server">Import wordlist:</asp:label>
                <input title="Import File" id="importedFile" runat="server" name="importedFile" type="file" />
                <asp:button ToolTip="Import" id="importButton" runat="server" cssclass="button" onclick="importButton_Click"
                    text="Import" />
            </asp:panel>
            <asp:panel id="addPanel" runat="server" class="module" style="height: 124px;">
                <br />
                <br />
                <br />
                <asp:label ToolTip="Add a word" id="Label1" runat="server">Add a word:</asp:label>
                <asp:textbox ToolTip="Add a word" id="addWordBox" runat="server"></asp:textbox>
                <asp:button ToolTip="Add" id="addButton" runat="server" class="button" onclick="addButton_Click"
                    text="Add" />
            </asp:panel>
        </td>
    </tr>
</table>
