<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Flickr.ascx.cs" Inherits="Widgets_Flickr" %>
<div id="<%= ClientID %>" class="ektronWidgetFlickr">
    <asp:HiddenField ID="hdnPhotoCollection" Value="" runat="server" />
    <asp:HiddenField ID="hdnImageCollectionCount" Value="0" runat="server" />
    <asp:HiddenField ID="hdnGettabindex" Value="-1" runat="server" />
    <asp:HiddenField ID="hdnPane" runat="server" />
    <asp:HiddenField ID="hdnSearchText" runat="server" />
    <asp:HiddenField ID="hdnSeatchType" runat="server" />
    <asp:HiddenField ID="hdnSortBy" runat="server" />
    <asp:HiddenField ID="hdnIdList" runat="server" Value="" />
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
        <asp:PlaceHolder ID="phContent" runat="server">
            <asp:Label ID="lbData" runat="server"></asp:Label>
            <asp:GridView ID="uxGVPhotoList" PagerSettings-Mode="NumericFirstLast" runat="server"
                AllowPaging="true" AutoGenerateColumns="false" CellPadding="1" CellSpacing="1"
                BorderWidth="1" BorderColor="#EEEEEE" Width="100%" GridLines="None" OnPageIndexChanging="uxGVPhotoList_PageIndexChanging">
                <RowStyle />
                <PagerStyle HorizontalAlign="Right" />
                <AlternatingRowStyle BackColor="#EEEEEE" />
                <Columns>
                    <asp:TemplateField ItemStyle-CssClass="first_row">
                        <ItemStyle Width="40%" VerticalAlign="Top" />
                        <ItemTemplate>
                            <%# Eval("ImageLinkforView")%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="60%" VerticalAlign="Top" />
                        <ItemTemplate>
                            <label style="font-size: 0.9em; text-align: left;">
                                <%# Eval("Title") %></label>
                            <br />
                            <br />
                            <asp:Literal runat="server" ID="uxlitShortDesc" Text='<%# Eval("ShortDesc") %>'></asp:Literal></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%; height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" id="flickerEditImage" title="edit icon" src="<%=appPath%>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget
                to select images you wish to display.
            </div>
        </asp:PlaceHolder>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div class="Flickr" id="<%=ClientID %>">
                <ul class="ektronWidgetFKTabs clearfix">
                    <li><a href="#" onclick="Ektron.Widget.Flickr.SwitchPane(this, 'ImageListTab'); HideRemove();  return false;"
                        id="ImageListTab" class="ektronWidgetFKTab selectedTab">Image List</a></li>
                    <li><a href="#" onclick="Ektron.Widget.Flickr.SwitchPane(this, 'SearchLink'); HideRemove(); return false;"
                        id="SearchLink" class="ektronWidgetFKTab">Search</a></li>
                    <li><a href="#" onclick="Ektron.Widget.Flickr.SwitchPane(this, 'Collection');  ShowRemove(); return false;"
                        id="Collection" class="ektronWidgetFKTab">Collection</a></li>
                    <li><a href="#" onclick="Ektron.Widget.Flickr.SwitchPane(this, 'Property'); HideRemove(); return false;"
                        id="Property" class="ektronWidgetFKTab">Properties</a></li>
                </ul>
                <div class="pane ImageListTab">
                    <div class="FKOptions FKViewOptions">
                        Sort by:
                        <select id="<%= ClientID %>sort_by" onchange="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].FirstImages();">
                            <option value="flickr.photos.getRecent" selected="selected">Recent Photos</option>
                            <option value="flickr.interestingness.getList">Interesting Photos</option>
                        </select>
                    </div>
                    <ul class="Image-list ektronWidgetFKImages">
                    </ul>
                    <ul class="ektronWidgetFKButtonWrapper">
                        <li><a id="<%= ClientID %>First" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].FirstImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonFirst" title="First" style="display: none;">
                            <span>First</span></a></li>
                        <li><a id="<%= ClientID %>Previous" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].PreviousImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonPrevious" title="Previous" style="display: none;">
                            <span>Prev</span></a></li>
                        <li><span class="Image-result">No Results</span></li>
                        <li><a id="<%= ClientID %>Next" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].NextImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonNext" title="Next" style="display: none;">
                            <span>Next</span></a></li>
                        <li><a id="<%= ClientID %>Last" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].LastImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonLast" title="Last" style="display: none;">
                            <span>Last</span></a></li>
                    </ul>
                    <asp:Button ID="btnAddCollection" runat="server" Text="Add to Collection" OnClick="btnAddCollection_Click" />
                    <br />
                </div>
                <div class="pane SearchLink" style="display: none;">
                    <div class="search-box FKOptions FKSearchOptions">
                        <asp:Literal runat="server" ID="uxSearchFirstImages"></asp:Literal>
                        Search by:
                        <select id="<%= ClientID %>searchtype" onchange="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchFirstImages();">
                            <option value="TEXT">Text</option>
                            <option value="TAG">Tag</option>
                        </select>
                        <input type="text" id="<%= ClientID %>SearchText" onkeypress="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].KeyPressHandler(this, event, '<%= ClientID %>');" />
                        <a id="<%= ClientID %>Search" title="Search" class="ektronWidgetFKGoButton" onclick="Ektron.Widget.Flickr.ResetPages();Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchImages();">
                            Go</a>
                    </div>
                    <ul class="Image-search ektronWidgetFKImages">
                    </ul>
                    <ul class="ektronWidgetFKButtonWrapper ektronWidgetFKSearchButtons">
                        <li><a id="<%= ClientID %>FirstSearch" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchFirstImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonFirst" title="First" style="display: none;">
                            <span>First</span></a></li>
                        <li><a id="<%= ClientID %>PreviousSearch" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchPreviousImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonPrevious" title="Previous" style="display: none;">
                            <span>Prev</span></a></li>
                        <li><span class="Image-search-result">No Results</span></li>
                        <li><a id="<%= ClientID %>NextSearch" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchNextImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonNext" title="Next" style="display: none;">
                            <span>Next</span></a></li>
                        <li><a id="<%= ClientID %>LastSearch" onclick="Ektron.Widget.Flickr.widgets['<%= ClientID %>'].SearchLastImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonLast" title="Last" style="display: none;">
                            <span>Last</span></a></li>
                    </ul>
                    <asp:Button ID="btnAddSearch" runat="server" Text="Add to Collection" OnClick="btnAddSearch_Click" />
                    <br />
                </div>
                <div class="pane Collection" ms_positioning="GridLayout" style="overflow: auto; display: none;
                    position: relative; height: 485px;">
                    <ul class="sortable boxy" id="ulSelected" style="width: 300px; height: <%=intHeight%>px">
                        <asp:Repeater ID="rptSelected" runat="server">
                            <ItemTemplate>
                                <li id="<%# Eval("Id")%>">
                                    <table cellpadding="1" cellspacing="1" border="0" width="100%">
                                        <tr>
                                            <td align="left" class="first_row" style="display: none;">
                                                <input type="checkbox" id="chkRequired1" runat="server" value='<%# Eval("Id") %>' checked='true' style="cursor: pointer; display: none;" name="chkRequired1" />
                                                <asp:Literal ID="ltquestionID" runat="server" Text='<%# Eval("Id") %>'></asp:Literal>
                                            </td>
                                            <td class="first_row" style="width: 20px;">
                                                <asp:CheckBox Style="cursor: default" runat="server" ID="uxchkRemove" />
                                            </td>
                                            <td class="first_row" align="left" valign="middle" width="90px">
                                                <%# Eval("ImageLink")%>
                                            </td>
                                            <td>
                                                <label style="font-size: 0.9em;">
                                                    <%# Eval("Title") %></label>
                                                <br />
                                                <br />
                                                <asp:TextBox runat="server" Text='<%# Eval("ShortDesc") %>' TextMode="MultiLine"
                                                    ID="uxtxtShortDesc" CssClass="txtdragdesc" MaxLength="200"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                    <asp:Label ID="uxNoDataAdded" runat="server"><br /><br />No image added.</asp:Label>
                </div>
                <div class="pane Property" style="display: none;">
                    <table style="width: 99%;">
                        <tr style="display: none;">
                            <td>
                                API Key:
                            </td>
                            <td>
                                <asp:TextBox ID="ApiKeyTextBox" runat="server" Style="width: 95%;">8d14f34875c77761bc18730f39fb13c9</asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:RadioButton runat="server" Text="Gallery View" GroupName="Displaymode" ID="uxRadioGallery" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Number of Columns:
                            </td>
                            <td>
                                <asp:TextBox ID="NumColsTextBox" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                    onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" runat="server"
                                    Style="width: 95%;" MaxLength="3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:RadioButton runat="server" Text="List View" GroupName="Displaymode" ID="uxRadioList" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Number of Records Per Page:
                            </td>
                            <td>
                                <asp:TextBox ID="uxRecordPerPage" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                    onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" runat="server"
                                    Style="width: 95%;" MaxLength="3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Picture Size:
                            </td>
                            <td>
                                <asp:DropDownList ID="uxSizeDropDownList" runat="server">
                                    <asp:ListItem Value="_t">thumbnail</asp:ListItem>
                                    <asp:ListItem Value="_s">small</asp:ListItem>
                                    <asp:ListItem Value="_m">medium</asp:ListItem>
                                    <asp:ListItem Value="_b">large</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Name of the widget
                            </td>
                            <td>
                                <asp:TextBox ID="uxWdgetName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <asp:TextBox ID="tbData" runat="server" Style="display: none;">
            </asp:TextBox>
            <asp:TextBox ID="tbData1" runat="server" Style="display: none;">
            </asp:TextBox>
            <br />
            <br />
            <div style="width: 100%; text-align: left;">
                <asp:Button ID="uxbtnRemove" runat="server" Visible="false" Text="Remove from collection"
                    OnClick="uxbtnRemove_Click" Style="display: none;" /><br />
                <label style="display: none; float: left;" id="helptext">
                    Note: Drag and drop the images to reorder.</label>
                <div style="text-align: right;">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
</div>
