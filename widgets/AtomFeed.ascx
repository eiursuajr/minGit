<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AtomFeed.ascx.cs" Inherits="Widgets_AtomFeed" %>

<div style="padding: 12px;" class="atom">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <asp:Label ID="lblData" runat="server"></asp:Label>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <a target="new" href="<%# (Container.DataItem as AtomFeedWidgetRepeaterData).url %>">
                        <%# (Container.DataItem as AtomFeedWidgetRepeaterData).contentTitle %>
                    </a><span id="open<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>">
                        <a onclick="$ektron('#open<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#close<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#<%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %>').slideDown('slow');">
                            &#9668;</a></span><span id="close<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>"
                                style="display: none;"><a onclick="$ektron('#open<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#close<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %><%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#<%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %>').slideUp('slow');">
                                    &#9660;</a></span><br />
                    <div class="atomEntry" id="<%# (Container.DataItem as AtomFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as AtomFeedWidgetRepeaterData).count %>"
                        style="display: none; border: dashed 1px black; padding: 6px; margin-top: 6px;">
                        <%# (Container.DataItem as AtomFeedWidgetRepeaterData).content %>
                    </div>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div class="atomFeedEdit"  id="<%=ClientID%>_edit">
                <table style="width: 99%">
                    <tr>
                        <td>
                            Feed URL:
                        </td>
                        <td>
                            <asp:TextBox ID="feedURL" runat="server" Style="width: 95%;"  CssClass="feedURL"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>
                            Number of Posts:
                        </td>
                        <td>
                            <asp:TextBox ID="numPosts" runat="server" Style="width: 95%;"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <div class="atomFeedButton">
								<asp:Button ID="CancelButton" CssClass="AFCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                                <asp:Button ID="SaveButton"  CssClass="atomFeedSaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" /></div>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="atomFeedSaving" style="display: none;">
                Saving Widget ...
            </div>
        </asp:View>
    </asp:MultiView>
</div>
