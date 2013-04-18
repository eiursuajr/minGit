<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SyncDialogs.ascx.cs" Inherits="SyncDialogs" %>
<!-- Modal Dialog: Create Sync Relationship -->
<div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
    id="CreateSyncRelationshipModal">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
        <h3 class="ui-dialog-title header">
            <asp:Literal ID="lblCreateServerRelationship" runat="server"></asp:Literal>
            <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span id="spanCloseDialog"
                runat="server" class="ui-icon ui-icon-closethick"></span></a>
        </h3>
    </div>
    <div class="ektronModalBody clearfix" id="step1of3">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <h4>
                <strong>
                    <asp:Label ID="lblStep1of3" runat="server" /></strong>:
                <asp:Label ID="lblConnectToRemoteServer" runat="server" /></h4>
            <div class="messages">
            </div>
            <table class="ektronSyncTable" cellspacing="0">
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblRemoteServer" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtServerName" name="txtServerName" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblChooseCertificate" runat="server" />:
                    </th>
                    <td>
                        <select id="selectCertificate" name="selectCertificate" class="selectCertificate">
                            <option value="">
                                <asp:Literal ID="lblInitialOptionLoading" runat="server" /></option>
                        </select>
                    </td>
                </tr>
            </table>
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnConnect" runat="server" CssClass="greenHover button connectButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnCancelStep1" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
        </ul>
    </div>
    <div class="ektronModalBody" id="step2of3">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <h4>
                <strong>
                    <asp:Label ID="lblStep2of3" runat="server" /></strong>:
                <asp:Label ID="lblChooseCmsSite" runat="server" /></h4>
            <div class="messages">
            </div>
            <table class="ektronSyncTable" cellspacing="0">
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblRemoteServer2" runat="server" />:
                    </th>
                    <td>
                        <span id="RemoteServer"></span>
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblPortNumber2" runat="server" />:
                    </th>
                    <td>
                        <span id="PortNumber"></span>
                    </td>
                </tr>
            </table>
            <ul class="server" id="remoteSites">
            </ul>
            <input type="hidden" name="selectedRemoteSite" id="selectedRemoteSite" value="" autocomplete="off" />
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnNextStep2" runat="server" CssClass="blueHover button nextButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnCancelStep2" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnBackStep2" runat="server" CssClass="blueHover button backButton buttonRight" /></li>
        </ul>
    </div>
    <div class="ektronModalBody" id="step3of3">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <h4>
                <strong>
                    <asp:Label ID="lblStep3of3" runat="server" /></strong>:
                <asp:Label ID="lblConfigureSync" runat="server" /></h4>
            <div class="messages">
            </div>
            <fieldset>
                <div class="siteSelections">
                    <h4>
                        <asp:Literal ID="lblDatabaseCopiedFrom" runat="server" />:</h4>
                    <div id="copiedFromServerWrapper">
                        <ul class="server" id="directionRemoteServer">
                            <li class="selected last">
                                <h5>
                                    <span id="remoteDatabaseName"></span><span class="caption">(Remote)</span>
                                </h5>
                                <ul class="cmsInfo">
                                    <li>
                                        <asp:Label CssClass="serverInfoLabel" ID="lblRemoteSiteServerNameHeader" runat="server"></asp:Label>
                                        <span id="lblRemoteSiteServerName"></span></li>
                                    <li>
                                        <div class="selectRemoteMultiSite selectMultiSite">
                                            <table cellspacing="0">
                                                <tr>
                                                    <td class="cmsInfoHeader">
                                                        <label for="selectRemoteMultiSite">
                                                            <asp:Literal ID="lblForSelectRemoteMultiSite" runat="server" />
                                                        </label>
                                                    </td>
                                                    <td>
                                                        <select id="selectRemoteMultiSite" class="multiSiteDropDown" autocomplete="off">
                                                        </select>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                    <h4 class="replacingDatabase">
                        <asp:Literal ID="lblReplacingDatabase" runat="server" />:</h4>
                    <div id="replacingDatabaseServer">
                        <ul class="server" id="directionLocalServer">
                            <li class="last">
                                <input id="localSite" type="hidden" runat="server" class="localDatabaseName" />
                                <input id="localDatabaseServer" type="hidden" runat="server" class="localDatabaseServer" />
                                <input id="localServer" type="hidden" runat="server" class="localServerName" />
                                <h5>
                                    <span class="caption">(Local)</span>
                                    <asp:Literal ID="litLocalSiteDatabaseName" runat="server"></asp:Literal></h5>
                                <ul class="cmsInfo">
                                    <li>
                                        <asp:Label CssClass="serverInfoLabel" ID="lblLocalSiteServerNameHeader" runat="server"></asp:Label>
                                        <asp:Label ID="lblLocalSiteServerName" runat="server"></asp:Label>
                                    </li>
                                    <li>
                                        <div class="selectLocalMultiSite selectMultiSite">
                                            <table cellspacing="0">
                                                <tr>
                                                    <td class="cmsInfoHeader">
                                                        <label for="selectLocalMultiSite">
                                                            <asp:Literal ID="lblForSelectLocalMultiSite" runat="server" />
                                                        </label>
                                                    </td>
                                                    <td>
                                                        <select id="selectLocalMultiSite" class="multiSiteDropDown" runat="server" visible="false"
                                                            autocomplete="off">
                                                        </select>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="cmsInfoHeader">
                                                        <label for="selectLocalMultiSitePath">
                                                            <asp:Literal ID="lblForSelectLocalMultiSitePath" runat="server" />
                                                        </label>
                                                    </td>
                                                    <td>
                                                        <select id="selectLocalMultiSitePath" class="multiSitePathDropDown" autocomplete="off">
                                                        </select>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
                <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix directionInfo">
                    <li>
                        <asp:HyperLink ID="btnToggleCreateSyncDirection" runat="server" CssClass="blueHover button buttonRight syncDirectionButton" /><input
                            type="hidden" id="createSyncDirection" value="download" /></li>
                </ul>
            </fieldset>
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnCreate" runat="server" CssClass="greenHover button createButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnCancelStep3" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnBackStep3" runat="server" CssClass="blueHover button backButton buttonRight" /></li>
        </ul>
    </div>
