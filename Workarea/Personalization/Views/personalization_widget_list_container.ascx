<%@ Control Language="C#" AutoEventWireup="true" CodeFile="personalization_widget_list_container.ascx.cs" Inherits="WidgetControls_widget_list_container" %>
<%@ Register TagPrefix="MVC" TagName="WidgetList" Src="personalization_widget_list.ascx" %>

<table class="ektronWidgetPage" summary="Ektron Widgets">
    <tbody>
        <tr id="ektronWidgetList" class="widgetList">
            <asp:PlaceHolder ID="phWidgetLists" runat="server"></asp:PlaceHolder>
        </tr>
    </tbody>
</table>