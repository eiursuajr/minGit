<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetaDataList.ascx.cs"
    Inherits="Widgets_MetaDataList" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/ContentBlock/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <CMS:MetaDataList ID="MDList" runat="server" ContentType="AllTypes" />
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=uniqueId%>" class="MDWidget">
            <div class="MDEdit">
                <div class="MDTabInterface">
                    <ul class="MDTabWrapper clearfix">
                        <li class="MDTab selected"><a href="#FolderSource"><span>Folder Source</span></a></li>
                        <li class="MDTab"><a href="#SourceOptions"><span>Source Options</span></a></li>
                        <li class="MDTab"><a href="#DisplayOptions"><span>Display Options</span></a></li>
                    </ul>
                    <div class="FolderSource MDTabPanel">
                        <UC:FolderTree ID="MDFolderTree" runat="server" />
                    </div>
                    <div class="SourceOptions MDTabPanel">
                        <table>
                            <tr>
                                <td>
                                    Folder Id:
                                </td>
                                <td>
                                    <asp:TextBox ID="optFolderId" CssClass="folderid" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Recursive:
                                </td>
                                <td>
                                    <asp:CheckBox ID="optRecursive" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Exact Phrase:
                                </td>
                                <td>
                                    <asp:CheckBox ID="optExactPhrase" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Match All Keywords:
                                </td>
                                <td>
                                    <asp:CheckBox ID="optMatchAll" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Keyword Name:
                                </td>
                                <td>
                                    <asp:DropDownList ID="optKeywordName" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Keyword Value:
                                </td>
                                <td>
                                    <asp:TextBox ID="optKeywordVal" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Keyword Value Separator:
                                </td>
                                <td>
                                    <asp:TextBox ID="optSeperator" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="DisplayOptions MDTabPanel">
                        <table>
                            <tr>
                                <td>
                                    Enable Paging:
                                </td>
                                <td>
                                    <asp:CheckBox ID="optPaging" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Max Results:
                                </td>
                                <td>
                                    <asp:TextBox ID="optMaxNum" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Navigation / Teaser:
                                </td>
                                <td>
                                    <asp:DropDownList ID="optNavTeaser" runat="server">
                                        <asp:ListItem Text="ecmNavigation" Value="ecmNavigation"></asp:ListItem>
                                        <asp:ListItem Text="ecmTeaser" Value="ecmTeaser"></asp:ListItem>
                                        <asp:ListItem Text="ecmUnOrderedList" Value="ecmUnOrderedList"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Sort Order:
                                </td>
                                <td>
                                    <asp:DropDownList ID="optSortOrder" runat="server">
                                        <asp:ListItem Text="Ascending" Value="Ascending"></asp:ListItem>
                                        <asp:ListItem Text="Descending" Value="Descending"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    OrderBy:</td>
                                <td>
                                    <asp:DropDownList ID="OrderKeyDropDownList" runat="server">
                                        <asp:ListItem Value="Title">Title</asp:ListItem>
                                        <asp:ListItem Value="DateModified">DateModified</asp:ListItem>
                                        <asp:ListItem Value="DateCreated">DateCreated</asp:ListItem>
                                        <asp:ListItem Value="LastEditorFname">LastEditorFname</asp:ListItem>
                                        <asp:ListItem Value="LastEditorLname">LastEditorLname</asp:ListItem>
                                        <asp:ListItem Value="ID">ID</asp:ListItem>
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
                                    Include Icons:
                                </td>
                                <td>
                                    <asp:CheckBox ID="optIncludeIcons" runat="server" Checked="false" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Button ID="CancelButton" CssClass="MDCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                    <asp:Button ID="SaveButton" CssClass="MDSave" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <input type="hidden" id="hdnAppPath" name = "hdnAppPath" value="<%=appPath%>" />
                    <input type="hidden" class="hdnFolderPath" id="hdnFolderPath" name="hdnFolderPath" value="" runat="server" />
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>