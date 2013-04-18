<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TaxonomySummary.ascx.cs"
    Inherits="widgets_TaxonomySummary" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/TaxonomySummary/TaxonomyTree.ascx" TagPrefix="UC" TagName="TaxonomyTree" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:PlaceHolder ID="phContent" runat="server">
            <div class="theme<%=SelectedThemes%>">
                <h3 id="uxHeaderText" runat="server" class="taxlistheader">
                </h3>
                <CMS:Directory CssClass="taxList" ID="TaxonomySummary1" EnableAjax="true" runat="server"
                    EnablePaging="false" TaxonomyId="0" />
                <asp:Label ID="errorLb" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%; height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath %>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget
                to select the Taxonomy you wish to display.
            </div>
        </asp:PlaceHolder>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit" class="TSWidget">
            <div class="TSEdit">
                <div class="TSTabInterface">
                    <ul class="TSTabWrapper">
                        <li class="TSTab <%=TaxonomySelected%>"><a href="#ByTaxonomy" class="LinkTaxonomy"><span>Taxonomy</span></a></li>
                        <li class="TSTab <%=PropertySelected%>"><a href="#ByProperty" class="LinkProperty"><span>Property</span></a></li>
                        <li class="TSTab"><a href="#ByThemes" class="LinkThemes"><span>Themes</span></a></li>
                    </ul>
                    <div class="ByTaxonomy TSTabPanel">
                        <div style="height: 150px; overflow: auto;">
                            <UC:TaxonomyTree ID="taxtree" runat="server" />
                        </div>
                        <hr />
                        <span style="float: left;">Taxonomy Path: <span class="curPath">
                            <%=m_strTaxonomyPath%>
                        </span></span>
                    </div>
                    <div class="ByProperty TSTabPanel">
                        <table style="width: auto;">
                            <tr>
                                <td class="label">
                                    Header Text:
                                </td>
                                <td>
                                    <asp:TextBox ID="headertext" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Taxonomy Id:
                                </td>
                                <td>
                                    <asp:TextBox ID="taxonomyid" CssClass="folderid" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Taxonomy Path:
                                </td>
                                <td>
                                    <asp:Label ID="taxonomypath" CssClass="taxonomypath" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    MaxResults:
                                </td>
                                <td>
                                    <asp:TextBox ID="pagesize" runat="server" Style="width: auto;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Teaser:
                                </td>
                                <td>
                                    <asp:CheckBox ID="TeaserCheckBox" runat="server" Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Enable Paging:
                                </td>
                                <td>
                                    <asp:CheckBox ID="EnablePagingCheckBox" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Sort Order:
                                </td>
                                <td>
                                    <asp:DropDownList ID="DirectionSelectDropDownList" runat="server">
                                        <asp:ListItem Value="Ascending">Ascending</asp:ListItem>
                                        <asp:ListItem Value="Descending">Descending</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Order Key:
                                </td>
                                <td>
                                    <asp:DropDownList ID="OrderKeyDropDownList" runat="server">
                                        <asp:ListItem Value="content_title">Title</asp:ListItem>
                                        <asp:ListItem Value="last_edit_date">Date Modified</asp:ListItem>
                                        <asp:ListItem Value="date_created">Date Created</asp:ListItem>
                                        <asp:ListItem Value="go_live">Go Live Date</asp:ListItem>
                                        <asp:ListItem Value="taxonomy_item_display_order">Item Display Order</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="ByThemes TSTabPanel">
                        <table style="width: auto; background-color: #cccccc; border: 1px solid #333333;">
                            <tr>
                                <td width="25%">
                                    <strong>Select a theme:</strong><br />
                                    <asp:ListBox Height="150" Width="150" SelectionMode="Single" ID="uxThemes" runat="server">
                                    </asp:ListBox>
                                </td>
                                <td width="75%">
                                    <strong>Preview:</strong><br />
                                    <%--<img id="uxFramePreview" runat="server" alt="Preview" border="1" width="325" height="150"
                                        src="~/widgets/TaxonomySummary/themes/default/preview.jpg" />--%>
                                    <iframe id="uxFramePreview" runat="server" src="Widgets/TaxonomySummary/Preview.aspx"
                                        width="100%" frameborder="0"></iframe>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="TSEditControls">
                        <asp:Button ID="CancelButton" CssClass="TSCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        <asp:Button ID="SaveButton" CssClass="TSSave" runat="server" OnClick="SaveButton_Click"
                            Text="Save" />
                    </div>
                    <asp:TextBox ID="tbData" CssClass="HiddenTBData" runat="server" Style="display: none;"></asp:TextBox>
                    <asp:TextBox ID="tbFolderPath" CssClass="HiddenTBFolderPath" runat="server" Style="display: none;"></asp:TextBox>
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>