<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ITunePodcast.ascx.cs" Inherits="widgets_ITunePodcast" %>
<div id="<%= ClientID %>" class="ektronWidgetITunePodcast">
    <asp:HiddenField ID="hdnPodcastCollection" Value="" runat="server" />
    <asp:HiddenField ID="hdnPodcastCollectionCount" Value="0" runat="server" />
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
            <asp:GridView ID="uxGVPodcastList" PagerSettings-Mode="NumericFirstLast" runat="server"
                AllowPaging="true" AutoGenerateColumns="false" CellPadding="1" CellSpacing="1"
                BorderWidth="1" BorderColor="#EEEEEE" Width="100%" GridLines="None" OnPageIndexChanging="uxGVPodcastList_PageIndexChanging">
                <RowStyle />
                <PagerStyle HorizontalAlign="Right" />
                <AlternatingRowStyle BackColor="#EEEEEE" />
                <Columns>
                    <asp:TemplateField ItemStyle-CssClass="first_row">
                        <ItemStyle Width="15%" VerticalAlign="Top" />
                        <ItemTemplate>
                            <%# Eval("ImageLinkforView")%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="85%" VerticalAlign="Top" />
                        <ItemTemplate>
                            <label style="font-size: 0.9em; text-align: left;">
                                <%# Eval("TrackName")%></label>
                            <br />
                            <br />
                            <strong>Collection Name:</strong> <%# Eval("CollectionName")%><br />
                            <strong>Artist Name:</strong> <%# Eval("ArtistName")%></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%; height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath %>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget
                to select itune podcasts you wish to display.
            </div>
        </asp:PlaceHolder>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div class="ITunePodcast" id="<%=ClientID %>">
                <ul class="ektronWidgetFKTabs clearfix">
                    <li><a href="#" onclick="Ektron.Widget.ITunePodcast.SwitchPane(this, 'ImageListTab'); HideRemove();  return false;"
                        id="ImageListTab" class="ektronWidgetFKTab selectedTab">Podcast List</a></li>
                    <li><a href="#" onclick="Ektron.Widget.ITunePodcast.SwitchPane(this, 'SearchLink'); HideRemove(); return false;"
                        id="SearchLink" class="ektronWidgetFKTab">Search</a></li>
                    <li><a href="#" onclick="Ektron.Widget.ITunePodcast.SwitchPane(this, 'Collection');  ShowRemove(); return false;"
                        id="Collection" class="ektronWidgetFKTab">Collection</a></li>
                    <li><a href="#" onclick="Ektron.Widget.ITunePodcast.SwitchPane(this, 'Property'); HideRemove(); return false;"
                        id="Property" class="ektronWidgetFKTab">Properties</a></li>
                </ul>
                <div class="pane ImageListTab">
                    <div class="FKOptions">
                    </div>
                    <ul class="Image-list ektronWidgetFKImages">
                    </ul>
                    <ul class="ektronWidgetFKButtonWrapper">
                        <li><a id="<%= ClientID %>First" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].FirstImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonFirst" title="First" style="display: none;">
                            <span>First</span></a></li>
                        <li><a id="<%= ClientID %>Previous" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].PreviousImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonPrevious" title="Previous" style="display: none;">
                            <span>Prev</span></a></li>
                        <li><span class="Image-result">No Results</span></li>
                        <li><a id="<%= ClientID %>Next" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].NextImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonNext" title="Next" style="display: none;">
                            <span>Next</span></a></li>
                        <li><a id="<%= ClientID %>Last" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].LastImages();"
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
                        <input type="text" id="<%= ClientID %>SearchText" onkeypress="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].KeyPressHandler(this, event, '<%= ClientID %>');" />
                        <a id="<%= ClientID %>Search" title="Search" class="ektronWidgetFKGoButton" onclick=" Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].SearchImages();">
                            Go</a>
                    </div>
                    <ul class="Image-search ektronWidgetFKImages">
                    </ul>
                    <ul class="ektronWidgetFKButtonWrapper ektronWidgetFKSearchButtons">
                        <li><a id="<%= ClientID %>FirstSearch" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].SearchFirstImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonFirst" title="First" style="display: none;">
                            <span>First</span></a></li>
                        <li><a id="<%= ClientID %>PreviousSearch" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].SearchPreviousImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonPrevious" title="Previous" style="display: none;">
                            <span>Prev</span></a></li>
                        <li><span class="Image-search-result">No Results</span></li>
                        <li><a id="<%= ClientID %>NextSearch" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].SearchNextImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonNext" title="Next" style="display: none;">
                            <span>Next</span></a></li>
                        <li><a id="<%= ClientID %>LastSearch" onclick="Ektron.Widget.ITunePodcast.widgets['<%= ClientID %>'].SearchLastImages();"
                            class="ektronWidgetFKButton ektronWidgetFKButtonLast" title="Last" style="display: none;">
                            <span>Last</span></a></li>
                    </ul>
                    <asp:Button ID="btnAddSearch" runat="server" Text="Add to Collection" OnClick="btnAddSearch_Click" />
                    <br />
                </div>
                <div class="pane Collection" ms_positioning="GridLayout" style="overflow: auto; display: none;
                    position: relative; height: 485px;">
                    <div class="FKOptions">
                    </div>
                    <ul class="sortable boxy" id="ulSelected" style="width: 600px; height: <%=intHeight%>px">
                        <asp:Repeater ID="rptSelected" runat="server">
                            <ItemTemplate>
                                <li id="<%# Eval("TrackId")%>">
                                    <table cellpadding="1" cellspacing="1" border="0" width="100%">
                                        <tr>
                                            <td align="left" class="first_row" style="display: none;">
                                                <input type="checkbox" id="chkRequired1" runat="server" value='<%# Eval("TrackId") %>' checked='true' style="cursor: pointer; display: none;" name="chkRequired1" />
                                                <asp:Literal ID="ltquestionID" runat="server" Text='<%# Eval("TrackId") %>'></asp:Literal>
                                            </td>
                                            <td class="first_row" style="width: 20px;">
                                                <asp:CheckBox Style="cursor: default" runat="server" ID="uxchkRemove" />
                                            </td>
                                            <td class="first_row" align="left" valign="top" width="90px">
                                                <%# Eval("ImageLink")%>
                                            </td>
                                            <td style="width:600px; font-size:11px;">
                                                <label style="display:block; padding:0 0 6px 0;">
                                                    <%# Eval("TrackName") %></label>
                                                
                                                <strong>Collection Name:</strong> <%# Eval("CollectionName")%><br />
                                                <strong>Artist Name:</strong> <%# Eval("ArtistName")%>
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                    <asp:Label ID="uxNoDataAdded" runat="server"><br /><br />No podcast added.</asp:Label>
                </div>
                <div class="pane Property" style="display: none;">
                    <div class="FKOptions">
                    </div>
                    <table style="width: 99%;">
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
                                    <asp:ListItem Value="60">small</asp:ListItem>
                                    <asp:ListItem Value="100">large</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Name of the widget
                            </td>
                            <td>
                                <asp:TextBox ID="uxWdgetName" runat="server" MaxLength="50"></asp:TextBox>
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
