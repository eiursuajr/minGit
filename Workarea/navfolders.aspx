<%@ Page Language="C#" AutoEventWireup="true" Inherits="navfolders" CodeFile="navfolders.aspx.cs" %>

<!-- do *NOT* change this doctype or it'll break the workarea accordion layout! -->
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>navfolders</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
		var m_loaded = false;

		function Startup() {
			setTimeout("m_loaded = true;", 1);
		}

		function IsLoaded() {
			return (m_loaded);
		}
Ektron.ready(function() {
		    if ("undefined" !== typeof (top.Ektron.Workarea.Height)) {
		        top.Ektron.Workarea.Height.heightFix(function(height) {
		            $ektron("html, body, iframe").css("height", (height - 20) + "px");
		        });
		        top.Ektron.Workarea.Height.execute();
		    }
		});
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
<body class="UiNavigation" onload="Startup();" >
    <iframe id="ContentTree" name="ContentTree" frameborder="0" marginheight="0" marginwidth="0"
        width="100%" height="100%" scrolling="auto" src="blank.htm"></iframe>
    <iframe style="display: none" id="AdminTree" name="AdminTree" src="blank.htm" frameborder="0"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="no"></iframe>
    <iframe style="display: none" id="LibraryTree" name="LibraryTree" src="blank.htm" frameborder="0"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto"></iframe>
    <iframe style="display: none" id="ReportTree" name="ReportTree" src="blank.htm" frameborder="0"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="no"></iframe>
</body>
</html>