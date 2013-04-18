<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DmsSync.aspx.cs" Inherits="Workarea_sync_DmsSync" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
    <link type="text/css" href="css/ektron.workarea.sync.profile.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />   
    
    <script type="text/javascript">
        Ektron.ready(function() {
            Ektron.Workarea.Sync.Relationships.Init();
            Ektron.Workarea.Sync.DmsSync.Init();

            var contentLanguage = $ektron("#hdnContentLanguage").attr("value");
            var contentAssetId = $ektron("#hdnContentAssetId").attr("value");
            var contentAssetVersion = $ektron("#hdnContentAssetVersion").attr("value");
            var contentId = $ektron("#hdnContentId").attr("value");
            var folderId = $ektron("#hdnFolderId").attr("value");
            var isMultiSite = $ektron("#hdnIsMultisite").attr("value");
            var showDialog = $ektron("#hdnShowDialog").attr("value");

            if (showDialog == "true") {
                Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(
                    contentLanguage,
                    contentId,
                    contentAssetId,
                    contentAssetVersion,
                    folderId,
                    isMultiSite);

                Ektron.Workarea.Sync.DmsSync.LayoutDialogs();
            }
        });
    </script> 
</head>
<body>
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncClientResources" runat="server" />
    
    <form id="form1" runat="server">  
        <asp:Panel ID="pnlMessage" runat="server">
            <div class="errorMessage">
                <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlDmsSync" runat="server">
            <!-- Modal Dialog: Show Sync Configurations Modal -->
            <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="ShowSyncConfigModal" style="display: none;">
                <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
                    <h3 class="ui-dialog-title header">
                        <span class="headerText"></span>
                        <asp:HyperLink ToolTip="Close" ID="closeDialogLink3" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose" runat="server">
                            <span class="ui-icon ui-icon-closethick">Close</span>
                        </asp:HyperLink>
                    </h3>
                </div>
                <div class="ektronModalBody">
                    <div class="ui-dialog-content ui-widget-content">
                        <p class="messages"></p>
                        <ul class="server" id="configurations"></ul>
                        <select id="selectConfigs" size="7" ></select>
                    </div>
                    <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                        <li><asp:HyperLink ToolTip="Close" ID="btnCloseConfigDialog" runat="server" CssClass="button buttonNoIcon buttonRight redHover" /></li>
                        <li><asp:HyperLink ToolTip="Start Sync" ID="btnStartSync" runat="server" CssClass="greenHover button performSyncButton buttonRight" onclick="Ektron.Workarea.Sync.Relationships.StartContentFolderSync(); return false;" /></li>
                    </ul>
                </div>
            </div>
            
            <ektron:SyncDialogs ID="syncDialogs" runat="server" />
        </asp:Panel>
        
        <asp:HiddenField ID="hdnContentLanguage" runat="server" />
        <asp:HiddenField ID="hdnContentId" runat="server" />
        <asp:HiddenField ID="hdnContentAssetId" runat="server" />
        <asp:HiddenField ID="hdnContentAssetVersion" runat="server" />
        <asp:HiddenField ID="hdnFolderId" runat="server" />
        <asp:HiddenField ID="hdnIsMultisite" runat="server" />
        <asp:HiddenField ID="hdnShowDialog" runat="server" />
        
    </form>
</body>
</html>
