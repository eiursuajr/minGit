<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Restore.aspx.cs" Inherits="Workarea_sync_Restore" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Restore Synchronized Files</title>   
</head>
<body>
        <!-- Ektron Client Script -->
    <asp:Literal id="ektronClientScript" runat="server"></asp:Literal>

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
            <div class="pageWrapper">
                <div id="divStatusMessage" runat="server"></div>
                <strong><asp:Label ID="lblRestoreFiles" runat="server"></asp:Label></strong>
                <asp:TreeView ID="tvFileHierarchy" runat="server">
                    <DataBindings>
                        <asp:TreeNodeBinding DataMember="Nodes"  />
                    </DataBindings>
                </asp:TreeView>
            </div>
        </div>
    </form>
</body>
</html>
