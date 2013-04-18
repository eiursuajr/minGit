<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RSSFeed.ascx.cs" Inherits="Widgets_RSSFeed" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <asp:Label ID="lblData" runat="server"></asp:Label>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <a target="new" href="<%# (Container.DataItem as RSSFeedWidgetRepeaterData).url %>">
                        <%# (Container.DataItem as RSSFeedWidgetRepeaterData).contentTitle %>
                    </a><span id="open<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>">
                        <a onclick="$ektron('#open<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#close<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#<%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %>').slideDown('slow');">
                            ◄</a></span><span id="close<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>"
                                style="display: none;"><a onclick="$ektron('#open<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#close<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %><%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#<%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %>').slideUp('slow');">
                                    ▼</a></span><br />
                    <div class="rssItem" id="<%# (Container.DataItem as RSSFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as RSSFeedWidgetRepeaterData).count %>"
                        style="display: none; border: dashed 1px black; padding: 6px; margin-top: 6px;">
                        <%# (Container.DataItem as RSSFeedWidgetRepeaterData).content %>
                    </div>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit">
                <table style="width: 99%;">
                    <tr>
                        <td>
                            Feed URL:
                        </td>
                        <td>
                            <asp:TextBox ID="feedURL" runat="server" Style="width: 95%;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
						    <asp:Button ID="CancelButton" CssClass="RSSCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:View>
    </asp:MultiView>
</div>
