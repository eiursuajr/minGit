<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncProfile.aspx.cs" Inherits="SyncProfile" %>

<%@ Register Src="SyncSchedule.ascx" TagPrefix="ektron" TagName="SyncSchedule" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link type="text/css" href="css/ektron.workarea.sync.profile.css" rel="stylesheet" />
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />
    <link type="text/css" href="../java/plugins/modal/ektron.modal.css" rel="stylesheet" />
    <script type="text/javascript">
        Ektron.ready(function () {
            Ektron.Workarea.Sync.Relationships.Init();
            Ektron.Workarea.Sync.Profile.Init();
            Ektron.Workarea.Sync.Schedule.Init();
        });

    </script>
</head>
<body>
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncResources" runat="server" />
    <form id="form1" runat="server">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
            <table>
                <tr id="rowToolbarButtons" runat="server">
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer" id="divSyncTabs" runat="server">
        <asp:Panel ID="pnlProfile" runat="server">
            <div class="errorMessage" id="divErrorMessage" runat="server">
            </div>
            <table id="tblProfile" runat="server">
                <tr>
                    <td class="label">
                        <asp:Label ID="lblScheduleName" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:TextBox ToolTip="Enter Name here" CssClass="textInput" ID="txtScheduleName"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblLocalSite" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:TextBox ToolTip="Enter Locale Site here" CssClass="textInput" ID="txtLocalSite"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblRemoteSite" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:TextBox ToolTip="Enter Remote Site here" CssClass="textInput" ID="txtRemoteSite"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trMultiSiteRow" runat="server">
                    <td class="label">
                        <asp:Label ID="lblMultiSiteFolder" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:TextBox ToolTip="Enter Multisite Folder here" CssClass="textInput" ID="txtMultiSiteFolder"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblItemsToSynchronize" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <table>
                            <tr>
                                <td>
                                    <ul id="ulItemsToSynchronize">
                                        <li>
                                            <asp:CheckBox ID="chkDatabase" runat="server" />
                                            <ul>
                                                <li>
                                                    <asp:CheckBoxList ID="cbScope" runat="server" CssClass="dbscopeitems" EnableViewState="false">
                                                    </asp:CheckBoxList>
                                                </li>
                                            </ul>
                                        </li>
                                        <li>
                                            <asp:CheckBox ID="chkWorkarea" runat="server" />
                                        </li>
                                        <li>
                                            <asp:CheckBox ID="chkTemplates" runat="server" />
                                            <ul>
                                                <li>
                                                    <asp:CheckBox ID="chkBinaries" runat="server" />
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </td>
                                <td>
                                    <div class="scopeitemswrapper">
                                        <span class="scopeitemstitle"><%=apiContent.EkMsgRef.GetMessage("lbl Tables scope")%></span><span class="scopeitemsclose"><%=apiContent.EkMsgRef.GetMessage("close title")%></span>
                                        <ul class="scopeitemslist">
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblFilters" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <table>
                            <tr>
                                <td class="filterOptions">
                                    <asp:RadioButtonList ToolTip="Select to Include/Exclude Files" ID="rdoFilters" runat="server">
                                        <asp:ListItem Value="None"></asp:ListItem>
                                        <asp:ListItem Value="Include"></asp:ListItem>
                                        <asp:ListItem Value="Exclude"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    <table class="filters">
                                        <tr>
                                            <td>
                                                <div>
                                                    <asp:Label ID="lblFileFilterHeader" runat="server"></asp:Label></div>
                                                <asp:TextBox ToolTip="Enter list of file extensions seperated by commas" ID="txtFileFilters"
                                                    runat="server"></asp:TextBox>
                                                <div id="divFileFilterDesc" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <asp:Label ID="lblDirectoryFilterHeader" runat="server"></asp:Label></div>
                                                <asp:TextBox ToolTip="Enter list of directories seperated by commas" ID="txtDirectoryFilters"
                                                    runat="server"></asp:TextBox>
                                                <div id="divDirectoryFilterDesc" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblSyncDirection" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:RadioButtonList ToolTip="Select Synchronization Direction" ID="rdoDirection"
                            runat="server">
                            <asp:ListItem Value="Bidirectional"></asp:ListItem>
                            <asp:ListItem Value="Upload"></asp:ListItem>
                            <asp:ListItem Value="Download"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblConflictResolution" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <table>
                            <tr>
                                <td>
                                    <asp:RadioButtonList ToolTip="Select the Conflict Resolution Policy" ID="rdoConflictResolution"
                                        runat="server">
                                        <asp:ListItem Value="DestinationWins"></asp:ListItem>
                                        <asp:ListItem Value="SourceWins" Selected="True" ></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    <div class="optionDescription">
                                        <asp:Label ID="lblDestinationWinsDesc" runat="server"></asp:Label>
                                        <asp:Label ID="lblSourceWinsDesc" runat="server"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trRestoration" runat="server" visible="false">
                    <td class="label">
                        <asp:Label ID="lblRestoration" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <asp:CheckBox ID="chkRestoration" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblSchedule" runat="server"></asp:Label>
                    </td>
                    <td class="option">
                        <ektron:SyncSchedule ID="ssScheduleOptions" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <input type="hidden" id="hdnDisplayMode" runat="server" />
    <ektron:SyncDialogs ID="syncDialogs" runat="server" />
    </form>
</body>
</html>
