<%@ Page Language="C#" AutoEventWireup="true" Inherits="naviconbar" CodeFile="naviconbar.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>naviconbar</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <%=StyleSheetJS%>

    <script type="text/javascript" src="java/stylehelper.js">
</script>

    <script type="text/javascript">
    
        <!--//--><![CDATA[//><!--
		var m_loaded = false;
		var m_DelayedActionTimer = 0;
		var m_MouseOutDetected = false;
		var jsAppImgPath="<asp:literal id=jsAppImgPath runat=Server/>";

		function MakeNavTreeVisible(treeName) {
			top.MakeNavTreeVisible(treeName);
		}

		function StartEnlarge(e) {
			top.StartEnlarge(e);
		}

		function StartShrink(e) {
			top.StartShrink(e);
		}

		function SetClassPaths() {
			// update all the stylesheet class paths
			MakeClassPathRelative("body", "FolderIconBar", "backgroundImage", jsAppImgPath, GetRelativeClassPath());
			MakeClassPathRelative("td", "NavIconBarNm", "backgroundImage", jsAppImgPath, GetRelativeClassPath());
			MakeClassPathRelative("td", "NavIconBarOver", "backgroundImage", jsAppImgPath, GetRelativeClassPath());
			MakeClassPathRelative("td", "NavIconBarSelectedOver", "backgroundImage", jsAppImgPath, GetRelativeClassPath());
			MakeClassPathRelative("td", "NavIconBarSelected", "backgroundImage", jsAppImgPath, GetRelativeClassPath());
		}

		function Startup() {
			if (IsBrowserSafari() && (!CanAccessStyleSheets())){
				setTimeout('Startup()', 1500);
			} else {
				StartupContinued();
			}
		}
		function StartupContinued() {
			SetClassPaths();

			m_loaded = true;
		}
		
		function IsLoaded() {
			return m_loaded;
		}
		
		function IconClicked (IconObj, TreeName) {
			// Need to know if "Selected" or "SelectedOver" should 
			// be applied when master calls back:
			m_MouseOutDetected = false;
			SelectButton(GetIconNumber(IconObj), TreeName);
		}

		function SelectButton(IconNumber, TreeName) {
			// Cancel any pending call-retries:
			ClearDelayedAction();
			// (Call the NavButtons.aspx to do update; it is master, 
			// we are slave. NavButtons.aspx will call SelectIcon()
			// when ready, to select and deselect icons)
			// 
			// Try updating buttons until success:
			if (!top.SelectButton(IconNumber, TreeName)) {
				PostDelayedAction("SelectButton(" + IconNumber + ",'" + TreeName + "');");
			}
		}
		
		function SelectIcon(iconNumber, selectFlag) {
			// Master calling, clear any pending calls to it:
			ClearDelayedAction();
			
			if (selectFlag) {
				// If mouse moved out or icon is hidden, then apply selected
				// class appearance, otherwise use selected & mouse-over class:
				if (m_MouseOutDetected || (document.getElementById(BuildIconName(iconNumber)).style.display == "none")) {
					document.getElementById(BuildIconName(iconNumber)).className = "NavIconBarSelected";
				}
				else {
					document.getElementById(BuildIconName(iconNumber)).className = "NavIconBarSelectedOver";
				}
			}
			else {
				// Un-Select the Icon:
				document.getElementById(BuildIconName(iconNumber)).className = "NavIconBarNm";
			}
		}
		
		function EnableIcon(iconNumber, enableFlag) {
			if (enableFlag) {
				document.getElementById(BuildIconName(iconNumber)).style.display = "";
			}
			else {
				document.getElementById(BuildIconName(iconNumber)).style.display = "none";
			}
		}

		function BuildIconName(iconNumber) {
			var iconName = "icon" + iconNumber;
			return (iconName);
		}
		
		function GetIconNumber(IconObj) {
			var iconNumber = parseInt(((IconObj.id).toLowerCase()).replace("icon", ""));
			return (iconNumber);
		}		

		function RollOverIcon(IconObj) {
			if (IconObj.className == "NavIconBarNm") {
				IconObj.className = "NavIconBarOver";
			}
			else {
				IconObj.className = "NavIconBarSelectedOver";
			}
		}
		
		function RollOutIcon(IconObj) {
			m_MouseOutDetected = true;
			if (IconObj.className == "NavIconBarOver") {
				IconObj.className = "NavIconBarNm";
			}
			else {
				IconObj.className = "NavIconBarSelected";
			}
		}
		
		function PostDelayedAction(functionName) {
			if (m_DelayedActionTimer)
				return; // already set.
			m_DelayedActionTimer = setTimeout(functionName, 100); 
		}
		
		function ClearDelayedAction() {
			if (m_DelayedActionTimer) {
				clearTimeout(m_DelayedActionTimer);
				m_DelayedActionTimer = 0;
			}
		}

		function UpClicked() {
			var NavObj = top.GetNavButtonObject();
			if (NavObj) {
				NavObj.UpClicked();
			}
		}
		
		function UpDoubleClicked() {
			var NavObj = top.GetNavButtonObject();
			if (NavObj) {
				NavObj.UpDoubleClicked();
			}
		}

		function DownClicked() {
			var NavObj = top.GetNavButtonObject();
			if (NavObj) {
				NavObj.DownClicked();
			}
		}
		
		function DownDoubleClicked() {
			var NavObj = top.GetNavButtonObject();
			if (NavObj) {
				NavObj.DownDoubleClicked();
			}
		}
        //--><!]]>
    </script>

    <script src="java/toolbar_roll.js">
