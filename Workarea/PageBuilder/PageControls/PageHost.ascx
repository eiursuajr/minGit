<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PageHost.ascx.cs" Inherits="PageHost" %>
<%@ Register Src="WidgetTray.ascx" TagName="WidgetTray" TagPrefix="UCEktron" %>

<asp:Literal ID="isMasterLayout" Visible="false" runat="server" EnableViewState="false">
    <script type="text/javascript" language="JavaScript">
        Ektron.PBMasterSettings = { 'isMasterLayout': <isMasterLayout>, 'pathToLock': '<pathToLock>', 'hasMasterLayout': <hasMasterLayout> }
    </script>
</asp:Literal>
<asp:Literal ID="dontAutoCloseMenu" Visible="false" runat="server" EnableViewState="false">
    <script type="text/javascript" language="JavaScript">
        Ektron.PBSettings = { 'dontClose': false }
    </script>
</asp:Literal>
<asp:Literal ID="sessionKeepalive" runat="server" Visible="false" EnableViewState="false">
    <script type="text/javascript" language="JavaScript">
        function sessionKeepAlive() {
            var wRequest = new Sys.Net.WebRequest();
            wRequest.set_url("<pagepostback>");
            wRequest.set_httpVerb("POST");
            wRequest.add_completed(sessionKeepAlive_Callback);
            wRequest.set_body("Message=keepalive");
            wRequest.get_headers()["Content-Length"] = 0;
            wRequest.invoke();
        }

        function sessionKeepAlive_Callback(executor, eventArgs){}
        window.setInterval( "sessionKeepAlive();", <millis>);
    </script>
</asp:Literal>

