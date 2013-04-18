<%@ Page Language="C#" AutoEventWireup="true" Inherits="editarea" CodeFile="editarea.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<asp:literal id="StyleSheetJS" runat="server" />

        <script type="text/javascript">
            var PerReadOnlyLib = false;  //<asp:Literal ID="litPerReadOnlyLib" runat="server"/>;
            var PerContentTreeLang = '<asp:Literal ID="litLanguageId1" runat="server"/>';
            var PerLibraryTreeLang = '<asp:Literal ID="litLanguageId2" runat="server"/>';
            var PerMainPage = '<asp:Literal ID="litMainPage" runat="server"/>';
            $ektron("#ek_nav_bottom").resize(function(){
                NavTreeResizing();
            });
        </script>
        <script type="text/javascript" src="java/workareawindow.js">
</script>
        <script type="text/javascript">
            if (document.layers) {
                onresize = function reDo() {top.ek_nav_bottom.document.location.href="reloadworkareatree.htm";}
            }
        </script>
	</head>
	
    <frameset rows="59,*" cols="100%" border="0">
		<frame id="workareatop" name="workareatop" src="workareatop.aspx?title=workarea_add_top.gif&tab=content" scrolling="no" noresize="noresize" frameborder="0" runat="server" />
		<frameset id="BottomFrameSet" cols="0,*" rows="100%" border="0">
			<frame name="ek_nav_bottom" id="ek_nav_bottom" src="workareanavigationtree.aspx" scrolling="no" frameborder="0" runat="server" />
			<frameset id="BottomRightFrame" rows="*,1" cols="100%" border="0">
				<frame id="ek_main" name="ek_main" src="edit.aspx" scrolling="no" frameborder="0" runat="server" />
			</frameset>
		</frameset>
	</frameset>

</html>

