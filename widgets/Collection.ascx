<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Collection.ascx.cs" Inherits="widgets_Collection" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Label ID="Text" runat="server" Visible="false">Select a Collection</asp:Label>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit" class="LSWidget">
            <table style="width: 95%;" class="ekColEditView">
                <tr>
                    <td>
                        Collection Id:
                    </td>
                    <td>
                        <asp:DropDownList ID="collectionlist" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr style="font-size: 80%; color: #888;">
                    <td>
                        Description:
                    </td>
                    <td>
                        <span class="ekcoldescription" id="description" runat="server"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Page Size:
                    </td>
                    <td>
                        <asp:TextBox ID="pagesize" runat="server" Style="width: 95%;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Teaser:
                    </td>
                    <td>
                        <asp:CheckBox ID="TeaserCheckBox" runat="server" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        EnablePaging:
                    </td>
                    <td>
                        <asp:CheckBox ID="EnablePagingCheckBox" runat="server" Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        IncludeIcons:
                    </td>
                    <td>
                        <asp:CheckBox ID="IncludeIconsCheckBox" runat="server" Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        AddText:
                    </td>
                    <td>
                        <asp:TextBox ID="AddTextTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        SelTaxonomyID:
                    </td>
                    <td>
                        <asp:TextBox ID="SelTaxonomyIDTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        DisplaySelectedContent:
                    </td>
                    <td>
                        <asp:CheckBox ID="DisplaySelectedContentCheckBox" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                    <asp:Button ID="CancelButton" CssClass="LSCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        <asp:Button ID="Button1" runat="server" OnClick="SaveButton_Click" Text="Save" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:View>
</asp:MultiView>