<div id="EktronPersonalizationWrapper" runat="server" class="EktronPersonalizationWrapper">
    <div class="topmenu">
        <div class="topmenuitem"><asp:Literal id="lblFile" runat="server" />
            <ul class="dropdown">
                <li id="PBNew" class="dropdown">
                    <asp:LinkButton ID="lbNew" runat="server" Visible="true" OnClientClick="return false;" />
                </li>
                <li id="PBCopy" class="dropdown">
                    <asp:LinkButton ID="lbCopy" runat="server" Visible="true" OnClientClick="return false;" />
                </li>
                <li id="PBEdit" class="dropdown">
                     <asp:LinkButton CssClass="edit" ID="lbEdit" runat="server" OnClick="lbEdit_Click" Visible="true" />
                </li>
                <li id="PBSave" class="dropdown">
                    <asp:LinkButton ID="lbSave" runat="server" OnClick="lbSave_Click" Visible="true" />
                </li>
                <li id="PBCheckin" class="dropdown2">
                    <asp:LinkButton ID="lbCheckin" OnClick="lbCheckin_Click" runat="server" Visible="true" />
                </li>
                <li id="PBPublish" class="dropdown2">
                    <asp:LinkButton ID="lbPublish" runat="server" OnClick="lbPublish_Click" Visible="true" />
                </li>
                <li id="PBCancel" class="dropdown">
                    <asp:LinkButton ID="lbCancel" runat="server" OnClick="lbCancel_Click" Visible="true" />
                </li>
            </ul>
        </div>

        <div class="topmenuitem">View
            <ul class="dropdown">
                <li id="PBViewPublishedCheckedIn" class="dropdown">
                    <asp:LinkButton ID="lbViewPublishedCheckedIn" runat="server" Visible="true" OnClick="lbViewPublishedCheckedIn_Click" />
                </li>
                <li id="PBProperties" class="dropdown">
                    <asp:HyperLink NavigateUrl="#" ID="lbProperties" runat="server" visible="true" />
                </li>
                <li id="PBPreview" class="dropdown">
                    <asp:LinkButton ID="lbPreview" runat="server" Visible="true" OnClick="lbPreview_Click" />
                </li>
                <li id="PBWorkarea" class="dropdown">
                    <asp:LinkButton ID="lbWorkarea" runat="server" Visible="true" />
                </li>
                <li id="PBAnalytics" class="dropdown">
                    <asp:LinkButton ID="lbAnalytics" runat="server" Visible="true" />
                </li>
            </ul>
        </div>

        <div class="topmenuitem" id="propsmenu" runat="server">Properties
            <ul class="dropdownProps">
                <li id="Li1" class="dropdown">
                    <div>
                        <table id="PBPropsTable">
                            <tr>
                                <td><span id="spnMode" runat="server" class="label">Current Mode</span></td>
                                <td><asp:Label ID="lblMode" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnTitle" runat="server" class="label">Title</span></td>
                                <td><asp:Label ID="lblTitle" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnPageID" runat="server" class="label">Page ID</span></td>
                                <td><asp:Label ID="lblPageid" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnLanguage" runat="server" class="label">Language</span></td>
                                <td><asp:Label ID="lblLanguage" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnLastEditor" runat="server" class="label">Last User to Edit</span></td>
                                <td><asp:Label ID="lblLasteditor" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnLastEditDate" runat="server" class="label">Last Edit date</span></td>
                                <td><asp:Label ID="lblLasteditdate" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnDateCreated" runat="server" class="label">Date Created</span></td>
                                <td><asp:Label ID="lblDatecreated" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnWireframeFile" runat="server" class="label">Wireframe</span></td>
                                <td><asp:Label ID="lblWireframe" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnContentPath" runat="server" class="label">Path</span></td>
                                <td><asp:Label ID="lblPath" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><span id="spnStatus" runat="server" class="label">Status</span></td>
                                <td><asp:Label ID="lblStatus" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trCheckedOut" runat="server" visible="false">
                                <td><span id="spnCurrentEditor" runat="server" class="label">Currently Checked out to</span></td>
                                <td><asp:Label ID="lblUserCheckedOut" runat="server"></asp:Label></td>
                            </tr>
                        </table>
                    </div>
                </li>
            </ul>
        </div>

        <div class="topmenuitem">Help
            <ul class="dropdown">
                <li id="PBPagesHelp" class="dropdown">
                    <asp:LinkButton ID="lbCreatingLayouts" runat="server" Visible="true" />
                </li>
                <li id="PBLayoutHelp" class="dropdown">
                    <asp:LinkButton ID="lbLayoutManagement" runat="server" Visible="true" />
                </li>
                <li id="PBLaunchHelp" class="dropdown">
                    <asp:LinkButton ID="lbLaunchHelp" runat="server" Visible="true" />
                </li>
            </ul>
        </div>

        <asp:Repeater ID="repWidgetMenus" runat="server">
            <ItemTemplate>
                <div class="topmenuitem"><%# (Container.DataItem as Ektron.Cms.PageBuilder.PageBuilderMenu).Title %>
                    <ul class="dropdown">
                        <asp:Repeater ID="repWidgetMenuItems" runat="server">
                            <ItemTemplate>
                                <li class="dropdown">
                                    <asp:LinkButton ID="lbBtn" runat="server" Visible="true" />
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="MenuTack">
            <a href="#" onclick="Ektron.PageBuilder.WidgetTray.menuTack(); return false;"><img title="Thumbtack" alt="Thumbtack" id="MenuTack" class="menuTackImg" runat="server" src="#" /></a>
        </div>
        <div class="MenuToggle">
            <a href="#" onclick="Ektron.PageBuilder.WidgetTray.menuToggle(); return false;"><img title="Open / Close" alt="Open / Close" id="MenuImg" class="menuToggleImg" runat="server" src="#" /></a>
        </div>

        <div class="topmenuitem_right"><asp:LinkButton ID="lbLogout" runat="server" Visible="true">Logout</asp:LinkButton></div>
        <asp:Panel ID="pnlSearchWidget" runat="server">
            <div class="searchbox_alignright">
                <div style="float: left;"><input class="topmenuinputbox" ID="topMenuInput" type="text" runat="server" /></div>
                <div style="float: right;"><input class="searchbutton" type="button" /></div>
            </div>
            <div class="topmenutext_right"><asp:Label id="lblFilterControlList" AssociatedControlID="topMenuInput" runat="server" /></div>
        </asp:Panel>
        <br class="clear" />
    </div>

    <div id="widgetlist" class="controldashboard">
        <UCEktron:WidgetTray ID="tray" runat="server" Visible="false" />
    </div>

    <div class="controldashboardbottom"><!--<img id="imgdashbottom" runat="server" alt="" src="#" style="width:100%; height:5px;" />--></div>
    <div class="pullchain">
        <a href="#" onclick="Ektron.PageBuilder.WidgetTray.toggleTray(); return false;"><img id="imgpullchain" alt="pullchain" runat="server" src="#" /></a>
    </div>
</div>
