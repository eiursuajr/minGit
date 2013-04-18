<%@ Page Language="C#" AutoEventWireup="true" Inherits="navtoolbar" CodeFile="navtoolbar.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>navtoolbar</title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <asp:Literal id="StyleSheetJS" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--		
		var m_Loaded = false;
		var jsAppImgPath="<asp:literal id=jsAppImgPath runat=Server/>";
		
		function ShrinkFrame(e) {
			top.ShrinkFrame(e);
		}
		
		function StartEnlarge(e) {
			top.StartEnlarge(e);
		}
		
		function StartShrink(e) {
			top.StartShrink(e);
		}

		function SetClassPaths() {
			// Update all the stylesheet classes now (must delay for Safari):
			MakeClassPathRelative("table", "FolderToolbar", "backgroundImage", jsAppImgPath, GetRelativeClassPath())
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
			m_Loaded = true;
		}
		
		function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
			var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
			var popupwin = window.open(url, hWind, cToolBar);
			return popupwin;
		}
		
		//--><!]]>
    </script>
    <style type="text/css">
        html,body
        {
            width:100%;
            height:100%;
            margin:0px;
            padding:0px;
        }
    </style>
</head>
<body onmouseout="StartShrink(event);" onmouseover="StartEnlarge(event);" onload="Startup();">
    <div class="ektronNavToolbar">    
        <div class="ektronTitlebar">
            <span class="help">
                <asp:Literal ID="HelpButton" runat="server" />
            </span>
        </div>
    </div>
</body>
</html>

