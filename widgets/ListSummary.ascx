<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListSummary.ascx.cs" Inherits="widgets_ListSummary" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/ListSummary/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <CMS:ListSummary ID="ListSummary1" runat="server" EnablePaging="True" FolderID="16"
           ContentParameter="id" MaxResults="2" LinkTarget="_blank" />
        <asp:Label ID="errorLb" runat="server" />
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit" class="LSWidget">
            <div class="LSEdit">
                <div class="LSTabInterface">
                    <ul class="LSTabWrapper">
                        <li class="LSTab selected"><a href="#ByProperty"><span>Property</span></a></li>
                        <li class="LSTab"><a href="#ByFolder"><span>Folder</span></a></li>
                    </ul>
                    <div class="ByFolder LSTabPanel">
                        <UC:FolderTree ID="foldertree" runat="server" />
                    </div>
                    <div class="ByProperty LSTabPanel">
                        <table style="width: auto;">
                            <tr>
                                <td>
                                    Folder Id:</td>
                                <td>
                                    <asp:TextBox ID="folderid" CssClass="folderid" runat="server" Style="width: auto;"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                   MaxResults:</td>
                                <td>
                                    <asp:TextBox ID="pagesize" runat="server" Style="width: auto;"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    Teaser:</td>
                                <td>
                                    <asp:CheckBox ID="TeaserCheckBox" runat="server" Checked="true" /></td>
                            </tr>
                            <tr>
                                <td>
                                    Recursive:</td>
                                <td>
                                    <asp:CheckBox ID="RecursiveCheckBox" runat="server" Checked="false" /></td>
                            </tr>
                            <tr>
                                <td>
                                    EnablePaging:</td>
                                <td>
                                    <asp:CheckBox ID="EnablePagingCheckBox" runat="server" Checked="false" /></td>
                            </tr>
                            <tr>
                                <td>
                                    IncludeIcons:</td>
                                <td>
                                    <asp:CheckBox ID="IncludeIconsCheckBox" runat="server" Checked="false" /></td>
                            </tr>
                            <tr>
                                <td>
                                    OrderByDirection:</td>
                                <td>
                                    <asp:DropDownList ID="DirectionSelectDropDownList" runat="server">
                                        <asp:ListItem Value="Ascending">Ascending</asp:ListItem>
                                        <asp:ListItem Value="Descending">Descending</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    OrderKey:</td>
                                <td>
                                    <asp:DropDownList ID="OrderKeyDropDownList" runat="server">
                                        <asp:ListItem Value="Title">Title</asp:ListItem>
                                        <asp:ListItem Value="DateModified">DateModified</asp:ListItem>
                                        <asp:ListItem Value="DateCreated">DateCreated</asp:ListItem>
                                        <asp:ListItem Value="LastEditorFname">LastEditorFname</asp:ListItem>
                                        <asp:ListItem Value="LastEditorLname">LastEditorLname</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
							<tr>
                                <td>
                                    ContentType:</td>
                                <td>
                                    <asp:DropDownList ID="ContentTypeDropDownList" runat="server">
                                        <asp:ListItem Value="AllTypes">AllTypes</asp:ListItem>
                                        <asp:ListItem Value="Content">Content</asp:ListItem>
                                        <asp:ListItem Value="Forms">Forms</asp:ListItem>
                                        <asp:ListItem Value="Archive_Content">Archive_Content</asp:ListItem>
                                        <asp:ListItem Value="Archive_Forms">Archive_Forms</asp:ListItem>
                                        <asp:ListItem Value="Assets">Assets</asp:ListItem>
                                        <asp:ListItem Value="Archive_Assets">Archive_Assets</asp:ListItem>
                                        <asp:ListItem Value="LibraryItem">LibraryItem</asp:ListItem>
                                        <asp:ListItem Value="Multimedia">Multimedia</asp:ListItem>
                                        <asp:ListItem Value="Archive_Media">Archive_Media</asp:ListItem>
                                        <asp:ListItem Value="NonLibraryContent">NonLibraryContent</asp:ListItem>
                                        <asp:ListItem Value="DiscussionTopic">DiscussionTopic</asp:ListItem>
                                        <asp:ListItem Value="CatalogEntry">CatalogEntry</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    AddText:</td>
                                <td>
                                    <asp:TextBox ID="AddTextTextBox" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    SelTaxonomyID:</td>
                                <td>
                                    <asp:TextBox ID="SelTaxonomyIDTextBox" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    DisplaySelectedContent:</td>
                                <td>
                                    <asp:CheckBox ID="DisplaySelectedContentCheckBox" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="LSEditControls">
						<asp:Button ID="CancelButton" CssClass="LSCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        <asp:Button ID="SaveButton" CssClass="LSSave" runat="server" OnClick="SaveButton_Click"
                            Text="Save" />
                    </div>
                    <asp:TextBox ID="tbData" CssClass="HiddenTBData" runat="server" Style="display: none;"></asp:TextBox>
                    <asp:TextBox ID="tbFolderPath" CssClass="HiddenTBFolderPath" runat="server" Style="display: none;"></asp:TextBox>
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>
