<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncCustomConfig.aspx.cs"
    Inherits="SyncCustomConfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
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
    <div class="ektronPageContainer" id="divSyncCustomConfig" runat="server">
        <div class="errorMessage" id="divErrorMessage" runat="server">
        </div>
        <div class="selectContainer">
            <asp:Label ID="lblavailable" runat="server" CssClass="label"></asp:Label>
            <asp:ListBox ID="lbAvaillableEntities" runat="server" CssClass="availlableentitieslist">
            </asp:ListBox>
        </div>
        <span class="scopeActionButtons" id="actionButtons" runat="server"><a class="button buttonAdd"
            href="#" title="Add" onclick="Ektron.Workarea.Sync.CustomConfig.IncludeItem();">
        </a><a class="button buttonAddAll" href="#" title="Add All" onclick="Ektron.Workarea.Sync.CustomConfig.IncludeAllItems();">
        </a><a class="button buttonRemove" href="#" title="Remove" onclick="Ektron.Workarea.Sync.CustomConfig.ExcludeItem();">
        </a><a class="button buttonRemoveAll" href="#" title="Remove All" onclick="Ektron.Workarea.Sync.CustomConfig.ExcludeAllItems();">
        </a></span>
        <div class="selectContainer">
            <asp:Label ID="lblselected" runat="server" CssClass="label"></asp:Label>
            <asp:ListBox ID="lbSelectedEntities" runat="server" CssClass="selectedentitieslist">
            </asp:ListBox>
        </div>
        <span class="scopeActionButtons" id="actionButtons2" runat="server"><a class="button buttonUp"
            href="#" title="Move Up" onclick="Ektron.Workarea.Sync.CustomConfig.MoveUp();"></a>
            <a class="button buttonDown" href="#" title="Move Down" onclick="Ektron.Workarea.Sync.CustomConfig.MoveDown();">
            </a></span>
        <input id="hdnSelectedEntities" runat="server" type="hidden" />
    </div>
    <ektronUI:Css ID="ektronworkareacss1" runat="server" Path="{WorkareaPath}/csslib/ektron.workarea.css" />
    <ektronUI:JavaScript ID="ektronworkareajs1" runat="server"
        Path="{WorkareaPath}/java/ektron.workarea.js" />
    <ektronUI:Css ID="csscustomconfig" runat="server" Path="{WorkareaPath}/sync/css/Ektron.Workarea.Sync.CustomConfig.css" />
    <ektronUI:JavaScript ID="jscustomconfig" runat="server" Path="{WorkareaPath}/sync/js/Ektron.Workarea.Sync.CustomConfig.js" />
    <ektronUI:JavaScriptBlock ID="jscustomconfiginit" runat="server" ExecutionMode="OnEktronReady">
        <ScriptTemplate>
            Ektron.Workarea.Sync.CustomConfig.Init();
        </ScriptTemplate>
    </ektronUI:JavaScriptBlock>
    </form>
</body>
</html>
