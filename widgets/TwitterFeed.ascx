<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TwitterFeed.ascx.cs" Inherits="widgets_TwitterFeed" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!-- Google Ajax Api -->
<script src="http://www.google.com/jsapi?key=notsupplied-wizard" type="text/javascript"></script>
<!-- Dynamic Feed Control and Stylesheet -->
<script src="http://www.google.com/uds/solutions/dynamicfeed/gfdynamicfeedcontrol.js" type="text/javascript"></script>

<div class="twit">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <!--  Twitter RSS feed starts here  -->
            <h3 class="header pink">
                Twitter</h3>
            <div class="message_content">
                <div class="todays_events_text">
                    <div id="feed-control<%=ClientID%>">
                        <span class="nofeeds">No Feeds...</span>
                    </div>

                    <script type="text/javascript">
                        // Load the feeds API and set the onload callback.
                        google.load("feeds", '1');
                    </script>

                    <!-- ++End Dynamic Feed Control Wizard Generated Code++ -->
                </div>
            </div>
            <!-- Twitter Feed Ends -->
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <asp:UpdatePanel ID="updatepnl" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
            <div id="<%=ClientID%>" class="TFWidget">
                <fieldset id="SelectedList">
                    <legend><strong>Twitter Feeds</strong></legend>
                    <asp:Repeater ID="repFriends" runat="server" OnItemDataBound="repFriends_ItemDataBound"
                        OnItemCommand="repFriends_ItemCommand">
                        <HeaderTemplate>
                            <ul class="EkActivityFeedPreferenceList clearfix">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="PreferenceListItem"><span class="name">
                                <asp:LinkButton ID="btnUp" CssClass="PreferenceListUp" CommandName="up" CommandArgument="<%# Container.ItemIndex %>"
                                    runat="server" ToolTip="Up">
                        
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDown" CssClass="PreferenceListDown" CommandName="down" CommandArgument="<%# Container.ItemIndex %>"
                                    runat="server" ToolTip="Down">
                       
                                </asp:LinkButton>
                                <label class="feedblock">
                                    Feed Name:</label>
                                <%# DataBinder.Eval(Container.DataItem, "Name")%>
                                <label class="feedblock">
                                    Feed Url:</label>
                                <%# DataBinder.Eval(Container.DataItem, "URL")%>
                            </span>
                                <asp:LinkButton ID="btnDelete" CssClass="PreferenceListDel" CommandName="btnDelete"
                                    CommandArgument="<%# Container.ItemIndex %>" runat="server" ToolTip="Delete">
                        <span>Delete</span>
                                </asp:LinkButton>
                                <br />
                            </li>
                            <br />
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblfeed" AssociatedControlID="feed" runat="server">Name:</asp:Label>
                    <input type="text" name="feed" id="feed" class="textbox" size="33" runat="server" /><br />
                    <asp:Label ID="lblurl" AssociatedControlID="url" runat="server">Url:</asp:Label>
                    <input type="text" name="url" id="url" class="textbox" size="36" runat="server" /><br />
                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" OnClientClick="Ektron.Widgets.Twitter.AddTwit(this.id);" />
                </fieldset>
                <div class="CBEditControls">
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                    <asp:Button ID="SaveButton" CssClass="CBSave" runat="server" Text="Save" OnClick="SaveButton_Click" />
                </div>
            </div>
            </ContentTemplate>
            </asp:UpdatePanel>
        </asp:View>
    </asp:MultiView>
    <div class='HiddenTBData'>
        <asp:HiddenField ID="tbData" runat="server" />
    </div>
</div>
