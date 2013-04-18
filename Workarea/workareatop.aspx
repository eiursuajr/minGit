<%@ Page Language="C#" AutoEventWireup="true" Inherits="workareatop" CodeFile="workareatop.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">
        var selectSettingsTab = <asp:Literal id="SelectSettingsTab" runat="server" text="false"/>;
        
        function Initialize()
        {
            self.focus();
            if (selectSettingsTab) {
                top.switchSettingsTab();
            }
        }

		function ChangePage(tab, pageName)
		{
			if (typeof(top.CanNavigate) == "function" && !top.CanNavigate())
			{
				return;
			}

			//ensure ie9 resizes the accordion properly
			if ("undefined" !== typeof (top.Ektron.Workarea.Height)) {
                		top.Ektron.Workarea.Height.execute();
            		}

			// Load the appropriate page on the right side
		    top.SelectMainWindow(pageName);

            // Update the tree on the left side
            top.MakeNavTreeVisible(pageName);
            
            //highlight root folder.
            try{
                if ("undefined" != typeof(top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.g_selectedFolderList)) {
                    top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.g_selectedFolderList = '0';
                    top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.showSelectedFolderTree();
                }
            }catch(e){}

			$ektron("#tabs .selected").removeClass("selected");
			$ektron(tab).addClass("selected");
		}
    </script>

</head>
<body onload="Initialize();">
    <div class="ektronWorkAreaHeader">
        <h1 class="logo" title="Ektron CMS400.NET">
            Ektron CMS400.NET</h1>
        <div class="dvVersion">
            <label class="version">
                <asp:Literal runat="Server" ID="ltrVersion" />
            </label>
         </div>
         <div class="licInfo">
             <asp:Literal runat="Server" ID="ltrLic" />
         </div>
        <div class="userInfo">
            <asp:Label ToolTip="User Name" ID="lblUser" CssClass="userName" runat="server" />
            |
            <asp:Label ToolTip="New Messages" ID="userUnreadMessages" CssClass="userMessages"
                runat="server" />
        </div>
        <div class="tabs">
            <div id="tabs" class="ui-tabs">
                <ul class="ui-tabs-nav ui-helper-reset">
                    <li class="left">&nbsp;</li>
                    <li id="Desktop"><a title="Desktop" href="#" id="DesktopLink" onclick="ChangePage(this, 'SmartDesktopTree')"
                        runat="server">Desktop</a> </li>
                    <li class="divider"></li>
                    <li id="Content"><a title="Content" href="#" id="ContentLink" onclick="ChangePage(this, 'ContentTree')"
                        runat="server">Content</a> </li>
                    <li class="divider"></li>
                    <li id="Library"><a title="Library" href="#" id="LibraryLink" onclick="ChangePage(this, 'LibraryTree')"
                        runat="server">Library</a> </li>
                    <li class="divider"></li>
                    <li id="Settings"><a title="Settings" href="#" id="SettingsLink" onclick="ChangePage(this, 'AdminTree')"
                        runat="server">Settings</a> </li>
                    <li class="divider"></li>
                    <li id="Reports"><a title="Reports" href="#" id="ReportsLink" onclick="ChangePage(this, 'ReportTree')"
                        runat="server">Reports</a> </li>
                    <li class="divider"></li>
                    <li id="Help"><a title="Help" href="#" id="HelpLink" onclick="ChangePage(this, 'Help')"
                        runat="server">Help</a> </li>
                </ul>
            </div>
        </div>
    </div>
</body>
</html>
