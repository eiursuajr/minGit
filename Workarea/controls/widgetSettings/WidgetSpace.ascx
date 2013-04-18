<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WidgetSpace.ascx.cs" Inherits="Workarea_controls_widgetSettings_WidgetSpace" %>

<script language="javascript" type="text/javascript">
var WidgetHandlerPath = "<asp:literal id="jsWidgetHandlerPath" runat="server"/>";

function VerifyWidgetsSpace(mode, id)
{
    if (mode == "remove")
    {
        if (confirm('<%=m_refMsg.GetMessage("js remove sel items confirm")%>'))
        {
            document.forms[0].action = "widgetsettings.aspx?action=widgetspace&mode=" + mode;
            document.forms[0].submit();
            return false;
        }
    }
    else
    {
        var title = document.getElementById('<%=txtTitle.ClientID%>');
        if (title.value == '')
        {
            alert('<%=m_refMsg.GetMessage("js: alert title required")%>');
            title.focus();
            return false;
        }
        var id = '<%=m_id%>';
        $ektron.ajax({
          url: WidgetHandlerPath + "WidgetSpaceHandler.ashx?action=" + mode + "&name=" + title.value + "&id=" + id,
          cache: false,
          success: function(html){
                if (html != null && html.indexOf("<error>") >= 0)
                {
                    html = html.replace("<error>", "");
                    html = html.replace("</error>", "");
                    alert(html);
                    return false;
                }
                document.forms[0].action = "widgetsettings.aspx?action=widgetspace&mode=" + mode + "&id=" + id;
                document.forms[0].submit();
            }
        });
    }
}
</script>

<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="ViewAll" runat="Server">
        <table class="ektronGrid">
            <tr class="title-header">
                <th style="width:1%; white-space:nowrap;">&#160;</th>
                <th title="ID" style="width:1%; white-space:nowrap"><%=m_refMsg.GetMessage("generic id")%></th>
                <th title="Title"><%=m_refMsg.GetMessage("generic title")%></th>
                <th title="Scope"><%=m_refMsg.GetMessage("generic scope")%></th>
            </tr>
            <asp:Repeater ID="ViewAllRepeater" runat="server">
                <ItemTemplate>
                    <tr class="row">
                        <td><asp:ImageButton ToolTip="Edit" ID="editButton" OnClick="editButton_Click" ImageUrl="../../images/UI/Icons/contentEdit.png" runat="server" CommandArgument="<%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>" /></td>
                        <td title="ID - <%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>"><%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%></td>
                        <td title="<%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Title%>"><%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Title%></td>
                        <td title="<%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Scope.ToString()%>"><%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Scope.ToString()%></td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="evenrow">
                        <td><asp:ImageButton ToolTip="Edit" ID="editButton" OnClick="editButton_Click" ImageUrl="../../images/UI/Icons/contentEdit.png" runat="server" CommandArgument="<%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>" /></td>
                        <td title="ID - <%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>"><%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%></td>
                        <td title="<%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Title%>"><%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Title%></td>
                        <td title="<%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Scope.ToString()%>"><%#(Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Scope.ToString()%></td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
    </asp:View>
    <asp:View ID="ViewAdd" runat="server">
        <div class="ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label"><asp:Label ID="lblWidgetsSpaceTitle" runat="Server" /></td>
                    <td class="value"><asp:TextBox ToolTip="Enter Widget Title here" ID="txtTitle" runat="server" /></td>
                </tr>
                <tr id="tr_groupSpace" runat="server">
                    <td class="label" title="Group Space"><asp:Literal ID="ltrGroupSpace" runat="Server" />:</td>
                    <td class="value">
                       <asp:RadioButton ToolTip="Open" ID="rdoGroupSpace" class="value" runat="server" GroupName="UserGroupSpace" />&nbsp;&nbsp;
                       <asp:RadioButton ToolTip="Restricted" ID="rdoUserSpace" runat="server" GroupName="UserGroupSpace" />
                    </td>
                </tr>
            </table>
            <div class="ektronTopSpace"></div>
            <div id="widgetDisplay">
                <fieldset>
                    <legend title="<%= lblSelectWidgets.Text%>"><asp:Literal ID="lblSelectWidgets" runat="server" /></legend>
                    <div class="widgetsHeader">
                        <h4 title="<%= widgetTitle.Text%>">
                            <asp:Literal ID="widgetTitle" runat="server" /></h4>
                        <ul id="widgetActions" class="buttonWrapper">
                            <li>
                                <asp:LinkButton ID="btnSelectNone" runat="server" CssClass="redHover button selectNoneButton"
                                    OnClientClick="UnselectAllWidgets();return false;" /></li>
                            <li>
                                <asp:LinkButton ID="btnSelectAll" runat="server" CssClass="greenHover button selectAllButton buttonRight"
                                    OnClientClick="SelectAllWidgets();return false;" /></li>
                        </ul>
                    </div>         
                    <div class="ektronTopSpace"></div>           
                    <div id="widgets">                        
                        <ul id="widgetList">
                            <asp:Repeater ID="repWidgetTypes" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div class="widget">
                                            <input type="checkbox" name="widget<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID %>" id="widget<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID %>" /><img
                                                src="<%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ControlURL + ".jpg"%>"
                                                alt="<%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title%>" title="<%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title%>" /><div class="widgetTitle" title="<%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title%>">
                                                    <%#(Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title%>
                                                </div>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </fieldset>
            </div>
            <asp:Label ID="lbStatus" runat="server" />
        </div>
    </asp:View>
    <asp:View ID="ViewRemove" runat="server">
        <div class="ektronPageInfo">
            <ul>
                <asp:Repeater ID="viewAllForRemove" runat="server">
                    <ItemTemplate>
                        <li><input type="checkbox" id="chkSpace<%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>" name="chkSpace<%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).ID%>" /> <%# (Container.DataItem as Ektron.Cms.Personalization.WidgetSpaceData).Title%></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </asp:View>
    <asp:View ID="ViewError" runat="Server">
        <asp:Label ID="lblMessage" runat="Server" />
    </asp:View>
</asp:MultiView>

<asp:Label ID="lblNoWidgetSpaces" Visible="false" runat="server" />
</div>