</div>
<!-- Modal Dialog: Sync Status -->
<div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
    id="SyncStatusModal">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
        <h3 class="ui-dialog-title header">
            <span class="headerText">
                <asp:Literal ID="lblSyncStatus" runat="server" /></span> <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                    <span id="span1" runat="server" class="ui-icon ui-icon-closethick"></span>
            </a>
        </h3>
    </div>
    <div class="ektronModalBody">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <p class="messages">
            </p>
            <div class="syncStatusMessages">
            </div>
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnCloseSyncStatus" runat="server" CssClass="button buttonNoIcon buttonRight redHover" /></li>
        </ul>
    </div>
</div>
<!-- Modal Dialog: Confirm -->
<div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
    id="ConfirmDialog">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix  ektronModalHeader">
        <h3 class="ui-dialog-title header">
            <span class="headerText"></span><a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                <span id="span2" runat="server" class="ui-icon ui-icon-closethick"></span></a>
        </h3>
    </div>
    <div class="ektronModalBody">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <p class="messages">
            </p>
            <p class="messagesCaption">
            </p>
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnConfirmCancel" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnConfirmOk" runat="server" CssClass="greenHover button okButton buttonRight" /></li>
        </ul>
    </div>
</div>
<!-- Modal Dialog: Resolve Sync Collisions Dialog -->
<div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
    id="ResolveSyncCollisionsModal">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
        <h3 class="ui-dialog-title header">
            <span class="headerText">
                <asp:Literal ID="litResolveDialogTitle" runat="server"></asp:Literal></span>
            <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span id="span3"
                runat="server" class="ui-icon ui-icon-closethick"></span></a>
        </h3>
    </div>
    <div class="ektronModalBody" id="resolveSyncCollisionsDialogBody">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo">
            <div id="resolveSyncCollisionsMessages" class="messages">
            </div>
            <asp:Literal ID="litResolveDialogMessage" runat="server"></asp:Literal>
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnNextResolveSyncCollisions" runat="server" CssClass="greenHover button resolveSyncCollisionsButton buttonRight" /></li>
            <li>
                <asp:HyperLink ID="btnCancelResolveSyncCollisions" runat="server" CssClass="redHover button cancelResolveSyncCollisionsButton buttonRight" /></li>
        </ul>
    </div>
</div>
<!-- Modal Dialog: Create Sync Cloud Relationship -->
<div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
    id="CreateCloudRelationshipModal">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
        <h3 class="ui-dialog-title header">
            <asp:Literal ID="lblCreateCloudRelationship" runat="server"></asp:Literal>
            <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span id="spanCloseCloudDialog"
                runat="server" class="ui-icon ui-icon-closethick"></span></a>
        </h3>
    </div>
    <div class="ektronModalBody clearfix" id="cloudStep1">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo createCloud">
            <h4>
                <strong>
                    <asp:Label ID="lblCloudStep1" runat="server" /></strong>:
                <asp:Label ID="lblConnectToCloudServer" runat="server" /></h4>
            <div class="messages">
            </div>
            <table class="ektronSyncTable" cellspacing="0">
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblCloudSQLServer" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtSQLServer" name="txtSQLServer" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblIPAddress" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtIPAddress" name="txtIPAddress" class="ektronTextSmall serverIPAddress"
                            size="35" runat="server"/>
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblBlobStorage" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtBlobStorage" name="txtBlobStorage" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblAccountName" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtAccountName" name="txtAccountName" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblContainerName" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtContainerName" name="txtBlobStoragetxtContainerName" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblAccountKey" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtAccountKey" name="txtAccountKey" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
                <tr>
                    <th class="txtRight">
                        <asp:Label ID="lblCloudDomain" runat="server" />:
                    </th>
                    <td>
                        <input type="text" id="txtCloudDomain" name="txtCloudDomain" class="ektronTextSmall"
                            size="35" />
                    </td>
                </tr>
            </table>
            <input id="localDatabaseNameCloud" type="hidden" runat="server" class="localDatabaseNameCloud" />
            <input id="localDatabaseServerCloud" type="hidden" runat="server" class="localDatabaseServerCloud" />
            <input id="localServerNameCloud" type="hidden" runat="server" class="localServerNameCloud" />
        </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <li>
                <asp:HyperLink ID="btnCloudConnect" runat="server" CssClass="greenHover button connectButton buttonRight btnCloudConnect" /></li>
            <li>
                <asp:HyperLink ID="btnCancelStep1Cloud" runat="server" CssClass="redHover button cancelButton buttonRight btnCloudCancel" /></li>
        </ul>
    </div>
</div>
