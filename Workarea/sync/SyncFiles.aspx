<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncFiles.aspx.cs" Inherits="SyncFiles" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Synchronize Files</title>
        
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
            <asp:ScriptManager ID="scriptMananger" runat="server"></asp:ScriptManager>
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
                    <strong><asp:Label ID="lblSyncFilesHeader" runat="server"></asp:Label></strong>
                    <%--<asp:TreeView ID="tvFiles" runat="server"></asp:TreeView>--%>
                    <telerik:RadTreeView ID="rtvFiles" runat="server" 
                        onnodeexpand="rtvFiles_NodeExpand"></telerik:RadTreeView>
                </div>
            </div>

            <ektron:SyncDialogs ID="syncDialogs" runat="server" />
            
        </form>
    </body>
</html>
