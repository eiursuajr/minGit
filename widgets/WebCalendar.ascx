<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WebCalendar.ascx.cs" Inherits="widgets_WebCalendar" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/WebCalendar/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>

<asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
    <asp:View ID="View" runat="server">
    <asp:Label CssClass="WarningNoEdit" ID="WarningNoEdit" runat="server" Visible="false" />
        <CMS:WebCalendar ID="calendar" runat="server"></CMS:WebCalendar>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit" class="WCEdit">
            <ul class="WCTabWrapper">
                <li class="WCTab selected"><a href="#<%=ClientID%>SelectedCalendars"><span>Selected Calendars</span></a></li>
                <li class="WCTab"><a href="#<%=ClientID%>ByFolder"><span>Folder</span></a></li>
                <li class="WCTab"><a href="#<%=ClientID%>ByUser"><span>User</span></a></li>
                <li class="WCTab"><a href="#<%=ClientID%>ByGroup"><span>Group</span></a></li>
            </ul>
            <div id="<%=ClientID%>SelectedCalendars" class="SelectedCalendars">
                <table>
                    <tr>
                        <th>Source ID</th>
                        <th>Name</th>
                        <th>Event Color</th>
                        <th></th>
                    </tr>
                    <tr class="selectedtemplate" style="display:none;">
                        <td class="id">{type}-{id}</td>
                        <td class="name">{name}</td>
                        <td class="bgcolor">
                            <select class="bgcolor" id="bgcolor_uniqueid">
                                <option value="AutoSelect" selected="selected" title="<%=SitePath %>widgets/WebCalendar/images/colors/AutoSelect.png">AutoSelect</option>
                                <option value="Blue" title="<%=SitePath %>widgets/WebCalendar/images/colors/Blue.png">Blue</option>
                                <option value="DarkBlue" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkBlue.png">DarkBlue</option>
                                <option value="DarkGreen" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkGreen.png">DarkGreen</option>
                                <option value="DarkRed" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkRed.png">DarkRed</option>
                                <option value="Green" title="<%=SitePath %>widgets/WebCalendar/images/colors/Green.png">Green</option>
                                <option value="Orange" title="<%=SitePath %>widgets/WebCalendar/images/colors/Orange.png">Orange</option>
                                <option value="Pink" title="<%=SitePath %>widgets/WebCalendar/images/colors/Pink.png">Pink</option>
                                <option value="Red" title="<%=SitePath %>widgets/WebCalendar/images/colors/Red.png">Red</option>
                                <option value="Violet" title="<%=SitePath %>widgets/WebCalendar/images/colors/Violet.png">Violet</option>
                                <option value="Yellow" title="<%=SitePath %>widgets/WebCalendar/images/colors/Yellow.png">Yellow</option>
                            </select>
                        </td>
                        <td><span class="click remove" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.RemoveSource(this);">X</span></td>
                    </tr>
                <asp:Repeater ID="sourcerepeater" runat="server" OnItemDataBound="sourcerepeater_databound">
                    <ItemTemplate>
                    <tr class="source">
                        <td class="id"><asp:Literal ID="itemID" runat="server"></asp:Literal></td>
                        <td class="name"><asp:Literal ID="itemName" runat="server"></asp:Literal></td>
                        <td class="bgcolor">
                            <select class="bgcolor" id="<%=ClientID%>_<%# DataBinder.Eval(Container.DataItem, "defaultId")%>" which="<%# DataBinder.Eval(Container.DataItem, "backColor")%>">
                                <option value="AutoSelect" selected="selected" title="<%=SitePath %>widgets/WebCalendar/images/colors/AutoSelect.png">AutoSelect</option>
                                <option value="Blue" title="<%=SitePath %>widgets/WebCalendar/images/colors/Blue.png">Blue</option>
                                <option value="DarkBlue" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkBlue.png">DarkBlue</option>
                                <option value="DarkGreen" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkGreen.png">DarkGreen</option>
                                <option value="DarkRed" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkRed.png">DarkRed</option>
                                <option value="Green" title="<%=SitePath %>widgets/WebCalendar/images/colors/Green.png">Green</option>
                                <option value="Orange" title="<%=SitePath %>widgets/WebCalendar/images/colors/Orange.png">Orange</option>
                                <option value="Pink" title="<%=SitePath %>widgets/WebCalendar/images/colors/Pink.png">Pink</option>
                                <option value="Red" title="<%=SitePath %>widgets/WebCalendar/images/colors/Red.png">Red</option>
                                <option value="Violet" title="<%=SitePath %>widgets/WebCalendar/images/colors/Violet.png">Violet</option>
                                <option value="Yellow" title="<%=SitePath %>widgets/WebCalendar/images/colors/Yellow.png">Yellow</option>
                            </select>
                        </td>
                        <td><span class="click remove" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.RemoveSource(this);">X</span></td>
                    </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                    <tr class="source even">
                        <td class="id"><asp:Literal ID="itemID" runat="server"></asp:Literal></td>
                        <td class="name"><asp:Literal ID="itemName" runat="server"></asp:Literal></td>
                        <td class="bgcolor">
                            <select class="bgcolor" id="<%=ClientID%>_<%# DataBinder.Eval(Container.DataItem, "defaultId")%>" which="<%# DataBinder.Eval(Container.DataItem, "backColor")%>">
                                <option value="AutoSelect" selected="selected" title="<%=SitePath %>widgets/WebCalendar/images/colors/AutoSelect.png">AutoSelect</option>
                                <option value="Blue" title="<%=SitePath %>widgets/WebCalendar/images/colors/Blue.png">Blue</option>
                                <option value="DarkBlue" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkBlue.png">DarkBlue</option>
                                <option value="DarkGreen" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkGreen.png">DarkGreen</option>
                                <option value="DarkRed" title="<%=SitePath %>widgets/WebCalendar/images/colors/DarkRed.png">DarkRed</option>
                                <option value="Green" title="<%=SitePath %>widgets/WebCalendar/images/colors/Green.png">Green</option>
                                <option value="Orange" title="<%=SitePath %>widgets/WebCalendar/images/colors/Orange.png">Orange</option>
                                <option value="Pink" title="<%=SitePath %>widgets/WebCalendar/images/colors/Pink.png">Pink</option>
                                <option value="Red" title="<%=SitePath %>widgets/WebCalendar/images/colors/Red.png">Red</option>
                                <option value="Violet" title="<%=SitePath %>widgets/WebCalendar/images/colors/Violet.png">Violet</option>
                                <option value="Yellow" title="<%=SitePath %>widgets/WebCalendar/images/colors/Yellow.png">Yellow</option>
                            </select>
                        </td>
                        <td><span class="click remove" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.RemoveSource(this);">X</span></td>
                    </tr>
                    </AlternatingItemTemplate>
                </asp:Repeater>
                <asp:literal ID="nosources" runat="server" Visible="false"><tr class="nosources center"><td colspan="4">No sources selected</td></tr></asp:literal>
                </table>
            </div>
            <div id="<%=ClientID%>ByFolder" class="WebCalByFolder">
                <UC:FolderTree ID="foldertree" runat="server" />
            </div>
            <div id="<%=ClientID%>ByUser" class="WebCalByUser">
                <div class="mycalendar" id="divMyCalendar" runat="server">
                    <input type="button" class="Execute" causesvalidation="false" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.AddSource('User', <%=LoggedInUserId %>, '<%=LoggedInUserName %>');" value="Add My Calendar" />
                </div>
                <div class="searcharea">
                    <input type="text" class="UserName" />
                    <input type="button" class="Execute" causesvalidation="false" onclick="Ektron.PFWidgets.WebCalendar.User.UserSearch(this, 0);" value="Search Users" />
                </div>
                <br class="clear" />
                <table>
                    <tr>
                        <th>Avatar</th>
                        <th>Name</th>
                        <th>Last Name</th>
                        <th>Email</th>
                        <th></th>
                    </tr>
                    <tr class="enterSearchTerm center">
                        <td colspan="5">Please enter a search term above.</td>
                    </tr>
                    <tr class="noResults center" style="display:none;">
                        <td colspan="5">No results.</td>
                    </tr>
                    <tr class="Searching center" style="display:none;">
                        <td colspan="5">Searching...</td>
                    </tr>
                    <tr class="resultsTemplate" style="display:none;">
                        <td>usericon</td>
                        <td>fname</td>
                        <td>lname</td>
                        <td>email</td>
                        <td><span class="add" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.AddSource('User', templid, 'fname lname');">Add calendar</span></td>
                    </tr>
                </table>
                <div class="paging">
                    <span><span class="prev" style="display:none;">&lt;&lt;</span></span>
                    <span><span class="next" style="display:none;">&gt;&gt;</span></span>
                </div>
            </div>
            <div id="<%=ClientID%>ByGroup" class="WebCalByGroup">
                <div class="searcharea">
                    <input type="text" class="GroupName" />
                    <input type="button" class="Execute" causesvalidation="false" onclick="return Ektron.PFWidgets.WebCalendar.Group.GroupSearch(this, 0);" value="Search Groups" />
                </div>
                <br class="clear" />
                <table>
                    <tr>
                        <th>Avatar</th>
                        <th>Group Name</th>
                        <th></th>
                    </tr>
                    <tr class="enterSearchTerm center">
                        <td colspan="3">Please enter a search term above.</td>
                    </tr>
                    <tr class="noResults center" style="display:none;">
                        <td colspan="3">No results.</td>
                    </tr>
                    <tr class="Searching center" style="display:none;">
                        <td colspan="3">Searching...</td>
                    </tr>
                    <tr class="resultsTemplate" style="display:none;">
                        <td>groupicon</td>
                        <td>
                            <div class="groupname">gname</div>
                            <div class="groupdesc">gdesc</div>
                        </td>
                        <td><span class="add" onclick="return Ektron.PFWidgets.WebCalendar.DataStore.AddSource('Group', templid, 'gname');">Add calendar</span></td>
                    </tr>
                </table>
                <div class="paging">
                    <span><span class="prev" style="display:none;">&lt;&lt;</span></span>
                    <span><span class="next" style="display:none;">&gt;&gt;</span></span>
                </div>
            </div>
            <div class="WCEditControls">
				<asp:Button ID="CancelButton" CssClass="WCCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                <asp:Button ID="SaveButton" CssClass="WCSave" runat="server" OnClientClick="Ektron.PFWidgets.WebCalendar.DataStore.UpdateSaveString();" OnClick="SaveButton_Click" Text="Save" />
            </div>
            <asp:TextBox ID="CalendarList" CssClass="HiddenWCData" runat="server" Style="display: none;" ToolTip="format: id-type-color:id-type-color"></asp:TextBox>
        </div>
    </asp:View>
</asp:MultiView>