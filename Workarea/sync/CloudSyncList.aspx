<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CloudSyncList.aspx.cs" Inherits="Workarea_sync_CloudSyncList" %>

<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cloud Synchronization</title>
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
    <link type="text/css" href="css/ektron.workarea.sync.profile.css" rel="stylesheet" />
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.relationships.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />
    <link type="text/css" href="../java/plugins/modal/ektron.modal.css" rel="stylesheet" />
    <script type="text/javascript">
        Ektron.ready(function () {
            Ektron.Workarea.Sync.Relationships.Init();
        });
    </script>
</head>
<body>
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncClientResources" runat="server" />
    <form id="form1" runat="server">
    <div class="ektronPageHeader">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
            <table>
                <tr id="rowToolbarButtons" runat="server">
                    <%--<asp:LinkButton  ID="lbSync" runat="server" style="cursor: default;" onmouseout="HideTransString();RollOut(this);" onmouseover="ShowTransString('Sync Now');RollOver(this);" OnClientClick="alert('Cloud Sync');" ><img class="button" id="image_100" src="/WorkArea/images/ui/icons/sync.png" onclick="SelectButton(this);"></asp:LinkButton>--%>
                    <td id="image_cell_101" class="button" title="Sync Now">
                        <%--<asp:LinkButton ID="image_link_101" runat="server" OnClick="SyncCloudClick" OnClientClick="Ektron.Workarea.Overlay.block();" onMouseOver="ShowTransString('Sync Now');RollOver(this);" onMouseOut="HideTransString();RollOut(this);" style="cursor: default;">
                            <img onClick="SelectButton(this);" src="/WorkArea/images/ui/icons/sync.png" id="image_101" class="button" />
                        </asp:LinkButton>--%>
                        <%--<a class="syncButton" title="Sync" href="CloudSync.aspx?referrer=CloudSyncList.aspx">
                            <img src="<%=_siteApi.AppPath +"/images/ui/icons/sync.png" %>" id="image_101" class="button" />
                        </a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer">
        <asp:Repeater ID="rptRelationshipList" runat="server">
            <ItemTemplate>
                <ul class="relationshipList">
                    <li id="liRelationship" runat="server" class="relationship upload">
                        <div class="relationshipInfo">
                            <div class="syncButtons" id="divRelationshipButtons" runat="server">
                                <a onclick="Ektron.Workarea.Sync.Relationships.InitialSynchronize(this);" class="syncButton" rel="<%# Eval("Id") %>" title="Sync" href="#"></a>
                                <a href="#" title="Get Status" rel="<%# Eval("Id") %>" class="statusButton" onclick="Ektron.Workarea.Sync.Relationships.ShowSyncStatus(this);"></a>
                            </div>
                            <h5><a href="CloudSync.aspx?referrer=CloudSyncList.aspx"><%# Eval("Name") %></a></h5>
                            <ul>
                                <li><span>Local Site Path : </span>
                                    <%# Eval("LocalSite.SitePath")%></li>
                                <li><span>Local Database : </span>
                                    <%# Eval("LocalSite.Connection.ServerName")%></li>
                                <li><span>Remote Database : </span>
                                    <%# Eval("RemoteSite.Connection.ServerName")%></li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
    <ektron:SyncDialogs ID="syncDialogs" runat="server" />
</body>
</html>
