<%@ Page Language="C#" AutoEventWireup="true" CodeFile="loadbalancing.aspx.cs" Inherits="Workarea_loadbalancing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <asp:Literal ID="ltrStyleSheetJS" runat="server"></asp:Literal>
    <link type="text/css" rel="Stylesheet" href="sync/sync.css" />    
    
    <asp:Literal ID="StringResourcesJs" runat="server"></asp:Literal>
    
    <script type="text/javascript">
        Ektron.ready(function() {
            Ektron.Workarea.LoadBalancing.initializeUI();
        });
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">Load Balancing</div>
            <div class="ektronToolbar">
                <table>
                    <tr id="ektronToolbarRow" runat="server"></tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid">
                <tr class="stripe">
                    <td class="left" style="width: 70%;">
                        <div class="label">
                            <asp:Label ToolTip="Force load balance" ID="lblForceLBSyncHeader" runat="server"></asp:Label>
                        </div>
                        <div class="description">
                            <span id="spanForceLoadBalancedSyncDesc" runat="server"></span>
                        </div>
                    </td>
                    <td class="action">
                        <div style="margin-left: auto; margin-right: auto; width:100px;">
                            <a class="button buttonNoIcon greenHover forceLoadBalancedSyncButton" style="font-family: Trebuchet MS,Tahoma,Verdana,Arial,sans-serif; font-size:13.2px;" href="#" id="linkStart" runat="server" title="Start">Start</a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        
        <!-- Modal Dialog: Load Balancing Status -->
        <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="loadBalanceStatusWindow" >
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
                <h3 class="ui-dialog-title header">
                    <span class="headerText" id="spanStatusDialogHeader" runat="server"></span>
                    <asp:HyperLink ID="closeDialogLink3" NavigateUrl="#Close" ToolTip="Close this dialog" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose" runat="server"></asp:HyperLink>
                </h3>
            </div>
            <div class="ektronModalBody">
                <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                    <p class="messages"></p>
                    <div class="syncStatusMessages"></div>
                </div>
                <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                    <li>
                        <asp:HyperLink ToolTip="Close" ID="btnCloseLoadBalanceStatus" runat="server" Text="Close" CssClass="button buttonNoIcon buttonRight redHover" />
                    </li>
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
