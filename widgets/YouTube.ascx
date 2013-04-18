<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YouTube.ascx.cs" Inherits="Widgets_YouTube" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server">
        <asp:View ID="View" runat="server">
            <asp:Label ID="lblData" runat="server"></asp:Label>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <a target="new" href="<%# (Container.DataItem as YouTubeWidgetRepeaterData).url %>">
                        <%# (Container.DataItem as YouTubeWidgetRepeaterData).contentTitle %>
                    </a><span id="open<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>">
                        <a onclick="$ektron('#open<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#close<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#<%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>content<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %>').slideDown('slow');">
                            &#9668;</a></span><span id="close<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>"
                                style="display: none;"><a onclick="$ektron('#open<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>').css('display', 'inline');$ektron('#close<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %><%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>').css('display', 'none');$ektron('#<%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>content<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %>').slideUp('slow');">
                                    &#9660;</a></span><br />
                    <div class="YouTube" id="<%# (Container.DataItem as YouTubeWidgetRepeaterData).hostid %>content<%# (Container.DataItem as YouTubeWidgetRepeaterData).count %>"
                        style="display: none; border: dashed 1px black; padding: 6px; margin-top: 6px;">
                        <%# (Container.DataItem as YouTubeWidgetRepeaterData).content %>
                    </div>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
            <table width="100%">
                <tr>
                    <td>
                        <div style="text-align: left;">
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClick="LinkButton2_Click">More</asp:LinkButton></div>
                    </td>
                    <td>
                        <div style="text-align: right;">
                            <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">Less</asp:LinkButton></div>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <asp:Repeater ID="Repeater2" runat="server" OnItemCommand="Repeater2_ItemCommand">
                <ItemTemplate>
                    <br />
                    <asp:LinkButton ID="LinkButton1" runat="server"><%# (Container.DataItem as YouTubeFeedPair).FeedName%></asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
                <asp:TextBox ID="KeywordSearchTextBox" runat="server" Width="218px" 
                style="margin-left: 14px"></asp:TextBox>
        </asp:View>
    </asp:MultiView>
</div>
