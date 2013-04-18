<%@ Page Language="C#" AutoEventWireup="true" Inherits="navbuttons" CodeFile="navbuttons.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>navbuttons</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <%=m_strStyleSheetJS%>

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
		var m_DragbarSize = 14;//5;
		var m_BodySize = parent.document.getElementById("nav_divider").rows;
		var m_FrameSizes = m_BodySize.split(",");
		var m_Dragging = false;
		var m_Offset = 0;
		var m_buttonSize = new Array;
		var m_DelayedActionTimer=0;
		var m_OrigResizeBarClass = "";
		var m_loaded = false;
		var jsAppImgPath="<asp:literal id=jsAppImgPath runat=Server/>";

		function IsBrowserSafari() {
			var posn;
			posn = parseInt(navigator.appVersion.indexOf('Safari'));
			return (0 <= posn);
		}

		function SetClassPaths() {
			// Update all the stylesheet classes now (must delay for Safari):
			MakeClassPathRelative("td", "FolderButtonBarNm", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
			MakeClassPathRelative("td", "FolderButtonBarOver", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
			MakeClassPathRelative("td", "FolderButtonBarSelected", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
			MakeClassPathRelative("td", "FolderButtonBarSelectedOver", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
			MakeClassPathRelative("td", "FolderResizeBar", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
		}

		function Startup() {
			if (IsBrowserSafari() && (!CanAccessStyleSheets())){
				setTimeout('Startup()', 500);
			} else {
				StartupContinued();
			}
		}

		function StartupContinued() {
			SetClassPaths();

			// This function sets up all the page level variables so everything is initailized
			var tmpButtonDisplay = new Array;

			var barHeight;
			var barPadding, btnPadding;
			
			if (IsBrowserSafari()) {
				barPadding = 11;
				btnPadding = 7;
			} else {
				barPadding = 0;
				btnPadding = 0;
			}
			// First get the present size of the dragbar
			// If this is netscape6-7 than increase the ResizeBar a few pixels to help with
			// the lag in the dragging speed
			if (!IsBrowserIE()) {
				var tmpHeight = parseInt(getStyleByClass(document.getElementById("ResizeBar").tagName, "FolderResizeBar", "height"));
				setStyleByCssClass(document.getElementById("ResizeBar").tagName, "FolderResizeBar", "height", tmpHeight + 4);
			}
			// Save the rendered height of the Resizebar for later frame size calculations
			barHeight = parseInt(document.getElementById("ResizeBar").offsetHeight);
			m_DragbarSize = barHeight + barPadding;
			if (IsBrowserSafari() & m_DragbarSize < 16) {
				m_DragbarSize = 16;
			}
			// Save the number of buttons created for this list
			var NumOfButtons = GetNumberofButtons();
			for (var iLoop = 0; iLoop < NumOfButtons; iLoop++) {
				// Save the current state of each button so it can be restored later
				tmpButtonDisplay[iLoop] = document.getElementById("Button" + iLoop).style.display;
				// Now make the button visible, so we can get its render size below
				document.getElementById("Button" + iLoop).style.display = "";
			}
			for (var iLoop = 0; iLoop < NumOfButtons; iLoop++) {
				m_buttonSize[iLoop] = btnPadding + parseInt(document.getElementById("Button" + iLoop.toString()).offsetHeight);
			if (IsBrowserSafari()) {
				m_buttonSize[iLoop] = 25;
			}

			}
			for (var iLoop = 0; iLoop < NumOfButtons; iLoop++) {
				// Now restore the buttons original display setting
				document.getElementById("Button" + iLoop).style.display = tmpButtonDisplay[iLoop];
			}
			
			UpdateThisFrameSize(GetTotalActiveSize() + (m_DragbarSize * 1));
			m_OrigResizeBarClass = document.getElementById("ResizeBar").bgColor;
			m_loaded = true;
			UpdateIconbar();
		}

		function IsLoaded() {
			return m_loaded;
		}
		
		function IsBrowserIE() {
			return (top.IsBrowserIE());
		}
		
		function StartEnlarge(e) {
			top.StartEnlarge(e);
		}

		function StartShrink(e) {
			if (IsBrowserIE()) {
				var myElement = e.toElement;
			}
			else {
				var myElement = e.relatedTarget;
			}
			if (myElement == null) {
				if ((m_Dragging) && (!IsBrowserIE())) {
					StopDrag();
				}
				else {
					top.StartShrink(e);
				}
			}
		}

		function AllowDrag(e) {
			if (!IsBrowserSafari()) {
				m_Offset = e.clientY;
				document.getElementById("ResizeBar").bgColor = "yellow";
				m_Dragging = true;
			}
		}

		function StopDrag() {
			if (m_Dragging) {
				m_Dragging = false;
				if (parseInt((GetThisFrameSize() - m_DragbarSize)) < m_DragbarSize) {
					UpdateThisFrameSize(m_DragbarSize);
				}
				else if (GetThisFrameSize() > parseInt((m_DragbarSize * 1) + (GetTotalButtonSize() * 1))) {
					UpdateThisFrameSize(parseInt((m_DragbarSize * 1) + (GetTotalButtonSize() * 1)));
				}
				else {
					UpdateThisFrameSize(parseInt((m_DragbarSize * 1) + (GetTotalActiveSize() * 1)));
				}
			}
		}
		
		function UpdateThisFrameSize(SizeChange) {
			var bodySize = parent.document.getElementById("nav_divider").rows;
			var frameSizes = bodySize.split(",");
			
			m_FrameSizes[2] = parseInt((SizeChange) * 1);
			parent.document.getElementById("nav_divider").rows = m_FrameSizes[0] + "," + m_FrameSizes[1] + "," + m_FrameSizes[2] + "," + m_FrameSizes[3];
			return (m_FrameSizes[2]);
		}
		
		function GetThisFrameSize() {
			var bodySize = parent.document.getElementById("nav_divider").rows;
			var frameSizes = bodySize.split(",");
			m_FrameSizes[2] = parseInt((m_FrameSizes[2]) * 1);
			return (m_FrameSizes[2]);
		}
		
		function GetNumberofButtons() {
			var buttonNumber = 0;
		
			while (document.getElementById("Button" + buttonNumber) != null) {
				buttonNumber++;
			}
			return (buttonNumber);
		}
		
		function Drag(e) {
			var activeSize = GetTotalActiveSize();
			var activeButtons = GetNumOfActiveButtons();
			var btnQty = GetNumberofButtons();
			var tmpFrameSize = 0;

			if (m_Dragging) {
				if (!e) {
					var e = document.event;
				}
				tmpFrameSize = GetThisFrameSize() + (m_Offset - e.clientY);
				
				if (tmpFrameSize >= (GetTotalButtonSize() + (m_DragbarSize * 1))) {
					tmpFrameSize = (GetTotalButtonSize() + (m_DragbarSize * 1));
				}
				else if (tmpFrameSize <= (m_DragbarSize * 1)) {
					tmpFrameSize = (m_DragbarSize * 1);
				}
				if (parseInt((tmpFrameSize - m_DragbarSize)) < m_DragbarSize) {
					UpdateThisFrameSize(m_DragbarSize);
				}
				if ((tmpFrameSize - m_DragbarSize) < activeSize) {
					if (Math.floor(parseInt(activeSize - (tmpFrameSize - m_DragbarSize))) > Math.floor(m_buttonSize[(activeButtons - 1)] / 2)) {
						document.getElementById("Button" + (activeButtons - 1).toString()).style.display = "none";
						UpdateIconbar();
					}
				}
				else {
					if ((activeButtons < btnQty) && (Math.floor((parseInt((tmpFrameSize - m_DragbarSize)) - activeSize)) > Math.floor(m_buttonSize[(activeButtons)] / 2))) {
						document.getElementById("Button" + activeButtons).style.display = "";
						UpdateIconbar();
					}
				}
				UpdateThisFrameSize(tmpFrameSize);
			}
		}
		
		function GetNumOfActiveButtons() {
			var iLoop = 0;
			var btnQty = GetNumberofButtons();
		
			for (iLoop = 0; iLoop < btnQty; iLoop++) {
				if (document.getElementById("Button" + iLoop).style.display != "") {
					break;
				}
			}
			return (iLoop);
		}
		
		function GetTotalButtonSize() {
			var retValue = 0;
			
			for (var iLoop = 0; iLoop < m_buttonSize.length; iLoop++) {
				retValue += (m_buttonSize[iLoop] * 1);
			}
			return (retValue);
		}
		
		function GetTotalActiveSize() {
			var ActiveButtons = GetNumOfActiveButtons();
			var iLoop = 0;
			var retValue = 0;
		
			for (iLoop = 0; iLoop < ActiveButtons; iLoop++) {
				retValue += (m_buttonSize[iLoop] * 1);
			}
			return (retValue);
		}
		
		function MakeNavTreeVisible(treeName) {
			top.MakeNavTreeVisible(treeName);
		}

		function UpdateIconbar() {
			ClearDelayedAction();
			for (var iLoop = 0; iLoop < GetNumberofButtons(); iLoop++) {
				if (!top.EnableIcon(iLoop, (document.getElementById("Button" + iLoop).style.display == "none"))) {
					PostDelayedAction('UpdateIconbar()');
					return;
				}
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
		
		function RollOverButton(ButtonObj) {
			if (ButtonObj.className == "FolderButtonBarNm") {
				ButtonObj.className = "FolderButtonBarOver";
			}
			else {
				ButtonObj.className = "FolderButtonBarSelectedOver";
			}
		}
		
		function RollOutButton(ButtonObj) {
			if (ButtonObj.className == "FolderButtonBarOver") {
				ButtonObj.className = "FolderButtonBarNm";
			}
			else {
				ButtonObj.className = "FolderButtonBarSelected";
			}
		}
		
		function UnSelectButtons() {
			var NumOfButtons = GetNumberofButtons();
			
			for (var iLoop = 0; iLoop < NumOfButtons; iLoop++) {
				document.getElementById("Button" + iLoop.toString()).className = "FolderButtonBarNm";
				top.SelectIcon(iLoop, false);
			}
		}
		
		function BuildButtonName(ButtonNumber) {
			var ButtonName = "Button" + ButtonNumber;
			return (ButtonName);
		}
		
		function GetButtonNumber(ButtonObj) {
			var ButtonNumber = parseInt((ButtonObj.id).replace("Button", ""));
			return (ButtonNumber);
		}		

		function ButtonClicked(ButtonObj, TreeName) {
			SelectButton(GetButtonNumber(ButtonObj), TreeName);
		}
		
		function SelectButton(ButtonNumber, TreeName) {
			var ButtonName = BuildButtonName(ButtonNumber);
			if (typeof(top.CanNavigate) == "function") {
				if (!top.CanNavigate()) {
					return;
				}
			}

			MakeNavTreeVisible(TreeName);
			UnSelectButtons();
			top.SelectIcon(ButtonNumber, true);
			if (document.getElementById(ButtonName).style.display == "none") {
				document.getElementById(ButtonName).className = "FolderButtonBarSelected";
			}
			else {
				document.getElementById(ButtonName).className = "FolderButtonBarSelectedOver";
			}
		
			// update main-workarea window:
		    top.SelectMainWindow(TreeName);
		}

		// Create a JavaScript Structure, to pass to Drag():
		function MyPsuedoMouseEventStruct() {
		}
		MyPsuedoMouseEventStruct.clientY = 0;

		function DownClicked() {
			var NumActive;
			// abort if currently dragging:
			if (m_Dragging) {
				return;
			}
			NumActive = GetNumOfActiveButtons();
			if (NumActive == 0) {
				return; // all buttons already hidden.
			}
			m_Dragging = true; // (pseudo dragging)
			// Determine the height of the bottom-most displayed 
			// button, and "drag" the bar down by that ammount:
			MyPsuedoMouseEventStruct.clientY = (m_Offset + (m_buttonSize[NumActive - 1]) * 1);
			Drag(MyPsuedoMouseEventStruct);
			m_Dragging = false;
		}

		function DownDoubleClicked() {
			while (GetNumOfActiveButtons()) {
				if (m_Dragging) {
					break;
				}
				DownClicked();
			}
		}

		function UpClicked() {
			var NumActive;
			// abort if currently dragging:
			if (m_Dragging) {
				return;
			}
			NumActive = GetNumOfActiveButtons();
			if (NumActive == GetNumberofButtons()) {
				return; // all buttons already showing.
			}
			m_Dragging = true; // (pseudo dragging)
			// Determine the height of the first hidden
			// button, and "drag" the bar up by that ammount:
			MyPsuedoMouseEventStruct.clientY = (m_Offset - (m_buttonSize[NumActive] * 1));
			Drag(MyPsuedoMouseEventStruct);
			m_Dragging = false;
		}

		function UpDoubleClicked() {
			while (GetNumOfActiveButtons() < GetNumberofButtons()) {
				if (m_Dragging) {
					break;
				}
				UpClicked();
			}
		}
		
		//--><!]]>
    </script>

</head>
<body class="workarea" onload="Startup();" onmousemove="Drag(event);" onmouseout="StartShrink(event);"
    onmouseover="StartEnlarge(event);">
    <table width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td class="FolderResizeBar" ondragstart="return false;" id="ResizeBar" height="10px">
                <img style="cursor: n-resize;" align="bottom" onmousedown="AllowDrag(event);return false;"
                    onmouseup="StopDrag(event);" border="0" src="images/application/resize_bar.gif"
                    ondragstart="return false;"/>
            </td>
        </tr>

        <tr>
            <td title="Content" class="FolderButtonBarNm" onclick="ButtonClicked(this, 'ContentTree');" id="Button0"
                valign="middle" onmouseover="RollOverButton(this)" onmouseout="RollOutButton(this)"
                runat="server">
                <!--<img style="MARGIN-LEFT: 3px;" id="iconImage0" title="alt select contenttree button text" src="images/application/icon_content.gif" align="absmiddle" > content button text-->
            </td>
        </tr>
        <% if (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))  {  %>
       

        <tr>
            <td title="My Worksapace" class="FolderButtonBarNm" onclick="ButtonClicked(this, 'WorkSpaceTree');" id="Button4"
                valign="middle" onmouseover="RollOverButton(this)" onmouseout="RollOutButton(this)"
                runat="server">
                <!--<img style="MARGIN-LEFT: 3px;" id="iconImage0" title="alt select contenttree button text" src="images/application/icon_content.gif" align="absmiddle" > content button text-->
            </td>
        </tr>
        <% } %>
        <tr>
            <td title="Library" class="FolderButtonBarNm" onclick="ButtonClicked(this, 'LibraryTree');" id="Button1"
                valign="middle" onmouseover="RollOverButton(this)" onmouseout="RollOutButton(this)"
                runat="server">
                <!--<img style="MARGIN-LEFT: 3px;" id="Img2" title="alt select librarytree button text" src="images/application/icon_library.gif" align="absmiddle"> library button text-->
            </td>
        </tr>
        <tr>
            <td title="Modules" class="FolderButtonBarNm" onclick="ButtonClicked(this, 'ModuleTree');" id="Button2"
                valign="middle" onmouseover="RollOverButton(this)" onmouseout="RollOutButton(this)"
                runat="server">
                <!--<img style="MARGIN-LEFT: 3px;" id="Img3" title="alt select moduletree button text" src="images/application/icon_modules.gif" align="absmiddle"> modules button text-->
            </td>
        </tr>
        <tr>
            <td title="Settings" class="FolderButtonBarNm" onclick="ButtonClicked(this, 'AdminTree');" id="Button3"
                valign="middle" onmouseover="RollOverButton(this)" onmouseout="RollOutButton(this)"
                runat="server">
                <!--<img style="MARGIN-LEFT: 3px;" id="Img4" title="alt select admintree button text"src="images/application/icon_admin.gif" align="absmiddle"> administrate button text-->
            </td>
        </tr>
    </table>
</body>
</html>

