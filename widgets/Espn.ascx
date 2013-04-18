<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Espn.ascx.cs" Inherits="Widgets_Espn" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <asp:Label ID="lblData" runat="server"></asp:Label>
            <asp:Repeater ID="Repeater2" runat="server">
                <ItemTemplate>
                    <a target="new" href="<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).url %>">
                        <%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).contentTitle%>
                    </a><span id="open<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>">
                        <a onclick="$ektron('#open<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#close<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %>').slideDown('slow');">
                            ◄</a></span><span id="close<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>"
                                style="display: none;"><a onclick="$ektron('#open<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#close<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %><%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %>').slideUp('slow');">
                                    ▼</a></span><br />
                    <div class="newsEntry" id="<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).hostid %>content<%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).count %>"
                        style="display: none; border: dashed 1px black; padding: 6px; margin-top: 6px;">
                        <%# (Container.DataItem as EspnNewsFeedWidgetRepeaterData).content%>
                    </div>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit">
                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server"><%# (Container.DataItem as EspnNewsFeedPair).FeedName%></asp:LinkButton><br />
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
            </div>
        </asp:View>
    </asp:MultiView>
</div>
