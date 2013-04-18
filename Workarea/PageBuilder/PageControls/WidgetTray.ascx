<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WidgetTray.ascx.cs" Inherits="pagebuilder_PlaceHolder_WidgetTray" %>
<asp:Repeater ID="repWidgetTypes" runat="server">
    <HeaderTemplate>
        <a href="#" class="scrollLeft" onclick="Ektron.PageBuilder.WidgetTray.scrollLeft(); return false;">&nbsp;</a>
        <a href="#" class="scrollRight" onclick="Ektron.PageBuilder.WidgetTray.scrollRight(); return false;">&nbsp;</a>
        <div id="widgetlistWrapper">
            <ul class="ektronPersonalizationWidgetList widgetList">
    </HeaderTemplate>
    <ItemTemplate>
            <li id="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID %>-Widget" title="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" class="widgetToken">
                <img src="<%# RequestInformation.WidgetsPath + (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ControlURL %>.jpg" title="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" alt="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" />
                <span><%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ButtonText %></span>
                <input type="hidden" value="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID %>" />
            </li>
    </ItemTemplate>
    <FooterTemplate>
            </ul>
        </div>
    </FooterTemplate>
</asp:Repeater>