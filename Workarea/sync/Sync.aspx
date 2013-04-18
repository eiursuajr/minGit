<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Sync.aspx.cs" Inherits="Sync" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Relationship List Sample</title>
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.relationships.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />
    <link type="text/css" href="../java/plugins/modal/ektron.modal.css" rel="stylesheet" />
    
    <script type="text/javascript">
        Ektron.ready(function() {
            Ektron.Workarea.Sync.Relationships.Init();
        });
    </script>
</head>
<body>    
    <!-- Ektron Client Script -->
    <asp:Literal id="ektronClientScript" runat="server"></asp:Literal>
    
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncClientResources" runat="server" />
    
    <form id="form1" runat="server">        
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
                <table>
                    <tr id="rowToolbarButtons" runat="server"></tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer" id="divSyncTabs" runat="server">
            <div>
                <asp:Literal ID="litNoRelationshipsMessage" runat="server"></asp:Literal>
                <asp:Repeater ID="rptRelationshipList" runat="server" OnItemDataBound="HandleRelationshipListItemDataBound">
                    <ItemTemplate>
                        <ul class="relationshipList">
                            <li id="liRelationship" runat="server" class="relationship">
                                <div class="relationshipInfo">
                                    <div class="syncButtons" id="divRelationshipButtons" runat="server">
                                    </div>
                                    <h5><asp:Literal ID="litDatabaseName" runat="server"></asp:Literal></h5>
                                    <ul>
                                        <li><asp:Label CssClass="relationshipLabel" ID="lblServerName" runat="server"></asp:Label> <asp:Literal ID="litServerName" runat="server"></asp:Literal></li>
                                        <li><asp:Label CssClass="relationshipLabel" ID="lblLocalSite" runat="server"></asp:Label> <asp:Literal ID="litLocalSite" runat="server"></asp:Literal></li>
                                        <li><asp:Label CssClass="relationshipLabel" ID="lblRemoteSite" runat="server"></asp:Label> <asp:Literal ID="litRemoteSite" runat="server"></asp:Literal></li>
                                    </ul>
                                </div>
                            </li>
                            <li id="profilesListItem" runat="server" class="profiles">
                                <table class="profileList">
                                    <tr>
                                        <th><asp:Literal ID="litProfileHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litProfileIdHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litScheduleHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litLastRunTimeHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litLastRunResultHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litCurrentStatusHeader" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="litButtonsHeader" runat="server"></asp:Literal></th>
                                    </tr>
                                    <asp:Repeater ID="rptProfileList" runat="server" OnItemDataBound="HandleProfileListItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="profileName">
                                                    <a href="#" id="linkProfileName" runat="server"></a>
                                                </td>
                                                 <td class="profileId">
                                                     <asp:Literal ID="litProfileId" runat="server"></asp:Literal>
                                                </td>
                                                <td class="schedule">
                                                    <asp:Literal ID="litSchedule" runat="server"></asp:Literal>
                                                    <p class="nextRunTime"><asp:Literal ID="litNextRunTime" runat="server"></asp:Literal></p>
                                                </td>
                                                <td class="lastRunTime">
                                                    <asp:Literal ID="litLastRunTime" runat="server"></asp:Literal>
                                                </td>
                                                <td class="lastRunResult">
                                                    <asp:Literal ID="litLastRunResult" runat="server"></asp:Literal>
                                                </td>
                                                <td class="currentStatus">
                                                    <asp:Literal ID="litCurrentStatus" runat="server"></asp:Literal>
                                                </td>
                                                <td class="profileButtons">
                                                    <div class="syncButtons" id="divProfileButtons" runat="server">
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </li>
                        </ul>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <ektron:SyncDialogs ID="syncDialogs" runat="server" />
    </form>
</body>
</html>