</script>

</head>
<body class="FolderIconBar" onmouseover="StartEnlarge(event);" onmouseout="StartShrink(event);"
    onload="Startup();" ondragstart="return false;" topmargin="0" bottommargin="0">
    <table cellpadding="0" cellspacing="0" id="Table1" width="100%" height="100%">
        <tr>
            <td align="left">
                <table cellpadding="0" cellspacing="0" id="TableCtrl" height="100%">
                    <tr>
                        <td id="UpContainer" onclick="UpClicked();" ondblclick="UpDoubleClicked();" height="10">
                            <a href="#">
                                <img id="UpBtn" src="images/application/btn_15x9_up-nm.gif" title="<%=m_MsgHelper.GetMessage("alt show more buttons button text")%>"
                                    ondragstart="return false;" border="0" align="left" onmouseup="SwapImageUp('UpBtn');"
                                    onmousedown="SwapImageDown('UpBtn');" onmouseout="SwapImageOut('UpBtn');" onmouseover="SwapImageOver('UpBtn');">
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td id="DownContainer" onclick="DownClicked();" ondblclick="DownDoubleClicked();"
                            height="10">
                            <a href="#">
                                <img id="DownBtn" src="images/application/btn_15x9_down-nm.gif" title="<%=m_MsgHelper.GetMessage("alt show fewer buttons button text")%>"
                                    ondragstart="return false;" border="0" align="left" onmouseup="SwapImageUp('DownBtn');"
                                    onmousedown="SwapImageDown('DownBtn');" onmouseout="SwapImageOut('DownBtn');"
                                    onmouseover="SwapImageOver('DownBtn');">
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="right">
                <table cellpadding="0" cellspacing="0" id="Table2"  height="100%">
                    <tr>
                        <td id="icon0" class="NavIconBarNm" title="<%=m_MsgHelper.GetMessage("alt select contenttree button text")%>"
                            onmouseover="RollOverIcon(this)" onclick="IconClicked(this, 'ContentTree');"
                            onmouseout="RollOutIcon(this)">
                            <img align="middle" id="iconImage0" src="images/application/navbar/icon_content.gif" title="Content Tree" alt="Content Tree"/>
                        </td>
                        <td id="icon1" class="NavIconBarNm" title="<%=m_MsgHelper.GetMessage("alt select librarytree button text")%>"
                            onmouseover="RollOverIcon(this)" onclick="IconClicked(this, 'LibraryTree');"
                            onmouseout="RollOutIcon(this)">
                            <img align="middle" id="iconImage1" src="images/application/navbar/icon_library.gif" title="Library Tree" alt="Library Tree"
                            onmouseover="RollOverIcon(this)" onmouseout="RollOutIcon(this)"/>
                        </td>
                        <td id="icon2" class="NavIconBarNm" title="<%=m_MsgHelper.GetMessage("alt select moduletree button text")%>"
                            onmouseover="RollOverIcon(this)" onclick="IconClicked(this, 'ModuleTree');" onmouseout="RollOutIcon(this)">
                            <img align="middle" id="iconImage2" src="images/application/navbar/icon_modules.gif" title="Module Tree" alt="Module Tree"
                            onmouseover="RollOverIcon(this)" onmouseout="RollOutIcon(this)"/>
                        </td>
                        <td id="icon3" class="NavIconBarNm" title="<%=m_MsgHelper.GetMessage("alt select admintree button text")%>"
                            onmouseover="RollOverIcon(this)" onclick="IconClicked(this, 'AdminTree');" onmouseout="RollOutIcon(this)">
                            <img align="middle" id="iconImage3" src="images/application/navbar/icon_admin.gif" title="Settings Tree" alt="Settings Tree"
                            onmouseover="RollOverIcon(this)" onmouseout="RollOutIcon(this)"/>
                        </td>
        <% if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking)) { %>
          
                        <td id="icon4" class="NavIconBarNm" title="<%=m_MsgHelper.GetMessage("alt select workspacetree button text")%>"
                            onmouseover="RollOverIcon(this)" onclick="IconClicked(this, 'WorkSpaceTree');" onmouseout="RollOutIcon(this)">
                            <img align="middle" id="Img1" src="images/application/user.gif" title="My Workspace Tree" alt="My Workspace Tree"
                            onmouseover="RollOverIcon(this)" onmouseout="RollOutIcon(this)" width="16" height="16"/>
                        </td>
        <% }%>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
